using AppLogin.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Stripe;
using WebHotel_vesion1._0.ViewModel;   
using WebHotel_vesion1._0.Repositories.Interfaces;
using WebHotel_vesion1._0.Enums;
using WebHotel_vesion1._0.Models;

namespace WebHotel_vesion1._0.Controllers
{
    [Authorize]
    public class PagosController : Controller


        
    {

        private readonly ILogger<PagosController> _logger;  
        private  readonly IConfiguration _iconfiguration;
        private readonly IReservaRepository _ireserva;
        private readonly AppDbContext _context;

        public PagosController(
    IConfiguration iconfiguration,
    IReservaRepository ireserva,
    ILogger<PagosController> logger,
    AppDbContext context)
{
    _iconfiguration = iconfiguration;
    StripeConfiguration.ApiKey = _iconfiguration["Stripe:Secretkey"];
    _ireserva = ireserva;
    _logger = logger;
    _context = context;
}
        [Authorize(Roles ="Cliente")]
        public async Task<ActionResult> Pagar(int id) {
            var reservaVM = await _ireserva.BuscarReservacion(id);

            var stripePublicKey = _iconfiguration["Stripe:PublicKey"];
            var stripeSecretKey = _iconfiguration["Stripe:Secretkey"];

            ViewBag.StripePublicKey = stripePublicKey;
            ViewBag.StripeSecretkey = stripeSecretKey;

            return View(reservaVM);
        
        }
        // GET: PagosController
        [Authorize(Roles = "Cliente")]
        [HttpPost]
        public async Task<IActionResult> ProcesarPago([FromBody] PagoRequest pagorequest)
        {
            // 1. Usuario con sesión activa
            var userSesion = User.FindFirst("IdUsuario")?.Value;
            if (string.IsNullOrEmpty(userSesion))
                return Unauthorized();

            // 2. Buscar la reserva
            var reserva = await _ireserva.BuscarReservacion(pagorequest.ReservaId);
            if (reserva == null)
                return NotFound(new { error = "Reserva no encontrada." });

            // 3. Que la reserva sea de este usuario
            if (reserva.UsuarioId != userSesion)
                return Forbid();

            // 4. Que se pueda pagar (no pagada ni cancelada)
            if (reserva.Estado != EstadoReserva.Pendiente)
            {
                return BadRequest(new
                {
                    error = "La reserva no está disponible para pago."
                });
            }

            try
            {
                // 5. El monto SIEMPRE lo calcula el servidor, nunca el cliente
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)Math.Round(reserva.Total * 100m),
                    Currency = "crc",
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true
                    },
                    // 6. La "nota pegada al paquete": el webhook la recibe de vuelta
                    Metadata = new Dictionary<string, string>   
                {
                    ["ReservaId"] = reserva.Id.ToString(),
                    ["UsuarioId"] = userSesion
                }
                };

                // 7. Evita crear dos PaymentIntents si el usuario hace doble clic
                var requestOptions = new RequestOptions
                {
                    IdempotencyKey = $"reserva-{reserva.Id}"
                };

                var service = new PaymentIntentService();
                var intent = await service.CreateAsync(options, requestOptions);

                // 8. Solo devolvemos el clientSecret. NO marcamos nada como pagado aquí.
                return Ok(new { clientSecret = intent.ClientSecret });
            }
            catch (StripeException ex)
            {
                // 9. Detalle al log, mensaje genérico al cliente
                _logger.LogError(ex, "Error de Stripe en reserva {Id}", reserva.Id);
                return BadRequest(new { error = "No se pudo iniciar el pago." });
            }
        }

        // ============================================================
        // ENDPOINT 2: lo llama STRIPE, DESPUÉS de que el pago se aprueba
        // ============================================================
        [AllowAnonymous]          // Stripe no tiene sesión en tu sistema
        [IgnoreAntiforgeryToken]  // por si validás antiforgery globalmente
        [HttpPost("api/stripe/webhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                   _iconfiguration["Stripe:WebhookSecret"],
                    throwOnApiVersionMismatch: false);
            }
            catch (StripeException)
            {
                return BadRequest(); // firma inválida: no es Stripe
            }

            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var pi = (PaymentIntent)stripeEvent.Data.Object;

                if (!pi.Metadata.TryGetValue("ReservaId", out var reservaIdValue) ||
                    !int.TryParse(reservaIdValue, out var reservaId))
                {
                    return Ok();
                }

                if (!pi.Metadata.TryGetValue("UsuarioId", out var usuarioId))
                {
                    return Ok();
                }

                var reserva = await _context.Tb_Reservas
                    .FirstOrDefaultAsync(r => r.Id == reservaId);

                if (reserva == null)
                {
                    _logger.LogWarning(
                        "No se encontró la reserva {ReservaId} para el PaymentIntent {PaymentIntentId}",
                        reservaId,
                        pi.Id);

                    return Ok();
                }

                if (reserva.UsuarioId != usuarioId)
                {
                    _logger.LogWarning(
                        "El usuario del PaymentIntent {PaymentIntentId} no coincide con la reserva {ReservaId}",
                        pi.Id,
                        reservaId);

                    return Ok();
                }

                var pagoExistente = await _context.Pagos
                    .AnyAsync(p => p.StripePaymentIntentId == pi.Id);

                if (pagoExistente)
                {
                    return Ok();
                }

                var montoEsperado = (long)Math.Round(reserva.Total * 100m);

                if (pi.AmountReceived != montoEsperado)
                {
                    _logger.LogError(
                        "El monto del PaymentIntent {PaymentIntentId} no coincide con la reserva {ReservaId}",
                        pi.Id,
                        reservaId);

                    return Ok();
                }

                string? marcaTarjeta = null;
                string? ultimos4 = null;

                if (!string.IsNullOrWhiteSpace(pi.LatestChargeId))
                {
                    var chargeService = new ChargeService();

                    var charge = await chargeService.GetAsync(
                        pi.LatestChargeId,
                        new ChargeGetOptions
                        {
                            Expand = new List<string>
                            {
                                "payment_method_details"
                            }
                        });

                    marcaTarjeta = charge.PaymentMethodDetails?.Card?.Brand;
                    ultimos4 = charge.PaymentMethodDetails?.Card?.Last4;
                }

                var pago = new Pago
                {
                    StripePaymentIntentId = pi.Id,
                    ReservaId = reserva.Id,
                    UsuarioId = reserva.UsuarioId,
                    Monto = pi.AmountReceived / 100m,
                    Moneda = pi.Currency.ToUpperInvariant(),
                    TipoPago = TipoPago.Tarjeta,
                    Estado = EstadoPago.Pagado,
                    MetodoPago = "Tarjeta",
                    MarcaTarjeta = marcaTarjeta,
                    Ultimos4 = ultimos4,
                    FechaCreacion = DateTime.UtcNow,
                    FechaPago = DateTime.UtcNow
                };

                reserva.Estado = EstadoReserva.Confirmada;

                _context.Pagos.Add(pago);

                await _context.SaveChangesAsync();
            }

            return Ok();
        }
      


     
    }
}

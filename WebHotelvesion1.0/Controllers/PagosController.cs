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

        public PagosController(IConfiguration iconfiguration,IReservaRepository ireserva, ILogger<PagosController> logger) {
            _iconfiguration = iconfiguration;
            StripeConfiguration.ApiKey = _iconfiguration["Stripe:Secretkey"];
            _ireserva = ireserva;
            _logger = logger;
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
            if (reserva.Estado)
                return BadRequest(new { error = "La reserva no está disponible para pago." });

            try
            {
                // 5. El monto SIEMPRE lo calcula el servidor, nunca el cliente
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)Math.Round(reserva.Total * 100),
                    Currency = "crc",
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true
                    },
                    // 6. La "nota pegada al paquete": el webhook la recibe de vuelta
                    Metadata = new Dictionary<string, string>
                {
                    { "ReservaId", reserva.Id.ToString() },
                    { "UsuarioId", userSesion }
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

                if (!pi.Metadata.TryGetValue("ReservaId", out var reservaIdStr)
                    || !int.TryParse(reservaIdStr, out var reservaId))
                    return Ok();

                var reserva = await _ireserva.BuscarReservacion(reservaId);
                if (reserva == null) return Ok();

                if (reserva.Estado!=EstadoPago.Pendiente) return Ok(); // ya procesado

                var esperado = (long)Math.Round(reserva.Total * 100);
                if (pi.AmountReceived != esperado)
                {
                    _logger.LogError("Monto no coincide en reserva {Id}", reservaId);
                    return Ok();
                }

                reserva.Estado = EstadoPago.Pagado;
                reserva.PaymentIntentId = pi.Id;
                await _ireserva.ActualizarReservacion(reserva); // ajustá al nombre real
            }

            return Ok();
        }
        userSesion  = User?.FindFirst("IdUsuario")?.Value;
            if (userSesion==null)
                {
                    return BadRequest(new { error = "Usuario no autorizado para realizar este pago." });
            }
            var reservavm= await _ireserva.BuscarReservacion(pagorequest.ReservaId);   

            if(reservavm.UsuarioId != userSesion)
            {
                return BadRequest(new { error = "Usuario no autorizado para realizar este pago." });
            }   
            pagorequest.Monto = reservavm.Total  * 100; // Convertir a centavos

            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long?)pagorequest.Monto, // en centavos
                    Currency = "crc",
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true
                    }
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);


                if(paymentIntent.Status == "succeeded"  )
                {
                    reservavm.Estado = EstadoPago.Pagado;
                }
                return Json(new { clientSecret = paymentIntent.ClientSecret });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }


        }
      


        // POST: PagosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

       
        
    }
}

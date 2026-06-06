using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebHotel_vesion1._0.Enums;
using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.Repositories.Interfaces;
using WebHotel_vesion1._0.ViewModels;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace WebHotel_vesion1._0.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        // GET: ReservasController

        private readonly IReserva _ireserva;
        private readonly ILogger<ReservasController> _logger;

        public ReservasController(IReserva ireserva, ILogger<ReservasController> logger)
        {
            _ireserva = ireserva;
            _logger = logger;
        }
        [Authorize(Roles = "Administrador")]
        public ActionResult GetReservas()
        {
            return View();
        }

        // GET: ReservasController/Details/5


        // GET: ReservasController/Create


        [Authorize(Roles ="Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EfectuarReservacion(int Id, DateTime ckeck_in, DateTime ckeck_out, string total)
        {
            if (Id == 0 || ckeck_in == default || ckeck_out == default || ckeck_out <= ckeck_in)
            {
                TempData["Error"] = "Fechas o habitación inválidas.";
                return RedirectToAction("Detalle", "Habitaciones", new { id = Id });
            }

            var usuarioId = User?.FindFirst("IdUsuario")?.Value;
            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Acceso");
            }

            if (!decimal.TryParse(total?.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var totalDecimal))
            {
                TempData["Error"] = "Importe inválido.";
                return RedirectToAction("Detalle", "Habitaciones", new { id = Id });
            }

            try
            {
                var reserva = new Reserva()
                {
                    HabitacionId = Id,
                    UsuarioId = usuarioId,
                    FechadIngreso = ckeck_in,
                    FechaSalida = ckeck_out,
                    MetodoPago = "TARJETA",
                    Estado = EstadoReserva.Pendiente,
                    Total = totalDecimal
                };
                var reservaCreada = await _ireserva.CrearReserva(reserva);

                if (reservaCreada == null)
                {
                    _logger.LogWarning("CrearReserva devolvió null. Usuario:{User} Habitacion:{Room}", usuarioId, Id);
                    TempData["Error"] = "No se pudo crear la reserva. Intente más tarde.";
                    return RedirectToAction("Detalle", "Habitaciones", new { id = Id });
                }

                return RedirectToAction("ConfirmaReservacion", new { id = reservaCreada.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando reserva. Usuario:{User} Habitacion:{Room}", usuarioId, Id);
                TempData["Error"] = "Ocurrió un error inesperado.";
                return RedirectToAction("Detalle", "Habitaciones", new { id = Id });
            }
        }


        [Authorize(Roles ="Cliente")]
        public async Task<ActionResult> ConfirmaReservacion(int id) {
            ReservaVM reservacreada = await _ireserva.BuscarReservacion(id);


            return View(reservacreada);



        }
        [Authorize(Roles = "Cliente")]
        public async Task<ActionResult> MisReservas() {
            string usuarioId = User?.FindFirst("IdUsuario")?.Value;
            try {

                List<ReservaVM> misreservas = await _ireserva.GetReservas(usuarioId);


                return View(misreservas);
            }

            catch (Exception Ex) {

                Console.WriteLine(Ex.ToString());


            }


            return View();


        }

     
        
        
    }
}

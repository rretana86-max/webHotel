using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebHotel_vesion1._0.Enums;
using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.Repositories.Interfaces;
using WebHotel_vesion1._0.ViewModels;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using WebHotel_vesion1._0.Dto;
using WebHotel_vesion1._0.Service;
using WebHotel_vesion1._0.HandleErros.Exceptions;

namespace WebHotel_vesion1._0.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        // GET: ReservasController

        private readonly IReservaService _ireserva;
        private readonly ILogger<ReservasController> _logger;

        public ReservasController(IReservaService ireserva, ILogger<ReservasController> logger)
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
        public async Task<ActionResult> EfectuarReservacion(ReservaDto reservadto)
        {

            string usuarioId = "";
            int reservaid = 0;
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Fechas o habitación inválidas.";
                return RedirectToAction("Detalle", "Habitaciones", new { id = reservadto.HabitacionId });   
            }

           usuarioId = User?.FindFirst("IdUsuario")?.Value;
            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Acceso");
            }
                try {
                    reservaid = await _ireserva.CrearReserva(usuarioId, reservadto);



                    return RedirectToAction("ConfirmaReservacion", new { id = 21 });

                }
                catch (NotFoundException ex) {

                    TempData["Mensaje"] = ex.Message;
                
                
                }
            return RedirectToAction("index", "home");
        }
           
        


        [Authorize(Roles ="Cliente")]
        public async Task<ActionResult> ConfirmaReservacion(int id) {

            if (id == 0) {

                return NotFound("id debe ser difrente de 0");
            
            }

            ReservaVM reservacreada= new ReservaVM ();
            try { reservacreada = await _ireserva.BuscarReservacion(id);


            }

            catch (NotFoundException ex ) {
                TempData["Mensaje"] = ex.Message.ToString();

                return RedirectToAction("Detalle", "Habitaciones");

            }
            catch (NullReferenceException ex) {

                Console.WriteLine(ex.ToString);
            }
                
             


            return View (reservacreada);



        }
        [Authorize(Roles = "Cliente")]
        public async Task<ActionResult> MisReservas() {
            string usuarioId = User?.FindFirst("IdUsuario")?.Value;// nobtenemos el id del usuario con sesion activa
            List<ReservaVM> misreservas;

            if (String.IsNullOrEmpty(usuarioId.Trim())) {


                return BadRequest("Id inválido");
            }
            try {
              


                misreservas = await _ireserva.GetReservas(usuarioId);



            }
            catch (NotFoundException ex) {

                return NotFound(ex.Message);
            
            }
              


            return View(misreservas);


        }

     
        
        
    }
}

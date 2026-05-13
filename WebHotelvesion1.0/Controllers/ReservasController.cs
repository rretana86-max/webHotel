using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebHotel_vesion1._0.Enums;
using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.Repositories.Interfaces;

namespace WebHotel_vesion1._0.Controllers
{
    public class ReservasController : Controller
    {
        // GET: ReservasController

        private readonly IReserva _ireserva;
  
        public ReservasController(IReserva ireserva )
        {


            _ireserva = ireserva;
           
        }
        public ActionResult GetReservas()
        {
            return View();
        }

        // GET: ReservasController/Details/5
      

        // GET: ReservasController/Create
       

        // POST: ReservasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EfectuarReservacion(int Id,DateTime ckeck_in,DateTime ckeck_out )
        { Reserva r= new Reserva();
           
            try
            {
                // validaciones de seguridad extras
              String UsuarioId_ =Convert.ToString(User.FindFirst("IdUsuario").Value);// almacenamos el id del usuario que esta con sesion activa 

                if ((ckeck_in!=null &&ckeck_out!=null) && (UsuarioId_ != null)&&(Id!=0)) {

                    // creamos la reserva
                    Reserva reserva = new Reserva()
                    {

                        HabitacionId = Id,
                        UsuarioId = UsuarioId_,
                        FechadIngreso = ckeck_in,
                        FechaSalida = ckeck_out,
                        MetodoPago = "TARJETA",
                        Estado = EstadoReserva.Pendiente


                    };

                    await _ireserva.CrearReserva(reserva);



                }


                // llamamos al metodo para guardar la reservacion del usuario
                return RedirectToAction("index","Home");
            }
            catch
            {
                return View();
            }
        }

        // GET: ReservasController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReservasController/Edit/5
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

        // GET: ReservasController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReservasController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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

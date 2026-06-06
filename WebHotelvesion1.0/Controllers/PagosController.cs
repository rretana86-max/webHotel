using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Stripe;
using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.Repositories.Interfaces;

namespace WebHotel_vesion1._0.Controllers
{
    [Authorize]
    public class PagosController : Controller


        
    {


        private  readonly IConfiguration _iconfiguration;
        private readonly IReserva _ireserva;

        public PagosController(IConfiguration iconfiguration,IReserva ireserva) {
            _iconfiguration = iconfiguration;
            StripeConfiguration.ApiKey = _iconfiguration["Stripe:Secretkey"];
            _ireserva = ireserva;
 
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
        public async Task <IActionResult> ProcesarPago([FromBody]PagoRequest pago)
        {
            // validamos ese usuario  si exista y no sea alguien se este pasando de listo 
            string userSesion  = User?.FindFirst("IdUsuario")?.Value;
            if (userSesion==null)
                {
                    return BadRequest(new { error = "Usuario no autorizado para realizar este pago." });
            }
            var reservavm= await _ireserva.BuscarReservacion(pago.ReservaId);   
            pago.Monto = reservavm.Total  * 100; // Convertir a centavos

            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long?)pago.Monto, // en centavos
                    Currency = "crc",
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true
                    }
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                return Json(new { clientSecret = paymentIntent.ClientSecret });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }


        }

        
      

        // GET: PagosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
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

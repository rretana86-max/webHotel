using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using WebHotel_vesion1._0.Models;

namespace WebHotel_vesion1._0.Controllers
{
    public class PagosController : Controller


        
    {


        private  readonly IConfiguration _iconfiguration;

        public PagosController(IConfiguration iconfiguration) {
            _iconfiguration = iconfiguration;
            StripeConfiguration.ApiKey = _iconfiguration["Stripe:Secretkey"];
 
    }


            // GET: PagosController
        public async Task <IActionResult> ProcesarPago([FromBody]PagoRequest pago)
        {
            int monto = 950;

            try
            {
                // Log the incoming payment request
                Console.WriteLine("Processing payment with PaymentMethodId: " + pago.PaymentMethodId);

                var options = new PaymentIntentCreateOptions
                {

                    Amount = monto,
                    Currency = "crc",
                    PaymentMethod = pago.PaymentMethodId,
                    Confirm = true,
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                // Log the successful payment intent creation
                Console.WriteLine("PaymentIntent created successfully: " + paymentIntent.Id);

                return Json(new { success = true, paymentIntentId = paymentIntent.Id });
            }
            catch (Exception ex)
            {
                // Log the error details
                Console.WriteLine("Error processing payment: " + ex.Message);

                return Json(new { success = false, message = ex.Message });
            }
      
          
        }

        // GET: PagosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PagosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PagosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: PagosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PagosController/Delete/5
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

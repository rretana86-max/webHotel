using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.Repositories.Interfaces;

namespace WebHotel_vesion1._0.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUsuario _iusuario;
        private readonly IHabitacion _habitacion;
        //

       private static int totalUsuarios_=0;
        private static int habitacionesDisponibles_ = 0;
        private static int habitacionesOcupadas_ = 0;
        public HomeController(ILogger<HomeController> logger, IHabitacion habitacion,IUsuario usuario)
        {
            _logger = logger;
            _habitacion = habitacion; 
            _iusuario = usuario;
        }

        [Authorize(Roles = "Administrador,Empleado")]
        public async Task<ActionResult> Dasboard()
        {
            var listusuarios = await _iusuario.getAll();
            var listhabitaciones = await _habitacion.ListarHabitaciones();

            var model = new DashboardViewModel
            {
                TotalUsuarios = listusuarios?.Count ?? 0,
                HabitacionesDisponibles = listhabitaciones?.Count(h => h.EstaDisponible) ?? 0,
                HabitacionesOcupadas = listhabitaciones?.Count(h => !h.EstaDisponible) ?? 0,
                ReservasHoy = 0
            };

            return View(model);
        }

        public async Task<IActionResult> ObtenerDatos()
        {
            var listusuarios = await _iusuario.getAll();
            var listhabitaciones = await _habitacion.ListarHabitaciones();

            var totalUsuarios = listusuarios?.Count ?? 0;
            var habitacionesDisponibles = listhabitaciones?.Count(h => h.EstaDisponible) ?? 0;
            var habitacionesOcupadas = listhabitaciones?.Count(h => !h.EstaDisponible) ?? 0;

            var datos = new
            {
                etiquetas = new[] { "Usuarios", "Hab. Disponibles", "Hab. Ocupadas" },
                valores = new[] { totalUsuarios, habitacionesDisponibles, habitacionesOcupadas }
            };
            return  Json(datos);

        }


        [AllowAnonymous]
        public   async Task<IActionResult> Index()
        {
            List<Habitacion> listhabitaciones = await _habitacion.ListarHabitaciones();

            return View(listhabitaciones);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

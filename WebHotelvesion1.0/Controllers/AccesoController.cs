using AppLogin.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.Repositories.Interfaces;
using WebHotel_vesion1._0.Service;

namespace WebHotel_vesion1._0.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IAuth _auth;
     

        public AccesoController(IAuth auth)
        {
            _auth = auth;    
            
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            ViewBag.Mensaje = "";
            return View();


        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string Correo,string Clave)
        {
            string Rol = "";

            
            var user = new Usuario();
            user = await _auth.Login(Correo, Clave);
            if (user != null) {// se codifica lo relacionado con el usario para obtener datos que utilizaremos mas adelante para hacer validaciones 
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name,user.NombreCompleto),
                       new Claim("Correo",user.Correo),
                       new Claim("IdUsuario",user.IdUsuario)
                     };

                foreach (var rol in user.UsuarioRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, rol.Rol.Nombre));

                    Rol = rol.Rol.Nombre;


                }
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // se redirecciona al controlador dependiendo del rol 
                if (Rol.Equals("Administrador") || Rol.Equals("Empleado"))
                {
                    TempData["WelcomeMessage"] = $"Bienvenido {user.NombreCompleto}";

                    return RedirectToAction("Dasboard", "Home");


                }
                else if (Rol.Equals("Cliente")) {

                    return RedirectToAction("index", "Home");
                
                }

            }
            ViewBag.Mensaje = "Usuario o contrasenia incorrectos";

            return  View("Login");


        }

        //cierra la sesion del usuario
        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


            return RedirectToAction("Login", "Acceso");
        }
    }
}

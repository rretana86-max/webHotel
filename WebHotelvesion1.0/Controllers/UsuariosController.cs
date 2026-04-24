using AppLogin.Data;

using DocumentFormat.OpenXml.Office2019.Drawing.Model3D;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.Models.ViewModel;
using WebHotel_vesion1._0.Repositories.Interfaces;
using   BC =BCrypt.Net.BCrypt;
namespace WebHotel_vesion1._0.Controllers
{
    [Authorize]
   
    public class UsuariosController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IUsuario _iusuario;
        private readonly IRol _irol;
        private readonly IUsuarioRol _usuarioRol;    
        public UsuariosController(IWebHostEnvironment hostingEnvironment, IUsuario isuario,IRol irol, IUsuarioRol usuarioRol) {

            _hostingEnvironment = hostingEnvironment;
            _iusuario = isuario;
            _irol = irol;
            _usuarioRol = usuarioRol;
        }

        //C:\CursosWeb\WebHotel'vesion1.0\WebHotel'vesion1.0\Views\Usuarios\
        // GET: EmpleadosController
        [Authorize(Roles ="Administrador ,Empleado")]
        public async Task<ActionResult> listar_Empleados() {
            List<Usuario> listempleados = new List<Usuario>();


            listempleados = await _iusuario .getAll();


            return View(listempleados);
        }

        // GET: EmpleadosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EmpleadosController/Create
        [Authorize(Roles = "Administrador,Empleado")]
     
        public async Task<IActionResult> Create()
        {
            var usuariorol = new UsuarioViewModel {


                Roles = await  _irol.GetRols() 

        };


            return  View(usuariorol);
        }





        // POST: EmpleadosController/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioViewModel user, IFormFile Imagen)
        {
            if (user == null || Imagen == null)
            {
                TempData["Status"] = 0;
                return RedirectToAction("Create");
            }

            var profile = Path.Combine(_hostingEnvironment.WebRootPath, "profile");
            if (!Directory.Exists(profile))
            {
                Directory.CreateDirectory(profile);
            }

            var fileExtension = Path.GetExtension(Imagen.FileName);
            if (string.IsNullOrEmpty(fileExtension)) fileExtension = ".jpg";
            var filename = Guid.NewGuid().ToString() + fileExtension;
            var filepath = Path.Combine(profile, filename);

            using (var filestream = new FileStream(filepath, FileMode.Create))
            {
                await Imagen.CopyToAsync(filestream);
            }

            var usuario = new Usuario
            {
                IdUsuario = user.IdUsuario,
                NombreCompleto = user.NombreCompleto,
                Correo = user.Correo,
                Clave = BC.HashPassword(user.Clave),
                ImageUrl = Path.Combine("profile", filename).Replace("\\", "/")
            };

            int valor = (int)await _iusuario.Create(usuario);
            TempData["Status"] = valor;

            if (valor == 1)
            {
                var userrol = new UsuarioRol { IdUsuario = user.IdUsuario, IdRol = user.IdRol };
                await _usuarioRol.InsertUserRol(userrol);
            }

            return RedirectToAction("Create");
        }

        // GET: EmpleadosController/Edit/5
        // metodo edit para seleccionar mediante el id usuario 
        public async  Task< ActionResult> Edit(string  id)

        {
            var id_ = id;
            var user = await _iusuario.getUser(id);
            List<Rol> listroles = await _irol.GetRols();
            var usuariorol = new UsuarioViewModel
            {
                IdUsuario = user.IdUsuario,
                NombreCompleto = user.NombreCompleto,
                Correo = user.Correo,
                Clave = "",
                imageUrl = user.ImageUrl,
                Roles = listroles

            };
            return View(usuariorol);
        }

        // POST: EmpleadosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  async Task<IActionResult> Edit( UsuarioViewModel userviewmodel, IFormFile Imagen)
        {

            if (Imagen == null) {
                return Content("Se debe    seleccionar una imagen ");
            }
             
            if (userviewmodel == null) {


                return Content("El modelo no puede ser nulo");

            }

           Usuario olduser = await _iusuario.getUser(userviewmodel.IdUsuario);
            var roles = new UsuarioViewModel
            { // almacenamos los roles en la vista para que en caso de retorno no de null a momento de listar los roles 
                Roles = await _irol.GetRols()


            };
            if (olduser == null) {

                return Content("No se encontro el registro en el sistema");
            
            }

            try
            {
                string fileExtension = Path.GetExtension(Imagen.FileName);//obtenemos el nombre de la imagen 
                string newFileName = Guid.NewGuid().ToString() + fileExtension;// creamos un nombre unico 

              
                var profile = Path.Combine(_hostingEnvironment.WebRootPath, "profile");  // obtenemos la ruta de la carpeta profile
              
                var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, olduser.ImageUrl);  // obtenemos la ruta de la vieja imagen 

                if (System.IO.File.Exists(oldImagePath)) { 
                
                System.IO.File.Delete(oldImagePath); 
                }

                oldImagePath = oldImagePath.Replace("\\", "/");

                int cont =Directory.GetFiles(profile).Length; 
              
                //asignamos el nombre al nuevo archivo
              

                var filePath = Path.Combine(profile, newFileName);

                // Guardar la nueva imagen
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Imagen.CopyToAsync(fileStream);
                }


              olduser.ImageUrl=  Path.Combine("profile", newFileName).Replace("\\", "/");
                //  carga de las propiedades con los datos del modelo 
                Usuario user = new Usuario
                {

                  IdUsuario = userviewmodel.IdUsuario,
                    NombreCompleto = userviewmodel.NombreCompleto,
                    Correo = userviewmodel.Correo,
                    Clave =  BC.HashPassword( userviewmodel.Clave),// se encripta la clave
                    ImageUrl = olduser.ImageUrl

                };
                //llamada para el metodo de actualizar usuario
                bool result = await _iusuario.UserUpdate(user);
                if (result) { 

                    // llamada al metodo para almacenar el IdUsuario y el IdRol 
                    UsuarioRol usuariorol = new UsuarioRol
                    {

                        IdUsuario = userviewmodel.IdUsuario,
                        IdRol = userviewmodel.IdRol,
                    };

                
                  await   _usuarioRol.UpdateUserRol(usuariorol);
            }
            }
            catch
            {
                throw new Exception("Ha ocurrido un error ");
                return View();
            }
            
           
           // return View();
            return RedirectToAction("listar_Empleados");
        }

        // GET: EmpleadosController/Delete/5
        [Authorize(Roles="Administrador")]
        public async Task<ActionResult> Delete(string  id)
        {
            var user = await _iusuario.getUser(id);
            return View(user);
        }

        // POST: EmpleadosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteUser(string IdUsuario)
        {
           Usuario usuarioExistente = await _iusuario.getUser(IdUsuario);
            if (usuarioExistente != null)
            {


                var profile = Path.Combine(_hostingEnvironment.WebRootPath, "profile");  // obtenemos la ruta de la carpeta profile

                var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, usuarioExistente.ImageUrl);  // obtenemos la ruta de la vieja imagen 

                if (System.IO.File.Exists(oldImagePath))
                {

                    System.IO.File.Delete(oldImagePath);
                }


            }
            else {

                return Content("<h3 class='bg-danger' > No se pudo elimar el usuario</h3>");
            
            }

              bool result =await   _iusuario.Delete(IdUsuario);

                return RedirectToAction("listar_Empleados");
            
         
           
            return View();
        }
        [Authorize(Roles ="Administrador,Empleado")]
        public async Task<IActionResult> MostrarDatosUsuario(string id) {

            if (String.IsNullOrEmpty(id)) {

                return Content("El id no puede estar vacio");
            
            }
            Usuario usuario = await _iusuario.getUser(id);

            //UsuarioViewModel userviewmodel = new UsuarioViewModel
            //{


            //    IdUsuario = usuario.IdUsuario,
            //    NombreCompleto = usuario.NombreCompleto,
            //    Correo = usuario.Correo,
            //    Roles = await _irol.GetRols()

            //};
            return View(usuario);
        
        }
    }
}

[1mdiff --git a/WebHotelvesion1.0/Controllers/HabitacionesController.cs b/WebHotelvesion1.0/Controllers/HabitacionesController.cs[m
[1mindex 52169c8..0f91a54 100644[m
[1m--- a/WebHotelvesion1.0/Controllers/HabitacionesController.cs[m
[1m+++ b/WebHotelvesion1.0/Controllers/HabitacionesController.cs[m
[36m@@ -11,6 +11,7 @@[m [musing WebHotel_vesion1._0.Models;[m
 using WebHotel_vesion1._0.Models.ViewModel;[m
 using WebHotel_vesion1._0.Repositories.Interfaces;[m
 using WebHotel_vesion1._0.Dto;[m
[32m+[m[32musing WebHotel_vesion1._0.Service;[m
 [m
 [m
 namespace WebHotel_vesion1._0.Controllers[m
[36m@@ -20,15 +21,15 @@[m [mnamespace WebHotel_vesion1._0.Controllers[m
     public class HabitacionesController : Controller[m
     {[m
         [m
[31m-        private readonly IHabitacion _ihabitacion;[m
[31m-        private readonly IWebHostEnvironment _hostingEnvironment;[m
[32m+[m[32m        private readonly IHabitacionService _ihabitacion;[m
[32m+[m[41m      [m
 [m
[31m-        public HabitacionesController( IWebHostEnvironment hostingEnvironment, IHabitacion ihabitacion)[m
[32m+[m[32m        public HabitacionesController( IHabitacionService ihabitacion)[m
         {[m
 [m
 [m
 [m
[31m-            _hostingEnvironment = hostingEnvironment;[m
[32m+[m[41m          [m
           [m
             _ihabitacion = ihabitacion;[m
 [m
[36m@@ -71,71 +72,28 @@[m [mnamespace WebHotel_vesion1._0.Controllers[m
         [ValidateAntiForgeryToken][m
         public async Task<ActionResult> Create(HabitacionViewModel habitacion, IFormFile Imagen)[m
         {[m
[31m-            IFormFile file = null;[m
[31m-[m
[31m-            try[m
[32m+[m[32m            if (!ModelState.IsValid)[m
             {[m
[31m-                if (habitacion != null && Imagen != null)[m
[31m-                {[m
[31m-[m
[31m-[m
[31m-[m
[31m-[m
[31m-[m
[31m-[m
[31m-                    file = Imagen;[m
[31m-[m
[31m-                    var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");[m
[31m-[m
[31m-[m
[31m-                    if (!Directory.Exists(uploads))[m
[31m-                    {[m
[31m-[m
[31m-[m
[31m-                        Directory.CreateDirectory(uploads);[m
[31m-[m
[31m-                    }[m
[31m-                    int cont = Directory.GetFiles(uploads).Length;[m
[31m-                    // cambiamos el nombre de la imagen [m
[31m-                    String filename = $"{cont:D2}.jpeg";[m
[31m-[m
[31m-[m
[31m-                    //var filePath = Path.Combine(uploads, file.FileName);[m
[31m-                    //combinamos la ruta con el nuevo nombre[m
[31m-                    var filePath = Path.Combine(uploads, filename);[m
[31m-[m
[31m-[m
[31m-                    //guardamos el archivo[m
[31m-                    using (var fileStream = new FileStream(filePath, FileMode.Create))[m
[31m-                    {[m
[31m-                        await file.CopyToAsync(fileStream);[m
[31m-                    }[m
[31m-[m
[31m-                    Habitacion _habitacion = new Habitacion()[m
[31m-                    {[m
[31m-[m
[31m-[m
[31m-                        Id = habitacion.Id,[m
[31m-[m
[31m-                        Numero = habitacion.Numero,[m
[31m-                        Descripcion = habitacion.Descripcion,[m
[31m-                        EstaDisponible= habitacion.EstaDisponible,[m
[31m-                        Tipo = habitacion.Tipo,[m
[31m-                        PrecioPorNoche = habitacion.PrecioPorNoche,[m
[31m-                        imageUrl = Path.Combine("uploads", filename).Replace("\\", "/").Trim()[m
[32m+[m[32m                return View(habitacion);[m
[32m+[m[32m            }[m
 [m
[31m-                    };[m
[31m-                    _ihabitacion.CrearHabitacion(_habitacion);[m
[32m+[m[32m            if (Imagen == null || Imagen.Length == 0)[m
[32m+[m[32m            {[m
[32m+[m[32m                ModelState.AddModelError("Imagen", "Debe seleccionar una imagen para la habitación.");[m
[32m+[m[32m                return View(habitacion);[m
[32m+[m[32m            }[m
 [m
[31m-                }[m
[32m+[m[32m            try[m
[32m+[m[32m            {[m
[32m+[m[32m                await _ihabitacion.CrearHabitacion(habitacion, Imagen);[m
[32m+[m[32m                TempData["Success"] = "La habitación se registró correctamente.";[m
[32m+[m[32m                return RedirectToAction(nameof(Create));[m
             }[m
[31m-            catch[m
[32m+[m[32m            catch (Exception)[m
             {[m
[31m-                return View();[m
[32m+[m[32m                ModelState.AddModelError(string.Empty, "No se pudo registrar la habitación. Revise los datos e inténtelo nuevamente.");[m
[32m+[m[32m                return View(habitacion);[m
             }[m
[31m-[m
[31m-[m
[31m-            return RedirectToAction("Create");[m
         }[m
 [m
 [m
[36m@@ -152,77 +110,17 @@[m [mnamespace WebHotel_vesion1._0.Controllers[m
         // POST: HabitacionesController/Edit/5[m
         [HttpPost][m
         [ValidateAntiForgeryToken][m
[31m-        public async Task<ActionResult> Edit(Habitacion habitacion, IFormFile Imagen)[m
[32m+[m[32m       public async Task<ActionResult> Edit(Habitacion habitacion, IFormFile Imagen)[m
         {[m
             if (habitacion == null)[m
             {[m
                 return BadRequest("Datos inválidos");[m
             }[m
[31m-[m
[31m-            var habitacionExistente = await _ihabitacion.getHabitacion(habitacion.Id);[m
[31m-[m
[31m-            if (habitacionExistente == null)[m
[31m-            {[m
[31m-                return NotFound("Habitación no encontrada");[m
[31m-            }[m
[31m-[m
[31m-            try[m
[31m-            {[m
[31m-                if (Imagen != null)[m
[31m-                {[m
[31m-                    string FileNameExtension = Path.GetExtension(Imagen.FileName);// obtenemos la extension del archivo[m
[31m-[m
[31m-                    string NewImageName = Guid.NewGuid().ToString() + FileNameExtension;//creamos un nuevo nombre [m
[31m-[m
[31m-                    var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");  //obtiene la  Ruta completa de la carpeta uploads[m
[31m-[m
[31m-[m
[31m-                    if (!string.IsNullOrEmpty(habitacionExistente.imageUrl))    // Eliminar imagen anterior si existe[m
[31m-                    {[m
[31m-                        var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, habitacionExistente.imageUrl);[m
[31m-[m
[31m-                        //oldImagePath = oldImagePath.Replace("\\", "/");[m
[31m-                        if (System.IO.File.Exists(oldImagePath))[m
[31m-                        {[m
[31m-                            System.IO.File.Delete(oldImagePath);[m
[31m-[m
[31m-                        }[m
[31m-                    }[m
[31m-[m
[31m-[m
[31m-                    var filePath = Path.Combine(uploads, NewImageName);[m
[31m-[m
[31m-                    // Guardar la nueva imagen[m
[31m-                    using (var fileStream = new FileStream(filePath, FileMode.Create))[m
[31m-                    {[m
[31m-                        await Imagen.CopyToAsync(fileStream);[m
[31m-                    }[m
[31m-[m
[31m-                    // Guardar la nueva ruta relativa en la base de datos[m
[31m-                    habitacionExistente.imageUrl = Path.Combine("uploads", NewImageName).Replace("\\", "/");[m
[31m-                }[m
[31m-[m
[31m-                // Actualizar otros datos de la habitación[m
[31m-[m
[31m-                habitacionExistente.Numero = habitacion.Numero;[m
[31m-                habitacionExistente.Descripcion = habitacion.Descripcion;[m
[31m-                habitacionExistente.EstaDisponible = habitacion.EstaDisponible;[m
[31m-                habitacionExistente.Tipo = habitacion.Tipo;[m
[31m-                habitacionExistente.PrecioPorNoche = habitacion.PrecioPorNoche;[m
[31m-              [m
[31m-[m
[31m-[m
[31m-                await _ihabitacion.ActualizarHabitacion(habitacionExistente);[m
[31m-[m
[31m-                return RedirectToAction("listarHabitaciones"); // Redirigir al listado de habitaciones[m
[31m-            }[m
[31m-            catch (Exception ex)[m
[31m-            {[m
[31m-                ModelState.AddModelError("", "Error al actualizar la habitación: " + ex.Message);[m
[31m-                return View(habitacion);[m
[31m-            }[m
[32m+[m[32m           await _ihabitacion.ActualizarHabitacion(habitacion,Imagen);[m
[32m+[m[32m          // viewData["Success"] = "La habitación se actualizó correctamente.";[m[41m   [m
[32m+[m[32m           return RedirectToAction(nameof(listarHabitaciones));[m
         }[m
[31m-[m
[32m+[m[41m        [m
         // metodo para ver mas informacion relacionada con la habitacion [m
         [Authorize(Roles="Cliente")][m
         public async Task<IActionResult> Detalle(int id)[m

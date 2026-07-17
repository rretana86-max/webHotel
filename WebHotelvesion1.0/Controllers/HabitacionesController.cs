using DocumentFormat.OpenXml.Office2010.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.IO;
using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.Models.ViewModel;
using WebHotel_vesion1._0.Repositories.Interfaces;


namespace WebHotel_vesion1._0.Controllers
{

    [Authorize]
    public class HabitacionesController : Controller
    {
        
        private readonly IHabitacion _ihabitacion;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HabitacionesController( IWebHostEnvironment hostingEnvironment, IHabitacion ihabitacion)
        {



            _hostingEnvironment = hostingEnvironment;
          
            _ihabitacion = ihabitacion;




        }
        // GET: HabitacionesController
        [Authorize(Roles = "Administrador,Empleado")]
        public async Task<IActionResult> listarHabitaciones()
        {

            var habitaciones =await  _ihabitacion.ListarHabitaciones();
            return View( habitaciones);

        }


        // GET: HabitacionesController/Details/5
        [Authorize(Roles = "Administrador,Empleado")]
        public async Task<ActionResult> Details(int id)
        {
            Habitacion habitaciondetalles = await _ihabitacion.getHabitacion(id);


            return View(habitaciondetalles);

        }


        // GET: HabitacionesController/Create
        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: HabitacionesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(HabitacionViewModel habitacion, IFormFile Imagen)
        {
            IFormFile file = null;

            try
            {
                if (habitacion != null && Imagen != null)
                {






                    file = Imagen;

                    var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");


                    if (!Directory.Exists(uploads))
                    {


                        Directory.CreateDirectory(uploads);

                    }
                    int cont = Directory.GetFiles(uploads).Length;
                    // cambiamos el nombre de la imagen 
                    String filename = $"{cont:D2}.jpeg";


                    //var filePath = Path.Combine(uploads, file.FileName);
                    //combinamos la ruta con el nuevo nombre
                    var filePath = Path.Combine(uploads, filename);


                    //guardamos el archivo
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    Habitacion _habitacion = new Habitacion()
                    {


                        Id = habitacion.Id,

                        Numero = habitacion.Numero,
                        Descripcion = habitacion.Descripcion,
                        Tipo = habitacion.Tipo,
                        PrecioPorNoche = habitacion.PrecioPorNoche,
                        imageUrl = Path.Combine("uploads", filename).Replace("\\", "/").Trim()

                    };
                    _ihabitacion.CrearHabitacion(_habitacion);

                }
            }
            catch
            {
                return View();
            }


            return RedirectToAction("Create");
        }


        // GET: HabitacionesController/Edit/5
        [Authorize(Roles = "Administrador,Empleado")]
        public async Task<ActionResult> Edit(int id)
        {
            Habitacion habitacionupdate = await _ihabitacion.getHabitacion(id);
            return View(habitacionupdate);

        }


        // POST: HabitacionesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Habitacion habitacion, IFormFile Imagen)
        {
            if (habitacion == null)
            {
                return BadRequest("Datos inválidos");
            }

            var habitacionExistente = await _ihabitacion.getHabitacion(habitacion.Id);

            if (habitacionExistente == null)
            {
                return NotFound("Habitación no encontrada");
            }

            try
            {
                if (Imagen != null)
                {
                    string FileNameExtension = Path.GetExtension(Imagen.FileName);// obtenemos la extension del archivo

                    string NewImageName = Guid.NewGuid().ToString() + FileNameExtension;//creamos un nuevo nombre 

                    var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");  //obtiene la  Ruta completa de la carpeta uploads


                    if (!string.IsNullOrEmpty(habitacionExistente.imageUrl))    // Eliminar imagen anterior si existe
                    {
                        var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, habitacionExistente.imageUrl);

                        oldImagePath = oldImagePath.Replace("\\", "/");
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);

                        }
                    }


                    var filePath = Path.Combine(uploads, NewImageName);

                    // Guardar la nueva imagen
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Imagen.CopyToAsync(fileStream);
                    }

                    // Guardar la nueva ruta relativa en la base de datos
                    habitacionExistente.imageUrl = Path.Combine("uploads", NewImageName).Replace("\\", "/");
                }

                // Actualizar otros datos de la habitación

                habitacionExistente.Numero = habitacion.Numero;
                habitacionExistente.Descripcion = habitacion.Descripcion;
                habitacionExistente.Tipo = habitacion.Tipo;
                habitacionExistente.PrecioPorNoche = habitacion.PrecioPorNoche;
            


                await _ihabitacion.ActualizarHabitacion(habitacionExistente);

                return RedirectToAction("listarHabitaciones"); // Redirigir al listado de habitaciones
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar la habitación: " + ex.Message);
                return View(habitacion);
            }
        }

        // metodo para ver mas informacion relacionada con la habitacion 
        [Authorize(Roles="Cliente")]
        public async Task<IActionResult> Detalle(int id)
        {
          

            Habitacion habitacionDetalle = await _ihabitacion.getHabitacion(id);
            return View(habitacionDetalle);

        }




        // GET: HabitacionesController/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            Habitacion habitacion = new Habitacion();
            habitacion = await _ihabitacion.getHabitacion(id);

            return View(habitacion);
        }




        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteHabitacion(int id)
        {
            try
            {
                _ihabitacion.DeleteHabitacion(id);


                return RedirectToAction(nameof(listarHabitaciones));
            }
            catch
            {
                return View();
            }


        }
        [Authorize(Roles = "Administrador,Empleado")]
        public async Task<IActionResult> ReporteHabitaciones()
        {
            var habitaciones = await _ihabitacion.ListarHabitaciones();
            return View(habitaciones);
        }


//  crear exportar  reporte  pdf con QuestPDF
        public async Task<IActionResult> ExportarPDF()
        {
            var habitaciones = await _ihabitacion.ListarHabitaciones();

            var pdf = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    //  TÍTULO
                    page.Header().Column(col =>
                    {
                        col.Item().Text("Reporte de Habitaciones - WebHotel")
                            .FontSize(18)
                            .Bold()
                            .AlignCenter();

                        col.Item().LineHorizontal(1);
                    });

                    // 🔹 TABLA
                    page.Content().PaddingTop(10).Table(table =>
                    {
                        // COLUMNAS
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(60);
                            columns.RelativeColumn();
                            columns.ConstantColumn(90);
                            columns.ConstantColumn(110);
                            columns.RelativeColumn();
                        });

                        // 🔥 HEADER NEGRO (ESTILO WEB)
                        table.Header(header =>
                        {
                            header.Cell().Element(CellHeaderDark).Text("Número");
                            header.Cell().Element(CellHeaderDark).Text("Tipo");
                            header.Cell().Element(CellHeaderDark).Text("Precio");
                            header.Cell().Element(CellHeaderDark).Text("Disponibilidad");
                            header.Cell().Element(CellHeaderDark).Text("Descripción");
                        });

                        // 🔹 DATOS
                        int index = 0;

                        foreach (var h in habitaciones)
                        {
                            var bgColor = index % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;

                            table.Cell().Element(c => CellBody(c).Background(bgColor)).Text(h.Numero);
                            table.Cell().Element(c => CellBody(c).Background(bgColor)).Text(h.Tipo);
                            table.Cell().Element(c => CellBody(c).Background(bgColor))
                                .Text($"₡ {h.PrecioPorNoche:N2}");

                           
                            index++;
                        }
                    });

                    // 🔹 FOOTER
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                        });
                });
            });

            var stream = new MemoryStream();
            pdf.GeneratePdf(stream);

            return File(stream.ToArray(), "application/pdf", "ReporteHabitaciones.pdf");
        }

        // 🔹 ESTILO HEADER
        static IContainer CellHeaderDark(IContainer container)
        {
            return container
                .Background(Colors.Black)
                .Padding(6)
                .AlignCenter()
                .DefaultTextStyle(x => x.FontColor(Colors.White).Bold());
        }
        // 🔹 ESTILO BODY
        static IContainer CellBody(IContainer container)
        {
            return container
                .Padding(5)
                .AlignCenter();
        }
    }
}

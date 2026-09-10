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
using WebHotel_vesion1._0.Dto;
using WebHotel_vesion1._0.Service;


namespace WebHotel_vesion1._0.Controllers
{

    [Authorize]
    public class HabitacionesController : Controller
    {
        
        private readonly IHabitacionService _ihabitacion;
      

        public HabitacionesController( IHabitacionService ihabitacion)
        {



          
          
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
            if (!ModelState.IsValid)
            {
                return View(habitacion);
            }

            if (Imagen == null || Imagen.Length == 0)
            {
                ModelState.AddModelError("Imagen", "Debe seleccionar una imagen para la habitación.");
                return View(habitacion);
            }

            try
            {
                await _ihabitacion.CrearHabitacion(habitacion, Imagen);
                TempData["Success"] = "La habitación se registró correctamente.";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "No se pudo registrar la habitación. Revise los datos e inténtelo nuevamente.");
                return View(habitacion);
            }
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
           await _ihabitacion.ActualizarHabitacion(habitacion,Imagen);
          // viewData["Success"] = "La habitación se actualizó correctamente.";   
           return RedirectToAction(nameof(listarHabitaciones));
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



        //Update  status Room   
        public async Task<IActionResult> UpdateStatus([FromBody]UpdateAvailabilityRequest request)
        {
            var habitacion = await _ihabitacion.getHabitacion(request.Id);
            if (habitacion == null)
            {
                return NotFound();
            }
              
            await _ihabitacion.UpdateAvailabilityRoom(request.Id,request.EstaDisponible);  
            return RedirectToAction(nameof(listarHabitaciones));
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

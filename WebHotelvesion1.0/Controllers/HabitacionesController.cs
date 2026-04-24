using DocumentFormat.OpenXml.Office2010.Excel;
using FastReport;
using FastReport.Export.PdfSimple;
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
        private readonly IConfiguration _iconfiguration;
        private readonly IHabitacion _ihabitacion;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HabitacionesController(IConfiguration iconfigiuration,IWebHostEnvironment hostingEnvironment, IHabitacion ihabitacion) {



            _hostingEnvironment = hostingEnvironment;
            _iconfiguration = iconfigiuration;
            _ihabitacion = ihabitacion; 
           



        }
        // GET: HabitacionesController
        [Authorize(Roles = "Administrador,Empleado")]
        public async Task<IActionResult> listarHabitaciones() {
            
            var habitaciones=_ihabitacion.ListarHabitaciones();  
            return View(await habitaciones);
        
        }


        // GET: HabitacionesController/Details/5
        [Authorize(Roles ="Administrador,Empleado")]
        public  async Task<ActionResult> Details(int id)
        {
            Habitacion  habitaciondetalles = await  _ihabitacion.getHabitacion(id);


            return View(habitaciondetalles);

        }


        // GET: HabitacionesController/Create
        [Authorize(Roles ="Administrador")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: HabitacionesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  async Task<ActionResult> Create(HabitacionViewModel habitacion,IFormFile Imagen)
           {
            IFormFile file =null;

            try 
            {
                if (habitacion != null && Imagen != null) {






                    file = Imagen;

                    var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");


                    if (!Directory.Exists(uploads)) {


                        Directory.CreateDirectory(uploads);
                    
                    }
                    int cont =Directory.GetFiles(uploads).Length;
                    // cambiamos el nombre de la imagen 
                    String filename= $"{ cont:D2}.jpeg";     


                    //var filePath = Path.Combine(uploads, file.FileName);
                    //combinamos la ruta con el nuevo nombre
                    var filePath=Path.Combine(uploads, filename);


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


            return  RedirectToAction("Create");
        }


        // GET: HabitacionesController/Edit/5
        [Authorize(Roles ="Administrador,Empleado")]
        public async Task< ActionResult> Edit(int  id)
        { Habitacion  habitacionupdate = await _ihabitacion.getHabitacion(id);
            return View(habitacionupdate);

        }


        // POST: HabitacionesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task < ActionResult> Edit(Habitacion habitacion, IFormFile Imagen)
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
                habitacionExistente.EstaDisponible = habitacion.EstaDisponible;
             

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
        public async Task<IActionResult> Detalle(int id) {
            var stripePublicKey = _iconfiguration["Stripe:PublicKey"];

            ViewBag.StripePublicKey = stripePublicKey;
      
            Habitacion habitacionDetalle = await _ihabitacion.getHabitacion(id);
            return View(habitacionDetalle);
        
        }




        // GET: HabitacionesController/Delete/5
        [Authorize(Roles ="Administrador")]
        public async Task<IActionResult>  Delete(int  id)
        {
            Habitacion habitacion= new Habitacion();
            habitacion= await _ihabitacion.getHabitacion(id);
            
            return View(habitacion);
        }

       
      
        
        // POST: HabitacionesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteHabitacion(int id )
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

        public async Task<IActionResult> ReporteHabitaciones()
        {
            var habitaciones = await _ihabitacion.ListarHabitaciones();
            return View(habitaciones);
        }



        // accion para exportar el reporte de habitaciones a PDF utilizando FastReport  
        public async Task<IActionResult> ExportarPDF()
        {
            var habitaciones = await _ihabitacion.ListarHabitaciones();
            using var report = new FastReport.Report();

            var dataTable = new System.Data.DataTable("Habitaciones");
            dataTable.Columns.Add("Numero");
            dataTable.Columns.Add("Tipo");
            dataTable.Columns.Add("PrecioPorNoche");
            dataTable.Columns.Add("Disponibilidad");
            dataTable.Columns.Add("Descripcion");

            foreach (var h in habitaciones)
            {
                dataTable.Rows.Add(
                    h.Numero,
                    h.Tipo,
                    h.PrecioPorNoche.ToString("N2"),
                    h.EstaDisponible ? "Disponible" : "No Disponible",
                    h.Descripcion
                );
            }

            var dataSet = new System.Data.DataSet();
            dataSet.Tables.Add(dataTable);
            report.RegisterData(dataSet, "Habitaciones");

            // Página
            var page = new FastReport.ReportPage();
            page.Name = "Page1";
            report.Pages.Add(page);

            // Título
            var titleBand = new FastReport.ReportTitleBand();
            titleBand.Name = "ReportTitle1";
            titleBand.Height = FastReport.Utils.Units.Centimeters * 2;
            page.Bands.Add(titleBand);

            var titleText = new FastReport.TextObject();
            titleText.Name = "Title";
            titleText.Bounds = new System.Drawing.RectangleF(0, 0,
                FastReport.Utils.Units.Centimeters * 19, FastReport.Utils.Units.Centimeters * 1.5f);
            titleText.Text = "Reporte de Habitaciones - WebHotel";
            titleText.HorzAlign = FastReport.HorzAlign.Center;
            titleText.Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold);
            titleBand.Objects.Add(titleText);

            // Banda de datos
            var dataBand = new FastReport.DataBand();
            dataBand.Name = "Data1";
            dataBand.Height = FastReport.Utils.Units.Centimeters * 1;
            dataBand.DataSource = report.GetDataSource("Habitaciones");
            page.Bands.Add(dataBand);

            // Columnas
            float[] widths = { 3, 3, 3, 3, 7 };
            string[] fields = { "Numero", "Tipo", "PrecioPorNoche", "Disponibilidad", "Descripcion" };
            float x = 0;

            foreach (var i in Enumerable.Range(0, fields.Length))
            {
                var col = new FastReport.TextObject();
                col.Name = $"Col{fields[i]}";
                col.Bounds = new System.Drawing.RectangleF(
                    FastReport.Utils.Units.Centimeters * x, 0,
                    FastReport.Utils.Units.Centimeters * widths[i],
                    FastReport.Utils.Units.Centimeters * 1);
                col.Text = $"[Habitaciones.{fields[i]}]";
                col.Font = new System.Drawing.Font("Arial", 9);
                dataBand.Objects.Add(col);
                x += widths[i];
            }

            report.GetDataSource("Habitaciones").Enabled = true;
            report.Prepare();

            System.Diagnostics.Debug.WriteLine($"Páginas generadas: {report.PreparedPages.Count}");

            using var ms = new MemoryStream();
            var pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport();
            report.Export(pdfExport, ms);
            ms.Position = 0;

            var bytes = ms.ToArray();
            if (bytes.Length == 0)
            {
                return Content("El PDF está vacío - problema en el reporte");
            }

            return File(bytes, "application/pdf", "ReporteHabitaciones.pdf");
        }
    }
}

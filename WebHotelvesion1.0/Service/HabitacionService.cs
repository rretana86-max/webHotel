using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.Models.ViewModel;
using WebHotel_vesion1._0.Repositories.Interfaces;

namespace WebHotel_vesion1._0.Service
{
    public class HabitacionService : IHabitacionService
    {

        private readonly IHabitacion _habitacionRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HabitacionService(IHabitacion habitacionRepository, IWebHostEnvironment hostingEnvironment)
        {
            _habitacionRepository = habitacionRepository;

            _hostingEnvironment = hostingEnvironment;
        }   
       

        public  async Task CrearHabitacion(HabitacionViewModel habitacion, IFormFile imageFile)
        {
            try
            {


                IFormFile file = imageFile;
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
                    EstaDisponible = habitacion.EstaDisponible,
                    Tipo = habitacion.Tipo,
                    PrecioPorNoche = habitacion.PrecioPorNoche,
                    imageUrl = Path.Combine("uploads", filename).Replace("\\", "/").Trim()

                };



                await _habitacionRepository.CrearHabitacion(_habitacion);

            }


            catch (IOException ex)
            {
                throw new Exception(ex.ToString());

            }


        }

         


      

        public async  Task<Habitacion> getHabitacion(int id)
        {
            var habitacion = await _habitacionRepository.getHabitacion(id);


            return habitacion;
        }

        public async Task<List<Habitacion>> ListarHabitaciones() // get all rooms 
        {
          return  await _habitacionRepository.ListarHabitaciones();
        }
        public async Task<bool> UpdateAvailabilityRoom(int id, bool estaDisponible)
        {
            var habitacionExist = await _habitacionRepository.getHabitacion(id);
            if (habitacionExist == null) {
                throw new Exception("dont file the room");

            }
            habitacionExist.EstaDisponible = estaDisponible;
            await _habitacionRepository.UpdateAvailabilityRoom();


            return true;
        }

        public async Task<Habitacion> ActualizarHabitacion(Habitacion habitacion, IFormFile imageFile)
        {
           var habitacionExistente = await  _habitacionRepository .getHabitacion(habitacion.Id);

            if (habitacionExistente == null)
            {
                throw new Exception (" Room don't exist in the database ");
            }

            try
            {
                if (imageFile != null)
                {
                    string FileNameExtension = Path.GetExtension(imageFile.FileName);// obtenemos la extension del archivo

                    string NewImageName = Guid.NewGuid().ToString() + FileNameExtension;//creamos un nuevo nombre 

                    var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");  //obtiene la  Ruta completa de la carpeta uploads


                    if (!string.IsNullOrEmpty(habitacionExistente.imageUrl))    // Eliminar imagen anterior si existe
                    {
                        var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, habitacionExistente.imageUrl);

                        //oldImagePath = oldImagePath.Replace("\\", "/");
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);

                        }
                    }


                    var filePath = Path.Combine(uploads, NewImageName);

                    // Guardar la nueva imagen
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    // Guardar la nueva ruta relativa en la base de datos
                    habitacionExistente.imageUrl = Path.Combine("uploads", NewImageName).Replace("\\", "/");
                }

                // Actualizar otros datos de la habitación

                habitacionExistente.Numero = habitacion.Numero;
                habitacionExistente.Descripcion = habitacion.Descripcion;
                habitacionExistente.EstaDisponible = habitacion.EstaDisponible;
                habitacionExistente.Tipo = habitacion.Tipo;
                habitacionExistente.PrecioPorNoche = habitacion.PrecioPorNoche;


                bool response = await _habitacionRepository.ActualizarHabitacion(habitacionExistente);
                if (response == true) return habitacionExistente;

                return null;

                // Redirigir al listado de habitaciones
            }
            catch (Exception ex ){
               
            Console.WriteLine(ex.Message);
            }

            
            var HabitacionNull = new Habitacion();
            return HabitacionNull;
        }


        public async Task<bool> DeleteHabitacion(int id)
        {
              Habitacion  habitacionExistente =  await  _habitacionRepository .getHabitacion(id);

           //Remove image from folder
            
            if (!string.IsNullOrEmpty(habitacionExistente.imageUrl))
            {
                var imagePath = Path.Combine(
                    _hostingEnvironment.WebRootPath,
                    habitacionExistente.imageUrl);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }


            if (habitacionExistente != null)
              {
                 await   _habitacionRepository.DeleteHabitacion(habitacionExistente);
              }
              else
              {
                  throw new Exception("Room don't exist in the database");
              } 

              return true;
        }

    }
}

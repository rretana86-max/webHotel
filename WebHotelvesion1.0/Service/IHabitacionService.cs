using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.Models.ViewModel;

namespace WebHotel_vesion1._0.Service
{
    public interface IHabitacionService
    {

        public Task CrearHabitacion(HabitacionViewModel habitacion, IFormFile imageFile);


        public Task<List<Habitacion>> ListarHabitaciones();
        public Task<Habitacion> getHabitacion(int id);
        public Task<Habitacion> ActualizarHabitacion(Habitacion habitacion,IFormFile imageFile);
        public Task<bool> DeleteHabitacion(int id);

        public Task<bool> UpdateAvailabilityRoom(int id, bool estaDisponible);




    }
}

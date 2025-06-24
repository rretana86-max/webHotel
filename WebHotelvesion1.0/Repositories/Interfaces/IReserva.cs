


using WebHotel_vesion1._0.Models;

namespace WebHotel_vesion1._0.Repositories.Interfaces

   
{
    public interface IReserva { 
   public Task<List<Reserva>> GetReserva();// metodo para obtener las reservas
   public Task<Reserva> CrearReserva(Reserva reserva); // metodo que crea la reservacion
   public Task<Reserva> BuscarReservacion(int id);
  public Task<bool> ActualizarReservacion(Reserva reserva);
   public Task<bool> EliminarReservacion(int id);
        public Task<Habitacion> GetHabitacion(int id);
    }
}




using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.ViewModels;

namespace WebHotel_vesion1._0.Repositories.Interfaces

   
{
    public interface IReserva { 
   public Task<List<ReservaVM>> GetReservas(string id);// metodo para obtener las reservas de un usuario
   public Task<Reserva> CrearReserva(Reserva reserva); // metodo que crea la reservacion
   public Task<ReservaVM> BuscarReservacion(int id);
  public Task<bool> ActualizarReservacion(Reserva reserva);
   public Task<bool> EliminarReservacion(int id);
        public Task<ReservaVM> GetReservaPay(int id); 
    }
}

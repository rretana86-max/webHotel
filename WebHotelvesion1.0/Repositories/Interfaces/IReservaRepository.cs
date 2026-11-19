using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.ViewModel;

namespace WebHotel_vesion1._0.Repositories.Interfaces
{
    public interface IReservaRepository
    {
        Task<List<ReservaVM>> GetReservas(string id);

        Task<Reserva> CrearReserva(Reserva reserva);

        Task<ReservaVM> BuscarReservacion(int id);

        Task<bool> MarcarReservaComoConfirmada(int reservaId);

        Task<bool> EliminarReservacion(int id);

        Task<bool> CancelarReservacion(int id, string usuarioId);

        Task<ReservaVM> GetReservaPay(int id);
    }
}

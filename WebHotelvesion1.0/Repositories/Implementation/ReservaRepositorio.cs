using AppLogin.Data;
using System.Data.Common;
using WebHotel_vesion1._0.Controllers;
using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.Repositories.Interfaces;

namespace WebHotel_vesion1._0.Repositories.Implementation
{
    public class ReservaRepositorio : IReserva
    {  private readonly  AppDbContext _context;
        public ReservaRepositorio(AppDbContext context) {
        
        
        _context = context; 
        }
        public async Task<bool> ActualizarReservacion(Reserva reserva)
        {
            //var ReservaExistente =await _context.
            throw new NotImplementedException();
        }

        public Task<Reserva> BuscarReservacion(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Reserva> CrearReserva(Reserva  nuevareserva){
            try {

                _context.Tb_Reservas.AddAsync(nuevareserva);
                _context.SaveChangesAsync();

            } catch (DbException EX) {
                Console.WriteLine("error al insertar registro");
            
            }
          
            return nuevareserva;
        }

        public Task<bool> EliminarReservacion(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Habitacion> GetHabitacion(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Reserva>> GetReserva()
        {
            throw new NotImplementedException();
        }
    }
}

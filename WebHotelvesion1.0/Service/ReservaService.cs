
using Microsoft.Data.SqlClient;
using WebHotel_vesion1._0.Dto;
using WebHotel_vesion1._0.Enums;
using WebHotel_vesion1._0.HandleErros.Exceptions;
using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.Repositories.Interfaces;
using WebHotel_vesion1._0.ViewModels;

namespace WebHotel_vesion1._0.Service
{
    public class ReservaService : IReservaService
    {

        private readonly IReservaRepository _ireserva;  
        
        public ReservaService(IReservaRepository ireserva)
        {
            _ireserva = ireserva;
        }   
        public Task<bool> ActualizarReservacion(Reserva reserva)
        {
            throw new NotImplementedException();
        }

        public async  Task<ReservaVM> BuscarReservacion(int id)
        {

            if (id <= 0) {


                throw new ArgumentException("El id debe ser mayor a 0");
            
            }
            var reservavm = await _ireserva.BuscarReservacion(id);
            if (reservavm == null) throw new NotFoundException("recurso no encontrado");

            return reservavm;
        }

        public async Task<int > CrearReserva(string  UsuarioId, ReservaDto reservadto)
        {


            try
            {
                var reserva = new Reserva()
                {
                    HabitacionId = reservadto.HabitacionId,
                    UsuarioId = UsuarioId,
                    FechadIngreso = reservadto.ckeck_in,
                    FechaSalida = reservadto.check_out,
                    MetodoPago = "TARJETA",
                    Estado =EstadoReserva.Pendiente,
                    Total = reservadto.Total
                };
                
              var reservacreada=  await _ireserva.CrearReserva(reserva);  
                return reservacreada.Id;    
            }

            catch (Exception ex ) { }



            return 0;
        }

        public Task<bool> EliminarReservacion(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ReservaVM> GetReservaPay(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ReservaVM>> GetReservas(string id)
        {
            var reservavm = new List<ReservaVM>();
            try {
             

               

                  reservavm = await     _ireserva.GetReservas(id);

                if (reservavm == null || reservavm.Count == 0   ) {
                
               return reservavm?? new List<ReservaVM>();    

                }
               

            }
            catch (SqlException ex)
            {
                // transformas excepción técnica en una más entendible
                throw new Exception("Error al acceder a la base de datos", ex);
            }
            return reservavm ;
           
        }
    }
}

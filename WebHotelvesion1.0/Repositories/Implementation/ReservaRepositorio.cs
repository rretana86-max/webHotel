using AppLogin.Data;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using WebHotel_vesion1._0.Controllers;
using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.Repositories.Interfaces;
using WebHotel_vesion1._0.ViewModels;

namespace WebHotel_vesion1._0.Repositories.Implementation
{
    public class ReservaRepositorio : IReserva
    {
        private readonly AppDbContext _context;
        public ReservaRepositorio(AppDbContext context)
        {


            _context = context;
        }
        public async Task<bool> ActualizarReservacion(Reserva reserva)
        {
            //var ReservaExistente =await _context.
            throw new NotImplementedException();
        }

        public async Task<ReservaVM> BuscarReservacion(int id)
        {
            ReservaVM reservavm = new ReservaVM();
            try
            {
                reservavm = await _context.Tb_Reservas.Where(r => r.Id == id).Select(r => new ReservaVM
                {
                    Id = r.Id,
                    FechaIngreso = r.FechadIngreso,
                    FechaSalida = r.FechaSalida,
                    NombreUsuario = r.Usuario.NombreCompleto,
                    EmailUsuario = r.Usuario.Correo,
                    HabitacionId = r.HabitacionId,
                    NombreHabitacion = r.Habitacion.Descripcion,
                    PrecioPorNoche = r.Habitacion.PrecioPorNoche,
                    Total = r.Total,
                    Estado= r.Estado.ToString()
                }).FirstOrDefaultAsync();

                return reservavm;
            }

            catch (DbException ex)
            {



            }

            return null;

        }
           // create  reservation 
        public async Task<Reserva> CrearReserva(Reserva nuevareserva)
        {
            try
            {

                _context.Tb_Reservas.AddAsync(nuevareserva);
                await _context.SaveChangesAsync();

            }
            catch (DbException EX)
            {
                Console.WriteLine("error al insertar registro");

            }

            return nuevareserva;
        }

        public Task<bool> EliminarReservacion(int id)
        {
            throw new NotImplementedException();
        }

        // search a book  by id 
        public async Task<ReservaVM> GetReservaPay(int id)
        {  var reserva = await _context.Tb_Reservas.Where(r => r.Id == id).Select(r => new ReservaVM
            {
                Id = r.Id,
                FechaIngreso = r.FechadIngreso,
                FechaSalida = r.FechaSalida,
                NombreUsuario = r.Usuario.NombreCompleto,
                EmailUsuario = r.Usuario.Correo,
                HabitacionId = r.HabitacionId,
                NombreHabitacion = r.Habitacion.Descripcion,
                PrecioPorNoche = r.Habitacion.PrecioPorNoche,
                Total = r.Total,
                Estado=r.Estado.ToString()

            }).FirstOrDefaultAsync();
            return reserva; 
        }

        //Obtiene las reservas de un  cliente especifico
        public async Task<List<ReservaVM>> GetReservas(string id)
        {
            var user_reservas = await   _context.Tb_Reservas.Where(u => u.Usuario.IdUsuario == id).Select(u=> new ReservaVM {
           Id=u.Id,
             NombreHabitacion=u.Habitacion.Descripcion,
             NombreUsuario=u.Usuario.NombreCompleto,
             EmailUsuario=u.Usuario.Correo,
             PrecioPorNoche=u.Habitacion.PrecioPorNoche,
             
             FechaIngreso=u.FechadIngreso,
             FechaSalida=u.FechaSalida,
             Total=u.Total,
             Estado=u.Estado.ToString()

            }).ToListAsync();
            return  user_reservas;
        }
    }
}

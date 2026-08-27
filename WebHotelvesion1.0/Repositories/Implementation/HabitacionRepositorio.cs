using AppLogin.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.Repositories.Interfaces;

namespace WebHotel_vesion1._0.Repositories.Implementation
{
    public class HabitacionRepositorio : IHabitacion

    {
        private readonly AppDbContext _context;

        public HabitacionRepositorio(AppDbContext context)
        {
            _context = context;
        }

       

        public async Task CrearHabitacion(Habitacion habitacion)
        {
            try {
                _context.Habitacion.Add(habitacion);
                _context.SaveChanges();  
            }



            catch { }   
        }


        // update entire room entity , including all properties, even if they are not modified  
        public async Task<bool> ActualizarHabitacion(Habitacion habitacionExistente)
        {
            try
            {

                _context.Entry(habitacionExistente).CurrentValues.SetValues(habitacionExistente);

                await _context.SaveChangesAsync(); 


            }



            catch (SqlException ex)
            {
                Console.WriteLine("Error al establecer conexion con el servidor ");
                return false;
            }
            catch (DbUpdateException ex)
            {

                Console.WriteLine("Error en actualizar la base de datos " + ex.ToString());


            }

            return true;
        }

        public async Task<bool> UpdateAvailabilityRoom(int id, bool disponibilidad)
        {
            try
            {
                var habitacion = _context.Habitacion.FirstOrDefault(e => e.Id == id);
                if (habitacion != null)
                {
                    habitacion.EstaDisponible= disponibilidad;
                    await _context.SaveChangesAsync();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error al establecer conexion con el servidor ");
                return false;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Error en actualizar la base de datos " + ex.ToString());
                return false;
            }
            return true;
        }
        public async  Task<bool> DeleteHabitacion(int id)
        {
            try {
               
                var habitaciondelete =  _context.Habitacion.FirstOrDefault(e => e.Id== id);


                if (habitaciondelete != null) { 

                    _context.Remove(habitaciondelete);

                    _context.SaveChanges();
           
                
                }
             

            }
            catch (SqlException ex ) { }


            return true;
        }

        public async  Task<Habitacion> getHabitacion(int id)

        {   Habitacion  habitacion = _context.Habitacion.FirstOrDefault(e => e.Id == id);
            return   habitacion;
        }

        public async Task<List<Habitacion>> ListarHabitaciones()
        {
            var habitaciones =await  _context.Habitacion.ToListAsync();  
            return habitaciones;    
        }
    }
}

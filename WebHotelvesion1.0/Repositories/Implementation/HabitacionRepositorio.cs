using AppLogin.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.Repositories.Interfaces;
using WebHotel_vesion1._0.ViewModel;

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



            catch (SqlException ex ) {

                Console.WriteLine("Error al establecer conexion con el servidor ");

            }   
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

        public async Task<bool> UpdateAvailabilityRoom()
        {
           try{
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
        public async  Task<bool> DeleteHabitacion(Habitacion habitacion)
        {
            try {
               _context.Habitacion.Remove(habitacion);
                await _context.SaveChangesAsync();


                
             

            }
            catch (SqlException ex ) {




                Console.WriteLine("Error al establecer conexion con el servidor ");
                return false;
             }


            return true;
        }

        public async  Task<Habitacion> getHabitacion(int id)

        {
            //  return await _context.Habitaciones.FindAsync(id);

       Habitacion habitacion = _context.Habitacion.FirstOrDefault(e => e.Id == id);
            return   habitacion;
        }
//get all rooms from database 
        public async Task<List<Habitacion>> ListarHabitaciones()
        {
            var habitaciones =await  _context.Habitacion.ToListAsync();  
            return habitaciones;    
        }

        //functionality to obtain  is aivalable rooms

        public async Task<List<HabitacionViewModel>> getHabitacionDisponible() {


            var disponibles = await _context.Habitacion.Where(d => d.EstaDisponible == true).Select(h => new HabitacionViewModel { 
        Id=h.Id,
        Descripcion=h.Descripcion,  
        PrecioPorNoche=h.PrecioPorNoche,   
        imageUrl=h.imageUrl
            
            
            }).ToListAsync();

            return disponibles;
        
        }

    }
}

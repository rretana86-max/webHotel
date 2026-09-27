using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using WebHotel_vesion1._0.Controllers;
using WebHotel_vesion1._0.Models;

namespace WebHotel_vesion1._0.Repositories.Interfaces
{
    public interface IHabitacion

    {
        public  Task CrearHabitacion(Habitacion habitacion);


        public Task<List<Habitacion>> ListarHabitaciones();
        public Task<Habitacion> getHabitacion(int id);
        public Task <bool>ActualizarHabitacion(Habitacion habitacion );
        public Task<bool> DeleteHabitacion(Habitacion habitacion );

        public Task<bool> UpdateAvailabilityRoom();

    }
}

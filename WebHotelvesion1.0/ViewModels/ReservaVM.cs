namespace WebHotel_vesion1._0.ViewModels
{
 
    public class ReservaVM
    {
        public int Id { get; set; }

        public DateTime FechaIngreso { get; set; }
        public DateTime FechaSalida { get; set; }

        // Datos del usuario (solo lo necesario)
        public string NombreUsuario { get; set; }
        public string EmailUsuario { get; set; }

        // Datos de la habitación
        public int HabitacionId { get; set; }
        public string NombreHabitacion { get; set; }
        public decimal PrecioPorNoche { get; set; }

        // Extras útiles
        public int CantidadNoches { get; set; }
        public decimal Total { get; set; }
       public  Enum Estado { get; set; }
    }
}

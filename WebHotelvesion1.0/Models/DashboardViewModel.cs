namespace WebHotel_vesion1._0.Models
{
    public class DashboardViewModel
    {
        public int TotalUsuarios { get; set; }
        public int HabitacionesDisponibles { get; set; }
        public int HabitacionesOcupadas { get; set; }
        public int ReservasHoy { get; set; } = 0;
    }
}
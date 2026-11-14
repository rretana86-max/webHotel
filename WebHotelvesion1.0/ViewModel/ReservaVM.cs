using WebHotel_vesion1._0.Enums;

namespace WebHotel_vesion1._0.ViewModel
{
    public class ReservaVM
    {
        public int Id { get; set; }

        public DateTime FechaIngreso { get; set; }

        public DateTime FechaSalida { get; set; }

        public string NombreUsuario { get; set; } = null!;

        public string UsuarioId { get; set; } = null!;

        public string EmailUsuario { get; set; } = null!;

        public int HabitacionId { get; set; }

        public string NombreHabitacion { get; set; } = null!;

        public decimal PrecioPorNoche { get; set; }

        public int CantidadNoches { get; set; }

        public decimal Total { get; set; }

        public EstadoReserva Estado { get; set; }
    }
}

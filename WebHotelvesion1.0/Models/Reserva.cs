using WebHotel_vesion1._0.Enums;

namespace WebHotel_vesion1._0.Models
{
    public class Reserva
    {

        public int Id { get; set; }
        public DateTime FechadIngreso { get; set; } // Fecha en que se realiza la reserva
        public DateTime FechaSalida {  get; set;  }
        public string UsuarioId { get; set; } // Cliente que reserva
        public Usuario Usuario { get; set; } // Relación con Usuario
        public int HabitacionId { get; set; } // Habitación reservada
        public Habitacion Habitacion { get; set; } // Relación con Habitación
        public string MetodoPago { get; set; } // Ejemplo: "Tarjeta" o "Físico"

        public decimal Total { get; set; }// talmacena el total a pagar 
        public EstadoReserva  Estado { get; set; } // Estado de confirmación de la reserva
    }
}

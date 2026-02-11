using WebHotel_vesion1._0.Enums;

namespace WebHotel_vesion1._0.Models
{
    public class Pago
    {
        public int Id { get; set; }

        public decimal Monto { get; set; }
        public string Moneda { get; set; } = "crc";

        public TipoPago TipoPago { get; set; }
        public EstadoPago Estado { get; set; }

        // Stripe
        public string StripePaymentIntentId { get; set; } = null!;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaPago { get; set; }

        // Relaciones
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;

        public int ReservaId { get; set; }
        public Reserva Reserva { get; set; } = null!;

    }
}

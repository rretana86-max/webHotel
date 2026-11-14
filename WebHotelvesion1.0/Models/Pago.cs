using WebHotel_vesion1._0.Enums;

namespace WebHotel_vesion1._0.Models
{
    public class Pago
    {
        public int Id { get; set; }

        public decimal Monto { get; set; }

        public string Moneda { get; set; } = "CRC";

        public TipoPago TipoPago { get; set; } = TipoPago.Tarjeta;

        public EstadoPago Estado { get; set; }

        public string StripePaymentIntentId { get; set; } = null!;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaPago { get; set; }

        public string UsuarioId { get; set; } = null!;

        public Usuario Usuario { get; set; } = null!;

        public int ReservaId { get; set; }

        public Reserva Reserva { get; set; } = null!;

        public string? MetodoPago { get; set; }

        public string? Ultimos4 { get; set; }

        public string? MarcaTarjeta { get; set; }
    }
}

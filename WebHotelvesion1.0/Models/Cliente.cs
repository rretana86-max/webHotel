namespace WebHotel_vesion1._0.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Telefono { get; set; } = null!;

        public string? StripeCustomerId { get; set; }

        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }

}

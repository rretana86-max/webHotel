namespace WebHotel_vesion1._0.Models
{
    public class PagoRequest
    {      public  string UserId { get; set; }
        public string PaymentMethodId { get; set; }
        public int ReservaId { get; set; }
        public decimal Monto { get; set; }

    }
}

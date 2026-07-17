using System.ComponentModel.DataAnnotations;

namespace WebHotel_vesion1._0.Dto
{
    public class ReservaDto
    {

        //int Id, DateTime ckeck_in, DateTime ckeck_out, string total
          public int  HabitacionId { get; set; }  
        public int   ReservaId { get; set; }
        [Required(ErrorMessage ="Campo obligatorio")]
        public DateTime ckeck_in { get; set; }
        [Required(ErrorMessage = "Campo obligatorio")]
        public DateTime check_out { get; set; }
        [Required(ErrorMessage = "Monto obligatorio")]
        public decimal Total { get; set; }
    }
}

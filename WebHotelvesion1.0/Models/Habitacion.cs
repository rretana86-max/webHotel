namespace WebHotel_vesion1._0.Models
{


    public class Habitacion
    {
        public int Id { get; set; } // Identificador único
        public string Numero { get; set; } // Número o identificador de la habitación
        public string Tipo { get; set; } // Ejemplo: Individual, Doble, Suite
        public decimal PrecioPorNoche { get; set; } // Precio por noche
     
        public string Descripcion { get; set; } // Descripción adicional de la habitación
        public string imageUrl { get; set; }// campo que guarda la ruta de la imagen 
        public bool EstaDisponible { get; set; }//campo para configurar la disponibilidad de la habitacion 
        public ICollection<Reserva> Reservas { get; set; }

    }




}

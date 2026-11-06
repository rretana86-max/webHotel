using System.ComponentModel.DataAnnotations;
using WebHotel_vesion1._0.Models;

namespace WebHotel_vesion1._0.ViewModel
{
    public class UsuarioViewModel
    {

        [RegularExpression("^([0][1-9]-[0-9]{4}-[0-9]{4})|([0-9]-[0-9]{3}-[0-9]{6})$",
           ErrorMessage = "Formato de cedula  invalido Fisica = 0#-####-####   Juridica =#-###-######")]
        public string IdUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }
        public int IdRol { get; set; }
         public string imageUrl { get; set; }
        public List<Rol> Roles { get; set; }

       
    }
}

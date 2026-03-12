using AppLogin.Data;
using Microsoft.EntityFrameworkCore;
using Stripe.Events;
using WebHotel_vesion1._0.Models;
using BC = BCrypt.Net.BCrypt;
namespace WebHotel_vesion1._0.Repositories.Service
{
    public class AuthService : IAuth
    {

        private readonly AppDbContext _context;
        public AuthService(AppDbContext context) {
        
        _context= context;  
        
        }


        public async Task<Usuario> Login(string email, string password)
        {
           Usuario userAuth =  _context.Usuarios.Include(e=>e.UsuarioRoles).ThenInclude(ur=>ur.Rol).FirstOrDefault(u=>u.Correo==email);
           

            if (userAuth != null)
            {
                string hashpassword = userAuth.Clave;

                if (BC.Verify(password, hashpassword))
                {

                    return userAuth;

                }

            }
            return null;
        }
    }
}

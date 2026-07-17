using AppLogin.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


//using Stripe.Events;
using WebHotel_vesion1._0.Models;
using BC = BCrypt.Net.BCrypt;
namespace WebHotel_vesion1._0.Service
{
    public class AuthService : IAuth
    {

        private readonly AppDbContext _context;
        private ILogger<AuthService> _ilogger;
        public AuthService(AppDbContext context, ILogger<AuthService> logger ) {
        
        _context= context;
            _ilogger = logger;
        }


        public async Task<Usuario> Login(string email, string password)
        {


            try {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) {
                    _ilogger.LogError("Los campos de email y password no deben estar vacios ");


                    return null;
                }

                Usuario userAuth = await _context.Usuarios.Include(e => e.UsuarioRoles).ThenInclude(ur => ur.Rol).FirstOrDefaultAsync(u => u.Correo == email);

                if (userAuth != null)
                {
                    string hashpassword = userAuth.Clave;

                    if (BC.Verify(password, hashpassword))// verifica el password 
                    {

                        return userAuth;

                    }
                    return null;


                }
                

            }
            catch (Exception ex) {

                _ilogger.LogError(ex.Message);
            }


        
            return null;
        }
    }
}

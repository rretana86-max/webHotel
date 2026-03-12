using WebHotel_vesion1._0.Models;

namespace WebHotel_vesion1._0.Repositories.Service
{
    public interface IAuth
    {
        public Task<Usuario> Login(string email, string password);
    }
}

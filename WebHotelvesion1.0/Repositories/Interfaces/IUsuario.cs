using Microsoft.AspNetCore.Mvc;
using WebHotel_vesion1._0.Enums;
using WebHotel_vesion1._0.Models;

namespace WebHotel_vesion1._0.Repositories.Interfaces
{
    public interface IUsuario
    {
        public  Task<UserCreationStatus> Create(Usuario user);
        public Task<bool> UserUpdate(Usuario usuario); 
        public Task<bool> Delete(string id);
        public Task<Usuario> getUser(string id);
      
        public Task<List<Usuario>> getAll();

    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
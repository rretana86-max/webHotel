using AppLogin.Data;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using WebHotel_vesion1._0.Enums;
using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.Repositories.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace WebHotel_vesion1._0.Repositories.Implementation
{
    public class UsuarioRepositorio : IUsuario
    {    // variable of AppDbContext
        private readonly AppDbContext _context;
        // instancia que almacenara el usuario que  se actualizara 
        private  readonly Usuario usuario_;
        private readonly ILogger<UsuarioRepositorio> _logger;


        public UsuarioRepositorio(AppDbContext context ,ILogger<UsuarioRepositorio> loger) {

            _context = context;
            _logger = loger;
            usuario_ = new Usuario();
            
        }
        public async  Task<UserCreationStatus> Create(Usuario usuario)
        {
           
      

            try {
                //valida que el usuario o correo no exista en la base de datos antes de guardar el usuario 

                if (_context.Usuarios.Any(x => x.IdUsuario.Equals(usuario.IdUsuario) || x.Correo == usuario.Correo))
                {
                    return UserCreationStatus.DuplicateEmailOrPassword;
                }


                usuario.FechaRegistro = DateTime.Now;
                usuario.FechaActualizacion = null;
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();

                //return UserCreationStatus.Success;

            }

            catch (SqlException ex)
            {
                _logger.LogError("Duplicado de clave primaria");

                return UserCreationStatus.ErrorConexionString;
            }


           


            return UserCreationStatus.Success;


        }

        
        public async Task<List<Usuario>> getAll()// retorna todos los usuarios 
        {
            var users = await _context.Usuarios
                  .Include(u => u.UsuarioRoles)        // Incluir la relación UsuarioRoles
                  .ThenInclude(ur => ur.Rol)           // Incluir la información del Rol (nombre, id)
                  .ToListAsync();
            return users;
        }

        //kkk

        public async Task<Usuario> getUser(string id)
        {
            
            try
            {
                if (String.IsNullOrEmpty(id)) {
                   _logger.LogError("El id no  debe estar vacio");
                    return null;
                }
                var  user = _context.Usuarios.Include(e => e.UsuarioRoles).ThenInclude(ur => ur.Rol).Where(u => u.IdUsuario == id).FirstOrDefault();
               //int   result = VerificaPassword(user,  id);
           

                return user;
            }
            catch {


                _logger.LogError ("Error en la consulta de la base de datos ");
            
           
            }
            return null;
        }
        //metodo para verificar email y contrasenia al iniciar sesion 
        public async Task<Usuario> getUserInitSesion(string clave, string email)
        {   
            if (String.IsNullOrEmpty(clave) || String.IsNullOrEmpty(email))
            {


                _logger.LogError("The key  or  email is null or empty");
                return null;

            }

            try {
               
                var user_ =  _context.Usuarios.Include(e => e.UsuarioRoles).ThenInclude(ur => ur.Rol) .FirstOrDefault(u =>  u.Correo == email);

               // bool isValid = BC.Verify(email,user_.Clave);
                return  user_;
                // validamos la contrasenia 
                        }
            catch (DbUpdateException ex) {

                _logger.LogError("Error in database ");
            
            
            }
            return null;
           
        }

        public  async Task<bool> UserUpdate(Usuario usuario)
        { 

            try
            {
                var usuarioExistente = await _context.Usuarios.FindAsync(usuario.IdUsuario);
                if (usuarioExistente == null)
                {
                    return false; // Si no existe, no se puede actualizar
                }
                usuario.FechaRegistro = usuarioExistente.FechaRegistro;
                usuario.FechaActualizacion = DateTime.Now;
                 _context.Entry(usuarioExistente).CurrentValues.SetValues(usuario);
              int resultado = await  _context.SaveChangesAsync();

                _logger.LogInformation("Actualizacion exitosa");
                // await  _context.SaveChangesAsync();

                //_context.Entry(habitacionExistente).CurrentValues.SetValues(habitacionExistente);

                //await _context.SaveChangesAsync()


            }

            catch (DbUpdateException ex ) {
                 
                return false;
            
            
            }
            return  true;
        }

        // metodo para eliminar un usuario en especifico

        public async Task<bool> Delete(string id)
        {
            if (String.IsNullOrEmpty(id)) return false;


            try {

                var userdelete =  _context.Usuarios.FirstOrDefault(e => e.IdUsuario == id);
                if (userdelete!=null) {


                   _context.Usuarios.Remove(userdelete);
                   _context.SaveChanges();
                }
              
            
            }


            catch ( Exception ex) { }


            return true;
        }


        // metodo para verificacion de la contrasenia para cuando va actualizar los datos el usuario


        //public int  VerificaPassword(Usuario usuario,string password) {

        //    var result = passwordhasher.VerifyHashedPassword(usuario, usuario.Clave, password);

        //    if (result != PasswordVerificationResult.Success) //  verifica si la comprobacion ha sido  exitosa 
        //    {
        //        return 0;
        //    }

        //    return 1;
        //}
    }
}

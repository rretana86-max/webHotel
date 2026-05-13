using Microsoft.EntityFrameworkCore;
using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.Repositories.Interfaces;

namespace AppLogin.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }



        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Habitacion> Habitacion { get; set; }
        public DbSet<UsuarioRol> UsuarioRols { get; set; }  
        public DbSet<Reserva> Tb_Reservas { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>(tb =>
            {
                tb.HasKey(col => col.IdUsuario);
                tb.Property(col => col.IdUsuario)
                 .ValueGeneratedNever();// no genera campo autoincrementable
                tb.Property(col => col.NombreCompleto).HasMaxLength(50).IsRequired();
                tb.Property(col => col.Correo).HasMaxLength(50).IsRequired();
                tb.Property(col => col.Clave).HasMaxLength(20000).IsRequired();
                tb.Property(col => col.ImageUrl).IsRequired();
            tb.Property(col => col.FechaRegistro) .HasDefaultValueSql("GETDATE()");
                tb.Property(col => col.FechaActualizacion).IsRequired(false); // permite valores null, sin default, sin generación automática

                modelBuilder.Entity<Usuario>().ToTable("tb_Usuarios");
              
            });
         
           

            //configuracion tabla roles
            modelBuilder.Entity<Rol>(tb =>
            {
                tb.HasKey(col => col.IdRol);
                tb.Property(col => col.IdRol).IsRequired()
                   .ValueGeneratedNever();// no genera campo autoincrementable
                tb.Property(col => col.Nombre).HasMaxLength(50).IsRequired();

                tb.Property(col => col.FechaRegistro).HasDefaultValueSql("GETDATE()");
                tb.Property(col => col.FechaActualizacion).IsRequired(false); // permite valores null, sin default, sin generación automática

                modelBuilder.Entity<Rol>().ToTable("tb_Roles");
                //tb.HasData(
                //        new Rol { IdRol = 1, Nombre = "Administrador" },
                //        new Rol { IdRol = 2, Nombre = "Empleado" },
                //        new Rol { IdRol = 3, Nombre = "Cliente" }
                //    );
            });
            //modelBuilder.Entity<Rol>().ToTable("tb_Roles");
            //tb.HasData(
            //        new Rol { IdRol = 1, Nombre = "Administrador" },
            //        new Rol { IdRol = 2, Nombre = "Empleado" },
            //        new Rol { IdRol = 3, Nombre = "Cliente" }
            //    );

            // setup properties UsuarioRol
            modelBuilder.Entity<UsuarioRol>(tb =>
            {
                tb.HasKey(ur => new { ur.IdUsuario, ur.IdRol });

                tb.HasOne(ur => ur.Usuario)
                  .WithMany(u => u.UsuarioRoles)
                  .HasForeignKey(ur => ur.IdUsuario);

                tb.HasOne(ur => ur.Rol)
                  .WithMany(r => r.UsuarioRoles)
                  .HasForeignKey(ur => ur.IdRol);

                tb.ToTable("UsuarioRol");
                
            });
            modelBuilder.Entity<Reserva>(tb =>

            {
                tb.HasKey(r => r.Id);
                tb.Property(r => r.MetodoPago).HasMaxLength(20).IsRequired();
                tb.Property(r => r.FechadIngreso).IsRequired();
                tb.Property(r => r.FechaSalida).IsRequired();
                tb.Property(r => r.Estado).IsRequired();

                tb.HasOne(r => r.Usuario)
                  .WithMany(u => u.Reservas)
                  .HasForeignKey(r => r.UsuarioId);

                tb.HasOne(r => r.Habitacion)
                  .WithMany(h => h.Reservas)
                  .HasForeignKey(r => r.HabitacionId);

                tb.ToTable("tb_Reservas");
            }

            );
            modelBuilder.Entity<Habitacion>(tb => {
                tb.HasKey(h => h.Id);
                tb.Property(h => h.Id);
                tb.Property(h => h.Descripcion).HasMaxLength(200).IsRequired();
                tb.Property(h => h.Numero).IsRequired();
                tb.Property(h => h.Tipo).HasMaxLength(100).IsRequired();
          
                tb.Property(h => h.PrecioPorNoche).HasColumnType("decimal(18,2)").IsRequired();
                tb.Property(h => h.imageUrl).HasMaxLength(200).IsRequired();
                tb.ToTable("tb_Habitaciones");
            });

        }
        public override int SaveChanges()   
        {
            var entidadesModificadas = ChangeTracker.Entries<IFechas>()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entidad in entidadesModificadas)
            {
                entidad.Entity.FechaActualizacion = DateTime.Now;
                
            }

            return  base.SaveChanges();
        }
    }
}


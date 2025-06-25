using AppLogin.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.Repositories.Implementation;
using WebHotel_vesion1._0.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
 .AddCookie(option =>
 {

     option.LoginPath = "/Acceso/Login";
     option.ExpireTimeSpan = TimeSpan.FromMinutes(15);
     option.AccessDeniedPath = "/Home/Privacy";

 }
 );
// inyeccion del contexto de la base de datos 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionPorDefecto")));

// Inyeccion de dependencias de las interfaces y sus clases
builder.Services.AddScoped<IUsuario, UsuarioRepositorio>();
builder.Services.AddScoped<IRol,RolRepositorio>();
builder.Services.AddScoped<IUsuarioRol,UsuarioRolRepositorio>();
builder.Services.AddScoped<IHabitacion, HabitacionRepositorio>();
builder.Services.AddScoped<IReserva,ReservaRepositorio>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=index}/{id?}");

app.Run();

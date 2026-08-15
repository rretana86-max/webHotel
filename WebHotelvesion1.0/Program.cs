using AppLogin.Data;
using FastReport.Utils;
using FastReport;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Stripe;
using WebHotel_vesion1._0.Models;
using WebHotel_vesion1._0.Repositories.Implementation;
using WebHotel_vesion1._0.Repositories.Interfaces;
using System.Linq;
using System.Reflection;
using QuestPDF.Infrastructure;
using WebHotel_vesion1._0.Service;



var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;

//registrar el middware de ecepcion global para capturar errores no manejados en la aplicación 

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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyeccion de dependencias de las interfaces y sus clases
builder.Services.AddScoped<IUsuario, UsuarioRepositorio>();
builder.Services.AddScoped<IRol,RolRepositorio>();
builder.Services.AddScoped<IUsuarioRol,UsuarioRolRepositorio>();
builder.Services.AddScoped<IHabitacion, HabitacionRepositorio>();
builder.Services.AddScoped<IReservaRepository, ReservaRepositorio>();
builder.Services.AddScoped<IReservaService,ReservaService>();
builder.Services.AddScoped<IAuth, AuthService>();
builder.Services.AddFastReport();

// Registrar proveedores de conexión de FastReport de forma robusta:
// antes se llamaba RegisteredObjects.AddConnection(typeof(FastReport.Data.MsSqlDataConnection));
// ese tipo puede no existir en todas las versiones/paquetes; usamos reflexión para localizarlo.
var msSqlType = AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(a =>
    {
        try { return a.GetTypes(); }
        catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null)!; }
        catch { return Array.Empty<Type>(); }
    })
    .FirstOrDefault(t => t.FullName == "FastReport.Data.MsSqlDataConnection"
                      || t.FullName == "FastReport.Data.MSSqlDataConnection"
                      || t.Name == "MsSqlDataConnection"
                      || t.Name == "MSSqlDataConnection");

if (msSqlType != null)
{
    RegisteredObjects.AddConnection(msSqlType);
}
else
{
    // opcional: registrar otras conexiones o loggear
    // Console.WriteLine("FastReport MsSql data connection type not found. Install data provider package if needed.");
}

var app = builder.Build();
app.UseFastReport();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:Secretkey").Get<String>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//app.UseMiddleware<ExceptionMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=index}/{id?}");

app.Run();

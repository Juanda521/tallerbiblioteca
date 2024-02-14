using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using tallerbiblioteca.Context;
using tallerbiblioteca.Services;
using PdfSharp.Drawing;
using PdfSharp.Fonts;

var builder = WebApplication.CreateBuilder(args);
//IMPORTANTE lo utilizamos para no estar creando objetos de tipo service en los controladores
builder.Services.AddScoped<ConfiguracionServices>();
builder.Services.AddScoped<RolServices>();
builder.Services.AddScoped<EjemplarServices>();
builder.Services.AddScoped<LibrosServices>();
builder.Services.AddScoped<UsuariosServices>();
builder.Services.AddScoped<PeticionesServices>();
builder.Services.AddScoped<PrestamosServices>();
builder.Services.AddScoped<EmailServices>();
builder.Services.AddScoped<DevolucionesServices>();
builder.Services.AddScoped<SancionesServices>();
builder.Services.AddScoped<PublicacionesServices>();
builder.Services.AddScoped<BackupService>();
builder.Services.AddScoped<AutoresServices>();
builder.Services.AddScoped<GenerosServices>();
builder.Services.AddScoped<ReservasServices>();

// builder.Services.AddScoped<PeticionesServices>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BibliotecaDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Usuarios/login"; // Ruta de inicio de sesi�n
    options.LogoutPath = "/Usuarios/login"; // Ruta de cierre de sesi�n
    options.AccessDeniedPath = "/Configuracion/AccesoDenegado"; // 
  
});
builder.Services.AddSession();
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
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Libros}/{action=Catalog}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuarios}/{action=Login}/{id?}");

app.Run();

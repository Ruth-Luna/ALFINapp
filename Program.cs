using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Configuración de cultura predeterminada
var defaultCulture = new CultureInfo("es-ES");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

// Registrar servicios
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar servicio de memoria distribuida para la sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Registrar servicio HTTP
builder.Services.AddHttpClient();

// Registrar IHttpContextAccessor para acceso global al HttpContext
builder.Services.AddHttpContextAccessor();

// Registrar automáticamente los servicios que siguen el patrón "DBServices"
var servicesAssembly = typeof(Program).Assembly;

foreach (var serviceType in servicesAssembly.GetTypes()
    .Where(t => t.IsClass && !t.IsAbstract && t.Name.StartsWith("DBServices")))
{
    builder.Services.AddScoped(serviceType);
}

var app = builder.Build();

// Configuración del middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Configuración de manejo de códigos de estado
app.UseStatusCodePagesWithReExecute("/Home/Error404");

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Configuración de rutas predeterminadas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

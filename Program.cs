using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory()) 
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) 
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true) // Agregar archivo local si existe
    .AddEnvironmentVariables();

var defaultCulture = new CultureInfo("es-ES");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient();

builder.Services.AddHttpContextAccessor();

var servicesAssembly = typeof(Program).Assembly;

foreach (var serviceType in servicesAssembly.GetTypes()
    .Where(t => t.IsClass && !t.IsAbstract && t.Name.StartsWith("DBServices")))
{
    builder.Services.AddScoped(serviceType);
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Error404");

app.UseSession();
app.UseHttpsRedirection();

// Configuración para deshabilitar la caché de archivos estáticos
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "no-store, no-cache, must-revalidate, proxy-revalidate");
        ctx.Context.Response.Headers.Append("Expires", "0");
        ctx.Context.Response.Headers.Append("Pragma", "no-cache");
    }
});


app.UseRouting();

app.UseAuthorization();

// Configuración de rutas predeterminadas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

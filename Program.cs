using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.Extensions.FileProviders; // ← Agregar esta línea

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

builder.Services.Configure<Microsoft.AspNetCore.Mvc.Razor.RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Clear();
    options.ViewLocationFormats.Add("~/API/Views/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("~/API/Views/Shared/{0}.cshtml");
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(80);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<MDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();


var assemblies = AppDomain.CurrentDomain.GetAssemblies();
foreach (var assembly in assemblies)
{
    foreach (var serviceType in assembly.GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract && t.Name.StartsWith("DBServices")))
    {
        builder.Services.AddScoped(serviceType);
    }
}

foreach (var assembly in assemblies)
{
    foreach (var serviceType in assembly.GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract && 
                    (t.Name.StartsWith("UseCase") || t.Name.StartsWith("Repository")))) // Filtrar por prefijo
    {
        var interfaceType = serviceType.GetInterfaces().FirstOrDefault();
        if (interfaceType != null)
        {
            builder.Services.AddScoped(interfaceType, serviceType);
        }
    }
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

// Configurar archivos estáticos para que los busque en `API/wwwroot`
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "API", "wwwroot")),
    RequestPath = ""
});

app.UseRouting();

app.UseAuthorization();

// Configuración de rutas predeterminadas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

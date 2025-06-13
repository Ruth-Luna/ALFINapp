using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory()) 
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) 
    .AddEnvironmentVariables();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 524288000; // 500 MB
});

var defaultCulture = new CultureInfo("es-ES");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

builder.Services.AddControllersWithViews();

builder.Services.Configure<Microsoft.AspNetCore.Mvc.Razor.RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Clear();
    options.ViewLocationFormats.Add("~/Views/{1}/{0}.cshtml"); // Directorio estándar
    options.ViewLocationFormats.Add("~/Views/Shared/{0}.cshtml"); // Vistas compartidas
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<MDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);

builder.Services.AddDbContextFactory<MDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);

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

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = ""
});

app.UseRouting();

app.UseAuthorization();

// Configuración de rutas predeterminadas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

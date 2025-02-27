# ALFINapp

Aplicación web para la gestión de clientes y tipificaciones.

## Estructura del Proyecto

```
ALFINapp/
├── Controllers/
│   └── VendedorController.cs
├── Filters/
├── Services/
├── Tests/
│   └── Controllers/
│       └── VendedorControllerTests.cs
│   └── Services/
├── Views/
├── wwwroot/
└── README.md
```

## Requisitos

- .NET 6.0 o superior
- SQL Server
- Visual Studio Code o Visual Studio 2022

## Configuración del Desarrollo

1. Clonar el repositorio:
```bash
git clone https://github.com/Emerzon-Chavez-WyA/ALFINapp.git 
cd ALFINapp
```

2. Restaurar paquetes NuGet:
```bash
dotnet restore
```

3. Ejecutar migraciones de base de datos:
```bash
dotnet ef database update
```

## Pruebas

Para ejecutar las pruebas unitarias:
```bash
dotnet test
```

## Tecnologías Utilizadas

### Backend

- ASP.NET Core MVC 6.0
- Entity Framework Core
- xUnit para pruebas unitarias
- SQL Server 
- LINQ
- Dependency Injection
- Custom Middleware y Filtros

### Frontend

- Razor Pages
- JavaScript
- jQuery
- Bootstrap 5
- HTML5
- CSS3
- AJAX
- SweetAlert2
- DataTables.js

### Herramientas y Patrones

- Git para control de versiones
- Patrón MVC
- Patrón Repository
- Inyección de Dependencias
- Custom Authentication/Authorization
- Session Management

## Características Principales

- Gestión de clientes
- Tipificación de llamadas
- Sistema de derivaciones
- Gestión de usuarios y roles
- Sistema de Tipificaciones entre otros

## Licencia

[Tipo de licencia] - ver archivo LICENSE.md para más detalles
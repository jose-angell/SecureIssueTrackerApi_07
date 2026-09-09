# SecureIssueTrackerApi_07

Descripción
-----------
SecureIssueTrackerApi_07 es una API REST para gestión de incidencias (tickets) desarrollada en C# y ASP.NET Core (TargetFramework: .NET 10). Está diseñada como proyecto de ejemplo para seguir buenas prácticas de separación por capas (Domain, Application, Infrastructure, Controllers, DTOs).

Estado actual
------------
La aplicación implementa las funcionalidades principales de gestión de tickets:
- Crear ticket (Create)
- Obtener ticket por Id (GetById)
- Listado de tickets con filtros (GetAll) — filtros soportados: Title, Description, Status, Priority, CreatedByUserId, AssignedToUserId, FromDate, ToDate
- Actualizar la descripción
- Eliminar ticket
- Transiciones de estado: Open, InProgress, Resolved, Closed
- Asignar ticket a usuario (AssignTo)

Tecnologías
-----------
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core (AppDbContext)

Requisitos
---------
- .NET 10 SDK
- Base de datos configurada según AppDbContext (por defecto configuraciones en Infrastructure)

Cómo ejecutar
--------------
1. Restaurar paquetes y compilar:
   dotnet restore
   dotnet build
2. Ejecutar la API:
   dotnet run --project SecureIssueTrackerApi_07

Notas importantes
-----------------
- El método GetAll soporta filtros y la proyección a DTO; se ha corregido la lógica para aplicar los filtros solo cuando los parámetros están presentes.
- Las validaciones de enums y conversiones desde cliente se realizan server-side (usar Enum.TryParse y Enum.IsDefined cuando proceda).

Contribuir
---------
Pull requests y issues son bienvenidos. Para cambios grandes, abrir primero un issue describiendo la propuesta.

Licencia
--------
Ver LICENSE.txt en el repositorio.

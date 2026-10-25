# Sistema de Control Clínico Veterinario

Proyecto desarrollado en **C# (.NET)** aplicando Programación Orientada a Objetos (POO), arquitectura modular y modelado UML.

---

## 🎥 Demostración en Video
Haz clic en el siguiente enlace para ver la explicación completa de la arquitectura, modelado en BlueJ y ejecución en vivo:

👉 **[Ver Video de la Demostración](https://screenapp.io/app/share/A0gHNuuzynTileI3w8c3a5bn)**

---

## 📁 Estructura del Repositorio
- `src/`: Código fuente modular en C# (.NET).
- `diagramas/`: Diagramas de Casos de Uso y Flujo (`.drawio` e imágenes).
- `BlueJ_Veterinaria/`: Proyecto completo con clases Java y diagrama para BlueJ.

## 🚀 Ejecución del Sistema
```bash
dotnet build
dotnet run
```

## 🐘 Persistencia en PostgreSQL (Npgsql)

### Requisitos
- PostgreSQL en ejecución y herramientas cliente (`pg_dump`) en el `PATH`.
- Cadena de conexión por defecto:
  `Host=localhost;Port=5432;Database=veterinaria_db;Username=postgres;Password=postgres;`
- Para usar otra, defina la variable de entorno `VETERINARIA_DB_CONNECTION`:
  ```bash
  export VETERINARIA_DB_CONNECTION="Host=localhost;Port=5432;Database=veterinaria_db;Username=postgres;Password=MI_CLAVE;"
  ```
- Si `pg_dump` no está en el `PATH`, indique su ruta con `PG_DUMP_PATH`.

Al arrancar, `DatabaseInitializer` crea la base `veterinaria_db` (si no existe) y las tablas
`clientes`, `mascotas` y `consultas` con `CREATE TABLE IF NOT EXISTS`.

### Estructura de la capa de datos
| Carpeta | Contenido |
|---|---|
| `Data/` | `Conexion` (cadena centralizada), `DatabaseInitializer` (DDL), `DbUtil` (mensajes de error) |
| `Data/Repositories/` | `ClienteRepository`, `MascotaRepository`, `ConsultaRepository` (CRUD parametrizado) |
| `Models/` | Entidades persistentes `ClienteModel`, `MascotaModel`, `ConsultaModel` |
| `Views/` | Formularios de consola con grilla, enlazados a los repositorios |
| `Services/` | `BackupService.GenerarRespaldo()` (ejecuta `pg_dump`) |

### Respaldos
- Opción **4. Generar Respaldo** del menú principal.
- Respaldo automático al salir (se omite con `dotnet run -- --sin-respaldo`).
- Archivos: `backups/backup_veterinaria_yyyyMMdd_HHmmss.sql` (carpeta ignorada por git).
- Restauración: `psql -U postgres -d veterinaria_db -f backups/<archivo>.sql`

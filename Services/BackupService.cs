using System.Diagnostics;
using System.Runtime.InteropServices;
using ControlClinicoVeterinario.Data;

namespace ControlClinicoVeterinario.Services;

/// <summary>
/// Genera respaldos (redundancia) de la base de datos ejecutando <c>pg_dump</c>.
/// </summary>
public static class BackupService
{
    /// <summary>Carpeta local donde se almacenan los respaldos.</summary>
    public const string CarpetaRespaldos = "backups";

    /// <summary>Permite indicar la ruta completa de pg_dump si no está en el PATH.</summary>
    public const string VariableRutaPgDump = "PG_DUMP_PATH";

    private static readonly TimeSpan TiempoMaximo = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Ejecuta pg_dump y guarda el archivo <c>backups/backup_veterinaria_yyyyMMdd_HHmmss.sql</c>.
    /// </summary>
    /// <returns>Ruta absoluta del archivo generado.</returns>
    /// <exception cref="InvalidOperationException">Si pg_dump no existe o termina con error.</exception>
    public static string GenerarRespaldo()
    {
        var builder = Conexion.Builder;

        var carpeta = Path.Combine(Directory.GetCurrentDirectory(), CarpetaRespaldos);
        Directory.CreateDirectory(carpeta);

        var archivo = Path.Combine(carpeta, $"backup_veterinaria_{DateTime.Now:yyyyMMdd_HHmmss}.sql");

        var inicio = new ProcessStartInfo
        {
            FileName = ResolverPgDump(),
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        // ArgumentList escapa cada argumento de forma segura (sin concatenar cadenas de shell).
        inicio.ArgumentList.Add("--host=" + (builder.Host ?? "localhost"));
        inicio.ArgumentList.Add("--port=" + builder.Port);
        inicio.ArgumentList.Add("--username=" + builder.Username);
        inicio.ArgumentList.Add("--dbname=" + builder.Database);
        inicio.ArgumentList.Add("--format=plain");
        inicio.ArgumentList.Add("--encoding=UTF8");
        inicio.ArgumentList.Add("--clean");
        inicio.ArgumentList.Add("--if-exists");
        inicio.ArgumentList.Add("--no-password");
        inicio.ArgumentList.Add("--file=" + archivo);

        // La contraseña se pasa por variable de entorno del proceso hijo, nunca por línea de comandos.
        if (!string.IsNullOrEmpty(builder.Password))
        {
            inicio.Environment["PGPASSWORD"] = builder.Password;
        }

        Process proceso;
        try
        {
            proceso = Process.Start(inicio)
                      ?? throw new InvalidOperationException("No se pudo iniciar pg_dump.");
        }
        catch (System.ComponentModel.Win32Exception)
        {
            throw new InvalidOperationException(
                $"No se encontró 'pg_dump'. Instale las herramientas cliente de PostgreSQL, agréguelas al PATH " +
                $"o defina la variable de entorno {VariableRutaPgDump} con la ruta completa del ejecutable.");
        }

        using (proceso)
        {
            // Lectura asíncrona para evitar bloqueos si el búfer de salida se llena.
            var errorTask = proceso.StandardError.ReadToEndAsync();
            var salidaTask = proceso.StandardOutput.ReadToEndAsync();

            if (!proceso.WaitForExit((int)TiempoMaximo.TotalMilliseconds))
            {
                try { proceso.Kill(entireProcessTree: true); } catch { /* ya terminó */ }
                EliminarSiExiste(archivo);
                throw new InvalidOperationException("pg_dump excedió el tiempo máximo de ejecución.");
            }

            Task.WaitAll(errorTask, salidaTask);

            if (proceso.ExitCode != 0)
            {
                EliminarSiExiste(archivo);
                throw new InvalidOperationException(
                    $"pg_dump terminó con código {proceso.ExitCode}: {errorTask.Result.Trim()}");
            }
        }

        if (!File.Exists(archivo) || new FileInfo(archivo).Length == 0)
        {
            EliminarSiExiste(archivo);
            throw new InvalidOperationException("pg_dump no generó contenido en el archivo de respaldo.");
        }

        return archivo;
    }

    private static string ResolverPgDump()
    {
        var configurada = Environment.GetEnvironmentVariable(VariableRutaPgDump);
        if (!string.IsNullOrWhiteSpace(configurada))
        {
            return configurada;
        }

        // En Windows el instalador oficial no siempre agrega PostgreSQL al PATH.
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var baseDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "PostgreSQL");
            if (Directory.Exists(baseDir))
            {
                // Se elige la versión más reciente instalada (carpetas "16", "17", ...).
                var candidato = Directory.GetDirectories(baseDir)
                    .OrderByDescending(d => double.TryParse(Path.GetFileName(d),
                        System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : 0)
                    .Select(d => Path.Combine(d, "bin", "pg_dump.exe"))
                    .FirstOrDefault(File.Exists);
                if (candidato != null)
                {
                    return candidato;
                }
            }
        }

        return "pg_dump";
    }

    private static void EliminarSiExiste(string archivo)
    {
        try
        {
            if (File.Exists(archivo))
            {
                File.Delete(archivo);
            }
        }
        catch (IOException)
        {
            // Si no se puede borrar el archivo parcial no se interrumpe el flujo.
        }
    }
}

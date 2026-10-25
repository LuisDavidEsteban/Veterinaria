using System.Text;
using ControlClinicoVeterinario.Data;
using ControlClinicoVeterinario.Services;
using ControlClinicoVeterinario.Views;

namespace ControlClinicoVeterinario;

public class Program
{
    /// <summary>Argumento opcional para omitir el respaldo automático al salir.</summary>
    private const string ArgSinRespaldo = "--sin-respaldo";

    public static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        var respaldarAlSalir = !args.Contains(ArgSinRespaldo, StringComparer.OrdinalIgnoreCase);

        Consola.Titulo("Sistema de Control Clínico Veterinario");
        if (!InicializarBaseDeDatos())
        {
            return 1;
        }

        try
        {
            MenuPrincipal();
        }
        catch (EntradaFinalizadaException)
        {
            Console.WriteLine();
            Consola.Aviso("Entrada finalizada; cerrando la aplicación.");
        }

        if (respaldarAlSalir)
        {
            Console.WriteLine("Generando respaldo automático antes de salir...");
            GenerarRespaldo();
        }

        Console.WriteLine("Saliendo del sistema...");
        return 0;
    }

    private static bool InicializarBaseDeDatos()
    {
        var b = Conexion.Builder;
        Console.WriteLine($"Conectando a PostgreSQL en {b.Host}:{b.Port}, base '{b.Database}', usuario '{b.Username}'...");
        try
        {
            DatabaseInitializer.Inicializar();
            Consola.Exito("Base de datos lista (tablas verificadas/creadas).");
            return true;
        }
        catch (Exception ex)
        {
            Consola.Error("No se pudo inicializar la base de datos: " + DbUtil.MensajeAmigable(ex));
            Consola.Aviso($"Puede cambiar la cadena de conexión con la variable de entorno {Conexion.VariableEntorno}.");
            return false;
        }
    }

    private static void MenuPrincipal()
    {
        var clientes = new ClienteForm();
        var mascotas = new MascotaForm();
        var consultas = new ConsultaForm();

        while (true)
        {
            Consola.Titulo("Menú Principal");
            Console.WriteLine("1. Clientes / Dueños");
            Console.WriteLine("2. Mascotas");
            Console.WriteLine("3. Consultas / Citas");
            Console.WriteLine("4. Generar Respaldo de la base de datos");
            Console.WriteLine("0. Salir");
            Console.Write("Seleccione una opción: ");

            string opcion;
            try
            {
                opcion = Consola.LeerLinea();
            }
            catch (OperacionCanceladaException)
            {
                continue;
            }

            switch (opcion)
            {
                case "1": clientes.Mostrar(); break;
                case "2": mascotas.Mostrar(); break;
                case "3": consultas.Mostrar(); break;
                case "4": GenerarRespaldo(); break;
                case "0": return;
                default: Consola.Error("Opción no reconocida."); break;
            }
        }
    }

    private static void GenerarRespaldo()
    {
        try
        {
            var archivo = BackupService.GenerarRespaldo();
            Consola.Exito($"Respaldo generado: {archivo}");
        }
        catch (Exception ex)
        {
            Consola.Error("No se pudo generar el respaldo: " + ex.Message);
        }
    }
}

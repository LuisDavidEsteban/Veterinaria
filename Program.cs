namespace ControlClinicoVeterinario;

public class Program
{
    public static void Main(string[] args)
    {
        var gestor = new GestorClinica();

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("===== Sistema de Control Clínico Veterinario =====");
            Console.WriteLine("1. Registrar Dueño");
            Console.WriteLine("2. Registrar Mascota");
            Console.WriteLine("3. Agendar Consulta");
            Console.WriteLine("4. Atender/Completar Consulta");
            Console.WriteLine("5. Ver Historial Clínico de Mascota");
            Console.WriteLine("6. Listar todas las Consultas");
            Console.WriteLine("7. Salir");
            Console.Write("Seleccione una opción: ");
            var opcionTexto = Console.ReadLine();

            if (!int.TryParse(opcionTexto, out int opcion))
            {
                Console.WriteLine("Opción inválida, ingrese un número.");
                continue;
            }

            switch (opcion)
            {
                case 1:
                    RegistrarDueno(gestor);
                    break;
                case 2:
                    RegistrarMascota(gestor);
                    break;
                case 3:
                    AgendarConsulta(gestor);
                    break;
                case 4:
                    AtenderConsulta(gestor);
                    break;
                case 5:
                    VerHistorial(gestor);
                    break;
                case 6:
                    ListarConsultas(gestor);
                    break;
                case 7:
                    Console.WriteLine("Saliendo del sistema...");
                    return;
                default:
                    Console.WriteLine("Opción no reconocida.");
                    break;
            }
        }
    }

    private static void RegistrarDueno(GestorClinica gestor)
    {
        Console.Write("DPI: ");
        var dpi = Console.ReadLine() ?? "";
        Console.Write("Nombre completo: ");
        var nombre = Console.ReadLine() ?? "";
        Console.Write("Teléfono: ");
        var telefono = Console.ReadLine() ?? "";
        Console.Write("Dirección: ");
        var direccion = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(dpi) || string.IsNullOrWhiteSpace(nombre))
        {
            Console.WriteLine("El DPI y el nombre son obligatorios.");
            return;
        }

        if (gestor.BuscarDuenoPorDpi(dpi) != null)
        {
            Console.WriteLine("Ya existe un dueño registrado con ese DPI.");
            return;
        }

        gestor.RegistrarDueno(dpi, nombre, telefono, direccion);
        Console.WriteLine("Dueño registrado exitosamente.");
    }

    private static void RegistrarMascota(GestorClinica gestor)
    {
        Console.Write("DPI del dueño: ");
        var dpi = Console.ReadLine() ?? "";
        var dueno = gestor.BuscarDuenoPorDpi(dpi);
        if (dueno == null)
        {
            Console.WriteLine("No existe un dueño con ese DPI. Registre primero al dueño.");
            return;
        }

        Console.Write("Tipo de mascota (1=Perro, 2=Gato): ");
        if (!int.TryParse(Console.ReadLine(), out int tipo) || (tipo != 1 && tipo != 2))
        {
            Console.WriteLine("Tipo de mascota inválido.");
            return;
        }

        Console.Write("Nombre: ");
        var nombreMascota = Console.ReadLine() ?? "";
        if (string.IsNullOrWhiteSpace(nombreMascota))
        {
            Console.WriteLine("El nombre es obligatorio.");
            return;
        }

        Console.Write("Edad (años): ");
        if (!int.TryParse(Console.ReadLine(), out int edad) || edad < 0)
        {
            Console.WriteLine("Edad inválida.");
            return;
        }

        Console.Write("Raza: ");
        var raza = Console.ReadLine() ?? "";

        Console.Write("Peso (kg): ");
        if (!double.TryParse(Console.ReadLine(), out double peso) || peso <= 0)
        {
            Console.WriteLine("Peso inválido.");
            return;
        }

        if (tipo == 1)
        {
            Console.Write("Tamaño de raza (Pequeño/Mediano/Grande): ");
            var tamanio = Console.ReadLine() ?? "";
            var perro = gestor.RegistrarPerro(nombreMascota, edad, raza, peso, dpi, tamanio);
            Console.WriteLine($"Perro registrado con ID {perro.Id}.");
        }
        else
        {
            Console.Write("¿Es de interior? (S/N): ");
            var respuesta = Console.ReadLine() ?? "";
            var esIndoor = respuesta.Trim().ToUpper() == "S";
            var gato = gestor.RegistrarGato(nombreMascota, edad, raza, peso, dpi, esIndoor);
            Console.WriteLine($"Gato registrado con ID {gato.Id}.");
        }
    }

    private static void AgendarConsulta(GestorClinica gestor)
    {
        Console.Write("ID de la mascota: ");
        if (!int.TryParse(Console.ReadLine(), out int idMascota))
        {
            Console.WriteLine("ID inválido.");
            return;
        }

        Console.WriteLine("Veterinarios disponibles:");
        foreach (var vet in gestor.ListarVeterinarios())
        {
            Console.WriteLine($"- {vet.Colegiado}: {vet.Nombre} ({vet.Especialidad})");
        }

        Console.Write("Código de colegiado del veterinario: ");
        var colegiado = Console.ReadLine() ?? "";

        Console.Write("Motivo de la consulta: ");
        var motivo = Console.ReadLine() ?? "";

        try
        {
            var consulta = gestor.ProgramarConsulta(idMascota, colegiado, motivo, DateTime.Now);
            Console.WriteLine($"Consulta agendada con ID {consulta.IdConsulta}.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void AtenderConsulta(GestorClinica gestor)
    {
        Console.Write("ID de la consulta: ");
        if (!int.TryParse(Console.ReadLine(), out int idConsulta))
        {
            Console.WriteLine("ID inválido.");
            return;
        }

        Console.Write("Diagnóstico: ");
        var diagnostico = Console.ReadLine() ?? "";

        Console.Write("Costo: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal costo) || costo < 0)
        {
            Console.WriteLine("Costo inválido.");
            return;
        }

        try
        {
            gestor.AtenderConsulta(idConsulta, diagnostico, costo);
            Console.WriteLine("Consulta completada exitosamente.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void VerHistorial(GestorClinica gestor)
    {
        Console.Write("ID de la mascota: ");
        if (!int.TryParse(Console.ReadLine(), out int idMascota))
        {
            Console.WriteLine("ID inválido.");
            return;
        }

        var mascota = gestor.BuscarMascotaPorId(idMascota);
        if (mascota == null)
        {
            Console.WriteLine("No existe una mascota con ese ID.");
            return;
        }

        mascota.MostrarFicha();
        Console.WriteLine("--- Historial de consultas ---");

        var historial = gestor.ObtenerHistorialPorMascota(idMascota);
        if (historial.Count == 0)
        {
            Console.WriteLine("Esta mascota no tiene consultas registradas.");
            return;
        }

        foreach (var consulta in historial)
        {
            Console.WriteLine();
            consulta.MostrarResumen();
        }
    }

    private static void ListarConsultas(GestorClinica gestor)
    {
        if (gestor.Consultas.Count == 0)
        {
            Console.WriteLine("No hay consultas registradas.");
            return;
        }

        foreach (var consulta in gestor.Consultas)
        {
            Console.WriteLine();
            consulta.MostrarResumen();
        }
    }
}

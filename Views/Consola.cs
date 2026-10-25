using System.Globalization;

namespace ControlClinicoVeterinario.Views;

/// <summary>Se lanza cuando el usuario cancela la captura de un formulario.</summary>
public class OperacionCanceladaException : Exception
{
    public OperacionCanceladaException() : base("Operación cancelada por el usuario.") { }
}

/// <summary>Se lanza cuando la entrada estándar se cierra (EOF).</summary>
public class EntradaFinalizadaException : Exception
{
    public EntradaFinalizadaException() : base("La entrada estándar fue cerrada.") { }
}

/// <summary>
/// Controles de entrada/salida reutilizables para los formularios de consola.
/// En cualquier campo el usuario puede escribir <c>/c</c> para cancelar la operación.
/// </summary>
public static class Consola
{
    public const string ComandoCancelar = "/c";

    private static readonly string[] FormatosFecha =
    {
        "dd/MM/yyyy HH:mm", "dd/MM/yyyy H:mm", "d/M/yyyy HH:mm", "d/M/yyyy H:mm",
        "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd HH:mm", "yyyy-MM-dd"
    };

    public static string LeerLinea()
    {
        var linea = Console.ReadLine();
        if (linea == null)
        {
            throw new EntradaFinalizadaException();
        }
        linea = linea.Trim();
        if (linea.Equals(ComandoCancelar, StringComparison.OrdinalIgnoreCase))
        {
            throw new OperacionCanceladaException();
        }
        return linea;
    }

    /// <summary>
    /// Lee un texto. Si se proporciona <paramref name="valorActual"/>, presionar Enter lo conserva.
    /// </summary>
    public static string LeerTexto(string etiqueta, bool obligatorio = false, string valorActual = null, int longitudMaxima = 0)
    {
        while (true)
        {
            Console.Write(valorActual != null ? $"{etiqueta} [{valorActual}]: " : $"{etiqueta}: ");
            var valor = LeerLinea();

            if (valor.Length == 0 && valorActual != null)
            {
                return valorActual;
            }
            if (obligatorio && valor.Length == 0)
            {
                Error("Este campo es obligatorio.");
                continue;
            }
            if (longitudMaxima > 0 && valor.Length > longitudMaxima)
            {
                Error($"Máximo {longitudMaxima} caracteres.");
                continue;
            }
            return valor;
        }
    }

    public static int LeerEntero(string etiqueta, int? valorActual = null, int minimo = int.MinValue, int maximo = int.MaxValue)
    {
        while (true)
        {
            Console.Write(valorActual.HasValue ? $"{etiqueta} [{valorActual}]: " : $"{etiqueta}: ");
            var texto = LeerLinea();
            if (texto.Length == 0 && valorActual.HasValue)
            {
                return valorActual.Value;
            }
            if (int.TryParse(texto, NumberStyles.Integer, CultureInfo.InvariantCulture, out var valor)
                && valor >= minimo && valor <= maximo)
            {
                return valor;
            }
            Error(maximo == int.MaxValue
                ? $"Ingrese un número entero mayor o igual a {minimo}."
                : $"Ingrese un número entero entre {minimo} y {maximo}.");
        }
    }

    public static decimal LeerDecimal(string etiqueta, decimal? valorActual = null, decimal minimo = 0, decimal maximo = decimal.MaxValue)
    {
        while (true)
        {
            Console.Write(valorActual.HasValue
                ? $"{etiqueta} [{valorActual.Value.ToString(CultureInfo.InvariantCulture)}]: "
                : $"{etiqueta}: ");
            var texto = LeerLinea();
            if (texto.Length == 0 && valorActual.HasValue)
            {
                return valorActual.Value;
            }
            // Se aceptan tanto punto como coma decimal.
            texto = texto.Replace(',', '.');
            if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out var valor)
                && valor >= minimo && valor <= maximo)
            {
                return valor;
            }
            Error($"Ingrese un número entre {minimo} y {maximo}.");
        }
    }

    public static DateTime LeerFecha(string etiqueta, DateTime? valorActual = null)
    {
        var porDefecto = valorActual ?? DateTime.Now;
        while (true)
        {
            Console.Write($"{etiqueta} (dd/MM/yyyy HH:mm) [{porDefecto:dd/MM/yyyy HH:mm}]: ");
            var texto = LeerLinea();
            if (texto.Length == 0)
            {
                return new DateTime(porDefecto.Year, porDefecto.Month, porDefecto.Day,
                                    porDefecto.Hour, porDefecto.Minute, 0);
            }
            if (DateTime.TryParseExact(texto, FormatosFecha, CultureInfo.InvariantCulture,
                                       DateTimeStyles.None, out var fecha))
            {
                return fecha;
            }
            Error("Formato de fecha inválido. Ejemplo: 24/10/2026 15:30");
        }
    }

    public static bool Confirmar(string pregunta)
    {
        while (true)
        {
            Console.Write($"{pregunta} (s/n): ");
            var r = LeerLinea().ToLowerInvariant();
            if (r is "s" or "si" or "sí") return true;
            if (r is "n" or "no") return false;
        }
    }

    public static void Titulo(string texto)
    {
        Console.WriteLine();
        Escribir(ConsoleColor.Cyan, $"===== {texto} =====");
    }

    public static void Exito(string mensaje) => Escribir(ConsoleColor.Green, "✔ " + mensaje);
    public static void Error(string mensaje) => Escribir(ConsoleColor.Red, "✖ " + mensaje);
    public static void Aviso(string mensaje) => Escribir(ConsoleColor.Yellow, "! " + mensaje);

    private static void Escribir(ConsoleColor color, string mensaje)
    {
        var anterior = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(mensaje);
        Console.ForegroundColor = anterior;
    }
}

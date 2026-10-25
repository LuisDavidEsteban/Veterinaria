namespace ControlClinicoVeterinario.Views;

/// <summary>Define una columna de la grilla: encabezado + función que obtiene el valor.</summary>
public record Columna<T>(string Encabezado, Func<T, string> Valor, int AnchoMaximo = 30);

/// <summary>
/// Grilla de datos para consola (equivalente a un DataGridView): dibuja una tabla con bordes.
/// </summary>
public static class GrillaConsola
{
    public static void Mostrar<T>(string titulo, IReadOnlyList<T> filas, IReadOnlyList<Columna<T>> columnas)
    {
        Console.WriteLine();
        Console.WriteLine($"{titulo} ({filas.Count} registro{(filas.Count == 1 ? "" : "s")})");

        if (filas.Count == 0)
        {
            Console.WriteLine("  (sin registros)");
            return;
        }

        var celdas = filas
            .Select(f => columnas.Select(c => Recortar(c.Valor(f) ?? string.Empty, c.AnchoMaximo)).ToArray())
            .ToList();

        var anchos = columnas
            .Select((c, i) => Math.Max(c.Encabezado.Length, celdas.Max(fila => fila[i].Length)))
            .ToArray();

        var separador = "+" + string.Join("+", anchos.Select(a => new string('-', a + 2))) + "+";

        Console.WriteLine(separador);
        Console.WriteLine("| " + string.Join(" | ", columnas.Select((c, i) => c.Encabezado.PadRight(anchos[i]))) + " |");
        Console.WriteLine(separador);
        foreach (var fila in celdas)
        {
            Console.WriteLine("| " + string.Join(" | ", fila.Select((v, i) => v.PadRight(anchos[i]))) + " |");
        }
        Console.WriteLine(separador);
    }

    private static string Recortar(string valor, int maximo)
    {
        valor = valor.Replace('\n', ' ').Replace('\r', ' ');
        return valor.Length <= maximo ? valor : valor[..(maximo - 1)] + "…";
    }
}

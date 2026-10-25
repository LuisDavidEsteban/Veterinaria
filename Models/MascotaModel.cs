namespace ControlClinicoVeterinario.Models;

/// <summary>
/// Entidad persistente que representa a una mascota en la tabla <c>mascotas</c>.
/// </summary>
public class MascotaModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string Raza { get; set; } = string.Empty;
    public int Edad { get; set; }
    public decimal Peso { get; set; }
    public int ClienteId { get; set; }

    /// <summary>Nombre del dueño (sólo lectura, se llena mediante JOIN al listar).</summary>
    public string NombreCliente { get; set; } = string.Empty;

    public override string ToString() => $"#{Id} - {Nombre} ({Especie})";
}

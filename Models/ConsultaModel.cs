namespace ControlClinicoVeterinario.Models;

/// <summary>
/// Entidad persistente que representa una consulta/cita en la tabla <c>consultas</c>.
/// </summary>
public class ConsultaModel
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string Diagnostico { get; set; } = string.Empty;
    public string Tratamiento { get; set; } = string.Empty;
    public int MascotaId { get; set; }

    /// <summary>Nombre de la mascota (sólo lectura, se llena mediante JOIN al listar).</summary>
    public string NombreMascota { get; set; } = string.Empty;

    public override string ToString() => $"#{Id} - {Fecha:dd/MM/yyyy HH:mm} - {Motivo}";
}

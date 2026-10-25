namespace ControlClinicoVeterinario.Models;

/// <summary>
/// Entidad persistente que representa a un cliente/dueño en la tabla <c>clientes</c>.
/// </summary>
public class ClienteModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;

    public override string ToString() => $"#{Id} - {Nombre}";
}

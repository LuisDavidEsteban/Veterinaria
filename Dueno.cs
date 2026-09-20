namespace ControlClinicoVeterinario;

public class Dueno
{
    public string Dpi { get; set; }
    public string NombreCompleto { get; set; }
    public string Telefono { get; set; }
    public string Direccion { get; set; }

    public Dueno(string dpi, string nombreCompleto, string telefono, string direccion)
    {
        Dpi = dpi;
        NombreCompleto = nombreCompleto;
        Telefono = telefono;
        Direccion = direccion;
    }
}

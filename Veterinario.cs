namespace ControlClinicoVeterinario;

public class Veterinario
{
    public string Colegiado { get; set; }
    public string Nombre { get; set; }
    public string Especialidad { get; set; }

    public Veterinario(string colegiado, string nombre, string especialidad)
    {
        Colegiado = colegiado;
        Nombre = nombre;
        Especialidad = especialidad;
    }
}

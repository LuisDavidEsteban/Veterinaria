namespace ControlClinicoVeterinario;

public class Consulta
{
    public int IdConsulta { get; set; }
    public DateTime Fecha { get; set; }
    public Mascota Mascota { get; set; }
    public Dueno Dueno { get; set; }
    public Veterinario Veterinario { get; set; }
    public string Motivo { get; set; }
    public string Diagnostico { get; set; }
    public decimal Costo { get; set; }
    public EstadoConsulta Estado { get; set; }

    public Consulta(int idConsulta, DateTime fecha, Mascota mascota, Dueno dueno, Veterinario veterinario, string motivo)
    {
        IdConsulta = idConsulta;
        Fecha = fecha;
        Mascota = mascota;
        Dueno = dueno;
        Veterinario = veterinario;
        Motivo = motivo;
        Diagnostico = string.Empty;
        Costo = 0;
        Estado = EstadoConsulta.Activa;
    }

    public void MostrarResumen()
    {
        Console.WriteLine($"Consulta #{IdConsulta} | Fecha: {Fecha:dd/MM/yyyy HH:mm} | Estado: {Estado}");
        Console.WriteLine($"Mascota: {Mascota.Nombre} (ID {Mascota.Id}) | Dueño: {Dueno.NombreCompleto}");
        Console.WriteLine($"Veterinario: {Veterinario.Nombre} ({Veterinario.Especialidad})");
        Console.WriteLine($"Motivo: {Motivo}");
        if (Estado == EstadoConsulta.Completada)
        {
            Console.WriteLine($"Diagnóstico: {Diagnostico}");
            Console.WriteLine($"Costo: Q{Costo}");
        }
    }
}

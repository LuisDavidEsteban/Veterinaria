namespace ControlClinicoVeterinario;

public class GestorClinica
{
    public List<Mascota> Mascotas { get; set; }
    public List<Dueno> Duenos { get; set; }
    public List<Veterinario> Veterinarios { get; set; }
    public List<Consulta> Consultas { get; set; }

    private int siguienteIdMascota;
    private int siguienteIdConsulta;

    public GestorClinica()
    {
        Mascotas = new List<Mascota>();
        Duenos = new List<Dueno>();
        Veterinarios = new List<Veterinario>();
        Consultas = new List<Consulta>();
        siguienteIdMascota = 1;
        siguienteIdConsulta = 1;
        CargarDatosSemilla();
    }

    private void CargarDatosSemilla()
    {
        var dueno1 = new Dueno("1234567890101", "Ana López", "55551234", "Zona 1, Ciudad");
        var dueno2 = new Dueno("2345678901012", "Carlos Méndez", "55555678", "Zona 5, Ciudad");
        Duenos.Add(dueno1);
        Duenos.Add(dueno2);

        var vet1 = new Veterinario("VET001", "Dra. Sofía Ramírez", "Medicina General");
        var vet2 = new Veterinario("VET002", "Dr. Luis Herrera", "Cirugía");
        Veterinarios.Add(vet1);
        Veterinarios.Add(vet2);

        var perro1 = new Perro(siguienteIdMascota++, "Rex", 3, "Labrador", 28.5, dueno1, "Grande");
        var gato1 = new Gato(siguienteIdMascota++, "Michi", 2, "Siamés", 4.2, dueno2, true);
        Mascotas.Add(perro1);
        Mascotas.Add(gato1);

        var consulta1 = new Consulta(siguienteIdConsulta++, DateTime.Now.AddDays(-5), perro1, dueno1, vet1, "Chequeo general");
        consulta1.Diagnostico = "Sano, vacunas al día";
        consulta1.Costo = 150;
        consulta1.Estado = EstadoConsulta.Completada;
        Consultas.Add(consulta1);

        var consulta2 = new Consulta(siguienteIdConsulta++, DateTime.Now.AddDays(-1), gato1, dueno2, vet2, "Revisión de piel");
        Consultas.Add(consulta2);
    }

    public Dueno RegistrarDueno(string dpi, string nombreCompleto, string telefono, string direccion)
    {
        var dueno = new Dueno(dpi, nombreCompleto, telefono, direccion);
        Duenos.Add(dueno);
        return dueno;
    }

    public Dueno BuscarDuenoPorDpi(string dpi)
    {
        foreach (var dueno in Duenos)
        {
            if (dueno.Dpi == dpi)
            {
                return dueno;
            }
        }
        return null;
    }

    public Mascota BuscarMascotaPorId(int id)
    {
        foreach (var mascota in Mascotas)
        {
            if (mascota.Id == id)
            {
                return mascota;
            }
        }
        return null;
    }

    public Veterinario BuscarVeterinarioPorColegiado(string colegiado)
    {
        foreach (var veterinario in Veterinarios)
        {
            if (veterinario.Colegiado == colegiado)
            {
                return veterinario;
            }
        }
        return null;
    }

    public Consulta BuscarConsultaPorId(int id)
    {
        foreach (var consulta in Consultas)
        {
            if (consulta.IdConsulta == id)
            {
                return consulta;
            }
        }
        return null;
    }

    public Perro RegistrarPerro(string nombre, int edad, string raza, double pesoKg, string dpiDueno, string tamanioRaza)
    {
        var dueno = BuscarDuenoPorDpi(dpiDueno);
        if (dueno == null)
        {
            throw new InvalidOperationException("No existe un dueño con ese DPI");
        }

        var perro = new Perro(siguienteIdMascota++, nombre, edad, raza, pesoKg, dueno, tamanioRaza);
        Mascotas.Add(perro);
        return perro;
    }

    public Gato RegistrarGato(string nombre, int edad, string raza, double pesoKg, string dpiDueno, bool esIndoor)
    {
        var dueno = BuscarDuenoPorDpi(dpiDueno);
        if (dueno == null)
        {
            throw new InvalidOperationException("No existe un dueño con ese DPI");
        }

        var gato = new Gato(siguienteIdMascota++, nombre, edad, raza, pesoKg, dueno, esIndoor);
        Mascotas.Add(gato);
        return gato;
    }

    public Consulta ProgramarConsulta(int idMascota, string colegiadoVeterinario, string motivo, DateTime fecha)
    {
        var mascota = BuscarMascotaPorId(idMascota);
        if (mascota == null)
        {
            throw new InvalidOperationException("No existe una mascota con ese ID");
        }

        var veterinario = BuscarVeterinarioPorColegiado(colegiadoVeterinario);
        if (veterinario == null)
        {
            throw new InvalidOperationException("No existe un veterinario con ese código de colegiado");
        }

        var consulta = new Consulta(siguienteIdConsulta++, fecha, mascota, mascota.Dueno, veterinario, motivo);
        Consultas.Add(consulta);
        return consulta;
    }

    public void AtenderConsulta(int idConsulta, string diagnostico, decimal costo)
    {
        var consulta = BuscarConsultaPorId(idConsulta);
        if (consulta == null)
        {
            throw new InvalidOperationException("No existe una consulta con ese ID");
        }

        if (consulta.Estado == EstadoConsulta.Completada)
        {
            throw new InvalidOperationException("La consulta ya fue completada anteriormente");
        }

        consulta.Diagnostico = diagnostico;
        consulta.Costo = costo;
        consulta.Estado = EstadoConsulta.Completada;
    }

    public List<Consulta> ObtenerHistorialPorMascota(int idMascota)
    {
        var historial = new List<Consulta>();
        foreach (var consulta in Consultas)
        {
            if (consulta.Mascota.Id == idMascota)
            {
                historial.Add(consulta);
            }
        }
        historial.Sort((a, b) => a.Fecha.CompareTo(b.Fecha));
        return historial;
    }

    public List<Veterinario> ListarVeterinarios()
    {
        return Veterinarios;
    }

    public List<Mascota> ListarMascotas()
    {
        return Mascotas;
    }
}

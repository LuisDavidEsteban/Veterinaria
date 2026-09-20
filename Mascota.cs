namespace ControlClinicoVeterinario;

public abstract class Mascota
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public int Edad { get; set; }
    public string Especie { get; set; }
    public string Raza { get; set; }
    public double PesoKg { get; set; }
    public Dueno Dueno { get; set; }

    protected Mascota(int id, string nombre, int edad, string especie, string raza, double pesoKg, Dueno dueno)
    {
        Id = id;
        Nombre = nombre;
        Edad = edad;
        Especie = especie;
        Raza = raza;
        PesoKg = pesoKg;
        Dueno = dueno;
    }

    public abstract string EmitirSonido();

    public virtual void MostrarFicha()
    {
        Console.WriteLine($"ID: {Id}");
        Console.WriteLine($"Nombre: {Nombre}");
        Console.WriteLine($"Especie: {Especie}");
        Console.WriteLine($"Raza: {Raza}");
        Console.WriteLine($"Edad: {Edad} años");
        Console.WriteLine($"Peso: {PesoKg} kg");
        Console.WriteLine($"Dueño: {Dueno.NombreCompleto} (DPI: {Dueno.Dpi})");
        Console.WriteLine($"Sonido: {EmitirSonido()}");
    }
}

namespace ControlClinicoVeterinario;

public class Perro : Mascota
{
    public string TamanioRaza { get; set; }

    public Perro(int id, string nombre, int edad, string raza, double pesoKg, Dueno dueno, string tamanioRaza)
        : base(id, nombre, edad, "Perro", raza, pesoKg, dueno)
    {
        TamanioRaza = tamanioRaza;
    }

    public override string EmitirSonido()
    {
        return "Guau Guau";
    }

    public override void MostrarFicha()
    {
        base.MostrarFicha();
        Console.WriteLine($"Tamaño de raza: {TamanioRaza}");
    }
}

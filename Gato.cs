namespace ControlClinicoVeterinario;

public class Gato : Mascota
{
    public bool EsIndoor { get; set; }

    public Gato(int id, string nombre, int edad, string raza, double pesoKg, Dueno dueno, bool esIndoor)
        : base(id, nombre, edad, "Gato", raza, pesoKg, dueno)
    {
        EsIndoor = esIndoor;
    }

    public override string EmitirSonido()
    {
        return "Miau Miau";
    }

    public override void MostrarFicha()
    {
        base.MostrarFicha();
        Console.WriteLine($"Es de interior: {(EsIndoor ? "Sí" : "No")}");
    }
}

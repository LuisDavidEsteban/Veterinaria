using System.Globalization;
using ControlClinicoVeterinario.Data.Repositories;
using ControlClinicoVeterinario.Models;

namespace ControlClinicoVeterinario.Views;

public class MascotaForm : FormularioCrud<MascotaModel>
{
    private readonly MascotaRepository _mascotas = new();
    private readonly ClienteRepository _clientes = new();
    private readonly ConsultaRepository _consultas = new();

    public static readonly IReadOnlyList<Columna<MascotaModel>> ColumnasMascota = new[]
    {
        new Columna<MascotaModel>("Id", m => m.Id.ToString()),
        new Columna<MascotaModel>("Nombre", m => m.Nombre),
        new Columna<MascotaModel>("Especie", m => m.Especie),
        new Columna<MascotaModel>("Raza", m => m.Raza),
        new Columna<MascotaModel>("Edad", m => m.Edad.ToString()),
        new Columna<MascotaModel>("Peso (kg)", m => m.Peso.ToString("0.00", CultureInfo.InvariantCulture)),
        new Columna<MascotaModel>("Dueño", m => $"{m.ClienteId} - {m.NombreCliente}")
    };

    protected override string Titulo => "Gestión de Mascotas";
    protected override string NombreEntidad => "mascota";
    protected override bool EsFemenino => true;
    protected override IRepository<MascotaModel> Repositorio => _mascotas;
    protected override IReadOnlyList<Columna<MascotaModel>> Columnas => ColumnasMascota;

    protected override int ObtenerId(MascotaModel entidad) => entidad.Id;

    protected override MascotaModel CapturarCampos(MascotaModel existente)
    {
        var clientes = _clientes.ObtenerTodos();
        if (clientes.Count == 0)
        {
            throw new InvalidOperationException("Primero debe registrar al menos un cliente (dueño).");
        }

        var m = new MascotaModel { Id = existente?.Id ?? 0 };
        m.Nombre = Consola.LeerTexto("Nombre", obligatorio: true, existente?.Nombre, 100);
        m.Especie = Consola.LeerTexto("Especie (Perro, Gato, Ave...)", obligatorio: true, existente?.Especie, 50);
        m.Raza = Consola.LeerTexto("Raza", valorActual: existente?.Raza, longitudMaxima: 100);
        m.Edad = Consola.LeerEntero("Edad (años)", existente?.Edad, minimo: 0, maximo: 100);
        m.Peso = Consola.LeerDecimal("Peso (kg)", existente?.Peso, minimo: 0, maximo: 9999.99m);

        GrillaConsola.Mostrar("Clientes disponibles", clientes, new[]
        {
            new Columna<ClienteModel>("Id", c => c.Id.ToString()),
            new Columna<ClienteModel>("Nombre", c => c.Nombre),
            new Columna<ClienteModel>("Teléfono", c => c.Telefono)
        });

        while (true)
        {
            m.ClienteId = Consola.LeerEntero("Id del dueño", existente?.ClienteId, minimo: 1);
            if (clientes.Any(c => c.Id == m.ClienteId))
            {
                break;
            }
            Consola.Error("No existe un cliente con ese Id.");
        }
        return m;
    }

    protected override void MostrarDetalle(MascotaModel m)
    {
        Console.WriteLine($"Id:      {m.Id}");
        Console.WriteLine($"Nombre:  {m.Nombre}");
        Console.WriteLine($"Especie: {m.Especie}");
        Console.WriteLine($"Raza:    {m.Raza}");
        Console.WriteLine($"Edad:    {m.Edad} años");
        Console.WriteLine($"Peso:    {m.Peso.ToString("0.00", CultureInfo.InvariantCulture)} kg");
        Console.WriteLine($"Dueño:   {m.NombreCliente} (Id {m.ClienteId})");

        GrillaConsola.Mostrar("Historial clínico", _consultas.ObtenerPorMascota(m.Id), ConsultaForm.ColumnasConsulta);
    }
}

using ControlClinicoVeterinario.Data.Repositories;
using ControlClinicoVeterinario.Models;

namespace ControlClinicoVeterinario.Views;

public class ConsultaForm : FormularioCrud<ConsultaModel>
{
    private readonly ConsultaRepository _consultas = new();
    private readonly MascotaRepository _mascotas = new();

    public static readonly IReadOnlyList<Columna<ConsultaModel>> ColumnasConsulta = new[]
    {
        new Columna<ConsultaModel>("Id", c => c.Id.ToString()),
        new Columna<ConsultaModel>("Fecha", c => c.Fecha.ToString("dd/MM/yyyy HH:mm")),
        new Columna<ConsultaModel>("Mascota", c => $"{c.MascotaId} - {c.NombreMascota}"),
        new Columna<ConsultaModel>("Motivo", c => c.Motivo, 25),
        new Columna<ConsultaModel>("Diagnóstico", c => c.Diagnostico, 25),
        new Columna<ConsultaModel>("Tratamiento", c => c.Tratamiento, 25)
    };

    protected override string Titulo => "Gestión de Consultas / Citas";
    protected override string NombreEntidad => "consulta";
    protected override bool EsFemenino => true;
    protected override IRepository<ConsultaModel> Repositorio => _consultas;
    protected override IReadOnlyList<Columna<ConsultaModel>> Columnas => ColumnasConsulta;

    protected override IEnumerable<(string Tecla, string Texto, Action Accion)> OpcionesExtra => new (string, string, Action)[]
    {
        ("5", "Ver historial clínico de una mascota", VerHistorial)
    };

    protected override int ObtenerId(ConsultaModel entidad) => entidad.Id;

    protected override ConsultaModel CapturarCampos(ConsultaModel existente)
    {
        var mascotas = _mascotas.ObtenerTodos();
        if (mascotas.Count == 0)
        {
            throw new InvalidOperationException("Primero debe registrar al menos una mascota.");
        }

        GrillaConsola.Mostrar("Mascotas disponibles", mascotas, new[]
        {
            new Columna<MascotaModel>("Id", m => m.Id.ToString()),
            new Columna<MascotaModel>("Nombre", m => m.Nombre),
            new Columna<MascotaModel>("Especie", m => m.Especie),
            new Columna<MascotaModel>("Dueño", m => m.NombreCliente)
        });

        var c = new ConsultaModel { Id = existente?.Id ?? 0 };
        while (true)
        {
            c.MascotaId = Consola.LeerEntero("Id de la mascota", existente?.MascotaId, minimo: 1);
            if (mascotas.Any(m => m.Id == c.MascotaId))
            {
                break;
            }
            Consola.Error("No existe una mascota con ese Id.");
        }

        c.Fecha = Consola.LeerFecha("Fecha", existente?.Fecha);
        c.Motivo = Consola.LeerTexto("Motivo", obligatorio: true, existente?.Motivo, 250);
        c.Diagnostico = Consola.LeerTexto("Diagnóstico", valorActual: existente?.Diagnostico);
        c.Tratamiento = Consola.LeerTexto("Tratamiento", valorActual: existente?.Tratamiento);
        return c;
    }

    protected override void MostrarDetalle(ConsultaModel c)
    {
        Console.WriteLine($"Id:          {c.Id}");
        Console.WriteLine($"Fecha:       {c.Fecha:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"Mascota:     {c.NombreMascota} (Id {c.MascotaId})");
        Console.WriteLine($"Motivo:      {c.Motivo}");
        Console.WriteLine($"Diagnóstico: {c.Diagnostico}");
        Console.WriteLine($"Tratamiento: {c.Tratamiento}");
    }

    private void VerHistorial()
    {
        var id = Consola.LeerEntero("Id de la mascota", minimo: 1);
        var mascota = _mascotas.ObtenerPorId(id);
        if (mascota == null)
        {
            Consola.Error($"No existe una mascota con Id {id}.");
            return;
        }

        Console.WriteLine($"Mascota: {mascota.Nombre} ({mascota.Especie}) - Dueño: {mascota.NombreCliente}");
        GrillaConsola.Mostrar("Historial clínico", _consultas.ObtenerPorMascota(id), ColumnasConsulta);
    }
}

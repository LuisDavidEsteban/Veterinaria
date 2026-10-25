using ControlClinicoVeterinario.Data.Repositories;
using ControlClinicoVeterinario.Models;

namespace ControlClinicoVeterinario.Views;

public class ClienteForm : FormularioCrud<ClienteModel>
{
    private readonly ClienteRepository _clientes = new();
    private readonly MascotaRepository _mascotas = new();

    protected override string Titulo => "Gestión de Clientes / Dueños";
    protected override string NombreEntidad => "cliente";
    protected override IRepository<ClienteModel> Repositorio => _clientes;

    protected override IReadOnlyList<Columna<ClienteModel>> Columnas { get; } = new[]
    {
        new Columna<ClienteModel>("Id", c => c.Id.ToString()),
        new Columna<ClienteModel>("Nombre", c => c.Nombre),
        new Columna<ClienteModel>("Teléfono", c => c.Telefono),
        new Columna<ClienteModel>("Dirección", c => c.Direccion),
        new Columna<ClienteModel>("Correo", c => c.Correo)
    };

    protected override int ObtenerId(ClienteModel entidad) => entidad.Id;

    protected override ClienteModel CapturarCampos(ClienteModel existente)
    {
        var c = new ClienteModel { Id = existente?.Id ?? 0 };
        c.Nombre = Consola.LeerTexto("Nombre", obligatorio: true, existente?.Nombre, 150);
        c.Telefono = Consola.LeerTexto("Teléfono", valorActual: existente?.Telefono, longitudMaxima: 30);
        c.Direccion = Consola.LeerTexto("Dirección", valorActual: existente?.Direccion, longitudMaxima: 250);

        while (true)
        {
            c.Correo = Consola.LeerTexto("Correo", valorActual: existente?.Correo, longitudMaxima: 150);
            if (string.IsNullOrEmpty(c.Correo) || (c.Correo.Contains('@') && c.Correo.Contains('.')))
            {
                break;
            }
            Consola.Error("Correo inválido (ejemplo: nombre@dominio.com).");
        }
        return c;
    }

    protected override void MostrarDetalle(ClienteModel c)
    {
        Console.WriteLine($"Id:        {c.Id}");
        Console.WriteLine($"Nombre:    {c.Nombre}");
        Console.WriteLine($"Teléfono:  {c.Telefono}");
        Console.WriteLine($"Dirección: {c.Direccion}");
        Console.WriteLine($"Correo:    {c.Correo}");

        var mascotas = _mascotas.ObtenerPorCliente(c.Id);
        GrillaConsola.Mostrar("Mascotas del cliente", mascotas, MascotaForm.ColumnasMascota);
    }
}

using ControlClinicoVeterinario.Models;
using Npgsql;
using NpgsqlTypes;

namespace ControlClinicoVeterinario.Data.Repositories;

public class ClienteRepository : IRepository<ClienteModel>
{
    private const string SelectBase =
        "SELECT id, nombre, telefono, direccion, correo FROM clientes";

    public int Crear(ClienteModel cliente)
    {
        Validar(cliente);
        const string sql = @"INSERT INTO clientes (nombre, telefono, direccion, correo)
                             VALUES (@nombre, @telefono, @direccion, @correo)
                             RETURNING id";

        using var conexion = Conexion.CrearConexion();
        using var comando = new NpgsqlCommand(sql, conexion);
        AgregarParametros(comando, cliente);

        cliente.Id = Convert.ToInt32(comando.ExecuteScalar());
        return cliente.Id;
    }

    public List<ClienteModel> ObtenerTodos()
    {
        var lista = new List<ClienteModel>();
        using var conexion = Conexion.CrearConexion();
        using var comando = new NpgsqlCommand(SelectBase + " ORDER BY id", conexion);
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(Mapear(lector));
        }
        return lista;
    }

    public ClienteModel ObtenerPorId(int id)
    {
        using var conexion = Conexion.CrearConexion();
        using var comando = new NpgsqlCommand(SelectBase + " WHERE id = @id", conexion);
        comando.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
        using var lector = comando.ExecuteReader();
        return lector.Read() ? Mapear(lector) : null;
    }

    public bool Actualizar(ClienteModel cliente)
    {
        Validar(cliente);
        const string sql = @"UPDATE clientes
                             SET nombre = @nombre, telefono = @telefono,
                                 direccion = @direccion, correo = @correo
                             WHERE id = @id";

        using var conexion = Conexion.CrearConexion();
        using var comando = new NpgsqlCommand(sql, conexion);
        AgregarParametros(comando, cliente);
        comando.Parameters.Add("@id", NpgsqlDbType.Integer).Value = cliente.Id;
        return comando.ExecuteNonQuery() > 0;
    }

    public bool Eliminar(int id)
    {
        using var conexion = Conexion.CrearConexion();
        using var comando = new NpgsqlCommand("DELETE FROM clientes WHERE id = @id", conexion);
        comando.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
        return comando.ExecuteNonQuery() > 0;
    }

    private static void AgregarParametros(NpgsqlCommand comando, ClienteModel c)
    {
        comando.Parameters.Add("@nombre", NpgsqlDbType.Varchar).Value = c.Nombre.Trim();
        comando.Parameters.Add("@telefono", NpgsqlDbType.Varchar).Value = DbUtil.ANulo(c.Telefono);
        comando.Parameters.Add("@direccion", NpgsqlDbType.Varchar).Value = DbUtil.ANulo(c.Direccion);
        comando.Parameters.Add("@correo", NpgsqlDbType.Varchar).Value = DbUtil.ANulo(c.Correo);
    }

    private static ClienteModel Mapear(NpgsqlDataReader r) => new ClienteModel
    {
        Id = r.GetInt32(0),
        Nombre = r.GetString(1),
        Telefono = DbUtil.TextoOVacio(r, 2),
        Direccion = DbUtil.TextoOVacio(r, 3),
        Correo = DbUtil.TextoOVacio(r, 4)
    };

    private static void Validar(ClienteModel c)
    {
        ArgumentNullException.ThrowIfNull(c);
        if (string.IsNullOrWhiteSpace(c.Nombre))
        {
            throw new ArgumentException("El nombre del cliente es obligatorio.");
        }
        if (!string.IsNullOrWhiteSpace(c.Correo) && !c.Correo.Contains('@'))
        {
            throw new ArgumentException("El correo electrónico no tiene un formato válido.");
        }
    }
}

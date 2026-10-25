using ControlClinicoVeterinario.Models;
using Npgsql;
using NpgsqlTypes;

namespace ControlClinicoVeterinario.Data.Repositories;

public class MascotaRepository : IRepository<MascotaModel>
{
    private const string SelectBase = @"
        SELECT m.id, m.nombre, m.especie, m.raza, m.edad, m.peso, m.cliente_id, c.nombre
        FROM mascotas m
        INNER JOIN clientes c ON c.id = m.cliente_id";

    public int Crear(MascotaModel mascota)
    {
        Validar(mascota);
        const string sql = @"INSERT INTO mascotas (nombre, especie, raza, edad, peso, cliente_id)
                             VALUES (@nombre, @especie, @raza, @edad, @peso, @clienteId)
                             RETURNING id";

        using var conexion = Conexion.CrearConexion();
        using var comando = new NpgsqlCommand(sql, conexion);
        AgregarParametros(comando, mascota);

        mascota.Id = Convert.ToInt32(comando.ExecuteScalar());
        return mascota.Id;
    }

    public List<MascotaModel> ObtenerTodos()
    {
        using var conexion = Conexion.CrearConexion();
        using var comando = new NpgsqlCommand(SelectBase + " ORDER BY m.id", conexion);
        return Leer(comando);
    }

    /// <summary>Lista las mascotas que pertenecen a un cliente.</summary>
    public List<MascotaModel> ObtenerPorCliente(int clienteId)
    {
        using var conexion = Conexion.CrearConexion();
        using var comando = new NpgsqlCommand(SelectBase + " WHERE m.cliente_id = @clienteId ORDER BY m.id", conexion);
        comando.Parameters.Add("@clienteId", NpgsqlDbType.Integer).Value = clienteId;
        return Leer(comando);
    }

    public MascotaModel ObtenerPorId(int id)
    {
        using var conexion = Conexion.CrearConexion();
        using var comando = new NpgsqlCommand(SelectBase + " WHERE m.id = @id", conexion);
        comando.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
        return Leer(comando).FirstOrDefault();
    }

    public bool Actualizar(MascotaModel mascota)
    {
        Validar(mascota);
        const string sql = @"UPDATE mascotas
                             SET nombre = @nombre, especie = @especie, raza = @raza,
                                 edad = @edad, peso = @peso, cliente_id = @clienteId
                             WHERE id = @id";

        using var conexion = Conexion.CrearConexion();
        using var comando = new NpgsqlCommand(sql, conexion);
        AgregarParametros(comando, mascota);
        comando.Parameters.Add("@id", NpgsqlDbType.Integer).Value = mascota.Id;
        return comando.ExecuteNonQuery() > 0;
    }

    public bool Eliminar(int id)
    {
        using var conexion = Conexion.CrearConexion();
        using var comando = new NpgsqlCommand("DELETE FROM mascotas WHERE id = @id", conexion);
        comando.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
        return comando.ExecuteNonQuery() > 0;
    }

    private static List<MascotaModel> Leer(NpgsqlCommand comando)
    {
        var lista = new List<MascotaModel>();
        using var r = comando.ExecuteReader();
        while (r.Read())
        {
            lista.Add(new MascotaModel
            {
                Id = r.GetInt32(0),
                Nombre = r.GetString(1),
                Especie = r.GetString(2),
                Raza = DbUtil.TextoOVacio(r, 3),
                Edad = r.GetInt32(4),
                Peso = r.GetDecimal(5),
                ClienteId = r.GetInt32(6),
                NombreCliente = r.GetString(7)
            });
        }
        return lista;
    }

    private static void AgregarParametros(NpgsqlCommand comando, MascotaModel m)
    {
        comando.Parameters.Add("@nombre", NpgsqlDbType.Varchar).Value = m.Nombre.Trim();
        comando.Parameters.Add("@especie", NpgsqlDbType.Varchar).Value = m.Especie.Trim();
        comando.Parameters.Add("@raza", NpgsqlDbType.Varchar).Value = DbUtil.ANulo(m.Raza);
        comando.Parameters.Add("@edad", NpgsqlDbType.Integer).Value = m.Edad;
        comando.Parameters.Add("@peso", NpgsqlDbType.Numeric).Value = m.Peso;
        comando.Parameters.Add("@clienteId", NpgsqlDbType.Integer).Value = m.ClienteId;
    }

    private static void Validar(MascotaModel m)
    {
        ArgumentNullException.ThrowIfNull(m);
        if (string.IsNullOrWhiteSpace(m.Nombre))
        {
            throw new ArgumentException("El nombre de la mascota es obligatorio.");
        }
        if (string.IsNullOrWhiteSpace(m.Especie))
        {
            throw new ArgumentException("La especie de la mascota es obligatoria.");
        }
        if (m.Edad < 0)
        {
            throw new ArgumentException("La edad no puede ser negativa.");
        }
        if (m.Peso < 0 || m.Peso > 9999.99m)
        {
            throw new ArgumentException("El peso debe estar entre 0 y 9999.99 kg.");
        }
        if (m.ClienteId <= 0)
        {
            throw new ArgumentException("Debe indicar un cliente (dueño) válido.");
        }
    }
}

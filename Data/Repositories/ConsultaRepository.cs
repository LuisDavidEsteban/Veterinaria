using ControlClinicoVeterinario.Models;
using Npgsql;
using NpgsqlTypes;

namespace ControlClinicoVeterinario.Data.Repositories;

public class ConsultaRepository : IRepository<ConsultaModel>
{
    private const string SelectBase = @"
        SELECT co.id, co.fecha, co.motivo, co.diagnostico, co.tratamiento, co.mascota_id, m.nombre
        FROM consultas co
        INNER JOIN mascotas m ON m.id = co.mascota_id";

    public int Crear(ConsultaModel consulta)
    {
        Validar(consulta);
        const string sql = @"INSERT INTO consultas (fecha, motivo, diagnostico, tratamiento, mascota_id)
                             VALUES (@fecha, @motivo, @diagnostico, @tratamiento, @mascotaId)
                             RETURNING id";

        using var conexion = Conexion.CrearConexion();
        using var comando = new NpgsqlCommand(sql, conexion);
        AgregarParametros(comando, consulta);

        consulta.Id = Convert.ToInt32(comando.ExecuteScalar());
        return consulta.Id;
    }

    public List<ConsultaModel> ObtenerTodos()
    {
        using var conexion = Conexion.CrearConexion();
        using var comando = new NpgsqlCommand(SelectBase + " ORDER BY co.fecha DESC, co.id DESC", conexion);
        return Leer(comando);
    }

    /// <summary>Historial clínico de una mascota ordenado cronológicamente.</summary>
    public List<ConsultaModel> ObtenerPorMascota(int mascotaId)
    {
        using var conexion = Conexion.CrearConexion();
        using var comando = new NpgsqlCommand(SelectBase + " WHERE co.mascota_id = @mascotaId ORDER BY co.fecha, co.id", conexion);
        comando.Parameters.Add("@mascotaId", NpgsqlDbType.Integer).Value = mascotaId;
        return Leer(comando);
    }

    public ConsultaModel ObtenerPorId(int id)
    {
        using var conexion = Conexion.CrearConexion();
        using var comando = new NpgsqlCommand(SelectBase + " WHERE co.id = @id", conexion);
        comando.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
        return Leer(comando).FirstOrDefault();
    }

    public bool Actualizar(ConsultaModel consulta)
    {
        Validar(consulta);
        const string sql = @"UPDATE consultas
                             SET fecha = @fecha, motivo = @motivo, diagnostico = @diagnostico,
                                 tratamiento = @tratamiento, mascota_id = @mascotaId
                             WHERE id = @id";

        using var conexion = Conexion.CrearConexion();
        using var comando = new NpgsqlCommand(sql, conexion);
        AgregarParametros(comando, consulta);
        comando.Parameters.Add("@id", NpgsqlDbType.Integer).Value = consulta.Id;
        return comando.ExecuteNonQuery() > 0;
    }

    public bool Eliminar(int id)
    {
        using var conexion = Conexion.CrearConexion();
        using var comando = new NpgsqlCommand("DELETE FROM consultas WHERE id = @id", conexion);
        comando.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
        return comando.ExecuteNonQuery() > 0;
    }

    private static List<ConsultaModel> Leer(NpgsqlCommand comando)
    {
        var lista = new List<ConsultaModel>();
        using var r = comando.ExecuteReader();
        while (r.Read())
        {
            lista.Add(new ConsultaModel
            {
                Id = r.GetInt32(0),
                Fecha = r.GetDateTime(1),
                Motivo = r.GetString(2),
                Diagnostico = DbUtil.TextoOVacio(r, 3),
                Tratamiento = DbUtil.TextoOVacio(r, 4),
                MascotaId = r.GetInt32(5),
                NombreMascota = r.GetString(6)
            });
        }
        return lista;
    }

    private static void AgregarParametros(NpgsqlCommand comando, ConsultaModel c)
    {
        // La columna es TIMESTAMP (sin zona horaria): se envía como fecha "sin especificar".
        comando.Parameters.Add("@fecha", NpgsqlDbType.Timestamp).Value =
            DateTime.SpecifyKind(c.Fecha, DateTimeKind.Unspecified);
        comando.Parameters.Add("@motivo", NpgsqlDbType.Varchar).Value = c.Motivo.Trim();
        comando.Parameters.Add("@diagnostico", NpgsqlDbType.Text).Value = DbUtil.ANulo(c.Diagnostico);
        comando.Parameters.Add("@tratamiento", NpgsqlDbType.Text).Value = DbUtil.ANulo(c.Tratamiento);
        comando.Parameters.Add("@mascotaId", NpgsqlDbType.Integer).Value = c.MascotaId;
    }

    private static void Validar(ConsultaModel c)
    {
        ArgumentNullException.ThrowIfNull(c);
        if (string.IsNullOrWhiteSpace(c.Motivo))
        {
            throw new ArgumentException("El motivo de la consulta es obligatorio.");
        }
        if (c.Fecha == default)
        {
            throw new ArgumentException("La fecha de la consulta es obligatoria.");
        }
        if (c.MascotaId <= 0)
        {
            throw new ArgumentException("Debe indicar una mascota válida.");
        }
    }
}

using Npgsql;

namespace ControlClinicoVeterinario.Data;

/// <summary>
/// Punto centralizado para obtener conexiones a PostgreSQL.
/// La cadena de conexión puede sobrescribirse con la variable de entorno
/// <c>VETERINARIA_DB_CONNECTION</c> sin necesidad de recompilar.
/// </summary>
public static class Conexion
{
    public const string VariableEntorno = "VETERINARIA_DB_CONNECTION";

    public const string CadenaPorDefecto =
        "Host=localhost;Port=5432;Database=veterinaria_db;Username=postgres;Password=postgres;";

    private static string _cadena;

    /// <summary>Cadena de conexión activa.</summary>
    public static string Cadena
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_cadena))
            {
                var desdeEntorno = Environment.GetEnvironmentVariable(VariableEntorno);
                _cadena = string.IsNullOrWhiteSpace(desdeEntorno) ? CadenaPorDefecto : desdeEntorno;
            }
            return _cadena;
        }
        set => _cadena = value;
    }

    /// <summary>Componentes de la cadena (host, puerto, base, usuario, contraseña).</summary>
    public static NpgsqlConnectionStringBuilder Builder => new NpgsqlConnectionStringBuilder(Cadena);

    /// <summary>
    /// Crea y abre una nueva conexión. Debe liberarse con <c>using</c>;
    /// Npgsql gestiona internamente el pool de conexiones.
    /// </summary>
    public static NpgsqlConnection CrearConexion()
    {
        var conexion = new NpgsqlConnection(Cadena);
        conexion.Open();
        return conexion;
    }
}

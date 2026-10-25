using Npgsql;

namespace ControlClinicoVeterinario.Data;

/// <summary>
/// Utilidades compartidas por los repositorios.
/// </summary>
internal static class DbUtil
{
    /// <summary>Convierte cadenas vacías en <see cref="DBNull"/> para guardarlas como NULL.</summary>
    public static object ANulo(string valor) =>
        string.IsNullOrWhiteSpace(valor) ? DBNull.Value : valor.Trim();

    public static string TextoOVacio(NpgsqlDataReader lector, int ordinal) =>
        lector.IsDBNull(ordinal) ? string.Empty : lector.GetString(ordinal);

    /// <summary>
    /// Traduce las excepciones de PostgreSQL a mensajes comprensibles para el usuario.
    /// </summary>
    public static string MensajeAmigable(Exception ex) => ex switch
    {
        // 23503 = foreign_key_violation; 23001 = restrict_violation (ON DELETE RESTRICT).
        PostgresException pg when pg.SqlState == PostgresErrorCodes.ForeignKeyViolation
                               || pg.SqlState == PostgresErrorCodes.RestrictViolation =>
            // Las FK se definen con ON DELETE RESTRICT: borrar un padre con hijos produce 23001,
            // mientras que insertar/actualizar un hijo con un padre inexistente produce 23503.
            (pg.ConstraintName, pg.SqlState == PostgresErrorCodes.RestrictViolation) switch
            {
                ("fk_mascotas_clientes", false) => "El cliente indicado no existe.",
                ("fk_consultas_mascotas", false) => "La mascota indicada no existe.",
                ("fk_mascotas_clientes", true) => "No se puede eliminar: el cliente tiene mascotas registradas. Elimínelas o reasígnelas primero.",
                ("fk_consultas_mascotas", true) => "No se puede eliminar: la mascota tiene consultas registradas. Elimínelas primero.",
                _ => "Operación rechazada por integridad referencial: " + pg.MessageText
            },
        PostgresException pg when pg.SqlState == PostgresErrorCodes.CheckViolation =>
            "Algún valor no cumple las reglas (edad y peso deben ser >= 0).",
        PostgresException pg when pg.SqlState == PostgresErrorCodes.StringDataRightTruncation =>
            "Uno de los textos excede la longitud máxima permitida.",
        PostgresException pg when pg.SqlState == PostgresErrorCodes.InvalidPassword
                               || pg.SqlState == PostgresErrorCodes.InvalidAuthorizationSpecification =>
            "Credenciales de PostgreSQL inválidas. Revise la cadena de conexión.",
        PostgresException pg => $"Error de base de datos ({pg.SqlState}): {pg.MessageText}",
        NpgsqlException => "No fue posible comunicarse con PostgreSQL. Verifique que el servidor esté en ejecución. Detalle: " + ex.Message,
        ArgumentException => ex.Message,
        InvalidOperationException => ex.Message,
        _ => "Error inesperado: " + ex.Message
    };
}

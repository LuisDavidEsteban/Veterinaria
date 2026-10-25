namespace ControlClinicoVeterinario.Data.Repositories;

/// <summary>
/// Contrato CRUD común para todos los repositorios.
/// </summary>
public interface IRepository<T>
{
    /// <summary>Inserta la entidad y devuelve el Id generado (también lo asigna a la entidad).</summary>
    int Crear(T entidad);

    List<T> ObtenerTodos();

    /// <summary>Devuelve la entidad o <c>null</c> si no existe.</summary>
    T ObtenerPorId(int id);

    /// <summary>Devuelve <c>true</c> si se actualizó algún registro.</summary>
    bool Actualizar(T entidad);

    /// <summary>Devuelve <c>true</c> si se eliminó algún registro.</summary>
    bool Eliminar(int id);
}

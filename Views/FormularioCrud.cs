using ControlClinicoVeterinario.Data;
using ControlClinicoVeterinario.Data.Repositories;

namespace ControlClinicoVeterinario.Views;

/// <summary>
/// Formulario CRUD genérico de consola. Cada botón (Guardar, Actualizar, Eliminar, Buscar)
/// está enlazado al método correspondiente del repositorio; la grilla se refresca
/// automáticamente después de cada operación y los campos se limpian tras cada éxito.
/// </summary>
public abstract class FormularioCrud<T> where T : class, new()
{
    protected abstract string Titulo { get; }
    protected abstract string NombreEntidad { get; }

    /// <summary>Indica si el sustantivo de la entidad es femenino (para la concordancia de los mensajes).</summary>
    protected virtual bool EsFemenino => false;

    private string Art => EsFemenino ? "la" : "el";
    private string Del => EsFemenino ? "de la" : "del";
    private string Este => EsFemenino ? "esta" : "este";
    private string Un => EsFemenino ? "una" : "un";
    private string O => EsFemenino ? "a" : "o";
    protected abstract IRepository<T> Repositorio { get; }
    protected abstract IReadOnlyList<Columna<T>> Columnas { get; }

    /// <summary>
    /// "Campos de texto" del formulario. Se reinicia con <see cref="LimpiarCampos"/>
    /// tras cada operación exitosa.
    /// </summary>
    protected T Campos { get; private set; } = new T();

    /// <summary>Captura los valores de los campos. Si <paramref name="existente"/> no es null se edita.</summary>
    protected abstract T CapturarCampos(T existente);

    protected abstract int ObtenerId(T entidad);

    /// <summary>Muestra la ficha completa de un registro.</summary>
    protected abstract void MostrarDetalle(T entidad);

    /// <summary>Opciones adicionales específicas del formulario (clave, descripción, acción).</summary>
    protected virtual IEnumerable<(string Tecla, string Texto, Action Accion)> OpcionesExtra =>
        Enumerable.Empty<(string, string, Action)>();

    public void Mostrar()
    {
        while (true)
        {
            Consola.Titulo(Titulo);
            RefrescarGrilla();

            Console.WriteLine();
            Console.WriteLine("1. Guardar   2. Actualizar   3. Eliminar   4. Buscar");
            foreach (var extra in OpcionesExtra)
            {
                Console.WriteLine($"{extra.Tecla}. {extra.Texto}");
            }
            Console.WriteLine("0. Volver al menú principal");
            Console.WriteLine($"(Escriba {Consola.ComandoCancelar} en cualquier campo para cancelar)");
            Console.Write("Opción: ");

            string opcion;
            try
            {
                opcion = Consola.LeerLinea();
            }
            catch (OperacionCanceladaException)
            {
                return;
            }

            switch (opcion)
            {
                case "1": Ejecutar(BtnGuardar_Click); break;
                case "2": Ejecutar(BtnActualizar_Click); break;
                case "3": Ejecutar(BtnEliminar_Click); break;
                case "4": Ejecutar(BtnBuscar_Click); break;
                case "0": return;
                default:
                    var extra = OpcionesExtra.FirstOrDefault(o => o.Tecla == opcion);
                    if (extra.Accion != null)
                    {
                        Ejecutar(extra.Accion);
                    }
                    else
                    {
                        Consola.Error("Opción no reconocida.");
                    }
                    break;
            }
        }
    }

    /// <summary>Recarga la grilla desde la base de datos.</summary>
    protected void RefrescarGrilla()
    {
        try
        {
            GrillaConsola.Mostrar($"Listado de {NombreEntidad}s", Repositorio.ObtenerTodos(), Columnas);
        }
        catch (Exception ex) when (ex is not EntradaFinalizadaException)
        {
            Consola.Error("No se pudo cargar la grilla: " + DbUtil.MensajeAmigable(ex));
        }
    }

    protected void LimpiarCampos() => Campos = new T();

    // ---------------- Manejadores de los "botones" ----------------

    private void BtnGuardar_Click()
    {
        Consola.Titulo($"Registro de {NombreEntidad}");
        LimpiarCampos();
        Campos = CapturarCampos(null);
        var id = Repositorio.Crear(Campos);
        Consola.Exito($"{Capitalizar(NombreEntidad)} guardad{O} correctamente con Id {id}.");
        LimpiarCampos();
    }

    private void BtnActualizar_Click()
    {
        var actual = SolicitarExistente("actualizar");
        if (actual == null) return;

        Consola.Aviso("Presione Enter para conservar el valor actual mostrado entre [ ].");
        Campos = CapturarCampos(actual);
        if (Repositorio.Actualizar(Campos))
        {
            Consola.Exito($"{Capitalizar(NombreEntidad)} #{ObtenerId(Campos)} actualizad{O} correctamente.");
        }
        else
        {
            Consola.Aviso($"{Capitalizar(Art)} {NombreEntidad} ya no existe; no se actualizó ningún registro.");
        }
        LimpiarCampos();
    }

    private void BtnEliminar_Click()
    {
        var actual = SolicitarExistente("eliminar");
        if (actual == null) return;

        MostrarDetalle(actual);
        if (!Consola.Confirmar($"¿Confirma eliminar {Este} {NombreEntidad}?"))
        {
            Consola.Aviso("Eliminación cancelada.");
            return;
        }

        if (Repositorio.Eliminar(ObtenerId(actual)))
        {
            Consola.Exito($"{Capitalizar(NombreEntidad)} eliminad{O} correctamente.");
        }
        else
        {
            Consola.Aviso($"{Capitalizar(Art)} {NombreEntidad} ya no existe.");
        }
        LimpiarCampos();
    }

    private void BtnBuscar_Click()
    {
        var encontrado = SolicitarExistente("buscar");
        if (encontrado == null) return;

        Consola.Titulo($"Detalle de {NombreEntidad}");
        MostrarDetalle(encontrado);
        LimpiarCampos();
    }

    // ---------------- Utilidades ----------------

    private T SolicitarExistente(string accion)
    {
        var id = Consola.LeerEntero($"Id {Del} {NombreEntidad} a {accion}", minimo: 1);
        var entidad = Repositorio.ObtenerPorId(id);
        if (entidad == null)
        {
            Consola.Error($"No existe {Un} {NombreEntidad} con Id {id}.");
        }
        return entidad;
    }

    private static string Capitalizar(string texto) =>
        string.IsNullOrEmpty(texto) ? texto : char.ToUpper(texto[0]) + texto[1..];

    /// <summary>
    /// Envoltura try/catch común: muestra mensajes claros y nunca deja caer la aplicación.
    /// </summary>
    protected static void Ejecutar(Action accion)
    {
        try
        {
            accion();
        }
        catch (OperacionCanceladaException ex)
        {
            Consola.Aviso(ex.Message);
        }
        catch (EntradaFinalizadaException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Consola.Error(DbUtil.MensajeAmigable(ex));
        }
    }
}

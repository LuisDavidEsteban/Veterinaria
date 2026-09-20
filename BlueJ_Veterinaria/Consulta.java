import java.time.LocalDate;

public class Consulta {
    private int id;
    private LocalDate fecha;
    private Mascota mascota;
    private Dueno dueno;
    private Veterinario veterinario;
    private Receta receta;
    private String diagnostico;
    private double costo;

    public Consulta(int id, LocalDate fecha, Mascota mascota, Dueno dueno, Veterinario veterinario,
                    Receta receta, String diagnostico, double costo) {
        this.id = id;
        this.fecha = fecha;
        this.mascota = mascota;
        this.dueno = dueno;
        this.veterinario = veterinario;
        this.receta = receta;
        this.diagnostico = diagnostico;
        this.costo = costo;
    }

    public double calcularCostoTotal() {
        double total = costo;
        if (receta != null) {
            total += receta.calcularTotal();
        }
        return total;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public LocalDate getFecha() {
        return fecha;
    }

    public void setFecha(LocalDate fecha) {
        this.fecha = fecha;
    }

    public Mascota getMascota() {
        return mascota;
    }

    public void setMascota(Mascota mascota) {
        this.mascota = mascota;
    }

    public Dueno getDueno() {
        return dueno;
    }

    public void setDueno(Dueno dueno) {
        this.dueno = dueno;
    }

    public Veterinario getVeterinario() {
        return veterinario;
    }

    public void setVeterinario(Veterinario veterinario) {
        this.veterinario = veterinario;
    }

    public Receta getReceta() {
        return receta;
    }

    public void setReceta(Receta receta) {
        this.receta = receta;
    }

    public String getDiagnostico() {
        return diagnostico;
    }

    public void setDiagnostico(String diagnostico) {
        this.diagnostico = diagnostico;
    }

    public double getCosto() {
        return costo;
    }

    public void setCosto(double costo) {
        this.costo = costo;
    }

    @Override
    public String toString() {
        return "Consulta " + id + " - " + fecha + " - " + diagnostico;
    }
}

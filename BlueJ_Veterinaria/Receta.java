import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;

public class Receta {
    private int numero;
    private LocalDate fecha;
    private List<Medicamento> medicamentos;

    public Receta(int numero, LocalDate fecha) {
        this.numero = numero;
        this.fecha = fecha;
        this.medicamentos = new ArrayList<Medicamento>();
    }

    public void agregarMedicamento(Medicamento medicamento) {
        if (medicamento != null) {
            medicamentos.add(medicamento);
        }
    }

    public boolean quitarMedicamento(Medicamento medicamento) {
        return medicamentos.remove(medicamento);
    }

    public double calcularTotal() {
        double total = 0;
        for (Medicamento medicamento : medicamentos) {
            total += medicamento.getPrecio();
        }
        return total;
    }

    public int getNumero() {
        return numero;
    }

    public void setNumero(int numero) {
        this.numero = numero;
    }

    public LocalDate getFecha() {
        return fecha;
    }

    public void setFecha(LocalDate fecha) {
        this.fecha = fecha;
    }

    public List<Medicamento> getMedicamentos() {
        return medicamentos;
    }

    public void setMedicamentos(List<Medicamento> medicamentos) {
        this.medicamentos = medicamentos;
    }

    @Override
    public String toString() {
        return "Receta " + numero + " (" + medicamentos.size() + " medicamentos)";
    }
}

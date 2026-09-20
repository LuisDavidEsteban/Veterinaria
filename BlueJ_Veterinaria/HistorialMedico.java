import java.util.ArrayList;
import java.util.List;

public class HistorialMedico {
    private Mascota mascota;
    private List<Consulta> consultas;

    public HistorialMedico(Mascota mascota) {
        this.mascota = mascota;
        this.consultas = new ArrayList<Consulta>();
    }

    public void agregarConsulta(Consulta consulta) {
        if (consulta != null && !consultas.contains(consulta)) {
            consultas.add(consulta);
        }
    }

    public double calcularGastoTotal() {
        double total = 0;
        for (Consulta consulta : consultas) {
            total += consulta.calcularCostoTotal();
        }
        return total;
    }

    public Mascota getMascota() {
        return mascota;
    }

    public void setMascota(Mascota mascota) {
        this.mascota = mascota;
    }

    public List<Consulta> getConsultas() {
        return consultas;
    }

    public void setConsultas(List<Consulta> consultas) {
        this.consultas = consultas;
    }

    @Override
    public String toString() {
        return "Historial de " + mascota.getNombre() + " (" + consultas.size() + " consultas)";
    }
}

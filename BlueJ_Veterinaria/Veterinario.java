import java.util.ArrayList;
import java.util.List;

public class Veterinario {
    private String colegiado;
    private String nombre;
    private String especialidad;
    private List<String> turnos;

    public Veterinario(String colegiado, String nombre, String especialidad) {
        this.colegiado = colegiado;
        this.nombre = nombre;
        this.especialidad = especialidad;
        this.turnos = new ArrayList<String>();
    }

    public void agregarTurno(String turno) {
        if (turno != null && !turnos.contains(turno)) {
            turnos.add(turno);
        }
    }

    public boolean quitarTurno(String turno) {
        return turnos.remove(turno);
    }

    public String getColegiado() {
        return colegiado;
    }

    public void setColegiado(String colegiado) {
        this.colegiado = colegiado;
    }

    public String getNombre() {
        return nombre;
    }

    public void setNombre(String nombre) {
        this.nombre = nombre;
    }

    public String getEspecialidad() {
        return especialidad;
    }

    public void setEspecialidad(String especialidad) {
        this.especialidad = especialidad;
    }

    public List<String> getTurnos() {
        return turnos;
    }

    public void setTurnos(List<String> turnos) {
        this.turnos = turnos;
    }

    @Override
    public String toString() {
        return nombre + " (" + especialidad + ")";
    }
}

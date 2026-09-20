import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;

public class GestorClinica {
    private List<Mascota> mascotas;
    private List<Dueno> duenos;
    private List<Consulta> consultas;
    private List<Veterinario> veterinarios;
    private int siguienteConsulta;

    public GestorClinica() {
        this.mascotas = new ArrayList<Mascota>();
        this.duenos = new ArrayList<Dueno>();
        this.consultas = new ArrayList<Consulta>();
        this.veterinarios = new ArrayList<Veterinario>();
        this.siguienteConsulta = 1;
    }

    public void registrarDueno(Dueno dueno) {
        if (dueno != null && !duenos.contains(dueno)) {
            duenos.add(dueno);
        }
    }

    public void registrarMascota(Mascota mascota, Dueno dueno) {
        if (mascota == null || dueno == null) {
            return;
        }
        registrarDueno(dueno);
        dueno.agregarMascota(mascota);
        if (!mascotas.contains(mascota)) {
            mascotas.add(mascota);
        }
    }

    public void registrarVeterinario(Veterinario veterinario) {
        if (veterinario != null && !veterinarios.contains(veterinario)) {
            veterinarios.add(veterinario);
        }
    }

    public Consulta registrarConsulta(Mascota mascota, Veterinario veterinario, Receta receta,
                                      String diagnostico, double costo) {
        if (mascota == null || veterinario == null || mascota.getDueno() == null) {
            return null;
        }
        Consulta consulta = new Consulta(siguienteConsulta++, LocalDate.now(), mascota,
                mascota.getDueno(), veterinario, receta, diagnostico, costo);
        consultas.add(consulta);
        mascota.getHistorial().agregarConsulta(consulta);
        return consulta;
    }

    public Mascota buscarMascota(int id) {
        for (Mascota mascota : mascotas) {
            if (mascota.getId() == id) {
                return mascota;
            }
        }
        return null;
    }

    public Dueno buscarDueno(int id) {
        for (Dueno dueno : duenos) {
            if (dueno.getId() == id) {
                return dueno;
            }
        }
        return null;
    }

    public Veterinario buscarVeterinario(String colegiado) {
        for (Veterinario veterinario : veterinarios) {
            if (veterinario.getColegiado().equals(colegiado)) {
                return veterinario;
            }
        }
        return null;
    }

    public List<Consulta> consultasDeMascota(Mascota mascota) {
        List<Consulta> resultado = new ArrayList<Consulta>();
        for (Consulta consulta : consultas) {
            if (consulta.getMascota() == mascota) {
                resultado.add(consulta);
            }
        }
        return resultado;
    }

    public double totalFacturado() {
        double total = 0;
        for (Consulta consulta : consultas) {
            total += consulta.calcularCostoTotal();
        }
        return total;
    }

    public List<Mascota> getMascotas() {
        return mascotas;
    }

    public void setMascotas(List<Mascota> mascotas) {
        this.mascotas = mascotas;
    }

    public List<Dueno> getDuenos() {
        return duenos;
    }

    public void setDuenos(List<Dueno> duenos) {
        this.duenos = duenos;
    }

    public List<Consulta> getConsultas() {
        return consultas;
    }

    public void setConsultas(List<Consulta> consultas) {
        this.consultas = consultas;
    }

    public List<Veterinario> getVeterinarios() {
        return veterinarios;
    }

    public void setVeterinarios(List<Veterinario> veterinarios) {
        this.veterinarios = veterinarios;
    }
}

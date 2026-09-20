public abstract class Mascota {
    private int id;
    private String nombre;
    private int edad;
    private double peso;
    private Dueno dueno;
    private HistorialMedico historial;

    public Mascota(int id, String nombre, int edad, double peso) {
        this.id = id;
        this.nombre = nombre;
        this.edad = edad;
        this.peso = peso;
        this.historial = new HistorialMedico(this);
    }

    public abstract String emitirSonido();

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getNombre() {
        return nombre;
    }

    public void setNombre(String nombre) {
        this.nombre = nombre;
    }

    public int getEdad() {
        return edad;
    }

    public void setEdad(int edad) {
        this.edad = edad;
    }

    public double getPeso() {
        return peso;
    }

    public void setPeso(double peso) {
        this.peso = peso;
    }

    public Dueno getDueno() {
        return dueno;
    }

    public void setDueno(Dueno dueno) {
        this.dueno = dueno;
    }

    public HistorialMedico getHistorial() {
        return historial;
    }

    public void setHistorial(HistorialMedico historial) {
        this.historial = historial;
    }

    @Override
    public String toString() {
        return nombre + " (" + edad + " anios, " + peso + " kg)";
    }
}

public class Medicamento {
    private String codigo;
    private String nombre;
    private String dosis;
    private double precio;

    public Medicamento(String codigo, String nombre, String dosis, double precio) {
        this.codigo = codigo;
        this.nombre = nombre;
        this.dosis = dosis;
        this.precio = precio;
    }

    public String getCodigo() {
        return codigo;
    }

    public void setCodigo(String codigo) {
        this.codigo = codigo;
    }

    public String getNombre() {
        return nombre;
    }

    public void setNombre(String nombre) {
        this.nombre = nombre;
    }

    public String getDosis() {
        return dosis;
    }

    public void setDosis(String dosis) {
        this.dosis = dosis;
    }

    public double getPrecio() {
        return precio;
    }

    public void setPrecio(double precio) {
        this.precio = precio;
    }

    @Override
    public String toString() {
        return nombre + " " + dosis + " - " + precio;
    }
}

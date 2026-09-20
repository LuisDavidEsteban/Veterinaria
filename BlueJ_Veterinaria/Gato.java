public class Gato extends Mascota {
    private String color;
    private boolean domestico;

    public Gato(int id, String nombre, int edad, double peso, String color, boolean domestico) {
        super(id, nombre, edad, peso);
        this.color = color;
        this.domestico = domestico;
    }

    @Override
    public String emitirSonido() {
        return "Miau";
    }

    public String getColor() {
        return color;
    }

    public void setColor(String color) {
        this.color = color;
    }

    public boolean isDomestico() {
        return domestico;
    }

    public void setDomestico(boolean domestico) {
        this.domestico = domestico;
    }
}

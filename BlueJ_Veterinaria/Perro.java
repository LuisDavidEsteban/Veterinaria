public class Perro extends Mascota {
    private String raza;
    private String tamano;

    public Perro(int id, String nombre, int edad, double peso, String raza, String tamano) {
        super(id, nombre, edad, peso);
        this.raza = raza;
        this.tamano = tamano;
    }

    @Override
    public String emitirSonido() {
        return "Guau";
    }

    public String getRaza() {
        return raza;
    }

    public void setRaza(String raza) {
        this.raza = raza;
    }

    public String getTamano() {
        return tamano;
    }

    public void setTamano(String tamano) {
        this.tamano = tamano;
    }
}

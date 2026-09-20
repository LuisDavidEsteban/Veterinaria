public class Ave extends Mascota {
    private String especie;
    private boolean puedeVolar;

    public Ave(int id, String nombre, int edad, double peso, String especie, boolean puedeVolar) {
        super(id, nombre, edad, peso);
        this.especie = especie;
        this.puedeVolar = puedeVolar;
    }

    @Override
    public String emitirSonido() {
        return "Pio";
    }

    public String getEspecie() {
        return especie;
    }

    public void setEspecie(String especie) {
        this.especie = especie;
    }

    public boolean isPuedeVolar() {
        return puedeVolar;
    }

    public void setPuedeVolar(boolean puedeVolar) {
        this.puedeVolar = puedeVolar;
    }
}

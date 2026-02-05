class Semaforas {
    private int laisviResursai;

    public Semaforas(int resursai) {
        this.laisviResursai = resursai;
    }

    // Request - laukimas, kol bus laisvas resursas
    public synchronized void request() {
        while (laisviResursai == 0) {
            try {
                System.out.println(Thread.currentThread().getName() + " laukia traktoriaus...");
                wait(); // Laukiame, kol traktorius bus atlaisvintas
            } catch (InterruptedException e) {
                Thread.currentThread().interrupt();
            }
        }
        laisviResursai--; // Pasiimam traktorių
        System.out.println(Thread.currentThread().getName() + " gavo traktorių. Liko: " + laisviResursai);
    }

    // Release - traktoriaus grąžinimas
    public synchronized void release() {
        laisviResursai++; // Grąžiname traktorių
        System.out.println(Thread.currentThread().getName() + " grąžino traktorių. Dabar laisvų: " + laisviResursai);
        notifyAll(); // Pažadiname laukiančias gijas
    }
}

class DarbineGija extends Thread {
    private Semaforas semaforas;

    public DarbineGija(Semaforas semaforas, String pavadinimas) {
        super(pavadinimas);
        this.semaforas = semaforas;
    }

    @Override
    public void run() {
        semaforas.request(); // Pasiimame traktorių
        try {
            System.out.println(getName() + " dirba su traktoriu...");
            Thread.sleep(2000); // Simuliuojame darbą (pvz., šienavimas ar derliaus nuėmimas)
        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
        } finally {
            semaforas.release(); // Atlaisviname traktorių
        }
    }
}

public class Ferma {
    public static void main(String[] args) {
        Semaforas semaforas = new Semaforas(2); // Turime 2 traktorius

        // Paleidžiame 5 darbuotojus (gijas)
        for (int i = 1; i <= 5; i++) {
            new DarbineGija(semaforas, "Darbuotojas-" + i).start();
        }
    }
}

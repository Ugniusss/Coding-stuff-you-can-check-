class Worker extends Thread {
    private final Lauk farm1;
    private final Lauk farm2;
    private final Lauk farm3;

    public Worker(Lauk farm1, Lauk farm2, Lauk farm3) {
        this.farm1 = farm1;
        this.farm2 = farm2;
        this.farm3 = farm3;
    }

    @Override
    public void run() {
        for (int i = 0; i < 1000; i++) {
            // Workers interact with all three farms
            farm1.pliusJavai(2);
            farm2.pliusJavai(2);
            farm3.pliusJavai(2);

            farm1.minusJavai(1);
            farm2.minusJavai(1);
            farm3.minusJavai(1);
        }
    }
}

class Lauk {
    private int javai;

    public Lauk(int initialjavai) {
        this.javai = initialjavai;
    }

    // Nuiimt prideti sync
    public synchronized void pliusJavai(int amount) {
        int oldJavai = javai;
        javai = oldJavai + amount;
    }

    // Nuiimt prideti sync
    public synchronized void minusJavai(int amount) {
        if (javai >= amount) {
            int oldJavai = javai;
            javai = oldJavai - amount;
        } else {
            System.out.println("Per mazai javu");
        }
    }

    public int getJavai() {
        return javai;
    }
}

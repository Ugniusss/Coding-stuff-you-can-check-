public class Main {
    public static void main(String[] args) {
        Lauk farm1 = new Lauk(50);
        Lauk farm2 = new Lauk(100);
        Lauk farm3 = new Lauk(200);

        Worker[] workers = new Worker[10];


        for (int i = 0; i < workers.length; i++) {
            workers[i] = new Worker(farm1, farm2, farm3);
            workers[i].start();
        }

        for (Worker worker : workers) {
            try { worker.join(); } catch (InterruptedException ignored) {}
        }


        System.out.println("Pasodinti javai lauke1: " + farm1.getJavai());
        System.out.println("Pasodinti javai lauke2: " + farm2.getJavai());
        System.out.println("Pasodinti javai lauke3: " + farm3.getJavai());
    }
}

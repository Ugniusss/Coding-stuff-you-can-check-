//
// Source code recreated from a .class file by IntelliJ IDEA
// (powered by FernFlower decompiler)
//

public class TTest extends Thread {
    volatile boolean finished = false;
    static int workload = 0;
    static int nThreads = 0;

    public TTest() {
    }

    boolean work(long var1) {
        return var1 < 0L;
    }

    public void run() {
        long var1 = (long)workload * 1000000L / (long)nThreads;

        for(long var3 = 0L; var3 < var1; ++var3) {
            boolean var5 = this.work(var3);
            if (var5) {
                System.out.println("Never be printed");
            }
        }

        this.finished = true;
    }

    static double makePerformanceTest() throws Exception {
        long var0 = System.currentTimeMillis();
        TTest[] var2 = new TTest[nThreads];

        int var3;
        for(var3 = 0; var3 < nThreads; ++var3) {
            (var2[var3] = new TTest()).start();
        }

        for(var3 = 0; var3 < nThreads; ++var3) {
            while(!var2[var3].finished) {
                var2[var3].join();
            }
        }

        long var7 = System.currentTimeMillis();
        double var5 = (double)(var7 - var0) / 1000.0;
        return var5;
    }

    public static void main(String[] var0) {
        try {
            double var1;
            if (var0.length >= 2 && (nThreads = Integer.parseInt(var0[0])) >= 1 && nThreads <= 16 && (workload = Integer.parseInt(var0[1])) >= 1 && workload <= 100000000) {
                System.err.println("#Test for: nThreads=" + nThreads + " workload=" + workload);
                var1 = makePerformanceTest();
                System.err.println("#Completed. Running time: " + var1 + "s");
                System.out.println(nThreads + " " + workload + " " + var1);
                System.exit(0);
            } else {
                System.err.println("Simple system multithreading performance test. Ver 1.3");
                System.err.println("Parameters: <number threads 1..16> <workload: 1..100000000>");
                System.err.println("#Make auto test: find workload for > 1 sec...");
                nThreads = 1;
                workload = 1;
                var1 = 0.0;

                while(true) {
                    var1 = makePerformanceTest();
                    if (var1 > 1.0) {
                        System.out.println("#nThreads #workload #timeS #speedup");
                        var1 = 0.0;

                        for(nThreads = 1; nThreads <= 32; nThreads *= 2) {
                            double var3 = makePerformanceTest();
                            var1 = nThreads == 1 ? var3 : var1;
                            double var5 = var1 / var3;
                            System.out.println(nThreads + " " + workload + " " + var3 + " " + var5);
                        }

                        System.out.println("#completed");
                        System.exit(1);
                        break;
                    }

                    workload *= 2;
                }
            }
        } catch (Exception var7) {
            System.out.println(var7);
            var7.printStackTrace();
            System.exit(4);
        }

    }
}

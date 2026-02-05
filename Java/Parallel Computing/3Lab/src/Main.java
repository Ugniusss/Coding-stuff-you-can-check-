import java.util.Arrays;
import java.util.Random;
import java.util.concurrent.ForkJoinPool;
import java.util.concurrent.RecursiveAction;

public class Main {

    public static void main(String[] args) {
        int arraySize = 100000000;
        int[] threadCounts = {1, 2, 4, 8, 16, 32};
        boolean debugMode = true;

        int[] originalArray = new int[arraySize];
        Random rand = new Random();
        for (int i = 0; i < originalArray.length; i++) {
            originalArray[i] = rand.nextInt(1000000);
        }

        long singleThreadTime = 0;

        for (int threadsCount : threadCounts) {
            int[] array = Arrays.copyOf(originalArray, originalArray.length);
            ForkJoinPool forkJoinPool = new ForkJoinPool(threadsCount);
            long startTime = System.currentTimeMillis();

            System.out.println("Operating in " + (debugMode ? "DEBUG" : "FAST") + " mode.");
            System.out.println("Testing with " + threadsCount + " threads.");

            QuicksortTask task = new QuicksortTask(array, 0, array.length - 1, debugMode, startTime);
            forkJoinPool.invoke(task);

            long endTime = System.currentTimeMillis();
            long duration = endTime - startTime;
            System.out.println("Sorting with " + threadsCount + " threads took: " + duration + " ms");

            if (threadsCount == 1) {
                singleThreadTime = duration;
            } else if (!debugMode) {
                double speedup = (double) singleThreadTime / duration;
                System.out.println("Speedup with " + threadsCount + " threads: " + speedup);
            }
        }
    }

    static class QuicksortTask extends RecursiveAction {
        private final int[] array;
        private final int left;
        private final int right;
        private final boolean debugMode;
        private final long startTime;

        public QuicksortTask(int[] array, int left, int right, boolean debugMode, long startTime) {
            this.array = array;
            this.left = left;
            this.right = right;
            this.debugMode = debugMode;
            this.startTime = startTime;
        }

        @Override
        protected void compute() {
            if (right - left <= 10000) {
                quickSort(array, left, right);
                if (debugMode) {
                    //System.out.println("Thread " + Thread.currentThread().getId() + " directly sorted from " + left + " to " + right + ". ⏱: " + (System.currentTimeMillis() - startTime) + " ms");
                }
                return;
            }
            int pi = partition(array, left, right);

            if (debugMode) {
                System.out.println("Thread " + Thread.currentThread().getId() + " partitioned from " + left + " to " + right + " at pivot " + pi + ". ⏱: " + (System.currentTimeMillis() - startTime) + " ms");

            }

            invokeAll(new QuicksortTask(array, left, pi - 1, debugMode, startTime),
                    new QuicksortTask(array, pi + 1, right, debugMode, startTime));
        }

        private void quickSort(int[] arr, int low, int high) {
            if (low < high) {
                int pi = partition(arr, low, high);
                quickSort(arr, low, pi - 1);
                quickSort(arr, pi + 1, high);
            }
        }

        private int partition(int[] arr, int low, int high) {
            int pivot = arr[high];
            int i = low - 1;
            for (int j = low; j < high; j++) {
                if (arr[j] <= pivot) {
                    i++;
                    int temp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = temp;
                }
            }
            int temp = arr[i + 1];
            arr[i + 1] = arr[high];
            arr[high] = temp;
            return i + 1;
        }
    }
}

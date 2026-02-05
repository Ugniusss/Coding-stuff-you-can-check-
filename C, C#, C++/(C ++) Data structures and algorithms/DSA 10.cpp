// Visual Stuidio

#include <iostream>
#include <vector>
#include <algorithm>
#include <chrono>

using namespace std;

int konteiSK = 0;
int konteiSK1 = 0;

// -----------------LSD------------
void radixSortLSD(int* arr, int n) {
    int max_val = arr[0];
    for (int i = 1; i < n; i++) {
        if (arr[i] > max_val) {
            max_val = arr[i];
        }
    }
    for (int exp = 1; max_val / exp > 0; exp *= 10) {
        int* output = new int[n];
        int count[10] = { 0 };
        for (int i = 0; i < n; i++) {
            count[(arr[i] / exp) % 10]++;
        }
        for (int i = 1; i < 10; i++) {
            count[i] += count[i - 1];
            if (count[i] != 0) {
                konteiSK1++;
            }
        }
        for (int i = n - 1; i >= 0; i--) {
            output[count[(arr[i] / exp) % 10] - 1] = arr[i];
            count[(arr[i] / exp) % 10]--;
        }
        for (int i = 0; i < n; i++) {
            arr[i] = output[i];
        }
        delete[] output;
    }
}
// -----------------LSD------------
// 
// -----------------MSD------------
int getMax(int *array, int n) {
    int max = array[0];
    for (int i = 1; i < n; i++)
        if (array[i] > max)
            max = array[i];
    return max;
}


void countingSort(int* array, int size, int place) {
    const int max = 10;
    int* output = new int[size];
    int count[max];
    
    for (int i = 0; i < max; ++i)
        count[i] = 0;


    for (int i = 0; i < size; i++)
        count[(array[i] / place) % 10]++;


    for (int i = 1; i < max; i++) {
        count[i] += count[i - 1];
        if (count[i] != 0) {
            konteiSK++;
        }
    }
    

    for (int i = size - 1; i >= 0; i--) {
        output[count[(array[i] / place) % 10] - 1] = array[i];
        count[(array[i] / place) % 10]--;
    }

    for (int i = 0; i < size; i++)
        array[i] = output[i];

}

void radixSortMSD(int* array, int size) {
    int max = getMax(array, size);
    for (int place = 1; max / place > 0; place *= 10)
        countingSort(array, size, place);
}
// -----------------MSD------------

int main() {
    int n = 100000;
    int* arr1 = new int[n];
    int* arr2 = new int[n];

    for (int i = 0; i < n; ++i) {
        arr1[i] = n - i;
        arr2[i] = n - i;
        //cout << arr2[i] << " ";
    }
    cout << endl << endl;

    auto start = chrono::steady_clock::now();
    radixSortLSD(arr1, n);
    auto end = chrono::steady_clock::now();
    cout << "\t\t Konteineriu sk (LSD):" << konteiSK1 << endl;

    auto start1 = chrono::steady_clock::now();
    radixSortMSD(arr2, n);
    auto end1 = chrono::steady_clock::now();
    cout << "\t\t Konteineriu sk (MSD):" << konteiSK << endl;

    for (int i = 0; i < n; i++) {
        //cout << arr1[i] << " ";
        //cout << arr2[i] << " ";
    }
    cout << "Radix sort LSD elapsed time in milliseconds: "<< chrono::duration_cast<chrono::milliseconds>(end - start).count()<< " ms" << endl;
    cout << "Radix sort MSD elapsed time in milliseconds: "<< chrono::duration_cast<chrono::milliseconds>(end1 - start1).count()<< " ms" << endl;

    delete[] arr1;
    delete[] arr2;
    return 0;
}

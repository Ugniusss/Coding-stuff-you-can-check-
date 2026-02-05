// Visual Studio
// Ugnius Padolskis

#include <iostream>
#include <cstdlib>
#include <ctime>
#include <chrono>

using namespace std;

void ShakerSort(int arr[], int size);
int* generateRandomArray(int size);
int* copyArray(int* arr, int size);
void bubbleSort(int* arr, int size);
void CombSort(int* arr, int size);
int getNextGap(int gap);
void gnomeSort(int* arr, int size);
void insertionSort(int* arr, int size);
int shellSort(int* arr, int size);


int main(){
    int size = 10000;
    int *arr = generateRandomArray(size);
    int *arr2 = copyArray(arr, size);
    int *arr3 = copyArray(arr, size);
    int *arr4 = copyArray(arr, size);
    int *arr5 = copyArray(arr, size);
    int *arr6 = copyArray(arr, size);
    
    // SHAKER
    auto start = chrono::high_resolution_clock::now();
    ShakerSort(arr, size);
    auto end = chrono::high_resolution_clock::now();
    auto duration = std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count();
    cout << "Shaker: " << duration << " ms" << endl;
    
    // BUBBLE
    auto start2 = chrono::high_resolution_clock::now();
    bubbleSort(arr2, size);
    auto end2 = chrono::high_resolution_clock::now();
    auto duration2 = std::chrono::duration_cast<std::chrono::milliseconds>(end2 - start2).count();
    cout << "Bubble: " << duration2 << " ms" << endl;
    
    // COMB
    auto start3 = chrono::high_resolution_clock::now();
    CombSort(arr3, size);
    auto end3 = chrono::high_resolution_clock::now();
    auto duration3 = std::chrono::duration_cast<std::chrono::milliseconds>(end3 - start3).count();
    cout << "Comb: " << duration3 << " ms" << endl;
    
    // GNOME
    auto start4 = chrono::high_resolution_clock::now();
    gnomeSort(arr4, size);
    auto end4 = chrono::high_resolution_clock::now();
    auto duration4 = std::chrono::duration_cast<std::chrono::milliseconds>(end4 - start4).count();
    cout << "Gnome: " << duration4 << " ms" << endl;

    // INSERTION
    auto start5 = chrono::high_resolution_clock::now();
    insertionSort(arr5, size);
    auto end5 = chrono::high_resolution_clock::now();
    auto duration5 = std::chrono::duration_cast<std::chrono::milliseconds>(end5 - start5).count();
    cout << "Insertion: " << duration5 << " ms" << endl;

    // SHELL
    auto start6 = chrono::high_resolution_clock::now();
    shellSort(arr6, size);
    auto end6 = chrono::high_resolution_clock::now();
    auto duration6 = std::chrono::duration_cast<std::chrono::milliseconds>(end6 - start6).count();
    cout << "Shell: " << duration6 << " ms" << endl;

    for (int i = 0; i < size; i++) {
        //cout << arr6[i] << " ";
    }
    return 0;
}

int shellSort(int *arr, int size){
    for (int gap = size / 2; gap > 0; gap /= 2){
        for (int i = gap; i < size; i += 1){
            int temp = arr[i];
            int j;
            for (j = i; j >= gap && arr[j - gap] > temp; j -= gap)
                arr[j] = arr[j - gap];
            arr[j] = temp;
        }
    }
    return 0;
}


void insertionSort(int *arr, int size){
    int i, key, j;
    for (i = 1; i < size; i++){
        key = arr[i];
        j = i - 1;
        while (j >= 0 && arr[j] > key){
            arr[j + 1] = arr[j];
            j = j - 1;
        }
        arr[j + 1] = key;
    }
}

void gnomeSort(int *arr, int size){
    int index = 0;
    while (index < size) {
        if (index == 0)
            index++;
        if (arr[index] >= arr[index - 1])
            index++;
        else {
            swap(arr[index], arr[index - 1]);
            index--;
        }
    }
    return;
}


int getNextGap(int gap){
    gap = (gap * 10) / 13;
    if (gap < 1) {
        return 1;
    }
    return gap;
}

void CombSort(int *arr, int size){
    int gap = size;
    bool swapped = true;
    while (gap != 1 || swapped == true){
        gap = getNextGap(gap);
        swapped = false;
        for (int i = 0; i < size - gap; i++){
            if (arr[i] > arr[i + gap]){
                swap(arr[i], arr[i + gap]);
                swapped = true;
            }
        }
    }
}

void bubbleSort(int *arr, int size) {
    for (int i = 0; i < size - 1; i++) {
        for (int j = 0; j < size - i - 1; j++) {
            if (arr[j] > arr[j + 1]) {
                swap(arr[j], arr[j + 1]);
            }
        }
    }
}

int* generateRandomArray(int size) {
    //srand(time(NULL)); 
    int* arr = new int[size]; 
    for (int i = 0; i < size; i++) {
        arr[i] = rand() % 100; 
    }
    return arr; 
}

int* copyArray(int* arr, int size) {
    int* copy = new int[size]; 
    for (int i = 0; i < size; i++) {
        copy[i] = arr[i]; 
    }
    return copy; 
}

void ShakerSort(int arr[], int size) {
    int i, j, k;
    for (i = 0; i < size;) {
        for (j = i + 1; j < size; j++) {
            if (arr[j] < arr[j - 1])
                swap(arr[j], arr[j - 1]);
        }
        size--;
        for (k = size - 1; k > i; k--) {
            if (arr[k] < arr[k - 1])
                swap(arr[k], arr[k - 1]);
        }
        i++;
    }
}

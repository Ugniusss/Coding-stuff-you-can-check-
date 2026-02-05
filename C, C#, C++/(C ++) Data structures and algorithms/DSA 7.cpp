// Visual Studio
// Ugnius Padolskis

#include <iostream>
#include <cstdlib>
#include <ctime>
#include <chrono>
#include <iomanip>

#define MAX_NUM 10000 

using namespace std;

struct Node
{
    int key;
    struct Node* left, * right;
};

struct Node* newNode(int item)
{
    struct Node* temp = new Node;
    temp->key = item;
    temp->left = temp->right = NULL;
    return temp;
}


void ShakerSort(int arr[], int size);
int* generateRandomArray(int size);
int* copyArray(int* arr, int size);
void bubbleSort(int* arr, int size);
void CombSort(int* arr, int size);
int getNextGap(int gap);
void gnomeSort(int* arr, int size);
void insertionSort(int* arr, int size);
int shellSort(int* arr, int size);
void heapify(int arr[], int n, int i);
void heapSort(int arr[], int n);
void storeSorted(Node* root, int arr[], int& i);
Node* insert(Node* node, int key);
void treeSort(int arr[], int n);
int partition(int arr[], int start, int end);
void quickSort(int arr[], int start, int end);
void mergeSort(int array[], int const begin, int const end);
void merge(int array[], int const left, int const mid, int const right);

// Comp - Comparison/Palyginimai
// Pr - Priskyrimas

long int shakerCompCounter = 0;
int shakerPrCounter = 0;


int bubbleCompCounter = 0;
int bubblePrCounter = 0;


int combCompCounter = 0;
int combPrCounter = 0;


int gnomeCompCounter = 0;
int gnomePrCounter = 0;


int insertCompCounter = 0;
int insertPrCounter = 0;


int shellCompCounter = 0;
int shellPrCounter = 0;


int heapCompCounter = 0;
int heapPrCounter = 0;

int treeCompCounter = 0;
int treePrCounter = 0;

int quickCompCounter = 0;
int quickPrCounter = 0;

int mergeCompCounter = 0;
int mergePrCounter = 0;




int main() {
    int size = 10000;
    int* arr = generateRandomArray(size);
    int* arr2 = copyArray(arr, size);
    int* arr3 = copyArray(arr, size);
    int* arr4 = copyArray(arr, size);
    int* arr5 = copyArray(arr, size);
    int* arr6 = copyArray(arr, size);
    //int* arr7 = copyArray(arr, size);
    int* arr8 = copyArray(arr, size);
    int* arr9 = copyArray(arr, size);
    int* arr10 = copyArray(arr, size);
    int arr7[9] = { 3, 5 ,9, 1, 2 ,4 ,8 ,7, 6 };

    
    // SHAKER--------------------------------------------------------------------------------------------------
    auto start = chrono::high_resolution_clock::now();
    ShakerSort(arr, size);
    auto end = chrono::high_resolution_clock::now();
    auto duration = std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count();
    cout << "Shaker: " << duration << " ms,"<< setw(25) << "lyginimo operaciju sk: " << shakerCompCounter
        << ", priskyrimo op sk: " << shakerPrCounter << endl;

    // BUBBLE--------------------------------------------------------------------------------------------------
    auto start2 = chrono::high_resolution_clock::now();
    bubbleSort(arr2, size);
    auto end2 = chrono::high_resolution_clock::now();
    auto duration2 = std::chrono::duration_cast<std::chrono::milliseconds>(end2 - start2).count();
    cout << "Bubble: " << duration2 << " ms," << setw(25) << " lyginimo operaciju sk: " << bubbleCompCounter
        << ", priskyrimo op sk: " << bubblePrCounter << endl;
    
    // COMB--------------------------------------------------------------------------------------------------
    auto start3 = chrono::high_resolution_clock::now();
    CombSort(arr3, size);
    auto end3 = chrono::high_resolution_clock::now();
    auto duration3 = std::chrono::duration_cast<std::chrono::milliseconds>(end3 - start3).count();
    cout << "Comb: " << duration3 << " ms," << setw(25) << " lyginimo operaciju sk: " << combCompCounter
        << ", priskyrimo op sk: " << combPrCounter << endl;
    
    // GNOME--------------------------------------------------------------------------------------------------
    auto start4 = chrono::high_resolution_clock::now();
    gnomeSort(arr4, size);
    auto end4 = chrono::high_resolution_clock::now();
    auto duration4 = std::chrono::duration_cast<std::chrono::milliseconds>(end4 - start4).count();
    cout << "Gnome: " << duration4 << " ms," << setw(25) << " lyginimo operaciju sk: " << gnomeCompCounter
         << ", priskyrimo op sk: "<< gnomePrCounter << endl;
    
    // INSERTION--------------------------------------------------------------------------------------------------
    auto start5 = chrono::high_resolution_clock::now();
    insertionSort(arr5, size);
    auto end5 = chrono::high_resolution_clock::now();
    auto duration5 = std::chrono::duration_cast<std::chrono::milliseconds>(end5 - start5).count();
    cout << "Insertion: " << duration5 << " ms," << setw(25) << " lyginimo operaciju sk: " << insertCompCounter
         << ", priskyrimo op sk: "<< insertPrCounter << endl;
    
    // SHELL--------------------------------------------------------------------------------------------------
    auto start6 = chrono::high_resolution_clock::now();
    shellSort(arr6, size);
    auto end6 = chrono::high_resolution_clock::now();
    auto duration6 = std::chrono::duration_cast<std::chrono::milliseconds>(end6 - start6).count();
    cout << "Shell: " << duration6 << " ms," << setw(25) << " lyginimo operaciju sk: " << shellCompCounter
         << ", priskyrimo op sk: "<< shellPrCounter << endl;
    cout << endl;
    
    
    
    // HEAP--------------------------------------------------------------------------------------------------
    auto start7 = chrono::high_resolution_clock::now();
    for (int i = size / 2 - 1; i >= 0; --i) {
        heapify(arr7, size, MAX_NUM);
    }
    heapSort(arr7, 9);
    auto end7 = chrono::high_resolution_clock::now();
    auto duration7 = std::chrono::duration_cast<std::chrono::milliseconds>(end7 - start7).count();
     cout << "Heap: " << duration7 << " ms," << setw(25) << " lyginimo operaciju sk: " << heapCompCounter
         << ", priskyrimo op sk: " << heapPrCounter << endl;

    // TREE--------------------------------------------------------------------------------------------------
    auto start8 = chrono::high_resolution_clock::now();
    treeSort(arr8, size);
    auto end8 = chrono::high_resolution_clock::now();
    auto duration8 = std::chrono::duration_cast<std::chrono::milliseconds>(end8 - start8).count();
    cout << "Tree: " << duration8 << " ms," << setw(25) << " lyginimo operaciju sk: " << treeCompCounter
         << ", priskyrimo op sk: " << treePrCounter << endl;

    // QUICK--------------------------------------------------------------------------------------------------
    auto start9 = chrono::high_resolution_clock::now();
    quickSort(arr9, 0, size - 1);
    auto end9 = chrono::high_resolution_clock::now();
    auto duration9 = std::chrono::duration_cast<std::chrono::milliseconds>(end9 - start9).count();
    cout << "Quick: " << duration9 << " ms," << setw(25) << " lyginimo operaciju sk: " << quickCompCounter
         << ", priskyrimo op sk: " << quickPrCounter << endl;

    // MERGE--------------------------------------------------------------------------------------------------
    auto start10 = chrono::high_resolution_clock::now();
    mergeSort(arr10, 0, size - 1);
    auto end10 = chrono::high_resolution_clock::now();
    auto duration10 = std::chrono::duration_cast<std::chrono::milliseconds>(end10 - start10).count();
    cout << "Merge: " << duration10 << " ms," << setw(25) << " lyginimo operaciju sk: " << mergeCompCounter
        << ", priskyrimo op sk: " << mergePrCounter << endl;

    for (int i = 0; i < size; i++) {
        //cout << arr10[i] << " ";
    }
    return 0;
}
// MERGE
void merge(int array[], int const left, int const mid, int const right) {
    auto const subArrayOne = mid - left + 1;
    auto const subArrayTwo = right - mid;

    auto* leftArray = new int[subArrayOne], * rightArray = new int[subArrayTwo];

    for (auto i = 0; i < subArrayOne; i++) {
        leftArray[i] = array[left + i];
        mergePrCounter++;
    }

    for (auto j = 0; j < subArrayTwo; j++) {
        rightArray[j] = array[mid + 1 + j];
        mergePrCounter++;
    }

    auto indexOfSubArrayOne = 0, indexOfSubArrayTwo = 0;
    int indexOfMergedArray = left;

    while (indexOfSubArrayOne < subArrayOne && indexOfSubArrayTwo < subArrayTwo) {
        if (leftArray[indexOfSubArrayOne] <= rightArray[indexOfSubArrayTwo]) {
            array[indexOfMergedArray] = leftArray[indexOfSubArrayOne];
            indexOfSubArrayOne++;
        }
        else {
            array[indexOfMergedArray] = rightArray[indexOfSubArrayTwo];
            indexOfSubArrayTwo++;
        }
        indexOfMergedArray++;
        mergePrCounter++;
        mergeCompCounter++;
    }

    while (indexOfSubArrayOne < subArrayOne) {
        array[indexOfMergedArray] = leftArray[indexOfSubArrayOne];
        indexOfSubArrayOne++;
        indexOfMergedArray++;
        mergePrCounter++;
    }

    while (indexOfSubArrayTwo < subArrayTwo) {
        array[indexOfMergedArray] = rightArray[indexOfSubArrayTwo];
        indexOfSubArrayTwo++;
        indexOfMergedArray++;
        mergePrCounter++;
    }
    delete[] leftArray;
    delete[] rightArray;
}

void mergeSort(int array[], int const begin, int const end) {
    if (begin >= end) {
        return;
    }

    auto mid = begin + (end - begin) / 2;
    mergeSort(array, begin, mid);
    mergeSort(array, mid + 1, end);
    merge(array, begin, mid, end);
}

// QUICK
int partition(int arr[], int start, int end) {
    int pivot = arr[start];
    int count = 0;
    for (int i = start + 1; i <= end; i++) {
        if (arr[i] <= pivot) {
            count++;
            quickCompCounter++;
        }
    }
    int pivotIndex = start + count;
    swap(arr[pivotIndex], arr[start]);
    quickPrCounter++;
    int i = start, j = end;
    while (i < pivotIndex && j > pivotIndex) {
        while (arr[i] <= pivot) {
            i++;
            quickCompCounter++;
        }
        while (arr[j] > pivot) {
            j--; quickCompCounter++;
        }
        if (i < pivotIndex && j > pivotIndex) {
            swap(arr[i++], arr[j--]);
            quickPrCounter++;
        }
    }
    return pivotIndex;
}

void quickSort(int arr[], int start, int end) {
    if (start >= end)
        return;
    int p = partition(arr, start, end);
    quickSort(arr, start, p - 1);
    quickSort(arr, p + 1, end);
}

// TREE
void storeSorted(Node* root, int arr[], int& i) {
    if (root != NULL) {
        treePrCounter++;
        treeCompCounter++;
        storeSorted(root->left, arr, i);
        arr[i++] = root->key;
        storeSorted(root->right, arr, i);
    }
}

Node* insert(Node* node, int key) {
    if (node == NULL) return newNode(key);
    if (key < node->key) {
        node->left = insert(node->left, key);
        treePrCounter++;
        treeCompCounter++;
    }
    else if (key > node->key) {
        node->right = insert(node->right, key);
        treePrCounter++;
        treeCompCounter++;
    }
    return node;
}


void treeSort(int arr[], int n) {
    struct Node* root = NULL;
    root = insert(root, arr[0]);
    for (int i = 1; i < n; i++) {
        root = insert(root, arr[i]);
    }
    int i = 0;
    storeSorted(root, arr, i);
}

// HEAP
void heapify(int arr[], int n, int i) {
    int largest = i;
    int l = 2 * i + 1;
    int r = 2 * i + 2;

    if (l < n && arr[l] > arr[largest]) {
        heapCompCounter++;
        largest = l;
    }

    if (r < n && arr[r] > arr[largest]) {
        heapCompCounter++;
        largest = r;
    }

    if (largest != i) {
        heapPrCounter++;
        swap(arr[i], arr[largest]);

        heapify(arr, n, largest);
    }
}

void heapSort(int arr[], int n) {
    for (int i = n / 2 - 1; i >= 0; i--) {
        heapify(arr, n, i);
    }

    for (int i = n - 1; i >= 0; i--) {
        heapPrCounter++;
        swap(arr[0], arr[i]);
        heapify(arr, i, 0);
    }
}

int shellSort(int* arr, int size) {
    for (int gap = size / 2; gap > 0; gap /= 2) {
        for (int i = gap; i < size; i += 1) {
            int temp = arr[i];
            int j;
            for (j = i; j >= gap && arr[j - gap] > temp; j -= gap) {
                arr[j] = arr[j - gap];
                shellCompCounter++;
                shellPrCounter++;
            } 
            arr[j] = temp;
            shellPrCounter++;
        }
    }
    return 0;
}


void insertionSort(int* arr, int size) {
    int i, key, j;
    for (i = 1; i < size; i++) {
        key = arr[i];
        j = i - 1;
        while (j >= 0 && arr[j] > key) {
            arr[j + 1] = arr[j];
            j = j - 1;
            insertCompCounter++;
            insertPrCounter++;
        }
        arr[j + 1] = key;
        insertPrCounter++;
    }
}

void gnomeSort(int* arr, int size) {
    int index = 0;
    while (index < size) {
        if (index == 0) {
            index++;
        }
            
        if (arr[index] >= arr[index - 1]) {
            index++;
            gnomeCompCounter++;
        }
            
        else {
            swap(arr[index], arr[index - 1]);
            index--;
            gnomePrCounter++;
            gnomePrCounter++;
            gnomePrCounter++;
        }
    }
    return;
}


int getNextGap(int gap) {
    gap = (gap * 10) / 13;
    if (gap < 1) {
        return 1;
    }
    return gap;
}

void CombSort(int* arr, int size) {
    int gap = size;
    bool swapped = true;
    while (gap != 1 || swapped == true) {
        gap = getNextGap(gap);
        swapped = false;
        for (int i = 0; i < size - gap; i++) {
            if (arr[i] > arr[i + gap]) {
                swap(arr[i], arr[i + gap]);
                swapped = true;
                combPrCounter++;
                combPrCounter++;
                combPrCounter++;
            }
            combCompCounter++;
        }
    }
}

void bubbleSort(int* arr, int size) {
    for (int i = 0; i < size - 1; i++) {
        for (int j = 0; j < size - i - 1; j++) {
            bubbleCompCounter++;
            if (arr[j] > arr[j + 1]) {
                swap(arr[j], arr[j + 1]);
                bubblePrCounter++;
            }
        }
    }
}

int* generateRandomArray(int size) {
    //srand(time(NULL)); 
    int* arr = new int[size];
    for (int i = 0; i < size; i++) {
        arr[i] = rand() % MAX_NUM;
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
            shakerCompCounter++;
            if (arr[j] < arr[j - 1]) {
                swap(arr[j], arr[j - 1]);
                shakerPrCounter++;
                shakerPrCounter++;
                shakerPrCounter++;
            }
                
        }
        size--;
        for (k = size - 1; k > i; k--) {
            shakerCompCounter++;
            if (arr[k] < arr[k - 1]) {
                swap(arr[k], arr[k - 1]);
                shakerPrCounter++;
                shakerPrCounter++;
                shakerPrCounter++;
            }
        }
        i++;
    }
}

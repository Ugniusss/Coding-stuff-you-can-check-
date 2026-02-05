// Visual Studio
// Ugnius Padolskis


#include <iostream>
#include <cstdlib>
#include <string>
#include <ctime>
#include <chrono>
#include <iomanip>
#include "headr.h"

#define MAX_NUM 10000 

using namespace std;

template <typename T>
Arr<T>::Arr(int m) {
    this->max = m;
    this->kiek = 0;
    this->arr = new T[this->max];
}

template <typename T>
int Arr<T>::size() {
    return this->kiek;
}

template <typename T>
T& Arr<T>::operator[] (int i) {
    if (i < 0 || i >= this->kiek) {
        cout << "n";
    }
    else {
        return this->arr[i];
    }
}

template <typename T>
void Arr<T>::add(const T& value) {
    if (this->kiek < this->max) {
        this->arr[kiek++] = value;
    }
    else {
        cout << "n";
    }
}

// HYBRID SORT
template <typename T>
void Hybrid(Arr<T>& arr, int left, int right) {
    if (left < right) {
        int size = right - left + 1;
        if (size <= 10) {
            insertionSort(arr, left, right);
        }
        else if (size <= 100) {
            bubbleSort(arr, left, right);
        }
        else {
            int mid = left + (right - left) / 2;
            Hybrid(arr, left, mid);
            Hybrid(arr, mid + 1, right);
            merge(arr, left, mid, right);
        }
    }
}

// INSERTION SORT
template <typename T>
void insertionSort(Arr<T>& arr, int left, int right) {
    for (int i = left + 1; i <= right; ++i) {
        T key = arr[i];
        int j = i - 1;
        while (j >= left && arr[j] > key) {
            arr[j + 1] = arr[j];
            j--;
        }
        arr[j + 1] = key;
    }
}

// BUBBLE SORT
template <typename T>
void bubbleSort(Arr<T>& arr, int left, int right) {
    for (int i = left; i <= right; ++i) {
        for (int j = right; j > i; --j) {
            if (arr[j] < arr[j - 1]) {
                swap(arr[j], arr[j - 1]);
            }
        }
    }
}

// MERGE SORT
template <typename T>
void merge(Arr<T>& arr, int left, int mid, int right) {
    int subArrayOne = mid - left + 1;
    int subArrayTwo = right - mid;

    Arr<T> le(subArrayOne);
    Arr<T> ri(subArrayTwo);

    for (int i = 0; i < subArrayOne; ++i) {
        le.add(arr[left + i]);
    }

    for (int j = 0; j < subArrayTwo; ++j) {
        ri.add(arr[mid + 1 + j]);
    }

    int indexOfSubArrayOne = 0;
    int indexOfSubArrayTwo = 0;
    int indexOfMergedArray = left;

    while (indexOfSubArrayOne < subArrayOne && indexOfSubArrayTwo < subArrayTwo) {
        if (le[indexOfSubArrayOne] <= ri[indexOfSubArrayTwo]) {
            arr[indexOfMergedArray] = le[indexOfSubArrayOne];
            indexOfSubArrayOne++;
        }
        else {
            arr[indexOfMergedArray] = ri[indexOfSubArrayTwo];
            indexOfSubArrayTwo++;
        }
        indexOfMergedArray++;
    }

    while (indexOfSubArrayOne < subArrayOne) {
        arr[indexOfMergedArray] = le[indexOfSubArrayOne];
        indexOfSubArrayOne++;
        indexOfMergedArray++;
    }

    while (indexOfSubArrayTwo < subArrayTwo) {
        arr[indexOfMergedArray] = ri[indexOfSubArrayTwo];
        indexOfSubArrayTwo++;
        indexOfMergedArray++;
    }
}

template <typename T>
void mergeSort(Arr<T>& arr, int left, int right) {
    if (left < right) {
        int mid = left + (right - left) / 2;
        mergeSort(arr, left, mid);
        mergeSort(arr, mid + 1, right);
        merge(arr, left, mid, right);
    }
}



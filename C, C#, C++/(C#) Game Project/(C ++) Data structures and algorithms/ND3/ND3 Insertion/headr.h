// Visual Studio
// Ugnius Padolskis

#pragma once
#ifndef hND3_H
#define hND3_H

using namespace std;
template <typename T>
class Arr {
private:
    T* arr;
    int kiek;
    int max;

public:
    Arr(int m);
    ~Arr() {};
    int size();
    T& operator [] (int i);
    void add(const T& value);

};

template<typename T>
void insertionSort(Arr<T>& arr, int size);

template <typename T>
void mergeSort(Arr<T>& arr, int const left, int const right);

template <typename T>
void merge(Arr<T>& arr, int const left, int const mid, int const right);

template <typename T>
void bubbleSort(Arr<T>& array);

template <typename T>
void Hybrid(Arr<T>& arr, int size, int p);

#endif 
#include <stdio.h>
#include <time.h>
#include <stdlib.h>
#include <string.h>
#include "file.h"

int main() {
    char first[] = "first.bin";
    char secon[] = "secon.bin";
    saveCount = 0;
    loadCount = 0;
    srand(time(NULL));
    int k = 5;
    int s = 0;
    int* arr1, * arr2, *arr3;
    arr1 = malloc(k * sizeof(int));
    arr2 = malloc(k * sizeof(int));
    arr3 = malloc(k * sizeof(int));
    //1 
    fillArray(arr1, k);
    fillArray(arr2, k);
    fillArray(arr3, k);
    //2 
    saveToFile(arr1, k, first);
    //3 
    saveToFile(arr3, k, first);
    //4 
    loadFromFile(arr1, k, first);
    //5 
    saveToFile(arr2, k, secon);
    //6
    loadFromFile(arr2, k, secon);
    //7
    loadFromFile(arr2, k, first);
    if (saveCount == loadCount) {
        printf("all good");
    }
    else {
        printf("smt wrong");
    }
    return 0;
}

void fillArray(int arr[], int k) {
    for (int i = 0; i < k; i++) {
        arr[i] = rand() % 10;
    }
}

void printArray(int arr[], int k) {
    for (int i = 0; i < k; i++) {
        printf("%d ", arr[i]);
    }
}
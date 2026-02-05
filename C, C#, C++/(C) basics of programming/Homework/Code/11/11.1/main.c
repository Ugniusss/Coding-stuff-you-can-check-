#include <stdio.h>
#include <time.h>
#include <stdlib.h>
#include <string.h>
#include "file.h"


int main() {
    srand(time(NULL));
    int k = 5;
    int s = 0;
    int *arr, *arr1;
    arr = malloc(k * sizeof(int));
    arr1 = malloc(k * sizeof(int));
//1 
    fillArray(arr, k);
    for (int i = 0; i < k; ++i) {
        arr1[i] = arr[i];
    }
//2 
    printf("First : ");
    printArray(arr, k);
    printf("\n");
//3 
    saveToFile(arr, k);
//4 
    loadFromFile(arr, k);
//5 
    printf("Second: ");
    printArray(arr, k);
    for (int i = 0; i < k; ++i) {
        if (arr1[i] != arr[i]) {
            s++;
        }
    }
    if (s > 0) {
        printf("something’s wrong");
    }
    else {
        printf("all good");
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

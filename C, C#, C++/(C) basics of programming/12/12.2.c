//Visual Studio
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <stdbool.h>

//------------
int* randomArray(int size);
void printArray(int size, int* array);
void freeArray(int* array);
bool check(int size, int* array);
void check2(int size, int* array);
void bubble(int size, int* array);
void quick(int size, int* array, int first);
void insertion(int size, int* array);
void selection(int size, int* array);
void merge(int size, int* array, int first);
void test1();
void test2();
void test3();

//------------
int counter;
//------------

int main() {
    counter = 0;
  //srand(time(NULL));
    test1();
    test2();
    test3();
    return 0;
}

// timeFinds
double timee(int size, int* array) {
    clock_t start = clock();
    bubble(size, array);
    clock_t end = clock();
    double laikas = (double)(end - start) / CLOCKS_PER_SEC;
    return laikas;
}

// Test1
void test1() {
    int size = 1000;
    int* array = randomArray(size);
    int array1[1000];
    int array2[1000];
    int array3[1000];
    int array4[1000];
    for (int i = 0; i < size; ++i) {
        array1[i] = array[i];
        array2[i] = array[i];
        array3[i] = array[i];
        array4[i] = array[i];
    }
    printArray(size, array);
    printf("\n");

    //(bubble sort) 
    printf("bubble sort:    ");
    double laikas = timee(size, array);
    printArray(size, array);
    check2(size, array);
    printf("\t %f seconds\n", laikas);


    //(quick sort)
    counter = 0;
    printf("quick sort:     ");
    laikas = timee(size, array);
    quick(size - 1, array1, 0);
    printArray(size, array1);
    check2(size, array1);
    printf("\t %f seconds\n", laikas);

    //(insertion)
    printf("insertion sort: ");
    laikas = timee(size, array);
    insertion(size, array2);
    printArray(size, array2);
    check2(size, array2);
    printf("\t %f seconds\n", laikas);

    //(selection)
    printf("selection sort: ");
    laikas = timee(size, array);
    selection(size, array3);
    printArray(size, array3);
    check2(size, array3);
    printf("\t %f seconds\n", laikas);

    //(merge)
    printf("merge sort:     ");
    laikas = timee(size, array);
    merge(size - 1, array4, 0);
    printArray(size, array4);
    check2(size, array4);
    printf("\t %f seconds\n", laikas);

    printf("\n");
    freeArray(array);
}

// Test2
void test2() {
    int size = 10;
    int* array = randomArray(size);
    int array1[10];
    int array2[10];
    int array3[10];
    int array4[10];
    for (int i = 0; i < size; ++i) {
        array1[i] = array[i];
        array2[i] = array[i];
        array3[i] = array[i];
        array4[i] = array[i];
    }
    printArray(size, array);
    printf("\n");

    //(bubble sort) 
    printf("bubble sort:    ");
    double laikas = timee(size, array);
    printArray(size, array);
    check2(size, array);
    printf("\t %f seconds\n", laikas);


    //(quick sort)
    counter = 0;
    printf("quick sort:     ");
    laikas = timee(size, array);
    quick(size - 1, array1, 0);
    printArray(size, array1);
    check2(size, array1);
    printf("\t %f seconds\n", laikas);

    //(insertion)
    printf("insertion sort: ");
    laikas = timee(size, array);
    insertion(size, array2);
    printArray(size, array2);
    check2(size, array2);
    printf("\t %f seconds\n", laikas);

    //(selection)
    printf("selection sort: ");
    laikas = timee(size, array);
    selection(size, array3);
    printArray(size, array3);
    check2(size, array3);
    printf("\t %f seconds\n", laikas);

    //(merge)
    printf("merge sort:     ");
    laikas = timee(size, array);
    merge(size - 1, array4, 0);
    printArray(size, array4);
    check2(size, array4);
    printf("\t %f seconds\n", laikas);

    printf("\n");
    freeArray(array);
}

// Test3
void test3() {
    int size = 10;
    int* array = randomArray(size);
    int array1[10];
    int array2[10];
    int array3[10];
    int array4[10];
    for (int i = 0; i < size; ++i) {
        array1[i] = array[i];
        array2[i] = array[i];
        array3[i] = array[i];
        array4[i] = array[i];
    }
    printArray(size, array);
    printf("\n");

    //(bubble sort) 
    printf("bubble sort:    ");
    double laikas = timee(size, array);
    printArray(size, array);
    check2(size, array);
    printf("\t %f seconds\n", laikas);


    //(quick sort)
    counter = 0;
    printf("quick sort:     ");
    laikas = timee(size, array);
    quick(size - 1, array1, 0);
    printArray(size, array1);
    check2(size, array1);
    printf("\t %f seconds\n", laikas);

    //(insertion)
    printf("insertion sort: ");
    laikas = timee(size, array);
    insertion(size, array2);
    printArray(size, array2);
    check2(size, array2);
    printf("\t %f seconds\n", laikas);

    //(selection)
    printf("selection sort: ");
    laikas = timee(size, array);
    selection(size, array3);
    printArray(size, array3);
    check2(size, array3);
    printf("\t %f seconds\n", laikas);

    //(merge)
    printf("merge sort:     ");
    laikas = timee(size, array);
    merge(size - 1, array4, 0);
    printArray(size, array4);
    check2(size, array4);
    printf("\t %f seconds\n", laikas);

    printf("\n");
    freeArray(array);
}
// C (start) 
// (bubble sort)
void bubble(int size, int* array) {
    int a;
    counter = 0;
    for (int i = 0; i < size - 1; ++i) {
        for (int j = i + 1; j < size; ++j) {
            if (array[i] > array[j]) {
                counter++;
                a = array[i];
                array[i] = array[j];
                array[j] = a;
            }
        }
    }
}

// (quick sort)
void quick(int size, int* array, int first) {
    int i, j, num, a;
    if (first < size) {
        num = first;
        i = first;
        j = size;

        while (i < j) {
            counter++;
            while (array[i] <= array[num] && i < size) {
                i++;
            }
            while (array[j] > array[num]) {
                j--;
            }
            if (i < j) {
                a = array[i];
                array[i] = array[j];
                array[j] = a;
            }
        }
        a = array[num];
        array[num] = array[j];
        array[j] = a;
        quick(j - 1, array, first);
        quick(size, array, j + 1);
    }
}
// (insertion)
void insertion(int size, int* array) {
    int a, t;
    counter = 0;
    for (int i = 1; i < size; ++i) {
        a = array[i];
        t = i - 1;
        while (a < array[t] && t >= 0) {
            counter++;
            array[t + 1] = array[t];
            t--;
        }
        array[t + 1] = a;
    }
}

// (selection)
void selection(int size, int* array) {
    int a;
    counter = 0;
    int min;
    for (int i = 0; i < size - 1; i++) {
        min = i;
        for (int j = i + 1; j < size; j++) {
            counter++;
            if (array[j] < array[min]) {
                min = j;
            }
        }
        a = array[i];
        array[i] = array[min];
        array[min] = a;
    }
}

// (merge)
void merge(int size, int* array, int first) {
    counter = 0;
    if (first < size) {
        int a = first + (size - first) / 2;
        merge(a, array, first);
        merge(size, array, a + 1);
        int i, j, k;
        int n1 = a - first + 1;
        int n2 = size - a;
        int* L = malloc(n1 * sizeof(int));
        int* R = malloc(n2 * sizeof(int));
        for (i = 0; i < n1; ++i) {
            L[i] = array[first + i];
        }

        for (j = 0; j < n2; ++j) {
            R[j] = array[a + 1 + j];
        }

        i = 0;
        j = 0;
        k = first;
        while (i < n1 && j < n2) {
            counter++;
            if (L[i] <= R[j]) {
                array[k] = L[i];
                i++;
            }
            else {
                array[k] = R[j];
                j++;
            }
            k++;
        }
        while (i < n1) {
            array[k] = L[i];
            i++;
            k++;
        }
        while (j < n2) {
            array[k] = R[j];
            j++;
            k++;
        }
    }
}
// C (end)

// B (start)
void check2(int size, int* array) {
    if (check(size, array)) {
        printf(" <-- Sorted, ");
    }
    else {
        printf(" <-- Not sorted, ");
    }
    printf("counter: %d, ", counter);
}
bool check(int size, int* array) {
    for (int i = 0; i < size - 1; ++i) {
        for (int j = i + 1; j < size; ++j) {
            if (array[i] > array[j]) {
                return false;
            }
        }
    }
    return true;
}
// B (end)


// A (start)
int* randomArray(int size) {
    int* array = malloc(size * sizeof(int));
    for (int i = 0; i < size; ++i) {
        array[i] = rand() % 10; // random number (0,10)
    }
    return array;
}

void printArray(int size, int* array) {
    printf("Array: ");
    for (int i = 0; i < size; ++i) {
        printf("%d ", array[i]);
    }
}

void freeArray(int* array) {
    free(array);
}
// A (end)
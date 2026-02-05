//Visual Studio
/*
Užduotis 1a.
Apibrėžkite funkciją createArray, kuri dinaminėje atmintyje leidžia sukurti (naują) sveikųjų skaičių masyvą, užpildytą
atsitiktinėmis reikšmėmis iš nurodyto intervalo. Ši funkcija gauna būsimo masyvo dydį size, išskiria atitinkamo dydžio
bloką dinaminėje atmintyje, ir užpildo jį atsitiktiniais skaičiais iš intervalo [low; high]. Paskutiniajam veiksmui atlikti, jei
norite, galite pasinaudoti ankstesnėse (7) pratybose parengta funkcija generateArray. Funkcija createArray sėkmės
atveju turi grąžinti rodyklę į pirmą naujai sukurto masyvo elementą, nesėkmės atveju – NULL.
*/
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <time.h>
//Prototype---
int* createArraay(int s);
void generateArray(int* array[], int size, int low, int high);
//Prototype---

int main() {
	int size = 5;			//Can be edited
	int low = 1;			//Can be edited
	int high = 10;			//Can be edited
	int* array;
	array = createArraay(size);
	generateArray(array, size, low, high);
	printf("\n%d ", *(array));
	return 0;
}

int* createArraay(int s) {
	int* p = (int*)malloc(s * sizeof(int));
	return p;
}


void generateArray(int* array[], int size, int low, int high) {
	srand(time(NULL));
	for (int i = 0; i < size*2; i = i + 2) {
		*(array + i) = rand() % high + low;
		//printf("%d ", array[i]);
	}
}

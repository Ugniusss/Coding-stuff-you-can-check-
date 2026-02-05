//Visual Studio
/*
Užduotis 4.
Apibrėžkite funkciją, kuri leidžia užpildyti masyvą atsitiktinėmis reikšmėmis.
Funkcijos prototipas: void generateArray(int data[], int size, int low, int high)
Aprašymas: kiekvienam iš size elementų, esančių masyve data, funkcija turi priskirti atsitiktinę reikšmę iš intervalo [low;
high]; nieko papildomai grąžinti nereikia, t.y. funkcijos rezultatas yra masyve esančios (naujos) reikšmės
*/
#include <stdio.h>
#include <stdlib.h>   
//Prototype---
void generateArray(int data[], int size, int low, int high);
//Prototype---

int main() {
	int data[100];
	int size, low, high;
	printf("Enter array size and range: ");
	scanf("%d%d%d", &size, &low, &high);
	generateArray(data, size, low, high);
	return 0;
}

void generateArray(int data[], int size, int low, int high) {
	srand(time(NULL));
	for (int i = 0; i < size; ++i) {
		data[i] = rand() % high + low;
		printf("%d ", data[i]);
	}
}
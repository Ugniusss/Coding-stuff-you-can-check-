//Visual Studio
/*
Užduotis 4.
Apibrėžkite funkciją splitData, kuri moka vieną masyvą padalinti į dvi dalis, dinaminėje atmintyje sukurdama du naujus
masyvus (į pirmą masyvą įrašydama skaičius esančius pradinio masyvo pradžioje, o į antrą masyvą – pradinio masyvo
likusią dalį). Ši funkcija gauna penkis parametrus. Du pirmi parametrai leidžia gauti pradinio masyvo duomenis ir dydį,
trečias parametras nurodo pirmosios dalies dydį (pagal jį nustatoma, kurioje vietoje vyksta perskėlimas), o likę du
parametrai skirti perduoti (grąžinti) tuos du naujai sukurtus masyvus (pirmųjų elementų adresus) iš funkcijos į likusią
programą. Funkcija pirmiausia patikrina, ar visų (!) parametrų reikšmės korektiškos, tuomet sukuria du naujus atitinkamų
dydžių masyvus dinaminėje atmintyje, po ko perkelia reikiamą skaičių elementų į atitinkamai pirmą ir į antrą masyvą.
Funkcija tikisi, jog iki ją iškviečiant šie du masyvai dar nebus sukurti, t.y. tikisi, jog gaus parametrų reikšmes lygias NULL ir
sukurs masyvus savo viduje. Sėkmės atveju, funkcija grąžina skaičių 0, nesėkmės atveju -1
*/
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <time.h>
#define A_SIZE 7
//Prototype---
int splitData(int size, int array[], int fsize, int* sk1, int* sk2);
//Prototype---

int main() {
	int size = 7;
	int *array[A_SIZE] = { 1,2,3,4,5,6,7 };
	int fsize = 5;
	int *sk1 = NULL, *sk2 = NULL;
	printf("%d", splitData(size, array, fsize, &sk1, &sk2));
	return 0;
}

int splitData(int size, int array[], int fsize, int* sk1, int* sk2) {
	if (size > 0) {
		
		sk1 = (int*)malloc(fsize * sizeof(int));
		sk2 = (int*)malloc((size - fsize) * sizeof(int));
		for (int i = 0; i < fsize * 2; i = i + 2) {
			*(sk1 + i) = *(array + i);
			//printf("(1):%d\n", sk1[i]);
		}
		for (int i = fsize * 2; i < size * 2; i = i + 2) {
			*(sk2 + i) = *(array + i);
			//printf("  (2):%d\n", sk2[i]);
		}
		return 0;
	}
	else {
		return -1;
	}
}
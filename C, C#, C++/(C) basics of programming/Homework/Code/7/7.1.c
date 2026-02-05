//Visual Studio
/*
Užduotis 1.
Apibrėžkite funkciją, kuri patikrina, ar duotas skaičius papuola į nustatytą intervalą.
Funkcijos prototipas: int isInRange(int number, int low, int high)
Aprašymas: funkcija turi grąžinti true, jei skaičius number priklauso intervalui [low; high] (su sąlyga, kad low neviršija
high), kitaip ji turi grąžinti false.
*/
#include <stdio.h>
//Prototype---
int isInRange(int number, int low, int high);
//Prototype---

int main() {
	int number = 0, low = 0, high = 0;
	isInRange(number, low, high);
	return 0;
}

//Finds if number is in range
int isInRange(int number, int low, int high) {
	printf("Enter a number and a range: ");
	scanf("%d%d%d", &number, &low, &high);
	if (low <= high) {
		if (number >= low && number <= high) {
			printf("True");
		}
		else {
			printf("False");
		}
	}
	else {
		printf("False");
	}
}
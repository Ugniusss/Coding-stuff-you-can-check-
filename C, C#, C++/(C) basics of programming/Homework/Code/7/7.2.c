//Visual Studio
/*
Užduotis 2.
Apibrėžkite funkciją, kuri suskaičiuoja duoto skaičiaus faktorialą.
Funkcijos prototipas: int getFactorial(int number)
Aprašymas: funkcija skaičiavimams naudojasi rekursija ir grąžina neneigiamo skaičiaus number faktorialą, o jei jo
suskaičiuoti negalima – grąžina 0.
Įdomu: apibrėžkite (kitą) funkciją, kuri duoda tą patį rezultatą, bet rekursija nesinaudoja.
*/
#include <stdio.h>
//Prototype---
int getFactorial(int number);
int getFactorial2(int number);
//Prototype---

int main() {
	int number = 0;
	printf("Enter a positive number: ");
	scanf("%d", &number);
	if (number < 1) {
		printf("0");
	}
	else {
		printf("Recursive: %d\n", getFactorial(number));
		printf("Normal: %d", getFactorial2(number));
	}
	return 0;
}

int getFactorial(int number) {
	if (number == 1) {
		return 1;
	}
	else {
		return  number*getFactorial(number-1);
	}
}

int getFactorial2(int number) {
	int sum = 1;
	while (number > 1) {
		sum = sum * number;
		number--;
	}
	return sum;
}
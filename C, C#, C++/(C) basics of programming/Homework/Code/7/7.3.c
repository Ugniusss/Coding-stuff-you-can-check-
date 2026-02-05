//Visual Studio
/*
Užduotis 3.
Apibrėžkite funkciją, kuri leidžia gauti (įvesti) vieną skaičių iš standartinio įvedimo srauto.
Funkcijos prototipas: int getPositiveNumber(char *msg)
Aprašymas: funkcija atspausdina į ekraną tekstinę eilutę msg (kurioje tikisi gauti vartotojui skirtą pranešimą apie tai, ką
reikia įvesti), ir ją (eilutę ekrane) kartoja (prašo vartotojo įvesti vėl ir vėl) tol, kol vartotojas neįveda vieno skaičiaus
eilutėje, kurį ši funkcija ir grąžina.
*/
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <ctype.h>

//Prototype---
int getPositiveNumber(char* msg);
//Prototype---

int main() {
	char msg[] = { "Enter a positive number: " };
	printf("%d",getPositiveNumber(&msg));
	return 0;
}

int getPositiveNumber(char* msg) {
	
	AGAIN:
	char a[100];
	printf("%s\n", msg);
	scanf("%s", &a);
	int n = atoi(a);
	if (n > 0) {
		return n;
	}
	goto AGAIN;
	return getPositiveNumber(&msg);
}

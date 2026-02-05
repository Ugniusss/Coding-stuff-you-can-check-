//Visual Studio
/*
Užduotis 5.
Apibrėžkite funkciją, kuri grąžina failo dydį baitais.
Funkcijos prototipas: long getFileSize(char *fileName)
Aprašymas: funkcija gauna failo vardą kaip parametrą, bando jį atidaryti (skaitymui binariniu režimu), ir peršokus į failo
pabaigą (fseek) sužinoti (ftell) kiek baitų jis užima; tą baitų skaičių funkcija ir grąžina (patikrinkite su OS rodomomis
reikšmėmis), o jei kažkas nepavyko (pvz. tokio failo nėra) – funkcija grąžina reikšmę -1
*/
#include <stdio.h>
//Prototype---
long getFileSize(char* fileName);
//Prototype---

int main() {
	char fn[100] = "";
	printf("Enter file name ");
	scanf("%s", &fn);
	printf("%d",getFileSize(fn));
	return 0;
}

long getFileSize(char* fileName) {
	int a;
	FILE* fp = fopen(fileName, "rb");
	if (fp == NULL) {
		return -1;
	}
	fseek(fp, 0L, SEEK_END);
	a = ftell(fp);
	fclose(fp);
	return a;
}
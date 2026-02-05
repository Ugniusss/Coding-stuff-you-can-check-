//Visual Studio
/*
Užduotis 6.
Apibrėžkite funkciją, skirtą vartotojo sąsajoje pateikiamam meniu spausdinti ir vartotojo pasirinkimui gauti.
Funkcijos prototipas: int showMenu(char *menuTitle, char *menuOptions[], int menuSize, char *inputMsg)
Aprašymas: funkcija turi atspausdinti meniu antraštę (parametras menuTitle), tuomet atspausdinti meniu iš menuSize
pasirinkimų, kur meniu elementų (eilučių) pavadinimai pateikiami (eilučių) masyve menuOptions. Tuomet funkcija
atspausdina tekstinę eilutę inputMsg, kurioje perduodamas vartotojui skirtas tekstas, kuriame prašoma įvesti savo
pasirinkimą, ir gavusi iš vartotojo skaičių (atitinkamo meniu punkto numerį) jį ir grąžina; jeigu vartotojo įvedimas
nekorektiškas (įvedamas ne skaičius arba tokio meniu punkto nėra), visas spausdinimo procesas (antraštė, meniu,
prašymas įvesti) kartojamas iš naujo, tol, kol įvedimas netaps korektiškas.
*/
#include <stdio.h>
//Prototype---
int showMenu(char* menuTitle, char* menuOptions[], int menuSize, char* inputMsg);
//Prototype---

int main() {
	char mT[] = { "Pavadinimas" };
	char *mO[3][8] = { "Option1","Option2" ,"Option3" };
	char iM[27] = {"Choose your option (1-3): "};
	int *mS = 3;
	printf("%d",showMenu(mT, mO, mS, iM));
	return 0;
}

int showMenu(char* menuTitle, char* menuOptions[], int menuSize, char* inputMsg) {
	int choice = 0;
	AGAIN:
	printf("%s\n", menuTitle);
	for (int i = 0; i < menuSize; ++i) {
		printf("%s ", menuOptions[i]);
	}
	printf("\n%s", inputMsg);
	scanf("%d", &choice);
	if (choice > menuSize || choice < 1) {
		goto AGAIN;
	}
	return choice;
}
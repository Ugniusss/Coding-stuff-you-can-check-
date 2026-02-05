//Visual Studio
/*Užduotis 1.
Parašykite programą, kuri savo viduje iš eilės vieną po kito atlieka TIKSLIAI šiuos žingsnius :
a) apibrėžia masyvą, galinti sutalpinti 10 elementų, tame pačiame sakinyje inicializuodama juos nulinėmis reikšmėmis
b) atspausdina visą masyvą į ekraną
c) pačiam pirmam, ketvirtam ir dešimtam masyvo elementams priskiria reikšmes atitinkamai 1, 2 ir 3
d) ištrina iš masyvo trečią elementą
e) įterpia į masyvą naują elementą su reikšme 4, taip, kad po įterpimo jis būtų septintas
f) atspausdina visą masyvą į ekraną
g) paprašo vartotojo įvesti du skaičius(x ir y), ir masyvo elementui su indeksu x nustato naują reikšmę, lygią y
h) paprašo vartotojo įvesti vieną skaičių(x), ir ištrina iš masyvo elementą su indeksu x
į) paprašo vartotoją įvesti du skaičius(x ir y), ir į masyvą įterpia naują elementą su reikšme y, taip, kad po įterpimo jo
indeksas būtų x
j) atspausdina visą masyvą į ekrana*/

#include <stdio.h>

void Prints(int max, int m[]);

int main() {
	int max = 10;
	int i = 0;

//a
	int m[10] = {0,0,0,0,0,0,0,0,0,0};

//b
	Prints(max, m);

//c
	m[0] = 1;
	m[3] = 2;
	m[9] = 3;
	//Prints(max, m);
	// 
//d
	for (int i = 2; i < 9; ++i) {
		m[i] = m[i + 1];
	}
	max--;
	//Prints(max, m);
	
//e
	max++;
	for (int i = 6; i < 9-1; ++i) {
		m[i + 1] = m[i];
	}
	m[6] = 4;
	//Prints(max, m);

//f
	Prints(max, m);

//g
	int x, y;
	printf("Enter 2 numbers (x, y) ");
	scanf("%d%d", &x, &y);
	m[x - 1] = y;
	//Prints(max, m);

//h
	printf("Enter 1 number x ");
	scanf("%d", &x);
	for (int i = x - 1; i < 9; ++i) {
		m[i] = m[i + 1];
	}
	max--;
	//Prints(max, m);

//i?
	max++;
	printf("Enter 2 numbers (x, y) ");
	scanf("%d%d", &x, &y);
	for (int i = x - 1; i < 9 - 1; ++i) {
		m[i + 1] = m[i];
	}
	m[x - 1] = y;
	//Prints(max, m);

//j
	Prints(max, m);
	return 0;
}

void Prints(int max, int m[]) {
	int i = 0;
	while (i != max) {
		printf("%d ", m[i]);
		i++;
	}
	printf("\n");
}

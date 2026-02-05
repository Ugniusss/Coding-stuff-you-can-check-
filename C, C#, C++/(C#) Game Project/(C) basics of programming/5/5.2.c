//Visual Studio
/*
Parašykite programą, kurios viduje apibrėžiamas masyvas, galintis sutalpinti 1000 elementų. Ši programa turi paprašyti
vartotojo įvesti tris skaičius (a, b, c), į masyvą įrašyti c atsitiktinai sugeneruotų reikšmių, kurių kiekviena priklauso
intervalui [a; b], ir atspausdinti masyvo turinį (tas c reikšmių) į ekraną
*/

#include <stdio.h>
#include <time.h>
#include <stdlib.h>

int main() {
	int a, b, c;
	int m[1000];
	int i = 0;
	printf("Enter 3 numbers (a, b, c) ");
	scanf("%d%d%d", &a, &b, &c);
	srand(time(NULL));
	while (i != c) {
		m[i] = (rand() % b + 1) + a - 1;
		printf("%d ", m[i]);
		i++;
	}
}
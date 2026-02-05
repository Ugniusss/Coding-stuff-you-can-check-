//Visual Studio
/*
Parašykite programą, kuri paprašo vartotojo įvesti du skaičius (s ir n), o po to nuskaito lygiai n teigiamų būsimo masyvo x
elementų reikšmių. Jei reikšmė neteigiama – prašykite kartoti to reikšmės įvedimą. Programa turi atspausdinti visas
masyve esančių skaičių poras (xi, xj) tokias kad xi ir xj sandauga yra lygi s.
*/

#include <stdio.h>

int main() {
	int s, n;
	int x;
	int m[100];
	printf("Enter 2 numbers positive (s, n) ");
	scanf("%d%d", &s, &n);
	for (int i = 0; i < n; ++i) {
		AGAIN:
		printf("Enter positive number %d: ", i + 1);
		scanf("%d", &x);
		if (x > 0) {
			m[i] = x;
		}
		else {
			printf("Bad number \n");
			goto AGAIN;
		}
	}
	for (int i = 0; i < n; ++i) {
		for (int j = i + 1; j < n; ++j) {
			if (m[i] * m[j] == s) {
				printf("%d and %d\n", m[i], m[j]);
			}
		}
	}
}
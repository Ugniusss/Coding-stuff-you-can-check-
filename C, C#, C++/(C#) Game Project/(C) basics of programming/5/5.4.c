//Visual Studio
/*
Parašykite programą, kuri leidžia vesti ir įsimena vartotojo vedamus teigiamus skaičius iki tol, kol jis įveda pirmą
neteigiamą reikšmę. Programa turi atspausdinti visus įvestus pirminius skaičius, kiekvieną pirminį skaičių spausdindama
tik vieną kartą, t. y. išvedant rezultatus sykį jau atspausdinta pirminio skaičiaus reikšmė nebekartojama.
*/

#include <stdio.h>

int main() {
	int x, i = 0;
	int m[1000];
	int l = 0;
	printf("Enter positive numbers (negative number will end the cycle): ");
	while (1) {
		scanf("%d", &x);
		if (x < 0) {
			break;
		}
		for (int j = 0; j < i; ++j) {
			if (m[j] == x) {
				l++;
			}
		}
		if (l == 0) {
			m[i] = x;
			i++;
		}
		l = 0;
	}
	
	for (int j = 0; j < i; ++j) {
		for (int z = 2; z < m[j]; ++z) {
			if (m[j] % z == 0) {
				goto SKIP;
			}
		}	
	printf("%d ", m[j]);
	SKIP:;
	}
}


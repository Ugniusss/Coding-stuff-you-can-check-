//Visual Studio
#include <stdio.h>
#include <math.h>
#include <stdlib.h>

int main() {
	int b, sk, skA = 0, kiek = 0, max = 0;
	printf("Enter sequence of numbers, at the end write a negative number\n");
	while (1) {
		scanf("%d", &sk);
		if (sk < 0) {
			break;
		}
		b = sk;
		while (sk != 0) {
			sk = sk / 10;
			kiek++;
		}
		if (kiek > max) {
			max = kiek;
			skA = b;
		}
		kiek = 0;
	}
	printf("Most digits in: %d", skA);
}
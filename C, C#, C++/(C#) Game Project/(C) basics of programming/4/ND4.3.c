//Visual Studio
#include <stdio.h>
#include <math.h>

int main() {
	int a, b, c;
	int f[100];
	printf("Enter 3 positive numbers: ");
	scanf("%d%d%d", &a, &b, &c);
	if (a < 0 || b < 0 || c < 0) {
		printf("Badly entered numbers.");
		return 0;
	}
	for (int i = 2; i < c+1; ++i) {
		f[0] = a;
		f[1] = b;
		f[i] = f[i - 2] + f[i - 1];
	}
	printf("%d", f[c]);
	return 0;
}



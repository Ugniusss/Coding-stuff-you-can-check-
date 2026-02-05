//Visual Studio
#include <stdio.h>
#include <math.h>

int main() {
	int a, b, c;
	int d = 0, sk1 = 0, sk2 = 0;
	printf("Enter 3 numbers: ");
	scanf("%d%d%d", &a, &b, &c);
	d = b * b - 4 * a * c;
	if (d >= 0) {
		sk1 = ((-b) + sqrt(d)) / 2 * a;
		sk2 = ((-b) - sqrt(d)) / 2 * a;
		printf("Soliutions to ax^2+bx+c=0 function, are: %d %d", sk1, sk2);
	}
	else {
		printf("Function doesn't have a soluton");
	}
	return 0;
}
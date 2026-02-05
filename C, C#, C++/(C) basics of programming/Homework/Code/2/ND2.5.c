// 5.
#include <stdio.h>
#include <math.h>

int main() {
	double x, y, z, z1, sum = 0;
	printf("Enter 3 numbers (x, y, z): ");
	scanf_s("%lf%lf%lf", &x, &y, &z);
// a)
	sum = x + 4 * y + pow(z, 3);
	printf("x + 4y + z^3 = ");
	printf("%.2f", sum);
	sum = 0;
//b
	(z >= 0) ? (z1 = z) : (z1 = z * (-1));
	sum = ((x + sqrt(y)) * (pow(z, 4) - z1 + 46.3));
	printf("\n(x + sqrt(y)(z^4-|z|+46.3) = ");
	printf("%.4f", sum);
	return 0;
}

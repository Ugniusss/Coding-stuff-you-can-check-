//Visual Studio
#include <stdio.h>
#include <math.h>
#include <stdlib.h>

int main() {
	int a, b;
	int sum = 0, min = 100000, max = -10000;
	double averag;
	printf("Enter how many numbers u want to enter: ");
	scanf("%d", &a);
	for (int i = 0; i < a; ++i) {
		printf("Enter %d number: ", i + 1);
		scanf("%d", &b);
		sum += b;
		min = (b < min) ? b : min;
		max = (b > max) ? b : max;
	}
	averag = (double) sum / a;
	printf("Sum: %d \nAverage: %.2f \nMin: %d \nMax: %d \n", sum, averag, min, max);
	return 0;
}
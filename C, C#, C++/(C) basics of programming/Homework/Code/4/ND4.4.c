//Visual Studio
#include <stdio.h>
#include <math.h>

int main() {
	int a, b, c;
	int s1, s2, s3, big;
	int max = -1, p = 1;
 
	printf("Enter 3 natural numbers: ");
	scanf("%d%d%d", &a, &b, &c);
	big = (a > b) ? ((a > c) ? a : c) : ((b > c) ? b : c);
	for (int i = 1; i < 100; ++i) {
		s1 = a % i;
		s2 = b % i;
		s3 = c % i;
		//printf("%d - %d - %d --- %d\n", s1,s2,s3, i);
		if (s1 == 0 && s2 == 0 && s3 == 0) {
			max = i;
		}
	}
	printf("greatest common divisor: %d\n", max);
	while (p) {
		if ((big % a == 0) && (big % b == 0) && (big % c == 0)) {
			printf("Least common multiple: %d", big);
			p = 0;
		}
		++big;
	}
	return 0;
}
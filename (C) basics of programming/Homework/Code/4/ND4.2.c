//Visual Studio
#include <stdio.h>
#include <math.h>

int main() {
	int a, b, c;
	printf("Enter an interval (a,b] and c: ");
	scanf("%d%d%d", &a, &b, &c);
	for (int i = a; i <= b; ++i) {
		if (i >= 0 && i % c == 1 && i > c) {
			printf("%d ", i);
		}
	}
	return 0;
}
// 3
#include <stdio.h>

int main() {
	int sk;
	printf("Enter a number: ");
	scanf_s("%d", &sk);
	printf("%s", (sk % 2 == 0) ? ("number is even") : ("number is odd"));
	return 0;
}
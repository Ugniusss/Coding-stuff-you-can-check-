//Visual Studio
#include <stdio.h>

int main() {
	FILE* f;
	f = fopen("out.txt", "w");
	int sk;
	int skSum = 1;
	printf("Enter a positive number: ");
	scanf("%d", &sk);
	for (int i = 1; i <= sk; ++i) {
		skSum *= i;
	}
	printf("%d", skSum);
	fprintf(f, "%d", skSum);
	fclose(f);
	return 0;
}
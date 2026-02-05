//Visual Studio
#include <stdio.h>

int main() {
	int sk1, sk2, sk3;
	char a;
	float av;
	char fn[20];
	char find[] = ".txt";
	FILE* fp;
	printf("Enter 3 numbers between them ';' : ");
	scanf("%d%c%d%c%d", &sk1, &a, &sk2, &a, &sk3);

	av = (float)(sk1 + sk2 + sk3) / 3;
	AGAIN:
	printf("Enter file name (ending '.txt') : ");
	scanf("%s", &fn);
	if (strstr(fn, find) == NULL) {
		printf("Bad name!\n");
		goto AGAIN;
	}
	fp = fopen(fn, "w");
	fprintf(fp, "%.2f", av);
	fclose(fp);
	printf("Success");
	return 0;
}
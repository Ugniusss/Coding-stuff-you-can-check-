//Visual Studio
#include <stdio.h>

int main() {
	FILE* fp;
	fp = fopen("in.txt", "w");
	AGAIN:
	printf("Enter a number between 10 and 1000, no more than 3 digits after '.' : ");
	double num, sk;
	int len, k = 0;
	char dot = '.';
	char numb[8];
	int pos;
	scanf("%lf", &num);
	if (num < 9 || num > 1000) {
		printf("Badly entered numbers! \n");
		goto AGAIN;
	}
	sk = num;
	while ((int)(sk) % 10 != 0) {
		sk *= 10;
		
	}
	sprintf(numb, "%.3lf", num);
	
	len = snprintf(NULL, 0, "%g", num);
	len--;
	printf("%s length: %d", numb, len);
	fprintf(fp, "%s length: %d",numb, len);
	fclose(fp);
	return 0;
}

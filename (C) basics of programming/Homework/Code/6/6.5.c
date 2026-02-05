//Visual studio
#include <stdio.h>

int main() {
	char email[50];
	char emailGood[50] = { " " };
	char s1 = '@';
	char s2 = '.';
	int posE = 0;
	int posD = 0;
	int len;
	int a = 0;
	FILE* fp;
	fp = fopen("emails.txt", "a");
	AGAIN:
	printf("Enter an email: ");
	scanf("%s", &email);
	len = strlen(email);
	for (int i = 0; i < len; ++i) {
		if (email[i] == s1) {
			posE = i;
		}
	}
	for (int i = posE; i < len; ++i) {
		if (email[i] == s2) {
			posD = i;
		}
	}
	if (posE == 0 || posD == 0 || posD - 2 < posE) {
		printf("Bad email!\n");
		goto AGAIN;
	}
	for (int i = posE+1; i < len; ++i) {
		emailGood[a] = email[i];
		a++;
	}
	
	fprintf(fp,"%s\n", email);
	printf("%s\n", emailGood);
	fclose(fp);
	return 0;
}

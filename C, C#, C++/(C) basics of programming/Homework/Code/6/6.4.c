//Visual Studio
#include <stdio.h>

int main() {
	int i = 0;
	int year, month, day;
	char week[7][10];
	int months[12] = { 31,28,31,30,31,30,31,31,30,31,30,31 };
	int a = 0;
	FILE* fp;
	fp = fopen("week.txt", "r");
	AGAIN:
	printf("Enter a date:");
	scanf("%d-%d-%d", &year, &month, &day);
	if (month < 0 || month > 13 || day < 0 || day > 31 || year > 2022 || year < 1700) {
		printf("Incorect date!\n");
		goto AGAIN;
	}
	while (!feof(fp)) {
		fscanf(fp, "%s", week[i]);
		i++;
	}
	if ((year % 400 == 0) || ((year % 4 == 0) && (year % 100 != 0))) {
		months[1] = 29;
	}
	for (i = 0; i < month - 1; i++) {
		a = a + months[i];
	}
	a = a + (day + year + (year / 4) - 2);
	a = a % 7;
	printf("\nThe day is: %s", week[a]);
	fclose(fp);
	return 0;
}
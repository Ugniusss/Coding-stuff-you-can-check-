// Visual Studio
// Ugnius Padolskis
// 2023-01-07
// Sorts an array
#include "Header.h"

void Sort(int k, Info* car) {
	int nr;
	Info temp;
	printf("What do you want sort?(1:Company name, 2:Model name, 3:Year of manufacture, 4:Car price)\n");
	while (scanf("%d", &nr) != 1)
	{
		printf("Please enter a correct number: ");
		while (getchar() != '\n');
	}
	switch (nr)
	{
	case 1:
		for (int i = 0; i < k - 1; ++i) {
			for (int j = i + 1; j < k; ++j) {
				int comp = strcmp(car[i].FName, car[j].FName);
				if (comp > 0) {
					temp = car[i];
					car[i] = car[j];
					car[j] = temp;
				}
			}
		}
		break;

	case 2:
		for (int i = 0; i < k - 1; ++i) {
			for (int j = i + 1; j < k; ++j) {
				int comp = strcmp(car[i].model, car[j].model);
				if (comp > 0) {
					temp = car[i];
					car[i] = car[j];
					car[j] = temp;
				}
			}
		}
		break;

	case 3:
		for (int i = 0; i < k; ++i) {
			for (int j = i; j < k; ++j) {
				if (car[i].year > car[j].year) {
					temp = car[i];
					car[i] = car[j];
					car[j] = temp;
				}
			}
		}
		break;

	case 4:
		for (int i = 0; i < k; ++i) {
			for (int j = i; j < k; ++j) {
				if (car[i].price > car[j].price) {
					temp = car[i];
					car[i] = car[j];
					car[j] = temp;
				}
			}
		}
		break;
	default:
		break;
	}
}
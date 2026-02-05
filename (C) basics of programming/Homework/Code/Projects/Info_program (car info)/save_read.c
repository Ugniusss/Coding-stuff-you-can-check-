// Visual Studio
// Ugnius Padolskis
// 2023-01-07
// Saves and reads info from/to file

#include "Header.h"

// Reads info from file
int Read(char fn[], Info* car) {
	FILE* file;
	int i = 0;
	fopen_s(&file, "Info.bin", "rb");
	while (!feof(file)) {
		fscanf_s(file, "%[^;]; %[^;]; %d; %lf", car[i].FName, 10, car[i].model, 10, &car[i].year, &car[i].price);
		i++;
	}
	return i;
}

// Saves info to binary (.bin) file
int Save(int k, Info* car) {
	FILE* file;
	fopen_s(&file, "Info.bin", "wb");
	if (file == NULL) {
		return 1;
	}
	for (int i = 0; i < k; ++i) {
		fprintf_s(file, "%s; %s; %d; %.2lf", car[i].FName, car[i].model, car[i].year, car[i].price);
	}
	fseek(file, 0, SEEK_SET);
	fclose(file);
}
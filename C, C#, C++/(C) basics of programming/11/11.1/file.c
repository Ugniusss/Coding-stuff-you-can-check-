//Visual Studio
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

// SAves info to binary (.bin) file
void saveToFile(int arr[], int k) {
	FILE* file;
	fopen_s(&file, "Info.bin", "wb");
	fprintf_s(file, "%d ", k);
	for (int i = 0; i < k; ++i) {
		fprintf_s(file, "%d ", arr[i]);
	}
	fseek(file, 0, SEEK_SET);
	fclose(file);
}

// Reads info from file
void loadFromFile(int arr[], int k) {
	FILE* file;
	fopen_s(&file, "Info.bin", "rb");
	fscanf_s(file, "%d ", &k);
	for (int i = 0; i < k; ++i) {
		fscanf_s(file, "%d ", &arr[i]);
		
	}
}
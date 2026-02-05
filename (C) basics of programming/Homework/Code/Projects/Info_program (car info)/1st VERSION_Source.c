//Visual Studio
//Namu Darbas: Variantas A: (2. Automobilis (firmos pavadinimas, modelio pavadinimas, pagaminimo metai, automobilio kaina).)

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#define MAX_SIZE 10
#define MAX_SIZE2 20

typedef struct {
	char FName[MAX_SIZE];		//Firmos pavadinimas
	char model[MAX_SIZE];		//Modelio pavadinimas
	int year;					//Pagaminimo metai
	double price;				//Kaina
} Info;

Info car[MAX_SIZE2];

//Prototype-------------
int Choise();			// Create's or opens file
void Switch(int k);			// Add, edit car details. Main function
int EntersInfo(int k, Info *car);
int Preview(int k, Info* car);
int Validation(char FNamee[], char modell[], int yearr, double pricee);
int Delete(int k, Info* car);
void Edit(int k, Info* car);
int Save(int k, Info* car);
int Read(char fn[], Info* car);
//Prototype-------------

/*
Honda Civic 2003 999.99
Peugeot 406 2000 500.00
Audi A6 2008 5000.00
BMW M3 2007 3500.95
*/

int main() {
	int k;
	k = Choise();
	Switch(k);
	return 0;
}

// Create's or opens file
int Choise() {
	int q;
	char fn[MAX_SIZE]; // FILE NAME
	FILE* fptr = NULL;
	int k = 0;
FileOption:
	printf("If you'd like to create new car details file, write : 0\nIf you'd like to open a file and edit it, write : 1\n");
	scanf("%d", &q);
	switch (q)
	{
		//New
	case 0:
		return 0;
		//Not_New
	case 1:
		printf("Enter file name: ");
		scanf("%s", &fn);
		fptr = fopen(fn, "rb");
		if (fptr == 0) {
			printf("Error!\n");
			goto FileOption;
		}
		else {
			k = Read(fn ,car);
		}
		return k;
		break;
	default:
		break;
	}
}

// Add, edit car details. Main function
void Switch(int k) {
	int choise = NULL;
	Menu:
	printf("\nEnter what you'd like to do:\n\t1 : Enter new car info\n\t2 : Edit existing car info\n\t3 : Delete existing car info\n\t4 : Preview info\n\t5 : Save info to file\n\t6 : Exit\n");
	scanf("%d", &choise);
	switch (choise) {
		case 1:
// Call function that creates new info	
			EntersInfo(k, car);
			k++;
			goto Menu;
		   break;
		case 2:
// Call function that EDITS	
			Edit(k, car);
			goto Menu;
		   break;
		case 3:
// Call function that DELETES
			k = Delete(k, car);
			goto Menu;
		   break;
		case 4:
// Call function that PRINTS info
			Preview(k, car);
			goto Menu;
		   break;
// Saves entered data into the file
		case 5:
			Save(k, car);
			printf("Saved successfully! (file \"Info.bin\")\n");
			goto Menu;
			break;
// Ends program
		case 6:
			exit(1);
			break;
	default:
		   break;
	
	}
}

// Edits selected car info
void Edit(int k, Info* car) {
	int nr = 0;
	int nr2 = 0;
	Preview(k, car);
	printf("\nSelect car info u want to be edited: ");
	scanf("%d", &nr);
	nr--;
	if (nr < 0 || nr > k) {
		printf("\nWrong Number!\n");
		goto EditEnd;
	}
	printf("\nSelect what u want to edit (1:Company name, 2:Model name, 3:Year of manufacture, 4:Car price)\n");
	scanf("%d", &nr2);
	if (nr2 < 0 || nr2 > 4) {
		printf("\nWrong selection!\n");
		goto EditEnd;
	}
DDarKart:
	switch (nr2) {
	case 1:
		scanf("%s", &car[nr].FName);
		break;
	case 2:
		scanf("%s", &car[nr].model);
		break;
	case 3:
		scanf("%d", &car[nr].year);
		break;
	case 4:
		scanf("%lf", &car[nr].price);
		break;
	default:
		break;
	}
	if (Validation(car[nr].FName, car[nr].model, car[nr].year, car[nr].price) == 1) {
		goto DDarKart;
	}
EditEnd:;
}

// Reads info from file
int Read(char fn[], Info* car) {
	FILE* file;
	int i = 0;
	fopen_s(&file, fn, "rb");
	while (!feof(file)) {
		fscanf_s(file, "%[^;]; %[^;]; %d; %lf", car[i].FName, 10, car[i].model, 10, &car[i].year, &car[i].price);
		i++;
	}
	i--;
	return i;
}
// SAves info to binary (.bin) file
int Save(int k, Info* car) {
	FILE* file;
	fopen_s(&file, "Info.bin", "wb");
	if (file == NULL) {
		return 1;
	}
	for (int i = 0; i < k; ++i) {
		fprintf_s(file, "%s; %s; %d; %.2lf\n", car[i].FName, car[i].model, car[i].year, car[i].price);
	}
	fseek(file, 0, SEEK_SET);
	fclose(file);
}

// Deletes selected car info
int Delete(int k, Info* car) {
	int nr = 0;
	Preview(k, car);
	printf("\nSelect car info u want to be deleted: ");
	scanf("%d", &nr);
	if (nr < 0 || nr > k) {
		printf("\nWrong Number!\n");
		goto EdittEnd;
	}
	nr--;
	if (nr >= 0 && nr < k) {
		for (int i = nr; i < k-1; ++i) {
			car[i] = car[i + 1];
		}
		k--;
	}
	else {
		printf("\nWrong Number!\n");
		return 0;
	}
	return k;
EdittEnd:;
}

// Prints all car info
int Preview(int k, Info* car) {
	//printf("\n\nk:%d", k);
	printf("\n######Company name # Model name # Year of manufacture # Car price #\n");
	for (int i = 0; i < k; ++i) {
		printf("# %-2d # %-12s# %-11s# %-20d# %-10.2lf#\n", i+1, car[i].FName, car[i].model, car[i].year, car[i].price);
	}
	return 0;
}

// Enters all car info
int EntersInfo(int k, Info *car) {
	DarKart:
	printf("\nEnter company name, model name, year of manufacture, car price\n");
	scanf("%s %s %d %lf", &car[k].FName, &car[k].model, &car[k].year, &car[k].price);
	if (Validation(car[k].FName, car[k].model, car[k].year, car[k].price) == 1) {
		goto DarKart;
	}
	return 0;
}

//Validation: Company name, model, year, price
int Validation(char FNamee[], char modell[], int yearr, double pricee) {
	int ps = 0;
	if (FNamee[0] < 'A' || FNamee[0] > 'Z') {
		printf("\nError: badly entered information!\n");
		ps = 1;
		goto PS;
	}
	if (strlen(FNamee) > MAX_SIZE) {
		printf("\nError: badly entered information!\n");
		ps = 1;
		goto PS;
	}
	if (FNamee[0] == '\0') {
		printf("\nError: badly entered information!\n");
		ps = 1;
		goto PS;
	}
	if (modell[0] == '\0') {
		printf("\nError: badly entered information!\n");
		ps = 1;
		goto PS;
	}
	if (yearr < 1885 || yearr > 2023) {
		printf("\nError: badly entered information!\n");
		ps = 1;
		goto PS;
	}
	if (pricee < 0) {
		printf("\nError: badly entered information!\n");
		ps = 1;
		goto PS;
	}
	if (ps == 0) {
		printf("\nData entered successfully!\n");
	}
	PS:
	return ps;
}


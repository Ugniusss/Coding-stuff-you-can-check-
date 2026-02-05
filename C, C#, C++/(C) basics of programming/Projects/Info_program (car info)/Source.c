// Visual Studio
// Ugnius Padolskis
// 2023-01-07
// Namu Darbas: Variantas A: (2. Automobilis (firmos pavadinimas, modelio pavadinimas, pagaminimo metai, automobilio kaina).)
// Program for adding, editing, deleting car info.

/*
Honda Civic 2003 999.99
Peugeot 406 2000 500.00
Audi A6 2008 5000.00
BMW M3 2007 3500.95
Honda Civic 2000 456.23
Peugeot 208 2020 5555.00
Audi A8 2008 4000.00
BMW M4 2018 7894.95
*/

#include "Header.h"
#include "save_read.c"
#include "sort.c"

// Filters selected info
void Filter(int k, Info* car) {
	int nr;
	int num = 0;
	char name[20];
	int in[20];
	int p = 0;
	int ar;
	
	for (int i = 0; i < k; ++i) {
		in[i] = 0;
	}
	printf("What do you want filter? (1:Company name, 2:Model name, 3:Year of manufacture, 4:Car price)\n");
	while (scanf("%d", &nr) != 1)
	{
		printf("Please enter a correct number: ");
		while (getchar() != '\n');
	}
	switch (nr)
	{
	case 1:
		printf("write the name of the company you want to see ");
		scanf("%s", &name);
		for (int i = 0; i < k; ++i) {
			char* res = strstr(name, car[i].FName);
			if (res != NULL) {
				in[p] = i;
				p++;
			}
		}
		break;

	case 2:
		printf("write the name of the model you want to see ");
		scanf("%s", &name);
		for (int i = 0; i < k; ++i) {
			char* res = strstr(name, car[i].model);
			if (res != NULL) {
				in[p] = i;
				p++;
			}
		}
		break;

	case 3:
		printf("write the year you want to see ");
		scanf("%d", &num);
		for (int i = 0; i < k; ++i) {
			if (num == car[i].year) {
				in[p] = i;
				p++;
			}
		}
		break;

	case 4:
		printf("write the price range you want to see ");
		scanf("%d %d", &ar, &num);
		for (int i = 0; i < k; ++i) {
			if (ar < car[i].price && num > car[i].price) {
				in[p] = i;
				p++;
			}
		}
		break;

	default:
		break;
	}
	if (p != 0) {
		printf("\n######Company name # Model name # Year of manufacture # Car price #\n");
		for (int i = 0; i < p; ++i) {
			printf("# %-2d # %-12s# %-11s# %-20d# %-10.2lf#\n", i + 1, car[in[i]].FName, car[in[i]].model, car[in[i]].year, car[in[i]].price);
		}
	}
	else {
		printf("No filters found\n");
	}
}

int main() {
	car = malloc(MAX_SIZE * sizeof(Info));
	int k;			// Number of cars
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
		fptr = fopen("Info.bin", "rb");
		if (fptr == 0) {
			printf("Error!\n");
			goto FileOption;
		}
		else {
			k = Read(fn, car);
		}
		return k;
		break;
	default:
		break;
	}
}

// Add, edit car details. Main function
void Switch(int k) {
	time_t start_time = time(NULL);
	int choise = NULL;
Menu:
	printf("\nEnter what you'd like to do:\n\t1 : Enter new car info\n\t2 : Edit existing car info\n\t3 : Delete existing car info\n\t4 : Preview info\n\t5 : Save info to file\n\t6 : Exit\n\t7 : Sort\n\t8 : Filter\n");
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
		
	case 5:
		// Saves entered data into the file
		Save(k, car);
		printf("Saved successfully! (file \"Info.bin\")\n");
		goto Menu;
		break;
		
	case 6:
		// Ends program
		free(car);
		time_t end_time = time(NULL);
		double elapsed_time = difftime(end_time, start_time);
		FILE* f;
		f = fopen("time.log", "w+"); 
		fprintf(f, "Elapsed time: %.f seconds\n", elapsed_time);
		exit(1);
		break;

	case 7:
		// Sorts
		Sort(k, car);
		goto Menu;
		break;
	case 8:
		// Filers info
		Filter(k, car);
		goto Menu;
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
	while (scanf("%d", &nr) != 1)
	{
		printf("Please enter a correct number: ");
		while (getchar() != '\n');
	}
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
Another:
	switch (nr2) {
	case 1:
		printf("Enter new company name: ");
		scanf("%s", &car[nr].FName);
		break;
	case 2:
		printf("Enter new model name: ");
		scanf("%s", &car[nr].model);
		break;
	case 3:
		printf("Enter new year of manufacture: ");
		scanf("%d", &car[nr].year);
		break;
	case 4:
		printf("Enter new price: ");
		scanf("%lf", &car[nr].price);
		break;
	default:
		break;
	}
	if (Validation(car[nr].FName, car[nr].model, car[nr].year, car[nr].price) == 1) {
		goto Another;
	}
EditEnd:;
}


// Deletes selected car info
int Delete(int k, Info* car) {
	int nr = 0;
	Preview(k, car);
	printf("\nSelect car info u want to be deleted: ");
	while (scanf("%d", &nr) != 1)
	{
		printf("Please enter a correct number: ");
		while (getchar() != '\n');
	}
	if (nr < 0 || nr > k) {
		printf("\nWrong Number!\n");
		goto EdittEnd;
	}
	nr--;
	if (nr >= 0 && nr < k) {
		for (int i = nr; i < k - 1; ++i) {
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
	printf("\n######Company name # Model name # Year of manufacture # Car price #\n");
	for (int i = 0; i < k; ++i) {
		printf("# %-2d # %-12s# %-11s# %-20d# %-10.2lf#\n", i + 1, car[i].FName, car[i].model, car[i].year, car[i].price);
	}
	return 0;
}

// Enters all car info
int EntersInfo(int k, Info* car) {
AndAnother:
	printf("\nEnter company name, model name, year of manufacture, car price\n");
	scanf("%s %s %d %lf", &car[k].FName, &car[k].model, &car[k].year, &car[k].price);
	if (Validation(car[k].FName, car[k].model, car[k].year, car[k].price) == 1) {
		goto AndAnother;
	}
	return 0;
}

// Validation: Company name, model, year, price
int Validation(char FNamee[], char modell[], int year, double price) {
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
	if (year < 1885 || year > 2023) {
		printf("\nError: badly entered information!\n");
		ps = 1;
		goto PS;
	}
	if (price < 0) {
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


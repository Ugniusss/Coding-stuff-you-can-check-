// Visual Studio
// Ugnius Padolskis
// 2023-01-07
// Prototypes, structs

#pragma once
#ifndef HEADER_H
#define HEADER_H

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#define MAX_SIZE 10
#define MAX_SIZE2 20

typedef struct {
	char FName[MAX_SIZE];				// Company name
	char model[MAX_SIZE2];				// Model name
	int year;							// Year of manufacture
	double price;						// Price
} Info;

Info* car;

int Choise();				// Create's or opens file
void Switch(int k);			// Add, edit car details. Main function
int EntersInfo(int k, Info* car);
int Preview(int k, Info* car);
int Validation(char FNamee[], char modell[], int yearr, double pricee);
int Delete(int k, Info* car);
void Edit(int k, Info* car);
inline void Sort(int k, Info* car);
inline int Save(int k, Info* car);
inline int Read(char fn[], Info* car);


#endif
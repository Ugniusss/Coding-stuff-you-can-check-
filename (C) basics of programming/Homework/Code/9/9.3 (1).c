//Visual Studio
/*
Užduotis 2.
Sukurkite sveikųjų skaičių steką, apibrėždami reikiamus duomenų tipus ir su jais dirbančias funkcijas:
a) apibrėžkite struktūrinį duomenų tipą Stack, kurio viduje būtų saugomas dinaminis masyvas (rodyklė į pirmą dinaminio
masyvo elementą) ir jo talpa (dydis). Duomenų tipo vardui sutrumpinti pasinaudokite žodžiu typedef.
b) apibrėžkite funkciją void initStack(Stack *stack), kuri nustatytų pradines struktūros reikšmes (lygias 0)
c) apibrėžkite funkciją void printStack(Stack *stack), kuri cikle atspausdintų visus dinaminio masyvo elementus
d) apibrėžkite funkciją int getStackSize(Stack *stack), kuri tiesiog grąžina Stack viduje talpinamo dinaminio masyvo talpą
(dydį)
d) apibrėžkite funkciją void push(Stack *stack, int value), kuri (padidinusi dinaminio masyvo talpą) įterptų naują reikšmę
į pabaigą
e) apibrėžkite funkciją int top(Stack *stack), kuri grąžintu paskutinį dinaminio masyvo elementą (arba 0, jei masyvas
tuščias).
f) apibrėžkite funkciją int pop(Stack *stack), kuri ne tik grąžina paskutinį dinaminio masyvo elementą (daro tą patį, ką ir
funkciją top, ir todėl į ją kreipiasi), bet ir ištrina jį iš masyvo (atitinkamai sumažina ir dinaminio masyvo talpą)
g) apibrėžkite funkciją void destroyStack(Stack *stack), kuri atlaisvina visą naudojamą atmintį (atitinkamai, atnaujina ir
Stack viduje esančius laukus).
*/
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <time.h>
#include <math.h>
// A.
typedef struct {
	int size;
	double *y;
	double *x;
}Stack;

//Prototype---
void initStack(Stack* point);
void printStack(Stack* point);
void push(Stack* point, double x1, double y1);
double top(Stack* point);
double pop(Stack* point);
double getDistance(double sk1, double sk2, double sk3, double sk4, int l);
//Prototype---

int main() {
	double y1;
	double x1;
	Stack point;
	initStack(&point);
		//printStack(&point);
	point.x = (Stack*)malloc(point.size * sizeof(Stack));
	point.y = (Stack*)malloc(point.size * sizeof(Stack));
	x1 = 5.0;
	y1 = 5.0;
	push(&point, x1, y1);
	x1 = -5.0;
	y1 = -5.0;
	push(&point, x1, y1);
	x1 = -5.0;
	y1 = 5.0;
	push(&point, x1, y1);
	x1 = 5.0;
	y1 = -5.0;
	push(&point, x1, y1);
	x1 = 0;
	y1 = 0;
	push(&point, x1, y1);
	printStack(&point);
		//top(&point);
		//pop(&point);
	//printStack(&point);
	return 0;
}


double pop(Stack* point) {
	int a;
	int b;
	a = *(point->x + point->size);
	b = *(point->y + point->size);
	point->size--;
	printf("%d:%d", a, b);

}


double top(Stack* point) {
	double  a;
	double b;
	if (point->size == 0) {
		return 0;
	}
	else {
		a = *(point->x + point->size);
		b = *(point->y + point->size);
		printf("(%.2f:%.2f)\n", a, b);
	}
}

void push(Stack* point, double x1, double y1) {
	point->size++;
	*(point->x + point->size) = x1;
	*(point->y + point->size) = y1;
}


void initStack(Stack* point) {
	point->x = 0;
	point->y = 0;
	point->size = 0;
}

void printStack(Stack* point) {
	double p;
	for (int i = 0; i < point->size; ++i) {
		for (int j = i+1; j < point->size; ++j) {
			p = getDistance(point->x[i], point->y[i], point->x[j], point->y[j], j);
			printf("(%.2f:%.2f) - (%.2f:%.2f) = %.2f\n", point->x[i], point->y[i], point->x[j], point->y[j], p);
		}
	}
}

double getDistance(double sk1, double sk2,double sk3,double sk4, int l) {
	double d;
	d = sqrt(pow((sk3 - sk1), 2) +
		pow((sk4 - sk2), 2));

	return d;
}
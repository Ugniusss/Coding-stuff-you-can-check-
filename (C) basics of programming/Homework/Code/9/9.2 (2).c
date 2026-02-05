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
// A.
typedef struct {
	int size;
	int* arr;
}Stack;

//Prototype---
void initStack(Stack* stack);
void printStack(Stack* stack);
int getStackSize(Stack* stack);
void push(Stack* stack, int value);
int top(Stack* stack);
int pop(Stack* stack);
void destroyStack(Stack* stack);
//Prototype---

int main() {
	int value; 
	Stack stack;
	initStack(&stack);
	stack.arr = (Stack*)malloc(stack.size * sizeof(Stack));
	value = 500;
	push(&stack, value);
		//printStack(&stack);
		//printf(" last %d\n", top(&stack));
	top(&stack);
		//printf(" pop %d\n", pop(&stack));
	pop(&stack);
		//printStack(&stack);
		//printf(" stack size %d\n", getStackSize(&stack));
	getStackSize(&stack);
		//destroyStack(&stack);
	printStack(&stack);
	return 0;
}
// G. 
void destroyStack(Stack* stack) {
	for (int i = 1; i <= stack->size; ++i) {
		free(stack->arr[i]);
		stack->arr = NULL;
	}
	free(stack);
	stack = NULL;
}

// F.
int pop(Stack* stack) {
	int a;
	a = *(stack->arr + stack->size);
	stack->size--;
	return a;
	
}

// E.
int top(Stack* stack) {
	int a;
	if (stack->size < 0) {
		return 0;
	}
	else {
		a = *(stack->arr + stack->size);
		return a;
	}
}

// D.
void push(Stack* stack, int value) {
	stack->size++;
	*(stack->arr + stack->size) = value;
}

// D.
int getStackSize(Stack* stack) {
	int a;
	a = stack->size * sizeof(int);
	return a;
}

// B.
void initStack(Stack* stack) {
	stack->arr = 0;
	stack->size = 0;
}
// C.
void printStack(Stack* stack) {
	for (int i = 1; i <= stack->size; ++i) {
		printf("%d \n", stack->arr[i]);
	}
}
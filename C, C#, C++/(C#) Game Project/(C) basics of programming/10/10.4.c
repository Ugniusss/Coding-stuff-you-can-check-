//Visual Studio
/*

*/
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <time.h>
#include <assert.h>
//Prototype---
char *strchrr(char a[], char b);
//Prototype---

int main() {

	assert(strcmp(strchrr("12345", '3'), "345") == 0);
	assert(strcmp(strchrr("3", '3'), "3") == 0);
	assert(strcmp(strchrr("34", '3'), "3") != 0);
	return 0;
}

char *strchrr(char a[], char b) {
	char *c;
	c = NULL;
	for (int i = 0; a[i] != '\0'; i++) {
		if (a[i] == b) {
			c = &(a[i]);
			break;
		}
	}
	return c;
}
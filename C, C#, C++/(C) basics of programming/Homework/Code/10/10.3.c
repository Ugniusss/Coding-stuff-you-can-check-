//Visual Studio
/*

*/
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <time.h>
#include <assert.h>
//Prototype---
int strncmpp(char a[], char b[], int k);
//Prototype---

int main() {

	assert(strncmpp("12345", "12345", 5) == 0);
	assert(strncmpp("", "", 0) == 0);
	assert(strncmpp("1234", "12345", 5) < 0);
	assert(strncmpp("12345", "1234", 5) > 0);
	assert(strncmpp("1", "1234", 4) < 0);
	assert(strncmpp("123", "1", 3) > 0);
	assert(strncmpp("123", "1", 1) == 0);
	return 0;
}

int strncmpp(char a[], char b[], int k) {
	return strncmp(a, b, k);
}
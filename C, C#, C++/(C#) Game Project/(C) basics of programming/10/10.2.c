//Visual Studio
/*

*/
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <time.h>
#include <assert.h>
//Prototype---
int strcmpp(char a[], char b[]);
//Prototype---

int main() {

	assert(strcmpp("Labas", "rytas") == 0);
	assert(strcmpp("", "") == -1);
	assert(strcmpp("test", "test") == -1);
	assert(strcmpp("test", "Test") == 1);
	assert(strcmpp("t", "Test") == 1);
	assert(strcmpp("Tst", "t") == 0);
	return 0;
}

int strcmpp(char a[], char b[]) {
	
	if (a == b) {
		return -1;
	}
	if (a > b) {
		return 0;
	}
	if (a < b) {
		return 1;
	}
}
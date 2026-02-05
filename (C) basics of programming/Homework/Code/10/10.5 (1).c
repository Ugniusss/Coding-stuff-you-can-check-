//Visual Studio
/*

*/
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <time.h>
#include <assert.h>
//Prototype---
int memcmpp(char a[], char b[], int k);
//Prototype---

int main() {

	assert(memcmpp("123", "123", 3) == 0);
	assert(memcmpp("12", "123", 3) < 0);
	assert(memcmpp("123", "12", 3) > 0);
	assert(memcmpp("", "", 1) == 0);
	
	return 0;
}

int memcmpp(char a[], char b[], int k) {
	return memcmp(a, b, k);
}
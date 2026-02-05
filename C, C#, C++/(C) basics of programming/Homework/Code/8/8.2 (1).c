//Visual Studio
/*
Užduotis 2.
Apibrėžkite funkciją, kuri gauna argc-1 failų vardų per komandinės eilutės parametrus, ir grąžina daugiausiai baitų
atmintyje užimančio failo vardą.

*/
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <time.h>
//Prototype---

//Prototype---

int main(int argc, char **argv) {
	printf("%s", argv[Special(argc, argv)]);
	return 0;
}

int Special(int argc, char** argv) {
	int size = 0;
	int max = -1;
	int nr;
	FILE* fp;
	for (int i = 0; i < argc; ++i) {
		fp = fopen(argv[i], "r");
		if (fp == NULL) {
			printf("%s - Can't open the file/File doesn't exist\n", argv[i]);
		}
		else {
			fseek(fp, 0, SEEK_END);
			size = ftell(fp);
		}
		if (size > max) {
			max = size;
			nr = i;
		}
		size = 0;
	}
	return nr;
}

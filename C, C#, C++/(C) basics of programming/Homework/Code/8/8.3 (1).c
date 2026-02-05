//Visual Studio
/*
Užduotis 3.
Apibrėžkite funkciją swap, kuri moka sukeisti dviejų (tai funkcijai perduodamų) kintamųjų (sveikųjų skaičių) reikšmes
vietomis, taip, kad apkeitimas vyktų funkcijoje, o efektas liktų galioti ne tik funkcijoje, bet ir už jos ribų.
*/
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <time.h>
//Prototype---

//Prototype---

int main() {
	int sk1 = 5;
	int sk2 = 2;
	//printf("Before: %d -> %d\n", sk1, sk2);
	Swap(&sk1, &sk2);
	//printf("After: %d <- %d", sk1, sk2);
	return 0;
}
int Swap(int* sk1, int* sk2) {
	*sk1 = *sk1 + *sk2;
	*sk2 = *sk1 - *sk2;
	*sk1 = *sk1 - *sk2;
	return 0;
}
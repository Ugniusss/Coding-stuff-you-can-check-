
// 2a.
// a) 11011 = 27, 1001 0100 = 148, 0011 0010 1101 0101 = 13013
// b) 37 = 100101, 241 = 1111 0001, 2487 = 1001 1011 0111
// c) 6E2 = 1762, ED33 = 60723, 123456 = 1193046
// d) 243 = F3, 2483 = 9B3, 4612 = 1204


// 2b.
#include <stdio.h>

int main() {
// binary to decimal
// 1)
	int b1 = 11011, k1 = 1, n1 = 0, bd1 = 0;
	int b2 = 10010100, k2 = 1, n2 = 0, bd2 = 0;
	unsigned long long b3 = 11001011010101; 
	int k3 = 1, n3 = 0, bd3 = 0;
	while (b1 > 0){
		n1 = b1 % 10; 
		bd1 = bd1 + n1 * k1;
		b1 = b1 / 10;  
		k1 = k1 * 2;
	}
	printf("%d", bd1);
// 2)
	while (b2 > 0){
		n2 = b2 % 10;
		bd2 = bd2 + n2 * k2;
		b2 = b2 / 10;
		k2 = k2 * 2;
	}
	printf(", %d", bd2);
// 3)
	while (b3 > 0){
		n3 = b3 % 10;
		bd3 = bd3 + n3 * k3;
		b3 = b3 / 10;
		k3 = k3 * 2;
	}
	printf(", %llu\n", bd3);
// decimal to binary
// 1)
	int d1 = 37, a[10], i;
	for (i = 0; d1 > 0; i++){
		a[i] = d1 % 2;
		d1 = d1 / 2;
	}
	for (i = i - 1; i >= 0; i--){
		printf("%d", a[i]);
	}
	printf(", ");
// 2)
	int d2 = 241, b[10];
	for (i = 0; d2 > 0; i++) {
		b[i] = d2 % 2;
		d2 = d2 / 2;
	}
	for (i = i - 1; i >= 0; i--) {
		printf("%d", b[i]);
	}
	printf(", ");
// 3)
	int d3 = 2487, c[15];
	for (i = 0; d3 > 0; i++) {
		c[i] = d3 % 2;
		d3 = d3 / 2;
	}
	for (i = i - 1; i >= 0; i--) {
		printf("%d", c[i]);
	}
// hexadecimal to decimal
	printf("\n%ld, %ld, %ld", 0x6E2, 0xED33, 0x123456);
// decimal to hexadecimal
	printf("\n%X, %X, %X", 243,2483,4612);
	return 0;
}
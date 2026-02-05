// 6
#include <stdio.h>

int function(int sk1, int sk2, int sk3) {
	int min, max;
	max = (sk1 > sk2) ? (sk1 > sk3 ? sk1 : sk3) : (sk2 > sk3 ? sk2 : sk3);
	min = (sk1 < sk2) ? (sk1 < sk3 ? sk1 : sk3) : (sk2 < sk3 ? sk2 : sk3);
	printf("Max: %d Min: %d", max, min);
}

int main() {
	int sk1, sk2, sk3;
	printf("Enter 3 numbers: ");
	scanf_s("%d%d%d", &sk1, &sk2, &sk3);
	function(sk1, sk2, sk3);
	return 0;
}
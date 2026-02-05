//Visual Studio
/*
Užduotis 1.
Sukurkite duomenų tipus ir funkcijas atstumui tarp dviejų taškų plokštumoje suskaičiuoti:
a) apibrėžkite struktūrinį duomenų tipą Point, kurio viduje būtų saugomi du realūs skaičiai – taško plokštumoje
koordinatės x ir y. Duomenų tipo vardui sutrumpinti pasinaudokite raktiniu žodžiu typedef. Funkcijoje main sukurkite du
Point tipo kintamuosius p1 ir p2, atitinkančius tašką su koordinatėmis (2.0, -3.0) ir tašką su koordinatėmis (-4.0, 5.0)
atitinkamai
b) apibrėžkite funkciją void printPoint(Point p), kuri gavusi taško koordinates nusakančią struktūrą Point, atspausdina jo
koordinates į ekraną, formatu (x, y). Pasinaudokite funkcija printPoint taškų p1 ir p2 koordinatėms atspausdinti
c) apibrėžkite funkciją Point createPoint(double x, double y), kuri turint du realius skaičius leistų gauti tašką su
atitinkamomis koordinatėmis (sukurtų Point tipo struktūrą, užpildytų ją koordinatėmis, ir grąžintų tolesniam
panaudojimui). Perrašykite main funkciją taip, kad taškai p1 ir p2 būtų kuriami naudojantis funkcija createPoint
d) apibrėžkite funkciją double getDistance(Point a, Point b), kuri randa (grąžina) atstumą tarp dviejų taškų plokštumoje.
Perrašykite main funkciją taip, kad ji atliktų vieną veiksmą - apskaičiuotų atstumą tarp taškų p1 ir p2. Tai reikia atlikti
vienu C kalbos sakiniu – funkcijai getDistance tiesiogiai perduokite createPoint rezultatą(-us), o kintamieji p1 ir p2 tampa
nebūtini.
*/
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <time.h>
#include <math.h>

typedef struct Point {
	double x;
	double y;
}Pt;
//Prototype---
void printPoint(Pt p);
double distance();
Pt createPoint(double x, double y);
double getDistance(Pt a, Pt b);
//Prototype---

int main() {
	
	printf("(A)Distance between them is: %.2f\n", distance());
	double x = 2.0, y = -3.0;
	Pt third = createPoint(x, y);
		//printf("%.2f:%.2f\n", third.x, third.y);
	 x = -4.0, y = 5.0;
	Pt forth = createPoint(x, y);
		//printf("%.2f:%.2f", forth.x, forth.y);
	printf("(D)Distance between them is: %.2f\n", getDistance(third, forth));
	return 0;
}
// A.
double distance() {
	Pt first;
	Pt second;
	first.x = 2.0;
	first.y = -3.0;
	//printPoint(first);
	second.x = -4.0;
	second.y = 5.0;
	//printPoint(second);
		double d;
	d = sqrt(pow((second.x - first.x), 2) + pow((second.y - first.y), 2));
	return d;

}
// B.
void printPoint(Pt p) {
	printf("(%.1f:%.1f)\n", p.x, p.y);
	return 0;
}
// C.
Pt createPoint(double x, double y) {
	Pt point;
	point.x = x;
	point.y = y;
	return point;
}
// D.
double getDistance(Pt a, Pt b) {
	double d;
	d = sqrt(pow((b.x - a.x), 2) +
		pow((b.y - a.y), 2));
	return d;
}
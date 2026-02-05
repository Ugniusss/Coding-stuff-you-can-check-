// Visual Studio

#include <iostream>
#include <vector>
#include "Header.h"

using namespace std;

void Langas::setAukstis(float a) { aukstis = a; }
void Langas::setPlotis(float a) { plotis = a; }
void Durys::setPlotis(float a) { plotis = a; }
void Durys::setAukstis(float a) { aukstis = a; }
void Kambarys::setAukstis(float a) { aukstis = a; }
void Kambarys::setIlgis(float a) { ilgis = a; }
void Kambarys::setPlotis(float a) { plotis = a; }
void Kambarys::papLangas(Langas l) {
    lan.push_back(l);
}
void Kambarys::papDuris(Durys d) {
    dur.push_back(d);
}

float Kambarys::sienosP() {
    float plotas = ilgis * aukstis * plotis;
    for (int i = 0; i < lan.size(); ++i) {
        plotas = plotas - lan[i].getPlotis() * lan[i].getAukstis();
    }
    for (int i = 0; i < dur.size(); ++i) {
        plotas = plotas - dur[i].getPlotis() * dur[i].getAukstis();
    }
    return plotas;
}

float Kambarys::grindI() {
    float peri = 2 * (ilgis + plotis);
    for (int i = 0; i < dur.size(); ++i) {
        peri = peri - dur[i].getPlotis();
    }
    return peri;
}

int main() {

    Langas lan1;
    lan1.setAukstis(2);
    lan1.setPlotis(3);

    Langas lan2;
    lan2.setAukstis(2);
    lan2.setPlotis(3);

    Durys dur1;
    dur1.setAukstis(5);
    dur1.setPlotis(5);

    Durys dur2;
    dur2.setAukstis(5);
    dur2.setPlotis(25);

    Kambarys kam;
    kam.setAukstis(20);
    kam.setIlgis(20);
    kam.setPlotis(20);

    kam.papLangas(lan1);
    kam.papLangas(lan2);

    kam.papDuris(dur1);
    kam.papDuris(dur2);

    cout << "Kambario plotas: " << kam.sienosP() << endl;
    cout << "Grindu ilgis: " << kam.grindI();
    return 0;
}


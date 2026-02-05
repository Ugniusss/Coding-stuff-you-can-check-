// Visual Studio

#include <iostream>
#include <vector>
#include "Header.h"

using namespace std;

//-----LANGAI------------

Langas::Langas(float a, float b) {
    aukstis = a;
    plotis = b;
    sukurtalangu++;
}

Langas::~Langas() {
    sukurtalangu--;

}

float Langas::getAukstis() { return aukstis; }
float Langas::getPlotis() { return plotis; }
int Langas::getLang() { return sukurtalangu; }

//-----DURYS------------
Durys::Durys(float a, float b) {
    aukstis = a;
    plotis = b;
    sukurtaduru++;
}

Durys::~Durys() {
    sukurtaduru--;

}

float Durys::getAukstis() { return aukstis; }
float Durys::getPlotis() { return plotis; }
int Durys::getDur() { return sukurtaduru; }

//-----KAMBARYS------------
Kambarys::Kambarys(float a, float b,float c) {
    aukstis = a;
    plotis = b;
    ilgis = c;
}
Kambarys::~Kambarys() {
    for (auto l : lan) {
        delete l;
    }
    lan.clear();

    for (auto d : dur) {
        delete d;

    }
    dur.clear();
}

float Kambarys::getAukstis() { return aukstis; }
float Kambarys::getPlotis() { return plotis; }
float Kambarys::getIlgis() { return ilgis; }


void Kambarys::papLangas(Langas *l) {
    lan.push_back(l);
}
void Kambarys::papDuris(Durys *d) {
    dur.push_back(d);
}


float Kambarys::sienosP() {
    float plotas = ilgis * aukstis * plotis;
    for (int i = 0; i < lan.size(); ++i) {
        plotas = plotas - lan[i]->getPlotis() * lan[i]->getAukstis();
        
    }
    for (int i = 0; i < dur.size(); ++i) {
        plotas = plotas - dur[i]->getPlotis() * dur[i]->getAukstis();

    }

    return plotas;
}

float Kambarys::grindI() {
    float peri = 2 * (ilgis + plotis);
    for (int i = 0; i < dur.size(); ++i) {
        peri = peri - dur[i]->getPlotis();
    }
    return peri;
}



//--------------MAIN------------------------
int main() {

    Langas* lan1 = new Langas(2, 3);
 
    Langas* lan2 = new Langas(2, 3);

    Langas* lan3 = new Langas(2, 3);

    Durys* dur1 = new Durys(5, 5);

    Durys* dur2 = new Durys(5, 5);

    Kambarys* kam = new Kambarys(20, 20, 20);

    kam->papLangas(lan1); 
    kam->papLangas(lan2); 
    kam->papLangas(lan3); 
    cout << "Sukurta langu: " << Langas::getLang() << endl;

    kam->papDuris(dur1); 
    kam->papDuris(dur2); 

    cout << "Sukurta duru: " << Durys::getDur() << endl;

    cout << "Kambario plotas: " << kam->sienosP() << endl;
    cout << "Grindu ilgis: " << kam->grindI() << endl;
  
    delete kam;

    cout << "Po sunaikinimo langu liko: " << Langas::getLang() << endl;
    cout << "Po sunaikinimo duru liko: " << Durys::getDur() << endl;
   
    return 0;
}


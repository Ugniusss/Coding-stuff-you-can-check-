// Visual Studio

#include <iostream>
#include <vector>
#include "H6.h"

using namespace std;

//-----LANGAI------------

Langas::Langas(float a, float b) {
    aukstis = a;
    plotis = b;
    sukurtalangu++;
}

Langas::~Langas() {
    if (sukurtalangu > 0) {
        sukurtalangu--;
    }
}

float Langas::getAukstis() { return aukstis; }
float Langas::getPlotis() { return plotis; }
int Langas::getLang() { return sukurtalangu; }

//-----DURYS----------------------------------------------------
Durys::Durys(float a, float b) {
    aukstis = a;
    plotis = b;
    sukurtaduru++;
}

Durys::~Durys() {
    if (sukurtaduru > 0) {
        sukurtaduru--;
    }
}

float Durys::getAukstis() { return aukstis; }
float Durys::getPlotis() { return plotis; }
int Durys::getDur() { return sukurtaduru; }

//-----KAMBARYS------------------------------------------------
Kambarys::Kambarys(float a, float b, float c) {
    aukstis = a;
    plotis = b;
    ilgis = c;
}

Kambarys::Kambarys(const Kambarys& k) {
    aukstis = k.aukstis;
    plotis = k.plotis;
    ilgis = k.ilgis;
    dur = new Durys(k.dur->getAukstis(), k.dur->getPlotis());
    for (auto i : k.lan) {
        Langas* lang = new Langas(*i);
        lan.push_back(lang);
    }
}

Kambarys& Kambarys::operator=(const Kambarys& k) {
    if (this == &k) return *this;
    for (Langas* langas : lan) {
        delete langas;
    }

    delete dur;
    
    lan.clear();

    for (Langas* langas : k.lan) {
        for (auto i : k.lan) {
            Langas* lang = new Langas(*i);
            lan.push_back(lang);
        }
    }

    dur = new Durys(k.dur->getAukstis(), k.dur->getPlotis());

    aukstis = k.aukstis;
    ilgis = k.ilgis;
    plotis = k.plotis;

    return *this;
}

Kambarys::~Kambarys() {
    for (auto l : lan) {
        delete l;
    }
    lan.clear();
    delete dur;
}

float Kambarys::getAukstis() { return aukstis; }
float Kambarys::getPlotis() { return plotis; }
float Kambarys::getIlgis() { return ilgis; }

void Kambarys::papLangas(Langas* l) {
    lan.push_back(l);
}
void Kambarys::papDuris(Durys* d) {
    dur = d;
}

Durys* Kambarys::getDurys() {
    return dur;
}

vector <Langas*> Kambarys::getLangas() {
    return lan;
}

//------------------STUF----------------------------------------------
float sienosP(Kambarys k) {
    float plotas = 0;
    float LangPlotas = 0;
    float DurPlotas = 0;
    plotas = k.getIlgis() * k.getAukstis() * k.getPlotis();
    for (auto lan : k.getLangas()) {
        LangPlotas += lan->getPlotis() * lan->getAukstis();
    }
    DurPlotas += k.getDurys()->getPlotis() * k.getDurys()->getAukstis();
    plotas = plotas - LangPlotas - DurPlotas;
    return plotas;
}

float grindI(Kambarys k) {
    float peri = 0;
    peri = 2 * (k.getIlgis() + k.getPlotis());
    float DurPlot = 0;
    DurPlot += k.getDurys()->getPlotis();
    peri = peri - DurPlot;
    return peri;
}

//--------------MAIN----------------------------------------------------------
int main() {

    Langas* lan1 = new Langas(2, 3);

    Langas* lan2 = new Langas(2, 3);

    Langas* lan3 = new Langas(2, 3);

    Durys* dur1 = new Durys(5, 5);

    Kambarys* k1 = new Kambarys(10, 10, 10);

    //Kambarys* k2 = new Kambarys(10, 10, 10);

    k1->papLangas(lan1);
    k1->papLangas(lan2);
    k1->papLangas(lan3);
    //cout << "Sukurta langu: " << Langas::getLang() << endl;

    k1->papDuris(dur1);
    //cout << "Sukurta duru: " << Durys::getDur() << endl;
    
    cout << "Kambario plotas: " << sienosP(*k1) << endl;
    cout << "Grindu ilgis: " << grindI(*k1) << endl << endl;

    Kambarys* k2 = new  Kambarys(*k1);
    delete k1;
    
    cout << "K2_Kambario plotas: " << sienosP(*k2) << endl;
    cout << "K2_Grindu ilgis: " << grindI(*k2) << endl;

    //cout << "Po sunaikinimo langu liko: " << Langas::getLang() << endl;
    //cout << "Po sunaikinimo duru liko: " << Durys::getDur() << endl;

    return 0;
}


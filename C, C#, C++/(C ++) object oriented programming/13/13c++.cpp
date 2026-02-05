#include <iostream>
#include <vector>
#include <string>
#include <iomanip>
#include <cmath>

using namespace std;

class Preke {
private:
    string pavadinimas;
    double kaina;
    string kategorija;

public:
    Preke(string p, double k, string ka) {
        pavadinimas = p;
        kaina = k;
        kategorija = ka;
    }
    double getKaina() {
        return kaina;
    }
    string getPavadinimas() {
        return pavadinimas;
    }
    string getKategorija() {
        return kategorija;
    }
};

class Krepselis {
private:
    vector <Preke> prekes;

public:
    void PridetiPrek(Preke preke) {
        prekes.push_back(preke);
    }
    double VisuKaina() {
        double suma = 0;
        vector <Preke>::iterator it;
        for (it = prekes.begin(); it != prekes.end(); ++it ) {
            suma += (*it).getKaina();
        }
        return suma;
    }
};

class Nuolaida {
public:
    
    virtual double skNuolaida(Krepselis krep) = 0;
};

class FiksuotaNuolaida : public Nuolaida {
private:
    FiksuotaNuolaida* kitaNuolaida;
    double riba;
    double minus;

public:
    FiksuotaNuolaida(double r, double m) {
        riba = r;
        minus = m;
    }

    double skNuolaida(Krepselis krep) {
        if (krep.VisuKaina() > riba) {
            return krep.VisuKaina() - minus;
        }
        else if(kitaNuolaida != NULL) {
            return kitaNuolaida->skNuolaida(krep);
        }
        else {
            return krep.VisuKaina();
        }
    }
    void setKitaNuolaida(FiksuotaNuolaida* nuolaida) {
        kitaNuolaida = nuolaida;
    }
};


class SantykineNuolaida : public Nuolaida {
private:
    double riba;
    double procentas;
    SantykineNuolaida *kitaNuolaida;

public:
    SantykineNuolaida(double r, double m) {
        riba = r;
        procentas = m;
    }
    double skNuolaida(Krepselis krep) {
        if (riba < krep.VisuKaina()) {
            return (krep.VisuKaina() * (100 - procentas)) / 100;
        }
        else if (kitaNuolaida != NULL) {
            return kitaNuolaida->skNuolaida(krep);
        }
        else {
            return krep.VisuKaina();
        }
        
    }
    void setKitaNuolaida(SantykineNuolaida* nuolaida) {
        kitaNuolaida = nuolaida;
    }
};

class Parduotuve {
private:
    //vector <Preke> katalogas;
    Nuolaida* nuolaida;

public:
    Parduotuve() : nuolaida(nullptr) {}

    template<class nuolaidaObj>
    void setNuolaida(nuolaidaObj* d) {
        nuolaida = d;
    }
    float kainaPoNuolaidos(Krepselis krep) {
        return nuolaida->skNuolaida(krep);
    }
    
};



int main() {
    Parduotuve pard;

    Preke p1("pav1", 80.0, "kat1");
    Preke p2("pav2", 60.0, "kat2");
    Preke p3("pav3", 90.0, "kat3");
    Preke p4("pav4", 50.0, "kat4");
    Preke p5("pav5", 150.0, "kat5");
    Preke p6("pav6", 160.0, "kat6");

    FiksuotaNuolaida* fn1 = new FiksuotaNuolaida(300, 20);
    FiksuotaNuolaida* fn2 = new FiksuotaNuolaida(200,20);
    FiksuotaNuolaida* fn3 = new FiksuotaNuolaida(100,10);
    FiksuotaNuolaida* fn4 = NULL;
    
    fn3->setKitaNuolaida(fn4);
    fn2->setKitaNuolaida(fn3);
    fn1->setKitaNuolaida(fn2);
    pard.setNuolaida(fn1);
    
    cout << "Fiksuota Nuolaida" << endl;
    
    Krepselis krep3;
    krep3.PridetiPrek(p4);
    cout << "suma 50: " << endl;
    cout << pard.kainaPoNuolaidos(krep3) << endl;

    Krepselis krep;
    krep.PridetiPrek(p4);
    krep.PridetiPrek(p5);
    cout << "suma 200: " << endl;
    cout << pard.kainaPoNuolaidos(krep) << endl;

    Krepselis krep1;
    krep1.PridetiPrek(p4);
    krep1.PridetiPrek(p5);
    krep1.PridetiPrek(p5);
    cout << "suma 350: " << endl;
    cout << pard.kainaPoNuolaidos(krep1) << endl;

    SantykineNuolaida* sn1 = new SantykineNuolaida(300, 20);
    SantykineNuolaida* sn2 = new SantykineNuolaida(200, 20);
    SantykineNuolaida* sn3 = new SantykineNuolaida(100, 10);
    SantykineNuolaida* sn4 = NULL;

    sn3->setKitaNuolaida(sn4);
    sn2->setKitaNuolaida(sn3);
    sn1->setKitaNuolaida(sn2);
    pard.setNuolaida(sn1);

    cout << endl;
    cout << "santykine nuolaida" << endl;
    cout << endl;

    cout << "suma 50: " << endl;
    cout << pard.kainaPoNuolaidos(krep3) << endl;

    cout << "suma 200: " << endl;
    cout << pard.kainaPoNuolaidos(krep) << endl;

    cout << "suma 350: " << endl;
    cout << pard.kainaPoNuolaidos(krep1) << endl;
    

    return 0;
}

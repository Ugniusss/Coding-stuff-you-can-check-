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
    const double& getKaina() const {
        return kaina;
    }
    const string& getPavadinimas() const {
        return pavadinimas;
    }
    const string& getKategorija() const {
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
        for (it = prekes.begin(); it != prekes.end(); ++it) {
            suma += (*it).getKaina();
        }
        return suma;
    }
    const vector<Preke>& getPrekes() const {
        return prekes;
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
            return minus;
        }
        else if (kitaNuolaida != NULL) {
            return kitaNuolaida->skNuolaida(krep);
        }
        else {
            return 0;
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
    SantykineNuolaida* kitaNuolaida;

public:
    SantykineNuolaida(double r, double m) {
        riba = r;
        procentas = m;
    }
    double skNuolaida(Krepselis krep) {
        if (riba < krep.VisuKaina()) {
            return (krep.VisuKaina() * procentas) / 100;
        }
        else if (kitaNuolaida != NULL) {
            return kitaNuolaida->skNuolaida(krep);
        }
        else {
            return 0;
        }

    }
    void setKitaNuolaida(SantykineNuolaida* nuolaida) {
        kitaNuolaida = nuolaida;
    }
};

class Parduotuve {
private:
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

class NuolaidaKategorijai : public Nuolaida {
private:
    string kategorijosKodas;
    Nuolaida* kitaNuolaida;

public:
    NuolaidaKategorijai(const string& kodas) : kategorijosKodas(kodas), kitaNuolaida(nullptr) {}

    double skNuolaida(Krepselis krep) {
        Krepselis laikinasKrep;
        const vector<Preke>& prekes = krep.getPrekes();
        for (const Preke& preke : prekes) {
            if (preke.getKategorija() == kategorijosKodas) {
                laikinasKrep.PridetiPrek(preke);
            }
        }

        if (kitaNuolaida != nullptr) {
            return kitaNuolaida->skNuolaida(laikinasKrep);
        }
        else {
            return 0;
        }
    }

    void setKitaNuolaida(Nuolaida* nuolaida) {
        kitaNuolaida = nuolaida;
    }
};


class NuolaidaBrangiomsPrekems : public Nuolaida {
private:
    double riba;
    Nuolaida* kitaNuolaida;

public:
    NuolaidaBrangiomsPrekems(double r) : riba(r), kitaNuolaida(nullptr) {}

    double skNuolaida(Krepselis krep) {
        Krepselis laikinasKrep;
        const vector<Preke>& prekes = krep.getPrekes();
        for (const Preke& preke : prekes) {
            if (preke.getKaina() > riba) {
                laikinasKrep.PridetiPrek(preke);
            }
        }

        if (kitaNuolaida != nullptr) {
            return kitaNuolaida->skNuolaida(laikinasKrep);
        }
        else {
            return 0;
        }
    }

    void setKitaNuolaida(Nuolaida* nuolaida) {
        kitaNuolaida = nuolaida;
    }
};

class NuolaidaPavadinimui : public Nuolaida {
private:
    string raktinisZodis;
    Nuolaida* kitaNuolaida;

public:
    NuolaidaPavadinimui(const string& zodis) : raktinisZodis(zodis), kitaNuolaida(nullptr) {}

    double skNuolaida(Krepselis krep) {
        Krepselis laikinasKrep;
        const vector<Preke>& prekes = krep.getPrekes();
        for (const Preke& preke : prekes) {
            if (preke.getPavadinimas().find(raktinisZodis) != string::npos) {
                laikinasKrep.PridetiPrek(preke);
            }
        }

        if (kitaNuolaida != nullptr) {
            return kitaNuolaida->skNuolaida(laikinasKrep);
        }
        else {
            return 0;
        }
    }

    void setKitaNuolaida(Nuolaida* nuolaida) {
        kitaNuolaida = nuolaida;
    }
};
int main() {
    Parduotuve pard;

    Preke p1("tv", 101.0, "samsung");
    Preke p2("tv", 250.0, "samsung");
    Preke p3("telefonas", 150.0, "apple");
    Preke p4("telefonas", 350.0, "apple");
    Preke p5("masina", 150.0, "nokia");
    Preke p6("masina", 400.0, "nokia");

    FiksuotaNuolaida* fn1 = new FiksuotaNuolaida(300, 20);
    FiksuotaNuolaida* fn2 = new FiksuotaNuolaida(200, 15);
    FiksuotaNuolaida* fn3 = new FiksuotaNuolaida(100, 10);
    FiksuotaNuolaida* fn4 = nullptr;

    SantykineNuolaida* sn1 = new SantykineNuolaida(300, 20);
    SantykineNuolaida* sn2 = new SantykineNuolaida(200, 15);
    SantykineNuolaida* sn3 = new SantykineNuolaida(100, 10);
    SantykineNuolaida* sn4 = nullptr;

                 cout << "PAGAL KATEGORIJAS" << endl;

    NuolaidaKategorijai* nk1 = new NuolaidaKategorijai("samsung");
    NuolaidaKategorijai* nk2 = new NuolaidaKategorijai("apple");
    NuolaidaKategorijai* nk3 = new NuolaidaKategorijai("nokia");

    fn3->setKitaNuolaida(fn4);
    fn2->setKitaNuolaida(fn3);
    fn1->setKitaNuolaida(fn2);
    nk1->setKitaNuolaida(fn1);
    pard.setNuolaida(nk1);
    Krepselis krep1;
    krep1.PridetiPrek(p1);
    cout << "Nuolaida (suma 101, nuolaida taikoma -10, kai kategorija samsung): " << pard.kainaPoNuolaidos(krep1) << endl;

    fn3->setKitaNuolaida(fn4);
    fn2->setKitaNuolaida(fn3);
    fn1->setKitaNuolaida(fn2);
    nk2->setKitaNuolaida(fn1);
    pard.setNuolaida(nk2);
    Krepselis krep2;
    krep2.PridetiPrek(p3);
    krep2.PridetiPrek(p4);
    cout << "Nuolaida (suma 150+350=500, turetu but -20, kai kategorija apple): " << pard.kainaPoNuolaidos(krep2) << endl;

    sn3->setKitaNuolaida(sn4);
    sn2->setKitaNuolaida(sn3);
    sn1->setKitaNuolaida(sn2);
    nk3->setKitaNuolaida(sn1);
    pard.setNuolaida(nk3);
    Krepselis krep3;
    krep3.PridetiPrek(p4);
    krep3.PridetiPrek(p5);
    krep3.PridetiPrek(p6);
    cout << "Nuolaida (suma 150+400=550, turetu but-20%(110), kai kategorija nokia): " << pard.kainaPoNuolaidos(krep3) << endl << endl << endl;

                cout << "PAGAL BRANGUMA" << endl;

    NuolaidaBrangiomsPrekems* nbp1 = new NuolaidaBrangiomsPrekems(300.0);
    NuolaidaBrangiomsPrekems* nbp2 = new NuolaidaBrangiomsPrekems(200);
    NuolaidaBrangiomsPrekems* nbp3 = new NuolaidaBrangiomsPrekems(100);

    fn3->setKitaNuolaida(fn4);
    fn2->setKitaNuolaida(fn3);
    fn1->setKitaNuolaida(fn2);
    nbp1->setKitaNuolaida(fn1);
    pard.setNuolaida(nbp1);
    Krepselis krep4;
    krep4.PridetiPrek(p6);
    cout << "Nuolaida (suma 450, nuolaida taikoma kai -20, kai >300): " << pard.kainaPoNuolaidos(krep4) << endl;

    fn3->setKitaNuolaida(fn4);
    fn2->setKitaNuolaida(fn3);
    fn1->setKitaNuolaida(fn2);
    nbp2->setKitaNuolaida(fn1);
    pard.setNuolaida(nbp2);
    Krepselis krep5;
    krep5.PridetiPrek(p2);
    cout << "Nuolaida (suma 250, turetu but -15, kai >200): " << pard.kainaPoNuolaidos(krep5) << endl;

    sn3->setKitaNuolaida(sn4);
    sn2->setKitaNuolaida(sn3);
    sn1->setKitaNuolaida(sn2);
    nbp3->setKitaNuolaida(sn1);
    pard.setNuolaida(nbp3);
    Krepselis krep6;
    krep6.PridetiPrek(p3);
    krep6.PridetiPrek(p5);
    cout << "Nuolaida (suma 150+150=300, turetu but-20%(45), kai >100): " << pard.kainaPoNuolaidos(krep6) << endl << endl << endl;
    
    
                    cout << "PAGAL PAVADINIMA"  << endl;

    NuolaidaPavadinimui* np1 = new NuolaidaPavadinimui("tv");
    NuolaidaPavadinimui* np2 = new NuolaidaPavadinimui("telefonas");
    NuolaidaPavadinimui* np3 = new NuolaidaPavadinimui("masina");

    fn3->setKitaNuolaida(fn4);
    fn2->setKitaNuolaida(fn3);
    fn1->setKitaNuolaida(fn2);
    np1->setKitaNuolaida(fn1);
    pard.setNuolaida(np1);
    Krepselis krep7;
    krep7.PridetiPrek(p1);
    cout << "Nuolaida (suma 101, nuolaida taikoma kai -10, kai pavadinimas tv): " << pard.kainaPoNuolaidos(krep7) << endl;

    fn3->setKitaNuolaida(fn4);
    fn2->setKitaNuolaida(fn3);
    fn1->setKitaNuolaida(fn2);
    np2->setKitaNuolaida(fn1);
    pard.setNuolaida(np2);
    Krepselis krep8;
    krep8.PridetiPrek(p3);
    krep8.PridetiPrek(p4);
    cout << "Nuolaida (suma 150+350=500, turetu but -20, kai pavadinimas telefonas): " << pard.kainaPoNuolaidos(krep8) << endl;

    sn3->setKitaNuolaida(sn4);
    sn2->setKitaNuolaida(sn3);
    sn1->setKitaNuolaida(sn2);
    np3->setKitaNuolaida(sn1);
    pard.setNuolaida(np3);
    Krepselis krep9;
    krep9.PridetiPrek(p4);
    krep9.PridetiPrek(p5);
    krep9.PridetiPrek(p6);
    cout << "Nuolaida (suma 150+400=550, turetu but-20%(110), kai pavadinimas masina): " << pard.kainaPoNuolaidos(krep9) << endl;

    return 0;
}

/* 
  Is pradziu dariau viska per viena grandine pvz

    fn3->setKitaNuolaida(fn4);
    fn2->setKitaNuolaida(fn3);
    fn1->setKitaNuolaida(fn2);
    np3->setKitaNuolaida(fn1);
    np2->setKitaNuolaida(np3);
    np1->setKitaNuolaida(np2);

  kad patikrintu visas kategorijas ir tada eitu per nuolaidas, 
  taciau taip neveikia, todel dariau daug grandiniu.
*/
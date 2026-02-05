// Visual Studio
// Ugnius Padolskis

#include <iostream>
#include <vector>
#include<iterator>
#include "H11.h"

using namespace std;

//------------TASKAS----------------
Taskas::Taskas(double x, double y) {
    this->x = x, this->y = y;
}

double Taskas::get_x() const {
    return x;
}
double Taskas::get_y() const {
    return y;
}
void Taskas::set_x(double x) {
    this->x = x;
}
void Taskas::set_y(double y) {
    this->y = y;
}
//------------TASKAS----------------
// 
//------------FIGURA----------------
Figura::Figura(Taskas pos) {
    pozicija = pos;
}
Taskas Figura::get_pos() {
    return pozicija;
}
void Figura::set_pos(Taskas pos) {
    pozicija = pos;
}
void Figura::spausdinti() {
    std::cout << "Tsk pos(centras): " << get_pos().get_x() << " " << get_pos().get_y() << endl;
}
//------------FIGURA----------------
// 
//------------SKRITULYS-------------
Skritulys::Skritulys(Taskas centras, double spindulys) : Figura(centras) {
    this->spindulys = spindulys;
    cout << "Skritulys Konstruktorius" << endl;
}
void Skritulys::set_spin(double spindulys) {
    this->spindulys = spindulys;
}
double Skritulys::get_spin() {
    return spindulys;
}
double Skritulys::perimetras() {
    return 2 * pi * spindulys;
}
double Skritulys::plotas() {
    return pi * spindulys * spindulys;
}
void Skritulys::spausdinti() {

    std::cout << "Skritulys" << endl;
    Figura::spausdinti();
    std::cout << "Skritulio spindulys: " << spindulys << endl;

}
Skritulys::~Skritulys() {
    cout << "Skritulys Destruktorius" << endl;
}
//------------SKRITULYS-------------
// 
//------------ELIPSE----------------
Elipse::Elipse(Taskas pos, double spindulys, double b) : Skritulys(pos, spindulys) {
    cout << "Elipse Konstruktorius" << endl;
    this->b = b;
}
Elipse::~Elipse() {
    cout << "Elipse Destruktorius" << endl;
}
double Elipse::get_a() {
    return spindulys;
}
double Elipse::get_b() {
    return b;
}
void Elipse::set_a(double a) {
    spindulys = a;
}
void Elipse::set_b(double b) {
    this->b = b;
}
double Elipse::perimetras() {
    return pi * (3 * (spindulys + b) - sqrt((3 * spindulys + b) * (spindulys + 3 * b)));
}
double Elipse::plotas() {
    return pi * spindulys * b;
}
void Elipse::spausdinti() {

    std::cout << "Elipse" << endl;
    Figura::spausdinti();
    std::cout << "Elipses pusasis a: " << spindulys << endl;
    std::cout << "Elipses pusasis b: " << b << endl;

}
//------------ELIPSE----------------
// 
//------------KVADRATAS-------------
Kvardratas::Kvardratas(Taskas pos, double krastine1) : Figura(pos) {
    cout << "Kvardratas Konstruktorius" << endl;
    this->krastine1 = krastine1;
}

Kvardratas::~Kvardratas() {
    cout << "Kvardratas Destruktorius" << endl;
}
double Kvardratas::get_kras1() {
    return krastine1;
}

void Kvardratas::set_kras1(double krastine1) {
    this->krastine1 = krastine1;
}

double Kvardratas::plotas() {
    return krastine1 * krastine1;
}

double Kvardratas::perimetras() {
    return  krastine1 * 4;
}
void Kvardratas::spausdinti() {

    std::cout << "Kvadratas" << endl;
    Figura::spausdinti();
    std::cout << "Kvadrato krastine: " << krastine1 << endl;

}
//------------KVADRATAS-------------
//
//------------STACIAKAMPIS----------
Staciakampis::Staciakampis(Taskas pos, double krastine1, double  krastine2) : Kvardratas(pos, krastine1), krastine2(krastine2) {
    cout << "Staciakampis Konstruktorius" << endl;
}
Staciakampis::~Staciakampis() {
    cout << "Staciakampis Destruktorius" << endl;
}

double Staciakampis::get_kras2() {
    return krastine2;
}

void Staciakampis::set_kras2(double krastine2) {
    this->krastine2 = krastine2;
}
double Staciakampis::perimetras() {
    return 2 * (krastine1 + krastine2);
}
double Staciakampis::plotas() {
    return krastine1 * krastine2;

}
void Staciakampis::spausdinti() {

    std::cout << "Staciakampis" << endl;
    Figura::spausdinti();
    std::cout << "Krastines ilgiai: " << krastine1 << ", " << krastine2 << endl;

}
//------------STACIAKAMPIS----------


int main() {
    
    
    Taskas tt(6, 9);
    /* std::cout << "tasko kords: " << t.get_x() << " " << t.get_y() << endl;
   
   t.set_x(5.0); t.set_y(2.5);
   std::cout << "tasko kords: " << t.get_x() << " " << t.get_y() << endl;

   std::cout << endl;

    Figura f(t);
    f.spausdinti();

    std::cout << endl;

    Skritulys s(t,12);
    
    s.spausdinti();
    std::cout << "Skritulio perimetras: " << s.perimetras() << endl;
    std::cout << "Skritulio plotas: " << s.plotas() << endl;
    std::cout << endl;

    Elipse e(t, 12, 8);

    e.spausdinti();
    std::cout << "Elipses perimetras: " << e.perimetras() << endl;
    std::cout << "Elipses plotas: " << e.plotas() << endl;
    std::cout << endl;

    Kvardratas k(t, 8);

    k.spausdinti();
    std::cout << "Kvadrato perimetras: " << k.perimetras() << endl;
    std::cout << "Kvadrato plotas: " << k.plotas() << endl;
    std::cout << endl;

    Staciakampis st(t, 8, 6);

    st.spausdinti();
    std::cout << "Staciakampio perimetras: " << st.perimetras() << endl;
    std::cout << "Staciakampio plotas: " << st.plotas() << endl;
    */
    vector<Figura*> fig;

    Taskas* t = new Taskas(6, 9);
    Figura* f = new Figura(*t);
    Skritulys* f1 = new Skritulys(*t, 10);
    Elipse* f2 = new Elipse(*t, 10,8);
    Kvardratas* f3 = new Kvardratas(*t, 4);
    Staciakampis* f4 = new Staciakampis(*t,4, 6);

    std::cout << endl;

    fig.push_back(f1);
    fig.push_back(f2);
    fig.push_back(f3);
    fig.push_back(f4);

    int sKiek = 0;
    int eKiek = 0;
    int kKiek = 0;
    int stKiek = 0;

    double p = 0;
    double plot = 0;

    for (vector<Figura*>::iterator it = fig.begin(); it != fig.end(); ++it) {
        
        if (Elipse* e = dynamic_cast<Elipse*>(*it)) {
            cout << endl;
            dynamic_cast<Elipse*>(*it)->spausdinti();
            eKiek++;
            p += e->perimetras();
        }
        else if (Skritulys* s = dynamic_cast<Skritulys*>(*it)) {
            cout << endl;
            s->spausdinti();
            sKiek++;
            p += s->perimetras();
        }
        
       
        if (Staciakampis* st = dynamic_cast<Staciakampis*>(*it)) {
            cout << endl;
            dynamic_cast<Staciakampis*>(*it)->spausdinti(); 
            stKiek++;
            plot += st->plotas();
        }
        else if (Kvardratas* k = dynamic_cast<Kvardratas*>(*it)) {
            cout << endl;
            dynamic_cast<Kvardratas*>(*it)->spausdinti();
            kKiek++;
            plot += k->plotas();
        }
        
    }
    cout << endl << endl;
    cout << "Skrituliu kiekis: " << sKiek << endl;
    cout << "Elipsiu kiekis: " << eKiek << endl;
    cout << "Kvardratu kiekis: " << kKiek << endl;
    cout << "Staciakampiu kiekis: " << stKiek << endl;

    cout << endl;
    cout << "Apvaliu perimetras: " << p << endl;
    cout << "Kampuotu plotas: " << plot << endl;
    cout << endl;

    for (vector<Figura*>::iterator it = fig.begin(); it != fig.end(); ++it) {
        delete* it;
    }
   
    return 0;
}


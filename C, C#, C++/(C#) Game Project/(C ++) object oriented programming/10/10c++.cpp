// Visual Studio
// Ugnius Padolskis

#include <iostream>
#include "H10.h"

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
//------------SKRITULYS-------------
// 
//------------ELIPSE----------------
Elipse::Elipse(Taskas pos, double spindulys, double b) : Skritulys(pos, spindulys) {
    this->b = b;
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
double Elipse::plotas(){
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
Kvardratas::Kvardratas(Taskas pos, double krastine) : Figura(pos) {
    this->krastine = krastine;
}
double Kvardratas::get_kras() {
    return krastine;
}

void Kvardratas::set_kras(double krastine) {
    this->krastine = krastine;
}

double Kvardratas::plotas(){
    return krastine * krastine;
}

double Kvardratas::perimetras() {
    return  krastine * 4;
}
void Kvardratas::spausdinti() {
    
    std::cout << "Kvadratas" << endl;
    Figura::spausdinti();
    std::cout << "Kvadrato krastine: " << krastine << endl;
    
}
//------------KVADRATAS-------------
//
//------------STACIAKAMPIS----------
Staciakampis::Staciakampis(Taskas pos, double krastine, double b) : Kvardratas(pos, krastine), b(b) {}
double Staciakampis::get_a() {
    return krastine;
}
double Staciakampis::get_b(){
    return b;
}
void Staciakampis::set_a(double a) {
    krastine = a;
}
void Staciakampis::set_b(double b) {
    this->b = b;
}
double Staciakampis::perimetras() {
    return 2 * (krastine + b);
}
double Staciakampis::plotas() {
    return krastine * b;
    
}
void Staciakampis::spausdinti() {
    
    std::cout << "Staciakampis" << endl;
    Figura::spausdinti();
    std::cout << "Krastines ilgiai: " << krastine << ", " << b << endl;
   
}
//------------STACIAKAMPIS----------



int main(){
    Taskas t(2.5, 5.0);
    std::cout << "tasko kords: " << t.get_x() << " " << t.get_y() << endl;
    
    t.set_x(5.0); t.set_y(2.5);
    std::cout << "tasko kords: " << t.get_x() << " " << t.get_y() << endl;
    
    std::cout << endl;
    
    Figura f(t);
    f.spausdinti();

    std::cout << endl;

    Skritulys s(t, 5.0);
    s.spausdinti();
    std::cout << "Skritulio perimetras: " << s.perimetras() << endl;
    std::cout << "Skritulio plotas: " << s.plotas() << endl;
    std::cout << endl;

    Elipse e(t, 2.0, 3.0);
    e.spausdinti();
    std::cout << "Elipses perimetras: " << e.perimetras() << endl;
    std::cout << "Elipses plotas: " << e.plotas() << endl;
    std::cout << endl;

    Kvardratas k(t, 5);
    k.spausdinti();
    std::cout << "Kvadrato perimetras: " << k.perimetras() << endl;
    std::cout << "Kvadrato plotas: " << k.plotas() << endl;
    std::cout << endl;

    Staciakampis st(t, 4, 6);
    st.spausdinti();
    std::cout << "Staciakampio perimetras: " << st.perimetras() << endl;
    std::cout << "Staciakampio plotas: " << st.plotas() << endl;
    return 0;
}


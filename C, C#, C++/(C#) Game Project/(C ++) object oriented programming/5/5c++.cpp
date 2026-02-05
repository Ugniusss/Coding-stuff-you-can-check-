#include <iostream>
#include <stdexcept>
#include <limits>
#include <string>
#include "H.h"

using namespace std;

Trupmena::Trupmena() {
    skaitiklis = 1;
    vardiklis = 1;
}

Trupmena::Trupmena(int skaitiklis, int vardiklis) {
    try {
        if (vardiklis == 0) {
            throw invalid_argument("vardiklis = 0");
        }
        this->skaitiklis = skaitiklis;
        this->vardiklis = vardiklis;
        Paprastinti();
    }

    catch (invalid_argument a) {
        cout << a.what() << endl;
    }

}
int Trupmena::BendrasDal(int sk, int va) {
    if (va == 0) {
        return sk;
    }
    return BendrasDal(va, sk % va);
}

void Trupmena::Paprastinti() {
    int bDal;
    bDal = BendrasDal(skaitiklis, vardiklis);
    this->skaitiklis /= bDal;
    this->vardiklis /= bDal;
}
void Trupmena::toString() {
    string trup;
    trup = to_string(skaitiklis) + "/" + to_string(vardiklis);
    cout << trup;
}
void Trupmena::Add(Trupmena a) {
    this->skaitiklis = skaitiklis * a.vardiklis + a.skaitiklis * vardiklis;
    this->vardiklis = vardiklis * a.vardiklis;

    Paprastinti();
}

Trupmena Trupmena::operator+(const Trupmena& t) const {
    Trupmena trup;
    trup.skaitiklis = (t.vardiklis * skaitiklis) + (vardiklis * t.skaitiklis);
    trup.vardiklis = vardiklis * t.vardiklis;
    trup.Paprastinti();
    return trup;
}

Trupmena Trupmena::operator-(const Trupmena& t) const {
    Trupmena trup;
    trup.skaitiklis = (t.vardiklis * skaitiklis) - (vardiklis * t.skaitiklis);
    trup.vardiklis = vardiklis * t.vardiklis;
    trup.Paprastinti();
    return trup;
}
Trupmena& Trupmena::operator+=(const Trupmena& t) {
    this->skaitiklis = (t.vardiklis * skaitiklis) + (vardiklis * t.skaitiklis);
    this->vardiklis = vardiklis * t.vardiklis;
    this->Paprastinti();
    return *this;
}
Trupmena& Trupmena::operator-=(const Trupmena& t) {
    this->skaitiklis = (t.vardiklis * skaitiklis) - (vardiklis * t.skaitiklis);
    this->vardiklis = vardiklis * t.vardiklis;
    this->Paprastinti();
    return *this;
}


Trupmena& Trupmena::operator++() {
    this->skaitiklis = this->skaitiklis + this->vardiklis;
    this->Paprastinti();
    return *this;


}

Trupmena Trupmena::operator++(int) {
    Trupmena y(*this);
    this->skaitiklis = this->skaitiklis + this->vardiklis;
    this->Paprastinti();
    return y;
}

ostream& operator<<(ostream& isv, const Trupmena& t) {
    isv << t.skaitiklis << '/' << t.vardiklis;
    return isv;
}

bool Trupmena::operator< (Trupmena t) {
    return (this->skaitiklis * t.vardiklis) < (t.skaitiklis * this->vardiklis);
}
bool Trupmena::operator> (Trupmena t) {
    return (skaitiklis * t.vardiklis) > (t.skaitiklis * vardiklis);
}

int main(){
    Trupmena a(1, 4);
    Trupmena b(6, 12);
    Trupmena c = a + b;
    Trupmena d = b-a;
    Trupmena e;
    
    
    if (a < b) {
        cout << " a mazesnis" << endl;
    }
    else {
        cout << " b mazesnis" << endl;

    }

    if (a > b) {
        cout << " a didesnis" << endl;
    }
    else {
        cout << " b didesnis" << endl;

    }
    
    
    c.toString(); cout <<"  a+b (1/4 + 1/2 = 3/4)" << endl;
    d.toString(); cout <<"  b-a (1/2 - 1/4 = 1/4)" << endl;
    d += a;
    d.toString(); cout <<"  d+=a (1/4 + 1/4 = 1/2)" << endl;
    d -= a;
    d.toString(); cout <<"  d-=a (1/2 - 1/4 = 1/4)" << endl;
    d++;
    d.toString(); cout <<"  d++ (1/4 + 1 = 5/4)" << endl;
    e = d++;
    e.toString(); cout <<"  e = ++d (1/4 + 1 = 5/4)" << endl;
    cout << a << " " << b; cout << "  a b (1/4 1/2)" << endl;
    return 0;
}


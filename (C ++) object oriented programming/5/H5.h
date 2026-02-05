#ifndef H_H
#define H_H

#include <iostream>
#include <stdexcept>
#include <string>
#include <limits>

using namespace std;

class Trupmena {
private:
    int skaitiklis;
    unsigned int vardiklis;
    void Paprastinti();
    int BendrasDal(int sk, int va);

public:
    Trupmena();
    Trupmena(int skaitiklis, int vardiklis);  
    void toString();
    void Add(Trupmena a);
    Trupmena operator+(const Trupmena& t) const;
    Trupmena operator-(const Trupmena& t) const;
    Trupmena& operator+=(const Trupmena& t);
    Trupmena& operator-=(const Trupmena& t);
    bool operator< (Trupmena t);
    bool operator> (Trupmena t);
    friend ostream& operator<<(ostream& isv, const Trupmena& t);
    Trupmena& operator++();
    Trupmena operator++(int);
};
#endif 
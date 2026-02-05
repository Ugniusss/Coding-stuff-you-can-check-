#pragma once
#ifndef HEADER_4
#define HEADER_4

#include <stdexcept>
#include <limits>

using namespace std;

class Skaicius {
private:
    int sk;
    static int minimaliReiksme;
    static int maximaliReiksme;

public:
    int getReiksme();
    void setReiksme(int a);
    static void setMin(int a);
    static void setMax(int a);
};

int Skaicius::minimaliReiksme;
int Skaicius::maximaliReiksme;

#endif
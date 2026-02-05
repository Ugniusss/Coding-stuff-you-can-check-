#ifndef HEADER_H
#define HEADER_H

#include <vector>

using namespace std;

class Langas {
private:
    float aukstis;
    float plotis;
    static int sukurtalangu;

public:
    Langas(float a, float b);
    ~Langas();
    float getAukstis();
    float getPlotis();
    static int getLang();
   
};

int Langas::sukurtalangu = 0;

class Durys {
private:
    float aukstis;
    float plotis;
    static int sukurtaduru;

public:
    Durys(float a, float b);
    ~Durys();
    float getAukstis();
    float getPlotis();
    static int getDur();
   
};

int Durys::sukurtaduru = 0;

class Kambarys {
private:
    float aukstis;
    float plotis;
    float ilgis;
    vector <Langas*> lan;
    vector <Durys*> dur;

public:
    Kambarys(float a, float b, float c);
    ~Kambarys();
    float getAukstis();
    float getPlotis();
    float getIlgis();
    void papLangas(Langas *l);
    void papDuris(Durys *d);
    float sienosP();
    float grindI();
    
};


#endif


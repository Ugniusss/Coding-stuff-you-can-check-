#ifndef H6_H
#define H6_H

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
    Durys* dur;

public:
    Kambarys(float a, float b, float c);
    Kambarys(const Kambarys& k);
    ~Kambarys();
    float getAukstis();
    float getPlotis();
    float getIlgis();
    void papLangas(Langas* l);
    void papDuris(Durys* d);
    Durys* getDurys();
    vector <Langas*> getLangas();
    Kambarys& operator=(const Kambarys& k);
};

float sienosP(Kambarys k);
float grindI(Kambarys k);

#endif


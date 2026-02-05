#ifndef HEADER_H
#define HEADER_H

#include <vector>

using namespace std;

class Langas {
public:
    void setAukstis(float a);
    float getAukstis() { return aukstis; }
    void setPlotis(float a);
    float getPlotis() { return plotis; }

private:
    float aukstis;
    float plotis;
};

class Durys {
public:
    void setAukstis(float a);
    float getAukstis() { return aukstis; }
    void setPlotis(float a);
    float getPlotis() { return plotis; }

private:
    float aukstis;
    float plotis;
};

class Kambarys {
public:
    void setAukstis(float a);
    float getAukstis() { return aukstis; }
    void setPlotis(float a);
    float getPlotis() { return plotis; }
    void setIlgis(float a);
    float getIlgis() { return ilgis; }
    void papLangas(Langas l);
    void papDuris(Durys d);
    float sienosP();
    float grindI();

private:
    float aukstis;
    float plotis;
    float ilgis;
    vector <Langas> lan;
    vector <Durys> dur;
};

#endif
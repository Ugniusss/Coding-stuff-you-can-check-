#pragma once
#ifndef H10_H
#define H10_H

const double pi = 3.14;

class Taskas {
private:
    double x;
    double y;

public:
    Taskas(double x = 0, double y = 0);
    ~Taskas() {}
    double get_x() const;
    double get_y() const;
    void set_x(double x);
    void set_y(double y);
};

class Figura {
protected:
    Taskas pozicija;
public:
    Figura(Taskas pos = Taskas());
    virtual ~Figura() {}
    Taskas get_pos();
    void set_pos(Taskas pos);
    void spausdinti();
};

class Skritulys : public Figura {
protected:
    double spindulys;

public:
    Skritulys(Taskas centras = Taskas(), double spindulys = 0);
    virtual ~Skritulys() {}
    void set_spin(double spindulys);
    double get_spin();
    double perimetras();
    double plotas();
    virtual void spausdinti();
};

class Elipse : public Skritulys {
protected:
    double b;

public:
    Elipse(Taskas centras = Taskas(), double spindulys = 0, double b = 0);
    ~Elipse(){}
    double get_a();
    double get_b();
    void set_a(double a);
    void set_b(double b);
    double perimetras();
    double plotas();
    void spausdinti();
};

class Kvardratas : public Figura {
protected:
    double krastine;

public:
    Kvardratas(Taskas pos = Taskas(), double krastine = 0);
    ~Kvardratas() {}
    double get_kras();
    void set_kras(double krastine);
    double perimetras();
    double plotas();
    void spausdinti();
};

class Staciakampis : public Kvardratas {
protected:
    double b;

public:
    Staciakampis(Taskas pos = Taskas(), double krastine = 0, double b = 0);
    ~Staciakampis(){}
    double get_a();
    double get_b();
    void set_a(double a);
    void set_b(double b);
    double perimetras();
    double plotas();
    void spausdinti();
};
#endif
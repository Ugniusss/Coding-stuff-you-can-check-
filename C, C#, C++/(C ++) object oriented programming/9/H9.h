// Visual Studio
#pragma once

#ifndef H9_H
#define H9_H

#include <iostream>
#include <vector>
#include <string>
#include <algorithm>

using namespace std;

template <class T> class ManoIteratorius;

template <class T>
class Vector {
private:
    T* t;
    int Paskutinis;
    int dydis;
    int zingsnis = 5;
public:
    Vector(int dydis, int zingsnis);
    ~Vector();
    int size();
    void push_back(const T& el);
    T& operator[](int i)const;
    friend class ManoIteratorius <T>;
    typedef ManoIteratorius<T> iterator;
    iterator begin();
    iterator end();
};

template <class T>
class ManoIteratorius {
    typedef ManoIteratorius<T> iterator;
private:
    Vector <T> &vec;
    int pos = 0;

public:
    ManoIteratorius(Vector<T>& v, int p);
    T& operator*()const;
    iterator& operator++();
    bool operator!=(const ManoIteratorius<T>& other);

    bool operator<(const ManoIteratorius<T>& other);
};

class SkaiciuPalyginimas {
public:
    int operator()(int i1, int i2);
};

class StringPalyginimas {
public:
    int operator()(const string& s1, const string& s2);
};

template <typename T, class FunkcinisObjektas>
inline void BubbleSort(vector<T> array, FunkcinisObjektas fnct);


#endif
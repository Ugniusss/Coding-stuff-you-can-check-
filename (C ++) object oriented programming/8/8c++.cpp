// Visual Studio
#include "H8.h"

using namespace std;

template <class T> Vector<T>::Vector(int d, int z) {
    t = new T[d];
    Paskutinis = 0;
    dydis = d;
    zingsnis = z;
}

template <class T> Vector<T>::~Vector() {
    delete[] t;
    Paskutinis = 0;
}

template <class T> int Vector<T>::size() {
    return Paskutinis;
}

template <class T> void Vector<T>::push_back(const T& el) {

    if (dydis <= Paskutinis) {
        cout << "PUSH " << endl;
        T* tNaujas = new T[dydis + zingsnis];
        for (int i = 0; i < dydis; ++i) {
            tNaujas[i] = t[i];
        }
        delete[] t;
        t = tNaujas;
        dydis += zingsnis;
    }
    t[Paskutinis] = el;
    Paskutinis++;
}

template <class T> T& Vector<T>::operator[](int i) const{
    return t[i];
}

template <typename T>
inline void BubbleSort(Vector <T> &array) {
    for (typename Vector<T>::iterator i = array.begin(); i != array.end(); ++i)
        for (typename Vector<T>::iterator j = array.begin(); j < array.end(); ++j)
            if (*i < *j)
                std::iter_swap(i, j);
}

template <typename T>
ManoIteratorius<T>::ManoIteratorius(Vector<T>& v, int p) : vec(v), pos(p){}

template <class T>
typename Vector<T>::iterator Vector<T>::begin(){
    return iterator(*this, 0);
}

template <class T>
typename Vector<T>::iterator Vector<T>::end(){
    return iterator(*this, Paskutinis);
}

template <typename T>
T& ManoIteratorius<T>::operator*() const {
    return vec[pos];
}

template <typename T>
ManoIteratorius<T>& ManoIteratorius<T>::operator++() {
    this->pos++;
    return *this;
}
template <typename T>
bool ManoIteratorius<T>::operator!=(const ManoIteratorius<T>& other) {
    bool lyg = true;
    if (this->pos == other.pos) {
        lyg = false;

    }
    return lyg;
}

template <typename T>
bool ManoIteratorius<T>::operator<(const ManoIteratorius<T>& other) {
    bool sp = false;
    if (this->pos < other.pos) {
        sp = true;

    }
    return sp;
}

int main() {
    Vector <int> skVec(3, 2);
    
    //vector <int> skVec;
    int sk;
    while (1) {
        cin >> sk;
        if (sk == 0) {
            break;
        }
        skVec.push_back(sk);
    }
    
    
    
    cout << endl;
    cout << endl;
    cout << "Po rikiavimo:" << endl;

     //RIKIAVIMAS
    BubbleSort(skVec);

    for (auto i = skVec.begin(); i != skVec.end(); ++i) {
        cout << *i << " ";
    }
    
    cout << endl;
    cout << endl;
    cout << endl;

    Vector <string> zdVec(3,2);
    string zd;
    while (1) {
        cin >> zd;
        if (zd == "-") {
            break;
        }
        zdVec.push_back(zd);
    }

   
    
    cout << endl;
    cout << endl;
    cout << "Po rikiavimo:" << endl;

     //RIKIAVIMAS
    BubbleSort(zdVec);
    for (auto i = zdVec.begin(); i != zdVec.end(); ++i) {
        cout << *i << " ";
    }
    
    return 0;
}



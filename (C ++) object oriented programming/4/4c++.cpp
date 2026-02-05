//Visual Studio
//Ugnius Padolskis Informatika 1k 1g

#include <iostream>
#include "Header_4.h"

using namespace std;

 void Skaicius::setReiksme(int a) {
     if (minimaliReiksme > a || a > maximaliReiksme) {
         throw out_of_range("Iseina is reziu");
     }
    sk = a;
}

int Skaicius::getReiksme() {
    return sk;
}

void Skaicius::setMin(int a) {
    minimaliReiksme = a;
}
void Skaicius::setMax(int a) {
    maximaliReiksme = a;
}

int main(){
    Skaicius::setMin(0);
    Skaicius::setMax(100);
    Skaicius m[10] = {};
    int skaicius;
    int i = 0;  
        while (i < 10) {
            try {
                if (!(cin >> skaicius)) {
                    throw invalid_argument("Ivesta raide");
                }
                m[i].setReiksme(skaicius);
                i++;
            }
            catch (invalid_argument a) {
                cin.clear();
                cin.ignore(numeric_limits<streamsize>::max(), '\n');
                cout << a.what() << endl;
            }
            catch ( out_of_range b) {
                cout << b.what() << endl;
            }
        } 
    cout << "Ivesti skaiciai: ";
    for (int i = 0; i < 10; ++i) {
        cout << m[i].getReiksme() << " ";
    }
    return 0;
}


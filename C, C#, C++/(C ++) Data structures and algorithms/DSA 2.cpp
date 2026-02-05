//Visual Studio
#include <iostream>
#include <vector>
#include <string>

using namespace std;

typedef vector <int> Ilgas;

Ilgas readIlgas();
void printIlgas(Ilgas& num);
Ilgas sumIlgas(Ilgas& sk1, Ilgas& sk2);

int main() {
    Ilgas sk1 = readIlgas();
    Ilgas sk2 = readIlgas();
    Ilgas suma = sumIlgas(sk1, sk2);
    printIlgas(suma);
    return 0;
}

void printIlgas(Ilgas& num) {
    for (int i = num.size() - 1; i >= 0; i--) {
        cout << num[i];
    }
    cout << endl;
}

Ilgas readIlgas() {
    string sk;
    cin >> sk;
    Ilgas num;
    for (int i = sk.size() - 1; i >= 0; i--) {
        char b = sk[i] - '0';
        num.push_back(b);
    }
    return num;
}

/*

1111111111111111111111111111111111111111111111111111111111111111111111111

3333333333333333333333333333333333333333333333333333333333333333333333333

*/

Ilgas sumIlgas(Ilgas& sk1, Ilgas& sk2) {
    Ilgas suma;
    int a = 0;
    for (int i = 0; i < max(sk1.size(), sk2.size()); i++) {
        int skSum = a;
        if (i < sk1.size()) {
            skSum += sk1[i];
        }
        if (i < sk2.size()) {
            skSum += sk2[i];
        }
        suma.push_back(skSum % 10);
        a = skSum / 10;
    }
    if (a > 0) {
        suma.push_back(a);
    }
    return suma;
}



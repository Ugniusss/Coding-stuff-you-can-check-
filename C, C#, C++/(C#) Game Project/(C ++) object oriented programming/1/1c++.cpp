// Visual Studio
#include <iostream>
#include <vector>
#include <string>

using namespace std;

void Bub(vector<int>& a, int size);
void Bubb(vector<string>& b, int size);

int main(){
    // 1)
    //cout << "Hello World!\n";

    // 2)
    //int a[100];
    int b;
    int size = 0;
    vector <int> a;
    
    for (int i = 0; i < 100; ++i) {
        cin >> b;
        if (b != 0) {
            a.push_back(b);
            size++;
        }
        else {
            i = 101;
        }

    }
    
    // 3)
    
    // 4)

    // 5)
    /*
    vector <string> b;
    string c;
    for (int i = 0; i < 100; ++i) {
        cin >> c;
        if (c == "-") {
            i = 101;
        }
        else {
            b.push_back(c);
            size++;
        }

    }*/
    Bub(a, size);
    //Bubb(b, size);
    for (int i = 0; i < size; ++i) {
        cout << a[i] << " ";
        //cout << b[i] << " ";
    }
    return 0;
}

// 3)

void Bub(vector<int>& a, int size) {
    for (int i = 0; i < size - 1; ++i) {
        for (int j = 0; j < size - 1 - i; ++j) {
            if (a[j] > a[j + 1]) {
                swap(a[j], a[j + 1]);
            }
        }
    }
}


//5)
void Bubb(vector<string>& b, int size) {
for (int i = 0; i < size - 1; ++i) {
    for (int j = 0; j < size - 1 - i; ++j) {
        if (b[j] > b[j + 1]) {
            swap(b[j], b[j + 1]);
        }
    }
}
}

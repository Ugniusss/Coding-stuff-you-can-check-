//Visual Studio
#include <iostream>
#include <vector>
#include <ctime>
#include <cstdlib>

using namespace std;
class ArrayList {
private:
    int* data;
    int size;
    int capacity;
public:
    ArrayList();
    void add(int value);
    void remove(int value);
    bool contains(int value);
};

class LinkedList {
private:
    struct Node {
        int data;
        Node* next;
    };
    Node* head;
public:
    LinkedList();
    void add(int value);
    void remove(int value);
    bool contains(int value);
};

class BinarySearchTree {
private:
    struct Node {
        int data;
        Node* left;
        Node* right;
    };
    Node* root;
    void add(Node* node, int value);
    void remove(Node* node, int value);
    bool contains(Node* node, int value);
public:
    BinarySearchTree();
    void add(int value);
    void remove(int value);
    bool contains(int value);
};

class AVLTree {
private:
    struct Node {
        int data;
        Node* left;
        Node* right;
        int height;
    };
    Node* root;
    int height(Node* node);
    int balanceFactor(Node* node);
    void fixHeight(Node* node);
    Node* rotateRight(Node* node);
    Node* rotateLeft(Node* node);
    Node* balance(Node* node);
    Node* add(Node* node, int value);
    Node* remove(Node* node, int value);
    bool contains(Node* node, int value);
public:
    AVLTree();
    void add(int value);
    void remove(int value);
    bool contains(int value);
};





void test(ArrayList& list, LinkedList& linkedList, BinarySearchTree& binarySearchTree, AVLTree& avlTree, int size) {
    srand(time(NULL)); // Inicializuojame atsitiktinių skaičių generatorių
    int* values = new int[size];
    for (int i = 0; i < size; i++) {
        int value = rand() % size; // Sugeneruojame atsitiktinę reikšmę
        values[i] = value;
        list.add(value);
        linkedList.add(value);
        binarySearchTree.add(value);
        avlTree.add(value);
    }

    clock_t start, end;
    double timeUsed;

    start = clock();
    for (int i = 0; i < size; i++) {
        int value = rand() % size; // Sugeneruojame atsitiktinę reikšmę
        list.contains(value);
    }
    end = clock();
    timeUsed = ((double)(end - start)) / CLOCKS_PER_SEC;
    cout << "ArrayList contains operation time: " << timeUsed << endl;

    start = clock();
    for (int i = 0; i < size; i++) {
        int value = rand() % size; // Sugeneruojame atsitiktinę reikšmę
        binarySearchTree.contains(value);
    }
    end = clock();
    timeUsed = ((double)(end - start)) / CLOCKS_PER_SEC;
    cout << "BinarySearchTree contains operation time: " << timeUsed << endl;

    start = clock();
    for (int i = 0; i < size; i++) {
        int value = rand() % size; // Sugeneruojame atsitiktinę reikšmę
        avlTree.contains(value);
    }
    end = clock();
    timeUsed = ((double)(end - start)) / CLOCKS_PER_SEC;
    cout << "AVLTree contains operation time: " << timeUsed << endl;

    delete[] values;
}
int main() {
    ArrayList list;
    LinkedList linkedList;
    BinarySearchTree binarySearchTree;
    AVLTree avlTree;
    int size = 1000000; // Testuojame su milijonu reikšmių
    test(list, linkedList, binarySearchTree, avlTree, size);
    return 0;
}
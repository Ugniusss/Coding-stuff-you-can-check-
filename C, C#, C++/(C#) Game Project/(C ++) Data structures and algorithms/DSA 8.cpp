//Visual Studio
// Ugnius Padolskis
#include <iostream>
#include <limits.h>
#include <chrono>

using namespace std;

struct KeyValue {
    int key;
    int value;
};

struct node{
    int data;
    node* left;
    node* right;
};

node* CreateNode(int data){
    node* newnode = new node;
    newnode->data = data;
    newnode->left = NULL;
    newnode->right = NULL;

    return newnode;
}

node* InsertIntoTree(node* root, int data){
    node* temp = CreateNode(data);
    node* t = new node;
    t = root;

    if (root == NULL)
        root = temp;
    else
    {
        while (t != NULL)
        {
            if (t->data < data)
            {
                if (t->right == NULL)
                {
                    t->right = temp;
                    break;
                }
                t = t->right;
            }

            else if (t->data > data)
            {
                if (t->left == NULL)
                {
                    t->left = temp;
                    break;
                }
                t = t->left;
            }
        }
    }
    return root;
}


void Search(node* root, int data){
    int depth = 0;
    node* temp = new node;
    temp = root;
 
    while (temp != NULL)
    {
        depth++;
        if (temp->data == data)
        {
            cout << "\nData found at depth: " << depth;
            return;
        }
        else if (temp->data > data)
            temp = temp->left;
        else
            temp = temp->right;
    }

    cout << "\n Data not found";
    return;
}


int const Size = 10000;

void Insert(KeyValue ary[], int hFn, int Size, KeyValue kv) {

    int pos, n = 0;
    pos = kv.key % hFn;

    while (ary[pos].key != INT_MIN) {
        if (ary[pos].key == INT_MAX)
            break;
        pos = (pos + 1) % hFn;
        n++;
        if (n == Size)
            break;
    }

    if (n == Size)
        cout << "Hash table is full\n";
    else
        ary[pos] = kv;
}

void Delete(KeyValue ary[], int hFn, int Size, int key) {

    int n = 0, pos;

    pos = key % hFn;

    while (n++ != Size) {
        if (ary[pos].key == INT_MIN) {
            cout << "Key not found in hash table\n";
            break;
        }
        else if (ary[pos].key == key) {
            ary[pos].key = INT_MAX;
            cout << "Key-value pair deleted\n\n";
            break;
        }
        else {
            pos = (pos + 1) % hFn;
        }
    }
    if (--n == Size)
        cout << "Key not found in hash table\n";
}

void Search(KeyValue ary[], int hFn, int Size, int key) {
    int pos, n = 0;

    pos = key % hFn;

    while (n++ != Size) {
        if (ary[pos].key == key) {
            cout << "Value found at index " << pos << endl;
            break;
        }
        else
            if (ary[pos].key == INT_MAX || ary[pos].key != INT_MIN)
                pos = (pos + 1) % hFn;
    }
    if (--n == Size)
        cout << "Key not found in hash table\n";
}



int main() {
    int hashTableFunction = 10000;
    int i;
    KeyValue tab[Size];


    for (i = 0; i < Size; ++i) {
        tab[i].key = INT_MIN;
    }

    for (i = 0; i < 10000; ++i) {
        KeyValue kv;
        kv.key = i;
        kv.value = i * i;
        Insert(tab, hashTableFunction, Size, kv);
    }

    auto start = chrono::steady_clock::now();
    Search(tab, hashTableFunction, Size, 9999);
    auto end = chrono::steady_clock::now();
    cout << "Hash Table elapsed time in microseconds: " << chrono::duration_cast<chrono::microseconds>(end - start).count()
        << endl;

    node* root = new node;
    root = NULL;

    for (i = 0; i < 10000; ++i) {
        root = InsertIntoTree(root, i);
    }
       

    auto start1 = chrono::steady_clock::now();
    Search(root, 9999);
    auto end1 = chrono::steady_clock::now();

    cout << endl;

    cout << "Binary search tree elapsed time in microseconds: " << chrono::duration_cast<chrono::microseconds>(end1 - start1).count() << endl;

    return 0;
}


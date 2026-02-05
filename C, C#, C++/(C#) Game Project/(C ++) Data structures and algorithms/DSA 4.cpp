//Visual Studio
//Ugnius Padolskis Informatika 1k 1g

#include <iostream>
#include <cstdlib>
#include <ctime>
#include <chrono>

using namespace std;

//-----------CLASS----------
// Binary search tree node
class bst_Node {
public:
    int data;
    bst_Node* left;
    bst_Node* right;
};
// Linked list
class listNode {
public:
    int data;
    listNode* next;
};
//-----------CLASS----------

//----------PROTOTYPE-------
bst_Node* createNode(int data);
listNode* ll_createNode(int data);
void ll_insert(listNode*& head, int data);
void find(listNode* head);
void insert(bst_Node*& root, int data);
void find(bst_Node* root);
//----------PROTOTYPE-------


int main() {
    srand(time(NULL));
    bst_Node* root = NULL;
    listNode* head = NULL;
    for (int i = 0; i < 100000; i++) {
        int num = rand() % 100;
        insert(root, num);
        ll_insert(head, num);
    }
    auto start = chrono::high_resolution_clock::now();
    find(root);
    auto end = chrono::high_resolution_clock::now();
    auto start1 = chrono::high_resolution_clock::now();
    find(head);
    auto end1 = chrono::high_resolution_clock::now();
    auto duration = std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count();
    auto duration1 = std::chrono::duration_cast<std::chrono::milliseconds>(end1 - start1).count();
    cout << "binary search tree: " << duration << " milliseconds\n";
    cout << "linked list: " << duration1 << " milliseconds\n";
    return 0;
}

bst_Node* createNode(int data) {
    bst_Node* newNode = new bst_Node();
    newNode->data = data;
    newNode->left = NULL;
    newNode->right = NULL;
    return newNode;
}

listNode* ll_createNode(int data) {
    listNode* newNode = new listNode();
    newNode->data = data;
    newNode->next = NULL;
    return newNode;
}

void ll_insert(listNode*& head, int data) {
    listNode* newNode = ll_createNode(data);
    newNode->next = head;
    head = newNode;
}

void find(listNode* head) {
    while (head != NULL) {
        head = head->next;
    }
}

void insert(bst_Node*& root, int data) {
    if (root == NULL) {
        root = createNode(data);
        return;
    }
    if (data <= root->data) {
        insert(root->left, data);
    }
    else {
        insert(root->right, data);
    }
}

void find(bst_Node* root) {
    if (root == NULL) {
        return;
    }
    find(root->left);
    find(root->right);
}




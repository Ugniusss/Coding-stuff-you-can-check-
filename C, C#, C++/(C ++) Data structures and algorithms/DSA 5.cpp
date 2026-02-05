#include <iostream>
#include <cstdlib>
#include <ctime>
#include <chrono>
using namespace std;

//------------------STRUCT
struct Node {
    int data;
    Node* left;
    Node* right;
};
//------------------STRUCT

//------------------PROTOTYPE
Node* newNode(int data);
Node* insert(Node* node, int data);
int height(Node* node);
int getBalance(Node* node);
Node* leftRotate(Node* node);
Node* rightRotate(Node* node);
Node* balance(Node* node);
Node* buildAVLTree(int arr[], int start, int end);
Node* buildBST(int arr[], int start, int end);
void inorderTraversal(Node* node);
string searchAVL(Node* root, int target);
string searchBST(Node* root, int target);
//------------------PROTOTYPE

int main() {
    const int SIZE = 100000;
    int arr[SIZE];
    // srand(time(NULL));
    for (int i = 0; i < SIZE; i++) {
        arr[i] = rand() % 1000000 + 1;
        //cout << arr[i] << endl;
    }
    Node* avlRoot = buildAVLTree(arr, 0, SIZE - 1);
    Node* bstRoot = buildBST(arr, 0, SIZE - 1);
    // AVL
    cout << "AVL tree: ";
    //inorderTraversal(avlRoot);
    cout << endl;
    auto start = chrono::high_resolution_clock::now();
    cout << "Skaicius: " << 42 << searchAVL(avlRoot, 42) << endl;
    auto end = chrono::high_resolution_clock::now();
    auto duration = std::chrono::duration_cast<std::chrono::microseconds>(end - start).count();
    cout << "Ieskojo: " << duration << " ms" << endl;
    cout << endl << endl;

    // BIN
    cout << "Binary search tree: ";
    //inorderTraversal(bstRoot);
    cout << endl;
    auto start1 = chrono::high_resolution_clock::now();
    cout << "Skaicius: " << arr[0] << searchBST(bstRoot, arr[0]) << endl;
    auto end1 = chrono::high_resolution_clock::now();
    auto duration1 = std::chrono::duration_cast<std::chrono::microseconds>(end1 - start1).count();
    cout << "Ieskojo: " << duration1 << " ms" << endl;
    return 0;
}


Node* newNode(int data) {
    Node* node = new Node;
    node->data = data;
    node->left = node->right = NULL;
    return node;
}

Node* insert(Node* node, int data) {
    if (node == NULL)
        return newNode(data);
    if (data < node->data)
        node->left = insert(node->left, data);
    else if (data > node->data)
        node->right = insert(node->right, data);
    return node;
}

int height(Node* node) {
    if (node == NULL)
        return 0;
    return max(height(node->left), height(node->right)) + 1;
}

int getBalance(Node* node) {
    if (node == NULL)
        return 0;
    return height(node->left) - height(node->right);
}

Node* leftRotate(Node* node) {
    Node* rightChild = node->right;
    Node* leftGrandchild = rightChild->left;
    rightChild->left = node;
    node->right = leftGrandchild;
    return rightChild;
}

Node* rightRotate(Node* node) {
    Node* leftChild = node->left;
    Node* rightGrandchild = leftChild->right;
    leftChild->right = node;
    node->left = rightGrandchild;
    return leftChild;
}

Node* balance(Node* node) {
    if (node == NULL)
        return node;
    int balanceFactor = getBalance(node);
    if (balanceFactor > 1) {
        if (getBalance(node->left) >= 0) {
            node = rightRotate(node);
        }
        else {
            node->left = leftRotate(node->left);
            node = rightRotate(node);
        }
    }
    else if (balanceFactor < -1) {
        if (getBalance(node->right) <= 0) {
            node = leftRotate(node);
        }
        else {
            node->right = rightRotate(node->right);
            node = leftRotate(node);
        }
    }
    return node;
}

Node* buildAVLTree(int arr[], int start, int end) {
    if (start > end)
        return NULL;
    int mid = (start + end) / 2;
    Node* root = newNode(arr[mid]);
    root->left = buildAVLTree(arr, start, mid - 1);
    root->right = buildAVLTree(arr, mid + 1, end);
    return balance(root);
}

Node* buildBST(int arr[], int start, int end) {
    if (start > end)
        return NULL;
    int mid = (start + end) / 2;
    Node* root = newNode(arr[mid]);
    root->left = buildBST(arr, start, mid - 1);
    root->right = buildBST(arr, mid + 1, end);
    return root;
}

void inorderTraversal(Node* node) {
    if (node == NULL)
        return;
    inorderTraversal(node->left);
    cout << node->data << " ";
    inorderTraversal(node->right);
}

string searchAVL(Node* root, int target) {
    if (root == nullptr) {
        return " Nera";
    }
    if (root->data == target) {
        return " Yra";
    }
    if (root->data < target) {
        return searchAVL(root->right, target);
    }
    else {
        return searchAVL(root->left, target);
    }
}

string searchBST(Node* root, int target) {
    if (root == nullptr) {
        return " Nera";
    }
    if (root->data == target) {
        return " Yra";
    }
    if (root->data < target) {
        return searchBST(root->right, target);
    }
    else {
        return searchBST(root->left, target);
    }
}






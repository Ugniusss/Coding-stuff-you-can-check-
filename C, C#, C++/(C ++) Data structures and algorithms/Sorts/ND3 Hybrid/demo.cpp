#include <iostream>
#include <vector>
#include <random>
#include <chrono>
#include "headr.h"
#include "functions.cpp"

template <typename T>
void printVector(const std::vector<T>& vec) {
    for (const auto& item : vec) {
        std::cout << item << " ";
    }
    std::cout << std::endl;
}

int main() {
    std::vector<int> numbers;
    const int count = 1000;

    std::random_device rd;
    std::mt19937 gen(rd());
    std::uniform_int_distribution<int> dis(1, 1000);

    for (int i = 0; i < count; ++i) {
        numbers.push_back(dis(gen));
    }

    std::cout << "Unsorted numbers: ";
    printVector(numbers);


    Arr<int> arr(count);
    for (const auto& num : numbers) {
        arr.add(num);
    }

    auto startTime = std::chrono::steady_clock::now();
    Hybrid(arr, 0, arr.size() - 1);
    auto endTime = std::chrono::steady_clock::now();

    std::cout << "Sorted numbers: ";
    for (int i = 0; i < arr.size(); ++i) {
        std::cout << arr[i] << " ";
    }
    std::cout << std::endl;
    auto elapsedTime = std::chrono::duration_cast<std::chrono::milliseconds>(endTime - startTime).count();

    std::cout << "Time taken: " << elapsedTime << " milliseconds" << std::endl;

    return 0;
}
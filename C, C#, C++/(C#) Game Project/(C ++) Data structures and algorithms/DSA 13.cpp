// Visual Studio
// Ugnius Padolskis

#include <iostream>
#include <vector>
#include <string>
#include <algorithm>

using namespace std;

int levenshteinDistance(const string& word1, const string& word2) {
    int len1 = word1.length();
    int len2 = word2.length();
    vector<vector<int>> DP(len1 + 1, vector<int>(len2 + 1, 0));
    for (int i = 0; i <= len1; i++) {
        for (int j = 0; j <= len2; j++) {
            if (i == 0) {
                DP[i][j] = j;
            }
            else if (j == 0) {
                DP[i][j] = i;
            }
            else if (word1[i - 1] == word2[j - 1]) {
                DP[i][j] = DP[i - 1][j - 1];
            }
            else {
                DP[i][j] = 1 + min({ DP[i - 1][j], DP[i][j - 1], DP[i - 1][j - 1] });
            }
        }
    }
    return DP[len1][len2];
}

int main() {
    string word1 = "laime";
    string word2 = "kelme";
    int distance = levenshteinDistance(word1, word2);
    cout << "ilgis- " << word1 << ", " << word2 << ": " << distance << endl;
    return 0;
}
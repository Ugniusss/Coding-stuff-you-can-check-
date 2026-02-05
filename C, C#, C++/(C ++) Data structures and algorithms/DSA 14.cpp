#include <iostream>
#include <vector>
#include <queue>
#include <cmath>
#include <limits>

using namespace std;

// Structure to represent a city
struct City {
    int x;
    int y;
};

// Structure to represent an edge between two cities
struct Edge {
    int destination;
    double weight;
};

// Function to calculate the Euclidean distance between two cities
double calculateDistance(const City& city1, const City& city2) {
    int dx = city2.x - city1.x;
    int dy = city2.y - city1.y;
    return sqrt(dx * dx + dy * dy);
}

// Dijkstra algorithm to find the shortest path
vector<double> dijkstra(const vector<vector<Edge>>& graph, int source) {
    int numCities = graph.size();

    vector<double> distance(numCities, numeric_limits<double>::max());
    distance[source] = 0.0;

    priority_queue<pair<double, int>, vector<pair<double, int>>, greater<pair<double, int>>> pq;
    pq.push(make_pair(0.0, source));

    while (!pq.empty()) {
        int current = pq.top().second;
        double currentDist = pq.top().first;
        pq.pop();

        if (currentDist > distance[current])
            continue;

        for (const Edge& edge : graph[current]) {
            int next = edge.destination;
            double weight = edge.weight;

            double newDist = currentDist + weight;
            if (newDist < distance[next]) {
                distance[next] = newDist;
                pq.push(make_pair(newDist, next));
            }
        }
    }

    return distance;
}

// A* algorithm to find the shortest path with a heuristic function
vector<double> aStar(const vector<vector<Edge>>& graph, const vector<City>& cities, int source, int target) {
    int numCities = graph.size();

    vector<double> distance(numCities, numeric_limits<double>::max());
    distance[source] = 0.0;

    priority_queue<pair<double, int>, vector<pair<double, int>>, greater<pair<double, int>>> pq;
    pq.push(make_pair(0.0, source));

    while (!pq.empty()) {
        int current = pq.top().second;
        double currentDist = pq.top().first;
        pq.pop();

        if (current == target)
            break;

        if (currentDist > distance[current])
            continue;

        for (const Edge& edge : graph[current]) {
            int next = edge.destination;
            double weight = edge.weight;

            double newDist = currentDist + weight;
            if (newDist < distance[next]) {
                distance[next] = newDist;

                // Calculate the heuristic based on different methods
                double heuristic = calculateDistance(cities[next], cities[target]);
                // Uncomment one of the following lines to choose the heuristic method:
                // double heuristic = abs(cities[next].x - cities[target].x); // Manhattan distance (difference in x-coordinates)
                // double heuristic = abs(cities[next].y - cities[target].y); // Manhattan distance (difference in y-coordinates)
                // double heuristic = calculateDistance(cities[next], cities[target]); // Euclidean distance

                pq.push(make_pair(newDist + heuristic, next));
            }
        }
    }

    return distance;
}

int main() {
    int numCities = 5;

    // Generate random cities
    vector<City> cities(numCities);
    for (int i = 0; i < numCities; i++) {
        cities[i].x = rand() % 100;
        cities[i].y = rand() % 100;
    }

    // Generate random graph with edges
    vector<vector<Edge>> graph(numCities);
    for (int i = 0; i < numCities; i++) {
        for (int j = i + 1; j < numCities; j++) {
            double distance = calculateDistance(cities[i], cities[j]);
            graph[i].push_back({ j, distance });
            graph[j].push_back({ i, distance });
        }
    }

    int source = 0;
    int target = numCities - 1;

    // Dijkstra algorithm
    vector<double> dijkstraDistances = dijkstra(graph, source);

    cout << "Dijkstra algorithm:" << endl;
    cout << "Shortest distance from city " << source << " to city " << target << ": " << dijkstraDistances[target] << endl;

    // A* algorithm with different heuristic methods
    vector<double> aStarDistances;

    cout << endl << "A* algorithm with different heuristic methods:" << endl;

    // Heuristic based on the difference in x-coordinates
    aStarDistances = aStar(graph, cities, source, target);
    cout << "Heuristic based on x-coordinates difference:" << endl;
    cout << "Shortest distance from city " << source << " to city " << target << ": " << aStarDistances[target] << endl;

    // Heuristic based on the difference in y-coordinates
    aStarDistances = aStar(graph, cities, source, target);
    cout << "Heuristic based on y-coordinates difference:" << endl;
    cout << "Shortest distance from city " << source << " to city " << target << ": " << aStarDistances[target] << endl;

    // Heuristic based on the Euclidean distance
    aStarDistances = aStar(graph, cities, source, target);
    cout << "Heuristic based on Euclidean distance:" << endl;
    cout << "Shortest distance from city " << source << " to city " << target << ": " << aStarDistances[target] << endl;

    return 0;
}

int graph[V][V] = {    { 0,1,2,3,4,5,6,7 },
                       { 4, 0, 8, 0, 0, 0, 0, 11, 0 },
                       { 0, 8, 0, 7, 0, 4, 0, 0, 2 },
                       { 0, 0, 7, 0, 9, 14, 0, 0, 0 },
                       { 0, 0, 0, 9, 0, 10, 0, 0, 0 },
                       { 0, 0, 4, 14, 10, 0, 2, 0, 0 },
                       { 0, 0, 0, 0, 0, 2, 0, 1, 6 },
                       { 8, 11, 0, 0, 0, 0, 1, 0, 7 },
                       { 0, 0, 2, 0, 0, 0, 6, 7, 0 } };

dijkstra(graph, 0);
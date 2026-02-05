const express = require("express");
const bodyParser = require("body-parser");
const neo4j = require("neo4j-driver");

const app = express();
app.use(bodyParser.json());

// Neo4j Connection
const driver = neo4j.driver(
    "bolt://localhost:7687",
    neo4j.auth.basic("neo4j", "password") // Replace with your Neo4j password
);
const session = driver.session();

// Helper function to clean up session
const cleanupSession = async () => await session.close();

// Routes

// Register a new city
app.put("/cities", async (req, res) => {
    const { name, country } = req.body;

    if (!name || !country) {
        return res.status(400).send("Missing city name or country.");
    }

    try {
        await session.run(
            "MERGE (c:City {name: $name, country: $country})",
            { name, country }
        );
        res.status(201).send();
    } catch (error) {
        res.status(400).send("Couldn't register city.");
    }
});

// Get all cities
app.get("/cities", async (req, res) => {
    try {
        const result = await session.run("MATCH (c:City) RETURN c");
        const cities = result.records.map((record) => record.get("c").properties);
        res.json(cities);
    } catch (error) {
        res.status(400).send("Couldn't retrieve cities.");
    }
});

// Get a single city
app.get("/cities/:name", async (req, res) => {
    const { name } = req.params;

    try {
        const result = await session.run(
            "MATCH (c:City {name: $name}) RETURN c",
            { name }
        );

        if (result.records.length === 0) {
            return res.status(404).send("City not found.");
        }

        const city = result.records[0].get("c").properties;
        res.json(city);
    } catch (error) {
        res.status(400).send("Couldn't retrieve city.");
    }
});

// Register an airport
app.put("/cities/:name/airports", async (req, res) => {
    const cityName = req.params.name;
    const { code, name, numberOfTerminals, address } = req.body;

    if (!code || !name || !numberOfTerminals || !address) {
        return res.status(400).send("Missing airport details.");
    }

    try {
        const cityResult = await session.run(
            "MATCH (c:City {name: $cityName}) RETURN c",
            { cityName }
        );

        if (cityResult.records.length === 0) {
            return res.status(404).send("City not found.");
        }

        await session.run(
            `
        MERGE (a:Airport {code: $code, name: $name, numberOfTerminals: $numberOfTerminals, address: $address})
        MERGE (c:City {name: $cityName})-[:HAS_AIRPORT]->(a)
      `,
            { code, name, numberOfTerminals, address, cityName }
        );

        res.status(201).send("Airport registered successfully.");
    } catch (error) {
        res.status(400).send("Couldn't register airport.");
    }
});

// Get airports in a city
app.get("/cities/:name/airports", async (req, res) => {
    const { name } = req.params;

    try {
        const result = await session.run(
            `
        MATCH (:City {name: $name})-[:HAS_AIRPORT]->(a:Airport)
        RETURN a
      `,
            { name }
        );

        const airports = result.records.map((record) => record.get("a").properties);
        res.json(airports);
    } catch (error) {
        res.status(400).send("Couldn't retrieve airports.");
    }
});

// Get a single airport
app.get("/airports/:code", async (req, res) => {
    const { code } = req.params;

    try {
        const result = await session.run(
            `
        MATCH (a:Airport {code: $code})<-[:HAS_AIRPORT]-(c:City)
        RETURN a, c.name AS city
      `,
            { code }
        );

        if (result.records.length === 0) {
            return res.status(404).send("Airport not found.");
        }

        const airport = result.records[0].get("a").properties;
        const city = result.records[0].get("city");
        res.json({ ...airport, city });
    } catch (error) {
        res.status(400).send("Couldn't retrieve airport.");
    }
});

// Register a new flight
app.put("/flights", async (req, res) => {
    const { number, fromAirport, toAirport, price, flightTimeInMinutes, operator } = req.body;

    if (!number || !fromAirport || !toAirport || !price || !flightTimeInMinutes || !operator) {
        return res.status(400).send("Missing flight details.");
    }

    try {
        // Check if both airports exist
        const airportCheck = await session.run(
            `
            MATCH (from:Airport {code: $fromAirport}), (to:Airport {code: $toAirport})
            RETURN from, to
            `,
            { fromAirport, toAirport }
        );

        if (airportCheck.records.length === 0) {
            return res.status(404).send("One or both airports do not exist.");
        }

        // Register the flight (directional)
        await session.run(
            `
            MATCH (from:Airport {code: $fromAirport}), (to:Airport {code: $toAirport})
            MERGE (from)-[:FLIES_TO {number: $number, price: $price, flightTimeInMinutes: $flightTimeInMinutes, operator: $operator}]->(to)
            `,
            { number, fromAirport, toAirport, price, flightTimeInMinutes, operator }
        );

        res.status(201).send("Flight registered successfully.");
    } catch (error) {
        console.error(error);
        res.status(400).send("Couldn't register flight.");
    }
});

// Get full flight information
app.get("/flights/:number", async (req, res) => {
    const { number } = req.params;

    try {
        const result = await session.run(
            `
        MATCH (from:Airport)-[f:FLIES_TO {number: $number}]->(to:Airport)
        RETURN f, from, to
      `,
            { number }
        );

        if (result.records.length === 0) {
            return res.status(404).send("Flight not found.");
        }

        const flight = result.records[0].get("f").properties;
        const fromAirport = result.records[0].get("from").properties;
        const toAirport = result.records[0].get("to").properties;

        res.json({
            ...flight,
            fromAirport: fromAirport.code,
            fromCity: fromAirport.city,
            toAirport: toAirport.code,
            toCity: toAirport.city,
        });
    } catch (error) {
        res.status(400).send("Couldn't retrieve flight information.");
    }
});

// Search for flights between two cities (up to 3 stops)
app.get("/search/flights/:fromCity/:toCity", async (req, res) => {
    const { fromCity, toCity } = req.params;

    try {
        // Query to find flights with up to 3 stops
        const result = await session.run(
            `
MATCH (start:City {name: $fromCity})-[:HAS_AIRPORT]->(a1:Airport)
MATCH (end:City {name: $toCity})-[:HAS_AIRPORT]->(a4:Airport)

// Direct flights
OPTIONAL MATCH (a1)-[f1:FLIES_TO]->(a4)
WITH start, end, a1, a4, collect([f1]) AS directFlights

// 1-stop flights
OPTIONAL MATCH (a1)-[f1:FLIES_TO]->(a2:Airport)-[f2:FLIES_TO]->(a4)
WITH start, end, a1, a4, directFlights, collect([f1, f2]) AS oneStopFlights

// 2-stop flights
OPTIONAL MATCH (a1)-[f1:FLIES_TO]->(a2:Airport)-[f2:FLIES_TO]->(a3:Airport)-[f3:FLIES_TO]->(a4)
WITH start, end, a1, a4, directFlights, oneStopFlights, collect([f1, f2, f3]) AS twoStopFlights

// Combine all flights
WITH directFlights + oneStopFlights + twoStopFlights AS allFlights, a1, a4

// Filter only valid flight paths (no nulls)
UNWIND allFlights AS flightPath
WITH a1, a4, flightPath WHERE ALL(flight IN flightPath WHERE flight IS NOT NULL)

// Calculate total price and time
WITH a1, a4, flightPath, 
     reduce(price = 0, f IN flightPath | price + f.price) AS totalPrice,
     reduce(time = 0, f IN flightPath | time + f.flightTimeInMinutes) AS totalTime

RETURN 
    a1.code AS fromAirport, 
    a4.code AS toAirport, 
    [f IN flightPath | f.number] AS flights,
    totalPrice AS price,
    totalTime AS timeInMinutes
            `,
            { fromCity, toCity }
        );

        // Format response
        const flights = result.records.map((record) => ({
            fromAirport: record.get("fromAirport"),
            toAirport: record.get("toAirport"),
            flights: record.get("flights"),
            price: record.get("price"),
            timeInMinutes: record.get("timeInMinutes"),
        }));

        res.json(flights);
    } catch (error) {
        console.error(error);
        res.status(400).send("Couldn't search for flights.");
    }
});


// Cleanup (delete all data)
app.post("/cleanup", async (req, res) => {
    try {
        await session.run("MATCH (n) DETACH DELETE n");
        res.send("Database cleaned up successfully.");
    } catch (error) {
        res.status(400).send("Couldn't cleanup database.");
    }
});

// Start the server
const PORT = 4000;
app.listen(PORT, () => console.log(`Server running on http://localhost:${PORT}`));

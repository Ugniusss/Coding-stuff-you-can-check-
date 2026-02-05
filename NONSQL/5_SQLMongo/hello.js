const express = require("express");
const bodyParser = require("body-parser");
const mongoose = require("mongoose");
const redis = require("redis");

// Express and middleware
const app = express();
app.use(bodyParser.json());

// MongoDB connection
mongoose.connect("mongodb://localhost:27017", {});

const db = mongoose.connection;
db.on("error", console.error.bind(console, "MongoDB connection error:"));

// Redis connection
const redisClient = redis.createClient({ host: "localhost", port: 6379 });
redisClient.on("error", (err) => console.log("Redis Error:", err));


const StationSchema = new mongoose.Schema({
    name: { type: String, required: true },
}, { versionKey: false });  // Disables the __v field);
const Station = mongoose.model("Station", StationSchema);

const RouteSchema = new mongoose.Schema({
    number: String,
    startStation: String,
    endStation: String,
    distance: Number,
    time: String,
});
const Route = mongoose.model("Route", RouteSchema);

const TicketSchema = new mongoose.Schema({
    ticketNumber: String,
    startStation: String,
    endStation: String,
    price: Number,
    passengerName: String,
    routeNumber: String,
});
const Ticket = mongoose.model("Ticket", TicketSchema);

const generateRandomLetters = (length) => {
    return Array.from({ length }, () => String.fromCharCode(97 + Math.floor(Math.random() * 26))).join("");
};

const generateRandomDigits = (length) => {
    return Array.from({ length }, () => Math.floor(Math.random() * 10)).join("");
};

app.put("/stations", async (req, res) => {
    const { name } = req.body;

    if (!name) {
        return res.status(400).json({ error: "Station name is required" });
    }

    try {
        let station = await Station.findOne({ name });
        if (!station) {
            station = new Station({ name });
            await station.save();
        }
        res.status(200).json(station);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

app.get("/stations", async (req, res) => {
    try {
        const stations = await Station.find().select("-_id");
        res.status(200).json(stations);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

app.put("/routes", async (req, res) => {
    const { startStation, endStation, distance, time } = req.body;

    if (!startStation || !endStation || !distance || !time) {
        return res.status(400).json({ error: "All fields are required" });
    }

    try {
        const randomLetters = generateRandomLetters(4);
        const randomDigits = generateRandomDigits(4);
        const routeNumber = `${randomLetters}${randomDigits}`;

        const route = await Route.findOne({ startStation, endStation });
        if (!route) {
            const newRoute = new Route({
                number: routeNumber,
                startStation,
                endStation,
                distance,
                time,
            });
            await newRoute.save();
            res.status(201).json(newRoute);
        } else {
            route.startStation = startStation;
            route.endStation = endStation;
            route.distance = distance;
            route.time = time;
            await route.save();
            res.status(200).json(route);
        }
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

app.get("/routes", async (req, res) => {
    try {
        const routes = await Route.find().select("-_id -__v");
        res.status(200).json(routes);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

const findRoutePath = async (startStation, endStation) => {
    const graph = {};
    const routes = await Route.find();

    routes.forEach(route => {
        if (!graph[route.startStation]) graph[route.startStation] = [];
        if (!graph[route.endStation]) graph[route.endStation] = [];
        graph[route.startStation].push({ station: route.endStation, distance: route.distance, routeNumber: route.number });
    });

    const queue = [{ station: startStation, path: [], routeNumbers: [], totalDistance: 0 }];
    const visited = new Set();

    while (queue.length > 0) {
        const current = queue.shift();
        const { station, path, routeNumbers, totalDistance } = current;

        if (visited.has(station)) continue;
        visited.add(station);

        const newPath = [...path, station];
        const newRouteNumbers = [...routeNumbers];

        if (station === endStation) {
            return { path: newPath, routeNumbers: newRouteNumbers, totalDistance };
        }

        for (const neighbor of (graph[station] || [])) {
            if (!visited.has(neighbor.station)) {
                queue.push({
                    station: neighbor.station,
                    path: newPath,
                    routeNumbers: [...newRouteNumbers, neighbor.routeNumber],
                    totalDistance: totalDistance + neighbor.distance,
                });
            }
        }
    }

    return null;
};


app.put("/tickets", async (req, res) => {
    const { startStation, endStation, passengerName } = req.body;

    if (!startStation || !endStation || !passengerName) {
        return res.status(400).json({ error: "Start station, end station, and passenger name are required" });
    }

    try {
        const startStationExists = await Station.findOne({ name: startStation });
        const endStationExists = await Station.findOne({ name: endStation });
        if (!startStationExists || !endStationExists) {
            return res.status(404).json({ error: "Start or End station not found" });
        }

        const routePath = await findRoutePath(startStation, endStation);

        if (!routePath) {
            return res.status(404).json({ error: "No route found between the provided stations" });
        }

        const { path, routeNumbers, totalDistance } = routePath;

        const price = (0.14 * totalDistance + 0.3).toFixed(2);

        const randomLetters = generateRandomLetters(4);
        const randomDigits = generateRandomDigits(4);
        const ticketNumber = `${randomDigits}${randomLetters}`;

        const ticket = new Ticket({
            ticketNumber,
            startStation,
            endStation,
            price,
            passengerName,
        });

        await ticket.save();

        res.status(201).json({
            ticket: {
                ticketNumber,
                startStation,
                endStation,
                price,
                passengerName,
                route: path.join("->"),
                routes: routeNumbers,
            },
        });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});


/*
app.get("/tickets", async (req, res) => {
    try {
        const tickets = await Ticket.find().select("-_id -__v");
        res.json(tickets);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});
*/
app.get("/tickets", async (req, res) => {
    try {
        const tickets = await Ticket.find().select("-_id -__v");

        const enrichedTickets = await Promise.all(
            tickets.map(async (ticket) => {
                const routePath = await findRoutePath(ticket.startStation, ticket.endStation);
                if (routePath && routePath.path && routePath.routeNumbers) {
                    const { path, routeNumbers } = routePath;
                    return {
                        ...ticket.toObject(),
                        route: path.join("->"),
                        routes: routeNumbers,
                    };
                }
                return ticket.toObject();
            })
        );

        res.status(200).json(enrichedTickets);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

app.get("/tickets/:ticketNumber", async (req, res) => {
    try {
        const ticket = await Ticket.findOne({ ticketNumber: req.params.ticketNumber }).select("-_id -__v");
        if (!ticket) return res.status(404).json({ error: "Ticket not found" });

        const routePath = await findRoutePath(ticket.startStation, ticket.endStation);
        if (routePath && routePath.path && routePath.routeNumbers) {
            const { path, routeNumbers } = routePath;
            return res.status(200).json({
                ...ticket.toObject(),
                route: path.join("->"),
                routes: routeNumbers,
            });
        }

        return res.status(200).json(ticket.toObject());
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});





app.post("/cleanup", async (req, res) => {
    try {
        await Station.deleteMany({});
        await Route.deleteMany({});
        await Ticket.deleteMany({});

        res.status(200).json({ message: "All data has been cleaned up successfully!" });
    } catch (error) {
        res.status(500).json({ error: "Error occurred during cleanup: " + error.message });
    }
});


// Start the server
const PORT = 3000;
app.listen(PORT, () => console.log(`Server running on port ${PORT}`));

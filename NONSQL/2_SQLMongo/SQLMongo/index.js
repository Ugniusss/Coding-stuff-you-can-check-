/*
* {
  "id": "1",
  "name": "Banana",
  "category": "Fruit",
  "price": 1.99
}

{
  "id": "2",
  "name": "Chocolate",
  "category": "Sweet",
  "price": 5.00
}

{
  "id": "3",
  "name": "Potato",
  "category": "Vegetable",
  "price": 0.50
}
*
* */


const express = require('express');
const { MongoClient } = require('mongodb');
const bodyParser = require('body-parser');

const app = express();
app.use(bodyParser.json());

// MongoDB Connection Setup
const mongoUrl = 'mongodb://localhost:27017'; // Adjust the URL as needed
const dbName = 'clientDB'; // Name of your database
let db, clientCollection;

MongoClient.connect(mongoUrl, { useNewUrlParser: true, useUnifiedTopology: true })
    .then((client) => {
      db = client.db(dbName);
      clientCollection = db.collection('clients');
      productCollection = db.collection('products');
      orderCollection = db.collection('orders'); // Initialize orderCollection
      console.log('Connected to MongoDB');
    })
    .catch((error) => console.error(error));


// Define a regular expression for email validation
const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

// Start the server
app.listen(8080, () => {
  console.log('Server running on port 8080');
});

// Register a new client
app.put('/clients', async function (req, res) {
  const { id, name, email } = req.body;

  // Validate the input
  if (!id || !name || !email || !email.match(emailRegex)) {
    res.status(400).send({ message: 'Invalid input' });
    return;
  }

  try {
    // Check if a client with the same ID already exists
    const existingClient = await clientCollection.findOne({ id });
    if (existingClient) {
      res.status(400).send({ message: 'Client with this ID already exists' });
      return;
    }

    // Insert the new client into the collection
    await clientCollection.insertOne({ id, name, email });
    res.status(201).send({ id });
  } catch (err) {
    res.status(500).send({ message: 'Error registering client', error: err });
  }
});

// Get client details
app.get('/clients/:clientId', async function (req, res) {
  const clientId = req.params.clientId;

  try {
    const client = await clientCollection.findOne({ id: clientId });
    if (client) {
      // Exclude the _id field from the response
      const { _id, ...clientData } = client;
      res.status(200).send(clientData); // Send only the client data without _id
    } else {
      res.status(404).send({ message: 'Client not found' });
    }
  } catch (err) {
    res.status(500).send({ message: 'Error retrieving client', error: err });
  }
});

// Delete a client and its associated orders (assuming "orders" is part of the client document)
app.delete('/clients/:clientId', async function (req, res) {
  const clientId = req.params.clientId;

  try {
    const result = await clientCollection.deleteOne({ id: clientId });
    if (result.deletedCount === 1) {
      res.status(204).send(); // Client deleted
    } else {
      res.status(404).send({ message: 'Client not found' });
    }
  } catch (err) {
    res.status(500).send({ message: 'Error deleting client', error: err });
  }
});

// Register a new product
app.put('/products', async function (req, res) {
  const { id, name, category, price } = req.body;

  // Validate the input
  if (!id || !name || !category || price == null || isNaN(price)) {
    res.status(400).send({ message: 'Invalid input, missing id, name, category, or price' });
    return;
  }

  try {
    // Check if a product with the same ID already exists
    const existingProduct = await productCollection.findOne({ id });
    if (existingProduct) {
      res.status(400).send({ message: 'Product with this ID already exists' });
      return;
    }

    // Insert the new product into the collection
    await productCollection.insertOne({ id, name, category, price });
    res.status(201).send({ id });
  } catch (err) {
    res.status(500).send({ message: 'Error registering product', error: err });
  }
});

// List all products
app.get('/products', async function (req, res) {
  try {
    const products = await productCollection.find({}, { projection: { _id: 0 } }).toArray();
    res.status(200).send(products);
  } catch (err) {
    res.status(500).send({ message: 'Error fetching products', error: err });
  }
});

// Get product details by ID
app.get('/products/:productId', async function (req, res) {
  const productId = req.params.productId;

  try {
    const product = await productCollection.findOne({ id: productId }, { projection: { _id: 0 }});
    if (product) {
      res.status(200).send(product);
    } else {
      res.status(404).send({ message: 'Product not found' });
    }
  } catch (err) {
    res.status(500).send({ message: 'Error fetching product', error: err });
  }
});

// Delete a product
app.delete('/products/:productId', async function (req, res) {
  const productId = req.params.productId;

  try {
    const result = await productCollection.deleteOne({ id: productId });
    if (result.deletedCount === 1) {
      res.status(204).send(); // Product deleted
    } else {
      res.status(404).send({ message: 'Product not found' });
    }
  } catch (err) {
    res.status(500).send({ message: 'Error deleting product', error: err });
  }
});

app.put('/orders', async function (req, res) {
  const { clientId, items } = req.body;

  // Validate the input
  if (!clientId || !Array.isArray(items) || items.length === 0) {
    return res.status(400).send({ message: 'Invalid input, missing clientId or items' });
  }

  try {
    // Check if the client exists
    const client = await clientCollection.findOne({ id: clientId });
    if (!client) {
      return res.status(404).send({ message: 'Client not found' });
    }

    // Validate product IDs in the items array
    const productIds = items.map(item => item.productId); // Extract product IDs from items
    const products = await productCollection.find({ id: { $in: productIds } }).toArray(); // Find matching products

    // Check if all product IDs are valid
    const validProductIds = products.map(product => product.id); // Array of valid product IDs
    const invalidProductIds = productIds.filter(id => !validProductIds.includes(id)); // Find invalid product IDs

    if (invalidProductIds.length > 0) {
      return res.status(404).send({ message: 'One or more products not found', invalidProductIds });
    }

    // Generate a unique order ID (you can customize this)
    const orderId = new Date().getTime().toString(); // A simple timestamp-based order ID

    // Insert the new order into the collection
    await orderCollection.insertOne({
      id: orderId,
      clientId: clientId,
      items: items,
      orderDate: new Date() // Add the date of the order
    });

    // Return the order ID in the response
    res.status(201).send({ id: orderId });
  } catch (err) {
    console.error('Error registering order:', err);  // Log the actual error
    res.status(500).send({ message: 'Error registering order', error: err.message || err });
  }
});

// Get all orders for a specific client
app.get('/clients/:clientId/orders', async function (req, res) {
  const clientId = req.params.clientId;

  try {
    // Check if the client exists
    const client = await clientCollection.findOne({ id: clientId });
    if (!client) {
      return res.status(404).send({ message: 'Client not found' });
    }

    // Fetch all orders for the specified clientId
    const orders = await orderCollection.find({ clientId: clientId }, { projection: { _id: 0, id: 1, items: 1 } }).toArray();

    // Check if the client has any orders
    if (orders.length === 0) {
      return res.status(404).send({ message: 'No orders found for this client' });
    }

    // Return the orders for the client
    res.status(200).send(orders);
  } catch (err) {
    console.error('Error fetching client orders:', err);  // Log the actual error
    res.status(500).send({ message: 'Error fetching client orders', error: err.message || err });
  }
});

app.get('/statistics/top/clients', async function (req, res) {
  try {
    // Aggregate top 10 clients by order count
    const topClients = await orderCollection.aggregate([
      { $group: { _id: "$clientId", totalOrders: { $sum: 1 } } }, // Group by clientId and count orders
      { $sort: { totalOrders: -1 } }, // Sort by totalOrders in descending order
      { $limit: 10 }, // Limit to top 10
      { $project: { _id: 0, clientId: "$_id", totalOrders: 1 } } // Format the result
    ]).toArray();

    res.status(200).send(topClients);
  } catch (err) {
    console.error('Error fetching top clients:', err);
    res.status(500).send({ message: 'Error fetching top clients', error: err.message || err });
  }
});
app.get('/statistics/top/products', async function (req, res) {
  try {
    // Aggregate top 10 products by total quantity ordered
    const topProducts = await orderCollection.aggregate([
      { $unwind: "$items" }, // Flatten the items array to process each product individually
      { $group: { _id: "$items.productId", quantity: { $sum: "$items.quantity" } } }, // Group by productId and sum quantities
      { $sort: { quantity: -1 } }, // Sort by quantity in descending order
      { $limit: 10 }, // Limit to top 10
      { $project: { _id: 0, productId: "$_id", quantity: 1 } } // Format the result
    ]).toArray();

    res.status(200).send(topProducts);
  } catch (err) {
    console.error('Error fetching top products:', err);
    res.status(500).send({ message: 'Error fetching top products', error: err.message || err });
  }
});
app.get('/statistics/orders/total', async function (req, res) {
  try {
    // Count the total number of orders
    const totalOrders = await orderCollection.countDocuments();

    res.status(200).send({ total: totalOrders });
  } catch (err) {
    console.error('Error fetching total orders:', err);
    res.status(500).send({ message: 'Error fetching total orders', error: err.message || err });
  }
});
app.get('/statistics/orders/totalValue', async function (req, res) {
  try {
    // Calculate the total value of all orders
    const totalValue = await orderCollection.aggregate([
      { $unwind: "$items" }, // Flatten items array
      { $lookup: { from: 'products', localField: 'items.productId', foreignField: 'id', as: 'productInfo' } }, // Join with products collection to get product prices
      { $unwind: "$productInfo" }, // Unwind the product info array
      { $group: { _id: null, totalValue: { $sum: { $multiply: ["$items.quantity", "$productInfo.price"] } } } }, // Calculate total value
      { $project: { _id: 0, totalValue: 1 } } // Format the result
    ]).toArray();

    res.status(200).send(totalValue[0] || { totalValue: 0 });
  } catch (err) {
    console.error('Error fetching total value of orders:', err);
    res.status(500).send({ message: 'Error fetching total value of orders', error: err.message || err });
  }
});
app.post('/cleanup', async function (req, res) {
  try {
    // Delete all data from clients, products, and orders collections
    await clientCollection.deleteMany({});
    await productCollection.deleteMany({});
    await orderCollection.deleteMany({});

    res.status(204).send(); // No content response indicating successful deletion
  } catch (err) {
    console.error('Error deleting data:', err);
    res.status(500).send({ message: 'Error deleting data', error: err.message || err });
  }
});

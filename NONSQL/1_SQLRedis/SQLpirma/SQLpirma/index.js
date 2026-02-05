const bodyParser = require('body-parser');
const express = require('express');
const { createClient } = require('redis');

const app = express();
const redisClient = createClient();

app.use(bodyParser.json('application/json'));

const idRegex = /^[a-zA-Z0-9]+$/;
const carLicenseNoRegex = /^[A-Z0-9]{1,7}$/;
const spotNoRegex = /^[0-9]+$/;

// Užregistruoti garažą
app.put('/garage', async function (req, res) {
  const { id, spots, address } = req.body;

  if (id && spots && address && id.match(idRegex)) {
    await redisClient.set(garageKey(id), JSON.stringify({ spots, address, occupied: 0 }));
    res.status(201).send();
  } else {
    res.status(400).send({ message: 'Blogai ivesti garazo duomenys' });
  }
});

// Gauti garažo informaciją.
app.get('/garage/:id', async function (req, res) {
  const id = req.params.id;

  if (id.match(idRegex)) {
    const garage = await redisClient.get(garageKey(id));
    if (garage != null) {
      res.status(200).send(JSON.parse(garage));
    } else {
      res.status(404).send(); // Garage not found
    }
  } else {
    res.status(400).send({ message: 'Blogas garazo ID.' });
  }
});

// Gauti garažo vietų skaičių
app.get('/garage/:id/configuration/spots', async function (req, res) {
  const id = req.params.id;

  if (id.match(idRegex)) {
    const garage = await redisClient.get(garageKey(id));
    if (garage != null) {
      const garageData = JSON.parse(garage);
      res.status(200).send({ spots: garageData.spots });
    } else {
      res.status(404).send({ message: 'Garazas tokiu ID nerastas.' });
    }
  } else {
    res.status(400).send({ message: 'Blogas garazo ID.' });
  }
});

// Pakeisti garažo vietų skaičių
app.post('/garage/:id/configuration/:spots', async function (req, res) {
  const id = req.params.id;
  const spots = parseInt(req.params.spots, 10);

  // checkina ID
  if (!id.match(idRegex)) {
    res.status(400).send({ message: 'Blogas garazo ID.' });
    return;
  }

  // checkina ar viet >0
  if (isNaN(spots) || spots <= 0) {
    res.status(400).send({ message: 'Vietu skaicius turi buti >0.' });
    return;
  }

  const garage = await redisClient.get(garageKey(id));
  if (garage != null) {
    const garageData = JSON.parse(garage);

    // updatina vietu nr
    garageData.spots = spots;
    await redisClient.set(garageKey(id), JSON.stringify(garageData));

    res.status(200).send({spots: garageData.spots });
  } else {
    res.status(404).send({ message: 'Garazas tokiu ID nerastas' });
  }
});

// Užregistruoti užimtą vietą garaže.
app.post('/garage/:id/spots/:spotNo', async function (req, res) {
  const id = req.params.id;
  const spotNo = parseInt(req.params.spotNo, 10);
  const { licenseNo } = req.body;

  if (!id.match(idRegex)) {
    res.status(400).send({ message: 'Blogas garazo ID.' });
    return;
  }

  if (isNaN(spotNo) || spotNo <= 0) {
    res.status(400).send({ message: 'Vietu skaicius turi buti >0.' });
    return;
  }

  if (!licenseNo || !licenseNo.match(carLicenseNoRegex)) {
    res.status(400).send({ message: 'Blogi numeriai.' });
    return;
  }

  const garage = await redisClient.get(garageKey(id));
  if (garage != null) {
    const garageData = JSON.parse(garage);

    if (spotNo > garageData.spots) {
      res.status(404).send({ message: '.' });
      return;
    }

    if (garageData.occupiedSpots && garageData.occupiedSpots[spotNo]) {
      res.status(400).send({ message: `Vieta ${spotNo} uzimta.` });
      return;
    }

    if (!garageData.occupiedSpots) {
      garageData.occupiedSpots = {};
    }
    garageData.occupiedSpots[spotNo] = licenseNo;

    await redisClient.set(garageKey(id), JSON.stringify(garageData));

    res.status(200).send({ licenseNo: licenseNo});
  } else {
    res.status(404).send({ message: 'Garazas tokiu ID nerastas' });
  }
});

// Pažymėti vietą kaip laisvą
app.delete('/garage/:id/spots/:spotNo', async function (req, res) {
  const id = req.params.id;
  const spotNo = parseInt(req.params.spotNo, 10);

  if (!id.match(idRegex)) {
    res.status(400).send({ message: 'Blogas garazo ID.' });
    return;
  }

  if (isNaN(spotNo) || spotNo <= 0) {
    res.status(400).send({ message: 'Blogas garazo ID.' });
    return;
  }

  const garage = await redisClient.get(garageKey(id));
  if (garage != null) {
    const garageData = JSON.parse(garage);

    if (spotNo > garageData.spots || !garageData.occupiedSpots || !garageData.occupiedSpots[spotNo]) {
      res.status(400).send({ message: 'Vietos nera.' });
      return;
    }

    delete garageData.occupiedSpots[spotNo];
    await redisClient.set(garageKey(id), JSON.stringify(garageData));

    res.status(200).send({ message: `Vieta ${spotNo} dabar lasiva.` });
  } else {
    res.status(404).send({ message: 'Garazas tokiu ID nerastas' });
  }
});

// Gauti laisvų ir užimtų vietų skaičių garaže
app.get('/garage/:id/status', async function (req, res) {
  const id = req.params.id;

  if (!id.match(idRegex)) {
    res.status(400).send({ message: 'Blogas garazo ID.' });
    return;
  }

  const garage = await redisClient.get(garageKey(id));
  if (garage != null) {
    const garageData = JSON.parse(garage);
    const occupiedSpots = garageData.occupiedSpots ? Object.keys(garageData.occupiedSpots).length : 0;
    const freeSpots = garageData.spots - occupiedSpots;

    res.status(200).send({ freeSpots, occupiedSpots });
  } else {
    res.status(404).send({ message: 'Garazas tokiu ID nerastas' });
  }
});

// Gauti automoblio numerį, kuris užima vietą
app.get('/garage/:id/spots/:spotNo', async function (req, res) {
  const id = req.params.id;
  const spotNo = parseInt(req.params.spotNo, 10);

  if (!id.match(idRegex)) {
    res.status(400).send({ message: 'Blogas garazo ID.' });
    return;
  }

  if (isNaN(spotNo) || spotNo <= 0) {
    res.status(400).send({ message: 'Blogas garazo ID.' });
    return;
  }

  const garage = await redisClient.get(garageKey(id));
  if (garage != null) {
    const garageData = JSON.parse(garage);

    if (spotNo > garageData.spots) {
      res.status(404).send({ message: 'Vieta nerasta.' });
      return;
    }

    if (garageData.occupiedSpots && garageData.occupiedSpots[spotNo]) {
      res.status(200).send({ licenseNo: garageData.occupiedSpots[spotNo] });
    } else {
      res.status(204).send(); // Spot is free
    }
  } else {
    res.status(404).send({ message: 'Garazas tokiu ID nerastas' });
  }
});

app.post('/flushall', async function (req, res) {
  try {
    await redisClient.flushAll();
    res.status(200).send({ message: 'Redis duomenų bazė išvalyta.' });
  } catch (err) {
    res.status(500).send({ message: 'Įvyko klaida išvalant duomenų bazę.', error: err });
  }
});
function garageKey(id){
  return `Garage:${id}`;
}
function respondWithInvalidGarageError(res) {
  res.status(400).send({ message: 'Blogas garazo ID.' });
}

// Connect
app.listen(8080, async () => {
  await redisClient.connect();
  console.log('STARTUOJAM.');
});

const express = require('express');
const { Client } = require('cassandra-driver');
const bodyParser = require('body-parser');
const { v4: uuidv4 } = require('uuid');
const { setup } = require('./setupCassandra');

const app = express();
app.use(bodyParser.json());

const cassandraClient = new Client({
    contactPoints: ['127.0.0.1'],
    localDataCenter: 'datacenter1',
    keyspace: 'video_service'
});

async function initialize() {
    await cassandraClient.connect();
}

(async () => {
    await setup();
    await initialize();

    // 1. Register a new channel
    app.put('/channels', async (req, res) => {
        const { name, owner } = req.body;
        if (!name || !owner) {
            return res.status(400).json({ message: 'Invalid input' });
        }

        const id = uuidv4();
        const query = 'INSERT INTO channels (id, name, owner) VALUES (?, ?, ?) IF NOT EXISTS';
        await cassandraClient.execute(query, [id, name, owner], { prepare: true });
        res.status(201).json({ id });
    });

    // 2. List all channels
    app.get('/channels', async (req, res) => {
        const query = 'SELECT id, name, owner FROM channels';
        const result = await cassandraClient.execute(query);
        res.status(200).json(result.rows);
    });

    // 3. Get channel by ID
    app.get('/channels/:channelId', async (req, res) => {
        const channelId = req.params.channelId;
        const query = 'SELECT id, name, owner FROM channels WHERE id = ?';
        const result = await cassandraClient.execute(query, [channelId], { prepare: true });

        if (result.rowLength === 0) return res.status(404).json({ message: 'Channel not found' });
        res.status(200).json(result.rows[0]);
    });

    // 4. Delete channel by ID
    app.delete('/channels/:channelId', async (req, res) => {
        const channelId = req.params.channelId;
        const query = 'DELETE FROM channels WHERE id = ?';
        await cassandraClient.execute(query, [channelId], { prepare: true });
        res.status(204).send();
    });

    // 5. Add video to a channel
    app.put('/channels/:channelId/videos', async (req, res) => {
        const channelId = req.params.channelId;
        const { title, description, duration } = req.body;
        if (!title || duration === undefined) return res.status(400).json({ message: 'Invalid input' });

        const videoId = uuidv4();
        const query = 'INSERT INTO videos_by_channel (channel_id, video_id, title, description, duration, views) VALUES (?, ?, ?, ?, ?, 0) IF NOT EXISTS';
        await cassandraClient.execute(query, [channelId, videoId, title, description, duration], { prepare: true });
        res.status(201).json({ id: videoId });
    });

    // 6. List videos in a channel
    app.get('/channels/:channelId/videos', async (req, res) => {
        const channelId = req.params.channelId;
        const query = 'SELECT video_id as id, title, description, duration FROM videos_by_channel WHERE channel_id = ?';
        const result = await cassandraClient.execute(query, [channelId], { prepare: true });

        if (result.rowLength === 0) return res.status(404).json({ message: 'Channel not found' });
        res.status(200).json(result.rows);
    });

    // 7. Get video by ID
    app.get('/channels/:channelId/videos/:videoId', async (req, res) => {
        const { channelId, videoId } = req.params;
        const query = 'SELECT video_id as id, title, description, duration FROM videos_by_channel WHERE channel_id = ? AND video_id = ?';
        const result = await cassandraClient.execute(query, [channelId, videoId], { prepare: true });

        if (result.rowLength === 0) return res.status(404).json({ message: 'Video or channel not found' });
        res.status(200).json(result.rows[0]);
    });

    // 8. Delete video by ID
    app.delete('/channels/:channelId/videos/:videoId', async (req, res) => {
        const { channelId, videoId } = req.params;
        const query = 'DELETE FROM videos_by_channel WHERE channel_id = ? AND video_id = ?';
        await cassandraClient.execute(query, [channelId, videoId], { prepare: true });
        res.status(204).send();
    });

    // 9. Get video views
    app.get('/channels/:channelId/videos/:videoId/views', async (req, res) => {
        const { videoId } = req.params;
        const query = 'SELECT views FROM video_views WHERE video_id = ?';
        const result = await cassandraClient.execute(query, [videoId], { prepare: true });

        if (result.rowLength === 0) return res.status(404).json({ message: 'Video not found' });
        res.status(200).json({ views: result.rows[0].views });
    });

    // 10. Register a view
    app.post('/channels/:channelId/videos/:videoId/views/register', async (req, res) => {
        const { videoId } = req.params;
        const query = 'UPDATE video_views SET views = views + 1 WHERE video_id = ?';
        await cassandraClient.execute(query, [videoId], { prepare: true });
        res.status(204).send();
    });

    // Start the server
    app.listen(8080, async () => {
        console.log('!!! ---SERVERIS VEIKIA--- !!!');
    });
})();

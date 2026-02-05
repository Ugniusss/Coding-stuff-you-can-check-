const cassandra = require('cassandra-driver');

// Connect to Cassandra
const client = new cassandra.Client({
    contactPoints: ['127.0.0.1'],
    localDataCenter: 'datacenter1'
});

async function setup() {
    try {
        await client.execute(`
            CREATE KEYSPACE IF NOT EXISTS video_service 
            WITH replication = {'class': 'SimpleStrategy', 'replication_factor': 1};
        `);

        await client.execute(`USE video_service;`);

        // Create channels table
        await client.execute(`
            CREATE TABLE IF NOT EXISTS channels (
                 id UUID PRIMARY KEY,
                 name TEXT,
                 owner TEXT
            );
        `);

        await client.execute(`
            CREATE TABLE IF NOT EXISTS videos_by_channel (
                   channel_id UUID,
                   video_id UUID,
                   title TEXT,
                   description TEXT,
                   duration INT,
                   PRIMARY KEY (channel_id, video_id)
                );
        `);

        const result = await client.execute(`SELECT column_name FROM system_schema.columns WHERE keyspace_name='video_service' AND table_name='videos_by_channel' AND column_name='views';`);

        if (result.rowLength === 0) {
            await client.execute(`
                ALTER TABLE videos_by_channel ADD views INT;
            `);
        }

        await client.execute(`
            CREATE TABLE IF NOT EXISTS video_views (
                                                       video_id UUID PRIMARY KEY,
                                                       views COUNTER
            );
        `);

        console.log('!!! ---LENTELES SUKURTOS--- !!!');
    } catch (error) {
        console.error('Error setting up Cassandra:', error);
    } finally {
        await client.shutdown();
    }
}

module.exports = { setup };

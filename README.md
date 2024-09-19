1. **Build the Docker images:**
   ```bash
   docker-compose build
   ```

2. **Start the containers:**
   ```bash
   docker-compose up -d
   ```

3. **Access the Kafka broker:**
   * **Host:** `localhost`
   * **Port:** `9092`

### Stopping the Kafka Cluster

```bash
docker-compose down

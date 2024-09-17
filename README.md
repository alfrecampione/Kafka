# Steps
## Step 1: Run docker container for kafka server
Get the Docker image in: [apache/kafka docker image](https://hub.docker.com/layers/apache/kafka/3.8.0/images/sha256-c9aea96a4813e77e703541b1d8f7d58c9ee05b77353da33684db55c840548791)

Start the Kafka Docker container:
```cmd
$ docker run -p 9092:9092 apache/kafka-native:3.8.0
```
Once the Kafka server has successfully launched, you will have a basic Kafka environment running and ready to use.


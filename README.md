# Steps
## Step 1: Run Docker Container for Kafka Server
1. Get the Docker Image: You can use the [Apache Kafka Docker Image](https://hub.docker.com/layers/apache/kafka/3.8.0/images/sha256-c9aea96a4813e77e703541b1d8f7d58c9ee05b77353da33684db55c840548791)

1. Start the Kafka Docker container: Run the following command to start Kafka:
	```bash
	docker run -p 9092:9092 apache/kafka-native:3.8.0
	```

	- Note: Make sure that apache/kafka:3.8.0 is the correct tag and replace it with the appropriate version if needed.

	Once the Kafka server has successfully launched, you will have a basic Kafka environment running and ready to use.

## Step 2: Run Docker Container for the Producer
1. Build the Producer Docker Image: Navigate to the directory containing your Dockerfile for the Producer:
	```bash
	cd Producer
	```
	Build the Docker image for your Producer:
	```bash
	docker build -t <producer-image> .
	```
1. Run the Producer Docker Container: Start the container using the image you built:
	```bash
	docker run -e KAFKA_BROKER=host.docker.internal:9092 -p 8080:8080 <producer-image>
	```
	
	- Note: ```host.docker.internal``` is used to connect to the Kafka server from within Docker on some systems. Adjust this if necessary depending on your environment.
	- Replace ```8080:8080``` with the appropriate port mapping if your Producer uses a different port.

## Step 3: Run Docker Container for the Consumer
1. Build the Consumer Docker Image: Navigate to the directory containing your Dockerfile for the Consumer:
	```bash
	cd Consumer
	```
	Build the Docker image for your Consumer:
	```bash
	docker build -t <consumer-image> .
	```
1. Run the Consumer Docker Container: Start the container using the image you built:
	```bash
	docker run -e KAFKA_BROKER=host.docker.internal:9092 -p 8081:8081 <consumer-image>
	```
	
	- Note: Adjust the port mapping ```8081:8081``` based on your Consumer configuration.
	- Make sure to use the correct environment variable ```KAFKA_BROKER``` value that matches your Kafka server configuration.
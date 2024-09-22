docker run -p 9092:9092  --name kafka --network kafka-network apache/kafka:3.8.0

docker run -d -e BOOTSTRAP_SERVERS=kafka:9092 -e TOPIC=quickstart-events --name producer --network kafka-network producer

docker run -d -e BOOTSTRAP_SERVERS=kafka:9092 -e TOPIC=quickstart-events --name consumer --network kafka-network consumer

docker run -d -p 2181:2181 --net=kafka-network --name=zookeeper -e ZOOKEEPER_CLIENT_PORT=2181 confluentinc/cp-zookeeper:7.3.1

docker run -d --net=kafka-network --name=kafka -p 9092:9092 -e KAFKA_ZOOKEEPER_CONNECT=zookeeper:2181 -e KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://kafka:9092 -e KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR=1 confluentinc/cp-kafka
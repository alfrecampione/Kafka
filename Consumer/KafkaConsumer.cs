using Confluent.Kafka;
using System.Text.Json;

namespace Consumer;
public class KafkaConsumer(string bootstrapServers = "localhost:9092", string topic = "quickstart-events")
{
    public class MessagePayload
    {
        public string? Message { get; set; }
        public string? Hash { get; set; }
    }

    public void ConsumeEvent()
    {
        var config = new ProducerConfig { BootstrapServers = bootstrapServers };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe("quickstart-events");

        try
        {
            while (true)
            {
                var consumeResult = consumer.Consume();
                // Deserializar el mensaje JSON
                var messagePayload = JsonSerializer.Deserialize<MessagePayload>(consumeResult.Message.Value);
                if (messagePayload == null)
                    break;

                Console.WriteLine($"Received message:");
                Console.WriteLine($" - Time: {messagePayload.Message}");
                Console.WriteLine($" - Hash: {messagePayload.Hash}");
            }
        }
        catch (ConsumeException e)
        {
            Console.WriteLine($"Error occurred: {e.Error.Reason}");
        }
    }
    public static void Start()
    {
        var consumer = new KafkaConsumer();
        consumer.ConsumeEvent();
    }
}
using Confluent.Kafka;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;

namespace Producer;

public class KafkaProducer(string bootstrapServers = "localhost:9092", string topic = "quickstart-events")
{

    public class MessagePayload
    {
        public string? Message { get; set; }
        public string? Hash { get; set; }
    }

    private static string GenerateSha256(string rawData)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));
        StringBuilder builder = new StringBuilder();
        foreach (var bytePart in bytes)
        {
            builder.Append(bytePart.ToString("x2"));
        }
        return builder.ToString();
    }
    public async Task ProduceEventsAsync(string message)
    {
        var config = new ProducerConfig { BootstrapServers = bootstrapServers };

        using var producer = new ProducerBuilder<Null, string>(config).Build();
        try
        { // Hora en formato ISO 8601
            var hash = GenerateSha256(message);
            // Crear el payload con ambos valores
            var payload = new MessagePayload
            {
                Message = message,
                Hash = hash
            };

            var sender = JsonSerializer.Serialize(payload);
            var result = await producer.ProduceAsync(topic, new Message<Null, string> { Value = sender });
            Console.WriteLine($"Delivered '{result.Value}' to '{result.TopicPartitionOffset}'");
        }
        catch (ProduceException<Null, string> e)
        {
            Console.WriteLine($"Delivery failed: {e.Error.Reason}");
        }
    }


    public static void Start()
    {
        var producer = new KafkaProducer();  // Assume KafkaProducer is defined elsewhere

        // Method that will be run as a task
        static async Task ActiveProducer(KafkaProducer producer)
        {
            try
            {
                var systemTime = DateTime.UtcNow.ToString("o");
                await producer.ProduceEventsAsync(systemTime);  // Use 'await' for asynchronous Kafka call
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred in producer: {ex.Message}");
                throw;
            }
        }

        // Use Task.Run in an infinite loop to produce Kafka messages every second
        Task.Run(async () =>
        {
            while (true)
            {
                await ActiveProducer(producer);  // Execute the ActiveProducer task
                await Task.Delay(1000);  // Delay for 1 second before next execution
            }
        });
    }

}
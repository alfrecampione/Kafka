using Confluent.Kafka;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;

namespace Producer;

public class KafkaProducer(IProducer<Null, string> producer, string topic = "quickstart-events")
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
            Console.WriteLine($"Delivered '{result.Value}' to '{result.Topic}'");
            producer.Flush();
        }
        catch (ProduceException<Null, string> e)
        {
            Console.WriteLine($"Delivery failed: {e.Error.Reason}");
        }
    }


    public static async void Start(string bootstrapServers, string topic)
    {
        var config = new ProducerConfig { BootstrapServers = bootstrapServers, AllowAutoCreateTopics = true };
        using var producer = new ProducerBuilder<Null, string>(config).Build();

        var my_producer = new KafkaProducer(producer, topic);

        Console.WriteLine("New Producer started");

        // Method that will be run as a task
        async Task ActiveProducer()
        {
            try
            {
                var systemTime = DateTime.UtcNow.ToString("o");
                await my_producer.ProduceEventsAsync(systemTime);
                Console.WriteLine("Message sended");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred in producer: {ex.Message}");
                throw;
            }
        }
        int count = 0;
        while (count++ < 1)
        {
            ActiveProducer().Wait();  // Execute the ActiveProducer task
            await Task.Delay(1000);  // Delay for 1 second before next execution
        }
    }
}
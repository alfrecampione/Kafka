using Confluent.Kafka;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
        var producer = new KafkaProducer();

        static void ActiveProducer(object? producer)
        {
            if (producer == null)
                producer = new KafkaProducer();
            try
            {
                var systemTime = DateTime.UtcNow.ToString("o");
                ((KafkaProducer)producer).ProduceEventsAsync(systemTime).Wait();
            }
            catch
            {
                throw new ArgumentException(nameof(producer));
            }

        }

        while (true)
        {
            var thread = new Thread(ActiveProducer);
            thread.Start(producer);
            Thread.Sleep(1000);
        }

    }
}
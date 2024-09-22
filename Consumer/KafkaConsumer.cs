using Confluent.Kafka;
using System.Text.Json;
using System.Threading;

namespace Consumer;

public class KafkaConsumer(IConsumer<Ignore, string> consumer, string topic = "quickstart-events")
{
    private CancellationTokenSource? _cancellationTokenSource;

    public class MessagePayload
    {
        public string? Message { get; set; }
        public string? Hash { get; set; }
    }

    public void ConsumeEvent()
    {

        consumer.Subscribe(topic);
        Console.WriteLine($"Consumer subscribed to {string.Join("|", consumer.Subscription)}");

        try
        {
            Console.WriteLine("Consuming messages...");
            var consumeResult = consumer.Consume();
            Console.WriteLine("Result obtained");

            if (consumeResult == null)
            {
                Console.WriteLine("No message received");
                return;
            }

            var messagePayload = JsonSerializer.Deserialize<MessagePayload>(consumeResult.Message.Value);
            if (messagePayload != null)
            {
                Console.WriteLine($"Received message:");
                Console.WriteLine($" - Time: {messagePayload.Message}");
                Console.WriteLine($" - Hash: {messagePayload.Hash}");
            }
            consumer.Close();

        }
        catch (ConsumeException e)
        {
            Console.WriteLine($"Error occurred: {e.Error.Reason}");
        }
        finally
        {
            consumer.Close(); // Ensure consumer is closed gracefully
        }

        Console.WriteLine("Consumer stopped");
    }

    public static void Start(string bootstrapServers, string topic)
    {
        var config = new ConsumerConfig() { BootstrapServers = bootstrapServers, GroupId = "quickstart-group" };
        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        var my_consumer = new KafkaConsumer(consumer, topic);
        Console.WriteLine("New Consumer started");

        void ActiveConsumer()
        {
            try
            {
                my_consumer.ConsumeEvent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred in consumer: {ex.Message}");
                throw;
            }
        }

        while (true)
        {
            ActiveConsumer();  // Execute the ActiveConsumer task
        }
    }

    // Method to stop the consumer gracefully
    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
    }
}

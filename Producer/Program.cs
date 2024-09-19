using Producer;
class Program
{
    static void Main(string[] args)
    {
        string bootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS") ?? "localhost:9092";
        string topic = Environment.GetEnvironmentVariable("TOPIC") ?? "quickstart-events";

        Console.WriteLine("Consumer started");
        var task = Task.Run(() => KafkaProducer.Start(bootstrapServers, topic));

    }
}
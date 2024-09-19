using Producer;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

string bootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS") ?? "localhost:9092";
string topic = Environment.GetEnvironmentVariable("TOPIC") ?? "quickstart-events";

Console.WriteLine("Producer started");
var task = Task.Run(() => KafkaProducer.Start(bootstrapServers, topic));

app.Run();
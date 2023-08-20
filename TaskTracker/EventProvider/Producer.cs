using System.Text.Json;
using Confluent.Kafka;

namespace EventProvider;

public class Producer
{
    private readonly string producerName;
    private readonly IProducer<Null, string> producer;
    
    public Producer(string producerName)
    {
        this.producerName = producerName;
        var config = new ProducerConfig
                     {
                         BootstrapServers = "localhost:9092",
                     };
        producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task Produce<TData>(string topic, TData data)
    {
        var @event = Event<TData>.Create(producerName, data);
        var errors = await SchemaValidator.Validate(@event);
        if (errors.Any())
            throw new Exception(string.Join(@"\n", errors.Select(x => x.ToString())));
        
        var value = JsonSerializer.Serialize((object)@event);
        Console.WriteLine($"Produced [{topic}], [{value}]");
        await producer.ProduceAsync(topic, new Message<Null, string> { Value = value });
    }
}
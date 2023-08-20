using System.Text.Json;
using Confluent.Kafka;
using EventProvider.Models.Task.Business;
using EventProvider.Models.User;
using EventProvider.Models.Task.Streaming;

namespace EventProvider;
public class Consumer
{
    private readonly IConsumer<Null, string> consumer;
    
    public Consumer(string groupId, string[] topics)
    {
        var config = new ConsumerConfig
                     {
                         GroupId = groupId,
                         BootstrapServers = "localhost:9092",
                         AutoOffsetReset = AutoOffsetReset.Earliest
                     };
        consumer = new ConsumerBuilder<Null, string>(config).Build(); 
        consumer.Subscribe(topics);
    }

    public IEvent? Consume()
    {
        var result = consumer.Consume(TimeSpan.FromMilliseconds(100));
        if (result?.Message == null) 
            return null;
        
        Console.WriteLine($"Consumed message [{result.Topic}], [{result.Message.Value}]");
        return ParseMessage(result);

    }
    
    private static IEvent? ParseMessage(ConsumeResult<Null, string> result)
    {
        var data = result.Message.Value;
        var eventInfo = JsonSerializer.Deserialize<EventBase>(data)!.EventInfo;
        
        return eventInfo.Name switch
               {
                   EventNames.UserCreated => JsonSerializer.Deserialize<UserCreatedEventV1>(data),
                   EventNames.UserDeleted => JsonSerializer.Deserialize<UserDeletedEventV1>(data),
                   EventNames.UserUpdated => JsonSerializer.Deserialize<UserUpdatedEventV1>(data),
                   
                   EventNames.TaskCreated => JsonSerializer.Deserialize<TaskCreatedEventV1>(data),
                   
                   EventNames.TaskReassigned => JsonSerializer.Deserialize<TaskReassignedEventV1>(data),
                   EventNames.TaskAdded => JsonSerializer.Deserialize<TaskAddedEventV1>(data),
                   EventNames.TaskCompleted => JsonSerializer.Deserialize<TaskACompletedEventV1>(data),
                   _ => throw new Exception($"It seems you forgot to to add parser for event name [{eventInfo.Name}]?")
               };
    }
}
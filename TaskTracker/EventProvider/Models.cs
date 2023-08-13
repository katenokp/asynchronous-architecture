using System.Text.Json;
using Confluent.Kafka;

namespace EventProvider;

public class Message
{
    public Message(IMessageData? data)
    {
        Value = data;
    }

    public bool IsEmpty => Value == null;
    public IMessageData? Value { get; set; }

    public static Message Empty()
    {
        return new Message(default);
    }
}

public interface IMessageData
{
}

public record UserRegisteredData(Guid UserId, string Name, string Role): IMessageData;
public record UserUpdatedData(Guid UserId, string Name, string Role): IMessageData;
public record UserDeletedData(Guid UserId): IMessageData;
public record UserRoleChangedData(Guid UserId, string Role): IMessageData;

public record TaskAssignedData(Guid TaskId, Guid AssignedToUserId): IMessageData;
public record TaskAddedData(Guid TaskId, Guid AssignedToUserId): IMessageData;
public record TaskCompletedData(Guid TaskId, Guid UserId): IMessageData;

public static class Topics
{
    public static class UserTopics
    {
        public const string Updated = "User.Streaming.Updated";
        public const string Deleted = "User.Streaming.Deleted";
        public const string Registered = "User.Registered";
        public const string RoleChanged = "User.RoleChanged";
    }

    public static class TaskTopics
    {
        public const string Added = "Task.Added";
        public const string Assigned = "Task.Assigned";
        public const string Completed = "Task.Completed";
    }
}

public class Producer
{
    private readonly IProducer<Null, string> producer;
    
    public Producer()
    {
        var config = new ProducerConfig
                     {
                         BootstrapServers = "localhost:9092"
                     };
        producer = new ProducerBuilder<Null, string>(config).Build();
    }

    private async Task Produce<T>(string topic, T data)
    {
        var value = JsonSerializer.Serialize(data);
        Console.WriteLine($"Produced [{topic}], [{value}]");
        await producer.ProduceAsync(topic, new Message<Null, string> { Value = value });
    }

    public async Task ProduceUserEvent(UserUpdatedData data) => await Produce(Topics.UserTopics.Updated, data);
    public async Task ProduceUserEvent(UserRegisteredData data) => await Produce(Topics.UserTopics.Registered, data);
    public async Task ProduceUserEvent(UserDeletedData data) => await Produce(Topics.UserTopics.Deleted, data);
    public async Task ProduceUserEvent(UserRoleChangedData data) => await Produce(Topics.UserTopics.RoleChanged, data);
    
    public async Task ProduceTaskEvent(TaskAddedData data) => await Produce(Topics.TaskTopics.Added, data);
    public async Task ProduceTaskEvent(TaskAssignedData data) => await Produce(Topics.TaskTopics.Assigned, data);
    public async Task ProduceTaskEvent(TaskCompletedData data) => await Produce(Topics.TaskTopics.Completed, data);
}

public class Consumer
{
    private readonly IConsumer<Null, string> consumer;
    
    public Consumer(params string[] topics)
    {
        var config = new ConsumerConfig
                     {
                         GroupId = "auth-group",
                         BootstrapServers = "localhost:9092",
                         AutoOffsetReset = AutoOffsetReset.Earliest
                     };
        consumer = new ConsumerBuilder<Null, string>(config).Build(); 
        consumer.Subscribe(topics);
    }

    public Message Consume()
    {
        var result = consumer.Consume(TimeSpan.FromMilliseconds(100));
        if (result?.Message != null)
        {
            Console.WriteLine($"Consumed message [{result.Topic}], [{result.Message.Value}]");
            return new Message(ParseMessage(result));
        }

        return Message.Empty();
    }

    private static IMessageData? ParseMessage(ConsumeResult<Null, string> result)
    {
        var data = result.Message.Value;
        return result.Topic switch
               {
                   Topics.UserTopics.Registered => JsonSerializer.Deserialize<UserRegisteredData>(data),
                   Topics.UserTopics.Deleted => JsonSerializer.Deserialize<UserDeletedData>(data),
                   Topics.UserTopics.Updated => JsonSerializer.Deserialize<UserUpdatedData>(data),
                   Topics.UserTopics.RoleChanged => JsonSerializer.Deserialize<UserRoleChangedData>(data),

                   Topics.TaskTopics.Assigned => JsonSerializer.Deserialize<TaskAssignedData>(data),
                   Topics.TaskTopics.Added => JsonSerializer.Deserialize<TaskAddedData>(data),
                   Topics.TaskTopics.Completed => JsonSerializer.Deserialize<TaskCompletedData>(data),
                   _ => default
               };
    }
}
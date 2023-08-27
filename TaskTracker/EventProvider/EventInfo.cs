using System.ComponentModel.DataAnnotations;

namespace EventProvider;

public interface IEvent
{
    public EventInfo EventInfo { get; set; }
}

public class EventBase: Event<object>
{
    public EventInfo EventInfo { get; set; }
    public object Data { get; set; }
}

public class Event<TData>: IEvent
{
    public EventInfo EventInfo { get; set; }
    public TData Data { get; set; }
    
    public static Event<TData> Create(string producerName, TData data, string eventName)
    {
        return new Event<TData>
               {
                   EventInfo = EventInfo.Create(producerName, eventName, 1),
                   Data = data
               };
    }
}

public class EventInfo
{
    [Required]
    public Guid EventId { get; set; }
    [Required]
    public int Version { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public DateTime Time { get; set; }
    [Required]
    public string Producer { get; set; }

    public static EventInfo Create(string producerName, string eventName, int version)
    {
        return new EventInfo
               {
                   Producer = producerName,
                   Name = eventName,
                   Time = DateTime.Now,
                   Version = version,
                   EventId = Guid.NewGuid()
               };
    }
}

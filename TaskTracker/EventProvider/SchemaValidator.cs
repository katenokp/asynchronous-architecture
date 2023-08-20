using System.Text.Json;
using NJsonSchema;
using NJsonSchema.Validation;

namespace EventProvider;

public class SchemaValidator
{
    public static async Task<ValidationError[]> Validate(IEvent @event)
    {
        var schema = await JsonSchema.FromFileAsync(Helpers.GetSchemaFileName(@event.GetType()));
        return schema.Validate(JsonSerializer.Serialize((object)@event)).ToArray();
    }
}
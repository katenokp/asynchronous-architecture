using System.Reflection;
using NJsonSchema;

namespace EventProvider;

public class SchemaGenerator
{
    public static void Main()
    {
        var eventTypes = Assembly.GetExecutingAssembly()
                     .GetTypes()
                     .Where(type => typeof(IEvent).IsAssignableFrom(type) && !type.IsInterface)
                     .ToArray();
        foreach (var eventType in eventTypes)
        {
            CreateSchema(eventType);
        }
    }

    private static void CreateSchema(Type type)
    {
        var schema = JsonSchema.FromType(type);
        var fileName = Helpers.GetSchemaFileName(type);

        PrepareDirectory(fileName);
        File.WriteAllText(fileName, schema.ToJson());
    }

    private static void PrepareDirectory(string fileName)
    {
        var directoryName = Path.GetDirectoryName(fileName);
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }
    }
}
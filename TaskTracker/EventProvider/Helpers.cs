namespace EventProvider;

public static class Helpers
{
    public static string GetSchemaFileName(Type type)
    {
        var path = Path.Combine(type.FullName.Replace("Models", "Schemas").Split("."));
        var fullPath = Path.GetFullPath(Path.Combine(type.Assembly.Location, @"..\..\..\..\..", path));
        return $"{fullPath}.json";
    }
}
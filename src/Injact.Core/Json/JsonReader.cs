namespace Injact.Core;

public class JsonReader
{
    public static T? Read<T>(string path)
    {
        using var fileStream = File.OpenRead(path);
        return JsonSerializer.Deserialize<T>(fileStream);
    }

    public static async Task<T?> ReadAsync<T>(string path)
    {
        await using var fileStream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<T>(fileStream);
    }
}

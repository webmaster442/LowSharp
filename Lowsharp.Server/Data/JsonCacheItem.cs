namespace Lowsharp.Server.Data;

public class JsonCacheItem
{
    public required string Id { get; set; }
    public required string Result { get; set; }
    public required DateTime CreateDate { get; set; }
}

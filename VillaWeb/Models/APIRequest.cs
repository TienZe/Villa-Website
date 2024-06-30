using static VillaUtility.SD;
using VillaWeb.Infrastructures;

namespace VillaWeb.Models;

public class APIRequest
{
    public ApiType ApiType { get; set; } = ApiType.GET;
    public string Url { get; set; }
    public string ContentType { get; set; } = ContentTypes.Json;
    public object? Data { get; set; }
    public string? Token { get; set; }
}
using System.IO;
using Newtonsoft.Json;

namespace CmcStandardPrinting.Domain.Uploads;

public class FileTransferStatus
{
    public const string HandlerPath = "/";

    [JsonProperty("group")]
    public string? Group { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("size")]
    public int Size { get; set; }

    [JsonProperty("progress")]
    public string Progress { get; set; } = "1.0";

    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;

    [JsonProperty("thumbnail_url")]
    public string ThumbnailUrl { get; set; } = string.Empty;

    [JsonProperty("delete_url")]
    public string DeleteUrl { get; set; } = string.Empty;

    [JsonProperty("delete_type")]
    public string DeleteType { get; set; } = "DELETE";

    [JsonProperty("result_code")]
    public int ResultCode { get; set; }

    [JsonProperty("error")]
    public string? Error { get; set; }

    public static FileTransferStatus FromFile(FileInfo fileInfo)
    {
        return FromFile(fileInfo.Name, (int)fileInfo.Length);
    }

    public static FileTransferStatus FromFile(string fileName, int length)
    {
        var file = Path.GetFileName(fileName) ?? string.Empty;
        return new FileTransferStatus
        {
            Name = file,
            Type = "image/png",
            Size = length,
            Url = $"{HandlerPath}FileTransferHandler.ashx?f={file}",
            ThumbnailUrl = $"{HandlerPath}Thumbnail.ashx?f={file}",
            DeleteUrl = $"{HandlerPath}FileTransferHandler.ashx?f={file}",
            DeleteType = "DELETE",
            ResultCode = 0
        };
    }
}

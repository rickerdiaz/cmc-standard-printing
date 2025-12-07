using System.Collections.Generic;

namespace CmcStandardPrinting.Domain.DigitalAssets;

public sealed class GenericCodeValueList
{
    public int Code { get; set; }
    public string Value { get; set; } = string.Empty;
}

public sealed class DigitalAsset
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int MediaType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Keyword { get; set; } = string.Empty;
}

public sealed class ResponseDigitalAssets
{
    public List<DigitalAsset> Data { get; set; } = new();
    public int Total { get; set; }
}

namespace CmcStandardPrinting.Domain.Sources;

public sealed class Source
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Global { get; set; }
}

public sealed class GenericCodeValueList
{
    public int Code { get; set; }
    public string Value { get; set; } = string.Empty;
}

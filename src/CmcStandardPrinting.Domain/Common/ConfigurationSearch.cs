namespace CmcStandardPrinting.Domain.Common;

public sealed class ConfigurationcSearch
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CodeProperty { get; set; }
    public int CodeSite { get; set; }
    public int CodeTrans { get; set; }
    public int Type { get; set; }
    public string CodeUser { get; set; } = string.Empty;
    public string CodeListe { get; set; } = string.Empty;
    public int Status { get; set; }
    public int Tree { get; set; }
    public int Link { get; set; }
}

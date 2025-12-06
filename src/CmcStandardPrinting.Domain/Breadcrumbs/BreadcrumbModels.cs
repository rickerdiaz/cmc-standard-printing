namespace CmcStandardPrinting.Domain.Breadcrumbs;

public sealed class BreadcrumbsData
{
    public int CodeUser { get; set; }
    public int ListeItemType { get; set; }
    public string CodeListe { get; set; } = string.Empty;
    public string Tab { get; set; } = string.Empty;
    public string Save { get; set; } = string.Empty;
}

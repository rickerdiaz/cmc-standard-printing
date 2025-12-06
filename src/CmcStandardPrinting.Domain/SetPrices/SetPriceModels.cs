namespace CmcStandardPrinting.Domain.SetPrices;

public sealed class SetPrice
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Main { get; set; }
    public int HasMain { get; set; }
    public bool Global { get; set; }
    public int CodeCurrency { get; set; }
    public string Format { get; set; } = string.Empty;
    public string Symbole { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool ChkDisable { get; set; }
    public double Factor { get; set; }
}

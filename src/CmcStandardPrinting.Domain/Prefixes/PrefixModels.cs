using System.Collections.Generic;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.SaleSites;

namespace CmcStandardPrinting.Domain.Prefixes;

public sealed class PrefixGeneric
{
    public int CodePrefix { get; set; }
    public int CodeListe { get; set; }
    public int CodeTrans { get; set; }
    public int CodeUser { get; set; }
}

public sealed class Prefix
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Type { get; set; }
    public string Gender { get; set; } = string.Empty;
    public int TranslationCode { get; set; }
    public string PrefixLanguage { get; set; } = string.Empty;
    public bool IsGlobal { get; set; }
    public int CodeOwner { get; set; }
}

public sealed class PrefixData
{
    public User Profile { get; set; } = new();
    public Prefix Info { get; set; } = new();
    public List<GenericTranslation> Translation { get; set; } = new();
    public List<GenericList> Sharing { get; set; } = new();
    public int ActionType { get; set; }
}

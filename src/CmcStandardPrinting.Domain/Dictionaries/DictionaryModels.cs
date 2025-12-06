namespace CmcStandardPrinting.Domain.Dictionaries;

using System.Collections.Generic;

public sealed class DictionaryItem
{
    public int CodeGroup { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CodeDictionary { get; set; }
}

public sealed class DictionarySearch
{
    public int CodeSite { get; set; }
    public int CodeTrans { get; set; }
    public int CodeProperty { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class DictionaryInfo
{
    public int CodeDictionary { get; set; }
    public int CodeGroup { get; set; }
    public int CodeTransMain { get; set; }
}

public sealed class DictionaryTranslation
{
    public string Name { get; set; } = string.Empty;
    public int CodeTrans { get; set; }
}

public sealed class DictionaryData
{
    public DictionaryInfo Info { get; set; } = new();
    public List<DictionaryTranslation> Translation { get; set; } = new();
}

public sealed class DictionaryDeleteCode
{
    public int CodeDictionary { get; set; }
}

public sealed class DictionaryDeleteData
{
    public List<DictionaryDeleteCode> CodeList { get; set; } = new();
    public int CodeUser { get; set; }
    public int CodeSite { get; set; }
    public bool ForceDelete { get; set; }
}

public sealed class DictionaryListItem
{
    public int Code { get; set; }
    public string Value { get; set; } = string.Empty;
}

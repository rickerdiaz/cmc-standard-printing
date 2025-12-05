using System.Collections.Generic;

namespace CmcStandardPrinting.Domain.Printers;

public enum GroupLevel
{
    Property,
    Site
}

public sealed class Printer
{
    public int Code { get; set; }
    public string? Name { get; set; }
    public string? SaleSiteName { get; set; }
    public bool IsGlobal { get; set; }
    public int Status { get; set; }
    public int CodeSaleSite { get; set; }
}

public sealed class PrinterProfile
{
    public int Code { get; set; }
    public int CodeSite { get; set; }
}

public sealed class PrinterInfo
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Status { get; set; }
    public bool IsGlobal { get; set; }
    public int CodeSaleSite { get; set; }
}

public sealed class SharingItem
{
    public int Code { get; set; }
}

public sealed class PrinterData
{
    public PrinterInfo Info { get; set; } = new();
    public PrinterProfile Profile { get; set; } = new();
    public List<SharingItem> Sharing { get; set; } = new();
    public List<int> MergeList { get; set; } = new();
}

public sealed class ConfigurationSearch
{
    public int CodeSite { get; set; }
    public int CodeProperty { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class GenericTree
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ParentCode { get; set; }
    public string ParentName { get; set; } = string.Empty;
    public bool Flagged { get; set; }
    public int Type { get; set; }
    public bool Global { get; set; }
}

public sealed class TreeNode
{
    public string Title { get; set; } = string.Empty;
    public int Key { get; set; }
    public bool Icon { get; set; }
    public List<TreeNode> Children { get; set; } = new();
    public bool Select { get; set; }
    public bool Selected { get; set; }
    public string ParentTitle { get; set; } = string.Empty;
    public GroupLevel GroupLevel { get; set; }
    public object? Note { get; set; }
}

public sealed class ResponseCallBack
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? ReturnValue { get; set; }
    public bool Status { get; set; }
    public List<Param>? Parameters { get; set; }
}

public sealed class Param
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public sealed class GenericTranslation
{
    public int CodeTrans { get; set; }
    public string TranslationName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public sealed class GenericDeleteData
{
    public List<DeleteCode> CodeList { get; set; } = new();
    public int CodeUser { get; set; }
    public int CodeSite { get; set; }
    public bool ForceDelete { get; set; }
}

public sealed class DeleteCode
{
    public int Code { get; set; }
}

public sealed class GenericList
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class StandardPrintingInput
{
    public string IntCodePrintList { get; set; } = string.Empty;
    public string UserLocale { get; set; } = string.Empty;
    public string StrSelectedCodeListe { get; set; } = string.Empty;
    public int CodeUser { get; set; }
    public int IntCodeTrans { get; set; }
    public string StrExcelFilename { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
}

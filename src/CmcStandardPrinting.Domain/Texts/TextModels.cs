namespace CmcStandardPrinting.Domain.Texts;

using System.Collections.Generic;
using CmcStandardPrinting.Domain.Common;
using CmcStandardPrinting.Domain.Users;

public sealed class Text
{
    public int TextCode { get; set; }
    public string TextName { get; set; } = string.Empty;
    public string TextDate { get; set; } = string.Empty;
    public bool Global { get; set; }
}

public sealed class TextData
{
    public User Profile { get; set; } = new();
    public Text Info { get; set; } = new();
    public List<GenericList>? Sharing { get; set; }
    public int ActionType { get; set; }
    public List<int>? MergeList { get; set; }
}

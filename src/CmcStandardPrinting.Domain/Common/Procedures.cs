using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CmcStandardPrinting.Domain.Common;

public static class Common
{
    public const string SP_GET_KEYWORDCODENAME = "GET_KEYWORDCODENAME";
    public const string SP_API_GET_Config = "API_GET_Config";
    public const string SP_API_GET_PasswordAndLoginInfo = "API_GET_PasswordAndLoginInfo";
    public const string SP_API_UPDATE_Sharing = "API_UPDATE_Sharing";

    public static string MapPath(string relative) => Path.Combine(AppContext.BaseDirectory, relative);

    public static string Join(IEnumerable values, string prefix, string suffix, string separator)
    {
        if (values == null) return string.Empty;
        var parts = values.Cast<object?>().Select(v => $"{prefix}{v}{suffix}");
        return string.Join(separator, parts);
    }

    public static string ReplaceSpecialCharacters(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        var result = value;
        var specialChars = new Dictionary<string, string>
        {
            { "ä", "ae" },
            { "ö", "oe" },
            { "ü", "ue" },
            { "ß", "ss" },
            { "é", "e" },
            { "è", "e" },
            { "ê", "e" },
            { "à", "a" },
            { "á", "a" },
            { "â", "a" },
            { "ù", "u" },
            { "û", "u" },
            { "ú", "u" },
            { "î", "i" },
            { "ï", "i" }
        };

        foreach (var key in specialChars.Keys)
        {
            result = result.Replace(key, specialChars[key], StringComparison.OrdinalIgnoreCase);
        }

        return result;
    }
}

using System;
using System.Collections;

namespace EgsReport;

/// <summary>
/// Legacy user structure mirrored from the original VB implementation to keep DevExpress
/// reporting signatures compatible with the printer workflow.
/// </summary>
public struct structUser
{
    public int Code { get; set; }
    public byte Status { get; set; }
    public string Username { get; set; }
    public DateTime DateModified { get; set; }
    public DateTime DateCreated { get; set; }
    public structSite Site { get; set; }
    public enumGroupLevel RoleLevelHighest { get; set; }

    public int CodeLang { get; set; }
    public int CodeTrans { get; set; }
    public int LastSetPrice { get; set; }
    public int CodeTimeZone { get; set; }
    public int PageSize { get; set; }
    public int NutrientDBCode { get; set; }
    public string Fullname { get; set; }
    public string SiteThemeFolder { get; set; }
    public string SiteName { get; set; }
    public string SiteLogoURL { get; set; }
    public string Email { get; set; }

    public bool UseBestUnit { get; set; }
    public enumListeDisplayMode eDisplayMode { get; set; }

    public ArrayList? arrRoles { get; set; }
    public ArrayList? arrSitesAccessible { get; set; }
    public ArrayList? arrListeTypeApprovalRequired { get; set; }
    public ArrayList? arrRolesNames { get; set; }
    public DateTime DateLastAccessed { get; set; }

    public string WebHomePageBrowseListTableForMerchandise { get; set; }
    public string WebHomePageBrowseListTableForRecipe { get; set; }
    public string WebHomePageBrowseListTableForMenu { get; set; }

    public int LastSetPriceSales { get; set; }
    public int EGSID { get; set; }
    public bool FullText { get; set; }
    public string ListItemColor { get; set; }
    public string ListAlternatingItemColor { get; set; }
    public bool RemoveTrailingZeroes { get; set; }
    public string PrintOutput { get; set; }
    public string UnsavedItemColor { get; set; }
    public string UserSession { get; set; }

    public int CodeCaptions { get; set; }
    public enumUserRights RoleRights { get; set; }

    public bool PreviewAllowMultipleWindows { get; set; }
    public bool DisplayQuantitiesAsFractions { get; set; }
    public bool AutoConversion { get; set; }

    public string ClientSerial { get; set; }
    public int LoginStatusCode { get; set; }
    public bool AllowAccessToOtherSite { get; set; }
    public string CulturePref { get; set; }
}

public struct structSite
{
    public int Code { get; set; }
    public string Name { get; set; }
}

public enum enumGroupLevel
{
    Global = 0,
    Site = 1,
    Property = 2,
    User = 3
}

public enum enumListeDisplayMode
{
    Details = 0,
    Thumbnail = 1,
    List = 2,
    ProjectList = 3,
    NutrientView = 4,
    AllergenView = 5
}

public enum enumReportType
{
    None = 0,
    MerchandiseList = 1,
    RecipeList = 3,
    RecipeDetail = 4,
    MerchandiseDetail = 5,
    ShoppingListDetail = 7,
    MenuDetail = 10,
    MenuList = 11,
    MerchandiseNutrientList = 12,
    RecipeNutrientList = 13,
    MenuNutrientList = 14,
    MerchandisePriceList = 15,
    MerchandiseThumbnails = 16,
    RecipeThumbnails = 17,
    MenuThumbnails = 18,
    MenuPlan = 19
}

public enum enumPrintSubRecipesOptions
{
    None = 0,
    All = 1,
    FirstLevel = 2
}

public enum enumUserRights
{
    Admin = 11,
    Editor = 13,
    Approver = 12,
    Visitor = 14
}

public enum enumFileType
{
    HTML = 0,
    PDF = 1,
    Excel = 2,
    RTF = 3,
    WordDocument = 4
}

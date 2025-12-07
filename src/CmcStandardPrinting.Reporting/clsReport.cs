using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using DevExpress.XtraReports.UI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace EgsReport;

public class clsReport
{
    private readonly ILogger<clsReport> _logger;

    public clsReport()
        : this(NullLogger<clsReport>.Instance)
    {
    }

    public clsReport(ILogger<clsReport> logger)
    {
        _logger = logger;
    }

    public string TitleColor { get; set; } = string.Empty;
    public bool NoPrintLines { get; set; }
    public string FooterAddress { get; set; } = string.Empty;
    public string FooterLogoPath { get; set; } = string.Empty;
    public bool PictureOneRight { get; set; }
    public bool DisplaySubRecipeNormalFont { get; set; }
    public bool DisplaySubRecipeAstrisk { get; set; }
    public int DisplayRecipeDetails { get; set; } = -1;
    public string strMigrosParam { get; set; } = ";;;";
    public bool blnThumbnailsView { get; set; }
    public string SelectedWeek { get; set; } = string.Empty;
    public int CodeUserPlan { get; set; }
    public string CLIENT { get; set; } = string.Empty;

    public XtraReport CreateReport(
        int intCodePrintList,
        structUser udtUser,
        string strConnection,
        ref int documentOutput,
        string strPhotoPath,
        string strLogoPath = "",
        int intFoodlaw = 1)
    {
        _logger.LogInformation("CreateReport called for print list {PrintList}", intCodePrintList);
        return BuildBasicReport(strPhotoPath, strLogoPath, strConnection, udtUser, null, ref documentOutput, IsCalcmenuOnline);
    }

    public XtraReport CreateReport(
        DataSet ds2,
        structUser udtUser,
        string strConnection,
        ref int documentOutput,
        string strPhotoPath = "",
        string strLogoPath = "",
        string strLogoPath2 = "",
        string strSiteUrl = "",
        bool IsCalcmenuOnline = false,
        bool blnIsAllowMetricImperial = true,
        int intFoodlaw = 1,
        int CodePrintList = 0)
    {
        _logger.LogInformation("CreateReport called with dataset for print list {PrintList}", CodePrintList);
        return BuildBasicReport(strPhotoPath, strLogoPath, strConnection, udtUser, ds2, ref documentOutput, IsCalcmenuOnline);
    }

    public XtraReport CreateReport_CMC(
        DataSet ds2,
        string strConnection,
        ref int documentOutput,
        string strPhotoPath = "",
        string strLogoPath = "",
        string strLogoPath2 = "",
        string strSiteUrl = "",
        bool IsCalcmenuOnline = false,
        bool blnIsAllowMetricImperial = true,
        int intFoodlaw = 1,
        int CodePrintList = 0,
        string userLocale = "en-US",
        int codeUser = 1)
    {
        _logger.LogInformation(
            "CreateReport_CMC invoked with CodePrintList {CodePrintList} and user {User}",
            CodePrintList,
            codeUser);

        var udtUser = new structUser
        {
            Code = codeUser,
            CulturePref = userLocale,
            Username = string.Empty,
            Site = new structSite()
        };

        return BuildBasicReport(strPhotoPath, strLogoPath, strConnection, udtUser, ds2, ref documentOutput, IsCalcmenuOnline);
    }

    private XtraReport BuildBasicReport(
        string strPhotoPath,
        string strLogoPath,
        string strConnection,
        structUser udtUser,
        DataSet? data,
        ref int documentOutput,
        bool isCalcmenuOnline,
        int intFoodlaw = 1)
    {
        clsGlobal.G_strPhotoPath = strPhotoPath;
        clsGlobal.G_strLogoPath = strLogoPath;
        clsGlobal.G_strLogoPath2 = strLogoPath;
        clsGlobal.G_IsCalcmenuOnline = isCalcmenuOnline;
        clsGlobal.G_strConnection = strConnection;
        clsGlobal.G_CLIENT = CLIENT;

        clsGlobal.G_ReportOptions.strFooterAddress = FooterAddress;
        clsGlobal.G_ReportOptions.strFooterLogoPath = string.IsNullOrWhiteSpace(FooterLogoPath)
            ? strLogoPath
            : FooterLogoPath;
        clsGlobal.G_ReportOptions.flagNoLines = NoPrintLines;
        clsGlobal.G_ReportOptions.blnPictureOneRight = PictureOneRight;
        clsGlobal.G_ReportOptions.intPageLanguage = udtUser.CodeLang;
        clsGlobal.G_ReportOptions.strTitleColor = NormalizeHtmlColor(TitleColor);
        clsGlobal.G_ReportOptions.intfoodLaw = intFoodlaw;

        ApplyReportOptions(data);

        if (data?.Tables.Count > 0)
        {
            clsGlobal.G_ReportOptions.dtProfile = data.Tables[0];
        }

        if (data?.Tables.Count > 1)
        {
            clsGlobal.G_ReportOptions.dtDetail = data.Tables[1];
        }

        if (documentOutput == 0)
        {
            documentOutput = (int)enumFileType.PDF;
        }

        var normalizedTitleColor = NormalizeHtmlColor(TitleColor);

        var report = new XrReports(data ?? new DataSet())
        {
            TitleColor = normalizedTitleColor,
            FooterAddress = FooterAddress,
            FooterLogoPath = string.IsNullOrWhiteSpace(FooterLogoPath) ? strLogoPath : FooterLogoPath,
            HideLines = NoPrintLines,
            PictureOneRight = PictureOneRight,
            DisplaySubRecipeAsterisk = DisplaySubRecipeAstrisk,
            DisplaySubRecipeNormalFont = DisplaySubRecipeNormalFont,
            DisplayRecipeDetails = DisplayRecipeDetails,
            MigrosParam = strMigrosParam,
            ThumbnailsView = blnThumbnailsView,
            SelectedWeek = SelectedWeek,
            CodeUserPlan = CodeUserPlan,
            Culture = string.IsNullOrWhiteSpace(udtUser.CulturePref)
                ? CultureInfo.InvariantCulture.Name
                : udtUser.CulturePref,
            ExplicitTitle = CLIENT
        };

        report.ApplyData(data ?? new DataSet());

        report.CreateDocument(false);

        _logger.LogDebug(
            "Report prepared. PhotoPath={PhotoPath}, LogoPath={LogoPath}, Connection={Connection}",
            strPhotoPath,
            strLogoPath,
            strConnection);

        return report;
    }

    private static void ApplyReportOptions(DataSet? data)
    {
        if (data == null || data.Tables.Count == 0)
        {
            return;
        }

        var profile = data.Tables[0];
        var reportOptions = clsGlobal.G_ReportOptions;

        static T? GetValue<T>(DataRow row, IReadOnlyDictionary<string, DataColumn> columns, string columnName)
        {
            if (!columns.TryGetValue(columnName, out var column))
            {
                return default;
            }

            var value = row[column];
            if (value == DBNull.Value)
            {
                return default;
            }

            try
            {
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }
            catch
            {
                return default;
            }
        }

        static bool? GetBool(DataRow row, IReadOnlyDictionary<string, DataColumn> columns, string columnName)
        {
            var value = GetValue<object>(row, columns, columnName);
            if (value == null)
            {
                return null;
            }

            try
            {
                return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                if (bool.TryParse(value.ToString(), out var parsed))
                {
                    return parsed;
                }

                return null;
            }
        }

        static double? GetDouble(DataRow row, IReadOnlyDictionary<string, DataColumn> columns, string columnName)
        {
            var value = GetValue<object>(row, columns, columnName);
            if (value == null)
            {
                return null;
            }

            if (double.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
            {
                return parsed;
            }

            return null;
        }

        static float? GetFloat(DataRow row, IReadOnlyDictionary<string, DataColumn> columns, string columnName)
        {
            var dbl = GetDouble(row, columns, columnName);
            return dbl.HasValue ? (float)dbl.Value : null;
        }

        static int? GetInt(DataRow row, IReadOnlyDictionary<string, DataColumn> columns, string columnName)
        {
            var value = GetValue<object>(row, columns, columnName);
            if (value == null)
            {
                return null;
            }

            if (int.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
            {
                return parsed;
            }

            return null;
        }

        var profileColumns = profile
            .Columns
            .Cast<DataColumn>()
            .ToDictionary(c => c.ColumnName, StringComparer.OrdinalIgnoreCase);

        if (profile.Rows.Count > 0)
        {
            var firstRow = profile.Rows[0];

            void MapBool(string columnName, Action<bool> setter)
            {
                var value = GetBool(firstRow, profileColumns, columnName);
                if (value.HasValue)
                {
                    setter(value.Value);
                }
            }

            void MapString(string columnName, Action<string> setter)
            {
                var value = GetValue<string>(firstRow, profileColumns, columnName);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    setter(value);
                }
            }

            void MapInt(string columnName, Action<int> setter)
            {
                var value = GetInt(firstRow, profileColumns, columnName);
                if (value.HasValue)
                {
                    setter(value.Value);
                }
            }

            void MapFloat(string columnName, Action<float> setter)
            {
                var value = GetFloat(firstRow, profileColumns, columnName);
                if (value.HasValue)
                {
                    setter(value.Value);
                }
            }

            void MapDouble(string columnName, Action<double> setter)
            {
                var value = GetDouble(firstRow, profileColumns, columnName);
                if (value.HasValue)
                {
                    setter(value.Value);
                }
            }

            MapBool("IncludeNutrients", v => reportOptions.blnIncludeNutrients = v);
            MapBool("IncludeHaccp", v => reportOptions.blnIncludeHACCP = v);
            MapBool("IncludeInfo", v => reportOptions.blnIncludeInfo = v);
            MapBool("IncludeRemark", v => reportOptions.blnIncludeRemark = v);
            MapBool("IncludeNumber", v => reportOptions.blnIncludeNumber = v);
            MapBool("IncludeCategory", v => reportOptions.blnIncludeCategory = v);
            MapBool("IncludeSource", v => reportOptions.blnIncludeSource = v);
            MapBool("IncludeDate", v => reportOptions.blnIncludeDate = v);
            MapBool("IncludeCostOfGoods", v => reportOptions.blnIncludeCostOfGoods = v);
            MapBool("IncludeIngrNumber", v => reportOptions.blnIncludeIngrNumber = v);
            MapBool("IncludeIngrPreparation", v => reportOptions.blnIncludeIngrPreparation = v);
            MapBool("IncludePreparation", v => reportOptions.blnIncludePreparation = v);
            MapBool("IncludeCookingTip", v => reportOptions.blnIncludeCookingTip = v);
            MapBool("IncludeNetQty", v => reportOptions.blnIncludeNetQty = v);
            MapBool("IncludeGrossQty", v => reportOptions.blnIncludeGrossQty = v);
            MapBool("IncludeKeyword", v => reportOptions.blnIncludeKeyword = v);
            MapBool("IncludeAllergens", v => reportOptions.blnIncludeAllergens = v);
            MapBool("AllergensAbbrev", v => reportOptions.blnAllergensAbbrev = v);
            MapBool("IncludePicture", v => reportOptions.blnIncludePicture = v);
            MapBool("PicturesAll", v => reportOptions.blnPicturesAll = v);
            MapBool("PicturePathAccessible", v => reportOptions.blnPicturePathAccessible = v);
            MapBool("IncludeDerivedKeyword", v => reportOptions.blnIncludeDerivedKeyword = v);
            MapBool("IncludeAlternativeIngredient", v => reportOptions.blnIncludeAlternativeIngredient = v);
            MapBool("IncludeHighlightSection", v => reportOptions.blnIncludeHighlightSection = v);
            MapBool("IncludeWastage", v => reportOptions.blnIncludeWastage = v);
            MapBool("UseMetricImperial", v => reportOptions.blnUseMetricImperial = v);
            MapBool("IncludeMetric", v => reportOptions.blIncludeMetric = v);
            MapBool("IncludeImperial", v => reportOptions.blIncludeImperial = v);
            MapBool("IncludeMetricQtyGross", v => reportOptions.blnIncludeMetricQtyGross = v);
            MapBool("IncludeMetricQtyNet", v => reportOptions.blnIncludeMetricQtyNet = v);
            MapBool("IncludeImperialQtyGross", v => reportOptions.blnIncludeImperialQtyGross = v);
            MapBool("IncludeImperialQtyNet", v => reportOptions.blnIncludeImperialQtyNet = v);
            MapBool("IncludePlacement", v => reportOptions.blnIncludePlacement = v);
            MapBool("IncludeProcSequenceNo", v => reportOptions.blnIncludeProcSequenceNo = v);
            MapBool("RemoveTrailingZeros", v => reportOptions.blnRemoveTrailingZeros = v);
            MapBool("UseFractions", v => reportOptions.blnUseFractions = v);
            MapBool("IncludeDescription", v => reportOptions.blnIncludeDescription = v);
            MapBool("IncludeAddNotes", v => reportOptions.blnIncludeAddNotes = v);
            MapBool("IncludeRecipeStatus", v => reportOptions.blnIncludeRecipeStatus = v);
            MapBool("IncludeComposition", v => reportOptions.blnIncludeComposition = v);
            MapBool("MigrosCustomPrint", v => reportOptions.blnMigrosCustomPrint = v);

            MapString("FontName", v => reportOptions.strFontName = v);
            MapString("FontName2", v => reportOptions.strFontName2 = v);
            MapString("FontTitleName", v => reportOptions.strFontTitleName = v);
            MapString("SortBy", v => reportOptions.strSortBy = v);
            MapString("GroupBy", v => reportOptions.strGroupBy = v);
            MapString("TextItemFormat", v => reportOptions.strTextItemFormat = v);
            MapString("SubStyle", v => reportOptions.strSubStyle = v);

            MapFloat("FontSize", v => reportOptions.sgFontSize = v);
            MapFloat("FontSize2", v => reportOptions.sgFontSize2 = v);
            MapFloat("FontTitleSize", v => reportOptions.sgFontTitleSize = v);

            MapDouble("PageWidth", v => reportOptions.dblPageWidth = v);
            MapDouble("PageHeight", v => reportOptions.dblPageHeight = v);
            MapDouble("LeftMargin", v => reportOptions.dblLeftMargin = v);
            MapDouble("RightMargin", v => reportOptions.dblRightMargin = v);
            MapDouble("TopMargin", v => reportOptions.dblTopMargin = v);
            MapDouble("BottomMargin", v => reportOptions.dblBottomMargin = v);
            MapDouble("LineSpace", v => reportOptions.dblLineSpace = v);

            MapBool("Landscape", v => reportOptions.blLandscape = v);
            MapBool("ShrinkToFit", v => reportOptions.blnShrinkToFit = v);
            MapBool("IncludeGda", v => reportOptions.blnIncludeGDA = v);

            MapInt("Translation", v => reportOptions.intTranslation = v);
            MapInt("DataLines", v => reportOptions.intDatalines = v);
            MapInt("LoadPictureType", v => reportOptions.blnLoadPictureType = v);
            MapInt("YieldOption", v => reportOptions.intYieldOption = v);
            MapInt("SelectedNutrientSet", v => reportOptions.intSelectedNutrientSet = v);
            MapInt("CodeSet", v => reportOptions.intSelectedNutrientSet = v);
            MapInt("EnergyDisplay", v => reportOptions.intEnergyDisplay = v);
        }

        var printType = enumReportType.None;
        if (profile.Columns.Contains("printprofiletype") && profile.Rows.Count > 0)
        {
            try
            {
                printType = (enumReportType)Convert.ToInt32(profile.Rows[0]["printprofiletype"]);
            }
            catch
            {
                printType = enumReportType.None;
            }
        }

        static DataTable? TableAt(DataSet dataSet, int index)
        {
            return dataSet.Tables.Count > index ? dataSet.Tables[index] : null;
        }

        static bool HasRows(DataTable? table) => table?.Rows.Count > 0;

        reportOptions.dtKeywords = TableAt(data, 2);
        reportOptions.dtCodes = TableAt(data, 3);

        var table4 = TableAt(data, 4);
        var table5 = TableAt(data, 5);
        var table6 = TableAt(data, 6);
        var table7 = TableAt(data, 7);
        var table8 = TableAt(data, 8);
        var table9 = TableAt(data, 9);
        var table10 = TableAt(data, 10);
        var table11 = TableAt(data, 11);
        var table12 = TableAt(data, 12);
        var table13 = TableAt(data, 13);
        var table14 = TableAt(data, 14);
        var table15 = TableAt(data, 15);

        switch (printType)
        {
            case enumReportType.RecipeDetail:
                reportOptions.dtSteps = HasRows(table4) ? table4 : reportOptions.dtSteps;
                reportOptions.dtListeNote = HasRows(table5) ? table5 : reportOptions.dtListeNote;
                reportOptions.dtAllergens = HasRows(table15)
                    ? table15
                    : HasRows(table4)
                        ? table4
                        : reportOptions.dtAllergens;
                break;
            case enumReportType.MenuDetail:
                reportOptions.dtSteps = HasRows(table4) ? table4 : reportOptions.dtSteps;
                reportOptions.dtListeNote = HasRows(table5) ? table5 : reportOptions.dtListeNote;
                reportOptions.dtAllergens = HasRows(table6) ? table6 : reportOptions.dtAllergens;
                break;
            case enumReportType.MerchandiseDetail:
                reportOptions.dtAllergens = HasRows(table4) ? table4 : reportOptions.dtAllergens;
                reportOptions.dtProductLink = HasRows(table5) ? table5 : reportOptions.dtProductLink;
                break;
            default:
                if (HasRows(table4) && reportOptions.dtAllergens == null)
                {
                    reportOptions.dtAllergens = table4;
                }

                if (HasRows(table5) && reportOptions.dtListeNote == null)
                {
                    reportOptions.dtListeNote = table5;
                }

                if (HasRows(table4) && reportOptions.dtSteps == null)
                {
                    reportOptions.dtSteps = table4;
                }

                break;
        }

        reportOptions.dtSubtitle = HasRows(table6) ? table6 : reportOptions.dtSubtitle;
        reportOptions.blnIncludeSubtitle = HasRows(reportOptions.dtSubtitle);

        reportOptions.dtTimeTypes = HasRows(table7) ? table7 : reportOptions.dtTimeTypes;
        reportOptions.blnIncludeTimeTypes = HasRows(reportOptions.dtTimeTypes);

        if (printType == enumReportType.RecipeDetail && HasRows(table8))
        {
            reportOptions.dtNotes = table8;
        }
        else if (printType == enumReportType.MenuDetail && HasRows(table7))
        {
            reportOptions.dtNotes = table7;
        }

        reportOptions.blnIncludeNotes = HasRows(reportOptions.dtNotes);

        reportOptions.dtComplementPreparation = HasRows(table9) ? table9 : reportOptions.dtComplementPreparation;
        reportOptions.blnIncludeIngredientComplement = HasRows(reportOptions.dtComplementPreparation);

        reportOptions.dtBrands = HasRows(table10) ? table10 : reportOptions.dtBrands;
        reportOptions.blnIncludeBrand = HasRows(reportOptions.dtBrands);

        reportOptions.dtPublications = HasRows(table11) ? table11 : reportOptions.dtPublications;
        reportOptions.blnIncludePublication = HasRows(reportOptions.dtPublications);

        reportOptions.dtCookbook = HasRows(table12) ? table12 : reportOptions.dtCookbook;
        reportOptions.blnIncludeCookbook = HasRows(reportOptions.dtCookbook);

        if (HasRows(table13))
        {
            reportOptions.dtComment = table13;
            reportOptions.blnIncludeComment = true;
        }
        else if (printType == enumReportType.MenuDetail && HasRows(table8))
        {
            reportOptions.dtComment = table8;
            reportOptions.blnIncludeComment = true;
        }

        reportOptions.dtKiosk = HasRows(table14) ? table14 : reportOptions.dtKiosk;
        reportOptions.blnIncludeKiosk = HasRows(reportOptions.dtKiosk);

        reportOptions.blnIncludeIngredientPreparation = HasRows(reportOptions.dtSteps);
        reportOptions.blnIncludeAllergens = HasRows(reportOptions.dtAllergens);
        reportOptions.blnIncludeKeyword = HasRows(reportOptions.dtKeywords);
    }

    private static string NormalizeHtmlColor(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
        {
            return string.Empty;
        }

        if (!color.StartsWith("#", true, CultureInfo.InvariantCulture))
        {
            return color;
        }

        // ensure #RRGGBB
        return color.Length == 4
            ? $"#{color[1]}{color[1]}{color[2]}{color[2]}{color[3]}{color[3]}"
            : color;
    }
}

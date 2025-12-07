using System.Data;
using System.Globalization;

namespace EgsReport;

/// <summary>
/// Captures legacy shared state used by the original reporting layer so the new
/// implementation can retain the same configuration surface.
/// </summary>
public static class clsGlobal
{
    public const int INTCOLUMNSPACE = 5;

    public static string G_strPhotoPath { get; set; } = string.Empty;
    public static string G_strLogoPath { get; set; } = string.Empty;
    public static string G_strLogoPath2 { get; set; } = string.Empty;
    public static bool G_IsCalcmenuOnline { get; set; }

    public static string G_strDisplayUnit { get; set; } = string.Empty;
    public static double G_dblFactor { get; set; }
    public static double G_dblMaxWidthPicture { get; set; }
    public static double G_dblMaxHeightPicture { get; set; }
    public static int G_intMaxRecipeDetailsWidth { get; set; }

    public static int intAvailableWidth { get; set; }
    public static int intAvailableHeight { get; set; }
    public static int intCurrentX { get; set; }
    public static int intCurrentY { get; set; }
    public static int intTextHeight { get; set; }
    public static int intTextWidth { get; set; }

    public static bool G_IsCalcmenuOnlineReadOnly => G_IsCalcmenuOnline;
    public static string G_CLIENT { get; set; } = string.Empty;

    public static ReportOptions G_ReportOptions { get; } = new();

    public static string fctConvertToFraction(double dblValue, bool blnUseFractions = true)
    {
        if (!blnUseFractions)
        {
            return dblValue.ToString(CultureInfo.InvariantCulture);
        }

        double[] myV1 = [0.05, 0.1, 0.125, 0.2, 0.25, 1d / 3d, 0.4, 0.5, 0.6, 2d / 3d, 0.75, 0.8, 1];
        string[] myF1 =
        [
            "1/50",
            "1/10",
            "1/8",
            "1/5",
            "1/4",
            "1/3",
            "2/5",
            "1/2",
            "3/5",
            "2/3",
            "3/4",
            "4/5",
            "1"
        ];

        double lngEntier = Math.Floor(dblValue);
        double dblDecimal = dblValue - lngEntier;
        string strValue = lngEntier.ToString(CultureInfo.InvariantCulture);

        if (dblDecimal > 0.001)
        {
            double dblErrorMin = 1000;
            int indexMin = 20;

            if (lngEntier > 0)
            {
                myF1[12] = string.Empty; // legacy behavior when integer part exists

                for (int i = 0; i <= 12; i++)
                {
                    double dblError = Math.Abs(dblDecimal - myV1[i]) / dblDecimal;
                    if (dblError < dblErrorMin)
                    {
                        dblErrorMin = dblError;
                        indexMin = i;
                    }
                }

                if (indexMin == 12)
                {
                    lngEntier += 1;
                }

                strValue = $"{lngEntier.ToString(CultureInfo.InvariantCulture)} {myF1[indexMin]}".Trim();
            }
            else
            {
                dblErrorMin = 1000;
                indexMin = 20;

                for (int i = 0; i <= 12; i++)
                {
                    double dblError = Math.Abs(dblDecimal - myV1[i]) / dblDecimal;
                    if (dblError < dblErrorMin)
                    {
                        dblErrorMin = dblError;
                        indexMin = i;
                    }
                }

                strValue = myF1[indexMin];
            }
        }

        return strValue.Trim();
    }

    public static string fctConvertToFraction2(double dblValue)
    {
        if (dblValue == -1)
        {
            return string.Empty;
        }

        decimal[] myV1 =
        [
            0.05m,
            0.1m,
            0.125m,
            0.2m,
            0.25m,
            1m / 3m,
            0.4m,
            0.5m,
            0.6m,
            2m / 3m,
            0.75m,
            0.8m,
            1m
        ];

        string[] myF1 =
        [
            "1/50",
            "1/10",
            "1/8",
            "1/5",
            "1/4",
            "1/3",
            "2/5",
            "1/2",
            "3/5",
            "2/3",
            "3/4",
            "4/5",
            "1"
        ];

        decimal lngEntier = Math.Floor((decimal)dblValue);
        decimal dblDecimal = (decimal)dblValue - lngEntier;
        string strValue = lngEntier.ToString(CultureInfo.InvariantCulture);

        if (dblDecimal > 0.001m)
        {
            decimal dblErrorMin = 1000;
            int indexMin = 20;

            if (lngEntier > 0)
            {
                myF1[12] = string.Empty; // legacy behavior when integer part exists

                for (int i = 0; i <= 12; i++)
                {
                    decimal dblError = Math.Abs(dblDecimal - myV1[i]) / dblDecimal;
                    if (dblError < dblErrorMin)
                    {
                        dblErrorMin = dblError;
                        indexMin = i;
                    }
                }

                if (indexMin == 12)
                {
                    lngEntier += 1;
                }

                strValue = $"{lngEntier.ToString(CultureInfo.InvariantCulture)} {myF1[indexMin]}".Trim();
            }
            else
            {
                dblErrorMin = 1000;
                indexMin = 20;

                for (int i = 0; i <= 12; i++)
                {
                    decimal dblError = Math.Abs(dblDecimal - myV1[i]) / dblDecimal;
                    if (dblError < dblErrorMin)
                    {
                        dblErrorMin = dblError;
                        indexMin = i;
                    }
                }

                strValue = myF1[indexMin];
            }
        }

        return strValue.Trim();
    }
}

public class ReportOptions
{
    public double dblReportType { get; set; }
    public DataTable? dtDetail { get; set; }
    public DataTable? dtKeywords { get; set; }
    public DataTable? dtAllergens { get; set; }
    public DataTable? dtCodes { get; set; }
    public string strSubStyle { get; set; } = string.Empty;
    public bool blnRecipe { get; set; }
    public bool blnIncludeNutrients { get; set; }
    public bool blnIncludeHACCP { get; set; }
    public bool blnIncludeKeyword { get; set; }
    public bool blnIncludeAllergens { get; set; }
    public bool blnAllergensAbbrev { get; set; }
    public bool blnIncludeInfo { get; set; }
    public bool blnPicturePathAccessible { get; set; }
    public bool blnIncludeNumber { get; set; }
    public bool blnIncludeCategory { get; set; }
    public bool blnIncludeSource { get; set; }
    public bool blnIncludeDate { get; set; }
    public bool blnIncludeCostOfGoods { get; set; }
    public bool blnIncludeRemark { get; set; }
    public bool blnIncludeIngrNumber { get; set; }
    public bool blnIncludeIngrPreparation { get; set; }
    public bool blnWithPicture { get; set; }
    public bool blnIncludePreparation { get; set; }
    public bool blnIncludeCookingTip { get; set; }
    public bool blnIncludeNetQty { get; set; }
    public bool blnIncludeGrossQty { get; set; }
    public int intTranslation { get; set; }
    public bool blnPicturesAll { get; set; }
    public bool blnIncludePicture { get; set; }
    public string strFontName { get; set; } = string.Empty;
    public float sgFontSize { get; set; }
    public string strFontName2 { get; set; } = string.Empty;
    public float sgFontSize2 { get; set; }
    public double dblPageWidth { get; set; }
    public double dblPageHeight { get; set; }
    public double dblLeftMargin { get; set; }
    public double dblRightMargin { get; set; }
    public double dblTopMargin { get; set; }
    public double dblBottomMargin { get; set; }
    public bool blLandscape { get; set; }
    public int intPageLanguage { get; set; }
    public bool blnShrinkToFit { get; set; }
    public string strSortBy { get; set; } = string.Empty;
    public string strGroupBy { get; set; } = string.Empty;
    public bool blnRemoveTrailingZeros { get; set; }
    public double dblLineSpace { get; set; }
    public string strTextItemFormat { get; set; } = string.Empty;
    public bool blnIncludeGDA { get; set; }
    public string strFooterAddress { get; set; } = string.Empty;
    public string strFooterLogoPath { get; set; } = string.Empty;
    public bool blnPictureOneRight { get; set; }
    public bool flagNoLines { get; set; }
    public string strTitleColor { get; set; } = string.Empty;
    public string strFontTitleName { get; set; } = string.Empty;
    public float sgFontTitleSize { get; set; }
    public DataTable? dtSteps { get; set; }
    public DataTable? dtProductLink { get; set; }
    public bool blnIncludeDerivedKeyword { get; set; }
    public int intCodeTrans { get; set; }
    public DataTable? dtMPConfig { get; set; }
    public DataTable? dtPlan { get; set; }
    public DataTable? dtMPIngr { get; set; }
    public DataTable? dtPlan2 { get; set; }
    public int intYieldOption { get; set; }
    public bool bFoodcostOnly { get; set; }
    public bool bIncludeGDAImage { get; set; }
    public bool bIncludeNutrients { get; set; }
    public bool blIncludeMetric { get; set; }
    public bool blIncludeImperial { get; set; }
    public DataTable? dtListeNote { get; set; }
    public bool blnMode { get; set; }
    public bool blnIncludeIngredientPreparation { get; set; }
    public bool blnIncludeIngredientComplement { get; set; }
    public bool blnIncludeNotes { get; set; }
    public bool blnIncludeSubtitle { get; set; }
    public bool blnIncludeTimeTypes { get; set; }
    public DataTable? dtNotes { get; set; }
    public DataTable? dtSubtitle { get; set; }
    public DataTable? dtComplementPreparation { get; set; }
    public DataTable? dtTimeTypes { get; set; }
    public bool blnIncludeBrand { get; set; }
    public bool blnIncludePlacement { get; set; }
    public bool blnIncludePublication { get; set; }
    public DataTable? dtPublications { get; set; }
    public DataTable? dtBrands { get; set; }
    public bool blnIncludeProcSequenceNo { get; set; }
    public int intDatalines { get; set; }
    public int blnLoadPictureType { get; set; }
    public bool blnIncludeMetricQtyGross { get; set; }
    public bool blnIncludeMetricQtyNet { get; set; }
    public bool blnIncludeImperialQtyGross { get; set; }
    public bool blnIncludeImperialQtyNet { get; set; }
    public bool blnIncludeAlternativeIngredient { get; set; }
    public bool blnIncludeHighlightSection { get; set; }
    public bool blnUseMetricImperial { get; set; }
    public bool blnIncludeWastage { get; set; }
    public bool blnUseFractions { get; set; }
    public bool blnIncludeDescription { get; set; }
    public bool blnIncludeAddNotes { get; set; }
    public bool blnIncludeCookbook { get; set; }
    public bool blnIncludeKiosk { get; set; }
    public bool blnIncludeComment { get; set; }
    public DataTable? dtCookbook { get; set; }
    public DataTable? dtComment { get; set; }
    public DataTable? dtKiosk { get; set; }
    public bool blnIncludeServeWith { get; set; }
    public bool blnIncludeRecipeStatus { get; set; }
    public int intSelectedNutrientSet { get; set; }
    public DataTable? dtProfile { get; set; }
    public int intfoodLaw { get; set; }
    public int intEnergyDisplay { get; set; }
    public bool blnIncludeComposition { get; set; }
    public bool blnMigrosCustomPrint { get; set; }
}

public struct tRecipe
{
    public double QuantityNet { get; set; }
    public string sUnit { get; set; }
    public string sUnitFormat { get; set; }
    public double UnitFactor { get; set; }
}

public struct tNutrient
{
    public double Value { get; set; }
    public string Name { get; set; }
    public string Unit { get; set; }
    public string Heading { get; set; }
    public string Format { get; set; }
    public string Number { get; set; }
    public double GDA { get; set; }
    public string FormatKCAL { get; set; }
    public int Position { get; set; }
    public int DisplayPosition { get; set; }
}

public enum enumMPStyle
{
    A4HWLogo = 1,
    A4HWOLogo = 2,
    A4CWLogo = 3,
    A4CWOLogo = 4,
    A4HCWLogo = 5,
    A4HCWOLogo = 6,
    A4CCWLogo = 7,
    A4CCWOLogo = 8,
    A3CWLogo = 9,
    A3CWOLogo = 10,
    A3HCWLogo = 11,
    A3HCWOLogo = 12,
    A3CCWLogo = 13,
    A3CCWOLogo = 14,
    _6by4 = 15,
    A4CWLogoOld = 16,
    A4CWOLogoOld = 17,
    A4CCWLogoOld = 18,
    A4CCWOLogoOld = 19,
    A3CWLogoOld = 20,
    A3CWOLogoOld = 21,
    A3CCWLogoOld = 22,
    A3CCWOLogoOld = 23,
    A2CWLogo = 24,
    A2CWOLogo = 25,
    A2CCWLogo = 26,
    A2CCWOLogo = 27,
    A2CWOLogoNew = 28,
    A2CCWOLogoNew = 29,
}

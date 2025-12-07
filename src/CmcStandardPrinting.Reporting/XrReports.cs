using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using DevExpress.XtraReports.UI;

namespace EgsReport;

/// <summary>
/// Lightweight C# port of the legacy <c>xrReports</c> DevExpress layout. The goal is not to
/// recreate every designer detail but to preserve the configurable surface area that callers
/// relied on (title colors, footer metadata, and layout flags) while delegating the actual
/// rendering to the modern <see cref="StandardDetailReport"/> implementation.
/// </summary>
public class XrReports : XtraReport
{
    public string TitleColor { get; set; } = string.Empty;
    public string FooterAddress { get; set; } = string.Empty;
    public string FooterLogoPath { get; set; } = string.Empty;
    public bool HideLines { get; set; }
    public bool PictureOneRight { get; set; }
    public bool DisplaySubRecipeAsterisk { get; set; }
    public bool DisplaySubRecipeNormalFont { get; set; }
    public int DisplayRecipeDetails { get; set; } = -1;
    public string MigrosParam { get; set; } = ";;;;";
    public bool ThumbnailsView { get; set; }
    public string SelectedWeek { get; set; } = string.Empty;
    public int LeftMarginOffset { get; set; }
    public string SiteUrl { get; set; } = string.Empty;
    public enumMPStyle MenuPlanPrintStyle { get; set; } = enumMPStyle.GenericWeek;
    public int CodeUserPlan { get; set; }
    public string Culture { get; set; } = CultureInfo.InvariantCulture.Name;
    public string ExplicitTitle { get; set; } = string.Empty;

    /// <summary>
    /// Create a wrapper report and optionally bind data immediately.
    /// </summary>
    public XrReports(DataSet? data = null)
    {
        ReportUnit = ReportUnit.TenthsOfAMillimeter;
        PaperKind = System.Drawing.Printing.PaperKind.A4;
        Landscape = false;

        if (data is not null)
        {
            ApplyData(data);
        }
    }

    /// <summary>
    /// Rebuilds the report bands for the provided dataset using the standard detail layout.
    /// Callers can set any of the public properties before invoking this method to influence
    /// colors, footer content, and optional flags.
    /// </summary>
    public void ApplyData(DataSet? data)
    {
        data ??= new DataSet();

        Bands.Clear();

        var normalizedTitleColor = NormalizeHtmlColor(TitleColor);
        var footerLogo = string.IsNullOrWhiteSpace(FooterLogoPath) ? clsGlobal.G_strLogoPath : FooterLogoPath;

        var detailReport = new StandardDetailReport(
            data,
            normalizedTitleColor,
            FooterAddress,
            footerLogo,
            HideLines,
            explicitTitle: ExplicitTitle);

        var detailBand = new DetailBand
        {
            HeightF = detailReport.PageHeight
        };

        detailBand.Controls.Add(new XRSubreport
        {
            ReportSource = detailReport,
            SizeF = new SizeF(PageWidth - Margins.Left - Margins.Right, detailReport.PageHeight)
        });

        Bands.Add(detailBand);
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

        return color.Length == 4
            ? $"#{color[1]}{color[1]}{color[2]}{color[2]}{color[3]}{color[3]}"
            : color;
    }
}

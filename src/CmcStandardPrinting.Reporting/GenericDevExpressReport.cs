using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;

namespace EgsReport;

/// <summary>
/// C# port of the legacy VB <c>clsGenericDevExpress</c> helper used for master plan
/// printing. The goal is to preserve the existing DevExpress pipeline while we migrate
/// the remaining report layouts.
/// </summary>
public class GenericDevExpressReport
{
    private static readonly Font DayLabelFont = new("Arial", 12, FontStyle.Bold);
    private static readonly Font DetailBoldFont = new("Arial", 8, FontStyle.Bold);
    private static readonly Font DetailRegularFont = new("Arial", 8, FontStyle.Regular);

    private const int DefaultPageHeight = 2101; // tenths of a millimeter
    private const int DefaultPageWidth = 2970;
    private const int ReportMargin = 254;

    /// <summary>
    /// Renders a weekly master plan report with one column per plan and one row per restaurant.
    /// Returns an empty string to mirror the legacy contract, or a message when the dataset is
    /// missing the required tables.
    /// </summary>
    public string printMP(DataSet dsMain, string strFileNamePDF)
    {
        var (restaurants, masterPlans, planValues, dates, error) = ExtractMasterPlanTables(dsMain);
        if (error is not null)
        {
            return error;
        }

        var report = CreateReportShell();
        var availableHeight = report.PageHeight - (report.Margins.Top + report.Margins.Bottom);
        var availableWidth = report.PageWidth - (report.Margins.Left + report.Margins.Right);

        var detail = new DetailBand { HeightF = availableHeight };
        var dayHeight = availableHeight / 6f;

        for (var dayPlan = 1; dayPlan <= 6; dayPlan++)
        {
            var panel = BuildMasterPlanPanel(
                restaurants!,
                masterPlans!,
                planValues!,
                dates!,
                dayPlan,
                dayHeight,
                availableWidth);
            panel.LocationF = new DevExpress.Utils.PointFloat(0, (dayPlan - 1) * dayHeight);
            detail.Controls.Add(panel);
        }

        report.Bands.Add(detail);

        EnsureDirectory(strFileNamePDF);
        report.ExportToPdf(strFileNamePDF);
        return string.Empty;
    }

    /// <summary>
    /// Single-page variant that keeps all rows together on one panel. This mirrors the legacy
    /// helper that coalesced multiple days for compact rendering.
    /// </summary>
    public string printMPSinglePage(DataSet dsMain, string strFileNamePDF)
    {
        var (restaurants, masterPlans, planValues, dates, error) = ExtractMasterPlanTables(dsMain);
        if (error is not null)
        {
            return error;
        }

        var report = CreateReportShell();
        var availableHeight = report.PageHeight - (report.Margins.Top + report.Margins.Bottom);
        var availableWidth = report.PageWidth - (report.Margins.Left + report.Margins.Right);

        var detail = new DetailBand { HeightF = availableHeight };
        var panel = BuildMasterPlanPanel(
            restaurants!,
            masterPlans!,
            planValues!,
            dates!,
            1,
            availableHeight,
            availableWidth,
            includeAllDays: true);
        detail.Controls.Add(panel);
        report.Bands.Add(detail);

        EnsureDirectory(strFileNamePDF);
        report.ExportToPdf(strFileNamePDF);
        return string.Empty;
    }

    private static XtraReport CreateReportShell()
    {
        return new XtraReport
        {
            ReportUnit = ReportUnit.TenthsOfAMillimeter,
            PaperKind = PaperKind.Custom,
            PageWidth = DefaultPageWidth,
            PageHeight = DefaultPageHeight,
            Landscape = false,
            Margins = new Margins(ReportMargin, ReportMargin, ReportMargin, ReportMargin)
        };
    }

    private static XRPanel BuildMasterPlanPanel(
        DataTable restaurants,
        DataTable masterPlans,
        DataTable planValues,
        DataTable dates,
        int dayPlan,
        float height,
        float width,
        bool includeAllDays = false)
    {
        var panel = new XRPanel
        {
            Dpi = 254f,
            SizeF = new SizeF(width, height)
        };

        var currentY = 0f;
        var dayLabel = ResolveDayLabel(dates, dayPlan);
        var dayLabelHeight = ReportingTextUtils.MeasureText(
            dayLabel,
            DayLabelFont,
            800,
            StringFormat.GenericDefault,
            panel.Padding).Height;

        panel.Controls.Add(CreateLabel(dayLabel, DayLabelFont, 0, 0, 600, dayLabelHeight, TextAlignment.TopLeft, bold: true));
        currentY += dayLabelHeight + 10f;

        panel.Controls.Add(new XRLine
        {
            Dpi = 254f,
            LineStyle = System.Drawing.Drawing2D.DashStyle.Solid,
            LineWidth = 1,
            LocationF = new DevExpress.Utils.PointFloat(0, currentY),
            SizeF = new SizeF(width, 2)
        });
        currentY += 12f;

        // Column headers
        var rowHeight = ReportingTextUtils.MeasureText("Restaurant", DetailBoldFont, 600, StringFormat.GenericDefault, panel.Padding)
            .Height;

        panel.Controls.Add(CreateLabel("N.R.", DetailBoldFont, 0, currentY, 100, rowHeight, TextAlignment.TopLeft));
        panel.Controls.Add(CreateLabel("Restaurant", DetailBoldFont, 110, currentY, 600, rowHeight, TextAlignment.TopLeft));

        var valueAreaWidth = width - 755;
        var colWidth = masterPlans.Rows.Count == 0 ? valueAreaWidth : valueAreaWidth / masterPlans.Rows.Count;
        var headerWidth = (colWidth + 5) * masterPlans.Rows.Count;
        var currentX = width - headerWidth;

        foreach (DataRow plan in masterPlans.Rows)
        {
            var title = Convert.ToString(plan["name"]) ?? string.Empty;
            var headerHeight = ReportingTextUtils.MeasureText(title, DetailBoldFont, (int)colWidth, StringFormat.GenericDefault, panel.Padding)
                .Height;
            panel.Controls.Add(CreateLabel(title, DetailBoldFont, currentX, currentY, colWidth, headerHeight, TextAlignment.TopLeft));
            rowHeight = Math.Max(rowHeight, headerHeight);
            currentX += colWidth + 5;
        }

        currentY += rowHeight + 15f;
        panel.Controls.Add(new XRLine
        {
            Dpi = 254f,
            LineStyle = System.Drawing.Drawing2D.DashStyle.Solid,
            LineWidth = 5,
            LocationF = new DevExpress.Utils.PointFloat(0, currentY),
            SizeF = new SizeF(width, 6)
        });
        currentY += 15f;

        var restaurantIndex = 1;
        foreach (DataRow restaurant in restaurants.Rows)
        {
            var numberHeight = ReportingTextUtils.MeasureText(restaurantIndex.ToString(), DetailBoldFont, 150, StringFormat.GenericDefault, panel.Padding)
                .Height;
            panel.Controls.Add(CreateLabel(restaurantIndex.ToString(), DetailBoldFont, 0, currentY, 100, numberHeight, TextAlignment.TopLeft));

            var name = Convert.ToString(restaurant["name"]) ?? string.Empty;
            var nameHeight = ReportingTextUtils.MeasureText(name, DetailBoldFont, 600, StringFormat.GenericDefault, panel.Padding)
                .Height;
            panel.Controls.Add(CreateLabel(name, DetailBoldFont, 110, currentY, 600, nameHeight, TextAlignment.TopLeft));

            var detailHeight = Math.Max(numberHeight, nameHeight);
            currentX = width - headerWidth;

            foreach (DataRow plan in masterPlans.Rows)
            {
                var valueText = LookupPlanValue(planValues, restaurant, plan, dayPlan, includeAllDays);
                var valueHeight = ReportingTextUtils.MeasureText(valueText, DetailRegularFont, (int)colWidth, StringFormat.GenericDefault, panel.Padding)
                    .Height;
                valueHeight = Math.Max(40, valueHeight);

                panel.Controls.Add(CreateLabel(
                    valueText,
                    DetailRegularFont,
                    currentX,
                    currentY,
                    colWidth,
                    valueHeight,
                    TextAlignment.TopCenter));

                detailHeight = Math.Max(detailHeight, valueHeight);
                currentX += colWidth + 5;
            }

            currentY += detailHeight + 5f;
            restaurantIndex++;
        }

        return panel;
    }

    private static XRLabel CreateLabel(
        string text,
        Font font,
        float x,
        float y,
        float width,
        float height,
        TextAlignment alignment,
        bool bold = false)
    {
        return new XRLabel
        {
            Font = bold ? new Font(font, FontStyle.Bold) : font,
            Text = text,
            SizeF = new SizeF(width, height),
            LocationF = new DevExpress.Utils.PointFloat(x, y),
            TextAlignment = alignment,
            Padding = new PaddingInfo(2, 2, 0, 0),
            WordWrap = true,
            CanGrow = true,
            CanShrink = false
        };
    }

    private static string LookupPlanValue(DataTable planValues, DataRow restaurant, DataRow plan, int dayPlan, bool includeAllDays)
    {
        var restaurantCode = restaurant.Table.Columns.Contains("coderestaurant")
            ? Convert.ToString(restaurant["coderestaurant"]) ?? string.Empty
            : string.Empty;
        var planCode = plan.Table.Columns.Contains("codemasterplan")
            ? Convert.ToString(plan["codemasterplan"]) ?? string.Empty
            : string.Empty;

        if (string.IsNullOrWhiteSpace(restaurantCode) || string.IsNullOrWhiteSpace(planCode))
        {
            return string.Empty;
        }

        var filter = includeAllDays
            ? $"coderestaurant='{restaurantCode}' and codemasterplan='{planCode}'"
            : $"coderestaurant='{restaurantCode}' and codemasterplan='{planCode}' and dayplan={dayPlan}";

        var rows = planValues.Select(filter);
        if (rows.Length == 0)
        {
            return string.Empty;
        }

        return Convert.ToString(rows[0]["planvalue1"]) ?? string.Empty;
    }

    private static string ResolveDayLabel(DataTable dates, int dayPlan)
    {
        var dayName = dayPlan switch
        {
            1 => "Monday",
            2 => "Tuesday",
            3 => "Wednesday",
            4 => "Thursday",
            5 => "Friday",
            6 => "Saturday",
            _ => "Day"
        };

        if (dates.Rows.Count == 0 || !dates.Columns.Contains("startdate"))
        {
            return dayName;
        }

        var baseDate = dates.Rows[0]["startdate"];
        if (DateTime.TryParse(Convert.ToString(baseDate), out var parsed))
        {
            return $"{dayName} {parsed.Date.AddDays(dayPlan - 1):d}";
        }

        return dayName;
    }

    private static void EnsureDirectory(string outputPath)
    {
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    private static (
        DataTable? restaurants,
        DataTable? masterPlans,
        DataTable? planValues,
        DataTable? dates,
        string? error) ExtractMasterPlanTables(DataSet ds)
    {
        if (ds.Tables.Count < 4)
        {
            return (null, null, null, null, "Missing master plan tables");
        }

        return (ds.Tables[0], ds.Tables[1], ds.Tables[2], ds.Tables[3], null);
    }
}

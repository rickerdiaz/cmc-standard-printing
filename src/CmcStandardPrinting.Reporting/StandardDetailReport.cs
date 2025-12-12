using System;
using System.Data;
using System.Drawing;
using System.IO;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;

namespace EgsReport;

/// <summary>
/// Minimal DevExpress report that mirrors the legacy standard detail layout. The goal is to
/// keep the old printer pipeline working while we incrementally rebuild the richer VB-based
/// layout logic.
/// </summary>
public class StandardDetailReport : XtraReport
{
    public StandardDetailReport(
        DataSet? data,
        string titleColor,
        string footerAddress,
        string footerLogoPath,
        bool hideLines,
        string? explicitTitle = null,
        double? availableWidth = null,
        ReportOptions? fontOptions = null)
    {
        DataSource = data;

        var profileTable = data?.Tables.Count > 0 ? data.Tables[0] : null;
        var detailTable = data?.Tables.Count > 1 ? data.Tables[1] : profileTable;

        var reportTitle = ResolveTitle(explicitTitle, profileTable);

        var layoutWidth = ResolveWidth(availableWidth);

        CreateMargins();
        CreateReportHeader(reportTitle, titleColor, footerLogoPath, fontOptions);
        CreatePageHeader(detailTable, hideLines, layoutWidth, fontOptions);
        CreateDetail(detailTable, hideLines, layoutWidth, fontOptions);
        CreateFooter(footerAddress, fontOptions);

        if (detailTable != null)
        {
            DataMember = detailTable.TableName;
        }
    }

    private void CreateMargins()
    {
        TopMargin = new TopMarginBand();
        BottomMargin = new BottomMarginBand();
        Bands.AddRange(new Band[] { TopMargin, BottomMargin });
    }

    private static double ResolveWidth(double? availableWidth)
    {
        if (availableWidth.HasValue && availableWidth.Value > 0)
        {
            return availableWidth.Value;
        }

        // fallback to a sensible legacy-friendly width
        return 750;
    }

    private void CreateReportHeader(
        string title,
        string titleColor,
        string footerLogoPath,
        ReportOptions? options)
    {
        var header = new ReportHeaderBand { HeightF = 70f };

        var titleFont = ResolveFont(
            options?.strFontTitleName,
            options?.sgFontTitleSize > 0 ? options.sgFontTitleSize : 16,
            FontStyle.Bold,
            new Font("Arial", 16, FontStyle.Bold));

        var label = new XRLabel
        {
            Text = title,
            Font = titleFont,
            BoundsF = new RectangleF(0, 0, 600, 30),
            Padding = new PaddingInfo(4, 4, 4, 4)
        };

        if (!string.IsNullOrWhiteSpace(titleColor))
        {
            try
            {
                label.ForeColor = ColorTranslator.FromHtml(titleColor);
            }
            catch
            {
                // fall back to the default color if parsing fails
            }
        }

        header.Controls.Add(label);

        var logoPath = footerLogoPath ?? string.Empty;
        if (File.Exists(logoPath))
        {
            var logo = new XRPictureBox
            {
                ImageUrl = logoPath,
                Sizing = ImageSizeMode.Squeeze,
                BoundsF = new RectangleF(620, 0, 120, 40)
            };
            header.Controls.Add(logo);
        }

        Bands.Add(header);
    }

    private void CreatePageHeader(
        DataTable? detailTable,
        bool hideLines,
        double tableWidth,
        ReportOptions? options)
    {
        if (detailTable == null || detailTable.Columns.Count == 0)
        {
            return;
        }

        var header = new PageHeaderBand { HeightF = 24f };
        var headerTable = new XRTable { BoundsF = new RectangleF(0, 0, (float)tableWidth, 24f) };
        var headerRow = new XRTableRow();

        var headerFont = ResolveFont(
            options?.strFontName,
            options?.sgFontSize > 0 ? options.sgFontSize : 9,
            FontStyle.Bold,
            new Font("Arial", 9, FontStyle.Bold));

        foreach (DataColumn column in detailTable.Columns)
        {
            var cell = new XRTableCell
            {
                Text = column.ColumnName,
                Font = headerFont,
                Padding = new PaddingInfo(4, 4, 4, 4)
            };

            if (!hideLines)
            {
                cell.Borders = BorderSide.Bottom;
            }

            headerRow.Cells.Add(cell);
        }

        headerTable.Rows.Add(headerRow);
        header.Controls.Add(headerTable);
        Bands.Add(header);
    }

    private void CreateDetail(
        DataTable? detailTable,
        bool hideLines,
        double tableWidth,
        ReportOptions? options)
    {
        var detailBand = new DetailBand();

        if (detailTable == null || detailTable.Columns.Count == 0)
        {
            detailBand.Controls.Add(new XRLabel
            {
                Text = "No data available",
                Font = new Font("Arial", 10),
                Padding = new PaddingInfo(4, 4, 4, 4)
            });
            Bands.Add(detailBand);
            return;
        }

        var table = new XRTable { BoundsF = new RectangleF(0, 0, (float)tableWidth, 20f) };
        var row = new XRTableRow();

        var detailFont = ResolveFont(
            options?.strFontName,
            options?.sgFontSize > 0 ? options.sgFontSize : 9,
            FontStyle.Regular,
            new Font("Arial", 9));

        foreach (DataColumn column in detailTable.Columns)
        {
            var cell = new XRTableCell
            {
                Font = detailFont,
                Padding = new PaddingInfo(4, 4, 4, 4),
                ExpressionBindings =
                {
                    new ExpressionBinding("BeforePrint", "Text", $"[{column.ColumnName}]")
                }
            };

            if (!hideLines)
            {
                cell.Borders = BorderSide.Bottom;
            }

            row.Cells.Add(cell);
        }

        table.Rows.Add(row);
        detailBand.Controls.Add(table);
        Bands.Add(detailBand);
    }

    private void CreateFooter(string footerAddress, ReportOptions? options)
    {
        var footer = new PageFooterBand { HeightF = 40f };

        var footerFont = ResolveFont(
            options?.strFontName,
            options?.sgFontSize > 0 ? options.sgFontSize : 8,
            FontStyle.Regular,
            new Font("Arial", 8));

        if (!string.IsNullOrWhiteSpace(footerAddress))
        {
            footer.Controls.Add(new XRLabel
            {
                Text = footerAddress,
                Font = footerFont,
                BoundsF = new RectangleF(0, 0, 500, 20),
                Padding = new PaddingInfo(4, 4, 4, 4)
            });
        }

        var pageInfo = new XRPageInfo
        {
            PageInfo = PageInfo.NumberOfTotal,
            BoundsF = new RectangleF(650, 0, 100, 20),
            TextAlignment = TextAlignment.MiddleRight,
            Font = footerFont
        };
        footer.Controls.Add(pageInfo);

        Bands.Add(footer);
    }

    private static string ResolveTitle(string? explicitTitle, DataTable? profileTable)
    {
        if (!string.IsNullOrWhiteSpace(explicitTitle))
        {
            return explicitTitle;
        }

        if (profileTable?.Rows.Count > 0)
        {
            var firstRow = profileTable.Rows[0];
            if (profileTable.Columns.Contains("Name"))
            {
                return Convert.ToString(firstRow["Name"]) ?? "Standard Printing";
            }
            if (profileTable.Columns.Contains("Title"))
            {
                return Convert.ToString(firstRow["Title"]) ?? "Standard Printing";
            }
        }

        return "Standard Printing";
    }

    private static Font ResolveFont(string? preferredName, float size, FontStyle style, Font fallback)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(preferredName))
            {
                return new Font(preferredName, size, style);
            }
        }
        catch (ArgumentException)
        {
            // fall back to legacy defaults if the requested font is unavailable
        }

        return fallback;
    }
}

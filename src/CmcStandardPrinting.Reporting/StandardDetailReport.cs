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
        string? explicitTitle = null)
    {
        DataSource = data;

        var profileTable = data?.Tables.Count > 0 ? data.Tables[0] : null;
        var detailTable = data?.Tables.Count > 1 ? data.Tables[1] : profileTable;

        var reportTitle = ResolveTitle(explicitTitle, profileTable);

        CreateMargins();
        CreateReportHeader(reportTitle, titleColor, footerLogoPath);
        CreatePageHeader(detailTable, hideLines);
        CreateDetail(detailTable, hideLines);
        CreateFooter(footerAddress);

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

    private void CreateReportHeader(string title, string titleColor, string footerLogoPath)
    {
        var header = new ReportHeaderBand { HeightF = 70f };

        var label = new XRLabel
        {
            Text = title,
            Font = new Font("Arial", 16, FontStyle.Bold),
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

    private void CreatePageHeader(DataTable? detailTable, bool hideLines)
    {
        if (detailTable == null || detailTable.Columns.Count == 0)
        {
            return;
        }

        var pageHeader = new PageHeaderBand { HeightF = 24f };
        var headerTable = new XRTable { BoundsF = new RectangleF(0, 0, 750, 24f) };
        var headerRow = new XRTableRow();

        foreach (DataColumn column in detailTable.Columns)
        {
            var cell = new XRTableCell
            {
                Text = column.ColumnName,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Padding = new PaddingInfo(4, 4, 4, 4)
            };

            if (!hideLines)
            {
                cell.Borders = BorderSide.Bottom;
            }

            headerRow.Cells.Add(cell);
        }

        headerTable.Rows.Add(headerRow);
        pageHeader.Controls.Add(headerTable);
        Bands.Add(pageHeader);
    }

    private void CreateDetail(DataTable? detailTable, bool hideLines)
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

        var table = new XRTable { BoundsF = new RectangleF(0, 0, 750, 20f) };
        var row = new XRTableRow();

        foreach (DataColumn column in detailTable.Columns)
        {
            var cell = new XRTableCell
            {
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

    private void CreateFooter(string footerAddress)
    {
        var footer = new PageFooterBand { HeightF = 40f };

        if (!string.IsNullOrWhiteSpace(footerAddress))
        {
            footer.Controls.Add(new XRLabel
            {
                Text = footerAddress,
                Font = new Font("Arial", 8),
                BoundsF = new RectangleF(0, 0, 500, 20),
                Padding = new PaddingInfo(4, 4, 4, 4)
            });
        }

        var pageInfo = new XRPageInfo
        {
            PageInfo = PageInfo.NumberOfTotal,
            BoundsF = new RectangleF(650, 0, 100, 20),
            TextAlignment = TextAlignment.MiddleRight,
            Font = new Font("Arial", 8)
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
}

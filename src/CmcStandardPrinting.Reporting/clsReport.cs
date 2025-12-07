using System.Data;
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
        return BuildBasicReport(strPhotoPath, strLogoPath, strConnection, udtUser, null, ref documentOutput);
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
        return BuildBasicReport(strPhotoPath, strLogoPath, strConnection, udtUser, ds2, ref documentOutput);
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

        return BuildBasicReport(strPhotoPath, strLogoPath, strConnection, udtUser, ds2, ref documentOutput);
    }

    private XtraReport BuildBasicReport(
        string strPhotoPath,
        string strLogoPath,
        string strConnection,
        structUser udtUser,
        DataSet? data,
        ref int documentOutput)
    {
        clsGlobal.G_strPhotoPath = strPhotoPath;
        clsGlobal.G_strLogoPath = strLogoPath;
        clsGlobal.G_strLogoPath2 = strLogoPath;

        if (documentOutput == 0)
        {
            documentOutput = (int)enumFileType.PDF;
        }

        var report = new XtraReport
        {
            DataSource = data
        };

        var detailBand = new DetailBand();
        var header = new XRLabel
        {
            Text = string.IsNullOrWhiteSpace(TitleColor)
                ? "Standard Printing"
                : $"Standard Printing - {TitleColor}",
            Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold),
            WidthF = 500
        };
        detailBand.Controls.Add(header);
        report.Bands.Add(detailBand);

        if (data?.Tables.Count > 0)
        {
            report.DataMember = data.Tables[0].TableName;
            report.CreateDocument(false);
        }

        _logger.LogDebug(
            "Report prepared. PhotoPath={PhotoPath}, LogoPath={LogoPath}, Connection={Connection}",
            strPhotoPath,
            strLogoPath,
            strConnection);

        return report;
    }
}

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
        object udtUser,
        string strConnection,
        ref int documentOutput,
        string strPhotoPath,
        string strLogoPath = "",
        int intFoodlaw = 1)
    {
        _logger.LogInformation("CreateReport called for print list {PrintList}", intCodePrintList);
        return BuildBasicReport(strPhotoPath, strLogoPath, strConnection, null);
    }

    public XtraReport CreateReport(
        DataSet ds2,
        object udtUser,
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
        return BuildBasicReport(strPhotoPath, strLogoPath, strConnection, ds2);
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

        return BuildBasicReport(strPhotoPath, strLogoPath, strConnection, ds2);
    }

    private XtraReport BuildBasicReport(string strPhotoPath, string strLogoPath, string strConnection, DataSet? data)
    {
        var report = new XtraReport
        {
            DataSource = data
        };

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

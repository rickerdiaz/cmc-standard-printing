using System;
using System.Collections.Generic;
using System.Data;
<<<<<<< HEAD
using Microsoft.Data.SqlClient;
=======
using System.Data.SqlClient;
>>>>>>> main
using CmcStandardPrinting.Domain.Common;
using CmcStandardPrinting.Domain.MenuPlans;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuPlanController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<MenuPlanController> _logger;

    public MenuPlanController(IConfiguration configuration, ILogger<MenuPlanController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("menuplaninfo/{codemenuplan:int}/{codetrans:int}/{copiedInfo:bool?}")]
    public ActionResult<DataTable> GetMenuPlanInfo(int codemenuplan, int codetrans, bool copiedInfo = false)
    {
        try
        {
            var dt = new DataTable();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "MP_GETMenuPlanInfo";
            cmd.Parameters.Add("@CodeMenuPlan", SqlDbType.Int).Value = codemenuplan;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@Copy", SqlDbType.Bit).Value = copiedInfo;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return Ok(dt);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid parameters when retrieving menu plan info");
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving menu plan info");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("menuplan/restaurant/{codesite:int?}/{coderestaurant:int?}/{namerestaurant?}")]
    public ActionResult<DataTable> GetRestaurant(int codesite = -1, int coderestaurant = -1, string namerestaurant = "")
    {
        try
        {
            var dt = new DataTable();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "MP_GETRestaurant";
            cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = coderestaurant;
            cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar).Value = namerestaurant ?? string.Empty;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return Ok(dt);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid parameters when retrieving restaurants");
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving restaurants");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("menuplan/masterplan/{coderestaurant:int?}/{codetrans:int?}")]
    public ActionResult<DataTable> GetMasterPlan(int coderestaurant = -1, int codetrans = -1)
    {
        try
        {
            var dt = new DataTable();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "MP_GetMasterPlan";
            cmd.Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = coderestaurant;
            cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = codetrans;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return Ok(dt);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid parameters when retrieving master plans");
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving master plans");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost("menuplan/delete")]
    public ActionResult<ResponseCallBack> DeleteMenuPlan([FromBody] GenericDeleteData data)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandText = "API_DELETE_MENUPLAN";
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            foreach (var del in data.Codes)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CodeMenuPlan", SqlDbType.Int).Value = del;
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
                cmd.ExecuteNonQuery();
            }

            if (resultCode != 0)
            {
                return StatusCode(500, Fail(response, resultCode, "Delete menu plan failed"));
            }

            return Ok(Success(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete menu plan");
            if (resultCode == 0) resultCode = 500;
            return StatusCode(500, Fail(response, resultCode, "Delete menu plan failed"));
        }
    }

    [HttpPost("menuplan/copy")]
    public ActionResult<ResponseCallBack> CopyMenuPlan([FromBody] MenuPlan data)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            var masterPlanList = string.Empty;
            foreach (var map in data.Source)
            {
                if (map.CodeMasterPlan > 0)
                {
                    masterPlanList += $"{map.CodeMasterPlanSource}:{map.CodeMasterPlan},";
                }
            }

            masterPlanList = masterPlanList.TrimEnd(',');

            cmd.Connection = cn;
            cmd.CommandText = "MP_COPYMenuPlan2";
            cmd.CommandType = CommandType.StoredProcedure;
            cn.Open();
            var pCode = cmd.Parameters.Add("@CodeMenuPlan", SqlDbType.Int);
            pCode.Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@CodeMenuPlanSrc", SqlDbType.Int).Value = data.CopiedFromMpCode;
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.CodeTrans;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = data.Name ?? string.Empty;
            cmd.Parameters.Add("@Number", SqlDbType.NVarChar).Value = data.Number ?? string.Empty;
            cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = data.Description ?? string.Empty;
            cmd.Parameters.Add("@CodeRestaurant", SqlDbType.Int).Value = data.CopyRestaurant ? -1 : data.CodeRestaurantTo;
            cmd.Parameters.Add("@CyclePlan", SqlDbType.Bit).Value = data.CyclePlan;
            cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = GetDate(data.StartDate);
            cmd.Parameters.Add("@CodeCategory", SqlDbType.Int).Value = data.CodeCategory;
            cmd.Parameters.Add("@CodeSeason", SqlDbType.Int).Value = data.CodeSeason;
            cmd.Parameters.Add("@CodeService", SqlDbType.Int).Value = data.CodeService;
            cmd.Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = data.CodeSetPrice;
            cmd.Parameters.Add("@Duration", SqlDbType.Int).Value = data.Duration;
            cmd.Parameters.Add("@Recurrence", SqlDbType.Int).Value = data.Recurrence;
            cmd.Parameters.Add("@MasterPlanList", SqlDbType.NVarChar).Value = masterPlanList;
            cmd.ExecuteNonQuery();
            resultCode = GetInt(pCode.Value, -1);
            if (resultCode < 0)
            {
                return StatusCode(500, Fail(response, resultCode, "Copy menu plan failed"));
            }

            return Ok(Success(response, resultCode));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to copy menu plan");
            if (resultCode == 0) resultCode = 500;
            return StatusCode(500, Fail(response, resultCode, "Copy menu plan failed"));
        }
    }

    [HttpGet("menuplan/export/costmargin/{codemainmenu:int?}/{coderestaurant:int?}/{codetrans:int?}/{baseurl?}/{codeSetPrice:int?}/{culture?}/{CodeUser:int?}/{margin1:double?}/{margin2:double?}/{dayTotalCost:double?}")]
    public ActionResult<ResponseCallBack> GetMscMenuRecipeCostExport(
        int codemainmenu,
        int coderestaurant,
        int codetrans,
        string baseurl,
        int codeSetPrice,
        string culture,
        int CodeUser,
        double margin1,
        double margin2,
        double dayTotalCost)
    {
        var response = new ResponseCallBack();
        try
        {
            var report = new MenuPlanReportExporter();
            var folderPath = _configuration.GetValue<string>("ReportFolder") ?? string.Empty;
            var reportUrl = _configuration.GetValue<string>("ReportURL") ?? string.Empty;
            var ds = new DataSet();
            var userConnectionString = GetUserConnectionString(CodeUser);

            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(userConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Report_MenuPlan_Export_RecipeCostMargin]";
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Add("@CodeMainMenu", SqlDbType.Int).Value = codemainmenu;
            cmd.Parameters.Add("@CodeRestaurant", SqlDbType.Int).Value = coderestaurant;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@BaseURL", SqlDbType.VarChar).Value = baseurl ?? string.Empty;
            cmd.Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = codeSetPrice;
            cmd.Parameters.Add("@Culture", SqlDbType.VarChar).Value = culture ?? string.Empty;
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = CodeUser;
            cmd.Parameters.Add("@Margin1", SqlDbType.Float).Value = margin1;
            cmd.Parameters.Add("@Margin2", SqlDbType.Float).Value = margin2;
            cmd.Parameters.Add("@DayTotalCost", SqlDbType.Float).Value = dayTotalCost;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            var result = report.GetExportMenuPlanRecipeCostMargin(ds, folderPath, reportUrl);
            return Ok(Success(response, result));
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid parameters when exporting menu plan recipe cost");
            return BadRequest(Fail(new ResponseCallBack(), 400, "Missing or invalid parameters"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error exporting menu plan recipe cost");
            return StatusCode(500, Fail(new ResponseCallBack(), 500, "Unexpected error occured"));
        }
    }

    private sealed class MenuPlanReportExporter
    {
        public object GetExportMenuPlanRecipeCostMargin(DataSet dataSet, string folderPath, string reportUrl)
        {
            return new
            {
                FolderPath = folderPath,
                ReportUrl = reportUrl,
                Tables = dataSet.Tables.Count
            };
        }
    }

    private static ResponseCallBack Success(ResponseCallBack r, object? returnValue = null)
    {
        r.Code = 0;
        r.Message = "OK";
        r.Status = true;
        r.ReturnValue = returnValue;
        return r;
    }

    private static ResponseCallBack Fail(ResponseCallBack r, int code, string message)
    {
        r.Code = code;
        r.Message = message;
        r.Status = false;
        return r;
    }

    private static DateTime GetDate(string? s)
    {
        if (DateTime.TryParse(s, out var d)) return d;
        return DateTime.Now;
    }

    private static int GetInt(object? value, int fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (int.TryParse(Convert.ToString(value), out var i)) return i;
        try { return Convert.ToInt32(value); } catch { return fallback; }
    }

    private string GetUserConnectionString(int codeUser)
    {
        var mainDbConnectionEncrypted = _configuration.GetConnectionString("MainDB") ?? string.Empty;
        var decryptedMainDb = PrinterControllerDecrypt(mainDbConnectionEncrypted);
        if (string.IsNullOrWhiteSpace(decryptedMainDb))
        {
            throw new InvalidOperationException("Main DB connection string is missing");
        }

        var ds = new DataSet();
        using (var cmd = new SqlCommand())
        using (var cn = new SqlConnection(decryptedMainDb))
        {
            cmd.Connection = cn;
            cmd.CommandText = "[dbo].[Kiosk_API_Get_UserByUserCode]";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@UserCode", SqlDbType.Int).Value = codeUser;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
        }

        if (ds.Tables.Count < 2 || ds.Tables[1].Rows.Count == 0)
        {
            throw new InvalidOperationException("No connection data returned for the specified user.");
        }

        var r = ds.Tables[1].Rows[0];
        var dataSourceEnc = Convert.ToString(r["DataSource"]);
        var initialCatalogEnc = Convert.ToString(r["InitialCatalog"]);
        var userIdEnc = Convert.ToString(r["User_ID"]);
        var passwordEnc = Convert.ToString(r["Password"]);

        var dataSource = PrinterControllerDecrypt(dataSourceEnc ?? string.Empty);
        var initialCatalog = PrinterControllerDecrypt(initialCatalogEnc ?? string.Empty);
        var userId = PrinterControllerDecrypt(userIdEnc ?? string.Empty);
        var password = PrinterControllerDecrypt(passwordEnc ?? string.Empty);

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = dataSource,
            InitialCatalog = initialCatalog,
            UserID = userId,
            Password = password,
            IntegratedSecurity = false
        };

        var connString = builder.ConnectionString;
        var debugEnabled = _configuration.GetValue("DebugEnabled", false);
        var debugConnection = _configuration.GetValue<string>("DebugConnection");
        if (debugEnabled && !string.IsNullOrWhiteSpace(debugConnection))
        {
            connString = debugConnection!;
        }

        return connString;
    }

    private static string PrinterControllerDecrypt(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var iv = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        var key = System.Text.Encoding.UTF8.GetBytes("&%#@?,:*");
        try
        {
            using var des = System.Security.Cryptography.DES.Create();
            var inputBytes = Convert.FromBase64String(text.Replace(" ", "+"));
            using var ms = new System.IO.MemoryStream();
            using (var cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateDecryptor(key, iv), System.Security.Cryptography.CryptoStreamMode.Write))
            {
                cs.Write(inputBytes, 0, inputBytes.Length);
                cs.FlushFinalBlock();
                return System.Text.Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        catch
        {
            return string.Empty;
        }
    }
}

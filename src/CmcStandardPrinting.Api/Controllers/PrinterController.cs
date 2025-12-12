using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
<<<<<<< HEAD
using Microsoft.Data.SqlClient;
=======
using System.Data.SqlClient;
>>>>>>> main
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CmcStandardPrinting.Domain.Printers;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrinterController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PrinterController> _logger;

    public PrinterController(IConfiguration configuration, ILogger<PrinterController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpPost("search")]
    public ActionResult<List<Printer>> GetPrinterConfigByName([FromBody] ConfigurationSearch data)
    {
        try
        {
            var ds = new DataSet();
            using (var cmd = new SqlCommand())
            using (var cn = new SqlConnection(ConnectionString))
            {
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[dbo].[API_MANAGE_Generic]";
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                    cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWPRINTERCONFIG";
                    cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = data.CodeProperty;
                    cn.Open();
                    using var da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                }
                finally
                {
                    cn.Close();
                }
            }

            var printers = new List<Printer>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    printers.Add(new Printer
                    {
                        Code = GetInt(r["ID"]),
                        Name = GetStr(r["Name"]),
                        IsGlobal = GetBool(r["IsGlobal"]),
                        CodeSaleSite = GetInt(r["CodeSaleSite"]),
                        SaleSiteName = GetStr(r["SaleSiteName"]),
                        Status = GetInt(r["Status"])
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(data.Name))
            {
                var result = new List<Printer>();
                foreach (var word in data.Name.Trim().Split(','))
                {
                    var w = (word ?? string.Empty).Trim();
                    if (w.Length == 0) continue;
                    foreach (var s in printers)
                    {
                        if (s.Name?.ToLowerInvariant().Contains(w.ToLowerInvariant()) == true)
                        {
                            result.Add(s);
                        }
                    }
                }
                printers = result;
            }

            return Ok(printers);
        }
        catch (ArgumentException aex)
        {
            LogWarn("GetPrinterConfigByName: Missing or invalid parameters", aex);
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            LogError("GetPrinterConfigByName: Unexpected error occured", ex);
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("sharing/{codesite:int}/{tree:int}/{codecategory:int}")]
    public ActionResult<List<TreeNode>> GetPrinterSharing(int codesite, int tree, int codecategory)
    {
        try
        {
            var ds = new DataSet();
            using (var cmd = new SqlCommand())
            using (var cn = new SqlConnection(ConnectionString))
            {
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandText = "[dbo].[API_GET_SharingPrinter]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                    cmd.Parameters.Add("@CodeCategory", SqlDbType.Int).Value = codecategory;
                    cn.Open();
                    using var da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                }
                finally
                {
                    cn.Close();
                }
            }

            var sharings = new List<GenericTree>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    sharings.Add(new GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        ParentCode = GetInt(r["ParentCode"]),
                        ParentName = GetStr(r["ParentName"]),
                        Flagged = GetBool(r["Flagged"]),
                        Type = GetInt(r["Type"]),
                        Global = GetBool(r["Global"])
                    });
                }
            }

            var sharingdata = new List<TreeNode>();
            var parents = sharings.Where(obj => obj.ParentCode == 0 && obj.Type == 1).OrderBy(obj => obj.Name).ToList();
            foreach (var p in parents)
            {
                var parent = new TreeNode
                {
                    Title = p.Name,
                    Key = p.Code,
                    Icon = false,
                    Children = CreateChildrenSharing(sharings, p.Code),
                    Select = p.Flagged,
                    Selected = p.Flagged,
                    ParentTitle = p.ParentName,
                    GroupLevel = GroupLevel.Property
                };
                if (parent.Children != null && parent.Children.Count > 0)
                    sharingdata.Add(parent);
            }

            return Ok(sharingdata);
        }
        catch (ArgumentException aex)
        {
            LogWarn("GetPrinterSharing: Missing or invalid parameters", aex);
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            LogError("GetPrinterSharing: Unexpected error occured", ex);
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost]
    public ActionResult<ResponseCallBack> SavePrinter([FromBody] PrinterData data)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        SqlTransaction? trans = null;
        try
        {
            LogInfo("SavePrinter: " + JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            var arrSharing = new ArrayList();
            foreach (var sh in data.Sharing)
            {
                if (!arrSharing.Contains(sh.Code)) arrSharing.Add(sh.Code);
            }
            var codeSiteList = "(" + string.Join(",", arrSharing.Cast<object>()) + ")";

            using (var cmd = new SqlCommand())
            using (var cn = new SqlConnection(ConnectionString))
            {
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[MANAGE_PRINTERCONFIGUPDATE]";
                    cmd.Parameters.Clear();
                    var pCode = cmd.Parameters.Add("@Code", SqlDbType.Int);
                    pCode.Value = data.Info.Code;
                    pCode.Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.Add("@CodeGroup", SqlDbType.Int).Value = 0;
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = data.Info.Name;
                    cmd.Parameters.Add("@Status", SqlDbType.Int).Value = data.Info.Status;
                    cmd.Parameters.Add("@CodeAcct", SqlDbType.NVarChar, 25).Value = "";
                    cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.IsGlobal;
                    cmd.Parameters.Add("@CodeSiteList", SqlDbType.NVarChar, 2000).Value = codeSiteList;
                    cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.Profile.Code;
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.Profile.CodeSite;
                    cmd.Parameters.Add("@CodeSaleSite", SqlDbType.Int).Value = data.Info.CodeSaleSite;
                    var ret = cmd.Parameters.Add("@retval", SqlDbType.Int);
                    ret.Direction = ParameterDirection.ReturnValue;

                    cn.Open();
                    trans = cn.BeginTransaction();
                    cmd.Transaction = trans;

                    cmd.ExecuteNonQuery();
                    resultCode = GetInt(ret.Value, -1);
                    if (resultCode != 0)
                    {
                        throw new Exception($"[{resultCode}] Save printer failed");
                    }

                    response.Code = 0;
                    response.Message = "OK";
                    response.ReturnValue = resultCode;
                    response.Status = true;
                    trans.Commit();
                }
                catch (Exception dbEx)
                {
                    LogError("SavePrinter: Database error occured", dbEx);
                    try { trans?.Rollback(); trans?.Dispose(); } catch { }
                    if (resultCode == 0) resultCode = 500;
                    response.Code = resultCode;
                    response.Status = false;
                    response.Message = "Save printer configuration failed";
                    return StatusCode(500, response);
                }
                finally
                {
                    cn.Close();
                }
            }
        }
        catch (ArgumentException aex)
        {
            LogWarn("SavePrinter: Missing or invalid parameters", aex);
            response.Code = 400;
            response.Message = "Missing or invalid parameters";
            response.Parameters = new List<Param> { new() { Name = "data", Value = "PrinterData" } };
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            LogError("SavePrinter: Unexpected error occured", ex);
            response.Status = false;
            response.Message = "Unexpected error occured";
            response.Code = 500;
            return StatusCode(500, response);
        }
        return Ok(response);
    }

    [HttpPost("delete")]
    public ActionResult<ResponseCallBack> DeleteCategory([FromBody] GenericDeleteData data)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        try
        {
            LogInfo("DeleteCategory: " + JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            var arrPrinterCodes = new ArrayList();
            foreach (var c in data.CodeList)
            {
                if (!arrPrinterCodes.Contains(c.Code)) arrPrinterCodes.Add(c.Code);
            }
            var codeList = string.Join(",", arrPrinterCodes.Cast<object>().Select(x => Convert.ToString(x)));

            using (var cmd = new SqlCommand())
            using (var cn = new SqlConnection(ConnectionString))
            {
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandText = "API_DELETE_Generic";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeList;
                    cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWPRINTERCONFIG";
                    cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                    cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = data.ForceDelete;
                    var skipList = cmd.Parameters.Add("@SkipList", SqlDbType.NVarChar, 4000);
                    skipList.Direction = ParameterDirection.Output;
                    var ret = cmd.Parameters.Add("@Return", SqlDbType.Int);
                    ret.Direction = ParameterDirection.ReturnValue;

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    resultCode = GetInt(ret.Value, -1);
                    if (resultCode != 0)
                    {
                        throw new Exception($"[{resultCode}] Delete category failed");
                    }

                    response.Code = 0;
                    response.Message = "OK";
                    response.ReturnValue = GetStr(skipList.Value);
                    response.Status = true;
                }
                catch (Exception dbEx)
                {
                    LogError("DeleteCategory: Database error occured", dbEx);
                    if (resultCode == 0) resultCode = 500;
                    response.Code = resultCode;
                    response.Status = false;
                    response.ReturnValue = GetStr(cmd.Parameters["@SkipList"].Value);
                    response.Message = "Delete category failed";
                    return StatusCode(500, response);
                }
                finally
                {
                    cn.Close();
                }
            }
        }
        catch (ArgumentException aex)
        {
            LogWarn("DeleteCategory: Missing or invalid parameters", aex);
            response.Code = 400;
            response.Message = "Missing or invalid parameters";
            response.Parameters = new List<Param> { new() { Name = "data", Value = "GenericDeleteData" } };
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            LogError("DeleteCategory: Unexpected error occured", ex);
            response.Status = false;
            response.Message = "Unexpected error occured";
            response.Code = 500;
            return StatusCode(500, response);
        }
        return Ok(response);
    }

    [HttpGet("getsalesite")]
    public ActionResult<List<GenericList>> GetCodeSaleSite()
    {
        try
        {
            var ds = new DataSet();
            using (var cmd = new SqlCommand())
            using (var cn = new SqlConnection(ConnectionString))
            {
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[dbo].[API_GET_PrinterSaleSite]";
                    cn.Open();
                    using var da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                }
                finally
                {
                    cn.Close();
                }
            }

            var userAccounts = new List<GenericList>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    userAccounts.Add(new GenericList
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"])
                    });
                }
            }

            return Ok(userAccounts);
        }
        catch (ArgumentException aex)
        {
            LogWarn("GetCodeSaleSite: Missing or invalid parameters", aex);
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            LogError("GetCodeSaleSite: Unexpected error occured", ex);
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost("standardprinting")]
    public ActionResult<object> StandardPrinting([FromBody] StandardPrintingInput inputData)
    {
        try
        {
            LogInfo("standardPrinting started");
            var intCodePrintList = Convert.ToInt32(inputData.IntCodePrintList);
            var codeUser = Convert.ToInt32(inputData.CodeUser);
            var userLocale = string.IsNullOrWhiteSpace(inputData.UserLocale) ? "en-US" : inputData.UserLocale;

            var userConnectionString = GetUserConnectionString(codeUser);

            var cReport = new EgsReport.clsReport();
            var folderPath = _configuration["ReportFolder"] ?? string.Empty;
            var folderURL = _configuration["ReportURL"] ?? string.Empty;
            var picNormal = inputData.ImagePath ?? string.Empty;

            var ds = new DataSet();
            using (var cmd = new SqlCommand())
            using (var cn = new SqlConnection(userConnectionString))
            {
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandText = "[dbo].[sp_EgswPrintListGetListDetails]";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 600;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@intCodePrintList", SqlDbType.Int).Value = intCodePrintList;
                    cmd.Parameters.Add("@blnDeleteAfterFetch", SqlDbType.Bit).Value = false;
                    cmd.Parameters.Add("@blnBestUnitConversion", SqlDbType.Bit).Value = false;
                    cmd.Parameters.Add("@intPictureList", SqlDbType.Int).Value = 0;
                    cmd.Parameters.Add("@intNutCodeSet", SqlDbType.Int).Value = 0;

                    cn.Open();
                    using var da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                }
                finally
                {
                    cn.Close();
                }
            }

            var hasData = ds != null && ds.Tables.Count > 0;
            if (hasData)
            {
                var strFilename = SubGetFileName();
                var outputPath = Path.Combine(folderPath, strFilename);
                var outDir = Path.GetDirectoryName(outputPath) ?? folderPath;
                if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);

                var documentOutput = 1;
                XtraReport report = cReport.CreateReport_CMC(
                    ds,
                    userConnectionString,
                    ref documentOutput,
                    picNormal,
                    "",
                    "",
                    "",
                    false,
                    false,
                    intFoodlaw: 2,
                    CodePrintList: intCodePrintList,
                    userLocale: userLocale,
                    codeUser: codeUser
                );

                report.ExportToPdf(outputPath);

                folderURL = folderURL.TrimEnd('/') + "/" + strFilename;
                LogInfo("FolderPath: " + outputPath);
                LogInfo("FolderURL: " + folderURL);

                return Ok(folderURL);
            }

            LogWarn("standardPrinting: dataset is empty; skipping report generation.", new Exception("Empty dataset"));
            return Ok(string.Empty);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, ex);
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    private static List<TreeNode> CreateChildrenSharing(List<GenericTree> sharings, int parentCode)
    {
        var children = sharings.Where(s => s.ParentCode == parentCode).OrderBy(s => s.Name).ToList();
        var result = new List<TreeNode>();
        foreach (var c in children)
        {
            var node = new TreeNode
            {
                Title = c.Name,
                Key = c.Code,
                Icon = false,
                Children = CreateChildrenSharing(sharings, c.Code),
                Select = c.Flagged,
                Selected = c.Flagged,
                ParentTitle = c.ParentName,
                GroupLevel = GroupLevel.Property
            };
            result.Add(node);
        }
        return result;
    }

    private static string SubGetFileName()
    {
        var m_strTime = DateTime.UtcNow.ToFileTimeUtc().ToString();
        var strFileName = $"Export_{m_strTime}.pdf";
        return strFileName;
    }

    private static int GetInt(object? value, int fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (int.TryParse(Convert.ToString(value), out var i)) return i;
        try { return Convert.ToInt32(value); } catch { return fallback; }
    }

    private static string GetStr(object? value)
    {
        return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value) ?? string.Empty;
    }

    private static bool GetBool(object? value)
    {
        if (value == null || value == DBNull.Value) return false;
        if (bool.TryParse(Convert.ToString(value), out var b)) return b;
        try { return Convert.ToInt32(value) != 0; } catch { return false; }
    }

    private string GetUserConnectionString(int codeUser)
    {
        var mainDbConnectionEncrypted = _configuration.GetConnectionString("MainDB") ?? string.Empty;
        var decryptedMainDb = Decrypt(mainDbConnectionEncrypted);
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
        var dataSourceEnc = GetStr(r["DataSource"]);
        var initialCatalogEnc = GetStr(r["InitialCatalog"]);
        var userIdEnc = GetStr(r["User_ID"]);
        var passwordEnc = GetStr(r["Password"]);

        var dataSource = Decrypt(dataSourceEnc);
        var initialCatalog = Decrypt(initialCatalogEnc);
        var userId = Decrypt(userIdEnc);
        var password = Decrypt(passwordEnc);

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

    private static string Decrypt(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var iv = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        var key = Encoding.UTF8.GetBytes("&%#@?,:*");

        try
        {
            using var des = new DESCryptoServiceProvider();
            var input = Convert.FromBase64String(text);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, des.CreateDecryptor(key, iv), CryptoStreamMode.Write))
            {
                cs.Write(input, 0, input.Length);
                cs.FlushFinalBlock();
            }
            return Encoding.UTF8.GetString(ms.ToArray());
        }
        catch
        {
            return string.Empty;
        }
    }

    private void LogInfo(string msg) => _logger.LogInformation(msg);
    private void LogWarn(string msg, Exception ex) => _logger.LogWarning(ex, msg);
    private void LogError(string msg, Exception ex) => _logger.LogError(ex, msg);
}

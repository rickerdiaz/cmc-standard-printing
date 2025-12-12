using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CalcmenuAPI_CMC_CoopGastro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrinterController : ControllerBase
    {
        // TODO: Wire real logger
        private static void LogInfo(string msg) { /* hook log4net or ILogger */ }
        private static void LogWarn(string msg, Exception ex) { /* hook log4net or ILogger */ }
        private static void LogError(string msg, Exception ex) { /* hook log4net or ILogger */ }

        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        // POST /api/print/search
        [HttpPost("search")]
        public ActionResult<List<Models.Printer>> GetPrinterConfigByName([FromBody] Models.ConfigurationcSearch data)
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

                var printers = new List<Models.Printer>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    printers.Add(new Models.Printer
                    {
                        Code = GetInt(r["ID"]),
                        Name = GetStr(r["Name"]),
                        IsGlobal = GetBool(r["IsGlobal"]),
                        CodeSaleSite = GetInt(r["CodeSaleSite"]),
                        SaleSiteName = GetStr(r["SaleSiteName"]),
                        Status = GetInt(r["Status"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(data.Name))
                {
                    var result = new List<Models.Printer>();
                    foreach (var word in data.Name.Trim().Split(','))
                    {
                        var w = (word ?? string.Empty).Trim();
                        if (w.Length == 0) continue;
                        foreach (var s in printers)
                        {
                            if (s.Name?.ToLowerInvariant().Contains(w.ToLowerInvariant()) == true)
                                result.Add(s);
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

        // GET /api/printer/sharing/{codesite}/{tree}/{codecategory}
        [HttpGet("sharing/{codesite:int}/{tree:int}/{codecategory:int}")]
        public ActionResult<List<Models.TreeNode>> GetPrinterSharing(int codesite, int tree, int codecategory)
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

                var sharings = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    sharings.Add(new Models.GenericTree
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

                var sharingdata = new List<Models.TreeNode>();
                var parents = sharings.Where(obj => obj.ParentCode == 0 && obj.Type == 1).OrderBy(obj => obj.Name).ToList();
                foreach (var p in parents)
                {
                    var parent = new Models.TreeNode
                    {
                        title = p.Name,
                        key = p.Code,
                        icon = false,
                        children = CreateChildrenSharing(sharings, p.Code),
                        select = p.Flagged,
                        selected = p.Flagged,
                        parenttitle = p.ParentName,
                        groupLevel = Models.GroupLevel.Property
                    };
                    if (parent.children != null && parent.children.Count > 0)
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

        private static List<Models.TreeNode> CreateChildrenSharing(List<Models.GenericTree> sharings, int parentCode)
        {
            var children = sharings.Where(s => s.ParentCode == parentCode).OrderBy(s => s.Name).ToList();
            var result = new List<Models.TreeNode>();
            foreach (var c in children)
            {
                var node = new Models.TreeNode
                {
                    title = c.Name,
                    key = c.Code,
                    icon = false,
                    children = CreateChildrenSharing(sharings, c.Code),
                    select = c.Flagged,
                    selected = c.Flagged,
                    parenttitle = c.ParentName,
                    groupLevel = Models.GroupLevel.Property
                };
                result.Add(node);
            }
            return result;
        }

        // POST api/printer
        [HttpPost]
        public ActionResult<Models.ResponseCallBack> SavePrinter([FromBody] Models.PrinterData data)
        {
            var response = new Models.ResponseCallBack();
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
                response.Parameters = new List<Models.param> { new Models.param { name = "data", value = "RecipeData" } };
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

        // POST api/printer/delete
        [HttpPost("delete")]
        public ActionResult<Models.ResponseCallBack> DeleteCategory([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
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
                response.Parameters = new List<Models.param> { new Models.param { name = "data", value = "RecipeData" } };
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

        // GET /api/printer/getsalesite
        [HttpGet("getsalesite")]
        public ActionResult<List<Models.GenericList>> GetCodeSaleSite()
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

                var userAccounts = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    userAccounts.Add(new Models.GenericList
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]) 
                    });
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

        // POST /api/standardprinting
        [HttpPost("standardprinting")]
        public ActionResult<object> StandardPrinting([FromBody] InputData inputData)
        {
            try
            {
                LogInfo("standardPrinting started");
                var intCodePrintList = Convert.ToInt32(inputData.IntCodePrintList);
                var codeUser = Convert.ToInt32(inputData.CodeUser);
                var userLocale = string.IsNullOrWhiteSpace(inputData.userLocale) ? "en-US" : inputData.userLocale;

                var userConnectionString = GetUserConnectionString(codeUser);

                var cReport = new EgsReport.clsReport();
                var folderPath = HttpContext.RequestServices.GetService<IConfiguration>()?["ReportFolder"] ?? string.Empty;
                var folderURL = HttpContext.RequestServices.GetService<IConfiguration>()?["ReportURL"] ?? string.Empty;
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

                    XtraReport report = cReport.CreateReport_CMC(
                        ds,
                        userConnectionString,
                        1,
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
                else
                {
                    LogWarn("standardPrinting: dataset is empty; skipping report generation.", new Exception("Empty dataset"));
                    return Ok(string.Empty);
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        private static string SubGetFileName()
        {
            var m_strTime = DateTime.UtcNow.ToFileTimeUtc().ToString();
            var strFileName = $"Export_{m_strTime}.pdf";
            return strFileName;
        }

        public class InputData
        {
            public string IntCodePrintList { get; set; } = string.Empty;
            public string userLocale { get; set; } = string.Empty;
            public string strSelectedCodeListe { get; set; } = string.Empty;
            public int CodeUser { get; set; }
            public int intCodeTrans { get; set; }
            public string strExcelFilename { get; set; } = string.Empty;
            public string ImagePath { get; set; } = string.Empty;
        }

        // Helpers ported from VB/Common
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

        private static string GetUserConnectionString(int codeUser)
        {
            // TODO: Replace with actual implementation from EgsData/modFunctions
            // Example: decrypt per-user connection string
            return "";
        }
    }

    // NOTE: Placeholder Models - replace with actual models namespace
    namespace Models
    {
        public enum GroupLevel { Property }
        public class Printer { public int Code { get; set; } public string? Name { get; set; } public bool IsGlobal { get; set; } public int CodeSaleSite { get; set; } public string? SaleSiteName { get; set; } public int Status { get; set; } }
        public class ConfigurationcSearch { public int CodeSite { get; set; } public int CodeProperty { get; set; } public string Name { get; set; } = string.Empty; }
        public class GenericTree { public int Code { get; set; } public string Name { get; set; } = string.Empty; public int ParentCode { get; set; } public string ParentName { get; set; } = string.Empty; public bool Flagged { get; set; } public int Type { get; set; } public bool Global { get; set; } }
        public class TreeNode { public string title { get; set; } = string.Empty; public int key { get; set; } public bool icon { get; set; } public List<TreeNode> children { get; set; } = new(); public bool select { get; set; } public bool selected { get; set; } public string parenttitle { get; set; } = string.Empty; public GroupLevel groupLevel { get; set; } }
        public class ResponseCallBack { public int Code { get; set; } public string Message { get; set; } = string.Empty; public object? ReturnValue { get; set; } public bool Status { get; set; } public List<param>? Parameters { get; set; } }
        public class param { public string name { get; set; } = string.Empty; public string value { get; set; } = string.Empty; }
        public class PrinterData { public PrinterInfo Info { get; set; } = new(); public PrinterProfile Profile { get; set; } = new(); public List<SharingItem> Sharing { get; set; } = new(); }
        public class PrinterInfo { public int Code { get; set; } public string Name { get; set; } = string.Empty; public int Status { get; set; } public bool IsGlobal { get; set; } public int CodeSaleSite { get; set; } }
        public class PrinterProfile { public int Code { get; set; } public int CodeSite { get; set; } }
        public class SharingItem { public int Code { get; set; } }
        public class GenericDeleteData { public List<DeleteCode> CodeList { get; set; } = new(); public int CodeUser { get; set; } public int CodeSite { get; set; } public bool ForceDelete { get; set; } }
        public class DeleteCode { public int Code { get; set; } }
        public class GenericList { public int Code { get; set; } public string Name { get; set; } = string.Empty; }
    }
}

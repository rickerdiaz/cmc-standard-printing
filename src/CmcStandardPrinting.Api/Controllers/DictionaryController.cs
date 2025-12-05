using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CmcStandardPrinting.Domain.Dictionaries;
using CmcStandardPrinting.Domain.Printers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DictionaryController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DictionaryController> _logger;

    public DictionaryController(IConfiguration configuration, ILogger<DictionaryController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpPost("search")]
    public ActionResult<List<DictionaryItem>> GetDictionaryByName([FromBody] DictionarySearch data)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_MANAGE_Generic]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.CodeTrans;
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWDICTIONARY";
            cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = data.CodeProperty;
            cn.Open();
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(ds);
            }

            var dictionary = new List<DictionaryItem>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    dictionary.Add(new DictionaryItem
                    {
                        CodeGroup = GetInt(r["CodeGroup"]),
                        Name = GetStr(r["Name"]),
                        CodeDictionary = GetInt(r["CodeDictionary"])
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(data.Name))
            {
                var dictionaryresult = new List<DictionaryItem>();
                var arrNames = data.Name.Trim().Split(',');
                foreach (var word in arrNames)
                {
                    var w = (word ?? string.Empty).Trim();
                    if (w.Length == 0) continue;
                    foreach (var s in dictionary)
                    {
                        if (s.Name?.ToLowerInvariant().Contains(w.ToLowerInvariant()) == true)
                        {
                            dictionaryresult.Add(s);
                        }
                    }
                }

                dictionary = dictionaryresult;
            }

            return Ok(dictionary);
        }
        catch (ArgumentException aex)
        {
            LogWarn("GetDictionaryByName: Missing or invalid parameters", aex);
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            LogError("GetDictionaryByName: Unexpected error occured", ex);
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("sharing/{codesite:int}/{type:int}/{tree:int}/{codedictionary:int}")]
    public ActionResult<List<TreeNode>> GetdictionarySharing(int codesite, int type, int tree, int codedictionary)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandText = "[dbo].[API_GET_SharingAll]";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codedictionary;
            cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = 117;
            cn.Open();
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(ds);
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
                {
                    sharingdata.Add(parent);
                }
            }

            return Ok(sharingdata);
        }
        catch (ArgumentException aex)
        {
            LogWarn("GetdictionarySharing: Missing or invalid parameters", aex);
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            LogError("GetdictionarySharing: Unexpected error occured", ex);
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("translation/{code:int}/{codesite:int}")]
    public ActionResult<List<GenericTranslation>> GetDictionaryTranslation(int code, int codesite)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_DictionaryTranslation]";
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = code;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cn.Open();
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(ds);
            }

            var translations = new List<GenericTranslation>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    translations.Add(new GenericTranslation
                    {
                        CodeTrans = GetInt(r["CodeTrans"]),
                        TranslationName = GetStr(r["TranslationName"]),
                        Name = GetStr(r["Name"])
                    });
                }
            }

            return Ok(translations);
        }
        catch (ArgumentException aex)
        {
            LogWarn("GetDictionaryTranslation: Missing or invalid parameters", aex);
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            LogError("GetDictionaryTranslation: Unexpected error occured", ex);
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost]
    public ActionResult<ResponseCallBack> SaveDictionary([FromBody] DictionaryData data)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        try
        {
            LogInfo("SaveDictionary: " + JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cn.Open();
            var codeDictionary = data.Info.CodeDictionary;

            cmd.CommandText = "MANAGE_DICTIONARYVALIDATION";
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (var t in data.Translation)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CodeGroup", SqlDbType.Int).Value = data.Info.CodeGroup;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 600).Value = t.Name;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = t.CodeTrans;
                cmd.Parameters.Add("@CodeDictionary", SqlDbType.Int).Value = codeDictionary;
                var ret = cmd.Parameters.Add("@retval", SqlDbType.Int);
                ret.Direction = ParameterDirection.ReturnValue;
                cmd.ExecuteNonQuery();
                resultCode = GetInt(ret.Value, -1);
                if (resultCode != 0)
                {
                    throw new Exception($"[{resultCode}] Save dictionary failed");
                }
            }

            codeDictionary = data.Info.CodeDictionary;

            cmd.CommandText = "MANAGE_DICTIONARYUPDATE";
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (var t in data.Translation)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CodeGroup", SqlDbType.Int).Value = data.Info.CodeGroup;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 600).Value = t.Name;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = t.CodeTrans;
                cmd.Parameters.Add("@CodeTransMain", SqlDbType.Int).Value = data.Info.CodeTransMain;
                var pCodeDict = cmd.Parameters.Add("@CodeDictionary", SqlDbType.Int);
                pCodeDict.Direction = ParameterDirection.InputOutput;
                pCodeDict.Value = codeDictionary;
                var ret = cmd.Parameters.Add("@retval", SqlDbType.Int);
                ret.Direction = ParameterDirection.ReturnValue;
                cmd.ExecuteNonQuery();
                codeDictionary = GetInt(pCodeDict.Value, -1);
                resultCode = GetInt(ret.Value, -1);
                if (resultCode != 0)
                {
                    throw new Exception($"[{resultCode}] Save dictionary failed");
                }
            }

            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = codeDictionary;
            response.Status = true;
        }
        catch (Exception dbEx)
        {
            LogError("SaveDictionary: Database error occured", dbEx);
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.Message = "Save dictionary failed";
            return StatusCode(500, response);
        }

        return Ok(response);
    }

    [HttpPost("delete")]
    public ActionResult<ResponseCallBack> DeleteDictionary([FromBody] DictionaryDeleteData data)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        try
        {
            LogInfo("DeleteDictionary: " + JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            var codeDictionarySet = new HashSet<int>(data.CodeList.Select(c => c.CodeDictionary));
            var codeList = string.Join(",", codeDictionarySet);

            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandText = "API_DELETE_Generic";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeList;
            cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWDICTIONARY";
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
                throw new Exception($"[{resultCode}] Delete dictionary failed");
            }

            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = GetStr(skipList.Value);
            response.Status = true;
        }
        catch (ArgumentException aex)
        {
            LogWarn("DeleteDictionary: Missing or invalid parameters", aex);
            response.Code = 400;
            response.Message = "Missing or invalid parameters";
            response.Parameters = new List<Param> { new() { Name = "data", Value = nameof(DictionaryDeleteData) } };
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            LogError("DeleteDictionary: Unexpected error occured", ex);
            response.Status = false;
            response.Message = "Unexpected error occured";
            response.Code = resultCode == 0 ? 500 : resultCode;
            response.ReturnValue = resultCode == 0 ? GetStr(null) : response.ReturnValue;
            return StatusCode(500, response);
        }

        return Ok(response);
    }

    [HttpGet("{codesite:int}/{codetrans:int}/{codegroup:int}")]
    public ActionResult<List<DictionaryListItem>> GetDictionaryList(int codesite, int codetrans, int codegroup)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_PMDictionaryGetList";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@CodeGroup", SqlDbType.Int).Value = codegroup;
            cn.Open();
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(ds);
            }

            var dictionary = new List<DictionaryListItem>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    dictionary.Add(new DictionaryListItem
                    {
                        Code = GetInt(r["CodeDictionary"]),
                        Value = GetStr(r["Name"])
                    });
                }
            }

            return Ok(dictionary);
        }
        catch (ArgumentException aex)
        {
            LogWarn("GetDictionaryList: Missing or invalid parameters", aex);
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            LogError("GetDictionaryList: Unexpected error occured", ex);
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("{codesite:int}/{codetrans:int}/{codegroup:int}/{codegroup2:int}")]
    public ActionResult<List<DictionaryListItem>> GetDictionaryListAll(int codesite, int codetrans, int codegroup, int codegroup2)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_PMDictionaryGetListALL";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@CodeGroup", SqlDbType.Int).Value = codegroup;
            cmd.Parameters.Add("@CodeGroup2", SqlDbType.Int).Value = codegroup2;
            cn.Open();
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(ds);
            }

            var dictionary = new List<DictionaryListItem>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    dictionary.Add(new DictionaryListItem
                    {
                        Code = GetInt(r["CodeDictionary"]),
                        Value = GetStr(r["Name"])
                    });
                }
            }

            return Ok(dictionary);
        }
        catch (ArgumentException aex)
        {
            LogWarn("GetDictionaryListAll: Missing or invalid parameters", aex);
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            LogError("GetDictionaryListAll: Unexpected error occured", ex);
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

    private void LogInfo(string msg) => _logger.LogInformation(msg);
    private void LogWarn(string msg, Exception ex) => _logger.LogWarning(ex, msg);
    private void LogError(string msg, Exception ex) => _logger.LogError(ex, msg);
}

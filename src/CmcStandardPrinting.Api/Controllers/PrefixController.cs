using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CmcStandardPrinting.Domain.Common;
using CmcStandardPrinting.Domain.Prefixes;
using CmcStandardPrinting.Domain.Printers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrefixController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PrefixController> _logger;

    public PrefixController(IConfiguration configuration, ILogger<PrefixController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpPost]
    public async Task<ActionResult<ResponseCallBack>> SavePrefix([FromBody] PrefixData data, CancellationToken ct)
    {
        var response = new ResponseCallBack();
        SqlTransaction? tx = null;
        var resultCode = 0;
        try
        {
            var arrSharing = new ArrayList();
            foreach (var sh in data.Sharing ?? new List<GenericList>())
            {
                if (!arrSharing.Contains(sh.Code))
                {
                    arrSharing.Add(sh.Code);
                }
            }

            var codeSiteList = "(" + string.Join(",", arrSharing.Cast<object>()) + ")";

            await using var cn = new SqlConnection(ConnectionString);
            await cn.OpenAsync(ct);
            tx = (SqlTransaction)await cn.BeginTransactionAsync(ct);
            await using var cmd = new SqlCommand("[UPDATE_Prefix]", cn, tx)
            {
                CommandType = CommandType.StoredProcedure
            };

            var pCode = cmd.Parameters.Add("@Code", SqlDbType.Int);
            pCode.Direction = ParameterDirection.InputOutput;
            pCode.Value = data?.Info?.Code ?? 0;

            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = data?.Info?.Name ?? string.Empty;
            cmd.Parameters.Add("@IsFemale", SqlDbType.Bit).Value = string.Equals(data?.Info?.Gender, "Feminine", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
            cmd.Parameters.Add("@TranslationCode", SqlDbType.Int).Value = data?.Info?.TranslationCode ?? 0;
            cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data?.Info?.IsGlobal ?? false;
            cmd.Parameters.Add("@CodeSites", SqlDbType.NVarChar, 2000).Value = codeSiteList;

            var ret = cmd.Parameters.Add("@retval", SqlDbType.Int);
            ret.Direction = ParameterDirection.ReturnValue;

            await cmd.ExecuteNonQueryAsync(ct);

            resultCode = Convert.ToInt32(ret.Value ?? -1);
            if (resultCode != 0)
            {
                throw new InvalidOperationException("Save prefix failed");
            }

            var codePrefix = Convert.ToInt32(pCode.Value);
            if (codePrefix > 0 && data?.Sharing != null)
            {
                var sharingList = data.Sharing.Select(x => x.Code).Distinct().ToList();
                var codeSharedTo = string.Join(",", sharingList);

                cmd.CommandText = Common.SP_API_UPDATE_Sharing;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codePrefix;
                cmd.Parameters.Add("@CodeOwner", SqlDbType.Int).Value = data.Info.CodeOwner;
                cmd.Parameters.Add("@CodeSharedToList", SqlDbType.VarChar, 4000).Value = codeSharedTo;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = 154;
                cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.IsGlobal;

                var sharingResult = cmd.Parameters.Add("@Return", SqlDbType.Int);
                sharingResult.Direction = ParameterDirection.ReturnValue;

                await cmd.ExecuteNonQueryAsync(ct);
                resultCode = Convert.ToInt32(sharingResult.Value ?? -1);
                if (resultCode != 0)
                {
                    throw new InvalidOperationException("Save prefix sharing failed");
                }
            }

            await tx.CommitAsync(ct);

            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = codePrefix;
            response.Status = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            try
            {
                if (tx != null)
                {
                    await tx.RollbackAsync(ct);
                }
            }
            catch
            {
                // ignored
            }

            _logger.LogError(ex, "SavePrefix failed");
            response.Code = resultCode == 0 ? 500 : resultCode;
            response.Status = false;
            response.Message = "Save prefix failed";
            return StatusCode(500, response);
        }
    }

    [HttpPost("update_recipe_ingredient_prefix")]
    public async Task<ActionResult<ResponseCallBack>> UpdateRecipeIngredientPrefix([FromBody] PrefixGeneric data, CancellationToken ct)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        try
        {
            await using var cn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand("UPDATE_RecipeIngredientPrefix", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = data?.CodeListe ?? 0;
            cmd.Parameters.Add("@CodePrefix", SqlDbType.Int).Value = data?.CodePrefix ?? 0;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data?.CodeTrans ?? 0;
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data?.CodeUser ?? 0;

            await cn.OpenAsync(ct);
            await cmd.ExecuteNonQueryAsync(ct);

            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = string.Empty;
            response.Status = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateRecipeIngredientPrefix failed");
            response.Code = resultCode == 0 ? 500 : resultCode;
            response.Status = false;
            response.Message = "Update recipe ingredient prefix failed.";
            return StatusCode(500, response);
        }
    }

    [HttpPost("delete")]
    public async Task<ActionResult<ResponseCallBack>> DeletePrefix([FromBody] GenericDeleteData data, CancellationToken ct)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        try
        {
            var arrProjectCodes = new ArrayList();
            foreach (var c in data.CodeList ?? new List<DeleteCode>())
            {
                if (!arrProjectCodes.Contains(c.Code))
                {
                    arrProjectCodes.Add(c.Code);
                }
            }

            var codeProjectList = string.Join(",", arrProjectCodes.Cast<object>().Select(x => Convert.ToString(x)));

            await using var cn = new SqlConnection(ConnectionString);
            await using var cmd = new SqlCommand("API_DELETE_Generic", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeProjectList;
            cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "PREFIX";
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
            cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = data.ForceDelete;

            var skip = cmd.Parameters.Add("@SkipList", SqlDbType.VarChar, 4000);
            skip.Direction = ParameterDirection.Output;

            var ret = cmd.Parameters.Add("@Return", SqlDbType.Int);
            ret.Direction = ParameterDirection.ReturnValue;

            await cn.OpenAsync(ct);
            await cmd.ExecuteNonQueryAsync(ct);

            resultCode = Convert.ToInt32(ret.Value ?? -1);
            if (resultCode != 0)
            {
                throw new InvalidOperationException("Delete prefix failed");
            }

            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = skip.Value == DBNull.Value ? string.Empty : Convert.ToString(skip.Value);
            response.Status = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeletePrefix failed");
            response.Code = resultCode == 0 ? 500 : resultCode;
            response.Status = false;
            response.Message = "Delete prefix failed";
            response.ReturnValue = string.Empty;
            return StatusCode(500, response);
        }
    }

    [HttpGet("{codesite:int?}/{name?}")]
    public ActionResult<List<Prefix>> GetPrefix(int codesite, string? name = "")
    {
        try
        {
            name ??= string.Empty;
            var ds = new DataSet();

            using var cmd = new SqlCommand
            {
                Connection = new SqlConnection(ConnectionString),
                CommandType = CommandType.StoredProcedure,
                CommandText = "[dbo].[GET_PrefixList]"
            };

            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@vchName", SqlDbType.NVarChar).Value = name;

            cmd.Connection.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cmd.Connection.Close();

            var prefixes = MapPrefixes(ds.Tables[0]);

            if (!string.IsNullOrWhiteSpace(name))
            {
                prefixes = FilterByName(prefixes, name);
            }

            return Ok(prefixes);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPrefix failed: {CodeSite}/{Name}", codesite, name);
            return StatusCode(500);
        }
    }

    [HttpPost("search")]
    public ActionResult<List<Prefix>> GetPrefix2([FromBody] ConfigurationcSearch data)
    {
        try
        {
            data.Name ??= string.Empty;
            var ds = new DataSet();

            using var cmd = new SqlCommand
            {
                Connection = new SqlConnection(ConnectionString),
                CommandType = CommandType.StoredProcedure,
                CommandText = "[dbo].[GET_PrefixList]"
            };

            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
            cmd.Parameters.Add("@vchName", SqlDbType.NVarChar).Value = data.Name;
            cmd.Parameters.Add("@CodeProperty", SqlDbType.NVarChar).Value = data.CodeProperty;

            cmd.Connection.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cmd.Connection.Close();

            var prefixes = MapPrefixes(ds.Tables[0]);

            if (!string.IsNullOrWhiteSpace(data.Name))
            {
                prefixes = FilterByName(prefixes, data.Name);
            }

            return Ok(prefixes);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPrefix2 failed");
            return StatusCode(500);
        }
    }

    [HttpPost("getLanguageTranslations")]
    public ActionResult<List<GenericList>> GetLanguageTranslations([FromBody] GenericTranslation data)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand
            {
                Connection = new SqlConnection(ConnectionString),
                CommandType = CommandType.StoredProcedure,
                CommandText = "[dbo].[GET_PrefixTranslationList]"
            };

            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = 1;
            cmd.Parameters.Add("@CodeSites", SqlDbType.NVarChar).Value = "en";

            cmd.Connection.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cmd.Connection.Close();

            var translations = new List<GenericList>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                translations.Add(new GenericList
                {
                    Code = GetInt(r["Code"]),
                    Name = GetStr(r["Name"])
                });
            }

            return Ok(translations);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetLanguageTranslations failed");
            return StatusCode(500);
        }
    }

    [HttpGet("prefixtranslations/{codesite:int?}/{isglobal:bool?}/{codesites?}")]
    public ActionResult<List<GenericList>> GetPrefixTranslationList(int codesite, bool isglobal, string? codesites = "")
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand
            {
                Connection = new SqlConnection(ConnectionString),
                CommandType = CommandType.StoredProcedure,
                CommandText = "[dbo].[GET_PrefixTranslationList]"
            };

            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = isglobal;
            cmd.Parameters.Add("@CodeSites", SqlDbType.NVarChar).Value = codesites ?? string.Empty;

            cmd.Connection.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cmd.Connection.Close();

            var translations = new List<GenericList>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                translations.Add(new GenericList
                {
                    Code = GetInt(r["Code"]),
                    Name = GetStr(r["Name"])
                });
            }

            return Ok(translations);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPrefixTranslationList failed");
            return StatusCode(500);
        }
    }

    [HttpGet("sharing/{codesite:int}/{type:int}/{tree:int}/{codeprefix:int}")]
    public ActionResult<List<TreeNode>> GetPrefixSharing(int codesite, int type, int tree, int codeprefix)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand
            {
                Connection = new SqlConnection(ConnectionString),
                CommandText = "[dbo].[API_GET_SharingPrefix]",
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@CodePrefix", SqlDbType.Int).Value = codeprefix;

            cmd.Connection.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var sharings = new List<GenericTree>();
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

            var sharingdata = new List<TreeNode>();
            var parents = sharings.Where(obj => obj.ParentCode == 0 && obj.Type == 1).OrderBy(obj => obj.Name).ToList();
            foreach (var p in parents)
            {
                var parent = new TreeNode
                {
                    Title = p.Name,
                    Key = p.Code,
                    Icon = false,
                    Children = CreateChildren(sharings, p.Code),
                    Select = p.Flagged,
                    Selected = p.Flagged,
                    ParentTitle = p.ParentName
                };
                sharingdata.Add(parent);
            }

            return Ok(sharingdata);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPrefixSharing failed");
            return StatusCode(500);
        }
    }

    private static List<TreeNode> CreateChildren(List<GenericTree> sharingdata, int code)
    {
        var children = new List<TreeNode>();
        var kids = sharingdata.Where(obj => obj.ParentCode == code && obj.Type == 2).OrderBy(obj => obj.Name).ToList();
        foreach (var k in kids)
        {
            var child = new TreeNode
            {
                Title = k.Name,
                Key = k.Code,
                Icon = false,
                Children = new List<TreeNode>(),
                Select = k.Flagged,
                Selected = k.Flagged,
                ParentTitle = k.ParentName,
                Note = k.Global
            };
            children.Add(child);
        }

        return children;
    }

    private static List<Prefix> MapPrefixes(DataTable table)
    {
        var prefixes = new List<Prefix>();
        foreach (DataRow r in table.Rows)
        {
            prefixes.Add(new Prefix
            {
                Code = GetInt(r["Code"]),
                Name = GetStr(r["Name"]),
                Gender = GetStr(r["Gender"]),
                TranslationCode = GetInt(r["TranslationCode"]),
                PrefixLanguage = GetStr(r["PrefixLanguage"]),
                IsGlobal = GetBool(r["IsGlobal"])
            });
        }

        return prefixes;
    }

    private static List<Prefix> FilterByName(List<Prefix> prefixes, string name)
    {
        var result = new List<Prefix>();
        foreach (var word in name.Split(','))
        {
            var w = ReplaceSpecialCharacters(word.Trim().ToLowerInvariant());
            foreach (var prefix in prefixes)
            {
                if (prefix.Name.ToLowerInvariant().Contains(w))
                {
                    result.Add(prefix);
                }
            }
        }

        return result;
    }

    private static int GetInt(object value, int fallback = 0)
    {
        if (value == null || value == DBNull.Value)
        {
            return fallback;
        }

        return int.TryParse(Convert.ToString(value), out var i) ? i : fallback;
    }

    private static string GetStr(object value) => value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value) ?? string.Empty;

    private static bool GetBool(object value)
    {
        if (value == null || value == DBNull.Value)
        {
            return false;
        }

        if (value is bool b)
        {
            return b;
        }

        return int.TryParse(Convert.ToString(value), out var i) ? i != 0 : false;
    }

    private static string ReplaceSpecialCharacters(string value)
    {
        return value
            .Replace("ç", "c")
            .Replace("ö", "o")
            .Replace("ş", "s")
            .Replace("ü", "u")
            .Replace("ı", "i")
            .Replace("ğ", "g");
    }
}

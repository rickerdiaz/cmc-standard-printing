using System;
using System.Collections.Generic;
using System.Data;
<<<<<<< HEAD
using Microsoft.Data.SqlClient;
=======
using System.Data.SqlClient;
>>>>>>> main
using System.Globalization;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.Translations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
public class TranslationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TranslationController> _logger;

    public TranslationController(IConfiguration configuration, ILogger<TranslationController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("/api/Translation/{codesite:int}")]
    public ActionResult<List<GenericList>> GetTranslationList(int codesite)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[GET_TRANSLATIONCODENAME]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = true;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var translations = new List<GenericList>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    translations.Add(new GenericList
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"])
                    });
                }
            }

            return Ok(translations);
        }
        catch (Exception ex)
        {
            LogError("GetTranslationList: Database error occurred", ex);
            return StatusCode(500);
        }
    }

    [HttpGet("/api/Translation/Dict/{codetrans:int}/{codesite:int}")]
    public ActionResult<int> GetCodeDict(int codetrans, int codesite)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[sp_EgswTranslationGetList]";
            cmd.Parameters.Add("@intCode", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = codesite;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var code = 0;
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    code = GetInt(r["CodeDictionary"]);
                }
            }

            return Ok(code);
        }
        catch (Exception ex)
        {
            LogError("GetCodeDict: Database error occurred", ex);
            return StatusCode(500);
        }
    }

    [HttpGet("/api/translation/ingredient/{codeliste:int}/{codesite:int?}/{excludecodetrans:int?}")]
    public ActionResult<List<RecipeIngredientTranslation>> GetIngredientTranslation(int codeliste, int codesite = -1, int excludecodetrans = -1)
    {
        try
        {
            var list = new List<RecipeIngredientTranslation>();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "API_GET_IngredientTranslation";
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@ExcludeCodeTrans", SqlDbType.Int).Value = excludecodetrans;
            cn.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new RecipeIngredientTranslation
                {
                    CodeTrans = GetInt(dr["CodeTrans"]),
                    Name = GetStr(dr["Name"]),
                    Complement = GetStr(dr["Complement"]),
                    Preparation = GetStr(dr["Preparation"]),
                    AlternativeIngredient = GetStr(dr["AlternativeIngredient"]),
                    CodeUnitDisplaySelection = GetInt(dr["CodeUnitDisplaySelection"]),
                    IsGenderSensitive = GetBool(dr["IsGenderSensitive"]),
                    PrefixCode = GetInt(dr["PrefixCode"]),
                    PrefixGender = GetBool(dr["IsFemale"]) ? "Feminine" : "Masculine"
                });
            }
            dr.Close();
            return Ok(list);
        }
        catch (Exception ex)
        {
            LogError("GetIngredientTranslation: Database error occurred", ex);
            return StatusCode(500);
        }
    }

    [HttpGet("/api/translation/recipe/ingredient/{codeliste:int}/{codesite:int?}/{excludecodetrans:int?}")]
    public ActionResult<List<RecipeIngredientTranslation>> GetRecipeIngredientTranslation(int codeliste, int codesite = -1, int excludecodetrans = -1)
    {
        try
        {
            var list = new List<RecipeIngredientTranslation>();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "API_GET_RecipeIngredientTranslation";
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@ExcludeCodeTrans", SqlDbType.Int).Value = excludecodetrans;
            cn.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new RecipeIngredientTranslation
                {
                    ItemId = GetInt(dr["Id"]),
                    CodeTrans = GetInt(dr["CodeTrans"]),
                    Name = GetStr(dr["Name"]),
                    Complement = GetStr(dr["Complement"]),
                    Preparation = GetStr(dr["Preparation"]),
                    AlternativeIngredient = GetStr(dr["AlternativeIngredient"])
                });
            }
            dr.Close();
            return Ok(list);
        }
        catch (Exception ex)
        {
            LogError("GetRecipeIngredientTranslation: Database error occurred", ex);
            return StatusCode(500);
        }
    }

    [HttpGet("/api/translation/recipe/procedure/{codeliste:int}/{excludecodetrans:int?}")]
    public ActionResult<List<RecipeProcedureTranslation>> GetRecipeProcedureTranslation(int codeliste, int excludecodetrans = -1)
    {
        try
        {
            var list = new List<RecipeProcedureTranslation>();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "API_GET_RecipeProcedureTranslation";
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cmd.Parameters.Add("@ExcludeCodeTrans", SqlDbType.Int).Value = excludecodetrans;
            cn.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new RecipeProcedureTranslation
                {
                    NoteId = GetInt(dr["NoteId"]),
                    CodeTrans = GetInt(dr["CodeTrans"]),
                    Note = GetStr(dr["Note"]),
                    AbbrevNote = GetStr(dr["AbbrevNote"])
                });
            }
            dr.Close();
            return Ok(list);
        }
        catch (Exception ex)
        {
            LogError("GetRecipeProcedureTranslation: Database error occurred", ex);
            return StatusCode(500);
        }
    }

    private static int GetInt(object? value, int fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (value is int i) return i;
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var ii)) return ii;
        try { return Convert.ToInt32(value, CultureInfo.InvariantCulture); } catch { return fallback; }
    }

    private static string GetStr(object? value, string fallback = "")
    {
        if (value == null || value == DBNull.Value) return fallback;
        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? fallback;
    }

    private static bool GetBool(object? value)
    {
        if (value == null || value == DBNull.Value) return false;
        if (value is bool b) return b;
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var i)) return i != 0;
        if (bool.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var bb)) return bb;
        return false;
    }

    private void LogError(string msg, Exception ex) => _logger.LogError(ex, msg);
}

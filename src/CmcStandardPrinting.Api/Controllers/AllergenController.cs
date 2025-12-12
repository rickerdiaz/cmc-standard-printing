using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using CmcStandardPrinting.Domain.Allergens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AllergenController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AllergenController> _logger;

    public AllergenController(IConfiguration configuration, ILogger<AllergenController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("{codetrans:int}/{codesite:int}/{name?}")]
    public ActionResult<List<BrandTreeNode>> GetAllergens(int codetrans, int codesite, string? name = "")
    {
        try
        {
            name = NormalizeName(name);
            using var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Allergens]";
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = (object?)name ?? DBNull.Value;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var allergens = new List<BrandTreeNode>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                allergens.Add(new BrandTreeNode
                {
                    Title = GetStr(r["Name"]),
                    Key = GetStr(r["Code"]),
                    HasPicture = true,
                    Picture = GetStr(r["PictureName"])
                });
            }

            return Ok(allergens);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetAllergens: argument error");
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllergens: database error");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("allergensnew/{codetrans:int}/{codesite:int}/{name?}")]
    public ActionResult<List<BrandTreeNode>> GetAllergensNew(int codetrans, int codesite, string? name = "")
    {
        try
        {
            name = NormalizeName(name);
            using var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Allergens]";
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = (object?)name ?? DBNull.Value;
            cmd.Parameters.Add("@CodeSite", SqlDbType.VarChar).Value = codesite;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var allergens = new List<BrandTreeNode>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                allergens.Add(new BrandTreeNode
                {
                    Title = GetStr(r["Name"]),
                    Key = GetStr(r["Code"]),
                    HasPicture = true,
                    Picture = GetStr(r["PictureName"])
                });
            }

            return Ok(allergens);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetAllergensNew: argument error");
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllergensNew: database error");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("list/{codetrans:int}")]
    public ActionResult<List<GenericCodeValueList>> GetAllergenList(int codetrans)
    {
        try
        {
            using var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Allergens]";
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var allergens = new List<GenericCodeValueList>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                allergens.Add(new GenericCodeValueList
                {
                    Code = r["Code"],
                    Value = r["Name"]
                });
            }

            return Ok(allergens);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetAllergenList: argument error");
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllergenList: database error");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("{codeliste:int}/{codetrans:int}/{codesite:int}")]
    public ActionResult<List<ListeAllergen>> GetListeAllergen(int codeliste, int codetrans, int codesite)
    {
        try
        {
            using var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_ListeAllergen]";
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var allergens = new List<ListeAllergen>();
            foreach (DataRow p in ds.Tables[0].Rows)
            {
                allergens.Add(new ListeAllergen
                {
                    CodeListe = codeliste,
                    CodeAllergen = p["CodeAllergen"],
                    Contain = p["Contain"],
                    Trace = p["Trace"],
                    NonAllergen = p["NonAllergen"],
                    Derived = p["Derived"],
                    Hidden = p["Hidden"]
                });
            }

            return Ok(allergens);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetListeAllergen: argument error");
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetListeAllergen: database error");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("ingredient/{codeliste:int}/{codetrans:int}/{codesite:int}")]
    public ActionResult<List<IngredientAllergen>> GetIngredientAllergen(int codeliste, int codetrans, int codesite)
    {
        try
        {
            using var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_ListeAllergen]";
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var allergens = new List<IngredientAllergen>();
            foreach (DataRow p in ds.Tables[0].Rows)
            {
                var contain = GetBool(p["Contain"]);
                var trace = GetBool(p["Trace"]);
                if (contain || trace)
                {
                    allergens.Add(new IngredientAllergen
                    {
                        CodeListe = codeliste,
                        CodeAllergen = p["CodeAllergen"],
                        Contain = p["Contain"],
                        Trace = p["Trace"],
                        NonAllergen = p["NonAllergen"]
                    });
                }
            }

            return Ok(allergens);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetIngredientAllergen: argument error");
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetIngredientAllergen: database error");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("derived/{codeliste:int}")]
    public ActionResult<List<ListeAllergen>> GetListeAllergenDerived(int codeliste)
    {
        try
        {
            using var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_RecipeDerivedAllergens]";
            cmd.Parameters.Add("@FirstCode", SqlDbType.Int).Value = codeliste;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var allergens = new List<ListeAllergen>();
            foreach (DataRow p in ds.Tables[0].Rows)
            {
                allergens.Add(new ListeAllergen
                {
                    CodeListe = p["SecondCode"],
                    CodeAllergen = p["CodeAllergen"],
                    Contain = p["Contain"],
                    Trace = p["Trace"]
                });
            }

            return Ok(allergens);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetListeAllergenDerived: argument error");
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetListeAllergenDerived: database error");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    private static string? NormalizeName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name) || name == "null" || name == "undefined")
        {
            return null;
        }

        return name;
    }

    private static string GetStr(object? value)
    {
        return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value) ?? string.Empty;
    }

    private static bool GetBool(object? value)
    {
        if (value == null || value == DBNull.Value)
        {
            return false;
        }

        if (bool.TryParse(Convert.ToString(value), out var result))
        {
            return result;
        }

        try
        {
            return Convert.ToInt32(value) != 0;
        }
        catch
        {
            return false;
        }
    }
}

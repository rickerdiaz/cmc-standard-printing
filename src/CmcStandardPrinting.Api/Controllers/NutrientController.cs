using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using CmcStandardPrinting.Domain.Common;
using CmcStandardPrinting.Domain.Nutrients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NutrientController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<NutrientController> _logger;

    public NutrientController(IConfiguration configuration, ILogger<NutrientController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("nutrientdb/{codesite:int}/{codeset:int}/{codetrans:int}/{page:int}/{searchstring?}")]
    public ActionResult<NutrientResponse> GetNutrientTest(int codesite, int codeset, int codetrans, int page, string searchstring = "")
    {
        const int pageCount = 25;
        try
        {
            using var cmd = new SqlCommand();
            var ds = new DataSet();
            var dsDefinitions = new DataSet();

            using (var cn = new SqlConnection(ConnectionString))
            {
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[GetNutrientDataPerSet]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@CodeSet", SqlDbType.Int).Value = codeset;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                cmd.Parameters.Add("@nvcSearchString", SqlDbType.NVarChar).Value = searchstring ?? string.Empty;
                cmd.Parameters.Add("@intFilterType", SqlDbType.Int).Value = 3;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }

            using (var cn = new SqlConnection(ConnectionString))
            {
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[sp_EgswNutrientGetList]";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@intCodeNutrientDB", SqlDbType.Int).Value = -1;
                cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = codetrans;
                cmd.Parameters.Add("@intCodeSet", SqlDbType.Int).Value = codeset;
                cn.Open();
                using var da2 = new SqlDataAdapter(cmd);
                da2.Fill(dsDefinitions);
            }

            if (ds.Tables.Count == 0 || dsDefinitions.Tables.Count == 0)
            {
                return Ok(new NutrientResponse());
            }

            var nutrient = ds.Tables[0];
            var query = nutrient.AsEnumerable().Skip(page * pageCount).Take(pageCount);
            var filtered = query.Any() ? query.CopyToDataTable() : nutrient.Clone();
            foreach (DataColumn col in filtered.Columns)
            {
                if (col.ColumnName.StartsWith("val", StringComparison.OrdinalIgnoreCase))
                {
                    col.ColumnName = (col.Ordinal - 4).ToString();
                }
            }

            if (dsDefinitions.Tables.Count > 1)
            {
                dsDefinitions.Tables.RemoveAt(1);
            }

            var definitions = new List<NutrientDefinition>();
            foreach (DataRow row in dsDefinitions.Tables[0].Rows)
            {
                definitions.Add(new NutrientDefinition
                {
                    Code = GetInt(row["Code"]),
                    Value = GetStr(row["DisplayName"])
                });
            }

            var nutrients = new List<Dictionary<string, object?>>();
            foreach (DataRow row in filtered.Rows)
            {
                var dict = new Dictionary<string, object?>();
                foreach (DataColumn col in filtered.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }
                nutrients.Add(dict);
            }

            return Ok(new NutrientResponse
            {
                NutrientDefinition = definitions,
                Nutrients = nutrients
            });
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get nutrient test data");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("nutrient/{codeliste:int}/{codenutrientset:int}/{codesite:int}/{codetrans:int}/{imposedtype:int}")]
    public ActionResult<List<RecipeNutrition>> GetNutrient(int codeliste, int codenutrientset, int codesite, int codetrans, int imposedtype)
    {
        var nutrients = new List<RecipeNutrition>();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_NutrientData]";
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cmd.Parameters.Add("@CodeNutrientSet", SqlDbType.Int).Value = codenutrientset;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@ImposedType", SqlDbType.Int).Value = imposedtype;
            cn.Open();
            using var dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    var n = new RecipeNutrition
                    {
                        Id = GetInt(dr["Id"]),
                        Nutr_No = GetInt(dr["Nutr_No"]),
                        Position = GetInt(dr["Position"]),
                        Name = GetStr(dr["Name"]),
                        TagName = GetStr(dr["TagName"]),
                        Format = GetStr(dr["Format"]),
                        Value = SafePos(GetDbl(dr["Value"], -1)),
                        Imposed = SafePos(GetDbl(dr["Imposed"], -1)),
                        Percent = SafePos(GetDbl(dr["Percent"], -1)),
                        Unit = GetStr(dr["Unit"]),
                        GDA = GetInt(dr["GDA"]),
                        CodeNutrientSet = GetInt(dr["CodeNutrientSet"]),
                        NutrientSet = GetStr(dr["NutrientSet"]),
                        DisplayNutrition = GetBool(dr["DisplayNutrition"]),
                        Display = GetBool(dr["Display"]),
                        ImposedType = GetInt(dr["ImposedType"]),
                        PortionSize = GetStr(dr["PortionSize"]),
                        NutritionBasis = GetStr(dr["NutritionBasis"])
                    };
                    nutrients.Add(n);
                }
            }
            return Ok(nutrients);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get nutrient data");
            return StatusCode(500);
        }
    }

    [HttpGet("nutrientset/{codesite:int}")]
    public ActionResult<List<GenericCodeValueList>> GetNutrientSet(int codesite)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "API_GET_NutrientSetList";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            var nutrientsets = new List<GenericCodeValueList>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                nutrientsets.Add(new GenericCodeValueList
                {
                    Code = GetInt(r["Code"]),
                    Value = GetStr(r["DisplayName"])
                });
            }
            return Ok(nutrientsets);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get nutrient set list");
            return StatusCode(500);
        }
    }

    private static int GetInt(object? value, int fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (int.TryParse(Convert.ToString(value), out var i)) return i;
        try
        {
            return Convert.ToInt32(value);
        }
        catch
        {
            return fallback;
        }
    }

    private static double GetDbl(object? value, double fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (double.TryParse(Convert.ToString(value), out var d)) return d;
        try
        {
            return Convert.ToDouble(value);
        }
        catch
        {
            return fallback;
        }
    }

    private static bool GetBool(object? value)
    {
        if (value == null || value == DBNull.Value) return false;
        if (bool.TryParse(Convert.ToString(value), out var b)) return b;
        try
        {
            return Convert.ToInt32(value) != 0;
        }
        catch
        {
            return false;
        }
    }

    private static string GetStr(object? value) => value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value) ?? string.Empty;

    private static double SafePos(double value) => value < 0 ? 0 : value;
}

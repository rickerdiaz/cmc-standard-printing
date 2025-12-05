using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.SetPrices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SetPriceController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SetPriceController> _logger;

    public SetPriceController(IConfiguration configuration, ILogger<SetPriceController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("sharing/{codesite:int}/{tree:int}/{codesetprice:int}")]
    public ActionResult<List<TreeNode>> GetSetPriceSharing(int codesite, int tree, int codesetprice)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_SharingAll]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 2;
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codesetprice;
            cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = 110;
            cn.Open();
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
            var parents = sharings.Where(o => o.ParentCode == 0 && o.Type == 1).OrderBy(o => o.Name).ToList();
            foreach (var p in parents)
            {
                if (sharingdata.All(o => o.Key != p.Code))
                {
                    var parentNode = new TreeNode
                    {
                        Title = p.Name,
                        Key = p.Code,
                        Icon = false,
                        Children = CreateChildren(sharings, p.Code),
                        Select = p.Flagged,
                        Selected = p.Flagged,
                        ParentTitle = p.ParentName,
                        GroupLevel = GroupLevel.Property
                    };
                    sharingdata.Add(parentNode);
                }
            }

            return Ok(sharingdata);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load set price sharing");
            return StatusCode(500);
        }
    }

    [HttpGet("list/{codesite:int}/{codesetprice?}")]
    public ActionResult<List<SetPrice>> GetSetPriceList(int codesite, int? codesetprice = null)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[sp_EgswSetPriceGetList]";
            var codeVal = codesetprice ?? -1;
            cmd.Parameters.Add("@intCode", SqlDbType.Int).Value = codeVal;
            cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = -1;
            cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = 1;
            cmd.Parameters.Add("@tntType", SqlDbType.Int).Value = (codeVal == -1 ? -1 : 1);
            cmd.Parameters.Add("@bitGlobalOnly", SqlDbType.Bit).Value = 0;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var list = new List<SetPrice>();
            if (ds.Tables.Count > 1)
            {
                foreach (DataRow r in ds.Tables[1].Rows)
                {
                    list.Add(new SetPrice
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Main = GetBool(r["Main"]),
                        HasMain = GetInt(r["hasMain"]),
                        Global = GetBool(r["IsGlobal"]),
                        CodeCurrency = GetInt(r["CodeCurrency"]),
                        Format = GetStr(r["Format"]),
                        Symbole = GetStr(r["Symbole"]),
                        Description = GetStr(r["Description"]),
                        ChkDisable = GetBool(r["chkDisable"]),
                        Factor = GetDbl(r["FactorToMain"])
                    });
                }
            }

            return Ok(list);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load set price list");
            return StatusCode(500);
        }
    }

    [HttpGet("source/{code:int}")]
    public ActionResult<DataTable> GetSetPriceByCodeSource(int code)
    {
        try
        {
            var dt = new DataTable();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_SetPriceBySource]";
            cmd.Parameters.Add("@codeSource", SqlDbType.Int).Value = code;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return Ok(dt);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load set price by code");
            return StatusCode(500);
        }
    }

    [HttpGet("{codesite:int}/{type:int}/{name?}")]
    public ActionResult<List<SetPrice>> GetSetPriceByName(int codesite, int type, string name = "")
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_MANAGE_Generic]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = name ?? string.Empty;
            cmd.Parameters.Add("@CodeMain", SqlDbType.Int).Value = -1;
            cmd.Parameters.Add("@CodeGroup", SqlDbType.Int).Value = -1;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var list = new List<SetPrice>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                list.Add(new SetPrice
                {
                    Code = GetInt(r["Code"]),
                    Name = GetStr(r["Name"]),
                    Main = GetBool(r["Main"]),
                    HasMain = GetInt(r["hasMain"]),
                    Global = GetBool(r["IsGlobal"])
                });
            }

            return Ok(list);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load set price search results");
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
                GroupLevel = GroupLevel.Site
            };
            children.Add(child);
        }

        return children;
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
}

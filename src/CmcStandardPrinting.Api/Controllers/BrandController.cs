using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CmcStandardPrinting.Domain.Brands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BrandController : ControllerBase
{
    private readonly ILogger<BrandController> _logger;
    private readonly string _connectionString;

    public BrandController(ILogger<BrandController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration.GetConnectionString("Default") ?? string.Empty;
    }

    [HttpGet("{codeliste:int}/{codetrans:int}")]
    public async Task<ActionResult<List<BrandListItem>>> GetBrandByCode(int codeliste, int codetrans, CancellationToken cancellationToken)
    {
        try
        {
            await using var cn = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand("[dbo].[GET_ListeIngredientsBrandList]", cn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;

            await cn.OpenAsync(cancellationToken);
            await using var rdr = await cmd.ExecuteReaderAsync(cancellationToken);

            var brands = new List<BrandListItem>();
            while (await rdr.ReadAsync(cancellationToken))
            {
                brands.Add(new BrandListItem
                {
                    Code = ReadInt(rdr, "CodeBrand"),
                    Value = ReadString(rdr, "NameBrand")
                });
            }

            return Ok(brands);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetBrandByCode failed: {CodeListe}/{CodeTrans}", codeliste, codetrans);
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("site/{codesite:int}/{codetrans:int}/{activeonly:bool?}/{tree:bool?}/{name?}")]
    public async Task<ActionResult<object>> GetBrandBySite(int codesite, int codetrans, string? name = "", bool activeonly = true, bool tree = true, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name) || name == "null" || name == "undefined")
            {
                name = null;
            }

            await using var cn = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand("[dbo].[GET_BRANDCODENAME]", cn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@ActiveOnly", SqlDbType.Bit).Value = activeonly;
            cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = (object?)name ?? DBNull.Value;

            await cn.OpenAsync(ct);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);

            var brands = new List<Brand>();
            while (await rdr.ReadAsync(ct))
            {
                brands.Add(new Brand
                {
                    Code = ReadInt(rdr, "Code"),
                    Name = ReadString(rdr, "Name"),
                    ParentCode = ReadInt(rdr, "ParentCode"),
                    ParentName = ReadString(rdr, "ParentName"),
                    Flagged = false
                });
            }

            if (!tree)
            {
                return Ok(brands);
            }

            var branddata = new List<BrandTreeNode>();
            var parents = brands.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
            foreach (var p in parents)
            {
                var parent = new BrandTreeNode
                {
                    Title = p.Name,
                    Key = p.Code,
                    Icon = false,
                    Children = BuildBrandTreeNodes(brands, p.Code),
                    Select = p.Flagged,
                    ParentTitle = p.ParentName
                };
                branddata.Add(parent);
            }

            return Ok(branddata);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetBrandBySite failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("tree/{codesite:int}/{codetrans:int}/{codebrands}/{codeliste:int}/{brandclass:int}")]
    public async Task<ActionResult<List<BrandTreeNode>>> GetBrandTree(int codesite, int codetrans, string codebrands, int codeliste, int brandclass, CancellationToken ct)
    {
        try
        {
            await using var cn = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand("[dbo].[API_GET_Brands]", cn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@CodeBrands", SqlDbType.VarChar, 2000).Value = codebrands;
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;

            await cn.OpenAsync(ct);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);

            var brands = new List<Brand>();
            while (await rdr.ReadAsync(ct))
            {
                if (ReadInt(rdr, "IsIngredientbrand") == brandclass)
                {
                    brands.Add(new Brand
                    {
                        Code = ReadInt(rdr, "Code"),
                        Name = ReadString(rdr, "Name"),
                        ParentCode = ReadInt(rdr, "ParentCode"),
                        ParentName = ReadString(rdr, "ParentName"),
                        Flagged = ReadBool(rdr, "Flagged"),
                        Note = ReadString(rdr, "IsIngredientbrand"),
                        Classification = ReadInt(rdr, "BrandClassification")
                    });
                }
            }

            var branddata = new List<BrandTreeNode>();
            var parents = brands.Where(obj => obj.ParentCode == 0).OrderBy(obj => obj.Name).ToList();
            foreach (var p in parents)
            {
                var parent = new BrandTreeNode
                {
                    Title = p.Name,
                    Key = p.Code,
                    Icon = false,
                    Children = BuildBrandTreeNodes(brands, p.Code),
                    Select = p.Flagged,
                    ParentTitle = p.ParentName,
                    Note = p.Note,
                    Classification = p.Classification
                };
                branddata.Add(parent);
            }

            return Ok(branddata);
        }
        catch (ArgumentException)
        {
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetBrandTree failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    private static List<BrandTreeNode> BuildBrandTreeNodes(List<Brand> data, int code)
    {
        var children = new List<BrandTreeNode>();
        var kids = data.Where(obj => obj.Code != code && obj.ParentCode == code && code > 0).OrderBy(obj => obj.Name).ToList();
        foreach (var k in kids)
        {
            var child = new BrandTreeNode
            {
                Title = k.Name,
                Key = k.Code,
                Children = BuildBrandTreeNodes(data, k.Code),
                Icon = false,
                Select = k.Flagged,
                ParentTitle = k.ParentName,
                Note = k.Note,
                Classification = k.Classification
            };
            children.Add(child);
        }

        return children;
    }

    private static int ReadInt(SqlDataReader rdr, string field)
    {
        var value = rdr[field];
        return value == DBNull.Value ? 0 : Convert.ToInt32(value);
    }

    private static string ReadString(SqlDataReader rdr, string field)
    {
        var value = rdr[field];
        return value == DBNull.Value ? string.Empty : Convert.ToString(value) ?? string.Empty;
    }

    private static bool ReadBool(SqlDataReader rdr, string field)
    {
        var value = rdr[field];
        return value != DBNull.Value && Convert.ToBoolean(value);
    }
}

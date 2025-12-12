using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
<<<<<<< HEAD
using Microsoft.Data.SqlClient;
=======
using System.Data.SqlClient;
>>>>>>> main
using System.Globalization;
using System.Linq;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.Units;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
public class UnitController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<UnitController> _logger;

    public UnitController(IConfiguration configuration, ILogger<UnitController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpGet("/api/unit/{codesite:int}/{codetrans:int}/{kind:int}")]
    public ActionResult<List<GenericList>> GetUnitByKind(int codesite, int codetrans, int kind)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[GET_UNITCODENAME]";
            cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@intUnitType", SqlDbType.Int).Value = kind;
            cmd.Parameters.Add("@intCodeliste", SqlDbType.Int).Value = -1;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var list = new List<GenericList>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                list.Add(new GenericList
                {
                    Code = GetInt(r["Code"]),
                    Name = GetStr(r["NameDisplay"])
                });
            }
            return Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUnitByKind: database error");
            return StatusCode(500);
        }
    }

    [HttpGet("/api/unitlist/{codesite:int}/{codetrans:int}/{codesetprice:int}/{mainType:int}")]
    public ActionResult<List<Unit>> GetUnit(int codesite, int codetrans, int codesetprice, int mainType, int type = (int)UnitType.Neutral, int codeliste = -1)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Units]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = codesetprice;
            cmd.Parameters.Add("@MainType", SqlDbType.Int).Value = mainType;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var list = new List<Unit>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                list.Add(new Unit
                {
                    Code = GetInt(dr["CodeUnit"]),
                    Value = GetStr(dr["UnitName"]),
                    Price = GetStr(dr["Price"]),
                    PriceUnit = GetStr(dr["PriceUnit"]),
                    PriceFactor = GetDbl(dr["PriceFactor"]),
                    IsIngredient = GetInt(dr["IsIngredient"]) != 0,
                    IsMetric = GetInt(dr["IsMetric"]),
                    IsYield = GetInt(dr["PriceFactor"]),
                    Format = GetStr(dr["Format"], "#,###.###")
                });
            }
            return Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUnit: database error");
            return StatusCode(500);
        }
    }

    [HttpGet("/api/unit/search/{codesite:int}/{name?}")]
    public ActionResult<ResponseCallBack> GetUnitByName(int codesite, string name = "")
    {
        var response = new ResponseCallBack();
        int resultCode = 0;
        try
        {
            var intCodeUnit = -1;
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            if (!string.IsNullOrWhiteSpace(name))
            {
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "API_GET_UnitByName";
                cmd.Parameters.Add("@UnitName", SqlDbType.NVarChar, 32).Value = GetStr(name);
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = GetInt(codesite);
                var outParam = cmd.Parameters.Add("@CodeUnit", SqlDbType.Int);
                outParam.Direction = ParameterDirection.Output;
                cn.Open();
                cmd.ExecuteNonQuery();
                intCodeUnit = GetInt(outParam.Value);
                response.Code = 0;
                response.Message = "OK";
                response.ReturnValue = intCodeUnit.ToString(CultureInfo.InvariantCulture);
                response.Status = true;
                return Ok(response);
            }
            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = string.Empty;
            response.Status = true;
            return Ok(response);
        }
        catch (DatabaseException)
        {
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.Message = "Search unit failed";
            return StatusCode(500, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUnitByName: unexpected error");
            response.Status = false;
            response.Message = "Unexpected error occured";
            response.Code = 500;
            return StatusCode(500, response);
        }
    }

    [HttpGet("/api/unit/{codeUnit:int}/{codeTrans:int}")]
    public ActionResult<List<UnitInfo>> GetUnitInfo(int codeunit, int codetrans)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_UnitInfo]";
            cmd.Parameters.Add("@CodeUnit", SqlDbType.Int).Value = codeunit;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var list = new List<UnitInfo>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                list.Add(new UnitInfo
                {
                    Code = GetInt(r["Code"]),
                    Name = GetStr(r["NameDisplay"]),
                    TypeMain = GetInt(r["TypeMain"]),
                    Type = GetInt(r["Type"]),
                    Factor = GetInt(r["Factor"]),
                    Format = GetStr(r["Format"])
                });
            }
            return Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUnitInfo: database error");
            return StatusCode(500);
        }
    }

    [HttpGet("/api/unit/convert/{codesite:int}/{codetrans:int}/{conversiontype:int}/{codeunit:int}/{useplural:int}")]
    public ActionResult<UnitConvert> ConvertUnit(int codesite, int codetrans, int conversiontype, int codeunit, int useplural, double value1 = 0.0, double value2 = 0.0)
    {
        try
        {
            var unit = new UnitConvert
            {
                Code = codeunit,
                Name = string.Empty,
                Value1 = value1,
                Value2 = value2
            };

            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_egswGetBestUnitConversion3";
            cmd.Parameters.Add("@fltValue", SqlDbType.Float).Value = value1;
            cmd.Parameters.Add("@fltValue2", SqlDbType.Float).Value = value2;
            cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = codesite;
            var pUnitCode = cmd.Parameters.Add("@intUnitCode", SqlDbType.Int);
            pUnitCode.Value = codeunit;
            var pName = cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 30);
            pName.Value = string.Empty;
            var pFormat = cmd.Parameters.Add("@nvcFormat", SqlDbType.NVarChar, 15);
            pFormat.Value = string.Empty;
            var pUnitFactor = cmd.Parameters.Add("@fltUnitFactor", SqlDbType.Float);
            pUnitFactor.Value = 0;
            var pUnitTypeMain = cmd.Parameters.Add("@intUnitTypeMain", SqlDbType.Int);
            pUnitTypeMain.Value = DBNull.Value;
            cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@UnitDisplayType", SqlDbType.SmallInt).Value = conversiontype;
            cmd.Parameters.Add("@intUsePlural", SqlDbType.SmallInt).Value = useplural;

            cmd.Parameters["@fltValue"].Direction = ParameterDirection.InputOutput;
            cmd.Parameters["@fltValue2"].Direction = ParameterDirection.InputOutput;
            pUnitCode.Direction = ParameterDirection.InputOutput;
            pName.Direction = ParameterDirection.InputOutput;
            pFormat.Direction = ParameterDirection.InputOutput;
            pUnitFactor.Direction = ParameterDirection.InputOutput;
            pUnitTypeMain.Direction = ParameterDirection.InputOutput;

            cn.Open();
            cmd.ExecuteNonQuery();

            unit.Code = GetInt(pUnitCode.Value, codeunit);
            unit.Name = GetStr(pName.Value);
            unit.Value1 = GetDbl(cmd.Parameters["@fltValue"].Value);
            unit.Value2 = GetDbl(cmd.Parameters["@fltValue2"].Value);
            return Ok(unit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ConvertUnit: database error");
            return StatusCode(500);
        }
    }

    [HttpGet("/api/unit/{codesite:int}/{codetrans:int}/{type:int}/{status:int}/{codeproperty:int?}/{merchandiseyield:int?}/{name?}")]
    public ActionResult<List<Unit>> SearchUnitByName(int codesite, int codetrans, int type, int status, int codeproperty = -1, int merchandiseyield = 0, string name = "")
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
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = status;
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWUNIT";
            cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = codeproperty;
            if (merchandiseyield == 1)
                cmd.Parameters.Add("@IsMerchandise", SqlDbType.Bit).Value = 1;
            else if (merchandiseyield == 2)
                cmd.Parameters.Add("@IsYield", SqlDbType.Bit).Value = 1;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var units = MapUnits(ds.Tables[0]);
            units = FilterUnitsByName(units, name);
            return Ok(units);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SearchUnitByName: database error");
            return StatusCode(500);
        }
    }

    [HttpPost("/api/unit/search")]
    public ActionResult<UnitResponseSearch> SearchUnitByName2([FromBody] ConfigurationSearch data)
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
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.CodeTrans;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = data.Type;
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = data.Status;
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EGSWUNIT";
            cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = data.CodeProperty;
            cmd.Parameters.Add("@skip", SqlDbType.Int).Value = data.Skip;
            cmd.Parameters.Add("@rowsPerPage", SqlDbType.Int).Value = data.RowsPerPage;
            cmd.Parameters.Add("@textSearch", SqlDbType.NVarChar, 1000).Value = data.Name ?? string.Empty;
            if (data.MerchandiseYield == 1)
                cmd.Parameters.Add("@IsMerchandise", SqlDbType.Bit).Value = 2;
            else if (data.MerchandiseYield == 2)
                cmd.Parameters.Add("@IsMerchandise", SqlDbType.Bit).Value = 0;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var totalCount = GetInt(ds.Tables[0].Rows[0]["Total"]);
            var units = MapUnits(ds.Tables[1]);
            units = FilterUnitsByName(units, data.Name ?? string.Empty);

            return Ok(new UnitResponseSearch(units, totalCount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SearchUnitByName2: database error");
            return StatusCode(500);
        }
    }

    [HttpGet("/api/unit/translation/{codeunit:int}/{codesite:int}")]
    public ActionResult<List<UnitTranslation>> GetUnitTranslation(int codeunit, int codesite)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_UnitTranslation]";
            cmd.Parameters.Add("@CodeUnit", SqlDbType.Int).Value = codeunit;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var list = new List<UnitTranslation>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                list.Add(new UnitTranslation
                {
                    CodeTrans = GetInt(r["CodeTrans"]),
                    TranslationName = GetStr(r["TranslationName"]),
                    NameDisplay = GetStr(r["NameDisplay"]),
                    NameDef = GetStr(r["NameDef"]),
                    NamePlural = GetStr(r["NamePlural"]),
                    AutoConversion = GetStr(r["AutoConversion"]),
                    Format = GetStr(r["Format"]),
                    IsIngredient = GetBool(r["Ingredient"]),
                    IsYield = GetBool(r["Yield"]),
                    UsedAsYield = GetInt(r["UsedAsYield"]),
                    UsedAsIngredient = GetInt(r["UsedAsIngredient"])
                });
            }
            return Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUnitTranslation: database error");
            return StatusCode(500);
        }
    }

    [HttpGet("/api/unit/sharing/{codesite:int}/{type:int}/{tree:int}/{codeunit:int}")]
    public ActionResult<List<TreeNode>> GetUnitSharing(int codesite, int type, int tree, int codeunit)
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
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = codeunit;
            cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = 135;
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

            var result = new List<TreeNode>();
            var parents = sharings.Where(o => o.ParentCode == 0 && o.Type == 1).OrderBy(o => o.Name).ToList();
            foreach (var p in parents)
            {
                    if (result.All(o => o.Key != p.Code))
                    {
                        var parent = new TreeNode
                        {
                            Title = p.Name,
                            Key = p.Code,
                            Icon = false,
                            Children = CreateChildren(sharings, p.Code),
                            Select = p.Flagged,
                            ParentTitle = p.ParentName
                        };
                        result.Add(parent);
                    }
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUnitSharing: database error");
            return StatusCode(500);
        }
    }

    [HttpPost("api/unit")]
    public ActionResult<ResponseCallBack> SaveUnit([FromBody] UnitData data)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        SqlTransaction? trans = null;
        try
        {
            var arrSharing = new ArrayList();
            foreach (var sh in data.Sharing ?? new List<GenericList>())
            {
                if (!arrSharing.Contains(sh.Code)) arrSharing.Add(sh.Code);
            }
            var codeSiteList = Join(arrSharing, "(", ")", ",");

            var arrMerge = new ArrayList();
            foreach (var m in data.MergeList ?? new List<int>())
            {
                if (!arrMerge.Contains(m)) arrMerge.Add(m);
            }
            var mergeList = Join(arrMerge, "(", ")", ",");

            var yield = data.Info.IsYield ? 1 : 0;
            var ingredient = data.Info.IsIngredient ? 1 : 0;

            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_EgswUnitUpdate";
            cmd.Parameters.Clear();
            var retval = cmd.Parameters.Add("@retval", SqlDbType.Int);
            cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.Info.CodeUser;
            cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = data.Info.CodeSite;
            var pCode = cmd.Parameters.Add("@intCode", SqlDbType.Int);
            pCode.Value = data.Info.Code;
            cmd.Parameters.Add("@nvcNameDef", SqlDbType.NVarChar, 32).Value = data.Info.NameDef ?? string.Empty;
            cmd.Parameters.Add("@nvcNamePlural", SqlDbType.NVarChar, 32).Value = data.Info.NamePlural ?? string.Empty;
            cmd.Parameters.Add("@nvcNameDisp", SqlDbType.NVarChar, 32).Value = data.Info.NameDisplay ?? string.Empty;
            cmd.Parameters.Add("@nvcAutoConversion", SqlDbType.NVarChar, 500).Value = data.Info.AutoConversion ?? string.Empty;
            cmd.Parameters.Add("@IsBasic", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("@IsStock", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("@IsPackaging", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("@IsTranspo", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("@IsIngredient", SqlDbType.Bit).Value = ingredient;
            cmd.Parameters.Add("@IsYield", SqlDbType.Bit).Value = yield;
            cmd.Parameters.Add("@IsServing", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("@fltFactor", SqlDbType.Float).Value = 1;
            cmd.Parameters.Add("@intTypeMain", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@IsMetric", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("@nvcFormat", SqlDbType.NVarChar, 15).Value = data.Info.Format ?? string.Empty;
            cmd.Parameters.Add("@IsAdded", SqlDbType.Bit).Value = 1;
            cmd.Parameters.Add("@intPosition", SqlDbType.Int).Value = 0;
            cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.Global;
            cmd.Parameters.Add("@bitUseMakes", SqlDbType.Bit).Value = 1;
            cmd.Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = data.Info.Code == -1 ? 1 : 2;

            pCode.Direction = ParameterDirection.InputOutput;
            retval.Direction = ParameterDirection.ReturnValue;

            codeSiteList = (codeSiteList ?? string.Empty).Trim();
            if (!string.IsNullOrEmpty(codeSiteList))
            {
                if (codeSiteList.StartsWith("(") && codeSiteList.EndsWith(")"))
                    cmd.Parameters.Add("@vchCodeSiteList", SqlDbType.VarChar, 8000).Value = codeSiteList;
                else
                    throw new DatabaseException($"[{resultCode}] Save unit failed");
            }
            if (!string.IsNullOrEmpty(mergeList))
                cmd.Parameters.Add("@vchCodeMergeList", SqlDbType.VarChar, 8000).Value = mergeList;

            cn.Open();
            trans = cn.BeginTransaction();
            cmd.Transaction = trans;
            cmd.ExecuteNonQuery();
            resultCode = GetInt(retval.Value, -1);
            if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Save unit failed");

            var codeunit = GetInt(pCode.Value);

            cmd.CommandText = "sp_EgswActivateDeactivateUnits";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@Codes", SqlDbType.VarChar).Value = codeunit.ToString(CultureInfo.InvariantCulture);
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = data.Info.IsActive ? 1 : 2;
            cmd.ExecuteNonQuery();

            if (codeunit > 0 && data.Translation != null)
            {
                cmd.CommandText = "sp_EgswItemTranslationUpdate";
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var t in data.Translation)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@intCode", SqlDbType.Int).Value = codeunit;
                    cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar, 260).Value = t.NameDisplay ?? string.Empty;
                    cmd.Parameters.Add("@nvcName2", SqlDbType.NVarChar, 150).Value = t.NameDef ?? string.Empty;
                    cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = t.CodeTrans;
                    cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = data.Info.CodeSite;
                    cmd.Parameters.Add("@tntListType", SqlDbType.Int).Value = 3;
                    cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.Info.CodeUser;
                    cmd.Parameters.Add("@tntType", SqlDbType.Int).Value = data.Info.Type;
                    cmd.Parameters.Add("@nvcPlural", SqlDbType.NVarChar, 150).Value = t.NamePlural ?? string.Empty;
                    var r = cmd.Parameters.Add("@retval", SqlDbType.Int);
                    r.Direction = ParameterDirection.ReturnValue;
                    cmd.ExecuteNonQuery();
                    resultCode = GetInt(r.Value, -1);
                    if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Save unit failed");
                }
            }

            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = codeunit;
            response.Status = true;
            trans.Commit();
            return Ok(response);
        }
        catch (DatabaseException)
        {
            try { trans?.Rollback(); trans?.Dispose(); } catch { }
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.Message = resultCode == -82 ? "Merging of system units not allowed" : "Save unit failed";
            return StatusCode(500, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SaveUnit: unexpected error");
            response.Status = false;
            response.Message = "Unexpected error occured";
            response.Code = 500;
            return StatusCode(500, response);
        }
    }

    [HttpPost("api/unit/delete")]
    public ActionResult<ResponseCallBack> DeleteUnit([FromBody] GenericDeleteData data)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        try
        {
            var codes = (data.CodeList ?? new List<GenericList>()).Select(c => c.Code).Distinct().ToList();
            var joined = string.Join(",", codes);

            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandText = "API_DELETE_Generic";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = joined;
            cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWUNIT";
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
            cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = data.ForceDelete;
            var skip = cmd.Parameters.Add("@SkipList", SqlDbType.NVarChar, 4000);
            skip.Direction = ParameterDirection.Output;
            var ret = cmd.Parameters.Add("@Return", SqlDbType.Int);
            ret.Direction = ParameterDirection.ReturnValue;

            cn.Open();
            cmd.ExecuteNonQuery();
            resultCode = GetInt(ret.Value, -1);
            if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Delete unit failed");

            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = GetStr(skip.Value);
            response.Status = true;
            return Ok(response);
        }
        catch (DatabaseException)
        {
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.ReturnValue = GetStr(null);
            response.Message = "Delete unit failed";
            return StatusCode(500, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteUnit: unexpected error");
            response.Status = false;
            response.Message = "Unexpected error occured";
            response.Code = 500;
            return StatusCode(500, response);
        }
    }

    [HttpPost("api/unit/activatedeactivate")]
    public ActionResult<ResponseCallBack> ActivateDeactivate([FromBody] ActivateDeactivate data)
    {
        var response = new ResponseCallBack();
        var resultCode = 0;
        SqlTransaction? trans = null;
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandText = "sp_EgswActivateDeactivateUnits";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@Codes", SqlDbType.VarChar).Value = string.Join(",", data.CodesList ?? new List<int>());
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = data.Status;

            cn.Open();
            trans = cn.BeginTransaction();
            cmd.Transaction = trans;
            cmd.ExecuteNonQuery();

            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = "OK";
            response.Status = true;
            trans.Commit();
            return Ok(response);
        }
        catch (DatabaseException)
        {
            try { trans?.Rollback(); trans?.Dispose(); } catch { }
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.Message = resultCode == -82 ? ($"{(data.Status == 1 ? "Activation" : "Deactivation")} of system units not allowed") : "Save unit failed";
            return StatusCode(500, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ActivateDeactivate: unexpected error");
            response.Status = false;
            response.Message = "Unexpected error occured";
            response.Code = 500;
            return StatusCode(500, response);
        }
    }

    private static List<Unit> MapUnits(DataTable table)
    {
        var units = new List<Unit>();
        foreach (DataRow r in table.Rows)
        {
            units.Add(new Unit
            {
                Code = GetInt(r["Code"]),
                Name = GetStr(r["NameDisplay"]),
                NameDisplay = GetStr(r["NameDisplay"]),
                NameDef = GetStr(r["NameDef"]),
                NamePlural = GetStr(r["NamePlural"]),
                AutoConversion = GetStr(r["AutoConversion"]),
                Format = GetStr(r["Format"]),
                Global = GetBool(r["Global"]),
                IsYield = GetInt(r["Yield"]),
                IsIngredient = GetBool(r["Ingredient"]),
                IsAdded = GetBool(r["Added"]),
                IsActive = GetBool(r["Status"])
            });
        }

        return units;
    }

    private static List<Unit> FilterUnitsByName(List<Unit> units, string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return units;

        var unitresult = new List<Unit>();
        foreach (var w in name.Split(','))
        {
            var word = (w ?? string.Empty).Trim();
            if (word.Length == 0) continue;
            var key = ReplaceSpecialCharacters(word.ToLowerInvariant());
            foreach (var c in units)
            {
                if (!string.IsNullOrEmpty(c.Name) && c.Name.ToLowerInvariant().Contains(key)) unitresult.Add(c);
            }
        }
        return unitresult;
    }

    private static List<TreeNode>? CreateChildren(List<GenericTree> sharingdata, int code)
    {
            var children = new List<TreeNode>();
            var kids = sharingdata.Where(o => o.ParentCode == code && o.Type == 2).OrderBy(o => o.Name).ToList();
            foreach (var k in kids)
            {
                var child = new TreeNode
                {
                    Title = k.Name,
                    Key = k.Code,
                    Icon = false,
                    Children = new List<TreeNode>(),
                    Select = k.Flagged,
                    ParentTitle = k.ParentName,
                    Note = k.Global
                };
                children.Add(child);
            }

        return children;
    }

    private static string Join(ArrayList list, string prefix, string suffix, string separator)
    {
        var val = string.Empty;
        foreach (var c in list)
        {
            if (!string.IsNullOrEmpty(val)) val += separator;
            val += c?.ToString();
        }
        return string.IsNullOrEmpty(val) ? string.Empty : prefix + val + suffix;
    }

    private static int GetInt(object? value, int fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (value is int i) return i;
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var ii)) return ii;
        try { return Convert.ToInt32(value, CultureInfo.InvariantCulture); } catch { return fallback; }
    }

    private static double GetDbl(object? value, double fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (value is double d) return d;
        if (double.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var dd)) return dd;
        try { return Convert.ToDouble(value, CultureInfo.InvariantCulture); } catch { return fallback; }
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
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var i)) return i != 0;
        if (bool.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var bb)) return bb;
        return false;
    }

    private static string ReplaceSpecialCharacters(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        var result = value;
        var specialChars = new Dictionary<string, string>
        {
            { "ä", "ae" },
            { "ö", "oe" },
            { "ü", "ue" },
            { "ß", "ss" },
            { "é", "e" },
            { "è", "e" },
            { "ê", "e" },
            { "à", "a" },
            { "á", "a" },
            { "â", "a" },
            { "ù", "u" },
            { "û", "u" },
            { "ú", "u" },
            { "î", "i" },
            { "ï", "i" }
        };

        foreach (var k in specialChars.Keys)
        {
            result = result.Replace(k, specialChars[k], StringComparison.OrdinalIgnoreCase);
        }

        return result;
    }
}

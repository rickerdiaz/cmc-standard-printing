using System;
using System.Collections.Generic;
using System.Data;
<<<<<<< HEAD
using Microsoft.Data.SqlClient;
=======
using System.Data.SqlClient;
>>>>>>> main
using System.Globalization;
using CmcStandardPrinting.Domain.Ingredients;
using CmcStandardPrinting.Domain.NetworkSuppliers;
using CmcStandardPrinting.Domain.Printers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
public class NetworkSupplierController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<NetworkSupplierController> _logger;

    public NetworkSupplierController(IConfiguration configuration, ILogger<NetworkSupplierController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpPost("/api/NetworkSupplier/ManageUnit")]
    public ActionResult<NetworkSupplierUnitManage> ManageUnit([FromBody] NetworkSupplierUnitManage data)
    {
        if (data == null)
        {
            return BadRequest();
        }

        var xmlData = string.Empty;
        if (data.TransactionType == 2)
        {
            xmlData = string.Format(
                CultureInfo.InvariantCulture,
                "<units><unit1>{0}</unit1><unit2>{1}</unit2><unit3>{2}</unit3><unit4>{3}</unit4></units>",
                data.Unit1 ?? string.Empty,
                data.Unit2 ?? string.Empty,
                data.Unit3 ?? string.Empty,
                data.Unit4 ?? string.Empty);
        }

        using var cmd = new SqlCommand();
        using var cn = new SqlConnection(ConnectionString);
        try
        {
            cmd.CommandTimeout = 120;
            cmd.Connection = cn;
            cmd.CommandText = "API_ManageUnit";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@transaction", SqlDbType.Int).Value = data.TransactionType;
            cmd.Parameters.Add("@Unit", SqlDbType.VarChar, 50).Value = data.Unit1 ?? string.Empty;
            cmd.Parameters.Add("@xmldata", SqlDbType.Xml).Value = xmlData;
            cmd.Parameters.Add("@output", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@Unit1Code", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@Unit2Code", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@Unit3Code", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@Unit4Code", SqlDbType.Int).Direction = ParameterDirection.Output;

            cn.Open();
            cmd.ExecuteNonQuery();

            if (data.TransactionType == 1)
            {
                data.Unit1Code = GetInt(cmd.Parameters["@output"].Value, -1);
            }
            else if (data.TransactionType == 2)
            {
                data.Unit1Code = GetInt(cmd.Parameters["@Unit1Code"].Value, -1);
                data.Unit2Code = GetInt(cmd.Parameters["@Unit2Code"].Value, -1);
                data.Unit3Code = GetInt(cmd.Parameters["@Unit3Code"].Value, -1);
                data.Unit4Code = GetInt(cmd.Parameters["@Unit4Code"].Value, -1);
            }

            data.ResponseCode = 200;
            data.ResponseMessage = string.Empty;
            return Ok(data);
        }
        catch (DatabaseException ex)
        {
            data.ResponseCode = 500;
            data.ResponseMessage = ex.Message;
            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ManageUnit: unexpected error");
            data.ResponseCode = 500;
            data.ResponseMessage = ex.Message;
            return Ok(data);
        }
    }

    [HttpGet("/api/NetworkSupplier/DictionaryTranslationCode/{codeTrans?}")]
    public ActionResult<string> GetDictionaryTranslation(string? codeTrans)
    {
        using var cmd = new SqlCommand();
        using var cn = new SqlConnection(ConnectionString);
        try
        {
            cmd.CommandTimeout = 120;
            cmd.Connection = cn;
            cmd.CommandText = "API_GetDictionaryCode";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = int.TryParse(codeTrans, out var parsed) ? parsed : -1;
            cmd.Parameters.Add("@CodeDictionary", SqlDbType.Int).Direction = ParameterDirection.Output;
            cn.Open();
            cmd.ExecuteNonQuery();

            return Ok(GetStr(cmd.Parameters["@CodeDictionary"].Value, "0"));
        }
        catch (DatabaseException)
        {
            return Ok("2");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetDictionaryTranslation: unexpected error");
            return Ok("2");
        }
    }

    [HttpPost("/api/NetworkSupplier/update_supplier_merchandise")]
    public ActionResult<ResponseCallBack> SaveSupplierNetworkMerchandise([FromBody] SupplierNetworkMerchandise data)
    {
        var response = new ResponseCallBack();
        SqlTransaction? transaction = null;
        var resultCode = 0;

        try
        {
            if (data == null)
            {
                response.Code = 400;
                response.Message = "Missing or invalid parameters";
                response.Parameters = new List<Param> { new() { Name = "data", Value = "Merchandise" } };
                return Ok(response);
            }

            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            try
            {
                cmd.CommandTimeout = 120;
                cmd.Connection = cn;
                cmd.CommandText = "API_UpdateSupplierMerchandise";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.CodeTrans;
                cmd.Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = data.CodeSetPrice;
                cmd.Parameters.Add("@EgsRef", SqlDbType.NVarChar, 100).Value = (data.EgsRef ?? string.Empty).Trim();
                cmd.Parameters.Add("@Number", SqlDbType.NVarChar, 120).Value = (data.Number ?? string.Empty).Trim();
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 260).Value = (data.Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@OriginalName", SqlDbType.NVarChar, 260).Value = (data.OriginalName ?? string.Empty).Trim();
                cmd.Parameters.Add("@Category", SqlDbType.NVarChar, 200).Value = (data.Category ?? string.Empty).Trim();
                cmd.Parameters.Add("@Brand", SqlDbType.NVarChar, 200).Value = (data.Brand ?? string.Empty).Trim();
                cmd.Parameters.Add("@Supplier", SqlDbType.NVarChar, 200).Value = (data.Supplier ?? string.Empty).Trim();
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 2000).Value = (data.Description ?? string.Empty).Trim();
                cmd.Parameters.Add("@Declaration", SqlDbType.NVarChar, 2000).Value = (data.Declaration ?? string.Empty).Trim();
                cmd.Parameters.Add("@Ingredients", SqlDbType.NVarChar, 2000).Value = (data.Ingredients ?? string.Empty).Trim();
                cmd.Parameters.Add("@Preparation", SqlDbType.NVarChar, 2000).Value = (data.Preparation ?? string.Empty).Trim();
                cmd.Parameters.Add("@CookingTip", SqlDbType.NVarChar, 2000).Value = (data.CookingTip ?? string.Empty).Trim();
                cmd.Parameters.Add("@Refinement", SqlDbType.NVarChar, 2000).Value = (data.Refinement ?? string.Empty).Trim();
                cmd.Parameters.Add("@Storage", SqlDbType.NVarChar, 2000).Value = (data.Storage ?? string.Empty).Trim();
                cmd.Parameters.Add("@Productivity", SqlDbType.VarChar, 2000).Value = (data.Productivity ?? string.Empty).Trim();
                cmd.Parameters.Add("@Allergen", SqlDbType.NVarChar, 2000).Value = (data.Allergen ?? string.Empty).Trim();
                cmd.Parameters.Add("@CountryOrigin", SqlDbType.NVarChar, 100).Value = (data.CountryOrigin ?? string.Empty).Trim();
                cmd.Parameters.Add("@Attachment", SqlDbType.NVarChar, 200).Value = (data.Attachment ?? string.Empty).Trim();
                cmd.Parameters.Add("@SpecificDetermination", SqlDbType.NVarChar, 300).Value = (data.SpecificDetermination ?? string.Empty).Trim();
                cmd.Parameters.Add("@Barcode", SqlDbType.NVarChar, 60).Value = (data.Barcode ?? string.Empty).Trim();
                cmd.Parameters.Add("@Price1", SqlDbType.Float).Value = data.Price1;
                cmd.Parameters.Add("@Price2", SqlDbType.Float).Value = data.Price2;
                cmd.Parameters.Add("@Price3", SqlDbType.Float).Value = data.Price3;
                cmd.Parameters.Add("@Price4", SqlDbType.Float).Value = data.Price4;
                cmd.Parameters.Add("@Ratio1", SqlDbType.Float).Value = data.Ratio1;
                cmd.Parameters.Add("@Ratio2", SqlDbType.Float).Value = data.Ratio2;
                cmd.Parameters.Add("@Ratio3", SqlDbType.Float).Value = data.Ratio3;
                cmd.Parameters.Add("@Unit1", SqlDbType.NVarChar, 100).Value = (data.Unit1 ?? string.Empty).Trim();
                cmd.Parameters.Add("@Unit2", SqlDbType.NVarChar, 100).Value = (data.Unit2 ?? string.Empty).Trim();
                cmd.Parameters.Add("@Unit3", SqlDbType.NVarChar, 100).Value = (data.Unit3 ?? string.Empty).Trim();
                cmd.Parameters.Add("@Unit4", SqlDbType.NVarChar, 100).Value = (data.Unit4 ?? string.Empty).Trim();
                cmd.Parameters.Add("@Tax", SqlDbType.NVarChar, 30).Value = (data.Tax ?? string.Empty).Trim();
                cmd.Parameters.Add("@Wastage", SqlDbType.Float).Value = 0;
                cmd.Parameters.Add("@Wastage2", SqlDbType.Float).Value = 0;
                cmd.Parameters.Add("@Wastage3", SqlDbType.Float).Value = 0;
                cmd.Parameters.Add("@Wastage4", SqlDbType.Float).Value = 0;
                cmd.Parameters.Add("@Wastage5", SqlDbType.Float).Value = 0;

                cmd.Parameters.Add("@N1", SqlDbType.Float).Value = data.N1;
                cmd.Parameters.Add("@N2", SqlDbType.Float).Value = data.N2;
                cmd.Parameters.Add("@N3", SqlDbType.Float).Value = data.N3;
                cmd.Parameters.Add("@N4", SqlDbType.Float).Value = data.N4;
                cmd.Parameters.Add("@N5", SqlDbType.Float).Value = data.N5;
                cmd.Parameters.Add("@N6", SqlDbType.Float).Value = data.N6;
                cmd.Parameters.Add("@N7", SqlDbType.Float).Value = data.N7;
                cmd.Parameters.Add("@N8", SqlDbType.Float).Value = data.N8;
                cmd.Parameters.Add("@N9", SqlDbType.Float).Value = data.N9;
                cmd.Parameters.Add("@N10", SqlDbType.Float).Value = data.N10;
                cmd.Parameters.Add("@N11", SqlDbType.Float).Value = data.N11;
                cmd.Parameters.Add("@N12", SqlDbType.Float).Value = data.N12;
                cmd.Parameters.Add("@N13", SqlDbType.Float).Value = data.N13;
                cmd.Parameters.Add("@N14", SqlDbType.Float).Value = data.N14;
                cmd.Parameters.Add("@N15", SqlDbType.Float).Value = data.N15;
                cmd.Parameters.Add("@N16", SqlDbType.Float).Value = data.N16;
                cmd.Parameters.Add("@N17", SqlDbType.Float).Value = data.N17;
                cmd.Parameters.Add("@N18", SqlDbType.Float).Value = data.N18;
                cmd.Parameters.Add("@N19", SqlDbType.Float).Value = data.N19;
                cmd.Parameters.Add("@N20", SqlDbType.Float).Value = data.N20;
                cmd.Parameters.Add("@N21", SqlDbType.Float).Value = data.N21;
                cmd.Parameters.Add("@N22", SqlDbType.Float).Value = data.N22;
                cmd.Parameters.Add("@N23", SqlDbType.Float).Value = data.N23;
                cmd.Parameters.Add("@N24", SqlDbType.Float).Value = data.N24;
                cmd.Parameters.Add("@N25", SqlDbType.Float).Value = data.N25;
                cmd.Parameters.Add("@N26", SqlDbType.Float).Value = data.N26;
                cmd.Parameters.Add("@N27", SqlDbType.Float).Value = data.N27;
                cmd.Parameters.Add("@N28", SqlDbType.Float).Value = data.N28;
                cmd.Parameters.Add("@N29", SqlDbType.Float).Value = data.N29;
                cmd.Parameters.Add("@N30", SqlDbType.Float).Value = data.N30;
                cmd.Parameters.Add("@N31", SqlDbType.Float).Value = data.N31;
                cmd.Parameters.Add("@N32", SqlDbType.Float).Value = data.N32;
                cmd.Parameters.Add("@N33", SqlDbType.Float).Value = data.N33;
                cmd.Parameters.Add("@N34", SqlDbType.Float).Value = data.N34;
                cmd.Parameters.Add("@N35", SqlDbType.Float).Value = data.N35;
                cmd.Parameters.Add("@N36", SqlDbType.Float).Value = data.N36;
                cmd.Parameters.Add("@N37", SqlDbType.Float).Value = data.N37;
                cmd.Parameters.Add("@N38", SqlDbType.Float).Value = data.N38;
                cmd.Parameters.Add("@N39", SqlDbType.Float).Value = data.N39;
                cmd.Parameters.Add("@N40", SqlDbType.Float).Value = data.N40;
                cmd.Parameters.Add("@N41", SqlDbType.Float).Value = data.N41;
                cmd.Parameters.Add("@N42", SqlDbType.Float).Value = data.N42;

                cmd.Parameters.Add("@N1Name", SqlDbType.NVarChar, 100).Value = (data.N1Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N2Name", SqlDbType.NVarChar, 100).Value = (data.N2Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N3Name", SqlDbType.NVarChar, 100).Value = (data.N3Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N4Name", SqlDbType.NVarChar, 100).Value = (data.N4Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N5Name", SqlDbType.NVarChar, 100).Value = (data.N5Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N6Name", SqlDbType.NVarChar, 100).Value = (data.N6Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N7Name", SqlDbType.NVarChar, 100).Value = (data.N7Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N8Name", SqlDbType.NVarChar, 100).Value = (data.N8Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N9Name", SqlDbType.NVarChar, 100).Value = (data.N9Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N10Name", SqlDbType.NVarChar, 100).Value = (data.N10Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N11Name", SqlDbType.NVarChar, 100).Value = (data.N11Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N12Name", SqlDbType.NVarChar, 100).Value = (data.N12Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N13Name", SqlDbType.NVarChar, 100).Value = (data.N13Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N14Name", SqlDbType.NVarChar, 100).Value = (data.N14Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N15Name", SqlDbType.NVarChar, 100).Value = (data.N15Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N16Name", SqlDbType.NVarChar, 100).Value = (data.N16Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N17Name", SqlDbType.NVarChar, 100).Value = (data.N17Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N18Name", SqlDbType.NVarChar, 100).Value = (data.N18Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N19Name", SqlDbType.NVarChar, 100).Value = (data.N19Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N20Name", SqlDbType.NVarChar, 100).Value = (data.N20Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N21Name", SqlDbType.NVarChar, 100).Value = (data.N21Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N22Name", SqlDbType.NVarChar, 100).Value = (data.N22Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N23Name", SqlDbType.NVarChar, 100).Value = (data.N23Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N24Name", SqlDbType.NVarChar, 100).Value = (data.N24Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N25Name", SqlDbType.NVarChar, 100).Value = (data.N25Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N26Name", SqlDbType.NVarChar, 100).Value = (data.N26Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N27Name", SqlDbType.NVarChar, 100).Value = (data.N27Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N28Name", SqlDbType.NVarChar, 100).Value = (data.N28Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N29Name", SqlDbType.NVarChar, 100).Value = (data.N29Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N30Name", SqlDbType.NVarChar, 100).Value = (data.N30Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N31Name", SqlDbType.NVarChar, 100).Value = (data.N31Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N32Name", SqlDbType.NVarChar, 100).Value = (data.N32Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N33Name", SqlDbType.NVarChar, 100).Value = (data.N33Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N34Name", SqlDbType.NVarChar, 100).Value = (data.N34Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N35Name", SqlDbType.NVarChar, 100).Value = (data.N35Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N36Name", SqlDbType.NVarChar, 100).Value = (data.N36Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N37Name", SqlDbType.NVarChar, 100).Value = (data.N37Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N38Name", SqlDbType.NVarChar, 100).Value = (data.N38Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N39Name", SqlDbType.NVarChar, 100).Value = (data.N39Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N40Name", SqlDbType.NVarChar, 100).Value = (data.N40Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N41Name", SqlDbType.NVarChar, 100).Value = (data.N41Name ?? string.Empty).Trim();
                cmd.Parameters.Add("@N42Name", SqlDbType.NVarChar, 100).Value = (data.N42Name ?? string.Empty).Trim();

                cmd.Parameters.Add("@XMLTranslation", SqlDbType.Xml).Value = GenerateXmlTranslation(data.ProductTranslation);
                cmd.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                cn.Open();
                transaction = cn.BeginTransaction();
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();

                resultCode = GetInt(cmd.Parameters["@Return"].Value, -1);
                if (resultCode > 0)
                {
                    transaction.Commit();
                }
                else
                {
                    throw new DatabaseException($"[{resultCode}] Merchandise update failed");
                }

                response.Code = 0;
                response.ReturnValue = resultCode;
                response.Message = $"{ReplaceSpecialCharacters(data.Name)} successfully saved.";
                response.Status = true;
                return Ok(response);
            }
            catch (DatabaseException ex)
            {
                _logger.LogError(ex, "SaveSupplierNetworkMerchandise: database error occurred");
                try
                {
                    transaction?.Rollback();
                    transaction?.Dispose();
                }
                catch
                {
                }

                if (resultCode == 0)
                {
                    resultCode = 500;
                }

                response.Code = resultCode;
                response.Status = false;
                response.Message = "Save merchandise failed";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveSupplierNetworkMerchandise: unexpected error");
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                return Ok(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SaveSupplierNetworkMerchandise: missing or invalid parameters");
            response.Code = 400;
            response.Message = "Missing or invalid parameters";
            response.Parameters = new List<Param> { new() { Name = "data", Value = "Merchandise" } };
            return Ok(response);
        }
    }

    [HttpGet("/api/NetworkSupplier/ingredient/b/{codesite:int}/{codetrans:int}/{codesetprice:int}/{type:int}/{codeliste:int}")]
    public ActionResult<List<Ingredient>> GetMerchandiseIngredient(
        int codesite,
        int codetrans,
        int codesetprice,
        int type,
        int codeliste,
        [FromQuery] string name = "",
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10,
        [FromQuery] int searchtype = 0,
        [FromQuery] int category = -1,
        [FromQuery] int sharetype = 0,
        [FromQuery] int namefilter = 0,
        [FromQuery] int isfulltext = 0,
        [FromQuery] int sortby = 1,
        [FromQuery] int status = -1)
    {
        try
        {
            var ingredients = new List<Ingredient>();
            using var cmd = new SqlCommand { CommandTimeout = 1200 };
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_MerchandiseIngredient]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = GetInt(codesite, -1);
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = GetInt(codetrans, -1);
            cmd.Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = GetInt(codesetprice, -1);
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 250).Value = GetStr(name);
            cmd.Parameters.Add("@SearchType", SqlDbType.Int).Value = GetInt(searchtype, 0);
            cmd.Parameters.Add("@Category", SqlDbType.Int).Value = GetInt(category, -1);
            cmd.Parameters.Add("@ShareType", SqlDbType.Int).Value = GetInt(sharetype, 0);
            cmd.Parameters.Add("@pimstatus", SqlDbType.Int).Value = GetInt(status, -1);
            cmd.Parameters.Add("@intType", SqlDbType.Int).Value = GetInt(type, 0);
            cmd.Parameters.Add("@skip", SqlDbType.Int).Value = skip;
            cmd.Parameters.Add("@rowsPerPage", SqlDbType.Int).Value = take;
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = GetInt(codeliste, -1);
            if (isfulltext == 0)
            {
                cmd.Parameters.Add("@NameFilter", SqlDbType.Int).Value = GetInt(namefilter, 0);
            }

            cmd.Parameters.Add("@SortBy", SqlDbType.Int).Value = GetInt(sortby, 0);
            cmd.Parameters.Add("@IsFullText", SqlDbType.Int).Value = GetInt(isfulltext, 0);

            cn.Open();
            using var dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    _ = GetInt(dr["Total"]);
                }
            }

            if (dr.NextResult())
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ingredients.Add(new Ingredient
                        {
                            CodeListe = GetInt(dr["CodeListe"]),
                            Name = GetStr(dr["Name"]),
                            Number = GetStr(dr["Number"]),
                            CodeUser = GetInt(dr["CodeUser"]),
                            Type = GetInt(dr["Type"]),
                            Price = GetDbl(dr["Price"]),
                            UnitName = GetStr(dr["UnitName"]),
                            UnitMetric = GetStr(dr["UnitMetric"]),
                            UnitImperial = GetStr(dr["UnitImperial"]),
                            CodeUnit = GetInt(dr["CodeUnit"]),
                            CodeUnitMetric = GetInt(dr["CodeUnitMetric"]),
                            CodeUnitImperial = GetInt(dr["CodeUnitImperial"]),
                            CategoryName = GetStr(dr["CategoryName"]),
                            SourceName = GetStr(dr["SourceName"]),
                            SupplierName = GetStr(dr["SupplierName"]),
                            CodeBrand = GetInt(dr["CodeBrand"]),
                            BrandName = GetStr(dr["BrandName"]),
                            Wastage1 = GetInt(dr["Wastage1"]),
                            Wastage2 = GetInt(dr["Wastage2"]),
                            Wastage3 = GetInt(dr["Wastage3"]),
                            Wastage4 = GetInt(dr["Wastage4"]),
                            Wastage5 = GetInt(dr["Wastage5"]),
                            WastageTotal = GetInt(dr["WastageTotal"]),
                            Status = dr["Status"],
                            ImposedPrice = GetDbl(dr["ImposedPrice"]),
                            Constant = GetInt(dr["Constant"]),
                            Preparation = GetStr(dr["Preparation"]),
                            Allprice = DisplayAllPrice(GetInt(dr["CodeListe"]), codesetprice, type),
                            withTranslation = GetInt(dr["withTranslation"]),
                            isLocked = GetBool(dr["isLocked"]),
                            yieldIng = GetDbl(dr["Yield"])
                        });
                    }
                }
            }

            dr.Close();
            return Ok(ingredients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetMerchandiseIngredient: unexpected error");
            return StatusCode(500);
        }
    }

    private string DisplayAllPrice(int codeListe, int codeSetPrice, int type)
    {
        var allPrice = string.Empty;
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_IngredientAllSetPrice]";
            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeListe;
            cmd.Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = codeSetPrice;
            cmd.Parameters.Add("@mainType", SqlDbType.Int).Value = GetInt(type, 0);
            cn.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                allPrice += Convert.ToString(dr["Allprice"], CultureInfo.InvariantCulture) + ",";
            }

            dr.Close();
            return allPrice;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DisplayAllPrice: unexpected error");
            throw new DatabaseException("Request failed");
        }
    }

    [HttpGet("/api/NetworkSupplier/GetSupplierNetworkStatus")]
    public ActionResult<string> GetSupplierNetworkStatus()
    {
        using var cmd = new SqlCommand();
        using var cn = new SqlConnection(ConnectionString);
        try
        {
            cmd.CommandTimeout = 120;
            cmd.Connection = cn;
            cmd.CommandText = "API_GetSupplierNetworkStatus";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Status", SqlDbType.Int).Direction = ParameterDirection.Output;
            cn.Open();
            cmd.ExecuteNonQuery();

            return Ok(GetStr(cmd.Parameters["@Status"].Value, "0"));
        }
        catch (DatabaseException)
        {
            return Ok("0");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetSupplierNetworkStatus: unexpected error");
            return Ok("0");
        }
    }

    private static string GenerateXmlTranslation(List<SupplierProductTranslation> productTranslations)
    {
        if (productTranslations == null || productTranslations.Count == 0)
        {
            return string.Empty;
        }

        var xml = string.Empty;
        foreach (var data in productTranslations)
        {
            xml += string.Format(
                CultureInfo.InvariantCulture,
                "<Table><CodeTrans>{0}</CodeTrans><EgsRef>{1}</EgsRef><Name>{2}</Name><OriginalName>{3}</OriginalName><Category>{4}</Category><Brand>{5}</Brand><Description>{6}</Description><Declaration>{7}</Declaration><Ingredients>{8}</Ingredients><Preparation>{9}</Preparation><CookingTip>{10}</CookingTip><Refinement>{11}</Refinement><Storage>{12}</Storage><Productivity>{13}</Productivity><SpecificDetermination>{14}</SpecificDetermination></Table>\n",
                XmlEscape(string.IsNullOrEmpty(data.CodeTrans) ? string.Empty : data.CodeTrans),
                XmlEscape(string.IsNullOrEmpty(data.EgsRef) ? string.Empty : data.EgsRef),
                XmlEscape(string.IsNullOrEmpty(data.Name) ? string.Empty : data.Name),
                XmlEscape(string.IsNullOrEmpty(data.OriginalName) ? string.Empty : data.OriginalName),
                XmlEscape(string.IsNullOrEmpty(data.Category) ? string.Empty : data.Category),
                XmlEscape(string.IsNullOrEmpty(data.Brand) ? string.Empty : data.Brand),
                XmlEscape(string.IsNullOrEmpty(data.Description) ? string.Empty : data.Description),
                XmlEscape(string.IsNullOrEmpty(data.Declaration) ? string.Empty : data.Declaration),
                XmlEscape(string.IsNullOrEmpty(data.Ingredients) ? string.Empty : data.Ingredients),
                XmlEscape(string.IsNullOrEmpty(data.Preparation) ? string.Empty : data.Preparation),
                XmlEscape(string.IsNullOrEmpty(data.CookingTip) ? string.Empty : data.CookingTip),
                XmlEscape(string.IsNullOrEmpty(data.Refinement) ? string.Empty : data.Refinement),
                XmlEscape(string.IsNullOrEmpty(data.Storage) ? string.Empty : data.Storage),
                XmlEscape(string.IsNullOrEmpty(data.Productivity) ? string.Empty : data.Productivity),
                XmlEscape(string.IsNullOrEmpty(data.SpecificDetermination) ? string.Empty : data.SpecificDetermination));
        }

        return string.IsNullOrEmpty(xml) ? string.Empty : $"<NewDataSet>{xml}</NewDataSet>";
    }

    private static string XmlEscape(string data)
    {
        return data.Replace("&", "&amp;")
            .Replace("'", "&apos;")
            .Replace("\"", "&quot;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;");
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
        if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var i)) return i != 0;
        if (bool.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var bb)) return bb;
        return false;
    }

    private static double GetDbl(object? value, double fallback = 0)
    {
        if (value == null || value == DBNull.Value) return fallback;
        if (value is double d) return d;
        if (double.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var dd)) return dd;
        try { return Convert.ToDouble(value, CultureInfo.InvariantCulture); } catch { return fallback; }
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI
{
    [ApiController]
    [Route("api/[controller]")]
    public class NutrientController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("nutrientdb/{codesite:int}/{codeset:int}/{codetrans:int}/{page:int}/{searchstring?}")]
        public ActionResult<DataSet> GetNutrientTest(int codesite, int codeset, int codetrans, int page, string searchstring = "")
        {
            const int pageCount = 25;
            try
            {
                var ds = new DataSet();
                var ds2 = new DataSet();
                using (var cmd = new SqlCommand())
                {
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
                        da2.Fill(ds2);
                    }
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
                if (ds2.Tables.Count > 1) ds2.Tables.RemoveAt(1);
                ds2.Tables[0].TableName = "NutrientDefinition";
                filtered.TableName = "Nutrients";
                ds2.Tables.Add(filtered);
                return Ok(ds2);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("nutrient/{codeliste:int}/{codenutrientset:int}/{codesite:int}/{codetrans:int}/{imposedtype:int}")]
        public ActionResult<List<Models.RecipeNutrition>> GetNutrient(int codeliste, int codenutrientset, int codesite, int codetrans, int imposedtype)
        {
            var nutrients = new List<Models.RecipeNutrition>();
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
                        var n = new Models.RecipeNutrition
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
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("nutrientset/{codesite:int}")]
        public ActionResult<List<Models.GenericCodeValueList>> GetNutrientSet(int codesite)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "API_GET_NutrientSetList";
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                var nutrientsets = new List<Models.GenericCodeValueList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    nutrientsets.Add(new Models.GenericCodeValueList
                    {
                        Code = GetInt(r["Code"]),
                        Value = GetStr(r["DisplayName"]) 
                    });
                }
                return Ok(nutrientsets);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("nutrientsetheader/{CodeNutrientDB:int?}/{CodeTrans:int?}/{CodeSite:int?}/{CodeSet:int?}")]
        public ActionResult<List<Models.GenericList>> GetNutrientSetHeader(int CodeNutrientDB = -1, int CodeTrans = -1, int CodeSite = -1, int CodeSet = -1)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_NutrientsAsHeader]";
                cmd.Parameters.Add("@intCodeNutrientDB", SqlDbType.Int).Value = CodeNutrientDB;
                cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = CodeTrans;
                cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = CodeSite;
                cmd.Parameters.Add("@intCodeSet", SqlDbType.Int).Value = CodeSet;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                var nutrientsets = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    nutrientsets.Add(new Models.GenericList
                    {
                        Code = GetInt(r["Nutr_No"]),
                        Value = GetStr(r["Name"]) 
                    });
                }
                return Ok(nutrientsets);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("nutrientlinks/{codesite:int}/{codeset:int}/{codetrans:int}/{filtertype:int}/{take:int?}/{skip:int?}/{searchstring?}")]
        public ActionResult<Models.NutrientListData> GetNutrientDataPerSet(int codesite, int codeset, int codetrans, int filtertype, int take = 10, int skip = 0, string searchstring = "")
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                if (searchstring == null) searchstring = string.Empty;
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[GetNutrientDataPerSet]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@CodeSet", SqlDbType.Int).Value = codeset;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                cmd.Parameters.Add("@nvcSearchString", SqlDbType.NVarChar).Value = searchstring;
                cmd.Parameters.Add("@intFilterType", SqlDbType.Int).Value = filtertype;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var nutrients = new List<Models.NutrientList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    var n = new Models.NutrientList
                    {
                        NDB_No = GetStr(r["NDB_No"]),
                        Name = GetStr(r["Desc"])
                    };
                    var cols = ds.Tables[0].Columns.Count;
                    if (cols > 5) n.N1 = GetDbl(r["val1"]);
                    if (cols > 6) n.N2 = GetDbl(r["val2"]);
                    if (cols > 7) n.N3 = GetDbl(r["val3"]);
                    if (cols > 8) n.N4 = GetDbl(r["val4"]);
                    if (cols > 9) n.N5 = GetDbl(r["val5"]);
                    if (cols > 10) n.N6 = GetDbl(r["val6"]);
                    if (cols > 11) n.N7 = GetDbl(r["val7"]);
                    if (cols > 12) n.N8 = GetDbl(r["val8"]);
                    if (cols > 13) n.N9 = GetDbl(r["val9"]);
                    if (cols > 14) n.N10 = GetDbl(r["val10"]);
                    if (cols > 15) n.N11 = GetDbl(r["val11"]);
                    if (cols > 16) n.N12 = GetDbl(r["val12"]);
                    if (cols > 17) n.N13 = GetDbl(r["val13"]);
                    if (cols > 18) n.N14 = GetDbl(r["val14"]);
                    if (cols > 19) n.N15 = GetDbl(r["val15"]);
                    if (cols > 20) n.N16 = GetDbl(r["val16"]);
                    if (cols > 21) n.N17 = GetDbl(r["val17"]);
                    if (cols > 22) n.N18 = GetDbl(r["val18"]);
                    if (cols > 23) n.N19 = GetDbl(r["val19"]);
                    if (cols > 24) n.N20 = GetDbl(r["val20"]);
                    if (cols > 25) n.N21 = GetDbl(r["val21"]);
                    if (cols > 26) n.N22 = GetDbl(r["val22"]);
                    if (cols > 27) n.N23 = GetDbl(r["val23"]);
                    if (cols > 28) n.N24 = GetDbl(r["val24"]);
                    if (cols > 29) n.N25 = GetDbl(r["val25"]);
                    if (cols > 30) n.N26 = GetDbl(r["val26"]);
                    if (cols > 31) n.N27 = GetDbl(r["val27"]);
                    if (cols > 32) n.N28 = GetDbl(r["val28"]);
                    if (cols > 33) n.N29 = GetDbl(r["val29"]);
                    if (cols > 34) n.N30 = GetDbl(r["val30"]);
                    if (cols > 35) n.N31 = GetDbl(r["val31"]);
                    if (cols > 36) n.N32 = GetDbl(r["val32"]);
                    if (cols > 37) n.N33 = GetDbl(r["val33"]);
                    if (cols > 38) n.N34 = GetDbl(r["val34"]);
                    nutrients.Add(n);
                }

                var totalCount = nutrients.Count;
                take = take > totalCount + 1 ? totalCount : take;
                take = take <= 0 ? 1 : take;
                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                skip = skip > totalPages ? totalPages : skip;
                skip = skip < 1 ? 0 : skip;
                var data = new Models.NutrientListData(nutrients.Skip(take * skip).Take(take).ToList(), totalCount);
                return Ok(data);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        // Helpers
        private static int GetInt(object value, int fallback = 0) { if (value == null || value == DBNull.Value) return fallback; if (int.TryParse(Convert.ToString(value), out var i)) return i; try { return Convert.ToInt32(value); } catch { return fallback; } }
        private static double GetDbl(object value, double fallback = 0) { if (value == null || value == DBNull.Value) return fallback; if (double.TryParse(Convert.ToString(value), out var d)) return d; try { return Convert.ToDouble(value); } catch { return fallback; } }
        private static string GetStr(object value) => value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value);
        private static bool GetBool(object value) { if (value == null || value == DBNull.Value) return false; if (value is bool b) return b; if (int.TryParse(Convert.ToString(value), out var i)) return i != 0; if (bool.TryParse(Convert.ToString(value), out var bb)) return bb; return false; }
        private static double SafePos(double v) => v < 0 ? 0 : v;
    }
}

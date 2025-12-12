using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI_CMC_CoopGastro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AllergenController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("{codetrans:int}/{codesite:int}/{name?}")]
        public ActionResult<List<Models.BrandTreeNode>> GetAllergens(int codetrans, int codesite, string? name = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name) || name == "null" || name == "undefined") name = null;
                var ds = new DataSet();
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

                var allergens = new List<Models.BrandTreeNode>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    allergens.Add(new Models.BrandTreeNode
                    {
                        title = GetStr(r["Name"]),
                        key = GetStr(r["Code"]),
                        hasPicture = true,
                        picture = GetStr(r["PictureName"]) 
                    });
                }
                return Ok(allergens);
            }
            catch (ArgumentException)
            {
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("allergensnew/{codetrans:int}/{codesite:int}/{name?}")]
        public ActionResult<List<Models.BrandTreeNode>> GetAllergensNew(int codetrans, int codesite, string? name = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name) || name == "null" || name == "undefined") name = null;
                var ds = new DataSet();
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

                var allergens = new List<Models.BrandTreeNode>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    allergens.Add(new Models.BrandTreeNode
                    {
                        title = GetStr(r["Name"]),
                        key = GetStr(r["Code"]),
                        hasPicture = true,
                        picture = GetStr(r["PictureName"]) 
                    });
                }
                return Ok(allergens);
            }
            catch (ArgumentException)
            {
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("list/{codetrans:int}")]
        public ActionResult<List<Models.GenericCodeValueList>> GetAllergenList(int codetrans)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_Allergens]";
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var allergens = new List<Models.GenericCodeValueList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    allergens.Add(new Models.GenericCodeValueList
                    {
                        Code = r["Code"],
                        Value = r["Name"]
                    });
                }
                return Ok(allergens);
            }
            catch (ArgumentException)
            {
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("{codeliste:int}/{codetrans:int}/{codesite:int}")]
        public ActionResult<List<Models.ListeAllergen>> GetListeAllergen(int codeliste, int codetrans, int codesite)
        {
            try
            {
                var ds = new DataSet();
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

                var allergens = new List<Models.ListeAllergen>();
                foreach (DataRow p in ds.Tables[0].Rows)
                {
                    allergens.Add(new Models.ListeAllergen
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
            catch (ArgumentException)
            {
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("ingredient/{codeliste:int}/{codetrans:int}/{codesite:int}")]
        public ActionResult<List<Models.IngredientAllergen>> GetIngredientAllergen(int codeliste, int codetrans, int codesite)
        {
            try
            {
                var ds = new DataSet();
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

                var allergens = new List<Models.IngredientAllergen>();
                foreach (DataRow p in ds.Tables[0].Rows)
                {
                    var contain = GetBool(p["Contain"]);
                    var trace = GetBool(p["Trace"]);
                    if (contain || trace)
                    {
                        allergens.Add(new Models.IngredientAllergen
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
            catch (ArgumentException)
            {
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("derived/{codeliste:int}")]
        public ActionResult<List<Models.ListeAllergen>> GetListeAllergenDerived(int codeliste)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_RecipeDerivedAllergens]";
                cmd.Parameters.Add("@FirstCode", SqlDbType.Int).Value = codeliste;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var allergens = new List<Models.ListeAllergen>();
                foreach (DataRow p in ds.Tables[0].Rows)
                {
                    allergens.Add(new Models.ListeAllergen
                    {
                        CodeListe = p["SecondCode"],
                        CodeAllergen = p["CodeAllergen"],
                        Contain = p["Contain"],
                        Trace = p["Trace"]
                    });
                }
                return Ok(allergens);
            }
            catch (ArgumentException)
            {
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception)
            {
                return Problem(title: "Request failed", statusCode: 500);
            }
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
    }

    // Placeholder models - replace with actual project models
    namespace Models
    {
        public class BrandTreeNode { public string title { get; set; } = string.Empty; public string key { get; set; } = string.Empty; public bool hasPicture { get; set; } public string picture { get; set; } = string.Empty; }
        public class GenericCodeValueList { public object Code { get; set; } = default!; public object Value { get; set; } = default!; }
        public class ListeAllergen { public object CodeListe { get; set; } = default!; public object CodeAllergen { get; set; } = default!; public object Contain { get; set; } = default!; public object Trace { get; set; } = default!; public object NonAllergen { get; set; } = default!; public object Derived { get; set; } = default!; public object Hidden { get; set; } = default!; }
        public class IngredientAllergen { public object CodeListe { get; set; } = default!; public object CodeAllergen { get; set; } = default!; public object Contain { get; set; } = default!; public object Trace { get; set; } = default!; public object NonAllergen { get; set; } = default!; }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using log4net;
using Microsoft.AspNetCore.Mvc;
using static EgsData.modGlobalDeclarations; // ConnectionString
using static EgsData.modFunctions;         // GetStr, GetBool, GetInt

namespace CalcmenuAPI.Core.Controllers
{
    [ApiController]
    public class AllergenController : ControllerBase
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType);

        [HttpGet("/api/allergens/{codetrans:int}/{codesite:int}/{name?}")]
        public ActionResult<List<Models.BrandTreeNode>> GetAllergens(int codetrans, int codesite, string? name = "")
        {
            try
            {
                var ds = new DataSet();
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(name) || name == "null" || name == "undefined") name = null;
                        cmd.Connection = cn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "[dbo].[API_GET_Allergens]";
                        cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                        cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                        cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = (object?)name ?? DBNull.Value;
                        cn.Open();
                        using var _da = new SqlDataAdapter(cmd);
                        _da.Fill(ds);
                    }
                    finally
                    {
                        cn.Close();
                        (cn as IDisposable)?.Dispose();
                    }
                }

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

                return Ok(allergens.ToList());
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("/api/allergensnew/{codetrans:int}/{codesite:int}/{name?}")]
        public ActionResult<List<Models.BrandTreeNode>> GetAllergensNew(int codetrans, int codesite, string? name = "")
        {
            try
            {
                var ds = new DataSet();
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(name) || name == "null" || name == "undefined") name = null;
                        cmd.Connection = cn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "[dbo].[API_GET_Allergens]";
                        cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                        cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = (object?)name ?? DBNull.Value;
                        cmd.Parameters.Add("@CodeSite", SqlDbType.VarChar).Value = codesite;
                        cn.Open();
                        using var _da = new SqlDataAdapter(cmd);
                        _da.Fill(ds);
                    }
                    finally
                    {
                        cn.Close();
                        (cn as IDisposable)?.Dispose();
                    }
                }

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

                return Ok(allergens.ToList());
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("/api/allergen/list/{codetrans:int}")]
        public ActionResult<List<Models.GenericCodeValueList>> GetAllergenList(int CodeTrans)
        {
            var allergens = new List<Models.GenericCodeValueList>();
            try
            {
                var ds = new DataSet();
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        cmd.Connection = cn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "[dbo].[API_GET_Allergens]";
                        cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = CodeTrans;
                        cn.Open();
                        using var _da = new SqlDataAdapter(cmd);
                        _da.Fill(ds);
                    }
                    finally
                    {
                        cn.Close();
                        (cn as IDisposable)?.Dispose();
                    }
                }

                allergens = ds.Tables[0].AsEnumerable()
                    .Select(p => new Models.GenericCodeValueList
                    {
                        Code = p.Field<object>("Code"),
                        Value = p.Field<object>("Name")
                    })
                    .ToList();

                return Ok(allergens);
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("/api/allergen/{codeliste:int}/{codetrans:int}/{codesite:int}")]
        public ActionResult<List<Models.ListeAllergen>> GetListeAllergen(int CodeListe, int CodeTrans, int CodeSite)
        {
            var allergens = new List<Models.ListeAllergen>();
            try
            {
                var ds = new DataSet();
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        cmd.Connection = cn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "[dbo].[API_GET_ListeAllergen]";
                        cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = CodeListe;
                        cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = CodeTrans;
                        cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = CodeSite;
                        cn.Open();
                        using var _da = new SqlDataAdapter(cmd);
                        _da.Fill(ds);
                    }
                    finally
                    {
                        cn.Close();
                        (cn as IDisposable)?.Dispose();
                    }
                }

                allergens = ds.Tables[0].AsEnumerable()
                    .Select(p => new Models.ListeAllergen
                    {
                        CodeListe = CodeListe,
                        CodeAllergen = p.Field<object>("CodeAllergen"),
                        Contain = p.Field<object>("Contain"),
                        Trace = p.Field<object>("Trace"),
                        NonAllergen = p.Field<object>("NonAllergen"),
                        Derived = p.Field<object>("Derived"),
                        Hidden = p.Field<object>("Hidden")
                    })
                    .ToList();

                return Ok(allergens);
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("/api/allergen/ingredient/{codeliste:int}/{codetrans:int}/{codesite:int}")]
        public ActionResult<List<Models.IngredientAllergen>> GetIngredientAllergen(int CodeListe, int CodeTrans, int CodeSite)
        {
            var allergens = new List<Models.IngredientAllergen>();
            try
            {
                var ds = new DataSet();
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        cmd.Connection = cn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "[dbo].[API_GET_ListeAllergen]";
                        cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = CodeListe;
                        cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = CodeTrans;
                        cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = CodeSite;
                        cn.Open();
                        using var _da = new SqlDataAdapter(cmd);
                        _da.Fill(ds);
                    }
                    finally
                    {
                        cn.Close();
                        (cn as IDisposable)?.Dispose();
                    }
                }

                allergens = ds.Tables[0].AsEnumerable()
                    .Where(p => GetBool(p.Field<object>("Contain")) || GetBool(p.Field<object>("Trace")))
                    .Select(p => new Models.IngredientAllergen
                    {
                        CodeListe = CodeListe,
                        CodeAllergen = p.Field<object>("CodeAllergen"),
                        Contain = p.Field<object>("Contain"),
                        Trace = p.Field<object>("Trace"),
                        NonAllergen = p.Field<object>("NonAllergen")
                    })
                    .ToList();

                return Ok(allergens);
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }

        [HttpGet("/api/allergen/derived/{codeliste:int}")]
        public ActionResult<List<Models.ListeAllergen>> GetListeAllergenDerived(int CodeListe)
        {
            var allergens = new List<Models.ListeAllergen>();
            try
            {
                var ds = new DataSet();
                using (var cmd = new SqlCommand())
                using (var cn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        cmd.Connection = cn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "[dbo].[API_GET_RecipeDerivedAllergens]";
                        cmd.Parameters.Add("@FirstCode", SqlDbType.Int).Value = CodeListe;
                        cn.Open();
                        using var _da = new SqlDataAdapter(cmd);
                        _da.Fill(ds);
                    }
                    finally
                    {
                        cn.Close();
                        (cn as IDisposable)?.Dispose();
                    }
                }

                allergens = ds.Tables[0].AsEnumerable()
                    .Select(p => new Models.ListeAllergen
                    {
                        CodeListe = p.Field<object>("SecondCode"),
                        CodeAllergen = p.Field<object>("CodeAllergen"),
                        Contain = p.Field<object>("Contain"),
                        Trace = p.Field<object>("Trace")
                    })
                    .ToList();

                return Ok(allergens);
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                return Problem(title: "Request failed", statusCode: 400);
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                return Problem(title: "Request failed", statusCode: 500);
            }
        }
    }
}

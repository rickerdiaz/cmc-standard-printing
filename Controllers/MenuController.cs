using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("{codeliste:int}/{codesite:int}/{codetrans:int}/{codesetprice:int}/{codenutrientset:int}")]
        public ActionResult<Models.MenuData> GetMenu(int codeliste, int codesite, int codetrans, int codesetprice, int codenutrientset, int codeuser = 0, int rolelevel = 0)
        {
            var menudata = new Models.MenuData();
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_MenuInfo]";
                cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeliste;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                cmd.Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = codesetprice;
                cmd.Parameters.Add("@CodeNutrientSet", SqlDbType.Int).Value = codenutrientset;
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        var info = new Models.Menu();
                        while (dr.Read())
                        {
                            info.Type = 16;
                            info.CodeListe = GetInt(dr["Code"]);
                            info.Number = GetStr(dr["Number"]);
                            info.Name = GetStr(dr["Name"]);
                            info.Category = GetStr(dr["Category"]);
                            info.CodeCategory = GetInt(dr["CodeCategory"]);
                            info.Remark = GetStr(dr["Remark"]);
                            info.Yield = GetDbl(dr["Yield"]);
                            info.YieldUnit = GetStr(dr["YieldUnit"]);
                            info.CodeYieldUnit = GetInt(dr["CodeYieldUnit"]);
                            info.CodeTrans = GetInt(dr["CodeTrans"]);
                            info.Date1 = GetDate(dr["Dates"]).ToString("MM/dd/yyyy");
                            info.Description = GetStr(dr["Description"]);
                            info.SrWeight = GetDbl(dr["SrWeight"]);
                            info.SrQty = GetDbl(dr["SrQty"]);
                            info.SrUnit = GetStr(dr["SrUnit"]);
                            info.SrUnitCode = GetInt(dr["SrUnitCode"]);
                            info.CodeSite = GetInt(dr["CodeSite"]);
                            info.CodeUser = GetInt(dr["CodeUser"]);
                            info.DateCreated = GetStr(dr["DateCreated"]);
                            info.DateLastModified = GetStr(dr["DateLastModified"]);
                            info.CreatedBy = GetStr(dr["CreatedBy"]);
                            info.CodeCreatedBy = GetInt(dr["CodeCreatedBy"], -1);
                            info.ModifiedBy = GetStr(dr["ModifiedBy"]);
                            info.CodeModifiedBy = GetInt(dr["CodeModifiedBy"], -1);
                            info.Pictures = GetStr(dr["Pictures"]);
                            info.DefaultPicture = GetInt(dr["DefaultPicture"]);
                            info.Note = GetStr(dr["Note"]);
                            info.FootNote1 = GetStr(dr["FootNote1"]);
                            info.FootNote2 = GetStr(dr["FootNote2"]);
                            info.FootNote1Clean = GetStr(dr["FootNote1Clean"]);
                            info.FootNote2Clean = GetStr(dr["FootNote2Clean"]);
                            info.MethodFormat = GetStr(dr["MethodFormat"]);
                            info.IsGlobal = GetBool(dr["isGlobal"]);
                            info.Source = GetStr(dr["Source"]);
                            info.CodeSource = GetInt(dr["CodeSource"]);
                        }
                        menudata.Info = info;
                    }

                    dr.NextResult();
                    if (dr.HasRows)
                    {
                        var attachments = new List<Models.MenuAttachment>();
                        while (dr.Read())
                        {
                            attachments.Add(new Models.MenuAttachment
                            {
                                Id = GetInt(dr["Id"]),
                                Name = GetStr(dr["Name"]),
                                Resource = GetStr(dr["Resource"]),
                                Type = GetInt(dr["Type"]),
                                IsDefault = GetBool(dr["Default"]) 
                            });
                        }
                        menudata.Attachment = attachments;
                    }

                    dr.NextResult();
                    if (dr.HasRows)
                    {
                        var calculation = new List<Models.MenuCalculation>();
                        while (dr.Read())
                        {
                            var calc = new Models.MenuCalculation
                            {
                                Id = GetInt(dr["Id"]),
                                CodeListe = GetInt(dr["CodeListe"]),
                                Coef = GetDbl(dr["Coef"]),
                                CalcPrice = GetDbl(dr["CalcPrice"]),
                                ImposedPrice = GetDbl(dr["ImposedPrice"]),
                                CodeSetPrice = GetInt(dr["CodeSetPrice"]),
                                Tax = GetInt(dr["Tax"]),
                                TaxValue = GetDbl(dr["TaxValue"]) 
                            };
                            calculation.Add(calc);
                        }
                        menudata.Calculation = calculation;
                    }

                    dr.NextResult();
                    if (dr.HasRows)
                    {
                        var nutrients = new List<Models.MenuNutrition>();
                        while (dr.Read())
                        {
                            nutrients.Add(new Models.MenuNutrition
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
                            });
                        }
                        menudata.Nutrition = nutrients;
                    }

                    dr.NextResult();
                    var ingredientTranslation = new List<Models.MenuIngredientTranslation>();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ingredientTranslation.Add(new Models.MenuIngredientTranslation
                            {
                                ItemId = GetInt(dr["ID"]),
                                CodeTrans = GetInt(dr["CodeTrans"]),
                                Name = GetStr(dr["Name"]),
                                Remark = GetStr(dr["Remark"]),
                                Complement = GetStr(dr["Complement"]),
                                Note = GetStr(dr["Note"]),
                                Preparation = GetStr(dr["Preparation"]),
                                AlternativeIngredient = GetStr(dr["AlternativeIngredient"]),
                                Step = GetInt(dr["Step"]) 
                            });
                        }
                    }

                    dr.NextResult();
                    if (dr.HasRows)
                    {
                        var ingredient = new List<Models.MenuIngredient>();
                        while (dr.Read())
                        {
                            var it = new Models.MenuIngredient
                            {
                                CodeListe = codeliste,
                                CodeUser = GetInt(dr["CodeUser"]),
                                ItemId = GetInt(dr["ItemId"]),
                                ItemCode = GetInt(dr["ItemCode"]),
                                ItemName = GetStr(dr["ItemName"]),
                                ItemType = GetInt(dr["ItemType"]),
                                ItemQty = GetDbl(dr["ItemQty"]),
                                ItemCodeUnit = GetInt(dr["ItemCodeUnit"]),
                                ItemUnit = GetStr(dr["ItemUnit"]),
                                Step = GetInt(dr["Step"]),
                                Position = GetInt(dr["Position"]),
                                itemSellingPrice = GetDbl(dr["itemSellingPrice"]),
                                Cons = GetDbl(dr["Const"]),
                                ImposedPrice = GetDbl(dr["ImposedPrice"]),
                                Complement = GetStr(dr["Complement"]),
                                Preparation = GetStr(dr["Preparation"]),
                                AlternativeIngredient = GetStr(dr["AlternativeIngredient"]),
                                TmpName = GetStr(dr["TmpName"]),
                                TmpQty = GetStr(dr["TmpQty"]),
                                TmpUnit = GetStr(dr["TmpUnit"]),
                                TmpComplement = GetStr(dr["TmpComplement"]),
                                TmpPreparation = GetStr(dr["TmpPreparation"]),
                                Wastage1 = GetInt(dr["Wastage1"]),
                                Wastage2 = GetInt(dr["Wastage2"]),
                                Wastage3 = GetInt(dr["Wastage3"]),
                                Wastage4 = GetInt(dr["Wastage4"]),
                                Price = GetDbl(dr["Price"]),
                                YieldIng = GetDbl(dr["Yield"]),
                                PriceUnit = GetStr(dr["PriceUnit"]),
                                Amount = GetDbl(dr["Amount"]),
                                QuantityMetric = GetDbl(dr["QuantityMetric"]),
                                CodeUnitMetric = GetInt(dr["CodeUnitMetric"]),
                                UnitMetric = GetStr(dr["UnitMetric"]),
                                QuantityImperial = GetDbl(dr["QuantityImperial"]),
                                CodeUnitImperial = GetInt(dr["CodeUnitImperial"]),
                                UnitImperial = GetStr(dr["UnitImperial"]),
                                ConvertDirection = GetInt(dr["ConvertDirection"]),
                                IsQuickEncode = GetBool(GetInt(dr["ItemType"]) == 0),
                                IsAllowMetricImperial = false,
                                ApprovalStatusCode = GetInt(dr["ApprovalStatusCode"]),
                                ApprovalRequestedBy = GetInt(dr["ApprovalRequestedBy"]),
                                ApprovalRequestedDate = GetStr(dr["ApprovalRequestedDate"]),
                                ApprovalBy = GetInt(dr["ApprovalBy"]),
                                ApprovalDate = GetStr(dr["ApprovalDate"]),
                                CodeBrand = GetInt(dr["CodeBrand"]),
                                CodeUnitDisplaySelection = GetInt(dr["CodeUnitDisplaySelection"]),
                                isLocked = GetBool(dr["isLocked"]),
                                Remark = GetStr(dr["Remark"]),
                                Factor = GetDbl(dr["Factor"])
                            };
                            it.Translation = GetInt(dr["ItemType"]) != 75
                                ? ingredientTranslation.Where(c => c.ItemId == it.ItemId).ToList()
                                : ingredientTranslation.Where(c => c.Step == it.Step).ToList();
                            ingredient.Add(it);
                        }
                        menudata.Ingredient = ingredient;
                    }

                    dr.NextResult();
                    var procedureTranslation = new List<Models.MenuProcedureTranslation>();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            procedureTranslation.Add(new Models.MenuProcedureTranslation
                            {
                                NoteId = GetInt(dr["NoteId"]),
                                CodeTrans = GetInt(dr["CodeTrans"]),
                                Note = GetStr(dr["Note"]),
                                AbbrevNote = GetStr(dr["AbbrevNote"]) 
                            });
                        }
                    }

                    dr.NextResult();
                    if (dr.HasRows)
                    {
                        var procedure = new List<Models.MenuProcedure>();
                        while (dr.Read())
                        {
                            procedure.Add(new Models.MenuProcedure
                            {
                                NoteId = GetInt(dr["NoteId"]),
                                Position = GetInt(dr["Position"]),
                                Note = GetStr(dr["Note"]),
                                AbbrevNote = GetStr(dr["AbbrevNote"]),
                                Translation = procedureTranslation.Where(c => c.NoteId == GetInt(dr["NoteId"])).ToList(),
                                Picture = GetStr(dr["Picture"]),
                                hasPicture = GetStr(dr["hasPicture"]) 
                            });
                        }
                        menudata.Procedure = procedure;
                    }

                    dr.NextResult();
                    if (dr.HasRows)
                    {
                        var translations = new List<Models.MenuTranslation>();
                        while (dr.Read())
                        {
                            translations.Add(new Models.MenuTranslation
                            {
                                Id = GetInt(dr["ID"]),
                                CodeTrans = GetInt(dr["CodeTrans"]),
                                TranslationName = GetStr(dr["TranslationName"]),
                                Name = GetStr(dr["Name"]),
                                Remark = GetStr(dr["Remark"]),
                                Description = GetStr(dr["Description"]),
                                Notes = GetStr(dr["Notes"]) 
                            });
                        }
                        menudata.Translation = translations;
                    }

                    dr.NextResult();
                    if (dr.HasRows)
                    {
                        var comments = new List<Models.MenuComment>();
                        while (dr.Read())
                        {
                            comments.Add(new Models.MenuComment
                            {
                                Sequence = GetInt(dr["Sequence"]),
                                Owner = GetInt(dr["Owner"]),
                                Description = GetStr(dr["Description"]),
                                PostedBy = GetStr(dr["PostedBy"]),
                                SubmitDate = GetStr(dr["SubmitDate"]),
                                DateLastModified = GetStr(dr["DateLastModified"]) 
                            });
                        }
                        menudata.Comment = comments;
                    }

                    dr.NextResult();
                    if (dr.HasRows)
                    {
                        var allergens = new List<Models.ListeAllergen>();
                        while (dr.Read())
                        {
                            allergens.Add(new Models.ListeAllergen
                            {
                                CodeAllergen = GetInt(dr["CodeAllergen"]),
                                AllergenName = GetStr(dr["Allergen"]),
                                Contain = GetBool(dr["Contain"]),
                                NonAllergen = GetBool(dr["NonAllergen"]),
                                Trace = GetBool(dr["Trace"]),
                                Derived = GetBool(dr["Derived"]),
                                Hidden = GetBool(dr["Hidden"]),
                                PictureName = GetStr(dr["PictureName"]),
                                Complete = GetStr(dr["Complete"]),
                                SwissLaw = GetStr(dr["SwLaw"]),
                                EULaw = GetStr(dr["EuLaw"]) 
                            });
                        }
                        menudata.Allergen = allergens;
                    }

                    dr.NextResult();
                    if (dr.HasRows)
                    {
                        var proceduretemplates = new List<Models.ProcedureTemplateInfo>();
                        while (dr.Read())
                        {
                            proceduretemplates.Add(new Models.ProcedureTemplateInfo
                            {
                                Code = GetInt(dr["Code"]),
                                Name = GetStr(dr["Name"]),
                                Global = GetBool(dr["Global"]) 
                            });
                        }
                        menudata.ProcedureTemplate = proceduretemplates;
                    }
                }
                return Ok(menudata);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        // Helpers (minimal parity)
        private static int GetInt(object value, int fallback = 0) { if (value == null || value == DBNull.Value) return fallback; if (int.TryParse(Convert.ToString(value), out var i)) return i; try { return Convert.ToInt32(value); } catch { return fallback; } }
        private static double GetDbl(object value, double fallback = 0) { if (value == null || value == DBNull.Value) return fallback; if (double.TryParse(Convert.ToString(value), out var d)) return d; try { return Convert.ToDouble(value); } catch { return fallback; } }
        private static string GetStr(object value) => value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value);
        private static bool GetBool(object value) { if (value == null || value == DBNull.Value) return false; if (value is bool b) return b; if (int.TryParse(Convert.ToString(value), out var i)) return i != 0; if (bool.TryParse(Convert.ToString(value), out var bb)) return bb; return false; }
        private static DateTime GetDate(object value) { if (value == null || value == DBNull.Value) return DateTime.MinValue; if (DateTime.TryParse(Convert.ToString(value), out var d)) return d; return DateTime.MinValue; }
        private static double SafePos(double v) => v < 0 ? 0 : v;
    }
}

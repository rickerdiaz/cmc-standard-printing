using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuPlanController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpGet("menuplaninfo/{codemenuplan:int}/{codetrans:int}/{copiedInfo:bool?}")]
        public ActionResult<DataTable> GetMenuPlanInfo(int codemenuplan, int codetrans, bool copiedInfo = false)
        {
            try
            {
                var dt = new DataTable();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "MP_GETMenuPlanInfo";
                cmd.Parameters.Add("@CodeMenuPlan", SqlDbType.Int).Value = codemenuplan;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                cmd.Parameters.Add("@Copy", SqlDbType.Bit).Value = copiedInfo;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return Ok(dt);
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

        [HttpGet("menuplan/restaurant/{codesite:int?}/{coderestaurant:int?}/{namerestaurant?}")]
        public ActionResult<DataTable> GetRestaurant(int codesite = -1, int coderestaurant = -1, string namerestaurant = "")
        {
            try
            {
                var dt = new DataTable();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "MP_GETRestaurant";
                cmd.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = coderestaurant;
                cmd.Parameters.Add("@nvcName", SqlDbType.NVarChar).Value = namerestaurant ?? string.Empty;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return Ok(dt);
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

        [HttpGet("menuplan/masterplan/{coderestaurant:int?}/{codetrans:int?}")]
        public ActionResult<DataTable> GetMasterPlan(int coderestaurant = -1, int codetrans = -1)
        {
            try
            {
                var dt = new DataTable();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "MP_GetMasterPlan";
                cmd.Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = coderestaurant;
                cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = codetrans;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return Ok(dt);
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

        [HttpPost("menuplan/delete")]
        public ActionResult<Models.ResponseCallBack> DeleteMenuPlan([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandText = "API_DELETE_MENUPLAN";
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                foreach (var del in data.Codes)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@CodeMenuPlan", SqlDbType.Int).Value = del;
                    cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
                    cmd.ExecuteNonQuery();
                }
                if (resultCode != 0) return StatusCode(500, Fail(response, resultCode, "Delete menu plan failed"));
                return Ok(Success(response));
            }
            catch (Exception)
            {
                if (resultCode == 0) resultCode = 500;
                return StatusCode(500, Fail(response, resultCode, "Delete menu plan failed"));
            }
        }

        [HttpPost("menuplan/copy")]
        public ActionResult<Models.ResponseCallBack> CopyMenuPlan([FromBody] Models.MenuPlan data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                var masterPlanList = string.Empty;
                foreach (var map in data.source)
                {
                    if (map.CodeMasterPlan > 0)
                    {
                        masterPlanList += $"{map.CodeMasterPlanSource}:{map.CodeMasterPlan},";
                    }
                }
                masterPlanList = masterPlanList.TrimEnd(',');

                cmd.Connection = cn;
                cmd.CommandText = "MP_COPYMenuPlan2";
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                var pCode = cmd.Parameters.Add("@CodeMenuPlan", SqlDbType.Int);
                pCode.Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@CodeMenuPlanSrc", SqlDbType.Int).Value = data.copiedFromMPCode;
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.codeUser;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.codeTrans;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = data.name ?? string.Empty;
                cmd.Parameters.Add("@Number", SqlDbType.NVarChar).Value = data.number ?? string.Empty;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = data.description ?? string.Empty;
                cmd.Parameters.Add("@CodeRestaurant", SqlDbType.Int).Value = data.copyRestaurant ? -1 : data.codeRestaurantTo;
                cmd.Parameters.Add("@CyclePlan", SqlDbType.Bit).Value = data.cyclePlan;
                cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = GetDate(data.startDate);
                cmd.Parameters.Add("@CodeCategory", SqlDbType.Int).Value = data.codeCategory;
                cmd.Parameters.Add("@CodeSeason", SqlDbType.Int).Value = data.codeSeason;
                cmd.Parameters.Add("@CodeService", SqlDbType.Int).Value = data.codeService;
                cmd.Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = data.codeSetPrice;
                cmd.Parameters.Add("@Duration", SqlDbType.Int).Value = data.duration;
                cmd.Parameters.Add("@Recurrence", SqlDbType.Int).Value = data.recurrence;
                cmd.Parameters.Add("@MasterPlanList", SqlDbType.NVarChar).Value = masterPlanList;
                cmd.ExecuteNonQuery();
                resultCode = GetInt(pCode.Value, -1);
                if (resultCode < 0) return StatusCode(500, Fail(response, resultCode, "Copy menu plan failed"));
                return Ok(Success(response, resultCode));
            }
            catch (Exception)
            {
                if (resultCode == 0) resultCode = 500; return StatusCode(500, Fail(response, resultCode, "Copy menu plan failed"));
            }
        }

        [HttpGet("menuplan/export/costmargin/{codemainmenu:int?}/{coderestaurant:int?}/{codetrans:int?}/{baseurl?}/{codeSetPrice:int?}/{culture?}/{CodeUser:int?}/{margin1:double?}/{margin2:double?}/{dayTotalCost:double?}")]
        public ActionResult<Models.ResponseCallBack> GetMSCMenuRecipeCostExport(int codemainmenu, int coderestaurant, int codetrans, string baseurl, int codeSetPrice, string culture, int CodeUser, double margin1, double margin2, double dayTotalCost)
        {
            var response = new Models.ResponseCallBack();
            try
            {
                var Report = new EGSTelerikReport.Coop();
                var FolderPath = System.Configuration.ConfigurationManager.AppSettings["ReportFolder"];
                var ReportURL = System.Configuration.ConfigurationManager.AppSettings["ReportURL"];
                var ds = new DataSet();
                var userConnectionString = GetUserConnectionString(CodeUser);

                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(userConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_Report_MenuPlan_Export_RecipeCostMargin]";
                cmd.CommandTimeout = 3600;
                cmd.Parameters.Add("@CodeMainMenu", SqlDbType.Int).Value = codemainmenu;
                cmd.Parameters.Add("@CodeRestaurant", SqlDbType.Int).Value = coderestaurant;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                cmd.Parameters.Add("@BaseURL", SqlDbType.VarChar).Value = baseurl ?? string.Empty;
                cmd.Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = codeSetPrice;
                cmd.Parameters.Add("@Culture", SqlDbType.VarChar).Value = culture ?? string.Empty;
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = CodeUser;
                cmd.Parameters.Add("@Margin1", SqlDbType.Float).Value = margin1;
                cmd.Parameters.Add("@Margin2", SqlDbType.Float).Value = margin2;
                cmd.Parameters.Add("@DayTotalCost", SqlDbType.Float).Value = dayTotalCost;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                var result = Report.GetExportMenuPlanRecipeCostMargin(ds, FolderPath, ReportURL);
                return Ok(Success(response, result));
            }
            catch (ArgumentException)
            {
                return BadRequest(Fail(new Models.ResponseCallBack(), 400, "Missing or invalid parameters"));
            }
            catch (Exception)
            {
                return StatusCode(500, Fail(new Models.ResponseCallBack(), 500, "Unexpected error occured"));
            }
        }

        // Helpers
        private static Models.ResponseCallBack Success(Models.ResponseCallBack r, object? returnValue = null) { r.Code = 0; r.Message = "OK"; r.Status = true; r.ReturnValue = returnValue; return r; }
        private static Models.ResponseCallBack Fail(Models.ResponseCallBack r, int code, string message) { r.Code = code; r.Message = message; r.Status = false; return r; }
        private static DateTime GetDate(string? s) { if (DateTime.TryParse(s, out var d)) return d; return DateTime.Now; }
        private static int GetInt(object? value, int fallback = 0) { if (value == null || value == DBNull.Value) return fallback; if (int.TryParse(Convert.ToString(value), out var i)) return i; try { return Convert.ToInt32(value); } catch { return fallback; } }

        // TODO: implement based on your environment
        private static string GetUserConnectionString(int codeUser) => System.Configuration.ConfigurationManager.ConnectionStrings["Default"]?.ConnectionString ?? string.Empty;
    }
}

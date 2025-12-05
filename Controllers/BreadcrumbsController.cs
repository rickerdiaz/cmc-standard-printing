using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CalcmenuAPI_CMC_CoopGastro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BreadcrumbsController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;

        [HttpPost]
        public ActionResult<Models.ResponseCallBack> UpdateBreadcrumbs([FromBody] Models.Breadcrumbs data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction? trans = null;
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_EgswBreadcrumbsUpdate";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.CodeUser;
                cmd.Parameters.Add("@intTypeItem", SqlDbType.Int).Value = data.ListeItemType;
                cmd.Parameters.Add("@intCodeItem", SqlDbType.NVarChar, 150).Value = data.CodeListe;
                cmd.Parameters.Add("@intTab", SqlDbType.NVarChar, 150).Value = data.Tab;
                cmd.Parameters.Add("@intSave", SqlDbType.NVarChar, 150).Value = data.Save;

                cn.Open();
                trans = cn.BeginTransaction();
                cmd.Transaction = trans;
                cmd.ExecuteNonQuery();

                response.Code = 0;
                response.Message = "OK";
                response.ReturnValue = 0;
                response.Status = true;
                trans.Commit();
            }
            catch (Exception)
            {
                try { trans?.Rollback(); trans?.Dispose(); } catch { }
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode;
                response.Status = false;
                response.Message = "Save breadcrumbs failed";
                return StatusCode(500, response);
            }
            return Ok(response);
        }
    }

    // Placeholder models - replace with actual
    namespace Models
    {
        public class Breadcrumbs { public int CodeUser { get; set; } public int ListeItemType { get; set; } public string CodeListe { get; set; } = string.Empty; public string Tab { get; set; } = string.Empty; public string Save { get; set; } = string.Empty; }
        public class ResponseCallBack { public int Code { get; set; } public string Message { get; set; } = string.Empty; public object? ReturnValue { get; set; } public bool Status { get; set; } public List<param>? Parameters { get; set; } }
        public class param { public string name { get; set; } = string.Empty; public string value { get; set; } = string.Empty; }
    }
}

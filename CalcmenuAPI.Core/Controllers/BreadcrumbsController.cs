using System.Data;
using System.Data.SqlClient;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static EgsData.modGlobalDeclarations; // ConnectionString, DebugEnabled

namespace CalcmenuAPI.Core.Controllers
{
    [ApiController]
    public class BreadcrumbsController : ControllerBase
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType);

        [HttpPost("api/breadcrumbs")]
        public ActionResult<Models.ResponseCallBack> UpdateBreadcrumbs([FromBody] Models.Breadcrumbs data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction? _trans = null;

            try
            {
                if (DebugEnabled)
                    Log.Info("Calling " + System.Reflection.MethodBase.GetCurrentMethod()!.Name + ":" + JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                try
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[sp_EgswBreadcrumbsUpdate]";

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = data.CodeUser;
                    cmd.Parameters.Add("@intTypeItem", SqlDbType.Int).Value = data.ListeItemType;
                    cmd.Parameters.Add("@intCodeItem", SqlDbType.NVarChar, 150).Value = data.CodeListe;
                    cmd.Parameters.Add("@intTab", SqlDbType.NVarChar, 150).Value = data.Tab;
                    cmd.Parameters.Add("@intSave", SqlDbType.NVarChar, 150).Value = data.Save;

                    cn.Open();
                    _trans = cn.BeginTransaction();
                    cmd.Transaction = _trans;
                    cmd.ExecuteNonQuery();

                    response.Code = 0;
                    response.Message = "OK";
                    response.ReturnValue = 0;
                    response.Status = true;
                    _trans.Commit();
                }
                catch (DatabaseException ex)
                {
                    Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Database error occured", ex);
                    try { _trans?.Rollback(); (_trans as IDisposable)?.Dispose(); } catch { }
                    if (resultCode == 0) resultCode = 500;
                    response.Code = resultCode;
                    response.Status = false;
                    response.Message = "Save breadcrumbs failed";
                }
                finally
                {
                    cn.Close();
                    (cn as IDisposable)?.Dispose();
                }
            }
            catch (ArgumentException aex)
            {
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Missing or invalid parameters", aex);
                response.Code = 400;
                response.Message = "Missing or invalid parameters";
                response.Parameters = new System.Collections.Generic.List<Models.param> { new Models.param { name = "data", value = "RecipeData" } };
            }
            catch (Exception ex)
            {
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod()!.Name + ": Unexpected error occured", ex);
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
            }

            return Ok(response);
        }
    }
}

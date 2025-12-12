using System;
using System.Data;
<<<<<<< HEAD
using Microsoft.Data.SqlClient;
=======
using System.Data.SqlClient;
>>>>>>> main
using CmcStandardPrinting.Domain.Breadcrumbs;
using CmcStandardPrinting.Domain.Printers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BreadcrumbsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<BreadcrumbsController> _logger;

    public BreadcrumbsController(IConfiguration configuration, ILogger<BreadcrumbsController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpPost]
    public ActionResult<ResponseCallBack> UpdateBreadcrumbs([FromBody] BreadcrumbsData data)
    {
        var response = new ResponseCallBack();
        int resultCode = 0;
        SqlTransaction? trans = null;

        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[sp_EgswBreadcrumbsUpdate]";
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
        catch (Exception ex)
        {
            try
            {
                trans?.Rollback();
                trans?.Dispose();
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
            response.Message = "Save breadcrumbs failed";
            _logger.LogError(ex, "UpdateBreadcrumbs: database error");
            return StatusCode(500, response);
        }

        return Ok(response);
    }
}

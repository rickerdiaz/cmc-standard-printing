using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using CmcStandardPrinting.Domain.Common;
using CmcStandardPrinting.Domain.Printers;
using CmcStandardPrinting.Domain.Workflows;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CmcStandardPrinting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkflowController : ControllerBase
{
    private WorkflowAttachment? _attachments;
    private string _tempAttachments = string.Empty;
    private int _intCodeListe;
    private int _codeWorkflow;
    private string _recipeName = string.Empty;

    private readonly IConfiguration _configuration;
    private readonly ILogger<WorkflowController> _logger;

    public WorkflowController(IConfiguration configuration, ILogger<WorkflowController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string ConnectionString => _configuration.GetConnectionString("Default") ?? string.Empty;

    [HttpPost("search")]
    public ActionResult<List<Workflow>> GetWorkflowByName([FromBody] ConfigurationcSearch data)
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
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = 1;
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 200).Value = "EgswWorkflow";
            cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = data.CodeProperty;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var list = new List<Workflow>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Workflow
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Archive = GetBool(r["Archive"])
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(data.Name))
            {
                var result = new List<Workflow>();
                foreach (var w in data.Name.Split(','))
                {
                    var word = (w ?? string.Empty).Trim();
                    if (word.Length == 0) continue;
                    var key = Common.ReplaceSpecialCharacters(word.ToLowerInvariant());
                    foreach (var s in list)
                    {
                        if (!string.IsNullOrEmpty(s.Name) && s.Name.ToLowerInvariant().Contains(key)) result.Add(s);
                    }
                }
                list = result;
            }

            return Ok(list);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "GetWorkflowByName failed");
            return Problem(title: "Request failed", statusCode: 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetWorkflowByName failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost]
    public ActionResult<ResponseCallBack> SaveWorkflow([FromBody] WorkflowData data)
    {
        var response = new ResponseCallBack();
        int resultCode = 0;
        SqlTransaction? trans = null;
        try
        {
            var codeWorkflowList = string.Join(",", (data.CodeList ?? new List<GenericList>()).Select(c => c.Code).Distinct());

            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "MANAGE_WORKFLOWUPDATE";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = data.Info.Code;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = data.Info.Name ?? string.Empty;
            cmd.Parameters.Add("@Taskname", SqlDbType.NVarChar).Value = data.Info.TaskName ?? string.Empty;
            cmd.Parameters.Add("@Codeuser", SqlDbType.Int).Value = data.Info.User;
            cmd.Parameters.Add("@isArchive", SqlDbType.Bit).Value = data.Info.Archive;
            var retval = cmd.Parameters.Add("@retval", SqlDbType.Int);
            retval.Direction = ParameterDirection.ReturnValue;
            cn.Open();
            trans = cn.BeginTransaction();
            cmd.Transaction = trans;
            cmd.CommandTimeout = 120;
            cmd.ExecuteNonQuery();
            var codeWorkflow = GetInt(cmd.Parameters["@Code"].Value, -1);
            resultCode = GetInt(retval.Value, -1);
            if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Save Workflow failed");
            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = codeWorkflow;
            response.Status = true;
            trans.Commit();

            using var cmd2 = new SqlCommand();
            using var cn2 = new SqlConnection(ConnectionString);
            cmd2.Connection = cn2;
            cmd2.CommandText = "MANAGE_WorkflowSequence";
            cmd2.CommandType = CommandType.StoredProcedure;
            cmd2.Parameters.Clear();
            cmd2.Parameters.Add("@CodeTask", SqlDbType.VarChar, 4000).Value = codeWorkflowList;
            cmd2.Parameters.Add("@Code", SqlDbType.Int).Value = data.Info.Code;
            var ret2 = cmd2.Parameters.Add("@Return", SqlDbType.Int);
            ret2.Direction = ParameterDirection.ReturnValue;
            cn2.Open();
            cmd2.ExecuteNonQuery();
            resultCode = GetInt(ret2.Value, -1);
            if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Sequence failed");
            return Ok(response);
        }
        catch (DatabaseException ex)
        {
            try { trans?.Rollback(); trans?.Dispose(); } catch { }
            _logger.LogError(ex, "SaveWorkflow failed");
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.Message = "Save Workflow failed";
            return StatusCode(500, response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "SaveWorkflow failed");
            response.Status = false;
            response.Message = "Request failed";
            response.Code = 400;
            return StatusCode(400, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SaveWorkflow failed");
            response.Status = false;
            response.Message = "Unexpected error occured";
            response.Code = 500;
            return StatusCode(500, response);
        }
    }

    [HttpGet("getrecipeworkflow/{code:int?}")]
    public ActionResult<List<Workflow>> GetWorkflow(int code = 0)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Workflow]";
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = code;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            var list = new List<Workflow>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Workflow
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Archive = GetBool(r["Archive"])
                    });
                }
            }
            return Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetWorkflow failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost("delete")]
    public ActionResult<ResponseCallBack> DeleteWorkflow([FromBody] GenericDeleteData data)
    {
        var response = new ResponseCallBack();
        int resultCode = 0;
        try
        {
            var list = (data.CodeList ?? new List<GenericList>()).Select(c => c.Code).Distinct().ToList();
            var codeList = string.Join(",", list);
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandText = "API_DELETE_Generic";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeList;
            cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWWORKFLOW";
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
            if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Delete workflow failed");
            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = GetStr(skip.Value);
            response.Status = true;
            return Ok(response);
        }
        catch (DatabaseException ex)
        {
            _logger.LogError(ex, "DeleteWorkflow failed");
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.ReturnValue = string.Empty;
            response.Message = "Delete workflow failed";
            return StatusCode(500, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteWorkflow failed");
            response.Status = false;
            response.Message = "Unexpected error occured";
            response.Code = 500;
            return StatusCode(500, response);
        }
    }

    [HttpGet("getworkflowuser/{codesite:int}")]
    public ActionResult<List<GenericList>> GetUserWorkflow(int codesite)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_Users]";
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            var list = new List<GenericList>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new GenericList { Code = GetInt(r["Code"]), Name = GetStr(r["Name"]) });
                }
            }
            return Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUserWorkflow failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost("saveworkflowtask")]
    public ActionResult<ResponseCallBack> SaveWorkflowTask([FromBody] WorkflowData data)
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
            cmd.CommandText = "MANAGE_WORKFLOWTASKUPDATE";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = data.Info.Code;
            cmd.Parameters.Add("@CodeTaskWorkflow", SqlDbType.Int).Value = data.Info.CodeTaskWorkflow;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = data.Info.TaskName ?? string.Empty;
            cmd.Parameters.Add("@WorkFlowName", SqlDbType.NVarChar).Value = data.Info.WorkflowName ?? string.Empty;
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.Info.User;
            cmd.Parameters.Add("@Duration", SqlDbType.Decimal).Value = data.Info.Duration;
            cmd.Parameters.Add("@CodeTask", SqlDbType.Int).Value = data.Info.CodeTask;
            cmd.Parameters.Add("@isArchive", SqlDbType.Bit).Value = data.Info.Archive;
            var retval = cmd.Parameters.Add("@retval", SqlDbType.Int);
            retval.Direction = ParameterDirection.ReturnValue;
            cn.Open();
            trans = cn.BeginTransaction();
            cmd.Transaction = trans;
            cmd.CommandTimeout = 120;
            cmd.ExecuteNonQuery();
            var codeTask = GetInt(cmd.Parameters["@CodeTask"].Value, -1);
            resultCode = GetInt(retval.Value, -1);
            response.Code = codeTask;
            response.Message = "OK";
            response.ReturnValue = resultCode;
            response.Status = true;
            trans.Commit();
            return Ok(response);
        }
        catch (DatabaseException ex)
        {
            try { trans?.Rollback(); trans?.Dispose(); } catch { }
            _logger.LogError(ex, "SaveWorkflowTask failed");
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.Message = "Save Workflow failed";
            return StatusCode(500, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SaveWorkflowTask failed");
            response.Status = false;
            response.Message = "Unexpected error occured";
            response.Code = 500;
            return StatusCode(500, response);
        }
    }

    [HttpGet("workflowtask/search/{code:int}")]
    public ActionResult<List<Workflow>> GetWorkflowTaskByName(int code)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_MANAGE_Workflow]";
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = code;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            var list = new List<Workflow>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Workflow
                    {
                        Code = GetInt(r["CodeWorkflow"]),
                        CodeTaskWorkflow = GetInt(r["CodeTask"]),
                        TaskName = GetStr(r["Name"]),
                        User = GetStr(r["Fullname"]),
                        Duration = GetDbl(r["Duration"]).ToString(CultureInfo.InvariantCulture)
                    });
                }
            }
            return Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetWorkflowTaskByName failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("getrecipeworkflowtask/{code:int}/{codetask:int}")]
    public ActionResult<List<Workflow>> GetWorkflowTask(int code, int codetask)
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_GET_WorkflowTask]";
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = code;
            cmd.Parameters.Add("@CodeTask", SqlDbType.Int).Value = codetask;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            var list = new List<Workflow>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Workflow
                    {
                        Code = GetInt(r["CodeWorkflow"]),
                        Name = GetStr(r["Name"]),
                        Duration = GetStr(r["Duration"]),
                        TaskName = GetStr(r["TaskName"]),
                        CodeTaskWorkflow = GetInt(r["CodeTask"]),
                        User = GetInt(r["CodeUser"]).ToString(CultureInfo.InvariantCulture)
                    });
                }
            }
            return Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetWorkflowTask failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost("workflowtask/delete")]
    public ActionResult<ResponseCallBack> DeleteWorkflowTask([FromBody] GenericDeleteData data)
    {
        var response = new ResponseCallBack();
        int resultCode = 0;
        try
        {
            var codes = (data.CodeList ?? new List<GenericList>()).Select(c => c.CodeDictionary).Distinct().ToList();
            var codeList = string.Join(",", codes);
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandText = "API_DELETE_Generic";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeList;
            cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWWORKFLOWTASK";
            cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.Type;
            cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
            cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = data.ForceDelete;
            var skip = cmd.Parameters.Add("@SkipList", SqlDbType.NVarChar, 4000);
            skip.Direction = ParameterDirection.Output;
            var ret = cmd.Parameters.Add("@Return", SqlDbType.Int);
            ret.Direction = ParameterDirection.ReturnValue;
            cn.Open();
            cmd.ExecuteNonQuery();
            resultCode = GetInt(ret.Value, -1);
            if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Delete workflow task failed");
            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = GetStr(skip.Value);
            response.Status = true;
            return Ok(response);
        }
        catch (DatabaseException ex)
        {
            _logger.LogError(ex, "DeleteWorkflowTask failed");
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.ReturnValue = string.Empty;
            response.Message = "Delete workflow failed";
            return StatusCode(500, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteWorkflowTask failed");
            response.Status = false;
            response.Message = "Unexpected error occured";
            response.Code = 500;
            return StatusCode(500, response);
        }
    }

    [HttpPost("saveworkflowtasksequence")]
    public ActionResult<ResponseCallBack> SaveSequenceWorkflow([FromBody] WorkflowData data)
    {
        var response = new ResponseCallBack();
        int resultCode = 0;
        try
        {
            var codes = (data.CodeList ?? new List<GenericList>()).Select(d => d.Code).Distinct().ToList();
            var codeList = string.Join(",", codes);
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandText = "MANAGE_WorkflowSequence";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@CodeTask", SqlDbType.VarChar, 4000).Value = codeList;
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = data.Info.Code;
            var ret = cmd.Parameters.Add("@Return", SqlDbType.Int);
            ret.Direction = ParameterDirection.ReturnValue;
            cn.Open();
            cmd.ExecuteNonQuery();
            resultCode = GetInt(ret.Value, -1);
            if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Sequence failed");
            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = string.Empty;
            response.Status = true;
            return Ok(response);
        }
        catch (DatabaseException ex)
        {
            _logger.LogError(ex, "SaveSequenceWorkflow failed");
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.ReturnValue = string.Empty;
            response.Message = "Workflow sequence failed";
            return StatusCode(500, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SaveSequenceWorkflow failed");
            response.Status = false;
            response.Message = "Unexpected error occured";
            response.Code = 500;
            return StatusCode(500, response);
        }
    }

    [HttpGet("checkIsArchive/{code:int}")]
    public ActionResult<ResponseCallBack> WorkflowIsArchive(int code)
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
            cmd.CommandText = "API_GET_CHECKWORKFLOWSTATUS";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@Code", SqlDbType.Int).Value = code;
            var retval = cmd.Parameters.Add("@retval", SqlDbType.Int);
            retval.Direction = ParameterDirection.ReturnValue;
            cn.Open();
            trans = cn.BeginTransaction();
            cmd.Transaction = trans;
            cmd.CommandTimeout = 120;
            cmd.ExecuteNonQuery();
            resultCode = GetInt(retval.Value, -1);
            if (resultCode == 0)
            {
                response.Code = 0;
                response.Message = "OK";
                response.Status = true;
            }
            trans.Commit();
            return Ok(response);
        }
        catch (DatabaseException ex)
        {
            try { trans?.Rollback(); trans?.Dispose(); } catch { }
            _logger.LogError(ex, "WorkflowIsArchive failed");
            response.Status = false;
            response.Message = "Workflow validation failed";
            response.Code = 500;
            return StatusCode(500, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WorkflowIsArchive failed");
            response.Status = false;
            response.Message = "Workflow validation failed";
            response.Code = 500;
            return StatusCode(500, response);
        }
    }

    [HttpPost("saveworkflow")]
    public ActionResult<ResponseCallBack> SaveWorkflowListe([FromBody] WorkflowDataRecipe data)
    {
        var response = new ResponseCallBack();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            if ((data.RecipeWorkflowData?.Count ?? 0) > 0)
            {
                cmd.CommandTimeout = 120;
                cmd.Connection = cn;
                cn.Open();
                foreach (var wf in data.RecipeWorkflowData)
                {
                    cmd.CommandText = "API_UPDATE_ListeWorkflow";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = wf.CodeListe;
                    cmd.Parameters.Add("@RecipeName", SqlDbType.NVarChar, 255).Value = wf.RecipeName ?? string.Empty;
                    cmd.Parameters.Add("@CodeWorkflowTask", SqlDbType.Int).Value = wf.CodeWorkflowTask;
                    cmd.Parameters.Add("@Attachment", SqlDbType.NVarChar, 255).Value = wf.Attachment ?? string.Empty;
                    cmd.Parameters.Add("@TaskStatus", SqlDbType.NVarChar, 255).Value = wf.TaskStatus ?? string.Empty;
                    cmd.Parameters.Add("@DateTime", SqlDbType.NVarChar, 50).Value = wf.DateTime ?? string.Empty;
                    cmd.Parameters.Add("@UpdateDate", SqlDbType.NVarChar, 50).Value = wf.UpdateDate ?? string.Empty;
                    cmd.Parameters.Add("@IsTemp", SqlDbType.Bit).Value = wf.IsTemp;
                    cmd.ExecuteNonQuery();
                }

                foreach (var wf in data.RecipeWorkflowData)
                {
                    if (wf.IsTemp && data.WorkflowAttachment != null)
                    {
                        cmd.CommandText = "IF NOT EXISTS(SELECT CodeWorkflowTask from EgswWorkflowFiles where CodeWorkflowTask=@CodeWorkflowTask and CodeListe = @CodeListe)\nBEGIN\nINSERT INTO EgswWorkflowFiles(CodeWorkflowTask, CodeListe, Flag, [Filename], Filecaption, [Default])\nVALUES (@CodeWorkflowTask, @CodeListe, @Flag, @FileName, @Filecaption, @Default)\nEND";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CodeWorkflowTask", SqlDbType.Int).Value = wf.CodeWorkflowTask;
                        cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = wf.CodeListe;
                        cmd.Parameters.Add("@Flag", SqlDbType.Int).Value = data.WorkflowAttachment.Type;
                        cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = data.WorkflowAttachment.Resource ?? string.Empty;
                        cmd.Parameters.Add("@Filecaption", SqlDbType.NVarChar).Value = data.WorkflowAttachment.Name ?? string.Empty;
                        cmd.Parameters.Add("@Default", SqlDbType.Bit).Value = data.WorkflowAttachment.IsDefault;
                        cmd.ExecuteNonQuery();
                    }
                }

                var ctr = 0;
                foreach (var wf in data.RecipeWorkflowData)
                {
                    ctr++;
                    if (!string.IsNullOrEmpty(data.CustomTempAttachments))
                    {
                        _attachments = data.WorkflowAttachment;
                        _tempAttachments = data.CustomTempAttachments;
                        _intCodeListe = wf.CodeListe;
                        _codeWorkflow = wf.CodeWorkflowTask;
                        _recipeName = wf.RecipeName;
                        SaveAttachments(ctr, data.RecipeWorkflowData.Count);
                    }
                }
            }
            response.Code = 0;
            response.Message = "Workflow successfully saved.";
            response.Status = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SaveWorkflowListe failed");
            response.Status = false;
            response.Message = "Save workflow failed";
            return StatusCode(500, response);
        }
    }

    [HttpPost("listeworkflowupdate")]
    public ActionResult<ResponseCallBack> UpdateListeWorkflow([FromBody] WorkflowDataRecipe data)
    {
        var response = new ResponseCallBack();
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            if ((data.RecipeWorkflowData?.Count ?? 0) > 0)
            {
                cmd.CommandTimeout = 120;
                cmd.Connection = cn;
                cn.Open();
                foreach (var wf in data.RecipeWorkflowData)
                {
                    if (string.Equals(wf.TaskStatus, "Started", StringComparison.OrdinalIgnoreCase))
                    {
                        cmd.CommandText = "UPDATE EgswListeWorkflow SET TaskStatus = @TaskStatus, UpdateDate = @UpdateDate WHERE ID = @ID";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@TaskStatus", SqlDbType.NVarChar, 255).Value = wf.TaskStatus ?? string.Empty;
                        cmd.Parameters.Add("@UpdateDate", SqlDbType.NVarChar, 255).Value = wf.UpdateDate ?? string.Empty;
                        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = wf.ID;
                    }
                    else
                    {
                        cmd.CommandText = "UPDATE EgswListeWorkflow SET TaskStatus = @TaskStatus WHERE ID = @ID";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@TaskStatus", SqlDbType.NVarChar, 255).Value = wf.TaskStatus ?? string.Empty;
                        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = wf.ID;
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            response.Code = 0;
            response.Message = "Workflow successfully saved.";
            response.Status = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateListeWorkflow failed");
            response.Status = false;
            response.Message = "Save workflow failed";
            return StatusCode(500, response);
        }
    }

    [HttpPost("workflowtemp/delete")]
    public ActionResult<ResponseCallBack> DeleteWorkflowTemp()
    {
        var response = new ResponseCallBack();
        int resultCode = 0;
        try
        {
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandText = "TRUNCATE TABLE EgswListeWorkflow_Temp";
            cmd.CommandType = CommandType.Text;
            cn.Open();
            cmd.ExecuteNonQuery();
            response.Code = 0;
            response.Message = "Recipe Workflow Temp Deleted";
            response.ReturnValue = "OK";
            response.Status = true;
            return Ok(response);
        }
        catch (DatabaseException ex)
        {
            _logger.LogError(ex, "DeleteWorkflowTemp failed");
            if (resultCode == 0) resultCode = 500;
            response.Code = -1;
            response.Status = false;
            response.ReturnValue = "Not OK";
            response.Message = "Delete workflow failed";
            return StatusCode(500, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteWorkflowTemp failed");
            response.Status = false;
            response.Message = "Unexpected error occured";
            response.Code = 500;
            return StatusCode(500, response);
        }
    }

    [HttpPost("workflowliste/delete")]
    public ActionResult<ResponseCallBack> DeleteWorkflowListe([FromBody] GenericDeleteData data)
    {
        var response = new ResponseCallBack();
        int resultCode = 0;
        try
        {
            var codes = (data.CodeList ?? new List<GenericList>()).Select(c => c.Code).Distinct().ToList();
            var codeList = string.Join(",", codes);
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandText = "API_DELETE_Generic";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeList;
            cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWWORKFLOWLISTE";
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
            if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Delete recipe workflow liste failed");
            response.Code = 0;
            response.Message = "OK";
            response.ReturnValue = GetStr(skip.Value);
            response.Status = true;
            return Ok(response);
        }
        catch (DatabaseException ex)
        {
            _logger.LogError(ex, "DeleteWorkflowListe failed");
            if (resultCode == 0) resultCode = 500;
            response.Code = resultCode;
            response.Status = false;
            response.ReturnValue = string.Empty;
            response.Message = "Delete recipe worlkflow liste failed";
            return StatusCode(500, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteWorkflowListe failed");
            response.Status = false;
            response.Message = "Unexpected error occured";
            response.Code = 500;
            return StatusCode(500, response);
        }
    }

    [HttpGet("/api/validaterecipename/{recipename?}")]
    public ActionResult<List<WorkflowRecipe>> ValidateRecipeName(string recipename = "")
    {
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandText = "SELECT Code, Name FROM EgswListe WHERE Name=@Name AND Type=8";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = recipename ?? string.Empty;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            var list = new List<WorkflowRecipe>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow rc in ds.Tables[0].Rows)
                {
                    list.Add(new WorkflowRecipe { CodeListe = GetInt(rc["Code"]), Name = GetStr(rc["Name"]) });
                }
            }
            return Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ValidateRecipeName failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpGet("/api/validaterecipeworkflow/{codeliste:int?}/{coderecipeworkflow:int?}")]
    public ActionResult<ResponseCallBack> ValidateRecipeWorkflow(int codeliste = 0, int coderecipeworkflow = 0)
    {
        var response = new ResponseCallBack();
        try
        {
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandText = "SELECT LWF.CodeListe, WFT.CodeWorkflow FROM EgswListeWorkflow LWF INNER JOIN EgswWorkflowTask WFT ON WFT.ID = LWF.CodeWorkflowTask WHERE LWF.CodeListe = @CodeListe and WFT.CodeWorkflow = @CodeWorkflow";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@CodeListe", SqlDbType.VarChar).Value = codeliste;
            cmd.Parameters.Add("@CodeWorkflow", SqlDbType.VarChar).Value = coderecipeworkflow;
            cmd.CommandType = CommandType.Text;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                response.Code = 1;
                response.Message = "OK";
                response.ReturnValue = "Already exists.";
                response.Status = true;
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ValidateRecipeWorkflow failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    [HttpPost("/api/workflow/worklfowlisteallowdelete")]
    public ActionResult<List<RecipeWorkflowList>> ValidateWorflowlisteAllowdelete([FromBody] GenericDeleteData data)
    {
        try
        {
            var codes = (data.CodeList ?? new List<GenericList>()).Select(c => c.Code).Distinct().ToList();
            var codeList = string.Join(",", codes);
            var ds = new DataSet();
            using var cmd = new SqlCommand();
            using var cn = new SqlConnection(ConnectionString);
            cmd.Connection = cn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[API_AllowDeleteWorkflowListe]";
            cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeList;
            cn.Open();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            var list = new List<RecipeWorkflowList>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new RecipeWorkflowList
                    {
                        ID = GetInt(r["ID"]),
                        Task = GetStr(r["Task"]),
                        CodeWorkflowTask = GetInt(r["CodeWorkflowTask"]),
                        TaskStatus = GetStr(r["TaskStatus"])
                    });
                }
            }
            return Ok(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ValidateWorflowlisteAllowdelete failed");
            return Problem(title: "Request failed", statusCode: 500);
        }
    }

    private void SaveAttachments(int ctr, int count)
    {
        try
        {
            var filesFolder = FilesFolder;
            var tempFolder = TempFolder2;
            var filesFolderForListe = (filesFolder.EndsWith("\\") ? filesFolder : filesFolder + "\\") + (_recipeName ?? string.Empty) + "\\";
            var filesFolderForListeWorkflow = filesFolderForListe + _codeWorkflow.ToString(CultureInfo.InvariantCulture) + "\\";
            tempFolder = tempFolder.EndsWith("\\") ? tempFolder : tempFolder + "\\";

            if (_attachments != null)
            {
                if (_attachments.Type == 0 && !string.IsNullOrWhiteSpace(_attachments.Resource))
                {
                    var source = Path.Combine(tempFolder, _attachments.Resource);
                    if (System.IO.File.Exists(source))
                    {
                        Directory.CreateDirectory(filesFolderForListe);
                        Directory.CreateDirectory(filesFolderForListeWorkflow);
                        var dest = Path.Combine(filesFolderForListeWorkflow, _attachments.Resource);
                        if (!System.IO.File.Exists(dest)) System.IO.File.Copy(source, dest, true);
                    }
                }
            }

            if (ctr == count)
            {
                if (!string.IsNullOrEmpty(_tempAttachments))
                {
                    var arr = _tempAttachments.Trim().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var a in arr)
                    {
                        var source = Path.Combine(tempFolder, a);
                        if (System.IO.File.Exists(source)) System.IO.File.Delete(source);
                        var delete = true;
                        if (_attachments != null && _attachments.Type == 0 && string.Equals((_attachments.Resource ?? string.Empty).Trim(), a.Trim(), StringComparison.OrdinalIgnoreCase))
                            delete = false;
                        if (delete)
                        {
                            var pic = Path.Combine(filesFolderForListe, a);
                            if (System.IO.File.Exists(pic)) System.IO.File.Delete(pic);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SaveAttachments failed");
        }
    }

    private string FilesFolder
    {
        get
        {
            var tmp = GetStr(_configuration["files"]).Trim();
            if (string.IsNullOrEmpty(tmp)) tmp = Common.MapPath("files");
            if (string.IsNullOrEmpty(tmp)) tmp = Directory.GetCurrentDirectory();
            return tmp.TrimEnd('\\') + "\\";
        }
    }

    private string TempFolder2
    {
        get
        {
            var tmp = GetStr(_configuration["temp"]).Trim();
            if (string.IsNullOrEmpty(tmp)) tmp = Common.MapPath("temp");
            if (string.IsNullOrEmpty(tmp)) tmp = Path.GetTempPath();
            return tmp.TrimEnd('\\') + "\\";
        }
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
}

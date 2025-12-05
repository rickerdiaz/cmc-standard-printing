using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI
{
    [ApiController]
    public class WorkflowController : ControllerBase
    {
        // state used by SaveAttachments (kept to mirror VB behavior)
        private Models.WorkflowAttachment _attachments;
        private string _tempAttachments;
        private int _intCodeListe;
        private int _codeWorkflow;
        private string _recipeName;

        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;
        private IConfiguration Config => (IConfiguration)HttpContext.RequestServices.GetService(typeof(IConfiguration));

        // Display & Search of Recipe Workflow
        [HttpPost("/api/workflow/search")]
        public ActionResult<List<Models.Workflow>> GetWorkflowByName([FromBody] Models.ConfigurationcSearch data)
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

                var list = new List<Models.Workflow>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Models.Workflow
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Archive = GetBool(r["Archive"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(data.Name))
                {
                    var result = new List<Models.Workflow>();
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
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        // Insert & Update Recipe Workflow
        [HttpPost("api/workflow")]
        public ActionResult<Models.ResponseCallBack> SaveWorkflow([FromBody] Models.WorkflowData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction trans = null;
            try
            {
                var codeWorkflowList = string.Join(",", (data.CodeList ?? new List<Models.GenericList>()).Select(c => c.Code).Distinct());

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

                // sequence
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
            catch (DatabaseException)
            {
                try { trans?.Rollback(); trans?.Dispose(); } catch { }
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode;
                response.Status = false;
                response.Message = "Save Workflow failed";
                return StatusCode(500, response);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                return StatusCode(500, response);
            }
        }

        // Display workflow item
        [HttpGet("/api/workflow/getrecipeworkflow/{code:int?}")]
        public ActionResult<List<Models.Workflow>> GetWorkflow(int code = 0)
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
                var list = new List<Models.Workflow>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Models.Workflow
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Archive = GetBool(r["Archive"]) 
                    });
                }
                return Ok(list);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        // Delete Recipe Workflow
        [HttpPost("api/workflow/delete")]
        public ActionResult<Models.ResponseCallBack> DeleteWorkflow([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                var list = (data.CodeList ?? new List<Models.GenericList>()).Select(c => c.Code).Distinct().ToList();
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
                if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Delete sales site failed");
                response.Code = 0;
                response.Message = "OK";
                response.ReturnValue = GetStr(skip.Value);
                response.Status = true;
                return Ok(response);
            }
            catch (DatabaseException)
            {
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode;
                response.Status = false;
                response.ReturnValue = string.Empty;
                response.Message = "Delete workflow failed";
                return StatusCode(500, response);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                return StatusCode(500, response);
            }
        }

        // Populate the combobox
        [HttpGet("/api/workflow/getworkflowuser/{codesite:int}")]
        public ActionResult<List<Models.GenericList>> GetUserWorkflow(int codesite)
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
                var list = new List<Models.GenericList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Models.GenericList { Code = GetInt(r["Code"]), Name = GetStr(r["Name"]) });
                }
                return Ok(list);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        // Insert & Update Recipe Task Workflow
        [HttpPost("api/workflow/saveworkflowtask")]
        public ActionResult<Models.ResponseCallBack> SaveWorkflowTask([FromBody] Models.WorkflowData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction trans = null;
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
            catch (DatabaseException)
            {
                try { trans?.Rollback(); trans?.Dispose(); } catch { }
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode;
                response.Status = false;
                response.Message = "Save Workflow failed";
                return StatusCode(500, response);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                return StatusCode(500, response);
            }
        }

        // Display the Workflow Task
        [HttpGet("/api/workflowtask/search/{code:int}")]
        public ActionResult<List<Models.Workflow>> GetWorkflowTaskByName(int code)
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
                var list = new List<Models.Workflow>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Models.Workflow
                    {
                        Code = GetInt(r["CodeWorkflow"]),
                        CodeTaskWorkflow = GetInt(r["CodeTask"]),
                        TaskName = GetStr(r["Name"]),
                        User = GetStr(r["Fullname"]),
                        Duration = GetDbl(r["Duration"]) 
                    });
                }
                return Ok(list);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        // Display Workflow Task Information item
        [HttpGet("/api/workflow/getrecipeworkflowtask/{code:int}/{codetask:int}")]
        public ActionResult<List<Models.Workflow>> GetWorkflowTask(int code, int codetask)
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
                var list = new List<Models.Workflow>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Models.Workflow
                    {
                        Code = GetInt(r["CodeWorkflow"]),
                        Name = GetStr(r["Name"]),
                        Duration = GetStr(r["Duration"]),
                        TaskName = GetStr(r["TaskName"]),
                        CodeTaskWorkflow = GetInt(r["CodeTask"]),
                        User = GetInt(r["CodeUser"]) 
                    });
                }
                return Ok(list);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        // Delete Recipe Workflow Task Information
        [HttpPost("api/workflowtask/delete")]
        public ActionResult<Models.ResponseCallBack> DeleteWorkflowTask([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                var codes = (data.CodeList ?? new List<Models.GenericList>()).Select(c => c.CodeDictionary).Distinct().ToList();
                var codeList = string.Join(",", codes);
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandText = "API_DELETE_Generic";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeList;
                cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "EGSWWORKFLOWTASK";
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.Type; // utilize for task code
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = data.ForceDelete;
                var skip = cmd.Parameters.Add("@SkipList", SqlDbType.NVarChar, 4000);
                skip.Direction = ParameterDirection.Output;
                var ret = cmd.Parameters.Add("@Return", SqlDbType.Int);
                ret.Direction = ParameterDirection.ReturnValue;
                cn.Open();
                cmd.ExecuteNonQuery();
                resultCode = GetInt(ret.Value, -1);
                if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Delete sales site failed");
                response.Code = 0;
                response.Message = "OK";
                response.ReturnValue = GetStr(skip.Value);
                response.Status = true;
                return Ok(response);
            }
            catch (DatabaseException)
            {
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode;
                response.Status = false;
                response.ReturnValue = string.Empty;
                response.Message = "Delete workflow failed";
                return StatusCode(500, response);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                return StatusCode(500, response);
            }
        }

        // Update Workflow Sequence
        [HttpPost("api/workflowtask/saveworkflowtasksequence")]
        public ActionResult<Models.ResponseCallBack> SaveSequenceWorkflow([FromBody] Models.WorkflowData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                var codes = (data.CodeList ?? new List<Models.GenericList>()).Select(d => d.Code).Distinct().ToList();
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
            catch (DatabaseException)
            {
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode;
                response.Status = false;
                response.ReturnValue = string.Empty;
                response.Message = "Workflow sequence failed";
                return StatusCode(500, response);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                return StatusCode(500, response);
            }
        }

        // Check Archive Workflow
        [HttpGet("/api/workflow/checkIsArchive/{code:int}")]
        public ActionResult<Models.ResponseCallBack> WorkflowIsArchive(int code)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            SqlTransaction trans = null;
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
                response.Code = 0;
                response.Message = "OK";
                response.ReturnValue = resultCode;
                response.Status = true;
                trans.Commit();
                return Ok(response);
            }
            catch (DatabaseException)
            {
                try { trans?.Rollback(); trans?.Dispose(); } catch { }
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode;
                response.Status = false;
                response.Message = "Database error occured.";
                return StatusCode(500, response);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        // Check Workflow Status
        [HttpPost("/api/workflow/workflowallowdelete")]
        public ActionResult<List<Models.Workflow>> ValidateWorkflowAllowDelete([FromBody] Models.GenericDeleteData data)
        {
            try
            {
                var codes = (data.CodeList ?? new List<Models.GenericList>()).Select(c => c.Code).Distinct().ToList();
                var codeList = string.Join(",", codes);
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_WorkflowAllowDelete]";
                cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeList;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                var list = new List<Models.Workflow>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Models.Workflow
                    {
                        Code = GetInt(r["WorkflowCode"]),
                        CodeTaskWorkflow = GetInt(r["TaskCode"]),
                        TaskName = GetStr(r["Task"]),
                        User = GetStr(r["AssignedUser"]),
                        Duration = GetDbl(r["Duration"]),
                        WorkflowName = GetStr(r["CodeWorkflowName"]),
                        Name = GetStr(r["CodeWorkflowName"]),
                        CodeTask = GetInt(r["TaskCode"]) 
                    });
                }
                return Ok(list);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        // ECAM APIs - Display List of Recipe WorkFlow
        [HttpGet("/api/recipeworkflowlist/{codeuser:int?}/{istemp:int?}/{taskstatus?}")]
        public ActionResult<List<Models.RecipeWorkflowList>> GetRecipeWorkflowList(int codeuser, int istemp, string taskstatus = "")
        {
            if (string.Equals(taskstatus, "All", StringComparison.OrdinalIgnoreCase)) taskstatus = string.Empty;
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_WorkflowList]";
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = codeuser;
                cmd.Parameters.Add("@TaskStatus", SqlDbType.VarChar).Value = taskstatus ?? string.Empty;
                cmd.Parameters.Add("@IsTemp", SqlDbType.Int).Value = istemp;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                var list = new List<Models.RecipeWorkflowList>();
                foreach (DataRow wf in ds.Tables[0].Rows)
                {
                    list.Add(new Models.RecipeWorkflowList
                    {
                        ID = GetInt(wf["ID"]),
                        Workflow = GetStr(wf["Workflow"]),
                        Task = GetStr(wf["Task"]),
                        CodeListe = GetInt(wf["CodeListe"]),
                        Recipe = GetStr(wf["Recipe"]),
                        Attachment = GetStr(wf["Attachment"]),
                        User = GetStr(wf["AssignedUser"]),
                        DateTime = GetStr(wf["DateTime"]),
                        Duration = GetDbl(wf["Duration"]),
                        TaskStatus = GetStr(wf["TaskStatus"]),
                        CodeWorkflowTask = GetInt(wf["CodeWorkflowTask"]) 
                    });
                }
                return Ok(list);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/workflowtaskuser/{codeworkflow:int?}")]
        public ActionResult<List<Models.WorkflowTaskUser>> GetWorkflowTaskAndUser(int codeworkflow)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_WorkflowTaskUser]";
                cmd.Parameters.Add("@CodeWorkflow", SqlDbType.Int).Value = codeworkflow;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                var list = new List<Models.WorkflowTaskUser>();
                foreach (DataRow wf in ds.Tables[0].Rows)
                {
                    list.Add(new Models.WorkflowTaskUser
                    {
                        ID = GetInt(wf["ID"]),
                        WorkflowCode = GetInt(wf["WorkflowCode"]),
                        Workflow = GetStr(wf["WorkflowName"]),
                        TaskCode = GetInt(wf["TaskCode"]),
                        Task = GetStr(wf["TaskName"]),
                        UserCode = GetInt(wf["UserCode"]),
                        User = GetStr(wf["UserName"]),
                        Duration = GetInt(wf["Duration"]) 
                    });
                }
                return Ok(list);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        // Save / Update Recipe Workflow Liste
        [HttpPost("api/listeworkflow")]
        public ActionResult<Models.ResponseCallBack> SaveListeWorkflow([FromBody] Models.WorkflowDataRecipe data)
        {
            var response = new Models.ResponseCallBack();
            try
            {
                if (data == null) throw new ArgumentNullException("workflow data is empty");
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

                    int ctr = 0;
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
            catch (HttpResponseException) { throw; }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Save workflow failed";
                return StatusCode(500, response);
            }
        }

        [HttpPost("api/listeworkflowupdate")]
        public ActionResult<Models.ResponseCallBack> UpdateListeWorkflow([FromBody] Models.WorkflowDataRecipe data)
        {
            var response = new Models.ResponseCallBack();
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
            catch (HttpResponseException) { throw; }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Save workflow failed";
                return StatusCode(500, response);
            }
        }

        // Delete Added Workflow (if not saved)
        [HttpPost("api/workflowtemp/delete")]
        public ActionResult<Models.ResponseCallBack> DeleteWorkflowTemp()
        {
            var response = new Models.ResponseCallBack();
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
            catch (DatabaseException)
            {
                if (resultCode == 0) resultCode = 500;
                response.Code = -1;
                response.Status = false;
                response.ReturnValue = "Not OK";
                response.Message = "Delete workflow failed";
                return StatusCode(500, response);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                return StatusCode(500, response);
            }
        }

        // Delete Recipe Liste Workflow
        [HttpPost("api/workflowliste/delete")]
        public ActionResult<Models.ResponseCallBack> DeleteWorkflowListe([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                var codes = (data.CodeList ?? new List<Models.GenericList>()).Select(c => c.Code).Distinct().ToList();
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
                if (resultCode != 0) throw new DatabaseException($"[{resultCode}] Delete recipe worlkflow liste failed");
                response.Code = 0;
                response.Message = "OK";
                response.ReturnValue = GetStr(skip.Value);
                response.Status = true;
                return Ok(response);
            }
            catch (DatabaseException)
            {
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode;
                response.Status = false;
                response.ReturnValue = string.Empty;
                response.Message = "Delete recipe worlkflow liste failed";
                return StatusCode(500, response);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Unexpected error occured";
                response.Code = 500;
                return StatusCode(500, response);
            }
        }

        // Validations
        [HttpGet("/api/validaterecipename/{recipename?}")]
        public ActionResult<List<Models.WorkflowRecipe>> ValidateRecipeName(string recipename = "")
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
                var list = new List<Models.WorkflowRecipe>();
                foreach (DataRow rc in ds.Tables[0].Rows)
                {
                    list.Add(new Models.WorkflowRecipe { CodeListe = GetStr(rc["Code"]), Name = GetStr(rc["Name"]) });
                }
                return Ok(list);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/validaterecipeworkflow/{codeliste:int?}/{coderecipeworkflow:int?}")]
        public ActionResult<Models.ResponseCallBack> ValidateRecipeWorkflow(int codeliste = 0, int coderecipeworkflow = 0)
        {
            var response = new Models.ResponseCallBack();
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
                if (ds.Tables[0].Rows.Count > 0)
                {
                    response.Code = 1;
                    response.Message = "OK";
                    response.ReturnValue = "Already exists.";
                    response.Status = true;
                }
                return Ok(response);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("/api/workflow/worklfowlisteallowdelete")]
        public ActionResult<List<Models.RecipeWorkflowList>> ValidateWorflowlisteAllowdelete([FromBody] Models.GenericDeleteData data)
        {
            try
            {
                var codes = (data.CodeList ?? new List<Models.GenericList>()).Select(c => c.Code).Distinct().ToList();
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
                var list = new List<Models.RecipeWorkflowList>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(new Models.RecipeWorkflowList
                    {
                        ID = GetInt(r["ID"]),
                        Task = GetStr(r["Task"]),
                        CodeWorkflowTask = GetInt(r["CodeWorkflowTask"]),
                        TaskStatus = GetStr(r["TaskStatus"]) 
                    });
                }
                return Ok(list);
            }
            catch (HttpResponseException) { throw; }
            catch (Exception) { return StatusCode(500); }
        }

        // Helpers
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
            catch (Exception)
            {
                // swallow like original Log.Error("Save attachment failed")
            }
        }

        private string FilesFolder
        {
            get
            {
                var tmp = GetStr(Config?["files"]).Trim();
                if (string.IsNullOrEmpty(tmp)) tmp = Common.MapPath("files");
                if (string.IsNullOrEmpty(tmp)) tmp = Directory.GetCurrentDirectory();
                return tmp.TrimEnd('\\') + "\\";
            }
        }

        private string TempFolder2
        {
            get
            {
                var tmp = GetStr(Config?["temp"]).Trim();
                if (string.IsNullOrEmpty(tmp)) tmp = Common.MapPath("temp");
                if (string.IsNullOrEmpty(tmp)) tmp = Path.GetTempPath();
                return tmp.TrimEnd('\\') + "\\";
            }
        }

        private static int GetInt(object value, int fallback = 0)
        {
            if (value == null || value == DBNull.Value) return fallback;
            if (value is int i) return i;
            if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var ii)) return ii;
            try { return Convert.ToInt32(value, CultureInfo.InvariantCulture); } catch { return fallback; }
        }
        private static double GetDbl(object value, double fallback = 0)
        {
            if (value == null || value == DBNull.Value) return fallback;
            if (value is double d) return d;
            if (double.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var dd)) return dd;
            try { return Convert.ToDouble(value, CultureInfo.InvariantCulture); } catch { return fallback; }
        }
        private static string GetStr(object value, string fallback = "")
        {
            if (value == null || value == DBNull.Value) return fallback;
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
        private static bool GetBool(object value)
        {
            if (value == null || value == DBNull.Value) return false;
            if (value is bool b) return b;
            if (int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var i)) return i != 0;
            if (bool.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out var bb)) return bb;
            return false;
        }
    }
}

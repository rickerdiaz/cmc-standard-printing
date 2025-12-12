using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CalcmenuAPI
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcedureTemplateController : ControllerBase
    {
        private string ConnectionString => HttpContext.RequestServices.GetService<IConfiguration>()?.GetConnectionString("Default") ?? string.Empty;
        private IConfiguration Config => HttpContext.RequestServices.GetService<IConfiguration>();

        private string m_PictureNames = string.Empty;
        private string m_TempPictureNames = string.Empty;
        private string[] arrPictures = Array.Empty<string>();

        [HttpGet("{codesite:int}/{codetrans:int}/{type:int}/{name?}")]
        public ActionResult<List<Models.ProcedureTemplateInfo>> SearchProcedureTemplate(int codesite, int codetrans, int type, string name = "")
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                var dsn = Config?.GetSection("AppSettings")?["dsn"] ?? ConnectionString;
                cmd.Connection = new SqlConnection(dsn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_ProcedureTemplate]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                cmd.Connection.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                cmd.Connection.Close();

                var templates = new List<Models.ProcedureTemplateInfo>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    templates.Add(new Models.ProcedureTemplateInfo
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Global = GetBool(r["Global"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    var result = new List<Models.ProcedureTemplateInfo>();
                    foreach (var word in name.Split(','))
                    {
                        var w = Common.ReplaceSpecialCharacters(word.Trim().ToLowerInvariant());
                        foreach (var c in templates)
                        {
                            if (c.Name.ToLowerInvariant().Contains(w)) result.Add(c);
                        }
                    }
                    templates = result;
                }
                return Ok(templates);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("search")]
        public ActionResult<List<Models.ProcedureTemplateInfo>> SearchProcedureTemplate2([FromBody] Models.ConfigurationcSearch data)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                var dsn = Config?.GetSection("AppSettings")?["dsn"] ?? ConnectionString;
                cmd.Connection = new SqlConnection(dsn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_ProcedureTemplate]";
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = data.CodeTrans;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = data.Type;
                cmd.Connection.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                cmd.Connection.Close();

                var templates = new List<Models.ProcedureTemplateInfo>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    templates.Add(new Models.ProcedureTemplateInfo
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        Global = GetBool(r["Global"]) 
                    });
                }

                if (!string.IsNullOrWhiteSpace(data.Name))
                {
                    var result = new List<Models.ProcedureTemplateInfo>();
                    foreach (var word in data.Name.Split(','))
                    {
                        var w = Common.ReplaceSpecialCharacters(word.Trim().ToLowerInvariant());
                        foreach (var c in templates)
                        {
                            if (c.Name.ToLowerInvariant().Contains(w)) result.Add(c);
                        }
                    }
                    templates = result;
                }

                return Ok(templates);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("/api/proceduretemplateinfo/{codetemplate:int}/{codesite:int}/{codetrans:int}/{type:int}")]
        public ActionResult<Models.ProcedureTemplate> GetProcedureTemplateInfo(int codetemplate, int codesite, int codetrans, int type)
        {
            var template = new Models.ProcedureTemplate();
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_ProcedureTemplateInfo]";
                cmd.Parameters.Add("@CodeTemplate", SqlDbType.Int).Value = codetemplate;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = codesite;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codetrans;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                cn.Open();
                using var dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    var info = new Models.ProcedureTemplateInfo();
                    while (dr.Read())
                    {
                        info.Code = GetInt(dr["Code"]);
                        info.Name = GetStr(dr["Name"]);
                        info.Global = GetBool(dr["Global"]);
                        info.Note = GetStr(dr["Note"]);
                        template.Global = GetBool(dr["Global"]);
                    }
                    template.Info = info;
                }

                dr.NextResult();
                var translation = new List<Models.RecipeTranslation>();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        translation.Add(new Models.RecipeTranslation
                        {
                            CodeTrans = GetInt(dr["CodeTrans"]),
                            TranslationName = GetStr(dr["TranslationName"]),
                            Name = GetStr(dr["Name"]) 
                        });
                    }
                    template.Translations = translation;
                }

                dr.NextResult();
                var procedureTranslation = new List<Models.RecipeProcedureTranslation>();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        procedureTranslation.Add(new Models.RecipeProcedureTranslation
                        {
                            NoteId = GetInt(dr["NoteId"]),
                            CodeTrans = GetInt(dr["CodeTrans"]),
                            Note = GetStr(dr["Note"]),
                            AbbrevNote = GetStr(dr["AbbrevNote"]),
                            Position = GetInt(dr["Position"]) 
                        });
                    }
                }

                dr.NextResult();
                if (dr.HasRows)
                {
                    var procedures = new List<Models.RecipeProcedure>();
                    while (dr.Read())
                    {
                        var noteId = GetInt(dr["NoteId"]);
                        procedures.Add(new Models.RecipeProcedure
                        {
                            NoteId = noteId,
                            Position = GetInt(dr["Position"]),
                            Note = GetStr(dr["Note"]),
                            AbbrevNote = GetStr(dr["AbbrevNote"]),
                            Translation = procedureTranslation.Where(c => c.NoteId == noteId).ToList(),
                            Picture = GetStr(dr["Picture"]),
                            hasPicture = GetStr(dr["hasPicture"]) 
                        });
                    }
                    template.Procedures = procedures;
                }

                dr.NextResult();
                if (dr.HasRows)
                {
                    var users = new List<Models.GenericList>();
                    while (dr.Read())
                    {
                        users.Add(new Models.GenericList { Code = GetInt(dr["CodeUser"]), Value = GetStr(dr["IsAssigned"]), Name = GetStr(dr["Name"]) });
                    }
                    template.Users = users;
                }
                return Ok(template);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("sharing/{codetemplate:int}")]
        public ActionResult<List<Models.TreeNode>> GetProcedureTemplateSharing(int codetemplate)
        {
            try
            {
                var ds = new DataSet();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[dbo].[API_GET_SharingProcedureTemplate]";
                cmd.Parameters.Add("@CodeTemplate", SqlDbType.Int).Value = codetemplate;
                cn.Open();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                var sharings = new List<Models.GenericTree>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    sharings.Add(new Models.GenericTree
                    {
                        Code = GetInt(r["Code"]),
                        Name = GetStr(r["Name"]),
                        ParentCode = GetInt(r["ParentCode"]),
                        ParentName = GetStr(r["ParentName"]),
                        Flagged = GetBool(r["Flagged"]),
                        Type = GetInt(r["Type"]),
                        Global = GetBool(r["Global"]) 
                    });
                }

                var sharingdata = new List<Models.TreeNode>();
                var parents = sharings.Where(obj => obj.ParentCode == 0 && obj.Type == 1).OrderBy(obj => obj.Name).ToList();
                foreach (var p in parents)
                {
                    var parent = new Models.TreeNode
                    {
                        title = p.Name,
                        key = p.Code,
                        icon = false,
                        children = CreateChildrenSharing(sharings, p.Code),
                        select = p.Flagged,
                        selected = p.Flagged,
                        parenttitle = p.ParentName,
                        groupLevel = GroupLevel.Property
                    };
                    if (parent.children != null && parent.children.Count > 0)
                        sharingdata.Add(parent);
                }
                return Ok(sharingdata);
            }
            catch (ArgumentException) { return BadRequest(); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost]
        public ActionResult<Models.ResponseCallBack> SaveProcedureTemplate([FromBody] Models.ProcedureTemplate data)
        {
            var response = new Models.ResponseCallBack();
            SqlTransaction? _trans = null;
            int resultCode = 0;
            try
            {
                if (data?.Info == null) return BadRequest();
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                int intCodeTemplate = -1;
                int intCodeUser = GetInt(data.Info.CodeUser);

                var arrSharing = new ArrayList();
                if (data.Sharing != null)
                {
                    foreach (var sh in data.Sharing)
                    {
                        if (!arrSharing.Contains(sh.Code)) arrSharing.Add(sh.Code);
                    }
                }
                var codeSharedTo = Common.Join(arrSharing, string.Empty, string.Empty, ",");

                cmd.CommandTimeout = 120;
                cmd.Connection = cn;
                cmd.CommandText = "[dbo].[API_UPDATE_ProcedureTemplate]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@CodeTemplate", SqlDbType.Int).Value = data.Info.Code;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = data.Info.Name;
                cmd.Parameters.Add("@Type", SqlDbType.Int).Value = data.Info.Type;
                cmd.Parameters.Add("@Global", SqlDbType.Bit).Value = GetBool(data.Info.Global);
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int, 260).Value = data.Info.CodeSite;
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int, 20).Value = data.Info.CodeTrans;
                cmd.Parameters.Add("@Note", SqlDbType.NVarChar).Value = data.Info.Note;
                cmd.Parameters.Add("@CodeSiteList", SqlDbType.NVarChar, 2000).Value = codeSharedTo;
                cmd.Parameters.Add("@CodeTemplateNew", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.Output;
                cn.Open();
                _trans = cn.BeginTransaction();
                cmd.Transaction = _trans;
                cmd.ExecuteNonQuery();
                intCodeTemplate = GetInt(cmd.Parameters["@CodeTemplateNew"].Value, -1);
                resultCode = GetInt(cmd.Parameters["@Return"].Value, -1);
                if (resultCode != 0) return StatusCode(500, Fail(response, resultCode, "Procedure Template update failed"));
                _trans.Commit();

                if (intCodeTemplate > 0)
                {
                    if (data.Sharing != null)
                    {
                        cmd.CommandText = Common.SP_API_UPDATE_Sharing;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@Code", SqlDbType.Int).Value = intCodeTemplate;
                        cmd.Parameters.Add("@CodeOwner", SqlDbType.Int).Value = intCodeUser;
                        cmd.Parameters.Add("@CodeSharedToList", SqlDbType.VarChar, 4000).Value = codeSharedTo;
                        cmd.Parameters.Add("@Type", SqlDbType.Int).Value = 128;
                        cmd.Parameters.Add("@CodeEgswTable", SqlDbType.Int).Value = 50;
                        cmd.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = data.Info.Global;
                        cmd.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                        cmd.ExecuteNonQuery();
                        resultCode = GetInt(cmd.Parameters["@Return"].Value, -1);
                        if (resultCode != 0) return StatusCode(500, Fail(response, resultCode, "Save procedure template sharing failed"));
                    }

                    var dsCurrentProcedure = new DataSet();
                    cmd.CommandText = "sp_EgswGetInstruction";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeTemplate;
                    cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = data.Info.CodeTrans;
                    using (var da = new SqlDataAdapter(cmd)) da.Fill(dsCurrentProcedure);

                    if (data.Procedures != null && data.Procedures.Count > 0)
                    {
                        int _noteID;
                        if (dsCurrentProcedure != null && dsCurrentProcedure.Tables.Count > 0 && dsCurrentProcedure.Tables[0].Rows.Count > 0)
                        {
                            var dtProcedure = dsCurrentProcedure.Tables[0];
                            cmd.CommandText = @"DELETE FROM EgswListeNoteTrans WHERE EgswListeNoteID = @NoteID; 
DELETE FROM EgswListeNote WHERE ID = @NoteID;";
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@NoteID", SqlDbType.Int);
                            foreach (DataRow dr in dtProcedure.Rows)
                            {
                                _noteID = GetInt(dr["ID"]);
                                if (data.Procedures.Where(s => s.NoteId == _noteID).Count() == 0)
                                {
                                    cmd.Parameters["@NoteID"].Value = _noteID;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        var procpics = string.Empty;
                        foreach (var procedure in data.Procedures)
                        {
                            cmd.CommandText = "sp_EgswListeNoteUpdate";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@NoteId", SqlDbType.Int).Value = procedure.NoteId;
                            cmd.Parameters.Add("@Codeliste", SqlDbType.Int).Value = intCodeTemplate;
                            cmd.Parameters.Add("@Position", SqlDbType.Int).Value = procedure.Position;
                            cmd.Parameters.Add("@DigitalAsset", SqlDbType.NVarChar, 2000).Value = DBNull.Value;
                            cmd.Parameters.Add("@Picture", SqlDbType.NVarChar, 2000).Value = GetStr(procedure.Picture);
                            cmd.Parameters.Add("@FreakOutMoment", SqlDbType.Bit).Value = DBNull.Value;
                            cmd.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add("@retVal", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                            cmd.ExecuteNonQuery();
                            resultCode = GetInt(cmd.Parameters["@retval"].Value, -1);
                            if (resultCode != 0) return StatusCode(500, Fail(response, resultCode, "Update Procedure Template notes failed"));
                            _noteID = procedure.NoteId <= 0 ? GetInt(cmd.Parameters["@ID"].Value, -1) : procedure.NoteId;

                            if (_noteID > 0 && procedure.Translation != null)
                            {
                                cmd.CommandText = @"IF EXISTS ( SELECT ID FROM EgsWListeNoteTrans WHERE EgswListeNoteId = @NoteID AND CodeTrans = @Codetrans ) 
BEGIN 
    UPDATE EgswListeNoteTrans SET Note = @Note, Comment = @Comment, CookMode = @CookMode WHERE EgswListeNoteId = @NoteID AND CodeTrans = @Codetrans
END 
ELSE BEGIN
    INSERT INTO EgswListeNoteTrans ( EgswListeNoteID, CodeTrans, Note, Comment, CookMode ) VALUES ( @NoteID, @CodeTrans, @Note, @Comment, @CookMode )
END";
                                cmd.CommandType = CommandType.Text;
                                foreach (var item in procedure.Translation)
                                {
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add("@NoteID", SqlDbType.Int).Value = _noteID;
                                    cmd.Parameters.Add("@Codetrans", SqlDbType.Int).Value = item.CodeTrans;
                                    cmd.Parameters.Add("@Note", SqlDbType.NVarChar, 2000).Value = data.Info.CodeTrans == item.CodeTrans ? procedure.Note : (!string.IsNullOrEmpty(item.Note) ? item.Note : string.Empty);
                                    cmd.Parameters.Add("@Comment", SqlDbType.NVarChar, 2000).Value = string.Empty;
                                    cmd.Parameters.Add("@CookMode", SqlDbType.NVarChar, 4000).Value = item.AbbrevNote;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            procpics += (procedure.Picture ?? string.Empty) + ";";
                        }
                        m_PictureNames = procpics;
                        m_TempPictureNames = data.TempPictures ?? string.Empty;
                        arrPictures = (procpics ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries);
                        var th = new Thread(SavePictures) { Priority = ThreadPriority.Lowest };
                        th.Start();
                    }
                    else if (data.Procedures != null && data.Procedures.Count == 0)
                    {
                        cmd.CommandText = @"DELETE FROM dbo.EgswListeNoteTrans WHERE EgswListeNoteID IN ( SELECT ID FROM dbo.EgswListeNote WHERE CodeListe = @CodeListe );
DELETE FROM dbo.EgswListeNote WHERE CodeListe = @CodeListe;";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeTemplate;
                        cmd.ExecuteNonQuery();
                    }

                    if (data.translations != null)
                    {
                        foreach (var t in data.translations)
                        {
                            cmd.CommandText = @"IF EXISTS(SELECT Id FROM EgswListeTranslation WHERE CodeListe=@CodeListe AND CodeTrans=@CodeTrans)
BEGIN
    UPDATE EgswListeTranslation SET [Name]=@Name, [SubTitle]=@SubName, [Remark]=@Remark, [Description]=@Description, [FootNote1]=@FootNote1, [FootNote2]=@FootNote2,  [FootNote1Clean]=dbo.[fn_CleanText](@FootNote1), [FootNote2Clean]=dbo.[fn_CleanText](@FootNote2), [CCPDescription]=@CCPDescription WHERE CodeListe=@CodeListe AND CodeTrans=@CodeTrans
END
ELSE
BEGIN
    INSERT INTO EgswListeTranslation(CodeListe, CodeTrans, [Name], [SubTitle], [Remark], [Description], [FootNote1], [FootNote2], [FootNote1Clean], [FootNote2Clean], [CCPDescription]) VALUES(@CodeListe, @CodeTrans, @Name, @SubName, @Remark, @Description, @FootNote1, @FootNote2, dbo.[fn_CleanText](@FootNote1), dbo.[fn_CleanText](@FootNote2), @CCPDescription)
END";
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeTemplate;
                            cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = t.CodeTrans;
                            cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = GetStr(t.Name);
                            cmd.Parameters.Add("@SubName", SqlDbType.NVarChar).Value = GetStr(t.SubName);
                            cmd.Parameters.Add("@Remark", SqlDbType.NVarChar).Value = GetStr(t.Remark);
                            cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = GetStr(t.Description);
                            cmd.Parameters.Add("@FootNote1", SqlDbType.NVarChar).Value = GetStr(t.Notes);
                            cmd.Parameters.Add("@FootNote2", SqlDbType.NVarChar).Value = GetStr(t.AdditionalNotes);
                            cmd.Parameters.Add("@CCPDescription", SqlDbType.NVarChar).Value = GetStr(t.CCPDescription);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    if (data.Users != null)
                    {
                        var userList = data.Users.Where(row => row.Value == "1").Select(row => row.Code).Distinct().ToList();
                        var codeusers = string.Join(",", userList);
                        cmd.CommandText = "[dbo].[API_UPDATE_ProcedureTemplateUser]";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CodeTemplate", SqlDbType.Int).Value = intCodeTemplate;
                        cmd.Parameters.Add("@CodeUsers", SqlDbType.VarChar, 4000).Value = codeusers;
                        cmd.Parameters.Add("@CodeUserOwner", SqlDbType.Int).Value = intCodeUser;
                        cmd.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                        cmd.ExecuteNonQuery();
                        resultCode = GetInt(cmd.Parameters["@Return"].Value, -1);
                        if (resultCode != 0) return StatusCode(500, Fail(response, resultCode, "Save Procedure Template users failed"));
                    }
                }

                response.Code = 0; response.ReturnValue = intCodeTemplate; response.Message = Common.ReplaceSpecialCharacters(data.Info.Name) + " successfully saved."; response.Status = true;
                return Ok(response);
            }
            catch (Exception)
            {
                try { _trans?.Rollback(); } catch { }
                if (resultCode == 0) resultCode = 500;
                return StatusCode(500, Fail(response, resultCode, "Save Procedure Template failed"));
            }
        }

        [HttpPost("delete")]
        public ActionResult<Models.ResponseCallBack> DeleteProcedureTemplate([FromBody] Models.GenericDeleteData data)
        {
            var response = new Models.ResponseCallBack();
            int resultCode = 0;
            try
            {
                using var cmd = new SqlCommand();
                using var cn = new SqlConnection(ConnectionString);
                var arrCodes = new ArrayList();
                foreach (var c in data.CodeList)
                {
                    if (!arrCodes.Contains(c.Code)) arrCodes.Add(c.Code);
                }
                var codeList = Common.Join(arrCodes, string.Empty, string.Empty, ",");
                cmd.Connection = cn;
                cmd.CommandText = "API_DELETE_Generic";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@CodeList", SqlDbType.VarChar, 4000).Value = codeList;
                cmd.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = "PROCEDURETEMPLATE";
                cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = data.CodeUser;
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = data.CodeSite;
                cmd.Parameters.Add("@ForceDelete", SqlDbType.Bit).Value = data.ForceDelete;
                cmd.Parameters.Add("@SkipList", SqlDbType.VarChar, 4000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                cn.Open();
                cmd.ExecuteNonQuery();
                resultCode = GetInt(cmd.Parameters["@Return"].Value, -1);
                if (resultCode != 0) return StatusCode(500, Fail(response, resultCode, "Delete procedure template failed"));
                response.Code = 0; response.Message = "OK"; response.ReturnValue = GetStr(cmd.Parameters["@SkipList"].Value); response.Status = true;
                return Ok(response);
            }
            catch (Exception)
            {
                if (resultCode == 0) resultCode = 500;
                response.Code = resultCode; response.Status = false; response.ReturnValue = string.Empty; response.Message = "Delete procedure template failed";
                return StatusCode(500, response);
            }
        }

        private List<Models.TreeNode> CreateChildrenSharing(List<Models.GenericTree> sharingdata, int code)
        {
            var children = new List<Models.TreeNode>();
            if (sharingdata != null)
            {
                var kids = sharingdata.Where(obj => obj.ParentCode == code && obj.Type == 2).OrderBy(obj => obj.Name).ToList();
                foreach (var k in kids)
                {
                    var child = new Models.TreeNode
                    {
                        title = k.Name,
                        key = k.Code,
                        icon = false,
                        children = null,
                        select = k.Flagged,
                        parenttitle = k.ParentName,
                        note = k.Global
                    };
                    children.Add(child);
                }
            }
            return children;
        }

        private void SavePictures()
        {
            try
            {
                var strTempFolder = TempFolder2;
                if (!string.IsNullOrWhiteSpace(m_PictureNames))
                {
                    var arrPictureNames = m_PictureNames.Trim().Split(';', StringSplitOptions.RemoveEmptyEntries);
                    m_PictureNames = string.Join(";", arrPictures);
                    if (arrPictureNames.Length > 0)
                    {
                        for (int ctr = 0; ctr < arrPictureNames.Length; ctr++)
                        {
                            var name = arrPictureNames[ctr];
                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                var pic = name;
                                string source;
                                if (pic.Contains("| DAM", StringComparison.OrdinalIgnoreCase))
                                {
                                    pic = pic.Substring(0, pic.IndexOf("|", StringComparison.Ordinal));
                                    source = Path.Combine(DamFolder, pic);
                                    File.Copy(source, Path.Combine(strTempFolder, arrPictures.ElementAtOrDefault(ctr) ?? pic), true);
                                    pic = arrPictures.ElementAtOrDefault(ctr) ?? pic;
                                }
                                source = Path.Combine(strTempFolder, pic);
                                if (File.Exists(source))
                                {
                                    File.Copy(source, Path.Combine(PicOriginalFolder, pic), true);
                                    fctResizeConvertImage(source, Path.Combine(PicNormalFolder, pic), 300, 300, false);
                                    fctResizeConvertImage(source, Path.Combine(PicThumbnailFolder, pic), 200, 200, false);
                                }
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(m_TempPictureNames))
                {
                    var arrPictureNames = m_TempPictureNames.Trim().Split(';', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var pic in arrPictureNames)
                    {
                        if (!string.IsNullOrWhiteSpace(pic) && !pic.Contains("| DAM", StringComparison.OrdinalIgnoreCase))
                        {
                            var source = Path.Combine(strTempFolder, pic);
                            if (File.Exists(source)) File.Delete(source);
                            if ((m_PictureNames ?? string.Empty).IndexOf(pic, StringComparison.OrdinalIgnoreCase) == -1)
                            {
                                var po = Path.Combine(PicOriginalFolder, pic); if (File.Exists(po)) File.Delete(po);
                                var pn = Path.Combine(PicNormalFolder, pic); if (File.Exists(pn)) File.Delete(pn);
                                var pt = Path.Combine(PicThumbnailFolder, pic); if (File.Exists(pt)) File.Delete(pt);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public string TempFolder2
        {
            get
            {
                var tmp = GetStr(Config?.GetSection("AppSettings")?["temp"]).Trim();
                if (string.IsNullOrEmpty(tmp)) tmp = Common.MapPath("temp");
                return tmp.TrimEnd('\\') + "\\";
            }
        }

        public string DamFolder
        {
            get
            {
                var tmp = GetStr(Config?.GetSection("AppSettings")?["dam"]).Trim();
                if (string.IsNullOrEmpty(tmp)) tmp = Common.MapPath("DigitalAssets");
                return tmp.TrimEnd('\\') + "\\";
            }
        }

        public string PicNormalFolder
        {
            get
            {
                var tmp = GetStr(Config?.GetSection("AppSettings")?["picnormal"]).Trim();
                if (string.IsNullOrEmpty(tmp)) tmp = Common.MapPath("picnormal");
                return tmp.TrimEnd('\\') + "\\";
            }
        }

        public string PicThumbnailFolder
        {
            get
            {
                var tmp = GetStr(Config?.GetSection("AppSettings")?["picthumbnail"]).Trim();
                if (string.IsNullOrEmpty(tmp)) tmp = Common.MapPath("picthumbnail");
                return tmp.TrimEnd('\\') + "\\";
            }
        }

        public string PicOriginalFolder
        {
            get
            {
                var tmp = GetStr(Config?.GetSection("AppSettings")?["picoriginal"]).Trim();
                if (string.IsNullOrEmpty(tmp)) tmp = Common.MapPath("picoriginal");
                return tmp.TrimEnd('\\') + "\\";
            }
        }

        private bool fctResizeConvertImage(string strFile, string strDestination, int newWidth, int newHeight, bool blnDelete = false)
        {
            try
            {
                double dblTempW, dblTempH;
                int H1, W1;
                using var originalBitmap = Image.FromFile(strFile);
                var ratio = (decimal)originalBitmap.Height / (decimal)originalBitmap.Width;
                if (newHeight > newWidth)
                {
                    dblTempH = newHeight;
                    dblTempW = dblTempH / (double)ratio;
                }
                else
                {
                    dblTempW = newWidth;
                    dblTempH = dblTempW * (double)ratio;
                }
                while (dblTempW > newWidth || dblTempH > newHeight)
                {
                    dblTempW *= 0.999;
                    dblTempH *= 0.999;
                }
                W1 = (int)dblTempW; H1 = (int)dblTempH;
                using var newbmp = new Bitmap(W1, H1);
                using (var g = Graphics.FromImage(newbmp))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.Clear(Color.White);
                    g.DrawImage(originalBitmap, 0, 0, W1, H1);
                }
                Directory.CreateDirectory(Path.GetDirectoryName(strDestination) ?? "");
                newbmp.Save(strDestination, System.Drawing.Imaging.ImageFormat.Jpeg);
                if (blnDelete && File.Exists(strFile)) File.Delete(strFile);
            }
            catch { return false; }
            return true;
        }

        // Helpers
        private static int GetInt(object value, int fallback = 0) { if (value == null || value == DBNull.Value) return fallback; if (int.TryParse(Convert.ToString(value), out var i)) return i; try { return Convert.ToInt32(value); } catch { return fallback; } }
        private static string GetStr(object value) => value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value);
        private static bool GetBool(object value) { if (value == null || value == DBNull.Value) return false; if (value is bool b) return b; if (int.TryParse(Convert.ToString(value), out var i)) return i != 0; if (bool.TryParse(Convert.ToString(value), out var bb)) return bb; return false; }
    }
}

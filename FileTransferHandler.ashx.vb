Imports System.Collections.Generic
Imports System.Configuration
Imports System.IO
Imports System.Linq
Imports System.Web
Imports System.Web.Script.Serialization
Imports log4net
Imports System.Drawing
Imports System.Drawing.Imaging


Public Class FileTransferHandler
    Implements IHttpHandler

    Private Shared ReadOnly Log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Private ReadOnly js As New JavaScriptSerializer()

    Public ReadOnly Property TempFolder() As String
        Get
            Dim tmp As String = GetStr(ConfigurationManager.AppSettings("temp")).Trim()
            If tmp.Equals(String.Empty) Then
                tmp = Common.MapPath("temp")
            End If
            Return tmp.TrimEnd("\") + "\"
        End Get
    End Property

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.AddHeader("Pragma", "no-cache")
        context.Response.AddHeader("Cache-Control", "private, no-cache")

        HandleMethod(context)
    End Sub

    ' Handle request based on method
    Private Sub HandleMethod(context As HttpContext)
        Try
            Select Case context.Request.HttpMethod
                Case "HEAD", "GET"
                    If GivenFilename(context) Then
                        DeliverFile(context)
                    Else
                        'ListCurrentFiles(context)
                    End If
                    Exit Select

                Case "POST", "PUT"
                    UploadFile(context)
                    Exit Select

                Case "DELETE"
                    DeleteFile(context)
                    Exit Select

                Case "OPTIONS"
                    ReturnOptions(context)
                    Exit Select
                Case Else

                    context.Response.ClearHeaders()
                    context.Response.StatusCode = 405
                    Exit Select
            End Select
        Catch ex As Exception
            Log.Error("File transfer failed", ex)
            PreserveStackTrace(ex)
            Throw ex
        End Try
    End Sub

    Private Shared Sub ReturnOptions(context As HttpContext)
        context.Response.AddHeader("Allow", "DELETE,GET,HEAD,POST,PUT,OPTIONS")
        context.Response.StatusCode = 200
    End Sub

    ' Delete file from the server
    Private Sub DeleteFile(context As HttpContext)
        Try
            Dim filePath = TempFolder + context.Request("f")
            If File.Exists(filePath) Then
                File.Delete(filePath)
            End If
        Catch ex As Exception
            Log.Error("Unable to delete file", ex)
        End Try
    End Sub

    ' Upload file to the server
    Private Sub UploadFile(context As HttpContext)
        Try
            Dim statuses = New List(Of FilesStatus)()
            Dim headers = context.Request.Headers

            If String.IsNullOrEmpty(headers("X-File-Name")) Then
                UploadWholeFile(context, statuses)
            Else
                UploadPartialFile(headers("X-File-Name"), context, statuses)
            End If

            WriteJsonIframeSafe(context, statuses)
        Catch ex As Exception
            Log.Error("Unable to upload file", ex)
        End Try
    End Sub

    ' Upload partial file
    Private Sub UploadPartialFile(fileName As String, context As HttpContext, statuses As List(Of FilesStatus))
        Try
            If context.Request.Files.Count <> 1 Then
                Throw New HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request")
            End If
            Dim inputStream = context.Request.Files(0).InputStream
            Dim fullName = TempFolder & Path.GetFileName(fileName)

            Using fs = New FileStream(fullName, FileMode.Append, FileAccess.Write)
                Dim buffer = New Byte(1023) {}

                Dim l = inputStream.Read(buffer, 0, 1024)
                While l > 0
                    fs.Write(buffer, 0, l)
                    l = inputStream.Read(buffer, 0, 1024)
                End While
                fs.Flush()
                fs.Close()
            End Using
            statuses.Add(New FilesStatus(New FileInfo(fullName)))
        Catch ex As Exception
            Log.Error("Unable to upload partial file", ex)
        End Try
    End Sub

    ' Upload entire file
    Private Sub UploadWholeFile(context As HttpContext, statuses As List(Of FilesStatus))
        Try
            If context.Request.Files.Count > 0 Then ' RBAJ-2013.12.16
                Dim newfilename As String = String.Empty
                Dim file = context.Request.Files(0)

                If Not System.IO.Directory.Exists(TempFolder) Then
                    System.IO.Directory.CreateDirectory(TempFolder)
                End If

                'JDO 12.12.2013 CWA-9473

                If context.Request.Files.AllKeys(0).ToString() = "filepicture" AndAlso context.Request.Form.Count > 0 Then

                    Dim pictureNumber = GetInt(context.Request.Form(0), 1)
                    Dim str As String = file.ContentType.ToString().Substring(0, file.ContentType.ToString().IndexOf("/"))
                    Dim fileExtension As String = Path.GetExtension(file.FileName) ' RBAJ-2013.12.19
                    If str = "image" Then
                        newfilename = "P" & Format(Now, "MMddyyHHmmss") & "_" & pictureNumber.ToString() + fileExtension
                        If DebugEnabled Then
                            Log.Info("Uploading:[" + newfilename + "]")
                        End If
                        file.SaveAs(TempFolder & newfilename)
                        TestRotate(TempFolder & newfilename)
                        statuses.Add(New FilesStatus(newfilename, file.ContentLength))
                    Else
                        statuses.Add(New FilesStatus() With {.result_code = 440, .error = "Invalid file type"}) ' RBAJ-2014.01.07
                        Throw New ArgumentException("Invalid file type")
                    End If
                ElseIf context.Request.Files.AllKeys(0).ToString().Contains("fileprocpicture") Or context.Request.Files.AllKeys(0).ToString().Contains("filecookbookpicture") Then 'WVM-2014.09.30
                    Dim strFile As String = context.Request.Files.AllKeys(0).ToString()
                    strFile = strFile.Substring(15, strFile.Length - 15)
                    Dim pictureNumber = GetInt(strFile)
                    Dim str As String = file.ContentType.ToString().Substring(0, file.ContentType.ToString().IndexOf("/"))
                    Dim fileExtension As String = Path.GetExtension(file.FileName)
                    If str = "image" Then
                        newfilename = "P" & Format(Now, "MMddyyHHmmss") & "_" & pictureNumber.ToString() + fileExtension
                        If DebugEnabled Then
                            Log.Info("Uploading:[" + newfilename + "]")
                        End If
                        file.SaveAs(TempFolder & newfilename)
                        TestRotate(TempFolder & newfilename)
                        statuses.Add(New FilesStatus(newfilename, file.ContentLength))
                    Else
                        statuses.Add(New FilesStatus() With {.result_code = 440, .error = "Invalid file type"})
                        Throw New ArgumentException("Invalid file type")
                    End If
                ElseIf context.Request.Files.AllKeys(0).ToString().Contains("fileToUpload") Then 'WVM-2014.11.06
                    Dim str As String = file.ContentType.ToString().Substring(0, file.ContentType.ToString().IndexOf("/"))
                    Dim fileExtension As String = Path.GetExtension(file.FileName)

                    If str = "image" Then
                        newfilename = "P" & Format(Now, "MMddyyHHmmss") & fileExtension
                        If DebugEnabled Then
                            Log.Info("Uploading:[" + newfilename + "]")
                        End If
                        file.SaveAs(TempFolder & newfilename)
                        TestRotate(TempFolder & newfilename)
                        statuses.Add(New FilesStatus(newfilename, file.ContentLength))
                    ElseIf str = "video" Then
                        newfilename = "V" & Format(Now, "MMddyyHHmmss") & fileExtension
                        If DebugEnabled Then
                            Log.Info("Uploading:[" + newfilename + "]")
                        End If
                        file.SaveAs(TempFolder & newfilename)
                        TestRotate(TempFolder & newfilename)
                        statuses.Add(New FilesStatus(newfilename, file.ContentLength))
                    Else
                        statuses.Add(New FilesStatus() With {.result_code = 440, .error = "Invalid file type"})
                        Throw New ArgumentException("Invalid file type")
                    End If
                Else
                    newfilename = Path.GetFileName(file.FileName)
                    If DebugEnabled Then
                        Log.Info("Uploading:[" + newfilename + "]")
                    End If
                    file.SaveAs(TempFolder & newfilename)
                    TestRotate(TempFolder & newfilename)
                    statuses.Add(New FilesStatus(newfilename, file.ContentLength))
                End If

            End If
        Catch ex As Exception
            Log.Error("Unable to upload whole file", ex)
        End Try
    End Sub

    Private Sub WriteJsonIframeSafe(context As HttpContext, statuses As List(Of FilesStatus))
        context.Response.AddHeader("Vary", "Accept")
        Try
            If context.Request("HTTP_ACCEPT").Contains("application/json") Then
                context.Response.ContentType = "application/json"
            Else
                context.Response.ContentType = "text/plain"
            End If
        Catch
            context.Response.ContentType = "text/plain"
        End Try

        Dim jsonObj = js.Serialize(statuses.ToArray())
        context.Response.Write(jsonObj)
    End Sub

    Private Shared Function GivenFilename(context As HttpContext) As Boolean
        Return Not String.IsNullOrEmpty(context.Request("f"))
    End Function

    Private Sub DeliverFile(context As HttpContext)
        Try
            Dim filename = context.Request("f")
            Dim filePath = TempFolder & Convert.ToString(filename)

            If File.Exists(filePath) Then
                context.Response.AddHeader("Content-Disposition", "attachment; filename=""" + filename + """")
                context.Response.ContentType = "application/octet-stream"
                context.Response.ClearContent()
                context.Response.WriteFile(filePath)
            Else
                context.Response.StatusCode = 404
            End If
        Catch ex As Exception
            Log.Error("Unable to deliver file", ex)
        End Try
    End Sub

    Private Sub ListCurrentFiles(context As HttpContext)
        Try
            Dim files = New DirectoryInfo(TempFolder).GetFiles("*", SearchOption.TopDirectoryOnly).Where(Function(f) Not f.Attributes.HasFlag(FileAttributes.Hidden)).[Select](Function(f) New FilesStatus(f)).ToArray()

            Dim jsonObj As String = js.Serialize(files)
            context.Response.AddHeader("Content-Disposition", "inline; filename=""files.json""")
            context.Response.Write(jsonObj)
            context.Response.ContentType = "application/json"
        Catch ex As Exception
            Log.Error("Unable to list current files", ex)
        End Try
    End Sub

    Public Function TestRotate(sImageFilePath As String) As Boolean
        Dim img As Image = Image.FromFile(sImageFilePath)

        If img.PropertyIdList.Contains(&H112) Then
            Dim propOrientation As PropertyItem = img.GetPropertyItem(&H112)
            Dim orientation As Short = BitConverter.ToInt16(propOrientation.Value, 0)

            If orientation = 6 Then
                img.RotateFlip(RotateFlipType.Rotate90FlipNone)
            ElseIf orientation = 8 Then
                img.RotateFlip(RotateFlipType.Rotate270FlipNone)
            Else
            End If
        End If
    End Function


End Class
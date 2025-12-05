Imports System
Imports System.Web
Imports System.Web.Http
Imports System.Collections.Generic
Imports System.Linq
Imports AttributeRouting.Web.Http
Imports System.Data.SqlClient
Imports System.Web.Script.Serialization
Imports log4net
Imports System.Net
Imports Newtonsoft.Json

Namespace CalcmenuAPI
    Public Class TextController
        Inherits ApiController

        Private Shared ReadOnly Log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)


        <HttpPost> <[POST]("/api/text/search")> _
        Public Function GetTextByName2(data As Models.ConfigurationcSearch) As List(Of Models.Text)
            Try
                Dim ds As New DataSet
                Using cmd As New SqlCommand
                    With cmd
                        Using cn As New SqlConnection(ConnectionString)
                            Try
                                .Connection = cn
                                .CommandType = CommandType.StoredProcedure
                                .CommandText = "[dbo].[API_SEARCH_TEXT]"
                                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 2000).Value = data.Name
                                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = data.CodeTrans
                                .Parameters.Add("@rowsPerPage", SqlDbType.Int, 50).Value = 10
                                .Parameters.Add("@namefiltertype", SqlDbType.Int, 10).Value = 4
                                .Parameters.Add("@skip", SqlDbType.Int, 0).Value = 0

                                cn.Open()
                                Dim _da As New SqlDataAdapter(cmd)
                                _da.Fill(ds)
                            Finally
                                If Not cn Is Nothing Then
                                    cn.Close()
                                    CType(cn, IDisposable).Dispose()
                                End If
                            End Try
                        End Using
                    End With
                End Using
                Dim texts As New List(Of Models.Text)
                For Each r In ds.Tables(0).Rows
                    texts.Add(New Models.Text With {
                                    .TextCode = GetInt(r("Code")), _
                                    .TextName = r("Name"), _
                                    .TextDate = GetBool(r("Dates"))})
                Next


                If data.Name.Trim <> "" Then

                    Dim textresult As New List(Of Models.Text)

                    'Dim arrNames() As String = data.Name.Trim.Split(",")
                    'For Each word In arrNames
                    '    If word.Trim.ToString <> "" Then    'RDTC 2015.08.10; added this otherwise the rows are getting duplicated
                    '        For Each s In taxes
                    '            If s.TaxValue.ToString.ToLower.Contains(Common.ReplaceSpecialCharacters(word.Trim.ToLower)) Or _
                    '                s.TaxName.ToString.ToLower.Contains(Common.ReplaceSpecialCharacters(word.Trim.ToLower)) Then
                    '                taxresult.Add(s)
                    '            End If
                    '        Next
                    '    End If
                    'Next
                    texts = textresult

                End If

                Return texts.ToList
            Catch aex As ArgumentException
                Log.Warn(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Missing or invalid parameters", aex)
                Throw New HttpResponseException(GenericErrorResponse("Request failed", HttpStatusCode.BadRequest, 440))
            Catch hex As HttpResponseException
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Unexpected error occured", hex)
                Throw hex
            Catch ex As Exception
                Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Unexpected error occured", ex)
                Throw New HttpResponseException(GenericErrorResponse("Request failed", HttpStatusCode.InternalServerError, 500))
            End Try
        End Function


    End Class
End Namespace

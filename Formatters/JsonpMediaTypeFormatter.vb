Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Formatting
Imports System.Net.Http.Headers
Imports System.Threading.Tasks
Imports System.Web

Public Class JsonpMediaTypeFormatter
    Inherits JsonMediaTypeFormatter
    Private m_callbackQueryParameter As String

    Public Sub New()
        SupportedMediaTypes.Add(DefaultMediaType)
        SupportedMediaTypes.Add(New MediaTypeHeaderValue("text/javascript"))

        MediaTypeMappings.Add(New UriPathExtensionMapping("jsonp", DefaultMediaType))
    End Sub

    Public Property CallbackQueryParameter() As String
        Get
            Return If(m_callbackQueryParameter, "callback")
        End Get
        Set(value As String)
            m_callbackQueryParameter = value
        End Set
    End Property

    Public Overrides Function WriteToStreamAsync(type As Type, value As Object, stream As Stream, content As HttpContent, transportContext As TransportContext) As Task
        Dim callback As String = String.Empty

        If IsJsonpRequest(callback) Then
            Return Task.Factory.StartNew(Sub()
                                             Dim writer = New StreamWriter(stream)
                                             writer.Write(callback & Convert.ToString("("))
                                             writer.Flush()

                                             MyBase.WriteToStreamAsync(type, value, stream, content, transportContext).Wait()

                                             writer.Write(")")
                                             writer.Flush()

                                         End Sub)
        Else
            Return MyBase.WriteToStreamAsync(type, value, stream, content, transportContext)
        End If
    End Function


    Private Function IsJsonpRequest(ByRef callback As String) As Boolean
        callback = Nothing

        If HttpContext.Current.Request.HttpMethod <> "GET" Then
            Return False
        End If

        callback = HttpContext.Current.Request.QueryString(CallbackQueryParameter)

        Return Not String.IsNullOrEmpty(callback)
    End Function
End Class

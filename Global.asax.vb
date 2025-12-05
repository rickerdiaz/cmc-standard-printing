Imports System.Web.SessionState
Imports log4net

Public Class Global_asax
    Inherits System.Web.HttpApplication

    Private Shared ReadOnly Log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application is started
        Log.Info("CalcmenuAPI was started")

        ConfigManager.Initialize()
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session is started
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires at the beginning of each request
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires upon attempting to authenticate the use
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim lastError As Exception = Nothing
            If Not Server.GetLastError Is Nothing Then
                lastError = Server.GetLastError().GetBaseException()
            End If
            If lastError IsNot Nothing Then
                Log.Error(lastError.Message)
                Log.Error(lastError.StackTrace)
                Log.Info(lastError.Message)
                Log.Info(lastError.StackTrace)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session ends
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application ends
    End Sub

End Class
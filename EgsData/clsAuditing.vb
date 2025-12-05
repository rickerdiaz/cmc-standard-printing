Imports System.Data.SqlClient
Imports System.Data

Public Class clsAuditing
    Inherits clsDBRoutine
    Private L_AppType As enumAppType
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType
    Private L_udtUser As structUser


    Public Sub New(ByVal eAppType As enumAppType, ByVal strCnn As String, _
       Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)

        Try
            L_AppType = eAppType
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn
        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try
    End Sub


    Public Function GetAuditingList() As DataTable
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter

        Dim cmd As New SqlCommand
        Dim cn As New SqlConnection(L_strCnn)
        ''Dim strText As String = "SELECT * FROM AuditList WHERE YEAR(Dates)='" & Date.Today.Year & _
        ''                        "' AND MONTH(DATES)= '" & Date.Today.Month & _
        ''                        "' AND DAY(DATES)='" & Date.Today.Day & "'"

        Dim strText As String = "SELECT * FROM AuditList "
                                
        Try
            With cmd
                .Connection = cn
                .CommandText = strText
                .CommandType = CommandType.Text

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                cn.Close()
            End With
        Catch ex As Exception

        End Try

        Return dt
    End Function
End Class

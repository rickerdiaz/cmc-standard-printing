Imports System.Data.SqlClient

Public Class clsMenuEngineering
    Inherits clsDBRoutine

    Private L_AppType As enumAppType
    Private L_bytFetchType As enumEgswFetchType
    Private L_strCnn As String


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

    Public Function UpdateMenuItems(ByVal strCodeList As String, ByVal intCodeUser As Integer, _
        ByVal blnUseAverage As Boolean, ByVal dtmDateFrom As Object, ByVal dtmDateTo As Object) As Integer
        Dim sqlCnn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand()

        With sqlCmd
            .Connection = sqlCnn
            .CommandText = "sp_EgswMenuItemUpdate"
            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
            .Parameters.Add("@vchCodeList", SqlDbType.VarChar, 5000).Value = strCodeList
            .Parameters.Add("@UseAverage", SqlDbType.Bit).Value = blnUseAverage
            .Parameters.Add("@dtmDateFrom", SqlDbType.DateTime).Value = IIf(dtmDateFrom Is Nothing, DBNull.Value, CDate(dtmDateFrom))
            .Parameters.Add("@dtmDateTo", SqlDbType.DateTime).Value = IIf(dtmDateTo Is Nothing, DBNull.Value, CDate(dtmDateTo))
            .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

            Try
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                Return CInt(.Parameters("@retval").Value)
            Catch ex As Exception
                Return -1
            End Try
        End With
    End Function


End Class

Imports System.Data.SqlClient
Imports System.Text

Public Class clsCMO
    Inherits clsDBRoutine

    Private l_strCnn As String

    Public Sub New(ByVal strCnn As String)
        l_strCnn = strCnn
    End Sub

    Public Function SetUserAction(ByVal intCodeEgs As Integer, ByVal intMnuType As Integer) As enumEgswErrorCode
        Dim sb As StringBuilder = New StringBuilder

        With sb
            .Append("UPDATE [User] ")
            .Append("SET MnuType=@MnuType ")
            .Append("WHERE EgsId=@EgsId")
        End With

        Dim sqlCmd As New SqlCommand
        With sqlCmd
            .Connection = New SqlConnection(l_strCnn)
            .CommandText = sb.ToString
            .CommandType = CommandType.Text
            .CommandTimeout = 3600
            .Parameters.Add("@EGSID", SqlDbType.Int).Value = intCodeEgs
            .Parameters.Add("@MnuType", SqlDbType.Int).Value = intMnuType

            Try
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                Return enumEgswErrorCode.OK
            Catch ex As Exception
                If .Connection.State <> ConnectionState.Closed Then .Connection.Close()
                Return enumEgswErrorCode.GeneralError
            End Try
        End With

    End Function

    Public Function SetUserPackage(ByVal intCodeEgs As Integer, ByVal intCMOnlineLogID As Integer) As enumEgswErrorCode
        Dim sb As StringBuilder = New StringBuilder

        With sb
            .Append("UPDATE [User] ")
            .Append("SET CMOnlineLogID=@CMOnlineLogID ")
            .Append("WHERE EgsId=@EgsId")
        End With

        Dim sqlCmd As New SqlCommand
        With sqlCmd
            .Connection = New SqlConnection(l_strCnn)
            .CommandText = sb.ToString
            .CommandType = CommandType.Text
            .CommandTimeout = 3600
            .Parameters.Add("@EGSID", SqlDbType.Int).Value = intCodeEgs
            .Parameters.Add("@CMOnlineLogID", SqlDbType.Int).Value = intCMOnlineLogID

            Try
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                Return enumEgswErrorCode.OK
            Catch ex As Exception
                If .Connection.State <> ConnectionState.Closed Then .Connection.Close()
                Return enumEgswErrorCode.GeneralError
            End Try
        End With

    End Function

    Public Function CreateUser(ByVal intEgsID As Integer, ByVal strCodeDicList As String, ByVal strPicturePath As String, ByVal intListeCodeDic As Integer, ByVal intSiteLang As Integer, ByVal intTimeZone As Integer, _
        ByVal strPaperSize As String, ByVal strPaperWeight As String, ByVal strPaperWidth As String, ByVal strMarginUnit As String, _
        ByVal strTM As String, ByVal strBM As String, ByVal strFont As String, ByVal strFontSize As String, ByVal strLM As String, ByVal strRM As String, ByVal strLS As String, ByVal intDefaultDBCode As Integer) As enumEgswErrorCode

        Return UpdateUser(True, intEgsID, strCodeDicList, strPicturePath, intListeCodeDic, intSiteLang, intTimeZone, _
                strPaperSize, strPaperWeight, strPaperWidth, strMarginUnit, _
                strTM, strBM, strFont, strFontSize, strLM, strRM, strLS, intDefaultDBCode)
    End Function

    Public Function ModifyUser(ByVal intEgsID As Integer, ByVal strCodeDicList As String, ByVal strPicturePath As String, ByVal intListeCodeDic As Integer, ByVal intSiteLang As Integer, ByVal intTimeZone As Integer) As enumEgswErrorCode
        Return UpdateUser(False, intEgsID, strCodeDicList, strPicturePath, intListeCodeDic, intSiteLang, intTimeZone, _
                        "", "", "", "", "", "", "", "", "", "", "", 0)
    End Function

    Private Function UpdateUser(ByVal blnAdd As Boolean, ByVal intEgsID As Integer, ByVal strCodeDicList As String, ByVal strPicturePath As String, ByVal intListeCodeDic As Integer, ByVal intSiteLang As Integer, ByVal intTimeZone As Integer, _
        ByVal strPaperSize As String, ByVal strPaperWeight As String, ByVal strPaperWidth As String, ByVal strMarginUnit As String, _
        ByVal strTM As String, ByVal strBM As String, ByVal strFont As String, ByVal strFontSize As String, ByVal strLM As String, ByVal strRM As String, ByVal strLS As String, ByVal intDefaultDBCode As Integer) As enumEgswErrorCode
        Dim sb As StringBuilder = New StringBuilder

        With sb
            If blnAdd Then
                .Append("DELETE FROM [User] WHERE EGSID=@EGSID ")
                .Append("INSERT INTO [User] ")
                .Append(" (EGSID, Status, DbaseName, CodeDicList, PicturePath, ListeCodeDic, SiteLang, TimeZone, PaperSize, PaperHeight, ")
                .Append(" PaperWidth, MarginUnit, TM, BM, Font, FontSize, LM, RM, LS, DefaultDBCode) ")
                .Append("VALUES ")
                .Append(" (@EGSID, @Status, '', @CodeDicList, @PicturePath, @ListeCodeDic, @SiteLang, @TimeZone, @PaperSize, @PaperHeight, ")
                .Append(" @PaperWidth, @MarginUnit, @TM, @BM, @Font, @FontSize, @LM, @RM, @LS, @DefaultDBCode) ")
            Else
                .Append("UPDATE [User] ")
                .Append("SET CodeDicList=@CodeDicList, PicturePath=@PicturePath, ")
                .Append("ListeCodeDic=@ListeCodeDic, SiteLang=@SiteLang, TimeZone=@TimeZone ")
                .Append("WHERE EGSID=@EGSID ")
            End If
        End With

        Dim sqlCmd As New SqlCommand
        With sqlCmd
            .Connection = New SqlConnection(l_strCnn)
            .CommandText = sb.ToString
            .CommandType = CommandType.Text
            .CommandTimeout = 3600
            .Parameters.Add("@EGSID", SqlDbType.Int).Value = intEgsID
            .Parameters.Add("@CodeDicList", SqlDbType.NVarChar, 50).Value = strCodeDicList
            .Parameters.Add("@PicturePath", SqlDbType.NVarChar, 50).Value = strPicturePath
            .Parameters.Add("@ListeCodeDic", SqlDbType.Int).Value = intListeCodeDic
            .Parameters.Add("@SiteLang", SqlDbType.Int).Value = intSiteLang
            .Parameters.Add("@TimeZone", SqlDbType.Int).Value = intTimeZone

            If blnAdd Then
                .Parameters.Add("@Status", SqlDbType.Bit).Value = DBNull.Value
                .Parameters.Add("@PaperSize", SqlDbType.NVarChar, 50).Value = strPaperSize
                .Parameters.Add("@PaperHeight", SqlDbType.NVarChar, 50).Value = strPaperWeight
                .Parameters.Add("@PaperWidth", SqlDbType.NVarChar, 50).Value = strPaperWidth
                .Parameters.Add("@MarginUnit", SqlDbType.NVarChar, 50).Value = strMarginUnit
                .Parameters.Add("@TM", SqlDbType.NVarChar, 50).Value = strTM
                .Parameters.Add("@BM", SqlDbType.NVarChar, 50).Value = strBM
                .Parameters.Add("@Font", SqlDbType.NVarChar, 50).Value = strFont
                .Parameters.Add("@FontSize", SqlDbType.NVarChar, 50).Value = strFontSize
                .Parameters.Add("@LM", SqlDbType.NVarChar, 50).Value = strLM
                .Parameters.Add("@RM", SqlDbType.NVarChar, 50).Value = strRM
                .Parameters.Add("@LS", SqlDbType.NVarChar, 50).Value = strLS
                .Parameters.Add("@DefaultDBCode", SqlDbType.Int).Value = intDefaultDBCode
            End If

            Try
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                Return enumEgswErrorCode.OK
            Catch ex As Exception
                If .Connection.State <> ConnectionState.Closed Then .Connection.Close()
                Return enumEgswErrorCode.GeneralError
            End Try
        End With
    End Function

    Public Function CreateDbase(ByVal intEgsID As Integer, ByVal intLanguage As Integer) As enumEgswErrorCode
        Dim sqlCmd As New SqlCommand
        With sqlCmd
            .Connection = New SqlConnection(l_strCnn)
            .CommandText = "sp_EGSRestoreData"
            .CommandType = CommandType.StoredProcedure
            .CommandTimeout = 3600
            .Parameters.Add("@EgsId", SqlDbType.Int).Value = intEgsID
            .Parameters.Add("@Language", SqlDbType.Int).Value = intLanguage
            '.Parameters.Add("@retval", SqlDbType.Int)
            '.Parameters("@retval").Direction = ParameterDirection.Output
            Try
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                Return enumEgswErrorCode.OK
                ' Return CType(.Parameters("@retval").Value, enumEgswErrorCode)
            Catch ex As Exception
                If .Connection.State <> ConnectionState.Closed Then .Connection.Close()
                Return enumEgswErrorCode.GeneralError
            End Try
        End With


    End Function

    Public Function GetUser(ByVal intEGSID As Integer) As SqlDataReader
        Dim sb As StringBuilder = New StringBuilder
        sb.Append("SELECT * FROM [User] WHERE EGSID=" & intEGSID)

        Dim dr As SqlDataReader
        Dim sqlCmd As New SqlCommand
        With sqlCmd
            .Connection = New SqlConnection(l_strCnn)
            .CommandText = sb.ToString
            .CommandType = CommandType.Text
            .CommandTimeout = 3600
            .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.Output
            Try
                .Connection.Open()
                dr = .ExecuteReader(CommandBehavior.CloseConnection)
                Return dr
            Catch ex As Exception
                If .Connection.State <> ConnectionState.Closed Then .Connection.Close()
                Return Nothing
            End Try
        End With
    End Function

    Public Function GetDefaultDbase() As DataTable
        Dim sb As StringBuilder = New StringBuilder
        sb.Append("SELECT * FROM DefaultDbase ORDER BY [Name]")

        Dim sqlCmd As New SqlCommand
        With sqlCmd
            .Connection = New SqlConnection(l_strCnn)
            .CommandText = sb.ToString
            .CommandType = CommandType.Text
            Try
                Dim ds As DataSet = New DataSet
                Dim sqlDa As SqlDataAdapter = New SqlDataAdapter
                sqlDa.SelectCommand = sqlCmd
                sqlDa.Fill(ds)
                Return ds.Tables(0)
            Catch ex As Exception
                If .Connection.State <> ConnectionState.Closed Then .Connection.Close()
                Return Nothing
            End Try
        End With

    End Function
End Class

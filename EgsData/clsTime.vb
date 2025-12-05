Imports System.Data
Imports System.Data.SqlClient
Public Class clsTime
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType
    ' RDC 04.12.2013 - CWM 5212
    Private L_ErrCode As enumEgswErrorCode

    Public Sub New(ByVal strCnn As String)
        L_strCnn = strCnn
        L_bytFetchType = enumEgswFetchType.DataTable
    End Sub

    Public Function GetTimebySite(intCodeSite As Integer, intCodeTrans As Integer, Optional strSearch As String = "", Optional intCodeProperty As Integer = -1) As DataTable
        Dim dtTime As New DataTable
        Dim da As New SqlDataAdapter
        Dim sCon As New SqlConnection(L_strCnn)
        Dim sCom As New SqlCommand()
        With sCom
            .Connection = sCon
            .CommandText = "sp_GetTimebySite"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@CodeSite", SqlDbType.Int, 4).Value = intCodeSite
            .Parameters.Add("@CodeTrans", SqlDbType.Int, 4).Value = intCodeTrans
            If strSearch <> "" Then
                .Parameters.Add("@Search", SqlDbType.NVarChar, 100).Value = strSearch
            End If
            .Parameters.Add("@CodeProperty", SqlDbType.Int).Value = intCodeProperty
            sCon.Open()
            da.SelectCommand = sCom
            dtTime.BeginLoadData()
            da.Fill(dtTime)
            dtTime.EndLoadData()
            sCon.Close()
            sCon.Dispose()
            sCom.Dispose()
        End With
        Return dtTime

    End Function

    Public Function GetTimeSiteDetails(intCodeSite As Integer, intCode As Integer, intCodeTrans As Integer) As DataSet

        Dim dsTime As New DataSet
        Dim da As New SqlDataAdapter
        Dim sCon As New SqlConnection(L_strCnn)
        Dim sCom As New SqlCommand()
        With sCom
            .Connection = sCon
            .CommandText = "sp_GetTimebySite"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@CodeSite", SqlDbType.Int, 4).Value = intCodeSite
            .Parameters.Add("@CodeTrans", SqlDbType.Int, 4).Value = intCodeTrans
            .Parameters.Add("@Code", SqlDbType.Int, 4).Value = intCode

            sCon.Open()
            da.SelectCommand = sCom
            da.Fill(dsTime)
            sCon.Close()
            sCon.Dispose()
            sCom.Dispose()
        End With
        Return dsTime

    End Function

    Public Function GetTimeSharing(intCode As Integer, intCodeTrans As Integer) As DataSet

        Dim dsTime As New DataSet
        Dim da As New SqlDataAdapter
        Dim sCon As New SqlConnection(L_strCnn)
        Dim sCom As New SqlCommand()
        With sCom
            .Connection = sCon
            .CommandText = "sp_GetTimebySharing"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@Code", SqlDbType.Int, 4).Value = intCode
            .Parameters.Add("@CodeTrans", SqlDbType.Int, 4).Value = intCodeTrans

            sCon.Open()
            da.SelectCommand = sCom
            da.Fill(dsTime)
            sCon.Close()
            sCon.Dispose()
            sCom.Dispose()
        End With
        Return dsTime

    End Function

    Public Function GetTimeName(intCode As Integer, intCodeTrans As Integer) As String
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader
        Dim strName As String = ""

        With cmd
            .Connection = cn
            .CommandText = "SELECT CASE ISNULL(b.Name,'') WHEN '' THEN a.Name ELSE b.Name END AS Name " & vbCrLf & _
                           "FROM           dbo.TimeType            AS a " & vbCrLf & _
                           "    LEFT JOIN  dbo.TimeTypeTranslation AS b ON a.ID            = b.CodeTimeType " & vbCrLf & _
                                                                      "AND b.CodeTrans In (@intCodeTrans, NULL) " & vbCrLf & _
                           "    INNER JOIN dbo.EgswSharing         AS c ON a.ID            = c.Code " & vbCrLf & _
                                                                      "AND c.CodeEgswTable = 153 " & vbCrLf & _
                                                                      "AND a.ID            = @intCode "
            .CommandType = CommandType.Text
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
            Try
                cn.Open()
                strName = CStr(.ExecuteScalar())
                'dr = .ExecuteReader(CommandBehavior.CloseConnection)
                'If dr.Read Then
                '    strName = CStrDB(dr.Item("Name"))
                'End If
                'dr.Close()
                cn.Close()
            Catch ex As Exception
                cn.Close()
                Throw ex
            End Try
        End With
        Return strName
    End Function

    Public Function UpdateTime(ByRef intCode As Integer, strName As String, intCodeTrans As Integer, blIsGlobal As Boolean, strSites As String) As Integer
        Dim intRetVal As Integer
        Dim sCon As New SqlConnection(L_strCnn)
        Dim sCom As New SqlCommand()
        With sCom
            .Connection = sCon
            .CommandText = "sp_UpdateTime"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@Code", SqlDbType.Int, 4).Value = intCode
            .Parameters("@Code").Direction = ParameterDirection.InputOutput
            .Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = strName
            .Parameters.Add("@CodeTrans", SqlDbType.Int, 4).Value = intCodeTrans
            .Parameters.Add("@IsGlobal", SqlDbType.Bit, 1).Value = blIsGlobal
            .Parameters.Add("@Site", SqlDbType.NVarChar, 100).Value = strSites
            .Parameters.Add("@RetVal", SqlDbType.Int, 4).Direction = ParameterDirection.ReturnValue
            .Connection.Open()
            .ExecuteNonQuery()
            If intCode = -2 Then
                intCode = CInt(.Parameters("@Code").Value)
            End If
            intRetVal = CInt(.Parameters("@RetVal").Value)
            .Connection.Close()
            sCon.Dispose()
            sCom.Dispose()
            Return intRetVal
        End With
    End Function

    Public Function UpdateTimeSharing(ByVal intCode As Integer, ByVal intCodeSite As Integer, _
                                     ByVal strCodeSharedTo As String, ByVal intCodeEgswTable As enumDbaseTables, blIsGLobal As Boolean) As Integer


        Dim cn As SqlConnection
        Dim cmd As SqlCommand = New SqlCommand

        Try
            With cmd
                cn = New SqlConnection(L_strCnn)
                cn.Open()
                .Connection = cn
                .CommandText = "DELETE FROM EgswSharing WHERE Code=" & intCode & " AND CodeUserOwner=" & intCodeSite & _
                               " AND CodeEgswTable=" & intCodeEgswTable & " AND Type=1"
                .CommandType = CommandType.Text
                .ExecuteNonQuery()
            End With
            cn.Close()
            cn.Dispose()
            cmd.Dispose()

            With cmd
                cn = New SqlConnection(L_strCnn)
                .Connection = cn
                .CommandText = "sp_EgswUpdateSharing"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int)
                .Parameters.Add("@intCodeSite", SqlDbType.Int)
                .Parameters.Add("@intCodeSitesShared", SqlDbType.Int)
                .Parameters.Add("@intCodeEgswTable", SqlDbType.Int)
                .Parameters.Add("@IsGlobal", SqlDbType.Bit)
                cn.Open()


                Dim arrCodeSites() As String
                If Not strCodeSharedTo = "-1" Then
                    strCodeSharedTo = strCodeSharedTo.Replace("(", "")
                    strCodeSharedTo = strCodeSharedTo.Replace(")", "")
                    arrCodeSites = strCodeSharedTo.Split(CChar(","))

                    For i As Integer = 0 To UBound(arrCodeSites)
                        If IsNumeric(arrCodeSites(i)) Then
                            .Parameters("@intCode").Value = intCode
                            .Parameters("@intCodeSite").Value = intCodeSite
                            .Parameters("@intCodeSitesShared").Value = arrCodeSites(i)
                            .Parameters("@intCodeEgswTable").Value = intCodeEgswTable
                            .Parameters("@IsGlobal").Value = blIsGLobal
                            .ExecuteNonQuery()
                        End If
                    Next
                Else
                    .Parameters("@intCode").Value = intCode
                    .Parameters("@intCodeSite").Value = intCodeSite
                    .Parameters("@intCodeSitesShared").Value = CInt(strCodeSharedTo)
                    .Parameters("@intCodeEgswTable").Value = intCodeEgswTable
                    .Parameters("@IsGlobal").Value = blIsGLobal
                    .ExecuteNonQuery()
                End If
            End With
            cn.Close()
            cn.Dispose()
            cmd.Dispose()
            Return 0
        Catch ex As Exception
            cn.Close()
            cmd.Dispose()
            Return -1
        End Try
    End Function

    Public Function DeleteTime(intCode As Integer) As Integer
        Dim cn As SqlConnection
        Dim intRetVal As Integer = 0
        Dim cmd As SqlCommand = New SqlCommand
        Dim sCon As New SqlConnection(L_strCnn)
        Dim sCom As New SqlCommand()
        With sCom
            .Connection = sCon
            .CommandText = "sp_DeleteTime"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@Code", SqlDbType.Int, 4).Value = intCode
            .Parameters.Add("@RetVal", SqlDbType.Int, 4).Direction = ParameterDirection.ReturnValue
            .Connection.Open()
            .ExecuteNonQuery()
            intRetVal = CInt(.Parameters("@RetVal").Value)
            .Connection.Close()
            sCon.Dispose()
            sCom.Dispose()
        End With
        Return intRetVal
    End Function

    Public Function UpdateTimeTranslation(intCode As Integer, intLangCode As Integer, strTimeTranslation As String) As Integer
        Dim cn As New SqlConnection(L_strCnn)
        Dim cm As SqlCommand
        Dim intRetVal As Integer = 0

        Try
            cn.Open()
            cm = New SqlCommand()
            With cm
                .Connection = cn
                .CommandType = CommandType.StoredProcedure
                .CommandText = "sp_UpdateTimeTranslation"
                .Parameters.Add("@intTimeCode", SqlDbType.Int, 4).Value = intCode
                .Parameters.Add("@vchTimeTypeName", SqlDbType.NVarChar, 150).Value = strTimeTranslation 'AGL 2013.06.27 - changed to nvarchar
                .Parameters.Add("@intCodeTrans", SqlDbType.Int, 4).Value = intLangCode
                .Parameters.Add("@intRetCode", SqlDbType.Int, 4).Value = 0
                .Parameters.Add("@intRetCode", SqlDbType.Int, 4).Direction = ParameterDirection.ReturnValue
                .ExecuteNonQuery()
                intRetVal = CInt(.Parameters("@intRetCode").Value)
                .Dispose()
            End With
            cn.Close()
            cn.Dispose()
        Catch sqlEx As SqlException
            MsgBox(sqlEx.ErrorCode & vbCrLf & _
                   sqlEx.Message, MsgBoxStyle.Critical, clsEGSLanguage.CodeType.Calcmenu)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, clsEGSLanguage.CodeType.ERR_PAGE_UNKNOWN)
        End Try

    End Function

    ' RDC 04.12.2013 - CWM-5212 Recipe Time Standardization
    Public Function StandardizeRecipeTime(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal eListeType As enumDataListType, _
                                       ByVal eItemListType As enumDataListType, ByVal eFormat As enumEgswStandardizationFormat) As enumEgswErrorCode

        Dim cmd As New SqlCommand

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswItemStandardizeAll"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@tntFormat", SqlDbType.TinyInt).Value = eFormat
                .Parameters.Add("@tntType", SqlDbType.TinyInt).Value = eListeType
                .Parameters.Add("@tntListType", SqlDbType.TinyInt).Value = eItemListType

                .Parameters("@retval").Direction = ParameterDirection.ReturnValue
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With

        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

    End Function

End Class

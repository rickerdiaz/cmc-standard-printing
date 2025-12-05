Imports System.Data.SqlClient
Imports System.Data
Public Class clsProject
#Region "Variable Declarations / Dependencies"
    Private L_ErrCode As enumEgswErrorCode
    Private L_intCodeSite As Int32 = -1
    Private L_RoleLevelHighest As Int16 = -1

    'Properties
    Private L_udtUser As structUser
    Private L_AppType As enumAppType
    Private L_dtList As DataTable
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType
    Private L_intCode As Int32
    Private L_strName As String
    Private L_IsGlobal As Boolean
#End Region


#Region "Private Methods"

    Private Function FetchList(ByVal lngCodeSite As Int32, ByVal lngCode As Int32, _
        ByVal lngCodeTrans As Int32, Optional ByVal strName As String = "", Optional ByVal intCode As Integer = -1, Optional ByVal intParent As Integer = -1, Optional ByVal blIsParent As Boolean = False, _
        Optional ByVal intCodeUser As Integer = -1) As Object
        'DRR 05.25.2012 added optional param - intCodeUser

        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader
        Dim cmd As New SqlCommand
        Dim lngCodeProperty As Int32 = -1

        If L_udtUser.RoleLevelHighest = 0 Then 'Get ALL items
            lngCodeProperty = -1
        ElseIf L_udtUser.RoleLevelHighest = 1 Then 'Get ALL items for a site
            lngCodeSite = L_udtUser.Site.Code
        ElseIf L_udtUser.RoleLevelHighest = 2 Then 'Get ALL items for a property
            lngCodeProperty = L_udtUser.Site.Group
        End If

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "GET_Project"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@Code", SqlDbType.Int, 4).Value = lngCode
                .Parameters.Add("@ParentCode", SqlDbType.Int, 4).Value = IIf(intParent = -1, DBNull.Value, intParent)
                .Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = IIf(strName.Trim() = "", DBNull.Value, strName)
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = lngCodeTrans
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                If blIsParent = True Then
                    .Parameters.Add("@IsParent", SqlDbType.Bit).Value = blIsParent
                End If
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser 'DRR 05.25.2012
                '.CommandTimeout = 60000
            End With

            If L_bytFetchType = enumEgswFetchType.DataReader Then
                cmd.Connection.Open()
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection)

            ElseIf L_bytFetchType = enumEgswFetchType.DataTable Then
                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With

            ElseIf L_bytFetchType = enumEgswFetchType.DataSet Then
                With da
                    .SelectCommand = cmd
                    'dt.BeginLoadData()
                    .Fill(ds, "ItemList")
                    'dt.EndLoadData()
                End With
            End If

        Catch ex As Exception
            dr = Nothing
            ds = Nothing
            dt.Dispose()
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        If L_bytFetchType = enumEgswFetchType.DataReader Then
            Return dr
        ElseIf L_bytFetchType = enumEgswFetchType.DataTable Then
            Return dt
        ElseIf L_bytFetchType = enumEgswFetchType.DataSet Then
            Return ds
        End If

        Return Nothing
    End Function

    Private Function RemoveFromList(ByVal lngCode As Integer, ByVal lngCodeUser As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Dim lngCodeProperty As Int32

        If L_udtUser.RoleLevelHighest = 0 Then 'Unshare to ALL 
            lngCodeProperty = -1
        Else 'Unshare to ALL sites belonging to a property or Unshare to self
            lngCodeProperty = L_udtUser.Site.Group
        End If

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "DELETE_Project"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@Code", SqlDbType.Int).Value = lngCode
                ''.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser

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
            'Throw New Exception(ex.Message, ex)
        End Try


        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()

       
        Return L_ErrCode

    End Function

    Private Function GetProjectChildListofParent(ByVal intParent As Integer, strParentCode As String) As ArrayList
        Dim arrChildList As New ArrayList
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim cmd As New SqlCommand
        Dim lngCodeProperty As Int32 = -1
        ''GET_ItemChildList
        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "GET_ItemChildList"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@ParentCode", SqlDbType.Int, 4).Value = intParent
                .Parameters.Add("@ColParentCode", SqlDbType.NVarChar, 1000).Value = strParentCode
                .Parameters.Add("@Type", SqlDbType.Int).Value = 1
            End With
            With da
                .SelectCommand = cmd
                dt.BeginLoadData()
                .Fill(dt)
                dt.EndLoadData()
            End With
        Catch ex As Exception
            dt.Dispose()
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
        If dt.Rows.Count > 0 Then
            For Each dr As DataRow In dt.Rows
                If arrChildList.Contains(dr("Code")) = False Then
                    arrChildList.Add(dr("Code"))
                End If
            Next
        End If
        Return arrChildList
    End Function



#End Region

#Region "Public"

    Public Sub GetProjectChildListe(intParentCode As Integer, ByVal strCollection As String, ByRef arrChildList As ArrayList)
        Dim arrTemp As ArrayList = GetProjectChildListofParent(intParentCode, strCollection)
        Dim blHasChild As Boolean = True
        strCollection = ""
        If arrTemp.Count > 0 Then
            For Each strId As String In arrTemp
                If arrChildList.Contains(strId) = False Then
                    arrChildList.Add(strId)
                    If strCollection = "" Then
                        strCollection += strId
                    Else
                        strCollection += ","
                        strCollection += strId
                    End If
                End If
            Next
        End If
        If strCollection <> "" Then
            GetProjectChildListe(-1, strCollection, arrChildList)
            Exit Sub
        End If
    End Sub

    Public Sub New(ByVal udtUser As structUser, ByVal eAppType As enumAppType, ByVal strCnn As String, _
       Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader, _
       Optional ByVal CreateRecord As Boolean = False)

        Try
            L_udtUser = udtUser
            L_AppType = eAppType
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn

        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try

    End Sub

    Public Sub New(strConn As String)
        L_strCnn = strConn
        L_bytFetchType = enumEgswFetchType.DataTable
    End Sub

    Public Overloads Function GetList(ByVal lngCode As Int32) As Object

        Return FetchList(-1, lngCode, enumDataListType.NoType, -1, 255)

    End Function

    Public Overloads Function Remove(ByVal intCode As Integer, ByVal intCodeUser As Integer) As enumEgswErrorCode

        Return RemoveFromList(intCode, intCodeUser)

    End Function

    Public Function GetListPerSite(ByVal nCodeSite As Integer, Optional ByVal lngTrans As Integer = -1, Optional ByVal nCodeUser As Integer = -1, Optional ByVal intClientCode As Integer = -1) As Object
        'DRR 05.25.2012 added optional param - nCodeUser
        'AMTLA 2013.11.18 Added optional param intClientCode 
        If intClientCode = 35 Then
            Return FetchList(nCodeSite, -1, lngTrans, , , , , nCodeUser) ''intCodeUser:=nCodeUser)
        Else
            Return FetchList(nCodeSite, -1, lngTrans, intCodeUser:=nCodeUser)
        End If

    End Function

    Public Function GetListPerNameandSite(ByVal nCodeSite As Integer, ByVal strName As String) As Object
        Return FetchList(nCodeSite, -1, enumDataListType.NoType, -1, 255, strName)
    End Function

    Public Function GetListByParent(ByVal nCodeSite As Integer, ByVal intParent As Integer, Optional ByVal lngTrans As Integer = -1, Optional strFilter As String = "", Optional ByVal nCodeUser As Integer = -1, Optional ByVal intClientCode As Integer = -1)
        'AMTLA 2013.11.18 Added optional param intClientCode 
        If intClientCode = 35 Then
            Return FetchList(nCodeSite, -1, lngTrans, strFilter, -1, intParent, , nCodeUser)
        Else
            Return FetchList(nCodeSite, -1, lngTrans, strFilter, -1, intParent)
        End If
    End Function


    Public Function GetProjectParentList(ByVal nCodeSite As Integer) As Object
        Return FetchList(nCodeSite, -1, -1, "", -1, -1, True)
    End Function
    Public Function SaveProject(ByRef intID As Integer, ByVal intParent As Integer, ByVal strName As String, ByVal blIsGlobal As Boolean, ByVal intCodeUser As Integer, ByVal dteDateUpdated As Date, ByVal intStatus As Integer, Optional ByVal strPicture As String = Nothing, Optional ByVal strCodeSiteList As String = "") As Integer
        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim cmd As SqlCommand = New SqlCommand
        Dim intReturn As Integer = 0
        Try
            With cmd
                cn.Open()
                .Connection = cn
                .CommandText = "INSERT_Project"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@Code", SqlDbType.Int, 4).Value = intID
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = blIsGlobal
                .Parameters.Add("@NAME", SqlDbType.NVarChar).Value = strName
                .Parameters.Add("@ParentCode", SqlDbType.Int).Value = IIf(IsNothing(intParent), DBNull.Value, intParent)
                .Parameters.Add("@DateUpdated", SqlDbType.SmallDateTime, 9).Value = dteDateUpdated
                .Parameters.Add("@intCreatedBy", SqlDbType.Int, 4).Value = intCodeUser
                .Parameters.Add("@ProjectStatus", SqlDbType.Int, 4).Value = intStatus
                .Parameters.Add("@vchPicture", SqlDbType.VarChar, 2000).Value = strPicture 'JTOC 24.05.2013
                .Parameters.Add("@strCodeSiteList", SqlDbType.VarChar, 2000).Value = strCodeSiteList    'JTOC 07.06.2013
                .Parameters("@Code").Direction = ParameterDirection.InputOutput
                cmd.Parameters.Add("@retval", SqlDbType.Int)
                cmd.Parameters("@retval").Direction = ParameterDirection.ReturnValue
                .ExecuteNonQuery()

                If intID = -2 Then
                    intID = CInt(.Parameters("@Code").Value)
                End If
                intReturn = CInt(.Parameters("@retval").Value)


            End With

            cn.Close()
            cn.Dispose()
            cmd.Dispose()

            If intReturn = 0 Then
                Return intID
            Else
                Return intReturn
            End If


        Catch ex As Exception
            cn.Close()
            cmd.Dispose()
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function
    Public Function SaveUserProject(ByVal dt As DataTable) As Integer
        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim cmd As SqlCommand = New SqlCommand
        Dim intReturn As Integer = 0

        For Each row As DataRow In dt.Rows
            Try
                With cmd
                    cn.Open()
                    .Connection = cn
                    .CommandText = "sp_EgswUpdateUserProject"
                    .CommandType = CommandType.StoredProcedure
                    .Parameters.Add("@CodeUser", SqlDbType.NVarChar, 4000).Value = row("CodeUser")
                    .Parameters.Add("@CodeProject", SqlDbType.Int).Value = row("CodeProject")
                    .ExecuteNonQuery()


                End With

                cn.Close()
                cn.Dispose()
                cmd.Dispose()

                intReturn = enumEgswErrorCode.OK
            Catch ex As Exception
                cn.Close()
                cmd.Dispose()
                Return enumEgswErrorCode.GeneralError
            End Try
        Next
    End Function

    'Public Function GetProjectbySharing(strCodeSites As String) As DataTable
    '	Dim cn As SqlConnection = New SqlConnection(L_strCnn)
    '	Dim cmd As SqlCommand = New SqlCommand
    '	Dim da As New SqlDataAdapter
    '	Dim dt As New DataTable
    '	With cmd
    '		cmd.Connection = cn
    '		cmd.CommandType = CommandType.StoredProcedure
    '		cmd.CommandText = "GetProjectbySharings"
    '		cmd.Parameters.Add("@CodeSites", SqlDbType.NVarChar, 100).Value = strCodeSites
    '	End With
    '	With da
    '		.SelectCommand = cmd
    '		dt.BeginLoadData()
    '		.Fill(dt)
    '		dt.EndLoadData()
    '	End With
    '	Return dt
    'End Function


    Public Function UpdateSharing(ByVal intCode As Integer, ByVal intCodeSite As Integer, _
                                     ByVal strCodeSharedTo As String, ByVal intCodeEgswTable As enumDbaseTables) As enumEgswErrorCode



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
                            .ExecuteNonQuery()
                        End If
                    Next
                Else
                    .Parameters("@intCode").Value = intCode
                    .Parameters("@intCodeSite").Value = intCodeSite
                    .Parameters("@intCodeSitesShared").Value = CInt(strCodeSharedTo)
                    .Parameters("@intCodeEgswTable").Value = intCodeEgswTable
                    .ExecuteNonQuery()
                End If
            End With
            cn.Close()
            cn.Dispose()
            cmd.Dispose()
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            cn.Close()
            cmd.Dispose()
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function ProcessRemoveProject(ByVal codeLang As Integer, ByVal intcodeUser As Integer, ByVal intCodeSite As Integer, _
                                           ByVal strID As String, ByVal strName As String, _
                                           ByRef strOK As String, ByRef strInUsed As String, Optional ByRef strNotDeleted As String = "NotDeleted") As String
        Dim eErrCode As enumEgswErrorCode
        eErrCode = Remove(CInt(strID), intcodeUser)
        If eErrCode <> enumEgswErrorCode.OK And eErrCode <> enumEgswErrorCode.OneItemNotDeleted And eErrCode <> enumEgswErrorCode.GeneralError Then
            Dim strMsg As String = ""
            Dim cNotes As clsNotes = New clsNotes(eErrCode, strMsg, codeLang)
            ProcessRemoveProject = strMsg
            Exit Function
        End If
        If eErrCode = enumEgswErrorCode.OneItemNotDeleted Then
            strInUsed += "-" & strName & vbCrLf
        ElseIf eErrCode = enumEgswErrorCode.GeneralError Then
            strNotDeleted += "-" & strName & vbCrLf
        Else
            strOK += "-" & strName & vbCrLf
        End If
        Return ""
    End Function

    ' RDC 03.14.2013 - CWM-3300 Cookbook Standardization
    Public Function StandardizeProject(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal eListeType As enumDataListType, _
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

#End Region
End Class

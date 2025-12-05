Imports System.Data.SqlClient
Imports System.Data
Public Class clsPlacement
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

    Public ReadOnly Property ItemsNotDeleted() As DataTable
        Get
            ItemsNotDeleted = L_dtList
        End Get
    End Property
#Region "Private Methods"

    Private Function FetchList(ByVal lngCodeSite As Int32, ByVal lngCode As Int32, ByVal eType As enumDataListType, _
        ByVal lngCodeTrans As Int32, ByVal bytStatus As Byte, Optional ByVal strName As String = "") As Object


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
                .CommandText = "GET_PlacementList"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = lngCodeTrans
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@Status", SqlDbType.TinyInt).Value = bytStatus
                If strName.Trim <> "" Then _
                    .Parameters.Add("@vchName", SqlDbType.NVarChar, 150).Value = strName
                .CommandTimeout = 60000
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

    Private Function FetchTranslationList(ByVal lngCodeTrans As Long) As DataTable

        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswBrandGetTranslationList"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = lngCodeTrans
            End With

            With da
                .SelectCommand = cmd
                dt.BeginLoadData()
                .Fill(dt)
                dt.EndLoadData()
            End With

        Catch ex As Exception
            cmd.Dispose()
            dt = Nothing
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return dt

    End Function

    Private Function RemoveFromList(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByVal lngCode As Int32, ByVal IsGlobal As Boolean, ByVal TranMode As enumEgswTransactionMode, _
        Optional ByVal bytStatus As Byte = 0, Optional ByVal strCodeList As String = "") As enumEgswErrorCode
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
                .CommandText = "DELETE_Placement"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = IsGlobal
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = lngCodeProperty
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                strCodeList.Trim()
                If strCodeList <> "" Then
                    If Not (strCodeList.StartsWith("(") And strCodeList.EndsWith(")")) Then
                        Return enumEgswErrorCode.InvalidCodeList
                    Else
                        .Parameters.Add("@vchCodeList", SqlDbType.VarChar, 8000).Value = strCodeList
                    End If
                End If

                If TranMode = enumEgswTransactionMode.ModifyStatus Then
                    .Parameters.Add("@bytStatus", SqlDbType.TinyInt).Value = bytStatus
                End If

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

        If L_ErrCode = enumEgswErrorCode.OneItemNotDeleted Then
            Dim da As New SqlDataAdapter

            Try
                cmd.CommandText = "sp_EgswItemGetNotDeleted"
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.Clear()
                cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                cmd.Parameters.Add("@vchTableName", SqlDbType.VarChar, 50).Value = "EgswBrand"

                L_dtList = New DataTable
                With da
                    .SelectCommand = cmd
                    L_dtList.BeginLoadData()
                    .Fill(L_dtList)
                    L_dtList.EndLoadData()
                End With
            Catch ex As Exception
                L_dtList.Dispose()
                If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
                cmd.Dispose()
                Throw New Exception(ex.Message, ex)
            End Try
        End If

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

    End Function

    '-- JBB 06.06.2012
    Private Function GetPlacementChildListofParent(ByVal intParent As Integer, strParentCode As String) As ArrayList
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
                .Parameters.Add("@Type", SqlDbType.Int).Value = 4
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

    Public Sub New(ByVal strCnn As String)
        L_strCnn = strCnn
        L_bytFetchType = enumEgswFetchType.DataTable
    End Sub


    Public Overloads Function GetList(ByVal lngCode As Int32) As Object

        Return FetchList(-1, lngCode, enumDataListType.NoType, -1, 255)

    End Function

    Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal strCodeList As String) As enumEgswErrorCode

        L_udtUser.Code = lngCodeUser
        Return RemoveFromList(lngCodeUser, lngCodeSite, 0, False, enumEgswTransactionMode.MultipleDelete, , strCodeList)

    End Function

    Public Function GetListPerSite(ByVal nCodeSite As Integer) As Object
        Return FetchList(nCodeSite, -1, enumDataListType.NoType, -1, 255)
    End Function

    Public Function GetListPerNameandSite(ByVal nCodeSite As Integer, ByVal strName As String) As Object
        Return FetchList(nCodeSite, -1, enumDataListType.NoType, -1, 255, strName)
    End Function

	Public Function SavePlacement(ByVal intID As Integer, ByVal intParent As Integer, ByVal strName As String, ByVal blIsGlobal As Boolean, Optional ByVal strCodeSiteList As String = "") As Integer
		Dim cn As SqlConnection = New SqlConnection(L_strCnn)
		Dim cmd As SqlCommand = New SqlCommand
		Try
			With cmd
				cn.Open()
				.Connection = cn
				.CommandText = "UPDATE_Placement"
				.CommandType = CommandType.StoredProcedure
				.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = blIsGlobal
				.Parameters.Add("@NAME", SqlDbType.NVarChar).Value = strName
				.Parameters.Add("@ParentPlacement", SqlDbType.Int).Value = IIf(IsNothing(intParent), DBNull.Value, intParent)
				.Parameters.Add("@strCodeSiteList", SqlDbType.NVarChar, 4000).Value = strCodeSiteList 'JTOC 07.06.2013

				.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.InputOutput
				.Parameters("@ID").Value = intID

				.ExecuteNonQuery()

				If intID = -2 Then
					intID = CInt(.Parameters("@ID").Value)
				'JTOC 22.05.2013 CWM-6088
				ElseIf .Parameters("@ID").Value IsNot Nothing AndAlso CInt(.Parameters("@ID").Value) = -3 Then
					intID = CInt(.Parameters("@ID").Value)
				End If
			End With

			cn.Close()
			cn.Dispose()
			cmd.Dispose()
			Return intID
		Catch ex As Exception
			cn.Close()
			cmd.Dispose()
			Return enumEgswErrorCode.GeneralError
		End Try
	End Function

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

    Public Function ProcessRemovePlacement(ByVal codeLang As Integer, ByVal intcodeUser As Integer, ByVal intCodeSite As Integer, _
                                           ByVal strID As String, ByVal strName As String, _
                                           ByRef strOK As String, ByRef strInUsed As String) As String
        Dim eErrCode As enumEgswErrorCode
        eErrCode = Remove(intcodeUser, intCodeSite, "(" + strID + ")")
        If eErrCode <> enumEgswErrorCode.OK And eErrCode <> enumEgswErrorCode.OneItemNotDeleted Then
            Dim strMsg As String = ""
            Dim cNotes As clsNotes = New clsNotes(eErrCode, strMsg, codeLang)
            ProcessRemovePlacement = strMsg
            Exit Function
        End If
        If eErrCode = enumEgswErrorCode.OneItemNotDeleted Then
            strInUsed += "-" & strName & vbCrLf
        Else
            strOK += "-" & strName & vbCrLf
        End If
    End Function


    '-- JBB 01.18.2012
    '-- Get Pacement Child
    Public Function GetPlacementbyparent(ByVal intCodeParent As Integer, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer)
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim cmd As New SqlCommand
        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[GET_PlacementListbyParent]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodePlacement", SqlDbType.Int).Value = intCodeParent
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite

                .CommandTimeout = 60000
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

        cmd.Dispose()
        Return dt
    End Function
    '--


    '-- JBB 06.06.2012
    Public Sub GetPlacementChildListe(intParentCode As Integer, ByVal strCollection As String, ByRef arrChildList As ArrayList)
        Dim arrTemp As ArrayList = GetPlacementChildListofParent(intParentCode, strCollection)
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
            GetPlacementChildListe(-1, strCollection, arrChildList)
            Exit Sub
        End If
    End Sub

    ' RDC 03.14.2013 - CWM-3300 Placement Standardization
    Public Function StandardizePlacement(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal eListeType As enumDataListType, _
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

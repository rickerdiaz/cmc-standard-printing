Imports System.Data.SqlClient
Imports System.Data

#Region "Class Header"
'Name               : clsKeyword
'Decription         : Manages Keyword Table
'Date Created       : 07.09.2005
'Author             : VBV
'Revision History   : VBV - 20.09.2005 - Accepts a connection string as opposed to a connection object. Class performs on a disconnected state.
'                     VBV - 23.09.2005 - Assign user upon creation of object, expose CodeUser
#End Region

''' <summary>
''' Manages Keyword Table
''' </summary>
''' <remarks></remarks>

Public Class clsKeyword
#Region "Variable Declarations / Dependencies"
    Inherits clsDBRoutine

    'Private L_Cnn As SqlConnection
    'Private cmd As New SqlCommand
    Private L_ErrCode As enumEgswErrorCode
    Private L_lngCodeSite As Int32 = -1
    Private L_RoleLevelHighest As Int16 = -1

    'Properties
    Private L_udtUser As structUser
    Private L_AppType As enumAppType
    Private L_dtList As DataTable
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType
    Private L_lngCode As Int32
#End Region


#Region "Class Functions and Properties"
    Public Sub New(ByVal udtUser As structUser, ByVal eAppType As enumAppType, ByVal strCnn As String, _
        Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)

        Try
            'If eAppType = enumAppType.SmartClient Then
            '    If objCnn Is Nothing Then
            '        L_Cnn = New SqlConnection
            '        L_Cnn.ConnectionString = strCnn
            '        L_Cnn.Open()
            '    ElseIf objCnn.State = ConnectionState.Closed Then
            '        objCnn.Open()
            '        L_Cnn = objCnn
            '    Else
            '        L_Cnn = objCnn
            '    End If
            '    L_strCnn = L_Cnn.ConnectionString
            'End If            
            If udtUser.Code <= 0 Then Throw New Exception("Invalid CodeUser.")
            L_udtUser = udtUser
            L_AppType = eAppType
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn
        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try

    End Sub

    Protected Overrides Sub Finalize()
        ClearMarkings(enumDataListItemType.Menu) 'items marked as not deleted
        ClearMarkings(enumDataListItemType.Recipe) 'items marked as not deleted
        ClearMarkings(enumDataListItemType.Merchandise) 'items marked as not deleted
        MyBase.Finalize()
    End Sub

    Public ReadOnly Property AppType() As enumAppType
        Get
            AppType = L_AppType
        End Get
    End Property

    Public ReadOnly Property ItemsNotDeleted() As DataTable
        Get
            ItemsNotDeleted = L_dtList
        End Get
    End Property

    Public ReadOnly Property ConnectionString() As String
        Get
            ConnectionString = L_strCnn
        End Get
    End Property

    'Public Property CodeUser() As Int32
    '    Get
    '        CodeUser = l_udtuser.code
    '    End Get
    '    Set(ByVal value As Int32)
    '        l_udtuser.code = value
    '    End Set
    'End Property

    'Public Property CodeSite() As Int32
    '    Get
    '        CodeSite = L_lngCodeSite
    '    End Get
    '    Set(ByVal value As Int32)
    '        L_lngCodeSite = value
    '    End Set
    'End Property

#End Region

#Region "Private Methods"

    Private Function FetchKeywordList(ByVal lngCodeSite As Int32, ByVal lngCode As Int32, ByVal eType As enumDataListItemType, _
        ByVal lngCodeTrans As Int32, ByVal bytStatus As Byte, Optional ByVal strName As String = "") As Object

        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader = Nothing
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
                .CommandText = "sp_EgswKeywordGetList"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@tntType", SqlDbType.TinyInt).Value = eType
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = lngCodeTrans
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@Status", SqlDbType.TinyInt).Value = bytStatus
                If strName.Trim <> "" Then _
                    .Parameters.Add("@vchName", SqlDbType.NVarChar, 200).Value = strName
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

    Private Function FetchKeywordTranslationList(ByVal lngCodeTrans As Long) As DataTable

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
                .CommandText = "sp_EgswKeywordGetTranslationList"
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
            dt.Dispose()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return dt

    End Function

	Public Function KeywordsHasChildrenOrBeingUsed(ByVal intCode As Integer) As Boolean
		Dim cmd As New SqlCommand

		With cmd

				.Connection = New SqlConnection(L_strCnn)
				.CommandText = "sp_KeywordsHasChildrenOrBeingUsed"
				.CommandTimeout = 60 * 10
				.CommandType = CommandType.StoredProcedure
				.Parameters.Add("@retval", SqlDbType.Bit)
				.Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
				.Parameters("@retval").Direction = ParameterDirection.ReturnValue

				.Connection.Open()
				.ExecuteNonQuery()
				.Connection.Close()
				L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)

			End With

	End Function

	Private Function SaveKeywordIntoList(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
		ByRef lngCode As Int32, ByVal udtKeyword As structKeyword, ByVal strCodeSiteList As String, _
		ByVal strCodeKeywordList As String, ByVal TranMode As enumEgswTransactionMode, _
		Optional ByVal oTransaction As SqlTransaction = Nothing, Optional ByVal strPicture As String = Nothing) As enumEgswErrorCode

		Dim cmd As New SqlCommand

		Try
			With cmd
				'If L_AppType = enumAppType.WebApp Then
				'    .Connection = New SqlConnection(GetConnection("dsn"))
				'Else
				'    .Connection = L_Cnn
				'End If
				If oTransaction Is Nothing Then
					.Connection = New SqlConnection(L_strCnn)
				Else
					.Connection = oTransaction.Connection
					.Transaction = oTransaction
				End If
				.CommandText = "sp_EgswKeywordUpdate"
				.CommandTimeout = 60 * 10
				.CommandType = CommandType.StoredProcedure
				.Parameters.Add("@retval", SqlDbType.Int)
				.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
				.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
				.Parameters.Add("@intCode", SqlDbType.Int).Value = udtKeyword.Code
				.Parameters.Add("@nvcName", SqlDbType.NVarChar, 200).Value = udtKeyword.Name ' JBB 09.01.2011 (Change the max number from 25 to 200 ) '.Parameters.Add("@nvcName", SqlDbType.NVarChar, 25).Value = udtKeyword.Name
				.Parameters.Add("@tntType", SqlDbType.TinyInt).Value = udtKeyword.Type
				.Parameters.Add("@intParent", SqlDbType.Int).Value = udtKeyword.Parent
				.Parameters.Add("@IsInheritable", SqlDbType.Bit).Value = udtKeyword.Inheritable
				.Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = udtKeyword.IsGlobal
				.Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
				.Parameters.Add("@vchPicture", SqlDbType.VarChar, 2000).Value = strPicture

				.Parameters("@intCode").Direction = ParameterDirection.InputOutput
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

				strCodeSiteList.Trim()
				If strCodeSiteList <> "" Then
					If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) Then
						Return enumEgswErrorCode.InvalidCodeList
					Else
						.Parameters.Add("@vchCodeSiteList", SqlDbType.VarChar, 8000).Value = strCodeSiteList
					End If
				End If

				strCodeKeywordList.Trim()
				If strCodeKeywordList <> "" Then
					If Not (strCodeKeywordList.StartsWith("(") And strCodeKeywordList.EndsWith(")")) Then
						Return enumEgswErrorCode.InvalidCodeList
					Else
						.Parameters.Add("@vchCodeMergeList", SqlDbType.VarChar, 8000).Value = strCodeKeywordList
					End If
                End If

                .Parameters("@retval").Direction = ParameterDirection.ReturnValue
				If oTransaction Is Nothing Then .Connection.Open()
                .ExecuteNonQuery()
                If oTransaction Is Nothing Then .Connection.Close()
                
				L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
				lngCode = CInt(.Parameters("@intCode").Value)
			End With

			If L_ErrCode = enumEgswErrorCode.OK Then
				strCodeKeywordList.Trim()
				If strCodeKeywordList <> "" And (TranMode = enumEgswTransactionMode.MergeDelete Or TranMode = enumEgswTransactionMode.MergeHide) Then
					If Not (strCodeKeywordList.StartsWith("(") And strCodeKeywordList.EndsWith(")")) Then
						Return enumEgswErrorCode.InvalidCodeList
					Else

						cmd = New SqlCommand
						With cmd
							.Connection = New SqlConnection(L_strCnn)
							.CommandText = "INSERT INTO EgswKeyDetails (CodeListe, CodeKey, Derived)" & _
							"( SELECT DISTINCT [CodeListe], " & lngCode & _
							", Derived FROM EgswKeyDetails " & _
							"WHERE CodeKey IN " & strCodeKeywordList & _
							")"
							.CommandTimeout = 60 * 10
							.CommandType = CommandType.Text

							.Connection.Open()
							.ExecuteNonQuery()
							.Connection.Close()

						End With
					End If
				End If
			End If
		Catch ex As Exception
			L_ErrCode = enumEgswErrorCode.GeneralError
			If oTransaction Is Nothing AndAlso cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
			cmd.Dispose()
			Throw New Exception(ex.Message, ex)
		End Try

		If oTransaction Is Nothing AndAlso cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
		cmd.Dispose()
		Return L_ErrCode

	End Function

    Private Function RemoveFromList(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByVal lngCode As Int32, ByVal IsGlobal As Boolean, ByVal TranMode As enumEgswTransactionMode, _
        ByVal dataListeItemType As enumDataListItemType, Optional ByVal bytStatus As Byte = 0, _
        Optional ByVal strCodeList As String = "", Optional ByVal blnForceDelete As Boolean = False) As enumEgswErrorCode

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
                .CommandText = "sp_EgswKeywordDelete"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = IsGlobal
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = lngCodeProperty
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                .Parameters.Add("@tntType", SqlDbType.TinyInt).Value = dataListeItemType
                .Parameters.Add("@IsForceDelete", SqlDbType.Bit).Value = blnForceDelete

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
                cmd.Parameters.Add("@vchTableName", SqlDbType.VarChar, 50).Value = "EgswKeyword"

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

    Private Function ClearMarkings(ByVal dataListItemType As enumDataListItemType) As enumEgswErrorCode
        'Deactivate items that were not deleted by the Delete module
        If L_udtUser.Code <> -1 And L_lngCodeSite <> -1 Then
            Return (RemoveFromList(L_udtUser.Code, L_lngCodeSite, 0, False, enumEgswTransactionMode.Deactivate, dataListItemType))
        End If
    End Function

    ''-- JBB 06.05.2012
    Private Function GetKeywordChildListofParent(ByVal intParent As Integer, strParentCode As String) As ArrayList
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
                .Parameters.Add("@Type", SqlDbType.Int).Value = 2
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

#Region "Get Methods"
    ''' <summary>
    ''' Get all Keywords.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList() As Object

        Return FetchKeywordList(-1, -1, enumDataListItemType.NoType, -1, 255)

    End Function

    ''' <summary>
    ''' Get a Keyword by Code.
    ''' </summary>
    ''' <param name="lngCode">The Code of the Keyword to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal lngCode As Int32) As Object

        Return FetchKeywordList(-1, lngCode, enumDataListItemType.NoType, -1, 255)

    End Function

    ''' <summary>
    ''' Get all Keywords by Type.
    ''' </summary>
    ''' <param name="eType">One of the enumDataListType values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal eType As enumDataListItemType) As Object

        Return FetchKeywordList(-1, -1, eType, -1, 255)

    End Function

    ''' <summary>
    ''' Get all Keywords by Status.
    ''' </summary>
    ''' <param name="bytStatus">The status of the Keywords to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal bytStatus As Byte) As Object
        'Get all by Status
        Return FetchKeywordList(-1, -1, enumDataListItemType.NoType, -1, bytStatus)

    End Function

    ''' <summary>
    ''' Get all Keywords with the list of Site names to which they are shared to.
    ''' </summary>
    ''' <param name="eType">One of the enumDataListType values.</param>
    ''' <param name="lngCodeTrans">The Code of the language translation.</param>
    ''' <param name="bytStatus">The status of the Keywords to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal eType As enumDataListItemType, ByVal lngCodeTrans As Int32, ByVal bytStatus As Byte) As Object

        Return FetchKeywordList(-1, -1, eType, lngCodeTrans, bytStatus)

    End Function

    ''' <summary>
    ''' Get all Keywords shared to a specific site.
    ''' </summary>
    ''' <param name="eType">One of the enumDataListType values.</param>
    ''' <param name="lngCodeTrans">The Code of the language translation.</param>
    ''' <param name="bytStatus">The status of the Keyword to be fetched.</param>
    ''' <param name="lngCodeSite">The site to which the Keyword is shared.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal eType As enumDataListItemType, ByVal lngCodeTrans As Int32, ByVal bytStatus As Byte, ByVal lngCodeSite As Int32) As Object

        Return FetchKeywordList(lngCodeSite, -1, eType, lngCodeTrans, bytStatus)

    End Function

    ''' <summary>
    ''' Get all Translations.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTranslationList() As DataTable
        'Get all
        Return FetchKeywordTranslationList(-1)

    End Function

    'modified by ADR 05.10.11 - Added functionality to get derived mdse keyword for recipe
    'modified by ADR 05.10.11 - Added optional parameter IncludeInheritable
    Public Function GetListKeywordCodeName(ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, ByVal type As enumDataListItemType, Optional ByVal flagActiveOnly As Boolean = True, Optional ByVal IncludeInheritable As Boolean = False) As Object
        Dim strCommandText As String = "[GET_KEYWORDCODENAME]"

        '@ListeType int,
        '@CodeSite int,
        '@CodeTrans int,
        '@ActiveOnly bit =1

        If IncludeInheritable = false Then
            Dim arrParam(3) As SqlParameter
            arrParam(0) = New SqlParameter("@ListeType", type)
            arrParam(1) = New SqlParameter("@CodeSite", intCodeSite)
            arrParam(2) = New SqlParameter("@CodeTrans", L_udtUser.CodeTrans)
            arrParam(3) = New SqlParameter("@ActiveOnly", flagActiveOnly)


            Try
                Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
            Catch ex As Exception
                Throw ex
            End Try

        Else

            'Added by ADR 05.10.11 - functionality for derived mdse keyword for recipe

            Dim arrParam2(4) As SqlParameter
            arrParam2(0) = New SqlParameter("@ListeType", type)
            arrParam2(1) = New SqlParameter("@CodeSite", intCodeSite)
            arrParam2(2) = New SqlParameter("@CodeTrans", L_udtUser.CodeTrans)
            arrParam2(3) = New SqlParameter("@ActiveOnly", flagActiveOnly)
            arrParam2(4) = New SqlParameter("@IncludeInheritable", IncludeInheritable)

            Try
                Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam2)
            Catch ex As Exception
                Throw ex
            End Try
        End If

    End Function

    Public Function GetListKeywordBasic(ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, ByVal type As enumDataListItemType, ByVal intActive As Integer, Optional ByVal flagParentOrder As Boolean = False, Optional ByVal intExcludeKey As Integer = 0) As Object
        Dim strCommandText As String = "[GET_KEYWORDLIST]"

        '@ListeType int,
        '@CodeSite int,
        '@CodeTrans int,
        '@ActiveOnly bit =1

        Dim arrParam(5) As SqlParameter
        arrParam(0) = New SqlParameter("@ListeType", type)
        arrParam(1) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(2) = New SqlParameter("@CodeTrans", intCodeTrans)
        arrParam(3) = New SqlParameter("@ActiveOnly", intActive)
        arrParam(4) = New SqlParameter("@ParentOrder", flagParentOrder)
        arrParam(5) = New SqlParameter("@ExcludeKey", intExcludeKey)

        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function


	Public Function GetKeywordBySharing(ByVal strCodeSite As String, ByVal intCodeTrans As Integer, ByVal type As enumDataListItemType) As DataTable
		Dim strCommandText As String = "[GET_KeywordsbySharing]"

		'@ListeType int,
		'@CodeSite int,
		'@CodeTrans int,
		'@ActiveOnly bit =1

		Dim arrParam(2) As SqlParameter
		arrParam(0) = New SqlParameter("@ListeType", type)
		arrParam(1) = New SqlParameter("@nCodeSites", strCodeSite)
		arrParam(2) = New SqlParameter("@CodeTrans", intCodeTrans)

		Try
			Return ExecuteFetchType(enumEgswFetchType.DataTable, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
		Catch ex As Exception
			Throw ex
		End Try
	End Function

    Public Function USAGetClassifications(Optional ByVal intParentID As Integer = -1) As Object
        Dim strCommandText As String = "[GetClassifications]"

        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@ParentID", intParentID)
        
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''' <summary>
    ''' Get a specific translation.
    ''' </summary>
    ''' <param name="lngCodeTrans">The Code of the language translation.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTranslationList(ByVal lngCodeTrans As Long) As DataTable
        'Filter by CodeTrans
        Return FetchKeywordTranslationList(lngCodeTrans)

    End Function

    ''' <summary>
    ''' Get a Category by Name w/in the codesite.
    ''' </summary>
    ''' <param name="strName">The name of the Category to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal strName As String, ByVal eDataListeType As enumDataListItemType, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, ByVal bytStatus As Byte) As Object

        Return FetchKeywordList(intCodeSite, -1, eDataListeType, intCodeTrans, bytStatus, strName)

    End Function

    '--- VRP 10.03.2008
    Public Function GetKeyword(ByVal udtUser As structUser, ByVal listetype As enumDataListItemType) As SqlDataReader

        Dim cmd As New SqlCommand
        Dim lngCodeProperty As Int32 = -1

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandText = "SELECT distinct EgsWKeyword.Code,EgsWKeyword.Name " & vbCrLf & _
                                "FROM EgsWKeyword " & vbCrLf & _
                                "INNER JOIN EgsWKeyDetails ON EgsWKeyword.Code=EgsWKeyDetails.CodeKey " & vbCrLf & _
                                "INNER JOIN EgsWListe ON EgsWListe.Code = EgsWKeyDetails.CodeListe " & vbCrLf & _
                                "WHERE EgsWKeyword.Type =" & listetype & " " & vbCrLf & _
                                "AND EgsWKeyword.Name <>'' " & vbCrLf & _
                                "AND EgsWListe.Type =" & listetype & " " & vbCrLf & _
                                "AND CodeListe IN " & vbCrLf & _
                                "(SELECT distinct Code " & vbCrLf & _
                                "FROM EgsWSharing WHERE CodeEGSWTable=50 AND (CodeUserSharedTo=" & udtUser.Site.Code & " AND Type in (1,5)) OR  (CodeUserSharedTo=" & udtUser.Code & " AND Type in (3,8,7)) OR  (CodeUserSharedTo=(SELECT [Group] FROM EgsWSite WHERE Code=" & udtUser.Site.Code & ") AND Type in (2,6)) OR  Type in (9,10)) " & vbCrLf & _
                                "ORDER By EgsWKeyword.Name " & vbCrLf
                .CommandType = CommandType.Text
                Dim dr As SqlDataReader
                dr = .ExecuteReader()
                Return dr
            End With
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
            Return Nothing
        End Try
    End Function '------

    Public Function GetKeywordBrandSite(ByVal strParentName As String) As Object
        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@ParentName", SqlDbType.VarChar, 100)
        arrParam(0).Value = strParentName
        Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "Get_KeywordBrandSite", arrParam)
    End Function


    ''-- JBB 06.05.2012
    Public Sub GetKeywordsChildListe(intParentCode As Integer, ByVal strCollection As String, ByRef arrChildList As ArrayList)
        Dim arrTemp As ArrayList = GetKeywordChildListofParent(intParentCode, strCollection)
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
            GetKeywordsChildListe(-1, strCollection, arrChildList)
            Exit Sub
        End If
    End Sub



#End Region

#Region "Update Methods"
    ''' <summary>
    ''' Standardize Keywords
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="eListeType">One of the enumDataListType values.</param>
    ''' <param name="eItemListType">One of the enumDataListType values.</param>
    ''' <param name="eFormat">One of the enumEgswStandardizationFormat values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Standardize(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal eListeType As enumDataListType, _
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

    ''' <summary>
    ''' Updates the global status of a Keyword.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the Keyword to be updated.</param>
    ''' <param name="IsGlobal">The global status of the Keyword to be updated.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateGlobalStatus(ByVal lngCodeUser As Int32, ByVal lngCode As Int32, ByVal IsGlobal As Boolean) As enumEgswErrorCode

        Dim cmd As New SqlCommand

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswKeywordUpdateGlobal"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = IsGlobal
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser

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

    ''' <summary>
    ''' Updates Keyword without sharing it to any sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the Keyword to be updated.</param>
    ''' <param name="udtKeyword">One of the structKeyword values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByRef lngCode As Int32, ByVal udtKeyword As structKeyword) As enumEgswErrorCode

        Return SaveKeywordIntoList(lngCodeUser, lngCodeSite, lngCode, udtKeyword, "", "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    ''' <summary>
    ''' Updates Keyword sharing it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtKeyword">One of the structKeyword values.</param>
    ''' <param name="strCodeSiteList">The list of sites where Keyword will be shared.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtKeyword As structKeyword, ByVal strCodeSiteList As String) As enumEgswErrorCode

        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) And strCodeSiteList.Trim <> "" Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        Return SaveKeywordIntoList(lngCodeUser, lngCodeSite, lngCode, udtKeyword, strCodeSiteList, "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    ''' <summary>
    ''' Updates Keyword and its Translations and share it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtKeyword">One of the structKeyword values.</param>
    ''' <param name="strCodeSiteList">The list of sites where Keyword will be shared.</param>
    ''' <param name="strTransCodeList">The list of Translation Codes of the Keyword.</param>
    ''' <param name="strTransNameList">The list of Translation Names of the Keyword.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtKeyword As structKeyword, ByVal strCodeSiteList As String, _
        ByVal arrTransCode() As String, ByVal arrTransName() As String, Optional ByVal strPicture As String = Nothing) As enumEgswErrorCode


        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) And strCodeSiteList.Trim <> "" Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        Dim t As SqlTransaction
        Dim cn As New SqlConnection(L_strCnn)

        cn.Open()
        t = cn.BeginTransaction()
        L_ErrCode = SaveKeywordIntoList(lngCodeUser, lngCodeSite, lngCode, udtKeyword, strCodeSiteList, "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode), _
             t, strPicture)

        If L_ErrCode = enumEgswErrorCode.OK Then
            Try
                'Update Translations
                ' Dim arrTransCode() As String = strTransCodeList.Split(CChar(","))
                ' Dim arrTransName() As String = strTransNameList.Split(CChar(","))
                Dim c As Int32 = arrTransCode.Length - 1
                Dim i As Int32
                Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

                For i = 0 To c
                    If IsNumeric(arrTransCode(i)) Then
                        L_ErrCode = oTrans.UpdateTranslation(lngCode, arrTransName(i), CInt(arrTransCode(i)), udtKeyword.Type, lngCodeSite, lngCodeUser, enumDataListType.Keyword)
                        If L_ErrCode <> enumEgswErrorCode.OK Then Exit For
                    End If
                Next

            Catch ex As Exception
                L_ErrCode = enumEgswErrorCode.GeneralError
            End Try
        End If

        If L_ErrCode = enumEgswErrorCode.OK Then
            t.Commit()
        Else
            t.Rollback()
        End If
        t.Dispose()
        If cn.State <> ConnectionState.Closed Then cn.Close()
        cn.Dispose()
        Return L_ErrCode

    End Function

    ''' <summary>
    ''' Updates Keyword and its Translations and share it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtKeyword">One of the structKeyword values.</param>
    ''' <param name="strCodeSiteList">The list of sites where Keyword will be shared.</param>
    ''' <param name="dtTranslations">The list of Translations of the Keyword.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtKeyword As structKeyword, _
        ByVal strCodeSiteList As String, ByVal dtTranslations As DataTable) As enumEgswErrorCode

        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        Dim t As SqlTransaction
        Dim cn As New SqlConnection(L_strCnn)

        cn.Open()
        t = cn.BeginTransaction()
        L_ErrCode = SaveKeywordIntoList(lngCodeUser, lngCodeSite, lngCode, udtKeyword, strCodeSiteList, "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode), _
             t)

        If L_ErrCode > 0 Then
            Try
                Dim rowX As DataRow
                Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

                For Each rowX In dtTranslations.Rows
                    L_ErrCode = oTrans.UpdateTranslation(lngCode, rowX.Item("Name").ToString, CInt(rowX.Item("CodeTrans")), udtKeyword.Type, lngCodeSite, lngCodeUser, enumDataListType.Keyword)
                    If L_ErrCode <> enumEgswErrorCode.OK Then Exit For
                Next

            Catch ex As Exception
                L_ErrCode = enumEgswErrorCode.GeneralError
            End Try
        End If

        If L_ErrCode = enumEgswErrorCode.OK Then
            t.Commit()
        Else
            t.Rollback()
        End If
        t.Dispose()
        If cn.State <> ConnectionState.Closed Then cn.Close()
        cn.Dispose()
        Return L_ErrCode

    End Function

    ''' <summary>
    ''' Updatess Keyword's translations (multiple update)
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="eListeType">One of the enumDataListType values.</param>
    ''' <param name="dtTranslations">The list of Translations of the Keyword.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal eListeType As enumDataListType, ByVal dtTranslations As DataTable) As enumEgswErrorCode

        Dim t As SqlTransaction
        Dim cn As New SqlConnection(L_strCnn)

        cn.Open()
        t = cn.BeginTransaction()
        Try
            Dim rowX As DataRow
            Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

            For Each rowX In dtTranslations.Rows
                L_ErrCode = oTrans.UpdateTranslation(lngCode, rowX.Item("Name").ToString, CInt(rowX.Item("CodeTrans")), eListeType, lngCodeSite, lngCodeUser, enumDataListType.Keyword)
                If L_ErrCode <> enumEgswErrorCode.OK Then Exit For
            Next

            oTrans = Nothing
        Catch
            L_ErrCode = enumEgswErrorCode.GeneralError
        End Try

        If L_ErrCode = enumEgswErrorCode.OK Then
            t.Commit()
        Else
            t.Rollback()
        End If
        t.Dispose()
        If cn.State <> ConnectionState.Closed Then cn.Close()
        cn.Dispose()
        Return L_ErrCode

    End Function

    ''' <summary>
    ''' Updates a Keyword's translation (single update)
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="eListeType">One of the enumDataListType values.</param>
    ''' <param name="lngCodeTrans">The code of the Keyword's translation.</param>
    ''' <param name="strNameTrans">The name of the Keyword's translation.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, ByVal eListeType As enumDataListType, _
        ByVal lngCodeTrans As Int32, ByVal strNameTrans As String) As enumEgswErrorCode

        Dim t As SqlTransaction
        Dim cn As New SqlConnection(L_strCnn)

        cn.Open()
        t = cn.BeginTransaction()
        Try
            Dim oTrans As New clsTranslation(L_AppType, L_strCnn)
            L_ErrCode = oTrans.UpdateTranslation(lngCode, strNameTrans, lngCodeTrans, eListeType, lngCodeSite, lngCodeUser, enumDataListType.Keyword)

            oTrans = Nothing
        Catch
            L_ErrCode = enumEgswErrorCode.GeneralError
        End Try

        If L_ErrCode = enumEgswErrorCode.OK Then
            t.Commit()
        Else
            t.Rollback()
        End If
        t.Dispose()
        If cn.State <> ConnectionState.Closed Then cn.Close()
        cn.Dispose()
        Return L_ErrCode

    End Function

    ''' <summary>
    ''' Merge Keywords
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="strCodeKeywordList">The list of Keyword Codes to be merged.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal strCodeKeywordList As String, ByVal udtKeyword As structKeyword) As enumEgswErrorCode
        Return SaveKeywordIntoList(lngCodeUser, lngCodeSite, 0, udtKeyword, "", strCodeKeywordList, enumEgswTransactionMode.MergeDelete)
    End Function

    ''' <summary>
    ''' Updates Status of the Keywords specified in the list (strCodeList).
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="strCodeList">The list of Keyword Codes to be updated.</param>
    ''' <param name="bytStatus">The Status of the Keyword.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal strCodeList As String, ByVal bytStatus As Byte, ByVal dataListItemType As enumDataListItemType) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, -1, -1, False, enumEgswTransactionMode.Deactivate, dataListItemType, bytStatus, strCodeList)

    End Function

    ''' <summary>
    ''' Updates Status of a Keyword.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the Keyword to be updated.</param>
    ''' <param name="IsGlobal">The Global Status of the Keyword.</param>
    ''' <param name="bytStatus">The Status of the Keyword.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCode As Int32, ByVal IsGlobal As Boolean, _
        ByVal bytStatus As Byte, ByVal dataListItemType As enumDataListItemType) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, -1, lngCode, IsGlobal, enumEgswTransactionMode.Deactivate, dataListItemType, bytStatus)

    End Function

    ''' <summary>
    ''' Updates the position of items.
    ''' </summary>
    ''' <param name="strCodeList">The list of item codes to be moved.</param>
    ''' <param name="flagMoveUp"></param>
    ''' <param name="lngCodeSite">The CodeSite of the items to be moved.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdatePosition(ByVal strCodeList As String, ByVal flagMoveUp As Boolean, _
        ByVal lngCodeSite As Int32, ByVal eListeType As enumDataListItemType) As enumEgswErrorCode

        Dim cmd As New SqlCommand

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswKeywordMovePos"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@txtCodeList", SqlDbType.VarChar, 8000).Value = strCodeList
                .Parameters.Add("@bitMoveUp", SqlDbType.TinyInt).Value = flagMoveUp
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@intListeType", SqlDbType.Int).Value = eListeType

                .Parameters("@retval").Direction = ParameterDirection.ReturnValue
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With

        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

    End Function

#End Region

#Region "Remove Methods"
    ''' <summary>
    ''' Purge Keyword List.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal dataListItemType As enumDataListItemType) As enumEgswErrorCode

        Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Purge, dataListItemType)

    End Function

    ''' <summary>
    ''' Deletes a Keyword.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the Keyword to be deleted.</param>
    ''' <param name="IsGlobal">The Global status of the Keyword to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, _
        ByVal lngCode As Int32, ByVal IsGlobal As Boolean, ByVal dataListItemType As enumDataListItemType) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, -1, lngCode, IsGlobal, enumEgswTransactionMode.Delete, dataListItemType)

    End Function

    ''' <summary>
    ''' Deletes Keywords specified in the list strCodeList.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="strCodeList">The list of Keyword Codes to be deleted.</param>
    ''' <param name="lngCodeSite">The CodeSite from were you are trying to delete, pass -1 if you want to delete using the user's role.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal strCodeList As String, ByVal dataListItemType As enumDataListItemType, Optional ByVal blnForceDelete As Boolean = False) As enumEgswErrorCode

        L_udtUser.Code = lngCodeUser
        Return RemoveFromList(lngCodeUser, lngCodeSite, 0, False, enumEgswTransactionMode.MultipleDelete, dataListItemType, , strCodeList, blnForceDelete)

    End Function

#End Region

#Region " Additional Methods "

    Public Function GetOne(ByVal intCode As Integer, Optional ByVal intCodeTrans As Integer = -1) As DataRow
        Dim tempFetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataSet
        Dim ds As DataSet = CType(GetList(intCode), DataSet)
        L_bytFetchType = tempFetchType

        Dim dt As DataTable = ds.Tables(2)
        If dt.DefaultView.Count = 0 Then Return Nothing

        Dim rw As DataRow = dt.Rows(0)
        If intCodeTrans > -1 Then
            Dim dtTrans As DataTable = ds.Tables(1)
            Dim rwTrans As DataRow

            If dtTrans.Select("CodeTrans=" & CStr(intCodeTrans)).Length > 0 Then
                rwTrans = dtTrans.Select("CodeTrans=" & CStr(intCodeTrans))(0)
                If Len(Trim(CStr(rwTrans("translationname")))) > 0 Then rw("name") = CStr(rwTrans("translationname"))
            End If
        End If
        Return rw
    End Function

    Public Function GetCode(ByVal strName As String, ByVal intCodeSite As Integer, ByVal eDataListeType As enumDataListItemType, ByVal intCodeTrans As Integer, Optional ByVal intParentCode As Integer = -99, Optional ByVal blnCommitToDbase As Boolean = False) As Integer
        If Trim(strName) = "" Then strName = "Not Defined"
        Dim tempFetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataTable
        Dim dt As DataTable = CType(GetList(strName, eDataListeType, intCodeSite, intCodeTrans, 255), DataTable)
        L_bytFetchType = tempFetchType

        Dim intCode As Integer = -1
        Dim rw As DataRow = dt.Rows(0)

        If Not IsDBNull(rw("code")) Then intCode = CInt(dt.Rows(0)("Code"))
        If Not blnCommitToDbase Then GoTo Done

        If intCode > -1 Then
            If IsDBNull(dt.Rows(0)("Status")) OrElse CInt(dt.Rows(0)("Status")) <> 1 Then 'inactive
                Dim cItem As clsItem = New clsItem(L_udtUser, enumAppType.WebApp, L_strCnn)
                cItem.UpdateStatus(intCode, intCodeSite, enumDbaseTables.EgswBrand, 1)
            End If
        Else
            Dim keyword As structKeyword
            keyword.Type = CType(eDataListeType, enumDataListItemType)
            keyword.Code = intCode
            keyword.Name = strName
            keyword.Parent = intParentCode
            keyword.IsGlobal = False
            keyword.Inheritable = False

            Update(L_udtUser.Code, intCodeSite, intCode, keyword)
        End If
Done:
        Return intCode
    End Function

#End Region

End Class

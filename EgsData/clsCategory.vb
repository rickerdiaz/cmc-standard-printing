Imports System.Data.SqlClient
Imports System.Data

''' <summary>
''' Manages Category Table
''' </summary>
''' <remarks></remarks>


Public Class clsCategory
#Region "Class Header"
    'Name               : clsCategory
    'Decription         : Manages Category Table
    'Date Created       : 07.09.2005
    'Author             : VBV
    'Revision History   : VBV - 20.09.2005 - Accepts a connection string as opposed to a connection object. Class performs on a disconnected state.
    '                     VBV - 23.09.2005 - Assign user upon creation of object, expose CodeUser
    '                     VBV - 28.09.2005 - Added overload method GetList(ByVal strName As String, ByVal eType As enumDataListType)
    '                     VBV - 14.12.2005 - Added fetch list for getting Categories with mark status
    '                     VBV - 03.01.2006 - Added option to add "All Category" when fetching list
    '
#End Region

#Region "Variable Declarations / Dependencies"
    Inherits clsDBRoutine
    'Inherits clsDBRoutine

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
    'Public Sub New(ByVal eAppType As enumAppType, ByVal objCnn As SqlConnection, _
    '    ByVal strCnn As String, Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)
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

    Public Property FetchReturnType() As enumEgswFetchType
        Get
            FetchReturnType = L_bytFetchType
        End Get
        Set(ByVal value As enumEgswFetchType)
            L_bytFetchType = value
        End Set
    End Property

    Public Property UserStruct() As structUser
        Get
            UserStruct = L_udtUser
        End Get
        Set(ByVal value As structUser)
            L_udtUser = UserStruct
            If L_udtUser.RoleLevelHighest < 0 Then Throw New Exception("User has an invalid RoleLevel.")
        End Set
    End Property

    Public Property Code() As Int32
        Get
            Code = L_lngCode
        End Get
        Set(ByVal value As Int32)
            L_lngCode = value
        End Set
    End Property
#End Region

#Region "Private Methods"
    Private Function FetchListMark(ByVal intCodeUser As Int32, ByVal intCodeSite As Int32, _
        ByVal EFilterMark As FilterMark, Optional ByVal intCodeTrans As Int32 = -1) As Object
        'vbv 14.12.2005
        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader = Nothing
        Dim cmd As New SqlCommand
        'Dim lngCodeProperty As Int32 = -1        

        If L_udtUser.RoleLevelHighest = 0 Then 'Get ALL items
            'lngCodeProperty = -1
        ElseIf L_udtUser.RoleLevelHighest = 1 Then 'Get ALL items for a site
            intCodeSite = L_udtUser.Site.Code
        ElseIf L_udtUser.RoleLevelHighest = 2 Then 'Get ALL items for a property
            'lngCodeProperty = L_udtUser.Site.Group
        End If

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "CATEG_GetListMark"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@flagGetMark", SqlDbType.TinyInt).Value = EFilterMark
                If intCodeTrans <> -1 Then _
                    .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
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
                    .Fill(ds, "ItemList")
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
        Else
            Return Nothing
        End If
    End Function

    Private Function FetchList(ByVal lngCodeSite As Int32, ByVal lngCode As Int32, ByVal eType As enumDataListItemType, _
        ByVal lngCodeTrans As Int32, ByVal bytStatus As Integer, Optional ByVal strName As String = "", Optional ByVal flagIncludeAll As Boolean = False) As Object

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
                .CommandText = "sp_EgswCategoryGetList"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@tntType", SqlDbType.TinyInt).Value = eType
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = lngCodeTrans
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = L_udtUser.Code 'vbv 23.03.2006
                .Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = lngCodeProperty
                .Parameters.Add("@Status", SqlDbType.TinyInt).Value = bytStatus
                If strName.Trim <> "" Then _
                    .Parameters.Add("@vchName", SqlDbType.NVarChar, 150).Value = strName
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
                    If flagIncludeAll Then
                        Dim rX As DataRow = dt.NewRow
                        rX.Item("Code") = 0
                        rX.Item("Name") = "*All category*"
                        dt.Rows.InsertAt(rX, 0)
                    End If
                End With

            ElseIf L_bytFetchType = enumEgswFetchType.DataSet Then
                With da
                    .SelectCommand = cmd
                    .Fill(ds, "ItemList")
                    If flagIncludeAll Then
                        Dim rX As DataRow = ds.Tables(0).NewRow()
                        rX.Item("Code") = 0
                        rX.Item("Name") = "*All category*"
                        ds.Tables(0).Rows.InsertAt(rX, 0)
                    End If
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
        Else
            Return Nothing
        End If
    End Function

    Private Function FetchTranslationList(ByVal lngCodeTrans As Long) As DataTable

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
                .CommandText = "sp_EgswCategoryGetTranslationList"
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

    Private Function SaveIntoList(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByRef lngCode As Int32, ByVal udtCategory As structCategory, ByVal strCodeSiteList As String, _
        ByVal strCodeCategoryList As String, ByVal TranMode As enumEgswTransactionMode, _
        Optional ByVal oTransaction As SqlTransaction = Nothing) As enumEgswErrorCode

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

                .CommandText = "sp_EgswCategoryUpdate"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 150).Value = udtCategory.Name
                .Parameters.Add("@tntType", SqlDbType.TinyInt).Value = udtCategory.Type
                .Parameters.Add("@intCodeGroup", SqlDbType.Int).Value = udtCategory.CodeGroup
                .Parameters.Add("@nvcCodeAcct", SqlDbType.NVarChar, 25).Value = udtCategory.CodeAcct
                .Parameters.Add("@intPosition", SqlDbType.Int).Value = udtCategory.Position
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = udtCategory.IsGlobal
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode

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

                strCodeCategoryList.Trim()
                If strCodeCategoryList <> "" Then
                    If Not (strCodeCategoryList.StartsWith("(") And strCodeCategoryList.EndsWith(")")) Then
                        Return enumEgswErrorCode.InvalidCodeList
                    Else
                        .Parameters.Add("@vchCodeMergeList", SqlDbType.VarChar, 8000).Value = strCodeCategoryList
                    End If
                End If

                .Parameters("@retval").Direction = ParameterDirection.ReturnValue
                If oTransaction Is Nothing Then .Connection.Open()
                .ExecuteNonQuery()
                If oTransaction Is Nothing Then .Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                lngCode = CInt(.Parameters("@intCode").Value)
            End With

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

    Public Function Deactivate(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal strCodeList As String, ByVal dataListItemType As enumDataListItemType) As enumEgswErrorCode
        Return RemoveFromList(lngCodeUser, lngCodeSite, 0, False, enumEgswTransactionMode.Deactivate, dataListItemType, , , strCodeList)
    End Function

    'Private Function RemoveFromList(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
    'ByVal lngCode As Int32, ByVal IsGlobal As Boolean, ByVal TranMode As enumEgswTransactionMode, _
    'Optional ByVal bytStatus As Byte = 0, Optional ByVal strCodeList As String = "") As enumEgswErrorCode
    Private Function RemoveFromList(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByVal lngCode As Int32, ByVal IsGlobal As Boolean, ByVal TranMode As enumEgswTransactionMode, _
        ByVal dataListItemType As enumDataListItemType, Optional ByVal blnForceDelete As Boolean = False, Optional ByVal bytStatus As Short = 0, Optional ByVal strCodeList As String = "") As enumEgswErrorCode

        Dim cmd As New SqlCommand
        Dim lngCodeProperty As Int32

        If L_udtUser.RoleLevelHighest = 0 Then 'Unshare to ALL 
            lngCodeProperty = -1
        Else 'Unshare to ALL sites belonging to a property or Unshare to self
            lngCodeProperty = L_udtUser.Site.Group
        End If

        'IsGlobal = L_udtUser.RoleLevelHighest = 0

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswCategoryDelete"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = IsGlobal
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = lngCodeProperty
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                .Parameters.Add("@tntType", SqlDbType.TinyInt).Value = dataListItemType
                .Parameters.Add("@blnForceDelete", SqlDbType.Bit).Value = blnForceDelete
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
                cmd.Parameters.Add("@vchTableName", SqlDbType.VarChar, 50).Value = "EgswCategory"

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
            'Return RemoveFromList(L_udtUser.Code, L_lngCodeSite, -1, False, enumEgswTransactionMode.Deactivate)
            Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Deactivate, dataListItemType)
        End If
    End Function

#End Region

#Region "Get Methods"
    ''' <summary>
    ''' Get all Categorys.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>    
    Public Overloads Function GetList() As Object
        Return FetchList(-1, -1, enumDataListItemType.NoType, -1, 255)

    End Function

    ''' <summary>
    ''' Get a Category by Name.
    ''' </summary>
    ''' <param name="strName">The name of the Category to be fetched.</param>
    ''' <param name="eType">One of the enumDataListType values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal strName As String, ByVal eType As enumDataListItemType) As Object

        Return FetchList(-1, -1, eType, -1, 255, strName)

    End Function

    ''' <summary>
    ''' Get a Category by Code.
    ''' </summary>
    ''' <param name="lngCode">The Code of the Category to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal lngCode As Int32) As Object 'DataTable

        Return FetchList(-1, lngCode, enumDataListItemType.NoType, -1, 255)

    End Function

    ''' <summary>
    ''' Get all Categorys by Type.
    ''' </summary>
    ''' <param name="eType">One of the enumDataListType values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal eType As enumDataListItemType) As Object 'DataTable

        Return FetchList(-1, -1, eType, -1, 255)

    End Function

    ''' <summary>
    ''' Get all Categorys by Status.
    ''' </summary>
    ''' <param name="bytStatus">The status of the Categorys to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal bytStatus As Short) As Object 'DataTable
        'Get all by Status
        Return FetchList(-1, -1, enumDataListItemType.NoType, -1, bytStatus)

    End Function

    ''' <summary>
    ''' Get all Categorys with the list of Site names to which they are shared to.
    ''' </summary>
    ''' <param name="eType">One of the enumDataListType values.</param>
    ''' <param name="lngCodeTrans">The Code of the language translation.</param>
    ''' <param name="bytStatus">The status of the Categorys to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal eType As enumDataListItemType, ByVal lngCodeTrans As Int32, ByVal bytStatus As Short) As Object 'DataTable

        Return FetchList(-1, -1, eType, lngCodeTrans, bytStatus)

    End Function

    ''' <summary>
    ''' Get all Categories used by items marked for Kiosk
    ''' </summary>
    ''' <param name="eType">One of the enumDataListType values.</param>
    ''' <param name="lngCodeTrans">The Code of the language translation.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal eType As enumDataListItemType, ByVal lngCodeTrans As Int32) As Object 'DataTable
        'RDTC 10.10.2006
        Return FetchList(-1, -5, eType, lngCodeTrans, 1)

    End Function


    ''' <summary>
    ''' Get all Categorys shared to a specific site.
    ''' </summary>
    ''' <param name="eType">One of the enumDataListType values.</param>
    ''' <param name="lngCodeTrans">The Code of the language translation.</param>
    ''' <param name="bytStatus">The status of the Category to be fetched.</param>
    ''' <param name="lngCodeSite">The site to which the Category is shared.</param>
    ''' <param name="flagIncludeAll">Include "All Category" from the list.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal eType As enumDataListItemType, ByVal lngCodeTrans As Int32, ByVal bytStatus As Integer, ByVal lngCodeSite As Int32, Optional ByVal flagIncludeAll As Boolean = False) As Object 'DataTable

        Return FetchList(lngCodeSite, -1, eType, lngCodeTrans, bytStatus, , flagIncludeAll)

    End Function

    ''' <summary>
    ''' Get all Categories 
    ''' </summary>    
    ''' <param name="eType"></param>
    ''' <param name="lngCodeTrans"></param>
    ''' <param name="bytStatus"></param>
    ''' <param name="ESiteFilter"></param>
    ''' <param name="flagIncludeAll"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal eType As enumDataListItemType, ByVal lngCodeTrans As Int32, ByVal bytStatus As Short, ByVal ESiteFilter As FilterSite, ByVal strNameFilter As String, Optional ByVal flagIncludeAll As Boolean = False) As Object 'DataTable        
        'Public Overloads Function GetList(ByVal flagGetMySite As Boolean, ByVal eType As enumDataListItemType, ByVal lngCodeTrans As Int32, ByVal bytStatus As Byte, ByVal lngCodeSite As Int32, Optional ByVal flagIncludeAll As Boolean = False) As Object 'DataTable        
        'VBV 23.03.2006
        'If flagGetMySite Then
        '    Return FetchList(lngCodeSite, -2, eType, lngCodeTrans, bytStatus, , flagIncludeAll)
        'Else
        '    Return FetchList(lngCodeSite, -3, eType, lngCodeTrans, bytStatus, , flagIncludeAll)
        'End If
        'use -1 for lngCodeSite to return from all sites
        'use lngCodeSite for lngCode to filter in sproc
        Return FetchList(-1, ESiteFilter, eType, lngCodeTrans, bytStatus, strNameFilter, flagIncludeAll)

    End Function

    ''' <summary>
    ''' Get all Translations.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTranslationList() As Object 'DataTable
        'Get all
        Return FetchTranslationList(-1)

    End Function

    ''' <summary>
    ''' Get a specific translation.
    ''' </summary>
    ''' <param name="lngCodeTrans">The Code of the language translation.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTranslationList(ByVal lngCodeTrans As Long) As Object 'DataTable
        'Filter by CodeTrans
        Return FetchTranslationList(lngCodeTrans)

    End Function

    ''' <summary>
    ''' Get a Category by Name w/in the codesite.
    ''' </summary>
    ''' <param name="strName">The name of the Category to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal strName As String, ByVal eDataListeType As enumDataListItemType, ByVal bytStatus As Integer, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer) As Object

        Return FetchList(intCodeSite, -4, eDataListeType, intCodeTrans, bytStatus, strName)

    End Function

    ''' <summary>
    ''' Get categories by site for markings.
    ''' </summary>
    ''' <param name="intCodeUser">The code of the user.</param>
    ''' <param name="intCodeSite">The code of the site.</param>    
    ''' <param name="EMarkFilter">One of FilterMark values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetListMark(ByVal intCodeUser As Int32, ByVal intCodeSite As Integer, ByVal intCodeTrans As Int32, ByVal EMarkFilter As FilterMark) As Object
        'vbv 14.12.2005
        Return FetchListMark(intCodeUser, intCodeSite, EMarkFilter, intCodeTrans)

    End Function

    Public Function GetListCategoryCodeName(ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, ByVal type As enumDataListItemType, Optional ByVal flagActiveOnly As Boolean = True) As Object
        Dim strCommandText As String = "[GET_CATEGORYCODENAME]"

        '@ListeType int,
        '@CodeSite int,
        '@CodeTrans int,
        '@ActiveOnly bit =1

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
    End Function


    Public Function GetListCategory(ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, ByVal type As enumDataListItemType, ByVal intStatus As Integer) As Object
        Dim strCommandText As String = "[GET_CATEGORYLIST]"

        '@ListeType int,
        '@CodeSite int,
        '@CodeTrans int,
        '@ActiveOnly bit =1

        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@ListeType", type)
        arrParam(1) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(2) = New SqlParameter("@CodeTrans", L_udtUser.CodeTrans)
        arrParam(3) = New SqlParameter("@Status", intStatus)

        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

	Public Function GetCategoryCodeByName(ByVal strName As String, Optional ByVal intType As Integer = -1) As Integer
		Dim cmd As SqlCommand = New SqlCommand
		Dim da As New SqlDataAdapter
		Dim dt As New DataTable
		Try
			With cmd
				.Connection = New SqlConnection(L_strCnn)
				.Connection.Open()
				.CommandType = CommandType.StoredProcedure
				.CommandText = "sp_EgsWGetCategoryCodeByName"
				.Parameters.Add("@strName", SqlDbType.NVarChar, 100).Value = strName
				.Parameters.Add("@intType", SqlDbType.Int).Value = intType
				.ExecuteNonQuery()

				With da
					.SelectCommand = cmd
					dt.BeginLoadData()
					.Fill(dt)
					dt.EndLoadData()
				End With
				.Connection.Close()
				.Dispose()
			End With

			Return dt.Rows(0).Item("Code")

		Catch ex As Exception
			cmd.Dispose()
		End Try
	End Function

#End Region

#Region "Update Methods"
    ''' <summary>
    ''' Standardize Categorys
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="eListeType">One of the enumDataListType values.</param>
    ''' <param name="eItemListType">One of the enumDataListType values.</param>
    ''' <param name="eFormat">One of the enumEgswStandardizationFormat values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Standardize(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal eListeType As enumDataListItemType, _
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
    ''' Updates the global status of a Category.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the Category to be updated.</param>
    ''' <param name="IsGlobal">The global status of the Category to be updated.</param>
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
                .CommandText = "sp_EgswCategoryUpdateGlobal"
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
    ''' Updates Category without sharing it to any sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the Category to be updated.</param>
    ''' <param name="udtCategory">One of the structCategory values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByRef lngCode As Int32, ByVal udtCategory As structCategory) As enumEgswErrorCode

        Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtCategory, "", "", _
             CType(IIf(lngCode < 1, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    ''' <summary>
    ''' Updates Category sharing it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtCategory">One of the structCategory values.</param>
    ''' <param name="strCodeSiteList">The list of sites where Category will be shared.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtCategory As structCategory, ByVal strCodeSiteList As String) As enumEgswErrorCode

        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) And strCodeSiteList.Trim <> "" Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtCategory, strCodeSiteList, "", _
             CType(IIf(lngCode < 1, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    ''' <summary>
    ''' Updates Category and its Translations and share it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtCategory">One of the structCategory values.</param>
    ''' <param name="strCodeSiteList">The list of sites where Category will be shared.</param>
    ''' <param name="arrTransCode">The list of Translation Codes of the Category.</param>
    ''' <param name="arrTransName">The list of Translation Names of the Category.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtCategory As structCategory, ByVal strCodeSiteList As String, _
        ByVal arrTransCode() As String, ByVal arrTransName() As String) As enumEgswErrorCode


        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) And strCodeSiteList.Trim <> "" Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        Dim t As SqlTransaction
        Dim cn As New SqlConnection(L_strCnn)

        cn.Open()
        t = cn.BeginTransaction()
        L_ErrCode = SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtCategory, strCodeSiteList, "", _
             CType(IIf(lngCode < 1, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

        If L_ErrCode = enumEgswErrorCode.OK Then
            Try
                'Update Translations
                ' Dim arrTransCode() As String = strTransCodeList.Split(CChar(","))
                '  Dim arrTransName() As String = strTransNameList.Split(CChar(","))
                Dim c As Int32 = arrTransCode.Length - 1
                Dim i As Int32
                Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

                For i = 0 To c
                    If IsNumeric(arrTransCode(i)) Then
                        L_ErrCode = oTrans.UpdateTranslation(lngCode, arrTransName(i), CInt(arrTransCode(i)), udtCategory.Type, lngCodeSite, lngCodeUser, enumDataListType.Category)
                        If L_ErrCode <> enumEgswErrorCode.OK Then Exit For
                    End If
                Next
                oTrans = Nothing
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
    ''' Updates Category and its Translations and share it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtCategory">One of the structCategory values.</param>
    ''' <param name="strCodeSiteList">The list of sites where Category will be shared.</param>
    ''' <param name="dtTranslations">The list of Translations of the Category.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtCategory As structCategory, _
        ByVal strCodeSiteList As String, ByVal dtTranslations As DataTable) As enumEgswErrorCode

        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) And strCodeSiteList.Trim <> "" Then
            Return enumEgswErrorCode.InvalidCodeList
        End If


        Dim t As SqlTransaction
        Dim cn As New SqlConnection(L_strCnn)

        cn.Open()
        t = cn.BeginTransaction()
        L_ErrCode = SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtCategory, strCodeSiteList, "", _
             CType(IIf(lngCode < 1, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode), _
             t)

        If L_ErrCode > 0 Then
            Try
                Dim rowX As DataRow
                Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

                For Each rowX In dtTranslations.Rows
                    L_ErrCode = oTrans.UpdateTranslation(lngCode, rowX.Item("Name").ToString, CInt(rowX.Item("CodeTrans")), udtCategory.Type, lngCodeSite, lngCodeUser, enumDataListType.Category)
                    If L_ErrCode <> enumEgswErrorCode.OK Then Exit For
                Next

                oTrans = Nothing
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
    ''' Updates Category's translations (multiple update)
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="eListeType">One of the enumDataListType values.</param>
    ''' <param name="dtTranslations">The list of Translations of the Category.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal eListeType As enumDataListType, ByVal dtTranslations As DataTable) As enumEgswErrorCode

        Dim t As SqlTransaction
        Dim cn As New SqlConnection(L_strCnn)

        cn.Open()
        t = cn.BeginTransaction()
        Dim rowX As DataRow

        Try
            Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

            For Each rowX In dtTranslations.Rows
                L_ErrCode = oTrans.UpdateTranslation(lngCode, rowX.Item("Name").ToString, CInt(rowX.Item("CodeTrans")), eListeType, lngCodeSite, lngCodeUser, enumDataListType.Category)
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
    ''' Updates a Category's translation (single update)
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="eListeType">One of the enumDataListType values.</param>
    ''' <param name="lngCodeTrans">The code of the Category's translation.</param>
    ''' <param name="strNameTrans">The name of the Category's translation.</param>
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
            L_ErrCode = oTrans.UpdateTranslation(lngCode, strNameTrans, lngCodeTrans, eListeType, lngCodeSite, lngCodeUser, enumDataListType.Category)

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
    ''' Merge Categorys
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="strCodeCategoryList">The list of Category Codes to be merged.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal strCodeCategoryList As String, ByVal udtCategory As structCategory, Optional ByRef intCode As Integer = 0) As enumEgswErrorCode
        Return SaveIntoList(lngCodeUser, lngCodeSite, intCode, udtCategory, "", strCodeCategoryList, enumEgswTransactionMode.MergeDelete)
    End Function

    ''' <summary>
    ''' Updates Status of the Categorys specified in the list (strCodeList).
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>    
    ''' <param name="strCodeList">The list of Category Codes to be updated.</param>
    ''' <param name="bytStatus">The Status of the Category.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal strCodeList As String, ByVal bytStatus As Short, ByVal dataListItemType As enumDataListItemType) As enumEgswErrorCode
        'Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal strCodeList As String, ByVal bytStatus As Byte) As enumEgswErrorCode

        'Return RemoveFromList(lngCodeUser, lngCodeSite, -1, False, enumEgswTransactionMode.Deactivate, bytStatus, strCodeList)
        Return RemoveFromList(lngCodeUser, -1, -1, False, enumEgswTransactionMode.Deactivate, dataListItemType, , bytStatus, strCodeList)

    End Function

    ''' <summary>
    ''' Updates Status of a Category.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>    
    ''' <param name="lngCode">The Code of the Category to be updated.</param>
    ''' <param name="IsGlobal">The Global Status of the Category.</param>
    ''' <param name="bytStatus">The Status of the Category.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCode As Int32, ByVal IsGlobal As Boolean, _
        ByVal bytStatus As Short, ByVal dataListItemType As enumDataListItemType) As enumEgswErrorCode

        'Return RemoveFromList(lngCodeUser, lngCodeSite, lngCode, IsGlobal, enumEgswTransactionMode.Deactivate, bytStatus)
        Return RemoveFromList(lngCodeUser, -1, lngCode, IsGlobal, enumEgswTransactionMode.Deactivate, dataListItemType, , bytStatus)

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
                .CommandText = "sp_EgswCategoryMovePos"
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

    '====================== 
    ' ADD / EDIT CATEGORY
    ' VRP 24.04.2009
    '====================== 
    Public Function UpdateCategory(ByRef intCode As Integer, ByVal udtCategory As structCategory, _
                                   ByVal intCodeSite As Integer, ByVal intCodeUser As Integer, _
                                   ByVal strCodeSiteList As String, ByVal arrTransCode() As String, _
                                   ByVal arrTransName() As String) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandType = CommandType.StoredProcedure
                .CommandText = "MANAGE_CATEGORYUPDATE"

                .Parameters.Add("@retVal", SqlDbType.Int)
                .Parameters.Add("@Code", SqlDbType.Int).Value = intCode
                .Parameters.Add("@CodeGroup", SqlDbType.Int).Value = udtCategory.CodeGroup
                .Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = udtCategory.Name
                .Parameters.Add("@ListeType", SqlDbType.Int).Value = udtCategory.Type
                .Parameters.Add("@CodeAcct", SqlDbType.NVarChar, 25).Value = udtCategory.CodeAcct
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = udtCategory.IsGlobal
                .Parameters.Add("@CodeSiteList", SqlDbType.VarChar, 8000).Value = strCodeSiteList
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@IsProduct", SqlDbType.Bit).Value = udtCategory.IsProduct

                Dim strTemp As String = "MANAGE_CATEGORYUPDATE " & intCode & ", " & udtCategory.CodeGroup & " " & udtCategory.Name & " " & udtCategory.Type & " " _
                                        & udtCategory.CodeAcct & " " & udtCategory.IsGlobal & " " & strCodeSiteList & " " & intCodeUser & " " & intCodeSite & " " & udtCategory.IsProduct
                .Parameters("@retVal").Direction = ParameterDirection.ReturnValue
                .Parameters("@Code").Direction = ParameterDirection.InputOutput

                .Connection.Open()
                .ExecuteNonQuery()
                'Dim strTemp1 As String = .Parameters("@SQL").Value.ToString
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                intCode = CInt(.Parameters("@Code").Value)

                If L_ErrCode = enumEgswErrorCode.OK Then

                    Try
                        Dim c As Int32 = arrTransCode.Length - 1
                        Dim i As Int32
                        Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

                        For i = 0 To c
                            If IsNumeric(arrTransCode(i)) Then
                                L_ErrCode = oTrans.UpdateTranslation(intCode, arrTransName(i), CInt(arrTransCode(i)), udtCategory.Type, intCodeSite, intCodeUser, enumDataListType.Category)
                                If L_ErrCode <> enumEgswErrorCode.OK Then Exit For
                            End If
                        Next
                        oTrans = Nothing
                    Catch ex As Exception
                        L_ErrCode = enumEgswErrorCode.GeneralError
                    End Try

                End If

                cmd.Dispose()
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            cmd.Dispose()
        End Try
        Return L_ErrCode
    End Function

#End Region

#Region "Remove Methods"
    ''' <summary>
    ''' Purge Category List.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal dataListItemType As enumDataListItemType) As enumEgswErrorCode

        'Return RemoveFromList(-1, -1, -1, False, enumEgswTransactionMode.Purge)
        Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Purge, dataListItemType)

    End Function

    ''' <summary>
    ''' Deletes a Category.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>    
    ''' <param name="lngCode">The Code of the Category to be deleted.</param>
    ''' <param name="IsGlobal">The Global status of the Category to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, _
        ByVal lngCode As Int32, ByVal IsGlobal As Boolean, ByVal dataListItemType As enumDataListItemType, ByVal blnForceDelete As Boolean) As enumEgswErrorCode
        'Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _        

        'Return RemoveFromList(lngCodeUser, lngCodeSite, lngCode, IsGlobal, enumEgswTransactionMode.Delete)
        Return RemoveFromList(lngCodeUser, -1, lngCode, IsGlobal, enumEgswTransactionMode.Delete, dataListItemType, blnForceDelete)

    End Function

    ''' <summary>
    ''' Deletes Categorys specified in the list strCodeList.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>    
    ''' <param name="strCodeList">The list of Category Codes to be deleted.</param>
    ''' <param name="lngCodeSite">The CodeSite from were you are trying to delete, pass -1 if you want to delete using the user's role.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>    
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Integer, ByVal strCodeList As String, ByVal dataListItemType As enumDataListItemType, ByVal blnForceDelete As Boolean) As enumEgswErrorCode
        'Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal strCodeList As String) As enumEgswErrorCode

        L_udtUser.Code = lngCodeUser
        'L_lngCodeSite = lngCodeSite
        'Return RemoveFromList(lngCodeUser, lngCodeSite, 0, False, enumEgswTransactionMode.MultipleDelete, , strCodeList)
        Return RemoveFromList(lngCodeUser, lngCodeSite, 0, False, enumEgswTransactionMode.MultipleDelete, dataListItemType, blnForceDelete, , strCodeList)

    End Function

#End Region

#Region " Other Function "

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

    Public Function GetCode(ByVal strName As String, ByVal eDataListeType As enumDataListItemType, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, Optional ByVal blnCommitToDbase As Boolean = False) As Integer
        If Trim(strName) = "" Then strName = "Not Defined"
        Dim tempFetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataTable
        Dim dt As DataTable = CType(GetList(strName, eDataListeType, 255, intCodeSite, intCodeTrans), DataTable)
        L_bytFetchType = tempFetchType

        Dim intCode As Integer = -1
        Dim rw As DataRow = dt.Rows(0)
        Dim strCodeSiteList As String = "(" & intCodeSite & ")"
        Dim drCodeTrans As SqlDataReader
        Dim i As Integer
        Dim cTrans As New clsLanguage(enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataReader)
        Dim bytStatus As Short = 1
        Dim cLang As New clsEGSLanguage(intCodeTrans)



        If Not IsDBNull(rw("code")) Then intCode = CInt(dt.Rows(0)("Code"))
        If Not blnCommitToDbase Then GoTo Done

        If intCode > -1 Then
            If IsDBNull(dt.Rows(0)("Status")) OrElse CInt(dt.Rows(0)("Status")) <> 1 Then 'inactive
                Dim cItem As clsItem = New clsItem(L_udtUser, enumAppType.WebApp, L_strCnn)
                cItem.UpdateStatus(intCode, intCodeSite, enumDbaseTables.EgswCategory, 1)
            End If
        Else
            Dim category As structCategory
            category.Code = intCode
            category.Name = strName
            category.IsGlobal = False
            category.Type = CType(eDataListeType, enumDataListItemType)
            category.IsGlobal = False
            category.CodeAcct = "0"

            drCodeTrans = CType(cTrans.GetList(intCodeSite, bytStatus), SqlDataReader)

            Dim arryTransCodeList As New ArrayList, arryTransNameList As New ArrayList
            While drCodeTrans.Read
                arryTransCodeList.Add(CStr(drCodeTrans.Item("Code")))
            End While
            drCodeTrans.Close()

            Dim strTransCodeList(arryTransCodeList.Count), strTransNameList(arryTransCodeList.Count) As String

            For i = 0 To arryTransCodeList.Count - 1
                strTransCodeList(i) = arryTransCodeList(i).ToString
                strTransNameList(i) = strName
            Next


            Update(L_udtUser.Code, intCodeSite, category.Code, category, strCodeSiteList, strTransCodeList, strTransNameList)
            'Update(L_udtUser.Code, intCodeSite, intCode, category)
            intCode = category.Code
        End If
Done:
        Return intCode
    End Function

#End Region

End Class

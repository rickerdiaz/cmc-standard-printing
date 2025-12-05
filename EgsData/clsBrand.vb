Imports System.Data.SqlClient
Imports System.Data
Imports EgsData.clsDBRoutine

#Region "Class Header"
'Name               : clsBrand
'Decription         : Manages Brand Table
'Date Created       : 05.09.2005
'Author             : VBV
'Revision History   : VBV / 07.09.2005 / Compatibility with WebApp and SmartClient
'                   : VBV - 20.09.2005 - Accepts a connection string as opposed to a connection object. Class performs on a disconnected state.
'                     VBV - 23.09.2005 - Assign user upon creation of object, expose CodeUser
'                     VBV - 01.12.2005 - Added option to creates a new record when instantiating the class, expose data columns as properties of the class
#End Region

''' <summary>
''' Manages Brand Table
''' </summary>
''' <remarks></remarks>

Public Class clsBrand


#Region "Variable Declarations / Dependencies"
    'Inherits clsDBRoutine

    'Private L_Cnn As SqlConnection
    'Private cmd As New SqlCommand
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
    'vbv 01.12.2005
    Private L_strName As String
    Private L_ItemType As enumDataListItemType
    Private L_IsGlobal As Boolean
    'vbv 01.12.2005
    'Private WithEvents i As clsInventory
#End Region

#Region "Class Functions and Properties"
#Region "Functions"
    Public Sub New(ByVal udtUser As structUser, ByVal eAppType As enumAppType, ByVal strCnn As String, _
       Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader, _
       Optional ByVal CreateRecord As Boolean = False)

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
            '     If udtUser.Code <= 0 Then Throw New Exception("Invalid CodeUser.")
            L_udtUser = udtUser
            L_AppType = eAppType
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn


            If CreateRecord Then
                'L_intCode = -1
                'Update(L_udtUser.Code, L_udtUser.Site.Code,L_intCode,
                'Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
                '        ByRef lngCode As Int32, ByVal udtBrand As structBrand) As enumEgswErrorCode
            End If
        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try

    End Sub

    Public Sub New(strConnection As String)
        L_strCnn = strConnection
        L_bytFetchType = enumEgswFetchType.DataTable
    End Sub

    Protected Overrides Sub Finalize()
        ClearMarkings() 'items marked as not deleted
        MyBase.Finalize()
    End Sub

#End Region

#Region "Properties"
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

    Public Property Code() As Int32
        Get
            Code = L_intCode
        End Get
        Set(ByVal value As Int32)
            L_intCode = value
        End Set
    End Property

    Public Property Type() As enumDataListItemType
        Get
            Type = L_ItemType
        End Get
        Set(ByVal value As enumDataListItemType)
            L_ItemType = value
        End Set
    End Property

    Public Property IsGlobal() As Boolean
        Get
            IsGlobal = L_IsGlobal
        End Get
        Set(ByVal value As Boolean)
            L_IsGlobal = value
        End Set
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
    '        CodeSite = L_intCodeSite
    '    End Get
    '    Set(ByVal value As Int32)
    '        L_intCodeSite = value
    '    End Set
    'End Property
#End Region

#End Region

#Region "Private Methods"

    ''-- JBB 06.05.2012

    Private Function GetBrandChildListofParent(ByVal intParent As Integer, strParentCode As String) As ArrayList
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
                .Parameters.Add("@Type", SqlDbType.Int).Value = 3
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
                .CommandText = "sp_EgswBrandGetList"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@tntType", SqlDbType.TinyInt).Value = eType
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
            Throw New Exception(ex.Message, ex)
        Finally
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
        End Try


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

    Private Function SaveIntoList(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByRef lngCode As Int32, ByVal udtBrand As structBrand, ByVal strCodeSiteList As String, _
        ByVal strCodeBrandList As String, ByVal TranMode As enumEgswTransactionMode, _
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

                .CommandText = "sp_EgswBrandUpdate"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@intCode", SqlDbType.Int).Value = udtBrand.Code
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 150).Value = udtBrand.Name
                .Parameters.Add("@tntType", SqlDbType.TinyInt).Value = udtBrand.Type
                .Parameters.Add("@intPosition", SqlDbType.Int).Value = udtBrand.Position
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = udtBrand.IsGlobal
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                .Parameters.Add("@CodeParent", SqlDbType.Int).Value = IIf(IsNothing(udtBrand.Parent), DBNull.Value, udtBrand.Parent) ' JBB 12.28.2010
                .Parameters.Add("@IsCanBeParent", SqlDbType.Bit).Value = udtBrand.IsCanBeParent ' JBB 07.13.2012
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

                strCodeBrandList.Trim()
                If strCodeBrandList <> "" Then
                    If Not (strCodeBrandList.StartsWith("(") And strCodeBrandList.EndsWith(")")) Then
                        Return enumEgswErrorCode.InvalidCodeList
                    Else
                        .Parameters.Add("@vchCodeMergeList", SqlDbType.VarChar, 8000).Value = strCodeBrandList
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
                .CommandText = "sp_EgswBrandDelete"
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

    Private Function ClearMarkings() As enumEgswErrorCode
        'Deactivate items that were not deleted by the Delete module
        If L_udtUser.Code <> -1 And L_intCodeSite <> -1 Then
            Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Deactivate)
        End If
    End Function

#End Region

#Region "Get Methods"
    ''' <summary>
    ''' Get all Brands.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList() As Object

        Return FetchList(-1, -1, enumDataListType.NoType, -1, 255)

    End Function

    ''' <summary>
    ''' Get a Brand by Code.
    ''' </summary>
    ''' <param name="lngCode">The Code of the Brand to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal lngCode As Int32) As Object

        Return FetchList(-1, lngCode, enumDataListType.NoType, -1, 255)

    End Function

    ''' <summary>
    ''' Get all Brands by Type.
    ''' </summary>
    ''' <param name="eType">One of the enumDataListType values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal eType As enumDataListType) As Object

        Return FetchList(-1, -1, eType, -1, 255)

    End Function

    ''' <summary>
    ''' Get all Brands by Status.
    ''' </summary>
    ''' <param name="bytStatus">The status of the Brands to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal bytStatus As Byte) As Object
        'Get all by Status
        Return FetchList(-1, -1, enumDataListType.NoType, -1, bytStatus)

    End Function

    ''' <summary>
    ''' Get all Brands with the list of Site names to which they are shared to.
    ''' </summary>
    ''' <param name="eType">One of the enumDataListType values.</param>
    ''' <param name="lngCodeTrans">The Code of the language translation.</param>
    ''' <param name="bytStatus">The status of the Brands to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal eType As enumDataListType, ByVal lngCodeTrans As Int32, ByVal bytStatus As Byte) As Object

        Return FetchList(-1, -1, eType, lngCodeTrans, bytStatus)

    End Function

    ''' <summary>
    ''' Get all Brands shared to a specific site.
    ''' </summary>
    ''' <param name="eType">One of the enumDataListType values.</param>
    ''' <param name="lngCodeTrans">The Code of the language translation.</param>
    ''' <param name="bytStatus">The status of the Brand to be fetched.</param>
    ''' <param name="lngCodeSite">The site to which the Brand is shared.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal eType As enumDataListType, ByVal lngCodeTrans As Int32, ByVal bytStatus As Byte, ByVal lngCodeSite As Int32) As Object

        Return FetchList(lngCodeSite, -1, eType, lngCodeTrans, bytStatus)

    End Function

    ''' <summary>
    ''' Get all Translations.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTranslationList() As DataTable
        'Get all
        Return FetchTranslationList(-1)

    End Function

    ''' <summary>
    ''' Get a specific translation.
    ''' </summary>
    ''' <param name="lngCodeTrans">The Code of the language translation.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTranslationList(ByVal lngCodeTrans As Long) As DataTable
        'Filter by CodeTrans
        Return FetchTranslationList(lngCodeTrans)

    End Function

    ''' <summary>
    ''' Get a Category by Name w/in the codesite.
    ''' </summary>
    ''' <param name="strName">The name of the Category to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal strName As String, ByVal eDataListeType As enumDataListType, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, ByVal bytStatus As Byte) As Object

        Return FetchList(intCodeSite, -1, eDataListeType, intCodeTrans, bytStatus, strName)

    End Function


    Public Function GetListBrand(ByVal enumType As enumDataListItemType, _
                                 ByVal intCodeTrans As Integer, ByVal intCodeSite As Integer, _
                                 Optional ByVal intStatus As Integer = 255, _
                                 Optional strSearchString As String = "",
                                 Optional intCodeProperty As Integer = -1) As Object
        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader
        Dim cmd As New SqlCommand

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[GET_BRANDLIST]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@ListeType", SqlDbType.Int).Value = enumType
                .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@Status", SqlDbType.TinyInt).Value = intStatus
                .Parameters.Add("@SearchString", SqlDbType.NVarChar).Value = strSearchString 'AGL 2014.07.11
                .Parameters.Add("@CodeProperty", SqlDbType.Int).Value = intCodeProperty 'MKAM 2014.10.24
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
        Finally
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
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

    Public Function GetBrandBrandSite(ByVal intBrand As Integer) As DataTable
        Dim dt As New DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        With cmd
            .Connection = New SqlConnection(L_strCnn)
            .CommandText = "Get_BrandBrandSite"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intCodeBrand", SqlDbType.Int).Value = intBrand
        End With
        With da
            .SelectCommand = cmd
            dt.BeginLoadData()
            .Fill(dt)
            dt.EndLoadData()
        End With
        Return dt
    End Function

    '-- JBB 03.06.2012
    Public Function GetBrandBrandSite(ByVal strBrand As String) As DataTable
        Dim dt As New DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        With cmd
            .Connection = New SqlConnection(L_strCnn)
            .CommandText = "Get_ListBrandBrandSite"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@nvchCodeBrand", SqlDbType.NVarChar, 200).Value = strBrand
        End With
        With da
            .SelectCommand = cmd
            dt.BeginLoadData()
            .Fill(dt)
            dt.EndLoadData()
        End With
        Return dt
    End Function

    '--

#End Region

#Region "Update Methods"
    ''' <summary>
    ''' Standardize Brands
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="eListeType">One of the enumDataListType values.</param>
    ''' <param name="eItemListType">One of the enumDataListType values.</param>
    ''' <param name="eFormat">One of the enumEgswStandardizationFormat values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Standardize(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal eListeType As enumDataListType, _
        ByVal eItemListType As enumDataListItemType, ByVal eFormat As enumEgswStandardizationFormat) As enumEgswErrorCode

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
        Finally
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
        End Try

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

    End Function

    ''' <summary>
    ''' Updates the global status of a Brand.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the Brand to be updated.</param>
    ''' <param name="IsGlobal">The global status of the Brand to be updated.</param>
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
                .CommandText = "sp_EgswBrandUpdateGlobal"
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
        Finally
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
        End Try

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

    End Function

    ''' <summary>
    ''' Updates Brand without sharing it to any sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the Brand to be updated.</param>
    ''' <param name="udtBrand">One of the structBrand values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByRef lngCode As Int32, ByVal udtBrand As structBrand) As enumEgswErrorCode

        Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtBrand, "(" & lngCodeSite & ")", "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    ''' <summary>
    ''' Updates Brand sharing it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtBrand">One of the structBrand values.</param>
    ''' <param name="strCodeSiteList">The list of sites where Brand will be shared.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtBrand As structBrand, ByVal strCodeSiteList As String) As enumEgswErrorCode

        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) And strCodeSiteList.Trim <> "" Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtBrand, strCodeSiteList, "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    ''' <summary>
    ''' Updates Brand and its Translations and share it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtBrand">One of the structBrand values.</param>
    ''' <param name="strCodeSiteList">The list of sites where Brand will be shared.</param>
    ''' <param name="arrTransCode">The list of Translation Codes of the Brand.</param>
    ''' <param name="arrTransName">The list of Translation Names of the Brand.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtBrand As structBrand, ByVal strCodeSiteList As String, _
        ByVal arrTransCode() As String, ByVal arrTransName() As String) As enumEgswErrorCode

        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) And strCodeSiteList.Trim <> "" Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        Dim t As SqlTransaction
        Dim cn As New SqlConnection(L_strCnn)

        Try


            cn.Open()
            t = cn.BeginTransaction()
            L_ErrCode = SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtBrand, strCodeSiteList, "", _
                 CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode), _
                 t)

            If L_ErrCode = enumEgswErrorCode.OK Then
                Try
                    'Update Translations
                    '     Dim arrTransCode() As String = (strTransCodeList.Split(CChar(",")))
                    '    Dim arrTransName() As String = strTransNameList.Split(CChar(","))
                    Dim c As Int32 = arrTransCode.Length - 1
                    Dim i As Int32
                    Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

                    For i = 0 To c
                        If IsNumeric(arrTransCode(i)) Then
                            L_ErrCode = oTrans.UpdateTranslation(lngCode, arrTransName(i), CInt(arrTransCode(i)), udtBrand.Type, lngCodeSite, lngCodeUser, enumDataListType.Brand)
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

        Catch ex As Exception
        Finally
            If cn.State <> ConnectionState.Closed Then cn.Close()
            cn.Dispose()
        End Try

        Return L_ErrCode

    End Function

    ''' <summary>
    ''' Updates Brand and its Translations and share it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtBrand">One of the structBrand values.</param>
    ''' <param name="strCodeSiteList">The list of sites where Brand will be shared.</param>
    ''' <param name="dtTranslations">The list of Translations of the Brand.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtBrand As structBrand, _
        ByVal strCodeSiteList As String, ByVal dtTranslations As DataTable) As enumEgswErrorCode

        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) And strCodeSiteList.Trim <> "" Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        Dim t As SqlTransaction
        Dim cn As New SqlConnection(L_strCnn)

        Try
            cn.Open()
            t = cn.BeginTransaction()
            L_ErrCode = SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtBrand, strCodeSiteList, "", _
                 CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode), _
                 t)

            If L_ErrCode > 0 Then
                Try
                    Dim rowX As DataRow
                    Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

                    For Each rowX In dtTranslations.Rows
                        L_ErrCode = oTrans.UpdateTranslation(lngCode, rowX.Item("Name").ToString, CInt(rowX.Item("CodeTrans")), udtBrand.Type, lngCodeSite, lngCodeUser, enumDataListType.Brand)
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

        Catch ex As Exception

        Finally
            If cn.State <> ConnectionState.Closed Then cn.Close()
            cn.Dispose()
        End Try

        Return L_ErrCode

    End Function

    ''' <summary>
    ''' Updatess Brand's translations (multiple update)
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="eListeType">One of the enumDataListType values.</param>
    ''' <param name="dtTranslations">The list of Translations of the Brand.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal eListeType As enumDataListType, ByVal dtTranslations As DataTable) As enumEgswErrorCode

        Dim t As SqlTransaction
        Dim cn As New SqlConnection(L_strCnn)

        Try
            cn.Open()
            t = cn.BeginTransaction()
            Try
                Dim rowX As DataRow
                Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

                For Each rowX In dtTranslations.Rows
                    L_ErrCode = oTrans.UpdateTranslation(lngCode, rowX.Item("Name").ToString, CInt(rowX.Item("CodeTrans")), eListeType, lngCodeSite, lngCodeUser, enumDataListType.Brand)
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

        Catch ex As Exception

        Finally
            If cn.State <> ConnectionState.Closed Then cn.Close()
            cn.Dispose()
        End Try

        Return L_ErrCode

    End Function

    ''' <summary>
    ''' Updates a Brand's translation (single update)
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="eListeType">One of the enumDataListType values.</param>
    ''' <param name="lngCodeTrans">The code of the Brand's translation.</param>
    ''' <param name="strNameTrans">The name of the Brand's translation.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, ByVal eListeType As enumDataListType, _
        ByVal lngCodeTrans As Int32, ByVal strNameTrans As String) As enumEgswErrorCode

        Dim t As SqlTransaction
        Dim cn As New SqlConnection(L_strCnn)

        Try
            cn.Open()
            t = cn.BeginTransaction()
            Try
                Dim oTrans As New clsTranslation(L_AppType, L_strCnn)
                L_ErrCode = oTrans.UpdateTranslation(lngCode, strNameTrans, lngCodeTrans, eListeType, lngCodeSite, lngCodeUser, enumDataListType.Brand)

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

        Catch ex As Exception
        Finally
            If cn.State <> ConnectionState.Closed Then cn.Close()
            cn.Dispose()
        End Try

        Return L_ErrCode

    End Function

    ''' <summary>
    ''' Merge Brands
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="strCodeBrandList">The list of Brand Codes to be merged.</param>
    ''' <param name="udtBrand">Brand info.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByVal strCodeBrandList As String, ByVal udtBrand As structBrand) As enumEgswErrorCode

        Return SaveIntoList(lngCodeUser, lngCodeSite, 0, udtBrand, "", strCodeBrandList, enumEgswTransactionMode.MergeDelete)

    End Function

    ''' <summary>
    ''' Updates Status of the Brands specified in the list (strCodeList).
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="strCodeList">The list of Brand Codes to be updated.</param>
    ''' <param name="bytStatus">The Status of the Brand.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal strCodeList As String, ByVal bytStatus As Byte) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, -1, -1, False, enumEgswTransactionMode.Deactivate, bytStatus, strCodeList)

    End Function

    ''' <summary>
    ''' Updates Status of a Brand.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the Brand to be updated.</param>
    ''' <param name="IsGlobal">The Global Status of the Brand.</param>
    ''' <param name="bytStatus">The Status of the Brand.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCode As Int32, ByVal IsGlobal As Boolean, _
        ByVal bytStatus As Byte) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, -1, lngCode, IsGlobal, enumEgswTransactionMode.Deactivate, bytStatus)

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
        ByVal lngCodeSite As Int32) As enumEgswErrorCode

        Dim cmd As New SqlCommand

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswBrandMovePos"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@vchCodeList", SqlDbType.VarChar, 8000).Value = strCodeList
                .Parameters.Add("@bitMoveUp", SqlDbType.TinyInt).Value = flagMoveUp
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite

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
        Finally
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
        End Try

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

    End Function

    '====================== 
    ' ADD / EDIT BRAND
    ' VRP 27.04.2009
    '====================== 
    Public Function UpdateBrand(ByRef intCode As Integer, ByVal udtBrand As structBrand, _
                                   ByVal intCodeSite As Integer, ByVal intCodeUser As Integer, _
                                   ByVal strCodeSiteList As String, ByVal arrTransCode() As String, _
                                   ByVal arrTransName() As String, ByVal intParent As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandType = CommandType.StoredProcedure
                .CommandText = "MANAGE_BRANDUPDATE"

                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@Code", SqlDbType.Int).Value = intCode
                .Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = udtBrand.Name
                .Parameters.Add("@ListeType", SqlDbType.Int).Value = udtBrand.Type
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = udtBrand.IsGlobal
                .Parameters.Add("@CodeSiteList", SqlDbType.VarChar, 8000).Value = strCodeSiteList
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@CodeParent", SqlDbType.Int).Value = IIf(intParent = 0, DBNull.Value, intParent)
                .Parameters.Add("@IsCanBeParent", SqlDbType.Bit).Value = udtBrand.IsCanBeParent
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue
                .Parameters("@Code").Direction = ParameterDirection.InputOutput

                .Connection.Open()
                .ExecuteNonQuery()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                intCode = CInt(.Parameters("@Code").Value)

                If L_ErrCode = enumEgswErrorCode.OK Then
                    Try
                        Dim i As Int32
                        Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

                        For i = 0 To arrTransCode.Length - 1
                            If IsNumeric(arrTransCode(i)) Then
                                L_ErrCode = oTrans.UpdateTranslation(intCode, arrTransName(i), CInt(arrTransCode(i)), udtBrand.Type, intCodeSite, intCodeUser, enumDataListType.Brand)
                                If L_ErrCode <> enumEgswErrorCode.OK Then Exit For
                            End If
                        Next

                    Catch ex As Exception
                        L_ErrCode = enumEgswErrorCode.GeneralError
                    End Try
                End If

                cmd.Dispose()
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            cmd.Dispose()
        Finally
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
        End Try
        Return L_ErrCode
    End Function

#End Region

#Region "Remove Methods"
    ''' <summary>
    ''' Purge Brand List.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove() As enumEgswErrorCode

        Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Purge)

    End Function

    ''' <summary>
    ''' Deletes a Brand.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the Brand to be deleted.</param>
    ''' <param name="IsGlobal">The Global status of the Brand to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
            ByVal lngCode As Int32, ByVal IsGlobal As Boolean) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, lngCodeSite, lngCode, IsGlobal, enumEgswTransactionMode.Delete)

    End Function

    ''' <summary>
    ''' Deletes Brands specified in the list strCodeList.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="strCodeList">The list of Brand Codes to be deleted.</param>
    ''' <param name="lngCodeSite">The CodeSite from were you are trying to delete, pass -1 if you want to delete using the user's role.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal strCodeList As String) As enumEgswErrorCode

        L_udtUser.Code = lngCodeUser
        Return RemoveFromList(lngCodeUser, lngCodeSite, 0, False, enumEgswTransactionMode.MultipleDelete, , strCodeList)

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

    Public Function GetCode(ByVal strName As String, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, Optional ByVal blnCommitToDbase As Boolean = False) As Integer
        If Trim(strName) = "" Then strName = "Not Defined"
        Dim tempFetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataTable
        Dim dt As DataTable = CType(GetList(strName, enumDataListType.Merchandise, intCodeSite, intCodeTrans, 255), DataTable)
        L_bytFetchType = tempFetchType

        Dim intCode As Integer = -1
        Dim rw As DataRow = dt.Rows(0)

        If Not IsDBNull(rw("code")) Then intCode = CInt(dt.Rows(0)("Code"))
        If Not blnCommitToDbase Then GoTo Done

        If intCode > -1 Then
            If CInt(dt.Rows(0)("Status")) <> 1 Then 'inactive
                Dim cItem As clsItem = New clsItem(L_udtUser, enumAppType.WebApp, L_strCnn)
                cItem.UpdateStatus(intCode, intCodeSite, enumDbaseTables.EgswBrand, 1)
            End If
        Else
            Dim brand As structBrand
            brand.Type = enumDataListItemType.Merchandise
            brand.Code = intCode
            brand.Name = strName
            brand.IsGlobal = False

            Update(L_udtUser.Code, intCodeSite, intCode, brand)
        End If
Done:
        Return intCode
    End Function


    Public Sub UpdateBrandBrabdSite(ByVal dtOldBrandBrandSite As DataTable, ByVal dtNewBrandBrandSite As DataTable, ByVal nBrandCode As Integer)
        Dim arrDelete As ArrayList = GetItemsRemoved("id", dtOldBrandBrandSite, dtNewBrandBrandSite)
        If arrDelete.Count > 0 Then
            Dim cmd As New SqlCommand
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandType = CommandType.StoredProcedure
                .CommandText = "DELETE_BrandBrandSite"

                For intIndex As Integer = 0 To arrDelete.Count - 1
                    Dim strBrandSite As String = arrDelete(intIndex)
                    .Parameters.Clear()
                    .Parameters.Add("@Brand", SqlDbType.Int, 4).Value = nBrandCode
                    .Parameters.Add("@BrandSite", SqlDbType.Int, 4).Value = CInt(strBrandSite)
                    .Connection.Open()
                    .ExecuteNonQuery()
                    .Connection.Close()
                Next
            End With
        End If

        Dim arrNew As ArrayList = GetItemsNew("id", dtOldBrandBrandSite, dtNewBrandBrandSite)
        If arrNew.Count > 0 Then
            Dim cmd As New SqlCommand
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandType = CommandType.StoredProcedure
                .CommandText = "UPDATE_BrandBrandSite"

                For intIndex As Integer = 0 To arrNew.Count - 1
                    Dim strBrandSite As String = arrNew(intIndex)
                    .Parameters.Clear()
                    .Parameters.Add("@Brand", SqlDbType.Int, 4).Value = nBrandCode
                    .Parameters.Add("@BrandSite", SqlDbType.Int, 4).Value = CInt(strBrandSite)
                    .Connection.Open()
                    .ExecuteNonQuery()
                    .Connection.Close()
                Next
            End With
        End If
    End Sub


    Private Function GetItemsRemoved(ByVal FieldName As String, ByVal dtOldDB As DataTable, ByVal dtNewDB As DataTable) As ArrayList
        Dim rowOldDB As DataRow
        Dim rowNewDB As DataRow
        Dim nRowOldDBFieldValue As String
        Dim nRowNewDBFieldValue As String
        Dim arrRemoveItems As New ArrayList

        For Each rowOldDB In dtOldDB.Rows
            nRowOldDBFieldValue = CStr(rowOldDB.Item(FieldName))
            For Each rowNewDB In dtNewDB.Rows
                nRowNewDBFieldValue = CStr(rowNewDB.Item(FieldName))
                If (nRowOldDBFieldValue = nRowNewDBFieldValue) Then
                    GoTo GoToNextRowInOldDB
                End If
            Next
            arrRemoveItems.Add(nRowOldDBFieldValue)
GoToNextRowInOldDB:
        Next

        Return arrRemoveItems

    End Function


    Private Function GetItemsNew(ByVal FieldName As String, ByVal dtOldDB As DataTable, ByVal dtNewDB As DataTable) As ArrayList
        Dim rowOldDB As DataRow
        Dim rowNewDB As DataRow
        Dim nRowOldDBFieldValue As String
        Dim nRowNewDBFieldValue As String
        Dim arrNewItems As New ArrayList
        For Each rowNewDB In dtNewDB.Rows
            nRowNewDBFieldValue = CStr(rowNewDB.Item(FieldName))
            For Each rowOldDB In dtOldDB.Rows
                nRowOldDBFieldValue = CStr(rowOldDB.Item(FieldName))
                If (nRowOldDBFieldValue = nRowNewDBFieldValue) Then
                    GoTo GoToNextRowInOldDB
                End If
            Next
            arrNewItems.Add(nRowNewDBFieldValue)
GoToNextRowInOldDB:
        Next

        Return arrNewItems

    End Function


    ''-- JBB 06.05.2012
    Public Sub GetBrandChildListe(intParentCode As Integer, ByVal strCollection As String, ByRef arrChildList As ArrayList)
        Dim arrTemp As ArrayList = GetBrandChildListofParent(intParentCode, strCollection)
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
            GetBrandChildListe(-1, strCollection, arrChildList)
            Exit Sub
        End If
    End Sub

#End Region


#Region "Brand Site"
    'GET_BrandSiteSharing
    Public Function GETBrandSiteSharing(ByVal strName As String, intCodeSite As Integer, intCodeTrans As Integer, Optional ByVal intID As Integer = -1) As DataTable
        Dim dtTable As New DataTable()
        Dim cmd As New SqlCommand()
        Dim conn As New SqlConnection(L_strCnn)
        Dim da As New SqlDataAdapter()
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "GET_BrandSiteSharing"
        If strName.Trim() <> "" Then
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 200).Value = strName
        End If
        If intID <> -1 Then
            cmd.Parameters.Add("@Code", SqlDbType.Int, 4).Value = intID
        End If
        cmd.Parameters.Add("@intCodeSite", SqlDbType.Int, 4).Value = intCodeSite
        cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int, 4).Value = intCodeTrans
        Try
            da.SelectCommand = cmd
            dtTable.BeginLoadData()
            da.Fill(dtTable)
            dtTable.EndLoadData()
        Catch ex As Exception
            dtTable = Nothing
        End Try
        Return dtTable
    End Function

    Public Function GetBrandSiteList(ByVal strName As String, Optional ByVal intID As Integer = -1, Optional intCodeSite As Integer = -1, Optional ByVal intCodeProperty As Integer = -1) As DataTable
        Dim dtTable As New DataTable()
        Dim cmd As New SqlCommand()
        Dim conn As New SqlConnection(L_strCnn)
        Dim da As New SqlDataAdapter()
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "GET_BrandSiteList"
        If strName.Trim() <> "" Then
            cmd.Parameters.Add("@nvrName", SqlDbType.NVarChar, 200).Value = strName
        End If
        If intID <> -1 Then
            cmd.Parameters.Add("@nID", SqlDbType.Int, 4).Value = intID
        End If
        cmd.Parameters.Add("@nCodeSite", SqlDbType.Int, 4).Value = intCodeSite
        cmd.Parameters.Add("@nCodeProperty", SqlDbType.Int).Value = intCodeProperty
        Try
            da.SelectCommand = cmd
            dtTable.BeginLoadData()
            da.Fill(dtTable)
            dtTable.EndLoadData()
        Catch ex As Exception
            dtTable = Nothing
        End Try
        Return dtTable
    End Function


    Public Function GetBrandSiteListwithSharing(ByVal strName As String, Optional ByVal intID As Integer = -1, Optional strCodeSites As String = "") As DataTable
        Dim dtTable As New DataTable()
        Dim cmd As New SqlCommand()
        Dim conn As New SqlConnection(L_strCnn)
        Dim da As New SqlDataAdapter()
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "GET_BrandSiteListbySharing"
        If strName.Trim() <> "" Then
            cmd.Parameters.Add("@nvrName", SqlDbType.NVarChar, 200).Value = strName
        End If
        If intID <> -1 Then
            cmd.Parameters.Add("@nID", SqlDbType.Int, 4).Value = intID
        End If
        cmd.Parameters.Add("@nCodeSites", SqlDbType.NVarChar, 200).Value = strCodeSites
        Try
            da.SelectCommand = cmd
            dtTable.BeginLoadData()
            da.Fill(dtTable)
            dtTable.EndLoadData()
        Catch ex As Exception
            dtTable = Nothing
        End Try
        Return dtTable
    End Function


    Public Function InsertBrandSite(ByRef intID As Integer, ByVal strName As String, ByVal strSearchName As String, strCodeSiteList As String, Optional blnIsGlobal As Boolean = False, Optional intMetImpBoth As Integer = 0) As Integer
        Dim cmd As New SqlCommand
        Dim conn As New SqlConnection(L_strCnn)
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "INSERT_BrandSite"
        cmd.Parameters.Add("@intId", SqlDbType.Int, 4).Value = intID
        cmd.Parameters.Add("@vrName", SqlDbType.NVarChar, 200).Value = strName 'AGL 2013.02.09 - changed to nvarchar
        cmd.Parameters.Add("@vrSearch", SqlDbType.NVarChar, 200).Value = strSearchName 'AGL 2013.02.09 - changed to nvarchar
        cmd.Parameters.Add("@strCodeSiteList", SqlDbType.NVarChar, 4000).Value = strCodeSiteList 'AGL 2013.06.07 - added codesitelist
        cmd.Parameters.Add("@isGlobal", SqlDbType.Bit).Value = blnIsGlobal 'JTOC 11.06.2013 - added isGlobal
        cmd.Parameters.Add("@intMetImpBoth", SqlDbType.SmallInt).Value = intMetImpBoth  'JTOC 30.07.2013

        cmd.Parameters.Add("@retval", SqlDbType.Int)
        cmd.Parameters("@retval").Direction = ParameterDirection.ReturnValue
        cmd.Parameters("@intId").Direction = ParameterDirection.InputOutput
        cmd.Connection.Open()
        cmd.ExecuteNonQuery()
        cmd.Connection.Close()
        Dim intValue As Integer = CInt(cmd.Parameters("@retval").Value)
        intID = CInt(cmd.Parameters("@intId").Value)
        Return intValue
    End Function

    Public Function DeleteBrandSite(ByVal intID As Integer, ByVal intCodeUser As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Dim conn As New SqlConnection(L_strCnn)
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "DELETE_BrandSite"
        cmd.Parameters.Add("@intId", SqlDbType.Int, 4).Value = intID
        cmd.Parameters.Add("@intCodeUser", SqlDbType.Int, 4).Value = intCodeUser 'AGL 2013.11.29 - added codeUser
        cmd.Parameters.Add("@retval", SqlDbType.Int)
        cmd.Parameters("@retval").Direction = ParameterDirection.ReturnValue
        cmd.Connection.Open()
        cmd.ExecuteNonQuery()
        cmd.Connection.Close()
        Return CType(cmd.Parameters("@retval").Value, enumEgswErrorCode)
    End Function

    Public Sub UpdateNutrientBrandSiteDisplay(ByVal intID As Integer, ByVal strNPos As String, ByVal IsDisplay As Boolean)
        Dim cmd As New SqlCommand
        Dim conn As New SqlConnection(L_strCnn)
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "UPDATE_BrandSiteNutrientDisplay"
        cmd.Parameters.Add("@nID", SqlDbType.Int, 4).Value = intID
        cmd.Parameters.Add("@nvN", SqlDbType.VarChar, 10).Value = strNPos
        cmd.Parameters.Add("@bDisplay", SqlDbType.Bit, 1).Value = IsDisplay
        cmd.Connection.Open()
        cmd.ExecuteNonQuery()
        cmd.Connection.Close()
    End Sub

    Public Function UpdateBrandSiteSharing(ByVal intCode As Integer, ByVal intCodeSite As Integer, _
                                     ByVal strCodeSharedTo As String, ByVal intCodeEgswTable As enumDbaseTables, Optional ByVal blnGlobal As Boolean = True) As enumEgswErrorCode
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
                .Parameters.Add("@isGlobal", SqlDbType.Bit)
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
                            .Parameters("@isGlobal").Value = blnGlobal
                            .ExecuteNonQuery()
                        End If
                    Next
                Else
                    .Parameters("@intCode").Value = intCode
                    .Parameters("@intCodeSite").Value = intCodeSite
                    .Parameters("@intCodeSitesShared").Value = CInt(strCodeSharedTo)
                    .Parameters("@intCodeEgswTable").Value = intCodeEgswTable
                    .Parameters("@isGlobal").Value = blnGlobal
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

    Public Function ProcessDeleteBrandSite(ByVal intID As Integer, ByVal codeLang As Integer, ByVal strName As String, ByRef strOK As String, ByRef strInUsed As String, ByVal intCodeUser As Integer) As String
        Dim eErrCode As enumEgswErrorCode
        eErrCode = DeleteBrandSite(intID, intCodeUser) '' Remove(intcodeUser, intCodeSite, "(" + strID + ")")
        If eErrCode <> enumEgswErrorCode.OK And eErrCode <> enumEgswErrorCode.FK Then
            Dim strMsg As String = ""
            Dim cNotes As clsNotes = New clsNotes(eErrCode, strMsg, codeLang)
            ProcessDeleteBrandSite = strMsg
            Exit Function
        End If
        If eErrCode = enumEgswErrorCode.FK Then
            strInUsed += "-" & strName & vbCrLf
        Else
            strOK += "-" & strName & vbCrLf
        End If
        Return ""
    End Function

    Public Function ExportBrandSite(intcodeUser As Integer, intCodeBrandSite As Integer) As Integer
        Dim cmd As New SqlCommand
        Dim conn As New SqlConnection(L_strCnn)
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "FlagBrandSiteRecipesForExport"
        cmd.Parameters.Add("@CodeUser", SqlDbType.Int, 4).Value = intcodeUser
        cmd.Parameters.Add("@CodeBrandSite", SqlDbType.Int, 4).Value = intCodeBrandSite
        cmd.Parameters.Add("@Retval", SqlDbType.Int)
        cmd.Parameters("@Retval").Direction = ParameterDirection.ReturnValue
        cmd.Connection.Open()
        cmd.ExecuteNonQuery()
        cmd.Connection.Close()
        Dim intReturn As Integer = CInt(cmd.Parameters("@Retval").Value)
        Return intReturn
    End Function

#End Region


#Region "For USA"

    Public Function GetListBrandCodeName(ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, Optional ByVal flagActiveOnly As Boolean = True) As Object
        Dim strCommandText As String = "[GET_BRANDCODENAME]"

        '@ListeType int,
        '@CodeSite int,
        '@CodeTrans int,
        '@ActiveOnly bit =1

        ''Dim arrParam(2) As SqlParameter
        ''arrParam(0) = New SqlParameter("@CodeSite", intCodeSite)
        ''arrParam(1) = New SqlParameter("@CodeTrans", intCodeTrans)
        ''arrParam(2) = New SqlParameter("@ActiveOnly", flagActiveOnly)

        Dim dtTable As New DataTable()
        Dim cmd As New SqlCommand()
        Dim conn As New SqlConnection(L_strCnn)
        Dim da As New SqlDataAdapter()
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = strCommandText
        cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
        cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
        cmd.Parameters.Add("@ActiveOnly", SqlDbType.Bit).Value = flagActiveOnly
        Try
            da.SelectCommand = cmd
            dtTable.BeginLoadData()
            da.Fill(dtTable)
            dtTable.EndLoadData()
        Catch ex As Exception
            dtTable = Nothing
        End Try
        Return dtTable
    End Function

    '-- JBB 01.18.2012
    '-- Get Pacement Child
    Public Function GetBrandbyparent(ByVal intCodeParent As Integer, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, Optional ByVal flagActiveOnly As Boolean = True)
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
                .CommandText = "[GET_BRANDBYPARENT]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@CodeBrand", SqlDbType.Int).Value = intCodeParent
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@ActiveOnly", SqlDbType.Bit).Value = flagActiveOnly

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

#End Region

#Region "BrandSite Keywords"
	Public Function GetBrandSiteKeywords(ByVal intCodeBrandSite As Integer, intCodeTrans As Integer) As DataTable
		Dim dt As New DataTable
		Dim cmd As New SqlCommand
		Dim da As New SqlDataAdapter
		With cmd
			.Connection = New SqlConnection(L_strCnn)
			.CommandText = "Get_KeywordsBrandSite"
			.CommandType = CommandType.StoredProcedure
			.Parameters.Add("@CodeKeyword", SqlDbType.Int).Value = 0
			.Parameters.Add("@CodeBrandSite", SqlDbType.Int).Value = intCodeBrandSite
			.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans

		End With
		With da
			.SelectCommand = cmd
			dt.BeginLoadData()
			.Fill(dt)
			dt.EndLoadData()
		End With
		Return dt
	End Function

	Public Sub UpdateKeywordBrandSite(ByVal dtOldKeywordBrandSite As DataTable, ByVal dtNewKeywordBrandSite As DataTable, ByVal nBrandCode As Integer)
		Dim arrDelete As ArrayList = GetItemsRemoved("id", dtOldKeywordBrandSite, dtNewKeywordBrandSite)
		If arrDelete.Count > 0 Then
			Dim cmd As New SqlCommand
			With cmd
				.Connection = New SqlConnection(L_strCnn)
				.CommandType = CommandType.StoredProcedure
				.CommandText = "DELETE_KeywordsBrandSite"

				For intIndex As Integer = 0 To arrDelete.Count - 1
					Dim strBrandSite As String = arrDelete(intIndex)
					.Parameters.Clear()
					.Parameters.Add("@CodeKeyword", SqlDbType.Int, 4).Value = CInt(strBrandSite)
					.Parameters.Add("@CodeBrandSite", SqlDbType.Int, 4).Value = nBrandCode
					.Connection.Open()
					.ExecuteNonQuery()
					.Connection.Close()
				Next
			End With
		End If

		Dim arrNew As ArrayList = GetItemsNew("id", dtOldKeywordBrandSite, dtNewKeywordBrandSite)
		If arrNew.Count > 0 Then
			Dim cmd As New SqlCommand
			With cmd
				.Connection = New SqlConnection(L_strCnn)
				.CommandType = CommandType.StoredProcedure
				.CommandText = "UPDATE_KeywordsBrandSite"

				For intIndex As Integer = 0 To arrNew.Count - 1
					Dim strKeyCode As String = arrNew(intIndex)
					.Parameters.Clear()
					.Parameters.Add("@CodeKeyword", SqlDbType.Int, 4).Value = CInt(strKeyCode)
					.Parameters.Add("@CodeBrandSite", SqlDbType.Int, 4).Value = nBrandCode
					.Connection.Open()
					.ExecuteNonQuery()
					.Connection.Close()
				Next
			End With
		End If
	End Sub

#End Region

End Class



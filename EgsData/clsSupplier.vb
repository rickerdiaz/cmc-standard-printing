Imports System.Data.SqlClient
Imports System.Data

''' <summary>
''' Manages Supplier Table
''' </summary>
''' <remarks></remarks>

Public Class clsSupplier
#Region "Class Header"
    'Name               : clsSupplier
    'Decription         : Manages Supplier Table
    'Date Created       : 07.09.2005
    'Author             : VBV
    'Revision History   : VBV - 20.09.2005 - Accepts a connection string as opposed to a connection object. Class performs on a disconnected state.
    '                     VBV - 23.09.2005 - Assign user upon creation of object, expose CodeUser
    '                     VBV - 30.09.2005 - Added overload method GetList(ByVal strName As String, ByVal eType As enumDataListType)
    '                     VBV - 20.12.2005 - Added fetch list for getting supplier with marked status
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
    Public Sub New(ByVal udtUser As structUser, ByVal eAppType As enumAppType, ByVal strCnn As String, _
        Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)

        Try
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
        ClearMarkings() 'items marked as not deleted
        MyBase.Finalize()
    End Sub

    Public ReadOnly Property AppType() As enumAppType
        Get
            AppType = L_AppType
        End Get
    End Property

    Public ReadOnly Property ItemsNotDeleted() As Object  'DataTable
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
            ByVal EFilterMark As FilterMark) As Object
        'vbv 20.12.2005
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
                .CommandText = "SUP_GetListMark"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@flagGetMark", SqlDbType.TinyInt).Value = EFilterMark                
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

    Private Function FetchList(ByVal lngCodeSite As Int32, ByVal lngCode As Int32, _
        ByVal bytStatus As Integer, Optional ByVal strName As String = "", Optional ByVal flagIncludeAll As Boolean = False) As Object  'DataTable

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
                .Connection = New SqlConnection(L_strCnn)
                'End If
                .CommandText = "SUP_GetList"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@Status", SqlDbType.TinyInt).Value = bytStatus
                If strName <> "" Then
                    .Parameters.Add("@vchName", SqlDbType.NVarChar, 15).Value = strName
                End If
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
                        rX.Item("NameRef") = "*All supplier*"
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
                        rX.Item("NameRef") = "*All supplier*"
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

    Private Function FetchListContact(ByVal lngCodeSite As Int32, ByVal lngCode As Int32, _
Optional ByVal lngCodeSupplier As Int32 = -1, Optional ByVal strName As String = "") As Object  'DataTable

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
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "SUP_ContactGetList"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@intCodeSupplier", SqlDbType.Int).Value = lngCodeSupplier
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                If strName <> "" Then
                    .Parameters.Add("@nvcLName", SqlDbType.NVarChar, 15).Value = strName
                End If
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

    Private Function FetchListGroup(ByVal lngCodeSite As Int32, ByVal lngCode As Int32, _
     ByVal bytStatus As Integer, Optional ByVal strName As String = "", Optional ByVal flagIncludeAll As Boolean = False) As Object  'DataTable

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
                .Connection = New SqlConnection(L_strCnn)
                'End If
                .CommandText = "SUP_GroupGetList"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@Status", SqlDbType.TinyInt).Value = bytStatus
                If strName <> "" Then
                    .Parameters.Add("@nvchName", SqlDbType.NVarChar, 15).Value = strName
                End If
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
                        rX.Item("NameRef") = "*All supplier*"
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
                        rX.Item("NameRef") = "*All supplier*"
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

    Private Function SaveIntoList(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByRef lngCode As Int32, ByVal udtSupplier As structSupplier, ByVal strCodeSiteList As String, _
        ByVal strCodeSupplierList As String, ByVal TranMode As enumEgswTransactionMode, _
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
                .CommandText = "SUP_Update"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@intCode", SqlDbType.Int).Value = udtSupplier.Code

                .Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 15).Value = udtSupplier.Number
                .Parameters.Add("@nvcNameRef", SqlDbType.NVarChar, 15).Value = udtSupplier.NameRef
                .Parameters.Add("@nvcCompany", SqlDbType.NVarChar, 50).Value = udtSupplier.Company
                .Parameters.Add("@nvcURL", SqlDbType.NVarChar, 50).Value = udtSupplier.URL
                .Parameters.Add("@nvcNote", SqlDbType.NVarChar, 2000).Value = udtSupplier.Note
                .Parameters.Add("@nvcTerms", SqlDbType.NVarChar, 2000).Value = IIf(udtSupplier.Terms Is Nothing, "", udtSupplier.Terms)
                .Parameters.Add("@UseDefaultTerms", SqlDbType.Bit).Value = udtSupplier.UseDefaultTerms
                .Parameters.Add("@nvcAcctRef", SqlDbType.NVarChar, 20).Value = IIf(udtSupplier.AccountingRef Is Nothing, "", udtSupplier.AccountingRef)
                .Parameters.Add("@nvcAddress1", SqlDbType.NVarChar, 200).Value = udtSupplier.Address1
                .Parameters.Add("@nvcAddress2", SqlDbType.NVarChar, 200).Value = udtSupplier.Address2
                .Parameters.Add("@WithTax", SqlDbType.Bit).Value = udtSupplier.WithTax
                .Parameters.Add("@nvcCity", SqlDbType.NVarChar, 30).Value = udtSupplier.City_1
                .Parameters.Add("@nvcZip", SqlDbType.NVarChar, 15).Value = udtSupplier.Zip_1
                .Parameters.Add("@nvcCountry", SqlDbType.NVarChar, 30).Value = udtSupplier.Country_1
                .Parameters.Add("@nvcState", SqlDbType.NVarChar, 30).Value = udtSupplier.State_1
                .Parameters.Add("@nvcTel", SqlDbType.NVarChar, 15).Value = udtSupplier.Tel
                .Parameters.Add("@nvcFax", SqlDbType.NVarChar, 15).Value = udtSupplier.Fax
                .Parameters.Add("@nvcEmail", SqlDbType.NVarChar, 50).Value = udtSupplier.Email
                .Parameters.Add("@nvcCity2", SqlDbType.NVarChar, 30).Value = IIf(udtSupplier.City_2 Is Nothing, "", udtSupplier.City_2)
                .Parameters.Add("@nvcZip2", SqlDbType.NVarChar, 15).Value = IIf(udtSupplier.Zip_2 Is Nothing, "", udtSupplier.Zip_2)
                .Parameters.Add("@nvcCountry2", SqlDbType.NVarChar, 30).Value = IIf(udtSupplier.Country_2 Is Nothing, "", udtSupplier.Country_2)
                .Parameters.Add("@nvcState2", SqlDbType.NVarChar, 30).Value = IIf(udtSupplier.State_2 Is Nothing, "", udtSupplier.State_2)
                .Parameters.Add("@nvcRemark", SqlDbType.NVarChar, 30).Value = udtSupplier.Remark
                .Parameters.Add("@intCodeSupplierGroup", SqlDbType.Int).Value = udtSupplier.CodeSupplierGroup
                .Parameters.Add("@AddFlag", SqlDbType.Bit).Value = udtSupplier.AddFlag
                .Parameters.Add("@UpdateFlag", SqlDbType.Bit).Value = udtSupplier.UpdateFlag
                .Parameters.Add("@ImportFlag", SqlDbType.Bit).Value = udtSupplier.ImportFlag
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = udtSupplier.IsGlobal
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

                strCodeSupplierList.Trim()
                If strCodeSupplierList <> "" Then
                    If Not (strCodeSupplierList.StartsWith("(") And strCodeSupplierList.EndsWith(")")) Then
                        Return enumEgswErrorCode.InvalidCodeList
                    Else
                        .Parameters.Add("@vchCodeMergeList", SqlDbType.VarChar, 8000).Value = strCodeSupplierList
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

    Private Function SaveIntoListContact(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
ByRef lngCode As Int32, ByVal udtSupplier As structSupplier, ByVal strCodeSiteList As String, _
ByVal strCodeClientList As String, ByVal TranMode As enumEgswTransactionMode, _
Optional ByVal oTransaction As SqlTransaction = Nothing) As enumEgswErrorCode


        Dim cmd As New SqlCommand
        Try
            With cmd
                If oTransaction Is Nothing Then
                    .Connection = New SqlConnection(L_strCnn)
                Else
                    .Connection = oTransaction.Connection
                    .Transaction = oTransaction
                End If
                .CommandText = "SUP_ContactUpdate"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@intCodeSupplier", SqlDbType.Int).Value = udtSupplier.Code
                .Parameters.Add("@intCode", SqlDbType.Int).Value = udtSupplier.CodeSupplierContact
                .Parameters.Add("@nvcLName", SqlDbType.NVarChar, 30).Value = udtSupplier.LName
                .Parameters.Add("@nvcFName", SqlDbType.NVarChar, 30).Value = udtSupplier.FName
                .Parameters.Add("@nvcTitle", SqlDbType.NVarChar, 30).Value = udtSupplier.Title
                .Parameters.Add("@nvcJobPosition", SqlDbType.NVarChar, 100).Value = udtSupplier.JobPosition
                .Parameters.Add("@nvcTel", SqlDbType.NVarChar, 15).Value = udtSupplier.ContactTel
                .Parameters.Add("@nvcFax", SqlDbType.NVarChar, 15).Value = udtSupplier.ContactFax
                .Parameters.Add("@nvcMobile", SqlDbType.NVarChar, 15).Value = udtSupplier.ContactMobile
                .Parameters.Add("@nvcEmail", SqlDbType.NVarChar, 50).Value = udtSupplier.ContactEmail
                .Parameters.Add("@nvcNote", SqlDbType.NVarChar, 2000).Value = udtSupplier.Note
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = udtSupplier.IsGlobal
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

                strCodeClientList.Trim()
                If strCodeClientList <> "" Then
                    If Not (strCodeClientList.StartsWith("(") And strCodeClientList.EndsWith(")")) Then
                        Return enumEgswErrorCode.InvalidCodeList
                    Else
                        .Parameters.Add("@vchCodeMergeList", SqlDbType.VarChar, 8000).Value = strCodeClientList
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

    Private Function SaveIntoListGroup(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByRef lngCode As Int32, ByVal udtSupplier As structSupplier, ByVal strCodeSiteList As String, _
        ByVal strCodeSupplierList As String, ByVal TranMode As enumEgswTransactionMode, _
        Optional ByVal oTransaction As SqlTransaction = Nothing) As enumEgswErrorCode


        Dim cmd As New SqlCommand
        Try
            With cmd
                If oTransaction Is Nothing Then
                    .Connection = New SqlConnection(L_strCnn)
                Else
                    .Connection = oTransaction.Connection
                    .Transaction = oTransaction
                End If
                .CommandText = "SUP_GroupUpdate"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 100).Value = udtSupplier.GroupName
                .Parameters.Add("@nvcNote", SqlDbType.NVarChar, 2000).Value = udtSupplier.Note
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = udtSupplier.IsGlobal
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

                strCodeSupplierList.Trim()
                If strCodeSupplierList <> "" Then
                    If Not (strCodeSupplierList.StartsWith("(") And strCodeSupplierList.EndsWith(")")) Then
                        Return enumEgswErrorCode.InvalidCodeList
                    Else
                        .Parameters.Add("@vchCodeMergeList", SqlDbType.VarChar, 8000).Value = strCodeSupplierList
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
        Optional ByVal bytStatus As Integer = 0, Optional ByVal strCodeList As String = "") As enumEgswErrorCode

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
                .CommandText = "SUP_Delete"
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
                cmd.Parameters.Add("@vchTableName", SqlDbType.VarChar, 50).Value = "EgswSupplier"

                L_dtList = New DataTable
                With da
                    .SelectCommand = cmd
                    L_dtList.BeginLoadData()
                    .Fill(L_dtList)
                    L_dtList.EndLoadData()
                End With
            Catch ex As Exception
                L_dtList = Nothing
                If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
                cmd.Dispose()
                Throw New Exception(ex.Message, ex)
            End Try
        End If

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

    End Function

    Private Function RemoveFromListContact(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
ByVal lngCode As Int32, ByVal IsGlobal As Boolean, ByVal TranMode As enumEgswTransactionMode, _
Optional ByVal bytStatus As Integer = 0, Optional ByVal strCodeList As String = "") As enumEgswErrorCode

        Dim cmd As New SqlCommand
        Dim lngCodeProperty As Int32

        If L_udtUser.RoleLevelHighest = 0 Then 'Unshare to ALL 
            lngCodeProperty = -1
        Else 'Unshare to ALL sites belonging to a property or Unshare to self
            lngCodeProperty = L_udtUser.Site.Group
        End If
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "SUP_ContactDelete"
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
                cmd.Parameters.Add("@vchTableName", SqlDbType.VarChar, 50).Value = "EgswSupplierContact"

                L_dtList = New DataTable
                With da
                    .SelectCommand = cmd
                    L_dtList.BeginLoadData()
                    .Fill(L_dtList)
                    L_dtList.EndLoadData()
                End With
            Catch ex As Exception
                L_dtList = Nothing
                If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
                cmd.Dispose()
                Throw New Exception(ex.Message, ex)
            End Try
        End If

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

    End Function


    Private Function RemoveFromListGroup(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByVal lngCode As Int32, ByVal IsGlobal As Boolean, ByVal TranMode As enumEgswTransactionMode, _
        Optional ByVal bytStatus As Integer = 0, Optional ByVal strCodeList As String = "") As enumEgswErrorCode

        Dim cmd As New SqlCommand
        Dim lngCodeProperty As Int32

        If L_udtUser.RoleLevelHighest = 0 Then 'Unshare to ALL 
            lngCodeProperty = -1
        Else 'Unshare to ALL sites belonging to a property or Unshare to self
            lngCodeProperty = L_udtUser.Site.Group
        End If


        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "SUP_GroupDelete"
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
                cmd.Parameters.Add("@vchTableName", SqlDbType.VarChar, 50).Value = "EgswSupplierGroup"

                L_dtList = New DataTable
                With da
                    .SelectCommand = cmd
                    L_dtList.BeginLoadData()
                    .Fill(L_dtList)
                    L_dtList.EndLoadData()
                End With
            Catch ex As Exception
                L_dtList = Nothing
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
        If L_udtUser.Code <> -1 And L_lngCodeSite <> -1 Then
            Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Deactivate)
        End If
    End Function

#End Region

#Region "Get Methods"

    ''' <summary>
    ''' Get all Suppliers with the list of Site names to which they are shared to.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList() As Object  'DataTable

        Return FetchList(-1, -1, 255)

    End Function

    ''' <summary>
    ''' Get a Supplier by Name.
    ''' </summary>
    ''' <param name="strName">The name of the Supplier to be fetched.</param>    
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal strName As String) As Object

        Return FetchList(-1, -1, 255, strName)

    End Function

    ''' <summary>
    ''' Get a Supplier by Code.
    ''' </summary>
    ''' <param name="lngCode">The Code of the Supplier to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal lngCode As Int32) As Object  'DataTable

        Return FetchList(-1, lngCode, 255)

    End Function

    ''' <summary>
    ''' Get all Suppliers shared to a specific site and filtered by status.
    ''' </summary>
    ''' <param name="bytStatus">The status of the Suppliers to be fetched.</param>    
    ''' <param name="lngCodeSite">The CodeSite where the Suppliers is shared to.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal bytStatus As Integer, ByVal lngCodeSite As Int32, Optional ByVal flagIncludeAll As Boolean = False) As Object  'DataTable
        'Get all by Status
        Return FetchList(lngCodeSite, -1, bytStatus, , flagIncludeAll)

    End Function

    ''' <summary>
    ''' Get a Supplier by Name w/in the codesite.
    ''' </summary>
    ''' <param name="strName">The name of the Supplier to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal strName As String, ByVal intCodeSite As Integer, ByVal bytStatus As Integer) As Object

        Return FetchList(intCodeSite, -1, bytStatus, strName)

    End Function

    ''' <summary>
    ''' Get suppliers by site for markings.
    ''' </summary>
    ''' <param name="intCodeUser">The code of the user.</param>
    ''' <param name="intCodeSite">The code of the site.</param>    
    ''' <param name="EMarkFilter">One of FilterMark values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetListMark(ByVal intCodeUser As Int32, ByVal intCodeSite As Integer, ByVal EMarkFilter As FilterMark) As Object
        'vbv 20.12.2005
        Return FetchListMark(intCodeUser, intCodeSite, EMarkFilter)

    End Function

    ''' <summary>
    ''' Get a ClientSupplier by CodeSite and CodeSupplier
    ''' </summary>
    ''' <param name="lngCodeSupplier">The Code of the Supplier to be fetched.</param>
    ''' <param name="lngCodeSite">The Code of the Client Site to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetListContact(ByVal lngCodeSupplier As Int32, ByVal lngCodeSite As Int32) As Object  'DataTable

        Return FetchListContact(lngCodeSite, -1, lngCodeSupplier)

    End Function

    ''' <summary>
    ''' Get a Client Contact by Code.
    ''' </summary>
    ''' <param name="lngCode">The Code of the Client Contact to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetListContact(ByVal lngCode As Int32) As Object  'DataTable

        Return FetchListContact(-1, lngCode, 255)

    End Function

    ''' <summary>
    ''' Get a Supplier Group by Code.
    ''' </summary>
    ''' <param name="lngCode">The Code of the Supplier Group to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetListGroup(ByVal lngCode As Int32) As Object

        Return FetchListGroup(-1, lngCode, 255)

    End Function
    ''' <summary>
    ''' Get a Supplier Group by Site and status
    ''' </summary>
    ''' <param name="bytStatus">The status of the Supplier Group to be fetched.</param> 
    ''' <param name="lngCodeSite">The CodeSite of the Supplier Group to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetListGroup(ByVal bytStatus As Integer, ByVal lngCodeSite As Int32) As Object
        '
        Return FetchListGroup(-1, -1, 255)

    End Function

    Public Function GetListSupplier(ByVal intCodeSite As Integer, ByVal intStatus As Integer, Optional ByVal intCodeProperty As Integer = -1) As Object
        Dim strCommandText As String = "[GET_SupplierList]"

        '@ListeType int,
        '@CodeSite int,
        '@CodeTrans int,
        '@ActiveOnly bit =1

        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@Status", intStatus)
        arrParam(2) = New SqlParameter("@CodeProperty", intCodeProperty)

        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListSupplierCodeName(ByVal intCodeSite As Integer, Optional ByVal flagActiveOnly As Boolean = True) As Object
        Dim strCommandText As String = "[GET_SupplierCODENAME]"

        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@ActiveOnly", flagActiveOnly)
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function
#End Region

#Region "Update Methods"

    ''' <summary>
    ''' Updates the global status of a Supplier.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the Supplier to be updated.</param>
    ''' <param name="IsGlobal">The global status of the Supplier to be updated.</param>
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
                .CommandText = "sp_EgswSupplierUpdateGlobal"
                .CommandType = CommandType.StoredProcedure
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
    ''' Updates Supplier without sharing it to any sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the Supplier to be updated.</param>
    ''' <param name="udtSupplier">Supplier info.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByRef lngCode As Int32, ByVal udtSupplier As structSupplier) As enumEgswErrorCode

        Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtSupplier, "(" & lngCodeSite & ")", "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    ''' <summary>
    ''' Updates Supplier sharing it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtSupplier">Supplier info.</param>
    ''' <param name="strCodeSiteList">The list of sites where Supplier will be shared.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtSupplier As structSupplier, ByVal strCodeSiteList As String) As enumEgswErrorCode

        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtSupplier, strCodeSiteList, "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    ''' <summary>
    ''' Merge Suppliers
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="strCodeSupplierList">The list of Supplier Codes to be merged.</param>
    ''' <param name="udtSupplier">Supplier info.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByVal strCodeSupplierList As String, ByVal udtSupplier As structSupplier) As enumEgswErrorCode

        Return SaveIntoList(lngCodeUser, lngCodeSite, 0, udtSupplier, "", strCodeSupplierList, enumEgswTransactionMode.MergeDelete)

    End Function


    ''' <summary>
    ''' Updates Status of the Suppliers specified in the list (strCodeList).
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="strCodeList">The list of Supplier Codes to be updated.</param>
    ''' <param name="bytStatus">The Status of the Supplier.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal strCodeList As String, ByVal bytStatus As Integer) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, lngCodeSite, -1, False, enumEgswTransactionMode.Deactivate, bytStatus, strCodeList)

    End Function

    ''' <summary>
    ''' Updates Status of a Supplier.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the Supplier to be updated.</param>
    ''' <param name="IsGlobal">The Global Status of the Supplier.</param>
    ''' <param name="bytStatus">The Status of the Supplier.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal lngCode As Int32, ByVal IsGlobal As Boolean, _
        ByVal bytStatus As Integer) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, lngCodeSite, lngCode, IsGlobal, enumEgswTransactionMode.Deactivate, bytStatus)

    End Function

    ''' <summary>
    ''' Updates Client without sharing it to any sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the Client to be updated.</param>
    ''' <param name="udtSupplier">Client info.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function UpdateContact(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByRef lngCode As Int32, ByVal udtSupplier As structSupplier) As enumEgswErrorCode

        Return SaveIntoListContact(lngCodeUser, lngCodeSite, lngCode, udtSupplier, "", "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    ''' <summary>
    ''' Updates Client sharing it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtSupplier">Client info.</param>
    ''' <param name="strCodeSiteList">The list of sites where Client will be shared.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function UpdateContact(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtSupplier As structSupplier, ByVal strCodeSiteList As String) As enumEgswErrorCode

        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        Return SaveIntoListContact(lngCodeUser, lngCodeSite, lngCode, udtSupplier, strCodeSiteList, "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function


    ''' <summary>
    ''' Updates Client without sharing it to any sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the Client to be updated.</param>
    ''' <param name="udtSupplier">Client info.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function UpdateGroup(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByRef lngCode As Int32, ByVal udtSupplier As structSupplier) As enumEgswErrorCode

        Return SaveIntoListContact(lngCodeUser, lngCodeSite, lngCode, udtSupplier, "", "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    ''' <summary>
    ''' Updates Client sharing it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtSupplier">Client info.</param>
    ''' <param name="strCodeSiteList">The list of sites where Client will be shared.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function UpdateGroup(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtSupplier As structSupplier, ByVal strCodeSiteList As String) As enumEgswErrorCode

        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        Return SaveIntoListGroup(lngCodeUser, lngCodeSite, lngCode, udtSupplier, strCodeSiteList, "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function


#End Region

#Region "Remove Methods"
    ''' <summary>
    ''' Purge Supplier List.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove() As enumEgswErrorCode

        Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Purge)

    End Function

    ''' <summary>
    ''' Deletes a Supplier.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>    
    ''' <param name="lngCode">The Code of the Supplier to be deleted.</param>
    ''' <param name="IsGlobal">The Global status of the Supplier to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, _
        ByVal lngCode As Int32, ByVal IsGlobal As Boolean) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, -1, lngCode, IsGlobal, enumEgswTransactionMode.Delete)

    End Function

    ''' <summary>
    ''' Deletes Suppliers specified in the list strCodeList.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>    
    ''' <param name="strCodeList">The list of Supplier Codes to be deleted.</param>
    ''' <param name="lngCodeSite">The CodeSite from were you are trying to delete, pass -1 if you want to delete using the user's role.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngcodeSite As Int32, ByVal strCodeList As String) As enumEgswErrorCode

        L_udtUser.Code = lngCodeUser
        'L_lngCodeSite = lngCodeSite
        Return RemoveFromList(lngCodeUser, lngcodeSite, 0, False, enumEgswTransactionMode.MultipleDelete, , strCodeList)

    End Function

    ''' <summary>
    ''' Deletes a Supplier Contact.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>    
    ''' <param name="lngCode">The Code of the Client to be deleted.</param>
    ''' <param name="IsGlobal">The Global status of the Supplier to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function RemoveContact(ByVal lngCodeUser As Int32, _
        ByVal lngCode As Int32, ByVal IsGlobal As Boolean) As enumEgswErrorCode

        Return RemoveFromListContact(lngCodeUser, -1, lngCode, IsGlobal, enumEgswTransactionMode.Delete)

    End Function

    ''' <summary>
    ''' Deletes Supplier Contacts specified in the list strCodeList.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>    
    ''' <param name="strCodeList">The list of Supplier Codes to be deleted.</param>
    ''' <param name="lngCodeSite">The CodeSite from were you are trying to delete, pass -1 if you want to delete using the user's role.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function RemoveContact(ByVal lngCodeUser As Int32, ByVal lngcodeSite As Int32, ByVal strCodeList As String) As enumEgswErrorCode

        L_udtUser.Code = lngCodeUser
        Return RemoveFromListContact(lngCodeUser, lngcodeSite, 0, False, enumEgswTransactionMode.MultipleDelete, , strCodeList)

    End Function

    ''' <summary>
    ''' Purge Client Contact List.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function RemoveContact() As enumEgswErrorCode

        Return RemoveFromListContact(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Purge)

    End Function

    ''' <summary>
    ''' Deletes a Supplier Contact.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>    
    ''' <param name="lngCode">The Code of the Client to be deleted.</param>
    ''' <param name="IsGlobal">The Global status of the Supplier to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function RemoveGroup(ByVal lngCodeUser As Int32, _
        ByVal lngCode As Int32, ByVal IsGlobal As Boolean) As enumEgswErrorCode

        Return RemoveFromListGroup(lngCodeUser, -1, lngCode, IsGlobal, enumEgswTransactionMode.Delete)

    End Function

    ''' <summary>
    ''' Deletes Supplier Contacts specified in the list strCodeList.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>    
    ''' <param name="strCodeList">The list of Supplier Codes to be deleted.</param>
    ''' <param name="lngCodeSite">The CodeSite from were you are trying to delete, pass -1 if you want to delete using the user's role.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function RemoveGroup(ByVal lngCodeUser As Int32, ByVal lngcodeSite As Int32, ByVal strCodeList As String) As enumEgswErrorCode

        L_udtUser.Code = lngCodeUser
        Return RemoveFromListGroup(lngCodeUser, lngcodeSite, 0, False, enumEgswTransactionMode.MultipleDelete, , strCodeList)

    End Function

    ''' <summary>
    ''' Purge Client Contact List.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function RemoveGroup() As enumEgswErrorCode

        Return RemoveFromListGroup(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Purge)

    End Function

#End Region

#Region " Other Function "

    Public Function GetOne(ByVal intCode As Integer) As DataRow
        Dim tempFetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataSet
        Dim ds As DataSet = CType(GetList(intCode), DataSet)
        L_bytFetchType = tempFetchType

        Dim dt As DataTable = ds.Tables(1)
        If dt.DefaultView.Count = 0 Then Return Nothing

        Return dt.Rows(0)
    End Function

    Public Function GetOneGroup(ByVal intCode As Integer, Optional ByVal intCodeTrans As Integer = -1) As DataRow
        Dim tempFetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataSet
        Dim ds As DataSet = CType(GetListGroup(intCode), DataSet)
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

    Public Function GetCode(ByVal strName As String, ByVal intCodeSite As Integer, Optional ByVal blnCommitToDbase As Boolean = False) As Integer
        If Trim(strName) = "" Then strName = "No Supplier"
        Dim tempFetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataTable
        Dim dt As DataTable = CType(GetList(strName, intCodeSite, 255), DataTable)
        L_bytFetchType = tempFetchType

        Dim intCode As Integer = -1

        If dt.DefaultView.Count > 0 Then
            Dim rw As DataRow = dt.Rows(0)
            If Not IsDBNull(rw("code")) Then intCode = CInt(dt.Rows(0)("Code"))
        End If

        If Not blnCommitToDbase Then GoTo Done

        If intCode > -1 Then
            If IsDBNull(dt.Rows(0)("Status")) OrElse CInt(dt.Rows(0)("Status")) <> 1 Then 'inactive
                Dim cItem As clsItem = New clsItem(L_udtUser, enumAppType.WebApp, L_strCnn)
                cItem.UpdateStatus(intCode, intCodeSite, enumDbaseTables.EgswSupplier, 1)
            End If
        Else
            Dim supplier As New structSupplier
            supplier.Code = intCode
            supplier.Address1 = ""
            supplier.Address2 = ""
            supplier.City_1 = ""
            supplier.Number = ""
            supplier.Company = ""
            supplier.Country_1 = ""
            supplier.Email = ""
            supplier.Fax = ""
            supplier.Note = ""
            supplier.Tel = ""
            supplier.NameRef = strName
            supplier.Remark = ""
            supplier.State_1 = ""
            supplier.URL = ""
            supplier.Zip_1 = ""
            supplier.IsGlobal = False

            Update(L_udtUser.Code, intCodeSite, intCode, supplier)
        End If
Done:
        Return intCode
    End Function

    ' RDC 03.14.2013 - CWM-3300 Supplier Standardization
    Public Function StandardizeSupplier(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal eListeType As enumDataListType, _
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

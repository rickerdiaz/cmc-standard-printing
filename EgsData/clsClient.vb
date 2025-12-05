Imports System.Data.SqlClient
Imports System.Data

Namespace FBControl
    ''' <summary>
    ''' Manages Client Table
    ''' </summary>
    ''' <remarks></remarks>

    Public Class clsClient
#Region "Class Header"
        'Name               : clsClient
        'Decription         : Manages Client Table
        'Date Created       : 29.09.06
        'Author             : JHL (based from other existing classes)
        'Revision History   : 
#End Region

#Region "Variable Declarations / Dependencies"
        Inherits clsDBRoutine

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


            If L_udtUser.RoleLevelHighest = 0 Then 'Get ALL items
            ElseIf L_udtUser.RoleLevelHighest = 1 Then 'Get ALL items for a site
                intCodeSite = L_udtUser.Site.Code
            ElseIf L_udtUser.RoleLevelHighest = 2 Then 'Get ALL items for a property
            End If

            Try
                With cmd
                    .Connection = New SqlConnection(L_strCnn)
                    .CommandText = "CLI_GetListMark"
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
            ByVal bytStatus As Integer, Optional ByVal strName As String = "", Optional ByVal lngCodeTrans As Int32 = -1, Optional ByVal flagIncludeAll As Boolean = False) As Object  'DataTable

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
                    .CommandText = "CLI_GetList"
                    .CommandType = CommandType.StoredProcedure
                    .CommandTimeout = 600
                    .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                    .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                    .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = lngCodeTrans
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
                            rX.Item("NameRef") = "*All Client*"
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
                            rX.Item("NameRef") = "*All Client*"
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
Optional ByVal lngCodeClient As Int32 = -1, Optional ByVal strName As String = "") As Object  'DataTable

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
                    .CommandText = "CLI_ContactGetList"
                    .CommandType = CommandType.StoredProcedure
                    .CommandTimeout = 600
                    .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                    .Parameters.Add("@intCodeClient", SqlDbType.Int).Value = lngCodeClient
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

        Private Function SaveIntoList(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
            ByRef lngCode As Int32, ByVal udtClient As structClient, ByVal strCodeSiteList As String, _
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
                    .CommandText = "CLI_Update"
                    .CommandType = CommandType.StoredProcedure
                    .Parameters.Add("@retval", SqlDbType.Int)
                    .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                    .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                    .Parameters.Add("@intCode", SqlDbType.Int).Value = udtClient.Code
                    .Parameters.Add("@tintStatus", SqlDbType.TinyInt).Value = udtClient.Status
                    .Parameters.Add("@nvcNameRef", SqlDbType.NVarChar, 15).Value = udtClient.NameRef
                    .Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 15).Value = IIf(udtClient.Number Is Nothing, "", udtClient.Number)
                    .Parameters.Add("@nvcCompany", SqlDbType.NVarChar, 50).Value = udtClient.Company
                    .Parameters.Add("@nvcBillingAddress", SqlDbType.NVarChar, 200).Value = udtClient.BillingAddress
                    .Parameters.Add("@nvcBillingCity", SqlDbType.NVarChar, 30).Value = udtClient.BillingCity
                    .Parameters.Add("@nvcBillingZip", SqlDbType.NVarChar, 15).Value = udtClient.BillingZip
                    .Parameters.Add("@nvcBillingCountry", SqlDbType.NVarChar, 30).Value = udtClient.BillingCountry
                    .Parameters.Add("@nvcBillingState", SqlDbType.NVarChar, 30).Value = udtClient.BillingState
                    .Parameters.Add("@nvcBillingTel", SqlDbType.NVarChar, 15).Value = udtClient.BillingTel
                    .Parameters.Add("@nvcBillingFax", SqlDbType.NVarChar, 15).Value = udtClient.BillingFax
                    .Parameters.Add("@nvcShippingAddress", SqlDbType.NVarChar, 200).Value = udtClient.ShippingAddress
                    .Parameters.Add("@nvcShippingCity", SqlDbType.NVarChar, 30).Value = udtClient.ShippingCity
                    .Parameters.Add("@nvcShippingZip", SqlDbType.NVarChar, 15).Value = udtClient.ShippingZip
                    .Parameters.Add("@nvcShippingCountry", SqlDbType.NVarChar, 30).Value = udtClient.ShippingCountry
                    .Parameters.Add("@nvcShippingState", SqlDbType.NVarChar, 30).Value = udtClient.ShippingState
                    .Parameters.Add("@nvcShippingTel", SqlDbType.NVarChar, 15).Value = udtClient.ShippingTel
                    .Parameters.Add("@nvcShippingFax", SqlDbType.NVarChar, 15).Value = udtClient.ShippingFax
                    .Parameters.Add("@nvcEmail", SqlDbType.NVarChar, 50).Value = udtClient.Email
                    .Parameters.Add("@nvcCompanyURL", SqlDbType.NVarChar, 50).Value = udtClient.CompanyURL
                    .Parameters.Add("@nvcBusinessURL", SqlDbType.NVarChar, 50).Value = udtClient.BusinessURL
                    .Parameters.Add("@nvcNote", SqlDbType.NVarChar, 2000).Value = udtClient.Note
                    .Parameters.Add("@nvcAccountingID", SqlDbType.NVarChar, 20).Value = IIf(udtClient.AccountingID Is Nothing, "", udtClient.AccountingID)
                    .Parameters.Add("@nvcGLAccount", SqlDbType.NVarChar, 20).Value = IIf(udtClient.GLAccount Is Nothing, "", udtClient.GLAccount)
                    .Parameters.Add("@nvcAccountingRef", SqlDbType.NVarChar, 20).Value = IIf(udtClient.AccountingRef Is Nothing, "", udtClient.AccountingRef)
                    .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = udtClient.IsGlobal
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

        Private Function SaveIntoListContact(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
    ByRef lngCode As Int32, ByVal udtClient As structClient, ByVal strCodeSiteList As String, _
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
                    .CommandText = "CLI_ContactUpdate"
                    .CommandType = CommandType.StoredProcedure
                    .Parameters.Add("@retval", SqlDbType.Int)
                    .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                    .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                    .Parameters.Add("@intCodeClient", SqlDbType.Int).Value = udtClient.Code
                    .Parameters.Add("@intCode", SqlDbType.Int).Value = udtClient.CodeClientContact
                    .Parameters.Add("@nvcLName", SqlDbType.NVarChar, 30).Value = udtClient.LName
                    .Parameters.Add("@nvcFName", SqlDbType.NVarChar, 30).Value = udtClient.FName
                    .Parameters.Add("@nvcTitle", SqlDbType.NVarChar, 30).Value = udtClient.Title
                    .Parameters.Add("@nvcJobPosition", SqlDbType.NVarChar, 100).Value = udtClient.JobPosition
                    .Parameters.Add("@nvcTel", SqlDbType.NVarChar, 15).Value = udtClient.ContactTel
                    .Parameters.Add("@nvcFax", SqlDbType.NVarChar, 15).Value = udtClient.ContactFax
                    .Parameters.Add("@nvcMobile", SqlDbType.NVarChar, 15).Value = udtClient.ContactMobile
                    .Parameters.Add("@nvcEmail", SqlDbType.NVarChar, 50).Value = udtClient.ContactEmail
                    .Parameters.Add("@nvcNote", SqlDbType.NVarChar, 2000).Value = udtClient.Note
                    .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = udtClient.IsGlobal
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
                    .Connection = New SqlConnection(L_strCnn)
                    .CommandText = "CLI_Delete"
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
                    cmd.Parameters.Add("@vchTableName", SqlDbType.VarChar, 50).Value = "EgswClient"

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
                    .CommandText = "CLI_ContactDelete"
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
                    cmd.Parameters.Add("@vchTableName", SqlDbType.VarChar, 50).Value = "EgswClientContact"

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
        ''' Get all Clients with the list of Site names to which they are shared to.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetList() As Object  'DataTable

            Return FetchList(-1, -1, 255)

        End Function

        ''' <summary>
        ''' Get a Client by Name.
        ''' </summary>
        ''' <param name="strName">The name of the Client to be fetched.</param>    
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetList(ByVal strName As String) As Object

            Return FetchList(-1, -1, 255, strName)

        End Function

        ''' <summary>
        ''' Get a Client by Code.
        ''' </summary>
        ''' <param name="lngCode">The Code of the Client to be fetched.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetList(ByVal lngCode As Int32) As Object  'DataTable

            Return FetchList(-1, lngCode, 255)

        End Function

        ''' <summary>
        ''' Get all Clients shared to a specific site and filtered by status.
        ''' </summary>
        ''' <param name="bytStatus">The status of the Clients to be fetched.</param>    
        ''' <param name="lngCodeSite">The CodeSite where the Clients is shared to.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetList(ByVal bytStatus As Integer, ByVal lngCodeSite As Int32, Optional ByVal flagIncludeAll As Boolean = False) As Object  'DataTable
            'Get all by Status
            Return FetchList(lngCodeSite, -1, bytStatus, , -1, flagIncludeAll)

        End Function

        ''' <summary>
        ''' Get all Clients to a specific site.
        ''' </summary>
        ''' <param name="lngCodeTrans">The Code of the language translation.</param>
        ''' <param name="bytStatus">The status of the Client to be fetched.</param>
        ''' <param name="lngCodeSite">The site to which the Client is shared.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetList(ByVal lngCodeTrans As Int32, ByVal lngCodeSite As Int32, ByVal bytStatus As Integer, Optional ByVal flagIncludeAll As Boolean = False) As Object

            Return FetchList(lngCodeSite, -1, bytStatus, , lngCodeTrans, flagIncludeAll)

        End Function

        ''' <summary>
        ''' Get a Client by Name w/in the codesite.
        ''' </summary>
        ''' <param name="strName">The name of the Client to be fetched.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetList(ByVal strName As String, ByVal intCodeSite As Integer, ByVal bytStatus As Integer) As Object

            Return FetchList(intCodeSite, -1, bytStatus, strName)

        End Function

        ''' <summary>
        ''' Get clients by site for markings.
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
        ''' Get a ClientContact by CodeSite and CodeClient
        ''' </summary>
        ''' <param name="lngCodeClient">The Code of the Client to be fetched.</param>
        ''' <param name="lngCodeSite">The Code of the Client Site to be fetched.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetListContact(ByVal lngCodeClient As Int32, ByVal lngCodeSite As Int32) As Object  'DataTable

            Return FetchListContact(lngCodeSite, -1, lngCodeClient)

        End Function

        ''' <summary>
        ''' Get a Client Contact by Code.
        ''' </summary>
        ''' <param name="lngCode">The Code of the Client Contact to be fetched.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function GetListContact(ByVal lngCode As Int32) As Object  'DataTable

            Return FetchListContact(-1, lngCode)

        End Function


#End Region

#Region "Update Methods"

        ''' <summary>
        ''' Updates the global status of a Client.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCode">The Code of the Client to be updated.</param>
        ''' <param name="IsGlobal">The global status of the Client to be updated.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateGlobalStatus(ByVal lngCodeUser As Int32, ByVal lngCode As Int32, ByVal IsGlobal As Boolean) As enumEgswErrorCode


            Dim cmd As New SqlCommand
            Try
                With cmd
                    .Connection = New SqlConnection(L_strCnn)
                    .CommandText = "CLI_UpdateGlobal"
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
        ''' Updates Client without sharing it to any sites.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
        ''' <param name="lngCode">The Code of the Client to be updated.</param>
        ''' <param name="udtClient">Client info.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
            ByRef lngCode As Int32, ByVal udtClient As structClient) As enumEgswErrorCode

            Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtClient, "", "", _
                 CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

        End Function

        ''' <summary>
        ''' Updates Client sharing it to specified sites.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
        ''' <param name="lngCode">The Code of the item to be updated.</param>
        ''' <param name="udtClient">Client info.</param>
        ''' <param name="strCodeSiteList">The list of sites where Client will be shared.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
            ByVal udtClient As structClient, ByVal strCodeSiteList As String) As enumEgswErrorCode

            strCodeSiteList.Trim()
            If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) Then
                Return enumEgswErrorCode.InvalidCodeList
            End If

            Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtClient, strCodeSiteList, "", _
                 CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

        End Function

        ''' <summary>
        ''' Merge Clients
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
        ''' <param name="strCodeClientList">The list of Client Codes to be merged.</param>
        ''' <param name="udtClient">Client info.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
            ByVal strCodeClientList As String, ByVal udtClient As structClient) As enumEgswErrorCode

            Return SaveIntoList(lngCodeUser, lngCodeSite, 0, udtClient, "", strCodeClientList, enumEgswTransactionMode.MergeDelete)

        End Function

        ''' <summary>
        ''' Updates Status of the Clients specified in the list (strCodeList).
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
        ''' <param name="strCodeList">The list of Client Codes to be updated.</param>
        ''' <param name="bytStatus">The Status of the Client.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal strCodeList As String, ByVal bytStatus As Integer) As enumEgswErrorCode

            Return RemoveFromList(lngCodeUser, lngCodeSite, -1, False, enumEgswTransactionMode.Deactivate, bytStatus, strCodeList)

        End Function

        ''' <summary>
        ''' Updates Status of a Client.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
        ''' <param name="lngCode">The Code of the Client to be updated.</param>
        ''' <param name="IsGlobal">The Global Status of the Client.</param>
        ''' <param name="bytStatus">The Status of the Client.</param>
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
        ''' <param name="udtClient">Client info.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function UpdateContact(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
            ByRef lngCode As Int32, ByVal udtClient As structClient) As enumEgswErrorCode

            Return SaveIntoListContact(lngCodeUser, lngCodeSite, lngCode, udtClient, "", "", _
                 CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

        End Function

        ''' <summary>
        ''' Updates Client sharing it to specified sites.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>
        ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
        ''' <param name="lngCode">The Code of the item to be updated.</param>
        ''' <param name="udtClient">Client info.</param>
        ''' <param name="strCodeSiteList">The list of sites where Client will be shared.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function UpdateContact(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
            ByVal udtClient As structClient, ByVal strCodeSiteList As String) As enumEgswErrorCode

            strCodeSiteList.Trim()
            If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) Then
                Return enumEgswErrorCode.InvalidCodeList
            End If

            Return SaveIntoListContact(lngCodeUser, lngCodeSite, lngCode, udtClient, strCodeSiteList, "", _
                 CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

        End Function

#End Region

#Region "Remove Methods"
        ''' <summary>
        ''' Purge Client List.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Remove() As enumEgswErrorCode

            Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Purge)

        End Function

        ''' <summary>
        ''' Deletes a Client.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>    
        ''' <param name="lngCode">The Code of the Client to be deleted.</param>
        ''' <param name="IsGlobal">The Global status of the Client to be deleted.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Remove(ByVal lngCodeUser As Int32, _
            ByVal lngCode As Int32, ByVal IsGlobal As Boolean) As enumEgswErrorCode

            Return RemoveFromList(lngCodeUser, -1, lngCode, IsGlobal, enumEgswTransactionMode.Delete)

        End Function

        ''' <summary>
        ''' Deletes Clients specified in the list strCodeList.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>    
        ''' <param name="strCodeList">The list of Client Codes to be deleted.</param>
        ''' <param name="lngCodeSite">The CodeSite from were you are trying to delete, pass -1 if you want to delete using the user's role.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngcodeSite As Int32, ByVal strCodeList As String) As enumEgswErrorCode

            L_udtUser.Code = lngCodeUser
            Return RemoveFromList(lngCodeUser, lngcodeSite, 0, False, enumEgswTransactionMode.MultipleDelete, , strCodeList)

        End Function

        ''' <summary>
        ''' Deletes a Client Contact.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>    
        ''' <param name="lngCode">The Code of the Client to be deleted.</param>
        ''' <param name="IsGlobal">The Global status of the Client to be deleted.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function RemoveContact(ByVal lngCodeUser As Int32, _
            ByVal lngCode As Int32, ByVal IsGlobal As Boolean) As enumEgswErrorCode

            Return RemoveFromListContact(lngCodeUser, -1, lngCode, IsGlobal, enumEgswTransactionMode.Delete)

        End Function

        ''' <summary>
        ''' Deletes Client Contacts specified in the list strCodeList.
        ''' </summary>
        ''' <param name="lngCodeUser">The Code of the current user.</param>    
        ''' <param name="strCodeList">The list of Client Codes to be deleted.</param>
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

        Public Function GetCode(ByVal strName As String, ByVal intCodeSite As Integer, Optional ByVal blnCommitToDbase As Boolean = False) As Integer
            If Trim(strName) = "" Then strName = "No Client"
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
                    cItem.UpdateStatus(intCode, intCodeSite, enumDbaseTables.EgswClient, 1)
                End If
            Else
                Dim Client As New structClient
                Client.Code = intCode
                Client.NameRef = strName
                Client.Number = ""
                Client.Company = ""
                Client.Email = ""
                Client.CompanyURL = ""
                Client.BusinessURL = ""
                Client.BillingAddress = ""
                Client.BillingCity = ""
                Client.BillingZip = ""
                Client.BillingCountry = ""
                Client.BillingState = ""
                Client.BillingTel = ""
                Client.BillingFax = ""
                Client.ShippingAddress = ""
                Client.ShippingCity = ""
                Client.ShippingZip = ""
                Client.ShippingCountry = ""
                Client.ShippingState = ""
                Client.ShippingTel = ""
                Client.ShippingFax = ""
                Client.Note = ""
                Client.AccountingID = ""
                Client.GLAccount = ""
                Client.AccountingRef = ""
                Client.IsGlobal = False

                Update(L_udtUser.Code, intCodeSite, intCode, Client)
            End If
Done:
            Return intCode
        End Function

#End Region
    End Class
End Namespace

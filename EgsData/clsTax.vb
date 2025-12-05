Imports System.Data.SqlClient
Imports System.Data

#Region "Class Header"
'Name               : clsTax
'Decription         : Manages Tax Table
'Date Created       : 07.09.2005
'Author             : VBV
'Revision History   : VBV - 20.09.2005 - Accepts a connection string as opposed to a connection object. Class performs on a disconnected state.
'                     VBV - 23.09.2005 - Assign user upon creation of object, expose CodeUser
'                     VBV - 30.09.2005 - Added overload method GetList(ByVal strName As String, ByVal eType As enumDataListType)
#End Region

''' <summary>
''' Manages Tax Table
''' </summary>
''' <remarks></remarks>

Public Class clsTax

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

    Private Function FetchList(ByVal lngCodeSite As Int32, ByVal lngCode As Int32, _
        ByVal bytStatus As Byte, Optional ByVal strName As String = "", Optional ByVal dblValue As Double = -1) As Object  'DataTable

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
                .CommandText = "sp_EgswTaxGetList"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@Status", SqlDbType.TinyInt).Value = bytStatus

                If dblValue > -1 Then
                    .Parameters.Add("@fltValue", SqlDbType.Float).Value = dblValue
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
        End If

    End Function

    Private Function SaveIntoList(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByRef lngCode As Int32, ByVal udtTax As structTax, ByVal strCodeSiteList As String, _
        ByVal strCodeTaxList As String, ByVal TranMode As enumEgswTransactionMode, _
        Optional ByVal oTransaction As SqlTransaction = Nothing) As enumEgswErrorCode


        Dim cmd As New SqlCommand
        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .Transaction = oTransaction
                .CommandText = "sp_EgswTaxUpdate"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@intCode", SqlDbType.Int).Value = udtTax.Code
				.Parameters.Add("@nvcDesc", SqlDbType.NVarChar, 50).Value = udtTax.Description
                .Parameters.Add("@fltValue", SqlDbType.Float).Value = udtTax.Value
                .Parameters.Add("@nvcNumberRef", SqlDbType.NVarChar, 20).Value = udtTax.NumberRef
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = udtTax.IsGlobal
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                .Parameters.Add("@retval", SqlDbType.Int)

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

                strCodeTaxList.Trim()
                If strCodeTaxList <> "" Then
                    If Not (strCodeTaxList.StartsWith("(") And strCodeTaxList.EndsWith(")")) Then
                        Return enumEgswErrorCode.InvalidCodeList
                    Else
                        .Parameters.Add("@vchCodeMergeList", SqlDbType.VarChar, 8000).Value = strCodeTaxList
                    End If
                End If

                .Parameters("@retval").Direction = ParameterDirection.ReturnValue
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                lngCode = CInt(.Parameters("@intCode").Value)
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

    Private Function RemoveFromList(ByVal lngCodeUser As Int32, ByVal lngCodesite As Int32, _
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
                .CommandText = "sp_EgswTaxDelete"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = IsGlobal
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodesite
                .Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = lngCodeProperty
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                .Parameters.Add("@retval", SqlDbType.Int)

                strCodeList.Trim()
                If strCodeList <> "" Then
                    If Not (strCodeList.StartsWith("(") And strCodeList.EndsWith(")")) Then
                        Return enumEgswErrorCode.InvalidCodeList
                    Else
                        .Parameters.Add("@vchCodeList", SqlDbType.VarChar, 8000).Value = strCodeList
                    End If
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
                cmd.Parameters.Add("@vchTableName", SqlDbType.VarChar, 50).Value = "EgswTax"

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
    ''' Get all Taxes with the list of Site names to which they are shared to.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList() As Object  'DataTable

        Return FetchList(-1, -1, 255)

    End Function

    ''' <summary>
    ''' Get a Tax by description.
    ''' </summary>
    ''' <param name="strName">The name of the Category to be fetched.</param>    
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal strName As String) As Object

        Return FetchList(-1, -1, 255, strName)

    End Function


    ''' <summary>
    ''' Get a Tax by value w/in copdesite.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal lngCodeSite As Integer, ByVal dblValue As Double, ByVal bytStatus As Byte) As Object
        Return FetchList(lngCodeSite, -1, bytStatus, "", dblValue)
    End Function


    ''' <summary>
    ''' Get a Tax by Code.
    ''' </summary>
    ''' <param name="lngCode">The Code of the Tax to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal lngCode As Int32) As Object  'DataTable

        Return FetchList(-1, lngCode, 255)

    End Function

    ''' <summary>
    ''' Get all Taxes by Status.
    ''' </summary>
    ''' <param name="bytStatus">The status of the Taxes to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal bytStatus As Byte) As Object  'DataTable
        'Get all by Status
        Return FetchList(-1, -1, bytStatus)

    End Function

    ''' <summary>
    ''' Get all Taxes shared to a specific site and filtered by status.
    ''' </summary>    
    ''' <param name="bytStatus">The status of the Tax to be fetched.</param>
    ''' <param name="lngCodeSite">The site to which the Tax is shared.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal bytStatus As Byte, ByVal lngCodeSite As Int32) As Object  'DataTable

        Return FetchList(lngCodeSite, -1, bytStatus)

    End Function
    Public Function GetTaxValue(ByVal intCode As Int32) As Double
        Dim rw As DataRow = GetOne(intCode)
        If rw Is Nothing Then
            Return 0
        Else
            Return CDbl(rw("Value"))
        End If
    End Function

    Public Function GetListTaxCodeValueNameReader(ByVal intCodeSite As Integer, Optional ByVal flagActiveOnly As Boolean = True) As Object
        Dim strCommandText As String = "[GET_TAXCODEVALUEDESC]"

        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@ActiveOnly", flagActiveOnly)
        Try
            Return ExecuteFetchType(enumEgswFetchType.DataReader, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function
#End Region

#Region "Update Methods"

    ''' <summary>
    ''' Updates the global status of a Tax.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the Tax to be updated.</param>
    ''' <param name="IsGlobal">The global status of the Tax to be updated.</param>
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
                .CommandText = "sp_EgswTaxUpdateGlobal"
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
    ''' Updates Tax without sharing it to any sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the Tax to be updated.</param>
    ''' <param name="udtTax">One of the structTax values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByRef lngCode As Int32, ByVal udtTax As structTax) As enumEgswErrorCode

        Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtTax, "", "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    ''' <summary>
    ''' Updates Tax sharing it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtTax">One of the structTax values.</param>
    ''' <param name="strCodeSiteList">The list of sites where Tax will be shared.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtTax As structTax, ByVal strCodeSiteList As String) As enumEgswErrorCode

        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtTax, strCodeSiteList, "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    ''' <summary>
    ''' Merge Taxes
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="strCodeTaxList">The list of Tax Codes to be merged.</param>
    ''' <param name="udtTax">Tax info.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByVal strCodeTaxList As String, ByVal udtTax As structTax) As enumEgswErrorCode

        Return SaveIntoList(lngCodeUser, lngCodeSite, 0, udtTax, "", strCodeTaxList, enumEgswTransactionMode.MergeDelete)

    End Function

    ''' <summary>
    ''' Updates Status of the Taxes specified in the list (strCodeList).
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="strCodeList">The list of Tax Codes to be updated.</param>
    ''' <param name="bytStatus">The Status of the Tax.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal strCodeList As String, ByVal bytStatus As Byte) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, -1, -1, False, enumEgswTransactionMode.Deactivate, bytStatus, strCodeList)

    End Function

    ''' <summary>
    ''' Updates Status of a Tax.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the Tax to be updated.</param>
    ''' <param name="IsGlobal">The Global Status of the Tax.</param>
    ''' <param name="bytStatus">The Status of the Tax.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCode As Int32, ByVal IsGlobal As Boolean, _
        ByVal bytStatus As Byte) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, lngCode, -1, IsGlobal, enumEgswTransactionMode.Deactivate, bytStatus)

    End Function

#End Region

#Region "Remove Methods"
    ''' <summary>
    ''' Purge Tax List.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove() As enumEgswErrorCode

        Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Purge)

    End Function

    ''' <summary>
    ''' Deletes a Tax.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the Tax to be deleted.</param>
    ''' <param name="IsGlobal">The Global status of the Tax to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, _
        ByVal lngCode As Int32, ByVal IsGlobal As Boolean) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, -1, lngCode, IsGlobal, enumEgswTransactionMode.Delete)

    End Function

    ''' <summary>
    ''' Deletes Taxes specified in the list strCodeList.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite from were you are trying to delete, pass -1 if you want to delete using the user's role.</param>
    ''' <param name="strCodeList">The list of Tax Codes to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngcodeSite As Int32, ByVal strCodeList As String) As enumEgswErrorCode

        L_udtUser.Code = lngCodeUser
        'L_lngCodeSite = lngCodeSite
        Return RemoveFromList(lngCodeUser, lngcodeSite, 0, False, enumEgswTransactionMode.MultipleDelete, , strCodeList)

    End Function

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

    Public Function GetCode(ByVal dblValue As Double, ByVal intCodeSite As Integer, Optional ByVal blnCommitToDbase As Boolean = False) As Integer
        Dim tempFetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataTable
        Dim dt As DataTable = CType(GetList(intCodeSite, dblValue, 255), DataTable)
        L_bytFetchType = tempFetchType

        Dim intCode As Integer = -1
        Dim rw As DataRow = dt.Rows(0)

        If Not IsDBNull(rw("code")) Then intCode = CInt(dt.Rows(0)("Code"))
        If Not blnCommitToDbase Then GoTo Done

        If intCode > -1 Then
            If CInt(dt.Rows(0)("Status")) <> 1 Then 'inactive
                Dim cItem As clsItem = New clsItem(L_udtUser, enumAppType.WebApp, L_strCnn)
                cItem.UpdateStatus(intCode, intCodeSite, enumDbaseTables.EgswTax, 1)
            End If
        Else
            Dim tax As structTax
            tax.Code = intCode
            tax.Description = dblValue.ToString
            tax.IsGlobal = False
            tax.NumberRef = dblValue.ToString
            tax.Value = dblValue

            Update(L_udtUser.Code, intCodeSite, intCode, tax)
        End If
Done:
        Return intCode
    End Function

    Public Sub PopulateTax(ByVal cbo As Windows.Forms.ComboBox, ByVal intCodeSite As Int32)
        cbo.ResetText()
        cbo.Items.Clear()
        Dim dr As SqlDataReader = CType(GetList(255, intCodeSite), SqlDataReader)
        If dr IsNot Nothing Then
            Do While dr.Read
                '                Debug.Print(dr.GetValue(dr.GetOrdinal("Name")).ToString)
                cbo.Items.Add(dr.GetValue(dr.GetOrdinal("Value")).ToString)
            Loop
        End If
        dr.Close()
        dr = Nothing
    End Sub

#End Region

#End Region

End Class

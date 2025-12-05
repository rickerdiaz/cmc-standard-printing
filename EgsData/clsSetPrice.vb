Imports System.Data.SqlClient
Imports System.Data

#Region "Class Header"
'Name               : clsSetPrice
'Decription         : Manages SetPrice Table
'Date Created       : 07.09.2005
'Author             : VBV
'Revision History   : VBV - 20.09.2005 - Accepts a connection string as opposed to a connection object. Class performs on a disconnected state.
'                     VBV - 23.09.2005 - Assign user upon creation of object, expose CodeUser
#End Region

''' <summary>
''' Manages SetPrice Table
''' </summary>
''' <remarks></remarks>

Public Class clsSetPrice
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
            ' If udtUser.Code <= 0 Then Throw New Exception("Invalid CodeUser.")
            L_udtUser = udtUser
            L_AppType = eAppType
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn
        Catch ex As Exception
            '    Throw New Exception("Error initializing object", ex)
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

#End Region

#Region "Private Methods"

    Private Function FetchList(ByVal lngCodeSite As Int32, ByVal lngCode As Int32, _
        ByVal bytStatus As Byte, ByVal tntType As SetPriceType, _
        Optional ByVal strName As String = "", Optional ByVal blnGlobalOnly As Boolean = False,
        Optional ByVal lngCodeProperty As Integer = -1) As Object

        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader
        Dim cmd As New SqlCommand
        'MKAM 2014.10.24 - comment out
        'Dim lngCodeProperty As Int32 = -1

        'If L_udtUser.RoleLevelHighest = 0 Then 'Get ALL items
        '    lngCodeProperty = -1
        'ElseIf L_udtUser.RoleLevelHighest = 1 Then 'Get ALL items for a site
        '    lngCodeSite = L_udtUser.Site.Code
        'ElseIf L_udtUser.RoleLevelHighest = 2 Then 'Get ALL items for a property
        '    lngCodeProperty = L_udtUser.Site.Group
        'End If

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswSetPriceGetList"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = lngCodeProperty
                .Parameters.Add("@Status", SqlDbType.TinyInt).Value = bytStatus
                If strName <> "" Then
                    .Parameters.Add("@vchName", SqlDbType.NVarChar, 50).Value = strName
                End If
                .Parameters.Add("@tntType", SqlDbType.TinyInt).Value = tntType
                .Parameters.Add("@bitGlobalOnly", SqlDbType.Bit).Value = blnGlobalOnly
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
                cmd.Connection.Close() 'DLS 27.01.2009
                cmd.Connection.Dispose() 'DLS 27.01.2009
            ElseIf L_bytFetchType = enumEgswFetchType.DataSet Then
                With da
                    .SelectCommand = cmd
                    'dt.BeginLoadData()
                    .Fill(ds, "ItemList")
                    'dt.EndLoadData()
                End With
                cmd.Connection.Close() 'DLS 27.01.2009
                cmd.Connection.Dispose() 'DLS 27.01.2009
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
        ByRef lngCode As Int32, ByVal udtSetPrice As structSetPrice, ByVal strCodeSiteList As String, _
        ByVal strCodeSetPriceList As String, ByVal TranMode As enumEgswTransactionMode, _
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
                .CommandText = "sp_EgswSetPriceUpdate"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@intCode", SqlDbType.Int).Value = udtSetPrice.Code
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 50).Value = udtSetPrice.Name
                .Parameters.Add("@tntCurrency", SqlDbType.TinyInt).Value = udtSetPrice.CodeCurrency
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = udtSetPrice.IsGlobal
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                .Parameters.Add("@tntType", SqlDbType.TinyInt).Value = udtSetPrice.Type
                .Parameters.Add("@intCodePurchasing", SqlDbType.Int).Value = udtSetPrice.CodePurchasing
                .Parameters.Add("@fltSPFactor", SqlDbType.Float).Value = udtSetPrice.SPFactor
                .Parameters.Add("@fltFactorToMain", SqlDbType.Float).Value = udtSetPrice.FactorToMain
                .Parameters.Add("@retval", SqlDbType.Int)

                .Parameters("@intCode").Direction = ParameterDirection.InputOutput
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                strCodeSiteList.Trim()
                If strCodeSiteList <> "" Then
                    If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) Then
                        Return enumEgswErrorCode.InvalidCodeList
                    Else
                        .Parameters.Add("@vchCodeSiteList", SqlDbType.Text).Value = strCodeSiteList
                        .Parameters.Add("@vchCodeSiteList2", SqlDbType.Text).Value = strCodeSiteList.Replace("(", "").Replace(")", "") 'JTOC 10.06.2013
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
                .CommandText = "sp_EgswSetPriceDelete"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = IsGlobal
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodesite
                .Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = lngCodeProperty
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                .Parameters.Add("@retVal", SqlDbType.TinyInt)

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
                cmd.Parameters.Add("@vchTableName", SqlDbType.VarChar, 50).Value = "EgswSetPrice"

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
        If L_udtUser.Code <> -1 And L_lngCodeSite <> -1 Then
            Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Deactivate)
        End If
    End Function

#End Region

#Region "Get Methods"
    ''' <summary>
    ''' Get all SetPrices with the list of Site names to which they are shared to.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal tntType As SetPriceType) As Object  'DataTable

        Return FetchList(-1, -1, 255, tntType)

    End Function

    ''' <summary>
    ''' Get all SetPrices shared to a specific site and filtered by status.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal bytStatus As Byte, ByVal lngCodeSite As Int32, ByVal tntType As SetPriceType, _
        Optional ByVal blnGlobalOnly As Boolean = False,
        Optional ByVal intCodeProperty As Integer = -1) As Object  'DataTable
        Return FetchList(lngCodeSite, -1, bytStatus, tntType, "", blnGlobalOnly, intCodeProperty)
    End Function

    ''' <summary>
    ''' Get a SetPrice by Code.
    ''' </summary>
    ''' <param name="lngCode">The Code of the SetPrice to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal lngCode As Int32) As Object  'DataTable

        Return FetchList(-1, lngCode, 255, SetPriceType.NoType)

    End Function

    ''' <summary>
    ''' Get all SetPrices by Status.
    ''' </summary>
    ''' <param name="bytStatus">The status of the SetPrices to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Overloads Function GetList(ByVal bytStatus As Byte) As Object  'DataTable
    '    'Get all by Status
    '    Return FetchList(-1, -1, bytStatus)

    'End Function
    ''' <summary>
    ''' Get setprice by name
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal lngCodeSite As Int32, ByVal strName As String, ByVal tntType As SetPriceType) As Object  'DataTable
        Return FetchList(lngCodeSite, -1, 255, tntType, strName)
    End Function

    Public Sub PopulateSetPriceList(ByVal cbo As Windows.Forms.ComboBox, ByVal intCodeSite As Int32, ByVal tntType As SetPriceType)
        cbo.ResetText()
        cbo.Items.Clear()
        Dim dr As SqlDataReader = CType(GetList(255, intCodeSite, tntType), SqlDataReader)
        If dr IsNot Nothing Then
            Do While dr.Read
                Debug.Print(dr.GetValue(dr.GetOrdinal("Name")).ToString)
                cbo.Items.Add(dr.GetValue(dr.GetOrdinal("Name")).ToString)
            Loop
        End If
        dr.Close()
        dr = Nothing
    End Sub

    Public Function GetCodeSetPrice(ByVal strSetPriceName As String, ByVal intCodeSite As Int32, ByVal tntType As SetPriceType) As Int32
        Dim dr As SqlDataReader = CType(GetList(intCodeSite, strSetPriceName, tntType), SqlDataReader)
        If dr IsNot Nothing Then
            Do While dr.Read
                Return CInt((dr.GetValue(dr.GetOrdinal("Code")).ToString))
            Loop
        Else
            Return 0
        End If
        dr.Close()
    End Function
    Public Function GetSetPriceName(ByVal intCode As Int32) As String
        Dim dr As SqlDataReader = CType(GetList(intCode), SqlDataReader)
        If dr IsNot Nothing Then
            dr.NextResult()
            Do While dr.Read
                Return (dr.GetValue(dr.GetOrdinal("Name")).ToString)
            Loop
            dr.Close()
        Else
            Return ""
        End If
    End Function

    Public Function GetSetofPriceCodeName(ByVal intCodeSite As Integer, ByVal intType As Integer, ByVal bActiveOnly As Boolean) As Object
        Dim strCommandText As String = "GET_SETOFPRICECODENAME"

        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@Type", intType)
        arrParam(2) = New SqlParameter("@ActiveOnly", bActiveOnly)

        Try
            Select Case L_bytFetchType
                Case enumEgswFetchType.DataReader
                    Return ExecuteReader(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
                Case enumEgswFetchType.DataSet
                    Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
                Case enumEgswFetchType.DataTable
                    Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam).Tables(0)
            End Select

            Return Nothing
        Catch ex As Exception
            Throw ex
        End Try
    End Function
#End Region

#Region "Update Methods"

    ''' <summary>
    ''' Updates the global status of a SetPrice.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the SetPrice to be updated.</param>
    ''' <param name="IsGlobal">The global status of the SetPrice to be updated.</param>
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
                .CommandText = "sp_EgswSetPriceUpdateGlobal"
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
    ''' Updates SetPrice without sharing it to any sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the SetPrice to be updated.</param>
    ''' <param name="udtSetPrice">One of the structSetPrice values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByRef lngCode As Int32, ByVal udtSetPrice As structSetPrice) As enumEgswErrorCode

        Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtSetPrice, "", "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    ''' <summary>
    ''' Updates SetPrice sharing it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtSetPrice">One of the structSetPrice values.</param>
    ''' <param name="strCodeSiteList">The list of sites where SetPrice will be shared.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtSetPrice As structSetPrice, ByVal strCodeSiteList As String) As enumEgswErrorCode

        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtSetPrice, strCodeSiteList, "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    ''' <summary>
    ''' Updates Status of the SetPrices specified in the list (strCodeList).
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="strCodeList">The list of SetPrice Codes to be updated.</param>
    ''' <param name="bytStatus">The Status of the SetPrice.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal strCodeList As String, ByVal bytStatus As Byte) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, lngCodeSite, -1, False, enumEgswTransactionMode.Deactivate, bytStatus, strCodeList)

    End Function

    ''' <summary>
    ''' Updates Status of a SetPrice.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the SetPrice to be updated.</param>
    ''' <param name="IsGlobal">The Global Status of the SetPrice.</param>
    ''' <param name="bytStatus">The Status of the SetPrice.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal lngCode As Int32, ByVal IsGlobal As Boolean, _
        ByVal bytStatus As Byte) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, lngCodeSite, lngCode, IsGlobal, enumEgswTransactionMode.Deactivate, bytStatus)

    End Function

#End Region

#Region "Remove Methods"
    ''' <summary>
    ''' Purge SetPrice List.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove() As enumEgswErrorCode

        Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Purge)

    End Function

    ''' <summary>
    ''' Deletes a SetPrice.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>    
    ''' <param name="lngCode">The Code of the SetPrice to be deleted.</param>
    ''' <param name="IsGlobal">The Global status of the SetPrice to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, _
        ByVal lngCode As Int32, ByVal IsGlobal As Boolean) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, -1, lngCode, IsGlobal, enumEgswTransactionMode.Delete)

    End Function

    ''' <summary>
    ''' Deletes SetPrices specified in the list strCodeList.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>    
    ''' <param name="strCodeList">The list of SetPrice Codes to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngCodesite As Int32, ByVal strCodeList As String) As enumEgswErrorCode

        L_udtUser.Code = lngCodeUser
        'L_lngCodeSite = lngCodeSite
        Return RemoveFromList(lngCodeUser, lngCodesite, 0, False, enumEgswTransactionMode.MultipleDelete, , strCodeList)

    End Function

#End Region

#Region " Other Functions "

    Public Function GetConversionRate(ByVal intCodeSetPrice1 As Integer, ByVal intCodeSetPrice2 As Integer, Optional ByRef sNewSymbole As String = "") As Double
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeSetPrice1", intCodeSetPrice1)
        arrParam(1) = New SqlParameter("@intCodeSetPrice2", intCodeSetPrice2)

        Dim dr As SqlDataReader = ExecuteReader(L_strCnn, CommandType.StoredProcedure, "sp_egswGetSetPriceConversionRate", arrParam)
        Dim nCurRate As Double
        While dr.Read
            nCurRate = CDbl(dr.Item("currate"))
            sNewSymbole = dr.Item("symboleNew").ToString
        End While
        dr.Close()
        Return nCurRate
    End Function

    Public Function GetOne(ByVal intCode As Integer) As DataRow
        Dim tempFetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataSet
        Dim ds As DataSet = CType(GetList(intCode), DataSet)
        L_bytFetchType = tempFetchType

        Dim dt As DataTable = ds.Tables(1)
        If dt.DefaultView.Count = 0 Then Return Nothing

        Dim rw As DataRow = dt.Rows(0)
        Return rw
    End Function

    Public Function GetOneWithSite(intCodeSite As Integer) As DataRow
        Dim tempFetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataSet
        Dim ds As DataSet = CType(GetList(intCodeSite, "", SetPriceType.Purchase), DataSet)
        L_bytFetchType = tempFetchType

        Dim dt As DataTable = ds.Tables(0) 'AGL 2014.09.04 - changed to 1
        If dt.DefaultView.Count = 0 Then Return Nothing

        Dim rw As DataRow = dt.Rows(0)
        Return rw
    End Function

    'Public Function GetSetPriceFormat(ByVal intCode As Integer, Optional ByRef strSymbole As String = "") As String
    '    Dim fetchType As enumEgswFetchType = L_bytFetchType
    '    L_bytFetchType = enumEgswFetchType.DataSet
    '    Dim ds As DataSet = CType(GetList(intCode), DataSet)
    '    L_bytFetchType = fetchType

    '    Dim dt As DataTable = ds.Tables(1)
    '    If dt.Rows.Count <> 0 Then
    '        strSymbole = CStr(dt.Rows(0).Item("symbole"))
    '        Return CStr(dt.Rows(0).Item("format"))
    '    Else
    '        Return "#,##0.00"
    '    End If
    'End Function

#End Region

End Class

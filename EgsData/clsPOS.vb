Imports System.Data.SqlClient
Imports System.Data

''' <summary>
''' Manages POS Configuration
''' </summary>
''' <remarks></remarks>

Public Class clsPOS
#Region "Class Header"
    'Name               : clsPOS
    'Decription         : Manages POS Configurations
    'Date Created       : 02.01.06
    'Author             : JHL
    'Revision History   : 
    '
#End Region
#Region "Variable Declarations / Dependencies"
    Inherits clsDBRoutine

    Private L_ErrCode As enumEgswErrorCode
    Private L_lngCodeSite As Int32 = -1
    Private L_RoleLevelHighest As Int16 = -1

    'Properties
    Private L_udtUser As structUser
    Private L_dtList As DataTable
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType
    Private L_lngCode As Int32
#End Region

#Region "Class Functions and Properties"
    Public Sub New(ByVal strCnn As String, _
        Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)
        'ByVal udtUser As structUser, 
        Try
            'If udtUser.Code <= 0 Then Throw New Exception("Invalid CodeUser.")
            'L_udtUser = udtUser
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn
        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try

    End Sub
#End Region

#Region "Private Methods"
    Private Function FetchList(ByVal lngCode As Int32, _
    Optional ByVal strName As String = "") As Object

        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader
        Dim cmd As New SqlCommand
        'Dim lngCodeProperty As Int32 = -1

        'If L_udtUser.RoleLevelHighest = 0 Then 'Get ALL items
        '    lngCodeProperty = -1
        'ElseIf L_udtUser.RoleLevelHighest = 1 Then 'Get ALL items for a site
        '    lngCodeSite = L_udtUser.Site.Code
        'ElseIf L_udtUser.RoleLevelHighest = 2 Then 'Get ALL items for a property
        '    lngCodeProperty = L_udtUser.Site.Group
        'End If

        dr = Nothing
        FetchList = Nothing
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswPOSInfoGetList"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                '.Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                '.Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = lngCodeProperty
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

    Private Function FetchConfigList(ByVal intID As Int32, ByVal intCodeSite As Int32) As Object

        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader
        Dim cmd As New SqlCommand

        dr = Nothing
        FetchConfigList = Nothing
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswPOSAutoConfigGetList"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intID", SqlDbType.Int).Value = intID
                .Parameters.Add("@intCodesite", SqlDbType.Int).Value = intCodeSite
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

    Private Function FetchPOSDataList(ByVal intID As Int32, ByVal intCodeSite As Int32) As Object

        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader
        Dim cmd As New SqlCommand

        dr = Nothing
        FetchPOSDataList = Nothing
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswTempPOSSalesDataGetList"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intID", SqlDbType.Int).Value = intID
                .Parameters.Add("@intCodesite", SqlDbType.Int).Value = intCodeSite
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

    Private Function SaveIntoList(ByVal lngCodeUser As Int32, _
    ByRef lngID As Int32, ByVal udtPOSAutoConfig As structPOSAutoConfig, ByVal strCodeSiteList As String, _
    ByVal strCodePOSAutoConfigList As String, ByVal TranMode As enumEgswTransactionMode, _
    Optional ByVal oTransaction As SqlTransaction = Nothing, Optional ByVal blnOnlyActiveChanged As Boolean = False) As enumEgswErrorCode

        Dim cmd As New SqlCommand

        Try
            With cmd
                If oTransaction Is Nothing Then
                    .Connection = New SqlConnection(L_strCnn)
                Else
                    .Connection = oTransaction.Connection
                    .Transaction = oTransaction
                End If

                .CommandText = "sp_EgswPOSAutoConfigUpdate"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intID", SqlDbType.Int).Value = lngID
                .Parameters.Add("@intCodePOStype", SqlDbType.Int).Value = udtPOSAutoConfig.CodePOStype
                .Parameters.Add("@bitActive", SqlDbType.Bit).Value = udtPOSAutoConfig.Active
                .Parameters.Add("@nvcPath1", SqlDbType.NVarChar, 300).Value = udtPOSAutoConfig.Path1
                .Parameters.Add("@nvcPath2", SqlDbType.NVarChar, 300).Value = udtPOSAutoConfig.Path2
                .Parameters.Add("@nvcPrefix1", SqlDbType.NVarChar, 300).Value = udtPOSAutoConfig.Prefix1
                .Parameters.Add("@nvcPrefix2", SqlDbType.NVarChar, 300).Value = udtPOSAutoConfig.Prefix2
                .Parameters.Add("@nvcArchivePath", SqlDbType.NVarChar, 300).Value = udtPOSAutoConfig.ArchivePath
                .Parameters.Add("@dtOpeningTime", SqlDbType.DateTime).Value = udtPOSAutoConfig.OpeningTime.ToString  'CDate(udtPOSAutoConfig.OpeningTime.ToShortTimeString) 'CDate(udtPOSAutoConfig.OpeningTime.ToShortTimeString)
                .Parameters.Add("@intMainSched", SqlDbType.Int).Value = udtPOSAutoConfig.MainSched
                If CDate(udtPOSAutoConfig.DateSales) <> CDate("1/1/1900") Then .Parameters.Add("@dtStartDate", SqlDbType.DateTime).Value = CDate(udtPOSAutoConfig.StartDate)
                .Parameters.Add("@dtStartTime", SqlDbType.DateTime).Value = CDate(udtPOSAutoConfig.StartTime)
                .Parameters.Add("@intEveryNth", SqlDbType.Int).Value = udtPOSAutoConfig.EveryNth
                .Parameters.Add("@bitMon", SqlDbType.Bit).Value = udtPOSAutoConfig.Mon
                .Parameters.Add("@bitTue", SqlDbType.Bit).Value = udtPOSAutoConfig.Tue
                .Parameters.Add("@bitWed", SqlDbType.Bit).Value = udtPOSAutoConfig.Wed
                .Parameters.Add("@bitThu", SqlDbType.Bit).Value = udtPOSAutoConfig.Thu
                .Parameters.Add("@bitFri", SqlDbType.Bit).Value = udtPOSAutoConfig.Fri
                .Parameters.Add("@bitSat", SqlDbType.Bit).Value = udtPOSAutoConfig.Sat
                .Parameters.Add("@bitSun", SqlDbType.Bit).Value = udtPOSAutoConfig.Sun
                .Parameters.Add("@intTheNth", SqlDbType.Int).Value = udtPOSAutoConfig.TheNth
                .Parameters.Add("@nvcNote", SqlDbType.NVarChar, 4000).Value = udtPOSAutoConfig.Note
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = udtPOSAutoConfig.CodeSite
                .Parameters.Add("@intCodeTerminal", SqlDbType.Int).Value = udtPOSAutoConfig.CodeTerminal                

                'MRC - 10.03.08 - Removed some items because these arent included in the SP
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = udtPOSAutoConfig.CodeSellingSetPrice
                .Parameters.Add("@intCodeSellingSetPrice", SqlDbType.Int).Value = udtPOSAutoConfig.CodeSellingSetPrice
                .Parameters.Add("@intCodePurchaseSetPrice", SqlDbType.Int).Value = udtPOSAutoConfig.CodePurchaseSetPrice
                .Parameters.Add("@fltSPFactor", SqlDbType.Int).Value = udtPOSAutoConfig.SPFactor


                .Parameters.Add("@intCodeTax", SqlDbType.Int).Value = udtPOSAutoConfig.CodeTax
                If CDate(udtPOSAutoConfig.DateSales) > CDate("1/1/1900") Then .Parameters.Add("@dtDateSales", SqlDbType.DateTime).Value = udtPOSAutoConfig.DateSales
                If CDate(udtPOSAutoConfig.LastImport) > CDate("1/1/1900") Then .Parameters.Add("@dtLastImport", SqlDbType.DateTime).Value = udtPOSAutoConfig.LastImport.ToString
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@bitOnlyActiveChanged", SqlDbType.Bit).Value = blnOnlyActiveChanged
                .Parameters("@intID").Direction = ParameterDirection.InputOutput
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                If oTransaction Is Nothing Then .Connection.Open()
                .ExecuteNonQuery()
                If oTransaction Is Nothing Then .Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                lngID = CInt(.Parameters("@intID").Value)
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

    Private Function RemovePOSConfigFromList(ByVal intID As Int32, ByVal TranMode As enumEgswTransactionMode, _
        Optional ByVal strIdList As String = "") As enumEgswErrorCode

        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswPOSAutoConfigDelete"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intId", SqlDbType.Int).Value = intID
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                strIdList.Trim()
                If strIdList <> "" Then
                    If Not (strIdList.StartsWith("(") And strIdList.EndsWith(")")) Then
                        Return enumEgswErrorCode.InvalidCodeList
                    Else
                        .Parameters.Add("@nvchIDList", SqlDbType.NVarChar, 4000).Value = strIdList
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

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

    End Function

    Private Function RemovePOSTempDataFromList(ByVal intID As Int32, ByVal TranMode As enumEgswTransactionMode, _
       Optional ByVal strIdList As String = "") As enumEgswErrorCode

        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswTempPOSSalesDataDelete"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intId", SqlDbType.Int).Value = intID
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                strIdList.Trim()
                If strIdList <> "" Then
                    If Not (strIdList.StartsWith("(") And strIdList.EndsWith(")")) Then
                        Return enumEgswErrorCode.InvalidCodeList
                    Else
                        .Parameters.Add("@nvchIDList", SqlDbType.NVarChar, 4000).Value = strIdList
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

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

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
                .CommandText = "sp_EgswTerminalMovePos"
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

    Public Function ImportSales(ByVal intID As Int32, ByVal lngCodeUser As Int32, ByVal strFile1 As String, ByVal strFile2 As String, _
    Optional ByVal oTransaction As SqlTransaction = Nothing, Optional ByVal blnOnlyActiveChanged As Boolean = False) As enumEgswErrorCode

        Dim cmd As New SqlCommand
        Try
            With cmd
                If oTransaction Is Nothing Then
                    .Connection = New SqlConnection(L_strCnn)
                Else
                    .Connection = oTransaction.Connection
                    .Transaction = oTransaction
                End If
                If intID <= 0 Then
                    .CommandText = "sp_EgswImportSalesMain"
                    .CommandTimeout = 600
                    .CommandType = CommandType.StoredProcedure
                    .Parameters.Add("@retval", SqlDbType.Int)
                Else
                    .CommandText = "sp_EgswImportSales"
                    .CommandTimeout = 600
                    .CommandType = CommandType.StoredProcedure
                    .Parameters.Add("@retval", SqlDbType.Int)
                    .Parameters.Add("@ID", SqlDbType.Int).Value = intID
                End If

                .Parameters.Add("@Locale_DecimalSep", SqlDbType.Char, 1).Value = G_strDecimalSeparatorLocal
                .Parameters.Add("@Locale_ThousandSep", SqlDbType.Char, 1).Value = G_strThouSeparatorLocal
                .Parameters.Add("@File1", SqlDbType.NVarChar, 4000).Value = strFile1
                .Parameters.Add("@File2", SqlDbType.NVarChar, 4000).Value = strFile2
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                If oTransaction Is Nothing Then .Connection.Open()
                .ExecuteNonQuery()
                If oTransaction Is Nothing Then .Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)

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

    Public Function UpdateTmpPOSData(ByRef lngID As Int32, ByVal udtPOSTempData As structPOSTempData, _
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

                .CommandText = "sp_EgswTempPOSSalesDataUpdate"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intID", SqlDbType.Int).Value = lngID
                .Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 100).Value = udtPOSTempData.POSNumber
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 1000).Value = udtPOSTempData.POSName
                .Parameters.Add("@nvcPrice", SqlDbType.NVarChar, 100).Value = udtPOSTempData.POSPrice
                .Parameters.Add("@nvcQty", SqlDbType.NVarChar, 100).Value = udtPOSTempData.POSQty
                .Parameters.Add("@nvcSalesDate", SqlDbType.NVarChar, 100).Value = udtPOSTempData.POSSalesDate
                .Parameters.Add("@nvcSite", SqlDbType.NVarChar, 100).Value = udtPOSTempData.POSSite
                .Parameters.Add("@nvcTerminal", SqlDbType.NVarChar, 100).Value = udtPOSTempData.POSTerminal
                .Parameters.Add("@nvcRefNo", SqlDbType.NVarChar, 100).Value = udtPOSTempData.POSRefNo
                .Parameters.Add("@nvcTaxRefNo", SqlDbType.NVarChar, 100).Value = udtPOSTempData.POSTaxRefNo
                .Parameters.Add("@nvcTaxValue", SqlDbType.NVarChar, 100).Value = udtPOSTempData.POSTaxValue
                .Parameters.Add("@nvcAmount", SqlDbType.NVarChar, 100).Value = udtPOSTempData.POSAmount
                .Parameters.Add("@nvcIssuanceType", SqlDbType.NVarChar, 100).Value = udtPOSTempData.POSIssuanceType
                .Parameters.Add("@nvcTime", SqlDbType.NVarChar, 100).Value = udtPOSTempData.POSTime
                .Parameters.Add("@nvcCurrency", SqlDbType.NVarChar, 100).Value = udtPOSTempData.POSCurrency
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                If oTransaction Is Nothing Then .Connection.Open()
                .ExecuteNonQuery()
                If oTransaction Is Nothing Then .Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
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

#End Region

#Region "Get Methods"
    ''' <summary>
    ''' Get all POS Types.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>    
    Public Overloads Function GetList() As Object
        Return FetchList(-1)
    End Function

    ''' <summary>
    ''' Get a POSType by Name.
    ''' </summary>
    ''' <param name="strName">The name of the Terminal to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal strName As String) As Object
        Return FetchList(-1, strName)
    End Function

    ''' <summary>
    ''' Get a Terminal by Code or by Site
    ''' </summary>
    ''' <param name="lngCode">The Code of the Terminal to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal lngCode As Int32) As Object 'DataTable
        Return FetchList(lngCode)
    End Function

    ''' <summary>
    ''' Get POS Config List
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetPOSConfigList() As Object 'DataTable
        Return FetchConfigList(-1, -1)
    End Function


    ''' <summary>
    ''' Get POS Config List
    ''' </summary>
    ''' <param name="intID">The Code of the POS to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetPOSConfigList(ByVal intID As Int32) As Object 'DataTable
        Return FetchConfigList(intID, -1)
    End Function

    Public Overloads Function GetPOSConfigList(ByVal intID As Int32, ByVal intCodesite As Integer) As Object 'DataTable
        Return FetchConfigList(intID, intCodesite)
    End Function

    Public Function GetPOSTypeName(ByVal lngCode As Int32) As String
        GetPOSTypeName = ""
        Dim dr As SqlDataReader = CType(GetList(lngCode), SqlDataReader)
        If dr IsNot Nothing Then
            Do While dr.Read
                Return (dr.GetValue(dr.GetOrdinal("Name")).ToString)
            Loop
            dr.Close()
        Else
            Return ""
        End If
    End Function
    Public Function GetCodePOS(ByVal strPOSTypeName As String) As Int32
        Dim dr As SqlDataReader = CType(GetList(strPOSTypeName), SqlDataReader)
        If dr IsNot Nothing Then
            Do While dr.Read
                Return CInt((dr.GetValue(dr.GetOrdinal("Code")).ToString))
            Loop
        Else
            Return 0
        End If
        dr.Close()
    End Function

    '''For POS Import
    ''' <summary>
    ''' Get Import POS Record based on ID
    ''' </summary>
    ''' <param name="intID">The ID of the record to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetPOSData(ByVal intID As Int32) As Object 'DataTable
        Return FetchPOSDataList(intID, -1)
    End Function

    ''' <summary>
    ''' Get List of Records Not Imported because of data errors
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetPOSData() As Object 'DataTable
        Return FetchPOSDataList(-1, -1)
    End Function

    ''' <summary>
    ''' Get List of Records not imported by site
    ''' </summary>
    ''' <param name="intID">pass -1</param>
    ''' <param name="intCodesite">pass the codesite wanted</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetPOSData(ByVal intID As Int32, ByVal intCodesite As Int32) As Object 'DataTable
        Return FetchPOSDataList(intID, intCodesite)
    End Function

#End Region

#Region "Update Methods"
    ''' <summary>
    ''' Updates Terminal
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngID">The Code of the Terminal to be updated.</param>
    ''' <param name="udtPOSAutoConfig">One of the structTerminal values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    Public Overloads Function Update(ByVal lngCodeUser As Int32, _
        ByRef lngID As Int32, ByVal udtPOSAutoConfig As structPOSAutoConfig) As enumEgswErrorCode

        Return SaveIntoList(lngCodeUser, lngID, udtPOSAutoConfig, "", "", _
             CType(IIf(lngID < 1, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    '''' <summary>
    '''' Merge Terminals
    '''' </summary>
    '''' <param name="lngCodeUser">The Code of the current user.</param>
    '''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    '''' <param name="strCodeTerminalList">The list of Terminal Codes to be merged.</param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal strCodeTerminalList As String, ByVal udtTerminal As structTerminal) As enumEgswErrorCode
    '    Return SaveIntoList(lngCodeUser, lngCodeSite, 0, udtTerminal, "", strCodeTerminalList, enumEgswTransactionMode.MergeDelete)
    'End Function

#End Region

#Region "Remove Methods"
    ''' <summary>
    ''' Deletes a POS auto config
    ''' </summary>
    ''' <param name="intId">The Code of the POS auto config to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal intID As Int32) As enumEgswErrorCode
        Return RemovePOSConfigFromList(intID, enumEgswTransactionMode.Delete)
    End Function

    ''' <summary>
    ''' Deletes POS autoconfig specified in the list strCodeList.
    ''' </summary>
    ''' <param name="strIDList">The List of Id to be deleted.</param>    
    ''' <returns></returns>
    ''' <remarks></remarks>    
    Public Overloads Function Remove(ByVal strIDList As String) As enumEgswErrorCode
        Return RemovePOSConfigFromList(-1, enumEgswTransactionMode.MultipleDelete, strIDList)
    End Function

    ''' <summary>
    ''' Deletes a POS Tempdata
    ''' </summary>
    ''' <param name="intId">The id of the POS temp data to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function RemoveTempData(ByVal intID As Int32) As enumEgswErrorCode
        Return RemovePOSTempDataFromList(intID, enumEgswTransactionMode.Delete)
    End Function

    ''' <summary>
    ''' Deletes POS Tempdata  specified in the list strIDList.
    ''' </summary>
    ''' <param name="strIDList">The List of Id to be deleted.</param>    
    ''' <returns></returns>
    ''' <remarks></remarks>    
    Public Overloads Function RemoveTempData(ByVal strIDList As String) As enumEgswErrorCode
        Return RemovePOSTempDataFromList(-1, enumEgswTransactionMode.MultipleDelete, strIDList)
    End Function




#End Region

#Region "Other Public Functions"
    Public Sub PopulatePOSTypeList(ByVal cbo As Windows.Forms.ComboBox)
        Dim dr As SqlDataReader = CType(GetList(), SqlDataReader)
        If dr IsNot Nothing Then
            Do While dr.Read
                Debug.Print(dr.GetValue(dr.GetOrdinal("Name")).ToString)
                cbo.Items.Add(dr.GetValue(dr.GetOrdinal("Name")).ToString)
            Loop
        End If

        dr.Close()
        dr = Nothing
    End Sub


    Public Sub AssignGlobalPOSConfig(ByVal strCnn As String, ByVal intID As Integer)
        Dim clsPOS As New clsPOS(strCnn)
        Dim dr As SqlDataReader = CType(clsPOS.GetPOSConfigList(intID), SqlDataReader)
        If dr IsNot Nothing Then
            dr.Read()
            With G_ImportConfig
                .ID = intID
                .Active = CBoolDB(dr.GetValue(dr.GetOrdinal("Active")).ToString)
                '.CodeSite = CIntDB(dr.GetValue(dr.GetOrdinal("CodeSite")).ToString)
                .Path1 = CStrDB(dr.GetValue(dr.GetOrdinal("Path1")).ToString)
                .Path2 = CStrDB(dr.GetValue(dr.GetOrdinal("Path2")).ToString)
                .Prefix1 = CStrDB(dr.GetValue(dr.GetOrdinal("Prefix1")).ToString)
                .Prefix2 = CStrDB(dr.GetValue(dr.GetOrdinal("Prefix2")).ToString)
                .CodeTerminal = CIntDB(dr.GetValue(dr.GetOrdinal("CodeTerminal")).ToString)
                .CodePOStype = CIntDB(dr.GetValue(dr.GetOrdinal("CodePOSType")).ToString)
                .CodeTerminal = CIntDB(dr.GetValue(dr.GetOrdinal("CodeTerminal")).ToString)
                .CodeSellingSetPrice = CIntDB(dr.GetValue(dr.GetOrdinal("CodeSellingSetPrice")).ToString)
                .CodePurchaseSetPrice = CIntDB(dr.GetValue(dr.GetOrdinal("CodePurchaseSetPrice")).ToString)
                .SPFactor = CDblDB(dr.GetValue(dr.GetOrdinal("SPFactor")).ToString)
                .ArchivePath = CStrDB(dr.GetValue(dr.GetOrdinal("ArchivePath")).ToString)
                .OpeningTime = CDateDB(dr.GetValue(dr.GetOrdinal("OpeningTime")))
                .DateSales = CDateDB(dr.GetValue(dr.GetOrdinal("DateSales")))
                .CodeTax = CIntDB(dr.GetValue(dr.GetOrdinal("CodeTax")).ToString)
                .Note = CStrDB(dr.GetValue(dr.GetOrdinal("Note")).ToString)
                .MainSched = CIntDB(dr.GetValue(dr.GetOrdinal("MainSched")).ToString)
                .StartDate = CDateDB(dr.GetValue(dr.GetOrdinal("StartDate")))
                .StartTime = CDateDB(dr.GetValue(dr.GetOrdinal("StartTime")))
                .EveryNth = CIntDB(dr.GetValue(dr.GetOrdinal("EveryNth")).ToString)
                .TheNth = CIntDB(dr.GetValue(dr.GetOrdinal("TheNth")).ToString)
                .Mon = CBoolDB(dr.GetValue(dr.GetOrdinal("Mon")).ToString)
                .Tue = CBoolDB(dr.GetValue(dr.GetOrdinal("Tue")).ToString)
                .Wed = CBoolDB(dr.GetValue(dr.GetOrdinal("Wed")).ToString)
                .Thu = CBoolDB(dr.GetValue(dr.GetOrdinal("Thu")).ToString)
                .Fri = CBoolDB(dr.GetValue(dr.GetOrdinal("Fri")).ToString)
                .Sat = CBoolDB(dr.GetValue(dr.GetOrdinal("Sat")).ToString)
                .Sun = CBoolDB(dr.GetValue(dr.GetOrdinal("Sun")).ToString)
                .LastImport = CDateDB(dr.GetValue(dr.GetOrdinal("LastImport")))
            End With
            dr.Close()
            dr = Nothing
        End If
    End Sub

    Public Sub GetPOSInfo(ByVal intCodePOS As Integer, ByVal strCnn As String)
        Dim clsPOS As New clsPOS(strCnn, enumEgswFetchType.DataReader)
        Dim dr As SqlDataReader = CType(clsPOS.GetList(intCodePOS), SqlDataReader)
        If dr IsNot Nothing Then
            Do While dr.Read
                With G_POSInfo
                    .Name = CStrDB(dr.GetValue(dr.GetOrdinal("Name")).ToString)
                    .File1 = CStrDB(dr.GetValue(dr.GetOrdinal("File1")).ToString)
                    .File2 = CStrDB(dr.GetValue(dr.GetOrdinal("File2")).ToString)
                    .LastID = CIntDB(dr.GetValue(dr.GetOrdinal("LastID")).ToString)
                    .Active = CBoolDB(dr.GetValue(dr.GetOrdinal("Active")).ToString)
                    .FileCount = CIntDB(dr.GetValue(dr.GetOrdinal("FileCount")).ToString)
                    .FileExtension = CStrDB(dr.GetValue(dr.GetOrdinal("FileExtension")).ToString)
                    .DeleteAfterImport = CBoolDB(dr.GetValue(dr.GetOrdinal("DeleteAfterImport")).ToString)
                End With
                Debug.Print(dr.GetValue(dr.GetOrdinal("Name")).ToString)
            Loop
        End If
        dr.Close()
        dr = Nothing
    End Sub
#End Region


End Class


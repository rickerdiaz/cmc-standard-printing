Imports System.Data.SqlClient
Imports System.Data
Public Class clsSalesItem

#Region "Variable Declarations / Dependencies"
    Inherits clsDBRoutine

    Private L_ErrCode As enumEgswErrorCode
    Private L_lngCodeSite As Int32 = -1
    Private L_RoleLevelHighest As Int16 = -1

    'Properties
    Private L_User As structUser
    Private L_dtList As DataTable
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType
    Private L_lngCode As Int32

    Private L_udtFilter As structSearchSalesFilter
#End Region

#Region "Class Functions and Properties"
    Public Sub New(ByVal udtUser As structUser, ByVal strCnn As String, _
        Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)

        Try
            If udtUser.Code <= 0 Then Throw New Exception("Invalid CodeUser.")
            L_User = udtUser
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn

        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try

    End Sub
#End Region

#Region "Private Methods"

    Private Function FetchListPriceHistory(ByVal intCodeSalesItem As Integer, ByVal intCodeSetPrice As Integer) As Object
        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader
        Dim cmd As New SqlCommand

        dr = Nothing
        FetchListPriceHistory = Nothing
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswSalesItemPriceHistoryGetList"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intCodeSalesItem", SqlDbType.Int).Value = intCodeSalesItem
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice
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

    Private Function FetchListPrice(ByVal intCodeSalesItem As Integer, ByVal intCodeSetPrice As Integer) As Object
        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader
        Dim cmd As New SqlCommand

        dr = Nothing
        FetchListPrice = Nothing
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswSalesItemPriceGetList"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intCodeSalesItem", SqlDbType.Int).Value = intCodeSalesItem
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice
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

    Private Function FetchList(ByVal udt As structSearchSalesFilter, Optional ByRef intRowCount As Integer = 0) As Object
        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader
        Dim cmd As New SqlCommand

        dr = Nothing
        FetchList = Nothing
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswSalesItemGetList"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intCode", SqlDbType.Int).Value = udt.Code
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = udt.CodeSite
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = udt.SetPriceSale
                If udt.Name.Trim <> "" Then .Parameters.Add("@nvcName", SqlDbType.NVarChar, 200).Value = udt.Name
                .Parameters.Add("@bitExactMatch", SqlDbType.Bit).Value = udt.IsExactMatch
                .Parameters.Add("@numNumber", SqlDbType.Int).Value = udt.Number

                If udt.PriceFrom > -1 Then .Parameters.Add("@fltFromPriceRange", SqlDbType.Float).Value = udt.PriceFrom
                If udt.PriceTo > -1 Then .Parameters.Add("@fltToPriceRange", SqlDbType.Float).Value = udt.PriceTo

                .Parameters.Add("@intCodeTax", SqlDbType.Int).Value = udt.Tax
                .Parameters.Add("@bitProduct", SqlDbType.Bit).Value = udt.IncludeProduct
                .Parameters.Add("@bitRecipe", SqlDbType.Bit).Value = udt.IncludeRecipe
                .Parameters.Add("@bitMenu", SqlDbType.Bit).Value = udt.IncludeMenu
                .Parameters.Add("@bitNoType", SqlDbType.Bit).Value = udt.IncludeNoTypes
                .Parameters.Add("@tntNotLinked", SqlDbType.TinyInt).Value = Math.Abs(udt.Linked)
                .Parameters.Add("@intPageSize", SqlDbType.Int).Value = udt.intPageSize
                .Parameters.Add("@intPageIndex", SqlDbType.Int).Value = udt.intPageIndex
                .Parameters.Add("@vchCodeList", SqlDbType.VarChar, 4000).Value = udt.strCodeList
                .Parameters.Add("@intRowCount", SqlDbType.Int).Value = intRowCount
                .Parameters("@intRowCount").Direction = ParameterDirection.Output
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
            intRowCount = CInt(cmd.Parameters("@intRowCount").Value)
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

    Private Function FetchList2(ByVal udt As structSearchSalesFilter, Optional ByRef intRowCount As Integer = 0) As Object
        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader
        Dim cmd As New SqlCommand

        dr = Nothing
        FetchList2 = Nothing
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswSalesItemGetList2"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intCode", SqlDbType.Int).Value = udt.Code
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = udt.CodeSite
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = udt.SetPriceSale
                If udt.Name.Trim <> "" Then .Parameters.Add("@nvcName", SqlDbType.NVarChar, 200).Value = udt.Name
                .Parameters.Add("@bitExactMatch", SqlDbType.Bit).Value = udt.IsExactMatch
                .Parameters.Add("@numNumber", SqlDbType.Int).Value = udt.Number

                If udt.PriceFrom > -1 Then .Parameters.Add("@fltFromPriceRange", SqlDbType.Float).Value = udt.PriceFrom
                If udt.PriceTo > -1 Then .Parameters.Add("@fltToPriceRange", SqlDbType.Float).Value = udt.PriceTo

                .Parameters.Add("@intCodeTax", SqlDbType.Int).Value = udt.Tax
                .Parameters.Add("@bitProduct", SqlDbType.Bit).Value = udt.IncludeProduct
                .Parameters.Add("@bitRecipe", SqlDbType.Bit).Value = udt.IncludeRecipe
                .Parameters.Add("@bitMenu", SqlDbType.Bit).Value = udt.IncludeMenu
                .Parameters.Add("@bitNoType", SqlDbType.Bit).Value = udt.IncludeNoTypes
                .Parameters.Add("@tntNotLinked", SqlDbType.TinyInt).Value = Math.Abs(udt.Linked)
                .Parameters.Add("@intPageSize", SqlDbType.Int).Value = udt.intPageSize
                .Parameters.Add("@intPageIndex", SqlDbType.Int).Value = udt.intPageIndex
                .Parameters.Add("@vchCodeList", SqlDbType.VarChar, 4000).Value = udt.strCodeList
                .Parameters.Add("@intRowCount", SqlDbType.Int).Value = intRowCount
                .Parameters("@intRowCount").Direction = ParameterDirection.Output
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
            intRowCount = CInt(cmd.Parameters("@intRowCount").Value)
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

    Private Function SaveIntoListePriceHistory(ByVal intcodeSalesItem As Integer, ByVal intCodeSetPrice As Integer, _
        ByVal dblPrice As Double, ByVal intCodeTax As Integer, ByVal dtmDateValid As Date, ByVal tranMode As enumEgswTransactionMode, _
        Optional ByRef sqlTran As SqlTransaction = Nothing) As enumEgswErrorCode
        Dim sqlCmd As SqlCommand = New SqlCommand

        Try
            With sqlCmd
                If sqlTran Is Nothing Then
                    .Connection = New SqlConnection(L_strCnn)
                    .Connection.Open()
                Else
                    .Connection = sqlTran.Connection
                    .Transaction = sqlTran
                End If

                .CommandType = CommandType.StoredProcedure
                .CommandText = "sp_EgswSalesItemPriceHistoryUpdate"
                .Parameters.Add("@intCodeSalesItem", SqlDbType.Int).Value = intcodeSalesItem
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice
                .Parameters.Add("@fltPrice", SqlDbType.Float).Value = dblPrice
                .Parameters.Add("@intCodeTax", SqlDbType.Int).Value = intCodeTax
                .Parameters.Add("@DateValid", SqlDbType.DateTime).Value = dtmDateValid
                .Parameters.Add("@tntTranMode", SqlDbType.Int).Value = tranMode
                .Parameters.Add("@retval", SqlDbType.Int)

                .Parameters("@retval").Direction = ParameterDirection.ReturnValue
                .ExecuteNonQuery()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                If sqlTran Is Nothing AndAlso sqlCmd.Connection.State <> ConnectionState.Closed Then sqlCmd.Connection.Close()
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If sqlTran Is Nothing AndAlso sqlCmd.Connection.State <> ConnectionState.Closed Then sqlCmd.Connection.Close()
            sqlCmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
        Return L_ErrCode

    End Function

    Private Function SaveIntoListPrice(ByVal intCodeSalesItem As Integer, ByVal intCodesetPrice As Integer, _
        ByVal dblPrice As Double, ByVal dblCoeff As Double, ByVal intCodeTax As Integer, ByVal dblSuggestedPrice As Double, _
        ByVal intStatus As Integer, Optional ByRef sqlTran As SqlTransaction = Nothing) As enumEgswErrorCode
        Dim sqlCmd As SqlCommand = New SqlCommand

        Try
            With sqlCmd
                If sqlTran Is Nothing Then
                    .Connection = New SqlConnection(L_strCnn)
                    .Connection.Open()
                Else
                    .Connection = sqlTran.Connection
                    .Transaction = sqlTran
                End If

                .CommandType = CommandType.StoredProcedure
                .CommandText = "sp_EgswSalesItemPriceUpdateOnly"
                .Parameters.Add("@intCodeSalesItem", SqlDbType.Int).Value = intCodeSalesItem
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodesetPrice
                .Parameters.Add("@fltPrice", SqlDbType.Float).Value = dblPrice
                .Parameters.Add("@fltCoeff", SqlDbType.Float).Value = dblCoeff
                .Parameters.Add("@intCodeTax", SqlDbType.Int).Value = intCodeTax
                .Parameters.Add("@SuggestedPrice", SqlDbType.Float).Value = dblSuggestedPrice
                .Parameters.Add("@Status", SqlDbType.Int).Value = intStatus
                .Parameters.Add("@retval", SqlDbType.Int)

                .Parameters("@retval").Direction = ParameterDirection.ReturnValue
                .ExecuteNonQuery()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                If sqlTran Is Nothing AndAlso sqlCmd.Connection.State <> ConnectionState.Closed Then sqlCmd.Connection.Close()
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If sqlTran Is Nothing AndAlso sqlCmd.Connection.State <> ConnectionState.Closed Then sqlCmd.Connection.Close()
            sqlCmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
        Return L_ErrCode
    End Function

    Private Function SaveIntoList(ByVal udtSalesItem As structSalesItem, ByRef intCode As Int32, _
    ByVal TranMode As enumEgswTransactionMode, _
    Optional ByVal intCodeListe As Int32 = -1, Optional ByVal intOldNumber As Integer = -1, _
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

                .CommandText = "sp_EgswSalesItemUpdate"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intNewNumber", SqlDbType.Int).Value = udtSalesItem.Number                
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = L_User.Code
                .Parameters.Add("@tintType", SqlDbType.TinyInt).Value = udtSalesItem.Type
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 200).Value = udtSalesItem.Name
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = udtSalesItem.CodeSite
                .Parameters.Add("@nvcDescription", SqlDbType.NVarChar, 200).Value = udtSalesItem.Description
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = udtSalesItem.CodeSetPrice
                .Parameters.Add("@fltPrice", SqlDbType.Float).Value = udtSalesItem.Price
                .Parameters.Add("@fltCoeff", SqlDbType.Float).Value = udtSalesItem.Coeff
                .Parameters.Add("@intCodeTax", SqlDbType.Int).Value = udtSalesItem.CodeTax
                .Parameters.Add("@LinkMissing", SqlDbType.Bit).Value = CByte(udtSalesItem.LinkMissing)
                .Parameters.Add("@LastImport", SqlDbType.Bit).Value = CByte(udtSalesItem.LastImport)
                .Parameters.Add("@Archive", SqlDbType.Bit).Value = CByte(udtSalesItem.Archive)
                .Parameters.Add("@SuggestedPrice", SqlDbType.Float).Value = udtSalesItem.SuggestedPrice
                .Parameters.Add("@nvcBarcode", SqlDbType.NVarChar, 20).Value = udtSalesItem.Barcode
                .Parameters.Add("@bitActive", SqlDbType.Bit).Value = udtSalesItem.Active
                .Parameters.Add("@bitPrintItem", SqlDbType.Bit).Value = udtSalesItem.Print

                If TranMode = -1 Then
                    .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
                    .Parameters.Add("@intOldNumber", SqlDbType.Int).Value = intOldNumber
                Else
                    .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = 0
                    .Parameters.Add("@intOldNumber", SqlDbType.Int).Value = 0
                End If

                .Parameters("@intCode").Direction = ParameterDirection.InputOutput
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                If oTransaction Is Nothing Then .Connection.Open()
                .ExecuteNonQuery()
                If oTransaction Is Nothing Then .Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                intCode = CInt(.Parameters("@intCode").Value)
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

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="intCodeSalesItem"></param>
    ''' <param name="strCodeSetPriceList"></param>
    ''' <param name="blnDeleteNotInList">if it will delete set prices not in list for the salesitem</param>
    ''' <param name="sqlTran"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function RemoveFromListPrice(ByVal intCodeSalesItem As Integer, ByVal strCodeSetPriceList As String, _
        ByVal blnDeleteNotInList As Boolean, Optional ByRef sqlTran As SqlTransaction = Nothing) As enumEgswErrorCode

        Dim sqlCmd As SqlCommand = New SqlCommand

        Try
            With sqlCmd
                If sqlTran Is Nothing Then
                    .Connection = New SqlConnection(L_strCnn)
                Else
                    .Connection = sqlTran.Connection
                    .Transaction = sqlTran
                End If

                .CommandType = CommandType.StoredProcedure
                .CommandText = "sp_EgswSalesItemPriceDelete"
                .Parameters.Add("@intCodeSalesItem", SqlDbType.Int).Value = intCodeSalesItem
                .Parameters.Add("@nvcCodeSetPriceList", SqlDbType.NVarChar, 4000).Value = strCodeSetPriceList
                .Parameters.Add("@bitDeleteNotInCodeSetPriceList", SqlDbType.Bit).Value = CInt(blnDeleteNotInList)
                .Parameters.Add("@retval", SqlDbType.Int)

                .Parameters("@retval").Direction = ParameterDirection.ReturnValue
                .ExecuteNonQuery()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                If sqlTran Is Nothing AndAlso sqlCmd.Connection.State <> ConnectionState.Closed Then sqlCmd.Connection.Close()
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If sqlTran Is Nothing AndAlso sqlCmd.Connection.State <> ConnectionState.Closed Then sqlCmd.Connection.Close()
            sqlCmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Private Function RemoveFromList(ByVal intCode As Int32, _
        ByVal TranMode As enumEgswTransactionMode, _
        Optional ByVal strCodeList As String = "", Optional ByVal blnPreviewOnly As Boolean = True, Optional ByRef dtPreview As DataTable = Nothing, _
        Optional ByVal intCodeListe As Int32 = -1, Optional ByVal intOldNumber As Integer = -1, _
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

                .CommandText = "sp_EgswSalesItemUpdate"
                .CommandType = CommandType.StoredProcedure

                If TranMode = -1 Then
                    .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
                    .Parameters.Add("@intOldNumber", SqlDbType.Int).Value = intOldNumber
                Else
                    .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = 0
                    .Parameters.Add("@intOldNumber", SqlDbType.Int).Value = 0
                End If
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = L_User.Code
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = L_User.Site.Code
                .Parameters.Add("@intNewNumber", SqlDbType.Int).Value = 0
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@retval", SqlDbType.Int)

                strCodeList.Trim()
                If strCodeList <> "" Then
                    .Parameters.Add("@nvcCodeList", SqlDbType.NVarChar, 4000).Value = strCodeList
                    .Parameters.Add("@bitPreviewOnly", SqlDbType.Bit).Value = CInt(blnPreviewOnly)
                End If

                .Parameters("@intCode").Direction = ParameterDirection.InputOutput
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                If oTransaction Is Nothing Then .Connection.Open()

                If blnPreviewOnly Then
                    Dim sqlDa As New SqlDataAdapter
                    sqlDa.SelectCommand = cmd

                    With sqlDa
                        dtPreview = New DataTable
                        .SelectCommand = cmd
                        dtPreview.BeginLoadData()
                        .Fill(dtPreview)
                        dtPreview.EndLoadData()
                    End With
                Else
                    .ExecuteNonQuery()
                End If

                If oTransaction Is Nothing Then .Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                intCode = CInt(.Parameters("@intCode").Value)
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

    'moved to clsLinkfbRnPos
    'Private Function UpdateLinkFbRnPOS(ByRef intID As Integer, ByVal intTypeLink As Integer, _
    '    ByVal TranMode As enumEgswTransactionMode, ByVal intCodeProduct As Object, ByVal intCodeListe As Object, _
    '    ByVal intCodeSalesItem As Integer, ByVal dblFactor As Double, ByVal intPriceUpdate As Integer, _
    '    ByVal intCodeUnitProduct As Integer, ByVal intCodeUnitListe As Integer, ByVal blnDefLink As Boolean, _
    '    Optional ByVal sqlTran As SqlTransaction = Nothing) As enumEgswErrorCode

    '    Dim sqlCmd As SqlCommand = New SqlCommand
    '    If sqlTran Is Nothing Then
    '        sqlCmd.Connection = New SqlConnection(L_strCnn)
    '        sqlCmd.Connection.Open()
    '    Else
    '        sqlCmd.Connection = sqlTran.Connection
    '        sqlCmd.Transaction = sqlTran
    '    End If

    '    Try
    '        With sqlCmd
    '            .CommandText = "sp_EgswLinkFbRnPOSUpdate"
    '            .CommandType = CommandType.StoredProcedure

    '            .Parameters.Add("@intID", SqlDbType.Int).Value = intID
    '            .Parameters.Add("@TypeLink", SqlDbType.TinyInt).Value = intTypeLink
    '            .Parameters.Add("@TranMode", SqlDbType.TinyInt).Value = TranMode
    '            .Parameters.Add("@CodeProduct", SqlDbType.Int).Value = intCodeProduct
    '            .Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe
    '            .Parameters.Add("@CodeSalesItem", SqlDbType.Int).Value = intCodeSalesItem
    '            .Parameters.Add("@Factor", SqlDbType.Float).Value = dblFactor
    '            .Parameters.Add("@PriceUpdate", SqlDbType.TinyInt).Value = intPriceUpdate
    '            .Parameters.Add("@CodeUnitProduct", SqlDbType.Int).Value = intCodeUnitProduct
    '            .Parameters.Add("@CodeUnitListe", SqlDbType.Int).Value = intCodeUnitListe
    '            .Parameters.Add("@DefLink", SqlDbType.Bit).Value = blnDefLink
    '            .Parameters.Add("@retval", SqlDbType.Int)

    '            .Parameters("@intID").Direction = ParameterDirection.InputOutput
    '            .Parameters("@retval").Direction = ParameterDirection.ReturnValue

    '            .ExecuteNonQuery()
    '            L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
    '            If sqlTran Is Nothing AndAlso sqlCmd.Connection.State <> ConnectionState.Closed Then sqlCmd.Connection.Close()
    '        End With
    '    Catch ex As Exception
    '        L_ErrCode = enumEgswErrorCode.GeneralError
    '        If sqlTran Is Nothing AndAlso sqlCmd.Connection.State <> ConnectionState.Closed Then sqlCmd.Connection.Close()
    '        sqlCmd.Dispose()
    '        Throw New Exception(ex.Message, ex)
    '    End Try
    '    Return L_ErrCode
    'End Function

    Private Function UpdateFlagList(ByVal strCodeList As String, ByVal TranMode As enumEgswTransactionMode, _
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

                .CommandText = "sp_EgswSalesItemUpdate"
                .CommandType = CommandType.StoredProcedure

    
                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = 0
                .Parameters.Add("@intOldNumber", SqlDbType.Int).Value = 0

                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = L_User.Code
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = L_User.Site.Code
                .Parameters.Add("@intNewNumber", SqlDbType.Int).Value = 0
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                .Parameters.Add("@intCode", SqlDbType.Int).Value = -1
                .Parameters.Add("@retval", SqlDbType.Int)

                strCodeList.Trim()
                If strCodeList <> "" Then
                    .Parameters.Add("@nvcCodeList", SqlDbType.NVarChar, 4000).Value = strCodeList
                End If

                .Parameters("@intCode").Direction = ParameterDirection.InputOutput
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
    ''' get sales item price by code sales item and set price
    ''' </summary>
    ''' <param name="intCodeSalesItem"></param>
    ''' <param name="intCodeSetPrice"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListPrice(ByVal intCodeSalesItem As Integer, ByVal intCodeSetPrice As Integer) As Object
        Return FetchListPrice(intCodeSalesItem, intCodeSetPrice)
    End Function
    ''' <summary>
    ''' get sales item price by code sales item
    ''' </summary>
    ''' <param name="intCodeSalesItem"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListPrice(ByVal intCodeSalesItem As Integer) As Object
        Return FetchListPrice(intCodeSalesItem, -1)
    End Function

    Public Function GetListPriceHistory(ByVal intCodeSalesItem As Integer) As Object
        Return FetchListPriceHistory(intCodeSalesItem, -1)
    End Function

    Public Function GetListPriceHistory(ByVal intCodeSalesItem As Integer, ByVal intcodeSetPrice As Integer) As Object
        Return FetchListPriceHistory(intCodeSalesItem, intcodeSetPrice)
    End Function



    ''' <summary>
    ''' Get Sales Item by Code.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>    
    Public Overloads Function GetList(ByVal intCode As Int32) As Object
        SetFilterDefault(L_udtFilter)
        L_udtFilter.Code = intCode
        Return FetchList(L_udtFilter)
    End Function
    ''' <summary>
    ''' Get Sales Item by Code with rowcount return.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>    
    Public Overloads Function GetList(ByVal intCode As Int32, ByRef intRowCount As Integer) As Object
        SetFilterDefault(L_udtFilter)
        L_udtFilter.Code = CInt(intCode)
        Return FetchList(L_udtFilter, intRowCount)
    End Function
    ''' <summary>
    ''' Get a SalesItem by Name.
    ''' </summary>
    ''' <param name="strName">The name of the SalesItem to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal strName As String, ByVal blnExactMatch As Boolean) As Object
        SetFilterDefault(L_udtFilter)
        L_udtFilter.Name = strName
        L_udtFilter.IsExactMatch = blnExactMatch
        Return FetchList(L_udtFilter)
    End Function
    ''' <summary>
    ''' Get a SalesItem by filter values.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal udt As structSearchSalesFilter, Optional ByRef intRowCount As Integer = 0) As Object 'DataTable
        Return FetchList(udt, intRowCount)
    End Function

    ''' <summary>
    ''' Get a SalesItem by filter values.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList2(ByVal udt As structSearchSalesFilter, Optional ByRef intRowCount As Integer = 0) As Object 'DataTable
        Return FetchList2(udt, intRowCount)
    End Function

    ''' <summary>
    ''' Get a SalesItem Name.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSalesItemName(ByVal intCode As Int32) As String
        Dim dr As SqlDataReader = CType(GetList(intCode), SqlDataReader)
        If dr IsNot Nothing Then
            Do While dr.Read
                Return (dr.GetValue(dr.GetOrdinal("Name")).ToString)
            Loop
        Else
            Return ""
        End If
    End Function
    ''' <summary>
    ''' Get Code of a SalesItem by Name.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCodeSalesItem(ByVal strSalesItemName As String) As Int32
        Dim dr As SqlDataReader = CType(GetList(strSalesItemName, True), SqlDataReader)
        If dr IsNot Nothing Then
            Do While dr.Read
                Return CInt((dr.GetValue(dr.GetOrdinal("Code")).ToString))
            Loop
        Else
            Return 0
        End If
    End Function

    Private Function GetLinked(ByVal intCodeListe As Integer, ByVal intCodeProduct As Integer, ByVal intCodeSite As Integer, Optional ByVal intCodeSetPrice As Integer = -1, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Dim strCommandText As String = "SALES_GetLinked"

        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeListe", intCodeListe)
        arrParam(1) = New SqlParameter("@intCodeProduct", intCodeProduct)
        arrParam(2) = New SqlParameter("@intCodesite", intCodeSite)
        arrParam(3) = New SqlParameter("@intCodesetPrice", intCodeSetPrice)

        Try
            If fetchType = enumEgswFetchType.UseDefault Then fetchType = L_bytFetchType
            Select Case fetchType
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

    Public Function GetLinkedToProduct(ByVal intCodeProduct As Integer, ByVal intCodeSite As Integer, Optional ByVal intCodeSetPrice As Integer = -1, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Return GetLinked(-1, intCodeProduct, intCodeSite, intCodeSetPrice, fetchType)
    End Function

    Public Function GetLinkedToListe(ByVal intCodeListe As Integer, ByVal intCodeSite As Integer, Optional ByVal intCodeSetPrice As Integer = -1, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Return GetLinked(intCodeListe, -1, intCodeSite, intCodeSetPrice, fetchType)
    End Function

    Public Function GetBlankSalesItem() As structSalesItem
        Dim udtSalesItem As structSalesItem
        With udtSalesItem
            .Active = True
            .Archive = True
            .Barcode = ""
            .Code = -1
            .CodeSetPrice = L_User.LastSetPriceSales
            .CodeSite = L_User.Site.Code
            .CodeTax = 1
            .Coeff = 1
            .DateModified = Now.Date
            .Description = ""
            .LastImport = True
            .LinkMissing = True
            .Name = ""
            .Number = ""
            .Price = 0
            .Print = True
            .SuggestedPrice = 0
            .Type = 1
        End With
        Return udtSalesItem
    End Function

#End Region

#Region "Update Methods"

    Public Function UpdateActiveFlag(ByVal strCodeListeList As String, ByVal blnFlag As Boolean) As enumEgswErrorCode
        If blnFlag Then
            Return UpdateFlagList(strCodeListeList, enumEgswTransactionMode.FlagActive)
        Else
            Return UpdateFlagList(strCodeListeList, enumEgswTransactionMode.FlagDeactivate)
        End If
    End Function

    Public Function UpdatePrintFlag(ByVal strCodeListeList As String, ByVal blnFlag As Boolean) As enumEgswErrorCode
        If blnFlag Then
            Return UpdateFlagList(strCodeListeList, enumEgswTransactionMode.FlagPrint)
        Else
            Return UpdateFlagList(strCodeListeList, enumEgswTransactionMode.FlagDeprint)
        End If
    End Function

    ''' <summary>
    ''' Updates SalesItem
    ''' </summary>
    ''' <param name="intCode">The Code of the SalesItem to be updated.</param>
    ''' <param name="udtSalesItem">One of the structSalesItem values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByRef intCode As Int32, ByVal udtSalesItem As structSalesItem) As enumEgswErrorCode
        Return SaveIntoList(udtSalesItem, intCode, _
             CType(IIf(intCode < 1, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))
    End Function

    ''' <summary>
    ''' used to update the salesitem price table, it deletes setprices not in the strCodeSetPriceList
    ''' </summary>
    ''' <param name="intcodeSalesItem">code of the sales item</param>
    ''' <param name="dt">datatable conatining a replica of salesitem price</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdatePrice(ByVal intcodeSalesItem As Integer, ByVal dt As DataTable) As enumEgswErrorCode
        Dim sqlCnn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlTran As SqlTransaction
        sqlCnn.Open()
        sqlTran = sqlCnn.BeginTransaction
        Try
            Dim dv As DataView = dt.DefaultView
            dv.Sort = "Status" 'this was done tod elete all exsiting records first
            Dim drv As DataRowView
            For Each drv In dv
                If Not CIntDB(drv("Status")) = 1 Then
                    If SaveIntoListPrice(intcodeSalesItem, CInt(drv("CodesetPrice")), CDbl(drv("Price")), _
                                    CDblDB(drv("Coeff")), CIntDB(drv("CodeTax")), CDblDB(drv("SuggestedPrice")), CIntDB(drv("Status")), sqlTran) <> enumEgswErrorCode.OK Then
                        sqlTran.Rollback()
                        Return enumEgswErrorCode.GeneralError
                    End If
                End If
            Next
            sqlTran.Commit()
            sqlCnn.Close()
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            sqlTran.Rollback()
            sqlCnn.Close()
        End Try
    End Function

    Public Function UpdatePriceHistory(ByVal intcodeSalesItem As Integer, ByVal dt As DataTable) As enumEgswErrorCode
        Dim sqlCnn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlTran As SqlTransaction
        sqlCnn.Open()
        sqlTran = sqlCnn.BeginTransaction
        Try
            Dim dv As DataView = dt.DefaultView
            dv.RowFilter = ""
            dv.Sort = "Status DESC" 'this was done to delete all exsiting records first
            Dim drv As DataRowView
            For Each drv In dv
                If Not CIntDB(drv("Status")) = 0 Then
                    If Me.SaveIntoListePriceHistory(intcodeSalesItem, CInt(drv("CodesetPrice")), CDbl(drv("Price")), _
                        CIntDB(drv("CodeTax")), CDateDB(drv("DateValid")), CType(drv("Status"), enumEgswTransactionMode), sqlTran) <> enumEgswErrorCode.OK Then
                        sqlTran.Rollback()
                        Return enumEgswErrorCode.GeneralError
                    End If
                End If
            Next
            sqlTran.Commit()
            sqlCnn.Close()
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            sqlTran.Rollback()
            sqlCnn.Close()
        End Try
    End Function

    ''' <summary>
    ''' Create Sales Item from Merchandise, Recipe, or Menu of Cm
    ''' </summary>
    ''' <param name="intCodeListe">The Code of Merchandise, Recipe, or Menu</param>
    ''' <param name="intOldNumber">Sales Item Number to be updated.</param>
    ''' <param name="intNewNumber"> Given Sales Item Number.</param>
    ''' <param name="intType"> 1=Merchandise, 2=Recipe, 3= Menu.</param>
    ''' <param name="udtSalesItem"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal intCodeListe As Int32, ByVal intOldNumber As Integer, ByVal intNewNumber As Integer, ByVal intType As Integer, _
        ByVal udtSalesItem As structSalesItem) As enumEgswErrorCode

        Return SaveIntoList(udtSalesItem, -1, enumEgswTransactionMode.None, intCodeListe, intOldNumber)

    End Function

    Public Function UpdateLinkForFG(ByRef intID As Integer, _
        ByVal intCodeProduct As Integer, ByVal intCodeSalesItem As Integer, ByVal dblFactor As Double, _
        ByVal intCodeUnitProduct As Integer, ByVal intCodeUnitListe As Integer) As enumEgswErrorCode
        Dim cLinkFbRnPos As clsLinkFbRnPos = New clsLinkFbRnPos(L_strCnn)
        Return cLinkFbRnPos.UpdateLinkFbRnPOS(intID, 2, enumEgswTransactionMode.Add, intCodeProduct, DBNull.Value, intCodeSalesItem, dblFactor, 1, intCodeUnitProduct, intCodeUnitListe, False)
    End Function

    Public Function UpdateLinkForProduct(ByRef intID As Integer, _
        ByVal intCodeProduct As Integer, ByVal intCodeSalesItem As Integer, ByVal dblFactor As Double, _
        ByVal intCodeUnitProduct As Integer, ByVal intCodeUnitListe As Integer) As enumEgswErrorCode
        Dim cLinkFbRnPos As clsLinkFbRnPos = New clsLinkFbRnPos(L_strCnn)
        Return cLinkFbRnPos.UpdateLinkFbRnPOS(intID, 1, enumEgswTransactionMode.Add, intCodeProduct, DBNull.Value, intCodeSalesItem, dblFactor, 1, intCodeUnitProduct, intCodeUnitListe, False)
    End Function

    'Public Function UpdateLinkForListe(ByRef intID As Integer, _
    'ByVal intCodeListe As Integer, ByVal intCodeSalesItem As Integer, ByVal dblFactor As Double, _
    'ByVal intCodeUnitProduct As Integer, ByVal intCodeUnitListe As Integer) As enumEgswErrorCode
    '    Return UpdateLinkFbRnPOS(intID, 2, enumEgswTransactionMode.Add, DBNull.Value, intCodeListe, intCodeSalesItem, dblFactor, 1, intCodeUnitProduct, intCodeUnitListe, False)
    'End Function

#End Region

#Region "Remove Methods"
    ''' <summary>
    ''' Purge Sales Item List
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal strCodeList As String, ByVal blnPreviewOnly As Boolean, ByRef dt As DataTable) As enumEgswErrorCode
        Return RemoveFromList(-1, enumEgswTransactionMode.Delete, strCodeList, blnPreviewOnly, dt)
    End Function

    ''' <summary>
    ''' Delete a SalesItem.
    ''' </summary>
    ''' <param name="intCode">The Code of the Terminal to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal intCode As Int32) As enumEgswErrorCode
        Return RemoveFromList(intCode, enumEgswTransactionMode.Delete)
    End Function

    Public Function DeleteLinkForFG(ByVal intCodeProduct As Integer, ByVal intCodeSalesItem As Integer) As enumEgswErrorCode
        Dim cLinkFbRnPos As clsLinkFbRnPos = New clsLinkFbRnPos(L_strCnn)
        Return cLinkFbRnPos.UpdateLinkFbRnPOS(1, 2, enumEgswTransactionMode.Delete, intCodeProduct, DBNull.Value, intCodeSalesItem, 1, 1, 0, 0, False)
    End Function

    Public Function DeleteListeLinkForProduct(ByVal intCodeProduct As Integer, ByVal intCodeSalesItem As Integer) As enumEgswErrorCode
        Dim cLinkFbRnPos As clsLinkFbRnPos = New clsLinkFbRnPos(L_strCnn)
        Return cLinkFbRnPos.UpdateLinkFbRnPOS(1, 1, enumEgswTransactionMode.Delete, intCodeProduct, DBNull.Value, intCodeSalesItem, 1, 1, 0, 0, False)
    End Function

    'Public Function DeleteListeLinkForListe(ByVal intCodeListe As Integer, ByVal intCodeSalesItem As Integer) As enumEgswErrorCode
    '    Return UpdateLinkFbRnPOS(1, 1, enumEgswTransactionMode.Delete, DBNull.Value, intCodeListe, intCodeSalesItem, 1, 1, 0, 0, False)
    'End Function

#End Region

#Region "Other Public Functions and Methods"
    Public Sub SetFilterDefault(ByRef udtFilter As structSearchSalesFilter)
        With L_udtFilter
            .Code = -1
            .CodeSite = -1
            .Name = ""
            .Number = 0
            .Tax = -1
            .SetPriceSale = -1
            .PriceFrom = -1
            .PriceTo = -1
            .IncludeProduct = False
            .IncludeRecipe = False
            .IncludeMenu = False
            .IncludeNoTypes = False
            .Linked = 0
            .IsExactMatch = False
            .intPageIndex = 0
            .intPageSize = 0
            .strCodeList = ""
        End With
        udtFilter = L_udtFilter
    End Sub

    Public Function SalesItemPriceBlankTable() As DataTable
        Dim dt As DataTable = New DataTable
        dt.Columns.Add("CodeSalesItem")
        dt.Columns.Add("CodesetPrice")
        dt.Columns.Add("Price")
        dt.Columns.Add("Coeff")
        dt.Columns.Add("CodeTax")
        dt.Columns.Add("SuggestedPrice")
        dt.Columns.Add("TaxValue")
        dt.Columns.Add("SetPriceName")

        Dim cSetPrice As clsSetPrice = New clsSetPrice(L_User, enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataReader)
        Dim dr As SqlDataReader = CType(cSetPrice.GetList(CByte(1), L_User.Site.Code, SetPriceType.Sale), SqlDataReader)

        Dim rw As DataRow
        While dr.Read
            rw = dt.NewRow
            rw("CodesetPrice") = dr("Code")
            rw("Price") = 0
            rw("Coeff") = 0
            rw("CodeTax") = 0
            rw("SuggestedPrice") = 0
            rw("TaxValue") = 0
            rw("SetPriceName") = dr("name")
            dt.Rows.Add(rw)
        End While

        Return dt
    End Function
#End Region

End Class

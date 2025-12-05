Imports System.Data.SqlClient
Imports System.Data

Public Class clsSales


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
    Private L_lngID As Int32

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

    Public Function FetchSalesList(ByVal udtSalesHistoryFilter As structSearchSalesHistoryFilter, Optional ByRef intRowCount As Integer = 0) As Object
        Dim cmd As New SqlCommand

        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader
        dr = Nothing
        FetchSalesList = Nothing
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswSalesGetList"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intID", SqlDbType.Int).Value = udtSalesHistoryFilter.ID
                '.Parameters.Add("@CodeUser", SqlDbType.Int).Value = L_User.Code
                If udtSalesHistoryFilter.DateSalesFrom <> Nothing Then .Parameters.Add("@dtSalesDateFrom", SqlDbType.DateTime).Value = udtSalesHistoryFilter.DateSalesFrom
                If udtSalesHistoryFilter.DateSalesTo <> Nothing Then .Parameters.Add("@dtSalesDateTo", SqlDbType.DateTime).Value = udtSalesHistoryFilter.DateSalesTo
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = udtSalesHistoryFilter.CodeSetPrice
                '.Parameters.Add("@bitSalesList", SqlDbType.Bit).Value = udtSalesHistoryFilter.MarkedSalesList
                '.Parameters.Add("@nvcSiteList", SqlDbType.NVarChar, 1000).Value = udtSalesHistoryFilter.SiteList
                '.Parameters.Add("@bitTerminal", SqlDbType.Int).Value = udtSalesHistoryFilter.MarkedTerminal
                '.Parameters.Add("@bitIssuanceType", SqlDbType.Bit).Value = udtSalesHistoryFilter.MarkedIssuanceType
                '.Parameters.Add("@bitProduct", SqlDbType.Bit).Value = udtSalesHistoryFilter.IncludeProduct
                '.Parameters.Add("@bitRecipe", SqlDbType.Bit).Value = udtSalesHistoryFilter.IncludeRecipe
                '.Parameters.Add("@bitMenu", SqlDbType.Bit).Value = udtSalesHistoryFilter.IncludeMenu
                .Parameters.Add("@bitOutputDone", SqlDbType.Int).Value = udtSalesHistoryFilter.OutputDone
                If udtSalesHistoryFilter.ItemName <> "" Then .Parameters.Add("@nvcSalesItemName", SqlDbType.NVarChar).Value = udtSalesHistoryFilter.ItemName
                'If udtSalesHistoryFilter.ItemNumber <> "" Then .Parameters.Add("@nvcSalesItemNumber", SqlDbType.NVarChar).Value = udtSalesHistoryFilter.ItemNumber
                '.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = udtSalesHistoryFilter
                '.Parameters.Add("@intCodeCategory", SqlDbType.Int).Value = udtSalesHistoryFilter.CodeCategory
                'If udtSalesHistoryFilter.MerchandiseKeyword <> "" Then .Parameters.Add("@nvcMerchandiseKeyword", SqlDbType.NVarChar).Value = udtSalesHistoryFilter.MerchandiseKeyword
                'If udtSalesHistoryFilter.RecipeKeyword <> "" Then .Parameters.Add("@nvcRecipeKeyword", SqlDbType.NVarChar).Value = udtSalesHistoryFilter.RecipeKeyword
                
                '.Parameters.Add("@intRowCount", SqlDbType.Int).Value = intRowCount
                '.Parameters("@intRowCount").Direction = ParameterDirection.Output
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
            'intRowCount = CInt(cmd.Parameters("@intRowCount").Value)
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

    Public Function FetchSalesTmpList(ByVal lngCode As Int32, _
        Optional ByVal strName As String = "") As Object
        Dim cmd As New SqlCommand

        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader

        dr = Nothing
        FetchSalesTmpList = Nothing
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswSalesTmpGetList"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intID", SqlDbType.Int).Value = -1
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

    Public Function FetchSalesImport(ByVal udtSalesHistoryFilter As structSearchSalesHistoryFilter, Optional ByRef intRowCount As Integer = 0) As Object
        Dim cmd As New SqlCommand

        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader
        dr = Nothing
        FetchSalesImport = Nothing
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswSalesGetImportSummary"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
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
            'intRowCount = CInt(cmd.Parameters("@intRowCount").Value)
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

End Class

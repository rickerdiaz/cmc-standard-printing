Imports System.Data.SqlClient

Public Class clsInventory
    Inherits clsDBRoutine

    Private L_udtUser As structUser
    Private L_bytFetchType As enumEgswFetchType
    Private L_strCnn As String
    Private L_ErrCode As enumEgswErrorCode

    Public Sub New(ByVal udtUser As structUser, ByVal strCnn As String, _
      Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)

        Try
            If udtUser.Code <= 0 Then Throw New Exception("Invalid CodeUser.")
            L_udtUser = udtUser
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn

        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try
    End Sub

#Region " GET FUNCTION "
    Public Function GetProductList(ByVal intProductFrom As Integer, Optional ByVal strMarkedItems As String = "") As Object
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "INV_GETPRODUCTLIST"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = L_udtUser.Site.Code
                .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = L_udtUser.CodeTrans
                .Parameters.Add("@ProductFrom", SqlDbType.Int).Value = intProductFrom
                .Parameters.Add("@MarkedItems", SqlDbType.VarChar, 8000).Value = strMarkedItems

                .Connection.Open()
                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                cmd.Dispose()
            End With
            Return dt
        Catch ex As Exception
            cmd.Connection.Close()
            cmd.Dispose()
            Return Nothing
        End Try
    End Function

    Public Function GetDetails(ByVal intCode As Integer) As DataTable
        Dim cmd As New SqlCommand
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandType = CommandType.StoredProcedure
                .CommandText = "INVE_GETDETAILS"
                .Parameters.Add("@CodeInvent", SqlDbType.Int).Value = intCode
                .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = L_udtUser.CodeTrans
                .Connection.Open()

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                cmd.Dispose()
            End With

            Return dt
        Catch ex As Exception
            cmd.Dispose()
            Return Nothing
        End Try

    End Function

    Public Function GetList(ByVal intCode As Integer) As DataTable
        Dim cmd As New SqlCommand
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandType = CommandType.StoredProcedure
                .CommandText = "[INV_GetList]"
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = L_udtUser.Site.Code

                .Connection.Open()

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                cmd.Dispose()
            End With

            Return dt
        Catch ex As Exception
            cmd.Dispose()
            Return Nothing
        End Try
    End Function

    Public Function GetProductSearchList(ByVal strName As String, ByVal intNameOption As Integer, _
            ByVal intCategoryCode As Integer, ByVal intSupplierCode As Integer, _
            ByVal intLocationCode As Integer) As DataTable

        Dim cmd As New SqlCommand
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandType = CommandType.StoredProcedure
                .CommandText = "[INVE_GETPRODSEARCHLIST]"
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 500).Value = strName
                .Parameters.Add("@intNameOption", SqlDbType.Int).Value = intNameOption
                .Parameters.Add("@intCategory", SqlDbType.Int).Value = intCategoryCode
                .Parameters.Add("@intSupplier", SqlDbType.Int).Value = intSupplierCode
                .Parameters.Add("@intLocation", SqlDbType.Int).Value = intLocationCode
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = L_udtUser.CodeTrans

                .Connection.Open()

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                cmd.Dispose()
            End With

            Return dt
        Catch ex As Exception
            cmd.Dispose()
            Return Nothing
        End Try
    End Function
#End Region

#Region " UPDATE FUNCTION "
    'Public Function UpdateInventory(ByRef intCode As Integer, ByVal strMarkedItems As String, _
    '                                ByVal tInventory As structInventory) As enumEgswErrorCode

    '    Dim cmd As New SqlCommand
    '    Try
    '        With cmd
    '            .Connection = New SqlConnection(L_strCnn)
    '            .CommandText = "INV_UPDATELIST"
    '            .CommandType = CommandType.StoredProcedure
    '            .Parameters.Add("@DateBegin", SqlDbType.SmallDateTime).Value = tInventory.dteDateBegin
    '            .Parameters.Add("@Note", SqlDbType.NVarChar, 2000).Value = tInventory.strNote
    '            .Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = tInventory.strName
    '            .Parameters.Add("@CodeUser", SqlDbType.Int).Value = L_udtUser.Code
    '            .Parameters.Add("@CodeSite", SqlDbType.Int).Value = L_udtUser.Site.Code
    '            .Parameters.Add("@OpenFrom", SqlDbType.TinyInt).Value = tInventory.intOpenFrom
    '            .Parameters.Add("@Code", SqlDbType.Int).Value = intCode
    '            .Parameters.Add("@MarkedItems", SqlDbType.NVarChar).Value = strMarkedItems
    '            .Parameters("@Code").Direction = ParameterDirection.InputOutput
    '            .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

    '            .Connection.Open()
    '            .ExecuteNonQuery()
    '            intCode = CInt(.Parameters("@Code").Value)
    '            .Connection.Close()
    '            .Connection.Dispose()
    '            L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
    '        End With
    '    Catch ex As Exception
    '        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
    '        cmd.Dispose()
    '        Throw New Exception(ex.Message, ex)
    '    End Try

    '    Return L_ErrCode
    'End Function

    Public Function Update(ByRef intCode As Integer, ByVal strMarkedItems As String, _
                                        ByVal tInventory As structInventory) As enumEgswErrorCode

        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "INVE_UPDATE"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@DateBegin", SqlDbType.SmallDateTime).Value = tInventory.dteDateBegin
                .Parameters.Add("@Note", SqlDbType.NVarChar, 2000).Value = tInventory.strNote
                .Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = tInventory.strName
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = L_udtUser.Code
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = L_udtUser.Site.Code
                .Parameters.Add("@OpenFrom", SqlDbType.TinyInt).Value = tInventory.intOpenFrom
                .Parameters.Add("@Code", SqlDbType.Int).Value = intCode
                .Parameters.Add("@MarkedItems", SqlDbType.NVarChar).Value = strMarkedItems
                .Parameters("@Code").Direction = ParameterDirection.InputOutput
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                .ExecuteNonQuery()
                intCode = CInt(.Parameters("@Code").Value)
                .Connection.Close()
                .Connection.Dispose()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        Return L_ErrCode
    End Function

    Public Function UpdateDetails(ByVal intCodeProduct As Integer, ByVal intCodeLocation As Integer, _
                                  ByVal intCodeInvent As Integer, ByVal dblQtyEditPack As Double, _
                                  ByVal dblQtyEditStock As Double, ByVal dblPrice As Double) As enumEgswErrorCode

        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "INVE_UPDATEDETAILS"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@CodeProduct", SqlDbType.Int).Value = intCodeProduct
                .Parameters.Add("@CodeLocation", SqlDbType.Int).Value = intCodeLocation
                .Parameters.Add("@CodeInvent", SqlDbType.Int).Value = intCodeInvent
                .Parameters.Add("@QtyEditPack", SqlDbType.Float).Value = dblQtyEditPack
                .Parameters.Add("@QtyEditStock", SqlDbType.Float).Value = dblQtyEditStock
                .Parameters.Add("@Price", SqlDbType.Float).Value = dblPrice
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                .Connection.Dispose()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        Return L_ErrCode
    End Function
#End Region

#Region " CLOSE FUNCTION "
    Public Function CloseInventory(ByVal intCodeInventory As Integer, ByVal dtDateEnd As DateTime, _
                                   ByVal dtDateStart As DateTime) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[INVE_CLOSE]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeInventory", SqlDbType.Int).Value = intCodeInventory
                .Parameters.Add("@dteDateClose", SqlDbType.DateTime).Value = dtDateEnd
                .Parameters.Add("@intUserCode", SqlDbType.Int).Value = L_udtUser.Code
                .Parameters.Add("@dteStartDate2", SqlDbType.DateTime).Value = dtDateStart

                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                .Connection.Dispose()
                L_ErrCode = enumEgswErrorCode.OK
            End With
        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        Return L_ErrCode
    End Function
#End Region
End Class

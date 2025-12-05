Imports System.Data.SqlClient

Public Class clsProduct
    Inherits clsDBRoutine

    Private L_User As structUser
    Private L_bytFetchType As enumEgswFetchType
    Private L_strCnn As String
    Private L_ErrCode As enumEgswErrorCode

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

#Region " GET FUNCTION "
    Public Function GetDefaultStructProduct() As structProduct
        Dim udtProduct As structProduct = New structProduct
        With udtProduct
            .ActionFlag = False
            .AddInstruction = ""
            .AvgPrice = 0
            .AutoTransferOutlet = False
            .Barcode = ""
            .CodeSalesItem = 0
            .Composition = ""
            .ConsumptionDays = 0
            .ConsumptionText = ""
            .DaysExp = 0
            .Description = ""
            .DoNotLink = False
            .Economat = 0
            .ExcludeFromAutoOutput = False
            .GoodsRatio = 0
            .GoodsRecipeUnit = 0
            .InCurrentInventory = False
            .InInventory = False
            .InventPrice = 0
            .IsFresh = False
            .IsGlobal = False
            .IsSelfOrder = False
            .LastUnitUsed = 0
            .LocationOutDef = 0
            .LocationProdDef = 0
            .MultiSup = False
            .Name = ""
            .Note = ""
            .Number = ""
            .PackingDate = CDate(#1/1/1900#)
            .PackingText = ""
            .PriceMax = 0
            .PriceMin = 0
            .PriceUpdate = 0
            .Qty2Economat = 0
            .QtyAllocated = 0
            .QtyInOrder = 0
            .QtyInventory = 0
            .QtyMax = 0
            .QtyMin = 0
            .QtyOnHand = 0
            .QtyOrderMin = 0
            .QtyOrderMax = 0
            .QtyOrderLast = 0
            .QtyOrderDef = 0
            .QuantityEconomat = 0
            .RawMaterial = False
            .RecipeLinkCode = 0
            .StockingPlace = 0
            .SupplierNumber = ""
            .TransferFlag = False
            .Type = 0
            .UseIO = False
            .UnitPack = 0
            .UnitRatio2 = 0
            .UnitRatio3 = 0
            .UnitStock = 0
        End With
        Return udtProduct
    End Function

    Public Function GetProductHistory(ByVal intCodeUser As Integer, ByVal listeType As enumDataListType, Optional ByVal intCodeTrans As Integer = -1) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            .CommandText = "sp_egswProductHistoryGet"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
            .Parameters.Add("@intListeType", SqlDbType.Int).Value = listeType
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetLinkedProduct(ByVal blnLinked As Boolean, ByVal intCodeSite As Integer, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Dim shrtLinked As Short = CShort(IIf(blnLinked, 1, 2))
        Dim structWineFilter As structSearchWineFilter
        Return GetList(-1, intCodeSite, "", -1, "", -1, -1, -1, -1, shrtLinked, True, False, -1, -1, 0, 0, 0, "", structWineFilter, fetchType)
    End Function

    Public Function GetOne(ByVal intCodeProduct As Integer, ByVal intCodeSite As Integer, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault, Optional ByVal intCodeTrans As Integer = 0) As Object
        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandType = CommandType.StoredProcedure
                .CommandText = "prod_getlist"
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeProduct
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans 'DLS
            End With

            If fetchType = enumEgswFetchType.UseDefault Then fetchType = L_bytFetchType
            GetOne = ExecuteFetchType(fetchType, sqlCmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetOneDetailed(ByVal intCodeProduct As Integer, ByVal intCodeSite As Integer, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.DataTable, Optional ByVal intAccountType As Integer = 1) As Object
        Dim sqlCmd As SqlCommand = New SqlCommand
        Dim dt As New DataTable
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandType = CommandType.StoredProcedure
                .CommandText = "prod_getlistdetailed"
                .Parameters.Add("@intCodeProduct", SqlDbType.Int).Value = intCodeProduct
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@intOption", SqlDbType.Int).Value = intAccountType
            End With

            If fetchType = enumEgswFetchType.UseDefault Then fetchType = L_bytFetchType
            Return ExecuteFetchType(L_bytFetchType, sqlCmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetMerchandiseLink(ByVal intCodeProduct As Integer, ByRef intCodeListe As Integer, ByRef strListeName As String) As Boolean
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader

        With cmd
            .Connection = cn
            .CommandText = "SELECT CodeProduct, CodeListe, TypeLink, m.Name " & _
                            "FROM EgswLinkFBRNPos l INNER JOIN EgswListe m " & _
                            "ON l.CodeListe = m.Code " & _
                            "WHERE CodeProduct = @CodeProduct "
            .CommandType = CommandType.Text
            .Parameters.Add("@CodeProduct", SqlDbType.Int).Value = intCodeProduct

            Try
                cn.Open()
                dr = .ExecuteReader(CommandBehavior.CloseConnection)
                If dr.Read Then
                    intCodeListe = CIntDB(dr.Item("CodeListe"))
                    strListeName = CStrDB(dr.Item("Name"))
                End If
                dr.Close()
                cn.Close()
            Catch ex As Exception
                cn.Close()
                Throw ex
            End Try

        End With
    End Function

    Public Function GetProductCategoryName(ByVal intCodeProduct As Integer, ByRef strProductName As String, ByRef strCategoryName As String) As Boolean
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader

        With cmd
            .Connection = cn
            .CommandText = "SELECT p.code, p.name ProductName, p.CodeCategory, c.Name CategoryName " & _
                            "FROM egswProduct p INNER JOIN EgswCategory c " & _
                            "ON p.CodeCategory = c.Code " & _
                            "WHERE p.Code=@CodeProduct "
            .CommandType = CommandType.Text
            .Parameters.Add("@CodeProduct", SqlDbType.Int).Value = intCodeProduct

            Try
                cn.Open()
                dr = .ExecuteReader(CommandBehavior.CloseConnection)
                If dr.Read Then
                    strProductName = CStrDB(dr.Item("ProductName"))
                    strCategoryName = CStrDB(dr.Item("CategoryName"))
                End If
                dr.Close()
                cn.Close()
            Catch ex As Exception
                cn.Close()
                Throw ex
            End Try

        End With
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="intCode"></param>
    ''' <param name="fetchType"></param>
    ''' <returns></returns>
    ''' <remarks>MRC 05.06.09 - Added a structSearchWineFilter param</remarks>
    Public Function GetList(ByVal intCode As Integer, _
        ByVal intCodeSite As Integer, _
        ByVal strName As String, _
        ByVal shrtNameOption As Integer, _
        ByVal strNumber As String, _
        ByVal shrtNumberOption As Integer, _
        ByVal intCategory As Integer, _
        ByVal intSupplier As Integer, _
        ByVal intLocation As Integer, _
        ByVal tntLinked As Short, _
        ByVal bitProduct As Boolean, _
        ByVal bitFinishedGood As Boolean, _
        ByVal dblFromPriceRange As Double, _
        ByVal dblToPriceRange As Double, _
        ByVal intPageIndex As Integer, _
        ByVal intPageSize As Integer, _
        ByRef intRowCount As Integer, _
        ByVal strCodeList As String, _
        ByVal strurctWineFilter As structSearchWineFilter, _
        Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault, _
        Optional ByVal blnIsCorporateUser As Boolean = False) As Object

        Dim sqlCmd As SqlCommand = New SqlCommand
        With sqlCmd
            .Connection = New SqlConnection(L_strCnn)
            .CommandType = CommandType.StoredProcedure
            .CommandText = "PROD_GetSearchResult"
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
            .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
            .Parameters.Add("@nvcName", SqlDbType.NVarChar, 150).Value = strName
            .Parameters.Add("@intNameOption", SqlDbType.Int).Value = shrtNameOption
            .Parameters.Add("@numNumber", SqlDbType.NVarChar, 20).Value = strNumber
            .Parameters.Add("@intNumberOption", SqlDbType.Int).Value = shrtNumberOption
            .Parameters.Add("@intCategory", SqlDbType.Int).Value = intCategory
            .Parameters.Add("@intSupplier", SqlDbType.Int).Value = intSupplier
            .Parameters.Add("@intLocation", SqlDbType.Int).Value = intLocation
            .Parameters.Add("@tntLinked", SqlDbType.TinyInt).Value = tntLinked
            .Parameters.Add("@bitProduct", SqlDbType.Bit).Value = bitProduct
            .Parameters.Add("@bitFinishedGood", SqlDbType.Bit).Value = bitFinishedGood
            .Parameters.Add("@fltFromPriceRange", SqlDbType.Float).Value = dblFromPriceRange
            .Parameters.Add("@fltToPriceRange", SqlDbType.Float).Value = dblToPriceRange
            .Parameters.Add("@intPageIndex", SqlDbType.Int).Value = intPageIndex
            .Parameters.Add("@intPageSize", SqlDbType.Int).Value = intPageSize
            .Parameters.Add("@intRowCount", SqlDbType.Int).Value = intRowCount
            .Parameters.Add("@vchCodeList", SqlDbType.VarChar, 4000).Value = strCodeList
            .Parameters("@intRowCount").Direction = ParameterDirection.InputOutput

            'Wine Search filters - MRC 05.06.09
            .Parameters.Add("@intCountry", SqlDbType.Int).Value = strurctWineFilter.Country
            .Parameters.Add("@intRegion", SqlDbType.Int).Value = strurctWineFilter.Region
            .Parameters.Add("@intSubRegion", SqlDbType.Int).Value = strurctWineFilter.SubRegion
            .Parameters.Add("@intProducer", SqlDbType.Int).Value = strurctWineFilter.Producer
            .Parameters.Add("@intWineType", SqlDbType.Int).Value = strurctWineFilter.WineType

            'Alcohol
            Dim dblAlcoholFrom As Double = -1
            Dim dblAlcoholTo As Double = -1
            If strurctWineFilter.Alcohol <> "" Then
                Dim arrAlcohol() As String = strurctWineFilter.Alcohol.Split(CChar("-"))
                If arrAlcohol.Length > 1 Then
                    If IsNumeric(arrAlcohol(0)) Then dblAlcoholFrom = CDbl(arrAlcohol(0))
                    If IsNumeric(arrAlcohol(1)) Then dblAlcoholTo = CDbl(arrAlcohol(1))
                End If
            End If
            .Parameters.Add("@fltFromAlcohol", SqlDbType.Float).Value = dblAlcoholFrom
            .Parameters.Add("@fltToAlcohol", SqlDbType.Float).Value = dblAlcoholTo

            'Vintage
            Dim intVintageFrom As Integer = 1900
            Dim intVintageTo As Integer = 1900
            If strurctWineFilter.Vintage <> "" Then
                Dim arrVintage() As String = strurctWineFilter.Vintage.Split(CChar("-"))
                If arrVintage.Length > 1 Then
                    If IsNumeric(arrVintage(0)) Then intVintageFrom = CInt(arrVintage(0))
                    If IsNumeric(arrVintage(1)) Then intVintageTo = CInt(arrVintage(1))
                End If
            End If
            .Parameters.Add("@intFromVintage", SqlDbType.Int).Value = intVintageFrom
            .Parameters.Add("@intToVintage", SqlDbType.Int).Value = intVintageTo


            'MRC 07.18.08   If Admin, display all products, regardless of codesite.
            .Parameters.Add("@bitIsAdminUser", SqlDbType.Bit).Value = blnIsCorporateUser
        End With
        Try
            If fetchType = enumEgswFetchType.UseDefault Then fetchType = L_bytFetchType
            GetList = ExecuteFetchType(fetchType, sqlCmd)
            intRowCount = CInt(sqlCmd.Parameters("@intRowCount").Value)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="intCode"></param>
    ''' <param name="fetchType"></param>
    ''' <returns></returns>
    ''' <remarks>MRC 05.06.09 - Added a structSearchWineFilter param</remarks>
    Public Function GetList2(ByVal intCode As Integer, _
        ByVal intCodeSite As Integer, _
        ByVal strName As String, _
        ByVal shrtNameOption As Integer, _
        ByVal strNumber As String, _
        ByVal shrtNumberOption As Integer, _
        ByVal intCategory As Integer, _
        ByVal intSupplier As Integer, _
        ByVal intLocation As Integer, _
        ByVal tntLinked As Short, _
        ByVal bitProduct As Boolean, _
        ByVal bitFinishedGood As Boolean, _
        ByVal dblFromPriceRange As Double, _
        ByVal dblToPriceRange As Double, _
        ByVal intPageIndex As Integer, _
        ByVal intPageSize As Integer, _
        ByRef intRowCount As Integer, _
        ByVal strCodeList As String, _
        ByVal strurctWineFilter As structSearchWineFilter, _
        Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault, _
        Optional ByVal blnIsCorporateUser As Boolean = False, _
        Optional ByVal blnIsSalesItemView As Boolean = False) As Object

        Dim sqlCmd As SqlCommand = New SqlCommand
        With sqlCmd
            .Connection = New SqlConnection(L_strCnn)
            .CommandType = CommandType.StoredProcedure
            .CommandText = "PROD_GetSearchResult2"
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
            .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
            .Parameters.Add("@nvcName", SqlDbType.NVarChar, 150).Value = strName
            .Parameters.Add("@intNameOption", SqlDbType.Int).Value = shrtNameOption
            .Parameters.Add("@numNumber", SqlDbType.NVarChar, 20).Value = strNumber
            .Parameters.Add("@intNumberOption", SqlDbType.Int).Value = shrtNumberOption
            .Parameters.Add("@intCategory", SqlDbType.Int).Value = intCategory
            .Parameters.Add("@intSupplier", SqlDbType.Int).Value = intSupplier
            .Parameters.Add("@intLocation", SqlDbType.Int).Value = intLocation
            .Parameters.Add("@tntLinked", SqlDbType.TinyInt).Value = tntLinked
            .Parameters.Add("@bitProduct", SqlDbType.Bit).Value = bitProduct
            .Parameters.Add("@bitFinishedGood", SqlDbType.Bit).Value = bitFinishedGood
            .Parameters.Add("@fltFromPriceRange", SqlDbType.Float).Value = dblFromPriceRange
            .Parameters.Add("@fltToPriceRange", SqlDbType.Float).Value = dblToPriceRange
            .Parameters.Add("@intPageIndex", SqlDbType.Int).Value = intPageIndex
            .Parameters.Add("@intPageSize", SqlDbType.Int).Value = intPageSize
            .Parameters.Add("@intRowCount", SqlDbType.Int).Value = intRowCount
            .Parameters.Add("@vchCodeList", SqlDbType.VarChar, 4000).Value = strCodeList
            .Parameters("@intRowCount").Direction = ParameterDirection.InputOutput

            'Wine Search filters - MRC 05.06.09
            .Parameters.Add("@intCountry", SqlDbType.Int).Value = strurctWineFilter.Country
            .Parameters.Add("@intRegion", SqlDbType.Int).Value = strurctWineFilter.Region
            .Parameters.Add("@intSubRegion", SqlDbType.Int).Value = strurctWineFilter.SubRegion
            .Parameters.Add("@intProducer", SqlDbType.Int).Value = strurctWineFilter.Producer
            .Parameters.Add("@intWineType", SqlDbType.Int).Value = strurctWineFilter.WineType

            'Alcohol
            Dim dblAlcoholFrom As Double = -1
            Dim dblAlcoholTo As Double = -1
            If strurctWineFilter.Alcohol <> "" Then
                Dim arrAlcohol() As String = strurctWineFilter.Alcohol.Split(CChar("-"))
                If arrAlcohol.Length > 1 Then
                    If IsNumeric(arrAlcohol(0)) Then dblAlcoholFrom = CDbl(arrAlcohol(0))
                    If IsNumeric(arrAlcohol(1)) Then dblAlcoholTo = CDbl(arrAlcohol(1))
                End If
            End If
            .Parameters.Add("@fltFromAlcohol", SqlDbType.Float).Value = dblAlcoholFrom
            .Parameters.Add("@fltToAlcohol", SqlDbType.Float).Value = dblAlcoholTo

            'Vintage
            Dim intVintageFrom As Integer = 1900
            Dim intVintageTo As Integer = 1900
            If strurctWineFilter.Vintage <> "" Then
                Dim arrVintage() As String = strurctWineFilter.Vintage.Split(CChar("-"))
                If arrVintage.Length > 1 Then
                    If IsNumeric(arrVintage(0)) Then intVintageFrom = CInt(arrVintage(0))
                    If IsNumeric(arrVintage(1)) Then intVintageTo = CInt(arrVintage(1))
                End If
            End If
            .Parameters.Add("@intFromVintage", SqlDbType.Int).Value = intVintageFrom
            .Parameters.Add("@intToVintage", SqlDbType.Int).Value = intVintageTo

            'MRC 07.18.08   If Admin, display all products, regardless of codesite.
            .Parameters.Add("@bitIsAdminUser", SqlDbType.Bit).Value = blnIsCorporateUser
            .Parameters.Add("@bitSalesItemView", SqlDbType.Bit).Value = blnIsSalesItemView
        End With
        Try
            If fetchType = enumEgswFetchType.UseDefault Then fetchType = L_bytFetchType
            GetList2 = ExecuteFetchType(fetchType, sqlCmd)
            intRowCount = CInt(sqlCmd.Parameters("@intRowCount").Value)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetDetails(ByVal intCodeProduct As Integer, Optional ByVal intCodeSite As Integer = -1, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Dim strCommandText As String = "PROD_GetDetails"

        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeProduct", intCodeProduct)
        arrParam(1) = New SqlParameter("@intCodeSite", intCodeSite)

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

    Public Function GetLocation(ByVal intCodeProduct As Integer, Optional ByVal intCodeSite As Integer = -1, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Dim strCommandText As String = "PROD_GetLocation"

        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeProduct", intCodeProduct)
        arrParam(1) = New SqlParameter("@intCodeSite", intCodeSite)

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

    'VRP 10.07.2009
    Public Function GetLocationCodeName() As Object
        Dim strCommandText As String = "PROD_GETLOCATIONCODENAME"
        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeSite", L_User.Site.Code)

        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetLinkedMerchandise(ByVal intCodeProduct As Integer, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Dim strCommandText As String = "PROD_GetLinkedMerchandise"

        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeProduct", intCodeProduct)

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

    Public Function GetSupplier(ByVal intCodeProduct As Integer, Optional ByVal intCodeSite As Integer = -1, Optional ByVal intCodeSupplier As Integer = -1, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Dim strCommandText As String = "PROD_GetSupplier"

        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeProduct", intCodeProduct)
        arrParam(1) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(2) = New SqlParameter("@intCodeSupplier", intCodeSupplier)

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

    'VRP 01.07.2009
    Public Function GetSupplierCodeName(Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Dim strCommandText As String = "PROD_GETSUPPLIERCODENAME"
        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeSite", L_User.Site.Code)

        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetWineDetails(ByVal intCodeProduct As Integer, Optional ByVal intCodeSite As Integer = -1, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Dim strCommandText As String = "PROD_GetWineDetails"

        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeProduct", intCodeProduct)
        arrParam(1) = New SqlParameter("@intCodeSite", intCodeSite)

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

    Public Function GetTranslation(ByVal intCodeProduct As Integer, Optional ByVal intCodeTrans As Integer = -1, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Dim strCommandText As String = "PROD_TranslationGetList"

        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeProduct", intCodeProduct)
        arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans)

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

    Public Function GetLinkedToListe(ByVal intCodeListe As Integer, ByVal intCodeSite As Integer, _
                                     Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault, _
                                     Optional ByVal intOption As Integer = -1) As Object

        Dim strCommandText As String = "PROD_GetLinked"
        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeListe", intCodeListe)
        arrParam(1) = New SqlParameter("@intCodesite", intCodeSite)
        arrParam(2) = New SqlParameter("@intOption", intOption) 'VRP 14.07.2008

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

    '--- VRP 16.02.2009
    Public Function GetFinishedGoodTrans(ByVal intCodeProduct As Integer) As DataTable
        Dim sqlCmd As SqlCommand = New SqlCommand
        Dim strCommandText As String = "SELECT pt.CodeProduct, pt.CodeTrans, pt.Name, lt.Composition, lt.AddInstruction " & vbCrLf
        strCommandText += "FROM EgswProductTranslation pt " & vbCrLf
        strCommandText += "LEFT OUTER JOIN EgswLabel l ON l.CodeProduct=pt.CodeProduct " & vbCrLf
        strCommandText += "LEFT OUTER JOIN EgswLabelTranslation lt ON l.ID = lt.IDLabel AND lt.Codetrans = pt.CodeTrans " & vbCrLf
        strCommandText += "WHERE pt.CodeProduct = @intCodeProduct " & vbCrLf

        Try
            With sqlCmd
                Dim arrParam(0) As SqlParameter
                arrParam(0) = New SqlParameter("@intCodeProduct", intCodeProduct)
                Return ExecuteDataset(L_strCnn, CommandType.Text, strCommandText, arrParam).Tables(0)
            End With
            Return Nothing
        Catch ex As Exception
            Throw ex
        End Try
    End Function '----

    Public Function GetCountry(Optional ByVal intCountryCode As Integer = -1) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        Dim strCommandText As String
        If intCountryCode > -1 Then
            strCommandText = "SELECT Code,Country,Area,Base,CountryCode,IDDPrefix,WineRank FROM EgswCountry WHERE Code=" & intCountryCode
        Else
            strCommandText = "SELECT Code,Country,Area,Base,CountryCode,IDDPrefix,WineRank, [RankNull]=CASE WHEN (WineRank IS NULL) THEN 1 ELSE 0 END " & vbCrLf
            strCommandText += "FROM EgswCountry " & vbCrLf
            strCommandText += "ORDER BY RankNull, WineRank, Country " & vbCrLf
        End If

        With cmd
            .Connection = cn
            .CommandText = strCommandText
            .CommandType = CommandType.Text
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetRegion(Optional ByVal intCountryCode As Integer = -1) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        Dim strCommandText As String
        If intCountryCode > -1 Then
            strCommandText = "SELECT Code,Name,Country,PickFlag1 FROM EgswRegion WHERE Country=" & intCountryCode
        Else
            strCommandText = "SELECT Code,Name,Country,PickFlag1 FROM EgswRegion"
        End If
        With cmd
            .Connection = cn
            .CommandText = strCommandText
            .CommandType = CommandType.Text
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetRegionByRegionCode(Optional ByVal intRegionCode As Integer = -1) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        Dim strCommandText As String
        If intRegionCode > -1 Then
            strCommandText = "SELECT Code,Name,Country,PickFlag1 FROM EgswRegion WHERE Code=" & intRegionCode
        Else
            strCommandText = "SELECT Code,Name,Country,PickFlag1 FROM EgswRegion"
        End If
        With cmd
            .Connection = cn
            .CommandText = strCommandText
            .CommandType = CommandType.Text
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetSubRegionBySubRegionCode(Optional ByVal intSubRegionCode As Integer = -1) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        Dim strCommandText As String
        If intSubRegionCode > -1 Then
            strCommandText = "SELECT Code,Name,Region,PickFlag1 FROM EgswSubRegion WHERE Code=" & intSubRegionCode
        Else
            strCommandText = "SELECT Code,Name,Region,PickFlag1 FROM EgswSubRegion"
        End If
        With cmd
            .Connection = cn
            .CommandText = strCommandText
            .CommandType = CommandType.Text
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetSubRegion(Optional ByVal intRegionCode As Integer = -1) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        Dim strCommandText As String
        If intRegionCode > -1 Then
            strCommandText = "SELECT Code,Name,Region,PickFlag1 FROM EgswSubRegion WHERE Region=" & intRegionCode
        Else
            strCommandText = "SELECT Code,Name,Region,PickFlag1 FROM EgswSubRegion"
        End If
        With cmd
            .Connection = cn
            .CommandText = strCommandText
            .CommandType = CommandType.Text
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetWineProducer(Optional ByVal intCode As Integer = -1) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        Dim strCommandText As String
        If intCode > -1 Then
            strCommandText = "SELECT Code,Name,PickFlag1 FROM EgswProducer WHERE Code=" & intCode
        Else
            strCommandText = "SELECT Code,Name,PickFlag1 FROM EgswProducer"
        End If
        With cmd
            .Connection = cn
            .CommandText = strCommandText
            .CommandType = CommandType.Text
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetWineType(Optional ByVal intCode As Integer = -1) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        Dim strCommandText As String
        If intCode > -1 Then
            strCommandText = "SELECT Code,Name,PickFlag1 FROM EgswWineType WHERE Code=" & intCode
        Else
            strCommandText = "SELECT Code,Name,PickFlag1 FROM EgswWineType"
        End If
        With cmd
            .Connection = cn
            .CommandText = strCommandText
            .CommandType = CommandType.Text
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetGrapeVarietalList(Optional ByVal intCode As Integer = -1) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        Dim strCommandText As String
        If intCode > -1 Then

            strCommandText = "SELECT Code,Name FROM EgswGrapeVarietalList WHERE Code="
        Else
            strCommandText = "SELECT Code,Name FROM EgswGrapeVarietalList"
        End If
        With cmd
            .Connection = cn
            .CommandText = strCommandText
            .CommandType = CommandType.Text
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetWineGrapeVarietal(Optional ByVal intCode As Integer = -1) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        Dim strCommandText As String
        If intCode > -1 Then
            strCommandText = "SELECT ID,CodeProduct,Amount,Name,CodeGrapeVarietal,Position FROM EgswWineGrapeVarietal WHERE CodeProduct=" & intCode
        Else
            strCommandText = "SELECT ID,CodeProduct,Amount,Name,CodeGrapeVarietal,Position FROM EgswWineGrapeVarietal WHERE CodeProduct=" & intCode
        End If
        With cmd
            .Connection = cn
            .CommandText = strCommandText
            .CommandType = CommandType.Text
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetWineUnit(Optional ByVal intCodeSite As Integer = 1, Optional ByVal intCodeTrans As Integer = 1) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        Dim strCommandText As String

        strCommandText = "SELECT DISTINCT " & vbCrLf
        strCommandText += "U.Code " & vbCrLf
        strCommandText += ",CASE WHEN ISNULL(UT.[Name],'') <>'' THEN UT.[Name] ELSE U.NameDisplay END AS NameDisplay " & vbCrLf
        strCommandText += ",S.Position " & vbCrLf
        strCommandText += "FROM EgswUnit U " & vbCrLf
        strCommandText += "INNER JOIN EgswSharing S ON U.Code = S.Code AND S.CodeEgswTable = 135 AND (S.CodeUserSharedTo = @intCodeSite OR S.IsGlobal=1) AND S.Status <>2 " & vbCrLf
        strCommandText += "LEFT OUTER JOIN EgswItemTranslation UT ON UT.Code=U.Code AND UT.CodeEgswTable=135 AND UT.CodeTrans=@intCodeTrans " & vbCrLf
        strCommandText += "WHERE TypeMain=200 " & vbCrLf
        strCommandText += "ORDER BY S.Position "

        Dim arrParam(1) As SqlParameter
        With cmd
            .Connection = cn
            .CommandText = strCommandText
            .CommandType = CommandType.Text
            arrParam(0) = New SqlParameter("@intCodeSite", intCodeSite)
            arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans)
        End With
        Try
            'Return ExecuteDataset(L_strCnn, CommandType.Text, strCommandText, arrParam).Tables(0)
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.Text, cmd.CommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetCategoryCodeName() As Object
        Dim strCommandText As String = "[PROD_GETCATEGORYCODENAME]"

        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeSite", L_User.Site.Code)
        arrParam(1) = New SqlParameter("@CodeTrans", L_User.CodeTrans)

        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function



#End Region

#Region " UPDATE FUNCTION "
    Public Function UpdateProduct(ByRef intCode As Integer, ByVal udtProduct As structProduct, Optional ByVal blnUpdateType As Boolean = False) As enumEgswErrorCode
        If blnUpdateType Then
            Return Update(intCode, udtProduct, 1, False, enumEgswTransactionMode.Add, False, "", Nothing)
        Else
            Return Update(intCode, udtProduct, 0, False, enumEgswTransactionMode.Add, False, "", Nothing)
        End If
    End Function

    Public Function UpdateFinishedGood(ByRef intCode As Integer, ByVal udtProduct As structProduct, ByVal blnAutoCreate As Boolean) As enumEgswErrorCode
        Return Update(intCode, udtProduct, 0, blnAutoCreate, enumEgswTransactionMode.Add, False, "", Nothing)
    End Function

    Public Function UpdateCountry(ByVal intCodeUser As Integer, ByVal intCodeSite As Integer, _
            ByVal intUpdateMode As Int16, ByVal intCode As Integer, Optional ByVal strCountry As String = Nothing, _
            Optional ByVal strArea As String = Nothing, Optional ByVal blnBase As Boolean = False, _
            Optional ByVal intCountryCode As Integer = Nothing, Optional ByVal strIDDPrefix As String = Nothing, _
            Optional ByVal intWineRank As Integer = 0) As enumEgswErrorCode

        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "PROD_UpdateCountry"

                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@tntFunction", SqlDbType.TinyInt).Value = intUpdateMode
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@vchCountry", SqlDbType.VarChar).Value = strCountry
                .Parameters.Add("@vchArea", SqlDbType.VarChar).Value = strArea
                .Parameters.Add("@bitBase", SqlDbType.Bit).Value = blnBase
                .Parameters.Add("@intCountryCode", SqlDbType.Int).Value = intCountryCode
                .Parameters.Add("@vchIDDPrefix", SqlDbType.VarChar).Value = strIDDPrefix
                If intWineRank > 0 Then .Parameters.Add("@intWineRank", SqlDbType.Int).Value = intWineRank

                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                .ExecuteNonQuery()

                UpdateCountry = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            UpdateCountry = enumEgswErrorCode.GeneralError
        Finally
            sqlCmd.Connection.Close()
        End Try
    End Function

    Public Function UpdateRegion(ByVal intCodeUser As Integer, ByVal intCodeSite As Integer, _
            ByVal intUpdateMode As Int16, ByVal intCode As Integer, Optional ByVal strName As String = Nothing, _
            Optional ByVal intCountryCode As Integer = Nothing) As enumEgswErrorCode

        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "PROD_UpdateRegion"

                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@tntFunction", SqlDbType.TinyInt).Value = intUpdateMode
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@vchName", SqlDbType.VarChar).Value = strName
                .Parameters.Add("@intCountryCode", SqlDbType.Int).Value = intCountryCode

                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                .ExecuteNonQuery()

                UpdateRegion = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            UpdateRegion = enumEgswErrorCode.GeneralError
        Finally
            sqlCmd.Connection.Close()
        End Try
    End Function

    Public Function UpdateSubRegion(ByVal intCodeUser As Integer, ByVal intCodeSite As Integer, _
            ByVal intUpdateMode As Int16, ByVal intCode As Integer, Optional ByVal strName As String = Nothing, _
            Optional ByVal intRegionCode As Integer = Nothing) As enumEgswErrorCode

        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "PROD_UpdateSubRegion"

                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@tntFunction", SqlDbType.TinyInt).Value = intUpdateMode
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@vchName", SqlDbType.VarChar).Value = strName
                .Parameters.Add("@intRegionCode", SqlDbType.Int).Value = intRegionCode

                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                .ExecuteNonQuery()

                UpdateSubRegion = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            UpdateSubRegion = enumEgswErrorCode.GeneralError
        Finally
            sqlCmd.Connection.Close()
        End Try
    End Function

    Public Function UpdateWineProducer(ByVal intCodeUser As Integer, ByVal intCodeSite As Integer, _
            ByVal intUpdateMode As Int16, ByVal intCode As Integer, Optional ByVal strName As String = Nothing) As enumEgswErrorCode

        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "PROD_UpdateProducer"

                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@tntFunction", SqlDbType.TinyInt).Value = intUpdateMode
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@vchName", SqlDbType.VarChar).Value = strName

                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                .ExecuteNonQuery()

                UpdateWineProducer = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            UpdateWineProducer = enumEgswErrorCode.GeneralError
        Finally
            sqlCmd.Connection.Close()
        End Try
    End Function

    Public Function UpdateWineType(ByVal intCodeUser As Integer, ByVal intCodeSite As Integer, _
            ByVal intUpdateMode As Int16, ByVal intCode As Integer, Optional ByVal strName As String = Nothing) As enumEgswErrorCode

        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "PROD_UpdateWineType"

                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@tntFunction", SqlDbType.TinyInt).Value = intUpdateMode
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@vchName", SqlDbType.VarChar).Value = strName

                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                .ExecuteNonQuery()

                UpdateWineType = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            UpdateWineType = enumEgswErrorCode.GeneralError
        Finally
            sqlCmd.Connection.Close()
        End Try
    End Function

    Public Function UpdateGrapeVarietalList(ByVal intCodeUser As Integer, ByVal intCodeSite As Integer, _
            ByVal intUpdateMode As Int16, ByVal intCode As Integer, Optional ByVal strName As String = Nothing) As enumEgswErrorCode

        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "PROD_UpdateGrapeVarietalList"

                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@tntFunction", SqlDbType.TinyInt).Value = intUpdateMode
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@vchName", SqlDbType.VarChar).Value = strName

                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                .ExecuteNonQuery()

                UpdateGrapeVarietalList = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            UpdateGrapeVarietalList = enumEgswErrorCode.GeneralError
        Finally
            sqlCmd.Connection.Close()
        End Try
    End Function

    'if intupdatetype=0, for finsihed good, will update tables: product, productdetails, label
    'if intupdatetype=1, product only
    Private Function Update(ByRef intCode As Integer, ByVal udt As structProduct, ByVal intUpdateType As Integer, ByVal blnAutoCreate As Boolean, ByVal tranMode As enumEgswTransactionMode, ByVal blnPreviewOnly As Boolean, ByVal strCodeList As String, ByRef dtPreview As DataTable) As enumEgswErrorCode
        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()

                .CommandType = CommandType.StoredProcedure
                .CommandText = "sp_EgswProductUpdate"

                .Parameters.Add("@RETURN_VALUE", SqlDbType.Int)
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@tntStatus", SqlDbType.TinyInt).Value = udt.Status
                .Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 20).Value = udt.Number
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 150).Value = udt.Name
                .Parameters.Add("@nvcDescription", SqlDbType.NVarChar, 2000).Value = udt.Description
                .Parameters.Add("@intUnit", SqlDbType.Int).Value = udt.Unit
                .Parameters.Add("@intUnitStock", SqlDbType.Int).Value = udt.UnitStock
                .Parameters.Add("@intUnitPack", SqlDbType.Int).Value = udt.UnitPack
                .Parameters.Add("@intUnitTrans", SqlDbType.Int).Value = udt.UnitTrans
                .Parameters.Add("@fltUnitRatio", SqlDbType.Float).Value = udt.UnitRatio
                .Parameters.Add("@fltUnitRatio2", SqlDbType.Float).Value = udt.UnitRatio2
                .Parameters.Add("@fltUnitRatio3", SqlDbType.Float).Value = udt.UnitRatio3
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = udt.CodeSite
                .Parameters.Add("@intSupplier", SqlDbType.Int).Value = udt.Supplier
                .Parameters.Add("@intTax", SqlDbType.Int).Value = udt.Tax
                .Parameters.Add("@tntCurrency", SqlDbType.TinyInt).Value = udt.Currency
                .Parameters.Add("@fltPrice", SqlDbType.Float).Value = udt.Price
                .Parameters.Add("@fltAvgPrice", SqlDbType.Float).Value = udt.AvgPrice
                .Parameters.Add("@fltLastPrice", SqlDbType.Float).Value = udt.LastPrice
                .Parameters.Add("@fltPriceMin", SqlDbType.Float).Value = udt.PriceMin
                .Parameters.Add("@fltPriceMax", SqlDbType.Float).Value = udt.PriceMax
                .Parameters.Add("@intRecipeLinkCode", SqlDbType.Int).Value = udt.RecipeLinkCode
                .Parameters.Add("@fltDaysExp", SqlDbType.Float).Value = udt.DaysExp
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = udt.IsGlobal
                .Parameters.Add("@RawMaterial", SqlDbType.Bit).Value = udt.RawMaterial
                .Parameters.Add("@MultiSup", SqlDbType.Bit).Value = udt.MultiSup
                .Parameters.Add("@tntPriceUpdate", SqlDbType.TinyInt).Value = udt.PriceUpdate
                .Parameters.Add("@TransferFlag", SqlDbType.Bit).Value = udt.TransferFlag
                .Parameters.Add("@fltQtyOnHand", SqlDbType.Float).Value = udt.QtyOnHand
                .Parameters.Add("@fltQtyInventory", SqlDbType.Float).Value = udt.QtyInventory
                .Parameters.Add("@fltQtyMax", SqlDbType.Float).Value = udt.QtyMax
                .Parameters.Add("@fltQtyMin", SqlDbType.Float).Value = udt.QtyMin
                .Parameters.Add("@fltQtyOrderMin", SqlDbType.Float).Value = udt.QtyOrderMin
                .Parameters.Add("@fltQtyOrderMax", SqlDbType.Float).Value = udt.QtyOrderMax
                .Parameters.Add("@fltQtyOrderLast", SqlDbType.Float).Value = udt.QtyOrderLast
                .Parameters.Add("@fltQtyOrderDef", SqlDbType.Float).Value = udt.QtyOrderDef
                .Parameters.Add("@intStockingPlace", SqlDbType.Int).Value = udt.StockingPlace
                .Parameters.Add("@InInventory", SqlDbType.Bit).Value = udt.InInventory
                .Parameters.Add("@InCurrentInventory", SqlDbType.Bit).Value = udt.InCurrentInventory
                .Parameters.Add("@nvcBarcode", SqlDbType.NVarChar, 20).Value = udt.Barcode
                .Parameters.Add("@sntEconomat", SqlDbType.SmallInt).Value = udt.Economat
                .Parameters.Add("@fltQuantityEconomat", SqlDbType.Float).Value = udt.QuantityEconomat
                .Parameters.Add("@fltQty2Economat", SqlDbType.Float).Value = udt.Qty2Economat
                .Parameters.Add("@fltInventPrice", SqlDbType.Float).Value = udt.InventPrice
                .Parameters.Add("@ActionFlag", SqlDbType.Bit).Value = udt.ActionFlag
                .Parameters.Add("@fltQtyInOrder", SqlDbType.Float).Value = udt.QtyInOrder
                .Parameters.Add("@intLocationProdDef", SqlDbType.Int).Value = udt.LocationProdDef
                .Parameters.Add("@intLocationOutDef", SqlDbType.Int).Value = udt.LocationOutDef
                .Parameters.Add("@UseIO", SqlDbType.Bit).Value = udt.UseIO
                .Parameters.Add("@fltQtyAllocated", SqlDbType.Float).Value = udt.QtyAllocated
                .Parameters.Add("@AutoTransferOutlet", SqlDbType.Bit).Value = udt.AutoTransferOutlet
                .Parameters.Add("@ExcludeFromAutoOutput", SqlDbType.Bit).Value = udt.ExcludeFromAutoOutput
                .Parameters.Add("@intLastUnitUsed", SqlDbType.Int).Value = udt.LastUnitUsed
                .Parameters.Add("@intCodeSalesItem", SqlDbType.Int).Value = IIf(udt.CodeSalesItem = 0, DBNull.Value, udt.CodeSalesItem)
                .Parameters.Add("@nvcSupplierNumber", SqlDbType.NVarChar, 20).Value = udt.SupplierNumber
                .Parameters.Add("@nvcUnitStockBarcode", SqlDbType.NVarChar, 20).Value = udt.UnitStockBarCode
                .Parameters.Add("@nvcUnitPackBarcode", SqlDbType.NVarChar, 20).Value = udt.UnitPackBarCode
                .Parameters.Add("@nvcNote", SqlDbType.NVarChar, 4000).Value = udt.Note
                .Parameters.Add("@nvcPicture1", SqlDbType.NVarChar, 100).Value = udt.Picture1
                .Parameters.Add("@nvcPicture2", SqlDbType.NVarChar, 100).Value = udt.Picture2
                .Parameters.Add("@nvcPicture3", SqlDbType.NVarChar, 100).Value = udt.Picture3
                .Parameters.Add("@nvcComposition", SqlDbType.NVarChar, 4000).Value = udt.Composition
                .Parameters.Add("@fltConsumptionDays", SqlDbType.Float).Value = udt.ConsumptionDays
                .Parameters.Add("@nvcConsumptionText", SqlDbType.NVarChar, 1000).Value = udt.ConsumptionText
                .Parameters.Add("@dtPackingDate", SqlDbType.DateTime).Value = udt.PackingDate
                .Parameters.Add("@nvcPackingText", SqlDbType.NVarChar, 1000).Value = udt.PackingText
                .Parameters.Add("@nvcAddInstruction", SqlDbType.NVarChar, 4000).Value = udt.AddInstruction
                .Parameters.Add("@tntUpdateType", SqlDbType.TinyInt).Value = intUpdateType
                .Parameters.Add("@bitAutoCreate", SqlDbType.Bit).Value = blnAutoCreate
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = tranMode
                .Parameters.Add("@bitPreviewOnly", SqlDbType.Bit).Value = blnPreviewOnly
                .Parameters.Add("@nvcCodeList", SqlDbType.NVarChar, 4000).Value = strCodeList
                .Parameters.Add("@fltGoodsRatio", SqlDbType.Float).Value = udt.GoodsRatio
                .Parameters.Add("@intCodeCategory", SqlDbType.Int).Value = udt.Category
                .Parameters.Add("@bitIncludeIngredient", SqlDbType.Bit).Value = udt.IncludeIngredient   'MRC March 17, 2008

                'MRC April 22, 2009
                If udt.WineProduct > 0 Then
                    .Parameters.Add("@intWineProduct", SqlDbType.Int).Value = udt.WineProduct
                End If

                .Parameters("@RETURN_VALUE").Direction = ParameterDirection.ReturnValue
                .Parameters("@intCode").Direction = ParameterDirection.InputOutput

                If blnPreviewOnly Then
                    Dim sqlDa As SqlDataAdapter = New SqlDataAdapter(sqlCmd)
                    sqlDa.Fill(dtPreview)
                Else
                    .ExecuteNonQuery()
                End If

                intCode = CInt(.Parameters("@intCode").Value)
                Update = CType(.Parameters("@RETURN_VALUE").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            Update = enumEgswErrorCode.GeneralError
        Finally
            sqlCmd.Connection.Close()
        End Try
    End Function

    'MRC 05.19.09
    Public Function GetProductShared(ByVal intCodeProduct As Integer, ByVal eShareType As ShareType, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            .CommandText = "PROD_GetShare"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeProduct
            .Parameters.Add("@sntType", SqlDbType.SmallInt).Value = eShareType
        End With

        Try
            If fetchType = enumEgswFetchType.ArrayList Then
                Dim dr As SqlDataReader
                Try
                    dr = CType(ExecuteFetchType(enumEgswFetchType.DataReader, cmd), SqlDataReader)
                Catch ex As Exception
                    dr.Close()
                    Throw ex
                End Try

                Dim arr As ArrayList = New ArrayList
                While dr.Read
                    arr.Add(dr("CodeSite"))
                End While
                dr.Close()

                Return arr
            ElseIf fetchType = enumEgswFetchType.UseDefault Then
                Return ExecuteFetchType(L_bytFetchType, cmd)
            Else
                Return ExecuteFetchType(fetchType, cmd)
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    'MRC 05.19.09
    Public Function UpdateProductSharing(ByVal intCode As Integer, ByVal intCodeUser As Integer, ByVal strCodeSiteList As String, ByVal blnGlobal As Boolean, ByVal type As ShareType) As enumEgswErrorCode
        Dim arrParam(5) As SqlParameter

        arrParam(0) = New SqlParameter("@intCode", intCode)
        arrParam(1) = New SqlParameter("@intCodeUser", intCodeUser)
        arrParam(2) = New SqlParameter("@vchCodeList", strCodeSiteList)
        arrParam(3) = New SqlParameter("@IsGlobal", blnGlobal)
        arrParam(4) = New SqlParameter("@sntShareType", type)
        arrParam(5) = New SqlParameter("@retval", SqlDbType.Int)
        arrParam(5).Direction = ParameterDirection.ReturnValue

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "PROD_UpdateShare", arrParam)
            Return CType(arrParam(5).Value, enumEgswErrorCode)
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
            Throw ex
        End Try
    End Function

    'MRC 05.20.09
    Public Function RemoveProductShared(ByVal intCodeListe As Integer, ByVal eShareType As ShareType, Optional ByVal strShareType As String = "") As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            .CommandText = "PROD_DeleteShare"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@sntType", SqlDbType.SmallInt).Value = eShareType
            .Parameters.Add("@vchTypeList", SqlDbType.VarChar, 8000).Value = strShareType
            .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
        End With

        Try
            cn.Open()
            cmd.ExecuteNonQuery()
            L_ErrCode = CType(cmd.Parameters("@retval").Value, enumEgswErrorCode)
            cn.Close()
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State <> ConnectionState.Closed Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    'MRC - 05.14.09
    Public Function UpdateProductMassMutation(ByVal udtUser As structUser, _
                ByVal strCodeProductList As String, _
                ByVal blnCheckStatusOnly As Boolean, _
                ByRef dtStatus As DataTable, _
                ByVal rolelevel As enumGroupLevel, _
                ByVal fnc As UserRightsFunction, _
                ByVal strCodeSiteList As String, _
                Optional ByVal intCodeSetPrice As Integer = -1, _
                Optional ByVal dblApprovedPriceNew As Double = -1.0, _
                Optional ByVal intCodeReplace As Integer = -1, _
                Optional ByVal strGenericParam As String = "") As enumEgswErrorCode

        Dim arrParam(12) As SqlParameter
        Dim intIDMain As Integer = -1

        If strCodeProductList.Length > 5000 Then
            intIDMain = fctSaveToTempList(strCodeProductList, udtUser.Code)
        End If

        arrParam(0) = New SqlParameter("@vchCodeProductList", SqlDbType.VarChar, 8000)
        arrParam(0).Value = strCodeProductList
        arrParam(1) = New SqlParameter("@intCodeUser", udtUser.Code)
        arrParam(2) = New SqlParameter("@intCodeSetPrice", intCodeSetPrice)
        arrParam(3) = New SqlParameter("@fltApprovedPriceNew", dblApprovedPriceNew)
        arrParam(4) = New SqlParameter("@CheckStatusOnly", blnCheckStatusOnly)
        arrParam(5) = New SqlParameter("@intCodeReplace", intCodeReplace)
        arrParam(6) = New SqlParameter("@intRoleLevel", rolelevel)
        arrParam(7) = New SqlParameter("@retval", SqlDbType.Int)
        arrParam(7).Direction = ParameterDirection.ReturnValue
        arrParam(8) = New SqlParameter("@tntFunction", fnc)
        arrParam(9) = New SqlParameter("@vchCodeSiteList", SqlDbType.VarChar, 8000)
        arrParam(9).Value = strCodeSiteList
        arrParam(10) = New SqlParameter("@intCodeTrans", udtUser.CodeTrans)
        arrParam(11) = New SqlParameter("@IDMain", intIDMain)
        arrParam(12) = New SqlParameter("@vchGenericParam", strGenericParam)
        Try
            dtStatus = ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "PROD_UpdateMassMutation", arrParam, 1200).Tables(0)
            Return CType(arrParam(7).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function fctSaveToTempList(ByVal strCodelist As String, ByVal intUser As Integer) As Integer
        Dim intIDMAIN As Integer
        Dim intReturn As Integer

        strCodelist = strCodelist.Replace("(", "")
        strCodelist = strCodelist.Replace(")", "")

        Dim arrCodeList As New ArrayList(strCodelist.Split(CChar(",")))

        '----- save main -----
        intReturn = MarkedListeMain(intUser, intIDMAIN)
        intReturn = MarkedListeDetails(intIDMAIN, arrCodeList)

        Return intIDMAIN
    End Function

    'VRP 08.23.2007
    Public Function MarkedListeMain(ByVal intCodeUser As Integer, ByRef intID As Integer) As enumEgswErrorCode
        Dim sqlCn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand

        Try
            With sqlCmd
                .Connection = sqlCn
                .CommandText = "sp_EgsW_TempMarkMain"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@IDMain", SqlDbType.Int).Direction = ParameterDirection.Output
                sqlCn.Open()
                .ExecuteNonQuery()
                intID = CInt(.Parameters("@IDMain").Value)
                sqlCn.Close()
                sqlCn.Dispose()
                Return enumEgswErrorCode.OK
            End With
        Catch ex As Exception
            sqlCn.Close()
            sqlCn.Dispose()
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function MarkedListeDetails(ByVal intID As Integer, ByVal arrCodeList As ArrayList) As enumEgswErrorCode
        Dim sqlCn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand
        Dim intCodeListe As Integer
        Dim i As Integer

        Try
            With sqlCmd
                .Connection = sqlCn
                .CommandText = "sp_EgsW_TempMarkDetails"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@IDMain", SqlDbType.Int)
                .Parameters.Add("@CodeListe", SqlDbType.Int)
                sqlCn.Open()

                For i = 0 To arrCodeList.Count - 1
                    If IsNumeric(arrCodeList(i)) Then
                        intCodeListe = CInt(arrCodeList(i))
                        .Parameters("@IDMain").Value = intID
                        .Parameters("@CodeListe").Value = intCodeListe
                        .ExecuteNonQuery()
                    End If
                Next i
                sqlCn.Close()
                sqlCn.Dispose()
                Return enumEgswErrorCode.OK
            End With
        Catch ex As Exception
            sqlCn.Close()
            sqlCn.Dispose()
            Return enumEgswErrorCode.GeneralError
        End Try

    End Function


    'MRC OCT 10, 2007
    'Public Function UpdateProductMassMutation(ByVal udtUser As structUser, ByVal strFieldName As String, ByVal intCodeReplace As Integer, ByVal strTableName As String, ByVal strCodeList As String, ByRef dtStatus As DataTable) As enumEgswErrorCode
    '    Dim arrParam(6) As SqlParameter

    '    arrParam(0) = New SqlParameter("@vchFieldName", SqlDbType.VarChar)
    '    arrParam(0).Value = strFieldName

    '    arrParam(1) = New SqlParameter("@intCodeNew", SqlDbType.Int)
    '    arrParam(1).Value = intCodeReplace

    '    arrParam(2) = New SqlParameter("@vchTableBasicList", SqlDbType.VarChar)
    '    arrParam(2).Value = strTableName

    '    arrParam(3) = New SqlParameter("@intCodeUser", SqlDbType.Int)
    '    arrParam(3).Value = udtUser.Site.Code

    '    arrParam(4) = New SqlParameter("@vchCodeSiteList", SqlDbType.VarChar)
    '    arrParam(4).Value = udtUser.Site.Code

    '    arrParam(5) = New SqlParameter("@vchCodes", SqlDbType.VarChar)
    '    arrParam(5).Value = strCodeList

    '    arrParam(6) = New SqlParameter("@retval", SqlDbType.Int)
    '    arrParam(6).Direction = ParameterDirection.ReturnValue

    '    Try
    '        'dtStatus = ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "PROD_UpdateMassMutation", arrParam, 1200).Tables(0)
    '        ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "PROD_UpdateMassMutation", arrParam, 1200)
    '        Return CType(arrParam(6).Value, enumEgswErrorCode)
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Function

    Public Function UpdateProductHistory(ByVal intCodeListe As Integer, ByVal intCodeUserID As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_egswProductHistoryUpdate"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeUserID", SqlDbType.Int).Value = intCodeUserID
                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                .ExecuteNonQuery()
                cmd.Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateDetails(ByVal intCodeProduct As Integer, ByVal intCodeSite As Integer, ByVal intSupplier As Integer, _
        ByVal intTax As Integer, ByVal dblPrice As Double, ByVal dblPriceMin As Double, ByVal dblPriceMax As Double, ByVal dblAvgPrice As Double, _
        ByVal dblLastPrice As Double, ByVal intPriceUpdate As Integer, ByVal blnTransferFlag As Boolean, ByVal dblQtyOnHand As Double, ByVal dblQtyInvty As Double, _
        ByVal dblQtyMax As Double, ByVal dblQtyMin As Double, ByVal intStockPlace As Integer, ByVal blnInInvty As Boolean, ByVal blnInCurrentInvty As Boolean, _
        ByVal strBarcode As String, ByVal intEconomat As Integer, ByVal dblQtyEconomat As Double, ByVal dblQty2Economat As Double, ByVal dblInvtyPrice As Double, _
        ByVal blnActionFlag As Boolean, ByVal dblQtyInOrder As Double, ByVal intLocationProdDef As Integer, ByVal intLocationOutDef As Integer, _
        ByVal blnUseIO As Boolean, ByVal dblQtyAllocated As Double, ByVal blnAutoTransferOutlet As Boolean, ByVal blnExcludeFromAutoOutput As Boolean, _
        ByVal intLastUnitUsed As Integer, Optional ByVal blnStatus As Boolean = True) As enumEgswErrorCode

        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "sp_egswproductdetailsupdate"

                .Parameters.Add("@RETURN_VALUE", SqlDbType.Int)
                .Parameters.Add("@intCodeProduct", SqlDbType.Int).Value = intCodeProduct
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@intSupplier", SqlDbType.Int).Value = intSupplier
                .Parameters.Add("@MultiSup", SqlDbType.Bit).Value = True
                .Parameters.Add("@intTax", SqlDbType.Int).Value = intTax
                .Parameters.Add("@fltPrice", SqlDbType.Float).Value = dblPrice
                .Parameters.Add("@fltPriceMin", SqlDbType.Float).Value = dblPriceMin
                .Parameters.Add("@fltPriceMax", SqlDbType.Float).Value = dblPriceMax
                .Parameters.Add("@fltAvgPrice", SqlDbType.Float).Value = dblAvgPrice
                .Parameters.Add("@fltLastPrice", SqlDbType.Float).Value = dblLastPrice
                .Parameters.Add("@tntPriceUpdate", SqlDbType.TinyInt).Value = intPriceUpdate
                .Parameters.Add("@TransferFlag", SqlDbType.Bit).Value = blnTransferFlag
                .Parameters.Add("@fltQtyOnHand", SqlDbType.Float).Value = dblQtyOnHand
                .Parameters.Add("@fltQtyInventory", SqlDbType.Float).Value = dblQtyInvty
                .Parameters.Add("@fltQtyMax", SqlDbType.Float).Value = dblQtyMax
                .Parameters.Add("@fltQtyMin", SqlDbType.Float).Value = dblQtyMin
                .Parameters.Add("@intStockingPlace", SqlDbType.Int).Value = intStockPlace
                .Parameters.Add("@InInventory", SqlDbType.Bit).Value = blnInInvty
                .Parameters.Add("@InCurrentInventory", SqlDbType.Bit).Value = blnInCurrentInvty
                .Parameters.Add("@nvcBarcode", SqlDbType.NVarChar, 20).Value = strBarcode
                .Parameters.Add("@sntEconomat", SqlDbType.SmallInt).Value = intEconomat
                .Parameters.Add("@fltQuantityEconomat", SqlDbType.Float).Value = dblQtyEconomat
                .Parameters.Add("@fltQty2Economat", SqlDbType.Float).Value = dblQty2Economat
                .Parameters.Add("@fltInventPrice", SqlDbType.Float).Value = dblInvtyPrice
                .Parameters.Add("@ActionFlag", SqlDbType.Bit).Value = blnActionFlag
                .Parameters.Add("@fltQtyInOrder", SqlDbType.Float).Value = dblQtyInOrder
                .Parameters.Add("@intLocationProdDef", SqlDbType.Int).Value = intLocationProdDef
                .Parameters.Add("@intLocationOutDef", SqlDbType.Int).Value = intLocationOutDef
                .Parameters.Add("@UseIO", SqlDbType.Bit).Value = blnUseIO
                .Parameters.Add("@fltQtyAllocated", SqlDbType.Float).Value = dblQtyAllocated
                .Parameters.Add("@AutoTransferOutlet", SqlDbType.Bit).Value = blnAutoTransferOutlet
                .Parameters.Add("@ExcludeFromAutoOutput", SqlDbType.Bit).Value = blnExcludeFromAutoOutput
                .Parameters.Add("@intLastUnitUsed", SqlDbType.Int).Value = intLastUnitUsed
                .Parameters.Add("@bitStatus", SqlDbType.Bit).Value = 1 ''always set to TRUE because if you are able to edit, it means that it is enabled to your site.  'blnStatus

                .Parameters("@RETURN_VALUE").Direction = ParameterDirection.ReturnValue

                .ExecuteNonQuery()
                UpdateDetails = CType(.Parameters("@RETURN_VALUE").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            UpdateDetails = enumEgswErrorCode.GeneralError
        Finally
            sqlCmd.Connection.Close()
        End Try
    End Function

    Public Function UpdateDetails(ByVal intCodeProduct As Integer, ByVal dtDetails As DataTable) As enumEgswErrorCode
        Dim r As DataRow = dtDetails.Rows(0)

        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "PROD_UpdateProductDetails"
                .Parameters.Add("@RETURN_VALUE", SqlDbType.Int)
                .Parameters.Add("@intCodeProduct", SqlDbType.Int).Value = intCodeProduct 'CInt(r("CodeProduct"))
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = CInt(r("CodeSite"))
                .Parameters.Add("@intSupplier", SqlDbType.Int).Value = CInt(r("Supplier"))
                .Parameters.Add("@MultiSup", SqlDbType.Bit).Value = True
                .Parameters.Add("@intTax", SqlDbType.Int).Value = r("tax")
                .Parameters.Add("@fltPrice", SqlDbType.Float).Value = r("Price")
                .Parameters.Add("@fltPriceMin", SqlDbType.Float).Value = r("PriceMin")
                .Parameters.Add("@fltPriceMax", SqlDbType.Float).Value = r("PriceMax")
                .Parameters.Add("@fltAvgPrice", SqlDbType.Float).Value = r("AvgPrice")
                .Parameters.Add("@fltLastPrice", SqlDbType.Float).Value = r("LastPrice")
                .Parameters.Add("@tntPriceUpdate", SqlDbType.TinyInt).Value = 1
                .Parameters.Add("@TransferFlag", SqlDbType.Bit).Value = r("TransferFlag")
                .Parameters.Add("@fltQtyOnHand", SqlDbType.Float).Value = r("QtyOnHand")
                .Parameters.Add("@fltQtyInventory", SqlDbType.Float).Value = r("QtyInventory")
                .Parameters.Add("@fltQtyMax", SqlDbType.Float).Value = r("QtyMax")
                .Parameters.Add("@fltQtyMin", SqlDbType.Float).Value = r("QtyMin")
                .Parameters.Add("@intStockingPlace", SqlDbType.Int).Value = r("StockingPlace")
                .Parameters.Add("@InInventory", SqlDbType.Bit).Value = r("InInventory")
                .Parameters.Add("@InCurrentInventory", SqlDbType.Bit).Value = r("InCurrentInventory")
                .Parameters.Add("@nvcBarcode", SqlDbType.NVarChar, 20).Value = r("Barcode")
                .Parameters.Add("@sntEconomat", SqlDbType.SmallInt).Value = r("Economat")
                .Parameters.Add("@fltQuantityEconomat", SqlDbType.Float).Value = r("QuantityEconomat")
                .Parameters.Add("@fltQty2Economat", SqlDbType.Float).Value = r("Qty2Economat")
                .Parameters.Add("@fltInventPrice", SqlDbType.Float).Value = r("InventPrice")
                .Parameters.Add("@ActionFlag", SqlDbType.Bit).Value = r("ActionFlag")
                .Parameters.Add("@fltQtyInOrder", SqlDbType.Float).Value = r("QtyInOrder")
                .Parameters.Add("@intLocationProdDef", SqlDbType.Int).Value = r("LocationProdDef")
                .Parameters.Add("@intLocationOutDef", SqlDbType.Int).Value = r("LocationOutDef")
                .Parameters.Add("@UseIO", SqlDbType.Bit).Value = r("UseIO")
                .Parameters.Add("@fltQtyAllocated", SqlDbType.Float).Value = r("QtyAllocated")
                .Parameters.Add("@AutoTransferOutlet", SqlDbType.Bit).Value = r("AutoTransferOutlet")
                .Parameters.Add("@ExcludeFromAutoOutput", SqlDbType.Bit).Value = r("ExcludeFromAutoOutput")
                .Parameters.Add("@intLastUnitUsed", SqlDbType.Int).Value = r("LastUnitUsed")
                .Parameters.Add("@bitStatus", SqlDbType.Bit).Value = 1 ''always set to TRUE because if you are able to edit, it means that it is enabled to your site.  'r("Status")

                .Parameters("@RETURN_VALUE").Direction = ParameterDirection.ReturnValue

                .ExecuteNonQuery()
                UpdateDetails = CType(.Parameters("@RETURN_VALUE").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            UpdateDetails = enumEgswErrorCode.GeneralError
        Finally
            sqlCmd.Connection.Close()
        End Try
    End Function

    Public Function UpdateSupplierDetail(ByVal intCodeSite As Integer, ByVal intCodeProduct As Integer, ByVal intCodeSupplier As Integer, ByVal dblPrice As Double, ByVal strNumber As String, ByVal dblDiscount As Double, _
        ByVal bitVatFlag As Boolean, ByVal dblQtyOrderMin As Double, ByVal dblQtyOrderMax As Double, ByVal dblQtyOrderLast As Double, ByVal dblQtyOrderDef As Double, _
        ByVal strUnitStockBarcode As String, ByVal strUnitPackBarcode As String, ByVal strNote As String, _
        ByVal strPicture1 As String, ByVal strPicture2 As String, ByVal strPicture3 As String, ByVal strName As String) As enumEgswErrorCode
        Return SupplierDetails(enumEgswTransactionMode.Edit, intCodeSite, intCodeProduct, intCodeSupplier, dblPrice, strNumber, dblDiscount, bitVatFlag, _
             dblQtyOrderMin, dblQtyOrderMax, dblQtyOrderLast, dblQtyOrderDef, strUnitStockBarcode, strUnitPackBarcode, strNote, _
             strPicture1, strPicture2, strPicture3, strName)
    End Function

    Public Function SetDefaultSupplierDetail(ByVal intCodeProduct As Integer, ByVal intCodesupplier As Integer, ByVal intcodeSite As Integer) As enumEgswErrorCode
        Return SupplierDetails(enumEgswTransactionMode.SetDefault, intcodeSite, intCodeProduct, intCodesupplier, 0, "", 0, False, 0, 0, 0, 0, "", "", "", "", "", "", "")
    End Function

    Private Function SupplierDetails(ByVal tntTranMode As enumEgswTransactionMode, ByVal intCodeSite As Integer, ByVal intCodeProduct As Integer, ByVal intCodeSupplier As Integer, ByVal dblPrice As Double, ByVal strNumber As String, ByVal dblDiscount As Double, _
        ByVal bitVatFlag As Boolean, ByVal dblQtyOrderMin As Double, ByVal dblQtyOrderMax As Double, ByVal dblQtyOrderLast As Double, ByVal dblQtyOrderDef As Double, _
        ByVal strUnitStockBarcode As String, ByVal strUnitPackBarcode As String, ByVal strNote As String, _
        ByVal strPicture1 As String, ByVal strPicture2 As String, ByVal strPicture3 As String, ByVal strName As String) As enumEgswErrorCode

        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "PROD_UpdateSupplierDetails"

                .Parameters.Add("@RETURN_VALUE", SqlDbType.Int)
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@intCodeProduct", SqlDbType.Int).Value = intCodeProduct
                .Parameters.Add("@intCodeSupplier", SqlDbType.Int).Value = intCodeSupplier
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = tntTranMode
                .Parameters.Add("@fltPrice", SqlDbType.Float).Value = dblPrice
                .Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 20).Value = strNumber
                .Parameters.Add("@fltDiscount", SqlDbType.Float).Value = dblDiscount
                .Parameters.Add("@bitVatFlag", SqlDbType.Bit).Value = bitVatFlag
                .Parameters.Add("@fltQtyOrderMin", SqlDbType.Float).Value = dblQtyOrderMin
                .Parameters.Add("@fltQtyOrderMax", SqlDbType.Float).Value = dblQtyOrderMax
                .Parameters.Add("@fltQtyOrderLast", SqlDbType.Float).Value = dblQtyOrderLast
                .Parameters.Add("@fltQtyOrderDef", SqlDbType.Float).Value = dblQtyOrderDef
                .Parameters.Add("@nvcUnitStockBarcode", SqlDbType.NVarChar, 20).Value = strUnitStockBarcode
                .Parameters.Add("@nvcUnitPackBarcode", SqlDbType.NVarChar, 20).Value = strUnitPackBarcode
                .Parameters.Add("@nvcNote", SqlDbType.NVarChar, 4000).Value = strNote
                .Parameters.Add("@nvcPicture1", SqlDbType.NVarChar, 100).Value = strPicture1
                .Parameters.Add("@nvcPicture2", SqlDbType.NVarChar, 100).Value = strPicture2
                .Parameters.Add("@nvcPicture3", SqlDbType.NVarChar, 100).Value = strPicture3
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 100).Value = strName

                .Parameters("@RETURN_VALUE").Direction = ParameterDirection.ReturnValue

                .ExecuteNonQuery()
                SupplierDetails = CType(.Parameters("@RETURN_VALUE").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            SupplierDetails = enumEgswErrorCode.GeneralError
        Finally
            sqlCmd.Connection.Close()
        End Try
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="intCodeProduct"></param>
    ''' <param name="intCodeLocation"></param>
    ''' <param name="intUpdateMode">0: insert/update 1: Update from Inventory module. 2: Delete</param>
    ''' <returns></returns>
    ''' <remarks>Updated by marvin, added qtymax and qtymin fields</remarks>
    Private Function ProducLocationUpdate(ByVal intCodeProduct As Integer, ByVal intCodeLocation As Integer, ByVal intCodeSite As Integer, ByVal intUpdateMode As Integer, Optional ByVal dblQtyMax As Double = Nothing, Optional ByVal dblQtyMin As Double = Nothing) As enumEgswErrorCode
        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "STKITEM_UpdateItem"

                .Parameters.Add("@RETURN_VALUE", SqlDbType.Int)
                .Parameters.Add("@intCodeProduct", SqlDbType.Int).Value = intCodeProduct
                .Parameters.Add("@intCodeLocation", SqlDbType.Int).Value = intCodeLocation
                .Parameters.Add("@intCodeInvent", SqlDbType.Int).Value = 0
                .Parameters.Add("@fltQtyInvent", SqlDbType.Float).Value = 0
                .Parameters.Add("@dteInvent", SqlDbType.DateTime).Value = Now.Date
                .Parameters.Add("@intUpdateMode", SqlDbType.TinyInt).Value = intUpdateMode
                .Parameters.Add("@fltQtyMax", SqlDbType.Float).Value = dblQtyMax
                .Parameters.Add("@fltQtyMin", SqlDbType.Float).Value = dblQtyMin
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters("@RETURN_VALUE").Direction = ParameterDirection.ReturnValue

                .ExecuteNonQuery()
                ProducLocationUpdate = CType(.Parameters("@RETURN_VALUE").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            ProducLocationUpdate = enumEgswErrorCode.GeneralError
        Finally
            sqlCmd.Connection.Close()
        End Try
    End Function

    Private Function ProductTranslationUpdate(ByVal intCodeProduct As Integer, ByVal strName As String, ByVal strDescription As String, ByVal intCodeTrans As Integer) As enumEgswErrorCode
        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "PROD_TranslationUpdate"

                .Parameters.Add("@RETURN_VALUE", SqlDbType.Int)
                .Parameters.Add("@intCodeProduct", SqlDbType.Int).Value = intCodeProduct
                .Parameters.Add("@nvcName", SqlDbType.NVarChar).Value = strName
                .Parameters.Add("@nvcDescription", SqlDbType.NVarChar).Value = strDescription
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
                '.Parameters.Add("@intUpdateMode", SqlDbType.TinyInt).Value = intUpdateMode
                .Parameters("@RETURN_VALUE").Direction = ParameterDirection.ReturnValue

                .ExecuteNonQuery()
                ProductTranslationUpdate = CType(.Parameters("@RETURN_VALUE").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            ProductTranslationUpdate = enumEgswErrorCode.GeneralError
        Finally
            sqlCmd.Connection.Close()
        End Try
    End Function

    Private Function ProductWineDetailsUpdate(ByVal intCodeProduct As Integer _
            , ByVal intCodeSite As Integer _
            , ByVal intTranMode As Integer _
            , ByVal intCodeCountry As Integer _
            , ByVal intCodeRegion As Integer _
            , ByVal intCodeSubRegion As Integer _
            , ByVal intCodeProducer As Integer, _
            ByVal intCodeType As Integer, _
            ByVal intRate As Integer, _
            ByVal intAlcohol As Integer, _
            ByVal intVintage As Integer, _
            ByVal strComment As String, _
            ByVal intsSize As Integer, _
            ByVal intUnitSize As Integer, _
            ByVal dblPeak As Double, _
            ByVal dblMerit As Double, _
            ByVal dblHoldUntil As Double, _
            ByVal dblDrinkBy As Double, _
            ByVal dblTasteNext As Double _
            ) As enumEgswErrorCode


        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "PROD_UpdateWineDetails"

                .Parameters.Add("@intCodeProduct", SqlDbType.Int).Value = intCodeProduct
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = intTranMode
                .Parameters.Add("@intCodeCountry", SqlDbType.Int).Value = intCodeCountry
                .Parameters.Add("@intCodeRegion", SqlDbType.Int).Value = intCodeRegion
                .Parameters.Add("@intCodeSubRegion", SqlDbType.Int).Value = intCodeSubRegion
                .Parameters.Add("@intCodeProducer", SqlDbType.Int).Value = intCodeProducer
                .Parameters.Add("@intCodeType", SqlDbType.Int).Value = intCodeType
                .Parameters.Add("@intRate", SqlDbType.Int).Value = intRate
                .Parameters.Add("@intAlcohol", SqlDbType.Int).Value = intAlcohol
                .Parameters.Add("@intVintage", SqlDbType.Int).Value = intVintage
                .Parameters.Add("@nvcComment", SqlDbType.NVarChar).Value = strComment
                .Parameters.Add("@intsSize", SqlDbType.Int).Value = intsSize
                .Parameters.Add("@intUnitSize", SqlDbType.Int).Value = intUnitSize
                .Parameters.Add("@fltPeak", SqlDbType.Int).Value = dblPeak
                .Parameters.Add("@fltMerit", SqlDbType.Int).Value = dblMerit
                .Parameters.Add("@fltHoldUntil", SqlDbType.Int).Value = dblHoldUntil
                .Parameters.Add("@fltDrinkBy", SqlDbType.Int).Value = dblDrinkBy
                .Parameters.Add("@fltTasteNext", SqlDbType.Int).Value = dblTasteNext

                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                .ExecuteNonQuery()
                ProductWineDetailsUpdate = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            ProductWineDetailsUpdate = enumEgswErrorCode.GeneralError
        Finally
            sqlCmd.Connection.Close()
        End Try
    End Function

    Private Function ProducWineGrapeVarietalDelete(ByVal intCodeProduct As Integer) As enumEgswErrorCode
        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "PROD_DeleteWineGrapeVarietal"

                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue
                .Parameters.Add("@intCodeProduct", SqlDbType.Int).Value = intCodeProduct

                .ExecuteNonQuery()
                ProducWineGrapeVarietalDelete = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            ProducWineGrapeVarietalDelete = enumEgswErrorCode.GeneralError
        End Try
    End Function

    Private Function ProducWineGrapeVarietalUpdate(ByVal intCodeProduct As Integer, ByVal dtWineGrapeVarietal As DataTable) As enumEgswErrorCode
        Try
            If Not dtWineGrapeVarietal Is Nothing Then
                If dtWineGrapeVarietal.Rows.Count > 0 Then
                    Dim r As DataRow
                    For Each r In dtWineGrapeVarietal.Rows
                        Dim sqlCmd As SqlCommand = New SqlCommand
                        Try
                            With sqlCmd
                                .Connection = New SqlConnection(L_strCnn)
                                .Connection.Open()
                                .CommandType = CommandType.StoredProcedure
                                .CommandText = "PROD_UpdateWineGrapeVarietal"

                                .Parameters.Add("@retval", SqlDbType.Int)
                                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                                .Parameters.Add("@intCodeProduct", SqlDbType.Int).Value = intCodeProduct
                                .Parameters.Add("@fltAmount", SqlDbType.Float).Value = CDbl(r("Amount"))
                                .Parameters.Add("@nvcName", SqlDbType.NVarChar).Value = CStr(r("Name"))
                                .Parameters.Add("@intCodeGrapeVarietal", SqlDbType.Int).Value = CInt(r("CodeGrapeVarietal"))
                                .Parameters.Add("@intPosition", SqlDbType.Int).Value = CInt(r("Position"))

                                .ExecuteNonQuery()
                                ProducWineGrapeVarietalUpdate = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                            End With
                        Catch ex As Exception
                            ProducWineGrapeVarietalUpdate = enumEgswErrorCode.GeneralError
                        Finally
                            sqlCmd.Connection.Close()
                        End Try
                    Next
                End If
            End If
        Catch ex As Exception
            ProducWineGrapeVarietalUpdate = enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function ProducLocationDetailsUpdate(ByVal intCodeProduct As Integer, ByVal intCodeLocation As Integer, ByVal dblQtyInventory As Double, _
       ByVal blnInInventory As Boolean, ByVal intCodeInventory As Integer, ByVal dblQtyEditStock As Double, ByVal dblQtyEditPack As Double, _
       ByVal dblQtyOutput As Double, ByVal dblQtyOnHandRollBack As Double, ByVal dblPriceRollBack As Double, _
       ByVal dblQtyMax As Double, ByVal dblQtyMin As Double) As enumEgswErrorCode
        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "EgswUpdateLocationDetails"

                .Parameters.Add("@RETURN_VALUE", SqlDbType.Int)
                .Parameters.Add("@intCodeProduct", SqlDbType.Int).Value = intCodeProduct
                .Parameters.Add("@intCodeLocation", SqlDbType.Int).Value = intCodeLocation
                .Parameters.Add("@intCodeInvent", SqlDbType.Int).Value = intCodeInventory
                .Parameters.Add("@fltQtyInvent", SqlDbType.Float).Value = dblQtyInventory
                .Parameters.Add("@blnInInventory", SqlDbType.Bit).Value = blnInInventory
                .Parameters.Add("@fltQtyEditStock", SqlDbType.Float).Value = dblQtyEditStock
                .Parameters.Add("@fltQtyEditPack", SqlDbType.Float).Value = dblQtyEditPack
                .Parameters.Add("@fltQtyOutput", SqlDbType.Float).Value = dblQtyOutput
                .Parameters.Add("@fltQtyOnHandRollBack", SqlDbType.Float).Value = dblQtyOnHandRollBack
                .Parameters.Add("@fltPriceRollBack", SqlDbType.Float).Value = dblPriceRollBack
                .Parameters.Add("@fltQtyMax", SqlDbType.Float).Value = dblQtyMax
                .Parameters.Add("@fltQtyMin", SqlDbType.Float).Value = dblQtyMin
                .Parameters("@RETURN_VALUE").Direction = ParameterDirection.ReturnValue

                .ExecuteNonQuery()
                ProducLocationDetailsUpdate = CType(.Parameters("@RETURN_VALUE").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            ProducLocationDetailsUpdate = enumEgswErrorCode.GeneralError
        Finally
            sqlCmd.Connection.Close()
        End Try
    End Function

    Public Function UpdateLocation(ByVal intCodeProduct As Integer, ByVal intCodeLocation As Integer, ByVal intCodeSite As Integer, Optional ByVal dblQtyMax As Double = Nothing, Optional ByVal dblQtyMin As Double = Nothing) As enumEgswErrorCode
        Return ProducLocationUpdate(intCodeProduct, intCodeLocation, intCodeSite, 0, dblQtyMax, dblQtyMin)
    End Function

    Public Function UpdateTranslation(ByVal intCodeProduct As Integer, ByVal strName As String, ByVal strDescription As String, ByVal intCodeTrans As Integer) As enumEgswErrorCode
        Return ProductTranslationUpdate(intCodeProduct, strName, strDescription, intCodeTrans)
    End Function

    Public Function UpdateWineDetails(ByVal intCodeProduct As Integer, _
            ByVal intCodeSite As Integer, _
            ByVal intTranMode As Integer, _
            ByVal intCodeCountry As Integer, _
            ByVal intCodeRegion As Integer, _
            ByVal intCodeSubRegion As Integer, _
            ByVal intCodeProducer As Integer, _
            ByVal intCodeType As Integer, _
            ByVal intRate As Integer, _
            ByVal intAlcohol As Integer, _
            ByVal intVintage As Integer, _
            ByVal strComment As String, _
            ByVal intsSize As Integer, _
            ByVal intUnitSize As Integer, _
            ByVal dblPeak As Double, _
            ByVal dblMerit As Double, _
            ByVal dblHoldUntil As Double, _
            ByVal dblDrinkBy As Double, _
            ByVal dblTasteNext As Double _
            ) As enumEgswErrorCode

        Return ProductWineDetailsUpdate(intCodeProduct, _
            intCodeSite, _
            intTranMode, _
            intCodeCountry, _
            intCodeRegion, _
            intCodeSubRegion, _
            intCodeProducer, _
            intCodeType, _
            intRate, _
            intAlcohol, _
            intVintage, _
            strComment, _
            intsSize, _
            intUnitSize, _
            dblPeak, _
            dblMerit, _
            dblHoldUntil, _
            dblDrinkBy, _
            dblTasteNext)
    End Function

    Public Function UpdateWineGrapeVarietal(ByVal intCodeProduct As Integer, _
            ByVal dtWineGrapeVarietal As DataTable) As enumEgswErrorCode

        Return ProducWineGrapeVarietalUpdate(intCodeProduct, dtWineGrapeVarietal)
    End Function

    Public Function UpdateLinkForMerchandise(ByRef intID As Integer, _
       ByVal intCodeProduct As Integer, ByVal intCodeListe As Integer, ByVal dblFactor As Double, _
       ByVal intCodeUnitProduct As Integer, ByVal intCodeUnitListe As Integer, _
       Optional ByVal blnIsDefault As Boolean = False, _
       Optional ByVal intCodeSite As Integer = -1) As enumEgswErrorCode
        'MRC 07.18.08   -   flag for the default product of the site. 
        'MRC 07.22.08   -   site of the user who made the link.
        Dim cLinkFbRnPos As clsLinkFbRnPos = New clsLinkFbRnPos(L_strCnn)
        Return cLinkFbRnPos.UpdateLinkFbRnPOS(intID, 0, enumEgswTransactionMode.Add, intCodeProduct, intCodeListe, DBNull.Value, dblFactor, 1, intCodeUnitProduct, intCodeUnitListe, False, , blnIsDefault, intCodeSite)
    End Function

    Public Function CreateFinishedGoodLabelAndSalesItem(ByVal intCodeListe As Integer, ByVal intCodeTrans1 As Integer, ByVal intCodeTrans2 As Integer) As Boolean
        'RDTC 23.08.2007

        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim cmd As SqlCommand = New SqlCommand
        Dim ErrCode As enumEgswErrorCode

        Try
            Dim sqlCmd As SqlCommand = New SqlCommand
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandType = CommandType.StoredProcedure
                .CommandText = "sp_EgswProductLabelSalesItemUpdate"
                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@CodeTrans1", SqlDbType.Int).Value = intCodeTrans1
                .Parameters.Add("@CodeTrans2", SqlDbType.Int).Value = intCodeTrans2
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                'cn.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                'cn.Close()

                ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return (ErrCode = enumEgswErrorCode.OK)
    End Function

    '--- VRP 09.02.2009
    Public Function UpdateFinishedGoodTrans(ByVal intCode As Integer, ByVal udt As structProductTranslation) As enumEgswErrorCode

        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()

                .CommandType = CommandType.StoredProcedure
                .CommandText = "sp_egswProductTransUpdate"

                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = udt.TranMode
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = udt.CodeTrans
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 150).Value = udt.Name
                .Parameters.Add("@nvcComposition", SqlDbType.NVarChar, 4000).Value = udt.Composition
                .Parameters.Add("@nvcAddInstruction", SqlDbType.NVarChar, 4000).Value = udt.AddInstruction

                .ExecuteNonQuery()
                .Connection.Close()
                .Dispose()
            End With
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function '--- 
#End Region

#Region " DELETE FUNCTION "
    Public Function DeleteProduct(ByVal intCode As Integer, ByVal strCodeList As String, ByVal blnPreview As Boolean, ByVal dtPreview As DataTable) As enumEgswErrorCode
        Return Update(intCode, GetDefaultStructProduct, 1, False, enumEgswTransactionMode.Delete, blnPreview, strCodeList, dtPreview)
    End Function

    Public Function DeleteSupplierDetail(ByVal intCodeProduct As Integer, ByVal intCodesupplier As Integer, ByVal intcodeSite As Integer) As enumEgswErrorCode
        Return SupplierDetails(enumEgswTransactionMode.Delete, intcodeSite, intCodeProduct, intCodesupplier, 0, "", 0, False, 0, 0, 0, 0, "", "", "", "", "", "", "")
    End Function

    Public Function DeleteLocation(ByVal intCodeProduct As Integer, ByVal intCodeLocation As Integer, ByVal intCodeSite As Integer) As enumEgswErrorCode
        Return ProducLocationUpdate(intCodeProduct, intCodeLocation, intCodeSite, 2)
    End Function

    Public Function DeleteWineGrapeVarietal(ByVal intCodeProduct As Integer) As enumEgswErrorCode
        Return ProducWineGrapeVarietalDelete(intCodeProduct)
    End Function

    Public Function DeleteListeLinkForMerchandise(ByVal intCodeProduct As Integer, ByVal intCodeListe As Integer) As enumEgswErrorCode
        Dim cLinkFbRnPos As clsLinkFbRnPos = New clsLinkFbRnPos(L_strCnn)
        Return cLinkFbRnPos.UpdateLinkFbRnPOS(1, 0, enumEgswTransactionMode.Delete, intCodeProduct, intCodeListe, DBNull.Value, 1, 1, 0, 0, False)
    End Function
#End Region

    '#Region "Variable Declarations / Dependencies"
    '    Private L_ErrCode As enumEgswErrorCode

    '    Private L_intCodeProduct As Int32
    '    Private L_udtUser As structUser
    '    Private L_strCnn As String
    '    Private L_intCodeSite As Int32 = -1
    '    Private L_EFetchType As enumEgswFetchType
    '    Private L_intUnit As Int32
    '    Private L_EUnitType As UnitType
    '    Private L_dblFactor As Double
    '#End Region

    '#Region "Constructor, Desctructor and Properties"
    '    Public Sub New(ByVal udtUser As structUser, ByVal strCnn As String, _
    '        Optional ByVal intCodeProduct As Int32 = 0, _
    '        Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)

    '        Try
    '            L_udtUser = udtUser
    '            L_strCnn = strCnn
    '            'L_intCodeSite = intCodeSite
    '            L_EFetchType = bytFetchType
    '            '
    '            L_intCodeProduct = intCodeProduct

    '        Catch ex As Exception
    '            Throw New Exception("Error initializing object", ex)
    '        End Try
    '    End Sub
    '#End Region

    '#Region "Public Methods"
    '#Region "Unit Related Methods"
    '    ''' <summary>
    '    ''' Get the unit code and unit type of this product based on the unit name.
    '    ''' </summary>
    '    ''' <param name="intCodeProduct">The code of this product.</param>
    '    ''' <param name="strUnitName">The unit name.</param>
    '    ''' <param name="intCodeUnit">The code of the unit that will be returned.</param>
    '    ''' <param name="EUnitType">The type of the unit that will be returned.</param>
    '    ''' <returns></returns>
    '    ''' <remarks></remarks>
    '    Public Overloads Function GetUnitCodeAndTypeFromUnitName(ByVal intCodeProduct As Int32, ByVal strUnitName As String, ByRef intCodeUnit As Int32, ByRef EUnitType As UnitType, ByRef dblFactor As Double) As enumEgswErrorCode
    '        L_intCodeProduct = intCodeProduct
    '        Return GetUnitCodeAndTypeFromUnitName(strUnitName, intCodeUnit, EUnitType, dblFactor)
    '    End Function

    '    ''' <summary>
    '    ''' Get the unit code and unit type of this product based on the unit name.
    '    ''' </summary>
    '    ''' <param name="strUnitName">The unit name.</param>
    '    ''' <param name="intCodeUnit">The code of the unit that will be returned.</param>
    '    ''' <param name="EUnitType">The type of the unit that will be returned.</param>
    '    ''' <returns></returns>
    '    ''' <remarks></remarks>
    '    Public Overloads Function GetUnitCodeAndTypeFromUnitName(ByVal strUnitName As String, ByRef intCodeUnit As Int32, ByRef EUnitType As UnitType, ByRef dblFactor As Double) As enumEgswErrorCode
    '        Dim cmdX As New SqlCommand("PROD_GetUnitCodeAndTypeFromName")
    '        'Dim intCodeProperty As Int32 = -1

    '        'If L_udtUser.RoleLevelHighest = 0 Then 'Get ALL items
    '        '    intCodeProperty = -1
    '        'ElseIf L_udtUser.RoleLevelHighest = 1 Then 'Get ALL items for a site
    '        '    L_intCodeSite = L_udtUser.Site.Code
    '        'ElseIf L_udtUser.RoleLevelHighest = 2 Then 'Get ALL items for a property
    '        '    intCodeProperty = L_udtUser.Site.Group
    '        'End If

    '        Try
    '            With cmdX
    '                .Connection = New SqlConnection(L_strCnn)
    '                .CommandType = CommandType.StoredProcedure
    '                .Parameters.Add("retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
    '                .Parameters.Add("@intCodeProduct", SqlDbType.Int).Value = L_intCodeProduct
    '                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 500).Value = strUnitName
    '                .Parameters.Add("@intCodeUnit", SqlDbType.Int).Direction = ParameterDirection.Output
    '                .Parameters.Add("@tntType", SqlDbType.TinyInt).Direction = ParameterDirection.Output
    '                .Parameters.Add("@fltFactor", SqlDbType.Float).Direction = ParameterDirection.Output
    '                .Connection.Open()
    '                .ExecuteNonQuery()
    '                .Connection.Close()

    '                intCodeUnit = CInt(.Parameters("@intCodeUnit").Value)
    '                EUnitType = CType(.Parameters("@tntType").Value, UnitType)
    '                dblFactor = CDbl(.Parameters("@fltFactor").Value)

    '                'Assign to properties
    '                L_intUnit = intCodeUnit
    '                L_EUnitType = EUnitType
    '                L_dblFactor = dblFactor

    '                L_ErrCode = CType(.Parameters("retval").Value, enumEgswErrorCode)
    '            End With

    '        Catch ex As Exception
    '            If cmdX.Connection.State <> ConnectionState.Closed Then cmdX.Connection.Close()
    '            cmdX.Dispose()
    '            Throw New Exception(ex.Message, ex)
    '        End Try

    '        If cmdX.Connection.State <> ConnectionState.Closed Then cmdX.Connection.Close()
    '        cmdX.Dispose()
    '        Return enumEgswErrorCode.OK
    '    End Function

    '    Public Overloads Function GetAllUnitRatio(ByVal intCodeProduct As Int32, ByRef dblUnitRatio As Double, ByRef dblUnitRatio2 As Double, ByRef dblUnitRatio3 As Double) As enumEgswErrorCode
    '        L_intCodeProduct = intCodeProduct
    '        Return GetAllUnitRatio(dblUnitRatio, dblUnitRatio2, dblUnitRatio3)
    '    End Function

    '    Public Overloads Function GetAllUnitRatio(ByRef dblUnitRatio As Double, ByRef dblUnitRatio2 As Double, ByRef dblUnitRatio3 As Double) As enumEgswErrorCode
    '        Dim cmdX As New SqlCommand("PROD_GetRatio")
    '        Dim intCodeProperty As Int32 = -1

    '        If L_udtUser.RoleLevelHighest = 0 Then 'Get ALL items
    '            intCodeProperty = -1
    '        ElseIf L_udtUser.RoleLevelHighest = 1 Then 'Get ALL items for a site
    '            L_intCodeSite = L_udtUser.Site.Code
    '        ElseIf L_udtUser.RoleLevelHighest = 2 Then 'Get ALL items for a property
    '            intCodeProperty = L_udtUser.Site.Group
    '        End If

    '        Try
    '            With cmdX
    '                .Connection = New SqlConnection(L_strCnn)
    '                .CommandType = CommandType.StoredProcedure
    '                .Parameters.Add("retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
    '                .Parameters.Add("@fltUnitRatio", SqlDbType.Float).Direction = ParameterDirection.Output
    '                .Parameters.Add("@fltUnitRatio2", SqlDbType.Float).Direction = ParameterDirection.Output
    '                .Parameters.Add("@fltUnitRatio3", SqlDbType.Float).Direction = ParameterDirection.Output
    '                .Parameters.Add("@intCodeProduct", SqlDbType.Int).Value = L_intCodeProduct
    '                .Connection.Open()
    '                .ExecuteNonQuery()
    '                .Connection.Close()

    '                dblUnitRatio = CDbl(.Parameters("@fltUnitRatio").Value)
    '                dblUnitRatio2 = CDbl(.Parameters("@fltUnitRatio2").Value)
    '                dblUnitRatio3 = CDbl(.Parameters("@fltUnitRatio3").Value)
    '                L_ErrCode = CType(.Parameters("retval").Value, enumEgswErrorCode)
    '            End With

    '        Catch ex As Exception
    '            If cmdX.Connection.State <> ConnectionState.Closed Then cmdX.Connection.Close()
    '            cmdX.Dispose()
    '            Throw New Exception(ex.Message, ex)
    '        End Try

    '        If cmdX.Connection.State <> ConnectionState.Closed Then cmdX.Connection.Close()
    '        cmdX.Dispose()
    '        Return enumEgswErrorCode.OK
    '    End Function

    '    Public Function GetUnitRatioByType(ByVal intCodeProduct As Int32, ByVal UnitType As UnitType) As Double
    '        L_intCodeProduct = intCodeProduct
    '        Return GetUnitRatioByType(UnitType)
    '    End Function

    '    Public Function GetUnitRatioByType(ByVal EUnitType As UnitType) As Double
    '        Dim dblUnitRatio, dblUnitRatio2, dblUnitRatio3 As Double

    '        GetAllUnitRatio(dblUnitRatio, dblUnitRatio2, dblUnitRatio3)
    '        Select Case EUnitType
    '            Case UnitType.Basic
    '                Return dblUnitRatio
    '            Case UnitType.Stock
    '                Return 1
    '            Case UnitType.Packaging
    '                If dblUnitRatio2 = 0 Then
    '                    Return 1
    '                Else
    '                    Return (1 / dblUnitRatio2)
    '                End If
    '            Case UnitType.Transportation
    '                dblUnitRatio3 = dblUnitRatio2 * dblUnitRatio3
    '                If dblUnitRatio3 = 0 Then
    '                    Return 1
    '                Else
    '                    Return (1 / dblUnitRatio3)
    '                End If
    '        End Select
    '    End Function

    '    Public Overloads Function GetAllUnits(ByVal intCodeProduct As Int32, Optional ByVal GetMode As Byte = 0) As Object
    '        L_intCodeProduct = intCodeProduct
    '        Return GetAllUnits(GetMode)
    '    End Function

    '    Public Overloads Function GetAllUnits(Optional ByVal GetMode As Byte = 0) As Object
    '        Dim ds As New DataSet
    '        Dim da As New SqlDataAdapter
    '        Dim dt As New DataTable
    '        Dim dr As SqlDataReader = Nothing
    '        Dim cmdX As New SqlCommand("PROD_GetAllUnits")

    '        Try
    '            With cmdX
    '                .Connection = New SqlConnection(L_strCnn)
    '                .CommandType = CommandType.StoredProcedure
    '                .Parameters.Add("retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
    '                .Parameters.Add("@intCodeProduct", SqlDbType.Int).Value = L_intCodeProduct
    '                .Parameters.Add("@tntGetMode", SqlDbType.TinyInt).Value = GetMode
    '            End With

    '            If L_EFetchType = enumEgswFetchType.DataReader Then
    '                cmdX.Connection.Open()
    '                dr = cmdX.ExecuteReader(CommandBehavior.CloseConnection)

    '            ElseIf L_EFetchType = enumEgswFetchType.DataTable Then
    '                With da
    '                    .SelectCommand = cmdX
    '                    dt.BeginLoadData()
    '                    .Fill(dt)
    '                    dt.EndLoadData()
    '                End With

    '            ElseIf L_EFetchType = enumEgswFetchType.DataSet Then
    '                With da
    '                    .SelectCommand = cmdX
    '                    .Fill(ds, "UnitList")
    '                End With
    '            End If
    '        Catch ex As Exception
    '            dr = Nothing
    '            ds = Nothing
    '            dt.Dispose()
    '            If cmdX.Connection.State <> ConnectionState.Closed Then cmdX.Connection.Close()
    '            cmdX.Dispose()
    '            Throw New Exception(ex.Message, ex)
    '        End Try

    '        If cmdX.Connection.State <> ConnectionState.Closed Then cmdX.Connection.Close()
    '        cmdX.Dispose()
    '        If L_EFetchType = enumEgswFetchType.DataReader Then
    '            Return dr
    '        ElseIf L_EFetchType = enumEgswFetchType.DataTable Then
    '            Return dt
    '        ElseIf L_EFetchType = enumEgswFetchType.DataSet Then
    '            Return ds
    '        Else
    '            Return Nothing
    '        End If
    '    End Function
    '#End Region

    '#Region "Supplier Related Methods"
    '    Public Overloads Function GetAllSuppliers(ByVal intCodeProduct As Int32) As Object
    '        L_intCodeProduct = intCodeProduct
    '        Return GetAllSuppliers()
    '    End Function

    '    Public Overloads Function GetAllSuppliers() As Object
    '        Dim ds As New DataSet
    '        Dim da As New SqlDataAdapter
    '        Dim dt As New DataTable
    '        Dim dr As SqlDataReader = Nothing
    '        Dim cmdX As New SqlCommand("PROD_GetAllSuppliers")

    '        Try
    '            With cmdX
    '                .Connection = New SqlConnection(L_strCnn)
    '                .CommandType = CommandType.StoredProcedure
    '                .Parameters.Add("retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
    '                .Parameters.Add("@intCodeProduct", SqlDbType.Int).Value = L_intCodeProduct
    '            End With

    '            If L_EFetchType = enumEgswFetchType.DataReader Then
    '                cmdX.Connection.Open()
    '                dr = cmdX.ExecuteReader(CommandBehavior.CloseConnection)

    '            ElseIf L_EFetchType = enumEgswFetchType.DataTable Then
    '                With da
    '                    .SelectCommand = cmdX
    '                    dt.BeginLoadData()
    '                    .Fill(dt)
    '                    dt.EndLoadData()
    '                End With

    '            ElseIf L_EFetchType = enumEgswFetchType.DataSet Then
    '                With da
    '                    .SelectCommand = cmdX
    '                    .Fill(ds, "SupplierList")
    '                End With
    '            End If
    '        Catch ex As Exception
    '            dr = Nothing
    '            ds = Nothing
    '            dt.Dispose()
    '            If cmdX.Connection.State <> ConnectionState.Closed Then cmdX.Connection.Close()
    '            cmdX.Dispose()
    '            Throw New Exception(ex.Message, ex)
    '        End Try

    '        If cmdX.Connection.State <> ConnectionState.Closed Then cmdX.Connection.Close()
    '        cmdX.Dispose()
    '        If L_EFetchType = enumEgswFetchType.DataReader Then
    '            Return dr
    '        ElseIf L_EFetchType = enumEgswFetchType.DataTable Then
    '            Return dt
    '        ElseIf L_EFetchType = enumEgswFetchType.DataSet Then
    '            Return ds
    '        Else
    '            Return Nothing
    '        End If
    '    End Function

    '    Public Overloads Function GetSupplierPrice(ByVal intCodeSupplier As Int32, ByVal intCodeProduct As Int32) As Double
    '        L_intCodeProduct = intCodeProduct
    '        Return GetSupplierPrice(intCodeSupplier)
    '    End Function

    '    Public Overloads Function GetSupplierPrice(ByVal intCodeSupplier As Int32) As Double
    '        Dim dblPrice As Double = 0
    '        Dim cmdX As New SqlCommand("PROD_GetSupPrice")
    '        Try
    '            With cmdX
    '                .Connection = New SqlConnection(L_strCnn)
    '                .CommandType = CommandType.StoredProcedure
    '                .Parameters.Add("retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
    '                .Parameters.Add("@intCodeProduct", SqlDbType.Int).Value = L_intCodeProduct
    '                .Parameters.Add("@intCodeSupplier", SqlDbType.Int).Value = intCodeSupplier
    '                .Parameters.Add("@fltPrice", SqlDbType.Float).Direction = ParameterDirection.Output
    '                .Connection.Open()
    '                .ExecuteNonQuery()
    '                .Connection.Close()
    '                dblPrice = CDbl(.Parameters("@fltPrice").Value)
    '            End With
    '        Catch ex As Exception
    '            If cmdX.Connection.State <> ConnectionState.Closed Then cmdX.Connection.Close()
    '            cmdX.Dispose()
    '            Throw ex
    '        End Try

    '        If cmdX.Connection.State <> ConnectionState.Closed Then cmdX.Connection.Close()
    '        cmdX.Dispose()
    '        Return dblPrice
    '    End Function
    '#End Region

    '    Public Overloads Function GetQuantityToAdd(ByVal intCodeProduct As Int32, ByVal intCodeSite As Int32, ByVal EQtyOrderType As QtyOrderType) As Double
    '        L_intCodeProduct = intCodeProduct
    '        Return GetQuantityToAdd(intCodeSite, EQtyOrderType)
    '    End Function

    '    Public Overloads Function GetQuantityToAdd(ByVal intCodeSite As Int32, ByVal EQtyOrderType As QtyOrderType) As Double
    '        Dim dblQty As Double
    '        Dim cmdX As New SqlCommand("PROD_GetQtyToAdd")
    '        Dim intCodeProperty As Int32 = -1

    '        If L_udtUser.RoleLevelHighest = 0 Then 'Get ALL items
    '            intCodeProperty = -1
    '        ElseIf L_udtUser.RoleLevelHighest = 1 Then 'Get ALL items for a site
    '            L_intCodeSite = L_udtUser.Site.Code
    '        ElseIf L_udtUser.RoleLevelHighest = 2 Then 'Get ALL items for a property
    '            intCodeProperty = L_udtUser.Site.Group
    '        End If

    '        Try
    '            With cmdX
    '                .Connection = New SqlConnection(L_strCnn)
    '                .CommandType = CommandType.StoredProcedure
    '                .Parameters.Add("retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
    '                .Parameters.Add("@tntOrderType", SqlDbType.TinyInt).Value = EQtyOrderType
    '                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
    '                .Parameters.Add("@intCodeProduct", SqlDbType.Int).Value = L_intCodeProduct
    '                .Parameters.Add("@fltQty", SqlDbType.Float).Direction = ParameterDirection.Output

    '                .Connection.Open()
    '                .ExecuteNonQuery()
    '                .Connection.Close()

    '                dblQty = CDbl(.Parameters("@fltQty").Value)
    '                L_ErrCode = CType(.Parameters("retval").Value, enumEgswErrorCode)
    '            End With

    '        Catch ex As Exception
    '            If cmdX.Connection.State <> ConnectionState.Closed Then cmdX.Connection.Close()
    '            cmdX.Dispose()
    '            Throw New Exception(ex.Message, ex)
    '        End Try

    '        If cmdX.Connection.State <> ConnectionState.Closed Then cmdX.Connection.Close()
    '        cmdX.Dispose()
    '        Return dblQty
    '    End Function
    '#End Region

End Class

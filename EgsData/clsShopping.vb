Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class clsShopping

    Inherits clsDBRoutine

    'Properties
    Private L_AppType As enumAppType
    Private L_dtList As DataTable
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType
    Private L_bytFetchTypeTemp As enumEgswFetchType
    Private L_lngCode As Int32
    Private L_udtUser As structUser

    Private Overloads Function ExecuteFetchType(ByVal eFetchType As enumEgswFetchType, ByRef sqlCmd As SqlCommand) As Object
        Try
            Dim da As New SqlDataAdapter
            If eFetchType = enumEgswFetchType.DataReader Then
                sqlCmd.Connection.Open()
                Return sqlCmd.ExecuteReader(CommandBehavior.CloseConnection)

            ElseIf eFetchType = enumEgswFetchType.DataTable Then
                Dim dt As New DataTable
                With da
                    .SelectCommand = sqlCmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                Return dt

            ElseIf eFetchType = enumEgswFetchType.DataSet Then
                Dim ds As New DataSet
                With da
                    .SelectCommand = sqlCmd
                    .Fill(ds, "ItemList")
                End With
                Return ds
            End If
            Return Nothing
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''' <summary>
    ''' Returns list of shopping list ingredients of the given shopping group
    ''' And the shopping group profile.
    ''' </summary>
    ''' <param name="intCodeShoppingList"></param>
    ''' <param name="intCodeTrans"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList(ByVal intCodeShoppingList As Integer, ByVal intCodeTrans As Integer) As DataSet
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable("dtMain")
        Dim da As New SqlDataAdapter


        With cmd
            .Connection = cn
            .CommandText = "sp_EgswShoppingListGet"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intCodeShoppingList", SqlDbType.Int).Value = intCodeShoppingList
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
        End With

        Try
            Return CType(ExecuteFetchType(enumEgswFetchType.DataSet, cmd), DataSet)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''' <summary>
    ''' Returns information about the given shopping list group
    ''' </summary>
    ''' <param name="intCodeShoppingList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListDetail(ByVal intCodeShoppingList As Integer) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            .CommandText = "sp_EgswShoppingListDetailGet"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intCodeShoppingList", SqlDbType.Int).Value = intCodeShoppingList
        End With

        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''' <summary>
    ''' Returns list of saved shopping list of the given user
    ''' </summary>
    ''' <param name="intCodeUser"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList(ByVal intCodeUser As Integer) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable("ShoppingListDetailList")
        Dim da As New SqlDataAdapter

        With cmd
            .Connection = cn
            .CommandText = "sp_egswShoppingListDetailGetList"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
        End With

        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function RemoveShoppingListDetail(ByVal intCodeShoppingList As Integer) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_egswShoppingListDetailDelete"
                .CommandType = CommandType.StoredProcedure

                cn.Open()
                .Parameters.Add("@intCodeShoppingList", SqlDbType.Int).Value = intCodeShoppingList
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                .ExecuteNonQuery()

                cn.Close()
                cn.Dispose()

                Return CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With

        Catch ex As Exception
            cn.Close()
            cn.Dispose()
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function UpdateShoppingListDetail(ByRef intCode As Integer, ByVal strName As String, ByVal dtmDates As Date, ByVal strNote As String, ByVal intCodeUser As Integer) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_EgswShoppingListDetailUpdate"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeShoppingList", SqlDbType.Int).Value = intCode
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 260).Value = strName
                .Parameters.Add("@nvcNote", SqlDbType.NVarChar, 1000).Value = strNote
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@dteDate", SqlDbType.DateTime).Value = dtmDates

                .Parameters.Add("@intCodeShoppingListNew", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                cn.Open()
                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()

                intCode = CInt(.Parameters("@intCodeShoppingListNew").Value)
                Return CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            cn.Close()
            cn.Dispose()
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

	Public Function UpdateShoppingList(ByVal intCodeShoppingList As Integer, ByVal intCodeListe As Integer, ByVal dblGrossQty As Double, ByVal dblNetQty As Double, ByVal intCodeUnit As Integer, ByVal intCodeUser As Integer, ByVal intCodeSetprice As Integer, Optional ByVal dblMetricGrossQty As Double = 0, Optional ByVal dblMetricNetQty As Double = 0, Optional ByVal dblImperialGrossQty As Double = 0, Optional ByVal dblImperialNetQty As Double = 0, Optional ByVal intMetricCodeUnit As Integer = 0, Optional ByVal intImperialCodeUnit As Integer = 0) As enumEgswErrorCode
		Dim cn As New SqlConnection(L_strCnn)
		Dim cmd As New SqlCommand

		Try
			With cmd
				.Connection = cn
				.CommandText = "sp_EgswShoppingListUpdate"
				.CommandType = CommandType.StoredProcedure

				.Parameters.Add("@intCodeShoppingList", SqlDbType.Int).Value = intCodeShoppingList
				.Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
				.Parameters.Add("@fltGrossQty", SqlDbType.Float).Value = dblGrossQty
				.Parameters.Add("@fltNetQty", SqlDbType.Float).Value = dblNetQty

				'JTOC 13.11.2012 Added Metric and Imperial
				.Parameters.Add("@fltMetricGrossQty", SqlDbType.Float).Value = dblMetricGrossQty
				.Parameters.Add("@fltMetricNetQty", SqlDbType.Float).Value = dblMetricNetQty
				.Parameters.Add("@fltImperialGrossQty", SqlDbType.Float).Value = dblImperialGrossQty
				.Parameters.Add("@fltImperialNetQty", SqlDbType.Float).Value = dblImperialNetQty

				.Parameters.Add("@intCodeUnitMetric", SqlDbType.Int).Value = intMetricCodeUnit
				.Parameters.Add("@intCodeUnitImperial", SqlDbType.Int).Value = intImperialCodeUnit

				.Parameters.Add("@intCodeUnit", SqlDbType.Int).Value = intCodeUnit
				.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
				.Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetprice
				.Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

				cn.Open()
				.ExecuteNonQuery()
				cn.Close()
				cn.Dispose()

				Return CType(.Parameters("@retval").Value, enumEgswErrorCode)
			End With
		Catch ex As Exception
			cn.Close()
			cn.Dispose()
			Return enumEgswErrorCode.GeneralError
		End Try
	End Function

    Public Function RemoveShoppingListMerchandise(ByVal intCodeShoppingList As Integer, ByVal intCodeListe As Integer) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_EgswShoppingListMerchandiseDelete"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeShopping", SqlDbType.Int).Value = intCodeShoppingList
                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                cn.Open()
                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()
                Return CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            cn.Close()
            cn.Dispose()
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Sub New(ByVal udtUser As structUser, ByVal eAppType As enumAppType, ByVal strCnn As String, _
        Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)

        Try
            L_AppType = eAppType
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn
            L_udtUser = udtUser
        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try
    End Sub
    ''' <summary>
    ''' Returns list of ingredients of selected recipes/menu
    ''' </summary>
    ''' <param name="arrCodesListe"></param>
    ''' <param name="intFirstCodeSetPrice"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList2(ByVal arrCodesListe As ArrayList, ByVal intFirstCodeSetPrice As Integer, Optional ByVal dblYield As Double = 0, Optional ByVal blnGroup As Boolean = False) As DataTable

        '// Create Table to Store Ingredients 
        Dim dt As New DataTable("Ing")
        With dt.Columns
            .Add("codeliste")
            .Add("name")
            .Add("number")
            .Add("netQty")
            .Add("grossQty")
            .Add("itemUnitName")
            .Add("symbole")
            .Add("itemPrice")
            .Add("itemCost")
            .Add("itemFormat")
            .Add("priceFormat")
            .Add("itemUnitCode")
            .Add("secondcodesetprice")
            .Add("priceUnit")
        End With


        '// Get Ingredients of each recipe in array
        Dim cListe As New clsListe(enumAppType.WebApp, L_strCnn)
        Dim counter As Integer
        Dim nTotal As Integer = arrCodesListe.Count - 1
        Dim nTotalSubRecipe As Integer
        Dim row As DataRow
        Dim dr As SqlDataReader

        Dim arrCodeListeSubRecipes As New ArrayList
        '// Include Sub Recipes Ingredients
        For counter = 0 To nTotal
            dr = CType(cListe.GetListeSubRecipes(CInt(arrCodesListe(counter))), SqlDataReader)
            While dr.Read
                If Not arrCodesListe.Contains(CStr(dr.Item("code"))) _
                    AndAlso Not arrCodeListeSubRecipes.Contains(CStr(dr.Item("code"))) Then
                    arrCodeListeSubRecipes.Add(CStr(dr.Item("code")))
                End If
            End While
            dr.Close()
        Next

        nTotal = arrCodesListe.Count - 1
        nTotalSubRecipe = arrCodeListeSubRecipes.Count - 1

        Dim cUnit As New clsUnit(L_udtUser, enumAppType.WebApp, L_strCnn)
        'Dim nUnitCode As Integer
        'Dim nUnitValue As Double
        'Dim nUnitValue2 As Double
        'Dim sUnitFormat As String
        'Dim sUnitName As String
        'Dim nUnitFactor As Double
        'Dim nUnitTypeMain As Integer
        Dim bUseBestUnit As Boolean = False
        If blnGroup Then bUseBestUnit = L_udtUser.UseBestUnit

        For counter = 0 To nTotal
            dr = CType(cListe.GetIngredients(1, CInt(arrCodesListe(counter)), L_udtUser.CodeTrans, L_udtUser.Site.Code, bUseBestUnit, intFirstCodeSetPrice, dblYield), SqlDataReader)
            While dr.Read
                ' only add Ingredients
                If CType(dr.Item("itemType"), enumDataListItemType) = enumDataListItemType.Merchandise Then
                    row = dt.NewRow
                    row("codeliste") = dr.Item("itemcode")
                    row("name") = dr.Item("itemname")
                    row("number") = dr.Item("itemnumber")
                    row("secondcodesetprice") = dr.Item("secondcodesetprice")
                    row("priceUnit") = dr.Item("priceUnit")
                    'If bUseBestUnit Then
                    '    sUnitName = CStrDB(row("itemUnitName"))
                    '    sUnitFormat = CStrDB(row("itemFormat"))
                    '    nUnitCode = CInt(dr.Item("itemUnitCode"))
                    '    nUnitValue = CDbl(dr.Item("netQuantity"))
                    '    nUnitValue2 = CDbl(dr.Item("grossQuantity"))
                    '    cUnit.ConvertToBestUnit(nUnitCode, nUnitValue, nUnitValue2, sUnitFormat, sUnitName, nUnitFactor, nUnitTypeMain, nOwner, nLanguage)
                    '    row("itemUnitCode") = nUnitCode
                    '    row("netQty") = nUnitValue
                    '    row("itemUnitName") = sUnitName
                    '    row("itemFormat") = sUnitFormat
                    '    row("grossQty") = nUnitValue2
                    'Else
                    row("itemUnitCode") = dr.Item("itemUnitCode")
                    row("netQty") = dr.Item("netQuantity")
                    row("itemUnitName") = dr.Item("itemUnit")
                    row("itemFormat") = dr.Item("itemFormat")
                    row("grossQty") = dr.Item("grossQuantity")
                    '          End If


                    row("itemCost") = dr.Item("itemCost")
                    row("symbole") = dr.Item("symbole")
                    row("priceFormat") = dr.Item("priceFormat")
                    If CDbl(dr.Item("netQuantity")) = 0 Then
                        row("itemUnitName") = ""
                    End If

                    row("itemPrice") = dr.Item("itemPrice")
                    dt.Rows.Add(row)

                End If
            End While
            dr.Close()
        Next

        For counter = 0 To nTotalSubRecipe
            dr = CType(cListe.GetIngredients(1, CInt(arrCodeListeSubRecipes(counter)), L_udtUser.CodeTrans, L_udtUser.Site.Code, bUseBestUnit, intFirstCodeSetPrice), SqlDataReader)
            While dr.Read
                ' only add Ingredients
                If CType(dr.Item("itemType"), enumDataListItemType) = enumDataListItemType.Merchandise Then
                    row = dt.NewRow
                    row("codeliste") = dr.Item("itemcode")
                    row("name") = dr.Item("itemname")
                    row("number") = dr.Item("itemnumber")
                    row("secondcodesetprice") = dr.Item("secondcodesetprice")
                    row("priceUnit") = dr.Item("priceUnit")
                    'If bUseBestUnit Then
                    '    sUnitName = CStrDB(row("itemUnitName"))
                    '    sUnitFormat = CStrDB(row("itemFormat"))
                    '    nUnitCode = CInt(dr.Item("itemUnitCode"))
                    '    nUnitValue = CDbl(dr.Item("netQuantity"))
                    '    nUnitValue2 = CDbl(dr.Item("grossQuantity"))
                    '    cUnit.ConvertToBestUnit(nUnitCode, nUnitValue, nUnitValue2, sUnitFormat, sUnitName, nUnitFactor, nUnitTypeMain, nOwner, nLanguage)
                    '    row("itemUnitCode") = nUnitCode
                    '    row("netQty") = nUnitValue
                    '    row("itemUnitName") = sUnitName
                    '    row("itemFormat") = sUnitFormat
                    '    row("grossQty") = nUnitValue2
                    'Else
                    row("itemUnitCode") = dr.Item("itemUnitCode")
                    row("netQty") = dr.Item("netQuantity")
                    row("itemUnitName") = dr.Item("itemUnit")
                    row("itemFormat") = dr.Item("itemFormat")
                    row("grossQty") = dr.Item("grossQuantity")
                    '          End If


                    row("itemCost") = dr.Item("itemCost")
                    row("symbole") = dr.Item("symbole")
                    row("priceFormat") = dr.Item("priceFormat")
                    If CDbl(dr.Item("netQuantity")) = 0 Then
                        row("itemUnitName") = ""
                    End If

                    row("itemPrice") = dr.Item("itemPrice")
                    dt.Rows.Add(row)

                End If
            End While
            dr.Close()
        Next
        If blnGroup = False Then Return dt

        Dim dtMerged As New DataTable("IngMerged")
        With dtMerged.Columns
            .Add("codeliste", System.Type.GetType("System.Int32"))
            .Add("name")
            .Add("number")
            .Add("netQty")
            .Add("grossQty")
            .Add("itemUnitName")
            .Add("symbole")
            .Add("itemPrice")
            .Add("itemCost")
            .Add("itemFormat")
            .Add("priceFormat")
            .Add("itemUnitCode", System.Type.GetType("System.Int32"))
            .Add("secondcodesetprice")
            .Add("priceUnit")
        End With

        Dim strRowFilter As String
        Dim rw As DataRow
        For Each row In dt.Rows
            strRowFilter = "codeListe=" & CInt(row("codeListe")) & _
             " AND itemUnitcode=" & CInt(row("itemUnitcode")) ' & _
            '" AND priceUnit='" & CInt(row("priceUnit")) & "'"
            If dtMerged.Select(strRowFilter).Length > 0 Then
                rw = dtMerged.Select(strRowFilter)(0)
                rw("netQty") = CDblDB(row("netQty")) + CDbl(rw("netQty"))
                rw("grossQty") = CDblDB(row("grossQty")) + CDbl(rw("grossQty"))
                'rw("itemPrice") = CDblDB(row("itemPrice")) + CDbl(rw("itemPrice"))
                rw("itemCost") = CDblDB(row("itemCost")) + CDbl(rw("itemCost"))
            Else
                rw = dtMerged.NewRow
                rw("codeliste") = row("codeliste")
                rw("name") = row("name")
                rw("number") = row("number")
                rw("netQty") = row("netQty")
                rw("grossQty") = row("grossQty")
                rw("itemUnitName") = row("itemUnitName")
                rw("symbole") = row("symbole")
                rw("itemPrice") = row("itemPrice")
                rw("itemCost") = row("itemCost")
                rw("itemFormat") = row("itemFormat")
                rw("priceFormat") = row("priceFormat")
                rw("itemUnitCode") = row("itemUnitCode")
                rw("secondcodesetprice") = row("secondcodesetprice")
                rw("priceUnit") = row("priceUnit")
                dtMerged.Rows.Add(rw)
            End If
        Next

        Return dtMerged
    End Function


    'Private Sub GetRecipeIng(ByRef dt As DataTable, ByVal intCodeListe As Integer, ByVal intCodeIng As Integer, ByVal intFirstCodeSetPrice As Integer, Optional ByVal dblOrigYieldFactor As Double = 1)
    Private Sub GetRecipeIng(ByRef dt As DataTable, ByVal intCodeListe As Integer, ByVal intCodeIng As Integer, ByVal intFirstCodeSetPrice As Integer, Optional ByVal dblOrigYieldFactor As Double = 1, Optional ByVal intIngID As Integer = 0)
        Dim cListe As clsListe = New clsListe(enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataTable)

        Dim intYieldUnit As Integer = 0
        Dim dtListe As DataTable = CType(cListe.GetListeBasic(intCodeListe), DataTable)
        If dtListe.Rows.Count > 0 Then
            Dim rwListe As DataRow = dtListe.Rows(0)
            Select Case CType(rwListe("type"), enumDataListItemType)
                Case enumDataListItemType.Menu
                    Dim dtIng As DataTable = CType(cListe.GetListeBasic(intCodeIng), DataTable)
                    If dtIng.Rows.Count > 0 Then
                        Dim rwIng As DataRow = dtIng.Rows(0)
                        intYieldUnit = CInt(rwIng("yieldUnit"))
                    End If
                    dtIng.Dispose() 'DLS May312007
            End Select
        End If
        dtListe.Dispose()

        Dim dtListeIng As DataTable = CType(cListe.GetIngredientsShopping(CDbl(1), intCodeListe, L_udtUser.CodeTrans, L_udtUser.Site.Code, False, intFirstCodeSetPrice), DataTable)
        'If dtListeIng.Select("itemCode=" & intCodeIng).Length > 0 Then
        '    Dim rwListeIng As DataRow = dtListeIng.Select("itemCode=" & intCodeIng)(0)
        If dtListeIng.Select("itemCode=" & intCodeIng).Length > 0 Then
            'Dim rwListeIng As DataRow = dtListeIng.Select("itemCode=" & intCodeIng)(0)
            ''Dim rwListeIng As DataRow = dtListeIng.Select("IngID=" & intIngID)(0)
            ''Dim dblYieldFactor As Double = CDbl(rwListeIng("netquantity"))
            ''dblYieldFactor = dblYieldFactor * dblOrigYieldFactor
            Dim rwListeIng As DataRow = dtListeIng.Select("IngID=" & intIngID)(0)
            Dim dblYieldFactor As Double = CDbl(rwListeIng("netquantity"))
            Dim dblSrYieldFactor As Double = CDbl(rwListeIng("srYield"))
            Dim flgOnlySubrecipe As Integer = CInt(rwListeIng("flgOnlySubrecipe"))
            Dim dblTotalYield As Double = 0
            Dim srLevel As Integer = CInt(rwListeIng("srLevel"))
            If flgOnlySubrecipe > 0 Then
                dblYieldFactor = dblYieldFactor / dblSrYieldFactor
            End If
            dblYieldFactor = dblYieldFactor * dblOrigYieldFactor
            If intYieldUnit > 0 AndAlso intYieldUnit = CInt(rwListeIng("itemunitcode")) Then
                'dblYieldFactor = 1 do nothing since same yield unit, meaning used as a recipe
            Else
                cListe.GetFactorSubRecipeIngredient(dblYieldFactor, CInt(rwListeIng("itemcode")), CInt(rwListeIng("itemunitcode")))
            End If

            Dim dtIng As DataTable = CType(cListe.GetIngredientsShopping(CDbl(1), CInt(rwListeIng("itemcode")), L_udtUser.CodeTrans, L_udtUser.Site.Code, L_udtUser.UseBestUnit, intFirstCodeSetPrice, dblYieldFactor), DataTable)
            Dim rwIng As DataRow

            Dim row As DataRow
            For Each rwIng In dtIng.Rows
                If CType(rwIng("itemtype"), enumDataListType) = enumDataListType.Merchandise Then
                    row = dt.NewRow
                    row("codeliste") = rwIng.Item("itemcode")
                    row("name") = rwIng.Item("itemname")
                    row("number") = rwIng.Item("itemnumber")
                    row("secondcodesetprice") = rwIng.Item("secondcodesetprice")
                    row("priceUnit") = rwIng.Item("priceUnit")
                    row("itemUnitCode") = rwIng.Item("itemUnitCode")
                    row("netQty") = rwIng.Item("netQuantity")
                    row("itemUnitName") = rwIng.Item("itemUnit")
                    row("itemFormat") = rwIng.Item("itemFormat")
                    row("grossQty") = rwIng.Item("grossQuantity")
                    row("itemCost") = rwIng.Item("itemCost")
                    row("symbole") = rwIng.Item("symbole")
                    row("priceFormat") = rwIng.Item("priceFormat")
                    row("MetricNetQty") = rwIng.Item("MetricNetQuantity") 'JTOC 15.02.2013
                    row("MetricGrossQty") = rwIng.Item("MetricGrossQuantity") 'JTOC 15.02.2013
                    row("ImperialNetQty") = rwIng.Item("ImperialNetQuantity") 'JTOC 15.02.2013
                    row("ImperialGrossQty") = rwIng.Item("ImperialGrossQuantity") 'JTOC 15.02.2013
                    row("MetricitemUnitCode") = rwIng.Item("MetricitemUnitCode") 'JTOC 13.11.2012
                    row("ImperialitemUnitCode") = rwIng.Item("ImperialitemUnitCode") 'JTOC 13.11.2012
                    row("MetricitemUnitName") = rwIng.Item("MetricitemUnit") 'JTOC 13.11.2012
                    row("ImperialitemUnitName") = rwIng.Item("ImperialitemUnit") 'JTOC 13.11.2012
                    row("itemCostImperial") = rwIng.Item("itemCostImperial") 'JTOC 06.12.2012
                    If CDbl(rwIng.Item("netQuantity")) = 0 Then row("itemUnitName") = ""
                    row("itemPrice") = rwIng.Item("itemPrice")
                    row("ItemPriceUnitCode") = rwIng.Item("ItemPriceUnitCode")
                    row("UnitFactor") = rwIng.Item("itemunitcodefactor")
                    row("IsConverted") = rwIng.Item("IsConverted")
                    dt.Rows.Add(row)
                    If dt.Rows.Count > 2000 Then Exit Sub
                ElseIf CType(rwIng("itemtype"), enumDataListType) = enumDataListType.Recipe Then
                    'GetRecipeIng(dt, CInt(rwListeIng("itemcode")), CInt(rwIng.Item("itemcode")), intFirstCodeSetPrice, dblYieldFactor)
                    GetRecipeIng(dt, CInt(rwListeIng("itemcode")), CInt(rwIng.Item("itemcode")), intFirstCodeSetPrice, dblYieldFactor, CDbl(rwIng.Item("IngID")))
                End If
            Next
        End If
    End Sub

    'MRC 08.24.09
    'Private Sub GetListIngComputedByYield(ByRef dt As DataTable, ByVal intCodeListe As Integer, ByVal intCodeIng As Integer, ByVal intFirstCodeSetPrice As Integer, Optional ByVal dblNewYieldFactor As Double = 1)
    Private Sub GetListIngComputedByYield(ByRef dt As DataTable, ByVal intCodeListe As Integer, ByVal intIngID As Integer, ByVal intCodeIng As Integer, ByVal intFirstCodeSetPrice As Integer, Optional ByVal dblNewYieldFactor As Double = 1)
        Dim cListe As clsListe = New clsListe(enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataTable)

        Dim intYieldUnit As Integer = 0, dblOriginalYieldFactor As Double = 0
        Dim dtListe As DataTable = CType(cListe.GetListeBasic(intCodeListe), DataTable)
        If dtListe.Rows.Count > 0 Then
            Dim rwListe As DataRow = dtListe.Rows(0)
            Select Case CType(rwListe("type"), enumDataListItemType)
                Case enumDataListItemType.Menu
                    Dim dtIng As DataTable = CType(cListe.GetListeBasic(intCodeIng), DataTable)
                    If dtIng.Rows.Count > 0 Then
                        Dim rwIng As DataRow = dtIng.Rows(0)
                        intYieldUnit = CInt(rwIng("yieldUnit"))
                    End If
                    dtIng.Dispose() 'DLS May312007
                    dblOriginalYieldFactor = CDblDB(rwListe("YIELD"))
                Case enumDataListItemType.Recipe                    'MRC Used for resizing yields when calculate button is pressed on shopping list.
                    dblOriginalYieldFactor = CDblDB(rwListe("YIELD"))

            End Select
        End If
        dtListe.Dispose()

        Dim dtListeIng As DataTable = CType(cListe.GetIngredientsShopping(CDbl(1), intCodeListe, L_udtUser.CodeTrans, L_udtUser.Site.Code, False, intFirstCodeSetPrice), DataTable)
        'If dtListeIng.Select("itemCode=" & intCodeIng).Length > 0 Then
        If dtListeIng.Select("IngID=" & intIngID).Length > 0 Then
            Dim rwListeIng As DataRow = dtListeIng.Select("IngID=" & intIngID)(0)
            Dim dblYieldFactor As Double = CDbl(rwListeIng("netquantity"))
            Dim dblSrYieldFactor As Double = CDbl(rwListeIng("srYield"))
            Dim flgOnlySubrecipe As Integer = CInt(rwListeIng("flgOnlySubrecipe"))
            Dim dblTotalYield As Double = 0
            Dim srLevel As Integer = CInt(rwListeIng("srLevel"))
            If srLevel = 0 And flgOnlySubrecipe > 0 Then
                dblYieldFactor = dblYieldFactor / dblSrYieldFactor
            End If

            dblYieldFactor = (dblYieldFactor * dblNewYieldFactor) / dblOriginalYieldFactor
            If intYieldUnit > 0 AndAlso intYieldUnit = CInt(rwListeIng("itemunitcode")) Then
                'dblYieldFactor = 1 do nothing since same yield unit, meaning used as a recipe
            Else
                cListe.GetFactorSubRecipeIngredient(dblYieldFactor, CInt(rwListeIng("itemcode")), CInt(rwListeIng("itemunitcode")))
            End If

            Dim dtIng As DataTable = CType(cListe.GetIngredientsShopping(CDbl(1), CInt(rwListeIng("itemcode")), L_udtUser.CodeTrans, L_udtUser.Site.Code, L_udtUser.UseBestUnit, intFirstCodeSetPrice, dblYieldFactor), DataTable)
            Dim rwIng As DataRow

            Dim row As DataRow
            For Each rwIng In dtIng.Rows
                If CType(rwIng("itemtype"), enumDataListType) = enumDataListType.Merchandise Then
                    row = dt.NewRow
                    row("codeliste") = rwIng.Item("itemcode")
                    row("name") = rwIng.Item("itemname")
                    row("number") = rwIng.Item("itemnumber")
                    row("secondcodesetprice") = rwIng.Item("secondcodesetprice")
                    row("priceUnit") = rwIng.Item("priceUnit")
                    row("itemUnitCode") = rwIng.Item("itemUnitCode")
                    row("netQty") = rwIng.Item("netQuantity")
                    row("itemUnitName") = rwIng.Item("itemUnit")
                    row("itemFormat") = rwIng.Item("itemFormat")
                    row("grossQty") = rwIng.Item("grossQuantity")
                    row("itemCost") = rwIng.Item("itemCost")
                    row("symbole") = rwIng.Item("symbole")
                    row("priceFormat") = rwIng.Item("priceFormat")

                    'JTOC 11.03.2013 Added Metric And Imperial Values
                    '---------------------------------------------------
                    row("MetricNetQty") = rwIng.Item("MetricNetQuantity")
                    row("MetricGrossQty") = rwIng.Item("MetricGrossQuantity")
                    row("ImperialNetQty") = rwIng.Item("ImperialNetQuantity")
                    row("ImperialGrossQty") = rwIng.Item("ImperialGrossQuantity")

                    row("MetricItemUnitCode") = rwIng.Item("MetricItemUnitCode")
                    row("ImperialItemUnitCode") = rwIng.Item("ImperialItemUnitCode")
                    row("MetricItemUnitName") = rwIng.Item("MetricItemUnit")
                    row("ImperialItemUnitName") = rwIng.Item("ImperialItemUnit")

                    row("ItemCostImperial") = rwIng.Item("ItemCostImperial")
                    '-----------------------------------------------------

                    If CDbl(rwIng.Item("netQuantity")) = 0 Then row("itemUnitName") = ""
                    row("itemPrice") = rwIng.Item("itemPrice")
                    row("ItemPriceUnitCode") = rwIng.Item("ItemPriceUnitCode")
                    row("UnitFactor") = rwIng.Item("itemunitcodefactor")
                    row("IsConverted") = rwIng.Item("IsConverted")
                    dt.Rows.Add(row)
                    If dt.Rows.Count > 2000 Then Exit Sub
                ElseIf CType(rwIng("itemtype"), enumDataListType) = enumDataListType.Recipe Then
                    'GetRecipeIng(dt, CInt(rwListeIng("itemcode")), CInt(rwIng.Item("itemcode")), intFirstCodeSetPrice, dblOrigYieldFactor)
                    'GetRecipeIng(dt, CInt(rwListeIng("itemcode")), CInt(rwIng.Item("itemcode")), intFirstCodeSetPrice, dblYieldFactor)
                    GetRecipeIng(dt, CInt(rwListeIng("itemcode")), CInt(rwIng.Item("itemcode")), intFirstCodeSetPrice, dblYieldFactor, CInt(rwIng.Item("IngID")))
                End If
            Next
        End If
    End Sub

    Public Function GetList3(ByVal arrCodesListe As ArrayList, ByVal intFirstCodeSetPrice As Integer, Optional ByVal dblYield As Double = 0, Optional ByVal blnGroup As Boolean = False) As DataTable
        '// Create Table to Store Ingredients 
        Dim dt As New DataTable("Ing")
        With dt.Columns
            .Add("codeliste")
            .Add("name")
            .Add("number")
            .Add("netQty")
            .Add("grossQty")
            .Add("itemUnitName")
            .Add("symbole")
            .Add("itemPrice")
            .Add("itemCost")
            .Add("itemFormat")
            .Add("priceFormat")
            .Add("itemUnitCode")
            .Add("secondcodesetprice")
            .Add("priceUnit")
            .Add("ItemPriceUnitCode")
        End With

        '// Get Ingredients of each recipe in array
        Dim cListe As New clsListe(enumAppType.WebApp, L_strCnn)
        Dim dr As SqlDataReader
        Dim row As DataRow
        Dim bUseBestUnit As Boolean = False
        'If Not blnGroup Then bUseBestUnit = L_udtUser.UseBestUnit

        For i As Integer = 0 To arrCodesListe.Count - 1
            dr = CType(cListe.GetIngredients(1, CInt(arrCodesListe(i)), L_udtUser.CodeTrans, L_udtUser.Site.Code, bUseBestUnit, intFirstCodeSetPrice, dblYield), SqlDataReader)
            While dr.Read
                ' only add Ingredients
                If CType(dr.Item("itemType"), enumDataListItemType) = enumDataListItemType.Merchandise Then
                    row = dt.NewRow
                    row("codeliste") = dr.Item("itemcode")
                    row("name") = dr.Item("itemname")
                    row("number") = dr.Item("itemnumber")
                    row("secondcodesetprice") = dr.Item("secondcodesetprice")
                    row("priceUnit") = dr.Item("priceUnit")
                    row("itemUnitCode") = dr.Item("itemUnitCode")
                    row("netQty") = dr.Item("netQuantity")
                    row("itemUnitName") = dr.Item("itemUnit")
                    row("itemFormat") = dr.Item("itemFormat")
                    row("grossQty") = dr.Item("grossQuantity")
                    row("itemCost") = dr.Item("itemCost")
                    row("symbole") = dr.Item("symbole")
                    row("priceFormat") = dr.Item("priceFormat")
                    If CDbl(dr.Item("netQuantity")) = 0 Then row("itemUnitName") = ""
                    row("itemPrice") = dr.Item("itemPrice")
                    row("ItemPriceUnitCode") = dr.Item("ItemPriceUnitCode")
                    dt.Rows.Add(row)
                ElseIf CType(dr.Item("itemType"), enumDataListItemType) = enumDataListItemType.Recipe Then
                    GetRecipeIng(dt, CInt(arrCodesListe(i)), CInt(dr.Item("itemcode")), intFirstCodeSetPrice)
                End If
            End While
            dr.Close()
        Next


        Dim cUnit As clsUnit = New clsUnit(L_udtUser, L_AppType, L_strCnn)
        Dim rwUnitPrice, rwUnitIng As DataRow
        Dim rw As DataRow
        For Each rw In dt.Rows
            If CInt(rw("ItemPriceUnitCode")) <> CInt(rw("itemUnitCode")) Then
                rwUnitPrice = cUnit.GetOne(CInt(rw("ItemPriceUnitCode")))
                rwUnitIng = cUnit.GetOne(CInt(rw("itemUnitCode")))

                rw("itemUnitName") = rw("priceUnit")
                rw("itemUnitCode") = rw("ItemPriceUnitCode")
                rw("itemFormat") = rwUnitPrice("format")
                rw("grossQty") = CDblDB(rw("grossQty")) / CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIng("factor"))
                rw("netQty") = CDblDB(rw("netQty")) / CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIng("factor"))
            End If
        Next

        'If blnGroup = False Then Return dt
        Dim dtMerged As New DataTable("IngMerged")
        With dtMerged.Columns
            .Add("codeliste", System.Type.GetType("System.Int32"))
            .Add("name")
            .Add("number")
            .Add("netQty")
            .Add("grossQty")
            .Add("itemUnitName")
            .Add("symbole")
            .Add("itemPrice")
            .Add("itemCost")
            .Add("itemFormat")
            .Add("priceFormat")
            .Add("itemUnitCode", System.Type.GetType("System.Int32"))
            .Add("secondcodesetprice")
            .Add("priceUnit")
        End With

        Dim strRowFilter As String
        For Each row In dt.Rows
            strRowFilter = "codeListe=" & CInt(row("codeListe")) & _
             " AND itemUnitcode=" & CInt(row("itemUnitcode")) ' & _
            '" AND priceUnit='" & CInt(row("priceUnit")) & "'"
            If dtMerged.Select(strRowFilter).Length > 0 Then
                rw = dtMerged.Select(strRowFilter)(0)
                rw("netQty") = CDblDB(row("netQty")) + CDbl(rw("netQty"))
                rw("grossQty") = CDblDB(row("grossQty")) + CDbl(rw("grossQty"))
                'rw("itemPrice") = CDblDB(row("itemPrice")) + CDbl(rw("itemPrice"))
                rw("itemCost") = CDblDB(row("itemCost")) + CDbl(rw("itemCost"))
            Else
                rw = dtMerged.NewRow
                rw("codeliste") = row("codeliste")
                rw("name") = row("name")
                rw("number") = row("number")
                rw("netQty") = row("netQty")
                rw("grossQty") = row("grossQty")
                rw("itemUnitName") = row("itemUnitName")
                rw("symbole") = row("symbole")
                rw("itemPrice") = row("itemPrice")
                rw("itemCost") = row("itemCost")
                rw("itemFormat") = row("itemFormat")
                rw("priceFormat") = row("priceFormat")
                rw("itemUnitCode") = row("itemUnitCode")
                rw("secondcodesetprice") = row("secondcodesetprice")
                rw("priceUnit") = row("priceUnit")
                dtMerged.Rows.Add(rw)
            End If
        Next
        Return dtMerged
    End Function

    'MRC 08.24.09
    Public Function GetListYield(ByVal arrCodesListe As ArrayList, Optional ByVal intCodeSetPrice As Integer = 0) As DataTable
        Dim dt As New DataTable("IngYield")
        With dt.Columns
            .Add("codeliste")
            .Add("name")
            .Add("yield")
            .Add("yieldunit")
            .Add("portionunit")
            .Add("percentage")
            .Add("computedyield")
            .Add("type")
        End With
        Try
            Dim cListe As New clsListe(enumAppType.WebApp, L_strCnn)
            Dim dr As SqlDataReader
            Dim row As DataRow
            For i As Integer = 0 To arrCodesListe.Count - 1
                'AGL Merging 2012.09.04
                dr = CType(cListe.GetListeList(L_udtUser.CodeTrans, -1, CInt(arrCodesListe(i)), False, intCodeSetPrice), SqlDataReader) 'JTOC 22.08.2012 Added False to exclude setprice
                While dr.Read
                    row = dt.NewRow
                    row("codeliste") = dr.Item("code")
                    row("name") = dr.Item("name")
                    row("yield") = dr.Item("yield")
                    row("yieldunit") = dr.Item("yieldunit")
                    row("portionunit") = dr.Item("yieldname")
                    row("percentage") = dr.Item("percent")
                    row("computedyield") = (CDblDB(dr.Item("yield")) * CDbl(dr.Item("percent"))) / 100
                    row("type") = dr.Item("type")
                    dt.Rows.Add(row)
                End While
                dr.Close()
            Next
        Catch ex As Exception

        End Try
        Return dt
    End Function


    Public Function GetList(ByVal arrCodesListe As ArrayList, ByVal intFirstCodeSetPrice As Integer, Optional ByVal dblYield As Double = -1, Optional ByVal blnGroup As Boolean = False) As DataTable
        '// Create Table to Store Ingredients 
        Dim dt As New DataTable("Ing")
        With dt.Columns
            .Add("codeliste")
            .Add("name")
            .Add("number")
            .Add("netQty")
            .Add("grossQty")
            .Add("itemUnitName")
            .Add("symbole")
            .Add("itemPrice")
            .Add("itemCost")
            .Add("itemFormat")
            .Add("priceFormat")
            .Add("itemUnitCode")
            .Add("secondcodesetprice")
            .Add("priceUnit")
            .Add("ItemPriceUnitCode")
        End With

        '// Get Ingredients of each recipe in array
        Dim cListe As New clsListe(enumAppType.WebApp, L_strCnn)
        Dim dr As SqlDataReader
        Dim row As DataRow
        Dim bUseBestUnit As Boolean = False
        'If Not blnGroup Then bUseBestUnit = L_udtUser.UseBestUnit

        For i As Integer = 0 To arrCodesListe.Count - 1
            dr = CType(cListe.GetIngredientsShopping(1, CInt(arrCodesListe(i)), L_udtUser.CodeTrans, L_udtUser.Site.Code, bUseBestUnit, intFirstCodeSetPrice, dblYield), SqlDataReader)
            While dr.Read
                ' only add Ingredients
                If CType(dr.Item("itemType"), enumDataListItemType) = enumDataListItemType.Merchandise Then
                    row = dt.NewRow
                    row("codeliste") = dr.Item("itemcode")
                    row("name") = dr.Item("itemname")
                    row("number") = dr.Item("itemnumber")
                    row("secondcodesetprice") = dr.Item("secondcodesetprice")
                    row("priceUnit") = dr.Item("priceUnit")
                    row("itemUnitCode") = dr.Item("itemUnitCode")
                    row("netQty") = dr.Item("netQuantity")
                    row("itemUnitName") = dr.Item("itemUnit")
                    row("itemFormat") = dr.Item("itemFormat")
                    row("grossQty") = dr.Item("grossQuantity")
                    row("itemCost") = dr.Item("itemCost")
                    row("symbole") = dr.Item("symbole")
                    row("priceFormat") = dr.Item("priceFormat")
                    If CDbl(dr.Item("netQuantity")) = 0 Then row("itemUnitName") = ""
                    row("itemPrice") = dr.Item("itemPrice")
                    row("ItemPriceUnitCode") = dr.Item("ItemPriceUnitCode")
                    dt.Rows.Add(row)
                ElseIf CType(dr.Item("itemType"), enumDataListItemType) = enumDataListItemType.Recipe Then
                    GetRecipeIng(dt, CInt(arrCodesListe(i)), CInt(dr.Item("itemcode")), intFirstCodeSetPrice)
                End If
            End While
            dr.Close()
        Next

        Dim cUnit As clsUnit = New clsUnit(L_udtUser, L_AppType, L_strCnn)
        Dim rwUnitPrice, rwUnitIng As DataRow
        Dim dtListeSetPrice As DataTable
        Dim rwListeSetPrice1, rwListeSetPrice As DataRow
        Dim rw As DataRow
        For Each rw In dt.Rows
            'convert accdg to price unit
            If IsDBNull(rw("ItemPriceUnitCode")) Then
                rwUnitPrice = cUnit.GetOne(CInt(rw("itemUnitCode")))
                rwUnitIng = cUnit.GetOne(CInt(rw("itemUnitCode")))

                'rw("itemUnitName") = rw("itemUnit")
                'rw("itemUnitCode") = CInt(rw("itemUnitCode"))
                'rw("itemFormat") = rwUnitPrice("format")
                rw("grossQty") = 0 'CDblDB(rw("grossQty")) '/ CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIng("factor"))
                rw("netQty") = 0 'CDblDB(rw("netQty")) '/ CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIng("factor"))
            Else
                If CInt(rw("ItemPriceUnitCode")) <> CInt(rw("itemUnitCode")) Then
                    rwUnitPrice = cUnit.GetOne(CInt(rw("ItemPriceUnitCode")))
                    rwUnitIng = cUnit.GetOne(CInt(rw("itemUnitCode")))

                    rw("itemUnitName") = rw("priceUnit")
                    rw("itemUnitCode") = rw("ItemPriceUnitCode")
                    rw("itemFormat") = rwUnitPrice("format")
                    rw("grossQty") = CDblDB(rw("grossQty")) / CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIng("factor"))
                    rw("netQty") = CDblDB(rw("netQty")) / CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIng("factor"))
                End If

                'convert to main unit
                dtListeSetPrice = CType(cListe.GetListeSetPrice(CInt(rw("codeListe")), intFirstCodeSetPrice, L_udtUser.CodeTrans, enumEgswFetchType.DataTable), DataTable)
                If dtListeSetPrice.Select("unit=" & CStr(rw("ItemPriceUnitCode"))).Length > 0 Then
                    rwListeSetPrice = dtListeSetPrice.Select("unit=" & CStr(rw("ItemPriceUnitCode")))(0)
                    If CInt(rwListeSetPrice("position")) <> 1 Then
                        'rwListeSetPrice1 = dtListeSetPrice.Select("position=1")(0)
                        rwListeSetPrice1 = dtListeSetPrice.Rows(0)

                        rw("priceUnit") = CStr(rwListeSetPrice1("name")).Replace("/", "")
                        rw("itemUnitCode") = rwListeSetPrice1("unit")
                        rw("netQty") = CDblDB(rw("netQty")) * CDblDB(rwListeSetPrice("ratio"))
                        rw("grossQty") = CDblDB(rw("grossQty")) * CDblDB(rwListeSetPrice("ratio"))
                        rw("itemUnitName") = rwListeSetPrice1("name")
                        rw("itemFormat") = rwListeSetPrice1("format")
                        rw("itemCost") = CDblDB(rw("grossQty")) * CDblDB(rwListeSetPrice1("Price"))
                        If CDbl(rw("netQty")) = 0 Then rw("itemUnitName") = ""
                        rw("itemPrice") = rwListeSetPrice1("Price")
                        rw("ItemPriceUnitCode") = rwListeSetPrice1("unit")
                    End If
                End If
                dtListeSetPrice.Dispose() 'DLS May312007
            End If


        Next

        'If blnGroup = False Then Return dt
        Dim dtMerged As New DataTable("IngMerged")
        With dtMerged.Columns
            .Add("codeliste", System.Type.GetType("System.Int32"))
            .Add("name")
            .Add("number")
            .Add("netQty")
            .Add("grossQty", System.Type.GetType("System.Double"))
            .Add("itemUnitName")
            .Add("symbole")
            .Add("itemPrice")
            .Add("itemCost")
            .Add("itemFormat")
            .Add("priceFormat")
            .Add("itemUnitCode", System.Type.GetType("System.Int32"))
            .Add("secondcodesetprice")
            .Add("priceUnit")
        End With

        Dim strRowFilter As String
        For Each row In dt.Rows
            strRowFilter = "codeListe=" & CInt(row("codeListe")) & _
             " AND itemUnitcode=" & CInt(row("itemUnitcode")) ' & _
            '" AND priceUnit='" & CInt(row("priceUnit")) & "'"
            If dtMerged.Select(strRowFilter).Length > 0 Then
                rw = dtMerged.Select(strRowFilter)(0)
                rw("netQty") = CDblDB(row("netQty")) + CDbl(rw("netQty"))
                rw("grossQty") = CDblDB(row("grossQty")) + CDbl(rw("grossQty"))
                'rw("itemPrice") = CDblDB(row("itemPrice")) + CDbl(rw("itemPrice"))
                rw("itemCost") = CDblDB(row("itemCost")) + CDbl(rw("itemCost"))
            Else
                rw = dtMerged.NewRow
                rw("codeliste") = row("codeliste")
                rw("name") = row("name")
                rw("number") = row("number")
                rw("netQty") = row("netQty")
                rw("grossQty") = row("grossQty")
                rw("itemUnitName") = row("itemUnitName")
                rw("symbole") = row("symbole")
                rw("itemPrice") = row("itemPrice")
                rw("itemCost") = row("itemCost")
                rw("itemFormat") = row("itemFormat")
                rw("priceFormat") = row("priceFormat")
                rw("itemUnitCode") = row("itemUnitCode")
                rw("secondcodesetprice") = row("secondcodesetprice")
                rw("priceUnit") = row("priceUnit")
                dtMerged.Rows.Add(rw)
            End If
        Next

        Dim rwZero() As DataRow = dtMerged.Select("grossQty=0")
        For i As Integer = 0 To rwZero.Length - 1
            dtMerged.Rows.Remove(rwZero(i))
        Next

        Return dtMerged
    End Function

    'MRC 08.24.09
    Public Function GetListComputedByYield(ByVal dtCodeliste As DataTable, ByVal intFirstCodeSetPrice As Integer, Optional ByVal dblYield As Double = -1, Optional ByVal blnGroup As Boolean = False, Optional ByVal blnCalculateOnly As Boolean = False, Optional ByVal blnMetImp As Boolean = False) As DataTable
        '// Create Table to Store Ingredients 
       Dim dt As New DataTable("Ing")
        With dt.Columns
            .Add("codeliste")
            .Add("name")
            .Add("number")
            .Add("netQty")
            .Add("grossQty")
            .Add("MetricNetQty") 'JTOC 13.11.2012
            .Add("MetricGrossQty") 'JTOC 13.11.2012
            .Add("ImperialNetQty") 'JTOC 13.11.2012
            .Add("ImperialGrossQty") 'JTOC 13.11.2012
            .Add("itemUnitName")
            .Add("MetricItemUnitName") 'JTOC 13.11.2012
            .Add("ImperialItemUnitName") 'JTOC 13.11.2012
            .Add("symbole")
            .Add("itemPrice")
            .Add("itemPriceImpMet") 'JTOC 09.10.2013
            .Add("itemCost")
            .Add("itemCostImperial") 'JTOC 06.12.2012
            .Add("itemFormat")
            .Add("priceFormat")
            .Add("itemUnitCode")
            .Add("MetricItemUnitCode") 'JTOC 13.11.2012
            .Add("ImperialItemUnitCode") 'JTOC 13.11.2012
            .Add("secondcodesetprice")
            .Add("priceUnit")
            .Add("ItemPriceUnitCode")
            .Add("UnitFactor")
            .Add("IsConverted")
        End With

        '// Get Ingredients of each recipe in array
        Dim cListe As New clsListe(enumAppType.WebApp, L_strCnn)
        Dim dr As SqlDataReader
        Dim row As DataRow
        Dim bUseBestUnit As Boolean = False
        'If Not blnGroup Then bUseBestUnit = L_udtUser.UseBestUnit

        For i As Integer = 0 To dtCodeliste.Rows.Count - 1

            ' If blnCalculateOnly Then
            dr = CType(cListe.GetIngredientsShopping(1, CInt(dtCodeliste.Rows(i)("codeliste")), L_udtUser.CodeTrans, L_udtUser.Site.Code, True, intFirstCodeSetPrice, CDbl(dtCodeliste.Rows(i)("computedyield"))), SqlDataReader)
            'Else
            '    dr = CType(cListe.GetIngredientsShopping(1, CInt(dtCodeliste.Rows(i)("codeliste")), L_udtUser.CodeTrans, L_udtUser.Site.Code, bUseBestUnit, CDbl(dtCodeliste.Rows(i)("yield"))), SqlDataReader)
            'End If


            While dr.Read
                ' only add Ingredients
                If CType(dr.Item("itemType"), enumDataListItemType) = enumDataListItemType.Merchandise Then
                    row = dt.NewRow
                    row("codeliste") = dr.Item("itemcode")
                    row("name") = dr.Item("itemname")
                    row("number") = dr.Item("itemnumber")
                    row("secondcodesetprice") = dr.Item("secondcodesetprice")
                    row("priceUnit") = dr.Item("priceUnit")
                    row("itemUnitCode") = dr.Item("itemUnitCode")
                    row("MetricitemUnitCode") = dr.Item("MetricitemUnitCode") 'JTOC 13.11.2012
                    row("ImperialitemUnitCode") = dr.Item("ImperialitemUnitCode") 'JTOC 13.11.2012
                    row("netQty") = dr.Item("netQuantity")
                    row("MetricNetQty") = dr.Item("MetricNetQuantity") 'JTOC 13.11.2012
                    row("MetricGrossQty") = dr.Item("MetricGrossQuantity")  'JTOC 13.11.2012
                    row("ImperialNetQty") = dr.Item("ImperialNetQuantity") 'JTOC 13.11.2012
                    row("ImperialGrossQty") = dr.Item("ImperialGrossQuantity") 'JTOC 13.11.2012
                    row("itemUnitName") = dr.Item("itemUnit")
                    row("MetricitemUnitName") = dr.Item("MetricitemUnit") 'JTOC 13.11.2012
                    row("ImperialitemUnitName") = dr.Item("ImperialitemUnit") 'JTOC 13.11.2012
                    row("itemFormat") = dr.Item("itemFormat")
                    row("grossQty") = dr.Item("grossQuantity")
                    row("itemCost") = dr.Item("itemCost")
                    row("itemCostImperial") = dr.Item("itemCostImperial") 'JTOC 06.12.2012
                    row("symbole") = dr.Item("symbole")
                    row("priceFormat") = dr.Item("priceFormat")
                    ''If CDbl(dr.Item("netQuantity")) = 0 Then row("itemUnitName") = ""
                    row("itemPrice") = dr.Item("itemPrice")
                    row("ItemPriceUnitCode") = dr.Item("ItemPriceUnitCode")
                    row("itemPriceImpMet") = dr.Item("itemPriceImpMet") 'JTOC 09.10.2013
                    row("UnitFactor") = dr.Item("itemunitcodefactor")
                    row("IsConverted") = dr.Item("IsConverted")
                    dt.Rows.Add(row)
                ElseIf CType(dr.Item("itemType"), enumDataListItemType) = enumDataListItemType.Recipe Then
                    'GetListIngComputedByYield(dt, CInt(dtCodeliste.Rows(i)("codeliste")), CInt(dr.Item("itemcode")), intFirstCodeSetPrice, CDbl(dtCodeliste.Rows(i)("computedyield")))
                    'GetListIngComputedByYield(dt, CInt(dtCodeliste.Rows(i)("codeliste")), CInt(dr.Item("itemcode")), intFirstCodeSetPrice)

                    'If blnCalculateOnly Then
                    '	GetListIngComputedByYield(dt, CInt(dtCodeliste.Rows(i)("codeliste")), CInt(dr.Item("itemcode")), intFirstCodeSetPrice, CDbl(dtCodeliste.Rows(i)("computedyield")))
                    'Else
                    '	GetListIngComputedByYield(dt, CInt(dtCodeliste.Rows(i)("codeliste")), CInt(dr.Item("itemcode")), intFirstCodeSetPrice)
                    'End If
                    'If blnCalculateOnly Then
                    '    GetListIngComputedByYield(dt, CInt(dtCodeliste.Rows(i)("codeliste")), CInt(dr.Item("IngID")), CInt(dr.Item("itemcode")), intFirstCodeSetPrice, CDbl(dtCodeliste.Rows(i)("computedyield")))
                    'Else
                    '    GetListIngComputedByYield(dt, CInt(dtCodeliste.Rows(i)("codeliste")), CInt(dr.Item("IngID")), CInt(dr.Item("itemcode")), intFirstCodeSetPrice)
                    'End If
                End If
            End While
            dr.Close()
        Next

        'Dim cUnit As clsUnit = New clsUnit(L_udtUser, L_AppType, L_strCnn)
        'Dim rwUnitPrice, rwUnitIng, rwUnitIngMet, rwUnitIngImp As DataRow
        'Dim dtListeSetPrice As DataTable
        'Dim rwListeSetPrice1, rwListeSetPrice As DataRow
        Dim rw As DataRow
        'For Each rw In dt.Rows
        '    'convert accdg to price unit
        '    If IsDBNull(rw("ItemPriceUnitCode")) Then
        '        rwUnitPrice = cUnit.GetOne(CInt(rw("itemUnitCode")))
        '        rwUnitIng = cUnit.GetOne(CInt(rw("itemUnitCode")))

        '        'rw("itemUnitName") = rw("itemUnit")
        '        'rw("itemUnitCode") = CInt(rw("itemUnitCode"))
        '        'rw("itemFormat") = rwUnitPrice("format")
        '        rw("grossQty") = 0 'CDblDB(rw("grossQty")) '/ CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIng("factor"))
        '        rw("netQty") = 0 'CDblDB(rw("netQty")) '/ CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIng("factor"))
        '        rw("MetricgrossQty") = 0 'JTOC 13.11.2012
        '        rw("MetricnetQty") = 0 'JTOC 13.11.2012
        '        rw("ImperialgrossQty") = 0 'JTOC 13.11.2012
        '        rw("ImperialnetQty") = 0 'JTOC 13.11.2012
        '    Else
        '        If CInt(rw("ItemPriceUnitCode")) <> CInt(rw("itemUnitCode")) Then
        '            rwUnitPrice = cUnit.GetOne(CInt(rw("ItemPriceUnitCode")))
        '            rwUnitIng = cUnit.GetOne(CInt(rw("itemUnitCode")))
        '            rwUnitIngMet = cUnit.GetOne(CInt(rw("MetricItemUnitCode"))) 'JTOC 09.06.2013
        '            rwUnitIngImp = cUnit.GetOne(CInt(rw("ImperialItemUnitCode"))) 'JTOC 09.06.2013


        '            'If blnMetImp = False Then
        '            '	rw("priceUnit") = rw("itemUnitName")
        '            '	rw("ItemPriceUnitCode") = rw("itemUnitCode")
        '            '	rw("itemPrice") = CDblDB(rw("itemPrice")) * CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIng("factor"))
        '            'Else
        '            '	If rw("MetricnetQty") <> 0 Then
        '            '		rw("priceUnit") = rw("metricItemUnitName")
        '            '		rw("ItemPriceUnitCode") = rw("metricItemUnitCode")
        '            '		rw("itemPrice") = CDblDB(rw("itemPrice")) * CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIngMet("factor"))
        '            '		rw("itemPrice") = CDbl(rw("itemPriceImpMet")) * CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIngMet("factor"))

        '            '	ElseIf rw("MetricnetQty") = 0 And rw("ImperialnetQty") <> 0 Then
        '            '		rw("priceUnit") = rw("imperialItemUnitName")
        '            '		rw("ItemPriceUnitCode") = rw("imperialItemUnitCode")
        '            '		rw("itemPrice") = CDblDB(rw("itemPrice")) * CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIngImp("factor"))
        '            '		rw("itemPrice") = CDbl(rw("itemPriceImpMet")) * CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIngImp("factor"))

        '            '	End If
        '            'End If

        '            'rw("itemUnitName") = rw("priceUnit")
        '            'rw("itemUnitCode") = rw("ItemPriceUnitCode")

        '            'rw("MetricitemUnitName") = rw("priceUnit") 'JTOC 13.11.2012
        '            'rw("MetricitemUnitCode") = rw("ItemPriceUnitCode") 'JTOC 13.11.2012
        '            'rw("ImperialitemUnitName") = rw("priceUnit") 'JTOC 13.11.2012
        '            'rw("ImperialitemUnitCode") = rw("ItemPriceUnitCode") 'JTOC 13.11.2012

        '            'rw("itemFormat") = rwUnitPrice("format")
        '            'rw("grossQty") = CDblDB(rw("grossQty")) / CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIng("factor"))
        '            'rw("netQty") = CDblDB(rw("netQty")) / CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIng("factor"))

        '            'rw("MetricgrossQty") = CDblDB(rw("MetricgrossQty")) / CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIngMet("factor")) 'JTOC 13.11.2012
        '            'rw("MetricnetQty") = CDblDB(rw("MetricnetQty")) / CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIngMet("factor")) 'JTOC 13.11.2012
        '            'rw("ImperialgrossQty") = CDblDB(rw("ImperialgrossQty")) / CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIngImp("factor")) 'JTOC 13.11.2012
        '            'rw("ImperialnetQty") = CDblDB(rw("ImperialnetQty")) / CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIngImp("factor")) 'JTOC 13.11.2012

        '        End If

        '        'convert to main unit
        '        dtListeSetPrice = CType(cListe.GetListeSetPrice(CInt(rw("codeListe")), intFirstCodeSetPrice, L_udtUser.CodeTrans, enumEgswFetchType.DataTable), DataTable)
        '        If dtListeSetPrice.Select("unit=" & CStr(rw("ItemPriceUnitCode"))).Length > 0 Then
        '            rwListeSetPrice = dtListeSetPrice.Select("unit=" & CStr(rw("ItemPriceUnitCode")))(0)
        '            If CInt(rwListeSetPrice("position")) <> 1 Then
        '                'rwListeSetPrice1 = dtListeSetPrice.Select("position=1")(0)
        '                rwListeSetPrice1 = dtListeSetPrice.Rows(0)

        '                rw("priceUnit") = CStr(rwListeSetPrice("name")).Replace("/", "") 'JTOC 09.09.2013 CStr(rwListeSetPrice1("name")).Replace("/", "")
        '                rw("itemUnitCode") = rwListeSetPrice("unit") 'JTOC 09.09.2013 rwListeSetPrice1("unit")
        '                'rw("MetricitemUnitCode") = rwListeSetPrice1("unit")
        '                'rw("ImperialitemUnitCode") = rwListeSetPrice1("unit")
        '                rw("netQty") = CDblDB(rw("netQty")) * CDblDB(rwListeSetPrice("ratio"))
        '                rw("grossQty") = CDblDB(rw("grossQty")) * CDblDB(rwListeSetPrice("ratio"))

        '                'rw("MetricnetQty") = CDblDB(rw("MetricnetQty")) * CDblDB(rwListeSetPrice("ratio")) 'JTOC 13.11.2012
        '                'rw("MetricgrossQty") = CDblDB(rw("MetricgrossQty")) * CDblDB(rwListeSetPrice("ratio")) 'JTOC 13.11.2012
        '                'rw("ImperialnetQty") = CDblDB(rw("ImperialnetQty")) * CDblDB(rwListeSetPrice("ratio")) 'JTOC 13.11.2012
        '                'rw("ImperialgrossQty") = CDblDB(rw("ImperialgrossQty")) * CDblDB(rwListeSetPrice("ratio")) 'JTOC 13.11.2012

        '                rw("itemUnitName") = rwListeSetPrice("name") 'JTOC 09.09.2013 rwListeSetPrice1("name")
        '                'rw("MetricitemUnitName") = rwListeSetPrice1("name") 'JTOC 13.11.2012
        '                'rw("ImperialitemUnitName") = rwListeSetPrice1("name") 'JTOC 13.11.2012
        '                rw("itemFormat") = rwListeSetPrice("format") 'JTOC 09.09.2013 rwListeSetPrice1("format")
        '                rw("itemCost") = CDblDB(rw("grossQty")) * CDblDB(rwListeSetPrice1("Price"))
        '                If CDbl(rw("netQty")) = 0 Then rw("itemUnitName") = ""
        '                'If CDbl(rw("MetricnetQty")) = 0 Then rw("itemUnitName") = "" 'JTOC 13.11.2012
        '                rw("itemPrice") = rwListeSetPrice("Price")  'JTOC 09.09.2013rwListeSetPrice1("Price")
        '                rw("ItemPriceUnitCode") = rwListeSetPrice("unit") 'rwListeSetPrice1("unit") 'JTOC 09.09.2013rwListeSetPrice1("Price")
        '            End If
        '        End If
        '        dtListeSetPrice.Dispose() 'DLS May312007
        '    End If


        'Next

        Dim dtMerged As New DataTable("IngMerged")
        With dtMerged.Columns
            .Add("codeliste", System.Type.GetType("System.Int32"))
            .Add("name")
            .Add("number")
            .Add("netQty")
            .Add("grossQty", System.Type.GetType("System.Double"))

            .Add("MetricnetQty")
            .Add("MetricgrossQty", System.Type.GetType("System.Double"))

            .Add("ImperialnetQty")
            .Add("ImperialgrossQty", System.Type.GetType("System.Double"))

            .Add("itemUnitName")
            .Add("MetricitemUnitName")
            .Add("ImperialitemUnitName")
            .Add("symbole")
            .Add("itemPrice")
            .Add("itemCost")
            .Add("itemCostImperial") 'JTOC 06.12.2012
            .Add("itemFormat")
            .Add("priceFormat")
            .Add("itemUnitCode", System.Type.GetType("System.Int32"))
            .Add("MetricitemUnitCode", System.Type.GetType("System.Int32"))
            .Add("ImperialitemUnitCode", System.Type.GetType("System.Int32"))
            .Add("secondcodesetprice")
            .Add("priceUnit")
            .Add("unitFactor")
        End With

        Dim strRowFilter As String
        For Each row In dt.Rows
            strRowFilter = "codeListe=" & CInt(row("codeListe"))
            '' " AND TypeMain='" & row("TypeMain") & "'"
            '" AND itemUnitcode=" & CInt(row("itemUnitcode")) ' & _
            '" AND priceUnit='" & CInt(row("priceUnit")) & "'"
            If dtMerged.Select(strRowFilter).Length > 0 Then
                rw = dtMerged.Select(strRowFilter)(0)
                rw("netQty") = CDblDB(row("netQty")) + CDblDB(rw("netQty"))
                rw("grossQty") = CDblDB(row("grossQty")) + CDblDB(rw("grossQty"))
                rw("MetricnetQty") = CDblDB(row("MetricnetQty")) + CDblDB(rw("MetricnetQty"))
                rw("MetricgrossQty") = CDblDB(row("MetricgrossQty")) + CDblDB(rw("MetricgrossQty"))
                rw("ImperialnetQty") = CDblDB(row("ImperialnetQty")) + CDblDB(rw("ImperialnetQty"))
                rw("ImperialgrossQty") = CDblDB(row("ImperialgrossQty")) + CDblDB(rw("ImperialgrossQty"))
                rw("itemPrice") = CDblDB(row("itemPrice")) + CDbl(rw("itemPrice"))
                rw("itemCost") = CDblDB(row("itemCost")) + CDblDB(rw("itemCost"))
                rw("itemCostImperial") = CDblDB(row("itemCostImperial")) + CDblDB(rw("itemCostImperial")) 'JTOC 06.12.2012
            Else
                rw = dtMerged.NewRow
                rw("codeliste") = row("codeliste")
                rw("name") = row("name")
                rw("number") = row("number")
                rw("netQty") = row("netQty")
                rw("grossQty") = row("grossQty")

                rw("MetricnetQty") = row("MetricnetQty")
                rw("MetricgrossQty") = row("MetricgrossQty")
                rw("ImperialnetQty") = row("ImperialnetQty")
                rw("ImperialgrossQty") = row("ImperialgrossQty")

                rw("itemUnitName") = row("itemUnitName")
                rw("MetricitemUnitName") = row("MetricitemUnitName")
                rw("ImperialitemUnitName") = row("ImperialitemUnitName")
                rw("symbole") = row("symbole")
                rw("itemPrice") = row("itemPrice")
                rw("itemCost") = row("itemCost")
                rw("itemCostImperial") = row("itemCostImperial") 'JTOC 06.12.2012
                rw("itemFormat") = row("itemFormat")
                rw("priceFormat") = row("priceFormat")
                rw("itemUnitCode") = row("itemUnitCode")
                rw("MetricitemUnitCode") = row("MetricitemUnitCode")
                rw("ImperialitemUnitCode") = row("ImperialitemUnitCode")
                rw("secondcodesetprice") = row("secondcodesetprice")
                rw("priceUnit") = row("priceUnit")

                rw("unitFactor") = row("UnitFactor")
                dtMerged.Rows.Add(rw)
            End If
        Next

        ''Dim dtMerged As New DataTable("IngMerged")
        'With dtMerged.Columns
        '    .Add("codeliste", System.Type.GetType("System.Int32"))
        '    .Add("name")
        '    .Add("number")
        '    .Add("netQty")
        '    .Add("grossQty", System.Type.GetType("System.Double"))

        '    .Add("MetricnetQty")
        '    .Add("MetricgrossQty", System.Type.GetType("System.Double"))

        '    .Add("ImperialnetQty")
        '    .Add("ImperialgrossQty", System.Type.GetType("System.Double"))

        '    .Add("itemUnitName")
        '    .Add("MetricitemUnitName")
        '    .Add("ImperialitemUnitName")
        '    .Add("symbole")
        '    .Add("itemPrice")
        '    .Add("itemCost")
        '    .Add("itemCostImperial") 'JTOC 06.12.2012
        '    .Add("itemFormat")
        '    .Add("priceFormat")
        '    .Add("itemUnitCode", System.Type.GetType("System.Int32"))
        '    .Add("MetricitemUnitCode", System.Type.GetType("System.Int32"))
        '    .Add("ImperialitemUnitCode", System.Type.GetType("System.Int32"))
        '    .Add("secondcodesetprice")
        '    .Add("priceUnit")
        '    .Add("TypeMain")
        '    .Add("unitFactor")
        'End With

        'Dim strRowFilter As String
        'For Each row In dt.Rows
        '    strRowFilter = "codeListe=" & CInt(row("codeListe")) & _
        '        " AND TypeMain='" & row("TypeMain") & "'"
        '    '" AND itemUnitcode=" & CInt(row("itemUnitcode")) ' & _
        '    '" AND priceUnit='" & CInt(row("priceUnit")) & "'"
        '    If dtMerged.Select(strRowFilter).Length > 0 Then
        '        rw = dtMerged.Select(strRowFilter)(0)
        '        If CInt(row("itemUnitcode")) <> rw("itemUnitCode") Then
        '            row("netQty") = CDblDB(row("netQty")) * CDblDB(row("UnitFactor"))
        '            row("grossQty") = CDblDB(row("grossQty")) * CDblDB(row("UnitFactor"))
        '            rw("netQty") = CDblDB(rw("netQty")) * CDblDB(rw("UnitFactor"))
        '            rw("grossQty") = CDblDB(rw("grossQty")) * CDblDB(rw("UnitFactor"))
        '            rw("netQty") = CDblDB(row("netQty")) + CDblDB(rw("netQty"))
        '            rw("grossQty") = CDblDB(row("grossQty")) + CDblDB(rw("grossQty"))

        '            rw("netQty") = CDblDB(rw("netQty")) / CDblDB(rw("UnitFactor"))
        '            rw("grossQty") = CDblDB(rw("grossQty")) / CDblDB(rw("UnitFactor"))

        '        Else
        '            rw("netQty") = CDblDB(row("netQty")) + CDblDB(rw("netQty"))
        '            rw("grossQty") = CDblDB(row("grossQty")) + CDblDB(rw("grossQty"))
        '        End If

        '        rw("MetricnetQty") = CDblDB(row("MetricnetQty")) + CDblDB(rw("MetricnetQty"))
        '        rw("MetricgrossQty") = CDblDB(row("MetricgrossQty")) + CDblDB(rw("MetricgrossQty"))
        '        rw("ImperialnetQty") = CDblDB(row("ImperialnetQty")) + CDblDB(rw("ImperialnetQty"))
        '        rw("ImperialgrossQty") = CDblDB(row("ImperialgrossQty")) + CDblDB(rw("ImperialgrossQty"))
        '        'rw("itemPrice") = CDblDB(row("itemPrice")) + CDbl(rw("itemPrice"))
        '        rw("itemCost") = CDblDB(row("itemCost")) + CDblDB(rw("itemCost"))
        '        rw("itemCostImperial") = CDblDB(row("itemCostImperial")) + CDblDB(rw("itemCostImperial")) 'JTOC 06.12.2012
        '    Else
        '        rw = dtMerged.NewRow
        '        rw("codeliste") = row("codeliste")
        '        rw("name") = row("name")
        '        rw("number") = row("number")
        '        rw("netQty") = row("netQty")
        '        rw("grossQty") = row("grossQty")

        '        rw("MetricnetQty") = row("MetricnetQty")
        '        rw("MetricgrossQty") = row("MetricgrossQty")
        '        rw("ImperialnetQty") = row("ImperialnetQty")
        '        rw("ImperialgrossQty") = row("ImperialgrossQty")

        '        rw("itemUnitName") = row("itemUnitName")
        '        rw("MetricitemUnitName") = row("MetricitemUnitName")
        '        rw("ImperialitemUnitName") = row("ImperialitemUnitName")
        '        rw("symbole") = row("symbole")
        '        rw("itemPrice") = row("itemPrice")
        '        rw("itemCost") = row("itemCost")
        '        rw("itemCostImperial") = row("itemCostImperial") 'JTOC 06.12.2012
        '        rw("itemFormat") = row("itemFormat")
        '        rw("priceFormat") = row("priceFormat")
        '        rw("itemUnitCode") = row("itemUnitCode")
        '        rw("MetricitemUnitCode") = row("MetricitemUnitCode")
        '        rw("ImperialitemUnitCode") = row("ImperialitemUnitCode")
        '        rw("secondcodesetprice") = row("secondcodesetprice")
        '        rw("priceUnit") = row("priceUnit")
        '        rw("TypeMain") = row("TypeMain")
        '        rw("unitFactor") = row("UnitFactor")
        '        dtMerged.Rows.Add(rw)
        '    End If
        'Next

        'MKAM 2015.03.20 - Include ingredients with 0 quantity (Novacoop)
        'Dim rwZero() As DataRow = dtMerged.Select("grossQty=0 AND MetricGrossQty=0") 'AGL 2013.02.15 - 3876
        'For i As Integer = 0 To rwZero.Length - 1
        '    dtMerged.Rows.Remove(rwZero(i))
        'Next

        Return dtMerged
    End Function
End Class

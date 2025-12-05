Imports System.Data.SqlClient
Imports System.Data
Imports System.Char
Imports EGSReadCSV
Imports System.IO
Imports System.Data.OleDb
Imports System.Globalization
Imports System.Text
Imports EgsImportSupplierNetwork

''' <summary>
''' For EgswTempImportProducts Table 
''' </summary>
''' <remarks></remarks>
''' 


Public Class clsImportMerchandise
#Region "Class Header"
    'Name               : clsImportMerchandise
    'Decription         : For EgswTempImportProducts Table
    'Date Created       : 10.01.06
    'Author             : JHL
    'Revision History   : 
    '
#End Region

#Region "Variable Declarations / Dependencies"
    Inherits clsDBRoutine

    Private L_ErrCode As enumEgswErrorCode
    'Private L_lngCodeSite As Int32 = -1
    'Private L_RoleLevelHighest As Int16 = -1

    'Properties
    Private L_udtUser As structUser
    'Private L_dtList As DataTable
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType
    Private L_lngID As Int32
    Private eTab As String = ControlChars.Tab 'VRP 22.08.2008
    Private eCRLF As String = ControlChars.CrLf 'VRP 22.08.2008
    'Private intIDMain As Integer =  

    Public mv_strImportationType As String '//LD20160606 Modular Variable
    Public mv_strCsvSettingsValue As String '//LD20160606 Modular Variable
    Public mv_strCsvSettings As String '//LD20160606 Modular Variable
    Public mv_strCSVSeparator As String '//LD20160606 Modular Variable
    Public mv_strDecimalSeparator As String '//LD20160606 Modular Variable
    Public mv_strThousandSeparator As String '//LD20160606 Modular Variable

#End Region

#Region "Class Functions and Properties"
    Public Sub New(ByVal strCnn As String, _
        Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)

        Try
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn
        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try

    End Sub
#End Region

#Region "Private Methods"
    Private Function FetchList(ByVal intID As Int32, Optional ByVal intCodeSite As Int32 = -1, _
    Optional ByVal blnGetCount As Boolean = False, Optional ByVal strName As String = "") As Object

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
                .CommandText = "sp_EgswTempImportProductsGetList"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 600
                .Parameters.Add("@intID", SqlDbType.Int).Value = intID
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                '.Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = lngCodeProperty
                If strName.Trim <> "" Then _
                    .Parameters.Add("@vchName", SqlDbType.NVarChar, 150).Value = strName
                .Parameters.Add("@bitGetCount", SqlDbType.Bit).Value = CByte(blnGetCount)
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

    Public Function SaveIntoList(ByVal dtmFileDate As DateTime, ByVal strNumber As String, ByVal strName As String, _
    ByVal dblPrice1 As Double, ByVal dblPrice2 As Double, ByVal dblPrice3 As Double, ByVal dblPrice4 As Double, _
    ByVal strUnit1 As String, ByVal strUnit2 As String, ByVal strUnit3 As String, ByVal strUnit4 As String, _
    ByVal dblRatio1 As Double, ByVal dblRatio2 As Double, ByVal dblRatio3 As Double, ByVal strLocation As String, _
    ByVal strSupplier As String, ByVal strCategory As String, ByVal dblTax As Double, ByVal strDescription As String, _
    ByVal dblWastage As Double, ByVal strDeliveryTime As String, ByVal strDeletedProduct As String, ByVal strUnavailable As String, _
    ByVal strOrderMin As String, ByVal strValidityStart As String, ByVal strValidityEnd As String, ByVal strCurrency As String, _
    ByVal strPicture As String, ByVal strComposition As String, ByVal strNutrName1 As String, ByVal strNutrName2 As String, ByVal strNutrName3 As String, _
    ByVal strNutrName4 As String, ByVal strNutrName5 As String, ByVal strNutrName6 As String, ByVal strNutrName7 As String, ByVal strNutrName8 As String, _
    ByVal strNutrName9 As String, ByVal strNutrName10 As String, ByVal strNutrName11 As String, ByVal strNutrName12 As String, ByVal dblNutrVal1 As Double, _
    ByVal dblNutrVal2 As Double, ByVal dblNutrVal3 As Double, ByVal dblNutrVal4 As Double, ByVal dblNutrVal5 As Double, ByVal dblNutrVal6 As Double, _
    ByVal dblNutrVal7 As Double, ByVal dblNutrVal8 As Double, ByVal dblNutrVal9 As Double, ByVal dblNutrVal10 As Double, ByVal dblNutrVal11 As Double, _
    ByVal dblNutrVal12 As Double, ByVal blnForRN As Boolean, ByVal dblWastage2 As Double, ByVal dblWastage3 As Double, ByVal dblWastage4 As Double, _
    ByVal dblWastage5 As Double, ByVal strIngredients As String, ByVal strPreparation As String, ByVal strCookingtip As String, ByVal strRefinement As String, _
    ByVal strStorage As String, ByVal strProductivity As String, ByVal blnImportFB As Boolean, ByVal blnImportRN As Boolean, _
    ByVal intCodeSite As Integer, ByVal intCodeSupplierGroup As Integer, ByVal blnImportNewSupplier As Boolean, ByVal blnForFB As Boolean, _
    Optional ByVal intImportID As Integer = 0) As enumEgswErrorCode
        Dim arrParam(71) As SqlParameter
        arrParam(0) = New SqlParameter("@FileDate", dtmFileDate)
        arrParam(1) = New SqlParameter("@Number", strNumber)
        arrParam(2) = New SqlParameter("@Name", strName)
        arrParam(3) = New SqlParameter("@Price1", dblPrice1)
        arrParam(4) = New SqlParameter("@Price2", dblPrice2)
        arrParam(5) = New SqlParameter("@Price3", dblPrice3)
        arrParam(6) = New SqlParameter("@Price4", dblPrice4)
        arrParam(7) = New SqlParameter("@Unit1", strUnit1)
        arrParam(8) = New SqlParameter("@Unit2", strUnit2)
        arrParam(9) = New SqlParameter("@Unit3", strUnit3)
        arrParam(10) = New SqlParameter("@Unit4", strUnit4)
        arrParam(11) = New SqlParameter("@Ratio1", dblRatio1)
        arrParam(12) = New SqlParameter("@Ratio2", dblRatio2)
        arrParam(13) = New SqlParameter("@Ratio3", dblRatio3)
        arrParam(14) = New SqlParameter("@Location", strLocation)
        arrParam(15) = New SqlParameter("@Supplier", strSupplier)
        arrParam(16) = New SqlParameter("@Category", strCategory)
        arrParam(17) = New SqlParameter("@Tax", dblTax)
        arrParam(18) = New SqlParameter("@Description", strDescription)
        arrParam(19) = New SqlParameter("@Wastage", dblWastage)
        arrParam(20) = New SqlParameter("@DeliveryTime", strDeliveryTime)
        arrParam(21) = New SqlParameter("@DeletedProduct", strDeletedProduct)
        arrParam(22) = New SqlParameter("@Unavailable", strUnavailable)
        arrParam(23) = New SqlParameter("@OrderMin", strOrderMin)
        arrParam(24) = New SqlParameter("@ValidityStart", strValidityStart)
        arrParam(25) = New SqlParameter("@ValidityEnd", strValidityEnd)
        arrParam(26) = New SqlParameter("@Currency", strCurrency)
        arrParam(27) = New SqlParameter("@Picture", strPicture)
        arrParam(28) = New SqlParameter("@Composition", strComposition)
        arrParam(29) = New SqlParameter("@NutrName1", strNutrName1)
        arrParam(30) = New SqlParameter("@NutrName2", strNutrName2)
        arrParam(31) = New SqlParameter("@NutrName3", strNutrName3)
        arrParam(32) = New SqlParameter("@NutrName4", strNutrName4)
        arrParam(33) = New SqlParameter("@NutrName5", strNutrName5)
        arrParam(34) = New SqlParameter("@NutrName6", strNutrName6)
        arrParam(35) = New SqlParameter("@NutrName7", strNutrName7)
        arrParam(36) = New SqlParameter("@NutrName8", strNutrName8)
        arrParam(37) = New SqlParameter("@NutrName9", strNutrName9)
        arrParam(38) = New SqlParameter("@NutrName10", strNutrName10)
        arrParam(39) = New SqlParameter("@NutrName11", strNutrName11)
        arrParam(40) = New SqlParameter("@NutrName12", strNutrName12)
        arrParam(41) = New SqlParameter("@NutrVal1", dblNutrVal1)
        arrParam(42) = New SqlParameter("@NutrVal2", dblNutrVal2)
        arrParam(43) = New SqlParameter("@NutrVal3", dblNutrVal3)
        arrParam(44) = New SqlParameter("@NutrVal4", dblNutrVal4)
        arrParam(45) = New SqlParameter("@NutrVal5", dblNutrVal5)
        arrParam(46) = New SqlParameter("@NutrVal6", dblNutrVal6)
        arrParam(47) = New SqlParameter("@NutrVal7", dblNutrVal7)
        arrParam(48) = New SqlParameter("@NutrVal8", dblNutrVal8)
        arrParam(49) = New SqlParameter("@NutrVal9", dblNutrVal9)
        arrParam(50) = New SqlParameter("@NutrVal10", dblNutrVal10)
        arrParam(51) = New SqlParameter("@NutrVal11", dblNutrVal11)
        arrParam(52) = New SqlParameter("@NutrVal12", dblNutrVal12)
        arrParam(53) = New SqlParameter("@ForRN", blnForRN)
        arrParam(54) = New SqlParameter("@Wastage2", dblWastage2)
        arrParam(55) = New SqlParameter("@Wastage3", dblWastage3)
        arrParam(56) = New SqlParameter("@Wastage4", dblWastage4)
        arrParam(57) = New SqlParameter("@Wastage5 ", dblWastage5)
        arrParam(58) = New SqlParameter("@Ingredients", strIngredients)
        arrParam(59) = New SqlParameter("@Preparation", strPreparation)
        arrParam(60) = New SqlParameter("@Cookingtip", strCookingtip)
        arrParam(61) = New SqlParameter("@Refinement", strRefinement)
        arrParam(62) = New SqlParameter("@Storage", strStorage)
        arrParam(63) = New SqlParameter("@Productivity", strProductivity)
        arrParam(64) = New SqlParameter("@ImportFB", blnImportFB)
        arrParam(65) = New SqlParameter("@ImportRN", blnImportRN)
        arrParam(66) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(67) = New SqlParameter("@CodeSupplierGroup", intCodeSupplierGroup)
        arrParam(68) = New SqlParameter("@ImportNewSupplier", blnImportNewSupplier)
        arrParam(69) = New SqlParameter("@ForFB", blnForFB)
        arrParam(70) = New SqlParameter("@retval", SqlDbType.Int)
        arrParam(70).Direction = ParameterDirection.ReturnValue
        arrParam(71) = New SqlParameter("@ImportID", intImportID)

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswTempImportProductsUpdate", arrParam)
            Return CType(arrParam(70).Value, enumEgswErrorCode)
        Catch ex As Exception
            ' Throw ex
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

#End Region

#Region "Get Methods"
    ''' <summary>
    ''' Get List from EgswImportProducts.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>    
    'Public Overloads Function GetList() As Object
    '    Return FetchList(-1)
    'End Function

    ''' <summary>
    ''' Get Record Count from EgswImportProducts.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>    
    Public Overloads Function GetList(ByVal blnGetCount As Boolean) As Object
        Return FetchList(-1, -1, blnGetCount)
    End Function

    ''' <summary>
    ''' Get List from EgswImportProducts.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>    
    Public Overloads Function GetList(Optional ByVal intID As Integer = -1, Optional ByVal intCodeSite As Integer = -1) As Object
        Return FetchList(intID, intCodeSite)
    End Function

#End Region


#Region " Bulk Import "
    Private Enum enumImportType
        CSVStandardFile = 0
        PistorFile = 1
        XLSFile = 2
        AutogrillFormat = -1
        Scana = 3
        SupplierNetwork = 4
        PistorFileNoHeader = 5
    End Enum

    Public Function fctGetDataSource(ByVal strFilePath As String, ByVal intImportType As Integer, Optional ByVal strDelimiter As String = ";", Optional ByVal strDecimalSeparator As String = ".", Optional ByVal strThousandSeparator As String = ",") As DataTable
        Dim dt As DataTable
        Select Case intImportType
            Case enumImportType.PistorFile
                dt = fctGetPISTORDataSource(strFilePath)

            Case enumImportType.PistorFileNoHeader          'mrc 04.20.2010
                dt = fctGetPISTORDataSource2(strFilePath)

            Case enumImportType.Scana
                dt = fctGetSCANADataSource(strFilePath)
            Case Else
                dt = fctGetCSVDataSource(strFilePath, intImportType, strDelimiter, strDecimalSeparator, strThousandSeparator)
        End Select
        Return dt
    End Function

    Public Function fctGetCSVDataSource(ByVal strFilePath As String, ByVal intImportType As Integer, Optional ByVal strDelimiter As String = ";", Optional ByVal strDecimalSeparator As String = ".", Optional ByVal strThousandSeparator As String = ",") As DataTable
        Dim dt As DataTable
        'Dim fileCSV As New StreamReader(strFilePath, Encoding.Default)
        'Dim objReadCSV As New CsvDataReader(fileCSV.BaseStream, CChar(strDelimiter), CChar(eTab), CChar(eCRLF))
        'dt = objReadCSV.Table

        'If System.IO.Path.GetExtension(strFilePath).ToLower() = ".xls" Then
        '    dt = ReadXLS(strFilePath)
        '    Return dt
        'End If

        dt = ReadCSV(strFilePath, strDelimiter)
        Dim row As DataRow = dt.Rows(0)
        Dim flagPrice1 As Boolean = False
        Dim flagPrice2 As Boolean = False
        Dim flagPrice3 As Boolean = False
        Dim flagPrice4 As Boolean = False
        Dim flagRatio2 As Boolean = False
        Dim flagTax As Boolean = False
        Dim flagWastage As Boolean = False
        Dim arryNutrientColIndex As New ArrayList
        Dim i As Integer = 0

        If intImportType = enumImportType.AutogrillFormat Then
            EditColName(dt)
            Return dt
            Exit Function
        ElseIf intImportType = enumImportType.CSVStandardFile Then
            EditColName(dt)
            Try
                For Each col As DataColumn In dt.Columns
                    'If row.Item(col).ToString <> "" Then
                    'col.ColumnName = row.Item(col).ToString
                    If col.ColumnName = "Price1" Then flagPrice1 = True
                    If col.ColumnName = "Price2" Then flagPrice2 = True
                    If col.ColumnName = "Price3" Then flagPrice3 = True
                    If col.ColumnName = "Price4" Then flagPrice4 = True
                    If col.ColumnName = "Tax" Then flagTax = True
                    If col.ColumnName = "Wastage" Then flagWastage = True
                    If Mid(col.ColumnName, 1, 2) = "N=" Then
                        arryNutrientColIndex.Add(i)
                    End If
                    'End If
                    i += 1
                Next
                'dt.Rows.Remove(row)
            Catch ex As Exception
            End Try
        ElseIf intImportType = enumImportType.Scana Then
            EditColNameSCANA(dt)

            flagPrice1 = False
            flagPrice2 = True
            flagPrice3 = False
            flagPrice4 = False
            flagRatio2 = True
            ''Return dt
            ''Exit Function
        End If


        Dim strLocaleDecimalSepar As String = strDecimalSeparator 'NumberFormatInfo.CurrentInfo.NumberDecimalSeparator LD20160603.YYYYMMDD Add Update of 'LLG 03.06.2016.DDMMYYY-
        Dim strLocaleThousandSepar As String = strThousandSeparator 'NumberFormatInfo.CurrentInfo.NumberGroupSeparator  LD20160603.YYYYMMDD Add Update of  'LLG 03.06.2016.DDMMYYY -

        Dim ix As Integer
        Dim colX As Integer

        '--------- Replace Seperator ---------------
        If strDelimiter = ";" And strDecimalSeparator <> strLocaleDecimalSepar And (flagPrice1 Or flagPrice2 Or flagPrice3 Or flagPrice4 Or flagTax Or flagWastage Or arryNutrientColIndex.Count > 0) Then
            For Each row In dt.Rows
                If flagPrice1 AndAlso IsDBNull(row.Item("Price1")) = False Then row.Item("Price1") = Replace(CStrDB(row.Item("Price1")), strDecimalSeparator, strLocaleDecimalSepar) : Replace(CStrDB(row.Item("Price1")), strThousandSeparator, strLocaleDecimalSepar)
                If flagPrice2 AndAlso IsDBNull(row.Item("Price2")) = False Then row.Item("Price2") = Replace(CStrDB(row.Item("Price2")), strDecimalSeparator, strLocaleDecimalSepar) : Replace(CStrDB(row.Item("Price2")), strThousandSeparator, strLocaleDecimalSepar)
                If flagPrice3 AndAlso IsDBNull(row.Item("Price3")) = False Then row.Item("Price3") = Replace(CStrDB(row.Item("Price3")), strDecimalSeparator, strLocaleDecimalSepar) : Replace(CStrDB(row.Item("Price3")), strThousandSeparator, strLocaleDecimalSepar)
                If flagPrice4 AndAlso IsDBNull(row.Item("Price4")) = False Then row.Item("Price4") = Replace(CStrDB(row.Item("Price4")), strDecimalSeparator, strLocaleDecimalSepar) : Replace(CStrDB(row.Item("Price4")), strThousandSeparator, strLocaleDecimalSepar)

                If flagRatio2 AndAlso IsDBNull(row.Item("Ratio2")) = False Then row.Item("Ratio2") = Replace(CStrDB(row.Item("Ratio2")), strDecimalSeparator, strLocaleDecimalSepar) : Replace(CStrDB(row.Item("Ratio2")), strThousandSeparator, strLocaleDecimalSepar)

                For ix = 0 To arryNutrientColIndex.Count - 1
                    colX = CInt(arryNutrientColIndex(ix))
                    If IsDBNull(row.Item(colX)) = False Then row.Item(colX) = Replace(CStr(row.Item(colX)), strDecimalSeparator, strLocaleDecimalSepar) : row.Item(colX) = Replace(CStrDB(row.Item(colX)), strThousandSeparator, strLocaleDecimalSepar)
                Next

                If intImportType = enumImportType.Scana Then
                    row("Unit1") = Replace(CStr(row("Unit1")).Trim, """", "")
                    row("Unit2") = Replace(CStr(row("Unit2")).Trim, """", "")
                    'row("Price1") = CDblDB(row("Price1"))
                    row("Price2") = CDblDB(row("Price2"))
                    row("Ratio2") = CDblDB(row("Ratio2"))
                End If
            Next
            ''If flagPrice1 Then dt.Columns("Price1").DataType = System.Type.GetType("System.Double")
            ''If flagPrice2 Then dt.Columns("Price2").DataType = System.Type.GetType("System.Double")
            ''If flagPrice3 Then dt.Columns("Price3").DataType = System.Type.GetType("System.Double")
            ''If flagPrice4 Then dt.Columns("Price4").DataType = System.Type.GetType("System.Double")
            ''If flagTax Then dt.Columns("Tax").DataType = System.Type.GetType("System.Double")
            ''If flagWastage Then dt.Columns("Wastage").DataType = System.Type.GetType("System.Double")
        End If


        '------------ Formatting --------------------
        If intImportType = enumImportType.Scana Then
            subFormatScanaData(dt)
        End If

        Return dt
    End Function


    Public Function detectTextEncoding(ByVal filename As String, <Runtime.InteropServices.Out> ByRef text As String, Optional ByVal taster As Integer = 1000) As Encoding
        try

        Dim b As Byte() = File.ReadAllBytes(filename)
        If b.Length >= 4 AndAlso b(0) = 0 AndAlso b(1) = 0 AndAlso b(2) = 254 AndAlso b(3) = 255 Then
            text = Encoding.GetEncoding("utf-32BE").GetString(b, 4, b.Length - 4)
            Return Encoding.GetEncoding("utf-32BE")
        ElseIf b.Length >= 4 AndAlso b(0) = 255 AndAlso b(1) = 254 AndAlso b(2) = 0 AndAlso b(3) = 0 Then
            text = Encoding.UTF32.GetString(b, 4, b.Length - 4)
            Return Encoding.UTF32
        ElseIf b.Length >= 2 AndAlso b(0) = 254 AndAlso b(1) = 255 Then
            text = Encoding.BigEndianUnicode.GetString(b, 2, b.Length - 2)
            Return Encoding.BigEndianUnicode
        ElseIf b.Length >= 2 AndAlso b(0) = 255 AndAlso b(1) = 254 Then
            text = Encoding.Unicode.GetString(b, 2, b.Length - 2)
            Return Encoding.Unicode
        ElseIf b.Length >= 3 AndAlso b(0) = 239 AndAlso b(1) = 187 AndAlso b(2) = 191 Then
            text = Encoding.UTF8.GetString(b, 3, b.Length - 3)
            Return Encoding.UTF8
        ElseIf b.Length >= 3 AndAlso b(0) = 43 AndAlso b(1) = 47 AndAlso b(2) = 118 Then
            text = Encoding.UTF7.GetString(b, 3, b.Length - 3)
            Return Encoding.UTF7
        End If

        If taster = 0 OrElse taster > b.Length Then taster = b.Length
        Dim i As Integer = 0
        Dim utf8 As Boolean = False
        While i < taster - 4
            If b(i) <= 127 Then
                i += 1
                Continue While
            End If

            If b(i) >= 194 AndAlso b(i) <= 223 AndAlso b(i + 1) >= 128 AndAlso b(i + 1) < 192 Then
                i += 2
                utf8 = True
                Continue While
            End If

            If b(i) >= 224 AndAlso b(i) <= 240 AndAlso b(i + 1) >= 128 AndAlso b(i + 1) < 192 AndAlso b(i + 2) >= 128 AndAlso b(i + 2) < 192 Then
                i += 3
                utf8 = True
                Continue While
            End If

            If b(i) >= 240 AndAlso b(i) <= 244 AndAlso b(i + 1) >= 128 AndAlso b(i + 1) < 192 AndAlso b(i + 2) >= 128 AndAlso b(i + 2) < 192 AndAlso b(i + 3) >= 128 AndAlso b(i + 3) < 192 Then
                i += 4
                utf8 = True
                Continue While
            End If

            utf8 = False
            Exit While
        End While

        If utf8 = True Then
            text = Encoding.UTF8.GetString(b)
            Return Encoding.UTF8
        End If

        Dim threshold As Double = 0.1
        Dim count As Integer = 0
        For n As Integer = 0 To taster - 1 Step 2
            If b(n) = 0 Then count += 1
        Next

        If (CDbl(count)) / taster > threshold Then
            text = Encoding.BigEndianUnicode.GetString(b)
            Return Encoding.BigEndianUnicode
        End If

        count = 0
        For n As Integer = 1 To taster - 1 Step 2
            If b(n) = 0 Then count += 1
        Next

        If (CDbl(count)) / taster > threshold Then
            text = Encoding.Unicode.GetString(b)
            Return Encoding.Unicode
        End If

        For n As Integer = 0 To taster - 9 - 1
            If ((b(n + 0) = "c" OrElse b(n + 0) = "C") AndAlso (b(n + 1) = "h" OrElse b(n + 1) = "H") AndAlso (b(n + 2) = "a" OrElse b(n + 2) = "A") AndAlso (b(n + 3) = "r" OrElse b(n + 3) = "R") AndAlso (b(n + 4) = "s" OrElse b(n + 4) = "S") AndAlso (b(n + 5) = "e" OrElse b(n + 5) = "E") AndAlso (b(n + 6) = "t" OrElse b(n + 6) = "T") AndAlso (b(n + 7) = "=")) OrElse ((b(n + 0) = "e" OrElse b(n + 0) = "E") AndAlso (b(n + 1) = "n" OrElse b(n + 1) = "N") AndAlso (b(n + 2) = "c" OrElse b(n + 2) = "C") AndAlso (b(n + 3) = "o" OrElse b(n + 3) = "O") AndAlso (b(n + 4) = "d" OrElse b(n + 4) = "D") AndAlso (b(n + 5) = "i" OrElse b(n + 5) = "I") AndAlso (b(n + 6) = "n" OrElse b(n + 6) = "N") AndAlso (b(n + 7) = "g" OrElse b(n + 7) = "G") AndAlso (b(n + 8) = "=")) Then
                If b(n + 0) = "c" OrElse b(n + 0) = "C" Then n += 8 Else n += 9
                If b(n) = """" OrElse b(n) = "'" Then n += 1
                Dim oldn As Integer = n
                While n < taster AndAlso (b(n) = "_" OrElse b(n) = "-" OrElse (b(n) >= "0" AndAlso b(n) <= "9") OrElse (b(n) >= "a" AndAlso b(n) <= "z") OrElse (b(n) >= "A" AndAlso b(n) <= "Z"))
                    n += 1
                End While

                Dim nb As Byte() = New Byte(n - oldn - 1) {}
                Array.Copy(b, oldn, nb, 0, n - oldn)
                Try
                    Dim internalEnc As String = Encoding.ASCII.GetString(nb)
                    text = Encoding.GetEncoding(internalEnc).GetString(b)
                    Return Encoding.GetEncoding(internalEnc)
                Catch
                    Exit For
                End Try
            End If
        Next

        text = Encoding.[Default].GetString(b)
        Return Encoding.[Default]
         Catch ex As Exception

            Return System.Text.Encoding.Default

        End Try

    End Function


    Function ReadCSV(strFileName As String, strDelimiters As String, Optional strdecimalSeparator As String = ".", Optional strThousandSeparator As String = ",")
        Dim outtext As String = ""
        Dim SelectedEncoding As System.Text.Encoding = detectTextEncoding(strFileName, outtext)
        Dim TextFileReader As New Microsoft.VisualBasic.FileIO.TextFieldParser(strFileName, SelectedEncoding, True)
        '//LD2016060 Overide Format
        GetRegionalSeparatorFormat()
        strDelimiters = mv_strCSVSeparator
        strdecimalSeparator = mv_strDecimalSeparator
        strThousandSeparator = mv_strThousandSeparator


        TextFileReader.TextFieldType = FileIO.FieldType.Delimited
        TextFileReader.SetDelimiters(strDelimiters)
        TextFileReader.HasFieldsEnclosedInQuotes = True
        Dim NumberFormat As String = "####0.000"
        Dim DecimalCount As Integer = 4
        BULKImportNumberFormat(NumberFormat, DecimalCount)
        Dim DecimalCount2 As Integer = DecimalCount - 1
        Dim TextFileTable As DataTable = Nothing

        Dim Column As DataColumn
        Dim Row As DataRow
        Dim UpperBound As Int32
        Dim ColumnCount As Int32
        Dim CurrentRow As String()

        While Not TextFileReader.EndOfData
            Try
                CurrentRow = TextFileReader.ReadFields()
                If Not CurrentRow Is Nothing Then
                    ''# Check if DataTable has been created
                    If TextFileTable Is Nothing Then
                        TextFileTable = New DataTable("TextFileTable")
                        ''# Get number of columns
                        UpperBound = CurrentRow.GetUpperBound(0)
                        ''# Create new DataTable
                        For ColumnCount = 0 To UpperBound
                            Column = New DataColumn()
                            Column.DataType = System.Type.GetType("System.String")
                            Column.ColumnName = CheckColName(CurrentRow(ColumnCount)) ' CurrentRow(ColumnCount)
                            Column.Caption = CurrentRow(ColumnCount)
                            Column.ReadOnly = False
                            Column.Unique = False
                            TextFileTable.Columns.Add(Column)
                        Next
                    End If





                    '1st row is for column headers, succeeding rows the data
                    If TextFileReader.LineNumber >= 3 Or TextFileReader.LineNumber = -1 Then
                        Row = TextFileTable.NewRow
                        For ColumnCount = 0 To UpperBound

                            If ColumnCount >= CurrentRow.Length Then
                                Continue For
                            End If

                            Row(ColumnCount) = CurrentRow(ColumnCount).ToString


                            ''LDSTART Price1 Trace languange LD20160406/YYYYMMDD - 
                            If TextFileTable.Columns(ColumnCount).ColumnName.ToLower() = "price1" Then
                                If CurrentRow(ColumnCount).ToString <> "" Then
                                    Try
                                        If strThousandSeparator = "," Then
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), System.Globalization.CultureInfo.GetCultureInfo("en-US")).ToString(NumberFormat)
                                        ElseIf strThousandSeparator = "." Then
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), CultureInfo.GetCultureInfo("it-IT")).ToString(NumberFormat)
                                        Else
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), System.Globalization.CultureInfo.InvariantCulture).ToString(NumberFormat)
                                        End If
                                    Catch ex As Exception
                                    Finally

                                    End Try
                                    Row(ColumnCount) = Double.Parse(Row(ColumnCount).Replace(" ", "")).ToString(NumberFormat)

                                    If Row(ColumnCount).ToString().Substring(Row(ColumnCount).ToString().Length - DecimalCount, 1) = "," Then
                                        Row(ColumnCount) = Row(ColumnCount).ToString().Substring(0, Row(ColumnCount).ToString().Length - DecimalCount) & "." & Row(ColumnCount).ToString().Substring(Row(ColumnCount).ToString().Length - DecimalCount2, DecimalCount2)
                                    End If
                                End If


                                ''LDSTART Price4 Trace languange LD20160406/YYYYMMDD - 
                            ElseIf TextFileTable.Columns(ColumnCount).ColumnName.ToLower() = "price2" Then
                                If CurrentRow(ColumnCount).ToString <> "" Then
                                    Try
                                        If strThousandSeparator = "," Then
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), System.Globalization.CultureInfo.GetCultureInfo("en-US")).ToString(NumberFormat)
                                        ElseIf strThousandSeparator = "." Then
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), CultureInfo.GetCultureInfo("it-IT")).ToString(NumberFormat)
                                        Else
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), System.Globalization.CultureInfo.InvariantCulture).ToString(NumberFormat)
                                        End If
                                    Catch ex As Exception
                                    Finally

                                    End Try
                                    Row(ColumnCount) = Double.Parse(Row(ColumnCount).Replace(" ", "")).ToString(NumberFormat)

                                    If Row(ColumnCount).ToString().Substring(Row(ColumnCount).ToString().Length - DecimalCount, 1) = "," Then
                                        Row(ColumnCount) = Row(ColumnCount).ToString().Substring(0, Row(ColumnCount).ToString().Length - DecimalCount) & "." & Row(ColumnCount).ToString().Substring(Row(ColumnCount).ToString().Length - DecimalCount2, DecimalCount2)
                                    End If
                                End If


                                ''LDSTART Price4 Trace languange LD20160406/YYYYMMDD - 
                            ElseIf TextFileTable.Columns(ColumnCount).ColumnName.ToLower() = "price3" Then
                                If CurrentRow(ColumnCount).ToString <> "" Then
                                    Try
                                        If strThousandSeparator = "," Then
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), System.Globalization.CultureInfo.GetCultureInfo("en-US")).ToString(NumberFormat)
                                        ElseIf strThousandSeparator = "." Then
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), CultureInfo.GetCultureInfo("it-IT")).ToString(NumberFormat)
                                        Else
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), System.Globalization.CultureInfo.InvariantCulture).ToString(NumberFormat)
                                        End If
                                    Catch ex As Exception
                                    Finally

                                    End Try
                                    Row(ColumnCount) = Double.Parse(Row(ColumnCount).Replace(" ", "")).ToString(NumberFormat)

                                    If Row(ColumnCount).ToString().Substring(Row(ColumnCount).ToString().Length - DecimalCount, 1) = "," Then
                                        Row(ColumnCount) = Row(ColumnCount).ToString().Substring(0, Row(ColumnCount).ToString().Length - DecimalCount) & "." & Row(ColumnCount).ToString().Substring(Row(ColumnCount).ToString().Length - DecimalCount2, DecimalCount2)
                                    End If
                                End If



                                ''LDSTART Price4 Trace languange LD20160406/YYYYMMDD - 
                            ElseIf TextFileTable.Columns(ColumnCount).ColumnName.ToLower() = "price4" Then
                                If CurrentRow(ColumnCount).ToString <> "" Then
                                    Try
                                        If strThousandSeparator = "," Then
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), System.Globalization.CultureInfo.GetCultureInfo("en-US")).ToString(NumberFormat)
                                        ElseIf strThousandSeparator = "." Then
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), CultureInfo.GetCultureInfo("it-IT")).ToString(NumberFormat)
                                        Else
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), System.Globalization.CultureInfo.InvariantCulture).ToString(NumberFormat)
                                        End If
                                    Catch ex As Exception
                                    Finally

                                    End Try
                                    Row(ColumnCount) = Double.Parse(Row(ColumnCount).Replace(" ", "")).ToString(NumberFormat)

                                    If Row(ColumnCount).ToString().Substring(Row(ColumnCount).ToString().Length - DecimalCount, 1) = "," Then
                                        Row(ColumnCount) = Row(ColumnCount).ToString().Substring(0, Row(ColumnCount).ToString().Length - DecimalCount) & "." & Row(ColumnCount).ToString().Substring(Row(ColumnCount).ToString().Length - DecimalCount2, DecimalCount2)
                                    End If
                                End If



                                ''LDSTART Ratio1 Trace languange LD20160406/YYYYMMDD - 
                            ElseIf TextFileTable.Columns(ColumnCount).ColumnName.ToLower() = "ratio1" Then
                                If CurrentRow(ColumnCount).ToString <> "" Then
                                    Try
                                        If strThousandSeparator = "," Then
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), System.Globalization.CultureInfo.GetCultureInfo("en-US")).ToString(NumberFormat)
                                        ElseIf strThousandSeparator = "." Then
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), CultureInfo.GetCultureInfo("it-IT")).ToString(NumberFormat)
                                        Else
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), System.Globalization.CultureInfo.InvariantCulture).ToString(NumberFormat)
                                        End If
                                    Catch ex As Exception
                                    Finally

                                    End Try
                                    Row(ColumnCount) = Double.Parse(Row(ColumnCount).Replace(" ", "")).ToString(NumberFormat)

                                    If Row(ColumnCount).ToString().Substring(Row(ColumnCount).ToString().Length - DecimalCount, 1) = "," Then
                                        Row(ColumnCount) = Row(ColumnCount).ToString().Substring(0, Row(ColumnCount).ToString().Length - DecimalCount) & "." & Row(ColumnCount).ToString().Substring(Row(ColumnCount).ToString().Length - DecimalCount2, DecimalCount2)
                                    End If
                                End If


                                ''LDSTART Ratio2 Trace languange LD20160406/YYYYMMDD - 
                            ElseIf TextFileTable.Columns(ColumnCount).ColumnName.ToLower() = "ratio2" Then
                                If CurrentRow(ColumnCount).ToString <> "" Then
                                    Try
                                        If strThousandSeparator = "," Then
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), System.Globalization.CultureInfo.GetCultureInfo("en-US")).ToString(NumberFormat)
                                        ElseIf strThousandSeparator = "." Then
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), CultureInfo.GetCultureInfo("it-IT")).ToString(NumberFormat)
                                        Else
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), System.Globalization.CultureInfo.InvariantCulture).ToString(NumberFormat)
                                        End If
                                    Catch ex As Exception
                                    Finally

                                    End Try
                                    Row(ColumnCount) = Double.Parse(Row(ColumnCount).Replace(" ", "")).ToString(NumberFormat)

                                    If Row(ColumnCount).ToString().Substring(Row(ColumnCount).ToString().Length - DecimalCount, 1) = "," Then
                                        Row(ColumnCount) = Row(ColumnCount).ToString().Substring(0, Row(ColumnCount).ToString().Length - DecimalCount) & "." & Row(ColumnCount).ToString().Substring(Row(ColumnCount).ToString().Length - DecimalCount2, DecimalCount2)
                                    End If
                                End If


                                ''LDSTART Ratio2 Trace languange LD20160406/YYYYMMDD - 
                            ElseIf TextFileTable.Columns(ColumnCount).ColumnName.ToLower() = "ratio3" Then
                                If CurrentRow(ColumnCount).ToString <> "" Then
                                    Try
                                        If strThousandSeparator = "," Then
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), System.Globalization.CultureInfo.GetCultureInfo("en-US")).ToString(NumberFormat)
                                        ElseIf strThousandSeparator = "." Then
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), CultureInfo.GetCultureInfo("it-IT")).ToString(NumberFormat)
                                        Else
                                            Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), System.Globalization.CultureInfo.InvariantCulture).ToString(NumberFormat)
                                        End If
                                    Catch ex As Exception
                                    Finally

                                    End Try
                                    Row(ColumnCount) = Double.Parse(Row(ColumnCount).Replace(" ", "")).ToString(NumberFormat)

                                    If Row(ColumnCount).ToString().Substring(Row(ColumnCount).ToString().Length - DecimalCount, 1) = "," Then
                                        Row(ColumnCount) = Row(ColumnCount).ToString().Substring(0, Row(ColumnCount).ToString().Length - DecimalCount) & "." & Row(ColumnCount).ToString().Substring(Row(ColumnCount).ToString().Length - DecimalCount2, DecimalCount2)
                                    End If
                                End If



                                'Wastage1 Add default Value LD20160406/YYYYMMDD - handle Blank Value - Value Required is Integer for Wastage1
                            ElseIf TextFileTable.Columns(ColumnCount).ColumnName.ToLower() = "wastage1" Then
                                If CurrentRow(ColumnCount).ToString = "" Then Row(ColumnCount) = "0"


                                'Wastage2 Add default Value LD20160406/YYYYMMDD - handle Blank Value - Value Required is Integer for Wastage1
                            ElseIf TextFileTable.Columns(ColumnCount).ColumnName.ToLower() = "wastage2" Then
                                If CurrentRow(ColumnCount).ToString = "" Then Row(ColumnCount) = "0"


                                'Wastage3 Add default Value LD20160406/YYYYMMDD - handle Blank Value - Value Required is Integer for Wastage1
                            ElseIf TextFileTable.Columns(ColumnCount).ColumnName.ToLower() = "wastage3" Then
                                If CurrentRow(ColumnCount).ToString = "" Then Row(ColumnCount) = "0"


                                'Wastage4 Add default Value LD20160406/YYYYMMDD - handle Blank Value - Value Required is Integer for Wastage1
                            ElseIf TextFileTable.Columns(ColumnCount).ColumnName.ToLower() = "wastage4" Then
                                If CurrentRow(ColumnCount).ToString = "" Then Row(ColumnCount) = "0"
                            Else
                            End If

                            '//LD END
                            ' ''Price1 Add default Value LD20160406/YYYYMMDD - handle Blank Value - Value Required is Integer for Wastage1
                            'If TextFileTable.Columns(ColumnCount).ColumnName = "Price1" Then
                            '    If CurrentRow(ColumnCount).ToString <> "" Then
                            '        Try
                            '            If strThousandSeparator = "," Then
                            '                Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), System.Globalization.CultureInfo.GetCultureInfo("en-US")).ToString("####0.000")
                            '            ElseIf strThousandSeparator = "." Then
                            '                Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), NumberStyles.None, CultureInfo.GetCultureInfo("it-IT")).ToString("####0.000")
                            '            Else
                            '                Row(ColumnCount) = Double.Parse(CurrentRow(ColumnCount).ToString().Replace(" ", ""), System.Globalization.CultureInfo.InvariantCulture).ToString("####0.000")
                            '            End If
                            '        Catch ex As Exception
                            '        Finally

                            '        End Try
                            '        Row(ColumnCount) = Double.Parse(Row(ColumnCount).Replace(" ", "")).ToString("####0.000")

                            '        If Row(ColumnCount).ToString().Substring(Row(ColumnCount).ToString().Length - 4, 1) = "," Then
                            '            Row(ColumnCount) = Row(ColumnCount).ToString().Substring(0, Row(ColumnCount).ToString().Length - 4) & "." & Row(ColumnCount).ToString().Substring(Row(ColumnCount).ToString().Length - 3, 3)
                            '        End If
                            '    End If
                            'End If



                            ''Wastage1 Add default Value LD20160406/YYYYMMDD - handle Blank Value - Value Required is Integer for Wastage1
                            'If TextFileTable.Columns(ColumnCount).ColumnName = "Wastage1" Then
                            '    If CurrentRow(ColumnCount).ToString = "" Then Row(ColumnCount) = "0"
                            'End If

                            ''Wastage2 Add default Value LD20160406/YYYYMMDD - handle Blank Value - Value Required is Integer for Wastage1
                            'If TextFileTable.Columns(ColumnCount).ColumnName = "Wastage2" Then
                            '    If CurrentRow(ColumnCount).ToString = "" Then Row(ColumnCount) = "0"
                            'End If

                            ''Wastage3 Add default Value LD20160406/YYYYMMDD - handle Blank Value - Value Required is Integer for Wastage1
                            'If TextFileTable.Columns(ColumnCount).ColumnName = "Wastage3" Then
                            '    If CurrentRow(ColumnCount).ToString = "" Then Row(ColumnCount) = "0"
                            'End If

                            ''Wastage4 Add default Value LD20160406/YYYYMMDD - handle Blank Value - Value Required is Integer for Wastage1
                            'If TextFileTable.Columns(ColumnCount).ColumnName = "Wastage4" Then
                            '    If CurrentRow(ColumnCount).ToString = "" Then Row(ColumnCount) = "0"
                            'End If


                        Next
                        TextFileTable.Rows.Add(Row)
                    End If

                End If
            Catch ex As  _
            Microsoft.VisualBasic.FileIO.MalformedLineException
                MsgBox("Line " & ex.Message & _
               "is not valid and will be skipped.")
            End Try
        End While

        TextFileReader.Dispose()

        Return TextFileTable

        'Me.DataGridView1.DataSource = TextFileTable

        'lblStatus.Text = "Scan complete. Number of Row(s): " & DataGridView1.Rows.Count.ToString
    End Function

    Public Function fctGetSCANADataSource(ByVal strFilePath As String) As DataTable
        Dim cImportScana As New EgsImportSupplierNetwork.clsMain
        Dim dt As DataTable
        dt = cImportScana.fctGetScanaDataSource(strFilePath, True)
        Return dt

        ''Dim dt As DataTable
        ''Dim fileCSV As New StreamReader(strFilePath, Encoding.Default)
        ''Dim objReadCSV As New EGSReadCSV.CsvDataReader(fileCSV.BaseStream, CChar(strDelimiter), CChar(eTab), CChar(eCRLF))
        ''dt = objReadCSV.Table

        ''EditColNameSCANA(dt)

        ''Dim strLocaleDecimalSepar As String = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator
        ''Dim strLocaleThousandSepar As String = NumberFormatInfo.CurrentInfo.NumberGroupSeparator

        ' ''--------- Replace Seperator ---------------
        ''If strDelimiter = ";" And strDecimalSeparator <> strLocaleDecimalSepar Then
        ''    For Each row As DataRow In dt.Rows
        ''        If IsDBNull(row.Item("Price2")) = False Then row.Item("Price2") = Replace(CStrDB(row.Item("Price2")), strDecimalSeparator, strLocaleDecimalSepar) : Replace(CStrDB(row.Item("Price2")), strThousandSeparator, strLocaleDecimalSepar)
        ''        If IsDBNull(row.Item("Ratio2")) = False Then row.Item("Ratio2") = Replace(CStrDB(row.Item("Ratio2")), strDecimalSeparator, strLocaleDecimalSepar) : Replace(CStrDB(row.Item("Ratio2")), strThousandSeparator, strLocaleDecimalSepar)
        ''        row("Unit1") = Replace(CStr(row("Unit1")).Trim, """", "")
        ''        row("Unit2") = Replace(CStr(row("Unit2")).Trim, """", "")
        ''        row("Price2") = CDblDB(row("Price2"))
        ''        row("Ratio2") = CDblDB(row("Ratio2"))
        ''    Next
        ''End If

        ' ''------------ Formatting --------------------
        ''If intImportType = enumImportType.Scana Then
        ''    subFormatScanaData(dt)
        ''End If


    End Function

    Public Function fctGetPISTORDataSource(ByVal strFilePath As String) As DataTable
        Dim cImportScana As New EgsImportSupplierNetwork.clsMain
        Dim dt As DataTable
        dt = cImportScana.fctGetPistorDataSource(strFilePath, True, L_strCnn)
        Return dt
    End Function

    'mrc 04.20.2010
    Public Function fctGetPISTORDataSource2(ByVal strFilePath As String) As DataTable
        Dim cImportScana As New EgsImportSupplierNetwork.clsMain
        Dim dt As DataTable
        dt = cImportScana.fctGetPistorDataSource2(strFilePath, True)
        Return dt
    End Function

    Public Function fctBulkImportDataSource(ByRef intIDMain As Integer, ByVal dt As DataTable, ByVal intCodeSupplierGroup As Integer, ByVal flagCompareByName As Boolean, _
                                            ByVal strFileName As String, ByVal intCodeSite As Integer, ByVal intCodeUser As Integer, _
                                            ByVal intCodeTrans As Integer, ByVal intCodeSetPrice As Integer, _
                                            Optional ByVal blnAddRecord As Boolean = True, Optional ByVal blnUpdateRecord As Boolean = True, _
                                            Optional ByVal blnGlobal As Boolean = False, Optional ByVal AutomaticImport As Boolean = False, _
                                            Optional ByVal dtFiledate As DateTime = Nothing, Optional ByVal flagCompareByCode As Boolean = False, _
                                            Optional ByVal strSharedSites As String = "", _
                                            Optional ByVal strSharedProperty As String = "", _
                                            Optional ByVal strSharedUsers As String = "", _
                                            Optional ByVal bIsSupplierNetwork As Boolean = False, _
                                            Optional ByVal flagCompareByNumberAndSupplier As Boolean = False, _
                                            Optional ByVal intDefaultSupplier As Integer = 0 _
                                            ) As String

        Dim strError As String = ""
        'Dim intIDMain As Integer = 0
        '--------- Save Import Main Info and Options -----------------
        Dim flagOK As Boolean
        Dim arryNutrientHeaders As New ArrayList
        Dim strNutrientName As String = ""
        Dim strNutrient() As String
        Dim arryNutrientColIndex As New ArrayList
        Dim i As Integer = 0

        '---------- Get Nutrient Headers -------------
        For Each col As DataColumn In dt.Columns
            If Mid(col.ColumnName, 1, 2) = "N=" Then
                strNutrient = Split(col.ColumnName, "=")
                strNutrientName = ""
                If UBound(strNutrient) = 1 Then
                    strNutrientName = strNutrient(1)
                ElseIf UBound(strNutrient) = 2 Then
                    strNutrientName = strNutrient(2)
                End If
                arryNutrientHeaders.Add(strNutrientName)
                arryNutrientColIndex.Add(col.ColumnName)
            ElseIf Mid(col.ColumnName, 1, 2) = "N|" Then
                strNutrient = Split(col.ColumnName, "|")
                strNutrientName = ""
                If UBound(strNutrient) = 2 Then
                    strNutrientName = strNutrient(2)
                End If
                arryNutrientHeaders.Add(strNutrientName)
                arryNutrientColIndex.Add(col.ColumnName)
            End If
            i += 1
        Next

        'DLS
        If intCodeSupplierGroup = enumImportType.PistorFile Then
            arryNutrientColIndex.Clear()
            arryNutrientHeaders.Clear()
            arryNutrientHeaders.Add("Energie")
            arryNutrientHeaders.Add("Eiweiss")
            arryNutrientHeaders.Add("KH")
            arryNutrientHeaders.Add("Fett")
        End If

        'Allergen
        Dim arryAllergenHeaders As New ArrayList
        Dim strAllergenName As String = ""
        Dim strAllergen() As String
        Dim arryAllergenColIndex As New ArrayList
        For Each col As DataColumn In dt.Columns


            If Mid(col.ColumnName, 1, 2) = "A=" Then
                strAllergen = Split(col.ColumnName, "=")
                strAllergenName = ""
                If UBound(strAllergen) = 1 Then
                    strAllergenName = strAllergen(1)
                End If
                arryAllergenHeaders.Add(strAllergenName)
                arryAllergenColIndex.Add(col.ColumnName)
            ElseIf Mid(col.ColumnName, 1, 2) = "N|" Then
                strAllergen = Split(col.ColumnName, "|")
                strAllergenName = ""
                If UBound(strAllergen) = 1 Then
                    strAllergenName = strAllergen(1)
                End If
                arryAllergenHeaders.Add(strAllergenName)
                arryAllergenColIndex.Add(col.ColumnName)
            End If
            i += 1
        Next

        'LD20170510 Add Name Translation
        Dim arryTranslationHeaders As New ArrayList
        Dim strTranslationName As String = ""
        Dim strTranslation() As String
        Dim arryTranslationColIndex As New ArrayList
        For Each col As DataColumn In dt.Columns

            If Mid(col.ColumnName, 1, 5) = "Name=" Then
                strTranslation = Split(col.ColumnName, "=")
                strTranslationName = ""
                If UBound(strTranslation) = 1 Then
                    strTranslationName = strTranslation(1)
                End If
                arryTranslationHeaders.Add(strTranslationName)
                arryTranslationColIndex.Add(col.ColumnName)
            ElseIf Mid(col.ColumnName, 1, 5) = "Name|" Then
                strTranslation = Split(col.ColumnName, "|")
                strTranslationName = ""
                If UBound(strTranslation) = 1 Then
                    strTranslationName = strTranslation(1)
                End If
                arryTranslationHeaders.Add(strTranslationName)
                arryTranslationColIndex.Add(col.ColumnName)
            End If
            i += 1
        Next



        flagOK = BULKImportMain(intIDMain, strFileName, flagCompareByName, blnAddRecord, blnUpdateRecord, intCodeSite, intCodeUser, blnGlobal, intCodeSupplierGroup, intCodeTrans, intCodeSetPrice, dt.Rows.Count, arryNutrientHeaders, AutomaticImport, dtFiledate, flagCompareByCode, strSharedSites, strSharedProperty, strSharedUsers, bIsSupplierNetwork, flagCompareByNumberAndSupplier, intDefaultSupplier)

        If flagOK And arryTranslationHeaders.Count > 0 Then
            BULKImportMainTranslation(intIDMain, arryTranslationHeaders)
        End If

        Dim IsCodeExist As Boolean = False

        For Each dc As DataColumn In dt.Columns
            dc.ReadOnly = False
            If dc.ColumnName.ToLower() = "code" Then
                IsCodeExist = True
            End If
        Next

        If IsCodeExist = True Then
            For Each dr As DataRow In dt.Rows

                If IsDBNull(dr("Code")) Then
                    dr("Code") = "-9"
                Else

                    If String.IsNullOrEmpty(dr("Code")) Then
                        dr("Code") = "-9"
                    End If
                End If

            Next
        End If



        If flagOK Then
            dt.Columns.Add("IdMain")
            For Each row As DataRow In dt.Rows
                row.Item("IdMain") = intIDMain
            Next
            If intCodeSupplierGroup = enumImportType.AutogrillFormat Then
                strError = fctBulkImportAutogrill(dt)
            Else
                strError = fctBulkImportStandardCSV(dt, arryNutrientColIndex, arryAllergenColIndex, arryTranslationColIndex)
            End If
        Else
            strError = "not imported"
        End If
        Return strError
    End Function

    Public Function BULKImportMain(ByRef intIDMain As Integer, ByVal strFileName As String, ByVal flagCompareByName As Boolean, ByVal flagAddRecord As Boolean, ByVal flagUpdateRecord As Boolean, _
                                      ByVal intCodeSite As Integer, ByVal intCodeUser As Integer, ByVal flagIsGlobal As Boolean, ByVal intCodeSupplierGroup As Integer, ByVal intCodeTrans As Integer, ByVal intCodeSetPrice As Integer, _
                                      ByVal intTotalRecord As Integer, ByVal arryNutrientHeaders As ArrayList, Optional ByVal AutomaticImport As Boolean = False, Optional ByVal dtFiledate As DateTime = Nothing, _
                                      Optional ByVal flagCompareByCode As Boolean = False, _
                                       Optional ByVal strSharedSites As String = "", _
                                        Optional ByVal strSharedProperty As String = "", _
                                        Optional ByVal strSharedUsers As String = "", _
                                        Optional ByVal bIsSupplierNetwork As Boolean = False, _
                                        Optional ByVal flagCompareByNumberAndSupplier As Boolean = False, _
                                        Optional ByVal intDefaultSupplier As Integer = 0) As Boolean
        Dim cmd As New SqlCommand
        Try

            If flagCompareByCode Then
                flagCompareByName = True
            End If

            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "BULK_ImportMain"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@FileName", SqlDbType.NVarChar, 100).Value = strFileName
                .Parameters.Add("@CompareByName", SqlDbType.Bit).Value = flagCompareByName
                .Parameters.Add("@AddNewRecord", SqlDbType.Bit).Value = flagAddRecord
                .Parameters.Add("@UpdateRecord", SqlDbType.Bit).Value = flagUpdateRecord
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = flagIsGlobal
                .Parameters.Add("@CodeSupplierGroup", SqlDbType.Int).Value = intCodeSupplierGroup
                .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice
                .Parameters.Add("@TotalRecord", SqlDbType.Int).Value = intTotalRecord
                .Parameters.Add("@AutomaticImport", SqlDbType.Bit).Value = AutomaticImport

                If AutomaticImport Then
                    .Parameters.Add("@FileDate", SqlDbType.DateTime).Value = dtFiledate
                End If


                '--------- Nutrient Headers ------------
                Dim i As Integer
                Dim strParameterName As String
                For i = 0 To arryNutrientHeaders.Count - 1

                    If i > 14 Then Exit For
                    strParameterName = "@N" & (i + 1)
                    .Parameters.Add(strParameterName, SqlDbType.NVarChar, 50).Value = arryNutrientHeaders(i).ToString
                Next

                .Parameters.Add("@CompareByCode", SqlDbType.Bit).Value = flagCompareByCode
                .Parameters.Add("@IDMain", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@SharedSites", SqlDbType.VarChar).Value = strSharedSites
                .Parameters.Add("@SharedProperty", SqlDbType.VarChar).Value = strSharedProperty
                .Parameters.Add("@SharedUsers", SqlDbType.VarChar).Value = strSharedUsers
                .Parameters.Add("@IsSupplierNetwork", SqlDbType.Bit).Value = bIsSupplierNetwork

                .Parameters.Add("@CompareBySupplier", SqlDbType.Bit).Value = flagCompareByNumberAndSupplier
                .Parameters.Add("@DefaultSupplier", SqlDbType.Int).Value = intDefaultSupplier

                .Connection.Open()
                .ExecuteNonQuery()
                cmd.Connection.Close()
                intIDMain = CInt(.Parameters("@IDMain").Value)
                If intIDMain = -1 Then
                    Return False
                Else
                    Return True
                End If

            End With
        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Public Function BULKImportDetails(ByVal intIDMain As Integer) As Boolean
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandTimeout = 900000
                .CommandText = "BULK_ImportDetails"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@IDMain", SqlDbType.Int).Value = intIDMain
                .Connection.Open()
                .ExecuteNonQuery()
                cmd.Connection.Close()
                Return True
            End With
        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            'Throw New Exception(ex.Message, ex)
            Return False
        End Try
    End Function

    Public Function fctBulkImportStandardCSV(ByVal dt As DataTable, ByVal arryNutrientColIndex As ArrayList, ByVal arryAllergenColIndex As ArrayList, Optional ByVal arryTranslationColIndex As ArrayList = Nothing) As String
        Dim strError As String = ""

        Try
            EditColName(dt) 'VRP 13.01.2009

            '------------- Bulk Importation -----------------
            'Copy all data to TempTable using SqlBulkCopy
            Dim bulkCopy As SqlBulkCopy
            bulkCopy = New SqlBulkCopy(L_strCnn)
            bulkCopy.DestinationTableName = "EgswBulkImportProducts"


            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IdMain", "IdMain")))
            If dt.Columns.Contains("Number") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Number", "Number")))
            End If
            If dt.Columns.Contains("Name") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Name", "Name")))
            End If

            If dt.Columns.Contains("Unit1") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Unit1", "Unit1")))
            End If

            If dt.Columns.Contains("Unit2") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Unit2", "Unit2")))
            End If

            If dt.Columns.Contains("Unit3") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Unit3", "Unit3")))
            End If

            If dt.Columns.Contains("Unit4") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Unit4", "Unit4")))
            End If

            If dt.Columns.Contains("Price1") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Price1", "Price1")))
            End If

            If dt.Columns.Contains("Price2") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Price2", "Price2")))
            End If

            If dt.Columns.Contains("Price3") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Price3", "Price3")))
            End If

            If dt.Columns.Contains("Price4") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Price4", "Price4")))
            End If

            If dt.Columns.Contains("Ratio1") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Ratio1", "Ratio1")))
            End If

            If dt.Columns.Contains("Ratio2") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Ratio2", "Ratio2")))
            End If

            If dt.Columns.Contains("Ratio3") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Ratio3", "Ratio3")))
            End If

            If dt.Columns.Contains("Supplier") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Supplier", "Supplier")))
            End If

            If dt.Columns.Contains("Category") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Category", "Category")))
            End If

            If dt.Columns.Contains("Tax") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Tax", "Tax")))
            End If

            If dt.Columns.Contains("Description") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Description", "Description")))
            End If

            If dt.Columns.Contains("Ingredients") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Ingredients", "Ingredients")))
            End If

            If dt.Columns.Contains("Preparation") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Preparation", "Preparation")))
            End If

            If dt.Columns.Contains("CookingTip") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("CookingTip", "CookingTip")))
            End If

            If dt.Columns.Contains("Refinement") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Refinement", "Refinement")))
            End If

            If dt.Columns.Contains("Storage") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Storage", "Storage")))
            End If

            If dt.Columns.Contains("Productivity") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Productivity", "Productivity")))
            End If

            If dt.Columns.Contains("Picture") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Picture", "Picture")))
            End If

            If dt.Columns.Contains("Wastage") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Wastage", "Wastage")))
            End If

            If dt.Columns.Contains("Wastage1") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Wastage1", "Wastage")))
            End If

            If dt.Columns.Contains("Wastage2") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Wastage2", "Wastage2")))
            End If

            If dt.Columns.Contains("Wastage3") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Wastage3", "Wastage3")))
            End If

            If dt.Columns.Contains("Wastage4") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Wastage4", "Wastage4")))
            End If

            If dt.Columns.Contains("Code") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Code", "CodeFile")))
            End If

            If dt.Columns.Contains("CodeTrans2") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("CodeTrans2", "CodeTrans2")))
            End If


            If dt.Columns.Contains("Name2") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Name2", "Name2")))
            End If


            If dt.Columns.Contains("Currency") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Currency", "Currency"))) 'LD20161018 Add Currency
            End If

            ' RBAJ-2012.09.18 [CMC-1262]
            If dt.Columns.Contains("UPC") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("UPC", "UPC")))
            End If

            If dt.Columns.Contains("Brand") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Brand", "Brand")))
            End If

            If dt.Columns.Contains("SpecificDetermination") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("SpecificDetermination", "SpecificDetermination"))) 'LD20161018 Add Specific Determination
            End If

            If dt.Columns.Contains("Keyword") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Keyword", "Keyword"))) 'LD20161018 Add Keyword
            End If
            If dt.Columns.Contains("Allegens") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Allegens", "Allegens"))) 'LD20161018 Add Allegens
            End If

            '---------- Get Nutrient Headers -------------
            Dim ix As Integer
            Dim strColDest As String
            Dim strColSource As String
            Dim strNDest As String
            For ix = 0 To arryNutrientColIndex.Count - 1
                strColSource = CStrDB(arryNutrientColIndex(ix))
                strColDest = "NutrVal" & (ix + 1)
                strNDest = "N" & (ix + 1)
                If dt.Columns.Contains(strColSource) Then
                    bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping(strColSource, strColDest)))
                    bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping(strColSource, strNDest)))

                End If

            Next

            '---------- Get Allergen Headers -------------
            For ix = 0 To arryAllergenColIndex.Count - 1
                If ix = 22 Then
                    Exit For
                End If
                strColSource = CStrDB(arryAllergenColIndex(ix))
                strColDest = "Allergen" & (ix + 1)
                If dt.Columns.Contains(strColSource) Then
                    bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping(strColSource, strColDest)))
                End If
            Next

            '---------Get Name Translation Header --------
            If arryTranslationColIndex IsNot Nothing AndAlso arryTranslationColIndex.Count > 0 Then
                '---------- Get Allergen Headers -------------
                For ix = 0 To arryTranslationColIndex.Count - 1
                    If ix = 7 Then
                        Exit For
                    End If
                    strColSource = CStrDB(arryTranslationColIndex(ix))
                    strColDest = "Translation" & (ix + 1)
                    If dt.Columns.Contains(strColSource) Then
                        bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping(strColSource, strColDest)))
                    End If
                Next
            End If

            'For i As Integer = 1 To 15
            '    strColSource = "NutrVal" & i
            '    strColDest = "NutrVal" & i
            '    If dt.Columns.Contains(strColSource) Then
            '        bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping(strColSource, strColDest)))
            '    End If
            'Next

            ''bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Price1", "Price1")))
            ''bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Price2", "Price2")))
            ''bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Price3", "Price3")))

            bulkCopy.WriteToServer(dt)
            bulkCopy.Close()

        Catch ex As Exception
            strError = ex.Message
        End Try




        Return strError
    End Function

    Public Function fctBulkImportPistorTable(ByRef intIDMain As Integer, ByVal dt As DataTable, ByVal strFileName As String, _
                                            ByVal udtUser As structUser, _
                                            Optional ByVal flagCompareByName As Boolean = True, _
                                            Optional ByVal blnAddRecord As Boolean = True, _
                                            Optional ByVal blnUpdateRecord As Boolean = True, _
                                            Optional ByVal blnGlobal As Boolean = False, Optional ByVal AutomaticImport As Boolean = False, _
                                            Optional ByVal dtFiledate As DateTime = Nothing, Optional ByVal flagCompareByCode As Boolean = False, _
                                            Optional ByVal strSharedSites As String = "", _
                                            Optional ByVal strSharedProperty As String = "", _
                                            Optional ByVal strSharedUsers As String = "", _
                                            Optional ByVal bIsSupplierNetwork As Boolean = False, _
                                            Optional ByVal flagCompareByNumberAndSupplier As Boolean = False, _
                                            Optional ByVal intDefaultSupplier As Integer = 0 _
                                            ) As String
        Dim strError As String = ""

        Try

            Dim arryNutrientColIndex As New ArrayList
            Dim arryNutrientHeaders As New ArrayList
            'If intCodeSupplierGroup = enumImportType.PistorFile Then
            arryNutrientColIndex.Clear()
            arryNutrientHeaders.Clear()
            arryNutrientHeaders.Add("Energie")
            arryNutrientHeaders.Add("Eiweiss")
            arryNutrientHeaders.Add("KH")
            arryNutrientHeaders.Add("Fett")
            'End If

            BULKImportMain(intIDMain, strFileName, flagCompareByName, blnAddRecord, blnUpdateRecord, udtUser.Site.Code, udtUser.Code, blnGlobal, enumImportType.PistorFile, udtUser.CodeTrans, udtUser.LastSetPrice, dt.Rows.Count, arryNutrientHeaders, AutomaticImport, dtFiledate, flagCompareByCode, strSharedSites, strSharedProperty, strSharedUsers, bIsSupplierNetwork, flagCompareByNumberAndSupplier, intDefaultSupplier)

            While dt.Columns.Count < 50
                dt.Columns.Add("Column" & Format(dt.Columns.Count + 1, "000"))
            End While
            'EditColName(dt) 'VRP 13.01.2009
            dt.Columns.Add("IDMain")
            For Each dtRow As DataRow In dt.Rows
                dtRow("IDMain") = intIDMain
            Next
            dt.AcceptChanges()
            '------------- Bulk Importation -----------------
            'Copy all data to TempTable using SqlBulkCopy
            Dim bulkCopy As SqlBulkCopy
            bulkCopy = New SqlBulkCopy(L_strCnn)
            bulkCopy.DestinationTableName = "EgswBulkImportPistordaten"

            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IDMain", "IDMain")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column001", "Column001")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column002", "Column002")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column003", "Column003")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column004", "Column004")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column005", "Column005")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column006", "Column006")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column007", "Column007")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column008", "Column008")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column009", "Column009")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column010", "Column010")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column011", "Column011")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column012", "Column012")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column013", "Column013")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column014", "Column014")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column015", "Column015")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column016", "Column016")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column017", "Column017")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column018", "Column018")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column019", "Column019")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column020", "Column020")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column021", "Column021")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column022", "Column022")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column023", "Column023")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column024", "Column024")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column025", "Column025")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column026", "Column026")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column027", "Column027")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column028", "Column028")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column029", "Column029")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column030", "Column030")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column031", "Column031")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column032", "Column032")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column033", "Column033")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column034", "Column034")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column035", "Column035")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column036", "Column036")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column037", "Column037")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column038", "Column038")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column039", "Column039")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column040", "Column040")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column041", "Column041")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column042", "Column042")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column043", "Column043")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column044", "Column044")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column045", "Column045")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column046", "Column046")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column047", "Column047")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column048", "Column048")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column049", "Column049")))
            bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Column050", "Column050")))

            ''bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Price1", "Price1")))
            ''bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Price2", "Price2")))
            ''bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Price3", "Price3")))

            bulkCopy.WriteToServer(dt)
            bulkCopy.Close()


            'Dim t As New Threading.Thread(AddressOf subImportPistor)
            't.Start()
            'Dim cMain As New clsMain
            'cMain.fctArrangeColumnsPistor(intIDMain, L_strCnn)
        Catch ex As Exception
            strError = ex.Message
        End Try




        Return strError
    End Function
    'Private Sub subImportPistor()
    '    Dim cMain As New clsMain
    '    cMain.fctArrangeColumnsPistor(intIDMain, L_strCnn)
    'End Sub

    'JRN 06.09.2010
    Public Sub fctArrangeColumnsPistor(ByVal intIDMain As Integer, ByVal L_strCnn As String)
        Dim sqlCon As New SqlClient.SqlConnection(L_strCnn)
        Dim sqlCmd As New SqlClient.SqlCommand
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter
        Dim lngLastId As Long

        With sqlCmd
            .Connection = sqlCon
            .Connection.Open()
            .Parameters.Clear()
            .CommandTimeout = 0
            .CommandText = "[BULK_ImportPistorReorderColumns]"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intIDMain", SqlDbType.Int).Value = intIDMain
            .ExecuteNonQuery()
            'With da
            '    .SelectCommand = sqlCmd
            '    dt.BeginLoadData()
            '    .Fill(dt)
            '    dt.EndLoadData()
            'End With
        End With

        sqlCon.Dispose()
        sqlCmd.Dispose()


    End Sub


    Private Function CheckColName(ByVal ColName As String) As String 'VRP 13.01.2009

        'LD20161017 make all the case to lower case to accept case sensitivity letter

        Dim TranslationCode As New System.Collections.Generic.List(Of Integer)
        TranslationCode.AddRange(New Integer() {1, 2, 3, 4, 9})

        'For i As Integer = 1 To 4
        For i As Integer = 1 To TranslationCode.Count
            Dim cLang As New clsEGSLanguage(TranslationCode(i - 1))
            Select Case ColName.ToLower()
                Case cLang.GetString(clsEGSLanguage.CodeType.Number).ToLower()
                    CheckColName = "Number"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Name).ToLower()
                    CheckColName = "Name"
                    Exit For

                Case cLang.GetString(clsEGSLanguage.CodeType.Name).ToLower() & 2
                    CheckColName = "Name2"
                    Exit For

                Case cLang.GetString(clsEGSLanguage.CodeType.Price).ToLower() & 1
                    CheckColName = "Price1"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Price).ToLower() & 2
                    CheckColName = "Price2"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Price).ToLower() & 3
                    CheckColName = "Price3"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Price).ToLower() & 4
                    CheckColName = "Price4"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Unit).ToLower() & 1
                    CheckColName = "Unit1"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Unit).ToLower() & 2
                    CheckColName = "Unit2"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Unit).ToLower() & 3
                    CheckColName = "Unit3"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Unit).ToLower() & 4
                    CheckColName = "Unit4"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Ratio).ToLower() & 1
                    CheckColName = "Ratio1"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Ratio).ToLower() & 2
                    CheckColName = "Ratio2"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Ratio).ToLower() & 3
                    CheckColName = "Ratio3"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Supplier).ToLower()
                    CheckColName = "Supplier"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Category).ToLower()
                    CheckColName = "Category"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Tax).ToLower()
                    CheckColName = "Tax"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Description).ToLower()
                    CheckColName = "Description"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Ingredients).ToLower()
                    CheckColName = "Ingredients"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Preparation).ToLower()
                    CheckColName = "Preparation"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.CookingTip).ToLower()

                    CheckColName = "CookingTip"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.FeedBack).ToLower()
                    CheckColName = "CookingTip"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Refinement).ToLower()
                    CheckColName = "Refinement"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Degustation_Development).ToLower()
                    CheckColName = "Refinement"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Storage).ToLower()
                    CheckColName = "Storage"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Picture).ToLower()
                    CheckColName = "Picture"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Productivity).ToLower()
                    CheckColName = "Productivity"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Remark).ToLower()
                    CheckColName = "Productivity"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Brand).ToLower() & 1
                    CheckColName = "Brand"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.Brand).ToLower()
                    CheckColName = "Brand"
                    Exit For

                Case cLang.GetString(clsEGSLanguage.CodeType.Wastage).ToLower()
                    CheckColName = "Wastage"
                    Exit For

                Case cLang.GetString(clsEGSLanguage.CodeType.Wastage).ToLower() & 1
                    CheckColName = "Wastage1"
                    Exit For

                Case cLang.GetString(clsEGSLanguage.CodeType.Wastage).ToLower() & 2
                    CheckColName = "Wastage2"
                    Exit For

                Case cLang.GetString(clsEGSLanguage.CodeType.Wastage).ToLower() & 3
                    CheckColName = "Wastage3"
                    Exit For

                Case cLang.GetString(clsEGSLanguage.CodeType.Wastage).ToLower() & 4
                    CheckColName = "Wastage4"
                    Exit For
                Case cLang.GetString(clsEGSLanguage.CodeType.SpecificDetermination).ToLower() 'LD20161017 add specific determination and make it to lower case to accept case sensitivity
                    CheckColName = "SpecificDetermination"
                    Exit For

                Case cLang.GetString(clsEGSLanguage.CodeType.Keyword).ToLower() 'LD20161017 add specific Keyword and make it to lower case to accept case sensitivity
                    CheckColName = "Keyword"
                    Exit For

                Case cLang.GetString(clsEGSLanguage.CodeType.Keywords).ToLower() 'LD20161017 add specific Keyword and make it to lower case to accept case sensitivity
                    CheckColName = "Keyword"
                    Exit For

                Case cLang.GetString(clsEGSLanguage.CodeType.Currency).ToLower() 'LD20161017 add Currency  and make it to lower case to accept case sensitivity
                    CheckColName = "Currency"
                    Exit For
                Case Else
                    CheckColName = ColName

            End Select
        Next

    End Function

    Private Sub EditColName(ByVal dt As DataTable) 'VRP 13.01.2009

        'LD20161017 make all the case to lower case to accept case sensitivity letter
        Dim TranslationCode As New System.Collections.Generic.List(Of Integer)
        TranslationCode.AddRange(New Integer() {1, 2, 3, 4, 9})
        For Each col As DataColumn In dt.Columns
            'For i As Integer = 1 To 4
            For i As Integer = 1 To TranslationCode.Count
                Dim cLang As New clsEGSLanguage(TranslationCode(i - 1))
                Select Case col.ColumnName.ToLower()
                    Case cLang.GetString(clsEGSLanguage.CodeType.Number).ToLower()
                        col.ColumnName = "Number"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Name).ToLower()
                        col.ColumnName = "Name"
                        Exit For

                    Case cLang.GetString(clsEGSLanguage.CodeType.Name).ToLower() & 2
                        col.ColumnName = "Name2"
                        Exit For

                    Case cLang.GetString(clsEGSLanguage.CodeType.Price).ToLower() & 1
                        col.ColumnName = "Price1"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Price).ToLower() & 2
                        col.ColumnName = "Price2"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Price).ToLower() & 3
                        col.ColumnName = "Price3"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Price).ToLower() & 4
                        col.ColumnName = "Price4"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Unit).ToLower() & 1
                        col.ColumnName = "Unit1"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Unit).ToLower() & 2
                        col.ColumnName = "Unit2"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Unit).ToLower() & 3
                        col.ColumnName = "Unit3"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Unit).ToLower() & 4
                        col.ColumnName = "Unit4"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Ratio).ToLower() & 1
                        col.ColumnName = "Ratio1"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Ratio).ToLower() & 2
                        col.ColumnName = "Ratio2"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Ratio).ToLower() & 3
                        col.ColumnName = "Ratio3"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Supplier).ToLower()
                        col.ColumnName = "Supplier"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Category).ToLower()
                        col.ColumnName = "Category"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Tax).ToLower()
                        col.ColumnName = "Tax"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Description).ToLower()
                        col.ColumnName = "Description"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Ingredients).ToLower()
                        col.ColumnName = "Ingredients"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Preparation).ToLower()
                        col.ColumnName = "Preparation"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.CookingTip).ToLower()

                        col.ColumnName = "CookingTip"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.FeedBack).ToLower()
                        col.ColumnName = "CookingTip"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Refinement).ToLower()
                        col.ColumnName = "Refinement"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Degustation_Development).ToLower()
                        col.ColumnName = "Refinement"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Storage).ToLower()
                        col.ColumnName = "Storage"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Picture).ToLower()
                        col.ColumnName = "Picture"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Productivity).ToLower()
                        col.ColumnName = "Productivity"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Remark).ToLower()
                        col.ColumnName = "Productivity"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Brand).ToLower() & 1
                        col.ColumnName = "Brand"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.Brand).ToLower()
                        col.ColumnName = "Brand"
                        Exit For

                    Case cLang.GetString(clsEGSLanguage.CodeType.Wastage).ToLower()
                        col.ColumnName = "Wastage"
                        Exit For

                    Case cLang.GetString(clsEGSLanguage.CodeType.Wastage).ToLower() & 1
                        col.ColumnName = "Wastage1"
                        Exit For

                    Case cLang.GetString(clsEGSLanguage.CodeType.Wastage).ToLower() & 2
                        col.ColumnName = "Wastage2"
                        Exit For

                    Case cLang.GetString(clsEGSLanguage.CodeType.Wastage).ToLower() & 3
                        col.ColumnName = "Wastage3"
                        Exit For

                    Case cLang.GetString(clsEGSLanguage.CodeType.Wastage).ToLower() & 4
                        col.ColumnName = "Wastage4"
                        Exit For
                    Case cLang.GetString(clsEGSLanguage.CodeType.SpecificDetermination).ToLower() 'LD20161017 add specific determination and make it to lower case to accept case sensitivity
                        col.ColumnName = "SpecificDetermination"
                        Exit For

                    Case cLang.GetString(clsEGSLanguage.CodeType.Keyword).ToLower() 'LD20161017 add specific Keyword and make it to lower case to accept case sensitivity
                        col.ColumnName = "Keyword"
                        Exit For

                    Case cLang.GetString(clsEGSLanguage.CodeType.Keywords).ToLower() 'LD20161017 add specific Keyword and make it to lower case to accept case sensitivity
                        col.ColumnName = "Keyword"
                        Exit For

                    Case cLang.GetString(clsEGSLanguage.CodeType.Currency).ToLower() 'LD20161017 add Currency  and make it to lower case to accept case sensitivity
                        col.ColumnName = "Currency"
                        Exit For


                End Select
            Next
        Next
    End Sub

    Private Sub EditColNameSCANA(ByVal dt As DataTable) 'VRP 02.03.2009
        Dim i As Integer = 1
        For Each col As DataColumn In dt.Columns
            Select Case i
                Case 2
                    col.ColumnName = "Supplier"
                Case 3
                    col.ColumnName = "Number"
                Case 4
                    col.ColumnName = "Name"
                Case 18
                    col.ColumnName = "Category"
                Case 20
                    col.ColumnName = "Unit2"
                Case 22
                    col.ColumnName = "Unit1"
                Case 23
                    col.ColumnName = "Ratio2"
                Case 30
                    col.ColumnName = "Price2"
                Case Else
                    col.ColumnName = "Column" & i
            End Select
            i += 1
        Next
        For count As Integer = 1 To dt.Columns.Count - 1
            If dt.Columns.Contains("Column" & count) Then
                dt.Columns.Remove("Column" & count)
            End If
        Next
    End Sub

    Private Sub subFormatScanaData(ByVal dt As DataTable) 'DLS Aug42009
        Dim colPrice1 As New DataColumn
        With colPrice1
            .DataType = System.Type.GetType("System.Double")
            .ColumnName = "Price1"
            .DefaultValue = 0
            .Expression = "ISNULL(CONVERT(Price2, System.Double), 0) / ISNULL(CONVERT(Ratio2, System.Double),0)"
        End With
        dt.Columns.Add(colPrice1)
    End Sub

    Public Function fctBulkImportPistor(ByVal dt As DataTable) As String
        Dim strError As String = ""

        Try
            '------------- Bulk Importation -----------------
            'Copy all data to TempTable using SqlBulkCopy
            Dim bulkCopy As SqlBulkCopy
            bulkCopy = New SqlBulkCopy(L_strCnn)
            bulkCopy.DestinationTableName = "EgswBulkImportProducts"



            ' Set up the column mappings by name.
            Dim mapID As New SqlBulkCopyColumnMapping("IdMain", "IdMain")
            bulkCopy.ColumnMappings.Add(mapID)

            ' Set up the column mappings by name.
            Dim mapNumber As New SqlBulkCopyColumnMapping("Column001", "Number")
            bulkCopy.ColumnMappings.Add(mapNumber)


            Dim mapName As New SqlBulkCopyColumnMapping("Column003", "Name")
            bulkCopy.ColumnMappings.Add(mapName)

            Dim mapPrice As New SqlBulkCopyColumnMapping("Column009", "Unit1")
            bulkCopy.ColumnMappings.Add(mapPrice)


            Dim mapF6 As New SqlBulkCopyColumnMapping("Column006", "Pistor_F6")
            bulkCopy.ColumnMappings.Add(mapF6)

            Dim mapF8 As New SqlBulkCopyColumnMapping("Column008", "Pistor_F8")
            bulkCopy.ColumnMappings.Add(mapF8)

            Dim mapF9 As New SqlBulkCopyColumnMapping("Column009", "Pistor_F9")
            bulkCopy.ColumnMappings.Add(mapF9)

            Dim mapF10 As New SqlBulkCopyColumnMapping("Column010", "Pistor_F10")
            bulkCopy.ColumnMappings.Add(mapF10)

            Dim mapF12 As New SqlBulkCopyColumnMapping("Column012", "Pistor_F12")
            bulkCopy.ColumnMappings.Add(mapF12)

            Dim mapF16 As New SqlBulkCopyColumnMapping("Column016", "Pistor_F16")
            bulkCopy.ColumnMappings.Add(mapF16)

            Dim mapF20 As New SqlBulkCopyColumnMapping("Column020", "Pistor_F20")
            bulkCopy.ColumnMappings.Add(mapF20)

            Dim mapF11 As New SqlBulkCopyColumnMapping("Column011", "Pistor_F11")
            bulkCopy.ColumnMappings.Add(mapF11)

            Dim mapF14 As New SqlBulkCopyColumnMapping("Column014", "Pistor_F14")
            bulkCopy.ColumnMappings.Add(mapF14)


            bulkCopy.WriteToServer(dt)
            bulkCopy.Close()

        Catch ex As Exception
            strError = ex.Message
        End Try



        Return strError
    End Function

    Public Function fctBulkImportAutogrill(ByVal dt As DataTable) As String
        Dim strError As String = ""

        Try
            '------------- Bulk Importation -----------------
            'Copy all data to TempTable using SqlBulkCopy
            Dim bulkCopy As SqlBulkCopy
            bulkCopy = New SqlBulkCopy(L_strCnn)
            bulkCopy.DestinationTableName = "EgswBulkImportProducts"



            ' Set up the column mappings by name.
            Dim mapID As New SqlBulkCopyColumnMapping("IdMain", "IdMain")
            bulkCopy.ColumnMappings.Add(mapID)

            ' Set up the column mappings by name.
            Dim map1 As New SqlBulkCopyColumnMapping("Column001", "AutoGrill_Keyword")
            bulkCopy.ColumnMappings.Add(map1)

            Dim map2 As New SqlBulkCopyColumnMapping("Column002", "Number")
            bulkCopy.ColumnMappings.Add(map2)

            ''Dim map3 As New SqlBulkCopyColumnMapping("Column003", "")
            ''bulkCopy.ColumnMappings.Add(map3)

            ''Dim map4 As New SqlBulkCopyColumnMapping("Column004", "")
            ''bulkCopy.ColumnMappings.Add(map4)

            Dim map5 As New SqlBulkCopyColumnMapping("Column005", "Supplier")
            bulkCopy.ColumnMappings.Add(map5)

            Dim map6 As New SqlBulkCopyColumnMapping("Column006", "Name")
            bulkCopy.ColumnMappings.Add(map6)

            Dim map7 As New SqlBulkCopyColumnMapping("Column007", "AutoGrill_Name2")
            bulkCopy.ColumnMappings.Add(map7)

            ''Dim map8 As New SqlBulkCopyColumnMapping("Column008", "")
            ''bulkCopy.ColumnMappings.Add(map8)

            Dim map9 As New SqlBulkCopyColumnMapping("Column009", "Category")
            bulkCopy.ColumnMappings.Add(map9)

            Dim map10 As New SqlBulkCopyColumnMapping("Column010", "Unit2")
            bulkCopy.ColumnMappings.Add(map10)

            Dim map11 As New SqlBulkCopyColumnMapping("Column011", "Unit1")
            bulkCopy.ColumnMappings.Add(map11)

            Dim map12 As New SqlBulkCopyColumnMapping("Column012", "Ratio2")
            bulkCopy.ColumnMappings.Add(map12)


            Dim map15 As New SqlBulkCopyColumnMapping("Column015", "AutoGrill_ArtType")
            bulkCopy.ColumnMappings.Add(map15)

            Dim map17 As New SqlBulkCopyColumnMapping("Column017", "Price2")
            bulkCopy.ColumnMappings.Add(map17)

            Dim map18 As New SqlBulkCopyColumnMapping("Column018", "Tax")
            bulkCopy.ColumnMappings.Add(map18)


            bulkCopy.WriteToServer(dt)
            bulkCopy.Close()

        Catch ex As Exception
            strError = ex.Message
        End Try



        Return strError
    End Function

    Public Function fctNullToZeroDBL(ByVal value As Object) As Double
        If value Is Nothing Then
            Return 0
        ElseIf Not IsNumeric(value) Then
            Return 0
        Else
            Return CDbl(value)
        End If
    End Function

    Public Function fctNullToString(ByVal value As Object) As String
        If value Is Nothing Or IsDBNull(value) Then
            Return ""
        ElseIf CStrDB(value) = "&nbsp;" Then
            Return ""
        Else
            Return CStr(value)
        End If
    End Function

    Public Function BULKImportStatus(ByVal intIDMain As Integer, ByRef intTotalRecords As Integer, ByRef intTotalImported As Integer, _
                                        ByRef intTotalErrors As Integer, ByRef intTotalAffected As Integer, ByRef intCalculated As Integer, ByRef bRecalculation As Boolean) As Integer
        Dim cmd As New SqlCommand
        Dim intPercent As Integer = 0
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "Bulk_CheckStatus"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@IDMain", SqlDbType.Int).Value = intIDMain
                .Parameters.Add("@TotalRecord", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@TotalImported", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@TotalErrors", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@ProgressPercent", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@TotalAffected", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@TotalCalculated", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@Recalculation", SqlDbType.Bit).Direction = ParameterDirection.Output


                .Connection.Open()
                .ExecuteNonQuery()
                intTotalRecords = CIntDB(.Parameters("@TotalRecord").Value)
                intTotalImported = CIntDB(.Parameters("@TotalImported").Value)
                intTotalErrors = CIntDB(.Parameters("@TotalErrors").Value)
                intPercent = CIntDB(.Parameters("@ProgressPercent").Value)

                intTotalAffected = CIntDB(.Parameters("@TotalAffected").Value)
                intCalculated = CIntDB(.Parameters("@TotalCalculated").Value)
                bRecalculation = CBoolDB(.Parameters("@Recalculation").Value)

                cmd.Connection.Close()
                Return intPercent
            End With
        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Return 0
            'Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Public Function PRICESImportStatus(ByVal intIDMain As Integer, ByRef intTotalRecords As Integer, ByRef intTotalImported As Integer, _
                                       ByRef intTotalErrors As Integer, ByRef intTotalAffected As Integer, ByRef intCalculated As Integer, ByRef bRecalculation As Boolean) As Integer
        Dim cmd As New SqlCommand
        Dim intPercent As Integer = 0
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "BULK_CHECKSTATUS_PRICES"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@IDMain", SqlDbType.Int).Value = intIDMain
                .Parameters.Add("@TotalRecord", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@TotalImported", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@TotalErrors", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@ProgressPercent", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@TotalAffected", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@TotalCalculated", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@Recalculation", SqlDbType.Bit).Direction = ParameterDirection.Output


                .Connection.Open()
                .ExecuteNonQuery()
                intTotalRecords = CIntDB(.Parameters("@TotalRecord").Value)
                intTotalImported = CIntDB(.Parameters("@TotalImported").Value)
                intTotalErrors = CIntDB(.Parameters("@TotalErrors").Value)
                intPercent = CIntDB(.Parameters("@ProgressPercent").Value)

                intTotalAffected = CIntDB(.Parameters("@TotalAffected").Value)
                intCalculated = CIntDB(.Parameters("@TotalCalculated").Value)
                bRecalculation = CBoolDB(.Parameters("@Recalculation").Value)

                cmd.Connection.Close()
                Return intPercent
            End With
        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Return 0
            'Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Public Function fctGetEgsWBulkImportMain(ByVal intCodeSite As Integer, Optional ByVal intIDMain As Integer = -1) As DataTable
        Dim cmd As New SqlCommand
        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                If intIDMain = -1 Then
                    .CommandText = "SELECT ID, Dates, FileName, AddNewRecord, UpdateRecord, CodeSite, TotalRecord, TotalImported, TotalErrors, Done,CodeMarkGroup, CodeSupplierGroup FROM EgsWBulkImportMain WHERE CodeSite=@CodeSite ORDER BY Dates DESC"
                    .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                Else
                    .CommandText = "SELECT ID, Dates, FileName, AddNewRecord, UpdateRecord, CodeSite, TotalRecord, TotalImported, TotalErrors, Done,CodeMarkGroup, CodeSupplierGroup FROM EgsWBulkImportMain WHERE CodeSite=@CodeSite AND ID = @IDMain "
                    .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                    .Parameters.Add("@IDMain", SqlDbType.Int).Value = intIDMain
                End If

                .CommandType = CommandType.Text
                .Connection.Open()
                .ExecuteNonQuery()

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                cmd.Connection.Close()
                Return dt
            End With
        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Return Nothing
        End Try
    End Function

    Public Function fctGetEgsBulkImportErrors(ByVal intCode As Integer, Optional ByVal enumType As MenuType = MenuType.Merchandise) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter

        Try
            With cmd
                .Connection = cn
                .CommandType = CommandType.StoredProcedure
                .CommandText = "EGSBULKSHOWERROR"
                .Parameters.Add("@IDMain", SqlDbType.Int).Value = intCode
                .Parameters.Add("@intType", SqlDbType.Int).Value = enumType
                .Connection.Open()
                .ExecuteNonQuery()

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                Return dt
            End With
        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Return Nothing
        End Try
    End Function

    Public Function fctGetEgsBulkImportTotalRec(ByVal intCode As Integer) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter

        Try
            With cmd
                .Connection = cn
                .CommandType = CommandType.StoredProcedure
                .CommandText = "EGSBULKSHOWIMPORTED"
                .Parameters.Add("@IDMain", SqlDbType.Int).Value = intCode
                .Connection.Open()
                .ExecuteNonQuery()

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                Return dt
            End With
        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Return Nothing
        End Try
    End Function





    '---
    Public Function fctGetXLSDataSource(ByVal strFilePath As String, Optional ByVal nClient As Integer = 0) As DataTable 'VRP 13.01.2009

        Select Case nClient 'VRP
            Case 11 'SV
                Try
                    Dim xld As New Aspose.Excel.ExcelDesigner
                    xld.Open(strFilePath)
                    Dim sheet As Aspose.Excel.Worksheet = xld.Excel.Worksheets.Item(0)
                    Return sheet.Cells.ExportDataTable(0, 0, sheet.Cells.MaxRow + 1, sheet.Cells.MaxColumn + 1, True)
                Catch ex As Exception
                    Return Nothing
                End Try
            Case Else
                Try
                    Dim cn As New System.Data.OleDb.OleDbConnection
                    With cn
                        .ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" _
                            & strFilePath & ";Extended Properties=""Excel 8.0;HDR=YES"";"


                        .Open()
                    End With

                    '* Create the Command Object
                    Dim cmd As New System.Data.OleDb.OleDbCommand
                    With cmd
                        .Connection = cn
                        .CommandType = CommandType.Text
                        .CommandText = "SELECT * FROM [Sheet1$]"
                        Dim da As New System.Data.OleDb.OleDbDataAdapter(cmd)
                        Dim ds As New DataSet
                        da.Fill(ds)
                        Dim dt As DataTable = ds.Tables(0)
                        'dt.Rows.Remove(dt.Rows(0)) 'DLS 21.1.2009
                        EditColName(dt) 'VRP 13.01.2009
                        Return dt
                    End With
                    cn.Close()
                Catch ex As Exception
                    Return Nothing
                End Try
        End Select
    End Function
#End Region


    '//LD20160606
#Region "Get Separator Type"

    '//LD20160606
    Public Sub GetRegionalSeparatorFormat()

        Try


            If mv_strImportationType = "0" Then

                If mv_strCsvSettingsValue = "5" Then
                    Exit Sub
                End If

                If mv_strCsvSettingsValue = "1" Then
                    'Dim culture As New System.Globalization.CultureInfo("")
                    mv_strCSVSeparator = "," 'culture.TextInfo.ListSeparator
                    mv_strDecimalSeparator = "." 'culture.NumberFormat.NumberDecimalSeparator
                    mv_strThousandSeparator = "," 'culture.NumberFormat.NumberDecimalDigits
                ElseIf mv_strCsvSettingsValue = "2" Then
                    mv_strCSVSeparator = ";"
                    mv_strDecimalSeparator = ","
                    mv_strThousandSeparator = "."
                ElseIf mv_strCsvSettingsValue = "3" Then
                    mv_strCSVSeparator = ";"
                    mv_strDecimalSeparator = "."
                    mv_strThousandSeparator = ","
                ElseIf mv_strCsvSettingsValue = "4" Then
                    mv_strCSVSeparator = ";"
                    mv_strDecimalSeparator = ","
                    mv_strThousandSeparator = "."
                End If


            End If

        Catch ex As Exception

        End Try

    End Sub


#End Region

    '//LD20170510 Add mechandise translation
    Public Function BULKImportMainTranslation(ByRef intIDMain As Integer, ByVal arryTranslationHeaders As ArrayList) As Boolean
        Dim cmd As New SqlCommand
        Try



            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "BULK_ImportTranslation"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@IDMain", SqlDbType.NVarChar, 100).Value = intIDMain

                '--------- Nutrient Headers ------------
                Dim i As Integer
                Dim strParameterName As String
                For i = 0 To arryTranslationHeaders.Count - 1

                    If i > 6 Then Exit For
                    strParameterName = "@Translation" & (i + 1)
                    .Parameters.Add(strParameterName, SqlDbType.NVarChar, 50).Value = arryTranslationHeaders(i).ToString
                Next

                .Connection.Open()
                .ExecuteNonQuery()
                cmd.Connection.Close()

                BULKImportMainTranslation = True


            End With
        Catch ex As Exception
            BULKImportMainTranslation = False
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    '//LD20170510 Get Max Decimal Places
    Public Function BULKImportNumberFormat(ByRef NumberFormat As String, ByRef DecimalCount As Integer) As Boolean
        Dim cmd As New SqlCommand
        Try



            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "BULK_NUMBERFORMAL"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@NumberFormat", SqlDbType.NVarChar, 30).Direction = ParameterDirection.Output
                .Parameters.Add("@DecimalCount", SqlDbType.Int).Direction = ParameterDirection.Output
                .Connection.Open()
                .ExecuteNonQuery()
                cmd.Connection.Close()
                NumberFormat = CStr(.Parameters("@NumberFormat").Value)
                DecimalCount = CInt(.Parameters("@DecimalCount").Value)
                BULKImportNumberFormat = True
                .Connection.Open()
                .ExecuteNonQuery()
                cmd.Connection.Close()

            End With
        Catch ex As Exception
            BULKImportNumberFormat = False
            NumberFormat = "####0.000"
            DecimalCount = 3
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
        End Try
    End Function

    'Function ReadXLS(strFileName As String)
    '    Dim dtReadExcel As New DataTable
    '    Try
    '        Dim workbook As New Spire.Xls.Workbook()
    '        workbook.LoadFromFile(strFileName)
    '        Dim sheet1 As Spire.Xls.Worksheet = workbook.Worksheets(0)
    '        dtReadExcel = sheet1.ExportDataTable()
    '        EditColName(dtReadExcel)
    '    Catch ex As Exception
    '    End Try
    '    Return dtReadExcel
    'End Function

End Class

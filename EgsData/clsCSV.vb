Imports System.IO
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Threading
Imports System.Text

Public Class clsCSV
    Inherits EgsData.clsDBRoutine
    Private reader As StreamReader
    Private m_nOwner As Integer
    Public m_nCodeUser As Integer
    Private m_ex As Exception
    Private m_arrCodes As ArrayList
    Private cListe As EgsData.clsListe
    Private m_arrCodesToSkip As ArrayList
    Private m_hashNutrientColIndex As Hashtable
    Private m_hashImportOptionFields As New Hashtable 'DLS 09/13/2005
    Private m_bCompareNumber As Boolean = False
    Private intCodeUser As Integer
    'VRP 03.01.2007
    Private L_AppType As EgsData.enumAppType
    Private L_strCnn As String
    Private L_bytFetchType As EgsData.enumEgswFetchType
    '----
    Public mv_strCSVSeparator As String '//LD20160803 Modular Variable
    Public mv_strDecimalSeparator As String '//LD20160803 Modular Variable
    Public mv_strThousandSeparator As String '//LD20160803 Modular Variable
    Public mv_strCsvSettingsValue As String '//LD20160803 Modular Variable

    Private mv_CultureSelected As System.Globalization.CultureInfo '//LD20160803 Modular Variable



    Enum adCSVMerchandiseColumns1
        code = 0
        number = 1
        name = 2
        unit1 = 3
        unit2 = 4
        unit3 = 5
        unit4 = 6
        price1 = 7
        price2 = 8
        price3 = 9
        price4 = 10
        ratio1 = 11
        ratio2 = 12
        ratio3 = 13
        '      ratio4 = 14
        supplier = 14
        category = 15
        tax = 16
        currency = 17
        codelink = 18
        n1 = 19
        n2 = 20
        n3 = 21
        n4 = 22
        n5 = 23
        n6 = 24
        n7 = 25
        n8 = 26
        n9 = 27
        n10 = 28
        n11 = 29
        n12 = 30
        picture = 31
        wastage1 = 32
        wastage2 = 33
        wastage3 = 34
        wastage4 = 35
        wastage5 = 36
        cookingme = 37
        description = 38
        ingredients = 39
        preparation = 40
        cookingtip = 41
        refinement = 42
        storage = 43
        productivity = 44
        language = 45
    End Enum

    Public Sub New(ByVal eAppType As EgsData.enumAppType, ByVal strCnn As String, _
        Optional ByVal bytFetchType As EgsData.enumEgswFetchType = EgsData.enumEgswFetchType.DataReader)
        L_AppType = eAppType
        L_strCnn = strCnn
        L_bytFetchType = bytFetchType
        cListe = New EgsData.clsListe(L_AppType, L_strCnn, L_bytFetchType)
        m_arrCodes = New ArrayList
        m_arrCodesToSkip = New ArrayList
    End Sub

    Public WriteOnly Property CodeUser() As Integer 'DLS 09/16/2005
        Set(ByVal Value As Integer)
            m_nCodeUser = Value
        End Set
    End Property

    Public WriteOnly Property Language() As Integer
        Set(ByVal Value As Integer)
            intCodeUser = Value
        End Set
    End Property

    Public ReadOnly Property GetException() As Exception
        Get
            Return m_ex
        End Get
    End Property

    Public Function WriteMerchandise(ByVal fullpath As String, ByVal arrCodes As ArrayList, ByVal nCodeUser As Integer, Optional ByVal nCodeSetPrice As Integer = 0, Optional ByVal nCodeLang As Integer = 0) As Boolean
        'On Error GoTo errWrite
        Dim sb As New StringBuilder
        Dim cNutrient As New EgsData.clsNutrient(L_AppType, L_strCnn, L_bytFetchType)
        Dim drPrice As SqlDataReader
        Dim nCodeliste As Integer
        Dim nPriceValues(3) As Double
        Dim sPriceUnits(3) As String
        Dim nPriceRatios(3) As Double
        Dim sSymbole As String
        Dim clang As New clsEGSLanguage(nCodeLang)
        Dim sPriceValues(3) As String
        Dim sPriceRatios(3) As String
        Dim cAllergen As New clsAllergen(New structUser, enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataTable) '//LD20160929 Declare new allegen to call function call header of allergen export
        '//LD2016060 Overide Format
        GetRegionalSeparatorFormat()
        Dim m_defaultCulture As System.Globalization.CultureInfo

        If mv_CultureSelected Is Nothing Then
            m_defaultCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US")
        Else

            m_defaultCulture = mv_CultureSelected
        End If

        Dim str_Separator As String = m_defaultCulture.TextInfo.ListSeparator

        If str_Separator = "" Then
            str_Separator = ";"
        End If
        'cNutrient.SetConnection(L_strCnn)
        'cListe.SetConnection(L_strCnn)

        Try
            '// Write header

            ' RJL - swissarmy :02-11-2014
            With sb
                .Append(clang.GetString(clsEGSLanguage.CodeType.code) & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Number) & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Name) & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Unit) & "1" & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Unit) & "2" & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Unit) & "3" & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Unit) & "4" & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Price) & "1" & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Price) & "2" & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Price) & "3" & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Price) & "4" & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Ratio) & "1" & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Ratio) & "2" & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Ratio) & "3" & str_Separator)
                '.Append(clang.GetString(clsEGSLanguage.CodeType.Ratio) & "4,")

                .Append(clang.GetString(clsEGSLanguage.CodeType.Supplier) & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Category) & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Brand) & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Tax) & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Currency) & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.code) & clang.GetString(clsEGSLanguage.CodeType.Link) & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Picture) & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Wastage) & "1" & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Wastage) & "2" & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Wastage) & "3" & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Wastage) & "4" & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Description) & str_Separator) 'AGL 2014.02.13 - 11639
                '.Append("wastage5,")


                '// Insert Nutrients Headers
                'Dim dr As SqlDataReader = cNutrient.GetNutrientDefListReader(m_nCodeUser)
                'Dim sRefName As String
                'Dim counter As Integer = 1
                'Dim nMaxNutrient As Integer = 12
                'Dim nMaxPrice As Integer = 4

                'While dr.Read
                '    sRefName = "N=" & CStr(dr.Item("DatabaseName")) & "=" & CStr(dr.Item("name"))
                '    sb.Append(fctEncodeFORCSV(sRefName))
                '    sb.Append(",")
                '    counter += 1
                'End While
                'nMaxNutrient = counter
                ''Do Until counter > nMaxNutrient
                ''    sRefName = "N=" & "" & "=" & ""
                ''    sb.Append(fctEncodeFORCSV(sRefName) & ",")
                ''    counter += 1
                ''Loop
                'dr.Close()
                'VRP 
                'Dim dt As DataTable = cNutrient.GetNutrientDefList(m_nCodeUser, 0) old Commented by LD20160930
                Dim dt As DataTable = cNutrient.GetNutrientHeaderForExport(m_nCodeUser)
                Dim sRefName As String
                Dim counter As Integer = 0
                Dim nMaxNutrient As Integer = 12
                Dim nMaxPrice As Integer = 4

                For Each row As DataRow In dt.Rows
                    sRefName = "N=" & CStr(row("DatabaseName")) & "=" & CStr(row("name"))
                    sb.Append(fctEncodeFORCSV(sRefName))
                    sb.Append(str_Separator)
                    counter += 1
                Next
                nMaxNutrient = counter
                Dim dr As SqlDataReader

                ''RJL - 9983 : 12-19-2013
                'If nMaxNutrient > 0 Then
                '    .Append("nutrients,")
                'End If

                ' RDC 11.04.2013 : Removed as per QA request based on issue CWM-9188
                ' RDC 11.04.2013 : Bug CWM-9188 - Export - CSV - Merchandise - Info1 values and Language are not included in the export output
                '.Append("cookingmethod,")
                '.Append(clang.GetString(clsEGSLanguage.CodeType.Ingredient) & ",") 'kmqdc 5.14.2015 'Change Description to Ingridient Column name
                .Append(clang.GetString(clsEGSLanguage.CodeType.Ingredients) & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Preparation) & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.CookingTip) & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Refinement) & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Storage) & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Productivity) & str_Separator)
                .Append(clang.GetString(clsEGSLanguage.CodeType.Language) & str_Separator)

                'LD20160929 ADD AllergenHeader

                Dim dtAllergen As DataTable = cAllergen.GetAllergenDefList(nCodeLang, nCodeUser)
                Dim Allergencounter As Integer = 0
                Dim AllergenLawType As String = "UL"
                For Each row As DataRow In dtAllergen.Rows
                    sRefName = "A=" & CStr(row("Header"))
                    sb.Append(fctEncodeFORCSV(sRefName))
                    sb.Append(str_Separator)
                    Allergencounter += 1
                Next

                If dtAllergen.Rows.Count > 14 Then
                    AllergenLawType = "SW"
                End If

                .Append(vbCrLf)





                '// insert merchandise per line
                Dim intIDMain As Integer = -1
                Dim strCodes As String = ""
                For i As Integer = 0 To arrCodes.Count - 1
                    strCodes += arrCodes(i).ToString & ","
                Next
                intIDMain = cListe.fctSaveToTempList(strCodes, m_nCodeUser)
                dr = cListe.GetListeList(intCodeUser, intIDMain, -1, True, 0, 0, m_nCodeUser)

                While dr.Read

                    nCodeliste = CInt(dr.Item("code"))
                    .Append(fctEncodeFORCSV(nCodeliste))
                    .Append(str_Separator)

                    .Append(fctEncodeFORCSV(dr.Item("number")))
                    .Append(str_Separator)

                    .Append(fctEncodeFORCSV(dr.Item("name")))
                    .Append(str_Separator)

                    '// get unit prices
                    'drPrice = cListe.GetListeSetOfPriceReader(nCodeliste, nCodeSetPrice)
                    Dim dtPrice As DataTable = cListe.GetListeSetOfPriceReader2(nCodeliste, nCodeSetPrice)
                    counter = 0
                    For counter = 0 To nMaxPrice - 1
                        nPriceValues(counter) = -1
                        sPriceUnits(counter) = ""
                        nPriceRatios(counter) = -1
                    Next

                    counter = 1

                    'If Not drPrice Is Nothing Then
                    '    While drPrice.Read
                    '        If counter <= nMaxPrice Then
                    '            sSymbole = drPrice.Item("symbole").ToString
                    '            nPriceValues(counter - 1) = CDbl(drPrice.Item("price"))
                    '            sPriceUnits(counter - 1) = fctEncodeFORCSV(drPrice.Item("namedef"))
                    '            nPriceRatios(counter - 1) = CDbl(drPrice.Item("ratio"))
                    '        Else
                    '            Exit While
                    '        End If
                    '        counter += 1
                    '    End While
                    '    drPrice.Close()
                    'End If
                    If Not dtPrice Is Nothing Then

                        For Each drx In dtPrice.Rows
                            If counter <= nMaxPrice Then
                                sSymbole = drx("symbole").ToString
                                nPriceValues(counter - 1) = CDbl(drx("price"))
                                sPriceUnits(counter - 1) = fctEncodeFORCSV(drx("namedef"))
                                nPriceRatios(counter - 1) = CDbl(drx("ratio"))

                                sPriceValues(counter - 1) = drx("price").ToString().Replace(".", mv_strDecimalSeparator)
                                sPriceRatios(counter - 1) = drx("ratio").ToString().Replace(".", mv_strDecimalSeparator)
                            Else
                                Exit For
                            End If
                            counter += 1
                        Next

                    End If

                    '// insert unit prices 
                    counter = 0
                    For counter = 0 To nMaxPrice - 1
                        If nPriceValues(counter) > -1 Then
                            .Append(sPriceUnits(counter) & str_Separator)
                        Else
                            .Append(str_Separator)
                        End If
                    Next
                    For counter = 0 To nMaxPrice - 1
                        If nPriceValues(counter) > -1 Then
                            .Append(sPriceValues(counter).ToString() & str_Separator)
                        Else
                            .Append(str_Separator)

                        End If
                    Next
                    For counter = 1 To nMaxPrice - 1
                        If nPriceValues(counter) > -1 Then
                            If CDbl(nPriceRatios(counter)) = 0 Then
                                .Append(0 & str_Separator)
                            Else
                                .Append(sPriceRatios(counter).ToString() & str_Separator)
                            End If
                        Else
                            .Append(str_Separator)
                        End If
                    Next

                    ' RDC 01.24.2014 : Applied fix for ?NAME# - LD20160725 remove space to fix the problem in delimitation upon opening in excel
                    .Append(fctEncodeFORCSV(dr.Item("suppliername")))
                    .Append(str_Separator)

                    ' RDC 01.24.2014 : Applied fix for ?NAME# - LD20160725 remove space to fix the problem in delimitation upon opening in excel
                    .Append(fctEncodeFORCSV(dr.Item("categoryname")))
                    .Append(str_Separator)

                    .Append(fctEncodeFORCSV(dr.Item("brandname")))
                    .Append(str_Separator)

                    .Append(dr.Item("tax1"))
                    .Append(str_Separator)

                    .Append(sSymbole) ' put currency symbole TODO
                    .Append(str_Separator)

                    .Append(dr.Item("linkcode"))
                    .Append(str_Separator)

                    .Append("""" & fctEncodeFORCSV(dr.Item("picturename")) & """")
                    .Append(str_Separator)

                    .Append(dr.Item("wastage1").ToString().Replace(".", mv_strDecimalSeparator))
                    .Append(str_Separator)

                    .Append(dr.Item("wastage2").ToString().Replace(".", mv_strDecimalSeparator))
                    .Append(str_Separator)

                    .Append(dr.Item("wastage3").ToString().Replace(".", mv_strDecimalSeparator))
                    .Append(str_Separator)

                    .Append(dr.Item("wastage4").ToString().Replace(".", mv_strDecimalSeparator))
                    .Append(str_Separator)

                    '.Append(dr.Item("wastage5"))
                    '.Append(",")

                    .Append(fctEncodeFORCSV(dr.Item("description")))
                    .Append(str_Separator)

                    For counter = 1 To nMaxNutrient
                        If IsDBNull((dr.Item("N" & counter))) Then
                            .Append("0")
                            If Not counter > nMaxNutrient Then
                                .Append(str_Separator)
                            End If
                        Else
                            If Not counter > nMaxNutrient Then
                                If dr.Item("N" & counter).ToString().Contains("-") Then 'LD20160725 Bug in double conversion change dr type to string instead of double 
                                    .Append("0")
                                Else
                                    .Append(dr.Item("N" & counter).ToString().Replace(".", mv_strDecimalSeparator))
                                End If

                                .Append(str_Separator)
                            End If
                        End If
                    Next

                    'For counter = 1 To nMaxNutrient
                    '    .Append(dr.Item("N" & counter))
                    '    If Not counter = nMaxNutrient Then
                    '        .Append(",")
                    '    End If
                    'Next

                    ' RDC 11.04.2013 : Removed as per QA request based on issue CWM-9188
                    ' RDC 11.04.2013 : Bug CWM-9188 - Export - CSV - Merchandise - Info1 values and Language are not included in the export output 
                    '.Append(fctEncodeFORCSV(dr.Item("cookingmethod")))
                    '.Append(",")



                    '.Append(fctEncodeFORCSV(dr.Item("ingredients")))
                    '.Append(",")

                    .Append(fctEncodeFORCSV(dr.Item("ingredients"))) ' KMQDC 11.24.2015
                    .Append(str_Separator)

                    .Append(fctEncodeFORCSV(dr.Item("preparation")))
                    .Append(str_Separator)

                    .Append(fctEncodeFORCSV(dr.Item("cookingtip")))
                    .Append(str_Separator)

                    .Append(fctEncodeFORCSV((dr.Item("refinement"))))
                    .Append(str_Separator)

                    .Append(fctEncodeFORCSV((dr.Item("storage"))))
                    .Append(str_Separator)

                    .Append(fctEncodeFORCSV((dr.Item("productivity"))))
                    .Append(str_Separator)

                    .Append((dr.Item("language")))
                    .Append(str_Separator)
                    'LD20160929 Write Allergen Loop to type of law
                    For counter = 1 To dtAllergen.Rows.Count
                        If IsDBNull((dr.Item(AllergenLawType & counter))) Then
                            .Append("")
                            .Append(str_Separator)
                        Else
                            If dr.Item(AllergenLawType & counter).ToString().Contains("-") Then 'LD20160725 Bug in double conversion change dr type to string instead of double 
                                .Append("")
                            Else
                                .Append(dr.Item(AllergenLawType & counter).ToString())
                            End If

                            .Append(str_Separator)

                        End If
                    Next


                    .Append(vbCrLf)
                End While
                dr.Close()
            End With
        Catch ex As Exception
            ' RDC 10.30.2013 : Added notification if csv conversion goes wrong.
            MsgBox(ex.Message)
        End Try

        'Dim writer As TextWriter = File.CreateText(fullpath)

        'writer.Write(sb.ToString)
        'writer.Close()

        'Dim fs As New FileStream(fullpath, FileMode.Create)
        'Dim t As New StreamWriter(fs, Encoding.UTF8)
        't.Write(sb.ToString)
        't.Close()
        't.Dispose()
        '//LD20160725 Change encoding type upon writing physically the text to UTF8 to fix the problem in special character letter when open the csv file in excel 
        Using tw As StreamWriter = New StreamWriter(fullpath, False, Encoding.UTF8)
            tw.Write(sb.ToString)
        End Using

        Return True
        Exit Function
errWrite:
        Return False
    End Function

    Function fctDecodeCSV(ByVal strX As String) As String
        If Left$(strX, 1) = """" And Right(strX, 1) = """" Then
            strX = Mid$(strX, 2, Len(strX) - 2)
        End If

        strX = strX.Replace("�", vbCrLf)
        strX = strX.Replace("�", Chr(13))
        strX = strX.Replace("@^@", Chr(34))
        strX = strX.Replace("@$@", ",")
        fctDecodeCSV = strX
    End Function

    Function fctEncodeFORCSV(ByVal value As Object) As String
        If value Is Nothing Then
            Return ""
        End If

        Dim sValue As String = value.ToString

        sValue = sValue.Replace(vbCrLf, "�")
        sValue = sValue.Replace(Chr(13), "�")
        'sValue = sValue.Replace(Chr(34), "@^@") 'AGL 2014.02.13 - 11639
        sValue = sValue.Replace("""", "'")


        'If sValue.ToString.IndexOf(",") > -1 Then
        '    sValue = Chr(34) & sValue & Chr(34)
        'End If
        Dim strSeparator As String = "," '= System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator

        If sValue.Length > 1 Then
            If sValue.Substring(0, 1) = "-" Then
                sValue = " " & sValue
            End If
        End If
        If sValue.Contains(strSeparator) Then
            sValue = Chr(34) & sValue & Chr(34)
        End If

        Return sValue.ToString
    End Function

    Function fctReplaceFORCSV(ByVal strX As String) As String

        Dim i As Integer
        Dim flagInside As Boolean

        flagInside = False
        For i = 1 To Len(strX)

            If Mid$(strX, i, 1) = """" Then
                flagInside = (Not flagInside)
            End If
            If flagInside And Mid$(strX, i, 1) = "," Then
                strX = Left(strX, i - 1) & "@$@" & Right(strX, Len(strX) - i)
            End If
        Next i
        fctReplaceFORCSV = strX

    End Function

    Public Function CSVExport(ByVal fullpath As String, ByVal dtMerchandise As DataTable, ByVal dtRecipe As DataTable, ByVal strSite As String, Optional nCodeLang As Integer = 1) As Boolean

        On Error GoTo errWrite
        Dim sb As New StringBuilder
        Dim cNutrient As New EgsData.clsNutrient(L_AppType, L_strCnn, L_bytFetchType)
        Dim drPrice As SqlDataReader
        Dim nCodeliste As Integer
        Dim nPriceValues(3) As Double
        Dim sPriceUnits(3) As String
        Dim nPriceRatios(3) As Double
        Dim sSymbole As String
        Dim clang As New clsEGSLanguage(nCodeLang)
        'AGL 2015.03.04 - changed separator to be user-defined
        Dim strSeparator As String = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator

        '// Write header
        With sb
            '.Append("Code" & strSeparator)
            '.Append("Genossenschaft" & strSeparator)
            '.Append("Typ" & strSeparator)
            '.Append("Kategorie" & strSeparator)
            '.Append("Name" & strSeparator)
            '.Append("Preise" & strSeparator)
            '.Append("Steuer" & strSeparator)
            '.Append("Einheit" & strSeparator)
            '.Append(vbCrLf)

            ''AMTLA 2017.03.01
            .Append(clang.GetString(clsEGSLanguage.CodeType.code) & strSeparator)
            .Append(clang.GetString(clsEGSLanguage.CodeType.Site) & strSeparator)
            .Append(clang.GetString(clsEGSLanguage.CodeType.Type_) & strSeparator)
            .Append(clang.GetString(clsEGSLanguage.CodeType.Category) & strSeparator)
            .Append(clang.GetString(clsEGSLanguage.CodeType.Name) & strSeparator)
            .Append(clang.GetString(clsEGSLanguage.CodeType.Price) & strSeparator)
            .Append(clang.GetString(clsEGSLanguage.CodeType.Tax) & strSeparator)
            .Append(clang.GetString(clsEGSLanguage.CodeType.Unit) & strSeparator)
            .Append(vbCrLf)


            '// insert merchandise
            If Not dtMerchandise Is Nothing Then
                For Each r As DataRow In dtMerchandise.Rows
                    .Append(fctEncodeFORCSV(r("Code")))
                    .Append(strSeparator)

                    '.Append(fctEncodeFORCSV(r("CodeSiteName")))
                    '.Append(strSeparator)
                    'AMTLA 2017.03.01
                    .Append(fctEncodeFORCSV(clang.GetString(clsEGSLanguage.CodeType.Merchandise))) ''.Append(fctEncodeFORCSV("W"))
                    .Append(strSeparator)

                    .Append(fctEncodeFORCSV(r("Category")))
                    .Append(strSeparator)

                    '.Append(r("Name"))
                    .Append(fctEncodeFORCSV(r("Name")))
                    .Append(strSeparator)

                    '.Append(fctEncodeFORCSV(r("Category")))
                    '.Append(",")

                    '.Append(fctEncodeFORCSV(r("Name")))
                    '.Append(",")

                    .Append(fctEncodeFORCSV(r("Price1")))
                    .Append(strSeparator)

                    Dim strTax As String = "0"
                    Select Case CStrDB(r("Tax1"))
                        Case "0"
                            strTax = "0"
                        Case "2.4"
                            strTax = "1"
                        Case "7.6"
                            strTax = "2"
                        Case "8"
                            strTax = "3"
                        Case Else
                            strTax = "-1"
                    End Select

                    .Append(fctEncodeFORCSV(r("Tax1")))
                    .Append(strSeparator)

                    .Append(fctEncodeFORCSV(r("Unit1")))
                    .Append(strSeparator)

                    .Append(vbCrLf)
                Next
            End If

            If Not dtRecipe Is Nothing Then
                For Each r As DataRow In dtRecipe.Rows
                    .Append(fctEncodeFORCSV(r("Code")))
                    .Append(strSeparator)

                    .Append(fctEncodeFORCSV(strSite))
                    .Append(strSeparator)

                    .Append(fctEncodeFORCSV(clang.GetString(clsEGSLanguage.CodeType.Recipe))) ''.Append(fctEncodeFORCSV("R"))
                    .Append(strSeparator)

                    .Append(fctEncodeFORCSV(r("Category")))
                    .Append(strSeparator)

                    '.Append(r("Name"))
                    .Append(fctEncodeFORCSV(r("Name")))
                    .Append(strSeparator)

                    '.Append(fctEncodeFORCSV(r("Category")))
                    '.Append(",")

                    '.Append(fctEncodeFORCSV(r("Name")))
                    '.Append(",")

                    Dim dblPricePerYield As Double = CDblDB(r("SellingPrice")) / CDblDB(r("YieldSize"))
                    .Append(fctEncodeFORCSV(FormatNumber(dblPricePerYield, 2)))
                    .Append(strSeparator)

                    Dim strTax As String = "0"
                    Select Case CStrDB(r("Tax"))
                        Case "0"
                            strTax = "0"
                        Case "2.4"
                            strTax = "1"
                        Case "7.6"
                            strTax = "2"
                        Case "8"
                            strTax = "3"
                        Case Else
                            strTax = "-1"
                    End Select

                    .Append(fctEncodeFORCSV(r("Tax")))
                    .Append(strSeparator)

                    .Append(fctEncodeFORCSV(r("YieldUnit")))
                    .Append(strSeparator)

                    .Append(vbCrLf)
                Next
            End If

        End With

        'Dim writer As TextWriter = File.CreateText(fullpath)
        'writer.Write(sb.ToString)
        'writer.Close()

        'Dim swriter As StreamWriter = File.CreateText(fullpath)        
        'swriter.Write(sb.ToString)
        'swriter.Close()

        Dim sw As New StreamWriter(File.Create(fullpath), System.Text.Encoding.UTF8)
        sw.WriteLine(sb.ToString)
        sw.Close()

        Return True
        Exit Function
errWrite:
        Return False
    End Function

    Function fctReplaceSemicolonWithComma(ByVal value As Object) As String
        If value Is Nothing Then
            Return ""
        End If

        Dim sValue As String = value.ToString

        sValue = sValue.Replace(";", ",")
        Return sValue.ToString
    End Function


    '//LD20160606
    Public Sub GetRegionalSeparatorFormat()

        Try

            If mv_strCsvSettingsValue = "1" Then
                'Dim culture As New System.Globalization.CultureInfo("")
                mv_strCSVSeparator = "," 'culture.TextInfo.ListSeparator
                mv_strDecimalSeparator = "." 'culture.NumberFormat.NumberDecimalSeparator
                mv_strThousandSeparator = "," 'culture.NumberFormat.NumberDecimalDigits
                mv_CultureSelected = System.Globalization.CultureInfo.GetCultureInfo("en-US")
            ElseIf mv_strCsvSettingsValue = "2" Then
                mv_strCSVSeparator = ";"
                mv_strDecimalSeparator = ","
                mv_strThousandSeparator = "."
                mv_CultureSelected = System.Globalization.CultureInfo.GetCultureInfo("de-DE")
            ElseIf mv_strCsvSettingsValue = "3" Then
                mv_strCSVSeparator = ";"
                mv_strDecimalSeparator = "."
                mv_strThousandSeparator = ","
                mv_CultureSelected = System.Globalization.CultureInfo.GetCultureInfo("fr-FR")
            ElseIf mv_strCsvSettingsValue = "4" Then
                mv_strCSVSeparator = ";"
                mv_strDecimalSeparator = ","
                mv_strThousandSeparator = "."
                mv_CultureSelected = System.Globalization.CultureInfo.GetCultureInfo("it-IT")
            End If




        Catch ex As Exception

        End Try

    End Sub

End Class

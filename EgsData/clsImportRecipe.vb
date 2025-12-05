Imports System.Data.SqlClient
Imports System.Data
Imports System.Char
Imports System.IO
Imports System.Data.OleDb
Imports System.Globalization
Imports System.Text
Imports System.Xml
Imports ICSharpCode.SharpZipLib.Zip
Imports OrganicBit.Zip

Public Class clsImportRecipe

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

    Private G_structTXS5 As sTXS5
    Private G_structTXS6 As sTXS6
    Private G_structTXS7 As sTXS7
    Private G_structTXS9 As sTXS9
    Private G_structTXC1 As sTXC1
    Private G_structTXC2 As sTXC2
    Private G_structTXC3 As sTXC3
    Private G_structXML As sXML

    Public n_IDMain As Integer

    Public Structure sTXS5
        Dim version As Integer
        Dim recipename As String
        Dim recipecategory As String
        Dim recipenumber As String
        Dim yieldquantity As Integer
        Dim yieldunitname As String
        Dim subrecipequantity As Integer
        Dim subrecipeunitname As String
        Dim picturename As String
        Dim languagecode As Integer
        Dim sourcename As String
        Dim ingnumber As String
        Dim quantity As Double
        Dim unit As String
        Dim item As String
        Dim complement As String
        Dim note As String
        Dim price As Double
        Dim priceunit As String
        Dim wastage1 As Integer
        Dim wastage2 As Integer
        Dim wastage3 As Integer
        Dim wastage4 As Integer
        Dim ingcategory As String
        Dim ingsupplier As String
        Dim preparation As String
    End Structure

    Public Structure sTXS6
        Dim version As Integer
        Dim recipename As String
        Dim sourceTXS As String
        Dim recipecategory As String
        Dim recipenumber As String
        Dim yieldquantity As Integer
        Dim yieldunitname As String
        Dim subrecipequantity As Integer
        Dim subrecipeunitname As String
        Dim picturename As String
        Dim languagecode As Integer
        Dim sourcename As String
        Dim ingnumber As String
        Dim quantity As Double
        Dim unit As String
        Dim item As String
        Dim complement As String
        Dim note As String
        Dim price As Double
        Dim priceunit As String
        Dim wastage1 As Integer
        Dim wastage2 As Integer
        Dim wastage3 As Integer
        Dim wastage4 As Integer
        Dim ingcategory As String
        Dim ingsupplier As String
        Dim preparation As String
    End Structure

    Public Structure sTXS7
        Dim version As Integer
        Dim type As Integer
        Dim recipename As String
        Dim sourceTXS As String
        Dim recipecategory As String
        Dim recipenumber As String
        Dim yieldquantity As Integer
        Dim yieldunitname As String
        Dim subrecipequantity As Integer
        Dim subrecipeunitname As String
        Dim picturename As String
        Dim languagecode As Integer
        Dim sourcename As String
        Dim ingnumber As String
        Dim quantity As Double
        Dim unit As String
        Dim item As String
        Dim complement As String
        Dim note As String
        Dim price As Double
        Dim priceunit As String
        Dim wastage1 As Integer
        Dim wastage2 As Integer
        Dim wastage3 As Integer
        Dim wastage4 As Integer
        Dim ingcategory As String
        Dim ingsupplier As String
        Dim preparation As String
    End Structure

    Public Structure sTXS9
        Dim version As Integer
        Dim type As Integer
        Dim recipename As String
        Dim sourceTXS As String
        Dim recipecategory As String
        Dim recipenumber As String
        Dim yieldquantity As Double
        Dim yieldunitname As String
        Dim subrecipequantity As Double
        Dim subrecipeunitname As String
        Dim batchqty As String
        Dim picturename As String
        Dim languagecode As Integer
        Dim sourcename As String
        Dim ingnumber As String
        Dim quantity As Double
        Dim unit As String
        Dim item As String
        Dim complement As String
        Dim note As String
        Dim price As Double
        Dim priceunit As String
        Dim wastage1 As Integer
        Dim wastage2 As Integer
        Dim wastage3 As Integer
        Dim wastage4 As Integer
        Dim ingcategory As String
        Dim ingsupplier As String
        Dim preparation As String
    End Structure

    Public Structure sTXC1
        Dim version As Integer
        Dim recipename As String
        Dim recipecategory As String
        Dim recipenumber As String
        Dim yieldquantity As Integer
        Dim yieldunitname As String
        Dim subrecipeunitcode As Integer
        Dim picturename As String
        Dim languagecode As Integer
        Dim sourcename As String
        Dim ingnumber As String
        Dim quantity As Double
        Dim unit As String
        Dim item As String
        Dim complement As String
        Dim preparation As String
    End Structure

    Public Structure sTXC2
        Dim version As Integer
        Dim recipename As String
        Dim recipecategory As String
        Dim recipenumber As String
        Dim yieldquantity As Double
        Dim yieldunitname As String
        Dim subrecipeunitcode As String
        Dim picturename As String
        Dim languagecode As Integer
        Dim sourcename As String
        Dim ingnumber As String
        Dim quantity As Double
        Dim unit As String
        Dim item As String
        Dim complement As String
        Dim note As String
        Dim preparation As String
    End Structure

    Public Structure sTXC3
        Dim version As Integer
        Dim recipename As String
        Dim recipecategory As String
        Dim recipenumber As String
        Dim yieldquantity As Double
        Dim yieldunitname As String
        Dim subrecipequantity As Double
        Dim subrecipeunitname As String
        Dim picturename As String
        Dim languagecode As Integer
        Dim sourcename As String
        Dim ingnumber As String
        Dim quantity As Double
        Dim unit As String
        Dim item As String
        Dim complement As String
        Dim note As String
        Dim price As Double
        Dim priceunit As String
        Dim wastage1 As Integer
        Dim wastage2 As Integer
        Dim wastage3 As Integer
        Dim wastage4 As Integer
        Dim ingcategory As String
        Dim ingsupplier As String
        Dim preparation As String
    End Structure

    Public Structure sXML
        Dim title As String
        Dim number As String
        Dim language As Integer
        Dim source As String
        Dim category As String
        Dim yieldquantity As Double
        Dim yieldunit As String
        Dim yieldpercent As Integer
        Dim batchqty As Integer
        Dim subrecipequantity As Double
        Dim subrecipeunit As String
        Dim picture As String
        Dim code As Integer
        Dim inventorycode As String
        Dim keyfield As String
        Dim nutrientname As String
        Dim nutrientvalue As Double
        Dim nutrientunit As String
        Dim nutrientdatabase As String
        Dim description As String
        Dim remark As String
        Dim coolingtime As String
        Dim heatingtime As String
        Dim heatingtemperature As String
        Dim heatingmode As String
        Dim ccpdescription As String
        Dim fgname As String
        Dim fgnumber As String
        Dim fgbarcode As String
        Dim fgcomposition As String
        Dim fgconsumewithin As Integer
        Dim fgtransportationunit As String
        Dim fgfactortransportation As Integer
        Dim fgpackagingunit As String
        Dim fgfactorpackaging As Integer
        Dim fgstockunit As String
        Dim fgfactorstock As Integer
        Dim fgbasicunit As String
        Dim fgnote As String
        Dim fgincludeinventory As Integer
        Dim keychild As String
        Dim keyparent As String
        Dim quantity As Double
        Dim unit As String
        Dim itemname As String
        Dim itemnumber As String
        Dim wastage1 As Integer
        Dim wastage2 As Integer
        Dim wastage3 As Integer
        Dim wastage4 As Integer
        Dim procedure As String
        Dim complement As String
        Dim ingkeyfield As String
        Dim ingtranslanguage As Integer
        Dim transcomplement As String
        Dim transprocedure As String
        Dim preparation As String
        Dim translanguage As Integer
        Dim transtitle As String
        Dim transdescription As String
        Dim transremark As String
        Dim transccpdescription As String
        Dim transpreparation As String

        Dim Contant As Double 'DLS
        Dim ImposedPrice As Double 'DLS
        Dim Tax As Double 'DLS

    End Structure

#Region " XML "

    Public Function ReadXML(ByVal UrlToXmlFile As String) As Boolean
        ReadXML = False

        Dim intCounter As Integer = 1
        Dim intMarkGroupId As Integer = -1
        Dim strMarkGroupName As String = "Imported_" & Now.ToString("MMddyyyyhhmmss")

        Dim intItemID As Integer = -1
        Dim bNewRecipe As Boolean = False
        Dim bNewRecipeTrans As Boolean = False
        Dim bNewRecord As Boolean = False
        Dim bNewRecordTrans As Boolean = False
        Dim bNewKeywords As Boolean = False
        Dim intRecipeID As Integer = 1
        Dim intId As Integer = 1
        Dim intRecipeIdKeywords As Integer = 1
        Dim dtRecipeXML As New DataTable
        Dim dtIngredientXML As New DataTable
        Dim dtRecipeTranslationXML As New DataTable
        Dim dtIngredientTranslationXML As New DataTable
        Dim dtKeywordsXML As New DataTable

        Dim arrStructKeywords As New ArrayList
        Dim arrStructIngredients As New ArrayList
        Dim arrStructNutrients As New ArrayList
        Dim arrTranslations As New ArrayList
        Dim arrIngTranslations As New ArrayList

        Dim reader As XmlTextReader
        Dim intVersion As Integer = 0
        Dim strNutrientCodeLink As String = ""
        Dim hashListeNameNumber As Hashtable = New Hashtable

        Dim stream As StreamReader = New StreamReader(UrlToXmlFile, System.Text.Encoding.Default)
        reader = New XmlTextReader(stream)
        Dim strX As String = ""

        Try
            While reader.Read
                If reader.Name = "recipenet" And reader.NodeType = XmlNodeType.EndElement Then Exit While

                If reader.NodeType = XmlNodeType.Element Then
                    Select Case reader.Name
                        Case "version" : intVersion = CInt(reader.ReadString)
                        Case "recipe"
                            arrStructNutrients = New ArrayList
                            arrStructKeywords = New ArrayList
                            arrStructIngredients = New ArrayList
                            arrIngTranslations = New ArrayList
                            arrTranslations = New ArrayList
                        Case "head"
                            ReadXML_Header(reader, intVersion, G_structXML)
                        Case "nutrients"
                            ReadXML_Nutrient(reader, arrStructNutrients)
                        Case "description" : G_structXML.description = DecodeString(reader.ReadString)
                        Case "remark" : G_structXML.remark = DecodeString(reader.ReadString)
                        Case "haccp"
                            ReadXML_HACCP(reader, G_structXML)
                        Case "finishedGood"
                            ReadXML_FinishedGood(reader, G_structXML)
                        Case "keywords"
                            ReadXML_Keywords(reader, arrStructKeywords)
                        Case "ingredients"
                            ReadXML_Ingredients(reader, arrStructIngredients, arrIngTranslations)
                        Case "preparation" : G_structXML.preparation = DecodeString(reader.ReadString)
                        Case "setpricecalctax" : G_structXML.Tax = fctNullToZeroDBL(DecodeString(reader.ReadString))
                        Case "setpricecalcconst" : G_structXML.Contant = fctNullToZeroDBL(DecodeString(reader.ReadString))
                        Case "setpricecalcimposedprice" : G_structXML.ImposedPrice = fctNullToZeroDBL(DecodeString(reader.ReadString))
                        Case "translations"
                            ReadXML_Translations(reader, arrTranslations)
                            bNewRecipe = PopulateRecipeDataTable(bNewRecipe, bNewRecipeTrans, G_structXML, arrTranslations, dtRecipeXML, dtRecipeTranslationXML, intId)
                            bNewRecord = PopulateIngredientsDataTable(bNewRecord, bNewRecordTrans, G_structXML, arrStructIngredients, arrIngTranslations, dtIngredientXML, dtIngredientTranslationXML, intRecipeID)
                            bNewKeywords = PopulateKeywordsDataTable(bNewKeywords, arrStructKeywords, G_structXML, dtKeywordsXML, intRecipeIdKeywords)
                    End Select
                End If
                intCounter += 1
            End While
            reader.Close()

            fctBulkImportXMLRecipe(dtRecipeXML)
            fctBulkImportXMLIngredients(dtIngredientXML)
            fctBulkImportXMLRecipeTrans(dtRecipeTranslationXML)
            fctBulkImportXMLIngTrans(dtIngredientTranslationXML)
            fctBulkImportXMLKeywords(dtKeywordsXML)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub ReadXML_Header(ByRef reader As XmlTextReader, ByVal intVersion As Integer, ByRef G_strucXML As sXML)
        If reader.IsEmptyElement Then Exit Sub

        Do
            Select Case reader.Name
                Case "title" : G_strucXML.title = DecodeString(reader.ReadString).Trim
                Case "number" : G_strucXML.number = DecodeString(reader.ReadString)
                Case "language" : G_strucXML.language = CInt(DecodeString(reader.ReadString))
                Case "source" : G_strucXML.source = DecodeString(reader.ReadString)
                Case "category" : G_strucXML.category = DecodeString(reader.ReadString)
                Case "yieldquantity" : G_strucXML.yieldquantity = CDbl(reader.ReadString)
                Case "yieldunit" : G_strucXML.yieldunit = DecodeString(reader.ReadString)
                Case "yieldpercent" : G_strucXML.yieldpercent = CInt(reader.ReadString)
                Case "batchqty" : G_strucXML.batchqty = CInt(reader.ReadString)
                Case "subrecipequantity" : G_strucXML.subrecipequantity = CDbl(reader.ReadString)
                Case "subrecipeunit" : G_strucXML.subrecipeunit = DecodeString(reader.ReadString)
                Case "picture" : G_strucXML.picture = DecodeString(reader.ReadString)
                Case "code" : G_strucXML.code = CInt(reader.ReadString)
                Case "inventorycode" : G_strucXML.inventorycode = DecodeString(reader.ReadString)
                Case "keyfield" : G_strucXML.keyfield = DecodeString(reader.ReadString)
            End Select
            If reader.Name = "head" And reader.NodeType = XmlNodeType.EndElement Then Exit Do
        Loop Until reader.Read = False
    End Sub

    Private Sub ReadXML_Nutrient(ByRef reader As XmlTextReader, ByRef arrStructNutrients As ArrayList)
        If reader.IsEmptyElement Then Exit Sub
        Do
            If reader.Name = "nutrients" And reader.NodeType = XmlNodeType.EndElement Then Exit Do

            If reader.Name = "nutrient" Then
                Do
                    If reader.Name = "nutrient" And reader.NodeType = XmlNodeType.EndElement Then
                        arrStructNutrients.Add(G_structXML)
                        Exit Do
                    End If

                    If reader.NodeType = XmlNodeType.Element And reader.Name <> "" Then
                        Select Case reader.Name
                            Case "name"
                                G_structXML.nutrientname = DecodeString(reader.ReadString)
                            Case "value"
                                Dim strValue As String = DecodeString(reader.ReadString)
                                If IsNumeric(strValue) Then
                                    G_structXML.nutrientvalue = CDbl(strValue)
                                Else
                                    G_structXML.nutrientvalue = -1
                                End If
                            Case "database" : G_structXML.nutrientdatabase = DecodeString(reader.ReadString)
                        End Select
                    End If
                Loop Until reader.Read = False
            End If
        Loop Until reader.Read = False
    End Sub

    Private Sub ReadXML_HACCP(ByRef reader As XmlTextReader, ByRef G_structXML As sXML)

        If reader.IsEmptyElement Then Exit Sub

        Do
            Select Case reader.Name
                Case "coolingtime" : G_structXML.coolingtime = DecodeString(reader.ReadString)
                Case "heatingtime" : G_structXML.heatingtime = DecodeString(reader.ReadString)
                Case "heatingmode" : G_structXML.heatingmode = DecodeString(reader.ReadString)
                Case "heatingtemperature" : G_structXML.heatingtemperature = DecodeString(reader.ReadString)
                Case "ccpdescription" : G_structXML.ccpdescription = DecodeString(reader.ReadString)
            End Select

            If reader.Name = "haccp" And reader.NodeType = XmlNodeType.EndElement Then Exit Do
        Loop Until reader.Read = False
    End Sub

    Private Sub ReadXML_FinishedGood(ByRef reader As XmlTextReader, ByRef G_structXML As sXML)

        If reader.IsEmptyElement Then Exit Sub

        Do
            Select Case reader.Name
                Case "name" : G_structXML.fgname = DecodeString(reader.ReadString)
                Case "number" : G_structXML.fgnumber = DecodeString(reader.ReadString)
                Case "barcode" : G_structXML.fgbarcode = DecodeString(reader.ReadString)
                Case "composition" : G_structXML.fgcomposition = DecodeString(reader.ReadString)
                Case "consumeWithin" : G_structXML.fgconsumewithin = CInt(reader.ReadString)
                Case "transportationUnit" : G_structXML.fgtransportationunit = DecodeString(reader.ReadString)
                Case "factorTransportationVsPackaging" : G_structXML.fgfactortransportation = CInt(DecodeString(reader.ReadString))
                Case "packagingUnit" : G_structXML.fgpackagingunit = DecodeString(reader.ReadString)
                Case "factorPackagingVsStock" : G_structXML.fgfactorpackaging = CInt(reader.ReadString)
                Case "stockUnit" : G_structXML.fgstockunit = DecodeString(reader.ReadString)
                Case "factorStockVsBasic" : G_structXML.fgfactorstock = CInt(reader.ReadString)
                Case "basicUnit" : G_structXML.fgbasicunit = DecodeString(reader.ReadString)
                Case "note" : G_structXML.fgnote = DecodeString(reader.ReadString)
                Case "includeInInventory" : G_structXML.fgincludeinventory = CInt(reader.ReadString)
            End Select

            If reader.Name = "finishedGood" And reader.NodeType = XmlNodeType.EndElement Then Exit Do
        Loop Until reader.Read = False
    End Sub

    Private Sub ReadXML_Keywords(ByRef reader As XmlTextReader, ByRef arrStructKeywords As ArrayList)
        If reader.IsEmptyElement Then Exit Sub
        Do
            If reader.Name = "keywords" And reader.NodeType = XmlNodeType.EndElement Then Exit Do

            If reader.Name = "key" Then
                Do
                    If reader.Name = "key" And reader.NodeType = XmlNodeType.EndElement Then
                        arrStructKeywords.Add(G_structXML)
                        Exit Do
                    End If

                    If reader.NodeType = XmlNodeType.Element And reader.Name <> "" Then
                        Select Case reader.Name
                            Case "keychild" : G_structXML.keychild = DecodeString(reader.ReadString)
                            Case "keyparent" : G_structXML.keyparent = DecodeString(reader.ReadString)
                        End Select
                    End If
                Loop Until reader.Read = False
            End If
        Loop Until reader.Read = False
    End Sub

    Private Sub ReadXML_Ingredients(ByRef reader As XmlTextReader, ByRef arrStructIngredients As ArrayList, ByRef arrStructIngTrans As ArrayList)
        If reader.IsEmptyElement Then Exit Sub
        Do
            If reader.Name = "ingredients" And reader.NodeType = XmlNodeType.EndElement Then Exit Do
            If reader.Name = "ing" Then
                Do
                    If reader.Name = "ing" And reader.NodeType = XmlNodeType.EndElement Then
                        arrStructIngredients.Add(G_structXML)
                        Exit Do
                    End If

                    If reader.NodeType = XmlNodeType.Element And reader.Name <> "" Then
                        Select Case reader.Name
                            Case "quantity" : G_structXML.quantity = CDbl(reader.ReadString)
                            Case "unit" : G_structXML.unit = DecodeString(reader.ReadString)
                            Case "itemname" : G_structXML.itemname = DecodeString(reader.ReadString)
                            Case "itemnumber" : G_structXML.itemnumber = DecodeString(reader.ReadString)
                            Case "wastage1" : G_structXML.wastage1 = CInt(reader.ReadString)
                            Case "wastage2" : G_structXML.wastage2 = CInt(reader.ReadString)
                            Case "wastage3" : G_structXML.wastage3 = CInt(reader.ReadString)
                            Case "wastage4" : G_structXML.wastage4 = CInt(reader.ReadString)
                            Case "procedure" : G_structXML.procedure = DecodeString(reader.ReadString)
                            Case "complement" : G_structXML.complement = DecodeString(reader.ReadString)
                            Case "keyfield" : G_structXML.keyfield = DecodeString(reader.ReadString)
                            Case "translations"
                            Case "trans"
                                If reader.Name = "trans" Then
                                    Do
                                        If reader.Name = "trans" And reader.NodeType = XmlNodeType.EndElement Then
                                            arrStructIngTrans.Add(G_structXML)
                                            Exit Do
                                        End If

                                        If reader.NodeType = XmlNodeType.Element And reader.Name <> "" Then
                                            Select Case reader.Name
                                                Case "translanguage" : G_structXML.ingtranslanguage = CInt(reader.ReadString)
                                                Case "transcomplement" : G_structXML.transcomplement = DecodeString(reader.ReadString)
                                                Case "transprocedure" : G_structXML.transprocedure = DecodeString(reader.ReadString)
                                            End Select
                                        End If
                                    Loop Until reader.Read = False
                                End If
                        End Select
                    End If
                Loop Until reader.Read = False
            End If
        Loop Until reader.Read = False
    End Sub

    Private Sub ReadXML_Translations(ByRef reader As XmlTextReader, ByRef arrStructTranslations As ArrayList)
        If reader.IsEmptyElement Then Exit Sub
        Do
            If reader.Name = "translations" And reader.NodeType = XmlNodeType.EndElement Then Exit Do

            If reader.Name = "trans" Then
                Do
                    If reader.Name = "trans" And reader.NodeType = XmlNodeType.EndElement Then
                        arrStructTranslations.Add(G_structXML)
                        Exit Do
                    End If

                    If reader.NodeType = XmlNodeType.Element And reader.Name <> "" Then
                        Select Case reader.Name
                            Case "translanguage" : G_structXML.translanguage = CInt(reader.ReadString)
                            Case "transtitle" : G_structXML.transtitle = DecodeString(reader.ReadString)
                            Case "transdescription" : G_structXML.transdescription = DecodeString(reader.ReadString)
                            Case "transremark" : G_structXML.transremark = DecodeString(reader.ReadString)
                            Case "transccpdescription" : G_structXML.transccpdescription = DecodeString(reader.ReadString)
                            Case "transpreparation" : G_structXML.transpreparation = DecodeString(reader.ReadString)
                        End Select
                    End If
                Loop Until reader.Read = False
            End If
        Loop Until reader.Read = False
    End Sub

    Private Function PopulateRecipeDataTable(ByVal bNewRecipe As Boolean, ByRef bNewRecipeTrans As Boolean, ByVal structXML As sXML, ByVal arrRecipeTrans As ArrayList, _
                                            ByRef dtRecipeXML As DataTable, ByRef dtRecipeTranslationXML As DataTable, ByRef intRecipeID As Integer) As Boolean

        Dim dr As DataRow
        'Dim dtCodeDic As DataTable
        Dim drTrans As DataRow

        If bNewRecipe = False Then
            With dtRecipeXML
                .Columns.Add(New DataColumn("IdRecipe", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("IdMain", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("ListeType", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("Number", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Name", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Source", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Category", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Yield", System.Type.GetType("System.Double")))
                .Columns.Add(New DataColumn("YieldUnitName", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("YieldPercent", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("LanguageCode", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("srQty", System.Type.GetType("System.Double")))
                .Columns.Add(New DataColumn("srUnitName", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Picturename", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Note", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Remark", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("CoolingTime", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("HeatingTime", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("HeatingTemperature", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("HeatingMode", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("CCPDescription", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Description", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Coeff", System.Type.GetType("System.Double")))
                .Columns.Add(New DataColumn("Tax", System.Type.GetType("System.Double")))
                .Columns.Add(New DataColumn("ImposedPrice", System.Type.GetType("System.Double")))
                bNewRecipe = True
            End With
        End If

        dr = dtRecipeXML.NewRow()
        dr("IdRecipe") = intRecipeID
        dr("IdMain") = CInt(n_IDMain)
        dr("ListeType") = 8
        dr("Number") = structXML.number
        dr("Name") = structXML.title
        dr("Source") = structXML.source
        dr("Category") = structXML.category
        dr("Yield") = structXML.yieldquantity
        dr("YieldUnitName") = structXML.yieldunit
        dr("YieldPercent") = structXML.yieldpercent
        dr("LanguageCode") = structXML.language
        dr("srQty") = structXML.subrecipequantity
        dr("srUnitName") = structXML.subrecipeunit
        dr("Picturename") = structXML.picture
        dr("Note") = structXML.preparation
        dr("Remark") = structXML.remark
        dr("CoolingTime") = structXML.coolingtime
        dr("HeatingTime") = structXML.heatingtime
        dr("HeatingTemperature") = structXML.heatingtemperature
        dr("HeatingMode") = structXML.heatingmode
        dr("CCPDescription") = structXML.ccpdescription
        dr("Description") = structXML.description
        dr("Coeff") = structXML.Contant
        dr("ImposedPrice") = structXML.ImposedPrice
        dr("Tax") = structXML.Tax
        dtRecipeXML.Rows.Add(dr)

        If bNewRecipeTrans = False Then
            With dtRecipeTranslationXML
                .Columns.Add(New DataColumn("IdMain", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("IdRecipe", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("Name", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Note", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Remark", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("CCPDescription", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Description", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("CodeDictionary", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("CodeTrans", System.Type.GetType("System.Int32")))
                bNewRecipeTrans = True
            End With
        End If

        With G_structXML
            For Each structXML In arrRecipeTrans
                'dtCodeDic = fctGetCodeDictionary(structXML.translanguage)

                drTrans = dtRecipeTranslationXML.NewRow

                drTrans("IdMain") = CInt(n_IDMain)
                drTrans("IdRecipe") = intRecipeID
                drTrans("Name") = structXML.transtitle
                drTrans("Note") = structXML.transpreparation
                drTrans("Remark") = structXML.transremark
                drTrans("CCPDescription") = structXML.transccpdescription
                drTrans("Description") = structXML.transdescription
                drTrans("CodeDictionary") = structXML.translanguage 'IIf(IsDBNull(dtCodeDic.Rows(0).Item("codedictionary")), 1, dtCodeDic.Rows(0).Item("codedictionary"))
                'drTrans("CodeTrans") = 
                dtRecipeTranslationXML.Rows.Add(drTrans)
                'dtCodeDic.Reset()
            Next
        End With

        intRecipeID += 1
        Return bNewRecipe
    End Function

    Private Function PopulateIngredientsDataTable(ByVal bNewRecord As Boolean, ByRef bNewRecordTrans As Boolean, ByVal structXML As sXML, _
                                                  ByVal arrIngredients As ArrayList, ByVal arrIngredientsTrans As ArrayList, ByRef dtIngXML As DataTable, _
                                                  ByRef dtIngTranslationXML As DataTable, ByRef intRecipeID As Integer) As Boolean

        Dim dr As DataRow
        'Dim dtCodeDic As DataTable
        Dim drTrans As DataRow
        Dim intPosition As Integer = 1
        Dim intPositionTrans As Integer = 1

        If bNewRecord = False Then
            With dtIngXML
                .Columns.Add(New DataColumn("IdMain", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("IdRecipe", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("Position", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("ItemListeType", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("Number", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Name", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Quantity", System.Type.GetType("System.Double")))
                .Columns.Add(New DataColumn("UnitName", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Complement", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Preparation", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Wastage1", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("Wastage2", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("Wastage3", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("Wastage4", System.Type.GetType("System.Int32")))
                bNewRecord = True
            End With
        End If

        With G_structXML
            For Each structXML In arrIngredients
                dr = dtIngXML.NewRow

                dr("IdMain") = CInt(n_IDMain)
                dr("IdRecipe") = intRecipeID
                dr("Position") = intPosition
                'dr("ItemListeType")  = structXML.item
                dr("Number") = structXML.itemnumber
                dr("Name") = structXML.itemname
                dr("Quantity") = structXML.quantity
                dr("UnitName") = structXML.unit
                dr("Complement") = structXML.complement
                dr("Preparation") = structXML.procedure
                dr("Wastage1") = structXML.wastage1
                dr("Wastage2") = structXML.wastage2
                dr("Wastage3") = structXML.wastage3
                dr("Wastage4") = structXML.wastage4
                dtIngXML.Rows.Add(dr)
                intPosition += 1
            Next
        End With

        If bNewRecordTrans = False Then
            With dtIngTranslationXML
                .Columns.Add(New DataColumn("IdMain", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("IdRecipe", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("Position", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("Name", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Complement", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Preparation", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("CodeDictionary", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("CodeTrans", System.Type.GetType("System.Int32")))
                bNewRecordTrans = True
            End With
        End If

        With G_structXML
            For Each structXML In arrIngredientsTrans
                'dtCodeDic = fctGetCodeDictionary(structXML.translanguage)
                drTrans = dtIngTranslationXML.NewRow

                drTrans("IdMain") = CInt(n_IDMain)
                drTrans("IdRecipe") = intRecipeID
                drTrans("Position") = intPositionTrans
                'drTrans("Name") = structXML.itemname
                drTrans("Complement") = structXML.transcomplement
                drTrans("Preparation") = structXML.transprocedure
                drTrans("CodeDictionary") = structXML.ingtranslanguage 'IIf(IsDBNull(dtCodeDic.Rows(0).Item("codedictionary")), 1, dtCodeDic.Rows(0).Item("codedictionary"))
                'drTrans("CodeTrans") = 
                dtIngTranslationXML.Rows.Add(drTrans)
                intPositionTrans += 1
                'dtCodeDic.Reset()
            Next
        End With

        intRecipeID += 1
        Return bNewRecord
    End Function

    Private Function PopulateKeywordsDataTable(ByVal bNewKeywords As Boolean, ByVal arrKeywords As ArrayList, ByVal structXML As sXML, _
                                               ByRef dtKeywords As DataTable, ByRef intRecipeId As Integer) As Boolean
        Dim drTrans As DataRow

        If bNewKeywords = False Then
            With dtKeywords
                .Columns.Add(New DataColumn("IdMain", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("IdRecipe", System.Type.GetType("System.Int32")))
                .Columns.Add(New DataColumn("KeywordName", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("ParentName", System.Type.GetType("System.String")))
                .Columns.Add(New DataColumn("Derived", System.Type.GetType("System.Boolean")))
                bNewKeywords = True
            End With
        End If

        With G_structXML
            For Each structXML In arrKeywords
                drTrans = dtKeywords.NewRow
                drTrans("IdMain") = CInt(n_IDMain)
                drTrans("IdRecipe") = intRecipeId
                drTrans("KeywordName") = structXML.keychild
                drTrans("ParentName") = structXML.keyparent
                drTrans("Derived") = 0
                dtKeywords.Rows.Add(drTrans)
            Next
        End With

        intRecipeId += 1
        Return bNewKeywords

    End Function

    Public Function fctBulkImportXMLRecipe(ByVal dt As DataTable) As String
        Dim strError As String = "XML Recipe Imported"

        Try
            '------------- Bulk Importation -----------------
            'Copy all data to TempTable using SqlBulkCopy
            Dim bulkCopy As SqlBulkCopy
            bulkCopy = New SqlBulkCopy(L_strCnn)
            bulkCopy.DestinationTableName = "EgswBulkImportRecipes"

            If dt.Columns.Contains("IdRecipe") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IdRecipe", "IDRecipe")))
            End If

            If dt.Columns.Contains("IdMain") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IdMain", "IdMain")))
            End If

            If dt.Columns.Contains("ListeType") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("ListeType", "ListeType")))
            End If

            If dt.Columns.Contains("Number") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Number", "Number")))
            End If

            If dt.Columns.Contains("Name") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Name", "Name")))
            End If

            If dt.Columns.Contains("Source") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Source", "Source")))
            End If

            If dt.Columns.Contains("Category") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Category", "Category")))
            End If

            If dt.Columns.Contains("Yield") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Yield", "Yield")))
            End If

            If dt.Columns.Contains("YieldUnitName") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("YieldUnitName", "YieldUnitName")))
            End If

            If dt.Columns.Contains("YieldPercent") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("YieldPercent", "Percent")))
            End If

            If dt.Columns.Contains("LanguageCode") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("LanguageCode", "CodeLang")))
            End If

            If dt.Columns.Contains("srQty") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("srQty", "srQty")))
            End If

            If dt.Columns.Contains("srUnitName") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("srUnitName", "srUnitName")))
            End If

            If dt.Columns.Contains("Picturename") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Picturename", "Picturename")))
            End If

            If dt.Columns.Contains("Note") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Note", "Note")))
            End If

            If dt.Columns.Contains("Remark") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Remark", "Remark")))
            End If

            If dt.Columns.Contains("CoolingTime") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("CoolingTime", "CoolingTime")))
            End If

            If dt.Columns.Contains("HeatingTime") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("HeatingTime", "HeatingTime")))
            End If

            If dt.Columns.Contains("HeatingTemperature") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("HeatingTemperature", "HeatingTemperature")))
            End If

            If dt.Columns.Contains("HeatingMode") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("HeatingMode", "HeatingMode")))
            End If

            If dt.Columns.Contains("CCPDescription") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("CCPDescription", "CCPDescription")))
            End If

            If dt.Columns.Contains("Description") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Description", "Description")))
            End If

            If dt.Columns.Contains("Coeff") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Coeff", "Coeff")))
            End If

            If dt.Columns.Contains("ImposedPrice") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("ImposedPrice", "ImposedPrice")))
            End If

            If dt.Columns.Contains("Tax") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Tax", "Tax")))
            End If

            bulkCopy.WriteToServer(dt)
            bulkCopy.Close()

        Catch ex As Exception
            strError = ex.Message
        End Try
        Return strError
    End Function

    Public Function fctBulkImportXMLIngredients(ByVal dt As DataTable) As String
        Dim strError As String = "XML Ing Imported"
        Try
            '------------- Bulk Importation -----------------
            'Copy all data to TempTable using SqlBulkCopy
            Dim bulkCopy As SqlBulkCopy
            bulkCopy = New SqlBulkCopy(L_strCnn)
            bulkCopy.DestinationTableName = "EgswBulkImportRecipesDetails"

            If dt.Columns.Contains("IdMain") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IdMain", "IdMain")))
            End If

            If dt.Columns.Contains("IdRecipe") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IdRecipe", "IdRecipe")))
            End If

            If dt.Columns.Contains("Position") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Position", "Position")))
            End If

            ''If dt.Columns.Contains("ItemListeType") Then
            ''    bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("ItemListeType", "ItemListeType")))
            ''End If

            If dt.Columns.Contains("Number") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Number", "Number")))
            End If

            If dt.Columns.Contains("Name") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Name", "Name")))
            End If

            If dt.Columns.Contains("Quantity") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Quantity", "Quantity")))
            End If

            If dt.Columns.Contains("UnitName") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("UnitName", "UnitName")))
            End If

            If dt.Columns.Contains("Complement") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Complement", "Complement")))
            End If

            If dt.Columns.Contains("Preparation") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Preparation", "Preparation")))
            End If

            If dt.Columns.Contains("Wastage1") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Wastage1", "Wastage1")))
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

            bulkCopy.WriteToServer(dt)
            bulkCopy.Close()

        Catch ex As Exception
            strError = ex.Message
        End Try
        Return strError
    End Function

    Public Function fctBulkImportXMLRecipeTrans(ByVal dt As DataTable) As String
        Dim strError As String = "Imported"
        Try
            '------------- Bulk Importation -----------------
            'Copy all data to TempTable using SqlBulkCopy
            Dim bulkCopy As SqlBulkCopy
            bulkCopy = New SqlBulkCopy(L_strCnn)
            bulkCopy.DestinationTableName = "EgswBulkImportRecipesTrans"

            If dt.Columns.Contains("IdMain") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IdMain", "IdMain")))
            End If

            If dt.Columns.Contains("IdRecipe") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IdRecipe", "IdRecipe")))
            End If

            If dt.Columns.Contains("Name") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Name", "Name")))
            End If

            If dt.Columns.Contains("Note") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Note", "Note")))
            End If

            If dt.Columns.Contains("Remark") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Remark", "Remark")))
            End If

            If dt.Columns.Contains("CCPDescription") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("CCPDescription", "CCPDescription")))
            End If

            If dt.Columns.Contains("Description") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Description", "Description")))
            End If

            If dt.Columns.Contains("CodeDictionary") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("CodeDictionary", "CodeDictionary")))
            End If

            If dt.Columns.Contains("CodeTrans") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("CodeTrans", "CodeTrans")))
            End If

            bulkCopy.WriteToServer(dt)
            bulkCopy.Close()

        Catch ex As Exception
            strError = ex.Message
        End Try
        Return strError
    End Function

    Public Function fctBulkImportXMLIngTrans(ByVal dt As DataTable) As String
        Dim strError As String = "Imported"
        Try
            '------------- Bulk Importation -----------------
            'Copy all data to TempTable using SqlBulkCopy
            Dim bulkCopy As SqlBulkCopy
            bulkCopy = New SqlBulkCopy(L_strCnn)
            bulkCopy.DestinationTableName = "EgswBulkImportRecipesDetailsTrans"

            If dt.Columns.Contains("IdMain") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IdMain", "IdMain")))
            End If

            If dt.Columns.Contains("IdRecipe") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IdRecipe", "IdRecipe")))
            End If

            If dt.Columns.Contains("Position") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Position", "Position")))
            End If

            If dt.Columns.Contains("Name") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Name", "Name")))
            End If

            If dt.Columns.Contains("Complement") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Complement", "Complement")))
            End If

            If dt.Columns.Contains("Preparation") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Preparation", "Preparation")))
            End If

            If dt.Columns.Contains("CodeDictionary") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("CodeDictionary", "CodeDictionary")))
            End If

            If dt.Columns.Contains("CodeTrans") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("CodeTrans", "CodeTrans")))
            End If

            bulkCopy.WriteToServer(dt)
            bulkCopy.Close()

        Catch ex As Exception
            strError = ex.Message
        End Try
        Return strError
    End Function

    Public Function fctBulkImportXMLKeywords(ByVal dt As DataTable) As String
        Dim strError As String = "Imported"
        Try
            '------------- Bulk Importation -----------------
            'Copy all data to TempTable using SqlBulkCopy
            Dim bulkCopy As SqlBulkCopy
            bulkCopy = New SqlBulkCopy(L_strCnn)
            bulkCopy.DestinationTableName = "EgswBulkImportRecipeKeywords"

            If dt.Columns.Contains("IdMain") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IdMain", "IdMain")))
            End If

            If dt.Columns.Contains("IdRecipe") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IdRecipe", "IdRecipe")))
            End If

            If dt.Columns.Contains("KeywordName") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("KeywordName", "KeywordName")))
            End If

            If dt.Columns.Contains("ParentName") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("ParentName", "ParentName")))
            End If

            If dt.Columns.Contains("Derived") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Derived", "Derived")))
            End If

            bulkCopy.WriteToServer(dt)
            bulkCopy.Close()

        Catch ex As Exception
            strError = ex.Message
        End Try
        Return strError
    End Function

    Private Function DecodeString(ByVal str As String) As String
        If str Is Nothing Then Return ""

        str = str.Replace("(&amp;R)", "@")
        str = str.Replace("&lt;", "<")
        str = str.Replace("&gt;", ">")
        str = str.Replace("&amp;", "&")
        str = str.Replace("&quot;", """")
        str = str.Replace("&apos;", "'")
        str = str.Replace("�", vbCrLf)
        str = str.Replace("", "")
        Return str
    End Function

    Public Function fctGetCodeDictionary(ByVal intTrans As Integer) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Dim strSQL As String = "select codedictionary from EgswTranslation where code=" & intTrans
        Try
            With cmd
                .Connection = cn
                .CommandText = strSQL
                .CommandType = CommandType.Text
                .Connection.Open()
                .ExecuteNonQuery()
                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
            End With
            Return dt
        Catch ex As Exception
            cmd.Connection.Dispose()
            Return Nothing
            Throw ex
        End Try
    End Function
#End Region

#Region " TXC "

    Public Function fctParseTXC(ByVal strFileName As String, ByRef dtRecipe As DataTable, _
                                ByRef dt As DataTable, ByRef dtRecipeTXC As DataTable) As DataTable
        Dim oFile As System.IO.File
        Dim oRead As System.IO.StreamReader
        Dim strText As String
        Dim strRt As String
        Dim ctr As Integer
        Dim ctrRecord As Integer
        Dim aryTXC() As String
        Dim aryDetails() As String
        Dim aryText() As String
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim x, y, z As Integer
        Dim bNewRecord As Boolean = False
        Dim bNewRecipe As Boolean = False
        Dim intPosition As Integer = 1
        Dim intRecipeID As Integer = 1
        Dim ctrIng As Integer = 1
        Dim strPreparation As String

        strRt = Chr(13) & Chr(10)
        Dim reader As StreamReader
        reader = New StreamReader(strFileName, True)

        strText = reader.ReadToEnd
        aryText = Split(strText, "version=")

        For x = 1 To aryText.Length - 1
            aryTXC = Split(aryText(x), strRt)

            If aryTXC(0) = "1" Then 'version 1
                Do 'recipe
                    Select Case i
                        Case 0 : G_structTXC1.version = CInt(aryTXC(i))
                        Case 1 : G_structTXC1.recipename = CStr(aryTXC(i))
                        Case 2 : G_structTXC1.recipecategory = CStr(aryTXC(i))
                        Case 3 : G_structTXC1.recipenumber = CStr(aryTXC(i))
                        Case 4 : G_structTXC1.yieldquantity = CInt(aryTXC(i))
                        Case 5 : G_structTXC1.yieldunitname = CStr(aryTXC(i))
                        Case 6 : G_structTXC1.subrecipeunitcode = CInt(aryTXC(i))
                        Case 7 : G_structTXC1.picturename = CStr(aryTXC(i))
                        Case 8 : G_structTXC1.languagecode = CInt(aryTXC(i))
                        Case 9 : G_structTXC1.sourcename = CStr(aryTXC(i))
                    End Select
                    i += 1
                Loop Until aryTXC(i) = "-"


                i = i + 1 'count number of ingredients
                Do
                    ctr += 1
                    i += 1
                Loop Until aryTXC(i) = "-"

                i = i - ctr
                G_structTXC1.preparation = Replace(CStr(aryTXC(i + ctr + 1)), "�", vbCrLf)

                bNewRecipe = fctAddColumnsRecipeTXCV1(G_structTXC1, bNewRecipe, intRecipeID, dtRecipe)

                For y = 0 To ctr - 1 'details
                    aryDetails = Split(aryTXC(i), ",")
                    For j = 0 To UBound(aryDetails)
                        Select Case j
                            Case 0 : G_structTXC1.ingnumber = CStr(aryDetails(j))
                            Case 1 : G_structTXC1.quantity = CInt(aryDetails(j))
                            Case 2 : G_structTXC1.unit = CStr(aryDetails(j))
                            Case 3 : G_structTXC1.item = CStr(aryDetails(j))
                            Case 4 : G_structTXC1.complement = CStr(aryDetails(j))
                        End Select
                    Next
                    bNewRecord = fctAddColumnsTXCV1(G_structTXC1, bNewRecord, intRecipeID, dt, intPosition)
                    intPosition += 1
                    i += 1
                Next
                i = 0
                ctr = 0
                intPosition = 1
                intRecipeID += 1
            End If

            If aryTXC(0) = "2" Then 'version 2
                Do 'recipe
                    Select Case i
                        Case 0 : G_structTXC2.version = CInt(aryTXC(i))
                        Case 1 : G_structTXC2.recipename = CStr(aryTXC(i))
                        Case 2 : G_structTXC2.recipecategory = CStr(aryTXC(i))
                        Case 3 : G_structTXC2.recipenumber = CStr(aryTXC(i))
                        Case 4 : G_structTXC2.yieldquantity = CDbl(aryTXC(i))
                        Case 5 : G_structTXC2.yieldunitname = CStr(aryTXC(i))
                        Case 6 : G_structTXC2.subrecipeunitcode = CStr(aryTXC(i))
                        Case 7 : G_structTXC2.picturename = CStr(aryTXC(i))
                        Case 8 : G_structTXC2.languagecode = CInt(aryTXC(i))
                        Case 9 : G_structTXC2.sourcename = CStr(aryTXC(i))
                    End Select
                    i += 1
                Loop Until aryTXC(i) = "-"


                i = i + 1 'count number of ingredients
                Do
                    ctr += 1
                    i += 1
                Loop Until aryTXC(i) = "-"
                i = i - ctr
                G_structTXC2.preparation = Replace(CStr(aryTXC(i + ctr + 1)), "�", vbCrLf)


                bNewRecipe = fctAddColumnsRecipeTXCV2(G_structTXC2, bNewRecipe, intRecipeID, dtRecipe) 'DLS

                For y = 0 To ctr - 1 'details
                    aryDetails = Split(aryTXC(i), ",")
                    For j = 0 To UBound(aryDetails)
                        Select Case j
                            Case 0 : G_structTXC2.ingnumber = CStr(aryDetails(j))
                            Case 1 : G_structTXC2.quantity = CInt(aryDetails(j))
                            Case 2 : G_structTXC2.unit = CStr(aryDetails(j))
                            Case 3 : G_structTXC2.item = CStr(aryDetails(j))
                            Case 4 : G_structTXC2.complement = CStr(aryDetails(j))
                            Case 5 : G_structTXC2.note = CStr(aryDetails(j))
                        End Select
                    Next
                    bNewRecord = fctAddColumnsTXCV2(G_structTXC2, bNewRecord, intRecipeID, dt, intPosition)
                    intPosition += 1
                    i += 1
                Next
                i = 0
                ctr = 0
                intPosition = 1
                intRecipeID += 1
            End If

            If aryTXC(0) = "3" Then 'version 3
                Do 'recipe
                    Select Case i
                        Case 0 : G_structTXC3.version = CInt(aryTXC(i))
                        Case 1 : G_structTXC3.recipename = CStr(aryTXC(i))
                        Case 2 : G_structTXC3.recipecategory = CStr(aryTXC(i))
                        Case 3 : G_structTXC3.recipenumber = CStr(aryTXC(i))
                        Case 4 : G_structTXC3.yieldquantity = CDbl(aryTXC(i))
                        Case 5 : G_structTXC3.yieldunitname = CStr(aryTXC(i))
                        Case 6 : G_structTXC3.subrecipequantity = CDbl(aryTXC(i))
                        Case 7 : G_structTXC3.subrecipeunitname = CStr(aryTXC(i))
                        Case 8 : G_structTXC3.picturename = CStr(aryTXC(i))
                        Case 9 : G_structTXC3.languagecode = CInt(aryTXC(i))
                        Case 10 : G_structTXC3.sourcename = CStr(aryTXC(i))
                    End Select
                    i += 1
                Loop Until aryTXC(i) = "-"



                i = i + 1 'count number of ingredients
                Do
                    ctr += 1
                    i += 1
                Loop Until aryTXC(i) = "-"

                i = i - ctr
                G_structTXC3.preparation = Replace(CStr(aryTXC(i + ctr + 1)), "�", vbCrLf)


                bNewRecipe = fctAddColumnsRecipeTXCV3(G_structTXC3, bNewRecipe, intRecipeID, dtRecipeTXC)

                'i = i + 1
                Do
                    For j = 0 To 14
                        Select Case j
                            Case 0
                                G_structTXC3.ingnumber = CStr(aryTXC(i))
                                i += 1
                            Case 1
                                G_structTXC3.quantity = fctNullToZeroDBL(aryTXC(i))
                                i += 1
                            Case 2
                                G_structTXC3.unit = CStr(aryTXC(i))
                                i += 1
                            Case 3
                                G_structTXC3.item = CStr(aryTXC(i))
                                i += 1
                            Case 4
                                G_structTXC3.complement = CStr(aryTXC(i))
                                i += 1
                            Case 5
                                G_structTXC3.note = CStr(aryTXC(i))
                                i += 1
                            Case 6
                                G_structTXC3.price = fctNullToZeroDBL(aryTXC(i))
                                i += 1
                            Case 7
                                G_structTXC3.priceunit = CStr(aryTXC(i))
                                i += 1
                            Case 8
                                G_structTXC3.wastage1 = fctNullToZero(aryTXC(i))
                                i += 1
                            Case 9
                                G_structTXC3.wastage2 = fctNullToZero(aryTXC(i))
                                i += 1
                            Case 10
                                G_structTXC3.wastage3 = fctNullToZero(aryTXC(i))
                                i += 1
                            Case 11
                                G_structTXC3.wastage4 = fctNullToZero(aryTXC(i))
                                i += 1
                            Case 12
                                G_structTXC3.ingcategory = CStr(aryTXC(i))
                                i += 1
                            Case 13
                                G_structTXC3.ingsupplier = CStr(aryTXC(i))
                                i += 1
                        End Select
                    Next
                    bNewRecord = fctAddColumnsTXCV3(G_structTXC3, bNewRecord, intRecipeID, dt, intPosition)
                    intPosition += 1
                    ctrIng += 1
                Loop Until aryTXC(i) = "-"

                i += 1
                ctr = 0
                intPosition = 1
                intRecipeID += 1

                ''Try
                ''    Do
                'strPreparation = aryTXC(i)
                ''    i += 1
                ''        Loop While aryTXC(i) <> ""
                ''    Catch ex As Exception

                ''End Try

                ''For z = 0 To ctrIng - 2
                'G_structTXC3.note = strPreparation
                ''Next
            End If
        Next

        Return dt
    End Function

    Private Function fctAddColumnsRecipeTXCV1(ByVal structV1 As sTXC1, ByVal bNewRecord As Boolean, ByVal intRecipeID As Integer, _
                                              ByRef dtRecipe As DataTable) As Boolean

        Dim dr As DataRow

        If bNewRecord = False Then
            dtRecipe.Columns.Add(New DataColumn("IdRecipe", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("IdMain", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("Version", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("Type", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("RecipeName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("RecipeCategory", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("RecipeNumber", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("YieldQuantity", System.Type.GetType("System.Double")))
            dtRecipe.Columns.Add(New DataColumn("YieldUnitName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("SubRecipeUnitCode", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("PictureName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("SourceName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("LanguageCode", System.Type.GetType("System.Int32")))
            bNewRecord = True
        End If

        dr = dtRecipe.NewRow()
        dr("IdRecipe") = intRecipeID
        dr("IdMain") = CInt(n_IDMain)
        dr("Version") = structV1.version
        dr("Type") = 8
        dr("RecipeName") = structV1.recipename
        dr("RecipeCategory") = structV1.recipecategory
        dr("RecipeNumber") = structV1.recipenumber
        dr("YieldQuantity") = structV1.yieldquantity
        dr("YieldUnitName") = structV1.yieldunitname
        dr("SubRecipeUnitCode") = structV1.subrecipeunitcode
        dr("PictureName") = structV1.picturename
        dr("SourceName") = structV1.sourcename
        dr("LanguageCode") = structV1.languagecode
        dtRecipe.Rows.Add(dr)

        Return bNewRecord
    End Function

    Private Function fctAddColumnsTXCV1(ByVal structV1 As sTXC1, ByVal bNewRow As Boolean, ByVal intRecipeID As Integer, ByRef dt As DataTable, Optional ByVal Position As Integer = 1) As Boolean

        Dim dr As DataRow

        If bNewRow = False Then
            dt.Columns.Add(New DataColumn("IdMain", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("IdRecipe", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Version", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Type", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("RecipeName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("RecipeCategory", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("RecipeNumber", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("YieldQuantity", System.Type.GetType("System.Double")))
            dt.Columns.Add(New DataColumn("YieldUnitName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("SubRecipeUnitCode", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("PictureName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("SourceName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("IngredientNumber", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Position", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Quantity", System.Type.GetType("System.Double")))
            dt.Columns.Add(New DataColumn("Unit", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Item", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Complement", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Preparation", System.Type.GetType("System.String")))
            bNewRow = True
        End If

        dr = dt.NewRow()
        dr("IdMain") = CInt(n_IDMain)
        dr("IdRecipe") = intRecipeID
        dr("Version") = structV1.version
        dr("Type") = 8
        dr("RecipeName") = structV1.recipename
        dr("RecipeCategory") = structV1.recipecategory
        dr("RecipeNumber") = structV1.recipenumber
        dr("YieldQuantity") = structV1.yieldquantity
        dr("YieldUnitName") = structV1.yieldunitname
        dr("SubRecipeUnitCode") = structV1.subrecipeunitcode
        dr("PictureName") = structV1.picturename
        dr("SourceName") = structV1.sourcename
        dr("IngredientNumber") = structV1.ingnumber
        dr("Position") = Position
        dr("Quantity") = structV1.quantity
        dr("Unit") = structV1.unit
        dr("Item") = structV1.item
        dr("Complement") = structV1.complement
        dr("Preparation") = structV1.preparation
        dt.Rows.Add(dr)

        Return bNewRow
    End Function

    Private Function fctAddColumnsTXCV2(ByVal structV2 As sTXC2, ByVal bNewRow As Boolean, ByVal intRecipeID As Integer, ByRef dt As DataTable, Optional ByVal Position As Integer = 1) As Boolean

        Dim dr As DataRow

        If bNewRow = False Then
            dt.Columns.Add(New DataColumn("IdMain", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("IdRecipe", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Version", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Type", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("RecipeName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("RecipeCategory", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("RecipeNumber", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("YieldQuantity", System.Type.GetType("System.Double")))
            dt.Columns.Add(New DataColumn("YieldUnitName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("SubRecipeUnitCode", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("PictureName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("SourceName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("IngredientNumber", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Position", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Quantity", System.Type.GetType("System.Double")))
            dt.Columns.Add(New DataColumn("Unit", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Item", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Complement", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Note", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Preparation", System.Type.GetType("System.String")))
            bNewRow = True
        End If

        dr = dt.NewRow()
        dr("IdMain") = CInt(n_IDMain)
        dr("IdRecipe") = intRecipeID
        dr("Version") = structV2.version
        dr("Type") = 8
        dr("RecipeName") = structV2.recipename
        dr("RecipeCategory") = structV2.recipecategory
        dr("RecipeNumber") = structV2.recipenumber
        dr("YieldQuantity") = structV2.yieldquantity
        dr("YieldUnitName") = structV2.yieldunitname
        dr("SubRecipeUnitCode") = structV2.subrecipeunitcode
        dr("PictureName") = structV2.picturename
        dr("SourceName") = structV2.sourcename
        dr("IngredientNumber") = structV2.ingnumber
        dr("Position") = Position
        dr("Quantity") = structV2.quantity
        dr("Unit") = structV2.unit
        dr("Item") = structV2.item
        dr("Complement") = structV2.complement
        dr("Note") = structV2.note
        dr("Preparation") = structV2.preparation
        dt.Rows.Add(dr)

        Return bNewRow
    End Function

    Private Function fctAddColumnsRecipeTXCV2(ByVal structV2 As sTXC2, ByVal bNewRecord As Boolean, ByVal intRecipeID As Integer, _
                                              ByRef dtRecipe As DataTable) As Boolean

        Dim dr As DataRow

        If bNewRecord = False Then
            dtRecipe.Columns.Add(New DataColumn("IdRecipe", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("IdMain", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("Version", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("Type", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("RecipeName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("RecipeCategory", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("RecipeNumber", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("YieldQuantity", System.Type.GetType("System.Double")))
            dtRecipe.Columns.Add(New DataColumn("YieldUnitName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("SubRecipeUnitCode", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("PictureName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("SourceName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("Note", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("LanguageCode", System.Type.GetType("System.Int32")))
            bNewRecord = True
        End If

        dr = dtRecipe.NewRow()
        dr("IdRecipe") = intRecipeID
        dr("IdMain") = CInt(n_IDMain)
        dr("Version") = structV2.version
        dr("Type") = 8
        dr("RecipeName") = structV2.recipename
        dr("RecipeCategory") = structV2.recipecategory
        dr("RecipeNumber") = structV2.recipenumber
        dr("YieldQuantity") = structV2.yieldquantity
        dr("YieldUnitName") = structV2.yieldunitname
        dr("SubRecipeUnitCode") = structV2.subrecipeunitcode
        dr("PictureName") = structV2.picturename
        dr("SourceName") = structV2.sourcename
        dr("Note") = structV2.preparation 'DLS
        dr("LanguageCode") = structV2.languagecode
        dtRecipe.Rows.Add(dr)

        Return bNewRecord
    End Function

    Private Function fctAddColumnsTXCV3(ByVal structV3 As sTXC3, ByVal bNewRow As Boolean, _
                                        ByVal intRecipeID As Integer, _
                                        ByRef dt As DataTable, Optional ByVal Position As Integer = 1) As Boolean

        Dim dr As DataRow

        If bNewRow = False Then
            dt.Columns.Add(New DataColumn("IdMain", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("IdRecipe", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Version", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Type", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("RecipeName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("RecipeCategory", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("RecipeNumber", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("YieldQuantity", System.Type.GetType("System.Double")))
            dt.Columns.Add(New DataColumn("YieldUnitName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("SubRecipeQuantity", System.Type.GetType("System.Double")))
            dt.Columns.Add(New DataColumn("SubRecipeUnitName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("PictureName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("SourceName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("IngredientNumber", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Position", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Quantity", System.Type.GetType("System.Double")))
            dt.Columns.Add(New DataColumn("Unit", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Item", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Complement", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Note", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Price", System.Type.GetType("System.Double")))
            dt.Columns.Add(New DataColumn("PriceUnit", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Wastage1", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Wastage2", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Wastage3", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Wastage4", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("IngredientCategory", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("IngredientSupplier", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Preparation", System.Type.GetType("System.String")))
            bNewRow = True
        End If

        dr = dt.NewRow()
        dr("IdMain") = CInt(n_IDMain)
        dr("IdRecipe") = intRecipeID
        dr("Version") = structV3.version
        dr("Type") = 8
        dr("RecipeName") = structV3.recipename
        dr("RecipeCategory") = structV3.recipecategory
        dr("RecipeNumber") = structV3.recipenumber
        dr("YieldQuantity") = structV3.yieldquantity
        dr("YieldUnitName") = structV3.yieldunitname
        dr("SubRecipeQuantity") = structV3.subrecipequantity
        dr("SubRecipeUnitName") = structV3.subrecipeunitname
        dr("PictureName") = structV3.picturename
        dr("SourceName") = structV3.sourcename
        dr("IngredientNumber") = structV3.ingnumber
        dr("Position") = Position
        dr("Quantity") = structV3.quantity
        dr("Unit") = structV3.unit
        dr("Item") = structV3.item
        dr("Complement") = structV3.complement
        dr("Note") = structV3.note
        dr("Price") = structV3.price
        dr("PriceUnit") = structV3.priceunit
        dr("Wastage1") = structV3.wastage1
        dr("Wastage2") = structV3.wastage2
        dr("Wastage3") = structV3.wastage3
        dr("Wastage4") = structV3.wastage4
        dr("IngredientCategory") = structV3.ingcategory
        dr("IngredientSupplier") = structV3.ingsupplier
        dr("Preparation") = ""
        dt.Rows.Add(dr)

        Return bNewRow
    End Function

    Private Function fctAddColumnsRecipeTXCV3(ByVal structV3 As sTXC3, ByVal bNewRecord As Boolean, _
                                              ByVal intRecipeID As Integer, _
                                              ByRef dtRecipeTXC As DataTable) As Boolean

        Dim dr As DataRow

        If bNewRecord = False Then
            dtRecipeTXC.Columns.Add(New DataColumn("IdRecipe", System.Type.GetType("System.Int32")))
            dtRecipeTXC.Columns.Add(New DataColumn("IdMain", System.Type.GetType("System.Int32")))
            dtRecipeTXC.Columns.Add(New DataColumn("Version", System.Type.GetType("System.Int32")))
            dtRecipeTXC.Columns.Add(New DataColumn("Type", System.Type.GetType("System.Int32")))
            dtRecipeTXC.Columns.Add(New DataColumn("RecipeName", System.Type.GetType("System.String")))
            dtRecipeTXC.Columns.Add(New DataColumn("RecipeCategory", System.Type.GetType("System.String")))
            dtRecipeTXC.Columns.Add(New DataColumn("RecipeNumber", System.Type.GetType("System.String")))
            dtRecipeTXC.Columns.Add(New DataColumn("YieldQuantity", System.Type.GetType("System.Double")))
            dtRecipeTXC.Columns.Add(New DataColumn("YieldUnitName", System.Type.GetType("System.String")))
            dtRecipeTXC.Columns.Add(New DataColumn("SubRecipeQuantity", System.Type.GetType("System.Double")))
            dtRecipeTXC.Columns.Add(New DataColumn("SubRecipeUnitName", System.Type.GetType("System.String")))
            dtRecipeTXC.Columns.Add(New DataColumn("PictureName", System.Type.GetType("System.String")))
            dtRecipeTXC.Columns.Add(New DataColumn("SourceName", System.Type.GetType("System.String")))
            dtRecipeTXC.Columns.Add(New DataColumn("Note", System.Type.GetType("System.String")))
            dtRecipeTXC.Columns.Add(New DataColumn("LanguageCode", System.Type.GetType("System.Int32")))
            bNewRecord = True
        End If

        dr = dtRecipeTXC.NewRow()
        dr("IdRecipe") = intRecipeID
        dr("IdMain") = CInt(n_IDMain)
        dr("Version") = structV3.version
        dr("Type") = 8
        dr("RecipeName") = structV3.recipename
        dr("RecipeCategory") = structV3.recipecategory
        dr("RecipeNumber") = structV3.recipenumber
        dr("YieldQuantity") = structV3.yieldquantity
        dr("YieldUnitName") = structV3.yieldunitname
        dr("SubRecipeQuantity") = structV3.subrecipequantity
        dr("SubRecipeUnitName") = structV3.subrecipeunitname
        dr("PictureName") = structV3.picturename
        dr("SourceName") = structV3.sourcename
        dr("Note") = structV3.preparation 'DLS
        dr("LanguageCode") = structV3.languagecode
        dtRecipeTXC.Rows.Add(dr)

        Return bNewRecord
    End Function

    Public Function fctBulkImportTXCRecipe(ByVal dt As DataTable) As String
        Dim strError As String = "TXC Recipe Imported"

        Try
            '------------- Bulk Importation -----------------
            'Copy all data to TempTable using SqlBulkCopy
            Dim bulkCopy As SqlBulkCopy
            bulkCopy = New SqlBulkCopy(L_strCnn)
            bulkCopy.DestinationTableName = "EgswBulkImportRecipes"

            If dt.Columns.Contains("IdRecipe") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IdRecipe", "IDRecipe")))
            End If

            If dt.Columns.Contains("IdMain") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IdMain", "IdMain")))
            End If

            If dt.Columns.Contains("Type") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Type", "ListeType")))
            End If

            If dt.Columns.Contains("RecipeNumber") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("RecipeNumber", "Number")))
            End If

            If dt.Columns.Contains("RecipeName") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("RecipeName", "Name")))
            End If

            If dt.Columns.Contains("SourceName") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("SourceName", "Source")))
            End If

            If dt.Columns.Contains("RecipeCategory") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("RecipeCategory", "Category")))
            End If

            If dt.Columns.Contains("YieldQuantity") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("YieldQuantity", "Yield")))
            End If

            If dt.Columns.Contains("YieldUnitName") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("YieldUnitName", "YieldUnitName")))
            End If

            If dt.Columns.Contains("SubRecipeQuantity") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("SubRecipeQuantity", "srQty")))
            End If

            If dt.Columns.Contains("SubRecipeUnitName") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("SubRecipeUnitName", "srUnitName")))
            End If

            If dt.Columns.Contains("PictureName") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("PictureName", "Picturename")))
            End If

            If dt.Columns.Contains("Note") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Note", "Note")))
            End If

            If dt.Columns.Contains("LanguageCode") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("LanguageCode", "CodeLang")))
            End If

            bulkCopy.WriteToServer(dt)
            bulkCopy.Close()

        Catch ex As Exception
            strError = ex.Message
        End Try
        Return strError
    End Function

    Public Function fctBulkImportTXCIngredients(ByVal dt As DataTable) As String
        Dim strError As String = "TXC Ing Imported"
        Try
            '------------- Bulk Importation -----------------
            'Copy all data to TempTable using SqlBulkCopy
            Dim bulkCopy As SqlBulkCopy
            bulkCopy = New SqlBulkCopy(L_strCnn)
            bulkCopy.DestinationTableName = "EgswBulkImportRecipesDetails"

            If dt.Columns.Contains("IdMain") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IdMain", "IdMain")))
            End If

            If dt.Columns.Contains("IdRecipe") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IdRecipe", "IdRecipe")))
            End If

            '---- No Type Indicated DLS October 29 2008------
            ''If dt.Columns.Contains("Type") Then
            ''    bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Type", "ItemListeType")))
            ''End If

            If dt.Columns.Contains("IngredientNumber") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IngredientNumber", "Number")))
            End If

            If dt.Columns.Contains("Position") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Position", "Position")))
            End If

            If dt.Columns.Contains("Item") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Item", "Name")))
            End If

            If dt.Columns.Contains("Quantity") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Quantity", "Quantity")))
            End If

            If dt.Columns.Contains("Unit") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Unit", "UnitName")))
            End If

            If dt.Columns.Contains("Complement") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Complement", "Complement")))
            End If

            ''If dt.Columns.Contains("Preparation") Then
            ''    bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Preparation", "Preparation")))
            ''End If

            If dt.Columns.Contains("Wastage1") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Wastage1", "Wastage1")))
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

            If dt.Columns.Contains("IngredientCategory") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IngredientCategory", "IngredientCategory")))
            End If

            If dt.Columns.Contains("IngredientSupplier") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IngredientSupplier", "IngredientSupplier")))
            End If

            If dt.Columns.Contains("Price") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Price", "IngredientPrice")))
            End If

            If dt.Columns.Contains("PriceUnit") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("PriceUnit", "IngredientPriceUnit")))
            End If

            bulkCopy.WriteToServer(dt)
            bulkCopy.Close()

        Catch ex As Exception
            strError = ex.Message
        End Try
        Return strError
    End Function

#End Region

#Region " TXS "

    Public Function fctParseTXS(ByVal strFileName As String, ByRef dtRecipe As DataTable, _
                                ByRef dt As DataTable) As DataTable

        Dim oFile As System.IO.File
        Dim oRead As System.IO.StreamReader
        Dim strText As String
        Dim strRt As String
        Dim ctr As Integer
        Dim ctrRecord As Integer
        Dim aryTXS() As String
        Dim aryDetails() As String
        Dim aryText() As String
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim x, y As Integer
        Dim bNewRecord As Boolean = False
        Dim bNewRecipe As Boolean = False
        Dim intPosition As Integer = 1
        Dim intRecipeID As Integer = 1

        strRt = Chr(13) & Chr(10)
        Dim reader As StreamReader
        reader = New StreamReader(strFileName, System.Text.Encoding.Default, True)

        strText = reader.ReadToEnd
        aryText = Split(strText, "version=")

        For x = 1 To aryText.Length - 1
            aryTXS = Split(aryText(x), strRt)

            If aryTXS(0) = "5" Then 'version 5
                Do 'recipe
                    Select Case i
                        Case 0 : G_structTXS5.version = CInt(aryTXS(i))
                        Case 1 : G_structTXS5.recipename = CStr(aryTXS(i))
                        Case 2 : G_structTXS5.recipecategory = CStr(aryTXS(i))
                        Case 3 : G_structTXS5.recipenumber = CStr(aryTXS(i))
                        Case 4 : G_structTXS5.yieldquantity = CInt(aryTXS(i))
                        Case 5 : G_structTXS5.yieldunitname = CStr(aryTXS(i))
                        Case 6 : G_structTXS5.subrecipequantity = CInt(aryTXS(i))
                        Case 7 : G_structTXS5.subrecipeunitname = CStr(aryTXS(i))
                        Case 8 : G_structTXS5.picturename = CStr(aryTXS(i))
                        Case 9 : G_structTXS5.languagecode = CInt(aryTXS(i))
                        Case 10 : G_structTXS5.sourcename = CStr(aryTXS(i))
                    End Select
                    i += 1
                Loop Until aryTXS(i) = "-"


                i = i + 1 'count number of ingredients
                Do
                    ctr += 1
                    i += 1
                Loop Until aryTXS(i) = "-"

                i = i - ctr
                G_structTXS5.preparation = CStr(aryTXS(i + ctr + 1))
                G_structTXS5.preparation = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS5.preparation).Replace(",", "").Replace("@$@", ","))
                G_structTXS5.preparation = Trim(G_structTXS5.preparation)
                G_structTXS5.preparation = G_structTXS5.preparation.Replace("�", vbCrLf)

                bNewRecipe = fctAddColumnsRecipeTXSV5(G_structTXS5, bNewRecipe, intRecipeID, dtRecipe)

                For y = 0 To ctr - 1 'details
                    aryTXS(i) = fctReplaceComma(aryTXS(i))
                    aryDetails = Split(aryTXS(i) & ",", ",")
                    For j = 0 To UBound(aryDetails)
                        aryDetails(j) = aryDetails(j).Replace("@$@", ",")
                        Select Case j
                            Case 0 : G_structTXS5.ingnumber = CStr(aryDetails(j))
                            Case 1 : G_structTXS5.quantity = CInt(aryDetails(j))
                            Case 2 : G_structTXS5.unit = CStr(aryDetails(j))
                            Case 3
                                G_structTXS5.item = CStr(aryDetails(j))
                                G_structTXS5.item = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS5.item).Replace(",", "").Replace("@$@", ","))
                            Case 4
                                G_structTXS5.complement = CStr(aryDetails(j))
                                G_structTXS5.complement = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS5.complement).Replace(",", "").Replace("@$@", ","))
                            Case 5 : G_structTXS5.note = CStr(aryDetails(j))
                            Case 6 : G_structTXS5.price = CDbl(aryDetails(j))
                            Case 7 : G_structTXS5.priceunit = CStr(aryDetails(j))
                            Case 8 : G_structTXS5.wastage1 = CInt(aryDetails(j))
                            Case 9 : G_structTXS5.wastage2 = CInt(aryDetails(j))
                            Case 10 : G_structTXS5.wastage3 = CInt(aryDetails(j))
                            Case 11 : G_structTXS5.wastage4 = CInt(aryDetails(j))
                            Case 12
                                G_structTXS5.ingcategory = CStr(aryDetails(j))
                                G_structTXS5.ingcategory = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS5.ingcategory).Replace(",", "").Replace("@$@", ","))
                                G_structTXS5.ingcategory = Trim(G_structTXS5.ingcategory)
                            Case 13
                                G_structTXS5.ingsupplier = CStr(aryDetails(j))
                                G_structTXS5.ingsupplier = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS5.ingsupplier).Replace(",", "").Replace("@$@", ","))
                                G_structTXS5.ingsupplier = Trim(G_structTXS5.ingsupplier)
                        End Select
                    Next
                    bNewRecord = fctAddColumnsTXSV5(G_structTXS5, bNewRecord, intRecipeID, dt, intPosition)
                    intPosition += 1
                    i += 1
                Next
                i = 0
                ctr = 0
                intPosition = 1
                intRecipeID += 1
            End If

            If aryTXS(0) = "6" Then 'version 6
                Do 'recipe
                    Select Case i
                        Case 0 : G_structTXS6.version = CInt(aryTXS(i))
                        Case 1 : G_structTXS6.recipename = CStr(aryTXS(i))
                        Case 2 : G_structTXS6.sourceTXS = CStr(aryTXS(i))
                        Case 3 : G_structTXS6.recipecategory = CStr(aryTXS(i))
                        Case 4 : G_structTXS6.recipenumber = CStr(aryTXS(i))
                        Case 5 : G_structTXS6.yieldquantity = CInt(aryTXS(i))
                        Case 6 : G_structTXS6.yieldunitname = CStr(aryTXS(i))
                        Case 7 : G_structTXS6.subrecipequantity = CInt(aryTXS(i))
                        Case 8 : G_structTXS6.subrecipeunitname = CStr(aryTXS(i))
                        Case 9 : G_structTXS6.picturename = CStr(aryTXS(i))
                        Case 10 : G_structTXS6.languagecode = CInt(aryTXS(i))
                        Case 11 : G_structTXS6.sourcename = CStr(aryTXS(i))
                    End Select
                    i += 1
                Loop Until aryTXS(i) = "-"



                i = i + 1 'count number of ingredients
                Do
                    ctr += 1
                    i += 1
                Loop Until aryTXS(i) = "-"

                i = i - ctr
                G_structTXS6.preparation = CStr(aryTXS(i + ctr + 1))
                G_structTXS6.preparation = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS6.preparation).Replace(",", "").Replace("@$@", ","))
                G_structTXS6.preparation = Trim(G_structTXS6.preparation)
                G_structTXS6.preparation = G_structTXS6.preparation.Replace("�", vbCrLf)

                bNewRecipe = fctAddColumnsRecipeTXSV6(G_structTXS6, bNewRecipe, intRecipeID, dtRecipe)

                For y = 0 To ctr - 1 'details
                    aryTXS(i) = fctReplaceComma(aryTXS(i))
                    aryDetails = Split(aryTXS(i) & ",", ",")
                    For j = 0 To UBound(aryDetails)
                        aryDetails(j) = aryDetails(j).Replace("@$@", ",")
                        Select Case j
                            Case 0 : G_structTXS6.ingnumber = CStr(aryDetails(j))
                            Case 1 : G_structTXS6.quantity = CInt(aryDetails(j))
                            Case 2 : G_structTXS6.unit = CStr(aryDetails(j))
                            Case 3
                                G_structTXS6.item = CStr(aryDetails(j))
                                G_structTXS6.item = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS6.item).Replace(",", "").Replace("@$@", ","))
                            Case 4
                                G_structTXS6.complement = CStr(aryDetails(j))
                                G_structTXS6.complement = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS6.complement).Replace(",", "").Replace("@$@", ","))
                            Case 5 : G_structTXS6.note = CStr(aryDetails(j))
                            Case 6 : G_structTXS6.price = CDbl(aryDetails(j))
                            Case 7 : G_structTXS6.priceunit = CStr(aryDetails(j))
                            Case 8 : G_structTXS6.wastage1 = CInt(aryDetails(j))
                            Case 9 : G_structTXS6.wastage2 = CInt(aryDetails(j))
                            Case 10 : G_structTXS6.wastage3 = CInt(aryDetails(j))
                            Case 11 : G_structTXS6.wastage4 = CInt(aryDetails(j))
                            Case 12
                                G_structTXS6.ingcategory = CStr(aryDetails(j))
                                G_structTXS6.ingcategory = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS6.ingcategory).Replace(",", "").Replace("@$@", ","))
                                G_structTXS6.ingcategory = Trim(G_structTXS6.ingcategory)
                            Case 13
                                G_structTXS6.ingsupplier = CStr(aryDetails(j))
                                G_structTXS6.ingsupplier = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS6.ingsupplier).Replace(",", "").Replace("@$@", ","))
                                G_structTXS6.ingsupplier = Trim(G_structTXS6.ingsupplier)
                        End Select
                    Next
                    bNewRecord = fctAddColumnsTXSV6(G_structTXS6, bNewRecord, intRecipeID, dt, intPosition)
                    intPosition += 1
                    i += 1
                Next
                i = 0
                ctr = 0
                intPosition = 1
                intRecipeID += 1
            End If

            If aryTXS(0) = "7" Then 'version 7
                Do 'recipe
                    Select Case i
                        Case 0 : G_structTXS7.version = CInt(aryTXS(i))
                        Case 1 : G_structTXS7.type = CInt(aryTXS(i))
                        Case 2 : G_structTXS7.recipename = CStr(aryTXS(i))
                        Case 3 : G_structTXS7.sourceTXS = CStr(aryTXS(i))
                        Case 4 : G_structTXS7.recipecategory = CStr(aryTXS(i))
                        Case 5 : G_structTXS7.recipenumber = CStr(aryTXS(i))
                        Case 6 : G_structTXS7.yieldquantity = CInt(aryTXS(i))
                        Case 7 : G_structTXS7.yieldunitname = CStr(aryTXS(i))
                        Case 8 : G_structTXS7.subrecipequantity = CInt(aryTXS(i))
                        Case 9 : G_structTXS7.subrecipeunitname = CStr(aryTXS(i))
                        Case 10 : G_structTXS7.picturename = CStr(aryTXS(i))
                        Case 11 : G_structTXS7.languagecode = CInt(aryTXS(i))
                        Case 12 : G_structTXS7.sourcename = CStr(aryTXS(i))
                    End Select
                    i += 1
                Loop Until aryTXS(i) = "-"


                i = i + 1 'count number of ingredients
                Do
                    ctr += 1
                    i += 1
                Loop Until aryTXS(i) = "-"

                i = i - ctr
                G_structTXS7.preparation = CStr(aryTXS(i + ctr + 1))
                G_structTXS7.preparation = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS7.preparation).Replace(",", "").Replace("@$@", ","))
                G_structTXS7.preparation = Trim(G_structTXS7.preparation)
                G_structTXS7.preparation = G_structTXS7.preparation.Replace("�", vbCrLf)

                bNewRecipe = fctAddColumnsRecipeTXSV7(G_structTXS7, bNewRecipe, intRecipeID, dtRecipe)

                For y = 0 To ctr - 1 'details
                    aryTXS(i) = fctReplaceComma(aryTXS(i))
                    aryDetails = Split(aryTXS(i) & ",", ",")
                    For j = 0 To UBound(aryDetails)
                        aryDetails(j) = aryDetails(j).Replace("@$@", ",")
                        Select Case j
                            Case 0 : G_structTXS7.ingnumber = CStr(aryDetails(j))
                            Case 1 : G_structTXS7.quantity = CInt(aryDetails(j))
                            Case 2 : G_structTXS7.unit = CStr(aryDetails(j))
                            Case 3
                                G_structTXS7.item = CStr(aryDetails(j))
                                G_structTXS7.item = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS7.item).Replace(",", "").Replace("@$@", ","))
                            Case 4
                                G_structTXS7.complement = CStr(aryDetails(j))
                                G_structTXS7.complement = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS7.complement).Replace(",", "").Replace("@$@", ","))
                            Case 5 : G_structTXS7.note = CStr(aryDetails(j))
                            Case 6 : G_structTXS7.price = CDbl(aryDetails(j))
                            Case 7 : G_structTXS7.priceunit = CStr(aryDetails(j))
                            Case 8 : G_structTXS7.wastage1 = CInt(aryDetails(j))
                            Case 9 : G_structTXS7.wastage2 = CInt(aryDetails(j))
                            Case 10 : G_structTXS7.wastage3 = CInt(aryDetails(j))
                            Case 11 : G_structTXS7.wastage4 = CInt(aryDetails(j))
                            Case 12
                                G_structTXS7.ingcategory = CStr(aryDetails(j))
                                G_structTXS7.ingcategory = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS7.ingcategory).Replace(",", "").Replace("@$@", ","))
                                G_structTXS7.ingcategory = Trim(G_structTXS7.ingcategory)
                            Case 13
                                G_structTXS7.ingsupplier = CStr(aryDetails(j))
                                G_structTXS7.ingsupplier = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS7.ingsupplier).Replace(",", "").Replace("@$@", ","))
                                G_structTXS7.ingsupplier = Trim(G_structTXS7.ingsupplier)
                        End Select
                    Next
                    bNewRecord = fctAddColumnsTXSV7(G_structTXS7, bNewRecord, intRecipeID, dt, intPosition)
                    intPosition += 1
                    i += 1
                Next
                i = 0
                ctr = 0
                intPosition = 1
                intRecipeID += 1
            End If

            If aryTXS(0) = "9" Then 'version 9
                Do 'recipe
                    Select Case i
                        Case 0 : G_structTXS9.version = CInt(aryTXS(i))
                        Case 1 : G_structTXS9.type = CInt(aryTXS(i))
                        Case 2 : G_structTXS9.recipename = CStr(aryTXS(i))
                        Case 3 : G_structTXS9.sourceTXS = CStr(aryTXS(i))
                        Case 4 : G_structTXS9.recipecategory = CStr(aryTXS(i))
                        Case 5 : G_structTXS9.recipenumber = CStr(aryTXS(i))
                        Case 6 : G_structTXS9.yieldquantity = CDbl(aryTXS(i))
                        Case 7 : G_structTXS9.yieldunitname = CStr(aryTXS(i))
                        Case 8 : G_structTXS9.subrecipequantity = CDbl(aryTXS(i))
                        Case 9 : G_structTXS9.subrecipeunitname = CStr(aryTXS(i))
                        Case 10 : G_structTXS9.batchqty = CStr(aryTXS(i))
                        Case 11 : G_structTXS9.picturename = CStr(aryTXS(i))
                        Case 12 : G_structTXS9.languagecode = CInt(aryTXS(i))
                        Case 13 : G_structTXS9.sourcename = CStr(aryTXS(i))
                    End Select
                    i += 1
                Loop Until aryTXS(i) = "-"

                If Not aryTXS(i + 1) = "-" Then 'just to make sure that theres an ingredient before doing the loop
                    i = i + 1 'count number of ingredients
                    Do
                        ctr += 1
                        i += 1
                    Loop Until aryTXS(i) = "-"
                Else
                    i = i + 1
                End If                

                i = i - ctr
                G_structTXS9.preparation = CStr(aryTXS(i + ctr + 1))
                G_structTXS9.preparation = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS9.preparation).Replace(",", "").Replace("@$@", ","))
                G_structTXS9.preparation = Trim(G_structTXS9.preparation)
                G_structTXS9.preparation = G_structTXS9.preparation.Replace("�", vbCrLf)


                bNewRecipe = fctAddColumnsRecipeTXSV9(G_structTXS9, bNewRecipe, intRecipeID, dtRecipe)


                For y = 0 To ctr - 1 'details
                    aryTXS(i) = fctReplaceComma(aryTXS(i))
                    aryDetails = Split(aryTXS(i) & ",", ",")
                    For j = 0 To UBound(aryDetails)
                        aryDetails(j) = aryDetails(j).Replace("@$@", ",")
                        Select Case j
                            Case 0 : G_structTXS9.ingnumber = CStr(aryDetails(j))
                            Case 1 : G_structTXS9.quantity = CDbl(aryDetails(j))
                            Case 2 : G_structTXS9.unit = CStr(aryDetails(j))
                            Case 3
                                G_structTXS9.item = CStr(aryDetails(j))
                                G_structTXS9.item = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS9.item).Replace(",", "").Replace("@$@", ","))
                            Case 4
                                G_structTXS9.complement = CStr(aryDetails(j))
                                G_structTXS9.complement = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS9.complement).Replace(",", "").Replace("@$@", ","))
                            Case 5 : G_structTXS9.note = CStr(aryDetails(j))
                            Case 6 : G_structTXS9.price = CDbl(IIf(IsDBNull(aryDetails(j)) Or aryDetails(j) = "", 0.0, aryDetails(j)))
                            Case 7 : G_structTXS9.priceunit = CStr(aryDetails(j))
                            Case 8 : G_structTXS9.wastage1 = CInt(aryDetails(j))
                            Case 9 : G_structTXS9.wastage2 = CInt(aryDetails(j))
                            Case 10 : G_structTXS9.wastage3 = CInt(aryDetails(j))
                            Case 11 : G_structTXS9.wastage4 = CInt(aryDetails(j))
                            Case 12
                                G_structTXS9.ingcategory = CStr(aryDetails(j))
                                G_structTXS9.ingcategory = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS9.ingcategory).Replace(",", "").Replace("@$@", ","))
                                G_structTXS9.ingcategory = Trim(G_structTXS9.ingcategory)
                            Case 13
                                G_structTXS9.ingsupplier = CStr(aryDetails(j))
                                G_structTXS9.ingsupplier = fctRemoveDoubleQuotes(fctReplaceComma(G_structTXS9.ingsupplier).Replace(",", "").Replace("@$@", ","))
                                G_structTXS9.ingsupplier = Trim(G_structTXS9.ingsupplier)
                        End Select
                    Next
                    bNewRecord = fctAddColumnsTXSV9(G_structTXS9, bNewRecord, intRecipeID, dt, intPosition)
                    intPosition += 1
                    i += 1
                Next
                i = 0
                ctr = 0
                intPosition = 1
                intRecipeID += 1
            End If
        Next
        Return dt
    End Function

    Private Function fctAddColumnsTXSV5(ByVal structV5 As sTXS5, ByVal bNewRow As Boolean, ByVal intRecipeID As Integer, _
                                        ByRef dt As DataTable, Optional ByVal Position As Integer = 1) As Boolean
        Dim dr As DataRow

        If bNewRow = False Then
            dt.Columns.Add(New DataColumn("IdMain", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("IdRecipe", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Version", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Type", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("RecipeName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("RecipeCategory", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("RecipeNumber", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("YieldQuantity", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("YieldUnitName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("SubRecipeQuantity", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("SubRecipeUnitName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("PictureName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("SourceName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("IngredientNumber", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Position", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Quantity", System.Type.GetType("System.Double")))
            dt.Columns.Add(New DataColumn("Unit", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Item", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Complement", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Note", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Price", System.Type.GetType("System.Double")))
            dt.Columns.Add(New DataColumn("PriceUnit", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Wastage1", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Wastage2", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Wastage3", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Wastage4", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("IngredientCategory", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("IngredientSupplier", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Preparation", System.Type.GetType("System.String")))
            bNewRow = True
        End If

        dr = dt.NewRow()
        dr("IdMain") = CInt(n_IDMain)
        dr("IdRecipe") = intRecipeID
        dr("Version") = structV5.version
        dr("Type") = 8
        dr("RecipeName") = structV5.recipename
        dr("RecipeCategory") = structV5.recipecategory
        dr("RecipeNumber") = structV5.recipenumber
        dr("YieldQuantity") = structV5.yieldquantity
        dr("YieldUnitName") = structV5.yieldunitname
        dr("SubRecipeQuantity") = structV5.subrecipequantity
        dr("SubRecipeUnitName") = structV5.subrecipeunitname
        dr("PictureName") = structV5.picturename
        dr("SourceName") = structV5.sourcename
        dr("IngredientNumber") = structV5.ingnumber
        dr("Position") = Position
        dr("Quantity") = structV5.quantity
        dr("Unit") = structV5.unit
        dr("Item") = structV5.item
        dr("Complement") = structV5.complement
        dr("Note") = structV5.note
        dr("Price") = structV5.price
        dr("PriceUnit") = structV5.priceunit
        dr("Wastage1") = structV5.wastage1
        dr("Wastage2") = structV5.wastage2
        dr("Wastage3") = structV5.wastage3
        dr("Wastage4") = structV5.wastage4
        dr("IngredientCategory") = structV5.ingcategory
        dr("IngredientSupplier") = structV5.ingsupplier
        dr("Preparation") = structV5.preparation
        dt.Rows.Add(dr)

        Return bNewRow
    End Function

    Private Function fctAddColumnsRecipeTXSV5(ByVal structV5 As sTXS5, ByVal bNewRecord As Boolean, _
                                              ByVal intRecipeID As Integer, ByRef dtRecipe As DataTable) As Boolean

        Dim dr As DataRow

        If bNewRecord = False Then
            dtRecipe.Columns.Add(New DataColumn("IdRecipe", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("IdMain", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("Version", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("Type", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("RecipeName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("RecipeCategory", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("RecipeNumber", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("YieldQuantity", System.Type.GetType("System.Double")))
            dtRecipe.Columns.Add(New DataColumn("YieldUnitName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("SubRecipeQuantity", System.Type.GetType("System.Double")))
            dtRecipe.Columns.Add(New DataColumn("SubRecipeUnitName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("PictureName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("SourceName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("Note", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("LanguageCode", System.Type.GetType("System.Int32")))
            bNewRecord = True
        End If

        dr = dtRecipe.NewRow()
        dr("IdRecipe") = intRecipeID
        dr("IdMain") = CInt(n_IDMain)
        dr("Version") = structV5.version
        dr("Type") = 8
        dr("RecipeName") = structV5.recipename
        dr("RecipeCategory") = structV5.recipecategory
        dr("RecipeNumber") = structV5.recipenumber
        dr("YieldQuantity") = structV5.yieldquantity
        dr("YieldUnitName") = structV5.yieldunitname
        dr("SubRecipeQuantity") = structV5.subrecipequantity
        dr("SubRecipeUnitName") = structV5.subrecipeunitname
        dr("PictureName") = structV5.picturename
        dr("SourceName") = structV5.sourcename
        dr("Note") = structV5.preparation 'DLS
        dr("LanguageCode") = structV5.languagecode
        dtRecipe.Rows.Add(dr)

        Return bNewRecord
    End Function

    Private Function fctAddColumnsTXSV6(ByVal structV6 As sTXS6, ByVal bNewRow As Boolean, ByVal intRecipeID As Integer, _
                                        ByRef dt As DataTable, Optional ByVal Position As Integer = 1) As Boolean
        Dim dr As DataRow

        If bNewRow = False Then
            dt.Columns.Add(New DataColumn("IdMain", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("IdRecipe", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Version", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Type", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("RecipeName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("SourceTXS", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("RecipeCategory", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("RecipeNumber", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("YieldQuantity", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("YieldUnitName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("SubRecipeQuantity", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("SubRecipeUnitName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("PictureName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("SourceName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("IngredientNumber", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Position", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Quantity", System.Type.GetType("System.Double")))
            dt.Columns.Add(New DataColumn("Unit", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Item", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Complement", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Note", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Price", System.Type.GetType("System.Double")))
            dt.Columns.Add(New DataColumn("PriceUnit", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Wastage1", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Wastage2", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Wastage3", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Wastage4", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("IngredientCategory", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("IngredientSupplier", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Preparation", System.Type.GetType("System.String")))
            bNewRow = True
        End If

        dr = dt.NewRow()
        dr("IdMain") = CInt(n_IDMain)
        dr("IdRecipe") = intRecipeID
        dr("Version") = structV6.version
        dr("Type") = 8
        dr("RecipeName") = structV6.recipename
        dr("SourceTXS") = structV6.sourceTXS
        dr("RecipeCategory") = structV6.recipecategory
        dr("RecipeNumber") = structV6.recipenumber
        dr("YieldQuantity") = structV6.yieldquantity
        dr("YieldUnitName") = structV6.yieldunitname
        dr("SubRecipeQuantity") = structV6.subrecipequantity
        dr("SubRecipeUnitName") = structV6.subrecipeunitname
        dr("PictureName") = structV6.picturename
        dr("SourceName") = structV6.sourcename
        dr("IngredientNumber") = structV6.ingnumber
        dr("Position") = Position
        dr("Quantity") = structV6.quantity
        dr("Unit") = structV6.unit
        dr("Item") = structV6.item
        dr("Complement") = structV6.complement
        dr("Note") = structV6.note
        dr("Price") = structV6.price
        dr("PriceUnit") = structV6.priceunit
        dr("Wastage1") = structV6.wastage1
        dr("Wastage2") = structV6.wastage2
        dr("Wastage3") = structV6.wastage3
        dr("Wastage4") = structV6.wastage4
        dr("IngredientCategory") = structV6.ingcategory
        dr("IngredientSupplier") = structV6.ingsupplier
        dr("Preparation") = structV6.preparation
        dt.Rows.Add(dr)

        Return bNewRow
    End Function

    Private Function fctAddColumnsRecipeTXSV6(ByVal structV6 As sTXS6, ByVal bNewRecord As Boolean, _
                                              ByVal intRecipeID As Integer, ByRef dtRecipe As DataTable) As Boolean

        Dim dr As DataRow

        If bNewRecord = False Then
            dtRecipe.Columns.Add(New DataColumn("IdRecipe", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("IdMain", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("Version", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("Type", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("RecipeName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("SourceTXS", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("RecipeCategory", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("RecipeNumber", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("YieldQuantity", System.Type.GetType("System.Double")))
            dtRecipe.Columns.Add(New DataColumn("YieldUnitName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("SubRecipeQuantity", System.Type.GetType("System.Double")))
            dtRecipe.Columns.Add(New DataColumn("SubRecipeUnitName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("PictureName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("SourceName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("Note", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("LanguageCode", System.Type.GetType("System.Int32")))
            bNewRecord = True
        End If

        dr = dtRecipe.NewRow()
        dr("IdRecipe") = intRecipeID
        dr("IdMain") = CInt(n_IDMain)
        dr("Version") = structV6.version
        dr("Type") = 8
        dr("RecipeName") = structV6.recipename
        dr("SourceTXS") = structV6.sourceTXS
        dr("RecipeCategory") = structV6.recipecategory
        dr("RecipeNumber") = structV6.recipenumber
        dr("YieldQuantity") = structV6.yieldquantity
        dr("YieldUnitName") = structV6.yieldunitname
        dr("SubRecipeQuantity") = structV6.subrecipequantity
        dr("SubRecipeUnitName") = structV6.subrecipeunitname
        dr("PictureName") = structV6.picturename
        dr("SourceName") = structV6.sourcename
        dr("Note") = structV6.preparation 'DLS
        dr("LanguageCode") = structV6.languagecode
        dtRecipe.Rows.Add(dr)

        Return bNewRecord
    End Function

    Private Function fctAddColumnsTXSV7(ByVal structV7 As sTXS7, ByVal bNewRow As Boolean, _
                                        ByVal intRecipeID As Integer, ByRef dt As DataTable, Optional ByVal Position As Integer = 1) As Boolean
        Dim dr As DataRow

        If bNewRow = False Then
            dt.Columns.Add(New DataColumn("IdMain", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("IdRecipe", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Version", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Type", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("RecipeName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("SourceTXS", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("RecipeCategory", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("RecipeNumber", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("YieldQuantity", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("YieldUnitName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("SubRecipeQuantity", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("SubRecipeUnitName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("PictureName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("SourceName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("IngredientNumber", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Position", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Quantity", System.Type.GetType("System.Double")))
            dt.Columns.Add(New DataColumn("Unit", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Item", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Complement", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Note", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Price", System.Type.GetType("System.Double")))
            dt.Columns.Add(New DataColumn("PriceUnit", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Wastage1", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Wastage2", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Wastage3", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Wastage4", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("IngredientCategory", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("IngredientSupplier", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Preparation", System.Type.GetType("System.String")))
            bNewRow = True
        End If

        dr = dt.NewRow()
        dr("IdMain") = CInt(n_IDMain)
        dr("IdRecipe") = intRecipeID
        dr("Version") = structV7.version
        dr("Type") = structV7.type
        dr("RecipeName") = structV7.recipename
        dr("SourceTXS") = structV7.sourceTXS
        dr("RecipeCategory") = structV7.recipecategory
        dr("RecipeNumber") = structV7.recipenumber
        dr("YieldQuantity") = structV7.yieldquantity
        dr("YieldUnitName") = structV7.yieldunitname
        dr("SubRecipeQuantity") = structV7.subrecipequantity
        dr("SubRecipeUnitName") = structV7.subrecipeunitname
        dr("PictureName") = structV7.picturename
        dr("SourceName") = structV7.sourcename
        dr("IngredientNumber") = structV7.ingnumber
        dr("Position") = Position
        dr("Quantity") = structV7.quantity
        dr("Unit") = structV7.unit
        dr("Item") = structV7.item
        dr("Complement") = structV7.complement
        dr("Note") = structV7.note
        dr("Price") = structV7.price
        dr("PriceUnit") = structV7.priceunit
        dr("Wastage1") = structV7.wastage1
        dr("Wastage2") = structV7.wastage2
        dr("Wastage3") = structV7.wastage3
        dr("Wastage4") = structV7.wastage4
        dr("IngredientCategory") = structV7.ingcategory
        dr("IngredientSupplier") = structV7.ingsupplier
        dr("Preparation") = structV7.preparation
        dt.Rows.Add(dr)

        Return bNewRow
    End Function

    Private Function fctAddColumnsRecipeTXSV7(ByVal structV7 As sTXS7, ByVal bNewRecord As Boolean, _
                                              ByVal intRecipeID As Integer, ByRef dtRecipe As DataTable) As Boolean

        Dim dr As DataRow

        If bNewRecord = False Then
            dtRecipe.Columns.Add(New DataColumn("IdRecipe", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("IdMain", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("Version", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("Type", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("RecipeName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("SourceTXS", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("RecipeCategory", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("RecipeNumber", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("YieldQuantity", System.Type.GetType("System.Double")))
            dtRecipe.Columns.Add(New DataColumn("YieldUnitName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("SubRecipeQuantity", System.Type.GetType("System.Double")))
            dtRecipe.Columns.Add(New DataColumn("SubRecipeUnitName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("PictureName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("SourceName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("Note", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("LanguageCode", System.Type.GetType("System.Int32")))
            bNewRecord = True
        End If

        dr = dtRecipe.NewRow()
        dr("IdRecipe") = intRecipeID
        dr("IdMain") = CInt(n_IDMain)
        dr("Version") = structV7.version
        dr("Type") = structV7.type
        dr("RecipeName") = structV7.recipename
        dr("SourceTXS") = structV7.sourceTXS
        dr("RecipeCategory") = structV7.recipecategory
        dr("RecipeNumber") = structV7.recipenumber
        dr("YieldQuantity") = structV7.yieldquantity
        dr("YieldUnitName") = structV7.yieldunitname
        dr("SubRecipeQuantity") = structV7.subrecipequantity
        dr("SubRecipeUnitName") = structV7.subrecipeunitname
        dr("PictureName") = structV7.picturename
        dr("SourceName") = structV7.sourcename
        dr("Note") = structV7.preparation 'DLS
        dr("LanguageCode") = structV7.languagecode
        dtRecipe.Rows.Add(dr)

        Return bNewRecord
    End Function

    Private Function fctAddColumnsTXSV9(ByVal structV9 As sTXS9, ByVal bNewRow As Boolean, ByVal intRecipeID As Integer, _
                                        ByRef dt As DataTable, Optional ByVal Position As Integer = 1) As Boolean

        Dim dr As DataRow

        If bNewRow = False Then
            dt.Columns.Add(New DataColumn("IdMain", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("IdRecipe", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Version", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Type", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("RecipeName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("SourceTXS", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("RecipeCategory", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("RecipeNumber", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("YieldQuantity", System.Type.GetType("System.Double")))
            dt.Columns.Add(New DataColumn("YieldUnitName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("SubRecipeQuantity", System.Type.GetType("System.Double")))
            dt.Columns.Add(New DataColumn("SubRecipeUnitName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("BatchQty", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("PictureName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("SourceName", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("IngredientNumber", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Position", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Quantity", System.Type.GetType("System.Double")))
            dt.Columns.Add(New DataColumn("Unit", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Item", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Complement", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Note", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Price", System.Type.GetType("System.Double")))
            dt.Columns.Add(New DataColumn("PriceUnit", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Wastage1", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Wastage2", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Wastage3", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("Wastage4", System.Type.GetType("System.Int32")))
            dt.Columns.Add(New DataColumn("IngredientCategory", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("IngredientSupplier", System.Type.GetType("System.String")))
            dt.Columns.Add(New DataColumn("Preparation", System.Type.GetType("System.String")))
            bNewRow = True
        End If

        dr = dt.NewRow()
        dr("IdMain") = CInt(n_IDMain)
        dr("IdRecipe") = intRecipeID
        dr("Version") = structV9.version
        dr("Type") = structV9.type
        dr("RecipeName") = structV9.recipename
        dr("SourceTXS") = structV9.sourceTXS
        dr("RecipeCategory") = structV9.recipecategory
        dr("RecipeNumber") = structV9.recipenumber
        dr("YieldQuantity") = structV9.yieldquantity
        dr("YieldUnitName") = structV9.yieldunitname
        dr("SubRecipeQuantity") = structV9.subrecipequantity
        dr("SubRecipeUnitName") = structV9.subrecipeunitname
        dr("BatchQty") = structV9.batchqty
        dr("PictureName") = structV9.picturename
        dr("SourceName") = structV9.sourcename
        dr("IngredientNumber") = structV9.ingnumber
        dr("Position") = Position
        dr("Quantity") = structV9.quantity
        dr("Unit") = structV9.unit
        dr("Item") = structV9.item
        dr("Complement") = structV9.complement
        dr("Note") = structV9.note
        dr("Price") = structV9.price
        dr("PriceUnit") = structV9.priceunit
        dr("Wastage1") = structV9.wastage1
        dr("Wastage2") = structV9.wastage2
        dr("Wastage3") = structV9.wastage3
        dr("Wastage4") = structV9.wastage4
        dr("IngredientCategory") = structV9.ingcategory
        dr("IngredientSupplier") = structV9.ingsupplier
        dr("Preparation") = structV9.preparation
        dt.Rows.Add(dr)

        Return bNewRow
    End Function

    Private Function fctAddColumnsRecipeTXSV9(ByVal structV9 As sTXS9, ByVal bNewRecord As Boolean, _
                                              ByVal intRecipeID As Integer, ByRef dtRecipe As DataTable) As Boolean

        Dim dr As DataRow

        If bNewRecord = False Then
            dtRecipe.Columns.Add(New DataColumn("IdRecipe", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("IdMain", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("Version", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("Type", System.Type.GetType("System.Int32")))
            dtRecipe.Columns.Add(New DataColumn("RecipeName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("SourceTXS", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("RecipeCategory", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("RecipeNumber", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("YieldQuantity", System.Type.GetType("System.Double")))
            dtRecipe.Columns.Add(New DataColumn("YieldUnitName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("SubRecipeQuantity", System.Type.GetType("System.Double")))
            dtRecipe.Columns.Add(New DataColumn("SubRecipeUnitName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("PictureName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("SourceName", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("Note", System.Type.GetType("System.String")))
            dtRecipe.Columns.Add(New DataColumn("LanguageCode", System.Type.GetType("System.Int32")))
            bNewRecord = True
        End If

        dr = dtRecipe.NewRow()
        dr("IdRecipe") = intRecipeID
        dr("IdMain") = CInt(n_IDMain)
        dr("Version") = structV9.version
        dr("Type") = structV9.type
        dr("RecipeName") = structV9.recipename
        dr("SourceTXS") = structV9.sourceTXS
        dr("RecipeCategory") = structV9.recipecategory
        dr("RecipeNumber") = structV9.recipenumber
        dr("YieldQuantity") = structV9.yieldquantity
        dr("YieldUnitName") = structV9.yieldunitname
        dr("SubRecipeQuantity") = structV9.subrecipequantity
        dr("SubRecipeUnitName") = structV9.subrecipeunitname
        dr("PictureName") = structV9.picturename
        dr("SourceName") = structV9.sourcename
        dr("Note") = structV9.preparation 'DLS
        dr("LanguageCode") = structV9.languagecode
        dtRecipe.Rows.Add(dr)

        Return bNewRecord
    End Function

    Private Function fctRemoveDoubleQuotes(ByVal strX As String) As String
        If Left$(strX, 1) = """" And Right(strX, 1) = """" Then
            strX = Mid$(strX, 2, Len(strX) - 2)
        End If
        fctRemoveDoubleQuotes = strX
    End Function

    Private Function fctReplaceComma(ByVal strX As String) As String
        Dim i As Integer
        Dim flagInside As Boolean

        flagInside = False
        For i = 1 To Len(strX)
            If Mid$(strX, i, 1) = """" Then flagInside = (Not flagInside)
            If flagInside And Mid$(strX, i, 1) = "," Then strX = Left(strX, i - 1) & "@$@" & Right(strX, Len(strX) - i)
        Next i
        fctReplaceComma = strX
    End Function

    Public Function fctBulkImportTXSRecipe(ByVal dt As DataTable) As String
        Dim strError As String = "TXS Recipe Imported"

        Try
            '------------- Bulk Importation -----------------
            'Copy all data to TempTable using SqlBulkCopy
            Dim bulkCopy As SqlBulkCopy
            bulkCopy = New SqlBulkCopy(L_strCnn)
            bulkCopy.DestinationTableName = "EgswBulkImportRecipes"

            If dt.Columns.Contains("IdRecipe") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IdRecipe", "IDRecipe")))
            End If

            If dt.Columns.Contains("IdMain") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IdMain", "IdMain")))
            End If

            If dt.Columns.Contains("Type") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Type", "ListeType")))
            End If

            If dt.Columns.Contains("RecipeNumber") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("RecipeNumber", "Number")))
            End If

            If dt.Columns.Contains("RecipeName") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("RecipeName", "Name")))
            End If

            If dt.Columns.Contains("SourceName") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("SourceName", "Source")))
            End If

            If dt.Columns.Contains("RecipeCategory") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("RecipeCategory", "Category")))
            End If

            If dt.Columns.Contains("YieldQuantity") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("YieldQuantity", "Yield")))
            End If

            If dt.Columns.Contains("YieldUnitName") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("YieldUnitName", "YieldUnitName")))
            End If

            If dt.Columns.Contains("SubRecipeQuantity") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("SubRecipeQuantity", "srQty")))
            End If

            If dt.Columns.Contains("SubRecipeUnitName") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("SubRecipeUnitName", "srUnitName")))
            End If

            If dt.Columns.Contains("PictureName") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("PictureName", "Picturename")))
            End If

            If dt.Columns.Contains("Note") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Note", "Note")))
            End If

            If dt.Columns.Contains("LanguageCode") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("LanguageCode", "CodeLang")))
            End If

            bulkCopy.WriteToServer(dt)
            bulkCopy.Close()

        Catch ex As Exception
            strError = ex.Message
        End Try
        Return strError
    End Function

    Public Function fctBulkImportTXSIngredients(ByVal dt As DataTable) As String
        Dim strError As String = "TXS Ing Imported"
        Try
            '------------- Bulk Importation -----------------
            'Copy all data to TempTable using SqlBulkCopy
            Dim bulkCopy As SqlBulkCopy
            bulkCopy = New SqlBulkCopy(L_strCnn)
            bulkCopy.DestinationTableName = "EgswBulkImportRecipesDetails"

            If dt.Columns.Contains("IdMain") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IdMain", "IdMain")))
            End If

            If dt.Columns.Contains("IdRecipe") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IdRecipe", "IdRecipe")))
            End If


            '---- No Type Indicated DLS October 29 2008------
            ''If dt.Columns.Contains("Type") Then
            ''    bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Type", "ItemListeType")))
            ''End If

            If dt.Columns.Contains("IngredientNumber") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IngredientNumber", "Number")))
            End If

            If dt.Columns.Contains("Position") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Position", "Position")))
            End If

            If dt.Columns.Contains("Item") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Item", "Name")))
            End If

            If dt.Columns.Contains("Quantity") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Quantity", "Quantity")))
            End If

            If dt.Columns.Contains("Unit") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Unit", "UnitName")))
            End If

            If dt.Columns.Contains("Complement") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Complement", "Complement")))
            End If

            ''If dt.Columns.Contains("Preparation") Then
            ''    bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Preparation", "Preparation")))
            ''End If

            If dt.Columns.Contains("Wastage1") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Wastage1", "Wastage1")))
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

            If dt.Columns.Contains("IngredientCategory") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IngredientCategory", "IngredientCategory")))
            End If

            If dt.Columns.Contains("IngredientSupplier") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("IngredientSupplier", "IngredientSupplier")))
            End If

            If dt.Columns.Contains("Price") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("Price", "IngredientPrice")))
            End If

            If dt.Columns.Contains("PriceUnit") Then
                bulkCopy.ColumnMappings.Add((New SqlBulkCopyColumnMapping("PriceUnit", "IngredientPriceUnit")))
            End If

            bulkCopy.WriteToServer(dt)
            bulkCopy.Close()

        Catch ex As Exception
            strError = ex.Message
        End Try
        Return strError
    End Function

#End Region



    Public Function fctBulkImportMainRecipe(ByVal strFileName As String, ByVal intCompareName As Integer, ByVal intAddNewRecord As Integer, _
                                               ByVal intUpdateRecord As Integer, ByVal intCodeSite As Integer, ByVal intCodeUser As Integer, _
                                               ByVal intCodeTrans As Integer, ByVal intCodeSetPrice As Integer, ByVal intTotalRecord As Integer, ByVal flagImportTextIngredient As Boolean) As Object
        Dim cmdX As New SqlCommand("[BULK_ImportMainRecipe]", New SqlConnection(L_strCnn))
        Dim IDMain As Integer
        Try
            With cmdX
                .CommandType = Data.CommandType.StoredProcedure
                .Parameters.Add("retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                .Parameters.Add("@FileName", SqlDbType.NVarChar, 100).Value = strFileName
                .Parameters.Add("@CompareByName", SqlDbType.Bit).Value = intCompareName
                .Parameters.Add("@AddNewRecord", SqlDbType.Bit).Value = intAddNewRecord
                .Parameters.Add("@UpdateRecord", SqlDbType.Bit).Value = intUpdateRecord
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice
                .Parameters.Add("@TotalRecord", SqlDbType.Int).Value = intTotalRecord
                .Parameters.Add("@ImportTextIngredient", SqlDbType.Bit).Value = flagImportTextIngredient
                .Parameters.Add("@IDMain", SqlDbType.Int).Direction = ParameterDirection.Output
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                IDMain = CInt(.Parameters("@IDMain").Value)
            End With
        Catch ex As Exception
            IDMain = -1
        End Try
        Return IDMain
    End Function

    Public Function fctBulkImportDetailsRecipe(ByVal intIDMain As Integer) As Boolean
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        Try
            With cmd
                .Connection = cn
                .CommandText = "BULK_ImportDetailsRecipe"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@IDMain", SqlDbType.Int).Value = intIDMain
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
            End With
            Return True
        Catch ex As Exception

        End Try
    End Function

    Public Function fctGetEgsWBulkImportMainRecipe(ByVal intCodeSite As Integer, Optional ByVal intIDMain As Integer = -1) As DataTable
        Dim cmd As New SqlCommand
        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                If intIDMain = -1 Then
                    .CommandText = "SELECT ID, Dates, FileName, AddNewRecord, UpdateRecord, CodeSite, TotalRecord, TotalImported, TotalErrors, Done, CodeMarkGroup FROM EgswBulkImportMainRecipe WHERE CodeSite=@CodeSite ORDER BY Dates DESC"
                    .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                Else
                    .CommandText = "SELECT ID, Dates, FileName, AddNewRecord, UpdateRecord, CodeSite, TotalRecord, TotalImported, TotalErrors, Done, CodeMarkGroup FROM EgswBulkImportMainRecipe WHERE CodeSite=@CodeSite AND ID = @IDMain "
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

    Public Function BULKImportStatus(ByVal intIDMain As Integer, ByRef intTotalRecords As Integer, ByRef intTotalImported As Integer, _
                                      ByRef intTotalErrors As Integer, ByRef bImportingMerchandise As Boolean, ByRef bImportingIngredients As Boolean) As Integer
        Dim cmd As New SqlCommand
        Dim intPercent As Integer = 0
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "BULK_CHECKSTATUS_RECIPE"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@IDMain", SqlDbType.Int).Value = intIDMain
                .Parameters.Add("@TotalRecord", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@TotalImported", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@TotalErrors", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@ProgressPercent", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@ImportingMerchandise", SqlDbType.Bit).Direction = ParameterDirection.Output
                .Parameters.Add("@ImportingIngredients", SqlDbType.Bit).Direction = ParameterDirection.Output

                .Connection.Open()
                .ExecuteNonQuery()
                intTotalRecords = CInt(.Parameters("@TotalRecord").Value)
                intTotalImported = CInt(.Parameters("@TotalImported").Value)
                intTotalErrors = CInt(.Parameters("@TotalErrors").Value)
                intPercent = CInt(.Parameters("@ProgressPercent").Value)

                bImportingMerchandise = CBool(.Parameters("@ImportingMerchandise").Value)
                bImportingIngredients = CBool(.Parameters("@ImportingIngredients").Value)
                

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

    Public Function fctGetEgsBulkImportTotalRec(ByVal intCode As Integer, Optional ByVal enumType As MenuType = MenuType.Merchandise) As DataTable
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

    Public Function ExtractFile(ByVal sourceFile As String, ByVal strSavedirectory As String) As Boolean
        On Error GoTo errExtract

        Dim reader As OrganicBit.Zip.ZipReader
        reader = New OrganicBit.Zip.ZipReader(sourceFile)
        Dim buffer(4096) As Byte
        Dim byteCount As Integer

        If Not Directory.Exists(strSavedirectory) Then
            Directory.CreateDirectory(strSavedirectory)
        End If

        If Right(strSavedirectory, 1) <> "\" Then
            strSavedirectory &= "\"
        End If

        '//Get zipped entries
        While reader.MoveNext
            Dim entry As OrganicBit.Zip.ZipEntry = reader.Current
            If entry.IsDirectory Then

            Else

                Dim strFileSegment As String() = entry.Name.Split(CChar("/"))
                Dim strFileName As String = strFileSegment(strFileSegment.Length - 1)

                If LCase(strFileName.Trim) <> "thumbs.db" Then
                    '// create output stream
                    Dim writer As FileStream = File.Open(strSavedirectory & strFileName, FileMode.Create)

                    '// write uncopmpresse data
                    byteCount = reader.Read(buffer, 0, buffer.Length)
                    While byteCount > 0
                        writer.Write(buffer, 0, byteCount)
                        byteCount = reader.Read(buffer, 0, buffer.Length)
                    End While

                    '// close
                    writer.Close()
                End If
            End If

        End While
        reader.Close()
        Return True

errExtract:
        reader.Close()
        Return False
    End Function


    Public Function ExtractFileReturnFilePics(ByVal sourceFile As String, ByRef strSavedirectory As String, ByRef arryPictures As ArrayList) As Boolean
        On Error GoTo errExtract

        Dim reader As OrganicBit.Zip.ZipReader
        reader = New OrganicBit.Zip.ZipReader(sourceFile)
        Dim buffer(4096) As Byte
        Dim byteCount As Integer

        If Not Directory.Exists(strSavedirectory) Then
            Directory.CreateDirectory(strSavedirectory)
        End If

        If Right(strSavedirectory, 1) <> "\" Then
            strSavedirectory &= "\"
        End If

        '//Get zipped entries
        While reader.MoveNext
            Dim entry As OrganicBit.Zip.ZipEntry = reader.Current
            If entry.IsDirectory Then

            Else

                Dim strFileSegment As String() = entry.Name.Split(CChar("/"))
                Dim strFileName As String = strFileSegment(strFileSegment.Length - 1)

                If LCase(strFileName.Trim) <> "thumbs.db" Then
                    '// create output stream
                    Dim writer As FileStream = File.Open(strSavedirectory & strFileName, FileMode.Create)

                    '// write uncopmpresse data
                    byteCount = reader.Read(buffer, 0, buffer.Length)
                    While byteCount > 0
                        writer.Write(buffer, 0, byteCount)
                        byteCount = reader.Read(buffer, 0, buffer.Length)
                    End While

                    '// close
                    writer.Close()
                End If
            End If

        End While
        reader.Close()

        '--------------- Return One File and Pics ----------------------------
        Dim strFiles() As String = Directory.GetFiles(strSavedirectory)
        Dim strfile As String
        Dim strFileOne As String = ""
        For Each strfile In strFiles
            If UCase(Right(strfile, 3)) = "TXS" Or UCase(Right(strfile, 3)) = "TXR" Or UCase(Right(strfile, 3)) = "XML" Or UCase(Right(strfile, 3)) = "TXC" Then
                strFileOne = strfile
            ElseIf UCase(Right(strfile, 3)) = "GIF" Or UCase(Right(strfile, 3)) = "JPG" Or UCase(Right(strfile, 3)) = "BMP" Then
                arryPictures.Add(strfile)
            End If
        Next
        strSavedirectory = strFileOne
        Return True
errExtract:
        reader.Close()
        Return False
    End Function
End Class

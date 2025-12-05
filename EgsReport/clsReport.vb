Imports EgsData
Imports DevExpress.XtraReports
Imports DevExpress.XtraPrinting
Imports DevExpress.Utils
Imports DevExpress.XtraReports.UI
Imports EgsReport.clsGlobal
Imports log4net

Public Class clsReport
    Private Shared ReadOnly Log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

#Region " Declarations "
    Dim m_flagSubRecipeAsterisk As Boolean = False 'DLS 08.09.2007
    Dim m_flagSubRecipeNormalFont As Boolean = False 'DLS 08.09.2007
    Dim m_strFooterAddress As String = "" 'DLS 28.08.2007
    Dim m_strFooterLogoPath As String = "" 'DLS 28.08.2007
    Dim m_flagOnePictureRight As Boolean = False  'DLS 28.08.2007
    Dim m_flagNoLines As Boolean = False  'DLS 28.08.2007
    Dim m_strTitleColor As String = "" 'DLS 
    Dim m_intRecipeDetails As Integer = -1 'VRP 14.12.2007
    Dim m_strMigrosParam As String = ";;;" 'VRP 14.12.2007
    Dim m_blnThumbnailsView As Boolean = False 'VRP 14.03.2008
    Dim m_strSelectedWeek As String = "" 'VRP 30.07.2008
    Dim m_strCLIENT As String 'VRP 09.01.2009
    Dim m_nCodeUserPlan As Integer 'VRP 30.04.2009
#End Region

#Region " Property "
    Public Property TitleColor() As String  'DLS 09.08.2007
        Get
            Return m_strTitleColor
        End Get
        Set(ByVal value As String)
            m_strTitleColor = value
        End Set
    End Property

    Public Property NoPrintLines() As Boolean   'DLS 09.08.2007
        Get
            Return m_flagNoLines
        End Get
        Set(ByVal value As Boolean)
            m_flagNoLines = value
        End Set
    End Property

    Public Property FooterAddress() As String  'DLS 09.08.2007
        Get
            Return m_strFooterAddress
        End Get
        Set(ByVal value As String)
            m_strFooterAddress = value
        End Set
    End Property

    Public Property FooterLogoPath() As String  'DLS 09.08.2007
        Get
            Return m_strFooterLogoPath
        End Get
        Set(ByVal value As String)
            m_strFooterLogoPath = value
        End Set
    End Property

    Public Property PictureOneRight() As Boolean   'DLS 09.08.2007
        Get
            Return m_flagOnePictureRight
        End Get
        Set(ByVal value As Boolean)
            m_flagOnePictureRight = value
        End Set
    End Property

    Public Property DisplaySubRecipeNormalFont() As Boolean 'DLS 09.08.2007
        Get
            Return m_flagSubRecipeNormalFont
        End Get
        Set(ByVal value As Boolean)
            m_flagSubRecipeNormalFont = value
        End Set
    End Property

    Public Property DisplaySubRecipeAstrisk() As Boolean 'DLS 09.08.2007
        Get
            Return m_flagSubRecipeAsterisk
        End Get
        Set(ByVal value As Boolean)
            m_flagSubRecipeAsterisk = value
        End Set
    End Property

    Public Property DisplayRecipeDetails() As Integer 'VRP 14.12.2007
        Get
            Return m_intRecipeDetails
        End Get
        Set(ByVal value As Integer)
            m_intRecipeDetails = value
        End Set
    End Property

    Public Property strMigrosParam() As String 'VRP 14.12.2007
        Get
            Return m_strMigrosParam
        End Get
        Set(ByVal value As String)
            m_strMigrosParam = value
        End Set
    End Property

    Public Property blnThumbnailsView() As Boolean 'VRP 17.03.2008
        Get
            Return m_blnThumbnailsView
        End Get
        Set(ByVal value As Boolean)
            m_blnThumbnailsView = value
        End Set
    End Property

    Public Property SelectedWeek() As String 'VRP 30.07.2008 for Menu Plan test
        Get
            Return m_strSelectedWeek
        End Get
        Set(ByVal value As String)
            m_strSelectedWeek = value
        End Set
    End Property

    Public Property CodeUserPlan() As Integer 'VRP 30.04.2009
        Get
            Return m_nCodeUserPlan
        End Get
        Set(ByVal value As Integer)
            m_nCodeUserPlan = value
        End Set
    End Property

    Public Property CLIENT() As String 'VRP 09.01.2009
        Get
            Return m_strCLIENT
        End Get
        Set(ByVal value As String)
            m_strCLIENT = value
        End Set
    End Property
#End Region

    Public Function CreateReport(ByVal intCodePrintList As Integer, ByVal udtUser As structUser, ByVal strConnection As String, _
           ByRef documentOutput As Integer, ByVal strPhotoPath As String, Optional ByVal strLogoPath As String = "", Optional intFoodlaw As Integer = 1) As XtraReport

        Dim ds As DataSet
        Dim cPrintList As New clsPrintList(udtUser, enumAppType.WebApp, strConnection, enumEgswFetchType.DataTable)
        ds = cPrintList.GetListDetails(intCodePrintList, True)
        Return CreateReport(ds, udtUser, strConnection, documentOutput, strPhotoPath, strLogoPath, "", "", False, True, intFoodlaw)
    End Function

    Public Function CreateReport(ByVal ds2 As DataSet, ByVal udtUser As structUser, ByVal strConnection As String,
        ByRef documentOutput As Integer, Optional ByVal strPhotoPath As String = "", Optional ByVal strLogoPath As String = "", Optional ByVal strLogoPath2 As String = "",
        Optional ByVal strSiteUrl As String = "", Optional ByVal IsCalcmenuOnline As Boolean = False, Optional blnIsAllowMetricImperial As Boolean = True, Optional intFoodlaw As Integer = 1, Optional CodePrintList As Integer = 0) As XtraReport
        'AGL 2014.08.04 - returned intFoodLaw parameter


        G_strPhotoPath = strPhotoPath
        G_strLogoPath = strLogoPath 'VRP 04.11.2007
        G_strLogoPath2 = strLogoPath2 'VRP 28.08.2008
        G_IsCalcmenuOnline = IsCalcmenuOnline 'VRP 08.01.2009
        'G_CLIENT = CLIENT 'VRP 09.01.2009

        Dim dtProfile As DataTable = ds2.Tables(0)
        Dim dtDetails As DataTable = ds2.Tables(1)
        Dim dtKeyword As DataTable = Nothing
        Dim dtAllergen As DataTable = Nothing
        Dim dtCodes As DataTable = Nothing
        Dim dtSteps As DataTable = Nothing 'VRP 15.05.2008
        Dim dtProductLink As DataTable = Nothing 'VRP 15.07.2008
        Dim printType As enumReportType = CType(dtProfile.Rows(0).Item("printprofiletype"), enumReportType)
        'Dim ReportDetail As New StandardDetail
        Dim dtListeNote As DataTable = Nothing ' JBB 06.30.2012

        ' Additional report fields for RECIPE DETAILS
        Dim dtSubTitles As DataTable ' RDC 02.14.2013 -> Subtitles
        If ds2.Tables.Count >= 7 Then
            dtSubTitles = ds2.Tables(6)
        End If

        Dim dtTimeTypes As DataTable  ' RDC 02.15.2013 -> Recipe Time
        If ds2.Tables.Count >= 8 Then
            dtTimeTypes = ds2.Tables(7)
        End If

        Dim dtNotes As DataTable ' RDC 02.15.2013 -> Serve with, Footnote1 and Footnote2
        'If ds.Tables.Count >= 9 Then
        '    dtNotes = ds.Tables(8)
        'ElseIf ds.Tables.Count >= 6 Then
        '    dtNotes = ds.Tables(7)
        'End If
        If ds2.Tables.Count >= 9 Then
            Select Case printType
                Case enumReportType.RecipeDetail
                    dtNotes = ds2.Tables(8)
                Case enumReportType.MenuDetail
                    dtNotes = ds2.Tables(7)
            End Select
        End If

        Dim dtComplePrep As DataTable ' RDC 02.15.2013 -> Ingredient Complement and Ingredient Preparation
        If ds2.Tables.Count >= 10 Then
            dtComplePrep = ds2.Tables(9)
        End If

        Dim dtBrands As DataTable
        If ds2.Tables.Count >= 11 Then ' RDC 02.22.2013 -> Recipe Brands 
            dtBrands = ds2.Tables(10)
        End If

        Dim dtPublications As DataTable
        If ds2.Tables.Count >= 12 Then ' RDC 02.22.2013 -> Recipe Publications
            dtPublications = ds2.Tables(11)
        End If

        'RDC 07.10.2013 - Cookbook table
        Dim dtCookbook As DataTable
        If ds2.Tables.Count >= 13 Then
            dtCookbook = ds2.Tables(12)
        End If

        'RDC 07.10.2013 - Comment table
        Dim dtComment As DataTable
        If ds2.Tables.Count >= 14 Then
            dtComment = ds2.Tables(13)
        ElseIf ds2.Tables.Count >= 7 Then
            dtComment = ds2.Tables(8)
            'Select Case printType
            '    Case enumReportType.RecipeDetail
            '        dtComment = ds.Tables(13)
            '    Case enumReportType.MenuDetail
            '        dtComment = ds.Tables(8)
            'End Select
        End If


        ' RDC 07.10.2013 - Kiosk table
        Dim dtKiosk As DataTable
        If ds2.Tables.Count >= 15 Then
            dtKiosk = ds2.Tables(14)
        End If

        If ds2.Tables.Count >= 3 Then
            dtKeyword = ds2.Tables(2)
        End If
        If ds2.Tables.Count >= 4 Then
            dtCodes = ds2.Tables(3)
        End If

        If ds2.Tables.Count >= 5 Then
            'dtAllergen = ds.Tables(4) ' KMQDC 5.28.2015

            Select Case printType
                Case enumReportType.RecipeDetail
                    dtSteps = ds2.Tables(4) 'VRP 19.05.2008
                    dtListeNote = ds2.Tables(5) ' JBB 06.30.2012

                    dtAllergen = ds2.Tables(4) ' KMQDC 5.28.2015
                    If ds2.Tables.Count >= 16 Then dtAllergen = ds2.Tables(15) 'AMTLA 2014.07.04
                Case enumReportType.MenuDetail
                    dtSteps = ds2.Tables(4) 'VRP 19.05.2008
                    dtListeNote = ds2.Tables(5) ' JBB 06.30.2012
                    dtAllergen = ds2.Tables(6) 'AMTLA 2014.07.04
                Case enumReportType.MerchandiseDetail 'VRP 15.08.2008 if merchandise detail
                    dtAllergen = ds2.Tables(4) ' NBG 06.06.2016
                    If ds2.Tables.Count >= 6 Then 'added checking for tables(5)'s existence
                        dtProductLink = ds2.Tables(5) ''AMTLA 2014.07.02 change ds.Tables(4) to ds.Tables(5)
                        If Not dtProductLink Is Nothing Then
                            Dim dvProductLink As New DataView(dtProductLink)
                            dvProductLink.RowFilter = "CodeSite=" & udtUser.Site.Code & " AND CodeSite<>0"
                            For i As Integer = 0 To udtUser.arrRoles.Count - 1
                                If udtUser.arrRoles(i) = "3" Then 'Corporate Chef
                                    dvProductLink.RowFilter = "CodeSite<>0"
                                    Exit For
                                End If
                            Next
                        End If
                    End If

            End Select

        End If

        Dim cConfig As New clsConfig(enumAppType.WebApp, strConnection)
        Dim intCodePrintProfile As Integer = CType(dtProfile.Rows(0).Item("codePrintProfile"), enumFileType)
        Dim sortBy As enumPrintSortType = CType(dtProfile.Rows(0).Item("sortBy"), enumFileType)
        Dim groupBy As enumPrintGroupType = CType(dtProfile.Rows(0).Item("groupBy"), enumFileType)
        printType = CType(dtProfile.Rows(0).Item("printprofiletype"), enumReportType)
        udtUser.CodeLang = CInt(dtProfile.Rows(0).Item("codeLang"))
        udtUser.CodeTrans = CInt(dtProfile.Rows(0).Item("codeTrans"))
        documentOutput = CType(dtProfile.Rows(0).Item("documentoutput"), enumFileType)

        Dim Report As New xrReports(udtUser.CodeLang, strConnection)
        Report.DisplaySubRecipeAstrisk = DisplaySubRecipeAstrisk 'DLS 09.08.2007
        Report.DisplaySubRecipeNormalFont = DisplaySubRecipeNormalFont 'DLS 09.08.2007
        Report.FooterAddress = FooterAddress 'DLS 28.08.2007
        Report.FooterLogoPath = FooterLogoPath 'DLS 28.08.2007
        Report.PictureOneRight = PictureOneRight 'DLS 28.08.2007
        Report.TitleColor = TitleColor
        Report.NoPrintLines = NoPrintLines 'DLS
        G_ReportOptions.flagNoLines = NoPrintLines 'DLS
        G_ReportOptions.strTitleColor = TitleColor 'DLS
        Report.DisplayRecipeDetails = DisplayRecipeDetails 'VRP 14.12.2007
        Report.strMigrosParam = strMigrosParam 'VRP 14.12.2007
        Report.blnThumbnailsView = blnThumbnailsView 'VRP 17.03.2008
        Report.strCnn = strConnection 'VRP 16.04.2008
        Report.udtUser = udtUser 'VRP 30.07.2008
        Report.SelectedWeek = SelectedWeek

        Dim strFooter As String = FooterAddress
        Dim strFooterSplit() As String = strFooter.Split("�")
        Dim strFooterAddressX As String = ""

        Select Case udtUser.CodeLang
            Case 2
                If UBound(strFooterSplit) >= 1 Then strFooterAddressX = strFooterSplit(1)
            Case 3
                If UBound(strFooterSplit) >= 2 Then strFooterAddressX = strFooterSplit(2)
            Case Else
                If UBound(strFooterSplit) >= 0 Then strFooterAddressX = strFooterSplit(0)
        End Select
        If strFooterAddressX = "" Then
            strFooterAddressX = strFooterSplit(0)
        End If

        G_ReportOptions.strFooterAddress = strFooterAddressX 'DLS 28.08.2007
        G_ReportOptions.strFooterLogoPath = FooterLogoPath 'DLS 28.08.2007
        G_ReportOptions.blnPictureOneRight = PictureOneRight 'DLS 28.08.2007

        ' Get Configuration
        G_ReportOptions.bIncludeGDAImage = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeGDAImage, clsConfig.CodeGroup.printprofile, "FALSE")
        G_ReportOptions.intfoodLaw = intFoodlaw
        Dim blnIncludeNumber As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeNumber, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeWastage As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeWastage, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeTax As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeTax, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeDate As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeDate, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludePrice As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePrice, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeCostOfGoods As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeCostOfGoods, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeFactor As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.prIncludeFactor, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeProfit As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeConst, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeSellingPrice As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeSellingPrice, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeImposedPrice As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeImposedPrice, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeGrossQty As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeGrossQty, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeNetQty As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeNetQty, clsConfig.CodeGroup.printprofile, "FALSE")

        Dim blnIncludeMetricGrossQty As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeMetricQtyGross, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeMetricNetQty As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeMetricQtyNet, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeImperialGrossQty As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeImperialQtyGross, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeImperialNetQty As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeImperialQtyNet, clsConfig.CodeGroup.printprofile, "FALSE")

        Dim blnIncludePicture As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePicture, clsConfig.CodeGroup.printprofile, "FALSE")
        '--- VRP 06.11.2007
        Dim strPictureOption As String = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrPictureOptions, clsConfig.CodeGroup.printprofile, "") 'VRP 06.11.2007
        Dim blnIncludePictureAll As Boolean
        Dim blnIncludePictureFirst As Boolean
        Dim blnIncludePictureRight As Boolean

        ' RDC 02.15.2013 
        ' Newly defined variables for newly added fields in the RECIPE DETAILS report.
        Dim blnIncludeNotes As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeRecipeNotes, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeSubTitle As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeSubtitle, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeServeWith As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeServeWith, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeTimeTypes As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeTimes, clsConfig.CodeGroup.printprofile, "FALSE")
        'Dim blnIncludeComplementPreparation As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeIngredientComplement, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeIngreientComplement As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeIngredientComplement, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeIngreientPreparation As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeIngredientPreparation, clsConfig.CodeGroup.printprofile, "FALSE")

        ' RDC 02.22.2013
        ' Added Brands and Placements in report layout
        Dim blnIncludeBrands As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeBrands, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludePublications As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePublication, clsConfig.CodeGroup.printprofile, "FALSE")
        'Dim blnIncludePlacements As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePlacements, clsConfig.CodeGroup.printprofile, "FALSE")
        ' End

        ' RDC 02.25.2013
        ' Added Procedure Sequence Number
        Dim blnIncludeProcSeqNumber As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeProcSeqNumber, clsConfig.CodeGroup.printprofile, "FALSE")
        ' End

        ' RDC 04.18.2013 - CWM-5350 Fix
        Dim blnIncludeMetricQtyGross As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeMetricQtyGross, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeMetricQtyNet As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeMetricQtyNet, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeImperialQtyGross As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeImperialQtyGross, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeImperialQtyNet As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeImperialQtyNet, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeAlternativeIngredient As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeAlternativeIngredient, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeHACCP As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeHACCP, clsConfig.CodeGroup.printprofile, "FALSE")
        ' End

        ' RDC 04.30.2013 - CWM-5517 Fix
        Dim blnIncludeHighlightSection As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeHighlightSection, clsConfig.CodeGroup.printprofile, "FALSE")

        ' RDC 03.20.2013 - Load Picture type in Manor
        ' Default to 0
        ' 0 - All
        ' 1 - Left
        ' 2 - Right
        Dim blnLoadPictureType As Integer = 0

        Select Case strPictureOption
            Case "20064"
                blnIncludePictureAll = True
                blnIncludePictureFirst = False
                blnIncludePictureRight = False
            Case "20063"
                blnIncludePictureFirst = True
                blnIncludePictureRight = False
                blnIncludePictureAll = False
                ' RDC 03.20.2013 - Load Picture type in Manor
                blnLoadPictureType = 1
            Case "20146"
                blnIncludePictureRight = True
                blnIncludePictureFirst = False
                blnIncludePictureAll = False
                ' RDC 03.20.2013 - Load Picture type in Manor
                blnLoadPictureType = 2
        End Select

        ' RDC 03.20.2013 - Load Picture type in Manor
        G_ReportOptions.blnLoadPictureType = blnLoadPictureType

        '------
        'Dim blnIncludePictureAll As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePictureAll, clsConfig.CodeGroup.printprofile, "FALSE")
        'Dim blnIncludePictureFirst As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePictureFirst, clsConfig.CodeGroup.printprofile, "FALSE")

        Dim blnIncludeInfo As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeInfo, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeNutrient As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeNutrient, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeGDA As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeGDA, clsConfig.CodeGroup.printprofile, "FALSE") 'DLS 11.08.2007
        Dim blnIncludeKeyword As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeKeyword, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeAllergens As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.prIncludeAllergens, clsConfig.CodeGroup.printprofile, "FALSE") ''AMTLA 2014.07.02 DRR 07.05.2012

        ' RDC 04.18.2013 - Removed
        'Dim blnIncludeHACCP As Boolean = False 'cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeHACCP, clsConfig.CodeGroup.printprofile, "FALSE") DRR 07.05.2012
        Dim blnIncludeCategory As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeCategory, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeSource As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeSource, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncluderemark As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeRemark, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeIngredientNumber As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeIngredientNumber, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeIngredientPreparation As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeIngredientPreparation, clsConfig.CodeGroup.printprofile, "FALSE") 'RDTC 23.05.2007
        Dim blnIncludeIngredientComplement As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeIngredientComplement, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludePreparation As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeProcedure, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeCookingTip As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeCookingTip, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludePrice2 As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePrice2, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeSupplier As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeSupplier, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnRemoveTrailingZeros As Boolean = udtUser.RemoveTrailingZeroes
        Dim blnIncludeDerivedKeyword As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeDerivedKeyword, clsConfig.CodeGroup.printprofile, "FALSE") 'VRP 11.09.2008

        Dim blnIncludeRecipeMenuName As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeRecipeListName, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeSubName As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeSubtitle, clsConfig.CodeGroup.printprofile, "FALSE")


        'AGL 2012.12.12
        Dim blIncludeMetric As Boolean
        Dim blIncludeImperial As Boolean
        If blnIsAllowMetricImperial = False Then
            blIncludeMetric = False
            blIncludeImperial = False
        Else
            blIncludeMetric = CBool(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrintDetailsUseMetric, clsConfig.CodeGroup.printprofile, "TRUE"))
            blIncludeImperial = CBool(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrintDetailsUseImperial, clsConfig.CodeGroup.printprofile, "TRUE"))
        End If

        'override booleans if rights is insufficient
        'Dim cRole As clsRoles = New clsRoles(enumAppType.WebApp, strConnection)
        'Dim mnu As MenuType
        'Select Case printType
        '    Case enumReportType.MenuDetail, enumReportType.MenuList, enumReportType.MenuNutrientList
        '        mnu = MenuType.Menu
        '    Case enumReportType.MerchandiseDetail, enumReportType.MerchandiseList, enumReportType.MerchandiseNutrientList
        '        mnu = MenuType.Merchandise
        '    Case enumReportType.RecipeDetail, enumReportType.RecipeList, enumReportType.RecipeNutrientList
        '        mnu = MenuType.Recipe
        'End Select

        'If blnIncludeNutrient Then blnIncludeNutrient = cRole.CheckRoleExist(mnu, UserRightsFunction.AllowNutrientAnalysis, udtUser.arrRoleRights)
        'If blnIncludeAllergens Then blnIncludeAllergens = cRole.CheckRoleExist(mnu, UserRightsFunction.AllowAllergen, udtUser.arrRoleRights)
        'If blnIncludePreparation Then blnIncludePreparation = cRole.CheckRoleExist(mnu, UserRightsFunction.AllowPreparation, udtUser.arrRoleRights)
        'If cRole.CheckRoleExist(mnu, UserRightsFunction.AllowCosting, udtUser.arrRoleRights) = False Then
        '    blnIncludeTax = False
        '    blnIncludePrice = False
        '    blnIncludeCostOfGoods = False
        '    blnIncludeFactor = False
        '    blnIncludeProfit = False
        '    blnIncludeSellingPrice = False
        '    blnIncludeImposedPrice = False
        'End If
        Dim blnPicturePathAccessible As Boolean = True
        Dim dblPageWidth As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrPaperPageWidth, clsConfig.CodeGroup.printprofile, "850")
        Dim dblPageHeight As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrPaperPageHeight, clsConfig.CodeGroup.printprofile, "1100")
        Dim dblListLeftMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListLeftMargin, clsConfig.CodeGroup.printprofile, "1")
        Dim dblListRightMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListRightMargin, clsConfig.CodeGroup.printprofile, "1")
        Dim dblListTopMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListTopMargin, clsConfig.CodeGroup.printprofile, "1")
        Dim dblListBottomMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListBottomMargin, clsConfig.CodeGroup.printprofile, "1")
        Dim marginUnit As enumPrintUnits = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrMarginUnit, clsConfig.CodeGroup.printprofile, CStr(enumPrintUnits.inch))
        Dim dblMarginFactor As Double = 100

        'Dim strListFontName As String = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListFont, clsConfig.CodeGroup.printprofile, "Arial")
        'Dim sgListFontSize As Single = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListFontSize, clsConfig.CodeGroup.printprofile, "9")
        Dim strFontTitleName As String = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrFontTitle, clsConfig.CodeGroup.printprofile, "Arial"), udtUser.CodeTrans, "Arial") 'VRP 31.10.2007
        Dim sgFontTitleSize As Single = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrFontTitleSize, clsConfig.CodeGroup.printprofile, "9"), udtUser.CodeTrans, "9") 'VRP 31.10.2007
        Dim strListFontName As String = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListFont, clsConfig.CodeGroup.printprofile, "Arial"), udtUser.CodeTrans, "Arial")
        Dim sgListFontSize As Single = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListFontSize, clsConfig.CodeGroup.printprofile, "9"), udtUser.CodeTrans, "9")
        Dim dblListLineSpace As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListLineSpacing, clsConfig.CodeGroup.printprofile, "1")
        'Dim strDetailFontName1 As String = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFont1, clsConfig.CodeGroup.printprofile, "Arial")
        'Dim strDetailFontName2 As String = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFont2, clsConfig.CodeGroup.printprofile, "Arial")
        'Dim sgDetailFontSize1 As Single = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFontSize1, clsConfig.CodeGroup.printprofile, "9")
        'Dim sgDetailFontSize2 As Single = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFontSize2, clsConfig.CodeGroup.printprofile, "9")
        Dim strDetailFontName1 As String = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFont2, clsConfig.CodeGroup.printprofile, "Arial"), udtUser.CodeTrans, "Arial")
        Dim strDetailFontName2 As String = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFont2, clsConfig.CodeGroup.printprofile, "Arial"), udtUser.CodeTrans, "Arial")
        Dim sgDetailFontSize1 As Single = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFontSize1, clsConfig.CodeGroup.printprofile, "9"), udtUser.CodeTrans, "9")
        Dim sgDetailFontSize2 As Single = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFontSize2, clsConfig.CodeGroup.printprofile, "9"), udtUser.CodeTrans, "9")

        Dim dblDetailLineSpace As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardLineSpacing, clsConfig.CodeGroup.printprofile, "1")
        Dim dblDetailLeftMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardLeftMargin, clsConfig.CodeGroup.printprofile, "1")
        Dim dblDetailRightMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardRightMargin, clsConfig.CodeGroup.printprofile, "1")
        Dim dblDetailTopMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardTopMargin, clsConfig.CodeGroup.printprofile, "1")
        Dim dblDetailBottomMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardBottomMargin, clsConfig.CodeGroup.printprofile, "1")

        Dim strTextItemFormat As String = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardTextItem, clsConfig.CodeGroup.printprofile, "0_0_0_0")

        Dim printOption As enumPrintOptions = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrPrintOptions, clsConfig.CodeGroup.printprofile, CStr(enumPrintOptions.RecipeCosting))
        Dim variation As enumPrintVariation = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrPrintVariation, clsConfig.CodeGroup.printprofile, CStr(enumPrintVariation.None))
        Dim printStyle As String = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrStyle, clsConfig.CodeGroup.printprofile, CStr(enumPrintStyle.Standard))

        ' RDC 07.09.2013
        Dim blnIncludeDescription As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeDescription, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeAddNotes As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeAddtionalNotes, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeCookbook As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeCookbook, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeKiosk As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeKiosk, clsConfig.CodeGroup.printprofile, "FALSE")

        ' RDC 07.10.2013
        Dim blnIncludeComment As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeComment, clsConfig.CodeGroup.printprofile, "FALSE")

        ' RDC 2014.11.17
        Dim blnIncludeComposition As Boolean = cConfig.GetConfig(intCodePrintProfile, 20406, clsConfig.CodeGroup.printprofile, "FALSE")

        ' RDC 07.24.2013 : Recipe Status
        Dim blnIncludeRecipeStatus As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeRecipeStatus, clsConfig.CodeGroup.printprofile, "FALSE")

        ' RDC 07.25.2013 : Nutrient Set
        Dim intSelectedNutrientSet As Integer = CIntDB(dtProfile.Rows(0)("Codeset")) 'cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeNutrientSet, clsConfig.CodeGroup.printprofile, "0")

        Select Case CType(dtProfile.Rows(0).Item("documentoutput"), enumFileType)
            Case enumFileType.HTML
                If dblDetailLineSpace < 1 Then dblDetailLineSpace = 1
                If dblListLineSpace < 1 Then dblListLineSpace = 1
        End Select

        ' SET MARGINS
        Select Case marginUnit
            Case enumPrintUnits.inch
                dblMarginFactor = 1
            Case enumPrintUnits.centimeter
                dblMarginFactor = 2.54
                'dblMarginFactor = 645.6
            Case enumPrintUnits.millimeter
                dblMarginFactor = 25.4
                'dblMarginFactor = 2540
        End Select

        dblListLeftMargin = (dblListLeftMargin / dblMarginFactor) * 100
        dblListRightMargin = (dblListRightMargin / dblMarginFactor) * 100
        dblListTopMargin = (dblListTopMargin / dblMarginFactor) * 100
        dblListBottomMargin = (dblListBottomMargin / dblMarginFactor) * 100

        dblDetailLeftMargin = (dblDetailLeftMargin / dblMarginFactor) * 100
        dblDetailRightMargin = (dblDetailRightMargin / dblMarginFactor) * 100
        dblDetailTopMargin = (dblDetailTopMargin / dblMarginFactor) * 100
        dblDetailBottomMargin = (dblDetailBottomMargin / dblMarginFactor) * 100

        G_ReportOptions.bFoodcostOnly = CBoolDB(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrShowFoodcostOnly, clsConfig.CodeGroup.printprofile, "False")) 'JRN 21.05.2010 -SV Show food cost only


        G_ReportOptions.blnRemoveTrailingZeros = blnRemoveTrailingZeros
        G_ReportOptions.blnMode = CBoolDB(ds2.Tables(0).Rows(0).Item("Mode")) '// DRR 07.05.2012

        ' RDC 01.07.2014 : Use Fractions
        G_ReportOptions.blnUseFractions = CBoolDB(cConfig.GetConfig(udtUser.Code, clsConfig.enumNumeros.UIDisplayQuantitiesAsFractions, clsConfig.CodeGroup.user, "False"))

        ' HANDLE SORTING
        Dim strSubHeader As String = ""
        Dim strReportTitle As String = ""
        Dim cLang As New clsEGSLanguage(udtUser.CodeLang)

        strSubHeader = cLang.GetString(clsEGSLanguage.CodeType.SortBy)

        Select Case sortBy
            Case enumPrintSortType.Category
                dtDetails.DefaultView.Sort = "categoryname, name "
                G_ReportOptions.strSortBy = "CategoryName"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Category)
            Case enumPrintSortType.Dates
                dtDetails.DefaultView.Sort = "dates, name "
                G_ReportOptions.strSortBy = "Dates"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Date_)
            Case enumPrintSortType.Number
                'dtDetail.DefaultView.Sort = "numberlen, number, name "
                'dtDetail.DefaultView.Sort = "number, name "
                G_ReportOptions.strSortBy = "Number"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Number)
            Case enumPrintSortType.Price
                If printType = enumReportType.ShoppingListDetail Then
                    dtDetails.DefaultView.Sort = "price, name "
                    G_ReportOptions.strSortBy = "Price"
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Price)
                ElseIf printType = enumReportType.MerchandiseList Then
                    dtDetails.DefaultView.Sort = "realitemPrice, name "
                    G_ReportOptions.strSortBy = "Price"
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Price)
                Else
                    dtDetails.DefaultView.Sort = "itemPrice, name "
                    G_ReportOptions.strSortBy = "Price"
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Price)
                End If
            Case enumPrintSortType.Tax
                dtDetails.DefaultView.Sort = "tax, name "
                G_ReportOptions.strSortBy = "Tax"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Tax)
            Case enumPrintSortType.SellingPrice
                dtDetails.DefaultView.Sort = "sellingprice, name "
                G_ReportOptions.strSortBy = "SellingPrice"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Selling_Price)
            Case enumPrintSortType.ImposedPrice
                dtDetails.DefaultView.Sort = "imposedprice, name "
                G_ReportOptions.strSortBy = "ImposedPrice"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.ImposedPrice)
            Case enumPrintSortType.CostOfGoods
                dtDetails.DefaultView.Sort = "calcprice,name "
                G_ReportOptions.strSortBy = "CalcPrice"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.CostOfGoods)
            Case enumPrintSortType.Const
                dtDetails.DefaultView.Sort = "coeff, name "
                G_ReportOptions.strSortBy = "Profit"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Profit)
            Case enumPrintSortType.Supplier
                If printType = enumReportType.ShoppingListDetail Then
                    dtDetails.DefaultView.Sort = "Supplier,name "
                Else
                    dtDetails.DefaultView.Sort = "NameRef,name "
                End If
                G_ReportOptions.strSortBy = "Supplier"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Supplier)
            Case enumPrintSortType.GrossQty
                dtDetails.DefaultView.Sort = "GrossQty, name "
                G_ReportOptions.strSortBy = "GrossQty"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Gross_Qty)
            Case enumPrintSortType.Amount
                dtDetails.DefaultView.Sort = "Amount, name "
                G_ReportOptions.strSortBy = "Amount"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Amount)
            Case enumPrintSortType.NetQty
                dtDetails.DefaultView.Sort = "netQty, name "
                G_ReportOptions.strSortBy = "NetQty"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Net_Qty)
            Case enumPrintSortType.Name
                dtDetails.DefaultView.Sort = "name "
                G_ReportOptions.strSortBy = "Name"
                '-- JBB 06.25.2012
                If printType = enumReportType.RecipeDetail Or printType = enumReportType.RecipeList Then
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Title)
                Else
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Name)
                End If
                '--
                'strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Name)
                '--
            Case enumPrintSortType.Wastage  'mcm 26.01.06
                dtDetails.DefaultView.Sort = "Totalwastage, name "
                G_ReportOptions.strSortBy = "Wastage"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Wastage)
            Case Else
                dtDetails.DefaultView.Sort = "name"
                G_ReportOptions.strSortBy = ""
                '-- JBB 06.25.2012
                If printType = enumReportType.RecipeDetail Or printType = enumReportType.RecipeList Then
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Title)
                Else
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Name)
                End If
                '--
                'strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Name)
                '--
        End Select

        G_ReportOptions.intYieldOption = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListYieldOption, clsConfig.CodeGroup.printprofile, "1")
        G_ReportOptions.blIncludeMetric = blIncludeMetric
        G_ReportOptions.blIncludeImperial = blIncludeImperial
        G_ReportOptions.blnIncludeNetQty = CBool(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeNetQty, clsConfig.CodeGroup.printprofile, "TRUE"))
        G_ReportOptions.blnIncludeGrossQty = CBool(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeGrossQty, clsConfig.CodeGroup.printprofile, "TRUE"))

        Select Case printType
            Case enumReportType.MerchandisePriceList
                With G_ReportOptions
                    .dblReportType = 0
                    .intPageLanguage = udtUser.CodeLang
                    .dblLineSpace = dblListLineSpace
                    Return Report.fctPrintMerchandisePriceList(dtDetails, strSubHeader, .intPageLanguage,
                                    blnIncludeNumber, blnIncludeSupplier, blnIncludeCategory, blnIncludePrice, blnIncludePrice2,
                                    strListFontName, sgListFontSize, dblPageWidth, dblPageHeight, dblListLeftMargin, dblListRightMargin,
                                    dblListTopMargin, dblListBottomMargin, False,
                                    strFontTitleName, sgFontTitleSize) 'VRP 05.11.2007
                End With
            Case enumReportType.MerchandiseList
                With G_ReportOptions
                    .dblReportType = 0
                    .intPageLanguage = udtUser.CodeLang
                    .dblLineSpace = dblListLineSpace
                    Return Report.fctPrintMerchandiseList(dtDetails, strSubHeader, .intPageLanguage,
                                    blnIncludeNumber, blnIncludeWastage, blnIncludeTax, blnIncludeDate, blnIncludePrice,
                                    strListFontName, sgListFontSize, dblPageWidth, dblPageHeight, dblListLeftMargin,
                                    dblListRightMargin, dblListTopMargin, dblListBottomMargin, False,
                                    strFontTitleName, sgFontTitleSize) 'VRP 05.11.2007

                End With


            Case enumReportType.RecipeList
                strReportTitle = cLang.GetString(clsEGSLanguage.CodeType.RecipeList)
                With G_ReportOptions
                    .dblReportType = 0
                    .intPageLanguage = udtUser.CodeLang
                    .dblLineSpace = dblListLineSpace
                    .blnRecipe = True
                    'Return Report.fctPrintRecipeMenuList(dtDetail, strReportTitle, strSubHeader, .intPageLanguage, _
                    '                blnIncludeNumber, blnIncludeCostOfGoods, blnIncludeProfit, blnIncludeTax, blnIncludeSellingPrice, _
                    '                blnIncludeImposedPrice, blnIncludeDate, strListFontName, sgListFontSize, dblPageWidth, dblPageHeight, _
                    '                dblListLeftMargin, dblListRightMargin, dblListTopMargin, dblListBottomMargin, False, _
                    '                strFontTitleName, sgFontTitleSize) 'VRP 05.11.2007

                    Return Report.PrintRecipeMenuListCoop(dtDetails, strReportTitle, strSubHeader, .intPageLanguage,
                                   blnIncludeNumber, blnIncludeCostOfGoods, blnIncludeProfit, blnIncludeTax, blnIncludeSellingPrice,
                                   blnIncludeImposedPrice, blnIncludeDate, strListFontName, sgListFontSize, dblPageWidth, dblPageHeight,
                                   dblListLeftMargin, dblListRightMargin, dblListTopMargin, dblListBottomMargin, False,
                                   strFontTitleName, sgFontTitleSize, blnIncludeRecipeMenuName, blnIncludeSubName, blnIncludeCategory) 'VRP 05.11.2007


                End With
            Case enumReportType.MenuList
                strReportTitle = cLang.GetString(clsEGSLanguage.CodeType.MenuList)
                With G_ReportOptions
                    .dblReportType = 0
                    .intPageLanguage = udtUser.CodeLang
                    .dblLineSpace = dblListLineSpace
                    .blnRecipe = False
                    'Return Report.fctPrintRecipeMenuList(dtDetail, strReportTitle, strSubHeader, .intPageLanguage, _
                    '                blnIncludeNumber, blnIncludeCostOfGoods, blnIncludeProfit, blnIncludeTax, blnIncludeSellingPrice, _
                    '                blnIncludeImposedPrice, blnIncludeDate, strListFontName, sgListFontSize, dblPageWidth, dblPageHeight, _
                    '                dblListLeftMargin, dblListRightMargin, dblListTopMargin, dblListBottomMargin, False, _
                    '                strFontTitleName, sgFontTitleSize) 'VRP 05.11.2007

                    Return Report.PrintRecipeMenuListCoop(dtDetails, strReportTitle, strSubHeader, .intPageLanguage,
                                   blnIncludeNumber, blnIncludeCostOfGoods, blnIncludeProfit, blnIncludeTax, blnIncludeSellingPrice,
                                   blnIncludeImposedPrice, blnIncludeDate, strListFontName, sgListFontSize, dblPageWidth, dblPageHeight,
                                   dblListLeftMargin, dblListRightMargin, dblListTopMargin, dblListBottomMargin, False,
                                   strFontTitleName, sgFontTitleSize) 'VRP 05.11.2007

                End With
            Case enumReportType.MerchandiseNutrientList, enumReportType.RecipeNutrientList, enumReportType.MenuNutrientList
                G_ReportOptions.dblLineSpace = dblListLineSpace
                G_ReportOptions.intPageLanguage = udtUser.CodeLang
                G_ReportOptions.dblReportType = 0

                Dim type As enumDataListItemType
                Dim intNutrientQty As Integer
                Select Case printType
                    Case enumReportType.MerchandiseNutrientList
                        type = enumDataListItemType.Merchandise
                    Case enumReportType.RecipeNutrientList
                        type = enumDataListItemType.Recipe
                    Case enumReportType.MenuNutrientList
                        type = enumDataListItemType.Menu
                End Select

                Select Case printOption
                    Case enumPrintOptions.NutrientPerYieldUnit
                        intNutrientQty = 0
                    Case enumPrintOptions.NutrientPer100gOr100ml
                        intNutrientQty = 1
                    Case enumPrintOptions.NutrientBoth
                        intNutrientQty = 2
                End Select

                'Return Report.fctPrintNutrientValuesList(dtDetail, type, intNutrientQty, _
                '                               strListFontName, sgListFontSize, dblPageWidth, dblPageHeight, dblListLeftMargin, _
                '                               dblListRightMargin, dblListTopMargin, dblListBottomMargin, udtUser, _
                '                               strFontTitleName, sgFontTitleSize) 'VRP 05.11.2007

                'VRP 25.06.2009

                ''-- JBB 05.23.2012
                If printType = enumReportType.RecipeNutrientList Then
                    Dim strNutrientToPrint As String = cConfig.GetConfig(udtUser.Site.Code, clsConfig.enumNumeros.PrNutrientList, clsConfig.CodeGroup.site, "")
                    Dim arrNutrientToPrint As New ArrayList(strNutrientToPrint.Split("_"))
                    '' -- For Recipe Nutrient List
                    '' Check Nutrient to be Displayed
                    Dim arrblDisplay(42) As Boolean
                    For intCounter As Integer = 1 To 42
                        arrblDisplay(intCounter) = False
                    Next

                    ' ---------------------- For AFTER V47 -------------------------- Enhancement CWA-23653
                    'Dim strNutrientToDisplay As String = cConfig.GetConfig(udtUser.Site.Code, clsConfig.enumNumeros.RecipeDefaultNutrientShow.GetHashCode.ToString() + intSelectedNutrientSet.ToString(), clsConfig.CodeGroup.site, "0")
                    'Dim strNutrientToDisplays As String() = strNutrientToDisplay.Split("_")
                    ' ---------------------- For AFTER V47 --------------------------


                    If strNutrientToPrint = "" Then
                        For Each drDetails As DataRow In dtDetails.Rows
                            For intNIndex As Integer = 1 To 42
                                If CBoolDB(drDetails("N" & intNIndex & "display")) = True Then
                                    arrblDisplay(intNIndex) = True
                                End If
                            Next
                            ' ---------------------- For AFTER V47 -------------------------- Enhancement CWA-23653
                            'For intNIndex As Integer = 1 To strNutrientToDisplays.Length - 1
                            '    If Convert.ToInt32(strNutrientToDisplays(intNIndex - 1)) > 0 Then
                            '        arrblDisplay(intNIndex) = True
                            '    End If
                            'Next
                            ' ---------------------- For AFTER V47 --------------------------

                        Next
                        Dim intTop12 As Integer = 1
                        Dim intCTop As Integer = cConfig.GetConfig(0, clsConfig.enumNumeros.PrNutrientNumber, clsConfig.CodeGroup.global, 10)
                        For intCounter As Integer = 1 To 42
                            If arrblDisplay(intCounter) = True Then
                                If intTop12 <= intCTop Then
                                    intTop12 = intTop12 + 1
                                Else
                                    arrblDisplay(intCounter) = False
                                End If
                            End If
                        Next
                    Else
                        For Each drDetails As DataRow In dtDetails.Rows
                            For intNIndex As Integer = 1 To 42
                                If arrNutrientToPrint.Contains(intNIndex.ToString()) Then
                                    arrblDisplay(intNIndex) = True
                                End If
                            Next
                        Next
                    End If

                    Return Report.PrintRecipeNutrientList(dtDetails, type, intNutrientQty, strListFontName, sgListFontSize,
                                                   dblPageWidth, dblPageHeight, dblListLeftMargin, dblListRightMargin,
                                                   dblListTopMargin, dblListBottomMargin, udtUser, arrblDisplay, strFontTitleName, sgFontTitleSize) 'Will add printStyle KMQDC'

                Else
                    Return Report.PrintNutrientList(dtDetails, type, intNutrientQty, strListFontName, sgListFontSize,
                                                    dblPageWidth, dblPageHeight, dblListLeftMargin, dblListRightMargin,
                                                    dblListTopMargin, dblListBottomMargin, udtUser, strFontTitleName, sgFontTitleSize) 'Will add printStyle KMQDC'

                End If
                ''--
            Case enumReportType.ShoppingListDetail
                G_ReportOptions.dblLineSpace = dblListLineSpace
                G_ReportOptions.dblReportType = 0
                G_ReportOptions.intPageLanguage = udtUser.CodeLang
                G_ReportOptions.strFontName2 = strListFontName
                G_ReportOptions.strFontTitleName = strFontTitleName
                G_ReportOptions.sgFontSize2 = sgListFontSize
                G_ReportOptions.sgFontTitleSize = sgFontTitleSize

                ' rename field to group
                Dim strGroupBy As String = ""
                Dim blnEnableGroup As Boolean = False
                Select Case groupBy
                    Case enumPrintGroupType.Category
                        strGroupBy = "CategoryName"
                        G_ReportOptions.strGroupBy = strGroupBy
                        blnEnableGroup = True
                    Case enumPrintGroupType.None
                        blnEnableGroup = False
                        G_ReportOptions.strGroupBy = ""
                    Case enumPrintGroupType.Supplier
                        blnEnableGroup = True
                        strGroupBy = "Supplier"
                        G_ReportOptions.strGroupBy = strGroupBy
                End Select

                Return Report.fctPrintShoppingList(dtDetails, strSubHeader, blnEnableGroup, strGroupBy, blnIncludePrice, blnIncludeGrossQty, blnIncludeNetQty,
                 blnIncludeNumber, strListFontName, sgListFontSize, dblPageWidth, dblPageHeight, dblListLeftMargin, dblListRightMargin, dblListTopMargin, dblListBottomMargin, False,
                 strFontTitleName, sgFontTitleSize) 'VRP 05.11.2007

            Case enumReportType.MerchandiseDetail

                'MCM 03.01.06
                '----------------------------
                With G_ReportOptions
                    .dblLineSpace = dblDetailLineSpace
                    .dblReportType = 5
                    .dtDetail = dtDetails
                    .dtKeywords = dtKeyword
                    .dtAllergens = dtAllergen
                    .blnPicturePathAccessible = blnPicturePathAccessible
                    .blnWithPicture = blnIncludePicture
                    .blnPicturesAll = blnIncludePictureAll
                    .blnIncludeInfo = blnIncludeInfo
                    .blnIncludeNutrients = blnIncludeNutrient
                    .blnIncludeGDA = blnIncludeGDA 'DLS Aug112007
                    .blnIncludeKeyword = blnIncludeKeyword
                    .blnIncludeDerivedKeyword = blnIncludeDerivedKeyword
                    .blnIncludeCookingTip = blnIncludeCookingTip
                    .intTranslation = udtUser.CodeTrans
                    '.strFontName = strDetailFontName1
                    .sgFontSize = sgDetailFontSize1
                    .strFontName2 = strDetailFontName2
                    .sgFontSize2 = sgDetailFontSize2
                    .dblPageWidth = dblPageWidth
                    .dblPageHeight = dblPageHeight
                    .dblLeftMargin = dblDetailLeftMargin
                    .dblRightMargin = dblDetailRightMargin
                    .dblTopMargin = dblDetailTopMargin
                    .dblBottomMargin = dblDetailBottomMargin
                    .intPageLanguage = udtUser.CodeLang
                    .blLandscape = False
                    .blnIncludeAllergens = blnIncludeAllergens
                    .strSubStyle = printStyle
                    '----------------------------
                    .strFontTitleName = strFontTitleName 'VRP 05.11.2007
                    .sgFontTitleSize = sgFontTitleSize 'VRP 05.11.2007
                    .blnPictureOneRight = blnIncludePictureRight 'VRP 06.11.2007
                    .dtProductLink = dtProductLink 'VRP 15.07.2008
                    .blnIncludeHighlightSection = blnIncludeHighlightSection    ' RDC 07.23.2013 : Higlight section
                    .blnIncludeRecipeStatus = blnIncludeRecipeStatus            ' RDC 07.24.2013 : Recipe Status
                    .intSelectedNutrientSet = intSelectedNutrientSet            ' RDC 08.02.2013 : Nutrient Set
                    .intfoodLaw = intFoodlaw

                    .blnIncludeComposition = blnIncludeComposition 'AGL 2014.11.17
                    Return Report.fctMasterReport(dtCodes, dblPageWidth, dblPageHeight, dblDetailLeftMargin,
                                                  dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin, .strFontName2, .sgFontSize2, False, printType,
                                                  .strFontTitleName, .sgFontTitleSize) 'VRP 05.11.2007
                End With


                'Return Report.fctPrintEgsMerchandiseDetails(dtDetail, blnIncludePicture, blnIncludeInfo, _
                '                blnIncludeNutrient, blnIncludeKeyword, udtUser.CodeLang, _
                '                strDetailFontName1, sgDetailFontSize1, strDetailFontName2, _
                '                sgDetailFontSize2, dblPageWidth, dblPageHeight, _
                '                dblDetailLeftMargin, dblDetailRightMargin, _
                '                dblDetailTopMargin, dblDetailBottomMargin, False)

            Case enumReportType.RecipeDetail, enumReportType.MenuDetail
                Dim type As enumDataListItemType
                Select Case printType
                    Case enumReportType.MerchandiseDetail
                        type = enumDataListItemType.Merchandise
                    Case enumReportType.RecipeDetail
                        type = enumDataListItemType.Recipe
                    Case enumReportType.MenuDetail
                        type = enumDataListItemType.Menu
                End Select
                '   Return Report.fctPrintRecipeEgsLayout(dtDetail, "64", False, type, udtUser.CodeTrans, strDetailFontName1, sgDetailFontSize1, _
                '   dblPageWidth, dblPageHeight, dblDetailLeftMargin, dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin, False)

                Select Case printOption
                    Case enumPrintOptions.RecipeCosting, enumPrintOptions.RecipeCostingAndPreparation, enumPrintOptions.MenuCosting, enumPrintOptions.MenuCostingAndDescription

                        'MCM 03.01.06
                        '----------------------------
                        type = enumDataListItemType.Recipe
                        With G_ReportOptions
                            'mcm 13.01.05
                            Select Case printOption
                                Case enumPrintOptions.RecipeCosting,
                                    enumPrintOptions.RecipeCostingAndPreparation,
                                    enumPrintOptions.MenuCosting,
                                    enumPrintOptions.MenuCostingAndDescription 'AGL 2014.03.13 - added enumPrintOptions.MenuCosting, enumPrintOptions.MenuCostingAndDescription
                                    .dblReportType = 6
                                    'Case enumPrintOptions.MenuCosting, enumPrintOptions.MenuCostingAndDescription
                                    '    .dblReportType = 7
                            End Select
                            If CInt(printStyle) = 10 Then
                                .dblReportType = 1
                                .blnMigrosCustomPrint = True
                            Else
                                .dblReportType = 6
                            End If
                            .dblLineSpace = dblDetailLineSpace
                            .intPageLanguage = udtUser.CodeLang
                            .dtDetail = dtDetails
                            .dtKeywords = dtKeyword
                            .dtAllergens = dtAllergen
                            .blnRecipe = type
                            .blnIncludeNumber = blnIncludeNumber
                            .blnIncludeCategory = blnIncludeCategory
                            .blnIncludeSource = blnIncludeSource
                            .blnIncludeDate = blnIncludeDate
                            .blnIncludeCostOfGoods = blnIncludeCostOfGoods
                            .blnIncludeRemark = blnIncluderemark
                            .blnIncludeIngrNumber = blnIncludeIngredientNumber
                            .blnIncludeIngrPreparation = blnIncludeIngredientPreparation   'RDTC 23.05.2007
                            .blnIncludePreparation = blnIncludePreparation
                            .blnIncludeNutrients = blnIncludeNutrient
                            .blnIncludeGDA = blnIncludeGDA 'DLS Aug112007
                            .blnIncludeHACCP = blnIncludeHACCP
                            .intTranslation = udtUser.CodeTrans
                            .blnIncludeKeyword = blnIncludeKeyword
                            .blnPicturesAll = blnIncludePictureAll
                            .blnWithPicture = blnIncludePicture
                            .blnPicturePathAccessible = blnPicturePathAccessible
                            .strFontName = strDetailFontName1
                            .sgFontSize = sgDetailFontSize1
                            .strFontName2 = strDetailFontName2
                            .sgFontSize2 = sgDetailFontSize2
                            .dblPageWidth = dblPageWidth
                            .dblPageHeight = dblPageHeight
                            .dblLeftMargin = dblDetailLeftMargin
                            .dblRightMargin = dblDetailRightMargin
                            .dblTopMargin = dblDetailTopMargin
                            .dblBottomMargin = dblDetailBottomMargin
                            .blLandscape = False
                            .blnIncludeNetQty = blnIncludeNetQty
                            .blnIncludeGrossQty = blnIncludeGrossQty
                            .blnIncludeAllergens = blnIncludeAllergens
                            .strTextItemFormat = strTextItemFormat
                            .strFontTitleName = strFontTitleName 'VRP 05.11.2007
                            .sgFontTitleSize = sgFontTitleSize 'VRP 05.11.2007
                            .blnPictureOneRight = blnIncludePictureRight 'VRP 06.11.2007
                            .dtSteps = dtSteps 'VRP 19.05.2008
                            .dtListeNote = dtListeNote ' JBB 06.30.2012
                            .blnIncludeHighlightSection = blnIncludeHighlightSection    ' RDC 07.23.2013 : Higlight section
                            '----------------------------
                            .blnIncludeDerivedKeyword = blnIncludeDerivedKeyword 'VRP 11.09.2008
                            .dtProfile = dtProfile ' RDC 09.05.2013
                            .dtNotes = dtNotes
                            .dtBrands = dtBrands
                            .dtKiosk = dtKiosk
                            .dtPublications = dtPublications
                            .dtCookbook = dtCookbook
                            .dtComment = dtComment
                            .dtCodes = dtCodes
                            .blnIncludeNotes = blnIncludeNotes
                            .blnIncludeAddNotes = blnIncludeAddNotes
                            .blnIncludeComment = blnIncludeComment

                            Select Case printOption
                                Case enumPrintOptions.RecipeCosting
                                    .blnIncludeKiosk = blnIncludeKiosk
                                    .blnIncludeBrand = blnIncludeBrands
                                    .blnIncludePublication = blnIncludePublications
                                    .blnIncludeComposition = blnIncludeComposition
                                    .blnIncludeCookbook = blnIncludeCookbook
                                    .blnRecipe = True
                                Case enumPrintOptions.MenuCosting
                                    .blnIncludeKiosk = False
                                    .blnIncludeBrand = False
                                    .blnIncludePublication = False
                                    .blnIncludeComposition = False
                                    .blnIncludeCookbook = False
                                    .blnRecipe = False
                            End Select
                            .blnIncludeIngredientComplement = blnIncludeIngredientComplement
                            .blnIncludeProcSequenceNo = blnIncludeProcSeqNumber
                            'Return Report.fctMasterReport(dtCodes, dblPageWidth, dblPageHeight, dblDetailLeftMargin, _
                            '                                dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin, _
                            '                                strDetailFontName2, sgDetailFontSize2, False, printType, _
                            '                                .strFontTitleName, .sgFontTitleSize) 'VRP 05.11.2007

                            If DisplayRecipeDetails = 2 Then 'ADF 'VRP 15.08.2008
                                Dim dtNew As New DataTable
                                dtNew.Columns.Add("Code")
                                dtNew.Columns.Add("Name")
                                dtNew.Columns.Add("CodeListeMain")
                                dtNew.Columns.Add("CodeListeParent")

                                Dim rowNew As DataRow
                                For Each row As DataRow In dtCodes.Rows
                                    If CIntDB(row("CodeListeParent")) = 0 Then
                                        rowNew = dtNew.NewRow
                                        rowNew("Code") = row("Code")
                                        rowNew("Name") = row("Name")
                                        rowNew("CodeListeMain") = row("Codelistemain")
                                        rowNew("CodeListeParent") = row("CodeListeParent")
                                        dtNew.Rows.Add(rowNew)
                                        For Each rowDet As DataRow In dtCodes.Rows
                                            If CIntDB(row("Code")) = CIntDB(rowDet("CodeListeMain")) And CIntDB(row("Code")) = CIntDB(rowDet("CodeListeParent")) Then
                                                rowNew = dtNew.NewRow
                                                rowNew("Code") = rowDet("Code")
                                                rowNew("Name") = rowDet("Name")
                                                rowNew("CodeListeMain") = rowDet("Codelistemain")
                                                rowNew("CodeListeParent") = rowDet("CodeListeParent")
                                                dtNew.Rows.Add(rowNew)
                                                For Each rowDet2 As DataRow In dtCodes.Rows
                                                    If CIntDB(row("Code")) = CIntDB(rowDet2("CodeListeMain")) And CIntDB(row("Code")) <> CIntDB(rowDet2("CodeListeParent")) _
                                                        And CIntDB(rowDet("Code")) <> CIntDB(rowDet2("CodeListeMain")) And CIntDB(rowDet("Code")) = CIntDB(rowDet2("CodeListeparent")) Then
                                                        rowNew = dtNew.NewRow
                                                        rowNew("Code") = rowDet2("Code")
                                                        rowNew("Name") = rowDet2("Name")
                                                        rowNew("CodeListeMain") = rowDet2("Codelistemain")
                                                        rowNew("CodeListeParent") = rowDet2("CodeListeParent")
                                                        dtNew.Rows.Add(rowNew)
                                                    End If
                                                Next
                                            End If
                                        Next
                                    End If
                                Next

                                Return Report.fctMasterReport(dtNew, dblPageWidth, dblPageHeight, dblDetailLeftMargin,
                                                           dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin,
                                                           strDetailFontName2, sgDetailFontSize2, False, printType,
                                                           .strFontTitleName, .sgFontTitleSize) 'VRP 05.11.2007
                            Else
                                Return Report.fctMasterReport(dtCodes, dblPageWidth, dblPageHeight, dblDetailLeftMargin,
                                                           dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin,
                                                           strDetailFontName2, sgDetailFontSize2, False, printType,
                                                           .strFontTitleName, .sgFontTitleSize, CodePrintList, dtDetails, dtNotes, dtSteps, dtListeNote) 'VRP 05.11.2007
                            End If

                        End With
                    Case enumPrintOptions.RecipePreparation, enumPrintOptions.MenuDescription
                        Dim strSubStyle As String = ""
                        If variation < 10 Then
                            strSubStyle = "0" & CStr(variation)
                        Else
                            strSubStyle = CStr(variation)
                        End If

                        'MCM 03.01.06
                        '----------------------------
                        With G_ReportOptions
                            If printOption = enumPrintOptions.MenuDescription Then
                                printStyle += 20 'mcm 13.01.06  for menu description reports
                            End If

                            If CInt(printStyle) = 10 Then
                                .dblReportType = 1
                                .blnMigrosCustomPrint = True
                            Else
                                .dblReportType = CInt(printStyle)
                            End If

                            .dblLineSpace = dblDetailLineSpace
                            .intPageLanguage = udtUser.CodeLang
                            '.dblReportType = CInt(printStyle)
                            .dtDetail = dtDetails
                            .dtKeywords = dtKeyword
                            .dtAllergens = dtAllergen
                            .strSubStyle = strSubStyle
                            .blnIncludeNutrients = blnIncludeNutrient
                            .blnIncludeGDA = blnIncludeGDA 'DLS Aug112007
                            .blnIncludeHACCP = blnIncludeHACCP
                            .blnPicturePathAccessible = blnPicturePathAccessible
                            .blnIncludeNumber = blnIncludeNumber
                            .blnIncludeCategory = blnIncludeCategory
                            .blnIncludeSource = blnIncludeSource
                            .blnIncludeDate = blnIncludeDate
                            .blnIncludeCostOfGoods = blnIncludeCostOfGoods
                            .blnIncludeRemark = blnIncluderemark
                            .blnIncludeIngrNumber = blnIncludeIngredientNumber
                            .blnIncludeIngrPreparation = blnIncludeIngredientPreparation   'RDTC 23.05.2007
                            .blnWithPicture = blnIncludePicture
                            .blnIncludePicture = blnIncludePicture
                            .intTranslation = udtUser.CodeTrans
                            .blnIncludePreparation = blnIncludePreparation
                            .blnIncludeKeyword = blnIncludeKeyword
                            .blnPicturesAll = blnIncludePictureAll
                            .strFontName = strDetailFontName1
                            .sgFontSize = sgDetailFontSize1
                            .strFontName2 = strDetailFontName2
                            .sgFontSize2 = sgDetailFontSize2
                            .dblPageWidth = dblPageWidth
                            .dblPageHeight = dblPageHeight
                            .dblLeftMargin = dblDetailLeftMargin
                            .dblRightMargin = dblDetailRightMargin
                            .dblTopMargin = dblDetailTopMargin
                            .dblBottomMargin = dblDetailBottomMargin
                            .blLandscape = False
                            .blnIncludeAllergens = blnIncludeAllergens
                            .strTextItemFormat = strTextItemFormat
                            .strFontTitleName = strFontTitleName 'VRP 05.11.2007
                            .sgFontTitleSize = sgFontTitleSize 'VRP 05.11.2007
                            .blnPictureOneRight = blnIncludePictureRight 'VRP 06.11.2007
                            .dtSteps = dtSteps 'VRP 19.05.2008
                            .dtListeNote = dtListeNote ' JBB 06.30.2012
                            '----------------------------
                            .blnIncludeDerivedKeyword = blnIncludeDerivedKeyword 'VRP 11.09.2008

                            ' RDC 02.15.2013 New Fields for RECIPE DETAILS Report
                            .dtNotes = dtNotes
                            .dtSubtitle = dtSubTitles
                            .dtComplementPreparation = dtComplePrep
                            .dtTimeTypes = dtTimeTypes
                            .blnIncludeNotes = blnIncludeNotes
                            .blnIncludeSubtitle = blnIncludeSubTitle
                            .blnIncludeIngredientPreparation = blnIncludeIngredientPreparation
                            .blnIncludeIngredientComplement = blnIncludeIngredientComplement
                            .blnIncludeTimeTypes = blnIncludeTimeTypes
                            ' End

                            ' RDC 02.22.2013 New Fields for RECIPE DETAILS Report
                            ' Publication and Brands field
                            .blnIncludeBrand = blnIncludeBrands
                            .blnIncludePublication = blnIncludePublications
                            .dtBrands = dtBrands
                            .dtPublications = dtPublications
                            'End

                            ' RDC 02.25.2013 Enabling or Disabling Procedure Sequence Number
                            .blnIncludeProcSequenceNo = blnIncludeProcSeqNumber
                            ' End

                            ' RDC 02.27.2013 PDF line count initialization
                            .intDatalines = 0
                            'End

                            ' RDC 04.26.2013 - Additional Report Options
                            .blnIncludeGrossQty = blnIncludeGrossQty
                            .blnIncludeNetQty = blnIncludeNetQty

                            ' RDC 04.30.2013 - CWM-5517 Fix
                            .blnIncludeHighlightSection = blnIncludeHighlightSection

                            ' RDC 05.15.2013 - Wastage and Alternative ingredient
                            .blnIncludeWastage = blnIncludeWastage
                            .blnIncludeAlternativeIngredient = blnIncludeAlternativeIngredient
                            .blnIncludeMetricQtyGross = blnIncludeMetricQtyGross
                            .blnIncludeMetricQtyNet = blnIncludeMetricQtyNet
                            .blnIncludeImperialQtyGross = blnIncludeImperialQtyGross
                            .blnIncludeImperialQtyNet = blnIncludeImperialQtyNet

                            ' RDC 07.10.2013 - New fields for Recipe Detail report
                            .blnIncludeDescription = blnIncludeDescription
                            .dtComment = dtComment
                            .dtKiosk = dtKiosk
                            .dtCookbook = dtCookbook
                            .blnIncludeKiosk = blnIncludeKiosk
                            .blnIncludeComment = blnIncludeComment
                            .blnIncludeCookbook = blnIncludeCookbook
                            ' RDC 07.24.2013 : Recipe Status
                            .blnIncludeRecipeStatus = blnIncludeRecipeStatus
                            ' RDC 07.25.2013 : Nutrient Set
                            .intSelectedNutrientSet = intSelectedNutrientSet
                            .dtProfile = dtProfile
                            ' RDC 08.16.2013 : Additional notes section in Report
                            .blnIncludeAddNotes = blnIncludeAddNotes

                            .blnIncludeComposition = blnIncludeComposition 'AGL 2014.11.17

                            If DisplayRecipeDetails = 4 Then 'Recipe Center 'VRP 11.07.2008
                                .blnIncludeNutrients = False
                                .blnIncludeGDA = False
                                .blnIncludeCategory = False
                                .blnIncludeHACCP = False
                                .blnIncludeKeyword = False
                                .blnIncludeSource = False
                                .blnIncludeRemark = False
                                .blnIncludeDescription = False  ' RDC 07.10.2013
                                .blnIncludeRecipeStatus = False ' RDC 07.24.2013 : Recipe Status
                                .blnWithPicture = True
                                .intPageLanguage = udtUser.CodeLang
                                .intTranslation = udtUser.CodeTrans
                                Report.SiteUrl = strSiteUrl
                            End If

                            Return Report.fctMasterReport(dtCodes, dblPageWidth, dblPageHeight, dblDetailLeftMargin,
                                                          dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin, strDetailFontName2, sgDetailFontSize2, False, printType,
                                                          .strFontTitleName, .sgFontTitleSize) 'VRP 05.11.2007
                        End With
                        'Return Report.fctPrintRecipeEgsStandard(dtDetail, dtKeyword, strSubStyle, blnIncludeNutrient, _
                        '            blnIncludeHACCP, blnPicturePathAccessible, blnIncludeNumber, blnIncludeCategory, blnIncludeSource, _
                        '             blnIncludeDate, blnIncludeCostOfGoods, blnIncluderemark, blnIncludeIngredientNumber, _
                        '            blnIncludePictureFirst, udtUser.CodeTrans, blnIncludeKeyword, blnIncludePictureAll, _
                        '             strDetailFontName1, sgDetailFontSize1, dblPageWidth, dblPageHeight, dblDetailLeftMargin, _
                        '             dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin, False)

                End Select
            Case enumReportType.MerchandiseThumbnails 'VRP 14.03.2008

        End Select
    End Function
    Public Function CreateReport_CMC(ByVal ds2 As DataSet, ByVal strConnection As String,
        ByRef documentOutput As Integer, Optional ByVal strPhotoPath As String = "", Optional ByVal strLogoPath As String = "", Optional ByVal strLogoPath2 As String = "",
        Optional ByVal strSiteUrl As String = "", Optional ByVal IsCalcmenuOnline As Boolean = False, Optional blnIsAllowMetricImperial As Boolean = True, Optional intFoodlaw As Integer = 1, Optional CodePrintList As Integer = 0,
                                      Optional ByVal userLocale As String = "en-US", Optional ByVal codeUser As Integer = 1) As XtraReport
        Log.Info("CreateReport_CMC")

        'AGL 2014.08.04 - returned intFoodLaw parameter
        Dim udtUser As structUser
        With udtUser
            .Code = codeUser
            .Site.Code = 1

        End With

        G_strConnection = strConnection
        G_strPhotoPath = strPhotoPath
        G_strLogoPath = strLogoPath 'VRP 04.11.2007
        G_strLogoPath2 = strLogoPath2 'VRP 28.08.2008
        G_IsCalcmenuOnline = IsCalcmenuOnline 'VRP 08.01.2009
        G_CLIENT = CLIENT 'VRP 09.01.2009

        Dim dtProfile As DataTable = ds2.Tables(0)
        Dim dtDetails As DataTable = ds2.Tables(1)
        Dim dtKeyword As DataTable = Nothing
        Dim dtAllergen As DataTable = Nothing
        Dim dtCodes As DataTable = Nothing
        Dim dtSteps As DataTable = Nothing 'VRP 15.05.2008
        Dim dtProductLink As DataTable = Nothing 'VRP 15.07.2008
        Dim printType As enumReportType = CType(dtProfile.Rows(0).Item("printprofiletype"), enumReportType)
        'Dim ReportDetail As New StandardDetail
        Dim dtListeNote As DataTable = Nothing ' JBB 06.30.2012

        ' Additional report fields for RECIPE DETAILS
        Dim dtSubTitles As DataTable ' RDC 02.14.2013 -> Subtitles
        If ds2.Tables.Count >= 7 AndAlso ds2.Tables(6).Rows.Count > 0 Then
            dtSubTitles = ds2.Tables(6)
        End If

        Dim dtTimeTypes As DataTable  ' RDC 02.15.2013 -> Recipe Time
        If ds2.Tables.Count >= 8 AndAlso ds2.Tables(7).Rows.Count > 0 Then
            dtTimeTypes = ds2.Tables(7)
        End If

        Dim dtNotes As DataTable ' RDC 02.15.2013 -> Serve with, Footnote1 and Footnote2
        'If ds.Tables.Count >= 9 Then
        '    dtNotes = ds.Tables(8)
        'ElseIf ds.Tables.Count >= 6 Then
        '    dtNotes = ds.Tables(7)
        'End If
        If ds2.Tables.Count >= 9 Then
            Select Case printType
                Case enumReportType.RecipeDetail
                    If ds2.Tables(8).Rows.Count > 0 Then dtNotes = ds2.Tables(8)
                Case enumReportType.MenuDetail
                    If ds2.Tables(7).Rows.Count > 0 Then dtNotes = ds2.Tables(7)
            End Select
        End If

        Dim dtComplePrep As DataTable ' RDC 02.15.2013 -> Ingredient Complement and Ingredient Preparation
        If ds2.Tables.Count >= 10 AndAlso ds2.Tables(9).Rows.Count > 0 Then
            dtComplePrep = ds2.Tables(9)
        End If

        Dim dtBrands As DataTable
        If ds2.Tables.Count >= 11 AndAlso ds2.Tables(10).Rows.Count > 0 Then ' RDC 02.22.2013 -> Recipe Brands 
            dtBrands = ds2.Tables(10)
        End If

        Dim dtPublications As DataTable
        If ds2.Tables.Count >= 12 AndAlso ds2.Tables(11).Rows.Count > 0 Then ' RDC 02.22.2013 -> Recipe Publications
            dtPublications = ds2.Tables(11)
        End If

        'RDC 07.10.2013 - Cookbook table
        Dim dtCookbook As DataTable
        If ds2.Tables.Count >= 13 AndAlso ds2.Tables(12).Rows.Count > 0 Then
            dtCookbook = ds2.Tables(12)
        End If

        'RDC 07.10.2013 - Comment table
        Dim dtComment As DataTable
        If ds2.Tables.Count >= 14 Then
            dtComment = ds2.Tables(13)
        ElseIf ds2.Tables.Count >= 7 Then
            dtComment = ds2.Tables(8)
            'Select Case printType
            '    Case enumReportType.RecipeDetail
            '        dtComment = ds.Tables(13)
            '    Case enumReportType.MenuDetail
            '        dtComment = ds.Tables(8)
            'End Select
        End If


        ' RDC 07.10.2013 - Kiosk table
        Dim dtKiosk As DataTable
        If ds2.Tables.Count >= 15 AndAlso ds2.Tables(14).Rows.Count > 0 Then
            dtKiosk = ds2.Tables(14)
        End If

        If ds2.Tables.Count >= 3 AndAlso ds2.Tables(2).Rows.Count > 0 Then
            dtKeyword = ds2.Tables(2)
        End If
        If ds2.Tables.Count >= 4 AndAlso ds2.Tables(3).Rows.Count > 0 Then
            dtCodes = ds2.Tables(3)
        End If

        If ds2.Tables.Count >= 5 Then
            'dtAllergen = ds.Tables(4) ' KMQDC 5.28.2015

            Select Case printType
                Case enumReportType.RecipeDetail
                    If ds2.Tables(4).Rows.Count > 0 Then dtSteps = ds2.Tables(4) 'VRP 19.05.2008
                    If ds2.Tables(5).Rows.Count > 0 Then dtListeNote = ds2.Tables(5) ' JBB 06.30.2012
                    If ds2.Tables(4).Rows.Count > 0 Then dtAllergen = ds2.Tables(15) ' KMQDC 5.28.2015

                    If ds2.Tables.Count >= 16 Then
                        If ds2.Tables(15).Rows.Count > 0 Then dtAllergen = ds2.Tables(15) 'AMTLA 2014.07.04
                    End If
                Case enumReportType.MenuDetail
                    dtSteps = ds2.Tables(4) 'VRP 19.05.2008
                    dtListeNote = ds2.Tables(5) ' JBB 06.30.2012
                    dtAllergen = ds2.Tables(6) 'AMTLA 2014.07.04
                Case enumReportType.MerchandiseDetail 'VRP 15.08.2008 if merchandise detail
                    dtAllergen = ds2.Tables(4) ' NBG 06.06.2016
                    If ds2.Tables.Count >= 6 Then 'added checking for tables(5)'s existence
                        dtProductLink = ds2.Tables(5) ''AMTLA 2014.07.02 change ds.Tables(4) to ds.Tables(5)
                        If Not dtProductLink Is Nothing Then
                            Dim dvProductLink As New DataView(dtProductLink)
                            dvProductLink.RowFilter = "CodeSite=" & udtUser.Site.Code & " AND CodeSite<>0"
                            'For i As Integer = 0 To udtUser.arrRoles.Count - 1
                            '    If udtUser.arrRoles(i) = "3" Then 'Corporate Chef
                            '        dvProductLink.RowFilter = "CodeSite<>0"
                            '        Exit For
                            '    End If
                            'Next
                        End If
                    End If

            End Select

        End If

        Dim cConfig As New clsConfig(enumAppType.WebApp, strConnection)
        Dim intCodePrintProfile As Integer = CType(dtProfile.Rows(0).Item("codePrintProfile"), enumFileType)
        Dim sortBy As enumPrintSortType = CType(dtProfile.Rows(0).Item("sortBy"), enumFileType)
        Dim groupBy As enumPrintGroupType = CType(dtProfile.Rows(0).Item("groupBy"), enumFileType)
        printType = CType(dtProfile.Rows(0).Item("printprofiletype"), enumReportType)
        udtUser.CodeLang = CInt(dtProfile.Rows(0).Item("codeLang"))
        udtUser.CodeTrans = CInt(dtProfile.Rows(0).Item("codeTrans"))
        documentOutput = CType(dtProfile.Rows(0).Item("documentoutput"), enumFileType)

        Dim Report As New xrReports(udtUser.CodeLang, strConnection)
        Report.DisplaySubRecipeAstrisk = DisplaySubRecipeAstrisk 'DLS 09.08.2007
        Report.DisplaySubRecipeNormalFont = DisplaySubRecipeNormalFont 'DLS 09.08.2007
        Report.FooterAddress = FooterAddress 'DLS 28.08.2007
        Report.FooterLogoPath = FooterLogoPath 'DLS 28.08.2007
        Report.PictureOneRight = PictureOneRight 'DLS 28.08.2007
        Report.TitleColor = TitleColor
        Report.NoPrintLines = NoPrintLines 'DLS
        G_ReportOptions.flagNoLines = NoPrintLines 'DLS
        G_ReportOptions.strTitleColor = TitleColor 'DLS
        Report.DisplayRecipeDetails = DisplayRecipeDetails 'VRP 14.12.2007
        Report.strMigrosParam = strMigrosParam 'VRP 14.12.2007
        Report.blnThumbnailsView = blnThumbnailsView 'VRP 17.03.2008
        Report.strCnn = strConnection 'VRP 16.04.2008
        Report.udtUser = udtUser 'VRP 30.07.2008
        Report.SelectedWeek = SelectedWeek

        Dim strFooter As String = FooterAddress
        Dim strFooterSplit() As String = strFooter.Split("�")
        Dim strFooterAddressX As String = ""

        Select Case udtUser.CodeLang
            Case 2
                If UBound(strFooterSplit) >= 1 Then strFooterAddressX = strFooterSplit(1)
            Case 3
                If UBound(strFooterSplit) >= 2 Then strFooterAddressX = strFooterSplit(2)
            Case Else
                If UBound(strFooterSplit) >= 0 Then strFooterAddressX = strFooterSplit(0)
        End Select
        If strFooterAddressX = "" Then
            strFooterAddressX = strFooterSplit(0)
        End If

        G_ReportOptions.strFooterAddress = strFooterAddressX 'DLS 28.08.2007
        G_ReportOptions.strFooterLogoPath = FooterLogoPath 'DLS 28.08.2007
        G_ReportOptions.blnPictureOneRight = PictureOneRight 'DLS 28.08.2007

        ' Get Configuration
        G_ReportOptions.bIncludeGDAImage = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeGDAImage, clsConfig.CodeGroup.printprofile, "FALSE")
        G_ReportOptions.intfoodLaw = intFoodlaw
        Dim blnIncludeNumber As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeNumber, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeWastage As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeWastage, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeTax As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeTax, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeDate As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeDate, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludePrice As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePrice, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeCostOfGoods As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeCostOfGoods, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeFactor As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.prIncludeFactor, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeProfit As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeConst, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeSellingPrice As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeSellingPrice, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeImposedPrice As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeImposedPrice, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeGrossQty As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeGrossQty, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeNetQty As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeNetQty, clsConfig.CodeGroup.printprofile, "FALSE")

        Dim blnIncludeMetricGrossQty As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeMetricQtyGross, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeMetricNetQty As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeMetricQtyNet, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeImperialGrossQty As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeImperialQtyGross, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeImperialNetQty As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeImperialQtyNet, clsConfig.CodeGroup.printprofile, "FALSE")

        Dim blnIncludePicture As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePicture, clsConfig.CodeGroup.printprofile, "FALSE")
        '--- VRP 06.11.2007
        Dim strPictureOption As String = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrPictureOptions, clsConfig.CodeGroup.printprofile, "") 'VRP 06.11.2007
        Dim blnIncludePictureAll As Boolean
        Dim blnIncludePictureFirst As Boolean
        Dim blnIncludePictureRight As Boolean

        ' RDC 02.15.2013 
        ' Newly defined variables for newly added fields in the RECIPE DETAILS report.
        Dim blnIncludeNotes As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeRecipeNotes, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeSubTitle As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeSubtitle, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeServeWith As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeServeWith, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeTimeTypes As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeTimes, clsConfig.CodeGroup.printprofile, "FALSE")
        'Dim blnIncludeComplementPreparation As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeIngredientComplement, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeIngreientComplement As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeIngredientComplement, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeIngreientPreparation As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeIngredientPreparation, clsConfig.CodeGroup.printprofile, "FALSE")

        ' RDC 02.22.2013
        ' Added Brands and Placements in report layout
        Dim blnIncludeBrands As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeBrands, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludePublications As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePublication, clsConfig.CodeGroup.printprofile, "FALSE")
        'Dim blnIncludePlacements As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePlacements, clsConfig.CodeGroup.printprofile, "FALSE")
        ' End

        ' RDC 02.25.2013
        ' Added Procedure Sequence Number
        Dim blnIncludeProcSeqNumber As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeProcSeqNumber, clsConfig.CodeGroup.printprofile, "FALSE")
        ' End

        ' RDC 04.18.2013 - CWM-5350 Fix
        Dim blnIncludeMetricQtyGross As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeMetricQtyGross, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeMetricQtyNet As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeMetricQtyNet, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeImperialQtyGross As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeImperialQtyGross, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeImperialQtyNet As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeImperialQtyNet, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeAlternativeIngredient As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeAlternativeIngredient, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeHACCP As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeHACCP, clsConfig.CodeGroup.printprofile, "FALSE")
        ' End

        ' RDC 04.30.2013 - CWM-5517 Fix
        Dim blnIncludeHighlightSection As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeHighlightSection, clsConfig.CodeGroup.printprofile, "FALSE")

        ' RDC 03.20.2013 - Load Picture type in Manor
        ' Default to 0
        ' 0 - All
        ' 1 - Left
        ' 2 - Right
        Dim blnLoadPictureType As Integer = 0

        Select Case strPictureOption
            Case "20064"
                blnIncludePictureAll = True
                blnIncludePictureFirst = False
                blnIncludePictureRight = False
            Case "20063"
                blnIncludePictureFirst = True
                blnIncludePictureRight = False
                blnIncludePictureAll = False
                ' RDC 03.20.2013 - Load Picture type in Manor
                blnLoadPictureType = 1
            Case "20146"
                blnIncludePictureRight = True
                blnIncludePictureFirst = False
                blnIncludePictureAll = False
                ' RDC 03.20.2013 - Load Picture type in Manor
                blnLoadPictureType = 2
        End Select

        ' RDC 03.20.2013 - Load Picture type in Manor
        G_ReportOptions.blnLoadPictureType = blnLoadPictureType

        '------
        'Dim blnIncludePictureAll As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePictureAll, clsConfig.CodeGroup.printprofile, "FALSE")
        'Dim blnIncludePictureFirst As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePictureFirst, clsConfig.CodeGroup.printprofile, "FALSE")

        Dim blnIncludeInfo As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeInfo, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeNutrient As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeNutrient, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeGDA As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeGDA, clsConfig.CodeGroup.printprofile, "FALSE") 'DLS 11.08.2007
        Dim blnIncludeKeyword As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeKeyword, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeAllergens As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.prIncludeAllergens, clsConfig.CodeGroup.printprofile, "FALSE") ''AMTLA 2014.07.02 DRR 07.05.2012

        ' RDC 04.18.2013 - Removed
        'Dim blnIncludeHACCP As Boolean = False 'cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeHACCP, clsConfig.CodeGroup.printprofile, "FALSE") DRR 07.05.2012
        Dim blnIncludeCategory As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeCategory, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeSource As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeSource, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncluderemark As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeRemark, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeIngredientNumber As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeIngredientNumber, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeIngredientPreparation As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeIngredientPreparation, clsConfig.CodeGroup.printprofile, "FALSE") 'RDTC 23.05.2007
        Dim blnIncludeIngredientComplement As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeIngredientComplement, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludePreparation As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeProcedure, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeCookingTip As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeCookingTip, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludePrice2 As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePrice2, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeSupplier As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeSupplier, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnRemoveTrailingZeros As Boolean = udtUser.RemoveTrailingZeroes
        Dim blnIncludeDerivedKeyword As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeDerivedKeyword, clsConfig.CodeGroup.printprofile, "FALSE") 'VRP 11.09.2008

        Dim blnIncludeRecipeMenuName As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeRecipeListName, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeSubName As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeSubtitle, clsConfig.CodeGroup.printprofile, "FALSE")


        'AGL 2012.12.12
        Dim blIncludeMetric As Boolean
        Dim blIncludeImperial As Boolean
        If blnIsAllowMetricImperial = False Then
            blIncludeMetric = False
            blIncludeImperial = False
        Else
            blIncludeMetric = CBool(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrintDetailsUseMetric, clsConfig.CodeGroup.printprofile, "TRUE"))
            blIncludeImperial = CBool(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrintDetailsUseImperial, clsConfig.CodeGroup.printprofile, "TRUE"))
        End If


        Dim blnPicturePathAccessible As Boolean = True
        Dim dblPageWidth As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrPaperPageWidth, clsConfig.CodeGroup.printprofile, "850")
        Dim dblPageHeight As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrPaperPageHeight, clsConfig.CodeGroup.printprofile, "1100")
        Dim dblListLeftMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListLeftMargin, clsConfig.CodeGroup.printprofile, "1")
        Dim dblListRightMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListRightMargin, clsConfig.CodeGroup.printprofile, "1")
        Dim dblListTopMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListTopMargin, clsConfig.CodeGroup.printprofile, "1")
        Dim dblListBottomMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListBottomMargin, clsConfig.CodeGroup.printprofile, "1")
        Dim marginUnit As enumPrintUnits = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrMarginUnit, clsConfig.CodeGroup.printprofile, CStr(enumPrintUnits.inch))
        Dim dblMarginFactor As Double = 100

        'Dim strListFontName As String = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListFont, clsConfig.CodeGroup.printprofile, "Arial")
        'Dim sgListFontSize As Single = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListFontSize, clsConfig.CodeGroup.printprofile, "9")
        Dim strFontTitleName As String = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrFontTitle, clsConfig.CodeGroup.printprofile, "Arial"), udtUser.CodeTrans, "Arial") 'VRP 31.10.2007
        Dim sgFontTitleSize As Single = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrFontTitleSize, clsConfig.CodeGroup.printprofile, "16"), udtUser.CodeTrans, "16") 'VRP 31.10.2007
        Dim strListFontName As String = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListFont, clsConfig.CodeGroup.printprofile, "Arial"), udtUser.CodeTrans, "Arial")
        Dim sgListFontSize As Single = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListFontSize, clsConfig.CodeGroup.printprofile, "9"), udtUser.CodeTrans, "9")
        Dim dblListLineSpace As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListLineSpacing, clsConfig.CodeGroup.printprofile, "1")
        'Dim strDetailFontName1 As String = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFont1, clsConfig.CodeGroup.printprofile, "Arial")
        'Dim strDetailFontName2 As String = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFont2, clsConfig.CodeGroup.printprofile, "Arial")
        'Dim sgDetailFontSize1 As Single = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFontSize1, clsConfig.CodeGroup.printprofile, "9")
        'Dim sgDetailFontSize2 As Single = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFontSize2, clsConfig.CodeGroup.printprofile, "9")
        Dim strDetailFontName1 As String = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFont2, clsConfig.CodeGroup.printprofile, "Arial"), udtUser.CodeTrans, "Arial")
        Dim strDetailFontName2 As String = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFont2, clsConfig.CodeGroup.printprofile, "Arial"), udtUser.CodeTrans, "Arial")
        Dim sgDetailFontSize1 As Single = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFontSize1, clsConfig.CodeGroup.printprofile, "9"), udtUser.CodeTrans, "9")
        Dim sgDetailFontSize2 As Single = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFontSize2, clsConfig.CodeGroup.printprofile, "9"), udtUser.CodeTrans, "9")

        Dim dblDetailLineSpace As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardLineSpacing, clsConfig.CodeGroup.printprofile, "1")
        Dim dblDetailLeftMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardLeftMargin, clsConfig.CodeGroup.printprofile, ".8")
        Dim dblDetailRightMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardRightMargin, clsConfig.CodeGroup.printprofile, ".8")
        Dim dblDetailTopMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardTopMargin, clsConfig.CodeGroup.printprofile, "1")
        Dim dblDetailBottomMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardBottomMargin, clsConfig.CodeGroup.printprofile, "1")

        Dim strTextItemFormat As String = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardTextItem, clsConfig.CodeGroup.printprofile, "0_0_0_0")

        Dim printOption As enumPrintOptions = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrPrintOptions, clsConfig.CodeGroup.printprofile, CStr(enumPrintOptions.RecipeCosting))
        Dim variation As enumPrintVariation = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrPrintVariation, clsConfig.CodeGroup.printprofile, CStr(enumPrintVariation.SmallPicture_Quantity_Name))
        Dim printStyle As String = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrStyle, clsConfig.CodeGroup.printprofile, CStr(enumPrintStyle.Standard))

        ' RDC 07.09.2013
        Dim blnIncludeDescription As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeDescription, clsConfig.CodeGroup.printprofile, "TRUE")
        Dim blnIncludeAddNotes As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeAddtionalNotes, clsConfig.CodeGroup.printprofile, "TRUE")
        Dim blnIncludeCookbook As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeCookbook, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeKiosk As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeKiosk, clsConfig.CodeGroup.printprofile, "FALSE")

        ' RDC 07.10.2013
        Dim blnIncludeComment As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeComment, clsConfig.CodeGroup.printprofile, "TRUE")

        ' RDC 2014.11.17
        Dim blnIncludeComposition As Boolean = cConfig.GetConfig(intCodePrintProfile, 20406, clsConfig.CodeGroup.printprofile, "TRUE")

        ' RDC 07.24.2013 : Recipe Status
        Dim blnIncludeRecipeStatus As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeRecipeStatus, clsConfig.CodeGroup.printprofile, "TRUE")

        ' RDC 07.25.2013 : Nutrient Set
        Dim intSelectedNutrientSet As Integer = CIntDB(dtProfile.Rows(0)("Codeset")) 'cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeNutrientSet, clsConfig.CodeGroup.printprofile, "0")

        Select Case CType(dtProfile.Rows(0).Item("documentoutput"), enumFileType)
            Case enumFileType.HTML
                If dblDetailLineSpace < 1 Then dblDetailLineSpace = 1
                If dblListLineSpace < 1 Then dblListLineSpace = 1
        End Select

        ' SET MARGINS
        Select Case marginUnit
            Case enumPrintUnits.inch
                dblMarginFactor = 1
            Case enumPrintUnits.centimeter
                dblMarginFactor = 2.54
                'dblMarginFactor = 645.6
            Case enumPrintUnits.millimeter
                dblMarginFactor = 25.4
                'dblMarginFactor = 2540
        End Select

        dblListLeftMargin = (dblListLeftMargin / dblMarginFactor) * 100
        dblListRightMargin = (dblListRightMargin / dblMarginFactor) * 100
        dblListTopMargin = (dblListTopMargin / dblMarginFactor) * 100
        dblListBottomMargin = (dblListBottomMargin / dblMarginFactor) * 100

        dblDetailLeftMargin = (dblDetailLeftMargin / dblMarginFactor) * 100
        dblDetailRightMargin = (dblDetailRightMargin / dblMarginFactor) * 100
        dblDetailTopMargin = (dblDetailTopMargin / dblMarginFactor) * 100
        dblDetailBottomMargin = (dblDetailBottomMargin / dblMarginFactor) * 100

        G_ReportOptions.bFoodcostOnly = CBoolDB(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrShowFoodcostOnly, clsConfig.CodeGroup.printprofile, "False")) 'JRN 21.05.2010 -SV Show food cost only


        G_ReportOptions.blnRemoveTrailingZeros = blnRemoveTrailingZeros
        G_ReportOptions.blnMode = CBoolDB(ds2.Tables(0).Rows(0).Item("Mode")) '// DRR 07.05.2012

        ' RDC 01.07.2014 : Use Fractions
        G_ReportOptions.blnUseFractions = CBoolDB(cConfig.GetConfig(udtUser.Code, clsConfig.enumNumeros.UIDisplayQuantitiesAsFractions, clsConfig.CodeGroup.user, "False"))

        ' HANDLE SORTING
        Dim strSubHeader As String = ""
        Dim strReportTitle As String = ""
        Dim cLang As New clsEGSLanguage(udtUser.CodeLang)

        strSubHeader = cLang.GetString(clsEGSLanguage.CodeType.SortBy)

        Select Case sortBy
            Case enumPrintSortType.Category
                dtDetails.DefaultView.Sort = "categoryname, name "
                G_ReportOptions.strSortBy = "CategoryName"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Category)
            Case enumPrintSortType.Dates
                dtDetails.DefaultView.Sort = "dates, name "
                G_ReportOptions.strSortBy = "Dates"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Date_)
            Case enumPrintSortType.Number
                'dtDetail.DefaultView.Sort = "numberlen, number, name "
                'dtDetail.DefaultView.Sort = "number, name "
                G_ReportOptions.strSortBy = "Number"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Number)
            Case enumPrintSortType.Price
                If printType = enumReportType.ShoppingListDetail Then
                    dtDetails.DefaultView.Sort = "price, name "
                    G_ReportOptions.strSortBy = "Price"
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Price)
                ElseIf printType = enumReportType.MerchandiseList Then
                    dtDetails.DefaultView.Sort = "realitemPrice, name "
                    G_ReportOptions.strSortBy = "Price"
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Price)
                Else
                    dtDetails.DefaultView.Sort = "itemPrice, name "
                    G_ReportOptions.strSortBy = "Price"
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Price)
                End If
            Case enumPrintSortType.Tax
                dtDetails.DefaultView.Sort = "tax, name "
                G_ReportOptions.strSortBy = "Tax"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Tax)
            Case enumPrintSortType.SellingPrice
                dtDetails.DefaultView.Sort = "sellingprice, name "
                G_ReportOptions.strSortBy = "SellingPrice"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Selling_Price)
            Case enumPrintSortType.ImposedPrice
                dtDetails.DefaultView.Sort = "imposedprice, name "
                G_ReportOptions.strSortBy = "ImposedPrice"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.ImposedPrice)
            Case enumPrintSortType.CostOfGoods
                dtDetails.DefaultView.Sort = "calcprice,name "
                G_ReportOptions.strSortBy = "CalcPrice"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.CostOfGoods)
            Case enumPrintSortType.Const
                dtDetails.DefaultView.Sort = "coeff, name "
                G_ReportOptions.strSortBy = "Profit"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Profit)
            Case enumPrintSortType.Supplier
                If printType = enumReportType.ShoppingListDetail Then
                    dtDetails.DefaultView.Sort = "Supplier,name "
                Else
                    dtDetails.DefaultView.Sort = "NameRef,name "
                End If
                G_ReportOptions.strSortBy = "Supplier"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Supplier)
            Case enumPrintSortType.GrossQty
                dtDetails.DefaultView.Sort = "GrossQty, name "
                G_ReportOptions.strSortBy = "GrossQty"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Gross_Qty)
            Case enumPrintSortType.Amount
                dtDetails.DefaultView.Sort = "Amount, name "
                G_ReportOptions.strSortBy = "Amount"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Amount)
            Case enumPrintSortType.NetQty
                dtDetails.DefaultView.Sort = "netQty, name "
                G_ReportOptions.strSortBy = "NetQty"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Net_Qty)
            Case enumPrintSortType.Name
                dtDetails.DefaultView.Sort = "name "
                G_ReportOptions.strSortBy = "Name"
                '-- JBB 06.25.2012
                If printType = enumReportType.RecipeDetail Or printType = enumReportType.RecipeList Then
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Title)
                Else
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Name)
                End If
                '--
                'strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Name)
                '--
            Case enumPrintSortType.Wastage  'mcm 26.01.06
                dtDetails.DefaultView.Sort = "Totalwastage, name "
                G_ReportOptions.strSortBy = "Wastage"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Wastage)
            Case Else
                dtDetails.DefaultView.Sort = "name"
                G_ReportOptions.strSortBy = ""
                '-- JBB 06.25.2012
                If printType = enumReportType.RecipeDetail Or printType = enumReportType.RecipeList Then
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Title)
                Else
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Name)
                End If
                '--
                'strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Name)
                '--
        End Select

        G_ReportOptions.intYieldOption = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListYieldOption, clsConfig.CodeGroup.printprofile, "1")
        G_ReportOptions.blIncludeMetric = blIncludeMetric
        G_ReportOptions.blIncludeImperial = blIncludeImperial
        G_ReportOptions.blnIncludeNetQty = CBool(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeNetQty, clsConfig.CodeGroup.printprofile, "TRUE"))
        G_ReportOptions.blnIncludeGrossQty = CBool(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeGrossQty, clsConfig.CodeGroup.printprofile, "TRUE"))

        Select Case printType
            Case enumReportType.MerchandisePriceList
                With G_ReportOptions
                    .dblReportType = 0
                    .intPageLanguage = udtUser.CodeLang
                    .dblLineSpace = dblListLineSpace
                    Return Report.fctPrintMerchandisePriceList(dtDetails, strSubHeader, .intPageLanguage,
                                    blnIncludeNumber, blnIncludeSupplier, blnIncludeCategory, blnIncludePrice, blnIncludePrice2,
                                    strListFontName, sgListFontSize, dblPageWidth, dblPageHeight, dblListLeftMargin, dblListRightMargin,
                                    dblListTopMargin, dblListBottomMargin, False,
                                    strFontTitleName, sgFontTitleSize, userLocale:=userLocale) 'VRP 05.11.2007
                End With
            Case enumReportType.MerchandiseList
                With G_ReportOptions
                    .dblReportType = 0
                    .intPageLanguage = udtUser.CodeLang
                    .dblLineSpace = dblListLineSpace
                    Return Report.fctPrintMerchandiseList(dtDetails, strSubHeader, .intPageLanguage,
                                    blnIncludeNumber, blnIncludeWastage, blnIncludeTax, blnIncludeDate, blnIncludePrice,
                                    strListFontName, sgListFontSize, dblPageWidth, dblPageHeight, dblListLeftMargin,
                                    dblListRightMargin, dblListTopMargin, dblListBottomMargin, False,
                                    strFontTitleName, sgFontTitleSize, userLocale:=userLocale) 'VRP 05.11.2007

                End With


            Case enumReportType.RecipeList
                strReportTitle = cLang.GetString(clsEGSLanguage.CodeType.RecipeList)
                With G_ReportOptions
                    .dblReportType = 0
                    .intPageLanguage = udtUser.CodeLang
                    .dblLineSpace = dblListLineSpace
                    .blnRecipe = True
                    'Return Report.fctPrintRecipeMenuList(dtDetail, strReportTitle, strSubHeader, .intPageLanguage, _
                    '                blnIncludeNumber, blnIncludeCostOfGoods, blnIncludeProfit, blnIncludeTax, blnIncludeSellingPrice, _
                    '                blnIncludeImposedPrice, blnIncludeDate, strListFontName, sgListFontSize, dblPageWidth, dblPageHeight, _
                    '                dblListLeftMargin, dblListRightMargin, dblListTopMargin, dblListBottomMargin, False, _
                    '                strFontTitleName, sgFontTitleSize) 'VRP 05.11.2007

                    Return Report.PrintRecipeMenuListCoop(dtDetails, strReportTitle, strSubHeader, .intPageLanguage,
                                   blnIncludeNumber, blnIncludeCostOfGoods, blnIncludeProfit, blnIncludeTax, blnIncludeSellingPrice,
                                   blnIncludeImposedPrice, blnIncludeDate, strListFontName, sgListFontSize, dblPageWidth, dblPageHeight,
                                   dblListLeftMargin, dblListRightMargin, dblListTopMargin, dblListBottomMargin, False,
                                   strFontTitleName, sgFontTitleSize, blnIncludeRecipeMenuName, blnIncludeSubName, blnIncludeCategory, userLocale:=userLocale) 'VRP 05.11.2007


                End With
            Case enumReportType.MenuList
                strReportTitle = cLang.GetString(clsEGSLanguage.CodeType.MenuList)
                With G_ReportOptions
                    .dblReportType = 0
                    .intPageLanguage = udtUser.CodeLang
                    .dblLineSpace = dblListLineSpace
                    .blnRecipe = False
                    'Return Report.fctPrintRecipeMenuList(dtDetail, strReportTitle, strSubHeader, .intPageLanguage, _
                    '                blnIncludeNumber, blnIncludeCostOfGoods, blnIncludeProfit, blnIncludeTax, blnIncludeSellingPrice, _
                    '                blnIncludeImposedPrice, blnIncludeDate, strListFontName, sgListFontSize, dblPageWidth, dblPageHeight, _
                    '                dblListLeftMargin, dblListRightMargin, dblListTopMargin, dblListBottomMargin, False, _
                    '                strFontTitleName, sgFontTitleSize) 'VRP 05.11.2007

                    Return Report.PrintRecipeMenuListCoop(dtDetails, strReportTitle, strSubHeader, .intPageLanguage,
                                   blnIncludeNumber, blnIncludeCostOfGoods, blnIncludeProfit, blnIncludeTax, blnIncludeSellingPrice,
                                   blnIncludeImposedPrice, blnIncludeDate, strListFontName, sgListFontSize, dblPageWidth, dblPageHeight,
                                   dblListLeftMargin, dblListRightMargin, dblListTopMargin, dblListBottomMargin, False,
                                   strFontTitleName, sgFontTitleSize) 'VRP 05.11.2007

                End With
            Case enumReportType.MerchandiseNutrientList, enumReportType.RecipeNutrientList, enumReportType.MenuNutrientList
                G_ReportOptions.dblLineSpace = dblListLineSpace
                G_ReportOptions.intPageLanguage = udtUser.CodeLang
                G_ReportOptions.dblReportType = 0

                Dim type As enumDataListItemType
                Dim intNutrientQty As Integer
                Select Case printType
                    Case enumReportType.MerchandiseNutrientList
                        type = enumDataListItemType.Merchandise
                    Case enumReportType.RecipeNutrientList
                        type = enumDataListItemType.Recipe
                    Case enumReportType.MenuNutrientList
                        type = enumDataListItemType.Menu
                End Select

                Select Case printOption
                    Case enumPrintOptions.NutrientPerYieldUnit
                        intNutrientQty = 0
                    Case enumPrintOptions.NutrientPer100gOr100ml
                        intNutrientQty = 1
                    Case enumPrintOptions.NutrientBoth
                        intNutrientQty = 2
                End Select

                'Return Report.fctPrintNutrientValuesList(dtDetail, type, intNutrientQty, _
                '                               strListFontName, sgListFontSize, dblPageWidth, dblPageHeight, dblListLeftMargin, _
                '                               dblListRightMargin, dblListTopMargin, dblListBottomMargin, udtUser, _
                '                               strFontTitleName, sgFontTitleSize) 'VRP 05.11.2007

                'VRP 25.06.2009

                ''-- JBB 05.23.2012
                If printType = enumReportType.RecipeNutrientList Then
                    Dim strNutrientToPrint As String = cConfig.GetConfig(udtUser.Site.Code, clsConfig.enumNumeros.PrNutrientList, clsConfig.CodeGroup.site, "")
                    Dim arrNutrientToPrint As New ArrayList(strNutrientToPrint.Split("_"))
                    '' -- For Recipe Nutrient List
                    '' Check Nutrient to be Displayed
                    Dim arrblDisplay(42) As Boolean
                    For intCounter As Integer = 1 To 42
                        arrblDisplay(intCounter) = False
                    Next

                    ' ---------------------- For AFTER V47 -------------------------- Enhancement CWA-23653
                    'Dim strNutrientToDisplay As String = cConfig.GetConfig(udtUser.Site.Code, clsConfig.enumNumeros.RecipeDefaultNutrientShow.GetHashCode.ToString() + intSelectedNutrientSet.ToString(), clsConfig.CodeGroup.site, "0")
                    'Dim strNutrientToDisplays As String() = strNutrientToDisplay.Split("_")
                    ' ---------------------- For AFTER V47 --------------------------


                    If strNutrientToPrint = "" Then
                        For Each drDetails As DataRow In dtDetails.Rows
                            For intNIndex As Integer = 1 To 42
                                If CBoolDB(drDetails("N" & intNIndex & "display")) = True Then
                                    arrblDisplay(intNIndex) = True
                                End If
                            Next
                            ' ---------------------- For AFTER V47 -------------------------- Enhancement CWA-23653
                            'For intNIndex As Integer = 1 To strNutrientToDisplays.Length - 1
                            '    If Convert.ToInt32(strNutrientToDisplays(intNIndex - 1)) > 0 Then
                            '        arrblDisplay(intNIndex) = True
                            '    End If
                            'Next
                            ' ---------------------- For AFTER V47 --------------------------

                        Next
                        Dim intTop12 As Integer = 1
                        Dim intCTop As Integer = cConfig.GetConfig(0, clsConfig.enumNumeros.PrNutrientNumber, clsConfig.CodeGroup.global, 10)
                        For intCounter As Integer = 1 To 42
                            If arrblDisplay(intCounter) = True Then
                                If intTop12 <= intCTop Then
                                    intTop12 = intTop12 + 1
                                Else
                                    arrblDisplay(intCounter) = False
                                End If
                            End If
                        Next
                    Else
                        For Each drDetails As DataRow In dtDetails.Rows
                            For intNIndex As Integer = 1 To 42
                                If arrNutrientToPrint.Contains(intNIndex.ToString()) Then
                                    arrblDisplay(intNIndex) = True
                                End If
                            Next
                        Next
                    End If

                    Return Report.PrintRecipeNutrientList(dtDetails, type, intNutrientQty, strListFontName, sgListFontSize,
                                                   dblPageWidth, dblPageHeight, dblListLeftMargin, dblListRightMargin,
                                                   dblListTopMargin, dblListBottomMargin, udtUser, arrblDisplay, strFontTitleName, sgFontTitleSize, userLocale:=userLocale) 'Will add printStyle KMQDC'

                Else
                    Return Report.PrintNutrientList(dtDetails, type, intNutrientQty, strListFontName, sgListFontSize,
                                                    dblPageWidth, dblPageHeight, dblListLeftMargin, dblListRightMargin,
                                                    dblListTopMargin, dblListBottomMargin, udtUser, strFontTitleName, sgFontTitleSize, userLocale:=userLocale) 'Will add printStyle KMQDC'

                End If
                ''--
            Case enumReportType.ShoppingListDetail
                G_ReportOptions.dblLineSpace = dblListLineSpace
                G_ReportOptions.dblReportType = 0
                G_ReportOptions.intPageLanguage = udtUser.CodeLang
                G_ReportOptions.strFontName2 = strListFontName
                G_ReportOptions.strFontTitleName = strFontTitleName
                G_ReportOptions.sgFontSize2 = sgListFontSize
                G_ReportOptions.sgFontTitleSize = sgFontTitleSize

                ' rename field to group
                Dim strGroupBy As String = ""
                Dim blnEnableGroup As Boolean = False
                Select Case groupBy
                    Case enumPrintGroupType.Category
                        strGroupBy = "CategoryName"
                        G_ReportOptions.strGroupBy = strGroupBy
                        blnEnableGroup = True
                    Case enumPrintGroupType.None
                        blnEnableGroup = False
                        G_ReportOptions.strGroupBy = ""
                    Case enumPrintGroupType.Supplier
                        blnEnableGroup = True
                        strGroupBy = "Supplier"
                        G_ReportOptions.strGroupBy = strGroupBy
                End Select

                Return Report.fctPrintShoppingList(dtDetails, strSubHeader, blnEnableGroup, strGroupBy, blnIncludePrice, blnIncludeGrossQty, blnIncludeNetQty,
                 blnIncludeNumber, strListFontName, sgListFontSize, dblPageWidth, dblPageHeight, dblListLeftMargin, dblListRightMargin, dblListTopMargin, dblListBottomMargin, False,
                 strFontTitleName, sgFontTitleSize) 'VRP 05.11.2007

            Case enumReportType.MerchandiseDetail

                'MCM 03.01.06
                '----------------------------
                With G_ReportOptions
                    .dblLineSpace = dblDetailLineSpace
                    .dblReportType = 5
                    .dtDetail = dtDetails
                    .dtKeywords = dtKeyword
                    .dtAllergens = dtAllergen
                    .blnPicturePathAccessible = blnPicturePathAccessible
                    .blnWithPicture = blnIncludePicture
                    .blnPicturesAll = blnIncludePictureAll
                    .blnIncludeInfo = blnIncludeInfo
                    .blnIncludeNutrients = blnIncludeNutrient
                    .blnIncludeGDA = blnIncludeGDA 'DLS Aug112007
                    .blnIncludeKeyword = blnIncludeKeyword
                    .blnIncludeDerivedKeyword = blnIncludeDerivedKeyword
                    .blnIncludeCookingTip = blnIncludeCookingTip
                    .intTranslation = udtUser.CodeTrans
                    '.strFontName = strDetailFontName1
                    .sgFontSize = sgDetailFontSize1
                    .strFontName2 = strDetailFontName2
                    .sgFontSize2 = sgDetailFontSize2
                    .dblPageWidth = dblPageWidth
                    .dblPageHeight = dblPageHeight
                    .dblLeftMargin = dblDetailLeftMargin
                    .dblRightMargin = dblDetailRightMargin
                    .dblTopMargin = dblDetailTopMargin
                    .dblBottomMargin = dblDetailBottomMargin
                    .intPageLanguage = udtUser.CodeLang
                    .blLandscape = False
                    .blnIncludeAllergens = blnIncludeAllergens
                    .strSubStyle = printStyle
                    '----------------------------
                    .strFontTitleName = strFontTitleName 'VRP 05.11.2007
                    .sgFontTitleSize = sgFontTitleSize 'VRP 05.11.2007
                    .blnPictureOneRight = blnIncludePictureRight 'VRP 06.11.2007
                    .dtProductLink = dtProductLink 'VRP 15.07.2008
                    .blnIncludeHighlightSection = blnIncludeHighlightSection    ' RDC 07.23.2013 : Higlight section
                    .blnIncludeRecipeStatus = blnIncludeRecipeStatus            ' RDC 07.24.2013 : Recipe Status
                    .intSelectedNutrientSet = intSelectedNutrientSet            ' RDC 08.02.2013 : Nutrient Set
                    .intfoodLaw = intFoodlaw

                    .blnIncludeComposition = blnIncludeComposition 'AGL 2014.11.17
                    Return Report.fctMasterReport(dtCodes, dblPageWidth, dblPageHeight, dblDetailLeftMargin,
                                                  dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin, .strFontName2, .sgFontSize2, False, printType,
                                                  .strFontTitleName, .sgFontTitleSize, CodePrintList, dtDetails, userLocale:=userLocale) 'VRP 05.11.2007
                End With


                'Return Report.fctPrintEgsMerchandiseDetails(dtDetail, blnIncludePicture, blnIncludeInfo, _
                '                blnIncludeNutrient, blnIncludeKeyword, udtUser.CodeLang, _
                '                strDetailFontName1, sgDetailFontSize1, strDetailFontName2, _
                '                sgDetailFontSize2, dblPageWidth, dblPageHeight, _
                '                dblDetailLeftMargin, dblDetailRightMargin, _
                '                dblDetailTopMargin, dblDetailBottomMargin, False)

            Case enumReportType.RecipeDetail, enumReportType.MenuDetail
                Dim type As enumDataListItemType
                Select Case printType
                    Case enumReportType.MerchandiseDetail
                        type = enumDataListItemType.Merchandise
                    Case enumReportType.RecipeDetail
                        type = enumDataListItemType.Recipe
                    Case enumReportType.MenuDetail
                        type = enumDataListItemType.Menu
                End Select
                '   Return Report.fctPrintRecipeEgsLayout(dtDetail, "64", False, type, udtUser.CodeTrans, strDetailFontName1, sgDetailFontSize1, _
                '   dblPageWidth, dblPageHeight, dblDetailLeftMargin, dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin, False)

                Select Case printOption
                    Case enumPrintOptions.RecipeCosting, enumPrintOptions.RecipeCostingAndPreparation, enumPrintOptions.MenuCosting, enumPrintOptions.MenuCostingAndDescription

                        'MCM 03.01.06
                        '----------------------------
                        type = enumDataListItemType.Recipe
                        With G_ReportOptions
                            'mcm 13.01.05
                            Select Case printOption
                                Case enumPrintOptions.RecipeCosting,
                                    enumPrintOptions.RecipeCostingAndPreparation,
                                    enumPrintOptions.MenuCosting,
                                    enumPrintOptions.MenuCostingAndDescription 'AGL 2014.03.13 - added enumPrintOptions.MenuCosting, enumPrintOptions.MenuCostingAndDescription
                                    .dblReportType = 6
                                    'Case enumPrintOptions.MenuCosting, enumPrintOptions.MenuCostingAndDescription
                                    '    .dblReportType = 7
                            End Select
                            If CInt(printStyle) = 10 Then
                                .dblReportType = 1
                                .blnMigrosCustomPrint = True
                            Else
                                .dblReportType = 6
                            End If
                            .dblLineSpace = dblDetailLineSpace
                            .intPageLanguage = udtUser.CodeLang
                            .dtDetail = dtDetails
                            .dtKeywords = dtKeyword
                            .dtAllergens = dtAllergen
                            .blnRecipe = type
                            .blnIncludeNumber = blnIncludeNumber
                            .blnIncludeCategory = blnIncludeCategory
                            .blnIncludeSource = blnIncludeSource
                            .blnIncludeDate = blnIncludeDate
                            .blnIncludeCostOfGoods = blnIncludeCostOfGoods
                            .blnIncludeRemark = blnIncluderemark
                            .blnIncludeIngrNumber = blnIncludeIngredientNumber
                            .blnIncludeIngrPreparation = blnIncludeIngredientPreparation   'RDTC 23.05.2007
                            .blnIncludePreparation = blnIncludePreparation
                            .blnIncludeNutrients = blnIncludeNutrient
                            .blnIncludeGDA = blnIncludeGDA 'DLS Aug112007
                            .blnIncludeHACCP = blnIncludeHACCP
                            .intTranslation = udtUser.CodeTrans
                            .blnIncludeKeyword = blnIncludeKeyword
                            .blnPicturesAll = blnIncludePictureAll
                            .blnWithPicture = blnIncludePicture
                            .blnPicturePathAccessible = blnPicturePathAccessible
                            .strFontName = strDetailFontName1
                            .sgFontSize = sgDetailFontSize1
                            .strFontName2 = strDetailFontName2
                            .sgFontSize2 = sgDetailFontSize2
                            .dblPageWidth = dblPageWidth
                            .dblPageHeight = dblPageHeight
                            .dblLeftMargin = dblDetailLeftMargin
                            .dblRightMargin = dblDetailRightMargin
                            .dblTopMargin = dblDetailTopMargin
                            .dblBottomMargin = dblDetailBottomMargin
                            .blLandscape = False
                            .blnIncludeNetQty = blnIncludeNetQty
                            .blnIncludeGrossQty = blnIncludeGrossQty
                            .blnIncludeAllergens = blnIncludeAllergens
                            .strTextItemFormat = strTextItemFormat
                            .strFontTitleName = strFontTitleName 'VRP 05.11.2007
                            .sgFontTitleSize = sgFontTitleSize 'VRP 05.11.2007
                            .blnPictureOneRight = blnIncludePictureRight 'VRP 06.11.2007
                            .dtSteps = dtSteps 'VRP 19.05.2008
                            .dtListeNote = dtListeNote ' JBB 06.30.2012
                            .blnIncludeHighlightSection = blnIncludeHighlightSection    ' RDC 07.23.2013 : Higlight section
                            '----------------------------
                            .blnIncludeDerivedKeyword = blnIncludeDerivedKeyword 'VRP 11.09.2008
                            .dtProfile = dtProfile ' RDC 09.05.2013
                            .dtNotes = dtNotes
                            .dtBrands = dtBrands
                            .dtKiosk = dtKiosk
                            .dtPublications = dtPublications
                            .dtCookbook = dtCookbook
                            .dtComment = dtComment
                            .dtCodes = dtCodes
                            .blnIncludeNotes = blnIncludeNotes
                            .blnIncludeAddNotes = blnIncludeAddNotes
                            .blnIncludeComment = blnIncludeComment

                            Select Case printOption
                                Case enumPrintOptions.RecipeCosting
                                    .blnIncludeKiosk = blnIncludeKiosk
                                    .blnIncludeBrand = blnIncludeBrands
                                    .blnIncludePublication = blnIncludePublications
                                    .blnIncludeComposition = blnIncludeComposition
                                    .blnIncludeCookbook = blnIncludeCookbook
                                    .blnRecipe = True
                                Case enumPrintOptions.MenuCosting
                                    .blnIncludeKiosk = False
                                    .blnIncludeBrand = False
                                    .blnIncludePublication = False
                                    .blnIncludeComposition = False
                                    .blnIncludeCookbook = False
                                    .blnRecipe = False
                            End Select
                            .blnIncludeIngredientComplement = blnIncludeIngredientComplement
                            .blnIncludeProcSequenceNo = blnIncludeProcSeqNumber
                            'Return Report.fctMasterReport(dtCodes, dblPageWidth, dblPageHeight, dblDetailLeftMargin, _
                            '                                dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin, _
                            '                                strDetailFontName2, sgDetailFontSize2, False, printType, _
                            '                                .strFontTitleName, .sgFontTitleSize) 'VRP 05.11.2007

                            If DisplayRecipeDetails = 2 Then 'ADF 'VRP 15.08.2008
                                Dim dtNew As New DataTable
                                dtNew.Columns.Add("Code")
                                dtNew.Columns.Add("Name")
                                dtNew.Columns.Add("CodeListeMain")
                                dtNew.Columns.Add("CodeListeParent")

                                Dim rowNew As DataRow
                                For Each row As DataRow In dtCodes.Rows
                                    If CIntDB(row("CodeListeParent")) = 0 Then
                                        rowNew = dtNew.NewRow
                                        rowNew("Code") = row("Code")
                                        rowNew("Name") = row("Name")
                                        rowNew("CodeListeMain") = row("Codelistemain")
                                        rowNew("CodeListeParent") = row("CodeListeParent")
                                        dtNew.Rows.Add(rowNew)
                                        For Each rowDet As DataRow In dtCodes.Rows
                                            If CIntDB(row("Code")) = CIntDB(rowDet("CodeListeMain")) And CIntDB(row("Code")) = CIntDB(rowDet("CodeListeParent")) Then
                                                rowNew = dtNew.NewRow
                                                rowNew("Code") = rowDet("Code")
                                                rowNew("Name") = rowDet("Name")
                                                rowNew("CodeListeMain") = rowDet("Codelistemain")
                                                rowNew("CodeListeParent") = rowDet("CodeListeParent")
                                                dtNew.Rows.Add(rowNew)
                                                For Each rowDet2 As DataRow In dtCodes.Rows
                                                    If CIntDB(row("Code")) = CIntDB(rowDet2("CodeListeMain")) And CIntDB(row("Code")) <> CIntDB(rowDet2("CodeListeParent")) _
                                                        And CIntDB(rowDet("Code")) <> CIntDB(rowDet2("CodeListeMain")) And CIntDB(rowDet("Code")) = CIntDB(rowDet2("CodeListeparent")) Then
                                                        rowNew = dtNew.NewRow
                                                        rowNew("Code") = rowDet2("Code")
                                                        rowNew("Name") = rowDet2("Name")
                                                        rowNew("CodeListeMain") = rowDet2("Codelistemain")
                                                        rowNew("CodeListeParent") = rowDet2("CodeListeParent")
                                                        dtNew.Rows.Add(rowNew)
                                                    End If
                                                Next
                                            End If
                                        Next
                                    End If
                                Next

                                Return Report.fctMasterReport(dtNew, dblPageWidth, dblPageHeight, dblDetailLeftMargin,
                                                           dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin,
                                                           strDetailFontName2, sgDetailFontSize2, False, printType,
                                                           .strFontTitleName, .sgFontTitleSize, userLocale:=userLocale) 'VRP 05.11.2007
                            Else
                                Return Report.fctMasterReport(dtCodes, dblPageWidth, dblPageHeight, dblDetailLeftMargin,
                                                           dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin,
                                                           strDetailFontName2, sgDetailFontSize2, False, printType,
                                                           .strFontTitleName, .sgFontTitleSize, CodePrintList, dtDetails, dtNotes, dtSteps, dtListeNote, userLocale:=userLocale) 'VRP 05.11.2007
                            End If

                        End With
                    Case enumPrintOptions.RecipePreparation, enumPrintOptions.MenuDescription
                        Dim strSubStyle As String = ""
                        If variation < 10 Then
                            strSubStyle = "0" & CStr(variation)
                        Else
                            strSubStyle = CStr(variation)
                        End If

                        'MCM 03.01.06
                        '----------------------------
                        With G_ReportOptions
                            If printOption = enumPrintOptions.MenuDescription Then
                                printStyle += 20 'mcm 13.01.06  for menu description reports
                            End If

                            If CInt(printStyle) = 10 Then
                                .dblReportType = 1
                                .blnMigrosCustomPrint = True
                            Else
                                .dblReportType = CInt(printStyle)
                            End If

                            .dblLineSpace = dblDetailLineSpace
                            .intPageLanguage = udtUser.CodeLang
                            '.dblReportType = CInt(printStyle)
                            .dtDetail = dtDetails
                            .dtKeywords = dtKeyword
                            .dtAllergens = dtAllergen
                            .strSubStyle = strSubStyle
                            .blnIncludeNutrients = blnIncludeNutrient
                            .blnIncludeGDA = blnIncludeGDA 'DLS Aug112007
                            .blnIncludeHACCP = blnIncludeHACCP
                            .blnPicturePathAccessible = blnPicturePathAccessible
                            .blnIncludeNumber = blnIncludeNumber
                            .blnIncludeCategory = blnIncludeCategory
                            .blnIncludeSource = blnIncludeSource
                            .blnIncludeDate = blnIncludeDate
                            .blnIncludeCostOfGoods = blnIncludeCostOfGoods
                            .blnIncludeRemark = blnIncluderemark
                            .blnIncludeIngrNumber = blnIncludeIngredientNumber
                            .blnIncludeIngrPreparation = blnIncludeIngredientPreparation   'RDTC 23.05.2007
                            .blnWithPicture = blnIncludePicture
                            .blnIncludePicture = blnIncludePicture
                            .intTranslation = udtUser.CodeTrans
                            .blnIncludePreparation = blnIncludePreparation
                            .blnIncludeKeyword = blnIncludeKeyword
                            .blnPicturesAll = blnIncludePictureAll
                            .strFontName = strDetailFontName1
                            .sgFontSize = sgDetailFontSize1
                            .strFontName2 = strDetailFontName2
                            .sgFontSize2 = sgDetailFontSize2
                            .dblPageWidth = dblPageWidth
                            .dblPageHeight = dblPageHeight
                            .dblLeftMargin = dblDetailLeftMargin
                            .dblRightMargin = dblDetailRightMargin
                            .dblTopMargin = dblDetailTopMargin
                            .dblBottomMargin = dblDetailBottomMargin
                            .blLandscape = False
                            .blnIncludeAllergens = blnIncludeAllergens
                            .strTextItemFormat = strTextItemFormat
                            .strFontTitleName = strFontTitleName 'VRP 05.11.2007
                            .sgFontTitleSize = sgFontTitleSize 'VRP 05.11.2007
                            .blnPictureOneRight = blnIncludePictureRight 'VRP 06.11.2007
                            .dtSteps = dtSteps 'VRP 19.05.2008
                            .dtListeNote = dtListeNote ' JBB 06.30.2012
                            '----------------------------
                            .blnIncludeDerivedKeyword = blnIncludeDerivedKeyword 'VRP 11.09.2008

                            ' RDC 02.15.2013 New Fields for RECIPE DETAILS Report
                            .dtNotes = dtNotes
                            .dtSubtitle = dtSubTitles
                            .dtComplementPreparation = dtComplePrep
                            .dtTimeTypes = dtTimeTypes
                            .blnIncludeNotes = blnIncludeNotes
                            .blnIncludeSubtitle = blnIncludeSubTitle
                            .blnIncludeIngredientPreparation = blnIncludeIngredientPreparation
                            .blnIncludeIngredientComplement = blnIncludeIngredientComplement
                            .blnIncludeTimeTypes = blnIncludeTimeTypes
                            ' End

                            ' RDC 02.22.2013 New Fields for RECIPE DETAILS Report
                            ' Publication and Brands field
                            .blnIncludeBrand = blnIncludeBrands
                            .blnIncludePublication = blnIncludePublications
                            .dtBrands = dtBrands
                            .dtPublications = dtPublications
                            'End

                            ' RDC 02.25.2013 Enabling or Disabling Procedure Sequence Number
                            .blnIncludeProcSequenceNo = blnIncludeProcSeqNumber
                            ' End

                            ' RDC 02.27.2013 PDF line count initialization
                            .intDatalines = 0
                            'End

                            ' RDC 04.26.2013 - Additional Report Options
                            .blnIncludeGrossQty = blnIncludeGrossQty
                            .blnIncludeNetQty = blnIncludeNetQty

                            ' RDC 04.30.2013 - CWM-5517 Fix
                            .blnIncludeHighlightSection = blnIncludeHighlightSection

                            ' RDC 05.15.2013 - Wastage and Alternative ingredient
                            .blnIncludeWastage = blnIncludeWastage
                            .blnIncludeAlternativeIngredient = blnIncludeAlternativeIngredient
                            .blnIncludeMetricQtyGross = blnIncludeMetricQtyGross
                            .blnIncludeMetricQtyNet = blnIncludeMetricQtyNet
                            .blnIncludeImperialQtyGross = blnIncludeImperialQtyGross
                            .blnIncludeImperialQtyNet = blnIncludeImperialQtyNet

                            ' RDC 07.10.2013 - New fields for Recipe Detail report
                            .blnIncludeDescription = blnIncludeDescription
                            .dtComment = dtComment
                            .dtKiosk = dtKiosk
                            .dtCookbook = dtCookbook
                            .blnIncludeKiosk = blnIncludeKiosk
                            .blnIncludeComment = blnIncludeComment
                            .blnIncludeCookbook = blnIncludeCookbook
                            ' RDC 07.24.2013 : Recipe Status
                            .blnIncludeRecipeStatus = blnIncludeRecipeStatus
                            ' RDC 07.25.2013 : Nutrient Set
                            .intSelectedNutrientSet = intSelectedNutrientSet
                            .dtProfile = dtProfile
                            ' RDC 08.16.2013 : Additional notes section in Report
                            .blnIncludeAddNotes = blnIncludeAddNotes

                            .blnIncludeComposition = blnIncludeComposition 'AGL 2014.11.17

                            If DisplayRecipeDetails = 4 Then 'Recipe Center 'VRP 11.07.2008
                                .blnIncludeNutrients = False
                                .blnIncludeGDA = False
                                .blnIncludeCategory = False
                                .blnIncludeHACCP = False
                                .blnIncludeKeyword = False
                                .blnIncludeSource = False
                                .blnIncludeRemark = False
                                .blnIncludeDescription = False  ' RDC 07.10.2013
                                .blnIncludeRecipeStatus = False ' RDC 07.24.2013 : Recipe Status
                                .blnWithPicture = True
                                .intPageLanguage = udtUser.CodeLang
                                .intTranslation = udtUser.CodeTrans
                                Report.SiteUrl = strSiteUrl
                            End If

                            Return Report.fctMasterReport(dtCodes, dblPageWidth, dblPageHeight, dblDetailLeftMargin,
                                                          dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin, strDetailFontName2, sgDetailFontSize2, False, printType,
                                                          .strFontTitleName, .sgFontTitleSize, intCodePrintList:=CodePrintList, dtDetails2:=dtDetails, userLocale:=userLocale) 'VRP 05.11.2007
                        End With
                        'Return Report.fctPrintRecipeEgsStandard(dtDetail, dtKeyword, strSubStyle, blnIncludeNutrient, _
                        '            blnIncludeHACCP, blnPicturePathAccessible, blnIncludeNumber, blnIncludeCategory, blnIncludeSource, _
                        '             blnIncludeDate, blnIncludeCostOfGoods, blnIncluderemark, blnIncludeIngredientNumber, _
                        '            blnIncludePictureFirst, udtUser.CodeTrans, blnIncludeKeyword, blnIncludePictureAll, _
                        '             strDetailFontName1, sgDetailFontSize1, dblPageWidth, dblPageHeight, dblDetailLeftMargin, _
                        '             dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin, False)

                End Select
            Case enumReportType.MerchandiseThumbnails 'VRP 14.03.2008

        End Select
    End Function

    Public Function CreateReport_Test(ByVal ds2 As DataSet, ByVal strConnection As String,
        ByRef documentOutput As Integer, Optional ByVal strPhotoPath As String = "", Optional ByVal strLogoPath As String = "", Optional ByVal strLogoPath2 As String = "",
        Optional ByVal strSiteUrl As String = "", Optional ByVal IsCalcmenuOnline As Boolean = False, Optional blnIsAllowMetricImperial As Boolean = True, Optional intFoodlaw As Integer = 1, Optional CodePrintList As Integer = 0,
                                      Optional ByVal userLocale As String = "en-US") As XtraReport
        Log.Info("CreateReport_Test")

        'AGL 2014.08.04 - returned intFoodLaw parameter
        Dim udtUser As structUser
        With udtUser
            .Code = 1
            .Site.Code = 1

        End With

        G_strConnection = strConnection
        G_strPhotoPath = strPhotoPath
        G_strLogoPath = strLogoPath 'VRP 04.11.2007
        G_strLogoPath2 = strLogoPath2 'VRP 28.08.2008
        G_IsCalcmenuOnline = IsCalcmenuOnline 'VRP 08.01.2009
        G_CLIENT = CLIENT 'VRP 09.01.2009

        Dim dtProfile As DataTable = ds2.Tables(0)
        Dim dtDetails As DataTable = ds2.Tables(1)
        Dim dtKeyword As DataTable = Nothing
        Dim dtAllergen As DataTable = Nothing
        Dim dtCodes As DataTable = Nothing
        Dim dtSteps As DataTable = Nothing 'VRP 15.05.2008
        Dim dtProductLink As DataTable = Nothing 'VRP 15.07.2008
        Dim printType As enumReportType = CType(dtProfile.Rows(0).Item("printprofiletype"), enumReportType)
        'Dim ReportDetail As New StandardDetail
        Dim dtListeNote As DataTable = Nothing ' JBB 06.30.2012

        ' Additional report fields for RECIPE DETAILS
        Dim dtSubTitles As DataTable ' RDC 02.14.2013 -> Subtitles
        If ds2.Tables.Count >= 7 Then
            dtSubTitles = ds2.Tables(6)
        End If

        Dim dtTimeTypes As DataTable  ' RDC 02.15.2013 -> Recipe Time
        If ds2.Tables.Count >= 8 Then
            dtTimeTypes = ds2.Tables(7)
        End If

        Dim dtNotes As DataTable ' RDC 02.15.2013 -> Serve with, Footnote1 and Footnote2
        'If ds.Tables.Count >= 9 Then
        '    dtNotes = ds.Tables(8)
        'ElseIf ds.Tables.Count >= 6 Then
        '    dtNotes = ds.Tables(7)
        'End If
        If ds2.Tables.Count >= 9 Then
            Select Case printType
                Case enumReportType.RecipeDetail
                    dtNotes = ds2.Tables(8)
                Case enumReportType.MenuDetail
                    dtNotes = ds2.Tables(7)
            End Select
        End If

        Dim dtComplePrep As DataTable ' RDC 02.15.2013 -> Ingredient Complement and Ingredient Preparation
        If ds2.Tables.Count >= 10 Then
            dtComplePrep = ds2.Tables(9)
        End If

        Dim dtBrands As DataTable
        If ds2.Tables.Count >= 11 Then ' RDC 02.22.2013 -> Recipe Brands 
            dtBrands = ds2.Tables(10)
        End If

        Dim dtPublications As DataTable
        If ds2.Tables.Count >= 12 Then ' RDC 02.22.2013 -> Recipe Publications
            dtPublications = ds2.Tables(11)
        End If

        'RDC 07.10.2013 - Cookbook table
        Dim dtCookbook As DataTable
        If ds2.Tables.Count >= 13 Then
            dtCookbook = ds2.Tables(12)
        End If

        'RDC 07.10.2013 - Comment table
        Dim dtComment As DataTable
        If ds2.Tables.Count >= 14 Then
            dtComment = ds2.Tables(13)
        ElseIf ds2.Tables.Count >= 7 Then
            dtComment = ds2.Tables(8)
            'Select Case printType
            '    Case enumReportType.RecipeDetail
            '        dtComment = ds.Tables(13)
            '    Case enumReportType.MenuDetail
            '        dtComment = ds.Tables(8)
            'End Select
        End If


        ' RDC 07.10.2013 - Kiosk table
        Dim dtKiosk As DataTable
        If ds2.Tables.Count >= 15 Then
            dtKiosk = ds2.Tables(14)
        End If

        If ds2.Tables.Count >= 3 Then
            dtKeyword = ds2.Tables(2)
        End If
        If ds2.Tables.Count >= 4 Then
            dtCodes = ds2.Tables(3)
        End If

        If ds2.Tables.Count >= 5 Then
            'dtAllergen = ds.Tables(4) ' KMQDC 5.28.2015

            Select Case printType
                Case enumReportType.RecipeDetail
                    dtSteps = ds2.Tables(4) 'VRP 19.05.2008
                    dtListeNote = ds2.Tables(5) ' JBB 06.30.2012

                    dtAllergen = ds2.Tables(4) ' KMQDC 5.28.2015
                    If ds2.Tables.Count >= 16 Then dtAllergen = ds2.Tables(15) 'AMTLA 2014.07.04
                Case enumReportType.MenuDetail
                    dtSteps = ds2.Tables(4) 'VRP 19.05.2008
                    dtListeNote = ds2.Tables(5) ' JBB 06.30.2012
                    dtAllergen = ds2.Tables(6) 'AMTLA 2014.07.04
                Case enumReportType.MerchandiseDetail 'VRP 15.08.2008 if merchandise detail
                    dtAllergen = ds2.Tables(4) ' NBG 06.06.2016
                    If ds2.Tables.Count >= 6 Then 'added checking for tables(5)'s existence
                        dtProductLink = ds2.Tables(5) ''AMTLA 2014.07.02 change ds.Tables(4) to ds.Tables(5)
                        If Not dtProductLink Is Nothing Then
                            Dim dvProductLink As New DataView(dtProductLink)
                            dvProductLink.RowFilter = "CodeSite=" & udtUser.Site.Code & " AND CodeSite<>0"
                            'For i As Integer = 0 To udtUser.arrRoles.Count - 1
                            '    If udtUser.arrRoles(i) = "3" Then 'Corporate Chef
                            '        dvProductLink.RowFilter = "CodeSite<>0"
                            '        Exit For
                            '    End If
                            'Next
                        End If
                    End If

            End Select

        End If

        Dim cConfig As New clsConfig(enumAppType.WebApp, strConnection)
        Dim intCodePrintProfile As Integer = CType(dtProfile.Rows(0).Item("codePrintProfile"), enumFileType)
        Dim sortBy As enumPrintSortType = CType(dtProfile.Rows(0).Item("sortBy"), enumFileType)
        Dim groupBy As enumPrintGroupType = CType(dtProfile.Rows(0).Item("groupBy"), enumFileType)
        printType = CType(dtProfile.Rows(0).Item("printprofiletype"), enumReportType)
        udtUser.CodeLang = CInt(dtProfile.Rows(0).Item("codeLang"))
        udtUser.CodeTrans = CInt(dtProfile.Rows(0).Item("codeTrans"))
        documentOutput = CType(dtProfile.Rows(0).Item("documentoutput"), enumFileType)

        Dim Report As New xrReports(udtUser.CodeLang, strConnection)
        Report.DisplaySubRecipeAstrisk = DisplaySubRecipeAstrisk 'DLS 09.08.2007
        Report.DisplaySubRecipeNormalFont = DisplaySubRecipeNormalFont 'DLS 09.08.2007
        Report.FooterAddress = FooterAddress 'DLS 28.08.2007
        Report.FooterLogoPath = FooterLogoPath 'DLS 28.08.2007
        Report.PictureOneRight = PictureOneRight 'DLS 28.08.2007
        Report.TitleColor = TitleColor
        Report.NoPrintLines = NoPrintLines 'DLS
        G_ReportOptions.flagNoLines = NoPrintLines 'DLS
        G_ReportOptions.strTitleColor = TitleColor 'DLS
        Report.DisplayRecipeDetails = DisplayRecipeDetails 'VRP 14.12.2007
        Report.strMigrosParam = strMigrosParam 'VRP 14.12.2007
        Report.blnThumbnailsView = blnThumbnailsView 'VRP 17.03.2008
        Report.strCnn = strConnection 'VRP 16.04.2008
        Report.udtUser = udtUser 'VRP 30.07.2008
        Report.SelectedWeek = SelectedWeek

        Dim strFooter As String = FooterAddress
        Dim strFooterSplit() As String = strFooter.Split("�")
        Dim strFooterAddressX As String = ""

        Select Case udtUser.CodeLang
            Case 2
                If UBound(strFooterSplit) >= 1 Then strFooterAddressX = strFooterSplit(1)
            Case 3
                If UBound(strFooterSplit) >= 2 Then strFooterAddressX = strFooterSplit(2)
            Case Else
                If UBound(strFooterSplit) >= 0 Then strFooterAddressX = strFooterSplit(0)
        End Select
        If strFooterAddressX = "" Then
            strFooterAddressX = strFooterSplit(0)
        End If

        G_ReportOptions.strFooterAddress = strFooterAddressX 'DLS 28.08.2007
        G_ReportOptions.strFooterLogoPath = FooterLogoPath 'DLS 28.08.2007
        G_ReportOptions.blnPictureOneRight = PictureOneRight 'DLS 28.08.2007

        ' Get Configuration
        G_ReportOptions.bIncludeGDAImage = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeGDAImage, clsConfig.CodeGroup.printprofile, "FALSE")
        G_ReportOptions.intfoodLaw = intFoodlaw
        Dim blnIncludeNumber As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeNumber, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeWastage As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeWastage, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeTax As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeTax, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeDate As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeDate, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludePrice As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePrice, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeCostOfGoods As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeCostOfGoods, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeFactor As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.prIncludeFactor, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeProfit As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeConst, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeSellingPrice As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeSellingPrice, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeImposedPrice As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeImposedPrice, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeGrossQty As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeGrossQty, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeNetQty As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeNetQty, clsConfig.CodeGroup.printprofile, "FALSE")

        Dim blnIncludeMetricGrossQty As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeMetricQtyGross, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeMetricNetQty As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeMetricQtyNet, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeImperialGrossQty As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeImperialQtyGross, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeImperialNetQty As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeImperialQtyNet, clsConfig.CodeGroup.printprofile, "FALSE")

        Dim blnIncludePicture As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePicture, clsConfig.CodeGroup.printprofile, "FALSE")
        '--- VRP 06.11.2007
        Dim strPictureOption As String = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrPictureOptions, clsConfig.CodeGroup.printprofile, "") 'VRP 06.11.2007
        Dim blnIncludePictureAll As Boolean
        Dim blnIncludePictureFirst As Boolean
        Dim blnIncludePictureRight As Boolean

        ' RDC 02.15.2013 
        ' Newly defined variables for newly added fields in the RECIPE DETAILS report.
        Dim blnIncludeNotes As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeRecipeNotes, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeSubTitle As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeSubtitle, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeServeWith As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeServeWith, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeTimeTypes As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeTimes, clsConfig.CodeGroup.printprofile, "FALSE")
        'Dim blnIncludeComplementPreparation As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeIngredientComplement, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeIngreientComplement As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeIngredientComplement, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeIngreientPreparation As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeIngredientPreparation, clsConfig.CodeGroup.printprofile, "FALSE")

        ' RDC 02.22.2013
        ' Added Brands and Placements in report layout
        Dim blnIncludeBrands As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeBrands, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludePublications As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePublication, clsConfig.CodeGroup.printprofile, "FALSE")
        'Dim blnIncludePlacements As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePlacements, clsConfig.CodeGroup.printprofile, "FALSE")
        ' End

        ' RDC 02.25.2013
        ' Added Procedure Sequence Number
        Dim blnIncludeProcSeqNumber As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeProcSeqNumber, clsConfig.CodeGroup.printprofile, "FALSE")
        ' End

        ' RDC 04.18.2013 - CWM-5350 Fix
        Dim blnIncludeMetricQtyGross As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeMetricQtyGross, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeMetricQtyNet As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeMetricQtyNet, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeImperialQtyGross As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeImperialQtyGross, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeImperialQtyNet As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeImperialQtyNet, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeAlternativeIngredient As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeAlternativeIngredient, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeHACCP As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeHACCP, clsConfig.CodeGroup.printprofile, "FALSE")
        ' End

        ' RDC 04.30.2013 - CWM-5517 Fix
        Dim blnIncludeHighlightSection As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeHighlightSection, clsConfig.CodeGroup.printprofile, "FALSE")

        ' RDC 03.20.2013 - Load Picture type in Manor
        ' Default to 0
        ' 0 - All
        ' 1 - Left
        ' 2 - Right
        Dim blnLoadPictureType As Integer = 0

        Select Case strPictureOption
            Case "20064"
                blnIncludePictureAll = True
                blnIncludePictureFirst = False
                blnIncludePictureRight = False
            Case "20063"
                blnIncludePictureFirst = True
                blnIncludePictureRight = False
                blnIncludePictureAll = False
                ' RDC 03.20.2013 - Load Picture type in Manor
                blnLoadPictureType = 1
            Case "20146"
                blnIncludePictureRight = True
                blnIncludePictureFirst = False
                blnIncludePictureAll = False
                ' RDC 03.20.2013 - Load Picture type in Manor
                blnLoadPictureType = 2
        End Select

        ' RDC 03.20.2013 - Load Picture type in Manor
        G_ReportOptions.blnLoadPictureType = blnLoadPictureType

        '------
        'Dim blnIncludePictureAll As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePictureAll, clsConfig.CodeGroup.printprofile, "FALSE")
        'Dim blnIncludePictureFirst As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePictureFirst, clsConfig.CodeGroup.printprofile, "FALSE")

        Dim blnIncludeInfo As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeInfo, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeNutrient As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeNutrient, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeGDA As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeGDA, clsConfig.CodeGroup.printprofile, "FALSE") 'DLS 11.08.2007
        Dim blnIncludeKeyword As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeKeyword, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeAllergens As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.prIncludeAllergens, clsConfig.CodeGroup.printprofile, "FALSE") ''AMTLA 2014.07.02 DRR 07.05.2012

        ' RDC 04.18.2013 - Removed
        'Dim blnIncludeHACCP As Boolean = False 'cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeHACCP, clsConfig.CodeGroup.printprofile, "FALSE") DRR 07.05.2012
        Dim blnIncludeCategory As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeCategory, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeSource As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeSource, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncluderemark As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeRemark, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeIngredientNumber As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeIngredientNumber, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeIngredientPreparation As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeIngredientPreparation, clsConfig.CodeGroup.printprofile, "FALSE") 'RDTC 23.05.2007
        Dim blnIncludeIngredientComplement As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeIngredientComplement, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludePreparation As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeProcedure, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeCookingTip As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeCookingTip, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludePrice2 As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludePrice2, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeSupplier As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeSupplier, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnRemoveTrailingZeros As Boolean = udtUser.RemoveTrailingZeroes
        Dim blnIncludeDerivedKeyword As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeDerivedKeyword, clsConfig.CodeGroup.printprofile, "FALSE") 'VRP 11.09.2008

        Dim blnIncludeRecipeMenuName As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeRecipeListName, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeSubName As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeSubtitle, clsConfig.CodeGroup.printprofile, "FALSE")


        'AGL 2012.12.12
        Dim blIncludeMetric As Boolean
        Dim blIncludeImperial As Boolean
        If blnIsAllowMetricImperial = False Then
            blIncludeMetric = False
            blIncludeImperial = False
        Else
            blIncludeMetric = CBool(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrintDetailsUseMetric, clsConfig.CodeGroup.printprofile, "TRUE"))
            blIncludeImperial = CBool(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrintDetailsUseImperial, clsConfig.CodeGroup.printprofile, "TRUE"))
        End If


        Dim blnPicturePathAccessible As Boolean = True
        Dim dblPageWidth As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrPaperPageWidth, clsConfig.CodeGroup.printprofile, "827") 'original 850
        Dim dblPageHeight As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrPaperPageHeight, clsConfig.CodeGroup.printprofile, "1169") 'original 1100
        Dim dblListLeftMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListLeftMargin, clsConfig.CodeGroup.printprofile, "1")
        Dim dblListRightMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListRightMargin, clsConfig.CodeGroup.printprofile, "1")
        Dim dblListTopMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListTopMargin, clsConfig.CodeGroup.printprofile, "1")
        Dim dblListBottomMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListBottomMargin, clsConfig.CodeGroup.printprofile, "1")
        Dim marginUnit As enumPrintUnits = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrMarginUnit, clsConfig.CodeGroup.printprofile, CStr(enumPrintUnits.inch))
        Dim dblMarginFactor As Double = 100

        'Dim strListFontName As String = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListFont, clsConfig.CodeGroup.printprofile, "Arial")
        'Dim sgListFontSize As Single = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListFontSize, clsConfig.CodeGroup.printprofile, "9")
        Dim strFontTitleName As String = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrFontTitle, clsConfig.CodeGroup.printprofile, "Arial"), udtUser.CodeTrans, "Arial") 'VRP 31.10.2007
        Dim sgFontTitleSize As Single = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrFontTitleSize, clsConfig.CodeGroup.printprofile, "16"), udtUser.CodeTrans, "16") 'VRP 31.10.2007 'original 16
        Dim strListFontName As String = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListFont, clsConfig.CodeGroup.printprofile, "Arial"), udtUser.CodeTrans, "Arial")
        Dim sgListFontSize As Single = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListFontSize, clsConfig.CodeGroup.printprofile, "9"), udtUser.CodeTrans, "9")
        Dim dblListLineSpace As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListLineSpacing, clsConfig.CodeGroup.printprofile, "1")
        'Dim strDetailFontName1 As String = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFont1, clsConfig.CodeGroup.printprofile, "Arial")
        'Dim strDetailFontName2 As String = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFont2, clsConfig.CodeGroup.printprofile, "Arial")
        'Dim sgDetailFontSize1 As Single = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFontSize1, clsConfig.CodeGroup.printprofile, "9")
        'Dim sgDetailFontSize2 As Single = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFontSize2, clsConfig.CodeGroup.printprofile, "9")
        Dim strDetailFontName1 As String = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFont2, clsConfig.CodeGroup.printprofile, "Arial"), udtUser.CodeTrans, "Arial")
        Dim strDetailFontName2 As String = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFont2, clsConfig.CodeGroup.printprofile, "Arial"), udtUser.CodeTrans, "Arial")
        Dim sgDetailFontSize1 As Single = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFontSize1, clsConfig.CodeGroup.printprofile, "9"), udtUser.CodeTrans, "9")
        Dim sgDetailFontSize2 As Single = GetFont(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardFontSize2, clsConfig.CodeGroup.printprofile, "9"), udtUser.CodeTrans, "9")

        Dim dblDetailLineSpace As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardLineSpacing, clsConfig.CodeGroup.printprofile, "1")
        Dim dblDetailLeftMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardLeftMargin, clsConfig.CodeGroup.printprofile, ".8")
        Dim dblDetailRightMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardRightMargin, clsConfig.CodeGroup.printprofile, ".8")
        Dim dblDetailTopMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardTopMargin, clsConfig.CodeGroup.printprofile, "1")
        Dim dblDetailBottomMargin As Double = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardBottomMargin, clsConfig.CodeGroup.printprofile, "1")

        Dim strTextItemFormat As String = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrCardTextItem, clsConfig.CodeGroup.printprofile, "0_0_0_0")

        Dim printOption As enumPrintOptions = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrPrintOptions, clsConfig.CodeGroup.printprofile, CStr(enumPrintOptions.RecipeCosting))
        Dim variation As enumPrintVariation = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrPrintVariation, clsConfig.CodeGroup.printprofile, CStr(enumPrintVariation.SmallPicture_Quantity_Name))
        Dim printStyle As String = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrStyle, clsConfig.CodeGroup.printprofile, CStr(enumPrintStyle.Standard))

        ' RDC 07.09.2013
        Dim blnIncludeDescription As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeDescription, clsConfig.CodeGroup.printprofile, "TRUE")
        Dim blnIncludeAddNotes As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeAddtionalNotes, clsConfig.CodeGroup.printprofile, "TRUE")
        Dim blnIncludeCookbook As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeCookbook, clsConfig.CodeGroup.printprofile, "FALSE")
        Dim blnIncludeKiosk As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeKiosk, clsConfig.CodeGroup.printprofile, "FALSE")

        ' RDC 07.10.2013
        Dim blnIncludeComment As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeComment, clsConfig.CodeGroup.printprofile, "TRUE")

        ' RDC 2014.11.17
        Dim blnIncludeComposition As Boolean = cConfig.GetConfig(intCodePrintProfile, 20406, clsConfig.CodeGroup.printprofile, "TRUE")

        ' RDC 07.24.2013 : Recipe Status
        Dim blnIncludeRecipeStatus As Boolean = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeRecipeStatus, clsConfig.CodeGroup.printprofile, "TRUE")

        ' RDC 07.25.2013 : Nutrient Set
        Dim intSelectedNutrientSet As Integer = CIntDB(dtProfile.Rows(0)("Codeset")) 'cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeNutrientSet, clsConfig.CodeGroup.printprofile, "0")

        Select Case CType(dtProfile.Rows(0).Item("documentoutput"), enumFileType)
            Case enumFileType.HTML
                If dblDetailLineSpace < 1 Then dblDetailLineSpace = 1
                If dblListLineSpace < 1 Then dblListLineSpace = 1
        End Select

        ' SET MARGINS
        Select Case marginUnit
            Case enumPrintUnits.inch
                dblMarginFactor = 1
            Case enumPrintUnits.centimeter
                dblMarginFactor = 2.54
                'dblMarginFactor = 645.6
            Case enumPrintUnits.millimeter
                dblMarginFactor = 25.4
                'dblMarginFactor = 2540
        End Select

        dblListLeftMargin = (dblListLeftMargin / dblMarginFactor) * 100
        dblListRightMargin = (dblListRightMargin / dblMarginFactor) * 100
        dblListTopMargin = (dblListTopMargin / dblMarginFactor) * 100
        dblListBottomMargin = (dblListBottomMargin / dblMarginFactor) * 100

        dblDetailLeftMargin = (dblDetailLeftMargin / dblMarginFactor) * 100
        dblDetailRightMargin = (dblDetailRightMargin / dblMarginFactor) * 100
        dblDetailTopMargin = (dblDetailTopMargin / dblMarginFactor) * 100
        dblDetailBottomMargin = (dblDetailBottomMargin / dblMarginFactor) * 100

        G_ReportOptions.bFoodcostOnly = CBoolDB(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrShowFoodcostOnly, clsConfig.CodeGroup.printprofile, "False")) 'JRN 21.05.2010 -SV Show food cost only


        G_ReportOptions.blnRemoveTrailingZeros = blnRemoveTrailingZeros
        G_ReportOptions.blnMode = CBoolDB(ds2.Tables(0).Rows(0).Item("Mode")) '// DRR 07.05.2012

        ' RDC 01.07.2014 : Use Fractions
        G_ReportOptions.blnUseFractions = CBoolDB(cConfig.GetConfig(udtUser.Code, clsConfig.enumNumeros.UIDisplayQuantitiesAsFractions, clsConfig.CodeGroup.user, "False"))

        ' HANDLE SORTING
        Dim strSubHeader As String = ""
        Dim strReportTitle As String = ""
        Dim cLang As New clsEGSLanguage(udtUser.CodeLang)

        strSubHeader = cLang.GetString(clsEGSLanguage.CodeType.SortBy)

        Select Case sortBy
            Case enumPrintSortType.Category
                dtDetails.DefaultView.Sort = "categoryname, name "
                G_ReportOptions.strSortBy = "CategoryName"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Category)
            Case enumPrintSortType.Dates
                dtDetails.DefaultView.Sort = "dates, name "
                G_ReportOptions.strSortBy = "Dates"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Date_)
            Case enumPrintSortType.Number
                'dtDetail.DefaultView.Sort = "numberlen, number, name "
                'dtDetail.DefaultView.Sort = "number, name "
                G_ReportOptions.strSortBy = "Number"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Number)
            Case enumPrintSortType.Price
                If printType = enumReportType.ShoppingListDetail Then
                    dtDetails.DefaultView.Sort = "price, name "
                    G_ReportOptions.strSortBy = "Price"
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Price)
                ElseIf printType = enumReportType.MerchandiseList Then
                    dtDetails.DefaultView.Sort = "realitemPrice, name "
                    G_ReportOptions.strSortBy = "Price"
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Price)
                Else
                    dtDetails.DefaultView.Sort = "itemPrice, name "
                    G_ReportOptions.strSortBy = "Price"
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Price)
                End If
            Case enumPrintSortType.Tax
                dtDetails.DefaultView.Sort = "tax, name "
                G_ReportOptions.strSortBy = "Tax"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Tax)
            Case enumPrintSortType.SellingPrice
                dtDetails.DefaultView.Sort = "sellingprice, name "
                G_ReportOptions.strSortBy = "SellingPrice"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Selling_Price)
            Case enumPrintSortType.ImposedPrice
                dtDetails.DefaultView.Sort = "imposedprice, name "
                G_ReportOptions.strSortBy = "ImposedPrice"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.ImposedPrice)
            Case enumPrintSortType.CostOfGoods
                dtDetails.DefaultView.Sort = "calcprice,name "
                G_ReportOptions.strSortBy = "CalcPrice"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.CostOfGoods)
            Case enumPrintSortType.Const
                dtDetails.DefaultView.Sort = "coeff, name "
                G_ReportOptions.strSortBy = "Profit"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Profit)
            Case enumPrintSortType.Supplier
                If printType = enumReportType.ShoppingListDetail Then
                    dtDetails.DefaultView.Sort = "Supplier,name "
                Else
                    dtDetails.DefaultView.Sort = "NameRef,name "
                End If
                G_ReportOptions.strSortBy = "Supplier"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Supplier)
            Case enumPrintSortType.GrossQty
                dtDetails.DefaultView.Sort = "GrossQty, name "
                G_ReportOptions.strSortBy = "GrossQty"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Gross_Qty)
            Case enumPrintSortType.Amount
                dtDetails.DefaultView.Sort = "Amount, name "
                G_ReportOptions.strSortBy = "Amount"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Amount)
            Case enumPrintSortType.NetQty
                dtDetails.DefaultView.Sort = "netQty, name "
                G_ReportOptions.strSortBy = "NetQty"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Net_Qty)
            Case enumPrintSortType.Name
                dtDetails.DefaultView.Sort = "name "
                G_ReportOptions.strSortBy = "Name"
                '-- JBB 06.25.2012
                If printType = enumReportType.RecipeDetail Or printType = enumReportType.RecipeList Then
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Title)
                Else
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Name)
                End If
                '--
                'strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Name)
                '--
            Case enumPrintSortType.Wastage  'mcm 26.01.06
                dtDetails.DefaultView.Sort = "Totalwastage, name "
                G_ReportOptions.strSortBy = "Wastage"
                strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Wastage)
            Case Else
                dtDetails.DefaultView.Sort = "name"
                G_ReportOptions.strSortBy = ""
                '-- JBB 06.25.2012
                If printType = enumReportType.RecipeDetail Or printType = enumReportType.RecipeList Then
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Title)
                Else
                    strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Name)
                End If
                '--
                'strSubHeader &= " " & cLang.GetString(clsEGSLanguage.CodeType.Name)
                '--
        End Select

        G_ReportOptions.intYieldOption = cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrListYieldOption, clsConfig.CodeGroup.printprofile, "1")
        G_ReportOptions.blIncludeMetric = blIncludeMetric
        G_ReportOptions.blIncludeImperial = blIncludeImperial
        G_ReportOptions.blnIncludeNetQty = CBool(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeNetQty, clsConfig.CodeGroup.printprofile, "TRUE"))
        G_ReportOptions.blnIncludeGrossQty = CBool(cConfig.GetConfig(intCodePrintProfile, clsConfig.enumNumeros.PrIncludeGrossQty, clsConfig.CodeGroup.printprofile, "TRUE"))

        Select Case printType
            Case enumReportType.MerchandisePriceList
                With G_ReportOptions
                    .dblReportType = 0
                    .intPageLanguage = udtUser.CodeLang
                    .dblLineSpace = dblListLineSpace
                    Return Report.fctPrintMerchandisePriceList(dtDetails, strSubHeader, .intPageLanguage,
                                    blnIncludeNumber, blnIncludeSupplier, blnIncludeCategory, blnIncludePrice, blnIncludePrice2,
                                    strListFontName, sgListFontSize, dblPageWidth, dblPageHeight, dblListLeftMargin, dblListRightMargin,
                                    dblListTopMargin, dblListBottomMargin, False,
                                    strFontTitleName, sgFontTitleSize, userLocale:=userLocale) 'VRP 05.11.2007
                End With
            Case enumReportType.MerchandiseList
                With G_ReportOptions
                    .dblReportType = 0
                    .intPageLanguage = udtUser.CodeLang
                    .dblLineSpace = dblListLineSpace
                    Return Report.fctPrintMerchandiseList(dtDetails, strSubHeader, .intPageLanguage,
                                    blnIncludeNumber, blnIncludeWastage, blnIncludeTax, blnIncludeDate, blnIncludePrice,
                                    strListFontName, sgListFontSize, dblPageWidth, dblPageHeight, dblListLeftMargin,
                                    dblListRightMargin, dblListTopMargin, dblListBottomMargin, False,
                                    strFontTitleName, sgFontTitleSize, userLocale:=userLocale) 'VRP 05.11.2007

                End With


            Case enumReportType.RecipeList
                strReportTitle = cLang.GetString(clsEGSLanguage.CodeType.RecipeList)
                With G_ReportOptions
                    .dblReportType = 0
                    .intPageLanguage = udtUser.CodeLang
                    .dblLineSpace = dblListLineSpace
                    .blnRecipe = True
                    'Return Report.fctPrintRecipeMenuList(dtDetail, strReportTitle, strSubHeader, .intPageLanguage, _
                    '                blnIncludeNumber, blnIncludeCostOfGoods, blnIncludeProfit, blnIncludeTax, blnIncludeSellingPrice, _
                    '                blnIncludeImposedPrice, blnIncludeDate, strListFontName, sgListFontSize, dblPageWidth, dblPageHeight, _
                    '                dblListLeftMargin, dblListRightMargin, dblListTopMargin, dblListBottomMargin, False, _
                    '                strFontTitleName, sgFontTitleSize) 'VRP 05.11.2007

                    Return Report.PrintRecipeMenuListCoop(dtDetails, strReportTitle, strSubHeader, .intPageLanguage,
                                   blnIncludeNumber, blnIncludeCostOfGoods, blnIncludeProfit, blnIncludeTax, blnIncludeSellingPrice,
                                   blnIncludeImposedPrice, blnIncludeDate, strListFontName, sgListFontSize, dblPageWidth, dblPageHeight,
                                   dblListLeftMargin, dblListRightMargin, dblListTopMargin, dblListBottomMargin, False,
                                   strFontTitleName, sgFontTitleSize, blnIncludeRecipeMenuName, blnIncludeSubName, blnIncludeCategory) 'VRP 05.11.2007


                End With
            Case enumReportType.MenuList
                strReportTitle = cLang.GetString(clsEGSLanguage.CodeType.MenuList)
                With G_ReportOptions
                    .dblReportType = 0
                    .intPageLanguage = udtUser.CodeLang
                    .dblLineSpace = dblListLineSpace
                    .blnRecipe = False
                    'Return Report.fctPrintRecipeMenuList(dtDetail, strReportTitle, strSubHeader, .intPageLanguage, _
                    '                blnIncludeNumber, blnIncludeCostOfGoods, blnIncludeProfit, blnIncludeTax, blnIncludeSellingPrice, _
                    '                blnIncludeImposedPrice, blnIncludeDate, strListFontName, sgListFontSize, dblPageWidth, dblPageHeight, _
                    '                dblListLeftMargin, dblListRightMargin, dblListTopMargin, dblListBottomMargin, False, _
                    '                strFontTitleName, sgFontTitleSize) 'VRP 05.11.2007

                    Return Report.PrintRecipeMenuListCoop(dtDetails, strReportTitle, strSubHeader, .intPageLanguage,
                                   blnIncludeNumber, blnIncludeCostOfGoods, blnIncludeProfit, blnIncludeTax, blnIncludeSellingPrice,
                                   blnIncludeImposedPrice, blnIncludeDate, strListFontName, sgListFontSize, dblPageWidth, dblPageHeight,
                                   dblListLeftMargin, dblListRightMargin, dblListTopMargin, dblListBottomMargin, False,
                                   strFontTitleName, sgFontTitleSize) 'VRP 05.11.2007

                End With
            Case enumReportType.MerchandiseNutrientList, enumReportType.RecipeNutrientList, enumReportType.MenuNutrientList
                G_ReportOptions.dblLineSpace = dblListLineSpace
                G_ReportOptions.intPageLanguage = udtUser.CodeLang
                G_ReportOptions.dblReportType = 0

                Dim type As enumDataListItemType
                Dim intNutrientQty As Integer
                Select Case printType
                    Case enumReportType.MerchandiseNutrientList
                        type = enumDataListItemType.Merchandise
                    Case enumReportType.RecipeNutrientList
                        type = enumDataListItemType.Recipe
                    Case enumReportType.MenuNutrientList
                        type = enumDataListItemType.Menu
                End Select

                Select Case printOption
                    Case enumPrintOptions.NutrientPerYieldUnit
                        intNutrientQty = 0
                    Case enumPrintOptions.NutrientPer100gOr100ml
                        intNutrientQty = 1
                    Case enumPrintOptions.NutrientBoth
                        intNutrientQty = 2
                End Select

                'Return Report.fctPrintNutrientValuesList(dtDetail, type, intNutrientQty, _
                '                               strListFontName, sgListFontSize, dblPageWidth, dblPageHeight, dblListLeftMargin, _
                '                               dblListRightMargin, dblListTopMargin, dblListBottomMargin, udtUser, _
                '                               strFontTitleName, sgFontTitleSize) 'VRP 05.11.2007

                'VRP 25.06.2009

                ''-- JBB 05.23.2012
                If printType = enumReportType.RecipeNutrientList Then
                    Dim strNutrientToPrint As String = cConfig.GetConfig(udtUser.Site.Code, clsConfig.enumNumeros.PrNutrientList, clsConfig.CodeGroup.site, "")
                    Dim arrNutrientToPrint As New ArrayList(strNutrientToPrint.Split("_"))
                    '' -- For Recipe Nutrient List
                    '' Check Nutrient to be Displayed
                    Dim arrblDisplay(42) As Boolean
                    For intCounter As Integer = 1 To 42
                        arrblDisplay(intCounter) = False
                    Next

                    ' ---------------------- For AFTER V47 -------------------------- Enhancement CWA-23653
                    'Dim strNutrientToDisplay As String = cConfig.GetConfig(udtUser.Site.Code, clsConfig.enumNumeros.RecipeDefaultNutrientShow.GetHashCode.ToString() + intSelectedNutrientSet.ToString(), clsConfig.CodeGroup.site, "0")
                    'Dim strNutrientToDisplays As String() = strNutrientToDisplay.Split("_")
                    ' ---------------------- For AFTER V47 --------------------------


                    If strNutrientToPrint = "" Then
                        For Each drDetails As DataRow In dtDetails.Rows
                            For intNIndex As Integer = 1 To 42
                                If CBoolDB(drDetails("N" & intNIndex & "display")) = True Then
                                    arrblDisplay(intNIndex) = True
                                End If
                            Next
                            ' ---------------------- For AFTER V47 -------------------------- Enhancement CWA-23653
                            'For intNIndex As Integer = 1 To strNutrientToDisplays.Length - 1
                            '    If Convert.ToInt32(strNutrientToDisplays(intNIndex - 1)) > 0 Then
                            '        arrblDisplay(intNIndex) = True
                            '    End If
                            'Next
                            ' ---------------------- For AFTER V47 --------------------------

                        Next
                        Dim intTop12 As Integer = 1
                        Dim intCTop As Integer = cConfig.GetConfig(0, clsConfig.enumNumeros.PrNutrientNumber, clsConfig.CodeGroup.global, 10)
                        For intCounter As Integer = 1 To 42
                            If arrblDisplay(intCounter) = True Then
                                If intTop12 <= intCTop Then
                                    intTop12 = intTop12 + 1
                                Else
                                    arrblDisplay(intCounter) = False
                                End If
                            End If
                        Next
                    Else
                        For Each drDetails As DataRow In dtDetails.Rows
                            For intNIndex As Integer = 1 To 42
                                If arrNutrientToPrint.Contains(intNIndex.ToString()) Then
                                    arrblDisplay(intNIndex) = True
                                End If
                            Next
                        Next
                    End If

                    Return Report.PrintRecipeNutrientList(dtDetails, type, intNutrientQty, strListFontName, sgListFontSize,
                                                   dblPageWidth, dblPageHeight, dblListLeftMargin, dblListRightMargin,
                                                   dblListTopMargin, dblListBottomMargin, udtUser, arrblDisplay, strFontTitleName, sgFontTitleSize) 'Will add printStyle KMQDC'

                Else
                    Return Report.PrintNutrientList(dtDetails, type, intNutrientQty, strListFontName, sgListFontSize,
                                                    dblPageWidth, dblPageHeight, dblListLeftMargin, dblListRightMargin,
                                                    dblListTopMargin, dblListBottomMargin, udtUser, strFontTitleName, sgFontTitleSize) 'Will add printStyle KMQDC'

                End If
                ''--
            Case enumReportType.ShoppingListDetail
                G_ReportOptions.dblLineSpace = dblListLineSpace
                G_ReportOptions.dblReportType = 0
                G_ReportOptions.intPageLanguage = udtUser.CodeLang
                G_ReportOptions.strFontName2 = strListFontName
                G_ReportOptions.strFontTitleName = strFontTitleName
                G_ReportOptions.sgFontSize2 = sgListFontSize
                G_ReportOptions.sgFontTitleSize = sgFontTitleSize

                ' rename field to group
                Dim strGroupBy As String = ""
                Dim blnEnableGroup As Boolean = False
                Select Case groupBy
                    Case enumPrintGroupType.Category
                        strGroupBy = "CategoryName"
                        G_ReportOptions.strGroupBy = strGroupBy
                        blnEnableGroup = True
                    Case enumPrintGroupType.None
                        blnEnableGroup = False
                        G_ReportOptions.strGroupBy = ""
                    Case enumPrintGroupType.Supplier
                        blnEnableGroup = True
                        strGroupBy = "Supplier"
                        G_ReportOptions.strGroupBy = strGroupBy
                End Select

                Return Report.fctPrintShoppingList(dtDetails, strSubHeader, blnEnableGroup, strGroupBy, blnIncludePrice, blnIncludeGrossQty, blnIncludeNetQty,
                 blnIncludeNumber, strListFontName, sgListFontSize, dblPageWidth, dblPageHeight, dblListLeftMargin, dblListRightMargin, dblListTopMargin, dblListBottomMargin, False,
                 strFontTitleName, sgFontTitleSize) 'VRP 05.11.2007

            Case enumReportType.MerchandiseDetail

                'MCM 03.01.06
                '----------------------------
                With G_ReportOptions
                    .dblLineSpace = dblDetailLineSpace
                    .dblReportType = 5
                    .dtDetail = dtDetails
                    .dtKeywords = dtKeyword
                    .dtAllergens = dtAllergen
                    .blnPicturePathAccessible = blnPicturePathAccessible
                    .blnWithPicture = blnIncludePicture
                    .blnPicturesAll = blnIncludePictureAll
                    .blnIncludeInfo = blnIncludeInfo
                    .blnIncludeNutrients = blnIncludeNutrient
                    .blnIncludeGDA = blnIncludeGDA 'DLS Aug112007
                    .blnIncludeKeyword = blnIncludeKeyword
                    .blnIncludeDerivedKeyword = blnIncludeDerivedKeyword
                    .blnIncludeCookingTip = blnIncludeCookingTip
                    .intTranslation = udtUser.CodeTrans
                    '.strFontName = strDetailFontName1
                    .sgFontSize = sgDetailFontSize1
                    .strFontName2 = strDetailFontName2
                    .sgFontSize2 = sgDetailFontSize2
                    .dblPageWidth = dblPageWidth
                    .dblPageHeight = dblPageHeight
                    .dblLeftMargin = dblDetailLeftMargin
                    .dblRightMargin = dblDetailRightMargin
                    .dblTopMargin = dblDetailTopMargin
                    .dblBottomMargin = dblDetailBottomMargin
                    .intPageLanguage = udtUser.CodeLang
                    .blLandscape = False
                    .blnIncludeAllergens = blnIncludeAllergens
                    .strSubStyle = printStyle
                    '----------------------------
                    .strFontTitleName = strFontTitleName 'VRP 05.11.2007
                    .sgFontTitleSize = sgFontTitleSize 'VRP 05.11.2007
                    .blnPictureOneRight = blnIncludePictureRight 'VRP 06.11.2007
                    .dtProductLink = dtProductLink 'VRP 15.07.2008
                    .blnIncludeHighlightSection = blnIncludeHighlightSection    ' RDC 07.23.2013 : Higlight section
                    .blnIncludeRecipeStatus = blnIncludeRecipeStatus            ' RDC 07.24.2013 : Recipe Status
                    .intSelectedNutrientSet = intSelectedNutrientSet            ' RDC 08.02.2013 : Nutrient Set
                    .intfoodLaw = intFoodlaw

                    .blnIncludeComposition = blnIncludeComposition 'AGL 2014.11.17
                    Return Report.fctMasterReport(dtCodes, dblPageWidth, dblPageHeight, dblDetailLeftMargin,
                                                  dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin, .strFontName2, .sgFontSize2, False, printType,
                                                  .strFontTitleName, .sgFontTitleSize, CodePrintList, dtDetails, userLocale:=userLocale) 'VRP 05.11.2007
                End With


                'Return Report.fctPrintEgsMerchandiseDetails(dtDetail, blnIncludePicture, blnIncludeInfo, _
                '                blnIncludeNutrient, blnIncludeKeyword, udtUser.CodeLang, _
                '                strDetailFontName1, sgDetailFontSize1, strDetailFontName2, _
                '                sgDetailFontSize2, dblPageWidth, dblPageHeight, _
                '                dblDetailLeftMargin, dblDetailRightMargin, _
                '                dblDetailTopMargin, dblDetailBottomMargin, False)

            Case enumReportType.RecipeDetail, enumReportType.MenuDetail
                Dim type As enumDataListItemType
                Select Case printType
                    Case enumReportType.MerchandiseDetail
                        type = enumDataListItemType.Merchandise
                    Case enumReportType.RecipeDetail
                        type = enumDataListItemType.Recipe
                    Case enumReportType.MenuDetail
                        type = enumDataListItemType.Menu
                End Select
                '   Return Report.fctPrintRecipeEgsLayout(dtDetail, "64", False, type, udtUser.CodeTrans, strDetailFontName1, sgDetailFontSize1, _
                '   dblPageWidth, dblPageHeight, dblDetailLeftMargin, dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin, False)

                Select Case printOption
                    Case enumPrintOptions.RecipeCosting, enumPrintOptions.RecipeCostingAndPreparation, enumPrintOptions.MenuCosting, enumPrintOptions.MenuCostingAndDescription

                        'MCM 03.01.06
                        '----------------------------
                        type = enumDataListItemType.Recipe
                        With G_ReportOptions
                            'mcm 13.01.05
                            Select Case printOption
                                Case enumPrintOptions.RecipeCosting,
                                    enumPrintOptions.RecipeCostingAndPreparation,
                                    enumPrintOptions.MenuCosting,
                                    enumPrintOptions.MenuCostingAndDescription 'AGL 2014.03.13 - added enumPrintOptions.MenuCosting, enumPrintOptions.MenuCostingAndDescription
                                    .dblReportType = 6
                                    'Case enumPrintOptions.MenuCosting, enumPrintOptions.MenuCostingAndDescription
                                    '    .dblReportType = 7
                            End Select
                            If CInt(printStyle) = 10 Then
                                .dblReportType = 1
                                .blnMigrosCustomPrint = True
                            Else
                                .dblReportType = 6
                            End If
                            .dblLineSpace = dblDetailLineSpace
                            .intPageLanguage = udtUser.CodeLang
                            .dtDetail = dtDetails
                            .dtKeywords = dtKeyword
                            .dtAllergens = dtAllergen
                            .blnRecipe = type
                            .blnIncludeNumber = blnIncludeNumber
                            .blnIncludeCategory = blnIncludeCategory
                            .blnIncludeSource = blnIncludeSource
                            .blnIncludeDate = blnIncludeDate
                            .blnIncludeCostOfGoods = blnIncludeCostOfGoods
                            .blnIncludeRemark = blnIncluderemark
                            .blnIncludeIngrNumber = blnIncludeIngredientNumber
                            .blnIncludeIngrPreparation = blnIncludeIngredientPreparation   'RDTC 23.05.2007
                            .blnIncludePreparation = blnIncludePreparation
                            .blnIncludeNutrients = blnIncludeNutrient
                            .blnIncludeGDA = blnIncludeGDA 'DLS Aug112007
                            .blnIncludeHACCP = blnIncludeHACCP
                            .intTranslation = udtUser.CodeTrans
                            .blnIncludeKeyword = blnIncludeKeyword
                            .blnPicturesAll = blnIncludePictureAll
                            .blnWithPicture = blnIncludePicture
                            .blnPicturePathAccessible = blnPicturePathAccessible
                            .strFontName = strDetailFontName1
                            .sgFontSize = sgDetailFontSize1
                            .strFontName2 = strDetailFontName2
                            .sgFontSize2 = sgDetailFontSize2
                            .dblPageWidth = dblPageWidth
                            .dblPageHeight = dblPageHeight
                            .dblLeftMargin = dblDetailLeftMargin
                            .dblRightMargin = dblDetailRightMargin
                            .dblTopMargin = dblDetailTopMargin
                            .dblBottomMargin = dblDetailBottomMargin
                            .blLandscape = False
                            .blnIncludeNetQty = blnIncludeNetQty
                            .blnIncludeGrossQty = blnIncludeGrossQty
                            .blnIncludeAllergens = blnIncludeAllergens
                            .strTextItemFormat = strTextItemFormat
                            .strFontTitleName = strFontTitleName 'VRP 05.11.2007
                            .sgFontTitleSize = sgFontTitleSize 'VRP 05.11.2007
                            .blnPictureOneRight = blnIncludePictureRight 'VRP 06.11.2007
                            .dtSteps = dtSteps 'VRP 19.05.2008
                            .dtListeNote = dtListeNote ' JBB 06.30.2012
                            .blnIncludeHighlightSection = blnIncludeHighlightSection    ' RDC 07.23.2013 : Higlight section
                            '----------------------------
                            .blnIncludeDerivedKeyword = blnIncludeDerivedKeyword 'VRP 11.09.2008
                            .dtProfile = dtProfile ' RDC 09.05.2013
                            .dtNotes = dtNotes
                            .dtBrands = dtBrands
                            .dtKiosk = dtKiosk
                            .dtPublications = dtPublications
                            .dtCookbook = dtCookbook
                            .dtComment = dtComment
                            .dtCodes = dtCodes
                            .blnIncludeNotes = blnIncludeNotes
                            .blnIncludeAddNotes = blnIncludeAddNotes
                            .blnIncludeComment = blnIncludeComment

                            Select Case printOption
                                Case enumPrintOptions.RecipeCosting
                                    .blnIncludeKiosk = blnIncludeKiosk
                                    .blnIncludeBrand = blnIncludeBrands
                                    .blnIncludePublication = blnIncludePublications
                                    .blnIncludeComposition = blnIncludeComposition
                                    .blnIncludeCookbook = blnIncludeCookbook
                                    .blnRecipe = True
                                Case enumPrintOptions.MenuCosting
                                    .blnIncludeKiosk = False
                                    .blnIncludeBrand = False
                                    .blnIncludePublication = False
                                    .blnIncludeComposition = False
                                    .blnIncludeCookbook = False
                                    .blnRecipe = False
                            End Select
                            .blnIncludeIngredientComplement = blnIncludeIngredientComplement
                            .blnIncludeProcSequenceNo = blnIncludeProcSeqNumber
                            'Return Report.fctMasterReport(dtCodes, dblPageWidth, dblPageHeight, dblDetailLeftMargin, _
                            '                                dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin, _
                            '                                strDetailFontName2, sgDetailFontSize2, False, printType, _
                            '                                .strFontTitleName, .sgFontTitleSize) 'VRP 05.11.2007

                            If DisplayRecipeDetails = 2 Then 'ADF 'VRP 15.08.2008
                                Dim dtNew As New DataTable
                                dtNew.Columns.Add("Code")
                                dtNew.Columns.Add("Name")
                                dtNew.Columns.Add("CodeListeMain")
                                dtNew.Columns.Add("CodeListeParent")

                                Dim rowNew As DataRow
                                For Each row As DataRow In dtCodes.Rows
                                    If CIntDB(row("CodeListeParent")) = 0 Then
                                        rowNew = dtNew.NewRow
                                        rowNew("Code") = row("Code")
                                        rowNew("Name") = row("Name")
                                        rowNew("CodeListeMain") = row("Codelistemain")
                                        rowNew("CodeListeParent") = row("CodeListeParent")
                                        dtNew.Rows.Add(rowNew)
                                        For Each rowDet As DataRow In dtCodes.Rows
                                            If CIntDB(row("Code")) = CIntDB(rowDet("CodeListeMain")) And CIntDB(row("Code")) = CIntDB(rowDet("CodeListeParent")) Then
                                                rowNew = dtNew.NewRow
                                                rowNew("Code") = rowDet("Code")
                                                rowNew("Name") = rowDet("Name")
                                                rowNew("CodeListeMain") = rowDet("Codelistemain")
                                                rowNew("CodeListeParent") = rowDet("CodeListeParent")
                                                dtNew.Rows.Add(rowNew)
                                                For Each rowDet2 As DataRow In dtCodes.Rows
                                                    If CIntDB(row("Code")) = CIntDB(rowDet2("CodeListeMain")) And CIntDB(row("Code")) <> CIntDB(rowDet2("CodeListeParent")) _
                                                        And CIntDB(rowDet("Code")) <> CIntDB(rowDet2("CodeListeMain")) And CIntDB(rowDet("Code")) = CIntDB(rowDet2("CodeListeparent")) Then
                                                        rowNew = dtNew.NewRow
                                                        rowNew("Code") = rowDet2("Code")
                                                        rowNew("Name") = rowDet2("Name")
                                                        rowNew("CodeListeMain") = rowDet2("Codelistemain")
                                                        rowNew("CodeListeParent") = rowDet2("CodeListeParent")
                                                        dtNew.Rows.Add(rowNew)
                                                    End If
                                                Next
                                            End If
                                        Next
                                    End If
                                Next

                                Return Report.fctMasterReport(dtNew, dblPageWidth, dblPageHeight, dblDetailLeftMargin,
                                                           dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin,
                                                           strDetailFontName2, sgDetailFontSize2, False, printType,
                                                           .strFontTitleName, .sgFontTitleSize) 'VRP 05.11.2007
                            Else
                                Return Report.fctMasterReport(dtCodes, dblPageWidth, dblPageHeight, dblDetailLeftMargin,
                                                           dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin,
                                                           strDetailFontName2, sgDetailFontSize2, False, printType,
                                                           .strFontTitleName, .sgFontTitleSize, CodePrintList, dtDetails, dtNotes, dtSteps, dtListeNote) 'VRP 05.11.2007
                            End If

                        End With
                    Case enumPrintOptions.RecipePreparation, enumPrintOptions.MenuDescription
                        Dim strSubStyle As String = ""
                        If variation < 10 Then
                            strSubStyle = "0" & CStr(variation)
                        Else
                            strSubStyle = CStr(variation)
                        End If

                        'MCM 03.01.06
                        '----------------------------
                        With G_ReportOptions
                            If printOption = enumPrintOptions.MenuDescription Then
                                printStyle += 20 'mcm 13.01.06  for menu description reports
                            End If

                            If CInt(printStyle) = 10 Then
                                .dblReportType = 1
                                .blnMigrosCustomPrint = True
                            Else
                                .dblReportType = CInt(printStyle)
                            End If

                            .dblLineSpace = dblDetailLineSpace
                            .intPageLanguage = udtUser.CodeLang
                            '.dblReportType = CInt(printStyle)
                            .dtDetail = dtDetails
                            .dtKeywords = dtKeyword
                            .dtAllergens = dtAllergen
                            .strSubStyle = strSubStyle
                            .blnIncludeNutrients = blnIncludeNutrient
                            .blnIncludeGDA = blnIncludeGDA 'DLS Aug112007
                            .blnIncludeHACCP = blnIncludeHACCP
                            .blnPicturePathAccessible = blnPicturePathAccessible
                            .blnIncludeNumber = blnIncludeNumber
                            .blnIncludeCategory = blnIncludeCategory
                            .blnIncludeSource = blnIncludeSource
                            .blnIncludeDate = blnIncludeDate
                            .blnIncludeCostOfGoods = blnIncludeCostOfGoods
                            .blnIncludeRemark = blnIncluderemark
                            .blnIncludeIngrNumber = blnIncludeIngredientNumber
                            .blnIncludeIngrPreparation = blnIncludeIngredientPreparation   'RDTC 23.05.2007
                            .blnWithPicture = blnIncludePicture
                            .blnIncludePicture = blnIncludePicture
                            .intTranslation = udtUser.CodeTrans
                            .blnIncludePreparation = blnIncludePreparation
                            .blnIncludeKeyword = blnIncludeKeyword
                            .blnPicturesAll = blnIncludePictureAll
                            .strFontName = strDetailFontName1
                            .sgFontSize = sgDetailFontSize1
                            .strFontName2 = strDetailFontName2
                            .sgFontSize2 = sgDetailFontSize2
                            .dblPageWidth = dblPageWidth
                            .dblPageHeight = dblPageHeight
                            .dblLeftMargin = dblDetailLeftMargin
                            .dblRightMargin = dblDetailRightMargin
                            .dblTopMargin = dblDetailTopMargin
                            .dblBottomMargin = dblDetailBottomMargin
                            .blLandscape = False
                            .blnIncludeAllergens = blnIncludeAllergens
                            .strTextItemFormat = strTextItemFormat
                            .strFontTitleName = strFontTitleName 'VRP 05.11.2007
                            .sgFontTitleSize = sgFontTitleSize 'VRP 05.11.2007
                            .blnPictureOneRight = blnIncludePictureRight 'VRP 06.11.2007
                            .dtSteps = dtSteps 'VRP 19.05.2008
                            .dtListeNote = dtListeNote ' JBB 06.30.2012
                            '----------------------------
                            .blnIncludeDerivedKeyword = blnIncludeDerivedKeyword 'VRP 11.09.2008

                            ' RDC 02.15.2013 New Fields for RECIPE DETAILS Report
                            .dtNotes = dtNotes
                            .dtSubtitle = dtSubTitles
                            .dtComplementPreparation = dtComplePrep
                            .dtTimeTypes = dtTimeTypes
                            .blnIncludeNotes = blnIncludeNotes
                            .blnIncludeSubtitle = blnIncludeSubTitle
                            .blnIncludeIngredientPreparation = blnIncludeIngredientPreparation
                            .blnIncludeIngredientComplement = blnIncludeIngredientComplement
                            .blnIncludeTimeTypes = blnIncludeTimeTypes
                            ' End

                            ' RDC 02.22.2013 New Fields for RECIPE DETAILS Report
                            ' Publication and Brands field
                            .blnIncludeBrand = blnIncludeBrands
                            .blnIncludePublication = blnIncludePublications
                            .dtBrands = dtBrands
                            .dtPublications = dtPublications
                            'End

                            ' RDC 02.25.2013 Enabling or Disabling Procedure Sequence Number
                            .blnIncludeProcSequenceNo = blnIncludeProcSeqNumber
                            ' End

                            ' RDC 02.27.2013 PDF line count initialization
                            .intDatalines = 0
                            'End

                            ' RDC 04.26.2013 - Additional Report Options
                            .blnIncludeGrossQty = blnIncludeGrossQty
                            .blnIncludeNetQty = blnIncludeNetQty

                            ' RDC 04.30.2013 - CWM-5517 Fix
                            .blnIncludeHighlightSection = blnIncludeHighlightSection

                            ' RDC 05.15.2013 - Wastage and Alternative ingredient
                            .blnIncludeWastage = blnIncludeWastage
                            .blnIncludeAlternativeIngredient = blnIncludeAlternativeIngredient
                            .blnIncludeMetricQtyGross = blnIncludeMetricQtyGross
                            .blnIncludeMetricQtyNet = blnIncludeMetricQtyNet
                            .blnIncludeImperialQtyGross = blnIncludeImperialQtyGross
                            .blnIncludeImperialQtyNet = blnIncludeImperialQtyNet

                            ' RDC 07.10.2013 - New fields for Recipe Detail report
                            .blnIncludeDescription = blnIncludeDescription
                            .dtComment = dtComment
                            .dtKiosk = dtKiosk
                            .dtCookbook = dtCookbook
                            .blnIncludeKiosk = blnIncludeKiosk
                            .blnIncludeComment = blnIncludeComment
                            .blnIncludeCookbook = blnIncludeCookbook
                            ' RDC 07.24.2013 : Recipe Status
                            .blnIncludeRecipeStatus = blnIncludeRecipeStatus
                            ' RDC 07.25.2013 : Nutrient Set
                            .intSelectedNutrientSet = intSelectedNutrientSet
                            .dtProfile = dtProfile
                            ' RDC 08.16.2013 : Additional notes section in Report
                            .blnIncludeAddNotes = blnIncludeAddNotes

                            .blnIncludeComposition = blnIncludeComposition 'AGL 2014.11.17

                            If DisplayRecipeDetails = 4 Then 'Recipe Center 'VRP 11.07.2008
                                .blnIncludeNutrients = False
                                .blnIncludeGDA = False
                                .blnIncludeCategory = False
                                .blnIncludeHACCP = False
                                .blnIncludeKeyword = False
                                .blnIncludeSource = False
                                .blnIncludeRemark = False
                                .blnIncludeDescription = False  ' RDC 07.10.2013
                                .blnIncludeRecipeStatus = False ' RDC 07.24.2013 : Recipe Status
                                .blnWithPicture = True
                                .intPageLanguage = udtUser.CodeLang
                                .intTranslation = udtUser.CodeTrans
                                Report.SiteUrl = strSiteUrl
                            End If

                            Return Report.fctMasterReport(dtCodes, dblPageWidth, dblPageHeight, dblDetailLeftMargin,
                                                          dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin, strDetailFontName2, sgDetailFontSize2, False, printType,
                                                          .strFontTitleName, .sgFontTitleSize, intCodePrintList:=CodePrintList, dtDetails2:=dtDetails, userLocale:=userLocale) 'VRP 05.11.2007
                        End With
                        'Return Report.fctPrintRecipeEgsStandard(dtDetail, dtKeyword, strSubStyle, blnIncludeNutrient, _
                        '            blnIncludeHACCP, blnPicturePathAccessible, blnIncludeNumber, blnIncludeCategory, blnIncludeSource, _
                        '             blnIncludeDate, blnIncludeCostOfGoods, blnIncluderemark, blnIncludeIngredientNumber, _
                        '            blnIncludePictureFirst, udtUser.CodeTrans, blnIncludeKeyword, blnIncludePictureAll, _
                        '             strDetailFontName1, sgDetailFontSize1, dblPageWidth, dblPageHeight, dblDetailLeftMargin, _
                        '             dblDetailRightMargin, dblDetailTopMargin, dblDetailBottomMargin, False)

                End Select
            Case enumReportType.MerchandiseThumbnails 'VRP 14.03.2008

        End Select
    End Function

    Private Function GetFont(ByVal str As String, ByVal intCodeTrans As String, ByVal strDefault As String) As String
        Dim arrFont(), arrFonts() As String
        If str.Trim <> "" Then
            If str.Contains("|") Then
                arrFonts = str.Split("|")
                For i As Integer = 0 To arrFonts.Length - 1
                    arrFont = arrFonts(i).Split("_")
                    If arrFont.Length > 1 AndAlso arrFont(0) = intCodeTrans Then
                        If arrFont(1).Trim.Length > 0 Then
                            Return arrFont(1)
                        Else
                            Return strDefault
                        End If
                    End If
                Next
                Return strDefault
            Else
                Return str
            End If
        Else
            Return str
        End If
    End Function



    '-- VRP 31.07.2008 FOR MENU PLAN TEST
    Public Function CreateReportPlan(ByVal ds As DataSet, ByVal udtUser As structUser, ByVal strConnection As String, _
                                         Optional ByVal strLogoPath As String = "", Optional ByVal enumMPPrintStyle As enumMPStyle = enumMPStyle.A4HWLogo, _
                                         Optional ByVal strPicPath As String = "") As XtraReport

        G_strPhotoPath = strPicPath
        G_strLogoPath = strLogoPath
        Dim Report As New xrReports(udtUser.CodeLang, strConnection)
        Report.NoPrintLines = True
        Report.strCnn = strConnection
        Report.udtUser = udtUser
        Report.SelectedWeek = SelectedWeek
        Report.MPPrintStyle = enumMPPrintStyle
        Report.CodeUserPlan = CodeUserPlan

        Dim cPLan As New clsMenuplan(enumAppType.WebApp, strConnection)
        With G_ReportOptions
            .flagNoLines = True
            .dtMPConfig = ds.Tables(0)
            .dtPlan = ds.Tables(1)
            .dtPlan2 = ds.Tables(2)
            .dblReportType = 25
            .blLandscape = False
            .bIncludeGDAImage = CBoolDB(.dtPlan.Rows(0)("PrintGDA").ToString)
            .bIncludeNutrients = CBoolDB(.dtPlan.Rows(0)("PrintNut").ToString)
        End With

        Select Case enumMPPrintStyle
            'Case enumMPStyle.A4HWLogo, enumMPStyle.A4HWOLogo
            '    G_ReportOptions.blLandscape = False
            'Case enumMPStyle.A4CWLogo, enumMPStyle.A4CWOLogo
            '    G_ReportOptions.blLandscape = True
            Case enumMPStyle.AngebotshinweisA4H, enumMPStyle.AngebotshinweisA4H_auf, enumMPStyle.KennzeichnungA4C, enumMPStyle.KennzeichnungA4C_auf
                If enumMPPrintStyle = enumMPStyle.AngebotshinweisA4H Or enumMPPrintStyle = enumMPStyle.AngebotshinweisA4H_auf Then
                    G_ReportOptions.blLandscape = False
                Else
                    G_ReportOptions.blLandscape = True
                End If


                Dim dtProposal As DataTable = ds.Tables(1)
                Dim dtNew As New DataTable

                dtNew.Columns.Add("ProposalNo")
                dtNew.Columns.Add("ProposalName")
                dtNew.Columns.Add("CodeTrans")
                dtNew.Columns.Add("Price")
                dtNew.Columns.Add("PrintNut")
                dtNew.Columns.Add("PrintGDA")

                Dim row As DataRow
                Dim strProposals() As String
                Dim strProposal() As String
                For Each rowPro As DataRow In dtProposal.Rows
                    strProposals = CStrDB(rowPro("ProposalName")).Split(CChar("�"))
                    For i As Integer = 0 To strProposals.GetUpperBound(0) - 1
                        strProposal = strProposals(i).Split(CChar("�"))
                        row = dtNew.NewRow
                        row("ProposalNo") = strProposal(0)
                        row("ProposalName") = strProposal(1)
                        row("CodeTrans") = rowPro("CodeTrans")
                        row("Price") = rowPro("Price")
                        row("PrintNut") = rowPro("PrintNut")
                        row("PrintGDA") = rowPro("PrintGDA")
                        dtNew.Rows.Add(row)
                    Next
                Next
                G_ReportOptions.dtPlan = ds.Tables(0)
                G_ReportOptions.dtMPConfig = dtNew
                G_ReportOptions.dtMPIngr = ds.Tables(2)
            Case enumMPStyle.EinlageblatterA5H, enumMPStyle.EinlageblatterA5H_auf
                G_ReportOptions.blLandscape = True

                Dim dtProposal As DataTable = ds.Tables(1)
                Dim dtNew2 As New DataTable

                dtNew2.Columns.Add("ProposalNo")
                dtNew2.Columns.Add("ProposalName")
                dtNew2.Columns.Add("CodeTrans")
                dtNew2.Columns.Add("Price")
                dtNew2.Columns.Add("PrintNut")
                dtNew2.Columns.Add("PrintGDA")
                Dim row2 As DataRow
                Dim strProposals() As String
                Dim strProposal() As String
                For Each rowPro As DataRow In dtProposal.Rows
                    strProposals = CStrDB(rowPro("ProposalName")).Split(CChar("�"))
                    For i As Integer = 0 To strProposals.GetUpperBound(0) - 1
                        strProposal = strProposals(i).Split(CChar("�"))
                        row2 = dtNew2.NewRow
                        row2("ProposalNo") = strProposal(0)
                        row2("ProposalName") = strProposal(1)
                        row2("CodeTrans") = rowPro("CodeTrans")
                        row2("Price") = rowPro("Price")
                        row2("PrintNut") = rowPro("PrintNut")
                        row2("PrintGDA") = rowPro("PrintGDA")
                        dtNew2.Rows.Add(row2)
                    Next
                Next


                Dim dtNew As New DataTable
                dtNew.Columns.Add("Code")
                dtNew.Columns.Add("CodeTrans")
                dtNew.Columns.Add("CodeDay")
                dtNew.Columns.Add("N1")
                dtNew.Columns.Add("N2")
                dtNew.Columns.Add("N3")
                dtNew.Columns.Add("N4")
                dtNew.Columns.Add("N5")
                dtNew.Columns.Add("N6")
                dtNew.Columns.Add("N7")
                dtNew.Columns.Add("N8")
                dtNew.Columns.Add("N9")
                dtNew.Columns.Add("N10")
                dtNew.Columns.Add("N11")
                dtNew.Columns.Add("N12")
                dtNew.Columns.Add("N13")
                dtNew.Columns.Add("N14")
                dtNew.Columns.Add("N15")

                Dim rowNew As DataRow
                Dim iNew As Integer = 0


                Dim dtTrans As DataTable = ds.Tables(3)
                For Each rowTrans As DataRow In dtTrans.Rows
                    Dim foundRowNew() As DataRow = ds.Tables(0).Select("CodeTrans=" & rowTrans("CodeTrans"))

                    For iNew = 0 To foundRowNew.GetUpperBound(0)
                        If foundRowNew(iNew)("ProposalNo") Mod 2 Then
                            rowNew = dtNew.NewRow
                            rowNew("Code") = foundRowNew(iNew)("ProposalNo")
                            rowNew("CodeTrans") = foundRowNew(iNew)("CodeTrans")
                            rowNew("CodeDay") = foundRowNew(iNew)("CodeDay")
                            rowNew("N1") = foundRowNew(iNew)("N1")
                            rowNew("N2") = foundRowNew(iNew)("N2")
                            rowNew("N3") = foundRowNew(iNew)("N3")
                            rowNew("N4") = foundRowNew(iNew)("N4")
                            rowNew("N5") = foundRowNew(iNew)("N5")
                            rowNew("N6") = foundRowNew(iNew)("N6")
                            rowNew("N7") = foundRowNew(iNew)("N7")
                            rowNew("N8") = foundRowNew(iNew)("N8")
                            rowNew("N9") = foundRowNew(iNew)("N9")
                            rowNew("N10") = foundRowNew(iNew)("N10")
                            rowNew("N11") = foundRowNew(iNew)("N11")
                            rowNew("N12") = foundRowNew(iNew)("N12")
                            rowNew("N13") = foundRowNew(iNew)("N13")
                            rowNew("N14") = foundRowNew(iNew)("N14")
                            rowNew("N15") = foundRowNew(iNew)("N15")
                            dtNew.Rows.Add(rowNew)
                        End If
                    Next
                Next

                'Dim dtTrans As DataTable = ds.Tables(3)
                'For Each rowTrans As DataRow In dtTrans.Rows
                '    iNew = 0
                '    For Each row As DataRow In ds.Tables(0).Rows

                '        If row("CodeTrans") = rowTrans("CodeTrans") Then
                '            rowNew = dtNew.NewRow
                '            If row("ProposalNo") Mod 2 Then
                '                rowNew("Code") = row("ProposalNo")
                '                rowNew("CodeTrans") = row("CodeTrans")
                '                rowNew("CodeDay") = row("CodeDay")
                '                dtNew.Rows.Add(rowNew)
                '            End If
                '            iNew += 1
                '        End If
                '    Next
                'Next


                G_ReportOptions.dtDetail = dtNew
                G_ReportOptions.dtPlan = ds.Tables(0)
                G_ReportOptions.dtMPConfig = dtNew2
                G_ReportOptions.dtMPIngr = ds.Tables(2)
                Return Report.fctMasterReportPlan(dtNew, strConnection, SelectedWeek)
            Case enumMPStyle.EinlageblatterA6H, enumMPStyle.EinlageblatterA6H_auf
                G_ReportOptions.blLandscape = False

                Dim dtProposal As DataTable = ds.Tables(1)
                Dim dtNew2 As New DataTable

                dtNew2.Columns.Add("ProposalNo")
                dtNew2.Columns.Add("ProposalName")
                dtNew2.Columns.Add("CodeTrans")
                dtNew2.Columns.Add("Price")
                dtNew2.Columns.Add("PrintNut")
                dtNew2.Columns.Add("PrintGDA")

                Dim row2 As DataRow
                Dim strProposals() As String
                Dim strProposal() As String
                For Each rowPro As DataRow In dtProposal.Rows
                    strProposals = CStrDB(rowPro("ProposalName")).Split(CChar("�"))
                    For i As Integer = 0 To strProposals.GetUpperBound(0) - 1
                        strProposal = strProposals(i).Split(CChar("�"))
                        row2 = dtNew2.NewRow
                        row2("ProposalNo") = strProposal(0)
                        row2("ProposalName") = strProposal(1)
                        row2("CodeTrans") = rowPro("CodeTrans")
                        row2("Price") = rowPro("Price")
                        row2("PrintNut") = rowPro("PrintNut")
                        row2("PrintGDA") = rowPro("PrintGDA")
                        dtNew2.Rows.Add(row2)
                    Next
                Next

                '---
                Dim dtNew As New DataTable
                dtNew.Columns.Add("Code")
                dtNew.Columns.Add("CodeTrans")
                dtNew.Columns.Add("CodeDay")
                dtNew.Columns.Add("N1")
                dtNew.Columns.Add("N2")
                dtNew.Columns.Add("N3")
                dtNew.Columns.Add("N4")
                dtNew.Columns.Add("N5")
                dtNew.Columns.Add("N6")
                dtNew.Columns.Add("N7")
                dtNew.Columns.Add("N8")
                dtNew.Columns.Add("N9")
                dtNew.Columns.Add("N10")
                dtNew.Columns.Add("N11")
                dtNew.Columns.Add("N12")
                dtNew.Columns.Add("N13")
                dtNew.Columns.Add("N14")
                dtNew.Columns.Add("N15")

                Dim rowNew As DataRow

                Dim dtTrans As DataTable = ds.Tables(3)
                Dim dtMP As DataTable = ds.Tables(0)

                Dim iCodeDay As Integer = 0
                Dim foundRowPlan() As DataRow

                For iCodeDay = 0 To 6
                    For Each rowTrans As DataRow In dtTrans.Rows
                        foundRowPlan = dtMP.Select("CodeDay=" & iCodeDay + 1 & " AND CodeTrans=" & rowTrans("CodeTrans"))
                        If Not foundRowPlan.Length = 0 Then
                            If foundRowPlan(0)("CodeTrans") = rowTrans("CodeTrans") Then
                                rowNew = dtNew.NewRow
                                rowNew("Code") = foundRowPlan(0)("ProposalNo")
                                rowNew("CodeTrans") = foundRowPlan(0)("CodeTrans")
                                rowNew("CodeDay") = iCodeDay + 1 'foundRowPlan(iPlan)("CodeDay")
                                rowNew("N1") = foundRowPlan(0)("N1")
                                rowNew("N2") = foundRowPlan(0)("N2")
                                rowNew("N3") = foundRowPlan(0)("N3")
                                rowNew("N4") = foundRowPlan(0)("N4")
                                rowNew("N5") = foundRowPlan(0)("N5")
                                rowNew("N6") = foundRowPlan(0)("N6")
                                rowNew("N7") = foundRowPlan(0)("N7")
                                rowNew("N8") = foundRowPlan(0)("N8")
                                rowNew("N9") = foundRowPlan(0)("N9")
                                rowNew("N10") = foundRowPlan(0)("N10")
                                rowNew("N11") = foundRowPlan(0)("N11")
                                rowNew("N12") = foundRowPlan(0)("N12")
                                rowNew("N13") = foundRowPlan(0)("N13")
                                rowNew("N14") = foundRowPlan(0)("N14")
                                rowNew("N15") = foundRowPlan(0)("N15")
                                dtNew.Rows.Add(rowNew)
                            End If

                            If foundRowPlan.Length > 4 Then
                                rowNew = dtNew.NewRow
                                rowNew("Code") = 5 'foundRowPlan(0)("ProposalNo")
                                rowNew("CodeTrans") = foundRowPlan(0)("CodeTrans")
                                rowNew("CodeDay") = iCodeDay + 1 'foundRowPlan(iPlan)("CodeDay")
                                dtNew.Rows.Add(rowNew)
                            End If
                        End If
                    Next
                Next

                Dim dv As New DataView(dtNew)
                dv.Sort = "CodeDay ASC"
                dtNew = dv.ToTable

                G_ReportOptions.dtDetail = dtNew
                G_ReportOptions.dtPlan = ds.Tables(0)
                G_ReportOptions.dtMPConfig = dtNew2
                G_ReportOptions.dtMPIngr = ds.Tables(2)
                Return Report.fctMasterReportPlan(dtNew, strConnection, SelectedWeek)
        End Select

        Return Report.fctMasterReportPlan(ds.Tables(0), strConnection, SelectedWeek)
    End Function

End Class

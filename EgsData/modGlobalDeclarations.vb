Imports System
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient

Public Module modGlobalDeclarations

#Region "Global Variables"
    Public G_strTempPath As String
    Public G_intDateFormatLocal As Int32
    Public G_strDateSeparatorLocal As String
    Public G_strThouSeparatorLocal As String
    Public G_strDecimalSeparatorLocal As String
    Public G_intTimeFormatLocal As Int32
    Public G_strTimeSeparatorLocal As String
    Public G_strListSeparatorLocal As String

    Public G_ImportConfig As structPOSAutoConfig
    Public G_POSInfo As structPOSInfo

    Public G_MenuType As MenuType

    Public g_ReportDetail As structReportDetail

    Public G_ConnectionDR As String
    Public G_structApplicationType As sApplicationType
    Public G_structCurrentList As sCurrentList

    ' RDC 03.15.2013 - CWM-3518 Fix
    'Public G_DefPageLanguage As Integer = 0
    ' RDC 04.08.2013 - CWM-4729 Fix
    Public G_DefMainLanguage As Integer = 0

    ' RDC 03.18.2013 - CWM-4138
    Public intSelectedReport As enumFileType

    ' RDC 07.09.2013 - Additional variable for Nutrient Codeset
    Public G_SelectedNutrientCodeSet As Integer = 0

    ' RDC 11.13.2013 : Global export options
    Public G_ExportOptions As WordExportOptions

    ' RDC 11.14.2013 : One Quantity Flag
    Public bitUseOneQuantity As Integer = 0

#End Region

#Region "UDTs"

    Public Structure structNutrientInfo
        Public Name As String
        Public Value As String
        Public Percent As String
        Public Visible As Boolean
        Public Position As Integer  '-- JBB 07.02.2012
    End Structure

    Public Structure structReportDetail
        Public strPhotoPath As String
        Public strLogoPath As String
        Public intCodeLang As Integer
        Public intCodeTrans As Integer
        Public intCodeSetPrice As Integer
        Public strSetPriceSymbole As String
    End Structure

    'MRC 03.05.09 - Kiosk
    Public Structure RecipeParameter
        Public CodeRecipe As Integer
        Public CodeTrans As Integer
        Public ConvertToBestUnit As Boolean
        Public CodeSetPrice As Integer
        Public YieldQty As Double
        Public CodeSite As Integer
        Public DisplayInMetric As Boolean
        Public ConvertPrice As Boolean
    End Structure

    'MRC 03.05.09 - Kiosk
    Public Structure SearchAndDisplayConfiguration
        'search filters
        Public DisplaySearchFilterName As Boolean
        Public DisplaySearchFilterNumber As Boolean
        Public DisplaySearchOptionLanguage As Boolean
        Public DisplaySearchFilterCategory As Boolean
        Public DisplaySearchFilterKeyword As Boolean
        Public DisplaySearchFilterSource As Boolean
        Public DisplaySearchFilterIngredientsWanted As Boolean
        Public DisplaySearchFilterIngredientsUnwanted As Boolean
        Public DisplaySearchOptionLocation As Boolean
        Public DisplaySearchOptionLanguageInterface As Boolean

        Public DisplayViewType As Boolean       'Display selection for thumbnail or list
        Public DisplayViewRecordsPerPage As Boolean
        Public DisplaySearchRecipeLabel As Boolean

        'search result
        Public ThumbnailColumnsCount As Integer 'number of columns in thumbnail view; either 2 or 3
        Public ThumbnailRecordCount As Integer  'number of records per page for the thumbnail view
        Public ListRecordCount As Integer       'number of records per page for the list view
        Public ThumbnailView As Integer

        Public DisplayColumnMarkCheckbox As Boolean
        Public DisplayColumnNumber As Boolean
        Public DisplayColumnNumberAndNameTogether As Boolean
        Public DisplayColumnDescription As Boolean
        Public DisplayColumnRemark As Boolean
        Public DisplayRecipeResultNumber As Boolean
    End Structure


    ' MRC Dec. 10 2007 - For RecipeEncoding, mass validation
    Structure structRecipeEncode
        Dim RowID As Integer
        Dim Code As String
        Dim Quantity As String
        Dim Unit As String
        Dim Name As String
        Dim Complement As String
        Dim Preparation As String
        Dim isValidated As Boolean
        Dim SecondCode As Integer
        Dim ItemID As Integer
        Dim Position As Integer
        Dim ItemType As Integer
    End Structure

    'JLC 26.04.2006
    Public Structure structSearchWineFilter
        Public Country As Integer
        Public Region As Integer
        Public SubRegion As Integer
        Public Producer As Integer
        Public WineType As Integer
        Public Alcohol As String
        Public Vintage As String
        Public Peak As String
        Public HoldUntil As String
        Public DrinkBy As String
        Public TasteNext As String
        Public Rating As String
        Public Comment As String
    End Structure

    'JLC 26.04.2006
    Public Structure structSearchSalesFilter
        Public Code As Integer
        Public CodeSite As Integer
        Public Name As String
        Public Number As Integer
        Public Tax As Integer
        Public SetPriceSale As Integer
        Public PriceFrom As Double
        Public PriceTo As Double
        Public IncludeProduct As Boolean
        Public IncludeRecipe As Boolean
        Public IncludeMenu As Boolean
        Public IncludeNoTypes As Boolean
        Public Linked As Integer ' 0 =all, 1= linked, 2= notlinked
        Public IsExactMatch As Boolean
        Public intPageIndex As Integer
        Public intPageSize As Integer
        Public strCodeList As String
    End Structure
    Public Structure structSearchSalesHistoryFilter
        Public ID As Integer
        Public MarkedSalesList As enumFbFilter '0:All, 1:marked
        Public SiteList As String 'list of sites
        Public MarkedTerminal As enumFbFilter '0:All, 1:marked
        Public CodeSetPrice As Integer
        Public MarkedIssuanceType As enumFbFilter '0:All, 1:marked
        Public IncludeProduct As Boolean
        Public IncludeRecipe As Boolean
        Public IncludeMenu As Boolean
        Public OutputDone As Integer '1=true, 0 =false, 2=all
        Public ItemName As String
        Public DateSalesFrom As DateTime
        Public DateSalesTo As DateTime
        Public CodeCategory As Int16
        Public ItemNumber As String
        Public MerchandiseKeyword As String
        Public RecipeKeyword As String
        Public Cost As Integer '0= LastPrice; 1=AvgPrice
        Public IsExactMatch As Boolean
        Public intPageIndex As Integer
        Public intPageSize As Integer

        'MRC - 06.11.09
        Public CodeSales As Integer
        Public CodeSite As Integer
        Public CodeIssuance As Integer
        Public CodeTerminal As Integer
        Public Type As Integer

    End Structure

    'Structure structKey
    '    'Key As String
    '    'KeyVersion As Integer

    '    'Header As String
    '    'Serial As String
    '    'CheckSum As Integer
    '    'ComputerName As String
    '    'ComputerNameId As Integer
    '    'Licence As Integer
    '    'Module_Pocket As Boolean
    '    'Module_Link2POS As Boolean
    '    'Module_Link2RcNetCm2004 As Boolean
    '    'Module_Link2Accounting As Boolean
    '    'Module_Inventory As Boolean
    '    'Module_Purchasing As Boolean
    '    'Module_Sales As Boolean
    '    'Module_Production As Boolean
    '    'Module_FBManager As Boolean
    '    'Module_InternalTransfer As Boolean
    '    Public Module_Approval As Boolean
    '    'AllowCitrix As Boolean
    '    'AllowTerminalService As Boolean
    '    'VersionLight As Boolean
    '    Dim LangueE As Boolean
    '    Dim LangueG As Boolean
    '    Dim LangueF As Boolean
    '    Dim LangueI As Boolean
    '    Dim LanguePb As Boolean
    '    Dim LangueS As Boolean
    '    Dim LangueJ As Boolean
    '    Dim LangueTk As Boolean
    '    Dim LangueGk As Boolean
    '    Dim LanguePp As Boolean
    '    Public ProgramType As EGSPrograms
    '    'ExpirationDate As Date
    '    'AllowsMoreThanOneDatabase As Boolean
    '    'ActivationDaysLeft As Integer '-1=Activated already
    '    'ComputerKey As String
    '    'VersionDatabase As Integer
    '    'Module_SupplierNetwork As Boolean
    '    'Module_ImportProducts As Boolean
    '    'SupportExpiration As Date        'vbv 26.05.2005
    '    'SupportUnlimited As Boolean      'vbv 26.05.2005
    'End Structure

    Structure structOptions

        Dim Language As Integer
        Dim LanguageHelp As Integer

        Dim DefaultCurrency As Integer
        Dim AutoLogin As Boolean
        Dim PlaySounds As Boolean
        Dim SupplierNumberUnique As Boolean
        Dim ShowLog As Boolean
        Dim ShowHiddenMenu As Boolean
        Dim ShowAnimation As Boolean

        Dim RemoveTrailingZero As Boolean
        Dim ConvertToBestUnit As Boolean
        Dim UnitType As Integer
        Dim FactorType As Integer
        Dim QtyType As Integer

        Dim RecipeAndMenuCalculationPerYield As Boolean
        Dim RecalculatePrices As Boolean
        Dim StrictEncoding As Boolean
        Dim AutoUpdateRnPrice As Boolean
        Dim AvgPriceOption As Integer

        Dim NutrientDB As Integer
        Dim HelpFile As String

        Dim HelpMDIMode As Byte
        Dim HelpShowMode As Byte
        Dim DatabaseAlias As String
        Dim dateAvgPriceFrom As Date
        Dim dateAvgPriceTo As Date
        Dim dblPriceWidthFactor As Double
        Dim AccountingSystem As Integer

        Dim POSAutoImport As Boolean
        Dim POSArchivePath As Boolean

        Dim email As String
        Dim Smtp As String
        Dim SmtpAuthUsername As String
        Dim SmtpAuthPassword As String
        Dim ScannerDevice As Integer 'None/Pocket Pc/Scan Pal 2
        Dim ScanPalPort As Integer
        Dim ScanPalBaud As Integer
        Dim Pages_NumberOfPageMax As Integer
        Dim Pages_NumberOfRowsMin As Integer
        Dim Pages_NumberOfRowsMax As Integer
        Dim Pages_UsePage As Boolean
        Dim Pages_NoMaximum As Boolean

        Dim ColorBackColor As Integer
        Dim ColorBackColorAlternate As Integer
        Dim ColorBackColorSel As Integer
        Dim ColorForeColor As Integer
        Dim ColorGridColor As Integer
        Dim ColorBackColorBkg As Integer
        Dim ColorEditableColumn As Integer
        Dim ColorHighlightText As Integer
        Dim ColorBackPickList As Integer
        Dim ColorBackInfoList As Integer
        Dim GridLines As Integer
        Dim ShowWallPaper As Boolean
        Dim GridPicture As String


        Dim ShowStructure As Boolean
        Dim ShowGrid As Boolean
        Dim ShowSymbols As Boolean
        Dim ShowIngrPreparation As Boolean
        Dim ExpandSubRecipe As Boolean

        Dim ShowItemNo As Boolean

        Dim DefaultItemLanguageCode As Integer
        Dim DefaultMerchandiseTaxCode As Integer
        Dim DefaultRecipeTaxCode As Integer
        Dim DefaultMenuTaxCode As Integer
        Dim DefaultRecipeYieldQty As Double
        Dim DefaultRecipeYieldUnitCode As Integer
        Dim DefaultMenuYieldQty As Double
        Dim DefaultRecipePercent As Integer
        Dim DefaultMenuPercent As Integer
        Dim DefaultRecipeCoeff As Double
        Dim DefaultMenuCoeff As Double

        Dim DictionaryEnglish As Integer
        Dim DictionaryGerman As Integer
        Dim DictionaryFrench As Integer
        Dim DictionaryItalian As Integer
        Dim DictionaryPortuguese As Integer
        Dim DictionarySpanish As Integer
        Dim DictionaryJapanese As Integer
        Dim DictionaryTurkish As Integer
        Dim DictionaryGreek As Integer
        Dim DictionaryThai As Integer
        Dim DictionaryHungarian As Integer
        Dim DictionaryPortuguesePortugal As Integer


        Dim blnEnergyKJ As Boolean

        Dim ImportType As Integer
        Dim ImportShowItem As Boolean
        Dim ImportShowOption As Boolean
        Dim ImportCompareBy As Integer
        Dim ImportAddNew As Boolean
        Dim ImportUpdate As Boolean
        Dim ImportUpdateType As Integer
        Dim ImportDelPriceUnit As Boolean
        Dim ImportSuppNetwork As Integer
        Dim ImportAutoLinkItems As Boolean
        Dim ImportNewSupplier As Boolean

    End Structure

    Structure structCurrency
        Dim Code As Integer
        Dim Symbole As String
        Dim Description As String
        Dim Format As String
        Dim Active As Boolean
        Dim Sign As String 'AGL 2012.10.24 CWM-1804
    End Structure

    Structure structTax
        Dim Code As Long
        Dim Value As Double
        Dim Description As String
        Dim NumberRef As String
        Dim IsGlobal As Boolean
    End Structure

    Structure structYieldUnit
        Dim Code As Long
        Dim Name As String
        Dim Active As Boolean
        Dim UserDefined As Integer
        Dim Serving As Boolean
        Dim NameDef As String
        Dim AutoConversion As String
        Dim Format As String
    End Structure

    Public Structure structListe
        Public Code As Integer
        Public CodeSite As Integer
        Public CodeUser As Integer
        Public Type As Integer
        Public Name As String
        Public Subtitle As String
        Public Subheading As String 'DRR 02.22.2011
        Public Template As Integer
        Public Number As String
        Public IsSiteAutonumber As Boolean 'JTOC 11.28.2013
        Public Brand As Integer
        Public Category As Integer
        Public Source As Integer
        Public Supplier As Integer
        Public Yield As Double
        Public YieldUnit As Integer
        Public Dates As Date
        Public Percent As Integer
        Public srQty As Double
        Public srWeight As Double
        Public srUnit As Integer
        Public PictureName As String
        Public Note As String
        Public Remark As String
        Public Wastage1 As Integer
        Public Wastage2 As Integer
        Public Wastage3 As Integer
        Public Wastage4 As Integer
        Public CoolingTime As String
        Public HeatingTime As String
        Public HeatingTemperature As String
        Public HeatingMode As String
        Public CCPDescription As String
        Public Description As String
        Public Ingredients As String
        Public Preparation As String
        Public CookingTip As String
        Public Refinement As String
        Public Storage As String
        Public Productivity As String
        Public [Protected] As Boolean
        Public CodeLink As Integer
        Public IsGlobal As Boolean
        Public CodeTrans As Integer
        Public AllowUse As Boolean

        ' for menu card
        Public MenuCardDateFrom As Date
        Public MenuCardDateUntil As Date
        Public MenuCardCodeSetPrice As Integer

        'for RX
        Public EgsRef As Integer
        Public EgsID As Integer

        'for clsRnXML
        Public BrandName As String
        Public CategoryName As String
        Public SourceName As String
        Public SupplierName As String
        Public YieldUnitName As String
        Public srUnitName As String

        ' for clsRnXML (for versions <5)
        Public Coeff As Double
        Public keyfield As String       'RDTC 09.08.2007; specific to Le Patron
        Public NetWeight As Double      'RDTC 06.03.2008

        Public TemplateCode As Integer 'VRP 04.04.2008
        Public NoteHeader As String  'VRP 24.06.2008
        Public CodeStyle As String 'VRP 01.07.2008

        Public StoringTemp As String 'DLS
        Public StoringTime As String 'DLS

        Public Online As Boolean 'MRC-08.27.08
        Public ProtectedCodeUser As Integer 'MRC-09.04.08
        Public DateProtected As Date 'MRC-09.04.08
        Public ProtectedNote As String 'MRC-09.02.08
        Public ProtectedComment As String 'MRC-09.02.08

        Public PriceSmallPortion As Double 'VRP 23.02.2009
        Public PriceLargePortion As Double 'VRP 23.02.2009

        Public DefaultPicture As Integer 'MRC 08.03.09

        'MRC 12.16.2010 - For Unilever USA

        'Yield 2
        Public Yield2 As Double
        Public YieldUnit2 As Integer
        Public Percent2 As Integer

        'Portion Size
        Public PortionSize As Double
        Public PortionSizeUnit As Integer
        Public Percent3 As Integer

        Public MethodFormat As String ' JBB 01.24.2011 Method Format
        Public FootNote1 As String ' JBB 03.24.2011
        Public FootNote2 As String ' JBB 03.24.2011
        Public ServeWith As String  ' JBB 04.04.2012

        '--JBB 07.05.2011
        Public Standard As Integer
        Public Difficulty As Integer
        Public Budget As Integer
        Public QuicknEasy As Integer
        Public ShowOff As Boolean
        Public ChefRecommended As Boolean
        '--

        '-- JBB 09.28.2011
        Public UPC As String '-- for Ingredients (USA)
        '--
        '-- JBB 01.27.2012
        Public CostperServing As Single
        Public CostperRecipe As Single
        '--


        '-- JBB 04.02.2012
        Public LegacyNumber As String
        '--

        'JTOC 21.11.2012
        Public Packaging As Integer
        Public Certification As Integer
        Public Origin As Integer
        Public Temperature As Integer
        Public Information As Integer

    End Structure


    Public Structure structStepTranslation
        Public CodeListe As Integer
        Public StepNum As Integer
        Public Name As String
        Public Procedure As String
        Public CodeTrans As Integer
    End Structure

    Public Structure structListeTranslation
        Public CodeListe As Integer
        Public CodeTrans As Integer
        Public Name As String
        Public Note As String
        Public Remark As String
        Public CCPDescription As String
        Public Ingredients As String
        Public Preparation As String
        Public CookingTip As String
        Public Refinement As String
        Public Storage As String
        Public Productivity As String
        Public Description As String
        Public NoteHeader As String 'VRP 26.06.2008

        Public StoringTemp As String 'DLS
        Public StoringTime As String 'DLS

        Public Subtitle As String 'ADR 03.25.11
        Public Subheading As String 'ADR 03.25.11

        Public FootNote1 As String 'ADR 04.04.11
        Public FootNote2 As String 'ADR 04.04.11

        Public ServeWith As String ' JBB 05.08.2012
    End Structure
    Public Structure structPrintProfile
        Public Code As Int32
        Public Name As String
        Public Type As enumReportType
        Public IsGlobal As Boolean
        Public Position As Int16
    End Structure
    Public Structure structBrand
        Public Code As Int32
        Public Name As String
        Public Type As enumDataListItemType
        Public IsGlobal As Boolean
        Public Position As Int16
        Public Parent As Integer 'JBB 12.28.2010
        Public IsCanBeParent As Boolean '' JBB 05.24.2012
    End Structure

    Public Structure structAllergen
        Public Code As Int32
        Public Abbreviation As String
    End Structure


    Public Structure structCategory
        Public Code As Int32
        Public CodeGroup As Int32
        Public Name As String
        Public Type As enumDataListItemType
        Public CodeAcct As String
        Public IsGlobal As Boolean
        Public Position As Int32
        Public IsProduct As Boolean 'MRC
    End Structure

    Public Structure structRole 'AGL 2013.07.02
        Public Code As Int32
        Public Name As String
        Public RoleLevel As Integer
    End Structure

    Public Structure structKeyword
        Public Code As Int32
        Public Name As String
        Public Parent As Int32
        Public Type As enumDataListItemType
        Public Inheritable As Boolean
        Public IsGlobal As Boolean
        Public Picture As String
    End Structure

    Public Structure structLabor
        Public Code As Int32
        Public Name As String
        Public HourlyRate As Double
        Public Currency As Int32 'structCurrency
        Public IsGlobal As Boolean
    End Structure

    Public Structure structNutrientRules
        Public Code As Int32
        Public Description As String
        Public Minimum As Double
        Public Maximum As Double
        Public Nutr_No As String
        Public IsComplex As Boolean
        Public IsGlobal As Boolean
        Public CodeSet As Integer
    End Structure

    Public Structure structSetPrice
        Public Code As Int32
        Public Name As String
        Public CodeCurrency As Int32
        Public IsGlobal As Boolean
        Public Type As SetPriceType
        Public CodePurchasing As Integer
        Public SPFactor As Double
        Public FactorToMain As Double
    End Structure

    Public Structure structSource
        Public Code As Int32
        Public Name As String
        Public EGSID As Int32
        Public IsGlobal As Boolean
    End Structure

    Public Structure structSupplier
        Public Code As Int32
        Public NameRef As String
        Public Company As String
        Public URL As String
        Public Number As String
        Public Terms As String
        Public Remark As String
        Public Note As String
        Public UseDefaultTerms As Boolean
        Public WithTax As Boolean
        Public AccountingRef As String
        Public Tel As String
        Public Fax As String
        Public Email As String
        Public Address1 As String
        Public Address2 As String
        Public City_1 As String
        Public Zip_1 As String
        Public Country_1 As String
        Public State_1 As String
        Public City_2 As String
        Public Zip_2 As String
        Public Country_2 As String
        Public State_2 As String


        Public AddFlag As Boolean
        Public UpdateFlag As Boolean
        Public ImportFlag As Boolean
        Public IsGlobal As Boolean

        Public CodeSupplierContact As Int32
        Public Title As String
        Public FName As String
        Public LName As String
        Public JobPosition As String
        Public ContactTel As String
        Public ContactFax As String
        Public ContactEmail As String
        Public ContactMobile As String

        Public CodeSupplierGroup As Int32
        Public GroupName As String
        Public GroupNote As String
        Public GroupIsGlobal As String
    End Structure

    Public Structure structUnit
        Public Code As Int32
        Public NameDef As String
        Public NameDisplay As String
        Public NamePlural As String
        Public AutoConversion As String
        Public IsBasic As Boolean
        Public IsStock As Boolean
        Public IsPackaging As Boolean
        Public IsTransportation As Boolean
        Public IsIngredient As Boolean
        Public IsYield As Boolean
        Public IsServing As Boolean
        Public Factor As Double
        Public TypeMain As Int16
        Public IsMetric As Int16 'JTOC 16.11.2012 from bit to int
        Public Format As String
        Public IsAdded As Boolean
        Public IsGlobal As Boolean
        Public Position As Int32
        Public useMakes As Boolean ' RDC 07.30.2013
    End Structure

    Public Structure structSite
        Public Code As Int32
        Public Name As String
        Public [Group] As Int32
        Public SiteLevel As enumGroupLevel
    End Structure

    Public Structure structUser
        Public Code As Int32
        Public Status As Byte
        Public Username As String
        '     Public Password As String
        Public DateModified As Date
        Public DateCreated As Date
        'DateLastAccess	FullName	Email	SMTPUID	SMTPPWD	Approver	Notify	ApproverOnly	
        'Name	Company	Address	City	Zip	State	CountryCode	CodeSourceGallery	EGSID	
        Public Site As structSite
        Public RoleLevelHighest As enumGroupLevel

        'JRL 29.9.2005
        Public CodeLang As Integer
        Public CodeTrans As Integer
        Public LastSetPrice As Integer
        Public CodeTimeZone As Integer
        Public PageSize As Integer
        Public NutrientDBCode As Integer
        Public Fullname As String
        Public SiteThemeFolder As String
        Public SiteName As String
        Public SiteLogoURL As String
        Public Email As String

        Public UseBestUnit As Boolean
        '        Public eRoleLevel As enumGroupLevel
        Public eDisplayMode As enumListeDisplayMode

        Public arrRoles As ArrayList
        'Public arrRoleRights As ArrayList
        Public arrSitesAccessible As ArrayList
        Public arrListeTypeApprovalRequired As ArrayList
        Public arrRolesNames As ArrayList
        Public DateLastAccessed As Date

        ' 21.10.2005
        Public WebHomePageBrowseListTableForMerchandise As String
        Public WebHomePageBrowseListTableForRecipe As String
        Public WebHomePageBrowseListTableForMenu As String

        'JLC 26.04.2006
        Public LastSetPriceSales As Integer
        Public EGSID As Integer
        Public FullText As Boolean
        Public ListItemColor As String
        Public ListAlternatingItemColor As String
        Public RemoveTrailingZeroes As Boolean
        Public PrintOutput As String '  _ delimited
        Public UnsavedItemColor As String
        Public UserSession As String

        Public CodeCaptions As Integer 'RDTC 26.09.2007
        Public RoleRights As enumUserRights '--JBB 12.21.2011

        Public PreviewAllowMultipleWindows As Boolean 'AGL 2012.11.16
        Public DisplayQuantitiesAsFractions As Boolean 'AGL 2013.04.18
        Public AutoConversion As Boolean 'JTOC 15.07.2013

        Public ClientSerial As String 'AGL 2014.09.09

        Public LoginStatusCode As Integer 'AGL 2014.09.12

        Public AllowAccessToOtherSite As Boolean 'MKAM 2014.11.06
        Public CulturePref As String
    End Structure

    Public Structure structMail
        Public RecipeName As String
        Public SenderName As String
        Public SenderEmail As String
        Public SenderCodeTrans As Integer
        Public RecipentName As String
        Public RecipientEmail As String
        Public Subject As String
        Public Body As String
        Public SMTPPort As String
        Public SMTPServer As String
    End Structure

    'jhl 20.12.05
    Public Structure structTerminal
        Public Code As Int32
        Public CodeSite As Int32
        Public Name As String
        Public Number As String
        Public DateFormat As Int32
        Public DateSeparator As String
        Public ThouSeparator As String
        Public DecimalSeparator As String
        Public TimeFormat As Int32
        Public TimeSeparator As String
        Public ListSeparator As String
        Public CodePOS As Int32
        Public Note As String
    End Structure

    'jhl 04.01.06
    Public Structure structPOSAutoConfig
        Public ID As Int32
        Public Active As Boolean
        Public Path1 As String
        Public Path2 As String
        Public Prefix1 As String
        Public Prefix2 As String
        Public CodePOStype As Int32
        Public CodeSite As Int32
        Public CodeTerminal As Int32
        Public CodeSellingSetPrice As Int32
        Public CodePurchaseSetPrice As Int32
        Public SPFactor As Double
        Public ArchivePath As String
        Public OpeningTime As DateTime
        Public DateSales As DateTime
        Public CodeTax As Int32
        Public Note As String
        Public MainSched As Int32
        Public StartDate As DateTime
        Public StartTime As DateTime
        Public EveryNth As Int32
        Public Mon As Boolean
        Public Tue As Boolean
        Public Wed As Boolean
        Public Thu As Boolean
        Public Fri As Boolean
        Public Sat As Boolean
        Public Sun As Boolean
        Public TheNth As Int32
        Public LastImport As DateTime
    End Structure
    Public Structure structPOSInfo
        Public Name As String
        Public File1 As String
        Public File2 As String
        Public LastID As Integer
        Public Active As Boolean
        Public FileCount As Integer
        Public FileExtension As String
        Public DeleteAfterImport As Boolean
    End Structure

    Public Structure structImportCSVConfig
        Public FilePath As String
        Public FileName As String
        Public LogFile As String
        Public RemoveFile As Boolean
        Public ArchivePath As String
        Public AddNewRec As Boolean
        Public UpdateExistingRec As Boolean
        Public UpdBasedDateMod As Boolean
        Public CompareByName As Boolean
        Public DeleteUnused As Boolean
        Public FieldSepar As String
        Public ThousandSepar As String
        Public DecimalSepar As String
        Public CodeSite As Int32
        Public CodeSetPrice As Int32
        Public bitUse As Boolean
        Public CodeTrans As Int32
        Public CodeUser As Int32
        Public IsGlobal As Boolean
        Public ImportFileType As Boolean
        Public ImportFileFolder As Boolean '0:File, 1:Folder
        Public ImportSched As Boolean
        Public ImportTime As DateTime
    End Structure

    Public Structure structPOSTempData
        Public ID As Integer
        Public POSNumber As String
        Public POSName As String
        Public POSPrice As String
        Public POSQty As String
        Public POSSalesDate As String
        Public POSSite As String
        Public POSTerminal As String
        Public POSRefNo As String
        Public POSTaxRefNo As String
        Public POSTaxValue As String
        Public POSAmount As String
        Public POSIssuanceType As String
        Public POSTime As String
        Public POSCurrency As String
        Public CodeSite As Integer
    End Structure

    'jhl 27.04.05
    Public Structure structSalesItem
        Public Code As Int32
        Public Number As String
        Public Name As String
        Public CodeSite As Int32
        Public LinkMissing As Boolean
        Public LastImport As Boolean
        Public Archive As Boolean
        Public Type As Integer
        Public Description As String
        Public DateModified As DateTime
        Public CodeSetPrice As Int32
        Public Price As Double
        Public Coeff As Double
        Public CodeTax As Int32
        Public SuggestedPrice As Double
        Public Barcode As String
        Public Active As Boolean
        Public Print As Boolean
        Public CodeProduct As Integer    'RDTC 26.07.2007

    End Structure

    'Public Structure structProduct
    '    Public Code As Integer
    '    Public Name As String
    '    Public Number As String
    '    Public Barcode As String
    '    Public Composition As String
    '    Public Note As String
    '    Public IncludeInventory As Boolean

    '    Public Unit As Integer
    '    Public UnitTranspo As Integer
    '    Public UnitPackaging As Integer
    '    Public UnitStock As Integer
    '    Public UnitRatio1 As Double
    '    Public UnitRatio2 As Double
    '    Public UnitRatio3 As Double
    '    Public GoodsRatio As Double
    '    Public GoodsUnit As Integer
    '    Public DaysExpire As Integer
    'End Structure

    Public Structure structProduct
        Public Code As Integer
        Public TranMode As Short
        Public Status As Short
        Public Category As Integer
        Public Number As String
        Public Name As String
        Public Description As String
        Public Unit As Integer
        Public UnitStock As Integer
        Public UnitPack As Integer
        Public UnitTrans As Integer
        Public UnitRatio As Double
        Public UnitRatio2 As Double
        Public UnitRatio3 As Double
        Public CodeSite As Integer
        Public Supplier As Integer
        Public Tax As Integer
        Public Currency As Short
        Public Price As Double
        Public AvgPrice As Double
        Public LastPrice As Double
        Public PriceMin As Double
        Public PriceMax As Double
        Public IsFresh As Boolean
        Public RecipeLinkCode As Integer
        Public GoodsRatio As Double
        Public GoodsRecipeUnit As Integer
        Public DoNotLink As Boolean
        Public DaysExp As Double
        Public IsGlobal As Boolean
        Public Type As Short
        Public RawMaterial As Boolean
        Public MultiSup As Boolean
        Public PriceUpdate As Short
        Public TransferFlag As Boolean
        Public IsSelfOrder As Boolean
        Public SupplierNumber As String
        Public QtyOnHand As Double
        Public QtyInventory As Double
        Public QtyMax As Double
        Public QtyMin As Double
        Public QtyOrderMin As Double
        Public QtyOrderMax As Double
        Public QtyOrderLast As Double
        Public QtyOrderDef As Double
        Public StockingPlace As Integer
        Public InInventory As Boolean
        Public InCurrentInventory As Boolean
        Public Barcode As String
        Public Economat As Short
        Public QuantityEconomat As Double
        Public Qty2Economat As Double
        Public InventPrice As Double
        Public ActionFlag As Boolean
        Public QtyInOrder As Double
        Public LocationProdDef As Integer
        Public LocationOutDef As Integer
        Public UseIO As Boolean
        Public QtyAllocated As Double
        Public AutoTransferOutlet As Boolean
        Public ExcludeFromAutoOutput As Boolean
        Public LastUnitUsed As Integer
        Public CodeSalesItem As Integer
        Public Composition As String
        Public ConsumptionDays As Double
        Public ConsumptionText As String
        Public PackingDate As Date
        Public PackingText As String
        Public AddInstruction As String
        Public UnitStockBarCode As String
        Public UnitPackBarCode As String
        Public Note As String
        Public Picture1 As String
        Public Picture2 As String
        Public Picture3 As String
        Public CodeUser As Integer
        Public IncludeIngredient As Boolean     ' MRC 031708 - For finished goods.
        Public WineProduct As Integer           ' MRC 04.21.09 - Wine Module
    End Structure

    Public Structure structProductTranslation 'VRP 09.02.2009
        Public Code As Integer
        Public TranMode As Short
        Public CodeTrans As Integer
        Public Name As String
        Public Composition As String
        Public AddInstruction As String
    End Structure

    Public Structure LocationData
        'VBV 19.12.2005
        Public Code As Int32
        Public Name As String
        Public IsGlobal As Boolean
        Public CodeSite As Int32
        Public Status As Integer
    End Structure
    Public Structure structIssuanceType
        Public Code As Int32
        Public CodeSite As Int32
        Public Name As String
        Public NumberRef As String
        Public Description As String
        Public IsGlobal As Boolean
        Public Position As Int16
    End Structure

    Public Structure structClient
        Public Code As Int32
        Public Status As Integer
        Public NameRef As String
        Public Number As String
        Public Company As String
        Public Email As String
        Public CompanyURL As String
        Public BusinessURL As String
        Public BillingAddress As String
        Public BillingCity As String
        Public BillingZip As String
        Public BillingCountry As String
        Public BillingState As String
        Public BillingTel As String
        Public BillingFax As String
        Public ShippingAddress As String
        Public ShippingCity As String
        Public ShippingZip As String
        Public ShippingCountry As String
        Public ShippingState As String
        Public ShippingTel As String
        Public ShippingFax As String
        Public Note As String
        Public AccountingID As String
        Public GLAccount As String
        Public AccountingRef As String
        Public IsGlobal As Boolean

        Public CodeClientContact As Int32
        Public Title As String
        Public FName As String
        Public LName As String
        Public JobPosition As String
        Public ContactTel As String
        Public ContactFax As String
        Public ContactEmail As String
        Public ContactMobile As String
    End Structure

    Public Structure structProductSupplier
        'MRC OCT 10, 007
        Public Name As String
        Public Unit As String
        Public Number As String
        Public CodeSupplier As Integer
        Public Supplier As String
        Public Note As String

        Public Price As Double
        Public Discount As Double
        Public QtyOrderMin As Double
        Public QtyOrderMax As Double
        Public QtyOrderDefault As Double
        Public QtyOrderLast As Double

        Public UnitStockBarCode As String
        Public UnitPackBarCode As String

        Public VatFlag As Boolean
    End Structure

    Public Structure structUpload
        Public Code As Int32
        Public HostName As String
        Public IPAddress As String
        Public FileType As enumUploadFileType
        Public Path As String
    End Structure

    Public Structure structMenuplan 'VRP 18.07.2008
        Public WeekDate As String
        Public DateStart As Date
        Public CodeTrans As String
        Public BusinessName As String
        Public BusinessNumber As String
        Public Street As String
        Public Zip As String
        Public City As String
        Public Phone As String
        Public Email As String
        Public Days As String
        Public Time As String
        Public OpenDays As String
        Public OpenTime As String
        Public CloseDays As String
        Public NoofProposal As Integer
        Public ProposalName As String
        Public Price As String
        Public CodeLogo As Integer
        Public PrintNut As Boolean
        Public PrintGDA As Boolean
    End Structure


    Public Structure structProcetureStyle 'VRP 20.11.2008
        Public strFontNameH As String
        Public dblFontSizeH As Double
        Public strFontColorH As String
        Public strBGColorH As String
        Public blnIsBoldH As Boolean
        Public blnIsItalicH As Boolean
        Public blnIsUnderlineH As Boolean
        Public dblFontSBH As Double
        Public dblFontSAH As Double
        Public strFontNameD As String
        Public dblFontSizeD As Double
        Public strFontColorD As String
        Public strBGColorD As String
        Public blnIsBoldD As Boolean
        Public blnIsItalicD As Boolean
        Public blnIsUnderlineD As Boolean
        Public dblFontSBD As Double
        Public dblFontSAD As Double
    End Structure

    Public Structure structInventory 'VRP 07.05.2009
        Public strName As String
        Public strNote As String
        Public dteDateBegin As Date
        Public intOpenFrom As Integer
        Public intCodeSite As Integer
        Public intCodeTrans As Integer
    End Structure

    Public Structure sFilters 'Menu Engineering 09.08.2010
        Dim dateFrom As DateTime
        Dim dateTo As DateTime
        Dim Cost As enumCost '0:LastPrice, 1:AveragePrice
        Dim SalesEncode As enumSalesEncoded '0:Manual, 1:Automatic
        Dim Outlet As Integer
        Dim Category As Integer
        Dim TypeProduct As Boolean
        Dim TypeRecipe As Boolean
        Dim TypeMenu As Boolean
        'Dim TopList As enumTopList
        'Dim ShowTop As Int16
        Dim SetPrice As Integer
        Dim SalesListCode As Integer
    End Structure

    Public Enum enumApplicationType
        FBControl = 1
        CalcmenuWeb = 2
    End Enum



    Public Structure sCurrentList
        Dim name As String
        Dim description As String
        Dim forecastfrom As Date
        Dim forecastto As Date
        Dim creator As String
        Dim codeuser As String
        Dim outlet As Integer
        Dim outletname As String
        Dim type As Integer
        Dim code As Integer
        Dim Cost As enumCost '0:LastPrice, 1:AveragePrice
        Dim TypeProduct As Boolean
        Dim TypeRecipe As Boolean
        Dim TypeMenu As Boolean
        Dim salesencode As enumSalesEncoded
        Dim setprice As Integer
        Dim category As Integer
    End Structure


    Public Structure sApplicationType
        Dim apptype As enumApplicationType
    End Structure


    Public Structure sRecomputeList
        Dim codelist As Integer
        Dim codesalesitem As Integer
        Dim qtysold As Integer
        Dim price As Double
        Dim cost As Double
        Dim menuengid As Integer
    End Structure

    ' RDC 11.11.2013 : Added for Export to Word Feature
    Public Structure WordExportOptions
        ' Basic Information
        Public blnExpIncludeLanguage As Boolean
        Public intExpSelectedLanguage As Integer
        Public blnExpIncludeRecipeNo As Boolean
        Public blnExpIncludeSubName As Boolean
        Public blnExpIncludeItemDesc As Boolean
        Public blnExpIncludeRemark As Boolean
        Public blnExpIncludeYield1 As Boolean
        Public blnExpIncludeYield2 As Boolean
        Public blnExpSubRecipeWt As Boolean
        Public blnExpIncludeRecipeTime As Boolean
        Public blnExpIncludeGrossQty As Boolean
        Public blnExpIncludeNetQty As Boolean
        Public blnExpIncludeMetricNetQty As Boolean
        Public blnExpIncludeMetricGrossQty As Boolean
        Public blnExpIncludeImperialNetQty As Boolean
        Public blnExpIncludeImperialGrossQty As Boolean
        Public blnExpIncludeProcedure As Boolean
        Public intExpSelectedProcedure As Integer
        Public blnExpIncludeNotes As Boolean
        Public blnExpIncludeAddNotes As Boolean
        Public blnExpIncludeNutrientInfo As Boolean
        Public intExpSelectedNutrientSet As Integer
        Public intExpSelectedNutrientComputation As Integer
        ' RDC 02.11.2014
        Public blnExpIncludeGDA As Boolean
        Public intExpSelectedGDA As Integer

        ' Advanced Information 
        Public blnExpAdvIncludeInfo As Boolean
        Public blnExpAdvIncludeBrands As Boolean
        Public blnExpAdvIncludeKeywords As Boolean
        Public blnExpAdvIncludeCookbook As Boolean
        Public blnExpAdvIncludePublication As Boolean
        Public blnExpAdvIncludeComments As Boolean

        'AGL 2015.01.22
        Public intEnergyDisplay As Integer

    End Structure

#End Region

#Region "Enums"
    Public Enum enumSalesEncoded
        Manual = 0
        Automatic = 1
    End Enum

    Public Enum enumNutrientImposedType
        YieldUnit = 1
        YieldUnit2 = 2
        PortionUnit = 3
    End Enum

    Public Enum EGSPrograms As Int16
        RecipeNet = 1
        CM2004 = 2
        FBControl = 4
        CMChef = 32
        InventoryControl = 64
        EGSSolution = 512
    End Enum

    Public Enum FormID As Long
        IX_UNDEFINED = 0
        IX_ITEM = 1
        IX_INVENT = 2
        IX_IO = 4
        IX_HISTORY = 8
        IX_REQLIST = 16
        IX_ORDERLIST = 32
        IX_ORDER = 64
        IX_RECORDER = 128
        IX_INVENTLIST = 256
        IX_PRODUCTION = 512
        IX_SALESDETAILS = 1024
        IX_PRODUCTIONLIST = 2048
        IX_PRODUCTGROUP = 4096
        IX_REQUESTORDERLIST = 8192
        IX_REQUESTORDER = 16384
        IX_DELIVERY = 32768
        IX_DELIVERYLIST = 65536
        IX_INVOICELIST = 131072
        IX_TRANSFER = 262144
        IX_TRANSFERLIST = 524288
        IX_TRANSFERALL = 1048576
        IX_TRANSFERREC = 2097152
        IX_TRANSRECEIVING = 4194304
        IX_TRANSDRLIST = 8388608
        IX_TRANSDRITEMS = 16777216
        IX_ASSEMBLY = 33554432
        IX_SALESITEMLIST = 67108864
        IX_LINKSALESITEM = 134217728
    End Enum

    Public Enum enumAppType As Byte
        WebApp = 1
        SmartClient = 2
        WebService = 3
    End Enum

    Enum enumDType
        TYPE_STRING = 1
        TYPE_INTEGER = 2
        TYPE_BOOLEAN = 3
        TYPE_DOUBLE = 4
        TYPE_DATETIME = 5
    End Enum

    Enum enumFormType
        'RDTC
        'Data List form or Update popup form?
        List = 1
        Update = 2
    End Enum

    Public Enum enumEgswFetchType
        [DataReader] = 1
        [DataTable] = 2
        [DataSet] = 3
        [UseDefault] = 4
        [ArrayList] = 5
    End Enum

    Public Enum enumEgswStandardizationFormat
        None = 0
        LowerCase = 1
        UpperCase = 2
        SentenceCase = 3
        TitleCase = 4
    End Enum

    Public Enum enumEgswMarkType
        CurrentlySelected = 0
        Modify = 2
        Delete = 3
        Merge = 4
        Deactivate = 8
        Share = 10
        Copy = 13
        Marked = 14
        NotDeleted = -6 'For deactivation
        RnCatMarkedFromFb = 15
    End Enum

    Public Enum enumEgswTypeItems
        NoType = 0
        SavedMark_Merchandise = enumDataListItemType.Merchandise
        SavedMark_Recipe = enumDataListItemType.Recipe
        SavedMark_Menu = enumDataListItemType.Menu
        SavedMark_SalesItem = enumDataListItemType.SalesItem
        'MRC Oct. 17, 2007 - Added for saving of marks for Products.
        SavedMark_Product = enumDataListItemType.Product
    End Enum

    Public Enum enumEgswTransactionMode
        'Transacation types
        None = 0
        Add = 1
        Edit = 2
        Delete = 3
        MergeDelete = 4
        Purge = 5
        MovePositionUp = 6
        MultipleDelete = 7
        Deactivate = 8
        DeleteAll = 9
        ModifyStatus = 10
        MergeHide = 11
        ModifyIsGlobalProperty = 12
        Copy = 13

        ModifyPasswordOnly = 20
        ModifySecurityQuestionAndAnswer = 21 'AGL 2014.09.16
        ResetPassword = 22 'AGL 2014.09.17

        UpdateAutoConversion = 30
        UpdateStatus = 31

        MovePositionDown = 40
        Standardize = 41
        Export = 42

        GetAllItems = 50
        GetOneItem = 51
        GetPreferredItem = 52


        ' JRL 26.9.2005 
        UpdateSystemKeyAndHeaderOnly = 53
        UpdateSystemenumGroupLevel = 54

        ' JRL 29.9.2005
        UpdateCodeTrans = 55

        ' JRL 1.10.2005
        FixPositionCount = 56

        UpdateFlagsOnly = 57
        MergeSystem = 58

        SetDefault = 59

        'salesitem for elvetino
        FlagActive = 60
        FlagPrint = 61
        FlagDeactivate = 62
        FlagDeprint = 63

    End Enum

    Public Enum enumDataListItemStatus As Byte
        All = 255
        Open = 0
        Closed = 1
    End Enum

    Public Enum enumDataListItemType As Short
        NoType = 0

        Merchandise = 2
        Recipe = 8
        Menu = 16
        Text = 4
        Separator = 32
        MenuCard = 28
        SalesItem = 7
        Product = 45
        Sales = 48
        Steps = 75
        MenuPlan = 24 'VRP 31.07.2008 TEST MENU PLAN
    End Enum

    Public Enum enumDataListType As Short
        'RDTC
        NoType = 0
        Sales = 48
        SalesItem = 7
        Product = 45

        Merchandise = 2
        Text = 4
        Recipe = 8
        Menu = 16
        Item = 64       'Recipe, Merchandise AND Menu

        Category = 1
        CategoryMerchandise = 66
        CategoryRecipe = 67
        CategoryMenu = 68

        Unit = 3
        Yield = 5
        Keyword = 6
        Source = 9
        Tax = 10
        Supplier = 11
        User = 12
        Brand = 14
        RecipeSingle = 15
        MenuSingle = 17
        Price = 18
        ShoppingList = 19
        NutrientRules = 20
        Labor = 21
        Connection = 22
        MenuMeal = 23
        MenuPlan = 24
        MenuProposal = 25
        MenuDay = 26
        Translation = 27
        MenuCard = 28

        Dash = 32
        SetOfPrice = 33

        Ingredient = 34
        MenuItems = 35
        Separators = 36
        PrintProfile = 37
        Allergen = 38
        Terminal = 39
        POSImportConfig = 40
        POSTempData = 41
        Location = 42
        IssuanceType = 43
        Client = 44
        ClientContact = 45
        SupplierContact = 46
        SupplierGroup = 47

        ProductionPlace = 49 'MRC 25.04.2008
        ProcedureStyles = 50 'VRP 01.06.2008
        MenuEngineering = 51
        ProcedureTemplate = 52 'VRP 27.04.2009
        Inventory = 1004 'VRP 12.05.2009

        'mrc 05.27.09 - Wine-related types
        Country = 53
        Region = 54
        SubRegion = 55
        Producer = 56
        WineType = 57
        GrapeVarietal = 58

        'Jbb 01.26.2011
        Placement = 59

        'RDC 02.21.2013 Recipe Management module
        TimeType = 70
        ' RDC 03.12.2013 - CMW-3300 Enhancement
        CookBook = 71
        MechandiseName = 72
        RecipeName = 73
        Publication = 74
        'Source = 9
        'Supplier = 11
        NutrientSet = 75
        KeywordMenu = 76
        'CategoryMenu = 68

        ' RDC 03.22.2013 CWM-4150 Enhancement
        ' Brand = 14
        ' CategoryMenu = 68
        ' CategoryMerchandise = 66
        ' CategoryRecipe = 67
        ' Cookbook = 71
        ' KeywordMenu = 76
        KeywordMerchandise = 77
        KeywordRecipe = 78
        Kiosk = 79
        ' NutrientSet = 75
        ' Publication = 74
        ' Source = 9
        ' Supplier = 11
        TaxRates = 80
        Units = 81
        ' RDC 04.12.2013 CWM-5212 Enhancement
        RecipeTime = 82

        'AGL 2013.07.02
        Role = 83

        'AGL 2015.03.04
        Restaurant = 59 'MRC 09.04.2009
    End Enum

    Public Enum enumScopeLevel
        'CMLevel = 0
        NoLevel = 0
        UserLevel = 1
        SiteLevel = 2
        SystemLevel = 3
        CMLevel = 10
    End Enum

    Public Enum enumEgswErrorCode
        'IMPORTANT!!!  All errors are less than ZERO
        'For RnWeb: starts with -2000
        'For EgsSolution 
        '   FBControl modules: -90 - -999
        '   RecipeNet modules: -1000 - -1999
        '   Configuration related: -2000 - -2100
        'List related shared by all solution (ie. Category, Supplier, Location): consume less than -90
        OK = 0
        GeneralError = -1
        ExecuteProcedure = -2
        DuplicateExistsActive = -3 'DuplicateExists
        NothingDone = -4
        TransactionClosed = -5
        OneItemNotDeleted = -6
        InvalidCodeSite = -7
        FK = -8
        NotExists = -9
        ItemLocked = -10
        ItemClosed = -11
        DuplicateExistsInactive = -12
        RequestInProcess = -13
        RequestNotInProcess = -14
        InsufficientRoleLevel = -15
        SiteHasNoUser = -16
        MethodNotSupported = -17 'Methods that are no longer supported
        TransactionAlreadyClosed = -18
        TransactionAlreadyOpened = -19
        SalesItemNumberExists = -20
        MissingBrand = -21
        MissingCategory = -22
        MissingSource = -23
        MissingSupplier = -24
        MissingUnit = -25
        MissingListe = -26
        MissingKeyword = -27
        InvalidCodeList = -30 'list of Codes/Ids passed to the stored procedure for multiple action
        FistTran = -52
        ItemNoInvent = -60
        ItemNoLocation = -61
        InvalidStockType = -62
        CannotSwitch = -63
        InvalidSupplier = -64
        UsedAsDirectIO = -70
        CannotShareItems = -71
        CorruptConfig = -80
        InvalidRights = -81
        MergingMultipleGlobalItem = -82
        ItemAlreadyReceived = -83
        InvalidListType = -90
        InvalidTranMode = -91
        InvProdLocInOpenInvent = -92
        InvProductLockAfterDate = -93
        InvProductExistsInLatestInvent = -94
        InvTransExistsAfterBeginDate = -95
        InvNoProductsForInvent = -96
        InvSomeProductsAreInOpenInvent = -97
        NumberTooLong = -100
        NameTooLong = -101
        SupplierTooLong = -102
        CategoryToolong = -103
        DescriptionTooLong = -104
        UnitTooLong = -105
        InvalidPrice = -106
        OrdCantDelete = -110
        OrdCantCreateNewRequisition = -111 'creating new requisition for Merged PO
        OrdCantCreateAutoNumber = -112
        OrdCantMergeDifferentSuppliers = -113
        ReqNoProductsWithQuantities = -120
        DrAlreadyAttachedToInvoice = -130
        DrIsBeingUsedInOrder = -131
        DrSomeItemsAreNotYetInStock = -132
        EAUserNotInApproverList = -140
        EAUserIsNotApprover = -141
        EANotAllRequiredHaveApproved = -142
        EANotAllHaveApproved = -143
        InvalidName = -144
        CfgInvalidAutoNumberPrefixLen = -2000
        PromoCodeError = -2001
        NotApplicable = -2002 'DLS
        OneItemNotDeactivated = -2003 'DRR
        MerchandiseInUse = -2004 'AGL 2012.10.04 - CWM-1330

        'AGL 2014.09.12
        PasswordLengthInsufficient = -2005
        PasswordReuseViolation = -2006
        PasswordComplexityNotMet = -2007
        PasswordExpired = -2008
        UserLoginLockedOut = -2009
        InitialPasswordNotYetChanged = -2010
    End Enum

    Public Enum ShareType
        CodeSite = 1
        CodeProperty = 2
        CodeUser = 3
        CodeSiteView = 5
        CodePropertyView = 6
        CodeUserView = 7
        CodeUserOwner = 8
        ExposedViewing = 9
        SubmittedToSystemForGlobalSharing = 10
    End Enum

    Public Enum SetPriceType
        NoType = 0
        Purchase = 1
        Sale = 2
    End Enum

    Public Enum UnitsType
        Stock = 1
        Packaging = 2
        Ingredient = 3
        Yield = 4
        Transportation = 5
    End Enum

    Public Enum enumGroupLevel
        [Global] = 0
        Site = 1
        [Property] = 2
        User = 3
    End Enum

    Public Enum MenuType
        [default] = 0
        ' Index
        Home = 991
        SalesItem = enumDataListType.SalesItem
        Merchandise = enumDataListType.Merchandise
        Recipe = enumDataListType.Recipe
        Text = enumDataListType.Text
        Menu = enumDataListType.Menu
        MenuCard = enumDataListType.MenuCard
        Product = enumDataListType.Product
        Sales = enumDataListType.Sales
        MenuEngineering = enumDataListType.MenuEngineering

        Configuration = 992
        ContactUs = 993

        ' Merchandise
        main = 100
        pricing = 101
        nutrients = 104
        keywords = 105
        sharing = 106
        pictures = 107
        signout = 108
        translate = 109
        info1 = 114
        sharingOwner = 115
        allergen = 116

        ' Recipe
        Calculate = 210
        Ingredient = 211
        ' Marvin Nov 23 2007 - for recipe encoding module
        IngredientText = 214
        Note = 212
        ViewHistory = 215 'AGL 2013.11.29
        'Composition = 217 'AGL 2014.11.14

        'Menu
        menuItems = 213
        Procedure = Note

        ' Admin
        ManageMerchandiseCategory = 501
        ManageGroups = 502
        ManageMainGroups = 503
        ManageNutrients = 504
        ManageNutrientRules = 510
        ManageTools = 505
        ManageTranslate = 506
        ManageUnits = 507
        'ManageSysUsers = 511
        ManageUsers = 508
        ManageYields = 509
        'ManageMerchandiseParentKeywords = 600
        ManageSetOfPrice = 601
        ManageTax = 602
        manageLanguage = 603
        ManageBrand = 604
        'ManageBrandSub = 605
        'ManageMerchandiseCategorySub = 606
        ManageImportListe = 607
        ManageSuppliers = 608
        'ManagePackaging = 609
        'ManageContainer = 610
        ManageRecipeCategory = 611
        'ManageRecipeCategorySub = 612
        ManageRecipeKeywords = 613
        'ManageAllergens = 615
        'ManageAdditives = 616
        'ManageDietetic = 617
        ManageMerchandiseKeywords = 618
        MerchandiseMarks = 619
        RecipeMarks = 620
        TextMarks = 621
        MerchandisePrintList = 622
        RecipePrintList = 623
        ShoppingListRecipe = 624
        ShoppingListeMenu = 642
        MerchandisePrintDetails = 625
        RecipePrintDetails = 626
        ManageMenuCategory = 627
        'ManageMenuSubCategory = 628

        ManagePrefix = 2101
        MerchandiseNew = 2102
        RecipeLink = 133216
        Labels = 2023

        MenuMarks = 629
        MenuPrintDetails = 630
        TextSearch = 631
        RecipeSearch = 632
        MenuSearch = 633
        ProductSearch = 634
        TextNew = 635
        RecipeNew = 636
        MenuNew = 637
        ProductNew = 638
        ManageConversionRate = 639
        ManageSystemPref = 640
        ManageApprovalPref = 646
        'ManageAdminProfile = 647
        'ManageMerchandiseKeywords = 642
        ManageSources = 643
        ManageCurrency = 645

        PendingApproval = 700
        PendingRequest = 701

        Recipe_MenuCards = 702
        Menu_MenuCards = 703

        ManageMenuKeywords = 704
        Options = 705
        'ApproveNewEntry = 707
        'ApproveChangeEntry = 708
        ApproveNewChangeEntry = 707
        ApproveReplaceEntry = 709
        ApprovePriceUpdate = 710
        ManageEmailPref = 711
        ApproveSubmitToSystem = 712
        ManageImagePref = 713
        '    ManageFTSPref = 714
        ApproveRecipeExchange = 715
        RecipeHistory = 716
        MerchandiseHistory = 717
        MenuHistory = 718

        ManageIPBlockList = 719

        TextPurge = 720
        TextStandardize = 721
        RecipeExchange = 722
        PendingRequestRecipeExchange = 723
        PendingRequestMain = 724
        ManageLaborCost = 725
        ManageLogs = 726
        ManageLabor = 727

        'FB Modules
        ProductFB = 1000
        AllowStockInput = 1001
        AllowStockOutputSpoilage = 1002
        AllowStockTransfer = 1003
        Inventory = enumDataListType.Inventory
        TransferRequest = 1006
        AllTransfers = 1008
        TransferReceipt = 1010
        TransferRequestReceiving = 1012
        Receiving = 1013
        PurchaseOrder = 1014
        Requisition = 1015

        ProductFBUpdatePrices = 1016 'update of prices (for linked application)
        ProductFBChangePrice = 1017 'update prices only
        DeliveryReceipt = 1018
        SalesEncodeManually = 1020 'option is allow only 
        SalesOrder = 1021
        Invoice = 1022
        POSItemList = 1023  'SalesItemList
        ClientDR = 1024

        Production = 1025
        ProductionChangeStatusToInProcess = 1027
        ProductionChangeStatusToInStock = 1028
        ProductionChangeStatusToOnHold = 1029
        ProductionChangeStatusToCompleted = 1030
        ProductionChangeStatusToCancelled = 1031

        ManageLandBasedCompanies = 1040
        ManageVessels = 1041
        ManageClients = 1050
        ManageTerminals = 1051
        ManageIssuances = 1052
        ManageTimeExpected = 1053
        ManageLocation = 1054
        ManageCategoryFB = 1055

        'Option is allow only
        ProductFBLockUnlock = 1056
        DeleteTransactions = 1057 'Input/Output/Transfer/Correction
        AllowModifyDeleteB4LastInvent = 1058

        LinkToPOS = 1060
        LinkToPocket = 1061
        LinkToRecipeDatabase = 1062
        ExportToAccounting = 1063

        FBHistory = 1064
        StockReport = 1065
        StockInfo = 1066
        ImportFromPOS = 1067


        Registration = 1068
        CMOnlineTerms = 2012
        CMOnlineLangPicture = 2013
        CMOnlineSubscription = 2014
        CMOnlineFeatures = 2015
        CMOnlineFAQ = 2016
        CMOnlineSubscriptionView = 2017
        CMOnlineConfirm = 2018
        CMOnlineDefaultPrintProfile = 2019



        ' JRL 26.09.2005
        Recipe_MenuCardSearch = 1069
        Menu_MenuCardSearch = 1070
        TextActions = enumDataListType.Text
        MerchandiseActions = enumDataListType.Merchandise
        RecipeActions = enumDataListType.Recipe
        MenuActions = enumDataListType.Menu
        SystemConfigList = 1071
        UsersConfigList = 1072
        MerchandiseConfigList = 1073
        ToolsConfigList = 1074
        SecurityConfigList = 1075
        RecipeConfigList = 1076
        MenuConfigList = 1077

        ManageRoleRights = 1078
        StandardizeBasic = 1079
        PurgeBasic = 1080
        Recalculate = 2040

        ManagePrintProfile = 2001
        MenuPrintList = 2002

        ExportTCPOS = 2003
        CSVImportOption = 2004
        CSVImportTemp = 2005

        ManageAllergen = 2006
        ManagePurchaseSetOfPrice = 2007

        'JLC
        SalesConfigList = 2008
        ManageSetOfPriceSales = 2009
        ManagePOSImportConfig = 2010
        ManagePOSTempData = 2011
        ManageBackupRestoreDbase = 2020
        ManageBackupRestorePicture = 2021
        ViewLicense = 2022
        LinkToFinishedGood = 2023
        ManageIssuanceType = 2024
        ManageClientContact = 2025
        ManageSupplierGroup = 2026
        ManageSupplierContact = 2027
        Detail = 2028
        LinkToProducts = 2029
        ProductSupplier = 2030
        ProductPictures = 2031
        ExportShopList = 2032
        SalesItemNew = 2033
        SalesItemMark = 2034

        ManageImages = 2035 'DLS Jan252007
        SalesHistoryDetails = 2036
        SalesTmp = 2037
        SalesItemLinking = 2038

        UploadOption = 2039

        MonitorBreadcrumbs = 2041 'VRP 21.09.2007
        ManageDefaultProcedure = 2042 'VRP 24.10.2007
        ManageProductionPlace = 2043 'MRC 24.04.2008

        ManageStudent = 2044 'VRP 30.04.2008 

        MenuPlan = enumDataListType.MenuPlan 'VRP 12.05.2008 test
        Haccp = 2045 'VRP 15.05.2008 new tab

        ShowStepOnProcedures = 2046 'MRC 06.16.08
        ManageAutoNumber = 2047 'MRC 06.18.08

        ManageProcedureStyles = 2048 'VRP 30.06.2008
        ManageMenuplan = 2049 'VRP 17.07.2008
        BulkImportation = 2050 'VRP 02.09.2008
        ConvertDB = 2051 'VRP 16.09.2008

        'MRC - 10.16.08
        SalesAnalysis = 2052    'For Menu Engineering and Sales Forcasting
        BulkImportationRecipe = 2053 'DLS

        'VRP 28.01.2009
        MerchandiseNutrientList = 2054
        RecipeNutrientList = 2055
        MenuNutrientList = 2056

        'mrc- 05.25.09
        ManageCountry = 2057
        ManageRegion = 2058
        ManageSubRegion = 2059
        ManageProducer = 2060
        ManageWineType = 2061
        ManageGrapeVarietal = 2062

        'MRC 09.01.09   -   Migros Menu Planning
        ManageMasterPlan = 2063

        'MRC 11.09.10   -   Migros CSV Export for OST
        CSVExport = 2064


        '---------------------
        'Additional USA Manage
        '---------------------
        ManagePlacement = 2071          ' JBB 01.26.2011
        ManageBrandSite = 2072          ' JBB 02.04.2011
        ManageProject = 2073            ' JBB 02.08.2011
        ManageNutrientSet = 2078        ' JBB 05.16.2012
        ManageProcedureTemplate = 2100 'WVM-2014.10.01

        '-------------------
        'Additional USA Tabs
        '-------------------
        Project = 2074                  ' JBB 02.11.2011
        BrandSite = 2075                ' JBB 02.11.2011
        WorkFlow = 2069
        RecipeTime = 2070               ' JBB 01.18.2010
        ImposedNutrient = 2067          ' MRC 12.13.10 - Unilever USA Imposed Nutrients for Recipes
        RecipeProjectList = 2068        ' JBB 12.29.2010
        Comments = 2065                 ' JRN 11.30.10 -unilever USA
        Placement = 2066                ' JBB 12.09.2010 -unilever USA
        Brand = 2076
        Attachment = 2077               ' JBB 04.28.2011 - Uniliver USA


        '-- Configuration for Consumers--
        '-- JBB 08.31.2012
        MetricANDImperial = 2080


        '--

        ' RDC 02.21.2013 Additonal Recipe Time management module
        ManageTime = 2079

        ' RDC 03.12.2013
        ManageMerchandiseName = 2081
        ManageRecipeName = 2082
        'AGL 2013.07.08
        'ManagePublication = 2083
        'ManageCookbook = 2084
        'ManageSource = 2085
        'ManageSupplier = 2086
        'ManageNutrientSetStandard = 2087
        'ManageKeywordMenu = 2088
        'ManageCategoryMenu = 2089
        ' RDC 03.22.2013 CWM-4150 Enhancement
        'ManageKeywordMerchandise = 2090
        'ManageKeywordRecipe = 2091
        'ManageKiosk = 2092
        'ManageTaxRates = 2093
        ' RDC 04.12.2013 CWM-5212 Enhancement
        ManageRecipeTime = 2094

        'AGL 2013.07.02
        ManageRoles = 2095

        'MRC 2013.07.05
        ManageDigitalAssets = 2096
        'AGL 2014.07.30
        RecipeAbbreviatedPreparation = 2097
        'AGL 2014.06.25
        ManageAlias = 2098
        'AGL 2014.09.11
        ManagePasswordAndLogin = 2099

        'MKAM 2015.02.16
        Declaration = 2103
        MenuPlanPrint = 2104
        ShoppingListMenuPlan = 2105

        'Raqi Pinili 2015.12.23
        ExportRecipeLabel = 2106

        'NBG 3.17.2016
        ManageNotes = 2111

        AlternativeIngredient = 2112

        ManagePackagingMethod = 2113
        ManageStorageInformation = 2114
        ManageSaleSite = 2115

        'LLG 3.31.2016
        Supplier = 2116
        AddArticle = 2118

        'JOP 4-11-2016
        ManageRecipeWorkflow = 2117

        'MKAM 2016.05.06
        MenuPlanConfigList = 2120
        ManageRestaurant = 2121
        ManageMenuPlanCategory = 2122
        ManageSeason = 2123
        ManageTypeofService = 2124


        ManagePrinter = 2125
        MenuPlanNew = 2126

        'JOP 11-10-2016
        ProductionLocation = 2127
    End Enum

    Public Enum UserRightsFunction
        AllowUse = 1
        AllowCreate = 2
        AllowModify = 3
        AllowDelete = 4
        AllowMerge = 5
        AllowStandardize = 6
        AllowTranslate = 7
        AllowPurge = 8
        AllowActivation = 9
        AllowSearch = 10
        AllowDeleteGroup = 11
        AllowDeleteSystem = 12
        AllowCopy = 13
        AllowReplace = 14
        AllowAssignKeyword = 15
        AllowMarking = 16
        AllowSaveMark = 17
        AllowLoadMark = 18
        AllowExecuteActionMark = 19
        AllowPrintList = 20
        AllowCreatePDF = 21
        AllowImport = 22
        AllowExport = 23
        AllowSubmit = 24
        AllowConvertToSystem = 25
        AllowApproveSubmitted = 26
        '  AllowTransferOwned = 27
        AllowTransfer = 28
        AllowAssignPicture = 29
        'AllowWeightcalculation = 30
        AllowResizing = 31
        'AllowModifyIngredientList = 32
        'AllowModifyIngredientPosition = 33
        AllowCosting = 34
        'AllowNutrientAnalysis = 35
        AllowDisplayNutrients = 35
        AllowEmail = 36
        AllowCreateUseSubRecipe = 37
        AllowExpose = 38
        AllowCreateShoppingList = 39
        AllowModifyItemList = 40
        AllowModifyItemPosition = 41
        'AllowNutrientEncoding = 42
        AllowEncodeNutrients = 42
        AllowPrintShoppingList = 43
        AllowPublishOnWeb = 44
        AllowCreateMenuCard = 45
        AllowSubmitToSystem = 46
        AllowBookmark = 47
        AllowCostingEncoding = 48
        AllowPreparation = 49
        AllowPreparationEncoding = 50
        AllowAllergen = 51
        AllowAllergenEncoding = 52
        AllowComposition = 53   'MKAM 2015.07.07

        AllowRollback = 100
        AllowDeleteDetails = 101

        AllowSort = 102
        AllowUnExpose = 103
        AllowSharing = 104
        AllowMoveUp = 105
        AllowMoveDown = 106
        'AllowSharingOwner = 107
        AllowPrintDetails = 108
        AllowMassUnpublish = 109    ' Use in Mass Mutation only
        AllowMassChangeBrand = 110
        AllowMassChangeCategory = 112
        AllowMassChangeSupplier = 113
        AllowMassChangeSource = 114
        AllowPrintNutrientList = 115

        AllowSearchNutrientRules = 116 'allow user to use nutrient rules ins earching
        AllowSearchAllergen = 117 'allow user to use allergen in searching

        AllowLinktoPOS = 118
        AllowLinktoFinishedGood = 119
        AllowDeactivate = 120
        AllowPrintPriceList = 121
        AllowLinktoProduct = 122

        AllowPrintActivate = 123
        AllowPrintDeactivate = 124

        AllowAssignOwner = 125 'VRP 09.10.2007
        AllowPrintLabel = 126 'VRP 24.10.2007

        AllowExportToExcel = 127 'VRP 06.03.2008
        AllowModifyMediaFile = 128 'VRP 12.05.2008

        AllowHaccpEncoding = 129 'VRP 15.05.2008

        AllowStepsOnProcedure = 130 'MRC 16.06.2008
        AllowCreateProtectedCopy = 131 'MRC 09.02.2008
        AllowProtect = 132 'MRC 09.02.2008
        AllowUnprotect = 133 'MRC 09.02.2008
        AllowCopySitePlan = 134 'VRP 08.12.2008 FOR SV MENU PLAN
        AllowCopyPrices = 135 'VRP 25.03.2009 

        AllowUseWine = 136                                              'MRC 05.05.2009

        AllowMassAddSupplier = 137                                      'MRC 05.14.2009
        AllowMassChangeTax = 138                                        'MRC 05.14.2009
        AllowMassAddLocation = 139                                      'MRC 05.14.2009
        AllowMassChangeDefaultLocation = 140                            'MRC 05.14.2009
        AllowMassChangeDefaultProductionLocation = 141                  'MRC 05.14.2009
        AllowMassChangeDefaultOutputLocation = 142                      'MRC 05.14.2009
        AllowMassChangeMinMaxQtyInStock = 143                           'MRC 05.14.2009
        AllowMassChangeMinMaxQtyToOrder = 144                           'MRC 05.14.2009
        AllowMassChangeDefaultQtyToOrder = 145                          'MRC 05.14.2009
        AllowMassChangeInventory = 146                                  'MRC 05.14.2009
        AllowMassChangeRawMaterial = 147                                'MRC 05.14.2009
        AllowMassChangeUseInputOutput = 148                             'MRC 05.14.2009
        AllowMassChangeAutomaticTransferToOutletBeforeAnOutput = 149    'MRC 05.14.2009
        AllowMassChangeExcludeFromAutomaticOutputOperation = 150        'MRC 05.14.2009 
        AllowMassAddTransferRequestAfterOutputOfSoldItems = 151         'MRC 05.14.2009
        AllowMassChangeDoNotLinkToCalcmenu2009 = 152                    'MRC 05.14.2009
        AllowMassSetLastSupplierUsedAsDefaultSupplier = 153             'MRC 05.14.2009
        AllowMassRemoveSupplier = 154                                   'MRC 05.14.2009

        'These are Admin rights for the Product                         'MRC 06.08.2009
        AllowMassAddDetail = 155
        AllowMassRemoveDetail = 156
        AllowCompareRecipe = 157                                        'JBB Dec 01, 2010 (Unilever USA Compare Recipe)
        AllowAddEditPlacement = 158                                     'MRC 12.01.2010
        AllowDeletePlacement = 159                                      'MRC 12.01.2010
        AllowChangeStatus = 160                                         'MRC 12.01.2010
        AllowVersion = 161                                              'JBB Dec 02, 2010 (Unilever USA Recipe Vesrioning)
        AllowCookbook = 162                                              'JBB 12.29.2010
        AllowExporttoWord = 163                                         'JBB 04.05.2011
        AllowCookmode = 165                                             'JBB 07.01.2011
        AllowRatings = 166                                             'JBB 07.04.2011
        AllowMassRecipeandWebStatus = 167                               'JBB 04.02.2012 (Recipe and Web Status Change)
        AllowMassRecipePlacement = 168                                  'JBB 04.03.2012
        'For Migros only..
        AllowMassChangeImposedPrice = 164                               'MRC 05.09.2011

        AllowDigitalAsset = 169
        AllowTips = 170

        AllowExportListToCSV = 171                                      ' RBAJ-2012.08.20 (Exporting)
        AllowExportListToExcel = 172                                    ' RBAJ-2012.08.20 (Exporting)
        AllowExportListToWord = 173                                     ' RBAJ-2012.08.20 (Exporting)
        AllowExportListToPDF = 174                                      ' RBAJ-2012.08.20 (Exporting)

        'AllowViewHistoryLogs = 20405 'AGL 2013.11.29
        AllowViewHistoryLogs = 209 'AGL 2013.11.29 'MKAM 2015.09.03

        ''-- JBB Merge 09.04.2012
        AllowManualCosting = 175
        AllowLegacyNumber = 176
        AllowFootnote1 = 177
        AllowFootnote2 = 178
        AllowShowOff = 179
        AllowChefRecommended = 180
        AllowProviderForExternalWebsite = 181
        AllowBrandSite = 182
        AllowPublication = 183

        ''--
        'AGL 2012.10.25
        'AllowMassChangeKiosk = 184 'AGL 2013.12.26 - 10307 - removed

        'JTOC 24.05.2013
        AllowVerifyRecipeTranslations = 185

        'JTOC 17.06.2013
        AllowRecipeIngredientApproval = 186

        'JTOC 05.07.2013
        'AGL 2013.07.18
        AllowPromoteBrand = 187

        'AGL 2013.07.30
        AllowEncodeComments = 188

        'AGL 2013.08.13 - RecipePrint (misc.)
        AllowRecipePrint = 189

        AllowMenuPrint = 196

        'AGL 2013.08.05
        AllowModifyMenuItems = 198

        ''AMTLA 2013.10.21
        AllowMoveMarkedItems = 199
        AllowPrintOrExport = 200
        AllowMoreActionSharing = 201

        ''RJL 01-14-2014
        AllowMerchandisePrint = 192

        '-WVM-2015.03.13
        AllowFinalAndVerified = 204
        AllowFinal = 205
        AllowVerified = 206

        AllowVersionCompare = 207

        AllowLabel = 208    'MKAM 2015.09.02 (Note: 119 is already in use)

        AllowMSCFoodReport = 209 'IAA MSC Food Report 12.18.2015 
        AllowMSCBeverageReport = 210 'IAA MSC Beverage Report 12.18.2015 
        AllowManorExportToExcel = 211 'IAA Manor Export To Excel 01.14.2016
        AllowWorkflow = 212

        AllowPrintLabel3 = 213 ' KMQDC 2016.05.17
        AllowPrintLabel4 = 214  ' KMQDC 2016.05.17
        AllowPrintLabel5 = 215

        AllowPrintNiceLabel = 216 'JOP

        AllowMenuplanPrintExport = 217  'MKAM 2016.11.09
        AllowMasterplanLocking = 218

        AllowEMenuPlan = 219

        EncodeImposedComposition = 221
        EncodePlannedValues = 222
        EncodeActualValues = 223

        AllowModifyOrigin = 224
    End Enum

    Public Enum enumListeDisplayMode
        Details = 0
        Thumbnail = 1
        List = 2
        ProjectList = 3
        NutrientView = 4 'AGL 2013.05.24
        AllergenView = 5 'MKAM 2014.07.03
    End Enum

    Public Enum enumFileType
        HTML = 0
        PDF = 1
        Excel = 2
        RTF = 3
        ' RDC 04.17.2013 - CWM-5325 Enhancement
        WordDocument = 4
    End Enum

    Public Enum enumReportType
        None = 0
        MerchandiseList = 1
        '        MechandiseListByCategory = 2
        RecipeList = 3
        RecipeDetail = 4
        MerchandiseDetail = 5
        '     RecipeListByCategory = 6
        ShoppingListDetail = 7
        '       ShoppingListDetailBySupplier = 8
        '      ShoppingListDetailByCategory = 9
        MenuDetail = 10
        MenuList = 11
        MerchandiseNutrientList = 12
        RecipeNutrientList = 13
        MenuNutrientList = 14
        MerchandisePriceList = 15

        '--- VRP 14.03.2008 For testing
        MerchandiseThumbnails = 16
        RecipeThumbnails = 17
        MenuThumbnails = 18
        '----

        MenuPlan = 19
    End Enum

    Public Enum enumPrintSubRecipesOptions
        None = 0
        All = 1
        FirstLevel = 2
    End Enum

    Public Enum enumDbaseTables
        Undefined = 0
        EgswBrand = 18
        EgswCategory = 19
        EgswKeyword = 43
        EgswLocation = 64
        EgswNutrientRules = 74
        EgswSetPrice = 110
        EgswSource = 117
        EgswSupplier = 120
        EgswTax = 126
        EgswUnit = 135
        EgswListe = 50
        EgswPrintProfile = 140
        EgswShoppingListDetail = 114
        EgswAllergen = 141
        EgswSite = 116
        EgswSalesItem = 103
        EgswUser = 136
        EgswIssuance = 40
        EgswClient = 22
        EgswSalesList = 105
        EgswTerminal = 128
        EgswClientContact = 143
        EgswSupplierContact = 144
        EgswSupplierGroup = 145
        EgswUploadConfig = 146
        EgswProcedureTemplate = 147 'VRP 24.03.2008
        EgswProcedureStyles = 149 'VRP 01.07.2008
        Placement = 150 ' JBB
        BransSite = 151 ' JBB 02.07.2011
        Project = 152
        TimeType = 153 'RDC 02.21.2013
    End Enum

    Public Enum enumFileNameOptions
        Name = 0
        Name_Filename = 1
        Filename_Name = 2
    End Enum

    Public Enum enumRequestType
        AddNew = UserRightsFunction.AllowCreate
        ChangeEntry = UserRightsFunction.AllowModify
        SubmitToSystem = UserRightsFunction.AllowSubmitToSystem
        changeBrand = UserRightsFunction.AllowMassChangeBrand
        changeCategory = UserRightsFunction.AllowMassChangeCategory
        changeSupplier = UserRightsFunction.AllowMassChangeSupplier
        changeSource = UserRightsFunction.AllowMassChangeSource
        Transfer = UserRightsFunction.AllowTransfer
        UpdateApprovedPrice = 1000
        replaceInMenu = 1001
        replaceInMenuCard = 1002
        replaceInRecipe = 1003
    End Enum
    Public Enum enumPrintGroupType
        None = 0
        Category = 1
        Supplier = 2
    End Enum

    Public Enum enumPrintSortType
        None = 0
        Name = 1
        Number = 2
        Dates = 3
        Category = 4
        Tax = 5
        Wastage = 6
        Price = 7
        Supplier = 8
        CostOfGoods = 9
        [Const] = 10
        SellingPrice = 11
        ImposedPrice = 12
        NetQty = 13
        GrossQty = 14
        Amount = 15
    End Enum

    Public Enum enumPrintUnits
        inch = 0
        centimeter = 1
        millimeter = 2
    End Enum

    Public Enum enumPrintStyle
        None = 0
        Standard = 1
        Modern = 2
        TwoColumns = 3
        EGSLayout = 4
        Standard2 = 5
        KolbsDesign = 6
        SVTest = 7
        CustomHero = 8
        EGSComposition = 9
        MigrosCustom = 10
    End Enum

    Public Enum enumPrintVariation
        None = 0
        SmallPicture_Quantity_Name = 1
        SmallPicture_Name_Quantity = 2
        MediumPicture_Name_Quantity = 12    '11  mcm 26.01.06 
        MediumPicture_Quantity_Name = 11    '12  mcm 26.01.06 
        LargePicture_Name_Quantity = 22     '21  mcm 26.01.06 
        LargePicture_Quantity_Name = 21     '22  mcm 26.01.06 
        Picture = 41
        Picture_Description = 42
    End Enum

    Public Enum enumApprovalStatus
        Undefined = -1
        Pending = 0
        Approved = 1
        Rejected = 2
        Cancelled = 3
    End Enum

    Public Enum enumPrintOptions
        None = 0
        MenuCosting = 1
        MenuDescription = 2
        MenuCostingAndDescription = 3
        MenuMealInfo = 4
        RecipePreparation = 5
        RecipeCosting = 6
        RecipeCostingAndPreparation = 7
        'MCM 10.02.06
        NutrientPerYieldUnit = 8
        NutrientPer100gOr100ml = 9
        NutrientBoth = 10
    End Enum

    Public Enum FilterSite As Int16
        'VBV 24.03.2006
        All = -1
        MySite = -2
        OtherSites = -3
    End Enum

    Public Enum FilterMark As Byte
        'VBV 14.12.2005
        All = 255
        UnMarked = 0
        Marked = 1
    End Enum

    Public Enum ApprovalModule
        'VBV 13.01.2006
        Undefined = 0
        Transfer = 5
        Requisition = 10
        PO = 11
    End Enum

    Public Enum enumItemStatus
        All = 255
        Active = 1
        Inactive = 2
    End Enum

    Public Enum enumMainSched
        SCHED_ONETIME = 1
        SCHED_DAILY = 2
        SCHED_WEEKLY = 3
        SCHED_MONTHLY = 4
        SCHED_POSCHANGES = 5
        SCHED_COMPUTERSTARTS = 6
        SCHED_PROGRAMSTARTS = 7
    End Enum

    Public Enum enumExportSalesItemFile As Byte
        TCPOS = 1
        Elvetino = 2
    End Enum

    Public Enum enumImportFileFormat
        Merchandise_Standard = 0
        Merchandise_Autogrill = 1
        Merchandise_Elvetino = 2
        Merchandise_LePatron = 3
        Merchandise_Manor = 4

        MANOR_rezept = 5
        MANOR_rezept_fremd = 6
        MANOR_rezept_wkl = 7
        MANOR_rezept_sup = 8



    End Enum

    Public Enum enumUploadFileType
        MERCHANDISE = 1
        SALES = 2
        IMAGE = 3
        BACKUP = 4
    End Enum

    Public Enum enumDate
        DateFrom = 0
        DateTo = 1
    End Enum

    Public Enum enumCost
        LASTPRICE = 0
        AVGPRICE = 1
    End Enum

    Public Enum enumFbFilter
        ALL = 0
        Marked = 1
    End Enum

    'mrc 06.01.09
    Public Enum enumProductViewMode
        Main = 1
        Details = 2
        Stock = 3
        SalesItem = 4
        Wine = 5
    End Enum

    '-- JBB 12.18.2011
    Public Enum enumUserRights
        Admin = 11
        Editor = 13
        Approver = 12
        Visitor = 14
    End Enum
    '--

    '// DRR 06.05.2012
    Public Enum enumFullTranslation
        Recipe = 8
        Ingredient = 2
        Unit = 1
    End Enum

    'AGL 2013.08.22 - DAM MediaType
    Public Enum enumDAMMediaType As Integer
        JPG = 1
        JPEG = 1
        PNG = 2
        EPS = 3
        BMP = 4
        TIFF = 5
        GIF = 6
    End Enum

    'MKAM 2014.07.02
    Public Enum enumAllergenFoodLaw
        EULaw = 1
        SwissLaw = 2
        Both = 3
    End Enum

#End Region

#Region "Class Objects"
    Public Class EatCHData
        Public Property restaurants As RestaurantModel()
    End Class

    Public Class RestaurantModel
        Public Property restaurants As String()
        Public Property menus As MenuModel()
    End Class

    Public Class MenuModel
        Public Property name As String
        Public Property reference As String
        Public Property type As String
        Public Property categories As CategoryModel()
    End Class

    Public Class CategoryModel
        Public Property name As String
        Public Property items As ItemModel()
    End Class

    Public Class ItemModel
        Public Property name As String
        Public Property description As String
        Public Property plu As String
        Public Property eatch_price As Integer  'RJPM 10.24.2023    Public Property price As Double
        Public Property ingredients As IngredientModel()
        Public Property dietary_restrictions As String()
        Public Property allergens As String()
        Public Property servings As Integer
    End Class

    Public Class IngredientModel
        Public Property name As String
    End Class

#End Region

End Module

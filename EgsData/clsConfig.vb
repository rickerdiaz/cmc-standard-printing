Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Threading
Imports System.Text
' Functions and sub procedures relating to EgswConfig Table (contains options)

Public Class clsConfig

    Inherits clsDBRoutine       ' Inherits common sql functions and procedures 

#Region "Variable Declarations / Dependencies"
    ' Declare fixed Ids that can be used in Code User Field of EgswConfig

    ' codegroup constants representsd the type of codeuser
    Public Const CONST_CODEGROUP_PRINTPROFILE As Integer = -4 ' The corresponding code user is the code of print profile
    Public Const CONST_CODEGROUP_SITE As Integer = -3      ' Outlet = Group = egswHotel
    Public Const CONST_CODEGROUP_PROPERTY As Integer = -2    ' Property = Main Group = EgswGroup
    Public Const CONST_CODEGROUP_USER As Integer = -1
    Public Const CONST_CODEGROUP_GLOBAL As Integer = 0

    ' codeuser constants represents default and global types
    Public Const CONST_CODEUSER_DEFAULT As Integer = -1
    Public Const CONST_CODEUSER_GLOBAL As Integer = 0

    Public Enum CodeGroup
        printprofile = -4
        site = -3
        property_ = -2
        user = -1
        [global] = 0
    End Enum

    Public Enum CodeUser
        default_ = -1
        [global] = 0
    End Enum

    ' Unqiue Ids used in EgswConfig Numero. These are options.
    ' RecipeNet Web should start from 200000
    Public Enum enumNumeros

        DaysSlowMoving = 14    ' - Generic
        DaysNonMoving = 15    ' - Generic
        ContactCompany = 100    ' - Generic
        ContactContact = 101    ' - Generic
        ContactAddress1 = 102    ' - Generic
        ContactAddress2 = 103    ' - Generic
        ContactCity = 104    ' - Generic
        ContactCountry = 105    ' - Generic
        ContactTel = 106    ' - Generic
        ContactFax = 107    ' - Generic
        ContactEmail = 108    ' - Generic
        Terms = 330    ' - Generic
        AccountingReceivable = 340    ' - Generic
        AccountingPayable = 341    ' - Generic
        AccountingExpenses = 342    ' - Generic
        AccountingSales = 343    ' - Generic
        AccountingPath = 345    ' - Generic
        IncomeAccount = 346    ' - Generic
        CostofgoodsAccount = 347    ' - Generic
        InventoryItemType = 348    ' - Generic
        AssetAccount = 349    ' - Generic
        AccountingSystem = 350    ' - Generic
        VatAccount = 351    ' - Generic
        POAutoNumber = 380    ' - Generic
        POPrefix = 381    ' - Generic
        PONextNumber = 382    ' - Generic
        POAutoKeepLength = 383    ' - Generic
        ProductAutoNumber = 390    ' - Generic
        ProductPrefix = 391    ' - Generic
        ProductNextNumber = 392    ' - Generic
        ProductAutoKeepLength = 393    ' - Generic
        AutoUpdateRnPrice = 400    ' - Generic
        ReqMultiSupplier = 410    ' - Generic
        POSAutoImport = 420    ' - Generic
        POSArchivePath = 430    ' - Generic
        EmailSMTP = 451    ' - Generic
        Scanner = 480    ' - Generic
        ScanPalPort = 481    ' - Generic
        ScanPalBaud = 482    ' - Generic
        POSOpeningTime = 492    ' - Generic
        ReqAutoNumber = 550    ' - Generic
        ReqPrefix = 551    ' - Generic
        ReqNextNumber = 552    ' - Generic
        ReqAutoKeepLength = 553    ' - Generic
        AvgPriceOption = 710    ' - Generic
        AvgPriceMonths = 711    ' - Generic
        AvgPriceDate = 712    ' - Generic
        AvgPriceType = 720    ' - Generic
        TransAutoNumber = 850    ' - Generic
        TransPrefix = 851    ' - Generic
        TransNextNumber = 852    ' - Generic
        TransAutoKeepLength = 853    ' - Generic
        TransDRAutoNumber = 900    ' - Generic
        TransDRPrefix = 901    ' - Generic
        TransDRNextNumber = 902    ' - Generic
        TransDRAutoKeepLength = 903    ' - Generic
        PriceForInvent = 910    ' - Generic
        EmailAddOption = 1011    ' - Generic
        EmailAddValue = 1012    ' - Generic
        ForceSupplierNo = 1020    ' - Generic
        DefaultCurrency = 3000    ' - Generic
        ShowCurrencyList = 3001    ' - Generic
        UnitType = 3002    ' - Generic
        FactorType = 3003    ' - Generic
        QtyDisplay = 3004    ' - Generic
        ShowLog = 3032    ' - Generic
        DefaultItemLanguageCode = 3034    ' - Generic
        DefaultMerchandiseTax = 3035    ' - Generic
        DefaultRecipeTax = 3036    ' - Generic
        DefaultMenuTax = 3037    ' - Generic
        DefaultRecipeYieldQty = 3038    ' - Generic
        DefaultRecipeYieldUnit = 3039    ' - Generic
        DefaultMenuYieldQty = 3040    ' - Generic
        DefaultRecipeCoeff = 3041    ' - Generic
        DefaultMenuCoeff = 3042    ' - Generic
        DefaultRecipePercent = 3043    ' - Generic
        DefaultMenuPercent = 3044    ' - Generic
        DictionaryLanguage = 3045    ' - Generic
        RemoveTrailing0 = 3046    ' - Generic
        ConvertToBestUnit = 3047    ' - Generic
        AutoLogin = 3048    ' - Generic
        NutrientDB = 3049    ' - Generic
        StrictEncoding = 3050    ' - Generic
        DefaultSetPriceSales = 3051    ' - Generic
        ShowUsername = 4050    ' - Generic
        ShowProductList_0 = 4100    ' - Generic
        ShowProductList_1 = 4101    ' - Generic
        ShowProductList_2 = 4102    ' - Generic
        ShowProductList_3 = 4103    ' - Generic
        ShowProductList_4 = 4104    ' - Generic
        ShowProductList_5 = 4105    ' - Generic
        ShowProductList_6 = 4106    ' - Generic
        ShowProductListStock_0 = 4120    ' - Generic
        ShowProductListStock_1 = 4121    ' - Generic
        ShowProductListStock_2 = 4122    ' - Generic
        ShowProductListStock_3 = 4123    ' - Generic
        ShowProductListStock_4 = 4124    ' - Generic
        ReqPO_0 = 4180    ' - Generic
        ReqPO_1 = 4181    ' - Generic
        Receiving_0 = 4190    ' - Generic
        Receiving_1 = 4191    ' - Generic
        Receiving_2 = 4192    ' - Generic
        Receiving_3 = 4193    ' - Generic
        Receiving_4 = 4194    ' - Generic
        Receiving_5 = 4195    ' - Generic
        ShowInventory_0 = 4200    ' - Generic
        ShowInputOutputField_0 = 4210    ' - Generic
        ShowInputOutputField_1 = 4211    ' - Generic
        ShowInputOutputField_2 = 4212    ' - Generic
        ShowInputOutputField_3 = 4213    ' - Generic
        ShowHistoryField_0 = 4220    ' - Generic
        ShowHistoryField_1 = 4221    ' - Generic
        ShowHistoryField_2 = 4222    ' - Generic
        ShowHistoryField_3 = 4223    ' - Generic
        ProgramLanguage = 4300    ' - Generic
        HelpLanguage = 4301    ' - Generic
        ColorBackColor = 4400    ' - Generic
        ColorBackColorAlternate = 4401    ' - Generic
        ColorBackColorSel = 4402    ' - Generic
        ColorForeColor = 4403    ' - Generic
        ColorGridColor = 4404    ' - Generic
        ColorBackColorBkg = 4405    ' - Generic
        ColorEditableColumn = 4406    ' - Generic
        ColorBackPickList = 4408    ' - Generic
        ColorBackInfoList = 4409    ' - Generic
        GridLines = 4411    ' - Generic
        ColorHighlightText = 4415    ' - Generic
        ShowWallPaper = 4425    ' - Generic
        GridPicture = 4430    ' - Generic
        ShowDisabledFunctions = 4510    ' - Generic
        ShowAnimation = 4511    ' - Generic
        HelpShowMode = 4730    ' - Generic
        HelpMDIMode = 4731    ' - Generic
        PriceWidthFactor = 5160    ' - Generic
        ImportType = 12500    ' - Generic
        ImportShowItem = 12501    ' - Generic
        ImportShowOption = 12502    ' - Generic
        ImportCompareBy = 12503    ' - Generic
        ImportAddNew = 12504    ' - Generic
        ImportUpdate = 12505    ' - Generic
        ImportUpdateType = 12506    ' - Generic
        ImportSuppNetwork = 12508    ' - Generic
        ImportAutoLinkItems = 12509    ' - Generic
        ImportNewSupplier = 12512    ' - Generic
        Pages_UsePage = 12600    ' - Generic
        Pages_NumberOfRowsMin = 12601    ' - Generic
        Pages_NumberOfRowsMax = 12602    ' - Generic
        Pages_NumberOfPageMax = 12603    ' - Generic
        Pages_NoMaximum = 12604    ' - Generic
        ShowStructure = 15000    ' - Generic
        ShowGrid = 15001    ' - Generic
        ShowIngredientPrep = 15004    ' - Generic
        ExpandSubRecipe = 15005    ' - Generic
        PlaySounds = 15006    ' - Generic
        ShowItemNo = 15007    ' - Generic
        PrListLeftMargin = 15011    ' - Generic
        PrListRightMargin = 15012    ' - Generic
        PrListTopMargin = 15013    ' - Generic
        PrListBottomMargin = 15014    ' - Generic
        PrListFont = 15015    ' - Generic
        PrListFontSize = 15016    ' - Generic
        PrListLineSpacing = 15017    ' - Generic
        PrCardLeftMargin = 15018    ' - Generic
        PrCardRightMargin = 15019    ' - Generic
        PrCardTopMargin = 15020    ' - Generic
        PrCardBottomMargin = 15021    ' - Generic
        PrCardFont1 = 15022    ' - Generic
        PrCardFontSize1 = 15023    ' - Generic
        PrCardFont2 = 15024    ' - Generic
        PrCardFontSize2 = 15025    ' - Generic
        PrRole = 15026    ' - Generic
        PrCardLineSpacing = 15027    ' - Generic
        PrCardTextItem = 15028    ' - Generic
        PrIncludeCookingTip = 15029    ' - Generic
        PrIncludeIngredientPreparation = 15030    ' - Generic
        PrIncludeGDA = 15031    ' - Generic
        Per1Yield = 15032    ' - Generic
        AutoRecalculate = 15033    ' - Generic
        PrFontTitle = 15034    ' - Generic
        PrFontTitleSize = 15035    ' - Generic
        SMTPUserName = 20000    ' - Generic
        SMTPPassword = 20001    ' - Generic
        SMTPServer = 20002    ' - Generic
        SMTPPort = 20003    ' - Generic
        SMTPSendUsing = 20004    ' - Generic
        SMTPEmailSender = 20005    ' - Generic
        FTSEnable = 20006    ' - Generic
        FTSIncrementPopulationInterval = 20007    ' - Generic
        FTSLanguage = 20008    ' - Generic
        ImageQuality = 20009    ' - Generic
        ImageSize = 20010    ' - Generic
        ImageImposedSize = 20011    ' - Generic
        SecurityIPLimitAttempts = 20012    ' - Generic
        SecurityIPLimitMaximumChoice = 20013    ' - Generic
        SecurityIPLimitMinimumChoice = 20014    ' - Generic
        SecurityUsernameLimitAttempts = 20015    ' - Generic
        UICodeLang = 20016    ' - Generic
        UICodeMainListeLang = 20017    ' - Generic
        UIPageSize = 20018    ' - Generic
        UIMainSetOfPrice = 20019    ' - Generic
        UIListeDisplay = 20020    ' - Generic
        UICodeTimezone = 20021    ' - Generic
        UIUseBestUnit = 20022    ' - Generic
        NutrientDatabase = 20023    ' - Generic
        UISiteName = 20024    ' - Generic
        UISiteThemeFolder = 20025    ' - Generic
        UISiteLogoFolder = 20026    ' - Generic
        UISiteFooterLogoFolder = 20027    ' - Generic
        EmailThreadIsBackground = 20029    ' - Generic
        EmailThreadPriority = 20030    ' - Generic
        SecurityUsernameMinChar = 20031    ' - Generic
        SecurityPasswordMinChar = 20032    ' - Generic
        UITextDisplayLengthLimit = 20033    ' - Generic
        RXDirectoryServer = 20034    ' - Generic
        RXDirectoryClient = 20035    ' - Generic
        EmailThreadSleepInterval = 20036    ' - Generic
        UISearchDefaultFilter = 20037    ' - Generic
        SecuritySSLConnection = 20038    ' - Generic
        UIHomePageBrowseListTableForMerchandise = 20040    ' - Generic
        UIHomePageBrowseListTableForRecipe = 20041    ' - Generic
        UIHomePageBrowseListTableForMenu = 20042    ' - Generic
        ImageSizeWidth = 20043    ' - Generic
        ImageSizeHeight = 20044    ' - Generic
        SendErrorMail = 20045    ' - Generic
        PrIncludeNumber = 20047    ' - Generic
        PrIncludeWastage = 20048    ' - Generic
        PrIncludePrice = 20049    ' - Generic
        PrIncludeTax = 20050    ' - Generic
        PrIncludeDate = 20051    ' - Generic
        PrIncludePicture = 20052    ' - Generic
        PrIncludeInfo = 20053    ' - Generic
        PrIncludeNutrient = 20054    ' - Generic
        PrIncludeKeyword = 20055    ' - Generic
        PrIncludeConst = 20056    ' - Generic
        PrIncludeSellingPrice = 20057    ' - Generic
        PrIncludeImposedPrice = 20058    ' - Generic
        PrIncludeCategory = 20059    ' - Generic
        PrIncludeSource = 20060    ' - Generic
        PrIncludeRemark = 20061    ' - Generic
        PrIncludeWeight = 20062    ' - Generic
        PrIncludePictureFirst = 20063    ' - Generic
        PrIncludePictureAll = 20064    ' - Generic
        PrIncludeLogo_Background = 20065    ' - Generic
        PrIncludeTextIn = 20066    ' - Generic
        PrIncludeBoldTextIngredient = 20067    ' - Generic
        PrIncludeIngredientNumber = 20068    ' - Generic
        PrIncludeStructure = 20069    ' - Generic
        PrIncludeWeightPercentage = 20070    ' - Generic
        PrIncludeGrossQty = 20071    ' - Generic
        PrIncludeNetQty = 20072    ' - Generic
        PrIncludeSubRecipeInIngredients = 20073    ' - Generic
        PrIncludeKeywords = 20074    ' - Generic
        PrIncludeProcedure = 20075    ' - Generic
        PrIncludeSubRecipeDetails = 20076    ' - Generic
        PrIncludeHACCP = 20077    ' - Generic
        PrIncludeCostOfGoods = 20078    ' - Generic
        PrSort = 20079    ' - Generic
        PrPaperSize = 20080    ' - Generic
        PrMarginUnit = 20082    ' - Generic
        PrStyle = 20083    ' - Generic
        PrDocumentOuput = 20084    ' - Generic
        PrShowNewPageIfDifferentSupplier = 20085    ' - Generic
        PrTranslation = 20086    ' - Generic
        PrSetOfPrice = 20087    ' - Generic
        PrYieldDefault = 20090    ' - Generic
        PrSubRecipes = 20091    ' - Generic
        PrPrintOptions = 20092    ' - Generic
        PrPrintVariation = 20093    ' - Generic
        PrPaperPageWidth = 20094    ' - Generic
        PrPaperPageHeight = 20095    ' - Generic
        prIncludeFactor = 20096    ' - Generic
        prIncludeAllergens = 20097    ' - Generic
        CSVImportOptRemoveFile = 20098    ' - Generic
        CSVImportOptArchivePath = 20099    ' - Generic
        CSVImportOptAddNewRec = 20100    ' - Generic
        CSVImportOptUpdateExistingRec = 20101    ' - Generic
        CSVImportOptUpdateBasedOnDate = 20102    ' - Generic
        CSVImportOptCompareByName = 20103    ' - Generic
        CSVImportOptDeleteUnused = 20104    ' - Generic
        CSVImportOptFieldSep = 20105    ' - Generic
        CSVImportOptThousandSep = 20106    ' - Generic
        CSVImportOptDecimalSep = 20107    ' - Generic
        CSVImportOptCodeSite = 20108    ' - Generic
        CSVImportOptCodeSetPrice = 20109    ' - Generic
        CSVImportOptbitUse = 20110    ' - Generic
        CSVImportOptCodeTrans = 20111    ' - Generic
        CSVImportOptCodeUser = 20112    ' - Generic
        CSVImportOptImportFile = 20113    ' - Generic
        TCPOSExportTime = 20114    ' - Generic
        TCPOSExportPath = 20115    ' - Generic
        TCPOSExportFileName = 20116    ' - Generic
        TCPOSExportOptions = 20117    ' - Generic
        TCPOSExportLastExportedDate = 20118    ' - Generic
        CSVImportOptGlobal = 20119    ' - Generic
        TCPOSExportSetPrice = 20120    ' - Generic
        FTSDefaultSearch = 20121    ' - Generic
        UIListItemColor = 20122    ' - Generic
        BackupDbasePath = 20123    ' - Generic
        BackupPicturePath = 20124    ' - Generic
        DefaultMerchandiseGlobal = 20125    ' - Generic
        DefaultMerchandisePrintProfile = 20126    ' - Generic
        DefaultRecipePrintProfile = 20127    ' - Generic
        DefaultMenuPrintProfile = 20128    ' - Generic
        DefaultMerchandiseListPrintProfile = 20129    ' - Generic
        DefaultRecipePrintListProfile = 20130    ' - Generic
        DefaultMenuPrintListProfile = 20131    ' - Generic
        UIRemoveTrailingZeros = 20132    ' - Generic
        PrIncludePrice2 = 20133    ' - Generic
        PrIncludeSupplier = 20134    ' - Generic
        UIPrintOutput = 20135    ' - Generic
        ElvetinoExportShoppingSource = 20136    ' - Generic
        ElvetinoExportShoppingOutput = 20137    ' - Generic
        ElvetinoExportShoppingUser = 20138    ' - Generic
        ElvetinoExportShoppingSetPrice = 20139    ' - Generic
        MainPurchasingSetofPrice = 20140    ' - Generic
        CSVImportFileType = 20141    ' - Generic
        CSVImportSched = 20143    ' - Generic
        CSVImportTime = 20144    ' - Generic
        PrIncludePictureRight = 20146    ' - Generic
        PrPictureOptions = 20147    ' - Generic
        PrIncludeDerivedKeyword = 20148    ' - Generic
        LastExportDate = 20150    ' - Generic
        UISiteFooterAddress = 20151    ' - Generic
        DefaultProcedureText = 20152    ' - Generic
        UIDisplayMerchandiseListPrice = 20153    ' - Generic
        UIDisplayMerchandiseListNutrient = 20154    ' - Generic
        UIDisplayRecipeListPrice = 20155    ' - Generic
        UIDisplayRecipeListPriceOption = 20156    ' - Generic
        UIDisplayRecipeListNutrient = 20157    ' - Generic
        UIDisplayRecipeDetailsSubRecipe = 20158    ' - Generic
        UIDisplayRecipeDetailsNutrient_Servings = 20159    ' - Generic
        UIDisplayRecipeDetailsNutrient_Per100 = 20160    ' - Generic
        UIDisplayMenuListPrice = 20161    ' - Generic
        UIDisplayMenuListPriceOption = 20162    ' - Generic
        UIDisplayMenuListNutrient = 20163    ' - Generic
        UIDisplayMerchandiseTab = 20164    ' - Generic
        UIDisplayRecipeTab = 20165    ' - Generic
        UIDisplayMenuTab = 20166    ' - Generic
        UIDisplayMerchandiseListCategory = 20167    ' - Generic
        UIDisplayRecipeListCategory = 20168    ' - Generic
        UIDisplayMenuListCategory = 20169    ' - Generic
        UIDisplayRecipeDetailsNutrient = 20170    ' - Generic
        UIDisplayRecipeDetailsPrice = 20171    ' - Generic
        UIDisplayMerchandiseDetailsNutrient = 20172    ' - Generic
        UIDisplayMerchandiseDetailsPrice = 20173    ' - Generic
        UIDisplayMenuDetailsNutrient = 20174    ' - Generic
        UIDisplayMenuDetailsPrice = 20175    ' - Generic
        UIDisplayMerchandiseDetailsProduct = 20176    ' - Generic
        UIProductCalculationType = 20177    ' - Generic
        UIRecipeIngredientsDefaultView = 20178    ' - Generic
        UIDisplayRecipeDetailsComposition = 20179    ' - Generic
        UIDisplayMerchandiseListSupplier = 20189    ' - Generic
        UIDisplayMerchandiseListSite = 20190    ' - Generic
        UIDisplayRecipeListSite = 20191    ' - Generic
        UIDisplayMenuListSite = 20192    ' - Generic
        UIDisplayRemoveTrailingZeros = 20193    ' - Generic
        UIDisplayRecipeListSource = 20194    ' - Generic
        UIAutoCalculateIngredients = 20195    ' - Generic
        UIProtectionWorkWithProtectedCopies = 20196    ' - Generic
        UIProtectionIncludeWhenPrintingAndExporting = 20197    ' - Generic
        DefaultMenuPlanCoeff = 20198    ' - Generic
        UIDisplayMerchandiseListDate = 20199    ' - Generic
        UIDisplayRecipeListDate = 20200    ' - Generic
        UIDisplayMenuListDate = 20201    ' - Generic
        UIRemoveBreadcrumbs = 20202    ' - Generic
        UIPrintingLabelPrinter = 20203    ' - Generic
        UIDisplayTranslatePercent = 20204    ' - Generic
        CSVImportCodeClientSCANA = 20205    ' - Generic
        CSVImportCodeClientPistor = 20206    ' - Generic
        CSVImportCodeClientSN = 20207    ' - Generic
        CSVImportSupplierNetwork = 20208    ' - Generic
        INVProductPrices = 20209    ' - Generic
        INVDefaultSupplierPrice = 20210    ' - Generic
        UIDisplayRecipeDetailsCostingPerYield = 20211    ' - Generic
        UIDisplayRecipeDetailsCostingTotalYield = 20212    ' - Generic
        UIDisplayMenuDetailsCostingPerYield = 20213    ' - Generic
        UIDisplayMenuDetailsCostingTotalYield = 20214    ' - Generic
        DefaultMenuPlanYield = 20215    ' - Generic
        EnableApprovalEmail = 20216    ' - Generic
        UIDisplayProductMainProductNumber = 20217    ' - Generic
        UIDisplayProductMainDefaultSupplier = 20218    ' - Generic
        UIDisplayProductMainStockPrice = 20219    ' - Generic
        UIDisplayProductMainDateModified = 20220    ' - Generic
        UIDisplayProductDetailProductNumber = 20221    ' - Generic
        UIDisplayProductDetailDefaultSupplier = 20222    ' - Generic
        UIDisplayProductDetailTransPrice = 20223    ' - Generic
        UIDisplayProductDetailPackPrice = 20224    ' - Generic
        UIDisplayProductDetailStockPrice = 20225    ' - Generic
        UIDisplayProductDetailRecipePrice = 20226    ' - Generic
        UIDisplayProductStockProductNumber = 20227    ' - Generic
        UIDisplayProductStockDefaultSupplier = 20228    ' - Generic
        UIDisplayProductStockQtyOnHand = 20229    ' - Generic
        UIDisplayProductStockQtyMax = 20230    ' - Generic
        UIDisplayProductStockQtyMin = 20231    ' - Generic
        UIDisplayProductStockStockPrice = 20232    ' - Generic
        UIDisplayProductStockRecipePrice = 20233    ' - Generic
        UIDisplayProductSalesProductNumber = 20234    ' - Generic
        UIDisplayProductSalesSalesItemNumber = 20235    ' - Generic
        UIDisplayProductSalesSellingPrice = 20236    ' - Generic
        UIDisplayProductSalesTax = 20237    ' - Generic
        UIDisplayProductSalesSellingPriceTax = 20238    ' - Generic
        UIDisplayProductWineProductNumber = 20239    ' - Generic
        UIDisplayProductWineStockPrice = 20240    ' - Generic
        UIDisplayProductWineCountry = 20241    ' - Generic
        UIDisplayProductWineProducer = 20242    ' - Generic
        UIDisplayProductWineWineType = 20243    ' - Generic
        UIDisplayProductWineSize = 20244    ' - Generic
        UIDisplayProductWineAlcohol = 20245    ' - Generic
        UIDisplayProductWineVintage = 20246    ' - Generic
        PrListYieldOption = 20247    ' - Generic
        UIDisplayRecipeImposedPrice = 20251    ' - Generic
        UIDisplayRecipeNetMarginPercent = 20252    ' - Generic
        UIDisplayMenuImposedPrice = 20253    ' - Generic
        UIDisplayMenuNetMarginPercent = 20254    ' - Generic
        NetMarginPercentMinimumValue = 20255    ' - Generic
        NetMarginPercentMaximumValue = 20256    ' - Generic
        PrShowFoodcostOnly = 20257    ' - Generic
        PrIncludeGDAImage = 20258    ' - Generic
        UIDisplayRecipeDetailsPlacement = 20259    ' - Generic
        UIDisplayRecipeDetailsKeyword = 20260    ' - Generic
        UIDisplayRecipeDetailsProject = 20261    ' - Generic
        UIDisplayRecipeDetailsAllergen = 20262    ' - Generic
        UIDisplayRecipeDetailsFileUpload = 20263    ' - Generic
        PreviewUseMetric = 20264    ' - Generic
        PreviewUseImperial = 20265    ' - Generic
        PreviewUseDescription = 20266    ' - Generic
        TwoPicturesInRecipe = 20268    ' - Generic
        DisplaySource = 20269    ' - Generic
        PrintDetailsUseMetric = 20270    ' - Generic
        PrintDetailsUseImperial = 20271    ' - Generic
        IsIncludeSub = 20272    ' - Generic
        SortBy = 20273    ' - Generic
        PrIncludeIngredientComplement = 20274    ' - Generic
        PrIncludeTimes = 20275    ' - Generic
        PrIncludeSubtitle = 20276    ' - Generic
        PrIncludeServeWith = 20277    ' - Generic
        PrIncludeFootNotes = 20278    ' - Generic
        PrIncludeBrands = 20279    ' - Generic
        PrIncludePublication = 20280    ' - Generic
        PrNutrientNumber = 20280    ' - Generic
        PrIncludePlacements = 20281    ' - Generic
        PrIncludeProcSeqNumber = 20282    ' - Generic
        PrIncludeRecipeNotes = 20283    ' - Generic
        PrIncludeFoodCostInPercent = 20284    ' - Generic
        UseImposedPriceForSubRecipe = 20295    ' - Generic
        UIDisplayMerchandiseListBrand = 20296    ' - Generic
        UIDisplayMerchandiseListTax = 20297    ' - Generic
        UIDisplayMerchandiseListStatus = 20298    ' - Generic
        UIListBehavior = 20299    ' - Generic
        MPDelifitEnergyKCalConditon = 20300    ' - Generic
        MPDelifitFatConditon = 20301    ' - Generic
        DisplayImposedPriceWOTaxForRecipeList = 20302    ' - Generic
        DisplayImposedPriceWTaxForRecipeList = 20303    ' - Generic
        PrNutrientList = 20381    ' - Generic
        StockInfoPopUp = 20382    ' - Generic
        CSVImportOptLogFile = 20383    ' - Generic
        CSVImportFileFolder = 20384    ' - Generic
        UseSubtitle = 20385    ' - Generic
        RecipePreviewInMultipleWindows = 20386    ' - Generic
        NutrientEnergyKCALFormat = 20387    ' - Generic
        OneOrMetricImperial = 20387    ' - Generic
        UIDisplayQuantitiesAsFractions = 20388    ' - Generic
        PrIncludeMetricQtyGross = 20389    ' - Generic
        PrIncludeMetricQtyNet = 20390    ' - Generic
        PrIncludeImperialQtyGross = 20391    ' - Generic
        PrIncludeImperialQtyNet = 20392    ' - Generic
        PrIncludeAlternativeIngredient = 20393    ' - Generic
        PrIncludeHighlightSection = 20394    ' - Generic
        PrIncludeDescription = 20395    ' - Generic
        PrIncludeAddtionalNotes = 20396    ' - Generic
        PrIncludeCookbook = 20397    ' - Generic
        PrIncludeKiosk = 20398    ' - Generic
        PrIncludeComment = 20399    ' - Generic
        UIAutomate = 20400    ' - Generic
        PrIncludeRecipeStatus = 20401    ' - Generic
        PrIncludeNutrientSet = 20402    ' - Generic
        OneOrMetricImperialNutrientCalculationBasis = 20403    ' - Generic
        PrIncludeExpGDA = 20403    ' - Generic
        OneOrMetricImperialQuantityBasis = 20404    ' - Generic
        ConversionType = 20405    ' - Generic
        UISearchListColumnFilters = 202680    ' - Generic
        UIMoreActionsMerchandiseCopy = 202681    ' - Generic
        UIMoreActionsMerchandiseDelete = 202682    ' - Generic
        UIMoreActionsMerchandiseEdit = 202683    ' - Generic
        UIMoreActionsMerchandiseMoveMarkedItems = 202684    ' - Generic
        UIMoreActionsMerchandiseMoveMarkedItemsKeywords = 202685    ' - Generic
        UIMoreActionsMerchandiseMoveMarkedItemsBrand = 202686    ' - Generic
        UIMoreActionsMerchandiseMoveMarkedItemsCategory = 202687    ' - Generic
        UIMoreActionsMerchandisePrintOrExport = 202688    ' - Generic
        UIMoreActionsMerchandisePrintOrExportPrintDetails = 202689    ' - Generic
        UIMoreActionsMerchandisePrintOrExportPrintList = 202690    ' - Generic
        UIMoreActionsMerchandisePrintOrExportPrintNutrient = 202691    ' - Generic
        UIMoreActionsMerchandisePrintOrExportPrintPrice = 202692    ' - Generic
        UIMoreActionsMerchandisePrintOrExportExport = 202693    ' - Generic
        UIMoreActionsMerchandiseReplace = 202694    ' - Generic
        UIMoreActionsMerchandiseSaveMarkedAs = 202695    ' - Generic
        UIMoreActionsMerchandiseSharing = 202696    ' - Generic
        UIMoreActionsMerchandiseSharingUnexpose = 202697    ' - Generic
        UIMoreActionsMerchandiseSharingExpose = 202698    ' - Generic
        UIMoreActionsMerchandiseSharingSharing = 202699    ' - Generic
        UIMoreActionsMerchandiseMoveMarkedItemSupplier = 202700    ' - Generic
        UIMoreActionsRecipeCompare = 202701    ' - Generic
        UIMoreActionsRecipeCopy = 202702    ' - Generic
        UIMoreActionsRecipeDelete = 202703    ' - Generic
        UIMoreActionsRecipeEdit = 202704    ' - Generic
        UIMoreActionsRecipeMoveMarkedItems = 202705    ' - Generic
        UIMoreActionsRecipeMoveMarkedItemsKiosk = 202706    ' - Generic
        UIMoreActionsRecipeMoveMarkedItemsPublication = 202707    ' - Generic
        UIMoreActionsRecipeMoveMarkedItemsCookbook = 202708    ' - Generic
        UIMoreActionsRecipeMoveMarkedItemsKeywords = 202709    ' - Generic
        UIMoreActionsRecipeMoveMarkedItemsCategory = 202710    ' - Generic
        UIMoreActionsRecipeMoveMarkedItemsRatings = 202711    ' - Generic
        UIMoreActionsRecipeMoveMarkedItemsRecipeStatus = 202712    ' - Generic
        UIMoreActionsRecipeMoveMarkedItemsSource = 202713    ' - Generic
        UIMoreActionsRecipePrintOrExport = 202714    ' - Generic
        UIMoreActionsRecipePrintOrExportDetails = 202715    ' - Generic
        UIMoreActionsRecipePrintOrExportList = 202716    ' - Generic
        UIMoreActionsRecipePrintOrExportNutrient = 202717    ' - Generic
        UIMoreActionsRecipePrintOrExportExport = 202718    ' - Generic
        UIMoreActionsRecipePrintOrExportCSV = 202719    ' - Generic
        UIMoreActionsRecipePrintOrExportExcel = 202720    ' - Generic
        UIMoreActionsRecipePrintOrExportWord = 202721    ' - Generic
        UIMoreActionsRecipeShoppingList = 202722    ' - Generic
        UIMoreActionsRecipeReplaceIngredient = 202723    ' - Generic
        UIMoreActionsRecipeSaveMarkedAs = 202724    ' - Generic
        UIMoreActionsRecipeSharing = 202725    ' - Generic
        UIMoreActionsRecipeSharingUnexpose = 202726    ' - Generic
        UIMoreActionsRecipeSharingExpose = 202727    ' - Generic
        UIMoreActionsRecipeSharingSharing = 202728    ' - Generic
        UIMoreActionsRecipeSharingTransfer = 202729    ' - Generic
        UIMoreActionsRecipeVersion = 202730    ' - Generic
        NutrientEnergyDisplayKJorKCAL = 202731    ' - Generic
        UIDisplayRecipeListName = 202732    ' - Generic
        UIDisplayRecipeListNumber = 202733    ' - Generic
        UIDisplayRecipeListBrand = 202734    ' - Generic
        UIDisplayRecipeListWorkflowStatus = 202735    ' - Generic
        UIDisplayRecipeListImage = 202736    ' - Generic
        UIDisplayRecipeListSharingStatus = 202737    ' - Generic
        UIMoreActionsMenuCompare = 202738    ' - Generic
        UIMoreActionsMenuCopy = 202739    ' - Generic
        UIMoreActionsMenuDelete = 202740    ' - Generic
        UIMoreActionsMenuEdit = 202741    ' - Generic
        UIMoreActionsMenuMoveMarkedItems = 202742    ' - Generic
        UIMoreActionsMenuMoveMarkedItemsKiosk = 202743    ' - Generic
        UIMoreActionsMenuMoveMarkedItemsPublication = 202744    ' - Generic
        UIMoreActionsMenuMoveMarkedItemsCookbook = 202745    ' - Generic
        UIMoreActionsMenuMoveMarkedItemsKeywords = 202746    ' - Generic
        UIMoreActionsMenuMoveMarkedItemsCategory = 202747    ' - Generic
        UIMoreActionsMenuMoveMarkedItemsRatings = 202748    ' - Generic
        UIMoreActionsMenuMoveMarkedItemsMenuStatus = 202749    ' - Generic
        UIMoreActionsMenuMoveMarkedItemsSource = 202750    ' - Generic
        UIMoreActionsMenuPrintOrExport = 202751    ' - Generic
        UIMoreActionsMenuPrintOrExportDetails = 202752    ' - Generic
        UIMoreActionsMenuPrintOrExportList = 202753    ' - Generic
        UIMoreActionsMenuPrintOrExportNutrient = 202754    ' - Generic
        UIMoreActionsMenuPrintOrExportExport = 202755    ' - Generic
        UIMoreActionsMenuPrintOrExportCSV = 202756    ' - Generic
        UIMoreActionsMenuPrintOrExportExcel = 202757    ' - Generic
        UIMoreActionsMenuPrintOrExportWord = 202758    ' - Generic
        UIMoreActionsMenuPrintOrExportShoppingList = 202759    ' - Generic
        UIMoreActionsMenuReplaceIngredient = 202760    ' - Generic
        UIMoreActionsMenuSaveMarkedAs = 202761    ' - Generic
        UIMoreActionsMenuSharing = 202762    ' - Generic
        UIMoreActionsMenuSharingUnexpose = 202763    ' - Generic
        UIMoreActionsMenuSharingExpose = 202764    ' - Generic
        UIMoreActionsMenuSharingSharing = 202765    ' - Generic
        UIMoreActionsMenuSharingTransfer = 202766    ' - Generic
        UIMoreActionsMenuVersion = 202767    ' - Generic
        UIDisplayRecipeListSecondaryBrand = 2026730    ' - Generic
        PrIncludeYield1 = 2026731    ' - Generic
        PrIncludeYield2 = 2026732    ' - Generic
        AllergenFoodLaw = 2026733    ' - COOP Gastronomie
        UISearchOption = 2026734    ' - Generic
        PasswordLoginEnforceStrongPolicy = 2026735    ' - Generic
        PasswordLoginExpiresAfter = 2026736    ' - Generic
        PasswordLoginMinimumLength = 2026737    ' - Generic
        PasswordLoginMinimumForReuse = 2026738    ' - Generic
        PasswordLoginMaximumFailedAttempts = 2026739    ' - Generic
        PasswordLoginLockoutPeriod = 2026740    ' - Generic
        DisplayProcedureTemplate = 2026741    ' - Generic
        UIDisplayMerchandiseListImage = 2026742    ' - Generic
        EULaw = 2026743    ' - Generic
        EULawBold = 2026744    ' - COOP Gastronomie
        SWLaw = 2026745    ' - Generic
        SWLawBold = 2026746    ' - COOP Gastronomie
        GenericLaw = 2026747    ' - COOP Gastronomie
        GenericLawOption = 2026748    ' - COOP Gastronomie
        GenericDisplayWeightPercentage = 2026749    ' - COOP Gastronomie
        UIMoreActionsMerchandiseMoveMarkedItemAssignUnassignAllergen = 2026750    ' - Generic
        EULawDisplayIng = 2026751    ' - COOP Gastronomie
        EULawDisplayWeightPercentage = 2026752    ' - COOP Gastronomie
        ClientHotline = 2026753    ' - Generic
        PrintPosition = 2026754    ' - Generic
        PrintName = 2026755    ' - Generic
        PrintApproveBy = 2026756    ' - Generic
        PrintApproveByName = 2026757    ' - Generic
        PrintCountrySide = 2026758    ' - Generic
        PrintKeyword = 2026759    ' - Generic
        PrintAllergens = 2026760    ' - Generic
        PrintFooter = 2026761    ' - Generic
        PrintSubtitle = 2026762    ' - Generic
        AutoSearch = 2026763    ' - Generic
        AllowInactiveIngredients = 2026764    ' - Generic
        ComplementBeforeIngredient = 2026765    ' - Generic
        ComplementAfterIngredient = 2026766    ' - Generic
        MenuPlanCalculateByPortion = 2026767    ' - Generic
        UseCodeOrNumber = 2026768    ' - Generic
        UIDisplayMenuListSource = 2026769    ' - Generic
        ShareSubrecipe = 2026770    ' - Generic
        CalculateSubRecipePrice = 2026771    ' - Generic
        CianoReportLang1 = 2026772    ' - Generic
        CianoReportLang2 = 2026773    ' - Generic
        DefaultMenuPlanTax = 2026774    ' - Generic
        GrossMargin = 2026774    ' - COOP VK
        PrIncludeRecipeListName = 2026775    ' - COOP VK
        DefaultMenuPlanSource = 2026775    ' - Generic
        UIMoreActionsRecipeChangeImposedPrice = 2026776    ' - Generic
        UIDisplayRecipeDetailsProcedure = 2026777    ' - Generic
        PrIncludeRecipeGrossMargin = 2026778    ' - Generic
        UIMoreActionNiceLabel = 2026779    ' - Generic
        KioskOptionShowRecipe = 2026780    ' - Generic
        KioskRecipeDisplay = 2026780    ' - Generic
        MenuPlanOpenCycleOnFirst = 2026782    ' - Generic
        MenuimposedWOTax = 2026783    ' - Generic
        MenuimposedWTax = 2026784    ' - Generic
        MenuSharing = 2026785    ' - Generic
        UseImposedCompositionOnly = 2026786    ' - Generic
        MenuPlanRowsPerPage = 2026787    ' - Migros
        MenuPlanSelectedCategory = 2026788    ' - Migros
        ShowOnlyCompleteSapArticle = 2026789    ' - Migros
        MenuPlanSAPOnly = 2026790    ' - Migros
        UIDisplayMenuPlanSAPOnly = 2026791    ' - Migros
        mgrsIncludeRecipeName = 3100003    ' - Migros
        mgrsIncludeLastUsedInMenuplan = 3110003    ' - Migros
        mgrsIncludeCostInCHF = 3120003    ' - Migros
        mgrsIncludeCostInPercent = 3130003    ' - Migros
        mgrsIncludeGrossMarginInPercent = 3140003    ' - Migros
        mgrsIncludeGrossMarginInCHF = 3150003    ' - Migros
        mgrsIncludeNetMarginInCHF = 3160003    ' - Migros
        mgrsIncludeNetMarginInPercent = 3170003    ' - Migros
        mgrsIncludeVatInPercent = 3180003    ' - Migros
        mgrsIncludeVPinclCostsAndVat = 3190003    ' - Migros
        mgrsIncludeRecipeNo = 3200003    ' - Migros
        mgrsIncludeFactoreSet = 3210003    ' - Migros
        mgrsIncludeKeyword = 3220003    ' - Migros
        mgrsPrintBanner = 3230003    ' - Migros
        UIDisplayMerchandiseListWeight = 3240003    ' - Migros
        mgrsIncludeRecipeID = 3300003    ' - Migros
        mgrsIncludeCategory = 3400003    ' - Migros
        mgrsIncludeOwner = 3500003    ' - Migros
        mgrsIncludeSource = 3600003    ' - Migros
        mgrsIncludeDateCreated = 3700003    ' - Migros
        mgrsIncludeRecipeStatus = 3800003    ' - Migros
        mgrsIncludeLastModifiedDate = 3900003    ' - Migros
        RecipeDefaultNutrientShow = 20267000    ' - Generic
        AllowMonday = 202670002    ' - Generic
        RecipeKeywordEditCollapse = 202670002    ' - COOP VK
        AllowTuesday = 202670003    ' - Generic
        AllowWednesday = 202670004    ' - Generic
        AllowThursday = 202670005    ' - Generic
        AllowFriday = 202670006    ' - Generic
        AllowSaturday = 202670007    ' - Generic
        AllowSunday = 202670008    ' - Generic
        PriceForSubRecipeWithoutTax = 2026700014    ' - Generic
        EnableNumberField = 2026700015    ' - Generic
        UsePrintLabel = 2026700071    ' - Generic
        DisplayVenueForRecipeList = 2026700073    ' - RCCL
        DisplayCookbookForRecipeList = 2026700074    ' - RCCL
        DisplayRecipeTypeForRecipeList = 2026700075    ' - RCCL
        AllowSharedToEditByOtherSites = 2026700076    ' - Generic
        DisplayProjectForRecipeList = 2026700077    ' - RCCL
        IsResetNumberFieldWhenCategoryChanged = 2026700080    ' - Generic
        NPOICategory = 2026700081    ' - UFS
        RecipeBakerWeightPercentage = 2026700082    ' - Generic
        NPOIStatus = 2026700083    ' - UFS
        NPOIRemark = 2026700084    ' - UFS
        IsAllowAutoUpdateOfMerchandiseNameInRecipeProcedure = 2026700085    ' - Generic
        NPOIKeywords = 2026700086    ' - UFS
        IsUsingImposedNutrient = 2026700087    ' - Generic
        NPOIIngNutrientValPer100gOr100ml = 2026700088    ' - UFS
        SendToFBProduction = 2026700089    ' - Generic
        ExportOschenDailyMP = 2026700090    ' - Generic
        SSLEnabled = 2026700090    ' - Generic
        SMPTPWOCridentials = 2026700091    ' - Generic
        NPOIAdditives = 2026700092    ' - UFS
        NPOIDieteticInfo = 2026700093    ' - UFS
        NPOIAdditionalDieteticInfo = 2026700094    ' - UFS
        NPOIGuidelineDailyAmount = 2026700095    ' - UFS
        NPOIPreparation = 2026700096    ' - UFS
        NPOIPicture = 2026700097    ' - UFS
        NPOIRecipeCosting = 2026700098    ' - UFS
        PrintQueueDaysBeforeDelete = 2026700099    ' - Generic
        WebServiceRecipeDisplay = 2026700100    ' - Generic
        UIDisplayRecipeListSellingPrice = 2026700101    ' - Generic
        IsAllowUseImposedNutrientOption = 2026700102    ' - Generic
        ShowMenuPlanImageBtn = 2026700103    ' - Generic
        ShowDelMonteExport = 2026700103    ' - Generic
        NPOIRecipeStory = 2026700104    ' - UFS
        NPOIPreparationOptinal = 2026700105    ' - UFS
        ShowNutriScoreUI = 2026700107    ' - Generic
        ShowExtraAttributesUI = 2026700108    ' - Generic
        ShowAbbreviatedPrepartion = 2026700300    ' - Generic
        UIMoreActionsMerchandiseValueOfEncodedEneryKcal = 2026700305    ' - Generic
        UIMoreActionsMerchandiseRecommendedEnergy = 2026700306    ' - Generic
        UIMoreActionsMerchandiseSaturatedFat = 2026700307    ' - Generic
        UIMoreActionsMerchandiseSugar = 2026700308    ' - Generic
        UIMoreActionsMerchandiseNutrient = 2026700309    ' - Generic
        UIMoreActionsMerchandisePrice = 2026700310    ' - Generic
        UIMoreActionsMerchandiseRatio = 2026700311    ' - Generic
        UIMoreActionsRecipeValueOfEncodedEneryKcal = 2026700312    ' - Generic
        UIMoreActionsRecipeRecommendedEnergy = 2026700313    ' - Generic
        UIMoreActionsRecipeSaturatedFat = 2026700314    ' - Generic
        UIMoreActionsRecipeSugar = 2026700315    ' - Generic
        UIMoreActionsRecipeNutrient = 2026700316    ' - Generic
        UIDisplayMerchandiseListDateOption = 2026700317    ' - Generic
        UIDisplayRecipeListDateOption = 2026700318    ' - Generic
        UIDisplayMenuListDateOption = 2026700319    ' - Generic
        MenuPlanSelectedPrintLabel1 = 2026700320    ' - Generic
        ShowMasterPlanBarcode = 2026700324    ' - Generic
        IsAllowReplaceComplementPreparationAlternativeIng = 2026700328    ' - Generic
        DisplayShipForRecipeList = 2026700329    ' - RCCL
        NPOIAllergens = 2026700350    ' - UFS
        NPOIClaimsAdditionalNutInfo = 2026700351    ' - UFS
        NPOICosting = 2026700352    ' - UFS
        NPOIGlobalCategories = 2026700353    ' - UFS
        NPOIGlobalKeywords = 2026700354    ' - UFS
        NPOIHACCP = 2026700355    ' - UFS
        NPOIIngrNutValPerServings = 2026700356    ' - UFS
        NPOINote = 2026700357    ' - UFS
        NPOINumber = 2026700358    ' - UFS
        NPOINutInfo = 2026700359    ' - UFS
        NPOISubCategory = 2026700360    ' - UFS
        NPOISubRecipes = 2026700361    ' - UFS
        MenuPlanExportIsMSC = 2026700362    ' - MSC
        MenuPlanExportMaxWeek = 2026700363    ' - Generic
        TESTEDD = 2026700364    'TestConfig - Generic
        TESTNUMEROBYISAGANI222111 = 2026700365    'This is numero test - COOP Gastronomie


    End Enum

    Private L_ErrCode As enumEgswErrorCode
    Private L_lngCodeUser As Int32 = -1
    Private L_lngCodeSite As Int32 = -1

    'Properties
    Private L_AppType As enumAppType
    Private L_dtList As DataTable
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType
    Private L_lngCode As Int32
#End Region

#Region "Class Functions and Properties"
    Public Sub New(ByVal eAppType As enumAppType, ByVal strCnn As String, _
      Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)

        Try

            L_AppType = eAppType
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn
        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Public ReadOnly Property AppType() As enumAppType
        Get
            AppType = L_AppType
        End Get
    End Property

    Public ReadOnly Property ItemsNotDeleted() As DataTable
        Get
            ItemsNotDeleted = L_dtList
        End Get
    End Property

    Public ReadOnly Property ConnectionString() As String
        Get
            ConnectionString = L_strCnn
        End Get
    End Property

    Public Property FetchReturnType() As enumEgswFetchType
        Get
            FetchReturnType = L_bytFetchType
        End Get
        Set(ByVal value As enumEgswFetchType)
            L_bytFetchType = value
        End Set
    End Property

    Public Property Code() As Int32
        Get
            Code = L_lngCode
        End Get
        Set(ByVal value As Int32)
            L_lngCode = value
        End Set
    End Property
#End Region

#Region "Update Methods"
    ''' <summary>
    ''' Updates an option in Configuration 
    ''' </summary>
    ''' <param name="intCodeUser"></param>
    ''' <param name="intCodeGroup"></param>
    ''' <param name="enumNumero"></param>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateConfig(ByVal intCodeUser As Integer, ByVal intCodeGroup As CodeGroup, ByVal enumNumero As enumNumeros, ByVal value As String, Optional ByVal flagDoNotCheckNumeric As Boolean = False) As enumEgswErrorCode
        If flagDoNotCheckNumeric = False And IsNumeric(value) Then
            Dim cultureToSave As New CultureInfo("en-US")
            value = CStr(CDbl(value).ToString(cultureToSave.NumberFormat))
        End If


        Try
            'Convert Boolean values
            Select Case value.ToUpper
                Case "TRUE"
                    value = "!B=1"
                Case "FALSE"
                    value = "!B=0"
            End Select

            Dim arrParam(3) As SqlParameter
            arrParam(0) = New SqlParameter("@intCodeUser", intCodeUser)
            arrParam(1) = New SqlParameter("@intNumero", enumNumero)
            arrParam(2) = New SqlParameter("@string", value)
            arrParam(3) = New SqlParameter("@intcodeGroup", intCodeGroup)
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_egswConfigUpdate", arrParam)
            'clsConfig.ExecuteNonQuery(GetConnection("dsn"), CommandType.StoredProcedure, "sp_egswUpdateEgswConfig", arrParam) 'jhl
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Throw New Exception
        End Try
    End Function

    Public Function UpdateConfigNutSet(ByVal intCodeUser As Integer, ByVal intCodeGroup As CodeGroup, ByVal enumNumero As enumNumeros, ByVal value As String, Optional ByVal flagDoNotCheckNumeric As Boolean = False, Optional ByVal intCodeSet As Integer = -1) As enumEgswErrorCode
        If flagDoNotCheckNumeric = False And IsNumeric(value) Then
            Dim cultureToSave As New CultureInfo("en-US")
            value = CStr(CDbl(value).ToString(cultureToSave.NumberFormat))
        End If

        Try
            'Convert Boolean values
            Select Case value.ToUpper
                Case "TRUE"
                    value = "!B=1"
                Case "FALSE"
                    value = "!B=0"
            End Select

            Dim arrParam(3) As SqlParameter
            arrParam(0) = New SqlParameter("@intCodeUser", intCodeUser)
            If intCodeSet = -1 Then '
                arrParam(1) = New SqlParameter("@intNumero", enumNumero)
            Else
                Dim strValue As String = enumNumero & intCodeSet.ToString()
                arrParam(1) = New SqlParameter("@intNumero", strValue)
            End If

            arrParam(2) = New SqlParameter("@string", value)
            arrParam(3) = New SqlParameter("@intcodeGroup", intCodeGroup)
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_egswConfigUpdate", arrParam)
            'clsConfig.ExecuteNonQuery(GetConnection("dsn"), CommandType.StoredProcedure, "sp_egswUpdateEgswConfig", arrParam) 'jhl
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Throw New Exception
        End Try
    End Function


    '' Add or update configuration 
    'Public Function UpdateConfig(ByVal intCodeUser As Integer, ByVal intCodeGroup As Integer, ByVal enumNumero As enumNumeros, ByVal value As String) As Boolean
    '    Try
    '        Dim arrParam(3) As SqlParameter
    '        arrParam(0) = New SqlParameter("@intCodeUser", intCodeUser)
    '        arrParam(1) = New SqlParameter("@intNumero", enumNumero)
    '        arrParam(2) = New SqlParameter("@string", value)
    '        arrParam(3) = New SqlParameter("@intcodeGroup", intCodeGroup)
    '        'jhl Me.ExecuteNonQuery(GetConnection("dsn"), CommandType.StoredProcedure, "sp_egswUpdateEgswConfig", arrParam)
    '        clsConfig.ExecuteNonQuery(GetConnection("dsn"), CommandType.StoredProcedure, "sp_egswUpdateEgswConfig", arrParam) 'jhl
    '        Return True
    '    Catch ex As Exception
    '        MyBase.m_nError = ex
    '        Return False
    '    End Try
    'End Function


#End Region

#Region "Get Methods"
    Public Function GetConfigBoolean(ByVal intCodeUser As Integer, ByVal enumNumero As enumNumeros, ByVal intCodeGroup As CodeGroup, Optional ByVal strReturnDefaultString As String = "") As Boolean
        'VBV 06.12.2005
        Dim strX As String = GetConfig(intCodeUser, enumNumero, intCodeGroup, strReturnDefaultString)
        Return (strX = "!B=1" Or strX = "1" Or strX.ToUpper = "TRUE")

    End Function

    Public ReadOnly Property VersionDatabase() As Long
        Get
            Return VERSION_DATABASE
        End Get
    End Property
    Public ReadOnly Property VersionWeb() As String
        Get
            Return VERSION_WEB
        End Get
    End Property


    ''' <summary>
    ''' Fetch a row in Configuration
    ''' </summary>
    ''' <param name="intCodeUser"></param>
    ''' <param name="enumNumero"></param>
    ''' <param name="intCodeGroup"></param>
    ''' <param name="strReturnDefaultString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>    
    Public Function GetConfig(ByVal intCodeUser As Integer, ByVal enumNumero As enumNumeros, ByVal intCodeGroup As CodeGroup, Optional ByVal strReturnDefaultString As String = "") As String
        Try
            If L_strCnn = "" Or L_strCnn Is Nothing Then Return ""

            Dim arrParam(3) As SqlParameter
            arrParam(0) = New SqlParameter("@intCodeUser", intCodeUser)
            arrParam(1) = New SqlParameter("@intNumero", enumNumero)
            arrParam(2) = New SqlParameter("@intcodeGroup", intCodeGroup)
            arrParam(3) = New SqlParameter("@string", SqlDbType.NVarChar, 500) 'AGL 2013.02.09 - changed to nvarchar
            arrParam(3).Direction = ParameterDirection.Output
            'clsConfig.ExecuteNonQuery(GetConnection("dsn"), CommandType.StoredProcedure, "sp_egswGetEgswConfig", arrParam)
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_egswConfigGetItem", arrParam)

            If arrParam(3).Value.ToString.Trim.Length = 0 Then
                Return strReturnDefaultString
            Else
                ' Convert true/false
                Select Case arrParam(3).Value.ToString.ToUpper
                    Case "!B=1"
                        Return CStr(True)
                    Case "!B=0"
                        Return CStr(False)
                End Select

                If enumNumero = enumNumeros.TCPOSExportTime Or enumNumero = enumNumeros.TCPOSExportLastExportedDate Then
                    Return CStr(arrParam(3).Value)
                End If


                ' handle float values
                Dim tmpCulture As CultureInfo = Thread.CurrentThread.CurrentCulture
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US")
                If IsNumeric(arrParam(3).Value) AndAlso CDbl(arrParam(3).Value) <> 0 Then
                    Dim dbl As Double = CDbl(arrParam(3).Value)
                    Thread.CurrentThread.CurrentCulture = tmpCulture
                    GetConfig = dbl.ToString()
                    Exit Function
                End If
                Thread.CurrentThread.CurrentCulture = tmpCulture
                'Dim dbl As Double = Val(arrParam(3).Value)
                'Dim str2 As String = dbl.ToString
                'If IsNumeric(dbl) And dbl <> 0 Then
                '    'Return str2
                '    Return dbl.ToString(Thread.CurrentThread.CurrentCulture)
                'End If

                Return arrParam(3).Value.ToString
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetConfigNutSet(ByVal intCodeUser As Integer, ByVal enumNumero As enumNumeros, ByVal intCodeGroup As CodeGroup, Optional ByVal strReturnDefaultString As String = "", Optional ByVal intCodeSet As Integer = -1) As String
        Try
            If L_strCnn = "" Or L_strCnn Is Nothing Then Return ""

            Dim arrParam(3) As SqlParameter
            arrParam(0) = New SqlParameter("@intCodeUser", intCodeUser)
            If intCodeSet = -1 Then
                arrParam(1) = New SqlParameter("@intNumero", enumNumero)
            Else
                Dim strValue As String = enumNumero & intCodeSet
                arrParam(1) = New SqlParameter("@intNumero", CInt(strValue))
            End If
            arrParam(2) = New SqlParameter("@intcodeGroup", intCodeGroup)
            arrParam(3) = New SqlParameter("@string", SqlDbType.NVarChar, 500) 'AGL 2013.02.09 - changed to nvarchar
            arrParam(3).Direction = ParameterDirection.Output
            'clsConfig.ExecuteNonQuery(GetConnection("dsn"), CommandType.StoredProcedure, "sp_egswGetEgswConfig", arrParam)
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_egswConfigGetItem", arrParam)

            If arrParam(3).Value.ToString.Trim.Length = 0 Then
                Return strReturnDefaultString
            Else
                ' Convert true/false
                Select Case arrParam(3).Value.ToString.ToUpper
                    Case "!B=1"
                        Return CStr(True)
                    Case "!B=0"
                        Return CStr(False)
                End Select

                If enumNumero = enumNumeros.TCPOSExportTime Or enumNumero = enumNumeros.TCPOSExportLastExportedDate Then
                    Return CStr(arrParam(3).Value)
                End If


                ' handle float values
                Dim tmpCulture As CultureInfo = Thread.CurrentThread.CurrentCulture
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US")
                If IsNumeric(arrParam(3).Value) AndAlso CDbl(arrParam(3).Value) <> 0 Then
                    Dim dbl As Double = CDbl(arrParam(3).Value)
                    Thread.CurrentThread.CurrentCulture = tmpCulture
                    GetConfigNutSet = dbl.ToString()
                    Exit Function
                End If
                Thread.CurrentThread.CurrentCulture = tmpCulture
                'Dim dbl As Double = Val(arrParam(3).Value)
                'Dim str2 As String = dbl.ToString
                'If IsNumeric(dbl) And dbl <> 0 Then
                '    'Return str2
                '    Return dbl.ToString(Thread.CurrentThread.CurrentCulture)
                'End If

                Return arrParam(3).Value.ToString
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ' Public Function GetVersion() As String
    'Try
    '    Dim strSQL As String = "SELECT * FROM rnVersion"
    '    'Dim dt As DataTable = ExecuteDataset(GetConnection("dsn"), CommandType.Text, strSQL).Tables(0)
    '    Return CStr(dt.Rows(0)("version"))

    'Catch ex As Exception
    '    'MyBase.m_nError = ex
    '    Return ""
    'End Try
    '   End Function

    '' Get Config String value
    'Public Function GetConfig(ByVal intCodeUser As Integer, ByVal enumNumero As enumNumeros, ByVal intCodeGroup As Integer, Optional ByVal strReturnDefaultString As String = "") As String
    '    Try
    '        Dim arrParam(3) As SqlParameter
    '        arrParam(0) = New SqlParameter("@intCodeUser", intCodeUser)
    '        arrParam(1) = New SqlParameter("@intNumero", enumNumero)
    '        arrParam(2) = New SqlParameter("@intcodeGroup", intCodeGroup)
    '        arrParam(3) = New SqlParameter("@string", SqlDbType.VarChar, 500)
    '        arrParam(3).Direction = ParameterDirection.Output

    '        'jhl Me.ExecuteNonQuery(GetConnection("dsn"), CommandType.StoredProcedure, "sp_egswGetEgswConfig", arrParam)
    '        clsConfig.ExecuteNonQuery(GetConnection("dsn"), CommandType.StoredProcedure, "sp_egswGetEgswConfig", arrParam)

    '        If arrParam(3).Value.ToString.Length = 0 Then
    '            Return strReturnDefaultString
    '        Else
    '            Return arrParam(3).Value.ToString
    '        End If
    '    Catch ex As Exception
    '        MyBase.m_nError = ex
    '        Return ""
    '    End Try
    'End Function
#End Region

    Public Function DeleteBackupPicture(ByVal intCode As Integer) As enumEgswErrorCode
        Dim sb As StringBuilder = New StringBuilder
        sb.Append("DELETE FROM EgswBackUp WHERE Code=" & intCode)

        Try
            ExecuteNonQuery(L_strCnn, CommandType.Text, sb.ToString)
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function GetBackupPath(ByVal intCode As Integer) As String
        Dim sb As StringBuilder = New StringBuilder
        sb.Append("SELECT @vchFilePath = FilePath FROM egswBackUp WHERE Code=" & intCode & " ")

        Dim sqlCmd As New SqlCommand
        With sqlCmd
            .Connection = New SqlConnection(L_strCnn)
            .CommandText = sb.ToString
            .CommandType = CommandType.Text
            .CommandTimeout = 3600
            .Parameters.Add("@vchFilePath", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output
            Try
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                Return CStr(.Parameters("@vchFilePath").Value)
            Catch ex As Exception
                Return ""
            End Try
        End With
    End Function

    Public Function BackupPicture(ByVal dtmDate As Date, ByVal intCodeUser As Integer, ByVal strDescription As String, ByVal strFilePath As String) As enumEgswErrorCode
        Dim sb As StringBuilder = New StringBuilder
        sb.Append("INSERT INTO EgswBackUp ")
        sb.Append("(Date, codeUser, Description, FilePath, [type]) ")
        sb.Append("VALUES ")
        sb.Append("(@dtmDate, @intCodeUser, @vchDesc, @vchFilePath, 2) ")

        Dim sqlCmd As SqlCommand = New SqlCommand
        With sqlCmd
            .Connection = New SqlConnection(L_strCnn)
            .CommandText = sb.ToString
            .CommandType = CommandType.Text
            .CommandTimeout = 3600

            .Parameters.Add("@dtmDate", SqlDbType.DateTime).Value = dtmDate
            .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
            .Parameters.Add("@vchDesc", SqlDbType.VarChar, 100).Value = strDescription
            .Parameters.Add("@vchFilePath", SqlDbType.VarChar, 200).Value = strFilePath

            Try
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                Return enumEgswErrorCode.OK
            Catch ex As Exception
                If .Connection.State <> ConnectionState.Closed Then .Connection.Close()
                Return enumEgswErrorCode.GeneralError
            End Try
        End With

    End Function


    Public Function BackupDatabase(ByVal dtmDate As Date, ByVal intCodeUser As Integer, ByVal strDescription As String, ByVal strFilePath As String, ByVal strDbaseName As String) As enumEgswErrorCode
        Dim sb As StringBuilder = New StringBuilder
        sb.Append("INSERT INTO EgswBackUp ")
        sb.Append("(Date, codeUser, Description, FilePath, [type]) ")
        sb.Append("VALUES ")
        sb.Append("(@dtmDate, @intCodeUser, @vchDesc, @vchFilePath, 1) ")

        sb.Append("BACKUP DATABASE " & strDbaseName & " ")
        sb.Append("TO DISK = @vchFilePath ")
        sb.Append("WITH FORMAT, ")
        sb.Append("NAME = @vchDesc ")

        Dim sqlCmd As SqlCommand = New SqlCommand
        With sqlCmd
            .Connection = New SqlConnection(L_strCnn)
            .CommandText = sb.ToString
            .CommandType = CommandType.Text
            .CommandTimeout = 3600

            .Parameters.Add("@dtmDate", SqlDbType.DateTime).Value = dtmDate
            .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
            .Parameters.Add("@vchDesc", SqlDbType.VarChar, 100).Value = strDescription
            .Parameters.Add("@vchFilePath", SqlDbType.VarChar, 200).Value = strFilePath

            Try
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                Return enumEgswErrorCode.OK
            Catch ex As Exception
                If .Connection.State <> ConnectionState.Closed Then .Connection.Close()
                Return enumEgswErrorCode.GeneralError
            End Try
        End With
    End Function

    Public Function RestoreDatabase(ByVal intCode As Integer, ByVal strDbaseName As String) As enumEgswErrorCode
        Dim sb As StringBuilder = New StringBuilder
        sb.Append("DECLARE @vchFilePath varchar(200) ")
        sb.Append("SELECT @vchFilePath = FilePath FROM egswBackUp WHERE Code=" & intCode & " ")
        sb.Append("DECLARE @int int ")
        sb.Append("EXEC xp_fileexist @vchFilePath, @int OUTPUT ")
        sb.Append("IF @int=1 ")
        sb.Append("BEGIN ")
        sb.Append("BEGIN TRY ")
        sb.Append("USE MASTER ALTER DATABASE " & strDbaseName & " SET SINGLE_USER WITH ROLLBACK IMMEDIATE ")
        sb.Append("RESTORE DATABASE " & strDbaseName & " ")
        sb.Append("FROM DISK = @vchFilePath ")
        sb.Append("SET @retval=0 ")
        sb.Append("END TRY ")
        sb.Append("BEGIN CATCH ")
        sb.Append("ALTER DATABASE " & strDbaseName & " SET MULTI_USER ")
        sb.Append("SET @retval=-1 ")
        sb.Append("END CATCH ")
        sb.Append("END ")
        sb.Append("ELSE ")
        sb.Append("BEGIN SET @retval=-9 END ")

        Dim sqlCmd As New SqlCommand
        With sqlCmd
            .Connection = New SqlConnection(L_strCnn)
            .CommandText = sb.ToString
            .CommandType = CommandType.Text
            .CommandTimeout = 3600
            .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.Output
            Try
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                If CInt(.Parameters("@retval").Value) = 0 Then
                    Return enumEgswErrorCode.OK
                ElseIf CInt(.Parameters("@retval").Value) = -9 Then
                    Return enumEgswErrorCode.NotExists
                Else
                    Return enumEgswErrorCode.GeneralError
                End If
            Catch ex As Exception
                If .Connection.State <> ConnectionState.Closed Then .Connection.Close()
                Return enumEgswErrorCode.GeneralError
            End Try
        End With
    End Function

    ''' <summary>
    ''' 1=dbase, 2= picture
    ''' </summary>
    ''' <param name="shortType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetBackupList(ByVal shortType As Short) As DataTable
        Dim strSQL As String = "SELECT * FROM egswBackUp WHERE [Type]=" & shortType
        Try
            Return ExecuteDataset(L_strCnn, CommandType.Text, strSQL).Tables(0)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function DeleteBackUpDatabase(ByVal intCode As Integer) As enumEgswErrorCode
        Dim sb As StringBuilder = New StringBuilder
        sb.Append("DECLARE @vchFilePath varchar(200) ")
        sb.Append("SELECT @vchFilePath=FilePath FROM EgswBackup ")
        sb.Append("DELETE FROM EgswBackUp WHERE Code=" & intCode)
        sb.Append("DECLARE @int int ")
        sb.Append("EXEC xp_fileexist @vchFilePath, @int OUTPUT ")
        sb.Append("IF @int=1 ")
        sb.Append("BEGIN ")
        sb.Append("EXEC XP_DELETE_FILE 0, @vchFilePath ")
        sb.Append("SET @retval=0 ")
        sb.Append("END ")
        sb.Append("ELSE ")
        sb.Append("BEGIN SET @retval=-1 END ")

        Try
            Dim arrParam() As SqlParameter = {New SqlParameter("@retval", SqlDbType.Int)}
            arrParam(0).Direction = ParameterDirection.Output
            ExecuteNonQuery(L_strCnn, CommandType.Text, sb.ToString, arrParam)

            If CInt(arrParam(0).Value) = 0 Then
                Return enumEgswErrorCode.OK
            Else
                Return enumEgswErrorCode.NotExists
            End If
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function GetDatabaseVersion() As Long
        Dim lngX As Long
        Dim sqlCmd As New SqlCommand
        With sqlCmd
            .Connection = New SqlConnection(L_strCnn)
            .CommandText = "SELECT Version FROM EgsWSystem"
            .CommandType = CommandType.Text
            .CommandTimeout = 3600

            Try
                .Connection.Open()
                lngX = CLng(.ExecuteScalar())
                sqlCmd.Connection.Close()
                sqlCmd.Dispose()

                Return lngX
            Catch ex As Exception
                Return -1
            End Try
        End With
    End Function

    Public Function GetApprovalMessage() As DataTable
        Dim sb As StringBuilder = New StringBuilder
        sb.Append("SELECT CodeTrans, MessageType, Message, [Title], ListeType FROM EgswApprovalEmailMessage")

        Dim sqlCmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        With sqlCmd
            .Connection = New SqlConnection(L_strCnn)
            .CommandText = sb.ToString
            .CommandType = CommandType.Text
            .CommandTimeout = 3600
            '.Parameters.Add("@Version", SqlDbType.Int).Direction = ParameterDirection.Output
            Try
                .Connection.Open()
                With da
                    .SelectCommand = sqlCmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                Return dt
            Catch ex As Exception
                da = Nothing
                dt = Nothing
                Throw New Exception(ex.Message, ex)
            End Try
        End With
    End Function

    Public Function GetParentSite(ByVal ParentCode As Integer, ByVal StrTable As String) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter
        Try
            With cmd
                .Connection = cn
                .CommandText = "[sp_EgswGetParentSite]"
                .Parameters.Add("@intCode", SqlDbType.Int).Value = ParentCode
                .Parameters.Add("@nvcTable", SqlDbType.NVarChar, 50).Value = StrTable
                .CommandType = CommandType.StoredProcedure
            End With
            cn.Open()
            With da
                .SelectCommand = cmd
                'dt.BeginLoadData()
                .Fill(dt)
                'dt.EndLoadData()
            End With
            Return dt
        Catch ex As Exception
        End Try
        cn.Close()
        cn.Dispose()
    End Function

    Public Function UpdateApprovalMessage(ByVal intCodeTrans As Integer, ByVal intMessageType As Integer, ByVal intListeType As Integer, ByVal strMessage As String, ByVal strTitle As String) As enumEgswErrorCode
        Dim sb As StringBuilder = New StringBuilder
        sb.Append("IF EXISTS ")
        sb.Append("( ")
        sb.Append("SELECT CodeTrans FROM EgswApprovalEmailMessage ")
        sb.Append("WHERE CodeTrans=@intCodeTrans AND MessageType=@intMessageType AND ListeType=@intListeType ")
        sb.Append(") ")
        sb.Append("BEGIN ")
        sb.Append("UPDATE EgswApprovalEmailMessage SET [Message]=@vchMessage, [Title]=@vchTitle ")
        sb.Append("WHERE CodeTrans=@intCodeTrans AND MessageType=@intMessageType AND ListeType=@intListeType ")
        sb.Append("END ")
        sb.Append("ELSE ")
        sb.Append("BEGIN ")
        sb.Append("INSERT INTO EgswApprovalEmailMessage([Message],[Title],CodeTrans,MessageType,ListeType) ")
        sb.Append("VALUES (@vchMessage,@vchTitle,@intCodeTrans,@intMessageType,@intListeType) ")
        sb.Append("END ")

        Try
            Dim sqlCmd As New SqlCommand
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = sb.ToString
                .CommandType = CommandType.Text
                .CommandTimeout = 3600
                .Parameters.Add("@vchMessage", SqlDbType.VarChar).Value = strMessage
                .Parameters.Add("@vchTitle", SqlDbType.VarChar).Value = strTitle
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@intMessageType", SqlDbType.Int).Value = intMessageType
                .Parameters.Add("@intListeType", SqlDbType.Int).Value = intListeType

                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
            End With
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function


    Public Function MetricImpNutrientCalculationBasis(intCodeSite) As Integer
        Try
            Dim clConfig As New clsConfig(enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataSet)

            Dim intBasis As Integer
            intBasis = clConfig.GetConfig(intCodeSite, clsConfig.enumNumeros.OneOrMetricImperialNutrientCalculationBasis, clsConfig.CodeGroup.site, 1) 'returns 1 for Metric, 2 for Imperial
            clConfig = Nothing

            'should return 1 for Metric, 0 for Imperial
            If intBasis = 2 Then 'imperial
                Return 0
            Else
                Return intBasis
            End If
            Return intBasis - 1
        Catch ex As Exception
            Return 0
        End Try
    End Function
End Class

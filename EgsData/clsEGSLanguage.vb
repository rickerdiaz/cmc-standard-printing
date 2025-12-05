Imports EgsWebFTB

Public Class clsEGSLanguage

    Private m_LanguageDefault As Integer = 1
    Private m_Language As Integer
    Private m_cli As New EgswKey.clsLicense

    Public Enum CodeType As Integer
        More = 158346
        MarkedItems = 161106
        ComputedYield = 161107
        Exact = 161078
        StartsWith = 161079
        Contains = 161080
        Second = 161082
        Third = 161083
        fourth = 161084
        OneTimeOnly = 161085
        Daily = 161086
        Weekly = 161087
        Monthly = 161088
        WhenFileChanges = 161089
        WhenComputerStarts = 161090
        Enter_s_Information = 161091
        SupplierGroup = 161092
        BillingInformation = 161093
        StartDate = 161094
        oftheMonth = 161095
        POSImportFailedData = 161096
        ManageSupplierContactDesc = 161097
        ManageTerminalDesc = 161098
        ManagePOSImportConfigDesc = 161099
        ManageLocationDesc = 161100
        ManageClientDesc = 161101
        ManageClientContactDesc = 161102
        ManagePOSImportTempDesc = 161103
        ManageIssuanceTypeDesc = 161104
        ManageSalesHistoryDesc = 161105

        '-------------------------------------------------------------------------------
        AccountingID = 155374
        AccountingRef = 157695
        ShippingInformation = 159133
        Purchasing = 134055

        StudentVersion = 157055
        ForceDeleteOfKeywords = 161049
        ForceDeleteOfKeywordsInfoIngredientRecipe = 171741
        ForceDeleteOfKeywordsInfoMerchandiseRecipe = 161050
        'ConfirmForceDeleteKeywordsIngredientRecipe = 171742
        ConfirmForceDeleteKeywordsMerchandiseRecipe = 161051


        ConfirmChangeNutrientDBIngredient = 171740
        ConfirmChangeNutrientDBMerchandise = 161028

        YieldOrIngredientShouldBeChecked = 161029

        HideDetails = 159162
        ShowDetails = 51907
        NotLinkedSalesItem = 160985
        SalesItemAddDesc = 160987
        SalesItemDesc = 160988

        Border = 159009
        Left = 51246
        Right = 51247
        Top = 51244
        Bottom = 51245

        Separator = 133251
        Title = 14090
        LastName = 155163
        FirstName = 3204
        Mobile = 158653

        Sunday = 7570
        Monday = 7571
        Tuesday = 7572
        Wednesday = 7573
        Thursday = 7574
        Friday = 7575
        Saturday = 7576
        Last = 133080
        [Optional] = 157772
        Week = 160593
        Prefix = 149766
        Archives = 156355
        OpeningTime = 158902

        Input = 10131
        Output = 10132
        Contact = 134032
        EffectivityDate = 160940
        LinkedSalesItem = 160941
        SellingSetofPriceFactortoPurchasingSetofPrice = 160953
        SellingsetofPriceDescription = 160958
        ConsumptionWithin = 160433
        Composition = 159139
        IncludeInInventory = 151322

        SalesItemAlreadyLinked = 147441
        SalesItemList = 144734
        LinkSalesItemToProduct = 159782
        LinkSalesItemToRecipeMenu = 159783
        Link = 55211
        Factor = 6416
        Create = 160090
        Modify = 4867

        Discount = 10513
        SelectedItemActivated = 160788
        SelectedItemDeactivated = 160789
        AreYouSureYouWantToMergeSelectedItems = 160802
        AreYouSureYouWantToDeleteSelectedItems = 171737
        AreYouSureYouWantToRemoveSelectedItems = 160790
        AreYouSureYouWantToDeactivateSelectedItems = 160806
        AreYouSureYouWantToPurgeItems = 160803
        SelectedItemHasBeenSuccessfullyDeleted = 171738
        SelectedItemHasBeenSuccessfullyRemoved = 160791
        YouCanOnlyMergeSimilarRecipes = 160801
        SelectTwoOrMoreItemsToMerge = 160805
        PleaseFillOutRequiredFields = 160804
        Average = 4877
        Barcode = 12515
        Inventory = 10431
        LastOrder = 24002
        OnHand = 10130
        InOrder = 10370
        DefaultLocation = 24163
        DefaultProductionLocation = 151976
        DefaultAutomaticOutputLocation = 155575
        AutomaticTransfer = 157435
        UseDirectInputOutput = 149501
        RawMaterial = 157437
        Product = 51056
        ExpiresAfter = 156742

        CreateProduct = 104835
        ModifyProduct = 104836

        SupplierNumber = 10369
        DeleteTrailingZeroes = 171736
        RemoveTrailingZeroes = 160775

        Stock = 10140
        Packaging = 7720
        Transportation = 7725
        Deactivate = 160774
        Terminal = 157329
        POSImportConfig = 159795
        SalesItem = 138244
        FinishedGood = 149531
        Location = 10430
        IssuanceType = 146211
        Client = 146080

        Imposed = 133005
        SetGlobalByDefault = 160716
        License = 158005
        LicensesVersionModules = 134061
        RecipeCenterDesc = 159613
        MenuPadDesc = 159612
        PocketKitchenDesc = 159611
        FandBDesc = 159610
        CMWebDesc = 159609
        CMEntDesc = 159608
        CMProDesc = 160423
        CMChefDesc = 159607
        PicturesManagement = 155024
        BackupRestorePictures = 160691

        BackupInProgress = 140100
        RestoreInProgress = 140101
        DeleteBackupAbout = 160690
        Backup = 3680
        BackupCompleted = 3685
        RestoreFailed = 140129
        BackupFailed = 140130
        PathToSaveBackup = 140180
        BackupRestoreDatabase = 156141
        BackupNow = 156552
        RestoreCompleted = 11040
        FileToRestore = 155841
        Restore = 132657
        DatabaseMangement = 132654

        AlternatingItemColor = 160687
        NormalItemColor = 160688
        PleaseSelectSubscriptionType = 160628
        PleaseAcceptTermsAndCondition = 160629
        CMOAccepts3TranslationsOnly = 160630

        CMOUserPrintOptionsAbout = 160627
        CMOPreparingAccount = 160623
        CMOLogoUpload = 160619
        CMOTranslationAbout = 160620
        CMOMainLangAbout = 160621
        CMOSiteLangAbout = 160622

        CMOPackageExpire = 160567
        CMOPackageViewAccount = 160566
        Confirmation = 15620
        ReferenceNumber = 158158
        Accept = 157594
        UserAgreement = 157322

        SubscribeNow = 157379
        'YouWillBeRedirectedToPaypal = 158947
        YouCanPayYourSubscriptionUsingChoicesBelow = 157167
        SpaceQuotaReached = 157129

        SpecifyThePreferedPaymentMethod = 24070
        KindlyChooseYourPaymentTerms = 158169
        CreditCard = 157163
        KindlyEmailUsYourCreditCardDetails = 158170
        'BankWireTransfer = 158171
        ForPhilippineClientOnly = 158696
        OurBankDetailsForBankTransfer = 158697
        PleaseAdviseUsOnceTheTransferHasBeenMade = 158174

        SignUp = 157926
        CMOWelcome = 160542
        CMOWelcomeDesc = 160483
        CMOFeatures = 160498
        NotAllowedInDemoVersion = 9070
        MaximumNumberOfMenus = 132598
        CurrentNumberOfMenus = 132599
        MaximumNumberOfRecipes = 132589
        CurrentNumberOfRecipes = 132590
        MaximumNumberOfMerchandise = 132592
        MaximumNumberOfIngredients = 171674
        CurrentNumberOfMerchandise = 132593
        CurrentNumberOfIngredients = 171675

        Subscription = 157135
        YourSubscriptionWillExpireOn = 157380
        SubscriptionFee = 157173
        PromoCode = 157175
        PromoPrice = 160466
        Period = 31860
        Subtotal = 24291
        Account = 150707
        ExpiryDate = 27135
        Balance = 146067
        FAQ = 157182
        TermsAndCondition = 157183

        solutionForIndividualTitle = 160487
        solutionForIndividualBody1 = 160488
        solutionForIndividualBody2 = 160489
        solutionForIndividualBody3 = 160490
        solutionForIndividualLink = 160491

        solutionForBusinessTitle = 160492
        solutionForBusinessBody1 = 160493
        solutionForBusinessBody2 = 160494
        solutionForBusinesSLink = 160495

        featMerchandiseMangement = 152141
        featIngredientMangement = 171688
        featIngredientMangementDesc = 171699
        featMerchandiseMangementDesc = 158220
        featRecipeMenuManagement = 158222
        featRecipeMenuManagementDesc = 160499
        featTextManagemnt = 160500
        featTextMangementDesc = 160501
        featSearchFeatures = 158998
        featSearchIngredientRecipeFeaturesDesc = 171701
        featSearchMerchandiseRecipeMenuFeaturesDesc = 158230
        featPicturesDesc = 160502
        featActionMarkIngredientRecipeDesc = 171702
        featActionMarkMerchandiseRecipeMenuDesc = 158232
        featNutrienLlinkingAndCalculation = 158234
        featNutrientLinkingAndCalculationDesc = 160503
        featExportationImportation = 160504
        featExportationImportationDesc = 160505
        featSupplierManagemnt = 158238
        featSupplierManagemntDesc = 160537
        featCategoryKeywordSourceMangement = 158240
        featCategoryKeywordSourceMangementDesc = 160538
        featTaxRateMangement = 158243
        featTaxRateMangementDesc = 160506
        featUnitMangement = 158246
        featUnitMangementDesc = 160539
        featPrintingPDFExcel = 158249
        featPrintingIngredientRecipePDFExcelDesc = 171703
        featPrintingMerchandiseRecipeMenuPDFExcelDesc = 158999
        featSetOfPriceCurrencyManagemnt = 159000
        featSetOfPriceCurrencyManagemntDesc = 160540
        featShoppingList = 51502
        featShoppingListDesc = 160507

        CMOPackages = 160508
        CMOPackagesAbout = 160518

        Font = 14070
        AccountIsCurrentlyUsed = 160295
        'IngredientAllergenDescription = 171735
        MerchandiseAllergenDescription = 160293
        'Successfully_Removed = 7335
        Abbreviation = 24121
        Allergens = 160292
        NothingWasChanged = 151250
        InvalidSElection = 156344
        Sharing = 157320
        LimitedByLicenses = 159275
        NameTooLong = 156721
        NumberToolong = 156720
        SupplierToolong = 156722
        CategoryTooLong = 156723
        UnitTooLong = 159140
        DescriptionTooLong = 156725
        UnvalidNumericInOneOfthefields = 135955
        UnitDoesNotExists = 159141
        Atleastoneunitdefined = 28655
        UnitSIdentical = 156734
        Namecannotbeblank = 159064
        Xcannotbeblank = 159142

        invalidnumericInoneofthefields = 135955
        currencydoesnotmatchchosensetofprice = 160258
        nameornumberalreadyexists = 160259
        dateimported = 160260
        dataerror = 131462
        RestartWindowsService = 160254

        SelectAtLeastTwo = 151364
        Missing = 135986
        File = 140056
        ChangeUnit = 136171
        Ratio = 147462
        saleItemsNumber = 158677
        CSVImportIngredientOptionDescription = 171734
        CSVImportMerchandiseOptionDescription = 160220
        PendingCSVImportIngredientListDescription = 17173
        PendingCSVImportMerchandiseListDescription = 160219
        ImportIngredientCSV = 171743
        ImportMerchandiseCSV = 155761 '156590
        PendingCSVImportListIngredient = 171731
        PendingCSVImportListMerchandise = 160218

        POSSettings = 158860
        'Time = 144591

        ArchivePath = 160217
        DecimalSeparator = 8914
        DeleteUnusedIngredientUnitsBeforeImport = 157142
        DeleteUnusedMerchandiseUnitsBeforeImport = 171694
        FieldSeparator = 155967
        ThousandSeparator = 156825
        UpdateExistingRecord = 159699
        FileNotFound = 30270
        FileMissing = 133330
        InvalidDirectory = 28008
        UpdateBaseOnDateModified = 159082


        Translation = 3206
        Calculation = 8210
        Accounts = 150707

        ConfirmClose = 160066
        EntrySubmittedForApproval = 160071
        RequestAlreadyExists = 160072
        InvalidPrice = 134195
        ThisIngredientIsPublishedOnTheWeb = 171710
        MerchandisePublishedOnTheWeb = 160018

        ThisIngredientIsNotPublishedOnTheWeb = 171711
        MerchandiseNotPublishedOnTheWeb = 160019
        OriginalVersion = 156683
        FirstLevel = 160004
        None = 8913
        NameShouldNotExceed25Characgers = 159950
        FormatShouldNotExceed10Charaters = 159949
        WebSiteProfile = 160109
        YourRequestHasBeenReviewed = 160085
        CreateTextDescription = 160191
        'PrintMechandiseDetails = 160160
        ShowListOfSavedMarks = 160188
        'TCPOSExport = 160184
        Activate = 160089
        MakeContentAvailableInKioskBrowser = 160094
        CreateASystemCopy = 160095
        DoNotPublish = 160098
        MakeContentUnAvailableInKioskBrowser = 160094
        Welcome = 160106
        ApprovalRouting = 160111
        SMTPSettings = 160113
        SMTPSettingsDescriptin = 150644
        PrintRecipeDetails = 160161
        Security = 160177
        PrintList = 160087
        PrintDetails = 160088
        PrintRecipeDetailsDesc = 160161
        PrintIngredientDetailsDesc = 171729
        PrintMerchandiseDetailsDesc = 160160
        CreateRecipeDescription = 160102
        ConfigurationDescription = 160105
        CustomizeViewAndSettings = 160108
        CustomizeSiteNameANdThemes = 160110
        ApprovalOfIngredientsRecipesAndOthers = 171715
        ApprovalOfMerchandiseRecipesANdOthers = 160112
        ConfigureConnectionToMailServer = 160114
        SetMaxLoginAttempts = 160115
        PrintProfileDescription = 160117
        CurrencyDescription = 160119
        SetOfPriceDescIngredientsRecipes = 171718
        SetOfPriceDescMerchandiseRecipes = 160120
        PropertiesDescription = 160121
        SiteDescription = 160122
        UsersDescription = 160123
        'ImageProcessingDescriptionIngredientRecipe = 171720
        ImageProcessingDescriptionMerchandiseRecipe = 160125
        BrandDescriptionIngredients = 171721
        BrandDescriptionMerchandise = 160130
        CategoryDescriptionIngredientRecipe = 171722
        CategoryDescriptionMerchandiseRecipe = 160132
        KeywordDescriptionIngredientRecipe = 171724
        KeywordDescriptionMerchandiseRecipe = 160135

        NutrientDescriptionUpToNNutrientValues = 171725
        NutrientDescriptionUpTo15NutrientValues = 160139
        NutrientRulesDescription = 160141
        SuppliersDescription = 104829
        UnitsDescriptionIngredientRecipe = 171726
        UnitsDescriptionMerchandiseRecipe = 160151
        TaxDescription = 160153
        SourceDescription = 160154
        ImportDescriptionIngredientRecipe = 171727
        ImportDescriptionMerchandiseRecipe = 160155
        ExchangeRateDescription = 160156
        PurgeTextDescription = 160157
        FormatAllTexts = 160158
        MenuCardDescription = 160164
        ShoppingListDescription = 160175
        StandardizeDescription = 160180
        PurgeItems = 160181
        ExportSalesItems = 160185
        'CreateIngredientDescription = 171730
        CreateMerchandiseDescription = 160187
        CreateMenuDescription = 160190
        'TextDescription = 160101
        TextDescriptionMerch = 172031 'JTOC 27.12.2012 172031 'AGL 2012.10.26 - CWM-1310
        YoureNotSignIn = 160040
        LastLogin = 160039
        EnterUsernameandPassword = 151907
        RememberMe = 160014
        [Continue] = 26000
        AddKeywords = 160033
        ForPrinting = 160023
        NotToPublish = 160028
        AddToShoppingList = 160030
        ThisIngredientIsExposed = 171712
        ThisMerchanidseIsExposed = 160020
        ThisIngredientIsNotExposed = 171713
        ThisMerchanidseIsNotExposed = 160021
        Advanced = 159778
        Notdefined = 138412
        UnwantedIngredients = 51130
        WantedIngredients = 51129
        WantedItems = 160210
        UnwantedItems = 160211
        Drafts = 160212
        ThisRecipeMenuIsPublishontheweb = 160012
        ThisRecipeMenuIsNotPublishontheweb = 160013
        RemoveexistingMarksFirst = 147126
        ShowToNewPageIfDifferentSupplier = 146114
        Yield = 51294
        SubmitForGlobalSharing = 160093
        RequestedBy = 159988
        RequestType = 159987
        MoveMarkedToANewBrand = 159966
        EnterTranslation = 159963
        EditTranslation = 174656
        EnterTaxInformation = 159962
        PrintProfile = 160116
        EditPrintProfile = 174871
        NewPrintProfile = 174870
        Quality = 159943
        Manage = 159924
        LicenseExpired = 150688
        RegistrationInformationSaved = 159430
        ProductID = 4185
        HeaderName = 27055
        EnterSerialNumber = 132561
        Registration = 11280
        Ok = 147070
        SendShoppingListToPocketKitchen = 155118
        Dictionary = 56500
        LabelDictionary = 175299
        Connection = 135971
        EmailSuccessfullySent = 150634
        Test = 134083
        UnknownLanguage = 159488
        RecipeApprovedAndExposedToAllUsers = 159487
        DownloadedOk = 156925
        ApprovedBy = 124024
        Download = 51252
        SubmittedToRecipeExchange = 159486
        Open = 147174
        ReplaceWith = 14816
        EmailNotFound = 151906
        ForgotPassword = 151912
        LoadListofShoppingListSaved = 155942
        SubmitTORecipeExchange = 159485
        Back = 24270
        Greaterthan = 133000
        Lessthan = 133001
        OnOrAfter = 132998
        Between = 132999
        OnOrBefore = 132997
        ResetFilter = 159349
        EnterAtLeastNCharacters = 159474
        [default] = 137030
        MaximumAttempts = 159473
        'IPBlockedList = 159472
        counter = 1145
        IPAddress = 159471
        Schedule = 159171
        LanguageWordBreaker = 159464
        AboutFTS = 159457
        Enable = 155507
        Every = 159461
        'FullPopulation = 159458
        FullTextSearch = 159459
        'IncrementalPopulation = 159463
        minutes = 159460
        Run = 159462

        ImageProcessing = 159446
        DoNotChangeAnything = 54210
        ActivatePictureConversion = 133049
        LocalTransformationPicture = 133043
        MaximumPictureSize = 133046
        Optimization = 133047
        ImposePictureSIze = 159444
        Pixel = 155263

        History = 13255
        Guide_SMTP = 150644
        SystemAlertNotifications = 159436
        MoveToNewCategory = 159435
        Total = 5350
        SubmitToSystem = 159433
        SubmittedToSystem = 159434
        Updating = 9030
        ImportingFile = 159144
        IncludeSubRecipes = 135948
        IncludeRecipesAndSubRecipes = 158783
        ChangePassword = 158186
        Energy = 20530
        FileType = 4890
        Marked = 7183
        SomeCategories = 133111
        CalculatePrice = 158810

        [Select] = 158306
        SystemDefined = 7755
        AddIngredient = 132860
        Tools = 8994
        LoadASetOfMarks = 151336
        MarkedMenus = 15360
        Authentication = 157034
        SaveMarksForMenu = 151346
        SaveMarksforRecipe = 151345
        'SaveMarksforMerchandise = 151344
        SaveMarksForIngredient = 171686

        DocumentOutput = 133085
        PrintShoppingList = 51532

        InvalidUnit = 51311
        ReplaceIngredient = 132864
        ReplaceInMenu = 135968
        ReplaceInRecipe = 135967

        RecomputingNutrients = 133242
        Create_A_New_Menu = 132855
        Modify_Menu = 1600
        SubRecipeDefinition = 156413
        Servings = 5390
        MealInfo = 147692
        TotalPrice = 147700
        WeightPer = 144738
        YieldLeft = 147704
        YieldLost = 147707
        YieldPortion = 147703
        YieldReturn = 147706
        YieldSold = 147708
        YieldSpecial = 147710
        Info1 = 10573
        SubmitEntry = 159385
        Was_Saved_Succesfully_ValuesComputed = 159387
        Was_Saved_Succesfully_ValuesNotComputed = 159386
        InvalidDate = 147075
        CreateMenuCard = 159388
        ModifyMenucard = 159389
        PendingApproval = 159112
        Requests = 158912
        EmailSent = 159390
        PleaseTryAgainLater = 51178
        From = 4856
        Messages = 51157
        Subject = 151435
        [To] = 31758
        UploadPicture = 133405
        UploadDigitalAssets = 171681
        Unwanted = 51336
        Wanted = 51139
        CCPDescription = 10554
        CoolingTime = 10555
        HACCP = 132678
        HeatingDegree = 10557
        HeatingMode = 10558
        HeatingTime = 10556
        [Of] = 21600
        TypeQtyandselectunit = 147737
        MarkedMerchandse = 133112
        MarkedIngredient = 171678
        MarkedRecipes = 133116
        CancelRequest = 159089
        ReplaceProposal = 151499
        Approve = 162382 '157629
        Disapprove = 157633
        Type = 143987
        ConvertToBestUnit = 151424
        DisplayOptions = 133023
        NutrientLinking = 132788
        NutrientIsPerServingAt100 = 132972
        Nutrient100MG = 132706
        NutrientLink = 156337
        NutrientSummary = 132971
        RemoveLink = 149706
        DeleteLink = 171685

        AddItem = 132877
        Contributionmargin = 146056
        Cost = 147727
        Qty_Sold = 135256
        Sales = 134056
        Sold = 31769
        TotalCM = 159273
        TotalRevenue = 158935

        AddSeparator = 132865
        AddMerchandise = 132841


        AddRecipe = 132555
        CostOfGoods = 1081
        ApprovedPrice = 159391
        ImposedPrice = 1480
        TheoreticalImposedSellingPrice = 158376

        Shoppinglist = 2780
        deleteMarked = 132602
        ActionMarks = 132607
        MoveMarkedToNewCategory = 171676
        MoveMarkedItemsToNewCategory = 132601
        MoveToNewSupplier = 156000
        ConvertToSystemRecipe = 159382
        CreateShoppingListForRecipes = 157214
        CreateShoppingListForMenus = 157217
        SendEmail = 51377
        DoNotExpose = 159383
        InvalidQty = 134194
        Items = 135989
        CookingTip = 26101
        ExposeToAllUsers = 159381
        Productivity = 26104
        Publishontheweb = 156672
        Refinement = 26102
        Storage = 26103
        SetOfMarks = 132954
        SaveMarkedAs = 132957
        ChooseSetOfMarkToSave = 132955
        Company = 20122
        Approval = 149513 'DLS for all and Autogrill text
        MenuCategory = 132569
        MerchandiseCategory = 5900
        NutrientRules = 135056
        RecipeCategory = 132568
        SystemPref = 7755
        MatchbyName = 155764
        MatchByNumber = 155763
        SortBy = 133254
        UnableToDelete = 132570
        Definition = 133286
        Ascending = 159379
        Descending = 159380
        Supplier_Used = 24260
        AllLowerCase = 54230
        AllUpperCase = 54220
        CapitalizeFirstLetterEachWord = 54240
        FirstLettercapitalized = 54245
        Source_Used = 133260
        [Global] = 159372
        ViewOwners = 160016
        Change = 137019
        Database = 3057
        RecalculateNutrients = 132828
        AddNutrientRule = 135058
        ModifyNutrientRule = 135059
        Maximum = 4855
        Minimum = 4854
        Nutrient = 10572
        Keyword = 132783
        ChooseFromTheList = 2430
        StandardizeUnits = 132915
        StandardizeYield = 132924
        Standardize = 132671
        StandardizeKeywords = 133266
        Inheritable = 159113
        InvalidFileType = 157306
        InvalidFileName = 133133
        Failed = 10417
        UnableSendEmail = 171453
        Successfully_Imported = 159370
        Add = 8395
        Exclude = 158730
        Overwrite = 147699
        Status = 10468
        DeleteFilesAfterImportation = 156485
        HideExisting = 157901
        UseMainUnitWhenAddingIngredientPrice = 171697
        UseMainUnitWhenAddingMerchandisePrice = 157314
        Assign = 24085
        AssignKeyword = 132600
        Existing = 135985
        Filename = 156754
        Import = 3760
        FileUploaded = 159437
        CompareBy = 159369
        MenuKeyword = 159298
        Merchandisekeyword = 151019
        Options = 10109
        RecipeKeyword = 151020
        Rename = 135979
        SelectedFile = 52110
        Selection = 10129
        StartImporting = 4755
        Upload = 147743
        Logo = 159368
        ConnectToSMTPServer = 51198
        Port = 135608
        SMTPOnNetwork = 159367
        SMTPOnServer = 159366
        SMTPServer = 51259
        [Using] = 20469
        CreateUser = 104869
        MainLanguage = 155236
        Role = 159365
        SelectLanguage = 147733
        SiteLanguage = 158577
        SupplierDetails = 132739
        CurrencyConversion = 150341
        'MainGroup = 159384  ' PROPERTY
        SelectColor = 133519
        Group = 134182
        Address = 27020
        City = 24153
        CreateSupplier = 132737
        Fax = 10524
        ModifySupplier = 132738
        Note = 10125
        Phone = 27050
        Position = 24152
        ReferenceName = 3305
        Representative = 3306
        State = 132740
        Supplier = 10990
        URL = 132741
        Zip = 152146
        '''''''''''''''''''''''''''
        RecomputingPrices = 133241
        Unable_To_Delete_marked_Items = 134111
        New_ = 24050
        Merchandise_Cost = 1080
        Selling_Price = 1090
        Merchandises = 1260
        Remark = 1280
        Price = 1290
        Wastage = 1300
        Category = 1450
        Calculated_Price = 1485
        Date_ = 1500
        For_ = 3140
        Percentage = 3150
        Const_ = 3161

        '--JBB 01.06.2011
        Name = 3205
        '--

        Unit_Price = 3215
        Picture = 3230
        Password = 15510
        Roles = 159951
        Recipe = 132541
        Users = 4865
        Unit = 5100
        Format = 5105
        Number = 5500
        Ingredients = 5590
        Preparation = 5600
        Amount = 5720
        Currency = 6390
        Procedure = 8220
        Delete = 8397
        Copy = 10103
        Search = 10121
        Tax = 10363
        Main = 31380
        Units = 20709
        Calculate = 24027
        Transfer = 24129
        Edit = 24150
        Results = 31755
        Language = 51086
        Details = 51123
        Email = 51257
        Username = 51261
        YieldUnit = 51392
        Close = 52307
        Selected_Keywords = 54710
        Keywords = 54730
        Qty = 55220
        Total_Tax = 132552
        Create_A_New_Merchandise = 132559
        Create_A_New_Recipe = 132597
        Net_Qty = 132614
        AutoConversion = 132630
        Gross_Qty = 132736
        Login = 132789
        Configuration = 156980
        Filter = 132848
        Summary = 132987
        New_Password = 133075
        Confirm_New_Password = 133076
        SellingPricePlusTax = 133365
        Suggested_Price = 133692
        Share = 143001
        Unshare = 143002
        Remove = 147652 '('Remove = 147652 'AGL Cleanup) ' RJL - swissarmy :02-10-2014
        DeleteExistingMarksFirst = 8397 '171684 'TODO: replace enum description with "Delete"
        Copyright = 156996
        Footer = 51097
        Please_login_your_appropriate_username_and_password = 171687
        Please_login_your_username_and_password = 151907

        Sign_In = 151910
        Sign_Out = 151911
        Submit = 155052
        Home = 155205
        Ownership = 136018
        Active = 156938
        Not_Active = 155994
        Email_Address = 155996
        Contact_Us = 156015
        Include = 156356
        Cancel = 24028
        ListOfOwners = 159977
        Save = 31098
        General_Settings = 137070
        Display = 132989 '24105
        Thumbnail = 132930
        List = 3234
        Private_ = 156955
        Hotels = 156957
        Shared_ = 156959
        Submitted = 156960
        Unsubmitted = 156962
        Set_Price = PurchasingSetOfPrice
        Prices = 156963
        Complement = 132565
        FindIn = 156964
        Yields = 156965
        Records_Affected = 156966
        Error_Date = 12525
        Error_Invalid = 134571
        Error_Incomplete = 151299
        Error_InvalidPictureFormat = 156968
        AddPicture = 156969
        Enter_Category_Informationn = 156970
        Enter_SetPrice_Information = 156971
        Enter_Keyword_Information = 156972
        Parent = 156978
        Enter_Unit_Information = 156973
        Enter_Yield_Information = 156974
        About_Recipe = 156975
        About_Merchandise = 156976
        'About_Ingredient = 171692
        About_ContactUs = 156977
        Yield_Name = 51294
        Unit_Name = 51092
        Name_of_Category = 6002
        Name_of_Keyword = 156979
        Tax_Rates = 156981
        Sorry_No_Result_Were_found = 156983
        Search_Results = 156982
        Invalid_Username_Or_Password = 156984
        already_exists = 156986
        was_saved_succesfully = 156987
        Source = 3721
        Invalid_value = 134571
        Price_for_the_unit_not_defined = 157002
        Same_unit_already_defined = 132719
        Tax_used = 157020
        Yield_Used = 133319
        Unit_Used = 133289
        Keyword_Used = 132779
        Categor_Used = 132571
        Modify_Merchandise = 29771
        Modify_Ingredient = 132861
        Modify_Recipe = 132554
        Please_wait = 6470
        Password_accepted = 115610
        All_Transfers_Successfully_Done = 138402
        Please_wait_Updating_Merchandise_Price = 157033
        'Please_wait_Updating_Ingredient_Price = 171693
        Invalid_Email = 151918
        Month = 157038
        Day = 31800
        Year = 157039
        No_Keywords_available = 157040
        ERR_PAGE_ACCESSDENIED = 157041
        ERR_SQL_NOTFOUND = 147647
        ERR_PAGE_UNKNOWN = 28000
        Sub_Recipe = 20200
        'RecipeBeingUsedAsaRecipe = 143508 'AGL Cleanup
        RecipeBeingUsedAsASubRecipe = 171683

        RecipeCanBeUsedAsaRecipe = 133207
        Weight = 133208
        Merge = 132667
        SaveConfirm = 157049
        Confirm = 156870
        CancelConfirm = 157056
        Adapt_the_QTY_to_the_new_yield = 3320
        Updated_Items = 14884
        Operation_failed = 30210
        SuccessfullyD_Deleted = 150333
        Marked_Shared = 157057
        Succesfully_updated = 31085
        Help = 110114
        All = 7181
        Treeview = 152004
        Help_Summary = 157076
        Confirm_Delete = 51402
        Cannot_Transfer_Record_Not_Submitted = 157079
        Cannot_Delete_marked_items = 157084
        Error_WastageValue = 157233
        SiteLanguageNotYetAvailable = 29170
        WebSite = 156669
        CurrencySymbol = 136905
        Description = 9920
        Currency_used = 157268
        SetOfPrice_Used = 157269
        MoveUp = 132669
        Movedown = 132670
        Print = 10970
        Refresh = 135990
        Cannot_share_items_notsubmitted = 157273
        Exchange_Rate = 157274
        Select_One_Item_to_Use_for_merging = 157275
        Successfully_meged = 157276
        Preview = 4891
        FoodCost = 31370
        ImposedSellingPrice = 5530
        Margin = 24068 'margin cost
        TotalCost = 157277
        Country = 56130
        Select_at_least_one = 157297
        Not_Available = 29170
        EnterUserDescription = 157299
        Guide_Enter_Password = 157300
        Guide_UploadPicture = 157301
        Guide_SearchAndAddIngredient = 159426
        'Guide_IngredientPricing = 171696
        Guide_MerchandisePricing = 157303
        Guide_KeywordsAssigning = 157304
        FileSizeLimit = 133045
        UserInformation = 132638
        PleaseSelectAnItem = 157305
        Confirm_Purged_UnusedCategories = 133325
        Purge = 132668
        Cannot_Merge_More_SystemUnits = 133290
        Cannot_Merge_More_SystemYields = 133315
        Previous = 132683
        [Next] = 24271
        ConfirmRefresh = 157334
        Cannot_Delete_SystemUnits = 133295
        Cannot_delete_SystemYields = 133314
        Modify_User = 4870
        MEssages_perpage = 157339
        Quick_browse = 157340
        Oneachpage = 157341
        RecordModifiedByOtherUser = 157342
        RecordDeletedByOtherUser = 157343
        Administrator = 15504
        Sortbyitemname = 151427
        SubmitToMainOffice = 157345
        NotSHared = 157346
        Confirm_CancelChanges = 134525
        Text = 10104
        AddText = 6055
        Deleted = 10399
        ModifyText = 6056
        SavingInProgress = 18460
        Support = 156012
        Browse = 52012
        WebColors = 133060
        UploadLogoForWebSite = 133057
        ChangeInfo = 158694
        SetBrand = 174733
        SetSupplier = 174734
        Menu = 1400
        CheckStatusOfRequest = 158019
        MenuCard = 3300
        ExecutiveChef = 159361
        MainGroupChef = 159360      ' PROPERTY CHEF
        Incomplete = 159035
        GroupChef = 3200
        Yes = 51204
        No = 7010
        Item_Used = 159362
        Enter_Brand_Information = 159363
        NoSelectedItem = 171689
        NoItemSelected = 155601

        Brand = 159364
        StandardizeCategories = 132896
        Export = 3800
        Replace = 14819
        Approved = 158952
        Not_Approved = 158953
        Clear = 149774
        TimeZone = 159445
        RecipeExchange = 155642
        This_is_a_system_generated_email = 159511

        ' JAN 12, 2006
        Youdonothaverightstoaccessthisfunction = 159918
        System = 7755
        EnterYourPassword = 15615
        PleaseSelectFromTheList = 132714
        Variation = 133099
        'IngredientDetails = 170780
        MerchandiseDetails = 157310
        RecipeDetails = 133100
        MenuDetails = 133101
        ShoppingListDetails = 51500
        [Const] = 3161
        RecipeNumber = 3195
        First = 133081
        CalcmenuWeb = 171706
        CalcmenuWeb2006 = 159946
        Calcmenu = 151438
        RecipeNet = 151437
        YouCannotMergeTwoOrMoreSystemUnits = 133290
        EnterDefaultSiteName = 159967
        EnterDefaultWebsiteTheme = 159968
        EnableGroupingSitesByPropertyByAdmin = 171707
        EnableGroupingSitesByPropertyByPropertyAdmin = 159969
        RequireUsersToSubmitInformation = 159970
        EnterTheTranslationForEachCorrespondinLanguage = 159971
        FieldMarkedWIthAsteriskManadatory = 151916
        ChooseBasicListToStandardize = 160180
        SelectAvailableLanguagesForIngredientRecipe = 171708
        SelectAvailableLanguagesForMerchandiseRecipeMenu = 159974

        SelectOneOrMorePriceGroupsForIngredientRecipe = 171709
        SelectOneOrMorePriceGroupsForMerchandiseRecipeMenu = 159975
        CheckItemsToInclude = 159976
        ChooseFormatBelow = 159978
        ChooseBasicListToPurge = 159979
        ChooseBasicListToRecalculate = 174539 ' NBG 9.28.2015
        AllowPrintShowppingList = 51532 'NBG 9.28.2015
        TheFollowingAreSharedSitesForThisItem = 159981
        MoveMarkedToANewSource = 159982
        Excel = 151854
        SelectedIngredientsShouldHaveTheFFUnits = 160005
        MoreActions = 160009
        YouHaveAttemptedToLoginTimes = 160035
        ThisAccountHasBeenDeactivated = 160036
        ContactYourSystemAdminToReactivateThisAccount = 160037
        WelcomeName = 155170
        RowsPerPage = 160045
        IngredientQuantities = 160047
        LastAccessed = 160048
        ReceivedFile = 160049
        FailedToReceive = 160051
        LinkedTo = 149645
        InvalidQuantity = 134194
        QuantityMustBeGreaterThan0 = 160055
        CreatNewRecipe = 132597
        CreateNewSubRecipe = 160056
        YouLoginHasExpired = 160058
        YourEntryRequiresApproval = 160067
        ClickTheButton = 160068
        RequestApproval = 155052
        NameExists = 155713
        AlreadyExistingRequestForThisEntry = 160072
        SelectUnit = 160074
        YouHaveNewRequestsToApprove = 160082
        DeleteSelectedFromList = 171714
        RemovedSelectedFromList = 160091
        ReplacedIngredientUsedInRecipesAndMenus = 160096
        CreateNewMenuCard = 159388
        ChangeSupplier = 132738
        ChangeCategory = 6000
        ChangeSource = 132621
        ChangeBrand = 159990
        ItemsWaitingForApproval = 159112
        MarkedItemsToBeProcessed = 160070
        CreateANewMerchandise = 132559
        CreateANewIngredient = 171673
        CreateANewRecipe = 132597
        CreateANewMenu = 132557
        ModifyMerchandise = 29771
        ModifyRecipe = 132554
        ModifyMenu = 101600
        WelcomeTo = 160107
        ShouldYouHaveQuestions = 156977
        MerchandiseDescription = 156976
        BlockedIPList = 159472
        LanguageDescForTranslatingIngredientsRecipes = 171717
        LanguageDescForTranslatingMerchandiseRecipes = 160118
        [Property] = 159384
        Site = 159751
        IngredientDescription = 160151
        ExchangeRate = 157274
        DeleteUnusedTexts = 160157
        PrintIngredientListDesc = 171728
        PrintMerchandiseListDesc = 160159
        PrintRecipeListDesc = 160162
        PrintMenuList = 2700
        PrintMenuDetailsDesc = 160163
        ModifyOrPreviewSavedMenuCards = 160170
        SelectionOfMerchandise = 160172
        PurgeTexts = 151389
        StandardizeTexts = 132912
        BottomMargin = 133166
        FontSize = 133168
        LeftMargin = 133163
        RightMargin = 133164
        TopMargin = 133165
        LineSpacing = 143509
        Papersize = 133161
        WhatToPRint = 133108
        Style = 10135
        MyProfile = 160038
        MerchandiseList = 5270
        'MerchandiseDetail = 157310
        'MerchandisePriceList = 160863 'AGL
        MenuList = 132939
        MenuDetail = 133101
        RecipeList = 132933
        RecipeDetail = 133100
        ShoppingListDetail = 51500
        'MerchandiseNutrientList = 134176 'AGL 

        MenuNutrientList = 134178
        RecipeNutrientList = 134177
        MenuCosting = 133123
        Modern = 133127
        MenuDescription = 160103 ' about menu
        MenuDescriptionMerch = 172030 'AGL 2012.10.26 - CWM-1310
        Menu_Description = 133124   ' translation of the phrase Menu Description
        RecipePreparation = 133096
        RecipeCosting = 133097
        EgsStandard = 133126
        TwoColumns = 133128
        EgsLayout = 147713
        SmallPictureQuantityName = 133172
        SmallPictureNameQuantity = 133173
        MediumPictureQuantityName = 133174
        MediumPictureNameQuantity = 133175
        LargePictureQuantityName = 133176
        LargePictureNameQuantity = 133177
        OutputDirectory = 159942
        '------------------
        'mcm 18.01.06
        Profit = 5801
        FC = 31375
        per_serving = 5795
        SellingPricebyServing = 105360
        Menu_No = 133349
        Recipe_No = 133144
        Quantity = 1310
        ImposedSellingPricebyServingPlusTax = 133352
        ImposedSellingPricebyServing = 133353
        Nutrients = 13060
        NA = 144688
        Per100gOR100mlat100Percent = 144687
        PerYPercentAt100 = 144686
        ItemsForyNetQuantity = 133350
        ProductDescription = 26100
        NoSource = 133326
        IngredientsForNServing = 133351
        IngredientsFor = 158157
        Waste = 143008
        Net = 135070
        Gross = 5741
        per_ = 155862
        VAT_ = 151404
        Nutrient1Yield = 144684
        Nutrient100G = 144682
        Nutrient100ml = 144689
        Page = 5610
        PerYieldUnitAt100 = 144685
        Both = 24044
        '-----------------------
        PurchasingSetOfPrice = 160353
        SellingSetofPrice = 160354
        Invalid_v = 161110

        GlobalOnly = 159274 'DLS June252007
        UploadConfigDefinition = 161180
        HostName = 161181
        Directory = 11060

        DatabaseIsNotCompatible = 158734
        ViewMyRecipes = 161132

        GDA = 161276
        GuidelineDailyAmounts = 161275

        English = 7270
        French = 7250
        Italian = 7280
        German = 7260
        Dutch = 157515
        Chinese = 158868

        Without = 161279
        [With] = 54295
        UsedAsIngredient = 159468
        NotUsedAsIngredient = 159469
        Complete = 52970
        NotDefined2 = 161291
        Defined = 161292
        NoGroup = 144582
        SelectAll = 24269
        DeSelectAll = 24268
        GoBackTo_S = 160776
        Persons = 155842
        CreateANewProduct = 104835
        ModifyaProduct = 104836
        AssignedKeyword = 158349
        DerivedKeyword = 158350

        Temperature = 161484
        ProductionDate = 161485
        ConsumptionDate = 161486
        Days = 31700
        Printer = 7030
        Tagesproduckt = 161487
        ConsumeBefore = 161488
        FreshEnjoy = 161489
        InfoAllergens = 161490
        AssignToAllMarked = 161491
        NoDishesFound = 21550
        oftext = 24011
        TempText = 161494
        Usernamealreadybeingused = 132640 'DLS

        Unassignkeywords = 161777
        Assignunassignkeywords = 161778
        Assignunassignallergens = 174435
        Recalculate = 160880
        RecalculateAndSaveYield = 176495
        Breadcrumbs = 161779
        MonitorBreadcrumbs = 161780
        UnwantedKeyword = 161781
        PrintLabels = 161782
        ProcedureTemplate = 161783
        Student = 161784

        Ingredientnutrientvaluespers = 161785
        Ingredientnutrientvaluesper100gml = 161786
        ExporttoExcel = 155926
        ApplyTemplate = 161787
        Areyousureyouwanttoreplaceo = 135969
        SetofPrice = 156961
        LastRecipe = 132934
        LastMenu = 132937
        AssignedDerivedKeywords = 161788
        Owner = 132616

        'DLS ***********************
        ValidateAll = 161468
        AddRows = 161823
        PasteFromClipBoard = 161824
        ThereAreNoUnlikedIngredientToBeVal = 171745
        ThereAreNoUnlikedMerchandiseToBeVal = 161825
        ChooseOther = 161826
        NewPrice = 8514
        DefaultPriceUnit = 161827
        ChooseFromExistingUnits = 161828
        AddThisNewExistingUnits = 161829
        ItemValidated = 161830
        LetMeEditIngredientBeforeAdding = 172217 'JTOC 27.12.2012 --171746
        LetMeEditMerchandiseBeforeAdding = 161831
        placeincomplement = 161832
        Allitemshavebeenvalidated = 161833
        Pleasechecktheprices = 161834
        Cut = 161835
        Paste = 161853
        AddtoRecipe = 161837
        Order = 10447
        Replaceexistingingredients = 161838
        NoIngredientsFound = 161839
        Areyousureyouwanttodeletepercentn = 132672

        'LinkToIngredientOrSubRecipe = 171747
        LinkToMerchandiseOrSubRecipe = 161841
        'AllItemsAreNowLinkedToIngredientRecipe = 171748
        AllItemsAreNowLinkedToMercRecipe = 161842
        ItemIsNowLinkedToIngredientRecipe = 171749
        ItemIsNowLinkedToMerchRecipe = 161843

        StoringTime = 161844
        StoringTemperature = 161845
        AddStep = 161986
        ItemOf = 161987
        LinkedProducts = 161988
        NotLinkedProducts = 161989

        DRAFT = 161855
        FORAPPROVAL = 159112
        CreatedBY = 158851

        _And = 27056
        _Or = 8990
        InsertHere = 162222
        DidYouMean = 162235
        ImportRecipes = 162276
        ImportIngredientCSVSupplierNetwork = 171704
        ImportMerchandiseCSVSupplierNetwork = 159264
        Notes = 162282
        Closed = 134826
        RecipeToManyIngredients = 159681
        Person = 147773

        January = 146043
        February = 146044
        March = 146045
        April = 146046
        May = 146047
        June = 146048
        July = 146049
        August = 146050
        September = 146051
        October = 146052
        November = 146053
        December = 146054

        Proposal = 151500
        NoOfProposal = 162205
        Embassy = 162203
        BusinessName = 162212
        BusinessNumber = 162213
        PriceAvailable = 162214
        UploadLogo = 162215
        Settings = 162216

        AND_ = 27056
        OR_ = 8990

        GrossMargin = 135257
        ImposedFactor = 155260
        ImposedFC = 156060
        ImposedProfit = 156061

        MenuPlan = 31732 'VRP 03.12.2008
        Street = 162340 'VRP 04.12.2008
        Go = 162386 'VRP 16.12.2008
        DeleteBreadcrumbsUponLogin = 171750
        RemoveBreadcrumbsUponLogin = 162530 'VRP 14.01.2009
        RecordDoesNotExist = 28483 'VRP 20.01.2009
        Grossmargininpercent = 161583 'DLS
        Netmargininpercent = 162955 'DLS

        SmallPortion = 161766 'VRP 23.02.2009
        LargePortion = 161767 'VRP 23.02.2009

        PackagingFactor = 175291 'ECAM
        PackagingInktVerp = 175292 'ECAM
        PackagingNet = 175293 'ECAM


        TheYieldQuantityChanged = 162198
        TheYieldChangeContinueSaving = 162199

        ADDPrice = 132900
        CopyPriceList = 163032

        Checking = 155995
        TotalErrors = 156784
        ImportationDone = 51174

        KeywordNotFound = 163046
        CostForTotalServings = 163057
        CostForServing = 163058

        ImposedSellingPriceTax = 132553
        AllProductsForInventories = 138031
        ProductsFromMarkedCategories = 138032
        ProductsFromMarkedLocations = 138033
        ProductsFromMarkedSuppliers = 138034
        ProductsFromOneOrMorePreviousInventories = 138035
        SelectWhichProductsYouWantForThisInventory = 138030

        LastPrice = 135283
        WeightedAveragePrice = 156542
        InventoryPriceUsedForTheProductPreviously = 147381
        PriceOfDefaultSupplier = 157281
        IfProductNotHaveADefinedPriceUseDefaultSupplier = 158410

        CreateInventory = 136230
        ModifyInventory = 136231

        PriceComparison = 161116
        FoodCostIn = 163060
        ImposedFoodCostIn = 163061
        StockValue = 135235
        RefNumber = 135100
        QtyPrevInventory = 160414
        QuantityInventory = 135110
        CurrentlyOpenedInventories = 136100
        NoOfItems = 136115
        OpenedOn = 136110
        InProgress = 1146
        InventoryStartedOn = 134021
        InventoryAdjustment = 124164
        SetQtyOnHandAsQtyInventory = 158946
        AddAProductToTheCurrentInventory = 136213
        DeleteAProductFrInventory = 171751
        RemoveAProductFrInventory = 136214
        ShowListOfAdjNeeded = 136212
        AddANewLocForProduct = 136215
        DeleteSelectedLocForProduct = 171753
        RemoveSelectedLocForProduct = 136216
        DeleteQtyForSelectedProdLoc = 171752
        RemoveQtyForSelectedProdLoc = 136217
        ResetQtyToZero = 155861
        NotApplicable = 157336
        Contents = 136030
        Liter = 133147
        InvalidAccountCode = 143981

        FeedBack = 169318
        Degustation_Development = 169310

        Version = 104862 ' JBB 12.22.2010
        Parents = 159944 ' JBB 12.28.2010 

        Exportto = 160232

        '// DRR 06.08.2011
        Ingredient = 133248
        IngredientList = 170779
        IngredientDetail = 170780
        IngredientNutrientList = 170781
        IngredientCategory = 170782
        IngredientKeyword = 170783
        IngredientPublishedOnTheWeb = 170784
        IngredientNotPublishedOnTheWeb = 170785
        IngredientCost = 170786

        'JBB 07.04.2011
        CookMode = 170849
        CookModeOnly = 170850
        AllRecipes = 133115
        NoneCookModeOnly = 170851
        Difficulty = 157160
        Budget = 167719
        Standard = 151286
        ShowOff = 170852
        QuicknEasy = 170853
        ChefRecommended = 170854
        'AGL 2012.10.18 - CWM-1593
        '--
        MovetoNewRating = 171973
        MovetoNewStandard = 170860
        '--
        High = 158849
        Low = 158850
        Medium = 157026
        Simple = 52960
        Moderate = 170855
        Challenging = 170856
        Gold = 170857
        Silver = 160894
        Bronze = 170859
        Unrated = 170858

        '-- JBB 01.06.2012
        LeadIn = 171219
        ServingSize = 55011
        NumberofServing = 171220
        TotalYield = 171221
        Attachment = 151436

        '// DRR 02.02.2012
        PreparationMethod = 171301
        Tips = 171302

        '-- JBB 06.18.2012
        BrandSiteSuccessfulExport = 150009

        '-- JBB 08.21.2012
        CheckedInByAnotherUser = 171597

        'JTOC 05.09.2012
        Placement = 171616
        Publication = 171617
        DigitalAsset = 171618
        BrandSite = 171619
        ExternalWebSite = 171620
        Rating = 147729

        'AGL Merging 2012.09.21
        'TotalIngredientCost = 171744
        TotalMerchandiseCost = 161578

        IngredientsAndProcedure = 171662
        AdditionalNotes = 171804
        ServeWith = 171764

        'JTOC 24.09.2012
        SubTitle = 167385
        Image = 133475
        PrimaryBrand = 171670
        SecondaryBrand = 171672
        Nutrition = 171774
        RecipeStatus = 171668
        WebStatus = 171669
        [Protected] = 171234
        [Date] = 1500
        ListOfCookbook = 171782
        CookbookName = 171700
        CookbookList = 171813
        Expensive = 171832
        Cheap = 171833
        Cookbook = 171834 '171755
        Ratings = 171840
        ShowAll = 167346
        Project = 170386
        ListOfCookbooks = 171863
        SelectedProject = 171783
        'SelectedCookbook = 171864

        ' RBAJ-2012.10.03
        Promotion = 171866
        Kiosk = 171481

        'AGL 2012.10.04
        ItemIsBeingUsed = 171152

        ' RBAJ-2012.10.05
        AllowCreateMenuCard = 171877
        CreateProtectedCopy = 161125
        Protect = 171802
        Unprotect = 171803
        AddSupplierProduct = 156503
        SetTaxProduct = 171805
        AddLocationProduct = 171879
        SetDefaultLocationProduct = 171882
        SetDefaultProductionLocationProduct = 171884
        SetDefaultOutputLocationProducts = 171885
        SetProductMinimumMaximumQuantityStock = 171886
        SetProductMinimumMaximumQuantityOrder = 171887
        SetProductDefaultQuantityOrder = 171888
        SetProductInventories = 171889
        SetProductRawMaterial = 171890
        SetProductUsedInputOutput = 171891
        SetProductAutomaticTransferOutletBeforeOutput = 171892
        SetProductExcludeAutomaticOutputOperation = 171893
        CreateTransferRequestAfterOutputProduct = 171894
        DoNotLinkToCalcmenu2009 = 171895
        SetProductLastSupplierDefaultSupplier = 171896
        AddProductDetailForSite = 171897
        RemoveProductDetailForSite = 171898
        AssignUnassignProject = 171899
        MoveMarkedRecipeWebStatus = 171789
        AssignPromotion = 171901
        ExportListToCSV = 171902
        ExportListToExcel = 171903
        ExportListToWord = 171904
        ExportListToPDF = 171905
        RegisterRecipeNetWeb = 171906
        ProductionPlace = 171907
        ProductionPlaceDetails = 171908
        MenuEngineeringEvaluateRecipe = 171909
        MenuEngineeringEvaluateMenu = 171910
        LoadMenuCardsList = 160169
        EditPreviewMenuCards = 171912
        CustomizeRights = 171913
        Actions = 171914
        ImageManagement = 171915
        SalesRecordsNotSuccessful = 171916
        SalesItemLinking = 171917
        SalesItemLinkingDesc = 171918
        ShowStepsProcedure = 171919
        AutoNumbering = 171920
        ProcedureStyles = 171921
        UpdateToLatestDatabaseVersion = 171922
        ImportXMLTXCTXS = 171923
        Region = 162315
        SubRegion = 171924
        LinkToProductRecipeMenu = 171925
        ErrorInItemsSharing = 171926
        CannotSwitch = 171927
        CorruptConfig = 171928
        ProcedureError = 171929
        FistRan = 171930
        InvalidCodeList = 171931
        InvalidCodeSite = 171932
        InvalidListType = 171933
        InvalidStockType = 171934
        InvalidSupplier = 171935
        InvalidTranMode = 171936
        ItemClosed = 171937
        ItemLocked = 171938
        ItemNoInventory = 171939
        ItemNoLocation = 171940
        MergingMultipleGlobalItems = 171941
        OneItemNotDeleted = 171942
        RequestInProcess = 171943
        RequestNotInProcess = 171944
        UsedAsDirectIO = 171945
        NothingWasDone = 171946
        SiteHasNoUser = 171947
        SalesItemNumberAlreadyExists = 171948
        InvalidPromoCode = 171949
        Comments = 157714
        WineType = 162338
        Producer = 162314
        SalesNotImported = 156695
        RemoveSupplier = 162907
        Compare = 170126
        Outlet = 24257
        NutrientSet = 171756
        MerchandiseAndProcedure = 171950
        UnassignedBrandsFromMerchandise = 171951
        AssignedBrands = 171780
        AutoCalculation = 171963
        PerYield = 171964
        MerchandiseQuantities = 171965
        UnsavedItems = 171240
        KioskList = 171967

        'AGL 2012.10.15 - CWM-1580
        BrandAndWebsites = 171971

        'AGL 2012.10.17 - CWM-1591
        EnterCookbookInformation = 171986
        EnterProjectInformation = 171985

        'AGL 2012.10.17 - CWM-1592
        UnwantedBrand = 171988
        UnwantedPrimaryBrand = 171989

        ' RBAJ-2012.10.17
        ManageKiosk = 171997

        'AGL 2012.10.17 - CWM-1608
        AssignCookbook = 171796
        UnassignCookbook = 171797
        AssignUnassignCookbook = 171899
        TheRecipeWillBeAssignedToTheSelectedCookbook = 171798
        TheRecipeWillBeAssignedToTheSelectedProject = 171993
        TheRecipeWillBeUnAssignedToTheCookbook = 171799
        TheRecipeWillBeUnAssignedToTheSelectedCookbook = 173362 ''AMTLA 2013.12.03  CWM-9403
        TheRecipeWillBeUnAssignedToTheSelectedProject = 171994
        SetCookBook = 171806
        SetProject = 171995

        'JTOC 18.10.2012
        Excellent = 172001
        Great = 172002
        PleaseEnterRecipeTitle = 171758

        'AGL 2012.10.18 - CWM-1593
        RemoveFromKiosk = 171972

        'JTOC 19.10.2012
        DigitalAssetAndTime = 172000
        PictureAndTime = 171999
        AssignPublication = 172006
        SetRecipePlacement = 171808
        SetRecipePublication = 171996
        Footnote1 = 172003
        Footnote2 = 172004
        SubName = 172007

        'AGL 2012.10.24 - CWM-1804
        Sign = 172021

        'AGL 2012.10.25 - CWM-1772
        AssignUnassignKiosk = 172026
        AssignKiosk = 172027
        UnassignKiosk = 172028

        'JTOC 29.10.2012
        PleaseEnterRecipeName = 172032
        'PleaseEnterRecipeSubTitle = 172033
        PleaseEnterRecipeSubName = 172034

        'AGL 2012.10.30 - CWM-1963
        SelectACookbook = 172039
        SelectAProject = 172040

        'AGL 2012.10.30 - CWM-1966
        ItemExistsWithSameDate = 172041

        'JTOC 31.10.2012
        'MoveMarkToANewRecipeAndWebStatus = 171789
        SetRecipeAndWebStatus = 171807
        MoveMarkedToNewRecipeStatus = 172042
        SetRecipeStatus = 172043
        ReplaceRecipeAndWebStatus = 172044
        ReplaceRecipeStatus = 172045

        'AGL 2012.10.31
        WantedMerchandise = 172046
        UnwantedMerchandise = 172047
        UsedAsMerchandise = 172048
        NotUsedAsMerchandise = 172049

        'JTOC 06.11.2012
        FullyTranslated = 171667
        Profile = 171666
        FullyTranslatedIn = 172127
        LastModifiedBy = 172128
        SelectTheItemToInlcude = 171788
        AddTagsOrRecipeNamesBelow = 172130
        SellingPrice = 151403
        ImposedNutrients = 171777
        Per100gmlat100Percent = 172131
        PerKGat100Percent = 175247
        At = 5210
        RecipeTime = 171719
        DateCreated = 134174
        DateLastModified = 162257
        By = 109730
        Validate = 161467
        IncludeSub = 172132


        'AGL 2012.11.08 - 
        NoRecordsToDisplay = 171610

        'AGL 2012.11.09 
        CheckedOutItems = 171664

        'AGL 2012.11.12
        ApplyWastage = 172136

        'JTOC 13.11.2012
        Constant = 143506
        GrossProfit = 5800
        MetricQty = 172137
        ImperialQty = 172138
        MetricGrossQty = 172139
        ImperialGrossQty = 172140

        'AGL 2012.11.14 - CWM-2235
        Apply = 167515

        'AGL 2012.11.15 
        AllowMultipleWindows = 172172
        RecipePreview = 172173

        'JTOC 20.11.2012
        UnitIsRequired = 172174
        PackagingMethod = 172179
        RecipeCertification = 172180
        Origin = 172181
        ConservationTemperature = 172182
        Information = 19000

        'JTOC 12.12.2012
        SpecifiedUnitIsInactive = 172223

        'AGL 2012.12.13
        ShowOnlyMyCheckedOutItems = 172221
        CheckIn = 172225

        'JTOC 26.12.2012
        UploadMediaFiles = 171792
        EnterDefaultProcedureTranslationText = 171791
        EnterDefaultProcedureText = 171790
        SearchIngredientByNameOrPartOfTheNameToAddQuicklyEnter = 171793
        UploadFile = 172208

        'JTOC 27.12.2012
        Procedure2 = 51280
        Instruction = 172240
        NutritionalBasis = 171779
        CalculatedNutrients = 171778
        DisplayNutrition = 171776
        ImposedNutrientType = 171775
        UnassignedBrandsFromIngredients = 172213
        PromoteTheFollowingBrandsInWebsites = 171865
        Comment = 147750
        AddComment = 171784
        PostedBy = 171785
        DatePosted = 171786

        'JTOC 07.01.2013
        InvalidYield = 172276
        SetRating = 172279

        ' RBAJ-2013.01.07
        IsIncorrect = 172277

        'JTOC 10.01.2013
        InvalidTaxRate = 135952

        'JTOC 24.01.2013
        OfVersion = 172340
        Merchandise = 167149
        CanBeUsedAsParent = 172318
        AlternativeIngredient = 172227
        MainPurchasingSetPrice = 161300
        ViewPictures = 171716
        RemoveVersion = 171723
        KeepLengthOfPrefix = 162358
        LogonAsDifferentUser = 172284
        Hour = 27220
        Minute = 155851
        User = 143014
        Action = 143995

        'JTOC 25.01.2013
        PrintType = 171233
        CategoryName = 51131
        ActivateAutoNumber = 149765
        SupplierNetwork = 159175
        Example = 162357

        'JTOC 31.01.2013
        ShoppingListNameAlreadyExists = 172403

        'JTOC 01.02.2013
        PleaseEnterYieldNumber = 172404
        PleaseSelectPlacement = 172405
        PleaseEnterValidYield = 172406
        AreYouSureYouWantToCreateNewVersion = 172407
        ClickToRedirectToHome = 172408
        ProtectedCopy = 172409
        WorkWithProtectedCopies = 171242
        IncludeWhenPrintingAndExporting = 171243
        FactorOfPurchasingSetOfPriceToMainPurchasingSetOfPrice = 172410
        FactorOfSellingSetOfPriceToMainPurchasingSetOfPrice = 172411
        ClientCode = 172412
        AddNewRecord = 172413
        UpdateExistingRecord2 = 172414
        DisplayAllSites = 172415
        IngredientsName = 172416
        ThereIsNoPrintProfileAvailable = 172417

        ' RDC 02.12.2013
        IngredientComplement = 172528
        IngredientPreparation = 172529
        Times = 172327
        Footnotes = 172531
        ' RDC 02.19.2013
        RecipeTimeHour = 27220
        RecipeTimeHours = 172541
        RecipeTimeMinute = 155851
        RecipeTimeMinutes = 159460
        RecipeTimeSecond = 171658
        RecipeTimeSeconds = 172540
        RecipeTimeAnd = 27056
        ' RDC 02.22.2013
        RecipeBrands = 172547
        RecipePlacements = 172548
        RecipePublications = 171617
        ' RDC 02.25.2013
        RecipeProcSequenceNo = 172566
        ' RDC 02.26.2013
        RecipeNotes = 169792
        RecipeAdditionalNotes = 171804
        ' KGS 06.29.2020
        RecipeSubname = 172321
        ' RDC 03.08.2013
        ImageFileUploadSuccessful = 172661
        ImageFileUploadFailed = 155517
        ' RDC 03.13.2013 - Issue CWM-3534 Fix
        UnableToPrint = 134119
        NoPrintProfileAvailable = 172417
        Selected = 167108

        'AGL 2013.02.13
        PleaseSelectN = 172530
        PleaseEnterPublicationName = 172532

        'JTOC 14.02.2013
        YouHaveNotChosenACategory = 5935
        YouHaveNotChosenASource = 172533

        'JTOC 20.02.2013
        SuccessfullyDone = 147071

        'AGL 2013.03.16 
        RecipeNumber2 = 172402

        'JTOC 22.03.2013
        MerchandiseNutrientList = 172691

        'JTOC 03.04.2013
        NoAssignedSiteForThisProperty = 172710

        ' RDC 04.04.2013 -CWM-3518 Fix
        PleaseEnterUsernameEmail = 172218

        'AGL 2013.04.18 
        DisplayAsFractions = 172740

        ' RDC 04.18.2013 - CWM-5350 Fix
        MetricQuantityGross = 172741
        MetricQuantityNet = 172742
        ImperialQuantityGross = 172743
        ImperialQuantityNet = 172744
        'AlternativeIngredient = 172227
        'HACCP = 132678

        'AGL 2013.04.23
        SpecificDetermination = 172764

        'JTOC 22.04.2013
        Verified = 172755
        ItemsAndProcedure = 172758

        'JTOC 24.04.2013 
        RecipeIngredientShoppingListQuantity = 172773

        ' RDC 04.29.2013 - CWM-5666 Fix
        NetQuantity = 5747
        GrossQuantity = 5746

        ' RDC 04.30.2013 - CWM-5517 Fix
        HighlightSection = 172794

        'JTOC 02.05.2013
        InvalidIngredientSequence = 172808

        'AGL 2013.05.20
        PageNofN = 160469
        ItemNofN = 161987

        'AGL 2013.05.21
        FirstPage = 160248
        PreviousPage = 158673
        NextPage = 158672
        LastPage = 160249

        'JTOC 28.05.2013
        ExternalWebsitesThatWillDisplayTheRecipe = 172965
        NutrientGroupForImposedNutrientValues = 172966
        UsedToDefineInWhichPublicationsWebsitesOrEventsTheRecipesHaveBeenMadeAvailableTo = 172967
        ThisGroupingIsUsedInViewingRecipesInAHierarchicalViewInTheRecipeList = 172968
        TheTimeForEachStageInPreparingTheRecipeUsersCanAssignTheseTimesToRecipe = 172969

        'JTOC 30.05.2013
        UsedToAutomaticallyGenerateAssignNumbersForMerchandiseAndRecipe = 172983

        'JTOC 05.06.2013
        WaitingForApproval = 172985
        SendRequestForApproval = 159088

        'JTOC 14.062013
        ThereAreNIngredientsWaitingForApproval = 172996
        Reject = 172997

        'AGL 2013.06.19
        NoPictureUploaded = 171794
        UploadImageFileOnly = 171795
        UploadLimitExceeded = 171760

        'JTOC 14.06.2013
        RequestRejected = 173044

        'JTOC 24.06.2013
        SendForApproval = 173047

        'AGL 2013.07.02
        RoleLevel = 173092

        ' RDC 07.09.2013 :  CWM-6986
        RecipeAddtionalNotes = 173107

        ' RDC 07.10.2013
        RecipeComment = 147750

        'AGL 2013.07.15
        PDFProductDeclaration = 173114

        ' RDC 07.30.2013 : Makes
        Makes = 172013

        ' RDC 08.07.2013 : Calculated Nutrient / Imposed Nutrient
        'CalculatedNutrient = 173135
        ImposedNutrient = 173137

        'AGL 2013.08.08 - 7434
        RoleManagementDescription = 173139
        DigitalAssetManagementDescription = 173138

        'AGL 2013.08.13 - 7162
        [Me] = 173142

        ' RDC 08.14.2013 : Nutrient Information and for
        NutrientInformation = 159934
        _For = 105200

        'AGL 2013.08.17
        UseCommasToSeparate = 173144

        'AGL 2013.08.24 
        ColumnFilter = 173145

        ' RDC 08.30.2013 : 173146 - Enter Digital Asset Information
        EnterDigitalAssetInfo = 173146
        EditDigitalAssetInfo = 174657
        'JTOC 10.16.2013
        SpellChecker = 14813

        'AMTLA 2013.10.22
        MoveMarkedItems = 173278
        Extension = 144655

        ' RDC 11.12.2013 : Added new translation for label Nutrient Computation
        NutrientComputation = 173288

        'AGL 2013.11.12
        RecipeID = 173289

        'JTOC 11.19.2013
        UnwantedSecondaryBrand = 173359

        'JTOC 12.17.2013
        QuantityType = 173363

        ' RDC 01.20.2014
        UpdatedBy = 171768
        ModifiedBy = 160416
        DateLastTested = 171769
        TestedBy = 172543
        DateDeveloped = 171770
        DevelopedBy = 172544
        FinalEditDate = 171771
        FinalEditBy = 172545
        DevelopmentPurpose = 171772

        ' RJL - swissarmy :02-10-2014
        Uploadpictures = 132606
        FooterReport = 171244
        Database_ = 135963
        NutrientSetInfo = 173155
        EditNutrientSetInfo = 174682
        Plural = 173159
        Filename_ = 51112
        One = 24030
        Time = 161577
        ListOption = 133196
        DetailsOption = 133222
        code = 30240
        ViewActualSize = 171237
        TotalRecords = 173152
        AddRecord = 173150
        UpdateRecord = 173151
        TotalImported = 173153
        TotalError = 173154
        Done = 52150
        Seq = 171765
        AutoCalculate = 171235
        ListFormat = 171762
        Bullet = 171763
        Metric = 7515
        Imperial = 7516
        Size = 19330
        Alcohol = 162318
        Vintage = 162319
        Importing = 156491
        Of_ = 167080
        UsedOnline = 168373
        NotUsedOnline = 171238
        AllCategories = 132617
        SelectorUploadFile = 159947
        Type_ = 124042
        ShowPercentageTranslated = 171241
        RecipeNutrients = 160263
        HeaderStyleOptions = 162232
        Template = 161710
        BackOffice = 162219
        Mode = 157030
        Steps = 172330
        ForceDeleteCategories = 171246
        Tabs = 173149
        PocketKitchen = 156941
        PortionSize = 169366
        Cook = 161286
        Current = 161663
        PleaseEnterAValidURL = 171759
        ExecutableFileNotPermitted = 171761
        Update = 151287
        Serving = 56420
        LegacyNumber = 171732
        FootNote = 167469
        Calculated = 172432
        Attributes = 172546
        Calories = 13000
        CaloriesfromFat = 55035
        DietaryFiber = 55090
        Calcium = 55110
        NutritionalInfo = 172542
        Others = 157755
        CopyCalc = 173161
        [Public] = 171236
        FooterAddress = 171245
        NutrientSetDefault = 173372
        EnergyNutrientDefault = 173419
        Singular = 173377
        AssignUser = 173376
        EqualsTo = 171014
        CostPerS = 171671
        PlsSelectAFiletoUpload = 162888
        AssignOwner = 171800
        ShareItems = 171801
        Video = 162418
        EqualTo = 173519
        PublicationDate = 173521
        CompareWith = 173523
        [true] = 173528
        [false] = 173527
        PSAOR = 173532
        ZipCode = 135055
        CnCImperialtoMetric = 173546
        CnCMetrictoImperial = 173547
        UPC = 172226
        'Basic = 159779
        'CheckAll = 173409
        'UncheckAll = 173410
        'ExpandAll = 151623
        MerchEncoding = 173411
        EncodeTranslation = 173429
        EncodeCosting = 173412
        DisplayCosting = 173430
        EncodeNutrients = 173414
        DisplayNutrients = 13065
        RecipeIngredientApproval = 173431
        Marking = 173432
        MerchandisePrint = 173418
        PrintNutrientList = 160086
        PrintPrice = 173433
        TextEncoding = 173448
        Translate = 155264
        RecipeEncoding = 173434
        YieldResizing = 173435
        CreateSubRecipe = 173436
        ModifyRecipeIngredient = 173437
        EncodeProcedure = 173438
        DisplayProcedure = 173439

        'NBG 2016.05.11
        DisplayAllergen = 175353

        EncodeNotes = 173440
        EncodeAdditionalNotes = 173441
        SetRecipeStatusToFinal = 173442
        PromoteRecipeBrand = 173443
        EncodeComments = 173444
        EncodeHACCP = 173445
        VerifyRecipeTranslation = 173446
        CompareRecipe = 173447
        CreateShoppingList = 172424
        RecipePrint = 173449
        MenuEncoding = 173450
        ModifyItems = 173484
        Sort = 150353

        ValidateYieldPercentage = 175579

        'KMQDC 6.11.2015 
        AssignAllergen = 174293
        MassChangeRecipeStatus = 174294
        ManageDigitalAsset = 174295
        ManagePasswordAndLogin = 174296
        ManageAlias = 174297
        OneQuantity = 173157
        ActiveAllergenManagement = 173778

        ManageMerchCategory = 173464
        ManageSite = 173462
        ManageProperty = 173461
        ManageNutrients = 173454
        ManageTranslation = 173457
        ManageUnits = 173482
        ManageUsers = 169557
        ManageNutrientRules = 173483
        ManageTax = 173394
        ManageBrand = 173463
        ManageSupplier = 173481
        ManageRecipeCategory = 173478
        ManageRecipeKeywords = 173468
        ManageMerchKeywords = 173465
        ManageMenuCategory = 173474
        ManageWebsiteProfile = 173458
        ManageSources = 173469
        ManageMenuKeywords = 173479
        ManageSMTPANS = 173455
        SystemConfig = 173451
        AccountConfiguration = 173460
        MerchConfig = 173536
        ToolsConfig = 173475
        RecipeConfig = 173467
        MenuConfig = 173473
        StandardizeBasic = 173480
        ManageNutrientSet = 173456
        ManagePublication = 173399
        ManageCookbook = 173402
        ManageTime = 173471
        PurgeBasic = 173477
        ManagePrintProfile = 173476
        ViewLicense = 173453
        ManageRoles = 173459
        ManageCurrency = 173452
        AccesstoDiffSites = 173661
        CompleteforDiffusion = 173582
        DoNotUse = 173162
        Final = 173164
        Invalid = 173583
        NeedsRefinement = 173166
        PackageDirection = 173167
        Prototype = 173169
        Retired = 173170
        SamplingRecipe = 173171
        UnderDevelopment = 173584
        UpdateinProgress = 173172
        BulkDirections = 173160
        Blue = 129711
        Cream = 173533
        Sunset = 173534
        Templates = 173535
        PleaseContactAdmin = 158834

        ''AMTLA 2014.02.24
        Portion = 173422
        ''AMTLA 2014.04.09
        SelectHere = 173623
        Basic = 159779
        CheckAll = 173409
        UnCheckAll = 173410
        ExpandAll = 151623
        CollapseAll = 151624

        'AGL 2014.05.21
        CopyrightText = 51050

        'AGL 2014.06.25
        [Alias] = 169515

        'MKAM 2014.07.02
        UnwantedAllergens = 173785
        WithoutAllergens = 173786
        WithAtLeastOneAllergen = 173787
        Contain = 161080
        Trace = 171309
        NonAllergen = 172386

        'AGL 2014.07.30
        AbbreviatedPreparation = 172332

        'AGL 2014.09.11
        PasswordAndLogin = 173865
        ThePasswordAndConfirmationPasswordMustMatch = 166088
        TheNewNasswordCannotBeTheSameAsTheOldOne = 170904
        YourPasswordWillExpireInPercentDDays = 170880
        TheNewPasswordCannotBeTheSameAsTheOldOne = 170897
        MinimumPasswordLengthIsPercentD = 173078
        PasswordHasExpired = 170902
        MinimumLengthOfPasswordMustBePercentDCharacters = 162890
        MinimumLength = 170876
        PleaseChangeYourPasswordToComplyWithTheStrongPasswordPolicy = 170881
        EnforceStrongPasswordPolicy = 170875
        KeepEmptyIfPasswordWillNotExpire = 170903
        PasswordReuse = 173866
        KeepEmptyIfPasswordCanBeReusedImmediately = 173867
        PasswordShouldHaveAtLeastOneUppercaseAndOneLowercaseLetter = 170877
        PasswordShouldHaveAtLeastOneNumber = 170878
        PasswordShouldHaveAtLeastOneSpecialCharacter = 170879
        YouCannotUseAPasswordThatIsTheSameAsAnyOfYourLastPercentDPasswords = 173872

        YourUserAccountHasBeenLockedDueToNumerousFailedLoInAttempts = 173873
        YouMayTryAgainAfterPercentNPercentM = 173874

        'AGL 2014.09.15
        Locked = 173875

        'AGL 2014.09.16
        SecurityQuestion = 162647
        SecurityAnswer = 162648
        SecurityAnswerIsRequired = 162088
        SecurityQuestionIsRequired = 162087
        PercentSIsRequired = 171252

        'AGL 2014.09.17 - 
        GeneratePassword = 171439

        'MKAM 2014.11.05
        Disclaimer = 173896
        AllergenDisclaimer = 173895

        'AMTLA
        UserInterface = 173925

        Lock = 157659
        Unlock = 157660

        'AGL 2014.02.26 - Sell Until
        SellUntil = 173425
        InsteadOf = 173428 'Statt

        'AGL 2015.02.05
        ListBehavior = 173943
        Empty = 56310

        'AGL 2015.02.06
        GenderSensitive = 172730

        'AGL 2014.02.24
        NutrientDescriptionPercentN = 171725
        Fat = 13030

        'AGL 2015.03.06
        Restaurant = 162499

        'MKAM 2015.01.16
        Declaration = 160432
        DeclarationDescription = 174277

        Recipelink = 133216
        Labels = 161333
        strNew = 135978

        'ECAM 2015.08.10
        ProcedureTemplateDescription = 174437
        ForYourApproval = 174286 'ANM 8/12/2015
        NoMatchesFound = 173406 ''ANM 8/17/2015

        'Raqi Pinili 2015.12.23
        ExportRecipeLabel = 174834

        ''IAA 01.21.2016
        PDFLabel = 174865
        WordLabel = 174867
        UseTemplateFromCatalog = 174881
        ReportCatalog = 175390
        IngredientType = 175391
        ReportStyle = 175392
        GoToCatalog = 175393

        ''AMTLA 2016.03.09
        SaleSite = 175286
        SendAutomaticReports = 175287
        Inactive = 175288
        InvalidUnits = 175289
        MayContainTraces = 175290

        'JOP 04-19-2016
        RecipeWorkflow = 175316
        RecipeWorkflowDescription = 175340

        'NBG 2016.03.17
        CountryProduction = 172823
        RecipeSharing = 175588
        RecipeSharingSubRecipe = 175589

        AllSites = 174537 'NBG 20161103
        ReplaceGlobalValidation = 175708 'NBG 20161103

        'LLG 2016.04.06
        AddArticle = 175294
        PIMNumber = 175473
        GetPIM = 175296


        'ECAM 2016.04.18
        Workflow = 175316
        Completed = 143673
        Started = 175331
        NotYetStarted = 175332
        Overdue = 175333
        StartDateAndTime = 167187

        'JOP 04-28-2016 for print config

        Countryside = 175344
        ComplianceVerification = 175345
        PrintFooter = 167142

        'JOP
        Recipients = 175357

        ValidateTime = 175363
        RequiredTime = 175364

        'JOP NICE LABEL
        DeclarationName = 174695
        ImposedComposition = 174386
        Pricefor = 175403
        Consumption = 175404
        Certification = 175405

        'LLG
        AllowInactiveMerch = 175395
        RecipeIngredient = 4834
        ExportNiceLabel = 175407
        CopyTheSelectedItem = 175470

        'ECAM
        SalesLocation = 175313

        'PJRB
        LastPrintedYield = 175587
        LotNumber = 171649
        PrintPreviewRecipeDetails = 132876

        'AMTLA 2016.10.24
        CheckedOutRecipes = 175704
        CheckedOutRecipesMenus = 175705

        'JOP 11.03.2016
        Shop = 175707

        LabelPrintingTool = 175479

        'JOP 11.10.2016
        ProductionLocation = 175309

        NumberOfCopiesForLabel = 175924

        'JBQL 01.06.2017
        ReplacedSuccessful = 175960
        Cannot_Delete_Marked_Items2 = 175961
        'JBQL 03.15.2017
        NotesDescription = 176056
        PackagingMethodDescription = 176057
        ConservationTemperatureDescription = 176058
        ProductionLocationDescription = 176059
        RecipeCertificationDescription = 176274
        ViewHistoryLogs = 176110
        Season = 172557
        TypeofService = 175347
        MassImportation = 176112
        CSVExport = 176113
        AssignBrand = 176114
        AssignCategory = 174010
        AssignSupplier = 176115
        MasterplanLocking = 176116
        ShopMigros = 176117
        PrintersMigros = 176118
        MasterPlan = 175335
        'PJRB 2017.03.24
        UseIngredientQuantity = 176093
        UseYieldQuantity = 176094
        UseDateOf = 176095
        Tomorrow = 176096
        Today = 134229
        WithTax = 157662
        WithoutTax = 159267

        'JBQL 2017-05-31
        RemoveFrom_Percent_m = 173360

        LicenseAccessAgain = 176215
        LicenseBeforeDate = 176221
        LicenseReminderNote = 176222
        LicenseHeader = 176223
        LicenseContactEGS = 176224
        LicensetoAvoidDisconnection = 176225
        LicensetoAvoidDisconnectionwDate = 176226
        LicenseHasExpired = 176227
        LicenseWillExpire = 176228
        LicenseHasExpiredwDays = 176229

        CopytoClipboard = 176236
        PicturesOfProcedures = 176275

        CanBeDeactivated = 176639

        ReplaceWarningDeleteComplementPreparation = 177512

        ' KGS 20200207
        WhatShouldBeExported = 177614
        ProductionPlan = 177615

        ' RGR
        KeywordBehavior = 177911
        KeywordIsCollapsed = 177912

        ReportCostAndMargin = 178543

        CostsPerDay = 180042
        CostsPerPortion = 180043

        'SM 2024.08.29 added for CMC COOP
        Products = 134171
        ProductList = 173745
        ProductNumber = 137060
        ProductNutrientList = 180655
        ProductListPrice = 180656
        Derived = 172489
    End Enum

    Public Function GetString(ByVal Code As CodeType, Optional intlicense As Integer = 0) As String  ' intlicense = 0 generic
        Dim strvalue As String
        ''''
        'FTB (EGS Manager Language) are based from EgsFtbLang table on EGS_DB
        ' Check if there's a custom translation
        Dim codeTrans As Integer = GetCustomText(Code, CInt(m_cli.l_App))

        Select Case m_Language
            Case 1 'english
                strvalue = FTBLow1(codeTrans)
            Case 2 'german
                strvalue = FTBLow2(codeTrans)
            Case 3 'french
                strvalue = FTBLow3(codeTrans)
            Case 4 'italian
                strvalue = FTBLow4(codeTrans)
            Case 6 'spanish
                strvalue = FTBLow6(codeTrans)
            Case 19 'dutch
                strvalue = FTBLow19(codeTrans)
            Case 42 'russian
                strvalue = FTBLow42(codeTrans)
            Case 15 'chinese
                strvalue = FTBLow43(codeTrans)
            Case 31 'japanese
                strvalue = FTBLow7(codeTrans)
            Case 9 'arabic
                strvalue = FTBLow49(codeTrans)
            Case Else 'default as english
                strvalue = FTBLow1(codeTrans)
        End Select

        If strvalue = String.Empty OrElse strvalue = Nothing Then
            strvalue = FTBLow1(codeTrans)
        End If

        'no translation founds
        If strvalue = Nothing Then
            strvalue = String.Concat("missing[", CInt(Code), "]")
            'Else
            '    strvalue = "@@" & strvalue
        End If

        Return strvalue
    End Function

    Public Property Language() As Integer
        Get
            Return m_Language
        End Get
        Set(ByVal Value As Integer)
            m_Language = Value
        End Set
    End Property

    Public Sub New(ByVal language As Integer)
        If language = 0 Then
            m_Language = m_LanguageDefault
        Else
            m_Language = language
        End If
    End Sub

End Class




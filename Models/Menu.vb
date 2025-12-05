Namespace CalcmenuAPI.Models

    Public Class MenuData
        Private m_Info As New Models.Menu
        Public Property Info As Models.Menu
            Get
                Return m_Info
            End Get
            Set(value As Models.Menu)
                m_Info = value
            End Set
        End Property
        Private m_Attachment As List(Of Models.MenuAttachment)
        Public Property Attachment As List(Of Models.MenuAttachment)
            Get
                Return m_Attachment
            End Get
            Set(value As List(Of Models.MenuAttachment))
                m_Attachment = value
            End Set
        End Property
        Private m_Calculation As List(Of Models.MenuCalculation)
        Public Property Calculation As List(Of Models.MenuCalculation)
            Get
                Return m_Calculation
            End Get
            Set(value As List(Of Models.MenuCalculation))
                m_Calculation = value
            End Set
        End Property
        Private m_Nutrition As List(Of Models.MenuNutrition)
        Public Property Nutrition As List(Of Models.MenuNutrition)
            Get
                Return m_Nutrition
            End Get
            Set(value As List(Of Models.MenuNutrition))
                m_Nutrition = value
            End Set
        End Property
        Private m_Ingredient As List(Of Models.MenuIngredient)
        Public Property Ingredient As List(Of Models.MenuIngredient)
            Get
                Return m_Ingredient
            End Get
            Set(value As List(Of Models.MenuIngredient))
                m_Ingredient = value
            End Set
        End Property

        Private m_Procedure As List(Of Models.MenuProcedure)
        Public Property Procedure As List(Of Models.MenuProcedure)
            Get
                Return m_Procedure
            End Get
            Set(value As List(Of Models.MenuProcedure))
                m_Procedure = value
            End Set
        End Property
        Private m_Keyword As List(Of Models.GenericTree)
        Public Property Keyword As List(Of Models.GenericTree)
            Get
                Return m_Keyword
            End Get
            Set(value As List(Of Models.GenericTree))
                m_Keyword = value
            End Set
        End Property
        Private m_BrandSite As List(Of Models.MenuBrandSite)
        Public Property BrandSite As List(Of Models.MenuBrandSite)
            Get
                Return m_BrandSite
            End Get
            Set(value As List(Of Models.MenuBrandSite))
                m_BrandSite = value
            End Set
        End Property
        Private m_Brands As List(Of Models.Brand)
        Public Property Brands As List(Of Models.Brand)
            Get
                Return m_Brands
            End Get
            Set(value As List(Of Models.Brand))
                m_Brands = value
            End Set
        End Property
        Private m_Publication As List(Of Models.MenuPublication)
        Public Property Publication As List(Of Models.MenuPublication)
            Get
                Return m_Publication
            End Get
            Set(value As List(Of Models.MenuPublication))
                m_Publication = value
            End Set
        End Property
        Private m_Sharing As List(Of Models.GenericList)
        Public Property Sharing As List(Of Models.GenericList)
            Get
                Return m_Sharing
            End Get
            Set(value As List(Of Models.GenericList))
                m_Sharing = value
            End Set
        End Property
        Private m_Project As List(Of Models.GenericTree)
        Public Property Project As List(Of Models.GenericTree)
            Get
                Return m_Project
            End Get
            Set(value As List(Of Models.GenericTree))
                m_Project = value
            End Set
        End Property
        Private m_Translation As List(Of Models.MenuTranslation)
        Public Property Translation As List(Of Models.MenuTranslation)
            Get
                Return m_Translation
            End Get
            Set(value As List(Of Models.MenuTranslation))
                m_Translation = value
            End Set
        End Property
        Private m_Comment As List(Of Models.MenuComment)
        Public Property Comment As List(Of Models.MenuComment)
            Get
                Return m_Comment
            End Get
            Set(value As List(Of Models.MenuComment))
                m_Comment = value
            End Set
        End Property
        Private m_History As List(Of Models.MenuHistory)
        Public Property History As List(Of Models.MenuHistory)
            Get
                Return m_History
            End Get
            Set(value As List(Of Models.MenuHistory))
                m_History = value
            End Set
        End Property
        Private m_Allergen As List(Of Models.ListeAllergen)
        Public Property Allergen As List(Of Models.ListeAllergen)
            Get
                Return m_Allergen
            End Get
            Set(ByVal value As List(Of Models.ListeAllergen))
                m_Allergen = value
            End Set
        End Property
        Private m_MenuLink As List(Of Models.MenuLinkList)
        Public Property MenuLink As List(Of Models.MenuLinkList)
            Get
                Return m_MenuLink
            End Get
            Set(value As List(Of Models.MenuLinkList))
                m_MenuLink = value
            End Set
        End Property
        Private m_ProcedureTemplate As List(Of Models.ProcedureTemplateInfo)
        Public Property ProcedureTemplate As List(Of Models.ProcedureTemplateInfo)
            Get
                Return m_ProcedureTemplate
            End Get
            Set(value As List(Of Models.ProcedureTemplateInfo))
                m_ProcedureTemplate = value
            End Set
        End Property
        Private m_TempProcPicture As String
        Public Property TempProcPicture As String
            Get
                Return m_TempProcPicture
            End Get
            Set(value As String)
                m_TempProcPicture = value
            End Set
        End Property
        Private m_hasApprover As Boolean
        Public Property hasApprover As Boolean
            Get
                Return m_hasApprover
            End Get
            Set(value As Boolean)
                m_hasApprover = value
            End Set
        End Property
        Private m_NextRoleLevelApprover As Integer
        Public Property NextRoleLevelApprover As Integer
            Get
                Return m_NextRoleLevelApprover
            End Get
            Set(value As Integer)
                m_NextRoleLevelApprover = value
            End Set
        End Property
    End Class
    Public Class Menu
        Public Sub New()

        End Sub
        Private m_CodeListe As Integer
        Public Property CodeListe As Integer
            Get
                Return m_CodeListe
            End Get
            Set(value As Integer)
                m_CodeListe = value
            End Set
        End Property
        Private m_Type As Integer
        Public Property Type As Integer
            Get
                Return m_Type
            End Get
            Set(value As Integer)
                m_Type = value
            End Set
        End Property
        Private m_Number As String
        Public Property Number As String
            Get
                Return m_Number
            End Get
            Set(value As String)
                m_Number = value
            End Set
        End Property
        Private m_Name As String
        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = value
            End Set
        End Property
        Private m_SubName As String
        Public Property SubName As String
            Get
                Return m_SubName
            End Get
            Set(value As String)
                m_SubName = value
            End Set
        End Property
        Private m_Category As String
        Public Property Category As String
            Get
                Return m_Category
            End Get
            Set(value As String)
                m_Category = value
            End Set
        End Property
        Private m_CodeCategory As Integer
        Public Property CodeCategory As Integer
            Get
                Return m_CodeCategory
            End Get
            Set(value As Integer)
                m_CodeCategory = value
            End Set
        End Property
        Private m_Source As String
        Public Property Source As String
            Get
                Return m_Source
            End Get
            Set(value As String)
                m_Source = value
            End Set
        End Property
        Private m_CodeSource As Integer
        Public Property CodeSource As Integer
            Get
                Return m_CodeSource
            End Get
            Set(value As Integer)
                m_CodeSource = value
            End Set
        End Property
        Private m_Remark As String
        Public Property Remark As String
            Get
                Return m_Remark
            End Get
            Set(value As String)
                m_Remark = value
            End Set
        End Property
        Private m_Description As String
        Public Property Description As String
            Get
                Return m_Description
            End Get
            Set(value As String)
                m_Description = value
            End Set
        End Property
        Private m_Yield As Double
        Public Property Yield As Double
            Get
                Return m_Yield
            End Get
            Set(value As Double)
                m_Yield = value
            End Set
        End Property
        Private m_YieldUnit As String
        Public Property YieldUnit As String
            Get
                Return m_YieldUnit
            End Get
            Set(value As String)
                m_YieldUnit = value
            End Set
        End Property
        Private m_CodeYieldUnit As Integer
        Public Property CodeYieldUnit As Integer
            Get
                Return m_CodeYieldUnit
            End Get
            Set(value As Integer)
                m_CodeYieldUnit = value
            End Set
        End Property
        Private m_Percent As Integer
        Public Property Percent As Integer
            Get
                Return m_Percent
            End Get
            Set(value As Integer)
                m_Percent = value
            End Set
        End Property
        Private m_Yield2 As Double
        Public Property Yield2 As Double
            Get
                Return m_Yield2
            End Get
            Set(value As Double)
                m_Yield2 = value
            End Set
        End Property
        Private m_YieldUnit2 As String
        Public Property YieldUnit2 As String
            Get
                Return m_YieldUnit2
            End Get
            Set(value As String)
                m_YieldUnit2 = value
            End Set
        End Property
        Private m_CodeYieldUnit2 As Integer
        Public Property CodeYieldUnit2 As Integer
            Get
                Return m_CodeYieldUnit2
            End Get
            Set(value As Integer)
                m_CodeYieldUnit2 = value
            End Set
        End Property
        Private m_SrQty As Double
        Public Property SrQty As Double
            Get
                Return m_SrQty
            End Get
            Set(value As Double)
                m_SrQty = value
            End Set
        End Property
        Private m_SrWeight As Double
        Public Property SrWeight As Double
            Get
                Return m_SrWeight
            End Get
            Set(value As Double)
                m_SrWeight = value
            End Set
        End Property
        Private m_SrUnit As String
        Public Property SrUnit As String
            Get
                Return m_SrUnit
            End Get
            Set(value As String)
                m_SrUnit = value
            End Set
        End Property
        Private m_SrUnitCode As Integer
        Public Property SrUnitCode As Integer
            Get
                Return m_SrUnitCode
            End Get
            Set(value As Integer)
                m_SrUnitCode = value
            End Set
        End Property
        Private m_CodeTrans As Integer
        Public Property CodeTrans As Integer
            Get
                Return m_CodeTrans
            End Get
            Set(value As Integer)
                m_CodeTrans = value
            End Set
        End Property
        Private m_CodeSite As Integer
        Public Property CodeSite As Integer
            Get
                Return m_CodeSite
            End Get
            Set(value As Integer)
                m_CodeSite = value
            End Set
        End Property
        Private m_CodeUser As Integer
        Public Property CodeUser As Integer
            Get
                Return m_CodeUser
            End Get
            Set(value As Integer)
                m_CodeUser = value
            End Set
        End Property
        Private m_DateCreated As String
        Public Property DateCreated As String
            Get
                Return m_DateCreated
            End Get
            Set(value As String)
                m_DateCreated = value
            End Set
        End Property
        Private m_Date1 As String
        Public Property Date1 As String
            Get
                Return m_Date1
            End Get
            Set(value As String)
                m_Date1 = value
            End Set
        End Property
        Private m_DateLastModified As String
        Public Property DateLastModified As String
            Get
                Return m_DateLastModified
            End Get
            Set(value As String)
                m_DateLastModified = value
            End Set
        End Property
        Private m_CreatedBy As String
        Public Property CreatedBy As String
            Get
                Return m_CreatedBy
            End Get
            Set(value As String)
                m_CreatedBy = value
            End Set
        End Property
        Private m_CodeCreatedBy As Integer
        Public Property CodeCreatedBy As Integer
            Get
                Return m_CodeCreatedBy
            End Get
            Set(value As Integer)
                m_CodeCreatedBy = value
            End Set
        End Property
        Private m_ModifiedBy As String
        Public Property ModifiedBy As String
            Get
                Return m_ModifiedBy
            End Get
            Set(value As String)
                m_ModifiedBy = value
            End Set
        End Property
        Private m_CodeModifiedBy As Integer
        Public Property CodeModifiedBy As Integer
            Get
                Return m_CodeModifiedBy
            End Get
            Set(value As Integer)
                m_CodeModifiedBy = value
            End Set
        End Property
        Private m_Pictures As String
        Public Property Pictures As String
            Get
                Return m_Pictures
            End Get
            Set(value As String)
                m_Pictures = value
            End Set
        End Property
        Private m_DefaultPicture As Integer
        Public Property DefaultPicture As Integer
            Get
                Return m_DefaultPicture
            End Get
            Set(value As Integer)
                m_DefaultPicture = value
            End Set
        End Property
        Private m_CustomTempPictures
        Public Property CustomTempPictures As String
            Get
                Return m_CustomTempPictures
            End Get
            Set(value As String)
                m_CustomTempPictures = value
            End Set
        End Property
        Private m_CustomTempAttachments
        Public Property CustomTempAttachments As String
            Get
                Return m_CustomTempAttachments
            End Get
            Set(value As String)
                m_CustomTempAttachments = value
            End Set
        End Property
        Private m_Rating As Integer
        Public Property Rating As Integer
            Get
                Return m_Rating
            End Get
            Set(value As Integer)
                m_Rating = value
            End Set
        End Property
        Private m_Difficulty As Integer
        Public Property Difficulty As Integer
            Get
                Return m_Difficulty
            End Get
            Set(value As Integer)
                m_Difficulty = value
            End Set
        End Property
        Private m_Budget As Integer
        Public Property Budget As Integer
            Get
                Return m_Budget
            End Get
            Set(value As Integer)
                m_Budget = value
            End Set
        End Property
        Private m_QE As Integer
        Public Property QuickAndEasy As Integer
            Get
                Return m_QE
            End Get
            Set(value As Integer)
                m_QE = value
            End Set
        End Property
        Private m_Dates As String
        Public Property [Dates] As String
            Get
                Return m_Dates
            End Get
            Set(value As String)
                m_Dates = value
            End Set
        End Property
        Private m_CodeMenuState As Integer
        Public Property CodeMenuState As Integer
            Get
                Return m_CodeMenuState
            End Get
            Set(value As Integer)
                m_CodeMenuState = value
            End Set
        End Property
        Private m_MenuState As String
        Public Property MenuState As String
            Get
                Return m_MenuState
            End Get
            Set(value As String)
                m_MenuState = value
            End Set
        End Property
        Private m_FootNote1 As String
        Public Property FootNote1 As String
            Get
                Return m_FootNote1
            End Get
            Set(value As String)
                m_FootNote1 = value
            End Set
        End Property
        Private m_FootNote2 As String
        Public Property FootNote2 As String
            Get
                Return m_FootNote2
            End Get
            Set(value As String)
                m_FootNote2 = value
            End Set
        End Property
        Private m_FootNote1Clean As String
        Public Property FootNote1Clean As String
            Get
                Return m_FootNote1Clean
            End Get
            Set(value As String)
                m_FootNote1Clean = value
            End Set
        End Property
        Private m_FootNote2Clean As String
        Public Property FootNote2Clean As String
            Get
                Return m_FootNote2Clean
            End Get
            Set(value As String)
                m_FootNote2Clean = value
            End Set
        End Property
        Private m_MethodFormat As String
        Public Property MethodFormat As String
            Get
                Return m_MethodFormat
            End Get
            Set(value As String)
                m_MethodFormat = value
            End Set
        End Property
        Private m_IsGlobal As Boolean
        Public Property IsGlobal As Boolean
            Get
                Return m_IsGlobal
            End Get
            Set(value As Boolean)
                m_IsGlobal = value
            End Set
        End Property

        Private m_CoolingTime As String
        Public Property CoolingTime As String
            Get
                Return m_CoolingTime
            End Get
            Set(value As String)
                m_CoolingTime = value
            End Set
        End Property
        Private m_HeatingTime As String
        Public Property HeatingTime As String
            Get
                Return m_HeatingTime
            End Get
            Set(value As String)
                m_HeatingTime = value
            End Set
        End Property
        Private m_HeatingTemperature As String
        Public Property HeatingTemperature As String
            Get
                Return m_HeatingTemperature
            End Get
            Set(value As String)
                m_HeatingTemperature = value
            End Set
        End Property
        Private m_HeatingMode As String
        Public Property HeatingMode As String
            Get
                Return m_HeatingMode
            End Get
            Set(value As String)
                m_HeatingMode = value
            End Set
        End Property
        Private m_StoringTime As String
        Public Property StoringTime As String
            Get
                Return m_StoringTime
            End Get
            Set(value As String)
                m_StoringTime = value
            End Set
        End Property
        Private m_StoringTemperature As String
        Public Property StoringTemperature As String
            Get
                Return m_StoringTemperature
            End Get
            Set(value As String)
                m_StoringTemperature = value
            End Set
        End Property
        Private m_CCPDescription As String
        Public Property CCPDescription As String
            Get
                Return m_CCPDescription
            End Get
            Set(value As String)
                m_CCPDescription = value
            End Set
        End Property
        Private m_CodeNutrientSet As Integer
        Public Property CodeNutrientSet As Integer
            Get
                Return m_CodeNutrientSet
            End Get
            Set(value As Integer)
                m_CodeNutrientSet = value
            End Set
        End Property
        Private m_ActualIngredients As String
        Public Property ActualIngredients As String 'AGL 2014.11.14
            Get
                Return m_ActualIngredients
            End Get
            Set(value As String)
                m_ActualIngredients = value
            End Set
        End Property
        Private m_Ingredients As String
        Public Property Ingredients As String 'AGL 2014.11.14
            Get
                Return m_Ingredients
            End Get
            Set(value As String)
                m_Ingredients = value
            End Set
        End Property

        Private m_PackagingMethodCode As Integer 'AMTLA 2015.01.30
        Public Property PackagingMethodCode As Integer
            Get
                Return m_PackagingMethodCode
            End Get
            Set(value As Integer)
                m_PackagingMethodCode = value
            End Set
        End Property

        Private m_Packaging As String 'AMTLA 2015.02.02
        Public Property Packaging As String
            Get
                Return m_Packaging
            End Get
            Set(value As String)
                m_Packaging = value
            End Set
        End Property

        Private m_CertificationCode As Integer 'AMTLA 2015.01.30
        Public Property CertificationCode As Integer
            Get
                Return m_CertificationCode
            End Get
            Set(value As Integer)
                m_CertificationCode = value
            End Set
        End Property

        Private m_Certification As String 'AMTLA 2015.02.02
        Public Property Certification As String
            Get
                Return m_Certification
            End Get
            Set(value As String)
                m_Certification = value
            End Set
        End Property

        Private m_InformationCode As Integer 'AMTLA 2015.01.30
        Public Property InformationCode As Integer
            Get
                Return m_InformationCode
            End Get
            Set(value As Integer)
                m_InformationCode = value
            End Set
        End Property

        Private m_Information As String 'AMTLA 2015.02.02
        Public Property Information As String
            Get
                Return m_Information
            End Get
            Set(value As String)
                m_Information = value
            End Set
        End Property

        Private m_TemperatureCode As Integer 'AMTLA 2015.01.30
        Public Property TemperatureCode As Integer
            Get
                Return m_TemperatureCode
            End Get
            Set(value As Integer)
                m_TemperatureCode = value
            End Set
        End Property

        Private m_Temperature As String 'AMTLA 2015.02.02
        Public Property Temperature As String
            Get
                Return m_Temperature
            End Get
            Set(value As String)
                m_Temperature = value
            End Set
        End Property

        Private m_Note As String 'AGL 2015.02.12
        Public Property Note As String
            Get
                Return m_Note
            End Get
            Set(value As String)
                m_Note = value
            End Set
        End Property

    End Class

    Public Class MenuAttachment
        Private m_Id As Integer
        Public Property Id As Integer
            Get
                Return m_Id
            End Get
            Set(value As Integer)
                m_Id = value
            End Set
        End Property
        Private m_Type As Integer
        Public Property Type As Integer
            Get
                Return m_Type
            End Get
            Set(value As Integer)
                m_Type = value
            End Set
        End Property
        Private m_Resource As String
        Public Property Resource As String
            Get
                Return m_Resource
            End Get
            Set(value As String)
                m_Resource = value
            End Set
        End Property
        Private m_Name As String
        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = value
            End Set
        End Property
        Private m_blnDefault As Boolean
        Public Property IsDefault As Boolean
            Get
                Return m_blnDefault
            End Get
            Set(value As Boolean)
                m_blnDefault = value
            End Set
        End Property
    End Class
    Public Class MenuCalculation
        Private m_Id As Integer
        Public Property [Id] As Integer
            Get
                Return m_Id
            End Get
            Set(value As Integer)
                m_Id = value
            End Set
        End Property
        Private m_CodeListe As Integer
        Public Property CodeListe As Integer
            Get
                Return m_CodeListe
            End Get
            Set(value As Integer)
                m_CodeListe = value
            End Set
        End Property
        Private m_Coef As Double
        Public Property Coef As Double
            Get
                Return m_Coef
            End Get
            Set(value As Double)
                m_Coef = value
            End Set
        End Property
        Private m_CalcPrice As Double
        Public Property CalcPrice As Double
            Get
                Return m_CalcPrice
            End Get
            Set(value As Double)
                m_CalcPrice = value
            End Set
        End Property
        Private m_ImposedPrice As Double
        Public Property ImposedPrice As Double
            Get
                Return m_ImposedPrice
            End Get
            Set(value As Double)
                m_ImposedPrice = value
            End Set
        End Property
        Private m_CodeSetPrice As Integer
        Public Property CodeSetPrice As Integer
            Get
                Return m_CodeSetPrice
            End Get
            Set(value As Integer)
                m_CodeSetPrice = value
            End Set
        End Property
        Private m_Tax As Integer
        Public Property Tax As Integer
            Get
                Return m_Tax
            End Get
            Set(value As Integer)
                m_Tax = value
            End Set
        End Property
        Private m_TaxValue As Double
        Public Property TaxValue As Double
            Get
                Return m_TaxValue
            End Get
            Set(value As Double)
                m_TaxValue = value
            End Set
        End Property
    End Class
    Public Class MenuNutrition
        Private m_Id As Integer
        Public Property [Id] As Integer
            Get
                Return m_Id
            End Get
            Set(value As Integer)
                m_Id = value
            End Set
        End Property
        Private m_Nutr_No As Integer
        Public Property Nutr_No As Integer
            Get
                Return m_Nutr_No
            End Get
            Set(value As Integer)
                m_Nutr_No = value
            End Set
        End Property
        Private m_Position As Integer
        Public Property Position As Integer
            Get
                Return m_Position
            End Get
            Set(value As Integer)
                m_Position = value
            End Set
        End Property
        Private m_Name As String
        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = value
            End Set
        End Property
        Private m_TagName As String
        Public Property TagName As String
            Get
                Return m_TagName
            End Get
            Set(value As String)
                m_TagName = value
            End Set
        End Property
        Private m_Value As Double ' RBAJ-2014.01.03 Changed from Double to String
        Public Property Value As Double
            Get
                Return m_Value
            End Get
            Set(value As Double)
                m_Value = value
            End Set
        End Property
        Private m_Imposed As Double
        Public Property Imposed As Double ' RBAJ-2014.01.03 Changed from Double to String  'MKAM Changed back to Double
            Get
                Return m_Imposed
            End Get
            Set(value As Double)
                m_Imposed = value
            End Set
        End Property
        Private m_Percent As String
        Public Property Percent As String ' RBAJ-2014.01.03 Changed from Double to String
            Get
                Return m_Percent
            End Get
            Set(value As String)
                m_Percent = value
            End Set
        End Property
        Private m_Format As String
        Public Property Format As String
            Get
                Return m_Format
            End Get
            Set(value As String)
                m_Format = value
            End Set
        End Property
        Private m_Unit As String
        Public Property Unit As String
            Get
                Return m_Unit
            End Get
            Set(value As String)
                m_Unit = value
            End Set
        End Property
        Private m_GDA As Integer
        Public Property GDA As Integer
            Get
                Return m_GDA
            End Get
            Set(value As Integer)
                m_GDA = value
            End Set
        End Property
        Private m_CodeNutrientSet As Integer
        Public Property CodeNutrientSet As Integer
            Get
                Return m_CodeNutrientSet
            End Get
            Set(value As Integer)
                m_CodeNutrientSet = value
            End Set
        End Property
        Private m_NutrientSet As String
        Public Property NutrientSet As String
            Get
                Return m_NutrientSet
            End Get
            Set(value As String)
                m_NutrientSet = value
            End Set
        End Property
        Private m_DisplayNutrition As Boolean
        Public Property DisplayNutrition As Boolean
            Get
                Return m_DisplayNutrition
            End Get
            Set(value As Boolean)
                m_DisplayNutrition = value
            End Set
        End Property
        Private m_Display As Boolean
        Public Property Display As Boolean
            Get
                Return m_Display
            End Get
            Set(value As Boolean)
                m_Display = value
            End Set
        End Property
        Private m_ImposedType As Integer
        Public Property ImposedType As Integer
            Get
                Return m_ImposedType
            End Get
            Set(value As Integer)
                m_ImposedType = value
            End Set
        End Property
        Private m_PortionSize As String
        Public Property PortionSize As String
            Get
                Return m_PortionSize
            End Get
            Set(value As String)
                m_PortionSize = value
            End Set
        End Property
        Private m_NutritionBasis As String
        Public Property NutritionBasis As String
            Get
                Return m_NutritionBasis
            End Get
            Set(value As String)
                m_NutritionBasis = value
            End Set
        End Property
    End Class
    Public Class MenuBrandSite
        Private m_Code As Integer
        Public Property Code As Integer
            Get
                Return m_Code
            End Get
            Set(value As Integer)
                m_Code = value
            End Set
        End Property
        Private m_Name As String
        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = value
            End Set
        End Property
        Private m_Enabled As Boolean
        Public Property Enabled As Boolean
            Get
                Return m_Enabled
            End Get
            Set(value As Boolean)
                m_Enabled = value
            End Set
        End Property
        Private m_DateFrom As String
        Public Property DateFrom As String
            Get
                Return m_DateFrom
            End Get
            Set(value As String)
                m_DateFrom = value
            End Set
        End Property
        Private m_DateTo As String
        Public Property DateTo As String
            Get
                Return m_DateTo
            End Get
            Set(value As String)
                m_DateTo = value
            End Set
        End Property
    End Class
    Public Class MenuIngredient
        Private m_CodeListe As Integer
        Public Property CodeListe As Integer
            Get
                Return m_CodeListe
            End Get
            Set(value As Integer)
                m_CodeListe = value
            End Set
        End Property

        Private m_CodeUser As Integer ' RBAJ-2014.01.24
        Public Property CodeUser As Integer
            Get
                Return m_CodeUser
            End Get
            Set(value As Integer)
                m_CodeUser = value
            End Set
        End Property

        Private m_ItemId As Integer
        Public Property ItemId As Integer
            Get
                Return m_ItemId
            End Get
            Set(value As Integer)
                m_ItemId = value
            End Set
        End Property

        Private m_ItemCode As Integer
        Public Property ItemCode As Integer
            Get
                Return m_ItemCode
            End Get
            Set(value As Integer)
                m_ItemCode = value
            End Set
        End Property

        Private m_ItemName As String
        Public Property ItemName As String
            Get
                Return m_ItemName
            End Get
            Set(value As String)
                m_ItemName = value
            End Set
        End Property

        Private m_ItemType As Integer
        Public Property ItemType As Integer
            Get
                Return m_ItemType
            End Get
            Set(value As Integer)
                m_ItemType = value
            End Set
        End Property

        Private m_ItemQty As Double
        Public Property ItemQty As Double
            Get
                Return m_ItemQty
            End Get
            Set(value As Double)
                m_ItemQty = value
            End Set
        End Property

        Private m_ItemUnit As String
        Public Property ItemUnit As String
            Get
                Return m_ItemUnit
            End Get
            Set(value As String)
                m_ItemUnit = value
            End Set
        End Property

        Private m_ItemCodeUnit As Integer
        Public Property ItemCodeUnit As Integer
            Get
                Return m_ItemCodeUnit
            End Get
            Set(value As Integer)
                m_ItemCodeUnit = value
            End Set
        End Property

        Private m_Step As Integer
        Public Property [Step] As Integer
            Get
                Return m_Step
            End Get
            Set(value As Integer)
                m_Step = value
            End Set
        End Property

        Private m_Position As Integer
        Public Property Position As Integer
            Get
                Return m_Position
            End Get
            Set(value As Integer)
                m_Position = value
            End Set
        End Property

        Private m_Complement As String
        Public Property Complement As String
            Get
                Return m_Complement
            End Get
            Set(value As String)
                m_Complement = value
            End Set
        End Property

        Private m_Preparation As String
        Public Property Preparation As String
            Get
                Return m_Preparation
            End Get
            Set(value As String)
                m_Preparation = value
            End Set
        End Property

        Private m_AlternativeIngredient As String
        Public Property AlternativeIngredient As String
            Get
                Return m_AlternativeIngredient
            End Get
            Set(value As String)
                m_AlternativeIngredient = value
            End Set
        End Property
        Private m_Remark As String
        Public Property Remark As String
            Get
                Return m_Remark
            End Get
            Set(value As String)
                m_Remark = value
            End Set
        End Property

        Private m_TmpName As String
        Public Property TmpName As String
            Get
                Return m_TmpName
            End Get
            Set(value As String)
                m_TmpName = value
            End Set
        End Property

        Private m_TmpQty As String
        Public Property TmpQty As String
            Get
                Return m_TmpQty
            End Get
            Set(value As String)
                m_TmpQty = value
            End Set
        End Property

        Private m_TmpUnit As String
        Public Property TmpUnit As String
            Get
                Return m_TmpUnit
            End Get
            Set(value As String)
                m_TmpUnit = value
            End Set
        End Property

        Private m_TmpComplement As String
        Public Property TmpComplement As String
            Get
                Return m_TmpComplement
            End Get
            Set(value As String)
                m_TmpComplement = value
            End Set
        End Property

        Private m_TmpPreparation As String
        Public Property TmpPreparation As String
            Get
                Return m_TmpPreparation
            End Get
            Set(value As String)
                m_TmpPreparation = value
            End Set
        End Property

        Private m_Wastage1 As Integer
        Public Property Wastage1 As Integer
            Get
                Return m_Wastage1
            End Get
            Set(value As Integer)
                m_Wastage1 = value
            End Set
        End Property

        Private m_Wastage2 As Integer
        Public Property Wastage2 As Integer
            Get
                Return m_Wastage2
            End Get
            Set(value As Integer)
                m_Wastage2 = value
            End Set
        End Property

        Private m_Wastage3 As Integer
        Public Property Wastage3 As Integer
            Get
                Return m_Wastage3
            End Get
            Set(value As Integer)
                m_Wastage3 = value
            End Set
        End Property

        Private m_Wastage4 As Integer
        Public Property Wastage4 As Integer
            Get
                Return m_Wastage4
            End Get
            Set(value As Integer)
                m_Wastage4 = value
            End Set
        End Property

        Private m_IsQuickEncode As Boolean
        Public Property IsQuickEncode As Boolean
            Get
                Return m_IsQuickEncode
            End Get
            Set(value As Boolean)
                m_IsQuickEncode = value
            End Set
        End Property
        Private m_isLocked As Boolean
        Public Property isLocked As Boolean
            Get
                Return m_isLocked
            End Get
            Set(value As Boolean)
                m_isLocked = value
            End Set
        End Property

        Private m_IsAllowMetricImperial As Boolean
        Public Property IsAllowMetricImperial As Boolean
            Get
                Return m_IsAllowMetricImperial
            End Get
            Set(value As Boolean)
                m_IsAllowMetricImperial = value
            End Set
        End Property

        Private m_QuantityMetric As Double
        Public Property QuantityMetric As Double
            Get
                Return m_QuantityMetric
            End Get
            Set(value As Double)
                m_QuantityMetric = value
            End Set
        End Property

        Private m_CodeUnitMetric As Integer
        Public Property CodeUnitMetric As Integer
            Get
                Return m_CodeUnitMetric
            End Get
            Set(value As Integer)
                m_CodeUnitMetric = value
            End Set
        End Property

        Private m_UnitMetric As String
        Public Property UnitMetric As String
            Get
                Return m_UnitMetric
            End Get
            Set(value As String)
                m_UnitMetric = value
            End Set
        End Property
        Private m_itemSellingPrice As Double
        Public Property itemSellingPrice As Double
            Get
                Return m_itemSellingPrice
            End Get
            Set(value As Double)
                m_itemSellingPrice = value
            End Set
        End Property
        Private m_ImposedPrice As Double
        Public Property ImposedPrice As Double
            Get
                Return m_ImposedPrice
            End Get
            Set(value As Double)
                m_ImposedPrice = value
            End Set
        End Property
        Private m_Cons As Double
        Public Property Cons As Double
            Get
                Return m_Cons
            End Get
            Set(value As Double)
                m_Cons = value
            End Set
        End Property
        Private m_QuantityImperial As Double
        Public Property QuantityImperial As Double
            Get
                Return m_QuantityImperial
            End Get
            Set(value As Double)
                m_QuantityImperial = value
            End Set
        End Property

        Private m_CodeUnitImperial As Integer
        Public Property CodeUnitImperial As Integer
            Get
                Return m_CodeUnitImperial
            End Get
            Set(value As Integer)
                m_CodeUnitImperial = value
            End Set
        End Property

        Private m_UnitImperial As String
        Public Property UnitImperial As String
            Get
                Return m_UnitImperial
            End Get
            Set(value As String)
                m_UnitImperial = value
            End Set
        End Property


        Private m_ConvertDirection As Integer
        Public Property ConvertDirection As Integer
            Get
                Return m_ConvertDirection
            End Get
            Set(value As Integer)
                m_ConvertDirection = value
            End Set
        End Property

        Private m_Price As Double
        Public Property Price As Double
            Get
                Return m_Price
            End Get
            Set(value As Double)
                m_Price = value
            End Set
        End Property
        Private m_YieldIng As Double
        Public Property YieldIng As Double
            Get
                Return m_YieldIng
            End Get
            Set(value As Double)
                m_YieldIng = value
            End Set
        End Property
        Private m_Factor As Double
        Public Property Factor As Double
            Get
                Return m_Factor
            End Get
            Set(value As Double)
                m_Factor = value
            End Set
        End Property

        Private m_PriceUnit As String
        Public Property PriceUnit As String
            Get
                Return m_PriceUnit
            End Get
            Set(value As String)
                m_PriceUnit = value
            End Set
        End Property

        Private m_Amount As Double
        Public Property Amount As Double
            Get
                Return m_Amount
            End Get
            Set(value As Double)
                m_Amount = value
            End Set
        End Property

        Private m_ApprovalStatusCode As Integer
        Public Property ApprovalStatusCode As Integer
            Get

                Return m_ApprovalStatusCode
            End Get
            Set(value As Integer)
                m_ApprovalStatusCode = value
            End Set
        End Property

        Private m_ApprovalRequestedBy As Integer
        Public Property ApprovalRequestedBy As Integer
            Get
                Return m_ApprovalRequestedBy
            End Get
            Set(value As Integer)
                m_ApprovalRequestedBy = value
            End Set
        End Property

        Private m_ApprovalRequestedDate As String
        Public Property ApprovalRequestedDate As String
            Get
                Return m_ApprovalRequestedDate
            End Get
            Set(value As String)

                m_ApprovalRequestedDate = value
            End Set
        End Property

        Private m_ApprovalBy As Integer
        Public Property ApprovalBy As Integer
            Get
                Return m_ApprovalBy
            End Get
            Set(value As Integer)
                m_ApprovalBy = value
            End Set
        End Property

        Private m_ApprovalDate As String
        Public Property ApprovalDate As String
            Get
                Return m_ApprovalDate
            End Get
            Set(value As String)
                m_ApprovalDate = value
            End Set
        End Property

        Private m_CodeBrand As Integer
        Public Property CodeBrand As Integer
            Get
                Return m_CodeBrand
            End Get
            Set(value As Integer)
                m_CodeBrand = value
            End Set
        End Property

        Private m_Pictures As String
        Public Property Pictures As String
            Get
                Return m_Pictures
            End Get
            Set(value As String)
                m_Pictures = value
            End Set
        End Property

        Private m_Videos As String
        Public Property Videos As String
            Get
                Return m_Videos
            End Get
            Set(value As String)
                m_Videos = value
            End Set
        End Property

        Private m_tempPictures As String
        Public Property tempPictures As String
            Get
                Return m_tempPictures
            End Get
            Set(value As String)
                m_tempPictures = value
            End Set
        End Property

        Private m_tempVideos As String
        Public Property tempVideos As String
            Get
                Return m_tempVideos
            End Get
            Set(value As String)
                m_tempVideos = value
            End Set
        End Property

        Private m_CodeUnitDisplaySelection As Integer
        Public Property CodeUnitDisplaySelection As Integer
            Get
                Return m_CodeUnitDisplaySelection
            End Get
            Set(value As Integer)
                m_CodeUnitDisplaySelection = value
            End Set
        End Property

        Private m_Translation As List(Of Models.MenuIngredientTranslation)
        Public Property Translation As List(Of Models.MenuIngredientTranslation)
            Get
                Return m_Translation
            End Get
            Set(value As List(Of Models.MenuIngredientTranslation))
                m_Translation = value
            End Set
        End Property

    End Class
    Public Class MenuIngredientTranslation
        Private m_ItemId As Integer
        Public Property ItemId As Integer
            Get
                Return m_ItemId
            End Get
            Set(value As Integer)
                m_ItemId = value
            End Set
        End Property

        Private m_CodeTrans As Integer
        Public Property CodeTrans As Integer
            Get
                Return m_CodeTrans
            End Get
            Set(value As Integer)
                m_CodeTrans = value
            End Set
        End Property

        Private m_Name As String
        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = value
            End Set
        End Property
        Private m_Remark As String
        Public Property Remark As String
            Get
                Return m_Remark
            End Get
            Set(value As String)
                m_Remark = value
            End Set
        End Property
        Private m_Note As String
        Public Property Note As String
            Get
                Return m_Note
            End Get
            Set(value As String)
                m_Note = value
            End Set
        End Property

        Private m_Complement As String
        Public Property Complement As String
            Get
                Return m_Complement
            End Get
            Set(value As String)
                m_Complement = value
            End Set
        End Property

        Private m_Preparation As String
        Public Property Preparation As String
            Get
                Return m_Preparation
            End Get
            Set(value As String)
                m_Preparation = value
            End Set
        End Property

        Private m_AlternativeIngredient As String
        Public Property AlternativeIngredient As String
            Get
                Return m_AlternativeIngredient
            End Get
            Set(value As String)
                m_AlternativeIngredient = value
            End Set
        End Property

        Private m_intStep As String
        Public Property [Step] As Integer
            Get
                Return m_intStep
            End Get
            Set(value As Integer)
                m_intStep = value
            End Set
        End Property
    End Class
    Public Class MenuProcedure
        Private m_NoteId As Integer
        Public Property NoteId As Integer
            Get
                Return m_NoteId
            End Get
            Set(value As Integer)
                m_NoteId = value
            End Set
        End Property
        Private m_Position As Integer
        Public Property Position As Integer
            Get
                Return m_Position
            End Get
            Set(value As Integer)
                m_Position = value
            End Set
        End Property
        Private m_Note As String
        Public Property Note As String
            Get
                Return m_Note
            End Get
            Set(value As String)
                m_Note = value
            End Set
        End Property
        Private m_AbbrevNote As String
        Public Property AbbrevNote As String
            Get
                Return m_AbbrevNote
            End Get
            Set(value As String)
                m_AbbrevNote = value
            End Set
        End Property
        Private m_Translation As List(Of Models.MenuProcedureTranslation)
        Public Property Translation As List(Of Models.MenuProcedureTranslation)
            Get
                Return m_Translation
            End Get
            Set(value As List(Of Models.MenuProcedureTranslation))
                m_Translation = value
            End Set
        End Property
        Private m_Picture As String
        Public Property Picture As String
            Get
                Return m_Picture
            End Get
            Set(value As String)
                m_Picture = value
            End Set
        End Property
        Private m_hasPicture As Boolean
        Public Property hasPicture As Boolean
            Get
                Return m_hasPicture
            End Get
            Set(value As Boolean)
                m_hasPicture = value
            End Set
        End Property
    End Class
    Public Class MenuProcedureTranslation
        Private m_NoteId As Integer
        Public Property NoteId As Integer
            Get
                Return m_NoteId
            End Get
            Set(value As Integer)
                m_NoteId = value
            End Set
        End Property
        Private m_CodeTrans As Integer
        Public Property CodeTrans As Integer
            Get
                Return m_CodeTrans
            End Get
            Set(value As Integer)
                m_CodeTrans = value
            End Set
        End Property
        Private m_Note As String
        Public Property Note As String
            Get
                Return m_Note
            End Get
            Set(value As String)
                m_Note = value
            End Set
        End Property
        Private m_AbbrevNote As String
        Public Property AbbrevNote As String
            Get
                Return m_AbbrevNote
            End Get
            Set(value As String)
                m_AbbrevNote = value
            End Set
        End Property
        Private m_Position As Integer
        Public Property Position As Integer
            Get
                Return m_Position
            End Get
            Set(value As Integer)
                m_Position = value
            End Set
        End Property
    End Class
    Public Class MenuPublication
        Private m_Code As Integer
        Public Property Code As Integer
            Get
                Return m_Code
            End Get
            Set(value As Integer)
                m_Code = value
            End Set
        End Property
        Private m_Type As Integer
        Public Property Type As Integer
            Get
                Return m_Type
            End Get
            Set(value As Integer)
                m_Type = value
            End Set
        End Property
        Private m_Name As String
        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = value
            End Set
        End Property
        Private m_Description As String
        Public Property Description As String
            Get
                Return m_Description
            End Get
            Set(value As String)
                m_Description = value
            End Set
        End Property
        Private m_Dates As String
        Public Property Dates As String
            Get
                Return m_Dates
            End Get
            Set(value As String)
                m_Dates = value
            End Set
        End Property
        Private m_CodeBrandSite As Integer
        Public Property CodeBrandSite As Integer
            Get
                Return m_CodeBrandSite
            End Get
            Set(value As Integer)
                m_CodeBrandSite = value
            End Set
        End Property
        Private m_PlacementId As Integer
        Public Property PlacementId As Integer
            Get
                Return m_PlacementId
            End Get
            Set(value As Integer)
                m_PlacementId = value
            End Set
        End Property
    End Class
    Public Class MenuTranslation
        Private m_Id As Integer
        Public Property Id As Integer
            Get
                Return m_Id
            End Get
            Set(value As Integer)
                m_Id = value
            End Set
        End Property
        Private m_CodeTrans As Integer
        Public Property CodeTrans As Integer
            Get
                Return m_CodeTrans
            End Get
            Set(value As Integer)
                m_CodeTrans = value
            End Set
        End Property
        Private m_TranslationName As String
        Public Property TranslationName As String
            Get
                Return m_TranslationName
            End Get
            Set(value As String)
                m_TranslationName = value
            End Set
        End Property
        Private m_Name As String
        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = value
            End Set
        End Property
        Private m_SubName As String
        Public Property SubName As String
            Get
                Return m_SubName
            End Get
            Set(value As String)
                m_SubName = value
            End Set
        End Property
        Private m_Remark As String
        Public Property Remark As String
            Get
                Return m_Remark
            End Get
            Set(value As String)
                m_Remark = value
            End Set
        End Property
        Private m_Description As String
        Public Property Description As String
            Get
                Return m_Description
            End Get
            Set(value As String)
                m_Description = value
            End Set
        End Property
        Private m_Notes As String
        Public Property Notes As String
            Get
                Return m_Notes
            End Get
            Set(value As String)
                m_Notes = value
            End Set
        End Property
        Private m_AdditionalNotes As String
        Public Property AdditionalNotes As String
            Get
                Return m_AdditionalNotes
            End Get
            Set(value As String)
                m_AdditionalNotes = value
            End Set
        End Property
        Private m_CCPDescription
        Public Property CCPDescription As String
            Get
                Return m_CCPDescription
            End Get
            Set(value As String)
                m_CCPDescription = value
            End Set
        End Property
        Private m_Picture As String
        Public Property Picture As String
            Get
                Return m_Picture
            End Get
            Set(value As String)
                m_Picture = value
            End Set
        End Property
        Private m_hasPicture As Boolean
        Public Property hasPicture As Boolean
            Get
                Return m_hasPicture
            End Get
            Set(value As Boolean)
                m_hasPicture = value
            End Set
        End Property
        Private m_Ingredients
        Public Property Ingredients As String
            Get
                Return m_Ingredients
            End Get
            Set(value As String)
                m_Ingredients = value
            End Set
        End Property
        Private m_Archive As Integer
        Public Property Archive As Integer
            Get
                Return m_Archive
            End Get
            Set(value As Integer)
                m_Archive = value
            End Set
        End Property

    End Class
    Public Class MenuComment
        Private m_Sequence As Integer
        Public Property Sequence As Integer
            Get
                Return m_Sequence
            End Get
            Set(value As Integer)
                m_Sequence = value
            End Set
        End Property
        Private m_Owner As Integer
        Public Property Owner As Integer
            Get
                Return m_Owner
            End Get
            Set(value As Integer)
                m_Owner = value
            End Set
        End Property
        Private m_Description As String
        Public Property Description As String
            Get
                Return m_Description
            End Get
            Set(value As String)
                m_Description = value
            End Set
        End Property
        Private m_PostedBy As String
        Public Property PostedBy As String
            Get
                Return m_PostedBy
            End Get
            Set(value As String)
                m_PostedBy = value
            End Set
        End Property
        Private m_SubmitDate As String
        Public Property SubmitDate As String
            Get
                Return m_SubmitDate
            End Get
            Set(value As String)
                m_SubmitDate = value
            End Set
        End Property
        Private m_DateLastModified As String
        Public Property DateLastModified As String
            Get
                Return m_DateLastModified
            End Get
            Set(value As String)
                m_DateLastModified = value
            End Set
        End Property
    End Class
    Public Class MenuCheckout
        Private m_CodeListe As Integer
        Public Property CodeListe As Integer
            Get
                Return m_CodeListe
            End Get
            Set(value As Integer)
                m_CodeListe = value
            End Set
        End Property
        Private m_CodeUser As Integer
        Public Property CodeUser As Integer
            Get
                Return m_CodeUser
            End Get
            Set(value As Integer)
                m_CodeUser = value
            End Set
        End Property
    End Class
    Public Class MenuUsedAsIngredient
        Private m_Code As Integer
        Public Property Code As Integer
            Get
                Return m_Code
            End Get
            Set(value As Integer)
                m_Code = value
            End Set
        End Property
        Private m_Number As String
        Public Property Number As String
            Get
                Return m_Number
            End Get
            Set(value As String)
                m_Number = value
            End Set
        End Property
        Private m_Name As String
        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = value
            End Set
        End Property
    End Class
    Public Class MenuHistory '' TODO JTOC
        Private m_DateAudit As String
        Public Property DateAudit As String
            Get
                Return m_DateAudit
            End Get
            Set(value As String)
                m_DateAudit = value
            End Set
        End Property
        Private m_FieldName As String
        Public Property FieldName As String
            Get
                Return m_FieldName
            End Get
            Set(value As String)
                m_FieldName = value
            End Set
        End Property
        Private m_Time As String
        Public Property Time As String
            Get
                Return m_Time
            End Get
            Set(value As String)
                m_Time = value
            End Set
        End Property
        Private m_FieldCode As String
        Public Property FieldCode As String
            Get
                Return m_FieldCode
            End Get
            Set(value As String)
                m_FieldCode = value
            End Set
        End Property
        Private m_Previous As String
        Public Property Previous As String
            Get
                Return m_Previous
            End Get
            Set(value As String)
                m_Previous = value
            End Set
        End Property
        Private m_New As String
        Public Property HNew As String
            Get
                Return m_New
            End Get
            Set(value As String)
                m_New = value
            End Set
        End Property
        Private m_User As String
        Public Property User As String
            Get
                Return m_User
            End Get
            Set(value As String)
                m_User = value
            End Set
        End Property
        Private m_AuditType As String
        Public Property AuditType As String
            Get
                Return m_AuditType
            End Get
            Set(value As String)
                m_AuditType = value
            End Set
        End Property
        Private m_CodeListe As String
        Public Property CodeListe As String
            Get
                Return m_CodeListe
            End Get
            Set(value As String)
                m_CodeListe = value
            End Set
        End Property
        Private m_CodeUser As String
        Public Property CodeUser As String
            Get
                Return m_CodeUser
            End Get
            Set(value As String)
                m_CodeUser = value
            End Set
        End Property
        Private m_IsCode As String
        Public Property IsCode As String
            Get
                Return m_IsCode
            End Get
            Set(value As String)
                m_IsCode = value
            End Set
        End Property
    End Class

    Public Class MenuHistoryResponse

        Private m_Data As List(Of Models.MenuHistory)
        Public Property Data As List(Of Models.MenuHistory)
            Get
                Return m_Data
            End Get
            Set(value As List(Of Models.MenuHistory))
                m_Data = value
            End Set
        End Property
        Private m_Count As Integer
        Public Property Count As Integer
            Get
                Return m_Count
            End Get
            Set(value As Integer)
                m_Count = value
            End Set
        End Property
        Public Sub New(data As List(Of Models.MenuHistory), count As Integer)
            Me.Count = count
            Me.Data = data

        End Sub
    End Class
End Namespace

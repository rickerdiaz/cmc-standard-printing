Namespace CalcmenuAPI.Models

    ''JDO 2.27.2014 SEARCH CONTROLLER MODELS
    Public Class GenericSearchList
        Private m_value As String = -1
        Public Property value As String
            Get
                Return m_value
            End Get
            Set(value As String)
                m_value = value
            End Set
        End Property
        Private m_name As String
        Public Property name As String
            Get
                Return m_name
            End Get
            Set(value As String)
                m_name = value
            End Set
        End Property
    End Class
    Public Class SearchRecipeName
        Private m_value As String
        Public Property value As String
            Get
                Return m_value
            End Get
            Set(value As String)
                m_value = value
            End Set
        End Property
        Private m_required As Boolean = False
        Public Property required As Boolean
            Get
                Return m_required
            End Get
            Set(value As Boolean)
                m_required = value
            End Set
        End Property
        Private m_fullText As Boolean = False
        Public Property fullText As Boolean
            Get
                Return m_fullText
            End Get
            Set(value As Boolean)
                m_fullText = value
            End Set
        End Property
    End Class
    Public Class SearchRecipeNumber
        Private m_value As String
        Public Property value As String
            Get
                Return m_value
            End Get
            Set(value As String)
                m_value = value
            End Set
        End Property
        Private m_modifier As Integer = 1
        Public Property modifier As Integer
            Get
                Return m_modifier
            End Get
            Set(value As Integer)
                m_modifier = value
            End Set
        End Property
        Private m_required As Boolean
        Public Property required As Boolean
            Get
                Return m_required
            End Get
            Set(value As Boolean)
                m_required = value
            End Set
        End Property
    End Class
    Public Class SearchRecipeLanguage
        Private m_value As String
        Public Property value As String
            Get
                Return m_value
            End Get
            Set(value As String)
                m_value = value
            End Set
        End Property
        Private m_name As String
        Public Property name As String
            Get
                Return m_name
            End Get
            Set(value As String)
                m_name = value
            End Set
        End Property
        Private m_verified As Boolean
        Public Property verified As Boolean
            Get
                Return m_verified
            End Get
            Set(value As Boolean)
                m_verified = value
            End Set
        End Property
    End Class
    Public Class GenericKeyword
        Private m_keyword As String
        Public Property keyword As String
            Get
                Return m_keyword
            End Get
            Set(value As String)
                m_keyword = value
            End Set
        End Property
        Private m_code As Integer
        Public Property code As Integer
            Get
                Return m_code
            End Get
            Set(value As Integer)
                m_code = value
            End Set
        End Property
        Private m_required As Boolean
        Public Property required As Boolean
            Get
                Return m_required
            End Get
            Set(value As Boolean)
                m_required = value
            End Set
        End Property
    End Class
    Public Class GenericKeywordRequired
        Private m_value As List(Of Models.GenericKeyword)
        Public Property value As List(Of Models.GenericKeyword)
            Get
                Return m_value
            End Get
            Set(value As List(Of Models.GenericKeyword))
                m_value = value
            End Set
        End Property
        Private m_required As Boolean
        Public Property required As Boolean
            Get
                Return m_required
            End Get
            Set(value As Boolean)
                m_required = value
            End Set
        End Property

    End Class
    Public Class GenericPrice
        Private m_value As String
        Public Property value As String
            Get
                Return m_value
            End Get
            Set(value As String)
                m_value = value
            End Set
        End Property
        Private m_name As String
        Public Property name As String
            Get
                Return m_name
            End Get
            Set(value As String)
                m_name = value
            End Set
        End Property
        Private m_type As List(Of Models.GenericSearchList)
        Public Property type As List(Of Models.GenericSearchList)
            Get
                Return m_type
            End Get
            Set(value As List(Of Models.GenericSearchList))
                m_type = value
            End Set
        End Property
        Private m_price1 As Double 'LLG 11.05.2015 change string to double for globalization
        Public Property price1 As Double
            Get
                Return m_price1
            End Get
            Set(value As Double)
                m_price1 = value
            End Set
        End Property
        Private m_price2 As Double 'LLG 11.05.2015 change string to double for globalization
        Public Property price2 As Double
            Get
                Return m_price2
            End Get
            Set(value As Double)
                m_price2 = value
            End Set
        End Property
    End Class
    Public Class GenericDate
        Private m_value As String
        Public Property value As String
            Get
                Return m_value
            End Get
            Set(value As String)
                m_value = value
            End Set
        End Property
        Private m_name As String
        Public Property name As String
            Get
                Return m_name
            End Get
            Set(value As String)
                m_name = value
            End Set
        End Property

        Private m_date1 As DateTime
        Public Property date1 As DateTime
            Get
                Return m_date1
            End Get
            Set(value As DateTime)
                m_date1 = value
            End Set
        End Property

        Private m_date2 As DateTime
        Public Property date2 As DateTime
            Get
                Return m_date2
            End Get
            Set(value As DateTime)
                m_date2 = value
            End Set
        End Property
    End Class
    Public Class GenericPublicationDate
        Private m_value As String
        Public Property value As String
            Get
                Return m_value
            End Get
            Set(value As String)
                m_value = value
            End Set
        End Property
        Private m_name As String
        Public Property name As String
            Get
                Return m_name
            End Get
            Set(value As String)
                m_name = value
            End Set
        End Property
        Private m_date As DateTime
        Public Property [date] As DateTime
            Get
                Return m_date
            End Get
            Set(value As DateTime)
                m_date = value
            End Set
        End Property
    End Class
    Public Class SearchRecipeData
        Private m_codeuser As Integer
        Public Property codeuser As Integer
            Get
                Return m_codeuser
            End Get
            Set(value As Integer)
                m_codeuser = value
            End Set
        End Property
        Private m_codesite As Integer
        Public Property codesite As Integer
            Get
                Return m_codesite
            End Get
            Set(value As Integer)
                m_codesite = value
            End Set
        End Property
        Private m_codetrans As Integer
        Public Property codetrans As Integer
            Get
                Return m_codetrans
            End Get
            Set(value As Integer)
                m_codetrans = value
            End Set
        End Property
        Private m_codesetprice As Integer
        Public Property codesetprice As Integer
            Get
                Return m_codesetprice
            End Get
            Set(value As Integer)
                m_codesetprice = value
            End Set
        End Property
        Private m_Name As Models.SearchRecipeName
        Public Property name As Models.SearchRecipeName
            Get
                Return m_Name
            End Get
            Set(value As Models.SearchRecipeName)
                m_Name = value
            End Set
        End Property
        Private m_Number As Models.SearchRecipeNumber
        Public Property number As Models.SearchRecipeNumber
            Get
                Return m_Number
            End Get
            Set(value As Models.SearchRecipeNumber)
                m_Number = value
            End Set
        End Property

        Private m_category As Models.GenericSearchList
        Public Property category As Models.GenericSearchList
            Get
                Return m_category
            End Get
            Set(value As Models.GenericSearchList)
                m_category = value
            End Set
        End Property

        Private m_brand As Models.GenericKeywordRequired
        Public Property brand As Models.GenericKeywordRequired
            Get
                Return m_brand
            End Get
            Set(value As Models.GenericKeywordRequired)
                m_brand = value
            End Set
        End Property
        Private m_unwantedBrand As Models.GenericKeywordRequired
        Public Property unwantedBrand As Models.GenericKeywordRequired
            Get
                Return m_unwantedBrand
            End Get
            Set(value As Models.GenericKeywordRequired)
                m_unwantedBrand = value
            End Set
        End Property
        Private m_recipeStatus As Models.GenericSearchList
        Public Property recipeStatus As Models.GenericSearchList
            Get
                Return m_recipeStatus
            End Get
            Set(value As Models.GenericSearchList)
                m_recipeStatus = value
            End Set
        End Property
        Private m_image As Models.GenericSearchList
        Public Property image As Models.GenericSearchList
            Get
                Return m_image
            End Get
            Set(value As Models.GenericSearchList)
                m_image = value
            End Set
        End Property
        Private m_keyword As Models.GenericKeywordRequired
        Public Property keyword As Models.GenericKeywordRequired
            Get
                Return m_keyword
            End Get
            Set(value As Models.GenericKeywordRequired)
                m_keyword = value
            End Set
        End Property
        Private m_unwantedKeyword As Models.GenericKeywordRequired
        Public Property unwantedKeyword As Models.GenericKeywordRequired
            Get
                Return m_unwantedKeyword
            End Get
            Set(value As Models.GenericKeywordRequired)
                m_unwantedKeyword = value
            End Set
        End Property
        Private m_language As Models.SearchRecipeLanguage
        Public Property language As Models.SearchRecipeLanguage
            Get
                Return m_language
            End Get
            Set(value As Models.SearchRecipeLanguage)
                m_language = value
            End Set
        End Property
        Private m_source As Models.GenericSearchList
        Public Property source As Models.GenericSearchList
            Get
                Return m_source
            End Get
            Set(value As Models.GenericSearchList)
                m_source = value
            End Set
        End Property
        Private m_filter As Models.GenericSearchList
        Public Property filter As Models.GenericSearchList
            Get
                Return m_filter
            End Get
            Set(value As Models.GenericSearchList)
                m_filter = value
            End Set
        End Property
        Private m_merchandise As Models.GenericKeywordRequired
        Public Property merchandise As Models.GenericKeywordRequired
            Get
                Return m_merchandise
            End Get
            Set(value As Models.GenericKeywordRequired)
                m_merchandise = value
            End Set
        End Property
        Private m_unwantedMerchandise As Models.GenericKeywordRequired
        Public Property unwantedMerchandise As Models.GenericKeywordRequired
            Get
                Return m_unwantedMerchandise
            End Get
            Set(value As Models.GenericKeywordRequired)
                m_unwantedMerchandise = value
            End Set
        End Property
        Private m_markedItems As Models.GenericSearchList
        Public Property markedItems As Models.GenericSearchList
            Get
                Return m_markedItems
            End Get
            Set(value As Models.GenericSearchList)
                m_markedItems = value
            End Set
        End Property
        Private m_usedAsIngredient As Models.GenericSearchList
        Public Property usedAsIngredient As Models.GenericSearchList
            Get
                Return m_usedAsIngredient
            End Get
            Set(value As Models.GenericSearchList)
                m_usedAsIngredient = value
            End Set
        End Property
        Private m_price As Models.GenericPrice
        Public Property price As Models.GenericPrice
            Get
                Return m_price
            End Get
            Set(value As Models.GenericPrice)
                m_price = value
            End Set
        End Property
        Private m_date As Models.GenericDate
        Public Property [date] As Models.GenericDate
            Get
                Return m_date
            End Get
            Set(value As Models.GenericDate)
                m_date = value
            End Set
        End Property
        Private m_publication As Models.GenericSearchList
        Public Property publication As Models.GenericSearchList
            Get
                Return m_publication
            End Get
            Set(value As Models.GenericSearchList)
                m_publication = value
            End Set
        End Property
        Private m_publicationDate As Models.GenericPublicationDate
        Public Property publicationDate As Models.GenericPublicationDate
            Get
                Return m_publicationDate
            End Get
            Set(value As Models.GenericPublicationDate)
                m_publicationDate = value
            End Set
        End Property
    End Class
    Public Class GenericSearch

        Private m_code As Integer
        Public Property code As Integer
            Get
                Return m_code
            End Get
            Set(value As Integer)
                m_code = value
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
        Private m_primarybrand As String = ""
        Public Property primarybrand As String
            Get
                Return m_primarybrand
            End Get
            Set(value As String)
                m_primarybrand = value
            End Set
        End Property

        Private m_secondarybrand As String = ""
        Public Property secondarybrand As String
            Get
                Return m_secondarybrand
            End Get
            Set(value As String)
                m_secondarybrand = value
            End Set
        End Property
        Private m_recipeStatus As String = ""
        Public Property recipeStatus As String
            Get
                Return m_recipeStatus
            End Get
            Set(value As String)
                m_recipeStatus = value
            End Set
        End Property
        Private m_subname As String = ""
        Public Property subname As String
            Get
                Return m_subname
            End Get
            Set(value As String)
                m_subname = value
            End Set
        End Property
        Private m_number As String = ""
        Public Property number As String
            Get
                Return m_number
            End Get
            Set(value As String)
                m_number = value
            End Set
        End Property

        Private m_price As Double
        Public Property price As Double
            Get
                Return m_price
            End Get
            Set(value As Double)
                m_price = value
            End Set
        End Property

        Private m_nutrition As Boolean = False
        Public Property [nutrition] As Boolean
            Get
                Return m_nutrition
            End Get
            Set(value As Boolean)
                m_nutrition = value
            End Set
        End Property
        Private m_image As Boolean = False
        Public Property [image] As Boolean
            Get
                Return m_image
            End Get
            Set(value As Boolean)
                m_image = value
            End Set
        End Property
        Private m_picturename As String = ""
        Public Property picturename As String
            Get
                Return m_picturename
            End Get
            Set(value As String)
                m_picturename = value
            End Set
        End Property
        Private m_category As String = ""
        Public Property category As String
            Get
                Return m_category
            End Get
            Set(value As String)
                m_category = value
            End Set
        End Property
        Private m_source As String = ""
        Public Property source As String
            Get
                Return m_source
            End Get
            Set(value As String)
                m_source = value
            End Set
        End Property
        Private m_owner As String = ""
        Public Property owner As String
            Get
                Return m_owner
            End Get
            Set(value As String)
                m_owner = value
            End Set
        End Property
        Private m_status As String = ""
        Public Property status As String
            Get
                Return m_status
            End Get
            Set(value As String)
                m_status = value
            End Set
        End Property
        Private m_issue As String = ""
        Public Property issue As String
            Get
                Return m_issue
            End Get
            Set(value As String)
                m_issue = value
            End Set
        End Property
        Private m_issuelist As List(Of String)
        Public Property issuelist As List(Of String)
            Get
                Return m_issuelist
            End Get
            Set(value As List(Of String))
                m_issuelist = value
            End Set
        End Property
        Private m_yield As Double
        Public Property yield As Double
            Get
                Return m_yield
            End Get
            Set(value As Double)
                m_yield = value
            End Set
        End Property
        Private m_yieldFormat As String = ""
        Public Property yieldFormat As String
            Get
                Return m_yieldFormat
            End Get
            Set(value As String)
                m_yieldFormat = value
            End Set
        End Property
        Private m_priceFormat As String = ""
        Public Property priceFormat As String
            Get
                Return m_priceFormat
            End Get
            Set(value As String)
                m_priceFormat = value
            End Set
        End Property
        Private m_yieldName As String = ""
        Public Property yieldName As String
            Get
                Return m_yieldName
            End Get
            Set(value As String)
                m_yieldName = value
            End Set
        End Property
        Private m_unit As String = ""
        Public Property unit As String
            Get
                Return m_unit
            End Get
            Set(value As String)
                m_unit = value
            End Set
        End Property
        Private m_calcPrice As Double
        Public Property calcPrice As Double
            Get
                Return m_calcPrice
            End Get
            Set(value As Double)
                m_calcPrice = value
            End Set
        End Property
        Private m_imposedPrice As Double
        Public Property imposedPrice As Double
            Get
                Return m_imposedPrice
            End Get
            Set(value As Double)
                m_imposedPrice = value
            End Set
        End Property
        Private m_supplier As String
        Public Property supplier As String
            Get
                Return m_supplier
            End Get
            Set(value As String)
                m_supplier = value
            End Set
        End Property
        Private m_brand As String
        Public Property brand As String
            Get
                Return m_brand
            End Get
            Set(value As String)
                m_brand = value
            End Set
        End Property
        Private m_tax As Double
        Public Property tax As Double
            Get
                Return m_tax
            End Get
            Set(value As Double)
                m_tax = value
            End Set
        End Property
        Private m_dateCreated As String 'LLG 11.05.2015 change string to date for globalization
        Public Property dateCreated As String
            Get
                Return m_dateCreated
            End Get
            Set(value As String)
                m_dateCreated = value
            End Set
        End Property
        Private m_leaf As Boolean = False
        Public Property [leaf] As Boolean
            Get
                Return m_leaf
            End Get
            Set(value As Boolean)
                m_leaf = value
            End Set
        End Property
        Private m_CheckoutUser As Integer
        Public Property CheckoutUser As Integer
            Get
                Return m_CheckoutUser
            End Get
            Set(value As Integer)
                m_CheckoutUser = value
            End Set
        End Property
        Private m_Contains As String
        Public Property Contains As String
            Get
                Return m_Contains
            End Get
            Set(value As String)
                m_Contains = value
            End Set
        End Property
        Private m_NonAllergens As String
        Public Property NonAllergens As String
            Get
                Return m_NonAllergens
            End Get
            Set(value As String)
                m_NonAllergens = value
            End Set
        End Property
        Private m_CompleteAllergen As Boolean
        Public Property CompleteAllergen As Boolean
            Get
                Return m_CompleteAllergen
            End Get
            Set(value As Boolean)
                m_CompleteAllergen = value
            End Set
        End Property
        Private m_Currency As String
        Public Property Currency As String
            Get
                Return m_Currency
            End Get
            Set(value As String)
                m_Currency = value
            End Set
        End Property
        Private m_FoodCost As Double
        Public Property FoodCost As Double
            Get
                Return m_FoodCost
            End Get
            Set(value As Double)
                m_FoodCost = value
            End Set
        End Property
        Private m_FoodCostPercent As Double
        Public Property FoodCostPercent As Double
            Get
                Return m_FoodCostPercent
            End Get
            Set(value As Double)
                m_FoodCostPercent = value
            End Set
        End Property
        Private m_GrossMargin As Double
        Public Property GrossMargin As Double
            Get
                Return m_GrossMargin
            End Get
            Set(value As Double)
                m_GrossMargin = value
            End Set
        End Property
        Private m_GrossMarginPercent As Double
        Public Property GrossMarginPercent As Double
            Get
                Return m_GrossMarginPercent
            End Get
            Set(value As Double)
                m_GrossMarginPercent = value
            End Set
        End Property
        Private m_NetMargin As Double
        Public Property NetMargin As Double
            Get
                Return m_NetMargin
            End Get
            Set(value As Double)
                m_NetMargin = value
            End Set
        End Property
        Private m_NetMarginPercent As Double
        Public Property NetMarginPercent As Double
            Get
                Return m_NetMarginPercent
            End Get
            Set(value As Double)
                m_NetMarginPercent = value
            End Set
        End Property
        Private m_ImposedSellingPriceWOTax As Double
        Public Property ImposedSellingPriceWOTax As Double
            Get
                Return m_ImposedSellingPriceWOTax
            End Get
            Set(value As Double)
                m_ImposedSellingPriceWOTax = value
            End Set
        End Property
        Private m_ImposedSellingPriceWTax As Double
        Public Property ImposedSellingPriceWTax As Double
            Get
                Return m_ImposedSellingPriceWTax
            End Get
            Set(value As Double)
                m_ImposedSellingPriceWTax = value
            End Set
        End Property
        'added for migros LLG 06.05.2016
        Private m_PIMFlag As Integer
        Public Property PIMFlag As Integer
            Get
                Return m_PIMFlag
            End Get
            Set(value As Integer)
                m_PIMFlag = value
            End Set
        End Property
        Private m_pimstatus As String = ""
        Public Property pimstatus As String
            Get
                Return m_pimstatus
            End Get
            Set(value As String)
                m_pimstatus = value
            End Set
        End Property
        Private m_datetested As Date
        Public Property DateTested As Date
            Get
                Return m_datetested
            End Get
            Set(value As Date)
                m_datetested = value
            End Set
        End Property

        Private m_withTranslation As Integer
        Public Property withTranslation As Integer
            Get
                Return m_withTranslation
            End Get
            Set(value As Integer)
                m_withTranslation = value
            End Set
        End Property

        Private m_IsLocked As Boolean
        Public Property IsLocked As Boolean
            Get
                Return m_IsLocked
            End Get
            Set(value As Boolean)
                m_IsLocked = value
            End Set
        End Property

    End Class

    Public Class ResponseGenericSearch
        Private m_Data As List(Of Models.GenericSearch)
        Public Property data As List(Of Models.GenericSearch)
            Get
                Return m_Data
            End Get
            Set(value As List(Of Models.GenericSearch))
                m_Data = value
            End Set
        End Property
        Private m_Count As Integer
        Public Property totalCount As Integer
            Get
                Return m_Count
            End Get
            Set(value As Integer)
                m_Count = value
            End Set
        End Property
        Public Sub New(data As List(Of Models.GenericSearch), _
            totalCount As Integer)
            Me.totalCount = totalCount
            Me.data = data
        End Sub
    End Class

    Public Class MenuplanSearch
        Private m_CheckoutUser As Integer
        Public Property CheckoutUser As Integer
            Get
                Return m_CheckoutUser
            End Get
            Set(value As Integer)
                m_CheckoutUser = value
            End Set
        End Property
        Private _code As Integer
        Public Property code() As Integer
            Get
                Return _code
            End Get
            Set(ByVal value As Integer)
                _code = value
            End Set
        End Property
        Private _Name As String
        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property
        Private _number As String
        Public Property number() As String
            Get
                Return _number
            End Get
            Set(ByVal value As String)
                _number = value
            End Set
        End Property
        Private m_restaurant As String
        Public Property restaurant() As String
            Get
                Return m_restaurant
            End Get
            Set(ByVal value As String)
                m_restaurant = value
            End Set
        End Property
        Private m_codeRestaurant As Integer
        Public Property codeRestaurant() As Integer
            Get
                Return m_codeRestaurant
            End Get
            Set(ByVal value As Integer)
                m_codeRestaurant = value
            End Set
        End Property
        Private m_cyclePlan As Boolean
        Public Property cyclePlan() As Boolean
            Get
                Return m_cyclePlan
            End Get
            Set(ByVal value As Boolean)
                m_cyclePlan = value
            End Set
        End Property
        Private _startDate As String
        Public Property startDate() As String
            Get
                Return _startDate
            End Get
            Set(ByVal value As String)
                _startDate = value
            End Set
        End Property
        Private _duration As Integer
        Public Property duration() As Integer
            Get
                Return _duration
            End Get
            Set(ByVal value As Integer)
                _duration = value
            End Set
        End Property
        Private _recurrence As String
        Public Property recurrence() As String
            Get
                Return _recurrence
            End Get
            Set(ByVal value As String)
                _recurrence = value
            End Set
        End Property
        Private _totalCost As String
        Public Property totalCost() As String
            Get
                Return _totalCost
            End Get
            Set(ByVal value As String)
                _totalCost = value
            End Set
        End Property
        Private _category As String
        Public Property category() As String
            Get
                Return _category
            End Get
            Set(ByVal value As String)
                _category = value
            End Set
        End Property
        Private _season As String
        Public Property season() As String
            Get
                Return _season
            End Get
            Set(ByVal value As String)
                _season = value
            End Set
        End Property
        Private _serviceType As String
        Public Property serviceType() As String
            Get
                Return _serviceType
            End Get
            Set(ByVal value As String)
                _serviceType = value
            End Set
        End Property
    End Class

    Public Class ResponseMenuPlanSearch
        Private m_Data As List(Of Models.MenuplanSearch)
        Public Property data As List(Of Models.MenuplanSearch)
            Get
                Return m_Data
            End Get
            Set(value As List(Of Models.MenuplanSearch))
                m_Data = value
            End Set
        End Property
        Private m_Count As Integer
        Public Property totalCount As Integer
            Get
                Return m_Count
            End Get
            Set(value As Integer)
                m_Count = value
            End Set
        End Property
        Public Sub New(data As List(Of Models.MenuplanSearch), _
            totalCount As Integer)
            Me.totalCount = totalCount
            Me.data = data

        End Sub
    End Class
    Public Class TextSearch
        Private _Name
        Public Property Name As String
            Get
                Return _Name
            End Get
            Set(value As String)
                _Name = value

            End Set
        End Property
        Private _CodeTrans
        Public Property CodeTrans As Integer
            Get
                Return _CodeTrans
            End Get
            Set(value As Integer)
                _CodeTrans = value
            End Set
        End Property
        Private _namefilter
        Public Property namefilter As Integer
            Get
                Return _namefilter
            End Get
            Set(value As Integer)
                _namefilter = value
            End Set
        End Property
    End Class
    Public Class ResponseSearchText
        Private m_Data As List(Of Models.TextSearch)
        Public Property data As List(Of Models.TextSearch)
            Get
                Return m_Data
            End Get
            Set(value As List(Of Models.TextSearch))
                m_Data = value
            End Set
        End Property
        Private m_Count As Integer
        Public Property totalCount As Integer
            Get
                Return m_Count
            End Get
            Set(value As Integer)
                m_Count = value
            End Set
        End Property
        Public Sub New(data As List(Of Models.TextSearch), _
            totalCount As Integer)
            Me.totalCount = totalCount
            Me.data = data
        End Sub
    End Class
End Namespace
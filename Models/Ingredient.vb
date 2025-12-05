Namespace CalcmenuAPI.Models
    Public Class IngredientData
        Private m_Info As Models.Ingredient
        Public Property Info As Models.Ingredient
            Get
                Return m_Info
            End Get
            Set(value As Models.Ingredient)
                m_Info = value
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
    End Class
    Public Class Ingredient
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
        Private m_Name As String
        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = value
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
        Private m_Price As Double
        Public Property Price As Double
            Get
                Return m_Price
            End Get
            Set(value As Double)
                m_Price = value
            End Set
        End Property
        Private m_UnitName As String
        Public Property UnitName As String
            Get
                Return m_UnitName
            End Get
            Set(value As String)
                m_UnitName = value
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
        Private m_UnitImperial As String
        Public Property UnitImperial As String
            Get
                Return m_UnitImperial
            End Get
            Set(value As String)
                m_UnitImperial = value
            End Set
        End Property
        Private m_CodeUnit As Integer
        Public Property CodeUnit As Integer
            Get
                Return m_CodeUnit
            End Get
            Set(value As Integer)
                m_CodeUnit = value
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
        Private m_CodeUnitImperial As Integer
        Public Property CodeUnitImperial As Integer
            Get
                Return m_CodeUnitImperial
            End Get
            Set(value As Integer)
                m_CodeUnitImperial = value
            End Set
        End Property
        Private m_CategoryName As String
        Public Property CategoryName As String
            Get
                Return m_CategoryName
            End Get
            Set(value As String)
                m_CategoryName = value
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
        Private m_SourceName As String
        Public Property SourceName As String
            Get
                Return m_SourceName
            End Get
            Set(value As String)
                m_SourceName = value
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
        Private m_BrandName As String
        Public Property BrandName As String
            Get
                Return m_BrandName
            End Get
            Set(value As String)
                m_BrandName = value
            End Set
        End Property
        Private m_SupplierName As String
        Public Property SupplierName As String
            Get
                Return m_SupplierName
            End Get
            Set(value As String)
                m_SupplierName = value
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
        Private m_Wastage5 As Integer
        Public Property Wastage5 As Integer
            Get
                Return m_Wastage5
            End Get
            Set(value As Integer)
                m_Wastage5 = value
            End Set
        End Property
        Private m_WastageTotal As Integer
        Public Property WastageTotal As Integer
            Get
                Return m_WastageTotal
            End Get
            Set(value As Integer)
                m_WastageTotal = value
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
        Private m_Status As String
        Public Property Status As String
            Get
                Return m_Status
            End Get
            Set(value As String)
                m_Status = value
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
        Private m_blnIsNewUnit As Boolean
        Public Property IsNewUnit As Boolean
            Get
                Return m_blnIsNewUnit
            End Get
            Set(value As Boolean)
                m_blnIsNewUnit = value
            End Set
        End Property
        Private m_Allprice As String
        Public Property Allprice As String
            Get
                Return m_Allprice
            End Get
            Set(value As String)
                m_Allprice = value
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
        Private m_isLocked As Boolean
        Public Property isLocked As Boolean
            Get
                Return m_isLocked
            End Get
            Set(value As Boolean)
                m_isLocked = value
            End Set
        End Property
        Private m_yieldIng As Double
        Public Property yieldIng As Double
            Get
                Return m_yieldIng
            End Get
            Set(value As Double)
                m_yieldIng = value
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
        Private m_Constant As Integer
        Public Property Constant As Integer
            Get
                Return m_Constant
            End Get
            Set(value As Integer)
                m_Constant = value
            End Set
        End Property
    End Class
    Public Class IngredientWeight
        Private m_Code As Integer
        Public Property Code As Integer
            Get
                Return m_Code
            End Get
            Set(value As Integer)
                m_Code = value
            End Set
        End Property
        Private m_CodeUnit As Integer
        Public Property CodeUnit As Integer
            Get
                Return m_CodeUnit
            End Get
            Set(value As Integer)
                m_CodeUnit = value
            End Set
        End Property
        Private m_Quantity As Double
        Public Property Quantity As Double
            Get
                Return m_Quantity
            End Get
            Set(value As Double)
                m_Quantity = value
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
    End Class
    Public Class IngredientWeightList
        Private m_Data As List(Of IngredientWeight)
        Public Property Data As List(Of IngredientWeight)
            Get
                Return m_Data
            End Get
            Set(value As List(Of IngredientWeight))
                m_Data = value
            End Set
        End Property
        Private m_DisplayCodeUnit As Integer
        Public Property DisplayCodeUnit As Integer
            Get
                Return m_DisplayCodeUnit
            End Get
            Set(value As Integer)
                m_DisplayCodeUnit = value
            End Set
        End Property
    End Class

    Public Class Response

        Private m_Data As List(Of Models.Ingredient)
        Public Property Data As List(Of Models.Ingredient)
            Get
                Return m_Data
            End Get
            Set(value As List(Of Models.Ingredient))
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
        Public Sub New(data As List(Of Models.Ingredient), count As Integer)
            Me.Count = count
            Me.Data = data

        End Sub
    End Class
    Public Class IngredientOnePrice
        Private m_Code As Integer
        'Private m_IngrPrice As List(Of Models.IngredientOnePrice)
        'Public Property IngrPrice As List(Of Models.IngredientOnePrice)
        '    Get
        '        Return m_IngrPrice
        '    End Get
        '    Set(value As List(Of Models.IngredientOnePrice))
        '        m_IngrPrice = value
        '    End Set
        'End Property

        Public Property Code As Integer
            Get
                Return m_Code
            End Get
            Set(value As Integer)
                m_Code = value
            End Set
        End Property
        Private m_CodeUnit As Integer
        Public Property CodeUnit As Integer
            Get
                Return m_CodeUnit
            End Get
            Set(value As Integer)
                m_CodeUnit = value
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
        Private m_Price As Double
        Public Property Price As Double
            Get
                Return m_Price
            End Get
            Set(value As Double)
                m_Price = value
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
        'Public Sub New(data As List(Of Models.IngredientOnePrice))
        '    Me.IngrPrice = data
        'End Sub
    End Class
End Namespace
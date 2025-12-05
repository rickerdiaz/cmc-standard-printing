Namespace CalcmenuAPI.Models

	Public Class Unit
		Private m_Code As Integer
		Public Property Code As Integer
			Get
				Return m_Code
			End Get
			Set(value As Integer)
				m_Code = value
			End Set
		End Property
		Private m_Value As String
		Public Property Value As String
			Get
				Return m_Value
			End Get
			Set(value As String)
				m_Value = value
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
		Private m_TypeMain As Integer
		Public Property TypeMain As Integer
			Get
				Return m_TypeMain
			End Get
			Set(value As Integer)
				m_TypeMain = value
			End Set
		End Property
		Private m_IsMetric As Integer
		Public Property IsMetric As Integer
			Get
				Return m_IsMetric
			End Get
			Set(value As Integer)
				m_IsMetric = value
			End Set
		End Property
        Private m_IsIngredient As Boolean
		Public Property IsIngredient As Boolean
			Get
				Return m_IsIngredient
			End Get
			Set(value As Boolean)
				m_IsIngredient = value
			End Set
		End Property
        Private m_IsYield As Boolean
        Public Property IsYield As Boolean
            Get
                Return m_IsYield
            End Get
            Set(value As Boolean)
                m_IsYield = value
            End Set
        End Property
        Private m_UsedAsIngredient As Integer
        Public Property UsedAsIngredient As Integer
            Get
                Return m_UsedAsIngredient
            End Get
            Set(value As Integer)
                m_UsedAsIngredient = value
            End Set
        End Property
        Private m_UsedAsYield As Integer
        Public Property UsedAsYield As Integer
            Get
                Return m_UsedAsYield
            End Get
            Set(value As Integer)
                m_UsedAsYield = value
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
		Private m_PriceUnit As String
		Public Property PriceUnit As String
			Get
				Return m_PriceUnit
			End Get
			Set(value As String)
				m_PriceUnit = value
			End Set
		End Property
		Private m_PriceFactor As Double
		Public Property PriceFactor As Double
			Get
				Return m_PriceFactor
			End Get
			Set(value As Double)
				m_PriceFactor = value
			End Set
		End Property
		Private m_FactorToMain As Double
		Public Property FactorToMain As Double
			Get
				Return m_FactorToMain
			End Get
			Set(value As Double)
				m_FactorToMain = value
			End Set
        End Property

        Private m_Name As String
        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = Value
            End Set
        End Property

        Private m_NameDisplay As String
        Public Property NameDisplay As String
            Get
                Return m_NameDisplay
            End Get
            Set(value As String)
                m_NameDisplay = Value
            End Set
        End Property

        Private m_NamePlural As String
        Public Property NamePlural As String
            Get
                Return m_NamePlural
            End Get
            Set(value As String)
                m_NamePlural = Value
            End Set
        End Property

        Private m_NameDef As String
        Public Property NameDef As String
            Get
                Return m_NameDef
            End Get
            Set(value As String)
                m_NameDef = Value
            End Set
        End Property

        Private m_AutoConversion As String
        Public Property AutoConversion As String
            Get
                Return m_AutoConversion
            End Get
            Set(value As String)
                m_AutoConversion = Value
            End Set
        End Property

        Private m_Format As String
        Public Property Format As String
            Get
                Return m_Format
            End Get
            Set(value As String)
                m_Format = Value
            End Set
        End Property

        Private m_Global As Boolean
        Public Property [Global] As Boolean
            Get
                Return m_Global
            End Get
            Set(value As Boolean)
                m_Global = value
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
        Private m_CodeSite As Integer
        Public Property CodeSite As Integer
            Get
                Return m_CodeSite
            End Get
            Set(value As Integer)
                m_CodeSite = value
            End Set
        End Property
        Private m_IsAdded As Boolean
        Public Property IsAdded As Boolean
            Get
                Return m_IsAdded
            End Get
            Set(value As Boolean)
                m_IsAdded = value
            End Set
        End Property

        Private m_IsActive As Boolean
        Public Property IsActive As Boolean
            Get
                Return m_IsActive
            End Get
            Set(value As Boolean)
                m_IsActive = value
            End Set
        End Property
    End Class
    Public Class UnitInfo
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
        Private m_Type As Integer
        Public Property Type As Integer
            Get
                Return m_Type
            End Get
            Set(value As Integer)
                m_Type = value
            End Set
        End Property
        Private m_Factor As Integer
        Public Property Factor As Integer
            Get
                Return m_Factor
            End Get
            Set(value As Integer)
                m_Factor = value
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
        Private m_TypeMain As Integer
        Public Property TypeMain As Integer
            Get
                Return m_TypeMain
            End Get
            Set(value As Integer)
                m_TypeMain = value
            End Set
        End Property
    End Class
    Public Class UnitConvert
        Private m_Value1 As Double
        Public Property Value1 As Double
            Get
                Return m_Value1
            End Get
            Set(value As Double)
                m_Value1 = value
            End Set
        End Property
        Private m_Value2 As Double
        Public Property Value2 As Double
            Get
                Return m_Value2
            End Get
            Set(value As Double)
                m_Value2 = value
            End Set
        End Property
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
    End Class

    Public Class UnitTranslation

        Private m_UsedAsIngredient As Integer
        Public Property UsedAsIngredient As Integer
            Get
                Return m_UsedAsIngredient
            End Get
            Set(value As Integer)
                m_UsedAsIngredient = value
            End Set
        End Property
        Private m_UsedAsYield As Integer
        Public Property UsedAsYield As Integer
            Get
                Return m_UsedAsYield
            End Get
            Set(value As Integer)
                m_UsedAsYield = value
            End Set
        End Property
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
        Private m_NameDisplay As String
        Public Property NameDisplay As String
            Get
                Return m_NameDisplay
            End Get
            Set(value As String)
                m_NameDisplay = value
            End Set
        End Property

        Private m_NamePlural As String
        Public Property NamePlural As String
            Get
                Return m_NamePlural
            End Get
            Set(value As String)
                m_NamePlural = value
            End Set
        End Property

        Private m_NameDef As String
        Public Property NameDef As String
            Get
                Return m_NameDef
            End Get
            Set(value As String)
                m_NameDef = value
            End Set
        End Property

        Private m_AutoConversion As String
        Public Property AutoConversion As String
            Get
                Return m_AutoConversion
            End Get
            Set(value As String)
                m_AutoConversion = value
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

        Private m_IsIngredient As Boolean
        Public Property IsIngredient As Boolean
            Get
                Return m_IsIngredient
            End Get
            Set(value As Boolean)
                m_IsIngredient = value
            End Set
        End Property
        Private m_IsYield As Boolean
        Public Property IsYield As Boolean
            Get
                Return m_IsYield
            End Get
            Set(value As Boolean)
                m_IsYield = value
            End Set
        End Property

        Private m_Global As Boolean
        Public Property [Global] As Boolean
            Get
                Return m_Global
            End Get
            Set(value As Boolean)
                m_Global = value
            End Set
        End Property
    End Class

    Public Class UnitData
        Private m_Profile As Models.User
        Public Property Profile As Models.User
            Get
                Return m_Profile
            End Get
            Set(value As Models.User)
                m_Profile = value
            End Set
        End Property
        Private m_Info As Models.Unit
        Public Property Info As Models.Unit
            Get
                Return m_Info
            End Get
            Set(value As Models.Unit)
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
        Private m_Translation As List(Of Models.UnitTranslation)
        Public Property Translation As List(Of Models.UnitTranslation)
            Get
                Return m_Translation
            End Get
            Set(value As List(Of Models.UnitTranslation))
                m_Translation = value
            End Set
        End Property
        Private m_ActionType As Integer
        Public Property ActionType As Integer
            Get
                Return m_ActionType
            End Get
            Set(value As Integer)
                m_ActionType = value
            End Set
        End Property
        Private m_MergeList As List(Of Integer)
        Public Property MergeList As List(Of Integer)
            Get
                Return m_MergeList
            End Get
            Set(value As List(Of Integer))
                m_MergeList = value
            End Set
        End Property
    End Class

    Public Class ActivateDeactivate
        Private m_Status As Integer
        Public Property Status As Integer
            Get
                Return m_Status
            End Get
            Set(value As Integer)
                m_Status = value
            End Set
        End Property

        Private m_CodesList As List(Of Integer)
        Public Property CodesList As List(Of Integer)
            Get
                Return m_CodesList
            End Get
            Set(value As List(Of Integer))
                m_CodesList = value
            End Set
        End Property

    End Class

    Public Class UnitResponseSearch
        Private m_Data As List(Of Models.Unit)
        Public Property data As List(Of Models.Unit)
            Get
                Return m_Data
            End Get
            Set(value As List(Of Models.Unit))
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
        Public Sub New(data As List(Of Models.Unit), _
           totalCount As Integer)
            Me.totalCount = totalCount
            Me.data = data
        End Sub
    End Class
End Namespace
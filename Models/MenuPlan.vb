Namespace CalcmenuAPI.Models

    Public Class MenuPlan
        Private _CodeMenuPlan As Integer
        Public Property codeMenuPlan() As Integer
            Get
                Return _CodeMenuPlan
            End Get
            Set(ByVal value As Integer)
                _CodeMenuPlan = value
            End Set
        End Property
        Private _copiedFromMPCode As Integer
        Public Property copiedFromMPCode() As Integer
            Get
                Return _copiedFromMPCode
            End Get
            Set(ByVal value As Integer)
                _copiedFromMPCode = value
            End Set
        End Property
        Private _codeRestaurantFrom As Integer
        Public Property codeRestaurantFrom() As Integer
            Get
                Return _codeRestaurantFrom
            End Get
            Set(ByVal value As Integer)
                _codeRestaurantFrom = value
            End Set
        End Property
        Private _codeRestaurantTo As Integer
        Public Property codeRestaurantTo() As Integer
            Get
                Return _codeRestaurantTo
            End Get
            Set(ByVal value As Integer)
                _codeRestaurantTo = value
            End Set
        End Property
        Private _copyRestaurant As Boolean
        Public Property copyRestaurant() As Boolean
            Get
                Return _copyRestaurant
            End Get
            Set(ByVal value As Boolean)
                _copyRestaurant = value
            End Set
        End Property

        Private _Name As String
        Public Property name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property
        Private _Number As String
        Public Property number() As String
            Get
                Return _Number
            End Get
            Set(ByVal value As String)
                _Number = value
            End Set
        End Property
        Private _Description As String
        Public Property description() As String
            Get
                Return _Description
            End Get
            Set(ByVal value As String)
                _Description = value
            End Set
        End Property
        Private _CodeRestaurant As Integer
        Public Property codeRestaurant() As Integer
            Get
                Return _CodeRestaurant
            End Get
            Set(ByVal value As Integer)
                _CodeRestaurant = value
            End Set
        End Property
        Private _CodeCategory As Integer
        Public Property codeCategory() As Integer
            Get
                Return _CodeCategory
            End Get
            Set(ByVal value As Integer)
                _CodeCategory = value
            End Set
        End Property
        Private _CodeSeason As Integer
        Public Property codeSeason() As Integer
            Get
                Return _CodeSeason
            End Get
            Set(ByVal value As Integer)
                _CodeSeason = value
            End Set
        End Property
        Private _CodeService As Integer
        Public Property codeService() As Integer
            Get
                Return _CodeService
            End Get
            Set(ByVal value As Integer)
                _CodeService = value
            End Set
        End Property
        Private _CyclePlan As Boolean
        Public Property cyclePlan() As Boolean
            Get
                Return _CyclePlan
            End Get
            Set(ByVal value As Boolean)
                _CyclePlan = value
            End Set
        End Property
        Private _StartDate As String
        Public Property startDate() As String
            Get
                Return _StartDate
            End Get
            Set(ByVal value As String)
                _StartDate = value
            End Set
        End Property
        Private _Duration As Integer
        Public Property duration() As Integer
            Get
                Return _Duration
            End Get
            Set(ByVal value As Integer)
                _Duration = value
            End Set
        End Property
        Private _Recurrence As Integer
        Public Property recurrence() As Integer
            Get
                Return _Recurrence
            End Get
            Set(ByVal value As Integer)
                _Recurrence = value
            End Set
        End Property

        Private _CodeSetPrice As Integer
        Public Property codeSetPrice() As Integer
            Get
                Return _CodeSetPrice
            End Get
            Set(ByVal value As Integer)
                _CodeSetPrice = value
            End Set
        End Property
        Private _CodeUser As Integer
        Public Property codeUser() As Integer
            Get
                Return _CodeUser
            End Get
            Set(ByVal value As Integer)
                _CodeUser = value
            End Set
        End Property
        Private _CodeTrans As Integer
        Public Property codeTrans() As Integer
            Get
                Return _CodeTrans
            End Get
            Set(ByVal value As Integer)
                _CodeTrans = value
            End Set
        End Property
        Private _source As List(Of MasterplanMapping)
        Public Property source() As List(Of MasterplanMapping)
            Get
                Return _source
            End Get
            Set(ByVal value As List(Of MasterplanMapping))
                _source = value
            End Set
        End Property
    End Class

    Public Class MasterplanMapping
        Public CodeMasterPlan As Integer
        Public CodeMasterPlanSource As Integer
    End Class

End Namespace
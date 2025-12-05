Namespace CalcmenuAPI.Models

    Public Class Tax


        Private m_TaxCode As Integer
        Public Property TaxCode As Integer
            Get
                Return m_TaxCode
            End Get
            Set(value As Integer)
                m_TaxCode = value
            End Set
        End Property
        Private m_TaxName As String
        Public Property TaxName As String
            Get
                Return m_TaxName
            End Get
            Set(value As String)
                m_TaxName = value
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
    Public Class TaxData
        Private m_Profile As Models.User
        Public Property Profile As Models.User
            Get
                Return m_Profile
            End Get
            Set(value As Models.User)
                m_Profile = value
            End Set
        End Property


        Private m_Info As Models.Tax
        Public Property Info As Models.Tax
            Get
                Return m_Info
            End Get
            Set(value As Models.Tax)
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

End Namespace
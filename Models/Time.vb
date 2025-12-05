Namespace CalcmenuAPI.Models

    Public Class TimeData
        Private m_Time As Models.Time
        Public Property Info As Models.Time
            Get
                Return m_Time
            End Get
            Set(value As Models.Time)
                m_Time = value
            End Set
        End Property
        Private m_Profile As Models.User
        Public Property Profile As Models.User
            Get
                Return m_Profile
            End Get
            Set(value As Models.User)
                m_Profile = value
            End Set
        End Property
        Private m_Sites As List(Of Models.GenericTree)
        Public Property Sites As List(Of Models.GenericTree)
            Get
                Return m_Sites
            End Get
            Set(value As List(Of Models.GenericTree))
                m_Sites = value
            End Set
        End Property

        Private m_Translations As List(Of Models.GenericTranslation)
        Public Property Translation As List(Of Models.GenericTranslation)
            Get
                Return m_Translations
            End Get
            Set(value As List(Of Models.GenericTranslation))
                m_Translations = value
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

    Public Class Time

        Private m_TimeName As String
        Public Property Name As String
            Get
                Return m_TimeName
            End Get
            Set(value As String)
                m_TimeName = value
            End Set
        End Property

        Private m_TimeCode As Integer
        Public Property Code As Integer
            Get
                Return m_TimeCode
            End Get
            Set(value As Integer)
                m_TimeCode = value
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

        Private m_CodeTrans As Integer
        Public Property CodeTrans() As Integer
            Get
                Return m_CodeTrans
            End Get
            Set(ByVal value As Integer)
                m_CodeTrans = value
            End Set
        End Property

        Private m_CodeUser As Integer
        Public Property CodeUser() As Integer
            Get
                Return m_CodeUser
            End Get
            Set(ByVal value As Integer)
                m_CodeUser = value
            End Set
        End Property

        Private m_CodeSite As Integer
        Public Property CodeSite() As Integer
            Get
                Return m_CodeSite
            End Get
            Set(ByVal value As Integer)
                m_CodeSite = value
            End Set
        End Property

        Private m_isTotal As Boolean
        Public Property [isTotal] As Boolean
            Get
                Return m_isTotal
            End Get
            Set(value As Boolean)
                m_isTotal = value
            End Set
        End Property

        Private m_RequiredTotal As Boolean
        Public Property [RequiredTotal] As Boolean
            Get
                Return m_RequiredTotal
            End Get
            Set(value As Boolean)
                m_RequiredTotal = value
            End Set
        End Property
    End Class

End Namespace
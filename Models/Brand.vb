Namespace CalcmenuAPI.Models

    Public Class Brand

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
        Private m_ParentCode As Integer
        Public Property ParentCode As Integer
            Get
                Return m_ParentCode
            End Get
            Set(value As Integer)
                m_ParentCode = value
            End Set
        End Property
        Private m_ParentName As String
        Public Property ParentName As String
            Get
                Return m_ParentName
            End Get
            Set(value As String)
                m_ParentName = value
            End Set
        End Property
        Private m_Flagged As Boolean
        Public Property Flagged As Boolean
            Get
                Return m_Flagged
            End Get
            Set(value As Boolean)
                m_Flagged = value
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
        Private m_Type As Integer
        Public Property Type As Integer
            Get
                Return m_Type
            End Get
            Set(value As Integer)
                m_Type = value
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
        Private m_Classification As Integer
        Public Property Classification As Integer
            Get
                Return m_Classification
            End Get
            Set(value As Integer)
                m_Classification = value
            End Set
        End Property
        Private m_blnCanBeParent As Boolean
        Public Property CanBeParent As Boolean
            Get
                Return m_blnCanBeParent
            End Get
            Set(value As Boolean)
                m_blnCanBeParent = value
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
    End Class

    Public Class BrandData
        Private m_Profile As Models.User
        Public Property Profile As Models.User
            Get
                Return m_Profile
            End Get
            Set(value As Models.User)
                m_Profile = value
            End Set
        End Property
        Private m_Info As Models.Brand
        Public Property Info As Models.Brand
            Get
                Return m_Info
            End Get
            Set(value As Models.Brand)
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
        Private m_Translation As List(Of Models.GenericTranslation)
        Public Property Translation As List(Of Models.GenericTranslation)
            Get
                Return m_Translation
            End Get
            Set(value As List(Of Models.GenericTranslation))
                m_Translation = value
            End Set
        End Property
        Private m_KioskList As List(Of Models.GenericList)
        Public Property KioskList As List(Of Models.GenericList)
            Get
                Return m_KioskList
            End Get
            Set(value As List(Of Models.GenericList))
                m_KioskList = value
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
Namespace CalcmenuAPI.Models

    Public Class PrefixGeneric
        Private m_CodePrefix As Integer
        Public Property CodePrefix As Integer
            Get
                Return m_CodePrefix
            End Get
            Set(value As Integer)
                m_CodePrefix = value
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
        Private m_CodeTrans As Integer
        Public Property CodeTrans As Integer
            Get
                Return m_CodeTrans
            End Get
            Set(value As Integer)
                m_CodeTrans = value
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

    Public Class Prefix
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
        Private m_Gender As String
        Public Property Gender As String
            Get
                Return m_Gender
            End Get
            Set(value As String)
                m_Gender = value
            End Set
        End Property
        Private m_TranslationCode As Integer
        Public Property TranslationCode As Integer
            Get
                Return m_TranslationCode
            End Get
            Set(value As Integer)
                m_TranslationCode = value
            End Set
        End Property
        Private m_PrefixLanguage As String
        Public Property PrefixLanguage As String
            Get
                Return m_PrefixLanguage
            End Get
            Set(value As String)
                m_PrefixLanguage = value
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
        Private m_CodeOwner As Integer
        Public Property CodeOwner As Integer
            Get
                Return m_CodeOwner
            End Get
            Set(value As Integer)
                m_CodeOwner = value
            End Set
        End Property
    End Class

    Public Class PrefixData
        Private m_Profile As Models.User
        Public Property Profile As Models.User
            Get
                Return m_Profile
            End Get
            Set(value As Models.User)
                m_Profile = value
            End Set
        End Property
        Private m_Info As Models.Prefix
        Public Property Info As Models.Prefix
            Get
                Return m_Info
            End Get
            Set(value As Models.Prefix)
                m_Info = value
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
    End Class
End Namespace


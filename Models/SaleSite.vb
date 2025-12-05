Namespace CalcmenuAPI.Models

    Public Class SaleSite

        Private m_Code As Integer
        Public Property Code As Integer
            Get
                Return m_Code
            End Get
            Set(value As Integer)
                m_Code = value
            End Set
        End Property

        Private m_LocationNumber As String
        Public Property LocationNumber As String
            Get
                Return m_LocationNumber
            End Get
            Set(value As String)
                m_LocationNumber = value
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

        Private m_Street As String
        Public Property Street As String
            Get
                Return m_Street
            End Get
            Set(value As String)
                m_Street = value
            End Set
        End Property

        Private m_ZipCode As Integer
        Public Property ZipCode As Integer
            Get
                Return m_ZipCode
            End Get
            Set(value As Integer)
                m_ZipCode = value
            End Set
        End Property

        Private m_City As String
        Public Property City As String
            Get
                Return m_City
            End Get
            Set(value As String)
                m_City = value
            End Set
        End Property

        Private m_CertificationID As String
        Public Property CertificationID As String
            Get
                Return m_CertificationID
            End Get
            Set(value As String)
                m_CertificationID = value
            End Set
        End Property

        Private m_isProductionLocation As String
        Public Property isProductionLocation As String
            Get
                Return m_isProductionLocation
            End Get
            Set(value As String)
                m_isProductionLocation = value
            End Set
        End Property

        Private m_isSalesSite As String
        Public Property isSalesSite As String
            Get
                Return m_isSalesSite
            End Get
            Set(value As String)
                m_isSalesSite = value
            End Set
        End Property

        Private m_codeLanguage As Integer
        Public Property codeLanguage As Integer
            Get
                Return m_codeLanguage
            End Get
            Set(value As Integer)
                m_codeLanguage = value
            End Set
        End Property
    End Class

    Public Class SaleSiteData
        Private m_Profile As Models.User
        Public Property Profile As Models.User
            Get
                Return m_Profile
            End Get
            Set(value As Models.User)
                m_Profile = value
            End Set
        End Property
        Private m_Info As Models.SaleSite
        Public Property Info As Models.SaleSite
            Get
                Return m_Info
            End Get
            Set(value As Models.SaleSite)
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

        Private m_Translation As List(Of Models.GenericTranslation)
        Public Property Translation As List(Of Models.GenericTranslation)
            Get
                Return m_Translation
            End Get
            Set(value As List(Of Models.GenericTranslation))
                m_Translation = value
            End Set
        End Property
    End Class

End Namespace
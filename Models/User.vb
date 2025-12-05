Namespace CalcmenuAPI.Models

    Public Class EncodedUserRights
        Public Value As String
    End Class
    Public Class UserRights
        Private m_RoleId As Integer
        Public Property RoleId As Integer
            Get
                Return m_RoleId
            End Get
            Set(value As Integer)
                m_RoleId = value
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
        Private m_RoleLevel As Integer
        Public Property RoleLevel As Integer
            Get
                Return m_RoleLevel
            End Get
            Set(value As Integer)
                m_RoleLevel = value
            End Set
        End Property
        Private m_Modules As Integer
        Public Property Modules As Integer
            Get
                Return m_Modules
            End Get
            Set(value As Integer)
                m_Modules = value
            End Set
        End Property
        Private m_Rights As Integer
        Public Property Rights As Integer
            Get
                Return m_Rights
            End Get
            Set(value As Integer)
                m_Rights = value
            End Set
        End Property
	End Class
	Public Class User
		Private m_Code As Integer = 0
		Public Property Code As Integer
			Get
				Return m_Code
			End Get
			Set(value As Integer)
				m_Code = value
			End Set
		End Property
		Private m_UserName As String
		Public Property UserName As String
			Get
				Return m_UserName
			End Get
			Set(value As String)
				m_UserName = value
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
		Private m_Email As String
		Public Property Email As String
			Get
				Return m_Email
			End Get
			Set(value As String)
				m_Email = value
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
		Private m_RoleLevel As Integer
		Public Property RoleLevel As Integer
			Get
				Return m_RoleLevel
			End Get
			Set(value As Integer)
				m_RoleLevel = value
			End Set
        End Property
        Private m_SalesSite As Integer
        Public Property SalesSite As Integer
            Get
                Return m_SalesSite
            End Get
            Set(value As Integer)
                m_SalesSite = value
            End Set
        End Property
        Private m_SalesSiteLanguage As Integer
        Public Property SalesSiteLanguage As Integer
            Get
                Return m_SalesSiteLanguage
            End Get
            Set(value As Integer)
                m_SalesSiteLanguage = value
            End Set
        End Property
        Private m_SiteName As String
        Public Property SiteName As String
            Get
                Return m_SiteName
            End Get
            Set(value As String)
                m_SiteName = value
            End Set
        End Property
	End Class
	Public Class UserData
		Private m_Info As String
		Public Property Info As String
			Get
				Return m_Info
			End Get
			Set(value As String)
				m_Info = value
			End Set
		End Property
		Private m_Config As String
		Public Property Config As String
			Get
				Return m_Config
			End Get
			Set(value As String)
				m_Config = value
			End Set
		End Property
		Private m_Rights As String
		Public Property Rights As String
			Get
				Return m_Rights
			End Get
			Set(value As String)
				m_Rights = value
			End Set
		End Property
	End Class
End Namespace
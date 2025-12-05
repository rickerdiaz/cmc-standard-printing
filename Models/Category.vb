Namespace CalcmenuAPI.Models
   
	Public Class Category
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
		Private m_Global As Boolean
		Public Property [Global] As Boolean
			Get
				Return m_Global
			End Get
			Set(value As Boolean)
				m_Global = value
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
        Private m_Picture As String
        Public Property Picture As String
            Get
                Return m_Picture
            End Get
            Set(value As String)
                m_Picture = value
            End Set
        End Property
        Private m_Archive As Integer
        Public Property Archive As Integer
            Get
                Return m_Archive
            End Get
            Set(value As Integer)
                m_Archive = value
            End Set
        End Property
	End Class
    Public Class CategoryData
        Private m_Profile As Models.User
        Public Property Profile As Models.User
            Get
                Return m_Profile
            End Get
            Set(value As Models.User)
                m_Profile = value
            End Set
        End Property
        Private m_Info As Models.Category
        Public Property Info As Models.Category
            Get
                Return m_Info
            End Get
            Set(value As Models.Category)
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
        Private m_MergeList As List(Of Integer)
        Public Property MergeList As List(Of Integer)
            Get
                Return m_MergeList
            End Get
            Set(value As List(Of Integer))
                m_MergeList = value
            End Set
		End Property       
        Private m_AutoNumber As Models.AutoNumber
		Public Property AutoNumber As Models.AutoNumber
			''Added Paulo Adaoag 2014-04-04
			Get
				Return m_AutoNumber
			End Get
			Set(value As Models.AutoNumber)
				m_AutoNumber = value
			End Set
        End Property
        Private m_Archive As Integer
        Public Property Archive As Integer
            Get
                Return m_Archive
            End Get
            Set(value As Integer)
                m_Archive = value
            End Set
        End Property
    End Class

End Namespace
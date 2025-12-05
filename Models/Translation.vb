Namespace CalcmenuAPI.Models
	Public Class Translation
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
		Private m_CodeDict As Integer
		Public Property CodeDict As Integer
			Get
				Return m_CodeDict
			End Get
			Set(value As Integer)
				m_CodeDict = value
			End Set
        End Property
        Private m_Codetrans As Integer
        Public Property Codetrans As Integer
            Get
                Return m_Codetrans
            End Get
            Set(value As Integer)
                m_Codetrans = value
            End Set
        End Property
    End Class

    Partial Public Class Translation
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


End Namespace
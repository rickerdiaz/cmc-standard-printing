Namespace CalcmenuAPI.Models
    Public Class Country

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

        Private m_Abbr As String
        Public Property Abbr As String
            Get
                Return m_Abbr
            End Get
            Set(value As String)
                m_Abbr = value
            End Set
        End Property
    End Class
End Namespace


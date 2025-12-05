Namespace CalcmenuAPI.Models
    Public Class Keyword
        Inherits GenericItem
        Private m_Type As Integer
        Public Property Type As Integer
            Get
                Return m_Type
            End Get
            Set(value As Integer)
                m_Type = value
            End Set
        End Property
    End Class
    Public Class KeywordData
        Inherits GenericData
        Private m_Info As Models.Keyword
        Public Overloads Property Info As Models.Keyword
            Get
                Return m_Info
            End Get
            Set(value As Models.Keyword)
                m_Info = value
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
    End Class
End Namespace
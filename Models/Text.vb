Namespace CalcmenuAPI.Models

    Public Class Text


        Private m_TextCode As Integer
        Public Property TextCode As Integer
            Get
                Return m_TextCode
            End Get
            Set(value As Integer)
                m_TextCode = value
            End Set
        End Property
        Private m_TextName As String
        Public Property TextName As String
            Get
                Return m_TextName
            End Get
            Set(value As String)
                m_TextName = value
            End Set
        End Property
        Private m_TextDate As String
        Public Property TextDate As String
            Get
                Return m_TextDate
            End Get
            Set(value As String)
                m_TextDate = value
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
    Public Class TextData
        Private m_Profile As Models.User
        Public Property Profile As Models.User
            Get
                Return m_Profile
            End Get
            Set(value As Models.User)
                m_Profile = value
            End Set
        End Property


        Private m_Info As Models.Text
        Public Property Info As Models.Text
            Get
                Return m_Info
            End Get
            Set(value As Models.Text)
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
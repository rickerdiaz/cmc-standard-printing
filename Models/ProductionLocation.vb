Namespace CalcmenuAPI.Models
    Public Class ProductionLocation
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

        Private m_IsGlobal As Boolean
        Public Property IsGlobal As Boolean
            Get
                Return m_IsGlobal
            End Get
            Set(value As Boolean)
                m_IsGlobal = value
            End Set
        End Property
    End Class

    Public Class ProductionLocationData
        Private m_Profile As Models.User
        Public Property Profile As Models.User
            Get
                Return m_Profile
            End Get
            Set(value As Models.User)
                m_Profile = value
            End Set
        End Property

        Private m_Info As Models.ProductionLocation
        Public Property Info As Models.ProductionLocation
            Get
                Return m_Info
            End Get
            Set(value As Models.ProductionLocation)
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

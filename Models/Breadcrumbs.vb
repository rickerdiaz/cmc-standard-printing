Namespace CalcmenuAPI.Models

    Public Class Breadcrumbs
        Private m_intCodeUser As Integer
        Private m_intCodeListe As Integer
        Private m_intTab As Integer
        Private m_intSave As Integer
        Private m_intListeType As Integer
        Public Property CodeUser As Integer
            Get
                Return m_intCodeUser
            End Get
            Set(value As Integer)
                m_intCodeUser = value
            End Set
        End Property
        Public Property ListeItemType As Integer
            Get
                Return m_intListeType
            End Get
            Set(value As Integer)
                m_intListeType = value
            End Set
        End Property
        Public Property CodeListe As Integer
            Get
                Return m_intCodeListe
            End Get
            Set(value As Integer)
                m_intCodeListe = value
            End Set
        End Property
        Public Property Tab As Integer
            Get
                Return m_intTab
            End Get
            Set(value As Integer)
                m_intTab = value
            End Set
        End Property
        Public Property Save As Integer
            Get
                Return m_intSave
            End Get
            Set(value As Integer)
                m_intSave = value
            End Set
        End Property
    End Class
    Public Class BreadcrumbsData
        Private m_Info As Models.Breadcrumbs
        Public Property Info As Models.Breadcrumbs
            Get
                Return m_Info
            End Get
            Set(value As Models.Breadcrumbs)
                m_Info = value
            End Set
        End Property
    End Class

End Namespace
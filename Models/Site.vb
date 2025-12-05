Namespace CalcmenuAPI.Models

    Public Class Site
        Private m_Name As String
        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = value
            End Set
        End Property
        Private m_RefName As String
        Public Property RefName As String
            Get
                Return m_RefName
            End Get
            Set(value As String)
                m_RefName = value
            End Set
        End Property
        Private m_Group As Integer
        Public Property Group As Integer
            Get
                Return m_Group
            End Get
            Set(value As Integer)
                m_Group = value
            End Set
        End Property
        Private m_SiteLevel As Integer
        Public Property SiteLevel As Integer
            Get
                Return m_SiteLevel
            End Get
            Set(value As Integer)
                m_SiteLevel = value
            End Set
        End Property
    End Class
    Public Class SiteData
        Private m_Info As New Models.Site
        Public Property Info As Models.Site
            Get
                Return m_Info
            End Get
            Set(value As Models.Site)
                m_Info = value
            End Set
        End Property
        Private m_Translation As List(Of Models.Translation)
        Public Property Translation As List(Of Models.Translation)
            Get
                Return m_Translation
            End Get
            Set(value As List(Of Models.Translation))
                m_Translation = value
            End Set
        End Property
        Private m_Autonumber As List(Of Models.GenericList)
        Public Property Autonumber As List(Of Models.GenericList)
            Get
                Return m_Autonumber
            End Get
            Set(value As List(Of Models.GenericList))
                m_Autonumber = value
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
        Private m_NutrientSet As List(Of Models.GenericList)
        Public Property NutrientSet As List(Of Models.GenericList)
            Get
                Return m_NutrientSet
            End Get
            Set(value As List(Of Models.GenericList))
                m_NutrientSet = value
            End Set
        End Property
        Private m_SetOfPrice As List(Of Models.GenericList)
        Public Property SetOfPrice As List(Of Models.GenericList)
            Get
                Return m_SetOfPrice
            End Get
            Set(value As List(Of Models.GenericList))
                m_SetOfPrice = value
            End Set
        End Property
        Private m_Tax As List(Of Models.Tax)
        Public Property Tax As List(Of Models.Tax)
            Get
                Return m_Tax
            End Get
            Set(value As List(Of Models.Tax))
                m_Tax = value
            End Set
        End Property
        Private m_Units As List(Of Models.Unit)
        Public Property Units As List(Of Models.Unit)
            Get
                Return m_Units
            End Get
            Set(value As List(Of Models.Unit))
                m_Units = value
            End Set
        End Property
        Private m_CodeClient As Integer
        Public Property CodeClient() As Integer
            Get
                Return m_CodeClient
            End Get
            Set(ByVal value As Integer)
                m_CodeClient = value
            End Set
        End Property

    End Class

End Namespace
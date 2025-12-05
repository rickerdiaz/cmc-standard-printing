Namespace CalcmenuAPI.Models

    Public Class Printer
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
        Private m_SaleSiteName As String
        Public Property SaleSiteName As String
            Get
                Return m_SaleSiteName
            End Get
            Set(value As String)
                m_SaleSiteName = value
            End Set
        End Property
        Private m_IsGlobal As Boolean
        Public Property [IsGlobal] As Boolean
            Get
                Return m_IsGlobal
            End Get
            Set(value As Boolean)
                m_IsGlobal = value
            End Set
        End Property
        Private m_Status As Integer
        Public Property Status As Integer
            Get
                Return m_Status
            End Get
            Set(value As Integer)
                m_Status = value
            End Set
        End Property
        Private m_CodeSaleSite As Integer
        Public Property CodeSaleSite As Integer
            Get
                Return m_CodeSaleSite
            End Get
            Set(value As Integer)
                m_CodeSaleSite = value
            End Set
        End Property
    End Class
    Public Class PrinterData
        Private m_Profile As Models.User
        Public Property Profile As Models.User
            Get
                Return m_Profile
            End Get
            Set(value As Models.User)
                m_Profile = value
            End Set
        End Property
        Private m_Info As Models.Printer
        Public Property Info As Models.Printer
            Get
                Return m_Info
            End Get
            Set(value As Models.Printer)
                m_Info = value
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
        Private m_Sharing As List(Of Models.GenericList)
        Public Property Sharing As List(Of Models.GenericList)
            Get
                Return m_Sharing
            End Get
            Set(value As List(Of Models.GenericList))
                m_Sharing = value
            End Set
        End Property
    End Class
End Namespace


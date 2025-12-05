Namespace CalcmenuAPI.Models
    Public Class Supplier

        Private m_Code As Integer
        Public Property Code As Integer
            Get
                Return m_Code
            End Get
            Set(value As Integer)
                m_Code = value
            End Set
        End Property

        Private m_Number As String
        Public Property Number As String
            Get
                Return m_Number
            End Get
            Set(value As String)
                m_Number = value
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

        Private m_Comapny As String
        Public Property Company As String
            Get
                Return m_Comapny
            End Get
            Set(value As String)
                m_Comapny = value
            End Set
        End Property

        Private m_Address1 As String
        Public Property Address1 As String
            Get
                Return m_Address1
            End Get
            Set(value As String)
                m_Address1 = value
            End Set
        End Property

        Private m_Address2 As String
        Public Property Address2 As String
            Get
                Return m_Address2
            End Get
            Set(value As String)
                m_Address2 = value
            End Set
        End Property

        Private m_ZipCode As String
        Public Property ZipCode As String
            Get
                Return m_ZipCode
            End Get
            Set(value As String)
                m_ZipCode = value
            End Set
        End Property

        Private m_State As String
        Public Property State As String
            Get
                Return m_State
            End Get
            Set(value As String)
                m_State = value
            End Set
        End Property

        Private m_Country As String
        Public Property Country As String
            Get
                Return m_Country
            End Get
            Set(value As String)
                m_Country = value
            End Set
        End Property

        Private m_PhoneNumber As String
        Public Property PhoneNumber As String
            Get
                Return m_PhoneNumber
            End Get
            Set(value As String)
                m_PhoneNumber = value
            End Set
        End Property

        Private m_Fax As String
        Public Property Fax As String
            Get
                Return m_Fax
            End Get
            Set(value As String)
                m_Fax = value
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

        Private m_URL As String
        Public Property URL As String
            Get
                Return m_URL
            End Get
            Set(value As String)
                m_URL = value
            End Set
        End Property

        Private m_Remark As String
        Public Property Remark As String
            Get
                Return m_Remark
            End Get
            Set(value As String)
                m_Remark = value
            End Set
        End Property

        Private m_Note As String
        Public Property Note As String
            Get
                Return m_Note
            End Get
            Set(value As String)
                m_Note = value
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

        Private m_City As String
        Public Property City As String
            Get
                Return m_City
            End Get
            Set(value As String)
                m_City = value
            End Set
        End Property
        Private m_CodeUser As Integer
        Public Property CodeUser As Integer
            Get
                Return m_CodeUser
            End Get
            Set(value As Integer)
                m_CodeUser = value
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

        Private m_ActionType As Integer
        Public Property ActionType As Integer
            Get
                Return m_ActionType
            End Get
            Set(value As Integer)
                m_ActionType = value
            End Set
        End Property
    End Class

    Public Class SupplierData
        Private m_Profile As Models.User
        Public Property Profile As Models.User
            Get
                Return m_Profile
            End Get
            Set(value As Models.User)
                m_Profile = value
            End Set
        End Property
        Private m_Info As Models.Supplier
        Public Property Info As Models.Supplier
            Get
                Return m_Info
            End Get
            Set(value As Models.Supplier)
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

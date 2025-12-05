Namespace CalcmenuAPI.Models

    Public Class ResponseCookBook

        Private m_Data As List(Of Models.TreeNode)
        Public Property children As List(Of Models.TreeNode)
            Get
                Return m_Data
            End Get
            Set(value As List(Of Models.TreeNode))
                m_Data = value
            End Set
        End Property
        Private m_Count As Integer
        Public Property totalCount As Integer
            Get
                Return m_Count
            End Get
            Set(value As Integer)
                m_Count = value
            End Set
        End Property
        Public Sub New(data As List(Of Models.TreeNode), _
            count As Integer)
            Me.totalCount = count
            Me.children = data

        End Sub
    End Class
    Public Class Cookbook
        Inherits GenericItem
        Private m_CodeOwner As Integer


        Public Property CodeOwner As Integer
            Get
                Return m_CodeOwner
            End Get
            Set(value As Integer)
                m_CodeOwner = value
            End Set
        End Property
        Private m_blnCanBeParent As Boolean
        Public Property CanBeParent As Boolean
            Get
                Return m_blnCanBeParent
            End Get
            Set(value As Boolean)
                m_blnCanBeParent = value
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
        Private m_hasPicture As Boolean
        Public Property hasPicture As Boolean
            Get
                Return m_hasPicture
            End Get
            Set(value As Boolean)
                m_hasPicture = value
            End Set
        End Property
    End Class
    Public Class CookbookData
        Private m_Profile As Models.User
        Public Property Profile As Models.User
            Get
                Return m_Profile
            End Get
            Set(value As Models.User)
                m_Profile = value
            End Set
        End Property

        Private m_Info As Models.Cookbook
        Public Property Info As Models.Cookbook
            Get
                Return m_Info
            End Get
            Set(value As Models.Cookbook)
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
        Private m_Users As List(Of Models.GenericList)
        Public Property Users As List(Of Models.GenericList)
            Get
                Return m_Users
            End Get
            Set(value As List(Of Models.GenericList))
                m_Users = value
            End Set
        End Property
    End Class
    Public Class ResponseCookBookRecipe

        Private m_Data As List(Of Models.TreeGridNode)
        Public Property Data As List(Of Models.TreeGridNode)
            Get
                Return m_Data
            End Get
            Set(value As List(Of Models.TreeGridNode))
                m_Data = value
            End Set
        End Property
        Private m_Count As Integer
        Public Property totalCount As Integer
            Get
                Return m_Count
            End Get
            Set(value As Integer)
                m_Count = value
            End Set
        End Property
        Public Sub New(data As List(Of Models.TreeGridNode), _
            count As Integer)
            Me.totalCount = count
            Me.Data = data

        End Sub
    End Class
End Namespace
Namespace CalcmenuAPI.Models

    Public Class RecipeLinkList
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
        Private m_LinkDescription As Integer
        Public Property LinkDescription As Integer
            Get
                Return m_LinkDescription
            End Get
            Set(value As Integer)
                m_LinkDescription = value
            End Set
        End Property
        Private m_Enabled As Boolean
        Public Property Enabled As Boolean
            Get
                Return m_Enabled
            End Get
            Set(value As Boolean)
                m_Enabled = value
            End Set
        End Property
    End Class

    Public Class MenuLinkList
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
        Private m_LinkDescription As Integer
        Public Property LinkDescription As Integer
            Get
                Return m_LinkDescription
            End Get
            Set(value As Integer)
                m_LinkDescription = value
            End Set
        End Property
        Private m_Enabled As Boolean
        Public Property Enabled As Boolean
            Get
                Return m_Enabled
            End Get
            Set(value As Boolean)
                m_Enabled = value
            End Set
        End Property
    End Class

    Public Class UsedLanguages
        Private m_Code As Integer
        Public Property Code As Integer
            Get
                Return m_Code
            End Get
            Set(value As Integer)
                m_Code = value
            End Set
        End Property
        Private m_Language As String
        Public Property Language As String
            Get
                Return m_Language
            End Get
            Set(value As String)
                m_Language = value
            End Set
        End Property
        Private m_CodeRef As Integer
        Public Property CodeRef As Integer
            Get
                Return m_CodeRef
            End Get
            Set(value As Integer)
                m_CodeRef = value
            End Set
        End Property
    End Class

    Public Class ResponseTreeGeneric

        Private m_Data As List(Of Models.GenericTreeNode)
        Public Property children As List(Of Models.GenericTreeNode)
            Get
                Return m_Data
            End Get
            Set(value As List(Of Models.GenericTreeNode))
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
        Public Sub New(data As List(Of Models.GenericTreeNode), _
            count As Integer)
            Me.totalCount = count
            Me.children = data

        End Sub
    End Class

    Public Class ResponseCallBack
        Private m_Status As Boolean
        Private m_Message As String
        Private m_ReturnValue As String
        Private m_Code As Integer = 0
        Private m_Parameters As List(Of param)

        Public Property Status As Boolean
            Get
                Return m_Status
            End Get
            Set(value As Boolean)
                m_Status = value
            End Set
        End Property
        Public Property Message As String
            Get
                Return m_Message
            End Get
            Set(value As String)
                m_Message = value
            End Set
        End Property
        Public Property ReturnValue As String
            Get
                Return m_ReturnValue
            End Get
            Set(value As String)
                m_ReturnValue = value
            End Set
        End Property
        Public Property Code As Integer
            Get
                Return m_Code
            End Get
            Set(value As Integer)
                m_Code = value
            End Set
        End Property
        Public Property Parameters As List(Of param)
            Get
                Return m_Parameters
            End Get
            Set(value As List(Of param))
                m_Parameters = value
            End Set
        End Property
    End Class
    Public Class param
        Public name As String
        Public value As String
    End Class

    Public Class GenericCodeValueList
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
    End Class

    Public Class GenericList
        Private m_Code As Integer
        Public Property Code As Integer
            Get
                Return m_Code
            End Get
            Set(value As Integer)
                m_Code = value
            End Set
        End Property
        Private m_CodeDictionary As Integer
        Public Property CodeDictionary As Integer
            Get
                Return m_CodeDictionary
            End Get
            Set(value As Integer)
                m_CodeDictionary = value
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
        Private m_Value2 As String
        Public Property Value2 As String
            Get
                Return m_Value2
            End Get
            Set(ByVal value As String)
                m_Value2 = value
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
        Private m_Global As Boolean
        Public Property [Global]() As Boolean
            Get
                Return m_Global
            End Get
            Set(ByVal value As Boolean)
                m_Global = value
            End Set
        End Property

        Private m_IsParent As Boolean
        Public Property IsParent As Boolean
            Get
                Return m_IsParent
            End Get
            Set(value As Boolean)
                m_IsParent = value
            End Set
        End Property
    End Class
    Public Class GenericItem
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
        Private m_Global As Boolean
        Public Property [Global] As Boolean
            Get
                Return m_Global
            End Get
            Set(value As Boolean)
                m_Global = value
            End Set
        End Property
        Private m_ParentCode As Integer
        Public Property ParentCode As Integer
            Get
                Return m_ParentCode
            End Get
            Set(value As Integer)
                m_ParentCode = value
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
        Private m_Inheritable As Boolean
        Public Property [Inheritable] As Boolean
            Get
                Return m_Inheritable
            End Get
            Set(value As Boolean)
                m_Inheritable = value
            End Set
        End Property
        Private m_CodeTrans As Integer
        Public Property CodeTrans As Integer
            Get
                Return m_CodeTrans
            End Get
            Set(ByVal value As Integer)
                m_CodeTrans = value
            End Set
        End Property

        Private m_IsParent As Boolean
        Public Property [IsParent] As Boolean
            Get
                Return m_IsParent
            End Get
            Set(value As Boolean)
                m_IsParent = value
            End Set
        End Property
    End Class

    Public Class GenericTree
        Friend Parent As Integer
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
        Private m_Number As String
        Public Property Number As String
            Get
                Return m_Number
            End Get
            Set(value As String)
                m_Number = value
            End Set
        End Property
        Private m_ParentCode As Integer
        Public Property ParentCode As Integer
            Get
                Return m_ParentCode
            End Get
            Set(value As Integer)
                m_ParentCode = value
            End Set
        End Property
        Private m_ParentName As String
        Public Property ParentName As String
            Get
                Return m_ParentName
            End Get
            Set(value As String)
                m_ParentName = value
            End Set
        End Property
        Private m_Flagged As Boolean
        Public Property [Flagged] As Boolean
            Get
                Return m_Flagged
            End Get
            Set(value As Boolean)
                m_Flagged = value
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
        Private m_Type As Integer
        Public Property Type As Integer
            Get
                Return m_Type
            End Get
            Set(value As Integer)
                m_Type = value
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
        Private m_link As String
        Public Property link As String
            Get
                Return m_link
            End Get
            Set(value As String)
                m_link = value
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
        Private m_CanBeParent As Boolean
        Public Property CanBeParent As Boolean
            Get
                Return m_CanBeParent
            End Get
            Set(value As Boolean)
                m_CanBeParent = value
            End Set
        End Property
    End Class
    Public Class GenericData
        Private m_Info As Models.GenericItem
        Public Property Info As Models.GenericItem
            Get
                Return m_Info
            End Get
            Set(value As Models.GenericItem)
                m_Info = value
            End Set
        End Property
        Private m_Translation As List(Of Models.GenericTranslation)
        Public Property Translation As List(Of Models.GenericTranslation)
            Get
                Return m_Translation
            End Get
            Set(value As List(Of Models.GenericTranslation))
                m_Translation = value
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
        Private m_Profile As Models.User
        Public Property Profile As Models.User
            Get
                Return m_Profile
            End Get
            Set(value As Models.User)
                m_Profile = value
            End Set
        End Property
        Private m_Classification As Integer
        Public Property Classification As Integer
            Get
                Return m_Classification
            End Get
            Set(value As Integer)
                m_Classification = value
            End Set
        End Property
    End Class
    Public Class GenericTranslation
        Private m_Id As Integer
        Public Property Id As Integer
            Get
                Return m_Id
            End Get
            Set(value As Integer)
                m_Id = value
            End Set
        End Property
        Private m_Code As Integer
        Public Property Code As Integer
            Get
                Return m_Code
            End Get
            Set(value As Integer)
                m_Code = value
            End Set
        End Property
        Private m_CodeTrans As Integer
        Public Property CodeTrans As Integer
            Get
                Return m_CodeTrans
            End Get
            Set(value As Integer)
                m_CodeTrans = value
            End Set
        End Property
        Private m_TranslationName As String
        Public Property TranslationName As String
            Get
                Return m_TranslationName
            End Get
            Set(value As String)
                m_TranslationName = value
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
        Private m_Name2 As String
        Public Property Name2 As String
            Get
                Return m_Name2
            End Get
            Set(value As String)
                m_Name2 = value
            End Set
        End Property
        Private m_NamePlural As String
        Public Property NamePlural As String
            Get
                Return m_NamePlural
            End Get
            Set(value As String)
                m_NamePlural = value
            End Set
        End Property
        Private m_CodeEgswTable As Integer
        Public Property CodeEgswTable As Integer
            Get
                Return m_CodeEgswTable
            End Get
            Set(value As Integer)
                m_CodeEgswTable = value
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
    End Class
    Public Class GenericDeleteData
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
        Private m_CodeProperty As Integer
        Public Property CodeProperty As Integer
            Get
                Return m_CodeProperty
            End Get
            Set(value As Integer)
                m_CodeProperty = value
            End Set
        End Property
        Private m_Type As Integer
        Public Property Type As Integer
            Get
                Return m_Type
            End Get
            Set(value As Integer)
                m_Type = value
            End Set
        End Property
        Private m_CodeList As List(Of Models.GenericList)
        Public Property CodeList As List(Of Models.GenericList)
            Get
                Return m_CodeList
            End Get
            Set(value As List(Of Models.GenericList))
                m_CodeList = value
            End Set
        End Property
        Private m_ForceDelete As Boolean
        Public Property ForceDelete As Boolean
            Get
                Return m_ForceDelete
            End Get
            Set(value As Boolean)
                m_ForceDelete = value
            End Set
        End Property
        Private m_Codes As Integer()
        Public Property Codes() As Integer()
            Get
                Return m_Codes
            End Get
            Set(ByVal value As Integer())
                m_Codes = value
            End Set
        End Property
    End Class
    Public Class GenericTreeNode
        Private m_title As String
        Public Property title As String
            Get
                Return m_title
            End Get
            Set(value As String)
                m_title = value
            End Set
        End Property
        Private m_id As Integer
        Public Property id As Integer
            Get
                Return m_id
            End Get
            Set(value As Integer)
                m_id = value
            End Set
        End Property
        Private m_collapsed As Boolean
        Public Property collapsed As Boolean
            Get
                Return True
            End Get
            Set(value As Boolean)
                m_collapsed = value
            End Set
        End Property

        Private m_children As List(Of Models.GenericTreeNode)
        Public Property children As List(Of Models.GenericTreeNode)
            Get
                Return m_children
            End Get
            Set(value As List(Of Models.GenericTreeNode))
                m_children = value
            End Set
        End Property
    End Class
    Public Class TreeNode
        Private m_title As String
        Public Property title As String
            Get
                Return m_title
            End Get
            Set(value As String)
                m_title = value
            End Set
        End Property
        Private m_key As Integer
        Public Property key As Integer
            Get
                Return m_key
            End Get
            Set(value As Integer)
                m_key = value
            End Set
        End Property
        Private m_unselectable As Boolean = False
        Public Property unselectable As Boolean
            Get
                Return m_unselectable
            End Get
            Set(value As Boolean)
                m_unselectable = value
            End Set
        End Property
        Private m_icon As Boolean = False
        Public Property icon As Boolean
            Get
                Return m_icon
            End Get
            Set(value As Boolean)
                m_icon = value
            End Set
        End Property
        Protected m_children As List(Of Models.TreeNode)
        Public Property children As List(Of Models.TreeNode)
            Get
                Return m_children
            End Get
            Set(value As List(Of Models.TreeNode))
                m_children = value
            End Set
        End Property
        Private m_select As Boolean
        Public Property [select] As Boolean
            Get
                Return m_select
            End Get
            Set(value As Boolean)
                m_select = value
            End Set
        End Property
        Private m_parenttitle As String
        Public Property parenttitle As String
            Get
                Return m_parenttitle
            End Get
            Set(value As String)
                m_parenttitle = value
            End Set
        End Property

        Private m_ParentCode As Integer
        Public Property ParentCode As Integer
            Get
                Return m_ParentCode
            End Get
            Set(value As Integer)
                m_ParentCode = value
            End Set
        End Property
        Private m_note As String
        Public Property note As String
            Get
                Return m_note
            End Get
            Set(value As String)
                m_note = value
            End Set
        End Property
        Private m_CanBeParent As Boolean
        Public Property CanBeParent As Boolean
            Get
                Return m_CanBeParent
            End Get
            Set(value As Boolean)
                m_CanBeParent = value
            End Set
        End Property
        Private m_addClass As String = ""
        Public Property addClass As String
            Get
                Return m_addClass
            End Get
            Set(value As String)
                m_addClass = value
            End Set
        End Property
        Private m_link As String
        Public Property link As String
            Get
                Return m_link
            End Get
            Set(value As String)
                m_link = value
            End Set
        End Property
        Private m_selected As Boolean
        Public Property [selected] As Boolean
            Get
                Return m_selected
            End Get
            Set(value As Boolean)
                m_selected = value
            End Set
        End Property
        Private m_groupLevel As Integer     'MKAM 2014.10.07
        Public Property groupLevel As Integer
            Get
                Return m_groupLevel
            End Get
            Set(ByVal value As Integer)
                m_groupLevel = value
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
        Private m_Picture As String
        Public Property Picture As String
            Get
                Return m_Picture
            End Get
            Set(value As String)
                m_Picture = value
            End Set
        End Property
    End Class
    Public Class TreeGridNode
        Private m_title As String
        Public Property title As String
            Get
                Return m_title
            End Get
            Set(value As String)
                m_title = value
            End Set
        End Property
        Private m_key As Integer
        Public Property key As Integer
            Get
                Return m_key
            End Get
            Set(value As Integer)
                m_key = value
            End Set
        End Property
        Private m_price As String = ""
        Public Property price As String
            Get
                Return m_price
            End Get
            Set(value As String)
                m_price = value
            End Set
        End Property
        Private m_subname As String = ""
        Public Property subname As String
            Get
                Return m_subname
            End Get
            Set(value As String)
                m_subname = value
            End Set
        End Property
        Private m_number As String = ""
        Public Property number As String
            Get
                Return m_number
            End Get
            Set(value As String)
                m_number = value
            End Set
        End Property
        Private m_primarybrand As String = ""
        Public Property primarybrand As String
            Get
                Return m_primarybrand
            End Get
            Set(value As String)
                m_primarybrand = value
            End Set
        End Property
        Private m_secondarybrand As String = ""
        Public Property secondarybrand As String
            Get
                Return m_secondarybrand
            End Get
            Set(value As String)
                m_secondarybrand = value
            End Set
        End Property
        Private m_status As String = ""
        Public Property status As String
            Get
                Return m_status
            End Get
            Set(value As String)
                m_status = value
            End Set
        End Property
        Private m_nutrition As Boolean = False
        Public Property [nutrition] As Boolean
            Get
                Return m_nutrition
            End Get
            Set(value As Boolean)
                m_nutrition = value
            End Set
        End Property
        Private m_image As Boolean = False
        Public Property [image] As Boolean
            Get
                Return m_image
            End Get
            Set(value As Boolean)
                m_image = value
            End Set
        End Property
        Private m_children As List(Of Models.TreeGridNode)
        Public Property children As List(Of Models.TreeGridNode)
            Get
                Return m_children
            End Get
            Set(value As List(Of Models.TreeGridNode))
                m_children = value
            End Set
        End Property

        Private m_parenttitle As String
        Public Property parenttitle As String
            Get
                Return m_parenttitle
            End Get
            Set(value As String)
                m_parenttitle = value
            End Set
        End Property
        Private m_iconcls As String
        Public Property iconcls As String
            Get
                Return m_iconcls
            End Get
            Set(value As String)
                m_iconcls = value
            End Set
        End Property
        Private m_expanded As Boolean = False
        Public Property [expanded] As Boolean
            Get
                Return m_expanded
            End Get
            Set(value As Boolean)
                m_expanded = value
            End Set
        End Property
        Private m_note As String
        Public Property note As String
            Get
                Return m_note
            End Get
            Set(value As String)
                m_note = value
            End Set
        End Property
        Private m_leaf As Boolean = False
        Public Property [leaf] As Boolean
            Get
                Return m_leaf
            End Get
            Set(value As Boolean)
                m_leaf = value
            End Set
        End Property
        Private m_CheckoutUser As Integer
        Public Property CheckoutUser As Integer
            Get
                Return m_CheckoutUser
            End Get
            Set(value As Integer)
                m_CheckoutUser = value
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
        Private m_IsLocked As Boolean
        Public Property IsLocked As Boolean
            Get
                Return m_IsLocked
            End Get
            Set(value As Boolean)
                m_IsLocked = value
            End Set
        End Property
    End Class
    Public Class TreeData
        Private m_KeywordItem As List(Of Models.TreeNode)
        Public Property KeywordItems As List(Of Models.TreeNode)
            Get
                Return m_KeywordItem
            End Get
            Set(value As List(Of Models.TreeNode))
                m_KeywordItem = value
            End Set
        End Property
    End Class

    Public Class BrandTreeNode
        Private m_title As String
        Public Property title As String
            Get
                Return m_title
            End Get
            Set(value As String)
                m_title = value
            End Set
        End Property
        Private m_key As Integer
        Public Property key As Integer
            Get
                Return m_key
            End Get
            Set(value As Integer)
                m_key = value
            End Set
        End Property
        Private m_unselectable As Boolean = False
        Public Property unselectable As Boolean
            Get
                Return m_unselectable
            End Get
            Set(value As Boolean)
                m_unselectable = value
            End Set
        End Property
        Private m_icon As Boolean = False
        Public Property icon As Boolean
            Get
                Return m_icon
            End Get
            Set(value As Boolean)
                m_icon = value
            End Set
        End Property
        Private m_children As List(Of Models.BrandTreeNode)
        Public Property children As List(Of Models.BrandTreeNode)
            Get
                Return m_children
            End Get
            Set(value As List(Of Models.BrandTreeNode))
                m_children = value
            End Set
        End Property
        Private m_select As Boolean
        Public Property [select] As Boolean
            Get
                Return m_select
            End Get
            Set(value As Boolean)
                m_select = value
            End Set
        End Property
        Private m_parenttitle As String
        Public Property parenttitle As String
            Get
                Return m_parenttitle
            End Get
            Set(value As String)
                m_parenttitle = value
            End Set
        End Property
        Private m_note As String
        Public Property note As String
            Get
                Return m_note
            End Get
            Set(value As String)
                m_note = value
            End Set
        End Property
        Private m_addClass As String = ""
        Public Property addClass As String
            Get
                Return m_addClass
            End Get
            Set(value As String)
                m_addClass = value
            End Set
        End Property

        Private m_Classification As Integer
        Public Property classification As Integer
            Get
                Return m_Classification
            End Get
            Set(value As Integer)
                m_Classification = value
            End Set
        End Property

        Private m_picture As String
        Public Property picture As String
            Get
                Return m_picture
            End Get
            Set(value As String)
                m_picture = value
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
    Public Class ProjectRecipe
        Friend Parent As Integer
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
        Private m_Subname As String
        Public Property Subname As String
            Get
                Return m_Subname
            End Get
            Set(value As String)
                m_Subname = value
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
        Private m_PrimaryBrand As String
        Public Property PrimaryBrand As String
            Get
                Return m_PrimaryBrand
            End Get
            Set(value As String)
                m_PrimaryBrand = value
            End Set
        End Property
        Private m_SecondaryBrand As String
        Public Property SecondaryBrand As String
            Get
                Return m_SecondaryBrand
            End Get
            Set(value As String)
                m_SecondaryBrand = value
            End Set
        End Property
        Private m_Status As String
        Public Property Status As String
            Get
                Return m_Status
            End Get
            Set(value As String)
                m_Status = value
            End Set
        End Property
        Private m_Nutrition As Boolean
        Public Property Nutrition As Boolean
            Get
                Return m_Nutrition
            End Get
            Set(value As Boolean)
                m_Nutrition = value
            End Set
        End Property
        Private m_Image As Boolean
        Public Property Image As Boolean
            Get
                Return m_Image
            End Get
            Set(value As Boolean)
                m_Image = value
            End Set
        End Property
        Private m_CheckoutUser As Integer
        Public Property CheckoutUser As Integer
            Get
                Return m_CheckoutUser
            End Get
            Set(value As Integer)
                m_CheckoutUser = value
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
        Private m_isLocked As Boolean
        Public Property isLocked As Boolean
            Get
                Return m_isLocked
            End Get
            Set(value As Boolean)
                m_isLocked = value
            End Set
        End Property
    End Class

    Public Class AutoNumber
        ''Added Paulo Adaoag 2014-04-04
        ''Auto numbering
        Private m_ItemType As Integer
        Public Property ItemType As Integer
            Get
                Return m_ItemType
            End Get
            Set(value As Integer)
                m_ItemType = value
            End Set
        End Property
        Private m_AutoNumberCodeSite As Integer
        Public Property AutoNumberCodeSite As Integer
            Get
                Return m_AutoNumberCodeSite
            End Get
            Set(value As Integer)
                m_AutoNumberCodeSite = value
            End Set
        End Property
        Private m_AutoNumber As Boolean
        Public Property AutoNumber As Boolean
            Get
                Return m_AutoNumber
            End Get
            Set(value As Boolean)
                m_AutoNumber = value
            End Set
        End Property
        Private m_AutoNumberPrefix As String
        Public Property AutoNumberPrefix As String
            Get
                Return m_AutoNumberPrefix
            End Get
            Set(value As String)
                m_AutoNumberPrefix = value
            End Set
        End Property
        Private m_AutoNumberStart As Integer
        Public Property AutoNumberStart As Integer
            Get
                Return m_AutoNumberStart
            End Get
            Set(value As Integer)
                m_AutoNumberStart = value
            End Set
        End Property
        Private m_AutoNumberKeepPrefixLength As Boolean
        Public Property AutoNumberKeepPrefixLength As Boolean
            Get
                Return m_AutoNumberKeepPrefixLength
            End Get
            Set(value As Boolean)
                m_AutoNumberKeepPrefixLength = value
            End Set
        End Property
    End Class

    Public Class ListeFiles
        Private m_Code As Integer
        Public Property Code As Integer
            Get
                Return m_Code
            End Get
            Set(value As Integer)
                m_Code = value
            End Set
        End Property
        Private m_Pictures As String
        Public Property Pictures As String
            Get
                Return m_Pictures
            End Get
            Set(value As String)
                m_Pictures = value
            End Set
        End Property
        Private m_Videos As String
        Public Property Videos As String
            Get
                Return m_Videos
            End Get
            Set(value As String)
                m_Videos = value
            End Set
        End Property
    End Class


    Public Class ConfigurationcSearch
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


        Private m_CodeProperty As Integer
        Public Property CodeProperty As Integer
            Get
                Return m_CodeProperty
            End Get
            Set(value As Integer)
                m_CodeProperty = value
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

        Private m_CodeTrans As Integer
        Public Property CodeTrans() As Integer
            Get
                Return m_CodeTrans
            End Get
            Set(ByVal value As Integer)
                m_CodeTrans = value
            End Set
        End Property

        Private m_Type As Integer
        Public Property Type() As Integer
            Get
                Return m_Type
            End Get
            Set(ByVal value As Integer)
                m_Type = value
            End Set
        End Property

        Private m_CodeUser As String
        Public Property CodeUser() As String
            Get
                Return m_CodeUser
            End Get
            Set(ByVal value As String)
                m_CodeUser = value
            End Set
        End Property

        Private m_CodeListe As String
        Public Property CodeListe() As String
            Get
                Return m_CodeListe
            End Get
            Set(ByVal value As String)
                m_CodeListe = value
            End Set
        End Property

        'Raqi Pinili 08.26.2015
        Private m_Status As Integer
        Public Property Status() As Integer
            Get
                Return m_Status
            End Get
            Set(ByVal value As Integer)
                m_Status = value
            End Set
        End Property

        'Raqi Pinili 08.26.2015
        Private m_Tree As Boolean
        Public Property Tree() As Integer
            Get
                Return m_Tree
            End Get
            Set(ByVal value As Integer)
                m_Tree = value
            End Set
        End Property

        'Raqi Pinili 08.26.2015
        Private m_Link As Boolean
        Public Property Link() As Integer
            Get
                Return m_Link
            End Get
            Set(ByVal value As Integer)
                m_Link = value
            End Set
        End Property

        'NBG 09.11.2015
        Private m_merchandiseyield As Integer
        Public Property MerchandiseYield() As Integer
            Get
                Return m_merchandiseyield
            End Get
            Set(ByVal value As Integer)
                m_merchandiseyield = value
            End Set
        End Property

        Private m_skip As Integer
        Public Property Skip() As Integer
            Get
                Return m_skip
            End Get
            Set(ByVal value As Integer)
                m_skip = value
            End Set
        End Property

        Private m_rowsperpage As Integer
        Public Property RowsPerPage() As Integer
            Get
                Return m_rowsperpage
            End Get
            Set(ByVal value As Integer)
                m_rowsperpage = value
            End Set
        End Property
    End Class
End Namespace
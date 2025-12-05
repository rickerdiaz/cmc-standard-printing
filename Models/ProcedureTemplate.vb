Namespace CalcmenuAPI.Models

    
    Public Class ProcedureTemplate
        Private m_Info As Models.ProcedureTemplateInfo
        Public Property Info As Models.ProcedureTemplateInfo
            Get
                Return m_Info
            End Get
            Set(value As Models.ProcedureTemplateInfo)
                m_Info = value
            End Set
        End Property
        Private m_translations As List(Of Models.RecipeTranslation)
        Public Property translations As List(Of Models.RecipeTranslation)
            Get
                Return m_translations
            End Get
            Set(value As List(Of Models.RecipeTranslation))
                m_translations = value
            End Set
        End Property
        Private m_Procedures As List(Of Models.RecipeProcedure)
        Public Property Procedures As List(Of Models.RecipeProcedure)
            Get
                Return m_Procedures
            End Get
            Set(value As List(Of Models.RecipeProcedure))
                m_Procedures = value
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
        Private m_Global As Boolean
        Public Property [Global] As Boolean
            Get
                Return m_Global
            End Get
            Set(value As Boolean)
                m_Global = value
            End Set
        End Property
        Private m_TempPictures As String
        Public Property TempPictures As String
            Get
                Return m_TempPictures
            End Get
            Set(value As String)
                m_TempPictures = value
            End Set
        End Property
    End Class
    Public Class ProcedureTemplateInfo
        Private m_CodeUser As Integer
        Public Property CodeUser As Integer
            Get
                Return m_CodeUser
            End Get
            Set(value As Integer)
                m_CodeUser = value
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
        Private m_Type As Integer
        Public Property Type As Integer
            Get
                Return m_Type
            End Get
            Set(value As Integer)
                m_Type = value
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
        Private m_CodeSite As Integer
        Public Property CodeSite As Integer
            Get
                Return m_CodeSite
            End Get
            Set(value As Integer)
                m_CodeSite = value
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
    End Class
End Namespace

Namespace CalcmenuAPI.Models
    Public Class Merchandise
        Public Property Name As String
        Public Property Number As String
        Public Property CodeListe As Integer
        Public Property CodeSite As Integer
        Public Property CodeUser As Integer
        Public Property UPC As String
        Public Property CodeBrand As Integer
        Public Property Brand As String
        Public Property CodeCategory As Integer
        Public Property Category As String
        Public Property CodeSupplier As Integer
        Public Property Supplier As String
        Public Property CodeTrans As Integer
        Public Property DefaultPicture As Integer
        Public Property Description As String
        Public Property CodeModifiedBy As Integer
        Public Property CustomTempPictures As List(Of String)
        Public Property CustomTempAttachments As List(Of MerchandiseAttachment)
        Public Property [Date] As String
        Public Property ModifiedDate As String
        Public Property Wastage1 As Integer
        Public Property Wastage2 As Integer
        Public Property Wastage3 As Integer
        Public Property Wastage4 As Integer
        Public Property Wastage5 As Integer
        Public Property Picture As List(Of String)
        Public Property InUse As Boolean
        Public Property CodeSetPrice As Integer
        Public Property CodeLink As String
        Public Property cGlobal As Boolean
        Public Property AllergenApproved As Boolean
        Public Property LinkNutrient As String
        Public Property CodeNutrientSet As Integer
        Public Property CodeCountry As Integer
        Public Property Country As String


        Public Class MerchandiseAttachment
            Private m_Id As Integer
            Public Property Id As Integer
                Get
                    Return m_Id
                End Get
                Set(value As Integer)
                    m_Id = value
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
            Private m_Resource As String
            Public Property Resource As String
                Get
                    Return m_Resource
                End Get
                Set(value As String)
                    m_Resource = value
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
            'Private m_blnDefault As Boolean
            'Public Property IsDefault As Boolean
            '    Get
            '        Return m_blnDefault
            '    End Get
            '    Set(value As Boolean)
            '        m_blnDefault = value
            '    End Set
            'End Property
        End Class

    End Class

    Public Class MerchandisePrice
        Public Property History As String
        Public Property Id As Integer
        Public Property Unit As String
        Public Property CodeUnit As Integer
        Public Property Price As Double
        Public Property Ratio As Double
        Public Property TaxCode As Integer
        Public Property TaxValue As Double
        Public Property Position As Integer
        Public Property CodeSetPrice As Integer
        Public Property IsUsed As Boolean
    End Class

    Public Class MerchandiseTranslation
        Public Property Id As Integer
        Public Property TranslationCode As Integer
        Public Property TranslationName As String
        Public Property CodeDictionary As Integer
        Public Property Name As String
        Public Property Ingredients As String
        Public Property Preparation As String
        Public Property CookingTip As String
        Public Property Refinement As String
        Public Property SpecificDetermination As String
        Public Property Storage As String
        Public Property Productivity As String
        Public Property Description As String
        Public Property PrefixCode As Integer
        Public Property PrefixName As String
        Public Property Gender As String
        Public Property IsGenderSensitive As Boolean
    End Class
    Public Class MerchandiseHistory '' TODO JTOC
        Private m_DateAudit As String
        Public Property DateAudit As String
            Get
                Return m_DateAudit
            End Get
            Set(value As String)
                m_DateAudit = value
            End Set
        End Property
        Private m_FieldName As String
        Public Property FieldName As String
            Get
                Return m_FieldName
            End Get
            Set(value As String)
                m_FieldName = value
            End Set
        End Property
        Private m_Time As String
        Public Property Time As String
            Get
                Return m_Time
            End Get
            Set(value As String)
                m_Time = value
            End Set
        End Property
        Private m_FieldCode As String
        Public Property FieldCode As String
            Get
                Return m_FieldCode
            End Get
            Set(value As String)
                m_FieldCode = value
            End Set
        End Property
        Private m_Previous As String
        Public Property Previous As String
            Get
                Return m_Previous
            End Get
            Set(value As String)
                m_Previous = value
            End Set
        End Property
        Private m_New As String
        Public Property HNew As String
            Get
                Return m_New
            End Get
            Set(value As String)
                m_New = value
            End Set
        End Property
        Private m_User As String
        Public Property User As String
            Get
                Return m_User
            End Get
            Set(value As String)
                m_User = value
            End Set
        End Property
        Private m_AuditType As String
        Public Property AuditType As String
            Get
                Return m_AuditType
            End Get
            Set(value As String)
                m_AuditType = value
            End Set
        End Property
        Private m_CodeListe As String
        Public Property CodeListe As String
            Get
                Return m_CodeListe
            End Get
            Set(value As String)
                m_CodeListe = value
            End Set
        End Property
        Private m_CodeUser As String
        Public Property CodeUser As String
            Get
                Return m_CodeUser
            End Get
            Set(value As String)
                m_CodeUser = value
            End Set
        End Property
        Private m_IsCode As String
        Public Property IsCode As String
            Get
                Return m_IsCode
            End Get
            Set(value As String)
                m_IsCode = value
            End Set
        End Property
    End Class

    Public Class MerchandiseData

        Public Property Info As Merchandise
        Public Property History As List(Of Models.MerchandiseHistory)
        Public Property Price As List(Of Models.MerchandisePrice)
        Public Property Keywords As List(Of Models.GenericTree)
        Public Property Nutrient As List(Of Models.RecipeNutrition)
        Public Property Attachment As List(Of Models.Merchandise.MerchandiseAttachment)
        Public Property Translation As List(Of Models.MerchandiseTranslation)
        Public Property Sharing As List(Of Models.TreeNode)
        Public Property Allergen As List(Of Models.ListeAllergen)
        Public Property AllergenUpdated As Boolean
        Public Property hasApprover As Boolean
        Public Property submitted As Boolean
        Public Property NextRoleLevelApprover As Integer
        Public Property TaxCode As Integer
    End Class
End Namespace


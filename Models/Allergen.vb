Namespace CalcmenuAPI.Models

    Public Class ListeAllergen
        Private m_CodeListe As Integer
        Public Property CodeListe() As Integer
            Get
                Return m_CodeListe
            End Get
            Set(ByVal value As Integer)
                m_CodeListe = value
            End Set
        End Property
        Private m_CodeAllergen As Integer
        Public Property CodeAllergen() As Integer
            Get
                Return m_CodeAllergen
            End Get
            Set(ByVal value As Integer)
                m_CodeAllergen = value
            End Set
        End Property
        Private m_AllergenName As String
        Public Property AllergenName() As String
            Get
                Return m_AllergenName
            End Get
            Set(ByVal value As String)
                m_AllergenName = value
            End Set
        End Property
        Private m_Contain As Boolean
        Public Property Contain() As Boolean
            Get
                Return m_Contain
            End Get
            Set(ByVal value As Boolean)
                m_Contain = value
            End Set
        End Property
        Private m_Trace As Boolean
        Public Property Trace() As Boolean
            Get
                Return m_Trace
            End Get
            Set(ByVal value As Boolean)
                m_Trace = value
            End Set
        End Property
        Private m_NonAllergen As Boolean
        Public Property NonAllergen() As Boolean
            Get
                Return m_NonAllergen
            End Get
            Set(ByVal value As Boolean)
                m_NonAllergen = value
            End Set
        End Property
        Private m_Derived As Boolean
        Public Property Derived() As Boolean
            Get
                Return m_Derived
            End Get
            Set(ByVal value As Boolean)
                m_Derived = value
            End Set
        End Property
        Private m_Hidden As Boolean
        Public Property Hidden() As Boolean
            Get
                Return m_Hidden
            End Get
            Set(ByVal value As Boolean)
                m_Hidden = value
            End Set
        End Property
        Private m_PictureName As String
        Public Property PictureName() As String
            Get
                Return m_PictureName
            End Get
            Set(ByVal value As String)
                m_PictureName = value
            End Set
        End Property
        Private m_Complete As Boolean
        Public Property Complete() As Boolean
            Get
                Return m_Complete
            End Get
            Set(ByVal value As Boolean)
                m_Complete = value
            End Set
        End Property
        Private m_SwissLaw As Boolean
        Public Property SwissLaw() As Boolean
            Get
                Return m_SwissLaw
            End Get
            Set(ByVal value As Boolean)
                m_SwissLaw = value
            End Set
        End Property
        Private m_EULaw As Boolean
        Public Property EULaw() As Boolean
            Get
                Return m_EULaw
            End Get
            Set(ByVal value As Boolean)
                m_EULaw = value
            End Set
        End Property

    End Class

    Public Class IngredientAllergen
        Private m_CodeListe As Integer
        Public Property CodeListe() As Integer
            Get
                Return m_CodeListe
            End Get
            Set(ByVal value As Integer)
                m_CodeListe = value
            End Set
        End Property
        Private m_CodeAllergen As Integer
        Public Property CodeAllergen() As Integer
            Get
                Return m_CodeAllergen
            End Get
            Set(ByVal value As Integer)
                m_CodeAllergen = value
            End Set
        End Property
        Private m_Contain As Boolean
        Public Property Contain() As Boolean
            Get
                Return m_Contain
            End Get
            Set(ByVal value As Boolean)
                m_Contain = value
            End Set
        End Property
        Private m_Trace As Boolean
        Public Property Trace() As Boolean
            Get
                Return m_Trace
            End Get
            Set(ByVal value As Boolean)
                m_Trace = value
            End Set 'aaa
        End Property
        Private m_NonAllergen As Boolean
        Public Property NonAllergen() As Boolean
            Get
                Return m_NonAllergen
            End Get
            Set(ByVal value As Boolean)
                m_NonAllergen = value
            End Set
        End Property

    End Class

End Namespace
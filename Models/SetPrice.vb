Namespace CalcmenuAPI.Models
    Public Class ResponseGenericSetPrice

        Private m_Data As List(Of Models.SetPrice)
        Public Property data As List(Of Models.SetPrice)
            Get
                Return m_Data
            End Get
            Set(value As List(Of Models.SetPrice))
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
        Public Sub New(data As List(Of Models.SetPrice), _
            totalCount As Integer)
            Me.totalCount = totalCount
            Me.data = data

        End Sub

        Public Structure ReturnData
            Public data As DataTable
            Public totalCount As Integer
        End Structure


    End Class
    Public Class SetPrice
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
        Private m_Format As String
        Public Property Format As String
            Get
                Return m_Format
            End Get
            Set(value As String)
                m_Format = value
            End Set
        End Property

        Private m_CodeCurrency As Integer
        Public Property CodeCurrency As Integer
            Get
                Return m_CodeCurrency
            End Get
            Set(value As Integer)
                m_CodeCurrency = value
            End Set
        End Property

        Private m_Symbole As String
        Public Property Symbole As String
            Get
                Return m_Symbole
            End Get
            Set(value As String)
                m_Symbole = value
            End Set
        End Property

        Private m_Description As String
        Public Property Description As String
            Get
                Return m_Description
            End Get
            Set(value As String)
                m_Description = value
            End Set
        End Property


        Private m_Factor As Double
        Public Property Factor As Double
            Get
                Return m_Factor
            End Get
            Set(value As Double)
                m_Factor = value
            End Set
        End Property

        Private m_Main As Boolean
        Public Property Main As Boolean
            Get
                Return m_Main
            End Get
            Set(value As Boolean)
                m_Main = value
            End Set
        End Property
        Private m_chkDisable As Boolean
        Public Property chkDisable As Boolean
            Get
                Return m_chkDisable
            End Get
            Set(value As Boolean)
                m_chkDisable = value
            End Set
        End Property
        Private m_hasMain As Integer
        Public Property hasMain As Integer
            Get
                Return m_hasMain
            End Get
            Set(value As Integer)
                m_hasMain = value
            End Set
        End Property

        Private m_EUR As Double
        Public Property EUR As Double
            Get
                Return m_EUR
            End Get
            Set(value As Double)
                m_EUR = value
            End Set
        End Property

        Private m_MED As String
        Public Property MED As Double
            Get
                Return m_MED
            End Get
            Set(value As Double)
                m_MED = value
            End Set
        End Property

        Private m_SAF As String
        Public Property SAF As Double
            Get
                Return m_SAF
            End Get
            Set(value As Double)
                m_SAF = value
            End Set
        End Property

        Private m_aas As String
        Public Property aas As Double
            Get
                Return m_aas
            End Get
            Set(value As Double)
                m_aas = value
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

        Private m_FactorToMain As Double
        Public Property FactorToMain As Double
            Get
                Return m_FactorToMain
            End Get
            Set(value As Double)
                m_FactorToMain = value
            End Set
        End Property
        Private m_isUserDefault As Boolean
        Public Property isUserDefault As Double
            Get
                Return m_isUserDefault
            End Get
            Set(value As Double)
                m_isUserDefault = value
            End Set
        End Property

    End Class
    Public Class SetPriceData
        Private m_Info As Models.SetPrice
        Public Property Info As Models.SetPrice
            Get
                Return m_Info
            End Get
            Set(value As Models.SetPrice)
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

        Private m_Profile As Models.User
        Public Property Profile As Models.User
            Get
                Return m_Profile
            End Get
            Set(value As Models.User)
                m_Profile = value
            End Set
        End Property

        
    End Class



End Namespace

Namespace CalcmenuAPI.Models
   
    Public Class [Alias]
        Private m_Code As Integer
        Public Property Code As Integer
            Get
                Return m_Code
            End Get
            Set(value As Integer)
                m_Code = value
            End Set
        End Property
        Private m_IdMain As Integer
        Public Property IdMain As Integer
            Get
                Return m_IdMain
            End Get
            Set(value As Integer)
                m_IdMain = value
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
        Private m_Name As String
        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = value
            End Set
        End Property
        Private m_Alias As String
        Public Property [Alias] As String
            Get
                Return m_Alias
            End Get
            Set(value As String)
                m_Alias = value
            End Set
        End Property
    End Class
    Public Class AliasData
        Private m_Profile As Models.User
        Public Property Profile As Models.User
            Get
                Return m_Profile
            End Get
            Set(value As Models.User)
                m_Profile = value
            End Set
        End Property
        Private m_Info As Models.Alias
        Public Property Info As Models.Alias
            Get
                Return m_Info
            End Get
            Set(value As Models.Alias)
                m_Info = value
            End Set
        End Property
        'Private m_Translation As List(Of Models.GenericTranslation)
        'Public Property Translation As List(Of Models.GenericTranslation)
        '    Get
        '        Return m_Translation
        '    End Get
        '    Set(value As List(Of Models.GenericTranslation))
        '        m_Translation = value
        '    End Set
        'End Property
        'Private m_Sharing As List(Of Models.GenericList)
        'Public Property Sharing As List(Of Models.GenericList)
        '    Get
        '        Return m_Sharing
        '    End Get
        '    Set(value As List(Of Models.GenericList))
        '        m_Sharing = value
        '    End Set
        'End Property
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
        'Private m_AutoNumber As Models.AutoNumber
        'Public Property AutoNumber As Models.AutoNumber
        '    ''Added Paulo Adaoag 2014-04-04
        '    Get
        '        Return m_AutoNumber
        '    End Get
        '    Set(value As Models.AutoNumber)
        '        m_AutoNumber = value
        '    End Set
        'End Property
    End Class

End Namespace
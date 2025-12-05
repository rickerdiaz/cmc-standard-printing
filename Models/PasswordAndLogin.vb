Namespace CalcmenuAPI.Models
   
    Public Class PasswordAndLogin
        Private m_blnEnforceStrongPassword As Integer
        Public Property EnforceStrongPassword As Boolean
            Get
                Return m_blnEnforceStrongPassword
            End Get
            Set(value As Boolean)
                m_blnEnforceStrongPassword = value
            End Set
        End Property
        Private m_strExpiresAfterNumberOfDays As String
        Public Property ExpiresAfterNumberOfDays As String
            Get
                Return m_strExpiresAfterNumberOfDays
            End Get
            Set(value As String)
                m_strExpiresAfterNumberOfDays = value
            End Set
        End Property
        Private m_intMinimumPasswordLength As Integer
        Public Property MinimumPasswordLength As Integer
            Get
                Return m_intMinimumPasswordLength
            End Get
            Set(value As Integer)
                m_intMinimumPasswordLength = value
            End Set
        End Property
        Private m_strMinimumPasswordReuse As String
        Public Property MinimumPasswordReuse As String
            Get
                Return m_strMinimumPasswordReuse
            End Get
            Set(value As String)
                m_strMinimumPasswordReuse = value
            End Set
        End Property
        Private m_intMaximumFailedLoginAttempts As Integer
        Public Property MaximumFailedLoginAttempts As Integer
            Get
                Return m_intMaximumFailedLoginAttempts
            End Get
            Set(value As Integer)
                m_intMaximumFailedLoginAttempts = value
            End Set
        End Property
        Private m_strLockoutPeriod As String
        Public Property LockoutPeriod As String
            Get
                Return m_strLockoutPeriod
            End Get
            Set(value As String)
                m_strLockoutPeriod = value
            End Set
        End Property
    End Class
    Public Class PasswordAndLoginData
        Private m_Info As Models.PasswordAndLogin
        Public Property Info As Models.PasswordAndLogin
            Get
                Return m_Info
            End Get
            Set(value As Models.PasswordAndLogin)
                m_Info = value
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
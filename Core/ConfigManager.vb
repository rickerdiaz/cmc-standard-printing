Imports log4net

Module ConfigManager

    Private ReadOnly Log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

    Public Sub Initialize()
        DebugEnabled = GetBool(ConfigurationManager.AppSettings("DebugEnabled"), False)
        ConnectionString = GetStr(ConfigurationManager.AppSettings("dsn"))
        ' Read the second connection string (MainDB) from appSettings
        MainDBConnectionString = GetStr(ConfigurationManager.AppSettings("MainDB"))
        DebugConnection = GetStr(ConfigurationManager.AppSettings("DebugDsn"))

        If String.IsNullOrEmpty(ConnectionString) Then
            Log.Fatal("The connection string cannot be empty, please check the web.config for the proper settings")
            Throw New ApplicationException("Unable to connect to database server")
        End If

        If String.IsNullOrEmpty(MainDBConnectionString) Then
            ' You can choose Fatal instead of Warn if this must be present at startup
            Log.Warn("MainDB connection string is empty. GetUserConnectionString may fail if MainDB is required.")
        End If
    End Sub

    Private m_DebugEnabled As Boolean
    Public Property DebugEnabled As Boolean
        Get
            Return m_DebugEnabled
        End Get
        Set(value As Boolean)
            m_DebugEnabled = value
        End Set
    End Property

    Private m_ConnectionString As String
    Public Property ConnectionString As String
        Get
            Return m_ConnectionString
        End Get
        Set(value As String)
            m_ConnectionString = value
        End Set
    End Property

    ' New: second connection string to the Main DB
    Private m_MainDBConnectionString As String
    Public Property MainDBConnectionString As String
        Get
            Return m_MainDBConnectionString
        End Get
        Set(value As String)
            m_MainDBConnectionString = value
        End Set
    End Property

    Private m_DebugConnection As String
    Public Property DebugConnection As String
        Get
            Return m_DebugConnection
        End Get
        Set(value As String)
            m_DebugConnection = value
        End Set
    End Property
End Module

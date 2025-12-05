Imports System.Reflection
Imports System.Configuration
Imports System.Runtime.Serialization
Imports System.IO
Imports log4net
Imports CalcmenuAPI.CalcmenuAPI
Imports System.Text
Imports System.Net.Mail
Imports System.Net.Mail.MailMessage
Imports System.Net.Mail.SmtpClient
Imports System.Net
Imports System.ComponentModel
Imports EgsData.modGlobalDeclarations
Imports EgsData.modFunctions
Imports System.Data.SqlClient

Module Common
    Private ReadOnly Log As ILog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
    Public dictSpellCheck As New Dictionary(Of String, String)() ''JDO 2014.03.31 This is for the spell check
    Dim thisLock As New Object ''JDO 2014.03.31 This is for the spell check

#Region "Private Constants"
    Public Const SP_API_GET_CategoryInfo As String = "API_GET_CategoryInfo"
    Public Const SP_API_GET_Config As String = "API_GET_Config"
    Public Const SP_API_GET_SiteInfo As String = "API_GET_SiteInfo"
    Public Const SP_API_GET_Sites As String = "API_GET_Sites"
    Public Const SP_API_GET_UnitByName As String = "API_GET_UnitByName"
    Public Const SP_API_UPDATE_Sharing As String = "API_UPDATE_Sharing"
    Public Const SP_API_UPDATE_Project As String = "API_UPDATE_Project"
    Public Const SP_API_UPDATE_ProjectUser As String = "API_UPDATE_ProjectUser"
    Public Const SP_GET_KEYWORDCODENAME As String = "GET_KEYWORDCODENAME"
    Public Const SP_API_GET_AliasInfo As String = "API_GET_AliasInfo" 'AGL 2014.06.26
    Public Const SP_API_GET_PasswordAndLoginInfo As String = "API_GET_PasswordAndLoginInfo" 'AGL 2014.09.11
#End Region

#Region "Enumerations"

    Public Enum GroupLevel
        [Global] = 0
        Site = 1
        [Property] = 2
        User = 3
    End Enum

    Public Enum ListeType As Integer
        Merchandise = 2
        Text = 4
        Recipe = 8
        Menu = 16
    End Enum

    Public Enum ListeDisplayMode
        Details = 0
        Thumbnail = 1
        List = 2
        ProjectList = 3
        NutrientView = 4
        AllergenView = 5
    End Enum

    Public Enum SetPriceType
        Purchasing = 1
        Sales = 2
    End Enum

    Public Enum UnitType As Integer
        Imperial = 0
        Metric = 1
        Neutral = 2
    End Enum

    Public Enum UnitKind As Integer
        Stock = 1
        Packaging = 2
        Ingredient = 3
        [Yield] = 4
        Transportation = 5
    End Enum

    Public Enum EgswErrorCode
        'IMPORTANT!!!  All errors are less than ZERO
        'For RnWeb: starts with -2000
        'For EgsSolution 
        '   FBControl modules: -90 - -999
        '   RecipeNet modules: -1000 - -1999
        '   Configuration related: -2000 - -2100
        'List related shared by all solution (ie. Category, Supplier, Location): consume less than -90
        OK = 0
        GeneralError = -1
        ExecuteProcedure = -2
        DuplicateExistsActive = -3 'DuplicateExists
        NothingDone = -4
        TransactionClosed = -5
        OneItemNotDeleted = -6
        InvalidCodeSite = -7
        FK = -8
        NotExists = -9
        ItemLocked = -10
        ItemClosed = -11
        DuplicateExistsInactive = -12
        RequestInProcess = -13
        RequestNotInProcess = -14
        InsufficientRoleLevel = -15
        SiteHasNoUser = -16
        MethodNotSupported = -17 'Methods that are no longer supported
        TransactionAlreadyClosed = -18
        TransactionAlreadyOpened = -19
        SalesItemNumberExists = -20
        MissingBrand = -21
        MissingCategory = -22
        MissingSource = -23
        MissingSupplier = -24
        MissingUnit = -25
        MissingListe = -26
        MissingKeyword = -27
        InvalidCodeList = -30 'list of Codes/Ids passed to the stored procedure for multiple action
        FistTran = -52
        ItemNoInvent = -60
        ItemNoLocation = -61
        InvalidStockType = -62
        CannotSwitch = -63
        InvalidSupplier = -64
        UsedAsDirectIO = -70
        CannotShareItems = -71
        CorruptConfig = -80
        InvalidRights = -81
        MergingMultipleGlobalItem = -82
        ItemAlreadyReceived = -83
        InvalidListType = -90
        InvalidTranMode = -91
        InvProdLocInOpenInvent = -92
        InvProductLockAfterDate = -93
        InvProductExistsInLatestInvent = -94
        InvTransExistsAfterBeginDate = -95
        InvNoProductsForInvent = -96
        InvSomeProductsAreInOpenInvent = -97
        NumberTooLong = -100
        NameTooLong = -101
        SupplierTooLong = -102
        CategoryToolong = -103
        DescriptionTooLong = -104
        UnitTooLong = -105
        InvalidPrice = -106
        OrdCantDelete = -110
        OrdCantCreateNewRequisition = -111 'creating new requisition for Merged PO
        OrdCantCreateAutoNumber = -112
        OrdCantMergeDifferentSuppliers = -113
        ReqNoProductsWithQuantities = -120
        DrAlreadyAttachedToInvoice = -130
        DrIsBeingUsedInOrder = -131
        DrSomeItemsAreNotYetInStock = -132
        EAUserNotInApproverList = -140
        EAUserIsNotApprover = -141
        EANotAllRequiredHaveApproved = -142
        EANotAllHaveApproved = -143
        InvalidName = -144
        CfgInvalidAutoNumberPrefixLen = -2000
        PromoCodeError = -2001
        NotApplicable = -2002 'DLS
        OneItemNotDeactivated = -2003 'DRR
        MerchandiseInUse = -2004 'AGL 2012.10.04 - CWM-1330
    End Enum

#End Region

    ''' <summary>
    ''' Gets a string.
    ''' </summary>
    ''' <param name="value">The raw string.</param>
    ''' <param name="def">The default value, returned when the raw string is <c>null</c>.</param>
    ''' <returns>The result.</returns>
    ''' <remarks>RBAJ 2012.07.04</remarks>
    Public Function GetStr(ByVal value As Object, Optional ByVal def As String = "") As String
        If value Is Nothing Then
            Return def
        ElseIf IsDBNull(value) Then
            Return def
        Else
            Return String.Concat(String.Empty, value)
        End If
    End Function

    ''' <summary>
    ''' Gets a date.
    ''' </summary>
    ''' <param name="value">The string value.</param>
    ''' <param name="def">The default value, returned when parsing fails.</param>
    ''' <returns>The result.</returns>
    ''' <remarks>RBAJ 2012.07.04</remarks>
    Public Function GetDate(ByVal value As Object, Optional ByVal def As DateTime = #1/1/1900#) As DateTime
        If value Is Nothing Then
            Return def
        End If
        Dim i As DateTime = def
        If Not DateTime.TryParse(GetStr(value), i) Then
            i = def
        End If
        Return i
    End Function

    ''' <summary>
    ''' Gets an integer.
    ''' </summary>
    ''' <param name="value">The string value.</param>
    ''' <param name="def">The default value, returned when parsing fails.</param>
    ''' <returns>The result.</returns>
    ''' <remarks>RBAJ 2012.07.04</remarks>
    Public Function GetInt(ByVal value As Object, Optional ByVal def As Integer = 0) As Integer
        If value Is Nothing Then
            Return def
        End If
        Dim i As Integer = def
        If Not Integer.TryParse(GetStr(value), i) Then
            i = def
        End If
        Return i
    End Function

    ''' <summary>
    ''' Gets a double.
    ''' </summary>
    ''' <param name="value">The string value.</param>
    ''' <param name="def">The default value, returned when parsing fails.</param>
    ''' <returns>The result.</returns>
    ''' <remarks>RBAJ 2012.07.04</remarks>
    Public Function GetDbl(ByVal value As Object, Optional ByVal def As Double = 0.0R) As Double
        If value Is Nothing Then
            Return def
        End If
        Dim i As Double = def
        If Not Double.TryParse(GetStr(value), i) Then
            i = def
        End If
        Return i
    End Function

    ''' <summary>
    ''' Gets a boolean.
    ''' </summary>
    ''' <param name="value">The string value.</param>
    ''' <param name="def">The default value, returned when parsing fails.</param>
    ''' <returns>The result.</returns>
    ''' <remarks>RBAJ 2012.07.04</remarks>
    Public Function GetBool(ByVal value As Object, Optional ByVal def As Boolean = False) As Boolean
        If value Is Nothing Then
            Return def
        Else
            If GetStr(value).ToLowerInvariant() = "yes" Then
                Return True
            ElseIf GetStr(value) = "1" Then
                Return True
            End If
            Dim b As Boolean = def
            If Not Boolean.TryParse(GetStr(value), b) Then
                b = def
            End If
            Return b
        End If
    End Function

    ''' <summary>
    ''' Prints a boolean.
    ''' </summary>
    ''' <param name="value">The value.</param>
    ''' <returns>The string result.</returns>
    ''' <remarks>RBAJ 2012.07.04</remarks>
    Public Function PrintBool(ByVal value As Boolean) As String
        Return CStr(IIf(value, "yes", "no"))
    End Function

    ''' <summary>
    ''' Preserve previous stack trace when re-throwing exceptions
    ''' </summary>
    ''' <param name="exception">Represent errors that occur during application execution.</param>
    ''' <remarks>RBAJ 2012.07.04</remarks>
    Public Sub PreserveStackTrace(exception As Exception)
        Dim preserveStackTrace As MethodInfo = GetType(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance Or BindingFlags.NonPublic)
        preserveStackTrace.Invoke(exception, Nothing)
    End Sub

    Public Function ReplaceSpecialCharacters(ByVal strStringToReplace As String) As String
        If Not strStringToReplace Is Nothing Then

            Dim strSpecialChars() As String = New String() {"[[]TM]", "[[]tm]", "[[]R]", "[[]r]", "[[]C]", "[[]c]", "[TM]", "[tm]", "[R]", "[r]", "[C]", "[c]"}
            Dim strSpecialCharsReplacement() As String = New String() {"™", "™", "®", "®", "©", "©", "™", "™", "®", "®", "©", "©"}

            For i As Integer = 0 To strSpecialChars.Length - 1
                If strStringToReplace.IndexOf(strSpecialChars(i)) > 0 Then
                    strStringToReplace = strStringToReplace.Replace(strSpecialChars(i), strSpecialCharsReplacement(i))
                End If
            Next

        End If

        Return strStringToReplace
    End Function

    ''' <summary>
    ''' Returns the physical file path that corresponds to the specified virtual path on the Web server.
    ''' </summary>
    ''' <param name="path">Path name</param>
    ''' <returns>Physical path</returns>
    ''' <remarks>RBAJ 2012.07.04</remarks>
    Public Function MapPath(ByVal path As String) As String
        If HttpContext.Current IsNot Nothing Then
            Return HttpContext.Current.Server.MapPath(path)
        End If

        Return HttpRuntime.AppDomainAppPath + path.Replace("~", String.Empty).Replace("/"c, "\"c)
    End Function

    Public Function GenericErrorResponse(message As String, statusCode As System.Net.HttpStatusCode, Optional errorCode As Integer = 0) As System.Net.Http.HttpResponseMessage
        Dim response = New Net.Http.HttpResponseMessage() With {
                              .Content = New Net.Http.StringContent("{""Error"":""" + message + """, ""ErrorCode"":""" + errorCode.ToString() + """}", Encoding.UTF8), _
                              .StatusCode = statusCode
                              }
        response.Content.Headers.ContentType = New Net.Http.Headers.MediaTypeHeaderValue("application/json")
        Return response
    End Function

    Public Function Join(ByVal arr As ArrayList, ByVal strPrefix As String, ByVal strSuffix As String, ByVal delimiter As String, Optional ByVal ValuePrefixandSuffix As String = "") As String
        Dim i As Integer = 0
        Dim str As String = ""
        While i < arr.Count
            str &= ValuePrefixandSuffix & arr(i) & ValuePrefixandSuffix & delimiter
            i += 1
        End While
        If str.Length = 0 Then Return ""
        str = strPrefix & str.Substring(0, str.Length - 1) & strSuffix
        Return str
    End Function

    Public Function ProxyEncode(text As String) As String
        Dim bytesToEncode As Byte()
        bytesToEncode = Encoding.UTF8.GetBytes(text)

        Dim encodedText As String
        encodedText = Convert.ToBase64String(bytesToEncode)
        Return encodedText
    End Function

    Public Function ProxyDecode(text As String) As String
        Dim decodedBytes As Byte()
        decodedBytes = Convert.FromBase64String(text)

        Dim decodedText As String
        decodedText = Encoding.UTF8.GetString(decodedBytes)
        Return decodedText
    End Function

    ''' <summary>
    ''' Logs method name and its parameters
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="param"></param>
    ''' <remarks></remarks>
    Public Sub LogMethodStart(ByRef source As System.Reflection.MethodBase, ByVal ParamArray param As String())
        Dim queryResults = From par In source.GetParameters() Select par.Name
        Log.Info(String.Format("Calling {0}({1})({2})", source.Name, String.Join(";", queryResults.ToList()), String.Join(";", param)))
    End Sub
    ''' <summary>
    ''' JDO 2014-03-31
    ''' Gets the Dictionary Folder indicated in web.config File 
    ''' This is for SPELL CHECKER
    ''' </summary>
    ''' <value></value>
    ''' <returns>The Dictionary Folder Location</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property DictFolder() As String
        Get
            Dim tmp As String = GetStr(ConfigurationManager.AppSettings("dict")).Trim()
            If tmp.Equals(String.Empty) Then
                tmp = Common.MapPath("dict")
            End If
            Return tmp.TrimEnd("\") + "\"
        End Get
    End Property
    ''' <summary>
    ''' JDO 2014-03-31
    ''' This is to load the dictionary file and cache it.
    ''' This is for SPELL CHECKER
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub loadSpellCheckDict()
        dictSpellCheck.Clear()
        SyncLock thisLock
            If dictSpellCheck.Count = 0 Then
                Using file As New StreamReader(DictFolder() + "en-US.dic")
                    Do While Not file.EndOfStream
                        Dim str As String = file.ReadLine()
                        dictSpellCheck.Add(str, GetSoundex(str.ToLower))
                    Loop
                End Using
            End If
        End SyncLock
    End Sub
    ''' <summary>
    ''' JDO 2014-03-31
    ''' Computes the "Soundex" value of a string.
    ''' This is for SPELL CHECKER
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSoundex(ByVal s As String) As String
        Dim str As String = ""
        If s <> "" And s.Length > 0 Then
            Dim previousCode, currentCode As String
            previousCode = ""
            str = str + s.Substring(0, 1)
            For Each chrs In s.Substring(1).ToCharArray()
                currentCode = ""
                If str.Length <> 4 Then
                    If "bfpv".IndexOf(chrs) > -1 Then
                        currentCode = "1"
                    ElseIf "cgjkqsxz".IndexOf(chrs) > -1 Then
                        currentCode = "2"
                    ElseIf "dt".IndexOf(chrs) > -1 Then
                        currentCode = "3"
                    ElseIf chrs = "l" Then
                        currentCode = "4"
                    ElseIf "mn".IndexOf(chrs) > -1 Then
                        currentCode = "5"
                    ElseIf chrs = "r" Then
                        currentCode = "6"
                    End If

                    If currentCode <> previousCode Then
                        str = str + currentCode
                    End If
                    If currentCode <> "" Then
                        previousCode = currentCode
                    End If

                End If
            Next
            If str.Length < 4 Then
                str = str + New String("0", 4 - str.Length)

            End If
        End If

        Return str
    End Function


    Public Sub SendEmail(ByVal strURL As String, ByVal errMessage As String, ByVal stackTrace As String, ByVal controller As String) 'As Integer

        Try
            If System.Configuration.ConfigurationManager.AppSettings("SMTPActivate") <> "" Then
                If Not CBool(System.Configuration.ConfigurationManager.AppSettings("SMTPActivate")) Then
                    Exit Sub
                End If
            End If

            If strURL.IndexOf(".calcmenuweb.com") > 0 Then
                If strURL.IndexOf("qa.calcmenuweb.com") > 0 Then
                    Exit Sub
                End If
            ElseIf errMessage.StartsWith("[-3]") Then   'Data Already Exists
                Exit Sub
            Else
                Exit Sub
            End If

            Dim Mail As New MailMessage
            Mail.Subject = "CALCMENU Web Error Notification - " & controller

            Dim strEmails As String = System.Configuration.ConfigurationManager.AppSettings("SMTPRecipient")
            Dim emails = strEmails.Split(",")
            For Each email In emails
                Mail.To.Add(email)
            Next
            'Mail.To.Add(System.Configuration.ConfigurationManager.AppSettings("SMTPRecipient"))
            'Mail.To.Add("support.noticket@eg-software.com")
            'Mail.To.Add("support@eg-software.com")
            Mail.From = New MailAddress(System.Configuration.ConfigurationManager.AppSettings("SMTPSender"))

            Mail.Body = "Hi Support Team,"
            Mail.Body += vbCrLf & vbCrLf & "This is to notify that an error has been occurred in CALCMENU Web application. Error details are as follows:" & vbCrLf
            Mail.Body += vbCrLf & "URL: " & vbCrLf & strURL
            Mail.Body += vbCrLf & vbCrLf & "Error Message: " & vbCrLf & errMessage
            Mail.Body += vbCrLf & vbCrLf & "Stack Trace: " & vbCrLf & stackTrace
            Mail.Body += vbCrLf & vbCrLf & vbCrLf & vbCrLf & vbCrLf & "*** This is an automatically generated email, please do not reply. ***" & vbCrLf

            Dim SMTP As New SmtpClient(System.Configuration.ConfigurationManager.AppSettings("SMTPClient"))
            SMTP.EnableSsl = True
            SMTP.Credentials = New System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings("SMTPSender"), System.Configuration.ConfigurationManager.AppSettings("SMTPSenderPass"))
            SMTP.Port = CInt(System.Configuration.ConfigurationManager.AppSettings("SMTPPort"))
            SMTP.Send(Mail)
            ' Return 1

        Catch aex As ArgumentException
            Log.Warn(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Missing or invalid parameters", aex)
            'Throw New HttpResponseException(GenericErrorResponse("Request failed", HttpStatusCode.BadRequest, 440))

        Catch hex As HttpResponseException
            Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Unexpected error occured", hex)
            'Throw hex

        Catch ex As Exception
            Log.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Unexpected error occured", ex)
            'Throw New HttpResponseException(GenericErrorResponse("Request failed", HttpStatusCode.InternalServerError, 500))
        End Try

        ' Return 0
    End Sub

    ''' <summary>
    ''' Create Sites for sharing details
    ''' </summary>
    ''' <param name="_sharingdata"></param>
    ''' <param name="_code"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateChildrenSharing(_sharingdata As List(Of Models.GenericTree),
                                    _code As Integer) As List(Of Models.TreeNode)

        Dim children As New List(Of Models.TreeNode)

        Dim kids = _sharingdata.Where(Function(obj) obj.ParentCode = _code And obj.Type = 2).OrderBy(Function(obj) obj.Name).ToList() ''P. Adaoag - removed (code < 0)  ' RBAJ-2014.04.23 Fixed infinite loop "obj.Code <> _code"

        For Each k In kids
            Dim child As New Models.TreeNode
            child.title = k.Name
            child.key = k.Code
            child.icon = False
            child.children = Nothing
            children.Add(child)
            child.select = k.Flagged
            child.selected = k.Flagged
            child.parenttitle = k.ParentName
            child.groupLevel = GroupLevel.Site     'MKAM 2014.11.11
            child.note = k.Global
        Next

        Return children

    End Function
    Public Function ConvertToDataTable(Of T)(list As IList(Of T)) As DataTable
        Dim entityType As Type = GetType(T)
        Dim table As New DataTable()
        Dim properties As PropertyDescriptorCollection = TypeDescriptor.GetProperties(entityType)
        For Each prop As PropertyDescriptor In properties
            table.Columns.Add(prop.Name, If(Nullable.GetUnderlyingType(prop.PropertyType), prop.PropertyType))
        Next
        For Each item As T In list
            Dim row As DataRow = table.NewRow()
            For Each prop As PropertyDescriptor In properties
                row(prop.Name) = If(prop.GetValue(item), DBNull.Value)
            Next
            table.Rows.Add(row)
        Next
        Return table
    End Function

    Public Function GetUserConnectionString(CodeUser As Integer) As String
        Try
            Dim DecMainDBConnectionString As String = Decrypt(MainDBConnectionString)
            Dim ds As New DataSet()

            Using cmd As New SqlClient.SqlCommand()
                Using cn As New SqlClient.SqlConnection(DecMainDBConnectionString)
                    cmd.Connection = cn
                    cmd.CommandText = "[dbo].[Kiosk_API_Get_UserByUserCode]"
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Clear()
                    cmd.Parameters.Add("@UserCode", SqlDbType.Int).Value = CodeUser
                    cn.Open()
                    Using da As New SqlClient.SqlDataAdapter(cmd)
                        da.Fill(ds)
                    End Using
                End Using
            End Using

            If ds.Tables.Count < 2 OrElse ds.Tables(1).Rows.Count = 0 Then
                Dim ex As New InvalidOperationException("No connection data returned for the specified user.")
                Log.Error(ex.Message, ex)
                Throw ex
            End If

            Dim r As DataRow = ds.Tables(1).Rows(0)

            'Encrypted values from the second table; column names are fixed as requested
            Dim dataSourceEnc As String = GetStr(r("DataSource"))
            Dim initialCatalogEnc As String = GetStr(r("InitialCatalog"))
            Dim userIdEnc As String = GetStr(r("User_ID"))
            Dim passwordEnc As String = GetStr(r("Password"))

            ' Decrypt via EgsData.modFunctions.Decrypt
            Dim dataSource As String = Decrypt(dataSourceEnc)
            Dim initialCatalog As String = Decrypt(initialCatalogEnc)
            Dim userId As String = Decrypt(userIdEnc)
            Dim password As String = Decrypt(passwordEnc)

            Dim builder As New SqlConnectionStringBuilder() With {
            .dataSource = dataSource,
            .initialCatalog = initialCatalog,
            .userId = userId,
            .password = password,
            .IntegratedSecurity = False
        }

            Dim connString As String = builder.ConnectionString

            ' Optional: sanitize sensitive parts before logging
            Dim safeConnString As String = Regex.Replace(connString, "Password=[^;]*", "Password=****", RegexOptions.IgnoreCase)

            Log.Info($"Generated connection string: {safeConnString} for CodeUser: {CodeUser}")

            If DebugEnabled Then
                connString = DebugConnection
                Log.Info($"Debug is enabled using: {connString} for CodeUser: {CodeUser}")
            End If

            Return connString

        Catch ex As Exception
            Dim contextMessage As String = "GetUserConnectionString: Unexpected error occurred"
            Log.Error(contextMessage, ex)
            Throw
        End Try
    End Function

End Module

Public Class DatabaseException
    Inherits System.Exception
    Implements ISerializable

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(ByVal message As String, ByVal inner As Exception)
        MyBase.New(message, inner)
    End Sub

    ' This constructor is needed for serialization.
    Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
        MyBase.New(info, context)
    End Sub
End Class
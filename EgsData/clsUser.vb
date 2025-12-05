Imports System.Data.SqlClient
Imports System.Data
Imports System
Imports System.Security.Cryptography

#Region "Class Header"
'Name               : clsUser
'Decription         : Manages User Table
'Date Created       : 13.09.2005
'Author             : VBV
'Revision History   : 
'                       Accepts a connection string as opposed to a connection object. Class performs on a disconnected state.
'
#End Region

''' <summary>
''' Manages User Table
''' </summary>
''' <remarks></remarks>

Public Class clsUser
    Inherits clsDBRoutine

    'Private L_Cnn As SqlConnection
    'Private cmd As New SqlCommand
    Private L_ErrCode As enumEgswErrorCode
    Private L_lngCodeUser As Int32 = -1
    Private L_lngCodeSite As Int32 = -1

    'Properties
    Private L_AppType As enumAppType
    Private L_dtList As DataTable
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType
    Private L_lngCode As Int32

    'AGL 2014.09.17
    Private Shared PASSWORD_CHARS_LCASE As String = "abcdefghijklmnopqrstuvwxyz"
    Private Shared PASSWORD_CHARS_UCASE As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
    Private Shared PASSWORD_CHARS_NUMERIC As String = "1234567890"
    Private Shared PASSWORD_CHARS_SPECIAL As String = "*$-+?_&=!%{}/"

#Region "Class Functions and Properties"
    Public Sub New(ByVal eAppType As enumAppType, ByVal strCnn As String, _
        Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)

        Try

            L_AppType = eAppType
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn
        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Public ReadOnly Property AppType() As enumAppType
        Get
            AppType = L_AppType
        End Get
    End Property

    Public ReadOnly Property ItemsNotDeleted() As DataTable
        Get
            ItemsNotDeleted = L_dtList
        End Get
    End Property

    Public ReadOnly Property ConnectionString() As String
        Get
            ConnectionString = L_strCnn
        End Get
    End Property

    Public Property FetchReturnType() As enumEgswFetchType
        Get
            FetchReturnType = L_bytFetchType
        End Get
        Set(ByVal value As enumEgswFetchType)
            L_bytFetchType = value
        End Set
    End Property

    Public Property Code() As Int32
        Get
            Code = L_lngCode
        End Get
        Set(ByVal value As Int32)
            L_lngCode = value
        End Set
    End Property
#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Get list of approvers with their email information and language for email sending purpose
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListApproversOnly() As Object
        Dim strCommandText As String = "sp_EgswUserGetListApprovers"
        Try
            Select Case L_bytFetchType
                Case enumEgswFetchType.DataReader
                    Return ExecuteReader(L_strCnn, CommandType.StoredProcedure, strCommandText)
                Case enumEgswFetchType.DataSet
                    Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText)
                Case enumEgswFetchType.DataTable
                    Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText).Tables(0)
            End Select

            Return Nothing
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetApprovers() As Object
        Dim strCommandText As String = "GET_APPROVERS"
        Try
            Select Case L_bytFetchType
                Case enumEgswFetchType.DataReader
                    Return ExecuteReader(L_strCnn, CommandType.StoredProcedure, strCommandText)
                Case enumEgswFetchType.DataSet
                    Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText)
                Case enumEgswFetchType.DataTable
                    Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText).Tables(0)
            End Select

            Return Nothing
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''' <summary>
    ''' Get user profile based on given username and password or CodeUser
    ''' </summary>
    ''' <param name="strUsername"></param>
    ''' <param name="strPassword"></param>
    ''' <param name="strUserSession"></param>
    ''' <param name="intCodeUser"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FetchUserProfile(ByVal strUsername As String, ByVal strPassword As String, ByVal strUserSession As String, ByRef intCodeUser As Integer) As DataSet
        Dim strCommandText As String = "sp_egswAuthenticateUser"
        Dim arrParam(3) As SqlParameter
        'strPassword = ConvertTextToHash(strPassword)
        If fctIsPassEncrypted(strUsername) Then 'VRP 15.05.2008
            strPassword = Encrypt(strPassword)
        Else
            strPassword = ConvertTextToHash(strPassword)
        End If '--- 

        arrParam(0) = New SqlParameter("@nvcUsername", strUsername)
        arrParam(1) = New SqlParameter("@nvcPassword", strPassword)
        arrParam(2) = New SqlParameter("@vchUserSession", strUserSession)
        arrParam(3) = New SqlParameter("@intCodeUser", intCodeUser)
        'arrParam(4) = New SqlParameter("@blnNewSession", 0)
        'arrParam(4).Direction = ParameterDirection.Output

        'arrParam(3).Direction = ParameterDirection.Output
        Try
            Dim ds = ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
            Return ds
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
    End Function
    ''' <summary>
    ''' Save User information to EgswUser Table
    ''' </summary>
    ''' <param name="intCodeUser">PK</param>
    ''' <param name="strFullname">Full name</param>
    ''' <param name="strUsername">Unique Usernmae</param>
    ''' <param name="strPassword">Password</param>
    ''' <param name="bytStatus"></param>
    ''' <param name="strEmail"></param>
    ''' <param name="strSMTPUID"></param>
    ''' <param name="strSMTPPWD"></param>
    ''' <param name="IsApprover"></param>
    ''' <param name="IsNotify"></param>
    ''' <param name="IsApproverOnly"></param>
    ''' <param name="IsFromRN"></param>
    ''' <param name="strName"></param>
    ''' <param name="strCompany"></param>
    ''' <param name="strAddress"></param>
    ''' <param name="strCity"></param>
    ''' <param name="strZip"></param>
    ''' <param name="strState"></param>
    ''' <param name="intCountryCode"></param>
    ''' <param name="intCodeSourceGallery"></param>
    ''' <param name="intEgsID"></param>
    ''' <param name="intCodeSite"></param>
    ''' <param name="strRolesCodeList"></param>
    ''' <param name="TranMode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function SaveToList(ByRef intCodeUser As Integer, ByVal strFullname As String, _
    ByVal strUsername As String, _
    ByVal strPassword As String, _
    ByVal bytStatus As Byte, _
    ByVal strEmail As String, _
    ByVal strSMTPUID As String, _
    ByVal strSMTPPWD As String, _
    ByVal IsApprover As Boolean, _
    ByVal IsNotify As Boolean, _
    ByVal IsApproverOnly As Boolean, _
    ByVal IsFromRN As Boolean, _
    ByVal strName As String, _
    ByVal strCompany As String, _
    ByVal strAddress As String, _
    ByVal strCity As String, _
    ByVal strZip As String, _
    ByVal strState As String, _
    ByVal intCountryCode As Integer, _
    ByVal intCodeSourceGallery As Integer, _
    ByVal intEgsID As Integer, _
    ByVal intCodeSite As Integer, _
    ByVal strRolesCodeList As String, _
    ByVal intCodeRestaurant As Integer, _
    ByVal TranMode As enumEgswTransactionMode, _
          Optional ByVal intActiveDirectoryType As Integer = 0, _
          Optional blnLoginLocked As Boolean = False, _
          Optional intSecurityQuestionID As Integer = -1, _
          Optional strCustomSecurityQuestion As String = "", _
          Optional strSecurityAnswer As String = "", _
          Optional blnAutoReports As Boolean = False, Optional intCodeSaleSite As Integer = 0,
          Optional blnUseProductLinking As Boolean = False, Optional blnUseKioskforWindows As Boolean = False) As enumEgswErrorCode


        Try
            'strPassword = ConvertTextToHash(strPassword) 'VRP 15.05.2008 Comment
            strPassword = Encrypt(strPassword) 'VRP 15.05.2008

            Dim arrParam(34) As SqlParameter
            arrParam(0) = New SqlParameter("@retVal", "")
            arrParam(0).Direction = ParameterDirection.ReturnValue
            arrParam(1) = New SqlParameter("@tntTranMode", TranMode)
            arrParam(2) = New SqlParameter("@intCode", intCodeUser)
            arrParam(2).Direction = ParameterDirection.InputOutput
            arrParam(3) = New SqlParameter("@nvcFullName", strFullname)
            arrParam(4) = New SqlParameter("@nvcUsername", strUsername)
            arrParam(5) = New SqlParameter("@nvcPassword", strPassword)
            arrParam(6) = New SqlParameter("@tntStatus", bytStatus)
            arrParam(7) = New SqlParameter("@nvcEmail", strEmail)
            arrParam(8) = New SqlParameter("@nvcSMTPUID", strSMTPUID)
            arrParam(9) = New SqlParameter("@nvcSMTPPWD", strSMTPPWD)
            arrParam(10) = New SqlParameter("@IsApprover", IsApprover)
            arrParam(11) = New SqlParameter("@IsNotify", IsNotify)
            arrParam(12) = New SqlParameter("@IsApproverOnly", IsApproverOnly)
            arrParam(13) = New SqlParameter("@IsFromRN", IsFromRN)
            arrParam(14) = New SqlParameter("@nvcName", strName)
            arrParam(15) = New SqlParameter("@nvcCompany", strCompany)
            arrParam(16) = New SqlParameter("@nvcAddress", strAddress)
            arrParam(17) = New SqlParameter("@nvcCity", strCity)
            arrParam(18) = New SqlParameter("@nvcZip", strZip)
            arrParam(19) = New SqlParameter("@nvcState", strState)
            arrParam(20) = New SqlParameter("@sntCountryCode", intCountryCode)
            arrParam(21) = New SqlParameter("@intCodeSourceGallery", intCodeSourceGallery)
            arrParam(22) = New SqlParameter("@intEGSID", intEgsID)
            arrParam(23) = New SqlParameter("@intCodeSite", intCodeSite)
            arrParam(24) = New SqlParameter("@vcRolesCodeList", strRolesCodeList)
            arrParam(25) = New SqlParameter("@nActiveDirectoryType", intActiveDirectoryType)
            arrParam(26) = New SqlParameter("@bitLoginLocked", blnLoginLocked) 'AGL 2014.09.15
            arrParam(27) = New SqlParameter("@intSecurityQuestionID", intSecurityQuestionID) 'AGL 2014.09.16
            arrParam(28) = New SqlParameter("@strCustomSecurityQuestion", strCustomSecurityQuestion) 'AGL 2014.09.16
            arrParam(29) = New SqlParameter("@strSecurityAnswer", strSecurityAnswer) 'AGL 2014.09.16
            arrParam(30) = New SqlParameter("@intCodeRestaurant", intCodeRestaurant) 'AGL 2015.04.04
            arrParam(31) = New SqlParameter("@intCodeSalesSite", intCodeSaleSite) 'AMTLA 2016.03.08
            arrParam(32) = New SqlParameter("@IsAutoReport", blnAutoReports) 'AMTLA 2016.03.08
            arrParam(33) = New SqlParameter("@UseProductLinking", blnUseProductLinking) 'JBL 2016.08.19
            arrParam(34) = New SqlParameter("@UseKioskWindows", blnUseKioskforWindows) 'JBL 2017.05.10

            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswUserUpdate", arrParam)
            intCodeUser = CInt(arrParam(2).Value)

            Return CType(arrParam(0).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try

    End Function
    ''' <summary>
    ''' Delete User from the list
    ''' </summary>
    ''' <param name="intCode"></param>
    ''' <param name="strCodeList"></param>
    ''' <param name="TrandMode"></param>
    ''' <returns></returns>
    ''' <remarks>multiple delete is not yet working!</remarks>
    Private Function DeleteFromList(ByVal intCode As Integer, ByVal strCodeList As String, ByVal TrandMode As enumEgswTransactionMode) As enumEgswErrorCode

        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@retval", "")
        arrParam(0).Direction = ParameterDirection.ReturnValue
        arrParam(1) = New SqlParameter("@intCode", intCode)
        arrParam(2) = New SqlParameter("@tntTranMode", TrandMode)
        arrParam(3) = New SqlParameter("@txtCodeList", strCodeList)

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswUserDelete", arrParam)
            Return CType(arrParam(0).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try
    End Function
#End Region

#Region "Get Methods"
    ''' <summary>
    ''' Get List of all users
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList() As Object
        Return Me.FetchList(-1, -1, -1, 0)
    End Function


    ''' <summary>
    ''' Get User with Roles Selected and Not Selected
    ''' </summary>
    ''' <param name="intCodeUser"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList(ByVal intCodeUser As Integer, ByVal intCodeTrans As Integer) As DataSet
        Dim lastFetchType As enumEgswFetchType = Me.FetchReturnType
        Me.FetchReturnType = enumEgswFetchType.DataSet
        Dim ds As DataSet = CType(Me.FetchList(intCodeUser, -1, -1, intCodeTrans), DataSet)
        Me.FetchReturnType = lastFetchType
        Return ds
    End Function

    ''' <summary>
    ''' Get User's profile, roles and accessible rights
    ''' </summary>
    ''' <param name="strUsername"></param>
    ''' <param name="strPassword"></param>
    ''' <param name="strUserSession"></param>
    ''' <param name="intCodeUser"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Login(ByVal strUsername As String, ByVal strPassword As String, ByVal strUserSession As String, ByVal intCodeUser As Integer, Optional ByVal bSavelogs As Boolean = False) As structUser
        Dim cUser As New clsUser(enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataReader)
        Dim ds As DataSet = Me.FetchUserProfile(strUsername, strPassword, strUserSession, intCodeUser)
        Dim dtProfile As DataTable = ds.Tables(0)
        Dim dtRights As DataTable = ds.Tables(1)
        Dim dtSites As DataTable = ds.Tables(2)
        Dim dtRequireApproval As DataTable = ds.Tables(3)
        Dim dtRoleLevels As DataTable = ds.Tables(4)
        Dim dtGroupLevel As DataTable = ds.Tables(5)
        Dim row As DataRow
        Dim sProfile As New structUser
        Dim cConfig As New clsConfig(enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataReader)



        sProfile.LoginStatusCode = 0

        If dtProfile.Rows.Count = 0 Then
            Return Nothing
        End If




        If bSavelogs Then
            SaveLogs(CIntDB(dtProfile.Rows(0)("code")), CDateDB(dtProfile.Rows(0)("datelastaccess")))
        End If





        Try
            With sProfile
                row = dtProfile.Rows(0)
                .Code = CInt(row("code"))
                .UseBestUnit = CBool(cConfig.GetConfig(.Code, clsConfig.enumNumeros.UIUseBestUnit, clsConfig.CodeGroup.user, "true"))
                .DateCreated = CDateDB(row.Item("DateCreated"))
                .DateLastAccessed = CDate(row("DateLastAccess"))
                If IsDBNull(row("datemodified")) Then .DateModified = Now Else .DateModified = CDate(row("datemodified"))
                .eDisplayMode = CType(cConfig.GetConfig(.Code, clsConfig.enumNumeros.UIListeDisplay, clsConfig.CodeGroup.user, CStr(enumListeDisplayMode.List)), enumListeDisplayMode)
                .CodeLang = CInt(cConfig.GetConfig(.Code, clsConfig.enumNumeros.UICodeLang, clsConfig.CodeGroup.user, "1"))
                .Site.Group = CInt(row("codeProperty"))
                .Site.Code = CInt(row("codeSite"))
                .Site.Name = CStr(row("SiteName"))
                .Site.SiteLevel = CType(row("SiteLevel"), enumGroupLevel)

                'AGL 2014.09.12 - LoginStatusCode
                .LoginStatusCode = CInt(row("LoginStatusCode"))
                .CulturePref = GetStr(row("CulturePref")) 'MKAM 2017.03.07

                .CodeTimeZone = CInt(cConfig.GetConfig(.Code, clsConfig.enumNumeros.UICodeTimezone, clsConfig.CodeGroup.user, "-1"))
                .CodeTrans = CInt(cConfig.GetConfig(.Code, clsConfig.enumNumeros.UICodeMainListeLang, clsConfig.CodeGroup.user, "1"))
                .LastSetPrice = CInt(cConfig.GetConfig(.Code, clsConfig.enumNumeros.UIMainSetOfPrice, clsConfig.CodeGroup.user, "0"))
                .LastSetPriceSales = CInt(cConfig.GetConfig(.Site.Code, clsConfig.enumNumeros.DefaultSetPriceSales, clsConfig.CodeGroup.site, "0"))
                .NutrientDBCode = CInt(cConfig.GetConfig(clsConfig.CONST_CODEUSER_GLOBAL, clsConfig.enumNumeros.NutrientDatabase, clsConfig.CodeGroup.global, "1"))
                .PageSize = CInt(cConfig.GetConfig(.Code, clsConfig.enumNumeros.UIPageSize, clsConfig.CodeGroup.user, "15"))
                .WebHomePageBrowseListTableForMerchandise = CStr(cConfig.GetConfig(.Code, clsConfig.enumNumeros.UIHomePageBrowseListTableForMerchandise, clsConfig.CodeGroup.user, "EgswCategory"))
                .WebHomePageBrowseListTableForRecipe = CStr(cConfig.GetConfig(.Code, clsConfig.enumNumeros.UIHomePageBrowseListTableForRecipe, clsConfig.CodeGroup.user, "EgswCategory"))
                .WebHomePageBrowseListTableForMenu = CStr(cConfig.GetConfig(.Code, clsConfig.enumNumeros.UIHomePageBrowseListTableForMenu, clsConfig.CodeGroup.user, "EgswCategory"))
                .Email = CStr(row("email"))
                .Fullname = CStr(row("fullname"))
                '     .Password = CStr(row("password"))
                .Username = strUsername
                .EGSID = CIntDB(row("EGSID"))
                .FullText = CBool(cConfig.GetConfig(.Code, clsConfig.enumNumeros.FTSEnable, clsConfig.CodeGroup.user, "TRUE"))
                .RemoveTrailingZeroes = CBool(cConfig.GetConfig(.Code, clsConfig.enumNumeros.UIRemoveTrailingZeros, clsConfig.CodeGroup.user, "FALSE"))
                .PrintOutput = cConfig.GetConfig(.Code, clsConfig.enumNumeros.UIPrintOutput, clsConfig.CodeGroup.user, "0_1_2_3")
                .AutoConversion = CBool(cConfig.GetConfig(.Code, clsConfig.enumNumeros.UIAutomate, clsConfig.CodeGroup.user, "FALSE")) 'JTOC 02.05.2014

                Dim strColor As String = cConfig.GetConfig(.Code, clsConfig.enumNumeros.UIListItemColor, clsConfig.CodeGroup.user, "White_WhiteSmoke_LightCyan")
                Dim arrColor() As String = strColor.Split(CChar("_"))
                If arrColor.Length > 0 Then
                    .ListItemColor = arrColor(0)
                Else
                    .ListItemColor = "White"
                End If
                If arrColor.Length > 1 Then
                    .ListAlternatingItemColor = arrColor(1)
                Else
                    .ListAlternatingItemColor = "WhiteSmoke"
                End If
                If arrColor.Length > 2 Then
                    .UnsavedItemColor = arrColor(2)
                Else
                    .UnsavedItemColor = "LightCyan"
                End If

                Dim arrRights As New ArrayList
                Dim arrRoles As New ArrayList
                Dim arrRolesNames As New ArrayList
                Dim arrRoleLevel As New ArrayList

                For Each row In dtRoleLevels.Rows
                    If arrRoleLevel.Contains(row("rolelevel").ToString) = False Then
                        arrRoleLevel.Add(row("rolelevel").ToString)
                    End If
                Next

                If arrRoleLevel.Contains(CStr(enumGroupLevel.Global)) Then
                    .RoleLevelHighest = enumGroupLevel.Global
                ElseIf arrRoleLevel.Contains(CStr(enumGroupLevel.Property)) Then
                    .RoleLevelHighest = enumGroupLevel.Property
                ElseIf arrRoleLevel.Contains(CStr(enumGroupLevel.User)) Then
                    .RoleLevelHighest = enumGroupLevel.User
                Else
                    .RoleLevelHighest = enumGroupLevel.Site

                End If

                Dim dtRoles As DataTable = GetUserRoles(.Code)
                For Each row In dtRights.Rows
                    If arrRoles.Contains(row("role").ToString) = False Then
                        arrRoles.Add(row("role").ToString)
                        arrRolesNames.Add(row("name").ToString)
                    End If
                Next


                '                For Each row In dtRights.Rows
                '                    If arrRoles.Contains(row("role").ToString) = False Then
                '                        arrRoles.Add(row("role").ToString)
                '                        arrRolesNames.Add(row("name").ToString)
                '                    End If


                '                    ' If site level user, do not allow to manage allergens if at property is enabled
                '                    If .RoleLevelHighest = enumGroupLevel.Site Then
                '                        If CType(row("modules"), MenuType) = MenuType.ManageAllergen Then
                '                            If CType(dtGroupLevel.Rows(0).Item("grouplevel"), enumGroupLevel) = enumGroupLevel.Property Then
                '                                If CType(row("rights"), UserRightsFunction) <> UserRightsFunction.AllowUse Then
                '                                    GoTo NextRight
                '                                End If
                '                            End If
                '                        End If
                '                    End If

                '                    arrRights.Add(row("modules").ToString & "_" & row("rights").ToString)
                'NextRight:
                '                Next

                .arrRoles = arrRoles
                '.arrRoleRights = arrRights
                .arrRolesNames = arrRolesNames


                Dim arrSites As New ArrayList
                For Each row In dtSites.Rows
                    arrSites.Add(row("code").ToString)
                Next
                .arrSitesAccessible = arrSites

                Dim arrApprove As New ArrayList
                For Each row In dtRequireApproval.Rows
                    arrApprove.Add(row("listetype").ToString)
                Next
                .arrListeTypeApprovalRequired = arrApprove

                '-- JBB 12.21.2011
                Dim dtUserRole As DataTable = CType(cUser.GetUserRoles(.Code), DataTable)
                Dim arrUserListe As New ArrayList

                For Each drRole As DataRow In dtUserRole.Rows
                    If Not arrUserListe.Contains(drRole("Role").ToString()) Then
                        arrUserListe.Add(drRole("Role").ToString())
                    End If
                Next
                If arrUserListe.Count = 1 Then
                    'If arrUserListe.Contains(CStr(enumUserRights.Visitor)) Then
                    .RoleRights = CType(arrUserListe(0), enumUserRights)
                    'End If
                Else
                    If arrUserListe.Contains(CStr(enumUserRights.Admin)) Then
                        .RoleRights = enumUserRights.Admin
                    ElseIf arrUserListe.Contains(CStr(enumUserRights.Approver)) Then
                        .RoleRights = enumUserRights.Approver
                    ElseIf arrUserListe.Contains(CStr(enumUserRights.Editor)) Then
                        .RoleRights = enumUserRights.Editor
                    ElseIf arrUserListe.Contains(CStr(enumUserRights.Visitor)) Then
                        .RoleRights = enumUserRights.Visitor
                    End If
                End If
                '--
            End With

            If sProfile.LastSetPrice = 0 Then   ' change
                Dim cSetprice As New clsSetPrice(sProfile, enumAppType.WebApp, L_strCnn)
                Dim dr As SqlDataReader = CType(cSetprice.GetList(1, sProfile.Site.Code, SetPriceType.Purchase), SqlDataReader)
                If dr.HasRows Then
                    dr.Read()
                    sProfile.LastSetPrice = CInt(dr("code"))
                    cConfig.UpdateConfig(sProfile.Code, clsConfig.CodeGroup.user, clsConfig.enumNumeros.UIMainSetOfPrice, sProfile.LastSetPrice.ToString)
                End If
                dr.Close()
            End If

            If sProfile.LastSetPriceSales = 0 Then   ' change
                Dim cSetprice As New clsSetPrice(sProfile, enumAppType.WebApp, L_strCnn)
                Dim dr As SqlDataReader = CType(cSetprice.GetList(1, sProfile.Site.Code, SetPriceType.Sale), SqlDataReader)
                If dr.HasRows Then
                    dr.Read()
                    sProfile.LastSetPriceSales = CInt(dr("code"))
                End If
                dr.Close()
            End If

            Return sProfile

        Catch ex As Exception
            Throw New Exception("Login Failed : " & ex.Message)
        End Try
    End Function

    Public Function GetListByEGSID(ByVal intEGSID As Integer) As DataTable
        Dim strSQL As String = "SELECT u.Code, u.codeSite, p.code AS CodeProperty FROM egswUser u INNER JOIN egswSite s ON u.Codesite= s.Code LEFT JOIN egswProperty p ON s.[group]=p.Code WHERE u.EGSID=" & intEGSID
        Return ExecuteDataset(L_strCnn, CommandType.Text, strSQL).Tables(0)
    End Function

    Public Function GetCodeUserBySessionID(ByVal strSessionID As String) As Integer
        Dim intX As Integer = 0
        Dim strSQL As String = "SELECT Code FROM egswUser WHERE UserSession=@Session"
        ''Dim arrParam() As SqlParameter = {New SqlParameter("@Session", strSessionID)}
        ''Dim dr As SqlDataReader = ExecuteReader(L_strCnn, CommandType.Text, strSQL, arrParam)
        ''If dr.Read Then 'DLS Jan272009
        ''    intX = CInt(dr("Code"))
        ''End If
        ''dr.Close()
        ''Return intX
        Dim cmd As New SqlCommand
        Dim cn As New SqlConnection(L_strCnn)
        With cmd
            .Connection = cn
            .CommandType = CommandType.Text
            .Parameters.Add("@Session", SqlDbType.NVarChar).Value = strSessionID
            .CommandText = strSQL
            .Connection.Open()
            intX = fctNullToZero(.ExecuteScalar)
            .Connection.Close()
            .Dispose()
        End With
        Return intX
    End Function

    Public Function GetCodeUserByEGSID(ByVal intCodeEGS As Integer) As Integer
        Dim intX As Integer = 0
        Dim strSQL As String = "SELECT Code FROM egswUser WHERE EGSID=@CodeEGS"
        ''Dim arrParam() As SqlParameter = {New SqlParameter("@Session", strSessionID)}
        ''Dim dr As SqlDataReader = ExecuteReader(L_strCnn, CommandType.Text, strSQL, arrParam)
        ''If dr.Read Then 'DLS Jan272009
        ''    intX = CInt(dr("Code"))
        ''End If
        ''dr.Close()
        ''Return intX
        Dim cmd As New SqlCommand
        Dim cn As New SqlConnection(L_strCnn)
        With cmd
            .Connection = cn
            .CommandType = CommandType.Text
            .Parameters.Add("@CodeEGS", SqlDbType.Int).Value = intCodeEGS
            .CommandText = strSQL
            .Connection.Open()
            intX = fctNullToZero(.ExecuteScalar)
            .Connection.Close()
            .Dispose()
        End With
        Return intX
    End Function

    Public Function GetCodeUserByUsername(ByVal strUsername As String) As Integer
        Dim intX As Integer = 0
        Dim strSQL As String = "SELECT Code FROM egswUser WHERE username=@strUserName"
        ''Dim arrParam() As SqlParameter = {New SqlParameter("@Session", strSessionID)}
        ''Dim dr As SqlDataReader = ExecuteReader(L_strCnn, CommandType.Text, strSQL, arrParam)
        ''If dr.Read Then 'DLS Jan272009
        ''    intX = CInt(dr("Code"))
        ''End If
        ''dr.Close()
        ''Return intX
        Dim cmd As New SqlCommand
        Dim cn As New SqlConnection(L_strCnn)
        With cmd
            .Connection = cn
            .CommandType = CommandType.Text
            .Parameters.Add("@strUserName", SqlDbType.NVarChar).Value = strUsername
            .CommandText = strSQL
            .Connection.Open()
            intX = fctNullToZero(.ExecuteScalar)
            .Connection.Close()
            .Dispose()
        End With
        Return intX
    End Function

    Public Function GetUserRoles(ByVal intCodeUser As Integer) As DataTable
        Dim strSQL As String = "SELECT * FROM EgswUserRoles ur INNER JOIN EgswRoles r ON ur.role=r.code WHERE ur.CodeUser=" & intCodeUser
        Return ExecuteDataset(L_strCnn, CommandType.Text, strSQL).Tables(0)
    End Function

    Public Function GetTimeZone(Optional ByVal intCode As Integer = -1) As Object
        Dim strCommandText As String = "sp_EgswTimeZoneGetList"
        Dim arrParam() As SqlParameter = {New SqlParameter("@code", intCode)}

        Try
            Select Case L_bytFetchType
                Case enumEgswFetchType.DataReader
                    Return ExecuteReader(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
                Case enumEgswFetchType.DataSet
                    Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
                Case enumEgswFetchType.DataTable
                    Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam).Tables(0)
            End Select
            Return Nothing
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Public Function GetTimeZoneCode(ByVal dblDiff As Double) As Integer
        Dim blnAdd As Boolean = False
        If dblDiff < 0 Then blnAdd = True

        dblDiff = Math.Abs(dblDiff)
        Dim intHours As Integer = CInt(dblDiff)
        Dim intMinutes As Integer = CInt((dblDiff - intHours) * 60)

        Dim strSQL As String = "SELECT Code FROM egswTimeZone WHERE HourDiff=@HourDiff AND MinDiff=@MinDiff AND blnAdd=@blnAdd"
        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@HourDiff", intHours)
        arrParam(1) = New SqlParameter("@MinDiff", intMinutes)
        arrParam(2) = New SqlParameter("@blnAdd", blnAdd)

        Dim dr As SqlDataReader = ExecuteReader(L_strCnn, CommandType.Text, strSQL, arrParam)
        If dr.Read Then Return CInt(dr("Code"))
        dr.Close() 'AGL 2013.06.18
        Return 0
    End Function

    ''' <summary>
    ''' Get List of users belonging to a property
    ''' </summary>
    ''' <param name="intCodeProperty"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListByProperty(ByVal intCodeProperty As Integer) As Object
        Return Me.FetchList(-1, -1, intCodeProperty, 0)
    End Function

    ''' <summary>
    ''' Get list of users belonging to a site
    ''' </summary>
    ''' <param name="intCodeSite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListBySite(ByVal intCodeSite As Integer, Optional intCodeProperty As Integer = -1) As Object
        Return Me.FetchList(-1, intCodeSite, intCodeProperty, 0)
    End Function

    'JTOC 02.17.2014 created for active/inactive filter    
    'MKAM 2014.10.24 added property filter
    Public Function GetListBySite(ByVal intCodeSite As Integer, ByVal blnActive As Boolean, Optional intCodeProperty As Integer = -1) As Object
        Return Me.FetchList(-1, intCodeSite, intCodeProperty, 0, blnActive)
    End Function

    ''' <summary>
    ''' Fetch list of rights and roles of a user
    ''' </summary>
    ''' <param name="intCodeUser"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUserRolesAndRights(ByVal intCodeUser As Integer) As Object
        Dim strCommandText As String = "sp_EgswUserGetListRolesRights"

        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeUser", intCodeUser)
        Try

            Select Case L_bytFetchType
                Case enumEgswFetchType.DataReader
                    Return ExecuteReader(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
                Case enumEgswFetchType.DataSet
                    Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
                Case enumEgswFetchType.DataTable
                    Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam).Tables(0)
            End Select

            Return Nothing
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Function

    Public Function GetUserFullNameEmail(ByVal intCodeSite As Integer) As DataTable
        Dim strCommandText As String = "GET_RecipientsNameEmail"

        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeSite", intCodeSite)
        Try

            'Select Case L_bytFetchType
            '    Case enumEgswFetchType.DataReader
            '        Return ExecuteReader(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
            '    Case enumEgswFetchType.DataSet
            '        Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
            'Case enumEgswFetchType.DataTable
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam).Tables(0)
            'End Select

            'Return Nothing
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    ''' <summary>
    ''' Fetch list of Users
    ''' </summary>
    ''' <param name="intCodeUser"></param>
    ''' <param name="intCodeSite"></param>
    ''' <param name="intCodeProperty"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function FetchList(ByVal intCodeUser As Integer, ByVal intCodeSite As Integer, ByVal intCodeProperty As Integer, ByVal intCodeTrans As Integer) As Object
        Dim strCommandText As String = "sp_EgswUserGetList"

        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeUser", intCodeUser)
        arrParam(1) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(2) = New SqlParameter("@intCodeProperty", intCodeProperty)
        arrParam(3) = New SqlParameter("@intCodeTrans", intCodeTrans)

        Try
            Select Case L_bytFetchType
                Case enumEgswFetchType.DataReader
                    Return ExecuteReader(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
                Case enumEgswFetchType.DataSet
                    Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
                Case enumEgswFetchType.DataTable
                    Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam).Tables(0)
            End Select

            Return Nothing
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Function FetchList(ByVal intCodeUser As Integer, ByVal intCodeSite As Integer, ByVal intCodeProperty As Integer, ByVal intCodeTrans As Integer, ByVal blnActive As Boolean) As Object
        Dim strCommandText As String = "sp_EgswUserGetList"

        Dim arrParam(4) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeUser", intCodeUser)
        arrParam(1) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(2) = New SqlParameter("@intCodeProperty", intCodeProperty)
        arrParam(3) = New SqlParameter("@intCodeTrans", intCodeTrans)
        arrParam(4) = New SqlParameter("@bitStatus", blnActive)

        Try
            Select Case L_bytFetchType
                Case enumEgswFetchType.DataReader
                    Return ExecuteReader(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
                Case enumEgswFetchType.DataSet
                    Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
                Case enumEgswFetchType.DataTable
                    Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam).Tables(0)
            End Select

            Return Nothing
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function fctIsPassEncrypted(ByVal strUserName As String) As Boolean 'VRP 15.05.2008
        If strUserName = "" Then Return False 'DLS 27.01.2009
        Dim strSQL As String = "" 'DLS 27.01.2009
        Dim flagX As Boolean = False
        strSQL &= "SELECT ISNULL(Np,0) FROM egswUser WHERE UserName=@Username OR Email=@UserName "

        Dim cmd As New SqlCommand
        Dim cn As New SqlConnection(L_strCnn)
        With cmd
            .Connection = cn
            .CommandType = CommandType.Text
            .CommandText = strSQL
            .Parameters.Add("@Username", SqlDbType.NVarChar).Value = strUserName
            .Connection.Open()
            flagX = CBool(.ExecuteScalar)
            .Connection.Close()
            .Dispose()
        End With

        Return flagX

    End Function


    Public Function GetUserProject(ByVal intCodeProject As Integer) As Object

        Dim strSQL As String
        Dim arrParam(0) As SqlParameter

        strSQL = "SELECT CodeUser, CodeProject FROM EgswUserProject WHERE CodeProject=@CodeProject"


        arrParam(0) = New SqlParameter("@CodeProject", SqlDbType.Int)
        arrParam(0).Value = intCodeProject
        Try
            Return ExecuteDataset(L_strCnn, CommandType.Text, strSQL, arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try

    End Function

#End Region

#Region "Update Methods"
    ''' <summary>
    ''' Deactivate user 
    ''' </summary>
    ''' <param name="strUserName"></param>
    ''' <param name="TranMode"></param>
    ''' <param name="oTransaction"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateUserDeactivate(ByVal strUserName As String, ByVal TranMode As enumEgswTransactionMode, _
      Optional ByVal oTransaction As SqlTransaction = Nothing) As enumEgswErrorCode
        'Return enumEgswErrorCode.OK
        Dim cmd As New SqlCommand
        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .Transaction = oTransaction
                .CommandText = "sp_EgswUserUpdateDeactivate"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@nvcUsername", SqlDbType.NVarChar, 25).Value = strUserName
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With

        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

    End Function

    'SM 12.16.2013 put it back
    Public Function UpdateLoginCount(ByVal strUserName As String) As DataTable
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter
        Dim cmd As New SqlCommand

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "UPDATE EgswUser SET LoginCount=LoginCount + 1 WHERE username='" & strUserName & "' SELECT LoginCount FROM EgsWUser where username='" & strUserName & "'"
                .CommandType = CommandType.Text

            End With

            With da
                .SelectCommand = cmd
                .Fill(dt)
            End With

        Catch ex As Exception
            dt.Dispose()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return dt
    End Function

    Public Function ResetLoginCount(ByVal strIP As String)
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter
        Dim cmd As New SqlCommand

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "UPDATE EgswInvalidLogin SET LoginCount=0, isLocked=0 WHERE IPAddress='" & strIP & "'" '"UPDATE EgswUser SET LoginCount=0 WHERE Code=" & CodeUser
                .CommandType = CommandType.Text

            End With

            With da
                .SelectCommand = cmd
                .Fill(dt)
            End With

        Catch ex As Exception
            dt.Dispose()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return dt
    End Function

    Public Function LogUserIP(ByVal UserIP As String) As Integer
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter
        Dim cmd As New SqlCommand
        Dim intVal As Integer
        ' 1 - newly added
        ' 2 - reached maximum count
        ' 3 - retried login
        ' 0 - error
        ' 4 - IP tries to login after 1 hour
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswInvalidLogin"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@UserIP", SqlDbType.NVarChar, 25).Value = UserIP
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                intVal = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With

        Catch ex As Exception
            dt.Dispose()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return intVal
    End Function

    Public Function CheckUserIP(ByVal UserIP As String) As Integer
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter
        Dim cmd As New SqlCommand
        Dim intVal As Integer
        'intVal > 0 can proceed to log in page
        'intVal <= 0 cannot proceed to log in page
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswCheckIPLogin"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@UserIP", SqlDbType.NVarChar, 25).Value = UserIP
                .Parameters.Add("@Dateval", SqlDbType.Int)
                .Parameters("@Dateval").Direction = ParameterDirection.ReturnValue
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                intVal = .Parameters("@Dateval").Value ', enumEgswErrorCode'CType()
            End With

        Catch ex As Exception
            dt.Dispose()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return intVal
    End Function

    Public Function CheckLoginCount(ByVal strUserName As String) As DataTable
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter
        Dim cmd As New SqlCommand

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "SELECT LoginCount, Status FROM EgsWUser where username='" & strUserName & "'"
                .CommandType = CommandType.Text

            End With

            With da
                .SelectCommand = cmd
                .Fill(dt)
            End With

        Catch ex As Exception
            dt.Dispose()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return dt
    End Function

    'SM 12.16.2013 put it back END

    ''' <summary>
    ''' Update User
    ''' </summary>
    ''' <param name="intCodeUser"></param>
    ''' <param name="strFullname"></param>
    ''' <param name="strUsername"></param>
    ''' <param name="strPassword"></param>
    ''' <param name="bytStatus"></param>
    ''' <param name="strEmail"></param>
    ''' <param name="strSMTPUID"></param>
    ''' <param name="strSMTPPWD"></param>
    ''' <param name="IsApprover"></param>
    ''' <param name="IsNotify"></param>
    ''' <param name="IsApproverOnly"></param>
    ''' <param name="IsFromRN"></param>
    ''' <param name="strName"></param>
    ''' <param name="strCompany"></param>
    ''' <param name="strAddress"></param>
    ''' <param name="strCity"></param>
    ''' <param name="strZip"></param>
    ''' <param name="strState"></param>
    ''' <param name="intCountryCode"></param>
    ''' <param name="intCodeSourceGallery"></param>
    ''' <param name="intEgsID"></param>
    ''' <param name="intCodeSite"></param>
    ''' <param name="strRolesCodeList"></param>
    ''' <param name="TranMode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateUser(ByRef intCodeUser As Integer, ByVal strFullname As String, _
     ByVal strUsername As String, _
     ByVal strPassword As String, _
     ByVal bytStatus As Byte, _
     ByVal strEmail As String, _
     ByVal strSMTPUID As String, _
     ByVal strSMTPPWD As String, _
     ByVal IsApprover As Boolean, _
     ByVal IsNotify As Boolean, _
     ByVal IsApproverOnly As Boolean, _
     ByVal IsFromRN As Boolean, _
     ByVal strName As String, _
     ByVal strCompany As String, _
     ByVal strAddress As String, _
     ByVal strCity As String, _
     ByVal strZip As String, _
     ByVal strState As String, _
     ByVal intCountryCode As Integer, _
     ByVal intCodeSourceGallery As Integer, _
     ByVal intEgsID As Integer, _
     ByVal intCodeSite As Integer, _
     ByVal strRolesCodeList As String, _
     ByVal TranMode As enumEgswTransactionMode, _
     ByVal intCodeRestaurant As Integer, _
     Optional ByVal intActiveDirectoryType As Integer = 0, _
     Optional blnLoginLocked As Boolean = False, Optional blnAutoReports As Boolean = False,
     Optional intCodeSaleSite As Integer = 0, Optional blnUseProductLinking As Boolean = False,
     Optional blnUseKioskforWindows As Boolean = False) As enumEgswErrorCode

        Return Me.SaveToList(intCodeUser, strFullname, strUsername, strPassword, bytStatus, _
        strEmail, strSMTPUID, strSMTPPWD, IsApprover, IsNotify, IsApproverOnly, IsFromRN, strName, _
         strCompany, strAddress, strCity, strZip, strState, intCountryCode, intCodeSourceGallery, _
          intEgsID, intCodeSite, strRolesCodeList, intCodeRestaurant, TranMode, intActiveDirectoryType, blnLoginLocked, _
          -1, "", "", blnAutoReports, intCodeSaleSite, blnUseProductLinking, blnUseKioskforWindows)

    End Function
    ''' <summary>
    ''' Update User's password
    ''' </summary>
    ''' <param name="intCodeUser"></param>
    ''' <param name="strPassword"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateUser(ByVal intCodeUser As Integer, ByVal strPassword As String) As enumEgswErrorCode
        Return Me.SaveToList(intCodeUser, "", "", strPassword, 0, _
             "", "", "", False, False, False, False, "", _
              "", "", "", "", "", 0, 0, _
               0, 0, "", -1, enumEgswTransactionMode.ModifyPasswordOnly)
    End Function
    ''' <summary>
    ''' Update User's Status
    ''' </summary>
    ''' <param name="intCodeUser"></param>
    ''' <param name="bytStatus"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateUser(ByVal intCodeUser As Integer, ByVal bytStatus As Byte) As enumEgswErrorCode
        Return Me.SaveToList(intCodeUser, "", "", "", bytStatus, _
             "", "", "", False, False, False, False, "", _
              "", "", "", "", "", 0, 0, _
               0, 0, "", -1, enumEgswTransactionMode.UpdateStatus)
    End Function

    Public Function UpdateUser(ByVal strPassword As String, ByVal strPassCode As String) As enumEgswErrorCode
        Dim strSQL As String = "SELECT Code FROM EgswUser WHERE PassCode='" & strPassCode & "'"
        Dim cmd As New SqlCommand
        Dim cn As New SqlConnection(L_strCnn)
        Dim intCodeUser As Integer

        Try
            With cmd
                .Connection = cn
                .CommandText = strSQL
                .CommandType = CommandType.Text
                .Connection.Open()
                intCodeUser = CInt(.ExecuteScalar())
                .Connection.Close()
                cmd.Dispose()
            End With

            Return Me.SaveToList(intCodeUser, "", "", strPassword, 0, _
                 "", "", "", False, False, False, False, "", _
                  "", "", "", "", "", 0, 0, _
                   0, 0, "", -1, enumEgswTransactionMode.ModifyPasswordOnly)
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function ChangePassword(intCodeUser As Integer, ByVal strNewPassword As String) As enumEgswErrorCode
        Try
            Return Me.SaveToList(intCodeUser, "", "", strNewPassword, 0, _
                 "", "", "", False, False, False, False, "", _
                  "", "", "", "", "", 0, 0, _
                   0, 0, "", -1, enumEgswTransactionMode.ModifyPasswordOnly)
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function ResetPassword(intCodeUser As Integer) As String
        Try
            Dim strNewPassword As String
            strNewPassword = GenerateStrongPassword()
            Me.SaveToList(intCodeUser, "", "", strNewPassword, 0, _
                 "", "", "", False, False, False, False, "", _
                  "", "", "", "", "", 0, 0, _
                   0, 0, "", -1, enumEgswTransactionMode.ResetPassword)
            Return strNewPassword
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function ChangeSecurityQuestionAndAnswer(intCodeUser As Integer, intSecurityQuestionID As Integer, strSecurityAnswer As String, Optional strCustomSecurityQuestion As String = "") As enumEgswErrorCode
        Try
            Return Me.SaveToList(intCodeUser, "", "", "", 0, _
                 "", "", "", False, False, False, False, "", _
                  "", "", "", "", "", 0, 0, _
                   0, 0, "", -1, enumEgswTransactionMode.ModifySecurityQuestionAndAnswer, intSecurityQuestionID:=intSecurityQuestionID, strSecurityAnswer:=strSecurityAnswer)
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function UpdateUserPassCode(ByVal strUser_Email As String, ByVal strSessionID As String) As String
        Dim strPassCode As String = ""
        Dim strSQL As String = "IF EXISTS(SELECT * FROM EGSWUser WHERE UserName='" & strUser_Email & "')" & vbCrLf
        strSQL = strSQL & "BEGIN " & vbCrLf
        strSQL = strSQL & "	UPDATE EgswUser" & vbCrLf
        strSQL = strSQL & "	SET PassCode= cast((SELECT Code FROM EgswUser WHERE UserName='" & strUser_Email & "') as nvarchar) + '" & strSessionID & "'" & vbCrLf
        strSQL = strSQL & "	WHERE UserName='" & strUser_Email & "'" & vbCrLf
        strSQL = strSQL & "	SELECT PassCode FROM EgswUser WHERE UserName='" & strUser_Email & "'" & vbCrLf
        strSQL = strSQL & "END" & vbCrLf
        strSQL = strSQL & "ELSE IF EXISTS(SELECT * FROM EgswUser WHERE Email='" & strUser_Email & "')" & vbCrLf
        strSQL = strSQL & "BEGIN " & vbCrLf
        strSQL = strSQL & "	UPDATE EgswUser" & vbCrLf
        strSQL = strSQL & "	SET PassCode=cast((SELECT Code FROM EgswUser WHERE Email='" & strUser_Email & "') as nvarchar) + '" & strSessionID & "'" & vbCrLf
        strSQL = strSQL & "	WHERE Email='" & strUser_Email & "'" & vbCrLf
        strSQL = strSQL & "	SELECT PassCode FROM EgswUser WHERE Email='" & strUser_Email & "'" & vbCrLf
        strSQL = strSQL & "END" & vbCrLf

        Dim cmd As New SqlCommand
        Dim cn As New SqlConnection(L_strCnn)

        Try
            With cmd
                .Connection = cn
                .CommandText = strSQL
                .CommandType = CommandType.Text
                .Connection.Open()
                strPassCode = CStr(.ExecuteScalar())
                .Connection.Close()
                cmd.Dispose()
                Return strPassCode
            End With
        Catch ex As Exception
            Return ""
        End Try

    End Function
#End Region

#Region "Remove Methods"
    ''' <summary>
    ''' Delete a user
    ''' </summary>
    ''' <param name="intCodeUser"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteFromList(ByVal intCodeUser As Integer) As enumEgswErrorCode
        Return Me.DeleteFromList(intCodeUser, "", enumEgswTransactionMode.Delete)
    End Function
#End Region


    ''' <summary>
    ''' copy defautl of code site to codeuser
    ''' </summary>
    ''' <param name="intCodeSite">source code site</param>
    ''' <param name="intCodeUser">source codeuser</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CopyDefaultFromSite(ByVal intCodeSite As Integer, ByVal intCodeUser As Integer) As enumEgswErrorCode
        Dim strSQL As String = ""
        strSQL &= "INSERT INTO egswConfig(codeUser, Numero, String, CodeGroup, IsFromRn) "
        strSQL &= "SELECT " & intCodeUser & ", Numero, String, -1 , 0 FROM egswConfig "
        strSQL &= "WHERE codeUser=" & intCodeSite & " AND CodeGroup=-3 "

        Dim sqlCmd As SqlCommand = New SqlCommand
        sqlCmd.Connection = New SqlConnection(L_strCnn)
        sqlCmd.CommandText = strSQL
        sqlCmd.CommandType = CommandType.Text

        Try
            sqlCmd.Connection.Open()
            sqlCmd.ExecuteNonQuery()
            sqlCmd.Connection.Close()

            Return enumEgswErrorCode.OK
        Catch ex As Exception
            If sqlCmd.Connection.State <> ConnectionState.Closed Then sqlCmd.Connection.Close()
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    'DLS March 28 2008
    Public Function UpdateUserSession(ByVal strSessionID As String) As enumEgswErrorCode
        Dim strSQL As String = ""
        strSQL &= "Update EgsWUser SET UserSession='' WHERE UserSession='" & strSessionID & "' "
        Dim sqlCmd As SqlCommand = New SqlCommand
        sqlCmd.Connection = New SqlConnection(L_strCnn)
        sqlCmd.CommandText = strSQL
        sqlCmd.CommandType = CommandType.Text
        Try
            sqlCmd.Connection.Open()
            sqlCmd.ExecuteNonQuery()
            sqlCmd.Connection.Close()
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            If sqlCmd.Connection.State <> ConnectionState.Closed Then sqlCmd.Connection.Close()
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function GetUserByActiveDirectory(ByVal strActiveDirectory As String) As SqlDataReader
        Dim strSQL As String = "SELECT * FROM egswUser WHERE username=@ActiveDirectory and ActiveDirectoryType=1 "
        Dim arrParam() As SqlParameter = {New SqlParameter("@ActiveDirectory", strActiveDirectory)}
        Dim dr As SqlDataReader = ExecuteReader(L_strCnn, CommandType.Text, strSQL, arrParam)
        Return dr
    End Function

    Public Function GetUserPasswordInfo(ByVal strText As String) As SqlDataReader 'VRP 15.01.2009
        Dim strSQL As String = ""
        strSQL += "SELECT UserName, Password, Email, FullName, ISNULL(EGSID,0) as EGSID  FROM EgswUser "
        strSQL += "WHERE UserName=@strText OR Email=@strText"

        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@strText", strText)

        Dim dr As SqlDataReader = ExecuteReader(L_strCnn, CommandType.Text, strSQL, arrParam)
        Return dr
    End Function


    Public Function GetSaleSite() As SqlDataReader 'AMTLA 2016.03.08
        Dim strSQL As String = ""
        strSQL += "SELECT code, name FROM EgswSalesSite "

        Dim arrParam(0) As SqlParameter
        '' arrParam(0) = New SqlParameter("@strText", strText)

        Dim dr As SqlDataReader = ExecuteReader(L_strCnn, CommandType.Text, strSQL, arrParam)
        Return dr
    End Function

    Public Function GetUserName(ByVal strPassCode As String) As String 'VRP 19.01.2009
        Dim strSQL As String = "SELECT UserName FROM EgswUser WHERE PassCode='" & strPassCode & "'"
        Dim cmd As New SqlCommand
        Dim cn As New SqlConnection(L_strCnn)
        Dim strUserName As String

        With cmd
            .Connection = cn
            .CommandType = CommandType.Text
            .CommandText = strSQL
            .Connection.Open()
            strUserName = CStr(.ExecuteScalar)
            .Connection.Close()
            .Dispose()
        End With

        Return strUserName
    End Function

    Public Function GetUserNameandPassword(ByVal intCode As String) As DataSet  'JBB 06.10.2011
        Dim strSQL As String = "SELECT UserName,Password FROM EgswUser WHERE Code=" & intCode & ""
        Dim cmd As New SqlCommand
        Dim ds As DataSet = ExecuteDataset(L_strCnn, CommandType.Text, strSQL)
        Return ds
    End Function



    '---JRN 09.01.2010
    Public Sub SaveLogs(ByVal intCodeUser As Integer, ByVal dateLogin As DateTime)
        Dim arrParam(1) As SqlParameter
        'arrParam(0) = New SqlParameter("@retval", "")
        'arrParam(0).Direction = ParameterDirection.ReturnValue)
        arrParam(0) = New SqlParameter("@intCodeUser", intCodeUser)
        arrParam(1) = New SqlParameter("@dateLogin", dateLogin)


        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "USER_EgswRecordLogin", arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
    '---

    '---JRN 09.02.2010
    Public Function GetUserLoginCountList(ByVal dateStart As String, ByVal dateEnd As String, Optional ByVal intRecsPerPage As Integer = 10, Optional ByVal intPage As Integer = 1, Optional ByVal bActiveOnly As Boolean = False) As DataTable
        Dim arrParam(4) As SqlParameter
        'arrParam(0) = New SqlParameter("@retval", "")
        'arrParam(0).Direction = ParameterDirection.ReturnValue)
        arrParam(0) = New SqlParameter("@dateStart", dateStart)
        arrParam(1) = New SqlParameter("@dateEnd", dateEnd)
        arrParam(2) = New SqlParameter("@RecsPerPage", intRecsPerPage)
        arrParam(3) = New SqlParameter("@Page", intPage)
        arrParam(4) = New SqlParameter("@bActiveOnly", bActiveOnly)

        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "USER_GETUserList", arrParam).Tables(0)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    '---

    '---JRN 09.02.2010
    Public Function GetUserLogins(ByVal intCodeUser As Integer, ByVal dateStart As String, ByVal dateEnd As String, Optional ByVal intRecsPerPage As Integer = 10, Optional ByVal intPage As Integer = 1) As DataSet
        Dim arrParam(4) As SqlParameter
        'arrParam(0) = New SqlParameter("@retval", "")
        'arrParam(0).Direction = ParameterDirection.ReturnValue)
        arrParam(0) = New SqlParameter("@CodeUser", intCodeUser)
        arrParam(1) = New SqlParameter("@dateStart", dateStart)
        arrParam(2) = New SqlParameter("@dateEnd", dateEnd)
        arrParam(3) = New SqlParameter("@RecsPerPage", intRecsPerPage)
        arrParam(4) = New SqlParameter("@Page", intPage)

        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "USER_GETUserLogins", arrParam)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    '---


    Public Function GetUserListExceptwithRole(ByVal intCodeSite As Integer, ByVal intCodeProperty As Integer, ByVal intCodeTrans As Integer, ByVal intExceptRole As Integer) As DataTable
        Dim arrParam(3) As SqlParameter
        'arrParam(0) = New SqlParameter("@retval", "")
        'arrParam(0).Direction = ParameterDirection.ReturnValue)
        arrParam(0) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@intCodeProperty", intCodeProperty)
        arrParam(2) = New SqlParameter("@intCodeTrans", intCodeTrans)
        arrParam(3) = New SqlParameter("@intExceptRole", intExceptRole)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "GET_UserListRole", arrParam).Tables(0)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetUserSecurityQuestions(ByVal intCodeUser As Integer, ByVal intCodeLang As Integer, blnActiveOnly As Boolean) As DataTable
        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeUser", intCodeUser)
        arrParam(1) = New SqlParameter("@intCodeLang", intCodeLang)
        arrParam(2) = New SqlParameter("@bitActiveOnly", blnActiveOnly)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "USP_EgswUserSecurityQuestionsGetList", arrParam).Tables(0)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GenerateStrongPassword() As String
        Dim intMinimumPasswordLength As Integer = 8
        Dim cConfig As New clsConfig(enumAppType.WebApp, L_strCnn)
        intMinimumPasswordLength = cConfig.GetConfig(0, clsConfig.enumNumeros.PasswordLoginMinimumLength, clsConfig.CodeGroup.global, 8)

        'AGL 2014.09.22 - password should not be less than 3 characters
        If intMinimumPasswordLength < 3 Then intMinimumPasswordLength = 3

        Dim blnPasswordStrengthValid As Boolean = False
        While blnPasswordStrengthValid = False
            GenerateStrongPassword = GenerateStrongPassword(intMinimumPasswordLength, _
                            intMinimumPasswordLength + 2)
            If isPasswordStrong(GenerateStrongPassword) Then
                blnPasswordStrengthValid = True
            End If
        End While

        cConfig = Nothing
    End Function

    Private Function isPasswordStrong(strPassword) As Boolean
        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@strPassword", strPassword)
        Dim blnPasswordIsstrong As Boolean = False
        Try
            blnPasswordIsstrong = CBool(ExecuteScalar(L_strCnn, CommandType.Text, "SELECT dbo.[fn_EgswCheckPasswordStrengthRequirement](@strPassword)", arrParam))
            Return blnPasswordIsstrong
        Catch ex As Exception
            Return False
        End Try
    End Function

    ' <summary>
    ' Generates a random password of the exact length.
    ' </summary>
    ' <param name="length">
    ' Exact password length.
    ' </param>
    ' <returns>
    ' Randomly generated password.
    ' </returns>
    Public Shared Function GenerateStrongPassword(length As Integer) As String
        'AGL 2014.09.22 - password should not be less than 3 characters
        If length < 3 Then length = 3

        GenerateStrongPassword = GenerateStrongPassword(length, length)
    End Function

    ' <summary>
    ' Generates a random password.
    ' </summary>
    ' <param name="minLength">
    ' Minimum password length.
    ' </param>
    ' <param name="maxLength">
    ' Maximum password length.
    ' </param>
    ' <returns>
    ' Randomly generated password.
    ' </returns>
    ' <remarks>
    ' The length of the generated password will be determined at
    ' random and it will fall with the range determined by the
    ' function parameters.
    ' </remarks>
    Public Shared Function GenerateStrongPassword(minLength As Integer, _
                                    maxLength As Integer) _
        As String

        ' Make sure that input parameters are valid.
        If (minLength <= 0 Or maxLength <= 0 Or minLength > maxLength) Then
            GenerateStrongPassword = Nothing
        End If

        'AGL 2014.09.22 - password should not be less than 3 characters
        If minLength < 3 Then minLength = 3
        If maxLength < minLength Then maxLength = minLength

        ' Create a local array containing supported password characters
        ' grouped by types. You can remove character groups from this
        ' array, but doing so will weaken the password strength.
        Dim charGroups As Char()() = New Char()() _
        { _
            PASSWORD_CHARS_LCASE.ToCharArray(), _
            PASSWORD_CHARS_UCASE.ToCharArray(), _
            PASSWORD_CHARS_NUMERIC.ToCharArray(), _
            PASSWORD_CHARS_SPECIAL.ToCharArray() _
        }

        ' Use this array to track the number of unused characters in each
        ' character group.
        Dim charsLeftInGroup As Integer() = New Integer(charGroups.Length - 1) {}

        ' Initially, all characters in each group are not used.
        Dim I As Integer
        For I = 0 To charsLeftInGroup.Length - 1
            charsLeftInGroup(I) = charGroups(I).Length
        Next

        ' Use this array to track (iterate through) unused character groups.
        Dim leftGroupsOrder As Integer() = New Integer(charGroups.Length - 1) {}

        ' Initially, all character groups are not used.
        For I = 0 To leftGroupsOrder.Length - 1
            leftGroupsOrder(I) = I
        Next

        ' Because we cannot use the default randomizer, which is based on the
        ' current time (it will produce the same "random" number within a
        ' second), we will use a random number generator to seed the
        ' randomizer.

        ' Use a 4-byte array to fill it with random bytes and convert it then
        ' to an integer value.
        Dim randomBytes As Byte() = New Byte(3) {}

        ' Generate 4 random bytes.
        Dim rng As RNGCryptoServiceProvider = New RNGCryptoServiceProvider()

        rng.GetBytes(randomBytes)

        ' Convert 4 bytes into a 32-bit integer value.
        Dim seed As Integer = ((randomBytes(0) And &H7F) << 24 Or _
                                randomBytes(1) << 16 Or _
                                randomBytes(2) << 8 Or _
                                randomBytes(3))

        ' Now, this is real randomization.
        Dim random As Random = New Random(seed)

        ' This array will hold password characters.
        Dim password As Char() = Nothing

        ' Allocate appropriate memory for the password.
        If (minLength < maxLength) Then
            password = New Char(random.Next(minLength - 1, maxLength)) {}
        Else
            password = New Char(minLength - 1) {}
        End If

        ' Index of the next character to be added to password.
        Dim nextCharIdx As Integer

        ' Index of the next character group to be processed.
        Dim nextGroupIdx As Integer

        ' Index which will be used to track not processed character groups.
        Dim nextLeftGroupsOrderIdx As Integer

        ' Index of the last non-processed character in a group.
        Dim lastCharIdx As Integer

        ' Index of the last non-processed group.
        Dim lastLeftGroupsOrderIdx As Integer = leftGroupsOrder.Length - 1

        ' Generate password characters one at a time.
        For I = 0 To password.Length - 1

            ' If only one character group remained unprocessed, process it;
            ' otherwise, pick a random character group from the unprocessed
            ' group list. To allow a special character to appear in the
            ' first position, increment the second parameter of the Next
            ' function call by one, i.e. lastLeftGroupsOrderIdx + 1.
            If (lastLeftGroupsOrderIdx = 0) Then
                nextLeftGroupsOrderIdx = 0
            Else
                nextLeftGroupsOrderIdx = random.Next(0, lastLeftGroupsOrderIdx)
            End If

            ' Get the actual index of the character group, from which we will
            ' pick the next character.
            nextGroupIdx = leftGroupsOrder(nextLeftGroupsOrderIdx)

            ' Get the index of the last unprocessed characters in this group.
            lastCharIdx = charsLeftInGroup(nextGroupIdx) - 1

            ' If only one unprocessed character is left, pick it; otherwise,
            ' get a random character from the unused character list.
            If (lastCharIdx = 0) Then
                nextCharIdx = 0
            Else
                nextCharIdx = random.Next(0, lastCharIdx + 1)
            End If

            ' Add this character to the password.
            password(I) = charGroups(nextGroupIdx)(nextCharIdx)

            ' If we processed the last character in this group, start over.
            If (lastCharIdx = 0) Then
                charsLeftInGroup(nextGroupIdx) = _
                                charGroups(nextGroupIdx).Length
                ' There are more unprocessed characters left.
            Else
                ' Swap processed character with the last unprocessed character
                ' so that we don't pick it until we process all characters in
                ' this group.
                If (lastCharIdx <> nextCharIdx) Then
                    Dim temp As Char = charGroups(nextGroupIdx)(lastCharIdx)
                    charGroups(nextGroupIdx)(lastCharIdx) = _
                                charGroups(nextGroupIdx)(nextCharIdx)
                    charGroups(nextGroupIdx)(nextCharIdx) = temp
                End If

                ' Decrement the number of unprocessed characters in
                ' this group.
                charsLeftInGroup(nextGroupIdx) = _
                           charsLeftInGroup(nextGroupIdx) - 1
            End If

            ' If we processed the last group, start all over.
            If (lastLeftGroupsOrderIdx = 0) Then
                lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1
                ' There are more unprocessed groups left.
            Else
                ' Swap processed group with the last unprocessed group
                ' so that we don't pick it until we process all groups.
                If (lastLeftGroupsOrderIdx <> nextLeftGroupsOrderIdx) Then
                    Dim temp As Integer = _
                                leftGroupsOrder(lastLeftGroupsOrderIdx)
                    leftGroupsOrder(lastLeftGroupsOrderIdx) = _
                                leftGroupsOrder(nextLeftGroupsOrderIdx)
                    leftGroupsOrder(nextLeftGroupsOrderIdx) = temp
                End If

                ' Decrement the number of unprocessed groups.
                lastLeftGroupsOrderIdx = lastLeftGroupsOrderIdx - 1
            End If
        Next

        ' Convert password characters into a string and return the result.
        GenerateStrongPassword = New String(password)
    End Function
End Class

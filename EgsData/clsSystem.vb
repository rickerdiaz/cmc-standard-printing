Option Strict Off
Imports System.Data.SqlClient
'Imports EgsKeyMk
Imports RnToolsProject
#Region "Class Header"
'Name               : clsSystem
'Decription         : Manages Category Table
'Date Created       : 07.09.2005
'Author             : VBV
'Revision History   : VBV - 20.09.2005 
'                       Accepts a connection string as opposed to a connection object. Class performs on a disconnected state.
'
#End Region

Public Class clsSystem
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

#Region "Class Functions and Properties"
    'Public Sub New(ByVal eAppType As enumAppType, ByVal objCnn As SqlConnection, _
    '    ByVal strCnn As String, Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)
    Public Sub New(ByVal eAppType As enumAppType, ByVal strCnn As String, _
        Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)

        Try
            'If eAppType = enumAppType.SmartClient Then
            '    If objCnn Is Nothing Then
            '        L_Cnn = New SqlConnection
            '        L_Cnn.ConnectionString = strCnn
            '        L_Cnn.Open()
            '    ElseIf objCnn.State = ConnectionState.Closed Then
            '        objCnn.Open()
            '        L_Cnn = objCnn
            '    Else
            '        L_Cnn = objCnn
            '    End If
            '    L_strCnn = L_Cnn.ConnectionString
            'End If
            L_AppType = eAppType
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn
        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try

    End Sub

    Protected Overrides Sub Finalize()
        ' ClearMarkings() 'items marked as not deleted
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

    Public Enum enumRegKeyOptions      ' Registry Information Options
        Module_Pocket = 4
        IsRecipeExchange = 66
        UseTotalNumberOfLicense = 76
        MultiLanguageTranslation = 90
        MultiSetOfPrice = 112
        IsRecipeGallery = 116
    End Enum

    Public Structure structRegOptions       ' User defined variable to hold Registry Options, Run Decompose Key to fill values
        Public IsRecipeExchange As Boolean
        Public IsRecipeGallery As Boolean
        Public AllowModulePocket As Boolean
        Public AllowMultiSetOfPrice As Boolean
        Public AllowMultiLanguageTranslation As Boolean
        Public UseTotalNumberofLicense As Boolean
        Public strversion As String
        Public strserial As String
        Public stroptions As String
        Public strLicenseRN As String
        Public strLicenseFB As String
        Public strDateDays As String
        Public strSupportDays As String
        Public licensestatus As enumLicenseStatus
        'Status: Status of the licence
        'Status = 1 : Trial period (limited use of the program)
        'Status = 2 : Over the trial period (still not registered) - (most functions blocked)
        'Status = 10: Registred
    End Structure

    Public Enum enumLicenseStatus
        TrialPeriod = 1
        OverTrialPeriod = 2
        Registered = 10
    End Enum

    Private m_structRegOptions As structRegOptions

#Region "Get Methods"

    ''' <summary>
    ''' Returns decomposed Registry Options in memory. Call DecomposeRegistrationKey first before this function
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property RegistryInformation() As structRegOptions
        Get
            Return m_structRegOptions
        End Get
    End Property

    ''' <summary>
    ''' Returns 1 if CheckVersion is valid, otherwise, 0.
    ''' </summary>
    ''' <param name="strKey"></param>
    ''' <param name="strheader"></param>
    ''' <param name="versionOnLine"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckRegistrationVersionType(ByVal strKey As String, ByVal strheader As String, ByRef versionOnLine As Boolean) As Integer
        'Dim EGSKey As New EgsKeyMk.EgsKeyMk
        'Return EGSKey.fctCheckVersionType(strKey, strheader, versionOnLine)
    End Function
    ''' <summary>
    ''' Returns Licencse Status
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckProtection() As enumLicenseStatus
        'Dim EGSKey As New EgsKeyMk.EgsKeyMk

        'Me.FetchReturnType = enumEgswFetchType.DataReader
        'Dim dr As SqlDataReader = Me.GetSystem
        'Dim strKey As String = ""
        'Dim strHeader As String = ""
        'While dr.Read
        '    strKey = CStrDB(dr("sysKey"))
        '    strHeader = CStrDB(dr("sysHeader"))
        'End While
        'dr.Close()

        'Dim blnVersionOnLine As Boolean
        'Dim status As Integer = 0
        ''Status: Status of the licence
        ''Status = 1 : Trial period (limited use of the program)
        ''Status = 2 : Over the trial period (still not registered) - (most functions blocked)
        ''Status = 10: Registred
        ''

        '' Check version
        'Dim blnIsVersionOk As Boolean = CheckRegistrationVersionType(strKey, strHeader, blnVersionOnLine)
        'If blnIsVersionOk Then
        '    m_structRegOptions.licensestatus = enumLicenseStatus.Registered
        'Else
        '    'm_structRegOptions.licensestatus = 1
        '    'If CheckProt(1) Then    ' check if dll 30 days exist and is valid
        '    '    intValue = softSENTRY   ' check if it reached 30 days
        '    '    If intValue = 1 Then    ' 30 days trial
        '    '        m_structRegOptions.licensestatus = 1
        '    '    Else        ' over 30 days
        '    '        m_structRegOptions.licensestatus = 2
        '    '    End If
        '    'Else
        '    '    m_structRegOptions.licensestatus = 2
        '    '    ' unchecked
        '    'End If
        '    m_structRegOptions.licensestatus = enumLicenseStatus.OverTrialPeriod
        '    ' unchecked
        'End If
        'Return m_structRegOptions.licensestatus
    End Function
    '    Private Function CheckProt(ByVal lngDLL As Long) As Boolean
    '        Dim FoundSizeLng As Long
    '        Dim strPath As String
    '        Dim strFile As String
    '        Dim RealSizeLng As Long

    '        On Error GoTo Err_fctCheckRcProt
    '        CheckProt = False
    '        '
    '        If lngDLL = 1 Then
    '            RealSizeLng = 64218
    '            strPath = "C:\Inetpub\wwwroot\EgsRecipeNetWeb\bin\"
    '            strFile = "rntrdy.dll"
    '            FoundSizeLng = FileLen(strPath & strFile)
    '        End If

    '        If FoundSizeLng = RealSizeLng Then
    '            CheckProt = True
    '        Else
    '            CheckProt = False
    '        End If

    '        Exit Function

    'Err_fctCheckRcProt:
    '        On Error GoTo 0
    '        Exit Function

    '    End Function

    ''' <summary>
    ''' Get System Information
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSystem() As Object
        Select Case L_bytFetchType
            Case enumEgswFetchType.DataReader
                Return ExecuteReader(L_strCnn, CommandType.StoredProcedure, "sp_EgswGetSystemList")
            Case enumEgswFetchType.DataSet
                Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "sp_EgswGetSystemList")
            Case enumEgswFetchType.DataTable
                Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "sp_EgswGetSystemList").Tables(0)
        End Select
        Return Nothing
    End Function


    ''' <summary>
    ''' Decompose License Key for RecipeNet Web
    ''' </summary>
    ''' <param name="strDecomposedKey"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DecomposeRegistrationKey(Optional ByRef strDecomposedKey As String = "") As Byte
        'Dim EGSKey As New EgsKeyMk.EgsKeyMk

        'Me.FetchReturnType = enumEgswFetchType.DataReader
        'Dim dr As SqlDataReader = Me.GetSystem
        'Dim strKey As String = ""
        'Dim strHeader As String = ""
        'While dr.Read
        '    strKey = CStrDB(dr("sysKey"))
        '    strHeader = CStrDB(dr("sysHeader"))
        'End While
        'dr.Close()

        'm_structRegOptions = New structRegOptions

        'Dim b As Byte = EGSKey.fctDecomposeKeySolution(strKey, strHeader, m_structRegOptions.strversion, m_structRegOptions.strserial, m_structRegOptions.stroptions, m_structRegOptions.strLicenseRN, m_structRegOptions.strLicenseFB, m_structRegOptions.strDateDays, m_structRegOptions.strSupportDays)
        'strDecomposedKey = m_structRegOptions.strserial

        '' 1 License = 5 Users
        'm_structRegOptions.strLicenseRN = CStr(CInt(m_structRegOptions.strLicenseRN) * 5)

        'DecomposeOptions(m_structRegOptions)  ' Read Options

        'Return b
    End Function
    ''' <summary>
    ''' Decompose Option Key to specific program options.
    ''' </summary>
    ''' <param name="strucRegOption"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function DecomposeOptions(ByRef strucRegOption As structRegOptions) As Boolean
        On Error GoTo errDecompose
        Dim k As Int32
        Dim j As Integer
        Dim bytMax_Option As Byte = 29
        Dim intOptions As Integer
        Dim i As Integer
        Dim flagOption As Boolean
        Dim from As Integer
        Dim length As Integer
        Dim curStr As String
        For k = 3 To 0 Step -1
            from = 6 * k + 1
            length = 6
            curStr = Mid(m_structRegOptions.stroptions, from, length)
            intOptions = fct32To10(curStr)
            flagOption = CInt((intOptions And CInt((2 ^ (j - 1))))) = CInt((2 ^ (j - 1)))
            If intOptions > 0 Then
                For j = 1 To bytMax_Option
                    i = (k * bytMax_Option) + j
                    Select Case i
                        Case enumRegKeyOptions.IsRecipeExchange
                            m_structRegOptions.IsRecipeExchange = flagOption
                        Case enumRegKeyOptions.IsRecipeGallery
                            m_structRegOptions.IsRecipeGallery = flagOption
                        Case enumRegKeyOptions.Module_Pocket
                            m_structRegOptions.AllowModulePocket = flagOption
                        Case enumRegKeyOptions.MultiSetOfPrice
                            m_structRegOptions.AllowMultiSetOfPrice = flagOption
                        Case enumRegKeyOptions.MultiLanguageTranslation
                            m_structRegOptions.AllowMultiLanguageTranslation = flagOption
                        Case enumRegKeyOptions.UseTotalNumberOfLicense
                            m_structRegOptions.UseTotalNumberofLicense = flagOption
                    End Select
                Next
            End If
        Next
        Return True
errDecompose:
        Return False
    End Function

#End Region

#Region "Save Methods"
    ''' <summary>
    ''' Enable Usage of Properties and Sites or Sites Only.
    ''' </summary>
    ''' <param name="eGoupLevel"></param>
    ''' <param name="oTrans"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateGroupLevel(ByVal eGoupLevel As enumGroupLevel, Optional ByVal oTrans As SqlTransaction = Nothing) As enumEgswErrorCode
        Return SaveIntoList(0, eGoupLevel, 0, 0, 0, Now, Now, 0, 0, 0, 0, Now, enumEgswTransactionMode.UpdateSystemenumGroupLevel, oTrans)
    End Function

    ''' <summary>
    ''' Update License Key
    ''' </summary>
    ''' <param name="strprgKey"></param>
    ''' <param name="strHeader"></param>
    ''' <param name="oTrans"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateLicense(ByVal strprgKey As String, ByVal strHeader As String, Optional ByVal oTrans As SqlTransaction = Nothing) As enumEgswErrorCode
        Return SaveIntoList(0, 0, 0, strprgKey, strHeader, Now, Now, 0, 0, 0, 0, Now, enumEgswTransactionMode.UpdateSystemKeyAndHeaderOnly, oTrans)
    End Function

#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Update System Registry Information
    ''' </summary>
    ''' <param name="intCode"></param>
    ''' <param name="grouplevel"></param>
    ''' <param name="intVersion"></param>
    ''' <param name="strPrgKey"></param>
    ''' <param name="strHeader"></param>
    ''' <param name="dtDates"></param>
    ''' <param name="dtStartDate"></param>
    ''' <param name="blnAcknAgreement"></param>
    ''' <param name="strSaved"></param>
    ''' <param name="IsNewVersion"></param>
    ''' <param name="intCountLog"></param>
    ''' <param name="dteLastDateLog"></param>
    ''' <param name="TranMode"></param>
    ''' <param name="oTransaction"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function SaveIntoList(ByVal intCode As Integer, ByVal grouplevel As enumGroupLevel, ByVal intVersion As Integer, ByVal strPrgKey As String, ByVal strHeader As String, ByVal dtDates As DateTime, ByVal dtStartDate As DateTime, ByVal blnAcknAgreement As Boolean, ByVal strSaved As String, ByVal IsNewVersion As Boolean, ByVal intCountLog As Integer, ByVal dteLastDateLog As DateTime, ByVal TranMode As enumEgswTransactionMode, Optional ByVal oTransaction As SqlTransaction = Nothing) As enumEgswErrorCode

        Dim arrParam(12) As SqlParameter
        arrParam(0) = New SqlParameter("@intCode", intCode)
        arrParam(1) = New SqlParameter("@intGroupLevel", grouplevel)
        arrParam(2) = New SqlParameter("@intVersion", intVersion)
        arrParam(3) = New SqlParameter("@ncharPrgKey", strPrgKey)
        arrParam(4) = New SqlParameter("@nvcHeader", strHeader)
        arrParam(5) = New SqlParameter("@dteDates", dtDates)
        arrParam(6) = New SqlParameter("@dteStartDate", dtStartDate)
        arrParam(7) = New SqlParameter("@HasAcknAgreement", blnAcknAgreement)
        arrParam(8) = New SqlParameter("@nvcSaved", strSaved)
        arrParam(9) = New SqlParameter("@IsNewVersion", IsNewVersion)
        arrParam(10) = New SqlParameter("@intCountLog", intCountLog)
        arrParam(11) = New SqlParameter("@dteLastLogDate", dteLastDateLog)
        arrParam(12) = New SqlParameter("@tntTranMode", TranMode)
        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswSystemUpdate", oTransaction, arrParam)
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Throw ex
        End Try
    End Function
#End Region

End Class

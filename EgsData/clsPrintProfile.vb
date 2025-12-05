Imports System.Data.SqlClient
Imports System.Data

#Region "Class Header"
'Name               : clsPrintProfile
'Decription         : Manages PrintProfile Table
'Date Created       : 12.7.2005
'Author             : JRL
'Revision History   : 
#End Region

''' <summary>
''' Manages PrintProfile Table
''' </summary>
''' <remarks></remarks>

Public Class clsPrintProfile

#Region "Variable Declarations / Dependencies"
    Inherits clsDBRoutine

    'Private L_Cnn As SqlConnection
    'Private cmd As New SqlCommand
    Private L_ErrCode As enumEgswErrorCode
    Private L_lngCodeSite As Int32 = -1
    Private L_RoleLevelHighest As Int16 = -1

    'Properties
    Private L_udtUser As structUser
    Private L_AppType As enumAppType
    Private L_dtList As DataTable
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType
    Private L_lngCode As Int32
#End Region

#Region "Class Functions and Properties"
    Public Sub New(ByVal udtUser As structUser, ByVal eAppType As enumAppType, ByVal strCnn As String, _
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
            '     If udtUser.Code <= 0 Then Throw New Exception("Invalid CodeUser.")
            L_udtUser = udtUser
            L_AppType = eAppType
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn
        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try

    End Sub

    Protected Overrides Sub Finalize()
        ClearMarkings() 'items marked as not deleted
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

    'Public Property CodeUser() As Int32
    '    Get
    '        CodeUser = l_udtuser.code
    '    End Get
    '    Set(ByVal value As Int32)
    '        l_udtuser.code = value
    '    End Set
    'End Property

    'Public Property CodeSite() As Int32
    '    Get
    '        CodeSite = L_lngCodeSite
    '    End Get
    '    Set(ByVal value As Int32)
    '        L_lngCodeSite = value
    '    End Set
    'End Property

#End Region

#Region "Private Methods"

    Private Function FetchList(ByVal lngCodeSite As Int32, ByVal lngCode As Int32, ByVal eType As enumReportType, _
        ByVal lngCodeTrans As Int32, ByVal bytStatus As Byte, Optional ByVal strName As String = "") As Object

        Dim ds As New DataSet
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim dr As SqlDataReader
        Dim cmd As New SqlCommand
        Dim lngCodeProperty As Int32 = -1

        If L_udtUser.RoleLevelHighest = 0 Then 'Get ALL items
            lngCodeProperty = -1
        ElseIf L_udtUser.RoleLevelHighest = 1 Then 'Get ALL items for a site
            lngCodeSite = L_udtUser.Site.Code
        ElseIf L_udtUser.RoleLevelHighest = 2 Then 'Get ALL items for a property
            lngCodeProperty = L_udtUser.Site.Group
        End If

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswPrintProfileGetList"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@tntType", SqlDbType.TinyInt).Value = eType
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = lngCodeTrans
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@Status", SqlDbType.TinyInt).Value = bytStatus
                If strName.Trim <> "" Then _
                    .Parameters.Add("@vchName", SqlDbType.NVarChar, 150).Value = strName
            End With

            If L_bytFetchType = enumEgswFetchType.DataReader Then
                cmd.Connection.Open()
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection)

            ElseIf L_bytFetchType = enumEgswFetchType.DataTable Then
                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With

            ElseIf L_bytFetchType = enumEgswFetchType.DataSet Then
                With da
                    .SelectCommand = cmd
                    'dt.BeginLoadData()
                    .Fill(ds, "ItemList")
                    'dt.EndLoadData()
                End With
            End If

        Catch ex As Exception
            dr = Nothing
            ds = Nothing
            dt.Dispose()
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        If L_bytFetchType = enumEgswFetchType.DataReader Then
            Return dr
        ElseIf L_bytFetchType = enumEgswFetchType.DataTable Then
            Return dt
        ElseIf L_bytFetchType = enumEgswFetchType.DataSet Then
            Return ds
        End If

    End Function

    Private Function FetchTranslationList(ByVal lngCodeTrans As Long) As DataTable

        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswPrintProfileGetTranslationList"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = lngCodeTrans
            End With

            With da
                .SelectCommand = cmd
                dt.BeginLoadData()
                .Fill(dt)
                dt.EndLoadData()
            End With

        Catch ex As Exception
            cmd.Dispose()
            dt = Nothing
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return dt

    End Function

    Private Function SaveIntoList(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByRef lngCode As Int32, ByVal udtPrintProfile As structPrintProfile, ByVal strCodeSiteList As String, _
        ByVal strCodePrintProfileList As String, ByVal TranMode As enumEgswTransactionMode, _
        Optional ByVal oTransaction As SqlTransaction = Nothing) As enumEgswErrorCode

        Dim cmd As New SqlCommand

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If

                If oTransaction Is Nothing Then
                    .Connection = New SqlConnection(L_strCnn)
                Else
                    .Connection = oTransaction.Connection
                    .Transaction = oTransaction
                End If

                .CommandText = "sp_EgswPrintProfileUpdate"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@intCode", SqlDbType.Int).Value = udtPrintProfile.Code
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 150).Value = udtPrintProfile.Name
                .Parameters.Add("@tntType", SqlDbType.TinyInt).Value = udtPrintProfile.Type
                .Parameters.Add("@intPosition", SqlDbType.Int).Value = udtPrintProfile.Position
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = udtPrintProfile.IsGlobal
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode

                .Parameters("@intCode").Direction = ParameterDirection.InputOutput
                .Parameters("@retval").Direction = ParameterDirection.ReturnValue

                strCodeSiteList.Trim()
                If strCodeSiteList <> "" Then
                    If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) Then
                        Return enumEgswErrorCode.InvalidCodeList
                    Else
                        .Parameters.Add("@vchCodeSiteList", SqlDbType.VarChar, 8000).Value = strCodeSiteList
                    End If
                End If

                strCodePrintProfileList.Trim()
                If strCodePrintProfileList <> "" Then
                    If Not (strCodePrintProfileList.StartsWith("(") And strCodePrintProfileList.EndsWith(")")) Then
                        Return enumEgswErrorCode.InvalidCodeList
                    Else
                        .Parameters.Add("@vchCodeMergeList", SqlDbType.VarChar, 8000).Value = strCodePrintProfileList
                    End If
                End If

                .Parameters("@retval").Direction = ParameterDirection.ReturnValue
                If oTransaction Is Nothing Then .Connection.Open()
                .ExecuteNonQuery()
                If oTransaction Is Nothing Then .Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                lngCode = CInt(.Parameters("@intCode").Value)
            End With

        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If oTransaction Is Nothing AndAlso cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        If oTransaction Is Nothing AndAlso cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode
    End Function

    Private Function RemoveFromList(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
         ByVal lngCode As Int32, ByVal IsGlobal As Boolean, ByVal TranMode As enumEgswTransactionMode, _
         Optional ByVal bytStatus As Byte = 0, Optional ByVal strCodeList As String = "") As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Dim lngCodeProperty As Int32

        If L_udtUser.RoleLevelHighest = 0 Then 'Unshare to ALL 
            lngCodeProperty = -1
        Else 'Unshare to ALL sites belonging to a property or Unshare to self
            lngCodeProperty = L_udtUser.Site.Group
        End If

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswPrintProfileDelete"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = IsGlobal
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeProperty", SqlDbType.Int).Value = lngCodeProperty
                .Parameters.Add("@tntTranMode", SqlDbType.TinyInt).Value = TranMode
                strCodeList.Trim()
                If strCodeList <> "" Then
                    If Not (strCodeList.StartsWith("(") And strCodeList.EndsWith(")")) Then
                        Return enumEgswErrorCode.InvalidCodeList
                    Else
                        .Parameters.Add("@vchCodeList", SqlDbType.VarChar, 8000).Value = strCodeList
                    End If
                End If

                If TranMode = enumEgswTransactionMode.ModifyStatus Then
                    .Parameters.Add("@bytStatus", SqlDbType.TinyInt).Value = bytStatus
                End If


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

        If L_ErrCode = enumEgswErrorCode.OneItemNotDeleted Then
            Dim da As New SqlDataAdapter

            Try
                cmd.CommandText = "sp_EgswItemGetNotDeleted"
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.Clear()
                cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                cmd.Parameters.Add("@vchTableName", SqlDbType.VarChar, 50).Value = "EgswPrintProfile"

                L_dtList = New DataTable
                With da
                    .SelectCommand = cmd
                    L_dtList.BeginLoadData()
                    .Fill(L_dtList)
                    L_dtList.EndLoadData()
                End With
            Catch ex As Exception
                L_dtList.Dispose()
                If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
                cmd.Dispose()
                Throw New Exception(ex.Message, ex)
            End Try
        End If

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

    End Function

    Private Function ClearMarkings() As enumEgswErrorCode
        'Deactivate items that were not deleted by the Delete module
        If L_udtUser.Code <> -1 And L_lngCodeSite <> -1 Then
            Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Deactivate)
        End If
    End Function


#End Region

#Region "Get Methods"
    ''' <summary>
    ''' Get all PrintProfiles.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList() As Object

        Return FetchList(-1, -1, enumReportType.None, -1, 255)

    End Function

    ''' <summary>
    ''' Get a PrintProfile by Code.
    ''' </summary>
    ''' <param name="lngCode">The Code of the PrintProfile to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal lngCode As Int32) As Object

        Return FetchList(-1, lngCode, enumReportType.None, -1, 255)

    End Function

    ''' <summary>
    ''' Get all PrintProfiles by Type.
    ''' </summary>
    ''' <param name="eType">One of the enumReportType values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal eType As enumReportType) As Object

        Return FetchList(-1, -1, eType, -1, 255)

    End Function

    ''' <summary>
    ''' Get all PrintProfiles by Status.
    ''' </summary>
    ''' <param name="bytStatus">The status of the PrintProfiles to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal bytStatus As Byte) As Object
        'Get all by Status
        Return FetchList(-1, -1, enumReportType.None, -1, bytStatus)

    End Function

    ''' <summary>
    ''' Get all PrintProfiles with the list of Site names to which they are shared to.
    ''' </summary>
    ''' <param name="eType">One of the enumDataListType values.</param>
    ''' <param name="lngCodeTrans">The Code of the language translation.</param>
    ''' <param name="bytStatus">The status of the PrintProfiles to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal eType As enumReportType, ByVal lngCodeTrans As Int32, ByVal bytStatus As Byte) As Object

        Return FetchList(-1, -1, eType, lngCodeTrans, bytStatus)

    End Function

    ''' <summary>
    ''' Get all PrintProfiles shared to a specific site.
    ''' </summary>
    ''' <param name="eType">One of the enumDataListType values.</param>
    ''' <param name="lngCodeTrans">The Code of the language translation.</param>
    ''' <param name="bytStatus">The status of the PrintProfile to be fetched.</param>
    ''' <param name="lngCodeSite">The site to which the PrintProfile is shared.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal eType As enumReportType, ByVal lngCodeTrans As Int32, ByVal bytStatus As Byte, ByVal lngCodeSite As Int32) As Object

        Return FetchList(lngCodeSite, -1, eType, lngCodeTrans, bytStatus)

    End Function

    ''' <summary>
    ''' Get all Translations.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTranslationList() As DataTable
        'Get all
        Return FetchTranslationList(-1)

    End Function

    ''' <summary>
    ''' Get a specific translation.
    ''' </summary>
    ''' <param name="lngCodeTrans">The Code of the language translation.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTranslationList(ByVal lngCodeTrans As Long) As DataTable
        'Filter by CodeTrans
        Return FetchTranslationList(lngCodeTrans)

    End Function

    ''' <summary>
    ''' Get a Category by Name w/in the codesite.
    ''' </summary>
    ''' <param name="strName">The name of the Category to be fetched.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal strName As String, ByVal eDataReporttype As enumReportType, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, ByVal bytStatus As Byte) As Object

        Return FetchList(intCodeSite, -1, eDataReporttype, intCodeTrans, bytStatus, strName)

    End Function

    Public Function GetPrintProfileCodeName(ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, _
                                            ByVal eType As enumReportType, Optional ByVal intCodeUser As Integer = -1,
                                            Optional ByVal intCodeProperty As Integer = -1) As Object 'VRP 28.10.2008

        Dim strCommandText As String = "GET_PRINTPROFILECODENAME"

        Dim arrParam(4) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans)
        arrParam(2) = New SqlParameter("@tntType", eType)
        arrParam(3) = New SqlParameter("@intCodeUser", intCodeUser)
        arrParam(4) = New SqlParameter("@intCodeProperty", intCodeProperty)
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetPrintProfileIndexByName(ByVal strName As String, ByVal intCode As Integer) As String
        Dim da As New SqlDataAdapter
        Dim cmd As New SqlCommand
        Dim intVal As String
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_PrintProfileIndex"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@Origname", SqlDbType.NVarChar).Value = strName
                .Parameters.Add("@Code", SqlDbType.Int).Value = intCode
                .Connection.Open()
                intVal = .ExecuteScalar()
                .Connection.Close()
            End With

        Catch ex As Exception
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return intVal
    End Function
#End Region

#Region "Update Methods"
    ''' <summary>
    ''' Standardize PrintProfiles
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="eListeType">One of the enumDataListType values.</param>
    ''' <param name="eItemListType">One of the enumDataListType values.</param>
    ''' <param name="eFormat">One of the enumEgswStandardizationFormat values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Standardize(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal eListeType As enumDataListType, _
        ByVal eItemListType As enumDataListItemType, ByVal eFormat As enumEgswStandardizationFormat) As enumEgswErrorCode

        Dim cmd As New SqlCommand

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswItemStandardizeAll"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@tntFormat", SqlDbType.TinyInt).Value = eFormat
                .Parameters.Add("@tntType", SqlDbType.TinyInt).Value = eListeType
                .Parameters.Add("@tntListType", SqlDbType.TinyInt).Value = eItemListType

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

    ''' <summary>
    ''' Updates the global status of a PrintProfile.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the PrintProfile to be updated.</param>
    ''' <param name="IsGlobal">The global status of the PrintProfile to be updated.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateGlobalStatus(ByVal lngCodeUser As Int32, ByVal lngCode As Int32, ByVal IsGlobal As Boolean) As enumEgswErrorCode

        Dim cmd As New SqlCommand

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswPrintProfileUpdateGlobal"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCode", SqlDbType.Int).Value = lngCode
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = IsGlobal
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser

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

    ''' <summary>
    ''' Updates PrintProfile without sharing it to any sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the PrintProfile to be updated.</param>
    ''' <param name="udtPrintProfile">One of the structPrintProfile values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByRef lngCode As Int32, ByVal udtPrintProfile As structPrintProfile) As enumEgswErrorCode

        Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtPrintProfile, "", "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    ''' <summary>
    ''' Updates PrintProfile sharing it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtPrintProfile">One of the structPrintProfile values.</param>
    ''' <param name="strCodeSiteList">The list of sites where PrintProfile will be shared.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtPrintProfile As structPrintProfile, ByVal strCodeSiteList As String) As enumEgswErrorCode

        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) And strCodeSiteList.Trim <> "" Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        Return SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtPrintProfile, strCodeSiteList, "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode))

    End Function

    ''' <summary>
    ''' Updates PrintProfile and its Translations and share it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtPrintProfile">One of the structPrintProfile values.</param>
    ''' <param name="strCodeSiteList">The list of sites where PrintProfile will be shared.</param>
    ''' <param name="strTransCodeList">The list of Translation Codes of the PrintProfile.</param>
    ''' <param name="strTransNameList">The list of Translation Names of the PrintProfile.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtPrintProfile As structPrintProfile, ByVal strCodeSiteList As String, _
        ByVal arrTransCode() As String, ByVal arrTransName() As String) As enumEgswErrorCode


        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) And strCodeSiteList.Trim <> "" Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        Dim t As SqlTransaction
        Dim cn As New SqlConnection(L_strCnn)

        cn.Open()
        t = cn.BeginTransaction()
        L_ErrCode = SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtPrintProfile, strCodeSiteList, "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode), _
             t)

        'If L_ErrCode = enumEgswErrorCode.OK Then
        '    Try
        '        'Update Translations
        '        '          Dim arrTransCode() As String = (strTransCodeList.Split(CChar(",")))
        '        '     Dim arrTransName() As String = strTransNameList.Split(CChar(","))
        '        '   Dim c As Int32 = arrTransCode.Length - 1
        '        '  Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

        '        'For i = 0 To c
        '        '    If IsNumeric(arrTransCode(i)) Then
        '        '        L_ErrCode = oTrans.UpdateTranslation(lngCode, arrTransName(i), CInt(arrTransCode(i)), udtPrintProfile.Type, lngCodeSite, lngCodeUser, enumDataListType.PrintProfile)
        '        '        If L_ErrCode <> enumEgswErrorCode.OK Then Exit For
        '        '    End If
        '        'Next

        '    Catch ex As Exception
        '        L_ErrCode = enumEgswErrorCode.GeneralError
        '    End Try
        'End If

        If L_ErrCode = enumEgswErrorCode.OK Then
            t.Commit()
        Else
            t.Rollback()
        End If
        t.Dispose()
        If cn.State <> ConnectionState.Closed Then cn.Close()
        cn.Dispose()
        Return L_ErrCode

    End Function

    ''' <summary>
    ''' Updates PrintProfile and its Translations and share it to specified sites.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="udtPrintProfile">One of the structPrintProfile values.</param>
    ''' <param name="strCodeSiteList">The list of sites where PrintProfile will be shared.</param>
    ''' <param name="dtTranslations">The list of Translations of the PrintProfile.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal udtPrintProfile As structPrintProfile, _
        ByVal strCodeSiteList As String, ByVal dtTranslations As DataTable) As enumEgswErrorCode

        strCodeSiteList.Trim()
        If Not (strCodeSiteList.StartsWith("(") And strCodeSiteList.EndsWith(")")) And strCodeSiteList.Trim <> "" Then
            Return enumEgswErrorCode.InvalidCodeList
        End If

        Dim t As SqlTransaction
        Dim cn As New SqlConnection(L_strCnn)

        cn.Open()
        t = cn.BeginTransaction()
        L_ErrCode = SaveIntoList(lngCodeUser, lngCodeSite, lngCode, udtPrintProfile, strCodeSiteList, "", _
             CType(IIf(lngCode < 0, enumEgswTransactionMode.Add, enumEgswTransactionMode.Edit), enumEgswTransactionMode), _
             t)

        If L_ErrCode > 0 Then
            Try
                'Dim rowX As DataRow
                Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

                'For Each rowX In dtTranslations.Rows
                '    L_ErrCode = oTrans.UpdateTranslation(lngCode, rowX.Item("Name").ToString, CInt(rowX.Item("CodeTrans")), udtPrintProfile.Type, lngCodeSite, lngCodeUser, enumDataListType.PrintProfile)
                '    If L_ErrCode <> enumEgswErrorCode.OK Then Exit For
                'Next

            Catch ex As Exception
                L_ErrCode = enumEgswErrorCode.GeneralError
            End Try
        End If

        If L_ErrCode = enumEgswErrorCode.OK Then
            t.Commit()
        Else
            t.Rollback()
        End If
        t.Dispose()
        If cn.State <> ConnectionState.Closed Then cn.Close()
        cn.Dispose()
        Return L_ErrCode

    End Function

    ''' <summary>
    ''' Updatess PrintProfile's translations (multiple update)
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="eListeType">One of the enumDataListType values.</param>
    ''' <param name="dtTranslations">The list of Translations of the PrintProfile.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, _
        ByVal eListeType As enumDataListType, ByVal dtTranslations As DataTable) As enumEgswErrorCode

        Dim t As SqlTransaction
        Dim cn As New SqlConnection(L_strCnn)

        cn.Open()
        t = cn.BeginTransaction()
        Try
            Dim rowX As DataRow
            Dim oTrans As New clsTranslation(L_AppType, L_strCnn)

            For Each rowX In dtTranslations.Rows
                L_ErrCode = oTrans.UpdateTranslation(lngCode, rowX.Item("Name").ToString, CInt(rowX.Item("CodeTrans")), eListeType, lngCodeSite, lngCodeUser, enumDataListType.PrintProfile)
                If L_ErrCode <> enumEgswErrorCode.OK Then Exit For
            Next

            oTrans = Nothing
        Catch
            L_ErrCode = enumEgswErrorCode.GeneralError
        End Try

        If L_ErrCode = enumEgswErrorCode.OK Then
            t.Commit()
        Else
            t.Rollback()
        End If
        t.Dispose()
        If cn.State <> ConnectionState.Closed Then cn.Close()
        cn.Dispose()
        Return L_ErrCode

    End Function

    ''' <summary>
    ''' Updates a PrintProfile's translation (single update)
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="lngCode">The Code of the item to be updated.</param>
    ''' <param name="eListeType">One of the enumDataListType values.</param>
    ''' <param name="lngCodeTrans">The code of the PrintProfile's translation.</param>
    ''' <param name="strNameTrans">The name of the PrintProfile's translation.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByRef lngCode As Int32, ByVal eListeType As enumDataListType, _
        ByVal lngCodeTrans As Int32, ByVal strNameTrans As String) As enumEgswErrorCode

        Dim t As SqlTransaction
        Dim cn As New SqlConnection(L_strCnn)

        cn.Open()
        t = cn.BeginTransaction()
        Try
            Dim oTrans As New clsTranslation(L_AppType, L_strCnn)
            L_ErrCode = oTrans.UpdateTranslation(lngCode, strNameTrans, lngCodeTrans, eListeType, lngCodeSite, lngCodeUser, enumDataListType.PrintProfile)

            oTrans = Nothing
        Catch
            L_ErrCode = enumEgswErrorCode.GeneralError
        End Try

        If L_ErrCode = enumEgswErrorCode.OK Then
            t.Commit()
        Else
            t.Rollback()
        End If
        t.Dispose()
        If cn.State <> ConnectionState.Closed Then cn.Close()
        cn.Dispose()
        Return L_ErrCode

    End Function

    ''' <summary>
    ''' Merge PrintProfiles
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCodeSite">The CodeSite where the item will be updated and NOT the CodeSite of CodeUser.</param>
    ''' <param name="strCodePrintProfileList">The list of PrintProfile Codes to be merged.</param>
    ''' <param name="udtPrintProfile">PrintProfile info.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByVal strCodePrintProfileList As String, ByVal udtPrintProfile As structPrintProfile) As enumEgswErrorCode

        Return SaveIntoList(lngCodeUser, lngCodeSite, 0, udtPrintProfile, "", strCodePrintProfileList, enumEgswTransactionMode.MergeDelete)

    End Function

    ''' <summary>
    ''' Updates Status of the PrintProfiles specified in the list (strCodeList).
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="strCodeList">The list of PrintProfile Codes to be updated.</param>
    ''' <param name="bytStatus">The Status of the PrintProfile.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal strCodeList As String, ByVal bytStatus As Byte) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, -1, -1, False, enumEgswTransactionMode.Deactivate, bytStatus, strCodeList)

    End Function

    ''' <summary>
    ''' Updates Status of a PrintProfile.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the PrintProfile to be updated.</param>
    ''' <param name="IsGlobal">The Global Status of the PrintProfile.</param>
    ''' <param name="bytStatus">The Status of the PrintProfile.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Update(ByVal lngCodeUser As Int32, ByVal lngCode As Int32, ByVal IsGlobal As Boolean, _
        ByVal bytStatus As Byte) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, -1, lngCode, IsGlobal, enumEgswTransactionMode.Deactivate, bytStatus)

    End Function

    ''' <summary>
    ''' Updates the position of items.
    ''' </summary>
    ''' <param name="strCodeList">The list of item codes to be moved.</param>
    ''' <param name="flagMoveUp"></param>
    ''' <param name="lngCodeSite">The CodeSite of the items to be moved.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdatePosition(ByVal strCodeList As String, ByVal flagMoveUp As Boolean, _
        ByVal lngCodeSite As Int32) As enumEgswErrorCode

        Dim cmd As New SqlCommand

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswPrintProfileMovePos"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@vchCodeList", SqlDbType.VarChar, 8000).Value = strCodeList
                .Parameters.Add("@bitMoveUp", SqlDbType.TinyInt).Value = flagMoveUp
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite

                .Parameters("@retval").Direction = ParameterDirection.ReturnValue
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With

        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

    End Function

#End Region

#Region "Remove Methods"
    ''' <summary>
    ''' Purge PrintProfile List.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove() As enumEgswErrorCode

        Return RemoveFromList(L_udtUser.Code, -1, -1, False, enumEgswTransactionMode.Purge)

    End Function

    ''' <summary>
    ''' Deletes a PrintProfile.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="lngCode">The Code of the PrintProfile to be deleted.</param>
    ''' <param name="IsGlobal">The Global status of the PrintProfile to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, _
        ByVal lngCode As Int32, ByVal IsGlobal As Boolean) As enumEgswErrorCode

        Return RemoveFromList(lngCodeUser, lngCodeSite, lngCode, IsGlobal, enumEgswTransactionMode.Delete)

    End Function

    ''' <summary>
    ''' Deletes PrintProfiles specified in the list strCodeList.
    ''' </summary>
    ''' <param name="lngCodeUser">The Code of the current user.</param>
    ''' <param name="strCodeList">The list of PrintProfile Codes to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function Remove(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal strCodeList As String) As enumEgswErrorCode

        L_udtUser.Code = lngCodeUser
        Return RemoveFromList(lngCodeUser, lngCodeSite, 0, False, enumEgswTransactionMode.MultipleDelete, , strCodeList)

    End Function

#End Region

#Region " Other Function "

    Public Function GetOne(ByVal intCode As Integer, Optional ByVal intCodeTrans As Integer = -1) As DataRow
        Dim tempFetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataSet
        Dim ds As DataSet = CType(GetList(intCode), DataSet)
        L_bytFetchType = tempFetchType

        Dim dt As DataTable = ds.Tables(2)
        If dt.DefaultView.Count = 0 Then Return Nothing

        Dim rw As DataRow = dt.Rows(0)
        If intCodeTrans > -1 Then
            Dim dtTrans As DataTable = ds.Tables(1)
            Dim rwTrans As DataRow

            If dtTrans.Select("CodeTrans=" & CStr(intCodeTrans)).Length > 0 Then
                rwTrans = dtTrans.Select("CodeTrans=" & CStr(intCodeTrans))(0)
                If Len(Trim(CStr(rwTrans("translationname")))) > 0 Then rw("name") = CStr(rwTrans("translationname"))
            End If
        End If
        Return rw
    End Function

    '    Public Function GetCode(ByVal strName As String, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, Optional ByVal blnCommitToDbase As Boolean = False) As Integer
    '        Dim tempFetchType As enumEgswFetchType = L_bytFetchType
    '        L_bytFetchType = enumEgswFetchType.DataTable
    '        Dim dt As DataTable = CType(GetList(strName, enumDataListType.Merchandise, intCodeSite, intCodeTrans, 255), DataTable)
    '        L_bytFetchType = tempFetchType

    '        Dim intCode As Integer = -1
    '        Dim rw As DataRow = dt.Rows(0)

    '        If Not IsDBNull(rw("code")) Then intCode = CInt(dt.Rows(0)("Code"))
    '        If Not blnCommitToDbase Then GoTo Done

    '        If intCode > -1 Then
    '            If CInt(dt.Rows(0)("Status")) <> 1 Then 'inactive
    '                Dim cItem As clsItem = New clsItem(L_udtUser, enumAppType.WebApp, L_strCnn)
    '                cItem.UpdateStatus(intCode, intCodeSite, enumDbaseTables.EgswPrintProfile, 1)
    '            End If
    '        Else
    '            Dim PrintProfile As structPrintProfile
    '            PrintProfile.Type = enumDataListItemType.Merchandise
    '            PrintProfile.Code = intCode
    '            PrintProfile.Name = strName
    '            PrintProfile.IsGlobal = False

    '            Update(L_udtUser.Code, intCodeSite, intCode, PrintProfile)
    '        End If
    'Done:
    '        Return intCode
    '    End Function


    Public Function GenerateDefault(ByVal intCodeSite As Integer, _
        ByVal intPapersize As Integer, ByVal intPageHeight As Integer, ByVal intPageWidth As Integer, _
        ByVal intMarginUnit As Integer, ByVal dblTopMargin As Double, ByVal dblBottomMargin As Double, _
        ByVal strFont As String, ByVal dblFontSize As Double, ByVal dblLeftMargin As Double, ByVal dblRightMargin As Double, _
        ByVal dblLineSpacing As Double) As enumEgswErrorCode
        Dim arrParam(12) As SqlParameter

        arrParam(0) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@intPapersize", intPapersize)
        arrParam(2) = New SqlParameter("@intPageHeight", intPageHeight)
        arrParam(3) = New SqlParameter("@intPageWidth", intPageWidth)
        arrParam(4) = New SqlParameter("@intMarginUnit", intMarginUnit)
        arrParam(5) = New SqlParameter("@fltTopMargin", dblTopMargin)
        arrParam(6) = New SqlParameter("@fltBottomMargin", dblBottomMargin)
        arrParam(7) = New SqlParameter("@fltFontSize", dblFontSize)
        arrParam(8) = New SqlParameter("@fltLeftMargin", dblLeftMargin)
        arrParam(9) = New SqlParameter("@fltRightMargin", dblRightMargin)
        arrParam(10) = New SqlParameter("@fltLineSpacing", dblLineSpacing)
        arrParam(11) = New SqlParameter("@vchFont", strFont)
        arrParam(12) = New SqlParameter("@retval", SqlDbType.Int)
        arrParam(12).Direction = ParameterDirection.ReturnValue

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswPrintProfileGenerateDefault", arrParam)
            Return CType(arrParam(12).Value, enumEgswErrorCode)
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

#End Region

End Class

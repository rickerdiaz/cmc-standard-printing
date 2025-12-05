Imports System.Data.SqlClient

#Region "Class Header"
'Name               : clsRoles
'Decription         : Manages Roles Table
'Date Created       : 26.9.2005
'Author             : JRL
'Revision History   : 
'                       Accepts a connection string as opposed to a connection object. Class performs on a disconnected state.
'
#End Region
Public Class clsRoles
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
        '  ClearMarkings() 'items marked as not deleted
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


#Region "Get Methods"

    ''' <summary>
    ''' Returns TRUE if a particular right is granted to a specific module. User rights must be passed in Array.
    ''' </summary>
    ''' <param name="intModuleID"></param>
    ''' <param name="intFunction"></param>
    ''' <param name="arrRights"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Function CheckRoleExist(ByVal intModuleID As MenuType, ByVal intFunction As UserRightsFunction, ByVal arrRights As ArrayList) As Boolean
    '    If arrRights Is Nothing Then Return False
    '    CheckRoleExist = False


    '    If arrRights.Contains(intModuleID & "_" & intFunction) Then Return True

    'End Function

    Public Function CheckRoleExist(ByVal intModuleID As MenuType, ByVal intFunction As UserRightsFunction, ByVal dtRights As DataTable) As Boolean
        CheckRoleExist = False

        If dtRights Is Nothing Then Return False
        If dtRights.Select("[modules]=" & intModuleID & " AND [rights]=" & intFunction).Length > 0 Then Return True
    End Function


    ''' <summary>
    ''' Fetch List of All Menus Enum
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMenus() As SortedList
        Dim sl As New SortedList
        Dim value As MenuType
        Dim strkey As String
        Dim selVal As New MenuType
        For Each value In System.Enum.GetValues(GetType(MenuType))
            selVal = value
            strkey = selVal.ToString

            If Not sl.Contains(strkey) Then
                sl.Add(strkey, CInt(value))
            End If
        Next
        Return sl
    End Function

    ''' <summary>
    ''' Fetch List of All Rights
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUserRightsFunctions() As SortedList
        Dim sl As New SortedList
        Dim value As UserRightsFunction
        Dim selVal As New UserRightsFunction
        Dim strKey As String
        For Each value In System.Enum.GetValues(GetType(UserRightsFunction))
            selVal = value
            strKey = selVal.ToString
            strKey = strKey.Replace("allow", "")
            sl.Add(strKey, CInt(value))
        Next
        Return sl
    End Function

    ''' <summary>
    ''' Returns list of module rights of a role 
    ''' </summary>
    ''' <param name="intRole"></param>
    ''' <param name="intModule"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList(ByVal intRole As Integer, ByVal intModule As Integer) As Object
        Return Me.FetchList(intRole, intModule)
    End Function

    ''' <summary>
    ''' Return list of rights of a role
    ''' </summary>
    ''' <param name="intRole"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList(ByVal intRole As Integer) As Object
        Return Me.FetchList(intRole, -1)
    End Function


    ''' <summary>
    ''' get all roles
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList() As Object
        Dim strCommandText As String = "SELECT * FROM EgswRoles ORDER BY [position]"
        Try
            Select Case L_bytFetchType
                Case enumEgswFetchType.DataReader
                    Return ExecuteReader(L_strCnn, CommandType.Text, strCommandText)
                Case enumEgswFetchType.DataSet
                    Return ExecuteDataset(L_strCnn, CommandType.Text, strCommandText)
                Case enumEgswFetchType.DataTable
                    Return ExecuteDataset(L_strCnn, CommandType.Text, strCommandText).Tables(0)
            End Select

            Return Nothing
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetUserRole(ByVal intCodeUser As Integer) As DataTable
        Dim strCommandText As String = "SELECT * FROM EgswUserRoles WHERE CodeUser=@CodeUser"
        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeUser", intCodeUser)

        Try
            Return ExecuteDataset(L_strCnn, CommandType.Text, strCommandText, arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetRole(ByVal intCodeRole As Integer, intCodeTrans As Integer, intRoleLevel As Integer) As DataSet
        Dim strCommandText As String = "[sp_egswGetRole]"
        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@intCode", intCodeRole)
        arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans)
        arrParam(2) = New SqlParameter("@intRoleLevel", intRoleLevel)


        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
        'Dim strCommandText As String = "SELECT * FROM EgswRoles WHERE Code=@CodeRole"
        'Dim arrParam(0) As SqlParameter
        'arrParam(0) = New SqlParameter("@CodeRole", intCodeRole)

        'Try
        '    Return ExecuteDataset(L_strCnn, CommandType.Text, strCommandText, arrParam).Tables(0)
        'Catch ex As Exception
        '    Throw ex
        'End Try
    End Function
#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Fetch list of roles rights
    ''' </summary>
    ''' <param name="intRole"></param>
    ''' <param name="intModule"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function FetchList(ByVal intRole As Integer, ByVal intModule As Integer) As Object
        Dim strCommandText As String = "sp_EgswRoleRightsGetList"

        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@intRole", intRole)
        arrParam(1) = New SqlParameter("@intModule", intModule)

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


   
#End Region

#Region "Update Methods"
    ''' <summary>
    ''' Update Role Rights
    ''' </summary>
    ''' <param name="fnc"></param>
    ''' <param name="typeModule"></param>
    ''' <param name="intRole"></param>
    ''' <param name="IsAllowed"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateList(ByVal fnc As UserRightsFunction, ByVal typeModule As MenuType, ByVal intRole As Integer, ByVal IsAllowed As Boolean) As enumEgswErrorCode
        Try
            Dim arrParam(3) As SqlParameter
            arrParam(0) = New SqlParameter("@intFunction", fnc)
            arrParam(1) = New SqlParameter("@intModule", typeModule)
            arrParam(2) = New SqlParameter("@intRole", intRole)
            arrParam(3) = New SqlParameter("@IsAllowed", IsAllowed)
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswRolesRightsUpdate", arrParam)
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Throw ex
        End Try

    End Function

    Public Function SaveRole(ByRef intCode As Integer, ByVal strName As String, ByVal intRoleLevel As Integer, intCodeUser As Integer, Optional intTranMode As Integer = 1, Optional strCodeMergeList As String = "") As enumEgswErrorCode
        Try
            Dim arrParam(6) As SqlParameter
            arrParam(0) = New SqlParameter("@intCode", intCode)
            arrParam(0).Direction = ParameterDirection.InputOutput
            arrParam(1) = New SqlParameter("@nvcName", strName)
            arrParam(2) = New SqlParameter("@intRoleLevel", intRoleLevel)
            arrParam(3) = New SqlParameter("@intCodeUser", intCodeUser)
            arrParam(4) = New SqlParameter("@tntTranMode", intTranMode)
            arrParam(5) = New SqlParameter("@vchCodeMergeList", strCodeMergeList)
            arrParam(6) = New SqlParameter("@retVal", 0)
            arrParam(6).Direction = ParameterDirection.ReturnValue
            Dim intReturn As Integer

            intReturn = ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "[sp_EgswRoleUpdate]", arrParam)

            'If intReturn = 0 Then
            intCode = arrParam(0).Value
            L_ErrCode = arrParam(6).Value  ''RJL


            Select Case L_ErrCode
                Case enumEgswErrorCode.FK
                    Return enumEgswErrorCode.FK
                Case Else
                    Return enumEgswErrorCode.OK
            End Select
            'Return enumEgswErrorCode.OK
            'Else
            'Return intReturn
            'End If

        Catch ex As Exception
            Throw ex
        End Try

    End Function

#End Region

#Region "Remove Methods"

#End Region

End Class

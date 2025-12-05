Imports System.Data.SqlClient
Imports System.Data

#Region "Class Header"
'Name               : clsMark
'Decription         : Manages MarkGroup and MarkGroupDetails Tables
'Date Created       : 22.11.2005
'Author             : JRL
'Revision History   : VBV - 19.12.2005 - Add removing of mark details
'                     VBV - 20.12.2005 - Add counting of marked items
'
#End Region

''' <summary>
''' Manages Marking Table
''' </summary>
''' <remarks></remarks>

Public Class clsMark
#Region "Variable Declarations / Dependencies"
    Inherits clsDBRoutine

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
    'Public Sub New(ByVal eAppType As enumAppType, ByVal objCnn As SqlConnection, _
    '    ByVal strCnn As String, Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)
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
            If udtUser.Code <= 0 Then Throw New Exception("Invalid CodeUser.")
            L_udtUser = udtUser
            L_AppType = eAppType
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn
        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try

    End Sub

    Protected Overrides Sub Finalize()
        ClearMarkings(enumDataListItemType.Menu) 'items marked as not deleted
        ClearMarkings(enumDataListItemType.Recipe) 'items marked as not deleted
        ClearMarkings(enumDataListItemType.Merchandise) 'items marked as not deleted
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

    Public Property UserStruct() As structUser
        Get
            UserStruct = L_udtUser
        End Get
        Set(ByVal value As structUser)
            L_udtUser = UserStruct
            If L_udtUser.RoleLevelHighest < 0 Then Throw New Exception("User has an invalid RoleLevel.")
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


    Private Function ClearMarkings(ByVal dataListItemType As enumDataListItemType) As enumEgswErrorCode
        'Deactivate items that were not deleted by the Delete module
 
    End Function

#End Region

#Region "private methods"
    Private Function FetchList(ByVal intID As Integer, ByVal intCodeUser As Integer, ByVal typeItem As enumEgswTypeItems, ByVal picktype As enumEgswMarkType) As Object
        Dim strCommandText As String = "sp_EgswMarkGroupGetList"

        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@intID", intID)
        arrParam(1) = New SqlParameter("@intCodeUser", intCodeUser)
        arrParam(2) = New SqlParameter("@tntTypeitem", typeItem)
        arrParam(3) = New SqlParameter("@tntPickType", picktype)

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

#Region "Save methods"
    ''' <summary>
    ''' Save Mark Group List
    ''' </summary>
    ''' <param name="intID">Mark Group ID</param>
    ''' <param name="strName">Mark Group Name</param>
    ''' <param name="typeItem">Mark Group Type Item</param>
    ''' <param name="strTableName">Mark Group Details Table Name</param>
    ''' <param name="strCodesList">Codes to be saved or updated</param>
    ''' <param name="PickType">Mark Group Details Pick Type</param>
    ''' <param name="ClearExistingMarkDetails">Delete existing saved codes </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateMarkGroupList(ByRef intID As Integer, ByVal strName As String, ByVal typeItem As enumEgswTypeItems, ByVal strTableName As String, ByVal strCodesList As String, _
        ByVal PickType As enumEgswMarkType, Optional ByVal ClearExistingMarkDetails As Boolean = False, _
        Optional ByVal InsertMark As Boolean = True) As enumEgswErrorCode
        Try

            Dim intIDMain As Integer = -1
            If strCodesList.Length > 5000 Then
                Dim clListe As New clsListe(enumAppType.WebApp, L_strCnn)
                intIDMain = clListe.fctSaveToTempList(strCodesList, L_udtUser.Code)
            End If

            Dim arrParam(10) As SqlParameter
            arrParam(0) = New SqlParameter("@retVal", "")
            arrParam(0).Direction = ParameterDirection.ReturnValue
            arrParam(1) = New SqlParameter("@nvcName", strName)
            arrParam(2) = New SqlParameter("@intCodeUser", L_udtUser.Code)
            arrParam(3) = New SqlParameter("@tntTypeItem", typeItem)
            arrParam(4) = New SqlParameter("@intID", intID)
            arrParam(4).Direction = ParameterDirection.InputOutput
            arrParam(5) = New SqlParameter("@vchCodeList", strCodesList)
            arrParam(6) = New SqlParameter("@vchTableName", strTableName)
            arrParam(7) = New SqlParameter("@tntPickType", PickType)
            arrParam(8) = New SqlParameter("@ClearExistingMarkDetails", ClearExistingMarkDetails)
            arrParam(9) = New SqlParameter("@flagUpdateMark", InsertMark) 'vbv 19.12.2005
            arrParam(10) = New SqlParameter("@intIDMain", intIDMain) 'DLS 31.08.2007


            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswMarkGroupUpdate", arrParam)
            intID = CInt(arrParam(4).Value)
            Return CType(arrParam(0).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try

    End Function

    Public Function UpdateMarked(ByVal intCodeTable As enumDbaseTables, ByVal strCodesList As String, _
       Optional ByVal TranType As Integer = 1) As enumEgswErrorCode

        Try
            Dim arrParam(4) As SqlParameter
            arrParam(0) = New SqlParameter("@retVal", "")
            arrParam(0).Direction = ParameterDirection.ReturnValue
            arrParam(1) = New SqlParameter("@intCodeUser", L_udtUser.Code)
            arrParam(2) = New SqlParameter("@intCodeEgswTable", intCodeTable)
            arrParam(3) = New SqlParameter("@vchCodeList", strCodesList)
            arrParam(4) = New SqlParameter("@tntTranMode", TranType) 'vbv 19.12.2005

            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "EgswMarked_Update", arrParam)
            Return CType(arrParam(0).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try

    End Function
#End Region

#Region "Delete methods"
    ''' <summary>
    ''' Delete Mark Group and details associated to it
    ''' </summary>
    ''' <param name="strCodesList">List of IDs</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteFromList(ByVal strCodesList As String) As enumEgswErrorCode

        Dim intIDMain As Integer = -1
        If strCodesList.Length > 5000 Then
            Dim clListe As New clsListe(enumAppType.WebApp, L_strCnn)
            intIDMain = clListe.fctSaveToTempList(strCodesList, L_udtUser.Code)
        End If

        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@retval", "")
        arrParam(0).Direction = ParameterDirection.ReturnValue
        arrParam(1) = New SqlParameter("@vchCodeList", strCodesList)
        arrParam(2) = New SqlParameter("@intIDMain", intIDMain) 'DLS 31.08.2007

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswMarkGroupDelete", arrParam)
            Return CType(arrParam(0).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try
    End Function
#End Region

#Region "Get methods"
    ''' <summary>
    ''' Returns list of mark groups of the current user
    ''' </summary>
    ''' <param name="typeitem"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMarkGroupList(ByVal typeitem As enumEgswTypeItems, Optional ByVal pickType As enumEgswMarkType = enumEgswMarkType.CurrentlySelected) As Object
        Return Me.FetchList(-1, L_udtUser.Code, typeitem, pickType)
    End Function

    ''' <summary>
    ''' Return list of code belonging to the current Mark Group ID
    ''' </summary>
    ''' <param name="intID"></param>
    ''' <param name="pickType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMarkGroupDetailsList(ByVal intID As Integer, ByVal pickType As enumEgswMarkType) As Object
        Return Me.FetchList(intID, -1, enumEgswTypeItems.NoType, pickType)
    End Function

    ''' <summary>
    ''' Returns the number of items currently marked per site.
    ''' </summary>
    ''' <param name="intCodeSite">The code of the site where items will be retrieved.</param>
    ''' <param name="ETable">One of enumDbaseTables values.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCountMarkedItem(ByVal intCodeSite As Int32, ByVal ETable As enumDbaseTables) As Int32
        'vbv 20.12.2005
        Dim cnt As Int32 = 0

        Try
            Dim cmdX As New SqlCommand("ITEM_CountMarked", New SqlConnection(L_strCnn))

            With cmdX
                .CommandTimeout = 60
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = L_udtUser.Code
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@intCodeEgsTable", SqlDbType.Int).Value = ETable
                .Connection.Open()
                cnt = CInt(cmdX.ExecuteScalar)
            End With

            If cmdX.Connection.State <> ConnectionState.Closed Then cmdX.Connection.Close()
            cmdX.Dispose()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        Return cnt
    End Function
    Public Function GetMarkedList(ByVal intcodetable As enumDbaseTables) As Object
        Dim strCommandText As String = "EgswMarked_GetList"

        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeUser", L_udtUser.Code)
        arrParam(1) = New SqlParameter("@intCodeEgswTable", intcodetable)

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


End Class
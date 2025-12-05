Imports System.Data.SqlClient
Imports System.Data

#Region "Class Header"
'Name               : clsItem
'Decription         : Generic Functions for Basic List Tables
'Date Created       : 20.10.2005
'Author             : JRL
'Revision History   : 
'                   :
'                     
#End Region

''' <summary>
''' Manages basic list Table
''' </summary>
''' <remarks></remarks>
Public Class clsItem


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
#End Region



#Region "Update Methods"

    ''' <summary>
    ''' update staus of the item
    ''' </summary>
    ''' <param name="intCode">will be used if no strCodeList supplied</param>
    ''' <param name="intCodeSite">site owner of the item</param>
    ''' <param name="intCodeEgswTable">table enum</param>
    ''' <param name="tntStatus">status, active/deactive</param>
    ''' <param name="strCodeList">if this is supplied, it will be used</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateStatus(ByVal intCode As Integer, ByVal intCodeSite As Integer, _
        ByVal intCodeEgswTable As enumDbaseTables, ByVal tntStatus As Int16, Optional ByVal intcodeproperty As Integer = -1, Optional ByVal strCodeList As String = "") As enumEgswErrorCode
        Dim arrParam(6) As SqlParameter

        arrParam(0) = New SqlParameter("@intCode", intCode)
        arrParam(1) = New SqlParameter("@intCodeEgsTable", intCodeEgswTable)
        arrParam(2) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(3) = New SqlParameter("@bytStatus", tntStatus)
        arrParam(4) = New SqlParameter("@vchCodeList", SqlDbType.VarChar, 1000)
        arrParam(5) = New SqlParameter("@retval", SqlDbType.Int)
        arrParam(6) = New SqlParameter("@intCodeProperty", intcodeproperty)

        arrParam(4).Value = strCodeList
        arrParam(5).Direction = ParameterDirection.ReturnValue

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswItemStatusUpdate", arrParam)
            Return CType(arrParam(5).Value, enumEgswErrorCode)
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
            Throw ex
        End Try
    End Function

#End Region

#Region "Private Methods"

#End Region

#Region "Get Methods"

    Public Overloads Function GetList(ByVal strCodeSites As String, ByVal strTable As String, ByVal type As enumDataListItemType) As Object
        Dim strCommandText As String = "sp_EgswItemGetListActive"

        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@vchCodeSiteList", strCodeSites)
        arrParam(1) = New SqlParameter("@vchTable", strTable)
        arrParam(2) = New SqlParameter("@intCodeTrans", L_udtUser.CodeTrans)
        arrParam(3) = New SqlParameter("@tntType", type)

        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Overloads Function GetList(ByVal strCodeSites As String, ByVal strTable As String, ByVal intCodeTrans As Integer, ByVal type As enumDataListItemType, Optional ByVal intStatus As Integer = 1) As Object
        Dim strCommandText As String = "sp_EgswItemGetMultiSite"

        Dim arrParam(4) As SqlParameter
        arrParam(0) = New SqlParameter("@vchCodeSiteList", strCodeSites)
        arrParam(1) = New SqlParameter("@vchTableName", strTable)
        arrParam(2) = New SqlParameter("@intCodeTrans", L_udtUser.CodeTrans)
        arrParam(3) = New SqlParameter("@intListeType", type)
        arrParam(4) = New SqlParameter("@intStatus", intStatus)

        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListCategory(ByVal intCodeTrans As Integer, ByVal type As enumDataListItemType) As Object
        Dim strCommandText As String = "[sp_EgswItemGetCategoryForProd]"

        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeTrans", L_udtUser.CodeTrans)
        arrParam(1) = New SqlParameter("@intListeType", type)

        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function


    'MRC-08.11.08-Added optional param intSource=-1, get categ by source, for Ducasse customization.
    Public Function GetListCategoryCodeName(ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, ByVal type As enumDataListItemType, Optional ByVal flagActiveOnly As Boolean = True, _
        Optional ByVal intSource As Integer = -1) As Object
        Dim strCommandText As String = "[GET_CATEGORYCODENAME]"

        '@ListeType int,
        '@CodeSite int,
        '@CodeTrans int,
        '@ActiveOnly bit =1
        '@Source int=-1

        Dim arrParam(4) As SqlParameter
        arrParam(0) = New SqlParameter("@ListeType", type)
        arrParam(1) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(2) = New SqlParameter("@CodeTrans", L_udtUser.CodeTrans)
        arrParam(3) = New SqlParameter("@ActiveOnly", flagActiveOnly)
        arrParam(4) = New SqlParameter("@Source", intSource)
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function


    Public Function GetListKeywordCodeName(ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, ByVal type As enumDataListItemType, Optional ByVal flagActiveOnly As Boolean = True) As Object
        Dim strCommandText As String = "[GET_KEYWORDCODENAME]"

        '@ListeType int,
        '@CodeSite int,
        '@CodeTrans int,
        '@ActiveOnly bit =1

        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@ListeType", type)
        arrParam(1) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(2) = New SqlParameter("@CodeTrans", L_udtUser.CodeTrans)
        arrParam(3) = New SqlParameter("@ActiveOnly", flagActiveOnly)

        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function


    Public Function GetListSupplierCodeName(ByVal intCodeSite As Integer, Optional ByVal flagActiveOnly As Boolean = True) As Object
        Dim strCommandText As String = "[GET_SupplierCODENAME]"

        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@ActiveOnly", flagActiveOnly)
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function


    Public Function GetListSourceCodeName(ByVal intCodeSite As Integer, Optional ByVal flagActiveOnly As Boolean = True) As Object
        Dim strCommandText As String = "[GET_SOURCECODENAME]"

        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@ActiveOnly", flagActiveOnly)
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListLocationCodeName(ByVal intCodeSite As Integer, Optional ByVal flagActiveOnly As Boolean = True) As Object
        Dim strCommandText As String = "[GET_LOCATIONCODENAME]"

        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@ActiveOnly", flagActiveOnly)
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

	Public Function GetListPublicationCodeName(ByVal intCodeSite As Integer, Optional ByVal flagActiveOnly As Boolean = True) As Object
		Dim strCommandText As String = "[GET_PUBLICATIONCODENAME]"

		Dim arrParam(1) As SqlParameter
		arrParam(0) = New SqlParameter("@CodeSite", intCodeSite)
		arrParam(1) = New SqlParameter("@ActiveOnly", flagActiveOnly)
		Try
			Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
		Catch ex As Exception
			Throw ex
		End Try
	End Function

    Public Function GetListTaxCodeValueNameReader(ByVal intCodeSite As Integer, Optional ByVal flagActiveOnly As Boolean = True) As Object
        Dim strCommandText As String = "[GET_TAXCODEValueDesc]"

        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@ActiveOnly", flagActiveOnly)
        Try
            Return ExecuteFetchType(enumEgswFetchType.DataReader, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListBrandCodeName(ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, Optional ByVal flagActiveOnly As Boolean = True) As Object
        Dim strCommandText As String = "[GET_BRANDCODENAME]"

        '@ListeType int,
        '@CodeSite int,
        '@CodeTrans int,
        '@ActiveOnly bit =1

        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@CodeTrans", L_udtUser.CodeTrans)
        arrParam(2) = New SqlParameter("@ActiveOnly", flagActiveOnly)

        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListTranslationsCodeValueNameReader(ByVal intCodeSite As Integer, ByVal bytStatus As Byte) As Object
        Dim strCommandText As String = "GET_TRANSLATIONCODENAME"

        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@Status", bytStatus)

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

    Public Function GetKeywordProjectRecipe(ByVal intCodeKey As Integer, ByVal intCodeTrans As Integer, ByVal intCodeSite As Integer, ByVal intCodeUser As Integer) As Object
        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeKey", intCodeKey)
        arrParam(1) = New SqlParameter("@CodeTrans", intCodeTrans)
        arrParam(2) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(3) = New SqlParameter("@CodeUser", intCodeUser)
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "GET_PROJECTRECIPElIST", arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListCookBook(ByVal intCodeTrans As Integer, ByVal intCodeSite As Integer, ByVal intCodeUser As Integer) As Object
        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@CodeTrans", intCodeTrans)
        arrParam(2) = New SqlParameter("@CodeUser", intCodeUser)
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "API_GET_ProjectList", arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    'Public Function GetStatusList(ByVal intType As Integer) As Object
    '    Dim arrParam(0) As SqlParameter
    '    arrParam(0) = New SqlParameter("@Type", intType)

    '    Try
    '        Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "GET_EGSWSTATUSList", arrParam)
    '    Catch ex As Exception

    '    End Try

    'End Function

    Public Function GetStatusList(ByVal intType As Integer, intCodeTrans As Integer) As Object
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@Type", intType)
        arrParam(1) = New SqlParameter("@CodeTrans", intCodeTrans)
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "GET_EGSWSTATUSList", arrParam)
        Catch ex As Exception

        End Try

    End Function

#End Region

#Region "Delete Methods"

#End Region



End Class

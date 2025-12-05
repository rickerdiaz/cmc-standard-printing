Imports System.Data.SqlClient
Imports System.Data

#Region "Class Header"
'Name               : clsProperty
'Decription         : Manages Property Table
'Date Created       : 26.9.2005
'Author             : JRL
'Revision History   : 
'                       Accepts a connection string as opposed to a connection object. Class performs on a disconnected state.
'
#End Region

Public Class clsProperty
    Inherits clsDBRoutine
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

#Region "Get Methods"
    ''' <summary>
    ''' Fetch list of properties
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList() As Object
        Return FetchPropertyList(-1)
    End Function

    ''' <summary>
    ''' Fetch a Property
    ''' </summary>
    ''' <param name="intCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList(ByVal intCode As Integer) As Object
        Return FetchPropertyList(intCode)
    End Function

    Public Function GetPropertyCodeName(ByVal intexCodeProperty As Integer) As Object
        Dim strCommandText As String = "GET_PROPERTYCODENAME"

        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@ExCludeCodeProperty", intexCodeProperty)

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
#Region "Save Methods"
    Public Function UpdateProperty(ByVal intCode As Integer, ByVal strName As String, ByVal strCodeSiteList As String, ByVal tranMode As enumEgswTransactionMode) As enumEgswErrorCode
        Try
            Dim arrParam(4) As SqlParameter
            arrParam(0) = New SqlParameter("@retval", "")
            arrParam(0).Direction = ParameterDirection.ReturnValue
            arrParam(1) = New SqlParameter("@intCode", intCode)
            arrParam(2) = New SqlParameter("@nvcName", strName)
            arrParam(3) = New SqlParameter("@txtCodeSiteList", strCodeSiteList)
            arrParam(4) = New SqlParameter("@tntTranMode", tranMode)
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswPropertyUpdate", arrParam)
            Return CType(arrParam(0).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try
    End Function
#End Region
#Region "Remove Methods"
    ''' <summary>
    ''' Delete one item from the list
    ''' </summary>
    ''' <param name="intCode">Code of the property. </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteFromList(ByVal intCode As Integer) As enumEgswErrorCode
        Return DeleteFromList(intCode, "", enumEgswTransactionMode.Delete)
    End Function

#End Region
#Region "Private Methods"
    ''' <summary>
    ''' Get list of properties
    ''' </summary>
    ''' <param name="intCodeProperty"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function FetchPropertyList(ByVal intCodeProperty As Integer) As Object
        Dim strCommandText As String = "sp_EgswPropertyGetList"

        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeProperty", intCodeProperty)

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

    ''' <summary>
    ''' Delete property
    ''' </summary>
    ''' <param name="intCode"></param>
    ''' <param name="strCodeList">JRL: Not yet working.</param>
    ''' <param name="TrandMode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function DeleteFromList(ByVal intCode As Integer, ByVal strCodeList As String, ByVal TrandMode As enumEgswTransactionMode) As enumEgswErrorCode

        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@retval", "")
        arrParam(0).Direction = ParameterDirection.ReturnValue
        arrParam(1) = New SqlParameter("@intCode", intCode)
        arrParam(2) = New SqlParameter("@tntTranMode", TrandMode)
        arrParam(3) = New SqlParameter("@txtCodeList", strCodeList)

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswPropertyDelete", arrParam)
            Return CType(arrParam(0).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try
    End Function
#End Region

End Class

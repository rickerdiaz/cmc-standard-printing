Imports System.Data.SqlClient
Imports System.Data

#Region "Class Header"
'Name               : clsCurrency
'Decription         : Manages Currency
'Date Created       : 28.09.2005
'Author             : JRL
'Revision History   : 
'
#End Region

Public Class clsCurrency
    Inherits clsDBRoutine

    Private L_Cnn As SqlConnection
    'Private L_Cmd As New SqlCommand
    Private L_ErrCode As enumEgswErrorCode

    'Properties
    Private L_AppType As enumAppType
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType

#Region "Class Functions and Properties"
    Public Sub New(ByVal eAppType As enumAppType, ByVal strCnn As String, Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)

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
            L_strCnn = strCnn
            L_bytFetchType = bytFetchType

        Catch ex As Exception
            Throw New Exception("Error initializing object", ex)
        End Try

    End Sub

    Public ReadOnly Property ConnectionString() As String
        Get
            ConnectionString = L_strCnn
        End Get
    End Property

#End Region

#Region "Update Methods"
    ''' <summary>
    ''' Update Currency
    ''' </summary>
    ''' <param name="intCode"></param>
    ''' <param name="strSymbole"></param>
    ''' <param name="strDescription"></param>
    ''' <param name="strFormat"></param>
    ''' <param name="Status"></param>
    ''' <param name="TranMode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateCurrency(ByVal intCode As Integer, ByVal strSymbole As String, ByVal strDescription As String, ByVal strFormat As String, ByVal Status As Byte, ByVal TranMode As enumEgswTransactionMode, ByVal intUserCode As Integer, ByVal strSign As String) As enumEgswErrorCode
        Try
            Dim arrParam(7) As SqlParameter
            arrParam(0) = New SqlParameter("@intCode", intCode)
            arrParam(1) = New SqlParameter("@Status", Status)
            arrParam(2) = New SqlParameter("@tntTranMode", TranMode)
            arrParam(3) = New SqlParameter("@nvcDescription", strDescription)
            arrParam(4) = New SqlParameter("@nvcFormat", strFormat)
            arrParam(5) = New SqlParameter("@ncharSymbole", strSymbole)
            arrParam(6) = New SqlParameter("@intCodeUser", intUserCode)
            arrParam(7) = New SqlParameter("@nvcSign", strSign)

            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswCurrencyUpdate", arrParam)
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''' <summary>
    ''' Update Currency Rate
    ''' </summary>
    ''' <param name="intCodeCur1"></param>
    ''' <param name="intCodeCur2"></param>
    ''' <param name="dblRate"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function UpdateCurrency(ByVal intCodeCur1 As Integer, ByVal intCodeCur2 As Integer, ByVal dblRate As Decimal) As enumEgswErrorCode
        Try
            Dim arrParam(2) As SqlParameter
            arrParam(0) = New SqlParameter("@intCodeCur1", intCodeCur1)
            arrParam(1) = New SqlParameter("@intCodeCur2", intCodeCur2)
            arrParam(2) = New SqlParameter("@fltRate", dblRate)

            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswCurrencyRateUpdate", arrParam)
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Throw ex
        End Try
    End Function

#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Fetch list of currencies
    ''' </summary>
    ''' <param name="intCode"></param>
    ''' <param name="bytStatus"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function FetchList(ByVal intCode As Integer, ByVal bytStatus As Byte, Optional ByVal strName As String = "") As Object
        Dim strCommandText As String = "sp_EgswCurrencyGetList"

        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@intCode", intCode)
        arrParam(1) = New SqlParameter("@Status", bytStatus)
        arrParam(2) = New SqlParameter("@nvcName", strName)

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
    ''' Get Currency rate conversion  from database
    ''' </summary>
    ''' <param name="intCode1"></param>
    ''' <param name="intCode2"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' Changed 
    Private Function FetchListRate(ByVal intCode1 As Integer, ByVal intCode2 As Integer) As Decimal
        Dim strCommandText As String = "sp_EgswCurrencyRateGetList"
        Dim currencyRate As Decimal = 0

        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@intCode1", intCode1)
        arrParam(1) = New SqlParameter("@intCode2", intCode2)
        'arrParam(2) = New SqlParameter("@fltRate", intCode2        ' RSDC 02.07.2013 (Removed)
        arrParam(2) = New SqlParameter("@fltRate", currencyRate)    ' RSDC 02.07.2013 (Update)
        arrParam(2).Direction = ParameterDirection.Output
        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
            Return CDec(arrParam(2).Value)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

#End Region

#Region "Get Methods"
    ''' <summary>
    ''' Get one currency 
    ''' </summary>
    ''' <param name="intCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList(ByVal intCode As Integer) As Object
        Return Me.FetchList(intCode, 255)
    End Function

    ''' <summary>
    ''' Get one currency 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList(ByVal strSymbole As String) As Object
        Return Me.FetchList(-1, 255, strSymbole)
    End Function

    ''' <summary>
    ''' Get List of Currencies
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList() As Object
        Return Me.FetchList(-1, 255)
    End Function
    ''' <summary>
    ''' Get currency exchange rate
    ''' </summary>
    ''' <param name="intCodeCur1"></param>
    ''' <param name="intCodeCur2"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList(ByVal intCodeCur1 As Integer, ByVal intCodeCur2 As Integer) As Double
        Return Me.FetchListRate(intCodeCur1, intCodeCur2)
    End Function


    ''' <summary>
    ''' Get list of Currencies. Filter by Status.
    ''' </summary>
    ''' <param name="intCode"></param>
    ''' <param name="bytStatus"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList(ByVal intCode As Integer, ByVal bytStatus As Byte) As Object
        Return Me.FetchList(intCode, bytStatus)
    End Function



#End Region

End Class

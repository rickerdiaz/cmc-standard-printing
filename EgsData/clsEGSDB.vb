Imports System.Data.SqlClient

Public Class clsEGSDB
    Inherits clsDBRoutine

    Private L_AppType As enumAppType
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType

    Public Sub New(ByVal eAppType As enumAppType, ByVal strCnn As String, _
       Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)
        L_AppType = eAppType
        L_bytFetchType = bytFetchType
        L_strCnn = strCnn
    End Sub

    Public Function GetUserInfo(ByVal intContactID As Integer) As Object
        Dim strCommandText As String = "sp_EgsGetContactDetailsByCode"
        Dim arrParam() As SqlParameter = {New SqlParameter("@p_intCode", intContactID)}

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

    Public Function GetUserInfoByID(ByVal intCodeEGS As Integer) As Object
        Dim strCommandText As String = "SELECT UserName, Password FROM EgsContact WHERE Code=@CodeEGS"
        Dim arrParam() As SqlParameter = {New SqlParameter("@CodeEGS", SqlDbType.Int)}
        arrParam(0).Value = intCodeEGS

        Try
            Select Case L_bytFetchType
                Case enumEgswFetchType.DataReader
                    Return ExecuteReader(L_strCnn, CommandType.Text, strCommandText, arrParam)
                Case enumEgswFetchType.DataSet
                    Return ExecuteDataset(L_strCnn, CommandType.Text, strCommandText, arrParam)
                Case enumEgswFetchType.DataTable
                    Return ExecuteDataset(L_strCnn, CommandType.Text, strCommandText, arrParam).Tables(0)
            End Select

            Return Nothing
        Catch ex As Exception
            Throw ex
        End Try
    End Function



    Public Function GetCodeEGSBySession(ByVal strSessionID As String) As Integer
        Dim intX As Integer = 0
        Dim strSQL As String = "SELECT Code FROM EgsContact WHERE SessionID=@strSessionID"
        Dim cmd As New SqlCommand
        Dim cn As New SqlConnection(L_strCnn)
        With cmd
            .Connection = cn
            .CommandType = CommandType.Text
            .Parameters.Add("@strSessionID", SqlDbType.VarChar, 35).Value = strSessionID
            .CommandText = strSQL
            .Connection.Open()
            intX = fctNullToZero(.ExecuteScalar)
            .Connection.Close()
            .Dispose()
        End With
        Return intX
    End Function

    Public Function ValidateLogin(ByVal strUserName As String, ByVal strPassword As String) As Integer
        Dim qry As String
        qry = "SELECT Code, [Level], FirstName, LastName, Email, Username, Password, ConfirmedReg "
        qry &= "FROM EgsContact WHERE "
        qry &= "REPLACE(Username,'-','')=Replace('" & Replace(strUserName, "'", "''") & "', '-', '') "
        qry &= "AND Password='" & Replace(strPassword, "'", "''") & "'"

        Dim dr As SqlDataReader = ExecuteReader(L_strCnn, CommandType.Text, qry)
        If dr.Read Then
            ValidateLogin = CInt(dr("Code"))
        Else
            ValidateLogin = 0
        End If
        dr.Close()
    End Function

    Public Function UpdateContactCMOnlineFileImported(ByVal intcodeEGS As Integer, ByVal shrtFileImported As Short) As enumEgswErrorCode
        Return SaveContactCMOnline(intcodeEGS, False, Now.Date, enumEgswTransactionMode.ModifyStatus, shrtFileImported)
    End Function

    Public Function UpdateContactCMOnline(ByVal intCodeEGS As Integer, _
        ByVal blnstatus As Boolean, ByVal dtmValidityDate As Date) As enumEgswErrorCode
        Return SaveContactCMOnline(intCodeEGS, blnstatus, dtmValidityDate, enumEgswTransactionMode.Add, 0)
    End Function

    Private Function SaveContactCMOnline(ByVal intCodeEGS As Integer, _
        ByVal blnstatus As Boolean, ByVal dtmValidityDate As Date, _
        ByVal tntTranMode As enumEgswTransactionMode, ByVal shrtFileImported As Short) As enumEgswErrorCode
        Dim arrParam(5) As SqlParameter

        arrParam(0) = New SqlParameter("@intCodeEGS", intCodeEGS)
        arrParam(1) = New SqlParameter("@bitStatus", blnstatus)
        arrParam(2) = New SqlParameter("@dtmValidityDate", dtmValidityDate)
        arrParam(3) = New SqlParameter("@tntTranMode", tntTranMode)
        arrParam(4) = New SqlParameter("@tntFileImported", shrtFileImported)
        arrParam(5) = New SqlParameter("@retval", SqlDbType.Int)
        arrParam(5).Direction = ParameterDirection.ReturnValue

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgsContactCMOnlineUpdate", arrParam)
            Return CType(arrParam(5).Value, enumEgswErrorCode)
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function InsertContactCMOnlineLog(ByVal intCodeEGS As Integer, _
        ByVal dtmDateRequest As Date, ByVal strCodePromo As String, ByVal intCodePayment As Integer, _
        ByVal dblCodePaymentAmount As Double, ByVal intCodePaymentDays As Integer, _
        ByVal intPaymentType As Integer, ByVal dblamountPaid As Double, ByRef intID As Integer) As enumEgswErrorCode

        Return SaveContactCMOnlineLog(intCodeEGS, dtmDateRequest, _
            strCodePromo, intCodePayment, dblCodePaymentAmount, intCodePaymentDays, _
            intPaymentType, DBNull.Value, 0, dblamountPaid, intID, enumEgswTransactionMode.Add)
    End Function

    Public Function UpdateContactCMOnlineLogStatus(ByVal intID As Integer, ByVal shrtStatus As Short, ByVal dtmDateStart As Date) As enumEgswErrorCode
        Return SaveContactCMOnlineLog(-1, DBNull.Value, "", 0, _
            0, 0, 0, dtmDateStart, shrtStatus, 0, intID, enumEgswTransactionMode.ModifyStatus)
    End Function

    Private Function SaveContactCMOnlineLog(ByVal intCodeEGS As Integer, _
        ByVal dtmDateRequest As Object, ByVal strCodePromo As String, _
        ByVal intCodePayment As Integer, ByVal dblCodePaymentAmount As Double, ByVal intCodePaymentDays As Integer, _
        ByVal intPaymentType As Integer, _
        ByVal dtmDateStart As Object, ByVal shrtStatus As Short, ByVal dblAmountPaid As Double, ByRef intID As Integer, _
        ByVal tranMode As enumEgswTransactionMode) As enumEgswErrorCode
        Dim arrParam(12) As SqlParameter

        arrParam(0) = New SqlParameter("@intCodeEGS", intCodeEGS)
        arrParam(1) = New SqlParameter("@dtmDateRequest", dtmDateRequest)
        arrParam(2) = New SqlParameter("@fltAmountPaid", dblAmountPaid)
        arrParam(3) = New SqlParameter("@nvcCodePromo", strCodePromo)
        arrParam(4) = New SqlParameter("@intCodePayment", intCodePayment)
        arrParam(5) = New SqlParameter("@fltCodePaymentAmount", dblCodePaymentAmount)
        arrParam(6) = New SqlParameter("@intCodePaymentDays", intCodePaymentDays)
        arrParam(7) = New SqlParameter("@intPaymentType", intPaymentType)

        arrParam(8) = New SqlParameter("@dtmDateStart", dtmDateStart)
        arrParam(9) = New SqlParameter("@tntStatus", shrtStatus)

        arrParam(10) = New SqlParameter("@tntTranMode", tranMode)
        arrParam(11) = New SqlParameter("@intID", intID)
        arrParam(12) = New SqlParameter("@retval", SqlDbType.Int)

        arrParam(11).Direction = ParameterDirection.InputOutput
        arrParam(12).Direction = ParameterDirection.ReturnValue

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgsContactCMOnlineLogUpdate", arrParam)
            intID = CInt(arrParam(11).Value)
            Return CType(arrParam(12).Value, enumEgswErrorCode)
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function GetContactCMOnlineLog(ByVal intLogId As Integer) As Object
        Return ListContactCMOnlineLog(0, False, intLogId)
    End Function

    Public Function GetContactCMOnlineLog(ByVal intCodeEgsID As Integer, ByVal blnIncludePending As Boolean, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        If fetchType <> enumEgswFetchType.UseDefault Then L_bytFetchType = fetchType
        Return ListContactCMOnlineLog(intCodeEgsID, blnIncludePending, 0)
    End Function

    Private Function ListContactCMOnlineLog(ByVal intCodeEgsID As Integer, Optional ByVal blnIncludePending As Boolean = False, Optional ByVal intLogID As Integer = 0) As Object
        Dim strCommandText As String = "sp_EgsContactCMOnlineLogGetList"
        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeEGS", intCodeEgsID)
        arrParam(1) = New SqlParameter("@bitIncludePending", blnIncludePending)
        arrParam(2) = New SqlParameter("@intID", intLogID)
        arrParam(3) = New SqlParameter("@dtmCurrent", Now.Date)

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

    Public Function GetContactCMOnlinePayment(Optional ByVal intcodePayment As Integer = -1, Optional ByVal intCodeEGS As Integer = -1) As Object
        Dim strCommandText As String = "sp_EgsContactCMOnlinePaymentGetList"
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodePayment", intcodePayment)
        arrParam(1) = New SqlParameter("@intCodeEGS", intCodeEGS)
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

    'Public Function GetContactCMOnlinePayment(Optional ByVal intcodePayment As Integer = -1, Optional ByVal intCodeEGS As Integer = -1) As Object
    '    Dim strSQL As String
    '    strSQL &= " IF @intCodePayment=-1 AND @intCodeEGS=-1"
    '    strSQL &= " SELECT * "
    '    strSQL &= " FROM EgsContactCMOnlinePayment"
    '    strSQL &= " ELSE IF @intCodePayment=-1 AND @intCodeEGS<>-1"
    '    strSQL &= " IF EXISTS(SELECT [ID] FROM EgsContactCMOnlineLog "
    '    strSQL &= " WHERE CodeEGS=@intCodeEGS AND Status=1) "
    '    strSQL &= " BEGIN"
    '    strSQL &= " SELECT *"
    '    strSQL &= " FROM EgsContactCMOnlinePayment"
    '    strSQL &= " WHERE codePayment <> 1"
    '    strSQL &= " End"
    '    strSQL &= " ELSE"
    '    strSQL &= " BEGIN"
    '    strSQL &= " SELECT *"
    '    strSQL &= " FROM EgsContactCMOnlinePayment"
    '    strSQL &= " End"
    '    strSQL &= " ELSE IF @intCodePayment<>-1 AND @intCodeEGS=-1"
    '    strSQL &= " SELECT *"
    '    strSQL &= " FROM EgsContactCMOnlinePayment"
    '    strSQL &= " WHERE codePayment=@intCodePayment"

    '    Dim arrParam(1) As SqlParameter
    '    arrParam(0) = New SqlParameter("@intCodePayment", intcodePayment)
    '    arrParam(1) = New SqlParameter("@intCodeEGS", intCodeEGS)
    '    Try
    '        Select Case L_bytFetchType
    '            Case enumEgswFetchType.DataReader
    '                Return ExecuteReader(L_strCnn, CommandType.Text, strSQL, arrParam)
    '            Case enumEgswFetchType.DataSet
    '                Return ExecuteDataset(L_strCnn, CommandType.Text, strSQL, arrParam)
    '            Case enumEgswFetchType.DataTable
    '                Return ExecuteDataset(L_strCnn, CommandType.Text, strSQL, arrParam).Tables(0)
    '        End Select
    '        Return Nothing
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Function


    Public Function GetContactCMOnlinePromo(Optional ByVal strPromoCode As String = "") As Object
        Dim strCommandText As String = "sp_EgsContactCMOnlinePromoGetList"
        Dim arrParam() As SqlParameter = {New SqlParameter("@nvcCodePromo", strPromoCode)}
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

    Public Function DetermineIfPaypalAllowed(ByVal intCountryCode As Integer) As Boolean
        Dim sqlConn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand
        sqlCmd.Connection = sqlConn
        sqlCmd.Connection.Open()

        sqlCmd.CommandText = "SELECT Paypal FROM CountryList WHERE Code=@c and Lang=@l"
        sqlCmd.CommandType = CommandType.Text
        sqlCmd.Parameters.Add("@c", SqlDbType.Int, 4).Value = intCountryCode
        sqlCmd.Parameters.Add("@l", SqlDbType.Int, 4).Value = 1
        Dim dr As SqlDataReader = sqlCmd.ExecuteReader

        dr.Read()
        Dim ppal As Boolean = CBool(dr.Item("Paypal"))
        dr.Close()

        sqlCmd.Connection.Close()
        sqlCmd.Dispose()
        sqlConn.Close()
        Return ppal
    End Function

    Public Function GetCountryCode(ByVal abbr As String) As Integer
        'DLS June152006
        Dim nCode As Integer
        Dim sqlConn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand
        sqlCmd.Connection = sqlConn
        sqlCmd.Connection.Open()

        sqlCmd.CommandText = "SELECT top 1 Code FROM CountryList WHERE abbr=@c "
        sqlCmd.CommandType = CommandType.Text
        sqlCmd.Parameters.Add("@c", SqlDbType.VarChar, 50).Value = abbr
        Dim dr As SqlDataReader = sqlCmd.ExecuteReader
        dr.Read()
        nCode = CInt(dr.Item("code"))
        dr.Close()
        sqlCmd.Connection.Close()
        sqlCmd.Dispose()
        sqlConn.Close()
        Return nCode
    End Function

    Public Function GetContactPayLog(ByVal intLogID As Integer) As DataRow
        Dim strCommandText As String = "SELECT PaymentStatus, CodeEGS FROM EgsContactPayLog WHERE [ID]=@LogID"
        Dim arrParam() As SqlParameter = {New SqlParameter("@LogID", SqlDbType.Int)}
        arrParam(0).Value = intLogID

        Try
            Dim dt As DataTable = ExecuteDataset(L_strCnn, CommandType.Text, strCommandText, arrParam).Tables(0)
            If dt.Rows.Count > 0 Then Return dt.Rows(0)
            Return Nothing
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    'this was made to clear all existing pending logs but was not commited by the user
    Public Function DeactivateCMOnlineLogByEGSID(ByVal intEGSID As Integer) As enumEgswErrorCode
        'do not delete trial account logs
        Dim strCommandText As String = "UPDATE EgsContactCMOnlineLog SET Status=10 WHERE CodeEgs=@CodeEGS AND CodePayment<>1 AND ISNULL(ContactPayLogID,0)=0 AND Status=0"
        Dim arrParam() As SqlParameter = {New SqlParameter("@CodeEGS", SqlDbType.Int)}
        arrParam(0).Value = intEGSID

        Try
            ExecuteNonQuery(L_strCnn, CommandType.Text, strCommandText, arrParam)
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function UpdateCMOnlineLogContactPayLogID(ByVal intLogID As Integer, ByVal intContactPayLogID As Integer) As enumEgswErrorCode
        Dim strCommandText As String = "UPDATE EgsContactCMOnlineLog SET ContactPayLogId=@ContactPayLogID WHERE [ID]=@LogID"
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@LogID", SqlDbType.Int)
        arrParam(1) = New SqlParameter("@ContactPayLogID", SqlDbType.Int)

        arrParam(0).Value = intLogID
        arrParam(1).Value = intContactPayLogID

        Try
            ExecuteNonQuery(L_strCnn, CommandType.Text, strCommandText, arrParam)
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try

    End Function

    'note:
    'EgscontactCMonlineLog
    'status= 0, pending
    'status= 1, active
    'status= 2, historical
    'status= 10, did not continue w/ the process and created a new one


    Public Function AddOnlinePaymentMain(ByVal strName As String, ByVal intLanguage As Integer, ByVal intCodeEGS As Integer) As String
        Dim arrParam(16) As SqlParameter
        arrParam(0) = New SqlParameter("@Name", strName)
        arrParam(1) = New SqlParameter("@Language", intLanguage)
        arrParam(2) = New SqlParameter("@ReturnURL", "www.calcmenuonline.com")
        arrParam(3) = New SqlParameter("@Email", "diane@calcmenu.com")
        arrParam(4) = New SqlParameter("@DateAdded", Now.Date)
        arrParam(5) = New SqlParameter("@DateExpiration", DBNull.Value)
        arrParam(6) = New SqlParameter("@CurrencyDisplay", 64)
        arrParam(7) = New SqlParameter("@CurrencyPay", 64)
        arrParam(8) = New SqlParameter("@CodeEGS", intCodeEGS)
        arrParam(9) = New SqlParameter("@AmountShippingDisplay", 0)
        arrParam(10) = New SqlParameter("@AmountShippingPay", 0)
        arrParam(11) = New SqlParameter("@ProcessURL", "http://www.calcmenuonline.com/Registration.aspx?type=MjAxOA%3d%3d-KQo8Kpg8xR4%3d&logid=")
        arrParam(12) = New SqlParameter("@PaymentFor", 1) 'for cmonline should be 1
        arrParam(13) = New SqlParameter("@TransactionID", SqlDbType.NVarChar, 50)

        arrParam(13).Direction = ParameterDirection.Output

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgsOnlinePaymentAddMain", arrParam)
            Return CStr(arrParam(13).Value)
        Catch ex As Exception
            Return "0"
        End Try
    End Function

    Public Function AddOnlinePaymentDetails(ByVal strTxnID As String, ByVal strDescription As String, ByVal dblPrice As Double) As enumEgswErrorCode
        Dim arrParam(7) As SqlParameter
        arrParam(0) = New SqlParameter("@TransactionID", strTxnID)
        arrParam(1) = New SqlParameter("@Description", strDescription)
        arrParam(2) = New SqlParameter("@DescriptionFTB", DBNull.Value)
        arrParam(3) = New SqlParameter("@PriceDisplay", dblPrice)
        arrParam(4) = New SqlParameter("@PricePay", dblPrice)
        'arrParam(5) = New SqlParameter("@Vatable", 0)
        arrParam(5) = New SqlParameter("@RateVat", 0)
        arrParam(6) = New SqlParameter("@Quantity", 1)
        arrParam(7) = New SqlParameter("@Pos", 0)

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgsOnlinePaymentAddDetails", arrParam)
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    'check if contact paylog id laready exists in egscontactpaycmonlinelog
    Public Function CheckIfContactPayLogIDExist(ByVal intContactPayLogID As Integer) As Boolean
        Dim strSQL As String
        strSQL &= " SELECT [ID]"
        strSQL &= " FROM EgsContactCMOnlineLog"
        strSQL &= " WHERE ISNULL(ContactPayLogId, 0)=@intContactPayLogID"

        Dim arrParam() As SqlParameter = {New SqlParameter("@intContactPayLogID", SqlDbType.Int)}
        arrParam(0).Value = intContactPayLogID

        Try
            Dim dr As SqlDataReader = ExecuteReader(L_strCnn, CommandType.Text, strSQL, arrParam)
            CheckIfContactPayLogIDExist = False
            If dr.Read Then CheckIfContactPayLogIDExist = True
            dr.Close()
        Catch ex As Exception
            Throw ex
        End Try
    End Function
End Class

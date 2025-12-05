Imports System.Data.SqlClient
Imports System.Data
Imports System.Net.Mail
Imports System.Net.Mail.MailMessage
Imports System.Net.Mail.SmtpClient
Imports System.Text

Public Class clsMail
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

#Region "ENUM"

    Public Enum enumMailType
        NetMargin = 1
        MenuPlan = 2
    End Enum

#End Region

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

#Region "Save Methods"
    ''' <summary>
    ''' Add email alerts for pending requests in email queue table
    ''' </summary>
    ''' <param name="strWebURL"></param>
    ''' <remarks></remarks>
    Public Function SetEmailForApprovers(ByVal strWebURL As String) As enumEgswErrorCode
        Dim cUser As New clsUser(enumAppType.WebApp, L_strCnn)
        Dim dr As SqlDataReader = CType(cUser.GetListApproversOnly(), SqlDataReader)
        Dim drItems As SqlDataReader
        Dim cApp As clsApproval
        Dim udtUser As New structUser
        Dim blnHasItemsToApprove As Boolean
        Dim arrID As New ArrayList
        Dim cConfig As New clsConfig(enumAppType.WebApp, L_strCnn)
        Dim strEmailSEnder As String = cConfig.GetConfig(clsConfig.CodeUser.[global], clsConfig.enumNumeros.SMTPEmailSender, clsConfig.CodeGroup.[global], "")
        Dim cLang As clsEGSLanguage
        ' Build message for email
        Dim blnIsEmailSent As Boolean

        ' loop per approver
        Try
            While dr.Read
                With udtUser
                    .Code = CInt(dr("code"))
                    .CodeTrans = CInt(dr("codeTrans"))
                    .CodeLang = CInt(dr("codeLang").ToString)
                    .Email = dr("email").ToString
                    .Fullname = dr("fullname").ToString
                    cApp = New clsApproval(udtUser, enumAppType.WebApp, L_strCnn)
                    drItems = CType(cApp.GetListeListToApprove(udtUser.Code), SqlDataReader)
                    blnHasItemsToApprove = False
                    ' check items to approve
                    While drItems.Read
                        If IsDBNull(drItems("emailSent")) Then blnIsEmailSent = False Else blnIsEmailSent = CBool(drItems("emailSent"))
                        If blnIsEmailSent = False Then
                            blnHasItemsToApprove = True
                            ' get the IDs to flag as email sent later
                            If Not arrID.Contains(drItems("ID").ToString) Then
                                arrID.Add(drItems("ID").ToString)
                            End If
                        End If
                    End While
                    drItems.Close()
                End With

                ' if user needs to be notified, add email in the email queue table

                If blnHasItemsToApprove = True Then
                    ' Build Message
                    cLang = New clsEGSLanguage(udtUser.CodeLang)
                    Dim sbBody As New StringBuilder
                    With sbBody
                        .Append(udtUser.Fullname & ",")
                        .Append("<BR>")
                        .Append("<BR>")
                        .Append(cLang.GetString(clsEGSLanguage.CodeType.YouHaveNewRequestsToApprove))
                        .Append("<BR>")
                        .Append("<BR>")
                        .Append("<a href='http://" & strWebURL & "'>" & cLang.GetString(clsEGSLanguage.CodeType.Sign_In) & "</a>")
                    End With

                    Dim errorCode As enumEgswErrorCode = UpdateEmailQueue(-1, udtUser.Email, strEmailSEnder, cLang.GetString(clsEGSLanguage.CodeType.CalcmenuWeb), sbBody.ToString, "", "", Net.Mail.MailPriority.High, False)
                    Select Case errorCode
                        Case enumEgswErrorCode.OK
                            ' do nothing
                        Case Else
                            ' do nothing
                    End Select
                End If
            End While
            dr.Close()

            '// flag approval request email as sent
            If arrID.Count <> 0 Then
                Dim strIDList As String = "(" & Join(arrID.ToArray, ",") & ")"
                cApp.UpdateListeApproveEmailSentFlag(strIDList, True)
            End If

            Return enumEgswErrorCode.OK

        Catch ex As Exception
            If drItems.IsClosed = False Then drItems.Close()
            If dr.IsClosed = False Then dr.Close()
            Throw ex
        End Try

    End Function

    Public Function fctUpdateEmailStatus(ByVal intCodeSite As Integer, ByVal intType As Integer, ByVal bStatus As Boolean) As String
        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@intType", intType)
        arrParam(2) = New SqlParameter("@bStatus", bStatus)


        Try

            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "EMAIL_UpdateEmailStatus", arrParam)
            Return ""
            'Return CType(arrParam(9).Value, enumEgswErrorCode)
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    ''' <summary>
    ''' Add email alerts for requestse that have been approved or rejected
    ''' </summary>
    ''' <param name="strWebURL"></param>
    ''' <remarks></remarks>
    Public Function SetEmailForRequestors(ByVal strWebURL As String) As enumEgswErrorCode

        Dim cUser As New clsUser(enumAppType.WebApp, L_strCnn)
        Dim dr As SqlDataReader = CType(cUser.GetList(), SqlDataReader)
        Dim drItems As SqlDataReader
        Dim cApp As clsApproval
        Dim udtUser As New structUser
        Dim blnHasItemsToEmail As Boolean
        Dim arrID As New ArrayList
        Dim cConfig As New clsConfig(enumAppType.WebApp, L_strCnn)
        Dim strEmailSEnder As String = cConfig.GetConfig(clsConfig.CodeUser.[global], clsConfig.enumNumeros.SMTPEmailSender, clsConfig.CodeGroup.[global], "")
        Dim blnIsEmailSent As Boolean
        Dim cLang As clsEGSLanguage
        ' Build message for email
        Try
            ' loop per user
            While dr.Read
                With udtUser
                    .Code = CInt(dr("code"))
                    .CodeTrans = CInt(dr("codeTrans"))
                    .CodeLang = CInt(dr("codeLang").ToString)
                    .Email = dr("email").ToString
                    .Fullname = dr("fullname").ToString
                    cApp = New clsApproval(udtUser, enumAppType.WebApp, L_strCnn)
                    drItems = CType(cApp.GetListeListRequest(udtUser.Code), SqlDataReader)
                    blnHasItemsToEmail = False
                    ' check items that have been approved and rejected 
                    While drItems.Read
                        If IsDBNull(drItems("emailSent")) Then blnIsEmailSent = False Else blnIsEmailSent = CBool(drItems("emailSent"))
                        If blnIsEmailSent = False And CType(drItems("status"), enumApprovalStatus) <> enumApprovalStatus.Pending Then
                            blnHasItemsToEmail = True
                            ' get the IDs to flag as email sent later
                            If Not arrID.Contains(drItems("ID").ToString) Then
                                arrID.Add(drItems("ID").ToString)
                            End If
                        End If

                    End While
                    drItems.Close()
                End With

                ' if user needs to be notified, add email in the email queue table

                If blnHasItemsToEmail = True Then
                    ' Build Message
                    cLang = New clsEGSLanguage(udtUser.CodeLang)
                    Dim sbBody As New StringBuilder
                    With sbBody
                        .Append(udtUser.Fullname & ",")
                        .Append("<BR>")
                        .Append("<BR>")
                        .Append(cLang.GetString(clsEGSLanguage.CodeType.YourRequestHasBeenReviewed))
                        .Append("<BR>")
                        .Append("<BR>")
                        .Append("<a href='http://" & strWebURL & "'>" & cLang.GetString(clsEGSLanguage.CodeType.Sign_In) & "</a>")
                    End With

                    Dim errorCode As enumEgswErrorCode = UpdateEmailQueue(-1, udtUser.Email, strEmailSEnder, cLang.GetString(clsEGSLanguage.CodeType.CalcmenuWeb), sbBody.ToString, "", "", Net.Mail.MailPriority.High, False)
                    Select Case errorCode
                        Case enumEgswErrorCode.OK
                            ' do nothing
                        Case Else
                            ' do nothing
                    End Select
                End If
            End While
            dr.Close()

            '// flag approval request email as sent
            If arrID.Count <> 0 Then
                Dim strIDList As String = "(" & Join(arrID.ToArray, ",") & ")"
                cApp.UpdateListeApproveEmailSentFlag(strIDList, True)
            End If

            Return enumEgswErrorCode.OK

        Catch ex As Exception
            If drItems.IsClosed = False Then drItems.Close()
            If dr.IsClosed = False Then dr.Close()
            Throw ex
        End Try

    End Function
    ''' <summary>
    ''' Add or update record in EGswEmaileQueue table
    ''' </summary>
    ''' <param name="intID"></param>
    ''' <param name="strTo"></param>
    ''' <param name="strFrom"></param>
    ''' <param name="strSubject"></param>
    ''' <param name="strBody"></param>
    ''' <param name="strCC"></param>
    ''' <param name="strBCC"></param>
    ''' <param name="priority"></param>
    ''' <param name="IsSent"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateEmailQueue(ByVal intID As Integer, ByVal strTo As String, ByVal strFrom As String, ByVal strSubject As String, ByVal strBody As String, ByVal strCC As String, ByVal strBCC As String, ByVal priority As MailPriority, ByVal IsSent As Boolean) As enumEgswErrorCode
        Dim arrParam(9) As SqlParameter
        arrParam(0) = New SqlParameter("@intID", intID)
        arrParam(1) = New SqlParameter("@nvcTo", strTo)
        arrParam(2) = New SqlParameter("@nvcFrom", strFrom)
        arrParam(3) = New SqlParameter("@nvcSubject", strSubject)
        arrParam(4) = New SqlParameter("@nvcBody", strBody)
        arrParam(5) = New SqlParameter("@nvcCC", strCC)
        arrParam(6) = New SqlParameter("@nvcBCC", strBCC)
        arrParam(7) = New SqlParameter("@intPriority", priority)
        arrParam(8) = New SqlParameter("@IsSent", IsSent)
        arrParam(9) = New SqlParameter("@retval", SqlDbType.Int)
        arrParam(9).Direction = ParameterDirection.ReturnValue

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswEmailQueueUpdate", arrParam)
            Return CType(arrParam(9).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function UpdateInsertEmailMessage(ByVal intID As Integer, ByVal intType As Integer, ByVal intCodeTrans As Integer, ByVal strTo As String, ByVal strFrom As String, ByVal strDisplayName As String, ByVal strSubject As String, ByVal strMessage As String, ByVal intPriority As MailPriority, ByVal strEncoding As String, ByVal strSMTPServer As String, ByVal intPort As Integer, ByVal strUsername As String, ByVal strPassword As String, ByVal strStartTime As String, ByVal intDay As Integer, ByVal intFrequency As Integer, ByVal bStatus As Boolean, ByRef intCodeReturn As Integer) As String
        Dim arrParam(18) As SqlParameter
        If intFrequency = 0 Then
            intDay = -1
        End If
        arrParam(0) = New SqlParameter("@intCodeSite", intID)
        arrParam(1) = New SqlParameter("@intType", intType)
        arrParam(2) = New SqlParameter("@intCodeTrans", intCodeTrans)
        arrParam(3) = New SqlParameter("@strTo", strTo)
        arrParam(4) = New SqlParameter("@strFrom", strFrom)
        arrParam(5) = New SqlParameter("@strDisplayName", strDisplayName)
        arrParam(6) = New SqlParameter("@strSubject", strSubject)
        arrParam(7) = New SqlParameter("@strMessage", strMessage)
        arrParam(8) = New SqlParameter("@intPriority", CInt(intPriority))
        arrParam(9) = New SqlParameter("@strEncoding", strEncoding)
        arrParam(10) = New SqlParameter("@strSMTPServer", strSMTPServer)
        arrParam(11) = New SqlParameter("@intPort", intPort)
        arrParam(12) = New SqlParameter("@strUsername", strUsername)
        arrParam(13) = New SqlParameter("@strPassword", strPassword)
        arrParam(14) = New SqlParameter("@strStartTime", strStartTime)
        arrParam(15) = New SqlParameter("@intFrequency", intFrequency)
        arrParam(16) = New SqlParameter("@intDay", intDay)
        arrParam(17) = New SqlParameter("@bStatus", bStatus)
        arrParam(18) = New SqlParameter("@intCodeReturn", intCodeReturn)
        arrParam(18).Direction = ParameterDirection.ReturnValue

        Try

            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "EMAIL_UpdateInsertEmailMsg", arrParam)
            intCodeReturn = CIntDB(arrParam(18).Value)
            Return ""
            'Return CType(arrParam(9).Value, enumEgswErrorCode)
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Function fctUpdateInsertMessageTranslation(ByVal intCodeMessage As Integer, ByVal intCodeTrans As Integer, ByVal strDisplayName As String, ByVal strSubject As String, ByVal strMessage As String) As String
        Dim arrParam(4) As SqlParameter
        
        arrParam(0) = New SqlParameter("@intCodeMessage", intCodeMessage)
        arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans)
        arrParam(2) = New SqlParameter("@strDisplayName", strDisplayName)
        arrParam(3) = New SqlParameter("@strSubject", strSubject)
        arrParam(4) = New SqlParameter("@strMessage", strMessage)

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "EMAIL_UpdateInsertEmailMsgTrans", arrParam)
            Return ""
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

#End Region

#Region "Private Methods"

#End Region

#Region "Get Methods"

    ''' <summary>
    ''' Returns list of items in queue for email sending
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList() As Object
        Try
            Return CType(ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "sp_EgswEmailQueueGetList"), SqlDataReader)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetNetMarginEmailSettings(ByVal intCodeSite As Integer, ByVal intType As Integer, Optional ByVal intDay As Integer = -1) As DataSet
        Dim ds As New DataSet
        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@intType", intType)
        arrParam(2) = New SqlParameter("@intDay", intDay)
        Try
            ds = ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "[EMAIL_GetNetMarginEmailSettings]", arrParam)
            'dt = ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "EMAIL_UpdateInsertEmailMsg", arrParam)
            'Return CType(arrParam(9).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try

        Return ds
    End Function

#End Region

#Region "Execute Methods"

    ''' <summary>
    ''' Send Email 
    ''' </summary>
    ''' <param name="SenderName"></param>
    ''' <param name="SenderEmail"></param>
    ''' <param name="RecipientName"></param>
    ''' <param name="RecipientEmail"></param>
    ''' <param name="Subj"></param>
    ''' <param name="Messg"></param>
    ''' <param name="strFile"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendMailBCC(ByVal SenderName As String, ByVal SenderEmail As String, ByVal RecipientName As String, ByVal RecipientEmail As String, _
        ByVal BCCName As String, ByVal BCCEmail As String, _
        ByVal Subj As String, ByVal Messg As String, ByVal strFile As String, Optional ByVal IsBodyHTML As Boolean = True) As enumEgswErrorCode

        Dim cConfig As clsConfig = New clsConfig(enumAppType.WebApp, L_strCnn)
        Dim strSMTPUsername As String = cConfig.GetConfig(clsConfig.CodeUser.global, clsConfig.enumNumeros.SMTPUserName, clsConfig.CodeGroup.global, "")
        Dim strSMTPPassword As String = cConfig.GetConfig(clsConfig.CodeUser.global, clsConfig.enumNumeros.SMTPPassword, clsConfig.CodeGroup.global, "")
        Dim strSMTPServer As String = cConfig.GetConfig(clsConfig.CodeUser.global, clsConfig.enumNumeros.SMTPServer, clsConfig.CodeGroup.global, "")
        Dim strSMTPPort As String = cConfig.GetConfig(clsConfig.CodeUser.global, clsConfig.enumNumeros.SMTPPort, clsConfig.CodeGroup.global, "25")
        Dim strSMTPEmailSender As String = cConfig.GetConfig(clsConfig.CodeUser.global, clsConfig.enumNumeros.SMTPEmailSender, clsConfig.CodeGroup.global, "")
        Dim strSMTPSendUsing As String = cConfig.GetConfig(clsConfig.CodeUser.global, clsConfig.enumNumeros.SMTPSendUsing, clsConfig.CodeGroup.global, "0")

        Dim MailAddressFrom As New MailAddress(SenderEmail, SenderName)
        Dim MailAddressTo As New MailAddress(RecipientEmail, RecipientName)
        Dim objMail As New MailMessage(MailAddressFrom, MailAddressTo)
        objMail.Bcc.Add(New MailAddress(BCCEmail, BCCName))
        objMail.Subject = Subj
        objMail.Body = Messg
        objMail.IsBodyHtml = IsBodyHTML

        If strFile <> "" Then
            objMail.Attachments.Add(New Attachment(strFile))
        End If

        Try
            Dim SMTP As New SmtpClient(strSMTPServer, CInt(strSMTPPort))
            SMTP.Send(objMail)
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Throw ex
        End Try
    End Function


    ''' <summary>
    ''' Send Email 
    ''' </summary>
    ''' <param name="SenderName"></param>
    ''' <param name="SenderEmail"></param>
    ''' <param name="RecipientName"></param>
    ''' <param name="RecipientEmail"></param>
    ''' <param name="Subj"></param>
    ''' <param name="Messg"></param>
    ''' <param name="strFile"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendMail(ByVal SenderName As String, ByVal SenderEmail As String, ByVal RecipientName As String, _
            ByVal RecipientEmail As String, ByVal Subj As String, ByVal Messg As String, ByVal strFile As String, _
            Optional ByVal IsBodyHTML As Boolean = True, _
            Optional ByVal SMTPServer As String = "", Optional ByVal SMTPPort As Integer = 0, _
            Optional ByVal SMTPServer2 As String = "", Optional ByVal SMTPPort2 As Integer = 0) As enumEgswErrorCode

        Dim cConfig As clsConfig = New clsConfig(enumAppType.WebApp, L_strCnn)
        Dim strSMTPUsername As String = cConfig.GetConfig(clsConfig.CodeUser.global, clsConfig.enumNumeros.SMTPUserName, clsConfig.CodeGroup.global, "")
        Dim strSMTPPassword As String = cConfig.GetConfig(clsConfig.CodeUser.global, clsConfig.enumNumeros.SMTPPassword, clsConfig.CodeGroup.global, "")
        Dim strSMTPServer As String = cConfig.GetConfig(clsConfig.CodeUser.global, clsConfig.enumNumeros.SMTPServer, clsConfig.CodeGroup.global, "")
        Dim strSMTPPort As String = cConfig.GetConfig(clsConfig.CodeUser.global, clsConfig.enumNumeros.SMTPPort, clsConfig.CodeGroup.global, "25")
        Dim strSMTPEmailSender As String = cConfig.GetConfig(clsConfig.CodeUser.global, clsConfig.enumNumeros.SMTPEmailSender, clsConfig.CodeGroup.global, "")
        Dim strSMTPSendUsing As String = cConfig.GetConfig(clsConfig.CodeUser.global, clsConfig.enumNumeros.SMTPSendUsing, clsConfig.CodeGroup.global, "0")

        If strSMTPEmailSender <> "" Then
            SenderEmail = strSMTPEmailSender
        End If

        Dim MailAddressFrom As New MailAddress(strSMTPUsername, SenderName)

        Dim MailAddressTo As New MailAddress(RecipientEmail, RecipientName)
        Dim objMail As New MailMessage(MailAddressFrom, MailAddressTo)
        objMail.Subject = Subj
        objMail.Body = Messg
        objMail.IsBodyHtml = IsBodyHTML
        If strFile <> "" Then
            objMail.Attachments.Add(New Attachment(strFile))
        End If

        Try
            If SMTPServer <> "" Then
                strSMTPServer = SMTPServer
                strSMTPPort = SMTPPort.ToString
            End If

            Dim SMTP As New SmtpClient(strSMTPServer, CInt(strSMTPPort))


            If strSMTPUsername <> "" Then
                'MRC 12.10.08 - New way of sending
                Dim basicAuthenticationInfo As New System.Net.NetworkCredential(strSMTPUsername, strSMTPPassword)
                SMTP.Host = strSMTPServer
                SMTP.UseDefaultCredentials = False
                SMTP.Credentials = basicAuthenticationInfo
            Else
                SMTP.UseDefaultCredentials = True 'DLS 20.1.2009
            End If

            Try
                SMTP.EnableSsl = True
                SMTP.Send(objMail)
            Catch ex As Exception
                SMTP.EnableSsl = False
                SMTP.Send(objMail)
            End Try


            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
            GoTo retry
        End Try

retry:
        Try
            Dim SMTP As New SmtpClient(SMTPServer2, SMTPPort2)
            SMTP.UseDefaultCredentials = True
            SMTP.Send(objMail)
            Return enumEgswErrorCode.OK
        Catch ex As Exception
        End Try
    End Function

    ''' <summary>
    ''' Send Email
    ''' </summary>
    ''' <param name="strSMTPUsername"></param>
    ''' <param name="strSMTPPassword"></param>
    ''' <param name="strSMTPServer"></param>
    ''' <param name="strSMTPPort"></param>
    ''' <param name="strSMTPEmailSender"></param>
    ''' <param name="strSMTPSendUsing"></param>
    ''' <param name="SenderName"></param>
    ''' <param name="SenderEmail"></param>
    ''' <param name="RecipientName"></param>
    ''' <param name="RecipientEmail"></param>
    ''' <param name="Subj"></param>
    ''' <param name="Messg"></param>
    ''' <param name="strFile"></param>
    ''' <param name="IsBodyHTML"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendMail(ByVal strSMTPUsername As String, ByVal strSMTPPassword As String, ByVal strSMTPServer As String, ByVal strSMTPPort As Integer, _
        ByVal strSMTPEmailSender As String, ByVal strSMTPSendUsing As String, _
         ByVal SenderName As String, ByVal SenderEmail As String, ByVal RecipientName As String, _
            ByVal RecipientEmail As String, ByVal Subj As String, ByVal Messg As String, ByVal strFile As String, Optional ByVal IsBodyHTML As Boolean = True) As enumEgswErrorCode

        Dim MailAddressFrom As New MailAddress(strSMTPUsername, strSMTPUsername)
        Dim MailAddressTo As New MailAddress(RecipientEmail, RecipientName)
        Dim objMail As New MailMessage(MailAddressFrom, MailAddressTo)
        objMail.Subject = Subj
        objMail.Body = Messg
        objMail.IsBodyHtml = IsBodyHTML
        If strFile <> "" Then
            objMail.Attachments.Add(New Attachment(strFile))
        End If

        Try
            Dim SMTP As New SmtpClient(strSMTPServer, CInt(strSMTPPort))
            'SMTP.Send(objMail)
            'Return enumEgswErrorCode.OK

            'MRC 12.10.08 - New way of sending
            Dim basicAuthenticationInfo As New System.Net.NetworkCredential(strSMTPUsername, strSMTPPassword) ''"jcfqsfhhfdyhvcgl"
            SMTP.Host = strSMTPServer
            SMTP.EnableSsl = True
            SMTP.UseDefaultCredentials = False
            SMTP.Credentials = basicAuthenticationInfo
            SMTP.Send(objMail)

            Return enumEgswErrorCode.OK

        Catch ex As Exception
            Throw ex
        End Try
    End Function
    ''' <summary>
    ''' Send all emails in queue
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendEmailInQueue() As enumEgswErrorCode
        Dim dr As SqlDataReader
        Try
            dr = CType(GetList(), SqlDataReader)
            Dim errorCode As enumEgswErrorCode
            While dr.Read
                errorCode = Me.SendMail(dr("From").ToString, dr("From").ToString, dr("To").ToString, dr("To").ToString, dr("Subject").ToString, dr("Body").ToString, "")

                ' log
                'Select Case errorCode
                '    Case enumEgswErrorCode.OK

                '    Case Else
                'End Select

                ' email has been sent
                errorCode = UpdateEmailQueue(CInt(dr("ID").ToString), "", "", "", "", "", "", MailPriority.High, True)
                'Select Case errorCode
                '    Case enumEgswErrorCode.OK

                '    Case Else
                'End Select


            End While
            dr.Close()

            Return enumEgswErrorCode.OK
        Catch ex As Exception
            If dr.IsClosed = False Then dr.Close()
            Throw ex
        End Try


    End Function
#End Region


End Class

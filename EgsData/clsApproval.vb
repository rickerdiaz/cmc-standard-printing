Imports System.Data.SqlClient
Imports System.Data
Imports System.Threading
Imports System.IO
Imports System.Text

Public Class clsApproval
#Region "Class Header"
    'Name               : clsApproval
    'Decription         : Manages Approval Table
    'Date Created       : 28.9.2005
    'Author             : JRL
    'Revision History   : 
    '
#End Region

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
    Private L_udtUser As structUser

    Private L_udtMail As structMail

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
            L_AppType = eAppType
            L_bytFetchType = bytFetchType
            L_strCnn = strCnn
            L_udtUser = udtUser
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

    Public Property MailInfo() As structMail
        Get
            MailInfo = L_udtMail
        End Get
        Set(ByVal value As structMail)
            L_udtMail = value
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
    ''' Returns TRUE if given action mark function requires approval, otherwise, false.
    ''' </summary>
    ''' <param name="fnc"></param>
    ''' <param name="udtUser"></param>
    ''' <param name="listetype"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsRequireApproval(ByVal fnc As UserRightsFunction, ByVal udtUser As structUser, ByVal listetype As enumDataListItemType) As Boolean
        Select Case fnc
            Case UserRightsFunction.AllowMassChangeBrand, UserRightsFunction.AllowMassChangeCategory, UserRightsFunction.AllowMassChangeSupplier, UserRightsFunction.AllowMassChangeSource, UserRightsFunction.AllowTransfer
                Return udtUser.arrListeTypeApprovalRequired.Contains(CStr(listetype))
            Case Else
                Return False
        End Select
    End Function

    ''' <summary>
    ''' Save approval setting
    ''' </summary>
    ''' <param name="intlisteType"></param>
    ''' <param name="blnApproveSite"></param>
    ''' <param name="blnApproveProperty"></param>
    ''' <param name="blnapproveSystem"></param>
    ''' <param name="blnapprovalFlag"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateApprovalSetting(ByVal intlisteType As Integer, _
               ByVal blnApproveSite As Boolean, ByVal blnApproveProperty As Boolean, _
               ByVal blnapproveSystem As Boolean, ByVal blnapprovalFlag As Boolean) As enumEgswErrorCode

        Dim arrParam(5) As SqlParameter
        arrParam(0) = New SqlParameter("@intListeType", intlisteType)
        arrParam(1) = New SqlParameter("@bitApproveSite", blnApproveSite)
        arrParam(2) = New SqlParameter("@bitApproveProperty", blnApproveProperty)
        arrParam(3) = New SqlParameter("@bitApproveSystem", blnapproveSystem)
        arrParam(4) = New SqlParameter("@bitApprovalFlag", blnapprovalFlag)
        arrParam(5) = New SqlParameter("@retval", SqlDbType.Int)
        arrParam(5).Direction = ParameterDirection.ReturnValue

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswApprovalSettingUpdate", arrParam)
            Return CType(arrParam(5).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try
    End Function


    ''' <summary>
    ''' Approve or disapprove a pending request
    ''' </summary>
    ''' <param name="intID"></param>
    ''' <param name="status"></param>
    ''' <param name="intCodeUserApprover"></param>
    ''' <param name="approverroleLevel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateListeApproveChangeStatus(ByVal intID As Integer, ByVal status As enumApprovalStatus, ByVal intCodeUserApprover As Integer, ByVal approverroleLevel As enumGroupLevel) As enumEgswErrorCode
        Dim arrParam(4) As SqlParameter
        arrParam(0) = New SqlParameter("@intID", intID)
        arrParam(1) = New SqlParameter("@intStatus", status)
        arrParam(2) = New SqlParameter("@intCodeUserApprover", intCodeUserApprover)
        arrParam(3) = New SqlParameter("@intRoleLevel", approverroleLevel)
        arrParam(4) = New SqlParameter("@retval", SqlDbType.Int)
        arrParam(4).Direction = ParameterDirection.ReturnValue

        Try
            ' adjust time out for transfer 
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswListeApproveUpdateChangeStatus", arrParam, 3600)
            Return CType(arrParam(4).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    ''' <summary>
    ''' Sent Request Email Flag as Sent/Not Yet Send
    ''' </summary>
    ''' <param name="strIDList"></param>
    ''' <param name="IsEmailSent"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateListeApproveEmailSentFlag(ByVal strIDList As String, ByVal IsEmailSent As Boolean) As enumEgswErrorCode
        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@IsEmailSent", IsEmailSent)
        arrParam(1) = New SqlParameter("@vchIDList", strIDList)
        arrParam(2) = New SqlParameter("@retval", SqlDbType.Int)
        arrParam(2).Direction = ParameterDirection.ReturnValue

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswListeApproveUpdateEmailFlag", arrParam)
            Return CType(arrParam(2).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''' <summary>
    ''' Add request in EgswListeApprove table
    ''' </summary>
    ''' <param name="intCodeListe"></param>
    ''' <param name="requestType"></param>
    ''' <param name="intCodeSetPrice"></param>
    ''' <param name="dblApprovedPriceNew"></param>
    ''' <param name="intCodeReplace"></param>
    ''' <param name="ApproverRoleLevel"></param>
    ''' <param name="intCodeSite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateListeApproval(ByVal intCodeListe As Integer, ByVal requestType As enumRequestType, ByVal intCodeSetPrice As Integer, ByVal dblApprovedPriceNew As Double, ByVal intCodeReplace As Integer, ByVal ApproverRoleLevel As enumGroupLevel, Optional ByVal intCodeSite As Integer = 0) As enumEgswErrorCode
        Dim arrParam(8) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeListe", intCodeListe)
        arrParam(1) = New SqlParameter("@intRequestType", requestType)
        arrParam(2) = New SqlParameter("@intCodeUser", L_udtUser.Code)
        arrParam(3) = New SqlParameter("@intCodeSetPrice", intCodeSetPrice)
        arrParam(4) = New SqlParameter("@fltApprovedPriceNew", dblApprovedPriceNew)
        arrParam(5) = New SqlParameter("@intCodeReplace", intCodeReplace)
        arrParam(6) = New SqlParameter("@intApproverRoleLevel", ApproverRoleLevel)
        arrParam(7) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(8) = New SqlParameter("@retval", SqlDbType.Int)
        arrParam(8).Direction = ParameterDirection.ReturnValue

        Try
            If requestType = enumRequestType.Transfer Then
                ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswListeApproveUpdate", arrParam, 3600)
            Else
                ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswListeApproveUpdate", arrParam)
            End If
            Return CType(arrParam(8).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try
    End Function


#End Region
#Region "Get Methods"
    ''' <summary>
    ''' Fetch List of Approval Settings
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetApprovalSetting() As Object
        Select Case L_bytFetchType
            Case enumEgswFetchType.DataReader
                Return ExecuteReader(L_strCnn, CommandType.StoredProcedure, "sp_EgswApprovalSettingGet")
            Case enumEgswFetchType.DataSet
                Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "sp_EgswApprovalSettingGet")
            Case enumEgswFetchType.DataTable
                Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "sp_EgswApprovalSettingGet").Tables(0)
        End Select
        Return Nothing
    End Function

    ''' <summary>
    ''' Get List of items requested by the current user
    ''' </summary>
    ''' <param name="intCodeUserFrom"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListeListRequest(ByVal intCodeUserFrom As Integer) As Object
        Return FetchListeList(intCodeUserFrom, -1)
    End Function

    Public Function IsListeRequestExist(ByVal intCodeUserFrom As Integer, ByVal intCodeListe As Integer) As Boolean
        Dim tmpFetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataTable
        Dim dt As DataTable = CType(FetchListeList(intCodeUserFrom, -1), DataTable)
        dt.DefaultView.RowFilter = "CodeListe=" & intCodeListe & " AND Status=0"
        If dt.DefaultView.Count > 0 Then Return True
        Return False
    End Function


   

    ''' <summary>
    '''  Determines the next approver of this request.
    ''' </summary>
    ''' <param name="intCodeListe"></param>
    ''' <param name="requestType"></param>
    ''' <param name="listeType">Assign listtype if codeliste doesn't exist</param>
    ''' <returns>Returns -1 if there's no approver for this request. Otherwise, returns Next Role Level</returns>
    ''' <remarks></remarks>
    Public Function GetNextRoleApprover(ByVal intCodeListe As Integer, ByVal requestType As enumRequestType, ByVal listeType As enumDataListItemType) As Integer
        Try
            Dim arrParam(5) As SqlParameter
            arrParam(0) = New SqlParameter("@intCodeListe", intCodeListe)
            arrParam(1) = New SqlParameter("@intRequestType", requestType)
            arrParam(2) = New SqlParameter("@intCodeUserRequesting", L_udtUser.Code)
            arrParam(3) = New SqlParameter("@intCodeUserRequestingRoleLevel", L_udtUser.RoleLevelHighest)
            arrParam(4) = New SqlParameter("@intRoleLevel", SqlDbType.Int)
            arrParam(4).Direction = ParameterDirection.Output
            arrParam(5) = New SqlParameter("@intListeType", listeType)

            ExecuteNonQuery(L_strCnn, CommandType.Text, "SET @intRoleLevel=dbo.fn_EgswListeApproveGetNextRoleApprover (@intCodeListe,@intRequestType,@intCodeUserRequesting,@intCodeUserRequestingRoleLevel,@intListeType)", arrParam)

            Return CInt(arrParam(4).Value)

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''' <summary>
    '''  Determines the current approver of this request.
    ''' </summary>
    ''' <param name="intCodeListe"></param>
    ''' <param name="requestType"></param>
    ''' <param name="listeType">Assign listtype if codeliste doesn't exist</param>
    ''' <returns>Returns -1 if there's no approver for this request. Otherwise, returns Next Role Level</returns>
    ''' <remarks></remarks>
    Public Function GetCurrentRoleApprover(ByVal intCodeListe As Integer, ByVal requestType As enumRequestType, ByVal listeType As enumDataListItemType) As Integer
        Try
            Dim arrParam(5) As SqlParameter
            arrParam(0) = New SqlParameter("@intCodeListe", intCodeListe)
            arrParam(1) = New SqlParameter("@intRequestType", requestType)
            arrParam(2) = New SqlParameter("@intCodeUserRequesting", L_udtUser.Code)
            arrParam(3) = New SqlParameter("@intCodeUserRequestingRoleLevel", L_udtUser.RoleLevelHighest)
            arrParam(4) = New SqlParameter("@intRoleLevel", SqlDbType.Int)
            arrParam(4).Direction = ParameterDirection.Output
            arrParam(5) = New SqlParameter("@intListeType", listeType)

            ExecuteNonQuery(L_strCnn, CommandType.Text, "SET @intRoleLevel=dbo.fn_EgswListeApproveGetCurrentRoleApprover (@intCodeListe,@intRequestType,@intCodeUserRequesting,@intCodeUserRequestingRoleLevel,@intListeType)", arrParam)

            Return CInt(arrParam(4).Value)

        Catch ex As Exception
            Throw ex
        End Try
    End Function


    ''' <summary>
    ''' Get List of items for approval
    ''' </summary>
    ''' <param name="intCodeUserApprover"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListeListToApprove(ByVal intCodeUserApprover As Integer) As Object
        Return FetchListeList(-1, intCodeUserApprover)
    End Function

    Public Function IsApprover(ByVal intCodeUser As Integer) As Boolean
		Dim strCommandText As String = "GET_LISTEAPPROVERS"	'JTOC 14.06.2013"GET_APPROVERS"        
        Try            
            Dim drApprovers As SqlDataReader = ExecuteReader(L_strCnn, CommandType.StoredProcedure, strCommandText)
            If Not drApprovers Is Nothing Then
                While drApprovers.Read
                    If CInt(drApprovers("Code")) = intCodeUser Then
                        Return True
                    End If
                End While
            End If

            Return False
        Catch ex As Exception
            Throw ex
        End Try
    End Function

		Public Function IsEditor(ByVal intCodeUser As Integer) As Boolean
		Dim strCommandText As String = "GET_LISTEEDITOR"
		Try
			Dim drEditors As SqlDataReader = ExecuteReader(L_strCnn, CommandType.StoredProcedure, strCommandText)
			If Not drEditors Is Nothing Then
				While drEditors.Read
					If CInt(drEditors("Code")) = intCodeUser Then
						Return True
					End If
				End While
			End If

			Return False
		Catch ex As Exception
			Throw ex
		End Try
	End Function

#End Region
#Region "Remove Methods"

  
#End Region

#Region "Private Methods"



    Private Function FetchListeList(ByVal intCodeUserFrom As Integer, ByVal intCodeUserApprover As Integer) As Object
        Try
            Dim arrParam(2) As SqlParameter
            arrParam(0) = New SqlParameter("@intCodeUserFrom", intCodeUserFrom)
            arrParam(1) = New SqlParameter("@intCodeUserApprover", intCodeUserApprover)
            arrParam(2) = New SqlParameter("@intCodeTrans", L_udtUser.CodeTrans)
            Return Me.ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "sp_EgswListeApproveGetList", arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function


#End Region


	Public Function FetchListeRequestList(ByVal intCodeSite As Integer) As Object
		Try
			Dim arrParam(2) As SqlParameter
			arrParam(0) = New SqlParameter("@intCodeSite", intCodeSite)

			Return Me.ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "sp_GetRequestApprovalList", arrParam)
		Catch ex As Exception
			Throw ex
		End Try
	End Function

	'JTOC 17.06.2013 Fetch quantity and unit of unvalidated ingredient
	Public Function FetchQuantityUnitList(ByVal intCodeSite As Integer, ByVal strName As String) As Object
		Try
			Dim arrParam(2) As SqlParameter
			arrParam(0) = New SqlParameter("@intCodeSite", intCodeSite)
			arrParam(1) = New SqlParameter("@strName", strName)


			Return Me.ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "sp_GetQuantityUnit", arrParam)
		Catch ex As Exception
			Throw ex
		End Try
	End Function

    Public Function SendMail(ByVal udtMailInfo As structMail, ByVal dtMsg As DataTable, Optional ByVal strURL As String = "") As Boolean
        Try
            Dim cMail As New clsMail(enumAppType.WebApp, L_strCnn)
            Dim cConfig As New clsConfig(enumAppType.WebApp, L_strCnn)

            Dim strSmtpServer As String = cConfig.GetConfig(clsConfig.CodeUser.global, clsConfig.enumNumeros.SMTPServer, clsConfig.CodeGroup.global, udtMailInfo.SMTPServer)
            Dim intSmtpPort As Integer = CInt(cConfig.GetConfig(clsConfig.CodeUser.global, clsConfig.enumNumeros.SMTPPort, clsConfig.CodeGroup.global, udtMailInfo.SMTPPort))
            Dim strSenderEmail As String = cConfig.GetConfig(clsConfig.CodeUser.global, clsConfig.enumNumeros.SMTPEmailSender, clsConfig.CodeGroup.global, "info@eg-software.com")
            Dim strCodeUser As String = ""

            Dim strCommandText As String = "GET_APPROVERS"
            Try
                'Email to approvers
                Dim drApprovers As SqlDataReader = ExecuteReader(L_strCnn, CommandType.StoredProcedure, strCommandText)
                If Not drApprovers Is Nothing Then
                    While drApprovers.Read
                        If cMail.SendMail("Calcmenu Web", udtMailInfo.SenderEmail, drApprovers("email").ToString.Trim, drApprovers("email").ToString.Trim, subTitle(udtMailInfo.RecipeName, 1, strURL, CInt(drApprovers("CodeTrans")), dtMsg), subMessage(udtMailInfo.SenderName, CStr(drApprovers("UserName")), udtMailInfo.RecipeName, 1, "", CIntDB(drApprovers("CodeTrans")), dtMsg), "", True, udtMailInfo.SMTPServer, CInt(udtMailInfo.SMTPPort)) = enumEgswErrorCode.OK Then

                        End If
                    End While
                End If

                'Email to sender
                If cMail.SendMail("Calcmenu Web", strSenderEmail, udtMailInfo.SenderName, udtMailInfo.SenderEmail, subTitle(udtMailInfo.RecipeName, 2, strURL, udtMailInfo.SenderCodeTrans, dtMsg), subMessage(udtMailInfo.SenderName, udtMailInfo.RecipentName, udtMailInfo.RecipeName, 2, , udtMailInfo.SenderCodeTrans, dtMsg), "", True, udtMailInfo.SMTPServer, CInt(udtMailInfo.SMTPPort)) = enumEgswErrorCode.OK Then

                End If
            Catch ex As Exception
                Throw ex
            End Try

            Dim strApproversEmail As String = ""

        Catch ex As Exception

        End Try

        Return True

    End Function

    Private Function subTitle(Optional ByVal strNameRecipe As String = "", Optional ByVal intMsgType As Integer = 1, Optional ByVal strURL As String = "", Optional ByVal intCodeTrans As Integer = 2, Optional ByVal dtMsgTrans As DataTable = Nothing) As String
        Try
            Dim strTitle As String = ""
            Select Case intMsgType
                Case 1
                    If Not dtMsgTrans Is Nothing Then
                        If dtMsgTrans.Select("CodeTrans=" & intCodeTrans & " AND MessageType=" & intMsgType & " AND ListeType=8").Length > 0 Then
                            Dim r As DataRow = dtMsgTrans.Select("CodeTrans=" & intCodeTrans & " AND MessageType=" & intMsgType & " AND ListeType=8")(0)
                            strTitle = r("Title").ToString.Trim
                            ''Else
                            ''    Select Case intCodeTrans
                            ''        Case 1
                            ''            strTitle = "Recette d'approbation pour %recipe"
                            ''        Case Else
                            ''            strTitle = "Recipe Approval for %recipe"
                            ''    End Select

                        End If
                        If strTitle.Length = 0 Then strTitle = "Recipe Approval for %recipe"
                        strTitle = strTitle.Replace("%recipe", strNameRecipe)
                        Return strTitle
                    End If
                Case 2
                    If Not dtMsgTrans Is Nothing Then
                        If dtMsgTrans.Select("CodeTrans=" & intCodeTrans & " AND MessageType=" & intMsgType & " AND ListeType=8").Length > 0 Then
                            Dim r As DataRow = dtMsgTrans.Select("CodeTrans=" & intCodeTrans & " AND MessageType=" & intMsgType & " AND ListeType=8")(0)
                            strTitle = r("Title").ToString.Trim
                            ''Else
                            ''    Select Case intCodeTrans
                            ''        Case 1
                            ''            strTitle = "Recette d'approbation pour %recipe"
                            ''        Case Else
                            ''            strTitle = "Recipe Approval for %recipe"
                            ''    End Select
                        End If

                        If strTitle.Length = 0 Then strTitle = "Recipe Approval for %recipe"
                        strTitle = strTitle.Replace("%recipe", strNameRecipe)
                        Return strTitle
                    End If
                Case 3
                    If Not dtMsgTrans Is Nothing Then
                        If dtMsgTrans.Select("CodeTrans=" & intCodeTrans & " AND MessageType=" & intMsgType & " AND ListeType=8").Length > 0 Then
                            Dim r As DataRow = dtMsgTrans.Select("CodeTrans=" & intCodeTrans & " AND MessageType=" & intMsgType & " AND ListeType=8")(0)
                            strTitle = r("Title").ToString.Trim
                            ''Else
                            ''    Select Case intCodeTrans
                            ''        Case 1
                            ''            strTitle = "Approuvé recette: %recipe"
                            ''        Case Else
                            ''            strTitle = "Recipe Approved: %recipe"
                            ''    End Select

                        End If

                        If strTitle.Length = 0 Then strTitle = "Recipe Approved: %recipe"
                        strTitle = strTitle.Replace("%recipe", strNameRecipe)
                        Return strTitle
                    End If
                Case 4
                    If Not dtMsgTrans Is Nothing Then
                        If dtMsgTrans.Select("CodeTrans=" & intCodeTrans & " AND MessageType=" & intMsgType & " AND ListeType=8").Length > 0 Then
                            Dim r As DataRow = dtMsgTrans.Select("CodeTrans=" & intCodeTrans & " AND MessageType=" & intMsgType & " AND ListeType=8")(0)
                            strTitle = r("Title").ToString.Trim
                            ''Else
                            ''    Select Case intCodeTrans
                            ''        Case 1
                            ''            strTitle = "Approuvé recette: %recipe"
                            ''        Case Else
                            ''            strTitle = "Recipe Approved: %recipe"
                            ''    End Select
                        End If
                        If strTitle.Length = 0 Then strTitle = "Recipe Approved: %recipe"
                        strTitle = strTitle.Replace("%recipe", strNameRecipe)
                        Return strTitle
                    End If
            End Select
            Return strTitle
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Function subMessage(Optional ByVal strNameSender As String = "", Optional ByVal strNameRecipient As String = "", Optional ByVal strNameRecipe As String = "", Optional ByVal intMsgType As Integer = 1, Optional ByVal strURL As String = "", Optional ByVal intCodeTrans As Integer = 2, Optional ByVal dtMsgTrans As DataTable = Nothing) As String

        'Message Type:
        'SCENARIO 1: Creator of the item submits the item for approval
        ' 1 - MESSAGE TO APPROVERS
        ' 2 - MESSAGE TO THE ITEM’S CREATOR
        'SCENARIO 2: Approvers approved the recipe
        ' 3 - MESSAGE TO THE APPROVERS
        ' 4 - MESSAGE TO THE ITEM’S CREATOR

        Dim msg As New StringBuilder
        Dim strMsg As String = ""
        Try
            Select Case intMsgType
                Case 1
                    With msg
                        If Not dtMsgTrans Is Nothing Then
                            If dtMsgTrans.Select("CodeTrans=" & intCodeTrans & " AND MessageType=" & intMsgType & " AND ListeType=8").Length > 0 Then
                                Dim r As DataRow = dtMsgTrans.Select("CodeTrans=" & intCodeTrans & " AND MessageType=" & intMsgType & " AND ListeType=8")(0)
                                '%approver - approver
                                '%submitter - submitter
                                '%recipe - recipe name
                                strMsg = r("Message").ToString.Trim
                            End If
                        End If


                        If strMsg.Length > 0 Then
                            strMsg = strMsg.Replace(vbCrLf, "<BR>")
                            strMsg = strMsg.Replace("%approver", strNameRecipient)
                            strMsg = strMsg.Replace("%submitter", strNameSender)
                            strMsg = strMsg.Replace("%recipe", strNameRecipe)

                            .Append("<html xmlns='http://www.w3.org/1999/xhtml' ><body><font face=arial size=2>")
                            .Append("<BR>")
                            .Append(strMsg)
                            .Append("<BR>")
                            .Append("</font></body></html>")

                        Else
                            ''Select Case intCodeTrans
                            ''    Case 1
                            ''        .Append("<html xmlns='http://www.w3.org/1999/xhtml' ><body><font face=arial size=2>")
                            ''        .Append("<BR>Hi " & strNameRecipient & ",")
                            ''        .Append("<BR><BR>Vous avez reçu une recette qui doit être approuvée. " & strNameSender & " vous soumets cette recette: ")
                            ''        .Append("<BR><BR><B>" & strNameRecipe & "</B>")
                            ''        .Append("<BR><BR>Veuillez vous <A HREF=" & strURL & ">connectez</A> à CALCMENU Web pour réviser et approuver la recette.")
                            ''        .Append("<BR><BR><BR>Cordialement,")
                            ''        .Append("<BR>L’équipe EGS")
                            ''        .Append("</font></body></html>")

                            ''    Case Else
                            .Append("<html xmlns='http://www.w3.org/1999/xhtml' ><body><font face=arial size=2>")
                            .Append("<BR>Hi " & strNameRecipient & ",")
                            .Append("<BR><BR>You have received a recipe for approval. " & strNameSender & " has submitted this recipe:")
                            .Append("<BR><BR><B>" & strNameRecipe & "</B>")
                            .Append("<BR><BR>Please <A HREF=" & strURL & ">login</A> to the CALCMENU Web site to review and approve the recipe.")
                            .Append("<BR><BR><BR>Regards,")
                            .Append("<BR>EGS Team")
                            .Append("</font></body></html>")
                            ''End Select
                        End If

                    End With
                    Return msg.ToString

                Case 2
                    With msg
                        If dtMsgTrans.Select("CodeTrans=" & intCodeTrans & " AND MessageType=" & intMsgType & " AND ListeType=8").Length > 0 Then
                            Dim r As DataRow = dtMsgTrans.Select("CodeTrans=" & intCodeTrans & " AND MessageType=" & intMsgType & " AND ListeType=8")(0)
                            '%approver - approver
                            '%submitter - submitter
                            '%recipe - recipe name
                            strMsg = r("Message").ToString.Trim
                        End If

                        If strMsg.Length > 0 Then
                            strMsg = strMsg.Replace(vbCrLf, "<BR>")
                            strMsg = strMsg.Replace("%approver", strNameRecipient)
                            strMsg = strMsg.Replace("%submitter", strNameSender)
                            strMsg = strMsg.Replace("%recipe", strNameRecipe)

                            .Append("<html xmlns='http://www.w3.org/1999/xhtml' ><body><font face=arial size=2>")
                            .Append("<BR>")
                            .Append(strMsg)
                            .Append("<BR>")
                            .Append("</font></body></html>")

                        Else
                            ''Select Case intCodeTrans
                            ''    Case 1
                            ''        .Append("<html xmlns='http://www.w3.org/1999/xhtml' ><body><font face=arial size=2>")
                            ''        .Append("<BR>Hi " & strNameSender & ",")
                            ''        .Append("<BR><BR>Votre nouvelle recette a été envoyée pour approbation. Vous avez soumis cette recette:")
                            ''        .Append("<BR><BR><B>" & strNameRecipe & "</B>")
                            ''        .Append("<BR><BR>Une fois approuvé, la recette sera disponible sur le site.")
                            ''        .Append("<BR><BR><BR>Cordialement,")
                            ''        .Append("<BR>L’équipe EGS")
                            ''        .Append("</font></body></html>")
                            ''    Case Else
                            .Append("<html xmlns='http://www.w3.org/1999/xhtml' ><body><font face=arial size=2>")
                            .Append("<BR>Hi " & strNameSender & ",")
                            .Append("<BR><BR>Your newly created recipe has been sent for approval. The recipe will be reviewed and approved first before it can be used online. You have submitted this recipe:")
                            .Append("<BR><BR><B>" & strNameRecipe & "</B>")
                            .Append("<BR><BR>Once approved, the recipe will be available online.")
                            .Append("<BR><BR><BR>Regards,")
                            .Append("<BR>EGS Team")
                            .Append("</font></body></html>")
                            ''End Select
                        End If

                    End With
                    Return msg.ToString
                Case 3
                    With msg
                        If Not dtMsgTrans Is Nothing Then
                            If dtMsgTrans.Select("CodeTrans=" & intCodeTrans & " AND MessageType=" & intMsgType & " AND ListeType=8").Length > 0 Then
                                Dim r As DataRow = dtMsgTrans.Select("CodeTrans=" & intCodeTrans & " AND MessageType=" & intMsgType & " AND ListeType=8")(0)
                                '%approver - approver
                                '%submitter - submitter
                                '%recipe - recipe name
                                strMsg = r("Message").ToString.Trim
                            End If


                            If strMsg.Length > 0 Then
                                strMsg = strMsg.Replace(vbCrLf, "<BR>")
                                strMsg = strMsg.Replace("%approver", strNameRecipient)
                                strMsg = strMsg.Replace("%submitter", strNameSender)
                                strMsg = strMsg.Replace("%recipe", strNameRecipe)

                                .Append("<html xmlns='http://www.w3.org/1999/xhtml' ><body><font face=arial size=2>")
                                .Append("<BR>")
                                .Append(strMsg)
                                .Append("<BR>")
                                .Append("</font></body></html>")
                            Else
                                Select Case intCodeTrans
                                    Case 1
                                        .Append("<html xmlns='http://www.w3.org/1999/xhtml' ><body><font face=arial size=2>")
                                        .Append("<BR>Hi " & strNameRecipient & ",")
                                        .Append("<BR><BR>La recette <B>" & strNameRecipe & "</B> a été approuvée.")
                                        .Append("<BR><BR>La recette sera disponible sur le site.")
                                        .Append("<BR><BR><BR>Cordialement,")
                                        .Append("<BR>L’équipe EGS")
                                        .Append("</font></body></html>")
                                    Case Else
                                        .Append("<html xmlns='http://www.w3.org/1999/xhtml' ><body><font face=arial size=2>")
                                        .Append("<BR>Hi " & strNameRecipient & ",")
                                        .Append("<BR><BR>The recipe <B>" & strNameRecipe & "</B> has been approved.")
                                        .Append("<BR><BR>The recipe will be available online.")
                                        .Append("<BR><BR><BR>Regards,")
                                        .Append("<BR>EGS Team")
                                        .Append("</font></body></html>")
                                End Select
                            End If
                        End If

                    End With
                    Return msg.ToString

                Case 4
                    With msg
                        If dtMsgTrans.Select("CodeTrans=" & intCodeTrans & " AND MessageType=" & intMsgType & " AND ListeType=8").Length > 0 Then
                            Dim r As DataRow = dtMsgTrans.Select("CodeTrans=" & intCodeTrans & " AND MessageType=" & intMsgType & " AND ListeType=8")(0)
                            '%approver - approver
                            '%submitter - submitter
                            '%recipe - recipe name
                            strMsg = r("Message").ToString.Trim
                        End If

                        If strMsg.Length > 0 Then
                            strMsg = strMsg.Replace(vbCrLf, "<BR>")
                            strMsg = strMsg.Replace("%approver", strNameRecipient)
                            strMsg = strMsg.Replace("%submitter", strNameSender)
                            strMsg = strMsg.Replace("%recipe", strNameRecipe)

                            .Append("<html xmlns='http://www.w3.org/1999/xhtml' ><body><font face=arial size=2>")
                            .Append("<BR>")
                            .Append(strMsg)
                            .Append("<BR>")
                            .Append("</font></body></html>")

                        Else
                            ''Select Case intCodeTrans
                            ''    Case 1
                            ''        .Append("<html xmlns='http://www.w3.org/1999/xhtml' ><body><font face=arial size=2>")
                            ''        .Append("<BR>Hi " & strNameSender & ",")
                            ''        .Append("<BR><BR>La recette <B>" & strNameRecipe & "</B> a été approuvée. Vous pouvez maintenant utiliser cette recette sur le site.")
                            ''        .Append("<BR><BR><BR>Cordialement,")
                            ''        .Append("<BR>L’équipe EGS")
                            ''        .Append("</font></body></html>")
                            ''    Case Else
                            .Append("<html xmlns='http://www.w3.org/1999/xhtml' ><body><font face=arial size=2>")
                            .Append("<BR>Hi " & strNameSender & ",")
                            .Append("<BR><BR>The recipe <B>" & strNameRecipe & "</B> has been approved. You can now use this recipe online.")
                            .Append("<BR><BR><BR>Regards,")
                            .Append("<BR>EGS Team")
                            .Append("</font></body></html>")
                            ''End Select
                        End If
                    End With
                    Return msg.ToString
            End Select
            Return msg.ToString
        Catch ex As Exception
            Return ""
        End Try
    End Function

End Class

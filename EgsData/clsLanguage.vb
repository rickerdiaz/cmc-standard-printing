Imports System.Data.SqlClient
Imports System.Data

#Region "Class Header"
'Name               : clsLanguage
'Decription         : Manages Language and Translation
'Date Created       : 28.09.2005
'Author             : JRL
'Revision History   : 
'
#End Region

Public Class clsLanguage
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
    ''' Update Translation
    ''' </summary>
    ''' <param name="intCode">Code of Translation</param>
    ''' <param name="strName">Translation name</param>
    ''' <param name="intCodeDict">EGS Language</param>
    ''' <param name="Status">Set Active True/False </param>
    ''' <param name="TranMode">Add, Edit, Update Status</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateTranslation(ByVal intCode As Integer, ByVal strName As String, ByVal intCodeDict As Integer, ByVal Status As Byte, ByVal TranMode As enumEgswTransactionMode, Optional blnGenderSensitive As Boolean = False) As enumEgswErrorCode
        Try
            Dim arrParam(6) As SqlParameter
            arrParam(0) = New SqlParameter("@retVal", "")
            arrParam(0).Direction = ParameterDirection.ReturnValue
            arrParam(1) = New SqlParameter("@intCode", intCode)
            arrParam(2) = New SqlParameter("@nvcName", strName)
            arrParam(3) = New SqlParameter("@intCodeDict", intCodeDict)
            arrParam(4) = New SqlParameter("@Status", Status)
            arrParam(5) = New SqlParameter("@tntTranMode", TranMode)
            arrParam(6) = New SqlParameter("@bitIsGenderSensitive", blnGenderSensitive)
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswTranslationUpdate", arrParam)
            Return CType(arrParam(0).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try
    End Function
#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Fetch list of translations
    ''' </summary>
    ''' <param name="intCode"></param>
    ''' <param name="bytStatus"></param>
    ''' <param name="intCodeSite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function FetchList(ByVal intCode As Integer, ByVal intCodeSite As Integer, ByVal bytStatus As Byte) As Object
        Dim strCommandText As String = "sp_EgswTranslationGetList"

        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@intCode", intCode)
        arrParam(1) = New SqlParameter("@Status", bytStatus)
        arrParam(2) = New SqlParameter("@intCodeSite", intCodeSite)


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


	Public Function GetListCodeName(ByVal intCodeSite As Integer, ByVal bytStatus As Byte, Optional intDropdown As Integer = 0, Optional intLanguage As Integer = 0) As Object 'JTOC 20.2012 Added intDropdown, intLanguage
		Dim strCommandText As String
		Dim arrParam(1) As SqlParameter

		If intDropdown = 0 Then
			ReDim arrParam(1)
			strCommandText = "GET_TRANSLATIONCODENAME"
			arrParam(0) = New SqlParameter("@CodeSite", intCodeSite)
			arrParam(1) = New SqlParameter("@Status", bytStatus)
		ElseIf intDropdown = 1 Then
			ReDim arrParam(0)
			strCommandText = "GET_PackagingCodeName"
			arrParam(0) = New SqlParameter("@intLanguage", intLanguage)
		ElseIf intDropdown = 2 Then
			ReDim arrParam(0)
			strCommandText = "GET_RecipeCertificationCodeName"
			arrParam(0) = New SqlParameter("@intLanguage", intLanguage)
		ElseIf intDropdown = 3 Then
			ReDim arrParam(0)
			strCommandText = "GET_InformationCodeName"
			arrParam(0) = New SqlParameter("@intLanguage", intLanguage)
		ElseIf intDropdown = 4 Then
			ReDim arrParam(0)
			strCommandText = "GET_ConservationTemperatureCodeName"
			arrParam(0) = New SqlParameter("@intLanguage", intLanguage)
		End If

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


    'ADR 05.02.11 - get Language list based on available translations
    Public Function GetListCodeNameCustom(ByVal intCodeSite As Integer, ByVal bytStatus As Byte, ByVal intCodeListe As Integer) As Object
        Dim strCommandText As String
        Dim arrParam(2) As SqlParameter

        strCommandText = "GET_TRANSLATIONCODENAMECUSTOM"
        arrParam(0) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@Status", bytStatus)
        arrParam(2) = New SqlParameter("@CodeListe", intCodeListe)

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

    Public Function GetListCodeNameCustomMuliple(ByVal intCodeSite As Integer, ByVal bytStatus As Byte, ByVal strCodeListe As String) As Object
        Dim strCommandText As String
        Dim arrParam(2) As SqlParameter

        strCommandText = "GET_TRANSLATIONCODENAMECUSTOMAdvance"
        arrParam(0) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@Status", bytStatus)
        arrParam(2) = New SqlParameter("@CodeListe", strCodeListe)

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
    ''' Fetch list of EGS Languages
    ''' </summary>
    ''' <param name="intUsed"></param>
    ''' <param name="intSelected"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function FetchLanguageList(ByVal intUsed As Integer, ByVal intSelected As Integer, Optional blnIgnoreNullCodeRef As Boolean = False) As Object
        'Optional ByVal strISOCode As String = "") As Object
        Dim strCommandText As String = "sp_EgswLanguageGetList"

        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@Selected", intSelected)
        arrParam(1) = New SqlParameter("@Used", intUsed)
        arrParam(2) = New SqlParameter("@bitIgnoreNullCodeRef", blnIgnoreNullCodeRef) 'AGL 2013.08.17 - 7622
        'If strISOCode <> "" Then _
        '   arrParam(2) = New SqlParameter("@nvcISOCode", SqlDbType.NVarChar, 10, strISOCode)

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
    ''' Delete 
    ''' </summary>
    ''' <param name="intCode"></param>
    ''' <param name="TrandMode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function DeleteFromList(ByVal intCode As Integer, ByVal strCodeTranslationList As String, ByVal TrandMode As enumEgswTransactionMode) As enumEgswErrorCode

        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@retval", "")
        arrParam(0).Direction = ParameterDirection.ReturnValue

        arrParam(1) = New SqlParameter("@intCode", intCode)
        arrParam(2) = New SqlParameter("@tntTranMode", TrandMode)
        arrParam(3) = New SqlParameter("@txtCodeList", strCodeTranslationList)
        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswTranslationDelete", arrParam)
            Return CType(arrParam(0).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try
    End Function
#End Region

#Region "Get Methods"
    ''' <summary>
    ''' Get one translation 
    ''' </summary>
    ''' <param name="intCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList(ByVal intCode As Integer) As Object
        Return Me.FetchList(intCode, -1, 255)
    End Function

    ''' <summary>
    ''' Get one translation 
    ''' </summary>
    ''' <param name="intcodesite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList(ByVal intCodeSite As Integer, ByVal bytStatus As Byte, ByVal intCodeToExclude As Integer) As DataTable
        Dim fetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataTable

        Dim dtTrans As DataTable = CType(FetchList(-1, intCodeSite, 255), DataTable)

        If dtTrans.Select("CodeTrans=" & intCodeToExclude).Length > 0 Then
            dtTrans.Rows.Remove(dtTrans.Select("CodeTrans=" & intCodeToExclude)(0))
            dtTrans.AcceptChanges()
        End If

        Return dtTrans
    End Function

    ''' <summary>
    ''' Get List of tranlsations
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList() As Object
        Return Me.FetchList(-1, -1, 255)
    End Function
    ''' <summary>
    ''' Get List of Translations shared to this site.
    ''' </summary>
    ''' <param name="intCodeSite"></param>
    ''' <param name="bytStatus"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetList(ByVal intCodeSite As Integer, ByVal bytStatus As Integer) As Object
        ' JRL 8.10.2005 Status is no not used. 
        Return Me.FetchList(-1, intCodeSite, 255)
    End Function


    ''' <summary>
    ''' Get List of EGS Languages
    ''' </summary>
    ''' <param name="intUsed"></param>
    ''' <param name="intSelected"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetLanguageList(Optional ByVal intUsed As Integer = 255, Optional ByVal intSelected As Integer = 255, Optional blnIgnoreNullCodeRef As Boolean = False) As Object
        Return Me.FetchLanguageList(intUsed, intSelected, blnIgnoreNullCodeRef)
    End Function

    ''' <summary>
    ''' Get if ISOCode exists
    ''' </summary>
    ''' <param name="strISOCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetLanguageList(ByVal strISOCode As String) As Object
        Dim strCommandText As String = "sp_EgswLanguageGetList"

        Try
            Dim arrParam(2) As SqlParameter
            arrParam(0) = New SqlParameter("@Selected", 255)
            arrParam(1) = New SqlParameter("@Used", 255)
            arrParam(2) = New SqlParameter("@nvcISOCode", strISOCode)

            Return CType(MyBase.ExecuteFetchType(enumEgswFetchType.DataReader, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam), SqlDataReader)

        Catch ex As Exception
            Throw ex
        End Try
    End Function


    Public Function GetTranslationName(ByVal intCodeTrans As Integer) As String
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim strName As String = ""

        With cmd
            .Connection = cn
            .CommandText = "Select @Name = NAME FROM EgsWTranslation where Code=@CodeTrans "
            .CommandType = CommandType.Text
            .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
            .Parameters.Add("@Name", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output
            Try
                cn.Open()
                .ExecuteNonQuery()
                strName = CStrDB(.Parameters("@Name").Value)
                cn.Close()
            Catch ex As Exception
                cn.Close()
                Throw ex
            End Try
        End With
        Return strName
    End Function

    Public Function GetMPTrans(ByVal intCodeSite As Integer, ByVal bytStatus As Byte, ByVal cLang As clsEGSLanguage) As DataTable 'VRP 31.10.2008
        'FOR GERMAN, FRENCH, ENGLISH ORDER 
        'FOR SV ONLY
        Dim strCommandText As String = "sp_EgswTranslationGetList"

        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@intCode", -1)
        arrParam(1) = New SqlParameter("@Status", bytStatus)
        arrParam(2) = New SqlParameter("@intCodeSite", intCodeSite)

        Try

            Dim dr As SqlDataReader = ExecuteReader(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
            Dim arrCode As New ArrayList
            arrCode.Add("2")
            arrCode.Add("3")
            arrCode.Add("1")

            Dim dt As New DataTable
            dt.Columns.Add("ID")
            dt.Columns.Add("Code")
            dt.Columns.Add("Name")

            Dim row As DataRow
            While dr.Read
                For i As Integer = 0 To arrCode.Count - 1
                    If CInt(dr.Item("CodeDictionary")) = CInt(arrCode(i)) Then
                        row = dt.NewRow
                        row("ID") = i + 1
                        row("Code") = CInt(dr.Item("Code"))

                        Select Case CInt(dr.Item("CodeDictionary"))
                            Case 1 : row("Name") = cLang.GetString(clsEGSLanguage.CodeType.English)
                            Case 2 : row("Name") = cLang.GetString(clsEGSLanguage.CodeType.German)
                            Case 3 : row("Name") = cLang.GetString(clsEGSLanguage.CodeType.French)
                            Case 4 : row("Name") = cLang.GetString(clsEGSLanguage.CodeType.Italian)
                            Case Else : row("Name") = dr.Item("name").ToString
                        End Select
                        dt.Rows.Add(row)
                        Exit For
                    End If
                Next
            End While
            dr.Close()
            Dim dv As New DataView(dt)
            dv.Sort = "ID ASC"
            Return dv.ToTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetCodeTrans(ByVal intCodeDict As Integer) As Integer

        Dim cmd As New SqlCommand
        Dim cn As New SqlConnection(L_strCnn)
        Dim strCommandText As String = "SELECT Code FROM EgswTranslation WHERE CodeDictionary=" & intCodeDict
        Dim intCodeTrans As Integer
        Try
            With cmd
                .Connection = cn
                .CommandType = CommandType.Text
                .CommandText = strCommandText
                .Connection.Open()
                intCodeTrans = CInt(.ExecuteScalar())
                .Connection.Close()
                .Dispose()
            End With
            Return intCodeTrans
        Catch
            Return Nothing
        End Try
    End Function

    Public Function GetCodeLang(ByVal intCodeTrans As Integer) As Integer
        Dim cmd As New SqlCommand
        Dim cn As New SqlConnection(L_strCnn)
        Dim strCommandText As String = "SELECT CodeDictionary FROM EgswTranslation WHERE Code=" & intCodeTrans
        Dim intCodeLang As Integer
        Try
            With cmd
                .Connection = cn
                .CommandType = CommandType.Text
                .CommandText = strCommandText
                .Connection.Open()
                intCodeLang = CInt(.ExecuteScalar())
                .Connection.Close()
                .Dispose()
            End With
            Return intCodeLang
        Catch
            Return Nothing
        End Try
    End Function

    Public Function GetLanguageRightToLeft(ByVal codeLang) As Boolean
        Try
            Dim dt As DataTable = GetLanguageList(1)

            For Each l In dt.Select("Code='" & codeLang & "'")
                If l("RightToLeft") Then Return True
            Next
        Catch ex As Exception
            Return False
        End Try

        Return False
    End Function

#End Region

#Region "Remove Methods"

    ''' <summary>
    ''' Delete one item from the list
    ''' </summary>
    ''' <param name="intCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteFromList(ByVal intCode As Integer) As enumEgswErrorCode
        Return DeleteFromList(intCode, "", enumEgswTransactionMode.Delete)
    End Function

#End Region

#Region " Other Function "

    Public Function GetOne(ByVal intCode As Integer) As DataRow
        Dim tempFetchType As enumEgswFetchType = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataSet
        Dim ds As DataSet = CType(GetList(intCode), DataSet)
        L_bytFetchType = tempFetchType

        Dim dt As DataTable = ds.Tables(0)
        If dt.DefaultView.Count = 0 Then Return Nothing
        Return dt.Rows(0)
    End Function

    Public Function GetCodeDictionary(ByVal intCodeTrans As Integer) As Integer
        Dim rw As DataRow = GetOne(intCodeTrans)
        If rw Is Nothing Then
            Return 0
        Else
            Return CInt(rw("codeDictionary"))
        End If
    End Function

#End Region
End Class

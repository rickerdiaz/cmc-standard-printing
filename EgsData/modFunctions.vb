Imports System.Text
Imports System.Security.Cryptography
Imports System.IO
Imports System.Reflection
Imports System.Configuration
Imports System.Web
Imports System.Globalization

Public Module modFunctions
    Private Const BASE32 As String = "0123456789ABCDEFGHJKLMNPRSTUWXYZ"

    ''' <summary>
    ''' Fill 2-pair values from Datatable to Hash
    ''' </summary>
    ''' <param name="KeyField"></param>
    ''' <param name="ValueField"></param>
    ''' <param name="Data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FillHash(ByVal KeyField As String, ByVal ValueField As String, ByVal Data As DataTable) As Hashtable
        Dim row As DataRow
        Dim hash As New Hashtable
        'jhl Dim sKeyField As String
        Dim sKeyFieldValue As String
        Dim sValueFieldValue As String

        For Each row In Data.Rows
            sKeyFieldValue = row.Item(KeyField).ToString  'jhl
            sValueFieldValue = row.Item(ValueField).ToString  'jhl 
            If Not hash.Contains(sKeyFieldValue.ToLower) Then
                hash.Add(sKeyFieldValue.ToLower, sValueFieldValue.ToLower)
            End If
        Next

        Return hash
    End Function

    Public Function FillHash(ByVal KeyField As String, ByVal ValueField As String, ByVal reader As SqlClient.SqlDataReader) As Hashtable
        Dim hash As New Hashtable
        Dim sKeyFieldValue As String
        Dim sValueFieldValue As String

        While reader.Read
            sKeyFieldValue = reader(KeyField).ToString
            sValueFieldValue = reader.Item(ValueField).ToString
            If Not hash.Contains(sKeyFieldValue.ToLower) Then
                hash.Add(sKeyFieldValue.ToLower, sValueFieldValue.ToLower)
            End If
        End While
        reader.Close()
        Return hash
    End Function

    ''' <summary>
    ''' One way Encryption
    ''' </summary>
    ''' <param name="strTextToHash"></param>
    ''' <returns></returns>
    ''' <remarks>Used in Password</remarks>
    Public Function ConvertTextToHash(ByVal strTextToHash As String) As String
        Dim saltAsString As String = "23sd$&*HF"
        Dim byteRepresentation() As Byte = UnicodeEncoding.UTF8.GetBytes(strTextToHash + saltAsString)
        Dim hashedTextInBytes() As Byte = Nothing
        Dim myMD5 As MD5CryptoServiceProvider = New MD5CryptoServiceProvider()
        hashedTextInBytes = myMD5.ComputeHash(byteRepresentation)
        Dim hashedText As String = Convert.ToBase64String(hashedTextInBytes)
        Return hashedText
    End Function

    Public Function FillHash(ByVal KeyField As String, ByVal ValueField As String, ByVal dv As DataView) As Hashtable
        Dim row As DataRowView
        Dim hash As New Hashtable
        Dim sKeyFieldValue As String
        Dim sValueFieldValue As String

        For Each row In dv
            sKeyFieldValue = row.Item(KeyField).ToString
            sValueFieldValue = row.Item(ValueField).ToString
            If Not hash.Contains(sKeyFieldValue.ToLower) Then
                hash.Add(sKeyFieldValue.ToLower, sValueFieldValue.ToLower)
            End If
        Next


        Return hash
    End Function

    ''' <summary>
    ''' Returns string value. Null value is converted to Empty string.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CStrDB(ByVal value As Object) As String
        'If value Is Nothing Or IsDBNull(value) Then Return "" Else Return CStr(value) 
        ' RBAJ-2012.11.21
        Return GetStr(value)
    End Function

    ''' <summary>
    ''' Returns string value. Null value is converted to Empty string.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CDateDB(ByVal value As Object) As DateTime
        'Try
        '    If value Is Nothing Or IsDBNull(value) Then Return #1/1/1753# Else Return CDate(value)
        'Catch ex As Exception
        '    Return #1/1/1753#
        'End Try
        ' RBAJ-2012.11.21
        Return GetDate(value)
    End Function

    ''' <summary>
    ''' Returns Integer value of object. If Value is nothing, returns 0.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CIntDB(ByVal value As Object) As Integer
        'MKAM 2016.04.26
        If value IsNot Nothing AndAlso value.GetType Is GetType(Boolean) Then
            Return IIf(value, 1, 0)
        Else
            Return GetInt(value)
        End If

        'If value Is Nothing Then
        '    Return 0
        'ElseIf IsDBNull(value) Then
        '    Return 0
        'Else
        '    Return CInt(value)
        'End If
        ' RBAJ-2012.11.21
        'Return GetInt(value)
    End Function

    ''' <summary>
    ''' Returns double value of object. If Value is nothing, returns 0.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CDblDB(ByVal value As Object) As Double
        'If value Is Nothing Or value Is "" Then
        '    Return 0
        'ElseIf IsDBNull(value) Then
        '    Return 0
        'Else
        '    Return CDbl(value)
        'End If
        ' RBAJ-2012.11.21
        Return GetDbl(value)
    End Function

    ''' <summary>
    ''' Returns Boolean value. If value is NULL, returns FALSE.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CBoolDB(ByVal value As Object) As Boolean
        'If value Is Nothing Then
        '    Return False
        'ElseIf IsDBNull(value) Then
        '    Return False
        'ElseIf value Is "" Then
        '    Return False
        'Else
        '    Return CBool(value)
        'End If
        ' RBAJ-2012.11.21
        Return GetBool(value)
    End Function

    Public Function fctNullToZeroDBL(ByVal value As Object, Optional ByVal dblDefault As Double = 0) As Double
        'If value Is Nothing Then
        '    Return dblDefault
        'ElseIf Not IsNumeric(value) Then
        '    Return dblDefault
        'Else
        '    Return CDbl(value)
        'End If
        Return GetDbl(value, dblDefault)
    End Function

    Public Function fctNullToBool(ByVal value As Object, Optional ByVal bDef As Boolean = False) As Boolean
        'Try
        '    If value Is Nothing Then
        '        Return bDef
        '    Else
        '        Return CBool(value)
        '    End If
        'Catch ex As Exception
        '    Return bDef
        'End Try
        Return GetBool(value, bDef)
    End Function

    Public Function fctNullToZero(ByVal value As Object, Optional ByVal dblDefault As Integer = 0) As Integer
        'If value Is Nothing Then
        '    Return dblDefault
        'ElseIf Not IsNumeric(value) Then
        '    Return dblDefault
        'Else
        '    Return CInt(value)
        'End If
        Return GetInt(value, dblDefault)
    End Function

    ''' <summary>
    ''' Convert Base 32 to 10
    ''' </summary>
    ''' <param name="CurStr"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function fct32To10(ByVal CurStr As String) As Integer
        Dim nbIter As Integer
        Dim i As Integer
        Dim j As Integer
        Dim TmpNb As Integer
        Dim SumNb As Integer
        Dim CarStr As String
        Dim PosInStr As Integer
        j = 0
        nbIter = Len(CurStr)
        For i = nbIter To 1 Step -1
            CarStr = Mid$(CurStr, i, 1)
            PosInStr = InStr(1, BASE32, CarStr, 0) - 1
            TmpNb = CInt(PosInStr * (32 ^ j))
            SumNb = SumNb + TmpNb
            j = j + 1
        Next 'I
        fct32To10 = SumNb
    End Function

    ''' <summary>
    ''' Convert Energy KJ to Kcal 
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ConvertKjtoKcal(ByVal value As Double) As Double
        'Return value / 4.182 '' WRONG! it should be 4.184
        Return value / 4.184
    End Function

    ''' <summary>
    ''' Get part of a string
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="index"></param>
    ''' <param name="delimiter"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseString(ByVal value As String, ByVal index As Integer, ByVal delimiter As Char) As String
        Dim values() As String = value.Split(delimiter)
        If (values.Length - 1) >= index Then
            Return values(index)
        Else
            Return ""
        End If
    End Function

    ''' <summary>
    ''' Remove Symbols
    ''' </summary>
    ''' <param name="sValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RemoveSymbols(ByVal sValue As String) As String
        Dim counter As Integer
        Dim nLastIndex As Integer = sValue.Length
        Dim c As Char
        Dim nAscii As Integer
        Dim sReturn As String = ""

        For counter = 1 To nLastIndex
            c = CChar(Mid(sValue, counter, 1))
            nAscii = Asc(c)
            Select Case nAscii
                Case Is = 1
                Case Else
                    sReturn = sReturn & c
            End Select
        Next

        Return sReturn
    End Function

    ''' <summary>
    ''' Returns TRUE if email is valid. Performs basic email validation.
    ''' </summary>
    ''' <param name="sEmail"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsValidEmail(ByVal sEmail As String) As Boolean
        ' Perform basic email validation
        If sEmail.IndexOf("@") < 0 Then
            Return False
        End If

        If sEmail.IndexOf(".") < 0 Then
            Return False
        End If

        If sEmail.Length <= 5 Then
            Return False
        End If

        Return True
    End Function

    Public Function fctTransformStrSearch(ByVal strText As String) As String
        If strText.Length = 0 Then Return ""


        Dim blnOpen As Boolean
        Dim blnClose As Boolean

        blnOpen = False
        blnClose = False

        If InStr(1, strText, "[") > 0 Then blnOpen = True
        If InStr(1, strText, "]") > 0 Then blnClose = True

        'strText = Trim(strText)
        strText = Replace(strText, "[", "[[]")
        strText = Replace(strText, "%", "[%]")
        '    strText = Replace(strText, "_", "[_]")     'RDTC March 6 2003

        '     strText = Replace(strText, "'", "''")

        strText = fctAccentedToNormalCharacters(strText, "AÀÁÂÃÄÅaàáâãäå", "A")
        strText = fctAccentedToNormalCharacters(strText, "EÈÉÊËeèéêë", "E")
        strText = fctAccentedToNormalCharacters(strText, "IÌÍÎÏiìíîï", "I")
        strText = fctAccentedToNormalCharacters(strText, "OÒÓÔÕÖØoòóôõöø", "O")
        strText = fctAccentedToNormalCharacters(strText, "UÙÚÛÜuùúûü", "U")
        strText = fctAccentedToNormalCharacters(strText, "NÑnñ", "N")
        strText = fctAccentedToNormalCharacters(strText, "SŠsš", "S")
        strText = fctAccentedToNormalCharacters(strText, "YÝyýÿ", "Y")

        'characters with various accents will be treated the same
        'prior to this all accents are changed to AEIOUSNY
        'RDTC Sept 8 2003
        strText = Replace(strText, "A", "[AÀÁÂÃÄÅaàáâãäå]")
        strText = Replace(strText, "E", "[EÈÉÊËeèéêë]")
        strText = Replace(strText, "I", "[IÌÍÎÏiìíîï]")
        strText = Replace(strText, "O", "[OÒÓÔÕÖØoòóôõöø]")
        strText = Replace(strText, "U", "[UÙÚÛÜuùúûü]")
        strText = Replace(strText, "N", "[NÑnñ]")
        strText = Replace(strText, "S", "[SŠsš]")
        strText = Replace(strText, "Y", "[YÝyýÿ]")

        fctTransformStrSearch = strText
    End Function

    ''-- JBB 06.15.2012
    Public Function FullTextContainCompatibility(ByVal strData As String, ByVal intType As Integer, ByRef IsModify As Boolean) As String
        
        'strData = strData.Replace(",", " ")
        Dim strResult As String = ""
        Dim strSplit() As String = strData.Split(",")
        If strSplit.Length > 1 Then
            For intIndex As Integer = 0 To UBound(strSplit)
                If strSplit(intIndex) <> "" Then
                    If strSplit(intIndex).Contains("(") Or strSplit(intIndex).Contains(")") Or strSplit(intIndex).Contains("[") Or strSplit(intIndex).Contains("]") Or strSplit(intIndex).Trim().Contains(" ") Then
                        strSplit(intIndex) = strSplit(intIndex).Replace("""", "")
                        If strResult = "" Then
                            strResult = """*" + strSplit(intIndex).Trim() + "*"""
                        Else
                            strResult = strResult & IIf(intType = 1, " AND ", " OR ") & """*" & strSplit(intIndex).Trim() & "*"""
                        End If
                    Else
                        strSplit(intIndex) = strSplit(intIndex).Trim().Replace("""", "")
                        If strResult = "" Then
                            strResult = """*" & strSplit(intIndex).Trim() & "*"""
                        Else
                            strResult = strResult & IIf(intType = 1, " AND ", " OR ") & """*" & strSplit(intIndex).Trim() & "*"""
                        End If
                    End If
                End If
                'If strSplit(intIndex) <> "" Then
                '    If strSplit(intIndex).Contains("(") Or strSplit(intIndex).Contains(")") Or strSplit(intIndex).Contains("[") Or strSplit(intIndex).Contains("]") Or strSplit(intIndex).Contains(" ") Then
                '        strSplit(intIndex) = strSplit(intIndex).Replace("""", "")
                '        If strResult = "" Then
                '            strResult = """*" + strSplit(intIndex) + "*"""
                '        Else
                '            strResult = strResult & IIf(intType = 1, " AND ", " OR ") & """*" & strSplit(intIndex) & "*"""
                '        End If
                '    Else
                '        strSplit(intIndex) = strSplit(intIndex).Replace("""", "")
                '        If strResult = "" Then
                '            strResult = """*" & strSplit(intIndex) & "*""" 'AGL 2012.11.13 - CWM-1801 - added double-quotes
                '        Else
                '            strResult = strResult & IIf(intType = 1, " AND ", " OR ") & """*" & strSplit(intIndex) & "*""" 'AGL 2012.11.13 - CWM-1801 - added double-quotes
                '        End If
                '    End If
                'End If
            Next
            IsModify = True
        Else
            strResult = strData
            IsModify = False
        End If
        Return strResult

        ''strData = strData.Replace(",", " ")
        'Dim strResult As String = ""
        'Dim strSplit() As String = strData.Split(",")
        'If strSplit.Length > 1 Then

        '    For intIndex As Integer = 0 To UBound(strSplit)
        '        strSplit(intIndex) = strSplit(intIndex).Trim
        '        If strSplit(intIndex) <> "" Then
        '            If strSplit(intIndex).Contains("(") Or strSplit(intIndex).Contains(")") Or strSplit(intIndex).Contains("[") Or strSplit(intIndex).Contains("]") Or strSplit(intIndex).Contains(" ") Then
        '                If strResult = "" Then
        '                    strResult = """*" + strSplit(intIndex) + "*"""
        '                Else
        '                    strResult = strResult & IIf(intType = 1, " AND ", " OR ") & """*" & strSplit(intIndex) & "*"""

        '                End If
        '            Else
        '                If strResult = "" Then
        '                    strResult = """*" & strSplit(intIndex) & "*""" 'AGL 2012.11.13 - CWM-1801 - added double-quotes
        '                Else
        '                    strResult = strResult & IIf(intType = 1, " AND ", " OR ") & """*" & strSplit(intIndex) & "*""" 'AGL 2012.11.13 - CWM-1801 - added double-quotes
        '                End If
        '            End If
        '        End If
        '    Next
        'Else
        '    strResult = strData
        'End If
        'Return strResult
    End Function
    ''--

    'Replace accented characters with "normal capital letters"
    'RDTC Sept 8 2003
    'strX is the the whole string, strAccents is a string of "equivalent" letters regardless of accent
    'strNormal is the character by which the accented characters are to be replaced with
    Function fctAccentedToNormalCharacters(ByVal strX As String, ByVal strAccents As String, ByVal strNormal As String) As String
        Dim i As Integer
        Dim strText As String

        strText = strX
        For i = 1 To Len(strAccents)
            If InStr(1, strText, Mid(strAccents, i, 1)) > 0 Then
                strText = Replace(strText, Mid(strAccents, i, 1), strNormal)
            End If
        Next

        fctAccentedToNormalCharacters = strText

    End Function

    ''' <summary>
    ''' Fetch 3-column display of liste. Source datatable is taken from Search function (clsListe).
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FormatDataTable(ByVal dt As DataTable) As DataTable
        Dim nTotal As Integer = dt.Rows.Count
        Dim counter As Integer = 0
        Dim counter2 As Integer = 0
        Dim nMaxCounter1 As Integer = (nTotal - 1)
        Dim row As DataRow
        Dim dtNew As New DataTable


        With dtNew
            .Columns.Add("code1")
            .Columns.Add("name1")
            .Columns.Add("language1")
            .Columns.Add("sOwner1")
            .Columns.Add("code2")
            .Columns.Add("name2")
            .Columns.Add("language2")
            .Columns.Add("sOwner2")
            .Columns.Add("code3")
            .Columns.Add("name3")
            .Columns.Add("language3")
            .Columns.Add("sOwner3")

            For counter = 0 To nMaxCounter1 Step 3

                If counter > nMaxCounter1 Then Exit For

                row = dtNew.NewRow
                'Column set 1
                row.Item("code1") = dt.Rows(counter).Item("code")
                row.Item("name1") = dt.Rows(counter).Item("name")
                row.Item("language1") = dt.Rows(counter).Item("codetrans")
                'row.Item("sOwner1") = dt.Rows(counter).Item("sowner")
                row.Item("sOwner1") = dt.Rows(counter).Item("IsOwner")

                'Column set 2
                If counter + 1 <= nMaxCounter1 Then
                    row.Item("code2") = dt.Rows(counter + 1).Item("code")
                    row.Item("name2") = dt.Rows(counter + 1).Item("name")
                    row.Item("language2") = dt.Rows(counter + 1).Item("codetrans")
                    'row.Item("sOwner2") = dt.Rows(counter + 1).Item("sowner")
                    row.Item("sOwner2") = dt.Rows(counter + 1).Item("IsOwner")
                End If

                'Column set 3
                If counter + 2 <= nMaxCounter1 Then
                    row.Item("code3") = dt.Rows(counter + 2).Item("code")
                    row.Item("name3") = dt.Rows(counter + 2).Item("name")
                    row.Item("language3") = dt.Rows(counter + 2).Item("codetrans")
                    'row.Item("sOwner3") = dt.Rows(counter + 2).Item("sowner")
                    row.Item("sOwner3") = dt.Rows(counter + 2).Item("IsOwner")
                End If
                dtNew.Rows.Add(row)
            Next

        End With

        Return dtNew

    End Function

    Public Sub ParseQtyUnitIngrFromText(ByVal strText As String, ByRef dblQty As Double, ByRef strUnit As String, ByRef strIngr As String, blnUseFractions As Boolean)
        'vbv 29.12.2005 - Converted to .Net
        Dim strTemp As String
        Dim i As Integer
        Dim j As Integer
        Dim strQty As String = ""
        Dim strQtyUnit As String
        Dim strAllowedNumericChars() As Char = {Chr(Asc("1")), Chr(Asc("2")), Chr(Asc("3")), Chr(Asc("4")), Chr(Asc("5")), Chr(Asc("6")), Chr(Asc("7")), Chr(Asc("8")), Chr(Asc("9")), Chr(Asc("0")), Chr(Asc(".")), Chr(Asc(",")), Chr(Asc("'")), Chr(Asc("/"))}

        strTemp = Trim(strText)
        If strTemp.Contains(Space(1)) Then
            i = strTemp.IndexOf(Space(1))
            strQtyUnit = strTemp.Substring(0, i)
            strIngr = strTemp.Substring(i + 1)

            j = strQtyUnit.LastIndexOfAny(strAllowedNumericChars)
            If j > -1 Then strQty = strQtyUnit.Substring(0, j)
            strUnit = strQtyUnit.Substring(j + 1).Trim
            If blnUseFractions Then
                dblQty = fctFraction2(strQty)
            Else
                dblQty = strQty
            End If

        End If

        If strQty = "" Then
            strIngr = strTemp
            strUnit = ""
            dblQty = -1
        End If

    End Sub

    Public Function fctFraction(ByVal strPValeur As String, blnUseFractions As Boolean) As Double


        'AGL 2013.05.06 - use fractions
        If blnUseFractions = False Then
            If IsNumeric(strPValeur) Then
                Return strPValeur
            Else
                Return 0
            End If
        End If
        Dim p As Integer
        Dim PS As Integer
        Dim WholeStr As String
        Dim FracStr As String
        Dim NumStr As String
        Dim DenomStr As String
        Dim intNum As Double
        Dim intDen As Double
        Dim dblValueVar As Double

        On Error GoTo ErrHandlerFraction
        fctFraction = 0
        strPValeur = Trim$(strPValeur)
        If (InStr(strPValeur, "/")) > 0 Then
            PS = InStr(strPValeur, " ")
            If PS > 0 Then   'if there is a space then there is a whole number
                WholeStr = Trim$(Mid$(strPValeur, 1, PS - 1))
                FracStr = Trim$(Mid$(strPValeur, PS + 1))
                strPValeur = FracStr
                dblValueVar = Val(WholeStr)
            End If
            p = InStr(strPValeur, "/")
            NumStr = Trim$(Mid$(strPValeur, 1, p - 1))
            DenomStr = Trim$(Mid$(strPValeur, p + 1))
            intNum = Val(NumStr)
            intDen = Val(DenomStr)
            dblValueVar = dblValueVar + (intNum / intDen)
        Else
            dblValueVar = CDbl(strPValeur)       ' whole number
        End If
        fctFraction = dblValueVar            '*sg 17/05/99 fixes problem with decimal separator

ExitFunctionNow:
        Exit Function

ErrHandlerFraction:
        On Error GoTo 0
        GoTo ExitFunctionNow
    End Function

    Public Function fctFraction2(ByVal strPValeur As String) As Double


        Dim p As Integer
        Dim PS As Integer
        Dim WholeStr As String
        Dim FracStr As String
        Dim NumStr As String
        Dim DenomStr As String
        Dim intNum As Double
        Dim intDen As Double
        Dim dblValueVar As Double

        On Error GoTo ErrHandlerFraction
        fctFraction2 = 0
        strPValeur = Trim$(strPValeur)
        If (InStr(strPValeur, "/")) > 0 Then
            PS = InStr(strPValeur, " ")
            If PS > 0 Then   'if there is a space then there is a whole number
                WholeStr = Trim$(Mid$(strPValeur, 1, PS - 1))
                FracStr = Trim$(Mid$(strPValeur, PS + 1))
                strPValeur = FracStr
                dblValueVar = Val(WholeStr)
            End If
            p = InStr(strPValeur, "/")
            NumStr = Trim$(Mid$(strPValeur, 1, p - 1))
            DenomStr = Trim$(Mid$(strPValeur, p + 1))
            intNum = Val(NumStr)
            intDen = Val(DenomStr)
            dblValueVar = dblValueVar + (intNum / intDen)
        Else
            dblValueVar = CDbl(strPValeur)       ' whole number
        End If
        fctFraction2 = dblValueVar            '*sg 17/05/99 fixes problem with decimal separator

ExitFunctionNow:
        Exit Function

ErrHandlerFraction:
        On Error GoTo 0
        GoTo ExitFunctionNow
    End Function

    Public Function fctReplaceWildChars(ByVal strValue As String) As String

        fctReplaceWildChars = strValue

        fctReplaceWildChars = Replace(fctReplaceWildChars, "\\", " ")
        fctReplaceWildChars = Replace(fctReplaceWildChars, "(", " ")
        fctReplaceWildChars = Replace(fctReplaceWildChars, ")", " ")
        fctReplaceWildChars = Replace(fctReplaceWildChars, "/", " ")
        fctReplaceWildChars = Replace(fctReplaceWildChars, "-", " ")
        fctReplaceWildChars = Replace(fctReplaceWildChars, ",", " ")
        fctReplaceWildChars = Replace(fctReplaceWildChars, ">", " ")
        fctReplaceWildChars = Replace(fctReplaceWildChars, "<", " ")
        fctReplaceWildChars = Replace(fctReplaceWildChars, "-", " ")
        fctReplaceWildChars = Replace(fctReplaceWildChars, "&", " ")
        fctReplaceWildChars = Replace(fctReplaceWildChars, "'", "")
        fctReplaceWildChars = Replace(fctReplaceWildChars, " ", "")

    End Function

    Public Function Encrypt(ByVal strText As String) As String 'VRP 15.05.2008
        Dim byKey() As Byte = {}
        Dim IV() As Byte = {&H12, &H34, &H56, &H78, &H90, &HAB, &HCD, &HEF}

        Try
            byKey = System.Text.Encoding.UTF8.GetBytes(Left("&%#@?,:*", 8))
            Dim des As New DESCryptoServiceProvider()
            Dim inputByteArray() As Byte = Encoding.UTF8.GetBytes(strText)
            Dim ms As New MemoryStream()
            Dim cs As New CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write)
            cs.Write(inputByteArray, 0, inputByteArray.Length)
            cs.FlushFinalBlock()
            Return Convert.ToBase64String(ms.ToArray())
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Function EncryptSHA256(ByVal strText As String) As String 'VRP 15.05.2008
        Dim byKey() As Byte = {}
        Dim IV() As Byte = {&H12, &H34, &H56, &H78, &H90, &HAB, &HCD, &HEF}

        Try
            byKey = System.Text.Encoding.UTF8.GetBytes(Left("&%#@?,:*", 8))
            Dim des As New SHA256Managed()
            Dim bytClearString() As Byte = Encoding.UTF8.GetBytes(strText)
            Dim sha As New  _
            System.Security.Cryptography.SHA256Managed()
            Dim hash() As Byte = sha.ComputeHash(bytClearString)
            Return Convert.ToBase64String(hash)
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Function Decrypt(ByVal strText As String) As String 'VRP 15.05.2008
        Dim byKey() As Byte = {}
        Dim IV() As Byte = {&H12, &H34, &H56, &H78, &H90, &HAB, &HCD, &HEF}
        Dim inputByteArray(strText.Length) As Byte

        Try
            byKey = System.Text.Encoding.UTF8.GetBytes(Left("&%#@?,:*", 8))
            Dim des As New DESCryptoServiceProvider()
            inputByteArray = Convert.FromBase64String(strText)
            Dim ms As New MemoryStream()
            Dim cs As New CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write)

            cs.Write(inputByteArray, 0, inputByteArray.Length)
            cs.FlushFinalBlock()
            Dim encoding As System.Text.Encoding = System.Text.Encoding.UTF8

            Return encoding.GetString(ms.ToArray())

        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Function fctEliminateZeroArrayElement(arr As ArrayList)
        For Each obj As Object In arr
            If obj <= 0 Then
                arr.Remove(obj)
                Return arr
            End If
        Next
        Return arr
    End Function

    ''' <summary>
    ''' Copy a directory's contents into another directory
    ''' </summary>
    ''' <param name="strSourceDirectoryPath">The source directory</param>
    ''' <param name="strDestinationDirectoryPath">The destination directory</param>
    ''' <remarks>AGL 2012.10.15 - CWM-1706 - added procedure to copy media files from source liste</remarks>
    Public Sub CopyDirectoryContents(strSourceDirectoryPath As String, strDestinationDirectoryPath As String)
        Try
            If Not strSourceDirectoryPath.EndsWith("\") Then
                strSourceDirectoryPath &= "\"
            End If

            If Not strDestinationDirectoryPath.EndsWith("\") Then
                strDestinationDirectoryPath &= "\"
            End If

            If System.IO.Directory.Exists(strSourceDirectoryPath) = True Then
                'Create new directory if not existing
                If System.IO.Directory.Exists(strDestinationDirectoryPath) = False Then
                    System.IO.Directory.CreateDirectory(strDestinationDirectoryPath)
                End If

                'Enumerate Files
                Dim strFilesWithin As String() = System.IO.Directory.GetFiles(strSourceDirectoryPath)
                For Each strFile As String In strFilesWithin
                    System.IO.File.Copy(strFile, strDestinationDirectoryPath & Replace(strFile, strSourceDirectoryPath, ""))
                Next

            End If
        Catch ex As Exception

        End Try

    End Sub

    ''' <summary>
    ''' Gets a string.
    ''' </summary>
    ''' <param name="value">The raw string.</param>
    ''' <param name="def">The default value, returned when the raw string is <c>null</c>.</param>
    ''' <returns>The result.</returns>
    ''' <remarks>RBAJ 2012.07.04</remarks>
    Public Function GetStr(ByVal value As Object, Optional ByVal def As String = "") As String
        If value Is Nothing Then
            Return def
        ElseIf IsDBNull(value) Then
            Return def
        Else
            Return String.Concat(String.Empty, value)
        End If
    End Function

    ''' <summary>
    ''' Gets a date.
    '''  #1/1/1900# - #4/30/1900#
    ''' </summary>
    ''' <param name="value">The string value.</param>
    ''' <param name="def">The default value, returned when parsing fails.</param>
    ''' <returns>The result.</returns>
    ''' <remarks>RBAJ 2012.07.04</remarks>
    'Public Function GetDate(ByVal value As Object, Optional ByVal def As DateTime = #4/30/1900#) As DateTime
    '    If value Is Nothing Then
    '        Return def
    '    End If

    '    Dim i As DateTime = def

    '    'JTOC 11.30.2013
    '    If Not IsDate(value) Then
    '        If value.ToString <> "" Then
    '            Try
    '                i = DateTime.Parse(value.ToString, New Globalization.CultureInfo("en-US"), Globalization.DateTimeStyles.None)
    '            Catch ex As Exception
    '                i = def
    '            End Try
    '        Else
    '            i = def
    '        End If
    '    Else

    '        'Dim ci As System.Globalization.CultureInfo = Nothing
    '        'Dim dtfi As System.Globalization.DateTimeFormatInfo = Nothing

    '        '' Instantiate a culture using CreateSpecificCulture.
    '        'ci = System.Globalization.CultureInfo.CreateSpecificCulture("en-US")
    '        'ci.DateTimeFormat.FullDateTimePattern = "mm/dd/yyyy"


    '        'If Not DateTime.TryParse(GetStr(value), New Globalization.CultureInfo("en-US"), Globalization.DateTimeStyles.None, i) Then
    '        '	i = def
    '        'End If
    '        i = value 'JTOC 12.16.2013
    '    End If

    '    Return i
    'End Function
    Public Function GetDate(ByVal value As Object, Optional ByVal def As DateTime = #4/30/1900#) As DateTime
        If value Is Nothing Then
            Return def
        End If

        Dim text As String = value.ToString().Trim()
        If text = "" Then
            Return def
        End If

        Dim result As DateTime

        ' Try to parse using en-US consistently
        If DateTime.TryParse(text,
                     New System.Globalization.CultureInfo("en-US"),
                     System.Globalization.DateTimeStyles.None,
                     result) Then
            Return result
        Else
            Return def
        End If
    End Function


    ''' <summary>
    ''' Gets an integer.
    ''' </summary>
    ''' <param name="value">The string value.</param>
    ''' <param name="def">The default value, returned when parsing fails.</param>
    ''' <returns>The result.</returns>
    ''' <remarks>RBAJ 2012.07.04</remarks>
    Public Function GetInt(ByVal value As Object, Optional ByVal def As Integer = 0) As Integer
        If value Is Nothing Then
            Return def
        End If
        Dim i As Integer = def
        If Not Integer.TryParse(GetStr(value), i) Then
            i = def
        End If
        Return i
    End Function

    ''' <summary>
    ''' Gets a double.
    ''' </summary>
    ''' <param name="value">The string value.</param>
    ''' <param name="def">The default value, returned when parsing fails.</param>
    ''' <returns>The result.</returns>
    ''' <remarks>RBAJ 2012.07.04</remarks>
    Public Function GetDbl(ByVal value As Object, Optional ByVal def As Double = 0.0R) As Double
        If value Is Nothing Then
            Return def
        End If
        Dim i As Double = def
        If Not Double.TryParse(GetStr(value), i) Then
            i = def
        End If
        Return i
    End Function

    ''' <summary>
    ''' Gets a boolean.
    ''' </summary>
    ''' <param name="value">The string value.</param>
    ''' <param name="def">The default value, returned when parsing fails.</param>
    ''' <returns>The result.</returns>
    ''' <remarks>RBAJ 2012.07.04</remarks>
    Public Function GetBool(ByVal value As Object, Optional ByVal def As Boolean = False) As Boolean
        If value Is Nothing Then
            Return def
        Else
            If GetStr(value).ToLowerInvariant() = "yes" Then
                Return True
            ElseIf GetStr(value) = "1" Then
                Return True
            End If
            Dim b As Boolean = def
            If Not Boolean.TryParse(GetStr(value), b) Then
                b = def
            End If
            Return b
        End If
    End Function

    ''' <summary>
    ''' Prints a boolean.
    ''' </summary>
    ''' <param name="value">The value.</param>
    ''' <returns>The string result.</returns>
    ''' <remarks>RBAJ 2012.07.04</remarks>
    Public Function PrintBool(ByVal value As Boolean) As String
        Return CStr(IIf(value, "yes", "no"))
    End Function

    ''' <summary>
    ''' Preserve previous stack trace when re-throwing exceptions
    ''' </summary>
    ''' <param name="exception">Represent errors that occur during application execution.</param>
    ''' <remarks>RBAJ 2012.07.04</remarks>
    Public Sub PreserveStackTrace(exception As Exception)
        Dim preserveStackTrace As MethodInfo = GetType(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance Or BindingFlags.NonPublic)
        preserveStackTrace.Invoke(exception, Nothing)
    End Sub
End Module

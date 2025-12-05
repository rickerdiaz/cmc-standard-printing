Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Text
Imports System.IO
Imports System.Collections
Imports Microsoft.VisualBasic.FileIO
'Imports Spire.Xls


'Public Class EgswTables

'    Public Structure structListe
'        Public Code As Integer
'        Public CodeSite As Integer
'        Public CodeUser As Integer
'        Public Type As Integer
'        Public Name As String
'        Public Number As String
'        Public Brand As Integer
'        Public Category As Integer
'        Public Source As Integer
'        Public Supplier As Integer
'        Public Yield As Double
'        Public YieldUnit As Integer
'        Public Dates As Date
'        Public Percent As Integer
'        Public srQty As Double
'        Public srWeight As Double
'        Public srUnit As Integer
'        Public PictureName As String
'        Public Note As String
'        Public Remark As String
'        Public Wastage1 As Integer
'        Public Wastage2 As Integer
'        Public Wastage3 As Integer
'        Public Wastage4 As Integer
'        Public CoolingTime As String
'        Public HeatingTime As String
'        Public HeatingTemperature As String
'        Public HeatingMode As String
'        Public CCPDescription As String
'        Public Description As String
'        Public Ingredients As String
'        Public Preparation As String
'        Public CookingTip As String
'        Public Refinement As String
'        Public Storage As String
'        Public Productivity As String
'        Public [Protected] As Boolean
'        Public CodeLink As Integer
'        Public IsGlobal As Boolean
'        Public CodeTrans As Integer
'        Public AllowUse As Boolean

'        ' for menu card
'        Public MenuCardDateFrom As Date
'        Public MenuCardDateUntil As Date
'        Public MenuCardCodeSetPrice As Integer

'        'for RX
'        Public EgsRef As Integer
'        Public EgsID As Integer

'        'for clsRnXML
'        Public BrandName As String
'        Public CategoryName As String
'        Public SourceName As String
'        Public SupplierName As String
'        Public YieldUnitName As String
'        Public srUnitName As String

'        ' for clsRnXML (for versions <5)
'        Public Coeff As Double
'    End Structure

'    Public Structure structListeTranslation
'        Public CodeListe As Integer
'        Public CodeTrans As Integer
'        Public Name As String
'        Public Note As String
'        Public Remark As String
'        Public CCPDescription As String
'        Public Ingredients As String
'        Public Preparation As String
'        Public CookingTip As String
'        Public Refinement As String
'        Public Storage As String
'        Public Productivity As String
'        Public Description As String
'    End Structure

'End Class

Public Class clsListe

    Inherits clsDBRoutine

    Private L_ErrCode As enumEgswErrorCode
    Private L_lngCodeUser As Int32 = -1
    Private L_lngCodeSite As Int32 = -1

    'Properties
    Private L_AppType As enumAppType
    Private L_dtList As DataTable
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType
    Private L_bytFetchTypeTemp As enumEgswFetchType
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

    Private m_Err As Exception
    Private m_arrListeFailed As ArrayList    ' store code liste that were not deleted, etc.

#Region " Get Function "
    'DLS
    Public Function ApprovalEnable(ByVal intListeType As Integer) As Boolean
        Dim sqlCmd As SqlCommand = New SqlCommand
        Dim flagX As Boolean = False

        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.Text
                .CommandText = "SELECT count(*) FROM EgswApprovalSetting WHERE ApprovalFlag=1 AND ListeType=@ListeType"
                .Parameters.Add("@ListeType", SqlDbType.Int).Value = intListeType
                If CInt(.ExecuteScalar) > 0 Then
                    flagX = True
                End If
                sqlCmd.Connection.Close() 'DLS 31.05.2007
                sqlCmd.Connection.Dispose()  'DLS 31.05.2007
                sqlCmd.Dispose()

                Return flagX
            End With
        Catch ex As Exception
            flagX = False
        End Try
    End Function

    '--- JRN 12.15.2010 Recipe Template
    Public Function GetTemplates() As DataTable
        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand

        With sqlCmd
            .Connection = cn
            .CommandText = "GET_RecipeTemplates"
            .CommandType = CommandType.StoredProcedure
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, sqlCmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    '---

    Public Function GetFTSListeSearchResult(ByVal udtUser As structUser, ByVal slParams As SortedList, ByVal intCodeTrans As Integer, ByVal intPagenumber As Integer, ByVal intPageSize As Integer, ByRef intTotalRows As Integer, Optional ByVal blnAllowCreateUseSubRecipe As Boolean = False, Optional ByVal blnFTSEnable As Boolean = False, Optional ByVal strSort As String = "", Optional ByVal intCodeSetPrice As Integer = -1) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable("EGSWLISTE")
        Dim sbSQL As New StringBuilder

        Dim strNumber As String = CStr(slParams("NUMBER"))
        'Dim strWord As String = fctTransformStrSearch(CStr(slParams("WORD")))
        Dim strWord As String = CStr(slParams("WORD"))
        Dim strKeywords As String = CStr(slParams("KEYWORDS"))
        Dim strUnKeywords As String = CStr(slParams("KEYUNWANTED")) 'VRP 11.09.2007
        Dim strCategory As String = CStr(slParams("CATEGORY"))
        Dim strBrand As String = CStr(slParams("BRAND"))
        Dim strSupplier As String = CStr(slParams("SUPPLIER"))
        Dim intListeType As enumDataListType = CType(slParams("TYPE"), enumDataListType)
        Dim strIngredientsWanted As String = fctTransformStrSearch(CStr(slParams("INGWANTED")))
        Dim strIngredientsUnwanted As String = fctTransformStrSearch(CStr(slParams("INGUNWANTED")))
        Dim nUserLevel As enumGroupLevel = CType(slParams("USERLEVEL"), enumGroupLevel)
        Dim strCodeSiteList As String = CStr(slParams("CODESITE"))
        Dim strFilter As String = CStr(slParams("FILTER"))



        'Dim intCodeSite As Integer '= CInt(slParams("CODESITE"))
        Dim intCodeUser As Integer = CInt(slParams("CODEUSER"))
        'Dim intCodeProperty As Integer = CInt(slParams("CODEPROPERTY"))

        Dim strCodelisteList As String = ""
        If Not slParams("CODELISTELIST") Is Nothing Then strCodelisteList = CStr(slParams("CODELISTELIST"))

        Dim strSource As String = ""
        If slParams.Contains("SOURCE") Then strSource = CStr(slParams("SOURCE"))

        ' price
        Dim strPrice As String = ""
        If slParams.Contains("PRICE") Then strPrice = CStr(slParams("PRICE"))
        Dim strPriceArr() As String = strPrice.Split(CChar("|"))
        Dim strPriceCol As String = "" ' store price column to search in
        If strPriceArr.Length = 2 Then
            strPriceCol = strPriceArr(0)
            strPrice = strPriceArr(1)
        End If

        ' if Price value is [Date1]-[date2], insert "BETWEEN" [Date1] "AND" [date2]
        If strPrice.IndexOf("-") > 0 Then
            Dim arrPrice() As String = strPrice.Split(CChar("-"))
            strPrice = " BETWEEN " & CDbl(arrPrice(0)).ToString(New System.Globalization.CultureInfo("en-US")) _
                & " AND " & CDbl(arrPrice(1)).ToString(New System.Globalization.CultureInfo("en-US"))
            'strPrice = " BETWEEN " & strPrice.Replace("-", " AND ")
        ElseIf strPrice.Trim.Length > 0 Then
            If strPrice.IndexOf(">") > -1 Then
                strPrice = ">" & CDbl(strPrice.Replace(">", "")).ToString(New System.Globalization.CultureInfo("en-US"))
            ElseIf strPrice.IndexOf("<") > -1 Then
                strPrice = "<" & CDbl(strPrice.Replace("<", "")).ToString(New System.Globalization.CultureInfo("en-US"))
            End If
        End If

        ' add price column to search in
        If strPrice.Length > 0 Then strPrice = strPriceCol & " " & strPrice

        ' date
        Dim strDate As String = ""
        If slParams.Contains("DATE") Then strDate = CStr(slParams("DATE"))
        If strDate.IndexOf("-") > 0 Then strDate = " BETWEEN '" & strDate.Replace("-", "' AND '") & "'"
        If strDate.IndexOf("=") > 0 Then strDate = strDate.Replace("=", "='") & "'"

        ' nutrient rules
        Dim strNutrientRules As String = CStr(slParams("NUTRIENTRULES"))
        If strNutrientRules = Nothing Then strNutrientRules = ""

        ' allergens
        Dim strAllergens As String = CStr(slParams("ALLERGENS"))
        If strAllergens = Nothing Then strAllergens = ""

        'sales
        Dim shrtSalesStatus As Short = 0 '0=show all, 1=show linked listes only, 2=show unlinked liste only
        If slParams.Contains("LINKEDSALES") Then shrtSalesStatus = CShort(slParams("LINKEDSALES"))

        'search by code
        Dim blnSearchByCode As Boolean = False
        Dim intCode As Integer = -1
        If slParams.Contains("CODE") Then
            intCode = CInt(slParams.Item("CODE"))
            If intCode > 0 Then blnSearchByCode = True
        End If

        'search global only 'DLS JUne252007
        Dim bGlobalOnly As Boolean = False
        If Not slParams("GLOBALONLY") Is Nothing Then bGlobalOnly = CBool(slParams("GLOBALONLY"))

        strIngredientsWanted = ""
        strIngredientsUnwanted = ""

        Dim nNutrientEnergy As Integer = CInt(slParams.Item("NUTRIENTENERGY"))

        'JBB 07.04.2011
        Dim intCookMode As Integer = slParams("COOKMODE")

        With sbSQL
            .Append("SET NOCOUNT ON ")
            .Append("SET ARITHABORT ON  ") '-- JBB 09.30.2011
            .Append("DECLARE @RecCount int ")
            .Append("SELECT @RecCount = @RecsPerPage * @Page + 1 ")
            .Append("IF @Page=0 SET @Page=1 ")

            If strWord.Length > 0 Then
                Dim cTrans As clsLanguage = New clsLanguage(L_AppType, L_strCnn, enumEgswFetchType.DataReader)
                Dim rwTrans As DataRow = cTrans.GetOne(intCodeTrans)
                Dim strLanguage As String = "'" & CStr(rwTrans("LangBreaker")) & "'"
                'strWord = fctTransformStrSearch(strWord)
                .Append("DECLARE @tblRank TABLE (Code int, Rank int) ")
                .Append("INSERT INTO @tblRank(Code, Rank)")
                .Append("SELECT  l.code, ")
                .Append("ISNULL(ISNULL(SUM(FTSTName.rank), SUM(FTSname.rank)), 0) * 2+ ")
                '.Append("ISNULL(ISNULL(SUM(FTSTNote.rank), SUM(FTSNote.rank)), 0) * .1+ ")
                .Append("ISNULL(ISNULL(SUM(FTSIngTName.rank), SUM(FTSIngName.rank)),0) AS Rank ")
                .Append("FROM egswListe l ")
                .Append("INNER JOIN egswlistetranslation lT ON l.code=lT.codeListe AND lT.Codetrans IN (" & intCodeTrans & ",NULL)  and lT.Name <> '' ") 'ADR 05.18.11 - from LEFT to INNER JOIN; added validation lT.Name <> ''
                .Append("LEFT JOIN RecipeTag RT ON l.Code=RT.Codeliste ") '// DRR 05.02.2011 added RecipeTag
                .Append("LEFT JOIN [EgswListeNote] LN ON LN.Codeliste=l.Code ") '// DRR 07.21.2011 added EgswListeNote
                .Append("LEFT JOIN [EgswListeNoteTrans] NT ON LN.ID=NT.EgswListeNoteID AND NT.CodeTrans IN (" & intCodeTrans & ",NULL) AND NT.Note<>'' AND NT.Comment<>'' ") '// DRR 07.21.2011 added EgswListeNoteTrans

                .Append("LEFT JOIN FREETEXTTABLE(egswliste, ([name],[CCPDescription],[CookingMethod],[CookingTip],[Description],[Ingredients],[Note],[Preparation],[Productivity],[Refinement],[Remark],[Storage]), @nvcWord, language " & strLanguage & ") FTSName ON l.code=FTSName.[Key] ")
                .Append("LEFT JOIN FREETEXTTABLE(egswlistetranslation, ([name],[CCPDescription],[CookingTip],[Description],[Ingredients],[Note],[Preparation],[Productivity],[Refinement],[Remark],[Storage]), @nvcWord, language " & strLanguage & ") FTSTName ON FTSTName.[Key]=lT.Id ")
                .Append("LEFT JOIN FREETEXTTABLE([RecipeTag] , ([Tagname]) , @nvcWord , language " & strLanguage & ") FTS_RecipeTag on RT.ID=FTS_RecipeTag.[Key] ") '// DRR 05.02.2011 added RecipeTag
                .Append("LEFT JOIN FREETEXTTABLE([EgswListeNoteTrans] , ([Note],[Comment],[CookMode]) , @nvcWord , language " & strLanguage & ") FTS_NoteTran on NT.ID=FTS_NoteTran.[Key] ") '// DRR 07.21.2011 added FTS ListeNoteTrans
                .Append("LEFT JOIN (SELECT CodeProduct, [Rank] FROM EgswLabel LEFT JOIN FREETEXTTABLE(egswLabel, ([Composition]), @nvcWord, language " & strLanguage & ")  FTSLabel ON EgswLabel.ID = FTSLabel.[Key]) AS FTSLabel1 ON FTSLabel1.CodeProduct = L.code ")
                '.Append("LEFT JOIN FREETEXTTABLE(egswliste, note, @nvcWord, language " & strLanguage & ") FTSNote ON l.code=FTSNote.[Key] ")
                '.Append("LEFT JOIN FREETEXTTABLE(egswlistetranslation, note, @nvcWord, language " & strLanguage & ") FTSTNote ON lT.Id=FTSTNote.[Key] ")

                .Append("LEFT JOIN egswDetails d ON l.Code=d.FirstCode ")
                .Append("LEFT JOIN egswListe l2 ON d.SecondCode=l2.Code ")
                .Append("LEFT JOIN egswlistetranslation l2T on l2T.codeListe=l2.code and l2T.Codetrans IN (" & intCodeTrans & ",NULL) " & " ")

                .Append("LEFT JOIN FREETEXTTABLE(egswliste, ([name],[CCPDescription],[CookingMethod],[CookingTip],[Description],[Ingredients],[Note],[Preparation],[Productivity],[Refinement],[Remark],[Storage]), @nvcWord, language " & strLanguage & ") FTSIngName ON l2.code=FTSIngName.[Key] ")
                .Append("LEFT JOIN FREETEXTTABLE(egswlistetranslation, ([name],[CCPDescription],[CookingTip],[Description],[Ingredients],[Note],[Preparation],[Productivity],[Refinement],[Remark],[Storage]), @nvcWord, language " & strLanguage & ") FTSIngTName ON FTSIngTName.[Key]=l2T.Id ")
                .Append("LEFT JOIN (SELECT CodeProduct, [Rank] FROM EgswLabel LEFT JOIN FREETEXTTABLE(egswLabel, ([Composition]), @nvcWord, language " & strLanguage & ")  FTSIngLabel ON EgswLabel.ID = FTSIngLabel.[Key]) AS FTSIngLabel1 ON FTSIngLabel1.CodeProduct = L2.code ")
                '.Append(" LEFT JOIN (SELECT * FROM ()) 

                .Append("WHERE (FTSName.rank Is Not NULL Or FTSTname.rank Is Not NULL Or FTSIngName.rank Is Not NULL Or FTSIngTName.rank Is Not NULL OR FTSIngLabel1.rank is not null or FTSLabel1.rank is not null OR FTS_RecipeTag.Rank IS NOT NULL OR FTS_NoteTran.Rank IS NOT NULL) ") '// DRR 05.02.2011 added RecipeTag '// DRR 07.21.2011 added ListeNoteTrans
                'Select Case intListeType
                '    Case enumDataListType.MenuItems, enumDataListType.Ingredient
                '    Case Else
                '        .Append("AND l.[Type]=" & intListeType & " ")
                'End Select

                Select Case intListeType
                    Case enumDataListType.MenuItems
                        .Append("AND (l.Type IN (2,4) OR l.type=8) ")
                        .Append("AND l.[use]=1 ")
                    Case enumDataListType.Ingredient
                        If blnAllowCreateUseSubRecipe Then
                            .Append("AND (l.Type IN (2,4) OR (l.type=8 and l.srQty>0)) ")
                        Else
                            .Append("AND (l.Type IN (2,4)) ")
                        End If
                        .Append(" AND l.[use]=1 ")
                    Case Else
                        .Append("AND l.Type=" & intListeType & " ")
                End Select

                .Append("GROUP BY l.code ")
                '.Append("ORDER BY rank DESC ")
                cmd.Parameters.Add("@nvcWord", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(strWord)
            End If

            .Append("CREATE TABLE #TempResults ")
            .Append("( ")
            .Append("ID int IDENTITY, ")
            .Append("code int, ")
            .Append("name nvarchar(260), ")
            .Append("number nvarchar(50), ")
            .Append("dates datetime, ")
            .Append("CodeSite int, ") '// DRR 06.16.2011
            .Append("price float, ")
            .Append("rank int ")
            .Append(") ")

            .Append("INSERT INTO #TempResults (code, name, number, dates, CodeSite, price, rank) ")
            .Append("SELECT DISTINCT r.code, CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end name, r.number, r.dates, r.CodeSite, ") 'ADR 05.18.11 - added DISTINCT

            If intListeType = enumDataListType.Merchandise Then
                .Append(" p.price, ")
            Else
                .Append(" 0, ")
            End If

            If strWord.Length > 0 Then
                .Append("rank.Rank AS rank ")
            Else
                .Append("0 AS rank ")
            End If


            .Append("FROM egswListe r ")
            .Append("INNER JOIN egswSharing ON egswSharing.Code=r.Code AND egswSharing.CodeEgswTable=dbo.fn_egswGetTableID('egswListe') ")

            ' Join rnListeTranslation table
            .Append("INNER JOIN egswListeTranslation l on r.code=l.codeListe AND l.codetrans IN (" & intCodeTrans & ",NULL) and l.Name <> '' ") 'ADR 05.18.11 - from LEFT to INNER JOIN; added validation l.Name <> ''

            ' Join Category table
            .Append("INNER JOIN  egswCategory c on c.code=r.category  ")
            .Append("LEFT OUTER JOIN egswItemTranslation cT on c.code=cT.code AND cT.codeTrans IN (" & intCodeTrans & ",NULL) AND cT.CodeEgswTable=dbo.fn_egswGetTableID('egswCategory') AND RTRIM(cT.Name)<>''  ")

            ' Join Brand
            If strBrand.Length > 0 Then
                .Append("LEFT JOIN egswBrand b ON b.code=r.Brand ")
                .Append("LEFT OUTER JOIN egswItemTranslation bT on b.code=bT.code AND bT.codeTrans IN (" & intCodeTrans & ",NULL) AND bT.CodeEgswTable=dbo.fn_egswGetTableID('egswBrand') ")
            End If

            ' Join Supplier
            If strSupplier.Length > 0 Then
                .Append("INNER JOIN egswSupplier supplier ON supplier.code=r.Supplier ")
            End If

            ' Join Source
            If strSource.Length > 0 Then
                .Append("LEFT OUTER JOIN egswSource source ON source.code=r.Source ")
            End If

            ' Join Keywords table
            If strKeywords.Length > 0 Or strUnKeywords.Length > 0 Then
                .Append("INNER JOIN egswKeyDetails kd on r.code=kd.codeListe  ")
                .Append("INNER JOIN egswKeyword k on kd.codeKey=k.code ")
                .Append("LEFT OUTER JOIN egswItemTranslation kT on k.code=kT.code AND kT.codeTrans IN (" & intCodeTrans & ",NULL) AND kT.CodeEgswTable=dbo.fn_egswGetTableID('egswKeyword') AND RTRIM(kT.Name)<>'' ")
            End If

            ' Join Ingredients, rnListe table for ingredients and traslation rnListe
            If strIngredientsWanted.Length > 0 Or strIngredientsUnwanted.Length > 0 Then
                .Append("LEFT OUTER JOIN egswDetails d on r.code=d.firstCode  ")
                .Append("LEFT OUTER JOIN egswListe r1 on  r1.code=d.secondcode ")
                .Append("INNER JOIN egswListeTranslation l2 on r1.code=l2.codeListe AND l2.codetrans IN (" & intCodeTrans & ",NULL) and l2.Name <> '' ") 'ADR 05.18.11 - from LEFT to INNER JOIN; added validation l2.Name <> ''
            End If

            Select Case intListeType
                Case enumDataListType.Ingredient, enumDataListType.Merchandise
                    ' Join Sub Recipes with prices when searching ingredient and merchandise
                    If intCodeSetPrice <> -1 Then
                        .Append("LEFT OUTER JOIN egswListeSetPrice p on r.code=p.codeliste AND p.codesetprice=" & intCodeSetPrice & " AND p.Position=1 ")
                    Else
                        .Append("LEFT OUTER JOIN egswListeSetPrice p on r.code=p.codeliste AND p.position=1 ")
                    End If
                Case enumDataListType.Recipe, enumDataListType.Menu
                    ' join calculations when seraching recipes / menu
            End Select

            If intCodeSetPrice <> -1 Then
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste and pCalc.codesetprice=" & intCodeSetPrice & " ")
            Else
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste ")
            End If

            'join nutrient rules
            If strNutrientRules.Trim.Length > 0 Then
                .Append("INNER JOIN egswNutrientVal ON r.code=egswNutrientVal.CodeListe ")
            End If

            'join allergens
            If strAllergens.Length > 0 Then
                .Append("LEFT OUTER JOIN egswListeAllergen a ON a.CodeListe=r.code ")
            End If


            If strWord.Length > 0 Then
                .Append("INNER JOIN @tblRank rank ON rank.Code=r.code ")
            End If

            .Append("WHERE ")

            If blnSearchByCode Then
                .Append(" r.code=" & intCode & " ")
            Else
                ' type
                Select Case intListeType
                    Case enumDataListType.MenuItems
                        .Append("(r.Type IN (2,4) OR r.type=8) ")
                        .Append("AND r.[use]=1 ")
                    Case enumDataListType.Ingredient
                        If blnAllowCreateUseSubRecipe Then
                            .Append("(r.Type IN (2,4) OR (r.type=8 and r.srQty>0)) ")
                        Else
                            .Append("(r.Type IN (2,4)) ")
                        End If
                        .Append(" and r.[use]=1 ")
                    Case Else
                        .Append("r.Type=" & intListeType & " ")
                        '.Append(" and (r.[use]=1 OR (r.codeUser =" & intCodeUser & " AND r.type IN (2,8,16))) ") was moved to egswSharing part (d one hu craeted)
                End Select

                If intListeType = enumDataListType.Merchandise Or intListeType = enumDataListType.Recipe Or intListeType = enumDataListType.Menu Or intListeType = enumDataListType.Ingredient Then
                    'check if user is searching his own site, if searching his own site, get all dat is shared to d user, user'site and user's property
                    If CStr("," & strCodeSiteList & ",").IndexOf("," & CStr(udtUser.Site.Code) & ",") > -1 Then
                        'get sharing of user
                        .Append("AND ((r.[use]=1 AND egswSharing.CodeUserSharedTo=" & CStr(udtUser.Site.Group) & " AND egswSharing.Type IN(" & ShareType.CodeProperty & ", " & ShareType.CodePropertyView & ")) ")
                        '.Append("OR (r.[use]=1 AND egswSharing.CodeUserSharedTo=" & CStr(udtUser.Site.Code) & " AND egswSharing.Type IN(" & ShareType.CodeSite & ", " & ShareType.CodeSiteView & ")) ")                        
                        .Append("OR (r.[use]=1 AND egswSharing.CodeUserSharedTo IN (" & strCodeSiteList & ") AND egswSharing.Type IN(" & ShareType.CodeSite & ", " & ShareType.CodeSiteView & ")) ")
                        .Append("OR (r.[use]=1 AND egswSharing.CodeUserSharedTo=" & CStr(udtUser.Code) & " AND egswSharing.Type IN(" & ShareType.CodeUser & ", " & ShareType.CodeUserView & ")) ")
                        .Append("OR (r.[use]=1 AND egswSharing.Type IN(" & ShareType.ExposedViewing & ")) ")

                        'd one who created
                        If intListeType <> enumDataListType.MenuItems AndAlso intListeType <> enumDataListType.Ingredient Then
                            .Append("OR (")
                            .Append("(egswSharing.CodeUserSharedTo=" & CStr(intCodeUser) & " ")
                            .Append("AND egswSharing.Type=" & ShareType.CodeUserOwner & " ")
                            .Append("AND r.[use]=1 ") '// DRR 04.11.2011 replace 0 to 1
                            .Append("AND r.type IN (2,8,16))) ")
                        End If
                    Else
                        .Append(" AND r.[use]=1 AND ((egswSharing.CodeUserSharedTo IN (" & strCodeSiteList & ") AND egswSharing.Type IN(" & ShareType.CodeSite & ")) ")
                    End If
                    .Append(") ")
                Else
                    .Append("AND (egswSharing.CodeUserSharedTo IN (" & strCodeSiteList & ") AND egswSharing.Type IN(" & ShareType.CodeSite & ")) ")
                End If

                ' Flags
                '.Append(" AND r.protected=0 ") DRR 05.05.2011 commented

                'GlobalOnly
                If bGlobalOnly Then 'DLS June252007
                    .Append(" AND r.IsGlobal=1 ")
                End If

                'If strWord.Length > 0 Then
                '    strWord = "%" & strWord & "%" ' always use like

                '    ' find match in rnliste table
                '    .Append("AND (r.Name like @nvcWord ")
                '    ' find match in rnlistetranslation table
                '    .Append("OR (l.name like @nvcWord ")
                '    .Append("AND l.codetrans=" & intCodeTrans & ")) ")
                '    cmd.Parameters.Add("@nvcWord", SqlDbType.NVarChar, 260).Value = strWord
                'End If

                If strNumber.Length > 0 Then
                    strNumber = "%" & strNumber & "%" ' always use like
                    .Append("AND r.Number like @nvcNumber ")
                    cmd.Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 25).Value = ReplaceSpecialCharacters(strNumber)
                End If

                ' Date
                If strDate.Length > 0 Then .Append(" AND r.dates " & strDate & " ")

                ' Price
                Select Case intListeType
                    Case enumDataListType.Merchandise
                        If strPrice.Length <> 0 Then
                            .Append("AND p." & strPrice & " ")
                        End If
                    Case enumDataListType.Recipe, enumDataListType.Menu
                        If strPrice.Length <> 0 Then
                            .Append("AND pCalc." & strPrice & " ")
                        End If
                End Select

                'Wanted Ingredient Search
                If strIngredientsWanted.Length > 0 Then
                    Dim strSQLEintCodeIng1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR r1.name LIKE ", "@nvcIngWanted", ReplaceSpecialCharacters(strIngredientsWanted), CChar(","), True)
                    Dim strSQLEintCodeIng2 As String = AddParam(cmd, SqlDbType.NVarChar, " OR l2.name LIKE ", "@nvcIng2Wanted", ReplaceSpecialCharacters(strIngredientsWanted), CChar(","), True)

                    ' Find match in ingredients
                    .Append("AND (r1.Name like " & strSQLEintCodeIng1 & " ")

                    ' find match ingredient in rnliste translation table
                    .Append("OR (l2.Name like " & strSQLEintCodeIng2 & " ")
                    .Append("AND l2.codeTrans=" & intCodeTrans & ")) ")
                End If

                ' Unwanted Ingredient Search
                If strIngredientsUnwanted.Length <> 0 Then
                    Dim strSQLEintCodeIngUw1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR name LIKE ", "@nvcIngUnWanted", ReplaceSpecialCharacters(strIngredientsUnwanted), CChar(","), True)
                    'compare it using egswliste.anme w/codetarns
                    .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select code FROM egswListe WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND CodeTrans=" & intCodeTrans & " ) ) ")
                    'compare it using egswlistetransaltion.name w/codetrans
                    .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select codeliste FROM egswlistetranslation WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND codetrans=" & intCodeTrans & " ) ) ")
                    'compare it using egswliste.name w/o codetrans for liste's w/o translstions
                    .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select code FROM egswListe WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND Code NOT IN (SELECT codeListe FROM egswListeTranslation WHERE CodeTrans=" & intCodeTrans & " )) ) ")
                End If

                'brand
                If strBrand.Length > 0 Then
                    .Append("AND (b.name=@nvcBrand OR bT.name=@nvcBrand) ")
                    cmd.Parameters.Add("@nvcBrand", SqlDbType.NVarChar, 150).Value = ReplaceSpecialCharacters(strBrand)
                End If

                'category
                If strCategory.Length > 0 And strCategory.ToString <> "-1" Then '// DRR 05.31.2011 added -1
                    If Not IsNumeric(strCategory) Then '// DRR 06.01.2011
                        .Append("AND (c.name=@nvcCategory OR cT.name=@nvcCategory) ")
                    Else
                        .Append("AND (c.code=@nvcCategory) ")
                    End If
                    cmd.Parameters.Add("@nvcCategory", SqlDbType.NVarChar, 150).Value = ReplaceSpecialCharacters(strCategory)
                End If

                'source
                If strSource.Length > 0 And strSource.ToString <> "-1" Then '// DRR 06.02.2011 added -1
                    If Not IsNumeric(strSource) Then
                        .Append("AND (source.name=@nvcSource) ")
                    Else
                        .Append("AND (source.Code=@nvcSource) ") '// DRR 06.02.2011
                    End If
                    cmd.Parameters.Add("@nvcSource", SqlDbType.NVarChar, 150).Value = ReplaceSpecialCharacters(strSource)
                End If

                ' SUPPLIER
                If strSupplier.Length > 0 And strSupplier.ToString <> "-1" Then '// DRR 06.02.2011 added -1
                    If Not IsNumeric(strSupplier) Then
                        .Append("AND supplier.nameref=@nvcSupplier ")
                    Else
                        .Append("AND supplier.Code=@nvcSupplier ") '// DRR 06.02.2011
                    End If
                    cmd.Parameters.Add("@nvcSupplier", SqlDbType.NVarChar, 150).Value = ReplaceSpecialCharacters(strSupplier)
                End If

                'Wanted keywords
                If strKeywords.Length > 0 Then
                    Dim strSQLEintCode1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR k.name LIKE ", "@nvcKeyworda", ReplaceSpecialCharacters(strKeywords), CChar(","), True)
                    Dim strSQLEintCode2 As String = AddParam(cmd, SqlDbType.NVarChar, " OR kt.name LIKE ", "@nvcKeywordb", ReplaceSpecialCharacters(strKeywords), CChar(","), True)

                    ' find match keyword in keyword parent table
                    .Append("AND ((k.name LIKE " & strSQLEintCode1 & " ")

                    ' find match keyword in keyword parent table translation
                    .Append("OR (kt.name LIKE " & strSQLEintCode2 & " ")
                    .Append("AND kt.codetrans=" & intCodeTrans & "))) ")
                End If

                ''---- Unwanted keywords VRP 11.09.2007
                'If strUnKeywords.Length > 0 Then
                '    If strUnKeywords.Length > 0 Then
                '        Dim strSQLEintCodeKeyUw As String = AddParam(cmd, SqlDbType.NVarChar, " OR k.name LIKE ", "@nvcUnKeyword", strUnKeywords, CChar(","), True)

                '        'compare it using egswliste.anme w/codetarns
                '        .Append("AND k.code NOT IN (select codeliste FROM egswKeyDetails WHERE codekey IN (select code FROM egswKeyword WHERE name LIKE " & strSQLEintCodeKeyUw & " ) ) ")
                '        'compare it using egswlistetransaltion.name w/codetrans
                '        .Append("AND k.code NOT IN (select codeliste FROM egswKeyDetails WHERE codekey IN (select code FROM egswItemTranslation WHERE name LIKE " & strSQLEintCodeKeyUw & " AND codetrans=" & intCodeTrans & " ) ) ")
                '        'compare it using egswliste.name w/o codetrans for liste's w/o translstions
                '        .Append("AND k.code NOT IN (select codeliste FROM egswKeyDetails WHERE codekey IN(select code FROM egswKeyword WHERE name LIKE " & strSQLEintCodeKeyUw & " AND Code NOT IN (SELECT code FROM egswItemTranslation WHERE CodeTrans=" & intCodeTrans & " )) ) ")
                '    End If
                'End If
                ''--------

                'nutrient rules
                If strNutrientRules.Trim.Length > 0 Then
                    Dim cNutrientRules As clsNutrientRules = New clsNutrientRules(udtUser, enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataSet)
                    Dim arr() As String = strNutrientRules.Split(CChar(","))
                    Array.Sort(arr)

                    Dim i As Integer = 1
                    Dim intLastPosition As Integer = 0
                    Dim arr2() As String
                    While i < arr.Length
                        arr2 = arr(i).Split(CChar("-"))
                        If CInt(arr2(0)) > 0 Then
                            Dim rwTemp As DataRow = CType(cNutrientRules.GetList(CInt(arr2(1))), DataSet).Tables(1).Rows(0)
                            If intLastPosition = CInt(arr2(0)) Then
                                .Append(" OR egswNutrientVal.N" & arr2(0) & " BETWEEN " & CStr(rwTemp("minimum")) & " AND " & CStr(rwTemp("maximum")) & " ")
                            Else
                                If i = 1 Then
                                    .Append(" AND ( ")
                                Else
                                    .Append(" ) AND ( ")
                                End If

                                .Append(" egswNutrientVal.N" & arr2(0) & " BETWEEN " & CStr(rwTemp("minimum")) & " AND " & CStr(rwTemp("maximum")) & " ")
                            End If

                            If i + 1 = arr.Length Then
                                .Append(" ) ")
                            End If

                            intLastPosition = CInt(arr2(0))
                        End If
                        i += 1
                    End While
                End If

                If strAllergens.Length > 0 Then
                    If strAllergens.IndexOf("NOT") > -1 Then
                        .Append(" AND (a.codeAllergen " & strAllergens & " OR a.codeAllergen IS NULL) ")
                    Else
                        .Append(" AND a.codeAllergen " & strAllergens & " ")
                    End If
                End If

                If nNutrientEnergy = 1 Then 'DLS Dec 10 2007
                    .Append("   AND (SELECT top 1 case WHEN ISNULL(N1,-1) =-1 THEN 0 ELSE N1 END FROM EgsWNutrientVal where CodeListe = r.code) > 0    ")
                ElseIf nNutrientEnergy = 2 Then
                    .Append("   AND (SELECT top 1 case WHEN ISNULL(N1,-1) =-1 THEN 0 ELSE N1 END FROM EgsWNutrientVal where CodeListe = r.code) = 0    ")
                End If

                'filter, this only works wen u r searching ur own site
                If CStr(strCodeSiteList) = CStr(udtUser.Site.Code) Then
                    Select Case UCase(strFilter)
                        Case "DRAFTS"
                            .Append(" AND r.codelink>0 ")
                        Case "SYSTEM"
                            .Append(" AND dbo.fn_EgswIsListeOwnedBySystem(r.code)>0 ")
                        Case "OWNED"
                            .Append(" AND dbo.fn_EgswIsListeOwnedBySite (r.code," & udtUser.Site.Code & ")= " & udtUser.Site.Code & " AND r.codeLink<1 ")
                        Case "SHARED"
                            .Append(" AND dbo.fn_EgswIsListeOwnedBySite (r.code," & udtUser.Site.Code & ")<> " & udtUser.Site.Code & " AND dbo.fn_EgswIsListeOwnedBySystem(r.code)=0 ")
                    End Select
                End If
            End If

            If shrtSalesStatus = 1 Then 'linked only
                If intListeType = enumDataListType.Merchandise Then
                    .Append(" AND r.Code IN (SELECT CodeListe FROM egswLinkFbRnPOS linkMP WHERE linkMP.TypeLink IN (0) AND linkMP.CodeListe=r.code ") 'for merchandise and product
                    .Append("       AND linkMP.CodeProduct IN (SELECT CodeProduct FROM egswLinkFbRnPOS linkPS WHERE linkPS.TypeLink IN (1) AND linkPS.CodeProduct=linkMP.codeProduct)) ") 'for product and salesitem
                ElseIf intListeType = enumDataListType.Recipe OrElse intListeType = enumDataListType.Menu Then
                    .Append(" AND r.Code IN (SELECT CodeListe FROM egswLinkFbRnPOS linkLS WHERE linkLS.TypeLink IN (2) AND linkLS.CodeListe=r.code )") ' for recipes/menus and salesitem
                End If
            ElseIf shrtSalesStatus = 2 Then 'not linked only
                If intListeType = enumDataListType.Merchandise Then
                    .Append(" AND r.Code NOT IN (SELECT CodeListe FROM egswLinkFbRnPOS linkMP WHERE linkMP.TypeLink IN (0) AND linkMP.CodeListe=r.code ") 'for merchandise and product
                    .Append("       AND linkMP.CodeProduct IN (SELECT CodeProduct FROM egswLinkFbRnPOS linkPS WHERE linkPS.TypeLink IN (1) AND linkPS.CodeProduct=linkMP.codeProduct)) ") 'for product and salesitem
                ElseIf intListeType = enumDataListType.Recipe OrElse intListeType = enumDataListType.Menu Then
                    .Append(" AND r.Code NOT IN (SELECT CodeListe FROM egswLinkFbRnPOS WHERE TypeLink IN (2) AND CodeListe=r.code )") ' for recipes/menus
                End If
            End If

            If strCodelisteList.Length > 0 Then
                .Append(" AND r.code IN " & strCodelisteList & " ")
            End If

            'JBB 07.02.2011
            If intListeType = enumDataListType.Recipe Then
                If intCookMode <> -1 Then
                    .Append(" AND  ISNULL(r.CookMode,0)=" & intCookMode)
                End If
            End If

            If strSort = "" Then
                .Append(" ORDER BY rank desc ")
            Else
                .Append(" ORDER BY " & strSort & " ")
            End If

            .Append("DECLARE @FirstRec int, @LastRec int, @MoreRecords int ")
            .Append("SELECT @FirstRec = (@Page - 1) * @RecsPerPage ")
            .Append("SELECT @LastRec = @Page * @RecsPerPage + 1 ")
            .Append("SELECT @iRow=COUNT(*) FROM #TempResults ")
            .Append("SELECT @MoreRecords=COUNT(*) FROM #TempResults WHERE ID>@LastRec ")

            .Append("DELETE FROM #TempResults WHERE ID <= @FirstRec OR ID >=@LastRec ")

            BuildFullySharedString(sbSQL, udtUser, intListeType, cmd)
            .Append("SELECT DISTINCT tr.ID, r.protected, r.code, r.type, CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end name, ")
            .Append("r.category, r.preparation, r.picturename, ")
            .Append("r.submitted, ISNULL(l.codetrans, r.codeTrans) as codeTrans, ")
            .Append("r.yield as yield, r.[percent], y.format as yieldFormat,ISNULL(yT.name,y.namedef) as yieldname, ")
            .Append("ISNULL(pCalc.coeff, 0) AS coeff, ISNULL(pCalc.calcPrice, 0) AS calcPrice, c.name AS categoryname, y.code as yieldCode, ")
            .Append("r.Supplier, r.source, r.remark, ")
            .Append("r.note, r.dates, r.submitted, replace(r.number, CHAR(1),'') AS NUMBER, ")
            .Append("r.wastage1,r.wastage2, r.wastage3,r.wastage4, ")
            .Append("r.picturename, ")
            .Append("r.srUnit,ISNULL(sruT.name,sru.namedef) as srUnitName, ")
            .Append("ISNULL(pCalc.coeff,0) AS coeff1, ")
            '.Append("r.currency, pCalc.coeff,")

            .Append("(1-((1-r.Wastage1/100.0) *")
            .Append("(1-r.Wastage2/100.0) * ")
            .Append("(1-r.Wastage3/100.0) * ")
            .Append("(1-r.Wastage4/100.0))) * 100.0 as TotalWastage, ISNULL(pCalc.imposedPrice,0) as ImposedSellingPrice, ")
            .Append("dbo.fn_EgswIsListeOwnedBySite (r.code," & udtUser.Site.Code & ") as sOwner, ")
            '.Append("0 as sOwner, ")
            .Append("ISNULL(fullyShared.Code, 0) as IsOwner,")
            '.Append("dbo.fn_EgswCheckListeFullySharedEditToUser(" & udtUser.Code & ", r.code) as IsOwner, ")
            .Append("dbo.fn_EgswIsListeOwnedBySystem(r.code) as IsSystemOwned, ") 'checks if it is owned by a ssytem site
            '.Append("0 as IsSystemOwned, ") 'checks if it is owned by a ssytem site
            .Append("CASE r.[Use] WHEN 0 THEN 1 ELSE 0 END AS IsDraft, ") 'checks if it is a draft
            .Append("CASE WHEN r.[use]=1 AND r.IsGlobal=1 THEN 1 ELSE 0 END AS IsGlobal, ") 'checks if it is a global, pending for approval is not cionsidered as global yet
            .Append("@MoreRecords AS MoreRecords, ")
            .Append("dbo.fn_EgswGetSetPriceData(r.code," & intCodeSetPrice & "," & intCodeTrans & ") as SetPriceData, ") ' used in searchlistelist.ascx for setprice computation
            .Append("dbo.fn_EgswGetSetPrice(r.code," & intCodeSetPrice & "," & intCodeTrans & ", '" & strPriceCol & "') as SetPriceValue, ") ' used in searchlistelist.ascx for setprice computation
            .Append("ISNULL(product.Code, 0) AS CodeFG ") ' uni
            .Append(", SO.[Name] AS SourceName ") ' sourcename for quick encoding
            .Append(", SU.[NameRef] AS SupplierName ") ' sourcename for quick encoding
            .Append(", egswNutrientVal.N1 AS N1,egswNutrientVal.N2 AS N2,egswNutrientVal.N3 AS N3,egswNutrientVal.N4 AS N4,egswNutrientVal.N5 AS N5,egswNutrientVal.N6 AS N6,egswNutrientVal.N7 AS N7,egswNutrientVal.N8 AS N8,egswNutrientVal.N9 AS N9,egswNutrientVal.N10 AS N10,egswNutrientVal.N11 AS N11,egswNutrientVal.N12 AS N12,egswNutrientVal.N13 AS N13,egswNutrientVal.N14 AS N14,egswNutrientVal.N15 AS N15 ") '---JRN 29.06.2010
            .Append(", egswNutrientVal.N16 AS N16,egswNutrientVal.N17 AS N17,egswNutrientVal.N18 AS N18,egswNutrientVal.N19 AS N19,egswNutrientVal.N20 AS N20,egswNutrientVal.N21 AS N21,egswNutrientVal.N22 AS N22,egswNutrientVal.N23 AS N23,egswNutrientVal.N24 AS N24,egswNutrientVal.N25 AS N25,egswNutrientVal.N26 AS N26,egswNutrientVal.N27 AS N27,egswNutrientVal.N28 AS N28,egswNutrientVal.N29 AS N29,egswNutrientVal.N30 AS N30 ") 'ADR 04.27.11
            .Append(", egswNutrientVal.N31 AS N31,egswNutrientVal.N32 AS N32,egswNutrientVal.N33 AS N33,egswNutrientVal.N34 AS N34 ") 'ADR 04.27.11
            .Append(", r.CodeSite as CodeSite ") '---JRN 29.06.2010
            .Append(", Site.Name as SiteName ") '---JRN 29.06.2010
            .Append(", ISNULL(r.CodeUser,0) as CodeUser ") '---JRN 29.06.2010
            .Append(",  ISNULL(r.[Use],0) as ListeUse ") '---JRN 29.06.2010
            .Append(",  ISNULL(r.[ApprovalStatus],0) as ApprovalStatus ") '---JRN 29.06.2010
            .Append(", ISNULL(r.checkoutuser,0) as Checkoutuser ") '// DRR 04.07.2011

            .Append(", r.Brand AS Brand ") '// DRR 05.30.2011
            .Append(", CASE WHEN b.name IS NULL OR LEN(RTRIM(LTRIM(b.name)))=0 THEN bT.Name ELSE b.name END AS BrandName ") '// DRR 05.30.2011

            'Select Case intListeType
            '    Case enumDataListType.Ingredient, enumDataListType.Merchandise
            '        .Append("p.price as Price ")
            '    Case Else
            '        .Append("0 as Price ")
            'End Select

            '-- JBB 09.28.2011
            .Append(",Version ")
            '-- JBB 09.30.2011
            If intListeType = 8 Then
                .Append(", CASE WHEN l.SubTitle IS NULL OR LEN(RTRIM(LTRIM(l.SubTitle)))=0 THEN r.SubTitle ELSE l.SubTitle end  SubTitle ")
                .Append(",CASE WHEN ISNULL(r.RecipeState,0) = 0 THEN (SELECT Name from EgswStatus where Code =1 and Type =0) else s1.name end RecipeStatus ")
                .Append(",CASE WHEN ISNULL(r.WebState,0) = 0 THEN (SELECT Name from EgswStatus where Code =1 and Type =1) else s2.name end WebStatus ")
                .Append(", egswNutrientVal.DisplayNutrition as DisplayNutrition ")
                .Append(", case when isnull(r.defaultpicture,0) = 0 then cast(0 as bit) else cast(1 as bit) end  as  ImageDisplay ")
                .Append(",ISNULL(Bd.Name,'') as PrimaryBrand ")
                .Append(",(SELECT ")
                .Append("( ")
                .Append("  SELECT ")
                .Append("  ( ")
                .Append("    SELECT  ")
                .Append("      n + '<br>' AS [text()] ")
                .Append("            FROM ")
                .Append("    ( ")
                .Append("      SELECT name as n ")
                .Append("		FROM recipebrand rb ")
                .Append("		inner join egswbrand b on b.Code = rb.Brand ")
                .Append("           where(rb.codeliste = r.Code And rb.brandclassification = 2) ")
                .Append("    ) r  ")
                .Append("    FOR XML PATH(''), TYPE ")
                .Append("  ) AS concat ")
                .Append("  FOR XML RAW, TYPE ")
                .Append(").value('/row[1]/concat[1]', 'varchar(max)')) AS SecondaryBrand ")
                '--
            End If

            .Append("FROM egswListe r INNER JOIN #TempResults tr ON r.code=tr.Code ")

            'this was just amede to check if user has edit and owner capabilities
            '.Append("LEFT OUTER JOIN dbo.fn_EgswGetListeFullySharedEditToUserByCodeUser(" & udtUser.Code & ", " & intListeType & ") fullyShared ON r.Code=fullyShared.Code ")

            .Append("LEFT OUTER JOIN @tblFullySharedWithEdit fullyShared ON r.Code=fullyShared.Code ")

            .Append("INNER JOIN egswSharing ON egswSharing.Code=r.Code AND egswSharing.CodeEgswTable=dbo.fn_egswGetTableID('egswListe') ")

            ' Join rnListeTranslation table
            .Append("INNER JOIN egswListeTranslation l on r.code=l.codeListe and l.codetrans IN (" & intCodeTrans & ",NULL) and l.Name <> '' ") 'ADR 05.18.11 - from LEFT to INNER JOIN; added validation lT.Name <> ''

            ' Join Yield table
            .Append("LEFT OUTER JOIN egswUnit y on y.code=r.yieldUnit ")
            .Append("LEFT OUTER JOIN egswItemTranslation yT on y.code=yT.code AND yT.codeTrans IN (" & intCodeTrans & ",NULL) AND yT.CodeEgswTable=dbo.fn_egswGetTableID('egswUnit') AND RTRIM(yT.Name)<>'' ")

            'Join Unit table for SubRecipe unit
            .Append("LEFT OUTER JOIN egswUnit sru on r.srunit=sru.code ")
            .Append("LEFT OUTER JOIN egswItemTranslation sruT on sru.code=sruT.code AND sruT.codeTrans IN (" & intCodeTrans & ",NULL) AND sruT.CodeEgswTable=dbo.fn_egswGetTableID('egswUnit') AND RTRIM(sruT.Name)<>'' ")

            ' Join Category table
            .Append("INNER JOIN  egswCategory c on c.code=r.category ")
            .Append("LEFT OUTER JOIN egswItemTranslation cT on c.code=cT.code AND cT.codeTrans IN (" & intCodeTrans & ",NULL) AND cT.CodeEgswTable=dbo.fn_egswGetTableID('egswCategory') AND RTRIM(cT.Name)<>''  ")

            ' Join Source table
            .Append("INNER JOIN  EgswSource SO ON SO.Code=r.Source ")

            ' Join Supplier table
            .Append("INNER JOIN  EgswSupplier SU ON SU.Code=r.Supplier ")

            'join product table for finished goods in recipe
            .Append("LEFT OUTER JOIN egswProduct product ON r.Code=product.RecipeLinkCode ")
            .Append("LEFT JOIN egswSite Site ON r.CodeSite=Site.Code ")
            'Join Product Table for merchandise linking of product for salesitem
            '.Append("LEFT OUTER JOIN egswLinkFbRnPOS link on link.CodeListe=r.code AND link.TypeLink=0 ")

            ' Join Brand
            'If strBrand.Length > 0 Then
            '    .Append("LEFT JOIN egswBrand b ON b.code=r.Brand ")
            '    .Append("LEFT OUTER JOIN egswItemTranslation bT on c.code=bT.code AND bT.codeTrans IN (" & intCodeTrans & ",NULL) AND bT.CodeEgswTable=dbo.fn_egswGetTableID('egswBrand') ")
            'End If

            ' Join Supplier
            'If strSupplier.Length > 0 Then
            '    .Append("INNER JOIN egswSupplier supplier ON supplier.code=r.Supplier ")
            'End If

            ' Join Source
            'If strSource.Length > 0 Then
            '    .Append("LEFT OUTER JOIN egswSource source ON source.code=r.Source ")
            'End If

            ' Join Keywords table
            'If strKeywords.Length <> 0 Then
            '    .Append("INNER JOIN egswKeyDetails kd on r.code=kd.codeListe  ")
            '    .Append("INNER JOIN egswKeyword k on kd.codeKey=k.code ")
            '    .Append("LEFT OUTER JOIN egswItemTranslation kT on k.code=kT.code AND kT.codeTrans IN (" & intCodeTrans & ",NULL) AND kT.CodeEgswTable=dbo.fn_egswGetTableID('egswKeyword') AND RTRIM(kT.Name)<>'' ")
            'End If

            ' Join Ingredients, rnListe table for ingredients and traslation rnListe
            'If strIngredientsWanted.Length <> 0 Or strIngredientsUnwanted.Length <> 0 Then
            '    .Append("LEFT OUTER JOIN egswDetails d on r.code=d.firstCode  ")
            '    .Append("LEFT OUTER JOIN egswListe r1 on  r1.code=d.secondcode ")
            '    .Append("LEFT OUTER JOIN egswListeTranslation l2 on r1.code=l2.codeListe AND l2.codetrans IN (" & intCodeTrans & ",NULL) ")
            'End If

            Select Case intListeType
                Case enumDataListType.Ingredient, enumDataListType.Merchandise
                    ' Join Sub Recipes with prices when searching ingredient and merchandise
                    If intCodeSetPrice <> -1 Then
                        .Append("LEFT OUTER JOIN egswListeSetPrice p on r.code=p.codeliste AND p.codesetprice=" & intCodeSetPrice & " ")
                    Else
                        .Append("LEFT OUTER JOIN egswListeSetPrice p on r.code=p.codeliste ")
                    End If
                Case enumDataListType.Recipe, enumDataListType.Menu
                    ' join calculations when seraching recipes / menu
            End Select

            If intCodeSetPrice <> -1 Then
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste and pCalc.codesetprice=" & intCodeSetPrice & " ")
            Else
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste ")
            End If

            'If strNutrientRules.Trim.Length > 0 Then
            '    'join nutrient rules
            '    .Append("INNER JOIN egswNutrientVal ON r.code=egswNutrientVal.CodeListe ")
            'End If

            .Append("LEFT OUTER JOIN EgswNutrientVal ON egswNutrientVal.Codeliste = r.Code ")

            .Append("LEFT JOIN egswBrand b ON b.code=r.Brand ") '// DRR 05.30.2011
            .Append("LEFT OUTER JOIN egswItemTranslation bT on c.code=bT.code AND bT.codeTrans IN (" & intCodeTrans & ",NULL) AND bT.CodeEgswTable=18 ") '// DRR 05.30.2011

            'JBB 10.20.2011
            If intListeType = 8 Then
                .Append("LEFT OUTER JOIN EgswStatus S1 ON S1.Code = r.RecipeState and S1.Type = 0 ")
                .Append("LEFT OUTER JOIN EgswStatus S2 ON S2.Code = r.WebState and S2.Type = 1 ")
                .Append("Left outer join egswbrand Bd on Bd.Code = r.Brand ")
            End If

            .Append("ORDER BY tr.ID ")
        End With

        Try
            With cmd
                .Connection = cn
                .CommandText = sbSQL.ToString
                .CommandTimeout = 10000
                .CommandType = CommandType.Text
                .Parameters.Add("@Page", SqlDbType.Int).Value = intPagenumber
                .Parameters.Add("@RecsPerPage", SqlDbType.Int).Value = intPageSize
                .Parameters.Add("@iRow", SqlDbType.Int).Direction = ParameterDirection.Output

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With

                intTotalRows = CInt(.Parameters("@iRow").Value)
            End With

            ' IsListeOwned(dt, udtUser.Site.Code)
            Return dt

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
            Return Nothing
        End Try
    End Function

    Public Sub GetFactorSubRecipeIngredient(ByRef dblIngQty As Double, ByVal intCodeListe As Integer, ByVal intIngUnit As Integer)

        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "sp_egswgetfactorsubrecipeingredient"
                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@fltIngQty", SqlDbType.Float).Value = dblIngQty
                .Parameters.Add("@intIngUnit", SqlDbType.Int).Value = intIngUnit
                .Parameters("@fltIngQty").Direction = ParameterDirection.InputOutput

                .ExecuteNonQuery()
                dblIngQty = CDbl(.Parameters("@fltIngQty").Value)
                sqlCmd.Connection.Close() 'DLS 31.05.2007
                sqlCmd.Dispose()
            End With
        Catch ex As Exception
            dblIngQty = 0
        End Try
    End Sub

    'AGL 2012.12.07
    Public Function GetUsedListe(ByRef intCodeSite As Integer, ByVal intCodeTrans As Integer) As DataTable
        Dim sqlDta As New SqlDataAdapter
        Dim dt As New DataTable
        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "SP_egswListeUsed"
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans


                sqlDta.SelectCommand = sqlCmd
                dt.BeginLoadData()
                sqlDta.Fill(dt)
                dt.EndLoadData()

                sqlCmd.Connection.Close() 'DLS 31.05.2007
                sqlCmd.Dispose()
                sqlDta.Dispose()
                Return dt
            End With
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public ReadOnly Property GetListeFailed() As ArrayList
        Get
            Return m_arrListeFailed
        End Get
    End Property

    Public Sub GetListeCountPerType(ByVal intcodeSite As Integer, ByRef intMerch As Integer, ByRef intRec As Integer, ByRef intMenu As Integer)
        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeSite", intcodeSite)
        arrParam(1) = New SqlParameter("@intMerchandiseCount", SqlDbType.Int)
        arrParam(2) = New SqlParameter("@intRecipeCount", SqlDbType.Int)
        arrParam(3) = New SqlParameter("@intMenuCount", SqlDbType.Int)

        arrParam(1).Direction = ParameterDirection.Output
        arrParam(2).Direction = ParameterDirection.Output
        arrParam(3).Direction = ParameterDirection.Output

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswGetListeCountPerType", arrParam)

            intMerch = CInt(arrParam(1).Value)
            intRec = CInt(arrParam(2).Value)
            intMenu = CInt(arrParam(3).Value)
        Catch ex As Exception

        End Try
    End Sub

    'Public ReadOnly Property GetListeFailed(ByVal codeLang As Integer) As String
    '    Get
    '        Dim counter As Integer
    '        Dim nLastIndex As Integer = m_arrListeFailed.Count - 1
    '        Dim nItemCode As Integer
    '        Dim sb As New StringBuilder("")
    '        Dim dt As DataTable
    '        Dim dtRecipe As DataTable
    '        Dim rowRecipes As DataRow
    '        For counter = 0 To nLastIndex
    '            nItemCode = CInt(m_arrListeFailed(counter))
    '            sb.Append("<BR>")
    '            dt = GetListe(nItemCode, codeLang)
    '            If dt.Rows.Count = 0 Then
    '                dt = GetListe(nItemCode)
    '            End If

    '            sb.Append(dt.Rows(0).Item("name"))

    '            ' Attached recipes name
    '            dtRecipe = Me.GetIngredientRecipes(nItemCode, codeLang)
    '            For Each rowRecipes In dtRecipe.Rows
    '                sb.Append("<BR>&nbsp;&nbsp;&nbsp;")
    '                sb.Append(rowRecipes.Item("name"))
    '            Next

    '        Next

    '        Return sb.ToString
    '    End Get
    'End Property
    ''' <summary>
    ''' Returns list of codes of local items owned by given site.
    ''' </summary>
    ''' <param name="intCodeSite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetLocalItemsOwned(ByVal intCodeSite As Integer) As Object
        Dim strCommandText As String = "LISTE_GetLocalItemsOwnedBySite"

        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeSite", intCodeSite)

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

    Public Function GetListeAllergen(ByVal intCodeListe As Integer, ByVal intcodeTrans As Integer) As Object
        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand

        With sqlCmd
            .Connection = cn
            .CommandText = "sp_EgswListeAllergensGet"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intcodeTrans
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, sqlCmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListeFinishedGood(ByVal intCodeListe As Integer, ByVal intCodeSite As Integer, _
       ByVal intCodeTrans As Integer) As Object
        'RDTC 20.07.2007
        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand

        With sqlCmd
            .Connection = cn
            .CommandText = "EgswListeGetFinishedGoodInfo"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@CodeRecipe", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
            .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, sqlCmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetProductTranslations(ByVal intCodeProduct As Integer, ByVal intCodeSite As Integer, _
       ByVal intCodeTrans As Integer) As Object
        'RDTC 20.07.2007
        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand

        With sqlCmd
            .Connection = cn
            .CommandText = "EgswProductGetTranslations"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@Code", SqlDbType.Int).Value = intCodeProduct
            .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
            .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, sqlCmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListeSharing(ByVal intCodeListe As Integer) As Object
        'JTOC 15.07.2013
        Try
            Dim cn As New SqlConnection(L_strCnn)
            Dim cmd As New SqlCommand
            Dim da As New SqlDataAdapter
            Dim dt As New DataTable
            Dim sbSQL As New StringBuilder



            With cmd
                .Connection = cn
                .CommandText = "sp_GetListeSharing"
                .CommandTimeout = 10000
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With

                'intTotalRows = CInt(.Parameters("@iRow").Value)
            End With
            Return dt
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
            Return Nothing
        End Try


    End Function

    Public Function GetProductSalesItems(ByVal intCodeProduct As Integer, ByVal intCodeSite As Integer, _
    ByVal intCodeTrans As Integer) As Object
        'RDTC 20.07.2007
        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand

        With sqlCmd
            .Connection = cn
            .CommandText = "EgswProductGetSalesItems"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@Code", SqlDbType.Int).Value = intCodeProduct
            .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
            .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, sqlCmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetSalesItemTranslations(ByVal intCodeSalesItem As Integer, ByVal intCodeTrans As Integer) As Object
        'RDTC 20.07.2007
        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand

        With sqlCmd
            .Connection = cn
            .CommandText = "EgswProductGetSalesItemTranslations"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@Code", SqlDbType.Int).Value = intCodeSalesItem
            .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, sqlCmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetSalesItemPrices(ByVal intCodeSalesItem As Integer) As Object
        'RDTC 20.07.2007
        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand

        With sqlCmd
            .Connection = cn
            .CommandText = "EgswSalesItemsGetSetPrices"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@Code", SqlDbType.Int).Value = intCodeSalesItem
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, sqlCmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListeHistory(ByVal intCodeUser As Integer, ByVal listeType As enumDataListType, Optional ByVal intCodeTrans As Integer = -1, Optional ByVal intCodeSite As Integer = -1, Optional ByVal intRecipeState As Integer = -1) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            .CommandTimeout = 0 'AGL Merging 2012.09.04
            .CommandText = "sp_egswListeHistoryGet"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
            .Parameters.Add("@intListeType", SqlDbType.Int).Value = listeType
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
            .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite 'AGL 2013.08.24
            .Parameters.Add("@intRecipeState", SqlDbType.Int).Value = intRecipeState 'JTOC 11.18.2013

        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function



    Public Sub GetListeWeight(ByVal dblNetQty As Double, ByVal intUnitCode As Integer, ByVal intCodeListe As Integer, ByVal dblYield As Double, ByRef dblWeight As Double)
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter

        With cmd
            .Connection = cn
            .CommandText = "sp_egswListeIngredientGetWeight"
            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@fltQty", SqlDbType.Float).Value = dblNetQty
            .Parameters.Add("@intUnitCode", SqlDbType.Int).Value = intUnitCode
            .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@fltYield", SqlDbType.Float).Value = dblYield
            .Parameters.Add("@fltWeight", SqlDbType.Float).Direction = ParameterDirection.Output
            cn.Open()
            .ExecuteNonQuery()
            dblWeight = CDblDB(.Parameters("@fltWeight").Value)
            cn.Close()
            cn.Dispose()
        End With
    End Sub

    Public Function GetListeCategorySearchResult(ByVal slParams As SortedList, ByVal intCodeTrans As Integer, ByVal blnFTSenable As Boolean, Optional ByVal blnAllowCreateUseSubRecipe As Boolean = False, Optional ByVal strSort As String = "", Optional ByVal intCodeSetPrice As Integer = -1) As SqlDataReader
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable("EGSWLISTE")
        Dim sbSQL As New StringBuilder

        Dim strNumber As String = CStr(slParams("NUMBER"))
        Dim strWord As String = fctTransformStrSearch(CStr(slParams("WORD")))
        Dim strKeywords As String = CStr(slParams("KEYWORDS"))
        Dim strCategory As String = CStr(slParams("CATEGORY"))
        Dim strBrand As String = CStr(slParams("BRAND"))
        Dim strSupplier As String = CStr(slParams("SUPPLIER"))
        Dim intListeType As enumDataListType = CType(slParams("TYPE"), enumDataListType)
        Dim strIngredientsWanted As String = fctTransformStrSearch(CStr(slParams("INGWANTED")))
        Dim strIngredientsUnwanted As String = fctTransformStrSearch(CStr(slParams("INGUNWANTED")))
        Dim nUserLevel As enumGroupLevel = CType(slParams("USERLEVEL"), enumGroupLevel)
        Dim intCodeSite As Integer = CInt(slParams("CODESITE"))
        Dim intCodeUser As Integer = CInt(slParams("CODEUSER"))
        Dim intCodeProperty As Integer = CInt(slParams("CODEPROPERTY"))

        Dim strSource As String = ""
        If slParams.Contains("SOURCE") Then strSource = CStr(slParams("SOURCE"))

        ' price
        Dim strPrice As String = ""
        If slParams.Contains("PRICE") Then strPrice = CStr(slParams("PRICE"))
        Dim strPriceArr() As String = strPrice.Split(CChar("|"))
        Dim strPriceCol As String = "" ' store price column to search in
        If strPriceArr.Length = 2 Then
            strPriceCol = strPriceArr(0)
            strPrice = strPriceArr(1)
        End If

        ' if Price value is [Date1]-[date2], insert "BETWEEN" [Date1] "AND" [date2]
        If strPrice.IndexOf("-") > 0 Then strPrice = " BETWEEN " & strPrice.Replace("-", " AND ")

        ' add price column to search in
        If strPrice.Length > 0 Then strPrice = strPriceCol & " " & strPrice

        ' date
        Dim strDate As String = ""
        If slParams.Contains("DATE") Then strDate = CStr(slParams("DATE"))
        If strDate.IndexOf("-") > 0 Then strDate = " BETWEEN '" & strDate.Replace("-", "' AND '") & "'"
        If strDate.IndexOf("=") > 0 Then strDate = strDate.Replace("=", "='") & "'"

        ' nutrient rules
        Dim strNutrientRules As String = CStr(slParams("NUTRIENTRULES"))
        If strNutrientRules = Nothing Then strNutrientRules = ""

        'search by code
        Dim blnSearchByCode As Boolean = False
        Dim intCode As Integer = -1
        If slParams.Contains("CODE") Then
            intCode = CInt(slParams.Item("CODE"))
            If intCode > 0 Then blnSearchByCode = True
        End If

        With sbSQL

            .Append("SELECT DISTINCT ")
            '            .Append("r.category as code, c.name, c.type, c.position ")
            .Append("r.category as code, c.name, c.type ")
            .Append("FROM egswListe r ")

            .Append("INNER JOIN egswSharing ON egswSharing.Code=r.Code AND egswSharing.CodeEgswTable=dbo.fn_egswGetTableID('egswListe') ")

            ' Join rnListeTranslation table
            .Append("LEFT OUTER JOIN egswListeTranslation l on r.code=l.codeListe and l.codetrans IN (" & intCodeTrans & ",NULL) " & " ")

            ' Join Yield table
            .Append("LEFT OUTER JOIN egswUnit y on y.code=r.yieldUnit ")
            .Append("LEFT OUTER JOIN egswItemTranslation yT on y.code=yT.code AND yT.codeTrans IN (" & intCodeTrans & ",NULL) AND yT.CodeEgswTable=dbo.fn_egswGetTableID('egswUnit') ")

            ' Join Category table
            .Append("INNER JOIN  egswCategory c on c.code=r.category  ")
            .Append("LEFT OUTER JOIN egswItemTranslation cT on c.code=cT.code AND cT.codeTrans IN (" & intCodeTrans & ",NULL) AND cT.CodeEgswTable=dbo.fn_egswGetTableID('egswCategory') ")

            ' Join Brand
            If strBrand.Length > 0 Then
                .Append("LEFT JOIN egswBrand b ON b.code=r.Brand ")
                .Append("LEFT OUTER JOIN egswItemTranslation bT on c.code=bT.code AND bT.codeTrans IN (" & intCodeTrans & ",NULL) AND bT.CodeEgswTable=dbo.fn_egswGetTableID('egswBrand') ")
            End If

            ' Join Supplier
            If strSupplier.Length > 0 Then
                .Append("INNER JOIN egswSupplier supplier ON supplier.code=r.Supplier ")
            End If

            ' Join Source
            If strSource.Length > 0 Then
                .Append("INNER JOIN egswSource source ON source.code=r.Source ")
            End If

            ' Join Keywords table
            If strKeywords.Length <> 0 Then
                .Append("INNER JOIN egswKeyDetails kd on r.code=kd.codeListe  ")
                .Append("INNER JOIN egswKeyword k on kd.codeKey=k.code ")
                .Append("LEFT OUTER JOIN egswItemTranslation kT on k.code=kT.code AND kT.codeTrans IN (" & intCodeTrans & ",NULL) AND kT.CodeEgswTable=dbo.fn_egswGetTableID('egswKeyword') ")
            End If

            ' Join Ingredients, rnListe table for ingredients and traslation rnListe
            If strIngredientsWanted.Length <> 0 Or strIngredientsUnwanted.Length <> 0 Then
                .Append("LEFT OUTER JOIN egswDetails d on r.code=d.firstCode  ")
                .Append("LEFT OUTER JOIN egswListe r1 on  r1.code=d.secondcode ")
                .Append("LEFT OUTER JOIN egswListeTranslation l2 on r1.code=l2.codeListe AND l2.codetrans IN (" & intCodeTrans & ",NULL) ")
            End If

            Select Case intListeType
                Case enumDataListType.Ingredient, enumDataListType.Merchandise
                    ' Join Sub Recipes with prices when searching ingredient and merchandise
                    If intCodeSetPrice <> -1 Then
                        .Append("LEFT OUTER JOIN egswListeSetPrice p on r.code=p.codeliste AND p.codesetprice=" & intCodeSetPrice & " ")
                    Else
                        .Append("LEFT OUTER JOIN egswListeSetPrice p on r.code=p.codeliste ")
                    End If
                Case enumDataListType.Recipe, enumDataListType.Menu
                    ' join calculations when seraching recipes / menu
            End Select

            If intCodeSetPrice <> -1 Then
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste and pCalc.codesetprice=" & intCodeSetPrice & " ")
            Else
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste ")
            End If

            If strNutrientRules.Trim.Length > 0 Then
                'join nutrient rules
                .Append("INNER JOIN egswNutrientVal ON r.code=egswNutrientVal.CodeListe ")
            End If

            .Append("WHERE ")

            If blnSearchByCode Then
                .Append(" AND r.code=" & intCode & " ")
            Else
                ' type
                Select Case intListeType
                    Case enumDataListType.MenuItems
                        .Append("(r.Type IN (2,4) OR r.type=8) ")
                        .Append("AND r.[use]=1 ")
                    Case enumDataListType.Ingredient
                        If blnAllowCreateUseSubRecipe Then
                            .Append("(r.Type IN (2,4) OR (r.type=8 and r.srQty>0)) ")
                        Else
                            .Append("(r.Type IN (2,4)) ")
                        End If
                        .Append(" and r.[use]=1 ")
                    Case Else
                        .Append("r.Type=" & intListeType & " ")
                        '.Append(" and (r.[use]=1 OR (r.codeUser =" & intCodeUser & " AND r.type IN (2,8,16))) ") was moved to egswSharing part (d one hu craeted)
                End Select

                'sharing
                .Append("AND ((egswSharing.CodeUserSharedTo=" & CStr(intCodeProperty) & " AND egswSharing.Type IN(" & ShareType.CodeProperty & ", " & ShareType.CodePropertyView & ")) ")
                .Append("OR (egswSharing.CodeUserSharedTo=" & CStr(intCodeSite) & " AND egswSharing.Type IN(" & ShareType.CodeSite & ", " & ShareType.CodeSiteView & ")) ")
                .Append("OR (egswSharing.CodeUserSharedTo=" & CStr(intCodeUser) & " AND egswSharing.Type IN(" & ShareType.CodeUser & ", " & ShareType.CodeUserView & ")) ")

                ' d one who created
                If intListeType <> enumDataListType.MenuItems AndAlso intListeType <> enumDataListType.Ingredient Then
                    .Append("OR (r.[use]=1 ")
                    .Append("OR (egswSharing.CodeUserSharedTo=" & CStr(intCodeUser) & " ")
                    .Append("AND egswSharing.Type=" & ShareType.CodeUserOwner & " ")
                    .Append("AND r.type IN (2,8,16))) ")
                End If
                .Append(") ")

                ' Flags
                '.Append(" AND r.protected=0 ") DRR commented 05.05.2011

                If strWord.Length > 0 Then
                    strWord = "%" & strWord & "%" ' always use like
                    ' find match in rnliste table
                    .Append("AND (r.Name like @nvcWord ")
                    ' find match in rnlistetranslation table
                    .Append("OR (l.name like @nvcWord ")
                    .Append("AND l.codetrans=" & intCodeTrans & ")) ")
                    cmd.Parameters.Add("@nvcWord", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(strWord)
                End If

                If strNumber.Length > 0 Then
                    strNumber = "%" & strNumber & "%" ' always use like
                    .Append("AND r.Number like @nvcNumber ")
                    cmd.Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 25).Value = ReplaceSpecialCharacters(strNumber)
                End If

                ' Date
                If strDate.Length > 0 Then .Append(" AND r.dates " & strDate & " ")

                ' Price
                Select Case intListeType
                    Case enumDataListType.Merchandise
                        If strPrice.Length <> 0 Then
                            .Append("AND p." & strPrice & " ")
                        End If
                    Case enumDataListType.Recipe, enumDataListType.Menu
                        If strPrice.Length <> 0 Then
                            .Append("AND pCalc." & strPrice & " ")
                        End If
                End Select

                ' Wanted Ingredient Search
                If strIngredientsWanted.Length > 0 Then
                    Dim strSQLEintCodeIng1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR r1.name LIKE ", "@nvcIngWanted", ReplaceSpecialCharacters(strIngredientsWanted), CChar(","), True)
                    Dim strSQLEintCodeIng2 As String = AddParam(cmd, SqlDbType.NVarChar, " OR l2.name LIKE ", "@nvcIng2Wanted", ReplaceSpecialCharacters(strIngredientsWanted), CChar(","), True)

                    ' Find match in ingredients
                    .Append("AND (r1.Name like " & strSQLEintCodeIng1 & " ")

                    ' find match ingredient in rnliste translation table
                    .Append("OR (l2.Name like " & strSQLEintCodeIng2 & " ")
                    .Append("AND l2.codeTrans=" & intCodeTrans & ")) ")
                End If

                ' Unwanted Ingredient Search
                If strIngredientsUnwanted.Length <> 0 Then
                    Dim strSQLEintCodeIngUw1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR name LIKE ", "@nvcIngUnWanted", ReplaceSpecialCharacters(strIngredientsUnwanted), CChar(","), True)
                    .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select code FROM egswListe WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND CodeTrans=" & intCodeTrans & " ) ) ")
                    .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select codeliste FROM egswlistetranslation WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND codetrans=" & intCodeTrans & " ) ) ")
                End If

                'brand
                If strBrand.Length > 0 Then
                    .Append("AND (b.name=@nvcBrand OR bT.name=@nvcBrand) ")
                    cmd.Parameters.Add("@nvcBrand", SqlDbType.NVarChar, 150).Value = ReplaceSpecialCharacters(strBrand)
                End If

                'category
                If strCategory.Length > 0 Then
                    .Append("AND (c.name=@nvcCategory OR cT.name=@nvcCategory) ")
                    cmd.Parameters.Add("@nvcCategory", SqlDbType.NVarChar, 150).Value = ReplaceSpecialCharacters(strCategory)
                End If

                'source
                If strSource.Length > 0 Then
                    .Append("AND source.name=@nvcSource ")
                    cmd.Parameters.Add("@nvcSource", SqlDbType.NVarChar, 150).Value = ReplaceSpecialCharacters(strSource)
                End If

                ' SUPPLIER
                If strSource.Length > 0 Then
                    .Append("AND supplier.nameref=@nvcSupplier ")
                    cmd.Parameters.Add("@nvcSupplier", SqlDbType.NVarChar, 150).Value = ReplaceSpecialCharacters(strSupplier)
                End If

                If strKeywords.Length > 0 Then
                    Dim strSQLEintCode1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR k.name LIKE ", "@nvcKeyworda", ReplaceSpecialCharacters(strKeywords), CChar(","), True)
                    Dim strSQLEintCode2 As String = AddParam(cmd, SqlDbType.NVarChar, " OR kt.name LIKE ", "@nvcKeywordb", ReplaceSpecialCharacters(strKeywords), CChar(","), True)

                    ' find match keyword in keyword parent table
                    .Append("AND ((k.name LIKE " & strSQLEintCode1 & " ")

                    ' find match keyword in keyword parent table translation
                    .Append("OR (kt.name LIKE " & strSQLEintCode2 & " ")
                    .Append("AND kt.codetrans=" & intCodeTrans & "))) ")
                End If

                'If strNutrientRules.Trim.Length > 0 Then
                '    Dim cNutrientRules As clsNutrientRules = New clsNutrientRules(udtUser, enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataSet)
                '    Dim arr() As String = strNutrientRules.Split(CChar(","))
                '    Array.Sort(arr)

                '    Dim i As Integer = 1
                '    Dim intLastPosition As Integer = 0
                '    Dim arr2() As String
                '    While i < arr.Length
                '        arr2 = arr(i).Split(CChar("-"))
                '        If CInt(arr2(0)) > 0 Then
                '            Dim rwTemp As DataRow = CType(cNutrientRules.GetList(CInt(arr2(1))), DataSet).Tables(1).Rows(0)
                '            If intLastPosition = CInt(arr2(0)) Then
                '                .Append(" OR egswNutrientVal.N" & arr2(0) & " BETWEEN " & CStr(rwTemp("minimum")) & " AND " & CStr(rwTemp("maximum")) & " ")
                '            Else
                '                If i = 1 Then
                '                    .Append(" AND ( ")
                '                Else
                '                    .Append(" ) AND ( ")
                '                End If

                '                .Append(" egswNutrientVal.N" & arr2(0) & " BETWEEN " & CStr(rwTemp("minimum")) & " AND " & CStr(rwTemp("maximum")) & " ")
                '            End If

                '            If i + 1 = arr.Length Then
                '                .Append(" ) ")
                '            End If

                '            intLastPosition = CInt(arr2(0))
                '        End If
                '        i += 1
                '    End While
                'End If
            End If

            .Append(" ORDER BY c.name ")
        End With

        Try
            With cmd
                .Connection = cn
                .CommandText = sbSQL.ToString
                .CommandType = CommandType.Text

                .Connection.Open()
                Dim dr As SqlDataReader
                dr = .ExecuteReader(CommandBehavior.CloseConnection)
                Return dr
            End With
        Catch ex As Exception
            cn.Close()
            Throw New Exception(ex.Message, ex)
            Return Nothing
        End Try
    End Function

    Public Function CheckListeFullySharedEditToUser(ByVal intCodeUser As Integer, ByVal intCodeListe As Integer, ByRef blnIsGlobal As Boolean, ByRef blnSystem As Boolean) As Boolean
        Try
            Dim sqlCmd As SqlCommand = New SqlCommand
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandType = CommandType.Text
                .Parameters.Add("@IsOwned", SqlDbType.Bit)
                .Parameters("@IsOwned").Direction = ParameterDirection.Output
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe

                .CommandText = "SET @IsOwned=dbo.fn_EgswCheckListeFullySharedEditToUser(@CodeUser,@CodeListe)"
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                CheckListeFullySharedEditToUser = CBool(.Parameters("@IsOwned").Value)
            End With

            Dim sqlCmd2 As SqlCommand = New SqlCommand
            With sqlCmd2
                .Connection = New SqlConnection(L_strCnn)
                .CommandType = CommandType.Text
                .Parameters.Add("@IsGlobal", SqlDbType.Bit)
                .Parameters("@IsGlobal").Direction = ParameterDirection.Output
                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe

                .CommandText = "SELECT @IsGlobal=ISNULL(IsGlobal,0) FROM egswListe WHERE Code=@CodeListe"
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                blnIsGlobal = CBool(.Parameters("@IsGlobal").Value)
            End With


            Dim sqlCmd3 As SqlCommand = New SqlCommand
            With sqlCmd3
                .Connection = New SqlConnection(L_strCnn)
                .CommandType = CommandType.Text
                .Parameters.Add("@IsSystem", SqlDbType.Bit)
                .Parameters("@IsSystem").Direction = ParameterDirection.Output
                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe

                .CommandText = "SET @IsSystem=dbo.fn_EgswIsListeOwnedBySystem(@CodeListe)"
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                blnSystem = CBool(.Parameters("@IsSystem").Value)
            End With
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Sub IsListeOwned(ByRef dt As DataTable, ByVal intCodeSite As Integer)
        Try
            Dim sqlCmd As SqlCommand = New SqlCommand
            Dim sqlCmd2 As SqlCommand = New SqlCommand

            Dim intCodeListe As Integer
            Dim row As DataRow
            Dim cn As New SqlConnection(L_strCnn)

            sqlCmd.Connection = cn
            sqlCmd.Parameters.Add("@IsOwned", SqlDbType.Int)
            sqlCmd.Parameters("@IsOwned").Direction = ParameterDirection.Output
            sqlCmd.Parameters.Add("@Code", SqlDbType.Int)
            sqlCmd.Parameters.Add("@intCodeSite", SqlDbType.Int)
            sqlCmd.CommandText = "SET @IsOwned=dbo.fn_EgswIsListeOwnedBySite(@Code,@intCodeSite)"
            sqlCmd.CommandType = CommandType.Text
            '  sqlCmd.Connection.Open()

            sqlCmd2.Connection = cn
            sqlCmd2.Parameters.Add("@IsOwned", SqlDbType.Bit)
            sqlCmd2.Parameters("@IsOwned").Direction = ParameterDirection.Output
            sqlCmd2.Parameters.Add("@Code", SqlDbType.Int).Value = intCodeListe
            sqlCmd2.CommandText = "SET @IsOwned=dbo.fn_EgswIsListeOwnedBySystem(@Code)"
            sqlCmd2.CommandType = CommandType.Text
            '     sqlCmd2.Connection.Open()

            cn.Open()

            For Each row In dt.Rows
                intCodeListe = CInt(row("code"))
                sqlCmd.Parameters("@Code").Value = intCodeListe
                sqlCmd.Parameters("@intCodeSite").Value = intCodeSite

                sqlCmd2.Parameters("@Code").Value = intCodeListe

                sqlCmd.ExecuteNonQuery()
                sqlCmd2.ExecuteNonQuery()

                row("sowner") = CInt(sqlCmd.Parameters("@IsOwned").Value)
                row("IsSystemOwned") = CInt(sqlCmd2.Parameters("@IsOwned").Value)
            Next

            dt.AcceptChanges()

            cn.Close()
            cn.Dispose()

        Catch ex As Exception

            EventLog.WriteEntry("ClsListe_SearchResult_IsListeOwned", ex.Message)

        End Try
    End Sub

    Private Sub BuildFullySharedString(ByRef sb As StringBuilder, ByVal udtUser As structUser, ByVal intListeType As Integer, ByRef cmd As SqlCommand)
        sb.Append(" DECLARE @tblFullySharedWithEdit TABLE ([Code] [int]) ")

        ' has edit function
        Dim cUser As clsUser = New clsUser(L_AppType, L_strCnn, enumEgswFetchType.DataTable)
        Dim dtUserRoleRights As DataTable = CType(cUser.GetUserRolesAndRights(udtUser.Code), DataTable)

        Dim cRoles As clsRoles = New clsRoles(L_AppType, L_strCnn)
        If cRoles.CheckRoleExist(CType(intListeType, MenuType), UserRightsFunction.AllowModify, dtUserRoleRights) Then
            Select Case udtUser.RoleLevelHighest
                Case enumGroupLevel.Global
                    sb.Append(" INSERT INTO @tblFullySharedWithEdit(Code) ")
                    sb.Append(" SELECT Code FROM #TempResults ")
                Case enumGroupLevel.Property
                    sb.Append(" INSERT INTO @tblFullySharedWithEdit(Code) ")
                    sb.Append(" SELECT l.code FROM egswSharing s INNER JOIN egswListe l ON s.code=l.code ")
                    sb.Append(" WHERE s.CodeEgswTable=dbo.fn_egswGetTableID('egswListe') ")
                    sb.Append(" AND ((s.Type IN (1) AND s.CodeUserSharedTo IN (SELECT Code FROM egswSite WHERE [group]=@CodeProperty)) ")
                    sb.Append(" OR (s.Type IN (2) AND s.CodeUserSharedTo=@CodeProperty) ")
                    sb.Append(" OR (s.Type IN (3) AND s.CodeUserSharedTo=@CodeUser)) ")
                    sb.Append(" AND s.Code IN (SELECT Code FROM #TempResults) ")
                    sb.Append(" AND l.IsGlobal=0 ")

                    cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = udtUser.Site.Group
                    cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = udtUser.Code

                Case enumGroupLevel.Site
                    sb.Append(" INSERT INTO @tblFullySharedWithEdit(Code) ")
                    sb.Append(" SELECT l.code FROM egswSharing s INNER JOIN egswListe l ON s.code=l.code ")
                    sb.Append(" WHERE s.Code IN (SELECT Code FROM #TempResults) ")
                    sb.Append(" AND s.CodeEgswTable=dbo.fn_egswGetTableID('egswListe') ")
                    sb.Append(" AND ((s.Type IN (1) AND s.CodeUserSharedTo=@CodeSite) ")
                    sb.Append(" OR (s.Type IN (2) AND s.CodeUserSharedTo=@CodeProperty) ")
                    sb.Append(" OR (s.Type IN (3) AND s.CodeUserSharedTo=@CodeUser)) ")
                    sb.Append(" AND l.IsGlobal=0 ")

                    cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = udtUser.Site.Code
                    cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = udtUser.Site.Group
                    cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = udtUser.Code
            End Select
        End If
    End Sub

    Private Sub BuildFullySharedString2(ByRef sb As StringBuilder, ByVal udtUser As structUser, ByVal intListeType As Integer, ByRef cmd As SqlCommand)
        'sb.Append(" DECLARE @tblFullySharedWithEdit TABLE ([Code] [int]) ")

        ' has edit function
        Dim cUser As clsUser = New clsUser(L_AppType, L_strCnn, enumEgswFetchType.DataTable)
        Dim dtUserRoleRights As DataTable = CType(cUser.GetUserRolesAndRights(udtUser.Code), DataTable)

        Dim cRoles As clsRoles = New clsRoles(L_AppType, L_strCnn)
        If cRoles.CheckRoleExist(CType(intListeType, MenuType), UserRightsFunction.AllowModify, dtUserRoleRights) Then
            Select Case udtUser.RoleLevelHighest
                Case enumGroupLevel.Global
                    'sb.Append(" INSERT INTO @tblFullySharedWithEdit(Code) ")
                    'sb.Append(" SELECT Code FROM #TempResults ")
                Case enumGroupLevel.Property
                    'sb.Append(" INSERT INTO @tblFullySharedWithEdit(Code) ")
                    'sb.Append(" SELECT l.code FROM egswSharing s INNER JOIN egswListe l ON s.code=l.code ")
                    'sb.Append(" WHERE s.CodeEgswTable=dbo.fn_egswGetTableID('egswListe') ")
                    sb.Append(" AND ((EgsWSharing.Type IN (1) AND EgsWSharing.CodeUserSharedTo IN (SELECT Code FROM egswSite WHERE [group]=" & udtUser.Site.Group & ")) ")
                    sb.Append(" OR (EgsWSharing.Type IN (2) AND EgsWSharing.CodeUserSharedTo=" & udtUser.Site.Group & ") ")
                    sb.Append(" OR (EgsWSharing.Type IN (3) AND EgsWSharing.CodeUserSharedTo=" & udtUser.Code & ")) ")
                    'sb.Append(" AND EgsWSharing.Code IN (SELECT Code FROM #TempResults) ")
                    sb.Append(" AND r.IsGlobal=0 ")

                    'cmd.ParameterEgsWSharing.Add("@CodeProperty", SqlDbType.Int).Value = udtUser.Site.Group
                    'cmd.ParameterEgsWSharing.Add("@CodeUser", SqlDbType.Int).Value = udtUser.Code

                Case enumGroupLevel.Site
                    'sb.Append(" INSERT INTO @tblFullySharedWithEdit(Code) ")
                    'sb.Append(" SELECT l.code FROM egswSharing s INNER JOIN egswListe l ON EgsWSharing.code=l.code ")
                    'sb.Append(" WHERE EgsWSharing.Code IN (SELECT Code FROM #TempResults) ")
                    'sb.Append(" AND EgsWSharing.CodeEgswTable=dbo.fn_egswGetTableID('egswListe') ")
                    sb.Append(" AND ((EgsWSharing.Type IN (1) AND EgsWSharing.CodeUserSharedTo=" & udtUser.Site.Code & ") ")
                    sb.Append(" OR (EgsWSharing.Type IN (2) AND EgsWSharing.CodeUserSharedTo=" & udtUser.Site.Group & ") ")
                    sb.Append(" OR (EgsWSharing.Type IN (3) AND EgsWSharing.CodeUserSharedTo=" & udtUser.Code & ")) ")
                    sb.Append(" AND r.IsGlobal=0 ")

                    'cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = 
                    'cmd.Parameters.Add("@CodeProperty", SqlDbType.Int).Value = 
                    'cmd.Parameters.Add("@CodeUser", SqlDbType.Int).Value = 
            End Select
        End If
    End Sub

    'Modified by ADR 05.10.11 - intType param converted to optional
    Public Function GetKeywordsListCode(ByVal strKeywords As String, ByVal nCodeTrans As Integer, Optional ByVal intType As Integer = -1) As String
        Dim strKeyCodes As String = ""
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader

        strKeywords = CStrDB(strKeywords)
        strKeywords = Replace(strKeywords, "'", "''")
        strKeywords = Replace(strKeywords, ",", "','")
        strKeywords = Replace(strKeywords, "' ", "'")
        strKeywords = Replace(strKeywords, " '", "'") 'DLS August 272009

        With cmd
            .Connection = cn

            'comment by ADR 05.10.11
            '.CommandText = "SELECT K.Code FROM EgsWKeyword K " & _
            '                "LEFT JOIN EgsWItemTranslation T ON T.CodeEgsWTable = 43 AND T.CodeTrans = " & nCodeTrans & " AND T.Code = K.Code " & _
            '                "WHERE ISNULL((case WHEN T.Name='' THEN NULL ELSE T.Name END),K.Name) in ('" & strKeywords & "') AND K.Type=" & intType

            .CommandText = "SELECT K.Code FROM EgsWKeyword K " & _
                           "LEFT JOIN EgsWItemTranslation T ON T.CodeEgsWTable = 43 AND T.CodeTrans = " & nCodeTrans & " AND T.Code = K.Code " & _
                           "WHERE ISNULL((case WHEN T.Name='' THEN NULL ELSE T.Name END),K.Name) in ('" & strKeywords & "')"

            .CommandType = CommandType.Text
            cn.Open()
            dr = .ExecuteReader()
            While dr.Read
                If strKeyCodes <> "" Then strKeyCodes &= ","
                strKeyCodes &= CStr(dr.Item("Code"))
            End While
            dr.Close()
            cn.Close()
        End With
        Return strKeyCodes
    End Function

    Public Function GetListeSearchIngredientsItems(ByVal udtUser As structUser, ByVal slParams As SortedList, _
                                                   ByVal intPagenumber As Integer, ByVal intPageSize As Integer, _
                                                   ByRef intTotalRows As Integer, _
                                                   Optional ByVal blnAllowCreateUseSubRecipe As Boolean = False, _
                                                   Optional ByVal blnFTSEnable As Boolean = False, _
                                                   Optional ByVal intSort As Integer = 0, _
                                                   Optional ByVal intCodeSetPrice As Integer = -1) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable("EGSWLISTE")
        Dim sbSQL As New StringBuilder

        Dim strNumber As String = ""
        Dim strWord As String = ""
        Dim intCategory As Integer
        Dim intListeType As enumDataListType
        Dim intCodeUser As Integer
        Dim shrtNameOption As Short = 2
        Dim shrtNumberOption As Short = 2
        Dim blnGlobalOnly As Boolean = False
        Dim intTypeListe As Integer 'MRC 08.04.08

        intTypeListe = CInt(slParams("LISTETYPE"))
        strNumber = CStr(slParams("NUMBER")).Trim
        strWord = CStr(slParams("WORD")).Trim
        intCategory = CInt(slParams("CATEGORY"))
        intListeType = CType(slParams("TYPE"), enumDataListType)
        intCodeUser = CInt(slParams("CODEUSER"))
        If slParams.Contains("NAMEOPTION") Then shrtNameOption = CShort(slParams("NAMEOPTION"))
        If slParams.Contains("NUMBEROPTION") Then shrtNumberOption = CShort(slParams("NUMBEROPTION"))
        If Not slParams("GLOBALONLY") Is Nothing Then blnGlobalOnly = CBool(slParams("GLOBALONLY"))

        If shrtNameOption <> 0 Then strWord = fctTransformStrSearch(strWord) 'NameOption

        With sbSQL
            .Append("SET NOCOUNT ON ")
            .Append("DECLARE @RecCount int ")
            .Append("SELECT @RecCount = @RecsPerPage * @Page + 1 ")
            .Append("IF @Page=0 SET @Page=1 ")

            .Append("DECLARE @FirstRec int, @LastRec int, @MoreRecords int ")
            .Append("DECLARE @CODETABLECATEGORY int ")
            .Append("DECLARE @CODETABLEUNIT int ")
            .Append("DECLARE @CODETABLELISTE int ")

            .Append("SET @CODETABLECATEGORY = 19 ") '--dbo.fn_egswGetTableID('egswCategory') 
            .Append("SET @CODETABLEUNIT = 135 ") '--dbo.fn_egswGetTableID('egswUnit') 
            .Append("SET @CODETABLELISTE = 50 ") '--dbo.fn_egswGetTableID('egswListe') 

            .Append("SELECT @FirstRec = (@Page - 1) * @RecsPerPage + 1 ")
            .Append("SELECT @LastRec = @Page * @RecsPerPage ")

            .Append(" ;WITH ListePage AS ")
            .Append("( ") 'start of recpage
            .Append("SELECT DISTINCT  CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end name,r.code, r.number, r.dates, ")
            .Append(" 0 as price, ")

            Dim strSort As String = ""
            Select Case intSort
                Case 1 'Name Asc
                    strSort = "(CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) ASC "
                Case 2 'Name Desc
                    strSort = "(CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) DESC "
                Case 3 'Number Asc
                    strSort = "r.Number ASC "
                Case 4 'Number Desc
                    strSort = "r.Number DESC "
                Case 5 'Date Asc
                    strSort = "r.Dates ASC "
                Case 6 'Date Desc
                    strSort = "r.Dates DESC "
            End Select

            If intSort = 0 Then
                .Append(" DENSE_RANK() OVER(Order BY (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) ASC  ) AS ID ")
            Else
                .Append(" DENSE_RANK() OVER(Order BY " & strSort & ") AS ID ")
            End If

            .Append("FROM egswListe r ")
            .Append("INNER JOIN egswSharing ON egswSharing.Code=r.Code AND egswSharing.CodeEgswTable=@CODETABLELISTE ")
            .Append("LEFT OUTER JOIN egswListeTranslation l on r.code=l.codeListe and l.codetrans IN (" & udtUser.CodeTrans & ",NULL) " & " ")


            If intCodeSetPrice <> -1 Then
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste and pCalc.codesetprice=" & intCodeSetPrice & " ")
            Else
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste ")
            End If

            .Append("WHERE ")

            'MRC 08.04.08
            Select Case intTypeListe
                Case enumDataListType.Merchandise
                    .Append("(r.Type=2) ")
                Case enumDataListType.Recipe
                    .Append("(r.type=8 and r.srQty>0) ")
                Case enumDataListType.Text
                    .Append("(r.Type=4) ")
                Case Else
                    Select Case intListeType
                        Case enumDataListType.MenuItems
                            .Append("(r.Type IN (2,4) OR r.type=8) ")
                            .Append("AND r.[use]=1 ")
                        Case enumDataListType.Ingredient
                            If blnAllowCreateUseSubRecipe Then
                                .Append("(r.Type IN (2,4) OR (r.type=8 and r.srQty>0)) ")
                            Else
                                .Append("(r.Type IN (2,4)) ")
                            End If
                            .Append(" and r.[use]=1 ")
                        Case Else
                            .Append("r.Type=" & intListeType & " ")
                    End Select
            End Select

            .Append("AND (egswSharing.CodeUserSharedTo IN (" & udtUser.Site.Code & ") AND egswSharing.Type IN(" & ShareType.CodeSite & "," & ShareType.CodeSiteView & ")) ")
            '.Append(" AND r.protected=0 ") DRR 05.05.2011 commented


            If strWord.Length > 0 Then
                If shrtNameOption = 0 Then 'exact
                    .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) = @nvcWord ")
                ElseIf shrtNameOption = 1 Then
                    strWord = strWord & "%" ' always use like
                    .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")
                Else 'contains
                    strWord = "%" & strWord & "%" ' always use like
                    .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")
                End If
                cmd.Parameters.Add("@nvcWord", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(strWord)
            End If

            If strNumber.Length > 0 Then
                If shrtNumberOption = 0 Then 'exact
                    .Append("AND r.Number = @nvcNumber ")
                ElseIf shrtNumberOption = 1 Then
                    strNumber = strNumber & "%" ' always use like
                    .Append("AND r.Number like @nvcNumber ")
                Else 'contains
                    strNumber = "%" & strNumber & "%" ' always use like
                    .Append("AND r.Number like @nvcNumber ")
                End If
                cmd.Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 25).Value = ReplaceSpecialCharacters(strNumber)
            End If

            'category
            If intCategory > 0 Then
                .Append(" AND r.category=" & intCategory & " ") 'VRP 12.03.2008
            End If

            If blnGlobalOnly Then 'DLS June252007
                .Append(" AND r.IsGlobal=1 ")
            End If
            .Append(") ") 'end of recpage




            .Append("SELECT DISTINCT tr.ID, r.protected, r.code, r.type, CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end name, ")
            .Append("r.category, ")
            .Append("ISNULL(l.codetrans, r.codeTrans) as codeTrans, ")
            .Append("r.yield as yield, r.[percent], y.format as yieldFormat,ISNULL(yT.name,y.namedef) as yieldname, ")
            .Append("ISNULL(pCalc.coeff, 0) AS coeff, ISNULL(pCalc.calcPrice, 0) AS calcPrice, y.code as yieldCode, ")
            .Append("replace(r.number, CHAR(1),'') AS NUMBER, ")
            .Append("r.wastage1,r.wastage2, r.wastage3,r.wastage4, ")
            .Append("r.srUnit,ISNULL(sruT.name,sru.namedef) as srUnitName, ")
            .Append("ISNULL(pCalc.coeff,0) AS coeff1, ")
            .Append("(1-((1-r.Wastage1/100.0) *")
            .Append("(1-r.Wastage2/100.0) * ")
            .Append("(1-r.Wastage3/100.0) * ")
            .Append("(1-r.Wastage4/100.0))) * 100.0 as TotalWastage, ISNULL(pCalc.imposedPrice,0) as ImposedSellingPrice, ")
            .Append("dbo.fn_EgswIsListeOwnedBySite (r.code," & udtUser.Site.Code & ") as sOwner, ")
            .Append("ISNULL(TR.Code, 0) as IsOwner, ")
            .Append("dbo.fn_EgswIsListeOwnedBySystem(r.code) as IsSystemOwned, ") 'checks if it is owned by a ssytem site
            .Append("CASE r.[Use] WHEN 0 THEN 1 ELSE 0 END AS IsDraft, ") 'checks if it is a draft
            .Append("CASE WHEN r.[use]=1 AND r.IsGlobal=1 THEN 1 ELSE 0 END AS IsGlobal, ") 'checks if it is a global, pending for approval is not cionsidered as global yet

            .Append("(SELECT COUNT(*) FROM ListePage) as iRow, ")
            .Append("(SELECT COUNT(*) FROM ListePage WHERE ID>@LastRec) AS MoreRecords, ")

            .Append("dbo.fn_EgswGetSetPriceData(r.code," & intCodeSetPrice & "," & udtUser.CodeTrans & ") as SetPriceData ") ' used in searchlistelist.ascx for setprice computation
            '.Append("dbo.fn_EgswGetSetPrice(r.code," & intCodeSetPrice & "," & udtUser.codetrans & ", '" & strPriceCol & "') as SetPriceValue, ") ' used in searchlistelist.ascx for setprice computation

            'VRP 07.08.2008 'ADD supplier, brand
            .Append(", r.Supplier, s.NameRef AS SupplierName ")
            .Append(", r.Brand, b.Name AS BrandName ")
            '---

            .Append("FROM egswListe r INNER JOIN ListePage tr ON r.code=tr.Code ")
            .Append("LEFT OUTER JOIN egswListeTranslation l on r.code=l.codeListe and l.codetrans IN (" & udtUser.CodeTrans & ",NULL) " & " ")
            .Append("LEFT OUTER JOIN egswUnit y on y.code=r.yieldUnit ")
            .Append("LEFT OUTER JOIN egswItemTranslation yT on y.code=yT.code AND yT.codeTrans IN (" & udtUser.CodeTrans & ",NULL) AND yT.CodeEgswTable=@CODETABLEUNIT AND RTRIM(yT.Name)<>'' ")
            .Append("LEFT OUTER JOIN egswUnit sru on sru.code=r.srunit ")
            .Append("LEFT OUTER JOIN egswItemTranslation sruT on sru.code=sruT.code AND sruT.codeTrans IN (" & udtUser.CodeTrans & ",NULL) AND sruT.CodeEgswTable=@CODETABLEUNIT AND RTRIM(sruT.Name)<>'' ")
            If intCodeSetPrice <> -1 Then
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste and pCalc.codesetprice=" & intCodeSetPrice & " ")
            Else
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste ")
            End If

            'VRP 07.08.2008 'Append brand, Supplier
            .Append("LEFT OUTER JOIN EgswSupplier s ON s.Code=r.Supplier ")
            .Append("LEFT OUTER JOIN EgswBrand b ON b.Code=r.Brand ")
            '---

            '.Append("WHERE TR.ID BETWEEN @FirstRec AND @LastRec ")
            '.Append("ORDER BY tr.ID ")
        End With

        Try
            With cmd
                .Connection = cn
                .CommandText = sbSQL.ToString
                .CommandTimeout = 10000
                .CommandType = CommandType.Text
                .Parameters.Add("@Page", SqlDbType.Int).Value = intPagenumber
                .Parameters.Add("@RecsPerPage", SqlDbType.Int).Value = intPageSize
                .Parameters.Add("@iRow", SqlDbType.Int).Direction = ParameterDirection.Output

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With

            End With
            intTotalRows = 0
            If dt.Rows.Count > 0 Then intTotalRows = CInt(dt.Rows.Item(0).Item("iRow"))

            Return dt
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
            Return Nothing
        End Try
    End Function '-----

    Public Function GetStrForSharing(ByVal strNameTable As String, ByVal intCodeGroup As Integer, ByVal strCodeSite As String, ByVal intCodeUser As Integer) As String
        Dim strX As String = ""

        If strNameTable = "" Then
            strX = "((Type in (2,6) AND CodeUserSharedTo= @CodeProperty) " & _
            "OR(Type in (1,5) AND CodeUserSharedTo= @CodeSite) " & _
            "OR (Type in (3,7,8) AND CodeUserSharedTo= @CodeUser) " & _
            "OR Type in (9,10)) "
        Else
            strX = "((@Table.Type in (2,6) AND @Table.CodeUserSharedTo= @CodeProperty) " & _
            "OR (@Table.Type in (1,5) AND @Table.CodeUserSharedTo in (@CodeSite)) " & _
            "OR (@Table.Type in (3,7,8) AND @Table.CodeUserSharedTo= @CodeUser) " & _
            "OR @Table.Type in (9,10) OR @Table.IsGlobal=1 ) "
        End If
        strX = strX.Replace("@Table", strNameTable)
        strX = strX.Replace("@CodeProperty", intCodeGroup.ToString)
        strX = strX.Replace("@CodeSite", strCodeSite)
        strX = strX.Replace("@CodeUser", intCodeUser.ToString)
        Return strX
    End Function

    Public Function GetRecipesInvolved(ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer) As DataTable
        Try
            Dim cn As New SqlConnection(L_strCnn)
            Dim cmd As New SqlCommand
            Dim da As New SqlDataAdapter
            Dim dt As New DataTable("EGSWLISTE")
            Dim sbSQL As New StringBuilder

            'RJl 9979 12-13-2013
            If intCodeListe = 0 Then
                intCodeListe = -1
            End If

            With sbSQL
                .Append("SELECT DISTINCT CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end name,r.code, r.number ")
                .Append("FROM EgswListe r LEFT OUTER JOIN egswListeTranslation l on r.code=l.codeListe and l.codetrans IN (" & intCodeTrans & ",NULL) " & " ")
                .Append("INNER JOIN EgswDetails d ON d.FirstCode = r.Code ")
                .Append("WHERE SecondCode =" & intCodeListe)
            End With

            With cmd
                .Connection = cn
                .CommandText = sbSQL.ToString
                .CommandTimeout = 10000
                .CommandType = CommandType.Text

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With

                'intTotalRows = CInt(.Parameters("@iRow").Value)
            End With
            Return dt
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
            Return Nothing
        End Try
    End Function


    Public Function GetListeSearchValidateIngr(ByVal intCodeTrans As Integer, ByVal intCodeSite As Integer, ByVal strWord As String) As DataTable
        Try
            Dim cn As New SqlConnection(L_strCnn)
            Dim cmd As New SqlCommand
            Dim da As New SqlDataAdapter

            Dim dt As New DataTable("EGSWLISTE")


            With cmd
                .Connection = cn
                .CommandText = "[GET_LISTESEARCH_VALIDATEINGR]"
                .CommandTimeout = 10000
                .CommandType = CommandType.StoredProcedure
                cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
                cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                cmd.Parameters.Add("@nvcWord", SqlDbType.NVarChar, 200).Value = strWord

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With


            End With
            Return dt
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
            Return Nothing
        End Try
    End Function


    Public Function GetListeSearchResult(ByVal udtUser As structUser, ByVal slParams As SortedList, ByVal intCodeTrans As Integer, ByVal intPagenumber As Integer, ByVal intPageSize As Integer, ByRef intTotalRows As Integer, Optional ByVal blnAllowCreateUseSubRecipe As Boolean = False, Optional ByVal blnFTSEnable As Boolean = False, Optional ByVal strSort As String = "", Optional ByVal intCodeSetPrice As Integer = -1, Optional ByVal SelectedDisplay As enumListeDisplayMode = enumListeDisplayMode.List, Optional ByVal bNutrientSummary As Boolean = True) As DataTable ' ' JBB add opitional parameter selectedDisplay

        'Return GetListeSearchResult2(udtUser, slParams, intCodeTrans, intPagenumber, intPageSize, intTotalRows, blnAllowCreateUseSubRecipe, blnFTSEnable, strSort, intCodeSetPrice)
        'Exit Function

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable("EGSWLISTE")
        Dim sbSQL As New StringBuilder

        Dim strNumber As String = CStr(slParams("NUMBER"))
        'Dim strWord As String = fctTransformStrSearch(CStr(slParams("WORD")))
        Dim strWord As String = CStr(slParams("WORD"))
        Dim strKeywords As String = CStr(slParams("KEYWORDS"))
        Dim strUnKeywords As String = CStr(slParams("KEYUNWANTED")) 'VRP 11.09.2007
        Dim strKeywordType As String = CStr(slParams("KEYTYPE")) 'VRP 18.10.2007
        Dim strCategory As String = CStr(slParams("CATEGORY"))
        Dim strBrand As String = CStr(slParams("BRAND"))
        Dim strSupplier As String = CStr(slParams("SUPPLIER"))
        Dim intListeType As enumDataListType = CType(slParams("TYPE"), enumDataListType)
        Dim strIngredientsWanted As String = fctTransformStrSearch(CStr(slParams("INGWANTED")))
        Dim strIngredientsUnwanted As String = fctTransformStrSearch(CStr(slParams("INGUNWANTED")))
        Dim nUserLevel As enumGroupLevel = CType(slParams("USERLEVEL"), enumGroupLevel)
        Dim strCodeSiteList As String = CStr(slParams("CODESITE"))
        Dim strFilter As String = CStr(slParams("FILTER"))

        'Dim intCodeSite As Integer '= CInt(slParams("CODESITE"))
        Dim intCodeUser As Integer = CInt(slParams("CODEUSER"))
        'Dim intCodeProperty As Integer = CInt(slParams("CODEPROPERTY"))

        Dim strCodelisteList As String = ""
        If Not slParams("MARKITEMLIST") Is Nothing Then strCodelisteList = CStr(slParams("MARKITEMLIST"))

        Dim strSource As String = ""
        If slParams.Contains("SOURCE") Then strSource = CStr(slParams("SOURCE"))

        'JBB 07.04.2011
        Dim intCookMode As Integer = slParams("COOKMODE")

        ' price
        Dim strPrice As String = ""
        If slParams.Contains("PRICE") Then strPrice = CStr(slParams("PRICE"))
        Dim strPriceArr() As String = strPrice.Split(CChar("|"))
        Dim strPriceCol As String = "" ' store price column to search in
        If strPriceArr.Length = 2 Then
            strPriceCol = strPriceArr(0)
            strPrice = strPriceArr(1)
        End If

        If strPrice.IndexOf("-") > 0 Then
            Dim arrPrice() As String = strPrice.Split(CChar("-"))
            strPrice = " BETWEEN " & CDbl(arrPrice(0)).ToString(New System.Globalization.CultureInfo("en-US")) _
                & " AND " & CDbl(arrPrice(1)).ToString(New System.Globalization.CultureInfo("en-US"))
            'strPrice = " BETWEEN " & strPrice.Replace("-", " AND ")
        ElseIf strPrice.Trim.Length > 0 Then
            If strPrice.IndexOf(">") > -1 Then
                strPrice = ">" & CDbl(strPrice.Replace(">", "")).ToString(New System.Globalization.CultureInfo("en-US"))
            ElseIf strPrice.IndexOf("<") > -1 Then
                strPrice = "<" & CDbl(strPrice.Replace("<", "")).ToString(New System.Globalization.CultureInfo("en-US"))
            End If
        End If

        ' if Price value is [Date1]-[date2], insert "BETWEEN" [Date1] "AND" [date2]
        'If strPrice.IndexOf("-") > 0 Then strPrice = " BETWEEN " & strPrice.Replace("-", " AND ")

        ' add price column to search in
        If strPrice.Length > 0 Then strPrice = strPriceCol & " " & strPrice

        ' date
        Dim strDate As String = ""
        If slParams.Contains("DATE") Then strDate = CStr(slParams("DATE"))
        If strDate.IndexOf("-") > 0 Then strDate = " BETWEEN '" & strDate.Replace("-", "' AND '") & "'"
        If strDate.IndexOf("=") > 0 Then strDate = strDate.Replace("=", "='") & "'"

        ' nutrient rules
        Dim strNutrientRules As String = CStr(slParams("NUTRIENTRULES"))
        If strNutrientRules = Nothing Then strNutrientRules = ""

        ' allergens
        Dim strAllergens As String = CStr(slParams("ALLERGENS"))
        If strAllergens = Nothing Then strAllergens = ""

        'sales
        Dim shrtSalesStatus As Short = 0 '0=show all, 1=show linked listes only, 2=show unlinked liste only
        If slParams.Contains("LINKEDSALES") Then shrtSalesStatus = CShort(slParams("LINKEDSALES"))

        Dim shrtNameOption As Short = 2 'contains
        If slParams.Contains("NAMEOPTION") Then shrtNameOption = CShort(slParams("NAMEOPTION"))

        ' ''if name search is not by exact match, transforn text
        ''strWord = strWord.Trim 'DLS
        ''If shrtNameOption <> 0 Then strWord = fctTransformStrSearch(strWord)

        Dim shrtNumberOption As Short = 2 'contains
        If slParams.Contains("NUMBEROPTION") Then shrtNumberOption = CShort(slParams("NUMBEROPTION"))

        'search global only 'DLS JUne252007
        Dim bGlobalOnly As Boolean = False
        If Not slParams("GLOBALONLY") Is Nothing Then bGlobalOnly = CBool(slParams("GLOBALONLY"))

        If strUnKeywords Is Nothing Then strUnKeywords = ""

        'search by code
        Dim blnSearchByCode As Boolean = False
        Dim intCode As Integer = -1
        If slParams.Contains("CODE") Then
            intCode = CInt(slParams.Item("CODE"))
            If intCode > 0 Then blnSearchByCode = True
            If strCodelisteList.Length > 0 Then blnSearchByCode = True
        End If

        'DLS 16.08.2007
        'Dim bExcludeKeywords As Boolean = CBool(slParams.Item("EXCLUDEKEY"))
        Dim nWithNutrientInfo As Integer = CInt(slParams.Item("WITHNUTRIENT"))
        Dim nUsedUnused As Integer = CInt(slParams.Item("USEDUNUSED"))
        Dim nWithComposition As Integer = CInt(slParams.Item("WITHCOMPOSITION"))

        Dim nNutrientEnergy As Integer = CInt(slParams.Item("NUTRIENTENERGY"))

        'nutrient summary
        Dim nNutrientSummary As Integer = CInt(slParams("NUTRIENTSUMMARY")) 'VRP 15.02.2008
        If nNutrientSummary = Nothing Then nNutrientSummary = 0

        'MRC - 09.03.08 - keyword option
        Dim intKeywordOption As Integer = CInt(slParams("KEYWORDOPTION")) 'MRC - 09.03.08
        If intKeywordOption = Nothing Then intKeywordOption = 0
        Dim intKeywordUnwantedOption As Integer = CInt(slParams("KEYWORDUNWANTEDOPTION")) 'MRC - 09.03.08
        If intKeywordUnwantedOption = Nothing Then intKeywordUnwantedOption = 0

        Dim intPictureOption As Integer = CInt(slParams("PICTUREOPTION")) 'VRP 15.09.2008
        Dim nUsedOnline As Integer = CInt(slParams("USEDONLINE")) 'VRP 30.09.2008
        Dim nTranslated As Integer = CInt(slParams("TRANSLATED")) 'VRP 02.02.2009

        Dim nSharedSite As Integer = CInt(slParams("SHAREDSITE")) 'VRP 21.03.2009

        'DLS
        Dim strKeywordsCode As String = ""
        Dim strUnKeywordsCode As String = ""

        'Remove trailing space - MRC - 09.03.08
        If strKeywords.IndexOf(",") > -1 Then
            Dim str() As String = strKeywords.Split(CChar(","))
            strKeywords = ""
            Dim i As Integer = 0
            While i < str.Length
                strKeywords += str(i).Trim
                i += 1
                If i < str.Length Then
                    strKeywords += ","
                End If
            End While
        End If

        'MRC - 09.04.08 - Added type of liste as a param for keywords search.
        If strKeywords <> "" Then
            'comment by ADR 05.10.11
            'strKeywordsCode = GetKeywordsListCode(strKeywords, intCodeTrans, intListeType)
            strKeywordsCode = GetKeywordsListCode(strKeywords, intCodeTrans)
            If strKeywordsCode = "" Then strKeywordsCode = "0" 'VRP 27.02.2009
        End If

        If strUnKeywords <> "" Then
            strUnKeywordsCode = GetKeywordsListCode(strUnKeywords, intCodeTrans, intListeType)
            If strUnKeywordsCode = "" Then strUnKeywordsCode = "0" 'VRP 27.02.2009
        End If
        '----

        '-- JBB Aug 25, 2010
        Dim intMainLanguageFilter As Integer = CIntDB(slParams("MAINLANGUAGEFILTER"))
        '--

        'MRC 10.04.2010
        If intMainLanguageFilter = 0 Then intMainLanguageFilter = -1

        With sbSQL
            .Append("SET NOCOUNT ON ")
            .Append("SET ARITHABORT ON  ") '-- JBB 09.30.2011
            .Append("DECLARE @RecCount int ")
            .Append("SELECT @RecCount = @RecsPerPage * @Page + 1 ")
            .Append("IF @Page=0 SET @Page=1 ")

            .Append("DECLARE @FirstRec int, @LastRec int, @MoreRecords int ")
            .Append("DECLARE @CODETABLECATEGORY int ")
            .Append("DECLARE @CODETABLEUNIT int ")
            .Append("DECLARE @CODETABLELISTE int ")


            '-------- FOR FULL TEXT -------------
            .Append("DECLARE @LANGBREAKER nvarchar(200) ")
            .Append("SELECT @LANGBREAKER = LangBreaker FROM EgsWTranslation WHERE Code= " & intCodeTrans & " ")
            .Append("SET @LANGBREAKER = ISNULL(@LANGBREAKER,'NEUTRAL') ")

            .Append("SET @CODETABLECATEGORY = 19 ") '--dbo.fn_egswGetTableID('egswCategory') 
            .Append("SET @CODETABLEUNIT = 135 ") '--dbo.fn_egswGetTableID('egswUnit') 
            .Append("SET @CODETABLELISTE = 50 ") '--dbo.fn_egswGetTableID('egswListe') 

            '.Append("SELECT @FirstRec = (@Page - 1) * @RecsPerPage ")
            '.Append("SELECT @LastRec = @Page * @RecsPerPage + 1 ")
            .Append("SELECT @FirstRec = (@Page - 1) * @RecsPerPage + 1 ")
            .Append("SELECT @LastRec = @Page * @RecsPerPage ")
            '.Append("CREATE TABLE #TempResults ")
            '.Append("( ")
            '.Append("ID int IDENTITY, ")
            '.Append("code int, ")
            '.Append("name nvarchar(260), ")
            '.Append("number nvarchar(50), ")
            '.Append("dates datetime, ")
            '.Append("price float ")
            '.Append(") ")

            '.Append("INSERT INTO #TempResults (code, name, number, dates, price) ")
            .Append(" ;WITH ListePage AS ")
            .Append("( ") 'start of recpage
            '.Append("SELECT DISTINCT  CASE WHEN ISNULL(l.name,'') = '' THEN r.name ELSE l.name end name,r.code, r.number, r.dates, ")
            '.Append("SELECT DISTINCT  CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name + cast(r.code as varchar(20)) ELSE l.name + cast(r.code as varchar(20)) end name,r.code, r.number, r.dates, ")

            'Added Codesite to sort, for autogrill.
            .Append("SELECT DISTINCT  CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name + cast(r.code as varchar(20)) ELSE l.name + cast(r.code as varchar(20)) end name,r.code, r.number, r.dates, site2.Name AS Site,")

            If intListeType = enumDataListType.Merchandise Then
                .Append(" p.price, ")
            Else
                .Append(" 0 as price, ")
            End If

            ''If strSort = "" Then
            ''    .Append(" ORDER BY [name] ")
            ''Else
            ''    .Append(" ORDER BY " & strSort & " ")
            ''End If
            Dim strSort2 As String
            If strSort Is Nothing Then strSort = ""
            If strSort.ToLower = "name ASC".ToLower Then
                strSort = "r.name ASC"
            ElseIf strSort.ToLower = "name DESC".ToLower Then
                strSort = "r.name DESC"
            End If

            Select Case strSort
                Case "r.name ASC" : strSort2 = "(CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name + '_' + cast(r.code as varchar(20)) ELSE l.name + '_' + cast(r.code as varchar(20)) end) ASC "
                Case "r.name DESC" : strSort2 = "(CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name  + '_' + cast(r.code as varchar(20)) ELSE l.name  + '_' + cast(r.code as varchar(20)) end)DESC "
                Case Else
                    strSort2 = strSort
            End Select

            If strSort = "rank DESC" Then strSort = ""

            If strSort = "" Then
                .Append(" DENSE_RANK() OVER(Order BY (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name + '_' + cast(r.code as varchar(20)) ELSE l.name + '_' + cast(r.code as varchar(20)) end) ASC  ) AS ID ")
                '.Append(" ROW_NUMBER() OVER(Order BY r.name ASC) AS ID ")
            ElseIf strSort = "r.CodeSite ASC" Then
                .Append(" DENSE_RANK() OVER(Order BY (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN site2.Name + '_' + r.name + '_' + cast(r.code as varchar(20)) ELSE site2.Name + '_' + l.name + '_' + cast(r.code as varchar(20)) end) ASC  ) AS ID ")
            ElseIf strSort = "r.CodeSite DESC" Then
                .Append(" DENSE_RANK() OVER(Order BY (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN site2.Name + '_' + r.name + '_' + cast(r.code as varchar(20)) ELSE site2.Name + '_' + l.name + '_' + cast(r.code as varchar(20)) end) DESC  ) AS ID ")
            Else
                .Append(" DENSE_RANK() OVER(Order BY " & strSort2 & ") AS ID ")
            End If

            .Append("FROM egswListe r ")

            .Append("INNER JOIN egswSharing ON egswSharing.Code=r.Code AND egswSharing.CodeEgswTable=@CODETABLELISTE  ")

            If strCodeSiteList = udtUser.Site.Code.ToString Then
                .Append(" AND " & GetStrForSharing("egswSharing", udtUser.Site.Group, strCodeSiteList, udtUser.Code)) 'DLS joining sharing condition
            Else
                .Append(" AND " & GetStrForSharing("egswSharing", -1, strCodeSiteList, -1))
            End If

            .Append("LEFT OUTER JOIN EgswSite site2 ON r.CodeSite=site2.Code ")

            ' Join rnListeTranslation table
            '.Append("LEFT OUTER JOIN egswListeTranslation l on r.code=l.codeListe and l.codetrans IN (" & intCodeTrans & ",NULL) " & " ")

            '.Append("LEFT OUTER JOIN egswListeTranslation l on r.code=l.codeListe and l.codetrans =" & intCodeTrans & " " & " ") 'Comment by ADR 05.12.11
            .Append("INNER JOIN egswListeTranslation l on r.code=l.codeListe and l.codetrans =" & intCodeTrans) ' & " AND RTRIM(l.Name) <> '' ") 'ADR 05.12.11 - Added filter for blank translation 'JTOC 06.11.2012 Removed filter

            '' Join Category table
            '.Append("INNER JOIN  egswCategory c on c.code=r.category  ")
            '.Append("LEFT OUTER JOIN egswItemTranslation cT on c.code=cT.code AND cT.codeTrans IN (" & intCodeTrans & ",NULL) AND cT.CodeEgswTable=@CODETABLECATEGORY AND RTRIM(cT.Name)<>''  ")

            If strBrand Is Nothing Then strBrand = ""
            ' '' Join Brand
            'If strBrand.Length > 0 Then
            '    .Append("LEFT JOIN egswBrand b ON b.code=r.Brand ")
            '    .Append("LEFT OUTER JOIN egswItemTranslation bT on b.code=bT.code AND bT.codeTrans IN (" & intCodeTrans & ",NULL) AND bT.CodeEgswTable=dbo.fn_egswGetTableID('egswBrand') ")
            'End If

            If strSupplier Is Nothing Then strSupplier = ""
            '' Join Supplier
            'If strSupplier.Length > 0 Then
            '.Append("INNER JOIN egswSupplier supplier ON supplier.code=r.Supplier ")
            'End If

            If strSource Is Nothing Then strSource = ""
            '' Join Source
            'If strSource.Length > 0 Then
            '.Append("LEFT OUTER JOIN egswSource source ON source.code=r.Source ")
            'End If

            '' Join Keywords table
            'If strKeywords.Length > 0 Or strUnKeywords.Length > 0 Then
            '    .Append("INNER JOIN egswKeyDetails kd on r.code=kd.codeListe  ")
            '    .Append("INNER JOIN egswKeyword k on kd.codeKey=k.code ")
            '    .Append("LEFT OUTER JOIN egswItemTranslation kT on k.code=kT.code AND kT.codeTrans IN (" & intCodeTrans & ",NULL) AND kT.CodeEgswTable=dbo.fn_egswGetTableID('egswKeyword') AND RTRIM(kT.Name)<>'' ")
            'End If

            ' Join Ingredients, rnListe table for ingredients and traslation rnListe
            If strIngredientsWanted Is Nothing Then strIngredientsWanted = ""
            If strIngredientsUnwanted Is Nothing Then strIngredientsUnwanted = ""

            If strIngredientsWanted.Length > 0 Or strIngredientsUnwanted.Length > 0 Then
                .Append("LEFT OUTER JOIN egswDetails d on r.code=d.firstCode  ")
                .Append("LEFT OUTER JOIN egswListe r1 on  r1.code=d.secondcode ")

                .Append("LEFT OUTER JOIN egswListeTranslation l2 on r1.code=l2.codeListe AND l2.codetrans IN (" & intCodeTrans & ",NULL) ") 'ADR 05.12.11 - under surveillance
            End If

            Select Case intListeType
                Case enumDataListType.Ingredient, enumDataListType.Merchandise
                    ' Join Sub Recipes with prices when searching ingredient and merchandise
                    If intCodeSetPrice <> -1 Then
                        .Append("LEFT OUTER JOIN egswListeSetPrice p on r.code=p.codeliste AND p.codesetprice=" & intCodeSetPrice & " AND p.Position=1 ")
                    Else
                        .Append("LEFT OUTER JOIN egswListeSetPrice p on r.code=p.codeliste AND p.position=1 ")
                    End If
                Case enumDataListType.Recipe, enumDataListType.Menu
                    ' join calculations when seraching recipes / menu
            End Select

            If intCodeSetPrice <> -1 Then
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste and pCalc.codesetprice=" & intCodeSetPrice & " ")
            Else
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste ")
            End If

            '// DRR 08.10.2011
            If strKeywordsCode.Length > 0 Then
                If Not intKeywordOption = 1 Then 'OR
                    .Append(" INNER JOIN EgsWKeyDetails KD on r.Code = KD.Codeliste AND KD.CodeKey IN (" & strKeywordsCode & ") ")
                End If
            End If
            '//

            'join nutrient rules
            If strNutrientRules Is Nothing Then strNutrientRules = ""
            If strNutrientRules.Trim.Length > 0 Then
                .Append("INNER JOIN egswNutrientVal ON r.code=egswNutrientVal.CodeListe ")
            End If



            'join allergens
            If strAllergens Is Nothing Then strAllergens = ""
            If strAllergens.Length > 0 Then
                .Append("LEFT OUTER JOIN egswListeAllergen a ON a.CodeListe=r.code ")
            End If

            .Append("WHERE ")

            If blnSearchByCode Then
                If intCode > 0 Then
                    .Append(" r.code=" & intCode & " ")
                Else
                    .Append(" r.code IN " & strCodelisteList & " ")
                End If
            Else
                ' type
                Select Case intListeType
                    Case enumDataListType.MenuItems
                        .Append("(r.Type IN (2,4) OR r.type=8) ")
                        '.Append("AND r.[use]=1 ")
                    Case enumDataListType.Ingredient
                        If blnAllowCreateUseSubRecipe Then
                            .Append("(r.Type IN (2,4) OR (r.type=8 and r.srQty>0)) ")
                        Else
                            .Append("(r.Type IN (2,4)) ")
                        End If
                        '.Append(" and r.[use]=1 ")
                    Case Else
                        .Append("r.Type=" & intListeType & " ")
                        .Append(" and (r.[use]=1 OR (r.codeUser =" & intCodeUser & " AND r.type IN (2,8,16))) ") 'Exclude drafts
                        '.Append(" and (r.[use]=1 OR (r.codeUser =" & intCodeUser & " AND r.type IN (2,8,16))) ") was moved to egswSharing part (d one hu craeted)
                End Select

                '-- JBB Aug 25, 2010 (Filter Main Language
                If intMainLanguageFilter <> -1 Then .Append(" AND r.codetrans = " + intMainLanguageFilter.ToString() & " ")

                '.Append(" AND r.protected=0 ") 'MRC 05.15.09 we now use this fields

                'DLS Commented
                ''If intListeType = enumDataListType.Merchandise Or intListeType = enumDataListType.Recipe Or intListeType = enumDataListType.Menu Or intListeType = enumDataListType.Ingredient Then
                ''    'check if user is searching his own site, if searching his own site, get all dat is shared to d user, user'site and user's property
                ''    If CStr("," & strCodeSiteList & ",").IndexOf("," & CStr(udtUser.Site.Code) & ",") > -1 Then
                ''        'get sharing of user
                ''        .Append("AND ((r.[use]=1 AND egswSharing.CodeUserSharedTo=" & CStr(udtUser.Site.Group) & " AND egswSharing.Type IN(" & ShareType.CodeProperty & ", " & ShareType.CodePropertyView & ")) ")
                ''        '.Append("OR (r.[use]=1 AND egswSharing.CodeUserSharedTo=" & CStr(udtUser.Site.Code) & " AND egswSharing.Type IN(" & ShareType.CodeSite & ", " & ShareType.CodeSiteView & ")) ")
                ''        .Append("OR (r.[use]=1 AND egswSharing.CodeUserSharedTo IN (" & strCodeSiteList & ") AND egswSharing.Type IN(" & ShareType.CodeSite & ", " & ShareType.CodeSiteView & ")) ")
                ''        .Append("OR (r.[use]=1 AND egswSharing.CodeUserSharedTo=" & CStr(udtUser.Code) & " AND egswSharing.Type IN(" & ShareType.CodeUser & ", " & ShareType.CodeUserView & ")) ")
                ''        .Append("OR (r.[use]=1 AND egswSharing.Type IN(" & ShareType.ExposedViewing & ")) ")

                ''        'd one who created
                ''        If intListeType <> enumDataListType.MenuItems AndAlso intListeType <> enumDataListType.Ingredient Then
                ''            .Append("OR (")
                ''            .Append("(egswSharing.CodeUserSharedTo=" & CStr(intCodeUser) & " ")
                ''            .Append("AND egswSharing.Type=" & ShareType.CodeUserOwner & " ")
                ''            .Append("AND r.[use]=0 AND r.type IN (2,8,16))) ")
                ''        End If
                ''    Else
                ''        .Append(" AND r.[use]=1 AND ((egswSharing.CodeUserSharedTo IN (" & strCodeSiteList & ") AND egswSharing.Type IN(" & ShareType.CodeSite & ")) ")
                ''    End If
                ''    .Append(") ")
                ''Else
                ''    .Append("AND (egswSharing.CodeUserSharedTo IN (" & strCodeSiteList & ") AND egswSharing.Type IN(" & ShareType.CodeSite & ")) ")
                ''End If

                ' Flags
                '.Append(" AND r.protected=0 ") 'MRC 09.01.08

                '' ''If strWord.Length > 0 Then
                '' ''    If shrtNameOption = 0 Then 'exact
                '' ''        '' find match in rnliste table
                '' ''        '.Append("AND (r.Name = @nvcWord ")
                '' ''        '' find match in rnlistetranslation table
                '' ''        '.Append("OR (l.name = @nvcWord ")
                '' ''        '.Append("AND l.codetrans=" & intCodeTrans & ")) ")
                '' ''        'DLS
                '' ''        .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) = @nvcWord ")

                '' ''    ElseIf shrtNameOption = 1 Then
                '' ''        strWord = strWord & "%" ' always use like
                '' ''        ' find match in rnliste table
                '' ''        '.Append("AND (r.Name like @nvcWord ")
                '' ''        '''' find match in rnlistetranslation table
                '' ''        '.Append("OR (l.name like @nvcWord ")
                '' ''        '.Append("AND l.codetrans=" & intCodeTrans & ")) ")
                '' ''        '.Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")
                '' ''        'strWord = fctTransformStrSearch(strWord)

                '' ''        strWord = strWord & "%" ' always use like
                '' ''        .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")
                '' ''    Else 'contains
                '' ''        strWord = "%" & strWord & "%" ' always use like
                '' ''        '' find match in rnliste table
                '' ''        '.Append("AND (r.Name like @nvcWord ")
                '' ''        '''' find match in rnlistetranslation table
                '' ''        '.Append("OR (l.name like @nvcWord ")
                '' ''        '.Append("AND l.codetrans=" & intCodeTrans & ")) ")
                '' ''        'strWord = fctTransformStrSearch(strWord)
                '' ''        strWord = "%" & strWord & "%" ' always use like
                '' ''        .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")
                '' ''    End If

                '' ''    cmd.Parameters.Add("@nvcWord", SqlDbType.NVarChar, 500).Value = strWord.Trim
                '' ''End If

                'DLS
                If strWord.Length > 0 Then
                    If shrtNameOption = 0 Then 'exact
                        '' find match in rnliste table
                        .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) = @nvcWord ")
                    ElseIf shrtNameOption = 1 Then

                        strWord = fctTransformStrSearch(strWord)

                        strWord = strWord & "%" ' always use like
                        .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")


                    Else 'contains
                        Dim flagFullTextEnable As Boolean = IsFullTextEnabled()

                        If fctSearchTextOKForFullText(strWord) = False Then
                            flagFullTextEnable = False
                        End If

                        If flagFullTextEnable = False Then
                            strWord = fctTransformStrSearch(strWord)
                            'mrc - 06.25.09 : Use of Asterisk
                            If strWord.IndexOf("*") > -1 Then
                                strWord = strWord.Replace("*", "%") ' replace asterisk with percent
                                .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")
                            Else
                                strWord = "%" & strWord & "%" ' always use like
                                .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")
                            End If
                        Else
                            strWord = fctTransformStrSearchFullText(strWord)
                            .Append("AND (CONTAINS(r.Name,@nvcWord, LANGUAGE @LANGBREAKER) OR CONTAINS(l.Name,@nvcWord, LANGUAGE @LANGBREAKER))  ")
                        End If


                    End If

                    cmd.Parameters.Add("@nvcWord", SqlDbType.NVarChar, 2000).Value = ReplaceSpecialCharacters(strWord)
                End If

                If strNumber.Length > 0 Then
                    If shrtNumberOption = 0 Then 'exact
                        ' find match in rnliste table
                        .Append("AND r.Number = @nvcNumber ")
                    ElseIf shrtNumberOption = 1 Then
                        strNumber = strNumber & "%" ' always use like
                        ' find match in rnliste table
                        .Append("AND r.Number like @nvcNumber ")
                    Else 'contains
                        strNumber = "%" & strNumber & "%" ' always use like
                        ' find match in rnliste table
                        .Append("AND r.Number like @nvcNumber ")
                    End If
                    cmd.Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 25).Value = ReplaceSpecialCharacters(strNumber)
                End If

                ' Date
                If strDate.Length > 0 Then .Append(" AND r.dates " & strDate & " ")

                ' Price
                Select Case intListeType
                    Case enumDataListType.Merchandise
                        If strPrice.Length <> 0 Then
                            .Append("AND p." & strPrice & " ")
                        End If
                    Case enumDataListType.Recipe, enumDataListType.Menu
                        If strPrice.Length <> 0 Then
                            .Append("AND pCalc." & strPrice & " ")
                        End If
                End Select

                ' Wanted Ingredient Search
                If strIngredientsWanted.Length > 0 Then
                    Dim strSQLEintCodeIng1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR r1.name LIKE ", "@nvcIngWanted", ReplaceSpecialCharacters(strIngredientsWanted), CChar(","), True)
                    Dim strSQLEintCodeIng2 As String = AddParam(cmd, SqlDbType.NVarChar, " OR l2.name LIKE ", "@nvcIng2Wanted", ReplaceSpecialCharacters(strIngredientsWanted), CChar(","), True)
                    Dim strSQLEintCodeIng3 As String = AddParam(cmd, SqlDbType.NVarChar, " OR d.name LIKE ", "@nvcIng3Wanted", ReplaceSpecialCharacters(strIngredientsWanted), CChar(","), True)

                    ' Find match in ingredients
                    .Append("AND (r1.Name like " & strSQLEintCodeIng1 & " ")

                    ' find match ingredient in rnliste translation table
                    .Append("OR (l2.Name like " & strSQLEintCodeIng2 & " ")
                    '.Append("AND l2.codeTrans=" & intCodeTrans & ")) ")

                    .Append("AND l2.codeTrans=" & intCodeTrans & ") ") 'ADR 05.12.11 - under surveillance

                    'MRC - 12.02.08 - Added search of ingredients in textmode
                    .Append("OR d.Name LIKE " & strSQLEintCodeIng3 & ") ")
                End If

                ' Unwanted Ingredient Search
                If strIngredientsUnwanted.Length <> 0 Then
                    Dim strSQLEintCodeIngUw1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR name LIKE ", "@nvcIngUnWanted", ReplaceSpecialCharacters(strIngredientsUnwanted), CChar(","), True)
                    'compare it using egswliste.anme w/codetarns
                    .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select code FROM egswListe WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND CodeTrans=" & intCodeTrans & " ) ) ")
                    'compare it using egswlistetransaltion.name w/codetrans
                    .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select codeliste FROM egswlistetranslation WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND codetrans=" & intCodeTrans & " ) ) ") 'ADR 05.12.11 - under surveillance
                    'compare it using egswliste.name w/o codetrans for liste's w/o translstions
                    .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select code FROM egswListe WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND Code NOT IN (SELECT codeListe FROM egswListeTranslation WHERE CodeTrans=" & intCodeTrans & " )) ) ") 'ADR 05.12.11 - under surveillance
                    'compare to text mode
                    .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE name LIKE " & strSQLEintCodeIngUw1 & ") ")
                End If

                'brand
                If strBrand.Length > 0 Then
                    '.Append("AND (b.name=@nvcBrand OR bT.name=@nvcBrand) ")
                    'cmd.Parameters.Add("@nvcBrand", SqlDbType.NVarChar, 150).Value = strBrand
                    .Append("AND r.brand =" & strBrand & " ") 'VRP 12.03.2008
                End If

                'category
                If strCategory.Length > 0 And strCategory <> "-1" Then
                    '.Append("AND (c.name=@nvcCategory OR cT.name=@nvcCategory) ")
                    'cmd.Parameters.Add("@nvcCategory", SqlDbType.NVarChar, 150).Value = strCategory
                    .Append(" AND r.category=" & strCategory & " ") 'VRP 12.03.2008
                End If

                'source
                If strSource.Length > 0 Then
                    '.Append("AND source.name=@nvcSource ")
                    'cmd.Parameters.Add("@nvcSource", SqlDbType.NVarChar, 150).Value = strSource
                    .Append(" AND r.Source=" & strSource & " ") 'VRP 12.03.2008
                End If

                ' SUPPLIER
                If strSupplier.Length > 0 Then
                    '.Append("AND supplier.nameref=@nvcSupplier ")
                    'cmd.Parameters.Add("@nvcSupplier", SqlDbType.NVarChar, 150).Value = strSupplier
                    .Append("AND r.Supplier=" & strSupplier & " ") 'VRP 12.03.2008
                End If

                'GlobalOnly
                If bGlobalOnly Then 'DLS June252007
                    .Append(" AND r.IsGlobal=1 ")
                End If

                'DLS 17.08.2007
                'Used/UnUsed as Ingredients
                If nUsedUnused = 1 Then 'used
                    .Append(" AND  0 < (SELECT count(SecondCode) FROM EgsWDetails WHERE SecondCode = R.Code) ")
                ElseIf nUsedUnused = 2 Then 'unused
                    .Append(" AND  0 = (SELECT count(SecondCode) FROM EgsWDetails WHERE SecondCode = R.Code) ")
                End If

                'DLS 17.08.2007
                'With Ingredients on Merchandise/ With Composistion on Labels
                If nWithComposition = 1 And intListeType = enumDataListType.Merchandise Then
                    .Append(" AND ISNULL(r.Ingredients,'') <> ''  ")
                ElseIf nWithComposition = 2 And intListeType = enumDataListType.Merchandise Then
                    .Append(" AND ISNULL(r.Ingredients,'') = ''  ")
                ElseIf nWithComposition = 1 And intListeType = enumDataListType.Recipe Then
                    .Append(" AND ISNULL((select Composition FROM EgsWLabel INNER JOIN EgsWProduct ON EgsWProduct.Code = EgsWLabel.CodeProduct AND  EgsWProduct.Code = R.Code ),'') <> ''  ")
                ElseIf nWithComposition = 2 And intListeType = enumDataListType.Recipe Then
                    .Append(" AND ISNULL((select Composition FROM EgsWLabel INNER JOIN EgsWProduct ON EgsWProduct.Code = EgsWLabel.CodeProduct AND  EgsWProduct.Code = R.Code ),'') = ''  ")
                End If

                'DLS 17.08.2007
                'With Nutrient Info or Without
                If nWithNutrientInfo = 1 Then
                    .Append("   AND dbo.fn_EgsWNutrientValIsBlank(r.code)=0    ")
                ElseIf nWithNutrientInfo = 2 Then
                    .Append("   AND dbo.fn_EgsWNutrientValIsBlank(r.code)=1    ")
                End If

                If nNutrientEnergy = 1 Then 'DLS Dec 10 2007
                    .Append("   AND (SELECT top 1 case WHEN ISNULL(N1,-1) =-1 THEN 0 ELSE N1 END FROM EgsWNutrientVal where CodeListe = r.code) > 0    ")
                ElseIf nNutrientEnergy = 2 Then
                    .Append("   AND (SELECT top 1 case WHEN ISNULL(N1,-1) =-1 THEN 0 ELSE N1 END FROM EgsWNutrientVal where CodeListe = r.code) = 0    ")
                End If

                ' Keywords
                If strKeywordsCode.Length > 0 Then
                    Dim strANDKeywords() As String = strKeywordsCode.Split(CChar(","))
                    Dim intX As Integer = UBound(strANDKeywords) + 1
                    If intKeywordOption = 1 And intX > 1 Then    'AND
                        .Append(" AND " & intX & "= (SELECT count(distinct(CodeKey)) FROM EgsWKeyDetails KD WHERE KD.CodeListe = r.Code AND KD.CodeKey in (" & strKeywordsCode & "))")
                        'Else    'OR
                        '    .Append(" AND r.Code in (SELECT CodeListe FROM EgsWKeyDetails WHERE CodeKey in (" & strKeywordsCode & "))") move to inner join DRR 08.11.2011
                    End If
                End If

                If strUnKeywordsCode.Length > 0 Then
                    Dim strANDKeywordsUnwanted() As String = strUnKeywordsCode.Split(CChar(","))
                    Dim intX As Integer = UBound(strANDKeywordsUnwanted) + 1
                    If intKeywordUnwantedOption = 1 And intX > 1 Then
                        .Append(" AND " & intX & "= (SELECT count(distinct(CodeKey)) FROM EgsWKeyDetails KD WHERE KD.CodeListe = r.Code AND KD.CodeKey not in (" & strUnKeywordsCode & "))")
                    Else
                        .Append(" AND r.Code not in (SELECT CodeListe FROM EgsWKeyDetails WHERE CodeKey in (" & strUnKeywordsCode & "))")
                    End If
                End If

                '' Keywords
                'If strUnKeywords.Length > 0 Then
                '    '    Dim strSQLEintCode1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR k.name <> ", "@nvcUNKeyworda", strUnKeywords, CChar(","), True)
                '    '    Dim strSQLEintCode2 As String = AddParam(cmd, SqlDbType.NVarChar, " OR kt.name <> ", "@nvcUNKeywordb", strUnKeywords, CChar(","), True)
                '    '    .Append("AND ((k.name <> " & strSQLEintCode1 & " ")

                '    '    ' find match keyword in keyword parent table translation
                '    '    .Append("OR (kt.name <> " & strSQLEintCode2 & " ")

                '    '    'keyword type
                '    '    If strKeywordType = "Derived" Then
                '    '        .Append("AND kd.Derived = 1 ")
                '    '    ElseIf strKeywords = "Assigned" Then
                '    '        .Append("AND kd.Derived = 0 ")
                '    '    End If

                '    '    .Append("AND kt.codetrans=" & intCodeTrans & "))) ")

                '    .Append(" AND r.Code not in (SELECT CodeListe FROM EgsWKeyDetails WHERE CodeKey in (" & strUnKeywordsCode & "))")
                'End If

                Select Case intPictureOption 'VRP 15.09.2008 picture options
                    Case 1 : .Append(" AND REPLACE(LTRIM(RTRIM(PictureName)), ';','')<>'' ")
                    Case 2 : .Append(" AND REPLACE(LTRIM(RTRIM(PictureName)), ';','')='' ")
                End Select

                If nUsedOnline = 1 Then 'VRP 30.09.2008
                    .Append(" AND r.Online=1 ")
                ElseIf nUsedOnline = 2 Then
                    .Append(" AND r.Online=0 ")
                End If

                If nTranslated = 1 Then 'VRP 02.02.2009
                    .Append(" AND dbo.fn_EgswGetListeTransPerc (r.Code, r.CodeSite, r.Type) >= 100")
                ElseIf nTranslated = 2 Then
                    .Append(" AND dbo.fn_EgswGetListeTransPerc (r.Code, r.CodeSite, r.Type) < 100")
                End If

                'nutrient rules
                If strNutrientRules Is Nothing Then strNutrientRules = ""
                If strNutrientRules.Trim.Length > 0 Then
                    Dim cNutrientRules As clsNutrientRules = New clsNutrientRules(udtUser, enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataSet)
                    Dim arr() As String = strNutrientRules.Split(CChar(","))
                    Array.Sort(arr)

                    Dim i As Integer = 1
                    Dim intLastPosition As Integer = 0
                    Dim arr2() As String
                    While i < arr.Length
                        arr2 = arr(i).Split(CChar("-"))
                        If CInt(arr2(0)) > 0 Then
                            Dim rwTemp As DataRow = CType(cNutrientRules.GetList(CInt(arr2(1))), DataSet).Tables(1).Rows(0)
                            If intLastPosition = CInt(arr2(0)) Then
                                .Append(" OR egswNutrientVal.N" & arr2(0) & " BETWEEN " & CStr(rwTemp("minimum")) & " AND " & CStr(rwTemp("maximum")) & " ")
                            Else
                                If i = 1 Then
                                    .Append(" AND ( ")
                                Else
                                    .Append(" ) AND ( ")
                                End If

                                .Append(" egswNutrientVal.N" & arr2(0) & " BETWEEN " & CStr(rwTemp("minimum")) & " AND " & CStr(rwTemp("maximum")) & " ")
                            End If

                            If i + 1 = arr.Length Then
                                .Append(" ) ")
                            End If

                            intLastPosition = CInt(arr2(0))
                        End If
                        i += 1
                    End While
                End If

                If strAllergens Is Nothing Then strAllergens = ""
                If strAllergens.Length > 0 Then
                    If strAllergens.IndexOf("NOT") > -1 Then
                        .Append(" AND (a.codeAllergen " & strAllergens & " OR a.codeAllergen IS NULL) ")
                    Else
                        .Append(" AND a.codeAllergen " & strAllergens & " ")
                    End If
                End If

                'filter, this only works wen u r searching ur own site
                'If CStr(strCodeSiteList) = CStr(udtUser.Site.Code) Then
                Select Case UCase(strFilter)
                    Case "1" '"'OWNED"
                        .Append(" AND r.CodeSite = " & strCodeSiteList & " ")
                    Case "2" '"PUBLIC"
                        .Append(" AND r.Code IN (SELECT S.Code FROM egsWSharing S WHERE S.CodeEGSWTable=50 AND S.Code = r.Code AND (S.IsGlobal=1 OR S.Type not in (1,8))  ) ")
                    Case "3" '"PRIVATE"
                        .Append(" AND r.Code NOT IN (SELECT S.Code FROM egsWSharing S WHERE S.CodeEGSWTable=50 AND S.Code = r.Code  AND (S.IsGlobal=1 OR S.Type not in (1,8)) ) ")
                    Case "4" '"SHARED"
                        '.Append(" AND r.CodeSite <> " & strCodeSiteList & " ")
                        If nSharedSite <> 0 Then 'VRP 21.03.2009
                            .Append(" AND r.CodeSite= " & nSharedSite & " ")
                        Else
                            .Append(" AND r.CodeSite <> " & strCodeSiteList & " ")
                        End If
                    Case "5" '"DRAFT"
                        .Append(" AND r.[use]=0 and r.submitted=0 ")
                        'Case "6" '"SYSTEM"
                        '    .Append(" AND dbo.fn_EgswIsListeOwnedBySystem(r.code)>0 ")
                    Case "6" '"For Approval" 'DLSXXXXXX
                        .Append(" AND r.submitted=1 ")
                    Case "7" '"Approved" 'DLSXXXXXX
                        .Append(" AND r.approvalstatus=1 AND r.submitted=0 ")
                    Case "8" '"Not Approved" 'DLSXXXXXX
                        .Append(" AND r.approvalstatus=2 AND r.submitted=0 ")
                End Select
                'end if
            End If

            If shrtSalesStatus = 1 Then 'linked only
                If intListeType = enumDataListType.Merchandise Then
                    .Append(" AND r.Code IN (SELECT CodeListe FROM egswLinkFbRnPOS linkMP WHERE linkMP.TypeLink IN (0) AND linkMP.CodeListe=r.code ") 'for merchandise and product
                    .Append("       AND linkMP.CodeProduct IN (SELECT CodeProduct FROM egswLinkFbRnPOS linkPS WHERE linkPS.TypeLink IN (1) AND linkPS.CodeProduct=linkMP.codeProduct)) ") 'for product and salesitem
                ElseIf intListeType = enumDataListType.Recipe OrElse intListeType = enumDataListType.Menu Then
                    .Append(" AND r.Code IN (SELECT CodeListe FROM egswLinkFbRnPOS linkLS WHERE linkLS.TypeLink IN (2) AND linkLS.CodeListe=r.code )") ' for recipes/menus and salesitem
                End If
            ElseIf shrtSalesStatus = 2 Then 'not linked only
                If intListeType = enumDataListType.Merchandise Then
                    .Append(" AND r.Code NOT IN (SELECT CodeListe FROM egswLinkFbRnPOS linkMP WHERE linkMP.TypeLink IN (0) AND linkMP.CodeListe=r.code ") 'for merchandise and product
                    .Append("       AND linkMP.CodeProduct IN (SELECT CodeProduct FROM egswLinkFbRnPOS linkPS WHERE linkPS.TypeLink IN (1) AND linkPS.CodeProduct=linkMP.codeProduct)) ") 'for product and salesitem
                ElseIf intListeType = enumDataListType.Recipe OrElse intListeType = enumDataListType.Menu Then
                    .Append(" AND r.Code NOT IN (SELECT CodeListe FROM egswLinkFbRnPOS WHERE TypeLink IN (2) AND CodeListe=r.code )") ' for recipes/menus
                End If
            End If

            'JBB 07.02.2011
            If intListeType = enumDataListType.Recipe Then
                If intCookMode <> -1 Then
                    .Append(" AND  ISNULL(r.CookMode,0)=" & intCookMode)
                End If
            End If

            ''If strSort = "" Then
            ''    .Append(" ORDER BY [name] ")
            ''Else
            ''    .Append(" ORDER BY " & strSort & " ")
            ''End If

            .Append(") ") 'end of recpage



            '.Append("SELECT @iRow=COUNT(*) FROM ListePage ")
            '.Append("SELECT @MoreRecords=COUNT(*) FROM ListePage WHERE ID>@LastRec ")

            '.Append("DELETE FROM #TempResults WHERE ID <= @FirstRec OR ID >=@LastRec ")

            ''BuildFullySharedString(sbSQL, udtUser, intListeType, cmd)
            .Append("SELECT DISTINCT tr.ID, r.protected, r.code, r.type, CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end name, ")
            .Append("r.category, r.preparation, r.picturename, ")
            .Append("r.submitted, ISNULL(l.codetrans, r.codeTrans) as codeTrans, ")
            .Append("r.yield as yield, r.[percent], y.format as yieldFormat,ISNULL(yT.name,y.namedef) as yieldname, ")
            .Append("ISNULL(pCalc.coeff, 0) AS coeff, ISNULL(pCalc.calcPrice, 0) AS calcPrice,ISNULL(pCalc.imposedPrice, 0) AS imposedPrice, ISNULL(pCalcTax.Value,0) as TaxValue, c.name AS categoryname, y.code as yieldCode, ")
            .Append("r.Supplier, r.source, r.remark, ")
            '.Append("r.note, r.dates, r.submitted, replace(r.number, CHAR(1),'') AS NUMBER, ")
            .Append("r.dates, r.submitted, replace(r.number, CHAR(1),'') AS NUMBER, ")
            .Append("r.wastage1,r.wastage2, r.wastage3,r.wastage4, ")
            .Append("r.picturename, ")
            .Append("r.srUnit,ISNULL(sruT.name,sru.namedef) as srUnitName, ")
            .Append("ISNULL(pCalc.coeff,0) AS coeff1, ")
            '.Append("r.currency, pCalc.coeff,")

            .Append("(1-((1-r.Wastage1/100.0) *")
            .Append("(1-r.Wastage2/100.0) * ")
            .Append("(1-r.Wastage3/100.0) * ")
            .Append("(1-r.Wastage4/100.0))) * 100.0 as TotalWastage, ISNULL(pCalc.imposedPrice,0) as ImposedSellingPrice, ")
            '.Append("dbo.fn_EgswIsListeOwnedBySite (r.code," & udtUser.Site.Code & ") as sOwner, ")
            .Append("r.CodeSite as sOwner, ")
            '.Append("0 as sOwner, ")
            .Append("ISNULL(TR.Code, 0) as IsOwner, ")
            '.Append("dbo.fn_EgswCheckListeFullySharedEditToUser(" & udtUser.Code & ", r.code) as IsOwner, ")
            '.Append("dbo.fn_EgswIsListeOwnedBySystem(r.code) as IsSystemOwned, ") 'checks if it is owned by a ssytem site
            .Append("0 as IsSystemOwned, ") 'checks if it is owned by a ssytem site
            '.Append("0 as IsSystemOwned, ") 'checks if it is owned by a ssytem site
            .Append("CASE r.[Use] WHEN 0 THEN 1 ELSE 0 END AS IsDraft, ") 'checks if it is a draft
            .Append("CASE WHEN r.[use]=1 AND r.IsGlobal=1 THEN 1 ELSE 0 END AS IsGlobal, ") 'checks if it is a global, pending for approval is not cionsidered as global yet
            '.Append("@MoreRecords AS MoreRecords, ")

            .Append(" (SELECT COUNT(*) FROM ListePage) as iRow, ")
            .Append("(SELECT COUNT(*) FROM ListePage WHERE ID>@LastRec) AS MoreRecords, ")

            .Append("dbo.fn_EgswGetSetPriceData(r.code," & intCodeSetPrice & "," & intCodeTrans & ") as SetPriceData, ") ' used in searchlistelist.ascx for setprice computation
            .Append("dbo.fn_EgswGetSetPrice(r.code," & intCodeSetPrice & "," & intCodeTrans & ", '" & strPriceCol & "') as SetPriceValue, ") ' used in searchlistelist.ascx for setprice computation
            .Append("ISNULL(product.Code, 0) AS CodeFG ") ' unit of product/salesitem 
            '.Append("ISNULL(link.CodeUnitProduct, 0) AS CodeUnitProduct, ") ' unit of product/salesitem 
            '.Append("ISNULL(link.CodeProduct, 0) AS CodeProduct ") ' unit of product/salesitem 

            'Select Case intListeType
            '    Case enumDataListType.Ingredient, enumDataListType.Merchandise
            '        .Append("p.price as Price ")
            '    Case Else
            '        .Append("0 as Price ")
            'End Select
            If bNutrientSummary = True Then 'JTOC 09.11.2013 Reenabled 'VRP 15.02.2008
                .Append(", egswNutrientVal.N1 AS N1,egswNutrientVal.N2 AS N2,egswNutrientVal.N3 AS N3,egswNutrientVal.N4 AS N4,egswNutrientVal.N5 AS N5,egswNutrientVal.N6 AS N6,egswNutrientVal.N7 AS N7,egswNutrientVal.N8 AS N8,egswNutrientVal.N9 AS N9,egswNutrientVal.N10 AS N10,egswNutrientVal.N11 AS N11,egswNutrientVal.N12 AS N12,egswNutrientVal.N13 AS N13,egswNutrientVal.N14 AS N14,egswNutrientVal.N15 AS N15 ")
                .Append(", egswNutrientVal.N16 AS N16,egswNutrientVal.N17 AS N17,egswNutrientVal.N18 AS N18,egswNutrientVal.N19 AS N19,egswNutrientVal.N20 AS N20,egswNutrientVal.N21 AS N21,egswNutrientVal.N22 AS N22,egswNutrientVal.N23 AS N23,egswNutrientVal.N24 AS N24,egswNutrientVal.N25 AS N25,egswNutrientVal.N26 AS N26,egswNutrientVal.N27 AS N27,egswNutrientVal.N28 AS N28,egswNutrientVal.N29 AS N29,egswNutrientVal.N30 AS N30 ") 'ADR 04.27.11
                .Append(", egswNutrientVal.N31 AS N31,egswNutrientVal.N32 AS N32,egswNutrientVal.N33 AS N33,egswNutrientVal.N34 AS N34 ") 'ADR 04.27.11
            End If

            .Append(", r.CodeSite, ISNULL(r.CodeUser,0) as CodeUser, EgswSite.Name AS SiteName, ISNULL(r.[Use],0) as ListeUse, ISNULL(r.ApprovalStatus,0) as ApprovalStatus ") 'DLS

            .Append(", supplier.NameRef AS SupplierName ") 'MRC 08.04.08
            .Append(", source.Name AS SourceName ") 'MRC 08.06.08
            .Append(", r.Brand AS Brand ") 'MRC 08.06.08
            .Append(", CASE WHEN b.name IS NULL OR LEN(RTRIM(LTRIM(b.name)))=0 THEN bT.Name ELSE b.name END AS BrandName ")
            .Append(", r.Protected AS Protected ") 'MRC 09.01.08

            '// DRR 12.28.2010
            .Append(", NULL as QuantityMetric ")
            .Append(", NULL as UnitMetric ")
            .Append(", NULL as CodeUnitMetric ")
            .Append(", NULL as QuantityImperial ")
            .Append(", NULL as UnitImperial ")
            .Append(", NULL as CodeUnitImperial ")
            .Append(", '' as AlternativeIngredient ")
            .Append(", '' as UnitMetric ")
            .Append(", '' as UnitImperial ")
            '//

            '// DRR 2.15.2011
            .Append(", ISNULL(r.checkoutuser,0) as Checkoutuser ")
            '//

            '// DRR 03.04.2011
            .Append(", '' as Description ")
            '//

            '// DRR 06.29.2011
            .Append(", '' as Tip ")
            .Append(", '' as DigitalAsset ")
            '//

            '// DRR 07.20.2011
            .Append(", 0 as FreakOutMoment ")
            '//


            '-- JBB 09.28.2011
            .Append(",Version ")
            '-- JBB 09.30.2011
            If intListeType = 8 Then
                .Append(", CASE WHEN l.SubTitle IS NULL OR LEN(RTRIM(LTRIM(l.SubTitle)))=0 THEN r.SubTitle ELSE l.SubTitle end  SubTitle ")
                .Append(",CASE WHEN ISNULL(r.RecipeState,0) = 0 THEN (SELECT Name from EgswStatus where Code =1 and Type =0) else s1.name end RecipeStatus ")
                .Append(",CASE WHEN ISNULL(r.WebState,0) = 0 THEN (SELECT Name from EgswStatus where Code =1 and Type =1) else s2.name end WebStatus ")
                .Append(", egswNutrientVal.DisplayNutrition as DisplayNutrition ")
                .Append(", case when isnull(r.defaultpicture,0) = 0 then cast(0 as bit) else cast(1 as bit) end  as  ImageDisplay ")
                .Append(",ISNULL(Bd.Name,'') as PrimaryBrand ")
                .Append(",(SELECT ")
                .Append("( ")
                .Append("  SELECT ")
                .Append("  ( ")
                .Append("    SELECT  ")
                .Append("      n + '<br>' AS [text()] ")
                .Append("            FROM ")
                .Append("    ( ")
                .Append("      SELECT name as n ")
                .Append("		FROM recipebrand rb ")
                .Append("		inner join egswbrand b on b.Code = rb.Brand ")
                .Append("           where(rb.codeliste = r.Code And rb.brandclassification = 2) ")
                .Append("    ) r  ")
                .Append("    FOR XML PATH(''), TYPE ")
                .Append("  ) AS concat ")
                .Append("  FOR XML RAW, TYPE ")
                .Append(").value('/row[1]/concat[1]', 'varchar(max)')) AS SecondaryBrand ")
                '--
            End If
            .Append("FROM egswListe r INNER JOIN ListePage tr ON r.code=tr.Code ")

            If bNutrientSummary = True Then 'JTOC 09.11.2013 Reenabled  'VRP 15.02.2008
                .Append("LEFT OUTER JOIN egswNutrientVal ON tr.Code=egswNutrientVal.CodeListe ")
            End If
            'this was just amede to check if user has edit and owner capabilities
            '.Append("LEFT OUTER JOIN dbo.fn_EgswGetListeFullySharedEditToUserByCodeUser(" & udtUser.Code & ", " & intListeType & ") fullyShared ON r.Code=fullyShared.Code ")

            ''.Append("LEFT OUTER JOIN @tblFullySharedWithEdit fullyShared ON r.Code=fullyShared.Code ")

            ''.Append("INNER JOIN egswSharing ON egswSharing.Code=r.Code AND egswSharing.CodeEgswTable=@CODETABLELISTE ")

            ' Join rnListeTranslation table
            '.Append("LEFT OUTER JOIN egswListeTranslation l on r.code=l.codeListe and l.codetrans IN (" & intCodeTrans & ",NULL) " & " ") 'Comment by ADR 05.12.11
            .Append("INNER JOIN egswListeTranslation l on r.code=l.codeListe and l.codetrans = " & intCodeTrans) ' & " AND RTRIM(l.Name) <> '' ") 'ADR 05.12.11 - Added filter for blank translation'JTOC 06.11.2012 Removed filter

            ' Join Yield table
            .Append("LEFT OUTER JOIN egswUnit y on y.code=r.yieldUnit ")
            .Append("LEFT OUTER JOIN egswItemTranslation yT on yT.CodeEgswTable=@CODETABLEUNIT AND y.code=yT.code AND yT.codeTrans IN (" & intCodeTrans & ",NULL) AND RTRIM(yT.Name)<>'' ")

            'Join Unit table for SubRecipe unit
            .Append("LEFT OUTER JOIN egswUnit sru on sru.code=r.srunit ")
            .Append("LEFT OUTER JOIN egswItemTranslation sruT on sruT.CodeEgswTable=@CODETABLEUNIT  AND sru.code=sruT.code AND sruT.codeTrans IN (" & intCodeTrans & ",NULL) AND RTRIM(sruT.Name)<>'' ")

            ' Join Category table
            .Append("INNER JOIN  egswCategory c on c.code=r.category ")
            .Append("LEFT OUTER JOIN egswItemTranslation cT on cT.CodeEgswTable=@CODETABLECATEGORY AND c.code=cT.code AND cT.codeTrans IN (" & intCodeTrans & ",NULL)  AND RTRIM(cT.Name)<>''  ")

            'join product table for finished goods in recipe
            .Append("LEFT OUTER JOIN egswProduct product ON r.Code=product.RecipeLinkCode ")

            .Append("INNER JOIN egswSupplier supplier ON supplier.code=r.Supplier ") 'MRC 08.04.08
            .Append("LEFT OUTER JOIN egswSource source ON source.code=r.Source ") 'MRC 08.06.08

            'Join Product Table for merchandise linking of product for salesitem
            '.Append("LEFT OUTER JOIN egswLinkFbRnPOS link on link.CodeListe=r.code AND link.TypeLink=0 ")

            ' Join Brand
            'If strBrand.Length > 0 Then
            .Append("LEFT JOIN egswBrand b ON b.code=r.Brand ")
            .Append("LEFT OUTER JOIN egswItemTranslation bT on c.code=bT.code AND bT.codeTrans IN (" & intCodeTrans & ",NULL) AND bT.CodeEgswTable=18 ")
            'End If

            ' Join Supplier
            'If strSupplier.Length > 0 Then
            '    .Append("INNER JOIN egswSupplier supplier ON supplier.code=r.Supplier ")
            'End If

            ' Join Source
            'If strSource.Length > 0 Then
            '    .Append("LEFT OUTER JOIN egswSource source ON source.code=r.Source ")
            'End If

            ' Join Keywords table
            'If strKeywords.Length <> 0 Then
            '    .Append("INNER JOIN egswKeyDetails kd on r.code=kd.codeListe  ")
            '    .Append("INNER JOIN egswKeyword k on kd.codeKey=k.code ")
            '    .Append("LEFT OUTER JOIN egswItemTranslation kT on k.code=kT.code AND kT.codeTrans IN (" & intCodeTrans & ",NULL) AND kT.CodeEgswTable=dbo.fn_egswGetTableID('egswKeyword') AND RTRIM(kT.Name)<>'' ")
            'End If

            ' Join Ingredients, rnListe table for ingredients and traslation rnListe
            'If strIngredientsWanted.Length <> 0 Or strIngredientsUnwanted.Length <> 0 Then
            '    .Append("LEFT OUTER JOIN egswDetails d on r.code=d.firstCode  ")
            '    .Append("LEFT OUTER JOIN egswListe r1 on  r1.code=d.secondcode ")
            '    .Append("LEFT OUTER JOIN egswListeTranslation l2 on r1.code=l2.codeListe AND l2.codetrans IN (" & intCodeTrans & ",NULL) ")
            'End If

            'Select Case intListeType
            '    Case enumDataListType.Ingredient, enumDataListType.Merchandise
            '        ' Join Sub Recipes with prices when searching ingredient and merchandise
            '        If intCodeSetPrice <> -1 Then
            '            .Append("LEFT OUTER JOIN egswListeSetPrice p on r.code=p.codeliste AND p.codesetprice=" & intCodeSetPrice & " ")
            '        Else
            '            .Append("LEFT OUTER JOIN egswListeSetPrice p on r.code=p.codeliste ")
            '        End If
            '    Case enumDataListType.Recipe, enumDataListType.Menu
            '        ' join calculations when seraching recipes / menu
            'End Select

            If intCodeSetPrice <> -1 Then
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste and pCalc.codesetprice=" & intCodeSetPrice & " ")
            Else
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste ")
            End If
            .Append("LEFT OUTER JOIN EgsWTax pCalcTax on pCalc.tax = pCalcTax.Code ") 'DLS May52009

            .Append("INNER JOIN EgswSite egswSite ON r.CodeSite=egswSite.Code ") 'VRP 19.05.2008

            'If strNutrientRules.Trim.Length > 0 Then
            '    'join nutrient rules
            '    .Append("INNER JOIN egswNutrientVal ON r.code=egswNutrientVal.CodeListe ")
            'End If

            'JBB 10.20.2011
            If intListeType = 8 Then
                .Append("LEFT OUTER JOIN EgswStatus S1 ON S1.Code = r.RecipeState and S1.Type = 0 ")
                .Append("LEFT OUTER JOIN EgswStatus S2 ON S2.Code = r.WebState and S2.Type = 1 ")
                .Append("Left outer join egswbrand Bd on Bd.Code = r.Brand ")
            End If


            'JTOC 09.11.2013 .Append("WHERE TR.ID BETWEEN @FirstRec AND @LastRec ")
            ''BuildFullySharedString2(sbSQL, udtUser, intListeType, cmd)

            .Append("ORDER BY tr.ID ")
        End With

        Try
            With cmd
                .Connection = cn
                .CommandText = sbSQL.ToString
                .CommandTimeout = 10000
                .CommandType = CommandType.Text
                .Parameters.Add("@Page", SqlDbType.Int).Value = intPagenumber
                .Parameters.Add("@RecsPerPage", SqlDbType.Int).Value = intPageSize
                .Parameters.Add("@iRow", SqlDbType.Int).Direction = ParameterDirection.Output

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With

                'intTotalRows = CInt(.Parameters("@iRow").Value)
            End With
            intTotalRows = 0
            If dt.Rows.Count > 0 Then intTotalRows = CInt(dt.Rows.Item(0).Item("iRow"))

            ' IsListeOwned(dt, udtUser.Site.Code)
            Return dt

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
            Return Nothing
        End Try
    End Function

    Public Function GetListeSearchResult2(ByVal udtUser As structUser, ByVal slParams As SortedList, ByVal intCodeTrans As Integer, ByVal intPagenumber As Integer, ByVal intPageSize As Integer, ByRef intTotalRows As Integer, Optional ByVal blnAllowCreateUseSubRecipe As Boolean = False, Optional ByVal blnFTSEnable As Boolean = False, Optional ByVal strSort As String = "", Optional ByVal intCodeSetPrice As Integer = -1, Optional ByVal bIncorrectNetMargin As Boolean = False, Optional ByVal dblMinimumNetMargin As Double = -1, Optional ByVal dblMaximumNetMargin As Double = -1, Optional ByVal strCodeListeIncorrectMargin As String = "", Optional ByVal blnUseImposedPriceForSubRecipe As Boolean = False, Optional ByVal blnByPassSharing As Boolean = False) As DataTable

        'Return GetListeSearchResult2(udtUser, slParams, intCodeTrans, intPagenumber, intPageSize, intTotalRows, blnAllowCreateUseSubRecipe, blnFTSEnable, strSort, intCodeSetPrice)
        'Exit Function

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable("EGSWLISTE")
        Dim sbSQL As New StringBuilder

        Dim strNumber As String = CStr(slParams("NUMBER"))
        'Dim strWord As String = fctTransformStrSearch(CStr(slParams("WORD")))
        Dim strWord As String = CStr(slParams("WORD"))
        Dim strKeywords As String = CStr(slParams("KEYWORDS"))
        Dim strUnKeywords As String = CStr(slParams("KEYUNWANTED")) 'VRP 11.09.2007
        Dim strKeywordType As String = CStr(slParams("KEYTYPE")) 'VRP 18.10.2007
        Dim strCategory As String = CStr(slParams("CATEGORY"))
        Dim strBrand As String = CStr(slParams("BRAND"))
        Dim strSupplier As String = CStr(slParams("SUPPLIER"))
        Dim intListeType As enumDataListType = CType(slParams("TYPE"), enumDataListType)
        Dim intListeTypeFilter As enumDataListType = CType(slParams("LISTETYPE"), enumDataListType) 'mrc 02.11.2010
        Dim strIngredientsWanted As String = fctTransformStrSearch(CStr(slParams("INGWANTED")))
        Dim strIngredientsUnwanted As String = fctTransformStrSearch(CStr(slParams("INGUNWANTED")))
        Dim nUserLevel As enumGroupLevel = CType(slParams("USERLEVEL"), enumGroupLevel)
        Dim strCodeSiteList As String = CStr(slParams("CODESITE"))
        Dim strFilter As String = CStr(slParams("FILTER"))

        'Dim intCodeSite As Integer '= CInt(slParams("CODESITE"))
        Dim intCodeUser As Integer = CInt(slParams("CODEUSER"))
        'Dim intCodeProperty As Integer = CInt(slParams("CODEPROPERTY"))

        Dim strCodelisteList As String = ""
        If Not slParams("MARKITEMLIST") Is Nothing Then strCodelisteList = CStr(slParams("MARKITEMLIST"))

        Dim strSource As String = ""
        If slParams.Contains("SOURCE") Then strSource = CStr(slParams("SOURCE"))

        ' price
        Dim strPrice As String = ""
        If slParams.Contains("PRICE") Then strPrice = CStr(slParams("PRICE"))
        Dim strPriceArr() As String = strPrice.Split(CChar("|"))
        Dim strPriceCol As String = "" ' store price column to search in
        If strPriceArr.Length = 2 Then
            strPriceCol = strPriceArr(0)
            strPrice = strPriceArr(1)
        End If

        If strPrice.IndexOf("-") > 0 Then
            Dim arrPrice() As String = strPrice.Split(CChar("-"))
            'strPrice = " BETWEEN " & CDbl(arrPrice(0)).ToString(New System.Globalization.CultureInfo("en-US")) _
            '                & " AND " & CDbl(arrPrice(1)).ToString(New System.Globalization.CultureInfo("en-US"))

            strPrice = " BETWEEN "
            If CDbl(arrPrice(0)) < CDbl(arrPrice(1)) Then
                strPrice = strPrice & CDbl(arrPrice(0)).ToString(New System.Globalization.CultureInfo("en-US")) _
                          & " AND " & CDbl(arrPrice(1)).ToString(New System.Globalization.CultureInfo("en-US"))
            Else
                strPrice = strPrice & CDbl(arrPrice(1)).ToString(New System.Globalization.CultureInfo("en-US")) _
                          & " AND " & CDbl(arrPrice(0)).ToString(New System.Globalization.CultureInfo("en-US"))
            End If

            'strPrice = " BETWEEN " & strPrice.Replace("-", " AND ")
        ElseIf strPrice.Trim.Length > 0 Then
            If strPrice.IndexOf(">") > -1 Then
                strPrice = ">" & CDbl(strPrice.Replace(">", "")).ToString(New System.Globalization.CultureInfo("en-US"))
            ElseIf strPrice.IndexOf("<") > -1 Then
                strPrice = "<" & CDbl(strPrice.Replace("<", "")).ToString(New System.Globalization.CultureInfo("en-US"))
            End If
        End If

        ' if Price value is [Date1]-[date2], insert "BETWEEN" [Date1] "AND" [date2]
        'If strPrice.IndexOf("-") > 0 Then strPrice = " BETWEEN " & strPrice.Replace("-", " AND ")

        ' add price column to search in
        Dim blnSpecialPrice As Boolean = False
        If strPrice.Length > 0 Then
            Select Case strPriceCol.ToLower
                Case "netmarginpercent"
                    strPriceCol = "(100 - ((pcalc.calcPrice / ISNULL(NULLIF((pCalc.imposedPrice / (ISNULL(NULLIF(pCalcTax.Value,0),1) + 100) * 100), 0), ISNULL(NULLIF(pcalc.calcPrice, 0), 1)) ) * 100)) "
                    blnSpecialPrice = True
                Case "tax"
                    strPriceCol = "pCalcTax.Value "
                    blnSpecialPrice = True
                Case "grossmargin"
                    strPriceCol = "(ISNULL(pCalc.calcPrice,0) * ISNULL(pCalc.CoEff,0)) - ISNULL(pCalc.calcPrice,0) "
                    blnSpecialPrice = True
                Case "grossmarginpercent"
                    strPriceCol = "100 - ISNULL(NULLIF((((ISNULL(pCalc.calcPrice,0) * ISNULL(pCalc.CoEff,0)) - (ISNULL(pCalc.calcPrice,0) * ISNULL(pCalc.CoEff,0) - ISNULL(pCalc.calcPrice,0))) / ISNULL(NULLIF((ISNULL(pCalc.calcPrice,0) * ISNULL(pCalc.CoEff,0)),0),1)) * 100,0), 100) "
                    blnSpecialPrice = True
                Case "sellingprice"
                    strPriceCol = "ISNULL(pCalc.calcPrice, 0) * ISNULL(pCalc.CoEff,0) "
                    blnSpecialPrice = True
                Case "sellingpricetax"
                    strPriceCol = "(ISNULL(pCalc.calcPrice, 0) * ISNULL(pCalc.CoEff,0)) + ((ISNULL(pCalc.calcPrice, 0) * ISNULL(pCalc.CoEff,0)) * (ISNULL(pCalcTax.Value, 0)/100)) "
                    blnSpecialPrice = True
                Case "foodcost"
                    strPriceCol = "(ISNULL(pCalc.calcPrice, 0) * ISNULL(pCalc.CoEff,0)) - ((ISNULL(pCalc.calcPrice,0) * ISNULL(pCalc.CoEff,0)) - ISNULL(pCalc.calcPrice,0)) "
                    blnSpecialPrice = True
                Case "imposedfoodcostpercent"
                    strPriceCol = "ISNULL(pCalc.calcPrice, 0) / ISNULL(NULLIF(ISNULL(pCalc.imposedPrice, 0) / (100 + ISNULL(pCalcTax.Value, 0)), 0), 1) "
                    'strPriceCol = "(ISNULL(pCalc.calcPrice, 0) / ISNULL(NULLIF(ISNULL(pCalc.imposedPrice, 0) / (100 + ISNULL(pCalcTax.Value, 0)), 0), 1)) * 100 "
                    blnSpecialPrice = True
            End Select
            strPrice = strPriceCol & " " & strPrice
        End If


        ' date
        Dim strDate As String = ""
        If slParams.Contains("DATE") Then strDate = CStr(slParams("DATE"))
        If strDate.IndexOf("-") > 0 Then strDate = " BETWEEN '" & strDate.Replace("-", "' AND '") & "'"
        If strDate.IndexOf("=") > 0 Then strDate = strDate.Replace("=", "='") & "'"

        ' nutrient rules
        Dim strNutrientRules As String = CStr(slParams("NUTRIENTRULES"))
        If strNutrientRules = Nothing Then strNutrientRules = ""

        ' allergens
        Dim strAllergens As String = CStr(slParams("ALLERGENS"))
        If strAllergens = Nothing Then strAllergens = ""

        'sales
        Dim shrtSalesStatus As Short = 0 '0=show all, 1=show linked listes only, 2=show unlinked liste only
        If slParams.Contains("LINKEDSALES") Then shrtSalesStatus = CShort(slParams("LINKEDSALES"))

        Dim shrtNameOption As Short = 2 'contains
        If slParams.Contains("NAMEOPTION") Then shrtNameOption = CShort(slParams("NAMEOPTION"))

        ' ''if name search is not by exact match, transforn text
        ''strWord = strWord.Trim 'DLS
        ''If shrtNameOption <> 0 Then strWord = fctTransformStrSearch(strWord)

        Dim shrtNumberOption As Short = 2 'contains
        If slParams.Contains("NUMBEROPTION") Then shrtNumberOption = CShort(slParams("NUMBEROPTION"))

        'search global only 'DLS JUne252007
        Dim bGlobalOnly As Boolean = False
        If Not slParams("GLOBALONLY") Is Nothing Then bGlobalOnly = CBool(slParams("GLOBALONLY"))

        If strUnKeywords Is Nothing Then strUnKeywords = ""

        'search by code
        Dim blnSearchByCode As Boolean = False
        Dim intCode As Integer = -1
        If slParams.Contains("CODE") Then
            intCode = CInt(slParams.Item("CODE"))
            If intCode > 0 Then blnSearchByCode = True
            If strCodelisteList.Length > 0 Then blnSearchByCode = True
        End If

        'DLS 16.08.2007
        'Dim bExcludeKeywords As Boolean = CBool(slParams.Item("EXCLUDEKEY"))
        Dim nWithNutrientInfo As Integer = CInt(slParams.Item("WITHNUTRIENT"))
        Dim nUsedUnused As Integer = CInt(slParams.Item("USEDUNUSED"))
        Dim nWithComposition As Integer = CInt(slParams.Item("WITHCOMPOSITION"))

        Dim nNutrientEnergy As Integer = CInt(slParams.Item("NUTRIENTENERGY"))

        'nutrient summary
        Dim nNutrientSummary As Integer = CInt(slParams("NUTRIENTSUMMARY")) 'VRP 15.02.2008
        If nNutrientSummary = Nothing Then nNutrientSummary = 0

        'MRC - 09.03.08 - keyword option
        Dim intKeywordOption As Integer = CInt(slParams("KEYWORDOPTION")) 'MRC - 09.03.08
        If intKeywordOption = Nothing Then intKeywordOption = 0
        Dim intKeywordUnwantedOption As Integer = CInt(slParams("KEYWORDUNWANTEDOPTION")) 'MRC - 09.03.08
        If intKeywordUnwantedOption = Nothing Then intKeywordUnwantedOption = 0

        Dim intPictureOption As Integer = CInt(slParams("PICTUREOPTION")) 'VRP 15.09.2008
        Dim nUsedOnline As Integer = CInt(slParams("USEDONLINE")) 'VRP 30.09.2008
        Dim nTranslated As Integer = CInt(slParams("TRANSLATED")) 'VRP 02.02.2009

        Dim nSharedSite As Integer = CInt(slParams("SHAREDSITE")) 'VRP 21.03.2009

        'DLS
        Dim strKeywordsCode As String = ""
        Dim strUnKeywordsCode As String = ""

        'Remove trailing space - MRC - 09.03.08
        If strKeywords.IndexOf(",") > -1 Then
            Dim str() As String = strKeywords.Split(CChar(","))
            strKeywords = ""
            Dim i As Integer = 0
            While i < str.Length
                strKeywords += str(i).Trim
                i += 1
                If i < str.Length Then
                    strKeywords += ","
                End If
            End While
        End If

        'MRC - 09.04.08 - Added type of liste as a param for keywords search.
        If strKeywords <> "" Then
            strKeywordsCode = GetKeywordsListCode(strKeywords, intCodeTrans, intListeType)
            If strKeywordsCode = "" Then strKeywordsCode = "0" 'VRP 27.02.2009
        End If

        If strUnKeywords <> "" Then
            strUnKeywordsCode = GetKeywordsListCode(strUnKeywords, intCodeTrans, intListeType)
            If strUnKeywordsCode = "" Then strUnKeywordsCode = "0" 'VRP 27.02.2009
        End If
        '----

        With sbSQL
            .Append("SET NOCOUNT ON ")
            .Append("DECLARE @RecCount int ")
            .Append("SELECT @RecCount = @RecsPerPage * @Page + 1 ")
            .Append("IF @Page=0 SET @Page=1 ")

            .Append("DECLARE @FirstRec int, @LastRec int, @MoreRecords int ")
            .Append("DECLARE @CODETABLECATEGORY int ")
            .Append("DECLARE @CODETABLEUNIT int ")
            .Append("DECLARE @CODETABLELISTE int ")
            '---JRN 05.01.2010
            '.Append("DECLARE @tblNutrients TABLE (")
            '.Append("Position int,")
            '.Append("Name nvarchar(50) ) ")
            '.Append("INSERT INTO @tblNutrients (Position, Name) ")
            '.Append("SELECT Position, Name FROM egswNutrientDef ")

            .Append("DECLARE @Nut nvarchar(4000) ")
            .Append("SELECT @Nut = (SELECT [Name] + '_' FROM egswNutrientDef WHERE Position in (1,2,3,4,5) FOR XML PATH('') ) ")
            '----

            '-------- FOR FULL TEXT -------------
            .Append("DECLARE @LANGBREAKER nvarchar(200) ")
            .Append("SELECT @LANGBREAKER = LangBreaker FROM EgsWTranslation WHERE Code= " & intCodeTrans & " ")
            .Append("SET @LANGBREAKER = ISNULL(@LANGBREAKER,'NEUTRAL') ")

            .Append("SET @CODETABLECATEGORY = 19 ") '--dbo.fn_egswGetTableID('egswCategory') 
            .Append("SET @CODETABLEUNIT = 135 ") '--dbo.fn_egswGetTableID('egswUnit') 
            .Append("SET @CODETABLELISTE = 50 ") '--dbo.fn_egswGetTableID('egswListe') 

            '.Append("SELECT @FirstRec = (@Page - 1) * @RecsPerPage ")
            '.Append("SELECT @LastRec = @Page * @RecsPerPage + 1 ")
            .Append("SELECT @FirstRec = (@Page - 1) * @RecsPerPage + 1 ")
            .Append("SELECT @LastRec = @Page * @RecsPerPage ")
            '.Append("CREATE TABLE #TempResults ")
            '.Append("( ")
            '.Append("ID int IDENTITY, ")
            '.Append("code int, ")
            '.Append("name nvarchar(260), ")
            '.Append("number nvarchar(50), ")
            '.Append("dates datetime, ")
            '.Append("price float ")
            '.Append(") ")

            '.Append("INSERT INTO #TempResults (code, name, number, dates, price) ")
            .Append(" ;WITH ListePage AS ")
            .Append("( ") 'start of recpage
            '.Append("SELECT DISTINCT  CASE WHEN ISNULL(l.name,'') = '' THEN r.name ELSE l.name end name,r.code, r.number, r.dates, ")
            '.Append("SELECT DISTINCT  CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name + cast(r.code as varchar(20)) ELSE l.name + cast(r.code as varchar(20)) end name,r.code, r.number, r.dates, ")

            'Added Codesite to sort, for autogrill.
            .Append("SELECT DISTINCT  CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name + cast(r.code as varchar(20)) ELSE l.name + cast(r.code as varchar(20)) end name,r.code, r.number, r.dates, site2.Name AS Site,")

            If intListeType = enumDataListType.Merchandise Then
                .Append(" p.price, ")
            Else
                .Append(" 0 as price, ")
            End If

            ''If strSort = "" Then
            ''    .Append(" ORDER BY [name] ")
            ''Else
            ''    .Append(" ORDER BY " & strSort & " ")
            ''End If
            Dim strSort2 As String
            If strSort Is Nothing Then strSort = ""
            If strSort.ToLower = "name ASC".ToLower Then
                strSort = "r.name ASC"
            ElseIf strSort.ToLower = "name DESC".ToLower Then
                strSort = "r.name DESC"
            End If

            Select Case strSort
                Case "r.name ASC" : strSort2 = "(CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name + '_' + cast(r.code as varchar(20)) ELSE l.name + '_' + cast(r.code as varchar(20)) end) ASC "
                Case "r.name DESC" : strSort2 = "(CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name  + '_' + cast(r.code as varchar(20)) ELSE l.name  + '_' + cast(r.code as varchar(20)) end)DESC "
                Case Else
                    strSort2 = strSort
            End Select

            If strSort = "rank DESC" Then strSort = ""

            If strSort = "" Then
                .Append(" DENSE_RANK() OVER(Order BY (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name + '_' + cast(r.code as varchar(20)) ELSE l.name + '_' + cast(r.code as varchar(20)) end) ASC  ) AS ID ")
                '.Append(" ROW_NUMBER() OVER(Order BY r.name ASC) AS ID ")
            ElseIf strSort = "r.CodeSite ASC" Then
                .Append(" DENSE_RANK() OVER(Order BY (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN site2.Name + '_' + r.name + '_' + cast(r.code as varchar(20)) ELSE site2.Name + '_' + l.name + '_' + cast(r.code as varchar(20)) end) ASC  ) AS ID ")
            ElseIf strSort = "r.CodeSite DESC" Then
                .Append(" DENSE_RANK() OVER(Order BY (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN site2.Name + '_' + r.name + '_' + cast(r.code as varchar(20)) ELSE site2.Name + '_' + l.name + '_' + cast(r.code as varchar(20)) end) DESC  ) AS ID ")
            Else
                .Append(" DENSE_RANK() OVER(Order BY " & strSort2 & ") AS ID ")
            End If


            .Append("FROM egswListe r ")

            'MRC - 05.13.2010 - Bypass Sharing used for Menu Plan Viewers only

            If Not blnByPassSharing Then
                .Append("INNER JOIN egswSharing ON egswSharing.Code=r.Code AND egswSharing.CodeEgswTable=@CODETABLELISTE  ")

                If strCodeSiteList = udtUser.Site.Code.ToString Then
                    .Append(" AND " & GetStrForSharing("egswSharing", udtUser.Site.Group, strCodeSiteList, udtUser.Code)) 'DLS joining sharing condition
                Else
                    .Append(" AND " & GetStrForSharing("egswSharing", -1, strCodeSiteList, -1))
                End If
            End If

            .Append("LEFT OUTER JOIN EgswSite site2 ON r.CodeSite=site2.Code ")

            ' Join rnListeTranslation table
            '.Append("LEFT OUTER JOIN egswListeTranslation l on r.code=l.codeListe and l.codetrans IN (" & intCodeTrans & ",NULL) " & " ")
            .Append("LEFT OUTER JOIN egswListeTranslation l on r.code=l.codeListe and l.codetrans =" & intCodeTrans & " " & " ")

            '' Join Category table
            '.Append("INNER JOIN  egswCategory c on c.code=r.category  ")
            '.Append("LEFT OUTER JOIN egswItemTranslation cT on c.code=cT.code AND cT.codeTrans IN (" & intCodeTrans & ",NULL) AND cT.CodeEgswTable=@CODETABLECATEGORY AND RTRIM(cT.Name)<>''  ")

            If strBrand Is Nothing Then strBrand = ""
            ' '' Join Brand
            'If strBrand.Length > 0 Then
            '    .Append("LEFT JOIN egswBrand b ON b.code=r.Brand ")
            '    .Append("LEFT OUTER JOIN egswItemTranslation bT on b.code=bT.code AND bT.codeTrans IN (" & intCodeTrans & ",NULL) AND bT.CodeEgswTable=dbo.fn_egswGetTableID('egswBrand') ")
            'End If

            If strSupplier Is Nothing Then strSupplier = ""
            '' Join Supplier
            'If strSupplier.Length > 0 Then
            '.Append("INNER JOIN egswSupplier supplier ON supplier.code=r.Supplier ")
            'End If

            If strSource Is Nothing Then strSource = ""
            '' Join Source
            'If strSource.Length > 0 Then
            '.Append("LEFT OUTER JOIN egswSource source ON source.code=r.Source ")
            'End If

            '' Join Keywords table
            'If strKeywords.Length > 0 Or strUnKeywords.Length > 0 Then
            '    .Append("INNER JOIN egswKeyDetails kd on r.code=kd.codeListe  ")
            '    .Append("INNER JOIN egswKeyword k on kd.codeKey=k.code ")
            '    .Append("LEFT OUTER JOIN egswItemTranslation kT on k.code=kT.code AND kT.codeTrans IN (" & intCodeTrans & ",NULL) AND kT.CodeEgswTable=dbo.fn_egswGetTableID('egswKeyword') AND RTRIM(kT.Name)<>'' ")
            'End If

            ' Join Ingredients, rnListe table for ingredients and traslation rnListe
            If strIngredientsWanted Is Nothing Then strIngredientsWanted = ""
            If strIngredientsUnwanted Is Nothing Then strIngredientsUnwanted = ""

            If strIngredientsWanted.Length > 0 Or strIngredientsUnwanted.Length > 0 Then
                .Append("LEFT OUTER JOIN egswDetails d on r.code=d.firstCode  ")
                .Append("LEFT OUTER JOIN egswListe r1 on  r1.code=d.secondcode ")
                .Append("LEFT OUTER JOIN egswListeTranslation l2 on r1.code=l2.codeListe AND l2.codetrans IN (" & intCodeTrans & ",NULL) ")
            End If

            Select Case intListeType
                Case enumDataListType.Ingredient, enumDataListType.Merchandise
                    ' Join Sub Recipes with prices when searching ingredient and merchandise
                    If intCodeSetPrice <> -1 Then
                        .Append("LEFT OUTER JOIN egswListeSetPrice p on r.code=p.codeliste AND p.codesetprice=" & intCodeSetPrice & " AND p.Position=1 ")
                    Else
                        .Append("LEFT OUTER JOIN egswListeSetPrice p on r.code=p.codeliste AND p.position=1 ")
                    End If
                Case enumDataListType.Recipe, enumDataListType.Menu
                    ' join calculations when seraching recipes / menu
            End Select

            If intCodeSetPrice <> -1 Then
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste and pCalc.codesetprice=" & intCodeSetPrice & " ")
            Else
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste ")
            End If

            'join nutrient rules
            If strNutrientRules Is Nothing Then strNutrientRules = ""
            If strNutrientRules.Trim.Length > 0 Then
                .Append("INNER JOIN egswNutrientVal ON r.code=egswNutrientVal.CodeListe ")
            End If



            'join allergens
            If strAllergens Is Nothing Then strAllergens = ""
            If strAllergens.Length > 0 Then
                .Append("LEFT OUTER JOIN egswListeAllergen a ON a.CodeListe=r.code ")
            End If

            If bIncorrectNetMargin Or strPrice <> "" Then
                .Append("LEFT OUTER JOIN EgsWTax pCalcTax on pCalc.tax = pCalcTax.Code ")
            End If

            .Append("WHERE ")

            If blnSearchByCode Then
                If intCode > 0 Then
                    .Append(" r.code=" & intCode & " ")
                Else
                    'AGL 2013.03.08 - check for parenthesis
                    strCodelisteList = strCodelisteList.Replace("(", "")
                    strCodelisteList = strCodelisteList.Replace(")", "")
                    .Append(" r.code IN (" & strCodelisteList & ") ")

                End If
            Else
                ' type
                Select Case intListeType
                    Case enumDataListType.MenuItems
                        Select Case intListeTypeFilter
                            Case enumDataListType.NoType, enumDataListType.Ingredient
                                .Append("(r.Type IN (2,4) OR r.type=8) ")
                            Case Else
                                .Append("r.Type=" & intListeTypeFilter & " ")
                        End Select

                    Case Else
                        Select Case intListeTypeFilter
                            Case enumDataListType.Ingredient
                                'Commented for the meantime, only for migro
                                'If blnAllowCreateUseSubRecipe Then
                                '    .Append("(r.Type IN (2,4) OR (r.type=8 and r.srQty>0)) ")
                                'Else
                                '    .Append("(r.Type IN (2,4)) ")
                                'End If

                                .Append("(r.Type IN (2,4) OR (r.type=8 and r.srQty>0)) ")

                            Case enumDataListType.Merchandise
                                .Append("(r.Type IN (2,4)) ")
                            Case enumDataListType.Recipe
                                .Append("(r.type=8 and r.srQty>0) ")
                            Case enumDataListType.Text
                                .Append("(r.Type = 4) ")
                            Case enumDataListType.NoType
                                .Append("r.Type=" & intListeType & " ")
                                .Append(" and (r.[use]=1 OR (r.codeUser =" & intCodeUser & " AND r.type IN (2,8,16))) ") 'Exclude drafts
                        End Select

                End Select
                'End If

                '.Append(" AND r.protected=0 ") 'MRC 05.15.09 we now use this fields

                'DLS Commented
                ''If intListeType = enumDataListType.Merchandise Or intListeType = enumDataListType.Recipe Or intListeType = enumDataListType.Menu Or intListeType = enumDataListType.Ingredient Then
                ''    'check if user is searching his own site, if searching his own site, get all dat is shared to d user, user'site and user's property
                ''    If CStr("," & strCodeSiteList & ",").IndexOf("," & CStr(udtUser.Site.Code) & ",") > -1 Then
                ''        'get sharing of user
                ''        .Append("AND ((r.[use]=1 AND egswSharing.CodeUserSharedTo=" & CStr(udtUser.Site.Group) & " AND egswSharing.Type IN(" & ShareType.CodeProperty & ", " & ShareType.CodePropertyView & ")) ")
                ''        '.Append("OR (r.[use]=1 AND egswSharing.CodeUserSharedTo=" & CStr(udtUser.Site.Code) & " AND egswSharing.Type IN(" & ShareType.CodeSite & ", " & ShareType.CodeSiteView & ")) ")
                ''        .Append("OR (r.[use]=1 AND egswSharing.CodeUserSharedTo IN (" & strCodeSiteList & ") AND egswSharing.Type IN(" & ShareType.CodeSite & ", " & ShareType.CodeSiteView & ")) ")
                ''        .Append("OR (r.[use]=1 AND egswSharing.CodeUserSharedTo=" & CStr(udtUser.Code) & " AND egswSharing.Type IN(" & ShareType.CodeUser & ", " & ShareType.CodeUserView & ")) ")
                ''        .Append("OR (r.[use]=1 AND egswSharing.Type IN(" & ShareType.ExposedViewing & ")) ")

                ''        'd one who created
                ''        If intListeType <> enumDataListType.MenuItems AndAlso intListeType <> enumDataListType.Ingredient Then
                ''            .Append("OR (")
                ''            .Append("(egswSharing.CodeUserSharedTo=" & CStr(intCodeUser) & " ")
                ''            .Append("AND egswSharing.Type=" & ShareType.CodeUserOwner & " ")
                ''            .Append("AND r.[use]=0 AND r.type IN (2,8,16))) ")
                ''        End If
                ''    Else
                ''        .Append(" AND r.[use]=1 AND ((egswSharing.CodeUserSharedTo IN (" & strCodeSiteList & ") AND egswSharing.Type IN(" & ShareType.CodeSite & ")) ")
                ''    End If
                ''    .Append(") ")
                ''Else
                ''    .Append("AND (egswSharing.CodeUserSharedTo IN (" & strCodeSiteList & ") AND egswSharing.Type IN(" & ShareType.CodeSite & ")) ")
                ''End If

                ' Flags
                '.Append(" AND r.protected=0 ") 'MRC 09.01.08

                '' ''If strWord.Length > 0 Then
                '' ''    If shrtNameOption = 0 Then 'exact
                '' ''        '' find match in rnliste table
                '' ''        '.Append("AND (r.Name = @nvcWord ")
                '' ''        '' find match in rnlistetranslation table
                '' ''        '.Append("OR (l.name = @nvcWord ")
                '' ''        '.Append("AND l.codetrans=" & intCodeTrans & ")) ")
                '' ''        'DLS
                '' ''        .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) = @nvcWord ")

                '' ''    ElseIf shrtNameOption = 1 Then
                '' ''        strWord = strWord & "%" ' always use like
                '' ''        ' find match in rnliste table
                '' ''        '.Append("AND (r.Name like @nvcWord ")
                '' ''        '''' find match in rnlistetranslation table
                '' ''        '.Append("OR (l.name like @nvcWord ")
                '' ''        '.Append("AND l.codetrans=" & intCodeTrans & ")) ")
                '' ''        '.Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")
                '' ''        'strWord = fctTransformStrSearch(strWord)

                '' ''        strWord = strWord & "%" ' always use like
                '' ''        .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")
                '' ''    Else 'contains
                '' ''        strWord = "%" & strWord & "%" ' always use like
                '' ''        '' find match in rnliste table
                '' ''        '.Append("AND (r.Name like @nvcWord ")
                '' ''        '''' find match in rnlistetranslation table
                '' ''        '.Append("OR (l.name like @nvcWord ")
                '' ''        '.Append("AND l.codetrans=" & intCodeTrans & ")) ")
                '' ''        'strWord = fctTransformStrSearch(strWord)
                '' ''        strWord = "%" & strWord & "%" ' always use like
                '' ''        .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")
                '' ''    End If

                '' ''    cmd.Parameters.Add("@nvcWord", SqlDbType.NVarChar, 500).Value = strWord.Trim
                '' ''End If

                'DLS
                If strWord.Length > 0 Then
                    If shrtNameOption = 0 Then 'exact
                        '' find match in rnliste table
                        .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) = @nvcWord ")
                    ElseIf shrtNameOption = 1 Then

                        strWord = fctTransformStrSearch(strWord)

                        strWord = strWord & "%" ' always use like
                        .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")


                    Else 'contains
                        Dim flagFullTextEnable As Boolean = IsFullTextEnabled()

                        If fctSearchTextOKForFullText(strWord) = False Then
                            flagFullTextEnable = False
                        End If

                        If flagFullTextEnable = False Then
                            strWord = fctTransformStrSearch(strWord)
                            'mrc - 06.25.09 : Use of Asterisk
                            If strWord.IndexOf("*") > -1 Then
                                strWord = strWord.Replace("*", "%") ' replace asterisk with percent
                                .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")
                            Else
                                strWord = "%" & strWord & "%" ' always use like
                                .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")
                            End If
                        Else
                            strWord = fctTransformStrSearchFullText(strWord)
                            .Append("AND (CONTAINS(r.Name,@nvcWord, LANGUAGE @LANGBREAKER) OR CONTAINS(l.Name,@nvcWord, LANGUAGE @LANGBREAKER))  ")
                        End If


                    End If

                    cmd.Parameters.Add("@nvcWord", SqlDbType.NVarChar, 2000).Value = strWord
                End If

                If strNumber.Length > 0 Then
                    If shrtNumberOption = 0 Then 'exact
                        ' find match in rnliste table
                        .Append("AND r.Number = @nvcNumber ")
                    ElseIf shrtNumberOption = 1 Then
                        strNumber = strNumber & "%" ' always use like
                        ' find match in rnliste table
                        .Append("AND r.Number like @nvcNumber ")
                    Else 'contains
                        strNumber = "%" & strNumber & "%" ' always use like
                        ' find match in rnliste table
                        .Append("AND r.Number like @nvcNumber ")
                    End If
                    cmd.Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 25).Value = strNumber
                End If

                ' Date
                If strDate.Length > 0 Then .Append(" AND r.dates " & strDate & " ")

                ' Price
                Select Case intListeType
                    Case enumDataListType.Merchandise

                        '---JRN 09.03.2010
                        If strPrice.Length <> 0 Then
                            If blnSpecialPrice Then

                            Else
                                .Append("AND p." & strPrice & " ")
                            End If

                        End If
                        '---

                    Case enumDataListType.Recipe, enumDataListType.Menu
                        If strPrice.Length <> 0 Then
                            If blnSpecialPrice Then
                                .Append("AND " & strPrice & " ")

                            Else
                                .Append("AND pCalc." & strPrice & " ")
                            End If

                        End If
                End Select

                ' Wanted Ingredient Search
                If strIngredientsWanted.Length > 0 Then
                    Dim strSQLEintCodeIng1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR r1.name LIKE ", "@nvcIngWanted", strIngredientsWanted, CChar(","), True)
                    Dim strSQLEintCodeIng2 As String = AddParam(cmd, SqlDbType.NVarChar, " OR l2.name LIKE ", "@nvcIng2Wanted", strIngredientsWanted, CChar(","), True)
                    Dim strSQLEintCodeIng3 As String = AddParam(cmd, SqlDbType.NVarChar, " OR d.name LIKE ", "@nvcIng3Wanted", strIngredientsWanted, CChar(","), True)

                    ' Find match in ingredients
                    .Append("AND (r1.Name like " & strSQLEintCodeIng1 & " ")

                    ' find match ingredient in rnliste translation table
                    .Append("OR (l2.Name like " & strSQLEintCodeIng2 & " ")
                    '.Append("AND l2.codeTrans=" & intCodeTrans & ")) ")
                    .Append("AND l2.codeTrans=" & intCodeTrans & ") ")

                    'MRC - 12.02.08 - Added search of ingredients in textmode
                    .Append("OR d.Name LIKE " & strSQLEintCodeIng3 & ") ")
                End If

                ' Unwanted Ingredient Search
                If strIngredientsUnwanted.Length <> 0 Then
                    Dim strSQLEintCodeIngUw1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR name LIKE ", "@nvcIngUnWanted", strIngredientsUnwanted, CChar(","), True)
                    'compare it using egswliste.anme w/codetarns
                    .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select code FROM egswListe WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND CodeTrans=" & intCodeTrans & " ) ) ")
                    'compare it using egswlistetransaltion.name w/codetrans
                    .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select codeliste FROM egswlistetranslation WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND codetrans=" & intCodeTrans & " ) ) ")
                    'compare it using egswliste.name w/o codetrans for liste's w/o translstions
                    .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select code FROM egswListe WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND Code NOT IN (SELECT codeListe FROM egswListeTranslation WHERE CodeTrans=" & intCodeTrans & " )) ) ")
                    'compare to text mode
                    .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE name LIKE " & strSQLEintCodeIngUw1 & " ")
                End If

                'brand
                If strBrand.Length > 0 Then
                    '.Append("AND (b.name=@nvcBrand OR bT.name=@nvcBrand) ")
                    'cmd.Parameters.Add("@nvcBrand", SqlDbType.NVarChar, 150).Value = strBrand
                    .Append("AND r.brand =" & strBrand & " ") 'VRP 12.03.2008
                End If

                'category
                If strCategory.Length > 0 And strCategory <> "-1" Then
                    '.Append("AND (c.name=@nvcCategory OR cT.name=@nvcCategory) ")
                    'cmd.Parameters.Add("@nvcCategory", SqlDbType.NVarChar, 150).Value = strCategory
                    .Append(" AND r.category=" & strCategory & " ") 'VRP 12.03.2008
                End If

                'source
                If strSource.Length > 0 Then
                    '.Append("AND source.name=@nvcSource ")
                    'cmd.Parameters.Add("@nvcSource", SqlDbType.NVarChar, 150).Value = strSource
                    .Append(" AND r.Source=" & strSource & " ") 'VRP 12.03.2008
                End If

                ' SUPPLIER
                If strSupplier.Length > 0 Then
                    '.Append("AND supplier.nameref=@nvcSupplier ")
                    'cmd.Parameters.Add("@nvcSupplier", SqlDbType.NVarChar, 150).Value = strSupplier
                    .Append("AND r.Supplier=" & strSupplier & " ") 'VRP 12.03.2008
                End If

                'GlobalOnly
                If bGlobalOnly Then 'DLS June252007
                    .Append(" AND r.IsGlobal=1 ")
                End If

                'DLS 17.08.2007
                'Used/UnUsed as Ingredients
                If nUsedUnused = 1 Then 'used
                    '.Append(" AND  0 < (SELECT count(SecondCode) FROM EgsWDetails WHERE SecondCode = R.Code) ")
                    .Append(" AND  0 < (SELECT count(SecondCode) FROM EgsWDetails WHERE SecondCode = R.Code) ")
                    .Append(" AND  0 < (SELECT COUNT(CodeListe) FROM EgswMPDetailsData WHERE CodeListe=R.Code) ")
                ElseIf nUsedUnused = 2 Then 'unused
                    '.Append(" AND  0 = (SELECT count(SecondCode) FROM EgsWDetails WHERE SecondCode = R.Code) ")
                    .Append(" AND  0 = (SELECT count(SecondCode) FROM EgsWDetails WHERE SecondCode = R.Code) ")
                    .Append(" AND  0 = (SELECT COUNT(CodeListe) FROM EgswMPDetailsData WHERE CodeListe=R.Code) ")
                End If

                'DLS 17.08.2007
                'With Ingredients on Merchandise/ With Composistion on Labels
                If nWithComposition = 1 And intListeType = enumDataListType.Merchandise Then
                    .Append(" AND ISNULL(r.Ingredients,'') <> ''  ")
                ElseIf nWithComposition = 2 And intListeType = enumDataListType.Merchandise Then
                    .Append(" AND ISNULL(r.Ingredients,'') = ''  ")
                ElseIf nWithComposition = 1 And intListeType = enumDataListType.Recipe Then
                    .Append(" AND ISNULL((select Composition FROM EgsWLabel INNER JOIN EgsWProduct ON EgsWProduct.Code = EgsWLabel.CodeProduct AND  EgsWProduct.Code = R.Code ),'') <> ''  ")
                ElseIf nWithComposition = 2 And intListeType = enumDataListType.Recipe Then
                    .Append(" AND ISNULL((select Composition FROM EgsWLabel INNER JOIN EgsWProduct ON EgsWProduct.Code = EgsWLabel.CodeProduct AND  EgsWProduct.Code = R.Code ),'') = ''  ")
                End If

                'DLS 17.08.2007
                'With Nutrient Info or Without
                If nWithNutrientInfo = 1 Then
                    .Append("   AND dbo.fn_EgsWNutrientValIsBlank(r.code)=0    ")
                ElseIf nWithNutrientInfo = 2 Then
                    .Append("   AND dbo.fn_EgsWNutrientValIsBlank(r.code)=1    ")
                End If

                If nNutrientEnergy = 1 Then 'DLS Dec 10 2007
                    .Append("   AND (SELECT top 1 case WHEN ISNULL(N1,-1) =-1 THEN 0 ELSE N1 END FROM EgsWNutrientVal where CodeListe = r.code) > 0    ")
                ElseIf nNutrientEnergy = 2 Then
                    .Append("   AND (SELECT top 1 case WHEN ISNULL(N1,-1) =-1 THEN 0 ELSE N1 END FROM EgsWNutrientVal where CodeListe = r.code) = 0    ")
                End If

                ' Keywords
                If strKeywordsCode.Length > 0 Then
                    Dim strANDKeywords() As String = strKeywordsCode.Split(CChar(","))
                    Dim intX As Integer = UBound(strANDKeywords) + 1
                    If intKeywordOption = 1 And intX > 1 Then    'AND
                        .Append(" AND " & intX & "= (SELECT count(distinct(CodeKey)) FROM EgsWKeyDetails KD WHERE KD.CodeListe = r.Code AND KD.CodeKey in (" & strKeywordsCode & "))")
                    Else    'OR
                        .Append(" AND r.Code in (SELECT CodeListe FROM EgsWKeyDetails WHERE CodeKey in (" & strKeywordsCode & "))")
                    End If
                End If

                If strUnKeywordsCode.Length > 0 Then
                    Dim strANDKeywordsUnwanted() As String = strUnKeywordsCode.Split(CChar(","))
                    Dim intX As Integer = UBound(strANDKeywordsUnwanted) + 1
                    If intKeywordUnwantedOption = 1 And intX > 1 Then
                        .Append(" AND " & intX & "= (SELECT count(distinct(CodeKey)) FROM EgsWKeyDetails KD WHERE KD.CodeListe = r.Code AND KD.CodeKey not in (" & strUnKeywordsCode & "))")
                    Else
                        .Append(" AND r.Code not in (SELECT CodeListe FROM EgsWKeyDetails WHERE CodeKey in (" & strUnKeywordsCode & "))")
                    End If
                End If

                '' Keywords
                'If strUnKeywords.Length > 0 Then
                '    '    Dim strSQLEintCode1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR k.name <> ", "@nvcUNKeyworda", strUnKeywords, CChar(","), True)
                '    '    Dim strSQLEintCode2 As String = AddParam(cmd, SqlDbType.NVarChar, " OR kt.name <> ", "@nvcUNKeywordb", strUnKeywords, CChar(","), True)
                '    '    .Append("AND ((k.name <> " & strSQLEintCode1 & " ")

                '    '    ' find match keyword in keyword parent table translation
                '    '    .Append("OR (kt.name <> " & strSQLEintCode2 & " ")

                '    '    'keyword type
                '    '    If strKeywordType = "Derived" Then
                '    '        .Append("AND kd.Derived = 1 ")
                '    '    ElseIf strKeywords = "Assigned" Then
                '    '        .Append("AND kd.Derived = 0 ")
                '    '    End If

                '    '    .Append("AND kt.codetrans=" & intCodeTrans & "))) ")

                '    .Append(" AND r.Code not in (SELECT CodeListe FROM EgsWKeyDetails WHERE CodeKey in (" & strUnKeywordsCode & "))")
                'End If

                Select Case intPictureOption 'VRP 15.09.2008 picture options
                    Case 1 : .Append(" AND REPLACE(LTRIM(RTRIM(PictureName)), ';','')<>'' ")
                    Case 2 : .Append(" AND REPLACE(LTRIM(RTRIM(PictureName)), ';','')='' ")
                End Select

                If nUsedOnline = 1 Then 'VRP 30.09.2008
                    .Append(" AND r.Online=1 ")
                ElseIf nUsedOnline = 2 Then
                    .Append(" AND r.Online=0 ")
                End If

                If nTranslated = 1 Then 'VRP 02.02.2009
                    .Append(" AND dbo.fn_EgswGetListeTransPerc (r.Code, r.CodeSite, r.Type) >= 100")
                ElseIf nTranslated = 2 Then
                    .Append(" AND dbo.fn_EgswGetListeTransPerc (r.Code, r.CodeSite, r.Type) < 100")
                End If

                'nutrient rules
                If strNutrientRules Is Nothing Then strNutrientRules = ""
                If strNutrientRules.Trim.Length > 0 Then
                    Dim cNutrientRules As clsNutrientRules = New clsNutrientRules(udtUser, enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataSet)
                    Dim arr() As String = strNutrientRules.Split(CChar(","))
                    Array.Sort(arr)

                    Dim i As Integer = 1
                    Dim intLastPosition As Integer = 0
                    Dim arr2() As String
                    While i < arr.Length
                        arr2 = arr(i).Split(CChar("-"))
                        If CInt(arr2(0)) > 0 Then
                            Dim rwTemp As DataRow = CType(cNutrientRules.GetList(CInt(arr2(1))), DataSet).Tables(1).Rows(0)
                            If intLastPosition = CInt(arr2(0)) Then
                                .Append(" OR egswNutrientVal.N" & arr2(0) & " BETWEEN " & CStr(rwTemp("minimum")) & " AND " & CStr(rwTemp("maximum")) & " ")
                            Else
                                If i = 1 Then
                                    .Append(" AND ( ")
                                Else
                                    .Append(" ) AND ( ")
                                End If

                                .Append(" egswNutrientVal.N" & arr2(0) & " BETWEEN " & CStr(rwTemp("minimum")) & " AND " & CStr(rwTemp("maximum")) & " ")
                            End If

                            If i + 1 = arr.Length Then
                                '.Append(" ) ")
                            End If

                            intLastPosition = CInt(arr2(0))
                        End If
                        i += 1
                    End While
                End If

                If strAllergens Is Nothing Then strAllergens = ""
                If strAllergens.Length > 0 Then
                    If strAllergens.IndexOf("NOT") > -1 Then
                        .Append(" AND (a.codeAllergen " & strAllergens & " OR a.codeAllergen IS NULL) ")
                    Else
                        .Append(" AND a.codeAllergen " & strAllergens & " ")
                    End If
                End If

                'filter, this only works wen u r searching ur own site
                'If CStr(strCodeSiteList) = CStr(udtUser.Site.Code) Then
                Select Case UCase(strFilter)
                    Case "1" '"'OWNED"
                        .Append(" AND r.CodeSite = " & strCodeSiteList & " ")
                    Case "2" '"PUBLIC"
                        .Append(" AND r.Code IN (SELECT S.Code FROM egsWSharing S WHERE S.CodeEGSWTable=50 AND S.Code = r.Code AND (S.IsGlobal=1 OR S.Type not in (1,8))  ) ")
                    Case "3" '"PRIVATE"
                        .Append(" AND r.Code NOT IN (SELECT S.Code FROM egsWSharing S WHERE S.CodeEGSWTable=50 AND S.Code = r.Code  AND (S.IsGlobal=1 OR S.Type not in (1,8)) ) ")
                    Case "4" '"SHARED"
                        '.Append(" AND r.CodeSite <> " & strCodeSiteList & " ")
                        If nSharedSite <> 0 Then 'VRP 21.03.2009
                            .Append(" AND r.CodeSite= " & nSharedSite & " ")
                        Else
                            .Append(" AND r.CodeSite <> " & strCodeSiteList & " ")
                        End If
                    Case "5" '"DRAFT"
                        .Append(" AND r.[use]=0 and r.submitted=0 ")
                        'Case "6" '"SYSTEM"
                        '    .Append(" AND dbo.fn_EgswIsListeOwnedBySystem(r.code)>0 ")
                    Case "6" '"For Approval" 'DLSXXXXXX
                        .Append(" AND r.submitted=1 ")
                    Case "7" '"Approved" 'DLSXXXXXX
                        .Append(" AND r.approvalstatus=1 AND r.submitted=0 ")
                    Case "8" '"Not Approved" 'DLSXXXXXX
                        .Append(" AND r.approvalstatus=2 AND r.submitted=0 ")
                End Select
                'end if
            End If

            If shrtSalesStatus = 1 Then 'linked only
                If intListeType = enumDataListType.Merchandise Then
                    .Append(" AND r.Code IN (SELECT CodeListe FROM egswLinkFbRnPOS linkMP WHERE linkMP.TypeLink IN (0) AND linkMP.CodeListe=r.code ") 'for merchandise and product
                    .Append("       AND linkMP.CodeProduct IN (SELECT CodeProduct FROM egswLinkFbRnPOS linkPS WHERE linkPS.TypeLink IN (1) AND linkPS.CodeProduct=linkMP.codeProduct)) ") 'for product and salesitem
                ElseIf intListeType = enumDataListType.Recipe OrElse intListeType = enumDataListType.Menu Then
                    .Append(" AND r.Code IN (SELECT CodeListe FROM egswLinkFbRnPOS linkLS WHERE linkLS.TypeLink IN (2) AND linkLS.CodeListe=r.code )") ' for recipes/menus and salesitem
                End If
            ElseIf shrtSalesStatus = 2 Then 'not linked only
                If intListeType = enumDataListType.Merchandise Then
                    .Append(" AND r.Code NOT IN (SELECT CodeListe FROM egswLinkFbRnPOS linkMP WHERE linkMP.TypeLink IN (0) AND linkMP.CodeListe=r.code ") 'for merchandise and product
                    .Append("       AND linkMP.CodeProduct IN (SELECT CodeProduct FROM egswLinkFbRnPOS linkPS WHERE linkPS.TypeLink IN (1) AND linkPS.CodeProduct=linkMP.codeProduct)) ") 'for product and salesitem
                ElseIf intListeType = enumDataListType.Recipe OrElse intListeType = enumDataListType.Menu Then
                    .Append(" AND r.Code NOT IN (SELECT CodeListe FROM egswLinkFbRnPOS WHERE TypeLink IN (2) AND CodeListe=r.code )") ' for recipes/menus
                End If
            End If

            '-------JRN 02.18.2010
            'If bIncorrectNetMargin Then
            '    .Append("AND (100 - ((pcalc.calcPrice / ISNULL(NULLIF((pCalc.imposedPrice / (ISNULL(NULLIF(pCalcTax.Value,0),1) + 100) * 100), 0), ISNULL(NULLIF(pcalc.calcPrice, 0), 1)) ) * 100)) < " & dblMinimumNetMargin & " ")
            'End If
            '-------

            ''If strSort = "" Then
            ''    .Append(" ORDER BY [name] ")
            ''Else
            ''    .Append(" ORDER BY " & strSort & " ")
            ''End If

            .Append(") ") 'end of recpage



            '.Append("SELECT @iRow=COUNT(*) FROM ListePage ")
            '.Append("SELECT @MoreRecords=COUNT(*) FROM ListePage WHERE ID>@LastRec ")

            '.Append("DELETE FROM #TempResults WHERE ID <= @FirstRec OR ID >=@LastRec ")

            ''BuildFullySharedString(sbSQL, udtUser, intListeType, cmd)
            .Append("SELECT DISTINCT tr.ID, r.protected, r.code, r.type, CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end name, ")
            .Append("r.category, r.preparation, r.picturename, ")
            .Append("r.submitted, ISNULL(l.codetrans, r.codeTrans) as codeTrans, ")
            .Append("r.yield as yield, r.[percent], y.format as yieldFormat,ISNULL(yT.name,y.namedisplay) as yieldname, ")
            '.Append("ISNULL(pCalc.coeff, 0) AS coeff, ISNULL(pCalc.calcPrice, 0) AS calcPrice,ISNULL(pCalc.imposedPrice, 0) AS imposedPrice, ISNULL(pCalcTax.Value,0) as TaxValue, c.name AS categoryname, y.code as yieldCode, ")

            'MRC - 04.08.2010
            If blnUseImposedPriceForSubRecipe Then
                .Append("ISNULL(pCalc.coeff, 0) AS coeff, ")
                .Append("ISNULL(dbo.fn_EgswGetRecipeTotalCost2(r.code," & intCodeSetPrice & "," & 1 & "), 0) AS calcPrice, ")
                .Append("ISNULL(pCalc.imposedPrice, 0) AS imposedPrice, ")
                .Append("ISNULL(pCalcTax.Value,0) as TaxValue, ")
                .Append("ISNULL(pCalcTax.Code,0) as CodeTax, ")
                .Append("ISNULL(pCalc.ApprovedPrice,0) as ApprovedPrice, ")
                .Append("c.name AS categoryname, ")
                .Append("y.code as yieldCode, ")
            Else
                .Append("ISNULL(pCalc.coeff, 0) AS coeff, ")
                .Append("ISNULL(pCalc.calcPrice, 0) AS calcPrice, ")
                .Append("ISNULL(pCalc.imposedPrice, 0) AS imposedPrice, ")
                .Append("ISNULL(pCalcTax.Value,0) as TaxValue, ")
                .Append("ISNULL(pCalcTax.Code,0) as CodeTax, ")
                .Append("ISNULL(pCalc.ApprovedPrice,0) as ApprovedPrice, ")
                .Append("c.name AS categoryname, ")
                .Append("y.code as yieldCode, ")
            End If

            '---JRN .9.03.2010
            '.Append("(100 - ((pcalc.calcPrice / ISNULL(NULLIF((pCalc.imposedPrice / (ISNULL(NULLIF(pCalcTax.Value,0),1) + 100) * 100), 0), ISNULL(NULLIF(pcalc.calcPrice, 0), 1)) ) * 100)) AS NetMarginPercent, ")
            '.Append("(ISNULL(pCalc.calcPrice,0) * ISNULL(pCalc.CoEff,0)) - ISNULL(pCalc.calcPrice,0) AS GrossMargin, ")
            '.Append("100 - ISNULL(NULLIF((((ISNULL(pCalc.calcPrice,0) * ISNULL(pCalc.CoEff,0)) - (ISNULL(pCalc.calcPrice,0) * ISNULL(pCalc.CoEff,0) - ISNULL(pCalc.calcPrice,0))) / ISNULL(NULLIF((ISNULL(pCalc.calcPrice,0) * ISNULL(pCalc.CoEff,0)),0),1)) * 100,0), 100) AS GrossMarginInPercent, ")
            '.Append("ISNULL(pCalc.calcPrice, 0) * ISNULL(pCalc.CoEff,0) AS SellingPrice, ")
            '.Append("(ISNULL(pCalc.calcPrice, 0) * ISNULL(pCalc.CoEff,0)) + ((ISNULL(pCalc.calcPrice, 0) * ISNULL(pCalc.CoEff,0)) * (ISNULL(pCalcTax.Value, 0)/100)) AS SellingPriceTax, ")
            '.Append("(ISNULL(pCalc.calcPrice, 0) * ISNULL(pCalc.CoEff,0)) - ((ISNULL(pCalc.calcPrice,0) * ISNULL(pCalc.CoEff,0)) - ISNULL(pCalc.calcPrice,0))  AS FoodCost, ")
            '.Append("ISNULL(pCalc.calcPrice, 0) / ISNULL(NULLIF(ISNULL(pCalc.imposedPrice, 0) / (100 + ISNULL(pCalcTax.Value, 0)), 0), 1) * 100 AS ImposedFoodCostInPercent, ")

            .Append("Keywords, ")
            .Append("@Nut as NutrientName, ")
            '---
            .Append("r.Supplier, r.source, r.remark, ")
            .Append("r.note, r.dates, r.submitted, replace(r.number, CHAR(1),'') AS NUMBER, ")
            .Append("r.wastage1,r.wastage2, r.wastage3,r.wastage4, ")
            .Append("r.picturename, ")
            .Append("r.srUnit,ISNULL(sruT.name,sru.namedef) as srUnitName, ")
            .Append("ISNULL(pCalc.coeff,0) AS coeff1, ")
            '.Append("r.currency, pCalc.coeff,")

            .Append("(1-((1-r.Wastage1/100.0) *")
            .Append("(1-r.Wastage2/100.0) * ")
            .Append("(1-r.Wastage3/100.0) * ")
            .Append("(1-r.Wastage4/100.0))) * 100.0 as TotalWastage, ISNULL(pCalc.imposedPrice,0) as ImposedSellingPrice, ")
            '.Append("dbo.fn_EgswIsListeOwnedBySite (r.code," & udtUser.Site.Code & ") as sOwner, ")
            .Append("r.CodeSite as sOwner, ")
            '.Append("0 as sOwner, ")
            .Append("ISNULL(TR.Code, 0) as IsOwner, ")
            '.Append("dbo.fn_EgswCheckListeFullySharedEditToUser(" & udtUser.Code & ", r.code) as IsOwner, ")
            '.Append("dbo.fn_EgswIsListeOwnedBySystem(r.code) as IsSystemOwned, ") 'checks if it is owned by a ssytem site
            .Append("0 as IsSystemOwned, ") 'checks if it is owned by a ssytem site
            '.Append("0 as IsSystemOwned, ") 'checks if it is owned by a ssytem site
            .Append("CASE r.[Use] WHEN 0 THEN 1 ELSE 0 END AS IsDraft, ") 'checks if it is a draft
            .Append("CASE WHEN r.[use]=1 AND r.IsGlobal=1 THEN 1 ELSE 0 END AS IsGlobal, ") 'checks if it is a global, pending for approval is not cionsidered as global yet
            '.Append("@MoreRecords AS MoreRecords, ")

            .Append(" (SELECT COUNT(*) FROM ListePage) as iRow, ")
            .Append("(SELECT COUNT(*) FROM ListePage WHERE ID>@LastRec) AS MoreRecords, ")

            .Append("dbo.fn_EgswGetSetPriceData(r.code," & intCodeSetPrice & "," & intCodeTrans & ") as SetPriceData, ") ' used in searchlistelist.ascx for setprice computation
            'If blnSpecialPrice Then
            '    .Append("dbo.fn_EgswGetSetPrice(r.code," & intCodeSetPrice & "," & intCodeTrans & ") as SetPriceValue, ")
            'Else
            '    .Append("dbo.fn_EgswGetSetPrice(r.code," & intCodeSetPrice & "," & intCodeTrans & ", '" & strPriceCol & "') as SetPriceValue, ") ' used in searchlistelist.ascx for setprice computation
            'End If
            .Append("dbo.fn_EgswGetSetPrice(r.code," & intCodeSetPrice & "," & intCodeTrans & ", '" & strPriceCol & "') as SetPriceValue, ") ' used in searchlistelist.ascx for setprice computation
            .Append("ISNULL(product.Code, 0) AS CodeFG ") ' unit of product/salesitem 
            '.Append("ISNULL(link.CodeUnitProduct, 0) AS CodeUnitProduct, ") ' unit of product/salesitem 
            '.Append("ISNULL(link.CodeProduct, 0) AS CodeProduct ") ' unit of product/salesitem 

            'Select Case intListeType
            '    Case enumDataListType.Ingredient, enumDataListType.Merchandise
            '        .Append("p.price as Price ")
            '    Case Else
            '        .Append("0 as Price ")
            'End Select
            'If bNutrientSummary = True Then 'VRP 15.02.2008
            '.Append(", egswNutrientVal.N1 AS SELECT Name FROM @tblNutrients WHERE Position=1, egswNutrientVal.N2 AS SELECT Name FROM @tblNutrients WHERE Position=2,egswNutrientVal.N3 AS SELECT Name FROM @tblNutrients WHERE Position=3,egswNutrientVal.N4 AS SELECT Name FROM @tblNutrients WHERE Position=4,egswNutrientVal.N5 AS SELECT Name FROM @tblNutrients WHERE Position=5") ',egswNutrientVal.N6 AS N6,egswNutrientVal.N7 AS N7,egswNutrientVal.N8 AS N8,egswNutrientVal.N9 AS N9,egswNutrientVal.N10 AS N10,egswNutrientVal.N11 AS N11,egswNutrientVal.N12 AS N12,egswNutrientVal.N13 AS N13,egswNutrientVal.N14 AS N14,egswNutrientVal.N15 AS N15 ")
            'MKAM 2015.06.01
            '.Append(", ISNULL(egswNutrientVal.N1, 0) AS N1, ISNULL(egswNutrientVal.N2, 0) AS N2,ISNULL(egswNutrientVal.N3, 0) AS N3,ISNULL(egswNutrientVal.N4, 0) AS N4,ISNULL(egswNutrientVal.N5, 0) AS N5") ',egswNutrientVal.N6 AS N6,egswNutrientVal.N7 AS N7,egswNutrientVal.N8 AS N8,egswNutrientVal.N9 AS N9,egswNutrientVal.N10 AS N10,egswNutrientVal.N11 AS N11,egswNutrientVal.N12 AS N12,egswNutrientVal.N13 AS N13,egswNutrientVal.N14 AS N14,egswNutrientVal.N15 AS N15 ")
            'End If

            .Append(", r.CodeSite, ISNULL(r.CodeUser,0) as CodeUser, EgswSite.Name AS SiteName, ISNULL(r.[Use],0) as ListeUse, ISNULL(r.ApprovalStatus,0) as ApprovalStatus ") 'DLS

            .Append(", supplier.NameRef AS SupplierName ") 'AGL 2014.10.28 '.Append(", supplier.Company AS SupplierName ") 'MRC 08.04.08
            .Append(", source.Name AS SourceName ") 'MRC 08.06.08
            .Append(", r.Brand AS Brand ") 'MRC 08.06.08
            .Append(", CASE WHEN b.name IS NULL OR LEN(RTRIM(LTRIM(b.name)))=0 THEN bT.Name ELSE b.name END AS BrandName ")
            .Append(", r.Protected AS Protected ") 'MRC 09.01.08

            .Append(", r.ModifiedDate AS ModifiedDate ") 'MRC 09.01.08

            .Append("FROM egswListe r ")
            .Append("CROSS APPLY ( SELECT TOP 3 K.Name + ',' FROM EgswListe L1 INNER JOIN EgswKeyDetails KD ON KD.Codeliste = r.Code INNER JOIN EgswKeyword K ON K.Code = KD.CodeKey WHERE(L1.Code = r.Code) FOR XML PATH('') )  D ( Keywords )")
            .Append("INNER JOIN ListePage tr ON r.code=tr.Code ")

            'If bNutrientSummary = True Then 'VRP 15.02.2008
            '.Append("LEFT OUTER JOIN egswNutrientVal ON tr.Code=egswNutrientVal.CodeListe ")   'MKAM 2015.06.01
            'End If
            'this was just amede to check if user has edit and owner capabilities
            '.Append("LEFT OUTER JOIN dbo.fn_EgswGetListeFullySharedEditToUserByCodeUser(" & udtUser.Code & ", " & intListeType & ") fullyShared ON r.Code=fullyShared.Code ")

            ''.Append("LEFT OUTER JOIN @tblFullySharedWithEdit fullyShared ON r.Code=fullyShared.Code ")

            ''.Append("INNER JOIN egswSharing ON egswSharing.Code=r.Code AND egswSharing.CodeEgswTable=@CODETABLELISTE ")

            ' Join rnListeTranslation table
            .Append("LEFT OUTER JOIN egswListeTranslation l on r.code=l.codeListe and l.codetrans IN (" & intCodeTrans & ",NULL) " & " ")

            ' Join Yield table
            .Append("LEFT OUTER JOIN egswUnit y on y.code=r.yieldUnit ")
            .Append("LEFT OUTER JOIN egswItemTranslation yT on yT.CodeEgswTable=@CODETABLEUNIT AND y.code=yT.code AND yT.codeTrans IN (" & intCodeTrans & ",NULL) AND RTRIM(yT.Name)<>'' ")

            'Join Unit table for SubRecipe unit
            .Append("LEFT OUTER JOIN egswUnit sru on sru.code=r.srunit ")
            .Append("LEFT OUTER JOIN egswItemTranslation sruT on sruT.CodeEgswTable=@CODETABLEUNIT  AND sru.code=sruT.code AND sruT.codeTrans IN (" & intCodeTrans & ",NULL) AND RTRIM(sruT.Name)<>'' ")

            ' Join Category table
            .Append("INNER JOIN  egswCategory c on c.code=r.category ")
            .Append("LEFT OUTER JOIN egswItemTranslation cT on cT.CodeEgswTable=@CODETABLECATEGORY AND c.code=cT.code AND cT.codeTrans IN (" & intCodeTrans & ",NULL)  AND RTRIM(cT.Name)<>''  ")

            'join product table for finished goods in recipe
            .Append("LEFT OUTER JOIN egswProduct product ON r.Code=product.RecipeLinkCode ")

            .Append("INNER JOIN egswSupplier supplier ON supplier.code=r.Supplier ") 'MRC 08.04.08
            .Append("LEFT OUTER JOIN egswSource source ON source.code=r.Source ") 'MRC 08.06.08

            'Join Product Table for merchandise linking of product for salesitem
            '.Append("LEFT OUTER JOIN egswLinkFbRnPOS link on link.CodeListe=r.code AND link.TypeLink=0 ")

            ' Join Brand
            'If strBrand.Length > 0 Then
            .Append("LEFT JOIN egswBrand b ON b.code=r.Brand ")
            .Append("LEFT OUTER JOIN egswItemTranslation bT on c.code=bT.code AND bT.codeTrans IN (" & intCodeTrans & ",NULL) AND bT.CodeEgswTable=18 ")
            'End If

            ' Join Supplier
            'If strSupplier.Length > 0 Then
            '    .Append("INNER JOIN egswSupplier supplier ON supplier.code=r.Supplier ")
            'End If

            ' Join Source
            'If strSource.Length > 0 Then
            '    .Append("LEFT OUTER JOIN egswSource source ON source.code=r.Source ")
            'End If

            ' Join Keywords table
            'If strKeywords.Length <> 0 Then
            '    .Append("INNER JOIN egswKeyDetails kd on r.code=kd.codeListe  ")
            '    .Append("INNER JOIN egswKeyword k on kd.codeKey=k.code ")
            '    .Append("LEFT OUTER JOIN egswItemTranslation kT on k.code=kT.code AND kT.codeTrans IN (" & intCodeTrans & ",NULL) AND kT.CodeEgswTable=dbo.fn_egswGetTableID('egswKeyword') AND RTRIM(kT.Name)<>'' ")
            'End If

            ' Join Ingredients, rnListe table for ingredients and traslation rnListe
            'If strIngredientsWanted.Length <> 0 Or strIngredientsUnwanted.Length <> 0 Then
            '    .Append("LEFT OUTER JOIN egswDetails d on r.code=d.firstCode  ")
            '    .Append("LEFT OUTER JOIN egswListe r1 on  r1.code=d.secondcode ")
            '    .Append("LEFT OUTER JOIN egswListeTranslation l2 on r1.code=l2.codeListe AND l2.codetrans IN (" & intCodeTrans & ",NULL) ")
            'End If

            'Select Case intListeType
            '    Case enumDataListType.Ingredient, enumDataListType.Merchandise
            '        ' Join Sub Recipes with prices when searching ingredient and merchandise
            '        If intCodeSetPrice <> -1 Then
            '            .Append("LEFT OUTER JOIN egswListeSetPrice p on r.code=p.codeliste AND p.codesetprice=" & intCodeSetPrice & " ")
            '        Else
            '            .Append("LEFT OUTER JOIN egswListeSetPrice p on r.code=p.codeliste ")
            '        End If
            '    Case enumDataListType.Recipe, enumDataListType.Menu
            '        ' join calculations when seraching recipes / menu
            'End Select

            If intCodeSetPrice <> -1 Then
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste and pCalc.codesetprice=" & intCodeSetPrice & " ")
            Else
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste ")
            End If
            .Append("LEFT OUTER JOIN EgsWTax pCalcTax on pCalc.tax = pCalcTax.Code ") 'DLS May52009

            .Append("INNER JOIN EgswSite egswSite ON r.CodeSite=egswSite.Code ") 'VRP 19.05.2008

            'If strNutrientRules.Trim.Length > 0 Then
            '    'join nutrient rules
            '    .Append("INNER JOIN egswNutrientVal ON r.code=egswNutrientVal.CodeListe ")
            'End If

            .Append("WHERE TR.ID BETWEEN @FirstRec AND @LastRec ")
            ' ''BuildFullySharedString2(sbSQL, udtUser, intListeType, cmd)
            'If bIncorrectNetMargin Then
            '    '.Append("AND 100 - (pcalc.calcPrice / ((pCalc.imposedPrice / (ISNULL(pCalcTax.Value,0) + 100)) * 100)) < " & dblMinimumNetMargin & " ")
            '    '.Append("AND (CASE WHEN pCalc.imposedPrice > 0 THEN (100 - (ISNULL(pcalc.calcPrice, 0) / ISNULL(pCalc.imposedPrice, 1))) ELSE 0 END) < " & dblMinimumNetMargin & ")")
            '    .Append("AND (100 - ((pcalc.calcPrice / ISNULL(NULLIF((pCalc.imposedPrice / (ISNULL(NULLIF(pCalcTax.Value,0),1) + 100) * 100), 0), ISNULL(NULLIF(pcalc.calcPrice, 0), 1)) ) * 100)) < " & dblMinimumNetMargin & " ")
            'End If
            .Append("ORDER BY tr.ID ")
        End With

        Try
            With cmd
                .Connection = cn
                Dim strTemp As String = sbSQL.ToString
                .CommandText = sbSQL.ToString
                .CommandTimeout = 10000
                .CommandType = CommandType.Text
                .Parameters.Add("@Page", SqlDbType.Int).Value = intPagenumber
                .Parameters.Add("@RecsPerPage", SqlDbType.Int).Value = intPageSize
                .Parameters.Add("@iRow", SqlDbType.Int).Direction = ParameterDirection.Output

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With

                'intTotalRows = CInt(.Parameters("@iRow").Value)
            End With
            intTotalRows = 0
            If dt.Rows.Count > 0 Then intTotalRows = CInt(dt.Rows.Item(0).Item("iRow"))

            ' IsListeOwned(dt, udtUser.Site.Code)
            Return dt

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
            Return Nothing
        End Try
    End Function

    Public Function GetListeSearchResultTest(ByVal udtUser As structUser, ByVal slParams As SortedList, ByVal intCodeTrans As Integer, ByVal intPagenumber As Integer, ByVal intPageSize As Integer, ByRef intTotalRows As Integer, Optional ByVal blnAllowCreateUseSubRecipe As Boolean = False, Optional ByVal blnFTSEnable As Boolean = False, Optional ByVal strSort As String = "", Optional ByVal intCodeSetPrice As Integer = -1) As DataTable

        'Return GetListeSearchResult2(udtUser, slParams, intCodeTrans, intPagenumber, intPageSize, intTotalRows, blnAllowCreateUseSubRecipe, blnFTSEnable, strSort, intCodeSetPrice)
        'Exit Function

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable("EGSWLISTE")
        Dim sbSQL As New StringBuilder

        Dim strNumber As String = CStr(slParams("NUMBER"))
        'Dim strWord As String = fctTransformStrSearch(CStr(slParams("WORD")))
        Dim strWord As String = CStr(slParams("WORD"))
        Dim strKeywords As String = CStr(slParams("KEYWORDS"))
        Dim strUnKeywords As String = CStr(slParams("KEYUNWANTED")) 'VRP 11.09.2007
        Dim strKeywordType As String = CStr(slParams("KEYTYPE")) 'VRP 18.10.2007
        Dim strCategory As String = CStr(slParams("CATEGORY"))
        Dim strBrand As String = CStr(slParams("BRAND"))
        Dim strSupplier As String = CStr(slParams("SUPPLIER"))
        Dim intListeType As enumDataListType = CType(slParams("TYPE"), enumDataListType)
        Dim intListeTypeFilter As enumDataListType = CType(slParams("LISTETYPE"), enumDataListType) 'mrc 02.11.2010
        Dim strIngredientsWanted As String = fctTransformStrSearch(CStr(slParams("INGWANTED")))
        Dim strIngredientsUnwanted As String = fctTransformStrSearch(CStr(slParams("INGUNWANTED")))
        Dim nUserLevel As enumGroupLevel = CType(slParams("USERLEVEL"), enumGroupLevel)
        Dim strCodeSiteList As String = CStr(slParams("CODESITE"))
        Dim strFilter As String = CStr(slParams("FILTER"))

        'Dim intCodeSite As Integer '= CInt(slParams("CODESITE"))
        Dim intCodeUser As Integer = CInt(slParams("CODEUSER"))
        'Dim intCodeProperty As Integer = CInt(slParams("CODEPROPERTY"))

        Dim strCodelisteList As String = ""
        If Not slParams("MARKITEMLIST") Is Nothing Then strCodelisteList = CStr(slParams("MARKITEMLIST"))

        Dim strSource As String = ""
        If slParams.Contains("SOURCE") Then strSource = CStr(slParams("SOURCE"))

        ' price
        Dim strPrice As String = ""
        If slParams.Contains("PRICE") Then strPrice = CStr(slParams("PRICE"))
        Dim strPriceArr() As String = strPrice.Split(CChar("|"))
        Dim strPriceCol As String = "" ' store price column to search in
        If strPriceArr.Length = 2 Then
            strPriceCol = strPriceArr(0)
            strPrice = strPriceArr(1)
        End If

        If strPrice.IndexOf("-") > 0 Then
            Dim arrPrice() As String = strPrice.Split(CChar("-"))
            'strPrice = " BETWEEN " & CDbl(arrPrice(0)).ToString(New System.Globalization.CultureInfo("en-US")) _
            '                & " AND " & CDbl(arrPrice(1)).ToString(New System.Globalization.CultureInfo("en-US"))

            strPrice = " BETWEEN "
            If CDbl(arrPrice(0)) < CDbl(arrPrice(1)) Then
                strPrice = strPrice & CDbl(arrPrice(0)).ToString(New System.Globalization.CultureInfo("en-US")) _
                          & " AND " & CDbl(arrPrice(1)).ToString(New System.Globalization.CultureInfo("en-US"))
            Else
                strPrice = strPrice & CDbl(arrPrice(1)).ToString(New System.Globalization.CultureInfo("en-US")) _
                          & " AND " & CDbl(arrPrice(0)).ToString(New System.Globalization.CultureInfo("en-US"))
            End If

            'strPrice = " BETWEEN " & strPrice.Replace("-", " AND ")
        ElseIf strPrice.Trim.Length > 0 Then
            If strPrice.IndexOf(">") > -1 Then
                strPrice = ">" & CDbl(strPrice.Replace(">", "")).ToString(New System.Globalization.CultureInfo("en-US"))
            ElseIf strPrice.IndexOf("<") > -1 Then
                strPrice = "<" & CDbl(strPrice.Replace("<", "")).ToString(New System.Globalization.CultureInfo("en-US"))
            End If
        End If

        ' if Price value is [Date1]-[date2], insert "BETWEEN" [Date1] "AND" [date2]
        'If strPrice.IndexOf("-") > 0 Then strPrice = " BETWEEN " & strPrice.Replace("-", " AND ")

        ' add price column to search in
        If strPrice.Length > 0 Then strPrice = strPriceCol & " " & strPrice

        ' date
        Dim strDate As String = ""
        If slParams.Contains("DATE") Then strDate = CStr(slParams("DATE"))
        If strDate.IndexOf("-") > 0 Then strDate = " BETWEEN '" & strDate.Replace("-", "' AND '") & "'"
        If strDate.IndexOf("=") > 0 Then strDate = strDate.Replace("=", "='") & "'"

        ' nutrient rules
        Dim strNutrientRules As String = CStr(slParams("NUTRIENTRULES"))
        If strNutrientRules = Nothing Then strNutrientRules = ""

        ' allergens
        Dim strAllergens As String = CStr(slParams("ALLERGENS"))
        If strAllergens = Nothing Then strAllergens = ""

        'sales
        Dim shrtSalesStatus As Short = 0 '0=show all, 1=show linked listes only, 2=show unlinked liste only
        If slParams.Contains("LINKEDSALES") Then shrtSalesStatus = CShort(slParams("LINKEDSALES"))

        Dim shrtNameOption As Short = 2 'contains
        If slParams.Contains("NAMEOPTION") Then shrtNameOption = CShort(slParams("NAMEOPTION"))

        ' ''if name search is not by exact match, transforn text
        ''strWord = strWord.Trim 'DLS
        ''If shrtNameOption <> 0 Then strWord = fctTransformStrSearch(strWord)

        Dim shrtNumberOption As Short = 2 'contains
        If slParams.Contains("NUMBEROPTION") Then shrtNumberOption = CShort(slParams("NUMBEROPTION"))

        'search global only 'DLS JUne252007
        Dim bGlobalOnly As Boolean = False
        If Not slParams("GLOBALONLY") Is Nothing Then bGlobalOnly = CBool(slParams("GLOBALONLY"))

        If strUnKeywords Is Nothing Then strUnKeywords = ""

        'search by code
        Dim blnSearchByCode As Boolean = False
        Dim intCode As Integer = -1
        If slParams.Contains("CODE") Then
            intCode = CInt(slParams.Item("CODE"))
            If intCode > 0 Then blnSearchByCode = True
            If strCodelisteList.Length > 0 Then blnSearchByCode = True
        End If

        'DLS 16.08.2007
        'Dim bExcludeKeywords As Boolean = CBool(slParams.Item("EXCLUDEKEY"))
        Dim nWithNutrientInfo As Integer = CInt(slParams.Item("WITHNUTRIENT"))
        Dim nUsedUnused As Integer = CInt(slParams.Item("USEDUNUSED"))
        Dim nWithComposition As Integer = CInt(slParams.Item("WITHCOMPOSITION"))

        Dim nNutrientEnergy As Integer = CInt(slParams.Item("NUTRIENTENERGY"))

        'nutrient summary
        Dim nNutrientSummary As Integer = CInt(slParams("NUTRIENTSUMMARY")) 'VRP 15.02.2008
        If nNutrientSummary = Nothing Then nNutrientSummary = 0

        'MRC - 09.03.08 - keyword option
        Dim intKeywordOption As Integer = CInt(slParams("KEYWORDOPTION")) 'MRC - 09.03.08
        If intKeywordOption = Nothing Then intKeywordOption = 0
        Dim intKeywordUnwantedOption As Integer = CInt(slParams("KEYWORDUNWANTEDOPTION")) 'MRC - 09.03.08
        If intKeywordUnwantedOption = Nothing Then intKeywordUnwantedOption = 0

        Dim intPictureOption As Integer = CInt(slParams("PICTUREOPTION")) 'VRP 15.09.2008
        Dim nUsedOnline As Integer = CInt(slParams("USEDONLINE")) 'VRP 30.09.2008
        Dim nTranslated As Integer = CInt(slParams("TRANSLATED")) 'VRP 02.02.2009

        Dim nSharedSite As Integer = CInt(slParams("SHAREDSITE")) 'VRP 21.03.2009

        'DLS
        Dim strKeywordsCode As String = ""
        Dim strUnKeywordsCode As String = ""

        'Remove trailing space - MRC - 09.03.08
        If strKeywords.IndexOf(",") > -1 Then
            Dim str() As String = strKeywords.Split(CChar(","))
            strKeywords = ""
            Dim i As Integer = 0
            While i < str.Length
                strKeywords += str(i).Trim
                i += 1
                If i < str.Length Then
                    strKeywords += ","
                End If
            End While
        End If

        'Remove trailing space - MRC - 09.03.08
        If strUnKeywords.IndexOf(",") > -1 Then
            Dim str() As String = strUnKeywords.Split(CChar(","))
            strUnKeywords = ""
            Dim i As Integer = 0
            While i < str.Length
                strUnKeywords += str(i).Trim
                i += 1
                If i < str.Length Then
                    strUnKeywords += ","
                End If
            End While
        End If

        'MRC - 09.04.08 - Added type of liste as a param for keywords search.
        If strKeywords <> "" Then
            strKeywordsCode = GetKeywordsListCode(strKeywords, intCodeTrans, intListeType)
            If strKeywordsCode = "" Then strKeywordsCode = "0" 'VRP 27.02.2009
        End If

        If strUnKeywords <> "" Then
            strUnKeywordsCode = GetKeywordsListCode(strUnKeywords, intCodeTrans, intListeType)
            If strUnKeywordsCode = "" Then strUnKeywordsCode = "0" 'VRP 27.02.2009
        End If
        '----

        With sbSQL
            .Append("SET NOCOUNT ON ")
            .Append("DECLARE @RecCount int ")
            .Append("SELECT @RecCount = @RecsPerPage * @Page + 1 ")
            .Append("IF @Page=0 SET @Page=1 ")

            .Append("DECLARE @FirstRec int, @LastRec int, @MoreRecords int ")
            .Append("DECLARE @CODETABLECATEGORY int ")
            .Append("DECLARE @CODETABLEUNIT int ")
            .Append("DECLARE @CODETABLELISTE int ")


            '-------- FOR FULL TEXT -------------
            .Append("DECLARE @LANGBREAKER nvarchar(200) ")
            .Append("SELECT @LANGBREAKER = LangBreaker FROM EgsWTranslation WHERE Code= " & intCodeTrans & " ")
            .Append("SET @LANGBREAKER = ISNULL(@LANGBREAKER,'NEUTRAL') ")

            .Append("SET @CODETABLECATEGORY = 19 ") '--dbo.fn_egswGetTableID('egswCategory') 
            .Append("SET @CODETABLEUNIT = 135 ") '--dbo.fn_egswGetTableID('egswUnit') 
            .Append("SET @CODETABLELISTE = 50 ") '--dbo.fn_egswGetTableID('egswListe') 

            '.Append("SELECT @FirstRec = (@Page - 1) * @RecsPerPage ")
            '.Append("SELECT @LastRec = @Page * @RecsPerPage + 1 ")
            .Append("SELECT @FirstRec = (@Page - 1) * @RecsPerPage + 1 ")
            .Append("SELECT @LastRec = @Page * @RecsPerPage ")
            '.Append("CREATE TABLE #TempResults ")
            '.Append("( ")
            '.Append("ID int IDENTITY, ")
            '.Append("code int, ")
            '.Append("name nvarchar(260), ")
            '.Append("number nvarchar(50), ")
            '.Append("dates datetime, ")
            '.Append("price float ")
            '.Append(") ")

            '.Append("INSERT INTO #TempResults (code, name, number, dates, price) ")
            .Append(" ;WITH ListePage AS ")
            .Append("( ") 'start of recpage
            '.Append("SELECT DISTINCT  CASE WHEN ISNULL(l.name,'') = '' THEN r.name ELSE l.name end name,r.code, r.number, r.dates, ")
            '.Append("SELECT DISTINCT  CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name + cast(r.code as varchar(20)) ELSE l.name + cast(r.code as varchar(20)) end name,r.code, r.number, r.dates, ")

            'Added Codesite to sort, for autogrill.
            .Append("SELECT DISTINCT  CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name + cast(r.code as varchar(20)) ELSE l.name + cast(r.code as varchar(20)) end name,r.code, r.number, r.dates, site2.Name AS Site,")

            If intListeType = enumDataListType.Merchandise Then
                .Append(" p.price, ")
            Else
                .Append(" 0 as price, ")
            End If

            ''If strSort = "" Then
            ''    .Append(" ORDER BY [name] ")
            ''Else
            ''    .Append(" ORDER BY " & strSort & " ")
            ''End If
            Dim strSort2 As String
            If strSort Is Nothing Then strSort = ""
            If strSort.ToLower = "name ASC".ToLower Then
                strSort = "r.name ASC"
            ElseIf strSort.ToLower = "name DESC".ToLower Then
                strSort = "r.name DESC"
            End If

            Select Case strSort
                Case "r.name ASC" : strSort2 = "(CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name + '_' + cast(r.code as varchar(20)) ELSE l.name + '_' + cast(r.code as varchar(20)) end) ASC "
                Case "r.name DESC" : strSort2 = "(CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name  + '_' + cast(r.code as varchar(20)) ELSE l.name  + '_' + cast(r.code as varchar(20)) end)DESC "
                Case Else
                    strSort2 = strSort
            End Select

            If strSort = "rank DESC" Then strSort = ""

            If strSort = "" Then
                .Append(" DENSE_RANK() OVER(Order BY (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name + '_' + cast(r.code as varchar(20)) ELSE l.name + '_' + cast(r.code as varchar(20)) end) ASC  ) AS ID ")
                '.Append(" ROW_NUMBER() OVER(Order BY r.name ASC) AS ID ")
            ElseIf strSort = "r.CodeSite ASC" Then
                .Append(" DENSE_RANK() OVER(Order BY (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN site2.Name + '_' + r.name + '_' + cast(r.code as varchar(20)) ELSE site2.Name + '_' + l.name + '_' + cast(r.code as varchar(20)) end) ASC  ) AS ID ")
            ElseIf strSort = "r.CodeSite DESC" Then
                .Append(" DENSE_RANK() OVER(Order BY (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN site2.Name + '_' + r.name + '_' + cast(r.code as varchar(20)) ELSE site2.Name + '_' + l.name + '_' + cast(r.code as varchar(20)) end) DESC  ) AS ID ")
            Else
                .Append(" DENSE_RANK() OVER(Order BY " & strSort2 & ") AS ID ")
            End If


            .Append("FROM egswListe r ")
            .Append("INNER JOIN egswSharing ON egswSharing.Code=r.Code AND egswSharing.CodeEgswTable=@CODETABLELISTE  ")

            If strCodeSiteList = udtUser.Site.Code.ToString Then
                .Append(" AND " & GetStrForSharing("egswSharing", udtUser.Site.Group, strCodeSiteList, udtUser.Code)) 'DLS joining sharing condition
            Else
                .Append(" AND " & GetStrForSharing("egswSharing", -1, strCodeSiteList, -1))
            End If

            .Append("LEFT OUTER JOIN EgswSite site2 ON r.CodeSite=site2.Code ")

            ' Join rnListeTranslation table
            '.Append("LEFT OUTER JOIN egswListeTranslation l on r.code=l.codeListe and l.codetrans IN (" & intCodeTrans & ",NULL) " & " ")
            .Append("LEFT OUTER JOIN egswListeTranslation l on r.code=l.codeListe and l.codetrans =" & intCodeTrans & " " & " ")

            '' Join Category table
            '.Append("INNER JOIN  egswCategory c on c.code=r.category  ")
            '.Append("LEFT OUTER JOIN egswItemTranslation cT on c.code=cT.code AND cT.codeTrans IN (" & intCodeTrans & ",NULL) AND cT.CodeEgswTable=@CODETABLECATEGORY AND RTRIM(cT.Name)<>''  ")

            If strBrand Is Nothing Then strBrand = ""
            ' '' Join Brand
            'If strBrand.Length > 0 Then
            '    .Append("LEFT JOIN egswBrand b ON b.code=r.Brand ")
            '    .Append("LEFT OUTER JOIN egswItemTranslation bT on b.code=bT.code AND bT.codeTrans IN (" & intCodeTrans & ",NULL) AND bT.CodeEgswTable=dbo.fn_egswGetTableID('egswBrand') ")
            'End If

            If strSupplier Is Nothing Then strSupplier = ""
            '' Join Supplier
            'If strSupplier.Length > 0 Then
            '.Append("INNER JOIN egswSupplier supplier ON supplier.code=r.Supplier ")
            'End If

            If strSource Is Nothing Then strSource = ""
            '' Join Source
            'If strSource.Length > 0 Then
            '.Append("LEFT OUTER JOIN egswSource source ON source.code=r.Source ")
            'End If

            '' Join Keywords table
            'If strKeywords.Length > 0 Or strUnKeywords.Length > 0 Then
            '    .Append("INNER JOIN egswKeyDetails kd on r.code=kd.codeListe  ")
            '    .Append("INNER JOIN egswKeyword k on kd.codeKey=k.code ")
            '    .Append("LEFT OUTER JOIN egswItemTranslation kT on k.code=kT.code AND kT.codeTrans IN (" & intCodeTrans & ",NULL) AND kT.CodeEgswTable=dbo.fn_egswGetTableID('egswKeyword') AND RTRIM(kT.Name)<>'' ")
            'End If

            ' Join Ingredients, rnListe table for ingredients and traslation rnListe
            If strIngredientsWanted Is Nothing Then strIngredientsWanted = ""
            If strIngredientsUnwanted Is Nothing Then strIngredientsUnwanted = ""

            If strIngredientsWanted.Length > 0 Or strIngredientsUnwanted.Length > 0 Then
                .Append("LEFT OUTER JOIN egswDetails d on r.code=d.firstCode  ")
                .Append("LEFT OUTER JOIN egswListe r1 on  r1.code=d.secondcode ")
                .Append("LEFT OUTER JOIN egswListeTranslation l2 on r1.code=l2.codeListe AND l2.codetrans IN (" & intCodeTrans & ",NULL) ")
            End If

            Select Case intListeType
                Case enumDataListType.Ingredient, enumDataListType.Merchandise
                    ' Join Sub Recipes with prices when searching ingredient and merchandise
                    If intCodeSetPrice <> -1 Then
                        .Append("LEFT OUTER JOIN egswListeSetPrice p on r.code=p.codeliste AND p.codesetprice=" & intCodeSetPrice & " AND p.Position=1 ")
                    Else
                        .Append("LEFT OUTER JOIN egswListeSetPrice p on r.code=p.codeliste AND p.position=1 ")
                    End If
                Case enumDataListType.Recipe, enumDataListType.Menu
                    ' join calculations when seraching recipes / menu
            End Select

            If intCodeSetPrice <> -1 Then
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste and pCalc.codesetprice=" & intCodeSetPrice & " ")
            Else
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste ")
            End If

            'join nutrient rules
            If strNutrientRules Is Nothing Then strNutrientRules = ""
            If strNutrientRules.Trim.Length > 0 Then
                .Append("INNER JOIN egswNutrientVal ON r.code=egswNutrientVal.CodeListe ")
            End If



            'join allergens
            If strAllergens Is Nothing Then strAllergens = ""
            If strAllergens.Length > 0 Then
                .Append("LEFT OUTER JOIN egswListeAllergen a ON a.CodeListe=r.code ")
            End If

            .Append("WHERE ")

            If blnSearchByCode Then
                If intCode > 0 Then
                    .Append(" r.code=" & intCode & " ")
                Else
                    .Append(" r.code IN " & strCodelisteList & " ")
                End If
            Else
                ' type
                Select Case intListeType
                    Case enumDataListType.MenuItems
                        Select Case intListeTypeFilter
                            Case enumDataListType.NoType, enumDataListType.Ingredient
                                .Append("(r.Type IN (2,4) OR r.type=8) ")
                            Case Else
                                .Append("r.Type=" & intListeTypeFilter & " ")
                        End Select

                    Case Else
                        Select Case intListeTypeFilter
                            Case enumDataListType.Ingredient
                                If blnAllowCreateUseSubRecipe Then
                                    .Append("(r.Type IN (2,4) OR (r.type=8 and r.srQty>0)) ")
                                Else
                                    .Append("(r.Type IN (2,4)) ")
                                End If
                            Case enumDataListType.Merchandise
                                .Append("(r.Type IN (2,4)) ")
                            Case enumDataListType.Recipe
                                .Append("(r.type=8 and r.srQty>0) ")
                            Case enumDataListType.Text
                                .Append("(r.Type = 4) ")
                            Case enumDataListType.NoType
                                .Append("r.Type=" & intListeType & " ")
                                .Append(" and (r.[use]=1 OR (r.codeUser =" & intCodeUser & " AND r.type IN (2,8,16))) ") 'Exclude drafts
                        End Select



                        'If intListeTypeFilter = enumDataListType.Recipe Then
                        '    .Append("r.type=8 and r.srQty>0")
                        '    .Append(" and (r.[use]=1 OR (r.codeUser =" & intCodeUser & " AND r.type IN (2,8,16))) ") 'Exclude drafts
                        'Else
                        '    .Append("r.Type=" & intListeTypeFilter & " ")
                        '    .Append(" and (r.[use]=1 OR (r.codeUser =" & intCodeUser & " AND r.type IN (2,8,16))) ") 'Exclude drafts
                        'End If

                End Select
                '.Append(" AND r.protected=0 ") 'MRC 05.15.09 we now use this fields

                'DLS Commented
                ''If intListeType = enumDataListType.Merchandise Or intListeType = enumDataListType.Recipe Or intListeType = enumDataListType.Menu Or intListeType = enumDataListType.Ingredient Then
                ''    'check if user is searching his own site, if searching his own site, get all dat is shared to d user, user'site and user's property
                ''    If CStr("," & strCodeSiteList & ",").IndexOf("," & CStr(udtUser.Site.Code) & ",") > -1 Then
                ''        'get sharing of user
                ''        .Append("AND ((r.[use]=1 AND egswSharing.CodeUserSharedTo=" & CStr(udtUser.Site.Group) & " AND egswSharing.Type IN(" & ShareType.CodeProperty & ", " & ShareType.CodePropertyView & ")) ")
                ''        '.Append("OR (r.[use]=1 AND egswSharing.CodeUserSharedTo=" & CStr(udtUser.Site.Code) & " AND egswSharing.Type IN(" & ShareType.CodeSite & ", " & ShareType.CodeSiteView & ")) ")
                ''        .Append("OR (r.[use]=1 AND egswSharing.CodeUserSharedTo IN (" & strCodeSiteList & ") AND egswSharing.Type IN(" & ShareType.CodeSite & ", " & ShareType.CodeSiteView & ")) ")
                ''        .Append("OR (r.[use]=1 AND egswSharing.CodeUserSharedTo=" & CStr(udtUser.Code) & " AND egswSharing.Type IN(" & ShareType.CodeUser & ", " & ShareType.CodeUserView & ")) ")
                ''        .Append("OR (r.[use]=1 AND egswSharing.Type IN(" & ShareType.ExposedViewing & ")) ")

                ''        'd one who created
                ''        If intListeType <> enumDataListType.MenuItems AndAlso intListeType <> enumDataListType.Ingredient Then
                ''            .Append("OR (")
                ''            .Append("(egswSharing.CodeUserSharedTo=" & CStr(intCodeUser) & " ")
                ''            .Append("AND egswSharing.Type=" & ShareType.CodeUserOwner & " ")
                ''            .Append("AND r.[use]=0 AND r.type IN (2,8,16))) ")
                ''        End If
                ''    Else
                ''        .Append(" AND r.[use]=1 AND ((egswSharing.CodeUserSharedTo IN (" & strCodeSiteList & ") AND egswSharing.Type IN(" & ShareType.CodeSite & ")) ")
                ''    End If
                ''    .Append(") ")
                ''Else
                ''    .Append("AND (egswSharing.CodeUserSharedTo IN (" & strCodeSiteList & ") AND egswSharing.Type IN(" & ShareType.CodeSite & ")) ")
                ''End If

                ' Flags
                '.Append(" AND r.protected=0 ") 'MRC 09.01.08

                '' ''If strWord.Length > 0 Then
                '' ''    If shrtNameOption = 0 Then 'exact
                '' ''        '' find match in rnliste table
                '' ''        '.Append("AND (r.Name = @nvcWord ")
                '' ''        '' find match in rnlistetranslation table
                '' ''        '.Append("OR (l.name = @nvcWord ")
                '' ''        '.Append("AND l.codetrans=" & intCodeTrans & ")) ")
                '' ''        'DLS
                '' ''        .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) = @nvcWord ")

                '' ''    ElseIf shrtNameOption = 1 Then
                '' ''        strWord = strWord & "%" ' always use like
                '' ''        ' find match in rnliste table
                '' ''        '.Append("AND (r.Name like @nvcWord ")
                '' ''        '''' find match in rnlistetranslation table
                '' ''        '.Append("OR (l.name like @nvcWord ")
                '' ''        '.Append("AND l.codetrans=" & intCodeTrans & ")) ")
                '' ''        '.Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")
                '' ''        'strWord = fctTransformStrSearch(strWord)

                '' ''        strWord = strWord & "%" ' always use like
                '' ''        .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")
                '' ''    Else 'contains
                '' ''        strWord = "%" & strWord & "%" ' always use like
                '' ''        '' find match in rnliste table
                '' ''        '.Append("AND (r.Name like @nvcWord ")
                '' ''        '''' find match in rnlistetranslation table
                '' ''        '.Append("OR (l.name like @nvcWord ")
                '' ''        '.Append("AND l.codetrans=" & intCodeTrans & ")) ")
                '' ''        'strWord = fctTransformStrSearch(strWord)
                '' ''        strWord = "%" & strWord & "%" ' always use like
                '' ''        .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")
                '' ''    End If

                '' ''    cmd.Parameters.Add("@nvcWord", SqlDbType.NVarChar, 500).Value = strWord.Trim
                '' ''End If

                'DLS
                If strWord.Length > 0 Then
                    If shrtNameOption = 0 Then 'exact
                        '' find match in rnliste table
                        .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) = @nvcWord ")
                    ElseIf shrtNameOption = 1 Then

                        strWord = fctTransformStrSearch(strWord)

                        strWord = strWord & "%" ' always use like
                        .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")


                    Else 'contains
                        Dim flagFullTextEnable As Boolean = IsFullTextEnabled()

                        If fctSearchTextOKForFullText(strWord) = False Then
                            flagFullTextEnable = False
                        End If

                        If flagFullTextEnable = False Then
                            strWord = fctTransformStrSearch(strWord)
                            'mrc - 06.25.09 : Use of Asterisk
                            If strWord.IndexOf("*") > -1 Then
                                strWord = strWord.Replace("*", "%") ' replace asterisk with percent
                                .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")
                            Else
                                strWord = "%" & strWord & "%" ' always use like
                                .Append("AND (CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end) like @nvcWord ")
                            End If
                        Else
                            strWord = fctTransformStrSearchFullText(strWord)
                            .Append("AND (CONTAINS(r.Name,@nvcWord, LANGUAGE @LANGBREAKER) OR CONTAINS(l.Name,@nvcWord, LANGUAGE @LANGBREAKER))  ")
                        End If


                    End If

                    cmd.Parameters.Add("@nvcWord", SqlDbType.NVarChar, 2000).Value = ReplaceSpecialCharacters(strWord)
                End If

                If strNumber.Length > 0 Then
                    If shrtNumberOption = 0 Then 'exact
                        ' find match in rnliste table
                        .Append("AND r.Number = @nvcNumber ")
                    ElseIf shrtNumberOption = 1 Then
                        strNumber = strNumber & "%" ' always use like
                        ' find match in rnliste table
                        .Append("AND r.Number like @nvcNumber ")
                    Else 'contains
                        strNumber = "%" & strNumber & "%" ' always use like
                        ' find match in rnliste table
                        .Append("AND r.Number like @nvcNumber ")
                    End If
                    cmd.Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 25).Value = ReplaceSpecialCharacters(strNumber)
                End If

                ' Date
                If strDate.Length > 0 Then .Append(" AND r.dates " & strDate & " ")

                ' Price
                Select Case intListeType
                    Case enumDataListType.Merchandise
                        If strPrice.Length <> 0 Then
                            .Append("AND p." & strPrice & " ")
                        End If
                    Case enumDataListType.Recipe, enumDataListType.Menu
                        If strPrice.Length <> 0 Then
                            .Append("AND pCalc." & strPrice & " ")
                        End If
                End Select

                ' Wanted Ingredient Search
                If strIngredientsWanted.Length > 0 Then
                    Dim strSQLEintCodeIng1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR r1.name LIKE ", "@nvcIngWanted", ReplaceSpecialCharacters(strIngredientsWanted), CChar(","), True)
                    Dim strSQLEintCodeIng2 As String = AddParam(cmd, SqlDbType.NVarChar, " OR l2.name LIKE ", "@nvcIng2Wanted", ReplaceSpecialCharacters(strIngredientsWanted), CChar(","), True)
                    Dim strSQLEintCodeIng3 As String = AddParam(cmd, SqlDbType.NVarChar, " OR d.name LIKE ", "@nvcIng3Wanted", ReplaceSpecialCharacters(strIngredientsWanted), CChar(","), True)

                    ' Find match in ingredients
                    .Append("AND (r1.Name like " & strSQLEintCodeIng1 & " ")

                    ' find match ingredient in rnliste translation table
                    .Append("OR (l2.Name like " & strSQLEintCodeIng2 & " ")
                    '.Append("AND l2.codeTrans=" & intCodeTrans & ")) ")
                    .Append("AND l2.codeTrans=" & intCodeTrans & ") ")

                    'MRC - 12.02.08 - Added search of ingredients in textmode
                    .Append("OR d.Name LIKE " & strSQLEintCodeIng3 & ") ")
                End If

                ' Unwanted Ingredient Search
                If strIngredientsUnwanted.Length <> 0 Then
                    Dim strSQLEintCodeIngUw1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR name LIKE ", "@nvcIngUnWanted", ReplaceSpecialCharacters(strIngredientsUnwanted), CChar(","), True)
                    'compare it using egswliste.anme w/codetarns
                    .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select code FROM egswListe WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND CodeTrans=" & intCodeTrans & " ) ) ")
                    'compare it using egswlistetransaltion.name w/codetrans
                    .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select codeliste FROM egswlistetranslation WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND codetrans=" & intCodeTrans & " ) ) ")
                    'compare it using egswliste.name w/o codetrans for liste's w/o translstions
                    .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select code FROM egswListe WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND Code NOT IN (SELECT codeListe FROM egswListeTranslation WHERE CodeTrans=" & intCodeTrans & " )) ) ")
                    'compare to text mode
                    .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE name LIKE " & strSQLEintCodeIngUw1 & ") ")
                End If

                'brand
                If strBrand.Length > 0 Then
                    '.Append("AND (b.name=@nvcBrand OR bT.name=@nvcBrand) ")
                    'cmd.Parameters.Add("@nvcBrand", SqlDbType.NVarChar, 150).Value = strBrand
                    .Append("AND r.brand =" & strBrand & " ") 'VRP 12.03.2008
                End If

                'category
                If strCategory.Length > 0 And strCategory <> "-1" Then
                    '.Append("AND (c.name=@nvcCategory OR cT.name=@nvcCategory) ")
                    'cmd.Parameters.Add("@nvcCategory", SqlDbType.NVarChar, 150).Value = strCategory
                    .Append(" AND r.category=" & strCategory & " ") 'VRP 12.03.2008
                End If

                'source
                If strSource.Length > 0 Then
                    '.Append("AND source.name=@nvcSource ")
                    'cmd.Parameters.Add("@nvcSource", SqlDbType.NVarChar, 150).Value = strSource
                    .Append(" AND r.Source=" & strSource & " ") 'VRP 12.03.2008
                End If

                ' SUPPLIER
                If strSupplier.Length > 0 Then
                    '.Append("AND supplier.nameref=@nvcSupplier ")
                    'cmd.Parameters.Add("@nvcSupplier", SqlDbType.NVarChar, 150).Value = strSupplier
                    .Append("AND r.Supplier=" & strSupplier & " ") 'VRP 12.03.2008
                End If

                'GlobalOnly
                If bGlobalOnly Then 'DLS June252007
                    .Append(" AND r.IsGlobal=1 ")
                End If

                'DLS 17.08.2007
                'Used/UnUsed as Ingredients
                If nUsedUnused = 1 Then 'used
                    .Append(" AND  0 < (SELECT count(SecondCode) FROM EgsWDetails WHERE SecondCode = R.Code) ")
                ElseIf nUsedUnused = 2 Then 'unused
                    .Append(" AND  0 = (SELECT count(SecondCode) FROM EgsWDetails WHERE SecondCode = R.Code) ")
                End If

                'DLS 17.08.2007
                'With Ingredients on Merchandise/ With Composistion on Labels
                If nWithComposition = 1 And intListeType = enumDataListType.Merchandise Then
                    .Append(" AND ISNULL(r.Ingredients,'') <> ''  ")
                ElseIf nWithComposition = 2 And intListeType = enumDataListType.Merchandise Then
                    .Append(" AND ISNULL(r.Ingredients,'') = ''  ")
                ElseIf nWithComposition = 1 And intListeType = enumDataListType.Recipe Then
                    .Append(" AND ISNULL((select Composition FROM EgsWLabel INNER JOIN EgsWProduct ON EgsWProduct.Code = EgsWLabel.CodeProduct AND  EgsWProduct.Code = R.Code ),'') <> ''  ")
                ElseIf nWithComposition = 2 And intListeType = enumDataListType.Recipe Then
                    .Append(" AND ISNULL((select Composition FROM EgsWLabel INNER JOIN EgsWProduct ON EgsWProduct.Code = EgsWLabel.CodeProduct AND  EgsWProduct.Code = R.Code ),'') = ''  ")
                End If

                'DLS 17.08.2007
                'With Nutrient Info or Without
                If nWithNutrientInfo = 1 Then
                    .Append("   AND dbo.fn_EgsWNutrientValIsBlank(r.code)=0    ")
                ElseIf nWithNutrientInfo = 2 Then
                    .Append("   AND dbo.fn_EgsWNutrientValIsBlank(r.code)=1    ")
                End If

                If nNutrientEnergy = 1 Then 'DLS Dec 10 2007
                    .Append("   AND (SELECT top 1 case WHEN ISNULL(N1,-1) =-1 THEN 0 ELSE N1 END FROM EgsWNutrientVal where CodeListe = r.code) > 0    ")
                ElseIf nNutrientEnergy = 2 Then
                    .Append("   AND (SELECT top 1 case WHEN ISNULL(N1,-1) =-1 THEN 0 ELSE N1 END FROM EgsWNutrientVal where CodeListe = r.code) = 0    ")
                End If

                ' Keywords
                If strKeywordsCode.Length > 0 Then
                    Dim strANDKeywords() As String = strKeywordsCode.Split(CChar(","))
                    Dim intX As Integer = UBound(strANDKeywords) + 1
                    If intKeywordOption = 1 And intX > 1 Then    'AND
                        .Append(" AND " & intX & "= (SELECT count(distinct(CodeKey)) FROM EgsWKeyDetails KD WHERE KD.CodeListe = r.Code AND KD.CodeKey in (" & strKeywordsCode & "))")
                    Else    'OR
                        .Append(" AND r.Code in (SELECT CodeListe FROM EgsWKeyDetails WHERE CodeKey in (" & strKeywordsCode & "))")
                    End If
                End If

                If strUnKeywordsCode.Length > 0 Then
                    Dim strANDKeywordsUnwanted() As String = strUnKeywordsCode.Split(CChar(","))
                    Dim intX As Integer = UBound(strANDKeywordsUnwanted) + 1
                    If intKeywordUnwantedOption = 1 And intX > 1 Then
                        '.Append(" AND " & intX & "= (SELECT count(distinct(CodeKey)) FROM EgsWKeyDetails KD WHERE KD.CodeListe = r.Code AND KD.CodeKey not in (" & strUnKeywordsCode & "))")
                        .Append(" AND " & intX & " <> (SELECT count(distinct(CodeKey)) FROM EgsWKeyDetails KD WHERE KD.CodeListe = r.Code AND KD.CodeKey not in (" & strUnKeywordsCode & "))")
                    Else
                        .Append(" AND r.Code not in (SELECT CodeListe FROM EgsWKeyDetails WHERE CodeKey in (" & strUnKeywordsCode & "))")
                    End If
                End If

                '' Keywords
                'If strUnKeywords.Length > 0 Then
                '    '    Dim strSQLEintCode1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR k.name <> ", "@nvcUNKeyworda", strUnKeywords, CChar(","), True)
                '    '    Dim strSQLEintCode2 As String = AddParam(cmd, SqlDbType.NVarChar, " OR kt.name <> ", "@nvcUNKeywordb", strUnKeywords, CChar(","), True)
                '    '    .Append("AND ((k.name <> " & strSQLEintCode1 & " ")

                '    '    ' find match keyword in keyword parent table translation
                '    '    .Append("OR (kt.name <> " & strSQLEintCode2 & " ")

                '    '    'keyword type
                '    '    If strKeywordType = "Derived" Then
                '    '        .Append("AND kd.Derived = 1 ")
                '    '    ElseIf strKeywords = "Assigned" Then
                '    '        .Append("AND kd.Derived = 0 ")
                '    '    End If

                '    '    .Append("AND kt.codetrans=" & intCodeTrans & "))) ")

                '    .Append(" AND r.Code not in (SELECT CodeListe FROM EgsWKeyDetails WHERE CodeKey in (" & strUnKeywordsCode & "))")
                'End If

                Select Case intPictureOption 'VRP 15.09.2008 picture options
                    Case 1 : .Append(" AND REPLACE(LTRIM(RTRIM(PictureName)), ';','')<>'' ")
                    Case 2 : .Append(" AND REPLACE(LTRIM(RTRIM(PictureName)), ';','')='' ")
                End Select

                If nUsedOnline = 1 Then 'VRP 30.09.2008
                    .Append(" AND r.Online=1 ")
                ElseIf nUsedOnline = 2 Then
                    .Append(" AND r.Online=0 ")
                End If

                If nTranslated = 1 Then 'VRP 02.02.2009
                    .Append(" AND dbo.fn_EgswGetListeTransPerc (r.Code, r.CodeSite, r.Type) >= 100")
                ElseIf nTranslated = 2 Then
                    .Append(" AND dbo.fn_EgswGetListeTransPerc (r.Code, r.CodeSite, r.Type) < 100")
                End If

                'nutrient rules
                If strNutrientRules Is Nothing Then strNutrientRules = ""
                If strNutrientRules.Trim.Length > 0 Then
                    Dim cNutrientRules As clsNutrientRules = New clsNutrientRules(udtUser, enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataSet)
                    Dim arr() As String = strNutrientRules.Split(CChar(","))
                    Array.Sort(arr)

                    Dim i As Integer = 1
                    Dim intLastPosition As Integer = 0
                    Dim arr2() As String
                    While i < arr.Length
                        arr2 = arr(i).Split(CChar("-"))
                        If CInt(arr2(0)) > 0 Then
                            Dim rwTemp As DataRow = CType(cNutrientRules.GetList(CInt(arr2(1))), DataSet).Tables(1).Rows(0)
                            If intLastPosition = CInt(arr2(0)) Then
                                .Append(" OR egswNutrientVal.N" & arr2(0) & " BETWEEN " & CStr(rwTemp("minimum")) & " AND " & CStr(rwTemp("maximum")) & " ")
                            Else
                                If i = 1 Then
                                    .Append(" AND ( ")
                                Else
                                    .Append(" ) AND ( ")
                                End If

                                .Append(" egswNutrientVal.N" & arr2(0) & " BETWEEN " & CStr(rwTemp("minimum")) & " AND " & CStr(rwTemp("maximum")) & " ")
                            End If

                            If i + 1 = arr.Length Then
                                .Append(" ) ")
                            End If

                            intLastPosition = CInt(arr2(0))
                        End If
                        i += 1
                    End While
                End If

                If strAllergens Is Nothing Then strAllergens = ""
                If strAllergens.Length > 0 Then
                    If strAllergens.IndexOf("NOT") > -1 Then
                        .Append(" AND (a.codeAllergen " & strAllergens & " OR a.codeAllergen IS NULL) ")
                    Else
                        .Append(" AND a.codeAllergen " & strAllergens & " ")
                    End If
                End If

                'filter, this only works wen u r searching ur own site
                'If CStr(strCodeSiteList) = CStr(udtUser.Site.Code) Then
                Select Case UCase(strFilter)
                    Case "1" '"'OWNED"
                        .Append(" AND r.CodeSite = " & strCodeSiteList & " ")
                    Case "2" '"PUBLIC"
                        .Append(" AND r.Code IN (SELECT S.Code FROM egsWSharing S WHERE S.CodeEGSWTable=50 AND S.Code = r.Code AND (S.IsGlobal=1 OR S.Type not in (1,8))  ) ")
                    Case "3" '"PRIVATE"
                        .Append(" AND r.Code NOT IN (SELECT S.Code FROM egsWSharing S WHERE S.CodeEGSWTable=50 AND S.Code = r.Code  AND (S.IsGlobal=1 OR S.Type not in (1,8)) ) ")
                    Case "4" '"SHARED"
                        '.Append(" AND r.CodeSite <> " & strCodeSiteList & " ")
                        If nSharedSite <> 0 Then 'VRP 21.03.2009
                            .Append(" AND r.CodeSite= " & nSharedSite & " ")
                        Else
                            .Append(" AND r.CodeSite <> " & strCodeSiteList & " ")
                        End If
                    Case "5" '"DRAFT"
                        .Append(" AND r.[use]=0 and r.submitted=0 ")
                        'Case "6" '"SYSTEM"
                        '    .Append(" AND dbo.fn_EgswIsListeOwnedBySystem(r.code)>0 ")
                    Case "6" '"For Approval" 'DLSXXXXXX
                        .Append(" AND r.submitted=1 ")
                    Case "7" '"Approved" 'DLSXXXXXX
                        .Append(" AND r.approvalstatus=1 AND r.submitted=0 ")
                    Case "8" '"Not Approved" 'DLSXXXXXX
                        .Append(" AND r.approvalstatus=2 AND r.submitted=0 ")
                End Select
                'end if
            End If

            If shrtSalesStatus = 1 Then 'linked only
                If intListeType = enumDataListType.Merchandise Then
                    .Append(" AND r.Code IN (SELECT CodeListe FROM egswLinkFbRnPOS linkMP WHERE linkMP.TypeLink IN (0) AND linkMP.CodeListe=r.code ") 'for merchandise and product
                    .Append("       AND linkMP.CodeProduct IN (SELECT CodeProduct FROM egswLinkFbRnPOS linkPS WHERE linkPS.TypeLink IN (1) AND linkPS.CodeProduct=linkMP.codeProduct)) ") 'for product and salesitem
                ElseIf intListeType = enumDataListType.Recipe OrElse intListeType = enumDataListType.Menu Then
                    .Append(" AND r.Code IN (SELECT CodeListe FROM egswLinkFbRnPOS linkLS WHERE linkLS.TypeLink IN (2) AND linkLS.CodeListe=r.code )") ' for recipes/menus and salesitem
                End If
            ElseIf shrtSalesStatus = 2 Then 'not linked only
                If intListeType = enumDataListType.Merchandise Then
                    .Append(" AND r.Code NOT IN (SELECT CodeListe FROM egswLinkFbRnPOS linkMP WHERE linkMP.TypeLink IN (0) AND linkMP.CodeListe=r.code ") 'for merchandise and product
                    .Append("       AND linkMP.CodeProduct IN (SELECT CodeProduct FROM egswLinkFbRnPOS linkPS WHERE linkPS.TypeLink IN (1) AND linkPS.CodeProduct=linkMP.codeProduct)) ") 'for product and salesitem
                ElseIf intListeType = enumDataListType.Recipe OrElse intListeType = enumDataListType.Menu Then
                    .Append(" AND r.Code NOT IN (SELECT CodeListe FROM egswLinkFbRnPOS WHERE TypeLink IN (2) AND CodeListe=r.code )") ' for recipes/menus
                End If
            End If

            ''If strSort = "" Then
            ''    .Append(" ORDER BY [name] ")
            ''Else
            ''    .Append(" ORDER BY " & strSort & " ")
            ''End If

            .Append(") ") 'end of recpage



            '.Append("SELECT @iRow=COUNT(*) FROM ListePage ")
            '.Append("SELECT @MoreRecords=COUNT(*) FROM ListePage WHERE ID>@LastRec ")

            '.Append("DELETE FROM #TempResults WHERE ID <= @FirstRec OR ID >=@LastRec ")

            ''BuildFullySharedString(sbSQL, udtUser, intListeType, cmd)
            .Append("SELECT DISTINCT tr.ID, r.protected, r.code, r.type, CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end name, ")
            .Append("r.category, r.preparation, r.picturename, ")
            .Append("r.submitted, ISNULL(l.codetrans, r.codeTrans) as codeTrans, ")
            .Append("r.yield as yield, r.[percent], y.format as yieldFormat,ISNULL(yT.name,y.namedef) as yieldname, ")
            .Append("ISNULL(pCalc.coeff, 0) AS coeff, ISNULL(pCalc.calcPrice, 0) AS calcPrice, c.name AS categoryname, y.code as yieldCode, ")
            .Append("r.Supplier, r.source, r.remark, ")
            .Append("r.note, r.dates, r.submitted, replace(r.number, CHAR(1),'') AS NUMBER, ")
            .Append("r.wastage1,r.wastage2, r.wastage3,r.wastage4, ")
            .Append("r.picturename, ")
            .Append("r.srUnit,ISNULL(sruT.name,sru.namedef) as srUnitName, ")
            .Append("ISNULL(pCalc.coeff,0) AS coeff1, ")
            '.Append("r.currency, pCalc.coeff,")

            .Append("(1-((1-r.Wastage1/100.0) *")
            .Append("(1-r.Wastage2/100.0) * ")
            .Append("(1-r.Wastage3/100.0) * ")
            .Append("(1-r.Wastage4/100.0))) * 100.0 as TotalWastage, ")

            '.Append("((ISNULL(pCalc.imposedPrice,0)*100) / (100 + ISNULL(TX.Value,0))) as ImposedSellingPrice, ")
            .Append("ISNULL(pCalc.imposedPrice,0) as ImposedSellingPrice, ") ' this is supposed to be the imposed selling price with tax

            '.Append("dbo.fn_EgswIsListeOwnedBySite (r.code," & udtUser.Site.Code & ") as sOwner, ")

            .Append("r.CodeSite as sOwner, ")
            '.Append("0 as sOwner, ")
            .Append("ISNULL(TR.Code, 0) as IsOwner, ")
            '.Append("dbo.fn_EgswCheckListeFullySharedEditToUser(" & udtUser.Code & ", r.code) as IsOwner, ")
            '.Append("dbo.fn_EgswIsListeOwnedBySystem(r.code) as IsSystemOwned, ") 'checks if it is owned by a ssytem site
            .Append("0 as IsSystemOwned, ") 'checks if it is owned by a ssytem site
            '.Append("0 as IsSystemOwned, ") 'checks if it is owned by a ssytem site
            .Append("CASE r.[Use] WHEN 0 THEN 1 ELSE 0 END AS IsDraft, ") 'checks if it is a draft
            .Append("CASE WHEN r.[use]=1 AND r.IsGlobal=1 THEN 1 ELSE 0 END AS IsGlobal, ") 'checks if it is a global, pending for approval is not cionsidered as global yet
            '.Append("@MoreRecords AS MoreRecords, ")

            .Append(" (SELECT COUNT(*) FROM ListePage) as iRow, ")
            .Append("(SELECT COUNT(*) FROM ListePage WHERE ID>@LastRec) AS MoreRecords, ")

            .Append("dbo.fn_EgswGetSetPriceData(r.code," & intCodeSetPrice & "," & intCodeTrans & ") as SetPriceData, ") ' used in searchlistelist.ascx for setprice computation
            .Append("dbo.fn_EgswGetSetPrice(r.code," & intCodeSetPrice & "," & intCodeTrans & ", '" & strPriceCol & "') as SetPriceValue, ") ' used in searchlistelist.ascx for setprice computation
            .Append("ISNULL(product.Code, 0) AS CodeFG ") ' unit of product/salesitem 
            '.Append("ISNULL(link.CodeUnitProduct, 0) AS CodeUnitProduct, ") ' unit of product/salesitem 
            '.Append("ISNULL(link.CodeProduct, 0) AS CodeProduct ") ' unit of product/salesitem 

            'Select Case intListeType
            '    Case enumDataListType.Ingredient, enumDataListType.Merchandise
            '        .Append("p.price as Price ")
            '    Case Else
            '        .Append("0 as Price ")
            'End Select
            'If bNutrientSummary = True Then 'VRP 15.02.2008
            .Append(", egswNutrientVal.N1 AS N1,egswNutrientVal.N2 AS N2,egswNutrientVal.N3 AS N3,egswNutrientVal.N4 AS N4,egswNutrientVal.N5 AS N5,egswNutrientVal.N6 AS N6,egswNutrientVal.N7 AS N7,egswNutrientVal.N8 AS N8,egswNutrientVal.N9 AS N9,egswNutrientVal.N10 AS N10,egswNutrientVal.N11 AS N11,egswNutrientVal.N12 AS N12,egswNutrientVal.N13 AS N13,egswNutrientVal.N14 AS N14,egswNutrientVal.N15 AS N15 ")
            .Append(", egswNutrientVal.N16 AS N16,egswNutrientVal.N17 AS N17,egswNutrientVal.N18 AS N18,egswNutrientVal.N19 AS N19,egswNutrientVal.N20 AS N20,egswNutrientVal.N21 AS N21,egswNutrientVal.N22 AS N22,egswNutrientVal.N23 AS N23,egswNutrientVal.N24 AS N24,egswNutrientVal.N25 AS N25,egswNutrientVal.N26 AS N26,egswNutrientVal.N27 AS N27,egswNutrientVal.N28 AS N28,egswNutrientVal.N29 AS N29,egswNutrientVal.N30 AS N30 ") 'ADR 04.27.11
            .Append(", egswNutrientVal.N31 AS N31,egswNutrientVal.N32 AS N32,egswNutrientVal.N33 AS N33,egswNutrientVal.N34 AS N34 ") 'ADR 04.27.11
            'End If

            .Append(", r.CodeSite, ISNULL(r.CodeUser,0) as CodeUser, EgswSite.Name AS SiteName, ISNULL(r.[Use],0) as ListeUse, ISNULL(r.ApprovalStatus,0) as ApprovalStatus ") 'DLS

            .Append(", supplier.NameRef AS SupplierName ") 'MRC 08.04.08
            .Append(", source.Name AS SourceName ") 'MRC 08.06.08
            .Append(", r.Brand AS Brand ") 'MRC 08.06.08
            .Append(", CASE WHEN b.name IS NULL OR LEN(RTRIM(LTRIM(b.name)))=0 THEN bT.Name ELSE b.name END AS BrandName ")
            .Append(", r.Protected AS Protected ") 'MRC 09.01.08

            .Append("FROM egswListe r INNER JOIN ListePage tr ON r.code=tr.Code ")

            'If bNutrientSummary = True Then 'VRP 15.02.2008
            .Append("LEFT OUTER JOIN egswNutrientVal ON tr.Code=egswNutrientVal.CodeListe ")
            'End If
            'this was just amede to check if user has edit and owner capabilities
            '.Append("LEFT OUTER JOIN dbo.fn_EgswGetListeFullySharedEditToUserByCodeUser(" & udtUser.Code & ", " & intListeType & ") fullyShared ON r.Code=fullyShared.Code ")

            ''.Append("LEFT OUTER JOIN @tblFullySharedWithEdit fullyShared ON r.Code=fullyShared.Code ")

            ''.Append("INNER JOIN egswSharing ON egswSharing.Code=r.Code AND egswSharing.CodeEgswTable=@CODETABLELISTE ")

            ' Join rnListeTranslation table
            .Append("LEFT OUTER JOIN egswListeTranslation l on r.code=l.codeListe and l.codetrans IN (" & intCodeTrans & ",NULL) " & " ")

            ' Join Yield table
            .Append("LEFT OUTER JOIN egswUnit y on y.code=r.yieldUnit ")
            .Append("LEFT OUTER JOIN egswItemTranslation yT on yT.CodeEgswTable=@CODETABLEUNIT AND y.code=yT.code AND yT.codeTrans IN (" & intCodeTrans & ",NULL) AND RTRIM(yT.Name)<>'' ")

            'Join Unit table for SubRecipe unit
            .Append("LEFT OUTER JOIN egswUnit sru on sru.code=r.srunit ")
            .Append("LEFT OUTER JOIN egswItemTranslation sruT on sruT.CodeEgswTable=@CODETABLEUNIT  AND sru.code=sruT.code AND sruT.codeTrans IN (" & intCodeTrans & ",NULL) AND RTRIM(sruT.Name)<>'' ")

            ' Join Category table
            .Append("INNER JOIN  egswCategory c on c.code=r.category ")
            .Append("LEFT OUTER JOIN egswItemTranslation cT on cT.CodeEgswTable=@CODETABLECATEGORY AND c.code=cT.code AND cT.codeTrans IN (" & intCodeTrans & ",NULL)  AND RTRIM(cT.Name)<>''  ")

            'join product table for finished goods in recipe
            .Append("LEFT OUTER JOIN egswProduct product ON r.Code=product.RecipeLinkCode ")

            .Append("INNER JOIN egswSupplier supplier ON supplier.code=r.Supplier ") 'MRC 08.04.08
            .Append("LEFT OUTER JOIN egswSource source ON source.code=r.Source ") 'MRC 08.06.08

            'Join Product Table for merchandise linking of product for salesitem
            '.Append("LEFT OUTER JOIN egswLinkFbRnPOS link on link.CodeListe=r.code AND link.TypeLink=0 ")

            ' Join Brand
            'If strBrand.Length > 0 Then
            .Append("LEFT JOIN egswBrand b ON b.code=r.Brand ")
            .Append("LEFT OUTER JOIN egswItemTranslation bT on c.code=bT.code AND bT.codeTrans IN (" & intCodeTrans & ",NULL) AND bT.CodeEgswTable=18 ")
            'End If

            ' Join Supplier
            'If strSupplier.Length > 0 Then
            '    .Append("INNER JOIN egswSupplier supplier ON supplier.code=r.Supplier ")
            'End If

            ' Join Source
            'If strSource.Length > 0 Then
            '    .Append("LEFT OUTER JOIN egswSource source ON source.code=r.Source ")
            'End If

            ' Join Keywords table
            'If strKeywords.Length <> 0 Then
            '    .Append("INNER JOIN egswKeyDetails kd on r.code=kd.codeListe  ")
            '    .Append("INNER JOIN egswKeyword k on kd.codeKey=k.code ")
            '    .Append("LEFT OUTER JOIN egswItemTranslation kT on k.code=kT.code AND kT.codeTrans IN (" & intCodeTrans & ",NULL) AND kT.CodeEgswTable=dbo.fn_egswGetTableID('egswKeyword') AND RTRIM(kT.Name)<>'' ")
            'End If

            ' Join Ingredients, rnListe table for ingredients and traslation rnListe
            'If strIngredientsWanted.Length <> 0 Or strIngredientsUnwanted.Length <> 0 Then
            '    .Append("LEFT OUTER JOIN egswDetails d on r.code=d.firstCode  ")
            '    .Append("LEFT OUTER JOIN egswListe r1 on  r1.code=d.secondcode ")
            '    .Append("LEFT OUTER JOIN egswListeTranslation l2 on r1.code=l2.codeListe AND l2.codetrans IN (" & intCodeTrans & ",NULL) ")
            'End If

            'Select Case intListeType
            '    Case enumDataListType.Ingredient, enumDataListType.Merchandise
            '        ' Join Sub Recipes with prices when searching ingredient and merchandise
            '        If intCodeSetPrice <> -1 Then
            '            .Append("LEFT OUTER JOIN egswListeSetPrice p on r.code=p.codeliste AND p.codesetprice=" & intCodeSetPrice & " ")
            '        Else
            '            .Append("LEFT OUTER JOIN egswListeSetPrice p on r.code=p.codeliste ")
            '        End If
            '    Case enumDataListType.Recipe, enumDataListType.Menu
            '        ' join calculations when seraching recipes / menu
            'End Select

            If intCodeSetPrice <> -1 Then
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste and pCalc.codesetprice=" & intCodeSetPrice & " ")
            Else
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on r.code=pCalc.codeliste ")
            End If

            .Append("INNER JOIN EgswSite egswSite ON r.CodeSite=egswSite.Code ") 'VRP 19.05.2008

            '.Append("LEFT OUTER JOIN EgswTax TX ON pCalc.Tax=TX.Code ")  'mrc 02.11.2010

            'If strNutrientRules.Trim.Length > 0 Then
            '    'join nutrient rules
            '    .Append("INNER JOIN egswNutrientVal ON r.code=egswNutrientVal.CodeListe ")
            'End If

            .Append("WHERE TR.ID BETWEEN @FirstRec AND @LastRec ")
            ''BuildFullySharedString2(sbSQL, udtUser, intListeType, cmd)

            .Append("ORDER BY tr.ID ")
        End With

        Try
            With cmd
                .Connection = cn
                .CommandText = sbSQL.ToString
                .CommandTimeout = 10000
                .CommandType = CommandType.Text
                .Parameters.Add("@Page", SqlDbType.Int).Value = intPagenumber
                .Parameters.Add("@RecsPerPage", SqlDbType.Int).Value = intPageSize
                .Parameters.Add("@iRow", SqlDbType.Int).Direction = ParameterDirection.Output

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With

                'intTotalRows = CInt(.Parameters("@iRow").Value)
            End With
            intTotalRows = 0
            If dt.Rows.Count > 0 Then intTotalRows = CInt(dt.Rows.Item(0).Item("iRow"))

            ' IsListeOwned(dt, udtUser.Site.Code)
            Return dt

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
            Return Nothing
        End Try
    End Function

    Public Function RADGET_LISTE(ByVal slParams As SortedList, ByVal udtUser As structUser, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, ByVal intCodeSetPrice As Integer) As DataSet
        Try
            Dim intCodeUser As Integer = udtUser.Code

            Dim intType As enumDataListType = CType(slParams("TYPE"), enumDataListType)
            Dim intCodeListe As Integer = CInt(slParams("CODE"))
            Dim strName As String = fctTransformStrSearch(CStr(slParams("WORD")))
            Dim intNameOption As Integer = CIntDB(slParams("NAMEOPTION"))
            Dim strNumber As String = CStr(slParams("NUMBER"))
            Dim intNumberOption As Integer = CIntDB(slParams("NUMBEROPTION"))
            Dim intKeywordOption As Integer = CIntDB(slParams("KEYWORDOPTION"))
            Dim intUnwantedKeywordOption As Integer = CIntDB(slParams("KEYWORDUNWANTEDOPTION"))

            Dim intCategory, intSource, intFilter, intBrand, intSupplier, intUsedUnused, intPicture, intComposition, intUsedOnline, intPriceType, intPriceOption, intDateOption As Integer
            Dim dblPrice1, dblPrice2 As Double
            Dim strKeyword, strUnwantedKeyword, strMarkItemList, strPrice, strDate As String
            Dim dtsDate1, dtsDate2 As DateTime

            'Category
            If slParams("CATEGORY") = "" Then
                intCategory = -99   '-2     'MKAM 2014.10.29
            Else
                intCategory = CInt(slParams("CATEGORY"))
            End If

            'Source
            If slParams("SOURCE") = "" Then
                intSource = -2
            Else
                intSource = CInt(slParams("SOURCE"))
            End If

            'Filter
            If slParams("FILTER").ToString.ToLower = "all" Or slParams("FILTER") = "" Then
                intFilter = 0
            Else
                intFilter = CInt(slParams("FILTER"))
            End If

            'Keyword
            If slParams("KEYWORDS") = "" Then
                strKeyword = ""
            Else
                strKeyword = CStr(slParams("KEYWORDS"))
            End If

            'MarkID
            If slParams("MARKITEMLIST") = "" Then
                strMarkItemList = ""
            Else
                strMarkItemList = CStr(slParams("MARKITEMLIST"))
            End If

            'Brand
            If slParams("BRAND") = "" Then
                intBrand = -2
            Else
                intBrand = CInt(slParams("BRAND"))
            End If

            'Supplier
            If slParams("SUPPLIER") = "" Then
                intSupplier = -2
            Else
                intSupplier = CInt(slParams("SUPPLIER"))
            End If

            'Used as ingredient
            If slParams("USEDUNUSED") = -1 Then
                intUsedUnused = 0
            Else
                intUsedUnused = CInt(slParams("USEDUNUSED"))
            End If

            'Wanted Ingredients
            Dim strIngredientsWanted As String = fctTransformStrSearch(CStr(slParams("INGWANTED")))

            'Unwanted Ingredients
            Dim strIngredientsUnwanted As String = fctTransformStrSearch(CStr(slParams("INGUNWANTED")))

            'Unwanted Keyword
            If slParams("KEYUNWANTED") = "" Then
                strUnwantedKeyword = ""
            Else
                strUnwantedKeyword = CStr(slParams("KEYUNWANTED"))
            End If

            'Picture            
            If slParams("PICTUREOPTION") = 0 Then
                intPicture = -1
            Else
                intPicture = CInt(slParams("PICTUREOPTION"))
            End If

            'Composition            
            If slParams("WITHCOMPOSITION") = 0 Then
                intComposition = -1
            Else
                intComposition = CInt(slParams("WITHCOMPOSITION"))
            End If

            'Used Online            
            If slParams("USEDONLINE") = 0 Then
                intUsedOnline = -1
            Else
                intUsedOnline = CInt(slParams("USEDONLINE"))
            End If

            'Price
            If slParams("PRICE") = "" Then
                strPrice = ""
            Else
                strPrice = CStr(slParams("PRICE"))
                Dim arrPriceInfo() As String = strPrice.Split("|")
                Select Case arrPriceInfo(0).ToUpper
                    Case "PRICE"
                        intPriceType = 1
                    Case "CALCPRICE"
                        intPriceType = 2
                    Case "IMPOSEDPRICE"
                        intPriceType = 3
                    Case "APPROVEDPRICE"
                        intPriceType = 4
                    Case Else
                        intPriceType = 0
                End Select

                If arrPriceInfo(1).IndexOf("=") > -1 Then
                    intPriceOption = 1
                    dblPrice1 = CDbl(arrPriceInfo(1).Replace("=", "").Trim)
                ElseIf arrPriceInfo(1).IndexOf(">") > -1 Then
                    intPriceOption = 2
                    dblPrice1 = CDbl(arrPriceInfo(1).Replace(">", "").Trim)
                ElseIf arrPriceInfo(1).IndexOf("<") > -1 Then
                    intPriceOption = 3
                    dblPrice1 = CDbl(arrPriceInfo(1).Replace("<", "").Trim)
                ElseIf arrPriceInfo(1).IndexOf("-") > -1 Then
                    intPriceOption = 4
                    dblPrice1 = CDbl(Left(arrPriceInfo(1), arrPriceInfo(1).IndexOf("-")).Trim)
                    dblPrice2 = CDbl(arrPriceInfo(1).Substring(arrPriceInfo(1).IndexOf("-") + 1, arrPriceInfo(1).Length - (arrPriceInfo(1).IndexOf("-") + 1)).Trim)
                End If

            End If

            'Date
            If slParams("DATE") = "" Then
                strDate = ""
                intDateOption = 0
            Else
                strDate = CStr(slParams("DATE"))
                Dim strTempDate As String = ""
                If strDate.IndexOf(">") > -1 Then
                    intDateOption = 2
                    strTempDate = strDate.Replace(">", "").Trim()
                    strTempDate = strTempDate.Replace("=", "").Trim()
                    dtsDate1 = CDate(strTempDate)

                ElseIf strDate.IndexOf("<") > -1 Then
                    intDateOption = 3
                    strTempDate = strDate.Replace("<", "").Trim()
                    strTempDate = strTempDate.Replace("=", "").Trim()
                    dtsDate1 = CDate(strTempDate)

                ElseIf strDate.IndexOf("-") > -1 Then
                    intDateOption = 4
                    Dim arrDateRange() As String = strDate.Split("-")
                    dtsDate1 = CDate(arrDateRange(0).Trim)
                    dtsDate2 = CDate(arrDateRange(1).Trim)

                End If

            End If

            Dim cn As New SqlConnection(L_strCnn)
            Dim cmd As New SqlCommand
            Dim da As New SqlDataAdapter
            Dim dt As New DataTable("SearchResult")
            Dim ds As New DataSet

            With cmd
                .Connection = cn

                'Common Filters for Merchandise Includes (should not be slow, takes 1 sec. to return 12000 resultset):
                ' - CodeListe
                ' - Name, Number
                ' - Category, Brand, Supplier
                ' - Filter
                ' - PictureName
                ' - Composition
                ' - Used Online
                ' - Price
                ' - Date

                'Advanced Filters for Merchandise Includes:
                ' - Keywords / Unwanted Keywords
                ' - Wanted Ingredients / Unwanted Ingredients
                ' - Marked Items                
                ' - Used as ingredient

                'Advanced Filters for Recipe Includes:
                ' - Keywords / Unwanted Keywords
                ' - Wanted Ingredients / Unwanted Ingredients
                ' - Marked Items
                ' - Composition
                ' - Used as ingredient

                If strKeyword.Trim = "" _
                        And strMarkItemList.Trim = "" _
                        And intUsedUnused = 0 _
                        And strUnwantedKeyword = "" _
                        And strIngredientsWanted = "" _
                        And strIngredientsUnwanted = "" Then

                    .CommandText = "[GET_LISTE]"
                Else
                    .CommandText = "[GET_LISTEADVANCED]"
                End If

                'If intType = enumDataListType.Merchandise Then
                'ElseIf intType = enumDataListType.Recipe Then
                'End If

                .CommandTimeout = 10000
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intType", SqlDbType.Int).Value = intType
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice
                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@nvcName", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(strName)
                .Parameters.Add("@tntNameFilter", SqlDbType.Int).Value = intNameOption
                .Parameters.Add("@nvcNumber", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(strNumber)
                .Parameters.Add("@tntNumberFilter", SqlDbType.Int).Value = intNumberOption
                .Parameters.Add("@intCategory", SqlDbType.Int).Value = intCategory
                .Parameters.Add("@intSource", SqlDbType.Int).Value = intSource
                .Parameters.Add("@tntFilter", SqlDbType.Int).Value = intFilter
                .Parameters.Add("@nvcKeyword", SqlDbType.NVarChar).Value = strKeyword
                .Parameters.Add("@tntKeywordFilter", SqlDbType.Int).Value = intKeywordOption
                .Parameters.Add("@nvcMarkItemList", SqlDbType.NVarChar).Value = strMarkItemList
                .Parameters.Add("@intBrand", SqlDbType.Int).Value = intBrand
                .Parameters.Add("@intSupplier", SqlDbType.Int).Value = intSupplier
                .Parameters.Add("@tntUsedAsIngredient", SqlDbType.Int).Value = intUsedUnused
                .Parameters.Add("@nvcWantedIngredients", SqlDbType.NVarChar).Value = strIngredientsWanted
                .Parameters.Add("@nvcUnwantedIngredients", SqlDbType.NVarChar).Value = strIngredientsUnwanted
                .Parameters.Add("@nvcUnwantedKeyword", SqlDbType.NVarChar).Value = strKeyword
                .Parameters.Add("@tntUnwantedKeywordFilter", SqlDbType.Int).Value = intUnwantedKeywordOption
                .Parameters.Add("@tntPicture", SqlDbType.Int).Value = intPicture
                .Parameters.Add("@tntComposition", SqlDbType.Int).Value = intComposition
                .Parameters.Add("@tntUsedOnline", SqlDbType.Int).Value = intUsedOnline
                .Parameters.Add("@tntPriceType", SqlDbType.Int).Value = intPriceType
                .Parameters.Add("@tntPriceOption", SqlDbType.Int).Value = intPriceOption
                .Parameters.Add("@fltPrice1", SqlDbType.Float).Value = dblPrice1
                .Parameters.Add("@fltPrice2", SqlDbType.Float).Value = dblPrice2
                .Parameters.Add("@tntDateOption", SqlDbType.Int).Value = intDateOption

                If intDateOption > 0 Then
                    .Parameters.Add("@dtsDate1", SqlDbType.DateTime).Value = dtsDate1
                End If

                If intDateOption = 4 Then
                    .Parameters.Add("@dtsDate2", SqlDbType.DateTime).Value = dtsDate2
                End If



                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With

                ds.Tables.Add(dt)

            End With

            Return ds

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
            Return Nothing
        End Try
    End Function
    ''' <summary>
    ''' Cleans uneven double-quotes, used for searching
    ''' </summary>
    ''' <param name="str">String to clean</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function CleanUnevenDoubleQuotes(str As String) As String
        Dim strToClean As String = str

        Dim intLastPosition As Integer = 0, intCounter As Integer = 0

        For i As Integer = 0 To strToClean.Length - 1
            If strToClean.Substring(i, 1) = """" Then
                intLastPosition = i
                intCounter += 1
            End If
        Next


        If intCounter Mod 2 > 0 Then ' And intLastPosition > 1 'AGL 2013.12.05 - removed intLastPosition > 1 condition
            strToClean = strToClean.Substring(0, intLastPosition) + IIf(intLastPosition <> strToClean.Length - 1, strToClean.Substring(intLastPosition + 1, strToClean.Length - intLastPosition - 1), "")
            'Else
            '          strToClean = strToClean.Substring(0, strToClean.Length - 1)
        End If


        Return strToClean
    End Function

    Public Function GET_LISTE(ByVal slParams As SortedList, ByVal udtUser As structUser, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, ByVal intCodeSetPrice As Integer, Optional ByVal blnFullText As Boolean = False, Optional ByVal blnEnableInitialDisplay As Boolean = False, Optional ByVal blnCheckStatus As Boolean = True, Optional ByVal blnByName As Boolean = False, Optional ByVal blnIsSubrecipe As Boolean = False) As DataSet
        Try
            Dim intNutrientSet As Integer
            Dim cConfig1 As New clsConfig(enumAppType.WebApp, L_strCnn)
            Dim intCodeSet As Integer = cConfig1.GetConfig(intCodeSite, clsConfig.enumNumeros.NutrientDatabase, clsConfig.CodeGroup.site, "0")


            Dim intSearchType As Integer = 1
            '1 - Simple
            '2 - Advanced
            '=================================================================================
            'Simple search filters only includes, other than that belongs to advanced already:
            '1. Name
            '2. Number
            '3. CodeTrans
            '4. Category
            '5. Source
            '6. Filter & Site

            '=================================================================================
            'Advanced
            '0. Source
            '1. Keywords
            '2. Marked Items
            '3. Brand
            '4. Supplier
            '5. Used/Unused
            '6. Wanted/Unwanted Ingredients
            '7. Pictures
            '8. Composition
            '9. Used Online
            '10.Price
            '11. Date
            '12. Cost per Serving and Recipe -- JBB 02.03.2012

            Dim intCodeUser As Integer = udtUser.Code
            Dim intType As enumDataListType = CType(slParams("TYPE"), enumDataListType)
            Dim intCodeListe As Integer = CIntDB(slParams("CODE"))

            '-- JBB 02.03.2012
            Dim dblcpsrvalue1 As Double = CIntDB(slParams("CPSRValue1"))
            Dim dblcpsrvalue2 As Double = CIntDB(slParams("CPSRValue2"))
            Dim intcpsroperation As Integer = CIntDB(slParams("CPSROperation"))
            Dim intcpsrcosting As Integer = CIntDB(slParams("CPSRCosting"))

            '--

            Dim intNameOption As Integer = CIntDB(slParams("NAMEOPTION"))

            ''-- JBB 06.014.2012
            Dim blIsAND As Boolean = CBoolDB(slParams("IsAND"))
            Dim intRecipeStatus As Integer = CIntDB(slParams("RecipeStatus"))
            Dim intWebStatus As Integer = CIntDB(slParams("WebStatus"))
            Dim strPrimaryBrand As String = CStrDB(slParams("PrimaryBrand"))
            Dim intPrimaryBrandOption As Integer = CInt(slParams("PrimaryBrandOption"))

            ''-- JBB 06.06.2012
            Dim strUPrimaryBrand As String = CStrDB(slParams("UnwantedPrimaryBrand"))
            Dim intUPrimaryBrandOption As Integer = CInt(slParams("UnwantedPrimaryBrandOption"))
            ''--
            'JTOC 11.19.2013
            Dim strSecondaryBrand As String = CStrDB(slParams("SecondaryBrand"))
            Dim intSecondaryBrandOption As Integer = CInt(slParams("SecondaryBrandOption"))
            Dim strUSecondaryBrand As String = CStrDB(slParams("UnwantedSecondaryBrand"))
            Dim intUSecondaryBrandOption As Integer = CInt(slParams("UnwantedSecondaryBrandOption"))

            '// DRR 06.06.2012
            Dim blnFullTranslation As Boolean = CBoolDB(slParams("FullTranslation"))
            '//

            'Name
            'Name

            'AGL 2013.10.25
            Dim theString As String = slParams("WORD") '"1,""2,3,4"",5"

            'AGL 2013.11.05 - 9329
            theString = theString.Replace("""""", """")

            'AGL 2013.11.12
            If theString.Length > 0 And theString.Contains("""") Then
                theString = CleanUnevenDoubleQuotes(theString)
            End If

            Dim blnOneWholeExactString As Boolean = False
            If theString <> "" Then
                Using rdr As New StringReader(theString)
                    Using parser As New TextFieldParser(rdr)
                        parser.TextFieldType = FieldType.Delimited
                        parser.Delimiters = New String() {" ", ","} 'AGL 2013.10.31 - 8922
                        parser.HasFieldsEnclosedInQuotes = True
                        Dim fields() As String = parser.ReadFields()

                        If fields.Length > 1 Then
                            Dim strParsed As String = ""
                            For i As Integer = 0 To fields.Length - 1
                                'If i <> fields.Length - 1 Then
                                '    strParsed &= """*" & fields(i) & "*"" AND "
                                'Else
                                '    strParsed &= """*" & fields(i) & "*"""
                                'End If

                                If i <> fields.Length - 1 Then
                                    strParsed &= fields(i).Replace(",", "") & ", "
                                Else
                                    strParsed &= fields(i).Replace(",", "")
                                End If
                                'lvResults.Items.Add("Field {0}: {1}", i, fields(i))

                            Next

                            slParams("WORD") = strParsed
                            blnOneWholeExactString = False
                        Else
                            slParams("WORD") = slParams("WORD").ToString.Replace("""", "")
                            blnOneWholeExactString = True
                        End If

                    End Using
                End Using
            End If



            Dim strName As String = ""
            If blnFullText Then
                strName = slParams("WORD")
                ''strName = """" + strName + """"

                If strName.Contains(" OR ") = True Or strName.Contains(" AND ") = True Then
                    If strName.Contains("""") = False Then
                        strName = """" + strName + """"

                    End If

                End If
                If strName.Contains("""") = False Then
                    strName = strName.Replace("(", " ")
                    strName = strName.Replace(")", " ")
                    strName = strName.Replace("-", " ")
                    strName = strName.Replace("&", " ")
                    strName = strName.Replace("  ", " ")
                    strName = strName.Replace("  ", " ")
                    strName = strName.Trim()


                End If


                'If strName.Contains(" ") = False Then
                '    'AGL 2013.09.11 - 8419 - added checking if text has been enclosed in double-quotes
                '    If strName.Contains("""") = False Then
                '        If strName <> "" Then
                '            strName = """" + strName + "*"""
                '        End If
                '    End If
                'Else
                '    If strName <> "" Then
                '        strName = """" + strName + "*"""
                '    End If
                'End If

                'If strName.Contains(" ") = False Then
                '    If strName <> "" Then
                '        strName = """" + strName + "*"""
                '    End If
                'End If

                'If strName.Contains(" ") = True Then
                '    'AGL 2013.09.11 - 8419 - added checking if text has been enclosed in double-quotes
                '    If strName.Contains("""") = True Then
                '        strName = strName.Replace("""", "")
                '    End If

                '    If strName <> "" Then
                '        strName = """" + strName + "*"""
                '    End If
                'End If

                'strName = FullTextContainCompatibility(strName, intNameOption, blIsmodify)


                blIsAND = False
            Else
                strName = slParams("WORD")
                Dim blIsmodify As Boolean = True

                If blnOneWholeExactString = True Then 'AGL 2013.10.25
                    blIsmodify = False 'AGL 2013.10.25
                Else
                    blIsmodify = True
                    strName = FullTextContainCompatibility(strName, intNameOption, blIsmodify)
                End If

                If blIsmodify = False Then

                    blIsAND = False
                    'strName = fctTransformStrSearch(CStrDB(slParams("WORD")))
                    If strName <> "" Then
                        strName = "%" & strName & "%"
                    End If

                Else
                    If strName.Contains(" OR ") = False And strName.Contains(" AND ") = False Then
                        blIsAND = False
                        'strName = fctTransformStrSearch(CStrDB(slParams("WORD")))
                        If strName <> "" Then
                            strName = "%" & strName & "%"
                        End If
                    Else
                        blIsAND = True
                    End If
                End If
            End If
            '       Dim strName As String = ""
            '       ' RBAJ-2013.01.09 Fixed fulltext options
            '       If blnFullText Then
            '           strName = slParams("WORD")
            '           strName = FullTextContainCompatibility(strName, intNameOption)
            '           blIsAND = strName.Contains(" OR ") Or strName.Contains(" AND ")
            '       Else
            '           'AGL 2013.02.07 - 3375 - commented codes below and uncommented codes way below (see below)

            '           'strName = slParams("WORD")
            '           'strName = fctTransformStrSearch(CStrDB(slParams("WORD")))
            '           'If strName <> "" Then
            '           '    strName = "%" & strName & "%"
            '           'End If

            '           'below
            '           blIsAND = False
            '           If intNameOption = 0 Then '' JBB  OR 06.13.2012 ''MRC 11.21.2011 - Do not use ascii conversions on the name if the name options is 'EXACT' only.
            '               strName = CStrDB(slParams("WORD"))
            '               If strName.Contains(",") Then 'strName.Contains(" ") Or 
            '                   blIsAND = True
            '                   'AGL 2013.08.21 - 7861
            '                   strName = strName.Replace("(", "").Replace(")", "")

            '                   strName = strName.Replace(",", " ")
            '                   strName = strName.Replace("  ", " ")
            '                   strName = strName.Replace(" ", " OR ")
            '	'AGL 2013.08.05
            '	While strName.EndsWith(" OR ")
            '		strName = strName.Substring(0, strName.Length - 4)
            '	End While
            'Else
            '	blIsAND = False
            '	strName = fctTransformStrSearch(CStrDB(slParams("WORD")))
            '	If strName <> "" Then
            '		strName = "%" & strName & "%"

            '	End If
            '               End If
            '           Else
            '               '''' JBB 06.04.2012 
            '               strName = CStrDB(slParams("WORD"))
            '               If strName.Contains(" ") = True Or strName.Contains(",") Then

            '                   blIsAND = True
            '                   strName = Trim(strName)
            '                   strName = strName.Replace(",", " ")
            '                   strName = strName.Replace("  ", " ")
            '                   strName = strName.Replace(" ", " AND ")
            '               Else
            '                   blIsAND = False
            '                   strName = fctTransformStrSearch(CStrDB(slParams("WORD")))
            '                   If strName <> "" Then
            '                       strName = "%" & strName & "%"
            '                   End If
            '               End If



            '               ''''OLD
            '               ''--strName = fctTransformStrSearch(CStrDB(slParams("WORD")))
            '           End If
            '       End If

            'If the user checked fulltext but did not specify a name... just use normal search
            If blnFullText = True And strName.Trim = "" Then blnFullText = False

            'Number
            Dim strNumber As String = CStrDB(slParams("NUMBER"))
            Dim intNumberOption As Integer = CIntDB(slParams("NUMBEROPTION"))

            'Category
            Dim intCategory As Integer
            If slParams("CATEGORY") = "" Then
                intCategory = -99   '-2     'MKAM 2014.10.29
            Else
                intCategory = CIntDB(slParams("CATEGORY"))
            End If

            'Filter(Owned / Public / Private)
            Dim intFilter As Integer, intSharedSite As Integer
            If slParams("FILTER") = "" Then
                intFilter = 0
            Else
                intFilter = CIntDB(slParams("FILTER"))

                If intFilter = 4 Then
                    intSharedSite = CIntDB(slParams("SHAREDSITE"))
                End If

            End If

            'Source
            Dim intSource As Integer
            If CStrDB(slParams("SOURCE")) = "" Then
                intSource = -2
            Else
                intSource = CIntDB(slParams("SOURCE"))
            End If

            'Keyword
            Dim intKeywordOption As Integer = CInt(slParams("KEYWORDOPTION"))
            Dim strKeywords As String = CStrDB(slParams("KEYWORDS"))
            If strKeywords.IndexOf(",") > -1 Then
                Dim str() As String = strKeywords.Split(CChar(","))
                strKeywords = ""
                Dim i As Integer = 0
                While i < str.Length
                    strKeywords += str(i).Trim
                    i += 1
                    If i < str.Length Then
                        strKeywords += ","
                    End If
                End While
            End If

            'Marked Items
            Dim strMarkedItems As String = CStrDB(slParams("MARKITEMLIST"))

            'Brand
            Dim intBrand As Integer
            If CStrDB(slParams("BRAND")) = "" Then
                intBrand = -2
            Else
                intBrand = CIntDB(slParams("BRAND"))
            End If

            'Brand 'JTOC 11.03.2013
            Dim intPublication As Integer
            If CStrDB(slParams("PUBLICATION")) = "" Then
                intPublication = -2
            Else
                intPublication = CIntDB(slParams("PUBLICATION"))
            End If

            'Supplier
            Dim intSupplier As Integer
            If CStrDB(slParams("SUPPLIER")) = "" Then
                intSupplier = -2
            Else
                intSupplier = CIntDB(slParams("SUPPLIER"))
            End If

            'Used/Unused Ingredient
            Dim intUsedAsIngredient As Integer
            If CStrDB(slParams("USEDUNUSED")) = "" Then
                intUsedAsIngredient = -1
            Else
                intUsedAsIngredient = CIntDB(slParams("USEDUNUSED"))
            End If


            'Wanted Ingredients            
            Dim strWantedIngredients As String = fctTransformStrSearch(CStrDB(slParams("INGWANTED")))

            'Unwanted Ingredients            
            Dim strUnwantedIngredients As String = fctTransformStrSearch(CStrDB(slParams("INGUNWANTED")))

            'Unwanted Keyword
            Dim intUnwantedKeywordOption As Integer = CInt(slParams("KEYWORDUNWANTEDOPTION"))
            Dim strUnwantedKeywords As String = CStrDB(slParams("KEYUNWANTED"))
            If strUnwantedKeywords.IndexOf(",") > -1 Then
                Dim str() As String = strUnwantedKeywords.Split(CChar(","))
                strUnwantedKeywords = ""
                Dim i As Integer = 0
                While i < str.Length
                    strUnwantedKeywords += str(i).Trim
                    i += 1
                    If i < str.Length Then
                        strUnwantedKeywords += ","
                    End If
                End While
            End If

            'Picture
            Dim intPicture As Integer
            If CStrDB(slParams("PICTUREOPTION")) = "" Then
                intPicture = -2
            Else
                intPicture = CIntDB(slParams("PICTUREOPTION"))
            End If

            '-- JBB 06.26.2012
            If intType <> enumDataListType.Recipe Then
                intPicture = -2
            End If
            '---


            'Composition
            Dim intComposition As Integer
            If CStrDB(slParams("WITHCOMPOSITION")) = "" Then
                intComposition = -2
            Else
                intComposition = CIntDB(slParams("WITHCOMPOSITION"))
            End If

            'Used Online
            Dim intUsedOnline As Integer
            If CStrDB(slParams("USEDONLINE")) = "" Then
                intUsedOnline = -2
            Else
                intUsedOnline = CIntDB(slParams("USEDONLINE"))
            End If

            'Price
            Dim strPriceInfo As String = CStrDB(slParams("PRICE"))
            Dim strPrice() As String = strPriceInfo.Split("|")
            Dim intPriceType As Integer, intPriceOption As Integer, dblPrice1 As Double, dblPrice2 As Double
            If strPrice.Length > 0 Then

                If CStrDB(strPrice(0)) = "" Then
                    intPriceType = -2
                Else
                    intPriceType = CIntDB(strPrice(0))
                End If

                If strPrice.Length > 1 Then

                    If CStrDB(strPrice(1)) = "" Then
                        intPriceOption = -2
                        dblPrice1 = 0
                        dblPrice2 = 0
                    Else
                        Dim strPriceDetail() As String = CStrDB(strPrice(1)).Split(" ")

                        If strPriceDetail.Length > 0 Then

                            If CStrDB(strPriceDetail(0)) = "" Then
                                intPriceOption = -2
                            Else

                                intPriceOption = CIntDB(strPriceDetail(0))

                                If strPriceDetail.Length > 1 Then

                                    If CStrDB(strPriceDetail(1)) = "" Then
                                        dblPrice1 = 0
                                        dblPrice2 = 0
                                    Else
                                        Dim strPriceDetail2() As String = CStrDB(strPriceDetail(1)).Split("-")

                                        If strPriceDetail2.Length > 0 Then

                                            If CStrDB(strPriceDetail2(0)) = "" Then
                                                dblPrice1 = 0
                                            Else
                                                dblPrice1 = CDblDB(strPriceDetail2(0))

                                                If strPriceDetail2.Length > 1 Then

                                                    If CStrDB(strPriceDetail2(1)) = "" Then
                                                        dblPrice2 = 0
                                                    Else
                                                        dblPrice2 = CDblDB(strPriceDetail2(1))
                                                    End If

                                                End If

                                            End If

                                        End If

                                    End If

                                End If

                            End If

                        End If

                    End If

                End If
            Else
                intPriceType = -2
                intPriceOption = -2
                dblPrice1 = 0
                dblPrice2 = 0
            End If


            'Date
            Dim strDateInfo As String = CStrDB(slParams("DATE"))
            Dim strDate() As String = strDateInfo.Split(" ")
            Dim intDateOption As Integer, dtsDateTime1 As DateTime, dtsDateTime2 As DateTime
            If strDate.Length > 0 Then

                If CStrDB(strDate(0)) = "" Then
                    intDateOption = -2
                Else
                    intDateOption = CIntDB(strDate(0))

                    If strDate.Length > 1 Then

                        If CStrDB(strDate(1)) = "" Then
                            'nothing
                        Else
                            Dim strDateDetails() As String = CStrDB(strDate(1)).Split("-") ''-- JBB 12.21.2011 -- CStrDB(strDate(1)).Split

                            If strDateDetails.Length > 0 Then

                                If CStrDB(strDateDetails(0)) = "" Then
                                    'nothing
                                Else
                                    dtsDateTime1 = CDateDB(strDateDetails(0))

                                    If strDateDetails.Length > 1 Then

                                        If CStrDB(strDateDetails(1)) = "" Then
                                            'nothing
                                        Else
                                            dtsDateTime2 = CDateDB(strDateDetails(1))
                                        End If

                                    End If
                                End If

                            End If


                        End If

                    End If
                End If

            End If


            'Date 'JTOC 11.03.2013
            Dim strPublicationDateInfo As String = CStrDB(slParams("PUBLICATIONDATE"))
            Dim strPublicationDate() As String = strPublicationDateInfo.Split(" ")
            Dim intPublicationDateOption As Integer, dtsPublicationDateTime1 As DateTime, dtsPublicationDateTime2 As DateTime
            If strPublicationDate.Length > 0 Then

                If CStrDB(strPublicationDate(0)) = "" Then
                    intPublicationDateOption = -2
                Else
                    intPublicationDateOption = CIntDB(strPublicationDate(0))

                    If strPublicationDate.Length > 1 Then

                        If CStrDB(strPublicationDate(1)) = "" Then
                            'nothing
                        Else
                            Dim strPublicationDateDetails() As String = CStrDB(strPublicationDate(1)).Split("-") ''-- JBB 12.21.2011 -- CStrDB(strDate(1)).Split

                            If strPublicationDateDetails.Length > 0 Then

                                If CStrDB(strPublicationDateDetails(0)) = "" Then
                                    'nothing
                                Else
                                    dtsPublicationDateTime1 = CDate(strPublicationDateDetails(0)) 'AGL 2013.10.25 - 8435 - changed CDateDB to CDate

                                    If strPublicationDateDetails.Length > 1 Then

                                        If CStrDB(strPublicationDateDetails(1)) = "" Then
                                            'nothing
                                        Else
                                            dtsPublicationDateTime2 = CDate(strPublicationDateDetails(1)) 'AGL 2013.10.25 - 8435 - changed CDateDB to CDate
                                        End If

                                    End If
                                End If

                            End If


                        End If

                    End If
                End If

            End If

            Dim intNutrientCodeSet As String = CIntDB(slParams("NutrientCodeSet")) 'JTOC 07.01.2013

            Dim strAllergens As String = CStrDB(slParams("ALLERGENS"))
            Dim strUnwantedAllergens As String = CStrDB(slParams("UnwantedAllergens"))
            Dim bWithAllergen As Boolean = CBoolDB(slParams("WithAllergen"))
            Dim bWithoutAllergen As Boolean = CBoolDB(slParams("WithoutAllergen"))
            '=================================================================================

            If strKeywords.Trim <> "" _
             Or strMarkedItems.Trim <> "" _
             Or strWantedIngredients.Trim <> "" _
             Or strUnwantedIngredients.Trim <> "" _
             Or strUnwantedKeywords.Trim <> "" _
             Or strAllergens.Trim <> "" _
             Or strUnwantedAllergens.Trim <> "" _
             Or bWithAllergen _
             Or bWithoutAllergen Then

                intSearchType = 2

            End If

            If intSource > -1 _
             Or intBrand > -1 _
             Or intSupplier > -1 _
             Or intUsedAsIngredient > 0 _
             Or intPicture > 0 _
             Or intComposition > -1 _
             Or intUsedOnline > -1 _
             Or (intPriceType > -1 And intPriceOption > -1) _
             Or intDateOption > -1 _
             Or intPublication > -1 _
             Or intPublicationDateOption > -1 Then

                intSearchType = 2

            End If

            If blnFullText Then
                intSearchType = 2
            End If

            '=================================================================================
            '-- JBB 02.03.2012

            If intcpsrcosting > 0 Then
                intSearchType = 2
            End If

            '===============================================

            '-- JBB 06.04.2012
            If intRecipeStatus <> -1 And intRecipeStatus <> 0 Then
                intSearchType = 2
            End If

            If intWebStatus <> -1 And intWebStatus <> 0 Then
                intSearchType = 2
            End If

            If strPrimaryBrand <> "" Then
                intSearchType = 2
            End If

            If strUPrimaryBrand <> "" Then
                intSearchType = 2
            End If

            If strSecondaryBrand <> "" Then
                intSearchType = 2
            End If

            If strUSecondaryBrand <> "" Then
                intSearchType = 2
            End If

            If blIsAND = True Then
                intSearchType = 2
            End If

            If intRecipeStatus = 0 Then
                intRecipeStatus = -1
            End If

            If intWebStatus = 0 Then
                intWebStatus = -1
            End If

            Dim cn As New SqlConnection(L_strCnn)
            Dim cmd As New SqlCommand
            Dim da As New SqlDataAdapter
            Dim dt As New DataTable("SearchResult")
            Dim ds As New DataSet

            Select Case intSearchType



                Case 1  'Simple Search
                    With cmd

                        .Connection = cn
                        .CommandText = "[GET_LISTE]"

                        '// DRR 05.25.2012
                        '// Unilever USA additional features:
                        '// Update the Recipe List to initially display only the recipes from the current user’s project. 
                        '// When the user searches for recipes, even the recipes that are not part of his project can be searched again.
                        If blnEnableInitialDisplay Then .CommandText = "[GET_LISTE3]"
                        '//

                        .CommandTimeout = 10000
                        .CommandType = CommandType.StoredProcedure
                        '============================================================================================
                        'Simple
                        '============================================================================================
                        .Parameters.Add("@intType", SqlDbType.Int).Value = intType
                        .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                        .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                        .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
                        .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice
                        .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
                        .Parameters.Add("@nvcName", SqlDbType.NVarChar).Value = IIf(blnFullText, strName, ReplaceSpecialCharacters(strName)) 'AGL 2013.10.23
                        .Parameters.Add("@tntNameFilter", SqlDbType.TinyInt).Value = intNameOption
                        .Parameters.Add("@nvcNumber", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(strNumber)
                        .Parameters.Add("@tntNumberFilter", SqlDbType.TinyInt).Value = intNumberOption
                        .Parameters.Add("@intCategory", SqlDbType.Int).Value = intCategory
                        .Parameters.Add("@tntFilter", SqlDbType.TinyInt).Value = intFilter
                        .Parameters.Add("@intFilterSite", SqlDbType.Int).Value = intSharedSite
                        .Parameters.Add("@intNutrientCodeSet", SqlDbType.Int).Value = intNutrientCodeSet 'JTOC 07.01.2013
                        .Parameters.Add("@isSubRecipe", SqlDbType.Bit).Value = blnIsSubrecipe 'JTOC 07.03.2013

                        If blnEnableInitialDisplay = False Then
                            .Parameters.Add("@blIsAnd", SqlDbType.Bit).Value = blIsAND ''-- JBB 06.04.2012
                            .Parameters.Add("@blnFullTranslation", SqlDbType.Bit).Value = blnFullTranslation '// DRR 06.06.2012
                            .Parameters.Add("@blnByName", SqlDbType.Bit).Value = blnByName
                        End If
                        .Parameters.Add("@CodeSet", SqlDbType.Int).Value = intCodeSet


                        '============================================================================================
                        With da
                            .SelectCommand = cmd
                            dt.BeginLoadData()
                            .Fill(dt)
                            dt.EndLoadData()
                        End With

                        ds.Tables.Add(dt)

                    End With

                Case 2  'Advanced Search

                    With cmd
                        .Connection = cn

                        .CommandText = IIf(blnFullText, "[GET_LISTEADVANCEDFULLTEXT]", "[GET_LISTEADVANCED]")

                        '// DRR 05.24.2012
                        '// Unilever USA additional features:
                        '// Update the Recipe List to initially display only the recipes from the current user’s project. 
                        '// When the user searches for recipes, even the recipes that are not part of his project can be searched again.
                        If blnEnableInitialDisplay Then .CommandText = IIf(blnFullText, "[GET_LISTEADVANCEDFULLTEXT2]", "[GET_LISTEADVANCED2]")
                        '//

                        .CommandTimeout = 10000
                        .CommandType = CommandType.StoredProcedure
                        '============================================================================================
                        'Simple
                        '============================================================================================
                        .Parameters.Add("@intType", SqlDbType.Int).Value = intType
                        .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                        .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                        .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
                        .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice
                        .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
                        .Parameters.Add("@nvcName", SqlDbType.NVarChar).Value = IIf(blnFullText, strName, ReplaceSpecialCharacters(strName)) 'AGL 2013.10.23
                        .Parameters.Add("@tntNameFilter", SqlDbType.TinyInt).Value = intNameOption
                        .Parameters.Add("@nvcNumber", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(strNumber)
                        .Parameters.Add("@tntNumberFilter", SqlDbType.TinyInt).Value = intNumberOption
                        .Parameters.Add("@intCategory", SqlDbType.Int).Value = intCategory
                        .Parameters.Add("@tntFilter", SqlDbType.TinyInt).Value = intFilter
                        .Parameters.Add("@intFilterSite", SqlDbType.Int).Value = intSharedSite
                        .Parameters.Add("@intNutrientCodeSet", SqlDbType.Int).Value = intNutrientCodeSet 'JTOC 07.01.2013
                        '============================================================================================
                        'Advanced
                        '============================================================================================
                        .Parameters.Add("@intSource", SqlDbType.Int).Value = intSource
                        .Parameters.Add("@nvcKeyword", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(strKeywords)
                        .Parameters.Add("@tntKeywordFilter", SqlDbType.TinyInt).Value = intKeywordOption
                        .Parameters.Add("@nvcMarkItemList", SqlDbType.NVarChar).Value = strMarkedItems
                        .Parameters.Add("@intBrand", SqlDbType.Int).Value = intBrand
                        .Parameters.Add("@intPublication", SqlDbType.Int).Value = intPublication 'JTOC 11.03.2013
                        .Parameters.Add("@intSupplier", SqlDbType.Int).Value = intSupplier
                        .Parameters.Add("@tntUsedAsIngredient", SqlDbType.TinyInt).Value = IIf(intUsedAsIngredient > 0, intUsedAsIngredient, 0)
                        .Parameters.Add("@nvcWantedIngredients", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(strWantedIngredients)
                        .Parameters.Add("@nvcUnwantedIngredients", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(strUnwantedIngredients)
                        .Parameters.Add("@nvcUnwantedKeyword", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(strUnwantedKeywords)
                        .Parameters.Add("@tntUnwantedKeywordFilter", SqlDbType.TinyInt).Value = intUnwantedKeywordOption
                        .Parameters.Add("@tntPicture", SqlDbType.Int).Value = intPicture
                        .Parameters.Add("@tntComposition", SqlDbType.Int).Value = intComposition
                        .Parameters.Add("@tntUsedOnline", SqlDbType.Int).Value = intUsedOnline
                        .Parameters.Add("@tntPriceType", SqlDbType.Int).Value = intPriceType
                        .Parameters.Add("@tntPriceOption", SqlDbType.Int).Value = intPriceOption
                        .Parameters.Add("@fltPrice1", SqlDbType.Float).Value = dblPrice1
                        .Parameters.Add("@fltPrice2", SqlDbType.Float).Value = dblPrice2
                        .Parameters.Add("@tntDateOption", SqlDbType.Int).Value = intDateOption
                        .Parameters.Add("@tntPublicationDateOption", SqlDbType.Int).Value = intPublicationDateOption 'JTOC 11.03.2013
                        .Parameters.Add("@nvcAllergen", SqlDbType.NVarChar).Value = strAllergens    'MKAM 2014.07.03
                        .Parameters.Add("@nvcUnwantedAllergen", SqlDbType.NVarChar).Value = strUnwantedAllergens    'MKAM 2014.07.03
                        .Parameters.Add("@blnWithoutAllergen", SqlDbType.Bit).Value = bWithoutAllergen  'MKAM 2014.07.03
                        .Parameters.Add("@blnWithAllergen", SqlDbType.Bit).Value = bWithAllergen 'MKAM 2014.07.03

                        'AGL 2012.10.30 - CWM-1944 - below-code no longer applies, status should be checked
                        'AGL Merging 2012.09.03 
                        'If blnCheckStatus = False Then
                        '    .CommandText = IIf(blnFullText, "[GET_LISTEADVANCEDFULLTEXT]", "[GET_LISTEADVANCED_CONSUMER]") '"[GET_LISTEADVANCED_CONSUMER]"
                        'End If

                        If intDateOption > -1 Then
                            .Parameters.Add("@dtsDate1", SqlDbType.DateTime).Value = dtsDateTime1
                            If intDateOption = 4 Then
                                .Parameters.Add("@dtsDate2", SqlDbType.DateTime).Value = dtsDateTime2
                            End If
                        End If

                        'JTOC 11.03.2013
                        If intPublicationDateOption > -1 Then
                            If Not dtsPublicationDateTime1 = #12:00:00 AM# Then 'AGL 2014.10.03 - added checking for blank date
                                .Parameters.Add("@dtsPublicationDate1", SqlDbType.DateTime).Value = dtsPublicationDateTime1
                            Else
                                intPublicationDateOption = -1
                                .Parameters("@tntPublicationDateOption").Value = intPublicationDateOption
                            End If
                            If intPublicationDateOption = 4 Then
                                If Not dtsPublicationDateTime2 = #12:00:00 AM# Then 'AGL 2014.10.03 - added checking for blank date
                                    .Parameters.Add("@dtsPublicationDate2", SqlDbType.DateTime).Value = dtsPublicationDateTime2
                                Else
                                    intPublicationDateOption = -1
                                    .Parameters("@tntPublicationDateOption").Value = intPublicationDateOption
                                End If
                            End If
                        End If

                        .Parameters.Add("@intCPSROperation", SqlDbType.Int).Value = intcpsroperation
                        .Parameters.Add("@intCPSRCosting", SqlDbType.Int).Value = intcpsrcosting
                        .Parameters.Add("@intCPSRValue1", SqlDbType.Float).Value = dblcpsrvalue1
                        .Parameters.Add("@intCPSRValue2", SqlDbType.Float).Value = dblcpsrvalue2

                        If blnEnableInitialDisplay = False Then
                            ''-- JBB 06.04.2012 - temporary comment DRR 06.05.2012
                            .Parameters.Add("@blIsAnd", SqlDbType.Bit).Value = blIsAND ''-- JBB 06.04.2012
                            .Parameters.Add("@intRecipeStatus", SqlDbType.Int).Value = intRecipeStatus
                            .Parameters.Add("@intWebStatus", SqlDbType.Int).Value = intWebStatus
                            .Parameters.Add("@sPrimaryBrand", SqlDbType.NVarChar).Value = strPrimaryBrand
                            .Parameters.Add("@intPrimaryFilter", SqlDbType.NVarChar).Value = intPrimaryBrandOption
                            .Parameters.Add("@sUnwantedPrimaryBrand", SqlDbType.NVarChar).Value = strUPrimaryBrand
                            .Parameters.Add("@intUnwantedPrimaryFilter", SqlDbType.NVarChar).Value = intUPrimaryBrandOption
                            .Parameters.Add("@blnFullTranslation", SqlDbType.Bit).Value = blnFullTranslation '// DRR 06.06.2012
                            .Parameters.Add("@blnByName", SqlDbType.Bit).Value = blnByName

                            'JTOC 11.19.2013
                            .Parameters.Add("@sSecondaryBrand", SqlDbType.NVarChar).Value = strSecondaryBrand
                            .Parameters.Add("@intSecondaryFilter", SqlDbType.NVarChar).Value = intSecondaryBrandOption
                            .Parameters.Add("@sUnwantedSecondaryBrand", SqlDbType.NVarChar).Value = strUSecondaryBrand
                            .Parameters.Add("@intUnwantedSecondaryFilter", SqlDbType.NVarChar).Value = intUSecondaryBrandOption

                        End If

                        With da
                            .SelectCommand = cmd
                            dt.BeginLoadData()
                            .Fill(dt)
                            dt.EndLoadData()
                        End With

                        ds.Tables.Add(dt)


                    End With

            End Select

            Return ds

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
            Return Nothing
        End Try
    End Function


    Public Function Get_ListeSearchForIngr( _
    intFilterIn As Integer, _
    strSearchString As String, _
    intFilterType As Integer, _
    intCodeTrans As Integer, _
    intCodeUser As Integer, _
    intCodeSite As Integer, _
    ByRef intTotalRows As Integer, _
    Optional ByVal intCodeSetPrice As Integer = -1, _
    Optional intSort As Integer = 0, _
    Optional intPage As Integer = 0, _
    Optional intSize As Integer = 10, _
    Optional blnIsGlobalOnly As Boolean = False, _
    Optional strFilter As String = "all", _
    Optional blnMenu As Boolean = False, _
    Optional intCategory As Integer = -1) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable("SearchResult")
        Dim ds As New DataSet
        Try
            With cmd
                .CommandTimeout = 120
                .Connection = cn
                .CommandText = "GET_LISTESEARCHINGR"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@FilterIn", SqlDbType.Int).Value = intFilterIn
                .Parameters.Add("@FilterType", SqlDbType.Int).Value = intFilterType
                .Parameters.Add("@SearchString", SqlDbType.NVarChar, 250).Value = strSearchString
                .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice
                .Parameters.Add("@Sorted", SqlDbType.Int).Value = intSort
                .Parameters.Add("@Page", SqlDbType.Int).Value = intPage
                .Parameters.Add("@RecSize", SqlDbType.Int).Value = intSize
                .Parameters.Add("@bitGlobalOnly", SqlDbType.Bit).Value = blnIsGlobalOnly
                .Parameters.Add("@strFilter", SqlDbType.NVarChar).Value = strFilter
                .Parameters.Add("@bitMenu", SqlDbType.NVarChar).Value = blnMenu
                .Parameters.Add("@intCategory", SqlDbType.Int).Value = intCategory
                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
            End With
            If dt.Rows.Count > 0 Then
                intTotalRows = CInt(dt.Rows(0)("iRow").ToString())
            Else
                intTotalRows = 0
            End If
            Return dt
        Catch ex As Exception
            Return Nothing
        End Try

    End Function


    Public Function Get_ListeSearchForIngrFT(intFilterIn As Integer, intListeType As Integer, strSearchString As String, intCodeTrans As Integer, intCodeUser As Integer, intCodeSite As Integer, ByRef intTotalRows As Integer, blAllow As Boolean, Optional ByVal intCodeSetPrice As Integer = -1, Optional intSort As Integer = 0, Optional intPage As Integer = 0, Optional intSize As Integer = 10) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable("SearchResult")
        Dim ds As New DataSet
        Dim cTrans As clsLanguage = New clsLanguage(L_AppType, L_strCnn, enumEgswFetchType.DataReader)
        Dim rwTrans As DataRow = cTrans.GetOne(intCodeTrans)
        Dim strLanguage As String = "" & CStrDB(rwTrans("LangBreaker")) & ""
        Try
            With cmd
                .Connection = cn
                .CommandText = "GET_LISTESEARCHINGRFT"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@FilterIn", SqlDbType.Int).Value = intFilterIn
                .Parameters.Add("@SearchString", SqlDbType.NVarChar, 250).Value = strSearchString
                .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@Language", SqlDbType.NVarChar, 20).Value = strLanguage
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice
                .Parameters.Add("@ListeType", SqlDbType.Int).Value = intListeType
                .Parameters.Add("@AllowCreateUseSubRecipe", SqlDbType.Bit).Value = blAllow
                .Parameters.Add("@Sorted", SqlDbType.Int).Value = intSort
                .Parameters.Add("@Page", SqlDbType.Int).Value = intPage
                .Parameters.Add("@RecSize", SqlDbType.Int).Value = intSize
                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
            End With
            If dt.Rows.Count > 0 Then
                intTotalRows = CInt(dt.Rows(0)("iRow").ToString())
            Else
                intTotalRows = 0
            End If
            Return dt
        Catch ex As Exception
            Return Nothing
        End Try

    End Function


    Public Function Get_ListeFilename(intCode As Integer) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable("SearchResult")
        Dim ds As New DataSet
        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_EgswGetListeFileName"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
            End With
            'If dt.Rows.Count > 0 Then
            '	intTotalRows = CInt(dt.Rows(0)("iRow").ToString())
            'Else
            '	intTotalRows = 0
            'End If
            Return dt
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Public Function IsFullTextEnabled() As Boolean
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim flagEnable As Boolean = False
        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_TOOL_FULLTEXT"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@IsEnable", SqlDbType.Bit).Direction = ParameterDirection.Output
                cn.Open()
                .ExecuteNonQuery()
                flagEnable = CBool(.Parameters("@IsEnable").Value)
                cn.Close()
                Return flagEnable
            End With
        Catch ex As Exception
            Return False
        End Try

    End Function

    'DLS
    Public Function fctSearchTextOKForFullText(ByVal strWord As String) As Boolean
        Dim flagOK As Boolean = True

        If strWord.Contains("0") Or strWord.Contains("1") Or _
                           strWord.Contains("2") Or strWord.Contains("3") Or _
                           strWord.Contains("3") Or strWord.Contains("4") Or _
                           strWord.Contains("5") Or strWord.Contains("6") Or _
                           strWord.Contains("7") Or strWord.Contains("8") Or strWord.Contains("9") Or strWord.Contains("|") Or strWord.Contains("?") Or strWord.Contains(",") Then
            flagOK = False
        ElseIf strWord.Contains(" ") Then

            Dim strX() As String = Split(strWord, " ")

            For i As Integer = 0 To UBound(strX)
                If strX(i).ToUpper = "AND" Or strX(i).ToUpper = "OR" Then
                    flagOK = False
                    Exit For
                End If
            Next
        End If
        Return flagOK
    End Function

    Public Function GetListeSearchResult2(ByVal udtUser As structUser, ByVal slParams As SortedList, _
                                          ByVal intCodeTrans As Integer, ByVal intPagenumber As Integer, _
                                          ByVal intPageSize As Integer, ByRef intTotalRows As Integer, _
                                          Optional ByVal blnAllowCreateUseSubRecipe As Boolean = False, _
                                          Optional ByVal blnFTSEnable As Boolean = False, _
                                          Optional ByVal strSort As String = "", _
                                          Optional ByVal intCodeSetPrice As Integer = -1) As DataTable

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable("EGSWLISTE")
        Dim sbSQL As New StringBuilder


        '---------------   Search List
        Dim strNumber As String = CStr(slParams("NUMBER"))
        Dim strWord As String = CStr(slParams("WORD"))
        Dim strCategory As String = CStr(slParams("CATEGORY"))
        Dim strBrand As String = CStr(slParams("BRAND"))
        Dim strSupplier As String = CStr(slParams("SUPPLIER"))
        Dim intListeType As enumDataListType = CType(slParams("TYPE"), enumDataListType)
        Dim strIngredientsWanted As String = CStr(slParams("INGWANTED")) 'fctTransformStrSearch(CStr(slParams("INGWANTED")))
        Dim strIngredientsUnwanted As String = CStr(slParams("INGUNWANTED")) 'fctTransformStrSearch(CStr(slParams("INGUNWANTED")))
        Dim intUserLevel As enumGroupLevel = CType(slParams("USERLEVEL"), enumGroupLevel)
        Dim strCodeSiteList As String = CStr(slParams("CODESITE"))
        Dim strFilter As String = CStr(slParams("FILTER"))
        Dim intCodeUser As Integer = CInt(slParams("CODEUSER"))

        '--- Mark Items
        Dim strCodelisteList As String = ""
        If Not slParams("MARKITEMLIST") Is Nothing Then strCodelisteList = CStr(slParams("MARKITEMLIST"))

        '--- Source
        Dim strSource As String = ""
        If slParams.Contains("SOURCE") Then strSource = CStr(slParams("SOURCE"))

        '--- Keywords
        Dim strKeyword As String = CStr(slParams("KEYWORDS"))
        Dim strUnwantedKeyword As String = ""
        If Not strUnwantedKeyword Is Nothing Then strUnwantedKeyword = CStr(slParams("KEYUNWANTED"))
        Dim strKeywordType As String = CStr(slParams("KEYTYPE"))
        Dim intKeywordOption As Integer = 0
        If Not CInt(slParams("KEYWORDOPTION")) = Nothing Then intKeywordOption = CInt(slParams("KEYWORDOPTION"))
        Dim intKeywordUnwantedOption As Integer = 0
        If Not CInt(slParams("KEYWORDUNWANTEDOPTION")) = Nothing Then intKeywordUnwantedOption = CInt(slParams("KEYWORDUNWANTEDOPTION"))

        Dim strKeywordsCode As String = ""
        Dim strUnKeywordsCode As String = ""

        If strKeyword.IndexOf(",") > -1 Then
            Dim str() As String = strKeyword.Split(CChar(","))
            strKeyword = ""
            Dim i As Integer = 0
            While i < str.Length
                strKeyword += str(i).Trim
                i += 1
                If i < str.Length Then
                    strKeyword += ","
                End If
            End While
        End If

        If strKeyword <> "" Then
            strKeywordsCode = GetKeywordsListCode(strKeyword, intCodeTrans, intListeType)
        End If

        If strUnwantedKeyword <> "" Then
            strUnKeywordsCode = GetKeywordsListCode(strUnwantedKeyword, intCodeTrans, intListeType)
        End If

        '-- Price
        Dim strPrice As String = ""
        If slParams.Contains("PRICE") Then strPrice = CStr(slParams("PRICE"))
        Dim strPriceArr() As String = strPrice.Split(CChar("|"))
        Dim strPriceCol As String = "" ' store price column to search in
        If strPriceArr.Length = 2 Then
            strPriceCol = strPriceArr(0)
            strPrice = strPriceArr(1)
        End If
        If strPrice.IndexOf("-") > 0 Then
            Dim arrPrice() As String = strPrice.Split(CChar("-"))
            strPrice = " BETWEEN " & CDbl(arrPrice(0)).ToString(New System.Globalization.CultureInfo("en-US")) _
                        & " AND " & CDbl(arrPrice(1)).ToString(New System.Globalization.CultureInfo("en-US"))
        ElseIf strPrice.Trim.Length > 0 Then
            If strPrice.IndexOf(">") > -1 Then
                strPrice = ">" & CDbl(strPrice.Replace(">", "")).ToString(New System.Globalization.CultureInfo("en-US"))
            ElseIf strPrice.IndexOf("<") > -1 Then
                strPrice = "<" & CDbl(strPrice.Replace("<", "")).ToString(New System.Globalization.CultureInfo("en-US"))
            End If
        End If
        If strPrice.Length > 0 Then strPrice = strPriceCol & " " & strPrice

        '--- Date
        Dim strDate As String = ""
        If slParams.Contains("DATE") Then strDate = CStr(slParams("DATE"))
        If strDate.IndexOf("-") > 0 Then strDate = " BETWEEN '" & strDate.Replace("-", "' AND '") & "'"
        If strDate.IndexOf("=") > 0 Then strDate = strDate.Replace("=", "='") & "'"

        '--- Nutrient Rules
        Dim strNutrientRules As String = ""
        If Not strNutrientRules Is Nothing Then strNutrientRules = CStr(slParams("NUTRIENTRULES"))

        '--- Allergens
        Dim strAllergens As String = ""
        If Not strAllergens Is Nothing Then strAllergens = CStr(slParams("ALLERGENS"))

        '--- Sales
        Dim shrtSalesStatus As Short = 0 '0=show all, 1=show linked listes only, 2=show unlinked liste only
        If slParams.Contains("LINKEDSALES") Then shrtSalesStatus = CShort(slParams("LINKEDSALES"))


        '--- Search By Name
        Dim shrtNameOption As Short = 2 'contains
        If slParams.Contains("NAMEOPTION") Then shrtNameOption = CShort(slParams("NAMEOPTION"))
        'If shrtNameOption <> 0 Then strWord = fctTransformStrSearch(strWord)

        '--- Search By Number
        Dim shrtNumberOption As Short = 2 'contains
        If slParams.Contains("NUMBEROPTION") Then shrtNumberOption = CShort(slParams("NUMBEROPTION"))

        '--- Search Global
        Dim bGlobalOnly As Boolean = False
        If Not slParams("GLOBALONLY") Is Nothing Then bGlobalOnly = CBool(slParams("GLOBALONLY"))

        '--- Search By Code
        Dim blnSearchByCode As Boolean = False
        Dim intCode As Integer = -1
        If slParams.Contains("CODE") Then
            intCode = CInt(slParams.Item("CODE"))
            If intCode > 0 Then blnSearchByCode = True
            If strCodelisteList.Length > 0 Then blnSearchByCode = True
        End If


        Dim blnExcludeKeywords As Boolean = CBool(slParams.Item("EXCLUDEKEY"))
        Dim intWithNutrientInfo As Integer = CInt(slParams.Item("WITHNUTRIENT"))
        Dim intUsedUnused As Integer = CInt(slParams.Item("USEDUNUSED"))
        Dim intWithComposition As Integer = CInt(slParams.Item("WITHCOMPOSITION"))
        Dim intNutrientEnergy As Integer = CInt(slParams.Item("NUTRIENTENERGY"))

        '--- Nutrient Summary
        Dim intNutrientSummary As Integer = 0
        If Not CInt(slParams("NUTRIENTSUMMARY")) = Nothing Then intNutrientSummary = CInt(slParams("NUTRIENTSUMMARY"))

        Dim intPictureOption As Integer = CInt(slParams("PICTUREOPTION")) 'VRP 15.09.2008
        Dim intUsedOnline As Integer = CInt(slParams("USEDONLINE")) 'VRP 30.09.2008


        '--- DECLARATIONS ---
        With sbSQL
            .Append("SET NOCOUNT ON " & vbCrLf)

            'Set Variables
            .Append("DECLARE @RecsPerPage int " & vbCrLf)
            .Append("DECLARE @Page int " & vbCrLf)
            .Append("DECLARE @RecCount int " & vbCrLf)
            .Append("DECLARE @FirstRec int " & vbCrLf)
            .Append("DECLARE @LastRec int " & vbCrLf)
            .Append("DECLARE @MoreRecords int " & vbCrLf)
            .Append("SET @RecsPerPage=" & intPageSize & vbCrLf)
            .Append("SET @Page=" & intPagenumber & vbCrLf)
            .Append("SELECT @RecCount = @RecsPerPage * @Page + 1 " & vbCrLf)
            .Append("SELECT @FirstRec = (@Page - 1) * @RecsPerPage + 1 " & vbCrLf)
            .Append("SELECT @LastRec = @Page * @RecsPerPage " & vbCrLf)

            'Set CodeEgswTable Value
            .Append("DECLARE @CODETABLECATEGORY int " & vbCrLf)
            .Append("DECLARE @CODETABLEUNIT int " & vbCrLf)
            .Append("DECLARE @CODETABLELISTE int " & vbCrLf)
            .Append("SET @CODETABLECATEGORY = 19 " & vbCrLf)
            .Append("SET @CODETABLEUNIT = 135 " & vbCrLf)
            .Append("SET @CODETABLELISTE = 50 " & vbCrLf)

        End With


        '--- RECORD PAGE WITH FILTER AND SORT ---
        With sbSQL
            'Liste Page
            .Append(" ;WITH ListePage AS( " & vbCrLf)
            .Append("SELECT DISTINCT " & vbCrLf)
            .Append("CASE WHEN ISNULL(LT.Name,'') = '' THEN L.name ELSE LT.name END Name, " & vbCrLf)
            .Append("L.Code, " & vbCrLf)

            'Sort
            If strSort.ToUpper = "NAME ASC" Then
                strSort = "L.Name ASC "
            ElseIf strSort.ToUpper = "NAME DESC" Then
                strSort = "L.Name DESC "
            End If

            Dim strSort2 As String = ""
            Select Case strSort.ToLower
                Case "L.Name ASC".ToLower, "", "r.name ASC".ToLower
                    strSort2 = "(CASE WHEN LT.name IS NULL OR LEN(RTRIM(LTRIM(LT.name)))=0 THEN L.name + '_' + cast(r.code as varchar(20)) ELSE LT.Name + '_' + cast(r.code as varchar(20)) END) ASC "
                Case "L.Name DESC", "r.name DESC".ToLower : strSort2 = "(CASE WHEN LT.name IS NULL OR LEN(RTRIM(LTRIM(LT.name)))=0 THEN L.name  + '_' + cast(r.code as varchar(20)) ELSE LT.Name  + '_' + cast(r.code as varchar(20)) END)DESC "
            End Select


            .Append("DENSE_RANK() OVER(Order BY " & strSort2 & ") AS ID " & vbCrLf)

            .Append("FROM EgswListe L " & vbCrLf)
            .Append("INNER JOIN EgswSharing S ON S.Code=L.Code AND S.CodeEgswTable=@CODETABLELISTE " & vbCrLf)

            If strCodeSiteList <> udtUser.Site.Code.ToString Then
                .Append(" AND " & GetStrForSharing("S", -1, strCodeSiteList, -1))
            Else
                .Append(" AND " & GetStrForSharing("S", udtUser.Site.Group, strCodeSiteList, udtUser.Code) & vbCrLf)
            End If
            .Append("LEFT OUTER JOIN egswListeTranslation LT on L.Code=LT.CodeListe AND LT.CodeTrans IN (" & intCodeTrans & ",NULL) " & " " & vbCrLf)

            .Append("WHERE ")
            If blnSearchByCode Then
                If intCode > 0 Then
                    .Append("L.code=" & intCode & " " & vbCrLf)
                Else
                    .Append(" r.code IN " & strCodelisteList & " ")
                End If

            Else 'Type
                Select Case intListeType
                    Case enumDataListType.MenuItems
                        .Append("(L.Type IN (2,4) OR L.type=8) " & vbCrLf)
                        .Append("(AND L.[use]=1 " & vbCrLf)
                    Case enumDataListType.Ingredient
                        If blnAllowCreateUseSubRecipe Then
                            .Append("(L.Type IN (2,4) OR (L.type=8 and L.srQty>0)) ")
                        Else
                            .Append("(L.Type IN (2,4)) ")
                        End If
                        .Append(" and L.[use]=1 ")
                    Case Else
                        .Append("L.Type=" & intListeType & " ")
                End Select

                'Sort Word
                If strWord.Length > 0 Then
                    If shrtNameOption = 0 Then 'exact
                        '.Append("AND (CASE WHEN LT.name IS NULL OR LEN(RTRIM(LTRIM(LT.name)))=0 THEN L.name ELSE LT.Name END)=@nvcWord " & vbCrLf)
                        .Append("AND LT.name='" & strWord & "' OR L.Name= '" & strWord & "'" & vbCrLf)
                    ElseIf shrtNameOption = 1 Then
                        strWord = strWord & "%" ' always use like
                        '.Append("AND (CASE WHEN LT.Name IS NULL OR LEN(RTRIM(LTRIM(LT.name)))=0 THEN L.name ELSE LT.name end) like @nvcWord " & vbCrLf)
                        .Append("AND LT.name LIKE '" & strWord & "' OR L.Name LIKE '" & strWord & "'" & vbCrLf)
                    Else 'contains
                        strWord = "%" & strWord & "%" ' always use like
                        '.Append("AND (CASE WHEN LT.name IS NULL OR LEN(RTRIM(LTRIM(LT.name)))=0 THEN L.name ELSE LT.name end) like @nvcWord " & vbCrLf)
                        .Append("AND LT.name LIKE '" & strWord & "' OR L.Name LIKE '" & strWord & "'" & vbCrLf)
                    End If
                End If

                'Sort Number
                If strNumber.Length > 0 Then
                    If shrtNumberOption = 0 Then 'exact
                        .Append("AND L.Number = '" & strNumber & "' " & vbCrLf)
                    ElseIf shrtNumberOption = 1 Then
                        strNumber = strNumber & "%" ' always use like
                        .Append("AND L.Number like '" & strNumber & "' " & vbCrLf)
                    Else 'contains
                        strNumber = "%" & strNumber & "%" ' always use like
                        .Append("AND L.Number like '" & strNumber & "' " & vbCrLf)
                    End If
                End If

                'Sort Date
                If strDate.Length > 0 Then .Append(" AND L.dates= '" & strDate & "' ")

                'Sort Price
                Select Case intListeType
                    Case enumDataListType.Merchandise
                        If strPrice.Length <> 0 Then
                            .Append("AND p." & strPrice & " ")
                        End If
                    Case enumDataListType.Recipe, enumDataListType.Menu
                        If strPrice.Length <> 0 Then
                            .Append("AND pCalc." & strPrice & " ")
                        End If
                End Select

                'Sort Wanted Ingredient
                If strIngredientsWanted.Length > 0 Then
                    Dim strSQLEintCodeIng1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR L1.name LIKE ", "@nvcIngWanted", ReplaceSpecialCharacters(strIngredientsWanted), CChar(","), True)
                    Dim strSQLEintCodeIng2 As String = AddParam(cmd, SqlDbType.NVarChar, " OR LT2.name LIKE ", "@nvcIng2Wanted", ReplaceSpecialCharacters(strIngredientsWanted), CChar(","), True)

                    ' Find match in ingredients
                    .Append("AND (L1.Name like " & strSQLEintCodeIng1 & " " & vbCrLf)

                    ' find match ingredient in rnliste translation table
                    .Append("OR (LT2.Name like " & strSQLEintCodeIng2 & " " & vbCrLf)
                    .Append("AND LT2.codeTrans=" & intCodeTrans & ")) " & vbCrLf)
                End If

                'Sort Unwanted Ingredient
                If strIngredientsUnwanted.Length <> 0 Then
                    Dim strSQLEintCodeIngUw1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR name LIKE ", "@nvcIngUnWanted", ReplaceSpecialCharacters(strIngredientsUnwanted), CChar(","), True)
                    'compare it using egswliste.anme w/codetarns
                    .Append("AND L.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select code FROM egswListe WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND CodeTrans=" & intCodeTrans & " ) ) ")
                    'compare it using egswlistetransaltion.name w/codetrans
                    .Append("AND L.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select codeliste FROM egswlistetranslation WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND codetrans=" & intCodeTrans & " ) ) ")
                    'compare it using egswliste.name w/o codetrans for liste's w/o translstions
                    .Append("AND L.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select code FROM egswListe WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND Code NOT IN (SELECT codeListe FROM egswListeTranslation WHERE CodeTrans=" & intCodeTrans & " )) ) ")
                End If

                'Sort Brand
                If strBrand.Length > 0 Then
                    .Append("AND L.brand =" & strBrand & " ")
                End If

                'Sort Category
                If strCategory.Length > 0 Then
                    .Append(" AND L.category=" & strCategory & " ")
                End If

                'Sort Source
                If strSource.Length > 0 Then
                    .Append(" AND L.Source=" & strSource & " ")
                End If

                'Sort Supplier
                If strSupplier.Length > 0 Then
                    .Append("AND L.Supplier=" & strSupplier & " ")
                End If

                'Sort GlobalOnly
                If bGlobalOnly Then
                    .Append(" AND L.IsGlobal=1 ")
                End If

                'Sort Used/Unused AS Ingredients
                If intUsedUnused = 1 Then 'used
                    .Append(" AND  0 < (SELECT count(SecondCode) FROM EgsWDetails WHERE SecondCode = L.Code) ")
                ElseIf intUsedUnused = 2 Then 'unused
                    .Append(" AND  0 = (SELECT count(SecondCode) FROM EgsWDetails WHERE SecondCode = L.Code) ")
                End If

                'Sort With Ingredient On Merchandise/ With Composistion on Labels
                If intWithComposition = 1 And intListeType = enumDataListType.Merchandise Then
                    .Append(" AND ISNULL(L.Ingredients,'') <> ''  ")
                ElseIf intWithComposition = 2 And intListeType = enumDataListType.Merchandise Then
                    .Append(" AND ISNULL(L.Ingredients,'') = ''  ")
                ElseIf intWithComposition = 1 And intListeType = enumDataListType.Recipe Then
                    .Append(" AND ISNULL((select Composition FROM EgsWLabel INNER JOIN EgsWProduct ON EgsWProduct.Code = EgsWLabel.CodeProduct AND  EgsWProduct.Code = L.Code ),'') <> ''  ")
                ElseIf intWithComposition = 2 And intListeType = enumDataListType.Recipe Then
                    .Append(" AND ISNULL((select Composition FROM EgsWLabel INNER JOIN EgsWProduct ON EgsWProduct.Code = EgsWLabel.CodeProduct AND  EgsWProduct.Code = L.Code ),'') = ''  ")
                End If

                'Sort With Nutrient Info Or Without
                If intWithNutrientInfo = 1 Then
                    .Append("   AND dbo.fn_EgsWNutrientValIsBlank(L.Code)=0    ")
                ElseIf intWithNutrientInfo = 2 Then
                    .Append("   AND dbo.fn_EgsWNutrientValIsBlank(L.Code)=1    ")
                End If

                If intNutrientEnergy = 1 Then 'DLS Dec 10 2007
                    .Append("   AND (SELECT top 1 case WHEN ISNULL(N1,-1) =-1 THEN 0 ELSE N1 END FROM EgsWNutrientVal where CodeListe = L.code) > 0    ")
                ElseIf intNutrientEnergy = 2 Then
                    .Append("   AND (SELECT top 1 case WHEN ISNULL(N1,-1) =-1 THEN 0 ELSE N1 END FROM EgsWNutrientVal where CodeListe = L.code) = 0    ")
                End If

                'Sort Keywords
                If strKeywordsCode.Length > 0 Then
                    Dim strANDKeywords() As String = strKeywordsCode.Split(CChar(","))
                    Dim intX As Integer = UBound(strANDKeywords) + 1
                    If intKeywordOption = 1 And intX > 1 Then 'AND
                        .Append(" AND " & intX & "= (SELECT count(distinct(CodeKey)) FROM EgsWKeyDetails KD WHERE KD.CodeListe = L.Code AND KD.CodeKey in (" & strKeywordsCode & "))")
                    Else 'OR
                        .Append(" AND L.Code in (SELECT CodeListe FROM EgsWKeyDetails WHERE CodeKey in (" & strKeywordsCode & "))")
                    End If
                End If

                If strUnKeywordsCode.Length > 0 Then
                    Dim strANDKeywordsUnwanted() As String = strUnKeywordsCode.Split(CChar(","))
                    Dim intX As Integer = UBound(strANDKeywordsUnwanted) + 1
                    If intKeywordUnwantedOption = 1 And intX > 1 Then
                        .Append(" AND " & intX & "= (SELECT count(distinct(CodeKey)) FROM EgsWKeyDetails KD WHERE KD.CodeListe = L.Code AND KD.CodeKey not in (" & strUnKeywordsCode & "))")
                    Else
                        .Append(" AND L.Code not in (SELECT CodeListe FROM EgsWKeyDetails WHERE CodeKey in (" & strUnKeywordsCode & "))")
                    End If
                End If

                'Sort Picture Option
                Select Case intPictureOption 'VRP 15.09.2008 picture options
                    Case 1 : .Append(" AND REPLACE(LTRIM(RTRIM(PictureName)), ';','')<>'' ")
                    Case 2 : .Append(" AND REPLACE(LTRIM(RTRIM(PictureName)), ';','')='' ")
                End Select

                'Sort Used Online
                If intUsedOnline = 1 Then
                    .Append(" AND L.Online=1 ")
                ElseIf intUsedOnline = 2 Then
                    .Append(" AND L.Online=0 ")
                End If

                'Sort Nutrient Rules
                If strNutrientRules.Trim.Length > 0 Then
                    Dim cNutrientRules As clsNutrientRules = New clsNutrientRules(udtUser, enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataSet)
                    Dim arr() As String = strNutrientRules.Split(CChar(","))
                    Array.Sort(arr)

                    Dim i As Integer = 1
                    Dim intLastPosition As Integer = 0
                    Dim arr2() As String
                    While i < arr.Length
                        arr2 = arr(i).Split(CChar("-"))
                        If CInt(arr2(0)) > 0 Then
                            Dim rwTemp As DataRow = CType(cNutrientRules.GetList(CInt(arr2(1))), DataSet).Tables(1).Rows(0)
                            If intLastPosition = CInt(arr2(0)) Then
                                .Append(" OR egswNutrientVal.N" & arr2(0) & " BETWEEN " & CStr(rwTemp("minimum")) & " AND " & CStr(rwTemp("maximum")) & " ")
                            Else
                                If i = 1 Then
                                    .Append(" AND ( ")
                                Else
                                    .Append(" ) AND ( ")
                                End If

                                .Append(" egswNutrientVal.N" & arr2(0) & " BETWEEN " & CStr(rwTemp("minimum")) & " AND " & CStr(rwTemp("maximum")) & " ")
                            End If

                            If i + 1 = arr.Length Then
                                .Append(" ) ")
                            End If

                            intLastPosition = CInt(arr2(0))
                        End If
                        i += 1
                    End While
                End If

                'Sort Allergens
                If strAllergens.Length > 0 Then
                    If strAllergens.IndexOf("NOT") > -1 Then
                        .Append(" AND (a.codeAllergen " & strAllergens & " OR a.codeAllergen IS NULL) ")
                    Else
                        .Append(" AND a.codeAllergen " & strAllergens & " ")
                    End If
                End If

                'filter, this only works wen u r searching ur own site
                Select Case UCase(strFilter)
                    Case "1" '"'OWNED"
                        .Append(" AND L.CodeSite = " & strCodeSiteList & " ")
                    Case "2" '"PUBLIC"
                        .Append(" AND L.Code IN (SELECT S.Code FROM egsWSharing S WHERE S.CodeEGSWTable=50 AND S.Type not in (1,8) AND S.Code = L.Code ) ")
                    Case "3" '"PRIVATE"
                        .Append(" AND L.Code NOT IN (SELECT S.Code FROM egsWSharing S WHERE S.CodeEGSWTable=50 AND S.Type not in (1,8) AND S.Code = L.Code ) ")
                    Case "4" '"SHARED"
                        .Append(" AND L.CodeSite <> " & strCodeSiteList & " ")
                    Case "5" '"DRAFT"
                        .Append(" AND L.[use]=0 and L.submitted=0 ")
                        'Case "6" '"SYSTEM"
                        '    .Append(" AND dbo.fn_EgswIsListeOwnedBySystem(L.Code)>0 ")
                    Case "6" '"For Approval" 'DLSXXXXXX
                        .Append(" AND L.submitted=1 ")
                    Case "7" '"Approved" 'DLSXXXXXX
                        .Append(" AND L.approvalstatus=1 AND L.submitted=0 ")
                    Case "8" '"Not Approved" 'DLSXXXXXX
                        .Append(" AND L.approvalstatus=2 AND L.submitted=0 ")
                End Select

                If shrtSalesStatus = 1 Then 'linked only
                    If intListeType = enumDataListType.Merchandise Then
                        .Append(" AND L.Code IN (SELECT CodeListe FROM egswLinkFbRnPOS linkMP WHERE linkMP.TypeLink IN (0) AND linkMP.CodeListe=L.code ") 'for merchandise and product
                        .Append("       AND linkMP.CodeProduct IN (SELECT CodeProduct FROM egswLinkFbRnPOS linkPS WHERE linkPS.TypeLink IN (1) AND linkPS.CodeProduct=linkMP.codeProduct)) ") 'for product and salesitem
                    ElseIf intListeType = enumDataListType.Recipe OrElse intListeType = enumDataListType.Menu Then
                        .Append(" AND L.Code IN (SELECT CodeListe FROM egswLinkFbRnPOS linkLS WHERE linkLS.TypeLink IN (2) AND linkLS.CodeListe=L.Code )") ' for recipes/menus and salesitem
                    End If
                ElseIf shrtSalesStatus = 2 Then 'not linked only
                    If intListeType = enumDataListType.Merchandise Then
                        .Append(" AND L.Code NOT IN (SELECT CodeListe FROM egswLinkFbRnPOS linkMP WHERE linkMP.TypeLink IN (0) AND linkMP.CodeListe=L.Code ") 'for merchandise and product
                        .Append("       AND linkMP.CodeProduct IN (SELECT CodeProduct FROM egswLinkFbRnPOS linkPS WHERE linkPS.TypeLink IN (1) AND linkPS.CodeProduct=linkMP.codeProduct)) ") 'for product and salesitem
                    ElseIf intListeType = enumDataListType.Recipe OrElse intListeType = enumDataListType.Menu Then
                        .Append(" AND L.Code NOT IN (SELECT CodeListe FROM egswLinkFbRnPOS WHERE TypeLink IN (2) AND CodeListe=L.code )") ' for recipes/menus
                    End If
                End If

                .Append(") " & vbCrLf & vbCrLf)
            End If
        End With

        '--- LIST SELECT ---
        With sbSQL
            .Append("SELECT DISTINCT " & vbCrLf)
            .Append("TR.ID, L.Code, REPLACE(L.Number, CHAR(1),'') AS Number, " & vbCrLf)
            .Append("CASE WHEN LT.Name IS NULL OR LEN(RTRIM(LTRIM(LT.Name)))=0 THEN L.Name ELSE LT.Name end Name, " & vbCrLf)
            .Append("ISNULL(LT.CodeTrans, L.CodeTrans) AS CodeTrans, L.Type, " & vbCrLf)
            .Append("L.Protected, L.Preparation, L.PictureName, L.Submitted, " & vbCrLf)
            .Append("L.Yield, Y.Code AS YieldCode, L.[Percent], Y.Format AS YieldFormat, ISNULL (YT.Name, Y.NameDef) AS YieldName, " & vbCrLf)
            .Append("ISNULL(pCalc.Coeff,0) AS Coeff, ISNULL(pCalc.CalcPrice, 0) AS CalcPrice, " & vbCrLf)
            .Append("L.Category, C.Name AS CategoryName, " & vbCrLf)
            .Append("L.Supplier, Sup.NameRef AS SupplierName, " & vbCrLf)
            .Append("L.Source, Src.Name AS SourceName, " & vbCrLf)
            .Append("L.Remark, L.Note, L.Dates, " & vbCrLf)
            .Append("L.Wastage1, L.Wastage2, L.Wastage3, L.Wastage4, " & vbCrLf)
            .Append("(1-((1-L.Wastage1/100.0) *(1-L.Wastage2/100.0) * (1-L.Wastage3/100.0) * (1-L.Wastage4/100.0))) * 100.0 as TotalWastage,  " & vbCrLf)
            .Append("L.SRUnit, ISNULL(SRUT.Name,SRU.NameDef) as SRUnitName, " & vbCrLf)
            .Append("ISNULL(pCalc.Coeff,0) AS Coeff1, ISNULL(pCalc.ImposedPrice,0) AS ImposedSellingPrice, " & vbCrLf)
            .Append("ISNULL(Prod.Code, 0) AS CodeFG, " & vbCrLf)
            .Append("L.CodeSite AS SOwner, ISNULL(TR.Code,0) AS IsOwner, 0 AS IsSystemOwned, " & vbCrLf)
            .Append("CASE L.[Use] WHEN 0 THEN 1 ELSE 0 END IsDraft, CASE WHEN L.[Use]=1 AND L.IsGlobal=1 THEN 1 ELSE 0 END AS IsGlobal, " & vbCrLf)
            .Append("N.N1, N.N2, N.N3, N.N4, N.N5, N.N6, N.N7, N.N8, N.N9, N.N10, N.N11, N.N12, N.N13, N.N14, N.N15, " & vbCrLf)
            .Append("N.N16, N.N17, N.N18, N.N19, N.N20, N.N21, N.N22, N.N23 N.N24, N.N25, N.N26, N.N27, N.N28, N.N29, N.N30, " & vbCrLf) 'ADR 04.27.11
            .Append("N.N31, N.N32, N.N33, N.N34, " & vbCrLf) 'ADR 04.27.11
            .Append("L.CodeSite, Site.Name AS SiteName, ISNULL(L.CodeUser,0) AS CodeUser, " & vbCrLf)
            .Append("ISNULL(L.[Use],0) AS ListeUse, " & vbCrLf)
            .Append("ISNULL(L.ApprovalStatus,0) AS ApprovalStatus, " & vbCrLf)
            .Append("dbo.fn_EgswGetSetPriceData(L.Code,3,3) as SetPriceData, " & vbCrLf)
            .Append("dbo.fn_EgswGetSetPrice(L.Code,3,3, '') as SetPriceValue, " & vbCrLf)
            .Append("(SELECT COUNT(*) FROM ListePage) as iRow, " & vbCrLf)
            .Append("(SELECT COUNT(*) FROM ListePage WHERE ID>@LastRec) AS MoreRecords " & vbCrLf & vbCrLf)

            .Append("FROM EgswListe L " & vbCrLf)
            .Append("INNER JOIN ListePage TR ON L.Code=TR.Code " & vbCrLf)
            .Append("INNER JOIN EgswSite Site ON L.CodeSite=Site.Code " & vbCrLf)
            .Append("INNER JOIN EgswCategory C ON L.Category=C.Code " & vbCrLf)
            .Append("INNER JOIN EgswSupplier Sup ON L.Supplier=Sup.Code " & vbCrLf)

            .Append("LEFT OUTER JOIN EgswListeTranslation LT ON L.Code=LT.CodeListe AND LT.CodeTrans IN (3, NULL) " & vbCrLf)
            .Append("LEFT OUTER JOIN EgswItemTranslation CT ON C.Code=CT.Code AND CT.CodeEgswTable=19 AND CT.CodeTrans IN (" & intCodeTrans & ", NULL) AND RTRIM(CT.Name)<>'' " & vbCrLf)
            .Append("LEFT OUTER JOIN EgswNutrientVal N ON TR.Code=N.CodeListe " & vbCrLf)
            .Append("LEFT OUTER JOIN EgswProduct Prod ON L.Code=Prod.RecipeLinkCode " & vbCrLf)
            .Append("LEFT OUTER JOIN EgswSource Src ON L.Source=Src.Code " & vbCrLf)

            If intCodeSetPrice <> -1 Then
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on L.code=pCalc.codeliste and pCalc.codesetprice=" & intCodeSetPrice & " " & vbCrLf)
            Else
                .Append("LEFT OUTER JOIN egswListeSetPriceCalc pCalc on L.code=pCalc.codeliste " & vbCrLf)
            End If

            .Append("LEFT OUTER JOIN EgswUnit Y ON L.YieldUnit=Y.Code " & vbCrLf)
            .Append("LEFT OUTER JOIN EgswItemTranslation YT ON Y.Code=YT.Code AND YT.CodeEgswTable=135 AND YT.CodeTrans IN (" & intCodeTrans & ", NULL) AND RTRIM(YT.Name)<>'' " & vbCrLf)
            .Append("LEFT OUTER JOIN EgswUnit SRU ON L.SRUnit=SRU.Code " & vbCrLf)
            .Append("LEFT OUTER JOIN EgswItemTranslation SRUT ON SRU.Code=SRUT.Code AND SRUT.CodeEgswTable=135 AND SRUT.CodeTrans IN (" & intCodeTrans & ", NULL) AND RTRIM(SRUT.Name)<>'' " & vbCrLf)

            .Append("WHERE TR.ID BETWEEN @FirstRec AND @LastRec " & vbCrLf)
            .Append("ORDER BY TR.ID " & vbCrLf)

        End With

        Try
            With cmd
                .Connection = cn
                .CommandText = sbSQL.ToString
                .CommandType = CommandType.Text

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With

                intTotalRows = 0
                If dt.Rows.Count > 0 Then intTotalRows = CInt(dt.Rows.Item(0).Item("iRow"))
            End With
            Return dt
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Public Function GetListeSearchIngredients(ByVal udtUser As structUser, _
                                              ByVal intRecsPerPage As Integer, ByVal intPageNumber As Integer, _
                                              ByVal enumListeType As enumDataListType, _
                                              ByVal strWord As String, ByVal intCategory As Integer, _
                                              ByVal strSortBy As String, _
                                              ByVal strWordType As String) As DataTable 'VRP 13.03.2008

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        With cmd
            .Connection = cn
            .CommandText = "sp_EgswGetListeSearch"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@RecsPerPage", SqlDbType.Int).Value = intRecsPerPage
            .Parameters.Add("@Page", SqlDbType.Int).Value = intPageNumber
            .Parameters.Add("@nvcWord", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(strWord)
            .Parameters.Add("@nvcWordType", SqlDbType.NVarChar, 1000).Value = ReplaceSpecialCharacters(strWordType)
            .Parameters.Add("@intlisteType", SqlDbType.Int).Value = enumListeType
            .Parameters.Add("@intCategory", SqlDbType.Int).Value = intCategory
            .Parameters.Add("@nvcSortBy", SqlDbType.NVarChar, 1000).Value = ReplaceSpecialCharacters(strSortBy)
            .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = udtUser.LastSetPrice
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = udtUser.CodeTrans
            .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = udtUser.Code

            cn.Open()
            With da
                .SelectCommand = cmd
                dt.BeginLoadData()
                .Fill(dt)
                dt.EndLoadData()
            End With
            cn.Close()
            cn.Dispose()
        End With
        Return dt
    End Function

    Public Sub GetListeIngredientPrice(ByVal intCodeListe As Integer, ByVal intCodeSetprice As Integer, ByVal intUnit As Integer, _
       ByRef dblPrice As Double, ByRef intPriceUnitCode As Integer, ByRef dblPriceFactor As Double, ByRef strPriceName As String, ByRef intPriceID As Integer, ByVal intCodeTrans As Integer, ByRef strSymbole As String, ByRef strSetPriceName As String, ByRef dblImposedPrice As Double, ByRef dblApprovedPrice As Double)
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        With cmd
            .Connection = cn
            .CommandText = "sp_egswListeIngredientGetPrice"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetprice
            .Parameters.Add("@intUnitCode", SqlDbType.Int).Value = intUnit
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
            .Parameters.Add("@fltPrice", SqlDbType.Float).Direction = ParameterDirection.Output
            .Parameters.Add("@intPriceUnitCode", SqlDbType.Int).Direction = ParameterDirection.Output
            .Parameters.Add("@fltPriceUnitCodeFactor", SqlDbType.Float).Direction = ParameterDirection.Output
            .Parameters.Add("@nvcPriceUnitName", SqlDbType.NVarChar, 30).Direction = ParameterDirection.Output
            .Parameters.Add("@intPriceID", SqlDbType.Int).Direction = ParameterDirection.Output
            .Parameters.Add("@nvcSymbole", SqlDbType.NVarChar, 5).Direction = ParameterDirection.Output
            .Parameters.Add("@nvcSetOfPrice", SqlDbType.NVarChar, 150).Direction = ParameterDirection.Output
            .Parameters.Add("@fltImposedPrice", SqlDbType.Float).Direction = ParameterDirection.Output
            .Parameters.Add("@fltApprovedPrice", SqlDbType.Float).Direction = ParameterDirection.Output

            cn.Open()
            .ExecuteNonQuery()
            dblPrice = CDbl(.Parameters("@fltPrice").Value)
            intPriceUnitCode = CInt(.Parameters("@intPriceUnitCode").Value)
            dblPriceFactor = CDbl(.Parameters("@fltPriceUnitCodeFactor").Value)
            strPriceName = CStr(.Parameters("@nvcPriceUnitName").Value)
            intPriceID = CInt(.Parameters("@intPriceID").Value)

            strSymbole = CStr(.Parameters("@nvcSymbole").Value)
            strSetPriceName = CStr(.Parameters("@nvcSetOfPrice").Value)
            dblImposedPrice = CDbl(.Parameters("@fltImposedPrice").Value)
            dblApprovedPrice = CDbl(.Parameters("@fltApprovedPrice").Value)
            cn.Close()
            cn.Dispose()
        End With
    End Sub

    Public Sub GetListeItemCostOfGoods(ByVal intCodeListe As Integer, ByVal intCodeSetprice As Integer, ByRef dblPrice As Double, ByRef intPriceunitCode As Integer, ByRef dblPriceFactor As Double, ByRef strPriceName As String, ByRef intPriceID As Integer, ByVal intCodeTrans As Integer, ByRef strSymbole As String, ByRef strSetPriceName As String, ByRef dblImposedPrice As Double, ByRef dblApprovedPrice As Double, Optional ByVal blnUseImposedPrice As Boolean = False)
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        With cmd
            .Connection = cn
            .CommandText = "sp_EgswListeGetCostOfGoods"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetprice
            .Parameters.Add("@fltPrice", SqlDbType.Float).Direction = ParameterDirection.Output
            .Parameters.Add("@intPriceUnitCode", SqlDbType.Int).Direction = ParameterDirection.Output
            .Parameters.Add("@fltPriceUnitCodeFactor", SqlDbType.Float).Direction = ParameterDirection.Output
            .Parameters.Add("@nvcPriceUnitName", SqlDbType.NVarChar, 30).Direction = ParameterDirection.Output
            .Parameters.Add("@intPriceID", SqlDbType.Int).Direction = ParameterDirection.Output
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
            .Parameters.Add("@nvcSymbole", SqlDbType.NVarChar, 5).Direction = ParameterDirection.Output
            .Parameters.Add("@nvcSetOfPrice", SqlDbType.NVarChar, 150).Direction = ParameterDirection.Output
            .Parameters.Add("@fltImposedPrice", SqlDbType.Float).Direction = ParameterDirection.Output
            .Parameters.Add("@fltApprovedPrice", SqlDbType.Float).Direction = ParameterDirection.Output
            .Parameters.Add("@bitUseImposedPrice", SqlDbType.Bit).Value = blnUseImposedPrice

            cn.Open()
            .ExecuteNonQuery()
            dblPrice = CDbl(.Parameters("@fltPrice").Value)
            intPriceunitCode = CInt(.Parameters("@intPriceUnitCode").Value)
            dblPriceFactor = CDbl(.Parameters("@fltPriceUnitCodeFactor").Value)
            strPriceName = CStr(.Parameters("@nvcPriceUnitName").Value)
            intPriceID = CInt(.Parameters("@intPriceID").Value)
            strSymbole = CStr(.Parameters("@nvcSymbole").Value)
            strSetPriceName = CStr(.Parameters("@nvcSetOfPrice").Value)
            dblImposedPrice = CDbl(.Parameters("@fltImposedPrice").Value)
            dblApprovedPrice = CDbl(.Parameters("@fltApprovedPrice").Value)
            cn.Close()
            cn.Dispose()
        End With
    End Sub

    Public Function GetMenuCardItems(ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer) As DataTable
        L_bytFetchTypeTemp = L_bytFetchType
        L_bytFetchType = enumEgswFetchType.DataTable
        Dim dt As DataTable = CType(GetMenuItems(intCodeListe, intCodeTrans), DataTable)
        L_bytFetchType = L_bytFetchTypeTemp

        With dt.Columns
            .Add("results")
            .Add("costpercentage")
            .Add("salePercentage")
        End With
        Return dt
    End Function

    Public Function GetComments(ByVal intCodeListe As Integer) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            .CommandText = "sp_EgswGetComment"
            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe

        End With

        Try
            Return ExecuteFetchType(enumEgswFetchType.DataTable, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetInstruction(ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            .CommandText = "sp_EgswGetInstruction"
            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans

        End With

        Try
            Return ExecuteFetchType(enumEgswFetchType.DataTable, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetMenuItems(ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            .CommandText = "sp_EgswListeItemsGetInfo"
            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
        End With

        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetIngredients(ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            .CommandText = "sp_egswListeIngredientsGetInfo"
            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
        End With

        Try
            If fetchType = enumEgswFetchType.UseDefault Then
                Return ExecuteFetchType(L_bytFetchType, cmd)
            Else
                Return ExecuteFetchType(fetchType, cmd)
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    'JTOC 14.06.2013
    Public Function GetRequestApprovalList(ByVal intCodeSite As Integer, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            .CommandText = "sp_GetRequestApprovalList"
            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
        End With

        Try
            If fetchType = enumEgswFetchType.UseDefault Then
                Return ExecuteFetchType(L_bytFetchType, cmd)
            Else
                Return ExecuteFetchType(fetchType, cmd)
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetIngredientsTranslation(ByVal intCodeListe As Integer, Optional ByVal intCodeTransToExclude As Integer = -2, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            .CommandText = "sp_egswListeIngredientsGetTranslations"
            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTransToExclude
        End With

        Try
            If fetchType = enumEgswFetchType.UseDefault Then
                Return ExecuteFetchType(L_bytFetchType, cmd)
            Else
                Return ExecuteFetchType(fetchType, cmd)
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetInstructionTranslation(ByVal intCodeliste As Integer, Optional ByVal intCodeTransToExclude As Integer = -2, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            .CommandText = "sp_EgswListeInstructionGetTranslations"
            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeliste
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTransToExclude
        End With

        Try
            If fetchType = enumEgswFetchType.UseDefault Then
                Return ExecuteFetchType(L_bytFetchType, cmd)
            Else
                Return ExecuteFetchType(fetchType, cmd)
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetIngredientsSetPrice(ByVal intCodeFirst As Integer, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            .CommandText = "sp_egswListeIngredientGetSetPrice"
            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@intFirstCode", SqlDbType.Int).Value = intCodeFirst
        End With

        Try
            If fetchType = enumEgswFetchType.UseDefault Then
                Return ExecuteFetchType(L_bytFetchType, cmd)
            Else
                Return ExecuteFetchType(fetchType, cmd)
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetIngredients(ByVal dblCurRate As Double, ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer, ByVal intCodeSite As Integer, ByVal blnUseBestUnit As Boolean, ByVal intCodeSetPrice As Integer, Optional ByVal dblYield As Double = -1, _
     Optional ByVal blnUseProduct As Boolean = False, _
     Optional ByVal intCodeSiteOfViewer As Integer = -1, Optional ByVal dblPercent As Double = -1, Optional ByVal intApp As Integer = 0, Optional ByVal intCodeSets As Integer = 0, Optional ByVal intCodeUser As Integer = 0) As Object   ', Optional ByVal intCodeUser As Integer = 0
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            If intApp = 28 Then
                .CommandText = "sp_egswListeIngredientsGetComputedUSA"
            Else
                .CommandText = "sp_egswListeIngredientsGetComputed"
            End If

            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@fltCurRate", SqlDbType.Float).Value = dblCurRate
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
            .Parameters.Add("@bitConvertBestUnit", SqlDbType.Bit).Value = blnUseBestUnit
            .Parameters.Add("@intCodeSetprice", SqlDbType.Int).Value = intCodeSetPrice
            .Parameters.Add("@bitUseProduct", SqlDbType.Bit).Value = blnUseProduct
            .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
            .Parameters.Add("@intCodeSet", SqlDbType.Int).Value = intCodeSets
            If intCodeUser > 0 Then .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser 'MKAM re-add this for V46
            If dblYield >= 0 Then .Parameters.Add("@fltYield", SqlDbType.Float).Value = dblYield
            If dblPercent >= 0 Then .Parameters.Add("@fltPercent", SqlDbType.Float).Value = dblPercent 'VRP 17.02.2009

            'MRC 07.21.08 - This is to pass the codesite of the current user viewing the recipe so that if ever 
            'the products are set to be displayed instead of the merchandise, the procedure will not use the the 
            'intCodeSite because it is the owner of the recipe. For example, Milk Shake is a recipe of Site 1.
            'Milk Shake has a Merchandise named Milk which is also created by Site 1 and is shared to Sites 2 and 3.
            'Milk is linked to 4 different products, 2 products from Site 2 (Carnation[default],Alaska), 
            'and 2 products from Site 3 (Anlene[default],Birch Tree). If ever
            'Site 2 is configured to view the recipe ingredients using Products instead of merchandise, Site 2 should
            'be able to see Carnation instead of milk. But if Site 1 tries to view Milk Shake, Site 1 should only            
            'display Milk because Site 1 did not link a product the the merchandise. Using another parameter as
            'the codesite of the viewer determines which ingredient name should be displayed because intCodeSite
            'parameter alone does not assure that it is also the codesite of the user currently viewing the recipe,
            'it might be the owner or the one who created the recipe.
            .Parameters.Add("@intCodeSiteOfViewer", SqlDbType.Int).Value = intCodeSiteOfViewer
        End With

        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function


    Public Function GetIngredients_UI(ByVal dblCurRate As Double, ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer, ByVal intCodeSite As Integer, ByVal blnUseBestUnit As Boolean, ByVal intCodeSetPrice As Integer, ByVal dblYield As Double, ByVal dblPercent As Double, _
 Optional ByVal blnUseProduct As Boolean = False, _
 Optional ByVal intCodeSiteOfViewer As Integer = -1, Optional ByVal intApp As Integer = 28, Optional ByVal intCodeSets As Integer = 0) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            If intApp = 28 Then
                .CommandText = "sp_egswListeIngredientsGetComputedUSA"
            Else
                .CommandText = "sp_egswListeIngredientsGetComputed"
            End If

            .CommandText = "sp_egswListeIngredientsGetComputed_UI"
            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@fltCurRate", SqlDbType.Float).Value = dblCurRate
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
            .Parameters.Add("@bitConvertBestUnit", SqlDbType.Bit).Value = blnUseBestUnit
            .Parameters.Add("@intCodeSetprice", SqlDbType.Int).Value = intCodeSetPrice
            .Parameters.Add("@bitUseProduct", SqlDbType.Bit).Value = blnUseProduct
            .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
            .Parameters.Add("@intCodeSet", SqlDbType.Int).Value = intCodeSets
            If dblYield >= 0 Then .Parameters.Add("@fltYield", SqlDbType.Float).Value = dblYield
            If dblPercent >= 0 Then .Parameters.Add("@fltPercent", SqlDbType.Float).Value = dblPercent 'VRP 17.02.2009

            'MRC 07.21.08 - This is to pass the codesite of the current user viewing the recipe so that if ever 
            'the products are set to be displayed instead of the merchandise, the procedure will not use the the 
            'intCodeSite because it is the owner of the recipe. For example, Milk Shake is a recipe of Site 1.
            'Milk Shake has a Merchandise named Milk which is also created by Site 1 and is shared to Sites 2 and 3.
            'Milk is linked to 4 different products, 2 products from Site 2 (Carnation[default],Alaska), 
            'and 2 products from Site 3 (Anlene[default],Birch Tree). If ever
            'Site 2 is configured to view the recipe ingredients using Products instead of merchandise, Site 2 should
            'be able to see Carnation instead of milk. But if Site 1 tries to view Milk Shake, Site 1 should only            
            'display Milk because Site 1 did not link a product the the merchandise. Using another parameter as
            'the codesite of the viewer determines which ingredient name should be displayed because intCodeSite
            'parameter alone does not assure that it is also the codesite of the user currently viewing the recipe,
            'it might be the owner or the one who created the recipe.
            .Parameters.Add("@intCodeSiteOfViewer", SqlDbType.Int).Value = intCodeSiteOfViewer
        End With

        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetIngredientsShopping(ByVal dblCurRate As Double, ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer, ByVal intCodeSite As Integer, ByVal blnUseBestUnit As Boolean, ByVal intCodeSetPrice As Integer, Optional ByVal dblYield As Double = -1,
                                    Optional ByVal isProduct As Boolean = False, Optional withProductLinking As Boolean = False) As Object
        'DLS May312007
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            If withProductLinking Then
                .CommandText = "[sp_EgswListeIngredientsGetComputedShopping_MSC]"
            Else
                .CommandText = "[sp_EgswListeIngredientsGetComputedShopping]"
            End If
            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@fltCurRate", SqlDbType.Float).Value = dblCurRate
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
            .Parameters.Add("@bitConvertBestUnit", SqlDbType.Bit).Value = blnUseBestUnit
            .Parameters.Add("@intCodeSetprice", SqlDbType.Int).Value = intCodeSetPrice
            .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
            If withProductLinking Then
                .Parameters.Add("@useProduct", SqlDbType.Bit).Value = isProduct
            End If
            If dblYield >= 0 Then .Parameters.Add("@fltYield", SqlDbType.Float).Value = dblYield
        End With

        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetMetricImperial(ByVal intCodeUnit As Integer) As Integer
        Try
            If L_strCnn = "" Or L_strCnn Is Nothing Then Return ""

            Dim arrParam(1) As SqlParameter
            arrParam(0) = New SqlParameter("@intUnitCode", intCodeUnit)
            arrParam(1) = New SqlParameter("@intMetImp", SqlDbType.Int)

            arrParam(1).Direction = ParameterDirection.Output

            'clsConfig.ExecuteNonQuery(GetConnection("dsn"), CommandType.StoredProcedure, "sp_egswGetEgswConfig", arrParam)
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswMetricOrImperial", arrParam)

            'If arrParam(1).Value.ToString.Trim.Length = 0 Then
            '	Return strReturnDefaultString
            'Else
            '	' Convert true/false
            '	Select Case arrParam(3).Value.ToString.ToUpper
            '		Case "!B=1"
            '			Return CStr(True)
            '		Case "!B=0"
            '			Return CStr(False)
            '	End Select

            '	If enumNumero = enumNumeros.TCPOSExportTime Or enumNumero = enumNumeros.TCPOSExportLastExportedDate Then
            '		Return CStr(arrParam(3).Value)
            '	End If


            '	' handle float values
            '	Dim tmpCulture As CultureInfo = Thread.CurrentThread.CurrentCulture
            '	Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US")
            '	If IsNumeric(arrParam(3).Value) AndAlso CDbl(arrParam(3).Value) <> 0 Then
            '		Dim dbl As Double = CDbl(arrParam(3).Value)
            '		Thread.CurrentThread.CurrentCulture = tmpCulture
            '		GetConfig = dbl.ToString()
            '		Exit Function
            '	End If
            '	Thread.CurrentThread.CurrentCulture = tmpCulture
            '	'Dim dbl As Double = Val(arrParam(3).Value)
            '	'Dim str2 As String = dbl.ToString
            '	'If IsNumeric(dbl) And dbl <> 0 Then
            '	'    'Return str2
            '	'    Return dbl.ToString(Thread.CurrentThread.CurrentCulture)
            '	'End If

            '	Return arrParam(3).Value.ToString
            'End If

            'AGL 2012.11.28
            If IsDBNull(arrParam(1).Value) Then
                Return 0
            Else
                Return arrParam(1).Value
            End If


        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetBestUnitConversion(ByRef dblValue As Double, ByRef dblValue2 As Double, ByVal intCodeSite As Integer, _
                                          ByRef intUnitCode As Integer, ByRef strName As String, ByRef strFormat As String, _
                                          ByRef dblUnitFactor As Double, ByRef intUnitTypeMain As Integer, ByVal intCodeTrans As Integer, _
                                          ByVal intUnitDisplayType As Integer) As Object

        '// DRR 04.12.2011

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        Try

            With cmd

                .Connection = cn
                .CommandText = "sp_egswGetBestUnitConversion2"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@fltValue", SqlDbType.Float).Value = dblValue
                .Parameters.Add("@fltValue2", SqlDbType.Float).Value = dblValue2
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@intUnitCode", SqlDbType.Int).Value = intUnitCode
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 30).Value = strName
                .Parameters.Add("@nvcFormat", SqlDbType.NVarChar, 15).Value = IIf(strFormat = Nothing, "", strFormat) 'AGL 2012.11.28
                .Parameters.Add("@fltUnitFactor", SqlDbType.Float).Value = dblUnitFactor
                .Parameters.Add("@intUnitTypeMain", SqlDbType.Int).Value = intUnitTypeMain
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@UnitDisplayType", SqlDbType.SmallInt).Value = intUnitDisplayType

                .Parameters("@fltValue").Direction = ParameterDirection.InputOutput
                .Parameters("@fltValue2").Direction = ParameterDirection.InputOutput
                .Parameters("@intUnitCode").Direction = ParameterDirection.InputOutput
                .Parameters("@nvcName").Direction = ParameterDirection.InputOutput
                .Parameters("@nvcFormat").Direction = ParameterDirection.InputOutput
                .Parameters("@fltUnitFactor").Direction = ParameterDirection.InputOutput
                .Parameters("@intUnitTypeMain").Direction = ParameterDirection.InputOutput

                cn.Open()
                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()

                dblValue = CDblDB(.Parameters("@fltValue").Value)
                dblValue2 = CDblDB(.Parameters("@fltValue2").Value)
                intUnitCode = CIntDB(.Parameters("@intUnitCode").Value)
                strName = CStrDB(.Parameters("@nvcName").Value)
                strFormat = CStrDB(.Parameters("@nvcFormat").Value)
                dblUnitFactor = CDblDB(.Parameters("@fltUnitFactor").Value)
                intUnitTypeMain = CIntDB(.Parameters("@intUnitTypeMain").Value)

            End With

        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()

    End Function

    'Public Function GetIngredientsReader(ByVal dblCurRate As Double, ByVal intCodeListe As Integer, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, ByVal blnUseBestUnit As Boolean, ByVal intCodeSetPrice As Integer) As SqlDataReader
    '    Dim cn As New SqlConnection(L_strCnn)
    '    Dim cmd As New SqlCommand

    '    With cmd
    '        .Connection = cn
    '        .CommandText = "sp_egswListeIngredientsGetComputed"
    '        .CommandType = CommandType.StoredProcedure

    '        .Parameters.Add("@fltCurRate", SqlDbType.Float).Value = dblCurRate
    '        .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeListe
    '        .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
    '        .Parameters.Add("@bitConvertBestUnit", SqlDbType.Bit).Value = blnUseBestUnit
    '        .Parameters.Add("@intCodeSetprice", SqlDbType.Int).Value = intCodeSetPrice
    '        .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
    '        cn.Open()
    '        m_dr = .ExecuteReader(CommandBehavior.CloseConnection)
    '        m_cn = cn
    '    End With
    '    Return m_dr
    'End Function

    Public Function GetMenuItems(ByVal dblCurRate As Double, ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer, ByVal intCodeSite As Integer, ByVal blnUseBestUnit As Boolean, ByVal intCodeSetPrice As Integer, Optional ByVal dblYield As Double = 0) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            .CommandText = "sp_EgswListeItemsGetComputed"
            .CommandType = CommandType.StoredProcedure
            .CommandTimeout = 120
            .Parameters.Add("@fltCurRate", SqlDbType.Float).Value = dblCurRate
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
            .Parameters.Add("@bitConvertBestUnit", SqlDbType.Bit).Value = blnUseBestUnit
            .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice
            .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
            .Parameters.Add("@fltYield", SqlDbType.Float).Value = dblYield 'DLS
        End With

        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListeTranslations(ByVal intCodeListe As Integer, Optional ByVal intCodeTransToExclude As Integer = -2, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object


        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            .CommandText = "sp_EgswListeTranslationGetList"
            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTransToExclude
        End With

        Try
            If fetchType = enumEgswFetchType.UseDefault Then
                Return ExecuteFetchType(L_bytFetchType, cmd)
            Else
                Return ExecuteFetchType(fetchType, cmd)
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetStepTranslation(ByVal intCodeListe As Integer, Optional ByVal intCodeTransToExclude As Integer = -2, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            .CommandText = "sp_EgswStepTranslationGetList"
            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTransToExclude
        End With

        Try
            If fetchType = enumEgswFetchType.UseDefault Then
                Return ExecuteFetchType(L_bytFetchType, cmd)
            Else
                Return ExecuteFetchType(fetchType, cmd)
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListe(ByVal intCodeListe As Integer, Optional ByVal intCodeTrans As Integer = -1) As Object
        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@intCode", intCodeListe)
        arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans)
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "sp_egswListeGet", arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListe(ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer, ByVal intIDMain As Integer) As Object
        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@intCode", intCodeListe)
        arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans)
        arrParam(2) = New SqlParameter("@intIDMain", intIDMain) 'VRP 04.01.2008
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "sp_egswListeGet", arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListe(ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer, ByVal intIDMain As Integer, ByVal intCodeSite As Integer) As Object
        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@intCode", intCodeListe)
        arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans)
        arrParam(2) = New SqlParameter("@intIDMain", intIDMain) 'VRP 04.01.2008
        arrParam(3) = New SqlParameter("@CodeSite", intCodeSite)
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "sp_egswListeGet", arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    '--- VRP 23.04.2008
    Public Function GetListeExport(ByVal intCodeListe As Integer, Optional ByVal intCodeTrans As Integer = -1) As Object
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@intCode", intCodeListe)
        arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans)
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "sp_egswListeGetExport", arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''--- VRP 04.01.2008
    'Public Function GetListeList(Optional ByVal intCodeTrans As Integer = -1, Optional ByVal intIDMain As Integer = -1) As SqlDataReader
    '    Dim intCodeListe As Integer = -1
    '    Dim arrParam(2) As SqlParameter
    '    arrParam(0) = New SqlParameter("@intCode", intCodeListe)
    '    arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans)
    '    arrParam(2) = New SqlParameter("@intIDMain", intIDMain) 'VRP 04.01.2008
    '    Try
    '        Return ExecuteReader(L_strCnn, CommandType.StoredProcedure, "sp_egswListeGet", arrParam)
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Function '----

    'MRC 08.24.09
    'AGL Merging 2012.09.04
    Public Function GetListeList(Optional ByVal intCodeTrans As Integer = -1, Optional ByVal intIDMain As Integer = -1, Optional ByVal intCodeListe As Integer = -1, Optional ByVal blnSetPrice As Boolean = True, Optional ByVal intCodeSetPrice As Integer = 0, Optional intNutrientSet As Integer = 0, Optional intCodeSite As Integer = 0) As SqlDataReader
        Dim arrParam(5) As SqlParameter
        arrParam(0) = New SqlParameter("@intCode", intCodeListe)
        arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans)
        arrParam(2) = New SqlParameter("@intIDMain", intIDMain) 'VRP 04.01.2008
        arrParam(3) = New SqlParameter("@blnSetPrice", blnSetPrice) 'JTOC 22.08.2012
        arrParam(4) = New SqlParameter("@intCodeSetPrice", intCodeSetPrice) 'JTOC 22.08.2012
        arrParam(5) = New SqlParameter("@intNutrientSet", intNutrientSet) ' RDC 01.24.2014 - Fixed merchandise displayed twice or more on the report.
        'arrParam(5) = New SqlParameter("@CodeUser", intCodeUser) '//LD20160929 Use to check Allergen Law
        arrParam(5) = New SqlParameter("@CodeSite", intCodeSite) '//LD20160929 Use to check Allergen Law


        Try
            Return ExecuteReader(L_strCnn, CommandType.StoredProcedure, "sp_egswListeGet", arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function '----


    'DLS May312007
    Public Function GetListeBasic(ByVal intCodeListe As Integer) As Object
        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@intCode", intCodeListe)
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "sp_EgswListeGetBasic", arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    'MRC 04.12.2010 - Customized for Hero
    Public Function GetListeLinkedItem(ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer) As Object
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeListe", intCodeListe)
        arrParam(1) = New SqlParameter("@CodeTrans", intCodeTrans)
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "[GET_LINKEDITEM]", arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function UpdateListeLinkedItem(ByVal intTranType As Integer, ByVal intCodeParent As Integer, ByVal intCodeListe As Integer) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataSet
        Try
            With cmd
                .Connection = cn
                .CommandText = "[UPDATE_LINKEDITEM]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@tntTranType", SqlDbType.Int).Value = intTranType
                .Parameters.Add("@CodeParent", SqlDbType.Int).Value = intCodeParent
                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                cn.Open()
                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function


    Public Function GetListeKeyword(ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer, ByVal eListeType As enumDataListItemType, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault, Optional ByVal flagShowAll As Boolean = False) As Object
        Dim arrParam(3) As SqlParameter

        arrParam(0) = New SqlParameter("@intCodeListe", intCodeListe)
        arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans)
        arrParam(2) = New SqlParameter("@intListeType", eListeType)
        arrParam(3) = New SqlParameter("@bShowAll", flagShowAll) 'DLS 18.11.2008
        Try
            If fetchType = enumEgswFetchType.UseDefault Then
                Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "sp_EgswKeyDetailsGetList", arrParam)
            Else
                Return ExecuteFetchType(fetchType, L_strCnn, CommandType.StoredProcedure, "sp_EgswKeyDetailsGetList", arrParam)
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListeKeywordsEATCH(ByVal intCodeListe As Integer) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_GetListAllergenEATCH"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeListe

                .Connection.Open()
                .ExecuteNonQuery()

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                Return dt
            End With
            cmd.Connection.Close()
            cmd.Dispose()
        Catch ex As Exception
        Finally
            cmd.Dispose()
        End Try
    End Function

    Public Function GetListeSalesHistory(ByVal intCodeListe As Integer, ByVal intCodeUser As Integer, ByVal dteDateFrom As Date, ByVal dteDateTo As Date, ByVal intCodeTrans As Integer) As Object
        Dim arrParam(4) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeListe", intCodeListe)
        arrParam(1) = New SqlParameter("@intCodeUser", intCodeUser)
        arrParam(2) = New SqlParameter("@dteDateFrom", dteDateFrom)
        arrParam(3) = New SqlParameter("@dteDateTo", dteDateTo)
        arrParam(4) = New SqlParameter("@intCodeTrans", intCodeTrans)

        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "sp_EgswListeSalesHistoryGetList", arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListeMenuCards(ByVal elistetype As enumDataListType, ByVal intCodeUser As Integer, ByVal intCodeTrans As Integer) As Object
        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeTrans", intCodeTrans)
        arrParam(1) = New SqlParameter("@intCodeUser", intCodeUser)
        arrParam(2) = New SqlParameter("@intType", elistetype)

        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "sp_EgswListeMenuCardGetByUser", arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetIngredientRecipes(ByVal intIngredientCode As Integer, ByVal intCodeTrans As Integer) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable("egswDetails")

        With cmd
            .Connection = cn
            .CommandText = "LISTE_GetRecipesByIngredient"
            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@intSecondCode", SqlDbType.Int).Value = intIngredientCode
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
        End With

        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Overloads Function GetListeSetPriceCalc(ByVal intCodeListe As Integer, ByVal dblYieldSize As Double, ByVal intCodeSetPrice As Integer) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        With cmd
            .Connection = cn
            .CommandText = "sp_EgswListeSetPriceCalcGetOneSetPrice"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intCodeliste", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@fltYieldSize", SqlDbType.Float).Value = dblYieldSize
            .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListeSetPrice(ByVal intCodeListe As Integer, ByVal intCodesetprice As Integer, ByVal intCodeTrans As Integer, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable(intCodesetprice.ToString)
        With cmd
            .Connection = cn
            .CommandText = "sp_egswListeSetPriceGetOneSetPrice"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodesetprice
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans

        End With
        Try
            If fetchType = enumEgswFetchType.UseDefault Then
                Return ExecuteFetchType(L_bytFetchType, cmd)
            Else
                Return ExecuteFetchType(fetchType, cmd)
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Overloads Function GetListeSetPriceCalc(ByVal intCodeListe As Integer, ByVal dblYieldSize As Double) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        With cmd
            .Connection = cn
            .CommandText = "sp_EgswListeSetPriceCalcGetList"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intCodeliste", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@fltYieldSize", SqlDbType.Float).Value = dblYieldSize
        End With

        With da
            .SelectCommand = cmd
            dt.BeginLoadData()
            .Fill(dt)
            dt.EndLoadData()
        End With
        cn.Close() 'test DLS

        'moved it to SP. having problems w/XML file, w/null values in column...
        'if field in all its row is null, xml, dont write it...
        'dt.Columns.Add("UpdateApprovedPrice")
        'dt.Columns.Add("UpdateImposedPrice")
        'dt.Columns.Add("UpdateCoeff")
        'dt.Columns.Add("UpdateTax")

        'Dim rw As DataRow



        'dt.Columns("UpdateApprovedPrice"). = 0
        'dt.Columns("UpdateImposedPrice").DefaultValue = 0
        'dt.Columns("UpdateCoeff").DefaultValue = 0
        'dt.Columns("UpdateTax").DefaultValue = 0
        Return dt
    End Function

    ''-- JBB 05.18.2012
    '' Added  intCodeSet As Integer, 
    ''
    Public Function GetListeNutrientList(ByVal intCodeListe As Integer, Optional ByRef strCodeLink As String = "00000", Optional ByRef strFoodDescription As String = "", Optional intCodeSet As Integer = 0, Optional ByVal blIsWithTemp As Boolean = False) As ArrayList
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        With cmd
            .Connection = cn
            .CommandText = "sp_egswNutrientValGetByListe"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intcodeliste", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intcodeSet", SqlDbType.Int).Value = intCodeSet  ''-- JBB 05.18.2012
            .Parameters.Add("@blISWithTemp", SqlDbType.Bit).Value = blIsWithTemp ''-- JBB 06.10.2012
        End With

        With da
            .SelectCommand = cmd
            dt.BeginLoadData()
            .Fill(dt)
            dt.EndLoadData()
        End With

        Dim row As DataRow
        Dim arr As New ArrayList(42) 'ADR 04.27.11 - value from 15 to 34
        Dim counter As Integer
        For Each row In dt.Rows
            strCodeLink = CStr(row.Item("codelink"))
            strFoodDescription = CStr(row.Item("Desc"))

            'ADR 04.27.11 - counetr value from 15 to 42
            For counter = 1 To 42
                If row.Table.Columns.Contains("N" & counter) AndAlso Not IsDBNull(row.Item("N" & counter)) Then
                    arr.Add(row.Item("N" & counter))
                Else
                    arr.Add(-1)
                End If
            Next

        Next


        Return arr
    End Function

    Public Function GetListeNutrientListVisibility(ByVal intCodeListe As Integer, Optional ByRef strCodeLink As String = "00000", Optional ByRef strFoodDescription As String = "", Optional intCodeSet As Integer = 0, Optional ByVal blIsWithTemp As Boolean = False) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        With cmd
            .Connection = cn
            .CommandText = "sp_egswNutrientValGetByListe"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intcodeliste", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intcodeSet", SqlDbType.Int).Value = intCodeSet  ''-- JBB 05.18.2012
            .Parameters.Add("@blISWithTemp", SqlDbType.Bit).Value = blIsWithTemp ''-- JBB 06.10.2012
        End With

        With da
            .SelectCommand = cmd
            dt.BeginLoadData()
            .Fill(dt)
            dt.EndLoadData()
        End With

        'Dim row As DataRow
        'Dim arr As New ArrayList(34) 'ADR 04.27.11 - value from 15 to 34
        'Dim counter As Integer
        'For Each row In dt.Rows
        '	strCodeLink = CStr(row.Item("codelink"))
        '	strFoodDescription = CStr(row.Item("Desc"))

        '	'ADR 04.27.11 - counetr value from 15 to 34
        '	For counter = 1 To 34
        '		If Not IsDBNull(row.Item("N" & counter)) Then
        '			arr.Add(row.Item("N" & counter))
        '		Else
        '			arr.Add(-1)
        '		End If
        '	Next

        'Next


        Return dt
    End Function

    'JTOC 31.08.2012 Get the codeset of the recipe
    Public Function GetListeNutrientCodeSet(ByVal intCodeListe As Integer) As Integer
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        With cmd
            .Connection = cn
            .CommandText = "sp_EgswCodeSetGetByListe"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intcodeliste", SqlDbType.Int).Value = intCodeListe
        End With

        With da
            .SelectCommand = cmd
            dt.BeginLoadData()
            .Fill(dt)
            dt.EndLoadData()
        End With
        Dim intCodeSet As Integer = 0
        Dim row As DataRow
        For Each row In dt.Rows
            If Not IsDBNull(row.Item("CodeSet")) Then intCodeSet = CStr(row.Item("CodeSet"))
        Next

        Return intCodeSet
    End Function

    '-- JBB Add CodeSet Default  0 
    Public Function GetListeNutrientListImposed(ByVal intCodeListe As Integer, Optional ByRef strCodeLink As String = "00000", Optional ByRef strFoodDescription As String = "", Optional ByRef intNutrientImposedType As Integer = 0, Optional ByRef strPortionSize As String = "", Optional ByRef blnDisplayNutrition As Boolean = False, Optional ByVal intCodeSite As Integer = -1, Optional ByRef strNutritionBasis As String = "", Optional ByVal intCodeSet As Integer = 0, Optional ByVal blIsWithTemp As Boolean = False) As ArrayList
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        With cmd
            .Connection = cn
            .CommandText = "sp_egswNutrientValGetByListe"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intcodeliste", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intcodeSet", SqlDbType.Int).Value = intCodeSet
            .Parameters.Add("@blISWithTemp", SqlDbType.Bit).Value = blIsWithTemp
        End With

        With da
            .SelectCommand = cmd
            dt.BeginLoadData()
            .Fill(dt)
            dt.EndLoadData()
        End With



        Dim row As DataRow
        Dim arr As New ArrayList(42)
        Dim nutinfo As New structNutrientInfo
        Dim counter As Integer
        For Each row In dt.Rows

            'Imposed Nutrients
            For counter = 1 To 42
                nutinfo.Name = "N" & counter & "Impose"

                nutinfo.Visible = CBoolDB(row.Item("N" & counter & "Display"))
                '--

                If Not IsDBNull(row.Item("N" & counter & "Impose")) Then
                    nutinfo.Value = row.Item("N" & counter & "Impose")
                Else
                    nutinfo.Value = -1
                End If

                If Not IsDBNull(row.Item("N" & counter & "ImposePercent")) Then
                    nutinfo.Percent = row.Item("N" & counter & "ImposePercent")
                Else
                    nutinfo.Percent = -1
                End If
                '-- JBB 07.02.2012
                nutinfo.Position = counter
                arr.Add(nutinfo)
            Next
        Next


        If Not dt Is Nothing Then
            If dt.Rows.Count > 0 Then
                intNutrientImposedType = CIntDB(dt.Rows(0)("ImposedType"))
                strPortionSize = CStrDB(dt.Rows(0).Item("PortionSize")) '// DRR 2.17.2011
                blnDisplayNutrition = CBoolDB(dt.Rows(0).Item("DisplayNutrition")) '// DRR 3.4.2011
                strNutritionBasis = CStrDB(dt.Rows(0)("NutritionBasis"))  '-- JBB 01.17.2012
            End If
        End If

        Return arr
    End Function

    ''-- JBB 05.18.2012 Add intCodeSet (NutrientSet Default 0)
    Public Function GetListeNutrientListImposedbySet(ByVal intCodeListe As Integer, Optional ByRef strCodeLink As String = "00000", Optional ByRef strFoodDescription As String = "", Optional ByRef intNutrientImposedType As Integer = 0, Optional ByRef strPortionSize As String = "", Optional ByRef blnDisplayNutrition As Boolean = False, Optional ByVal intCodeSite As Integer = -1, Optional ByRef strNutritionBasis As String = "", Optional ByVal intCodeSet As Integer = 0, Optional ByVal blIsWithTemp As Boolean = False) As ArrayList
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        With cmd
            .Connection = cn
            .CommandText = "sp_egswNutrientValGetByListe"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intcodeliste", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intcodeSet", SqlDbType.Int).Value = intCodeSet
            .Parameters.Add("@blISWithTemp", SqlDbType.Bit).Value = blIsWithTemp
        End With

        With da
            .SelectCommand = cmd
            dt.BeginLoadData()
            .Fill(dt)
            dt.EndLoadData()
        End With



        Dim row As DataRow
        Dim arr As New ArrayList(42)
        Dim nutinfo As New structNutrientInfo
        Dim counter As Integer
        For Each row In dt.Rows

            'Imposed Nutrients
            For counter = 1 To 42
                nutinfo.Name = "N" & counter & "Impose"

                nutinfo.Visible = CBoolDB(row.Item("N" & counter & "Display"))
                '--

                If Not IsDBNull(row.Item("N" & counter & "Impose")) Then
                    nutinfo.Value = row.Item("N" & counter & "Impose")
                Else
                    nutinfo.Value = -1
                End If

                If Not IsDBNull(row.Item("N" & counter & "ImposePercent")) Then
                    nutinfo.Percent = row.Item("N" & counter & "ImposePercent")
                Else
                    nutinfo.Percent = -1
                End If

                '-- JBB 07.02.2012
                nutinfo.Position = counter
                arr.Add(nutinfo)
            Next
        Next

        If Not dt Is Nothing Then
            If dt.Rows.Count > 0 Then
                intNutrientImposedType = CIntDB(dt.Rows(0)("ImposedType"))
                strPortionSize = CStrDB(dt.Rows(0).Item("PortionSize")) '// DRR 2.17.2011
                blnDisplayNutrition = CBoolDB(dt.Rows(0).Item("DisplayNutrition")) '// DRR 3.4.2011
                strNutritionBasis = CStrDB(dt.Rows(0)("NutritionBasis"))  '-- JBB 01.17.2012
            End If
        End If

        Return arr
    End Function


    ''' <summary>
    ''' JBB 06.08.2012
    ''' 
    Public Sub DeleteListeTempNutrientValImposed(ByVal intCodeListe As String)
        Dim cn As New SqlConnection(L_strCnn)
        Try
            Dim cmd As New SqlCommand
            With cmd
                .Connection = cn
                .CommandType = CommandType.StoredProcedure
                .CommandText = "DELETE_tmpEgswNutrientVal"
                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe
                cn.Open()
                .ExecuteNonQuery()
                cn.Close()
            End With
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
    End Sub

    Public Function GetListeUnits(ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer, ByVal intCodeSite As Integer) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            .CommandText = "sp_egswListeGetUnits"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
            .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListeUnitsAPI(codeSite As Integer, codeTrans As Integer, codeSetPrice As Integer, type As Integer, codeListe As Integer) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        'exec [dbo].[API_GET_Units] @CodeSite=1,@CodeTrans=2,@CodeSetPrice=1,@Type=2,@CodeListe=132018
        With cmd
            .Connection = cn
            .CommandText = "API_GET_Units"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@CodeSite", SqlDbType.Int).Value = codeSite
            .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = codeTrans
            .Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = codeSetPrice
            .Parameters.Add("@Type", SqlDbType.Int).Value = type
            .Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeListe
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListeShared(ByVal intCodeListe As Integer, ByVal eShareType As ShareType, Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            .CommandText = "sp_EgswListeSharedGet"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@sntType", SqlDbType.SmallInt).Value = eShareType
        End With

        Try
            If fetchType = enumEgswFetchType.ArrayList Then
                Dim dr As SqlDataReader
                Try
                    dr = CType(ExecuteFetchType(enumEgswFetchType.DataReader, cmd), SqlDataReader)
                Catch ex As Exception
                    dr.Close()
                    Throw ex
                End Try

                Dim arr As ArrayList = New ArrayList
                While dr.Read
                    arr.Add(dr("CodeSite"))
                End While
                dr.Close()

                Return arr
            ElseIf fetchType = enumEgswFetchType.UseDefault Then
                Return ExecuteFetchType(L_bytFetchType, cmd)
            Else
                Return ExecuteFetchType(fetchType, cmd)
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListeCode(ByVal strNumber As String, ByVal strName As String, ByVal listetype As enumDataListItemType, ByVal intCodeTrans As Integer, ByVal blnCompareNumber As Boolean, _
          Optional ByVal intCodeProperty As Integer = -1, Optional ByVal intCodeSite As Integer = -1, Optional ByVal intCodeUser As Integer = -1, _
          Optional ByVal intCodeUnit As Integer = -1, Optional ByVal MainListeType As enumDataListItemType = 0) As Integer

        If blnCompareNumber And strNumber = "" Then Return -1

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        cmd.Connection = cn
        cmd.CommandType = CommandType.Text

        Dim strSQL As String = " SELECT l.code" _
                                + " FROM egswListe l INNER JOIN egswSharing s" _
                                + " ON l.Code=s.Code AND s.CodeEgswTable=" + CStr(enumDbaseTables.EgswListe) _
                                + " WHERE " _
                                + " ((s.Type IN (" + CStr(ShareType.CodeSite) + ") AND s.CodeUserSharedTo=" + CStr(intCodeSite) + ") " _
                                + " OR (s.Type IN (" + CStr(ShareType.CodeProperty) + ") AND s.CodeUserSharedTo=" + CStr(intCodeProperty) + ") " _
                                + " OR (s.Type IN (" + CStr(ShareType.CodeUser) + ") AND s.CodeUserSharedTo=" + CStr(intCodeUser) + ")) "

        Select Case listetype
            Case enumDataListItemType.Text
                'rdtc 02.10.2007
                'if the item being checked is a text, then it can only be compared by name
                strSQL = strSQL _
                                   + " AND l.Name=@nvcValue " _
                                   + " AND l.Type= " + CStr(listetype)
            Case enumDataListItemType.Merchandise
                If blnCompareNumber Then
                    strSQL = strSQL _
                                    + " AND l.Number=@nvcValue " _
                                    + " AND l.Type=" + CStr(listetype)
                Else
                    strSQL = strSQL _
                                    + " AND l.Name=@nvcValue " _
                                    + " AND l.Type= " + CStr(listetype)
                End If
            Case Else
                If blnCompareNumber Then
                    strSQL = strSQL _
                                        + " AND l.Number=@nvcValue " _
                                        + " AND l.Type= " + CStr(listetype)
                Else
                    strSQL = strSQL _
                                    + " AND l.Name=@nvcValue "
                End If

                '------------------------------
                'RDTC 28.09.2007
                If intCodeUnit <> -1 And MainListeType > 0 And listetype = enumDataListItemType.Recipe Then
                    'check if the ingredient is a subrecipe
                    strSQL = strSQL & _
                        "AND ( " & vbCrLf & _
                        "l.code in " & vbCrLf & _
                        "(select distinct codeliste from egswlistesetprice where unitdisplay in  " & vbCrLf & _
                        "(select code from egswunit where typemain in  " & vbCrLf & _
                        "(select typemain from egswunit where code = " & intCodeUnit & "))) " & vbCrLf

                    If MainListeType = enumDataListItemType.Menu Then
                        strSQL = strSQL & " OR l.YieldUnit = " & intCodeUnit & vbCrLf
                    End If

                    strSQL = strSQL & ")"
                End If
                '------------------------------
        End Select


        cmd.CommandText = strSQL

        If blnCompareNumber Then
            cmd.Parameters.Add("@nvcValue", SqlDbType.NVarChar, 50).Value = ReplaceSpecialCharacters(strNumber)
        Else
            cmd.Parameters.Add("@nvcValue", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(strName)
        End If


        With da
            .SelectCommand = cmd
            dt.BeginLoadData()
            .Fill(dt)
            dt.EndLoadData()
        End With

        If dt.Rows.Count = 0 Then
            If Not blnCompareNumber Then
                '// Check if item exists in translation table
                cmd = New SqlCommand
                dt = New DataTable
                With cmd
                    .Connection = cn
                    .CommandType = CommandType.Text
                    .CommandText = " SELECT DISTINCT l.code" _
                                    + " FROM egswListe l INNER JOIN egswSharing s" _
                                    + " ON l.Code=s.Code AND s.CodeEgswTable=" + CStr(enumDbaseTables.EgswListe) _
                                    + " INNER JOIN egswListeTranslation t" _
                                    + " ON t.CodeListe=l.Code AND t.name<>'' " _
                                    + " WHERE ((s.Type IN (" + CStr(ShareType.CodeSite) + ") AND s.CodeUserSharedTo=" + CStr(intCodeSite) + ") " _
                                    + " OR (s.Type IN (" + CStr(ShareType.CodeProperty) + ") AND s.CodeUserSharedTo=" + CStr(intCodeSite) + ") " _
                                    + " OR (s.Type IN (" + CStr(ShareType.CodeUser) + ") AND s.CodeUserSharedTo=" + CStr(intCodeSite) + ")) " _
                                    + " AND t.name= @nvcName" _
                                    + " AND t.CodeTrans= " + CStr(intCodeTrans)

                    .Parameters.Add("@nvcName", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(strName)
                End With

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With

                If dt.Rows.Count = 0 Then
                    Return -1
                Else
                    Return CInt(dt.Rows(0).Item("code"))
                End If
            Else
                Return -1
            End If
        Else
            Return CInt(dt.Rows(0).Item("code"))
        End If
    End Function

    Public Function GetListeCode(ByVal strName As String, ByVal listetype As enumDataListItemType, Optional ByVal intCodeProperty As Integer = -1, Optional ByVal intCodeSite As Integer = -1, Optional ByVal intCodeUser As Integer = -1) As Integer
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim value As Integer

        With cmd
            .Connection = cn
            .CommandText = " SELECT l.code" _
                            + " FROM egswListe l INNER JOIN egswSharing s" _
                            + " ON l.Code=s.Code AND s.CodeEgswTable=" + CStr(enumDbaseTables.EgswListe) _
                            + " WHERE " _
                            + " ((s.Type IN (" + CStr(ShareType.CodeSite) + ") AND s.CodeUserSharedTo=" + CStr(intCodeSite) + ") " _
                            + " OR (s.Type IN (" + CStr(ShareType.CodeProperty) + ") AND s.CodeUserSharedTo=" + CStr(intCodeProperty) + ") " _
                            + " OR (s.Type IN (" + CStr(ShareType.CodeUser) + ") AND s.CodeUserSharedTo=" + CStr(intCodeUser) + ")) " _
                            + " AND l.Name=@nvcName" _
                            + " AND l.Type= " + CStr(listetype)

            .CommandType = CommandType.Text
            .Parameters.Add("@nvcName", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(strName)
            Try
                Dim dr As SqlDataReader
                cn.Open()
                dr = .ExecuteReader(CommandBehavior.CloseConnection)
                If dr.HasRows Then
                    dr.Read()
                    value = CInt(dr.Item(0))
                Else
                    value = -1
                End If
                dr.Close()
                cn.Close()
            Catch ex As Exception
                cn.Close()
                Throw ex
            End Try

        End With

        Return value
    End Function

    Public Function GetListeNamePictureName(ByVal intCode As Integer, ByVal intCodeTrans As Integer, ByRef strName As String, ByRef strPictureName As String) As Boolean
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader

        With cmd
            .Connection = cn
            .CommandText = "Select CASE WHEN t.name IS NULL OR LEN(LTRIM(RTRIM(t.[Name])))=0 THEN r.[Name] ELSE t.[Name] END name, " & _
                            "PictureName " & _
                            "FROM	egswListe r " & _
                            "LEFT OUTER JOIN egswListeTranslation t ON r.Code=t.codeliste and t.codetrans IN (@intCodeTrans, NULL) " & _
                            "where r.code=@intCode "
            .CommandType = CommandType.Text
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
            Try
                cn.Open()
                dr = .ExecuteReader(CommandBehavior.CloseConnection)
                If dr.Read Then
                    strName = CStrDB(dr.Item("Name"))
                    strPictureName = CStrDB(dr.Item("PictureName"))
                End If
                dr.Close()
                cn.Close()
            Catch ex As Exception
                cn.Close()
                Throw ex
            End Try

        End With

        Return True
    End Function

    Public Function GetListeName(ByVal intCode As Integer, ByVal intCodeTrans As Integer) As String
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader
        Dim strName As String = ""

        With cmd
            .Connection = cn
            .CommandText = "Select CASE WHEN t.name IS NULL OR LEN(LTRIM(RTRIM(t.[Name])))=0 THEN r.[Name] ELSE t.[Name] END name " & _
                            "FROM	egswListe r " & _
                            "LEFT OUTER JOIN egswListeTranslation t ON r.Code=t.codeliste and t.codetrans IN (@intCodeTrans, NULL) " & _
                            "where r.code=@intCode "
            .CommandType = CommandType.Text
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
            Try
                cn.Open()
                dr = .ExecuteReader(CommandBehavior.CloseConnection)
                If dr.Read Then
                    strName = CStrDB(dr.Item("Name"))
                End If
                dr.Close()
                cn.Close()
            Catch ex As Exception
                cn.Close()
                Throw ex
            End Try
        End With
        Return strName
    End Function

    Public Function GetCodeListe(ByVal strName As String) As Integer
        Dim cmd As New SqlCommand
        Dim intCodeliste As Integer = -1
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswGetCodeListe"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@strName", SqlDbType.NVarChar, 30).Value = strName
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                .ExecuteNonQuery()
                cmd.Connection.Close()
                intCodeliste = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()

        Return intCodeliste
    End Function

    Public Function GetListeNutrientPortionSize(intCode As Integer, intCodeTrans As Integer, intCodeSet As Integer) As String
        'select portionsize from EgswNutrientVal where isnull(PortionSize, '') <> '' and CodeListe =   2 and CodeSet = 
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim strResult As String = ""
        With cmd
            .Connection = cn
            .CommandText = "GetNutrientPortionSize" ''"select isnull(PortionSize, '') Portionsize from EgswNutrientVal where CodeListe = " + intCode.ToString() + " and CodeSet = " + intCodeSet.ToString()
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@CodeListe", SqlDbType.Int, 30).Value = intCode
            .Parameters.Add("@CodeTrans", SqlDbType.Int, 30).Value = intCodeTrans
            .Parameters.Add("@CodeSet", SqlDbType.Int, 30).Value = intCodeSet
            Try
                cn.Open()
                strResult = .ExecuteScalar().ToString()
                cn.Close()
            Catch ex As Exception
                cn.Close()
            End Try
        End With
        Return strResult
    End Function

    Public Function GetListeYieldAndUnit(ByVal intCode As Integer, ByVal intCodeTrans As Integer) As String
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader
        Dim strYield As String = ""
        Dim strPercent As String = ""
        Dim strYieldUnit As String = ""
        Dim strYieldPercent As String = ""

        With cmd
            .Connection = cn
            .CommandText = "Select Yield, [Percent], YieldUnit " & _
                            "FROM egswListe " & _
                            "where code=@intCode "
            .CommandType = CommandType.Text
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
            Try
                cn.Open()
                dr = .ExecuteReader(CommandBehavior.CloseConnection)
                If dr.Read Then
                    strYield = CStrDB(dr.Item("Yield"))
                    strPercent = CStrDB(dr.Item("Percent"))
                    strYieldUnit = CStrDB(dr.Item("YieldUnit"))
                End If

                strYieldPercent = strYield & "|" & strPercent & "|" & strYieldUnit

                dr.Close()
                cn.Close()
            Catch ex As Exception
                cn.Close()
                Throw ex
            End Try
        End With
        Return strYieldPercent
    End Function

    Public Function GetListeNoteDesc(ByVal intCode As Integer, ByVal intCodeTrans As Integer) As String
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader
        Dim strProcedure As String = ""
        Dim strDescription As String = ""
        Dim strNoteDesc As String = ""

        With cmd
            .Connection = cn
            .CommandText = "SELECT CASE WHEN t.Note IS NULL OR LEN(LTRIM(RTRIM(t.[Note])))=0 THEN r.[Note] " & _
                             "ELSE t.[Note] END Note, " & _
                             "CASE WHEN t.Description IS NULL OR LEN(LTRIM(RTRIM(t.[Description])))=0 THEN r.[Description] " & _
                             "ELSE t.[Description] END Description " & _
                            "FROM	egswListe r " & _
                            "LEFT OUTER JOIN egswListeTranslation t ON r.Code=t.codeliste and t.codetrans IN (@intCodeTrans, NULL) " & _
                            "where r.code=@intCode "
            .CommandType = CommandType.Text
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
            Try
                cn.Open()
                dr = .ExecuteReader(CommandBehavior.CloseConnection)
                If dr.Read Then
                    strProcedure = CStrDB(dr.Item("Note"))
                    strDescription = CStrDB(dr.Item("Description"))
                End If

                strNoteDesc = strProcedure & "|" & strDescription

                dr.Close()
                cn.Close()
            Catch ex As Exception
                cn.Close()
                Throw ex
            End Try
        End With
        Return strNoteDesc
    End Function

    Public Function GetListeSubNotes(ByVal intCode As Integer, ByVal intCodeTrans As Integer) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim ds As New DataSet
        With cmd
            .Connection = cn
            .CommandText = "SELECT L.Code, " & _
                                "ISNULL(LTRIM(RTRIM(L.Name)),LTRIM(RTRIM(LT.Name))) AS Name, " & _
                                "ISNULL(LTRIM(RTRIM(L.Note)),LTRIM(RTRIM(LT.Note))) AS Note, " & _
                                "ISNULL(LTRIM(RTRIM(L.Description)),LTRIM(RTRIM(LT.Description))) AS Description, " & _
                                "L.TemplateCode " & _
                            "FROM EgswListe L LEFT OUTER JOIN EgswListeTranslation LT " & _
                            "ON L.Code=LT.CodeListe AND LT.CodeTrans=" & intCodeTrans & " " & _
                            "WHERE Code IN " & _
                                "( " & _
                                "SELECT DISTINCT SecondCode " & _
                                "FROM EgswDetails " & _
                                "WHERE FirstCode=" & intCode & " " & _
                                ") " & _
                            "AND Type = 8 "

            .CommandType = CommandType.Text
            .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCode
            .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
            Try
                cn.Open()
                Dim da As New SqlDataAdapter(.CommandText, .Connection)
                da.Fill(ds)
                cn.Close()
            Catch ex As Exception
                cn.Close()
                Throw ex
            End Try
        End With
        Return ds.Tables(0)
    End Function

    Public Function GetListeCoeff(ByVal intCode As Integer, ByVal intCodeSetPrice As Integer) As Double
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader
        Dim strYield As String = ""
        Dim strPercent As String = ""
        Dim dblCoeff As Double = 0

        With cmd
            .Connection = cn
            .CommandText = "Select Coeff " & _
                            "FROM egswListe l INNER JOIN " & _
                            "egswListeSetPriceCalc r " & _
                            "ON l.Code = r.CodeListe " & _
                            "where l.code=@intCode AND r.CodeSetPrice = @intCodeSetPrice"
            .CommandType = CommandType.Text
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
            .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice
            Try
                cn.Open()
                dr = .ExecuteReader(CommandBehavior.CloseConnection)
                If dr.Read Then
                    dblCoeff = CDblDB(dr.Item("Coeff"))
                End If

                dr.Close()
                cn.Close()
            Catch ex As Exception
                cn.Close()
                Throw ex
            End Try
        End With
        Return dblCoeff
    End Function

    Public Function GetListeTotalCost(ByVal intCode As Integer, ByVal intCodesetPrice As Integer) As DataSet
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim ds As New DataSet
        With cmd
            .Connection = cn
            .CommandText = "SELECT  Id, CodeListe, Coeff, CalcPrice, CodeSetPrice, ImposedPrice, ApprovedPrice, Tax, Coeff2, OldImposedFactor " & _
                           "FROM EgswListeSetPriceCalc " & _
                           "WHERE CodeSetPrice = @intCodeSetPrice AND CodeListe = @intCodeListe"
            .CommandType = CommandType.Text
            .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodesetPrice
            .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCode
            Try
                cn.Open()
                'Dim da As New SqlDataAdapter(.CommandText, .Connection)
                Dim da As New SqlDataAdapter()
                da.SelectCommand = cmd
                da.Fill(ds)
                '.ExecuteReader(CommandBehavior.CloseConnection)                    
                cn.Close()
            Catch ex As Exception
                cn.Close()
                Throw ex
            End Try
        End With
        Return ds
    End Function

    Public Function GetRawMerchandiseList() As DataTable
        Dim sb As New StringBuilder
        With sb
            .Append("SELECT Egsw_Food_Des.NDB_No, Egsw_Food_Des.[Desc], Egsw_Nut_Data.Nutr_No, Egsw_Nut_Data.Nutr_Val, EgswNutrientDef.Position , EgswNutrientDef.format ")
            .Append("From Egsw_Food_Des ")
            .Append("INNER JOIN Egsw_Nut_Data ON Egsw_Nut_Data.NDB_No = Egsw_Food_Des.NDB_No ")
            .Append("INNER JOIN EgswNutrientDef ON EgswNutrientDef.position <=42 ") 'AGL 2012.10.01 - CWM-1317 - changed from 15 to 34
            .Append("AND Egsw_Nut_Data.Nutr_No = EgswNutrientDef.Nutr_No ")
            .Append("ORDER BY Egsw_Food_Des.[Desc], EgswNutrientDef.Position ")
        End With

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader
        Dim dt As New DataTable("RawMerchandise")
        Dim row As DataRow
        With cmd
            .Connection = cn
            .CommandText = sb.ToString
            .CommandType = CommandType.Text
            .CommandTimeout = 90000

            cn.Open()
            dr = .ExecuteReader(CommandBehavior.CloseConnection)
        End With

        '// Create table to display Nutrients list

        Dim counter As Integer = 0

        With dt.Columns
            .Add("NDB_No")
            .Add("DESC")
            For counter = 1 To 42 'AGL 2012.10.01 - CWM-1317 - changed from 15 to 34
                .Add("Val" & counter)
            Next
        End With

        Dim arrNutrNo As New ArrayList(50)    ' Store Nutrients added in the datatable
        Dim sNutr_no As String
        Dim sNutr_desc As String
        Dim sNDB_No As String
        Dim nNutr_Val As Double
        Dim nPosition As Integer
        Dim sFormat As String
        Dim bAddNewRow As Boolean

        While dr.Read
            sNutr_no = CStr(dr.Item("nutr_no"))
            sNutr_desc = CStr(dr.Item("desc"))
            sNDB_No = CStr(dr.Item("NDB_No"))
            nNutr_Val = CDbl(dr.Item("Nutr_Val"))
            nPosition = CInt(dr.Item("Position"))
            sFormat = CStr(dr.Item("Format"))

            If arrNutrNo.Contains(sNutr_desc) Then
                bAddNewRow = False
                row = dt.Rows(arrNutrNo.IndexOf(sNutr_desc))
            Else
                row = dt.NewRow
                bAddNewRow = True
            End If

            row("NDB_No") = sNDB_No
            row("Desc") = sNutr_desc
            row("Val" & nPosition) = nNutr_Val

            If bAddNewRow Then
                dt.Rows.Add(row)
                arrNutrNo.Add(sNutr_desc)
            End If
        End While
        dr.Close()
        Return dt
    End Function

    Public Function GetListeForTransfer(ByVal arr As ArrayList, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, ByVal blnOwnedOnly As Boolean) As DataTable
        Dim sb As New StringBuilder
        Dim SQLCodes As String = ""

        '// Set Codes for SQL 
        Dim counter As Integer
        Dim nTotal As Integer = arr.Count - 1
        If arr.Count = 1 Then
            SQLCodes = CStr(arr(0))
        Else
            For counter = 0 To nTotal - 1
                SQLCodes &= CStr(arr(counter)) & ","
            Next
            SQLCodes &= SQLCodes & CStr(arr(nTotal))
        End If

        '// Main SQL
        If blnOwnedOnly Then
            With sb
                .Append(" Select DISTINCT l.code,ISNULL(t.name,l.name) as name ")
                .Append(" FROM egswliste l ")
                .Append(" INNER JOIN egswSharing s ON s.code=l.code")
                .Append(" LEFT OUTER JOIN egswListeTranslation t ON l.code=t.codeliste  and t.codetrans IN (" & intCodeTrans & ", NULL)")
                .Append(" WHERE s.Type=" & ShareType.CodeSite)
                .Append(" AND s.CodeEgswTable=" & CStr(enumDbaseTables.EgswListe))
                .Append(" AND s.CodeUserSharedTo=" & CStr(intCodeSite))
                .Append(" AND l.code IN ")
                .Append("(")
                .Append(SQLCodes)
                .Append(")")
            End With
        Else
            With sb
                .Append(" Select DISTINCT l.code,ISNULL(t.name,l.name) as name ")
                .Append(" FROM egswliste l ")
                .Append(" LEFT OUTER JOIN egswListeTranslation t ON l.code=t.codeliste  and t.codetrans IN (" & intCodeTrans & ", NULL)")
                .Append(" WHERE l.code IN ")
                .Append("(")
                .Append(SQLCodes)
                .Append(")")
            End With
        End If

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable("Liste")
        Dim da As New SqlDataAdapter

        With cmd
            .Connection = cn
            .CommandText = sb.ToString
            .CommandType = CommandType.Text
        End With
        With da
            .SelectCommand = cmd
            dt.BeginLoadData()
            .Fill(dt)
            dt.EndLoadData()
        End With

        Return dt
    End Function


    Private Function GetItemsRemovedNameAndStep(ByVal strFieldName As String, ByVal dtOldDB As DataTable, ByVal dtNewDB As DataTable) As List(Of List(Of String)) 'ArrayList

        Dim rwOldDB As DataRow
        Dim rwNewDB As DataRow
        Dim strRowOldDBFieldValue As String
        Dim strRowOldDBFieldValueStep As String
        Dim strRowNewDBFieldValue As String
        Dim arrRemoveItems As New List(Of List(Of String)) 'ArrayList
        'Dim arrNew As New List(Of List(Of String))



        For Each rwOldDB In dtOldDB.Rows
            strRowOldDBFieldValue = CStr(rwOldDB.Item(strFieldName))
            strRowOldDBFieldValueStep = CStr(rwOldDB.Item("Step"))
            For Each rwNewDB In dtNewDB.Rows
                strRowNewDBFieldValue = CStr(rwNewDB.Item(strFieldName))
                If (strRowOldDBFieldValue = strRowNewDBFieldValue) Then
                    GoTo GoToNextRowInOldDB
                End If
            Next

            arrRemoveItems.Add(New List(Of String))
            arrRemoveItems(arrRemoveItems.Count - 1).Add(strRowOldDBFieldValue)
            arrRemoveItems(arrRemoveItems.Count - 1).Add(strRowOldDBFieldValueStep)

            'arrRemoveItems.Add(strRowOldDBFieldValue)

GoToNextRowInOldDB:
        Next

        Return arrRemoveItems
    End Function

    Private Function GetItemsRemoved(ByVal strFieldName As String, ByVal dtOldDB As DataTable, ByVal dtNewDB As DataTable) As ArrayList
        Dim rwOldDB As DataRow
        Dim rwNewDB As DataRow
        Dim strRowOldDBFieldValue As String
        Dim strRowNewDBFieldValue As String
        Dim arrRemoveItems As New ArrayList

        For Each rwOldDB In dtOldDB.Rows
            strRowOldDBFieldValue = CStr(rwOldDB.Item(strFieldName))
            For Each rwNewDB In dtNewDB.Rows
                strRowNewDBFieldValue = CStr(rwNewDB.Item(strFieldName))
                If (strRowOldDBFieldValue = strRowNewDBFieldValue) Then
                    GoTo GoToNextRowInOldDB
                End If
            Next
            arrRemoveItems.Add(strRowOldDBFieldValue)
GoToNextRowInOldDB:
        Next

        Return arrRemoveItems
    End Function

    Private Function GetItemsRemoved(ByVal strFieldName As String, ByVal strGroupFieldName As String, ByVal dtOldPrice As DataTable, ByVal dtNewPrice As DataTable) As ArrayList
        Dim rwOldDB As DataRow
        Dim rwNewDB As DataRow
        Dim strRowOldDBFieldValue As String
        Dim strRowNewDBFieldValue As String
        Dim strRowCodeSetPrice As String
        Dim arrRemoveItems As New ArrayList

        For Each rwOldDB In dtOldPrice.Rows
            strRowOldDBFieldValue = CStr(rwOldDB.Item(strFieldName))
            strRowCodeSetPrice = CStr(rwOldDB.Item(strGroupFieldName))
            For Each rwNewDB In dtNewPrice.Rows
                strRowNewDBFieldValue = CStr(rwNewDB.Item(strFieldName))
                If strRowOldDBFieldValue = strRowNewDBFieldValue Then
                    GoTo GoToNextRowInOldDB
                End If
            Next
            arrRemoveItems.Add(strRowOldDBFieldValue)
GoToNextRowInOldDB:
        Next

        Return arrRemoveItems
    End Function

    Private Function GetItemsRemoved(ByVal dtOldSource As DataTable, ByVal arrNewSource As ArrayList) As ArrayList
        Dim row As DataRow
        Dim intOldCode As Integer
        Dim intCounter As Integer
        Dim intLastIndex As Integer = arrNewSource.Count - 1
        Dim arrRemovedItems As New ArrayList
        Dim intCode As Integer

        For Each row In dtOldSource.Rows
            intOldCode = CInt(row.Item("code"))
            For intCounter = 0 To intLastIndex
                intCode = CInt(arrNewSource(intCounter))
                If intCode = intOldCode Then
                    GoTo NextRow
                End If
            Next
            arrRemovedItems.Add(intOldCode)
NextRow:
        Next

        Return arrRemovedItems
    End Function

    Public Function GetListeSubRecipes(ByVal intFirstCode As Integer) As Object
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        With cmd
            .Connection = cn
            .CommandText = "sp_EgswListeSubRecipesGetAffectedForExport"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intFirstCode", SqlDbType.Int).Value = intFirstCode
        End With

        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function IsFieldValueUsed(ByVal intCodeListe As Integer, ByVal strFieldName As String, ByVal strFieldValue As String, ByVal intCodeSite As Integer, ByVal type As enumDataListItemType) As Boolean
        If strFieldValue = "" Then Return False

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim SQL As String = "SELECT code FROM dbo.fn_egswGetListeFullySharedToUserByCodeSite(" + CStr(intCodeSite) + ") " _
             + "WHERE code<>" & intCodeListe & " AND " & strFieldName & "=@nvcValue and type=" & type

        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        With cmd
            .Connection = cn
            .CommandText = SQL
            .CommandType = CommandType.Text

            Select Case strFieldName.ToUpper
                Case "NUMBER"
                    .Parameters.Add("@nvcValue", SqlDbType.NVarChar, 20).Value = ReplaceSpecialCharacters(strFieldValue)
                Case Else
                    .Parameters.Add("@nvcValue", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(strFieldValue)
            End Select

            With da
                .SelectCommand = cmd
                dt.BeginLoadData()
                .Fill(dt)
                dt.EndLoadData()
            End With

        End With

        If dt.Rows.Count <> 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetListePictures(ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, _
             ByVal eListeType As enumDataListItemType, _
             Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault, _
             Optional ByVal intCodeListe As Integer = -1) As Object
        'DLS Feb.2.2007
        Dim arrParam(3) As SqlParameter

        arrParam(0) = New SqlParameter("@p_nListeType", eListeType)
        arrParam(1) = New SqlParameter("@p_nCodeSite", intCodeSite)
        arrParam(2) = New SqlParameter("@p_nCodeTrans", intCodeTrans)
        arrParam(3) = New SqlParameter("@p_nCodeListe", intCodeListe)

        Try
            If fetchType = enumEgswFetchType.UseDefault Then
                fetchType = L_bytFetchType
            End If

            Return ExecuteFetchType(fetchType, L_strCnn, CommandType.StoredProcedure, "sp_EgswListePictures", arrParam)

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListeAllPictures(Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.UseDefault) As Object
        'DLS Feb.13.2007
        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@return", SqlDbType.Int)
        arrParam(0).Direction = ParameterDirection.ReturnValue

        Try
            If fetchType = enumEgswFetchType.UseDefault Then
                fetchType = L_bytFetchType
            End If

            Return ExecuteFetchType(fetchType, L_strCnn, CommandType.StoredProcedure, "sp_EgswListeAllPictures", arrParam)

        Catch ex As Exception
            Throw ex
        End Try
    End Function
    '--- VRP 19.09.2007
    Public Function fctGetBreadcrumbs(ByVal intCodeUser As Integer, ByVal intCodeTrans As Integer, Optional ByVal intCodeSite As Integer = -1, Optional ByVal intTypeItem As Integer = -1, Optional ByVal intUser As Integer = -1, Optional ByVal dtDate As Date = #1/1/2007#) As DataTable
        Dim arrParam(5) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeUser", intCodeUser)
        arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans)
        arrParam(2) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(3) = New SqlParameter("@intTypeItem", intTypeItem)
        arrParam(4) = New SqlParameter("@intUser", intUser)
        arrParam(5) = New SqlParameter("@dtDate", dtDate)
        'arrParam(6) = New SqlParameter("@dtDateYear", dtDateYear)
        'arrParam(7) = New SqlParameter("@dtDateDay", dtDateDay)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "sp_EgswGetBreadcrumbs", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function '---------------

    Public Function fctGetListeSharedSites(ByVal intCode As Integer) As DataTable
        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@intCode", intCode)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "sp_EgswGetListeSharedSite", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function '---------------


    '// DRR 1.5.2011
    Public Function fctGetCheckedOutItems2(ByVal intCodeUser As Integer) As DataTable
        Dim strSQL As String = "SELECT Code,Type,Number,Name,CheckOutUser,0 as Flag FROM egswliste WHERE CheckOutUser=@User ORDER BY Name"
        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@User", intCodeUser)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.Text, strSQL.ToString, arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function '---------------


    '// DRR 2.25.2011
    Public Function fctGetCheckedOutItems(ByVal intCodeUser As Integer, ByVal intCodeSite As Integer, Optional ByVal intListeType As Integer = 8) As DataTable 'JTOC 10.18.2013 Added parameter intListeType
        'Dim strSQL As String
        '      strSQL = "SELECT egswliste.Code, egswliste.Type, egswliste.Number, egswliste.Name, egswliste.CheckOutUser, EgswUser.UserName, EgswUser.Code as UserCode, 0 as Flag "
        '      strSQL = strSQL & ", CAST(CASE WHEN EgswUser.CodeSite=@CodeSite THEN 1 ELSE 0 END AS BIT) IsSameSite" & vbCrLf
        'strSQL = strSQL & "FROM Egswliste " & vbCrLf
        'strSQL = strSQL & "INNER JOIN EgswUser on egswliste.CheckOutUser=EgswUser.Code " & vbCrLf
        '      strSQL = strSQL & "WHERE Egswliste.type in(8,16) And ((@User = -1 OR CheckOutUser=@User) AND CheckOutUser>0 /*AND Egswliste.CodeSite=@CodeSite*/ " & vbCrLf
        'strSQL = strSQL & "AND (Egswliste.Code IN (SELECT Code FROM EgswSharing WHERE CodeUserSharedTo = @CodeSite AND [Type]=5 AND CodeEgswTable=50) OR Egswliste.CodeSite=@CodeSite /*)*/ " & vbCrLf
        'strSQL = strSQL & "OR 1=(Select Top 1 IsGlobal From Egswsharing Where Code=Egswliste.Code And Codeegswtable=50 And IsGlobal=1) ))" & vbCrLf
        'strSQL = strSQL & "ORDER BY egswliste.Name "

        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@User", intCodeUser)
        arrParam(1) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(2) = New SqlParameter("@ListeType", intListeType)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "GET_CheckOutItems", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function '---------------

    '// DRR 10.10.2011
    Public Function fctIsAdminUser(ByVal intCodeUser As Integer) As Boolean
        Dim dt As New DataTable
        Dim strSQL As String
        'strSQL = "SELECT (case when count(codeuser)=0 then 0 else 1 end) IsAdmin FROM EgswuserRoles WHERE codeuser = @User and [role] = (SELECT Top 1 code FROM EgswRoles WHERE name = 'ADMIN' OR LOWER(name)='system admin') "	'AGL 2012.12.17 - added system admin role

        'JTOC 25.04.2013 select both admin and system admin not just the top 1
        'AGL 2013.08.14 - changed to LIKE '%admin%'
        strSQL = "SELECT (case when count(codeuser)=0 then 0 else 1 end) IsAdmin FROM EgswuserRoles WHERE codeuser = @User and [role] in (SELECT code FROM EgswRoles WHERE name IN ('ADMIN','AMMINISTRATORE')) "

        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@User", intCodeUser)
        Try
            dt = ExecuteDataset(L_strCnn, CommandType.Text, strSQL.ToString, arrParam).Tables(0)
            Return CBoolDB(dt.Rows(0)("IsAdmin"))
        Catch ex As Exception
            Throw ex
        End Try
    End Function '---------------


    '--- VRP 03.01.2008
    Public Function GetListeSetOfPriceReader(ByVal nCodeListe As Integer, ByVal nCodeSetPrice As Integer) As SqlDataReader
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader

        Try
            With cmd
                .Connection = cn
                '.CommandText = "SELECT p.*,u.*,cur.symbole,setprice.Name as SetOfPrice " & _
                '               "FROM EgswListeSetPrice p " & _
                '               "INNER JOIN EgswUnit u ON p.unit=u.code " & _
                '               "INNER JOIN EgswSetPrice setprice ON p.CodeSetPrice=setprice.Code " & _
                '               "INNER JOIN Egsw_Currency cur ON setprice.codecurrency=cur.code " & _
                '               "WHERE p.codeliste=" & nCodeListe
                .CommandText = "SELECT p.*,u.*,cur.symbole,setprice.Name as SetOfPrice " & _
                      "from EgsWListeSetPrice P " & _
                      "INNER JOIN EgsWUnit U ON P.UnitDisplay = U.Code " & _
                      "INNER JOIN EgswSetPrice setprice ON p.CodeSetPrice=setprice.Code " & _
                      "INNER JOIN Egsw_Currency cur ON setprice.codecurrency=cur.code " & _
                      "WHERE CodeListe =  " & nCodeListe & " " & _
                      "AND CodeSetPrice =  " & nCodeSetPrice & " " & _
                      "ORDER BY Position"
                .CommandType = CommandType.Text
            End With
            cn.Open()
            dr = cmd.ExecuteReader
            Return dr
        Catch ex As Exception
        End Try
        cn.Close()
        cn.Dispose()
    End Function



    Public Function GetListeSetOfPriceReader2(ByVal nCodeListe As Integer, ByVal nCodeSetPrice As Integer) As DataTable

        Using conn As New SqlConnection(L_strCnn)
            Dim dt As New DataTable()
            dt.TableName = "MigroLabel"
            Try
                Using cmd As SqlCommand = conn.CreateCommand()
                    cmd.CommandType = CommandType.Text
                    cmd.CommandText = "SELECT " & vbCrLf & _
                     "p.CodeListe,p.Unit,cast(isnull(p.Price,0) as varchar(30)) Price,p.Position, cast(isnull(p.Ratio,0) as varchar(30)) as Ratio, " & vbCrLf & _
                     "p.RatioNut, p.UnitDisplay, p.CodeSetPrice, p.Id, p.Tax, p.Pos2, p.BigPrice, p.BigRatio " & vbCrLf & _
                      ",u.*,cur.symbole,setprice.Name as SetOfPrice " & _
                      "from EgsWListeSetPrice P " & _
                      "INNER JOIN EgsWUnit U ON P.UnitDisplay = U.Code " & _
                      "INNER JOIN EgswSetPrice setprice ON p.CodeSetPrice=setprice.Code " & _
                      "INNER JOIN Egsw_Currency cur ON setprice.codecurrency=cur.code " & _
                      "WHERE CodeListe =  " & nCodeListe & " " & _
                      "AND CodeSetPrice =  " & nCodeSetPrice & " " & _
                      "ORDER BY Position"
                    cmd.CommandTimeout = 18000
                    conn.Open()
                    Dim x As New SqlDataAdapter(cmd)
                    x.Fill(dt)
                End Using
            Catch ex As Exception

            Finally
                If conn IsNot Nothing Then
                    conn.Close()
                    conn.Dispose()
                End If
            End Try
            Return dt
        End Using

    End Function

    Public Sub RecomputeSubRecipeLevel()
        'RDTC 15.10.2007
        Dim cmd As New SqlCommand

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_egswFixSRlevel"
                .CommandType = CommandType.StoredProcedure

                .CommandTimeout = 50000
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
            End With
        Catch ex As Exception
            If cmd.Connection.State = ConnectionState.Open Then
                cmd.Connection.Close()
                cmd.Dispose()
                'Throw New Exception(ex.Message, ex)
            End If
        End Try

        cmd.Dispose()
    End Sub

    Public Function subGetProcedureText(ByVal nCode As Integer) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dt As DataTable
        Dim da As New SqlDataAdapter
        Try
            With cmd
                .Connection = cn
                .CommandText = "SELECT Note FROM EgswListe " & _
                      "WHERE Code=" & nCode
                .CommandType = CommandType.Text
            End With
            cn.Open()
            With da
                .SelectCommand = cmd
                dt.BeginLoadData()
                .Fill(dt)
                dt.EndLoadData()
            End With
            Return dt
        Catch ex As Exception
        End Try
        cn.Close()
        cn.Dispose()
    End Function '----

    Public Function fctGetUsedTemplateCount(ByVal intCodeMain As Integer) As Integer 'VRP 18.04.2008
        Dim intX As Integer
        Try
            Dim cn As New SqlConnection(L_strCnn)
            Dim cmd As New SqlCommand
            Dim da As New SqlDataAdapter
            Dim dt As New DataTable("EGSWLISTE")
            Dim sbSQL As New StringBuilder

            If intCodeMain = 0 Then intCodeMain = -1
            With sbSQL
                .Append("SELECT COUNT(TemplateCode) AS Count ")
                .Append("FROM EgswListe ")
                .Append("WHERE TemplateCode=@CodeMain")
            End With

            With cmd
                .Connection = cn
                .CommandText = sbSQL.ToString
                .CommandType = CommandType.Text
                .Parameters.Add("@CodeMain", SqlDbType.Int).Value = intCodeMain

                cn.Open()
                intX = CInt(.ExecuteScalar)

                cn.Close()
                cn.Dispose()
                Return intX
            End With
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
            Return -1
        End Try
    End Function '---- 

    Public Function fctGetMediaFilesList(ByVal intFlag As Integer, ByVal intCodeliste As Integer, ByVal strFileName As String) As DataTable 'VRP 07.05.2008
        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@intFlag", intFlag)
        arrParam(1) = New SqlParameter("@CodeListe", intCodeliste)
        arrParam(2) = New SqlParameter("@FileName", strFileName)

        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "sp_EgswListeFilesGet", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    '--- VRP 24.03.2008
    Public Function fctGetProcedureTemplateList(ByVal intCodeSite As Integer, Optional ByVal intCode As Integer = 0, _
               Optional ByVal intOption As Integer = 0, Optional ByVal intCodeTrans As Integer = -1, _
               Optional ByVal intCodeMain As Integer = -1) As DataTable 'Test

        Dim sqlCmd As SqlCommand = New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "sp_EgswGetProcedureTemplate"
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@intCodeMain", SqlDbType.Int).Value = intCodeMain
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@intOption", SqlDbType.Int).Value = intOption
                .ExecuteNonQuery()
                With da
                    .SelectCommand = sqlCmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                .Dispose()
            End With
            Return dt
        Catch ex As Exception

        End Try
    End Function '----

    Public Function fctGetProcedureTemplateM(ByVal intCodeSite As Integer) As DataTable

        Dim sqlCmd As SqlCommand = New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "sp_EgswGetProcedureTemplateM"
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .ExecuteNonQuery()
                With da
                    .SelectCommand = sqlCmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                .Dispose()
            End With
            sqlCmd.Connection.Close() 'DLS 26.01.2009
            Return dt
        Catch ex As Exception
            sqlCmd.Dispose()
        End Try
    End Function '----

    Public Function fctGetProcedureTemplateD(ByVal intCodeTrans As Integer, ByVal intCodeMain As Integer, _
                Optional ByVal intOption As Integer = 0, Optional ByVal intCode As Integer = -1) As DataTable 'Test

        Dim sqlCmd As SqlCommand = New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "sp_EgswGetProcedureTemplateD"
                .Parameters.Add("@intCodeMain", SqlDbType.Int).Value = intCodeMain
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@intOption", SqlDbType.Int).Value = intOption
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .ExecuteNonQuery()
                With da
                    .SelectCommand = sqlCmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
            End With
            sqlCmd.Connection.Close() 'DLS 26.01.2009
            Return dt
        Catch ex As Exception
            sqlCmd.Dispose()
        End Try
    End Function '----

    Public Function fctGetSharingList(ByVal intCode As Integer, ByVal intCodeSite As Integer, _
              ByVal enumCodeEgswTable As enumDbaseTables) As DataTable
        Dim cmd As SqlCommand = New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "sp_EgswGetSharingList"
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@intCodeEgswTable", SqlDbType.Int).Value = enumCodeEgswTable
                .ExecuteNonQuery()

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                .Dispose()
            End With
            Return dt
        Catch ex As Exception
            cmd.Dispose()
        End Try
    End Function
    '----

    'MRC 18.06.08
    Public Function GetAutoNumberDetail(ByVal intCodeSite As Integer) As Object
        Dim cmd As SqlCommand = New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "sp_EgswGetAutoNumberDetail"
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .ExecuteNonQuery()

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                .Dispose()
            End With
            Return dt
        Catch ex As Exception
            cmd.Dispose()
        End Try
    End Function

    'JTOC 11.13.2013 
    Public Function GetAutoNumberDetailCategory(ByVal intCodeSite As Integer, ByVal intCodeCategory As Integer) As Object
        Dim cmd As SqlCommand = New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "sp_EgswGetAutoNumberDetailCategory"
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@intCodeCategory", SqlDbType.Int).Value = intCodeCategory
                .ExecuteNonQuery()

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                .Dispose()
            End With
            Return dt
        Catch ex As Exception
            cmd.Dispose()
        End Try
    End Function

    Public Function UpdateAutoNumberDetail(ByVal intCodeSite As Integer, ByVal intItemType As Integer, ByVal blnActivateAutoNum As Boolean, _
      ByVal strPrefix As String, ByVal strStartingNum As String, ByVal blnKeepLength As Boolean) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswUpdateAutoNumberDetail"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@intItemType", SqlDbType.Int).Value = intItemType
                .Parameters.Add("@blnAutoNumber", SqlDbType.Bit).Value = blnActivateAutoNum
                .Parameters.Add("@vchPrefix", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(strPrefix)
                .Parameters.Add("@vchStartingNum", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(strStartingNum)
                .Parameters.Add("@blnKeepLength", SqlDbType.Bit).Value = blnKeepLength
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                .ExecuteNonQuery()
                cmd.Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    'JTOC 11.13.2013
    Public Function UpdateAutoNumberDetailCategory(ByVal intCodeSite As Integer, ByVal intItemType As Integer, ByVal blnActivateAutoNum As Boolean, _
      ByVal strPrefix As String, ByVal strStartingNum As String, ByVal blnKeepLength As Boolean, ByVal intCodeCategory As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswUpdateAutoNumberDetailCategory"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@intItemType", SqlDbType.Int).Value = intItemType
                .Parameters.Add("@blnAutoNumber", SqlDbType.Bit).Value = blnActivateAutoNum
                .Parameters.Add("@vchPrefix", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(strPrefix)
                .Parameters.Add("@vchStartingNum", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(strStartingNum)
                .Parameters.Add("@blnKeepLength", SqlDbType.Bit).Value = blnKeepLength
                .Parameters.Add("@intCodeCategory", SqlDbType.Int).Value = intCodeCategory
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                .ExecuteNonQuery()
                cmd.Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function GetAutoNumber(ByVal intCodeSite As Integer, ByVal inCodeUser As Integer, ByVal intItemType As Integer, Optional ByVal intPlus As Integer = 0, Optional ByRef intCategory As Integer = -1) As String 'JTOC 11.14.2013 Added blnCategory parameter
        Dim strNumber As String = ""
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = IIf(intCategory = -1, "sp_EgswGetAutoNum", "sp_EgswGetAutoNumCategory") 'JTOC 11.14.2013 Added condition
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = inCodeUser
                .Parameters.Add("@intItemType", SqlDbType.Int).Value = intItemType
                .Parameters.Add("@intPlus", SqlDbType.SmallInt).Value = intPlus

                If intCategory <> -1 Then .Parameters.Add("@intCodeCategory", SqlDbType.Int).Value = intCategory 'JTOC 11.14.2013 

                .Parameters.Add("@vchNumber", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output
                .Parameters.Add("@ERR", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                .Connection.Open()
                .ExecuteNonQuery()
                cmd.Connection.Close()

                strNumber = CType(.Parameters("@vchNumber").Value, String)
                L_ErrCode = CType(.Parameters("@ERR").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return strNumber
    End Function

    Public Function fctGetProcedureStyles(ByVal intCode As Integer) As DataTable 'VRP 01.06.2008

        Dim sqlCmd As SqlCommand = New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "sp_EgswGetProcedureStyles"
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .ExecuteNonQuery()

                With da
                    .SelectCommand = sqlCmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                .Dispose()
            End With
            Return dt
        Catch ex As Exception
            sqlCmd.Dispose()
        End Try
    End Function

    Public Function fctGetTemplateStyleCount(ByVal intCodeStyle As Integer) As Integer 'VRP 09.07.2008
        Try
            Dim cn As New SqlConnection(L_strCnn)
            Dim cmd As New SqlCommand
            Dim da As New SqlDataAdapter
            Dim dt As New DataTable("EgswProcedureTemplateD")
            Dim sbSQL As New StringBuilder

            With sbSQL
                .Append("SELECT COUNT(CodeStyle) AS Count ")
                .Append("FROM EgswProcedureTemplateD ")
                .Append("WHERE CodeStyle=" & intCodeStyle)
            End With

            With cmd
                .Connection = cn
                .CommandText = sbSQL.ToString
                .CommandType = CommandType.Text

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With

                If Not dt.Rows.Count > 0 Then
                    Return -1
                Else
                    Return CInt(dt.Rows(0).Item("count"))
                End If

            End With
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
            Return -1
        End Try
    End Function '---- 

    Public Function fctGetListeStyleCount(ByVal intCodeStyle As Integer) As Integer 'VRP 09.07.2008
        Try
            fctGetListeStyleCount = 0

            Dim cn As New SqlConnection(L_strCnn)
            Dim cmd As New SqlCommand
            Dim da As New SqlDataAdapter
            Dim dt As New DataTable("EgswProcedureTemplateD")
            Dim sbSQL As New StringBuilder

            With sbSQL
                .Append("SELECT CodeStyle FROM EgswListe")
            End With

            With cmd
                .Connection = cn
                .CommandText = sbSQL.ToString
                .CommandType = CommandType.Text

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With

                Dim strCodestyles() As String
                For Each row As DataRow In dt.Rows
                    strCodestyles = CStrDB(row("CodeStyle")).Split(CChar("¤"))
                    For i As Integer = 0 To UBound(strCodestyles)
                        If Not strCodestyles(i) = "" Then
                            If CInt(strCodestyles(i)) = intCodeStyle Then
                                Return 1
                                Exit For
                            End If
                        End If
                    Next
                Next

            End With
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
            Return -1
        End Try
    End Function '---- 

    Public Function fctGetPropertySiteUser(ByVal intCodeUser As Integer, ByVal intCodeSite As Integer) As Object
        Dim strCommandText As String = "sp_EgswGetPropertySiteUser"
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeUser", intCodeUser)
        arrParam(1) = New SqlParameter("@intCodeSite", intCodeSite)

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

    Public Function fctGetUnitListeCodeName(ByVal intCodeListe As Integer, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, Optional ByVal intMetric As Integer = 3, Optional ByVal intListeType As Integer = -1) As Object 'VRP 27.10.2008
        Dim strCommandText As String = "GET_UNITLISTECODENAME"

        Dim arrParam(4) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeListe", intCodeListe)
        arrParam(1) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(2) = New SqlParameter("@intCodeTrans", intCodeTrans)
        arrParam(3) = New SqlParameter("@intMetric", intMetric)
        If intListeType <> -1 Then
            arrParam(4) = New SqlParameter("@intListeType", intListeType)
        End If

        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function


    Public Function fctGetSearchNames(ByVal strName As String, ByVal intType As MenuType, ByVal intCodeTrans As Integer, ByVal intCodeSite As Integer, ByVal intCodeUser As Integer) As DataTable
        Dim cmd As SqlCommand = New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "GET_SEARCHNAMES"
                .Parameters.Add("@Name", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(strName)
                .Parameters.Add("@Type", SqlDbType.Int).Value = intType
                .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser
                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
            End With
            Return dt
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetExportListe(ByVal intListeType As enumDataListItemType, ByVal intCodeSite As Integer, ByVal intCodeUser As Integer, ByVal intCodeSetPrice As Integer, ByVal intCodeTrans As Integer, _
         ByVal strSelectedCodeListe As String, ByRef strSQL As String, ByVal slIncludeList As SortedList, _
         Optional ByVal intStatus As Integer = 0, Optional ByVal intSupplier As Integer = 0, _
         Optional ByVal intCategory As Integer = 0) As DataTable

        Dim strCommandText As String = "sp_EgsWExportListeSQL"
        Dim arrParam(39) As SqlParameter

        arrParam(0) = New SqlParameter("@ListeType", intListeType)
        arrParam(1) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(2) = New SqlParameter("@CodeSetPrice", intCodeSetPrice)
        arrParam(3) = New SqlParameter("@CodeUser", intCodeUser)
        arrParam(4) = New SqlParameter("@CodeTrans", intCodeTrans)
        arrParam(5) = New SqlParameter("@SelectCodeListe", strSelectedCodeListe)
        arrParam(6) = New SqlParameter("@SQL", strSQL)
        arrParam(6).Direction = ParameterDirection.Output

        arrParam(7) = New SqlParameter("@incNumber", CBool(slIncludeList("NUMBER")))
        arrParam(8) = New SqlParameter("@incTax", CBool(slIncludeList("TAX")))
        arrParam(9) = New SqlParameter("@incPrices", CBool(slIncludeList("PRICES")))
        arrParam(10) = New SqlParameter("@incWastage", CBool(slIncludeList("WASTAGE")))
        arrParam(11) = New SqlParameter("@incSupplier", CBool(slIncludeList("SUPPLIER")))
        arrParam(12) = New SqlParameter("@incNutrients", CBool(slIncludeList("NUTRIENTS")))
        arrParam(13) = New SqlParameter("@incCategory", CBool(slIncludeList("CATEGORY")))
        arrParam(14) = New SqlParameter("@IncKeyword", CBool(slIncludeList("KEYWORD")))
        arrParam(15) = New SqlParameter("@incBrand", CBool(slIncludeList("BRAND")))
        arrParam(16) = New SqlParameter("@incInformation", CBool(slIncludeList("INFO")))
        arrParam(17) = New SqlParameter("@incImposedsellingprice", CBool(slIncludeList("IMPSELLING")))
        arrParam(18) = New SqlParameter("@incYield", CBool(slIncludeList("YIELD")))
        arrParam(19) = New SqlParameter("@incSource", CBool(slIncludeList("SOURCE")))
        arrParam(20) = New SqlParameter("@incCalculatePrice", CBool(slIncludeList("CALCPRICE")))
        arrParam(21) = New SqlParameter("@incImposedprice", CBool(slIncludeList("IMPPRICE")))
        arrParam(22) = New SqlParameter("@incConst", CBool(slIncludeList("CONST")))
        arrParam(23) = New SqlParameter("@incNote", CBool(slIncludeList("NOTE")))
        arrParam(24) = New SqlParameter("@incsellingprice", CBool(slIncludeList("SELLINGPRICE")))
        arrParam(25) = New SqlParameter("@incsellingpricetax", CBool(slIncludeList("SELLINGPRICETAX")))
        arrParam(26) = New SqlParameter("@inccode", CBool(slIncludeList("CODE")))
        arrParam(27) = New SqlParameter("@incmargin", CBool(slIncludeList("GROSSMARGIN")))
        arrParam(28) = New SqlParameter("@incdates", CBool(slIncludeList("DATES")))
        arrParam(29) = New SqlParameter("@incfoodcost", CBool(slIncludeList("FOODCOST")))
        arrParam(30) = New SqlParameter("@incimposedmargin", CBool(slIncludeList("IMPOSEDMARGIN")))
        arrParam(31) = New SqlParameter("@incimposedfoodcost", CBool(slIncludeList("IMPOSEDFOODCOST")))

        '-- VRP 12.01.2009
        arrParam(32) = New SqlParameter("@intStatus", intStatus)
        arrParam(33) = New SqlParameter("@intSupplier", intSupplier)
        arrParam(34) = New SqlParameter("@intCategory", intCategory)
        '---
        arrParam(35) = New SqlParameter("@incCurrency", CBool(slIncludeList("CURRENCY"))) 'VRP 18.03.2009

        arrParam(36) = New SqlParameter("@incSubName", CBool(slIncludeList("SUBNAME"))) 'KGS 2020.06.29
        arrParam(37) = New SqlParameter("@incAdditionalNotes", CBool(slIncludeList("ADDITIONALNOTES"))) 'KGS 2020.06.29
        arrParam(38) = New SqlParameter("@incDefaultPicture", CBool(slIncludeList("DEFAULTPICTURE"))) 'KGS 2020.06.30
        arrParam(39) = New SqlParameter("@incCategoryConcat", CBool(slIncludeList("CATEGORYCONCAT"))) 'KGS 2020.06.30

        Try
            Return CType(ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam), DataTable)
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    Public Function GetCurrencyFormat(ByVal CodeSetPrice As Integer) As String
        Dim cmd As New SqlCommand
        Dim nReturnValue As String

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "select PriceFormat=C.Format from EgswSetPrice P INNER JOIN Egsw_Currency C ON P.CodeCurrency=C.Code and P.Code=" & CodeSetPrice
                .CommandType = CommandType.Text
                .Connection.Open()
                nReturnValue = .ExecuteScalar()
                .Connection.Close()
            End With

            Return nReturnValue

        Catch ex As Exception
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
    End Function


    Public Function GetExportListeNiceLabel(ByVal intListeType As enumDataListItemType, ByVal intCodeSite As Integer, ByVal intCodeUser As Integer, ByVal intCodeTrans As Integer, _
         ByVal strSelectedCodeListe As String, ByRef strSQL As String, ByVal intCodeSetPrice As Integer) As DataTable

        Dim strCommandText As String = "sp_EgsWExportListeSQLNiceLabel"
        Dim arrParam(7) As SqlParameter

        arrParam(0) = New SqlParameter("@ListeType", intListeType)
        arrParam(1) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(3) = New SqlParameter("@CodeUser", intCodeUser)
        arrParam(4) = New SqlParameter("@CodeTrans", intCodeTrans)
        arrParam(5) = New SqlParameter("@SelectCodeListe", strSelectedCodeListe)
        arrParam(6) = New SqlParameter("@SQL", strSQL)
        arrParam(7) = New SqlParameter("@CodeSetPrice", intCodeSetPrice)
        arrParam(6).Direction = ParameterDirection.Output
        Try
            Return CType(ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam), DataTable)
        Catch ex As Exception
            Throw ex
        End Try
    End Function


    Public Function GetListeCopyPrice(ByVal blnCompareByName As Boolean, ByVal intCopyCodeSite As Integer, _
              ByVal intCodeSite As Integer, ByVal intListeType As enumDataListItemType, _
              ByVal intCodeTrans As Integer, ByVal intCodeSetPrice As Integer, _
              Optional ByVal strCodeListeList As String = "") As DataTable

        Dim cmd As New SqlCommand
        Dim cn As New SqlConnection(L_strCnn)

        Dim arrParam(6) As SqlParameter
        arrParam(0) = New SqlParameter("@bitCompareByName", blnCompareByName)
        arrParam(1) = New SqlParameter("@intCopyCodeSite", intCopyCodeSite)
        arrParam(2) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(3) = New SqlParameter("@intListeType", intListeType)
        arrParam(4) = New SqlParameter("@intCodeTrans", intCodeTrans)
        arrParam(5) = New SqlParameter("@intCodeSetPrice", intCodeSetPrice)
        arrParam(6) = New SqlParameter("@strCodeListeList", strCodeListeList)

        Try
            Return CType(ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "[GET_COPYPRICELIST]", arrParam), DataTable)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    '--JBB July 16, 2010
    '--Get Ingredients of Selected Recipe 
    Public Function GetListeRecipeIngredient(ByVal strCodeListe As String, intCodeSite As Integer) As DataTable
        Dim cmd As New SqlCommand
        Dim cn As New SqlConnection(L_strCnn)
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@NVCodeList", SqlDbType.VarChar, 8000)
        arrParam(0).Value = strCodeListe
        arrParam(1) = New SqlParameter("@intCodeSite", intCodeSite)
        Try
            Return CType(ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "[GETListeRecipeIngredientList]", arrParam), DataTable)
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Public Function GetListeRecipeIngredientSub(ByVal strCodeListe As String, ByVal intSecondCodeListe As Integer, intCodeSite As Integer, intCodeTrans As Integer) As DataTable
        Dim cmd As New SqlCommand
        Dim cn As New SqlConnection(L_strCnn)
        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@intSecondCodeListe", intSecondCodeListe)
        arrParam(1) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(2) = New SqlParameter("@intCodeTrans", intCodeTrans)
        arrParam(3) = New SqlParameter("@NVCodeList", SqlDbType.VarChar, 8000)
        arrParam(3).Value = strCodeListe
        Try
            Return CType(ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "[GETListeRecipeIngredientListSub]", arrParam), DataTable)
        Catch ex As Exception
            Return Nothing
        End Try

    End Function
    '--

    '--- JRN 09.13.2010
    '--- Get recipe codes created by user
    Public Function GetRecipeCodesBySource(ByVal intCodeUser As Integer) As DataTable
        Dim cmd As New SqlCommand
        Dim cn As New SqlConnection(L_strCnn)
        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeUser", SqlDbType.Int)
        arrParam(0).Value = intCodeUser
        Try
            Return CType(ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "[LISTE_GetRecipeCodesBySource]", arrParam), DataTable)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    '---

    '-- JBB 01.10.2012
    '-- Get Recipe Imposed Nutrient (Same with the Recipe Preview)
    Public Function GetRecipeImposedNutrients(ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer, Optional intCodeSet As Integer = 0) As DataTable
        Dim cmd As New SqlCommand
        Dim cn As New SqlConnection(L_strCnn)
        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeListe", SqlDbType.Int)
        arrParam(1) = New SqlParameter("@CodeTrans", SqlDbType.Int)
        arrParam(2) = New SqlParameter("@CodeSet", SqlDbType.Int)
        arrParam(0).Value = intCodeListe
        arrParam(1).Value = intCodeTrans
        arrParam(2).Value = intCodeSet

        Try
            Return CType(ExecuteFetchType(enumEgswFetchType.DataTable, L_strCnn, CommandType.StoredProcedure, "[Get_RecipeImposedNutrient]", arrParam), DataTable)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    '--

    '// DRR 05.14.2012
    Public Function GetSpecialCharacters()
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim sbSQL As New StringBuilder
        With sbSQL
            .Append("SELECT Name FROM  [dbo].[fn_EgswSpecialCharacters]()")
        End With

        Try
            With cmd
                .Connection = cn
                .CommandText = sbSQL.ToString
                .CommandTimeout = 10000
                .CommandType = CommandType.Text

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
            End With
            Return dt

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
            Return Nothing
        End Try
    End Function


#End Region

#Region " Delete Functions "

    Public Function RemoveListeShared(ByVal intCodeListe As Integer, ByVal eShareType As ShareType, Optional ByVal strShareType As String = "", Optional ByVal intCodeUser As Integer = -1) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = cn
            .CommandText = "sp_EgswListeSharedDelete"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@sntType", SqlDbType.SmallInt).Value = eShareType
            .Parameters.Add("@vchTypeList", SqlDbType.VarChar, 8000).Value = strShareType
            .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
            .Parameters.Add("@intCodeuser", SqlDbType.Int).Value = intCodeUser
        End With

        Try
            cn.Open()
            cmd.ExecuteNonQuery()
            L_ErrCode = CType(cmd.Parameters("@retval").Value, enumEgswErrorCode)
            cn.Close()
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State <> ConnectionState.Closed Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    ''' <summary>
    ''' Remove Liste
    ''' </summary>
    ''' <param name="udtUser"></param>
    ''' <param name="intCodeListe"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RemoveListe(ByVal udtUser As structUser, ByVal intCodeListe As Integer) As enumEgswErrorCode
        Dim dt As DataTable
        Try
            Dim blnIsApprovalRequired As Boolean
            ' execute mass mutation, we just have to reuse this function
            Dim errorcode As enumEgswErrorCode = UpdateListeMassMutation(udtUser, "(" & intCodeListe & ")", 0, 0, False, 0, udtUser.RoleLevelHighest, dt, UserRightsFunction.AllowDelete, "(" & udtUser.Site.Code & ")", blnIsApprovalRequired)

            ' if record still exists, then check its status
            If dt.Rows.Count <> 0 Then
                Dim row As DataRow = dt.Rows(0)
                errorcode = CType(row("status"), enumEgswErrorCode)
                Return errorcode
            Else
                Return enumEgswErrorCode.OK
            End If

        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Private Function RemoveListePricesItem(ByVal arr As ArrayList, Optional ByRef stCaller As SqlTransaction = Nothing) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Dim st As SqlTransaction
        Dim nValue As Integer
        Dim counter As Integer
        Dim nLastIndex As Integer = arr.Count - 1
        Try
            With cmd
                .CommandText = "sp_egswListeSetPriceDeleteItem"
                .CommandType = CommandType.StoredProcedure

                If stCaller Is Nothing Then
                    .Connection = New SqlConnection(L_strCnn)
                    cmd.Connection.Open()
                    st = cmd.Connection.BeginTransaction
                Else
                    st = stCaller
                    .Connection = st.Connection
                End If

                .Transaction = st
                .Parameters.Add("@intID", SqlDbType.Int)
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                For counter = 0 To nLastIndex
                    nValue = CInt(arr(counter))
                    .Parameters("@intID").Value = nValue
                    .ExecuteNonQuery()

                    L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                    If L_ErrCode <> enumEgswErrorCode.OK Then Throw New Exception
                Next
            End With

            If stCaller Is Nothing Then
                st.Commit()
                cmd.Connection.Close()
            Else
                stCaller = st
            End If
        Catch ex As Exception
            If stCaller Is Nothing Then st.Rollback()

            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function RemoveListeKeyDetailsItem(ByVal intCodeListe As Integer, ByVal arrItems As ArrayList, Optional ByRef stCaller As SqlTransaction = Nothing) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim st As SqlTransaction
        Dim intCounter As Integer
        Dim intLastindex As Integer = arrItems.Count - 1
        Dim intCodeKey As Integer
        Dim egswErrorType As enumEgswErrorCode = enumEgswErrorCode.GeneralError

        If intLastindex = -1 Then
            L_ErrCode = enumEgswErrorCode.OK
            Exit Function
        End If

        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_egswKeyDetailsDeleteListeItem"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeListe", SqlDbType.Int)
                .Parameters.Add("@intCodeKey", SqlDbType.Int)
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue


                If stCaller Is Nothing Then
                    .Connection = New SqlConnection(L_strCnn)
                    cmd.Connection.Open()
                    st = cmd.Connection.BeginTransaction
                Else
                    st = stCaller
                    .Connection = st.Connection
                End If

                .Transaction = st

                For intCounter = 0 To intLastindex
                    intCodeKey = CInt(arrItems(intCounter))
                    .Parameters("@intCodeListe").Value = intCodeListe
                    .Parameters("@intCodeKey").Value = intCodeKey
                    .ExecuteNonQuery()

                    egswErrorType = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                    If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception
                Next
            End With

            If Not stCaller Is Nothing Then
                stCaller = st
            Else
                st.Commit()
            End If

            cn.Close()
            cn.Dispose()
            L_ErrCode = egswErrorType
        Catch ex As Exception
            If Not stCaller Is Nothing Then
                Throw New Exception(ex.Message, ex)
            Else
                st.Rollback()
            End If
            cmd.Dispose()
        End Try

        If L_AppType = enumAppType.WebApp Then cn.Close()
        cmd.Dispose()
        Return L_ErrCode
    End Function


    Private Function RemoveListeDetailsItem2(ByVal arrItems As List(Of List(Of String)), ByVal intCodeListe As Integer) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataSet
        Dim st As SqlTransaction
        Dim intCounter As Integer
        Dim intLastIndex As Integer = arrItems.Count - 1
        If intLastIndex = -1 Then Exit Function
        Dim egswErrorType As enumEgswErrorCode = enumEgswErrorCode.GeneralError
        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_egswDetailsDeleteItem"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intIDDetails", SqlDbType.Int)
                .Parameters.Add("@intStep", SqlDbType.Int) 'JTOC 26.11.2012
                .Parameters.Add("@intCodeListe", SqlDbType.Int) 'JTOC 26.11.2012
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                cn.Open()
                st = cn.BeginTransaction
                .Transaction = st

                Dim intCodeKey As Integer
                Dim intStep As Integer
                For intCounter = 0 To intLastIndex
                    intCodeKey = CInt(arrItems(intCounter)(0))
                    intStep = CInt(arrItems(intCounter)(1))
                    .Parameters("@intIDDetails").Value = intCodeKey
                    .Parameters("@intStep").Value = intStep
                    .Parameters("@intCodeListe").Value = intCodeListe
                    .ExecuteNonQuery()

                    egswErrorType = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                    If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception
                Next
            End With

            st.Commit()
            cn.Close()
            cn.Dispose()
            L_ErrCode = egswErrorType
        Catch ex As Exception
            st.Rollback()
            cn.Close()
            cn.Dispose()
            L_ErrCode = egswErrorType
        End Try
    End Function

    Private Function RemoveListeDetailsItem(ByVal arrItems As ArrayList) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataSet
        Dim st As SqlTransaction
        Dim intCounter As Integer
        Dim intLastIndex As Integer = arrItems.Count - 1
        If intLastIndex = -1 Then Exit Function
        Dim egswErrorType As enumEgswErrorCode = enumEgswErrorCode.GeneralError
        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_egswDetailsDeleteItem"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intIDDetails", SqlDbType.Int)
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                cn.Open()
                st = cn.BeginTransaction
                .Transaction = st

                Dim intCodeKey As Integer
                For intCounter = 0 To intLastIndex
                    intCodeKey = CInt(arrItems(intCounter))
                    .Parameters("@intIDDetails").Value = intCodeKey
                    .ExecuteNonQuery()

                    egswErrorType = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                    If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception
                Next
            End With

            st.Commit()
            cn.Close()
            cn.Dispose()
            L_ErrCode = egswErrorType
        Catch ex As Exception
            st.Rollback()
            cn.Close()
            cn.Dispose()
            L_ErrCode = egswErrorType
        End Try
    End Function

    ' MRC: 01/24/08 - For Recipe Encoding
    Public Function RemoveListeDetailsByID(ByVal intID As Integer, Optional ByVal intPositionToLast As Integer = -1) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_EgswListeIngredientDeleteByID"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intPosition", SqlDbType.Int).Value = intPositionToLast
                .Parameters.Add("@intFirstcode", SqlDbType.Int).Value = intID
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                cn.Open()
                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            cn.Close()
            cn.Dispose()
            Throw New Exception(ex.Message, ex)
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function RemoveListeDetails(ByVal intFirstCode As Integer, ByVal intPosition As Integer, ByVal strName As String, ByVal strNumber As String, ByVal intCodeTrans As Integer) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_egswListeIngredientDeleteIfNotMatch"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(strName)
                .Parameters.Add("@intPosition", SqlDbType.Int).Value = intPosition
                .Parameters.Add("@intFirstCode", SqlDbType.Int).Value = intFirstCode
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                cn.Open()
                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            cn.Close()
            cn.Dispose()
            Throw New Exception(ex.Message, ex)
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function RemoveListeDetails(ByVal intFirstCode As Integer, ByVal intPositionToLast As Integer) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_egswListeIngredientDeleteByPositionToLast"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intPosition", SqlDbType.Int).Value = intPositionToLast
                .Parameters.Add("@intFirstcode", SqlDbType.Int).Value = intFirstCode
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                cn.Open()
                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            cn.Close()
            cn.Dispose()
            Throw New Exception(ex.Message, ex)
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    'MRC  051908
    Public Function RemoveListeDetailsStep(ByVal intCode As Integer, ByVal intStep As Integer) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_EgswDetailsStepDelete"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@intStep", SqlDbType.Int).Value = intStep
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                cn.Open()
                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            cn.Close()
            cn.Dispose()
            Throw New Exception(ex.Message, ex)
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function


    '--- VRP 16.10.2007
    Public Function fctRemoveBreadcrumbs(ByVal intID As Integer, Optional ByVal intCodeUser As Integer = 0) As enumEgswErrorCode
        Dim sqlCn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            Dim arrParam(1) As SqlParameter
            arrParam(0) = New SqlParameter("@intID", intID)
            arrParam(1) = New SqlParameter("@intCodeUser", intCodeUser)

            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswBreadcrumbsDelete", arrParam)
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function '------

    '--- VRP 26.03.2008
    Public Function fctRemoveProcedureTemplate(ByVal intCode As String) As enumEgswErrorCode
        Dim sqlCn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = sqlCn
                .CommandText = "sp_EgswRemoveProcedureTemplate"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                sqlCn.Open()
                .ExecuteNonQuery()
            End With
            sqlCn.Close()
            sqlCn.Dispose()

            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function fctRemoveProcedureStyle(ByVal intCode As String) As enumEgswErrorCode 'VRP 01.07.2008
        Dim sqlCn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = sqlCn
                .CommandText = "sp_EgswRemoveProcedureStyles"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                sqlCn.Open()
                .ExecuteNonQuery()
            End With
            sqlCn.Close()
            sqlCn.Dispose()

            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

#End Region

#Region " Update Function "

    Public Function UpdateListeAllergensDerived(ByVal intCodeListe As Integer) As enumEgswErrorCode
        Dim sqlCn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand
        With sqlCmd
            .Connection = sqlCn
            .CommandType = CommandType.StoredProcedure
            .CommandText = "sp_EgswAllergensUpdateDerived"
            .Parameters.Add("@intFirstCode", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
        End With
        Try
            ExecuteFetchType(L_bytFetchType, sqlCmd)
            Return CType(sqlCmd.Parameters("@return").Value, enumEgswErrorCode)
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function UpdateMerchandiseAllAllergens(ByVal intCodeListe As Integer) As enumEgswErrorCode
        Dim sqlCn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand
        With sqlCmd
            .Connection = sqlCn
            .CommandType = CommandType.StoredProcedure
            .CommandText = "sp_EgswListeMerchandiseUpdateAllAllergensDerived"
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
        End With
        Try
            ExecuteFetchType(L_bytFetchType, sqlCmd)
            Return CType(sqlCmd.Parameters("@return").Value, enumEgswErrorCode)
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try

    End Function

    Public Function UpdateListeAllergens(ByVal intCodeListe As Integer, ByVal strCodeAllergenList As String, Optional strAllergenXml As String = "") As enumEgswErrorCode
        Dim sqlCn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand

        With sqlCmd
            .Connection = sqlCn
            .CommandText = "sp_EgswListeAllergensUpdate"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@nvcAllergenCodes", SqlDbType.NVarChar, 4000).Value = ReplaceSpecialCharacters(strCodeAllergenList)
            .Parameters.Add("@nvcAllergenXml", SqlDbType.Xml).Value = strAllergenXml
            .Parameters.Add("@return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
        End With
        Try
            ExecuteFetchType(L_bytFetchType, sqlCmd)
            Return CType(sqlCmd.Parameters("@return").Value, enumEgswErrorCode)
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    ''' <summary>
    ''' Liste mass mutation 
    ''' </summary>
    ''' <param name="udtUser"></param>
    ''' <param name="strCodeListeList"></param>
    ''' <param name="intCodeSetPrice"></param>
    ''' <param name="ApprovedPriceNew"></param>
    ''' <param name="blnCheckStatusOnly"></param>
    ''' <param name="intCodeReplace"></param>
    ''' <param name="rolelevel"></param>
    ''' <param name="dtStatus"></param>
    ''' <param name="IsApprovalRequired">Returns true if approval is required, otherwise, false.</param>
    ''' <param name="fnc"></param>
    ''' <param name="strCodeSiteList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateListeMassMutation(ByVal udtUser As structUser, ByVal strCodeListeList As String, ByVal intCodeSetPrice As Integer, ByVal ApprovedPriceNew As Double, ByVal blnCheckStatusOnly As Boolean, ByVal intCodeReplace As Integer, ByVal rolelevel As enumGroupLevel, ByRef dtStatus As DataTable, ByVal fnc As UserRightsFunction, ByVal strCodeSiteList As String, ByRef IsApprovalRequired As Boolean) As enumEgswErrorCode
        Dim arrParam(12) As SqlParameter
        Dim intIDMain As Integer = -1

        If strCodeListeList.Length > 5000 Then
            intIDMain = fctSaveToTempList(strCodeListeList, udtUser.Code)
        End If

        arrParam(0) = New SqlParameter("@vchCodeListeList", SqlDbType.VarChar, 8000)
        arrParam(0).Value = strCodeListeList
        arrParam(1) = New SqlParameter("@intCodeUser", udtUser.Code)
        arrParam(2) = New SqlParameter("@intCodeSetPrice", intCodeSetPrice)
        arrParam(3) = New SqlParameter("@fltApprovedPriceNew", ApprovedPriceNew)
        arrParam(4) = New SqlParameter("@CheckStatusOnly", blnCheckStatusOnly)
        arrParam(5) = New SqlParameter("@intCodeReplace", intCodeReplace)
        arrParam(6) = New SqlParameter("@intRoleLevel", rolelevel)
        arrParam(7) = New SqlParameter("@retval", SqlDbType.Int)
        arrParam(7).Direction = ParameterDirection.ReturnValue
        arrParam(8) = New SqlParameter("@tntFunction", fnc)
        arrParam(9) = New SqlParameter("@vchCodeSiteList", SqlDbType.VarChar, 8000)
        arrParam(9).Value = strCodeSiteList
        arrParam(10) = New SqlParameter("@intCodeTrans", udtUser.CodeTrans)
        arrParam(11) = New SqlParameter("@IDMain", intIDMain)
        arrParam(12) = New SqlParameter("@IsApprovalRequired", SqlDbType.Bit)
        arrParam(12).Direction = ParameterDirection.Output
        Try
            dtStatus = ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "sp_EgswListeUpdateMassMutationWithApproval", arrParam, 1200).Tables(0)
            IsApprovalRequired = CBool(arrParam(12).Value)
            Return CType(arrParam(7).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function UpdateListeMassProtect(ByVal udtUser As structUser, ByVal strCodeListeList As String, ByVal blnCheckStatusOnly As Boolean, ByVal rolelevel As enumGroupLevel, ByRef dtStatus As DataTable, ByVal fnc As UserRightsFunction, ByVal strCodeSiteList As String, Optional ByVal strNote As String = "", Optional ByVal strComment As String = "") As enumEgswErrorCode
        Dim arrParam(10) As SqlParameter
        Dim intIDMain As Integer = -1

        If strCodeListeList.Length > 5000 Then
            intIDMain = fctSaveToTempList(strCodeListeList, udtUser.Code)
        End If

        arrParam(0) = New SqlParameter("@vchCodeListeList", SqlDbType.VarChar, 8000)
        arrParam(0).Value = strCodeListeList
        arrParam(1) = New SqlParameter("@intCodeUser", udtUser.Code)
        arrParam(2) = New SqlParameter("@CheckStatusOnly", blnCheckStatusOnly)
        arrParam(3) = New SqlParameter("@intRoleLevel", rolelevel)
        arrParam(4) = New SqlParameter("@retval", SqlDbType.Int)
        arrParam(4).Direction = ParameterDirection.ReturnValue
        arrParam(5) = New SqlParameter("@tntFunction", fnc)
        arrParam(6) = New SqlParameter("@vchCodeSiteList", SqlDbType.VarChar, 8000)
        arrParam(6).Value = strCodeSiteList
        arrParam(7) = New SqlParameter("@intCodeTrans", udtUser.CodeTrans)
        arrParam(8) = New SqlParameter("@IDMain", intIDMain)
        arrParam(9) = New SqlParameter("@nvcNote", strNote)
        arrParam(10) = New SqlParameter("@nvcComment", strComment)
        Try
            dtStatus = ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "sp_EgswListeUpdateMassProtect", arrParam, 1200).Tables(0)
            Return CType(arrParam(4).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function fctSaveToTempList(ByVal strCodelist As String, ByVal intUser As Integer) As Integer
        Dim intIDMAIN As Integer
        Dim intReturn As Integer

        strCodelist = strCodelist.Replace("(", "")
        strCodelist = strCodelist.Replace(")", "")

        Dim arrCodeList As New ArrayList(strCodelist.Split(CChar(",")))

        '----- save main -----
        intReturn = MarkedListeMain(intUser, intIDMAIN)
        intReturn = MarkedListeDetails(intIDMAIN, arrCodeList)

        Return intIDMAIN
    End Function

    'VRP 08.23.2007
    Public Function MarkedListeMain(ByVal intCodeUser As Integer, ByRef intID As Integer) As enumEgswErrorCode
        Dim sqlCn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand

        Try
            With sqlCmd
                .Connection = sqlCn
                .CommandText = "sp_EgsW_TempMarkMain"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@IDMain", SqlDbType.Int).Direction = ParameterDirection.Output
                sqlCn.Open()
                .ExecuteNonQuery()
                intID = CInt(.Parameters("@IDMain").Value)
                sqlCn.Close()
                sqlCn.Dispose()
                Return enumEgswErrorCode.OK
            End With
        Catch ex As Exception
            sqlCn.Close()
            sqlCn.Dispose()
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function MarkedListeDetails(ByVal intID As Integer, ByVal arrCodeList As ArrayList) As enumEgswErrorCode
        Dim sqlCn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand
        Dim intCodeListe As Integer
        Dim i As Integer

        Try
            With sqlCmd
                .Connection = sqlCn
                .CommandText = "sp_EgsW_TempMarkDetails"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@IDMain", SqlDbType.Int)
                .Parameters.Add("@CodeListe", SqlDbType.Int)
                sqlCn.Open()

                For i = 0 To arrCodeList.Count - 1
                    If IsNumeric(arrCodeList(i)) Then
                        intCodeListe = CInt(arrCodeList(i))
                        .Parameters("@IDMain").Value = intID
                        .Parameters("@CodeListe").Value = intCodeListe
                        .ExecuteNonQuery()
                    End If
                Next i
                sqlCn.Close()
                sqlCn.Dispose()
                Return enumEgswErrorCode.OK
            End With
        Catch ex As Exception
            sqlCn.Close()
            sqlCn.Dispose()
            Return enumEgswErrorCode.GeneralError
        End Try

    End Function

    Public Function StandardizeListe(ByVal intCodeSite As Integer, ByVal eFormat As enumEgswStandardizationFormat, ByVal eListeType As enumDataListItemType, ByVal intCodeUser As Integer) As enumEgswErrorCode
        Dim arrParam(5) As SqlParameter

        arrParam(0) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@tntFormat", eFormat)
        arrParam(2) = New SqlParameter("@tntType", eListeType)
        arrParam(3) = New SqlParameter("@tntListType", 64)
        arrParam(4) = New SqlParameter("@intCodeUser", intCodeUser)
        arrParam(5) = New SqlParameter("@retval", SqlDbType.Int)

        arrParam(5).Direction = ParameterDirection.ReturnValue

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswItemStandardizeAll", arrParam)
            Return CType(arrParam(5).Value, enumEgswErrorCode)
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
            Throw ex
        End Try
    End Function

    ' RDC 03.15.2013 - Recipe and Merchandise name standardization
    Public Function StandardizeMerchRecipeName(ByVal lngCodeUser As Int32, ByVal lngCodeSite As Int32, ByVal eListeType As enumDataListItemType, _
                                               ByVal eItemListType As enumDataListType, ByVal eFormat As enumEgswStandardizationFormat) As enumEgswErrorCode

        Dim cmd As New SqlCommand

        Try
            With cmd
                'If L_AppType = enumAppType.WebApp Then
                '    .Connection = New SqlConnection(GetConnection("dsn"))
                'Else
                '    .Connection = L_Cnn
                'End If
                .Connection = New SqlConnection(L_strCnn)
                .CommandTimeout = 0
                .CommandText = "sp_EgswItemStandardizeAll"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@retval", SqlDbType.Int)
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = lngCodeUser
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = lngCodeSite
                .Parameters.Add("@tntFormat", SqlDbType.TinyInt).Value = eFormat
                .Parameters.Add("@tntType", SqlDbType.TinyInt).Value = eListeType
                .Parameters.Add("@tntListType", SqlDbType.TinyInt).Value = eItemListType

                .Parameters("@retval").Direction = ParameterDirection.ReturnValue
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With

        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
        cmd.Dispose()
        Return L_ErrCode

    End Function

    Public Function UpdateListeSharing(ByVal intCode As Integer, ByVal blnGlobal As Boolean, ByVal strCodeSiteList As String, ByVal type As ShareType, ByVal intCodeUser As Integer, ByVal intCodeSite As Integer) As enumEgswErrorCode
        If strCodeSiteList = "" Then Return enumEgswErrorCode.OK

        If type = 6 Then
            type = 5
        End If
        Dim arrParam(8) As SqlParameter

        arrParam(0) = New SqlParameter("@intCode", intCode)
        arrParam(1) = New SqlParameter("@IsGlobal", blnGlobal)
        arrParam(2) = New SqlParameter("@intCodeUser", intCodeUser)
        arrParam(3) = New SqlParameter("@intCodeEgsTable", 50) 'temporary
        arrParam(4) = New SqlParameter("@intCodeSite", intCodeSite)
        arrParam(5) = New SqlParameter("@ShareItem", True)
        arrParam(6) = New SqlParameter("@txtCodeSiteList", strCodeSiteList)
        arrParam(7) = New SqlParameter("@sntShareType", type)
        arrParam(8) = New SqlParameter("@retval", SqlDbType.Int)
        arrParam(8).Direction = ParameterDirection.ReturnValue


        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswItemShare", arrParam)
            Return CType(arrParam(8).Value, enumEgswErrorCode)
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
            Throw ex
        End Try
    End Function

    Public Function UpdateListePicture(ByVal intCode As Integer, ByVal strPictureName As String) As enumEgswErrorCode
        Dim arrParam(2) As SqlParameter

        arrParam(0) = New SqlParameter("@intCode", intCode)
        arrParam(1) = New SqlParameter("@sPictureName", strPictureName)
        arrParam(2) = New SqlParameter("@retval", SqlDbType.Int)
        arrParam(2).Direction = ParameterDirection.ReturnValue
        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswSaveListPicture", arrParam)
            Return CType(arrParam(2).Value, enumEgswErrorCode)
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
            Throw ex
        End Try
    End Function

    Public Function UpdateListeSharingItems(ByVal intCodeListe As Integer, ByVal blnIsGlobal As Boolean, ByVal intCodeUser As Integer) As enumEgswErrorCode
        Dim arrParam(3) As SqlParameter

        arrParam(0) = New SqlParameter("@intCodeListe", intCodeListe)
        arrParam(1) = New SqlParameter("@intCodeUserOwner", intCodeUser)
        arrParam(2) = New SqlParameter("@IsGlobal", blnIsGlobal)
        arrParam(3) = New SqlParameter("@retval", SqlDbType.Int)

        arrParam(3).Direction = ParameterDirection.ReturnValue

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswListeItemsShare", arrParam)
            Return CType(arrParam(3).Value, enumEgswErrorCode)
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
            Throw ex
        End Try
    End Function

    Public Function UpdateListeMerge(ByVal arrOldValues As ArrayList, ByVal intCodeNew As Integer, ByVal intCodeUser As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Dim st As SqlTransaction
        Dim nLastIndex As Integer = arrOldValues.Count - 1
        Dim counter As Integer
        Dim intCode As Integer

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswListeUpdateMerge"
                .CommandType = CommandType.StoredProcedure

                .Connection.Open()
                st = .Connection.BeginTransaction
                .Transaction = st

                .Parameters.Add("@intCodeListeOld", SqlDbType.Int)
                .Parameters.Add("@intCodeListeNew", SqlDbType.Int)
                .Parameters.Add("@intCodeUser", SqlDbType.Int)
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                For counter = 0 To nLastIndex
                    intCode = CInt(arrOldValues.Item(counter))
                    .Parameters.Item("@intCodeListeOld").Value = intCode
                    .Parameters.Item("@intCodeListeNew").Value = intCodeNew
                    .Parameters.Item("@intCodeUser").Value = intCodeUser

                    .ExecuteNonQuery()
                    L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                    If L_ErrCode <> enumEgswErrorCode.OK Then Throw New Exception
                Next

                st.Commit()
                .Connection.Close()
            End With
        Catch ex As Exception
            st.Rollback()
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateListeHistory(ByVal intCodeListe As Integer, ByVal intCodeUserID As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_egswListeHistoryUpdate"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeUserID", SqlDbType.Int).Value = intCodeUserID
                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                .ExecuteNonQuery()
                cmd.Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function
    Public Function UpdateMenuPLanHistory(ByVal CodeMenuPlan As Integer, ByVal CodeUser As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswMenuPlanHistoryUpdate"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeUserID", SqlDbType.Int).Value = CodeUser
                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = CodeMenuPlan
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                .ExecuteNonQuery()
                cmd.Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateListeMealInfo(ByVal intCodeListe As Integer, ByVal dteDateUse As Date, ByVal dblYieldPrep As Double, ByVal dblYieldLeft As Double, ByVal dblYieldReturn As Double, ByVal dblYieldLost As Double, ByVal dblYieldSold As Double, ByVal dblYieldSoldSpec As Double, ByVal intTax1 As Integer, ByVal intTax2 As Integer, ByVal dblTotalPrice As Double, ByVal dblSellingPrice As Double, ByVal strNoteMeal As String) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_egswListeMealUpdate"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@dteDateUse", SqlDbType.DateTime).Value = dteDateUse
                .Parameters.Add("@fltYieldPrep", SqlDbType.Float).Value = dblYieldPrep
                .Parameters.Add("@fltYieldLeft", SqlDbType.Float).Value = dblYieldLeft
                .Parameters.Add("@fltYieldReturn", SqlDbType.Float).Value = dblYieldReturn
                .Parameters.Add("@fltYieldLost", SqlDbType.Float).Value = dblYieldLost
                .Parameters.Add("@fltYieldSold", SqlDbType.Float).Value = dblYieldSold
                .Parameters.Add("@fltYieldSoldSpec", SqlDbType.Float).Value = dblYieldSoldSpec
                .Parameters.Add("@intTax1", SqlDbType.Int).Value = intTax1
                .Parameters.Add("@intTax2", SqlDbType.Int).Value = intTax2
                .Parameters.Add("@fltTotalPrice", SqlDbType.Float).Value = dblTotalPrice
                .Parameters.Add("@fltSellingPrice", SqlDbType.Float).Value = dblSellingPrice
                .Parameters.Add("@nvcNoteMeal", SqlDbType.NVarChar, 2000).Value = ReplaceSpecialCharacters(strNoteMeal)
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    ''' <summary>
    ''' Replace ingredient in recipe or menu
    ''' </summary>
    ''' <param name="intCodeListeOld"></param>
    ''' <param name="intcodeListeNew"></param>
    ''' <param name="ReplaceInListeType"></param>
    ''' <param name="intCodeTrans"></param>
    ''' <param name="dtMissingUnits"></param>
    ''' <param name="udtUser"></param>
    ''' <param name="IsRequiredApproval"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateListeReplaceIngredient(ByVal requestType As enumRequestType, ByVal intCodeListeOld As Integer, ByVal intcodeListeNew As Integer, ByVal ReplaceInListeType As enumDataListItemType, ByVal intCodeTrans As Integer, ByRef dtMissingUnits As DataTable, ByVal udtUser As structUser, ByRef IsRequiredApproval As Boolean, ByVal intCodeSite As Integer, Optional ByVal strSelectedListe As String = "", _
                Optional ByVal strComplement As String = "", Optional ByVal strPreparation As String = "", Optional ByVal strAlternativeIngr As String = "", Optional ByVal intGLobal As Integer = 0) As enumEgswErrorCode
        Try
            Dim arrParam(9) As SqlParameter
            arrParam(0) = New SqlParameter("@retVal", "")
            arrParam(0).Direction = ParameterDirection.ReturnValue
            arrParam(1) = New SqlParameter("@intCodeListeOld", intCodeListeOld)
            arrParam(2) = New SqlParameter("@intCodeListeNew", intcodeListeNew)
            arrParam(3) = New SqlParameter("@ReplaceInListeType", ReplaceInListeType)
            arrParam(4) = New SqlParameter("@intCodeTrans", intCodeTrans)
            arrParam(5) = New SqlParameter("@intCodeUser", udtUser.Code)
            arrParam(6) = New SqlParameter("@intRequestType", requestType)
            arrParam(7) = New SqlParameter("@intRoleLevel", udtUser.RoleLevelHighest)
            arrParam(8) = New SqlParameter("@IsApprovalRequired", SqlDbType.Bit)
            arrParam(8).Direction = ParameterDirection.Output
            arrParam(9) = New SqlParameter("@intCodeSite", intCodeSite)

            '---JBB July 16, 2010
            If strSelectedListe <> "" Or _
             strComplement <> "" Or _
             strPreparation <> "" Or _
             strAlternativeIngr <> "" Or _
            intGLobal <> 0 Then
                ReDim Preserve arrParam(14) '(10)
                arrParam(10) = New SqlParameter("@vcrSelectedListe", SqlDbType.VarChar, 8000)
                arrParam(10).Value = strSelectedListe

                '// DRR 05.22.2012
                arrParam(11) = New SqlParameter("@ncharComplement", SqlDbType.NVarChar, 2000)
                arrParam(11).Value = strComplement

                arrParam(12) = New SqlParameter("@ncharPreparation", SqlDbType.NVarChar, 2000)
                arrParam(12).Value = strPreparation

                arrParam(13) = New SqlParameter("@ncharAlternativeIngr", SqlDbType.NVarChar, 300)
                arrParam(13).Value = strAlternativeIngr

                'NBG 9.17.2015 Add Global
                arrParam(14) = New SqlParameter("@bitGlobal", SqlDbType.Bit)
                arrParam(14).Value = intGLobal
            End If
            '---

            Dim ds As DataSet = ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "sp_EgswListeUpdateReplaceIngredientWithApproval", arrParam)
            If ds.Tables.Count = 1 Then
                dtMissingUnits = ds.Tables(0)
            End If

            IsRequiredApproval = CBool(arrParam(8).Value)

            Return CType(arrParam(0).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try

    End Function

    Public Function UpdateListeReplaceIngredientForMerchandise(ByVal requestType As enumRequestType, ByVal intCodeListeOld As Integer, ByVal intcodeListeNew As Integer, ByVal ReplaceInListeType As enumDataListItemType, ByVal intCodeTrans As Integer, ByRef dtMissingUnits As DataTable, ByVal udtUser As structUser, ByRef IsRequiredApproval As Boolean, ByVal intCodeSite As Integer, Optional ByVal strSelectedListe As String = "", _
              Optional ByVal strComplement As String = "", Optional ByVal strPreparation As String = "", Optional ByVal strAlternativeIngr As String = "", Optional ByVal intGLobal As Integer = 0) As enumEgswErrorCode
        Try
            Dim arrParam(9) As SqlParameter
            arrParam(0) = New SqlParameter("@retVal", "")
            arrParam(0).Direction = ParameterDirection.ReturnValue
            arrParam(1) = New SqlParameter("@intCodeListeOld", intCodeListeOld)
            arrParam(2) = New SqlParameter("@intCodeListeNew", intcodeListeNew)
            arrParam(3) = New SqlParameter("@ReplaceInListeType", ReplaceInListeType)
            arrParam(4) = New SqlParameter("@intCodeTrans", intCodeTrans)
            arrParam(5) = New SqlParameter("@intCodeUser", udtUser.Code)
            arrParam(6) = New SqlParameter("@intRequestType", requestType)
            arrParam(7) = New SqlParameter("@intRoleLevel", udtUser.RoleLevelHighest)
            arrParam(8) = New SqlParameter("@IsApprovalRequired", SqlDbType.Bit)
            arrParam(8).Direction = ParameterDirection.Output
            arrParam(9) = New SqlParameter("@intCodeSite", intCodeSite)

            '---JBB July 16, 2010
            If strSelectedListe <> "" Or _
             strComplement <> "" Or _
             strPreparation <> "" Or _
             strAlternativeIngr <> "" Or _
            intGLobal <> 0 Then
                ReDim Preserve arrParam(15) '(10)
                arrParam(11) = New SqlParameter("@vcrSelectedListe", SqlDbType.VarChar, 8000)
                arrParam(11).Value = strSelectedListe

                '// DRR 05.22.2012
                arrParam(12) = New SqlParameter("@ncharComplement", SqlDbType.NVarChar, 2000)
                arrParam(12).Value = strComplement

                arrParam(13) = New SqlParameter("@ncharPreparation", SqlDbType.NVarChar, 2000)
                arrParam(13).Value = strPreparation

                arrParam(14) = New SqlParameter("@ncharAlternativeIngr", SqlDbType.NVarChar, 300)
                arrParam(14).Value = strAlternativeIngr

                'NBG 9.17.2015 Add Global
                arrParam(15) = New SqlParameter("@bitGlobal", SqlDbType.Bit)
                arrParam(15).Value = intGLobal
            End If
            '---

            Dim ds As DataSet = ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "sp_EgswListeUpdateReplaceIngredientWithApprovalForMerchandise", arrParam)
            If ds.Tables.Count = 1 Then
                dtMissingUnits = ds.Tables(0)
            End If

            IsRequiredApproval = CBool(arrParam(8).Value)

            Return CType(arrParam(0).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try

    End Function


    Public Function ValidateUnitsReplaceIngredient(ByVal strCodeListeOld As String, ByVal intcodeListeNew As Integer, ByVal intCodeTrans As Integer, ByVal bIncludeRecipe As Boolean, ByVal bIncludeMenu As Boolean) As DataTable
        Try
            Dim arrParam(4) As SqlParameter
            arrParam(0) = New SqlParameter("@sCodeListeListOld", strCodeListeOld)
            arrParam(1) = New SqlParameter("@intCodeListeNew", intcodeListeNew)
            arrParam(2) = New SqlParameter("@intCodeTrans", intCodeTrans)
            arrParam(3) = New SqlParameter("@includeRecipe", bIncludeRecipe)
            arrParam(4) = New SqlParameter("@includeMenu", bIncludeMenu)

            Dim ds As DataSet = ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "sp_egsWListeReplaceValidateUnits", arrParam)
            Dim dtMissingUnits As New DataTable
            If ds.Tables.Count = 1 Then
                dtMissingUnits = ds.Tables(0)
            End If

            Return dtMissingUnits
        Catch ex As Exception
            Throw ex
        End Try

    End Function

    Public Function UpdateListeChangeSrWeight(ByVal intCodeListe As Integer, ByVal dblSrWeight As Double) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswListeUpdateChangeSrWeight"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@fltSrWeight", SqlDbType.Float).Value = dblSrWeight
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateListeSalesHistory(ByVal intCodeListe As Integer, ByVal intCodeUser As Integer, ByVal dteDateSold As Date, ByVal dblQty As Double) As enumEgswErrorCode
        Dim arrParam(4) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeListe", intCodeListe)
        arrParam(1) = New SqlParameter("@intCodeUser", intCodeUser)
        arrParam(2) = New SqlParameter("@dteDateSold", dteDateSold)
        arrParam(3) = New SqlParameter("@fltQty", dblQty)
        arrParam(4) = New SqlParameter("@retval", SqlDbType.Int)

        arrParam(4).Direction = ParameterDirection.ReturnValue

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswListeSalesHistoryUpdate", arrParam)
            L_ErrCode = CType(arrParam(4).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function UpdateListeRecomputeRatioNut(ByVal intCodeListe As Integer, Optional ByRef stCaller As SqlTransaction = Nothing, Optional ByVal intCodeSetPrice As Integer = -1) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                If stCaller Is Nothing Then
                    .Connection = New SqlConnection(L_strCnn)
                Else
                    .Connection = stCaller.Connection
                    .Transaction = stCaller
                End If

                .CommandText = "sp_EgswListeSetPriceUpdateRecomputeRatioNut"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeListe", SqlDbType.Float).Value = intCodeListe
                .Parameters.Add("@SelectedCodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                If stCaller Is Nothing Then .Connection.Open()
                .ExecuteNonQuery()
                If stCaller Is Nothing Then .Connection.Close()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If stCaller Is Nothing AndAlso cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateListeSetPrice(ByVal blnRowUpdatePrice As Boolean, ByVal intRowCodeListe As Integer, ByVal intRowCodeSetprice As Integer, ByVal intRowCodeUnit As Integer, ByVal intRowID As Integer, ByVal intRowPosition As Integer, ByVal dblRowPrice As Double, ByVal dblRowRatio As Double, ByVal dblRowRatioNut As Double, ByVal intTax As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Dim st As SqlTransaction
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_egswListeSetPriceUpdate"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intID", SqlDbType.Int).Value = intRowID
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intRowCodeSetprice
                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intRowCodeListe
                .Parameters.Add("@intCodeUnit", SqlDbType.Int).Value = intRowCodeUnit
                .Parameters.Add("@fltPrice", SqlDbType.Float).Value = dblRowPrice
                .Parameters.Add("@intPosition", SqlDbType.Int).Value = intRowPosition
                .Parameters.Add("@fltRatio", SqlDbType.Float).Value = dblRowRatio
                .Parameters.Add("@fltRatioNut", SqlDbType.Float).Value = dblRowRatioNut
                .Parameters.Add("@vchFnc", SqlDbType.VarChar, 20)
                .Parameters.Add("@intTax", SqlDbType.Int).Value = intTax
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                blnRowUpdatePrice = True

                If blnRowUpdatePrice Then
                    .Parameters("@vchFnc").Value = "UPDATEPRICE"
                Else
                    .Parameters("@vchFnc").Value = ""
                End If

                .Connection.Open()
                st = cmd.Connection.BeginTransaction
                .Transaction = st
                .ExecuteNonQuery()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                If L_ErrCode <> enumEgswErrorCode.OK Then Throw New Exception

                L_ErrCode = UpdateListeRecomputeRatioNut(intRowCodeListe, st)
                If L_ErrCode <> enumEgswErrorCode.OK Then Throw New Exception

                st.Commit()
                .Connection.Close()
            End With
        Catch ex As Exception
            st.Rollback()
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    'JTOC 20.06.2013 Update SecondCode of all details with the same name
    Public Function UpdateDetailSecondCode(ByVal intCodeListe As Integer, ByVal strName As String, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, ByVal intAdmin As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Dim st As SqlTransaction
        Dim dt As DataTable
        Dim row As DataRow
        Try

            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswUpdateDetailSecondCode"
                .CommandType = CommandType.StoredProcedure

                cmd.Connection.Open()
                st = cmd.Connection.BeginTransaction
                .Transaction = st

                L_bytFetchTypeTemp = L_bytFetchType
                L_bytFetchType = L_bytFetchTypeTemp

                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@strName", SqlDbType.NVarChar, 50).Value = strName
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@intAdmin", SqlDbType.Int).Value = intAdmin
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .ExecuteNonQuery()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                If L_ErrCode <> enumEgswErrorCode.OK Then Throw New Exception


                L_ErrCode = UpdateListeRecomputeRatioNut(intCodeListe, st)
                If L_ErrCode <> enumEgswErrorCode.OK Then Throw New Exception

                st.Commit()
                cmd.Connection.Close()
            End With
        Catch ex As Exception
            st.Rollback()
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function


    'JTOC 24.06.2013 Reject Approval
    Public Function RejectApproval(ByVal strName As String, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Dim st As SqlTransaction
        Dim dt As DataTable
        Dim row As DataRow
        Try

            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswRejectApproval"
                .CommandType = CommandType.StoredProcedure

                cmd.Connection.Open()
                st = cmd.Connection.BeginTransaction
                .Transaction = st

                L_bytFetchTypeTemp = L_bytFetchType
                L_bytFetchType = L_bytFetchTypeTemp

                .Parameters.Add("@strName", SqlDbType.NVarChar, 50).Value = strName
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .ExecuteNonQuery()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                If L_ErrCode <> enumEgswErrorCode.OK Then Throw New Exception

                st.Commit()
                cmd.Connection.Close()
            End With
        Catch ex As Exception
            st.Rollback()
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function


    Public Function UpdateListeSetPrice(ByVal intCodeListe As Integer, ByVal ds As DataSet) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Dim st As SqlTransaction
        Dim dt As DataTable
        Dim row As DataRow
        Try
            Dim dtOldPrice As DataTable
            Dim arrRemoveItems As ArrayList
            Dim intCodeSetPrice As Integer

            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_egswListeSetPriceUpdate"
                .CommandType = CommandType.StoredProcedure

                cmd.Connection.Open()
                st = cmd.Connection.BeginTransaction
                .Transaction = st

                L_bytFetchTypeTemp = L_bytFetchType

                If ds.Tables.Count <> 0 Then
                    dt = ds.Tables(0)
                    intCodeSetPrice = CInt(dt.TableName)
                    L_bytFetchType = enumEgswFetchType.DataTable
                    dtOldPrice = CType(GetListeSetPrice(intCodeListe, intCodeSetPrice, -1), DataTable)
                    arrRemoveItems = GetItemsRemoved("ID", "codesetprice", dtOldPrice, ds.Tables(intCodeSetPrice.ToString))
                    If arrRemoveItems.Count > 0 Then
                        RemoveListePricesItem(arrRemoveItems, st)
                    End If
                End If

                L_bytFetchType = L_bytFetchTypeTemp

                .Parameters.Add("@intID", SqlDbType.Int)
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int)
                .Parameters.Add("@intCodeListe", SqlDbType.Int)
                .Parameters.Add("@intCodeUnit", SqlDbType.Int)
                .Parameters.Add("@fltPrice", SqlDbType.Float)
                .Parameters.Add("@intPosition", SqlDbType.Int)
                .Parameters.Add("@fltRatio", SqlDbType.Float)
                .Parameters.Add("@fltRatioNut", SqlDbType.Float)
                .Parameters.Add("@vchFnc", SqlDbType.VarChar, 20)
                .Parameters.Add("@intTax", SqlDbType.Int)
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                Dim intRowID As Integer
                Dim intRowCodeSetPrice As Integer
                Dim intRowCodeListe As Integer = intCodeListe
                Dim intRowCodeUnit As Integer
                Dim dblRowPrice As Double
                Dim intRowPosition As Integer
                Dim dblRowRatio As Double
                Dim dblRowRatioNut As Double
                Dim intTax As Integer
                Dim bRowUpdatePrice As Boolean

                For Each dt In ds.Tables
                    For Each row In dt.Rows
                        ' Get Values
                        intRowID = CInt(row.Item("ID"))

                        If intCodeListe <> CInt(row.Item("codeliste")) Then intRowID = -1

                        intRowCodeSetPrice = CInt(row.Item("CodeSetPrice"))
                        intRowCodeUnit = CInt(row.Item("unit"))
                        dblRowPrice = CDbl(row.Item("Price"))
                        intRowPosition = CInt(row.Item("Position"))
                        dblRowRatio = CDbl(row.Item("Ratio"))
                        dblRowRatioNut = CDbl(row.Item("RatioNut"))
                        intTax = CInt(row.Item("tax"))

                        'liste weigth save in setprice doesn't have this column
                        If Not dt.Columns.Contains("updateprice") Then
                            bRowUpdatePrice = True
                        Else
                            If IsDBNull(row.Item("UpdatePrice")) Then
                                row.Item("UpdatePrice") = False
                                bRowUpdatePrice = False
                            Else
                                bRowUpdatePrice = CBool(row.Item("UpdatePrice"))
                            End If
                        End If

                        ' Save
                        .Parameters("@intID").Value = intRowID
                        .Parameters("@intCodeListe").Value = intRowCodeListe
                        .Parameters("@intCodeSetPrice").Value = intRowCodeSetPrice
                        .Parameters("@intCodeUnit").Value = intRowCodeUnit
                        .Parameters("@fltPrice").Value = dblRowPrice
                        .Parameters("@intPosition").Value = intRowPosition
                        .Parameters("@fltRatio").Value = dblRowRatio
                        .Parameters("@fltRatioNut").Value = dblRowRatioNut
                        .Parameters("@intTax").Value = intTax

                        If bRowUpdatePrice Then
                            .Parameters("@vchFnc").Value = "UPDATEPRICE"
                        Else
                            .Parameters("@vchFnc").Value = ""
                        End If

                        .ExecuteNonQuery()
                        L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                        If L_ErrCode <> enumEgswErrorCode.OK Then Throw New Exception
                    Next
                Next

                L_ErrCode = UpdateListeRecomputeRatioNut(intCodeListe, st, intCodeSetPrice) 'JTOC 08.21.2013 added intcodesetprice
                If L_ErrCode <> enumEgswErrorCode.OK Then Throw New Exception

                st.Commit()
                cmd.Connection.Close()
            End With
        Catch ex As Exception
            st.Rollback()
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateListeSetPriceChangePrice(ByVal intID As Integer, ByVal dblPrice As Double) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswListeSetPriceUpdateChangePrice"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intID", SqlDbType.Int).Value = intID
                .Parameters.Add("@fltPrice", SqlDbType.Float).Value = dblPrice
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateListeSetPriceCalc(ByVal intCodeListe As Integer, ByVal dt As DataTable) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Dim st As SqlTransaction
        Dim da As New SqlDataAdapter
        Dim row As DataRow
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_egswListeSetPriceCalcUpdate"
                .CommandType = CommandType.StoredProcedure

                cmd.Connection.Open()
                st = cmd.Connection.BeginTransaction
                .Transaction = st

                .Parameters.Add("@intCodeliste", SqlDbType.Int)
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int)
                .Parameters.Add("@fltCoeff", SqlDbType.Float)
                .Parameters.Add("@fltImposedPrice", SqlDbType.Float)
                .Parameters.Add("@fltApprovedPrice", SqlDbType.Float)
                .Parameters.Add("@intTax", SqlDbType.Int)
                .Parameters.Add("@bitUpdateCoeff", SqlDbType.Bit)
                .Parameters.Add("@bitUpdateImposedPrice", SqlDbType.Bit)
                .Parameters.Add("@bitUpdateApprovedPrice", SqlDbType.Bit)
                .Parameters.Add("@bitUpdateTax", SqlDbType.Bit)
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                Dim intRowListe As Integer
                Dim intRowCodeSetprice As Integer
                Dim dblCoeff As Double
                Dim dblImposedPrice As Double
                Dim dblApprovedPrice As Double
                Dim intTax As Double
                Dim blnUpdateCoeff As Boolean
                Dim blnUpdateImposedPrice As Boolean
                Dim blnUpdateApprovedPrice As Boolean
                Dim blnUpdateTax As Boolean

                For Each row In dt.Rows
                    ' Get values
                    intRowListe = intCodeListe
                    intRowCodeSetprice = CInt(row.Item("codesetprice"))
                    dblCoeff = CDbl(row.Item("coeff"))
                    dblImposedPrice = CDbl(row.Item("imposedprice"))
                    dblApprovedPrice = CDbl(row.Item("approvedprice"))
                    intTax = CInt(row.Item("tax"))

                    If Double.IsNaN(dblCoeff) Then dblCoeff = 0
                    If Double.IsNaN(dblImposedPrice) Then dblImposedPrice = 0
                    If Double.IsNaN(dblApprovedPrice) Then dblApprovedPrice = 0

                    If IsDBNull(row.Item("updatecoeff")) Then
                        dblCoeff = 0
                        row.Item("updatecoeff") = False
                    End If
                    If IsDBNull(row.Item("updateimposedprice")) Then
                        dblImposedPrice = 0
                        row.Item("updateimposedprice") = False
                    End If
                    If IsDBNull(row.Item("updateapprovedprice")) Then
                        dblApprovedPrice = 0
                        row.Item("updateapprovedprice") = False
                    End If
                    If IsDBNull(row.Item("updatetax")) Then
                        intTax = 0
                        row.Item("updatetax") = False
                    End If

                    blnUpdateCoeff = CBool(row.Item("updatecoeff"))
                    blnUpdateImposedPrice = CBool(row.Item("updateimposedprice"))
                    blnUpdateApprovedPrice = CBool(row.Item("updateapprovedprice"))
                    blnUpdateTax = CBool(row.Item("updatetax"))

                    ' Save
                    .Parameters("@intCodeliste").Value = intRowListe
                    .Parameters("@intCodeSetPrice").Value = intRowCodeSetprice
                    .Parameters("@fltCoeff").Value = dblCoeff
                    .Parameters("@fltImposedPrice").Value = dblImposedPrice
                    .Parameters("@fltApprovedPrice").Value = dblApprovedPrice
                    .Parameters("@intTax").Value = intTax
                    .Parameters("@bitUpdateCoeff").Value = blnUpdateCoeff
                    .Parameters("@bitUpdateImposedPrice").Value = blnUpdateImposedPrice
                    .Parameters("@bitUpdateApprovedPrice").Value = blnUpdateApprovedPrice
                    .Parameters("@bitUpdateTax").Value = blnUpdateTax
                    .ExecuteNonQuery()

                    L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                    If L_ErrCode <> enumEgswErrorCode.OK Then Throw New Exception
                Next
                st.Commit()
                cmd.Connection.Close()
            End With

        Catch ex As Exception
            st.Rollback()
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateListeSetPriceCalc(ByVal intCodeListe As Integer, _
        ByVal intCodesetPrice As Integer, ByVal dblCoeff As Double, ByVal dblImposedPrice As Double, _
        ByVal dblApprovedPrice As Double, ByVal intTax As Integer, ByVal blnUpdatecoeff As Boolean, ByVal blnUpdateImposedPrice As Boolean, _
        ByVal blnUpdateApprovedPrice As Boolean, ByVal blnUpdateTax As Boolean) As enumEgswErrorCode

        Dim arrParam(10) As SqlParameter

        arrParam(0) = New SqlParameter("@intCodeliste", intCodeListe)
        arrParam(1) = New SqlParameter("@intCodeSetPrice", intCodesetPrice)
        arrParam(2) = New SqlParameter("@fltCoeff", dblCoeff)
        arrParam(3) = New SqlParameter("@fltImposedPrice", dblImposedPrice)
        arrParam(4) = New SqlParameter("@fltApprovedPrice", dblApprovedPrice)
        arrParam(5) = New SqlParameter("@intTax", intTax)
        arrParam(6) = New SqlParameter("@bitUpdateCoeff", blnUpdatecoeff)
        arrParam(7) = New SqlParameter("@bitUpdateImposedPrice", blnUpdateImposedPrice)
        arrParam(8) = New SqlParameter("@bitUpdateApprovedPrice", blnUpdateApprovedPrice)
        arrParam(9) = New SqlParameter("@bitUpdateTax", blnUpdateTax)
        arrParam(10) = New SqlParameter("@retval", SqlDbType.Int)

        arrParam(10).Direction = ParameterDirection.ReturnValue

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_egswListeSetPriceCalcUpdate", arrParam)
            L_ErrCode = CType(arrParam(10).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Public Function UpdateListeKeywords(ByVal intCodeListe As Integer, ByVal arrKeywords As ArrayList, ByVal eListeType As enumDataListItemType, Optional ByVal blnAppend As Boolean = False) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim st As SqlTransaction

        L_bytFetchType = enumEgswFetchType.DataTable
        Dim dtOLDDB As DataTable = CType(GetListeKeyword(intCodeListe, -1, eListeType), DataTable)
        Dim arrRemovedItems As ArrayList = Me.GetItemsRemoved(dtOLDDB, arrKeywords)

        Dim intRowCodeListe As Integer = intCodeListe
        Dim intRowCodeKey As Integer
        Dim intRowDerived As Integer = 0
        Dim intCounter As Integer
        Dim intLastIndex As Integer = arrKeywords.Count - 1
        Dim egswErrorType As enumEgswErrorCode = enumEgswErrorCode.GeneralError

        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_egswKeyDetailsUpdate"
                .CommandType = CommandType.StoredProcedure

                cn.Open()
                st = cn.BeginTransaction

                If Not blnAppend Then
                    egswErrorType = RemoveListeKeyDetailsItem(intCodeListe, arrRemovedItems, st)
                    If egswErrorType <> enumEgswErrorCode.OK Then L_ErrCode = egswErrorType
                End If

                .Transaction = st
                .Parameters.Add("@intCodeListe", SqlDbType.Int)
                .Parameters.Add("@intCodekey", SqlDbType.Int)
                .Parameters.Add("@intDerived", SqlDbType.Int)
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                For intCounter = 0 To intLastIndex
                    ' Get Value
                    intRowCodeKey = CInt(arrKeywords(intCounter))
                    ' Save
                    .Parameters("@intCodeListe").Value = intRowCodeListe
                    .Parameters("@intCodekey").Value = intRowCodeKey
                    .Parameters("@intDerived").Value = intRowDerived
                    .ExecuteNonQuery()

                    egswErrorType = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                    If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception
                Next
                st.Commit()
                cn.Close()
                cn.Dispose()
                L_ErrCode = egswErrorType
            End With
        Catch ex As Exception
            st.Rollback()
            cn.Close()
            cn.Dispose()
            cmd.Dispose()
            m_Err = ex
            L_ErrCode = egswErrorType
        End Try

        If L_AppType = enumAppType.WebApp Then cn.Close()
        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateListeNutrients(ByVal intCodeListe As Integer, ByVal arrNutrients As ArrayList, Optional ByVal strCodeLink As String = "00000", Optional intCodeSet As Integer = 0) As enumEgswErrorCode
        Dim cmd As New SqlCommand

        Try
            ' there should be 14 elements in the array
            Dim intMaxNut As Integer = 14
            Dim intCurrentNut As Integer = arrNutrients.Count - 1
            Dim intFill As Integer = intMaxNut - intCurrentNut

            'comment by ADR 04.27.11
            'If intFill > 0 Then
            '    Dim counter As Integer
            '    For counter = 1 To intFill
            '        arrNutrients.Add(-1)
            '    Next
            'End If

            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_egswNutrientValUpdate"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@vchCodeLink", SqlDbType.VarChar, 20).Value = strCodeLink

                'ADR 04.27.11 - dynamic saving of nutrients
                Dim intVarCtr As Integer = 1

                'AGL 2013.07.19
                Dim reader As New Configuration.AppSettingsReader
                intMaxNut = reader.GetValue("NutrientValuesCount", GetType(Integer))
                If intMaxNut > arrNutrients.Count Then
                    intMaxNut = arrNutrients.Count
                End If

                For i As Integer = 0 To intMaxNut - 1
                    .Parameters.Add("@fltN" & intVarCtr, SqlDbType.Float).Value = CDbl(arrNutrients(i))
                    intVarCtr += 1
                Next
                .Parameters.Add("@CodeSet", SqlDbType.Int).Value = intCodeSet 'AGL 2013.06.27 - added codeset

                'comment by ADR 04.27.11
                '.Parameters.Add("@fltN1", SqlDbType.Float).Value = CDbl(arrNutrients(0))
                '.Parameters.Add("@fltN2", SqlDbType.Float).Value = CDbl(arrNutrients(1))
                '.Parameters.Add("@fltN3", SqlDbType.Float).Value = CDbl(arrNutrients(2))
                '.Parameters.Add("@fltN4", SqlDbType.Float).Value = CDbl(arrNutrients(3))
                '.Parameters.Add("@fltN5", SqlDbType.Float).Value = CDbl(arrNutrients(4))
                '.Parameters.Add("@fltN6", SqlDbType.Float).Value = CDbl(arrNutrients(5))
                '.Parameters.Add("@fltN7", SqlDbType.Float).Value = CDbl(arrNutrients(6))
                '.Parameters.Add("@fltN8", SqlDbType.Float).Value = CDbl(arrNutrients(7))
                '.Parameters.Add("@fltN9", SqlDbType.Float).Value = CDbl(arrNutrients(8))
                '.Parameters.Add("@fltN10", SqlDbType.Float).Value = CDbl(arrNutrients(9))
                '.Parameters.Add("@fltN11", SqlDbType.Float).Value = CDbl(arrNutrients(10))
                '.Parameters.Add("@fltN12", SqlDbType.Float).Value = CDbl(arrNutrients(11))
                '.Parameters.Add("@fltN13", SqlDbType.Float).Value = CDbl(arrNutrients(12))
                '.Parameters.Add("@fltN14", SqlDbType.Float).Value = CDbl(arrNutrients(13))
                '.Parameters.Add("@fltN15", SqlDbType.Float).Value = CDbl(arrNutrients(14))

                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateListeNutrientsImposed(ByVal intCodeListe As Integer, ByVal arrNutrients As ArrayList, ByVal arrNutrientsImposed As ArrayList, ByVal intImposedType As Integer, ByVal strPortionSize As String, ByVal blnDisplayNutrition As Boolean, Optional ByVal strCodeLink As String = "00000", Optional ByVal strNutritionBasis As String = "") As enumEgswErrorCode
        Dim cmd As New SqlCommand

        Try
            ' there should be 14 elements in the array
            Dim intMaxNut As Integer = 14
            Dim intCurrentNut As Integer = arrNutrientsImposed.Count - 1
            Dim intFill As Integer = intMaxNut - intCurrentNut

            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[sp_EgswNutrientValUpdateRecipeImposed]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@CodeLink", SqlDbType.VarChar, 20).Value = strCodeLink

                Dim intCtr As Integer = 1
                For Each nutrient In arrNutrients
                    .Parameters.Add("@N" & intCtr, SqlDbType.Float).Value = CDblDB(nutrient)
                    intCtr += 1
                Next


                For Each nutrientinfo As structNutrientInfo In arrNutrientsImposed
                    .Parameters.Add("@N" & nutrientinfo.Position & "Impose", SqlDbType.Float).Value = CDblDB(nutrientinfo.Value)
                Next

                For Each nutrientinfo As structNutrientInfo In arrNutrientsImposed
                    .Parameters.Add("@N" & nutrientinfo.Position & "ImposePercent", SqlDbType.Float).Value = CDblDB(nutrientinfo.Percent)
                Next

                For Each nutrientinfo As structNutrientInfo In arrNutrientsImposed
                    .Parameters.Add("@N" & nutrientinfo.Position & "Display", SqlDbType.Float).Value = CBoolDB(nutrientinfo.Visible)
                Next
                'For Each nutrientinfo As structNutrientInfo In arrNutrientsImposed
                '    .Parameters.Add("@N" & arrNutrientsImposed.IndexOf(nutrientinfo) + 1 & "Impose", SqlDbType.Float).Value = CDblDB(nutrientinfo.Value)
                'Next

                'For Each nutrientinfo As structNutrientInfo In arrNutrientsImposed
                '    .Parameters.Add("@N" & arrNutrientsImposed.IndexOf(nutrientinfo) + 1 & "ImposePercent", SqlDbType.Float).Value = CDblDB(nutrientinfo.Percent)
                'Next

                'For Each nutrientinfo As structNutrientInfo In arrNutrientsImposed
                '    .Parameters.Add("@N" & arrNutrientsImposed.IndexOf(nutrientinfo) + 1 & "Display", SqlDbType.Float).Value = CBoolDB(nutrientinfo.Visible)
                'Next

                .Parameters.Add("@ImposedType", SqlDbType.SmallInt).Value = intImposedType
                .Parameters.Add("@PortionSize", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(strPortionSize)
                .Parameters.Add("@DisplayNutrition", SqlDbType.Bit).Value = blnDisplayNutrition

                .Parameters.Add("@NutritionBasis", SqlDbType.NVarChar).Value = strNutritionBasis

                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateListeNutrientsImposed(ByVal intCodeListe As Integer, ByVal intCodeSet As Integer, ByVal arrNutrients As ArrayList, ByVal arrNutrientsImposed As ArrayList, ByVal intImposedType As Integer, ByVal strPortionSize As String, ByVal blnDisplayNutrition As Boolean, Optional ByVal strCodeLink As String = "00000", Optional ByVal strNutritionBasis As String = "") As enumEgswErrorCode
        Dim cmd As New SqlCommand

        Try
            ' there should be 14 elements in the array
            Dim intMaxNut As Integer = 14
            Dim intCurrentNut As Integer = arrNutrientsImposed.Count - 1
            Dim intFill As Integer = intMaxNut - intCurrentNut
            Dim arrParamList As New ArrayList
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[UPDATE_EgswNutrientValUpdateRecipeImposed]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@CodeLink", SqlDbType.VarChar, 20).Value = strCodeLink

                Dim intCtr As Integer = 1
                For Each nutrient In arrNutrients
                    If intCtr <= 42 Then
                        .Parameters.Add("@N" & intCtr, SqlDbType.Float).Value = CDblDB(nutrient)
                        intCtr += 1
                    End If
                Next

                For Each nutrientinfo As structNutrientInfo In arrNutrientsImposed
                    If arrParamList.Contains("@N" & nutrientinfo.Position & "Impose") = False Then
                        .Parameters.Add("@N" & nutrientinfo.Position & "Impose", SqlDbType.Float).Value = CDblDB(nutrientinfo.Value)
                        arrParamList.Add("@N" & nutrientinfo.Position & "Impose")
                    End If
                Next

                For Each nutrientinfo As structNutrientInfo In arrNutrientsImposed
                    If arrParamList.Contains("@N" & nutrientinfo.Position & "ImposePercent") = False Then
                        .Parameters.Add("@N" & nutrientinfo.Position & "ImposePercent", SqlDbType.Float).Value = CDblDB(nutrientinfo.Percent)
                        arrParamList.Add("@N" & nutrientinfo.Position & "ImposePercent")
                    End If
                Next

                For Each nutrientinfo As structNutrientInfo In arrNutrientsImposed
                    If arrParamList.Contains("@N" & nutrientinfo.Position & "Display") = False Then
                        .Parameters.Add("@N" & nutrientinfo.Position & "Display", SqlDbType.Float).Value = CBoolDB(nutrientinfo.Visible)
                        arrParamList.Add("@N" & nutrientinfo.Position & "Display")
                    End If
                Next
                ''Dim intCtr As Integer = 1
                ''For Each nutrient In arrNutrients
                ''    .Parameters.Add("@N" & intCtr, SqlDbType.Float).Value = CDblDB(nutrient)
                ''    intCtr += 1
                ''Next

                ''For Each nutrientinfo As structNutrientInfo In arrNutrientsImposed
                ''    .Parameters.Add("@N" & arrNutrientsImposed.IndexOf(nutrientinfo) + 1 & "Impose", SqlDbType.Float).Value = CDblDB(nutrientinfo.Value)
                ''Next

                ''For Each nutrientinfo As structNutrientInfo In arrNutrientsImposed
                ''    .Parameters.Add("@N" & arrNutrientsImposed.IndexOf(nutrientinfo) + 1 & "ImposePercent", SqlDbType.Float).Value = CDblDB(nutrientinfo.Percent)
                ''Next

                ''For Each nutrientinfo As structNutrientInfo In arrNutrientsImposed
                ''    .Parameters.Add("@N" & arrNutrientsImposed.IndexOf(nutrientinfo) + 1 & "Display", SqlDbType.Float).Value = CBoolDB(nutrientinfo.Visible)
                ''Next

                .Parameters.Add("@ImposedType", SqlDbType.SmallInt).Value = intImposedType
                .Parameters.Add("@PortionSize", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(strPortionSize)
                .Parameters.Add("@DisplayNutrition", SqlDbType.Bit).Value = blnDisplayNutrition

                .Parameters.Add("@NutritionBasis", SqlDbType.NVarChar).Value = strNutritionBasis
                .Parameters.Add("@CodeSet", SqlDbType.NVarChar).Value = intCodeSet

                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function InsertListeNutrientsImposed(ByVal intCodeListe As Integer, ByVal arrNutrients As ArrayList, Optional ByVal strNutritionBasis As String = "") As enumEgswErrorCode
        Dim cmd As New SqlCommand

        Try
            ' there should be 14 elements in the array
            Dim intMaxNut As Integer = 14

            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[INSERT_EgswNutrientValNewRecipeImposed]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe

                Dim intCtr As Integer = 1
                For Each nutrient In arrNutrients
                    If intCtr <= 42 Then
                        .Parameters.Add("@N" & intCtr, SqlDbType.Float).Value = CDblDB(nutrient)
                        intCtr += 1
                        If intCtr = 35 Then
                            Exit For
                        End If
                    End If
                Next
                .Parameters.Add("@NutritionBasis", SqlDbType.NVarChar).Value = strNutritionBasis
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                L_ErrCode = enumEgswErrorCode.OK
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function


    ''-- JBB 05.18.2012 Temp utrient Storage
    Public Function UpdateTempListeNutrientsImposed(ByVal intCodeListe As Integer, ByVal arrNutrientsImposed As ArrayList, ByVal intImposedType As Integer, ByVal strPortionSize As String, ByVal blnDisplayNutrition As Boolean, ByVal intCodeSet As Integer, Optional ByVal strCodeLink As String = "00000", Optional ByVal strNutritionBasis As String = "") As enumEgswErrorCode
        Dim cmd As New SqlCommand

        Try
            ' there should be 14 elements in the array
            Dim intMaxNut As Integer = 14
            Dim intCurrentNut As Integer = arrNutrientsImposed.Count - 1
            Dim intFill As Integer = intMaxNut - intCurrentNut
            Dim arrParamList As New ArrayList()
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[UPDATE_TempNutrietValRecipeImposed]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@CodeLink", SqlDbType.VarChar, 20).Value = strCodeLink

                Dim intCtr As Integer = 1

                For Each nutrientinfo As structNutrientInfo In arrNutrientsImposed
                    If Not arrParamList.Contains("@N" & nutrientinfo.Position & "Impose") Then
                        .Parameters.Add("@N" & nutrientinfo.Position & "Impose", SqlDbType.Float).Value = CDblDB(nutrientinfo.Value)
                        arrParamList.Add("@N" & nutrientinfo.Position & "Impose")
                    End If


                Next

                For Each nutrientinfo As structNutrientInfo In arrNutrientsImposed
                    If Not arrParamList.Contains("@N" & nutrientinfo.Position & "ImposePercent") Then
                        .Parameters.Add("@N" & nutrientinfo.Position & "ImposePercent", SqlDbType.Float).Value = CDblDB(nutrientinfo.Percent)
                        arrParamList.Add("@N" & nutrientinfo.Position & "ImposePercent")
                    End If

                Next

                For Each nutrientinfo As structNutrientInfo In arrNutrientsImposed
                    If Not arrParamList.Contains("@N" & nutrientinfo.Position & "Display") Then
                        .Parameters.Add("@N" & nutrientinfo.Position & "Display", SqlDbType.Float).Value = CBoolDB(nutrientinfo.Visible)
                        arrParamList.Add("@N" & nutrientinfo.Position & "Display")
                    End If

                Next

                'For Each nutrientinfo As structNutrientInfo In arrNutrientsImposed
                '    .Parameters.Add("@N" & arrNutrientsImposed.IndexOf(nutrientinfo) + 1 & "Impose", SqlDbType.Float).Value = CDblDB(nutrientinfo.Value)
                'Next

                'For Each nutrientinfo As structNutrientInfo In arrNutrientsImposed
                '    .Parameters.Add("@N" & arrNutrientsImposed.IndexOf(nutrientinfo) + 1 & "ImposePercent", SqlDbType.Float).Value = CDblDB(nutrientinfo.Percent)
                'Next

                'For Each nutrientinfo As structNutrientInfo In arrNutrientsImposed
                '    .Parameters.Add("@N" & arrNutrientsImposed.IndexOf(nutrientinfo) + 1 & "Display", SqlDbType.Float).Value = CBoolDB(nutrientinfo.Visible)
                'Next

                .Parameters.Add("@ImposedType", SqlDbType.SmallInt).Value = intImposedType
                .Parameters.Add("@PortionSize", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(strPortionSize)
                .Parameters.Add("@DisplayNutrition", SqlDbType.Bit).Value = blnDisplayNutrition

                .Parameters.Add("@NutritionBasis", SqlDbType.NVarChar).Value = strNutritionBasis
                .Parameters.Add("@CodeSet", SqlDbType.Int).Value = intCodeSet

                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateIngredientTranslation(ByVal intItemID As Integer, ByVal intCodeTrans As Integer, ByVal strComplement As String, ByVal strPreparation As String, ByVal strName As String, Optional ByRef stCaller As SqlTransaction = Nothing) As enumEgswErrorCode
        Dim st As SqlTransaction
        Dim cmd As New SqlCommand
        Try
            With cmd

                .CommandText = "sp_egswDetailsTranslationUpdate"
                .CommandType = CommandType.StoredProcedure

                If stCaller Is Nothing Then
                    .Connection = New SqlConnection(L_strCnn)
                    .Connection.Open()
                    st = .Connection.BeginTransaction
                Else
                    st = stCaller
                    .Connection = st.Connection
                End If

                .Transaction = st
                .Parameters.Add("@intIDDetails", SqlDbType.Int).Value = intItemID
                .Parameters.Add("@nvcComplement", SqlDbType.NVarChar, 2000).Value = ReplaceSpecialCharacters(strComplement)
                .Parameters.Add("@nvcPreparation", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(strPreparation)
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(strName)
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .ExecuteNonQuery()

                If stCaller Is Nothing Then
                    st.Commit()
                    .Connection.Close()
                Else
                    stCaller = st
                End If

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            If stCaller Is Nothing Then st.Rollback()

            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function FixRecipeSubRecipesLevel(ByVal intCode As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_egswListeRecipeFixSRLevel"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 50000 'AGL 2012.09.04 - added timeout 'AGL Merging 2012.09.17
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function GetRecipeYield(ByVal intCode As Integer) As Double

        Dim cmd As New SqlCommand
        Dim dblYield As Double = 1

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_GetRecipeYield"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode


                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .CommandTimeout = 50000
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                dblYield = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return dblYield

    End Function

    'JTOC 12.03.2013
    Public Function GetCategoryByListeCode(ByVal intCode As Integer) As Integer

        Dim cmd As New SqlCommand
        Dim intCategory As Integer = 0

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "GetCategoryByListeCode"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCode


                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .CommandTimeout = 50000
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                intCategory = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return intCategory

    End Function

    Public Function RecomputeRecipeSubRecipes(ByVal intCode As Integer, ByVal intCodeSetPrice As Integer, Optional ByVal dblOldYield As Double = -1, Optional ByVal dblNewYield As Double = -1, Optional ByVal intMetImp As Integer = 2, Optional ByVal blnUpdateNutrients As Boolean = True) As enumEgswErrorCode  'JTOC 28.02.2013 intMetric 0 = Imperial, 1 = Metric, 2 = Net/Gross Quantity
        Dim cmd As New SqlCommand

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_egswListeRecipeSubRecipesRecomputePricePerSetPrice"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@intCodeSetprice", SqlDbType.Int).Value = intCodeSetPrice

                'JTOC 28.02.2013 Added New Parameters
                If dblOldYield <> -1 Then
                    .Parameters.Add("@fltOldYield", SqlDbType.Float).Value = dblOldYield
                    .Parameters.Add("@fltNewYield", SqlDbType.Float).Value = dblNewYield
                End If
                .Parameters.Add("@intMetImp", SqlDbType.Int).Value = intMetImp
                .Parameters.Add("@bitUpdateNutrient", SqlDbType.Bit).Value = blnUpdateNutrients


                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .CommandTimeout = 50000
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function Recompute(ByVal intCode As Integer, ByVal arr As ArrayList, ByVal listeType As enumDataListItemType, Optional ByVal dblOldYield As Double = -1, Optional intMetricImp As Integer = 2) As enumEgswErrorCode
        Try
            Dim nLastIndex As Integer = arr.Count - 1
            Dim counter As Integer
            For counter = 0 To nLastIndex
                Select Case listeType
                    Case enumDataListItemType.Merchandise
                        L_ErrCode = RecomputeRecipesAndMenusOfItem(intCode, CInt(arr(counter)), intMetricImp) 'AGL 2013.08.30 - added intMetricImp
                    Case enumDataListItemType.Recipe
                        L_ErrCode = RecomputeRecipeSubRecipes(intCode, CInt(arr(counter)), dblOldYield, , intMetricImp)
                End Select
            Next

            Return L_ErrCode
        Catch ex As Exception
            Return L_ErrCode
        End Try
    End Function

    Public Function RecomputeAllSetPrice(CodeListe As Integer, CodeUser As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_RecomputeAllSetPrice"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = CodeListe
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = CodeUser

                .CommandTimeout = 50000
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                L_ErrCode = enumEgswErrorCode.OK
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            Throw New Exception(ex.Message, ex)
        Finally
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
        End Try

        Return L_ErrCode
    End Function

    Public Function UpdateListeIngredients(ByVal intCodeListe As Integer, ByVal dtIng As DataTable, ByVal dtIngSetPrice As DataTable, ByVal dtTrans As DataTable, Optional ByVal boolQuickEncode As Boolean = False, Optional ByVal blnWithStep As Boolean = False, Optional ByVal blnAllowMetricImperial As Boolean = False, Optional ByVal blnSendRequestApproval As Boolean = False, Optional ByVal intUserCode As Integer = -1, Optional intListeType As Integer = -1) As enumEgswErrorCode  'JTOC 07.12.2012 added blnAllowMetricImperial 'AGL
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim cmdSetPrice As New SqlCommand
        Dim cmdTranslate As New SqlCommand

        Dim egswErrorType As enumEgswErrorCode = enumEgswErrorCode.GeneralError
        Dim st As SqlTransaction = Nothing
        Dim da As New SqlDataAdapter
        Dim row As DataRow
        Dim rowChild As DataRow
        Dim dtOld As DataTable = CType(GetIngredients(intCodeListe, -1, enumEgswFetchType.DataTable), DataTable)
        Dim arrRemovedItems As ArrayList = GetItemsRemoved("itemID", dtOld, dtIng)
        Dim arrRemovedItems2 As List(Of List(Of String))
        If blnWithStep Then arrRemovedItems2 = GetItemsRemovedNameAndStep("itemID", dtOld, dtIng)

        Dim bHasMenuCardFieldsExists As Boolean = dtIng.Columns.Contains("MenuCardApprovedPriceNew") And dtIng.Columns.Contains("MenuCardCostPrice")

        Try

            If Not blnSendRequestApproval Then
                If blnWithStep Then
                    RemoveListeDetailsItem2(arrRemovedItems2, intCodeListe)
                Else
                    RemoveListeDetailsItem(arrRemovedItems)
                End If
            End If

            With cmd
                .Connection = cn
                .CommandText = "sp_EgswDetailsUpdate"
                .CommandType = CommandType.StoredProcedure

                cmdSetPrice.Connection = cn
                cmdSetPrice.CommandText = "sp_egswDetailSetPriceUpdate"
                cmdSetPrice.CommandType = CommandType.StoredProcedure

                cmdTranslate.Connection = cn
                cmdTranslate.CommandText = "sp_egswDetailsTranslationUpdate"
                cmdTranslate.CommandType = CommandType.StoredProcedure

                cn.Open()
                st = cn.BeginTransaction
                .Transaction = st

                cmdSetPrice.Transaction = st
                cmdTranslate.Transaction = st
                .Parameters.Add("@intFirstCode", SqlDbType.Int)
                .Parameters.Add("@intSecondCode", SqlDbType.Int)
                .Parameters.Add("@intPosition", SqlDbType.Int)
                .Parameters.Add("@fltQuantity", SqlDbType.Float)
                .Parameters.Add("@intCodeUnit", SqlDbType.Int)
                .Parameters.Add("@nvcComplement", SqlDbType.NVarChar, 2000)
                .Parameters.Add("@nvcPreparation", SqlDbType.NVarChar, 700)
                .Parameters.Add("@intWastage1", SqlDbType.Int)
                .Parameters.Add("@intWastage2", SqlDbType.Int)
                .Parameters.Add("@intWastage3", SqlDbType.Int)
                .Parameters.Add("@intWastage4", SqlDbType.Int)
                .Parameters.Add("@fltCoeff", SqlDbType.Float)
                .Parameters.Add("@fltMenuCardApprovedPriceNew", SqlDbType.Float)
                .Parameters.Add("@fltMenuCardCostPrice", SqlDbType.Float)
                .Parameters.Add("@intID", SqlDbType.Int).Direction = ParameterDirection.InputOutput
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                .Parameters.Add("@vchKeyField", SqlDbType.VarChar, 50)  'RDTC 09.08.2007

                .Parameters.Add("@blnIsQuickEncode", SqlDbType.Bit) ' MRC 020508 - Added this flag to determine if the caller is Quick Encoding so that complement and preparation fields wont be overwritten.
                .Parameters.Add("@intStep", SqlDbType.Int) ' MRC 052108 - Added this param for the step.
                If boolQuickEncode Then
                    .Parameters.Add("@nvcTmpQty", SqlDbType.NVarChar, 25)
                    .Parameters.Add("@nvcTmpUnit", SqlDbType.NVarChar, 25)
                    .Parameters.Add("@nvcTmpName", SqlDbType.NVarChar, 260)
                    .Parameters.Add("@nvcTmpComplement", SqlDbType.NVarChar, 2000)
                    .Parameters.Add("@nvcTmpPreparation", SqlDbType.NVarChar, 2000)
                End If

                .Parameters.Add("@fltQuantityMetric", SqlDbType.Float)
                .Parameters.Add("@intCodeUnitMetric", SqlDbType.Int)
                .Parameters.Add("@fltQuantityImperial", SqlDbType.Float)
                .Parameters.Add("@intCodeUnitImperial", SqlDbType.Int)
                .Parameters.Add("@AlternativeIngredient", SqlDbType.NVarChar, 300)
                .Parameters.Add("@Description", SqlDbType.NVarChar, 2000)
                .Parameters.Add("@Tip", SqlDbType.NVarChar, 4000)
                .Parameters.Add("@DigitalAsset", SqlDbType.NVarChar, 4000)
                .Parameters.Add("@FreakOutMoment", SqlDbType.Bit)
                .Parameters.Add("@IsAllowMetricImperial", SqlDbType.Bit)
                .Parameters.Add("@Remark", SqlDbType.NVarChar, 2000)
                .Parameters.Add("@intConvertDirection", SqlDbType.NVarChar, 2000)

                'If blnSendRequestApproval Then
                If intListeType = 8 Then
                    .Parameters.Add("@intApprovalStatusCode", SqlDbType.Int)
                    .Parameters.Add("@intApprovalRequestedBy", SqlDbType.Int)
                    .Parameters.Add("@dtApprovalRequestedDate", SqlDbType.DateTime)
                End If
                'EgswDetails.ApprovalStatusCode()
                'End If

                cmdSetPrice.Parameters.Add("@intIDDetails", SqlDbType.Int)
                cmdSetPrice.Parameters.Add("@intFirstCodeSetPrice", SqlDbType.Int)
                cmdSetPrice.Parameters.Add("@intSecondCodeSetPrice", SqlDbType.Int)
                cmdSetPrice.Parameters.Add("@fltConst", SqlDbType.Float)
                cmdSetPrice.Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                cmdTranslate.Parameters.Add("@intIDDetails", SqlDbType.Int)
                cmdTranslate.Parameters.Add("@nvcComplement", SqlDbType.NVarChar, 2000)
                cmdTranslate.Parameters.Add("@nvcPreparation", SqlDbType.NVarChar, 700)
                cmdTranslate.Parameters.Add("@nvcTip", SqlDbType.NVarChar, 4000) '// DRR 06.29.2011
                cmdTranslate.Parameters.Add("@intCodeTrans", SqlDbType.Int)
                cmdTranslate.Parameters.Add("@nvcName", SqlDbType.NVarChar, 260)
                cmdTranslate.Parameters.Add("@nvcAlternativeIngredient", SqlDbType.NVarChar, 2000) '// DRR 04.23.2012
                cmdTranslate.Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                Dim intRowFirstcode As Integer = intCodeListe
                Dim intRowSecondCode As Integer
                Dim intRowType As Integer
                Dim intRowPosition As Integer = 1
                Dim dblRowQuantity As Double
                Dim intRowCodeUnit As Integer

                Dim strRowComplement As String
                Dim strRowPreparation As String
                Dim strRowTip As String
                Dim strRowName As String
                Dim strAlternativeIngredientTrans As String

                Dim intRowWastage1 As Integer
                Dim intRowWastage2 As Integer
                Dim intRowWastage3 As Integer
                Dim intRowWastage4 As Integer
                Dim dblCoeff As Double

                '// DRR 12.29.2010 added for metric & imperial
                Dim dblQuantityMetric As Double
                Dim intCodeUnitMetric As Integer
                Dim dblQuantityImperial As Double
                Dim intCodeUnitImperial As Integer
                Dim strAlternativeIngredient As String
                '//
                Dim strDescription As String '// DRR 03.04.2011
                Dim strTip As String '// DRR 06.29.2011
                Dim strDigitalAsset As String '// DRR 06.29.2011
                Dim blnFreakOut As Boolean '// DRR 07.20.2011

                Dim strRemark As String '// JTOC 20.02.2013
                Dim intConvertDirection As String   '// JTOC 20.03.2013

                Dim intRowID As Integer
                Dim intRowFirstcodesetprice As Integer
                Dim intRowSecondCodesetprice As Integer
                Dim dblRowConst As Double

                Dim dblRowMenuCardApprovedPriceNew As Double
                Dim dblRowMenuCardCostPrice As Double

                Dim intCodeTrans As Integer
                Dim strKeyField As String

                Dim intStep As Integer  'MRC 052108
                Dim tmpqty As String = ""
                Dim tmpunit As String = ""
                Dim tmpname As String = ""
                Dim tmpcomplement As String = ""
                Dim tmppreparation As String = ""

                Dim intApprovalStatusCode As Integer = 1
                Dim intRequestBy As Integer = 0
                Dim dtRequestDate As Date


                Dim dr As DataRelation
                Dim ds As New DataSet

                ' MRC - To avoid global declaration errors that causes the xml exception,
                ' I added a counter that is parsed on the table name.
                Dim x As Integer = 0
                If dtIng.TableName <> "" And Not dtIng.TableName = Nothing Then
                    If dtIng.TableName.Length > 3 And dtIng.TableName.StartsWith("Ing") Then
                        x = CInt(dtIng.TableName.Substring(3, 1))
                        x += 1
                    ElseIf dtIng.TableName.Length > 5 And dtIng.TableName.StartsWith("Table") Then
                        x = CInt(dtIng.TableName.Substring(5, 1))
                        x += 1
                    Else
                        x = 0
                    End If
                    If dtIng.TableName.StartsWith("Ing") Then
                        dtIng.TableName = "Ing" & x
                    ElseIf dtIng.TableName.StartsWith("Table") Then
                        dtIng.TableName = "Table" & x
                    End If

                Else
                    dtIng.TableName = "Ing" & x
                End If


                If Not dtIngSetPrice Is Nothing Then
                    dtIngSetPrice.TableName = "IngSetPrice"
                    ds.Tables.Add(dtIng.Copy)
                    ds.Tables.Add(dtIngSetPrice.Copy)
                    dr = New DataRelation("IngSetPrice", ds.Tables(0).Columns("itemID"), ds.Tables(1).Columns("IDDetails"))
                    ds.Relations.Add(dr)
                Else
                    ds.Tables.Add(dtIng.Copy)
                End If

                If Not dtTrans Is Nothing Then
                    dtTrans.TableName = "translation"
                    ds.Tables.Add(dtTrans.Copy)
                    dr = New DataRelation("IngTranslation", ds.Tables(0).Columns("itemID"), ds.Tables("translation").Columns("IDDetails"))
                    ds.Relations.Add(dr)
                End If

                ds.Tables(0).DefaultView.Sort = "position"
                Dim drv As DataRowView

                For Each drv In ds.Tables(0).DefaultView
                    ' Get Values
                    intRowPosition = CInt(drv("position")) 'JTOC 17.06.2013
                    intRowSecondCode = CInt(drv.Item("itemcode"))
                    intRowType = CInt(drv.Item("itemType"))
                    dblRowQuantity = CDbl(drv.Item("itemqty"))
                    intRowCodeUnit = CInt(drv.Item("itemunitcode"))
                    strRowComplement = CStrDB(drv.Item("complement"))
                    strRowPreparation = CStrDB(drv.Item("preparation"))
                    strRowTip = CStrDB(drv.Item("tip")) '// DRR 06.29.2011
                    intRowWastage1 = CInt(drv.Item("wastage1"))
                    intRowWastage2 = CInt(drv.Item("wastage2"))
                    intRowWastage3 = CInt(drv.Item("wastage3"))
                    intRowWastage4 = CInt(drv.Item("wastage4"))
                    intRowID = CInt(drv.Item("itemID"))
                    dblCoeff = CDbl(drv.Item("coefficient"))
                    '// DRR 12.29.2010 added
                    dblQuantityMetric = 0
                    dblQuantityImperial = 0
                    intCodeUnitMetric = 0
                    intCodeUnitImperial = 0
                    intConvertDirection = 0
                    strAlternativeIngredient = ""
                    strDescription = ""
                    strRemark = ""
                    strTip = ""
                    strDigitalAsset = ""

                    If intListeType = 8 Then
                        intApprovalStatusCode = GetInt(drv.Item("ApprovalStatusCode"), 1)
                        intRequestBy = GetInt(drv.Item("ApprovalRequestedBy"), intUserCode)
                        dtRequestDate = GetDate(drv.Item("ApprovalRequestedDate"), Date.Now)
                    End If

                    If Not IsDBNull(drv.Item("QuantityMetric")) Then
                        dblQuantityMetric = CDbl(drv.Item("QuantityMetric"))
                    End If
                    If Not IsDBNull(drv.Item("QuantityImperial")) Then
                        dblQuantityImperial = CDbl(drv.Item("QuantityImperial"))
                    End If
                    If Not IsDBNull(drv.Item("CodeUnitMetric")) Then
                        intCodeUnitMetric = CInt(drv.Item("CodeUnitMetric"))
                    End If
                    If Not IsDBNull(drv.Item("CodeUnitImperial")) Then
                        intCodeUnitImperial = CInt(drv.Item("CodeUnitImperial"))
                    End If

                    'JTOC 20.03.2013
                    If Not IsDBNull(drv.Item("ConvertDirection")) Then
                        intConvertDirection = drv.Item("ConvertDirection")
                    End If
                    strAlternativeIngredient = CStrDB(drv.Item("AlternativeIngredient"))
                    strDescription = CStrDB(drv.Item("Description"))
                    strRemark = CStrDB(drv.Item("Remark"))
                    strTip = CStrDB(drv.Item("Tip"))
                    strDigitalAsset = CStrDB(drv.Item("DigitalAsset"))
                    '//
                    If Not IsDBNull(drv("FreakOutMoment")) Then blnFreakOut = drv("FreakOutMoment")

                    Try
                        strKeyField = CStrDB(drv.Item("KeyField"))
                    Catch ex As Exception
                        strKeyField = ""
                    End Try



                    row = ds.Tables(0).Select("itemID=" & intRowID)(0)

                    If bHasMenuCardFieldsExists Then
                        If IsDBNull(drv.Item("MenuCardCostPrice")) Then
                            dblRowMenuCardCostPrice = 0
                            dblRowMenuCardApprovedPriceNew = 0
                        Else
                            dblRowMenuCardCostPrice = CDbl(drv.Item("MenuCardCostPrice"))
                            dblRowMenuCardApprovedPriceNew = CDbl(drv.Item("MenuCardApprovedPriceNew"))
                        End If
                    Else
                        dblRowMenuCardCostPrice = 0
                        dblRowMenuCardApprovedPriceNew = 0
                    End If

                    intStep = CIntDB(drv.Item("step"))
                    If boolQuickEncode Then
                        'tmpqty = CStrDB(drv.Item("qty"))
                        'tmpunit = CStrDB(drv.Item("unit"))
                        'tmpname = CStrDB(drv.Item("name"))
                        'tmpcomplement = CStrDB(drv.Item("tmpcomplement"))
                        'tmppreparation = CStrDB(drv.Item("tmppreparation"))

                        If CStrDB(drv.Item("qty")).Trim.Length > 0 Then
                            tmpqty = CStrDB(drv.Item("qty"))
                        Else
                            tmpqty = Nothing
                        End If

                        If CStrDB(drv.Item("unit")).Trim.Length > 0 Then
                            tmpunit = CStrDB(drv.Item("unit"))
                        Else
                            tmpunit = Nothing
                        End If

                        If CStrDB(drv.Item("name")).Trim.Length > 0 Then
                            tmpname = CStrDB(drv.Item("name"))
                        Else
                            tmpname = Nothing
                        End If

                        If CStrDB(drv.Item("tmpcomplement")).Trim.Length > 0 Then
                            tmpcomplement = CStrDB(drv.Item("tmpcomplement"))
                        Else
                            tmpcomplement = Nothing
                        End If

                        If CStrDB(drv.Item("tmppreparation")).Trim.Length > 0 Then
                            tmppreparation = CStrDB(drv.Item("tmppreparation"))
                        Else
                            tmppreparation = Nothing
                        End If
                    End If

                    ' Save only the ingredients, not the steps.
                    If Not intRowType = 75 Then
                        ' Save
                        .Parameters("@intFirstCode").Value = intRowFirstcode
                        .Parameters("@intSecondCode").Value = intRowSecondCode
                        .Parameters("@intPosition").Value = intRowPosition
                        .Parameters("@fltQuantity").Value = dblRowQuantity
                        .Parameters("@intCodeUnit").Value = intRowCodeUnit
                        .Parameters("@nvcComplement").Value = ReplaceSpecialCharacters(strRowComplement)
                        .Parameters("@nvcPreparation").Value = ReplaceSpecialCharacters(strRowPreparation)
                        .Parameters("@intWastage1").Value = intRowWastage1
                        .Parameters("@intWastage2").Value = intRowWastage2
                        .Parameters("@intWastage3").Value = intRowWastage3
                        .Parameters("@intWastage4").Value = intRowWastage4
                        .Parameters("@fltCoeff").Value = dblCoeff
                        .Parameters("@fltMenuCardApprovedPriceNew").Value = dblRowMenuCardApprovedPriceNew
                        .Parameters("@fltMenuCardCostPrice").Value = dblRowMenuCardCostPrice
                        .Parameters("@intID").Value = intRowID
                        .Parameters("@vchKeyField").Value = strKeyField
                        .Parameters("@blnIsQuickEncode").Value = boolQuickEncode ' MRC 02/05/08
                        .Parameters("@intStep").Value = intStep ' MRC 02/05/08

                        If boolQuickEncode Then
                            .Parameters("@nvcTmpQty").Value = ReplaceSpecialCharacters(tmpqty)
                            .Parameters("@nvcTmpUnit").Value = ReplaceSpecialCharacters(tmpunit)
                            .Parameters("@nvcTmpName").Value = ReplaceSpecialCharacters(tmpname)
                            .Parameters("@nvcTmpComplement").Value = ReplaceSpecialCharacters(tmpcomplement)
                            .Parameters("@nvcTmpPreparation").Value = ReplaceSpecialCharacters(tmppreparation)
                        End If

                        '// DRR 12.29.2010
                        .Parameters("@fltQuantityMetric").Value = dblQuantityMetric
                        .Parameters("@intCodeUnitMetric").Value = intCodeUnitMetric
                        .Parameters("@fltQuantityImperial").Value = dblQuantityImperial
                        .Parameters("@intCodeUnitImperial").Value = intCodeUnitImperial
                        .Parameters("@intConvertDirection").Value = intConvertDirection 'JTOC 20.03.2013
                        .Parameters("@AlternativeIngredient").Value = ReplaceSpecialCharacters(strAlternativeIngredient)
                        .Parameters("@Description").Value = ReplaceSpecialCharacters(strDescription)
                        .Parameters("@Tip").Value = ReplaceSpecialCharacters(strTip)
                        .Parameters("@DigitalAsset").Value = ReplaceSpecialCharacters(strDigitalAsset)
                        '//
                        .Parameters("@FreakOutMoment").Value = blnFreakOut
                        .Parameters("@IsAllowMetricImperial").Value = blnAllowMetricImperial
                        .Parameters("@Remark").Value = ReplaceSpecialCharacters(strRemark)

                        If intListeType = 8 Then
                            .Parameters("@intApprovalStatusCode").Value = intApprovalStatusCode
                            .Parameters("@intApprovalRequestedBy").Value = intRequestBy
                            .Parameters("@dtApprovalRequestedDate").Value = IIf(dtRequestDate >= CDate(" 1/1/1753"), dtRequestDate, Date.Now)
                        End If
                        'If blnSendRequestApproval Then
                        '	.Parameters("@intApprovalRequestedBy").Value = intUserCode
                        'End If

                        .ExecuteNonQuery()

                        egswErrorType = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                        If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception

                        intRowID = CIntDB(.Parameters("@intID").Value)

                        '// save set price
                        For Each rowChild In row.GetChildRows("IngSetPrice")
                            intRowFirstcodesetprice = CInt(rowChild("firstcodesetprice"))
                            intRowSecondCodesetprice = CInt(rowChild("secondcodesetprice"))
                            dblRowConst = CDbl(rowChild("const"))

                            cmdSetPrice.Parameters("@intIDDetails").Value = intRowID
                            cmdSetPrice.Parameters("@intfirstCodeSetPrice").Value = intRowFirstcodesetprice
                            cmdSetPrice.Parameters("@intSecondCodeSetPrice").Value = intRowSecondCodesetprice
                            cmdSetPrice.Parameters("@fltConst").Value = dblRowConst
                            cmdSetPrice.ExecuteNonQuery()

                            egswErrorType = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                            If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception
                        Next

                        '// save Text Translation
                        For Each rowChild In row.GetChildRows("IngTranslation")
                            strRowComplement = CStrDB(rowChild("complement"))
                            strRowPreparation = CStrDB(rowChild("preparation"))
                            strRowTip = CStrDB(rowChild("tip"))
                            intCodeTrans = CInt(rowChild("codetrans"))
                            strRowName = CStrDB(rowChild("name"))
                            strAlternativeIngredientTrans = CStrDB(rowChild("alternativeIngredient"))

                            cmdTranslate.Parameters("@intIDDetails").Value = intRowID
                            cmdTranslate.Parameters("@nvcComplement").Value = ReplaceSpecialCharacters(strRowComplement)
                            cmdTranslate.Parameters("@nvcPreparation").Value = ReplaceSpecialCharacters(strRowPreparation)
                            cmdTranslate.Parameters("@nvcTip").Value = ReplaceSpecialCharacters(strRowTip)
                            cmdTranslate.Parameters("@intCodeTrans").Value = intCodeTrans
                            cmdTranslate.Parameters("@nvcName").Value = ReplaceSpecialCharacters(strRowName)
                            cmdTranslate.Parameters("@nvcAlternativeIngredient").Value = ReplaceSpecialCharacters(strAlternativeIngredientTrans)
                            cmdTranslate.ExecuteNonQuery()

                            egswErrorType = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                            If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception
                        Next
                    End If

                    intRowPosition = intRowPosition + 1
                Next

                ''DRR 02.11.2013 uncomment
                'For Each row In ds.Tables(0).Rows
                '    ' Get Values
                '    intRowSecondCode = CInt(row.Item("itemcode"))
                '    intRowType = CInt(row.Item("itemType"))
                '    dblRowQuantity = CDbl(row.Item("itemqty"))
                '    intRowCodeUnit = CInt(row.Item("itemunitcode"))
                '    strRowComplement = CStrDB(row.Item("complement"))
                '    strRowPreparation = CStrDB(row.Item("preparation"))
                '    intRowWastage1 = CInt(row.Item("wastage1"))
                '    intRowWastage2 = CInt(row.Item("wastage2"))
                '    intRowWastage3 = CInt(row.Item("wastage3"))
                '    intRowWastage4 = CInt(row.Item("wastage4"))
                '    intRowID = CInt(row.Item("itemID"))
                '    dblCoeff = CDbl(row.Item("coefficient"))

                '    If bHasMenuCardFieldsExists Then
                '        If IsDBNull(row.Item("MenuCardCostPrice")) Then
                '            dblRowMenuCardCostPrice = 0
                '            dblRowMenuCardApprovedPriceNew = 0
                '        Else
                '            dblRowMenuCardCostPrice = CDbl(row.Item("MenuCardCostPrice"))
                '            dblRowMenuCardApprovedPriceNew = CDbl(row.Item("MenuCardApprovedPriceNew"))
                '        End If
                '    Else
                '        dblRowMenuCardCostPrice = 0
                '        dblRowMenuCardApprovedPriceNew = 0
                '    End If

                '    ' Save
                '    .Parameters("@intFirstCode").Value = intRowFirstcode
                '    .Parameters("@intSecondCode").Value = intRowSecondCode
                '    .Parameters("@intPosition").Value = intRowPosition
                '    .Parameters("@fltQuantity").Value = dblRowQuantity
                '    .Parameters("@intCodeUnit").Value = intRowCodeUnit
                '    .Parameters("@nvcComplement").Value = strRowComplement
                '    .Parameters("@nvcPreparation").Value = strRowPreparation
                '    .Parameters("@intWastage1").Value = intRowWastage1
                '    .Parameters("@intWastage2").Value = intRowWastage2
                '    .Parameters("@intWastage3").Value = intRowWastage3
                '    .Parameters("@intWastage4").Value = intRowWastage4
                '    .Parameters("@fltCoeff").Value = dblCoeff
                '    .Parameters("@fltMenuCardApprovedPriceNew").Value = dblRowMenuCardApprovedPriceNew
                '    .Parameters("@fltMenuCardCostPrice").Value = dblRowMenuCardCostPrice
                '    .Parameters("@intID").Value = intRowID
                '    .ExecuteNonQuery()

                '    egswErrorType = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                '    If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception

                '    intRowID = CInt(.Parameters("@intID").Value)

                '    '// save set price
                '    For Each rowChild In row.GetChildRows("IngSetPrice")
                '        intRowFirstcodesetprice = CInt(rowChild("firstcodesetprice"))
                '        intRowSecondCodesetprice = CInt(rowChild("secondcodesetprice"))
                '        dblRowConst = CDbl(rowChild("const"))

                '        cmdSetPrice.Parameters("@intIDDetails").Value = intRowID
                '        cmdSetPrice.Parameters("@intfirstCodeSetPrice").Value = intRowFirstcodesetprice
                '        cmdSetPrice.Parameters("@intSecondCodeSetPrice").Value = intRowSecondCodesetprice
                '        cmdSetPrice.Parameters("@fltConst").Value = dblRowConst
                '        cmdSetPrice.ExecuteNonQuery()

                '        egswErrorType = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                '        If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception
                '    Next

                '    '// save Text Translation
                '    For Each rowChild In row.GetChildRows("IngTranslation")
                '        strRowComplement = CStr(rowChild("complement"))
                '        strRowPreparation = CStr(rowChild("preparation"))
                '        intCodeTrans = CInt(rowChild("codetrans"))
                '        strRowName = CStr(rowChild("name"))

                '        cmdTranslate.Parameters("@intIDDetails").Value = intRowID
                '        cmdTranslate.Parameters("@nvcComplement").Value = strRowComplement
                '        cmdTranslate.Parameters("@nvcPreparation").Value = strRowPreparation
                '        cmdTranslate.Parameters("@intCodeTrans").Value = intCodeTrans
                '        cmdTranslate.Parameters("@nvcName").Value = strRowName
                '        cmdTranslate.ExecuteNonQuery()

                '        egswErrorType = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                '        If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception
                '    Next

                '    intRowPosition = intRowPosition + 1
                'Next

                st.Commit()
                cn.Close()
                cn.Dispose()
            End With
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            st.Rollback()
            cn.Close()
            cn.Dispose()
            L_ErrCode = egswErrorType
        End Try
    End Function

    '// DRR 07.04.2011
    Public Function fctListeNoteUpdate(ByVal Codeliste As Integer, ByVal dtNote As DataTable, ByVal dtNoteTrans As DataTable) As enumEgswErrorCode

        Dim cn As SqlConnection
        Dim cmd As New SqlCommand
        Dim cmdTrans As New SqlCommand
        Dim egswErrorType As enumEgswErrorCode = enumEgswErrorCode.GeneralError
        Dim intID As Integer
        Dim intRowID As Integer
        Dim ds As New DataSet
        Dim dtr As DataRelation
        Dim row As DataRow

        Try

            With cmd
                cn = New SqlConnection(L_strCnn)

                .Connection = cn
                .CommandText = "sp_EgswListeNoteUpdate"
                .CommandType = CommandType.StoredProcedure

                cmdTrans.Connection = cn
                cmdTrans.CommandText = "sp_EgswListeNoteTranslationUpdate"
                cmdTrans.CommandType = CommandType.StoredProcedure

                .Parameters.Clear()
                .Parameters.Add("@NoteId", SqlDbType.Int) '// DRR 11.18.2011
                .Parameters.Add("@Codeliste", SqlDbType.Int)
                .Parameters.Add("@Position", SqlDbType.Int)
                .Parameters.Add("@DigitalAsset", SqlDbType.NVarChar, 2000)
                .Parameters.Add("@Picture", SqlDbType.NVarChar, 2000)
                .Parameters.Add("@FreakOutMoment", SqlDbType.Bit)
                .Parameters.Add("@ID", SqlDbType.Int)
                .Parameters("@ID").Direction = ParameterDirection.InputOutput
                .Parameters.Add("@retVal", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                cmdTrans.Parameters.Clear()
                cmdTrans.Parameters.Add("@Id", SqlDbType.Int) '// DRR 11.18.2011
                cmdTrans.Parameters.Add("@NoteID", SqlDbType.Int)
                cmdTrans.Parameters.Add("@Codetrans", SqlDbType.Int)
                cmdTrans.Parameters.Add("@Note", SqlDbType.NVarChar, 2000)
                cmdTrans.Parameters.Add("@Comment", SqlDbType.NVarChar, 2000)
                cmdTrans.Parameters.Add("@CookMode", SqlDbType.NVarChar, 4000)
                cmdTrans.Parameters.Add("@retVal", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                ds.Tables.Add(dtNote.Copy)
                If Not dtNoteTrans Is Nothing Then
                    dtNoteTrans.TableName = "translation"
                    ds.Tables.Add(dtNoteTrans.Copy)
                    dtr = New DataRelation("notetrans", ds.Tables(0).Columns("ID"), ds.Tables("translation").Columns("NoteID"))
                    ds.Relations.Add(dtr)
                End If

                ds.Tables(0).DefaultView.Sort = "ID, Pos"

                cn.Open()

                For Each drv As DataRowView In ds.Tables(0).DefaultView
                    intRowID = drv("ID")
                    row = ds.Tables(0).Select("ID=" & intRowID)(0)

                    .Parameters("@NoteId").Value = drv("ListeNoteId") '// DRR 11.18.2011
                    .Parameters("@Codeliste").Value = Codeliste 'drv("Codeliste")
                    .Parameters("@Position").Value = drv("Pos")
                    .Parameters("@DigitalAsset").Value = drv("DigitalAsset")
                    .Parameters("@Picture").Value = drv("Picture")
                    .Parameters("@FreakOutMoment").Value = drv("FreakOutMoment")
                    .Parameters("@ID").Value = drv("ID")
                    .ExecuteNonQuery()

                    intID = CIntDB(.Parameters("@ID").Value)
                    egswErrorType = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                    If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception

                    For Each rowChild In row.GetChildRows("notetrans")
                        cmdTrans.Parameters("@Id").Value = rowChild("ListeNoteTransId") '// DRR 11.18.2011
                        cmdTrans.Parameters("@NoteID").Value = intID
                        cmdTrans.Parameters("@Codetrans").Value = rowChild("Codetrans")
                        cmdTrans.Parameters("@Note").Value = rowChild("Note")
                        cmdTrans.Parameters("@Comment").Value = rowChild("Comment")
                        cmdTrans.Parameters("@CookMode").Value = rowChild("CookMode")
                        cmdTrans.ExecuteNonQuery()

                        egswErrorType = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                        If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception
                    Next

                Next

                cn.Close()
                cn.Dispose()

            End With

            Return enumEgswErrorCode.OK

        Catch ex As Exception
            cn.Close()
            cn.Dispose()
            m_Err = ex
            L_ErrCode = egswErrorType
        Finally
        End Try
    End Function

    '// DRR 07.04.2011
    Public Function fctListeNoteDelete(ByVal strListNoteId As String) As enumEgswErrorCode

        Dim cn As SqlConnection
        Dim cmd As New SqlCommand
        Dim egswErrorType As enumEgswErrorCode = enumEgswErrorCode.GeneralError

        Try

            With cmd
                cn = New SqlConnection(L_strCnn)

                .Connection = cn
                .CommandText = "sp_EgswListeNoteTranslationDelete"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@listNoteID", SqlDbType.NVarChar, 2000).Value = "(" & strListNoteId & ")"
                .Parameters.Add("@retVal", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                cn.Open()
                .ExecuteNonQuery()
                egswErrorType = CType(.Parameters("@retVal").Value, enumEgswErrorCode)
                cn.Close()
                cn.Dispose()
            End With

            Return enumEgswErrorCode.OK
            If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception

        Catch ex As Exception
            cn.Close()
            cn.Dispose()
            m_Err = ex
            L_ErrCode = egswErrorType
        Finally
        End Try
    End Function

    ' MRC: 01/24/08 - For Recipe Encoding
    Public Function UpdateListeIngredients(ByVal intCodeListe As Integer, ByVal intSecondCode As Integer, ByVal intPosition As Integer, _
      ByVal strQty As String, ByVal strUnit As String, ByVal strName As String, ByVal strComplement As String, ByVal strPreparation As String, ByRef intRowID As Integer, _
      ByVal intStep As Integer) As enumEgswErrorCode

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim cmdSetPrice As New SqlCommand
        Dim st As SqlTransaction
        Dim da As New SqlDataAdapter

        Dim egswErrorType As enumEgswErrorCode = enumEgswErrorCode.GeneralError

        Try
            cmd.Connection = cn
            cmd.CommandText = "sp_EgswDetailsUpdate2"
            cmd.CommandType = CommandType.StoredProcedure

            'cmdSetPrice.Connection = cn
            'cmdSetPrice.CommandText = "sp_egswDetailSetPriceUpdate"
            'cmdSetPrice.CommandType = CommandType.StoredProcedure

            cn.Open()
            st = cn.BeginTransaction

            cmd.Transaction = st
            cmdSetPrice.Transaction = st

            With cmd
                .Parameters.Add("@intFirstCode", SqlDbType.Int)
                .Parameters.Add("@intSecondCode", SqlDbType.Int)
                .Parameters.Add("@intPosition", SqlDbType.Int)
                .Parameters.Add("@nvcQuantity", SqlDbType.NVarChar, 25)
                .Parameters.Add("@nvcUnit", SqlDbType.NVarChar, 25)
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 260)
                .Parameters.Add("@nvcComplement", SqlDbType.NVarChar, 2000)
                .Parameters.Add("@nvcPreparation", SqlDbType.NVarChar, 2000)

                .Parameters.Add("@intStep", SqlDbType.Int)

                '.Parameters.Add("@intWastage1", SqlDbType.Int)
                '.Parameters.Add("@intWastage2", SqlDbType.Int)
                '.Parameters.Add("@intWastage3", SqlDbType.Int)
                '.Parameters.Add("@intWastage4", SqlDbType.Int)
                '.Parameters.Add("@fltCoeff", SqlDbType.Float)
                '.Parameters.Add("@fltMenuCardApprovedPriceNew", SqlDbType.Float)
                '.Parameters.Add("@fltMenuCardCostPrice", SqlDbType.Float)
                .Parameters.Add("@intID", SqlDbType.Int).Direction = ParameterDirection.InputOutput
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                'cmdSetPrice.Parameters.Add("@intIDDetails", SqlDbType.Int)
                'cmdSetPrice.Parameters.Add("@intfirstCodeSetPrice", SqlDbType.Int)
                'cmdSetPrice.Parameters.Add("@intSecondCodeSetPrice", SqlDbType.Int)
                'cmdSetPrice.Parameters.Add("@fltConst", SqlDbType.Float)
                'cmdSetPrice.Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                ' Save
                .Parameters("@intFirstCode").Value = intCodeListe
                .Parameters("@intSecondCode").Value = intSecondCode
                .Parameters("@intPosition").Value = intPosition
                .Parameters("@nvcQuantity").Value = ReplaceSpecialCharacters(strQty)
                .Parameters("@nvcUnit").Value = ReplaceSpecialCharacters(strUnit)
                .Parameters("@nvcName").Value = ReplaceSpecialCharacters(strName)
                .Parameters("@nvcComplement").Value = ReplaceSpecialCharacters(strComplement)
                .Parameters("@nvcPreparation").Value = ReplaceSpecialCharacters(strPreparation)

                .Parameters("@intStep").Value = intStep

                .Parameters("@intID").Value = intRowID
                .ExecuteNonQuery()
                egswErrorType = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception

                intRowID = CInt(.Parameters("@intID").Value)

                '// save set price
                'cmdSetPrice.Parameters("@intIDDetails").Value = intRowID
                'cmdSetPrice.Parameters("@intfirstCodeSetPrice").Value = intRowfirstcodesetprice
                'cmdSetPrice.Parameters("@intSecondCodeSetPrice").Value = intRowSecondCodesetprice
                'cmdSetPrice.Parameters("@fltConst").Value = dblRowConst
                'cmdSetPrice.ExecuteNonQuery()
                'egswErrorType = CType(cmdSetPrice.Parameters("@retval").Value, enumEgswErrorCode)
                'If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception

                '// save translation
                'Save Translation
                'egswErrorType = UpdateIngredientTranslation(intRowID, intCodeTrans, strRowComplement, strRowPreparation, "", st)
                'If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception

                st.Commit()
                cn.Close()
                cn.Dispose()
            End With
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            st.Rollback()
            cn.Close()
            cn.Dispose()
            m_Err = ex
            L_ErrCode = egswErrorType
        End Try
    End Function

    Public Function UpdateListeIngredients(ByVal intCodeListe As Integer, ByVal intRowSecondCode As Integer, ByVal intRowPosition As Integer, _
      ByVal dblRowQuantity As Double, ByVal intRowCodeUnit As Integer, _
      ByVal strRowComplement As String, ByVal strRowPreparation As String, _
      ByVal intRowWastage1 As Integer, ByVal intRowWastage2 As Integer, ByVal intRowWastage3 As Integer, ByVal intRowWastage4 As Integer, ByVal intRowID As Integer, ByVal intRowfirstcodesetprice As Integer, ByVal intRowSecondCodesetprice As Integer, ByVal dblRowConst As Double, _
      ByVal dblRowMenuCardApprovedPriceNew As Double, ByVal dblRowMenuCardCostPrice As Double, ByVal intCodeTrans As Integer, Optional ByVal boolQuickEncode As Boolean = False) As enumEgswErrorCode

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim cmdSetPrice As New SqlCommand
        Dim st As SqlTransaction
        Dim da As New SqlDataAdapter

        Dim egswErrorType As enumEgswErrorCode = enumEgswErrorCode.GeneralError

        Try
            cmd.Connection = cn
            cmd.CommandText = "sp_EgswDetailsUpdate"
            cmd.CommandType = CommandType.StoredProcedure

            cmdSetPrice.Connection = cn
            cmdSetPrice.CommandText = "sp_egswDetailSetPriceUpdate"
            cmdSetPrice.CommandType = CommandType.StoredProcedure

            cn.Open()
            st = cn.BeginTransaction

            cmd.Transaction = st
            cmdSetPrice.Transaction = st

            With cmd
                .Parameters.Add("@intFirstCode", SqlDbType.Int)
                .Parameters.Add("@intSecondCode", SqlDbType.Int)
                .Parameters.Add("@intPosition", SqlDbType.Int)
                .Parameters.Add("@fltQuantity", SqlDbType.Float)
                .Parameters.Add("@intCodeUnit", SqlDbType.Int)
                .Parameters.Add("@nvcComplement", SqlDbType.NVarChar, 2000)
                .Parameters.Add("@nvcPreparation", SqlDbType.NVarChar, 700)
                .Parameters.Add("@intWastage1", SqlDbType.Int)
                .Parameters.Add("@intWastage2", SqlDbType.Int)
                .Parameters.Add("@intWastage3", SqlDbType.Int)
                .Parameters.Add("@intWastage4", SqlDbType.Int)
                .Parameters.Add("@fltCoeff", SqlDbType.Float)
                .Parameters.Add("@fltMenuCardApprovedPriceNew", SqlDbType.Float)
                .Parameters.Add("@fltMenuCardCostPrice", SqlDbType.Float)
                .Parameters.Add("@intID", SqlDbType.Int).Direction = ParameterDirection.InputOutput
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                cmdSetPrice.Parameters.Add("@intIDDetails", SqlDbType.Int)
                cmdSetPrice.Parameters.Add("@intfirstCodeSetPrice", SqlDbType.Int)
                cmdSetPrice.Parameters.Add("@intSecondCodeSetPrice", SqlDbType.Int)
                cmdSetPrice.Parameters.Add("@fltConst", SqlDbType.Float)
                cmdSetPrice.Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                '' MRC - 02/05/2008 Determines if the trigger ofthe update is through quick encoding.
                .Parameters.Add("@blnIsQuickEncode", SqlDbType.Bit)

                ' Save
                .Parameters("@intFirstCode").Value = intCodeListe
                .Parameters("@intSecondCode").Value = intRowSecondCode
                .Parameters("@intPosition").Value = intRowPosition
                .Parameters("@fltQuantity").Value = dblRowQuantity
                .Parameters("@intCodeUnit").Value = intRowCodeUnit
                .Parameters("@nvcComplement").Value = ReplaceSpecialCharacters(strRowComplement)
                .Parameters("@nvcPreparation").Value = ReplaceSpecialCharacters(strRowPreparation)
                .Parameters("@intWastage1").Value = intRowWastage1
                .Parameters("@intWastage2").Value = intRowWastage2
                .Parameters("@intWastage3").Value = intRowWastage3
                .Parameters("@intWastage4").Value = intRowWastage4
                .Parameters("@fltCoeff").Value = dblRowConst
                .Parameters("@fltMenuCardApprovedPriceNew").Value = dblRowMenuCardApprovedPriceNew
                .Parameters("@fltMenuCardCostPrice").Value = dblRowMenuCardCostPrice
                .Parameters("@intID").Value = intRowID

                ' MRC - 02/05/2008
                .Parameters("@blnIsQuickEncode").Value = boolQuickEncode
                .ExecuteNonQuery()
                egswErrorType = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception

                intRowID = CInt(.Parameters("@intID").Value)

                '// save set price
                cmdSetPrice.Parameters("@intIDDetails").Value = intRowID
                cmdSetPrice.Parameters("@intfirstCodeSetPrice").Value = intRowfirstcodesetprice
                cmdSetPrice.Parameters("@intSecondCodeSetPrice").Value = intRowSecondCodesetprice
                cmdSetPrice.Parameters("@fltConst").Value = dblRowConst
                cmdSetPrice.ExecuteNonQuery()
                egswErrorType = CType(cmdSetPrice.Parameters("@retval").Value, enumEgswErrorCode)
                If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception

                '// save translation
                'Save Translation
                egswErrorType = UpdateIngredientTranslation(intRowID, intCodeTrans, strRowComplement, strRowPreparation, "", st)
                If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception

                st.Commit()
                cn.Close()
                cn.Dispose()
            End With
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            st.Rollback()
            cn.Close()
            cn.Dispose()
            m_Err = ex
            L_ErrCode = egswErrorType
        End Try
    End Function

    ' MRC 02/06/08 - Added new params for updating the tmp fields on EgswDetails
    ' Changed param type of quantity from double to string because we should only save the text fields.
    Public Function UpdateListeIngredients2(ByVal intCodeListe As Integer, ByVal intRowSecondCode As Integer, ByVal intRowPosition As Integer, _
      ByVal dblRowQuantity As Double, ByVal intRowCodeUnit As Integer, _
      ByVal strRowComplement As String, ByVal strRowPreparation As String, _
      ByVal intRowWastage1 As Integer, ByVal intRowWastage2 As Integer, ByVal intRowWastage3 As Integer, ByVal intRowWastage4 As Integer, ByVal intRowID As Integer, ByVal intRowfirstcodesetprice As Integer, ByVal intRowSecondCodesetprice As Integer, ByVal dblRowConst As Double, _
      ByVal dblRowMenuCardApprovedPriceNew As Double, ByVal dblRowMenuCardCostPrice As Double, ByVal intCodeTrans As Integer, Optional ByVal boolQuickEncode As Boolean = False, _
      Optional ByVal strName As String = "", Optional ByVal strUnit As String = "", Optional ByVal strQty As String = "", Optional ByVal intStep As Integer = Nothing) As enumEgswErrorCode

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim cmdSetPrice As New SqlCommand
        Dim st As SqlTransaction
        Dim da As New SqlDataAdapter

        Dim egswErrorType As enumEgswErrorCode = enumEgswErrorCode.GeneralError

        Try
            cmd.Connection = cn
            cmd.CommandText = "sp_EgswDetailsUpdate3"
            cmd.CommandType = CommandType.StoredProcedure

            cmdSetPrice.Connection = cn
            cmdSetPrice.CommandText = "sp_egswDetailSetPriceUpdate"
            cmdSetPrice.CommandType = CommandType.StoredProcedure

            cn.Open()
            st = cn.BeginTransaction

            cmd.Transaction = st
            cmdSetPrice.Transaction = st

            With cmd
                .Parameters.Add("@intFirstCode", SqlDbType.Int)
                .Parameters.Add("@intSecondCode", SqlDbType.Int)
                .Parameters.Add("@intPosition", SqlDbType.Int)
                .Parameters.Add("@fltQuantity", SqlDbType.Float)
                .Parameters.Add("@intCodeUnit", SqlDbType.Int)
                .Parameters.Add("@nvcComplement", SqlDbType.NVarChar, 2000)
                .Parameters.Add("@nvcPreparation", SqlDbType.NVarChar, 700)
                .Parameters.Add("@intWastage1", SqlDbType.Int)
                .Parameters.Add("@intWastage2", SqlDbType.Int)
                .Parameters.Add("@intWastage3", SqlDbType.Int)
                .Parameters.Add("@intWastage4", SqlDbType.Int)
                .Parameters.Add("@fltCoeff", SqlDbType.Float)
                .Parameters.Add("@fltMenuCardApprovedPriceNew", SqlDbType.Float)
                .Parameters.Add("@fltMenuCardCostPrice", SqlDbType.Float)
                .Parameters.Add("@intID", SqlDbType.Int).Direction = ParameterDirection.InputOutput
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                cmdSetPrice.Parameters.Add("@intIDDetails", SqlDbType.Int)
                cmdSetPrice.Parameters.Add("@intfirstCodeSetPrice", SqlDbType.Int)
                cmdSetPrice.Parameters.Add("@intSecondCodeSetPrice", SqlDbType.Int)
                cmdSetPrice.Parameters.Add("@fltConst", SqlDbType.Float)
                cmdSetPrice.Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                '' MRC - 02/05/2008 Determines if the trigger ofthe update is through quick encoding.
                .Parameters.Add("@blnIsQuickEncode", SqlDbType.Bit)

                '' MRC - 02/05/2008 Additional Parameters to fill the tmp fields
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 30)
                .Parameters.Add("@nvcQty", SqlDbType.NVarChar, 30)
                .Parameters.Add("@nvcUnit", SqlDbType.NVarChar, 30)
                .Parameters.Add("@intStep", SqlDbType.Int)

                ' Save
                .Parameters("@intFirstCode").Value = intCodeListe
                .Parameters("@intSecondCode").Value = intRowSecondCode
                .Parameters("@intPosition").Value = intRowPosition
                .Parameters("@fltQuantity").Value = dblRowQuantity
                .Parameters("@intCodeUnit").Value = intRowCodeUnit
                .Parameters("@nvcComplement").Value = ReplaceSpecialCharacters(strRowComplement)
                .Parameters("@nvcPreparation").Value = ReplaceSpecialCharacters(strRowPreparation)
                .Parameters("@intWastage1").Value = intRowWastage1
                .Parameters("@intWastage2").Value = intRowWastage2
                .Parameters("@intWastage3").Value = intRowWastage3
                .Parameters("@intWastage4").Value = intRowWastage4
                .Parameters("@fltCoeff").Value = dblRowConst
                .Parameters("@fltMenuCardApprovedPriceNew").Value = dblRowMenuCardApprovedPriceNew
                .Parameters("@fltMenuCardCostPrice").Value = dblRowMenuCardCostPrice
                .Parameters("@intID").Value = intRowID

                ' MRC - 02/05/2008
                .Parameters("@blnIsQuickEncode").Value = boolQuickEncode
                .Parameters("@nvcName").Value = ReplaceSpecialCharacters(strName)
                .Parameters("@nvcQty").Value = ReplaceSpecialCharacters(strQty)
                .Parameters("@nvcUnit").Value = ReplaceSpecialCharacters(strUnit)
                .Parameters("@intStep").Value = intStep

                .ExecuteNonQuery()
                egswErrorType = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception

                intRowID = CInt(.Parameters("@intID").Value)

                '// save set price
                'cmdSetPrice.Parameters("@intIDDetails").Value = intRowID
                'cmdSetPrice.Parameters("@intfirstCodeSetPrice").Value = intRowfirstcodesetprice
                'cmdSetPrice.Parameters("@intSecondCodeSetPrice").Value = intRowSecondCodesetprice
                'cmdSetPrice.Parameters("@fltConst").Value = dblRowConst
                'cmdSetPrice.ExecuteNonQuery()
                'egswErrorType = CType(cmdSetPrice.Parameters("@retval").Value, enumEgswErrorCode)
                'If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception

                ''// save translation
                ''Save Translation
                'egswErrorType = UpdateIngredientTranslation(intRowID, intCodeTrans, strRowComplement, strRowPreparation, "", st)
                'If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception

                st.Commit()
                cn.Close()
                cn.Dispose()
            End With
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            st.Rollback()
            cn.Close()
            cn.Dispose()
            m_Err = ex
            L_ErrCode = egswErrorType
        End Try
    End Function
    'MRC  051908
    Public Function UpdateListeIngredientStep(ByVal intCode As Integer, ByVal strName As String, ByVal strProcedure As String, ByVal intStep As Integer, Optional ByVal intCodeTemplate As Integer = 0, Optional ByVal intPosition As Integer = 0) As enumEgswErrorCode
        Dim arrParam(6) As SqlParameter

        arrParam(0) = New SqlParameter("@intCode", intCode)
        arrParam(1) = New SqlParameter("@nvcName", strName)
        arrParam(2) = New SqlParameter("@nvcProcedure", strProcedure)
        arrParam(3) = New SqlParameter("@intStep", intStep)
        arrParam(4) = New SqlParameter("@intPosition", intPosition) 'JTOC 26.11.2012 Added position
        arrParam(5) = New SqlParameter("@intCodeTemplate", intCodeTemplate)
        arrParam(6) = New SqlParameter("@retval", SqlDbType.Int)
        arrParam(6).Direction = ParameterDirection.ReturnValue
        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswDetailsStepUpdate", arrParam)
            Return CType(arrParam(4).Value, enumEgswErrorCode)
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
            Throw ex
        End Try
    End Function

    Public Function UpdateListeSharing(ByVal intCode As Integer, ByVal arrCodeUserSharedTo As ArrayList, ByVal eShareType As ShareType) As enumEgswErrorCode
        If arrCodeUserSharedTo.Count = 0 Then Return enumEgswErrorCode.OK
        Dim arrParam(6) As SqlParameter

        arrParam(0) = New SqlParameter("@Code", intCode)
        arrParam(1) = New SqlParameter("@CodeUserSharedTo", arrCodeUserSharedTo(0))
        arrParam(2) = New SqlParameter("@Type", eShareType)
        arrParam(3) = New SqlParameter("@EgswTableDesc", "egswListe")
        arrParam(4) = New SqlParameter("@Status", DBNull.Value)
        arrParam(5) = New SqlParameter("@listeType", DBNull.Value)
        arrParam(6) = New SqlParameter("@retval", SqlDbType.Int)

        arrParam(6).Direction = ParameterDirection.ReturnValue

        Dim i As Integer = 1
        While i < arrCodeUserSharedTo.Count
            arrParam(1).Value = CInt(arrCodeUserSharedTo(i))
            i += 1
            Try
                ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_egswSharingUpdate", arrParam)
                L_ErrCode = CType(arrParam(6).Value, enumEgswErrorCode)
            Catch ex As Exception
                Throw ex
            End Try
        End While
    End Function
    'default update, uses codeuser to compatre if name already exits, if u nwt to use site as comparision, set it to true..specifically used for import where in you import it to other sites
    Public Function UpdateListe(ByRef info As structListe, Optional ByVal blnCompareByCodeSite As Boolean = False, _
     Optional ByVal strCodeMergeList As String = "", Optional ByVal OverwriteDescription As Integer = 1, _
     Optional ByVal blnAutoNum As Boolean = False, Optional ByVal blnAllowDuplicates As Boolean = False) As enumEgswErrorCode ' JBB 12.02.2010 Add Optional blnAllowDuplicates
        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim cmd As SqlCommand = New SqlCommand
        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_egswListeUpdate"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = info.Code
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = info.CodeSite
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = info.CodeUser
                .Parameters.Add("@intType", SqlDbType.Int).Value = info.Type
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(info.Name)
                .Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 20).Value = ReplaceSpecialCharacters(info.Number)
                .Parameters.Add("@nvcSubtitle", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(IIf(info.Subtitle = Nothing, "", info.Subtitle))
                .Parameters.Add("@intTemplate", SqlDbType.Int).Value = info.Template
                .Parameters.Add("@intBrand", SqlDbType.Int).Value = info.Brand
                .Parameters.Add("@intCategory", SqlDbType.Int).Value = info.Category
                .Parameters.Add("@intSource", SqlDbType.Int).Value = info.Source
                .Parameters.Add("@intSupplier", SqlDbType.Int).Value = info.Supplier
                .Parameters.Add("@fltYield", SqlDbType.Float).Value = info.Yield
                .Parameters.Add("@intYieldUnit", SqlDbType.Int).Value = info.YieldUnit
                .Parameters.Add("@sdtDates", SqlDbType.SmallDateTime).Value = info.Dates
                .Parameters.Add("@sntPercent", SqlDbType.SmallInt).Value = info.Percent
                .Parameters.Add("@fltSrQty", SqlDbType.Float).Value = info.srQty
                .Parameters.Add("@intSrUnit", SqlDbType.Int).Value = info.srUnit
                .Parameters.Add("@fltSrWeight", SqlDbType.Float).Value = ReturnDBNullIfNothing(info.srWeight)
                .Parameters.Add("@nvcPictureName", SqlDbType.NVarChar, 200).Value = ReplaceSpecialCharacters(info.PictureName)
                .Parameters.Add("@nvcNote", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(info.Note)
                .Parameters.Add("@nvcRemark", SqlDbType.NVarChar, 250).Value = ReplaceSpecialCharacters(info.Remark)
                .Parameters.Add("@sntWastage1", SqlDbType.SmallInt).Value = info.Wastage1
                .Parameters.Add("@sntWastage2", SqlDbType.SmallInt).Value = info.Wastage2
                .Parameters.Add("@sntWastage3", SqlDbType.SmallInt).Value = info.Wastage3
                .Parameters.Add("@sntWastage4", SqlDbType.SmallInt).Value = info.Wastage4
                .Parameters.Add("@nvcCoolingTime", SqlDbType.NVarChar, 25).Value = ReplaceSpecialCharacters(info.CoolingTime)
                .Parameters.Add("@nvcHeatingTime", SqlDbType.NVarChar, 25).Value = ReplaceSpecialCharacters(info.HeatingTime)
                .Parameters.Add("@nvcHeatingTemperature", SqlDbType.NVarChar, 25).Value = ReplaceSpecialCharacters(info.HeatingTemperature)
                .Parameters.Add("@nvcHeatingMode", SqlDbType.NVarChar, 25).Value = ReplaceSpecialCharacters(info.HeatingMode)
                .Parameters.Add("@nvcCCPDescription", SqlDbType.NVarChar, 255).Value = ReplaceSpecialCharacters(info.CCPDescription)
                .Parameters.Add("@nvcPreparation", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(info.Preparation)
                .Parameters.Add("@nvcStorage", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(info.Storage)
                .Parameters.Add("@nvcProductivity", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(info.Productivity)
                .Parameters.Add("@bitProtected", SqlDbType.Bit).Value = info.Protected
                .Parameters.Add("@intCodeLink", SqlDbType.Int).Value = info.CodeLink
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = info.IsGlobal
                .Parameters.Add("@bitUse", SqlDbType.Bit).Value = info.AllowUse
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = info.CodeTrans
                .Parameters.Add("@intTemplateCode", SqlDbType.Int).Value = info.TemplateCode 'VRP 04.04.2008

                .Parameters.Add("@PackagingCode", SqlDbType.Int).Value = info.Packaging 'JTOC 21.11.2012
                .Parameters.Add("@CertificationCode", SqlDbType.Int).Value = info.Certification 'JTOC 21.11.2012
                .Parameters.Add("@OriginCode", SqlDbType.Int).Value = info.Origin 'JTOC 21.11.2012
                .Parameters.Add("@TemperatureCode", SqlDbType.Int).Value = info.Temperature 'JTOC 21.11.2012
                .Parameters.Add("@InformationCode", SqlDbType.Int).Value = info.Information 'JTOC 21.11.2012

                If info.CookingTip = Nothing Then info.CookingTip = ""
                If info.Description = Nothing Then info.Description = ""
                If info.Refinement = Nothing Then info.Refinement = ""
                If info.Ingredients = Nothing Then info.Ingredients = ""
                .Parameters.Add("@nvcCookingTip", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(info.CookingTip)
                .Parameters.Add("@nvcDescription", SqlDbType.NVarChar, 800).Value = ReplaceSpecialCharacters(info.Description) '// DRR 02.22.2011 extend lenght from 700 to 800
                .Parameters.Add("@nvcIngredients", SqlDbType.NVarChar, 2000).Value = ReplaceSpecialCharacters(info.Ingredients)
                .Parameters.Add("@nvcRefinement", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(info.Refinement)

                ' for Menu Card
                If info.MenuCardCodeSetPrice = Nothing Then info.MenuCardCodeSetPrice = 0
                If info.MenuCardDateFrom = Nothing Then info.MenuCardDateFrom = Now
                If info.MenuCardDateUntil = Nothing Then info.MenuCardDateUntil = Now
                .Parameters.Add("@dteMenuCardDateFrom", SqlDbType.DateTime).Value = info.MenuCardDateFrom
                .Parameters.Add("@dteMenuCardDateUntil", SqlDbType.DateTime).Value = info.MenuCardDateUntil
                .Parameters.Add("@intMenuCardCodeSetPrice", SqlDbType.Int).Value = info.MenuCardCodeSetPrice

                .Parameters.Add("@intEGSRef", SqlDbType.Int).Value = ReturnDBNullIfNothing(info.EgsRef)
                .Parameters.Add("@intEGSID", SqlDbType.Int).Value = ReturnDBNullIfNothing(info.EgsID)

                .Parameters.Add("@intCodeListeNew", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Parameters.Add("@bitCompareByCodeSite", SqlDbType.Bit).Value = blnCompareByCodeSite

                .Parameters.Add("@vchCodeMergeList", SqlDbType.VarChar, 700).Value = strCodeMergeList
                .Parameters.Add("@vchKeyField", SqlDbType.VarChar, 50).Value = info.keyfield                'RDTC 09.08.2007
                .Parameters.Add("@intOverwriteDescription", SqlDbType.Int).Value = OverwriteDescription     'RDTC 17.08.2007
                .Parameters.Add("@fltNetWeight", SqlDbType.Float).Value = info.NetWeight                    'RDTC 06.03.2008

                If info.NoteHeader = Nothing Then info.NoteHeader = "" ' JBB 03.23.2011
                .Parameters.Add("@nvcNoteHeader", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(info.NoteHeader) 'VRP 24.06.2008
                .Parameters.Add("@bitAutoNum", SqlDbType.Bit).Value = blnAutoNum    'MRC 06.25.08

                If info.CodeStyle = Nothing Then info.CodeStyle = "" ' JBB
                .Parameters.Add("@nvcCodeStyle", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(info.CodeStyle) 'VRP 01.07.2008

                If info.StoringTime = Nothing Then info.StoringTime = "" 'DLS
                If info.StoringTemp = Nothing Then info.StoringTemp = "" 'DLS

                .Parameters.Add("@sStoringTime", SqlDbType.NVarChar, 100).Value = ReplaceSpecialCharacters(info.StoringTime) 'DLS
                .Parameters.Add("@sStoringTemp", SqlDbType.NVarChar, 100).Value = ReplaceSpecialCharacters(info.StoringTemp) 'DLS

                .Parameters.Add("@bitOnline", SqlDbType.Bit).Value = info.Online 'MRC - 08.27.08
                .Parameters.Add("@nvcProtectedNote", SqlDbType.NVarChar, 200).Value = ReplaceSpecialCharacters(info.ProtectedNote) 'MRC - 09.01.08
                .Parameters.Add("@nvcProtectedComment", SqlDbType.NVarChar, 200).Value = ReplaceSpecialCharacters(info.ProtectedComment) 'MRC - 09.01.08

                '-- VRP 23.02.2009
                .Parameters.Add("@fltPriceSmallPortion", SqlDbType.Float).Value = info.PriceSmallPortion
                .Parameters.Add("@fltPriceLargePortion", SqlDbType.Float).Value = info.PriceLargePortion
                '---

                'MRC 08.04.09
                .Parameters.Add("@tntDefaultPicture", SqlDbType.TinyInt).Value = info.DefaultPicture

                'JBB 12.02.2010 --@bAllowDuplicates 
                If blnAllowDuplicates = True Then
                    .Parameters.Add("@bAllowDuplicates", SqlDbType.Bit).Value = True
                End If

                'MRC 01.05.11   
                'Yield2
                .Parameters.Add("@fltYield2", SqlDbType.Float).Value = info.Yield2
                .Parameters.Add("@intYieldUnit2", SqlDbType.Int).Value = info.YieldUnit2

                'Portion Size
                .Parameters.Add("@fltPortionSize", SqlDbType.Float).Value = info.PortionSize
                .Parameters.Add("@intPortionSizeUnit", SqlDbType.Int).Value = info.PortionSizeUnit

                'Method Format -- JBB 01.24.2011
                .Parameters.Add("@chrMethodFrmt", SqlDbType.Char, 1).Value = info.MethodFormat

                '// Sub-Heading - DRR 02.22.2011
                .Parameters.Add("@nvcSubHeading", SqlDbType.NVarChar, 255).Value = ReplaceSpecialCharacters(info.Subheading)

                'JBB 03.24.2011
                .Parameters.Add("@nvcFootNote1", SqlDbType.NVarChar, 4000).Value = ReplaceSpecialCharacters(info.FootNote1)
                .Parameters.Add("@nvcFootNote2", SqlDbType.NVarChar, 4000).Value = ReplaceSpecialCharacters(info.FootNote2)


                '--JBB 07.05.2011
                If info.Type = 8 Then
                    .Parameters.Add("@Standard", SqlDbType.Int).Value = info.Standard
                    .Parameters.Add("@Difficulty", SqlDbType.Int).Value = info.Difficulty
                    .Parameters.Add("@Budget", SqlDbType.Int).Value = info.Budget
                    If info.QuicknEasy <> -1 Then
                        .Parameters.Add("@QuickAndEasy", SqlDbType.Bit).Value = IIf(info.QuicknEasy = 1, True, False)
                    End If
                    .Parameters.Add("@ShowOff", SqlDbType.Bit).Value = info.ShowOff
                    .Parameters.Add("@ChefRecommended", SqlDbType.Bit).Value = info.ChefRecommended
                End If
                '--

                If info.Type = 2 Then
                    .Parameters.Add("@nvcUPC", SqlDbType.NVarChar, 25).Value = ReplaceSpecialCharacters(info.UPC)
                End If

                If info.Type = 8 Then
                    .Parameters.Add("@CostperServing", SqlDbType.Float).Value = info.CostperServing
                    .Parameters.Add("@CostperRecipe", SqlDbType.Float).Value = info.CostperRecipe
                    .Parameters.Add("@LegacyNumber", SqlDbType.NVarChar).Value = info.LegacyNumber
                    .Parameters.Add("@ServeWith", SqlDbType.NVarChar).Value = info.ServeWith

                    .Parameters.Add("@IsSiteAutonumber", SqlDbType.Bit).Value = info.IsSiteAutonumber 'JTOC 11.28.2013
                End If


                cn.Open()
                .ExecuteNonQuery()
                info.Code = CInt(.Parameters("@intCodeListeNew").Value)
                cn.Close()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            info.Code = -1
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateListeMerchandiseMain(ByRef info As structListe, Optional ByVal blnCompareByCodeSite As Boolean = False, _
     Optional ByVal strCodeMergeList As String = "", Optional ByVal OverwriteDescription As Integer = 1) As enumEgswErrorCode
        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim cmd As SqlCommand = New SqlCommand
        Try
            With cmd
                .Connection = cn
                .CommandText = "[sp_EgswListeUpdateMain]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = info.Code
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = info.CodeSite
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = info.CodeUser
                .Parameters.Add("@intType", SqlDbType.Int).Value = info.Type
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(info.Name)
                .Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 20).Value = ReplaceSpecialCharacters(info.Number)
                .Parameters.Add("@intBrand", SqlDbType.Int).Value = info.Brand
                .Parameters.Add("@intCategory", SqlDbType.Int).Value = info.Category
                .Parameters.Add("@intSource", SqlDbType.Int).Value = info.Source
                .Parameters.Add("@intSupplier", SqlDbType.Int).Value = info.Supplier
                .Parameters.Add("@fltYield", SqlDbType.Float).Value = info.Yield
                .Parameters.Add("@intYieldUnit", SqlDbType.Int).Value = info.YieldUnit
                .Parameters.Add("@sdtDates", SqlDbType.SmallDateTime).Value = info.Dates
                .Parameters.Add("@sntPercent", SqlDbType.SmallInt).Value = info.Percent
                .Parameters.Add("@fltSrQty", SqlDbType.Float).Value = info.srQty
                .Parameters.Add("@intSrUnit", SqlDbType.Int).Value = info.srUnit
                .Parameters.Add("@fltSrWeight", SqlDbType.Float).Value = ReturnDBNullIfNothing(info.srWeight)
                .Parameters.Add("@nvcPictureName", SqlDbType.NVarChar, 200).Value = ReplaceSpecialCharacters(info.PictureName)
                .Parameters.Add("@nvcNote", SqlDbType.NVarChar, 2000).Value = ReplaceSpecialCharacters(info.Note)
                .Parameters.Add("@nvcRemark", SqlDbType.NVarChar, 250).Value = ReplaceSpecialCharacters(info.Remark)
                .Parameters.Add("@sntWastage1", SqlDbType.SmallInt).Value = info.Wastage1
                .Parameters.Add("@sntWastage2", SqlDbType.SmallInt).Value = info.Wastage2
                .Parameters.Add("@sntWastage3", SqlDbType.SmallInt).Value = info.Wastage3
                .Parameters.Add("@sntWastage4", SqlDbType.SmallInt).Value = info.Wastage4
                .Parameters.Add("@nvcCoolingTime", SqlDbType.NVarChar, 25).Value = ReplaceSpecialCharacters(info.CoolingTime)
                .Parameters.Add("@nvcHeatingTime", SqlDbType.NVarChar, 25).Value = ReplaceSpecialCharacters(info.HeatingTime)
                .Parameters.Add("@nvcHeatingTemperature", SqlDbType.NVarChar, 25).Value = ReplaceSpecialCharacters(info.HeatingTemperature)
                .Parameters.Add("@nvcHeatingMode", SqlDbType.NVarChar, 25).Value = ReplaceSpecialCharacters(info.HeatingMode)
                .Parameters.Add("@nvcCCPDescription", SqlDbType.NVarChar, 255).Value = ReplaceSpecialCharacters(info.CCPDescription)
                .Parameters.Add("@nvcPreparation", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(info.Preparation)
                .Parameters.Add("@nvcStorage", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(info.Storage)
                .Parameters.Add("@nvcProductivity", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(info.Productivity)
                .Parameters.Add("@bitProtected", SqlDbType.Bit).Value = info.Protected
                .Parameters.Add("@intCodeLink", SqlDbType.Int).Value = info.CodeLink
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = info.IsGlobal
                .Parameters.Add("@bitUse", SqlDbType.Bit).Value = info.AllowUse
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = info.CodeTrans

                If info.CookingTip = Nothing Then info.CookingTip = ""
                If info.Description = Nothing Then info.Description = ""
                If info.Refinement = Nothing Then info.Refinement = ""
                If info.Ingredients = Nothing Then info.Ingredients = ""
                .Parameters.Add("@nvcCookingTip", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(info.CookingTip)
                .Parameters.Add("@nvcDescription", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(info.Description)
                .Parameters.Add("@nvcIngredients", SqlDbType.NVarChar, 2000).Value = ReplaceSpecialCharacters(info.Ingredients)
                .Parameters.Add("@nvcRefinement", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(info.Refinement)

                ' for Menu Card
                If info.MenuCardCodeSetPrice = Nothing Then info.MenuCardCodeSetPrice = 0
                If info.MenuCardDateFrom = Nothing Then info.MenuCardDateFrom = Now
                If info.MenuCardDateUntil = Nothing Then info.MenuCardDateUntil = Now
                .Parameters.Add("@dteMenuCardDateFrom", SqlDbType.DateTime).Value = info.MenuCardDateFrom
                .Parameters.Add("@dteMenuCardDateUntil", SqlDbType.DateTime).Value = info.MenuCardDateUntil
                .Parameters.Add("@intMenuCardCodeSetPrice", SqlDbType.Int).Value = info.MenuCardCodeSetPrice

                .Parameters.Add("@intEGSRef", SqlDbType.Int).Value = ReturnDBNullIfNothing(info.EgsRef)
                .Parameters.Add("@intEGSID", SqlDbType.Int).Value = ReturnDBNullIfNothing(info.EgsID)

                .Parameters.Add("@intCodeListeNew", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Parameters.Add("@bitCompareByCodeSite", SqlDbType.Bit).Value = blnCompareByCodeSite

                .Parameters.Add("@vchCodeMergeList", SqlDbType.VarChar, 700).Value = strCodeMergeList
                .Parameters.Add("@vchKeyField", SqlDbType.VarChar, 50).Value = info.keyfield                'RDTC 09.08.2007
                .Parameters.Add("@intOverwriteDescription", SqlDbType.Int).Value = OverwriteDescription     'RDTC 17.08.2007



                cn.Open()
                .ExecuteNonQuery()
                info.Code = CInt(.Parameters("@intCodeListeNew").Value)
                cn.Close()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            info.Code = -1
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateListeProcedure(ByRef info As structListe, Optional ByVal blnCompareByCodeSite As Boolean = False, _
     Optional ByVal strCodeMergeList As String = "", Optional ByVal OverwriteDescription As Integer = 1) As enumEgswErrorCode
        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim cmd As SqlCommand = New SqlCommand
        Try
            With cmd
                .Connection = cn
                .CommandText = "[sp_EgswListeUpdateProcedure]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = info.Code
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = info.CodeTrans
                .Parameters.Add("@nvcNote", SqlDbType.NVarChar, 2000).Value = ReplaceSpecialCharacters(info.Note)
                .Parameters.Add("@nvcDescription", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(info.Description)

                .Parameters.Add("@intCodeListeNew", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                cn.Open()
                .ExecuteNonQuery()
                info.Code = CInt(.Parameters("@intCodeListeNew").Value)
                cn.Close()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            info.Code = -1
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateListeTranslations(ByVal dt As DataTable, Optional ByVal intCodeListeOverride As Integer = -1) As enumEgswErrorCode
        Dim row As DataRow
        Dim info As structListeTranslation

        Try
            For Each row In dt.Rows
                ' Get Values
                With row
                    If intCodeListeOverride = -1 Then
                        info.CodeListe = CInt(.Item("codeListe"))
                    Else
                        info.CodeListe = intCodeListeOverride
                    End If

                    info.Name = CStr(.Item("name"))
                    info.Note = CStrDB(.Item("note"))
                    info.Preparation = CStrDB(.Item("preparation"))
                    info.Remark = CStrDB(.Item("remark"))
                    info.CodeTrans = CInt(.Item("codeTrans"))
                    info.CCPDescription = CStrDB(.Item("CCPDescription"))
                    info.Ingredients = CStrDB(.Item("Ingredients"))
                    info.Preparation = CStrDB(.Item("Preparation"))
                    info.CookingTip = CStrDB(.Item("CookingTip"))
                    info.Refinement = CStrDB(.Item("Refinement"))
                    info.Storage = CStrDB(.Item("Storage"))
                    info.Productivity = CStrDB(.Item("Productivity"))
                    info.Description = CStrDB(.Item("Description"))
                    info.NoteHeader = CStrDB(.Item("NoteHeader")) 'VRP 26.06.2008

                    info.StoringTime = CStrDB(.Item("StoringTime")) 'DLS
                    info.StoringTemp = CStrDB(.Item("StoringTemp")) 'DLS

                    info.Subtitle = CStrDB(.Item("Subtitle")) 'ADR 03.25.11
                    info.Subheading = CStrDB(.Item("Subheading")) 'ADR 03.25.11

                    info.FootNote1 = CStrDB(.Item("FootNote1")) 'ADR 04.04.11
                    info.FootNote2 = CStrDB(.Item("FootNote2")) 'ADR 04.04.11
                    info.ServeWith = CStrDB(.Item("ServeWith")) 'JBB 05.08.2012
                End With
                ' Save
                L_ErrCode = UpdateListeTranslation(info)
                If L_ErrCode <> enumEgswErrorCode.OK Then Throw New Exception
            Next
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            Throw New Exception(ex.Message, ex)
        End Try

        Return L_ErrCode
    End Function

    Public Function UpdateListeNameTranslations(ByVal intCodeListe As Integer, ByVal strName As String, ByVal intCodeTrans As Integer) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_EgswUpdateTranslationName"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 260).Value = strName
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans

                cn.Open()
                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()


            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateStepTranslations(ByVal dt As DataTable, Optional ByVal intCodeListeOverride As Integer = -1) As enumEgswErrorCode
        Dim row As DataRow
        Dim info As structStepTranslation

        Try
            For Each row In dt.Rows
                ' Get Values
                With row
                    If intCodeListeOverride = -1 Then
                        info.CodeListe = CInt(.Item("codeListe"))
                    Else
                        info.CodeListe = intCodeListeOverride
                    End If

                    info.StepNum = CInt(.Item("step"))
                    info.Name = CStr(.Item("name"))
                    info.Procedure = CStr(.Item("procedure"))
                    info.CodeTrans = CInt(.Item("codetrans"))
                End With
                ' Save
                L_ErrCode = UpdateStepTranslation(info)
                If L_ErrCode <> enumEgswErrorCode.OK Then Throw New Exception
            Next
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            Throw New Exception(ex.Message, ex)
        End Try

        Return L_ErrCode
    End Function

    Public Function UpdateListeTranslation(ByVal info As structListeTranslation) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_egswListeTranslationUpdate"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = CStrDB(info.CodeListe)
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(CStrDB(info.Name))
                .Parameters.Add("@nvcNote", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(CStrDB(info.Note))
                .Parameters.Add("@nvcRemark", SqlDbType.NVarChar, 250).Value = ReplaceSpecialCharacters(CStrDB(info.Remark))
                .Parameters.Add("@nvcCCPDescription", SqlDbType.NVarChar, 255).Value = ReplaceSpecialCharacters(CStrDB(info.CCPDescription))
                .Parameters.Add("@nvcIngredients", SqlDbType.NVarChar, 2000).Value = ReplaceSpecialCharacters(CStrDB(info.Ingredients))
                .Parameters.Add("@nvcPreparation", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(CStrDB(info.Preparation))
                .Parameters.Add("@nvcCookingTip", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(CStrDB(info.CookingTip))
                .Parameters.Add("@nvcRefinement", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(CStrDB(info.Refinement))
                .Parameters.Add("@nvcStorage", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(CStrDB(info.Storage))
                .Parameters.Add("@nvcProductivity", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(CStrDB(info.Productivity))
                .Parameters.Add("@nvcDescription", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(CStrDB(info.Description))
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = CStrDB(info.CodeTrans)
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                .Parameters.Add("@nvcNoteHeader", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(CStrDB(info.NoteHeader)) 'VRP 26.06.2008

                .Parameters.Add("@nvcStoringTemp", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(CStrDB(info.StoringTemp)) 'DLS 31.07.2008
                .Parameters.Add("@nvcStoringTime", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(CStrDB(info.StoringTime)) 'DLS 31.07.2008

                .Parameters.Add("@nvcSubtitle", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(CStrDB(info.Subtitle)) 'ADR 03.25.11
                .Parameters.Add("@nvcSubheading", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(CStrDB(info.Subheading)) 'ADR 03.25.11

                .Parameters.Add("@nvcFootNote1", SqlDbType.NVarChar, 4000).Value = ReplaceSpecialCharacters(CStrDB(info.FootNote1)) 'ADR 04.04.11
                .Parameters.Add("@nvcFootNote2", SqlDbType.NVarChar, 4000).Value = ReplaceSpecialCharacters(CStrDB(info.FootNote2)) 'ADR 04.04.11

                .Parameters.Add("@nvcServeWith", SqlDbType.NVarChar, 1000).Value = ReplaceSpecialCharacters(CStrDB(info.ServeWith)) 'ADR 04.04.11

                cn.Open()
                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateStepTranslation(ByVal info As structStepTranslation) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_EgswStepTranslationUpdate"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = CStrDB(info.CodeListe)
                .Parameters.Add("@intStep", SqlDbType.Int).Value = CStrDB(info.StepNum)
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(CStrDB(info.Name))
                .Parameters.Add("@nvcProcedure", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(CStrDB(info.Procedure))
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = CStrDB(info.CodeTrans)
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                cn.Open()
                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateListeChangeYield(ByVal intCode As Integer, ByVal dblYieldNew As Double, ByVal dblPercentNew As Double) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataSet
        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_egswListeUpdateChangeYield"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@fltYieldNew", SqlDbType.Float).Value = dblYieldNew
                .Parameters.Add("@fltPercentNew", SqlDbType.Float).Value = dblPercentNew
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                cn.Open()
                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function


    Public Function UpdateListeChangeYieldDT(ByVal intCode As Integer, ByVal dblYieldNew As Double, ByVal dblPercentNew As Double, Optional ByVal intCodeUser As Integer = -1) As DataTable
        Dim cmd As SqlCommand = New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "sp_EgswListeUpdateChangeYieldDT"
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@fltYieldNew", SqlDbType.Float).Value = dblYieldNew
                .Parameters.Add("@fltPercentNew", SqlDbType.Float).Value = dblPercentNew
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser 'JTOC 05.04.2013 Added CodeUser 
                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
            End With
            Return dt
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function UpdateListeQuantityChangeYield(ByVal intCode As Integer, ByVal dblYieldOld As Double) As enumEgswErrorCode ', ByVal dblPercentNew As Double) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataSet
        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_egswListeUpdateQuantityChangeYield"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@fltYieldOld", SqlDbType.Float).Value = dblYieldOld
                '.Parameters.Add("@fltPercentNew", SqlDbType.Float).Value = dblPercentNew
                '.Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                cn.Open()
                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()

                'L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateListeChangeYield2(ByVal intCode As Integer, ByVal dblYieldNew As Double) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataSet
        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_egswListeUpdateChangeYield2"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@fltYieldNew", SqlDbType.Float).Value = dblYieldNew
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                cn.Open()
                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function


    Public Function UpdateMerchandiseAllRecipesKeywordsDerived(ByVal intCodeliste As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_egswListeMerchandiseUpdateAllRecipesKeywordsDerived"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeliste
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                cmd.Connection.Open()
                .ExecuteNonQuery()
                cmd.Connection.Close()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdateRecipesKeywordsDerived(ByVal intCodeliste As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswListeRecipesUpdateKeywordsDerived"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeliste
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    '// DRR 06.01.2012
    Public Function UpdateListeFullTranslation(ByVal nCode As Integer, ByVal nCodeSite As Integer, ByVal nFlag As enumFullTranslation) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandTimeout = 0
                .CommandText = "[dbo].[sp_EgswListeFullyTranslateUpdate]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@nCode", SqlDbType.Int).Value = nCode
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = nCodeSite
                .Parameters.Add("@Flag", SqlDbType.Int).Value = nFlag
                .Parameters.Add("@Output", SqlDbType.Bit).Direction = ParameterDirection.Output
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        Return L_ErrCode
    End Function
    '//

    Public Function UpdateListeChangeNote(ByVal intCodeTrans As Integer, ByVal intCode As Integer, ByVal strNote As String) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim st As SqlTransaction
        Dim da As New SqlDataAdapter
        Dim dt As New DataSet

        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_egswListeUpdateChangeNote"
                .CommandType = CommandType.StoredProcedure

                cn.Open()

                st = cn.BeginTransaction
                .Transaction = st

                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCode
                .Parameters.Add("@nvcNote", SqlDbType.NVarChar, 2000).Value = ReplaceSpecialCharacters(strNote)
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                .ExecuteNonQuery()

                st.Commit()
                cn.Close()
                cn.Dispose()
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With

        Catch ex As Exception
            st.Rollback()
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function CopyListeAllToSite(ByVal intCodeListe As Integer, ByVal intCodeSiteTo As Integer) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Try
            With cmd
                .Connection = cn
                .CommandText = "LISTE_CopyListeToSite"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 400
                .Parameters.Add("@intCodeSiteNew", SqlDbType.Int).Value = intCodeSiteTo
                .Parameters.Add("@intCodeliste", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@retVal", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                cn.Open()
                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()
                '    intNewCode = CInt(.Parameters("@intCodeNew").Value)
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With

        Catch ex As Exception
            '      intNewCode = -1
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function CopyListeAll(ByVal intCodeListe As Integer, ByVal intCodeUserNew As Integer, Optional ByVal intCodeSite As Integer = 1, Optional ByRef intNewCode As Integer = -1, Optional ByVal blIsVersion As Boolean = False, Optional ByVal blIsCookmode As Boolean = False, Optional ByVal intCategory As Integer = -1) As enumEgswErrorCode ' JBB 12.02.2010 Add Optional blIsVersion ' JBB 07.012011 Add Optional ByVal blIsCookmode As Boolean = False
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Try
            ', Optional ByVal intCodeSite As Integer = 1
            With cmd
                .Connection = cn
                .CommandText = "sp_egswListeCopyAll"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 120
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUserNew
                .Parameters.Add("@intCodeliste", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@intCodenew", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@retVal", SqlDbType.Int).Direction = ParameterDirection.ReturnValue
                .Parameters.Add("@IsVersion", SqlDbType.Bit).Value = blIsVersion ' JBB 12.02.2010
                .Parameters.Add("@IsCookMode", SqlDbType.Bit).Value = blIsCookmode ' JBB 17.01.2011
                .Parameters.Add("@intCategory", SqlDbType.Int).Value = intCategory  ' JTOC 12.03.2013
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite 'mkam re-add to v46
                cn.Open()
                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()
                intNewCode = CInt(.Parameters("@intCodeNew").Value)
                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With

        Catch ex As Exception
            intNewCode = -1
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    'JBB 07.01.2011
    Public Function CheckListeCookmodeVersion(ByVal intCodeListe As Integer, Optional ByVal blIsCookmode As Boolean = False) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim errcode As Integer
        Try
            With cmd
                .Connection = cn
                .CommandText = "EgswCheckCookmodeVersion"
                .CommandType = CommandType.StoredProcedure

                If cn.State = ConnectionState.Closed Then cn.Open()

                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@IsCookmode", SqlDbType.Bit).Value = blIsCookmode
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()

                errcode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            errcode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return errcode
    End Function



    Public Function UpdateListeChangeOnlineFlag(ByVal intCode As Integer, ByVal blnNewValue As Boolean, ByVal intCodeUser As Integer) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_egswListeUpdateChangeFlag"
                .CommandType = CommandType.StoredProcedure

                If cn.State = ConnectionState.Closed Then cn.Open()

                .Parameters.Add("@nvcFieldName", SqlDbType.NVarChar, 50)
                .Parameters.Add("@intCodeListe", SqlDbType.Int)
                .Parameters.Add("@Flag", SqlDbType.Bit)
                .Parameters.Add("@intCodeUser", SqlDbType.Int)
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .Parameters.Item("@nvcFieldName").Value = "ONLINE"
                .Parameters.Item("@intCodeListe").Value = intCode
                .Parameters.Item("@Flag").Value = blnNewValue
                .Parameters.Item("@intCodeUser").Value = intCodeUser
                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function


    Public Function GetListeIsCookmode(ByVal intCodeListe As Integer) As Boolean
        Dim blCookMode As Boolean
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader

        With cmd
            .Connection = cn
            .CommandText = "Select ISNULL(Cookmode,0) As Cookmode " & _
                "FROM	egswListe r " & _
                "where r.code=@intCode "
            .CommandType = CommandType.Text
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCodeListe
            Try
                cn.Open()
                dr = .ExecuteReader(CommandBehavior.CloseConnection)
                If dr.Read Then
                    blCookMode = CBoolDB(dr.Item("Cookmode"))
                End If
                dr.Close()
                cn.Close()
            Catch ex As Exception
                cn.Close()
                Throw ex
            End Try
        End With
        Return blCookMode
    End Function

    Public Function UpdateListeSubmittedField(ByVal arr As ArrayList, ByVal bNewValue As Boolean, ByVal intCodeUser As Integer) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim st As SqlTransaction
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim nLastIndex As Integer = arr.Count - 1
        Dim counter As Integer
        Dim intCode As Integer

        Dim egswErrorType As enumEgswErrorCode = enumEgswErrorCode.GeneralError
        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_egswListeUpdateChangeFlag"
                .CommandType = CommandType.StoredProcedure

                If cn.State = ConnectionState.Closed Then cn.Open()

                st = cn.BeginTransaction
                .Transaction = st

                .Parameters.Add("@nvcFieldName", SqlDbType.NVarChar, 50)
                .Parameters.Add("@intCodeListe", SqlDbType.Int)
                .Parameters.Add("@Flag", SqlDbType.Bit)
                .Parameters.Add("@intCodeUser", SqlDbType.Int)
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                For counter = 0 To nLastIndex
                    intCode = CInt(arr(counter))
                    .Parameters.Item("@nvcFieldName").Value = "SUBMITTED"
                    .Parameters.Item("@intCodeListe").Value = intCode
                    .Parameters.Item("@Flag").Value = bNewValue
                    .Parameters.Item("@intCodeUser").Value = intCodeUser
                    .ExecuteNonQuery()

                    egswErrorType = CType(.Parameters("@retval").Value, enumEgswErrorCode)
                    If egswErrorType <> enumEgswErrorCode.OK Then Throw New Exception
                Next

                st.Commit()
                cn.Close()
                cn.Dispose()
                L_ErrCode = egswErrorType
            End With

        Catch ex As Exception
            st.Rollback()
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function FixRecipeIngredientsPrices(ByVal intCode As Integer, ByVal intCodesetprice As Integer) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_egswListeRecipeFixPrices"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodesetprice
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                cn.Open()
                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function RecomputeRecipesAndMenusOfItem(ByVal intSecondCode As Integer, ByVal intCodeSetPrice As Integer, intMetricImp As Integer) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_egswListeItemsRecomputeRecipesAndMenus"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 0 'AGL 2013.08.16 - 7850 - to avoid timeout
                .Parameters.Add("@intSecondCode", SqlDbType.Int).Value = intSecondCode
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice
                .Parameters.Add("@intMetricImp", SqlDbType.Int).Value = intMetricImp
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                cn.Open()
                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function

    ''' <summary>
    ''' Creates a copy of the liste for System Site ownership and the new copy is exposed / shared to all users
    ''' </summary>
    ''' <param name="intCodeuser"></param>
    ''' <param name="intCodeListe"></param>
    ''' <param name="intCodeListeNew"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ConvertToSystem(ByVal intCodeuser As Integer, ByVal intCodeListe As Integer, ByRef intCodeListeNew As Integer) As enumEgswErrorCode
        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@retval", SqlDbType.Int)
        arrParam(0).Direction = ParameterDirection.ReturnValue
        arrParam(1) = New SqlParameter("@intCodeUser", intCodeuser)
        arrParam(2) = New SqlParameter("@intCodeListe", intCodeListe)
        arrParam(3) = New SqlParameter("@intCodeListeNew", SqlDbType.Int)
        arrParam(3).Direction = ParameterDirection.Output

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswListeUpdateConvertToSytem", arrParam, 600)
            intCodeListeNew = CInt(arrParam(3).Value)
            Return CType(arrParam(0).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    '    Public Function Addlist(ByVal type As enumDataListType, ByVal strNumber As String, ByVal strName As String, ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, Optional ByVal dblYield As Double = 0, Optional ByVal intYieldUnit As Integer = 0) As Integer
    '        Dim tRnListe As New EgswTables.Liste
    '        ' init
    '        tRnListe.CCPDescription = ""
    '        tRnListe.CoolingTime = ""
    '        tRnListe.HeatingMode = ""
    '        tRnListe.HeatingTime = ""
    '        tRnListe.Storage = ""
    '        tRnListe.Preparation = ""
    '        tRnListe.Name = strName
    '        tRnListe.Number = strNumber
    '        tRnListe.Code = -1
    '        tRnListe.CodeTrans = intCodeTrans
    '        tRnListe.CodeSite = intCodeSite
    '        tRnListe.CodeUser = -1
    '        tRnListe.Dates = Now.Date
    '        tRnListe.Wastage1 = 0
    '        tRnListe.Wastage2 = 0
    '        tRnListe.Wastage3 = 0
    '        tRnListe.Wastage4 = 0
    '        tRnListe.Type = type
    '        tRnListe.Yield = dblYield
    '        tRnListe.YieldUnit = intYieldUnit
    '        tRnListe.Source = 0
    '        tRnListe.Note = ""
    '        tRnListe.Remark = ""
    '        tRnListe.Percent = 0
    '        tRnListe.srQty = 0
    '        tRnListe.srUnit = 0
    '        tRnListe.PictureName = ";;;"

    '        ' Unilever

    '        Dim cSupplier As New clsSupplier
    '        Dim cBrand As New clsBrand
    '        Dim cCategory As New clsCategory

    '        cBrand.SetConnection(L_strCnn)
    '        cCategory.SetConnection(L_strCnn)
    '        cSupplier.SetConnection(L_strCnn)

    '        If type = enumDataListType.Merchandise Then
    '            tRnListe.Brand = cBrand.GetBrandCode("Not defined", nOwner, enumDataListType.Merchandise, nLanguageID)
    '            tRnListe.Supplier = cSupplier.GetSuppliercode("No Supplier", nOwner)
    '            tRnListe.Category = cCategory.GetCategoryCode("Not defined", nOwner, enumDataListType.Merchandise, nLanguageID)
    '        Else
    '            tRnListe.Brand = 0
    '            tRnListe.Supplier = 0
    '            tRnListe.Category = 0
    '        End If

    '        Dim nReturnValue As Integer = Me.UpdateListe(tRnListe)
    '        If (nReturnValue = -2 Or tRnListe.Code = -1) And type = enumDataListType.Merchandise Then ' Duplicate number
    '            Dim c As Integer = 0
    '            Dim origNumber As String = tRnListe.Number
    '            Dim origName As String = tRnListe.Name
    '            Dim newname As String
    '            Dim retValue As Integer
    'GenerateNewName:
    '            c += 1
    '            tRnListe.Name = origName & "(" & c & ")"

    '            If type = enumDataListType.Merchandise And tRnListe.Number <> "" Then
    '                tRnListe.Number = origNumber & "(" & c & ")"
    '            End If

    '            nReturnValue = Me.UpdateListe(tRnListe)
    '            If nReturnValue = -2 Or tRnListe.Code = -1 Then
    '                GoTo GenerateNewName
    '            End If
    '        End If
    '        Return tRnListe.Code
    '    End Function

    Public Function UpdateListeTextPurge(ByVal intCodeSite As Integer, ByVal roleLevel As enumGroupLevel) As enumEgswErrorCode
        Try
            Dim arrParam(2) As SqlParameter
            arrParam(0) = New SqlParameter("@intCodeSite", intCodeSite)
            arrParam(1) = New SqlParameter("@intRoleLevel", roleLevel)
            arrParam(2) = New SqlParameter("@retval", SqlDbType.Int)

            arrParam(2).Direction = ParameterDirection.ReturnValue

            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswListeTextUpdatePurge", arrParam)

            L_ErrCode = CType(arrParam(2).Value, enumEgswErrorCode)
        Catch
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    'VRP 29.08.2007
    Public Function RecalculateRecipeKeywords() As enumEgswErrorCode
        Dim sqlCn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand

        Try
            With sqlCmd
                .Connection = sqlCn
                .CommandText = "sp_EgsWRecomputeRecipeKeywords"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 1000
                sqlCn.Open()
                .ExecuteNonQuery()
                sqlCn.Close()
                sqlCn.Dispose()
                Return enumEgswErrorCode.OK
            End With
        Catch ex As Exception
            sqlCn.Close()
            sqlCn.Dispose()
            Return enumEgswErrorCode.GeneralError
        End Try

    End Function

    Public Function RecalculateNutrients(ByVal intCodeUser As Integer, ByVal type As enumDataListType) As enumEgswErrorCode
        Try
            Dim arrParam(1) As SqlParameter
            arrParam(0) = New SqlParameter("@intCodeUser", intCodeUser)
            arrParam(1) = New SqlParameter("@intCodeListeType", type)
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswNutrientValUpdateRecomputeAll", arrParam)
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function RecalculatePrice() As enumEgswErrorCode
        Dim sqlCn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand

        Try
            With sqlCmd
                .Connection = sqlCn
                .CommandText = "sp_EgswRecalculatePrice"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 50000
                sqlCn.Open()
                .ExecuteNonQuery()
                sqlCn.Close()
                sqlCn.Dispose()
                Return enumEgswErrorCode.OK
            End With
        Catch ex As Exception
            sqlCn.Close()
            sqlCn.Dispose()
            Return enumEgswErrorCode.GeneralError
        End Try

    End Function
    '--- VRP 12.09.2007
    Public Function fctBreadcrumbsUpdate(ByVal intCodeUser As Integer, ByVal mnuTypeItem As MenuType, ByVal mnucodeitem As MenuType, ByVal mnuTab As enumEgswTransactionMode, ByVal intSave As Integer) As enumEgswErrorCode
        Dim sqlCn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            Dim arrParam(4) As SqlParameter
            arrParam(0) = New SqlParameter("@intCodeUser", intCodeUser)
            arrParam(1) = New SqlParameter("@intTypeItem", mnuTypeItem)
            arrParam(2) = New SqlParameter("@intCodeItem", mnucodeitem)
            arrParam(3) = New SqlParameter("@intTab", mnuTab)
            arrParam(4) = New SqlParameter("@intSave", intSave)

            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswBreadcrumbsUpdate", arrParam)
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function '-------

    '--- VRP 20.11.2007
    Public Function SaveDeleteMarksRecipe(ByVal strSessionID As String, ByVal lngCodeRecipe As Long, ByVal flagSave As Boolean, ByRef flagExist As Boolean, ByRef lngTotalRecipes As Long, Optional ByVal bClearFirst As Boolean = False) As Boolean
        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim cmd As SqlCommand = New SqlCommand
        Try
            '
            With cmd
                cn.Open()
                .Connection = cn
                .CommandText = "sp_EgsWLabelSaveDelSelected"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@p_sSessionID", SqlDbType.VarChar, 100).Value = strSessionID
                .Parameters.Add("@p_nCodeRecipe", SqlDbType.Int).Value = lngCodeRecipe
                .Parameters.Add("@p_bSave", SqlDbType.Bit).Value = flagSave
                .Parameters.Add("@p_bClearFirst", SqlDbType.Bit).Value = bClearFirst
                .Parameters.Add("@p_bExist", SqlDbType.Bit).Direction = ParameterDirection.Output
                .Parameters.Add("@p_nTotalRecord", SqlDbType.Int).Direction = ParameterDirection.Output
                .ExecuteNonQuery()
                flagExist = CBool(.Parameters("@p_bExist").Value)
                lngTotalRecipes = CInt(.Parameters("@p_nTotalRecord").Value)
            End With
            '
            cn.Close()
            cn.Dispose()
            cmd.Dispose()
            '
            Return True
        Catch ex As Exception
            'Throw ex
            cn.Close()
            cn.Dispose()
            Return False
        End Try
    End Function '--- 

    '--- VRP 25.03.2008
    Public Function fctUpdateProcedureTemplateM(ByVal intCode As Integer, ByVal strName As String, _
               ByVal intGLobal As Integer) As Integer

        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim cmd As SqlCommand = New SqlCommand
        Try
            With cmd
                cn.Open()
                .Connection = cn
                .CommandText = "sp_EgswUpdateProcedureTemplateM"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@strName", SqlDbType.NVarChar, 250).Value = ReplaceSpecialCharacters(strName)
                .Parameters.Add("@bitGlobal", SqlDbType.Bit).Value = intGLobal
                .Parameters.Add("@intCode", SqlDbType.Int).Direction = ParameterDirection.InputOutput
                .Parameters("@intCode").Value = intCode
                .ExecuteNonQuery()
                If intCode = 0 Then
                    intCode = CInt(.Parameters("@intCode").Value)
                End If
            End With
            cn.Close()
            cn.Dispose()
            cmd.Dispose()
            Return intCode
        Catch ex As Exception
            cn.Close()
            cmd.Dispose()
        End Try
    End Function

    Public Function fctUpdateProcedureTemplateD(ByVal intCode As Integer, ByVal intCodeMain As Integer, _
               ByVal strName As String, ByVal intPosition As Integer, _
               Optional ByVal bitRemoved As Byte = 0, Optional ByVal intCodeStyle As Integer = -1) As Integer

        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim cmd As SqlCommand = New SqlCommand
        Try
            With cmd
                cn.Open()
                .Connection = cn
                .CommandText = "sp_EgswUpdateProcedureTemplateD"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeMain", SqlDbType.Int).Value = intCodeMain
                .Parameters.Add("@strName", SqlDbType.NVarChar, 250).Value = ReplaceSpecialCharacters(strName)
                .Parameters.Add("@intPosition", SqlDbType.Int).Value = intPosition
                .Parameters.Add("@bitRemoved", SqlDbType.Bit).Value = bitRemoved
                .Parameters.Add("@intCode", SqlDbType.Int).Direction = ParameterDirection.InputOutput
                .Parameters("@intCode").Value = intCode
                .Parameters.Add("@intCodeStyle", SqlDbType.Int).Value = intCodeStyle
                .ExecuteNonQuery()

                If intCode = -1 Then
                    intCode = CInt(.Parameters("@intCode").Value)
                End If
            End With
            cn.Close()
            cn.Dispose()
            cmd.Dispose()
            Return intCode
        Catch ex As Exception
            cn.Close()
            cmd.Dispose()
        End Try

    End Function

    Public Function fctUpdateSharing(ByVal intCode As Integer, ByVal intCodeSite As Integer, _
             ByVal strCodeSharedTo As String, ByVal intCodeEgswTable As enumDbaseTables) As enumEgswErrorCode



        Dim cn As SqlConnection
        Dim cmd As SqlCommand = New SqlCommand

        Try
            With cmd
                cn = New SqlConnection(L_strCnn)
                cn.Open()
                .Connection = cn
                .CommandText = "DELETE FROM EgswSharing WHERE Code=" & intCode & " AND CodeUserOwner=" & intCodeSite & _
                      " AND CodeEgswTable=" & intCodeEgswTable & " AND Type=1"
                .CommandType = CommandType.Text
                .ExecuteNonQuery()
            End With
            cn.Close()
            cn.Dispose()
            cmd.Dispose()

            With cmd
                cn = New SqlConnection(L_strCnn)
                .Connection = cn
                .CommandText = "sp_EgswUpdateSharing"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int)
                .Parameters.Add("@intCodeSite", SqlDbType.Int)
                .Parameters.Add("@intCodeSitesShared", SqlDbType.Int)
                .Parameters.Add("@intCodeEgswTable", SqlDbType.Int)
                cn.Open()


                Dim arrCodeSites() As String
                If Not strCodeSharedTo = "-1" Then
                    strCodeSharedTo = strCodeSharedTo.Replace("(", "")
                    strCodeSharedTo = strCodeSharedTo.Replace(")", "")
                    arrCodeSites = strCodeSharedTo.Split(CChar(","))

                    For i As Integer = 0 To UBound(arrCodeSites)
                        If IsNumeric(arrCodeSites(i)) Then
                            .Parameters("@intCode").Value = intCode
                            .Parameters("@intCodeSite").Value = intCodeSite
                            .Parameters("@intCodeSitesShared").Value = arrCodeSites(i)
                            .Parameters("@intCodeEgswTable").Value = intCodeEgswTable
                            .ExecuteNonQuery()
                        End If
                    Next
                Else
                    .Parameters("@intCode").Value = intCode
                    .Parameters("@intCodeSite").Value = intCodeSite
                    .Parameters("@intCodeSitesShared").Value = CInt(strCodeSharedTo)
                    .Parameters("@intCodeEgswTable").Value = intCodeEgswTable
                    .ExecuteNonQuery()
                End If
            End With
            cn.Close()
            cn.Dispose()
            cmd.Dispose()
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            cn.Close()
            cmd.Dispose()
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Sub subUpdateMediaFilesList(ByVal intID As Integer, ByVal intCodeliste As Integer, ByVal strFileName As String, _
               Optional ByVal strFilecaption As String = "0", Optional ByVal intUrlFlag As Integer = 0, Optional ByVal flagDel As Boolean = False) 'VRP 07.05.2008

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As SqlCommand = New SqlCommand

        Try
            With cmd
                cn.Open()
                .Connection = cn
                .CommandText = "sp_EgswListeFileUpDel"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intID", SqlDbType.Int, 4).Value = intID
                .Parameters.Add("@CodeListe", SqlDbType.Int, 4).Value = intCodeliste
                .Parameters.Add("@FileName", SqlDbType.NVarChar, 250).Value = ReplaceSpecialCharacters(strFileName)
                .Parameters.Add("@Filecaption", SqlDbType.NVarChar, 100).Value = ReplaceSpecialCharacters(strFilecaption)
                .Parameters.Add("@CodeEgswTable", SqlDbType.Int, 4).Value = 50
                .Parameters.Add("@FlagUrl", SqlDbType.Int, 4).Value = intUrlFlag
                .Parameters.Add("@flagDelete", SqlDbType.Bit).Value = flagDel 'DLS
                .ExecuteNonQuery()
            End With
            '
            cn.Close()
            cn.Dispose()
            cmd.Dispose()
        Catch ex As Exception
            cn.Close()
            cmd.Dispose()
        End Try
    End Sub

    Public Sub subUpdateApprovalStatus(ByVal intPosition As Integer, ByVal intCodeliste As Integer, Optional ByVal bitApprove As Boolean = False, _
               Optional ByVal intCodeUser As Integer = -1) 'VRP 07.05.2008

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As SqlCommand = New SqlCommand

        Try
            With cmd
                cn.Open()
                .Connection = cn
                .CommandText = "UPDATE_ApprovalStatus"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intPosition", SqlDbType.Int, 4).Value = intPosition
                .Parameters.Add("@intCode", SqlDbType.Int, 4).Value = intCodeliste
                .Parameters.Add("@bitApprove", SqlDbType.Bit).Value = bitApprove
                .Parameters.Add("@intCodeUser", SqlDbType.Int, 4).Value = intCodeUser
                .ExecuteNonQuery()
            End With
            '
            cn.Close()
            cn.Dispose()
            cmd.Dispose()
        Catch ex As Exception
            cn.Close()
            cmd.Dispose()
        End Try
    End Sub

    Public Function fctUpdateProcedureStyles(ByVal intCode As Integer, ByVal strStyleName As String, ByVal blnIsGlobal As Boolean, _
               ByVal t_Style As structProcetureStyle) As Integer  'VRP 01.07.2008

        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim cmd As SqlCommand = New SqlCommand
        Try
            With cmd
                cn.Open()
                .Connection = cn
                .CommandText = "sp_EgswUpdateProcedureStyles"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 500).Value = ReplaceSpecialCharacters(strStyleName)
                .Parameters.Add("@bitIsGlobal", SqlDbType.Bit).Value = blnIsGlobal
                .Parameters.Add("@nvcFontNameH", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(t_Style.strFontNameH)
                .Parameters.Add("@fltFontSizeH", SqlDbType.Float).Value = t_Style.dblFontSizeH
                .Parameters.Add("@nvcFontColorH", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(t_Style.strFontColorH)
                .Parameters.Add("@nvcBGColorH", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(t_Style.strBGColorH)
                .Parameters.Add("@bitIsBoldH", SqlDbType.Bit).Value = t_Style.blnIsBoldH
                .Parameters.Add("@bitIsItalicH", SqlDbType.Bit).Value = t_Style.blnIsItalicH
                .Parameters.Add("@bitIsUnderlineH", SqlDbType.Bit).Value = t_Style.blnIsUnderlineH
                .Parameters.Add("@fltFontSBH", SqlDbType.Float).Value = t_Style.dblFontSBH
                .Parameters.Add("@fltFontSAH", SqlDbType.Float).Value = t_Style.dblFontSAH

                .Parameters.Add("@nvcFontNameD", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(t_Style.strFontNameD)
                .Parameters.Add("@fltFontSizeD", SqlDbType.Float).Value = t_Style.dblFontSizeD
                .Parameters.Add("@nvcFontColorD", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(t_Style.strFontColorD)
                .Parameters.Add("@nvcBGColorD", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(t_Style.strBGColorD)
                .Parameters.Add("@bitIsBoldD", SqlDbType.Bit).Value = t_Style.blnIsBoldD
                .Parameters.Add("@bitIsItalicD", SqlDbType.Bit).Value = t_Style.blnIsItalicD
                .Parameters.Add("@bitIsUnderlineD", SqlDbType.Bit).Value = t_Style.blnIsUnderlineD
                .Parameters.Add("@fltFontSBD", SqlDbType.Float).Value = t_Style.dblFontSBD
                .Parameters.Add("@fltFontSAD", SqlDbType.Float).Value = t_Style.dblFontSAD


                .Parameters.Add("@intCode", SqlDbType.Int).Direction = ParameterDirection.InputOutput
                .Parameters("@intCode").Value = intCode

                .ExecuteNonQuery()

                If intCode = -1 Then
                    intCode = CInt(.Parameters("@intCode").Value)
                End If
            End With

            cn.Close()
            cn.Dispose()
            cmd.Dispose()
            Return intCode
        Catch ex As Exception
            cn.Close()
            cmd.Dispose()
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function
    Public Function ResetApprovalStatus(ByVal intCode As Integer) As enumEgswErrorCode
        Dim cmd As SqlCommand = New SqlCommand
        Try
            cmd.Connection = New SqlConnection(L_strCnn)
            cmd.Connection.Open()
            cmd.CommandText = "Update EgsWListe SET ApprovalStatus = NULL, [use]=0 WHERE Code=@Code"
            cmd.Parameters.Add("@Code", SqlDbType.Int)
            cmd.Parameters("@Code").Value = intCode
            cmd.ExecuteNonQuery()

        Catch ex As Exception
            Throw ex
        Finally
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
        End Try
    End Function

    Public Function GetListeApprovalDetails(ByVal intCodeListe As Integer) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim value As DataTable

        With cmd
            .Connection = cn
            .CommandText = " SELECT" _
                + " CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN t.name ELSE l.name END Name" _
                + " ,ISNULL(config2.string,1) as CodeTrans" _
                + " ,u.UserName" _
                + " ,u.FullName" _
                + " ,u.Email" _
                + " FROM EgswListe l" _
                + " LEFT OUTER JOIN EgswListeTranslation t on t.CodeListe=l.Code AND t.CodeTrans=l.CodeTrans" _
                + " LEFT OUTER JOIN EgswUser u ON u.Code=l.CodeUser" _
                + " LEFT OUTER JOIN EgswConfig config2 ON u.code=config2.CodeUser" _
                + " AND config2.numero=20017 AND config2.codeGroup=-1" _
                + " WHERE l.Code=" & intCodeListe

            .CommandType = CommandType.Text

            Try
                Dim dt As DataTable
                Dim ds As New DataSet
                cn.Open()
                Dim da As New SqlDataAdapter(.CommandText, .Connection)
                da.Fill(ds)
                If Not ds Is Nothing Then
                    value = ds.Tables(0)
                End If
            Catch ex As Exception
                Throw ex
            Finally
                cn.Close()
                cn.Dispose()
            End Try

        End With

        Return value
    End Function

    '-- VRP 04.02.2009
    Public Function UpdateListeMassGlobal(ByVal intCodeListe As Integer, ByVal blnIsGlobal As Boolean, ByVal intCodeUser As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Dim cn As New SqlConnection(L_strCnn)

        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_EgswListeUpdateGlobal"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@IsGlobal", SqlDbType.Bit).Value = blnIsGlobal
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                .Connection.Dispose()
            End With

            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function fctClearComments(ByVal intCodeListe As Integer) As enumEgswErrorCode

        Dim cn As SqlConnection
        Dim cmd As SqlCommand = New SqlCommand

        Try
            With cmd
                cn = New SqlConnection(L_strCnn)
                cn.Open()
                .Connection = cn
                .CommandText = "[sp_EgswClearComments]"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 90000
                .Parameters.Add("@intCodeliste", SqlDbType.Int).Value = intCodeListe
                .ExecuteNonQuery()
            End With
            cn.Close()
            cn.Dispose()
            cmd.Dispose()

            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function
    Public Function fctCreateInsertComment(ByVal intCodeListe As Integer, ByVal intSequence As Integer, _
             ByVal strDescription As String, ByVal submitDate As Date, _
             ByVal dateLastModified As Date, ByVal intCodeUser As Integer) As enumEgswErrorCode

        Dim cn As SqlConnection
        Dim cmd As SqlCommand = New SqlCommand

        Try
            With cmd
                cn = New SqlConnection(L_strCnn)
                cn.Open()
                .Connection = cn
                .CommandText = "[sp_EgswInsertComment]"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 90000
                .Parameters.Add("@intCodeliste", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@intSequence", SqlDbType.Int).Value = intSequence
                .Parameters.Add("@strDescription", SqlDbType.NVarChar).Value = ReplaceSpecialCharacters(strDescription)
                .Parameters.Add("@dateSubmitDate", SqlDbType.DateTime).Value = submitDate
                .Parameters.Add("@dateLastModified", SqlDbType.DateTime).Value = dateLastModified
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser

                .ExecuteNonQuery()
            End With
            cn.Close()
            cn.Dispose()
            cmd.Dispose()

            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    '--- VRP 26.03.2009
    Public Function fctUpdateListePrices(ByVal intCodeSite As Integer, ByVal intCopyCodeSite As Integer, _
              ByVal intIDMain As Integer, ByVal blnCompareByName As Boolean, _
              ByVal dblPricePercent As Double, ByVal intCodeSetPrice As Integer, _
              ByVal intCodeUser As Integer) As enumEgswErrorCode

        Dim cn As SqlConnection
        Dim cmd As SqlCommand = New SqlCommand

        Try
            With cmd
                cn = New SqlConnection(L_strCnn)
                cn.Open()
                .Connection = cn
                .CommandText = "[TOOLS_COPYPRICES]"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 90000
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@CopyCodeSite", SqlDbType.Int).Value = intCopyCodeSite
                .Parameters.Add("@IDMain", SqlDbType.Int).Value = intIDMain
                .Parameters.Add("@CompareByName", SqlDbType.Bit).Value = blnCompareByName
                .Parameters.Add("@PricePercetage", SqlDbType.Float).Value = dblPricePercent
                .Parameters.Add("@CodeSetPrice", SqlDbType.Bit).Value = intCodeSetPrice
                .Parameters.Add("@CodeUser", SqlDbType.Bit).Value = intCodeUser
                .ExecuteNonQuery()
            End With
            cn.Close()
            cn.Dispose()
            cmd.Dispose()

            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    'DLS
    Public Function fctTransformStrSearchFullText(ByVal strText As String) As String
        strText = strText.Replace("""", "")
        strText = strText.Replace("|", "")
        strText = strText.Replace("!", "")
        strText = strText.Trim

        If strText.Contains(" ") Then
            strText = Replace(strText, " ", " AND ")
        Else
            strText = "FORMSOF(INFLECTIONAL,'" & Replace(strText, "'", "''") & "')"
        End If
        Return strText
    End Function

    'JBB 01.04.2011
    Public Sub UpdateRecipeStatus(ByVal intCodeListe As Integer, ByVal intRecipeState As Integer, ByVal intWebState As Integer, _
             ByVal intUpdatedBy As Integer, ByVal intCreatedBy As Integer, ByVal intlastModifiedBy As Integer, _
             ByVal intTestedBy As Integer, ByVal intDevelopedBy As Integer, ByVal intFinalEditBy As Integer, _
             ByVal dtDateCreated As DateTime, ByVal dtDateLastModified As DateTime, ByVal dtDateTested As DateTime, _
             ByVal dtDateDeveloped As DateTime, ByVal dtDateFinalEdit As DateTime, ByVal strDevelopmentPurpose As String, _
             ByVal bDateVisibility As Boolean, ByVal dtDateVisibilityFrom As DateTime, ByVal dtDateVisibilityTo As DateTime, ByVal intWebAuthor As Integer)

        Dim cn As SqlConnection
        Dim cmd As SqlCommand = New SqlCommand
        Try
            With cmd
                cn = New SqlConnection(L_strCnn)
                cn.Open()
                .Connection = cn
                .CommandText = "[UPDATE_RECIPESTATUS]"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 90000
                .Parameters.Add("@Code", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@RecipeState", SqlDbType.Int).Value = intRecipeState
                .Parameters.Add("@WebState", SqlDbType.Int).Value = intWebState
                If intCreatedBy <> -1 Then
                    .Parameters.Add("@CreatedBy", SqlDbType.Int).Value = intCreatedBy
                End If
                .Parameters.Add("@UpdatedBy", SqlDbType.Int).Value = intUpdatedBy

                .Parameters.Add("@LastModifiedBy", SqlDbType.Int).Value = intlastModifiedBy
                .Parameters.Add("@TestedBy", SqlDbType.Int).Value = intTestedBy
                .Parameters.Add("@DevelopedBy", SqlDbType.Int).Value = intDevelopedBy
                .Parameters.Add("@FinalEditBy", SqlDbType.Int).Value = intFinalEditBy
                If intCreatedBy <> -1 Then
                    .Parameters.Add("@DateCreated", SqlDbType.SmallDateTime).Value = dtDateCreated
                End If
                .Parameters.Add("@DateLastModified", SqlDbType.SmallDateTime).Value = dtDateLastModified
                .Parameters.Add("@DateTested", SqlDbType.SmallDateTime).Value = dtDateTested
                .Parameters.Add("@DateDeveloped", SqlDbType.SmallDateTime).Value = dtDateDeveloped
                .Parameters.Add("@DateFinalEdit", SqlDbType.SmallDateTime).Value = dtDateFinalEdit
                .Parameters.Add("@DevelopmentPurpose", SqlDbType.VarChar, 255).Value = strDevelopmentPurpose

                '-- JBB 05.09.2011
                .Parameters.Add("@ActivateVisibilityDate", SqlDbType.Bit).Value = bDateVisibility
                .Parameters.Add("@VisibilityDateFrom", SqlDbType.SmallDateTime).Value = dtDateVisibilityFrom
                .Parameters.Add("@VisibilityDateTo", SqlDbType.SmallDateTime).Value = dtDateVisibilityTo

                '--

                '-- JBB 01.17.2012
                .Parameters.Add("@WebAuthor", SqlDbType.Int).Value = intWebAuthor



                .ExecuteNonQuery()
            End With
            cn.Close()
            cn.Dispose()
            cmd.Dispose()
        Catch ex As Exception

        End Try

    End Sub

    'JBB 01.21.2011
    Public Sub UpdateRecipeStatusUpdated(ByVal strCodeListe As String, ByVal intUpdatedBy As Integer, ByVal intlastModifiedBy As Integer, _
             ByVal dtDateLastModified As DateTime)
        Dim cn As SqlConnection
        Dim cmd As SqlCommand = New SqlCommand
        Try
            With cmd
                cn = New SqlConnection(L_strCnn)
                cn.Open()
                .Connection = cn
                .CommandText = "[UPDATE_RecipeStatusUpdated]"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 90000
                .Parameters.Add("@Code", SqlDbType.VarChar, 8000).Value = strCodeListe
                .Parameters.Add("@UpdatedBy", SqlDbType.Int).Value = intUpdatedBy
                .Parameters.Add("@LastModifiedBy", SqlDbType.Int).Value = intlastModifiedBy
                .Parameters.Add("@DateLastModified", SqlDbType.SmallDateTime).Value = dtDateLastModified
                .ExecuteNonQuery()
            End With
            cn.Close()
            cn.Dispose()
            cmd.Dispose()
        Catch ex As Exception

        End Try

    End Sub

    '-- JBB 04.02.2012
    Public Sub UpdateRecipeandWebStatus(intWebState As Integer, intRecipeState As Integer, strCodeList As String, intCodeUser As Integer)
        Dim cn As SqlConnection
        Dim cmd As SqlCommand = New SqlCommand
        Try
            With cmd
                cn = New SqlConnection(L_strCnn)
                cn.Open()
                .Connection = cn
                .CommandText = "[UPDATE_RecipeansWebStatus]"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 90000
                .Parameters.Add("@WebState", SqlDbType.Int, 4).Value = intWebState
                .Parameters.Add("@RecipeState", SqlDbType.Int, 4).Value = intRecipeState
                .Parameters.Add("@CodeList", SqlDbType.NVarChar, 1000).Value = strCodeList
                .Parameters.Add("@CodeUser", SqlDbType.NVarChar, 4).Value = intCodeUser

                .ExecuteNonQuery()
            End With
            cn.Close()
            cn.Dispose()
            cmd.Dispose()
        Catch ex As Exception

        End Try

    End Sub

    '---

    '// DRR 1.4.2011 checkIN-checkOut Status 
    Public Function fctUpdateCheckOutStatus(ByVal intCodeListe As Integer, ByVal intCodeUser As Integer) As Boolean

        fctUpdateCheckOutStatus = False

        Dim cn As SqlConnection
        Dim cmd As SqlCommand = New SqlCommand

        Try
            With cmd
                cn = New SqlConnection(L_strCnn)
                cn.Open()
                .Connection = cn
                .CommandText = "UPDATE EgswListe SET CheckOutUser=@CodeUser WHERE Code=@Code"
                .CommandType = CommandType.Text
                .CommandTimeout = 90000
                .Parameters.Add("@Code", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser
                .ExecuteNonQuery()
            End With
            cn.Close()
            cn.Dispose()
            cmd.Dispose()

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function IsListeCheckOut(ByVal intCodeListe As Integer) As Integer
        Dim cmd As New SqlCommand
        Dim nReturnValue As Integer

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                '.CommandText = "SELECT (case when ISNULL(checkoutuser,0) > 0 then 1 else 0 end) FROM EgswListe WHERE Code=" & intCodeListe
                .CommandText = "SELECT CheckOutUser=ISNULL(checkoutuser,0) FROM EgswListe WHERE Code=" & intCodeListe
                .CommandType = CommandType.Text

                .Connection.Open()
                nReturnValue = CInt(.ExecuteScalar())
                .Connection.Close()
            End With

            Return nReturnValue

        Catch ex As Exception
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Public Function ListeCodesite(ByVal intCodeListe As Integer) As Integer
        Dim cmd As New SqlCommand
        Dim nReturnValue As Integer

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "SELECT CodeSite=ISNULL(CodeSite,0) FROM EgswListe WHERE Code=" & intCodeListe
                .CommandType = CommandType.Text
                .Connection.Open()
                nReturnValue = CInt(.ExecuteScalar())
                .Connection.Close()
            End With

            Return nReturnValue

        Catch ex As Exception
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
    End Function


    Public Function IsListeExists(ByVal intCodeListe As Integer) As Boolean
        Dim cmd As New SqlCommand
        Dim nReturnValue As Integer

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "SELECT COUNT(Code) FROM EgswListe WHERE Code = " & intCodeListe.ToString()
                .CommandType = CommandType.Text

                .Connection.Open()
                nReturnValue = CInt(.ExecuteScalar())
                .Connection.Close()
            End With

            If nReturnValue >= 1 Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    'Added by ADR 03.25.11
    Public Function isSearchResultExists(ByVal searchItem As String, ByVal mainLanguageFilter As Integer) As Boolean
        Dim cmd As New SqlCommand
        Dim nReturnValue As Integer

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "SELECT COUNT(name) FROM EgswListe WHERE Name LIKE  '%" & searchItem & "%' AND CodeTrans = " & mainLanguageFilter
                .CommandType = CommandType.Text

                .Connection.Open()
                nReturnValue = CInt(.ExecuteScalar())
                .Connection.Close()
            End With

            Return nReturnValue

        Catch ex As Exception
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Public Function fctUpdateProtectedStatus(ByVal intCodeListe As Integer, ByVal blProtected As Boolean) As Boolean

        fctUpdateProtectedStatus = False

        Dim cn As SqlConnection
        Dim cmd As SqlCommand = New SqlCommand

        Try
            With cmd
                cn = New SqlConnection(L_strCnn)
                cn.Open()
                .Connection = cn
                .CommandText = "UPDATE EgswListe SET Protected=@Protected WHERE Code=@Code"
                .CommandType = CommandType.Text
                .CommandTimeout = 90000
                .Parameters.Add("@Code", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@Protected", SqlDbType.Bit).Value = blProtected
                .ExecuteNonQuery()
            End With
            cn.Close()
            cn.Dispose()
            cmd.Dispose()

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function fctApplyWastageValues( _
     ByVal intCodeListe As Integer, _
     ByVal intWastage1 As Integer, _
     ByVal intWastage2 As Integer, _
     ByVal intWastage3 As Integer, _
     ByVal intWastage4 As Integer, _
     ByVal intWastage5 As Integer) As Boolean

        fctApplyWastageValues = False

        Dim cn As SqlConnection
        Dim cmd As SqlCommand = New SqlCommand

        Try
            With cmd
                cn = New SqlConnection(L_strCnn)
                cn.Open()
                .Connection = cn
                .CommandText = "SP_EgswListeUpdateWastage"
                .CommandType = CommandType.StoredProcedure
                .CommandTimeout = 90000
                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@intWastage1", SqlDbType.SmallInt).Value = intWastage1
                .Parameters.Add("@intWastage2", SqlDbType.SmallInt).Value = intWastage2
                .Parameters.Add("@intWastage3", SqlDbType.SmallInt).Value = intWastage3
                .Parameters.Add("@intWastage4", SqlDbType.SmallInt).Value = intWastage4
                .Parameters.Add("@intWastage5", SqlDbType.SmallInt).Value = intWastage5
                .ExecuteNonQuery()
            End With

            fctApplyWastageValues = True
        Catch ex As Exception
            fctApplyWastageValues = False
        Finally
            cn.Close()
            cn.Dispose()
            cmd.Dispose()
        End Try
    End Function

#End Region

#Region " Boolean Functions "

    Public Function IsPriceUsed(ByVal intID As Integer) As Boolean
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_egswIsPriceUsed"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intID", SqlDbType.Int).Value = intID
                .Parameters.Add("@IsUsed", SqlDbType.Int).Direction = ParameterDirection.Output

                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                IsPriceUsed = CBool(.Parameters("@IsUsed").Value)
            End With

        Catch ex As Exception
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Public Function IsListeSubmitted(ByVal intCodeListe As Integer) As Boolean
        Dim cmd As New SqlCommand
        Dim nReturnValue As Integer

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "SELECT ISNULL(Submitted,0) FROM egswListe WHERE code=" & intCodeListe
                .CommandType = CommandType.Text

                nReturnValue = CInt(.ExecuteScalar())
            End With

            If nReturnValue = 1 Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Public Function IsListeUsed(ByVal intCodeListe As Integer) As Boolean
        Dim cmd As New SqlCommand
        Dim nReturnValue As Integer

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                '.CommandText = "SELECT ISNULL(firstcode,0) FROM egswDetails where secondcode=" & intCodeListe
                .CommandText = "SELECT ISNULL(d.firstcode,0) FROM egswDetails d INNER JOIN egswListe l on d.FirstCode = l.Code and l.[Type] = 8 where d.secondcode=" & intCodeListe

                .CommandType = CommandType.Text

                .Connection.Open()
                nReturnValue = CInt(.ExecuteScalar())
                .Connection.Close()
            End With

            If nReturnValue = 0 Then
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Public Function ListeValidateBasicItems(Optional ByVal strBrandCodeList As String = "", Optional ByVal strCategoryCodeList As String = "", _
        Optional ByVal strSourceCodeList As String = "", Optional ByVal strSupplierCodeList As String = "", _
        Optional ByVal strUnitCodeList As String = "", Optional ByVal strListeCodeList As String = "", Optional ByVal strKeywordCodeList As String = "") As enumEgswErrorCode
        Dim arrParam(7) As SqlParameter

        arrParam(0) = New SqlParameter("@vchBrandCodeList", strBrandCodeList)
        arrParam(1) = New SqlParameter("@vchCategoryCodeList", strCategoryCodeList)
        arrParam(2) = New SqlParameter("@vchSourceCodeList", strSourceCodeList)
        arrParam(3) = New SqlParameter("@vchSupplierCodeList", strSupplierCodeList)
        arrParam(4) = New SqlParameter("@vchUnitcodeList", strUnitCodeList)
        arrParam(5) = New SqlParameter("@intError", SqlDbType.Int)
        arrParam(6) = New SqlParameter("@vchListeCodeList", strListeCodeList)
        arrParam(7) = New SqlParameter("@vchKeywordCodeList", strKeywordCodeList)

        arrParam(5).Direction = ParameterDirection.Output

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswListeValidateBasicItems", arrParam)
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
        Return CType(arrParam(5).Value, enumEgswErrorCode)
    End Function

#End Region

#Region "Private"

    Private Function AddParam(ByRef cmd As SqlCommand, ByVal type As SqlDbType, ByVal strSqlcompare As String, ByVal strParamName As String, ByVal strValue As String, ByVal chrDelimiter As Char, ByVal IsLike As Boolean) As String
        Dim intCounter As Integer = 0
        Dim arrValues() As String = strValue.Split(chrDelimiter)
        Dim strVal As String
        Dim sParamName As String
        Dim strSQL As String = ""

        If arrValues.Length = 1 Then
            If IsLike Then
                arrValues(0) = "%" & arrValues(0).Trim & "%"
            End If
            cmd.Parameters.Add(strParamName, SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(arrValues(0))
            Return strParamName
        End If

        If IsLike Then
            arrValues(0) = "%" & arrValues(0).Trim & "%"
        End If

        strSQL = strParamName
        cmd.Parameters.Add(strParamName, SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(arrValues(0))

        For Each strVal In arrValues
            If intCounter > 0 Then     ' start from second array
                sParamName = strParamName & intCounter
                strSQL &= strSqlcompare & sParamName

                If IsLike Then
                    strVal = "%" & strVal.Trim & "%"
                End If
                Select Case type
                    Case SqlDbType.NVarChar
                        cmd.Parameters.Add(sParamName, type, 260).Value = ReplaceSpecialCharacters(strVal)
                    Case SqlDbType.Int
                        cmd.Parameters.Add(sParamName, type).Value = ReplaceSpecialCharacters(strVal)
                End Select
            End If
            intCounter += 1
        Next
        Return strSQL
    End Function

    Private Function ReturnDBNullIfNothing(ByVal objTemp As Object) As Object
        If objTemp Is Nothing Then
            Return DBNull.Value
        Else
            Return objTemp
        End If
    End Function
    Private Function ExecuteFetchType(ByVal eFetchType As enumEgswFetchType, ByVal strCnn As String, ByVal cmdType As CommandType, ByVal strCommandText As String, ByVal arrParam() As SqlParameter) As Object
        Try
            Select Case eFetchType
                Case enumEgswFetchType.DataReader
                    Return ExecuteReader(strCnn, cmdType, strCommandText, arrParam)
                Case enumEgswFetchType.DataSet
                    Return ExecuteDataset(strCnn, cmdType, strCommandText, arrParam)
                Case enumEgswFetchType.DataTable
                    Return ExecuteDataset(strCnn, cmdType, strCommandText, arrParam).Tables(0)
            End Select
            Return Nothing
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Private Function ExecuteFetchType(ByVal eFetchType As enumEgswFetchType, ByRef sqlCmd As SqlCommand) As Object
        Try
            Dim da As New SqlDataAdapter
            If eFetchType = enumEgswFetchType.DataReader Then
                sqlCmd.Connection.Open()
                Return sqlCmd.ExecuteReader(CommandBehavior.CloseConnection)

            ElseIf eFetchType = enumEgswFetchType.DataTable Then
                Dim dt As New DataTable
                With da
                    .SelectCommand = sqlCmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                Return dt

            ElseIf eFetchType = enumEgswFetchType.DataSet Then
                Dim ds As New DataSet
                With da
                    .SelectCommand = sqlCmd
                    .Fill(ds, "ItemList")
                End With
                Return ds
            End If
            Return Nothing
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    'ADR 05.18.11 - Revised
    Private Function ReplaceSpecialCharacters(ByVal strStringToReplace As String) As String
        If Not strStringToReplace Is Nothing Then

            Dim strSpecialChars() As String = New String() {"[[]TM]", "[[]tm]", "[[]R]", "[[]r]", "[[]C]", "[[]c]", "[TM]", "[tm]", "[R]", "[r]", "[C]", "[c]"}
            Dim strSpecialCharsReplacement() As String = New String() {"™", "™", "®", "®", "©", "©", "™", "™", "®", "®", "©", "©"}

            For i As Integer = 0 To strSpecialChars.Length - 1
                If strStringToReplace.IndexOf(strSpecialChars(i)) > 0 Then
                    strStringToReplace = strStringToReplace.Replace(strSpecialChars(i), strSpecialCharsReplacement(i))
                End If
            Next

        End If

        Return strStringToReplace
    End Function

#End Region

#Region "Costing"


    Public Class Costing

        Private m_Price As Double
        Private m_PriceUnitCode As Integer
        Private m_PriceUnitFactor As Double
        Private m_WastageTotal As Integer
        Private m_NetQty As Double
        Private m_NetQtyUnitCode As Integer
        Private m_NetQtyUnitFactor As Double
        Private m_GrossQty As Double
        Private m_NetQtyMetric As Double 'DRR 12.29.2010
        Private m_GrossQtyMetric As Double 'DRR 12.29.2010
        Private m_NetQtyImperial As Double 'DRR 12.29.2010
        Private m_GrossQtyImperial As Double 'DRR 12.29.2010
        Private m_UnitMetric As Integer 'DRR 12.29.2010
        Private m_UnitImperial As Integer 'DRR 12.29.2010
        Private m_TotalCost As Double
        Private m_SuggestedPrice As Double

        Public Sub Reset()
            m_Price = 0
            m_PriceUnitCode = 0
            m_PriceUnitFactor = 0
            m_WastageTotal = 0
            m_NetQty = 0
            m_NetQtyUnitCode = 0
            m_NetQtyUnitFactor = 0
            m_GrossQty = 0
            m_SuggestedPrice = 0

            '// DRR 12.30.2010
            m_NetQtyMetric = 0
            m_GrossQtyMetric = 0
            m_NetQtyImperial = 0
            m_GrossQtyImperial = 0
            m_UnitMetric = 0
            m_UnitImperial = 0
            '//
        End Sub
        Public Property NetQtyUnitCode() As Integer
            Set(ByVal Value As Integer)
                m_NetQtyUnitCode = Value
            End Set
            Get
                Return m_NetQtyUnitCode
            End Get
        End Property
        Public Property NetQtyUnitFactor() As Double
            Set(ByVal Value As Double)
                m_NetQtyUnitFactor = Value
            End Set
            Get
                Return m_NetQtyUnitFactor
            End Get
        End Property

        Public Property Price() As Double
            Set(ByVal Value As Double)
                m_Price = Value
            End Set
            Get
                Return m_Price
            End Get
        End Property

        Public Property PriceUnitCode() As Integer
            Set(ByVal Value As Integer)
                m_PriceUnitCode = Value
            End Set
            Get
                Return m_PriceUnitCode
            End Get
        End Property

        Public Property PriceUnitFactor() As Double
            Set(ByVal Value As Double)
                m_PriceUnitFactor = Value
            End Set
            Get
                Return m_PriceUnitFactor
            End Get
        End Property
        Public Property WastageTotal() As Integer
            Set(ByVal Value As Integer)
                m_WastageTotal = Value
            End Set
            Get
                Return m_WastageTotal
            End Get
        End Property
        Public Property NetQty() As Double
            Get
                Return m_NetQty
            End Get
            Set(ByVal Value As Double)
                m_NetQty = Value
            End Set
        End Property
        Public Property GrossQty() As Double
            Get
                Return m_GrossQty
            End Get
            Set(ByVal Value As Double)
                m_GrossQty = Value
            End Set
        End Property

        '// DRR 12.29.2010 added
        Public Property NetQtyMetric As Double
            Get
                Return m_NetQtyMetric
            End Get
            Set(ByVal Value As Double)
                m_NetQtyMetric = Value
            End Set
        End Property

        Public Property GrossQtyMetric As Double
            Get
                Return m_GrossQtyMetric
            End Get
            Set(ByVal Value As Double)
                m_GrossQtyMetric = Value
            End Set
        End Property

        Public Property UnitMetric As Integer
            Get
                Return m_UnitMetric
            End Get
            Set(ByVal Value As Integer)
                m_UnitMetric = Value
            End Set
        End Property

        Public Property NetQtyImperial As Double
            Get
                Return m_NetQtyImperial
            End Get
            Set(ByVal Value As Double)
                m_NetQtyImperial = Value
            End Set
        End Property

        Public Property GrossQtyImperial As Double
            Get
                Return m_GrossQtyImperial
            End Get
            Set(ByVal Value As Double)
                m_GrossQtyImperial = Value
            End Set
        End Property

        Public Property UnitImperial As Integer
            Get
                Return m_UnitImperial
            End Get
            Set(ByVal Value As Integer)
                m_UnitImperial = Value
            End Set
        End Property
        '//

        Public ReadOnly Property SuggestedPrice() As Double
            Get
                Return m_SuggestedPrice
            End Get
        End Property

        Public Function ComputeForItemCost(Optional ByVal blnMetImp As Boolean = False) As Double
            ' Convert Unit Price if item price unit code is different from itm qty unit code
            If NetQtyUnitCode <> PriceUnitCode Then
                Price = ((Price / m_PriceUnitFactor) * m_NetQtyUnitFactor)
            End If

            ' Compute for Gross Quantity (ItemQty  / 1 - (TotalWastage / 100)) 
            GrossQty = NetQty / (1 - (WastageTotal / 100))

            '// DRR 12.30.2010 compute for Gross quantity
            GrossQtyMetric = NetQtyMetric / (1 - (WastageTotal / 100))
            GrossQtyImperial = NetQtyImperial / (1 - (WastageTotal / 100))

            ' Compute for Item Total Cost  ( Gross Quantity * Unit Price)

            If blnMetImp Then
                Return m_GrossQtyImperial * Price
            Else
                Return m_GrossQty * Price
            End If

        End Function

        Public Function ComputeForNetQty() As Double
            Return m_GrossQty * (1 - (m_WastageTotal / 100))
        End Function

        Public Function ComputeForGrossQty() As Double
            Return m_NetQty / (1 - (m_WastageTotal / 100))
        End Function

        Public Function ComputeForNetQtyMetric() As Double
            Return m_GrossQtyMetric * (1 - (m_WastageTotal / 100))
        End Function

        Public Function ComputeForGrossQtyMetric() As Double
            Return m_NetQtyMetric / (1 - (m_WastageTotal / 100))
        End Function

        Public Function ComputeForNetQtyImperial() As Double
            Return m_GrossQtyImperial * (1 - (m_WastageTotal / 100))
        End Function

        Public Function ComputeForGrossQtyImperial() As Double
            Return m_NetQtyImperial / (1 - (m_WastageTotal / 100))
        End Function

        Public Function GetWastageTotal(ByVal wastage1 As Integer, ByVal wastage2 As Integer, ByVal wastage3 As Integer, ByVal wastage4 As Integer) As Integer
            Return CInt((1 - ((1 - wastage1 / 100.0) * _
                       (1 - wastage2 / 100.0) * _
                       (1 - wastage3 / 100.0) * _
                       (1 - wastage4 / 100.0))) * 100.0)

        End Function


    End Class

#End Region

#Region "Kiosk"

    Public Function GetKioskRecipeDetails(ByVal udtUser As structUser, ByVal intCodeTrans As Integer, ByVal intCode As Integer, _
        ByRef dtHeader As DataTable, ByRef dtIngredients As DataTable, ByRef dtNutrients As DataTable, _
    Optional ByVal Manor As Integer = 0, Optional ByVal Supermarket As Integer = 1) As Boolean

        'RDTC 13.10.2006
        'return datatables for different areas of the recipe

        Dim sb As New StringBuilder
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dtHeaderTmp As New DataTable("RecipeHeader")
        Dim dtIngredientsTmp As New DataTable("RecipeIngredients")
        Dim dtNutrientsTmp As New DataTable("RecipeNutrients")
        Dim dtKeywordsTmp As New DataTable("RecipeKeywords")
        Dim dtKeywords As New DataTable
        Dim ds As DataSet = New DataSet

        '---------------------------
        '-- recipe's main details --
        '---------------------------
        sb.Append("SELECT  replace(r.number, CHAR(1),'') AS Number, r.picturename, " & vbCrLf)
        sb.Append("CASE WHEN rT.name IS NULL OR LEN(rTRIM(LTRIM(rT.name)))=0 THEN r.name ELSE rT.name end name," & vbCrLf)
        sb.Append("CASE WHEN rT.Description IS NULL OR LEN(rTRIM(LTRIM(rT.Description)))=0 THEN r.Description ELSE rT.Description end Description," & vbCrLf)
        sb.Append("CASE WHEN cT.name IS NULL OR LEN(rTRIM(LTRIM(cT.name)))=0 THEN c.name ELSE cT.name end category," & vbCrLf)
        sb.Append("CASE WHEN rT.note IS NULL OR LEN(rTRIM(LTRIM(rT.note)))=0 THEN r.note ELSE rT.note end note," & vbCrLf)
        sb.Append("s.name as source, r.yield," & vbCrLf)
        sb.Append("CASE WHEN yT.name IS NULL OR LEN(rTRIM(LTRIM(yT.name)))=0 THEN y.namedef ELSE yT.name end YieldName" & vbCrLf)
        sb.Append("FROM egswListe as r" & vbCrLf)
        sb.Append("INNER JOIN egswSource s on s.code = r.source" & vbCrLf)
        sb.Append("LEFT OUTER JOIN egswListeTranslation rT on r.code=rT.codeliste AND rT.codeTrans IN " & vbCrLf)
        sb.Append("(" & intCodeTrans & ",NULL)" & vbCrLf)
        sb.Append("INNER JOIN  egswCategory c on c.code=r.category  " & vbCrLf)
        sb.Append("LEFT OUTER JOIN egswItemTranslation cT on c.code=cT.code AND cT.codeTrans IN " & vbCrLf)
        sb.Append("(" & intCodeTrans & "1,NULL) AND cT.CodeEgswTable=dbo.fn_egswGetTableID('egswCategory')" & vbCrLf)
        sb.Append("INNER JOIN  egswUnit y on y.code=r.YieldUnit" & vbCrLf)
        sb.Append("LEFT OUTER JOIN egswItemTranslation yT on y.code=yT.code AND yT.codeTrans IN" & vbCrLf)
        sb.Append("(" & intCodeTrans & ",NULL) AND yT.CodeEgswTable=dbo.fn_egswGetTableID('egswUnit')" & vbCrLf)
        sb.Append("WHERE(r.code = " & intCode & ")" & vbCrLf)

        '-----------------------
        '-- recipe's keywords --
        '-----------------------
        sb.Append("SELECT DISTINCT Name " & vbCrLf)
        sb.Append("FROM egswKeyword " & vbCrLf)
        sb.Append("WHERE Code IN (Select CodeKey FROM egswKeydetails WHERE CodeListe = " & intCode & ")" & vbCrLf)

        '------------------------------
        '-- recipe's nutrient values --
        '------------------------------
        sb.Append("SELECT" & vbCrLf)
        sb.Append("n.n1, n.n2, n.n3, n.n4, n.n5, n.n6, n.n7, n.n8, n.n9, n.n10, n.n11, n.n12, " & vbCrLf)
        sb.Append("(select top 1 name from egswnutrientdef where position=1) as nut1," & vbCrLf)
        sb.Append("(select top 1 name from egswnutrientdef where position=2) as nut2," & vbCrLf)
        sb.Append("(select top 1 name from egswnutrientdef where position=3) as nut3," & vbCrLf)
        sb.Append("(select top 1 name from egswnutrientdef where position=4) as nut4," & vbCrLf)
        sb.Append("(select top 1 name from egswnutrientdef where position=5) as nut5," & vbCrLf)
        sb.Append("(select top 1 name from egswnutrientdef where position=6) as nut6," & vbCrLf)
        sb.Append("(select top 1 name from egswnutrientdef where position=7) as nut7," & vbCrLf)
        sb.Append("(select top 1 name from egswnutrientdef where position=8) as nut8," & vbCrLf)
        sb.Append("(select top 1 name from egswnutrientdef where position=9) as nut9," & vbCrLf)
        sb.Append("(select top 1 name from egswnutrientdef where position=10) as nut10," & vbCrLf)
        sb.Append("(select top 1 name from egswnutrientdef where position=11) as nut11," & vbCrLf)
        sb.Append("(select top 1 name from egswnutrientdef where position=12) as nut12," & vbCrLf)
        sb.Append("(select units from egsw_nutr_def where Nutr_no = " & vbCrLf)
        sb.Append("(select top 1 nutr_no from egswnutrientdef where position=1)) as u1," & vbCrLf)
        sb.Append("(select units from egsw_nutr_def where Nutr_no = " & vbCrLf)
        sb.Append("(select top 1 nutr_no from egswnutrientdef where position=2)) as u2," & vbCrLf)
        sb.Append("(select units from egsw_nutr_def where Nutr_no = " & vbCrLf)
        sb.Append("(select top 1 nutr_no from egswnutrientdef where position=3)) as u3," & vbCrLf)
        sb.Append("(select units from egsw_nutr_def where Nutr_no = " & vbCrLf)
        sb.Append("(select top 1 nutr_no from egswnutrientdef where position=4)) as u4," & vbCrLf)
        sb.Append("(select units from egsw_nutr_def where Nutr_no = " & vbCrLf)
        sb.Append("(select top 1 nutr_no from egswnutrientdef where position=5)) as u5," & vbCrLf)
        sb.Append("(select units from egsw_nutr_def where Nutr_no = " & vbCrLf)
        sb.Append("(select top 1 nutr_no from egswnutrientdef where position=6)) as u6," & vbCrLf)
        sb.Append("(select units from egsw_nutr_def where Nutr_no = " & vbCrLf)
        sb.Append("(select top 1 nutr_no from egswnutrientdef where position=7)) as u7," & vbCrLf)
        sb.Append("(select units from egsw_nutr_def where Nutr_no = " & vbCrLf)
        sb.Append("(select top 1 nutr_no from egswnutrientdef where position=8)) as u8," & vbCrLf)
        sb.Append("(select units from egsw_nutr_def where Nutr_no = " & vbCrLf)
        sb.Append("(select top 1 nutr_no from egswnutrientdef where position=9)) as u9," & vbCrLf)
        sb.Append("(select units from egsw_nutr_def where Nutr_no = " & vbCrLf)
        sb.Append("(select top 1 nutr_no from egswnutrientdef where position=10)) as u10," & vbCrLf)
        sb.Append("(select units from egsw_nutr_def where Nutr_no = " & vbCrLf)
        sb.Append("(select top 1 nutr_no from egswnutrientdef where position=11)) as u11," & vbCrLf)
        sb.Append("(select units from egsw_nutr_def where Nutr_no = " & vbCrLf)
        sb.Append("(select top 1 nutr_no from egswnutrientdef where position=12)) as u12" & vbCrLf)
        sb.Append("FROM egswNutrientval n" & vbCrLf)
        sb.Append("WHERE CodeListe = " & intCode & "" & vbCrLf)

        '------------------------
        '-- recipe ingredients --
        '------------------------
        'sb.Append("SELECT  d.position, i.type," & vbCrLf)
        'sb.Append("CASE WHEN it.name IS NULL OR LEN(rTRIM(LTRIM(it.name)))=0 THEN i.name ELSE it.name end Ingredient," &vbCrLf)
        'sb.Append("(d.quantity * r.yield) as quantity," & vbCrLf)
        'sb.Append("CASE WHEN ut.name IS NULL OR LEN(rTRIM(LTRIM(ut.name)))=0 THEN u.namedisplay ELSE ut.name end Unit" &vbCrLf)
        'sb.Append("FROM egswDetails d " & vbCrLf)
        'sb.Append("INNER JOIN egswListe r on r.code = d.firstcode" &vbCrLf)
        'sb.Append("LEFT OUTER JOIN egswListe i on i.code = d.secondcode" &vbCrLf)
        'sb.Append("LEFT OUTER JOIN egswListeTranslation iT on i.code=iT.codeliste AND iT.codeTrans IN " & vbCrLf)
        'sb.Append("(" & intCodeTrans & ",NULL)" & vbCrLf)
        'sb.Append("LEFT OUTER JOIN egswUnit u on u.code = d.CodeUnit" &vbCrLf)
        'sb.Append("LEFT OUTER JOIN egswItemTranslation uT on u.code=uT.code AND uT.codeTrans IN " & vbCrLf)
        'sb.Append("(" & intCodeTrans & ",NULL) AND uT.CodeEgswTable=dbo.fn_egswGetTableID('egswUnit')" & vbCrLf)
        'sb.Append("where(d.firstcode = " & intCode & ")" & vbCrLf)
        'sb.Append("order by d.position" & vbCrLf)


        Dim dblCurrencyRate As Double = 1.0
        sb.Append("declare @codeprice int " & vbCrLf)


        If Manor = 1 Then
            sb.Append("select top 1 @codeprice = code from egswsetprice where code > 0" & vbCrLf)
            sb.Append("exec sp_EgswListeIngredientsGetComputedManor ")
            sb.Append(intCode & ", " & intCodeTrans & ", ")
            sb.Append(dblCurrencyRate & ", " & 1 & ",")
            sb.Append("@codeprice , " & udtUser.Site.Code)
            sb.Append(", NULL, " & Supermarket)
        Else
            sb.Append("select top 1 @codeprice = code from egswsetprice where code > 0" & vbCrLf)
            sb.Append("exec sp_EgswListeIngredientsGetComputed ")
            sb.Append(intCode & ", " & intCodeTrans & ", ")
            sb.Append(dblCurrencyRate & ", " & 1 & ",")
            sb.Append("@codeprice , " & udtUser.Site.Code)
        End If

        Try
            With cmd
                .Connection = cn
                .CommandText = sb.ToString
                .CommandType = CommandType.Text

                With da
                    .SelectCommand = cmd
                    .Fill(ds)
                End With
            End With

            dtHeaderTmp = ds.Tables(0)
            dtKeywordsTmp = ds.Tables(1)
            dtNutrientsTmp = ds.Tables(2)
            dtIngredientsTmp = ds.Tables(3)

            Dim strKeywords As String = ""
            Dim r As DataRow

            For Each r In dtKeywordsTmp.Rows
                If strKeywords.Length = 0 Then
                    strKeywords = CStr(r.Item("name"))
                Else
                    strKeywords = strKeywords & ", " & CStr(r.Item("name"))
                End If
            Next

            dtHeader = dtHeaderTmp
            With dtHeader
                .Columns.Add("Keyword")
                .Rows(0).Item("Keyword") = strKeywords
            End With

            Dim i As Integer

            With dtNutrients
                .Columns.Add("Name")
                .Columns.Add("Value")
                .Columns.Add("Format")
                .Columns.Add("Unit")

                For i = 1 To 12
                    If Not IsDBNull(dtNutrientsTmp.Rows(0).Item("nut" & i)) Then
                        r = dtNutrients.NewRow
                        r.Item("Name") = dtNutrientsTmp.Rows(0).Item("nut" & i)

                        If IsDBNull(dtNutrientsTmp.Rows(0).Item("n" & i)) Then
                            r.Item("Value") = ""
                        ElseIf CInt(dtNutrientsTmp.Rows(0).Item("n" & i)) = -1 Then
                            r.Item("Value") = ""
                        Else
                            r.Item("Value") = Format(dtNutrientsTmp.Rows(0).Item("n" & i), "#,##0.00")
                        End If

                        If IsDBNull(dtNutrientsTmp.Rows(0).Item("u" & i)) Then
                            r.Item("Unit") = ""
                        Else
                            r.Item("Unit") = (dtNutrientsTmp.Rows(0).Item("u" & i))
                        End If

                        dtNutrients.Rows.Add(r)
                    End If
                Next
            End With

            Dim r2 As DataRow

            With dtIngredients
                .Columns.Add("Qty")
                .Columns.Add("Unit")
                .Columns.Add("Ingredient")

                For Each r In dtIngredientsTmp.Rows
                    r2 = dtIngredients.NewRow

                    If CInt(r.Item("itemType")) = 4 Or CInt(r.Item("itemType")) = 32 Then
                        r2.Item("Qty") = ""
                        r2.Item("Unit") = ""
                        r2.Item("Ingredient") = r.Item("itemName")
                        'r.Item("Ingredient")
                    Else
                        r2.Item("Qty") = Format(r.Item("grossQuantity"), CStr(r.Item("format")))
                        r2.Item("Unit") = r.Item("itemUnit")
                        r2.Item("Ingredient") = r.Item("itemName")
                    End If

                    dtIngredients.Rows.Add(r2)
                Next
            End With


            Return True
        Catch ex As Exception
            MsgBox(Err.Description)
            Return False
        End Try

    End Function

    Public Function GetKioskListeSearchResult(ByVal udtUser As structUser, ByVal slParams As SortedList, _
        ByVal intCodeTrans As Integer, ByVal intPagenumber As Integer, ByVal intPageSize As Integer, _
        ByRef intTotalRows As Integer, Optional ByVal strSort As String = "") As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable("EGSWLISTE")
        Dim sbSQL As New StringBuilder

        Dim strNumber As String = CStr(slParams("NUMBER"))
        Dim strWord As String = fctTransformStrSearch(CStr(slParams("WORD")))
        Dim strKeywords As String = CStr(slParams("KEYWORDS"))
        Dim strCategory As String = CStr(slParams("CATEGORY"))
        Dim intListeType As enumDataListType = CType(slParams("TYPE"), enumDataListType)
        Dim strIngredientsWanted As String = fctTransformStrSearch(CStr(slParams("INGWANTED")))
        Dim strIngredientsUnwanted As String = fctTransformStrSearch(CStr(slParams("INGUNWANTED")))
        Dim strFilter As String = CStr(slParams("FILTER"))
        Dim strSource As String = ""
        If slParams.Contains("SOURCE") Then strSource = CStr(slParams("SOURCE"))

        ' nutrient rules
        Dim strNutrientRules As String = CStr(slParams("NUTRIENTRULES"))
        If strNutrientRules = Nothing Then strNutrientRules = ""

        ' allergens
        Dim strAllergens As String = CStr(slParams("ALLERGENS"))
        If strAllergens = Nothing Then strAllergens = ""

        With sbSQL
            .Append("SET NOCOUNT ON " & vbCrLf)
            .Append("DECLARE @RecCount int " & vbCrLf)
            .Append("SELECT @RecCount = @RecsPerPage * @Page + 1 " & vbCrLf)
            .Append("IF @Page=0 SET @Page=1 " & vbCrLf)
            .Append("CREATE TABLE #TempResults ")
            .Append("( ")
            .Append("ID int IDENTITY, ")
            .Append("code int, ")
            .Append("name nvarchar(260), ")
            .Append("number nvarchar(50), ")
            .Append("dates datetime, ")
            .Append("price float ")
            .Append(") " & vbCrLf)

            .Append("INSERT INTO #TempResults (code, name, number, dates,price) " & vbCrLf)
            .Append("SELECT DISTINCT  r.code, CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end name, r.number, r.dates, 0 " & vbCrLf)
            .Append("FROM egswListe r " & vbCrLf)
            '.Append("INNER JOIN egswSharing ON egswSharing.Code=r.Code ANDegswSharing.CodeEgswTable=dbo.fn_egswGetTableID('egswListe') ")

            ' Join rnListeTranslation table
            .Append("LEFT OUTER JOIN egswListeTranslation l on r.code=l.codeListe and l.codetrans IN (" & intCodeTrans & ",NULL) " & " " & vbCrLf)

            ' Join Category table
            .Append("INNER JOIN  egswCategory c on c.code=r.category  " & vbCrLf)
            .Append("LEFT OUTER JOIN egswItemTranslation cT on c.code=cT.code AND cT.codeTrans IN (" & intCodeTrans & ",NULL) AND cT.CodeEgswTable=dbo.fn_egswGetTableID('egswCategory') AND RTRIM(cT.Name)<>''  " & vbCrLf)

            ' Join Source
            If strSource.Length > 0 Then
                'if for ALL Supplier, dont pass a supplier to this function anymore
                .Append("LEFT OUTER JOIN egswSource source ON source.code=r.Source " & vbCrLf)
            End If

            ' Join Keywords table
            If strKeywords.Length > 0 Then
                .Append("INNER JOIN egswKeyDetails kd on r.code=kd.codeListe " & vbCrLf)
                .Append("INNER JOIN egswKeyword k on kd.codeKey=k.code " & vbCrLf)
                .Append("LEFT OUTER JOIN egswItemTranslation kT on k.code=kT.code AND kT.codeTrans IN (" & intCodeTrans & ",NULL) AND kT.CodeEgswTable=dbo.fn_egswGetTableID('egswKeyword') AND RTRIM(kT.Name)<>'' " & vbCrLf)
            End If

            ' Join Ingredients, rnListe table for ingredients and traslation rnListe
            If strIngredientsWanted.Length > 0 Or strIngredientsUnwanted.Length > 0 Then
                .Append("LEFT OUTER JOIN egswDetails d on r.code=d.firstCode " & vbCrLf)
                .Append("LEFT OUTER JOIN egswListe r1 on r1.code=d.secondcode " & vbCrLf)
                .Append("LEFT OUTER JOIN egswListeTranslation l2 on r1.code=l2.codeListe AND l2.codetrans IN (" & intCodeTrans & ",NULL) " & vbCrLf)
            End If

            'join nutrient rules
            If strNutrientRules.Trim.Length > 0 Then
                .Append("INNER JOIN egswNutrientVal ON r.code=egswNutrientVal.CodeListe " & vbCrLf)
            End If

            'join allergens
            If strAllergens.Length > 0 Then
                .Append("LEFT OUTER JOIN egswListeAllergen a ON a.CodeListe=r.code " & vbCrLf)
            End If

            .Append("WHERE ")
            ' Flags/search criteria

            .Append(" r.Type=" & intListeType & " " & vbCrLf)
            '.Append(" AND r.protected=0 " & vbCrLf) DRR commented 05.05.2011
            .Append(" AND r.online=1 " & vbCrLf)

            If strWord.Length > 0 Then
                strWord = "%" & strWord & "%" ' always use like
                ' find match in rnliste table
                .Append("AND (r.Name like @nvcWord ")
                ' find match in rnlistetranslation table
                .Append("OR (l.name like @nvcWord ")
                .Append("AND l.codetrans=" & intCodeTrans & ")) ")
                cmd.Parameters.Add("@nvcWord", SqlDbType.NVarChar, 260).Value = ReplaceSpecialCharacters(strWord)
            End If

            If strNumber.Length > 0 Then
                strNumber = "%" & strNumber & "%" ' always use like
                .Append("AND r.Number like @nvcNumber ")
                cmd.Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 25).Value = ReplaceSpecialCharacters(strNumber)
            End If

            ' Wanted Ingredient Search
            If strIngredientsWanted.Length > 0 Then
                Dim strSQLEintCodeIng1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR r1.name LIKE ", "@nvcIngWanted", ReplaceSpecialCharacters(strIngredientsWanted), CChar(","), True)
                Dim strSQLEintCodeIng2 As String = AddParam(cmd, SqlDbType.NVarChar, " OR l2.name LIKE ", "@nvcIng2Wanted", ReplaceSpecialCharacters(strIngredientsWanted), CChar(","), True)

                ' Find match in ingredients
                .Append("AND (r1.Name like " & strSQLEintCodeIng1 & " " & vbCrLf)

                ' find match ingredient in rnliste translation table
                .Append("OR (l2.Name like " & strSQLEintCodeIng2 & " " & vbCrLf)
                .Append("AND l2.codeTrans=" & intCodeTrans & ")) " & vbCrLf)
            End If

            ' Unwanted Ingredient Search
            If strIngredientsUnwanted.Length <> 0 Then
                Dim strSQLEintCodeIngUw1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR name LIKE ", "@nvcIngUnWanted", ReplaceSpecialCharacters(strIngredientsUnwanted), CChar(","), True)
                'compare it using egswliste.anme w/codetarns
                .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select code FROM egswListe WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND CodeTrans=" & intCodeTrans & " ) ) " & vbCrLf)
                'compare it using egswlistetransaltion.name w/codetrans
                .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select codeliste FROM egswlistetranslation WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND codetrans=" & intCodeTrans & " ) ) " & vbCrLf)
                'compare it using egswliste.name w/o codetrans for liste's w/o translstions
                .Append("AND r.code NOT IN (select firstcode FROM egswDetails WHERE secondcode IN (select code FROM egswListe WHERE name LIKE " & strSQLEintCodeIngUw1 & " AND Code NOT IN (SELECT codeListe FROM egswListeTranslation WHERE CodeTrans=" & intCodeTrans & " )) ) " & vbCrLf)
            End If

            'category


            If strCategory.Length > 0 And strCategory.CompareTo("All Categories") <> 0 Then
                .Append("AND (c.name=@nvcCategory OR cT.name=@nvcCategory) " & vbCrLf)
                cmd.Parameters.Add("@nvcCategory", SqlDbType.NVarChar, 150).Value = ReplaceSpecialCharacters(strCategory)
            End If

            'source
            If strSource.Length > 0 And strSource.CompareTo("All Sources") <> 0 Then
                .Append("AND source.name=@nvcSource " & vbCrLf)
                cmd.Parameters.Add("@nvcSource", SqlDbType.NVarChar, 150).Value = ReplaceSpecialCharacters(strSource)
            End If

            'keywords
            If strKeywords.Length > 0 Then
                Dim strSQLEintCode1 As String = AddParam(cmd, SqlDbType.NVarChar, " OR k.name LIKE ", "@nvcKeyworda", ReplaceSpecialCharacters(strKeywords), CChar(","), True)
                Dim strSQLEintCode2 As String = AddParam(cmd, SqlDbType.NVarChar, " OR kt.name LIKE ", "@nvcKeywordb", ReplaceSpecialCharacters(strKeywords), CChar(","), True)

                ' find match keyword in keyword parent table
                .Append("AND ((k.name LIKE " & strSQLEintCode1 & " " & vbCrLf)

                ' find match keyword in keyword parent table translation
                .Append("OR (kt.name LIKE " & strSQLEintCode2 & " " & vbCrLf)
                .Append("AND kt.codetrans=" & intCodeTrans & "))) " & vbCrLf)
            End If

            'nutrient rules
            If strNutrientRules.Trim.Length > 0 Then
                Dim cNutrientRules As clsNutrientRules = New clsNutrientRules(udtUser, enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataSet)
                Dim arr() As String = strNutrientRules.Split(CChar(","))
                Array.Sort(arr)

                Dim i As Integer = 1
                Dim intLastPosition As Integer = 0
                Dim arr2() As String
                While i < arr.Length
                    arr2 = arr(i).Split(CChar("-"))
                    If CInt(arr2(0)) > 0 Then
                        Dim rwTemp As DataRow = CType(cNutrientRules.GetList(CInt(arr2(1))), DataSet).Tables(1).Rows(0)
                        If intLastPosition = CInt(arr2(0)) Then
                            .Append(" OR egswNutrientVal.N" & arr2(0) & " BETWEEN " & CStr(rwTemp("minimum")) & " AND " & CStr(rwTemp("maximum")) & " " & vbCrLf)
                        Else
                            If i = 1 Then
                                .Append(" AND ( ")
                            Else
                                .Append(" ) AND ( ")
                            End If

                            .Append(" egswNutrientVal.N" & arr2(0) & " BETWEEN " & CStr(rwTemp("minimum")) & " AND " & CStr(rwTemp("maximum")) & " " & vbCrLf)
                        End If

                        If i + 1 = arr.Length Then
                            .Append(" ) ")
                        End If

                        intLastPosition = CInt(arr2(0))
                    End If
                    i += 1
                End While
            End If

            If strAllergens.Length > 0 Then
                If strAllergens.IndexOf("NOT") > -1 Then
                    .Append(" AND (a.codeAllergen " & strAllergens & " OR a.codeAllergen IS NULL) " & vbCrLf)
                Else
                    .Append(" AND a.codeAllergen " & strAllergens & " " & vbCrLf)
                End If
            End If

            If strSort = "" Then
                .Append(" ORDER BY [name] " & vbCrLf)
            Else
                .Append(" ORDER BY " & strSort & " " & vbCrLf)
            End If

            .Append("DECLARE @FirstRec int, @LastRec int, @MoreRecords int " & vbCrLf)
            .Append("SELECT @FirstRec = (@Page - 1) * @RecsPerPage " & vbCrLf)
            .Append("SELECT @LastRec = @Page * @RecsPerPage + 1 " & vbCrLf)
            .Append("SELECT @iRow=COUNT(*) FROM #TempResults " & vbCrLf)
            .Append("SELECT @MoreRecords=COUNT(*) FROM #TempResults WHERE ID>@LastRec " & vbCrLf)

            .Append("DELETE FROM #TempResults WHERE ID <= @FirstRec OR ID >=@LastRec " & vbCrLf)

            BuildFullySharedString(sbSQL, udtUser, intListeType, cmd)

            .Append("SELECT DISTINCT tr.ID, r.code, r.type, CASE WHEN l.name IS NULL OR LEN(RTRIM(LTRIM(l.name)))=0 THEN r.name ELSE l.name end name, " & vbCrLf)
            .Append("CASE WHEN l.remark IS NULL OR LEN(RTRIM(LTRIM(l.remark)))=0 THEN r.remark ELSE l.remark end remark, " & vbCrLf)
            .Append("CASE WHEN l.description IS NULL OR LEN(RTRIM(LTRIM(l.description)))=0 THEN r.description ELSE l.description end description, " & vbCrLf)
            .Append("r.picturename, replace(r.number, CHAR(1),'') AS NUMBER," & vbCrLf)
            .Append("@MoreRecords AS MoreRecords " & vbCrLf)
            .Append("FROM egswListe r " & vbCrLf)
            .Append("INNER JOIN #TempResults tr ON r.code=tr.Code " & vbCrLf)
            ' Join rnListeTranslation table
            .Append("LEFT OUTER JOIN egswListeTranslation l on r.code=l.codeListe and l.codetrans IN (" & intCodeTrans & ",NULL) " & " ")
            .Append("ORDER BY tr.ID ")
        End With

        Try
            With cmd
                .Connection = cn
                .CommandText = sbSQL.ToString
                .CommandType = CommandType.Text
                .Parameters.Add("@Page", SqlDbType.Int).Value = intPagenumber
                .Parameters.Add("@RecsPerPage", SqlDbType.Int).Value = intPageSize
                .Parameters.Add("@iRow", SqlDbType.Int).Direction = ParameterDirection.Output

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With

                intTotalRows = CInt(.Parameters("@iRow").Value)
            End With

            ' IsListeOwned(dt, udtUser.Site.Code)
            Return dt

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
            Return Nothing
        End Try
    End Function

    Public Function UpdateListeMerchandiseInfo(ByRef info As structListe, Optional ByVal blnCompareByCodeSite As Boolean = False, _
           Optional ByVal strCodeMergeList As String = "", Optional ByVal OverwriteDescription As Integer = 1) As enumEgswErrorCode
        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim cmd As SqlCommand = New SqlCommand
        Try
            With cmd
                .Connection = cn
                .CommandText = "sp_egswListeUpdateMerchandiseInfo"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = info.Code
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = info.CodeTrans

                .Parameters.Add("@nvcProductivity", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(info.Productivity)
                .Parameters.Add("@nvcStorage", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(info.Storage)
                .Parameters.Add("@nvcPreparation", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(info.Preparation)
                If info.Ingredients = Nothing Then info.Ingredients = ""
                If info.CookingTip = Nothing Then info.CookingTip = ""
                If info.Description = Nothing Then info.Description = ""
                If info.Refinement = Nothing Then info.Refinement = ""
                .Parameters.Add("@nvcIngredients", SqlDbType.NVarChar, 2000).Value = ReplaceSpecialCharacters(info.Ingredients)
                .Parameters.Add("@nvcCookingTip", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(info.CookingTip)
                .Parameters.Add("@nvcDescription", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(info.Description)
                .Parameters.Add("@nvcRefinement", SqlDbType.NVarChar, 700).Value = ReplaceSpecialCharacters(info.Refinement)

                .Parameters.Add("@intCodeListeNew", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                cn.Open()
                .ExecuteNonQuery()
                info.Code = CInt(.Parameters("@intCodeListeNew").Value)
                cn.Close()

                L_ErrCode = CType(.Parameters("@retval").Value, enumEgswErrorCode)
            End With
        Catch ex As Exception
            info.Code = -1
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return L_ErrCode
    End Function


#End Region

#Region " For Tibits Merchandise List "
    Public Function fctGetTibitsMerchandiseList(ByVal strPathNormal As String, Optional ByVal strCategoryList As String = "", Optional ByVal strListeCodeList As String = "", Optional ByVal sortBy As enumPrintSortType = enumPrintSortType.None) As DataSet
        '// Create Picture Table
        Dim ds As New DataSet("dsMerchandiseList")
        Dim dt As New DataTable("dtMain")
        With dt.Columns
            .Add("Code", System.Type.GetType("System.Int32"))
            .Add("Number")
            .Add("NameEn")
            .Add("NameDe")
            .Add("Ratio") 'Supplier Number
            .Add("UnitEn")
            .Add("UnitDe")
            .Add("picture", System.Type.GetType("System.Byte[]"))
            .Add("picture2", System.Type.GetType("System.Byte[]"))
            .Add("picturename")
            .Add("picturename2")
        End With

        '// Fetch

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader = Nothing
        Dim sb As New StringBuilder

        With sb

            .Append("SELECT DISTINCT l.Code, l.Number, ")
            .Append("t.Name As NameEn, ")
            .Append("NameDe=(SELECT Top 1 t2.Name ")
            .Append("FROM EgswListeTranslation t2 ")
            .Append("WHERE t2.Codetrans=3 AND t2.Codeliste = l.Code), ")
            .Append("p2.ratio AS Ratio, ")

            .Append("(CASE WHEN utEn.Name='' THEN utDe.Name ELSE utEn.Name END) AS UnitEn1, ")
            .Append("(CASE WHEN utEn2.Name='' THEN utDe2.Name ELSE utEn2.Name END) AS UnitEn2, ")
            .Append("(CASE WHEN utDe.Name='' THEN utEn.Name ELSE utDe.Name END) AS UnitDe1, ")
            .Append("(CASE WHEN utDe2.Name='' THEN utEn2.Name ELSE utDe2.Name END) AS UnitDe2, ")
            .Append("l.Picturename ")
            .Append("FROM Egswliste l ")
            .Append("INNER JOIN EgswListeTranslation t ON l.Code=t.Codeliste AND t.Codetrans=1 ")
            .Append("INNER JOIN EgswSupplier s ON l.Supplier=s.Code ")

            .Append("LEFT OUTER JOIN EgswListeSetPrice p ON l.code=p.codeliste AND p.Position=1 AND p.CodeSetPrice = 1 ")
            .Append("LEFT OUTER JOIN EgswListeSetPrice p2 ON l.code=p2.codeliste AND p2.Position=2 AND p2.CodeSetPrice = 1 ")

            .Append("LEFT OUTER JOIN EgswUnit u ON p.unit=u.code ")
            .Append("LEFT OUTER JOIN EgswUnit u2 ON p2.unit=u2.code ")

            .Append("LEFT OUTER JOIN EgswItemTranslation utEn ON u.Code=utEn.Code AND utEn.Codetrans=1 AND utEn.CodeEgswTable=135 ")
            .Append("LEFT OUTER JOIN EgswItemTranslation utEn2 ON u2.Code=utEn2.Code AND utEn2.Codetrans=1 AND utEn2.CodeEgswTable=135 ")

            .Append("LEFT OUTER JOIN EgswItemTranslation utDe ON u.Code=utDe.Code AND utDe.Codetrans=3 AND utDe.CodeEgswTable=135 ")
            .Append("LEFT OUTER JOIN EgswItemTranslation utDe2 ON u2.Code=utDe2.Code AND utDe2.Codetrans=3 AND utDe2.CodeEgswTable=135 ")
            .Append("WHERE l.type=2 ")

            If strCategoryList.Length > 0 Then
                .Append("AND l.Category IN " & strCategoryList & " ")
            ElseIf strListeCodeList.Length > 0 Then
                .Append("AND l.Code IN " & strListeCodeList & " ")
            End If
            If sortBy = enumPrintSortType.Supplier Then
                .Append("Order by Supplier ")
            Else
                .Append("Order by NameDe ")
            End If
        End With

        Try
            With cmd
                .Connection = cn
                .CommandText = sb.ToString
                .CommandType = CommandType.Text

                cn.Open()
                dr = .ExecuteReader

                Dim row As DataRow
                Dim sPicture1ID As String
                Dim sPicture2ID As String
                Dim sPicture3ID As String
                Dim sPictures As String
                While dr.Read
                    ' Add row to DT
                    row = dt.NewRow
                    row("Code") = dr("Code")
                    row("Number") = dr("Number")
                    row("NameEn") = dr("NameEn")
                    row("NameDe") = dr("NameDe")
                    row("Ratio") = dr("Ratio")

                    Dim UnitEn1 As String = CStrDB(dr("UnitEn1"))
                    Dim UnitEn2 As String = CStrDB(dr("UnitEn2"))
                    Dim UnitDe1 As String = CStrDB(dr("UnitDE1"))
                    Dim UnitDe2 As String = CStrDB(dr("UnitDE2"))

                    If UnitEn1 = "" Then UnitEn1 = UnitDe1
                    If UnitEn2 = "" Then UnitEn2 = UnitDe2

                    Dim strUnitEN As String = ""
                    Dim strUnitDE As String = ""

                    strUnitEN = UnitEn1
                    If UnitEn2 <> "" Then
                        strUnitEN &= "/" & UnitEn2
                    End If

                    strUnitDE = UnitDe1
                    If UnitDe2 <> "" Then
                        strUnitDE &= "/" & UnitDe2
                    End If

                    row("UnitEn") = strUnitEN
                    row("UnitDe") = strUnitDE
                    row("picture") = System.DBNull.Value
                    row("picture2") = System.DBNull.Value
                    row("picturename") = ""
                    row("picturename2") = ""


                    ' Fetch Image from Livelink Server
                    sPictures = CStrDB(dr("picturename"))
                    If sPictures.Length > 0 Then
                        sPicture1ID = ParseString(sPictures, 0, CChar(";"))
                        sPicture2ID = ParseString(sPictures, 1, CChar(";"))
                        sPicture3ID = ParseString(sPictures, 2, CChar(";"))

                        Dim pic1, pic2, pic3 As Boolean
                        If sPicture1ID.Length > 0 Then
                            If File.Exists(strPathNormal & sPicture1ID) Then pic1 = True Else pic1 = False
                        Else
                            pic1 = False
                        End If
                        If sPicture2ID.Length > 0 Then
                            If File.Exists(strPathNormal & sPicture2ID) Then pic2 = True Else pic2 = False
                        Else
                            pic2 = False
                        End If
                        If sPicture3ID.Length > 0 Then
                            If File.Exists(strPathNormal & sPicture3ID) Then pic3 = True Else pic2 = False
                            pic3 = False
                        End If


                        If pic1 = False And pic2 = False And pic3 = False Then
                            GoTo NextRecord
                        End If

                        If pic1 = True Then
                            row("picture") = GetBytes(strPathNormal & sPicture1ID)
                            row("pictureName") = sPicture1ID
                        Else
                            If pic2 = True Then
                                row("picture") = GetBytes(strPathNormal & sPicture2ID)
                                row("pictureName") = sPicture2ID
                            ElseIf pic3 = True Then
                                row("picture") = GetBytes(strPathNormal & sPicture3ID)
                                row("pictureName") = sPicture3ID
                            Else
                                GoTo NextRecord
                            End If
                        End If

                        If pic2 = True Then
                            If Not pic1 = False Then
                                row("picture2") = GetBytes(strPathNormal & sPicture2ID)
                                row("pictureName2") = sPicture2ID
                            End If
                        ElseIf pic3 = True Then
                            If Not pic1 = False Then
                                row("picture2") = GetBytes(strPathNormal & sPicture3ID)
                                row("pictureName2") = sPicture3ID
                            End If
                        Else
                            row("picture2") = System.DBNull.Value
                        End If
                    End If
                    dt.Rows.Add(row)
NextRecord:
                End While
            End With

        Catch ex As Exception

        Finally
            dr.Close()
            cn.Close()
        End Try

        ds.Tables.Add(dt)
        Return ds
    End Function

    Private Function GetBytes(ByVal value As String) As Byte()
        If File.Exists(value) Then
            Dim f As FileStream
            f = File.OpenRead(value)

            Dim br As New BinaryReader(f)
            Dim b() As Byte

            b = br.ReadBytes(CInt(f.Length))
            br.Close()

            Return b
        Else
            Return Nothing
        End If
    End Function

#End Region

#Region "Compare Recipe" ' JBB Now 30 2010
    Public Function GetCompareRecipeList(ByVal intCodeListe As Integer, ByVal intLang As Integer, Optional intSite As Integer = 0, Optional intCodeUser As Integer = 0, Optional blnFinalOnly As Boolean = False) As DataTable
        Dim arrParams(4) As SqlParameter
        arrParams(0) = New SqlParameter("@CodeListe", SqlDbType.Int)
        arrParams(0).Value = intCodeListe
        arrParams(1) = New SqlParameter("@Lang", SqlDbType.Int)
        arrParams(1).Value = intLang
        arrParams(2) = New SqlParameter("@CodeSite", SqlDbType.Int)
        arrParams(2).Value = intSite
        arrParams(3) = New SqlParameter("@CodeUser", SqlDbType.Int)
        arrParams(3).Value = intCodeUser
        arrParams(3) = New SqlParameter("@FinalOnly", SqlDbType.Bit) 'AGL 2013.12.03
        arrParams(3).Value = blnFinalOnly

        Try
            Return CType(ExecuteFetchType(enumEgswFetchType.DataTable, L_strCnn, CommandType.StoredProcedure, "COMP_GetRecipeList", arrParams), DataTable)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetCompareRecipeIngredient(ByVal intCodeListe As Integer, ByVal intCodeCompare As Integer, ByVal intLang As Integer) As DataTable
        Dim arrParams(2) As SqlParameter
        arrParams(0) = New SqlParameter("@intCode", SqlDbType.Int)
        arrParams(0).Value = intCodeListe
        arrParams(1) = New SqlParameter("@intCodeCompare", SqlDbType.Int)
        arrParams(1).Value = intCodeCompare
        arrParams(2) = New SqlParameter("@intCodetrans", SqlDbType.Int)
        arrParams(2).Value = intLang
        Try
            Return CType(ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "COMP_RecipeIngredients", arrParams), DataTable)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetCompareRecipeNutrients(ByVal intCodeListe As Integer, ByVal intCompCodeListe As Integer, ByVal intCodeSite As Integer, intCodeSet As Integer) As DataTable
        Dim arrParams(3) As SqlParameter
        arrParams(0) = New SqlParameter("@intCodeSite", SqlDbType.Int)
        arrParams(0).Value = intCodeSite
        arrParams(1) = New SqlParameter("@intCodeListe", SqlDbType.Int)
        arrParams(1).Value = intCodeListe
        arrParams(2) = New SqlParameter("@intCompareCodeListe", SqlDbType.Int)
        arrParams(2).Value = intCompCodeListe
        arrParams(3) = New SqlParameter("@intCodeSet", SqlDbType.Int)
        arrParams(3).Value = intCodeSet

        Try
            Return CType(ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "COMP_GetRecipeNutrients", arrParams), DataTable)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetListeVersion(ByVal intCode As Integer) As String
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader
        Dim strName As String = ""

        With cmd
            .Connection = cn
            .CommandText = "Select ISNULL(Version, 1) As Versions " & _
                            "FROM	egswListe r " & _
                            "where r.code=@intCode "
            .CommandType = CommandType.Text
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
            Try
                cn.Open()
                dr = .ExecuteReader(CommandBehavior.CloseConnection)
                If dr.Read Then
                    strName = CStrDB(dr.Item("Versions"))
                End If
                dr.Close()
                cn.Close()
            Catch ex As Exception
                cn.Close()
                Throw ex
            End Try
        End With
        Return strName
    End Function


    Public Function GetListeVersionChange(ByVal intCode As Integer) As String
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader
        Dim strName As String = ""

        With cmd
            .Connection = cn
            .CommandText = "Select ISNULL(VersionToChange, 0) As VersionChange " & _
                            "FROM	egswListe r " & _
                            "where r.code=@intCode "
            .CommandType = CommandType.Text
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
            Try
                cn.Open()
                dr = .ExecuteReader(CommandBehavior.CloseConnection)
                If dr.Read Then
                    strName = IIf(CStrDB(dr.Item("VersionChange")) = "", 0, CStrDB(dr.Item("VersionChange")))
                End If
                dr.Close()
                cn.Close()
            Catch ex As Exception
                cn.Close()
                Throw ex
            End Try
        End With
        Return strName
    End Function



    'GET_ListeParentVersion
    Public Function GetListeParentVersion(ByVal intCode As Integer) As String
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader
        Dim strName As String = ""

        With cmd
            .Connection = cn
            .CommandText = "Select ISNULL(Parent,Code) As Parent " & _
                            "FROM	egswListe r " & _
                            "where r.code=@intCode "
            .CommandType = CommandType.Text
            .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
            Try
                cn.Open()
                dr = .ExecuteReader(CommandBehavior.CloseConnection)
                If dr.Read Then
                    strName = CStrDB(dr.Item("Parent"))
                End If
                dr.Close()
                cn.Close()
            Catch ex As Exception
                cn.Close()
                Throw ex
            End Try
        End With
        Return strName
    End Function



    Public Sub RemoveVersion(ByVal intCodeListe As Integer, Optional ByVal intCodeUser As Integer = 0, Optional ByVal intCodeSite As Integer = 0, Optional ByVal intCategory As Integer = -1)
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = New SqlConnection(L_strCnn)
            .CommandText = "UPDATE_ListeRemoveFromVersion"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
            .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
            .Parameters.Add("@intCategory", SqlDbType.Int).Value = intCategory 'JTOC 11.20.2013
            .Connection.Open()

            .ExecuteNonQuery()
            .Connection.Close()
        End With
    End Sub


    Public Sub SetVersionChange(ByVal intCodeListe As Integer, ByVal blChange As Boolean)
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = New SqlConnection(L_strCnn)
            .CommandText = "UPDATE EgswListe SET VersionToChange = " & IIf(blChange = True, "0", "1") & " WHERE Code = " & intCodeListe.ToString
            .CommandType = CommandType.Text
            .Connection.Open()

            .ExecuteNonQuery()
            .Connection.Close()
        End With
    End Sub



    'JBB 07.01.2011
    Public Function GetRecipeCookmodeVersion(ByVal intCodeListe As Integer, Optional ByVal blToCook As Boolean = False)
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim code As Integer
        Try
            With cmd
                .Connection = cn
                .CommandText = "GET_COOKMODEVERSION"
                .CommandType = CommandType.StoredProcedure

                If cn.State = ConnectionState.Closed Then cn.Open()

                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@ToCookMode", SqlDbType.Bit).Value = blToCook
                .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

                .ExecuteNonQuery()
                cn.Close()
                cn.Dispose()

                code = .Parameters("@retval").Value
            End With
        Catch ex As Exception
            code = -1
            If cn.State = ConnectionState.Open Then cn.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try

        cmd.Dispose()
        Return code
    End Function



    'JBB 05.24.2011
    Public Function GetCompareRecipeTime(ByVal intFrom As Integer, ByVal intTo As Integer, Optional ByVal intCodeTran As Integer = 1) As DataTable
        Dim arrParams(2) As SqlParameter
        arrParams(0) = New SqlParameter("@RecipeID", SqlDbType.Int)
        arrParams(0).Value = intFrom
        arrParams(1) = New SqlParameter("@RecipeIDCompare", SqlDbType.Int)
        arrParams(1).Value = intTo
        arrParams(2) = New SqlParameter("@CodeTrans", SqlDbType.Int)
        arrParams(2).Value = intCodeTran

        Try
            Return CType(ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "COMP_RecipeTime", arrParams), DataTable)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

#End Region

#Region "Recipe Brand" 'JBB Dec 07, 2010

    Public Function GetListeIngredientBrandList(ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer) As Object
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeListe", intCodeListe)
        arrParam(1) = New SqlParameter("@CodeTrans", intCodeTrans)
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "GET_ListeIngredientsBrandList", arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListeBrandList(ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer) As Object
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeListe", intCodeListe)
        arrParam(1) = New SqlParameter("@CodeTrans", intCodeTrans)
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "GET_ListeBrandList", arrParam)
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Public Function GetTempIngredientBrands(ByVal strCodeList As String, ByVal strSelected As String, ByVal intCodeTrans As Integer) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim sbSQL As New StringBuilder
        'With sbSQL
        '    .Append("SELECT DISTINCT B.Code as CodeBrand, CASE WHEN BT.Name IS NULL OR LTRIM(RTRIM(BT.Name)) = '' THEN B.Name ELSE BT.Name END as NameBrand")
        '    .Append(" FROM EgswListe L")
        '    .Append(" INNER JOIN EgswBrand B ON B.Code = L.Brand AND B.Code <> 0")
        '    .Append(" LEFT OUTER JOIN EgswItemTranslation BT ON BT.Code = B.Code AND BT.CodeEgswTable = 18")
        '    .Append(" AND BT.CodeTrans = " + intCodeTrans.ToString())
        '    .Append(" WHERE L.CODE IN (" + strCodeList + ")")
        '    If strSelected <> "" Then
        '        .Append(" AND B.CODE NOT IN (" + strSelected + ")")
        '    End If
        'End With
        'Try
        '    With cmd
        '        .Connection = cn
        '        .CommandText = sbSQL.ToString
        '        .CommandTimeout = 10000
        '        .CommandType = CommandType.Text

        '        With da
        '            .SelectCommand = cmd
        '            dt.BeginLoadData()
        '            .Fill(dt)
        '            dt.EndLoadData()
        '        End With
        '    End With
        '    Return dt

        'Catch ex As Exception
        '    Throw New Exception(ex.Message, ex)
        '    Return Nothing
        'End Try

        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@strCodeList", SqlDbType.NVarChar, 1000)
        arrParam(0).Value = strCodeList
        arrParam(1) = New SqlParameter("@strSelected", SqlDbType.NVarChar, 1000)
        arrParam(1).Value = strSelected
        arrParam(2) = New SqlParameter("@intCodeTrans", SqlDbType.Int, 4)
        arrParam(2).Value = intCodeTrans
        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "GetTempIngredientBrands", arrParam)
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Public Sub DeleteListeBrands(ByVal intCodeListe As String, ByVal strCodeBrand As String)
        Try
            Dim strSQL As String = "DELETE FROM RecipeBrand WHERE CodeListe = " + intCodeListe.ToString()
            If strCodeBrand.Trim <> "" Then
                strSQL += " AND Brand NOT IN (" + strCodeBrand + ")"
            End If
            ExecuteNonQuery(L_strCnn, CommandType.Text, strSQL)
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
    End Sub

    Public Sub InsertListeBrands(ByVal dtBrands As DataTable, ByVal intCodeListe As Integer)
        For Each drBrands As DataRow In dtBrands.Rows
            Dim intCodeBrand As String = CInt(drBrands("CodeBrand"))
            Dim intSequence As Integer = CBool(drBrands("Sequence"))
            Dim intCodeBrandClassification As Integer = CInt(drBrands("BrandClassification"))
            Dim arrParam(3) As SqlParameter
            arrParam(0) = New SqlParameter("@CodeListe", SqlDbType.Int)
            arrParam(0).Value = intCodeListe
            arrParam(1) = New SqlParameter("@CodeBrand", SqlDbType.Int)
            arrParam(1).Value = intCodeBrand
            arrParam(2) = New SqlParameter("@CodeBrandClassification", SqlDbType.Int)
            arrParam(2).Value = intCodeBrandClassification
            arrParam(3) = New SqlParameter("@Sequence", SqlDbType.Int)
            arrParam(3).Value = intSequence
            Try
                Dim intErrCode As Integer = ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "INSERT_ListeBrands", arrParam)
            Catch ex As Exception
                Throw ex
            End Try
        Next
    End Sub

    Public Function GetBrandClassification() As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        With cmd
            .Connection = cn
            .CommandType = CommandType.Text
            .CommandText = "SELECT ID, Name FROM BrandClassification"
        End With
        Try
            Return ExecuteFetchType(enumEgswFetchType.DataTable, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Sub UpdateListePrimaryBrand(ByVal intCodeBrand As Integer, ByVal intCodeListe As Integer)
        Try
            If intCodeBrand = -2 Then
                ExecuteNonQuery(L_strCnn, CommandType.Text, "UPDATE EgswListe SET BRAND = NULL WHERE Code = " + intCodeListe.ToString())

            Else
                ExecuteNonQuery(L_strCnn, CommandType.Text, "UPDATE EgswListe SET BRAND = " + intCodeBrand.ToString() + " WHERE Code = " + intCodeListe.ToString())
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

    Public Function GetBrandSiteofBrand(ByVal intCodeBrand As Integer) As String
        Dim strBrandSite As String = ""
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader

        With cmd
            .Connection = cn
            .CommandText = "select BS.Name from BrandToBrandSite BBS inner join brandsite BS on BS.ID = BBS.BrandSite where Brand =@intCodeBrand"
            .CommandType = CommandType.Text
            .Parameters.Add("@intCodeBrand", SqlDbType.Int).Value = intCodeBrand
            Try
                cn.Open()
                dr = .ExecuteReader(CommandBehavior.CloseConnection)
                If dr.Read Then
                    strBrandSite = CStrDB(dr.Item("Name"))
                End If
                dr.Close()
                cn.Close()
            Catch ex As Exception
                cn.Close()
                Throw ex
            End Try
        End With
        Return strBrandSite
    End Function


    Public Function GetPrimaryBrandbyCodeListe(intCodeListe As Integer) As String
        Dim strBrand As String = ""
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader

        With cmd
            .Connection = cn
            .CommandText = "select  dbo.fn_GET_PrimaryBrandByCodeListe (@intCodeListe) as PrimaryBrand"
            .CommandType = CommandType.Text
            .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
            Try
                cn.Open()
                dr = .ExecuteReader(CommandBehavior.CloseConnection)
                If dr.Read Then
                    strBrand = CStrDB(dr.Item("PrimaryBrand"))
                End If
                dr.Close()
                cn.Close()
            Catch ex As Exception
                cn.Close()
                Throw ex
            End Try
        End With
        Return strBrand
    End Function


#End Region

#Region "Recipe Placement"

    Public Function GetListePlacementList(ByVal intCodeListe As Integer) As DataTable
        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeListe", intCodeListe)
        Try
            Return ExecuteFetchType(enumEgswFetchType.DataTable, L_strCnn, CommandType.StoredProcedure, "GET_ListePlacementList", arrParam)
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Public Sub InsertListePlacement(ByVal dtPlacement As DataTable, ByVal intCodeListe As Integer)
        For Each drPlacement As DataRow In dtPlacement.Rows
            Dim intCode As Integer = CInt(drPlacement("code"))
            Dim strPlacement As String = CStrDB(drPlacement("Placement"))
            Dim strDescription As String = CStrDB(drPlacement("Description"))
            Dim dtmDates As DateTime = CDate(drPlacement("dates"))
            Dim strCodeBrand As String = CStrDB(drPlacement("codebrand"))
            Dim strCodeBrandSite As String = CStrDB(drPlacement("codebrandsite"))
            Dim strPlacementID As String = CStrDB(drPlacement("PlacementID"))
            Dim strCodeLBS As String = CStrDB(drPlacement("CodeLBS"))

            Dim arrParam(8) As SqlParameter
            arrParam(0) = New SqlParameter("@Code", SqlDbType.Int)
            arrParam(0).Value = intCode
            arrParam(1) = New SqlParameter("@CodeListe", SqlDbType.Int)
            arrParam(1).Value = intCodeListe
            arrParam(2) = New SqlParameter("@Placement", strPlacement)
            arrParam(3) = New SqlParameter("@dates", dtmDates)
            arrParam(4) = New SqlParameter("@description", strDescription)
            If strCodeBrand.Trim() <> "" Then
                arrParam(5) = New SqlParameter("@CodeBrand", CInt(strCodeBrand))
            Else
                arrParam(5) = New SqlParameter("@CodeBrand", Nothing)
            End If
            If strCodeBrandSite.Trim() <> "" Then
                arrParam(6) = New SqlParameter("@CodeBrandSite", CInt(strCodeBrandSite))
            Else
                arrParam(6) = New SqlParameter("@CodeBrandSite", Nothing)
            End If
            If strPlacementID.Trim() <> "" Then
                arrParam(7) = New SqlParameter("@PlacementID", CInt(strPlacementID))
            Else
                arrParam(7) = New SqlParameter("@PlacementID", Nothing)
            End If

            '' -- JBB 04.12.2012
            If strCodeLBS.Trim() <> "" Then
                arrParam(8) = New SqlParameter("@CodeLBS", CInt(strCodeLBS))
            Else
                arrParam(8) = New SqlParameter("@CodeLBS", Nothing)
            End If

            ''--


            Try
                Dim intErrCode As Integer = ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "INSERT_ListePlacement", arrParam)
            Catch ex As Exception
                Throw ex
            End Try
        Next
    End Sub

    Public Sub DeleteListePlacement(ByVal intCodeListe As String, ByVal strCode As String)
        Try
            Dim strSQL As String = "DELETE FROM EgswListePlacement WHERE CodeListe = " + intCodeListe.ToString()
            If strCode.Trim() <> "" Then
                strSQL += " AND CODE NOT IN (" + strCode + ")"
            End If
            ExecuteNonQuery(L_strCnn, CommandType.Text, strSQL)
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
    End Sub

    Public Sub DeleteListePlacementByCodeLBS(ByVal intCodeListe As String, ByVal strCode As String)
        Try
            Dim strSQL As String = "DELETE FROM EgswListePlacement WHERE CodeListe = " + intCodeListe.ToString()
            If strCode.Trim() <> "" Then
                strSQL += " AND CODELBS IN (" + strCode + ")"
            End If
            ExecuteNonQuery(L_strCnn, CommandType.Text, strSQL)
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
    End Sub

    Public Function GetTop10RecipeReplacement(ByVal intCodeListe As Integer) As Object
        '
        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeListe", intCodeListe)
        Try
            Return ExecuteFetchType(enumEgswFetchType.DataTable, L_strCnn, CommandType.StoredProcedure, "GET_TOP10RecipePlacement", arrParam)
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Public Function GetPlacementBrandSite(ByVal intCodeListe As Integer, ByVal intCodeBrand As Integer)
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeListe", intCodeListe)
        arrParam(1) = New SqlParameter("@CodeBrand", intCodeBrand)
        Try
            Return ExecuteFetchType(enumEgswFetchType.DataTable, L_strCnn, CommandType.StoredProcedure, "GET_PlacementBrandSite", arrParam)
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Public Sub UpdateForMassPlacement(strCodeListe As String, strPlacement As String, dDataes As Date, strDescription As String, strPlacementID As String, ByRef dtExisting As DataTable)
        Dim arrParam(4) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeListe", strCodeListe)
        arrParam(1) = New SqlParameter("@Placement", strPlacement)
        arrParam(2) = New SqlParameter("@dates", dDataes)
        arrParam(3) = New SqlParameter("@Description", strDescription)
        If strPlacementID <> "" Then
            arrParam(4) = New SqlParameter("@PlacementID", strPlacementID)
        Else
            arrParam(4) = New SqlParameter("@PlacementID", Nothing)
        End If
        Try

            'ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "UPDATE_MASSRecipePlacement", arrParam)

            dtExisting = ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "UPDATE_MASSRecipePlacement", arrParam).Tables(0)
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
    End Sub

#End Region

#Region "Recipe Time"

    Public Sub DeleteRecipeTime(ByVal intRecipeID As Integer)
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = New SqlConnection(L_strCnn)
            .CommandText = "DELETE_RecipeTime"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@RecipeID", SqlDbType.Int).Value = intRecipeID
            .Connection.Open()

            .ExecuteNonQuery()
            .Connection.Close()
        End With
    End Sub

    Public Sub DeleteRecipeTimebyTimeID(ByVal intRecipeID As Integer, ByVal intRecipeTimeID As Integer)
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = New SqlConnection(L_strCnn)
            .CommandText = "DELETE_RecipeTimeByTimeID"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@RecipeID", SqlDbType.Int).Value = intRecipeID
            .Parameters.Add("@RecipeTimeID", SqlDbType.Int).Value = intRecipeTimeID
            .Connection.Open()
            .ExecuteNonQuery()
            .Connection.Close()
        End With
    End Sub


    Public Sub InsertRecipeTime(ByVal intRecipeID As Integer, ByVal intRecipeTimeID As Integer, ByVal intRecipeTimeHH As Integer, ByVal intRecipeTimeMM As Integer, ByVal intRecipeTimeSS As Integer, ByVal intSequence As Integer)
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        With cmd
            .Connection = New SqlConnection(L_strCnn)
            .CommandText = "UPDATE_RecipeTime"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@RecipeID", SqlDbType.Int).Value = intRecipeID
            .Parameters.Add("@RecipeTimeID", SqlDbType.Int).Value = intRecipeTimeID
            .Parameters.Add("@Sequence", SqlDbType.Int).Value = intSequence
            .Parameters.Add("@RecipeTimeHH", SqlDbType.Int).Value = intRecipeTimeHH
            .Parameters.Add("@RecipeTimeMM", SqlDbType.Int).Value = intRecipeTimeMM
            .Parameters.Add("@RecipeTimeSS", SqlDbType.Int).Value = intRecipeTimeSS
            .Parameters.Add("@ListOrderID", SqlDbType.Int).Value = intSequence

            .Connection.Open()
            .ExecuteNonQuery()
            .Connection.Close()
        End With
    End Sub

    Public Function GetRecipeTime(ByVal intRecipeID As Integer, Optional ByVal intCodeTrans As Integer = 1, Optional intCodeSite As Integer = -1) As Object
        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@RecipeID", SqlDbType.Int, 4)
        arrParam(0).Value = intRecipeID

        ' RDC 03.05.2013 - Added for Recipe Time translation
        arrParam(1) = New SqlParameter("@CodeTrans", SqlDbType.Int, 4) 'AGL 2013.01.11
        arrParam(1).Value = intCodeTrans

        ' JTOC 23.04.2013 - Added CodeSite
        arrParam(2) = New SqlParameter("@CodeSite", SqlDbType.Int, 4)
        arrParam(2).Value = intCodeSite

        Return ExecuteFetchType(enumEgswFetchType.DataTable, L_strCnn, CommandType.StoredProcedure, "sp_GetRecipeTime", arrParam)

        ' RDC 03.05.2013 - Removed for Recipe Time translation
        'Return ExecuteFetchType(enumEgswFetchType.DataTable, L_strCnn, CommandType.StoredProcedure, "GET_RecipeTime", arrParam)
    End Function

    Public Sub SaveRecipeTime(ByVal dtRecipeTime As DataTable, ByVal intCodeListe As Integer)

        For Each drRecipeTime As DataRow In dtRecipeTime.Rows
            Dim strHH As String = drRecipeTime("RecipeTimeHH").ToString()
            Dim strMM As String = drRecipeTime("RecipeTimeMM").ToString()
            Dim strSS As String = drRecipeTime("RecipeTimeSS").ToString()
            Dim intTimeID As Integer = CInt(drRecipeTime("RecipeTimeID").ToString())
            Dim intSequence As Integer = CInt(drRecipeTime("Sequence").ToString())
            If intTimeID <> "-1" Then
                If strHH = "-1" And strMM = "-1" And strSS = "-1" Then
                    DeleteRecipeTimebyTimeID(intCodeListe, intTimeID)
                Else
                    If strHH = "-1" Then strHH = 0
                    If strMM = "-1" Then strMM = 0
                    If strSS = "-1" Then strSS = 0
                    Dim intHH As Integer = CInt(strHH)
                    Dim intMM As Integer = CInt(strMM)
                    Dim intSS As Integer = CInt(strSS)
                    InsertRecipeTime(intCodeListe, intTimeID, intHH, intMM, intSS, intSequence)
                End If
            End If
        Next

    End Sub

    ''-- JBB 01.06.2012
    Public Function GetTimeList(ByVal intCodeTrans As Integer, Optional ByVal intRecipeID As Integer = -1, Optional intCodeSite As Integer = -1) As DataTable
        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@RecipeID", SqlDbType.Int, 4)
        arrParam(0).Value = intRecipeID
        arrParam(1) = New SqlParameter("@CodeTrans", SqlDbType.Int, 4) 'AGL 2013.01.11
        arrParam(1).Value = intCodeTrans

        ' JTOC 23.04.2013 - Added CodeSite
        arrParam(2) = New SqlParameter("@CodeSite", SqlDbType.Int, 4)
        arrParam(2).Value = intCodeSite
        ' RDC 03.05.2013 - Added for Recipe Time translation
        Return ExecuteFetchType(enumEgswFetchType.DataTable, L_strCnn, CommandType.StoredProcedure, "sp_GetRecipeTime", arrParam)

        ' RDC 03.05.2013 - Removed for Recipe Time translation
        'Return ExecuteFetchType(enumEgswFetchType.DataTable, L_strCnn, CommandType.StoredProcedure, "GET_RecipeTime", arrParam)

    End Function
    ''--

    ' RDC 03.05.2013 - Recipe Time Name translation
    Public Function GetTimeTranslation(intCodeTrans As Integer) As Integer
        Dim cn As New SqlConnection(L_strCnn)
        Dim intRetVal As Integer = 1
        Try
            cn.Open()
            Dim cm As New SqlCommand("Select Code From dbo.EgswTranslation Where Code = " & intCodeTrans, cn)
            intRetVal = cm.ExecuteScalar
            cm.Dispose()
            cn.Close()
        Catch ex As Exception
        End Try
        Return intRetVal
    End Function
    ' End

    'RDC 03.15.2013 - CWM-4672 Fix
    'RDC 04.10.2013 - Spell Checker fix
    Public Function GetSpellCheckerLanguagePref(strLangCodeType As String, Optional intReturnType As Integer = 0) As String
        Dim strReturnResult As String = ""
        Dim strCondition As String = ""
        Dim cn As New SqlConnection(L_strCnn)

        Try
            cn.Open()

            Select Case intReturnType
                Case 0 ' Language Code
                    strReturnResult = "1"
                    strCondition = " Where a.Code = " & CType(strLangCodeType, Integer)
                Case 1 ' Language Name
                    strReturnResult = "English"
                    strCondition = " Where a.[Language] = '" & strLangCodeType.Replace("'", "''").Trim & "'"
                Case 2
                    strReturnResult = "2"
                    strCondition = " Where a.[Language] = '" & strLangCodeType.Replace("'", "''").Trim & "'"
                Case 3
                    strReturnResult = "English"
                    strCondition = " Where a.Code = " & CType(strLangCodeType, Integer)
                Case Else

            End Select

            Dim cm As New SqlCommand("Select  a.Code As LangCode, a.[Language], b.[Name], " & vbCrLf & _
                                             "IsNull(a.CodeRef,1) As CodeRef, IsNull(b.CodeDictionary,1) As CodeDictionary " & vbCrLf & _
                                     "From		  EgswLanguage    As a " & vbCrLf & _
                                      "Left Join EgswTranslation As b On a.[Language] = b.[Name] " & vbCrLf & strCondition, cn)
            Dim dr As SqlDataReader = cm.ExecuteReader
            If dr.HasRows Then
                While dr.Read
                    Select Case intReturnType
                        Case 0, 2
                            strReturnResult = dr.Item("CodeDictionary")
                        Case 1, 3
                            strReturnResult = dr.Item("Language")
                        Case Else

                    End Select
                End While
            End If
            dr.Close()
            cm.Dispose()
            cn.Close()
            cm = Nothing
            cn = Nothing
        Catch ex As Exception

        End Try

        Return strReturnResult
    End Function

#End Region

#Region "Recipe BrandSite"

    Public Function GetRecipeBrandSite(ByVal intCodeListe As Integer, ByVal blShowAll As Boolean) As DataTable
        Dim dtTable As New DataTable
        Dim da As New SqlDataAdapter
        Dim cmd As New SqlCommand
        Dim conn As SqlConnection
        conn = New SqlConnection(L_strCnn)
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "GET_RecipeBrandSite"
        cmd.Parameters.Add("@CodeListe", SqlDbType.Int, 4).Value = intCodeListe
        cmd.Parameters.Add("@ShowAll", SqlDbType.Bit, 1).Value = blShowAll

        da.SelectCommand = cmd
        dtTable.BeginLoadData()
        da.Fill(dtTable)
        dtTable.EndLoadData()

        Return dtTable
    End Function

    Public Function ProcessRecipeBrandSite(ByVal arrBrandSite As ArrayList, ByVal intCodeListe As Integer)
        For Each strBrandSite As String In arrBrandSite
            UpdateRecipeBrandSite(intCodeListe, CInt(strBrandSite))
        Next
    End Function

    Private Function UpdateRecipeBrandSite(ByVal intCodeListe As Integer, ByVal intCodeBrandSite As Integer)
        Dim cmd As New SqlCommand
        Dim conn As SqlConnection
        conn = New SqlConnection(L_strCnn)
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "UPDATE_RecipeBrandSite"
        cmd.Parameters.Add("@CodeListe", SqlDbType.Int, 4).Value = intCodeListe
        cmd.Parameters.Add("@CodeBrnadSite", SqlDbType.Int, 4).Value = intCodeBrandSite
        cmd.Connection.Open()
        cmd.ExecuteNonQuery()
        cmd.Connection.Close()

    End Function

    Public Function DeleteRecipeBrandSite(ByVal intCodeListe As Integer, ByVal strCodeBrandSite As String)
        Dim cmd As New SqlCommand
        Dim conn As SqlConnection
        conn = New SqlConnection(L_strCnn)
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "DELETE_RecipeBrandSite"
        cmd.Parameters.Add("@CodeListe", SqlDbType.Int, 4).Value = intCodeListe
        cmd.Parameters.Add("@CodeBrnadSite", SqlDbType.VarChar, 1000).Value = strCodeBrandSite
        cmd.Connection.Open()
        cmd.ExecuteNonQuery()
        cmd.Connection.Close()
    End Function

#End Region


#Region "Recipe BrandSite Consumers"

    Public Function GetRecipeBrandSiteCM(ByVal intCodeListe As String, ByVal blShowAll As Boolean, Optional ByVal intCodeSite As Integer = 0) As DataTable
        Dim dtTable As New DataTable
        Dim da As New SqlDataAdapter
        Dim cmd As New SqlCommand
        Dim conn As SqlConnection
        conn = New SqlConnection(L_strCnn)
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "GET_RecipeBrandSiteCM"
        cmd.Parameters.Add("@CodeListe", SqlDbType.NVarChar, 100).Value = intCodeListe
        cmd.Parameters.Add("@ShowAll", SqlDbType.Bit, 1).Value = blShowAll
        cmd.Parameters.Add("@intCodeSite", SqlDbType.Int, 4).Value = intCodeSite 'JTOC 11.06.2013 Added inCodeSite Parameter

        da.SelectCommand = cmd
        dtTable.BeginLoadData()
        da.Fill(dtTable)
        dtTable.EndLoadData()

        Return dtTable
    End Function

    Public Function ProcessRecipeBrandSiteCM(ByVal dtBrandSite As DataTable, ByVal intCodeListe As Integer)
        For Each drBS As DataRow In dtBrandSite.Rows
            Dim strBrandSite As String = drBS("codebrandsite").ToString()

            Dim dtDateTo As Date
            Dim dtDateFrom As Date
            dtDateTo = (CDateDB(drBS("BrandSiteDateTo")))
            dtDateFrom = (CDateDB(drBS("BrandSiteDateFrom")))

            UpdateRecipeBrandSiteCM(intCodeListe, CInt(strBrandSite), dtDateFrom, dtDateTo)
        Next
    End Function

    Private Function UpdateRecipeBrandSiteCM(ByVal intCodeListe As Integer, ByVal intCodeBrandSite As Integer, dtDateFrom As Date, dtDateTo As Date)
        Dim cmd As New SqlCommand
        Dim conn As SqlConnection
        conn = New SqlConnection(L_strCnn)
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "UPDATE_RecipeBrandSiteCM"
        cmd.Parameters.Add("@CodeListe", SqlDbType.Int, 4).Value = intCodeListe
        cmd.Parameters.Add("@CodeBrnadSite", SqlDbType.Int, 4).Value = intCodeBrandSite
        If dtDateFrom.ToShortDateString <> #1/1/1900# Then
            cmd.Parameters.Add("@DateFrom", SqlDbType.SmallDateTime).Value = dtDateFrom
        End If
        If dtDateTo.ToShortDateString <> #1/1/1900# Then
            cmd.Parameters.Add("@DateTo", SqlDbType.SmallDateTime).Value = dtDateTo
        End If


        cmd.Connection.Open()
        cmd.ExecuteNonQuery()
        cmd.Connection.Close()

    End Function

    Public Function DeleteRecipeBrandSiteCM(ByVal intCodeListe As Integer, ByVal strCodeBrandSite As String, Optional blnDeleteAll As Boolean = True)
        Dim cmd As New SqlCommand
        Dim conn As SqlConnection
        conn = New SqlConnection(L_strCnn)
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "DELETE_RecipeBrandSite"
        cmd.Parameters.Add("@CodeListe", SqlDbType.Int, 4).Value = intCodeListe
        cmd.Parameters.Add("@CodeBrnadSite", SqlDbType.VarChar, 1000).Value = strCodeBrandSite
        cmd.Parameters.Add("@blnDeleteAll", SqlDbType.Bit).Value = blnDeleteAll 'AGL 2012.10.31 
        cmd.Connection.Open()
        cmd.ExecuteNonQuery()
        cmd.Connection.Close()
    End Function

#End Region

#Region "Recipe Project"

    Public Function GetListeProject(ByVal intCodeListe As Integer, Optional ByVal strListe As String = "", Optional ByVal intSite As Integer = -1, Optional ByVal strText As String = "", Optional ByVal intCodeUser As Integer = -1, Optional ByVal intDisplayMode As Integer = -1)
        Dim arrParam(4) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeListe", intCodeListe)
        arrParam(1) = New SqlParameter("@CodeListList", strListe)
        arrParam(2) = New SqlParameter("@CodeSite", intSite)
        arrParam(3) = New SqlParameter("@Name", strText)
        arrParam(4) = New SqlParameter("@CodeUser", intCodeUser)
        Try
            If intDisplayMode = 3 Then
                Return ExecuteFetchType(enumEgswFetchType.DataSet, L_strCnn, CommandType.StoredProcedure, "sp_GetRecipeProjectList", arrParam)
            Else
                Return ExecuteFetchType(enumEgswFetchType.DataSet, L_strCnn, CommandType.StoredProcedure, "GET_RecipeProjectList", arrParam)
            End If

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
    End Function


    Public Sub DeleteRecipeProject(ByVal intCodeListe As Integer, ByVal strCodeProject As String)
        Dim cmd As New SqlCommand
        Dim conn As SqlConnection
        conn = New SqlConnection(L_strCnn)
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "DELETE_RecipeProject"
        cmd.Parameters.Add("@CodeListe", SqlDbType.Int, 4).Value = intCodeListe
        cmd.Parameters.Add("@CodeProject", SqlDbType.VarChar, 1000).Value = strCodeProject
        cmd.Connection.Open()
        cmd.ExecuteNonQuery()
        cmd.Connection.Close()
    End Sub

    Public Sub DeleteRecipeProjectActionMark(ByVal intCodeListe As Integer, ByVal strCodeProject As String)
        Dim cmd As New SqlCommand
        Dim conn As SqlConnection
        conn = New SqlConnection(L_strCnn)
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "DELETE_RecipeProjectActionMark"
        cmd.Parameters.Add("@CodeListe", SqlDbType.Int, 4).Value = intCodeListe
        cmd.Parameters.Add("@CodeProject", SqlDbType.VarChar, 1000).Value = strCodeProject
        cmd.Connection.Open()
        cmd.ExecuteNonQuery()
        cmd.Connection.Close()
    End Sub

    Public Function ProcessRecipeProject(ByVal arrProject As ArrayList, ByVal intCodeListe As Integer)
        For Each strProject As String In arrProject
            UpdateRecipeProject(intCodeListe, CInt(strProject))
        Next
    End Function

    Public Sub UpdateRecipeProject(ByVal intCodeListe As Integer, ByVal intCodeProject As Integer)
        Dim cmd As New SqlCommand
        Dim conn As SqlConnection
        conn = New SqlConnection(L_strCnn)
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "UPDATE_RecipeProject"
        cmd.Parameters.Add("@CodeListe", SqlDbType.Int, 4).Value = intCodeListe
        cmd.Parameters.Add("@CodeProject", SqlDbType.Int, 4).Value = intCodeProject
        cmd.Connection.Open()
        cmd.ExecuteNonQuery()
        cmd.Connection.Close()

    End Sub



#End Region

#Region "Recipe Tag"

    Public Function GetRecipeTag(ByVal intCodeListe As Integer) As Object
        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeListe", intCodeListe)
        Try
            Return ExecuteFetchType(enumEgswFetchType.DataTable, L_strCnn, CommandType.StoredProcedure, "GET_RecipeTagList", arrParam)
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Public Function UpdateRecipeTag(ByVal intCodeListe As Integer, ByVal strTagname As String) As Integer
        Dim intValue As Integer
        Dim cmd As New SqlCommand
        Dim conn As SqlConnection
        conn = New SqlConnection(L_strCnn)
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "UPDATE_RecipeTag"
        cmd.Parameters.Add("@CodeListe", SqlDbType.Int, 4).Value = intCodeListe
        cmd.Parameters.Add("@TagName", SqlDbType.VarChar, 260).Value = strTagname
        cmd.Parameters.Add("@retval", SqlDbType.Int, 4).Direction = ParameterDirection.ReturnValue
        'arrParam(5).Direction = ParameterDirection.ReturnValue
        cmd.Connection.Open()
        cmd.ExecuteNonQuery()
        intValue = cmd.Parameters("@retval").Value
        cmd.Connection.Close()
        Return intValue
    End Function

    Public Sub DeleteRecipeTag(ByVal intCodeListe As Integer)
        Dim cmd As New SqlCommand
        Dim conn As SqlConnection
        conn = New SqlConnection(L_strCnn)
        cmd.Connection = conn
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "DELETE_RecipeTag"
        cmd.Parameters.Add("@CodeListe", SqlDbType.Int, 4).Value = intCodeListe
        cmd.Connection.Open()
        cmd.ExecuteNonQuery()
        cmd.Connection.Close()
    End Sub

#End Region
    '
    '-- JBB 
#Region "Circle of Friends"

    Public Function GetListeCircleofFriends(ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer) As DataTable
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeListe", intCodeListe)
        arrParam(1) = New SqlParameter("@CodeTrans", intCodeTrans)
        Try
            Return ExecuteFetchType(enumEgswFetchType.DataTable, L_strCnn, CommandType.StoredProcedure, "GET_EgswListeLink", arrParam)
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Function

    Public Function GetListeSearchforCF(ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, ByVal strnumber As String, ByVal strname As String) As DataTable
        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(1) = New SqlParameter("@CodeTrans", intCodeTrans)
        arrParam(2) = New SqlParameter("@String1", strnumber)
        arrParam(3) = New SqlParameter("@String2", strname)

        Try
            Return ExecuteFetchType(enumEgswFetchType.DataTable, L_strCnn, CommandType.StoredProcedure, "GET_SearchforCF", arrParam)
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
    End Function

    Public Sub DeleteCircleofFriends(ByVal intCodeListe As Integer)
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        With cmd
            .Connection = cn
            .CommandType = CommandType.Text
            .CommandText = "Delete from EgswListeLink where code1=" & intCodeListe
            .Connection.Open()
            .ExecuteNonQuery()
            .Connection.Close()
        End With
    End Sub

    Public Sub InsertCircleofFriends(ByVal intCodeListe As Integer, ByVal intCircleofFriends As Integer)
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        With cmd
            .Connection = cn
            .CommandType = CommandType.StoredProcedure
            .CommandText = "INSERT_EgswListeLink"
            .Connection.Open()
            .Parameters.Add("@Code1", SqlDbType.Int, 4).Value = intCodeListe
            .Parameters.Add("@Code2", SqlDbType.Int, 4).Value = intCircleofFriends
            .ExecuteNonQuery()
            .Connection.Close()
        End With
    End Sub


#End Region

#Region "Digital Asset"
    Public Function UpdateDigitalAsset(ByVal strDigitalAsset As String, ByVal intID As Integer, ByVal blnFlag As Boolean) As enumEgswErrorCode
        Dim arrParam(2) As SqlParameter

        arrParam(0) = New SqlParameter("@DigitalAsset", SqlDbType.NVarChar, 200)
        arrParam(1) = New SqlParameter("@Id", SqlDbType.Int)
        arrParam(2) = New SqlParameter("@bitFlag", SqlDbType.Bit)

        arrParam(0).Value = strDigitalAsset
        arrParam(1).Value = intID
        arrParam(2).Value = blnFlag
        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswDigitalAssetUpdate", arrParam)
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
            Throw ex
        End Try
    End Function

    Public Function GetDigitalAsset(ByVal intID As Integer, ByVal blnFlag As Boolean) As Object
        Dim strSQL As String
        Dim arrParam(0) As SqlParameter

        If blnFlag Then
            strSQL = "SELECT IsNull(DigitalAsset,'') as DigitalAsset FROM EgswDetails WHERE Id=@Id"
        Else
            strSQL = "SELECT IsNull(DigitalAsset,'') as DigitalAsset FROM EgswListeNote WHERE Id=@Id"
        End If

        arrParam(0) = New SqlParameter("@Id", SqlDbType.Int)
        arrParam(0).Value = intID
        Try
            Return ExecuteDataset(L_strCnn, CommandType.Text, strSQL, arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function UpdateDigitalAssetRecipeNumber(ByVal strDigitalAssetId As String, ByVal recipenumber As String) As enumEgswErrorCode

        Dim sqlCmd As SqlCommand = New SqlCommand
        Dim flagX As Boolean = False
        Dim strSplitID As String() = strDigitalAssetId.Split(",")


        For Each strDigitalAssetId In strSplitID
            If Not strDigitalAssetId = "" Then
                Try
                    With sqlCmd
                        .Connection = New SqlConnection(L_strCnn)
                        .Connection.Open()
                        .CommandType = CommandType.StoredProcedure
                        .CommandText = "[sp_UpdateEgswDigitalAssetRecipeNumber]"
                        .Parameters.Add("@id", SqlDbType.Int).Value = strDigitalAssetId
                        .Parameters.Add("@nvcNumber", SqlDbType.NVarChar, 4000).Value = recipenumber
                        .ExecuteNonQuery()
                        .Parameters.Clear()
                        .Connection.Close()
                        .Connection.Dispose()
                        .Dispose()
                    End With


                Catch ex As Exception
                    Return enumEgswErrorCode.GeneralError
                    Throw ex
                End Try
            End If
        Next
        Return enumEgswErrorCode.OK

    End Function
#End Region


#Region "Ingredient ListeSetPrice"
    Public Function GetListeSetPriceLastPosition(ByVal lngCode As Long) As Integer
        Dim strSQL As String
        Dim arrParam(0) As SqlParameter
        Dim dt As DataTable

        strSQL = "select isnull(max(position),0) as Pos from egswlistesetprice where codeliste=@codeliste"

        arrParam(0) = New SqlParameter("@codeliste", SqlDbType.Int)
        arrParam(0).Value = lngCode
        Try
            dt = ExecuteDataset(L_strCnn, CommandType.Text, strSQL, arrParam).Tables(0)
            Return CInt(dt.Rows(0).Item("Pos"))
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function IsMerchandiseUsedByRecipe(ByVal lngCode As Long) As Boolean
        Dim strSQL As String
        Dim arrParam(0) As SqlParameter
        Dim dt As DataTable

        strSQL = "select count(secondcode) as ctr from egswdetails where secondcode=@codeliste"

        arrParam(0) = New SqlParameter("@codeliste", SqlDbType.Int)
        arrParam(0).Value = lngCode
        Try
            dt = ExecuteDataset(L_strCnn, CommandType.Text, strSQL, arrParam).Tables(0)
            If CLng(dt.Rows(0).Item("ctr")) > 0 Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function
#End Region
    ''GetIngredientNutrientList(Codeliste, CodeSet, CodeSetPrice, dblNutrientFactor)
    'Public Function GetIngredientNutrientList(ByVal nCodeListe As Integer, ByVal nCodeSet As Integer, ByVal nCodeSetPrice As Integer, Optional ByVal dNutFactor As Double = 1) As DataTable
    '    Dim sqlCmd As SqlCommand = New SqlCommand
    '    Dim dt As DataTable = New DataTable
    '    Try
    '        With sqlCmd
    '            .Connection = New SqlConnection(L_strCnn)
    '            .Connection.Open()
    '            .CommandType = CommandType.StoredProcedure
    '            .CommandText = "API_GET_IngredientNutrients"
    '            .Parameters.Add("@CodeListe", SqlDbType.Int).Value = nCodeListe
    '            .Parameters.Add("@CodeSet", SqlDbType.Int).Value = nCodeSet
    '            .Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = nCodeSetPrice
    '            .Parameters.Add("@NutFactor", SqlDbType.Float).Value = dNutFactor
    '        End With
    '        Dim da As New SqlDataAdapter(sqlCmd.CommandText, sqlCmd.Connection)
    '        da.Fill(dt)
    '        sqlCmd.Connection.Close()
    '    Catch ex As Exception
    '        sqlCmd.Connection.Close()
    '        Throw ex
    '    End Try
    '    Return dt
    'End Function

    Public Function GetIngredientNutrientList(ByVal nCodeListe As Integer, ByVal nCodeSet As Integer, ByVal nCodeSetPrice As Integer, ByVal nCodeTrans As Integer, Optional ByVal dNutFactor As Double = 1) As DataTable
        Dim cn As SqlConnection = New SqlConnection(L_strCnn)
        Dim sqlCmd As SqlCommand = New SqlCommand

        With sqlCmd
            .Connection = cn
            .CommandText = "API_GET_IngredientNutrients"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@CodeListe", SqlDbType.Int).Value = nCodeListe
            .Parameters.Add("@CodeSet", SqlDbType.Int).Value = nCodeSet
            .Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = nCodeSetPrice
            .Parameters.Add("@NutFactor", SqlDbType.Float).Value = dNutFactor
            .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = nCodeTrans
        End With
        Try
            Return ExecuteFetchType(enumEgswFetchType.DataTable, sqlCmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function ListeVerifiedUpdate(nCode As Integer, blnVal As Boolean, nCodetrans As Integer, nCodeuser As Integer) As enumEgswErrorCode 'DRR 02.12.2013
        Dim arrParam(4) As SqlParameter

        arrParam(0) = New SqlParameter("@intCode", SqlDbType.Int)
        arrParam(1) = New SqlParameter("@Value", SqlDbType.Bit)
        arrParam(2) = New SqlParameter("@Codetrans", SqlDbType.Int)
        arrParam(3) = New SqlParameter("@retVal", SqlDbType.Int)
        arrParam(4) = New SqlParameter("@CodeUser", SqlDbType.Int)

        arrParam(0).Value = nCode
        arrParam(1).Value = blnVal
        arrParam(2).Value = nCodetrans
        arrParam(3).Direction = ParameterDirection.ReturnValue
        arrParam(4).Value = nCodeuser

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswListeVerifiedUpdate", arrParam)
            Return CType(arrParam(3).Value, enumEgswErrorCode) 'enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
            Throw ex
        End Try
    End Function

    'AGL 2013.07.20
    Public Enum enumFilter As Integer
        Exact = 1
        StartsWith = 2
        Contains = 3
    End Enum

    ''' <summary>
    ''' Gets raw merchandise list from Egsw_Food_Desc
    ''' </summary>
    ''' <param name="intCodeSet">Code Nutrient Set</param>
    ''' <param name="intCodeSite">Code Site</param>
    ''' <param name="strSearchString">Search String</param>
    ''' <param name="intFilter">Search Filter</param>
    ''' <returns>DataTable</returns>
    ''' <remarks>Used primarily in Nutrient Linking, for merchandise</remarks>
    Public Function GetRawMerchandiseList(intCodeSet As Integer, intCodeSite As Integer, strSearchString As String, intFilter As clsListe.enumFilter) As DataTable
        'Dim sb As New StringBuilder
        'With sb
        '    .Append("SELECT Egsw_Food_Des.NDB_No, Egsw_Food_Des.[Desc], Egsw_Nut_Data.Nutr_No, Egsw_Nut_Data.Nutr_Val, EgswNutrientDef.Position , EgswNutrientDef.format ")
        '    .Append("From Egsw_Food_Des ")
        '    .Append("INNER JOIN Egsw_Nut_Data ON Egsw_Nut_Data.NDB_No = Egsw_Food_Des.NDB_No ")
        '    '.Append("INNER JOIN EgswNutrientDef ON EgswNutrientDef.position <=34 and CodeSite = " + intCodeSite.ToString() + " ")
        '    .Append("INNER JOIN EgswNutrientDef ON EgswNutrientDef.position <=34 and CodeSet = " + intCodeSet.ToString() + " ")
        '    .Append("AND Egsw_Nut_Data.Nutr_No = EgswNutrientDef.Nutr_No ")
        '    .Append("ORDER BY Egsw_Food_Des.[Desc], EgswNutrientDef.Position ")
        'End With

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand("sp_egswGetRawMerchandise", cn)
        Dim dr As SqlDataReader
        Dim dt As New DataTable("RawMerchandise")
        Dim row As DataRow


        cmd.Parameters.Add("@intCodeSet", SqlDbType.Int)
        cmd.Parameters.Add("@intCodeSite", SqlDbType.Int)
        cmd.Parameters.Add("@nvcSearchString", SqlDbType.NVarChar, 170)
        cmd.Parameters.Add("@intFilterType", SqlDbType.Int)

        cmd.Parameters("@intCodeSet").Value = intCodeSet
        cmd.Parameters("@intCodeSite").Value = intCodeSite
        cmd.Parameters("@nvcSearchString").Value = strSearchString
        cmd.Parameters("@intFilterType").Value = intFilter

        With cmd
            .CommandType = CommandType.StoredProcedure
            .CommandTimeout = 90000

            cn.Open()
            dr = .ExecuteReader(CommandBehavior.CloseConnection)
        End With

        '// Create table to display Nutrients list

        Dim counter As Integer = 0

        With dt.Columns
            .Add("NDB_No")
            .Add("DESC")
            For counter = 1 To 42
                .Add("Val" & counter)
            Next
        End With

        Dim arrNutrNo As New ArrayList(50)    ' Store Nutrients added in the datatable
        Dim sNutr_no As String
        Dim sNutr_desc As String
        Dim sNDB_No As String
        Dim nNutr_Val As Double
        Dim nPosition As Integer
        Dim sFormat As String
        Dim bAddNewRow As Boolean

        While dr.Read
            sNutr_no = CStr(dr.Item("nutr_no"))
            sNutr_desc = CStr(dr.Item("desc"))
            sNDB_No = CStr(dr.Item("NDB_No"))
            nNutr_Val = CDbl(dr.Item("Nutr_Val"))
            nPosition = CInt(dr.Item("Position"))
            sFormat = CStr(dr.Item("Format"))

            If arrNutrNo.Contains(sNutr_desc) Then
                bAddNewRow = False
                row = dt.Rows(arrNutrNo.IndexOf(sNutr_desc))
            Else
                row = dt.NewRow
                bAddNewRow = True
            End If

            row("NDB_No") = sNDB_No
            row("Desc") = sNutr_desc
            row("Val" & nPosition) = nNutr_Val

            If bAddNewRow Then
                dt.Rows.Add(row)
                arrNutrNo.Add(sNutr_desc)
            End If
        End While
        dr.Close()
        Return dt
    End Function

    Public Function GetRawMerchandiseList(intCodeSet As Integer, strFilter As String) As DataTable
        Dim sb As New StringBuilder
        With sb
            .Append("SELECT Egsw_Food_Des.NDB_No, Egsw_Food_Des.[Desc], Egsw_Nut_Data.Nutr_No, Egsw_Nut_Data.Nutr_Val, EgswNutrientDef.Position , EgswNutrientDef.format ")
            .Append("From Egsw_Food_Des ")
            .Append("INNER JOIN Egsw_Nut_Data ON Egsw_Nut_Data.NDB_No = Egsw_Food_Des.NDB_No ")
            '.Append("INNER JOIN EgswNutrientDef ON EgswNutrientDef.position <=34 and CodeSite = " + intCodeSite.ToString() + " ")
            .Append("INNER JOIN EgswNutrientDef ON EgswNutrientDef.position <=34 and CodeSet = " + intCodeSet.ToString() + " ")
            .Append("AND Egsw_Nut_Data.Nutr_No = EgswNutrientDef.Nutr_No ")
            .Append("WHERE REPLACE(Egsw_Food_Des.[Desc],'" + "," + "','" + " " + "') LIKE '" + strFilter + "' ")
            '.Append("ORDER BY Egsw_Food_Des.[Desc], EgswNutrientDef.Position ")
            .Append(" UNION ")
            .Append("SELECT Egsw_Food_Des.NDB_No, Egsw_Food_Des.[Desc], Egsw_Nut_Data.Nutr_No, Egsw_Nut_Data.Nutr_Val, EgswNutrientDef.Position , EgswNutrientDef.format ")
            .Append("From Egsw_Food_Des ")
            .Append("INNER JOIN Egsw_Nut_Data ON Egsw_Nut_Data.NDB_No = Egsw_Food_Des.NDB_No ")
            '.Append("INNER JOIN EgswNutrientDef ON EgswNutrientDef.position <=34 and CodeSite = " + intCodeSite.ToString() + " ")
            .Append("INNER JOIN EgswNutrientDef ON EgswNutrientDef.position <=34 and CodeSet = " + intCodeSet.ToString() + " ")
            .Append("AND Egsw_Nut_Data.Nutr_No = EgswNutrientDef.Nutr_No ")
            .Append("WHERE REPLACE(Egsw_Food_Des.[Desc],'" + "," + "','" + " " + "') LIKE '" + strFilter + "%' ")
            ' .Append("ORDER BY Egsw_Food_Des.[Desc], EgswNutrientDef.Position ")
            .Append(" UNION ")
            .Append("SELECT Egsw_Food_Des.NDB_No, Egsw_Food_Des.[Desc], Egsw_Nut_Data.Nutr_No, Egsw_Nut_Data.Nutr_Val, EgswNutrientDef.Position , EgswNutrientDef.format ")
            .Append("From Egsw_Food_Des ")
            .Append("INNER JOIN Egsw_Nut_Data ON Egsw_Nut_Data.NDB_No = Egsw_Food_Des.NDB_No ")
            '.Append("INNER JOIN EgswNutrientDef ON EgswNutrientDef.position <=34 and CodeSite = " + intCodeSite.ToString() + " ")
            .Append("INNER JOIN EgswNutrientDef ON EgswNutrientDef.position <=34 and CodeSet = " + intCodeSet.ToString() + " ")
            .Append("AND Egsw_Nut_Data.Nutr_No = EgswNutrientDef.Nutr_No ")
            .Append("WHERE REPLACE(Egsw_Food_Des.[Desc],'" + "," + "','" + " " + "') LIKE '%" + strFilter + "%' ")
            '.Append("ORDER BY Egsw_Food_Des.[Desc], EgswNutrientDef.Position ")

        End With

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader
        Dim dt As New DataTable("RawMerchandise")
        Dim row As DataRow
        With cmd
            .Connection = cn
            .CommandText = sb.ToString
            .CommandType = CommandType.Text
            .CommandTimeout = 90000

            cn.Open()
            dr = .ExecuteReader(CommandBehavior.CloseConnection)
        End With

        '// Create table to display Nutrients list

        Dim counter As Integer = 0

        With dt.Columns
            .Add("NDB_No")
            .Add("DESC")
            For counter = 1 To 42
                .Add("Val" & counter)
            Next
        End With

        Dim arrNutrNo As New ArrayList(50)    ' Store Nutrients added in the datatable
        Dim sNutr_no As String
        Dim sNutr_desc As String
        Dim sNDB_No As String
        Dim nNutr_Val As Double
        Dim nPosition As Integer
        Dim sFormat As String
        Dim bAddNewRow As Boolean

        While dr.Read
            sNutr_no = CStr(dr.Item("nutr_no"))
            sNutr_desc = CStr(dr.Item("desc"))
            sNDB_No = CStr(dr.Item("NDB_No"))
            nNutr_Val = CDbl(dr.Item("Nutr_Val"))
            nPosition = CInt(dr.Item("Position"))
            sFormat = CStr(dr.Item("Format"))

            If arrNutrNo.Contains(sNutr_desc) Then
                bAddNewRow = False
                row = dt.Rows(arrNutrNo.IndexOf(sNutr_desc))
            Else
                row = dt.NewRow
                bAddNewRow = True
            End If

            row("NDB_No") = sNDB_No
            row("Desc") = sNutr_desc
            row("Val" & nPosition) = nNutr_Val

            If bAddNewRow Then
                dt.Rows.Add(row)
                arrNutrNo.Add(sNutr_desc)
            End If
        End While
        dr.Close()
        Return dt
    End Function

    Public Function GetRawMerchandiseList(intCodeSet As Integer, intCodeSite As Integer, strSearchString As String, intFilter As clsListe.enumFilter, intCodeTrans As Integer) As DataTable
        'Dim sb As New StringBuilder
        'With sb
        '    .Append("SELECT Egsw_Food_Des.NDB_No, Egsw_Food_Des.[Desc], Egsw_Nut_Data.Nutr_No, Egsw_Nut_Data.Nutr_Val, EgswNutrientDef.Position , EgswNutrientDef.format ")
        '    .Append("From Egsw_Food_Des ")
        '    .Append("INNER JOIN Egsw_Nut_Data ON Egsw_Nut_Data.NDB_No = Egsw_Food_Des.NDB_No ")
        '    '.Append("INNER JOIN EgswNutrientDef ON EgswNutrientDef.position <=34 and CodeSite = " + intCodeSite.ToString() + " ")
        '    .Append("INNER JOIN EgswNutrientDef ON EgswNutrientDef.position <=34 and CodeSet = " + intCodeSet.ToString() + " ")
        '    .Append("AND Egsw_Nut_Data.Nutr_No = EgswNutrientDef.Nutr_No ")
        '    .Append("ORDER BY Egsw_Food_Des.[Desc], EgswNutrientDef.Position ")
        'End With
        Dim dt As New DataTable("RawMerchandise")
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand("GetNutrientDataPerSet", cn)
        Dim da As New SqlDataAdapter
        cmd.CommandType = CommandType.StoredProcedure


        ''@CodeSite
        cmd.Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
        cmd.Parameters.Add("@CodeSet", SqlDbType.Int).Value = intCodeSet
        cmd.Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
        cmd.Parameters.Add("@nvcSearchString", SqlDbType.NVarChar, 170).Value = strSearchString
        cmd.Parameters.Add("@intFilterType", SqlDbType.Int).Value = intFilter


        With da
            .SelectCommand = cmd
            dt.BeginLoadData()
            .Fill(dt)
            dt.EndLoadData()
        End With
        Return dt
    End Function

    Public Function GetListeHistoryLog(ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer, Optional intCodeUser As Integer = -1, _
                                       Optional intFieldCode As Integer = -1, Optional blnFirstFewRecords As Boolean = True, Optional intDateFilter As Integer = 0, _
                                       Optional dteDateFrom As Date = #1/1/1900#, Optional dteDateTo As Date = #1/1/1900#) As DataSet
        Dim ds As New DataSet("HistoryLog")
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand("", cn)
        Dim da As New SqlDataAdapter
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "[sp_EgswListeGetHistoryLogs]"

        ''@CodeSite
        cmd.Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
        cmd.Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
        cmd.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
        cmd.Parameters.Add("@intFieldCode", SqlDbType.Int).Value = intFieldCode
        cmd.Parameters.Add("@bitFirstFewRecords", SqlDbType.Bit).Value = blnFirstFewRecords
        cmd.Parameters.Add("@intDateFilterOption", SqlDbType.Int).Value = intDateFilter
        cmd.Parameters.Add("@dteDateFrom", SqlDbType.DateTime).Value = dteDateFrom
        cmd.Parameters.Add("@dteDateTo", SqlDbType.DateTime).Value = dteDateTo

        With da
            .SelectCommand = cmd
            'dt.BeginLoadData()
            .Fill(ds)
            'dt.EndLoadData()
        End With
        Return ds
    End Function


    Public Function SearchMenuPlanItems(CodeSite As Integer, CodeTrans As Integer, CodeSetPrice As Integer, Page As Integer,
                                        Optional RecsPerPage As Integer = 10,
                                        Optional CodeCategory As Integer? = Nothing,
                                        Optional NameFilter As String = "") As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable
        With cmd
            .Connection = cn
            .CommandText = "MP_SEARCHMenuItems"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@CodeSite", SqlDbType.Int).Value = CodeSite
            .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = CodeTrans
            .Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = CodeSetPrice
            .Parameters.Add("@Page", SqlDbType.Int).Value = Page
            .Parameters.Add("@RecsPerPage", SqlDbType.Int).Value = RecsPerPage
            .Parameters.Add("@CodeCategory", SqlDbType.Int).Value = CodeCategory
            .Parameters.Add("@Name", SqlDbType.NVarChar, 1000).Value = IIf(NameFilter = "", Nothing, NameFilter)

        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function SearchMenuPlanItems2(CodeSite As Integer, CodeTrans As Integer, CodeSetPrice As Integer, Page As Integer,
                                        Optional RecsPerPage As Integer = 10,
                                        Optional Type As Integer? = Nothing,
                                        Optional CodeCategory As Integer? = Nothing,
                                        Optional CodeSource As Integer? = Nothing,
                                        Optional CodeCookbook As Integer? = Nothing,
                                        Optional NameFilter As String = "") As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable
        With cmd
            .Connection = cn
            .CommandText = "MP_SEARCHMenuItems"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@CodeSite", SqlDbType.Int).Value = CodeSite
            .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = CodeTrans
            .Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = CodeSetPrice

            .Parameters.Add("@Type", SqlDbType.Int).Value = Type
            .Parameters.Add("@Name", SqlDbType.NVarChar, 1000).Value = IIf(NameFilter = "", Nothing, NameFilter)
            .Parameters.Add("@CodeCategory", SqlDbType.Int).Value = CodeCategory
            .Parameters.Add("@CodeSource", SqlDbType.Int).Value = CodeSource
            .Parameters.Add("@CodeCookbook", SqlDbType.Int).Value = CodeCookbook

            .Parameters.Add("@Page", SqlDbType.Int).Value = Page
            .Parameters.Add("@RecsPerPage", SqlDbType.Int).Value = RecsPerPage
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function SearchMenuPlanItems3(CodeSite As Integer, CodeTrans As Integer, CodeSetPrice As Integer, Page As Integer,
                                       Optional RecsPerPage As Integer = 10,
                                       Optional Type As Integer? = Nothing,
                                       Optional CodeCategory As Integer? = Nothing,
                                       Optional CodeSource As Integer? = Nothing,
                                       Optional CodeCookbook As Integer? = Nothing,
                                       Optional NameFilter As String = "") As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable
        With cmd
            .Connection = cn
            .CommandText = "MP_SEARCHMenuItemsLocalMasterPlan"
            .CommandType = CommandType.StoredProcedure
            .Parameters.Add("@CodeSite", SqlDbType.Int).Value = CodeSite
            .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = CodeTrans
            .Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = CodeSetPrice

            .Parameters.Add("@Type", SqlDbType.Int).Value = Type
            .Parameters.Add("@Name", SqlDbType.NVarChar, 1000).Value = IIf(NameFilter = "", Nothing, NameFilter)
            .Parameters.Add("@CodeCategory", SqlDbType.Int).Value = CodeCategory
            .Parameters.Add("@CodeSource", SqlDbType.Int).Value = CodeSource
            .Parameters.Add("@CodeCookbook", SqlDbType.Int).Value = CodeCookbook

            .Parameters.Add("@Page", SqlDbType.Int).Value = Page
            .Parameters.Add("@RecsPerPage", SqlDbType.Int).Value = RecsPerPage
        End With
        Try
            Return ExecuteFetchType(L_bytFetchType, cmd)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    'NBG 20161103
    Public Function CheckIfGlobal(ByVal intCodeListeOld As Integer, ByVal intCodeListeNew As Integer) As Integer
        Dim sqlCmd As SqlCommand = New SqlCommand
        Dim flagX As Integer = 0

        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "sp_EgswListeUpdateReplaceIngredientCheck"
                .Parameters.Add("@intCodeListeOld", SqlDbType.Int).Value = intCodeListeOld
                .Parameters.Add("@intCodeListeNew", SqlDbType.Int).Value = intCodeListeNew
                flagX = .ExecuteScalar()
                sqlCmd.Connection.Close()
                sqlCmd.Connection.Dispose()
                sqlCmd.Dispose()
                Return flagX
            End With
        Catch ex As Exception
            flagX = 0
            Return flagX
        End Try
    End Function

    'NBG 20170306
    Public Function GetMenuPlanName(ByVal intCodeListe As Integer) As DataTable
        Dim sqlDta As New SqlDataAdapter
        Dim dt As New DataTable
        Dim sqlCmd As SqlCommand = New SqlCommand
        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.Text
                .CommandText = "select top 1 mp.Name as SiteName from EgswMPDetailsData DD " & _
                            "inner join EgswMPDetails D on DD.IDDetails = d.ID " & _
                            "inner join EgswMPMain M on D.IDMain = M.ID " & _
                            "inner join EgswMPMenuPlan MP on mp.code = M.CodeMenuPlan " & _
                            "where CodeListe = @intCodeListe"
                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe

                sqlDta.SelectCommand = sqlCmd
                dt.BeginLoadData()
                sqlDta.Fill(dt)
                dt.EndLoadData()

                sqlCmd.Connection.Close() 'DLS 31.05.2007
                sqlCmd.Dispose()
                sqlDta.Dispose()
                Return dt
            End With
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Public Function ListeLockUpdate(nCode As Integer, blnVal As Boolean, nCodetrans As Integer) As enumEgswErrorCode 'DRR 02.12.2013
        Dim arrParam(3) As SqlParameter

        arrParam(0) = New SqlParameter("@intCode", SqlDbType.Int)
        arrParam(1) = New SqlParameter("@Value", SqlDbType.Bit)
        arrParam(2) = New SqlParameter("@retVal", SqlDbType.Int)

        arrParam(0).Value = nCode
        arrParam(1).Value = blnVal
        arrParam(2).Direction = ParameterDirection.ReturnValue

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswListeLockUpdate", arrParam)
            Return CType(arrParam(2).Value, enumEgswErrorCode) 'enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
            Throw ex
        End Try
    End Function

    Public Function GetYieldRatio(ByVal intCodeListe As Integer, ByRef qty As Double, ByRef UnitId As Integer) As Double
        Try
            Dim sqlCmd As SqlCommand = New SqlCommand
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandType = CommandType.Text
                .Parameters.Add("@totalqty", SqlDbType.Float)
                .Parameters("@totalqty").Direction = ParameterDirection.Output
                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@Qty", SqlDbType.Float).Value = qty
                .Parameters.Add("@Unit", SqlDbType.Int).Value = UnitId

                .CommandText = "SET @totalqty=dbo.fn_ConvertSubRecipeQtyToYield(@CodeListe,@Qty,@Unit)"
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

                GetYieldRatio = .Parameters("@totalqty").Value
            End With
        Catch ex As Exception
            Return False
        End Try
    End Function

End Class

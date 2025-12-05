Imports System.Data
Imports System.Text
Imports System.Data.SqlClient
Imports EgswKey

Imports Aspose.Words
Imports Aspose.Words.Tables
Imports Aspose.Words.Drawing

Public Class clsExporttoWord
    Private L_strCnn As String
    Private m_RecipeId As Integer
    Private m_Version As Integer
    Private L_sFolder As String
    Private L_strHostName As String
    Private L_strPort As String
    Private L_sURLFragment As String
    Private L_strImageRecipe As String
    Private L_blPrepAutoSpacing As Boolean
    Public Sub New(ByVal strCnn As String, ByVal strPicFolder As String, ByVal strPicHost As String, ByVal strPort As String, ByVal sURLFragment As String)
        L_strCnn = strCnn
        L_sFolder = strPicFolder
        L_strHostName = "http://" & strPicHost
        L_strPort = strPort
    End Sub

    'Render webcontrol to html


    Private Function GetRecipeDetails(ByVal intId As Integer, ByVal intVersion As Integer, ByVal intCodeTrans As Integer, ByVal intCookmode As Integer, intCodeSet As Integer, Optional ByVal blnMetImp As Boolean = True, Optional intCodeSite As Integer = 0, Optional intCodeUser As Integer = 0) As DataSet
        Dim da As New SqlDataAdapter
        Dim ds As New DataSet

        Try
            Dim cmdX As New SqlCommand("GetRecipeForDisplay", New SqlConnection(L_strCnn))

            With cmdX
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@RecipeID", SqlDbType.Int).Value = intId
                .Parameters.Add("@Version", SqlDbType.Int).Value = intVersion
                .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@CookMode", SqlDbType.Int).Value = intCookmode
                .Parameters.Add("@CodeNutrientSet", SqlDbType.Int).Value = intCodeSet
                .Parameters.Add("@bitMetImp", SqlDbType.Bit).Value = blnMetImp 'JTOC 12.02.2013
                .Parameters.Add("@CodeSite", SqlDbType.Bit).Value = intCodeSite 'AGL 2013.06.25
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser 'JOP 04.11.2017
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
            End With

            da.SelectCommand = cmdX
            da.Fill(ds)
            cmdX.Dispose()
            Return ds
        Catch ex As Exception
            Return Nothing
        End Try

    End Function


    Private Function GetMerchandiseDetails(ByVal intId As Integer, ByVal intVersion As Integer, ByVal intCodeTrans As Integer, ByVal intCookmode As Integer, intCodeSet As Integer) As DataSet
        Dim da As New SqlDataAdapter
        Dim ds As New DataSet

        Try
            Dim cmdX As New SqlCommand("GetMerchandiseForDisplay", New SqlConnection(L_strCnn))

            With cmdX
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@RecipeID", SqlDbType.Int).Value = intId
                .Parameters.Add("@Version", SqlDbType.Int).Value = intVersion
                .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@CookMode", SqlDbType.Int).Value = intCookmode
                .Parameters.Add("@CodeNutrientSet", SqlDbType.Int).Value = intCodeSet
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
            End With

            da.SelectCommand = cmdX
            da.Fill(ds)
            cmdX.Dispose()
            Return ds
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Public Function GetListeSite(ByVal intCodeListe As Integer)
        Dim strSQL As String
        Dim con As New SqlConnection(L_strCnn)
        Dim dr As SqlDataReader
        Dim intCodeSite As Integer
        con.Open()
        strSQL = "SELECT CodeSite FROM EgswListe WHERE Code =" & intCodeListe
        Dim cmdX As New SqlCommand(strSQL, con)
        dr = cmdX.ExecuteReader
        If dr.HasRows Then
            dr.Read()

            'JTOC 15.10.2012 Added condition for null codesite
            If Not IsDBNull(dr("CodeSite")) Then
                intCodeSite = dr("CodeSite")
            Else
                intCodeSite = 0
            End If

        Else
            intCodeSite = 0
        End If
        dr.Close() 'AGL 2013.06.18
        con.Close()
        Return intCodeSite
    End Function

    Public Function GetListeStandard(ByVal intStandardListe As Integer)
        Dim strSQL As String
        Dim con As New SqlConnection(L_strCnn)
        Dim dr As SqlDataReader
        Dim intStandard As Integer
        con.Open()
        strSQL = "SELECT Standard FROM EgswListe WHERE Code =" & intStandardListe
        Dim cmdX As New SqlCommand(strSQL, con)
        dr = cmdX.ExecuteReader
        If dr.HasRows Then
            dr.Read()
            intStandard = CIntDB(dr("Standard"))
        Else
            intStandard = 0
        End If
        dr.Close()
        con.Close()
        Return intStandard
    End Function
    Public Function bln2PicturesForGoldOnly() As Boolean
        Dim strSQL As String
        Dim con As New SqlConnection(L_strCnn)
        Dim dr As SqlDataReader
        Dim str As String
        Dim blForGold As Boolean
        con.Open()
        strSQL = "SELECT String FROM EgswConfig WHERE Numero = '20268'"
        Dim cmdX As New SqlCommand(strSQL, con)
        dr = cmdX.ExecuteReader
        If dr.HasRows Then
            dr.Read()
            str = dr("String")
            If str = "!B=1" Then
                blForGold = True
            Else
                blForGold = False
            End If
        Else
            blForGold = False
        End If
        dr.Close()
        con.Close()
        Return blForGold
    End Function

    Public Sub GetRecipeCode(ByVal intId As Integer, ByRef intRecipeID As Integer, ByRef intVersion As Integer)
        Dim strSQL As String
        Dim con As New SqlConnection(L_strCnn)
        Dim dr As SqlDataReader
        con.Open()
        strSQL = "SELECT CASE WHEN Parent IS NULL THEN Code ELSE Parent END AS RecipeID, ISNULL(Version, 0) Version FROM EgswListe WHERE Code=" & intId 'AGL Merging 2012.09.04
        Dim cmdX As New SqlCommand(strSQL, con)
        dr = cmdX.ExecuteReader
        If dr.HasRows Then
            dr.Read()
            intRecipeID = dr("RecipeID")
            intVersion = dr("Version")
        Else
            intRecipeID = 0
            intVersion = 0
        End If
        con.Close()
    End Sub


    Public Function GetRecipeDefaultPicture(ByVal intId As Integer) As Integer
        Dim intDefault As Integer
        Dim strSQL As String
        Dim con As New SqlConnection(L_strCnn)
        Dim dr As SqlDataReader
        con.Open()
        strSQL = "SELECT CASE WHEN defaultpicture IS NULL THEN 0 ELSE defaultpicture END AS DefaultPictureNo FROM EgswListe WHERE Code=" & intId
        Dim cmdX As New SqlCommand(strSQL, con)
        dr = cmdX.ExecuteReader
        If dr.HasRows Then
            dr.Read()
            intDefault = dr("DefaultPictureNo")
        Else
            intDefault = 0
        End If
        con.Close()
        Return intDefault
    End Function

    Public Property AutoSpacing() As Boolean
        Set(value As Boolean)
            L_blPrepAutoSpacing = value
        End Set
        Get
            Return L_blPrepAutoSpacing
        End Get
    End Property

    '    Public Function ExportToWordOld(ByVal intListeID As Integer, ByVal bitFormat As Byte, ByRef strErr As String, ByRef strFilename As String) As StringBuilder
    '        Dim dsRecipeDetails As DataSet
    '        Dim strPictureName As String
    '        Dim isDisplay As Boolean = False

    '        Dim strHTMLContent As StringBuilder = New StringBuilder()

    '        Dim lblRecipeID As String = ""
    '        Dim strRecipeID As String = ""
    '        Dim lblRecipeNumber As String = ""
    '        Dim strRecipeNumber As String = ""
    '        Dim lblSubTitle As String = ""
    '        Dim strSubTitle As String = ""
    '        Dim imgRecipe As String = ""
    '        Dim strRecipeName As String = ""
    '        Dim strSubHeading As String = ""
    '        Dim strServings As String = ""
    '        Dim strYield As String = ""
    '        Dim strServingsUnit As String = "" 'CMV 050211
    '        Dim strRecipeTime As String = ""
    '        Dim strMethodHeader As String = ""
    '        Dim strIngredients As String = ""
    '        Dim dblQty As Double
    '        Dim strUOM As String = ""
    '        Dim strDirections As String = ""
    '        Dim strFootNote1 As String = ""
    '        Dim strFootNote2 As String = ""
    '        Dim lblCostPerRecipe As String = ""
    '        Dim strCostPerRecipe As String = ""
    '        Dim lblCostPerServings As String = ""
    '        Dim strCostPerServings As String = ""
    '        Dim strCurrency As String = ""
    '        Dim lblInformation As String = ""
    '        Dim lblRecipeStatus As String = ""
    '        Dim strRecipeStatus As String = ""
    '        Dim lblWebStatus As String = ""
    '        Dim strWebStatus As String = ""
    '        Dim lblDateCreated As String = ""
    '        Dim strDateCreated As String = ""
    '        Dim lblDateLastModified As String = ""
    '        Dim strDateLastModified As String = ""
    '        Dim lblLastTested As String = ""
    '        Dim strLastTested As String = ""
    '        Dim lblDateDeveloped As String = ""
    '        Dim strDateDeveloped As String = ""
    '        Dim lblDateOfFinalEdit As String = ""
    '        Dim strDateOfFinalEdit As String = ""
    '        Dim lblDevelopmentPurpose As String = ""
    '        Dim strDevelopmentPurpose As String = ""
    '        Dim lblUpdatedBy As String = ""
    '        Dim strUpdatedBy As String = ""
    '        Dim lblCreatedBy As String = ""
    '        Dim strCreatedBy As String = ""
    '        Dim lblModifiedBy As String = ""
    '        Dim strModifiedBy As String = ""
    '        Dim lblTestedBy As String = ""
    '        Dim strTestedBy As String = ""
    '        Dim lblDevelopedBy As String = ""
    '        Dim strDevelopedBy As String = ""
    '        Dim lblFinalEditBy As String = ""
    '        Dim strFinalEditBy As String = ""
    '        Dim lblComments As String = ""
    '        Dim strSubmitDate As String = ""
    '        Dim strOwnerName As String = ""
    '        Dim strComments As String = ""
    '        Dim lblAttributes As String = ""
    '        Dim strAttributes As String = ""
    '        Dim strParents As String = ""
    '        Dim intAttributesCode As Integer
    '        Dim intAttributesParent As Integer
    '        Dim intAttributesMain As Integer
    '        Dim lblRecipeBrand As String = ""
    '        Dim strRecipeBrand As String = ""
    '        Dim strRecipeBrandClassification As String = ""
    '        Dim lblPlacements As String = ""
    '        Dim strPlacementName As String = ""
    '        Dim strPlacementDate As String = ""
    '        Dim strPlacementDescription As String = ""
    '        Dim lblNutritionalInformation As String = ""
    '        Dim lblCalories As String = ""
    '        Dim lblCaloriesFromFat As String = ""
    '        Dim lblSatFat As String = ""
    '        Dim lblTransFat As String = ""
    '        Dim lblMonoSatFat As String = ""
    '        Dim lblPolyFat As String = ""
    '        Dim lblTotalFat As String = ""
    '        Dim lblCholesterol As String = ""
    '        Dim lblSodium As String = ""
    '        Dim lblTotalCarbohydrates As String = ""
    '        Dim lblSugars As String = ""
    '        Dim lblDietaryFiber As String = ""
    '        Dim lblNetCarbohydrates As String = ""
    '        Dim lblProtein As String = ""
    '        Dim lblVitaminA As String = ""
    '        Dim lblVitaminC As String = ""
    '        Dim lblCalcium As String = ""
    '        Dim lblIron As String = ""
    '        Dim lblMonoUnsaturated As String = ""
    '        Dim lblPolyUnsaturated As String = ""
    '        Dim lblPotassium As String = ""
    '        Dim lblVitaminD As String = ""
    '        Dim lblVitaminE As String = ""
    '        Dim lblOmega3 As String = ""
    '        Dim strCalories As String = ""
    '        Dim strCaloriesFromFat As String = ""
    '        Dim strSatFat As String = ""
    '        Dim strTransFat As String = ""
    '        Dim strMonoSatFat As String = ""
    '        Dim strPolyFat As String = ""
    '        Dim strTotalFat As String = ""
    '        Dim strCholesterol As String = ""
    '        Dim strSodium As String = ""
    '        Dim strTotalCarbohydrates As String = ""
    '        Dim strSugars As String = ""
    '        Dim strDietaryFiber As String = ""
    '        Dim strNetCarbohydrates As String = ""
    '        Dim lblNetCarbs As String = ""
    '        Dim strProtein As String = ""
    '        Dim strVitaminA As String = ""
    '        Dim strVitaminC As String = ""
    '        Dim strCalcium As String = ""
    '        Dim strIron As String = ""
    '        Dim strMonoUnsaturated As String = ""
    '        Dim strPolyUnsaturated As String = ""
    '        Dim strPotassium As String = ""
    '        Dim strVitaminD As String = ""
    '        Dim strVitaminE As String = ""
    '        Dim strOmega3 As String = ""
    '        Dim strUnitCalories As String = ""
    '        Dim strUnitCaloriesFromFat As String = ""
    '        Dim strUnitSatFat As String = ""
    '        Dim strUnitTransFat As String = ""
    '        Dim strUnitMonoSatFat As String = ""
    '        Dim strUnitPolyFat As String = ""
    '        Dim strUnitTotalFat As String = ""
    '        Dim strUnitCholesterol As String = ""
    '        Dim strUnitSodium As String = ""
    '        Dim strUnitTotalCarbohydrates As String = ""
    '        Dim strUnitSugars As String = ""
    '        Dim strUnitDietaryFiber As String = ""
    '        Dim strUnitNetCarbohydrates As String = ""
    '        Dim strUnitProtein As String = ""
    '        Dim strUnitVitaminA As String = ""
    '        Dim strUnitVitaminC As String = ""
    '        Dim strUnitCalcium As String = ""
    '        Dim strUnitIron As String = ""
    '        Dim strUnitMonoUnsaturated As String = ""
    '        Dim strUnitPolyUnsaturated As String = ""
    '        Dim strUnitPotassium As String = ""
    '        Dim strUnitVitaminD As String = ""
    '        Dim strUnitVitaminE As String = ""
    '        Dim strUnitOmega3 As String = ""
    '        Dim strFormatCalories As String = ""
    '        Dim strFormatCaloriesFromFat As String = ""
    '        Dim strFormatSatFat As String = ""
    '        Dim strFormatTransFat As String = ""
    '        Dim strFormatMonoSatFat As String = ""
    '        Dim strFormatPolyFat As String = ""
    '        Dim strFormatTotalFat As String = ""
    '        Dim strFormatCholesterol As String = ""
    '        Dim strFormatSodium As String = ""
    '        Dim strFormatTotalCarbohydrates As String = ""
    '        Dim strFormatSugars As String = ""
    '        Dim strFormatDietaryFiber As String = ""
    '        Dim strFormatNetCarbohydrates As String = ""
    '        Dim strFormatProtein As String = ""
    '        Dim strFormatVitaminA As String = ""
    '        Dim strFormatVitaminC As String = ""
    '        Dim strFormatCalcium As String = ""
    '        Dim strFormatIron As String = ""
    '        Dim strFormatMonoUnsaturated As String = ""
    '        Dim strFormatPolyUnsaturated As String = ""
    '        Dim strFormatPotassium As String = ""
    '        Dim strFormatVitaminD As String = ""
    '        Dim strFormatVitaminE As String = ""
    '        Dim strFormatOmega3 As String = ""
    '        Dim strNutrients As String = ""

    '        GetRecipeCode(intListeID, m_RecipeId, m_Version)

    '        dsRecipeDetails = GetRecipeDetails(m_RecipeId, m_Version, 1, 0, 0)

    '        'TRANSLATION OF LABELS
    '        lblRecipeID = "Recipe ID"
    '        lblRecipeNumber = "Recipe Number"
    '        lblSubTitle = "Sub Title"
    '        lblCostPerRecipe = "Cost Per Recipe"
    '        lblCostPerServings = "Cost Per Serving"
    '        lblInformation = "Information"
    '        lblRecipeStatus = "Recipe Status:"
    '        lblUpdatedBy = "Updated By:"
    '        lblWebStatus = "Web Status:"
    '        lblDateCreated = "Date Created:"
    '        lblCreatedBy = "Created By:"
    '        lblDateLastModified = "Date Last Modified:"
    '        lblModifiedBy = "Modified By:"
    '        lblLastTested = "Last Tested:"
    '        lblTestedBy = "Tested By:"
    '        lblDateDeveloped = "Date Developed:"
    '        lblDevelopedBy = "Developed By:"
    '        lblDateOfFinalEdit = "Date of Final Edit:"
    '        lblFinalEditBy = "Final Edit By:"
    '        lblDevelopmentPurpose = "Development Purpose:"
    '        lblComments = "Comments"
    '        lblAttributes = "Attributes"
    '        lblRecipeBrand = "Brands"
    '        lblPlacements = "Placements"
    '        lblNutritionalInformation = "Nutritional Information per serving:"
    '        lblCalories = "Calories"
    '        lblCaloriesFromFat = "Calories from Fat"
    '        lblSatFat = "Sat Fat"
    '        lblTransFat = "Trans Fat"
    '        lblMonoSatFat = "Mono Sat Fat"
    '        lblPolyFat = "Poly Sat Fat"
    '        lblTotalFat = "Total Fat"
    '        lblCholesterol = "Cholesterol"
    '        lblSodium = "Sodium"
    '        lblTotalCarbohydrates = "Total Carbohydrates"
    '        lblSugars = "Sugars"
    '        lblDietaryFiber = "Dietary Fiber"
    '        lblNetCarbohydrates = "Net Carbohydrates"
    '        lblProtein = "Protein"
    '        lblVitaminA = "Vitamin A"
    '        lblVitaminC = "Vitamin C"
    '        lblCalcium = "Calcium"
    '        lblIron = "Iron"
    '        lblMonoUnsaturated = "Mono Unsaturated"
    '        lblPolyUnsaturated = "Poly Unsaturated"
    '        lblPotassium = "Potassium"
    '        lblVitaminD = "Vitamin D"
    '        lblVitaminE = "Vitamin E"
    '        lblNetCarbs = "* " & """Net Carbs""" & " are total carbohydrates minus dietary fiber and sugar alcohol as these have a minimal impact on blood sugar."
    '        'lblOmega3 = "Omega3"

    '        If dsRecipeDetails.Tables("Table").Rows.Count > 0 Then


    '            'SET VALUES
    '            strRecipeID = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("RecipeID"))
    '            strRecipeNumber = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("Number"))
    '            strSubTitle = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("SubTitle"))
    '            strRecipeName = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("Name"))
    '            strSubHeading = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("SubHeading"))

    '            strPictureName = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("PictureName"))

    '            'imgRecipe = "http://www.eg-software.com/Client/sesame%20brocolli%20salad.jpg"
    '            'imgRecipe = GetPicturePath(intListeID, strPictureName)

    '            If fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("Yield")) = "" Or _
    '                fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("Yield")) = "0" Then
    '                strYield = ""
    '            Else
    '                strYield = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("Yield"))
    '            End If
    '            strServingsUnit = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("ServingsUnit")) 'CMV 050211
    '            strServings = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("Servings")) & " " & strServingsUnit & " " & strYield 'CMV 050211

    '            strMethodHeader = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("MethodHeader"))
    '            strDirections = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("Note"))
    '            strFootNote1 = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("FootNote1"))
    '            strFootNote2 = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("FootNote2"))
    '            strCurrency = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("Currency"))
    '            'strCostPerRecipe = strCurrency & " " & fctCheckDbNullNumeric(dsRecipeDetails.Tables("Table").Rows(0).Item("CostPrice"))
    '            'strCostPerServings = strCurrency & " " & fctCheckDbNullNumeric(dsRecipeDetails.Tables("Table").Rows(0).Item("CostPricePerServing"))
    '            strRecipeStatus = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("RecipeStatusName"))
    '            strUpdatedBy = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("UpdatedBy"))
    '            strWebStatus = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("WebStatusName"))
    '            If Not IsDBNull(dsRecipeDetails.Tables("Table").Rows(0).Item("DateCreated")) Then strDateCreated = CDate(dsRecipeDetails.Tables("Table").Rows(0).Item("DateCreated")).ToString("MM/dd/yyyy")
    '            strCreatedBy = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("CreatedBy"))
    '            If Not IsDBNull(dsRecipeDetails.Tables("Table").Rows(0).Item("DateLastModified")) Then strDateLastModified = CDate(dsRecipeDetails.Tables("Table").Rows(0).Item("DateLastModified")).ToString("MM/dd/yyyy")
    '            strModifiedBy = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("ModifiedBy"))
    '            If Not IsDBNull(dsRecipeDetails.Tables("Table").Rows(0).Item("DateTested")) Then strLastTested = CDate(dsRecipeDetails.Tables("Table").Rows(0).Item("DateTested")).ToString("MM/dd/yyyy")
    '            strTestedBy = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("TestedBy"))
    '            If Not IsDBNull(dsRecipeDetails.Tables("Table").Rows(0).Item("DateDeveloped")) Then strDateDeveloped = CDate(dsRecipeDetails.Tables("Table").Rows(0).Item("DateDeveloped")).ToString("MM/dd/yyyy")
    '            strDevelopedBy = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("DevelopedBy"))
    '            If Not IsDBNull(dsRecipeDetails.Tables("Table").Rows(0).Item("DateFinalEdit")) Then strDateOfFinalEdit = CDate(dsRecipeDetails.Tables("Table").Rows(0).Item("DateFinalEdit")).ToString("MM/dd/yyyy")
    '            strFinalEditBy = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("FinalEditBy"))
    '            strDevelopmentPurpose = fctCheckDbNull(dsRecipeDetails.Tables("Table").Rows(0).Item("DevelopmentPurpose"))

    '            isDisplay = dsRecipeDetails.Tables("Table").Rows(0).Item("DisplayNutrition") ' JBB 07.22.2011

    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayCalories") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Calories")) = "" Then
    '                strUnitCalories = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_Calories"))
    '                strFormatCalories = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("CaloriesFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("Calories").ToString.Contains(".") Then
    '                    strCalories = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("Calories"), strFormatCalories)
    '                Else
    '                    strCalories = dsRecipeDetails.Tables("Table3").Rows(0).Item("Calories")
    '                End If
    '                strNutrients = strNutrients & lblCalories & " " & strCalories & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayCaloriesFromFat") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("CaloriesFromFat")) = "" Then
    '                strUnitCaloriesFromFat = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_CaloriesFromFat"))
    '                strFormatCaloriesFromFat = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("CaloriesFromFatFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("CaloriesFromFat").ToString.Contains(".") Then
    '                    strCaloriesFromFat = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("CaloriesFromFat"), strFormatCaloriesFromFat)
    '                Else
    '                    strCaloriesFromFat = dsRecipeDetails.Tables("Table3").Rows(0).Item("CaloriesFromFat")
    '                End If
    '                strNutrients = strNutrients & lblCaloriesFromFat & " " & strCaloriesFromFat & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplaySatFat") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("SatFat")) = "" Then
    '                strUnitSatFat = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_SatFat"))
    '                strFormatSatFat = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("SatFatFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("SatFat").ToString.Contains(".") Then
    '                    strSatFat = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("SatFat"), strFormatSatFat) & strUnitSatFat
    '                Else
    '                    strSatFat = dsRecipeDetails.Tables("Table3").Rows(0).Item("SatFat") & strUnitSatFat
    '                End If
    '                strNutrients = strNutrients & lblSatFat & " " & strSatFat & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayTransFat") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("TransFat")) = "" Then
    '                strUnitTransFat = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_TransFat"))
    '                strFormatTransFat = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("TransFatFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("TransFat").ToString.Contains(".") Then
    '                    strTransFat = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("TransFat"), strFormatTransFat) & strUnitTransFat
    '                Else
    '                    strTransFat = dsRecipeDetails.Tables("Table3").Rows(0).Item("TransFat") & strUnitTransFat
    '                End If
    '                strNutrients = strNutrients & lblTransFat & " " & strTransFat & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayMonoSatFat") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("MonoSatFat")) = "" Then
    '                strUnitMonoSatFat = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_MonoSatFat"))
    '                strFormatMonoSatFat = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("MonoSatFatFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("MonoSatFat").ToString.Contains(".") Then
    '                    strMonoSatFat = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("MonoSatFat"), strFormatMonoSatFat) & strUnitMonoSatFat
    '                Else
    '                    strMonoSatFat = dsRecipeDetails.Tables("Table3").Rows(0).Item("MonoSatFat") & strUnitMonoSatFat
    '                End If
    '                strNutrients = strNutrients & lblMonoSatFat & " " & strMonoSatFat & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayPolySatFat") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("PolySatFat")) = "" Then
    '                strUnitPolyFat = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_PolySatFat"))
    '                strFormatPolyFat = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("PolySatFatFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("PolySatFat").ToString.Contains(".") Then
    '                    strPolyFat = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("PolySatFat"), strFormatPolyFat) & strUnitPolyFat
    '                Else
    '                    strPolyFat = dsRecipeDetails.Tables("Table3").Rows(0).Item("PolySatFat") & strUnitPolyFat
    '                End If
    '                strNutrients = strNutrients & lblPolyFat & " " & strPolyFat & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayTotalFat") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("TotalFat")) = "" Then
    '                strUnitTotalFat = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_TotalFat"))
    '                strFormatTotalFat = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("TotalFatFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("TotalFat").ToString.Contains(".") Then
    '                    strTotalFat = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("TotalFat"), strFormatTotalFat) & strUnitTotalFat
    '                Else
    '                    strTotalFat = dsRecipeDetails.Tables("Table3").Rows(0).Item("TotalFat") & strUnitTotalFat
    '                End If
    '                strNutrients = strNutrients & lblTotalFat & " " & strTotalFat & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayCholesterol") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Cholesterol")) = "" Then
    '                strUnitCholesterol = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_Cholesterol"))
    '                strFormatCholesterol = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("CholesterolFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("Cholesterol").ToString.Contains(".") Then
    '                    strCholesterol = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("Cholesterol"), strFormatCholesterol) & strUnitCholesterol
    '                Else
    '                    strCholesterol = dsRecipeDetails.Tables("Table3").Rows(0).Item("Cholesterol") & strUnitCholesterol
    '                End If
    '                strNutrients = strNutrients & lblCholesterol & " " & strCholesterol & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplaySodium") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Sodium")) = "" Then
    '                strUnitSodium = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_Sodium"))
    '                strFormatSodium = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("SodiumFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("Sodium").ToString.Contains(".") Then
    '                    strSodium = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("Sodium"), strFormatSodium) & strUnitSodium
    '                Else
    '                    strSodium = dsRecipeDetails.Tables("Table3").Rows(0).Item("Sodium") & strUnitSodium
    '                End If
    '                strNutrients = strNutrients & lblSodium & " " & strSodium & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayTotalCarbohydrates") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("TotalCarbohydrates")) = "" Then
    '                strUnitTotalCarbohydrates = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_TotalCarbohydrates"))
    '                strFormatTotalCarbohydrates = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("TotalCarbohydratesFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("TotalCarbohydrates").ToString.Contains(".") Then
    '                    strTotalCarbohydrates = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("TotalCarbohydrates"), strFormatTotalCarbohydrates) & strUnitTotalCarbohydrates
    '                Else
    '                    strTotalCarbohydrates = dsRecipeDetails.Tables("Table3").Rows(0).Item("TotalCarbohydrates") & strUnitTotalCarbohydrates
    '                End If
    '                strNutrients = strNutrients & lblTotalCarbohydrates & " " & strTotalCarbohydrates & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplaySugars") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Sugars")) = "" Then
    '                strUnitSugars = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_Sugars"))
    '                strFormatSugars = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("SugarsFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("Sugars").ToString.Contains(".") Then
    '                    strSugars = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("Sugars"), strFormatSugars) & strUnitSugars
    '                Else
    '                    strSugars = dsRecipeDetails.Tables("Table3").Rows(0).Item("Sugars") & strUnitSugars
    '                End If
    '                strNutrients = strNutrients & lblSugars & " " & strSugars & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayDietaryFiber") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("DietaryFiber")) = "" Then
    '                strUnitDietaryFiber = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_DietaryFiber"))
    '                strFormatDietaryFiber = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("DietaryFiberFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("DietaryFiber").ToString.Contains(".") Then
    '                    strDietaryFiber = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("DietaryFiber"), strFormatDietaryFiber) & strUnitDietaryFiber
    '                Else
    '                    strDietaryFiber = dsRecipeDetails.Tables("Table3").Rows(0).Item("DietaryFiber") & strUnitDietaryFiber
    '                End If
    '                strNutrients = strNutrients & lblDietaryFiber & " " & strDietaryFiber & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayNetCarbohydrates") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("NetCarbohydrates")) = "" Then
    '                strUnitNetCarbohydrates = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_NetCarbohydrates"))
    '                strFormatNetCarbohydrates = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("NetCarbohydratesFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("NetCarbohydrates").ToString.Contains(".") Then
    '                    strNetCarbohydrates = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("NetCarbohydrates"), strFormatNetCarbohydrates) & strUnitNetCarbohydrates
    '                Else
    '                    strNetCarbohydrates = dsRecipeDetails.Tables("Table3").Rows(0).Item("NetCarbohydrates") & strUnitNetCarbohydrates
    '                End If
    '                strNetCarbohydrates = lblNetCarbohydrates & "* " & strNetCarbohydrates
    '                'strNutrients = strNutrients & lblNetCarbohydrates & " " & strNetCarbohydrates & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayProtein") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Protein")) = "" Then
    '                strUnitProtein = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_Protein"))
    '                strFormatProtein = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("ProteinFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("Protein").ToString.Contains(".") Then
    '                    strProtein = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("Protein"), strFormatProtein) & strUnitProtein
    '                Else
    '                    strProtein = dsRecipeDetails.Tables("Table3").Rows(0).Item("Protein") & strUnitProtein
    '                End If
    '                strNutrients = strNutrients & lblProtein & " " & strProtein & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayVitaminA") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminA")) = "" Then
    '                strUnitVitaminA = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_VitaminA"))
    '                strFormatVitaminA = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminAFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminA").ToString.Contains(".") Then
    '                    strVitaminA = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminA"), strFormatVitaminA) & strUnitVitaminA
    '                Else
    '                    strVitaminA = dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminA") & strUnitVitaminA
    '                End If
    '                strNutrients = strNutrients & lblVitaminA & " " & strVitaminA & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayVitaminC") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminC")) = "" Then
    '                strUnitVitaminC = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_VitaminC"))
    '                strFormatVitaminC = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminCFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminC").ToString.Contains(".") Then
    '                    strVitaminC = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminC"), strFormatVitaminC) & strUnitVitaminC
    '                Else
    '                    strVitaminC = dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminC") & strUnitVitaminC
    '                End If
    '                strNutrients = strNutrients & lblVitaminC & " " & strVitaminC & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayCalcium") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Calcium")) = "" Then
    '                strUnitCalcium = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_Calcium"))
    '                strFormatCalcium = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("CalciumFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("Calcium").ToString.Contains(".") Then
    '                    strCalcium = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("Calcium"), strFormatCalcium) & strUnitCalcium
    '                Else
    '                    strCalcium = dsRecipeDetails.Tables("Table3").Rows(0).Item("Calcium") & strUnitCalcium
    '                End If
    '                strNutrients = strNutrients & lblCalcium & " " & strCalcium & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayIron") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Iron")) = "" Then
    '                strUnitIron = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_Iron"))
    '                strFormatIron = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("IronFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("Iron").ToString.Contains(".") Then
    '                    strIron = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("Iron"), strFormatIron) & strUnitIron
    '                Else
    '                    strIron = dsRecipeDetails.Tables("Table3").Rows(0).Item("Iron") & strUnitIron
    '                End If
    '                strNutrients = strNutrients & lblIron & " " & strIron & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayMonoUnsaturated") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("MonoUnsaturated")) = "" Then
    '                strUnitMonoUnsaturated = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_MonoUnsaturated"))
    '                strFormatMonoUnsaturated = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("MonoUnsaturatedFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("MonoUnsaturated").ToString.Contains(".") Then
    '                    strMonoUnsaturated = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("MonoUnsaturated"), strFormatMonoUnsaturated) & strUnitMonoUnsaturated
    '                Else
    '                    strMonoUnsaturated = dsRecipeDetails.Tables("Table3").Rows(0).Item("MonoUnsaturated") & strUnitMonoUnsaturated
    '                End If
    '                strNutrients = strNutrients & lblMonoUnsaturated & " " & strMonoUnsaturated & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayPolyUnsaturated") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("PolyUnsaturated")) = "" Then
    '                strUnitPolyUnsaturated = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_PolyUnsaturated"))
    '                strFormatPolyUnsaturated = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("PolyUnsaturatedFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("PolyUnsaturated").ToString.Contains(".") Then
    '                    strPolyUnsaturated = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("PolyUnsaturated"), strFormatPolyUnsaturated) & strUnitPolyUnsaturated
    '                Else
    '                    strPolyUnsaturated = dsRecipeDetails.Tables("Table3").Rows(0).Item("PolyUnsaturated") & strUnitPolyUnsaturated
    '                End If
    '                strNutrients = strNutrients & lblPolyUnsaturated & " " & strPolyUnsaturated & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayPotassium") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Potassium")) = "" Then
    '                strUnitPotassium = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_Potassium"))
    '                strFormatPotassium = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("PotassiumFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("Potassium").ToString.Contains(".") Then
    '                    strPotassium = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("Potassium"), strFormatPotassium) & strUnitPotassium
    '                Else
    '                    strPotassium = dsRecipeDetails.Tables("Table3").Rows(0).Item("Potassium") & strUnitPotassium
    '                End If
    '                strNutrients = strNutrients & lblPotassium & " " & strPotassium & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayVitaminD") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminD")) = "" Then
    '                strUnitVitaminD = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_VitaminD"))
    '                strFormatVitaminD = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminDFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminD").ToString.Contains(".") Then
    '                    strVitaminD = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminD"), strFormatVitaminD) & strUnitVitaminD
    '                Else
    '                    strVitaminD = dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminD") & strUnitVitaminD
    '                End If
    '                strNutrients = strNutrients & lblVitaminD & " " & strVitaminD & ", "
    '            End If
    '            If dsRecipeDetails.Tables("Table3").Rows(0).Item("DisplayVitaminE") = 1 And Not fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminE")) = "" Then
    '                strUnitVitaminE = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("Unit_VitaminE"))
    '                strFormatVitaminE = fctCheckFormat(dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminEFormat"))
    '                If dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminE").ToString.Contains(".") Then
    '                    strVitaminE = Format(dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminE"), strFormatVitaminE) & strUnitVitaminE
    '                Else
    '                    strVitaminE = dsRecipeDetails.Tables("Table3").Rows(0).Item("VitaminE") & strUnitVitaminE
    '                End If
    '                strNutrients = strNutrients & lblVitaminE & " " & strVitaminE & ", "
    '            End If

    '            If Right(strNutrients, 2) = ", " Then strNutrients = strNutrients.Remove(Len(strNutrients) - 2, 2)

    '            'FORMAT TABLE
    '            strHTMLContent.Append("<html " & _
    '                            "xmlns:o='urn:schemas-microsoft-com:office:office' " & _
    '                            "xmlns:w='urn:schemas-microsoft-com:office:word'" & _
    '                            "xmlns='http://www.w3.org/TR/REC-html40'>;" & _
    '                            "<head><title></title>")

    '            strHTMLContent.Append("<!--[if gte mso 9]>" & _
    '                                             "<xml>" & _
    '                                             "<w:WordDocument>" & _
    '                                             "<w:View>Print</w:View>" & _
    '                                             "<w:Zoom>100</w:Zoom>" & _
    '                                             "</w:WordDocument>" & _
    '                                             "</xml>" & _
    '                                             "<![endif]-->")

    '            strHTMLContent.Append("<style>" & _
    '                                            "<!-- /* Style Definitions */ " & _
    '                                            "p.MsoFooter, li.MsoFooter, div.MsoFooter " & _
    '                                            "{margin:0in; " & _
    '                                            "margin-bottom:.0001pt; " & _
    '                                            "mso-pagination:widow-orphan; " & _
    '                                            "tab-stops:center 3.0in right 6.0in; " & _
    '                                            "font-size:12.0pt;} " & _
    '                                            "p.MsoHeader, li.MsoHeader, div.MsoHeader " & _
    '                                            "{margin:0in; " & _
    '                                            "margin-bottom:.0001pt; " & _
    '                                            "mso-pagination:widow-orphan; " & _
    '                                            "tab-stops:center 3.0in right 6.0in; " & _
    '                                            "font-size:12.0pt;} ")

    '            strHTMLContent.Append("@page Section1" & _
    '                                         "   {size:8.5in 11.0in; " & _
    '                                         "   margin:1in 1in 1in 1in; " & _
    '                                         "   mso-footer-margin:.5in; mso-paper-source:0;} " & _
    '                                         " div.Section1 " & _
    '                                         "   {page:Section1; " & _
    '                                         "font-size:11.5pt;font-family:""Calibri"";mso-fareast-font-family:""Calibri""; " & _
    '                                          " } " & _
    '                                         "-->" & _
    '                                        "</style></head>")
    '            ''
    '            strHTMLContent.Append("<body lang=EN-US >" & _
    '                                           "<div class=Section1>")

    '            If bitFormat = 1 Then
    '                strHTMLContent.Append("<table style='width: 620'>")
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td>")
    '                strHTMLContent.Append("<table style='width: 620'>")

    '                'Recipe Number
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td>")
    '                strHTMLContent.Append("<table>")
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(lblRecipeNumber.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strRecipeNumber.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")
    '                strHTMLContent.Append("</table>")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")

    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td>")
    '                strHTMLContent.Append("&nbsp;")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")

    '                'Sub Title
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td>")
    '                strHTMLContent.Append("<table>")
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(lblSubTitle.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strSubTitle.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")
    '                strHTMLContent.Append("</table>")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")

    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td>")
    '                strHTMLContent.Append("&nbsp;")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")

    '                'Image
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td style='text-align: center'>")
    '                strHTMLContent.Append("<table style='text-align: center'>")
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append(" <td style='text-align: center'>")
    '                strHTMLContent.Append("<img src='" & imgRecipe & "' height=240 width=240 />")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")
    '                strHTMLContent.Append("</table>")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")

    '                'Recipe Name
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td style='font-weight: bold; font-size: x-large; text-align: center; font-family: Calibri;'>")
    '                strHTMLContent.Append(strRecipeName.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")

    '                'Subheading
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td style='font-weight: bold; text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strSubHeading.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")

    '                'Servings
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td style='text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strServings.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")

    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td>")
    '                strHTMLContent.Append("&nbsp;")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")

    '                ''Recipe Time
    '                'strHTMLContent.Append("<tr>")
    '                'strHTMLContent.Append("<td style='text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
    '                'strHTMLContent.Append("<table align='center' cellspacing='10'>")
    '                'strHTMLContent.Append("<tr>")
    '                'For Each RecipeTime As DataRow In dsRecipeDetails.Tables("Table4").Rows
    '                '    strRecipeTime = RecipeTime.Item("Description")
    '                '    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                '    strHTMLContent.Append(strRecipeTime.ToString)
    '                '    strHTMLContent.Append("</td>")
    '                'Next
    '                'strHTMLContent.Append("</tr>")
    '                'strHTMLContent.Append("</table>")
    '                'strHTMLContent.Append("</td>")
    '                'strHTMLContent.Append("</tr>")

    '                'Recipe Time
    '                strHTMLContent.Append("<p style='text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
    '                For Each RecipeTime As DataRow In dsRecipeDetails.Tables("Table4").Rows
    '                    strRecipeTime = RecipeTime.Item("Description")
    '                    strHTMLContent.Append(strRecipeTime.ToString)
    '                    strHTMLContent.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
    '                Next

    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td>")
    '                strHTMLContent.Append("&nbsp;")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")

    '                'Ingredients
    '                If dsRecipeDetails.Tables("Table1").Rows.Count > 0 Then
    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td>")
    '                    strHTMLContent.Append("<table style='width: 620'>")
    '                    For Each Ingredients As DataRow In dsRecipeDetails.Tables("Table1").Rows
    '                        If Ingredients.Item("Description") <> "" Then
    '                            strIngredients = fctCheckDbNull(Ingredients.Item("Description"))
    '                        Else
    '                            strIngredients = fctCheckDbNullNumeric(Ingredients.Item("Quantity")) & " " & fctCheckDbNull(Ingredients.Item("UOM")) & " " & fctCheckDbNull(Ingredients.Item("Name"))
    '                        End If
    '                        strHTMLContent.Append("<tr>")
    '                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                        strHTMLContent.Append(strIngredients.ToString)
    '                        strHTMLContent.Append("</td>")
    '                        strHTMLContent.Append("</tr>")
    '                    Next
    '                    strHTMLContent.Append("</table>")
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")
    '                End If
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td>")
    '                strHTMLContent.Append("&nbsp;")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")

    '                'Method Header
    '                If strMethodHeader.ToString <> "" Then
    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; font-weight: bold'>")
    '                    strHTMLContent.Append(strMethodHeader.ToString)
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")
    '                End If

    '                'Directions
    '                If strDirections.ToString <> "" Then
    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                    strHTMLContent.Append(strDirections.ToString)
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")
    '                End If
    '                strHTMLContent.Append("</table>")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")

    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td>")
    '                strHTMLContent.Append("&nbsp;")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")

    '                'Footnote 1
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strFootNote1)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")

    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td>")
    '                strHTMLContent.Append("&nbsp;")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")

    '                'Footnote 2
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strFootNote2)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")

    '                'Information
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td>")
    '                strHTMLContent.Append("<table style='width: 620'>")
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td colspan='2' style='text-align: center; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(lblInformation.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td style='vertical-align: top;'>")
    '                strHTMLContent.Append("<table style='width: 620'>")
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(lblRecipeStatus.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strRecipeStatus.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
    '                strHTMLContent.Append("&nbsp;")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(lblUpdatedBy.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strUpdatedBy.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(lblWebStatus.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strWebStatus.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
    '                strHTMLContent.Append("&nbsp;")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append("&nbsp;")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append("&nbsp;")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(lblDateCreated.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strDateCreated.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
    '                strHTMLContent.Append("&nbsp;")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(lblCreatedBy.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strCreatedBy.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(lblDateLastModified.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strDateLastModified.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
    '                strHTMLContent.Append("&nbsp;")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(lblModifiedBy.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strModifiedBy.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(lblLastTested.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strLastTested.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
    '                strHTMLContent.Append("&nbsp;")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(lblTestedBy.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strTestedBy.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(lblDateDeveloped.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strDateDeveloped.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
    '                strHTMLContent.Append("&nbsp;")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(lblDevelopedBy.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strDevelopedBy.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(lblDateOfFinalEdit.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strDateOfFinalEdit.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
    '                strHTMLContent.Append("&nbsp;")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(lblFinalEditBy.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strFinalEditBy.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri; vertical-align: top; width:200'>")
    '                strHTMLContent.Append(lblDevelopmentPurpose.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; vertical-align: top;' colspan=4>")
    '                strHTMLContent.Append(strDevelopmentPurpose.ToString)
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")
    '                strHTMLContent.Append("</table>")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")
    '                strHTMLContent.Append("</table>")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")

    '                'Comments
    '                If dsRecipeDetails.Tables("Table5").Rows.Count > 0 Then
    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td>")
    '                    strHTMLContent.Append("<table style='width: 620'>")
    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td style='font-weight: bold; text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
    '                    strHTMLContent.Append(lblComments.ToString)
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")
    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td>")
    '                    strHTMLContent.Append("<table>")

    '                    strHTMLContent.Append("<p style='padding-right: 0px; font-size: 11.5pt; font-family: Calibri; vertical-align: top;'>")
    '                    For Each Comments As DataRow In dsRecipeDetails.Tables("Table5").Rows
    '                        If Not IsDBNull(Comments.Item("SubmitDate")) Then strSubmitDate = CDate(Comments.Item("SubmitDate")).ToString("MM/dd/yyyy")
    '                        strOwnerName = fctCheckDbNull(Comments.Item("OwnerName"))
    '                        strComments = fctCheckDbNull(Comments.Item("Description"))

    '                        strHTMLContent.Append("<tr>")
    '                        strHTMLContent.Append("<td style='padding-right: 10px; font-size: 11.5pt; font-family: Calibri; vertical-align: top;'>")
    '                        strHTMLContent.Append(strSubmitDate.ToString)
    '                        strHTMLContent.Append("</td>")
    '                        strHTMLContent.Append("<td style='padding-right: 0px; font-size: 11.5pt; font-family: Calibri; vertical-align: top; width: 130px;'>")
    '                        strHTMLContent.Append(strOwnerName.ToString)
    '                        strHTMLContent.Append("</td>")
    '                        strHTMLContent.Append("<td style='padding-right: 10px; font-size: 11.5pt; font-family: Calibri; vertical-align: top;'>")
    '                        strHTMLContent.Append(strComments.ToString)
    '                        strHTMLContent.Append("</td>")
    '                        strHTMLContent.Append("</tr>")
    '                    Next
    '                    strHTMLContent.Append("</table>")
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")

    '                    strHTMLContent.Append("</table>")
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")
    '                End If

    '                'Attributes
    '                If dsRecipeDetails.Tables("Table6").Rows.Count > 0 Then
    '                    Dim lngCode As Long
    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td>")
    '                    strHTMLContent.Append("<table style='width: 620'>")
    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td style='font-weight: bold; text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
    '                    strHTMLContent.Append(lblAttributes.ToString)
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")

    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                    For Each Attributes In dsRecipeDetails.Tables("Table6").Rows
    '                        If Not IsDBNull(Attributes("Main")) Then
    '                            lngCode = Attributes("Parent")
    '                            strAttributes = Attributes("Name")

    'ReIdentifyParent:

    '                            For Each Parents In dsRecipeDetails.Tables("Table6").Select("Code = " & lngCode)
    '                                strAttributes = fctCheckDbNull(Parents.Item("Name")) & " : " & strAttributes
    '                                If Parents("Parent") > 0 Then
    '                                    lngCode = fctCheckDbNull(Parents.Item("Parent"))
    '                                    GoTo ReIdentifyParent
    '                                Else
    '                                    Exit For
    '                                End If
    '                            Next

    '                            strHTMLContent.Append("<tr>")
    '                            strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                            strHTMLContent.Append(strAttributes.ToString)
    '                            strHTMLContent.Append("</td>")
    '                            strHTMLContent.Append("</tr>")

    '                        End If
    '                    Next
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")
    '                    strHTMLContent.Append("</table>")
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")
    '                End If

    '                'Recipe Brand
    '                If dsRecipeDetails.Tables("Table7").Rows.Count > 0 Then
    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td>")
    '                    strHTMLContent.Append("<table style='width: 620'>")
    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: center' colspan='2'>")
    '                    strHTMLContent.Append(lblRecipeBrand.ToString)
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")
    '                    For Each Brands As DataRow In dsRecipeDetails.Tables("Table7").Rows
    '                        strRecipeBrand = fctCheckDbNull(Brands.Item("BrandName"))
    '                        strRecipeBrandClassification = fctCheckDbNull(Brands.Item("BrandClassification"))
    '                        strHTMLContent.Append("<tr>")
    '                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                        strHTMLContent.Append(strRecipeBrand.ToString & " - " & strRecipeBrandClassification.ToString)
    '                        strHTMLContent.Append("</td>")
    '                        strHTMLContent.Append("</tr>")
    '                    Next

    '                    strHTMLContent.Append("</table>")
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")
    '                End If

    '                'Placements
    '                If dsRecipeDetails.Tables("Table8").Rows.Count > 0 Then
    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td>")
    '                    strHTMLContent.Append("<table style='width: 620'>")
    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: center' colspan='3'>")
    '                    strHTMLContent.Append(lblPlacements.ToString)
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")
    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td>")
    '                    strHTMLContent.Append("<table>")
    '                    For Each Placements As DataRow In dsRecipeDetails.Tables("Table8").Rows
    '                        strPlacementName = fctCheckDbNull(Placements.Item("Placement"))
    '                        If Not IsDBNull(Placements.Item("PlacementDate")) Then strPlacementDate = CDate(Placements.Item("PlacementDate")).ToString("MM/dd/yyyy")
    '                        strPlacementDescription = fctCheckDbNull(Placements.Item("Description"))

    '                        strHTMLContent.Append("<tr>")
    '                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; padding-right: 10px'>")
    '                        strHTMLContent.Append(strPlacementName.ToString)
    '                        strHTMLContent.Append("</td>")
    '                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; padding-right: 10px'>")
    '                        strHTMLContent.Append(strPlacementDate.ToString)
    '                        strHTMLContent.Append("</td>")
    '                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; padding-right: 10px'>")
    '                        strHTMLContent.Append(strPlacementDescription.ToString)
    '                        strHTMLContent.Append("</td>")
    '                        strHTMLContent.Append("</tr>")
    '                    Next
    '                    strHTMLContent.Append("</table>")
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")
    '                    strHTMLContent.Append("</table>")
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")
    '                End If

    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td>")
    '                strHTMLContent.Append("&nbsp;")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")

    '                'Nutrients
    '                If dsRecipeDetails.Tables("Table3").Rows.Count > 0 Then
    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td>")
    '                    strHTMLContent.Append("<table style='width: 620'>")
    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
    '                    strHTMLContent.Append(lblNutritionalInformation.ToString)
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")
    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                    strHTMLContent.Append(strNutrients.ToString)
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")
    '                    strHTMLContent.Append("</table>")
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")
    '                End If
    '                strHTMLContent.Append("<tr>")
    '                strHTMLContent.Append("<td>")
    '                strHTMLContent.Append("&nbsp;")
    '                strHTMLContent.Append("</td>")
    '                strHTMLContent.Append("</tr>")

    '                'Net Carbs
    '                If strNetCarbohydrates.ToString <> "" Then
    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td>")
    '                    strHTMLContent.Append("<table style='width: 620'>")
    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
    '                    strHTMLContent.Append(strNetCarbohydrates.ToString)
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")
    '                    strHTMLContent.Append("<tr>")
    '                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
    '                    strHTMLContent.Append(lblNetCarbs.ToString)
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")
    '                    strHTMLContent.Append("</table>")
    '                    strHTMLContent.Append("</td>")
    '                    strHTMLContent.Append("</tr>")
    '                End If

    '                strHTMLContent.Append("</table>")
    '            Else
    '                'Image
    '                strHTMLContent.Append("<p style='font-weight: bold; font-size: x-large; text-align: center; font-family: Calibri;'>")
    '                strHTMLContent.Append("<img src='" & imgRecipe & "' height=240 width=240 />")
    '                strHTMLContent.Append("</p>")

    '                'Recipe Name
    '                strHTMLContent.Append("<p style='font-weight: bold; font-size: x-large; text-align: center; font-family: Calibri;'>")
    '                strHTMLContent.Append(strRecipeName.ToString)
    '                strHTMLContent.Append("</p>")

    '                'Subheading
    '                strHTMLContent.Append("<p style='font-weight: bold; text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strSubHeading.ToString)
    '                strHTMLContent.Append("</p>")

    '                'Servings
    '                strHTMLContent.Append("<p style='text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strServings.ToString)
    '                strHTMLContent.Append("</p>")

    '                'Recipe Time
    '                strHTMLContent.Append("<p style='text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
    '                For Each RecipeTime As DataRow In dsRecipeDetails.Tables("Table4").Rows
    '                    strRecipeTime = RecipeTime.Item("Description")
    '                    strHTMLContent.Append(strRecipeTime.ToString)
    '                    strHTMLContent.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
    '                Next
    '                strHTMLContent.Append("</p>")

    '                'Ingredients
    '                If dsRecipeDetails.Tables("Table1").Rows.Count > 0 Then
    '                    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")
    '                    For Each Ingredients As DataRow In dsRecipeDetails.Tables("Table1").Rows
    '                        If Ingredients.Item("Description") <> "" Then
    '                            strIngredients = fctCheckDbNull(Ingredients.Item("Description"))
    '                        Else
    '                            strIngredients = fctCheckDbNullNumeric(Ingredients.Item("Quantity")) & " " & fctCheckDbNull(Ingredients.Item("UOM")) & " " & fctCheckDbNull(Ingredients.Item("Name"))
    '                        End If
    '                        strHTMLContent.Append(strIngredients.ToString)
    '                        strHTMLContent.Append("<br>")
    '                    Next
    '                    strHTMLContent.Append("</p>")
    '                End If

    '                'Method Header
    '                If strMethodHeader.ToString <> "" Then
    '                    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri; font-weight: bold'>")
    '                    strHTMLContent.Append(strMethodHeader.ToString)
    '                    strHTMLContent.Append("</p>")
    '                End If

    '                'Directions
    '                If strDirections.ToString <> "" Then
    '                    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")
    '                    strHTMLContent.Append(strDirections.ToString)
    '                    strHTMLContent.Append("</p>")
    '                End If

    '                'Footnote 1
    '                strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strFootNote1)
    '                strHTMLContent.Append("</p>")

    '                'Footnote 2
    '                strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")
    '                strHTMLContent.Append(strFootNote2)
    '                strHTMLContent.Append("</p>")

    '                ''Placements
    '                'If dsRecipeDetails.Tables("Table8").Rows.Count > 0 Then
    '                '    strHTMLContent.Append("<p style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: center'>")
    '                '    strHTMLContent.Append(lblPlacements.ToString)
    '                '    strHTMLContent.Append("</p>")
    '                '    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri; padding-right: 10px'>")
    '                '    For Each Placements As DataRow In dsRecipeDetails.Tables("Table8").Rows
    '                '        strPlacementName = fctCheckDbNull(Placements.Item("Placement"))
    '                '        If Not IsDBNull(Placements.Item("PlacementDate")) Then strPlacementDate = CDate(Placements.Item("PlacementDate")).ToString("MM/dd/yyyy")
    '                '        strPlacementDescription = fctCheckDbNull(Placements.Item("Description"))

    '                '        strHTMLContent.Append(strPlacementName.ToString)
    '                '        strHTMLContent.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
    '                '        strHTMLContent.Append(strPlacementDate.ToString)
    '                '        strHTMLContent.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
    '                '        strHTMLContent.Append(strPlacementDescription.ToString)
    '                '        strHTMLContent.Append("<br>")
    '                '    Next
    '                '    strHTMLContent.Append("</p>")
    '                'End If

    '                'Nutrients
    '                If dsRecipeDetails.Tables("Table3").Rows.Count > 0 Then
    '                    strHTMLContent.Append("<p style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
    '                    strHTMLContent.Append(lblNutritionalInformation.ToString)
    '                    strHTMLContent.Append("</p>")
    '                    strHTMLContent.Append(strNutrients.ToString)
    '                End If

    '                'Net Carbs
    '                If strNetCarbohydrates.ToString <> "" Then
    '                    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
    '                    strHTMLContent.Append(strNetCarbohydrates.ToString)
    '                    strHTMLContent.Append("</p>")
    '                    strHTMLContent.Append(lblNetCarbs.ToString)
    '                End If

    '            End If

    '            strHTMLContent.Append("</div></body></html>")


    '            strErr = ""

    '        Else
    '            strErr = "not found"
    '        End If
    '        Return strHTMLContent
    '    End Function

    Private Function fctFormatNumericQuantity(ByVal dblQty As Double, ByVal strFormat As String, _
    ByVal blnRemoveTrailingZeros As Boolean, Optional ByVal intDisplayAsFraction As Integer = 0) As String
        Dim strQty As String
        Dim dblWhole As Double
        Dim dblDecimal As Double
        Dim strWhole As String = Nothing
        Dim strDecimal As String
        Dim strTemp As String
        Dim lngWhole As Integer
        Dim i As Integer
        Dim strOriginal As String
        'Dim dblWhole As Double

        On Error GoTo Err_Format
        strQty = Format(dblQty, strFormat)
        strOriginal = strQty

        If intDisplayAsFraction = 1 Then
            strQty = ConvertDecimalToFraction2(fctCheckDbNullNumeric(dblQty)) '// DRR 07.10.2012 removed comment
        Else
            If blnRemoveTrailingZeros Then
                'lngWhole = CLng(Int(strQty))
                dblWhole = CDbl(Int(strQty))

                For i = 1 To Len(strQty)
                    strTemp = Mid(strQty, 1, i)

                    If IsNumeric(strTemp) Then
                        'If CLng(strTemp) = lngWhole Then
                        If CDbl(strTemp) = dblWhole Then
                            strWhole = strTemp
                            Exit For
                        End If
                    End If
                Next

                strDecimal = Replace(strQty, strWhole, "", 1, 1)
                Do While Microsoft.VisualBasic.Right(strDecimal, 1) = "0"
                    strDecimal = Microsoft.VisualBasic.Left(strDecimal, Len(strDecimal) - 1)
                Loop

                strQty = strWhole & strDecimal
            End If

            If Microsoft.VisualBasic.Right(strQty, 1) = "." Then strQty = Microsoft.VisualBasic.Left(strQty, Len(strQty) - 1)
            If Microsoft.VisualBasic.Right(strQty, 1) = "," Then strQty = Microsoft.VisualBasic.Left(strQty, Len(strQty) - 1)
            If Microsoft.VisualBasic.Right(strQty, 1) = "'" Then strQty = Microsoft.VisualBasic.Left(strQty, Len(strQty) - 1)

        End If

        fctFormatNumericQuantity = strQty
        Exit Function

Err_Format:
        fctFormatNumericQuantity = strOriginal
        'MsgBox(FTB(132577) & " " & Err.Number)
        Resume Next
    End Function

    Public Function ExportToWordNew2(ByVal intListeID As Integer, ByVal bitFormat As Byte, ByVal intCodeTrans As Integer, ByRef strErr As String, ByRef strFilename As String, _
        ByVal strImage As String, ByVal strImage2 As String, ByVal intCodeLang As Integer, blnUseFractions As Boolean, Optional ByVal bitQtyFormat As Byte = 0, _
        Optional ByVal blCookmode As Boolean = False, Optional ByVal intCodeSet As Integer = 0, Optional udtListeType As enumDataListItemType = enumDataListItemType.Recipe, _
        Optional ByVal blnMetImp As Boolean = True, Optional intCodeSite As Integer = 0, Optional ByVal intLangFromCodeDictionary As Integer = 1, _
        Optional blnRemoveTrailingZeroes As Boolean = False, Optional blnAfterIngredient As Boolean = False, Optional intCodeUser As Integer = 0) As StringBuilder
        Dim strHTMLContent As StringBuilder = New StringBuilder()
        Dim dsRecipeDetails As New DataSet

        'Dim oMhtCol As New clsMhtml.mhtImageCollection 'CMV 051911
        Dim lblRecipeID As String = ""
        Dim strRecipeID As String = ""
        Dim lblRecipeNumber As String = ""
        Dim strRecipeNumber As String = ""
        Dim lblSubTitle As String = ""
        Dim strSubTitle As String = ""

        'JTOC 10.29.2013
        '-------------------------------------------
        Dim lblRecipeDescription As String = ""
        Dim strRecipeDescription As String = ""
        Dim lblRecipeRemark As String = ""
        Dim strRecipeRemark As String = ""
        Dim lblYield1 As String = ""
        Dim lblYield2 As String = ""
        Dim lblWeight As String = ""
        Dim strWeight As String = ""
        Dim strWeightQty As String = ""
        '-------------------------------------------

        Dim imgRecipe As String = ""
        Dim strImagePath As String = ""
        Dim strRecipeName As String = ""
        Dim strSubHeading As String = ""
        Dim strServings As String = ""
        Dim strYield As String = ""
        Dim strYield2 As String = ""
        Dim strServingsUnit As String = "" 'CMV 050211
        Dim strRecipeTime As String = ""
        Dim strMethodHeader As String = ""
        Dim strIngredients As String = ""
        Dim dblQty As Double
        Dim strUOM As String = ""
        Dim strDirections As String = ""
        Dim strAbbrDirections As String = ""
        Dim strFootNote1 As String = ""
        Dim strFootNote2 As String = ""
        Dim lblCostPerRecipe As String = ""
        Dim strCostPerRecipe As String = ""
        Dim lblCostPerServings As String = ""
        Dim strCostPerServings As String = ""
        Dim strCurrency As String = ""
        Dim lblInformation As String = ""
        Dim lblRecipeStatus As String = ""
        Dim strRecipeStatus As String = ""
        Dim lblWebStatus As String = ""
        Dim strWebStatus As String = ""
        Dim lblDateCreated As String = ""
        Dim strDateCreated As String = ""
        Dim lblDateLastModified As String = ""
        Dim strDateLastModified As String = ""
        Dim lblLastTested As String = ""
        Dim strLastTested As String = ""
        Dim lblDateDeveloped As String = ""
        Dim strDateDeveloped As String = ""
        Dim lblDateOfFinalEdit As String = ""
        Dim strDateOfFinalEdit As String = ""
        Dim lblDevelopmentPurpose As String = ""
        Dim strDevelopmentPurpose As String = ""
        Dim lblUpdatedBy As String = ""
        Dim strUpdatedBy As String = ""
        Dim lblCreatedBy As String = ""
        Dim strCreatedBy As String = ""
        Dim lblModifiedBy As String = ""
        Dim strModifiedBy As String = ""
        Dim lblTestedBy As String = ""
        Dim strTestedBy As String = ""
        Dim lblDevelopedBy As String = ""
        Dim strDevelopedBy As String = ""
        Dim lblFinalEditBy As String = ""
        Dim strFinalEditBy As String = ""
        Dim lblComments As String = ""
        Dim strSubmitDate As String = ""
        Dim strOwnerName As String = ""
        Dim strComments As String = ""
        Dim lblAttributes As String = ""
        Dim strAttributes As String = ""
        Dim strParents As String = ""
        Dim intAttributesCode As Integer
        Dim intAttributesParent As Integer
        Dim intAttributesMain As Integer
        Dim lblRecipeBrand As String = ""
        Dim lblRecipeNote As String = ""
        Dim lblRecipeAddNote As String = ""
        Dim strRecipeBrand As String = ""
        Dim strRecipeBrandClassification As String = ""
        Dim lblPlacements As String = ""
        Dim strPlacementName As String = ""
        Dim strPlacementDate As String = ""
        Dim strPlacementDescription As String = ""
        Dim lblNutritionalInformation As String = ""
        Dim lblCalories As String = ""
        Dim lblCaloriesFromFat As String = ""
        Dim lblSatFat As String = ""
        Dim lblTransFat As String = ""
        Dim lblMonoSatFat As String = ""
        Dim lblPolyFat As String = ""
        Dim lblTotalFat As String = ""
        Dim lblCholesterol As String = ""
        Dim lblSodium As String = ""
        Dim lblTotalCarbohydrates As String = ""
        Dim lblSugars As String = ""
        Dim lblDietaryFiber As String = ""
        Dim lblNetCarbohydrates As String = ""
        Dim lblProtein As String = ""
        Dim lblVitaminA As String = ""
        Dim lblVitaminC As String = ""
        Dim lblCalcium As String = ""
        Dim lblIron As String = ""
        Dim lblMonoUnsaturated As String = ""
        Dim lblPolyUnsaturated As String = ""
        Dim lblPotassium As String = ""
        Dim lblVitaminD As String = ""
        Dim lblVitaminE As String = ""
        Dim lblOmega3 As String = ""
        Dim strCalories As String = ""
        Dim strCaloriesFromFat As String = ""
        Dim strSatFat As String = ""
        Dim strTransFat As String = ""
        Dim strMonoSatFat As String = ""
        Dim strPolyFat As String = ""
        Dim strTotalFat As String = ""
        Dim strCholesterol As String = ""
        Dim strSodium As String = ""
        Dim strTotalCarbohydrates As String = ""
        Dim strSugars As String = ""
        Dim strDietaryFiber As String = ""
        Dim strNetCarbohydrates As String = ""
        Dim lblNetCarbs As String = ""
        Dim strProtein As String = ""
        Dim strVitaminA As String = ""
        Dim strVitaminC As String = ""
        Dim strCalcium As String = ""
        Dim strIron As String = ""
        Dim strMonoUnsaturated As String = ""
        Dim strPolyUnsaturated As String = ""
        Dim strPotassium As String = ""
        Dim strVitaminD As String = ""
        Dim strVitaminE As String = ""
        Dim strOmega3 As String = ""
        Dim strUnitCalories As String = ""
        Dim strUnitCaloriesFromFat As String = ""
        Dim strUnitSatFat As String = ""
        Dim strUnitTransFat As String = ""
        Dim strUnitMonoSatFat As String = ""
        Dim strUnitPolyFat As String = ""
        Dim strUnitTotalFat As String = ""
        Dim strUnitCholesterol As String = ""
        Dim strUnitSodium As String = ""
        Dim strUnitTotalCarbohydrates As String = ""
        Dim strUnitSugars As String = ""
        Dim strUnitDietaryFiber As String = ""
        Dim strUnitNetCarbohydrates As String = ""
        Dim strUnitProtein As String = ""
        Dim strUnitVitaminA As String = ""
        Dim strUnitVitaminC As String = ""
        Dim strUnitCalcium As String = ""
        Dim strUnitIron As String = ""
        Dim strUnitMonoUnsaturated As String = ""
        Dim strUnitPolyUnsaturated As String = ""
        Dim strUnitPotassium As String = ""
        Dim strUnitVitaminD As String = ""
        Dim strUnitVitaminE As String = ""
        Dim strUnitOmega3 As String = ""
        Dim strFormatCalories As String = ""
        Dim strFormatCaloriesFromFat As String = ""
        Dim strFormatSatFat As String = ""
        Dim strFormatTransFat As String = ""
        Dim strFormatMonoSatFat As String = ""
        Dim strFormatPolyFat As String = ""
        Dim strFormatTotalFat As String = ""
        Dim strFormatCholesterol As String = ""
        Dim strFormatSodium As String = ""
        Dim strFormatTotalCarbohydrates As String = ""
        Dim strFormatSugars As String = ""
        Dim strFormatDietaryFiber As String = ""
        Dim strFormatNetCarbohydrates As String = ""
        Dim strFormatProtein As String = ""
        Dim strFormatVitaminA As String = ""
        Dim strFormatVitaminC As String = ""
        Dim strFormatCalcium As String = ""
        Dim strFormatIron As String = ""
        Dim strFormatMonoUnsaturated As String = ""
        Dim strFormatPolyUnsaturated As String = ""
        Dim strFormatPotassium As String = ""
        Dim strFormatVitaminD As String = ""
        Dim strFormatVitaminE As String = ""
        Dim strFormatOmega3 As String = ""
        Dim strNutrients As String = ""
        Dim isDisplay As Boolean = False ' JBB 07.22.2011
        Dim imgRecipe2 As String = "" 'TDQ 10242011

        'TDQ 11.8.2011
        Dim lblThiamin As String = ""
        Dim strThiamin As String = ""
        Dim strFormatThiamin As String = ""
        Dim strUnitThiamin As String = ""

        Dim lblRiboflavin As String = ""
        Dim strRiboflavin As String = ""
        Dim strFormatRiboflavin As String = ""
        Dim strUnitRiboflavin As String = ""

        Dim lblNiacin As String = ""
        Dim strNiacin As String = ""
        Dim strFormatNiacin As String = ""
        Dim strUnitNiacin As String = ""

        Dim lblVitaminB6 As String = ""
        Dim strVitaminB6 As String = ""
        Dim strFormatVitaminB6 As String = ""
        Dim strUnitVitaminB6 As String = ""

        Dim lblFolate As String = ""
        Dim strFolate As String = ""
        Dim strFormatFolate As String = ""
        Dim strUnitFolate As String = ""

        Dim lblVitaminB12 As String = ""
        Dim strVitaminB12 As String = ""
        Dim strFormatVitaminB12 As String = ""
        Dim strUnitVitaminB12 As String = ""

        Dim lblBiotin As String = ""
        Dim strBiotin As String = ""
        Dim strFormatBiotin As String = ""
        Dim strUnitBiotin As String = ""

        Dim lblPantothenicAcid As String = ""
        Dim strPantothenicAcid As String = ""
        Dim strFormatPantothenicAcid As String = ""
        Dim strUnitPantothenicAcid As String = ""

        Dim lblPhosphorus As String = ""
        Dim strPhosphorus As String = ""
        Dim strFormatPhosphorus As String = ""
        Dim strUnitPhosphorus As String = ""

        Dim lblIodine As String = ""
        Dim strIodine As String = ""
        Dim strFormatIodine As String = ""
        Dim strUnitIodine As String = ""

        Dim lblMagnesium As String = ""
        Dim strMagnesium As String = ""
        Dim strFormatMagnesium As String = ""
        Dim strUnitMagnesium As String = ""

        Dim lblZinc As String = ""
        Dim strZinc As String = ""
        Dim strFormatZinc As String = ""
        Dim strUnitZinc As String = ""

        Dim lblManganese As String = ""
        Dim strManganese As String = ""
        Dim strFormatManganese As String = ""
        Dim strUnitManganese As String = ""

        Dim strHeaderNutrientServing As String = ""


        'JTOC 11.05.2013
        Dim blnIncludeCostPerRecipe As Boolean = False ' IIf(bitFormat = 1, True, False)
        Dim blnIncludeCostPerServings As Boolean = False ' IIf(bitFormat = 1, True, False)
        Dim blnIncludeInformation As Boolean = False ' IIf(bitFormat = 1, True, False)
        Dim blnIncludeComment As Boolean = False ' IIf(bitFormat = 1, True, False)
        Dim blnIncludeKeyword As Boolean = False ' IIf(bitFormat = 1, True, False)
        Dim blnIncludeBrand As Boolean = False ' IIf(bitFormat = 1, True, False)
        Dim blnIncludePublication As Boolean = False ' IIf(bitFormat = 1, True, False)

        Dim blnDisplayPreparationHeader As Boolean = False
        Dim blnDisplayNotesHeader As Boolean = False
        Dim blnDisplayAdditionalNotes As Boolean = False

        GetRecipeCode(intListeID, m_RecipeId, m_Version)

        ' RDC 01.13.2014 : Code Site handler
        intCodeSite = getRecipeSiteOwner(m_RecipeId, m_Version)

        If udtListeType = enumDataListItemType.Merchandise Then
            'AGL 2012.10.12 - CWM-1634 - added branch for merchandise
            dsRecipeDetails = GetMerchandiseDetails(m_RecipeId, m_Version, intCodeTrans, 0, intCodeSet)
        Else
            dsRecipeDetails = GetRecipeDetails(m_RecipeId, m_Version, intCodeTrans, 0, intCodeSet, blnMetImp, intCodeSite, intCodeUser) 'CMV 051911
        End If

        ' RDC 12.09.2013 : Translation for labels
        Dim cLang As New clsEGSLanguage(intLangFromCodeDictionary) 'JTOC 12.11.2013 intCodeLang to intLangFromCodeDictionary

        'TRANSLATION OF LABELS
        lblRecipeID = cLang.GetString(clsEGSLanguage.CodeType.Recipe) & " ID" '"Recipe ID"
        lblRecipeNumber = cLang.GetString(clsEGSLanguage.CodeType.RecipeNumber) '"Recipe Number"
        lblRecipeDescription = cLang.GetString(clsEGSLanguage.CodeType.Description) '"Description"
        lblRecipeRemark = cLang.GetString(clsEGSLanguage.CodeType.Remark) '"Remark"
        lblYield1 = cLang.GetString(clsEGSLanguage.CodeType.Yield) & " 1:" '"Yield 1: "
        lblYield2 = cLang.GetString(clsEGSLanguage.CodeType.Yield) & " 2:" '"Yield 2: "
        lblWeight = cLang.GetString(clsEGSLanguage.CodeType.Weight) & "(" & cLang.GetString(clsEGSLanguage.CodeType.Sub_Recipe) & "):" '"Weight(Subrecipe): "



        'AGL 2012.10.31 - CWM-1971
        Dim clsLicense As New clsLicense
        If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
            lblSubTitle = cLang.GetString(clsEGSLanguage.CodeType.SubTitle) '"SubTitle" '-- JBB 02.21.2012 "Sub Title"
        Else
            lblSubTitle = cLang.GetString(clsEGSLanguage.CodeType.SubName)
        End If

        lblCostPerRecipe = cLang.GetString(clsEGSLanguage.CodeType.Cost) & " " & cLang.GetString(clsEGSLanguage.CodeType.per_) & " " & cLang.GetString(clsEGSLanguage.CodeType.Recipe)
        lblCostPerServings = cLang.GetString(clsEGSLanguage.CodeType.Cost) & " " & cLang.GetString(clsEGSLanguage.CodeType.per_serving)
        lblInformation = cLang.GetString(clsEGSLanguage.CodeType.Embassy)
        lblRecipeStatus = cLang.GetString(clsEGSLanguage.CodeType.RecipeStatus) & ":"
        lblUpdatedBy = cLang.GetString(clsEGSLanguage.CodeType.UpdatedBy) & ":"
        lblWebStatus = cLang.GetString(clsEGSLanguage.CodeType.WebStatus) & ":"
        lblDateCreated = cLang.GetString(clsEGSLanguage.CodeType.DateCreated) & ":"
        lblCreatedBy = cLang.GetString(clsEGSLanguage.CodeType.CreatedBY) & ":"
        lblDateLastModified = cLang.GetString(clsEGSLanguage.CodeType.DateLastModified) & ":"
        lblModifiedBy = cLang.GetString(clsEGSLanguage.CodeType.ModifiedBy) & ":"
        lblLastTested = cLang.GetString(clsEGSLanguage.CodeType.DateLastTested) & ":"
        lblTestedBy = cLang.GetString(clsEGSLanguage.CodeType.TestedBy) & ":"
        lblDateDeveloped = cLang.GetString(clsEGSLanguage.CodeType.DateDeveloped) & ":"
        lblDevelopedBy = cLang.GetString(clsEGSLanguage.CodeType.DevelopedBy) & ":"
        lblDateOfFinalEdit = cLang.GetString(clsEGSLanguage.CodeType.FinalEditDate) & ":"
        lblFinalEditBy = cLang.GetString(clsEGSLanguage.CodeType.FinalEditBy) & ":"
        lblDevelopmentPurpose = cLang.GetString(clsEGSLanguage.CodeType.DevelopmentPurpose) & ":"
        lblComments = cLang.GetString(clsEGSLanguage.CodeType.Comments)
        lblAttributes = cLang.GetString(clsEGSLanguage.CodeType.Attributes)
        lblRecipeBrand = cLang.GetString(clsEGSLanguage.CodeType.RecipeBrands)
        lblRecipeAddNote = cLang.GetString(clsEGSLanguage.CodeType.RecipeAddtionalNotes)
        lblRecipeNote = cLang.GetString(clsEGSLanguage.CodeType.Notes)
        'AGL 2012.10.31 - CWM-1971
        If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
            lblPlacements = cLang.GetString(clsEGSLanguage.CodeType.RecipePlacements)
        Else
            lblPlacements = cLang.GetString(clsEGSLanguage.CodeType.Publication)
        End If

        lblNutritionalInformation = ""
        lblCalories = cLang.GetString(clsEGSLanguage.CodeType.Calories)
        lblCaloriesFromFat = cLang.GetString(clsEGSLanguage.CodeType.CaloriesfromFat)
        lblSatFat = "Sat Fat"
        lblTransFat = "Trans Fat"
        lblMonoSatFat = "Mono Sat Fat"
        lblPolyFat = "Poly Sat Fat"
        lblTotalFat = "Total Fat"
        lblCholesterol = "Cholesterol"
        lblSodium = "Sodium"
        lblTotalCarbohydrates = "Total Carbohydrates"
        lblSugars = "Sugars"
        lblDietaryFiber = cLang.GetString(clsEGSLanguage.CodeType.DietaryFiber)
        lblNetCarbohydrates = "Net Carbohydrates"
        lblProtein = "Protein"
        lblVitaminA = "Vitamin A"
        lblVitaminC = "Vitamin C"
        lblCalcium = cLang.GetString(clsEGSLanguage.CodeType.Calcium)
        lblIron = "Iron"
        lblMonoUnsaturated = "Monounsaturated"
        lblPolyUnsaturated = "Polyunsaturated"
        lblPotassium = "Potassium"
        lblVitaminD = "Vitamin D"
        lblVitaminE = "Vitamin E"
        lblNetCarbs = "* " & """Net Carbs""" & " are total carbohydrates minus dietary fiber and sugar alcohol as these have a minimal impact on blood sugar."
        'lblOmega3 = "Omega3"
        lblThiamin = "Thiamin"
        lblRiboflavin = "Riboflavin"
        lblNiacin = "Niacin"
        lblVitaminB6 = "VitaminB6"
        lblFolate = "Folate"
        lblVitaminB12 = "VitaminB12"
        lblBiotin = "Biotin"
        lblPantothenicAcid = "Pantothenic_Acid"
        lblPhosphorus = "Phosphorus"
        lblIodine = "Iodine"
        lblMagnesium = "Magnesium"
        lblZinc = "Zinc"
        lblManganese = "Manganese"
        lblOmega3 = "Omega-3"

        If dsRecipeDetails.Tables("Table1").Rows.Count > 0 Then


            'SET VALUES
            strRecipeID = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("RecipeID"))
            strRecipeNumber = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Number"))
            strSubTitle = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("SubTitle"))

            'JTOC 10.29.2013
            '----------------------------------------------------------------------------------------------------
            strRecipeDescription = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Description"))
            strRecipeRemark = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Remark"))
            strWeight = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Weight"))
            strWeightQty = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("SrQty"))
            '----------------------------------------------------------------------------------------------------

            strRecipeName = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Name"))
            strSubHeading = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("SubHeading"))

            ''strImagePath = Server.MapPath("Images/test.jpg") 'CMV 051911

            ''Dim imageRecipe As New System.Web.UI.WebControls.Image 'CMV 051911
            ''With imageRecipe
            ''    .ID = "Image1"
            ''    .Height = 240
            ''    .Width = 240
            ''    .ImageUrl = "Images/test.jpg"
            ''End With

            ' RDC 12.12.2013 : Discarded on top variables in displaying yield/subrecipe wt.
            Dim decYield1 As Decimal = 0D, _
                decYield2 As Decimal = 0D, _
                decSrWt As Decimal = 0D
            Dim strYield1Unit As String = dsRecipeDetails.Tables(11).Rows(0).Item("Yield1Unit").ToString.Replace("[_]", "").Replace("n/a", "").Replace("_", " ").ToLower, _
                strYield2Unit As String = dsRecipeDetails.Tables(11).Rows(0).Item("Yield2Unit").ToString.Replace("[_]", "").Replace("n/a", "").Replace("_", " ").ToLower, _
                strSrWtUnit As String = dsRecipeDetails.Tables(11).Rows(0).Item("SrUnit").ToString.Replace("[_]", "").Replace("n/a", "").Replace("_", " ").ToLower

            If Not IsDBNull(dsRecipeDetails.Tables(11).Rows(0).Item("Yield1")) Then decYield1 = CDec(dsRecipeDetails.Tables(11).Rows(0).Item("Yield1"))
            If Not IsDBNull(dsRecipeDetails.Tables(11).Rows(0).Item("Yield2")) Then decYield2 = CDec(dsRecipeDetails.Tables(11).Rows(0).Item("Yield2"))
            If Not IsDBNull(dsRecipeDetails.Tables(11).Rows(0).Item("SubRecipeQty")) Then decSrWt = CDec(dsRecipeDetails.Tables(11).Rows(0).Item("SubRecipeQty"))

            'If decYield1 > 0 And Not strYield1Unit = "[_]" And Not strYield1Unit.ToLower = "n/a" And Not strYield1Unit.Trim.Length = 0 And Not strYield1Unit.EndsWith("s") And Not strYield1Unit.ToLower.Trim = "g" And Not strYield1Unit.Trim.Length = 1 Then
            '    If decYield1 > 1 Then
            '        If strYield1Unit.Trim.Length > 0 And Char.IsLetter(Mid(strYield1Unit, strYield1Unit.Length, 1)) Then strYield1Unit &= "s"
            '    End If

            'End If

            'If decYield2 > 0 And Not strYield2Unit = "[_]" And Not strYield2Unit.ToLower = "n/a" And Not strYield2Unit.Trim.Length = 0 And Not strYield2Unit.EndsWith("s") And Not strYield2Unit.ToLower.Trim = "g" And Not strYield2Unit.Trim.Length = 1 Then
            '    If decYield2 > 1 Then
            '        If strYield2Unit.Trim.Length > 0 And Char.IsLetter(Mid(strYield2Unit, strYield2Unit.Length, 1)) Then strYield2Unit &= "s"
            '    End If
            'End If

            'If CDec(Format(decSrWt, "#.000#")) > 0 And Not strSrWtUnit = "[_]" And Not strSrWtUnit.ToLower = "n/a" And Not strSrWtUnit.Trim.Length = 0 And Not strSrWtUnit.EndsWith("s") And Not strSrWtUnit.ToLower.Trim = "g" And Not strSrWtUnit.Trim.Length = 1 Then
            '    If decSrWt > 1 Then
            '        If strSrWtUnit.Trim.Length > 0 And Char.IsLetter(Mid(strSrWtUnit, strSrWtUnit.Length, 1)) Then strSrWtUnit &= "s"
            '    End If
            'End If

            Dim strYield1, strSrWt As String 

            Dim BlnConvertDecimaltoFraction As Boolean = CBool(dsRecipeDetails.Tables(12).Rows(0).Item("String"))

            If BlnConvertDecimaltoFraction = True Then
                strYield1 = ConvertDecimalToFraction2(fctCheckDbNullNumeric(decYield1))
                strSrWt = ConvertDecimalToFraction2(fctCheckDbNullNumeric(decSrWt))

                strYield2 = ConvertDecimalToFraction2(fctCheckDbNullNumeric(decYield2))
            Else
                strYield1 = fctCheckDbNullNumeric(decYield1)
                strSrWt = fctCheckDbNullNumeric(decSrWt)

                strYield2 = fctCheckDbNullNumeric(decYield2)
            End If

            Dim intFieldsToDisplay As Integer = 0, intFieldWidth As Integer = 0, intTableWidth As Integer = 620
            If G_ExportOptions.blnExpIncludeYield1 And decYield1 > 0 Then intFieldsToDisplay += 1
            If G_ExportOptions.blnExpIncludeYield2 And decYield2 > 0 Then intFieldsToDisplay += 1
            If G_ExportOptions.blnExpSubRecipeWt And decSrWt > 0 Then intFieldsToDisplay += 1

            Select Case intFieldsToDisplay
                Case 1
                    intFieldWidth = 620
                    intTableWidth = 250
                Case 2
                    intFieldWidth = 310
                    intTableWidth = 400
                Case 3
                    intFieldWidth = CInt(620 / 3)
                Case Else
                    intFieldWidth = CInt(620 / 3)
            End Select

            strServings = "<center><table width='" & intTableWidth & "'><tr>"
            With G_ExportOptions
                If .blnExpIncludeYield1 And decYield1 > 0 Then strServings &= "<td width='" & intFieldWidth & "' align='center' style='font-size: 11.5pt; font-family: Calibri;'> <b>" & lblYield1.ToString & "</b>&nbsp;" & strYield1 & " " & strYield1Unit & " </td>"
                If .blnExpIncludeYield2 And decYield2 > 0 Then strServings &= "<td width='" & intFieldWidth & "' align='center' style='font-size: 11.5pt; font-family: Calibri;'> <b>" & lblYield2.ToString & "</b>&nbsp;" & strYield2 & " " & strYield2Unit & "</td>"
                If .blnExpSubRecipeWt And decSrWt > 0 Then strServings &= "<td width='" & intFieldWidth & "' align='center' style='font-size: 11.5pt; font-family: Calibri;'> <b>" & lblWeight.ToString & "</b> &nbsp;" & strSrWt & " " & strSrWtUnit & "</td>"
            End With
            strServings &= "</tr></table></center>"

            strMethodHeader = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("MethodHeader"))
            strDirections = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Note"))
            ' RDC 11.28.2013 : Fix for "There is no row in position 0"
            'If Not IsDBNull(dsRecipeDetails.Tables("Table3").Rows.Count) Then
            If dsRecipeDetails.Tables("Table3").Rows.Count > 0 Then
                strAbbrDirections = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("CookMode"))
            End If

            strFootNote1 = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("FootNote1"))
            strFootNote2 = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("FootNote2"))
            strCurrency = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Currency"))
            'strCostPerRecipe = strCurrency & " " & fctCheckDbNullNumeric(dsRecipeDetails.Tables("Table1").Rows(0).Item("CostPrice"))
            'strCostPerServings = strCurrency & " " & fctCheckDbNullNumeric(dsRecipeDetails.Tables("Table1").Rows(0).Item("CostPricePerServing"))
            strRecipeStatus = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("RecipeStatusName"))
            strUpdatedBy = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("UpdatedBy"))
            strWebStatus = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("WebStatusName"))
            If Not IsDBNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateCreated")) Then strDateCreated = FormatDateTime(CStrDB(dsRecipeDetails.Tables("table1").Rows(0).Item("DateCreated")), DateFormat.ShortDate) 'CDate(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateCreated"))
            strCreatedBy = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("CreatedBy"))
            If Not IsDBNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateLastModified")) Then strDateLastModified = FormatDateTime(CStrDB(dsRecipeDetails.Tables("table1").Rows(0).Item("DateLastModified")), DateFormat.ShortDate) 'CDate(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateLastModified"))
            strModifiedBy = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("ModifiedBy"))
            If Not IsDBNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateTested")) Then strLastTested = FormatDateTime(CStrDB(dsRecipeDetails.Tables("table1").Rows(0).Item("DateTested")), DateFormat.ShortDate) 'CDate(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateTested"))
            strTestedBy = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("TestedBy"))
            If Not IsDBNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateDeveloped")) Then strDateDeveloped = FormatDateTime(CStrDB(dsRecipeDetails.Tables("table1").Rows(0).Item("DateDeveloped")), DateFormat.ShortDate) 'CDate(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateDeveloped"))
            strDevelopedBy = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("DevelopedBy"))
            If Not IsDBNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateFinalEdit")) Then strDateOfFinalEdit = FormatDateTime(CStrDB(dsRecipeDetails.Tables("table1").Rows(0).Item("DateFinalEdit")), DateFormat.ShortDate) 'CDate(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateFinalEdit"))
            strFinalEditBy = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("FinalEditBy"))
            strDevelopmentPurpose = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("DevelopmentPurpose"))

            isDisplay = CBoolDB(dsRecipeDetails.Tables("Table1").Rows(0).Item("DisplayNutrition")) ' JBB 07.22.2011

            Dim strHeader As String = fGetMethodFormat("nh")
            Dim strItems As String = fGetMethodFormat("s")
            Dim dicIsDisplay As New Dictionary(Of String, Boolean)
            Dim dicColumnName As New Dictionary(Of String, String)
            Dim dicUnit As New Dictionary(Of String, String)
            Dim dicFormat As New Dictionary(Of String, String)
            Dim intIndex As Integer = 0
            Dim strColCalories As String = ""
            Dim dtNutrients As DataTable = dsRecipeDetails.Tables("Table4")
            If dtNutrients.Rows.Count > 0 Then  '' JBB 05.23.2012
                For Each dcNutrient As DataColumn In dtNutrients.Columns
                    Dim strColumn As String = dcNutrient.ColumnName
                    If strColCalories = "" Then strColCalories = strColumn

                    If strColumn.Trim.ToLower <> "recipeid" And strColumn.Trim.ToLower <> "version" And strColumn.Trim.ToLower <> "portionsize" Then
                        If strColumn.Contains("Display") Then
                            dicIsDisplay.Add(strColumn.ToLower(), CBool(dtNutrients.Rows(intIndex)(strColumn)))
                        ElseIf strColumn.Contains("Unit_") Then
                            dicUnit.Add(strColumn.ToLower(), CStr(dtNutrients.Rows(intIndex)(strColumn)))
                        ElseIf strColumn.Contains("Format") Then
                            dicFormat.Add(strColumn.ToLower(), CStr(dtNutrients.Rows(intIndex)(strColumn)))
                        End If
                        'strNutrients.Append(strColumn.Replace("Display", "") + " " + dtNutrients.Rows(intIndex)(strColumn).ToString() + ", ")
                    End If
                    dicColumnName.Add(strColumn.ToLower(), strColumn)
                    'JTOC 14.12.2012 Removed Calo in condition
                    'If strColumn.Contains("Calo") And (Not strColumn.Contains("Display") Or Not strColumn.Contains("Unit_") Or Not strColumn.Contains("Format")) Then
                    If (Not strColumn.Contains("Display") Or Not strColumn.Contains("Unit_") Or Not strColumn.Contains("Format")) Then
                        strColCalories = strColumn
                    End If
                Next
                ' End If
                strHeaderNutrientServing = ""
                If dsRecipeDetails.Tables("Table4").Columns.Contains("PortionSize") = True Then
                    If dicColumnName.ContainsKey(strColCalories) Then
                        strHeaderNutrientServing = dsRecipeDetails.Tables("Table1").Rows(0).Item("PortionSize").ToString.Trim
                    Else
                        If dsRecipeDetails.Tables("Table4").Rows(0).Item(strColCalories).ToString.Trim <> "" Then
                            strHeaderNutrientServing = dsRecipeDetails.Tables("Table4").Rows(0).Item("PortionSize").ToString.Trim
                        Else
                            If dsRecipeDetails.Tables("Table1").Columns.Contains("Yield") = True Then
                                strHeaderNutrientServing = dsRecipeDetails.Tables("Table1").Rows(0).Item("Yield").ToString.Trim
                            End If
                        End If
                    End If
                Else
                    If dsRecipeDetails.Tables("Table1").Columns.Contains("Yield") = True Then
                        strHeaderNutrientServing = dsRecipeDetails.Tables("Table1").Rows(0).Item("Yield").ToString.Trim
                    End If
                End If

                Dim lstKey As List(Of String)
                lstKey = New List(Of String)(dicIsDisplay.Keys)
                For Each dcNutrient As DataColumn In dtNutrients.Columns
                    Dim strColumn As String = dcNutrient.ColumnName
                    If strColumn.Trim.ToLower <> "recipeid" And strColumn.Trim.ToLower <> "version" And strColumn.Trim.ToLower <> "portionsize" Then
                        If Not strColumn.Contains("Display") And Not strColumn.Contains("Unit_") And Not strColumn.Contains("Format") Then
                            If lstKey.Contains(("Display" + strColumn.ToString()).ToLower) = True Then
                                If dicIsDisplay(("Display" + strColumn.ToString()).ToLower) = True Then
                                    If dtNutrients.Rows(intIndex)(strColumn).ToString().Trim <> "-1" Then
                                        Dim strNutDisplayValue As String = Format(dicFormat((strColumn.ToString() + "Format").ToLower), IIf(dtNutrients.Rows(intIndex)(strColumn).ToString().Trim() <> "-1", dtNutrients.Rows(intIndex)(strColumn), 0)) '
                                        'strNutrients.Append(strColumn + " " + strNutDisplayValue + dicUnit(("Unit_" + strColumn.ToString()).ToLower) + ", ")
                                        strNutrients = strNutrients & Replace(strColumn, "_", " ") & " " & strNutDisplayValue & dicUnit(("Unit_" + strColumn.ToString()).ToLower) & ", "
                                        lblNutritionalInformation = cLang.GetString(clsEGSLanguage.CodeType.NutritionalInfo) & " " & strHeaderNutrientServing & " "
                                    End If
                                End If
                            End If
                        End If

                        'strNutrients.Append(strColumn.Replace("Display", "") + " " + dtNutrients.Rows(intIndex)(strColumn).ToString() + ", ")
                    End If
                Next

                If Right(strNutrients, 2) = ", " Then strNutrients = strNutrients.Remove(Len(strNutrients) - 2, 2)
            Else '' JBB 05.23.2012
                lblNutritionalInformation = ""
                strHeaderNutrientServing = ""
                strNutrients = ""
            End If '' JBB 05.23.2012

            ' RDC 11.28.2013 : Fix for "There is no row on position 0"
            If dsRecipeDetails.Tables("Table3").Rows.Count > 0 Then
                'JBB -- 07.14.2011
                strDirections = fctGetInstrunctions(dsRecipeDetails.Tables("Table3"), fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("MethodFormat")), blCookmode)
                'TDQ 2.24.2012
                strAbbrDirections = fctGetInstrunctions(dsRecipeDetails.Tables("Table3"), fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("MethodFormat")), True)
                ''fctCheckDbNull(strDirections)
                '' strAbbrDirections = fctCheckDbNull(strAbbrDirections)
                '--
            Else
                strDirections = ""
                strAbbrDirections = ""
            End If

            Dim strExpFont As String = GetLanguage(G_ExportOptions.intExpSelectedLanguage)

            'FORMAT TABLE

            strHTMLContent.Append("<html " & _
                "xmlns:o='urn:schemas-microsoft-com:office:office' " & _
                "xmlns:w='urn:schemas-microsoft-com:office:word'" & _
                "xmlns='http://www.w3.org/TR/REC-html40'>" & _
                "<head><meta charset='UTF-8' /><title></title>")
            '"<head><meta http-equiv='Content-Type' content='text/html; charset=BIG5' /><title></title>") '"<head><meta http-equiv='Content-Type' content=text/html;charset=utf-8 /><title></title>") '"<head><title></title>") 05.27.2011

            strHTMLContent.Append("<!--[if gte mso 9]>" & _
                "<xml>" & _
                "<w:WordDocument>" & _
                "<w:View>Print</w:View>" & _
                "<w:Zoom>100</w:Zoom>" & _
                "</w:WordDocument>" & _
                "</xml>" & _
                "<![endif]-->")
            strHTMLContent.Append("<html " & _
                "xmlns:o='urn:schemas-microsoft-com:office:office' " & _
                "xmlns:w='urn:schemas-microsoft-com:office:word'" & _
                "xmlns='http://www.w3.org/TR/REC-html40'>" & _
                "<head><meta charset='UTF-8' /><title></title>")
            '"<head><meta http-equiv='Content-Type' content='text/html;charset=BIG5' /><title></title>")
            strHTMLContent.Append("<style>" & _
                "<!-- /* Style Definitions */ " & _
                "p.MsoFooter, li.MsoFooter, div.MsoFooter " & _
                "{margin:0in; " & _
                "margin-bottom:.0001pt; " & _
                "mso-pagination:widow-orphan; " & _
                "tab-stops:center 3.0in right 6.0in; " & _
                "font-size:12.0pt;} " & _
                "p.MsoHeader, li.MsoHeader, div.MsoHeader " & _
                "{margin:0in; " & _
                "margin-bottom:.0001pt; " & _
                "mso-pagination:widow-orphan; " & _
                "tab-stops:center 3.0in right 6.0in; " & _
                "font-size:12.0pt;} ")

            strHTMLContent.Append("@page Section1" & _
                "   {size:8.5in 11.0in; " & _
                "   margin:1in 1in 1in 1in; " & _
                "   mso-footer-margin:.5in; mso-paper-source:0;} " & _
                " div.Section1 " & _
                "   {page:Section1; " & _
                "font-size:11.5pt;font-family:'" & strExpFont & "';mso-fareast-font-family:'" & strExpFont & "'; " & _
                " } " & _
                "-->" & _
                "</style></head>")

            strHTMLContent.Append("<body>" & _
                "<div class=Section1>")

            'strHTMLContent.Append("</div></body>")

            If bitFormat = 1 Then
                'strHTMLContent.Append("<table style='width: 620'>")
                'strHTMLContent.Append("<tr>")
                'strHTMLContent.Append("<td>")
                strHTMLContent.Append("<table style='width: 620'>")

                ' Recipe Name as per CWM-9519
                With strHTMLContent
                    .Append("<tr><td style='font-weight: bold; font-size: x-large; text-align: center; font-family: " & strExpFont & ";'>" & strRecipeName.ToString & "</td></tr>")

                    'Subheading
                    ' RDC 11.13.2013 : Option to display or not to display Sub Name/Heading
                    If G_ExportOptions.blnExpIncludeSubName Then
                        .Append("<tr><td style='font-weight: bold; text-align: center; font-size: 11.5pt; font-family: " & strExpFont & ";'>" & strSubHeading.ToString & "</td></tr>")
                    End If
                End With

                ' RDC 11.18.2013 : Create additional table if any of the settings are true
                With G_ExportOptions
                    If .blnExpIncludeRecipeNo Or .blnExpIncludeSubName Or .blnExpIncludeItemDesc Or .blnExpIncludeRemark Then
                        With strHTMLContent
                            .Append("<tr><td>" & _
                                        "<table style='font-family: " & strExpFont & ";'>")

                            ' Recipe Number
                            If G_ExportOptions.blnExpIncludeRecipeNo Then
                                .Append("<tr><td style='font-weight: bold; font-size: 11.5pt;' valign='top' width='20%'>" & lblRecipeNumber.Trim & "</td>" & _
                                            "<td style='font-size: 11.5pt;' valign='top' width='80%'>: &nbsp;" & strRecipeNumber.Trim & "</td></tr>")
                            End If

                            ' Sub Title
                            If G_ExportOptions.blnExpIncludeSubName Then
                                .Append("<tr><td style='font-weight: bold; font-size: 11.5pt;' valign='top'>" & lblSubTitle.Trim & "</td>" & _
                                            "<td style='font-size: 11.5pt;' valign='top' width='80%'>: &nbsp;" & strSubTitle.Trim & "</td></tr>")
                            End If

                            ' Description
                            If G_ExportOptions.blnExpIncludeItemDesc And strRecipeDescription.Trim.Length > 0 Then
                                .Append("<tr><td style='font-weight: bold; font-size: 11.5pt;' valign='top' width='20%'>" & lblRecipeDescription.Trim & "</td>" & _
                                        "<td style='font-size: 11.5pt;' valign='top' width='80%'>: &nbsp;" & strRecipeDescription.Trim & "</td></tr>")
                            End If

                            ' Remarks
                            If G_ExportOptions.blnExpIncludeRemark And strRecipeRemark.Trim.Length > 0 Then
                                .Append("<tr><td style='font-weight: bold; font-size: 11.5pt;' valign='top' width='20%'>" & lblRecipeRemark.Trim & "</td>" & _
                                            "<td style='font-size: 11.5pt;' valign='top' width='80%'>: &nbsp;" & strRecipeRemark.Trim & "</td></tr>")
                            End If

                            .Append("</table> " & _
                                    "</td></tr> " & _
                                    "<br />")
                        End With
                    End If
                End With


                strHTMLContent.Append("<tr>")
                strHTMLContent.Append("<td style='text-align: center'>")
                strHTMLContent.Append("<table style='text-align: center'>")
                strHTMLContent.Append("<tr>")
                strHTMLContent.Append(" <td style='text-align: center'>")
                imgRecipe = strImage ' getHtml(imageRecipe) 'CMV 051911
                strHTMLContent.Append(imgRecipe) 'CMV 051911
                strHTMLContent.Append("</td>")

                If Not strImage.Contains("nopic.jpg") And Not strImage2.Contains("nopic.jpg") Then
                    If Not strImage2 = "" Then
                        strHTMLContent.Append(" <td style='text-align: center'>")
                        imgRecipe2 = strImage2 ' getHtml(imageRecipe) 'CMV 051911
                        strHTMLContent.Append(imgRecipe2) 'CMV 051911
                        strHTMLContent.Append("</td>")
                    End If
                End If

                strHTMLContent.Append("</tr>")
                strHTMLContent.Append("</table>")
                strHTMLContent.Append("</td>")
                strHTMLContent.Append("</tr>")

                'Servings
                ' RDC 11.13.2013 : Option to display or not to display Servings
                If G_ExportOptions.blnExpIncludeYield1 Or G_ExportOptions.blnExpIncludeYield2 Or G_ExportOptions.blnExpSubRecipeWt Then
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td style='text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strServings.ToString)
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                End If

                strHTMLContent.Append("<tr>")
                strHTMLContent.Append("<td>")
                strHTMLContent.Append("&nbsp;")
                strHTMLContent.Append("</td>")
                strHTMLContent.Append("</tr>")

                'Recipe Time
                ' RDC 11.13.2013 : Option to display or not to display Recipe Time
                If G_ExportOptions.blnExpIncludeRecipeTime Then
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td style='text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                    For Each RecipeTime As DataRow In dsRecipeDetails.Tables("Table5").Rows
                        strRecipeTime = RecipeTime.Item("Description")
                        Dim intHours As Integer = CIntDB(RecipeTime("RecipeTimeHH"))
                        Dim intMinutes As Integer = CIntDB(RecipeTime("RecipeTimeMM"))
                        Dim intSeconds As Integer = CIntDB(RecipeTime("RecipeTimeSS"))
                        Dim strAnd As String = cLang.GetString(clsEGSLanguage.CodeType._And).ToString.ToLower & " "

                        If intHours > 0 And intMinutes > 0 And intSeconds > 0 Then          ' 111
                            If intHours = 1 Then strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHour).ToLower & ", ") Else strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHours).ToLower & ", ")
                            If intMinutes = 1 Then strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinute).ToLower & " " & strAnd) Else strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinutes).ToLower & " " & strAnd)
                            If intSeconds = 1 Then strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSecond).ToLower) Else strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSeconds).ToLower)
                        ElseIf intHours = 0 And intMinutes > 0 And intSeconds > 0 Then      ' 011
                            If intMinutes = 1 Then strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinute).ToLower & strAnd) Else strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinutes).ToLower & " " & strAnd)
                            If intSeconds = 1 Then strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSecond).ToLower) Else strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSeconds).ToLower)
                            strRecipeTime = strRecipeTime.Replace("0 %h", "")
                        ElseIf intHours > 0 And intMinutes > 0 And intSeconds = 0 Then      ' 110
                            If intHours = 1 Then strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHour).ToLower & " " & strAnd) Else strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHours).ToLower & " " & strAnd)
                            If intMinutes = 1 Then strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinute).ToLower) Else strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinutes).ToLower)
                            strRecipeTime = strRecipeTime.Replace("0 %s", "")
                        ElseIf intHours = 0 And intMinutes = 0 And intSeconds > 0 Then      ' 001
                            If intSeconds = 1 Then strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSecond).ToLower) Else strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSeconds).ToLower)
                            strRecipeTime = strRecipeTime.Replace("0 %h", "").Replace("0 %m", "")
                        ElseIf intHours = 0 And intMinutes > 0 And intSeconds = 0 Then      ' 010
                            If intMinutes = 1 Then strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinute).ToLower) Else strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinutes).ToLower)
                            strRecipeTime = strRecipeTime.Replace("0 %h", "").Replace("0 %s", "")
                        ElseIf intHours > 0 And intMinutes = 0 And intSeconds = 0 Then      ' 100
                            If intHours = 1 Then strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHour).ToLower) Else strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHours).ToLower)
                            strRecipeTime = strRecipeTime.Replace("0 %m", "").Replace("0 %s", "")
                        ElseIf intHours > 0 And intMinutes = 0 And intSeconds > 0 Then      ' 101
                            If intHours = 1 Then strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHour).ToLower & " " & strAnd) Else strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHours).ToLower & " " & strAnd)
                            If intSeconds = 1 Then strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSecond).ToLower) Else strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSeconds).ToLower)
                            strRecipeTime = strRecipeTime.Replace("0 %m", "")
                        Else                                                                ' 000
                            strRecipeTime = ""
                        End If

                        strHTMLContent.Append(strRecipeTime.ToString)
                        strHTMLContent.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")

                    Next
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                End If

                strHTMLContent.Append("<tr>")
                strHTMLContent.Append("<td>")
                strHTMLContent.Append("&nbsp;")
                strHTMLContent.Append("</td>")
                strHTMLContent.Append("</tr>")

                ' Ingredients
                ' RDC 11.14.2013 : Revised code for ingredient display
                If dsRecipeDetails.Tables(2).Rows.Count > 0 Then
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td>")
                    strHTMLContent.Append("<table style='font-size: 11.5pt; font-family: Calibri;' width='620'>")

                    For Each rwIngredient As DataRow In dsRecipeDetails.Tables(2).Rows
                        Dim intRemainingSize As Integer = 620
                        strHTMLContent.Append("<tr>")

                        Dim intItemType As Integer
                        Dim strIngredient As String = ""
                        Dim strItemName As String = ""
                        Dim strAltIngredient As String = ""
                        Dim strIngrComplement As String = ""
                        Dim strIngrPreparation As String = ""

                        If IsDBNull(rwIngredient("Type")) Then intItemType = 0 Else intItemType = rwIngredient("Type")

                        ' Ingredient = Complement IngredientName [or AlternativeIngredient], Preparation 
                        ' Ingredient Name
                        If Not IsDBNull(rwIngredient("Name")) And Not rwIngredient("Name").ToString.Trim.Length = 0 Then
                            strItemName = rwIngredient("Name").ToString.Trim
                        End If
                        ' Alternative Ingredient
                        If Not IsDBNull(rwIngredient("AlternativeIngredient")) And Not rwIngredient("AlternativeIngredient").ToString.Trim.Length = 0 Then
                            strAltIngredient = "[" & cLang.GetString(clsEGSLanguage.CodeType.OR_) & " " & rwIngredient("AlternativeIngredient").ToString.Trim & "]"
                        End If
                        ' Complement
                        If Not IsDBNull(rwIngredient("Complement")) And Not rwIngredient("Complement").ToString.Trim.Length = 0 Then
                            strIngrComplement = rwIngredient("Complement").ToString.Trim

                        End If
                        ' Preparation
                        If Not IsDBNull(rwIngredient("Preparation")) And Not rwIngredient("Preparation").ToString.Trim.Length = 0 Then
                            strIngrPreparation = rwIngredient("Preparation").ToString.Trim
                            blnDisplayPreparationHeader = True
                        End If

                        ' Combine all information to form 1 ingredient detail
                        ' RDC 12.02.2013 : Remove comma when there is no preparation present/defined
                        If blnAfterIngredient Then
                            If strItemName.Trim.Length >= 1 Then strIngredient &= strItemName & " "
                            If strIngrComplement.Trim.Length > 1 Then strIngredient &= strIngrComplement & " "
                        Else
                            If strIngrComplement.Trim.Length > 1 Then strIngredient &= strIngrComplement & " "
                            If strItemName.Trim.Length >= 1 Then strIngredient &= strItemName & " "
                        End If

                        'If strIngrComplement.Trim.Length > 1 Then strIngredient &= strIngrComplement & " "
                        'If strItemName.Trim.Length >= 1 Then strIngredient &= strItemName & " " ''AMTLA 2014.06.19 CWM-14647
                        If strAltIngredient.Trim.Length > 1 Then strIngredient &= strAltIngredient
                        If strIngrPreparation.Trim.Length > 1 Then strIngredient &= ", " & strIngrPreparation

                        ' Get alternate quantities for unvalidated ingredients
                        Dim dt As New DataTable



                        ' Get All quantities
                        ' For Metric and Imperial Quantities
                        Dim strMetricNet As String = "0", strMetricGross As String = "0", strMetricUnit As String = ""
                        Dim strImperialNet As String = "0", strImperialGross As String = "0", strImperialUnit As String = ""
                        ' For One Quantity
                        Dim strQtyNet As String = "0", strQtyGross As String = "0", strQtyUnit As String = ""
                        ' Total Wastage
                        Dim dblTotalWastage As Double = 0

                        If Not IsDBNull(rwIngredient("TotalWastage")) Then dblTotalWastage = CDbl(rwIngredient("TotalWastage"))
                        If rwIngredient("IngredientId") = 0 And rwIngredient("Type") = 0 Then
                            Dim dtqty As New DataTable
                            If Not rwIngredient("Quantity_Metric") Is Nothing Then
                                dtqty = getAlternateQuantity(rwIngredient("Quantity_Metric").ToString, rwIngredient("UOM_Metric"), intCodeTrans, intCodeSite)
                            Else
                                dtqty = getAlternateQuantity(rwIngredient("Quantity_Imperial").ToString, rwIngredient("UOM_Imperial"), intCodeTrans, intCodeSite)
                            End If

                            If dtqty.Rows.Count > 0 Then
                                For Each dr As DataRow In dtqty.Rows
                                    strMetricNet = dr("QtyMetric")
                                    strMetricGross = dr("QtyMetric")
                                    strMetricUnit = dr("UnitMetric")
                                    strImperialNet = dr("QtyImperial")
                                    strImperialGross = dr("QtyImperial")
                                    strImperialUnit = dr("UnitImperial")
                                Next
                            End If
                        Else
                            If Not IsDBNull(rwIngredient("Quantity_Metric")) Then
                                Dim metric_format As String = rwIngredient("UnitFormat").ToString

                                ' RDC 01.08.2014 : Display only in decimal form
                                'strMetricNet = ConvertDecimalToFraction2(rwIngredient("Quantity_Metric").ToString)
                                'strMetricGross = ConvertDecimalToFraction2(CDbl(rwIngredient("QtyMetricGross")))
                                'strMetricNet = Format(CDblDB(rwIngredient("Quantity_Metric").ToString), metric_format) 'Format(CDblDB(rwIngredient("Quantity_Metric").ToString), "##0.0#") ' RJL -  :02-14-2014
                                'strMetricGross = Format(CDblDB(rwIngredient("QtyMetricGross").ToString), metric_format) ' RJL -  :02-14-2014

                                strMetricNet = fctFormatNumericQuantity(CDblDB(rwIngredient("Quantity_Metric").ToString), metric_format, blnRemoveTrailingZeroes, 0)
                                strMetricGross = fctFormatNumericQuantity(CDblDB(rwIngredient("QtyMetricGross").ToString), metric_format, blnRemoveTrailingZeroes, 0)
                                ' RDC 11.27.2013 : Removed due to Sp reconstruction
                                'strMetricGross = fctConvertToFraction2(CDbl(rwIngredient("Quantity_Metric")) * CDbl(1 + (dblTotalWastage / 100)))
                            End If
                            If Not IsDBNull(rwIngredient("UOM_Metric")) Then
                                strMetricUnit = rwIngredient("UOM_Metric").ToString.Replace("n/a", "").Replace("[_]", "").Replace("N/A", "")
                            End If

                            If Not IsDBNull(rwIngredient("Quantity_Imperial")) Then
                                Dim imperial_format As String = rwIngredient("UnitFormat").ToString

                                If BlnConvertDecimaltoFraction = True Then
                                    strImperialNet = ConvertDecimalToFraction2(Format(CDblDB(rwIngredient("Quantity_Imperial").ToString), imperial_format))
                                    strImperialGross = ConvertDecimalToFraction2(Format(CDblDB(rwIngredient("QtyImperialGross").ToString), imperial_format))
                                Else
                                    strImperialNet = Format(CDblDB(rwIngredient("Quantity_Imperial").ToString), imperial_format)
                                    strImperialGross = Format(CDblDB(rwIngredient("QtyImperialGross").ToString), imperial_format)
                                End If

                                ' RDC 11.27.2013 : Removed due to Sp reconstruction
                                'strImperialGross = fctConvertToFraction2(CDbl(rwIngredient("Quantity_Imperial")) * CDbl(1 + (dblTotalWastage / 100)).ToString)
                            End If
                            If Not IsDBNull(rwIngredient("UOM_Imperial")) Then
                                strImperialUnit = rwIngredient("UOM_Imperial").ToString.Replace("n/a", "").Replace("[_]", "").Replace("N/A", "")
                            End If

                            If Not IsDBNull(rwIngredient("OneQtyNet")) Then

                                If BlnConvertDecimaltoFraction = True Then
                                    strQtyNet = ConvertDecimalToFraction2(rwIngredient("OneQtyNet"))
                                    strQtyGross = ConvertDecimalToFraction2(CDbl(rwIngredient("OneQtyGross")).ToString)
                                Else
                                    strQtyNet = rwIngredient("OneQtyNet")
                                    strQtyGross = CDbl(rwIngredient("OneQtyGross")).ToString
                                End If

                                ' RDC 11.27.2013 : Removed due to Sp reconstruction
                                'strQtyGross = fctConvertToFraction2(CDbl(rwIngredient("OneQtyNet")) * CDbl(1 + (dblTotalWastage / 100)).ToString)
                                strQtyUnit = rwIngredient("OneQtyUnit").ToString.Replace("n/a", "").Replace("[_]", "").Replace("N/A", "")
                            End If
                        End If



                        Dim intIncludedColumns As Integer = 0
                        If intItemType = 75 Then
                            Select Case bitUseOneQuantity
                                Case 0
                                    With G_ExportOptions
                                        If .blnExpIncludeImperialNetQty Then intIncludedColumns += 1
                                        If .blnExpIncludeImperialGrossQty Then intIncludedColumns += 1
                                        If .blnExpIncludeMetricNetQty Then intIncludedColumns += 1
                                        If .blnExpIncludeMetricGrossQty Then intIncludedColumns += 1
                                        intIncludedColumns += 1
                                    End With
                                Case 1
                                    With G_ExportOptions
                                        If .blnExpIncludeNetQty Then intIncludedColumns += 1
                                        If .blnExpIncludeGrossQty Then intIncludedColumns += 1
                                        intIncludedColumns += 1
                                    End With
                            End Select

                            strHTMLContent.Append("<td width ='100%' colspan='" & intIncludedColumns & "' valign='top'><b>" & strIngredient & "</b></td>")
                        Else
                            Dim intColSize As Integer = 100
                            Select Case bitUseOneQuantity
                                Case 0
                                    With G_ExportOptions
                                        If .blnExpIncludeImperialNetQty Then intIncludedColumns += 1
                                        If .blnExpIncludeImperialGrossQty Then intIncludedColumns += 1
                                        If .blnExpIncludeMetricNetQty Then intIncludedColumns += 1
                                        If .blnExpIncludeMetricGrossQty Then intIncludedColumns += 1
                                        intIncludedColumns += 1
                                    End With
                                Case 1
                                    With G_ExportOptions
                                        If .blnExpIncludeNetQty Then intIncludedColumns += 1
                                        If .blnExpIncludeGrossQty Then intIncludedColumns += 1
                                        intIncludedColumns += 1
                                    End With
                            End Select

                            ' RDC 12.18.2013 : Added best unit conversion for unvalidated ingredients
                            Dim intUnitCode As Integer = -1, intIsImperialMetric As Integer = 9, strUnitFormat As String = "", dblUnitFactor As Decimal = 0D, intTypeMain As Integer = 0

                            Dim strUnvalidatedMetricQty As String = strMetricNet, strUnvalidatedMetricUnit As String = strMetricUnit
                            Dim strUnvalidatedImperialQty As String = strImperialNet, strUnvalidatedImperialUnit As String = strImperialUnit

                            Select Case bitUseOneQuantity
                                Case 0 ' Display Metric/Imperial Gross/Net quantities   

                                    If G_ExportOptions.blnExpIncludeImperialNetQty Then
                                        If rwIngredient("Type") = 0 And rwIngredient("IngredientId") = 0 Then
                                            strHTMLContent.Append("<td width='110' valign='top'>")
                                            ' RDC 11.26.2013 : Do not display if quantity is zero
                                            If Not strUnvalidatedImperialQty = "0" Then

                                                If BlnConvertDecimaltoFraction = True Then
                                                    strHTMLContent.Append(ConvertDecimalToFraction2(fctCheckDbNullNumeric(strUnvalidatedImperialQty)) & " " & strUnvalidatedImperialUnit.Replace("_", " "))
                                                Else
                                                    strHTMLContent.Append(fctCheckDbNullNumeric(strUnvalidatedImperialQty) & " " & strUnvalidatedImperialUnit.Replace("_", " "))
                                                End If


                                            Else
                                                'strHTMLContent.Append(strUnvalidatedImperialUnit.Replace("_", " "))
                                                strHTMLContent.Append(" ")
                                            End If

                                            strHTMLContent.Append("</td>")
                                            intRemainingSize -= 110
                                        Else
                                            strHTMLContent.Append("<td width='110' valign='top'>")
                                            ' RDC 11.26.2013 : Do not display if quantity is zero
                                            If Not rwIngredient("type") = 4 Then ' RJL -  :02-17-2014
                                                If Not strImperialNet = "0" Then strHTMLContent.Append(strImperialNet & " " & strImperialUnit.Replace("_", " ")) Else strHTMLContent.Append(" ") 'strHTMLContent.Append(strImperialUnit.Replace("_", " "))
                                            End If

                                            strHTMLContent.Append("</td>")
                                            intRemainingSize -= 110
                                        End If

                                    End If

                                    If G_ExportOptions.blnExpIncludeImperialGrossQty Then
                                        If rwIngredient("Type") = 0 And rwIngredient("IngredientId") = 0 Then
                                            strHTMLContent.Append("<td width='110' valign='top'>")
                                            ' RDC 11.26.2013 : Do not display if quantity is zero
                                            If Not strUnvalidatedImperialQty = "0" Then

                                                If BlnConvertDecimaltoFraction = True Then
                                                    strHTMLContent.Append(ConvertDecimalToFraction2(fctCheckDbNullNumeric(strUnvalidatedImperialQty)) & " " & strUnvalidatedImperialUnit.Replace("_", " "))
                                                Else
                                                    strHTMLContent.Append(fctCheckDbNullNumeric(strUnvalidatedImperialQty) & " " & strUnvalidatedImperialUnit.Replace("_", " "))
                                                End If

                                            Else
                                                strHTMLContent.Append(" ")
                                            End If
                                            'strHTMLContent.Append(strUnvalidatedImperialUnit.Replace("_", " "))
                                            strHTMLContent.Append("</td>")
                                            intRemainingSize -= 110
                                        Else
                                            strHTMLContent.Append("<td width='110' valign='top'>")
                                            ' RDC 11.26.2013 : Do not display if quantity is zero
                                            If Not rwIngredient("type") = 4 Then ' RJL -  :02-17-2014
                                                If Not strImperialGross = "0" Then strHTMLContent.Append(strImperialGross & " " & strImperialUnit.Replace("_", " ")) Else strHTMLContent.Append(" ") 'strHTMLContent.Append(strImperialUnit.Replace("_", " "))
                                            End If
                                            strHTMLContent.Append("</td>")
                                            intRemainingSize -= 110
                                        End If

                                    End If

                                    If G_ExportOptions.blnExpIncludeMetricNetQty Then
                                        If rwIngredient("Type") = 0 And rwIngredient("IngredientId") = 0 Then
                                            strHTMLContent.Append("<td width='110' valign='top'>")
                                            ' RDC 11.26.2013 : Do not display if quantity is zero
                                            If Not strUnvalidatedMetricQty = "0" Then strHTMLContent.Append(strUnvalidatedMetricQty & " " & strUnvalidatedMetricUnit.Replace("_", " ")) Else strHTMLContent.Append(" ") 'strHTMLContent.Append(strUnvalidatedMetricUnit.Replace("_", " "))
                                            strHTMLContent.Append("</td>")
                                            intRemainingSize -= 110
                                        Else
                                            strHTMLContent.Append("<td width='110' valign='top'>")
                                            ' RDC 11.26.2013 : Do not display if quantity is zero
                                            If Not rwIngredient("type") = 4 Then ' RJL -  :02-17-2014
                                                If Not strMetricNet = "0" Then strHTMLContent.Append(strMetricNet & " " & strMetricUnit.Replace("_", " ")) Else strHTMLContent.Append(" ") 'strHTMLContent.Append(strMetricUnit.Replace("_", " "))
                                            End If
                                            strHTMLContent.Append("</td>")
                                            intRemainingSize -= 110
                                        End If

                                    End If

                                    If G_ExportOptions.blnExpIncludeMetricGrossQty Then
                                        If rwIngredient("Type") = 0 And rwIngredient("IngredientId") = 0 Then
                                            strHTMLContent.Append("<td width='110' valign='top'>")
                                            ' RDC 11.26.2013 : Do not display if quantity is zero
                                            If Not strUnvalidatedMetricQty = "0" Then strHTMLContent.Append(strUnvalidatedMetricQty & " " & strUnvalidatedMetricUnit.Replace("_", " ")) Else strHTMLContent.Append(" ") 'strHTMLContent.Append(strUnvalidatedMetricUnit.Replace("_", " "))
                                            strHTMLContent.Append("</td>")
                                            intRemainingSize -= 110
                                        Else
                                            strHTMLContent.Append("<td width='110' valign='top'>")
                                            ' RDC 11.26.2013 : Do not display if quantity is zero
                                            If Not rwIngredient("type") = 4 Then ' RJL -  :02-17-2014
                                                If Not strMetricGross = "0" Then strHTMLContent.Append(strMetricGross & " " & strMetricUnit.Replace("_", " ")) Else strHTMLContent.Append(" ") 'strHTMLContent.Append(strMetricUnit.Replace("_", " "))
                                            End If
                                            strHTMLContent.Append("</td>")
                                            intRemainingSize -= 110
                                        End If
                                    End If

                                Case 1 ' Display Gross and Net Quantities only

                                    If G_ExportOptions.blnExpIncludeNetQty Then
                                        strHTMLContent.Append("<td width='125' valign='top'>")
                                        ' RDC 11.26.2013 : Do not display if quantity is zero
                                        If Not rwIngredient("type") = 4 Then ' RJL -  :02-17-2014
                                            If Not strQtyNet = "0" Then strHTMLContent.Append(strQtyNet & " " & strQtyUnit) Else strHTMLContent.Append(" ") 'strHTMLContent.Append(strQtyUnit)
                                        End If
                                        strHTMLContent.Append("</td>")
                                        intRemainingSize -= 125
                                    End If

                                    If G_ExportOptions.blnExpIncludeGrossQty Then
                                        strHTMLContent.Append("<td width='125' valign='top'>")
                                        ' RDC 11.26.2013 : Do not display if quantity is zero
                                        If Not rwIngredient("type") = 4 Then ' RJL -  :02-17-2014
                                            If Not strQtyGross = "0" Then strHTMLContent.Append(strQtyGross & " " & strQtyUnit) Else strHTMLContent.Append(" ") 'strHTMLContent.Append(strQtyUnit)
                                        End If
                                        strHTMLContent.Append("</td>")
                                        intRemainingSize -= 125
                                    End If
                                Case Else
                            End Select

                            ' Ingredient name
                            ' RDC 11.29.2013 : Make steps in bold caption.
                            strHTMLContent.Append("<td width='" & intRemainingSize & "' valign='top' style='word-wrap:break-word;'>")
                            strHTMLContent.Append(strIngredient)
                            strHTMLContent.Append("</td></tr>")

                        End If

                        blnDisplayPreparationHeader = True
                    Next
                    strHTMLContent.Append("</table>")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                Else
                    blnDisplayPreparationHeader = False
                    blnDisplayAdditionalNotes = False

                End If
                strHTMLContent.Append("<tr>")
                strHTMLContent.Append("<td>")
                strHTMLContent.Append("&nbsp;")
                strHTMLContent.Append("</td>")
                strHTMLContent.Append("</tr>")

                ' RDC 11.14.2013 : Option to display or not to display Procedure/preparation

                If G_ExportOptions.intExpSelectedProcedure = 0 Then
                    strMethodHeader = cLang.GetString(clsEGSLanguage.CodeType.PreparationMethod)
                Else
                    strMethodHeader = cLang.GetString(clsEGSLanguage.CodeType.CookMode)
                End If

                If G_ExportOptions.blnExpIncludeProcedure Then
                    Select Case G_ExportOptions.intExpSelectedProcedure
                        Case 0
                            'Method Header
                            If strMethodHeader.ToString <> "" And blnDisplayPreparationHeader Then
                                strHTMLContent.Append("<tr>")
                                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; font-weight: bold'>")
                                strHTMLContent.Append(strMethodHeader.ToString)
                                strHTMLContent.Append("</td>")
                                strHTMLContent.Append("</tr>")
                            End If
                            '' Case 1
                            'Directions
                            If strDirections.ToString <> "" Then
                                strHTMLContent.Append("<tr>")
                                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                                'strHTMLContent.Append(strDirections.ToString.Replace(Chr(13) & Chr(10), "<br>")) ' TDQ 11.14.2011
                                strHTMLContent.Append(strDirections.ToString)
                                strHTMLContent.Append("</td>")
                                strHTMLContent.Append("</tr>")
                            End If
                        Case Else
                            'Method Header
                            If strMethodHeader.ToString <> "" And blnDisplayPreparationHeader Then
                                strHTMLContent.Append("<tr>")
                                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; font-weight: bold'>")
                                strHTMLContent.Append(strMethodHeader.ToString)
                                strHTMLContent.Append("</td>")
                                strHTMLContent.Append("</tr>")
                            End If

                            'Directions
                            If strAbbrDirections.ToString <> "" Then
                                strHTMLContent.Append("<tr>")
                                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                                'strHTMLContent.Append(strDirections.ToString.Replace(Chr(13) & Chr(10), "<br>")) ' TDQ 11.14.2011
                                strHTMLContent.Append(strAbbrDirections.ToString)
                                strHTMLContent.Append("</td>")
                                strHTMLContent.Append("</tr>")
                            End If
                    End Select

                End If

                strHTMLContent.Append("</table>")
                strHTMLContent.Append("</td>")
                strHTMLContent.Append("</tr>")

                'If dsRecipeDetails.Tables(1).Rows(0).Item("Note").ToString <> "" Then ' RJL - 11756 :02-17-2014
                blnDisplayNotesHeader = IIf(strFootNote1 <> "", True, False) ' RJL - 12798 :03-11-2014
                blnDisplayAdditionalNotes = IIf(strFootNote2 <> "", True, False) ' RJL - 12798 :03-11-2014
                'Else
                '    blnDisplayNotesHeader = False
                '    blnDisplayAdditionalNotes = False
                'End If

                ''AMTLA 2013.11.19 Added Header to Notes
                If G_ExportOptions.blnExpIncludeNotes And blnDisplayNotesHeader Then
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td>")
                    strHTMLContent.Append("<table style='width: 620'>")
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; font-weight: bold'>")
                    strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.Notes))
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strFootNote1.ToString.Replace(Chr(13) & Chr(10), "<br>"))
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                    strHTMLContent.Append("</table>")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                End If

                ''AMTLA 2013.11.19 Added Header to Additonal Notes
                If G_ExportOptions.blnExpIncludeAddNotes And blnDisplayAdditionalNotes Then
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td>")
                    strHTMLContent.Append("<table style='width: 620'>")
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; font-weight: bold'>")
                    strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.AdditionalNotes))
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strFootNote2.ToString.Replace(Chr(13) & Chr(10), "<br>"))
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                    strHTMLContent.Append("</table>")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                End If

                ''Nutrients
                'If isDisplay = True Then
                '    If dsRecipeDetails.Tables("Table4").Rows.Count > 0 Then
                '        strHTMLContent.Append("<p style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
                '        '-- JBB 02.23.2012
                '        Dim strNutBasis As String = fctCheckDbNull(dsRecipeDetails.Tables("Table4").Rows(0).Item("NutritionBasis"))
                '        If lblNutritionalInformation.ToString().Trim <> "" Then '-- JBB 02.23.2012
                '            If strNutBasis = "" Then
                '                strHTMLContent.Append(lblNutritionalInformation.ToString & " :")
                '            Else
                '                strHTMLContent.Append(lblNutritionalInformation.ToString & "(" & strNutBasis & ") :")
                '            End If
                '        End If
                '        'strHTMLContent.Append(lblNutritionalInformation.ToString)
                '        '-- 
                '        strHTMLContent.Append("</p>")
                '        strHTMLContent.Append(strNutrients.ToString)
                '    End If

                '    'Net Carbs
                '    If strNetCarbohydrates.ToString <> "" Then
                '        strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
                '        strHTMLContent.Append(strNetCarbohydrates.ToString)
                '        strHTMLContent.Append("</p>")
                '        strHTMLContent.Append(lblNetCarbs.ToString)
                '    End If
                'End If

                ' Nutrients
                ' RDC 11.14.2013 : Move nutrients section to its right place as presented on the specs
                If G_ExportOptions.blnExpIncludeNutrientInfo Then
                    Dim strNutBasis As String = ""
                    If Not IsDBNull(dsRecipeDetails.Tables("Table4").Rows(0).Item("NutritionBasis")) Then strNutBasis = dsRecipeDetails.Tables("Table4").Rows(0).Item("NutritionBasis")
                    strHTMLContent.Append(fctDisplayNutrientComputationForExport(m_RecipeId, dsRecipeDetails.Tables(1).Rows(0).Item("ServingsUnit").ToString, G_ExportOptions.intExpSelectedNutrientSet, G_ExportOptions.intExpSelectedLanguage, G_ExportOptions.intExpSelectedNutrientComputation, , strNutBasis, m_Version, True))

                    'Net Carbs
                    If isDisplay = True Then
                        If strNetCarbohydrates.ToString <> "" Then
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td>")
                            strHTMLContent.Append("<table style='width: 620'>")
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
                            strHTMLContent.Append(strNetCarbohydrates.ToString)
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                            strHTMLContent.Append(lblNetCarbs.ToString)
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")
                            strHTMLContent.Append("</table>")
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")
                        End If

                    End If
                End If

                ' RDC 02.11.2014 : GDA
                If G_ExportOptions.blnExpIncludeGDA Then
                    strHTMLContent.Append(fctDisplayGDAComputationForExport(m_RecipeId, dsRecipeDetails.Tables(1).Rows(0).Item("ServingsUnit").ToString, G_ExportOptions.intExpSelectedNutrientSet, G_ExportOptions.intExpSelectedLanguage, G_ExportOptions.intExpSelectedGDA, , "", m_Version, True))
                End If

                strHTMLContent.Append("<tr>")
                strHTMLContent.Append("<td>")
                strHTMLContent.Append("&nbsp;")
                strHTMLContent.Append("</td>")
                strHTMLContent.Append("</tr>")

                'Information
                ' RDC 11.13.2013 : Option to display or not to display Information
                If G_ExportOptions.blnExpAdvIncludeInfo Then
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td>")

                    strHTMLContent.Append("<table style='width: 620'>")
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td colspan='2' style='text-align: center; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                    'strHTMLContent.Append(lblInformation.ToString)
                    strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.Information))
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td style='vertical-align: top;'>")
                    strHTMLContent.Append("<table style='width: 500'>")
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                    'strHTMLContent.Append(lblRecipeStatus.ToString)
                    strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.RecipeStatus))
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strRecipeStatus.ToString)
                    strHTMLContent.Append("</td>")
                    'strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
                    'strHTMLContent.Append("&nbsp;")
                    'strHTMLContent.Append("</td>")
                    'AGL 2012.10.31 - CWM-1971
                    If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
                        'AGL 2013.05.16 - removed width
                        'strHTMLContent.Append("<td style='text-align: right; width: 100; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        'strHTMLContent.Append(lblUpdatedBy.ToString)
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.UpdatedBy))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strUpdatedBy.ToString)
                        strHTMLContent.Append("</td>")
                    End If
                    strHTMLContent.Append("</tr>")



                    'AGL 2012.10.31 - CWM-1971
                    If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
                        strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        'strHTMLContent.Append(lblWebStatus.ToString)
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.WebStatus))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strWebStatus.ToString)
                        strHTMLContent.Append("</td>")
                        'strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
                        'strHTMLContent.Append("&nbsp;")
                        'strHTMLContent.Append("</td>")
                        'strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        'strHTMLContent.Append("&nbsp;")
                        'strHTMLContent.Append("</td>")
                        'strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        'strHTMLContent.Append("&nbsp;")
                        'strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")

                    End If
                    'AGL 2013.05.06 - 4728 - brought out Date Created
                    strHTMLContent.Append("<tr>")

                    strHTMLContent.Append("</tr>")

                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                    'strHTMLContent.Append(lblDateCreated.ToString)
                    strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.DateCreated))
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strDateCreated.ToString)
                    strHTMLContent.Append("</td>")
                    'strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
                    'strHTMLContent.Append("&nbsp;")
                    'strHTMLContent.Append("</td>")

                    'AGL 2013.05.16 - removed width
                    'strHTMLContent.Append("<td style='text-align: right; width: 100; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                    'strHTMLContent.Append(lblCreatedBy.ToString)
                    strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.CreatedBY))
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strCreatedBy.ToString)
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")



                    strHTMLContent.Append("<tr>")
                    'AGL 2012.10.31 - CWM-1971
                    'If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
                    strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                    'strHTMLContent.Append(lblDateLastModified.ToString)
                    strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.DateLastModified))
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strDateLastModified.ToString)
                    strHTMLContent.Append("</td>")
                    'strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
                    'strHTMLContent.Append("&nbsp;")
                    'strHTMLContent.Append("</td>")
                    'End If
                    'AGL 2013.05.16 - removed width
                    'strHTMLContent.Append("<td style='text-align: right; width: 100; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                    'strHTMLContent.Append(lblModifiedBy.ToString)
                    strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.ModifiedBy))
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strModifiedBy.ToString)
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                    'AGL 2012.10.31 - CWM-1971
                    If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        'strHTMLContent.Append(lblLastTested.ToString)\
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.DateLastTested))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strLastTested.ToString)
                        strHTMLContent.Append("</td>")
                        'strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
                        'strHTMLContent.Append("&nbsp;")
                        'strHTMLContent.Append("</td>")

                        'AGL 2013.05.16 - removed width
                        'strHTMLContent.Append("<td style='text-align: right; width: 100; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        'strHTMLContent.Append(lblTestedBy.ToString)
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.TestedBy))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strTestedBy.ToString)
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        'strHTMLContent.Append(lblDateDeveloped.ToString)
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.DateDeveloped))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strDateDeveloped.ToString)
                        strHTMLContent.Append("</td>")
                        'strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
                        'strHTMLContent.Append("&nbsp;")
                        'strHTMLContent.Append("</td>")
                        'AGL 2013.05.16 - removed width
                        'strHTMLContent.Append("<td style='text-align: right; width: 100; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        'strHTMLContent.Append(lblDevelopedBy.ToString)
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.DateDeveloped))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strDevelopedBy.ToString)
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        'strHTMLContent.Append(lblDateOfFinalEdit.ToString)
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.FinalEditDate))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strDateOfFinalEdit.ToString)
                        strHTMLContent.Append("</td>")
                        'strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
                        'strHTMLContent.Append("&nbsp;")
                        'strHTMLContent.Append("</td>")
                        'AGL 2013.05.16 - removed width
                        'strHTMLContent.Append("<td style='text-align: right; width: 100; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        'strHTMLContent.Append(lblFinalEditBy.ToString)
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.FinalEditBy))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strFinalEditBy.ToString)
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri; vertical-align: top; width:200'>")
                        'strHTMLContent.Append(lblDevelopmentPurpose.ToString)
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.DevelopmentPurpose))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; vertical-align: top;' colspan=4>")
                        strHTMLContent.Append(strDevelopmentPurpose.ToString)
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                    End If
                    strHTMLContent.Append("</table>")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                    strHTMLContent.Append("</table>")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                End If

                'Recipe Brand
                ' RDC 11.13.2013 : Option to display or not to display Brand
                If G_ExportOptions.blnExpAdvIncludeBrands Then
                    If dsRecipeDetails.Tables("table8").Rows.Count > 0 Then
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td>")
                        strHTMLContent.Append("<table style='width: 620'>")
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: center' colspan='2'>")
                        'strHTMLContent.Append(lblRecipeBrand.ToString)
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.Brand))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")

                        For Each Brands As DataRow In dsRecipeDetails.Tables("table8").Rows
                            strRecipeBrand = fctCheckDbNull(Brands.Item("BrandName"))
                            strRecipeBrandClassification = fctCheckDbNull(Brands.Item("BrandClassification"))
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                            'AGL 2012.10.31 - CWM-1971
                            ' RDC 11.15.2013 : Added Or clsLicense.l_App = EgswKey.clsLicense.enumApp.RB
                            If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Or clsLicense.l_App = EgswKey.clsLicense.enumApp.RB Then
                                strHTMLContent.Append(strRecipeBrand.ToString & " - " & strRecipeBrandClassification.ToString)
                            Else
                                strHTMLContent.Append(strRecipeBrand.ToString)
                            End If

                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")
                        Next

                        strHTMLContent.Append("</table>")
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                    End If
                End If

                'Attributes
                ' RDC 11.13.2013 : Option to display or not to display Attributes/Keywords
                If G_ExportOptions.blnExpAdvIncludeKeywords Then
                    If dsRecipeDetails.Tables("table7").Rows.Count > 0 Then
                        Dim lngCode As Long
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td>")
                        strHTMLContent.Append("<table style='width: 620'>")
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='font-weight: bold; text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                        ' RDC 11.14.2013 : Replaced by Keywords instead of Attributes
                        'strHTMLContent.Append(lblAttributes.ToString)
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.Keywords))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")

                        ' RDC 12.02.2013 : Replaced code below.
                        For Each drKeywords As DataRow In dsRecipeDetails.Tables("Table7").Rows
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                            strHTMLContent.Append(drKeywords("Name"))
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")
                        Next
                        strHTMLContent.Append("</table>")
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")

                        '                        For Each Attributes In dsRecipeDetails.Tables("table7").Rows
                        '                            If Not IsDBNull(Attributes("Main")) Then
                        '                                lngCode = Attributes("Parent")
                        '                                strAttributes = fctCheckDbNull(Attributes("Name"))

                        'ReIdentifyParent:

                        '                                For Each Parents In dsRecipeDetails.Tables("table7").Select("Code = " & lngCode)
                        '                                    strAttributes = fctCheckDbNull(Parents.Item("Name")) & " : " & strAttributes
                        '                                    If Parents("Parent") > 0 Then
                        '                                        lngCode = fctCheckDbNull(Parents.Item("Parent"))
                        '                                        GoTo ReIdentifyParent
                        '                                    Else
                        '                                        Exit For
                        '                                    End If
                        '                                Next

                        '                                strHTMLContent.Append("<tr>")
                        '                                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        '                                strHTMLContent.Append(strAttributes.ToString)
                        '                                strHTMLContent.Append("</td>")
                        '                                strHTMLContent.Append("</tr>")
                        '                            End If
                        '                        Next
                        '                        strHTMLContent.Append("</td>")
                        '                        strHTMLContent.Append("</tr>")
                        '                        strHTMLContent.Append("</table>")
                        '                        strHTMLContent.Append("</td>")
                        '                        strHTMLContent.Append("</tr>")
                    End If
                End If

                ' Cookbooks
                ' RDC 11.14.2013 : Adding cookbooks section to the report
                If G_ExportOptions.blnExpAdvIncludeCookbook Then
                    If dsRecipeDetails.Tables(10).Rows.Count > 0 Then
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td>")
                        strHTMLContent.Append("<table style='width: 620'>")
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='font-weight: bold; text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.Cookbook))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")

                        For Each rwCookbooks As DataRow In dsRecipeDetails.Tables(10).Rows
                            strHTMLContent.Append("<tr><td style='font-size: 11.5pt; font-family: Calibri;'>")
                            strHTMLContent.Append(rwCookbooks("Name").ToString)
                            strHTMLContent.Append("</td></tr>")
                        Next

                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                        strHTMLContent.Append("</table>")
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")

                    End If
                End If

                'Placements
                ' RDC 11.13.2013 : Option to display or not to display Remark
                If G_ExportOptions.blnExpAdvIncludePublication Then
                    If dsRecipeDetails.Tables("table9").Rows.Count > 0 Then
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td>")
                        strHTMLContent.Append("<table style='width: 620'>")
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: center' colspan='3'>")
                        strHTMLContent.Append(lblPlacements.ToString)
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td>")
                        strHTMLContent.Append("<table>")
                        For Each Placements As DataRow In dsRecipeDetails.Tables("table9").Rows
                            strPlacementName = fctCheckDbNull(Placements.Item("Placement"))
                            If Not IsDBNull(Placements.Item("PlacementDate")) Then strPlacementDate = CDate(Placements.Item("PlacementDate")).ToString("MM/dd/yyyy")
                            strPlacementDescription = fctCheckDbNull(Placements.Item("Description"))

                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; padding-right: 10px'>")
                            strHTMLContent.Append(strPlacementName.ToString)
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; padding-right: 10px'>")
                            strHTMLContent.Append(strPlacementDate.ToString)
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; padding-right: 10px'>")
                            strHTMLContent.Append(strPlacementDescription.ToString)
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")
                        Next
                        strHTMLContent.Append("</table>")
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                        strHTMLContent.Append("</table>")
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                    End If
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td>")
                    strHTMLContent.Append("&nbsp;")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                End If

                'Comments
                ' RDC 11.13.2013 : Option to display or not to display Comments
                If G_ExportOptions.blnExpAdvIncludeComments Then
                    If dsRecipeDetails.Tables("table6").Rows.Count > 0 Then
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td>")
                        strHTMLContent.Append("<table style='width: 620'>")
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='font-weight: bold; text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                        'strHTMLContent.Append(lblComments.ToString)
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.Comments))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td>")
                        strHTMLContent.Append("<table>")
                        strHTMLContent.Append("<p style='padding-right: 0px; font-size: 11.5pt; font-family: Calibri; vertical-align: top;'>")
                        For Each Comments As DataRow In dsRecipeDetails.Tables("table6").Rows
                            If Not IsDBNull(Comments.Item("SubmitDate")) Then strSubmitDate = CDate(Comments.Item("SubmitDate")).ToString("MM/dd/yyyy")
                            strOwnerName = fctCheckDbNull(Comments.Item("OwnerName"))
                            strComments = fctCheckDbNull(Comments.Item("Description"))
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td style='padding-right: 10px; font-size: 11.5pt; font-family: Calibri; vertical-align: top;'>")
                            strHTMLContent.Append(strSubmitDate.ToString)
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("<td style='padding-right: 0px; font-size: 11.5pt; font-family: Calibri; vertical-align: top; width: 130px;'>")
                            strHTMLContent.Append(strOwnerName.ToString)
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("<td style='padding-right: 10px; font-size: 11.5pt; font-family: Calibri; vertical-align: top;'>")
                            strHTMLContent.Append(strComments.ToString)
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")
                        Next
                        strHTMLContent.Append("</table>")
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                        strHTMLContent.Append("</table>")
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                    End If
                End If
                strHTMLContent.Append("</table>")
            Else
                'strHTMLContent.Append("<table style='width: 620'>")
                'strHTMLContent.Append("<tr>")
                'strHTMLContent.Append("<td>")
                strHTMLContent.Append("<table style='width: 620'>")

                'Recipe Number
                ' RDC 11.14.2013 : Added condition to display or not to display Recipe Number
                If G_ExportOptions.blnExpIncludeRecipeNo Then
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td>")
                    strHTMLContent.Append("<table>")
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: FCalibri;'>")
                    strHTMLContent.Append(lblRecipeNumber.ToString)
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strRecipeNumber.ToString)
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                    strHTMLContent.Append("</table>")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td>")
                    strHTMLContent.Append("&nbsp;")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                End If

                'Sub Title
                ' RDC 11.14.2013 : Added condition to display or not to display Sub name
                If G_ExportOptions.blnExpIncludeSubName Then
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td>")
                    strHTMLContent.Append("<table>")
                    strHTMLContent.Append("<tr>")

                    strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(lblSubTitle.ToString)
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strSubTitle.ToString)
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                    strHTMLContent.Append("</table>")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")

                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td>")
                    strHTMLContent.Append("&nbsp;")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                    strHTMLContent.Append("</table>")
                End If

                'Image
                imgRecipe = strImage
                strHTMLContent.Append("<p style='font-weight: bold; font-size: x-large; text-align: center; font-family: Calibri;'>")
                strHTMLContent.Append(imgRecipe) 'CMV 051911

                If Not strImage.Contains("nopic.jpg") And Not strImage2.Contains("nopic.jpg") Then
                    If Not strImage2 = "" Then
                        'strHTMLContent.Append(" <td style='text-align: center'>")
                        strHTMLContent.Append("&nbsp")
                        imgRecipe2 = strImage2 ' getHtml(imageRecipe) 'CMV 051911
                        strHTMLContent.Append(imgRecipe2) 'CMV 051911
                        'strHTMLContent.Append("</td>")
                    End If
                End If

                strHTMLContent.Append("</p>")





                'Recipe Name
                strHTMLContent.Append("<p style='font-weight: bold; font-size: x-large; text-align: center; font-family: Calibri;'>")
                strHTMLContent.Append(strRecipeName.ToString)
                strHTMLContent.Append("</p>")

                'Subheading
                strHTMLContent.Append("<p style='font-weight: bold; text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                strHTMLContent.Append(strSubHeading.ToString)
                strHTMLContent.Append("</p>")

                'Servings
                ' RDC 11.14.2013 : Added condition to display or not to display Yield1, Yield2 and Sub Recipe weight
                If G_ExportOptions.blnExpIncludeYield1 Or G_ExportOptions.blnExpIncludeYield2 Or G_ExportOptions.blnExpSubRecipeWt Then
                    strHTMLContent.Append("<p style='text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strServings.ToString)
                    strHTMLContent.Append("</p>")
                End If


                'Recipe Time
                ' RDC 11.14.2013 : Added condition to display or not to display Recipe Time
                If G_ExportOptions.blnExpIncludeRecipeTime Then
                    strHTMLContent.Append("<p style='text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                    For Each RecipeTime As DataRow In dsRecipeDetails.Tables("Table5").Rows
                        strRecipeTime = RecipeTime.Item("Description")
                        strHTMLContent.Append(strRecipeTime.ToString)
                        strHTMLContent.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")

                    Next
                    strHTMLContent.Append("</p>")
                End If


                'Ingredients
                If dsRecipeDetails.Tables("Table1").Rows.Count > 0 Then
                    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")
                    '-- JBB 05.24.2011 (code pass by Cielo)
                    For Each Ingredients As DataRow In dsRecipeDetails.Tables("Table1").Rows
                        If Ingredients.Item("Type").ToString().Trim() <> "4" Then
                            '    strIngredients = fctCheckDbNull(Ingredients.Item("Description"))
                            'Else

                            If bitQtyFormat = 0 Then
                                'strIngredients = fctCheckDbNullNumeric(Ingredients.Item("Quantity_Metric")) & " " & fctCheckDbNull(Ingredients.Item("UOM_Metric")) & " " & fctCheckDbNull(Ingredients.Item("Complement")) & " " & fctCheckDbNull(Ingredients.Item("Name")) & "," & fctCheckDbNull(Ingredients.Item("Preparation"))
                                ''-- JBB 10.26.2011
                                'If fctCheckDbNull(Ingredients.Item("AlternativeIngredient")) <> "" Then
                                '    strIngredients = strIngredients & " or " & fctCheckDbNull(Ingredients.Item("AlternativeIngredient"))
                                'End If

                                'TDQ 11022011
                                If fctCheckDbNull(Ingredients.Item("AlternativeIngredient")) = "" Then
                                    strIngredients = IIf(Ingredients.Item("Quantity_Metric") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Metric"))) & " " & _
                                        fctCheckDbNull(Ingredients.Item("UOM_Metric")) & " " & _
                                        fctCheckDbNull(Ingredients.Item("Complement") & " " & _
                                        fctCheckDbNull(Ingredients.Item("Name") & IIf(AutoSpacing = True, " ", "") & _
                                        fctCheckDbNull(Ingredients.Item("Preparation"))))
                                Else
                                    strIngredients = IIf(Ingredients.Item("Quantity_Metric") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Metric"))) & " " & _
                                        fctCheckDbNull(Ingredients.Item("UOM_Metric")) & " " & _
                                        fctCheckDbNull(Ingredients.Item("Complement") & " " & _
                                        fctCheckDbNull(Ingredients.Item("Name") & " &#91or " & _
                                        fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(AutoSpacing = True, " ", "") & _
                                        fctCheckDbNull(Ingredients.Item("Preparation")))))
                                End If

                                'If fctCheckDbNull(Ingredients.Item("Preparation")) = "" Then 'TDQ 10172011
                                '    strIngredients = strIngredients.Substring(0, strIngredients.Length - 1)
                                'End If

                                '-- JBB 10.25.2011
                                ' strIngredients = strIngredients.Replace("0 N/A", "")
                                ' strIngredients = strIngredients.Replace("0 n/a", "")
                                strIngredients = strIngredients.Replace("N/A", "")
                                strIngredients = strIngredients.Replace("n/a", "")
                                strIngredients = strIngredients + "<br>"

                                '--


                            ElseIf bitQtyFormat = 1 Then
                                If IsNumeric(Ingredients.Item("Quantity_Imperial")) = True Then
                                    If fctCheckDbNull(Ingredients.Item("AlternativeIngredient")) = "" Then 'TDQ 10172011
                                        If blnUseFractions Then
                                            strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctConvertToFraction2(fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial")))) & " " & _
                                                fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " & _
                                                fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & IIf(AutoSpacing = True, " ", "") & fctCheckDbNull(Ingredients.Item("Preparation")))) 'TDQ 10142011
                                        Else
                                            strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial"))) & " " & _
                                                fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " & _
                                                fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & IIf(AutoSpacing = True, " ", "") & fctCheckDbNull(Ingredients.Item("Preparation")))) 'TDQ 10142011
                                        End If
                                    Else
                                        If blnUseFractions Then
                                            strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctConvertToFraction2(fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial")))) & " " & _
                                                fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " & _
                                                fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & " &#91or " & fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(AutoSpacing = True, " ", "") & fctCheckDbNull(Ingredients.Item("Preparation"))))) 'TDQ 10142011
                                        Else
                                            strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial"))) & " " & _
                                                fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " & _
                                                fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & " &#91or " & fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(AutoSpacing = True, " ", "") & fctCheckDbNull(Ingredients.Item("Preparation"))))) 'TDQ 10142011
                                        End If



                                    End If
                                    '-- JBB 10.25.2011
                                    'strIngredients = strIngredients.Replace("0 N/A", "")
                                    'strIngredients = strIngredients.Replace("0 n/a", "")
                                    strIngredients = strIngredients.Replace("N/A", "")
                                    strIngredients = strIngredients.Replace("n/a", "")

                                    '--
                                Else
                                    If fctCheckDbNull(Ingredients.Item("AlternativeIngredient")) = "" Then 'TDQ 10172011
                                        strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNull(Ingredients.Item("Quantity_Imperial"))) & " " & _
                                            fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " &
                                            fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & IIf(AutoSpacing = True, " ", "") & fctCheckDbNull(Ingredients.Item("Preparation")))) 'TDQ 10142011
                                    Else
                                        strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNull(Ingredients.Item("Quantity_Imperial"))) & " " & _
                                            fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " &
                                            fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & " &#91or " & fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(AutoSpacing = True, " ", "") & fctCheckDbNull(Ingredients.Item("Preparation"))))) 'TDQ 10142011
                                    End If
                                    '-- JBB 10.25.2011
                                    'strIngredients = strIngredients.Replace("0 N/A", "")
                                    'strIngredients = strIngredients.Replace("0 n/a", "")
                                    strIngredients = strIngredients.Replace("N/A", "")
                                    strIngredients = strIngredients.Replace("n/a", "")

                                    '--
                                End If

                                'If fctCheckDbNull(Ingredients.Item("Preparation")) = "" Then 'TDQ 10172011
                                '    strIngredients = strIngredients.Substring(0, strIngredients.Length - 1)
                                'End If

                                strIngredients = strIngredients + "<br>"

                            ElseIf bitQtyFormat = 2 Then
                                strIngredients = fctCheckDbNull(Ingredients.Item("Description"))
                            ElseIf bitQtyFormat = 3 Then ' JBB 07.08.2011
                                Dim strM As String = IIf(Ingredients.Item("Quantity_Metric") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Metric"))) & " " & fctCheckDbNull(Ingredients.Item("UOM_Metric")) & " "
                                '-- JBB 10.25.2011
                                'strM = strM.Replace("0 N/A", "")
                                'strM = strM.Replace("0 n/a", "")
                                strM = strM.Replace("N/A", "")
                                strM = strM.Replace("n/a", "")
                                '--
                                Dim strI As String = ""
                                If IsNumeric(Ingredients.Item("Quantity_Imperial")) = True Then
                                    If blnUseFractions Then
                                        strI = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctConvertToFraction2(fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial")))) & " " & fctCheckDbNull(Ingredients.Item("UOM_Imperial"))
                                    Else
                                        strI = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial"))) & " " & fctCheckDbNull(Ingredients.Item("UOM_Imperial"))
                                    End If

                                Else
                                    strI = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNull(Ingredients.Item("Quantity_Imperial"))) & " " & fctCheckDbNull(Ingredients.Item("UOM_Imperial"))
                                End If
                                '-- JBB 10.25.2011
                                'strI = strI.Replace("0 N/A", "")
                                'strI = strI.Replace("0 n/a", "")
                                strI = strI.Replace("N/A", "")
                                strI = strI.Replace("n/a", "")

                                Dim strIngName As String

                                'TDQ 11022011
                                If fctCheckDbNull(Ingredients.Item("AlternativeIngredient")) = "" Then
                                    strIngName = fctCheckDbNull(Ingredients.Item("Complement") & " " & _
                                        fctCheckDbNull(Ingredients.Item("Name") & IIf(AutoSpacing = True, " ", "") & _
                                        fctCheckDbNull(Ingredients.Item("Preparation"))))
                                Else
                                    strIngName = fctCheckDbNull(Ingredients.Item("Complement") & " " & _
                                        fctCheckDbNull(Ingredients.Item("Name") & " &#91or " & _
                                        fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(AutoSpacing = True, " ", "") & _
                                        fctCheckDbNull(Ingredients.Item("Preparation")))))
                                End If

                                'If fctCheckDbNull(Ingredients.Item("Preparation")) = "" Then
                                '    strIngName = strIngName.Substring(0, strIngName.Length - 1)
                                'End If

                                '--
                                Dim strTempTemp As String = "<table  style='font-size: 11.5pt; font-family: Calibri;'><tr><td style='width: 100' valign='top'>%M</td><td style='width: 100' valign='top'>%I</td><td valign='top'>%N</td></tr></table>"
                                strIngredients = strTempTemp.Replace("%M", strM).Replace("%I", strI).Replace("%N", strIngName)
                            End If
                        Else ' JBB 07.14.2011 if Text
                            If bitQtyFormat = 0 Then
                                strIngredients = fctCheckDbNull(Ingredients.Item("Name")) & "<br/>"
                            ElseIf bitQtyFormat = 1 Then
                                strIngredients = fctCheckDbNull(Ingredients.Item("Name")) & "<br/>"
                            ElseIf bitQtyFormat = 2 Then
                                strIngredients = fctCheckDbNull(Ingredients.Item("Description"))
                            Else
                                Dim strTempTemp As String = "<table  style='font-size: 11.5pt; font-family: Calibri;'><tr><td style='width: 100' valign='top'>&nbsp</td><td style='width: 100' valign='top'>&nbsp</td><td valign='top'>%N</td></tr></table>"
                                strIngredients = strTempTemp.Replace("%N", fctCheckDbNull(Ingredients.Item("Name")))
                            End If
                        End If
                        'strHTMLContent.Append("<tr>")
                        'strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strIngredients.ToString)
                        '--strHTMLContent.Append("<br>") JBB 10.25.2011
                        'strHTMLContent.Append("</td>")
                        'strHTMLContent.Append("</tr>")
                    Next
                    '--
                    'For Each Ingredients As DataRow In dsRecipeDetails.Tables("Table1").Rows
                    '    If Ingredients.Item("Description") <> "" Then
                    '        strIngredients = fctCheckDbNull(Ingredients.Item("Description"))
                    '    Else
                    '        strIngredients = fctCheckDbNullNumeric(Ingredients.Item("Quantity")) & " " & fctCheckDbNull(Ingredients.Item("UOM")) & " " & fctCheckDbNull(Ingredients.Item("Name"))
                    '    End If
                    '    strHTMLContent.Append(strIngredients.ToString)
                    '    strHTMLContent.Append("<br>")
                    'Next
                    '

                    strHTMLContent.Append("</p>")
                End If

                'Method Header
                If strMethodHeader.ToString <> "" Then
                    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri; font-weight: bold' align='center'>")
                    strHTMLContent.Append(strMethodHeader.ToString)
                    strHTMLContent.Append("</p>")
                End If

                'Directions
                If strDirections.ToString <> "" Then
                    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")
                    'strHTMLContent.Append(strDirections.ToString.Replace(Chr(13) & Chr(10), "<br>")) 'TDQ 3.8.2012
                    strHTMLContent.Append(strDirections.ToString)
                    strHTMLContent.Append("</p>")
                End If

                'Footnote 1
                ' RDC 11.14.2013 : Added condition to display or not to display Notes
                If G_ExportOptions.blnExpIncludeNotes Then
                    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strFootNote1.ToString.Replace(Chr(13) & Chr(10), "<br>"))
                    'strHTMLContent.Append(strFootNote1)
                    strHTMLContent.Append("</p>")
                End If



                'Footnote 2
                ' RDC 11.14.2013 : Added condition to display or not to display Additional Notes
                If G_ExportOptions.blnExpIncludeAddNotes Then
                    strHTMLContent.Append("<table style='width: 620'>")
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td colspan='2' style='text-align: center; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(lblRecipeAddNote.ToString)
                    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strFootNote2.ToString.Replace(Chr(13) & Chr(10), "<br>"))
                    'strHTMLContent.Append(strFootNote2)
                    strHTMLContent.Append("</p>")
                End If


                ''Placements
                If G_ExportOptions.blnExpAdvIncludePublication Then
                    'If dsRecipeDetails.Tables("table9").Rows.Count > 0 Then
                    '    strHTMLContent.Append("<p style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: center'>")
                    '    strHTMLContent.Append(lblPlacements.ToString)
                    '    strHTMLContent.Append("</p>")
                    '    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri; padding-right: 10px'>")
                    '    For Each Placements As DataRow In dsRecipeDetails.Tables("table9").Rows
                    '        strPlacementName = fctCheckDbNull(Placements.Item("Placement"))
                    '        If Not IsDBNull(Placements.Item("PlacementDate")) Then strPlacementDate = CDate(Placements.Item("PlacementDate")).ToString("MM/dd/yyyy")
                    '        strPlacementDescription = fctCheckDbNull(Placements.Item("Description"))
                    '        strHTMLContent.Append(strPlacementName.ToString)
                    '        strHTMLContent.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
                    '        strHTMLContent.Append(strPlacementDate.ToString)
                    '        strHTMLContent.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
                    '        strHTMLContent.Append(strPlacementDescription.ToString)
                    '        strHTMLContent.Append("<br>")
                    '    Next
                    '    strHTMLContent.Append("</p>")
                    'End If
                End If


                'Nutrients
                If isDisplay = True Then
                    If dsRecipeDetails.Tables("Table4").Rows.Count > 0 Then
                        strHTMLContent.Append("<p style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
                        '-- JBB 02.23.2012
                        Dim strNutBasis As String = fctCheckDbNull(dsRecipeDetails.Tables("Table4").Rows(0).Item("NutritionBasis"))
                        If lblNutritionalInformation.ToString().Trim <> "" Then '-- JBB 02.23.2012
                            If strNutBasis = "" Then
                                strHTMLContent.Append(lblNutritionalInformation.ToString & " :")
                            Else
                                strHTMLContent.Append(lblNutritionalInformation.ToString & "(" & strNutBasis & ") :")
                            End If
                        End If
                        'strHTMLContent.Append(lblNutritionalInformation.ToString)
                        '-- 
                        strHTMLContent.Append("</p>")
                        strHTMLContent.Append(strNutrients.ToString)
                    End If

                    'Net Carbs
                    If strNetCarbohydrates.ToString <> "" Then
                        strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
                        strHTMLContent.Append(strNetCarbohydrates.ToString)
                        strHTMLContent.Append("</p>")
                        strHTMLContent.Append(lblNetCarbs.ToString)
                    End If
                End If
            End If


            strHTMLContent.Append("</div></body></html>")
            strErr = ""
        Else
            strErr = cLang.GetString(clsEGSLanguage.CodeType.FileNotFound)
        End If
        Return strHTMLContent
    End Function
    Public Function MultipleExportToWordNew2(ByVal dtRecipes As DataTable, ByVal bitFormat As Byte, ByVal intCodeTrans As Integer, ByRef strErr As String, ByRef strFilename As String, _
     ByVal bl2PicGoldOnly As Boolean, ByVal blChckBoxPic2 As Boolean, ByVal intCodeLang As Integer, blnUseFractions As Boolean, _
     Optional ByVal bitQtyFormat As Byte = 0, Optional ByVal blCookMode As Boolean = False, Optional ByVal intCodeSet As Integer = 0, _
     Optional udtListeType As enumDataListItemType = enumDataListItemType.Recipe, Optional ByVal intLangFromCodeDictionary As Integer = 1, Optional intCodeSite As Integer = 0, _
     Optional blnRemoveTrailingZeroes As Boolean = False, Optional blnAfterIngredient As Boolean = False) As StringBuilder
        Dim strHTMLContent As StringBuilder = New StringBuilder()
        Dim dsRecipeDetails As DataSet

        'Dim oMhtCol As New clsMhtml.mhtImageCollection 'CMV 051911
        Dim lblRecipeID As String = ""
        Dim strRecipeID As String = ""
        Dim lblRecipeNumber As String = ""
        Dim strRecipeNumber As String = ""
        Dim lblSubTitle As String = ""
        Dim strSubTitle As String = ""


        'JTOC 10.29.2013
        '-------------------------------------------
        Dim lblRecipeDescription As String = ""
        Dim strRecipeDescription As String = ""
        Dim lblRecipeRemark As String = ""
        Dim strRecipeRemark As String = ""
        Dim lblYield1 As String = ""
        Dim lblYield2 As String = ""
        Dim lblWeight As String = ""
        Dim strWeight As String = ""
        Dim strWeightQty As String = ""
        '-------------------------------------------

        Dim imgRecipe As String = ""
        Dim strImagePath As String = ""
        Dim strRecipeName As String = ""
        Dim strSubHeading As String = ""
        Dim strServings As String = ""
        Dim strYield As String = ""
        Dim strYield2 As String = ""
        Dim strServingsUnit As String = "" 'CMV 050211
        Dim strRecipeTime As String = ""
        Dim strMethodHeader As String = ""
        Dim strIngredients As String = ""
        Dim dblQty As Double
        Dim strUOM As String = ""
        Dim strDirections As String = ""
        Dim strAbbrDirections As String = ""
        Dim strFootNote1 As String = ""
        Dim strFootNote2 As String = ""
        Dim lblCostPerRecipe As String = ""
        Dim strCostPerRecipe As String = ""
        Dim lblCostPerServings As String = ""
        Dim strCostPerServings As String = ""
        Dim strCurrency As String = ""
        Dim lblInformation As String = ""
        Dim lblRecipeStatus As String = ""
        Dim strRecipeStatus As String = ""
        Dim lblWebStatus As String = ""
        Dim strWebStatus As String = ""
        Dim lblDateCreated As String = ""
        Dim strDateCreated As String = ""
        Dim lblDateLastModified As String = ""
        Dim strDateLastModified As String = ""
        Dim lblLastTested As String = ""
        Dim strLastTested As String = ""
        Dim lblDateDeveloped As String = ""
        Dim strDateDeveloped As String = ""
        Dim lblDateOfFinalEdit As String = ""
        Dim strDateOfFinalEdit As String = ""
        Dim lblDevelopmentPurpose As String = ""
        Dim strDevelopmentPurpose As String = ""
        Dim lblUpdatedBy As String = ""
        Dim strUpdatedBy As String = ""
        Dim lblCreatedBy As String = ""
        Dim strCreatedBy As String = ""
        Dim lblModifiedBy As String = ""
        Dim strModifiedBy As String = ""
        Dim lblTestedBy As String = ""
        Dim strTestedBy As String = ""
        Dim lblDevelopedBy As String = ""
        Dim strDevelopedBy As String = ""
        Dim lblFinalEditBy As String = ""
        Dim strFinalEditBy As String = ""
        Dim lblComments As String = ""
        Dim strSubmitDate As String = ""
        Dim strOwnerName As String = ""
        Dim strComments As String = ""
        Dim lblAttributes As String = ""
        Dim strAttributes As String = ""
        Dim strParents As String = ""
        Dim intAttributesCode As Integer
        Dim intAttributesParent As Integer
        Dim intAttributesMain As Integer
        Dim lblRecipeBrand As String = ""
        Dim strRecipeBrand As String = ""
        Dim strRecipeBrandClassification As String = ""
        Dim lblPlacements As String = ""
        Dim strPlacementName As String = ""
        Dim strPlacementDate As String = ""
        Dim strPlacementDescription As String = ""
        Dim lblNutritionalInformation As String = ""
        Dim lblCalories As String = ""
        Dim lblCaloriesFromFat As String = ""
        Dim lblSatFat As String = ""
        Dim lblTransFat As String = ""
        Dim lblMonoSatFat As String = ""
        Dim lblPolyFat As String = ""
        Dim lblTotalFat As String = ""
        Dim lblCholesterol As String = ""
        Dim lblSodium As String = ""
        Dim lblTotalCarbohydrates As String = ""
        Dim lblSugars As String = ""
        Dim lblDietaryFiber As String = ""
        Dim lblNetCarbohydrates As String = ""
        Dim lblProtein As String = ""
        Dim lblVitaminA As String = ""
        Dim lblVitaminC As String = ""
        Dim lblCalcium As String = ""
        Dim lblIron As String = ""
        Dim lblMonoUnsaturated As String = ""
        Dim lblPolyUnsaturated As String = ""
        Dim lblPotassium As String = ""
        Dim lblVitaminD As String = ""
        Dim lblVitaminE As String = ""
        Dim lblOmega3 As String = ""
        Dim strCalories As String = ""
        Dim strCaloriesFromFat As String = ""
        Dim strSatFat As String = ""
        Dim strTransFat As String = ""
        Dim strMonoSatFat As String = ""
        Dim strPolyFat As String = ""
        Dim strTotalFat As String = ""
        Dim strCholesterol As String = ""
        Dim strSodium As String = ""
        Dim strTotalCarbohydrates As String = ""
        Dim strSugars As String = ""
        Dim strDietaryFiber As String = ""
        Dim strNetCarbohydrates As String = ""
        Dim lblNetCarbs As String = ""
        Dim strProtein As String = ""
        Dim strVitaminA As String = ""
        Dim strVitaminC As String = ""
        Dim strCalcium As String = ""
        Dim strIron As String = ""
        Dim strMonoUnsaturated As String = ""
        Dim strPolyUnsaturated As String = ""
        Dim strPotassium As String = ""
        Dim strVitaminD As String = ""
        Dim strVitaminE As String = ""
        Dim strOmega3 As String = ""
        Dim strUnitCalories As String = ""
        Dim strUnitCaloriesFromFat As String = ""
        Dim strUnitSatFat As String = ""
        Dim strUnitTransFat As String = ""
        Dim strUnitMonoSatFat As String = ""
        Dim strUnitPolyFat As String = ""
        Dim strUnitTotalFat As String = ""
        Dim strUnitCholesterol As String = ""
        Dim strUnitSodium As String = ""
        Dim strUnitTotalCarbohydrates As String = ""
        Dim strUnitSugars As String = ""
        Dim strUnitDietaryFiber As String = ""
        Dim strUnitNetCarbohydrates As String = ""
        Dim strUnitProtein As String = ""
        Dim strUnitVitaminA As String = ""
        Dim strUnitVitaminC As String = ""
        Dim strUnitCalcium As String = ""
        Dim strUnitIron As String = ""
        Dim strUnitMonoUnsaturated As String = ""
        Dim strUnitPolyUnsaturated As String = ""
        Dim strUnitPotassium As String = ""
        Dim strUnitVitaminD As String = ""
        Dim strUnitVitaminE As String = ""
        Dim strUnitOmega3 As String = ""
        Dim strFormatCalories As String = ""
        Dim strFormatCaloriesFromFat As String = ""
        Dim strFormatSatFat As String = ""
        Dim strFormatTransFat As String = ""
        Dim strFormatMonoSatFat As String = ""
        Dim strFormatPolyFat As String = ""
        Dim strFormatTotalFat As String = ""
        Dim strFormatCholesterol As String = ""
        Dim strFormatSodium As String = ""
        Dim strFormatTotalCarbohydrates As String = ""
        Dim strFormatSugars As String = ""
        Dim strFormatDietaryFiber As String = ""
        Dim strFormatNetCarbohydrates As String = ""
        Dim strFormatProtein As String = ""
        Dim strFormatVitaminA As String = ""
        Dim strFormatVitaminC As String = ""
        Dim strFormatCalcium As String = ""
        Dim strFormatIron As String = ""
        Dim strFormatMonoUnsaturated As String = ""
        Dim strFormatPolyUnsaturated As String = ""
        Dim strFormatPotassium As String = ""
        Dim strFormatVitaminD As String = ""
        Dim strFormatVitaminE As String = ""
        Dim strFormatOmega3 As String = ""
        Dim strNutrients As String = ""
        Dim isDisplay As Boolean = False ' JBB 07.22.2011
        Dim strFolderImagePath As String = ""
        Dim imgRecipe2 As String = "" 'TDQ 10252011

        'TDQ 11.8.2011
        Dim lblThiamin As String = ""
        Dim strThiamin As String = ""
        Dim strFormatThiamin As String = ""
        Dim strUnitThiamin As String = ""

        Dim lblRiboflavin As String = ""
        Dim strRiboflavin As String = ""
        Dim strFormatRiboflavin As String = ""
        Dim strUnitRiboflavin As String = ""

        Dim lblNiacin As String = ""
        Dim strNiacin As String = ""
        Dim strFormatNiacin As String = ""
        Dim strUnitNiacin As String = ""

        Dim lblVitaminB6 As String = ""
        Dim strVitaminB6 As String = ""
        Dim strFormatVitaminB6 As String = ""
        Dim strUnitVitaminB6 As String = ""

        Dim lblFolate As String = ""
        Dim strFolate As String = ""
        Dim strFormatFolate As String = ""
        Dim strUnitFolate As String = ""

        Dim lblVitaminB12 As String = ""
        Dim strVitaminB12 As String = ""
        Dim strFormatVitaminB12 As String = ""
        Dim strUnitVitaminB12 As String = ""

        Dim lblBiotin As String = ""
        Dim strBiotin As String = ""
        Dim strFormatBiotin As String = ""
        Dim strUnitBiotin As String = ""

        Dim lblPantothenicAcid As String = ""
        Dim strPantothenicAcid As String = ""
        Dim strFormatPantothenicAcid As String = ""
        Dim strUnitPantothenicAcid As String = ""

        Dim lblPhosphorus As String = ""
        Dim strPhosphorus As String = ""
        Dim strFormatPhosphorus As String = ""
        Dim strUnitPhosphorus As String = ""

        Dim lblIodine As String = ""
        Dim strIodine As String = ""
        Dim strFormatIodine As String = ""
        Dim strUnitIodine As String = ""

        Dim lblMagnesium As String = ""
        Dim strMagnesium As String = ""
        Dim strFormatMagnesium As String = ""
        Dim strUnitMagnesium As String = ""

        Dim lblZinc As String = ""
        Dim strZinc As String = ""
        Dim strFormatZinc As String = ""
        Dim strUnitZinc As String = ""

        Dim lblManganese As String = ""
        Dim strManganese As String = ""
        Dim strFormatManganese As String = ""
        Dim strUnitManganese As String = ""

        Dim blnIncludeCostPerRecipe As Boolean = False 'IIf(bitFormat = 1, True, False)
        Dim blnIncludeCostPerServings As Boolean = False 'IIf(bitFormat = 1, True, False)
        Dim blnIncludeInformation As Boolean = False 'IIf(bitFormat = 1, True, False)
        Dim blnIncludeComment As Boolean = False 'IIf(bitFormat = 1, True, False)
        Dim blnIncludeKeyword As Boolean = False 'IIf(bitFormat = 1, True, False)
        Dim blnIncludeBrand As Boolean = False 'IIf(bitFormat = 1, True, False)
        Dim blnIncludePublication As Boolean = False 'IIf(bitFormat = 1, True, False)

        'TRANSLATION OF LABELS
        'Dim cLang As New clsEGSLanguage(intCodeLang)
        Dim cLang As New clsEGSLanguage(intLangFromCodeDictionary) 'JTOC 12.11.2013 intCodeLang to intLangFromCodeDictionary

        lblRecipeID = cLang.GetString(clsEGSLanguage.CodeType.RecipeID)
        lblRecipeNumber = cLang.GetString(clsEGSLanguage.CodeType.RecipeNumber) 'lblRecipeNumber = "Recipe Number"
        'lblSubTitle = "SubTitle" '-- JBB 02.21.2012  "SubTitle"
        'AGL 2012.10.31 - CWM-1971
        Dim clsLicense As New clsLicense
        If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
            lblSubTitle = cLang.GetString(clsEGSLanguage.CodeType.SubTitle) '"SubTitle" '-- JBB 02.21.2012 "Sub Title"
        Else
            lblSubTitle = cLang.GetString(clsEGSLanguage.CodeType.SubName)
        End If
        lblCostPerRecipe = cLang.GetString(clsEGSLanguage.CodeType.CostForTotalServings) & ":" 'lblCostPerRecipe = "Cost Per Recipe"
        lblCostPerServings = cLang.GetString(clsEGSLanguage.CodeType.CostForServing) & ":" 'lblCostPerServings = "Cost Per Serving"
        lblInformation = cLang.GetString(clsEGSLanguage.CodeType.Information) & ":" 'lblInformation = "Information"
        lblRecipeStatus = cLang.GetString(clsEGSLanguage.CodeType.RecipeStatus) & ":" 'lblRecipeStatus = "Recipe Status:"
        lblUpdatedBy = cLang.GetString(clsEGSLanguage.CodeType.UpdatedBy) & ":"
        lblWebStatus = cLang.GetString(clsEGSLanguage.CodeType.WebStatus) & ":" 'lblWebStatus = "Web Status:"
        lblDateCreated = cLang.GetString(clsEGSLanguage.CodeType.DateCreated) & ":" 'lblDateCreated = "Date Created:"
        lblCreatedBy = cLang.GetString(clsEGSLanguage.CodeType.CreatedBY) & ":" 'lblCreatedBy = "Created By:"
        lblDateLastModified = cLang.GetString(clsEGSLanguage.CodeType.DateLastModified) & ":" 'lblDateLastModified = "Date Last Modified:"
        lblModifiedBy = cLang.GetString(clsEGSLanguage.CodeType.ModifiedBy) & ":"
        lblLastTested = cLang.GetString(clsEGSLanguage.CodeType.DateLastTested) & ":"
        lblTestedBy = cLang.GetString(clsEGSLanguage.CodeType.TestedBy) & ":"
        lblDateDeveloped = cLang.GetString(clsEGSLanguage.CodeType.DateDeveloped) & ":"
        lblDevelopedBy = cLang.GetString(clsEGSLanguage.CodeType.DevelopedBy) & ":"
        lblDateOfFinalEdit = cLang.GetString(clsEGSLanguage.CodeType.FinalEditDate) & ":"
        lblFinalEditBy = cLang.GetString(clsEGSLanguage.CodeType.FinalEditBy) & ":"
        lblDevelopmentPurpose = cLang.GetString(clsEGSLanguage.CodeType.DevelopmentPurpose) & ":"
        lblComments = cLang.GetString(clsEGSLanguage.CodeType.Comments) '"Comments"
        lblAttributes = cLang.GetString(clsEGSLanguage.CodeType.Attributes)
        lblRecipeBrand = cLang.GetString(clsEGSLanguage.CodeType.Brand) '"Brands"
        'AGL 2013.03.16 
        If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
            lblPlacements = cLang.GetString(clsEGSLanguage.CodeType.RecipePlacements)
        Else
            lblPlacements = cLang.GetString(clsEGSLanguage.CodeType.Publication) '"Placements"
        End If

        lblNutritionalInformation = cLang.GetString(clsEGSLanguage.CodeType.NutritionalInfo) & " " & cLang.GetString(clsEGSLanguage.CodeType.per_serving)
        lblCalories = cLang.GetString(clsEGSLanguage.CodeType.Calories)
        lblCaloriesFromFat = cLang.GetString(clsEGSLanguage.CodeType.CaloriesfromFat)
        lblSatFat = "Sat Fat"
        lblTransFat = "Trans Fat"
        lblMonoSatFat = "Mono Sat Fat"
        lblPolyFat = "Poly Sat Fat"
        lblTotalFat = "Total Fat"
        lblCholesterol = "Cholesterol"
        lblSodium = "Sodium"
        lblTotalCarbohydrates = "Total Carbohydrates"
        lblSugars = "Sugars"
        lblDietaryFiber = "Dietary Fiber"
        lblNetCarbohydrates = "Net Carbohydrates"
        lblProtein = "Protein"
        lblVitaminA = "Vitamin A"
        lblVitaminC = "Vitamin C"
        lblCalcium = "Calcium"
        lblIron = "Iron"
        lblMonoUnsaturated = "Mono Unsaturated"
        lblPolyUnsaturated = "Poly Unsaturated"
        lblPotassium = "Potassium"
        lblVitaminD = "Vitamin D"
        lblVitaminE = "Vitamin E"
        lblNetCarbs = "* " & """Net Carbs""" & " are total carbohydrates minus dietary fiber and sugar alcohol as these have a minimal impact on blood sugar."
        'lblOmega3 = "Omega3"
        lblThiamin = "Thiamin"
        lblRiboflavin = "Riboflavin"
        lblNiacin = "Niacin"
        lblVitaminB6 = "VitaminB6"
        lblFolate = "Folate"
        lblVitaminB12 = "VitaminB12"
        lblBiotin = "Biotin"
        lblPantothenicAcid = "Pantothenic_Acid"
        lblPhosphorus = "Phosphorus"
        lblIodine = "Iodine"
        lblMagnesium = "Magnesium"
        lblZinc = "Zinc"
        lblManganese = "Manganese"
        lblOmega3 = "Omega-3"

        ' RDC 12.09.2013 : Removed and replaced code below
        'lblRecipeDescription = "Description"
        'lblRecipeRemark = "Remark"
        'lblYield1 = "Yield 1: "
        'lblYield2 = "Yield 2: "
        'lblWeight = "Weight(Subrecipe): "

        ' RDC 12.09.2013 : Translation for labels

        'TRANSLATION OF LABELS
        'lblRecipeID = cLang.GetString(clsEGSLanguage.CodeType.Recipe) & " ID" '"Recipe ID"
        'lblRecipeNumber = cLang.GetString(clsEGSLanguage.CodeType.RecipeNumber) '"Recipe Number"
        lblRecipeDescription = cLang.GetString(clsEGSLanguage.CodeType.Description) '"Description"
        lblRecipeRemark = cLang.GetString(clsEGSLanguage.CodeType.Remark) '"Remark"
        lblYield1 = cLang.GetString(clsEGSLanguage.CodeType.Yield) & " 1:" '"Yield 1: "
        lblYield2 = cLang.GetString(clsEGSLanguage.CodeType.Yield) & " 2:" '"Yield 2: "
        lblWeight = cLang.GetString(clsEGSLanguage.CodeType.Weight) & "(" & cLang.GetString(clsEGSLanguage.CodeType.Sub_Recipe) & "):" '"Weight(Subrecipe): "

        'FORMAT TABLE
        strHTMLContent.Append("<html " & _
         "xmlns:o='urn:schemas-microsoft-com:office:office' " & _
         "xmlns:w='urn:schemas-microsoft-com:office:word'" & _
         "xmlns='http://www.w3.org/TR/REC-html40'>" & _
           "<head><meta http-equiv='Content-Type' content='text/html;charset=utf-32' /><title></title>") '"<head><meta http-equiv='Content-Type' content=text/html;charset=utf-8 /><title></title>") '"<head><title></title>") 05.27.2011

        strHTMLContent.Append("<!--[if gte mso 9]>" & _
           "<xml>" & _
           "<w:WordDocument>" & _
           "<w:View>Print</w:View>" & _
           "<w:Zoom>100</w:Zoom>" & _
           "</w:WordDocument>" & _
           "</xml>" & _
           "<![endif]-->")
        strHTMLContent.Append("<html " & _
          "xmlns:o='urn:schemas-microsoft-com:office:office' " & _
          "xmlns:w='urn:schemas-microsoft-com:office:word'" & _
          "xmlns='http://www.w3.org/TR/REC-html40'>" & _
          "<head><meta http-equiv='Content-Type' content='text/html;charset=utf-32' /><title></title>")


        strHTMLContent.Append("<style>" & _
          "<!-- /* Style Definitions */ " & _
          "p.MsoFooter, li.MsoFooter, div.MsoFooter " & _
          "{margin:0in; " & _
          "margin-bottom:.0001pt; " & _
          "mso-pagination:widow-orphan; " & _
          "tab-stops:center 3.0in right 6.0in; " & _
          "font-size:12.0pt;} " & _
          "p.MsoHeader, li.MsoHeader, div.MsoHeader " & _
          "{margin:0in; " & _
          "margin-bottom:.0001pt; " & _
          "mso-pagination:widow-orphan; " & _
          "tab-stops:center 3.0in right 6.0in; " & _
          "font-size:12.0pt;} ")

        strHTMLContent.Append("@page Section1" & _
          "   {size:8.5in 11.0in; " & _
          "   margin:1in 1in 1in 1in; " & _
          "   mso-footer-margin:.5in; mso-paper-source:0;} " & _
          " div.Section1 " & _
          "   {page:Section1; " & _
          "font-size:11.5pt;font-family:'Calibri';mso-fareast-font-family:'Calibri'; " & _
           " } " & _
          "-->" & _
          " @media all { " & _
          "     .page-break { display: none; } " & _
          " } " & _
          " @media print { " & _
          "     .page-break { display: block; page-break-before: always; } " & _
          " } </style></head>")

        strHTMLContent.Append("<body lang=EN-US>" & _
         "<div class=Section1>")

        Dim rowCount As Integer = dtRecipes.Rows.Count
        Dim x As Integer
        Dim intListeID As Integer
        Dim strImage As String
        Dim intStandard As Integer

        For x = 0 To rowCount - 1

            intListeID = dtRecipes.Rows(x).Item("CodeListe").ToString
            strImage = dtRecipes.Rows(x).Item("ImageLoc").ToString

            GetRecipeCode(intListeID, m_RecipeId, m_Version)

            ' RDC 01.13.2014 : Code Site handler
            intCodeSite = getRecipeSiteOwner(m_RecipeId, m_Version)

            'AGL 2012.10.12 - CWM-1634 - added branch for merchandise
            'dsRecipeDetails = GetRecipeDetails(m_RecipeId, m_Version, intCodeTrans, 0, intCodeSet) 'CMV 051911
            If udtListeType = enumDataListItemType.Merchandise Then
                dsRecipeDetails = GetMerchandiseDetails(m_RecipeId, m_Version, intCodeTrans, 0, intCodeSet)
            Else
                dsRecipeDetails = GetRecipeDetails(m_RecipeId, m_Version, intCodeTrans, 0, intCodeSet, , intCodeSite) 'CMV 051911
            End If

            lblNutritionalInformation = ""

            If dsRecipeDetails.Tables("table1").Rows.Count > 0 Then
                'SET VALUES
                strRecipeID = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("RecipeID"))
                strRecipeNumber = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("Number"))
                strSubTitle = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("SubTitle"))

                'JTOC 10.29.2013
                '----------------------------------------------------------------------------------------------------
                strRecipeDescription = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Description"))
                strRecipeRemark = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Remark"))
                strWeight = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Weight"))
                strWeightQty = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("SrQty"))
                '--------------------------------------------------

                strRecipeName = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("Name"))
                strSubHeading = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("SubHeading"))
                ''strImagePath = Server.MapPath("Images/test.jpg") 'CMV 051911

                ''Dim imageRecipe As New System.Web.UI.WebControls.Image 'CMV 051911
                ''With imageRecipe
                ''    .ID = "Image1"
                ''    .Height = 240
                ''    .Width = 240
                ''    .ImageUrl = "Images/test.jpg"
                ''End With

                ' RDC 12.12.2013 : Discarded on top variables in displaying yield/subrecipe wt.
                Dim decYield1 As Decimal = 0D, _
                 decYield2 As Decimal = 0D, _
                 decSrWt As Decimal = 0D
                Dim strYield1Unit As String = dsRecipeDetails.Tables(11).Rows(0).Item("Yield1Unit").ToString.Replace("[_]", "").Replace("n/a", "").Replace("_", " ").ToLower, _
                 strYield2Unit As String = dsRecipeDetails.Tables(11).Rows(0).Item("Yield2Unit").ToString.Replace("[_]", "").Replace("n/a", "").Replace("_", " ").ToLower, _
                 strSrWtUnit As String = dsRecipeDetails.Tables(11).Rows(0).Item("SrUnit").ToString.Replace("[_]", "").Replace("n/a", "").Replace("_", " ").ToLower

                If Not IsDBNull(dsRecipeDetails.Tables(11).Rows(0).Item("Yield1")) Then decYield1 = CDec(dsRecipeDetails.Tables(11).Rows(0).Item("Yield1"))
                If Not IsDBNull(dsRecipeDetails.Tables(11).Rows(0).Item("Yield2")) Then decYield2 = CDec(dsRecipeDetails.Tables(11).Rows(0).Item("Yield2"))
                If Not IsDBNull(dsRecipeDetails.Tables(11).Rows(0).Item("SubRecipeQty")) Then decSrWt = CDec(dsRecipeDetails.Tables(11).Rows(0).Item("SubRecipeQty"))

                If decYield1 > 0 And Not strYield1Unit = "[_]" And Not strYield1Unit.ToLower = "n/a" And Not strYield1Unit.Trim.Length = 0 And Not strYield1Unit.EndsWith("s") And Not strYield1Unit.ToLower.Trim = "g" Then
                    If decYield1 > 1 Then
                        If strYield1Unit.Trim.Length > 0 And Char.IsLetter(Mid(strYield1Unit, strYield1Unit.Length, 1)) Then strYield1Unit &= "s"
                    End If

                End If

                If decYield2 > 0 And Not strYield2Unit = "[_]" And Not strYield2Unit.ToLower = "n/a" And Not strYield2Unit.Trim.Length = 0 And Not strYield2Unit.EndsWith("s") And Not strYield2Unit.ToLower.Trim = "g" Then
                    If decYield2 > 1 Then
                        If strYield2Unit.Trim.Length > 0 And Char.IsLetter(Mid(strYield2Unit, strYield2Unit.Length, 1)) Then strYield2Unit &= "s"
                    End If
                End If

                If CDec(Format(decSrWt, "#.000#")) > 0 And Not strSrWtUnit = "[_]" And Not strSrWtUnit.ToLower = "n/a" And Not strSrWtUnit.Trim.Length = 0 And Not strSrWtUnit.EndsWith("s") And Not strSrWtUnit.ToLower.Trim = "g" Then
                    If decSrWt > 1 Then
                        If strSrWtUnit.Trim.Length > 0 And Char.IsLetter(Mid(strSrWtUnit, strSrWtUnit.Length, 1)) Then strSrWtUnit &= "s"
                    End If
                End If

                Dim strYield1,strSrWt As String 

                Dim BlnConvertDecimaltoFraction As Boolean = CBool(dsRecipeDetails.Tables(12).Rows(0).Item("String"))

                If BlnConvertDecimaltoFraction = True Then
                    strYield1 = ConvertDecimalToFraction2(fctCheckDbNullNumeric(decYield1))
                    strSrWt = ConvertDecimalToFraction2(fctCheckDbNullNumeric(decSrWt))

                    strYield2 = ConvertDecimalToFraction2(fctCheckDbNullNumeric(decYield2))
                Else
                    strYield1 = fctCheckDbNullNumeric(decYield1)
                    strSrWt = fctCheckDbNullNumeric(decSrWt)

                    strYield2 = fctCheckDbNullNumeric(decYield2)
                End If

                Dim intFieldsToDisplay As Integer = 0, intFieldWidth As Integer = 0, intTableWidth As Integer = 620
                If G_ExportOptions.blnExpIncludeYield1 And decYield1 > 0 Then intFieldsToDisplay += 1
                If G_ExportOptions.blnExpIncludeYield2 And decYield2 > 0 Then intFieldsToDisplay += 1
                If G_ExportOptions.blnExpSubRecipeWt And decSrWt > 0 Then intFieldsToDisplay += 1

                Select Case intFieldsToDisplay
                    Case 1
                        intFieldWidth = 620
                        intTableWidth = 250
                    Case 2
                        intFieldWidth = 310
                        intTableWidth = 400
                    Case 3
                        intFieldWidth = CInt(620 / 3)
                    Case Else
                        intFieldWidth = CInt(620 / 3)
                End Select

                strServings = "<center><table width='" & intTableWidth & "'><tr>"
                With G_ExportOptions
                    If .blnExpIncludeYield1 And decYield1 > 0 Then strServings &= "<td width='" & intFieldWidth & "' align='center' style='font-size: 11.5pt; font-family: Calibri;'> <b>" & lblYield1.ToString & "</b>&nbsp;" & strYield1 & " " & strYield1Unit & " </td>"
                    If .blnExpIncludeYield2 And decYield2 > 0 Then strServings &= "<td width='" & intFieldWidth & "' align='center' style='font-size: 11.5pt; font-family: Calibri;'> <b>" & lblYield2.ToString & "</b>&nbsp;" & strYield2 & " " & strYield2Unit & "</td>"
                    If .blnExpSubRecipeWt And decSrWt > 0 Then strServings &= "<td width='" & intFieldWidth & "' align='center' style='font-size: 11.5pt; font-family: Calibri;'> <b>" & lblWeight.ToString & "</b> &nbsp;" & strSrWt & " " & strSrWtUnit & "</td>"
                End With
                strServings &= "</tr></table>"

                'End If

                strMethodHeader = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("MethodHeader"))
                'strDirections = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("Note"))

                If dsRecipeDetails.Tables("Table3").Rows.Count > 0 Then
                    strDirections = fctGetInstrunctions(dsRecipeDetails.Tables("Table3"), fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("MethodFormat")), blCookMode)
                    'strAbbrDirections = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("CookMode"))
                    strAbbrDirections = fctGetInstrunctions(dsRecipeDetails.Tables("Table3"), fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("MethodFormat")), True)
                Else
                    strAbbrDirections = ""
                    strDirections = ""
                End If


                strFootNote1 = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("FootNote1"))
                strFootNote2 = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("FootNote2"))
                strCurrency = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("Currency"))
                'strCostPerRecipe = strCurrency & " " & fctCheckDbNullNumeric(dsRecipeDetails.Tables("table1").Rows(0).Item("CostPrice"))
                'strCostPerServings = strCurrency & " " & fctCheckDbNullNumeric(dsRecipeDetails.Tables("table1").Rows(0).Item("CostPricePerServing"))
                strRecipeStatus = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("RecipeStatusName"))
                strUpdatedBy = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("UpdatedBy"))
                strWebStatus = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("WebStatusName"))
                If Not IsDBNull(dsRecipeDetails.Tables("table1").Rows(0).Item("DateCreated")) Then strDateCreated = FormatDateTime(CStrDB(dsRecipeDetails.Tables("table1").Rows(0).Item("DateCreated")), DateFormat.ShortDate) 'CDate(dsRecipeDetails.Tables("table1").Rows(0).Item("DateCreated"))
                strCreatedBy = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("CreatedBy"))
                If Not IsDBNull(dsRecipeDetails.Tables("table1").Rows(0).Item("DateLastModified")) Then strDateLastModified = FormatDateTime(CStrDB(dsRecipeDetails.Tables("table1").Rows(0).Item("DateLastModified")), DateFormat.ShortDate) 'CDate(dsRecipeDetails.Tables("table1").Rows(0).Item("DateLastModified"))
                strModifiedBy = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("LastModifiedBy"))
                If Not IsDBNull(dsRecipeDetails.Tables("table1").Rows(0).Item("DateTested")) Then strLastTested = FormatDateTime(CStrDB(dsRecipeDetails.Tables("table1").Rows(0).Item("DateTested")), DateFormat.ShortDate) 'CDate(dsRecipeDetails.Tables("table1").Rows(0).Item("DateTested"))
                strTestedBy = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("TestedBy"))
                If Not IsDBNull(dsRecipeDetails.Tables("table1").Rows(0).Item("DateDeveloped")) Then strDateDeveloped = FormatDateTime(CStrDB(dsRecipeDetails.Tables("table1").Rows(0).Item("DateDeveloped")), DateFormat.ShortDate) 'CDate(dsRecipeDetails.Tables("table1").Rows(0).Item("DateDeveloped"))
                strDevelopedBy = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("DevelopedBy"))
                If Not IsDBNull(dsRecipeDetails.Tables("table1").Rows(0).Item("DateFinalEdit")) Then strDateOfFinalEdit = FormatDateTime(CStrDB(dsRecipeDetails.Tables("table1").Rows(0).Item("DateFinalEdit")), DateFormat.ShortDate) 'CDate(dsRecipeDetails.Tables("table1").Rows(0).Item("DateFinalEdit"))
                strFinalEditBy = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("FinalEditBy"))
                strDevelopmentPurpose = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("DevelopmentPurpose"))

                isDisplay = CBoolDB(dsRecipeDetails.Tables("table1").Rows(0).Item("DisplayNutrition")) ' JBB 07.22.2011
                Dim strHeaderNutrientServing As String = ""
                ''If dsRecipeDetails.Tables("table4").Columns.Contains("PortionSize") = True Then
                ''    If dsRecipeDetails.Tables("table4").Rows(0).Item("Calories").ToString.Trim <> "" Then
                ''        strHeaderNutrientServing = dsRecipeDetails.Tables("table4").Rows(0).Item("PortionSize").ToString.Trim
                ''    Else
                ''        If dsRecipeDetails.Tables("table1").Columns.Contains("Yield") = True Then
                ''            strHeaderNutrientServing = dsRecipeDetails.Tables("table1").Rows(0).Item("Yield").ToString.Trim
                ''        End If
                ''    End If
                ''Else
                ''    If dsRecipeDetails.Tables("table1").Columns.Contains("Yield") = True Then
                ''        strHeaderNutrientServing = dsRecipeDetails.Tables("table1").Rows(0).Item("Yield").ToString.Trim
                ''    End If
                ''End If


                ''-- JBBB
                Dim strHeader As String = fGetMethodFormat("nh")
                Dim strItems As String = fGetMethodFormat("s")
                Dim dicIsDisplay As New Dictionary(Of String, Boolean)
                Dim dicColumnName As New Dictionary(Of String, String)
                Dim dicUnit As New Dictionary(Of String, String)
                Dim dicFormat As New Dictionary(Of String, String)
                Dim intIndex As Integer = 0
                Dim dtNutrients As DataTable = dsRecipeDetails.Tables("table4")
                Dim strColCalories As String = "Calories"
                If dtNutrients.Rows.Count > 0 Then  '' JBB 05.23.2012
                    For Each dcNutrient As DataColumn In dtNutrients.Columns
                        Dim strColumn As String = dcNutrient.ColumnName
                        If strColumn.Trim.ToLower <> "recipeid" And strColumn.Trim.ToLower <> "version" And strColumn.Trim.ToLower <> "portionsize" Then
                            If strColumn.Contains("Display") Then
                                dicIsDisplay.Add(strColumn.ToLower(), CBool(dtNutrients.Rows(intIndex)(strColumn)))
                            ElseIf strColumn.Contains("Unit_") Then
                                dicUnit.Add(strColumn.ToLower(), CStr(dtNutrients.Rows(intIndex)(strColumn)))
                            ElseIf strColumn.Contains("Format") Then
                                dicFormat.Add(strColumn.ToLower(), CStr(dtNutrients.Rows(intIndex)(strColumn)))
                            End If
                            'strNutrients.Append(strColumn.Replace("Display", "") + " " + dtNutrients.Rows(intIndex)(strColumn).ToString() + ", ")
                        End If
                        dicColumnName.Add(strColumn.ToLower(), strColumn)
                        'JTOC 18.01.2013 Removed Calo in condition
                        'If strColumn.Contains("Calo") And (Not strColumn.Contains("Display") Or Not strColumn.Contains("Unit_") Or Not strColumn.Contains("Format")) Then
                        If (Not strColumn.Contains("Display") Or Not strColumn.Contains("Unit_") Or Not strColumn.Contains("Format")) Then
                            strColCalories = strColumn
                        End If

                    Next

                    strHeaderNutrientServing = ""
                    If dsRecipeDetails.Tables("table4").Columns.Contains("PortionSize") = True Then
                        If dicColumnName.ContainsKey(strColCalories) Then
                            strHeaderNutrientServing = dsRecipeDetails.Tables("table1").Rows(0).Item("PortionSize").ToString.Trim
                        Else
                            If dsRecipeDetails.Tables("table4").Rows(0).Item(strColCalories).ToString.Trim <> "" Then
                                strHeaderNutrientServing = dsRecipeDetails.Tables("table4").Rows(0).Item("PortionSize").ToString.Trim
                            Else
                                If dsRecipeDetails.Tables("table1").Columns.Contains("Yield") = True Then
                                    strHeaderNutrientServing = dsRecipeDetails.Tables("table1").Rows(0).Item("Yield").ToString.Trim
                                End If
                            End If
                        End If
                    Else
                        If dsRecipeDetails.Tables("table1").Columns.Contains("Yield") = True Then
                            strHeaderNutrientServing = dsRecipeDetails.Tables("table1").Rows(0).Item("Yield").ToString.Trim
                        End If
                    End If

                    Dim lstKey As List(Of String)
                    lstKey = New List(Of String)(dicIsDisplay.Keys)
                    For Each dcNutrient As DataColumn In dtNutrients.Columns
                        Dim strColumn As String = dcNutrient.ColumnName
                        If strColumn.Trim.ToLower <> "recipeid" And strColumn.Trim.ToLower <> "version" And strColumn.Trim.ToLower <> "portionsize" Then
                            If Not strColumn.Contains("Display") And Not strColumn.Contains("Unit_") And Not strColumn.Contains("Format") Then
                                If lstKey.Contains(("Display" + strColumn.ToString()).ToLower) = True Then
                                    If dicIsDisplay(("Display" + strColumn.ToString()).ToLower) = True Then
                                        If dtNutrients.Rows(intIndex)(strColumn).ToString().Trim <> "-1" Then
                                            Dim strNutDisplayValue As String = Format(dicFormat((strColumn.ToString() + "Format").ToLower), IIf(dtNutrients.Rows(intIndex)(strColumn).ToString().Trim() <> "-1", dtNutrients.Rows(intIndex)(strColumn), 0)) '
                                            'strNutrients.Append(strColumn + " " + strNutDisplayValue + dicUnit(("Unit_" + strColumn.ToString()).ToLower) + ", ")
                                            strNutrients = strNutrients & Replace(strColumn, "_", " ") & " " & strNutDisplayValue + dicUnit(("Unit_" + strColumn.ToString()).ToLower) & ", "
                                            lblNutritionalInformation = cLang.GetString(clsEGSLanguage.CodeType.NutritionalInfo) & " " & strHeaderNutrientServing & " "
                                        End If
                                    End If
                                End If
                            End If

                            'strNutrients.Append(strColumn.Replace("Display", "") + " " + dtNutrients.Rows(intIndex)(strColumn).ToString() + ", ")
                        End If
                    Next
                    ''-- 
                    If Right(strNutrients, 2) = ", " Then strNutrients = strNutrients.Remove(Len(strNutrients) - 2, 2)

                Else '' JBB 05.23.2012
                    lblNutritionalInformation = ""
                    strNutrients = ""
                    strHeaderNutrientServing = ""
                End If '' JBB 05.23.2012


                'If Right(strNutrients, 2) = ", " Then strNutrients = strNutrients.Remove(Len(strNutrients) - 2, 2)
                'JBB -- 07.14.2011
                strDirections = fctGetInstrunctions(dsRecipeDetails.Tables("table3"), fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("MethodFormat")), blCookMode)
                'TDQ 2.24.2012
                strAbbrDirections = fctGetInstrunctions(dsRecipeDetails.Tables("table3"), fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("MethodFormat")), True) ' RJL - 11774 :02-17-2014 'fctGetInstrunctions(dsRecipeDetails.Tables("Table3"), strAbbrDirections, True)
                strDirections = fctCheckDbNull(strDirections)
                strAbbrDirections = fctCheckDbNull(strAbbrDirections)

                If bitFormat = 1 Then
                    strHTMLContent.Append("<div class='page-break'></div>")

                    'strHTMLContent.Append("<table style='width: 620'>")
                    'strHTMLContent.Append("<tr>")
                    'strHTMLContent.Append("<td>")
                    strHTMLContent.Append("<table style='width: 620'>")

                    'Recipe Name
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td style='font-weight: bold; font-size: x-large; text-align: center; font-family: Calibri;'>")
                    strHTMLContent.Append(strRecipeName.ToString)
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")

                    ' RDC 11.18.2013 : Create additional table if any of the settings are true
                    With G_ExportOptions
                        If .blnExpIncludeRecipeNo Or .blnExpIncludeSubName Or .blnExpIncludeItemDesc Or .blnExpIncludeRemark Then
                            With strHTMLContent
                                .Append("<tr><td><table>")

                                ' Recipe Number
                                If G_ExportOptions.blnExpIncludeRecipeNo Then
                                    .Append("<tr><td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri;' valign='top' width='20%'>")
                                    .Append(lblRecipeNumber.ToString)
                                    .Append("</td>")
                                    .Append("<td style='font-size: 11.5pt; font-family: Calibri;' valign='top' width='80%'>")
                                    .Append(": &nbsp;" & strRecipeNumber.ToString)
                                    .Append("</td></tr>")
                                End If

                                ' Sub Title
                                If G_ExportOptions.blnExpIncludeSubName Then
                                    .Append("<tr><td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri;' valign='top'>")
                                    .Append(lblSubTitle.ToString)
                                    .Append("</td>")
                                    .Append("<td style='font-size: 11.5pt; font-family: Calibri;' valign='top' width='80%'>")
                                    .Append(": &nbsp;" & strSubTitle.ToString)
                                    .Append("</td></tr>")
                                End If

                                ' Description
                                If G_ExportOptions.blnExpIncludeItemDesc And strRecipeDescription.Trim.Length > 0 Then
                                    .Append("<tr><td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri;' valign='top' width='20%'>")
                                    .Append(lblRecipeDescription.ToString)
                                    .Append("</td>")
                                    .Append("<td style='font-size: 11.5pt; font-family: Calibri;' valign='top' width='80%'>")
                                    .Append(": &nbsp;" & strRecipeDescription.ToString)
                                    .Append("</td></tr>")
                                End If

                                ' Remarks
                                If G_ExportOptions.blnExpIncludeRemark And strRecipeRemark.Trim.Length > 0 Then
                                    .Append("<tr><td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri;' valign='top' width='20%'>")
                                    .Append(lblRecipeRemark.ToString)
                                    .Append("</td>")
                                    .Append("<td style='font-size: 11.5pt; font-family: Calibri;' valign='top' width='80%'>")
                                    .Append(": &nbsp;" & strRecipeRemark.ToString)
                                    .Append("</td></tr>")
                                End If

                                .Append("</table></td></tr>")
                                .Append("<br />")
                            End With
                        End If
                    End With



                    Dim strImage2 As String

                    strImage2 = dtRecipes.Rows(x).Item("ImageLoc2").ToString

                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td style='text-align: center'>")
                    strHTMLContent.Append("<table style='text-align: center'>")
                    strHTMLContent.Append("<tr>")
                    If strImage <> "" And strImage2 <> "" Then
                        strHTMLContent.Append(" <td style='text-align: center'>")
                        imgRecipe = strImage ' getHtml(imageRecipe) 'CMV 051911
                        strHTMLContent.Append(imgRecipe) 'CMV 051911
                        strHTMLContent.Append("</td>")

                        If Not strImage.Contains("nopic.jpg") And Not strImage2.Contains("nopic.jpg") Then
                            strHTMLContent.Append("<td style='text-align: center'>")
                            imgRecipe2 = strImage2 ' getHtml(imageRecipe) 'CMV 051911
                            strHTMLContent.Append(imgRecipe2) 'CMV 051911
                            strHTMLContent.Append("</td>")
                        End If

                    Else
                        strHTMLContent.Append(" <td style='text-align: center'>")
                        imgRecipe = strImage ' getHtml(imageRecipe) 'CMV 051911
                        strHTMLContent.Append(imgRecipe) 'CMV 051911
                        strHTMLContent.Append("</td>")
                    End If
                    strHTMLContent.Append("</tr>")
                    strHTMLContent.Append("</table>")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")

                    'Subheading
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td style='font-weight: bold; text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strSubHeading.ToString)
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")

                    'Servings
                    ' RDC 11.14.2013 : Display or not to display Servings
                    If G_ExportOptions.blnExpIncludeYield1 Or G_ExportOptions.blnExpIncludeYield2 Or G_ExportOptions.blnExpSubRecipeWt Then
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strServings.ToString)
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")

                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td>")
                        strHTMLContent.Append("&nbsp;")
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                    End If

                    'Recipe Time
                    ' RDC 11.14.2013 : Display or not to display Recipe Time
                    If G_ExportOptions.blnExpIncludeRecipeTime Then
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                        For Each RecipeTime As DataRow In dsRecipeDetails.Tables("Table5").Rows
                            strRecipeTime = RecipeTime.Item("Description")
                            Dim intHours As Integer = CIntDB(RecipeTime("RecipeTimeHH"))
                            Dim intMinutes As Integer = CIntDB(RecipeTime("RecipeTimeMM"))
                            Dim intSeconds As Integer = CIntDB(RecipeTime("RecipeTimeSS"))
                            Dim strAnd As String = cLang.GetString(clsEGSLanguage.CodeType._And).ToString.ToLower & " "

                            If intHours > 0 And intMinutes > 0 And intSeconds > 0 Then          ' 111
                                If intHours = 1 Then strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHour).ToLower & ", ") Else strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHours).ToLower & ", ")
                                If intMinutes = 1 Then strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinute).ToLower & " " & strAnd) Else strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinutes).ToLower & " " & strAnd)
                                If intSeconds = 1 Then strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSecond).ToLower) Else strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSeconds).ToLower)
                            ElseIf intHours = 0 And intMinutes > 0 And intSeconds > 0 Then      ' 011
                                If intMinutes = 1 Then strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinute).ToLower & strAnd) Else strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinutes).ToLower & " " & strAnd)
                                If intSeconds = 1 Then strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSecond).ToLower) Else strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSeconds).ToLower)
                                strRecipeTime = strRecipeTime.Replace("0 %h", "")
                            ElseIf intHours > 0 And intMinutes > 0 And intSeconds = 0 Then      ' 110
                                If intHours = 1 Then strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHour).ToLower & " " & strAnd) Else strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHours).ToLower & " " & strAnd)
                                If intMinutes = 1 Then strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinute).ToLower) Else strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinutes).ToLower)
                                strRecipeTime = strRecipeTime.Replace("0 %s", "")
                            ElseIf intHours = 0 And intMinutes = 0 And intSeconds > 0 Then      ' 001
                                If intSeconds = 1 Then strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSecond).ToLower) Else strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSeconds).ToLower)
                                strRecipeTime = strRecipeTime.Replace("0 %h", "").Replace("0 %m", "")
                            ElseIf intHours = 0 And intMinutes > 0 And intSeconds = 0 Then      ' 010
                                If intMinutes = 1 Then strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinute).ToLower) Else strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinutes).ToLower)
                                strRecipeTime = strRecipeTime.Replace("0 %h", "").Replace("0 %s", "")
                            ElseIf intHours > 0 And intMinutes = 0 And intSeconds = 0 Then      ' 100
                                If intHours = 1 Then strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHour).ToLower) Else strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHours).ToLower)
                                strRecipeTime = strRecipeTime.Replace("0 %m", "").Replace("0 %s", "")
                            ElseIf intHours > 0 And intMinutes = 0 And intSeconds > 0 Then      ' 101
                                If intHours = 1 Then strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHour).ToLower & " " & strAnd) Else strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHours).ToLower & " " & strAnd)
                                If intSeconds = 1 Then strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSecond).ToLower) Else strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSeconds).ToLower)
                                strRecipeTime = strRecipeTime.Replace("0 %m", "")
                            Else                                                                ' 000
                                strRecipeTime = ""
                            End If

                            strHTMLContent.Append(strRecipeTime.ToString)
                            strHTMLContent.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")

                        Next
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                    End If

                    ' Ingredients
                    ' RDC 11.14.2013 : Revised code for ingredient display
                    If dsRecipeDetails.Tables(2).Rows.Count > 0 Then
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td>")
                        strHTMLContent.Append("<table style='font-size: 11.5pt; font-family: Calibri; width: 620;'>")

                        For Each rwIngredient As DataRow In dsRecipeDetails.Tables(2).Rows
                            Dim intRemainingSize As Integer = 620
                            strHTMLContent.Append("<tr>")

                            Dim intItemType As Integer
                            Dim strIngredient As String = ""
                            Dim strItemName As String = ""
                            Dim strAltIngredient As String = ""
                            Dim strIngrComplement As String = ""
                            Dim strIngrPreparation As String = ""

                            If IsDBNull(rwIngredient("Type")) Then intItemType = 0 Else intItemType = rwIngredient("Type")

                            ' Ingredient = Complement IngredientName [or AlternativeIngredient], Preparation 
                            ' Ingredient Name
                            If Not IsDBNull(rwIngredient("Name")) And Not rwIngredient("Name").ToString.Trim.Length = 0 Then
                                strItemName = rwIngredient("Name").ToString.Trim
                            End If
                            ' Alternative Ingredient
                            If Not IsDBNull(rwIngredient("AlternativeIngredient")) And Not rwIngredient("AlternativeIngredient").ToString.Trim.Length = 0 Then
                                strAltIngredient = "[" & cLang.GetString(clsEGSLanguage.CodeType.OR_) & " " & rwIngredient("AlternativeIngredient").ToString.Trim & "]"
                            End If
                            ' Complement
                            If Not IsDBNull(rwIngredient("Complement")) And Not rwIngredient("Complement").ToString.Trim.Length = 0 Then
                                strIngrComplement = rwIngredient("Complement").ToString.Trim
                            End If
                            ' Preparation
                            If Not IsDBNull(rwIngredient("Preparation")) And Not rwIngredient("Preparation").ToString.Trim.Length = 0 Then
                                strIngrPreparation = rwIngredient("Preparation").ToString.Trim
                            End If

                            ' Combine all information to form 1 ingredient detail
                            If strIngrComplement.Trim.Length > 1 Then strIngredient &= strIngrComplement & " "
                            If strItemName.Trim.Length > 1 Then strIngredient &= strItemName & " "
                            If strAltIngredient.Trim.Length > 1 Then strIngredient &= strAltIngredient
                            If strIngrPreparation.Trim.Length > 1 Then strIngredient &= ", " & strIngrPreparation

                            ' Get All quantities
                            ' For Metric and Imperial Quantities
                            Dim strMetricNet As String = "0", strMetricGross As String = "0", strMetricUnit As String = ""
                            Dim strImperialNet As String = "0", strImperialGross As String = "0", strImperialUnit As String = ""
                            ' For One Quantity
                            Dim strQtyNet As String = "0", strQtyGross As String = "0", strQtyUnit As String = ""
                            ' Total Wastage
                            Dim dblTotalWastage As Double = 0

                            If Not IsDBNull(rwIngredient("TotalWastage")) Then dblTotalWastage = CDbl(rwIngredient("TotalWastage"))
                            If rwIngredient("IngredientId") = 0 And rwIngredient("Type") = 0 Then
                                Dim dtqty As New DataTable
                                If Not rwIngredient("Quantity_Metric") Is Nothing Then
                                    dtqty = getAlternateQuantity(rwIngredient("Quantity_Metric").ToString, rwIngredient("UOM_Metric"), intCodeTrans, intCodeSite)
                                Else
                                    dtqty = getAlternateQuantity(rwIngredient("Quantity_Imperial").ToString, rwIngredient("UOM_Imperial"), intCodeTrans, intCodeSite)
                                End If

                                If dtqty.Rows.Count > 0 Then
                                    For Each dr As DataRow In dtqty.Rows
                                        strMetricNet = dr("QtyMetric")
                                        strMetricGross = dr("QtyMetric")
                                        strMetricUnit = dr("UnitMetric")
                                        strImperialNet = dr("QtyImperial")
                                        strImperialGross = dr("QtyImperial")
                                        strImperialUnit = dr("UnitImperial")
                                    Next
                                End If
                            Else
                                If Not IsDBNull(rwIngredient("Quantity_Metric")) Then
                                    Dim metric_format As String = rwIngredient("UnitFormat").ToString
                                    ' RDC 01.08.2014 : Display only in decimal form
                                    'strMetricNet = ConvertDecimalToFraction2(rwIngredient("Quantity_Metric").ToString)
                                    'strMetricGross = ConvertDecimalToFraction2(CDbl(rwIngredient("QtyMetricGross")))
                                    strMetricNet = Format(CDblDB(rwIngredient("Quantity_Metric").ToString), "##0.0#")
                                    strMetricGross = Format(CDblDB(rwIngredient("QtyMetricGross").ToString), "##0.0#")

                                    strMetricNet = fctFormatNumericQuantity(CDblDB(rwIngredient("Quantity_Metric").ToString), metric_format, blnRemoveTrailingZeroes, 0)
                                    strMetricGross = fctFormatNumericQuantity(CDblDB(rwIngredient("QtyMetricGross").ToString), metric_format, blnRemoveTrailingZeroes, 0)
                                    ' RDC 11.27.2013 : Removed due to Sp reconstruction
                                    'strMetricGross = fctConvertToFraction2(CDbl(rwIngredient("Quantity_Metric")) * CDbl(1 + (dblTotalWastage / 100)))
                                End If
                                If Not IsDBNull(rwIngredient("UOM_Metric")) Then
                                    strMetricUnit = rwIngredient("UOM_Metric").ToString.Replace("n/a", "").Replace("[_]", "").Replace("N/A", "")
                                End If

                                If Not IsDBNull(rwIngredient("Quantity_Imperial")) Then

                                    If BlnConvertDecimaltoFraction = True Then
                                         strImperialNet = ConvertDecimalToFraction2(rwIngredient("Quantity_Imperial").ToString)
                                        strImperialGross = ConvertDecimalToFraction2(CDblDB(rwIngredient("QtyImperialGross")).ToString)
                                    Else
                                        strImperialNet = rwIngredient("Quantity_Imperial").ToString
                                        strImperialGross = CDblDB(rwIngredient("QtyImperialGross")).ToString
                                    End If

                                  
                                    ' RDC 11.27.2013 : Removed due to Sp reconstruction
                                    'strImperialGross = fctConvertToFraction2(CDbl(rwIngredient("Quantity_Imperial")) * CDbl(1 + (dblTotalWastage / 100)).ToString)
                                End If
                                If Not IsDBNull(rwIngredient("UOM_Imperial")) Then
                                    strImperialUnit = rwIngredient("UOM_Imperial").ToString.Replace("n/a", "").Replace("[_]", "").Replace("N/A", "")
                                End If

                                If Not IsDBNull(rwIngredient("OneQtyNet")) Then

                                    If BlnConvertDecimaltoFraction = True Then
                                        strQtyNet = ConvertDecimalToFraction2(rwIngredient("OneQtyNet"))
                                        strQtyGross = ConvertDecimalToFraction2(CDbl(rwIngredient("OneQtyGross")).ToString)
                                    Else
                                        strQtyNet = rwIngredient("OneQtyNet")
                                        strQtyGross = CDbl(rwIngredient("OneQtyGross")).ToString
                                    End If

                                    ' RDC 11.27.2013 : Removed due to Sp reconstruction
                                    'strQtyGross = fctConvertToFraction2(CDbl(rwIngredient("OneQtyNet")) * CDbl(1 + (dblTotalWastage / 100)).ToString)
                                    strQtyUnit = rwIngredient("OneQtyUnit").ToString.Replace("n/a", "").Replace("[_]", "").Replace("N/A", "")
                                End If
                            End If

                            Dim intIncludedColumns As Integer = 0
                            If intItemType = 75 Then

                                Select Case bitUseOneQuantity
                                    Case 0
                                        With G_ExportOptions
                                            If .blnExpIncludeImperialNetQty Then intIncludedColumns += 1
                                            If .blnExpIncludeImperialGrossQty Then intIncludedColumns += 1
                                            If .blnExpIncludeMetricNetQty Then intIncludedColumns += 1
                                            If .blnExpIncludeMetricGrossQty Then intIncludedColumns += 1
                                            intIncludedColumns += 1
                                        End With
                                    Case 1
                                        With G_ExportOptions
                                            If .blnExpIncludeNetQty Then intIncludedColumns += 1
                                            If .blnExpIncludeGrossQty Then intIncludedColumns += 1
                                            intIncludedColumns += 1
                                        End With
                                End Select
                                strHTMLContent.Append("<td width ='100%' colspan='" & intIncludedColumns & "' valign='top'><b>" & strIngredient & "</b></td>")
                            Else

                                Dim intColSize As Integer = 100
                                Select Case bitUseOneQuantity
                                    Case 0
                                        With G_ExportOptions
                                            If .blnExpIncludeImperialNetQty Then intIncludedColumns += 1
                                            If .blnExpIncludeImperialGrossQty Then intIncludedColumns += 1
                                            If .blnExpIncludeMetricNetQty Then intIncludedColumns += 1
                                            If .blnExpIncludeMetricGrossQty Then intIncludedColumns += 1
                                            intIncludedColumns += 1
                                        End With
                                    Case 1
                                        With G_ExportOptions
                                            If .blnExpIncludeNetQty Then intIncludedColumns += 1
                                            If .blnExpIncludeGrossQty Then intIncludedColumns += 1
                                            intIncludedColumns += 1
                                        End With
                                End Select

                                ' RDC 12.18.2013 : Added best unit conversion for unvalidated ingredients
                                Dim dt As New DataTable
                                Dim intUnitCode As Integer = -1, intIsImperialMetric As Integer = 9, strUnitFormat As String = "", dblUnitFactor As Decimal = 0D, intTypeMain As Integer = 0

                                Dim strUnvalidatedMetricQty As String = strMetricNet, strUnvalidatedMetricUnit As String = strMetricUnit
                                Dim strUnvalidatedImperialQty As String = strImperialNet, strUnvalidatedImperialUnit As String = strImperialUnit

                                Select Case bitUseOneQuantity
                                    Case 0 ' Display Metric/Imperial Gross/Net quantities   

                                        If G_ExportOptions.blnExpIncludeImperialNetQty Then
                                            If rwIngredient("Type") = 0 And rwIngredient("IngredientId") = 0 And Not strMetricNet = "0" Then
                                                strHTMLContent.Append("<td width='110' valign='top'>")
                                                ' RDC 11.26.2013 : Do not display if quantity is zero

                                                If Not strUnvalidatedImperialQty = "0" Then
                                                    If BlnConvertDecimaltoFraction = True Then
                                                        strHTMLContent.Append(ConvertDecimalToFraction2(CDblDB(strUnvalidatedImperialQty)) & " " & strUnvalidatedImperialUnit.Replace("_", " "))
                                                    Else
                                                        strHTMLContent.Append(CDblDB(strUnvalidatedImperialQty) & " " & strUnvalidatedImperialUnit.Replace("_", " "))
                                                    End If
                                                Else
                                                    strHTMLContent.Append(strUnvalidatedImperialUnit.Replace("_", " "))
                                                End If
                                                strHTMLContent.Append("</td>")
                                                intRemainingSize -= 110
                                            Else
                                                strHTMLContent.Append("<td width='110' valign='top'>")
                                                ' RDC 11.26.2013 : Do not display if quantity is zero
                                                If Not rwIngredient("type") = 4 Then ' RJL -  :02-17-2014
                                                    If Not strImperialNet = "0" Then strHTMLContent.Append(strImperialNet & " " & strImperialUnit.Replace("_", " ")) Else strHTMLContent.Append(strImperialUnit.Replace("_", " "))
                                                End If

                                                strHTMLContent.Append("</td>")
                                                intRemainingSize -= 110
                                            End If

                                            End If

                                        If G_ExportOptions.blnExpIncludeImperialGrossQty Then
                                            If rwIngredient("Type") = 0 And rwIngredient("IngredientId") = 0 And Not strMetricGross = "0" Then
                                                strHTMLContent.Append("<td width='110' valign='top'>")
                                                ' RDC 11.26.2013 : Do not display if quantity is zero
                                                If Not strUnvalidatedImperialQty = "0" Then
                                                    If BlnConvertDecimaltoFraction = True Then
                                                        strHTMLContent.Append(ConvertDecimalToFraction2(CDblDB(strUnvalidatedImperialQty)) & " " & strUnvalidatedImperialUnit.Replace("_", " "))
                                                    Else
                                                        strHTMLContent.Append(CDblDB(strUnvalidatedImperialQty) & " " & strUnvalidatedImperialUnit.Replace("_", " "))
                                                    End If
                                                Else
                                                    strHTMLContent.Append(strUnvalidatedImperialUnit.Replace("_", " "))
                                                End If
                                                strHTMLContent.Append("</td>")
                                                intRemainingSize -= 110
                                            Else
                                                strHTMLContent.Append("<td width='110' valign='top'>")
                                                ' RDC 11.26.2013 : Do not display if quantity is zero
                                                If Not rwIngredient("type") = 4 Then ' RJL -  :02-17-2014
                                                    If Not strImperialGross = "0" Then strHTMLContent.Append(strImperialGross & " " & strImperialUnit.Replace("_", " ")) Else strHTMLContent.Append(strImperialUnit.Replace("_", " "))
                                                End If

                                                strHTMLContent.Append("</td>")
                                                intRemainingSize -= 110
                                            End If

                                            End If

                                        If G_ExportOptions.blnExpIncludeMetricNetQty Then
                                            If rwIngredient("Type") = 0 And rwIngredient("IngredientId") = 0 Then
                                                strHTMLContent.Append("<td width='110' valign='top'>")
                                                ' RDC 11.26.2013 : Do not display if quantity is zero
                                                If Not strUnvalidatedMetricQty = "0" Then strHTMLContent.Append(strUnvalidatedMetricQty & " " & strUnvalidatedMetricUnit.Replace("_", " ")) Else strHTMLContent.Append(strUnvalidatedMetricUnit.Replace("_", " "))
                                                strHTMLContent.Append("</td>")
                                                intRemainingSize -= 110
                                            Else
                                                strHTMLContent.Append("<td width='110' valign='top'>")
                                                ' RDC 11.26.2013 : Do not display if quantity is zero
                                                If Not rwIngredient("type") = 4 Then ' RJL -  :02-17-2014
                                                    If Not strMetricNet = "0" Then strHTMLContent.Append(strMetricNet & " " & strMetricUnit.Replace("_", " ")) Else strHTMLContent.Append(strMetricUnit.Replace("_", " "))
                                                End If

                                                strHTMLContent.Append("</td>")
                                                intRemainingSize -= 110
                                            End If

                                        End If

                                        If G_ExportOptions.blnExpIncludeMetricGrossQty Then
                                            If rwIngredient("Type") = 0 And rwIngredient("IngredientId") = 0 Then
                                                strHTMLContent.Append("<td width='110' valign='top'>")
                                                ' RDC 11.26.2013 : Do not display if quantity is zero
                                                If Not strUnvalidatedMetricQty = "0" Then strHTMLContent.Append(strUnvalidatedMetricQty & " " & strUnvalidatedMetricUnit.Replace("_", " ")) Else strHTMLContent.Append(strUnvalidatedMetricUnit.Replace("_", " "))
                                                strHTMLContent.Append("</td>")
                                                intRemainingSize -= 110
                                            Else
                                                strHTMLContent.Append("<td width='110' valign='top'>")
                                                ' RDC 11.26.2013 : Do not display if quantity is zero
                                                If Not rwIngredient("type") = 4 Then ' RJL -  :02-17-2014
                                                    If Not strMetricGross = "0" Then strHTMLContent.Append(strMetricGross & " " & strMetricUnit.Replace("_", " ")) Else strHTMLContent.Append(strMetricUnit.Replace("_", " "))
                                                End If

                                                strHTMLContent.Append("</td>")
                                                intRemainingSize -= 110
                                            End If
                                        End If

                                    Case 1 ' Display Gross and Net Quantities only

                                        If G_ExportOptions.blnExpIncludeNetQty Then
                                            strHTMLContent.Append("<td width='125' valign='top'>")
                                            ' RDC 11.26.2013 : Do not display if quantity is zero
                                            If Not rwIngredient("type") = 4 Then ' RJL -  :02-17-2014
                                                If Not strQtyNet = "0" Then strHTMLContent.Append(strQtyNet & " " & strQtyUnit.Replace("_", " ")) Else strHTMLContent.Append(strQtyUnit.Replace("_", " "))
                                            End If

                                            strHTMLContent.Append("</td>")
                                            intRemainingSize -= 125
                                        End If

                                        If G_ExportOptions.blnExpIncludeGrossQty Then
                                            strHTMLContent.Append("<td width='125' valign='top'>")
                                            ' RDC 11.26.2013 : Do not display if quantity is zero
                                            If Not rwIngredient("type") = 4 Then ' RJL -  :02-17-2014
                                                If Not strQtyGross = "0" Then strHTMLContent.Append(strQtyGross & " " & strQtyUnit.Replace("_", " ")) Else strHTMLContent.Append(strQtyUnit.Replace("_", " "))
                                            End If

                                            strHTMLContent.Append("</td>")
                                            intRemainingSize -= 125
                                        End If
                                    Case Else
                                End Select

                                ' Ingredient name
                                ' RDC 11.29.2013 : Make steps in bold caption.
                                strHTMLContent.Append("<td width='" & intRemainingSize & "' valign='top'>")
                                strHTMLContent.Append(strIngredient)
                                strHTMLContent.Append("</td></tr>")

                            End If
                        Next
                        strHTMLContent.Append("</table>")
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                    End If
                    If G_ExportOptions.intExpSelectedProcedure = 0 Then
                        strMethodHeader = cLang.GetString(clsEGSLanguage.CodeType.PreparationMethod)
                    Else
                        strMethodHeader = cLang.GetString(clsEGSLanguage.CodeType.CookMode)
                    End If
                    ' RDC 11.14.2013 : Option to display or not to display Procedure/preparation
                    If G_ExportOptions.blnExpIncludeProcedure Then
                        Select Case G_ExportOptions.intExpSelectedProcedure
                            Case 0
                                'Method Header
                                If strMethodHeader.ToString <> "" Then
                                    strHTMLContent.Append("<tr>")
                                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; font-weight: bold'>")
                                    strHTMLContent.Append(strMethodHeader.ToString)
                                    strHTMLContent.Append("</td>")
                                    strHTMLContent.Append("</tr>")
                                End If
                                ''Case 1
                                'Directions
                                If strDirections.ToString <> "" Then
                                    strHTMLContent.Append("<tr>")
                                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                                    'strHTMLContent.Append(strDirections.ToString.Replace(Chr(13) & Chr(10), "<br>")) ' TDQ 11.14.2011
                                    strHTMLContent.Append(strDirections.ToString)
                                    strHTMLContent.Append("</td>")
                                    strHTMLContent.Append("</tr>")
                                End If
                            Case Else
                                'Method Header
                                If strMethodHeader.ToString <> "" Then
                                    strHTMLContent.Append("<tr>")
                                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; font-weight: bold'>")
                                    strHTMLContent.Append(strMethodHeader.ToString)
                                    strHTMLContent.Append("</td>")
                                    strHTMLContent.Append("</tr>")
                                End If

                                'Directions
                                If strAbbrDirections.ToString <> "" Then
                                    strHTMLContent.Append("<tr>")
                                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                                    'strHTMLContent.Append(strDirections.ToString.Replace(Chr(13) & Chr(10), "<br>")) ' TDQ 11.14.2011
                                    strHTMLContent.Append(strAbbrDirections.ToString)
                                    strHTMLContent.Append("</td>")
                                    strHTMLContent.Append("</tr>")
                                End If
                        End Select

                    End If

                    strHTMLContent.Append("</table>")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")

                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td>")
                    strHTMLContent.Append("&nbsp;")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")

                    ''AMTLA 2013.11.21  CWM-9602
                    If G_ExportOptions.blnExpIncludeNotes Then
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td>")
                        strHTMLContent.Append("<table style='width: 620'>")
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; font-weight: bold'>")
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.Notes))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strFootNote1.ToString.Replace(Chr(13) & Chr(10), "<br>"))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                        strHTMLContent.Append("</table>")
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                    End If

                    ''AMTLA 2013.11.21  CWM-9602
                    If G_ExportOptions.blnExpIncludeAddNotes Then
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td>")
                        strHTMLContent.Append("<table style='width: 620'>")
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; font-weight: bold'>")
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.AdditionalNotes))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strFootNote2.ToString.Replace(Chr(13) & Chr(10), "<br>"))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                        strHTMLContent.Append("</table>")
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                    End If

                    ' RDC 11.14.2013 : Display or not to display Nutrient info
                    ' RDC 11.26.2013 : Added new routine for displaying nutrient computation
                    If G_ExportOptions.blnExpIncludeNutrientInfo And dsRecipeDetails.Tables("Table4").Rows.Count > 0 Then
                        Dim strNutBasis As String = ""
                        If Not IsDBNull(dsRecipeDetails.Tables("Table4").Rows(0).Item("NutritionBasis")) Then strNutBasis = dsRecipeDetails.Tables("Table4").Rows(0).Item("NutritionBasis")
                        strHTMLContent.Append(fctDisplayNutrientComputationForExport(m_RecipeId, strServingsUnit, G_ExportOptions.intExpSelectedNutrientSet, G_ExportOptions.intExpSelectedLanguage, G_ExportOptions.intExpSelectedNutrientComputation, , strNutBasis, m_Version, True))

                        'Net Carbs
                        If isDisplay = True Then
                            If strNetCarbohydrates.ToString <> "" Then
                                strHTMLContent.Append("<tr>")
                                strHTMLContent.Append("<td>")
                                strHTMLContent.Append("<table style='width: 620'>")
                                strHTMLContent.Append("<tr>")
                                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
                                strHTMLContent.Append(strNetCarbohydrates.ToString)
                                strHTMLContent.Append("</td>")
                                strHTMLContent.Append("</tr>")
                                strHTMLContent.Append("<tr>")
                                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                                strHTMLContent.Append(lblNetCarbs.ToString)
                                strHTMLContent.Append("</td>")
                                strHTMLContent.Append("</tr>")
                                strHTMLContent.Append("</table>")
                                strHTMLContent.Append("</td>")
                                strHTMLContent.Append("</tr>")
                            End If
                        End If
                        strHTMLContent.Append("</table>")

                        strNutrients = ""
                    End If

                    ' RDC 02.11.2014 : GDA
                    If G_ExportOptions.blnExpIncludeGDA Then
                        strHTMLContent.Append(fctDisplayGDAComputationForExport(m_RecipeId, dsRecipeDetails.Tables(1).Rows(0).Item("ServingsUnit").ToString, G_ExportOptions.intExpSelectedNutrientSet, G_ExportOptions.intExpSelectedLanguage, G_ExportOptions.intExpSelectedGDA, , "", m_Version, True))
                    End If

                    'Information
                    ' RDC 11.13.2013 : Option to display or not to display Information
                    If G_ExportOptions.blnExpAdvIncludeInfo Then
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td>")

                        strHTMLContent.Append("<table style='width: 620'>")
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td colspan='2' style='text-align: center; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        'strHTMLContent.Append(lblInformation.ToString)
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.Information))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='vertical-align: top;'>")
                        strHTMLContent.Append("<table style='width: 500'>")
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        'strHTMLContent.Append(lblRecipeStatus.ToString)
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.RecipeStatus))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strRecipeStatus.ToString)
                        strHTMLContent.Append("</td>")
                        'strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
                        'strHTMLContent.Append("&nbsp;")
                        'strHTMLContent.Append("</td>")
                        'AGL 2012.10.31 - CWM-1971
                        If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
                            'AGL 2013.05.16 - removed width
                            'strHTMLContent.Append("<td style='text-align: right; width: 100; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                            strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                            'strHTMLContent.Append(lblUpdatedBy.ToString)
                            strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.UpdatedBy))
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                            strHTMLContent.Append(strUpdatedBy.ToString)
                            strHTMLContent.Append("</td>")
                        End If
                        strHTMLContent.Append("</tr>")



                        'AGL 2012.10.31 - CWM-1971
                        If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
                            strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                            'strHTMLContent.Append(lblWebStatus.ToString)
                            strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.WebStatus))
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                            strHTMLContent.Append(strWebStatus.ToString)
                            strHTMLContent.Append("</td>")
                            'strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
                            'strHTMLContent.Append("&nbsp;")
                            'strHTMLContent.Append("</td>")
                            'strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                            'strHTMLContent.Append("&nbsp;")
                            'strHTMLContent.Append("</td>")
                            'strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                            'strHTMLContent.Append("&nbsp;")
                            'strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")

                        End If
                        'AGL 2013.05.06 - 4728 - brought out Date Created
                        strHTMLContent.Append("<tr>")

                        strHTMLContent.Append("</tr>")

                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        'strHTMLContent.Append(lblDateCreated.ToString)
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.DateCreated))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strDateCreated.ToString)
                        strHTMLContent.Append("</td>")
                        'strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
                        'strHTMLContent.Append("&nbsp;")
                        'strHTMLContent.Append("</td>")

                        'AGL 2013.05.16 - removed width
                        'strHTMLContent.Append("<td style='text-align: right; width: 100; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        'strHTMLContent.Append(lblCreatedBy.ToString)
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.CreatedBY))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strCreatedBy.ToString)
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")



                        strHTMLContent.Append("<tr>")
                        'AGL 2012.10.31 - CWM-1971
                        'If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
                        strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        'strHTMLContent.Append(lblDateLastModified.ToString)
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.DateLastModified))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strDateLastModified.ToString)
                        strHTMLContent.Append("</td>")
                        'strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
                        'strHTMLContent.Append("&nbsp;")
                        'strHTMLContent.Append("</td>")
                        'End If
                        'AGL 2013.05.16 - removed width
                        'strHTMLContent.Append("<td style='text-align: right; width: 100; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        'strHTMLContent.Append(lblModifiedBy.ToString)
                        strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.ModifiedBy))
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strModifiedBy.ToString)
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                        'AGL 2012.10.31 - CWM-1971
                        If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                            'strHTMLContent.Append(lblLastTested.ToString)\
                            strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.DateLastTested))
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                            strHTMLContent.Append(strLastTested.ToString)
                            strHTMLContent.Append("</td>")
                            'strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
                            'strHTMLContent.Append("&nbsp;")
                            'strHTMLContent.Append("</td>")

                            'AGL 2013.05.16 - removed width
                            'strHTMLContent.Append("<td style='text-align: right; width: 100; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                            strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                            'strHTMLContent.Append(lblTestedBy.ToString)
                            strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.TestedBy))
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                            strHTMLContent.Append(strTestedBy.ToString)
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                            'strHTMLContent.Append(lblDateDeveloped.ToString)
                            strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.DateDeveloped))
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                            strHTMLContent.Append(strDateDeveloped.ToString)
                            strHTMLContent.Append("</td>")
                            'strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
                            'strHTMLContent.Append("&nbsp;")
                            'strHTMLContent.Append("</td>")
                            'AGL 2013.05.16 - removed width
                            'strHTMLContent.Append("<td style='text-align: right; width: 100; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                            strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                            'strHTMLContent.Append(lblDevelopedBy.ToString)
                            strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.DateDeveloped))
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                            strHTMLContent.Append(strDevelopedBy.ToString)
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                            'strHTMLContent.Append(lblDateOfFinalEdit.ToString)
                            strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.FinalEditDate))
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                            strHTMLContent.Append(strDateOfFinalEdit.ToString)
                            strHTMLContent.Append("</td>")
                            'strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; width: 50px;'>")
                            'strHTMLContent.Append("&nbsp;")
                            'strHTMLContent.Append("</td>")
                            'AGL 2013.05.16 - removed width
                            'strHTMLContent.Append("<td style='text-align: right; width: 100; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                            strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                            'strHTMLContent.Append(lblFinalEditBy.ToString)
                            strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.FinalEditBy))
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                            strHTMLContent.Append(strFinalEditBy.ToString)
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td style='text-align: right; font-weight: bold; font-size: 11.5pt; font-family: Calibri; vertical-align: top; width:200'>")
                            'strHTMLContent.Append(lblDevelopmentPurpose.ToString)
                            strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.DevelopmentPurpose))
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; vertical-align: top;' colspan=4>")
                            strHTMLContent.Append(strDevelopmentPurpose.ToString)
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")
                        End If
                        strHTMLContent.Append("</table>")
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                        strHTMLContent.Append("</table>")
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                    End If

                    'Recipe Brand
                    ' RDC 11.13.2013 : Option to display or not to display Brand
                    If G_ExportOptions.blnExpAdvIncludeBrands Then
                        If dsRecipeDetails.Tables("table8").Rows.Count > 0 Then
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td>")
                            strHTMLContent.Append("<table style='width: 620'>")
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: center' colspan='2'>")
                            'strHTMLContent.Append(lblRecipeBrand.ToString)
                            strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.Brand))
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")

                            For Each Brands As DataRow In dsRecipeDetails.Tables("table8").Rows
                                strRecipeBrand = fctCheckDbNull(Brands.Item("BrandName"))
                                strRecipeBrandClassification = fctCheckDbNull(Brands.Item("BrandClassification"))
                                strHTMLContent.Append("<tr>")
                                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                                'AGL 2012.10.31 - CWM-1971
                                ' RDC 11.15.2013 : Added Or clsLicense.l_App = EgswKey.clsLicense.enumApp.RB
                                If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Or clsLicense.l_App = EgswKey.clsLicense.enumApp.RB Then
                                    strHTMLContent.Append(strRecipeBrand.ToString & " - " & strRecipeBrandClassification.ToString)
                                Else
                                    strHTMLContent.Append(strRecipeBrand.ToString)
                                End If

                                strHTMLContent.Append("</td>")
                                strHTMLContent.Append("</tr>")
                            Next

                            strHTMLContent.Append("</table>")
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")
                        End If
                    End If

                    ' RDC 11.14.2013 : Moved as per presented on specs
                    If G_ExportOptions.blnExpAdvIncludeKeywords Then
                        'Attributes
                        If dsRecipeDetails.Tables("table7").Rows.Count > 0 Then
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td>")
                            strHTMLContent.Append("<table style='width: 620'>")
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td style='font-weight: bold; text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                            ' RDC 11.14.2013 : Replaced Attributes to Keywords
                            'strHTMLContent.Append(lblAttributes.ToString)
                            strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.Keywords))
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")

                            ' RDC 12.02.2013 : Replaced code below.
                            For Each drKeywords As DataRow In dsRecipeDetails.Tables("Table7").Rows
                                strHTMLContent.Append("<tr>")
                                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                                strHTMLContent.Append(drKeywords("Name"))
                                strHTMLContent.Append("</td>")
                                strHTMLContent.Append("</tr>")
                            Next
                            strHTMLContent.Append("</table>")
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")
                        End If
                    End If

                    ' Cookbooks
                    ' RDC 11.14.2013 : Adding cookbooks section to the report
                    If G_ExportOptions.blnExpAdvIncludeCookbook Then
                        If dsRecipeDetails.Tables(10).Rows.Count > 0 Then
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td>")
                            strHTMLContent.Append("<table style='width: 620'>")
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td style='font-weight: bold; text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                            strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.Cookbook))
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")

                            For Each rwCookbooks As DataRow In dsRecipeDetails.Tables(10).Rows
                                strHTMLContent.Append("<tr><td style='font-size: 11.5pt; font-family: Calibri;'>")
                                strHTMLContent.Append(rwCookbooks("Name").ToString)
                                strHTMLContent.Append("</td></tr>")
                            Next

                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")
                            strHTMLContent.Append("</table>")
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")

                        End If
                    End If

                    ' RDC 11.14.2013 : Moved as per presented on specs
                    If G_ExportOptions.blnExpAdvIncludePublication Then
                        'Placements
                        If dsRecipeDetails.Tables("table9").Rows.Count > 0 Then
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td>")
                            strHTMLContent.Append("<table style='width: 620'>")
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: center' colspan='3'>")
                            strHTMLContent.Append(lblPlacements.ToString)
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td>")
                            strHTMLContent.Append("<table>")
                            For Each Placements As DataRow In dsRecipeDetails.Tables("table9").Rows
                                strPlacementName = fctCheckDbNull(Placements.Item("Placement"))
                                If Not IsDBNull(Placements.Item("PlacementDate")) Then strPlacementDate = CDate(Placements.Item("PlacementDate")).ToString("MM/dd/yyyy")
                                strPlacementDescription = fctCheckDbNull(Placements.Item("Description"))

                                strHTMLContent.Append("<tr>")
                                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; padding-right: 10px'>")
                                strHTMLContent.Append(strPlacementName.ToString)
                                strHTMLContent.Append("</td>")
                                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; padding-right: 10px'>")
                                strHTMLContent.Append(strPlacementDate.ToString)
                                strHTMLContent.Append("</td>")
                                strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri; padding-right: 10px'>")
                                strHTMLContent.Append(strPlacementDescription.ToString)
                                strHTMLContent.Append("</td>")
                                strHTMLContent.Append("</tr>")
                            Next
                            strHTMLContent.Append("</table>")
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")
                            strHTMLContent.Append("</table>")
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")
                        End If

                        'AGL 2013.11.06
                        'strHTMLContent.Append("<tr>")
                        'strHTMLContent.Append("<td>")
                        'strHTMLContent.Append("&nbsp;")
                        'strHTMLContent.Append("</td>")
                        'strHTMLContent.Append("</tr>")
                    End If

                    ' RDC 11.14.2013 : Moved as per presented on specs
                    If G_ExportOptions.blnExpAdvIncludeComments Then
                        'Comments
                        If dsRecipeDetails.Tables("table6").Rows.Count > 0 Then
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td>")
                            strHTMLContent.Append("<table style='width: 620'>")
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td style='font-weight: bold; text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                            'strHTMLContent.Append(lblComments.ToString)
                            strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.Comments))
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")
                            strHTMLContent.Append("<tr>")
                            strHTMLContent.Append("<td>")
                            strHTMLContent.Append("<table>")

                            strHTMLContent.Append("<p style='padding-right: 0px; font-size: 11.5pt; font-family: Calibri; vertical-align: top;'>")
                            For Each Comments As DataRow In dsRecipeDetails.Tables("table6").Rows
                                If Not IsDBNull(Comments.Item("SubmitDate")) Then strSubmitDate = CDate(Comments.Item("SubmitDate")).ToString("MM/dd/yyyy")
                                strOwnerName = fctCheckDbNull(Comments.Item("OwnerName"))
                                strComments = fctCheckDbNull(Comments.Item("Description"))

                                strHTMLContent.Append("<tr>")
                                strHTMLContent.Append("<td style='padding-right: 10px; font-size: 11.5pt; font-family: Calibri; vertical-align: top;'>")
                                strHTMLContent.Append(strSubmitDate.ToString)
                                strHTMLContent.Append("</td>")
                                strHTMLContent.Append("<td style='padding-right: 0px; font-size: 11.5pt; font-family: Calibri; vertical-align: top; width: 130px;'>")
                                strHTMLContent.Append(strOwnerName.ToString)
                                strHTMLContent.Append("</td>")
                                strHTMLContent.Append("<td style='padding-right: 10px; font-size: 11.5pt; font-family: Calibri; vertical-align: top;'>")
                                strHTMLContent.Append(strComments.ToString)
                                strHTMLContent.Append("</td>")
                                strHTMLContent.Append("</tr>")
                            Next
                            strHTMLContent.Append("</table>")
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")

                            strHTMLContent.Append("</table>")
                            strHTMLContent.Append("</td>")
                            strHTMLContent.Append("</tr>")
                        End If
                    End If

                Else
                    'strHTMLContent.Append("<table style='width: 620'>")
                    'strHTMLContent.Append("<tr>")
                    'strHTMLContent.Append("<td>")
                    strHTMLContent.Append("<table style='width: 620'>")

                    'Recipe Name
                    strHTMLContent.Append("<p style='font-weight: bold; font-size: x-large; text-align: center; font-family: Calibri;'>")
                    strHTMLContent.Append(strRecipeName.ToString)
                    strHTMLContent.Append("</p>")

                    'Recipe Number
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td>")
                    strHTMLContent.Append("<table>")
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(lblRecipeNumber.ToString)
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strRecipeNumber.ToString)
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                    strHTMLContent.Append("</table>")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")

                    'strHTMLContent.Append("<tr>")
                    'strHTMLContent.Append("<td>")
                    'strHTMLContent.Append("&nbsp;")
                    'strHTMLContent.Append("</td>")
                    'strHTMLContent.Append("</tr>")

                    'Sub Title
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td>")
                    strHTMLContent.Append("<table>")
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(lblSubTitle.ToString)
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strSubTitle.ToString)
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                    strHTMLContent.Append("</table>")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")

                    'strHTMLContent.Append("<tr>")
                    'strHTMLContent.Append("<td>")
                    'strHTMLContent.Append("&nbsp;")
                    'strHTMLContent.Append("</td>")
                    'strHTMLContent.Append("</tr>")
                    'strHTMLContent.Append("</table>")

                    'Description
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td>")
                    strHTMLContent.Append("<table>")
                    strHTMLContent.Append("<tr>")
                    If strRecipeDescription.ToString <> "" Then
                        strHTMLContent.Append("<td style='font-weight: bold; width: 80 ;font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(lblRecipeDescription.ToString)
                        strHTMLContent.Append("</td>")
                    End If
                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strRecipeDescription.ToString)
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                    strHTMLContent.Append("</table>")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")

                    'strHTMLContent.Append("<tr>")
                    'strHTMLContent.Append("<td>")
                    'strHTMLContent.Append("&nbsp;")
                    'strHTMLContent.Append("</td>")
                    'strHTMLContent.Append("</tr>")

                    'Remark
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td>")
                    strHTMLContent.Append("<table>")
                    strHTMLContent.Append("<tr>")
                    If strRecipeRemark.ToString <> "" Then
                        strHTMLContent.Append("<td style='font-weight: bold; width: 80 ;font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(lblRecipeRemark.ToString)
                        strHTMLContent.Append("</td>")
                    End If
                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strRecipeRemark.ToString)
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                    strHTMLContent.Append("</table>")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")

                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td>")
                    strHTMLContent.Append("&nbsp;")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")

                    Dim strImage2 As String

                    strImage2 = dtRecipes.Rows(x).Item("ImageLoc2").ToString
                    intStandard = dtRecipes.Rows(x).Item("Ratings").ToString

                    imgRecipe = strImage
                    strHTMLContent.Append("<p style='font-weight: bold; font-size: x-large; text-align: center; font-family: Calibri;'>")
                    strHTMLContent.Append(imgRecipe) 'CMV 051911

                    If Not strImage.Contains("nopic.jpg") And Not strImage2.Contains("nopic.jpg") Then
                        If Not strImage2 = "" Then
                            'strHTMLContent.Append(" <td style='text-align: center'>")
                            strHTMLContent.Append("&nbsp")
                            imgRecipe2 = strImage2 ' getHtml(imageRecipe) 'CMV 051911
                            strHTMLContent.Append(imgRecipe2) 'CMV 051911
                            'strHTMLContent.Append("</td>")
                        End If
                    End If

                    strHTMLContent.Append("</p>")

                    ''Image
                    'imgRecipe = strImage 'getHtml(imageRecipe) 'CMV 051911
                    ''strHTMLContent.Append(imgRecipe) 'CMV 051911
                    'strHTMLContent.Append("<p style='font-weight: bold; font-size: x-large; text-align: center; font-family: Calibri;'>")
                    'strHTMLContent.Append(imgRecipe) 'CMV 051911
                    ''strHTMLContent.Append("<img src='" & imgRecipe & "' height=240 width=240 />")
                    'strHTMLContent.Append("</p>")

                    ''Recipe Name
                    'strHTMLContent.Append("<p style='font-weight: bold; font-size: x-large; text-align: center; font-family: Calibri;'>")
                    'strHTMLContent.Append(strRecipeName.ToString)
                    'strHTMLContent.Append("</p>")

                    'Subheading
                    strHTMLContent.Append("<p style='font-weight: bold; text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strSubHeading.ToString)
                    strHTMLContent.Append("</p>")

                    'Servings
                    strHTMLContent.Append("<p style='text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strServings.ToString)
                    strHTMLContent.Append("</p>")

                    'Recipe Time
                    strHTMLContent.Append("<p style='text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                    For Each RecipeTime As DataRow In dsRecipeDetails.Tables("table5").Rows
                        strRecipeTime = RecipeTime.Item("Description")
                        strHTMLContent.Append(strRecipeTime.ToString)
                        strHTMLContent.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
                        strRecipeTime = ""
                    Next
                    strHTMLContent.Append("</p>")

                    'Ingredients
                    If dsRecipeDetails.Tables("table2").Rows.Count > 0 Then
                        strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")
                        '-- JBB 05.24.2011 (code pass by Cielo)
                        For Each Ingredients As DataRow In dsRecipeDetails.Tables("table2").Rows
                            If Ingredients.Item("Type").ToString().Trim() <> "4" Then
                                '    strIngredients = fctCheckDbNull(Ingredients.Item("Description"))
                                'Else
                                If bitQtyFormat = 0 Then
                                    'strIngredients = fctCheckDbNullNumeric(Ingredients.Item("Quantity_Metric")) & " " & fctCheckDbNull(Ingredients.Item("UOM_Metric")) & " " & fctCheckDbNull(Ingredients.Item("Complement")) & " " & fctCheckDbNull(Ingredients.Item("Name")) & "," & fctCheckDbNull(Ingredients.Item("Preparation"))
                                    ''-- JBB 10.26.2011
                                    'If fctCheckDbNull(Ingredients.Item("AlternativeIngredient")) <> "" Then
                                    '    strIngredients = strIngredients & " or " & fctCheckDbNull(Ingredients.Item("AlternativeIngredient"))
                                    'End If


                                    'fctCheckDbNull(Ingredients.Item("Complement") & " " & _
                                    'fctCheckDbNull(Ingredients.Item("Name") 
                                    'TDQ 11022011
                                    If fctCheckDbNull(Ingredients.Item("AlternativeIngredient")) = "" Then
                                        strIngredients = IIf(Ingredients.Item("Quantity_Metric") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Metric"))) & " " & _
                                          fctCheckDbNull(Ingredients.Item("UOM_Metric")) & " " & _
                                          IIf(blnAfterIngredient, fctCheckDbNull(Ingredients.Item("Name")) & " " & fctCheckDbNull(Ingredients.Item("Complement")), _
                                              fctCheckDbNull(Ingredients.Item("Complement")) & " " & fctCheckDbNull(Ingredients.Item("Name"))) & " " & _
                                          IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & _
                                          fctCheckDbNull(Ingredients.Item("Preparation"))
                                    Else
                                        strIngredients = IIf(Ingredients.Item("Quantity_Metric") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Metric"))) & " " & _
                                          fctCheckDbNull(Ingredients.Item("UOM_Metric")) & " " & _
                                          IIf(blnAfterIngredient, fctCheckDbNull(Ingredients.Item("Name")) & " " & fctCheckDbNull(Ingredients.Item("Complement")), _
                                              fctCheckDbNull(Ingredients.Item("Complement")) & " " & fctCheckDbNull(Ingredients.Item("Name"))) & " &#91or " & _
                                          fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & _
                                          fctCheckDbNull(Ingredients.Item("Preparation")))
                                    End If

                                    'If fctCheckDbNull(Ingredients.Item("Preparation")) = "" Then 'TDQ 10172011
                                    '    strIngredients = strIngredients.Substring(0, strIngredients.Length - 1)
                                    'End If

                                    '-- JBB 10.25.2011
                                    'strIngredients = strIngredients.Replace("0 N/A", "")
                                    'strIngredients = strIngredients.Replace("0 n/a", "")
                                    strIngredients = strIngredients.Replace("N/A", "")
                                    strIngredients = strIngredients.Replace("n/a", "")
                                    strIngredients = strIngredients + "<br>"

                                    '--
                                ElseIf bitQtyFormat = 1 Then
                                    If IsNumeric(Ingredients.Item("Quantity_Imperial")) = True Then
                                        If fctCheckDbNull(Ingredients.Item("AlternativeIngredient")) = "" Then 'TDQ 10172011
                                            If blnUseFractions Then
                                                strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctConvertToFraction2(fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial")))) & " " & _
                                                 fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " & _
                                                 fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & fctCheckDbNull(Ingredients.Item("Preparation")))) 'TDQ 10142011
                                            Else
                                                strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial"))) & " " & _
                                                 fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " & _
                                                 fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & fctCheckDbNull(Ingredients.Item("Preparation")))) 'TDQ 10142011
                                            End If
                                        Else
                                            If blnUseFractions Then
                                                strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctConvertToFraction2(fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial")))) & " " & _
                                                 fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " & _
                                                 fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & " &#91or " & fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & fctCheckDbNull(Ingredients.Item("Preparation"))))) 'TDQ 10142011
                                            Else
                                                strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial"))) & " " & _
                                                 fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " & _
                                                 fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & " &#91or " & fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & fctCheckDbNull(Ingredients.Item("Preparation"))))) 'TDQ 10142011
                                            End If

                                        End If
                                        '-- JBB 10.25.2011
                                        'strIngredients = strIngredients.Replace("0 N/A", "")
                                        'strIngredients = strIngredients.Replace("0 n/a", "")
                                        strIngredients = strIngredients.Replace("N/A", "")
                                        strIngredients = strIngredients.Replace("n/a", "")

                                        '--
                                    Else
                                        If fctCheckDbNull(Ingredients.Item("AlternativeIngredient")) = "" Then 'TDQ 10172011
                                            strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNull(Ingredients.Item("Quantity_Imperial"))) & " " & _
                                              fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " &
                                              fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & fctCheckDbNull(Ingredients.Item("Preparation")))) 'TDQ 10142011
                                        Else
                                            strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNull(Ingredients.Item("Quantity_Imperial"))) & " " & _
                                              fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " &
                                              fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & " &#91or " & fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & fctCheckDbNull(Ingredients.Item("Preparation"))))) 'TDQ 10142011
                                        End If
                                        '-- JBB 10.25.2011
                                        'strIngredients = strIngredients.Replace("0 N/A", "")
                                        'strIngredients = strIngredients.Replace("0 n/a", "")
                                        strIngredients = strIngredients.Replace("N/A", "")
                                        strIngredients = strIngredients.Replace("n/a", "")

                                        '--
                                    End If

                                    'If fctCheckDbNull(Ingredients.Item("Preparation")) = "" Then 'TDQ 10172011
                                    '    strIngredients = strIngredients.Substring(0, strIngredients.Length - 1)
                                    'End If

                                    strIngredients = strIngredients + "<br>"

                                ElseIf bitQtyFormat = 2 Then
                                    strIngredients = fctCheckDbNull(Ingredients.Item("Description"))
                                ElseIf bitQtyFormat = 3 Then ' JBB 07.08.2011
                                    Dim strM As String = IIf(Ingredients.Item("Quantity_Metric") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Metric"))) & " " & fctCheckDbNull(Ingredients.Item("UOM_Metric")) & " "
                                    '-- JBB 10.25.2011
                                    'strM = strM.Replace("0 N/A", "")
                                    'strM = strM.Replace("0 n/a", "")
                                    strM = strM.Replace("N/A", "")
                                    strM = strM.Replace("n/a", "")
                                    '--
                                    Dim strI As String = ""
                                    If IsNumeric(Ingredients.Item("Quantity_Imperial")) = True Then
                                        If blnUseFractions Then
                                            strI = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctConvertToFraction2(fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial")))) & " " & fctCheckDbNull(Ingredients.Item("UOM_Imperial"))
                                        Else
                                            strI = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial"))) & " " & fctCheckDbNull(Ingredients.Item("UOM_Imperial"))
                                        End If
                                    Else
                                        strI = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNull(Ingredients.Item("Quantity_Imperial"))) & " " & fctCheckDbNull(Ingredients.Item("UOM_Imperial"))
                                    End If
                                    '-- JBB 10.25.2011
                                    ' strI = strI.Replace("0 N/A", "")
                                    ' strI = strI.Replace("0 n/a", "")
                                    strI = strI.Replace("N/A", "")
                                    strI = strI.Replace("n/a", "")

                                    Dim strIngName As String

                                    'TDQ 11022011
                                    If fctCheckDbNull(Ingredients.Item("AlternativeIngredient")) = "" Then
                                        strIngName = fctCheckDbNull(Ingredients.Item("Complement") & " " & _
                                          fctCheckDbNull(Ingredients.Item("Name") & IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & _
                                          fctCheckDbNull(Ingredients.Item("Preparation"))))
                                    Else
                                        strIngName = fctCheckDbNull(Ingredients.Item("Complement") & " " & _
                                          fctCheckDbNull(Ingredients.Item("Name") & " &#91or " & _
                                          fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & _
                                          fctCheckDbNull(Ingredients.Item("Preparation")))))
                                    End If

                                    'If fctCheckDbNull(Ingredients.Item("Preparation")) = "" Then
                                    '    strIngName = strIngName.Substring(0, strIngName.Length - 1)
                                    'End If

                                    '--
                                    Dim strTempTemp As String = "<table  style='font-size: 11.5pt; font-family: Calibri;'><tr><td style='width: 100' valign='top'>%M</td><td style='width: 100' valign='top'>%I</td><td valign='top'>%N</td></tr></table>"
                                    strIngredients = strTempTemp.Replace("%M", strM).Replace("%I", strI).Replace("%N", strIngName)
                                End If
                            Else ' JBB 07.14.2011 if Text
                                If bitQtyFormat = 0 Then
                                    strIngredients = fctCheckDbNull(Ingredients.Item("Name"))
                                    strIngredients = strIngredients + "<br>"
                                ElseIf bitQtyFormat = 1 Then
                                    strIngredients = fctCheckDbNull(Ingredients.Item("Name"))
                                    strIngredients = strIngredients + "<br>"
                                ElseIf bitQtyFormat = 2 Then
                                    strIngredients = fctCheckDbNull(Ingredients.Item("Description"))
                                Else
                                    Dim strTempTemp As String = "<table  style='font-size: 11.5pt; font-family: Calibri;width:620px'><tr><td style='width: 100' valign='top'>&nbsp</td><td style='width: 100' valign='top'>&nbsp</td><td valign='top'>%N</td></tr></table>"
                                    strIngredients = strTempTemp.Replace("%N", fctCheckDbNull(Ingredients.Item("Name")))
                                End If
                            End If
                            'strHTMLContent.Append("<tr>")
                            'strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                            strHTMLContent.Append(strIngredients.ToString)
                            '--strHTMLContent.Append("<br>") ' JBB 10.25.2011
                            'strHTMLContent.Append("</td>")
                            'strHTMLContent.Append("</tr>")
                        Next
                        '--
                        'For Each Ingredients As DataRow In dsRecipeDetails.Tables("table2").Rows
                        '    If Ingredients.Item("Description") <> "" Then
                        '        strIngredients = fctCheckDbNull(Ingredients.Item("Description"))
                        '    Else
                        '        strIngredients = fctCheckDbNullNumeric(Ingredients.Item("Quantity")) & " " & fctCheckDbNull(Ingredients.Item("UOM")) & " " & fctCheckDbNull(Ingredients.Item("Name"))
                        '    End If
                        '    strHTMLContent.Append(strIngredients.ToString)
                        '    strHTMLContent.Append("<br>")
                        'Next
                        '

                        strHTMLContent.Append("</p>")
                    End If

                    'Method Header
                    If strMethodHeader.ToString <> "" Then
                        strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri; font-weight: bold' align='center'>")
                        strHTMLContent.Append(strMethodHeader.ToString)
                        strHTMLContent.Append("</p>")
                    End If

                    'Directions
                    If strDirections.ToString <> "" Then
                        strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")
                        'strHTMLContent.Append(strDirections.ToString.Replace(Chr(13) & Chr(10), "<br>"))
                        strHTMLContent.Append(strDirections.ToString)
                        strHTMLContent.Append("</p>")
                    End If

                    'Footnote 1
                    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strFootNote1.ToString.Replace(Chr(13) & Chr(10), "<br>"))
                    'strHTMLContent.Append(strFootNote1)
                    strHTMLContent.Append("</p>")

                    'Footnote 2
                    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strFootNote2.ToString.Replace(Chr(13) & Chr(10), "<br>"))
                    'strHTMLContent.Append(strFootNote2)
                    strHTMLContent.Append("</p>")

                    ''Placements
                    'If dsRecipeDetails.Tables("table9").Rows.Count > 0 Then
                    '    strHTMLContent.Append("<p style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: center'>")
                    '    strHTMLContent.Append(lblPlacements.ToString)
                    '    strHTMLContent.Append("</p>")
                    '    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri; padding-right: 10px'>")
                    '    For Each Placements As DataRow In dsRecipeDetails.Tables("table9").Rows
                    '        strPlacementName = fctCheckDbNull(Placements.Item("Placement"))
                    '        If Not IsDBNull(Placements.Item("PlacementDate")) Then strPlacementDate = CDate(Placements.Item("PlacementDate")).ToString("MM/dd/yyyy")
                    '        strPlacementDescription = fctCheckDbNull(Placements.Item("Description"))

                    '        strHTMLContent.Append(strPlacementName.ToString)
                    '        strHTMLContent.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
                    '        strHTMLContent.Append(strPlacementDate.ToString)
                    '        strHTMLContent.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
                    '        strHTMLContent.Append(strPlacementDescription.ToString)
                    '        strHTMLContent.Append("<br>")
                    '    Next
                    '    strHTMLContent.Append("</p>")
                    'End If

                    'Nutrients
                    If isDisplay = True Then
                        If dsRecipeDetails.Tables("table4").Rows.Count > 0 Then
                            strHTMLContent.Append("<p style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
                            '-- JBB 02.23.2012
                            Dim strNutBasis As String = fctCheckDbNull(dsRecipeDetails.Tables("table4").Rows(0).Item("NutritionBasis"))
                            If lblNutritionalInformation.ToString().Trim <> "" Then '-- JBB 02.23.2012
                                If strNutBasis = "" Then
                                    strHTMLContent.Append(lblNutritionalInformation.ToString & " :")
                                Else
                                    strHTMLContent.Append(lblNutritionalInformation.ToString & "(" & strNutBasis & ") :")
                                End If
                            End If
                            'strHTMLContent.Append(lblNutritionalInformation.ToString)
                            '--
                            strHTMLContent.Append("</p>")
                            strHTMLContent.Append(strNutrients.ToString)
                        End If

                        'Net Carbs
                        If strNetCarbohydrates.ToString <> "" Then
                            strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
                            strHTMLContent.Append(strNetCarbohydrates.ToString)
                            strHTMLContent.Append("</p>")
                            strHTMLContent.Append(lblNetCarbs.ToString)
                        End If
                    End If
                    strNutrients = ""
                End If

                If Not rowCount = x + 1 Then
                    strHTMLContent.Append("<br style='page-break-before:always' />")
                End If

            Else
                strErr = cLang.GetString(clsEGSLanguage.CodeType.FileNotFound)
            End If
        Next

        strHTMLContent.Append("</table></div></body></html>")
        strErr = ""

        Return strHTMLContent
    End Function


    'JBB 07.14.2011
    Private Function fctGetInstrunctions(ByVal dtMethod As DataTable, ByVal strFormat As String, Optional ByVal blCookmode As Boolean = False) As String
        Dim strInstruction As String = ""
        strFormat = strFormat.Trim
        If strFormat = "" Then strFormat = "X"
        Dim strTemplate As String = fGetMethodFormat(strFormat)
        If strTemplate.IndexOf("%s") > 0 Then
            Dim strMethod As New StringBuilder
            For Each drMethod As DataRow In dtMethod.Rows
                'If drMethod("Description").ToString.Trim <> "" Then
                Dim strData As String = drMethod("note").ToString()
                If strData <> "" Then strData = System.Web.HttpUtility.HtmlEncode(strData)
                strData = strData.Replace(Chr(9), "&#9;")
                strData = strData.Replace(Chr(32), "&#32;")
                strData = strData.Replace(Chr(10), "<br />")
                strData = strData.Replace(Chr(13), "<br />")
                '-- JBB 10.20.2011 Cookmode
                If blCookmode = True Then
                    If dtMethod.Columns.Contains("CookMode") Then
                        If drMethod("CookMode").ToString.Trim() <> "" Then
                            strData = drMethod("CookMode").ToString.Trim
                            strMethod.Append(strData + "<br><br>")
                        Else
                            strData = ""
                        End If
                    End If
                Else
                    If strData <> "" Then
                        strMethod.Append(strData + "<br><br>")
                    End If

                End If
            Next
            strInstruction = strTemplate.Replace("%s", strMethod.ToString())
        Else
            ' RDC 12.10.2013
            Dim strList As String = "%s<br>"
            'Dim strList As String = "%s<br><br>"
            Dim strMethod As New StringBuilder
            Dim i As String = 1
            Dim x As String = 1
            Dim strData As String
            For Each drMethod As DataRow In dtMethod.Rows

                If strFormat = "N" Then
                    If drMethod("note").ToString = "" Then
                        strData = drMethod("note").ToString()
                    Else
                        strData = i + ".) " + drMethod("note").ToString()
                        i = i + 1
                    End If
                Else
                    If drMethod("note").ToString = "" Then
                        strData = drMethod("note").ToString()
                    Else
                        strData = "&#149" + "    " + drMethod("note").ToString()
                    End If
                End If
                '-- JBB 10.20.2011 Cookmode
                If blCookmode = True Then
                    If strFormat = "N" Then
                        If dtMethod.Columns.Contains("CookMode") Then
                            If drMethod("CookMode").ToString.Trim() <> "" Then
                                strData = x + ".) " + drMethod("CookMode").ToString.Trim
                                x = x + 1
                            Else
                                strData = ""
                            End If
                        End If
                    Else
                        If dtMethod.Columns.Contains("CookMode") Then
                            If drMethod("CookMode").ToString.Trim() <> "" Then
                                ' RDC 12.10.2013
                                strData = "&#149" + "    " + drMethod("CookMode").ToString.Trim 'JTOC 12.12.2013 Reenabled CWM-9618
                                'strData = "    " + drMethod("CookMode").ToString.Trim
                            Else
                                strData = ""
                            End If
                        End If
                    End If
                End If
                '--
                If strData <> "" Then
                    strMethod.Append(strList.Replace("%s", strData))
                End If
            Next

            ''If strFormat = "N" Then
            strInstruction = strMethod.ToString()
            ''  Else
            ''  strInstruction = strTemplate.Replace("%l", strMethod.ToString())
            ''End If

        End If

        If strInstruction = "<div></div>" Then
            strInstruction = ""
        End If
        Return strInstruction
    End Function

    Private Function fGetMethodFormat(ByVal strFormat As String) As String
        Dim strReturn As String = ""
        Select Case strFormat.Trim().ToLower
            Case "x"
                strReturn = "<div>%s</div>"
            Case "n"
                strReturn = "<ol>%l</ol>"
            Case "b"
                strReturn = "<ul>%l</ul>"
            Case "nh"
                strReturn = "<span>Nutritional Information %h</span><br>"
            Case "s"
                strReturn = "<span>%s</span><br>"
            Case Else

        End Select
        Return strReturn
    End Function

    Private Function fctCheckDbNull(ByVal objText As Object) As String
        Dim strX As String

        If IsDBNull(objText) Then
            strX = ""
        Else
            'strX = Trim(objText.ToString.Replace("[R]", "®").Replace("[TM]", "™").Replace("[", "").Replace("]", "").ToString)
            strX = Trim(objText.ToString.Replace("°", "&#176").Replace("®", "&#174").Replace("™", "&#8482").Replace("©", "&#169").Replace("[", "").Replace("]", "").ToString)
        End If

        Return strX
    End Function

    Private Function fctCheckDbNullNumeric(ByVal objDecimal As Object) As String
        Dim dblX As Double
        Dim strX As String
        If IsDBNull(objDecimal) Then
            strX = ""
            'ElseIf objDecimal <= 0 Then
            '    strX = ""
        Else
            dblX = FormatNumber(objDecimal, 6)
            strX = Trim(dblX.ToString.Replace("[", "").Replace("]", "").ToString)
        End If
        Return strX
    End Function

    Private Function fctCheckFormat(ByVal objText As Object) As String
        Dim strX As String

        If IsDBNull(objText) Then
            strX = ""
        Else
            strX = Trim(objText.ToString)
        End If

        Return strX
    End Function


    Public Function GetPicturePath(ByVal intListe As Integer, ByVal strPicturename As String, ByRef strPictureFolder As String, Optional ByVal strDefaultPicture As String = "nopic.jpg") As String
        Dim strPicturePath As String = ""
        Dim sPicture1 As String = ParseString(strPicturename, 0, CChar(";"))
        Dim sPicture2 As String = ParseString(strPicturename, 1, CChar(";"))
        Dim sPicture3 As String = ParseString(strPicturename, 2, CChar(";"))
        Dim sPicture4 As String = ParseString(strPicturename, 3, CChar(";"))
        Dim sPicture5 As String = ParseString(strPicturename, 4, CChar(";"))

        If Not L_strHostName.Contains(L_strPort) Then 'TDQ 10242011
            If L_strPort <> 80 Then 'DLS
                L_strHostName &= ":" & L_strPort
            End If
        End If

        Select Case GetRecipeDefaultPicture(intListe)
            Case 1
            Case 2 : sPicture1 = sPicture2
            Case 3 : sPicture1 = sPicture3
            Case 4 : sPicture1 = sPicture4
            Case 5 : sPicture1 = sPicture5
            Case Else : sPicture1 = sPicture1
        End Select
        If sPicture1 <> "" Then
            L_sFolder = "picnormal"
            L_sURLFragment = L_sFolder & "/" & sPicture1
            strPicturePath = L_strHostName & "/" & String.Concat(L_sURLFragment)
            strPictureFolder = L_sFolder & "/" & sPicture1
        Else
            L_sFolder = "images"
            L_sURLFragment = L_sFolder & "/" & strDefaultPicture
            strPicturePath = L_strHostName & "/" & String.Concat(L_sURLFragment)
            strPictureFolder = L_sFolder & "/" & strDefaultPicture
        End If
        Return strPicturePath
    End Function
    Public Function GetPicturePath2Pic(ByVal intListe As Integer, ByVal strPicturename As String, ByRef strPictureFolder As String, Optional ByVal strDefaultPicture As String = "nopic.jpg") As ArrayList
        Dim strPicturePath As New ArrayList
        Dim intDefaultPic As Integer
        Dim sPicture As String
        Dim sPicture1 As String = ParseString(strPicturename, 0, CChar(";"))
        Dim sPicture2 As String = ParseString(strPicturename, 1, CChar(";"))
        Dim sPicture3 As String = ParseString(strPicturename, 2, CChar(";"))
        Dim sPicture4 As String = ParseString(strPicturename, 3, CChar(";"))
        Dim sPicture5 As String = ParseString(strPicturename, 4, CChar(";"))

        If Not L_strHostName.Contains(L_strPort) Then 'TDQ 10242011
            If L_strPort <> 80 Then 'DLS
                L_strHostName &= ":" & L_strPort
            End If
        End If

        intDefaultPic = GetRecipeDefaultPicture(intListe)

        Select Case intDefaultPic
            Case 1 : sPicture = sPicture1
            Case 2 : sPicture = sPicture2
            Case 3 : sPicture = sPicture3
            Case 4 : sPicture = sPicture4
            Case 5 : sPicture = sPicture5
            Case Else : sPicture = sPicture1
        End Select

        If sPicture <> "" Then
            L_sFolder = "picnormal"
            L_sURLFragment = L_sFolder & "/" & sPicture
            strPicturePath.Add(L_strHostName & "/" & String.Concat(L_sURLFragment))
            strPicturePath.Add(L_sFolder & "/" & sPicture)
        Else
            L_sFolder = "images"
            L_sURLFragment = L_sFolder & "/" & strDefaultPicture
            strPicturePath.Add(L_strHostName & "/" & String.Concat(L_sURLFragment))
            strPicturePath.Add(L_sFolder & "/" & strDefaultPicture)
        End If

        If intDefaultPic = 1 Then
            If sPicture2 <> "" Then
                L_sFolder = "picnormal"
                L_sURLFragment = L_sFolder & "/" & sPicture2
                strPicturePath.Add(L_strHostName & "/" & String.Concat(L_sURLFragment))
                strPicturePath.Add(L_sFolder & "/" & sPicture2)
            Else
                L_sFolder = "images"
                L_sURLFragment = L_sFolder & "/" & strDefaultPicture
                strPicturePath.Add(L_strHostName & "/" & String.Concat(L_sURLFragment))
                strPicturePath.Add(L_sFolder & "/" & strDefaultPicture)
            End If
        Else
            If sPicture1 <> "" Then
                L_sFolder = "picnormal"
                L_sURLFragment = L_sFolder & "/" & sPicture1
                strPicturePath.Add(L_strHostName & "/" & String.Concat(L_sURLFragment))
                strPicturePath.Add(L_sFolder & "/" & sPicture1)
            Else
                L_sFolder = "images"
                L_sURLFragment = L_sFolder & "/" & strDefaultPicture
                strPicturePath.Add(L_strHostName & "/" & String.Concat(L_sURLFragment))
                strPicturePath.Add(L_sFolder & "/" & strDefaultPicture)
            End If
        End If

        Return strPicturePath
    End Function

    Function fctConvertToFraction(ByVal stWholeNumber As String, blnUseFractions As Boolean) As String  'dblValue As Double) As String
        Dim dblValue As Double
        Dim lngEntier
        Dim dblDecimal
        Dim strValue
        Dim dblError
        Dim dblErrorMin
        Dim myV1(12)
        Dim myF1(12)
        Dim myV2(6)
        Dim myF2(6)
        Dim i
        Dim Imin

        On Error GoTo err_fctConvertToFraction

        If stWholeNumber = "" Then
            dblValue = -1
        Else
            dblValue = CDbl(stWholeNumber)
        End If


        If dblValue = -1 Then
            fctConvertToFraction = ""
            Exit Function
        End If

        myV1(0) = 0.05 : myV1(1) = 0.1 : myV1(2) = 0.125 : myV1(3) = 0.2
        myV1(4) = 0.25 : myV1(5) = 1 / 3 : myV1(6) = 0.4 : myV1(7) = 0.5
        myV1(8) = 0.6 : myV1(9) = 2 / 3 : myV1(10) = 0.75 : myV1(11) = 0.8
        myV1(12) = 1
        myF1(0) = "1/50" : myF1(1) = "1/10" : myF1(2) = "1/8" : myF1(3) = "1/5"
        myF1(4) = "1/4" : myF1(5) = "1/3" : myF1(6) = "2/5" : myF1(7) = "1/2"
        myF1(8) = "3/5" : myF1(9) = "2/3" : myF1(10) = "3/4" : myF1(11) = "4/5"
        myF1(12) = "1"
        '
        myV2(0) = 0.125 : myV2(1) = 0.25 : myV2(2) = 1 / 3 : myV2(3) = 0.5
        myV2(4) = 2 / 3 : myV2(5) = 0.75 : myV2(6) = 1
        myF2(0) = "" : myF2(1) = "1/4" : myF2(2) = "1/3" : myF2(3) = "1/2"
        myF2(4) = "2/3" : myF2(5) = "3/4" : myF2(6) = ""
        '
        lngEntier = Int(dblValue)
        dblDecimal = dblValue - lngEntier
        strValue = lngEntier

        If dblDecimal > 0.001 Then
            If lngEntier > 0 Then
                dblErrorMin = 1000
                Imin = 20
                For i = 0 To 6
                    dblError = System.Math.Abs(dblDecimal - myV2(i)) / dblDecimal
                    If dblError < dblErrorMin Then dblErrorMin = dblError : Imin = i
                Next
                If Imin = 6 Then lngEntier = lngEntier + 1
                strValue = lngEntier & " " & myF2(Imin)
            Else
                dblErrorMin = 1000
                Imin = 20
                For i = 0 To 12
                    dblError = System.Math.Abs(dblDecimal - myV1(i)) / dblDecimal
                    If dblError < dblErrorMin Then dblErrorMin = dblError : Imin = i
                Next
                strValue = myF1(Imin)
            End If
        End If

        fctConvertToFraction = Trim(strValue)
        Exit Function
err_fctConvertToFraction:
        fctConvertToFraction = "Error"
        'MsgBox(FTB(132577) & " " & FTB(135955), vbExclamation)   '"Error. Invalid numeric value."

    End Function

    Function fctConvertToFraction2(ByVal stWholeNumber As String) As String  'dblValue As Double) As String
        Dim dblValue As Double
        Dim lngEntier
        Dim dblDecimal
        Dim strValue
        Dim dblError
        Dim dblErrorMin
        Dim myV1(12)
        Dim myF1(12)
        Dim myV2(6)
        Dim myF2(6)
        Dim i
        Dim Imin

        On Error GoTo err_fctConvertToFraction

        If stWholeNumber = "" Then
            dblValue = -1
        Else
            dblValue = CDbl(stWholeNumber)
        End If


        If dblValue = -1 Then
            fctConvertToFraction2 = ""
            Exit Function
        End If

        myV1(0) = 0.05 : myV1(1) = 0.1 : myV1(2) = 0.125 : myV1(3) = 0.2
        myV1(4) = 0.25 : myV1(5) = 1 / 3 : myV1(6) = 0.4 : myV1(7) = 0.5
        myV1(8) = 0.6 : myV1(9) = 2 / 3 : myV1(10) = 0.75 : myV1(11) = 0.8
        myV1(12) = 1

        myF1(0) = "1/50" : myF1(1) = "1/10" : myF1(2) = "1/8" : myF1(3) = "1/5"
        myF1(4) = "1/4" : myF1(5) = "1/3" : myF1(6) = "2/5" : myF1(7) = "1/2"
        myF1(8) = "3/5" : myF1(9) = "2/3" : myF1(10) = "3/4" : myF1(11) = "4/5"
        myF1(12) = "1"
        '
        myV2(0) = 0.125 : myV2(1) = 0.25 : myV2(2) = 1 / 3 : myV2(3) = 0.5
        myV2(4) = 2 / 3 : myV2(5) = 0.75 : myV2(6) = 1
        myF2(0) = "" : myF2(1) = "1/4" : myF2(2) = "1/3" : myF2(3) = "1/2"
        myF2(4) = "2/3" : myF2(5) = "3/4" : myF2(6) = ""
        '
        lngEntier = Int(dblValue)
        dblDecimal = dblValue - lngEntier
        strValue = lngEntier

        If dblDecimal > 0.001 Then
            If lngEntier > 0 Then
                dblErrorMin = 1000
                Imin = 20
                For i = 0 To 6
                    dblError = System.Math.Abs(dblDecimal - myV2(i)) / dblDecimal
                    If dblError < dblErrorMin Then dblErrorMin = dblError : Imin = i
                Next
                If Imin = 6 Then lngEntier = lngEntier + 1
                strValue = lngEntier & " " & myF2(Imin)
            Else
                dblErrorMin = 1000
                Imin = 20
                For i = 0 To 12
                    dblError = System.Math.Abs(dblDecimal - myV1(i)) / dblDecimal
                    If dblError < dblErrorMin Then dblErrorMin = dblError : Imin = i
                Next
                strValue = myF1(Imin)
            End If
        End If

        fctConvertToFraction2 = Trim(strValue)
        Exit Function
err_fctConvertToFraction:
        fctConvertToFraction2 = "Error"
        'MsgBox(FTB(132577) & " " & FTB(135955), vbExclamation)   '"Error. Invalid numeric value."

    End Function

    Public Function ConvertDecimalToFraction2(ByVal pValue As Decimal) As Object
        Dim strValue As String

        Dim lngEntier As Long
        Dim dblDecimal As Decimal

        Dim dblError As Decimal
        Dim dblErrorMin As Decimal
        Dim myV1(12) As Object
        Dim myF1(12) As Object
        Dim myV2(6) As Object
        Dim myF2(6) As Object
        Dim i As Integer
        Dim Imin As Integer

        Dim dblValue As Decimal = pValue

        If dblValue = -1 Then
            Return ""
            Exit Function
        End If

        myV1(0) = 0.05 : myV1(1) = 0.1 : myV1(2) = 0.125 : myV1(3) = 0.2
        myV1(4) = 0.25 : myV1(5) = 1 / 3 : myV1(6) = 0.4 : myV1(7) = 0.5
        myV1(8) = 0.6 : myV1(9) = 2 / 3 : myV1(10) = 0.75 : myV1(11) = 0.8
        myV1(12) = 1
        myF1(0) = "1/50" : myF1(1) = "1/10" : myF1(2) = "1/8" : myF1(3) = "1/5"
        myF1(4) = "1/4" : myF1(5) = "1/3" : myF1(6) = "2/5" : myF1(7) = "1/2"
        myF1(8) = "3/5" : myF1(9) = "2/3" : myF1(10) = "3/4" : myF1(11) = "4/5"
        myF1(12) = "1"
        '
        myV2(0) = 0.125 : myV2(1) = 0.25 : myV2(2) = 1 / 3 : myV2(3) = 0.5
        myV2(4) = 2 / 3 : myV2(5) = 0.75 : myV2(6) = 1
        myF2(0) = "" : myF2(1) = "1/4" : myF2(2) = "1/3" : myF2(3) = "1/2"
        myF2(4) = "2/3" : myF2(5) = "3/4" : myF2(6) = ""
        '
        lngEntier = Int(dblValue)
        dblDecimal = dblValue - lngEntier
        strValue = lngEntier

        If dblDecimal > 0.001 Then
            If lngEntier > 0 Then
                myF1(12) = "" '// DRR 08.09.2011
                dblErrorMin = 1000
                Imin = 20

                'For i = 0 To 6
                '    dblError = Math.Abs(dblDecimal - myV2(i)) / dblDecimal
                '    If dblError < dblErrorMin Then dblErrorMin = dblError : Imin = i
                'Next
                'If Imin = 6 Then lngEntier = lngEntier + 1
                'strValue = lngEntier & " " & myF2(Imin)

                For i = 0 To 12
                    dblError = System.Math.Abs(dblDecimal - myV1(i)) / dblDecimal
                    If dblError < dblErrorMin Then dblErrorMin = dblError : Imin = i
                Next
                If Imin = 12 Then lngEntier = lngEntier + 1
                strValue = lngEntier & " " & myF1(Imin)
            Else
                myF1(12) = "1" '// DRR 08.09.2011
                dblErrorMin = 1000
                Imin = 20

                For i = 0 To 12
                    dblError = System.Math.Abs(dblDecimal - myV1(i)) / dblDecimal
                    If dblError < dblErrorMin Then dblErrorMin = dblError : Imin = i
                Next
                strValue = myF1(Imin)
            End If
        End If

        Return Trim(strValue)

    End Function

    ' RDC 11.15.2013 : Check if quantity is a number alternative for isNumeric in .net
    Public Function IsANumber(valueToCheck) As Boolean
        Dim aNumber As Boolean = False

        Try
            Double.TryParse(valueToCheck, 1)
            aNumber = True
        Catch ex As Exception

        End Try

        Return aNumber
    End Function

    ' RDC 08.06.2013 : New function for displaying computed and imposed nutrients
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="intCode">Integer : Item code</param>
    ''' <param name="intNutrientSet">Optional Integer : Selected nutrient set (default 0)</param>
    ''' <param name="intCodeTrans">Optional Integer : Selected translation code (default 0)</param>
    ''' <param name="intDisplayOption">Optional NutrientDisplayOption : Option where user can display Calculated or Imposed nutrient or both.</param>
    ''' <remarks>Created for Nutrient Set requirement. </remarks>
    Function fctDisplayNutrientComputationForExport(intCode As Integer, strYieldUnit As String, Optional intNutrientSet As Integer = 0, Optional intCodeTrans As Integer = 1, Optional intDisplayOption As Integer = 0, Optional intCodeSetPrice As Integer = 1, Optional strNutrientBasis As String = "", Optional intVersion As Integer = 1, Optional blnUseLangCodeDictionary As Boolean = False) As StringBuilder
        Dim strHTMLContent As New StringBuilder
        ' RDC 11.20.2013 : Removed telerik table dependencies due to it will be printed as html controls.

        Dim cn As New SqlConnection(L_strCnn)
        Dim dtNutInfo As New DataTable
        Dim dtNutValue As New DataTable
        Dim dtNutDetail As New DataTable
        Dim dtNutrients As New DataTable
        Dim dsNutrients As New DataSet
        ' RDC 12.13.2013 : Language translation
        Dim dtLangTrans As New DataTable

        ' Nutrient Information
        Dim strImpYieldUnit As String = ""
        Dim strImpYieldUnit2 As String = ""
        Dim strPortionSize As String = ""
        Dim intImpType As Integer = 0
        Dim dblServingQty As Double = 0.0
        Dim dblServingQty2 As Double = 0.0
        Dim dblActualRecipeWt As Double = 0.0
        Dim strYieldQty As String = ""
        ' RDC 09.13.2013 : Unit Formatting
        Dim strYieldUnit1Format As String = "", strYieldUnit2Format As String = ""
        ' RDC 09.17.2013 : Unit
        Dim intYieldUnit1 As Integer = 0, intYieldUnit2 As Integer = 0

        Try
            cn.Open()
            If blnUseLangCodeDictionary Then
                Dim cm As New SqlCommand("Select CodeDictionary From EgswTranslation Where Code =" & intCodeTrans, cn)
                intCodeTrans = cm.ExecuteScalar
                cm.Dispose()
            End If


            Dim daNutrient As New SqlDataAdapter("Exec sp_EgswDisplayCalculatedImposedNutrient @intItemCode=" & intCode & ", @intCodeTrans=" & intCodeTrans & ", @intNutrientSet=" & intNutrientSet & ",  @intCodeSetPrice=" & intCodeSetPrice & ", @intVersion=" & intVersion, cn)
            'daNutrient.FillSchema(dsNutrients, SchemaType.Source) RDC 09.06.2013 : Removed due to constraint error
            daNutrient.Fill(dsNutrients)

            dtNutInfo = dsNutrients.Tables(0)
            dtNutValue = dsNutrients.Tables(1)
            dtNutDetail = dsNutrients.Tables(2)
            dtNutrients = dsNutrients.Tables(3)
            cn.Close()


            For Each drInfo As DataRow In dtNutInfo.Rows
                intImpType = CIntDB(drInfo("ImposedType"))
                strImpYieldUnit = CStrDB(drInfo("YieldUnitDesc"))
                strImpYieldUnit2 = CStrDB(drInfo("YieldUnit2Desc"))
                strPortionSize = CStrDB(drInfo("PortionSize"))
                dblServingQty = CDblDB(drInfo("Yield"))
                dblServingQty2 = CDblDB(drInfo("Yield2"))
                dblActualRecipeWt = CDblDB(drInfo("ActualWtInGms"))
                ' RDC 09.13.2013
                strYieldUnit1Format = CStrDB(drInfo("YieldUnit1Format"))
                strYieldUnit2Format = CStrDB(drInfo("YieldUnit2Format"))
                ' RDC 09.17.2013
                intYieldUnit1 = CIntDB(drInfo("YieldUnit"))
                intYieldUnit2 = CIntDB(drInfo("YieldUnit2"))
            Next

            ' RDC 09.12.2013 : Nutrient display fix
            Dim blnDisplayNutrient As Boolean
            For Each drNutValue As DataRow In dtNutValue.Rows
                blnDisplayNutrient = CBoolDB(drNutValue("DisplayNutrition"))
            Next

            If Not blnDisplayNutrient Then Exit Function

            ' RDC 09.13.2013 : Added unit formatting for Yield1 and Yield2 quantity
            ' RDC 12.12.2013 : Automatically display Yield1 unit if calculated nutrition is selected
            Dim strImposedUnit As String = ""
            Select Case G_ExportOptions.intExpSelectedNutrientComputation
                Case 0
                    strImposedUnit = strImpYieldUnit.Replace("_", " ")
                Case 1, 2
                    Select Case intImpType
                        Case 1
                            strImposedUnit = strImpYieldUnit.Replace("_", " ")
                            If dblServingQty = 0 Then strYieldQty = "" Else strYieldQty = Format(dblServingQty, strYieldUnit1Format).ToString
                        Case 2
                            strImposedUnit = strImpYieldUnit2.Replace("_", " ")
                            If dblServingQty2 = 0 Then strYieldQty = "" Else strYieldQty = Format(dblServingQty2, strYieldUnit2Format).ToString
                        Case 4
                            strImposedUnit = strPortionSize
                        Case Else
                            strImposedUnit = strImpYieldUnit.Replace("_", " ")
                            If dblServingQty = 0 Then strYieldQty = "" Else strYieldQty = Format(dblServingQty, strYieldUnit1Format).ToString
                    End Select
            End Select



            ' Plot Everything here
            Dim dblComputedValue() As Double = {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, _
                     0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}
            Dim dblImposedValue() As Double = {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, _
                     0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}
            Dim dblImposedValuePct() As Double = {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, _
                      0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}
            Dim blnNutrientVisible() As Boolean = {False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, _
                      False, False, False, False, False, False, False, False, False, False, False, False, False, False}

            Dim dblComputedNutValPerServing As Double = 0.0
            Dim dblComputedNutValPer100gml As Double = 0.0

            For Each drNutVal As DataRow In dtNutValue.Rows
                For intNutPos As Integer = 0 To 33 Step 1
                    If Not IsDBNull(drNutVal("N" & CStr(intNutPos + 1))) Then dblComputedValue(intNutPos) = drNutVal("N" & CStr(intNutPos + 1)) Else dblComputedValue(intNutPos) = 0.0
                    If Not IsDBNull(drNutVal("N" & CStr(intNutPos + 1) & "impose")) Then dblImposedValue(intNutPos) = drNutVal("N" & CStr(intNutPos + 1) & "impose") Else dblImposedValue(intNutPos) = 0.0
                    If Not IsDBNull(drNutVal("N" & CStr(intNutPos + 1) & "ImposePercent")) Then dblImposedValuePct(intNutPos) = drNutVal("N" & CStr(intNutPos + 1) & "ImposePercent") Else dblImposedValuePct(intNutPos) = 0.0
                    blnNutrientVisible(intNutPos) = CBoolDB(drNutVal("N" & CStr(intNutPos + 1) & "Display"))
                Next
            Next

            Dim strCalculatedNutrients As String = "", strImposedNutrients As String = "", strImpCalcNutrients As String = ""
            For Each drDetail As DataRow In dtNutrients.Rows

                Dim intSelPos As Integer = CInt(drDetail("NutSequence"))
                Dim strValFormat As String = CStr(drDetail("Format"))

                Dim dblComputedPerServingAt100 As Double = CDblDB(dblComputedValue(intSelPos - 1))
                Dim dblComputedPer100gmlAt100 As Double = 0.0

                Dim dblPerServing As Double = 0.0
                Dim dblPer100gml As Double = 0.0

                Select Case intDisplayOption
                    Case 0  ' Calculated
                        If CBoolDB(blnNutrientVisible(intSelPos - 1)) Then
                            Dim dblImposedNutPct As Double = dblImposedValuePct(intSelPos - 1)
                            Dim dblImposedNutVal As Double = dblImposedValue(intSelPos - 1)
                            Dim strValueToPrint As String = ""


                            If Not dblImposedNutVal < 0 And dblImposedNutPct < 0 Then
                                strValueToPrint = Format(dblImposedNutVal, strValFormat) & " " & drDetail("Units")
                            ElseIf dblImposedNutVal < 0 And Not dblImposedNutPct < 0 Then
                                strValueToPrint = dblImposedNutPct.ToString & "%"
                            ElseIf Not dblImposedNutVal < 0 And Not dblImposedNutPct < 0 Then
                                strValueToPrint = dblImposedNutPct.ToString & "%"
                            Else
                                strValueToPrint = ""
                            End If

                            If intSelPos = 1 Then
                                Dim dblKjPerServing As Double = 0, dblKjPer100gml As Double = 0
                                Dim dblKcalPerServing As Double = 0, dblKcalPer100gml As Double = 0

                                ' RDC 09.09.2013 : CWM-8371 Fix
                                If dblComputedValue(intSelPos - 1) > 0 Then
                                    ' Energy Per serving kJ
                                    dblKjPerServing = dblComputedValue(intSelPos - 1) / dblServingQty
                                    dblKcalPerServing = dblKjPerServing / 4.184

                                    ' Energy Per 100 g/ml kcal
                                    ' RDC 09.17.2013 : CWM-8415 Fix
                                    If intYieldUnit1 = 2 Or intYieldUnit1 = 8 Then
                                        dblKjPer100gml = dblKjPerServing / 10
                                        dblKcalPer100gml = dblKcalPerServing / 10
                                    Else
                                        dblKjPer100gml = (100 / dblActualRecipeWt) * (dblComputedValue(intSelPos - 1) / dblServingQty)
                                        dblKcalPer100gml = dblKjPer100gml / 4.184
                                    End If
                                End If

                                'AGL 2015.01.22
                                If G_ExportOptions.intEnergyDisplay = 1 Then 'KJ First
                                    ' RDC 12.05.2013 : Do not display nutrient if value is zero (0)
                                    'If dblKjPerServing > 0 Then
                                    strCalculatedNutrients &= drDetail("NutrientName").ToString & " (kJ): " & Format(dblKjPerServing, strValFormat).ToString & " kJ, "
                                    'End If

                                    If dblKcalPerServing > 0 Then
                                        strCalculatedNutrients &= drDetail("NutrientName").ToString & " (kcal): " & Format(dblKcalPerServing, strValFormat).ToString & " kcal, "
                                    End If
                                Else 'Kcal first
                                    ' RDC 12.05.2013 : Do not display nutrient if value is zero (0)
                                    'If dblKcalPerServing > 0 Then
                                    strCalculatedNutrients &= drDetail("NutrientName").ToString & " (kcal): " & Format(dblKcalPerServing, strValFormat).ToString & " kcal, "
                                    'End If

                                    'If dblKjPerServing > 0 Then
                                    strCalculatedNutrients &= drDetail("NutrientName").ToString & " (kJ): " & Format(dblKjPerServing, strValFormat).ToString & " kJ, "
                                    'End If
                                End If


                            Else
                                Dim dblNutPerServing As Double = 0, dblNutPer100gml As Double = 0
                                ' RDC 09.09.2013 : CWM-8371 Fix
                                'If dblComputedValue(intSelPos - 1) > 0 Then
                                dblNutPerServing = dblComputedValue(intSelPos - 1) / dblServingQty
                                If intYieldUnit1 = 2 Or intYieldUnit1 = 8 Then
                                    dblNutPer100gml = dblNutPerServing / 10
                                Else
                                    dblNutPer100gml = (100 / dblActualRecipeWt) * (dblComputedValue(intSelPos - 1) / dblServingQty)
                                End If
                                'End If

                                ' RDC 12.05.2013 : Do not display nutrient if value is zero (0)
                                'If dblNutPerServing > 0 Then
                                strCalculatedNutrients &= drDetail("Nutrientname").ToString & " : " & Format(dblNutPerServing, strValFormat).ToString & " " & drDetail("Units").ToString.Trim & ", "
                                'End If
                            End If

                        End If
                    Case 1  ' Imposed
                        If CBoolDB(blnNutrientVisible(intSelPos - 1)) Then

                            Dim dblImposedNutPct As Double = dblImposedValuePct(intSelPos - 1)
                            Dim dblImposedNutVal As Double = dblImposedValue(intSelPos - 1)
                            Dim strValueToPrint As String = ""
                            Dim strValueToPrintKcal As String = ""
                            strValueToPrintKcal = Format(ConvertKjtoKcal(dblImposedNutVal), strValFormat) & " kcal"

                            If Not dblImposedNutVal < 0 And dblImposedNutPct < 0 Then
                                strValueToPrint = Format(dblImposedNutVal, strValFormat) & " " & drDetail("Units")
                            ElseIf dblImposedNutVal < 0 And Not dblImposedNutPct < 0 Then
                                strValueToPrint = dblImposedNutPct.ToString & "%"
                            ElseIf Not dblImposedNutVal < 0 And Not dblImposedNutPct < 0 Then
                                strValueToPrint = dblImposedNutPct.ToString & "%"
                            Else
                                strValueToPrint = ""
                            End If

                            ' RDC 12.02.2013 : Do not display nutrient information if value is null/-1
                            
                            If strValueToPrint.Trim.Length > 0 Then
                                'AGL 2015.01.22
                                If G_ExportOptions.intEnergyDisplay = 1 Then
                                    If intSelPos = 1 Then
                                        strImposedNutrients &= drDetail("NutrientName") & " : " & strValueToPrint & ", " & drDetail("NutrientName").ToString & " (kcal): " & strValueToPrintKcal
                                    Else
                                        strImposedNutrients &= drDetail("NutrientName") & " : " & strValueToPrint
                                    End If
                                Else
                                    If intSelPos = 1 Then
                                        strImposedNutrients &= drDetail("NutrientName") & " : " & strValueToPrintKcal & ", " & drDetail("NutrientName").ToString & " (kJ): " & strValueToPrint
                                    Else
                                        strImposedNutrients &= drDetail("NutrientName").ToString & " (kJ): " & strValueToPrint
                                    End If

                                End If
                            End If
                        End If

                    Case 2  ' Both

                        If blnNutrientVisible(intSelPos - 1) Then

                            Dim dblImposedNutPct As Double = dblImposedValuePct(intSelPos - 1)
                            Dim dblImposedNutVal As Double = dblImposedValue(intSelPos - 1)
                            Dim strValueToPrint As String = ""
                            Dim strValueToPrintKCal As String = "" 'AGL 2015.01.22

                            strValueToPrintKCal = Format(ConvertKjtoKcal(dblImposedNutVal), strValFormat) + " kcal"

                            If Not dblImposedNutVal < 0 And dblImposedNutPct < 0 Then
                                strValueToPrint = Format(dblImposedNutVal, strValFormat) & " " & drDetail("Units")
                            ElseIf dblImposedNutVal < 0 And Not dblImposedNutPct < 0 Then
                                strValueToPrint = dblImposedNutPct.ToString & "%"
                            ElseIf Not dblImposedNutVal < 0 And Not dblImposedNutPct < 0 Then
                                strValueToPrint = dblImposedNutPct.ToString & "%"
                            Else
                                strValueToPrint = ""
                            End If

                            strImposedNutrients &= strValueToPrint & ", "

                            If intSelPos = 1 Then
                                Dim dblKjPerServing As Double = 0, dblKjPer100gml As Double = 0
                                Dim dblKcalPerServing As Double = 0, dblKcalPer100gml As Double = 0

                                ' RDC 09.09.2013 : CWM-8371 Fix
                                If dblComputedValue(intSelPos - 1) > 0 Then
                                    ' Energy Per serving kJ
                                    dblKjPerServing = dblComputedValue(intSelPos - 1) / dblServingQty
                                    dblKcalPerServing = dblKjPerServing / 4.184

                                    ' Energy Per 100 g/ml kcal
                                    If intYieldUnit1 = 2 Or intYieldUnit1 = 8 Then
                                        dblKjPer100gml = dblKjPerServing / 10
                                        dblKcalPer100gml = dblKcalPerServing / 10
                                    Else
                                        dblKjPer100gml = (100 / dblActualRecipeWt) * (dblComputedValue(intSelPos - 1) / dblServingQty)
                                        dblKcalPer100gml = dblKjPer100gml / 4.184
                                    End If

                                End If

                                'AGL 2015.01.22
                                If G_ExportOptions.intEnergyDisplay = 1 Then
                                    ' RDC 12.02.2013 : Do not display nutrient information if value is null/-1' RDC 12.02.2013 : Do not display nutrient information if value is null/-1
                                    'If dblKjPerServing > 0 Then
                                    strImpCalcNutrients &= "<tr><td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: left;' width='220'>" & drDetail("NutrientName").ToString & "</td>" & _
                                             "<td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: right;' width='200'>" & Format(dblKjPerServing, strValFormat).ToString & " kJ </td>" & _
                                             "<td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: right;' width='200'>" & IIf(strValueToPrint.ToString <> "", strValueToPrint.ToString, "") & "</td></tr>"
                                    'Else
                                    '    strImpCalcNutrients &= "<tr><td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: left;' width='220'>" & drDetail("NutrientName").ToString & "</td>" & _
                                    '             "<td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: right;' width='200'> &nbsp; </td>" & _
                                    '             "<td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: right;' width='200'>" & strValueToPrint.ToString & "</td></tr>"
                                    'End If

                                    'If dblKcalPerServing > 0 Then
                                    strImpCalcNutrients &= "<tr><td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: left;' width='220'> &nbsp; </td>" & _
                                           "<td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: right;' width='200'>" & Format(dblKcalPerServing, strValFormat).ToString & " kcal </td>" & _
                                           "<td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: right;' width='200'>" & IIf(strValueToPrint.ToString <> "", strValueToPrintKCal.ToString, "") & "</td></tr>"
                                    'End If
                                Else
                                    'If dblKcalPerServing > 0 Then
                                    strImpCalcNutrients &= "<tr><td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: left;' width='220'>" & drDetail("NutrientName").ToString & "</td>" & _
                                           "<td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: right;' width='200'>" & Format(dblKcalPerServing, strValFormat).ToString & " kcal </td>" & _
                                           "<td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: right;' width='200'>" & IIf(strValueToPrint.ToString <> "", strValueToPrintKCal.ToString, "") & "</td></tr>" 'AGL 2015.01.22
                                    'End If
                                    'If dblKjPerServing > 0 Then
                                    strImpCalcNutrients &= "<tr><td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: left;' width='220'> &nbsp; </td>" & _
                                             "<td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: right;' width='200'>" & Format(dblKjPerServing, strValFormat).ToString & " kJ </td>" & _
                                             "<td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: right;' width='200'>" & IIf(strValueToPrint.ToString <> "", strValueToPrint.ToString, "") & "</td></tr>"
                                    'Else
                                    '    strImpCalcNutrients &= "<tr><td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: left;' width='220'>" & drDetail("NutrientName").ToString & "</td>" & _
                                    '             "<td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: right;' width='200'> &nbsp; </td>" & _
                                    '             "<td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: right;' width='200'>" & IIf(strValueToPrint.ToString <> "", strValueToPrint.ToString, "") & "</td></tr>"
                                    'End If
                                End If



                        Else
                            Dim dblNutPerServing As Double = 0, dblNutPer100gml As Double = 0
                            ' RDC 09.09.2013 : CWM-8371 Fix
                            If dblComputedValue(intSelPos - 1) > 0 Then
                                dblNutPerServing = dblComputedValue(intSelPos - 1) / dblServingQty

                                ' RDC 09.17.2013 : CWM-8415 Fix
                                If intYieldUnit1 = 2 Or intYieldUnit1 = 8 Then
                                    dblNutPer100gml = dblNutPerServing / 10
                                Else
                                    dblNutPer100gml = (100 / dblActualRecipeWt) * (dblComputedValue(intSelPos - 1) / dblServingQty)
                                End If
                            End If

                            ' RDC 12.02.2013 : Do not display nutrient information if value is null/-1' RDC 12.02.2013 : Do not display nutrient information if value is null/-1
                                'If dblNutPerServing > 0 Then
                                strImpCalcNutrients &= "<tr><td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: left;' width='220'>" & drDetail("NutrientName").ToString & "</td>" & _
                                       "<td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: right;' width='200'>" & Format(dblNutPerServing, strValFormat).ToString & " " & drDetail("Units").ToString.Trim & "</td>" & _
                                       "<td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: right;' width='200'>" & IIf(strValueToPrint.ToString <> "", strValueToPrint.ToString, "") & "</td></tr>"
                                'Else
                                '    strImpCalcNutrients &= "<tr><td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: left;' width='220'>" & drDetail("NutrientName").ToString & "</td>" & _
                                '           "<td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: right;' width='200'> &nbsp;</td>" & _
                                '           "<td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: right;' width='200'>" & strValueToPrint.ToString & "</td></tr>"
                                'End If
                            End If

                        End If
                    Case Else

                End Select

            Next

            If strCalculatedNutrients.Trim.Length > 1 Then strCalculatedNutrients = Mid(strCalculatedNutrients, 1, Len(strCalculatedNutrients)) 'AGL 2015.01.22
            If strImposedNutrients.Trim.Length > 1 Then strImposedNutrients = Mid(strImposedNutrients, 1, Len(strImposedNutrients)) 'AGL 2015.01.22

            Dim cLang As New clsEGSLanguage(intCodeTrans)

            If strNutrientBasis.Trim.Length > 1 Then strNutrientBasis = "(" & strNutrientBasis & ")"


            strHTMLContent.Append("<tr>")
            strHTMLContent.Append("<td>")
            strHTMLContent.Append("<table style='width: 620'>")
            'strHTMLContent.Append("<tr>")
            'strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
            'strHTMLContent.Append(strNutrientBasis.ToString)
            'strHTMLContent.Append("</td></tr>")

            Select Case intDisplayOption
                Case 0 ' Calculated
                    strHTMLContent.Append("<tr><td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
                    strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.NutrientInformation).ToString & " : " & cLang.GetString(clsEGSLanguage.CodeType.CalculatedNutrients).ToString.Replace("Nutrients", " ") & " " & cLang.GetString(144686).Replace("%Y", strImposedUnit) & " " & strNutrientBasis)
                    strHTMLContent.Append("</td></tr>")
                    strHTMLContent.Append("<tr><td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: justify;'>")
                    strHTMLContent.Append(strCalculatedNutrients.ToString)
                    strHTMLContent.Append("</td></tr>")
                Case 1 ' Imposed
                    ' RDC 12.02.2013 : Added " " & strImposedUnit & to imposed header
                    ' RDC 12.04.2013 : Added strYieldQty
                    strHTMLContent.Append("<tr><td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
                    strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.NutrientInformation).ToString & " : " & cLang.GetString(clsEGSLanguage.CodeType.ImposedNutrients).ToString.Replace("Nutrients", " ") & " " & strYieldQty & " " & strImposedUnit & " " & strNutrientBasis)
                    strHTMLContent.Append("</td></tr>")
                    strHTMLContent.Append("<tr><td style='font-weight: regular; font-size: 11.5pt; font-family: Calibri; text-align: justify;'>")
                    strHTMLContent.Append(strImposedNutrients.ToString)
                    strHTMLContent.Append("</td></tr>")
                Case 2 ' Both

                    strHTMLContent.Append("<tr><td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: left;' colspan='2'>")
                    strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.NutrientInformation).ToString & " " & strNutrientBasis)
                    strHTMLContent.Append("</td></tr>")
                    ' RDC 12.02.2013 : Section header for nutrient
                    strHTMLContent.Append("<tr><td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: left;' width='220'>&nbsp;</td>" & _
                           "<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: right;' width='200'>" & cLang.GetString(clsEGSLanguage.CodeType.CalculatedNutrients) & " " & cLang.GetString(clsEGSLanguage.CodeType.PerYPercentAt100).Replace("%Y", strImpYieldUnit) & "</td>" & _
                           "<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: right;' width='200'>" & cLang.GetString(clsEGSLanguage.CodeType.ImposedNutrient) & " (" & strYieldQty & " " & strImposedUnit.Replace("(", "").Replace(")", "") & ")</td></tr>")
                    strHTMLContent.Append(strImpCalcNutrients.ToString)
                Case Else

            End Select
            strHTMLContent.Append("</table>")
            strHTMLContent.Append("</td></tr>")

        Catch ex As Exception

        End Try

        Return strHTMLContent

    End Function

    ' RDC 02.11.2014 : GDA Computation
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="intCode">Integer : Item code</param>
    ''' <param name="intNutrientSet">Optional Integer : Selected nutrient set (default 0)</param>
    ''' <param name="intCodeTrans">Optional Integer : Selected translation code (default 0)</param>
    ''' <param name="intDisplayOption">Optional NutrientDisplayOption : Option where user can display Calculated or Imposed nutrient or both.</param>
    ''' <remarks>Created for Nutrient Set requirement. </remarks>
    Function fctDisplayGDAComputationForExport(intCode As Integer, strYieldUnit As String, Optional intNutrientSet As Integer = 0, Optional intCodeTrans As Integer = 1, Optional intDisplayOption As Integer = 0, Optional intCodeSetPrice As Integer = 1, Optional strNutrientBasis As String = "", Optional intVersion As Integer = 1, Optional blnUseLangCodeDictionary As Boolean = False) As StringBuilder
        Dim strHTMLContent As New StringBuilder
        Try

            Dim cn As New SqlConnection(L_strCnn)
            cn.Open()
            Dim ds As New DataSet
            Dim da As New SqlDataAdapter("Exec sp_EgswDisplayCalculatedImposedNutrient @intItemCode=" & intCode & ", @intCodeTrans=" & intCodeTrans & ", @intNutrientSet=" & intNutrientSet & ",  @intCodeSetPrice=" & intCodeSetPrice, cn)
            da.Fill(ds)

            Dim dtItemInfo As DataTable = ds.Tables(0)
            Dim dtNutrientVal As DataTable = ds.Tables(1)
            Dim dtNutrientDetail As DataTable = ds.Tables(3)

            Dim dblActualRecipeWt As Double = 0
            Dim dblServingQty As Double = 0
            For Each drItemInfo As DataRow In dtItemInfo.Rows
                dblActualRecipeWt = CDblDB(drItemInfo("ActualWtInGms"))
                dblServingQty = CDblDB(drItemInfo("Yield"))
                strYieldUnit = CStrDB(drItemInfo("YieldUnitDesc"))
            Next


            ' Plot Everything here
            Dim dblComputedValue() As Double = {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, _
                                                0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}
            Dim dblImposedValue() As Double = {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, _
                                               0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}
            Dim dblImposedValuePct() As Double = {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, _
                                                  0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}
            Dim blnNutrientVisible() As Boolean = {False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, _
                                                   False, False, False, False, False, False, False, False, False, False, False, False, False, False}

            Dim intCalculatedCount As Integer = 0, intImposedCount As Integer = 0

            For Each drNutVal As DataRow In dtNutrientVal.Rows
                For intNutPos As Integer = 0 To 33 Step 1
                    If Not IsDBNull(drNutVal("N" & CStr(intNutPos + 1))) Then dblComputedValue(intNutPos) = drNutVal("N" & CStr(intNutPos + 1)) Else dblComputedValue(intNutPos) = 0.0
                    If Not IsDBNull(drNutVal("N" & CStr(intNutPos + 1) & "impose")) Then dblImposedValue(intNutPos) = drNutVal("N" & CStr(intNutPos + 1) & "impose") Else dblImposedValue(intNutPos) = 0.0
                    If Not IsDBNull(drNutVal("N" & CStr(intNutPos + 1) & "ImposePercent")) Then dblImposedValuePct(intNutPos) = drNutVal("N" & CStr(intNutPos + 1) & "ImposePercent") Else dblImposedValuePct(intNutPos) = 0.0
                    blnNutrientVisible(intNutPos) = CBoolDB(drNutVal("N" & CStr(intNutPos + 1) & "Display"))

                    ' Added condition for determining if all imposed or with defined calculated nutrient
                    If CDblDB(drNutVal("N" & CStr(intNutPos + 1))) > 0 Then intCalculatedCount += 1
                    If CDblDB(drNutVal("N" & CStr(intNutPos + 1) & "impose")) > 0 Or CDblDB(drNutVal("N" & CStr(intNutPos + 1) & "imposepercent")) > 0 Then intImposedCount += 1
                Next
            Next

            ' Do not generate GDA if values are imposed.
            If intCalculatedCount = 0 Then Exit Function
            Dim cLang As New clsEGSLanguage(intCodeTrans)

            strHTMLContent.Append("<tr><td> &nbsp;</td></tr>")
            strHTMLContent.Append("<tr>")
            strHTMLContent.Append("<td>")
            strHTMLContent.Append("<table style='width: 620'>")

            'strHTMLContent.Append("<tr><td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
            'strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.GDA))
            'strHTMLContent.Append("</td></tr>")

            Dim strExpGDAper100pct As String = ""
            Dim strExpGDAper100gml As String = ""
            Dim strExpGDA As String = ""

            For Each drDetail As DataRow In dtNutrientDetail.Rows
                Dim intSelPos As Integer = CInt(drDetail("NutSequence"))
                Dim dblGDAPer100Percent As Double = 0, dblGDAPer100gml As Double = 0

                ' RDC 09.09.2013 : CWM-8371 Fix
                If CDblDB(drDetail("GDA")) > 0 Then
                    Dim dblGDAValue As Double = CDblDB(drDetail("GDA"))
                    dblGDAPer100Percent = ((dblComputedValue(intSelPos - 1) / dblServingQty) / (dblGDAValue)) * 100
                    dblGDAPer100gml = ((100 / dblActualRecipeWt) * (dblComputedValue(intSelPos - 1) / dblServingQty) / (dblGDAValue)) * 100

                    Dim strGDAPer100Percent As String = "", strGDAPer100gml As String = ""

                    If Not dblGDAPer100Percent = 0 And dblGDAPer100Percent < 1 Then
                        strGDAPer100Percent = "< 1%"
                    ElseIf dblGDAPer100Percent = 0 Then
                        strGDAPer100Percent = "0%"
                    Else
                        strGDAPer100Percent = FormatNumber(dblGDAPer100Percent, 0).ToString & "%"
                    End If

                    strExpGDAper100pct &= drDetail("NutrientName") & " : " & strGDAPer100Percent & ", "

                    If Not dblGDAPer100gml = 0 And dblGDAPer100gml < 1 Then
                        strGDAPer100gml = "< 1%"
                    ElseIf dblGDAPer100gml = 0 Then
                        strGDAPer100gml = "0%"
                    Else
                        strGDAPer100gml = FormatNumber(dblGDAPer100gml, 0).ToString & "%"
                    End If

                    strExpGDAper100gml &= drDetail("NutrientName") & " : " & strGDAPer100gml & ", "

                    If intDisplayOption = 0 Then
                        strExpGDA &= "<tr><td style='font-size: 11.5pt; font-family: Calibri; text-align: left;'> " & drDetail("NutrientName") & " </td>"
                        strExpGDA &= "<td style='font-size: 11.5pt; font-family: Calibri; text-align: right;'> " & strGDAPer100Percent & " </td>"
                        strExpGDA &= "<td style='font-size: 11.5pt; font-family: Calibri; text-align: right;'> " & strGDAPer100gml & " </td></tr>"
                    End If

                End If
            Next

            ' RDC 02.11.2014 : Remove this code when there is already an option to display GDA (both, per serving unit and per 100g/ml)
            intDisplayOption = 1

            ' Report Section
            Select Case intDisplayOption
                Case 0 ' Both
                    strHTMLContent.Append("<tr><td colspan'3' style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
                    strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.GDA))
                    strHTMLContent.Append("</td></tr>")
                    strHTMLContent.Append("<tr><td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: left;'> " & cLang.GetString(clsEGSLanguage.CodeType.Nutrient) & " </td>")
                    strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: right;'> " & cLang.GetString(clsEGSLanguage.CodeType.PerYPercentAt100).Replace("%Y", strYieldUnit) & " </td>")
                    strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: right;'> " & cLang.GetString(clsEGSLanguage.CodeType.Per100gOR100mlat100Percent) & " </td></tr>")
                    strHTMLContent.Append(strExpGDA)
                    strHTMLContent.Append("</table>")
                Case 1 ' Per Yield Unit at 100%
                    strHTMLContent.Append("<tr><td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
                    strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.GDA) & " " & cLang.GetString(clsEGSLanguage.CodeType.PerYPercentAt100).Replace("%Y", strYieldUnit))
                    strHTMLContent.Append("</td></tr>")
                    strHTMLContent.Append("<tr><td style='font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
                    strHTMLContent.Append(Mid(strExpGDAper100pct, 1, Len(strExpGDAper100pct) - 2))
                    strHTMLContent.Append("</td></tr></table>")
                Case 2 ' Per 100g/100ml at 100%
                    strHTMLContent.Append("<tr><td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
                    strHTMLContent.Append(cLang.GetString(clsEGSLanguage.CodeType.GDA) & " " & cLang.GetString(clsEGSLanguage.CodeType.Per100gOR100mlat100Percent))
                    strHTMLContent.Append("</td></tr>")
                    strHTMLContent.Append("<tr><td style='font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
                    strHTMLContent.Append(Mid(strExpGDAper100pct, 1, Len(strExpGDAper100pct) - 2))
                    strHTMLContent.Append("</td></tr></table>")
                Case Else

            End Select

            cn.Close()
            cn.Dispose()

        Catch ex As Exception

        End Try
        Return strHTMLContent

    End Function


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="QuantityToConvert"></param>
    ''' <param name="UnitName"></param>
    ''' <param name="CodeSite"></param>
    ''' <param name="CodeTrans"></param>
    ''' <param name="DisplayType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetUnitType(ByVal QuantityToConvert As Double, UnitName As String, ByVal CodeSite As Integer, CodeTrans As Integer, DisplayType As Integer) As DataTable
        Dim dt As New DataTable
        Try
            Dim cn As New SqlConnection(L_strCnn)
            cn.Open()
            Dim strQry As String = "Exec sp_EgswGetUnitType @Quantity = " & QuantityToConvert & ",@vcUnitName = '" & UnitName & "', " & _
                                                           "@intCodeSite = " & CodeSite & ", @intCodeTrans = " & CodeTrans & ", " & _
                                                           "@intDisplayType = " & DisplayType

            Dim da As New SqlDataAdapter(strQry, cn)
            dt = New DataTable
            da.Fill(dt)

            da.Dispose()
            cn.Close()
            cn.Dispose()

        Catch ex As Exception

        End Try

        Return dt
    End Function

    ' RDC 02.10.2014 New Function 
    ''' <summary>
    ''' Returns corresponding font style to be used on selected language.
    ''' </summary>
    ''' <param name="LanguageCode">Integer : Language code</param>
    ''' <returns>String : Selected font name.</returns>
    ''' <remarks></remarks>
    Private Function GetLanguage(ByVal LanguageCode As Integer) As String
        Dim strSelectedFont As String = "Calibri"
        Try
            Dim arrLangInfo(1) As String
            Dim strQry As String = <sql>
                                       Select b.Code As LangCode, b.[Language] As CodeLang
                                       From dbo.EgswTranslation As a
                                           Left Join dbo.EgswLanguage As b On a.CodeDictionary = b.Code
                                       Where a.Code = @LanguageCode
                                   </sql>.Value
            Dim cn As New SqlConnection(L_strCnn)
            cn.Open()
            Dim cm As New SqlCommand
            Dim dr As SqlDataReader
            cm.Connection = cn
            cm.CommandType = CommandType.Text
            cm.CommandText = strQry
            cm.CreateParameter()
            cm.Parameters.Add("@LanguageCode", SqlDbType.Int).Value = LanguageCode
            dr = cm.ExecuteReader()

            If dr.HasRows Then
                While dr.Read
                    arrLangInfo(0) = dr("LangCode").ToString
                    arrLangInfo(1) = dr("CodeLang").ToString
                End While
            Else
                arrLangInfo(0) = 1
                arrLangInfo(1) = "English"
            End If

            Select Case CInt(arrLangInfo(0))
                Case 13, 15
                    strSelectedFont = "SimSun"
                Case Else
                    strSelectedFont = "Calibri"
            End Select

        Catch ex As Exception

        End Try

        Return strSelectedFont
    End Function

    ' RDC 02.10.2014 : Get Language Name
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="intLangCode">Integer : Selected language code.</param>
    ''' <returns>String: Returns corresponding language name for selected language code. </returns>
    ''' <remarks></remarks>
    Private Function getLanguageName(ByVal intLangCode As Integer) As String
        Dim strLangName As String = "English"
        Try
            Using cn As New SqlConnection(L_strCnn)
                cn.Open()

                Dim Qry As String = <sql>
                                        Select b.[Language] As CodeLang
                                        From dbo.EgswTranslation As a
                                           Left Join dbo.EgswLanguage As b On a.CodeDictionary = b.Code
                                        Where(a.Code = @LanguageCode or b.Code = @LanguageCode)

                                    </sql>.Value

                Using cm As New SqlCommand()
                    cm.Connection = cn
                    cm.CommandType = CommandType.Text
                    cm.CommandText = Qry

                    cm.CreateParameter()
                    cm.Parameters.Add("@LanguageCode", SqlDbType.Int).Value = intLangCode

                    Dim tmpLanguage As String = cm.ExecuteScalar

                    If tmpLanguage.Trim.Length > 0 Then
                        strLangName = tmpLanguage
                    End If

                End Using

                cn.Close()
            End Using
        Catch ex As Exception

        End Try
        Return strLangName
    End Function

    Private Function getAlternateQuantity(strQuantity As String, strUnit As String, Optional intCodeTrans As Integer = 1, Optional intCodeSite As Integer = 0) As DataTable
        Dim dt As New DataTable
        Try
            Using cn As New SqlConnection(L_strCnn)
                cn.Open()
                Using da As New SqlDataAdapter("exec sp_EgswGetMetricImperialFromStringQtyAndUnit '" & strQuantity & "','" & strUnit & "'," & intCodeTrans & "," & intCodeSite, cn)
                    da.Fill(dt)
                End Using
                cn.Close()
            End Using
        Catch ex As Exception

        End Try
        Return dt
    End Function

    Private Function getRecipeSiteOwner(intRecipeId As Integer, intVersion As Integer) As Integer
        Dim intSite As Integer = 1
        Try
            Using cn As New SqlConnection(L_strCnn)
                cn.Open()
                Using cm As New SqlCommand("Select CodeSite From EgswListe Where (Code =" & intRecipeId & " Or Parent = " & intRecipeId & ") And Version = " & intVersion, cn)
                    intSite = cm.ExecuteScalar
                End Using
                cn.Close()
            End Using
        Catch ex As Exception

        End Try
        Return intSite
    End Function

    Public Function AsposeExportToWord(ByVal intListeID As Integer,
                                       ByVal bitFormat As Byte,
                                       ByVal intCodeTrans As Integer,
                                       ByRef strErr As String,
                                       ByRef strFilename As String,
                                       ByVal strImage As String,
                                       ByVal strImage2 As String,
                                       ByVal intCodeLang As Integer,
                                       blnUseFractions As Boolean,
                                       Optional ByVal bitQtyFormat As Byte = 0,
                                       Optional ByVal blCookmode As Boolean = False,
                                       Optional ByVal intCodeSet As Integer = 0,
                                       Optional udtListeType As enumDataListItemType = enumDataListItemType.Recipe,
                                       Optional ByVal blnMetImp As Boolean = True,
                                       Optional intCodeSite As Integer = 0,
                                       Optional ByVal intLangFromCodeDictionary As Integer = 1,
                                       Optional blnRemoveTrailingZeroes As Boolean = False) As Document


        Dim doc As New Document
        Dim builder As DocumentBuilder = New DocumentBuilder(doc)

        Dim strHTMLContent As StringBuilder = New StringBuilder()
        Dim dsRecipeDetails As New DataSet

        Dim lblRecipeID As String = ""
        Dim lblRecipeNumber As String = ""
        Dim lblSubTitle As String = ""
        Dim lblRecipeDescription As String = ""
        Dim lblRecipeRemark As String = ""
        Dim lblYield1 As String = ""
        Dim lblYield2 As String = ""
        Dim lblWeight As String = ""
        Dim lblCostPerRecipe As String = ""
        Dim lblCostPerServings As String = ""
        Dim lblInformation As String = ""
        Dim lblRecipeStatus As String = ""
        Dim lblWebStatus As String = ""
        Dim lblDateCreated As String = ""
        Dim lblDateLastModified As String = ""
        Dim lblLastTested As String = ""
        Dim lblDateDeveloped As String = ""
        Dim lblDateOfFinalEdit As String = ""
        Dim lblDevelopmentPurpose As String = ""
        Dim lblUpdatedBy As String = ""
        Dim lblCreatedBy As String = ""
        Dim lblModifiedBy As String = ""
        Dim lblTestedBy As String = ""
        Dim lblDevelopedBy As String = ""
        Dim lblFinalEditBy As String = ""
        Dim lblComments As String = ""
        Dim lblAttributes As String = ""
        Dim lblRecipeBrand As String = ""
        Dim lblRecipeNote As String = ""
        Dim lblRecipeAddNote As String = ""
        Dim lblPlacements As String = ""
        Dim lblNutritionalInformation As String = ""
        Dim lblCalories As String = ""
        Dim lblCaloriesFromFat As String = ""
        Dim lblSatFat As String = ""
        Dim lblTransFat As String = ""
        Dim lblMonoSatFat As String = ""
        Dim lblPolyFat As String = ""
        Dim lblTotalFat As String = ""
        Dim lblCholesterol As String = ""
        Dim lblSodium As String = ""
        Dim lblTotalCarbohydrates As String = ""
        Dim lblSugars As String = ""
        Dim lblDietaryFiber As String = ""
        Dim lblNetCarbohydrates As String = ""
        Dim lblProtein As String = ""
        Dim lblVitaminA As String = ""
        Dim lblVitaminC As String = ""
        Dim lblCalcium As String = ""
        Dim lblIron As String = ""
        Dim lblMonoUnsaturated As String = ""
        Dim lblPolyUnsaturated As String = ""
        Dim lblPotassium As String = ""
        Dim lblVitaminD As String = ""
        Dim lblVitaminE As String = ""
        Dim lblOmega3 As String = ""
        Dim lblNetCarbs As String = ""
        Dim lblThiamin As String = ""
        Dim lblRiboflavin As String = ""
        Dim lblNiacin As String = ""
        Dim lblVitaminB6 As String = ""
        Dim lblFolate As String = ""
        Dim lblVitaminB12 As String = ""
        Dim lblBiotin As String = ""
        Dim lblPantothenicAcid As String = ""
        Dim lblPhosphorus As String = ""
        Dim lblIodine As String = ""
        Dim lblMagnesium As String = ""
        Dim lblZinc As String = ""
        Dim lblManganese As String = ""

        Dim strRecipeID As String = ""
        Dim strRecipeNumber As String = ""
        Dim strSubTitle As String = ""
        Dim strRecipeDescription As String = ""
        Dim strRecipeRemark As String = ""
        Dim strWeight As String = ""
        Dim strWeightQty As String = ""
        Dim strImagePath As String = ""
        Dim strRecipeName As String = ""
        Dim strSubHeading As String = ""
        Dim strServings As String = ""
        Dim strYield As String = ""
        Dim strYield2 As String = ""
        Dim strServingsUnit As String = ""
        Dim strRecipeTime As String = ""
        Dim strMethodHeader As String = ""
        Dim strIngredients As String = ""
        Dim strUOM As String = ""
        Dim strDirections As String = ""
        Dim strAbbrDirections As String = ""
        Dim strFootNote1 As String = ""
        Dim strFootNote2 As String = ""
        Dim strCostPerRecipe As String = ""
        Dim strCostPerServings As String = ""
        Dim strCurrency As String = ""
        Dim strRecipeStatus As String = ""
        Dim strWebStatus As String = ""
        Dim strDateCreated As String = ""
        Dim strDateLastModified As String = ""
        Dim strLastTested As String = ""
        Dim strDateDeveloped As String = ""
        Dim strDateOfFinalEdit As String = ""
        Dim strDevelopmentPurpose As String = ""
        Dim strUpdatedBy As String = ""
        Dim strCreatedBy As String = ""
        Dim strModifiedBy As String = ""
        Dim strTestedBy As String = ""
        Dim strDevelopedBy As String = ""
        Dim strFinalEditBy As String = ""
        Dim strSubmitDate As String = ""
        Dim strOwnerName As String = ""
        Dim strComments As String = ""
        Dim strAttributes As String = ""
        Dim strParents As String = ""
        Dim strRecipeBrand As String = ""
        Dim strRecipeBrandClassification As String = ""
        Dim strPlacementName As String = ""
        Dim strPlacementDate As String = ""
        Dim strPlacementDescription As String = ""
        Dim strCalories As String = ""
        Dim strCaloriesFromFat As String = ""
        Dim strSatFat As String = ""
        Dim strTransFat As String = ""
        Dim strMonoSatFat As String = ""
        Dim strPolyFat As String = ""
        Dim strTotalFat As String = ""
        Dim strCholesterol As String = ""
        Dim strSodium As String = ""
        Dim strTotalCarbohydrates As String = ""
        Dim strSugars As String = ""
        Dim strDietaryFiber As String = ""
        Dim strNetCarbohydrates As String = ""
        Dim strProtein As String = ""
        Dim strVitaminA As String = ""
        Dim strVitaminC As String = ""
        Dim strCalcium As String = ""
        Dim strIron As String = ""
        Dim strMonoUnsaturated As String = ""
        Dim strPolyUnsaturated As String = ""
        Dim strPotassium As String = ""
        Dim strVitaminD As String = ""
        Dim strVitaminE As String = ""
        Dim strOmega3 As String = ""
        Dim strUnitCalories As String = ""
        Dim strUnitCaloriesFromFat As String = ""
        Dim strUnitSatFat As String = ""
        Dim strUnitTransFat As String = ""
        Dim strUnitMonoSatFat As String = ""
        Dim strUnitPolyFat As String = ""
        Dim strUnitTotalFat As String = ""
        Dim strUnitCholesterol As String = ""
        Dim strUnitSodium As String = ""
        Dim strUnitTotalCarbohydrates As String = ""
        Dim strUnitSugars As String = ""
        Dim strUnitDietaryFiber As String = ""
        Dim strUnitNetCarbohydrates As String = ""
        Dim strUnitProtein As String = ""
        Dim strUnitVitaminA As String = ""
        Dim strUnitVitaminC As String = ""
        Dim strUnitCalcium As String = ""
        Dim strUnitIron As String = ""
        Dim strUnitMonoUnsaturated As String = ""
        Dim strUnitPolyUnsaturated As String = ""
        Dim strUnitPotassium As String = ""
        Dim strUnitVitaminD As String = ""
        Dim strUnitVitaminE As String = ""
        Dim strUnitOmega3 As String = ""
        Dim strFormatCalories As String = ""
        Dim strFormatCaloriesFromFat As String = ""
        Dim strFormatSatFat As String = ""
        Dim strFormatTransFat As String = ""
        Dim strFormatMonoSatFat As String = ""
        Dim strFormatPolyFat As String = ""
        Dim strFormatTotalFat As String = ""
        Dim strFormatCholesterol As String = ""
        Dim strFormatSodium As String = ""
        Dim strFormatTotalCarbohydrates As String = ""
        Dim strFormatSugars As String = ""
        Dim strFormatDietaryFiber As String = ""
        Dim strFormatNetCarbohydrates As String = ""
        Dim strFormatProtein As String = ""
        Dim strFormatVitaminA As String = ""
        Dim strFormatVitaminC As String = ""
        Dim strFormatCalcium As String = ""
        Dim strFormatIron As String = ""
        Dim strFormatMonoUnsaturated As String = ""
        Dim strFormatPolyUnsaturated As String = ""
        Dim strFormatPotassium As String = ""
        Dim strFormatVitaminD As String = ""
        Dim strFormatVitaminE As String = ""
        Dim strFormatOmega3 As String = ""
        Dim strNutrients As String = ""
        Dim strThiamin As String = ""
        Dim strFormatThiamin As String = ""
        Dim strUnitThiamin As String = ""
        Dim strRiboflavin As String = ""
        Dim strFormatRiboflavin As String = ""
        Dim strUnitRiboflavin As String = ""
        Dim strNiacin As String = ""
        Dim strFormatNiacin As String = ""
        Dim strUnitNiacin As String = ""
        Dim strVitaminB6 As String = ""
        Dim strFormatVitaminB6 As String = ""
        Dim strUnitVitaminB6 As String = ""
        Dim strFolate As String = ""
        Dim strFormatFolate As String = ""
        Dim strUnitFolate As String = ""
        Dim strVitaminB12 As String = ""
        Dim strFormatVitaminB12 As String = ""
        Dim strUnitVitaminB12 As String = ""
        Dim strBiotin As String = ""
        Dim strFormatBiotin As String = ""
        Dim strUnitBiotin As String = ""
        Dim strPantothenicAcid As String = ""
        Dim strFormatPantothenicAcid As String = ""
        Dim strUnitPantothenicAcid As String = ""
        Dim strPhosphorus As String = ""
        Dim strFormatPhosphorus As String = ""
        Dim strUnitPhosphorus As String = ""
        Dim strIodine As String = ""
        Dim strFormatIodine As String = ""
        Dim strUnitIodine As String = ""
        Dim strMagnesium As String = ""
        Dim strFormatMagnesium As String = ""
        Dim strUnitMagnesium As String = ""
        Dim strZinc As String = ""
        Dim strFormatZinc As String = ""
        Dim strUnitZinc As String = ""
        Dim strManganese As String = ""
        Dim strFormatManganese As String = ""
        Dim strUnitManganese As String = ""
        Dim strHeaderNutrientServing As String = ""

        Dim imgRecipe As String = ""
        Dim imgRecipe2 As String = ""

        Dim dblQty As Double

        Dim intAttributesCode As Integer
        Dim intAttributesParent As Integer
        Dim intAttributesMain As Integer

        Dim isDisplay As Boolean = False

        Dim blnIncludeCostPerRecipe As Boolean = False ' IIf(bitFormat = 1, True, False)
        Dim blnIncludeCostPerServings As Boolean = False ' IIf(bitFormat = 1, True, False)
        Dim blnIncludeInformation As Boolean = False ' IIf(bitFormat = 1, True, False)
        Dim blnIncludeComment As Boolean = False ' IIf(bitFormat = 1, True, False)
        Dim blnIncludeKeyword As Boolean = False ' IIf(bitFormat = 1, True, False)
        Dim blnIncludeBrand As Boolean = False ' IIf(bitFormat = 1, True, False)
        Dim blnIncludePublication As Boolean = False ' IIf(bitFormat = 1, True, False)

        Dim blnDisplayPreparationHeader As Boolean = False
        Dim blnDisplayNotesHeader As Boolean = False
        Dim blnDisplayAdditionalNotes As Boolean = False

        GetRecipeCode(intListeID, m_RecipeId, m_Version)

        ' RDC 01.13.2014 : Code Site handler
        intCodeSite = getRecipeSiteOwner(m_RecipeId, m_Version)

        If udtListeType = enumDataListItemType.Merchandise Then
            'AGL 2012.10.12 - CWM-1634 - added branch for merchandise
            dsRecipeDetails = GetMerchandiseDetails(m_RecipeId, m_Version, intCodeTrans, 0, intCodeSet)
        Else
            dsRecipeDetails = GetRecipeDetails(m_RecipeId, m_Version, intCodeTrans, 0, intCodeSet, blnMetImp, intCodeSite) 'CMV 051911
        End If

        ' RDC 12.09.2013 : Translation for labels
        Dim cLang As New clsEGSLanguage(intLangFromCodeDictionary) 'JTOC 12.11.2013 intCodeLang to intLangFromCodeDictionary

        'TRANSLATION OF LABELS
        lblRecipeID = cLang.GetString(clsEGSLanguage.CodeType.Recipe) & " ID" '"Recipe ID"
        lblRecipeNumber = cLang.GetString(clsEGSLanguage.CodeType.RecipeNumber) '"Recipe Number"
        lblRecipeDescription = cLang.GetString(clsEGSLanguage.CodeType.Description) '"Description"
        lblRecipeRemark = cLang.GetString(clsEGSLanguage.CodeType.Remark) '"Remark"
        lblYield1 = cLang.GetString(clsEGSLanguage.CodeType.Yield) & " 1:" '"Yield 1: "
        lblYield2 = cLang.GetString(clsEGSLanguage.CodeType.Yield) & " 2:" '"Yield 2: "
        lblWeight = cLang.GetString(clsEGSLanguage.CodeType.Weight) & "(" & cLang.GetString(clsEGSLanguage.CodeType.Sub_Recipe) & "):" '"Weight(Subrecipe): "



        'AGL 2012.10.31 - CWM-1971
        Dim clsLicense As New clsLicense
        If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
            lblSubTitle = cLang.GetString(clsEGSLanguage.CodeType.SubTitle) '"SubTitle" '-- JBB 02.21.2012 "Sub Title"
        Else
            lblSubTitle = cLang.GetString(clsEGSLanguage.CodeType.SubName)
        End If

        lblCostPerRecipe = cLang.GetString(clsEGSLanguage.CodeType.Cost) & " " & cLang.GetString(clsEGSLanguage.CodeType.per_) & " " & cLang.GetString(clsEGSLanguage.CodeType.Recipe)
        lblCostPerServings = cLang.GetString(clsEGSLanguage.CodeType.Cost) & " " & cLang.GetString(clsEGSLanguage.CodeType.per_serving)
        lblInformation = cLang.GetString(clsEGSLanguage.CodeType.Embassy)
        lblRecipeStatus = cLang.GetString(clsEGSLanguage.CodeType.RecipeStatus) & ":"
        lblUpdatedBy = cLang.GetString(clsEGSLanguage.CodeType.UpdatedBy) & ":"
        lblWebStatus = cLang.GetString(clsEGSLanguage.CodeType.WebStatus) & ":"
        lblDateCreated = cLang.GetString(clsEGSLanguage.CodeType.DateCreated) & ":"
        lblCreatedBy = cLang.GetString(clsEGSLanguage.CodeType.CreatedBY) & ":"
        lblDateLastModified = cLang.GetString(clsEGSLanguage.CodeType.DateLastModified) & ":"
        lblModifiedBy = cLang.GetString(clsEGSLanguage.CodeType.ModifiedBy) & ":"
        lblLastTested = cLang.GetString(clsEGSLanguage.CodeType.DateLastTested) & ":"
        lblTestedBy = cLang.GetString(clsEGSLanguage.CodeType.TestedBy) & ":"
        lblDateDeveloped = cLang.GetString(clsEGSLanguage.CodeType.DateDeveloped) & ":"
        lblDevelopedBy = cLang.GetString(clsEGSLanguage.CodeType.DevelopedBy) & ":"
        lblDateOfFinalEdit = cLang.GetString(clsEGSLanguage.CodeType.FinalEditDate) & ":"
        lblFinalEditBy = cLang.GetString(clsEGSLanguage.CodeType.FinalEditBy) & ":"
        lblDevelopmentPurpose = cLang.GetString(clsEGSLanguage.CodeType.DevelopmentPurpose) & ":"
        lblComments = cLang.GetString(clsEGSLanguage.CodeType.Comments)
        lblAttributes = cLang.GetString(clsEGSLanguage.CodeType.Attributes)
        lblRecipeBrand = cLang.GetString(clsEGSLanguage.CodeType.RecipeBrands)
        lblRecipeAddNote = cLang.GetString(clsEGSLanguage.CodeType.RecipeAddtionalNotes)
        lblRecipeNote = cLang.GetString(clsEGSLanguage.CodeType.Notes)
        'AGL 2012.10.31 - CWM-1971
        If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
            lblPlacements = cLang.GetString(clsEGSLanguage.CodeType.RecipePlacements)
        Else
            lblPlacements = cLang.GetString(clsEGSLanguage.CodeType.Publication)
        End If

        lblNutritionalInformation = ""
        lblCalories = cLang.GetString(clsEGSLanguage.CodeType.Calories)
        lblCaloriesFromFat = cLang.GetString(clsEGSLanguage.CodeType.CaloriesfromFat)
        lblSatFat = "Sat Fat"
        lblTransFat = "Trans Fat"
        lblMonoSatFat = "Mono Sat Fat"
        lblPolyFat = "Poly Sat Fat"
        lblTotalFat = "Total Fat"
        lblCholesterol = "Cholesterol"
        lblSodium = "Sodium"
        lblTotalCarbohydrates = "Total Carbohydrates"
        lblSugars = "Sugars"
        lblDietaryFiber = cLang.GetString(clsEGSLanguage.CodeType.DietaryFiber)
        lblNetCarbohydrates = "Net Carbohydrates"
        lblProtein = "Protein"
        lblVitaminA = "Vitamin A"
        lblVitaminC = "Vitamin C"
        lblCalcium = cLang.GetString(clsEGSLanguage.CodeType.Calcium)
        lblIron = "Iron"
        lblMonoUnsaturated = "Monounsaturated"
        lblPolyUnsaturated = "Polyunsaturated"
        lblPotassium = "Potassium"
        lblVitaminD = "Vitamin D"
        lblVitaminE = "Vitamin E"
        lblNetCarbs = "* " & """Net Carbs""" & " are total carbohydrates minus dietary fiber and sugar alcohol as these have a minimal impact on blood sugar."
        'lblOmega3 = "Omega3"
        lblThiamin = "Thiamin"
        lblRiboflavin = "Riboflavin"
        lblNiacin = "Niacin"
        lblVitaminB6 = "VitaminB6"
        lblFolate = "Folate"
        lblVitaminB12 = "VitaminB12"
        lblBiotin = "Biotin"
        lblPantothenicAcid = "Pantothenic_Acid"
        lblPhosphorus = "Phosphorus"
        lblIodine = "Iodine"
        lblMagnesium = "Magnesium"
        lblZinc = "Zinc"
        lblManganese = "Manganese"
        lblOmega3 = "Omega-3"

        If dsRecipeDetails.Tables("Table1").Rows.Count > 0 Then


            'SET VALUES
            strRecipeID = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("RecipeID"))
            strRecipeNumber = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Number"))
            strSubTitle = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("SubTitle"))

            'JTOC 10.29.2013
            '----------------------------------------------------------------------------------------------------
            strRecipeDescription = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Description"))
            strRecipeRemark = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Remark"))
            strWeight = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Weight"))
            strWeightQty = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("SrQty"))
            '----------------------------------------------------------------------------------------------------

            strRecipeName = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Name"))
            strSubHeading = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("SubHeading"))

            ''strImagePath = Server.MapPath("Images/test.jpg") 'CMV 051911

            ''Dim imageRecipe As New System.Web.UI.WebControls.Image 'CMV 051911
            ''With imageRecipe
            ''    .ID = "Image1"
            ''    .Height = 240
            ''    .Width = 240
            ''    .ImageUrl = "Images/test.jpg"
            ''End With

            ' RDC 12.12.2013 : Discarded on top variables in displaying yield/subrecipe wt.
            Dim decYield1 As Decimal = 0D, _
                decYield2 As Decimal = 0D, _
                decSrWt As Decimal = 0D
            Dim strYield1Unit As String = dsRecipeDetails.Tables(11).Rows(0).Item("Yield1Unit").ToString.Replace("[_]", "").Replace("n/a", "").Replace("_", " ").ToLower, _
                strYield2Unit As String = dsRecipeDetails.Tables(11).Rows(0).Item("Yield2Unit").ToString.Replace("[_]", "").Replace("n/a", "").Replace("_", " ").ToLower, _
                strSrWtUnit As String = dsRecipeDetails.Tables(11).Rows(0).Item("SrUnit").ToString.Replace("[_]", "").Replace("n/a", "").Replace("_", " ").ToLower

            If Not IsDBNull(dsRecipeDetails.Tables(11).Rows(0).Item("Yield1")) Then decYield1 = CDec(dsRecipeDetails.Tables(11).Rows(0).Item("Yield1"))
            If Not IsDBNull(dsRecipeDetails.Tables(11).Rows(0).Item("Yield2")) Then decYield2 = CDec(dsRecipeDetails.Tables(11).Rows(0).Item("Yield2"))
            If Not IsDBNull(dsRecipeDetails.Tables(11).Rows(0).Item("SubRecipeQty")) Then decSrWt = CDec(dsRecipeDetails.Tables(11).Rows(0).Item("SubRecipeQty"))

            'If decYield1 > 0 And Not strYield1Unit = "[_]" And Not strYield1Unit.ToLower = "n/a" And Not strYield1Unit.Trim.Length = 0 And Not strYield1Unit.EndsWith("s") And Not strYield1Unit.ToLower.Trim = "g" And Not strYield1Unit.Trim.Length = 1 Then
            '    If decYield1 > 1 Then
            '        If strYield1Unit.Trim.Length > 0 And Char.IsLetter(Mid(strYield1Unit, strYield1Unit.Length, 1)) Then strYield1Unit &= "s"
            '    End If

            'End If

            'If decYield2 > 0 And Not strYield2Unit = "[_]" And Not strYield2Unit.ToLower = "n/a" And Not strYield2Unit.Trim.Length = 0 And Not strYield2Unit.EndsWith("s") And Not strYield2Unit.ToLower.Trim = "g" And Not strYield2Unit.Trim.Length = 1 Then
            '    If decYield2 > 1 Then
            '        If strYield2Unit.Trim.Length > 0 And Char.IsLetter(Mid(strYield2Unit, strYield2Unit.Length, 1)) Then strYield2Unit &= "s"
            '    End If
            'End If

            'If CDec(Format(decSrWt, "#.000#")) > 0 And Not strSrWtUnit = "[_]" And Not strSrWtUnit.ToLower = "n/a" And Not strSrWtUnit.Trim.Length = 0 And Not strSrWtUnit.EndsWith("s") And Not strSrWtUnit.ToLower.Trim = "g" And Not strSrWtUnit.Trim.Length = 1 Then
            '    If decSrWt > 1 Then
            '        If strSrWtUnit.Trim.Length > 0 And Char.IsLetter(Mid(strSrWtUnit, strSrWtUnit.Length, 1)) Then strSrWtUnit &= "s"
            '    End If
            'End If

            Dim strYield1 As String = ConvertDecimalToFraction2(fctCheckDbNullNumeric(decYield1)), _
                strSrWt As String = ConvertDecimalToFraction2(fctCheckDbNullNumeric(decSrWt))

            Dim intFieldsToDisplay As Integer = 0, intFieldWidth As Integer = 0, intTableWidth As Integer = 620
            If G_ExportOptions.blnExpIncludeYield1 And decYield1 > 0 Then intFieldsToDisplay += 1
            If G_ExportOptions.blnExpIncludeYield2 And decYield2 > 0 Then intFieldsToDisplay += 1
            If G_ExportOptions.blnExpSubRecipeWt And decSrWt > 0 Then intFieldsToDisplay += 1

            Select Case intFieldsToDisplay
                Case 1
                    intFieldWidth = 620
                    intTableWidth = 250
                Case 2
                    intFieldWidth = 310
                    intTableWidth = 400
                Case 3
                    intFieldWidth = CInt(620 / 3)
                Case Else
                    intFieldWidth = CInt(620 / 3)
            End Select

            strYield2 = ConvertDecimalToFraction2(fctCheckDbNullNumeric(decYield2))

            Dim arrYields(0 To 2) As String
            Dim arrYieldsLabel(0 To 2) As String
            With G_ExportOptions
                If .blnExpIncludeYield1 And decYield1 > 0 Then
                    arrYields(0) = strYield1 & " " & strYield1Unit
                    arrYieldsLabel(0) = lblYield1.ToString
                End If
                If .blnExpIncludeYield2 And decYield2 > 0 Then
                    arrYields(1) = strYield2 & " " & strYield2Unit
                    arrYieldsLabel(1) = lblYield2.ToString
                End If
                If .blnExpSubRecipeWt And decSrWt > 0 Then
                    arrYields(2) = strSrWt & " " & strSrWtUnit
                    arrYieldsLabel(2) = lblWeight.ToString
                End If
            End With

            strServings = "<center><table width='" & intTableWidth & "'><tr>"
            With G_ExportOptions
                If .blnExpIncludeYield1 And decYield1 > 0 Then strServings &= "<td width='" & intFieldWidth & "' align='center' style='font-size: 11.5pt; font-family: Calibri;'> <b>" & lblYield1.ToString & "</b>&nbsp;" & strYield1 & " " & strYield1Unit & " </td>"
                If .blnExpIncludeYield2 And decYield2 > 0 Then strServings &= "<td width='" & intFieldWidth & "' align='center' style='font-size: 11.5pt; font-family: Calibri;'> <b>" & lblYield2.ToString & "</b>&nbsp;" & strYield2 & " " & strYield2Unit & "</td>"
                If .blnExpSubRecipeWt And decSrWt > 0 Then strServings &= "<td width='" & intFieldWidth & "' align='center' style='font-size: 11.5pt; font-family: Calibri;'> <b>" & lblWeight.ToString & "</b> &nbsp;" & strSrWt & " " & strSrWtUnit & "</td>"
            End With
            strServings &= "</tr></table></center>"

            strMethodHeader = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("MethodHeader"))
            strDirections = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Note"))
            ' RDC 11.28.2013 : Fix for "There is no row in position 0"
            'If Not IsDBNull(dsRecipeDetails.Tables("Table3").Rows.Count) Then
            If dsRecipeDetails.Tables("Table3").Rows.Count > 0 Then
                strAbbrDirections = fctCheckDbNull(dsRecipeDetails.Tables("Table3").Rows(0).Item("CookMode"))
            End If

            strFootNote1 = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("FootNote1"))
            strFootNote2 = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("FootNote2"))
            strCurrency = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Currency"))
            'strCostPerRecipe = strCurrency & " " & fctCheckDbNullNumeric(dsRecipeDetails.Tables("Table1").Rows(0).Item("CostPrice"))
            'strCostPerServings = strCurrency & " " & fctCheckDbNullNumeric(dsRecipeDetails.Tables("Table1").Rows(0).Item("CostPricePerServing"))
            strRecipeStatus = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("RecipeStatusName"))
            strUpdatedBy = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("UpdatedBy"))
            strWebStatus = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("WebStatusName"))
            If Not IsDBNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateCreated")) Then strDateCreated = CDate(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateCreated")).ToString("MM/dd/yyyy")
            strCreatedBy = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("CreatedBy"))
            If Not IsDBNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateLastModified")) Then strDateLastModified = CDate(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateLastModified")).ToString("MM/dd/yyyy")
            strModifiedBy = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("ModifiedBy"))
            If Not IsDBNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateTested")) Then strLastTested = CDate(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateTested")).ToString("MM/dd/yyyy")
            strTestedBy = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("TestedBy"))
            If Not IsDBNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateDeveloped")) Then strDateDeveloped = CDate(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateDeveloped")).ToString("MM/dd/yyyy")
            strDevelopedBy = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("DevelopedBy"))
            If Not IsDBNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateFinalEdit")) Then strDateOfFinalEdit = CDate(dsRecipeDetails.Tables("Table1").Rows(0).Item("DateFinalEdit")).ToString("MM/dd/yyyy")
            strFinalEditBy = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("FinalEditBy"))
            strDevelopmentPurpose = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("DevelopmentPurpose"))

            isDisplay = CBoolDB(dsRecipeDetails.Tables("Table1").Rows(0).Item("DisplayNutrition")) ' JBB 07.22.2011

            Dim strHeader As String = fGetMethodFormat("nh")
            Dim strItems As String = fGetMethodFormat("s")
            Dim dicIsDisplay As New Dictionary(Of String, Boolean)
            Dim dicColumnName As New Dictionary(Of String, String)
            Dim dicUnit As New Dictionary(Of String, String)
            Dim dicFormat As New Dictionary(Of String, String)
            Dim intIndex As Integer = 0
            Dim strColCalories As String = ""
            Dim dtNutrients As DataTable = dsRecipeDetails.Tables("Table4")
            If dtNutrients.Rows.Count > 0 Then  '' JBB 05.23.2012
                For Each dcNutrient As DataColumn In dtNutrients.Columns
                    Dim strColumn As String = dcNutrient.ColumnName
                    If strColCalories = "" Then strColCalories = strColumn

                    If strColumn.Trim.ToLower <> "recipeid" And strColumn.Trim.ToLower <> "version" And strColumn.Trim.ToLower <> "portionsize" Then
                        If strColumn.Contains("Display") Then
                            dicIsDisplay.Add(strColumn.ToLower(), CBool(dtNutrients.Rows(intIndex)(strColumn)))
                        ElseIf strColumn.Contains("Unit_") Then
                            dicUnit.Add(strColumn.ToLower(), CStr(dtNutrients.Rows(intIndex)(strColumn)))
                        ElseIf strColumn.Contains("Format") Then
                            dicFormat.Add(strColumn.ToLower(), CStr(dtNutrients.Rows(intIndex)(strColumn)))
                        End If
                        'strNutrients.Append(strColumn.Replace("Display", "") + " " + dtNutrients.Rows(intIndex)(strColumn).ToString() + ", ")
                    End If
                    dicColumnName.Add(strColumn.ToLower(), strColumn)
                    'JTOC 14.12.2012 Removed Calo in condition
                    'If strColumn.Contains("Calo") And (Not strColumn.Contains("Display") Or Not strColumn.Contains("Unit_") Or Not strColumn.Contains("Format")) Then
                    If (Not strColumn.Contains("Display") Or Not strColumn.Contains("Unit_") Or Not strColumn.Contains("Format")) Then
                        strColCalories = strColumn
                    End If
                Next
                ' End If
                strHeaderNutrientServing = ""
                If dsRecipeDetails.Tables("Table4").Columns.Contains("PortionSize") = True Then
                    If dicColumnName.ContainsKey(strColCalories) Then
                        strHeaderNutrientServing = dsRecipeDetails.Tables("Table1").Rows(0).Item("PortionSize").ToString.Trim
                    Else
                        If dsRecipeDetails.Tables("Table4").Rows(0).Item(strColCalories).ToString.Trim <> "" Then
                            strHeaderNutrientServing = dsRecipeDetails.Tables("Table4").Rows(0).Item("PortionSize").ToString.Trim
                        Else
                            If dsRecipeDetails.Tables("Table1").Columns.Contains("Yield") = True Then
                                strHeaderNutrientServing = dsRecipeDetails.Tables("Table1").Rows(0).Item("Yield").ToString.Trim
                            End If
                        End If
                    End If
                Else
                    If dsRecipeDetails.Tables("Table1").Columns.Contains("Yield") = True Then
                        strHeaderNutrientServing = dsRecipeDetails.Tables("Table1").Rows(0).Item("Yield").ToString.Trim
                    End If
                End If

                Dim lstKey As List(Of String)
                lstKey = New List(Of String)(dicIsDisplay.Keys)
                For Each dcNutrient As DataColumn In dtNutrients.Columns
                    Dim strColumn As String = dcNutrient.ColumnName
                    If strColumn.Trim.ToLower <> "recipeid" And strColumn.Trim.ToLower <> "version" And strColumn.Trim.ToLower <> "portionsize" Then
                        If Not strColumn.Contains("Display") And Not strColumn.Contains("Unit_") And Not strColumn.Contains("Format") Then
                            If lstKey.Contains(("Display" + strColumn.ToString()).ToLower) = True Then
                                If dicIsDisplay(("Display" + strColumn.ToString()).ToLower) = True Then
                                    If dtNutrients.Rows(intIndex)(strColumn).ToString().Trim <> "-1" Then
                                        Dim strNutDisplayValue As String = Format(dicFormat((strColumn.ToString() + "Format").ToLower), IIf(dtNutrients.Rows(intIndex)(strColumn).ToString().Trim() <> "-1", dtNutrients.Rows(intIndex)(strColumn), 0)) '
                                        'strNutrients.Append(strColumn + " " + strNutDisplayValue + dicUnit(("Unit_" + strColumn.ToString()).ToLower) + ", ")
                                        strNutrients = strNutrients & Replace(strColumn, "_", " ") & " " & strNutDisplayValue & dicUnit(("Unit_" + strColumn.ToString()).ToLower) & ", "
                                        lblNutritionalInformation = cLang.GetString(clsEGSLanguage.CodeType.NutritionalInfo) & " " & strHeaderNutrientServing & " "
                                    End If
                                End If
                            End If
                        End If

                        'strNutrients.Append(strColumn.Replace("Display", "") + " " + dtNutrients.Rows(intIndex)(strColumn).ToString() + ", ")
                    End If
                Next

                If Right(strNutrients, 2) = ", " Then strNutrients = strNutrients.Remove(Len(strNutrients) - 2, 2)
            Else '' JBB 05.23.2012
                lblNutritionalInformation = ""
                strHeaderNutrientServing = ""
                strNutrients = ""
            End If '' JBB 05.23.2012

            ' RDC 11.28.2013 : Fix for "There is no row on position 0"
            If dsRecipeDetails.Tables("Table3").Rows.Count > 0 Then
                'JBB -- 07.14.2011
                strDirections = fctGetInstrunctions(dsRecipeDetails.Tables("Table3"), fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("MethodFormat")), blCookmode)
                'TDQ 2.24.2012
                strAbbrDirections = fctGetInstrunctions(dsRecipeDetails.Tables("Table3"), fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("MethodFormat")), True)
                ''fctCheckDbNull(strDirections)
                '' strAbbrDirections = fctCheckDbNull(strAbbrDirections)
                '--
            Else
                strDirections = ""
                strAbbrDirections = ""
            End If

            Dim strExpFont As String = GetLanguage(G_ExportOptions.intExpSelectedLanguage)

            If bitFormat = 1 Then
                ' Recipe Name
                insertRecipeTitle(builder, strRecipeName.ToString)

                ' Recipe Sub Name
                If G_ExportOptions.blnExpIncludeSubName Then
                    insertNewLine(builder, 1)
                    insertRecipeTitle(builder, strSubHeading.ToString)
                End If

                insertNewLine(builder, 1)

                With G_ExportOptions
                    If .blnExpIncludeRecipeNo Or .blnExpIncludeSubName Or .blnExpIncludeItemDesc Or .blnExpIncludeRemark Then

                        Dim subTitleTable As Table = builder.StartTable()
                        ' Recipe Number
                        If G_ExportOptions.blnExpIncludeRecipeNo Then
                            insertRecipeSubTitle(builder, subTitleTable, lblRecipeNumber.Trim, strRecipeNumber.Trim)
                        End If
                        ' Sub Title
                        If G_ExportOptions.blnExpIncludeSubName Then
                            insertRecipeSubTitle(builder, subTitleTable, lblSubTitle.Trim, strSubTitle.Trim)
                        End If
                        ' Description
                        If G_ExportOptions.blnExpIncludeItemDesc And strRecipeDescription.Trim.Length > 0 Then
                            insertRecipeSubTitle(builder, subTitleTable, lblRecipeDescription.Trim, strRecipeDescription.Trim)
                        End If
                        ' Remarks
                        If G_ExportOptions.blnExpIncludeRemark And strRecipeRemark.Trim.Length > 0 Then
                            insertRecipeSubTitle(builder, subTitleTable, lblRecipeRemark.Trim, strRecipeRemark.Trim)
                        End If

                        builder.EndTable()
                    End If
                End With

                insertNewLine(builder, 3)
                ' Recipe Image 1
                imgRecipe = strImage
                insertRecipeImage(builder, imgRecipe)
                insertNewLine(builder, 1)
                ' Recipe Image 2
                If Not strImage.Contains("nopic.jpg") And Not strImage2.Contains("nopic.jpg") Then
                    If Not strImage2 = "" Then
                        imgRecipe2 = strImage2
                        insertRecipeImage(builder, imgRecipe2)
                        insertNewLine(builder, 1)
                    End If
                End If

                'Servings
                If G_ExportOptions.blnExpIncludeYield1 Or G_ExportOptions.blnExpIncludeYield2 Or G_ExportOptions.blnExpSubRecipeWt Then
                    insertYields(builder, arrYieldsLabel, arrYields)
                End If

                'Recipe Time
                If G_ExportOptions.blnExpIncludeRecipeTime Then

                    Dim recipeTable As Table = builder.StartTable()

                    For Each RecipeTime As DataRow In dsRecipeDetails.Tables("Table5").Rows
                        strRecipeTime = RecipeTime.Item("Description")
                        Dim intHours As Integer = CIntDB(RecipeTime("RecipeTimeHH"))
                        Dim intMinutes As Integer = CIntDB(RecipeTime("RecipeTimeMM"))
                        Dim intSeconds As Integer = CIntDB(RecipeTime("RecipeTimeSS"))
                        Dim strAnd As String = cLang.GetString(clsEGSLanguage.CodeType._And).ToString.ToLower & " "

                        If intHours > 0 And intMinutes > 0 And intSeconds > 0 Then          ' 111
                            If intHours = 1 Then strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHour).ToLower & ", ") Else strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHours).ToLower & ", ")
                            If intMinutes = 1 Then strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinute).ToLower & " " & strAnd) Else strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinutes).ToLower & " " & strAnd)
                            If intSeconds = 1 Then strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSecond).ToLower) Else strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSeconds).ToLower)
                        ElseIf intHours = 0 And intMinutes > 0 And intSeconds > 0 Then      ' 011
                            If intMinutes = 1 Then strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinute).ToLower & strAnd) Else strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinutes).ToLower & " " & strAnd)
                            If intSeconds = 1 Then strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSecond).ToLower) Else strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSeconds).ToLower)
                            strRecipeTime = strRecipeTime.Replace("0 %h", "")
                        ElseIf intHours > 0 And intMinutes > 0 And intSeconds = 0 Then      ' 110
                            If intHours = 1 Then strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHour).ToLower & " " & strAnd) Else strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHours).ToLower & " " & strAnd)
                            If intMinutes = 1 Then strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinute).ToLower) Else strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinutes).ToLower)
                            strRecipeTime = strRecipeTime.Replace("0 %s", "")
                        ElseIf intHours = 0 And intMinutes = 0 And intSeconds > 0 Then      ' 001
                            If intSeconds = 1 Then strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSecond).ToLower) Else strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSeconds).ToLower)
                            strRecipeTime = strRecipeTime.Replace("0 %h", "").Replace("0 %m", "")
                        ElseIf intHours = 0 And intMinutes > 0 And intSeconds = 0 Then      ' 010
                            If intMinutes = 1 Then strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinute).ToLower) Else strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinutes).ToLower)
                            strRecipeTime = strRecipeTime.Replace("0 %h", "").Replace("0 %s", "")
                        ElseIf intHours > 0 And intMinutes = 0 And intSeconds = 0 Then      ' 100
                            If intHours = 1 Then strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHour).ToLower) Else strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHours).ToLower)
                            strRecipeTime = strRecipeTime.Replace("0 %m", "").Replace("0 %s", "")
                        ElseIf intHours > 0 And intMinutes = 0 And intSeconds > 0 Then      ' 101
                            If intHours = 1 Then strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHour).ToLower & " " & strAnd) Else strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHours).ToLower & " " & strAnd)
                            If intSeconds = 1 Then strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSecond).ToLower) Else strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSeconds).ToLower)
                            strRecipeTime = strRecipeTime.Replace("0 %m", "")
                        Else                                                                ' 000
                            strRecipeTime = ""
                        End If

                        insertRecipeTime(builder, recipeTable, strRecipeTime.ToString)

                    Next

                    builder.EndTable()

                End If

                ' Ingredients
                If dsRecipeDetails.Tables(2).Rows.Count > 0 Then

                    Dim ingredientsTable As Table = builder.StartTable()

                    For Each rwIngredient As DataRow In dsRecipeDetails.Tables(2).Rows
                        Dim intRemainingSize As Integer = 620
                        strHTMLContent.Append("<tr>")

                        Dim intItemType As Integer
                        Dim strIngredient As String = ""
                        Dim strItemName As String = ""
                        Dim strAltIngredient As String = ""
                        Dim strIngrComplement As String = ""
                        Dim strIngrPreparation As String = ""

                        If IsDBNull(rwIngredient("Type")) Then intItemType = 0 Else intItemType = rwIngredient("Type")

                        ' Ingredient = Complement IngredientName [or AlternativeIngredient], Preparation 
                        ' Ingredient Name
                        If Not IsDBNull(rwIngredient("Name")) And Not rwIngredient("Name").ToString.Trim.Length = 0 Then
                            strItemName = rwIngredient("Name").ToString.Trim
                        End If
                        ' Alternative Ingredient
                        If Not IsDBNull(rwIngredient("AlternativeIngredient")) And Not rwIngredient("AlternativeIngredient").ToString.Trim.Length = 0 Then
                            strAltIngredient = "[" & cLang.GetString(clsEGSLanguage.CodeType.OR_) & " " & rwIngredient("AlternativeIngredient").ToString.Trim & "]"
                        End If
                        ' Complement
                        If Not IsDBNull(rwIngredient("Complement")) And Not rwIngredient("Complement").ToString.Trim.Length = 0 Then
                            strIngrComplement = rwIngredient("Complement").ToString.Trim

                        End If
                        ' Preparation
                        If Not IsDBNull(rwIngredient("Preparation")) And Not rwIngredient("Preparation").ToString.Trim.Length = 0 Then
                            strIngrPreparation = rwIngredient("Preparation").ToString.Trim
                            blnDisplayPreparationHeader = True
                        End If

                        ' Combine all information to form 1 ingredient detail
                        ' RDC 12.02.2013 : Remove comma when there is no preparation present/defined
                        If strIngrComplement.Trim.Length > 1 Then strIngredient &= strIngrComplement & " "
                        If strItemName.Trim.Length >= 1 Then strIngredient &= strItemName & " " ''AMTLA 2014.06.19 CWM-14647
                        If strAltIngredient.Trim.Length > 1 Then strIngredient &= strAltIngredient
                        If strIngrPreparation.Trim.Length > 1 Then strIngredient &= ", " & strIngrPreparation

                        ' Get alternate quantities for unvalidated ingredients
                        Dim dt As New DataTable



                        ' Get All quantities
                        ' For Metric and Imperial Quantities
                        Dim strMetricNet As String = "0", strMetricGross As String = "0", strMetricUnit As String = ""
                        Dim strImperialNet As String = "0", strImperialGross As String = "0", strImperialUnit As String = ""
                        ' For One Quantity
                        Dim strQtyNet As String = "0", strQtyGross As String = "0", strQtyUnit As String = ""
                        ' Total Wastage
                        Dim dblTotalWastage As Double = 0

                        If Not IsDBNull(rwIngredient("TotalWastage")) Then dblTotalWastage = CDbl(rwIngredient("TotalWastage"))
                        If rwIngredient("IngredientId") = 0 And rwIngredient("Type") = 0 Then
                            Dim dtqty As New DataTable
                            If Not rwIngredient("Quantity_Metric") Is Nothing Then
                                dtqty = getAlternateQuantity(rwIngredient("Quantity_Metric").ToString, rwIngredient("UOM_Metric"), intCodeTrans, intCodeSite)
                            Else
                                dtqty = getAlternateQuantity(rwIngredient("Quantity_Imperial").ToString, rwIngredient("UOM_Imperial"), intCodeTrans, intCodeSite)
                            End If

                            If dtqty.Rows.Count > 0 Then
                                For Each dr As DataRow In dtqty.Rows
                                    strMetricNet = dr("QtyMetric")
                                    strMetricGross = dr("QtyMetric")
                                    strMetricUnit = dr("UnitMetric")
                                    strImperialNet = dr("QtyImperial")
                                    strImperialGross = dr("QtyImperial")
                                    strImperialUnit = dr("UnitImperial")
                                Next
                            End If
                        Else
                            If Not IsDBNull(rwIngredient("Quantity_Metric")) Then
                                Dim metric_format As String = rwIngredient("UnitFormat").ToString
                                strMetricNet = fctFormatNumericQuantity(CDblDB(rwIngredient("Quantity_Metric").ToString), metric_format, blnRemoveTrailingZeroes, 0)
                                strMetricGross = fctFormatNumericQuantity(CDblDB(rwIngredient("QtyMetricGross").ToString), metric_format, blnRemoveTrailingZeroes, 0)

                            End If
                            If Not IsDBNull(rwIngredient("UOM_Metric")) Then
                                strMetricUnit = rwIngredient("UOM_Metric").ToString.Replace("n/a", "").Replace("[_]", "").Replace("N/A", "")
                            End If

                            If Not IsDBNull(rwIngredient("Quantity_Imperial")) Then
                                Dim imperial_format As String = rwIngredient("UnitFormat").ToString

                                strImperialNet = ConvertDecimalToFraction2(Format(CDblDB(rwIngredient("Quantity_Imperial").ToString), imperial_format))
                                strImperialGross = ConvertDecimalToFraction2(Format(CDblDB(rwIngredient("QtyImperialGross").ToString), imperial_format))

                            End If
                            If Not IsDBNull(rwIngredient("UOM_Imperial")) Then
                                strImperialUnit = rwIngredient("UOM_Imperial").ToString.Replace("n/a", "").Replace("[_]", "").Replace("N/A", "")
                            End If

                            If Not IsDBNull(rwIngredient("OneQtyNet")) Then
                                strQtyNet = ConvertDecimalToFraction2(rwIngredient("OneQtyNet"))
                                strQtyGross = ConvertDecimalToFraction2(CDbl(rwIngredient("OneQtyGross")).ToString)

                                strQtyUnit = rwIngredient("OneQtyUnit").ToString.Replace("n/a", "").Replace("[_]", "").Replace("N/A", "")
                            End If
                        End If



                        Dim intIncludedColumns As Integer = 0
                        If intItemType = 75 Then
                            Select Case bitUseOneQuantity
                                Case 0
                                    With G_ExportOptions
                                        If .blnExpIncludeImperialNetQty Then intIncludedColumns += 1
                                        If .blnExpIncludeImperialGrossQty Then intIncludedColumns += 1
                                        If .blnExpIncludeMetricNetQty Then intIncludedColumns += 1
                                        If .blnExpIncludeMetricGrossQty Then intIncludedColumns += 1
                                        intIncludedColumns += 1
                                    End With
                                Case 1
                                    With G_ExportOptions
                                        If .blnExpIncludeNetQty Then intIncludedColumns += 1
                                        If .blnExpIncludeGrossQty Then intIncludedColumns += 1
                                        intIncludedColumns += 1
                                    End With
                            End Select

                            strHTMLContent.Append("<td width ='100%' colspan='" & intIncludedColumns & "' valign='top'><b>" & strIngredient & "</b></td>")
                        Else
                            Dim intColSize As Integer = 100
                            Select Case bitUseOneQuantity
                                Case 0
                                    With G_ExportOptions
                                        If .blnExpIncludeImperialNetQty Then intIncludedColumns += 1
                                        If .blnExpIncludeImperialGrossQty Then intIncludedColumns += 1
                                        If .blnExpIncludeMetricNetQty Then intIncludedColumns += 1
                                        If .blnExpIncludeMetricGrossQty Then intIncludedColumns += 1
                                        intIncludedColumns += 1
                                    End With
                                Case 1
                                    With G_ExportOptions
                                        If .blnExpIncludeNetQty Then intIncludedColumns += 1
                                        If .blnExpIncludeGrossQty Then intIncludedColumns += 1
                                        intIncludedColumns += 1
                                    End With
                            End Select

                            Dim intUnitCode As Integer = -1, intIsImperialMetric As Integer = 9, strUnitFormat As String = "", dblUnitFactor As Decimal = 0D, intTypeMain As Integer = 0

                            Dim strUnvalidatedMetricQty As String = strMetricNet, strUnvalidatedMetricUnit As String = strMetricUnit
                            Dim strUnvalidatedImperialQty As String = strImperialNet, strUnvalidatedImperialUnit As String = strImperialUnit

                            Select Case bitUseOneQuantity
                                Case 0 ' Display Metric/Imperial Gross/Net quantities   

                                    If G_ExportOptions.blnExpIncludeImperialNetQty Then
                                        If rwIngredient("Type") = 0 And rwIngredient("IngredientId") = 0 Then

                                            If Not strUnvalidatedImperialQty = "0" Then
                                                insertIngredient(builder, ingredientsTable, ConvertDecimalToFraction2(fctCheckDbNullNumeric(strUnvalidatedImperialQty)) & " " & strUnvalidatedImperialUnit.Replace("_", " "))
                                            Else
                                                insertIngredient(builder, ingredientsTable, " ")
                                            End If

                                        Else
                                            If Not rwIngredient("type") = 4 Then
                                                If Not strImperialNet = "0" Then
                                                    insertIngredient(builder, ingredientsTable, strImperialNet & " " & strImperialUnit.Replace("_", " "))
                                                Else
                                                    insertIngredient(builder, ingredientsTable, " ")
                                                End If
                                            End If
                                        End If
                                    End If

                                    If G_ExportOptions.blnExpIncludeImperialGrossQty Then
                                        If rwIngredient("Type") = 0 And rwIngredient("IngredientId") = 0 Then
                                            If Not strUnvalidatedImperialQty = "0" Then
                                                insertIngredient(builder, ingredientsTable, ConvertDecimalToFraction2(fctCheckDbNullNumeric(strUnvalidatedImperialQty)) & " " & strUnvalidatedImperialUnit.Replace("_", " "))
                                            Else
                                                insertIngredient(builder, ingredientsTable, " ")
                                            End If
                                        Else
                                            If Not rwIngredient("type") = 4 Then
                                                If Not strImperialGross = "0" Then
                                                    insertIngredient(builder, ingredientsTable, strImperialGross & " " & strImperialUnit.Replace("_", " "))
                                                Else
                                                    insertIngredient(builder, ingredientsTable, " ")
                                                End If
                                            End If
                                        End If
                                    End If

                                    If G_ExportOptions.blnExpIncludeMetricNetQty Then
                                        If rwIngredient("Type") = 0 And rwIngredient("IngredientId") = 0 Then
                                            If Not strUnvalidatedMetricQty = "0" Then
                                                insertIngredient(builder, ingredientsTable, strUnvalidatedMetricQty & " " & strUnvalidatedMetricUnit.Replace("_", " "))
                                            Else
                                                insertIngredient(builder, ingredientsTable, " ")
                                            End If
                                        Else
                                            If Not rwIngredient("type") = 4 Then

                                                If Not strMetricNet = "0" Then
                                                    insertIngredient(builder, ingredientsTable, strMetricNet & " " & strMetricUnit.Replace("_", " "))
                                                Else
                                                    insertIngredient(builder, ingredientsTable, " ")
                                                End If


                                            End If
                                        End If
                                    End If

                                    If G_ExportOptions.blnExpIncludeMetricGrossQty Then
                                        If rwIngredient("Type") = 0 And rwIngredient("IngredientId") = 0 Then
                                            If Not strUnvalidatedMetricQty = "0" Then
                                                insertIngredient(builder, ingredientsTable, strUnvalidatedMetricQty & " " & strUnvalidatedMetricQty & " " & strUnvalidatedMetricUnit.Replace("_", " "))
                                            Else
                                                insertIngredient(builder, ingredientsTable, " ")
                                            End If
                                        Else
                                            If Not rwIngredient("type") = 4 Then
                                                If Not strMetricGross = "0" Then
                                                    insertIngredient(builder, ingredientsTable, strMetricGross & " " & strMetricUnit.Replace("_", " "))
                                                Else
                                                    insertIngredient(builder, ingredientsTable, " ")
                                                End If
                                            End If
                                        End If
                                    End If

                                Case 1 ' Display Gross and Net Quantities only

                                    If G_ExportOptions.blnExpIncludeNetQty Then
                                        If Not rwIngredient("type") = 4 Then
                                            If Not strQtyNet = "0" Then
                                                insertIngredient(builder, ingredientsTable, strQtyNet & " " & strQtyUnit)
                                            Else
                                                insertIngredient(builder, ingredientsTable, " ")
                                            End If
                                        End If
                                    End If

                                    If G_ExportOptions.blnExpIncludeGrossQty Then
                                        If Not rwIngredient("type") = 4 Then
                                            If Not strQtyNet = "0" Then
                                                insertIngredient(builder, ingredientsTable, strQtyGross & " " & strQtyUnit)
                                            Else
                                                insertIngredient(builder, ingredientsTable, " ")
                                            End If
                                        End If
                                    End If
                                Case Else
                            End Select

                            ' Ingredient name
                            insertIngredient(builder, ingredientsTable, strIngredient)
                            builder.EndRow()

                        End If

                        blnDisplayPreparationHeader = True
                    Next

                    builder.EndTable()
                Else
                    blnDisplayPreparationHeader = False
                    blnDisplayAdditionalNotes = False

                End If

                If G_ExportOptions.intExpSelectedProcedure = 0 Then
                    strMethodHeader = cLang.GetString(clsEGSLanguage.CodeType.PreparationMethod)
                Else
                    strMethodHeader = cLang.GetString(clsEGSLanguage.CodeType.CookMode)
                End If

                If G_ExportOptions.blnExpIncludeProcedure Then

                    Dim procedureTable As Table = builder.StartTable()


                    Select Case G_ExportOptions.intExpSelectedProcedure
                        Case 0
                            'Method Header
                            If strMethodHeader.ToString <> "" And blnDisplayPreparationHeader Then
                                insertProcedure(builder, procedureTable, strMethodHeader.ToString)
                            End If
                            'Directions
                            If strDirections.ToString <> "" Then
                                insertProcedure(builder, procedureTable, strDirections.ToString)
                            End If
                        Case Else
                            'Method Header
                            If strMethodHeader.ToString <> "" And blnDisplayPreparationHeader Then
                                insertProcedure(builder, procedureTable, strMethodHeader.ToString)
                            End If
                            'Directions
                            If strAbbrDirections.ToString <> "" Then
                                insertProcedure(builder, procedureTable, strAbbrDirections.ToString)
                            End If
                    End Select

                    builder.EndTable()
                End If

                blnDisplayNotesHeader = IIf(strFootNote1 <> "", True, False)
                blnDisplayAdditionalNotes = IIf(strFootNote2 <> "", True, False)

                Dim notesTable As Table = builder.StartTable()

                If G_ExportOptions.blnExpIncludeNotes And blnDisplayNotesHeader Then
                    insertNotes(builder, notesTable, cLang.GetString(clsEGSLanguage.CodeType.Notes), strFootNote1.ToString)
                End If

                'Additonal Notes
                If G_ExportOptions.blnExpIncludeAddNotes And blnDisplayAdditionalNotes Then
                    insertNotes(builder, notesTable, cLang.GetString(clsEGSLanguage.CodeType.AdditionalNotes), strFootNote2.ToString)
                End If

                builder.EndTable()

                ' Nutrients                
                If G_ExportOptions.blnExpIncludeNutrientInfo Then
                    Dim strNutBasis As String = ""
                    If Not IsDBNull(dsRecipeDetails.Tables("Table4").Rows(0).Item("NutritionBasis")) Then strNutBasis = dsRecipeDetails.Tables("Table4").Rows(0).Item("NutritionBasis")
                    strHTMLContent.Append(fctDisplayNutrientComputationForExport(m_RecipeId, dsRecipeDetails.Tables(1).Rows(0).Item("ServingsUnit").ToString, G_ExportOptions.intExpSelectedNutrientSet, G_ExportOptions.intExpSelectedLanguage, G_ExportOptions.intExpSelectedNutrientComputation, , strNutBasis, m_Version, True))

                    'Net Carbs
                    If isDisplay = True Then
                        If strNetCarbohydrates.ToString <> "" Then
                            Dim nutrientsTable As Table = builder.StartTable()
                            insertNewRowLeft(builder, nutrientsTable, strNetCarbohydrates.ToString)
                            insertNewRowCenter(builder, nutrientsTable, lblNetCarbs.ToString)
                            builder.EndTable()
                        End If
                    End If
                End If

                If G_ExportOptions.blnExpIncludeGDA Then
                    strHTMLContent.Append(fctDisplayGDAComputationForExport(m_RecipeId, dsRecipeDetails.Tables(1).Rows(0).Item("ServingsUnit").ToString, G_ExportOptions.intExpSelectedNutrientSet, G_ExportOptions.intExpSelectedLanguage, G_ExportOptions.intExpSelectedGDA, , "", m_Version, True))
                End If

                'Information
                Dim infoTable As Table = builder.StartTable()
                If G_ExportOptions.blnExpAdvIncludeInfo Then

                    builder.Font.Bold = True
                    insertNewRowCenter(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.Information))
                    insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.RecipeStatus), strRecipeStatus.ToString)

                    If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
                        insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.UpdatedBy), strUpdatedBy.ToString)
                    End If

                    If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
                        insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.WebStatus), strWebStatus.ToString)
                    End If

                    insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.DateCreated), strDateCreated.ToString)
                    insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.CreatedBY), strCreatedBy.ToString)
                    insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.DateLastModified), strDateLastModified.ToString)
                    insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.ModifiedBy), strModifiedBy.ToString)

                    If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
                        insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.DateLastTested), strLastTested.ToString)
                        insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.TestedBy), strTestedBy.ToString)
                        insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.DateDeveloped), strDateDeveloped.ToString)
                        insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.DevelopedBy), strDevelopedBy.ToString)
                        insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.FinalEditDate), strDateOfFinalEdit.ToString)
                        insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.FinalEditBy), strFinalEditBy.ToString)
                        insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.DevelopmentPurpose), strDevelopmentPurpose.ToString)
                    End If
                End If
                builder.EndTable()
                'Recipe Brand
                Dim recipeBrandTable As Table = builder.StartTable()
                If G_ExportOptions.blnExpAdvIncludeBrands Then
                    If dsRecipeDetails.Tables("table8").Rows.Count > 0 Then
                        builder.Font.Bold = True
                        insertNewRowCenter(builder, recipeBrandTable, cLang.GetString(clsEGSLanguage.CodeType.Brand))
                        builder.Font.Bold = False
                        For Each Brands As DataRow In dsRecipeDetails.Tables("table8").Rows
                            strRecipeBrand = fctCheckDbNull(Brands.Item("BrandName"))
                            strRecipeBrandClassification = fctCheckDbNull(Brands.Item("BrandClassification"))

                            If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Or clsLicense.l_App = EgswKey.clsLicense.enumApp.RB Then
                                insertNewRowCenter(builder, recipeBrandTable, strRecipeBrand.ToString & " - " & strRecipeBrandClassification.ToString)
                            Else
                                insertNewRowCenter(builder, recipeBrandTable, strRecipeBrand.ToString)
                            End If
                        Next
                    End If
                End If
                builder.EndTable()
                'Attributes
                Dim attributesTable As Table = builder.StartTable()
                If G_ExportOptions.blnExpAdvIncludeKeywords Then
                    If dsRecipeDetails.Tables("table7").Rows.Count > 0 Then

                        builder.Font.Bold = True
                        insertNewRowCenter(builder, attributesTable, cLang.GetString(clsEGSLanguage.CodeType.Keywords))
                        builder.Font.Bold = False

                        For Each drKeywords As DataRow In dsRecipeDetails.Tables("Table7").Rows
                            insertNewRowCenter(builder, attributesTable, drKeywords("Name"))
                        Next

                    End If
                End If
                builder.EndTable()
                ' Cookbooks
                Dim cookbookTable As Table = builder.StartTable()
                If G_ExportOptions.blnExpAdvIncludeCookbook Then
                    If dsRecipeDetails.Tables(10).Rows.Count > 0 Then

                        builder.Font.Bold = True
                        insertNewRowCenter(builder, cookbookTable, cLang.GetString(clsEGSLanguage.CodeType.Cookbook))
                        builder.Font.Bold = False

                        For Each rwCookbooks As DataRow In dsRecipeDetails.Tables(10).Rows
                            insertNewRowCenter(builder, cookbookTable, rwCookbooks("Name").ToString)
                        Next

                    End If
                End If
                builder.EndTable()
                'Placements
                Dim placementTable As Table = builder.StartTable()
                If G_ExportOptions.blnExpAdvIncludePublication Then
                    If dsRecipeDetails.Tables("table9").Rows.Count > 0 Then

                        builder.Font.Bold = True
                        insertNewRowCenter(builder, placementTable, lblPlacements.ToString)
                        builder.Font.Bold = False


                        For Each Placements As DataRow In dsRecipeDetails.Tables("table9").Rows
                            strPlacementName = fctCheckDbNull(Placements.Item("Placement"))
                            If Not IsDBNull(Placements.Item("PlacementDate")) Then strPlacementDate = CDate(Placements.Item("PlacementDate")).ToString("MM/dd/yyyy")
                            strPlacementDescription = fctCheckDbNull(Placements.Item("Description"))

                            insertPlacement(builder, placementTable, strPlacementName.ToString, strPlacementDate.ToString, strPlacementDescription.ToString)
                        Next

                    End If

                End If
                builder.EndTable()
                'Comments
                Dim commentTable As Table = builder.StartTable()
                If G_ExportOptions.blnExpAdvIncludeComments Then
                    If dsRecipeDetails.Tables("table6").Rows.Count > 0 Then

                        builder.Font.Bold = True
                        insertNewRowCenter(builder, commentTable, cLang.GetString(clsEGSLanguage.CodeType.Comments))
                        builder.Font.Bold = False

                        For Each Comments As DataRow In dsRecipeDetails.Tables("table6").Rows
                            If Not IsDBNull(Comments.Item("SubmitDate")) Then strSubmitDate = CDate(Comments.Item("SubmitDate")).ToString("MM/dd/yyyy")
                            strOwnerName = fctCheckDbNull(Comments.Item("OwnerName"))
                            strComments = fctCheckDbNull(Comments.Item("Description"))

                            insertComment(builder, commentTable, strSubmitDate.ToString, strOwnerName.ToString, strComments.ToString)

                        Next
                    End If
                End If
                builder.EndTable()
            Else
                strHTMLContent.Append("<table style='width: 620'>")

                'Recipe Number
                If G_ExportOptions.blnExpIncludeRecipeNo Then
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td>")
                    strHTMLContent.Append("<table>")
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: FCalibri;'>")
                    strHTMLContent.Append(lblRecipeNumber.ToString)
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strRecipeNumber.ToString)
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                    strHTMLContent.Append("</table>")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td>")
                    strHTMLContent.Append("&nbsp;")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                End If

                'Sub Title
                If G_ExportOptions.blnExpIncludeSubName Then
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td>")
                    strHTMLContent.Append("<table>")
                    strHTMLContent.Append("<tr>")

                    strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(lblSubTitle.ToString)
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strSubTitle.ToString)
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                    strHTMLContent.Append("</table>")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")

                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td>")
                    strHTMLContent.Append("&nbsp;")
                    strHTMLContent.Append("</td>")
                    strHTMLContent.Append("</tr>")
                    strHTMLContent.Append("</table>")
                End If

                'Image
                imgRecipe = strImage
                strHTMLContent.Append("<p style='font-weight: bold; font-size: x-large; text-align: center; font-family: Calibri;'>")
                strHTMLContent.Append(imgRecipe) 'CMV 051911

                If Not strImage.Contains("nopic.jpg") And Not strImage2.Contains("nopic.jpg") Then
                    If Not strImage2 = "" Then
                        strHTMLContent.Append("&nbsp")
                        imgRecipe2 = strImage2 ' getHtml(imageRecipe) 'CMV 051911
                        strHTMLContent.Append(imgRecipe2) 'CMV 051911
                    End If
                End If

                strHTMLContent.Append("</p>")





                'Recipe Name
                strHTMLContent.Append("<p style='font-weight: bold; font-size: x-large; text-align: center; font-family: Calibri;'>")
                strHTMLContent.Append(strRecipeName.ToString)
                strHTMLContent.Append("</p>")

                'Subheading
                strHTMLContent.Append("<p style='font-weight: bold; text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                strHTMLContent.Append(strSubHeading.ToString)
                strHTMLContent.Append("</p>")

                'Servings
                If G_ExportOptions.blnExpIncludeYield1 Or G_ExportOptions.blnExpIncludeYield2 Or G_ExportOptions.blnExpSubRecipeWt Then
                    strHTMLContent.Append("<p style='text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strServings.ToString)
                    strHTMLContent.Append("</p>")
                End If


                'Recipe Time
                If G_ExportOptions.blnExpIncludeRecipeTime Then
                    strHTMLContent.Append("<p style='text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                    For Each RecipeTime As DataRow In dsRecipeDetails.Tables("Table5").Rows
                        strRecipeTime = RecipeTime.Item("Description")
                        strHTMLContent.Append(strRecipeTime.ToString)
                        strHTMLContent.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")

                    Next
                    strHTMLContent.Append("</p>")
                End If


                'Ingredients
                If dsRecipeDetails.Tables("Table1").Rows.Count > 0 Then
                    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")

                    For Each Ingredients As DataRow In dsRecipeDetails.Tables("Table1").Rows
                        If Ingredients.Item("Type").ToString().Trim() <> "4" Then

                            If bitQtyFormat = 0 Then

                                If fctCheckDbNull(Ingredients.Item("AlternativeIngredient")) = "" Then
                                    strIngredients = IIf(Ingredients.Item("Quantity_Metric") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Metric"))) & " " & _
                                        fctCheckDbNull(Ingredients.Item("UOM_Metric")) & " " & _
                                        fctCheckDbNull(Ingredients.Item("Complement") & " " & _
                                        fctCheckDbNull(Ingredients.Item("Name") & IIf(AutoSpacing = True, " ", "") & _
                                        fctCheckDbNull(Ingredients.Item("Preparation"))))
                                Else
                                    strIngredients = IIf(Ingredients.Item("Quantity_Metric") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Metric"))) & " " & _
                                        fctCheckDbNull(Ingredients.Item("UOM_Metric")) & " " & _
                                        fctCheckDbNull(Ingredients.Item("Complement") & " " & _
                                        fctCheckDbNull(Ingredients.Item("Name") & " &#91or " & _
                                        fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(AutoSpacing = True, " ", "") & _
                                        fctCheckDbNull(Ingredients.Item("Preparation")))))
                                End If

                                strIngredients = strIngredients.Replace("N/A", "")
                                strIngredients = strIngredients.Replace("n/a", "")
                                strIngredients = strIngredients + "<br>"

                            ElseIf bitQtyFormat = 1 Then
                                If IsNumeric(Ingredients.Item("Quantity_Imperial")) = True Then
                                    If fctCheckDbNull(Ingredients.Item("AlternativeIngredient")) = "" Then
                                        If blnUseFractions Then
                                            strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctConvertToFraction2(fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial")))) & " " & _
                                                fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " & _
                                                fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & IIf(AutoSpacing = True, " ", "") & fctCheckDbNull(Ingredients.Item("Preparation"))))
                                        Else
                                            strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial"))) & " " & _
                                                fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " & _
                                                fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & IIf(AutoSpacing = True, " ", "") & fctCheckDbNull(Ingredients.Item("Preparation"))))
                                        End If
                                    Else
                                        If blnUseFractions Then
                                            strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctConvertToFraction2(fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial")))) & " " & _
                                                fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " & _
                                                fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & " &#91or " & fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(AutoSpacing = True, " ", "") & fctCheckDbNull(Ingredients.Item("Preparation"))))) 'TDQ 10142011
                                        Else
                                            strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial"))) & " " & _
                                                fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " & _
                                                fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & " &#91or " & fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(AutoSpacing = True, " ", "") & fctCheckDbNull(Ingredients.Item("Preparation"))))) 'TDQ 10142011
                                        End If



                                    End If
                                    strIngredients = strIngredients.Replace("N/A", "")
                                    strIngredients = strIngredients.Replace("n/a", "")
                                Else
                                    If fctCheckDbNull(Ingredients.Item("AlternativeIngredient")) = "" Then
                                        strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNull(Ingredients.Item("Quantity_Imperial"))) & " " & _
                                            fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " &
                                            fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & IIf(AutoSpacing = True, " ", "") & fctCheckDbNull(Ingredients.Item("Preparation"))))
                                    Else
                                        strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNull(Ingredients.Item("Quantity_Imperial"))) & " " & _
                                            fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " &
                                            fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & " &#91or " & fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(AutoSpacing = True, " ", "") & fctCheckDbNull(Ingredients.Item("Preparation"))))) 'TDQ 10142011
                                    End If
                                    strIngredients = strIngredients.Replace("N/A", "")
                                    strIngredients = strIngredients.Replace("n/a", "")
                                End If

                                strIngredients = strIngredients + "<br>"

                            ElseIf bitQtyFormat = 2 Then
                                strIngredients = fctCheckDbNull(Ingredients.Item("Description"))
                            ElseIf bitQtyFormat = 3 Then
                                Dim strM As String = IIf(Ingredients.Item("Quantity_Metric") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Metric"))) & " " & fctCheckDbNull(Ingredients.Item("UOM_Metric")) & " "
                                strM = strM.Replace("N/A", "")
                                strM = strM.Replace("n/a", "")

                                Dim strI As String = ""
                                If IsNumeric(Ingredients.Item("Quantity_Imperial")) = True Then
                                    If blnUseFractions Then
                                        strI = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctConvertToFraction2(fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial")))) & " " & fctCheckDbNull(Ingredients.Item("UOM_Imperial"))
                                    Else
                                        strI = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial"))) & " " & fctCheckDbNull(Ingredients.Item("UOM_Imperial"))
                                    End If

                                Else
                                    strI = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNull(Ingredients.Item("Quantity_Imperial"))) & " " & fctCheckDbNull(Ingredients.Item("UOM_Imperial"))
                                End If

                                strI = strI.Replace("N/A", "")
                                strI = strI.Replace("n/a", "")

                                Dim strIngName As String

                                If fctCheckDbNull(Ingredients.Item("AlternativeIngredient")) = "" Then
                                    strIngName = fctCheckDbNull(Ingredients.Item("Complement") & " " & _
                                        fctCheckDbNull(Ingredients.Item("Name") & IIf(AutoSpacing = True, " ", "") & _
                                        fctCheckDbNull(Ingredients.Item("Preparation"))))
                                Else
                                    strIngName = fctCheckDbNull(Ingredients.Item("Complement") & " " & _
                                        fctCheckDbNull(Ingredients.Item("Name") & " &#91or " & _
                                        fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(AutoSpacing = True, " ", "") & _
                                        fctCheckDbNull(Ingredients.Item("Preparation")))))
                                End If

                                Dim strTempTemp As String = "<table  style='font-size: 11.5pt; font-family: Calibri;'><tr><td style='width: 100' valign='top'>%M</td><td style='width: 100' valign='top'>%I</td><td valign='top'>%N</td></tr></table>"
                                strIngredients = strTempTemp.Replace("%M", strM).Replace("%I", strI).Replace("%N", strIngName)
                            End If
                        Else
                            If bitQtyFormat = 0 Then
                                strIngredients = fctCheckDbNull(Ingredients.Item("Name")) & "<br/>"
                            ElseIf bitQtyFormat = 1 Then
                                strIngredients = fctCheckDbNull(Ingredients.Item("Name")) & "<br/>"
                            ElseIf bitQtyFormat = 2 Then
                                strIngredients = fctCheckDbNull(Ingredients.Item("Description"))
                            Else
                                Dim strTempTemp As String = "<table  style='font-size: 11.5pt; font-family: Calibri;'><tr><td style='width: 100' valign='top'>&nbsp</td><td style='width: 100' valign='top'>&nbsp</td><td valign='top'>%N</td></tr></table>"
                                strIngredients = strTempTemp.Replace("%N", fctCheckDbNull(Ingredients.Item("Name")))
                            End If
                        End If
                        strHTMLContent.Append(strIngredients.ToString)
                    Next

                    strHTMLContent.Append("</p>")
                End If

                'Method Header
                If strMethodHeader.ToString <> "" Then
                    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri; font-weight: bold' align='center'>")
                    strHTMLContent.Append(strMethodHeader.ToString)
                    strHTMLContent.Append("</p>")
                End If

                'Directions
                If strDirections.ToString <> "" Then
                    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strDirections.ToString)
                    strHTMLContent.Append("</p>")
                End If

                'Footnote 1
                If G_ExportOptions.blnExpIncludeNotes Then
                    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strFootNote1.ToString.Replace(Chr(13) & Chr(10), "<br>"))
                    strHTMLContent.Append("</p>")
                End If



                'Footnote 2
                If G_ExportOptions.blnExpIncludeAddNotes Then
                    strHTMLContent.Append("<table style='width: 620'>")
                    strHTMLContent.Append("<tr>")
                    strHTMLContent.Append("<td colspan='2' style='text-align: center; font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(lblRecipeAddNote.ToString)
                    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")
                    strHTMLContent.Append(strFootNote2.ToString.Replace(Chr(13) & Chr(10), "<br>"))
                    strHTMLContent.Append("</p>")
                End If

                'Nutrients
                If isDisplay = True Then
                    If dsRecipeDetails.Tables("Table4").Rows.Count > 0 Then
                        strHTMLContent.Append("<p style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: left;'>")

                        Dim strNutBasis As String = fctCheckDbNull(dsRecipeDetails.Tables("Table4").Rows(0).Item("NutritionBasis"))
                        If lblNutritionalInformation.ToString().Trim <> "" Then '-- JBB 02.23.2012
                            If strNutBasis = "" Then
                                strHTMLContent.Append(lblNutritionalInformation.ToString & " :")
                            Else
                                strHTMLContent.Append(lblNutritionalInformation.ToString & "(" & strNutBasis & ") :")
                            End If
                        End If

                        strHTMLContent.Append("</p>")
                        strHTMLContent.Append(strNutrients.ToString)
                    End If

                    'Net Carbs
                    If strNetCarbohydrates.ToString <> "" Then
                        strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
                        strHTMLContent.Append(strNetCarbohydrates.ToString)
                        strHTMLContent.Append("</p>")
                        strHTMLContent.Append(lblNetCarbs.ToString)
                    End If
                End If
            End If


            strHTMLContent.Append("</div></body></html>")
            strErr = ""
        Else
            strErr = cLang.GetString(clsEGSLanguage.CodeType.FileNotFound)
        End If

        Return doc

    End Function

    Public Function AsposeMultipleExportToWord(ByVal dtRecipes As DataTable, ByVal bitFormat As Byte, ByVal intCodeTrans As Integer, ByRef strErr As String, ByRef strFilename As String, _
     ByVal bl2PicGoldOnly As Boolean, ByVal blChckBoxPic2 As Boolean, ByVal intCodeLang As Integer, blnUseFractions As Boolean, _
     Optional ByVal bitQtyFormat As Byte = 0, Optional ByVal blCookMode As Boolean = False, Optional ByVal intCodeSet As Integer = 0, Optional udtListeType As enumDataListItemType = enumDataListItemType.Recipe, _
     Optional ByVal intLangFromCodeDictionary As Integer = 1, Optional intCodeSite As Integer = 0, Optional blnRemoveTrailingZeroes As Boolean = False) As Document

        Dim doc As New Document
        Dim builder As DocumentBuilder = New DocumentBuilder(doc)

        Dim strHTMLContent As StringBuilder = New StringBuilder()
        Dim dsRecipeDetails As DataSet

        Dim lblRecipeID As String = ""
        Dim lblRecipeNumber As String = ""
        Dim lblSubTitle As String = ""
        Dim lblRecipeDescription As String = ""
        Dim lblRecipeRemark As String = ""
        Dim lblYield1 As String = ""
        Dim lblYield2 As String = ""
        Dim lblWeight As String = ""
        Dim lblCostPerRecipe As String = ""
        Dim lblCostPerServings As String = ""
        Dim lblInformation As String = ""
        Dim lblRecipeStatus As String = ""
        Dim lblWebStatus As String = ""
        Dim lblDateCreated As String = ""
        Dim lblDateLastModified As String = ""
        Dim lblLastTested As String = ""
        Dim lblDateDeveloped As String = ""
        Dim lblDateOfFinalEdit As String = ""
        Dim lblDevelopmentPurpose As String = ""
        Dim lblUpdatedBy As String = ""
        Dim lblCreatedBy As String = ""
        Dim lblModifiedBy As String = ""
        Dim lblTestedBy As String = ""
        Dim lblDevelopedBy As String = ""
        Dim lblFinalEditBy As String = ""
        Dim lblComments As String = ""
        Dim lblAttributes As String = ""
        Dim lblRecipeBrand As String = ""
        Dim lblPlacements As String = ""
        Dim lblNutritionalInformation As String = ""
        Dim lblCalories As String = ""
        Dim lblCaloriesFromFat As String = ""
        Dim lblSatFat As String = ""
        Dim lblTransFat As String = ""
        Dim lblMonoSatFat As String = ""
        Dim lblPolyFat As String = ""
        Dim lblTotalFat As String = ""
        Dim lblCholesterol As String = ""
        Dim lblSodium As String = ""
        Dim lblTotalCarbohydrates As String = ""
        Dim lblSugars As String = ""
        Dim lblDietaryFiber As String = ""
        Dim lblNetCarbohydrates As String = ""
        Dim lblProtein As String = ""
        Dim lblVitaminA As String = ""
        Dim lblVitaminC As String = ""
        Dim lblCalcium As String = ""
        Dim lblIron As String = ""
        Dim lblMonoUnsaturated As String = ""
        Dim lblPolyUnsaturated As String = ""
        Dim lblPotassium As String = ""
        Dim lblVitaminD As String = ""
        Dim lblVitaminE As String = ""
        Dim lblOmega3 As String = ""
        Dim lblNetCarbs As String = ""
        Dim lblThiamin As String = ""
        Dim lblRiboflavin As String = ""
        Dim lblNiacin As String = ""
        Dim lblVitaminB6 As String = ""
        Dim lblFolate As String = ""
        Dim lblVitaminB12 As String = ""
        Dim lblBiotin As String = ""
        Dim lblPantothenicAcid As String = ""
        Dim lblPhosphorus As String = ""
        Dim lblIodine As String = ""
        Dim lblMagnesium As String = ""
        Dim lblZinc As String = ""
        Dim lblManganese As String = ""

        Dim strRecipeID As String = ""
        Dim strRecipeNumber As String = ""
        Dim strSubTitle As String = ""
        Dim strRecipeDescription As String = ""
        Dim strRecipeRemark As String = ""
        Dim strWeight As String = ""
        Dim strWeightQty As String = ""
        Dim strImagePath As String = ""
        Dim strRecipeName As String = ""
        Dim strSubHeading As String = ""
        Dim strServings As String = ""
        Dim strYield As String = ""
        Dim strYield2 As String = ""
        Dim strServingsUnit As String = ""
        Dim strRecipeTime As String = ""
        Dim strMethodHeader As String = ""
        Dim strIngredients As String = ""
        Dim strUOM As String = ""
        Dim strDirections As String = ""
        Dim strAbbrDirections As String = ""
        Dim strFootNote1 As String = ""
        Dim strFootNote2 As String = ""
        Dim strCostPerRecipe As String = ""
        Dim strCostPerServings As String = ""
        Dim strCurrency As String = ""
        Dim strRecipeStatus As String = ""
        Dim strWebStatus As String = ""
        Dim strDateCreated As String = ""
        Dim strDateLastModified As String = ""
        Dim strLastTested As String = ""
        Dim strDateDeveloped As String = ""
        Dim strDateOfFinalEdit As String = ""
        Dim strDevelopmentPurpose As String = ""
        Dim strUpdatedBy As String = ""
        Dim strCreatedBy As String = ""
        Dim strModifiedBy As String = ""
        Dim strTestedBy As String = ""
        Dim strDevelopedBy As String = ""
        Dim strFinalEditBy As String = ""
        Dim strSubmitDate As String = ""
        Dim strOwnerName As String = ""
        Dim strComments As String = ""
        Dim strAttributes As String = ""
        Dim strParents As String = ""
        Dim strRecipeBrand As String = ""
        Dim strRecipeBrandClassification As String = ""
        Dim strPlacementName As String = ""
        Dim strPlacementDate As String = ""
        Dim strPlacementDescription As String = ""
        Dim strCalories As String = ""
        Dim strCaloriesFromFat As String = ""
        Dim strSatFat As String = ""
        Dim strTransFat As String = ""
        Dim strMonoSatFat As String = ""
        Dim strPolyFat As String = ""
        Dim strTotalFat As String = ""
        Dim strCholesterol As String = ""
        Dim strSodium As String = ""
        Dim strTotalCarbohydrates As String = ""
        Dim strSugars As String = ""
        Dim strDietaryFiber As String = ""
        Dim strNetCarbohydrates As String = ""
        Dim strProtein As String = ""
        Dim strVitaminA As String = ""
        Dim strVitaminC As String = ""
        Dim strCalcium As String = ""
        Dim strIron As String = ""
        Dim strMonoUnsaturated As String = ""
        Dim strPolyUnsaturated As String = ""
        Dim strPotassium As String = ""
        Dim strVitaminD As String = ""
        Dim strVitaminE As String = ""
        Dim strOmega3 As String = ""
        Dim strUnitCalories As String = ""
        Dim strUnitCaloriesFromFat As String = ""
        Dim strUnitSatFat As String = ""
        Dim strUnitTransFat As String = ""
        Dim strUnitMonoSatFat As String = ""
        Dim strUnitPolyFat As String = ""
        Dim strUnitTotalFat As String = ""
        Dim strUnitCholesterol As String = ""
        Dim strUnitSodium As String = ""
        Dim strUnitTotalCarbohydrates As String = ""
        Dim strUnitSugars As String = ""
        Dim strUnitDietaryFiber As String = ""
        Dim strUnitNetCarbohydrates As String = ""
        Dim strUnitProtein As String = ""
        Dim strUnitVitaminA As String = ""
        Dim strUnitVitaminC As String = ""
        Dim strUnitCalcium As String = ""
        Dim strUnitIron As String = ""
        Dim strUnitMonoUnsaturated As String = ""
        Dim strUnitPolyUnsaturated As String = ""
        Dim strUnitPotassium As String = ""
        Dim strUnitVitaminD As String = ""
        Dim strUnitVitaminE As String = ""
        Dim strUnitOmega3 As String = ""
        Dim strFormatCalories As String = ""
        Dim strFormatCaloriesFromFat As String = ""
        Dim strFormatSatFat As String = ""
        Dim strFormatTransFat As String = ""
        Dim strFormatMonoSatFat As String = ""
        Dim strFormatPolyFat As String = ""
        Dim strFormatTotalFat As String = ""
        Dim strFormatCholesterol As String = ""
        Dim strFormatSodium As String = ""
        Dim strFormatTotalCarbohydrates As String = ""
        Dim strFormatSugars As String = ""
        Dim strFormatDietaryFiber As String = ""
        Dim strFormatNetCarbohydrates As String = ""
        Dim strFormatProtein As String = ""
        Dim strFormatVitaminA As String = ""
        Dim strFormatVitaminC As String = ""
        Dim strFormatCalcium As String = ""
        Dim strFormatIron As String = ""
        Dim strFormatMonoUnsaturated As String = ""
        Dim strFormatPolyUnsaturated As String = ""
        Dim strFormatPotassium As String = ""
        Dim strFormatVitaminD As String = ""
        Dim strFormatVitaminE As String = ""
        Dim strFormatOmega3 As String = ""
        Dim strNutrients As String = ""
        Dim strFolderImagePath As String = ""
        Dim imgRecipe As String = ""
        Dim imgRecipe2 As String = ""
        Dim strThiamin As String = ""
        Dim strFormatThiamin As String = ""
        Dim strUnitThiamin As String = ""
        Dim strRiboflavin As String = ""
        Dim strFormatRiboflavin As String = ""
        Dim strUnitRiboflavin As String = ""
        Dim strNiacin As String = ""
        Dim strFormatNiacin As String = ""
        Dim strUnitNiacin As String = ""
        Dim strVitaminB6 As String = ""
        Dim strFormatVitaminB6 As String = ""
        Dim strUnitVitaminB6 As String = ""
        Dim strFolate As String = ""
        Dim strFormatFolate As String = ""
        Dim strUnitFolate As String = ""
        Dim strVitaminB12 As String = ""
        Dim strFormatVitaminB12 As String = ""
        Dim strUnitVitaminB12 As String = ""
        Dim strBiotin As String = ""
        Dim strFormatBiotin As String = ""
        Dim strUnitBiotin As String = ""
        Dim strPantothenicAcid As String = ""
        Dim strFormatPantothenicAcid As String = ""
        Dim strUnitPantothenicAcid As String = ""
        Dim strPhosphorus As String = ""
        Dim strFormatPhosphorus As String = ""
        Dim strUnitPhosphorus As String = ""
        Dim strIodine As String = ""
        Dim strFormatIodine As String = ""
        Dim strUnitIodine As String = ""
        Dim strMagnesium As String = ""
        Dim strFormatMagnesium As String = ""
        Dim strUnitMagnesium As String = ""
        Dim strZinc As String = ""
        Dim strFormatZinc As String = ""
        Dim strUnitZinc As String = ""
        Dim strManganese As String = ""
        Dim strFormatManganese As String = ""
        Dim strUnitManganese As String = ""

        Dim intAttributesCode As Integer
        Dim intAttributesParent As Integer
        Dim intAttributesMain As Integer
        Dim dblQty As Double

        Dim isDisplay As Boolean = False
        Dim blnIncludeCostPerRecipe As Boolean = False 'IIf(bitFormat = 1, True, False)
        Dim blnIncludeCostPerServings As Boolean = False 'IIf(bitFormat = 1, True, False)
        Dim blnIncludeInformation As Boolean = False 'IIf(bitFormat = 1, True, False)
        Dim blnIncludeComment As Boolean = False 'IIf(bitFormat = 1, True, False)
        Dim blnIncludeKeyword As Boolean = False 'IIf(bitFormat = 1, True, False)
        Dim blnIncludeBrand As Boolean = False 'IIf(bitFormat = 1, True, False)
        Dim blnIncludePublication As Boolean = False 'IIf(bitFormat = 1, True, False)

        Dim cLang As New clsEGSLanguage(intLangFromCodeDictionary) 'intCodeLang to intLangFromCodeDictionary

        lblRecipeID = cLang.GetString(clsEGSLanguage.CodeType.RecipeID)
        lblRecipeNumber = cLang.GetString(clsEGSLanguage.CodeType.RecipeNumber) 'lblRecipeNumber = "Recipe Number"

        Dim clsLicense As New clsLicense
        If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
            lblSubTitle = cLang.GetString(clsEGSLanguage.CodeType.SubTitle) '"SubTitle"
        Else
            lblSubTitle = cLang.GetString(clsEGSLanguage.CodeType.SubName)
        End If
        lblCostPerRecipe = cLang.GetString(clsEGSLanguage.CodeType.CostForTotalServings) & ":" 'lblCostPerRecipe = "Cost Per Recipe"
        lblCostPerServings = cLang.GetString(clsEGSLanguage.CodeType.CostForServing) & ":" 'lblCostPerServings = "Cost Per Serving"
        lblInformation = cLang.GetString(clsEGSLanguage.CodeType.Information) & ":" 'lblInformation = "Information"
        lblRecipeStatus = cLang.GetString(clsEGSLanguage.CodeType.RecipeStatus) & ":" 'lblRecipeStatus = "Recipe Status:"
        lblUpdatedBy = cLang.GetString(clsEGSLanguage.CodeType.UpdatedBy) & ":"
        lblWebStatus = cLang.GetString(clsEGSLanguage.CodeType.WebStatus) & ":" 'lblWebStatus = "Web Status:"
        lblDateCreated = cLang.GetString(clsEGSLanguage.CodeType.DateCreated) & ":" 'lblDateCreated = "Date Created:"
        lblCreatedBy = cLang.GetString(clsEGSLanguage.CodeType.CreatedBY) & ":" 'lblCreatedBy = "Created By:"
        lblDateLastModified = cLang.GetString(clsEGSLanguage.CodeType.DateLastModified) & ":" 'lblDateLastModified = "Date Last Modified:"
        lblModifiedBy = cLang.GetString(clsEGSLanguage.CodeType.ModifiedBy) & ":"
        lblLastTested = cLang.GetString(clsEGSLanguage.CodeType.DateLastTested) & ":"
        lblTestedBy = cLang.GetString(clsEGSLanguage.CodeType.TestedBy) & ":"
        lblDateDeveloped = cLang.GetString(clsEGSLanguage.CodeType.DateDeveloped) & ":"
        lblDevelopedBy = cLang.GetString(clsEGSLanguage.CodeType.DevelopedBy) & ":"
        lblDateOfFinalEdit = cLang.GetString(clsEGSLanguage.CodeType.FinalEditDate) & ":"
        lblFinalEditBy = cLang.GetString(clsEGSLanguage.CodeType.FinalEditBy) & ":"
        lblDevelopmentPurpose = cLang.GetString(clsEGSLanguage.CodeType.DevelopmentPurpose) & ":"
        lblComments = cLang.GetString(clsEGSLanguage.CodeType.Comments) '"Comments"
        lblAttributes = cLang.GetString(clsEGSLanguage.CodeType.Attributes)
        lblRecipeBrand = cLang.GetString(clsEGSLanguage.CodeType.Brand) '"Brands"
        'AGL 2013.03.16 
        If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
            lblPlacements = cLang.GetString(clsEGSLanguage.CodeType.RecipePlacements)
        Else
            lblPlacements = cLang.GetString(clsEGSLanguage.CodeType.Publication) '"Placements"
        End If

        lblNutritionalInformation = cLang.GetString(clsEGSLanguage.CodeType.NutritionalInfo) & " " & cLang.GetString(clsEGSLanguage.CodeType.per_serving)
        lblCalories = cLang.GetString(clsEGSLanguage.CodeType.Calories)
        lblCaloriesFromFat = cLang.GetString(clsEGSLanguage.CodeType.CaloriesfromFat)
        lblSatFat = "Sat Fat"
        lblTransFat = "Trans Fat"
        lblMonoSatFat = "Mono Sat Fat"
        lblPolyFat = "Poly Sat Fat"
        lblTotalFat = "Total Fat"
        lblCholesterol = "Cholesterol"
        lblSodium = "Sodium"
        lblTotalCarbohydrates = "Total Carbohydrates"
        lblSugars = "Sugars"
        lblDietaryFiber = "Dietary Fiber"
        lblNetCarbohydrates = "Net Carbohydrates"
        lblProtein = "Protein"
        lblVitaminA = "Vitamin A"
        lblVitaminC = "Vitamin C"
        lblCalcium = "Calcium"
        lblIron = "Iron"
        lblMonoUnsaturated = "Mono Unsaturated"
        lblPolyUnsaturated = "Poly Unsaturated"
        lblPotassium = "Potassium"
        lblVitaminD = "Vitamin D"
        lblVitaminE = "Vitamin E"
        lblNetCarbs = "* " & """Net Carbs""" & " are total carbohydrates minus dietary fiber and sugar alcohol as these have a minimal impact on blood sugar."
        lblThiamin = "Thiamin"
        lblRiboflavin = "Riboflavin"
        lblNiacin = "Niacin"
        lblVitaminB6 = "VitaminB6"
        lblFolate = "Folate"
        lblVitaminB12 = "VitaminB12"
        lblBiotin = "Biotin"
        lblPantothenicAcid = "Pantothenic_Acid"
        lblPhosphorus = "Phosphorus"
        lblIodine = "Iodine"
        lblMagnesium = "Magnesium"
        lblZinc = "Zinc"
        lblManganese = "Manganese"
        lblOmega3 = "Omega-3"

        lblRecipeDescription = cLang.GetString(clsEGSLanguage.CodeType.Description) '"Description"
        lblRecipeRemark = cLang.GetString(clsEGSLanguage.CodeType.Remark) '"Remark"
        lblYield1 = cLang.GetString(clsEGSLanguage.CodeType.Yield) & " 1:" '"Yield 1: "
        lblYield2 = cLang.GetString(clsEGSLanguage.CodeType.Yield) & " 2:" '"Yield 2: "
        lblWeight = cLang.GetString(clsEGSLanguage.CodeType.Weight) & "(" & cLang.GetString(clsEGSLanguage.CodeType.Sub_Recipe) & "):" '"Weight(Subrecipe): "

        Dim rowCount As Integer = dtRecipes.Rows.Count
        Dim x As Integer
        Dim intListeID As Integer
        Dim strImage As String
        Dim intStandard As Integer

        For x = 0 To rowCount - 1

            intListeID = dtRecipes.Rows(x).Item("CodeListe").ToString
            strImage = dtRecipes.Rows(x).Item("Image1Link").ToString

            GetRecipeCode(intListeID, m_RecipeId, m_Version)

            ' RDC 01.13.2014 : Code Site handler
            intCodeSite = getRecipeSiteOwner(m_RecipeId, m_Version)

            'AGL 2012.10.12 - CWM-1634 - added branch for merchandise
            'dsRecipeDetails = GetRecipeDetails(m_RecipeId, m_Version, intCodeTrans, 0, intCodeSet) 'CMV 051911
            If udtListeType = enumDataListItemType.Merchandise Then
                dsRecipeDetails = GetMerchandiseDetails(m_RecipeId, m_Version, intCodeTrans, 0, intCodeSet)
            Else
                dsRecipeDetails = GetRecipeDetails(m_RecipeId, m_Version, intCodeTrans, 0, intCodeSet, , intCodeSite) 'CMV 051911
            End If

            lblNutritionalInformation = ""

            If dsRecipeDetails IsNot Nothing Then
                If dsRecipeDetails.Tables("table1").Rows.Count > 0 Then
                    'SET VALUES
                    strRecipeID = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("RecipeID"))
                    strRecipeNumber = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("Number"))
                    strSubTitle = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("SubTitle"))

                    'JTOC 10.29.2013
                    '----------------------------------------------------------------------------------------------------
                    strRecipeDescription = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Description"))
                    strRecipeRemark = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Remark"))
                    strWeight = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("Weight"))
                    strWeightQty = fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("SrQty"))
                    '--------------------------------------------------

                    strRecipeName = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("Name"))
                    strSubHeading = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("SubHeading"))
                    ''strImagePath = Server.MapPath("Images/test.jpg") 'CMV 051911

                    ''Dim imageRecipe As New System.Web.UI.WebControls.Image 'CMV 051911
                    ''With imageRecipe
                    ''    .ID = "Image1"
                    ''    .Height = 240
                    ''    .Width = 240
                    ''    .ImageUrl = "Images/test.jpg"
                    ''End With

                    ' RDC 12.12.2013 : Discarded on top variables in displaying yield/subrecipe wt.
                    Dim decYield1 As Decimal = 0D, _
                     decYield2 As Decimal = 0D, _
                     decSrWt As Decimal = 0D
                    Dim strYield1Unit As String = dsRecipeDetails.Tables(11).Rows(0).Item("Yield1Unit").ToString.Replace("[_]", "").Replace("n/a", "").Replace("_", " ").ToLower, _
                     strYield2Unit As String = dsRecipeDetails.Tables(11).Rows(0).Item("Yield2Unit").ToString.Replace("[_]", "").Replace("n/a", "").Replace("_", " ").ToLower, _
                     strSrWtUnit As String = dsRecipeDetails.Tables(11).Rows(0).Item("SrUnit").ToString.Replace("[_]", "").Replace("n/a", "").Replace("_", " ").ToLower

                    If Not IsDBNull(dsRecipeDetails.Tables(11).Rows(0).Item("Yield1")) Then decYield1 = CDec(dsRecipeDetails.Tables(11).Rows(0).Item("Yield1"))
                    If Not IsDBNull(dsRecipeDetails.Tables(11).Rows(0).Item("Yield2")) Then decYield2 = CDec(dsRecipeDetails.Tables(11).Rows(0).Item("Yield2"))
                    If Not IsDBNull(dsRecipeDetails.Tables(11).Rows(0).Item("SubRecipeQty")) Then decSrWt = CDec(dsRecipeDetails.Tables(11).Rows(0).Item("SubRecipeQty"))

                    If decYield1 > 0 And Not strYield1Unit = "[_]" And Not strYield1Unit.ToLower = "n/a" And Not strYield1Unit.Trim.Length = 0 And Not strYield1Unit.EndsWith("s") And Not strYield1Unit.ToLower.Trim = "g" Then
                        If decYield1 > 1 Then
                            If strYield1Unit.Trim.Length > 0 And Char.IsLetter(Mid(strYield1Unit, strYield1Unit.Length, 1)) Then strYield1Unit &= "s"
                        End If

                    End If

                    If decYield2 > 0 And Not strYield2Unit = "[_]" And Not strYield2Unit.ToLower = "n/a" And Not strYield2Unit.Trim.Length = 0 And Not strYield2Unit.EndsWith("s") And Not strYield2Unit.ToLower.Trim = "g" Then
                        If decYield2 > 1 Then
                            If strYield2Unit.Trim.Length > 0 And Char.IsLetter(Mid(strYield2Unit, strYield2Unit.Length, 1)) Then strYield2Unit &= "s"
                        End If
                    End If

                    If CDec(Format(decSrWt, "#.000#")) > 0 And Not strSrWtUnit = "[_]" And Not strSrWtUnit.ToLower = "n/a" And Not strSrWtUnit.Trim.Length = 0 And Not strSrWtUnit.EndsWith("s") And Not strSrWtUnit.ToLower.Trim = "g" Then
                        If decSrWt > 1 Then
                            If strSrWtUnit.Trim.Length > 0 And Char.IsLetter(Mid(strSrWtUnit, strSrWtUnit.Length, 1)) Then strSrWtUnit &= "s"
                        End If
                    End If

                    Dim strYield1 As String = ConvertDecimalToFraction2(fctCheckDbNullNumeric(decYield1)), _
                        strSrWt As String = ConvertDecimalToFraction2(fctCheckDbNullNumeric(decSrWt))

                    Dim intFieldsToDisplay As Integer = 0, intFieldWidth As Integer = 0, intTableWidth As Integer = 620
                    If G_ExportOptions.blnExpIncludeYield1 And decYield1 > 0 Then intFieldsToDisplay += 1
                    If G_ExportOptions.blnExpIncludeYield2 And decYield2 > 0 Then intFieldsToDisplay += 1
                    If G_ExportOptions.blnExpSubRecipeWt And decSrWt > 0 Then intFieldsToDisplay += 1

                    Select Case intFieldsToDisplay
                        Case 1
                            intFieldWidth = 620
                            intTableWidth = 250
                        Case 2
                            intFieldWidth = 310
                            intTableWidth = 400
                        Case 3
                            intFieldWidth = CInt(620 / 3)
                        Case Else
                            intFieldWidth = CInt(620 / 3)
                    End Select

                    strYield2 = ConvertDecimalToFraction2(fctCheckDbNullNumeric(decYield2))

                    Dim arrYieldsLabel(0 To 2) As String
                    Dim arrYields(0 To 2) As String

                    With G_ExportOptions
                        If .blnExpIncludeYield1 And decYield1 > 0 Then
                            arrYieldsLabel(0) = lblYield1.ToString
                            arrYields(0) = strYield1 & " " & strYield1Unit
                        End If
                        If .blnExpIncludeYield2 And decYield2 > 0 Then
                            arrYieldsLabel(1) = lblYield2.ToString
                            arrYields(1) = strYield2 & " " & strYield2Unit
                        End If
                        If .blnExpSubRecipeWt And decSrWt > 0 Then
                            arrYieldsLabel(2) = lblWeight.ToString
                            arrYields(2) = strSrWt & " " & strSrWtUnit
                        End If
                    End With

                    strMethodHeader = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("MethodHeader"))

                    If dsRecipeDetails.Tables("Table3").Rows.Count > 0 Then
                        strDirections = fctGetInstrunctions(dsRecipeDetails.Tables("Table3"), fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("MethodFormat")), blCookMode)
                        strAbbrDirections = fctGetInstrunctions(dsRecipeDetails.Tables("Table3"), fctCheckDbNull(dsRecipeDetails.Tables("Table1").Rows(0).Item("MethodFormat")), True)
                    Else
                        strAbbrDirections = ""
                        strDirections = ""
                    End If


                    strFootNote1 = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("FootNote1"))
                    strFootNote2 = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("FootNote2"))
                    strCurrency = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("Currency"))
                    strRecipeStatus = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("RecipeStatusName"))
                    strUpdatedBy = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("UpdatedBy"))
                    strWebStatus = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("WebStatusName"))
                    If Not IsDBNull(dsRecipeDetails.Tables("table1").Rows(0).Item("DateCreated")) Then strDateCreated = CDate(dsRecipeDetails.Tables("table1").Rows(0).Item("DateCreated")).ToString("MM/dd/yyyy")
                    strCreatedBy = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("CreatedBy"))
                    If Not IsDBNull(dsRecipeDetails.Tables("table1").Rows(0).Item("DateLastModified")) Then strDateLastModified = CDate(dsRecipeDetails.Tables("table1").Rows(0).Item("DateLastModified")).ToString("MM/dd/yyyy")
                    strModifiedBy = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("LastModifiedBy"))
                    If Not IsDBNull(dsRecipeDetails.Tables("table1").Rows(0).Item("DateTested")) Then strLastTested = CDate(dsRecipeDetails.Tables("table1").Rows(0).Item("DateTested")).ToString("MM/dd/yyyy")
                    strTestedBy = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("TestedBy"))
                    If Not IsDBNull(dsRecipeDetails.Tables("table1").Rows(0).Item("DateDeveloped")) Then strDateDeveloped = CDate(dsRecipeDetails.Tables("table1").Rows(0).Item("DateDeveloped")).ToString("MM/dd/yyyy")
                    strDevelopedBy = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("DevelopedBy"))
                    If Not IsDBNull(dsRecipeDetails.Tables("table1").Rows(0).Item("DateFinalEdit")) Then strDateOfFinalEdit = CDate(dsRecipeDetails.Tables("table1").Rows(0).Item("DateFinalEdit")).ToString("MM/dd/yyyy")
                    strFinalEditBy = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("FinalEditBy"))
                    strDevelopmentPurpose = fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("DevelopmentPurpose"))
                    isDisplay = CBoolDB(dsRecipeDetails.Tables("table1").Rows(0).Item("DisplayNutrition"))

                    Dim strHeaderNutrientServing As String = ""
                    Dim strHeader As String = fGetMethodFormat("nh")
                    Dim strItems As String = fGetMethodFormat("s")
                    Dim dicIsDisplay As New Dictionary(Of String, Boolean)
                    Dim dicColumnName As New Dictionary(Of String, String)
                    Dim dicUnit As New Dictionary(Of String, String)
                    Dim dicFormat As New Dictionary(Of String, String)
                    Dim intIndex As Integer = 0
                    Dim dtNutrients As DataTable = dsRecipeDetails.Tables("table4")
                    Dim strColCalories As String = "Calories"
                    If dtNutrients.Rows.Count > 0 Then
                        For Each dcNutrient As DataColumn In dtNutrients.Columns
                            Dim strColumn As String = dcNutrient.ColumnName
                            If strColumn.Trim.ToLower <> "recipeid" And strColumn.Trim.ToLower <> "version" And strColumn.Trim.ToLower <> "portionsize" Then
                                If strColumn.Contains("Display") Then
                                    dicIsDisplay.Add(strColumn.ToLower(), CBool(dtNutrients.Rows(intIndex)(strColumn)))
                                ElseIf strColumn.Contains("Unit_") Then
                                    dicUnit.Add(strColumn.ToLower(), CStr(dtNutrients.Rows(intIndex)(strColumn)))
                                ElseIf strColumn.Contains("Format") Then
                                    dicFormat.Add(strColumn.ToLower(), CStr(dtNutrients.Rows(intIndex)(strColumn)))
                                End If
                            End If
                            dicColumnName.Add(strColumn.ToLower(), strColumn)
                            If (Not strColumn.Contains("Display") Or Not strColumn.Contains("Unit_") Or Not strColumn.Contains("Format")) Then
                                strColCalories = strColumn
                            End If

                        Next

                        strHeaderNutrientServing = ""
                        If dsRecipeDetails.Tables("table4").Columns.Contains("PortionSize") = True Then
                            If dicColumnName.ContainsKey(strColCalories) Then
                                strHeaderNutrientServing = dsRecipeDetails.Tables("table1").Rows(0).Item("PortionSize").ToString.Trim
                            Else
                                If dsRecipeDetails.Tables("table4").Rows(0).Item(strColCalories).ToString.Trim <> "" Then
                                    strHeaderNutrientServing = dsRecipeDetails.Tables("table4").Rows(0).Item("PortionSize").ToString.Trim
                                Else
                                    If dsRecipeDetails.Tables("table1").Columns.Contains("Yield") = True Then
                                        strHeaderNutrientServing = dsRecipeDetails.Tables("table1").Rows(0).Item("Yield").ToString.Trim
                                    End If
                                End If
                            End If
                        Else
                            If dsRecipeDetails.Tables("table1").Columns.Contains("Yield") = True Then
                                strHeaderNutrientServing = dsRecipeDetails.Tables("table1").Rows(0).Item("Yield").ToString.Trim
                            End If
                        End If

                        Dim lstKey As List(Of String)
                        lstKey = New List(Of String)(dicIsDisplay.Keys)
                        For Each dcNutrient As DataColumn In dtNutrients.Columns
                            Dim strColumn As String = dcNutrient.ColumnName
                            If strColumn.Trim.ToLower <> "recipeid" And strColumn.Trim.ToLower <> "version" And strColumn.Trim.ToLower <> "portionsize" Then
                                If Not strColumn.Contains("Display") And Not strColumn.Contains("Unit_") And Not strColumn.Contains("Format") Then
                                    If lstKey.Contains(("Display" + strColumn.ToString()).ToLower) = True Then
                                        If dicIsDisplay(("Display" + strColumn.ToString()).ToLower) = True Then
                                            If dtNutrients.Rows(intIndex)(strColumn).ToString().Trim <> "-1" Then
                                                Dim strNutDisplayValue As String = Format(dicFormat((strColumn.ToString() + "Format").ToLower), IIf(dtNutrients.Rows(intIndex)(strColumn).ToString().Trim() <> "-1", dtNutrients.Rows(intIndex)(strColumn), 0))
                                                strNutrients = strNutrients & Replace(strColumn, "_", " ") & " " & strNutDisplayValue + dicUnit(("Unit_" + strColumn.ToString()).ToLower) & ", "
                                                lblNutritionalInformation = cLang.GetString(clsEGSLanguage.CodeType.NutritionalInfo) & " " & strHeaderNutrientServing & " "
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        Next

                        If Right(strNutrients, 2) = ", " Then strNutrients = strNutrients.Remove(Len(strNutrients) - 2, 2)

                    Else
                        lblNutritionalInformation = ""
                        strNutrients = ""
                        strHeaderNutrientServing = ""
                    End If

                    strDirections = fctGetInstrunctions(dsRecipeDetails.Tables("table3"), fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("MethodFormat")), blCookMode)
                    strAbbrDirections = fctGetInstrunctions(dsRecipeDetails.Tables("table3"), fctCheckDbNull(dsRecipeDetails.Tables("table1").Rows(0).Item("MethodFormat")), True) ' RJL - 11774 :02-17-2014 'fctGetInstrunctions(dsRecipeDetails.Tables("Table3"), strAbbrDirections, True)
                    strDirections = fctCheckDbNull(strDirections)
                    strAbbrDirections = fctCheckDbNull(strAbbrDirections)

                    If bitFormat = 1 Then

                        ' Recipe Name
                        insertRecipeTitle(builder, strRecipeName.ToString)

                        insertNewLine(builder, 1)

                        With G_ExportOptions
                            If .blnExpIncludeRecipeNo Or .blnExpIncludeSubName Or .blnExpIncludeItemDesc Or .blnExpIncludeRemark Then
                                Dim subTitleTable As Table = builder.StartTable()
                                ' Recipe Number
                                If G_ExportOptions.blnExpIncludeRecipeNo Then
                                    insertRecipeSubTitle(builder, subTitleTable, lblRecipeNumber.Trim, strRecipeNumber.Trim)
                                End If
                                ' Sub Title
                                If G_ExportOptions.blnExpIncludeSubName Then
                                    insertRecipeSubTitle(builder, subTitleTable, lblSubTitle.Trim, strSubTitle.Trim)
                                End If
                                ' Description
                                If G_ExportOptions.blnExpIncludeItemDesc And strRecipeDescription.Trim.Length > 0 Then
                                    insertRecipeSubTitle(builder, subTitleTable, lblRecipeDescription.Trim, strRecipeDescription.Trim)
                                End If
                                ' Remarks
                                If G_ExportOptions.blnExpIncludeRemark And strRecipeRemark.Trim.Length > 0 Then
                                    insertRecipeSubTitle(builder, subTitleTable, lblRecipeRemark.Trim, strRecipeRemark.Trim)
                                End If

                                builder.EndTable()
                            End If
                        End With

                        insertNewLine(builder, 3)

                        Dim strImage2 As String = dtRecipes.Rows(x).Item("Image2Link").ToString

                        If strImage <> "" And strImage2 <> "" Then
                            ' Recipe Image 1
                            imgRecipe = strImage
                            insertRecipeImage(builder, imgRecipe)
                            insertNewLine(builder, 1)
                            ' Recipe Image 2
                            If Not strImage.Contains("nopic.jpg") And Not strImage2.Contains("nopic.jpg") Then
                                If Not strImage2 = "" Then
                                    imgRecipe2 = strImage2
                                    insertRecipeImage(builder, imgRecipe2)
                                    insertNewLine(builder, 1)
                                End If
                            End If
                        Else
                            ' Recipe Image 1
                            imgRecipe = strImage
                            insertRecipeImage(builder, imgRecipe)
                            insertNewLine(builder, 1)
                        End If

                        'Subheading
                        If G_ExportOptions.blnExpIncludeSubName Then
                            insertRecipeSubName(builder, strSubHeading.ToString)
                            insertNewLine(builder, 2)
                        End If

                        'Servings
                        If G_ExportOptions.blnExpIncludeYield1 Or G_ExportOptions.blnExpIncludeYield2 Or G_ExportOptions.blnExpSubRecipeWt Then
                            insertYields(builder, arrYieldsLabel, arrYields)
                        End If

                        Dim servingTable As Table = builder.StartTable

                        'Recipe Time
                        If G_ExportOptions.blnExpIncludeRecipeTime Then
                            For Each RecipeTime As DataRow In dsRecipeDetails.Tables("Table5").Rows

                                strRecipeTime = RecipeTime.Item("Description")
                                Dim intHours As Integer = CIntDB(RecipeTime("RecipeTimeHH"))
                                Dim intMinutes As Integer = CIntDB(RecipeTime("RecipeTimeMM"))
                                Dim intSeconds As Integer = CIntDB(RecipeTime("RecipeTimeSS"))
                                Dim strAnd As String = cLang.GetString(clsEGSLanguage.CodeType._And).ToString.ToLower & " "

                                If intHours > 0 And intMinutes > 0 And intSeconds > 0 Then          ' 111
                                    If intHours = 1 Then strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHour).ToLower & ", ") Else strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHours).ToLower & ", ")
                                    If intMinutes = 1 Then strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinute).ToLower & " " & strAnd) Else strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinutes).ToLower & " " & strAnd)
                                    If intSeconds = 1 Then strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSecond).ToLower) Else strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSeconds).ToLower)
                                ElseIf intHours = 0 And intMinutes > 0 And intSeconds > 0 Then      ' 011
                                    If intMinutes = 1 Then strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinute).ToLower & strAnd) Else strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinutes).ToLower & " " & strAnd)
                                    If intSeconds = 1 Then strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSecond).ToLower) Else strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSeconds).ToLower)
                                    strRecipeTime = strRecipeTime.Replace("0 %h", "")
                                ElseIf intHours > 0 And intMinutes > 0 And intSeconds = 0 Then      ' 110
                                    If intHours = 1 Then strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHour).ToLower & " " & strAnd) Else strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHours).ToLower & " " & strAnd)
                                    If intMinutes = 1 Then strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinute).ToLower) Else strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinutes).ToLower)
                                    strRecipeTime = strRecipeTime.Replace("0 %s", "")
                                ElseIf intHours = 0 And intMinutes = 0 And intSeconds > 0 Then      ' 001
                                    If intSeconds = 1 Then strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSecond).ToLower) Else strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSeconds).ToLower)
                                    strRecipeTime = strRecipeTime.Replace("0 %h", "").Replace("0 %m", "")
                                ElseIf intHours = 0 And intMinutes > 0 And intSeconds = 0 Then      ' 010
                                    If intMinutes = 1 Then strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinute).ToLower) Else strRecipeTime = strRecipeTime.Replace("%m", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeMinutes).ToLower)
                                    strRecipeTime = strRecipeTime.Replace("0 %h", "").Replace("0 %s", "")
                                ElseIf intHours > 0 And intMinutes = 0 And intSeconds = 0 Then      ' 100
                                    If intHours = 1 Then strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHour).ToLower) Else strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHours).ToLower)
                                    strRecipeTime = strRecipeTime.Replace("0 %m", "").Replace("0 %s", "")
                                ElseIf intHours > 0 And intMinutes = 0 And intSeconds > 0 Then      ' 101
                                    If intHours = 1 Then strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHour).ToLower & " " & strAnd) Else strRecipeTime = strRecipeTime.Replace("%h", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeHours).ToLower & " " & strAnd)
                                    If intSeconds = 1 Then strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSecond).ToLower) Else strRecipeTime = strRecipeTime.Replace("%s", cLang.GetString(clsEGSLanguage.CodeType.RecipeTimeSeconds).ToLower)
                                    strRecipeTime = strRecipeTime.Replace("0 %m", "")
                                Else                                                                ' 000
                                    strRecipeTime = ""
                                End If

                                insertNewRowCenter(builder, servingTable, strRecipeTime.ToString)

                            Next
                        End If
                        builder.EndTable()

                        ' Ingredients
                        If dsRecipeDetails.Tables(2).Rows.Count > 0 Then

                            Dim ingredientsTable As Table = builder.StartTable()

                            For Each rwIngredient As DataRow In dsRecipeDetails.Tables(2).Rows
                                Dim intRemainingSize As Integer = 620
                                Dim intItemType As Integer
                                Dim strIngredient As String = ""
                                Dim strItemName As String = ""
                                Dim strAltIngredient As String = ""
                                Dim strIngrComplement As String = ""
                                Dim strIngrPreparation As String = ""

                                If IsDBNull(rwIngredient("Type")) Then intItemType = 0 Else intItemType = rwIngredient("Type")

                                ' Ingredient = Complement IngredientName [or AlternativeIngredient], Preparation 
                                ' Ingredient Name
                                If Not IsDBNull(rwIngredient("Name")) And Not rwIngredient("Name").ToString.Trim.Length = 0 Then
                                    strItemName = rwIngredient("Name").ToString.Trim
                                End If
                                ' Alternative Ingredient
                                If Not IsDBNull(rwIngredient("AlternativeIngredient")) And Not rwIngredient("AlternativeIngredient").ToString.Trim.Length = 0 Then
                                    strAltIngredient = "[" & cLang.GetString(clsEGSLanguage.CodeType.OR_) & " " & rwIngredient("AlternativeIngredient").ToString.Trim & "]"
                                End If
                                ' Complement
                                If Not IsDBNull(rwIngredient("Complement")) And Not rwIngredient("Complement").ToString.Trim.Length = 0 Then
                                    strIngrComplement = rwIngredient("Complement").ToString.Trim
                                End If
                                ' Preparation
                                If Not IsDBNull(rwIngredient("Preparation")) And Not rwIngredient("Preparation").ToString.Trim.Length = 0 Then
                                    strIngrPreparation = rwIngredient("Preparation").ToString.Trim
                                End If

                                ' Combine all information to form 1 ingredient detail
                                If strIngrComplement.Trim.Length > 1 Then strIngredient &= strIngrComplement & " "
                                If strItemName.Trim.Length > 1 Then strIngredient &= strItemName & " "
                                If strAltIngredient.Trim.Length > 1 Then strIngredient &= strAltIngredient
                                If strIngrPreparation.Trim.Length > 1 Then strIngredient &= ", " & strIngrPreparation

                                ' Get All quantities
                                ' For Metric and Imperial Quantities
                                Dim strMetricNet As String = "0", strMetricGross As String = "0", strMetricUnit As String = ""
                                Dim strImperialNet As String = "0", strImperialGross As String = "0", strImperialUnit As String = ""
                                ' For One Quantity
                                Dim strQtyNet As String = "0", strQtyGross As String = "0", strQtyUnit As String = ""
                                ' Total Wastage
                                Dim dblTotalWastage As Double = 0

                                If Not IsDBNull(rwIngredient("TotalWastage")) Then dblTotalWastage = CDbl(rwIngredient("TotalWastage"))
                                If rwIngredient("IngredientId") = 0 And rwIngredient("Type") = 0 Then
                                    Dim dtqty As New DataTable
                                    If Not rwIngredient("Quantity_Metric") Is Nothing Then
                                        dtqty = getAlternateQuantity(rwIngredient("Quantity_Metric").ToString, rwIngredient("UOM_Metric"), intCodeTrans, intCodeSite)
                                    Else
                                        dtqty = getAlternateQuantity(rwIngredient("Quantity_Imperial").ToString, rwIngredient("UOM_Imperial"), intCodeTrans, intCodeSite)
                                    End If

                                    If dtqty.Rows.Count > 0 Then
                                        For Each dr As DataRow In dtqty.Rows
                                            strMetricNet = dr("QtyMetric")
                                            strMetricGross = dr("QtyMetric")
                                            strMetricUnit = dr("UnitMetric")
                                            strImperialNet = dr("QtyImperial")
                                            strImperialGross = dr("QtyImperial")
                                            strImperialUnit = dr("UnitImperial")
                                        Next
                                    End If
                                Else
                                    If Not IsDBNull(rwIngredient("Quantity_Metric")) Then
                                        Dim metric_format As String = rwIngredient("UnitFormat").ToString
                                        strMetricNet = Format(CDblDB(rwIngredient("Quantity_Metric").ToString), "##0.0#")
                                        strMetricGross = Format(CDblDB(rwIngredient("QtyMetricGross").ToString), "##0.0#")

                                        strMetricNet = fctFormatNumericQuantity(CDblDB(rwIngredient("Quantity_Metric").ToString), metric_format, blnRemoveTrailingZeroes, 0)
                                        strMetricGross = fctFormatNumericQuantity(CDblDB(rwIngredient("QtyMetricGross").ToString), metric_format, blnRemoveTrailingZeroes, 0)
                                    End If
                                    If Not IsDBNull(rwIngredient("UOM_Metric")) Then
                                        strMetricUnit = rwIngredient("UOM_Metric").ToString.Replace("n/a", "").Replace("[_]", "").Replace("N/A", "")
                                    End If

                                    If Not IsDBNull(rwIngredient("Quantity_Imperial")) Then
                                        strImperialNet = ConvertDecimalToFraction2(rwIngredient("Quantity_Imperial").ToString)
                                        strImperialGross = ConvertDecimalToFraction2(CDblDB(rwIngredient("QtyImperialGross")).ToString)
                                    End If
                                    If Not IsDBNull(rwIngredient("UOM_Imperial")) Then
                                        strImperialUnit = rwIngredient("UOM_Imperial").ToString.Replace("n/a", "").Replace("[_]", "").Replace("N/A", "")
                                    End If

                                    If Not IsDBNull(rwIngredient("OneQtyNet")) Then
                                        strQtyNet = ConvertDecimalToFraction2(rwIngredient("OneQtyNet"))
                                        strQtyGross = ConvertDecimalToFraction2(CDbl(rwIngredient("OneQtyGross")).ToString)
                                        strQtyUnit = rwIngredient("OneQtyUnit").ToString.Replace("n/a", "").Replace("[_]", "").Replace("N/A", "")
                                    End If
                                End If

                                Dim intIncludedColumns As Integer = 0
                                If intItemType = 75 Then

                                    Select Case bitUseOneQuantity
                                        Case 0
                                            With G_ExportOptions
                                                If .blnExpIncludeImperialNetQty Then intIncludedColumns += 1
                                                If .blnExpIncludeImperialGrossQty Then intIncludedColumns += 1
                                                If .blnExpIncludeMetricNetQty Then intIncludedColumns += 1
                                                If .blnExpIncludeMetricGrossQty Then intIncludedColumns += 1
                                                intIncludedColumns += 1
                                            End With
                                        Case 1
                                            With G_ExportOptions
                                                If .blnExpIncludeNetQty Then intIncludedColumns += 1
                                                If .blnExpIncludeGrossQty Then intIncludedColumns += 1
                                                intIncludedColumns += 1
                                            End With
                                    End Select
                                    insertNewRowCenter(builder, ingredientsTable, strIngredient)
                                Else

                                    Dim intColSize As Integer = 100
                                    Select Case bitUseOneQuantity
                                        Case 0
                                            With G_ExportOptions
                                                If .blnExpIncludeImperialNetQty Then intIncludedColumns += 1
                                                If .blnExpIncludeImperialGrossQty Then intIncludedColumns += 1
                                                If .blnExpIncludeMetricNetQty Then intIncludedColumns += 1
                                                If .blnExpIncludeMetricGrossQty Then intIncludedColumns += 1
                                                intIncludedColumns += 1
                                            End With
                                        Case 1
                                            With G_ExportOptions
                                                If .blnExpIncludeNetQty Then intIncludedColumns += 1
                                                If .blnExpIncludeGrossQty Then intIncludedColumns += 1
                                                intIncludedColumns += 1
                                            End With
                                    End Select

                                    Dim dt As New DataTable
                                    Dim intUnitCode As Integer = -1, intIsImperialMetric As Integer = 9, strUnitFormat As String = "", dblUnitFactor As Decimal = 0D, intTypeMain As Integer = 0

                                    Dim strUnvalidatedMetricQty As String = strMetricNet, strUnvalidatedMetricUnit As String = strMetricUnit
                                    Dim strUnvalidatedImperialQty As String = strImperialNet, strUnvalidatedImperialUnit As String = strImperialUnit

                                    Select Case bitUseOneQuantity
                                        Case 0 ' Display Metric/Imperial Gross/Net quantities   


                                            If G_ExportOptions.blnExpIncludeImperialNetQty Then
                                                If rwIngredient("Type") = 0 And rwIngredient("IngredientId") = 0 And Not strMetricNet = "0" Then

                                                    If Not strUnvalidatedImperialQty = "0" Then
                                                        insertIngredient(builder, ingredientsTable, ConvertDecimalToFraction2(CDblDB(strUnvalidatedImperialQty)) & " " & strUnvalidatedImperialUnit.Replace("_", " "))
                                                    Else
                                                        insertIngredient(builder, ingredientsTable, strUnvalidatedImperialUnit.Replace("_", " "))
                                                    End If

                                                Else
                                                    If Not rwIngredient("type") = 4 Then
                                                        If Not strImperialNet = "0" Then
                                                            insertIngredient(builder, ingredientsTable, strImperialNet & " " & strImperialUnit.Replace("_", " "))
                                                        Else
                                                            insertIngredient(builder, ingredientsTable, strImperialUnit.Replace("_", " "))
                                                        End If
                                                    End If
                                                End If
                                            End If

                                            If G_ExportOptions.blnExpIncludeImperialGrossQty Then
                                                If rwIngredient("Type") = 0 And rwIngredient("IngredientId") = 0 Then
                                                    If Not strUnvalidatedImperialQty = "0" Then
                                                        insertIngredient(builder, ingredientsTable, ConvertDecimalToFraction2(CDblDB(strUnvalidatedImperialQty)) & " " & strUnvalidatedImperialUnit.Replace("_", " "))
                                                    Else
                                                        insertIngredient(builder, ingredientsTable, strUnvalidatedImperialUnit.Replace("_", " "))
                                                    End If
                                                Else
                                                    If Not rwIngredient("type") = 4 Then
                                                        If Not strImperialGross = "0" Then
                                                            insertIngredient(builder, ingredientsTable, strImperialGross & " " & strImperialUnit.Replace("_", " "))
                                                        Else
                                                            insertIngredient(builder, ingredientsTable, strImperialUnit.Replace("_", " "))
                                                        End If
                                                    End If
                                                End If
                                            End If

                                            If G_ExportOptions.blnExpIncludeMetricNetQty Then
                                                If rwIngredient("Type") = 0 And rwIngredient("IngredientId") = 0 Then
                                                    If Not strUnvalidatedMetricQty = "0" Then
                                                        insertIngredient(builder, ingredientsTable, strUnvalidatedMetricQty & " " & strUnvalidatedMetricUnit.Replace("_", " "))
                                                    Else
                                                        insertIngredient(builder, ingredientsTable, strUnvalidatedMetricUnit.Replace("_", " "))
                                                    End If
                                                Else
                                                    If Not rwIngredient("type") = 4 Then

                                                        If Not strMetricNet = "0" Then
                                                            insertIngredient(builder, ingredientsTable, strMetricNet & " " & strMetricUnit.Replace("_", " "))
                                                        Else
                                                            insertIngredient(builder, ingredientsTable, strMetricUnit.Replace("_", " "))
                                                        End If
                                                    End If
                                                End If
                                            End If

                                            If G_ExportOptions.blnExpIncludeMetricGrossQty Then
                                                If rwIngredient("Type") = 0 And rwIngredient("IngredientId") = 0 Then
                                                    If Not strUnvalidatedMetricQty = "0" Then
                                                        insertIngredient(builder, ingredientsTable, strUnvalidatedMetricQty & " " & strUnvalidatedMetricUnit.Replace("_", " "))
                                                    Else
                                                        insertIngredient(builder, ingredientsTable, strUnvalidatedMetricUnit.Replace("_", " "))
                                                    End If
                                                Else
                                                    If Not rwIngredient("type") = 4 Then
                                                        If Not strMetricGross = "0" Then
                                                            insertIngredient(builder, ingredientsTable, strMetricGross & " " & strMetricUnit.Replace("_", " "))
                                                        Else
                                                            insertIngredient(builder, ingredientsTable, strMetricUnit.Replace("_", " "))
                                                        End If
                                                    End If
                                                End If
                                            End If

                                        Case 1 ' Display Gross and Net Quantities only


                                            If G_ExportOptions.blnExpIncludeNetQty Then
                                                If Not rwIngredient("type") = 4 Then
                                                    If Not strQtyNet = "0" Then
                                                        insertIngredient(builder, ingredientsTable, strQtyNet & " " & strQtyUnit.Replace("_", " "))
                                                    Else
                                                        insertIngredient(builder, ingredientsTable, strQtyUnit.Replace("_", " "))
                                                    End If
                                                End If
                                            End If

                                            If G_ExportOptions.blnExpIncludeGrossQty Then
                                                If Not rwIngredient("type") = 4 Then
                                                    If Not strQtyNet = "0" Then
                                                        insertIngredient(builder, ingredientsTable, strQtyGross & " " & strQtyUnit.Replace("_", " "))
                                                    Else
                                                        insertIngredient(builder, ingredientsTable, strQtyUnit.Replace("_", " "))
                                                    End If
                                                End If
                                            End If
                                        Case Else
                                    End Select

                                    ' Ingredient name
                                    insertIngredient(builder, ingredientsTable, strIngredient)
                                    builder.EndRow()

                                End If
                            Next
                            builder.EndTable()
                        End If

                        If G_ExportOptions.intExpSelectedProcedure = 0 Then
                            strMethodHeader = cLang.GetString(clsEGSLanguage.CodeType.PreparationMethod)
                        Else
                            strMethodHeader = cLang.GetString(clsEGSLanguage.CodeType.CookMode)
                        End If

                        If G_ExportOptions.blnExpIncludeProcedure Then
                            Dim procedureTable As Table = builder.StartTable()

                            Select Case G_ExportOptions.intExpSelectedProcedure
                                Case 0
                                    'Method Header
                                    If strMethodHeader.ToString <> "" Then
                                        insertProcedure(builder, procedureTable, strMethodHeader.ToString)
                                    End If
                                    ''Case 1
                                    'Directions
                                    If strDirections.ToString <> "" Then
                                        insertProcedure(builder, procedureTable, strDirections.ToString)
                                    End If
                                Case Else
                                    'Method Header
                                    If strMethodHeader.ToString <> "" Then
                                        insertProcedure(builder, procedureTable, strMethodHeader.ToString)
                                    End If

                                    'Directions
                                    If strAbbrDirections.ToString <> "" Then
                                        insertProcedure(builder, procedureTable, strAbbrDirections.ToString)
                                    End If
                            End Select

                            builder.EndTable()
                        End If

                        Dim notesTable As Table = builder.StartTable()

                        If G_ExportOptions.blnExpIncludeNotes Then
                            insertNotes(builder, notesTable, cLang.GetString(clsEGSLanguage.CodeType.Notes), strFootNote1.ToString)
                        End If

                        If G_ExportOptions.blnExpIncludeAddNotes Then
                            insertNotes(builder, notesTable, cLang.GetString(clsEGSLanguage.CodeType.AdditionalNotes), strFootNote2.ToString)
                        End If

                        builder.EndTable()

                        ' Nutrients  
                        If G_ExportOptions.blnExpIncludeNutrientInfo And dsRecipeDetails.Tables("Table4").Rows.Count > 0 Then
                            Dim strNutBasis As String = ""
                            If Not IsDBNull(dsRecipeDetails.Tables("Table4").Rows(0).Item("NutritionBasis")) Then strNutBasis = dsRecipeDetails.Tables("Table4").Rows(0).Item("NutritionBasis")
                            strHTMLContent.Append(fctDisplayNutrientComputationForExport(m_RecipeId, strServingsUnit, G_ExportOptions.intExpSelectedNutrientSet, G_ExportOptions.intExpSelectedLanguage, G_ExportOptions.intExpSelectedNutrientComputation, , strNutBasis, m_Version, True))

                            'Net Carbs
                            If isDisplay = True Then
                                If strNetCarbohydrates.ToString <> "" Then
                                    Dim nutrientsTable As Table = builder.StartTable()
                                    insertNewRowLeft(builder, nutrientsTable, strNetCarbohydrates.ToString)
                                    insertNewRowCenter(builder, nutrientsTable, lblNetCarbs.ToString)
                                    builder.EndTable()
                                End If
                            End If

                            strNutrients = ""
                        End If

                        ' RDC 02.11.2014 : GDA
                        If G_ExportOptions.blnExpIncludeGDA Then
                            strHTMLContent.Append(fctDisplayGDAComputationForExport(m_RecipeId, dsRecipeDetails.Tables(1).Rows(0).Item("ServingsUnit").ToString, G_ExportOptions.intExpSelectedNutrientSet, G_ExportOptions.intExpSelectedLanguage, G_ExportOptions.intExpSelectedGDA, , "", m_Version, True))
                        End If

                        'Information
                        Dim infoTable As Table = builder.StartTable()
                        If G_ExportOptions.blnExpAdvIncludeInfo Then


                            builder.Font.Bold = True
                            insertNewRowCenter(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.Information))
                            insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.RecipeStatus), strRecipeStatus.ToString)

                            If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
                                insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.UpdatedBy), strUpdatedBy.ToString)
                            End If

                            If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
                                insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.WebStatus), strWebStatus.ToString)
                            End If

                            insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.DateCreated), strDateCreated.ToString)
                            insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.CreatedBY), strCreatedBy.ToString)
                            insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.DateLastModified), strDateLastModified.ToString)
                            insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.ModifiedBy), strModifiedBy.ToString)

                            If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Then
                                insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.DateLastTested), strLastTested.ToString)
                                insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.TestedBy), strTestedBy.ToString)
                                insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.DateDeveloped), strDateDeveloped.ToString)
                                insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.DevelopedBy), strDevelopedBy.ToString)
                                insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.FinalEditDate), strDateOfFinalEdit.ToString)
                                insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.FinalEditBy), strFinalEditBy.ToString)
                                insertNewRowWithLabel(builder, infoTable, cLang.GetString(clsEGSLanguage.CodeType.DevelopmentPurpose), strDevelopmentPurpose.ToString)
                            End If
                        End If

                        'Recipe Brand
                        Dim recipeBrandTable As Table = builder.StartTable()
                        If G_ExportOptions.blnExpAdvIncludeBrands Then
                            If dsRecipeDetails.Tables("table8").Rows.Count > 0 Then
                                builder.Font.Bold = True
                                insertNewRowCenter(builder, recipeBrandTable, cLang.GetString(clsEGSLanguage.CodeType.Brand))
                                builder.Font.Bold = False

                                For Each Brands As DataRow In dsRecipeDetails.Tables("table8").Rows
                                    strRecipeBrand = fctCheckDbNull(Brands.Item("BrandName"))
                                    strRecipeBrandClassification = fctCheckDbNull(Brands.Item("BrandClassification"))

                                    If clsLicense.l_App = EgswKey.clsLicense.enumApp.USA Or clsLicense.l_App = EgswKey.clsLicense.enumApp.RB Then
                                        insertNewRowCenter(builder, recipeBrandTable, strRecipeBrand.ToString & " - " & strRecipeBrandClassification.ToString)
                                    Else
                                        insertNewRowCenter(builder, recipeBrandTable, strRecipeBrand.ToString)
                                    End If
                                Next
                            End If
                        End If
                        builder.EndTable()

                        If G_ExportOptions.blnExpAdvIncludeKeywords Then
                            'Attributes
                            Dim attributesTable As Table = builder.StartTable()
                            If dsRecipeDetails.Tables("table7").Rows.Count > 0 Then

                                builder.Font.Bold = True
                                insertNewRowCenter(builder, attributesTable, cLang.GetString(clsEGSLanguage.CodeType.Keywords))
                                builder.Font.Bold = False

                                For Each drKeywords As DataRow In dsRecipeDetails.Tables("Table7").Rows
                                    insertNewRowCenter(builder, attributesTable, drKeywords("Name"))
                                Next
                            End If
                            builder.EndTable()
                        End If

                        If G_ExportOptions.blnExpAdvIncludeCookbook Then
                            ' Cookbooks
                            Dim cookbookTable As Table = builder.StartTable()
                            If dsRecipeDetails.Tables(10).Rows.Count > 0 Then
                                builder.Font.Bold = True
                                insertNewRowCenter(builder, cookbookTable, cLang.GetString(clsEGSLanguage.CodeType.Cookbook))
                                builder.Font.Bold = False

                                For Each rwCookbooks As DataRow In dsRecipeDetails.Tables(10).Rows
                                    insertNewRowCenter(builder, cookbookTable, rwCookbooks("Name").ToString)
                                Next

                            End If
                            builder.EndTable()
                        End If

                        If G_ExportOptions.blnExpAdvIncludePublication Then
                            'Placements
                            Dim placementTable As Table = builder.StartTable()
                            If dsRecipeDetails.Tables("table9").Rows.Count > 0 Then

                                builder.Font.Bold = True
                                insertNewRowCenter(builder, placementTable, lblPlacements.ToString)
                                builder.Font.Bold = False

                                For Each Placements As DataRow In dsRecipeDetails.Tables("table9").Rows
                                    strPlacementName = fctCheckDbNull(Placements.Item("Placement"))
                                    If Not IsDBNull(Placements.Item("PlacementDate")) Then strPlacementDate = CDate(Placements.Item("PlacementDate")).ToString("MM/dd/yyyy")
                                    strPlacementDescription = fctCheckDbNull(Placements.Item("Description"))

                                    insertPlacement(builder, placementTable, strPlacementName.ToString, strPlacementDate.ToString, strPlacementDescription.ToString)
                                Next
                            End If
                            builder.EndTable()
                        End If

                        If G_ExportOptions.blnExpAdvIncludeComments Then
                            'Comments
                            Dim commentTable As Table = builder.StartTable()
                            If dsRecipeDetails.Tables("table6").Rows.Count > 0 Then
                                builder.Font.Bold = True
                                insertNewRowCenter(builder, commentTable, cLang.GetString(clsEGSLanguage.CodeType.Comments))
                                builder.Font.Bold = False

                                For Each Comments As DataRow In dsRecipeDetails.Tables("table6").Rows
                                    If Not IsDBNull(Comments.Item("SubmitDate")) Then strSubmitDate = CDate(Comments.Item("SubmitDate")).ToString("MM/dd/yyyy")
                                    strOwnerName = fctCheckDbNull(Comments.Item("OwnerName"))
                                    strComments = fctCheckDbNull(Comments.Item("Description"))

                                    insertComment(builder, commentTable, strSubmitDate.ToString, strOwnerName.ToString, strComments.ToString)
                                Next
                            End If
                            builder.EndTable()
                        End If

                    Else
                        'strHTMLContent.Append("<table style='width: 620'>")
                        'strHTMLContent.Append("<tr>")
                        'strHTMLContent.Append("<td>")
                        strHTMLContent.Append("<table style='width: 620'>")

                        'Recipe Name
                        strHTMLContent.Append("<p style='font-weight: bold; font-size: x-large; text-align: center; font-family: Calibri;'>")
                        strHTMLContent.Append(strRecipeName.ToString)
                        strHTMLContent.Append("</p>")

                        'Recipe Number
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td>")
                        strHTMLContent.Append("<table>")
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(lblRecipeNumber.ToString)
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strRecipeNumber.ToString)
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                        strHTMLContent.Append("</table>")
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")

                        'strHTMLContent.Append("<tr>")
                        'strHTMLContent.Append("<td>")
                        'strHTMLContent.Append("&nbsp;")
                        'strHTMLContent.Append("</td>")
                        'strHTMLContent.Append("</tr>")

                        'Sub Title
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td>")
                        strHTMLContent.Append("<table>")
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td style='font-weight: bold; font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(lblSubTitle.ToString)
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strSubTitle.ToString)
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                        strHTMLContent.Append("</table>")
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")

                        'strHTMLContent.Append("<tr>")
                        'strHTMLContent.Append("<td>")
                        'strHTMLContent.Append("&nbsp;")
                        'strHTMLContent.Append("</td>")
                        'strHTMLContent.Append("</tr>")
                        'strHTMLContent.Append("</table>")

                        'Description
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td>")
                        strHTMLContent.Append("<table>")
                        strHTMLContent.Append("<tr>")
                        If strRecipeDescription.ToString <> "" Then
                            strHTMLContent.Append("<td style='font-weight: bold; width: 80 ;font-size: 11.5pt; font-family: Calibri;'>")
                            strHTMLContent.Append(lblRecipeDescription.ToString)
                            strHTMLContent.Append("</td>")
                        End If
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strRecipeDescription.ToString)
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                        strHTMLContent.Append("</table>")
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")

                        'strHTMLContent.Append("<tr>")
                        'strHTMLContent.Append("<td>")
                        'strHTMLContent.Append("&nbsp;")
                        'strHTMLContent.Append("</td>")
                        'strHTMLContent.Append("</tr>")

                        'Remark
                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td>")
                        strHTMLContent.Append("<table>")
                        strHTMLContent.Append("<tr>")
                        If strRecipeRemark.ToString <> "" Then
                            strHTMLContent.Append("<td style='font-weight: bold; width: 80 ;font-size: 11.5pt; font-family: Calibri;'>")
                            strHTMLContent.Append(lblRecipeRemark.ToString)
                            strHTMLContent.Append("</td>")
                        End If
                        strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strRecipeRemark.ToString)
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")
                        strHTMLContent.Append("</table>")
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")

                        strHTMLContent.Append("<tr>")
                        strHTMLContent.Append("<td>")
                        strHTMLContent.Append("&nbsp;")
                        strHTMLContent.Append("</td>")
                        strHTMLContent.Append("</tr>")

                        Dim strImage2 As String

                        strImage2 = dtRecipes.Rows(x).Item("ImageLoc2").ToString
                        intStandard = dtRecipes.Rows(x).Item("Ratings").ToString

                        imgRecipe = strImage
                        strHTMLContent.Append("<p style='font-weight: bold; font-size: x-large; text-align: center; font-family: Calibri;'>")
                        strHTMLContent.Append(imgRecipe) 'CMV 051911

                        If Not strImage.Contains("nopic.jpg") And Not strImage2.Contains("nopic.jpg") Then
                            If Not strImage2 = "" Then
                                'strHTMLContent.Append(" <td style='text-align: center'>")
                                strHTMLContent.Append("&nbsp")
                                imgRecipe2 = strImage2 ' getHtml(imageRecipe) 'CMV 051911
                                strHTMLContent.Append(imgRecipe2) 'CMV 051911
                                'strHTMLContent.Append("</td>")
                            End If
                        End If

                        strHTMLContent.Append("</p>")

                        ''Image
                        'imgRecipe = strImage 'getHtml(imageRecipe) 'CMV 051911
                        ''strHTMLContent.Append(imgRecipe) 'CMV 051911
                        'strHTMLContent.Append("<p style='font-weight: bold; font-size: x-large; text-align: center; font-family: Calibri;'>")
                        'strHTMLContent.Append(imgRecipe) 'CMV 051911
                        ''strHTMLContent.Append("<img src='" & imgRecipe & "' height=240 width=240 />")
                        'strHTMLContent.Append("</p>")

                        ''Recipe Name
                        'strHTMLContent.Append("<p style='font-weight: bold; font-size: x-large; text-align: center; font-family: Calibri;'>")
                        'strHTMLContent.Append(strRecipeName.ToString)
                        'strHTMLContent.Append("</p>")

                        'Subheading
                        strHTMLContent.Append("<p style='font-weight: bold; text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strSubHeading.ToString)
                        strHTMLContent.Append("</p>")

                        'Servings
                        strHTMLContent.Append("<p style='text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strServings.ToString)
                        strHTMLContent.Append("</p>")

                        'Recipe Time
                        strHTMLContent.Append("<p style='text-align: center; font-size: 11.5pt; font-family: Calibri;'>")
                        For Each RecipeTime As DataRow In dsRecipeDetails.Tables("table5").Rows
                            strRecipeTime = RecipeTime.Item("Description")
                            strHTMLContent.Append(strRecipeTime.ToString)
                            strHTMLContent.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
                            strRecipeTime = ""
                        Next
                        strHTMLContent.Append("</p>")

                        'Ingredients
                        If dsRecipeDetails.Tables("table2").Rows.Count > 0 Then
                            strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")
                            '-- JBB 05.24.2011 (code pass by Cielo)
                            For Each Ingredients As DataRow In dsRecipeDetails.Tables("table2").Rows
                                If Ingredients.Item("Type").ToString().Trim() <> "4" Then
                                    '    strIngredients = fctCheckDbNull(Ingredients.Item("Description"))
                                    'Else
                                    If bitQtyFormat = 0 Then
                                        'strIngredients = fctCheckDbNullNumeric(Ingredients.Item("Quantity_Metric")) & " " & fctCheckDbNull(Ingredients.Item("UOM_Metric")) & " " & fctCheckDbNull(Ingredients.Item("Complement")) & " " & fctCheckDbNull(Ingredients.Item("Name")) & "," & fctCheckDbNull(Ingredients.Item("Preparation"))
                                        ''-- JBB 10.26.2011
                                        'If fctCheckDbNull(Ingredients.Item("AlternativeIngredient")) <> "" Then
                                        '    strIngredients = strIngredients & " or " & fctCheckDbNull(Ingredients.Item("AlternativeIngredient"))
                                        'End If

                                        'TDQ 11022011
                                        If fctCheckDbNull(Ingredients.Item("AlternativeIngredient")) = "" Then
                                            strIngredients = IIf(Ingredients.Item("Quantity_Metric") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Metric"))) & " " & _
                                              fctCheckDbNull(Ingredients.Item("UOM_Metric")) & " " & _
                                              fctCheckDbNull(Ingredients.Item("Complement") & " " & _
                                              fctCheckDbNull(Ingredients.Item("Name") & IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & _
                                              fctCheckDbNull(Ingredients.Item("Preparation"))))
                                        Else
                                            strIngredients = IIf(Ingredients.Item("Quantity_Metric") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Metric"))) & " " & _
                                              fctCheckDbNull(Ingredients.Item("UOM_Metric")) & " " & _
                                              fctCheckDbNull(Ingredients.Item("Complement") & " " & _
                                              fctCheckDbNull(Ingredients.Item("Name") & " &#91or " & _
                                              fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & _
                                              fctCheckDbNull(Ingredients.Item("Preparation")))))
                                        End If

                                        'If fctCheckDbNull(Ingredients.Item("Preparation")) = "" Then 'TDQ 10172011
                                        '    strIngredients = strIngredients.Substring(0, strIngredients.Length - 1)
                                        'End If

                                        '-- JBB 10.25.2011
                                        'strIngredients = strIngredients.Replace("0 N/A", "")
                                        'strIngredients = strIngredients.Replace("0 n/a", "")
                                        strIngredients = strIngredients.Replace("N/A", "")
                                        strIngredients = strIngredients.Replace("n/a", "")
                                        strIngredients = strIngredients + "<br>"

                                        '--
                                    ElseIf bitQtyFormat = 1 Then
                                        If IsNumeric(Ingredients.Item("Quantity_Imperial")) = True Then
                                            If fctCheckDbNull(Ingredients.Item("AlternativeIngredient")) = "" Then 'TDQ 10172011
                                                If blnUseFractions Then
                                                    strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctConvertToFraction2(fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial")))) & " " & _
                                                     fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " & _
                                                     fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & fctCheckDbNull(Ingredients.Item("Preparation")))) 'TDQ 10142011
                                                Else
                                                    strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial"))) & " " & _
                                                     fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " & _
                                                     fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & fctCheckDbNull(Ingredients.Item("Preparation")))) 'TDQ 10142011
                                                End If
                                            Else
                                                If blnUseFractions Then
                                                    strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctConvertToFraction2(fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial")))) & " " & _
                                                     fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " & _
                                                     fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & " &#91or " & fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & fctCheckDbNull(Ingredients.Item("Preparation"))))) 'TDQ 10142011
                                                Else
                                                    strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial"))) & " " & _
                                                     fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " & _
                                                     fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & " &#91or " & fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & fctCheckDbNull(Ingredients.Item("Preparation"))))) 'TDQ 10142011
                                                End If

                                            End If
                                            '-- JBB 10.25.2011
                                            'strIngredients = strIngredients.Replace("0 N/A", "")
                                            'strIngredients = strIngredients.Replace("0 n/a", "")
                                            strIngredients = strIngredients.Replace("N/A", "")
                                            strIngredients = strIngredients.Replace("n/a", "")

                                            '--
                                        Else
                                            If fctCheckDbNull(Ingredients.Item("AlternativeIngredient")) = "" Then 'TDQ 10172011
                                                strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNull(Ingredients.Item("Quantity_Imperial"))) & " " & _
                                                  fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " &
                                                  fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & fctCheckDbNull(Ingredients.Item("Preparation")))) 'TDQ 10142011
                                            Else
                                                strIngredients = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNull(Ingredients.Item("Quantity_Imperial"))) & " " & _
                                                  fctCheckDbNull(Ingredients.Item("UOM_Imperial")) & " " &
                                                  fctCheckDbNull(Ingredients.Item("Complement") & " " & fctCheckDbNull(Ingredients.Item("Name") & " &#91or " & fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & fctCheckDbNull(Ingredients.Item("Preparation"))))) 'TDQ 10142011
                                            End If
                                            '-- JBB 10.25.2011
                                            'strIngredients = strIngredients.Replace("0 N/A", "")
                                            'strIngredients = strIngredients.Replace("0 n/a", "")
                                            strIngredients = strIngredients.Replace("N/A", "")
                                            strIngredients = strIngredients.Replace("n/a", "")

                                            '--
                                        End If

                                        'If fctCheckDbNull(Ingredients.Item("Preparation")) = "" Then 'TDQ 10172011
                                        '    strIngredients = strIngredients.Substring(0, strIngredients.Length - 1)
                                        'End If

                                        strIngredients = strIngredients + "<br>"

                                    ElseIf bitQtyFormat = 2 Then
                                        strIngredients = fctCheckDbNull(Ingredients.Item("Description"))
                                    ElseIf bitQtyFormat = 3 Then ' JBB 07.08.2011
                                        Dim strM As String = IIf(Ingredients.Item("Quantity_Metric") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Metric"))) & " " & fctCheckDbNull(Ingredients.Item("UOM_Metric")) & " "
                                        '-- JBB 10.25.2011
                                        'strM = strM.Replace("0 N/A", "")
                                        'strM = strM.Replace("0 n/a", "")
                                        strM = strM.Replace("N/A", "")
                                        strM = strM.Replace("n/a", "")
                                        '--
                                        Dim strI As String = ""
                                        If IsNumeric(Ingredients.Item("Quantity_Imperial")) = True Then
                                            If blnUseFractions Then
                                                strI = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctConvertToFraction2(fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial")))) & " " & fctCheckDbNull(Ingredients.Item("UOM_Imperial"))
                                            Else
                                                strI = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNullNumeric(Ingredients.Item("Quantity_Imperial"))) & " " & fctCheckDbNull(Ingredients.Item("UOM_Imperial"))
                                            End If
                                        Else
                                            strI = IIf(Ingredients.Item("Quantity_Imperial") = "0", "", fctCheckDbNull(Ingredients.Item("Quantity_Imperial"))) & " " & fctCheckDbNull(Ingredients.Item("UOM_Imperial"))
                                        End If
                                        '-- JBB 10.25.2011
                                        ' strI = strI.Replace("0 N/A", "")
                                        ' strI = strI.Replace("0 n/a", "")
                                        strI = strI.Replace("N/A", "")
                                        strI = strI.Replace("n/a", "")

                                        Dim strIngName As String

                                        'TDQ 11022011
                                        If fctCheckDbNull(Ingredients.Item("AlternativeIngredient")) = "" Then
                                            strIngName = fctCheckDbNull(Ingredients.Item("Complement") & " " & _
                                              fctCheckDbNull(Ingredients.Item("Name") & IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & _
                                              fctCheckDbNull(Ingredients.Item("Preparation"))))
                                        Else
                                            strIngName = fctCheckDbNull(Ingredients.Item("Complement") & " " & _
                                              fctCheckDbNull(Ingredients.Item("Name") & " &#91or " & _
                                              fctCheckDbNull(Ingredients.Item("AlternativeIngredient") & "&#93" & IIf(fctCheckDbNull(Ingredients.Item("Preparation")) = "", "", IIf(AutoSpacing = True, ", ", ",")) & _
                                              fctCheckDbNull(Ingredients.Item("Preparation")))))
                                        End If

                                        'If fctCheckDbNull(Ingredients.Item("Preparation")) = "" Then
                                        '    strIngName = strIngName.Substring(0, strIngName.Length - 1)
                                        'End If

                                        '--
                                        Dim strTempTemp As String = "<table  style='font-size: 11.5pt; font-family: Calibri;'><tr><td style='width: 100' valign='top'>%M</td><td style='width: 100' valign='top'>%I</td><td valign='top'>%N</td></tr></table>"
                                        strIngredients = strTempTemp.Replace("%M", strM).Replace("%I", strI).Replace("%N", strIngName)
                                    End If
                                Else ' JBB 07.14.2011 if Text
                                    If bitQtyFormat = 0 Then
                                        strIngredients = fctCheckDbNull(Ingredients.Item("Name"))
                                        strIngredients = strIngredients + "<br>"
                                    ElseIf bitQtyFormat = 1 Then
                                        strIngredients = fctCheckDbNull(Ingredients.Item("Name"))
                                        strIngredients = strIngredients + "<br>"
                                    ElseIf bitQtyFormat = 2 Then
                                        strIngredients = fctCheckDbNull(Ingredients.Item("Description"))
                                    Else
                                        Dim strTempTemp As String = "<table  style='font-size: 11.5pt; font-family: Calibri;width:620px'><tr><td style='width: 100' valign='top'>&nbsp</td><td style='width: 100' valign='top'>&nbsp</td><td valign='top'>%N</td></tr></table>"
                                        strIngredients = strTempTemp.Replace("%N", fctCheckDbNull(Ingredients.Item("Name")))
                                    End If
                                End If
                                'strHTMLContent.Append("<tr>")
                                'strHTMLContent.Append("<td style='font-size: 11.5pt; font-family: Calibri;'>")
                                strHTMLContent.Append(strIngredients.ToString)
                                '--strHTMLContent.Append("<br>") ' JBB 10.25.2011
                                'strHTMLContent.Append("</td>")
                                'strHTMLContent.Append("</tr>")
                            Next
                            '--
                            'For Each Ingredients As DataRow In dsRecipeDetails.Tables("table2").Rows
                            '    If Ingredients.Item("Description") <> "" Then
                            '        strIngredients = fctCheckDbNull(Ingredients.Item("Description"))
                            '    Else
                            '        strIngredients = fctCheckDbNullNumeric(Ingredients.Item("Quantity")) & " " & fctCheckDbNull(Ingredients.Item("UOM")) & " " & fctCheckDbNull(Ingredients.Item("Name"))
                            '    End If
                            '    strHTMLContent.Append(strIngredients.ToString)
                            '    strHTMLContent.Append("<br>")
                            'Next
                            '

                            strHTMLContent.Append("</p>")
                        End If

                        'Method Header
                        If strMethodHeader.ToString <> "" Then
                            strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri; font-weight: bold' align='center'>")
                            strHTMLContent.Append(strMethodHeader.ToString)
                            strHTMLContent.Append("</p>")
                        End If

                        'Directions
                        If strDirections.ToString <> "" Then
                            strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")
                            'strHTMLContent.Append(strDirections.ToString.Replace(Chr(13) & Chr(10), "<br>"))
                            strHTMLContent.Append(strDirections.ToString)
                            strHTMLContent.Append("</p>")
                        End If

                        'Footnote 1
                        strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strFootNote1.ToString.Replace(Chr(13) & Chr(10), "<br>"))
                        'strHTMLContent.Append(strFootNote1)
                        strHTMLContent.Append("</p>")

                        'Footnote 2
                        strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri;'>")
                        strHTMLContent.Append(strFootNote2.ToString.Replace(Chr(13) & Chr(10), "<br>"))
                        'strHTMLContent.Append(strFootNote2)
                        strHTMLContent.Append("</p>")

                        ''Placements
                        'If dsRecipeDetails.Tables("table9").Rows.Count > 0 Then
                        '    strHTMLContent.Append("<p style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: center'>")
                        '    strHTMLContent.Append(lblPlacements.ToString)
                        '    strHTMLContent.Append("</p>")
                        '    strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri; padding-right: 10px'>")
                        '    For Each Placements As DataRow In dsRecipeDetails.Tables("table9").Rows
                        '        strPlacementName = fctCheckDbNull(Placements.Item("Placement"))
                        '        If Not IsDBNull(Placements.Item("PlacementDate")) Then strPlacementDate = CDate(Placements.Item("PlacementDate")).ToString("MM/dd/yyyy")
                        '        strPlacementDescription = fctCheckDbNull(Placements.Item("Description"))

                        '        strHTMLContent.Append(strPlacementName.ToString)
                        '        strHTMLContent.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
                        '        strHTMLContent.Append(strPlacementDate.ToString)
                        '        strHTMLContent.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
                        '        strHTMLContent.Append(strPlacementDescription.ToString)
                        '        strHTMLContent.Append("<br>")
                        '    Next
                        '    strHTMLContent.Append("</p>")
                        'End If

                        'Nutrients
                        If isDisplay = True Then
                            If dsRecipeDetails.Tables("table4").Rows.Count > 0 Then
                                strHTMLContent.Append("<p style='font-weight: bold; font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
                                '-- JBB 02.23.2012
                                Dim strNutBasis As String = fctCheckDbNull(dsRecipeDetails.Tables("table4").Rows(0).Item("NutritionBasis"))
                                If lblNutritionalInformation.ToString().Trim <> "" Then '-- JBB 02.23.2012
                                    If strNutBasis = "" Then
                                        strHTMLContent.Append(lblNutritionalInformation.ToString & " :")
                                    Else
                                        strHTMLContent.Append(lblNutritionalInformation.ToString & "(" & strNutBasis & ") :")
                                    End If
                                End If
                                'strHTMLContent.Append(lblNutritionalInformation.ToString)
                                '--
                                strHTMLContent.Append("</p>")
                                strHTMLContent.Append(strNutrients.ToString)
                            End If

                            'Net Carbs
                            If strNetCarbohydrates.ToString <> "" Then
                                strHTMLContent.Append("<p style='font-size: 11.5pt; font-family: Calibri; text-align: left;'>")
                                strHTMLContent.Append(strNetCarbohydrates.ToString)
                                strHTMLContent.Append("</p>")
                                strHTMLContent.Append(lblNetCarbs.ToString)
                            End If
                        End If
                        strNutrients = ""
                    End If

                    If Not rowCount = x + 1 Then
                        strHTMLContent.Append("<br style='page-break-before:always' />")
                    End If

                Else
                    strErr = cLang.GetString(clsEGSLanguage.CodeType.FileNotFound)
                End If
            End If
        Next

        strHTMLContent.Append("</table></div></body></html>")
        strErr = ""

        Return doc
    End Function

    Private Sub insertRecipeTitle(docBuilder As DocumentBuilder, recipeTitle As String)
        docBuilder.ParagraphFormat.Alignment = ParagraphAlignment.Center
        docBuilder.Font.Name = "Calibri"
        docBuilder.Font.Size = 24
        docBuilder.Font.Bold = True
        docBuilder.Write(recipeTitle)
    End Sub

    Private Sub insertRecipeSubName(docBuilder As DocumentBuilder, recipeTitle As String)
        docBuilder.ParagraphFormat.Alignment = ParagraphAlignment.Center
        docBuilder.Font.Name = "Calibri"
        docBuilder.Write(recipeTitle)
    End Sub

    Private Sub insertNewLine(docBuilder As DocumentBuilder, noOfNewLine As Integer)
        For ctr As Integer = 1 To noOfNewLine
            docBuilder.InsertBreak(BreakType.LineBreak)
        Next
    End Sub

    Private Sub insertRecipeSubTitle(docBuilder As DocumentBuilder, table As Table, label As String, value As String)

        docBuilder.Font.Size = 11.5
        docBuilder.Font.Bold = False

        docBuilder.InsertCell()
        table.ClearBorders()
        table.PreferredWidth = PreferredWidth.Auto
        table.Alignment = TableAlignment.Left
        docBuilder.Font.Bold = True
        docBuilder.ParagraphFormat.Alignment = ParagraphAlignment.Left
        docBuilder.Write(label)
        docBuilder.InsertCell()
        docBuilder.Write(":")
        docBuilder.InsertCell()
        docBuilder.Font.Bold = False
        docBuilder.Write(value)
        docBuilder.EndRow()

    End Sub

    Private Sub insertRecipeImage(docBuilder As DocumentBuilder, imgPath As String)
        Try
            Dim recipeImg As Shape = docBuilder.InsertImage(imgPath)
            recipeImg.WrapType = WrapType.Inline
            recipeImg.Width = 300
            recipeImg.Height = 300
        Catch ex As System.Net.WebException

        End Try
    End Sub

    Private Sub insertYields(docBuilder As DocumentBuilder, arrYieldsLabel As Array, arrYieldsValue As Array)

        Dim yieldTable As Table = docBuilder.StartTable

        Dim ctr As Integer = 0

        If arrYieldsValue IsNot Nothing Then
            For Each yieldValue As String In arrYieldsValue

                If yieldValue IsNot Nothing Then
                    docBuilder.InsertCell()
                    yieldTable.ClearBorders()
                    yieldTable.PreferredWidth = PreferredWidth.Auto
                    yieldTable.Alignment = TableAlignment.Center
                    docBuilder.Font.Bold = True
                    docBuilder.ParagraphFormat.Alignment = ParagraphAlignment.Center
                    docBuilder.Write(arrYieldsLabel(ctr))
                    docBuilder.InsertCell()
                    docBuilder.Write(":")
                    docBuilder.Font.Bold = False
                    docBuilder.InsertCell()
                    docBuilder.Font.Bold = False
                    docBuilder.Write(arrYieldsValue(ctr))
                    ctr += 1
                Else
                    Exit For
                End If
            Next
        End If

        docBuilder.EndTable()
    End Sub

    Private Sub insertRecipeTime(docBuilder As DocumentBuilder, recipeTable As Table, recipeTime As String)
        docBuilder.InsertCell()
        recipeTable.ClearBorders()
        recipeTable.PreferredWidth = PreferredWidth.Auto
        recipeTable.Alignment = TableAlignment.Center
        docBuilder.Write(recipeTime)
        docBuilder.EndRow()
    End Sub

    Private Sub insertIngredient(docBuilder As DocumentBuilder, ingredientsTable As Table, ingredients As String)
        docBuilder.InsertCell()
        ingredientsTable.ClearBorders()
        ingredientsTable.PreferredWidth = PreferredWidth.Auto
        ingredientsTable.Alignment = TableAlignment.Center
        docBuilder.Write(ingredients)
    End Sub

    Private Sub insertProcedure(docBuilder As DocumentBuilder, procedureTable As Table, procedureHeader As String)
        docBuilder.InsertCell()
        procedureTable.ClearBorders()
        procedureTable.PreferredWidth = PreferredWidth.Auto
        procedureTable.Alignment = TableAlignment.Center
        docBuilder.Font.Bold = True
        docBuilder.Write(procedureHeader)
        docBuilder.EndRow()
    End Sub

    Private Sub insertNotes(docBuilder As DocumentBuilder, notesTable As Table, label As String, value As String)
        docBuilder.InsertCell()
        notesTable.ClearBorders()
        notesTable.PreferredWidth = PreferredWidth.Auto
        notesTable.Alignment = TableAlignment.Left
        docBuilder.Font.Bold = True
        docBuilder.Write(label)
        docBuilder.InsertCell()
        notesTable.Alignment = TableAlignment.Center
        docBuilder.Font.Bold = False
        docBuilder.Write(value)
        docBuilder.EndRow()
    End Sub

    Private Sub insertPlacement(docBuilder As DocumentBuilder, placementTable As Table, name As String, strDate As String, desc As String)
        docBuilder.Font.Bold = False
        docBuilder.InsertCell()
        placementTable.ClearBorders()
        placementTable.PreferredWidth = PreferredWidth.Auto
        placementTable.Alignment = TableAlignment.Center
        docBuilder.Write(name)
        docBuilder.InsertCell()
        placementTable.Alignment = TableAlignment.Center
        docBuilder.Write(strDate)
        docBuilder.InsertCell()
        placementTable.Alignment = TableAlignment.Center
        docBuilder.Write(desc)
        docBuilder.EndRow()
    End Sub

    Private Sub insertComment(docBuilder As DocumentBuilder, commentTable As Table, strDate As String, ownerName As String, comment As String)
        docBuilder.Font.Bold = False
        docBuilder.InsertCell()
        commentTable.ClearBorders()
        commentTable.PreferredWidth = PreferredWidth.Auto
        commentTable.Alignment = TableAlignment.Center
        docBuilder.Write(strDate)
        docBuilder.InsertCell()
        commentTable.Alignment = TableAlignment.Center
        docBuilder.Write(ownerName)
        docBuilder.InsertCell()
        commentTable.Alignment = TableAlignment.Center
        docBuilder.Write(comment)
        docBuilder.EndRow()
    End Sub

    Private Sub insertNewRowLeft(docBuilder As DocumentBuilder, newTable As Table, value As String)
        docBuilder.InsertCell()
        newTable.ClearBorders()
        newTable.PreferredWidth = PreferredWidth.Auto
        newTable.Alignment = TableAlignment.Left
        docBuilder.Write(value)
        docBuilder.EndRow()
    End Sub

    Private Sub insertNewRowCenter(docBuilder As DocumentBuilder, newTable As Table, value As String)
        docBuilder.InsertCell()
        newTable.ClearBorders()
        newTable.PreferredWidth = PreferredWidth.Auto
        newTable.Alignment = TableAlignment.Center
        docBuilder.Write(value)
        docBuilder.EndRow()
    End Sub

    Private Sub insertNewRowRight(docBuilder As DocumentBuilder, newTable As Table, value As String)
        docBuilder.InsertCell()
        newTable.ClearBorders()
        newTable.PreferredWidth = PreferredWidth.Auto
        newTable.Alignment = TableAlignment.Right
        docBuilder.Write(value)
        docBuilder.EndRow()
    End Sub

    Private Sub insertNewRowWithLabel(docBuilder As DocumentBuilder, newTable As Table, label As String, value As String)
        docBuilder.InsertCell()
        newTable.ClearBorders()
        newTable.PreferredWidth = PreferredWidth.Auto
        newTable.Alignment = TableAlignment.Left
        docBuilder.Font.Bold = True
        docBuilder.Write(label)
        docBuilder.InsertCell()
        newTable.Alignment = TableAlignment.Center
        docBuilder.Font.Bold = False
        docBuilder.Write(value)
        docBuilder.EndRow()
    End Sub

End Class

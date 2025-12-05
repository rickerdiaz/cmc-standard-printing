Imports System.Data.SqlClient
Imports System.Data
Imports System.Text

#Region "Class Header"
'Name               : clsPrintList
'Decription         : Manages PrintList and PrintListDetail Table
'Date Created       : 12.5.2005
'Author             : JRL
'Revision History   : 
#End Region

''' <summary>
''' Manages PrintList Table
''' </summary>
''' <remarks></remarks>

Public Class clsPrintList

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
#End Region

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
        '     ClearMarkings() 'items marked as not deleted
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

#Region "Private Methods"

#End Region
#Region "Delete Methods"

#End Region
#Region "Save Methods"


    Public Function UpdatePrintListByPrintProfileType(ByVal udtUser As structUser, ByVal strCodeList As String, ByVal intCodeSetPrice As Integer, ByVal blnCheckStatusOnly As Boolean, ByRef dtStatus As DataTable, _
                       ByVal intCodePrintProfileType As Integer, ByVal sortBy As enumPrintSortType, ByVal groupBy As enumPrintGroupType, ByVal SubRecipes As enumPrintSubRecipesOptions, ByVal dblYieldNew As Double, ByVal ShowToNewPageIfNewSupplier As Boolean, _
                       ByVal documentOutput As enumFileType, ByVal PrintQueue As Boolean, ByRef intCodePrintList As Integer, ByVal table As enumDbaseTables, _
                       Optional ByVal strCodeSiteList As String = "", Optional ByVal intListeType As Integer = 0, Optional ByVal intIDMain As Integer = 0) As enumEgswErrorCode
        'Dim intIDMain As Integer = -1

        'If strCodeList.Length > 5000 Then
        '    Dim clListe As New clsListe(enumAppType.WebApp, L_strCnn)
        '    intIDMain = clListe.fctSaveToTempList(strCodeList, udtUser.Code)
        'End If


        Dim arrParam(21) As SqlParameter
        arrParam(0) = New SqlParameter("@retval", SqlDbType.Int)
        arrParam(0).Direction = ParameterDirection.ReturnValue
        arrParam(1) = New SqlParameter("@vchCodeList", strCodeList)
        arrParam(2) = New SqlParameter("@CheckStatusOnly", blnCheckStatusOnly)
        arrParam(3) = New SqlParameter("@intCodePrintProfile", 0)
        arrParam(4) = New SqlParameter("@intCodeLang", udtUser.CodeLang)
        arrParam(5) = New SqlParameter("@intCodeTrans", udtUser.CodeTrans)
        arrParam(6) = New SqlParameter("@intCodeUser", udtUser.Code)
        arrParam(7) = New SqlParameter("@intCodeSetPrice", intCodeSetPrice)
        arrParam(8) = New SqlParameter("@intSortBy", sortBy)
        arrParam(9) = New SqlParameter("@intGroupBy", groupBy)
        arrParam(10) = New SqlParameter("@intSubRecipes", SubRecipes)
        arrParam(11) = New SqlParameter("@fltYieldNew", dblYieldNew)
        arrParam(12) = New SqlParameter("@ShowNewPageIfDifferentSupplier", ShowToNewPageIfNewSupplier)
        arrParam(13) = New SqlParameter("@intDocumentOuput", documentOutput)
        arrParam(14) = New SqlParameter("@PrintQueue", PrintQueue)
        arrParam(15) = New SqlParameter("@intCodePrintList", intCodePrintList)
        arrParam(15).Direction = ParameterDirection.InputOutput
        arrParam(16) = New SqlParameter("@intCodeEgsTable", table)
        arrParam(17) = New SqlParameter("@vchIncludeAllByCodeSiteList", strCodeSiteList)
        arrParam(18) = New SqlParameter("@vchIncludeAllByCodeCategoriesList", "")
        arrParam(19) = New SqlParameter("@tntIncludeAllListeType", intListeType)
        arrParam(20) = New SqlParameter("@intCodePrintProfileType", intCodePrintProfileType)
        arrParam(21) = New SqlParameter("@intIDMain", intIDMain)

        Try
            dtStatus = ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "sp_EgswPrintListUpdate", arrParam).Tables(0)
            intCodePrintList = CInt(arrParam(15).Value)
            Return CType(arrParam(0).Value, enumEgswErrorCode)

        Catch ex As Exception
            Throw ex
        End Try
    End Function


    ''' <summary>
    ''' Preview or Add Selected codes in print list
    ''' </summary>
    ''' <param name="udtUser"></param>
    ''' <param name="strCodeList"></param>
    ''' <param name="intCodeSetPrice"></param>
    ''' <param name="blnCheckStatusOnly"></param>
    ''' <param name="dtStatus"></param>
    ''' <param name="intCodePrintProfile"></param>
    ''' <param name="sortBy"></param>
    ''' <param name="groupBy"></param>
    ''' <param name="SubRecipes"></param>
    ''' <param name="dblYieldNew"></param>
    ''' <param name="ShowToNewPageIfNewSupplier"></param>
    ''' <param name="documentOutput"></param>
    ''' <param name="PrintQueue"></param>
    ''' <param name="intCodePrintList"></param>
    ''' <param name="strIncludeAllByCategoryList">Print all liste </param>
    ''' <param name="strIncludeAllBySiteCodeList"></param>
    ''' <param name="table"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' JBB 05.21.2012 Add intCodeSet for Nutrient Set
    ''' RDC 08.05.2013 Add intNutrientDisplay for displaying Imposed and/or Calculated Nutrient
    Public Function UpdatePrintList(ByVal udtUser As structUser, ByVal strCodeList As String, ByVal intCodeSetPrice As Integer, ByVal blnCheckStatusOnly As Boolean, ByRef dtStatus As DataTable, _
                   ByVal intCodePrintProfile As Integer, ByVal sortBy As enumPrintSortType, ByVal groupBy As enumPrintGroupType, ByVal SubRecipes As enumPrintSubRecipesOptions, ByVal dblYieldNew As Double, ByVal ShowToNewPageIfNewSupplier As Boolean, _
                   ByVal documentOutput As enumFileType, ByVal PrintQueue As Boolean, ByRef intCodePrintList As Integer, ByVal table As enumDbaseTables, Optional ByVal strIncludeAllBySiteCodeList As String = "", Optional ByVal strIncludeAllByCategoryList As String = "", Optional ByVal includeAllListeType As enumDataListItemType = enumDataListItemType.NoType, _
                   Optional ByVal blnMode As Boolean = False, Optional ByVal intCodeSet As Integer = 0, Optional intNutrientDisplay As Integer = 0, Optional intListeType As Integer = 0, Optional ByVal intIDMain As Integer = 0, _
                   Optional ByVal intMP As Integer = 0) As enumEgswErrorCode

        'Dim intIDMain As Integer = -1

        'If strCodeList.Length > 5000 Then
        '    Dim clListe As New clsListe(enumAppType.WebApp, L_strCnn)
        '    intIDMain = clListe.fctSaveToTempList(strCodeList, udtUser.Code)
        'End If

        ' RDC 08.06.2013 : Changed arrParam(23) to arrParam(24)
        Dim arrParam(26) As SqlParameter
        arrParam(0) = New SqlParameter("@retval", SqlDbType.Int)
        arrParam(0).Direction = ParameterDirection.ReturnValue
        arrParam(1) = New SqlParameter("@vchCodeList", strCodeList)
        arrParam(2) = New SqlParameter("@CheckStatusOnly", blnCheckStatusOnly)
        arrParam(3) = New SqlParameter("@intCodePrintProfile", intCodePrintProfile)
        arrParam(4) = New SqlParameter("@intCodeLang", udtUser.CodeLang)
        arrParam(5) = New SqlParameter("@intCodeTrans", udtUser.CodeTrans)
        arrParam(6) = New SqlParameter("@intCodeUser", udtUser.Code)
        arrParam(7) = New SqlParameter("@intCodeSetPrice", intCodeSetPrice)
        arrParam(8) = New SqlParameter("@intSortBy", sortBy)
        arrParam(9) = New SqlParameter("@intGroupBy", groupBy)
        arrParam(10) = New SqlParameter("@intSubRecipes", SubRecipes)
        arrParam(11) = New SqlParameter("@fltYieldNew", dblYieldNew)
        arrParam(12) = New SqlParameter("@ShowNewPageIfDifferentSupplier", ShowToNewPageIfNewSupplier)
        arrParam(13) = New SqlParameter("@intDocumentOuput", documentOutput)
        arrParam(14) = New SqlParameter("@PrintQueue", PrintQueue)
        arrParam(15) = New SqlParameter("@intCodePrintList", intCodePrintList)
        arrParam(15).Direction = ParameterDirection.InputOutput
        arrParam(16) = New SqlParameter("@intCodeEgsTable", table)
        arrParam(17) = New SqlParameter("@vchIncludeAllByCodeSiteList", strIncludeAllBySiteCodeList)
        arrParam(18) = New SqlParameter("@vchIncludeAllByCodeCategoriesList", strIncludeAllByCategoryList)
        arrParam(19) = New SqlParameter("@tntIncludeAllListeType", includeAllListeType)
        arrParam(20) = New SqlParameter("@intCodePrintProfileType", 0)
        arrParam(21) = New SqlParameter("@intIDMain", intIDMain)
        arrParam(22) = New SqlParameter("@bitMode", blnMode)
        arrParam(23) = New SqlParameter("@intCodeSet", intCodeSet)
        arrParam(24) = New SqlParameter("@intNutrientDisplay", intNutrientDisplay)  ' RDC 08.05.2013
        arrParam(25) = New SqlParameter("@MPShoppingList", intListeType)
        arrParam(26) = New SqlParameter("@MPMasterPlan", intMP)

        Try
            dtStatus = ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "sp_EgswPrintListUpdate", arrParam).Tables(0)
            intCodePrintList = CInt(arrParam(15).Value)
            Return CType(arrParam(0).Value, enumEgswErrorCode)

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Sub MergeYieldToPrint(ByVal strCodeMergedList As String)
        UpdateYieldToPrint(enumEgswTransactionMode.MergeDelete, strCodeMergedList, L_udtUser.Code, _
                        0, 0, 0, 0, 0, False)
    End Sub
    Public Sub DeleteAllYieldToPrint()
        UpdateYieldToPrint(enumEgswTransactionMode.DeleteAll, "", L_udtUser.Code, 0, 0, 0, 0, 0, False)
    End Sub

    Public Sub UpdateYieldToPrint(ByVal bitToMerge As Boolean, ByVal dblYield As Double, ByVal intCodeListe As Integer, ByVal intCodeListeMain As Integer, _
                                 ByVal intCodeListeParent As Integer, ByVal intPercentage As Integer, _
                                 Optional ByVal intYieldUnit As Integer = 0, Optional ByVal strCourseName As String = "", Optional ByVal Position As Integer = 0, Optional ByVal masterPlan As Integer = 0)
        'UpdateYieldToPrint(enumEgswTransactionMode.Edit, "", L_udtUser.Code, _
        '    dblYield, intCodeListe, intCodeListeMain, intCodeListeParent, intPercentage, False)
        UpdateYieldToPrint(enumEgswTransactionMode.Edit, "", L_udtUser.Code, _
            dblYield, intCodeListe, intCodeListeMain, intCodeListeParent, intPercentage, bitToMerge, intYieldUnit, strCourseName, Position, masterPlan) 'VRP 21.01.2009
    End Sub

    Private Function UpdateYieldToPrint(ByVal tntTranMode As enumEgswTransactionMode, ByVal strCodeList As String, _
                                        ByVal intCodeUser As Integer, ByVal dblYield As Double, ByVal intCodeListe As Integer, _
                                        ByVal intCodeListeMain As Integer, ByVal intcodeListeParent As Integer, _
                                        ByVal intPercentage As Integer, ByVal bitToMerge As Boolean, _
                                        Optional ByVal intYieldUnit As Integer = 0, Optional ByVal strCourseName As String = "", _
                                        Optional ByVal Position As Integer = 0, Optional ByVal masterPlan As Integer = 0) As enumEgswErrorCode

        Dim arrParam(12) As SqlParameter
        arrParam(0) = New SqlParameter("@tntTranMode", tntTranMode)
        arrParam(1) = New SqlParameter("@vchCodeList", strCodeList)
        arrParam(2) = New SqlParameter("@intCodeUser", intCodeUser)
        arrParam(3) = New SqlParameter("@fltYield", dblYield)
        arrParam(4) = New SqlParameter("@intCodeListe", intCodeListe)
        arrParam(5) = New SqlParameter("@intCodeListeMain", intCodeListeMain)
        arrParam(6) = New SqlParameter("@intCodeListeParent", intcodeListeParent)
        arrParam(7) = New SqlParameter("@intPercentage", intPercentage)
        arrParam(8) = New SqlParameter("@bitToMerge", bitToMerge)
        arrParam(9) = New SqlParameter("@intYieldUnit", intYieldUnit) 'VRP 21.01.2009
        arrParam(10) = New SqlParameter("@nvcCourseName", strCourseName) 'VRP 07.07.2009
        arrParam(11) = New SqlParameter("@intPosition", Position)
        arrParam(12) = New SqlParameter("@MPMasterplan", masterPlan)

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_egswYieldToPrintUpdate", arrParam)
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function PurgeYieldToPrintMerged() As enumEgswErrorCode
        Dim arrParam() As SqlParameter = {New SqlParameter("@intCodeUser ", L_udtUser.Code)}
        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_egswYieldToPrintPurgeMerged", arrParam)
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function


#End Region
#Region "Get Methods"

    ''' <summary>+
    ''' Returns list of details of a given code
    ''' </summary>
    ''' <param name="intCodePrintList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetListDetails(ByVal intCodePrintList As Integer, _
           Optional ByVal DeleteAfterFetch As Boolean = True, _
           Optional ByVal Supermarket As Integer = 1, _
           Optional ByVal intKiosk As Integer = -1, _
           Optional ByVal intPictureList As Integer = 0, _
           Optional ByVal bForMoevenpickExport As Boolean = False, _
           Optional ByVal intCodeSet As Integer = -1, _
           Optional ByVal intCodeSite As Integer = -1, _
           Optional ByVal dataListeType As Integer = 0) As DataSet
        'intKiosk: Manor=1 ;  Else=-1
        'intPictureList: Original=0 ; Thumbnails=1

        Dim strCommandText As String
        Dim arrParam(4) As SqlParameter
        Dim arrParamManor(3) As SqlParameter
        Dim arrParamwithCodeSet(4) As SqlParameter
        If bForMoevenpickExport Then
            strCommandText = "sp_EgswPrintListGetListDetails"
            arrParam(0) = New SqlParameter("@intCodePrintList", intCodePrintList)
            arrParam(1) = New SqlParameter("@blnDeleteAfterFetch", DeleteAfterFetch)
            arrParam(2) = New SqlParameter("@blnBestUnitConversion", True)
            'arrParam(3) = New SqlParameter("@intPictureList", Nothing)
        ElseIf intKiosk = 1 Then 'RDTC 31.08.2007
            strCommandText = "sp_EgswPrintListGetListDetailsManor"
            arrParamManor(0) = New SqlParameter("@intCodePrintList", intCodePrintList)
            arrParamManor(1) = New SqlParameter("@blnDeleteAfterFetch", DeleteAfterFetch)
            arrParamManor(2) = New SqlParameter("@blnBestUnitConversion", L_udtUser.UseBestUnit)
            arrParamManor(3) = New SqlParameter("@SuperMarketOrRestaurant", Supermarket)

        ElseIf intKiosk = -1 Then
            strCommandText = "sp_EgswPrintListGetListDetails"
            arrParam(0) = New SqlParameter("@intCodePrintList", intCodePrintList)
            arrParam(1) = New SqlParameter("@blnDeleteAfterFetch", DeleteAfterFetch)
            arrParam(2) = New SqlParameter("@blnBestUnitConversion", L_udtUser.UseBestUnit)
            arrParam(3) = New SqlParameter("@intPictureList", intPictureList) 'VRP 17.03.2008
            arrParam(4) = New SqlParameter("@intNutCodeSet", intCodeSet) 'JTOC 11.13.2013 
            'ElseIf intKiosk = 2 Then 'VRP 07.03.2008
            '    strCommandText = "sp_EgswListeIngredientsGetComputedMigrosKiosk"
            '    arrParam(0) = New SqlParameter("@intCodeRecipe", intCodePrintList)
            '    arrParam(1) = New SqlParameter("@intCodeTrans", DeleteAfterFetch)
            '    arrParam(2) = New SqlParameter("@blnBestUnitConversion", L_udtUser.UseBestUnit)

        ElseIf intKiosk = 2 Then 'Recipe Center 'VRP 27.08.2008
            strCommandText = "sp_EgswPrintListGetListDetailsRC"
            arrParam(0) = New SqlParameter("@intCodePrintList", intCodePrintList)
            arrParam(1) = New SqlParameter("@blnDeleteAfterFetch", DeleteAfterFetch)
            arrParam(2) = New SqlParameter("@blnBestUnitConversion", L_udtUser.UseBestUnit)
        End If

        Try
            Select Case intKiosk
                Case 1
                    Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParamManor, 60000)
                Case -1, 2
                    Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam, 60000)
            End Select
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''IAA 06.10.2016
    Public Function GetReportLangISO(ByVal CodeTrans As Integer) As String
        Try
            Dim ds As New DataSet

            Using cmd As New SqlCommand
                With cmd
                    Using cn As New SqlConnection(ConnectionString)
                        Try
                            .Connection = cn
                            .CommandType = CommandType.StoredProcedure
                            .CommandText = "[dbo].[API_GET_Report_Catalog_LangISO]"
                            .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = CodeTrans
                            cn.Open()
                            Dim _da As New SqlDataAdapter(cmd)
                            _da.Fill(ds)

                            Return ds.Tables(0).Rows(0).Item("LangISO")

                        Finally
                            If Not cn Is Nothing Then
                                cn.Close()
                                CType(cn, IDisposable).Dispose()
                            End If
                        End Try

                    End Using
                End With
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    ''

    Public Function GetLastPrintedYield(ByVal intCodeUser As Integer, ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer) As DataTable 'PJRB 2016.10.07
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim lastPrintYield As Double = 0

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "GET_LastPrintedYield"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Connection.Open()
                '.ExecuteNonQuery()
                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                cmd.Dispose()

                If dt.Rows.Count > 0 Then
                    lastPrintYield = CDblDB(dt(0)(0))
                End If

                Return dt
            End With
        Catch ex As Exception
            cmd.Dispose()
            GetLastPrintedYield = Nothing
        End Try
    End Function

#End Region


#Region "Kiosk"

    Public Function UpdateKioskYieldtoPrint(ByVal udtUser As structUser, ByVal strCodeList As String) As Boolean
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim sbSQL As New StringBuilder

        With sbSQL
            .Append("Delete egswYieldToPrint Where Codeuser = " & udtUser.Code & vbCrLf)
            .Append("INSERT INTO egswYieldToPrint " & vbCrLf)
            .Append("(CodeListe, CodeUser, CodeListeParent, CodeListeMain, Yield, Percentage, ToMerge) " & vbCrLf)
            .Append("Select Code, " & udtUser.Code & ", Null, Code, Yield, [Percent],0 " & vbCrLf)
            .Append("from egswliste" & vbCrLf)
            .Append("Where Code in " & strCodeList)
        End With

        Try
            With cmd
                .Connection = cn
                .Connection.Open()
                .CommandText = sbSQL.ToString
                .CommandType = CommandType.Text
                .ExecuteNonQuery()
                .Connection.Close()
            End With

            Return True

        Catch ex As Exception
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
            Return False
        End Try

    End Function

    Public Function GetListDetailsMoevenPick(ByVal intCodePrintList As Integer, ByVal blnUseBestUnit As Boolean) As DataSet 'VRP 18.12.2008

        Dim strCommandText As String
        Dim arrParam(1) As SqlParameter

        strCommandText = "sp_EgswPrintDetailMoevenPick"
        arrParam(0) = New SqlParameter("@intCodePrintList", intCodePrintList)
        arrParam(1) = New SqlParameter("@blnBestUnitConversion", blnUseBestUnit)

        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam, 9000)
        Catch ex As Exception
            Throw ex
        End Try
    End Function
#End Region

End Class

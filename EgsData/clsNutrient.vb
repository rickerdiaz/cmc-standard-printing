Imports System.Data.SqlClient
Imports System.Data
Public Class clsNutrient

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

#Region "Private Methods"
    ''' <summary>
    ''' Update Nutrient List
    ''' </summary>
    ''' <param name="intNutr_No"></param>
    ''' <param name="strName"></param>
    ''' <param name="strFormat"></param>
    ''' <param name="intCodeUser"></param>
    ''' <param name="intCodeSite"></param>
    ''' <param name="TranMode"></param>
    ''' <param name="intnutrientdbcode"></param>
    ''' <returns></returns>
    ''' <remarks>Add,Edit,Delete,MoveUp,MoveDown,Fix Order Position</remarks>
    Private Function SaveNutrientToList(ByVal intNutr_No As Integer, ByVal strName As String, ByVal strFormat As String, _
                                        ByVal intCodeUser As Integer, ByVal intCodeSite As Integer, _
                                        ByVal TranMode As enumEgswTransactionMode, ByVal intnutrientdbcode As Integer, _
                                        ByVal dblGDA As Double, Optional ByVal blnCalcOnline As Boolean = False, Optional ByVal intCodeSet As Integer = -1) As enumEgswErrorCode
        Try
            Dim arrParam(10) As SqlParameter
            arrParam(0) = New SqlParameter("@retVal", "")
            arrParam(0).Direction = ParameterDirection.ReturnValue
            arrParam(1) = New SqlParameter("@tntTranMode", TranMode)
            arrParam(2) = New SqlParameter("@nvcName", strName)
            arrParam(3) = New SqlParameter("@nvcFormat", strFormat)
            arrParam(4) = New SqlParameter("@intCodeUser", intCodeUser)
            arrParam(5) = New SqlParameter("@intCodeSite", intCodeSite)
            arrParam(6) = New SqlParameter("@intNutr_No", intNutr_No)
            arrParam(7) = New SqlParameter("@intNutrientDBCode", intnutrientdbcode)
            arrParam(8) = New SqlParameter("@fltGDA", dblGDA)
            arrParam(9) = New SqlParameter("@IsCalcmenuOnline", blnCalcOnline)
            arrParam(10) = New SqlParameter("@intCodeSet", intCodeSet)

            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswNutrientUpdate", arrParam)
            Return CType(arrParam(0).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try

    End Function

#End Region

#Region "Get Methods"
    ''' <summary>
    ''' Get List of selected Nutrients and Nutrient Databases
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(Optional ByVal intCodeTrans As Integer = -1, Optional ByVal intCodeSite As Integer = -1, Optional intCodeSet As Integer = 0) As DataSet
        Dim strCommandText As String = "sp_EgswNutrientGetList"
        Try
            Dim arrParam(3) As SqlParameter
            arrParam(0) = New SqlParameter("@intCodeNutrientDB", -1)
            arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans) 'VRP 07.01.2009
            arrParam(2) = New SqlParameter("@intCodeSite", intCodeSite) 'VRP 08.01.2009
            arrParam(3) = New SqlParameter("@intCodeSet", intCodeSet) 'JBB 07.08.2013

            Return CType(MyBase.ExecuteFetchType(enumEgswFetchType.DataSet, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam), DataSet)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Overloads Function GetListDT(Optional ByVal intCodeTrans As Integer = -1, Optional ByVal intCodeSite As Integer = -1, Optional intCodeSet As Integer = 0) As DataTable
        Dim strCommandText As String = "sp_EgswNutrientGetList"
        Try
            Dim arrParam(3) As SqlParameter
            arrParam(0) = New SqlParameter("@intCodeNutrientDB", -1)
            arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans) 'VRP 07.01.2009
            arrParam(2) = New SqlParameter("@intCodeSite", intCodeSite) 'VRP 08.01.2009
            arrParam(3) = New SqlParameter("@intCodeSet", intCodeSet) 'JBB 07.08.2013

            Return CType(MyBase.ExecuteFetchType(enumEgswFetchType.DataTable, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam), DataTable)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    'Public Overloads Function GetList(ByVal intCodeTrans As Integer) As DataSet
    '    Dim strCommandText As String = "sp_EgswNutrientGetList"
    '    Try
    '        Dim arrParam(1) As SqlParameter
    '        arrParam(0) = New SqlParameter("@intCodeNutrientDB", -1)

    '        Return CType(MyBase.ExecuteFetchType(enumEgswFetchType.DataSet, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam), DataSet)
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Function
    Public Overloads Function GetDisclaimer(ByVal intCodeNutrientDB As Integer, ByVal intCodeTrans As Integer) As DataTable 'KMQDC 4.23.2015
        Dim strCommandText As String = "sp_EgswGetDisclaimer"
        Try
            Dim arrParam(1) As SqlParameter
            arrParam(0) = New SqlParameter("@CodeNutrient", intCodeNutrientDB)
            arrParam(1) = New SqlParameter("@CodeTrans", intCodeTrans) 'VRP 07.01.2009
            Return CType(MyBase.ExecuteFetchType(enumEgswFetchType.DataTable, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam), DataTable)
        Catch ex As Exception
            Throw ex
        End Try
    End Function


    Public Overloads Function GetListGDA(Optional ByVal intCodeTrans As Integer = -1, Optional ByVal intCodeSite As Integer = -1, Optional ByVal intCodeSet As Integer = 0) As DataSet
        Dim strCommandText As String = "sp_EgswNutrientGetList"
        Try
            Dim arrParam(4) As SqlParameter
            arrParam(0) = New SqlParameter("@intCodeNutrientDB", -1)
            arrParam(1) = New SqlParameter("@bitGDAOnly", True)
            arrParam(2) = New SqlParameter("@intCodeTrans", intCodeTrans) 'VRP 07.01.2009
            arrParam(3) = New SqlParameter("@intCodeSite", intCodeSite) 'VRP 07.01.2009
            arrParam(4) = New SqlParameter("@intCodeSet", intCodeSet) 'DRR 07.09.2013
            Return CType(MyBase.ExecuteFetchType(enumEgswFetchType.DataSet, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam), DataSet)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    ''' <summary>
    ''' Get List of nutrients of a Nutrient Database
    ''' </summary>
    ''' <param name="intCodeNutrientDB"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetListDB(ByVal intCodeNutrientDB As Integer, Optional ByVal intCodeTrans As Integer = -1) As DataTable 'VRP 08.09.2009
        Dim strCommandText As String = "sp_EgswNutrientGetList"
        Try
            Dim arrParam(1) As SqlParameter
            arrParam(0) = New SqlParameter("@intCodeNutrientDB", intCodeNutrientDB)
            arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans) 'VRP 07.01.2009
            Return CType(MyBase.ExecuteFetchType(enumEgswFetchType.DataTable, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam), DataTable)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    'Public Overloads Function GetList(ByVal intCodeNutrientDB As Integer, Optional ByVal intCodeTrans As Integer = -1) As DataTable
    '    Dim strCommandText As String = "sp_EgswNutrientGetList"
    '    Try
    '        Dim arrParam(1) As SqlParameter
    '        arrParam(0) = New SqlParameter("@intCodeNutrientDB", intCodeNutrientDB)
    '        arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans) 'VRP 07.01.2009
    '        Return CType(MyBase.ExecuteFetchType(enumEgswFetchType.DataTable, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam), DataTable)
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Function

    Public Function fctGetNutrientTrans(ByVal intCodeTrans As Integer) As DataTable
        Dim cmd As New SqlCommand
        Dim cn As New SqlConnection(L_strCnn)
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = cn
                .CommandText = "SELECT N.Nutr_No, " & _
                               "CASE WHEN NT.Name IS NULL OR LEN(LTRIM(RTRIM(NT.Name))) = 0 THEN N.Name ELSE NT.Name END Name, " & _
                               "N.Format, D.Units " & _
                               "FROM EgswNutrientDef N " & _
                               "INNER JOIN Egsw_NUTR_DEF D ON N.Nutr_No=D.Nutr_No " & _
                               "LEFT OUTER JOIN EgswNutrientDefTrans NT ON N.Nutr_No = NT.CodeMain AND NT.CodeTrans = " & intCodeTrans
                .CommandType = CommandType.Text
                .Connection.Open()
                .ExecuteNonQuery()

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                .Connection.Dispose()
            End With
            Return dt
        Catch ex As Exception
            Return Nothing
            cmd.Connection.Dispose()
        End Try
    End Function

    Public Function fctGetNutrientDEFTrans(ByVal intCodeTrans As Integer) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Dim strText As String
        If intCodeTrans = -1 Then
            strText = "SELECT n.Nutr_No, " & vbCrLf & _
                     "nt.CodeTrans, " & vbCrLf & _
                     "CASE WHEN nt.Name=NULL OR LEN(LTRIM(RTRIM(nt.Name)))=0 THEN n.Name ELSE ISNULL(nt.Name, n.Name) END Name, " & vbCrLf & _
                     "n.Position " & vbCrLf & _
                     "FROM egswNutrientDef n " & vbCrLf & _
                     "LEFT OUTER JOIN EgswNutrientDefTrans nt ON n.Nutr_No=nt.CodeMain " & vbCrLf & _
                     "ORDER BY n.Position "
        Else
            strText = "SELECT n.Nutr_No, " & vbCrLf & _
                     "nt.CodeTrans, " & vbCrLf & _
                     "CASE WHEN nt.Name=NULL OR LEN(LTRIM(RTRIM(nt.Name)))=0 THEN n.Name ELSE ISNULL(nt.Name, n.Name) END Name, " & vbCrLf & _
                     "n.Position " & vbCrLf & _
                     "FROM egswNutrientDef n " & vbCrLf & _
                     "LEFT OUTER JOIN EgswNutrientDefTrans nt ON n.Nutr_No=nt.CodeMain " & vbCrLf & _
                     "AND CodeTrans=" & intCodeTrans & " " & vbCrLf & _
                     "ORDER BY n.Position "
        End If


        Try
            With cmd
                .Connection = cn
                .CommandText = strText
                .CommandType = CommandType.Text
                .Connection.Open()
                .ExecuteNonQuery()

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                .Connection.Dispose()
            End With
            Return dt
        Catch ex As Exception
            cmd.Connection.Dispose()
            Return Nothing
        End Try
    End Function

    Public Function GetNutrientDefList(Optional ByVal intCodeTrans As Integer = -1, Optional ByVal intCodeSite As Integer = 0) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        ' RDC 10.30.2013 : Reconstruct query for acquiring nutrient list due to redundant output. Forcing the application to throw an exception.
        Try
            If intCodeTrans < 0 Then intCodeTrans = 1
            Dim sqlQry As String = "Select Case Len(IsNull(b.[Name],'')) When 0 Then a.[Name] Else b.[Name] End As [Name], " & vbCrLf & _
                                          "e.[Name] As DatabaseName, a.Format, c.TagName, a.CodeSite " & vbCrLf & _
                                   "From           EgswNutrientDef      As a " & vbCrLf & _
                                       "Left  Join EgswNutrientDefTrans As b On a.Nutr_No = b.CodeMain And a.CodeSet = b.CodeSet " & vbCrLf & _
                                       "Left  Join Egsw_Nutr_Def        As c On a.Nutr_No = c.Nutr_No " & vbCrLf & _
                                       "Inner Join EgswNutrientSet      As d On a.CodeSet = d.Code " & vbCrLf & _
                                       "Inner Join EgswNutrientDb       As e On d.NutrientDBCode = e.Code " & vbCrLf & _
                                   "Where a.CodeSite = " & intCodeSite & " And a.CodeSet = 0 And b.CodeTrans = " & intCodeTrans & vbCrLf & _
                                   "Order By a.Position ASC"

            da = New SqlDataAdapter(sqlQry, cn)
            da.Fill(dt)
        Catch ex As Exception

        End Try


        'Try
        '    Dim strText As String = "SELECT CASE WHEN Dt.Name=NULL OR LEN(LTRIM(RTRIM(Dt.Name)))=0 THEN D.Name ELSE ISNULL(Dt.Name,D.Name) END Name, " & _
        '                       "DB.Name as DatabaseName, D.Format,DF.TagName, D.CodeSite " & _
        '                       "FROM EgswNutrientDEF D " & _
        '                       "INNER JOIN EgsW_Nutr_DEF DF ON DF.nutr_no = D.nutr_no " & _
        '                       "INNER JOIN EgsWNutrientDB DB ON DB.Code = DF.nutrientdbcode " & _
        '                       "LEFT OUTER JOIN EgswNutrientDEFTRANS Dt ON D.Nutr_No=Dt.CodeMain AND Dt.CodeTrans=" & intCodeTrans & " " & _
        '                       "AND D.CodeSite=Dt.CodeSite " & _
        '                       "WHERE D.CodeSite=" & intCodeSite & _
        '                       "ORDER BY D.Position ASC "
        '    Return CType(ExecuteFetchType(enumEgswFetchType.DataTable, L_strCnn, CommandType.Text, strText), DataTable)
        'Catch ex As Exception
        '    Throw ex
        'End Try
        'cn.Dispose()

        'Try
        '    With cmd
        '        Dim arrParam(1) As SqlParameter
        '        arrParam(0) = New SqlParameter("@CodeTrans", intCodeTrans)
        '        arrParam(1) = New SqlParameter("@CodeSite", intCodeSite)
        '        Return CType(ExecuteFetchType(enumEgswFetchType.DataTable, L_strCnn, CommandType.StoredProcedure, "GET_NUTRIENTLIST", arrParam), DataTable)
        '    End With
        'Catch ex As Exception

        'End Try
        Return dt

    End Function
    '//LD20160930 Get Nutrient Header use in export of merchandise with nutrients
    Public Function GetNutrientHeaderForExport(Optional ByVal CodeUser As Integer = 0) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Try
            Dim sqlQry As String = "GET_NutrientsExportHeader " & CodeUser.ToString()
            da = New SqlDataAdapter(sqlQry, cn)
            da.Fill(dt)
        Catch ex As Exception
        End Try
        Return dt
    End Function

    'VRP 03.01.2008
    'Public Function GetNutrientDefListReader(Optional ByVal intCodeTrans As Integer = -1, Optional ByVal intCodeSite As Integer = 0) As SqlDataReader
    '    Dim cn As New SqlConnection(L_strCnn)
    '    Dim cmd As New SqlCommand
    '    Dim dr As SqlDataReader

    '    Try
    '        With cmd
    '            .Connection = cn
    '            .CommandText = "SELECT CASE WHEN Dt.Name=NULL OR LEN(LTRIM(RTRIM(Dt.Name)))=0 THEN D.Name ELSE Dt.Name END Name, " & _
    '                           "DB.Name as DatabaseName, D.Format,DF.TagName " & _
    '                           "FROM EgswNutrientDEF D " & _
    '                           "INNER JOIN EgsW_Nutr_DEF DF ON DF.nutr_no = D.nutr_no " & _
    '                           "INNER JOIN EgsWNutrientDB DB ON DB.Code = DF.nutrientdbcode " & _
    '                           "LEFT OUTER JOIN EgswNutrientDEFTRANS Dt ON D.Nutr_No=Dt.CodeMain AND Dt.CodeTrans=" & intCodeTrans & " " & _
    '                           "AND D.CodeSite=Dt.CodeSite " & _
    '                           "WHERE D.CodeSite=" & intCodeSite & _
    '                           "ORDER BY D.Position ASC "
    '            .CommandType = CommandType.Text
    '        End With
    '        cn.Open()
    '        dr = cmd.ExecuteReader
    '        Return dr
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    '    cn.Dispose()
    'End Function



    ''' <summary>
    ''' Get List by Nutrient name
    ''' </summary>
    ''' <param name="strNutrientRefName"></param>
    ''' <param name="strNutrDB"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function GetList(ByVal strNutrientRefName As String, ByVal strNutrDB As String) As Object
        Dim strCommandText As String = "sp_EgswNutrientGetList"
        Try
            Dim arrParam(2) As SqlParameter
            arrParam(0) = New SqlParameter("@intCodeNutrientDB", -1)
            If strNutrientRefName <> "" Then
                arrParam(1) = New SqlParameter("@nvcNutrientRefName", strNutrientRefName)
            End If
            If strNutrDB <> "" Then
                arrParam(2) = New SqlParameter("@nvcNutrDB", strNutrDB)
            End If
            Return CType(MyBase.ExecuteFetchType(enumEgswFetchType.DataReader, L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam), SqlDataReader)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    'Public Function GetRecipeNutrientFactor(ByVal lngRecipeCode As Long) As Double
    '    Dim cn As New SqlConnection(L_strCnn)
    '    Dim cmd As New SqlCommand

    '    Dim lngYieldUnit As Long
    '    Dim lngSrLevel As Long
    '    Dim KiloCode As Long
    '    Dim LiterCode As Long
    '    Dim lngMainUnitCode As Long
    '    Dim dblUnitfactor As Double
    '    Dim dblSubUnitfactor As Double
    '    Dim dr As SqlDataReader
    '    Dim dblSrQty As Double
    '    Dim dblYieldQty As Double


    '    Try
    '        '
    '        With cmd
    '            .Connection = cn
    '            .CommandText = "SELECT L.Code, L.Type, EgsWUnit.Code as YieldUnit,l.SrUnit as SubRecipeUnitCode,l.srlevel, " & _
    '                            "LiterCode = (select top 1 Code from EgsWUnit where type=100 order by code), " & _
    '                            "KiloCode = (select top 1 Code from EgsWUnit where type=200 order by code),  " & _
    '                            "SubRUnitMainCode = (select top 1 Code from EgsWUnit where type=SubUnit.type order by code), " & _
    '                            "SubRUnitFactor = SubUnit.Factor,l.SrQty ,L.Yield as YieldQty  " & _
    '                            "FROM EgsWListe l  " & _
    '                            "LEFT OUTER JOIN EgsWUnit ON EgsWUnit.code = l.yieldUnit " & _
    '                            "LEFT OUTER JOIN EgsWUnit SubUnit ON SubUnit.Code = l.SrUnit " & _
    '                            "WHERE l.Code = @p_nCode "
    '            .CommandType = CommandType.Text
    '            .Parameters.Add("@p_nCode", SqlDbType.Int).Value = lngRecipeCode
    '        End With
    '        cn.Open()
    '        dr = cmd.ExecuteReader
    '        '
    '        If dr.Read Then
    '            lngYieldUnit = CLng(dr.Item("YieldUnit"))
    '            lngSrLevel = CLng(dr.Item("srlevel"))
    '            KiloCode = CLng(dr.Item("KiloCode"))
    '            LiterCode = CLng(dr.Item("LiterCode"))
    '            lngMainUnitCode = CLng(dr.Item("SubRUnitMainCode"))
    '            dblSubUnitfactor = CDbl(dr.Item("SubRUnitFactor"))
    '            dblSrQty = CDbl(dr.Item("SrQty"))
    '            dblYieldQty = CDbl(dr.Item("YieldQty"))
    '        End If
    '        cn.Close()
    '        cn.Dispose()
    '        cmd.Dispose()
    '        '
    '        '---------- Get Unit Factor -------------------
    '        dblUnitfactor = fctGetRecipeNutrientFactor(lngYieldUnit, dblSrQty, lngSrLevel, KiloCode, LiterCode, lngMainUnitCode, dblSubUnitfactor)

    '        Return dblUnitfactor
    '    Catch ex As Exception
    '        'Throw ex
    '    End Try
    'End Function

    'VRP 11.02.2008
    Public Function GetRecipeNutrientFactor(ByVal lngRecipeCode As Long, Optional ByVal lngCodeSetPrice As Long = 1) As Double
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        Dim lngYieldUnit As Long
        Dim lngSrLevel As Long
        Dim KiloCode As Long
        Dim LiterCode As Long
        Dim lngMainUnitCode As Long
        Dim dblUnitfactor As Double
        Dim dblSubUnitfactor As Double
        Dim dr As SqlDataReader
        Dim dblSrQty As Double
        Dim dblYieldQty As Double
        Dim intType As Integer
        Dim dblComputedWeightKilo As Double
        Dim SubRUnitMainType As Long
        Dim Yield2MainType As Long


        Try
            '
            With cmd
                .Connection = cn
                .CommandText = "SELECT L.Code, L.Type, EgsWUnit.Code as YieldUnit,l.SrUnit as SubRecipeUnitCode,l.srlevel, " & _
                                "LiterCode = (select top 1 Code from EgsWUnit where type=200 order by code), " & _
                                "KiloCode = (select top 1 Code from EgsWUnit where type=100 order by code),  " & _
                                "SubRUnitMainCode = (select top 1 Code from EgsWUnit where type=SubUnit.type order by code), " & _
                                "SubRUnitMainType = (select top 1 typemain from EgsWUnit where type=SubUnit.type order by code), " & _
                                "SubRUnitFactor = SubUnit.Factor,SrQty = (case when EgsWUnit.Code in ((select top 1 Code from EgsWUnit where type=200 order by code),(select top 1 Code from EgsWUnit where type=100 order by code)) then l.srqty else case when EgsWUnit2.Code in ((select top 1 Code from EgsWUnit where type=200 order by code),(select top 1 Code from EgsWUnit where type=100 order by code)) then l.yield2 / (l.srqty * l.Yield) else l.srqty end end), " & _
                                "Yield2MainType = (select top 1 typemain from EgsWUnit where type=yield2unit.type order by code), L.Yield as YieldQty, dbo.fn_EgsWGETRecipeWeightActual(L.Code,@p_nCodeSetPrice) as ComputedWeightKilo  " & _
                                "FROM EgsWListe l  " & _
                                "LEFT OUTER JOIN EgsWUnit EgsWUnit ON EgsWUnit.code = l.yieldUnit " & _
                                "LEFT OUTER JOIN EgsWUnit EgsWUnit2 ON EgsWUnit2.code = l.yieldUnit2 " & _
                                "LEFT OUTER JOIN EgsWUnit SubUnit ON SubUnit.Code = l.SrUnit " & _
                                "LEFT OUTER JOIN EgsWUnit yield2unit ON yield2unit.Code = l.YieldUnit2 " & _
                                "WHERE l.Code = @p_nCode "
                .CommandType = CommandType.Text
                .Parameters.Add("@p_nCode", SqlDbType.Int).Value = lngRecipeCode
                .Parameters.Add("@p_nCodeSetPrice", SqlDbType.Int).Value = lngCodeSetPrice
            End With
            If cn.State = ConnectionState.Closed Then
                cn.Open()
            End If
            dr = cmd.ExecuteReader
            '
            If dr.Read Then
                lngYieldUnit = CLng(dr.Item("YieldUnit"))
                lngSrLevel = CLng(dr.Item("srlevel"))
                KiloCode = CLng(dr.Item("KiloCode"))
                LiterCode = CLng(dr.Item("LiterCode"))
                lngMainUnitCode = CLng(dr.Item("SubRUnitMainCode"))
                dblSubUnitfactor = CDbl(dr.Item("SubRUnitFactor"))
                dblSrQty = CDbl(dr.Item("SrQty"))
                dblYieldQty = CDbl(dr.Item("YieldQty"))
                intType = CInt(dr.Item("Type"))
                dblComputedWeightKilo = CDblDB(dr.Item("ComputedWeightKilo"))
                SubRUnitMainType = CInt(dr.Item("SubRUnitMainType"))
                Yield2MainType = CInt(dr.Item("Yield2MainType"))
            End If
            'MySQLConn.Close()
            cmd.Dispose()
            dr.Close()
            '
            '---------- Get Unit Factor -------------------
            dblUnitfactor = fctGetRecipeNutrientFactor(lngYieldUnit, dblSrQty, lngSrLevel, KiloCode, LiterCode, lngMainUnitCode, dblSubUnitfactor, SubRUnitMainType, Yield2MainType)
            If dblUnitfactor = -1 Then 'DLS
                If dblYieldQty > 0 Then
                    dblUnitfactor = dblComputedWeightKilo * 10 / dblYieldQty 'AGL 2014.01.08 
                Else
                    dblUnitfactor = dblComputedWeightKilo * 10
                End If

            End If
            'If dblUnitfactor = 0 Then dblUnitfactor = 1 'DLS

            Return dblUnitfactor
        Catch ex As Exception
            'Throw ex
        End Try
    End Function

    'Derived from same function in fmRecipe
    'I decided to have multiple non-nested ifs because it looks simpler
    'Also, Yield Unit can be different from the Sub Recipe Unit.
    'Priority is KG, regardless if it is Yield Unit or Subrecipe unit
    'DLS SubRecipeUnit should also consider
    Private Function fctGetRecipeNutrientFactor(ByVal lngYieldUnit As Long, _
        ByVal dblSrQty As Double, ByVal lngSrLevel As Long, ByVal KiloCode As Long, ByVal LiterCode As Long, _
        ByVal lngMainUnitCode As Long, ByVal dblSubUnitfactor As Double, ByVal SubRUnitMainType As Long, ByVal Yield2MainType As Long) As Double
        'DLS 12/6/2005
        Dim dblNutFactor As Double
        Dim blnFactorComputed As Boolean

        'Dim dblSubUnitfactor As Double         
        'Dim lngMainUnitCode As Double       

        blnFactorComputed = False
        dblNutFactor = -1


        ''3. Yield2 if it is in KG or LITER
        'If Yield2MainType = 100 And dblSrQty <> 0 Then
        '    dblNutFactor = dblSrQty * 10 * dblSubUnitfactor
        '    blnFactorComputed = True
        'End If

        'If Yield2MainType = 200 And dblSrQty <> 0 Then
        '    dblNutFactor = dblSrQty * 10 * dblSubUnitfactor
        '    blnFactorComputed = True
        'End If
        ''3. Yield2 if it is in KG or LITER


        '2. Sub recipe unit if it is in KG or LITER
        If SubRUnitMainType = 100 Or Yield2MainType = 100 And dblSrQty <> 0 Then
            dblNutFactor = dblSrQty * 10 * dblSubUnitfactor
            blnFactorComputed = True
        End If

        If SubRUnitMainType = 200 Or Yield2MainType = 200 And dblSrQty <> 0 Then
            dblNutFactor = dblSrQty * 10 * dblSubUnitfactor
            blnFactorComputed = True
        End If
        '2. Sub recipe unit if it is in KG or LITER

        '1.	Yield1 if it is in KG or LITER
        If lngYieldUnit = KiloCode Then
            'yield unit is Kilo
            dblNutFactor = 10
            blnFactorComputed = True
        End If

        If lngYieldUnit = LiterCode Then ''And Not blnFactorComputed Then
            'yield unit is Liter
            dblNutFactor = 10
            blnFactorComputed = True
        End If
        '1.	Yield1 if it is in KG or LITER

        '
        fctGetRecipeNutrientFactor = dblNutFactor
        '
    End Function

    Public Function fctGetNutrientsPerIngredient(ByVal intCodeTrans As Integer, ByVal intCodeListe As Integer, _
                                                ByVal mnuListeType As MenuType, ByVal intType As Integer) As DataSet ' VRP 06.05.2008

        Dim sqlCmd As SqlCommand = New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable
        Dim ds As New DataSet

        Try
            With sqlCmd
                .Connection = New SqlConnection(L_strCnn)
                .Connection.Open()
                .CommandType = CommandType.StoredProcedure
                .CommandText = "sp_EgswNutrientPerIngrGetList"
                .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@intType", SqlDbType.Int).Value = intType
                .Parameters.Add("@intListeType", SqlDbType.Int).Value = mnuListeType
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
                .ExecuteNonQuery()
                With da
                    .SelectCommand = sqlCmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
            End With
            ds.Tables.Add(dt)
            sqlCmd.Connection.Close()
            sqlCmd.Dispose()
            Return ds
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    'Public Function fctGetNutrientTrans(ByVal intCodeSite As Integer) As DataTable
    '    Dim cmd As New SqlCommand
    '    Dim cn As New SqlConnection(L_strCnn)
    '    Dim da As New SqlDataAdapter
    '    Dim dt As New DataTable

    '    Try
    '        With cmd
    '            .Connection = cn
    '            .CommandText = "SELECT * FROM EgswNutrientDefTrans WHERE CodeSite=@intCodeSite"
    '            .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
    '            .CommandType = CommandType.Text
    '            .Connection.Open()
    '            .ExecuteNonQuery()

    '            With da
    '                .SelectCommand = cmd
    '                dt.BeginLoadData()
    '                .Fill(dt)
    '                dt.EndLoadData()
    '            End With
    '            .Connection.Close()
    '            .Connection.Dispose()
    '        End With
    '        Return dt
    '    Catch ex As Exception
    '        Return Nothing
    '        cmd.Connection.Dispose()
    '    End Try
    'End Function

    'Public Function fctGetNutrientDEFTrans(ByVal intCodeTrans As Integer, ByVal intCodeSite As Integer) As DataTable
    '    Dim cn As New SqlConnection(L_strCnn)
    '    Dim cmd As New SqlCommand
    '    Dim da As New SqlDataAdapter
    '    Dim dt As New DataTable

    '    'Dim strText As String
    '    'If intCodeTrans = -1 Then
    '    '    strText = "SELECT DISTINCT n.Nutr_No, " & vbCrLf & _
    '    '             "nt.CodeTrans, " & vbCrLf & _
    '    '             "CASE WHEN nt.Name=NULL OR LEN(LTRIM(RTRIM(nt.Name)))=0 THEN n.Name ELSE ISNULL(nt.Name, n.Name) END Name, " & vbCrLf & _
    '    '             "n.Position " & vbCrLf & _
    '    '             "FROM egswNutrientDef n " & vbCrLf & _
    '    '             "LEFT OUTER JOIN EgswNutrientDefTrans nt ON n.Nutr_No=nt.CodeMain AND n.CodeSite = nt.CodeSite" & vbCrLf & _
    '    '             "WHERE nt.CodeSite=@intCodeSite " & vbCrLf & _
    '    '             "OR nt.CodeSite IS NULL " & vbCrLf & _
    '    '             "ORDER BY n.Position "
    '    'Else
    '    '    strText = "SELECT DISTINCT n.Nutr_No, " & vbCrLf & _
    '    '             "nt.CodeTrans, " & vbCrLf & _
    '    '             "CASE WHEN nt.Name=NULL OR LEN(LTRIM(RTRIM(nt.Name)))=0 THEN n.Name ELSE ISNULL(nt.Name, n.Name) END Name, " & vbCrLf & _
    '    '             "n.Position " & vbCrLf & _
    '    '             "FROM egswNutrientDef n " & vbCrLf & _
    '    '             "LEFT OUTER JOIN EgswNutrientDefTrans nt ON n.Nutr_No=nt.CodeMain " & vbCrLf & _
    '    '             "AND nt.CodeTrans=@intCodeTrans " & vbCrLf & _
    '    '             "AND nt.CodeSite=n.CodeSite " & vbCrLf & _
    '    '             "WHERE nt.CodeSite=@intCodeSite " & vbCrLf & _
    '    '             "OR nt.CodeSite IS NULL " & vbCrLf & _
    '    '             "ORDER BY n.Position "
    '    'End If


    '    Try
    '        With cmd
    '            .Connection = cn
    '            .CommandText = "sp_EgswNutrientDefTransGetList"
    '            .CommandType = CommandType.StoredProcedure
    '            .Parameters.Add("intCodeTrans", SqlDbType.Int).Value = intCodeTrans
    '            .Parameters.Add("intCodeSite", SqlDbType.Int).Value = intCodeSite
    '            .Connection.Open()
    '            .ExecuteNonQuery()

    '            With da
    '                .SelectCommand = cmd
    '                dt.BeginLoadData()
    '                .Fill(dt)
    '                dt.EndLoadData()
    '            End With
    '            .Connection.Close()
    '            .Connection.Dispose()
    '        End With
    '        Return dt
    '    Catch ex As Exception
    '        cmd.Connection.Dispose()
    '        Return Nothing
    '    End Try
    'End Function

    Public Function fctGetNutrientTrans(ByVal intCodeSite As Integer, ByVal intCodeTrans As Integer, Optional ByVal intCodeSiteTrans As Integer = 0, Optional ByVal intCodeSet As Integer = -1) As DataTable 'VRP 20.04.2009
        Dim cmd As New SqlCommand
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter

        Try
            With cmd
                .CommandType = CommandType.StoredProcedure
                .CommandText = "GET_NUTRIENTTRANSLIST"
                .Connection = New SqlConnection(L_strCnn)
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
                If intCodeSiteTrans <> 0 Then ' JBB 04.06.2011
                    .Parameters.Add("@CodeSiteTrans", SqlDbType.Int).Value = intCodeSiteTrans
                End If
                If intCodeSet <> -1 Then
                    .Parameters.Add("@CodeSet", SqlDbType.Int).Value = intCodeSet
                End If

                .Connection.Open()
                .ExecuteNonQuery()

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                .Connection.Dispose()
            End With
            Return dt
        Catch ex As Exception
            cmd.Connection.Close()
            Return Nothing
        End Try
    End Function
#End Region

#Region "Remove Methods"
    ''' <summary>
    ''' Remove Nutrient from the List
    ''' </summary>
    ''' <param name="intNutr_No"></param>
    ''' <param name="intCodeUser"></param>
    ''' <param name="intCodeSite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteFromList(ByVal intNutr_No As Integer, ByVal intCodeUser As Integer, ByVal intCodeSite As Integer, _
                                   ByVal blnCalcOnline As Boolean, intCodeSet As Integer) As enumEgswErrorCode
        Me.SaveNutrientToList(intNutr_No, "", "", intCodeUser, intCodeSite, enumEgswTransactionMode.Delete, -1, 0, blnCalcOnline, intCodeSet)
    End Function


    Public Function DeleteNutrientTrans(ByVal intNutr_No As Integer, ByVal intCodeSite As Integer, intCodeSet As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "DELETE FROM EgswNutrientDEFTrans WHERE CodeMain=" & intNutr_No & " AND CodeSite=" & intCodeSite & " AND CODESET= " & intCodeSet
                .CommandType = CommandType.Text
                .Connection.Open()
                .ExecuteNonQuery()
                cmd.Connection.Close()
                cmd.Dispose()
                L_ErrCode = enumEgswErrorCode.OK
            End With
        Catch ex As Exception
            cmd.Dispose()
            L_ErrCode = enumEgswErrorCode.GeneralError
        End Try
        Return L_ErrCode
    End Function
#End Region

#Region "Update Methods"
    ''' <summary>
    ''' Add Nutrient to  the List
    ''' </summary>
    ''' <param name="intNutr_No"></param>
    ''' <param name="intCodeUser"></param>
    ''' <param name="intCodeSite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateNutrient(ByVal intNutr_No As Integer, ByVal intCodeUser As Integer, ByVal intCodeSite As Integer, ByVal blnCalcOnline As Boolean, Optional ByVal intCodeSet As Integer = -1) As enumEgswErrorCode
        Return Me.SaveNutrientToList(intNutr_No, "", "", intCodeUser, intCodeSite, enumEgswTransactionMode.Add, -1, 0, blnCalcOnline, intCodeSet)
    End Function
    ''' <summary>
    ''' Set Nutrient Database
    ''' </summary>
    ''' <param name="intNutrientDBCode"></param>
    ''' <param name="intCodeUser"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateNutrient(ByVal intNutrientDBCode As Integer, ByVal intCodeUser As Integer, Optional intCodeSet As Integer = 0) As enumEgswErrorCode
        Return Me.SaveNutrientToList(-1, "", "", intCodeUser, -1, enumEgswTransactionMode.None, intNutrientDBCode, 0, , intCodeSet)
    End Function

    ''' <summary>
    ''' Update Nutrient's name and format
    ''' </summary>
    ''' <param name="intNutr_No"></param>
    ''' <param name="strName"></param>
    ''' <param name="strFormat"></param>
    ''' <param name="intCodeUser"></param>
    ''' <param name="intCodeSite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateNutrient(ByVal intNutr_No As Integer, ByVal strName As String, ByVal strFormat As String, _
                                    ByVal intCodeUser As Integer, ByVal intCodeSite As Integer, ByVal dblGDA As Double, _
                                    ByVal blnCalcOnline As Boolean, intCodeSet As Integer) As enumEgswErrorCode
        Return Me.SaveNutrientToList(intNutr_No, strName, strFormat, intCodeUser, intCodeSite, enumEgswTransactionMode.Edit, -1, dblGDA, blnCalcOnline, intCodeSet)
    End Function

    ''' <summary>
    ''' Move Position of the Nutrient Up
    ''' </summary>
    ''' <param name="intNutr_No"></param>
    ''' <param name="intCodeUser"></param>
    ''' <param name="intCodeSite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateNutrientMoveUp(ByVal intNutr_No As Integer, ByVal intCodeUser As Integer, ByVal intCodeSite As Integer, ByVal blnCalcOnline As Boolean, ByVal intcodeSet As Integer) As enumEgswErrorCode
        Return Me.SaveNutrientToList(intNutr_No, "", "", intCodeUser, intCodeSite, enumEgswTransactionMode.MovePositionUp, -1, 0, blnCalcOnline, intcodeSet)
    End Function

    ''' <summary>
    ''' Move Position of the Nutrient Down
    ''' </summary>
    ''' <param name="intNutr_No"></param>
    ''' <param name="intCodeUser"></param>
    ''' <param name="intCodeSite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateNutrientMoveDown(ByVal intNutr_No As Integer, ByVal intCodeUser As Integer, ByVal intCodeSite As Integer, ByVal blnCalcOnline As Boolean, ByVal intcodeSet As Integer) As enumEgswErrorCode
        Return Me.SaveNutrientToList(intNutr_No, "", "", intCodeUser, intCodeSite, enumEgswTransactionMode.MovePositionDown, -1, 0, blnCalcOnline, intcodeSet)
    End Function

    ''' <summary>
    ''' Recompute all recipes or menus
    ''' </summary>
    ''' <param name="intCodeUser"></param>
    ''' <param name="type"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateNutrientValRecomputeListe(ByVal intCodeUser As Integer, ByVal type As enumDataListType) As enumEgswErrorCode
        Try
            Dim arrParam(2) As SqlParameter
            arrParam(0) = New SqlParameter("@retVal", "")
            arrParam(0).Direction = ParameterDirection.ReturnValue
            arrParam(1) = New SqlParameter("@intCodeUser", intCodeUser)
            arrParam(2) = New SqlParameter("@intCodeListeType", type)
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswNutrientValUpdateRecomputeAll", arrParam)
            Return CType(arrParam(0).Value, enumEgswErrorCode)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function UpdateNutrientTranslation(ByVal intNutr_No As Integer, ByVal strName As String, _
                                              ByVal intCodeTrans As Integer, Optional ByVal intCodeSite As Integer = 0, Optional ByVal intCodeSet As Integer = 0) As enumEgswErrorCode 'VRP 07.01.2009
        Try
            Dim arrParam(4) As SqlParameter
            arrParam(0) = New SqlParameter("@intNutr_No", intNutr_No)
            arrParam(1) = New SqlParameter("@nvcName", strName)
            arrParam(2) = New SqlParameter("@intCodeTrans", intCodeTrans)
            arrParam(3) = New SqlParameter("@intCodeSite", intCodeSite)
            arrParam(4) = New SqlParameter("@intCodeSet", intCodeSet)
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "sp_EgswNutrientDefTransUpdate", arrParam)
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
            Throw ex
        End Try
    End Function
#End Region

End Class

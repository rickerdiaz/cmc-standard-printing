Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Text
Imports System.IO

Public Class clsMenuplan
    Inherits clsDBRoutine

    Private L_ErrCode As enumEgswErrorCode
    Private L_lngCodeUser As Int32 = -1
    Private L_lngCodeSite As Int32 = -1

    'Properties
    Private L_AppType As enumAppType
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType
    Private L_lngCode As Int32


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


#Region " UPDATE FUNCTION "
    Public Function subUpdatePlanLogo(ByVal strFileName As FileInfo, Optional ByVal intCode As Integer = 0) As enumEgswErrorCode 'VRP 17.07.2008
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswUpdateMenuplanLogo"
                .CommandType = CommandType.StoredProcedure

                Dim sFileName() As String = strFileName.Name.Split(CChar("."))
                .Parameters.Add("@nvcFileName", SqlDbType.NVarChar, 500).Value = sFileName(0)
                .Parameters.Add("@nvcExtension", SqlDbType.NVarChar, 5).Value = strFileName.Extension
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode

                .Connection.Open()
                .ExecuteNonQuery()
                cmd.Connection.Close()
                L_ErrCode = enumEgswErrorCode.OK
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

    Public Function UpdatePlanInfo(ByVal strSelectedWeek As String, ByVal intNo As Integer, ByVal strTitle As String, _
                                   ByVal strText As String, ByVal intCodeSite As Integer, ByVal intCodeUser As Integer) As enumEgswErrorCode

        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "PLAN_UPDATEINFOSV"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@SelectedWeek", SqlDbType.NVarChar).Value = strSelectedWeek
                .Parameters.Add("@No", SqlDbType.Int).Value = intNo
                .Parameters.Add("@Title", SqlDbType.NVarChar).Value = strTitle
                .Parameters.Add("@Text", SqlDbType.NVarChar).Value = strText

                .Connection.Open()
                .ExecuteNonQuery()
                cmd.Connection.Close()
                L_ErrCode = enumEgswErrorCode.OK
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

    Public Function UpdatePlanConfig(ByVal tPlan As structMenuplan, ByVal intCodeSite As Integer, _
                                     ByVal intCodeUser As Integer) As Integer

        Dim cmd As New SqlCommand
        Dim intCode As Integer = 0

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "PLAN_UPDATECONFIGSV"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@SelectedWeek", SqlDbType.NVarChar).Value = tPlan.WeekDate
                .Parameters.Add("@BusinessNumber", SqlDbType.NVarChar, 200).Value = tPlan.BusinessNumber
                .Parameters.Add("@Street", SqlDbType.NVarChar, 250).Value = tPlan.Street
                .Parameters.Add("@Zip", SqlDbType.NVarChar, 250).Value = tPlan.Zip
                .Parameters.Add("@City", SqlDbType.NVarChar, 250).Value = tPlan.City
                .Parameters.Add("@Phone", SqlDbType.NVarChar, 100).Value = tPlan.Phone
                .Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = tPlan.Email
                .Parameters.Add("@Days", SqlDbType.NVarChar, 100).Value = tPlan.Days
                .Parameters.Add("@Time", SqlDbType.NVarChar, 100).Value = tPlan.Time
                .Parameters.Add("@NoOfProposal", SqlDbType.Int).Value = tPlan.NoofProposal
                .Parameters.Add("@Price", SqlDbType.NVarChar, 500).Value = tPlan.Price
                .Parameters.Add("@CodeLogo", SqlDbType.Int).Value = tPlan.CodeLogo

                .Parameters.Add("@Code", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@DateStart", SqlDbType.DateTime).Value = tPlan.DateStart
                .Parameters.Add("@IsPrintNut", SqlDbType.Bit).Value = tPlan.PrintNut
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser 'VRP 29.04.2009

                .Connection.Open()
                .ExecuteNonQuery()

                intCode = CInt(.Parameters("@Code").Value)

                cmd.Connection.Close()
                Return intCode
            End With
        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
        cmd.Dispose()
    End Function

    Public Function fctUpdatePlanConfigTrans(ByVal intCode As Integer, ByVal tMenuPlan As structMenuplan) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswUpdateMenuplanConfigTrans"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeMain", SqlDbType.Int).Value = intCode
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = tMenuPlan.CodeTrans
                .Parameters.Add("@nvcBusinessName", SqlDbType.NVarChar).Value = tMenuPlan.BusinessName
                .Parameters.Add("@nvcProposalName", SqlDbType.NVarChar).Value = tMenuPlan.ProposalName
                .Parameters.Add("@nvcOpenDays", SqlDbType.NVarChar).Value = tMenuPlan.OpenDays
                .Parameters.Add("@nvcOpenTime", SqlDbType.NVarChar).Value = tMenuPlan.OpenTime
                .Parameters.Add("@nvcCloseDays", SqlDbType.NVarChar).Value = tMenuPlan.CloseDays

                .Connection.Open()
                .ExecuteNonQuery()
                cmd.Connection.Close()
            End With

            L_ErrCode = enumEgswErrorCode.OK
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdatePlan(ByVal intCodeSite As Integer, ByVal intCodeUserOwner As Integer, ByVal intCodeUserModify As Integer, _
                               ByVal strSelectedWeek As String, ByVal intCodeDay As Integer, ByVal intProposalNo As Integer, _
                               ByVal blnActionmenu As Boolean, ByVal strPrice1 As String, ByVal strPrice2 As String, _
                               ByVal strPrice3 As String, ByVal dblN1 As Double, ByVal dblN2 As Double, _
                               ByVal dblN3 As Double, ByVal dblN4 As Double, ByVal dblN5 As Double, _
                               ByVal dblN6 As Double, ByVal dblN7 As Double, ByVal dblN8 As Double, _
                               ByVal dblN9 As Double, ByVal dblN10 As Double, ByVal dblN11 As Double, _
                               ByVal dblN12 As Double, ByVal dblN13 As Double, ByVal dblN14 As Double, _
                               ByVal dblN15 As Double, ByVal blnCo As Boolean, ByVal strCoPrice1 As String, _
                               ByVal strCoPrice2 As String, ByVal strCoPrice3 As String, ByVal dblFactor1 As Double, _
                               ByVal dblFactor2 As Double, ByVal dblFactor3 As Double, Optional ByVal dblCalcPrice1 As Double = 0, _
                               Optional ByVal dblCalcPrice2 As Double = 0, Optional ByVal dblCalcPrice3 As Double = 0, _
                               Optional ByVal dblCOGAvg As Double = 0, Optional ByVal dblYield As Double = 0, _
                               Optional ByVal intYieldUnit As Integer = 0) As Integer

        Dim intCode As Integer = -1

        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "PLAN_UPDATESV"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@CodeUserOwner", SqlDbType.Int).Value = intCodeUserOwner
                .Parameters.Add("@CodeUserModify", SqlDbType.Int).Value = intCodeUserModify
                .Parameters.Add("@SelectedWeek", SqlDbType.NVarChar).Value = strSelectedWeek
                .Parameters.Add("@CodeDay", SqlDbType.Int).Value = intCodeDay
                .Parameters.Add("@ProposalNo", SqlDbType.Int).Value = intProposalNo
                .Parameters.Add("@IsActionMenu", SqlDbType.Bit).Value = blnActionmenu
                .Parameters.Add("@Price1", SqlDbType.NVarChar, 200).Value = strPrice1
                .Parameters.Add("@Price2", SqlDbType.NVarChar, 200).Value = strPrice2
                .Parameters.Add("@Price3", SqlDbType.NVarChar, 200).Value = strPrice3
                .Parameters.Add("@N1", SqlDbType.Float).Value = dblN1
                .Parameters.Add("@N2", SqlDbType.Float).Value = dblN2
                .Parameters.Add("@N3", SqlDbType.Float).Value = dblN3
                .Parameters.Add("@N4", SqlDbType.Float).Value = dblN4
                .Parameters.Add("@N5", SqlDbType.Float).Value = dblN5
                .Parameters.Add("@N6", SqlDbType.Float).Value = dblN6
                .Parameters.Add("@N7", SqlDbType.Float).Value = dblN7
                .Parameters.Add("@N8", SqlDbType.Float).Value = dblN8
                .Parameters.Add("@N9", SqlDbType.Float).Value = dblN9
                .Parameters.Add("@N10", SqlDbType.Float).Value = dblN10
                .Parameters.Add("@N11", SqlDbType.Float).Value = dblN11
                .Parameters.Add("@N12", SqlDbType.Float).Value = dblN12
                .Parameters.Add("@N13", SqlDbType.Float).Value = dblN13
                .Parameters.Add("@N14", SqlDbType.Float).Value = dblN14
                .Parameters.Add("@N15", SqlDbType.Float).Value = dblN15
                .Parameters.Add("@IsCO", SqlDbType.Bit).Value = blnCo
                .Parameters.Add("@CoPrice1", SqlDbType.NVarChar, 200).Value = strCoPrice1
                .Parameters.Add("@CoPrice2", SqlDbType.NVarChar, 200).Value = strCoPrice2
                .Parameters.Add("@CoPrice3", SqlDbType.NVarChar, 200).Value = strCoPrice3
                .Parameters.Add("@Factor1", SqlDbType.Float).Value = dblFactor1
                .Parameters.Add("@Factor2", SqlDbType.Float).Value = dblFactor2
                .Parameters.Add("@Factor3", SqlDbType.Float).Value = dblFactor3
                .Parameters.Add("@CalcPrice1", SqlDbType.Float).Value = dblCalcPrice1
                .Parameters.Add("@CalcPrice2", SqlDbType.Float).Value = dblCalcPrice2
                .Parameters.Add("@CalcPrice3", SqlDbType.Float).Value = dblCalcPrice3
                .Parameters.Add("@COGAvg", SqlDbType.Float).Value = dblCOGAvg
                .Parameters.Add("@Yield", SqlDbType.Float).Value = dblYield
                .Parameters.Add("@YieldUnit", SqlDbType.Int).Value = intYieldUnit

                .Parameters.Add("@Code", SqlDbType.Int).Direction = ParameterDirection.Output

                .Connection.Open()
                .ExecuteNonQuery()

                intCode = CInt(.Parameters("@Code").Value)

                cmd.Connection.Close()
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
        cmd.Dispose()
        Return intCode
    End Function

    Public Function fctUpdatePlanTrans(ByVal intCode As Integer, ByVal intCodeTrans As Integer, _
                                       ByVal strCoName As String, ByVal strItemName As String, _
                                       ByVal strItemName2 As String) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswUpdateMenuplanTrans"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeMain", SqlDbType.Int).Value = intCode
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@nvcCoName", SqlDbType.NVarChar).Value = strCoName
                .Parameters.Add("@nvcItemName", SqlDbType.NVarChar).Value = strItemName
                .Parameters.Add("@nvcItemName2", SqlDbType.NVarChar).Value = strItemName2

                .Connection.Open()
                .ExecuteNonQuery()
                cmd.Connection.Close()
            End With

            L_ErrCode = enumEgswErrorCode.OK
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Throw New Exception(ex.Message, ex)
        End Try
        cmd.Dispose()
        Return L_ErrCode
    End Function

    Public Function UpdatePlanDetails(ByVal intCodeSite As Integer, ByVal intCodeUser As Integer, _
                                      ByVal strSelectedWeek As String, ByVal intCodeDay As Integer, _
                                      ByVal intProposalNo As Integer, ByVal intCodeListe As Integer, _
                                      ByVal intListeType As Integer, ByVal strName As String, _
                                      ByVal intPosition As Integer, ByVal dblQty As Double, _
                                      ByVal intCodeUnit As Integer, ByVal intUnitFactor As Integer, _
                                      ByVal blnYieldUnit As Boolean, ByVal dblWastage1 As Double, _
                                      ByVal dblWastage2 As Double, ByVal dblWastage3 As Double, _
                                      ByVal dblWastage4 As Double, ByVal dblWastage5 As Double, _
                                      ByVal intCode As Integer) As Integer

        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "PLAN_UPDATEDETAILSSV"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@SelectedWeek", SqlDbType.NVarChar).Value = strSelectedWeek
                .Parameters.Add("@CodeDay", SqlDbType.Int).Value = intCodeDay
                .Parameters.Add("@ProposalNo", SqlDbType.Int).Value = intProposalNo
                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = intCodeListe
                .Parameters.Add("@ListeType", SqlDbType.Int).Value = intListeType
                .Parameters.Add("@Name", SqlDbType.NVarChar).Value = strName
                .Parameters.Add("@Position", SqlDbType.Int).Value = intPosition
                .Parameters.Add("@Qty", SqlDbType.Float).Value = dblQty
                .Parameters.Add("@CodeUnit", SqlDbType.Int).Value = intCodeUnit
                .Parameters.Add("@UnitFactor", SqlDbType.Int).Value = intUnitFactor
                .Parameters.Add("@IsYieldUnit", SqlDbType.Bit).Value = blnYieldUnit
                .Parameters.Add("@Wastage1", SqlDbType.Float).Value = dblWastage1
                .Parameters.Add("@Wastage2", SqlDbType.Float).Value = dblWastage2
                .Parameters.Add("@Wastage3", SqlDbType.Float).Value = dblWastage3
                .Parameters.Add("@Wastage4", SqlDbType.Float).Value = dblWastage4
                .Parameters.Add("@Wastage5", SqlDbType.Float).Value = dblWastage5
                .Parameters.Add("@Code", SqlDbType.Int).Direction = ParameterDirection.InputOutput
                .Parameters("@Code").Value = intCode

                .Connection.Open()
                .ExecuteNonQuery()

                intCode = CInt(.Parameters("@intCode").Value)

                cmd.Connection.Close()
            End With
        Catch ex As Exception
            cmd.Connection.Dispose()
        End Try
        Return intCode
    End Function

    Public Function fctUpdateTempMain(ByVal intCodeUser As Integer) As Integer
        Dim cmd As New SqlCommand
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter
        Dim sbSQL As New StringBuilder

        sbSQL.Append("INSERT INTO EgsW_TempMarkMain (Dates, CodeUser) ")
        sbSQL.Append("VALUES (GETDATE()," & intCodeUser & ") ")
        sbSQL.Append("SELECT TOP 1 ID FROM EgsW_TempMarkMain ORDER BY ID DESC ")

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandType = CommandType.Text
                .CommandText = sbSQL.ToString
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
            End With

            With da
                .SelectCommand = cmd
                dt.BeginLoadData()
                .Fill(dt)
                dt.EndLoadData()
            End With

            If Not dt.Rows.Count = 0 Then
                Return CIntDB(dt.Rows(0).Item("ID"))
            Else
                Return -1
            End If

        Catch ex As Exception
            cmd.Dispose()
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function fctUpdateTempDetails(ByVal intCode As Integer, ByVal intCodeListe As Integer, ByVal dblQty As Double, ByVal intCodeUnit As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Dim sbSQL As New StringBuilder

        sbSQL.Append("INSERT INTO EgsW_TempMarkDetails (IDMain, CodeListe, Qty, CodeUnit) ")
        sbSQL.Append("VALUES(" & intCode & "," & intCodeListe & "," & dblQty & "," & intCodeUnit & ") ")

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandType = CommandType.Text
                .CommandText = sbSQL.ToString
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
            End With
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            cmd.Dispose()
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function UpdatePrices(ByVal strSelectedWeek As String, ByVal intCodeSite As Integer, _
                                 Optional ByVal intCodeUser As Integer = 0) As enumEgswErrorCode

        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "PLAN_REFRESHPRICES"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@SelectedWeek", SqlDbType.NVarChar).Value = strSelectedWeek
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser
                .Connection.Open()
                .ExecuteNonQuery()
                cmd.Connection.Close()
                L_ErrCode = enumEgswErrorCode.OK
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
#End Region

#Region " GET FUNCTION "
    Public Function fctGetPlanLogoList(ByVal intCode As Integer) As DataTable 'VRP 17.07.2008
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "sp_EgswGetMenuplanLogoList"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode

                .Connection.Open()
                .ExecuteNonQuery()
                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                cmd.Dispose()
                Return dt
            End With
        Catch ex As Exception
            cmd.Dispose()
            fctGetPlanLogoList = Nothing
        End Try
    End Function

    '======== NEW
    Public Function GetPlanConfig(ByVal strSelectedWeek As String, Optional ByVal intCodeSite As Integer = -1, _
                                  Optional ByVal intCodeUser As Integer = 0, _
                                  Optional ByVal intOption As Integer = 0) As DataTable

        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "PLAN_GETCONFIGSV"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@SelectedWeek", SqlDbType.NVarChar).Value = strSelectedWeek
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@Option", SqlDbType.Int).Value = intOption

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
            cmd.Dispose()
            GetPlanConfig = Nothing
        End Try

    End Function

    Public Function fctGetPlanConfigTrans(ByVal intCode As Integer) As Object
        Try
            Dim sbSQL As New StringBuilder
            sbSQL.Append("SELECT * FROM EgswMenuPlanConfigTrans ")
            sbSQL.Append("WHERE CodeMain=" & intCode)

            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.Text, sbSQL.ToString)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function fctGetPlanList(ByVal strSelectedWeek As String, ByVal intCodeSite As Integer, _
                                   Optional ByVal intCodeUser As Integer = 0) As DataTable

        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "PLAN_GETLISTSV"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@SelectedWeek", SqlDbType.NVarChar, 200).Value = strSelectedWeek
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser 'VRP 29.04.2009

                .Connection.Open()
                .ExecuteNonQuery()
                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                cmd.Dispose()
                Return dt
            End With
        Catch ex As Exception
            cmd.Dispose()
            Return Nothing
        End Try

        'Dim cmd As New SqlCommand
        'Dim da As New SqlDataAdapter
        'Dim dt As New DataTable

        'Dim strSQL As String = ""
        'strSQL += "SELECT * FROM EgswMenuplan  "
        'strSQL += "WHERE Date=@Date AND CodeSite=@CodeSite AND CodeUserOwner=@CodeUser"

        'Try
        '    With cmd
        '        .Connection = New SqlConnection(L_strCnn)
        '        .CommandText = strSQL
        '        .CommandType = CommandType.Text
        '        .Parameters.Add("@Date", SqlDbType.NVarChar, 200).Value = strDate
        '        .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
        '        .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser 'VRP 29.04.2009

        '        .Connection.Open()
        '        .ExecuteNonQuery()
        '        With da
        '            .SelectCommand = cmd
        '            dt.BeginLoadData()
        '            .Fill(dt)
        '            dt.EndLoadData()
        '        End With
        '        .Connection.Close()
        '        cmd.Dispose()
        '        Return dt
        '    End With
        'Catch ex As Exception
        '    cmd.Dispose()
        '    Return Nothing
        'End Try
    End Function

    Public Function fctGetPlanInfo(ByVal strSelectedWeek As String, ByVal intCodeSite As Integer, _
                                   Optional ByVal intCodeUser As Integer = 0) As DataTable 'VRP 06.08.2008

        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "PLAN_GETINFOSV"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@SelectedWeek", SqlDbType.NVarChar, 200).Value = strSelectedWeek
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser 'VRP 29.04.2009

                .Connection.Open()
                .ExecuteNonQuery()
                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                cmd.Dispose()
                Return dt
            End With
        Catch ex As Exception
            cmd.Dispose()
            Return Nothing
        End Try

        'Dim cmd As New SqlCommand
        'Dim da As New SqlDataAdapter
        'Dim dt As New DataTable

        'Dim strSQL As String = ""
        'strSQL += "SELECT * FROM EgswMenuplan2 "
        'strSQL += "WHERE Date=@Date AND CodeSite=@CodeSite AND CodeUser=@CodeUser "
        'strSQL += "ORDER BY No "

        'Try
        '    With cmd
        '        .Connection = New SqlConnection(L_strCnn)
        '        '.CommandText = "SELECT * FROM EgswMenuplan2 WHERE Date='" & strDate & "' AND CodeSite=" & intCodeSite & " ORDER BY No"
        '        .CommandText = strSQL
        '        .CommandType = CommandType.Text
        '        .Parameters.Add("@Date", SqlDbType.NVarChar, 200).Value = strDate
        '        .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
        '        .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser 'VRP 29.04.2009

        '        .Connection.Open()
        '        .ExecuteNonQuery()
        '        With da
        '            .SelectCommand = cmd
        '            dt.BeginLoadData()
        '            .Fill(dt)
        '            dt.EndLoadData()
        '        End With
        '        .Connection.Close()
        '        cmd.Dispose()
        '        Return dt
        '    End With
        'Catch ex As Exception
        '    cmd.Dispose()
        '    Return Nothing
        'End Try
    End Function

    Public Function fctGetPlanTrans(ByVal strSelectedWeek As String, ByVal intCodeSite As Integer, _
                                    Optional ByVal intCodeUser As Integer = 0) As DataTable

        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "PLAN_GETLISTTRANSSV"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@SelectedWeek", SqlDbType.NVarChar, 200).Value = strSelectedWeek
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser 'VRP 29.04.2009

                .Connection.Open()
                .ExecuteNonQuery()
                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With
                .Connection.Close()
                cmd.Dispose()
                Return dt
            End With
        Catch ex As Exception
            cmd.Dispose()
            fctGetPlanTrans = Nothing
        End Try

        'Dim cmd As New SqlCommand
        'Dim da As New SqlDataAdapter
        'Dim dt As New DataTable

        'Dim strSQL As String = ""
        'strSQL += "SELECT mpt.CodeMain, mp.Date, mp.CodeDay, mp.ProposalNo, mpt.CodeTrans, mpt.CoName, mpt.ItemName, mpt.ItemName2 "
        'strSQL += "FROM EgswMenuplan mp "
        'strSQL += "INNER JOIN EgswMenuPlanTrans mpt ON mp.Code=mpt.CodeMain "
        'strSQL += "WHERE mp.Date=@Date AND mp.CodeSite=@CodeSite AND mp.CodeUserOwner=@CodeUser "
        'strSQL += "ORDER BY mpt.CodeMain, mp.CodeDay, mp.ProposalNo "

        ''Dim sb As New StringBuilder
        ''sb.Append("SELECT mpt.CodeMain, mp.Date, mp.CodeDay, mp.ProposalNo, mpt.CodeTrans, mpt.CoName, mpt.ItemName, mpt.ItemName2 ")
        ''sb.Append("FROM EgswMenuplan mp ")
        ''sb.Append("INNER JOIN EgswMenuPlanTrans mpt ON mp.Code=mpt.CodeMain ")
        ''sb.Append("WHERE mp.Date='" & strDate & "' AND mp.CodeSite=" & intCodeSite)
        ''sb.Append("ORDER BY mpt.CodeMain, mp.CodeDay, mp.ProposalNo ")

        'Try
        '    With cmd
        '        .Connection = New SqlConnection(L_strCnn)
        '        .CommandText = strSQL
        '        .CommandType = CommandType.Text
        '        .Parameters.Add("@Date", SqlDbType.NVarChar, 200).Value = strDate
        '        .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
        '        .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser 'VRP 29.04.2009

        '        .Connection.Open()
        '        .ExecuteNonQuery()
        '        With da
        '            .SelectCommand = cmd
        '            dt.BeginLoadData()
        '            .Fill(dt)
        '            dt.EndLoadData()
        '        End With
        '        .Connection.Close()
        '        cmd.Dispose()
        '        Return dt
        '    End With
        'Catch ex As Exception
        '    cmd.Dispose()
        '    fctGetPlanTrans = Nothing
        'End Try
    End Function

    Public Function fctGetPlanDetails(ByVal strSelectedWeek As String, ByVal intCodeSite As Integer, _
                                      ByVal intCodeUserSharedTo As Integer, ByVal intCodeTrans As Integer, _
                                      Optional ByVal intCodeUser As Integer = 0) As DataTable

        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "PLAN_GETDETAILSSV"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@SelectedWeek", SqlDbType.NVarChar).Value = strSelectedWeek
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@CodeUserSharedTo", SqlDbType.Int).Value = intCodeUserSharedTo
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser

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
            cmd.Dispose()
            fctGetPlanDetails = Nothing
        End Try


        'Dim cn As New SqlConnection(L_strCnn)
        'Dim cmd As New SqlCommand
        'Dim da As New SqlDataAdapter
        'Dim dt As New DataTable

        'Try
        '    With cmd
        '        .Connection = cn
        '        .CommandText = "sp_EgswGetMenuplanDetails"
        '        .CommandType = CommandType.StoredProcedure
        '        .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
        '        .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
        '        .Parameters.Add("@nvcSelectedWeek", SqlDbType.NVarChar).Value = strSelectedWeek
        '        .Parameters.Add("@intCodeUserSharedTo", SqlDbType.Int).Value = intCodeUserSharedTo

        '        .Connection.Open()
        '        .ExecuteNonQuery()

        '        With da
        '            .SelectCommand = cmd
        '            dt.BeginLoadData()
        '            .Fill(dt)
        '            dt.EndLoadData()
        '        End With
        '        Return dt
        '    End With
        '    cmd.Connection.Close()
        '    cmd.Dispose()
        'Catch ex As Exception
        '    cmd.Dispose()
        '    fctGetPlanDetails = Nothing
        'End Try
    End Function

    Public Function GetPlanDetailTrans(ByVal strSelectedWeek As String, ByVal intCodeSite As Integer, _
                                       ByVal intCodeUser As Integer) As DataTable

        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "PLAN_GETDETAILTRANSSV"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@SelectedWeek", SqlDbType.NVarChar).Value = strSelectedWeek
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = intCodeUser
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

    Public Function fctGetListeTrans(ByVal intCodeListe As Integer, ByVal intCodeTrans As Integer) As String
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim strListeName As String = ""
        Dim dr As SqlDataReader

        Dim sb As New StringBuilder
        sb.Append("SELECT CASE WHEN lt.Name IS NULL OR LEN(RTRIM(LTRIM(lt.Name)))=0 THEN l.Name ELSE lt.Name END Name ")
        sb.Append("FROM EgswListe l ")
        sb.Append("LEFT OUTER JOIN EgswListeTranslation lt ON l.Code=lt.CodeListe AND lt.CodeTrans IN (" & intCodeTrans & ", NULL) ")
        sb.Append("WHERE l.Code=" & intCodeListe)

        Try
            With cmd
                .Connection = cn
                .CommandType = CommandType.Text
                .CommandText = sb.ToString '"SELECT Name FROM EgswListeTranslation WHERE CodeListe=" & intCodeListe & " AND CodeTrans=" & intCodeTrans

                .Connection.Open()
                dr = .ExecuteReader()
                While dr.Read
                    strListeName = CStr(dr.Item("Name"))
                End While
                dr.Close()
                .Connection.Close()
                .Connection.Dispose()
            End With
            Return strListeName
        Catch ex As Exception
            Return strListeName
            cmd.Connection.Dispose()
        End Try
    End Function

    Public Function fctGetPrintDetailMenuPlan(ByVal strSelectedWeek As String, ByVal intCodeSite As Integer, _
                                              Optional ByVal intCodeUser As Integer = 0) As Object
        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@SelectedWeek", strSelectedWeek)
        arrParam(1) = New SqlParameter("@CodeSite", intCodeSite)
        arrParam(2) = New SqlParameter("@CodeUser", intCodeUser)

        Return Me.ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "PLAN_GETPRINTDETAILS", arrParam)
    End Function

    Public Function fctGetPlanNutVal(ByVal intIDMain As Integer, Optional ByVal intCodeSetPrice As Integer = -1) As Object
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@intIDMain", intIDMain)
        arrParam(1) = New SqlParameter("@CodeSetPrice", intCodeSetPrice)

        Try
            Return ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "sp_EgswMenuPlanNutrientValGet", arrParam)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function fctIsLogoUsed(ByVal intCodeLogo As Integer) As Boolean
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader
        Dim intCount As Integer

        Dim sb As New StringBuilder
        sb.Append("SELECT Count(*) FROM EgswMEnuPlanConfig ")
        sb.Append("WHERE CODELOGO= " & intCodeLogo)

        With cmd
            .Connection = cn
            .CommandType = CommandType.Text
            .CommandText = sb.ToString

            .Connection.Open()
            dr = .ExecuteReader()
            While dr.Read
                intCount = CIntDB((dr.Item(0)))
            End While
            dr.Close()
            .Connection.Close()
            .Connection.Dispose()

            If intCount > 0 Then
                Return True
            Else
                Return False
            End If
        End With
    End Function

#End Region

#Region " DELETE FUNCTION "
    Public Function subRemovePlanLogoList(ByVal intCode As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "DELETE FROM EgswMenuplanLogo WHERE Code=" & intCode
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

    Public Function fctRemovePlanConfig(ByVal intCode As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "DELETE FROM EgswMenuPlanConfig WHERE Code=" & intCode
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

    Public Function fctRemovePlanConfigTrans(ByVal intCode As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "DELETE FROM EgswMenuplanConfigTrans WHERE CodeMain=" & intCode
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

    Public Function fctRemovePlanTrans(ByVal intCode As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "DELETE FROM EgswMenuPlanTrans WHERE CodeMain=" & intCode
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

    Public Function fctRemovePlanDetails(ByVal strSelectedWeek As String, ByVal intCodeSite As Integer, _
                                         ByVal intCodeUser As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Dim sbSQL As New StringBuilder
        sbSQL.Append("DELETE FROM EgswMenuPlanDetails WHERE CodeSite=" & intCodeSite & " AND Date='" & strSelectedWeek & "' ")
        sbSQL.Append("AND CodeUser = " & intCodeUser)

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandType = CommandType.Text
                .CommandText = sbSQL.ToString
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                .Dispose()
            End With
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function fctRemovePlanInfo(ByVal strDate As String, ByVal intCodeSite As Integer, ByVal intCodeUser As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Dim sbSQL As New StringBuilder
        sbSQL.Append("DELETE FROM EgswMenuPlan2 WHERE Date='" & strDate & "' AND CodeSite=" & intCodeSite & " AND CodeUser = " & intCodeUser)

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandType = CommandType.Text
                .CommandText = sbSQL.ToString
                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()
                .Dispose()

                Return enumEgswErrorCode.OK
            End With
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function
#End Region




    '''============================================================================================================    
    ''' <summary>
    ''' MIGROS MASTER PLAN
    ''' </summary>
    ''' <param name="tntDisplayMode"></param>    
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetRestaurant(ByVal intCodeSite As Integer, Optional ByVal intRestaurantID As Integer = -1, Optional ByVal strName As String = "") As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_GetRestaurant"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@intRestaurantID", SqlDbType.Int).Value = intRestaurantID
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 200).Value = strName
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
            cmd.Dispose()
            GetRestaurant = Nothing
        End Try

    End Function

    Public Function GetMasterPlan(Optional ByVal intRestaurantID As Integer = -1) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_GetMasterPlan"
                .CommandType = CommandType.StoredProcedure
                If intRestaurantID > 0 Then
                    .Parameters.Add("@intRestaurantID", SqlDbType.Int).Value = intRestaurantID
                End If
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
            cmd.Dispose()
            GetMasterPlan = Nothing
        End Try

    End Function

    Public Function GetLanguage(Optional ByVal intRestaurantID As Integer = -1) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_GetLanguage"
                .CommandType = CommandType.StoredProcedure
                If intRestaurantID > 0 Then
                    .Parameters.Add("@intRestaurantID", SqlDbType.Int).Value = intRestaurantID
                End If
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
            cmd.Dispose()
            GetLanguage = Nothing
        End Try

    End Function


    Public Function UpdateRestauranMasterPlan(ByVal intRestaurantID As Integer, Optional ByVal intMasterPlanID As Integer = -1) As Integer

        Dim cmd As New SqlCommand
        Dim intCode As Integer = 0

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_UPDATERestaurantMasterPlan]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intRestaurantID", SqlDbType.Int).Value = intRestaurantID
                .Parameters.Add("@intMasterPlanID", SqlDbType.Int).Value = intMasterPlanID

                .Connection.Open()
                .ExecuteNonQuery()

                cmd.Connection.Close()
                Return intCode
            End With
        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Return -1

        Finally
            cmd.Dispose()

        End Try

    End Function


    Public Function UpdateRestaurant(ByRef intRestaurantID As Integer, ByVal strName As String, ByVal strLanguage As String) As Integer

        Dim cmd As New SqlCommand
        Dim intCode As Integer = -1

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_UPDATERestaurant]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intRestaurantID", SqlDbType.Int).Value = intRestaurantID
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 200).Value = strName
                .Parameters.Add("@nvcLanguage", SqlDbType.NVarChar, 200).Value = strLanguage
                .Parameters.Add("@intReturnID", SqlDbType.Int).Direction = ParameterDirection.Output

                .Connection.Open()
                .ExecuteNonQuery()

                intCode = CIntDB(.Parameters("@intReturnID").Value)

                cmd.Connection.Close()
                Return intCode
            End With
        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Return -1

        Finally
            cmd.Dispose()

        End Try

    End Function

    Public Function UpdateMasterPlan(ByRef intMasterPlanID As Integer, Optional ByVal strName As String = "") As Integer

        Dim cmd As New SqlCommand
        Dim intCode As Integer = -1

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_UPDATEMasterPlan]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intMasterPlanID", SqlDbType.Int).Value = intMasterPlanID
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 200).Value = strName

                .Parameters.Add("@intReturnID", SqlDbType.Int).Direction = ParameterDirection.Output

                .Connection.Open()
                .ExecuteNonQuery()

                intCode = CIntDB(.Parameters("@intReturnID").Value)

                cmd.Connection.Close()
                Return intCode
            End With
        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Return -1

        Finally
            cmd.Dispose()

        End Try

    End Function

    Public Overloads Function RemoveRestaurant(Optional ByVal intRestaurantID As Integer = -1, Optional ByVal strRestaurantIDList As String = "") As enumEgswErrorCode

        Dim cmd As New SqlCommand
        Dim intCode As Integer = -1

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_DELETERestaurant]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intRestaurantID", SqlDbType.Int).Value = intRestaurantID
                .Parameters.Add("@nvcRestaurantIDList", SqlDbType.NVarChar, 2000).Value = strRestaurantIDList
                .Parameters.Add("@intRetval", SqlDbType.Int).Direction = ParameterDirection.Output

                .Connection.Open()
                .ExecuteNonQuery()

                intCode = CIntDB(.Parameters("@intRetval").Value)

                cmd.Connection.Close()
                Return enumEgswErrorCode.OK
            End With
        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Return enumEgswErrorCode.GeneralError
        Finally
            cmd.Dispose()
        End Try

    End Function

    Public Function UpdateExportCostMargin(ByVal CodeUser As Integer, ByVal XMLData As String, ByVal Day As Integer) As Boolean

        Dim cmd As New SqlCommand
        Dim result As Boolean

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_UPDATEExportCostMargin]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = GetInt(CodeUser)
                .Parameters.Add("@XMLData", SqlDbType.Xml).Value = GetStr(XMLData)
                .Parameters.Add("@Day", SqlDbType.Int).Value = GetInt(Day)
                .Parameters.Add("@Result", SqlDbType.Int).Direction = ParameterDirection.Output

                .Connection.Open()
                .ExecuteNonQuery()

                result = CBoolDB(.Parameters("@Result").Value)

                cmd.Connection.Close()
                Return result
            End With
        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Return False
        Finally
            cmd.Dispose()
        End Try

    End Function
End Class

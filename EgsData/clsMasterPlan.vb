Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data
Imports System.Text
Imports System.IO
Imports EgsData
Imports System.Net

Public Class clsMasterPlan
    Inherits clsDBRoutine

    Private L_ErrCode As enumEgswErrorCode
    Private L_lngCodeUser As Int32 = -1
    Private L_lngCodeSite As Int32 = -1

    'Properties
    Private L_AppType As enumAppType
    Private L_strCnn As String
    Private L_bytFetchType As enumEgswFetchType
    Private L_lngCode As Int32
    Private L_udtUser As structUser

    Public Sub New(ByVal udtUser As structUser, ByVal eAppType As enumAppType, ByVal strCnn As String, _
      Optional ByVal bytFetchType As enumEgswFetchType = enumEgswFetchType.DataReader)

        Try
            L_udtUser = udtUser
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


    Public Function UpdateDetailsByHQ(ByVal intIDMain As Integer, ByVal intDayPlan As Integer, ByVal intCodeMasterPlan As Integer, _
                                  ByVal intCodeRestaurant As Integer, ByVal dblPrice As Double, _
                                  ByVal intCodeTax As Integer) As enumEgswErrorCode

        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_UPDATEDetailByHQ"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intIDMain", SqlDbType.Int).Value = intIDMain
                .Parameters.Add("@intDayPlan", SqlDbType.Int).Value = intDayPlan
                .Parameters.Add("@intCodeMasterPlan", SqlDbType.Int).Value = intCodeMasterPlan
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@fltPrice", SqlDbType.Float).Value = dblPrice
                .Parameters.Add("@intCodeTax", SqlDbType.Float).Value = intCodeTax
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

    Public Function UpdateDetails(ByVal intID As Integer, _
                                  ByVal intIDMain As Integer, _
                                  ByVal intDayPlan As Integer, _
                                  ByVal intCodeMasterPlan As Integer, _
                                  ByVal intCodeRestaurant As Integer, _
                                  ByVal blnIsLock As Boolean, _
                                  ByVal dblPlanValue1 As Double, _
                                  ByVal dblPlanValue2 As Double, _
                                  ByVal dblPrice As Double, _
                                  ByVal dblCalcPrice As Double, _
                                  ByVal intCodeSetPrice As Integer, _
                                  ByVal intCodeTax As Integer, _
                                  ByVal strName As String, _
                                  ByVal strName_EN As String, _
                                  ByVal strName_DE As String, _
                                  ByVal strName_FR As String, _
                                  ByVal strName_IT As String, _
                                  ByRef intIDDetail As Integer) As enumEgswErrorCode

        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_UPDATEDetailByID]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intID", SqlDbType.Int).Value = intID
                .Parameters.Add("@intIDMain", SqlDbType.Int).Value = intIDMain
                .Parameters.Add("@intDayPlan", SqlDbType.Int).Value = intDayPlan
                .Parameters.Add("@intCodeMasterPlan", SqlDbType.Int).Value = intCodeMasterPlan
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@bitIsLock", SqlDbType.Bit).Value = blnIsLock
                .Parameters.Add("@fltPlanValue1", SqlDbType.Float).Value = dblPlanValue1
                .Parameters.Add("@fltPlanValue2", SqlDbType.Float).Value = dblPlanValue2
                .Parameters.Add("@fltPrice", SqlDbType.Float).Value = dblPrice
                .Parameters.Add("@fltCalcPrice", SqlDbType.Float).Value = dblCalcPrice
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice
                .Parameters.Add("@intCodeTax", SqlDbType.Int).Value = intCodeTax
                .Parameters.Add("@nvcName", SqlDbType.NVarChar).Value = strName
                .Parameters.Add("@nvcName_EN", SqlDbType.NVarChar).Value = strName_EN
                .Parameters.Add("@nvcName_DE", SqlDbType.NVarChar).Value = strName_DE
                .Parameters.Add("@nvcName_FR", SqlDbType.NVarChar).Value = strName_FR
                .Parameters.Add("@nvcName_IT", SqlDbType.NVarChar).Value = strName_IT
                .Parameters.Add("@intIDDetail", SqlDbType.Int).Direction = ParameterDirection.Output

                .Connection.Open()
                .ExecuteNonQuery()

                intIDDetail = CInt(.Parameters("@intIDDetail").Value)

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


    Public Function UpdateDeadline(ByVal intCodeUser As Integer, ByVal dtsStartDate As DateTime, _
                                 ByVal dtsEndDate As DateTime, ByVal dtsDeadline As DateTime?,
                                 ByVal intCodeMasterMenuPlan As Integer,
                                 Optional ByVal intCodeMenuPlan As Integer = -1,
                                 Optional ByVal intIDMain As Integer = -1) As Integer

        Dim cmd As New SqlCommand
        'Dim intIDMain As Integer = -1

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_UPDATEDeadline]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeUser", SqlDbType.NVarChar).Value = intCodeUser
                .Parameters.Add("@dtsStartDate", SqlDbType.DateTime).Value = dtsStartDate
                .Parameters.Add("@dtsEndDate", SqlDbType.DateTime).Value = dtsEndDate
                .Parameters.Add("@dtsDeadline", SqlDbType.DateTime).Value = dtsDeadline
                .Parameters.Add("@intCodeMasterMenuPlan", SqlDbType.Int).Value = intCodeMasterMenuPlan
                .Parameters.Add("@intCodeMenuPlan", SqlDbType.Int).Value = intCodeMenuPlan
                .Parameters.Add("@intIDMain", SqlDbType.Int).Direction = ParameterDirection.InputOutput
                .Parameters("@intIDMain").Value = intIDMain

                .Connection.Open()
                .ExecuteNonQuery()

                intIDMain = CInt(.Parameters("@intIDMain").Value)

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
        Return intIDMain
    End Function

    Public Function ResetDeadline(ByVal intCodeUser As Integer, ByVal dtsStartDate As DateTime, _
                                 ByVal dtsEndDate As DateTime,
                                 Optional ByVal intIDMain As Integer = -1) As Integer

        Dim cmd As New SqlCommand
        'Dim intIDMain As Integer = -1

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_RESETDeadline]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeUser", SqlDbType.NVarChar).Value = intCodeUser
                .Parameters.Add("@dtsStartDate", SqlDbType.DateTime).Value = dtsStartDate
                .Parameters.Add("@dtsEndDate", SqlDbType.DateTime).Value = dtsEndDate
                .Parameters.Add("@intIDMain", SqlDbType.Int).Direction = ParameterDirection.InputOutput
                .Parameters("@intIDMain").Value = intIDMain

                .Connection.Open()
                .ExecuteNonQuery()

                intIDMain = CInt(.Parameters("@intIDMain").Value)

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
        Return intIDMain
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

    Public Function fctGetMasterMenuPlanOfRestaurant(ByVal intCodeRestaurant As Integer) As Object
        Try
            Dim sbSQL As New StringBuilder
            sbSQL.Append("SELECT CodeMasterMenuPlan FROM EgswMPRestaurant ")
            sbSQL.Append("WHERE Code=" & intCodeRestaurant)

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

    Public Function fctGetMPDataset(ByVal intIDMAin As Integer, ByVal intCodeRestaurant As Integer) As Object
        Dim strCommandText As String = "MP_GETPRINT_PLAN"
        Dim arrParam(1) As SqlParameter

        arrParam(0) = New SqlParameter("@IDMain", intIDMAin)
        arrParam(1) = New SqlParameter("@CodeRestaurant", intCodeRestaurant)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function fctMenuPlanDetails(ByVal strIDs As String, ByVal intCodeTrans As Integer) As Object
        Dim strCommandText As String = "MP_GETPRINT"
        Dim arrParam(1) As SqlParameter

        arrParam(0) = New SqlParameter("@IDs", strIDs)
        arrParam(1) = New SqlParameter("@CodeTrans", intCodeTrans)

        'Try
        'Select Case L_bytFetchType
        'Case enumEgswFetchType.DataReader
        '    Return ExecuteReader(L_strCnn, CommandType.Text, strCommandText, arrParam)
        'Case enumEgswFetchType.DataSet
        'Return ExecuteDataset(L_strCnn, CommandType.Text, strCommandText, arrParam)
        'Case enumEgswFetchType.DataTable
        Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam).Tables(0)
        'End Select

        Return Nothing
        'Catch ex As Exception
        'Throw ex
        'End Try
    End Function

    Public Function fctGetMPDatasetForDataList(ByVal intYear As Integer, ByVal intPage As Integer, ByVal intRecsPerPage As Integer, Optional ByVal intCodeRestaurant As Integer = -1) As Object
        Dim strCommandText As String = "MP_GetMPListForPrint"
        Dim arrParam(3) As SqlParameter

        arrParam(0) = New SqlParameter("@Year", intYear)
        arrParam(1) = New SqlParameter("@Page", intPage)
        arrParam(2) = New SqlParameter("@RecsPerPage", intRecsPerPage)
        arrParam(3) = New SqlParameter("@intCodeRestaurant", intCodeRestaurant)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetDeadlineByIDMain(ByVal intIDMain As Integer) As DateTime
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dtsDeadline As DateTime
        Dim dr As SqlDataReader

        Dim sb As New StringBuilder
        sb.Append("SELECT Deadline FROM EgswMPMain WHERE ID=" & intIDMain)

        Try
            With cmd
                .Connection = cn
                .CommandType = CommandType.Text
                .CommandText = sb.ToString

                .Connection.Open()
                dr = .ExecuteReader()
                While dr.Read
                    dtsDeadline = CDateDB(dr.Item("Deadline"))
                End While
                dr.Close()
                .Connection.Close()
                .Connection.Dispose()
            End With
            Return dtsDeadline
        Catch ex As Exception
            Return dtsDeadline
            cmd.Connection.Dispose()
        End Try
    End Function

    Public Function GetMenuRecipeDetails(ByVal IDDmain As String, ByVal intCodeTrans As Integer, ByVal intCodeRestaurant As Integer) As DataTable 'PJRB 2016.08.31 
        Return GetMenuRecipeDetails2(IDDmain, intCodeTrans, intCodeRestaurant)
    End Function

    Public Function GetMenuRecipeDetails(ByVal IDDmain As String, ByVal intCodeTrans As Integer, ByVal intCodeRestaurant As Integer, Optional ByVal boolIsAllRecipes As Boolean = False) As DataTable 'PJRB 2016.08.31 
        Return GetMenuRecipeDetails2(IDDmain, intCodeTrans, intCodeRestaurant, boolIsAllRecipes)
    End Function

    Public Function GetMenuRecipeDetails2(ByVal IDDmain As String, ByVal intCodeTrans As Integer, ByVal intCodeRestaurant As Integer, Optional ByVal boolIsAllRecipes As Boolean = False) As DataTable 'PJRB 2016.08.31 

        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_GetMenuRecipeDetails"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intIDMain", SqlDbType.Int).Value = IDDmain
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@boolAllRecipes", SqlDbType.Bit).Value = boolIsAllRecipes
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
            GetMenuRecipeDetails2 = Nothing
        End Try

    End Function

    Public Function GetMenuRecipeForReport(CodeMenuPlan As String, Recurrence As Integer, IDMain As Integer, DayPlan As Integer, IDDetail As Integer, CodeUser As Integer,
                                           ByRef IDMainPrint As Integer, CodeRestaurant As Integer) As Object
        Dim arrParam(7) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeMenuPlan", CodeMenuPlan)
        arrParam(1) = New SqlParameter("@Recurrence", Recurrence)
        arrParam(2) = New SqlParameter("@IDMain", IDMain)
        arrParam(3) = New SqlParameter("@DayPlan", DayPlan)
        arrParam(4) = New SqlParameter("@IDDetail", IDDetail)
        arrParam(5) = New SqlParameter("@CodeUser", CodeUser)
        arrParam(6) = New SqlParameter("@IDMain2", IDMainPrint)
        arrParam(7) = New SqlParameter("@CodeRestaurant", CodeRestaurant)
        arrParam(6).Direction = ParameterDirection.InputOutput

        Dim dt = ExecuteFetchType(L_bytFetchType, L_strCnn, CommandType.StoredProcedure, "MP_GetMenuRecipeForReport", arrParam)

        IDMainPrint = CInt(arrParam(6).Value)

        Return dt
    End Function

    Public Function GetCodeMasterMenuPlanByMenuPlanRestaurantCode(ByVal intCode As Integer, ByVal isResto As Boolean) As Integer
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim intReturnValue As Integer
        Dim dr As SqlDataReader

        Dim sb As New StringBuilder
        If isResto = True Then
            sb.Append("SELECT CodeMasterMenuPlan FROM EgswMPRestaurant WHERE Code = " & intCode)
        Else
            sb.Append("SELECT CodeMasterMenuPlan FROM EgswMPRestaurant WHERE Code = (SELECT CodeRestaurant FROM EgswMPMenuPlan WHERE Code =" & intCode & ")")
        End If



        Try
            With cmd
                .Connection = cn
                .CommandType = CommandType.Text
                .CommandText = sb.ToString

                .Connection.Open()
                dr = .ExecuteReader()
                While dr.Read
                    intReturnValue = CInt(dr.Item("CodeMasterMenuPlan"))
                End While
                dr.Close()
                .Connection.Close()
                .Connection.Dispose()
            End With
            Return intReturnValue
        Catch ex As Exception
            Return intReturnValue
            cmd.Connection.Dispose()
        End Try
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

    Public Function DeActivateRestaurant(ByVal intCodeRestaurant As Integer) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_DEACTIVATERestaurant]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant

                .Connection.Open()
                .ExecuteNonQuery()
                .Connection.Close()

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



    Public Function GetMasterMenuPlan() As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "SELECT Code, Name FROM EgswMPMasterMenuPlan"
                .CommandType = CommandType.Text
                .Connection.Open()

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
            GetMasterMenuPlan = Nothing
        End Try

    End Function

    Public Function GetAllRestaurants(ByVal intCodeSite As Integer) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_GetAllRestaurants"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
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
            GetAllRestaurants = Nothing
        End Try

    End Function


    '''============================================================================================================    
    ''' <summary>
    ''' MIGROS MASTER PLAN
    ''' </summary>
    ''' <param name="tntDisplayMode"></param>    
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 

    Public Function GetRestaurant(ByVal intCodeSite As Integer, Optional ByVal intCodeRestaurant As Integer = -1, Optional ByVal strName As String = "", Optional ByVal intCodeMasterMenuPlan As Integer = -1) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_GetRestaurant"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 200).Value = strName
                .Parameters.Add("@intCodeMasterMenuPlan", SqlDbType.Int).Value = intCodeMasterMenuPlan
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

    Public Function GetMasterPlan(Optional ByVal intCodeRestaurant As Integer = -1, Optional ByVal intCodeTrans As Integer = -1, Optional ByVal intCodeMasterMenuPlan As Integer = 1) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_GetMasterPlan"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@intCodeMasterMenuPlan", SqlDbType.Int).Value = intCodeMasterMenuPlan
                'If intCodeRestaurant > 0 Then
                '    .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                'End If
                If intCodeTrans > 0 Then
                    .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
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

    Public Function GetMasterPlanEatCH(Optional ByVal intCodeRestaurant As Integer = -1, Optional ByVal intCodeTrans As Integer = -1, Optional ByVal intCodeMasterMenuPlan As Integer = 1) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_GETMasterPlanEatCH"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@intCodeMasterMenuPlan", SqlDbType.Int).Value = intCodeMasterMenuPlan
                'If intCodeRestaurant > 0 Then
                '    .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                'End If
                If intCodeTrans > 0 Then
                    .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
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
            GetMasterPlanEatCH = Nothing
        End Try

    End Function

    Public Function GetMasterPlanByCodeUser(ByVal intCodeUser As Integer, ByVal intCodeMasterMenuPlan As Integer) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_GETMasterPlanByCodeUser"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@intCodeMasterMenuPlan", SqlDbType.Int).Value = intCodeMasterMenuPlan
                'If intCodeRestaurant > 0 Then
                '    .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                'End If
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
            GetMasterPlanByCodeUser = Nothing
        End Try

    End Function
    Public Function GetBanner(Optional ByVal intCodeRestaurant As Integer = -1, Optional used As Boolean = False, Optional codeSite As Integer = -1) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_GETBannerLayout"
                .CommandType = CommandType.StoredProcedure
                If intCodeRestaurant > 0 Then
                    .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                End If
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = codeSite
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
        End Try

    End Function

    Public Function GetLanguage(Optional ByVal intCodeRestaurant As Integer = -1, Optional used As Boolean = False, Optional codeSite As Integer = -1) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_GetLanguage"
                .CommandType = CommandType.StoredProcedure
                If intCodeRestaurant > 0 Then
                    .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                End If
                .Parameters.Add("@CodeSite", SqlDbType.Int).Value = codeSite
                .Connection.Open()
                .ExecuteNonQuery()

                With da
                    .SelectCommand = cmd
                    dt.BeginLoadData()
                    .Fill(dt)
                    dt.EndLoadData()
                End With

                If used Then
                    Dim dt2 = dt.Clone
                    For Each row In dt.Select("Flag=1")
                        dt2.ImportRow(row)
                    Next
                    Return dt2
                Else
                    Return dt
                End If
            End With
            cmd.Connection.Close()
            cmd.Dispose()
        Catch ex As Exception
            cmd.Dispose()
            GetLanguage = Nothing
        End Try

    End Function
    Public Function GetPrintWeek(ByVal intIDMain As Integer, ByVal intCodeRestaurant As Integer, ByVal intCodeTrans As Integer) As DataSet
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim ds As New DataSet

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_GETDetailsForPrintWeek]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intIDMain", SqlDbType.Int).Value = intIDMain
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans

                .Connection.Open()
                .ExecuteNonQuery()

                With da
                    .SelectCommand = cmd
                    .Fill(ds)

                End With
                Return ds
            End With
        Catch ex As Exception
            GetPrintWeek = Nothing
        Finally
            cmd.Connection.Close()
            cmd.Connection.Dispose()
        End Try
    End Function
    Public Function GetPrint(ByVal strIDs As String, ByVal intCodeTrans As Integer, Optional ByVal intCodeResto As Integer = -1) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_GETPRINT]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@nvcIDDetails", SqlDbType.NVarChar, -1).Value = strIDs
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans

                If intCodeResto <> -1 Then
                    .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeResto
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
        Catch ex As Exception
            GetPrint = Nothing
        Finally
            cmd.Connection.Close()
            cmd.Connection.Dispose()
        End Try

    End Function
    Public Function GetRestaurantByUser(ByVal intCodeUser As Integer, ByRef blnIsQ As Boolean, ByRef intCodeRestaurantHQ As Integer) As Integer

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim intCodeRestaurant As Integer = -1
        Dim bHQ As Boolean
        'Dim dr As SqlDataReader

        'Dim sb As New StringBuilder
        'sb.Append("SELECT CodeRestaurant, R.Name, ISNULL(IsHQ,0) AS IsHQ, CodeHQ = (SELECT Code FROM EgswMPRestaurant WHERE CodeSite=(SELECT CodeSite FROM EgswMPRestaurant WHERE Code=CodeRestaurant) AND IsHQ=1) ")
        'sb.Append("FROM EgswUser U INNER JOIN EgswMPRestaurant R ON U.CodeRestaurant=R.Code ")
        'sb.Append("WHERE U.Code=" & intCodeUser)

        Try
            With cmd
                .Connection = cn
                .CommandType = CommandType.StoredProcedure
                .CommandText = "[MP_GETRestaurantByCodeUser]"
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@bHQ", SqlDbType.Bit).Direction = ParameterDirection.Output
                .Parameters.Add("@intHQ", SqlDbType.Int).Direction = ParameterDirection.Output

                .Connection.Open()
                .ExecuteNonQuery()
                'dr = .ExecuteReader()
                intCodeRestaurant = CIntDB(.Parameters("@intCodeRestaurant").Value)
                blnIsQ = CBoolDB(.Parameters("@bHQ").Value)
                intCodeRestaurantHQ = CIntDB(.Parameters("@intHQ").Value)
                'While dr.Read
                '    intCodeRestaurant = CIntDB(dr.Item("CodeRestaurant"))
                '    intCodeRestaurantHQ = CIntDB(dr.Item("CodeHQ"))
                '    blnIsQ = CBoolDB(dr.Item("IsHQ"))
                'End While

            End With
            Return intCodeRestaurant
        Catch ex As Exception
            Return intCodeRestaurant
        Finally
            'dr.Close()
            cmd.Connection.Close()
            cmd.Connection.Dispose()
        End Try

    End Function

    Public Function GetMasterMenuPlanByUser(ByVal intCodeUser As Integer, ByRef intCodeMasterMenuPlan As Integer) As Integer
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand


        Try
            With cmd
                .Connection = cn
                .CommandType = CommandType.StoredProcedure
                .CommandText = "[MP_GetMasterMenuPlanByUser]"
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                .Parameters.Add("@intCodeMasterMenuPlan", SqlDbType.Int).Direction = ParameterDirection.Output

                .Connection.Open()
                .ExecuteNonQuery()
                intCodeMasterMenuPlan = CIntDB(.Parameters("@intCodeMasterMenuPlan").Value)


            End With
            Return intCodeMasterMenuPlan
        Catch ex As Exception
            Return intCodeMasterMenuPlan
        Finally
            'dr.Close()
            cmd.Connection.Close()
            cmd.Connection.Dispose()
        End Try
    End Function

    Public Function GetMasterMenuPlanByMenuPlan(ByVal codeMenuPlan As Integer) As Integer
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim intCodeMasterMenuPlan As Integer = -1
        Dim dr As SqlDataReader

        Dim sb As New StringBuilder
        sb.Append("SELECT CodeMasterMenuPlan FROM EgswMPrestaurant WHERE code = (select CodeRestaurant from egswmpmenuplan where code = " + codeMenuPlan.ToString + " )")

        Try
            With cmd
                .Connection = cn
                .CommandType = CommandType.Text
                .CommandText = sb.ToString

                .Connection.Open()
                dr = .ExecuteReader()
                While dr.Read
                    intCodeMasterMenuPlan = CIntDB(dr.Item("CodeMasterMenuPlan"))
                End While
                dr.Close()
                .Connection.Close()
                .Connection.Dispose()
            End With
            Return intCodeMasterMenuPlan
        Catch ex As Exception
            Return intCodeMasterMenuPlan
            cmd.Connection.Dispose()
        End Try
    End Function

    Public Function GetHQ(ByVal intCodeSite As Integer) As Integer
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim intCodeHQ As Integer = -1
        Dim dr As SqlDataReader

        Dim sb As New StringBuilder
        sb.Append("SELECT Code FROM EgswMPRestaurant WHERE IsHQ=1 AND [Status]=1 AND CodeSite=" & intCodeSite)

        Try
            With cmd
                .Connection = cn
                .CommandType = CommandType.Text
                .CommandText = sb.ToString

                .Connection.Open()
                dr = .ExecuteReader()
                While dr.Read
                    intCodeHQ = CIntDB(dr.Item("Code"))
                End While
                dr.Close()
                .Connection.Close()
                .Connection.Dispose()
            End With
            Return intCodeHQ
        Catch ex As Exception
            Return intCodeHQ
            cmd.Connection.Dispose()
        End Try
    End Function


    Public Function GetDetailDataByDetail(ByVal intIDDetail As Integer, ByVal intCodeTrans As Integer,
                                          Optional ByVal blnUseImposedPrice As Boolean = 0,
                                          Optional ByVal intCodeSetPrice As Integer = 1) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_GETDetailDataByDetail"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intIDDetail", SqlDbType.Int).Value = intIDDetail
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@bitUseImposedPrice", SqlDbType.Int).Value = blnUseImposedPrice
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice

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
            GetDetailDataByDetail = Nothing
        End Try

    End Function

    Public Function GetListeTranslations(ByVal intCodeListe As Integer) As SqlDataReader
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeListe", intCodeListe)
        Try
            Return ExecuteReader(L_strCnn, CommandType.StoredProcedure, "[MP_GETListeTranslations]", arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetListeTranslations(ByVal intCodeListe As Integer, getAll As Boolean) As DataTable
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeListe", intCodeListe)
        arrParam(1) = New SqlParameter("@getAll", getAll)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "[MP_GETListeTranslations]", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetMasterPlanByCodeRestaurant(ByVal intCodeTrans As Integer, Optional ByVal intCodeRestaurant As Integer = -1) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_GETMasterPlanByCodeRestaurant"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans

                If intCodeRestaurant > 0 Then
                    .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
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
            GetMasterPlanByCodeRestaurant = Nothing
        End Try

    End Function

    Public Function GetMain(ByVal dtsStartDate As DateTime, ByVal dtsEndDate As DateTime, ByVal intCodeRestaurant As Integer) As Integer

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim intCodeMain As Integer = -1
        Dim dr As SqlDataReader

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_GETMain]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@dtsStartDate", SqlDbType.DateTime).Value = dtsStartDate
                .Parameters.Add("@dtsEndDate", SqlDbType.DateTime).Value = dtsEndDate
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant

                .Connection.Open()
                dr = .ExecuteReader()
                While dr.Read
                    intCodeMain = CIntDB(dr.Item("ID"))
                End While

            End With
            Return intCodeMain
        Catch ex As Exception
            Return intCodeMain
        Finally
            dr.Close()
            cmd.Connection.Close()
            cmd.Connection.Dispose()
        End Try

    End Function


    Public Function GetMainDetail(ByVal dtsStartDate As DateTime, ByVal dtsEndDate As DateTime, ByVal intCodeRestaurant As Integer, ByVal intCodeMenuPlan As Integer, Optional ByVal intIDMain As Integer = -1) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_GETMain]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@dtsStartDate", SqlDbType.DateTime).Value = dtsStartDate
                .Parameters.Add("@dtsEndDate", SqlDbType.DateTime).Value = dtsEndDate
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@intCodeMenuPlan", SqlDbType.Int).Value = intCodeMenuPlan
                .Parameters.Add("@intIDMain", SqlDbType.Int).Value = IIf(intIDMain > 0, intIDMain, DBNull.Value)

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
            Return dt
        Catch ex As Exception
            Return dt
        Finally
            cmd.Connection.Close()
            cmd.Connection.Dispose()
        End Try

    End Function


    Public Function GetMain(ByVal dtsStartDate As DateTime, ByVal dtsEndDate As DateTime, ByVal intCodeRestaurant As Integer, ByVal intCodeMenuPlan As Integer, Optional ByVal intIDMain As Integer = -1) As Integer

        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim intCodeMain As Integer = -1
        Dim dr As SqlDataReader

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_GETMain]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@dtsStartDate", SqlDbType.DateTime).Value = dtsStartDate
                .Parameters.Add("@dtsEndDate", SqlDbType.DateTime).Value = dtsEndDate
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@intCodeMenuPlan", SqlDbType.Int).Value = intCodeMenuPlan
                .Parameters.Add("@intIDMain", SqlDbType.Int).Value = IIf(intIDMain > 0, intIDMain, DBNull.Value)

                .Connection.Open()
                dr = .ExecuteReader()
                While dr.Read
                    intCodeMain = CIntDB(dr.Item("ID"))
                End While

            End With
            Return intCodeMain
        Catch ex As Exception
            Return intCodeMain
        Finally
            dr.Close()
            cmd.Connection.Close()
            cmd.Connection.Dispose()
        End Try

    End Function

    Public Function GetDeadline(ByVal dtsStartDate As DateTime, ByVal dtsEndDate As DateTime, ByVal intCodeRestaurant As Integer) As DateTime
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dtsDeadline As DateTime
        Dim dr As SqlDataReader

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_GETMain]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@dtsStartDate", SqlDbType.DateTime).Value = dtsStartDate
                .Parameters.Add("@dtsEndDate", SqlDbType.DateTime).Value = dtsEndDate
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant

                .Connection.Open()
                dr = .ExecuteReader()
                While dr.Read
                    dtsDeadline = CDateDB(dr.Item("Deadline"))
                End While

            End With
            Return dtsDeadline
        Catch ex As Exception
            Return dtsDeadline
        Finally
            dr.Close()
            cmd.Connection.Close()
            cmd.Connection.Dispose()
        End Try

    End Function

    Public Function GetDeadline(ByVal dtsStartDate As DateTime, ByVal dtsEndDate As DateTime, ByVal intCodeRestaurant As Integer, ByVal intCodeMenuPlan As Integer, ByVal intIDMain As Integer) As DateTime
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dtsDeadline As DateTime
        Dim dr As SqlDataReader

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_GETMain]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@dtsStartDate", SqlDbType.DateTime).Value = dtsStartDate
                .Parameters.Add("@dtsEndDate", SqlDbType.DateTime).Value = dtsEndDate
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@intCodeMenuPlan", SqlDbType.Int).Value = intCodeMenuPlan
                .Parameters.Add("@intIDMain", SqlDbType.Int).Value = intIDMain

                .Connection.Open()
                dr = .ExecuteReader()
                While dr.Read
                    dtsDeadline = CDateDB(dr.Item("Deadline"))
                End While

            End With
            Return dtsDeadline
        Catch ex As Exception
            Return dtsDeadline
        Finally
            dr.Close()
            cmd.Connection.Close()
            cmd.Connection.Dispose()
        End Try

    End Function

    Public Function GetDetailByMain(ByVal intIDMain As Integer, Optional ByVal intDayPlan As Integer = -1, Optional ByVal strCodeMasterplan As String = "") As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_GETDetailByMain]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intIDMain", SqlDbType.Int).Value = intIDMain
                .Parameters.Add("@intDay", SqlDbType.Int).Value = intDayPlan
                .Parameters.Add("@codeMasterPlanList", SqlDbType.NVarChar, 1000).Value = strCodeMasterplan

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
            GetDetailByMain = Nothing
        End Try

    End Function

    Public Function GetDetailByCodeRestaurant(ByVal intCodeRestaurant As Integer, Optional ByVal blnUseImposedPrice As Boolean = False,
                                                     Optional ByVal intCodeNutrientSet As Integer = 0, Optional ByVal intCodeSetPrice As Integer = 1,
                                                     Optional ByVal intCodeTrans As Integer = 0) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_GETDetailByCodeRestaurant]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@bitUseImposedPrice", SqlDbType.Int).Value = blnUseImposedPrice
                .Parameters.Add("@intCodeNutrientSet", SqlDbType.Int).Value = intCodeNutrientSet
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans

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
            GetDetailByCodeRestaurant = Nothing
        End Try

    End Function

    Public Function GetDetailByMainAndCodeRestaurant(ByVal intIDMain As Integer, ByVal intCodeRestaurant As Integer, Optional ByVal blnUseImposedPrice As Boolean = False,
                                                     Optional ByVal intCodeNutrientSet As Integer = 0, Optional ByVal intCodeSetPrice As Integer = 1,
                                                     Optional ByVal intCodeTrans As Integer = 0) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandTimeout = "9999"
                .CommandText = "[MP_GETDetailByMainAndCodeRestaurant]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intIDMain", SqlDbType.Int).Value = intIDMain
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@bitUseImposedPrice", SqlDbType.Int).Value = blnUseImposedPrice
                .Parameters.Add("@intCodeNutrientSet", SqlDbType.Int).Value = intCodeNutrientSet
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans

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
            GetDetailByMainAndCodeRestaurant = Nothing
        End Try

    End Function
    Public Function GetIDDetailByDate(ByVal strDate As String, ByVal intDayPlan As Integer, ByVal intMasterPlan As Integer, ByVal intCodeRestaurant As Integer, Optional ByVal codeMenuPlan As Integer? = Nothing) As Integer
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_GETMPIDDetailsByDate]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@strDate", SqlDbType.DateTime).Value = strDate
                .Parameters.Add("@intDayPlan", SqlDbType.Int).Value = intDayPlan
                .Parameters.Add("@intCodeMasterPlan", SqlDbType.Int).Value = intMasterPlan
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@intIDDetail", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@CodeMenuPlan", SqlDbType.Int).Value = codeMenuPlan

                .Connection.Open()
                .ExecuteNonQuery()

                GetIDDetailByDate = CIntDB(.Parameters("@intIDDetail").Value)
                Return GetIDDetailByDate
            End With
            cmd.Connection.Close()
            cmd.Dispose()
        Catch ex As Exception
            cmd.Dispose()
            GetIDDetailByDate = -1
        End Try
    End Function
    Public Function GetDetailByIDDetail(ByVal intIDDetail As Integer, Optional ByVal intCodeSetPrice As Integer = 1) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_GETDetailByIDDetail]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intIDDetail", SqlDbType.Int).Value = intIDDetail
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetPrice
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
            GetDetailByIDDetail = Nothing
        End Try

    End Function

    Public Function GetPlanValueSummary(ByVal intIDMain As Integer, ByVal intCodeMasterPlan As Integer, ByVal intDayPlan As Integer, ByVal intCodeRestaurant As Integer) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_GETPlanValueSummary]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intIDMain", SqlDbType.Int).Value = intIDMain
                .Parameters.Add("@intCodeMasterPlan", SqlDbType.Int).Value = intCodeMasterPlan
                .Parameters.Add("@intDayPlan", SqlDbType.Int).Value = intDayPlan
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant

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
            GetPlanValueSummary = Nothing
        End Try

    End Function


    Public Function UpdateRestauranMasterPlan(ByVal intCodeRestaurant As Integer, Optional ByVal intCodeMasterPlan As Integer = -1) As Integer

        Dim cmd As New SqlCommand
        Dim intCode As Integer = 0

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_UPDATERestaurantMasterPlan]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@intCodeMasterPlan", SqlDbType.Int).Value = intCodeMasterPlan

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

    Public Function UpdateRestauranMasterPlanEatCh(ByVal intCodeRestaurant As Integer, Optional ByVal intCodeMasterPlan As Integer = -1) As Integer

        Dim cmd As New SqlCommand
        Dim intCode As Integer = 0

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_UPDATERestaurantMasterPlanEATCH]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@intCodeMasterPlan", SqlDbType.Int).Value = intCodeMasterPlan

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

    Public Function UpdateRestaurantEatCH(ByRef intCodeRestaurant As Integer, ByVal strName As String, ByVal strLanguage As String, ByVal intCodeSite As Integer, ByVal blnIsHQ As Boolean, ByVal intCodeMasterMenuPlan As Integer, ByVal strBanner As String, ByVal blnIsEatChRegistered As Boolean) As Integer

        Dim cmd As New SqlCommand
        Dim intCode As Integer = -1

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_UPDATERestaurant]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 200).Value = strName
                .Parameters.Add("@nvcLanguage", SqlDbType.NVarChar, 200).Value = strLanguage
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@bitIsHQ", SqlDbType.Bit).Value = blnIsHQ
                .Parameters.Add("@intCodeMasterMenuPlan", SqlDbType.Int).Value = intCodeMasterMenuPlan
                .Parameters.Add("@nvcBanner", SqlDbType.NVarChar, 200).Value = strBanner
                .Parameters.Add("@bitIsEatCHRegistered", SqlDbType.Bit).Value = blnIsEatChRegistered
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

    Public Function UpdateRestaurant(ByRef intCodeRestaurant As Integer, ByVal strName As String, ByVal strLanguage As String, ByVal intCodeSite As Integer, ByVal blnIsHQ As Boolean, ByVal intCodeMasterMenuPlan As Integer, ByVal strBanner As String) As Integer

        Dim cmd As New SqlCommand
        Dim intCode As Integer = -1

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_UPDATERestaurant]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 200).Value = strName
                .Parameters.Add("@nvcLanguage", SqlDbType.NVarChar, 200).Value = strLanguage
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@bitIsHQ", SqlDbType.Bit).Value = blnIsHQ
                .Parameters.Add("@intCodeMasterMenuPlan", SqlDbType.Int).Value = intCodeMasterMenuPlan
                .Parameters.Add("@nvcBanner", SqlDbType.NVarChar, 200).Value = strBanner
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


    Public Function AddLocalMasterPlanOnMasterPlanSave(ByVal intCodeRestaurant As Integer, ByVal intCodeMasterMenuPlan As Integer) As Integer

        Dim cmd As New SqlCommand
        Dim intCode As Integer = -1

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_UpdateMasterPlanWithLocalMasterplan]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeRestau", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@intCodeMasterMenuPlan", SqlDbType.Int).Value = intCodeMasterMenuPlan

                .Connection.Open()
                .ExecuteNonQuery()

                cmd.Connection.Close()
                Return 1
            End With
        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
            Return -1

        Finally
            cmd.Dispose()
        End Try

    End Function

    Public Function UpdateMasterPlan(ByRef intCodeMasterPlan As Integer, Optional ByVal strName As String = "", Optional ByVal strBGColor As String = "",
                                     Optional ByVal intCodeSite As Integer = -1, Optional ByVal CodeGroup As Integer = -1, Optional ByVal CodeMasterMenuPlan As Integer = 1) As enumEgswErrorCode

        Dim cmd As New SqlCommand
        Dim intCode As Integer = -1

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_UPDATEMasterPlan]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeMasterPlan", SqlDbType.Int).Value = intCodeMasterPlan
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 200).Value = strName
                .Parameters.Add("@nvcBGColor", SqlDbType.NVarChar, 200).Value = strBGColor
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@intReturnID", SqlDbType.Int).Direction = ParameterDirection.Output
                .Parameters.Add("@CodeGroup", SqlDbType.Int).Value = CodeGroup
                .Parameters.Add("@intCodeMasterMenuPlan", SqlDbType.Int).Value = CodeMasterMenuPlan

                .Connection.Open()
                .ExecuteNonQuery()

                intCodeMasterPlan = CIntDB(.Parameters("@intReturnID").Value)

                cmd.Connection.Close()

                If intCodeMasterPlan > 0 Then
                    L_ErrCode = enumEgswErrorCode.OK
                Else
                    L_ErrCode = intCodeMasterPlan
                End If

            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            Throw New Exception(ex.Message, ex)
        Finally
            cmd.Dispose()
        End Try
        Return L_ErrCode
    End Function

    Public Function UpdateTranslation(ByRef intCode As Integer, ByRef intType As Integer, ByRef intCodeTrans As Integer, ByVal strName As String) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_UPDATETranslation]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@intType", SqlDbType.Int).Value = intType
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@nvcName", SqlDbType.NVarChar, 200).Value = strName

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

    Public Overloads Function RemoveRestaurant(Optional ByVal intCodeRestaurant As Integer = -1, Optional ByVal strCodeRestaurantList As String = "") As enumEgswErrorCode

        Dim cmd As New SqlCommand
        Dim intCode As Integer = -1

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_DELETERestaurant]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@nvcCodeRestaurantList", SqlDbType.NVarChar, 2000).Value = strCodeRestaurantList
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

    Public Function GetRecipeListeForMenuPlan(ByVal arrIDDetails() As String) As DataTable
        Dim dt As New DataTable("dtMenuRecipes")
        Dim dtRec As New DataTable
        dt.Columns.Add("CodeListe")
        dt.Columns.Add("Quantity")

        For Each strIDDetail As String In arrIDDetails
            dtRec = Nothing
            dtRec = GetRecipeListeForMenuPlanInd(strIDDetail)
            For Each dtRow As DataRow In dtRec.Rows
                dt.Rows.Add(dtRow.Item("Codeliste"), dtRow.Item("Quantity"))
            Next
        Next
        Return dt
    End Function
    Public Function GetRecipeListeForMenuPlanInd(ByVal strIDDetails As String) As DataTable
        Dim strCommandText As String = "[mp_getmenurecipes]"
        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@IDDetails", strIDDetails)

        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try

    End Function
    Public Function GetListComputedByYieldMasterPlan(ByVal dtCodeliste As DataTable, ByVal intFirstCodeSetPrice As Integer, ByVal intCodeUser As Integer, ByVal intCodeTrans As Integer, ByVal L_udtUser As structUser, Optional ByVal dblYield As Double = -1, Optional ByVal blnGroup As Boolean = False) As DataTable
        '// Create Table to Store Ingredients 
        Dim cShop As New clsShopping(L_udtUser, enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataTable)
        Dim dt As New DataTable("Ing")
        Dim dt2 As New DataTable("sdf")
        With dt.Columns
            .Add("codeliste")
            .Add("name")
            .Add("number")
            .Add("netQty")
            .Add("grossQty")
            .Add("itemUnitName")
            .Add("symbole")
            .Add("itemPrice")
            .Add("itemCost")
            .Add("itemFormat")
            .Add("priceFormat")
            .Add("itemUnitCode")
            .Add("secondcodesetprice")
            .Add("priceUnit")
            .Add("ItemPriceUnitCode")
            .Add("itemType")
        End With
        '// Get Ingredients of each recipe in array
        Dim cListe As New clsMasterPlan(L_udtUser, enumAppType.WebApp, L_strCnn)
        'Dim dr As SqlDataReader
        Dim dt4 As DataTable
        Dim row As DataRow
        Dim bUseBestUnit As Boolean = False
        'If Not blnGroup Then bUseBestUnit = L_udtUser.UseBestUnit
        Dim intCodeListe As Integer = 0
        Dim intComp As Integer = 0

        If dtCodeliste.Rows.Count > 0 Then
            For Each dtCodeRow As DataRow In dtCodeliste.Rows
                dt4 = Nothing
                intCodeListe = CInt(dtCodeRow.Item(0))
                dt4 = cListe.GetMasterPlanShoppingList(intCodeTrans, intFirstCodeSetPrice, intCodeListe, intCodeUser, CInt(bUseBestUnit), 1)
                For Each dr As DataRow In dt4.Rows
                    ' only add Ingredients
                    If CType(dr.Item("itemType"), enumDataListItemType) = enumDataListItemType.Merchandise Then
                        row = dt.NewRow
                        row("codeliste") = dr.Item("itemcode")
                        row("name") = dr.Item("itemname")
                        row("number") = dr.Item("itemnumber")
                        row("secondcodesetprice") = dr.Item("secondcodesetprice")
                        row("priceUnit") = dr.Item("priceUnit")
                        row("itemUnitCode") = dr.Item("itemUnitCode")
                        row("netQty") = dr.Item("netQuantity")
                        row("itemUnitName") = dr.Item("itemUnit")
                        row("itemFormat") = dr.Item("itemFormat")
                        row("grossQty") = dr.Item("grossQuantity")
                        row("itemCost") = dr.Item("itemCost")
                        row("symbole") = dr.Item("symbole")
                        row("priceFormat") = dr.Item("priceFormat")
                        If CDbl(dr.Item("netQuantity")) = 0 Then row("itemUnitName") = ""
                        row("itemPrice") = dr.Item("itemPrice")
                        row("ItemPriceUnitCode") = dr.Item("ItemPriceUnitCode")
                        row("itemType") = dr.Item("itemType")
                        dt.Rows.Add(row)
                    ElseIf CType(dr.Item("itemType"), enumDataListItemType) = enumDataListItemType.Recipe Then
                        ''GetListIngComputedByYield(dt, intCodeListe, CInt(dr.Item("itemcode")), intFirstCodeSetPrice, L_udtUser, CDbl(1.0))
                        GetListIngComputedByYield(dt, CInt(dtCodeRow("codeliste")), CInt(dr.Item("itemcode")), intFirstCodeSetPrice, CDbl(dtCodeRow("computedyield")))
                        'GetListIngComputedByYield(dt, CInt(dtCodeliste.Rows(i)("codeliste")), CInt(dr.Item("itemcode")), intFirstCodeSetPrice)
                    End If
                Next
            Next



            Dim ctr, cnt, cntr As Integer
            For ctr = 0 To dt.Rows.Count - 1
                If ctr >= dt.Rows.Count - 1 Then Exit For
                For cnt = ctr + 1 To dt.Rows.Count - 1
                    If cnt >= dt.Rows.Count - 1 Then Exit For
                    intComp = CInt(dt.Rows(cnt).Item("codeliste").ToString)
                    If Convert.ToInt32(dt.Rows(cnt).Item("codeliste").ToString) <> -1 Then

                        If dt.Rows(ctr).Item("codeliste").ToString = dt.Rows(cnt).Item("codeliste").ToString Then
                            dt.Rows(ctr).Item("netqty") = Val(dt.Rows(ctr).Item("netqty")) + Val(dt.Rows(cnt).Item("netqty"))
                            dt.Rows(ctr).Item("grossQty") = Val(dt.Rows(ctr).Item("grossQty")) + Val(dt.Rows(cnt).Item("grossQty"))
                            dt.Rows(ctr).Item("itemCost") = Val(CStrDB(dt.Rows(ctr).Item("itemCost"))) + Val(CStrDB(dt.Rows(cnt).Item("itemCost")))
                            dt.Rows.RemoveAt(cnt)
                            cnt -= 1
                        End If

                    End If
                Next



            Next


            Return dt
        Else
            Return Nothing
        End If


    End Function

    Public Function InsertShoppingList(ByVal intCodeShoppingList As Integer, ByVal intCodeListe As Integer, ByVal dblGrossQty As Double, ByVal dblNetQty As Double, ByVal intCodeUnit As Integer, ByVal intCodeUser As Integer, ByVal intCodeSetprice As Integer, ByVal intCodeProposal As Integer, ByVal intCodeMeal As Integer, ByVal intCodePlan As Integer, ByVal intCodeType As Integer, ByVal dateNow As DateTime, ByVal intCodeRestaurant As Integer) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        'Try
        With cmd
            .Connection = cn
            .CommandText = "MP_InsertShoppingList"
            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@intCodeShoppingList", SqlDbType.Int).Value = intCodeShoppingList
            .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@fltGrossQty", SqlDbType.Float).Value = dblGrossQty
            .Parameters.Add("@fltNetQty", SqlDbType.Float).Value = dblNetQty
            .Parameters.Add("@intCodeUnit", SqlDbType.Int).Value = intCodeUnit
            .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
            .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = intCodeSetprice
            .Parameters.Add("@intCodePlan", SqlDbType.Int).Value = intCodePlan
            .Parameters.Add("@intCodeMeal", SqlDbType.Int).Value = intCodeMeal
            .Parameters.Add("@intCodeProposal", SqlDbType.Int).Value = intCodeSetprice
            .Parameters.Add("@intCodeType", SqlDbType.Int).Value = intCodeType
            .Parameters.Add("@Date", SqlDbType.DateTime).Value = dateNow
            .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
            cn.Open()
            .ExecuteNonQuery()
            cn.Close()
            cn.Dispose()


        End With
        ' Catch ex As Exception
        '  cn.Close()
        '  cn.Dispose()
        '  Return enumEgswErrorCode.GeneralError
        '  End Try
    End Function

    Public Function InsertShoppingListDetailMP(ByRef intCode As Integer, ByVal strName As String, ByVal dtmDates As Date, ByVal strNote As String, ByVal intCodeUser As Integer, ByVal intCodeRestaurant As Integer) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        ' Try
        With cmd
            .Connection = cn
            .CommandText = "MP_InsertNewShoppingList"
            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@intCodeShoppingList", SqlDbType.Int).Value = intCode
            .Parameters.Add("@Name", SqlDbType.NVarChar, 260).Value = strName
            .Parameters.Add("@Note", SqlDbType.NVarChar, 1000).Value = strNote
            .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
            .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
            .Parameters.Add("@Date", SqlDbType.DateTime).Value = dtmDates



            '.Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

            cn.Open()
            .ExecuteNonQuery()
            cn.Close()
            cn.Dispose()



        End With
        'Catch ex As Exception
        '    cn.Close()
        '    cn.Dispose()
        '    Return enumEgswErrorCode.GeneralError
        'End Try
    End Function

    Public Function UpdateShoppingList(ByVal intCodeShoppingList As Integer, ByVal intCodeListe As Integer, ByVal dblGrossQty As Double, ByVal dblNetQty As Double, ByVal dateNow As DateTime, ByVal intCodeRestaurant As Integer) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand

        'Try
        With cmd
            .Connection = cn
            .CommandText = "MP_UpdateShoppingList"
            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@intCodeShoppingList", SqlDbType.Int).Value = intCodeShoppingList
            .Parameters.Add("@intCodeListe", SqlDbType.Int).Value = intCodeListe
            .Parameters.Add("@fltGrossQty", SqlDbType.Float).Value = dblGrossQty
            .Parameters.Add("@fltNetQty", SqlDbType.Float).Value = dblNetQty
            '.Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
            .Parameters.Add("@date", SqlDbType.DateTime).Value = dateNow
            .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
            cn.Open()
            .ExecuteNonQuery()
            cn.Close()
            cn.Dispose()


        End With
        ' Catch ex As Exception
        '  cn.Close()
        '  cn.Dispose()
        '  Return enumEgswErrorCode.GeneralError
        '  End Try
    End Function
    Public Function GetShoppingListID() As Integer
        Dim strCommandText As String = "MP_CountRowsShoppingList"
        Dim dt As DataTable
        Try
            Dim intReturn As Integer = 0
            dt = ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText).Tables(0)
            If Not dt Is Nothing Then
                If dt.Rows.Count > 0 Then
                    intReturn = CIntDB(dt.Rows(0)(0))
                Else
                    intReturn = 0
                End If
            Else
                intReturn = 0
            End If
            Return intReturn
        Catch ex As Exception
            Throw ex
        End Try

    End Function

    Public Function RemoveShoppingListDetailMP(ByVal intCodeShoppingList As Integer, ByVal intCodeUser As Integer) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = cn
                .CommandText = "MP_DeleteShoppingList"
                .CommandType = CommandType.StoredProcedure

                cn.Open()
                .Parameters.Add("@intCodeShoppingList", SqlDbType.Int).Value = intCodeShoppingList
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
                .ExecuteNonQuery()

                cn.Close()
                cn.Dispose()

                Return Nothing
            End With

        Catch ex As Exception
            cn.Close()
            cn.Dispose()
            Return enumEgswErrorCode.GeneralError
        End Try
    End Function

    Public Function RemoveMasterPlan(ByRef dt As DataTable, Optional ByVal intCodeRestaurant As Integer = 0, Optional ByVal intCodeMasterPlan As Integer = -1, Optional ByVal blnCheckStatus As Boolean = False, Optional ByVal intCodeMasterMenuPlan As Integer = 1) As enumEgswErrorCode
        Dim strCommandText As String = "[MP_DELETEMasterPlan]"
        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeRestaurant", intCodeRestaurant)
        arrParam(1) = New SqlParameter("@intCodeMasterPlan", intCodeMasterPlan)
        arrParam(2) = New SqlParameter("@bitCheckStatus", blnCheckStatus)
        arrParam(3) = New SqlParameter("@intCodeMasterMenuPlan", intCodeMasterMenuPlan)

        Try
            dt = ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam).Tables(0)
            Return enumEgswErrorCode.OK
        Catch ex As Exception
            Return enumEgswErrorCode.GeneralError
        End Try

    End Function

    Public Function RemoveRestaurantMasterPlanAssociation(ByVal intCodeMasterPlan As Integer) As enumEgswErrorCode
        Dim cmd As SqlCommand = New SqlCommand
        Try
            cmd.Connection = New SqlConnection(L_strCnn)
            cmd.Connection.Open()
            cmd.CommandText = "DELETE FROM EgswMPRestaurantMasterPlan WHERE CodeMasterPlan=@intCodeMasterPlan"
            cmd.Parameters.Add("@intCodeMasterPlan", SqlDbType.Int)
            cmd.Parameters("@intCodeMasterPlan").Value = intCodeMasterPlan
            cmd.ExecuteNonQuery()

        Catch ex As Exception
            Throw ex
        Finally
            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            cmd.Dispose()
        End Try
    End Function


    Public Function UpdateShoppingListDetailMP(ByRef intCode As Integer, ByVal strName As String, ByVal dtmDates As Date, ByVal strNote As String, ByVal intCodeUser As Integer,
                                               Optional ByVal useProduct As Boolean = False) As enumEgswErrorCode
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        ' Try
        With cmd
            .Connection = cn
            .CommandText = "MP_UpdateNewShoppingList"
            .CommandType = CommandType.StoredProcedure

            .Parameters.Add("@intCodeShoppingList", SqlDbType.Int).Value = intCode
            .Parameters.Add("@Name", SqlDbType.NVarChar, 260).Value = strName
            .Parameters.Add("@Note", SqlDbType.NVarChar, 1000).Value = strNote
            .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = intCodeUser
            .Parameters.Add("@Date", SqlDbType.DateTime).Value = dtmDates
            '.Parameters.Add("@UseProduct", SqlDbType.Int).Value = useProduct


            .Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

            cn.Open()
            .ExecuteNonQuery()
            cn.Close()
            cn.Dispose()



        End With
        'Catch ex As Exception
        '    cn.Close()
        '    cn.Dispose()
        '    Return enumEgswErrorCode.GeneralError
        'End Try
    End Function
    'Private Sub GetListIngComputedByYield(ByRef dt As DataTable, ByVal intCodeListe As Integer, ByVal intCodeIng As Integer, ByVal intFirstCodeSetPrice As Integer, ByVal L_udtUser As structUser, Optional ByVal dblOrigYieldFactor As Double = 1)
    '    Dim cListe As clsListe = New clsListe(enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataTable)

    '    Dim intYieldUnit As Integer = 0
    '    Dim dtListe As DataTable = CType(cListe.GetListeBasic(intCodeListe), DataTable)
    '    If dtListe.Rows.Count > 0 Then
    '        Dim rwListe As DataRow = dtListe.Rows(0)
    '        Select Case CType(rwListe("type"), enumDataListItemType)
    '            Case enumDataListItemType.Menu
    '                Dim dtIng As DataTable = CType(cListe.GetListeBasic(intCodeIng), DataTable)
    '                If dtIng.Rows.Count > 0 Then
    '                    Dim rwIng As DataRow = dtIng.Rows(0)
    '                    intYieldUnit = CInt(rwIng("yieldUnit"))
    '                End If
    '                dtIng.Dispose() 'DLS May312007
    '        End Select
    '    End If
    '    dtListe.Dispose()

    '    Dim dtListeIng As DataTable = CType(cListe.GetIngredientsShopping(CDbl(1), intCodeListe, L_udtUser.CodeTrans, L_udtUser.Site.Code, False, intFirstCodeSetPrice), DataTable)
    '    If dtListeIng.Select("itemCode=" & intCodeIng).Length > 0 Then
    '        Dim rwListeIng As DataRow = dtListeIng.Select("itemCode=" & intCodeIng)(0)
    '        Dim dblYieldFactor As Double = CDbl(rwListeIng("netquantity"))
    '        dblYieldFactor = dblYieldFactor * dblOrigYieldFactor

    '        If intYieldUnit > 0 AndAlso intYieldUnit = CInt(rwListeIng("itemunitcode")) Then
    '            'dblYieldFactor = 1 do nothing since same yield unit, meaning used as a recipe
    '        Else
    '            cListe.GetFactorSubRecipeIngredient(dblYieldFactor, CInt(rwListeIng("itemcode")), CInt(rwListeIng("itemunitcode")))
    '        End If

    '        Dim dtIng As DataTable = CType(cListe.GetIngredientsShopping(CDbl(1), CInt(rwListeIng("itemcode")), L_udtUser.CodeTrans, L_udtUser.Site.Code, L_udtUser.UseBestUnit, intFirstCodeSetPrice, dblYieldFactor), DataTable)
    '        Dim rwIng As DataRow

    '        Dim row As DataRow
    '        For Each rwIng In dtIng.Rows
    '            If CType(rwIng("itemtype"), enumDataListType) = enumDataListType.Merchandise Then
    '                row = dt.NewRow
    '                row("codeliste") = rwIng.Item("itemcode")
    '                row("name") = rwIng.Item("itemname")
    '                row("number") = rwIng.Item("itemnumber")
    '                row("secondcodesetprice") = rwIng.Item("secondcodesetprice")
    '                row("priceUnit") = rwIng.Item("priceUnit")
    '                row("itemUnitCode") = rwIng.Item("itemUnitCode")
    '                row("netQty") = rwIng.Item("netQuantity")
    '                row("itemUnitName") = rwIng.Item("itemUnit")
    '                row("itemFormat") = rwIng.Item("itemFormat")
    '                row("grossQty") = rwIng.Item("grossQuantity")
    '                row("itemCost") = rwIng.Item("itemCost")
    '                row("symbole") = rwIng.Item("symbole")
    '                row("priceFormat") = rwIng.Item("priceFormat")
    '                If CDbl(rwIng.Item("netQuantity")) = 0 Then row("itemUnitName") = ""
    '                row("itemPrice") = rwIng.Item("itemPrice")
    '                row("ItemPriceUnitCode") = rwIng.Item("ItemPriceUnitCode")
    '                dt.Rows.Add(row)
    '                If dt.Rows.Count > 2000 Then Exit Sub
    '            ElseIf CType(rwIng("itemtype"), enumDataListType) = enumDataListType.Recipe Then
    '                GetRecipeIng(dt, CInt(rwListeIng("itemcode")), CInt(rwIng.Item("itemcode")), intFirstCodeSetPrice, L_udtUser, dblOrigYieldFactor)
    '                'GetRecipeIng(dt, CInt(rwListeIng("itemcode")), CInt(rwIng.Item("itemcode")), intFirstCodeSetPrice, dblYieldFactor)
    '            End If
    '        Next
    '    End If
    'End Sub

    'MRC 08.24.09
    Private Sub GetListIngComputedByYield(ByRef dt As DataTable, ByVal intCodeListe As Integer, ByVal intCodeIng As Integer, ByVal intFirstCodeSetPrice As Integer, Optional ByVal dblNewYieldFactor As Double = 1, Optional ByVal isProduct As Boolean = False, Optional withProductLinking As Boolean = False)
        Dim cListe As clsListe = New clsListe(enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataTable)

        Dim intYieldUnit As Integer = 0, dblOriginalYieldFactor As Double = 0
        Dim dtListe As DataTable = CType(cListe.GetListeBasic(intCodeListe), DataTable)
        If dtListe.Rows.Count > 0 Then
            Dim rwListe As DataRow = dtListe.Rows(0)
            Select Case CType(rwListe("type"), enumDataListItemType)
                Case enumDataListItemType.Menu
                    Dim dtIng As DataTable = CType(cListe.GetListeBasic(intCodeIng), DataTable)
                    If dtIng.Rows.Count > 0 Then
                        Dim rwIng As DataRow = dtIng.Rows(0)
                        intYieldUnit = CInt(rwIng("yieldUnit"))
                    End If
                    dtIng.Dispose() 'DLS May312007

                Case enumDataListItemType.Recipe                    'MRC Used for resizing yields when calculate button is pressed on shopping list.
                    dblOriginalYieldFactor = CDblDB(rwListe("YIELD"))

            End Select
        End If
        dtListe.Dispose()

        Dim dtListeIng As DataTable = CType(cListe.GetIngredientsShopping(CDbl(1), intCodeListe, L_udtUser.CodeTrans, L_udtUser.Site.Code, False, intFirstCodeSetPrice, , isProduct, withProductLinking), DataTable)
        If dtListeIng.Select("itemCode=" & intCodeIng).Length > 0 Then
            Dim rwListeIng As DataRow = dtListeIng.Select("itemCode=" & intCodeIng)(0)
            Dim dblYieldFactor As Double = CDbl(rwListeIng("netquantity"))
            dblYieldFactor = (dblYieldFactor * dblNewYieldFactor) / dblOriginalYieldFactor

            If intYieldUnit > 0 AndAlso intYieldUnit = CInt(rwListeIng("itemunitcode")) Then
                'dblYieldFactor = 1 do nothing since same yield unit, meaning used as a recipe
            Else
                cListe.GetFactorSubRecipeIngredient(dblYieldFactor, CInt(rwListeIng("itemcode")), CInt(rwListeIng("itemunitcode")))
            End If

            Dim dtIng As DataTable = CType(cListe.GetIngredientsShopping(CDbl(1), CInt(rwListeIng("itemcode")), L_udtUser.CodeTrans, L_udtUser.Site.Code, L_udtUser.UseBestUnit, intFirstCodeSetPrice, dblYieldFactor, isProduct, withProductLinking), DataTable)
            Dim rwIng As DataRow

            Dim row As DataRow
            For Each rwIng In dtIng.Rows
                If CType(rwIng("itemtype"), enumDataListType) = enumDataListType.Merchandise Then
                    row = dt.NewRow
                    row("codeliste") = rwIng.Item("itemcode")
                    row("itemType") = rwIng.Item("itemType")
                    row("name") = rwIng.Item("itemname")
                    row("number") = rwIng.Item("itemnumber")
                    row("secondcodesetprice") = rwIng.Item("secondcodesetprice")
                    row("priceUnit") = rwIng.Item("priceUnit")
                    row("itemUnitCode") = rwIng.Item("itemUnitCode")
                    row("netQty") = rwIng.Item("netQuantity")
                    row("itemUnitName") = rwIng.Item("itemUnit")
                    row("itemFormat") = rwIng.Item("itemFormat")
                    row("grossQty") = rwIng.Item("grossQuantity")
                    row("itemCost") = rwIng.Item("itemCost")
                    row("symbole") = rwIng.Item("symbole")
                    row("priceFormat") = rwIng.Item("priceFormat")
                    If CDbl(rwIng.Item("netQuantity")) = 0 Then row("itemUnitName") = ""
                    row("itemPrice") = rwIng.Item("itemPrice")
                    row("ItemPriceUnitCode") = rwIng.Item("ItemPriceUnitCode")
                    dt.Rows.Add(row)
                    If dt.Rows.Count > 2000 Then Exit Sub
                ElseIf CType(rwIng("itemtype"), enumDataListType) = enumDataListType.Recipe Then
                    'GetRecipeIng(dt, CInt(rwListeIng("itemcode")), CInt(rwIng.Item("itemcode")), intFirstCodeSetPrice, dblOrigYieldFactor)
                    GetRecipeIng(dt, CInt(rwListeIng("itemcode")), CInt(rwIng.Item("itemcode")), intFirstCodeSetPrice, dblYieldFactor)
                End If
            Next
        End If
    End Sub

    Private Sub GetRecipeIng(ByRef dt As DataTable, ByVal intCodeListe As Integer, ByVal intCodeIng As Integer, ByVal intFirstCodeSetPrice As Integer, Optional ByVal dblOrigYieldFactor As Double = 1)
        Dim cListe As clsListe = New clsListe(enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataTable)

        Dim intYieldUnit As Integer = 0
        Dim dtListe As DataTable = CType(cListe.GetListeBasic(intCodeListe), DataTable)
        If dtListe.Rows.Count > 0 Then
            Dim rwListe As DataRow = dtListe.Rows(0)
            Select Case CType(rwListe("type"), enumDataListItemType)
                Case enumDataListItemType.Menu
                    Dim dtIng As DataTable = CType(cListe.GetListeBasic(intCodeIng), DataTable)
                    If dtIng.Rows.Count > 0 Then
                        Dim rwIng As DataRow = dtIng.Rows(0)
                        intYieldUnit = CInt(rwIng("yieldUnit"))
                    End If
                    dtIng.Dispose() 'DLS May312007
            End Select
        End If
        dtListe.Dispose()

        Dim dtListeIng As DataTable = CType(cListe.GetIngredientsShopping(CDbl(1), intCodeListe, L_udtUser.CodeTrans, L_udtUser.Site.Code, False, intFirstCodeSetPrice), DataTable)
        If dtListeIng.Select("itemCode=" & intCodeIng).Length > 0 Then
            Dim rwListeIng As DataRow = dtListeIng.Select("itemCode=" & intCodeIng)(0)
            Dim dblYieldFactor As Double = CDbl(rwListeIng("netquantity"))
            dblYieldFactor = dblYieldFactor * dblOrigYieldFactor

            If intYieldUnit > 0 AndAlso intYieldUnit = CInt(rwListeIng("itemunitcode")) Then
                'dblYieldFactor = 1 do nothing since same yield unit, meaning used as a recipe
            Else
                cListe.GetFactorSubRecipeIngredient(dblYieldFactor, CInt(rwListeIng("itemcode")), CInt(rwListeIng("itemunitcode")))
            End If

            Dim dtIng As DataTable = CType(cListe.GetIngredientsShopping(CDbl(1), CInt(rwListeIng("itemcode")), L_udtUser.CodeTrans, L_udtUser.Site.Code, L_udtUser.UseBestUnit, intFirstCodeSetPrice, dblYieldFactor), DataTable)
            Dim rwIng As DataRow

            Dim row As DataRow
            For Each rwIng In dtIng.Rows
                If CType(rwIng("itemtype"), enumDataListType) = enumDataListType.Merchandise Then
                    row = dt.NewRow
                    row("codeliste") = rwIng.Item("itemcode")
                    row("itemtype") = rwIng.Item("itemtype")
                    row("name") = rwIng.Item("itemname")
                    row("number") = rwIng.Item("itemnumber")
                    row("secondcodesetprice") = rwIng.Item("secondcodesetprice")
                    row("priceUnit") = rwIng.Item("priceUnit")
                    row("itemUnitCode") = rwIng.Item("itemUnitCode")
                    row("netQty") = rwIng.Item("netQuantity")
                    row("itemUnitName") = rwIng.Item("itemUnit")
                    row("itemFormat") = rwIng.Item("itemFormat")
                    row("grossQty") = rwIng.Item("grossQuantity")
                    row("itemCost") = rwIng.Item("itemCost")
                    row("symbole") = rwIng.Item("symbole")
                    row("priceFormat") = rwIng.Item("priceFormat")
                    If CDbl(rwIng.Item("netQuantity")) = 0 Then row("itemUnitName") = ""
                    row("itemPrice") = rwIng.Item("itemPrice")
                    row("ItemPriceUnitCode") = rwIng.Item("ItemPriceUnitCode")
                    dt.Rows.Add(row)
                    If dt.Rows.Count > 2000 Then Exit Sub
                ElseIf CType(rwIng("itemtype"), enumDataListType) = enumDataListType.Recipe Then
                    GetRecipeIng(dt, CInt(rwListeIng("itemcode")), CInt(rwIng.Item("itemcode")), intFirstCodeSetPrice, dblYieldFactor)
                End If
            Next
        End If
    End Sub

    ''Private Sub GetRecipeIng(ByRef dt As DataTable, ByVal intCodeListe As Integer, ByVal intCodeIng As Integer, ByVal intFirstCodeSetPrice As Integer, ByVal L_udtUser As structUser, Optional ByVal dblOrigYieldFactor As Double = 1)
    ''    Dim cListe As clsListe = New clsListe(enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataTable)

    ''    Dim intYieldUnit As Integer = 0
    ''    Dim dtListe As DataTable = CType(cListe.GetListeBasic(intCodeListe), DataTable)
    ''    If dtListe.Rows.Count > 0 Then
    ''        Dim rwListe As DataRow = dtListe.Rows(0)
    ''        Select Case CType(rwListe("type"), enumDataListItemType)
    ''            Case enumDataListItemType.Menu
    ''                Dim dtIng As DataTable = CType(cListe.GetListeBasic(intCodeIng), DataTable)
    ''                If dtIng.Rows.Count > 0 Then
    ''                    Dim rwIng As DataRow = dtIng.Rows(0)
    ''                    intYieldUnit = CInt(rwIng("yieldUnit"))
    ''                End If
    ''                dtIng.Dispose() 'DLS May312007
    ''        End Select
    ''    End If
    ''    dtListe.Dispose()

    ''    Dim dtListeIng As DataTable = CType(cListe.GetIngredientsShopping(CDbl(1), intCodeListe, L_udtUser.CodeTrans, L_udtUser.Site.Code, False, intFirstCodeSetPrice), DataTable)
    ''    If dtListeIng.Select("itemCode=" & intCodeIng).Length > 0 Then
    ''        Dim rwListeIng As DataRow = dtListeIng.Select("itemCode=" & intCodeIng)(0)
    ''        Dim dblYieldFactor As Double = CDbl(rwListeIng("netquantity"))
    ''        dblYieldFactor = dblYieldFactor * dblOrigYieldFactor

    ''        If intYieldUnit > 0 AndAlso intYieldUnit = CInt(rwListeIng("itemunitcode")) Then
    ''            'dblYieldFactor = 1 do nothing since same yield unit, meaning used as a recipe
    ''        Else
    ''            cListe.GetFactorSubRecipeIngredient(dblYieldFactor, CInt(rwListeIng("itemcode")), CInt(rwListeIng("itemunitcode")))
    ''        End If

    ''        Dim dtIng As DataTable = CType(cListe.GetIngredientsShopping(CDbl(1), CInt(rwListeIng("itemcode")), L_udtUser.CodeTrans, L_udtUser.Site.Code, L_udtUser.UseBestUnit, intFirstCodeSetPrice, dblYieldFactor), DataTable)
    ''        Dim rwIng As DataRow

    ''        Dim row As DataRow
    ''        For Each rwIng In dtIng.Rows
    ''            If CType(rwIng("itemtype"), enumDataListType) = enumDataListType.Merchandise Then
    ''                row = dt.NewRow
    ''                row("codeliste") = rwIng.Item("itemcode")
    ''                row("name") = rwIng.Item("itemname")
    ''                row("number") = rwIng.Item("itemnumber")
    ''                row("secondcodesetprice") = rwIng.Item("secondcodesetprice")
    ''                row("priceUnit") = rwIng.Item("priceUnit")
    ''                row("itemUnitCode") = rwIng.Item("itemUnitCode")
    ''                row("netQty") = rwIng.Item("netQuantity")
    ''                row("itemUnitName") = rwIng.Item("itemUnit")
    ''                row("itemFormat") = rwIng.Item("itemFormat")
    ''                row("grossQty") = rwIng.Item("grossQuantity")
    ''                row("itemCost") = rwIng.Item("itemCost")
    ''                row("symbole") = rwIng.Item("symbole")
    ''                row("priceFormat") = rwIng.Item("priceFormat")
    ''                If CDbl(rwIng.Item("netQuantity")) = 0 Then row("itemUnitName") = ""
    ''                row("itemPrice") = rwIng.Item("itemPrice")
    ''                row("ItemPriceUnitCode") = rwIng.Item("ItemPriceUnitCode")
    ''                dt.Rows.Add(row)
    ''                If dt.Rows.Count > 2000 Then Exit Sub
    ''            ElseIf CType(rwIng("itemtype"), enumDataListType) = enumDataListType.Recipe Then
    ''                GetRecipeIng(dt, CInt(rwListeIng("itemcode")), CInt(rwIng.Item("itemcode")), intFirstCodeSetPrice, L_udtUser, dblYieldFactor)
    ''            End If
    ''        Next
    ''    End If
    ''End Sub

    Public Function GetShoppingListExisting(ByVal intCodeShoppingList As Integer) As DataTable
        Dim arrParam(0) As SqlParameter
        Dim strCommandText As String = "MP_GetShoppingListDetails"
        arrParam(0) = New SqlParameter("@intCodeShoppingList", intCodeShoppingList)
        'arrParam(1) = New SqlParameter("@intCodeUser", intCodeRestaurant)
        Dim dt As DataTable
        'Try
        dt = ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText, arrParam).Tables(0)
        Return dt
        ' Catch ex As Exception
        'Throw ex
        ' End Try

    End Function

    Public Function fctGetUnitTypeMain(ByVal intCode As Integer) As DataTable
        Dim arrParam(0) As SqlParameter
        Dim strCommandText As String = "MP_GetUnitTypeMain"
        arrParam(0) = New SqlParameter("@intCode", intCode)

        Dim dt As DataTable
        'Try
        dt = ExecuteDataset(L_strCnn, CommandType.StoredProcedure, strCommandText).Tables(0)
        Return dt
        ' Catch ex As Exception
        'Throw ex
        ' End Try

    End Function


    'MRC 08.24.09
    Public Function MPGetListComputedByYield(ByVal dtCodeliste As DataTable, ByVal intFirstCodeSetPrice As Integer, Optional ByVal dblYield As Double = -1,
                                             Optional ByVal blnGroup As Boolean = False,
                                             Optional ByVal useProduct As Boolean = False,
                                             Optional ByVal withProductLinking As Boolean = False) As DataTable
        '// Create Table to Store Ingredients 
        Dim dt As New DataTable("Ing")
        With dt.Columns
            .Add("codeliste")
            .Add("name")
            .Add("number")
            .Add("netQty")
            .Add("grossQty")
            .Add("itemUnitName")
            .Add("symbole")
            .Add("itemPrice")
            .Add("itemCost")
            .Add("itemFormat")
            .Add("priceFormat")
            .Add("itemUnitCode")
            .Add("secondcodesetprice")
            .Add("priceUnit")
            .Add("ItemPriceUnitCode")
            .Add("itemType")
        End With

        '// Get Ingredients of each recipe in array
        Dim cListe As New clsListe(enumAppType.WebApp, L_strCnn)
        Dim dr As SqlDataReader
        Dim row As DataRow
        Dim bUseBestUnit As Boolean = False
        'If Not blnGroup Then bUseBestUnit = L_udtUser.UseBestUnit

        '' The dtCodeListe here actually contains the IDDetail of the Menuplan.
        '' Each IDDetail can have 1 or more CodeListe in it.
        Dim dtCodeliste2 As DataTable = dtCodeliste.Copy
        dtCodeliste2.Clear()
        For i As Integer = 0 To dtCodeliste.Rows.Count - 1
            Dim dtTemp As DataTable = CType(GetShoppingList(CInt(dtCodeliste.Rows(i)("codeliste")), L_udtUser.CodeTrans, 1, bUseBestUnit, intFirstCodeSetPrice, L_udtUser.Site.Code, CDbl(dtCodeliste.Rows(i)("computedyield")),
                                                            enumEgswFetchType.DataTable, useProduct, withProductLinking), DataTable)
            If Not dtTemp Is Nothing Then
                If dtTemp.Rows.Count > 0 Then
                    For Each rTemp As DataRow In dtTemp.Rows
                        Dim r As DataRow = dtCodeliste2.NewRow
                        r("codeliste") = rTemp("ItemCode")
                        r("name") = rTemp("ItemName")
                        r("number") = rTemp("ItemNumber")
                        r("yield") = rTemp("NetQuantity")
                        r("yieldunit") = rTemp("ItemUnitCode")
                        r("portionunit") = rTemp("ItemUnit")
                        r("percentage") = 100
                        r("computedyield") = rTemp("NetQuantity")
                        r("coderestaurant") = CInt(dtCodeliste.Rows(i)("coderestaurant"))
                        r("itemtype") = rTemp("itemtype")   'CInt(dtCodeliste.Rows(i)("itemtype"))
                        dtCodeliste2.Rows.Add(r)

                        'Directly add merchandise items to shopping list
                        'If useProduct Then
                        '    Dim dtProd = GetLinkedProduct(rTemp("ItemCode"), intFirstCodeSetPrice)
                        '    For Each prod In dtProd.Rows
                        '        row = dt.NewRow
                        '        row("name") = prod("ProductName")
                        '        row("itemPrice") = prod("Price")

                        '        row("codeliste") = rTemp("ItemCode")
                        '        row("itemType") = rTemp("ItemType")
                        '        row("number") = ""
                        '        row("secondcodesetprice") = rTemp("secondcodesetprice")
                        '        row("priceUnit") = rTemp("priceUnit")
                        '        row("itemUnitCode") = rTemp("ItemUnitCode")
                        '        row("netQty") = rTemp("netQuantity")
                        '        row("itemUnitName") = rTemp("itemUnit")
                        '        row("itemFormat") = rTemp("itemFormat")
                        '        row("grossQty") = rTemp("grossQuantity")
                        '        row("itemCost") = rTemp("itemCost")
                        '        row("symbole") = rTemp("symbole")
                        '        row("priceFormat") = rTemp("priceFormat")
                        '        row("ItemPriceUnitCode") = rTemp("ItemPriceUnitCode")
                        '        dt.Rows.Add(row)
                        '    Next

                        'Else
                        If rTemp("itemtype") = 2 Then
                            row = dt.NewRow
                            row("codeliste") = rTemp("ItemCode")
                            row("itemType") = rTemp("ItemType")
                            row("name") = rTemp("ItemName")
                            row("number") = rTemp("ItemNumber")
                            row("secondcodesetprice") = rTemp("secondcodesetprice")
                            row("priceUnit") = rTemp("priceUnit")
                            row("itemUnitCode") = rTemp("ItemUnitCode")
                            row("netQty") = rTemp("netQuantity")
                            row("itemUnitName") = rTemp("itemUnit")
                            row("itemFormat") = rTemp("itemFormat")
                            row("grossQty") = rTemp("grossQuantity")
                            row("itemCost") = rTemp("itemCost")
                            row("symbole") = rTemp("symbole")
                            row("priceFormat") = rTemp("priceFormat")
                            row("itemPrice") = rTemp("itemPrice")
                            row("ItemPriceUnitCode") = rTemp("ItemPriceUnitCode")
                            dt.Rows.Add(row)
                        End If
                        'End If
                    Next
                End If
            End If
        Next

        dtCodeliste = dtCodeliste2

        For i As Integer = 0 To dtCodeliste.Rows.Count - 1
            'to do: check if Product
            dr = CType(cListe.GetIngredientsShopping(1, CInt(dtCodeliste.Rows(i)("codeliste")), L_udtUser.CodeTrans, L_udtUser.Site.Code, bUseBestUnit, intFirstCodeSetPrice, CDbl(dtCodeliste.Rows(i)("computedyield")), useProduct, withProductLinking), SqlDataReader)
            'dr = CType(GetShoppingList(CInt(dtCodeliste.Rows(i)("codeliste")), L_udtUser.CodeTrans, 1, bUseBestUnit, intFirstCodeSetPrice, L_udtUser.Site.Code, CDbl(dtCodeliste.Rows(i)("computedyield")), enumEgswFetchType.DataReader), SqlDataReader)
            While dr.Read
                ' only add Ingredients
                If CType(dr.Item("itemType"), enumDataListItemType) = enumDataListItemType.Merchandise Then
                    row = dt.NewRow
                    row("codeliste") = dr.Item("itemcode")
                    row("itemType") = dr.Item("itemType")
                    row("name") = dr.Item("itemname")
                    row("number") = dr.Item("itemnumber")
                    row("secondcodesetprice") = dr.Item("secondcodesetprice")
                    row("priceUnit") = dr.Item("priceUnit")
                    row("itemUnitCode") = dr.Item("itemUnitCode")
                    row("netQty") = dr.Item("netQuantity")
                    row("itemUnitName") = dr.Item("itemUnit")
                    row("itemFormat") = dr.Item("itemFormat")
                    row("grossQty") = dr.Item("grossQuantity")
                    row("itemCost") = dr.Item("itemCost")
                    row("symbole") = dr.Item("symbole")
                    row("priceFormat") = dr.Item("priceFormat")
                    If CDbl(dr.Item("netQuantity")) = 0 Then row("itemUnitName") = ""
                    row("itemPrice") = dr.Item("itemPrice")
                    row("ItemPriceUnitCode") = dr.Item("ItemPriceUnitCode")
                    dt.Rows.Add(row)
                ElseIf CType(dr.Item("itemType"), enumDataListItemType) = enumDataListItemType.Recipe Then
                    GetListIngComputedByYield(dt, CInt(dtCodeliste.Rows(i)("codeliste")), CInt(dr.Item("itemcode")), intFirstCodeSetPrice, CDbl(dtCodeliste.Rows(i)("computedyield")), useProduct, withProductLinking)
                    ''MPGetListIngComputedByYield(dt, CInt(dtCodeliste.Rows(i)("codeliste")), CInt(dr.Item("itemcode")), intFirstCodeSetPrice, CDbl(dtCodeliste.Rows(i)("computedyield")))
                End If
            End While
            dr.Close()
        Next

        Dim cUnit As clsUnit = New clsUnit(L_udtUser, L_AppType, L_strCnn)
        Dim rwUnitPrice, rwUnitIng As DataRow
        Dim dtListeSetPrice As DataTable
        Dim rwListeSetPrice1, rwListeSetPrice As DataRow
        Dim rw As DataRow
        If Not useProduct Then
            For Each rw In dt.Rows
                'convert accdg to price unit
                If IsDBNull(rw("ItemPriceUnitCode")) Then
                    rwUnitPrice = cUnit.GetOne(CInt(rw("itemUnitCode")))
                    rwUnitIng = cUnit.GetOne(CInt(rw("itemUnitCode")))

                    'rw("itemUnitName") = rw("itemUnit")
                    'rw("itemUnitCode") = CInt(rw("itemUnitCode"))
                    'rw("itemFormat") = rwUnitPrice("format")
                    rw("grossQty") = 0 'CDblDB(rw("grossQty")) '/ CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIng("factor"))
                    rw("netQty") = 0 'CDblDB(rw("netQty")) '/ CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIng("factor"))
                Else
                    If CInt(rw("ItemPriceUnitCode")) <> CInt(rw("itemUnitCode")) Then
                        rwUnitPrice = cUnit.GetOne(CInt(rw("ItemPriceUnitCode")))
                        rwUnitIng = cUnit.GetOne(CInt(rw("itemUnitCode")))

                        rw("itemUnitName") = rw("priceUnit")
                        rw("itemUnitCode") = rw("ItemPriceUnitCode")
                        rw("itemFormat") = rwUnitPrice("format")
                        rw("grossQty") = CDblDB(rw("grossQty")) / CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIng("factor"))
                        rw("netQty") = CDblDB(rw("netQty")) / CDblDB(rwUnitPrice("factor")) * CDblDB(rwUnitIng("factor"))
                    End If

                    'convert to main unit
                    dtListeSetPrice = CType(cListe.GetListeSetPrice(CInt(rw("codeListe")), intFirstCodeSetPrice, L_udtUser.CodeTrans, enumEgswFetchType.DataTable), DataTable)
                    If dtListeSetPrice.Select("unit=" & CStr(rw("ItemPriceUnitCode"))).Length > 0 Then
                        rwListeSetPrice = dtListeSetPrice.Select("unit=" & CStr(rw("ItemPriceUnitCode")))(0)
                        If CInt(rwListeSetPrice("position")) <> 1 Then
                            rwListeSetPrice1 = dtListeSetPrice.Select("position=1")(0)

                            rw("priceUnit") = CStr(rwListeSetPrice1("name")).Replace("/", "")
                            rw("itemUnitCode") = rwListeSetPrice1("unit")
                            rw("netQty") = CDblDB(rw("netQty")) * CDblDB(rwListeSetPrice("ratio"))
                            rw("grossQty") = CDblDB(rw("grossQty")) * CDblDB(rwListeSetPrice("ratio"))
                            rw("itemUnitName") = rwListeSetPrice1("name")
                            rw("itemFormat") = rwListeSetPrice1("format")
                            rw("itemCost") = CDblDB(rw("grossQty")) * CDblDB(rwListeSetPrice1("Price"))
                            If CDbl(rw("netQty")) = 0 Then rw("itemUnitName") = ""
                            rw("itemPrice") = rwListeSetPrice1("Price")
                            rw("ItemPriceUnitCode") = rwListeSetPrice1("unit")
                        End If
                    End If
                    dtListeSetPrice.Dispose() 'DLS May312007
                End If


            Next
        End If

        'If blnGroup = False Then Return dt
        Dim dtMerged As New DataTable("IngMerged")
        With dtMerged.Columns
            .Add("codeliste", System.Type.GetType("System.Int32"))
            .Add("name")
            .Add("number")
            .Add("netQty")
            .Add("grossQty", System.Type.GetType("System.Double"))
            .Add("itemUnitName")
            .Add("symbole")
            .Add("itemPrice")
            .Add("itemCost")
            .Add("itemFormat")
            .Add("priceFormat")
            .Add("itemUnitCode", System.Type.GetType("System.Int32"))
            .Add("secondcodesetprice")
            .Add("priceUnit")
            .Add("itemType")
        End With

        Dim strRowFilter As String
        For Each row In dt.Rows
            strRowFilter = "codeListe=" & CIntDB(row("codeListe")) & _
             " AND itemUnitcode=" & CIntDB(row("itemUnitcode")) ' & _
            '" AND priceUnit='" & CInt(row("priceUnit")) & "'"
            If dtMerged.Select(strRowFilter).Length > 0 Then
                rw = dtMerged.Select(strRowFilter)(0)
                rw("netQty") = CDblDB(row("netQty")) + CDblDB(rw("netQty"))
                rw("grossQty") = CDblDB(row("grossQty")) + CDblDB(rw("grossQty"))
                'rw("itemPrice") = CDblDB(row("itemPrice")) + CDbl(rw("itemPrice"))
                rw("itemCost") = CDblDB(row("itemCost")) + CDblDB(rw("itemCost"))
            Else
                rw = dtMerged.NewRow
                rw("codeliste") = row("codeliste")
                rw("name") = row("name")
                rw("number") = row("number")
                rw("netQty") = row("netQty")
                rw("grossQty") = row("grossQty")
                rw("itemUnitName") = row("itemUnitName")
                rw("symbole") = row("symbole")
                rw("itemPrice") = row("itemPrice")
                rw("itemCost") = row("itemCost")
                rw("itemFormat") = row("itemFormat")
                rw("priceFormat") = row("priceFormat")
                rw("itemUnitCode") = row("itemUnitCode")
                rw("secondcodesetprice") = row("secondcodesetprice")
                rw("priceUnit") = row("priceUnit")
                rw("itemType") = row("itemType")
                dtMerged.Rows.Add(rw)
            End If
        Next

        Dim rwZero() As DataRow = dtMerged.Select("grossQty=0")
        For i As Integer = 0 To rwZero.Length - 1
            dtMerged.Rows.Remove(rwZero(i))
        Next

        Return dtMerged
    End Function

    'MRC 08.24.09
    Private Sub MPGetListIngComputedByYield(ByRef dt As DataTable, ByVal intCodeListe As Integer, ByVal intCodeIng As Integer, ByVal intFirstCodeSetPrice As Integer, Optional ByVal dblNewYieldFactor As Double = 1)
        Dim cListe As clsListe = New clsListe(enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataTable)

        Dim intYieldUnit As Integer = 0, dblOriginalYieldFactor As Double = 0
        Dim dtListe As DataTable = CType(cListe.GetListeBasic(intCodeListe), DataTable)
        If dtListe.Rows.Count > 0 Then
            Dim rwListe As DataRow = dtListe.Rows(0)
            Select Case CType(rwListe("type"), enumDataListItemType)
                Case enumDataListItemType.Menu
                    Dim dtIng As DataTable = CType(cListe.GetListeBasic(intCodeIng), DataTable)
                    If dtIng.Rows.Count > 0 Then
                        Dim rwIng As DataRow = dtIng.Rows(0)
                        intYieldUnit = CInt(rwIng("yieldUnit"))
                    End If
                    dtIng.Dispose() 'DLS May312007

                Case enumDataListItemType.Recipe                    'MRC Used for resizing yields when calculate button is pressed on shopping list.
                    dblOriginalYieldFactor = CDblDB(rwListe("YIELD"))

            End Select
        End If
        dtListe.Dispose()


        Dim dtListeIng As DataTable = CType(GetShoppingList(intCodeListe, L_udtUser.CodeTrans, 1, False, intFirstCodeSetPrice, L_udtUser.Site.Code, -1, enumEgswFetchType.DataTable), DataTable)
        If dtListeIng.Select("itemCode=" & intCodeIng).Length > 0 Then
            Dim rwListeIng As DataRow = dtListeIng.Select("itemCode=" & intCodeIng)(0)
            Dim dblYieldFactor As Double = CDbl(rwListeIng("netquantity"))
            dblYieldFactor = (dblYieldFactor * dblNewYieldFactor) / dblOriginalYieldFactor

            If intYieldUnit > 0 AndAlso intYieldUnit = CInt(rwListeIng("itemunitcode")) Then
                'dblYieldFactor = 1 do nothing since same yield unit, meaning used as a recipe
            Else
                cListe.GetFactorSubRecipeIngredient(dblYieldFactor, CInt(rwListeIng("itemcode")), CInt(rwListeIng("itemunitcode")))
            End If

            Dim dtIng As DataTable = CType(cListe.GetIngredientsShopping(CDbl(1), CInt(rwListeIng("itemcode")), L_udtUser.CodeTrans, L_udtUser.Site.Code, L_udtUser.UseBestUnit, intFirstCodeSetPrice, dblYieldFactor), DataTable)
            Dim rwIng As DataRow

            Dim row As DataRow
            For Each rwIng In dtIng.Rows
                If CType(rwIng("itemtype"), enumDataListType) = enumDataListType.Merchandise Then
                    row = dt.NewRow
                    row("codeliste") = rwIng.Item("itemcode")
                    row("name") = rwIng.Item("itemname")
                    row("number") = rwIng.Item("itemnumber")
                    row("secondcodesetprice") = rwIng.Item("secondcodesetprice")
                    row("priceUnit") = rwIng.Item("priceUnit")
                    row("itemUnitCode") = rwIng.Item("itemUnitCode")
                    row("netQty") = rwIng.Item("netQuantity")
                    row("itemUnitName") = rwIng.Item("itemUnit")
                    row("itemFormat") = rwIng.Item("itemFormat")
                    row("grossQty") = rwIng.Item("grossQuantity")
                    row("itemCost") = rwIng.Item("itemCost")
                    row("symbole") = rwIng.Item("symbole")
                    row("priceFormat") = rwIng.Item("priceFormat")
                    row("itemType") = rwIng.Item("itemType")
                    If CDbl(rwIng.Item("netQuantity")) = 0 Then row("itemUnitName") = ""
                    row("itemPrice") = rwIng.Item("itemPrice")
                    row("ItemPriceUnitCode") = rwIng.Item("ItemPriceUnitCode")
                    dt.Rows.Add(row)
                    If dt.Rows.Count > 2000 Then Exit Sub
                ElseIf CType(rwIng("itemtype"), enumDataListType) = enumDataListType.Recipe Then
                    MPGetRecipeIng(dt, CInt(rwListeIng("itemcode")), CInt(rwIng.Item("itemcode")), intFirstCodeSetPrice, dblYieldFactor)
                End If
            Next
        End If
    End Sub

    Private Sub MPGetRecipeIng(ByRef dt As DataTable, ByVal intCodeListe As Integer, ByVal intCodeIng As Integer, ByVal intFirstCodeSetPrice As Integer, Optional ByVal dblOrigYieldFactor As Double = 1)
        Dim cListe As clsListe = New clsListe(enumAppType.WebApp, L_strCnn, enumEgswFetchType.DataTable)

        Dim intYieldUnit As Integer = 0
        Dim dtListe As DataTable = CType(cListe.GetListeBasic(intCodeListe), DataTable)
        If dtListe.Rows.Count > 0 Then
            Dim rwListe As DataRow = dtListe.Rows(0)
            Select Case CType(rwListe("type"), enumDataListItemType)
                Case enumDataListItemType.Menu
                    Dim dtIng As DataTable = CType(cListe.GetListeBasic(intCodeIng), DataTable)
                    If dtIng.Rows.Count > 0 Then
                        Dim rwIng As DataRow = dtIng.Rows(0)
                        intYieldUnit = CInt(rwIng("yieldUnit"))
                    End If
                    dtIng.Dispose() 'DLS May312007
            End Select
        End If
        dtListe.Dispose()

        Dim dtListeIng As DataTable = CType(cListe.GetIngredientsShopping(CDbl(1), intCodeListe, L_udtUser.CodeTrans, L_udtUser.Site.Code, False, intFirstCodeSetPrice), DataTable)
        If dtListeIng.Select("itemCode=" & intCodeIng).Length > 0 Then
            Dim rwListeIng As DataRow = dtListeIng.Select("itemCode=" & intCodeIng)(0)
            Dim dblYieldFactor As Double = CDbl(rwListeIng("netquantity"))
            dblYieldFactor = dblYieldFactor * dblOrigYieldFactor

            If intYieldUnit > 0 AndAlso intYieldUnit = CInt(rwListeIng("itemunitcode")) Then
                'dblYieldFactor = 1 do nothing since same yield unit, meaning used as a recipe
            Else
                cListe.GetFactorSubRecipeIngredient(dblYieldFactor, CInt(rwListeIng("itemcode")), CInt(rwListeIng("itemunitcode")))
            End If

            Dim dtIng As DataTable = CType(cListe.GetIngredientsShopping(CDbl(1), CInt(rwListeIng("itemcode")), L_udtUser.CodeTrans, L_udtUser.Site.Code, L_udtUser.UseBestUnit, intFirstCodeSetPrice, dblYieldFactor), DataTable)
            Dim rwIng As DataRow

            Dim row As DataRow
            For Each rwIng In dtIng.Rows
                If CType(rwIng("itemtype"), enumDataListType) = enumDataListType.Merchandise Then
                    row = dt.NewRow
                    row("codeliste") = rwIng.Item("itemcode")
                    row("name") = rwIng.Item("itemname")
                    row("number") = rwIng.Item("itemnumber")
                    row("itemType") = rwIng.Item("itemType")
                    row("secondcodesetprice") = rwIng.Item("secondcodesetprice")
                    row("priceUnit") = rwIng.Item("priceUnit")
                    row("itemUnitCode") = rwIng.Item("itemUnitCode")
                    row("netQty") = rwIng.Item("netQuantity")
                    row("itemUnitName") = rwIng.Item("itemUnit")
                    row("itemFormat") = rwIng.Item("itemFormat")
                    row("grossQty") = rwIng.Item("grossQuantity")
                    row("itemCost") = rwIng.Item("itemCost")
                    row("symbole") = rwIng.Item("symbole")
                    row("priceFormat") = rwIng.Item("priceFormat")
                    If CDbl(rwIng.Item("netQuantity")) = 0 Then row("itemUnitName") = ""
                    row("itemPrice") = rwIng.Item("itemPrice")
                    row("ItemPriceUnitCode") = rwIng.Item("ItemPriceUnitCode")
                    dt.Rows.Add(row)
                    If dt.Rows.Count > 2000 Then Exit Sub
                ElseIf CType(rwIng("itemtype"), enumDataListType) = enumDataListType.Recipe Then
                    MPGetRecipeIng(dt, CInt(rwListeIng("itemcode")), CInt(rwIng.Item("itemcode")), intFirstCodeSetPrice, dblYieldFactor)
                End If
            Next
        End If
    End Sub

    Public Function GetShoppingList(ByVal intCode As Integer, ByVal intCodeTrans As Integer, ByVal dblCurRate As Double, ByVal blnConvertBestUnit As Boolean, ByVal intCodeSetPrice As Integer,
                                    ByVal intCodeSite As Integer, Optional ByVal dblYield As Double = -1,
                                    Optional ByVal fetchType As enumEgswFetchType = enumEgswFetchType.DataTable,
                                    Optional ByVal isProduct As Boolean = False, Optional withProductLinking As Boolean = False) As Object
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                If withProductLinking Then
                    .CommandText = "[MP_GETShoppingList_MSC]"
                Else
                    .CommandText = "[MP_GETShoppingList]"
                End If
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCode", SqlDbType.Int).Value = intCode
                .Parameters.Add("@intCodeTrans", SqlDbType.Int).Value = intCodeTrans
                .Parameters.Add("@fltCurRate", SqlDbType.Float).Value = dblCurRate
                .Parameters.Add("@bitConvertBestUnit", SqlDbType.Bit).Value = blnConvertBestUnit
                .Parameters.Add("@intCodeSetprice", SqlDbType.Int).Value = intCodeSetPrice
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite
                .Parameters.Add("@useProduct", SqlDbType.Bit).Value = isProduct
                If dblYield >= 0 Then .Parameters.Add("@fltYield", SqlDbType.Float).Value = dblYield
            End With

            Return ExecuteFetchType(fetchType, cmd)
        Catch ex As Exception
            cmd.Dispose()
            GetShoppingList = Nothing
        End Try

    End Function

    Public Function GetLinkedProduct(ByVal codeListe As Integer, ByVal codeSetPrice As Integer) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_GETLinkedProduct]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = codeListe
                .Parameters.Add("@CodeSetprice", SqlDbType.Int).Value = codeSetPrice
            End With

            Return ExecuteFetchType(enumEgswFetchType.DataTable, cmd)
        Catch ex As Exception
            cmd.Dispose()
        End Try
    End Function

    Public Function GetMasterPlanSavedShoppingLists(ByVal intCodeUser As Integer) As DataTable
        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeUser", intCodeUser)

        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "MP_GetSavedShoppingLists", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetMasterPlanShoppingList(Optional ByVal intCodeTrans As Integer = -1, Optional ByVal intCodeSetPrice As Integer = -1, Optional ByVal intCodeListe As Integer = -1, Optional ByVal intCodeSite As Integer = -1, Optional ByVal bitCovnertBestUnit As Integer = -1, Optional ByVal ftlCurRate As Integer = -1) As DataTable
        Dim arrParam(5) As SqlParameter
        arrParam(0) = New SqlParameter("@intCode", intCodeListe)
        arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans)
        arrParam(2) = New SqlParameter("@fltCurRate", ftlCurRate)
        arrParam(3) = New SqlParameter("@bitConvertBestUnit", bitCovnertBestUnit)
        arrParam(4) = New SqlParameter("@intCodeSetPrice", intCodeSetPrice)
        arrParam(5) = New SqlParameter("@intCodeSite", intCodeSite)


        Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "MP_GETShoppingList", arrParam).Tables(0)

    End Function
    Public Function GetPrintMasterPlanShoppingList(ByVal intCodeShoppingList As Integer, ByVal intCodeUser As Integer, ByVal intCodeTrans As Integer, Optional ByVal useProduct As Boolean = False) As DataTable
        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeShoppingList", intCodeShoppingList)
        arrParam(1) = New SqlParameter("@intCodeTrans", intCodeTrans)
        arrParam(2) = New SqlParameter("@intCodeUser", intCodeUser)
        arrParam(3) = New SqlParameter("@UseProduct", useProduct)

        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "MP_GetShoppingListForPrint", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetPrintMasterPlanShoppingListName(ByVal intCodeShoppingList As Integer, ByVal intCodeUser As Integer) As DataTable
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeShoppingList", intCodeShoppingList)
        arrParam(1) = New SqlParameter("@intCodeUser", intCodeUser)

        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "MP_GetMasterPlanShoppingListName", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetMasterPlanTranslation(ByVal intCodeMasterPlan As Integer) As DataTable
        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeMasterPlan", intCodeMasterPlan)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "[MP_GETMasterPlanTranslation]", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetMenuSummaryForCopy(ByVal intIDMain As Integer, ByVal intDayPlan As Integer, ByVal intCodeMasterPlan As Integer, ByVal intCodeRestaurant As Integer, ByVal intCodeTrans As Integer) As DataTable
        Dim arrParam(4) As SqlParameter
        arrParam(0) = New SqlParameter("@intIDMain", intIDMain)
        arrParam(1) = New SqlParameter("@intDayPlan", intDayPlan)
        arrParam(2) = New SqlParameter("@intCodeMasterPlan", intCodeMasterPlan)
        arrParam(3) = New SqlParameter("@intCodeRestaurant", intCodeRestaurant)
        arrParam(4) = New SqlParameter("@intCodeTrans", intCodeTrans)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "[MP_GETCopyDetailByMainDayPlanAndCodeRestaurant]", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    Public Function GetMenuSummaryForCopyByDate(ByVal dts As DateTime, ByVal intCodeRestaurant As Integer, ByVal intCodeTrans As Integer) As DataTable
        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@dts", dts)
        arrParam(1) = New SqlParameter("@intCodeRestaurant", intCodeRestaurant)
        arrParam(2) = New SqlParameter("@intCodeTrans", intCodeTrans)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "[MP_GETCopyDetailByDate]", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    Public Function CopyPerMasterPlan(ByVal intIDMain As Integer, _
                                   ByVal intDayPlan As Integer, _
                                   ByVal intCodeRestaurant As Integer, _
                                   ByVal intIDMain2 As Integer, _
                                   ByVal intDayPlan2 As Integer, _
                                   ByVal intCodeRestaurant2 As Integer, _
                                   ByVal intCodeMasterPlan2 As Integer, _
                                   ByVal intNewCodeMasterPlan As Integer, _
                                   Optional ByVal blnOverwriteExisting As Boolean = False) As enumEgswErrorCode
        'TDQ 5.09.2011
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_COPYPerMasterplan]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intIDMain", SqlDbType.Int).Value = intIDMain
                .Parameters.Add("@intDayPlan", SqlDbType.Int).Value = intDayPlan
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@intIDMain2", SqlDbType.Int).Value = intIDMain2
                .Parameters.Add("@intDayPlan2", SqlDbType.Int).Value = intDayPlan2
                .Parameters.Add("@intCodeRestaurant2", SqlDbType.Int).Value = intCodeRestaurant2
                .Parameters.Add("@intCodeMasterPlan2", SqlDbType.Int).Value = intCodeMasterPlan2
                .Parameters.Add("@intNewCodeMasterPlan", SqlDbType.Int).Value = intNewCodeMasterPlan
                .Parameters.Add("@bitOverwriteExisting", SqlDbType.Bit).Value = blnOverwriteExisting

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
    Public Function CopyMasterPlan(ByVal intIDMain As Integer, _
                               ByVal intDayPlan As Integer, _
                               ByVal intCodeMasterPlan As Integer, _
                               ByVal intCodeRestaurant As Integer, _
                               ByVal intIDMain2 As Integer, _
                               ByVal intDayPlan2 As Integer, _
                               ByVal intCodeMasterPlan2 As Integer, _
                               ByVal intCodeRestaurant2 As Integer, _
                               Optional ByVal blnOverwriteExisting As Boolean = False, _
                               Optional ByVal intIDDetail As Integer = -1) As enumEgswErrorCode
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_COPYMasterplan]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intIDMain", SqlDbType.Int).Value = intIDMain
                .Parameters.Add("@intDayPlan", SqlDbType.Int).Value = intDayPlan
                .Parameters.Add("@intCodeMasterPlan", SqlDbType.Int).Value = intCodeMasterPlan
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
                .Parameters.Add("@intIDMain2", SqlDbType.Int).Value = intIDMain2
                .Parameters.Add("@intDayPlan2", SqlDbType.Int).Value = intDayPlan2
                .Parameters.Add("@intCodeMasterPlan2", SqlDbType.Int).Value = intCodeMasterPlan2
                .Parameters.Add("@intCodeRestaurant2", SqlDbType.Int).Value = intCodeRestaurant2
                .Parameters.Add("@bitOverwriteExisting", SqlDbType.Bit).Value = blnOverwriteExisting
                .Parameters.Add("@intIDDetail", SqlDbType.Int).Value = intIDDetail

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

    Public Function GetMenuPlanByRestaurant(ByVal CodeRestaurant As Integer) As Integer
        Dim cmd As New SqlCommand
        Dim CodeMenuPlan As Integer

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_GETMenuPlanByRestaurant"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@CodeRestaurant", SqlDbType.Int).Value = CodeRestaurant

                .Connection.Open()
                CodeMenuPlan = .ExecuteScalar()

            End With
            cmd.Connection.Close()
            cmd.Dispose()
        Catch ex As Exception
            Return -1
        Finally
            cmd.Dispose()
        End Try

        Return CodeMenuPlan
    End Function

    Public Function GetMenuPlanInfo(ByVal CodeMenuPlan As Integer, ByVal intCodeTrans As Integer) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_GETMenuPlanInfo"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@CodeMenuPlan", SqlDbType.Int).Value = CodeMenuPlan
                .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = intCodeTrans

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

    Public Function GetMenuPlanHQRestaurants(ByVal intCodeSite As Integer) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "SELECT Code FROM EgswMPRestaurant WHERE ISHQ = 1 AND CodeSite = @intCodeSite"
                .CommandType = CommandType.Text
                .Parameters.Add("@intCodeSite", SqlDbType.Int).Value = intCodeSite

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


    Public Function SaveMenuPlan(ByVal CodeMenuPlan As Integer, ByVal CodeUser As Integer, ByVal CodeTrans As Integer, ByVal Name As String, ByVal Number As String, ByVal Description As String,
                                 ByVal CodeRestaurant As Integer, ByVal CyclePlan As Integer, ByVal StartDate As DateTime, ByVal CodeSetPrice As Integer, ByVal Duration As Integer,
                                 ByVal Recurrence As Integer, ByVal CodeCategory As Integer, ByVal CodeSeason As Integer, ByVal CodeService As Integer, Optional ByVal MenuPlanImage As String = Nothing,
                                 Optional ByVal CodeMasterHQ As Integer = -1) As Integer
        Dim cmd As New SqlCommand
        Dim retVal As Integer

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_UPDATEMenuPlan"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@CodeMenuPlan", SqlDbType.Int).Value = CodeMenuPlan
                .Parameters.Add("@CodeUser", SqlDbType.Int).Value = CodeUser
                .Parameters.Add("@CodeTrans", SqlDbType.Int).Value = CodeTrans
                .Parameters.Add("@Name", SqlDbType.NVarChar, 200).Value = Name
                .Parameters.Add("@Number", SqlDbType.NVarChar, 50).Value = Number
                .Parameters.Add("@Description", SqlDbType.NVarChar, 1000).Value = Description
                .Parameters.Add("@CodeRestaurant", SqlDbType.Int).Value = CodeRestaurant
                .Parameters.Add("@CyclePlan", SqlDbType.Int).Value = CyclePlan
                .Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate
                .Parameters.Add("@CodeSetPrice", SqlDbType.Int).Value = CodeSetPrice
                .Parameters.Add("@Duration", SqlDbType.Int).Value = Duration
                .Parameters.Add("@Recurrence", SqlDbType.Int).Value = Recurrence
                .Parameters.Add("@CodeCategory", SqlDbType.Int).Value = CodeCategory
                .Parameters.Add("@CodeSeason", SqlDbType.Int).Value = CodeSeason
                .Parameters.Add("@CodeService", SqlDbType.Int).Value = CodeService
                If MenuPlanImage IsNot Nothing Then .Parameters.Add("@MenuPlanImage", SqlDbType.NVarChar).Value = MenuPlanImage 'RehaClinic
                If CodeMasterHQ > -1 Then .Parameters.Add("@CodeMasterHQ", SqlDbType.Int).Value = CodeMasterHQ
                .Parameters("@CodeMenuPlan").Direction = ParameterDirection.InputOutput

                .Connection.Open()
                .ExecuteNonQuery()

                CodeMenuPlan = CInt(.Parameters("@CodeMenuPlan").Value)
                Return CodeMenuPlan

            End With
            cmd.Connection.Close()
            cmd.Dispose()
        Catch ex As Exception
            cmd.Dispose()
        End Try
    End Function

    Public Function GetCyclePlanWeeks(ByVal CodeMenuPlan As Integer, ByVal SelectedDate As DateTime) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dt As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_GETCyclePlan"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@CodeMenuPlan", SqlDbType.Int).Value = CodeMenuPlan
                .Parameters.Add("@SelectedDate", SqlDbType.DateTime).Value = SelectedDate

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
        End Try
    End Function

    Public Function UpdateMain(ByVal CodeUser As Integer, ByVal StartDate As DateTime, ByVal dtsEndDate As DateTime, ByVal IDmain As Integer, ByVal CodeMenuPlan As Integer) As Integer
        Dim cmd As New SqlCommand
        Dim retVal As Integer

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_UPDATEMain"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intCodeUser", SqlDbType.Int).Value = CodeUser
                .Parameters.Add("@dtsStartDate", SqlDbType.DateTime).Value = StartDate
                .Parameters.Add("@dtsEndDate", SqlDbType.DateTime).Value = StartDate
                .Parameters.Add("@intIDMain", SqlDbType.Int).Value = IDmain
                .Parameters.Add("@intCodeMenuPlan", SqlDbType.Int).Value = CodeMenuPlan

                .Parameters("@intIDMain").Direction = ParameterDirection.InputOutput

                .Connection.Open()
                .ExecuteNonQuery()

                IDmain = CInt(.Parameters("@intIDMain").Value)
                Return IDmain

            End With
            cmd.Connection.Close()
            cmd.Dispose()
        Catch ex As Exception
            cmd.Dispose()
        End Try
    End Function

    Public Function DeleteMenuDetails(ByVal IDmain As Integer, ByVal CodeMasterPlanList As String) As Integer
        Dim cmd As New SqlCommand
        Dim retVal As Integer

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_DELETEMenuDetails"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@IDMain", SqlDbType.Int).Value = IDmain
                .Parameters.Add("@CodeMasterPlanList", SqlDbType.NVarChar, 100).Value = CodeMasterPlanList

                .Connection.Open()
                .ExecuteNonQuery()

            End With
            cmd.Connection.Close()
            cmd.Dispose()
        Catch ex As Exception
            cmd.Dispose()
        End Try
    End Function

    Public Function GetMasterPlanGroup(Optional ByVal CodeGroup As Integer = -1) As DataTable
        Dim arrParam(0) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeGroup", CodeGroup)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "[MP_GetMasterPlanGroup]", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function UpdateMasterPlanGroup(ByRef CodeGroup As Integer, ByVal Name As String) As Integer
        Dim cmd As New SqlCommand
        Dim intCode As Integer = -1

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_UPDATEMasterPlanGroup]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@CodeGroup", SqlDbType.Int).Direction = ParameterDirection.InputOutput
                .Parameters.Add("@Name", SqlDbType.NVarChar, 500).Value = Name
                .Parameters.Item("@CodeGroup").Value = CodeGroup

                .Connection.Open()
                .ExecuteNonQuery()

                CodeGroup = CIntDB(.Parameters("@CodeGroup").Value)

                cmd.Connection.Close()
                L_ErrCode = enumEgswErrorCode.OK
            End With
        Catch ex As Exception
            L_ErrCode = enumEgswErrorCode.GeneralError
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            Throw New Exception(ex.Message, ex)
        Finally
            cmd.Dispose()
        End Try
        Return CodeGroup
    End Function

    Public Function DeleteMasterPlanGroup(ByRef CodeGroupList As String) As DataTable
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim dtUsed As New DataTable

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[MP_DELETEMasterPlanGroup]"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@CodeGroupList", SqlDbType.NVarChar, 500).Value = CodeGroupList

                .Connection.Open()
                .ExecuteNonQuery()

                With da
                    .SelectCommand = cmd
                    dtUsed.BeginLoadData()
                    .Fill(dtUsed)
                    dtUsed.EndLoadData()
                End With
                Return dtUsed

                cmd.Connection.Close()
            End With
        Catch ex As Exception
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            Throw New Exception(ex.Message, ex)
        Finally
            cmd.Dispose()
        End Try
    End Function

    Public Function UPDATE_Details_Price(IDDetails As Integer, CodeSetPrice As Integer, PlanValue1 As Double, PlanValue2 As Double,
                                       Price As Double, CalcPrice As Double, DiscountedPrice As Double, CodeTax As Integer) As Integer
        Dim arrParam(7) As SqlParameter
        arrParam(0) = New SqlParameter("@IDDetails", IDDetails)
        arrParam(1) = New SqlParameter("@CodeSetPrice", CodeSetPrice)
        arrParam(2) = New SqlParameter("@PlanValue1", PlanValue1)
        arrParam(3) = New SqlParameter("@PlanValue2", PlanValue2)
        arrParam(4) = New SqlParameter("@Price", Price)
        arrParam(5) = New SqlParameter("@CalcPrice", CalcPrice)
        arrParam(6) = New SqlParameter("@DiscountedPrice", DiscountedPrice)
        arrParam(7) = New SqlParameter("@CodeTax", CodeTax)
        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "[MP_UDPATEDetailsPrice]", arrParam)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetDailyCost(idMain As Integer, codeSetPrice As Integer, codeRestaurant As Integer, isHQ As Boolean) As DataTable
        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@IDMain", idMain)
        arrParam(1) = New SqlParameter("@CodeSetPrice", codeSetPrice)
        arrParam(2) = New SqlParameter("@CodeRestaurant", codeRestaurant)
        arrParam(3) = New SqlParameter("@IsHQ", isHQ)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "[MP_GETDailyCost]", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetDailyMargin(idMain As Integer, codeSetPrice As Integer, codeRestaurant As Integer) As DataTable
        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@IDMain", idMain)
        arrParam(1) = New SqlParameter("@CodeSetPrice", codeSetPrice)
        arrParam(2) = New SqlParameter("@CodeRestaurant", codeRestaurant)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "[MP_GETDailyMargin]", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetDayPlanName(idMain As Integer, codeTrans As Integer) As DataTable
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@IDMain", idMain)
        arrParam(1) = New SqlParameter("@CodeTrans", codeTrans)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "[MP_GETDayPlanName]", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    Public Function UpdateDayPlanName(idMain As Integer, dayPlan As Integer, codeTrans As Integer, name As String) As Boolean
        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@IDMain", idMain)
        arrParam(1) = New SqlParameter("@DayPlan", dayPlan)
        arrParam(2) = New SqlParameter("@CodeTrans", codeTrans)
        arrParam(3) = New SqlParameter("@Name", SqlDbType.NVarChar, 50)
        arrParam(3).Value = name
        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "[MP_UDPATEDayPlanName]", arrParam)
            Return True
        Catch ex As Exception
            Return False
            Throw ex
        End Try
    End Function

    Public Function CopyMasterPlan(codeMasterPlanSource As Integer, codeUser As Integer) As Integer
        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeMPSource", codeMasterPlanSource)
        arrParam(1) = New SqlParameter("@CodeUser", codeUser)
        arrParam(2) = New SqlParameter("@CodeMPNew", SqlDbType.Int)
        arrParam(2).Direction = ParameterDirection.Output

        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "[MP_COPYMenuPlan]", arrParam)

            Return CInt(arrParam(2).Value)
        Catch ex As Exception
            Throw ex
            Return -1
        End Try
    End Function

    Public Function GetSeasonList(codeSite As Integer, codeTrans As Integer) As DataTable
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeSite", codeSite)
        arrParam(1) = New SqlParameter("@CodeTrans", codeTrans)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "[API_GET_SEASON]", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetServiceTypeList(codeSite As Integer, codeTrans As Integer) As DataTable
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeSite", codeSite)
        arrParam(1) = New SqlParameter("@CodeTrans", codeTrans)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "[API_GET_SERVICETYPE]", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetMasterPlanDetailsData(idDetails As Integer, codeTrans As Integer) As DataTable
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@IDDetails", idDetails)
        arrParam(1) = New SqlParameter("@CodeTrans", codeTrans)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "[MP_GETMasterPlanDetailsData]", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function UpdatePlannedValue(idMain As Integer, codeRestaurant As Integer, planValue As Integer, action As Integer, codeMasterPlan As Integer, dayPlan As Integer, codeMenuPlan As Integer) As Boolean
        Dim arrParam(6) As SqlParameter
        arrParam(0) = New SqlParameter("@IDMain", idMain)
        arrParam(1) = New SqlParameter("@CodeRestaurant", codeRestaurant)
        arrParam(2) = New SqlParameter("@PlanValue", planValue)
        arrParam(3) = New SqlParameter("@Action", action)
        arrParam(4) = New SqlParameter("@CodeMasterPlan", codeMasterPlan)
        arrParam(5) = New SqlParameter("@DayPlan", dayPlan)
        arrParam(6) = New SqlParameter("@CodeMenuPlan", codeMenuPlan)
        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "[MP_UDPATEPlannedValue]", arrParam)
            Return True
        Catch ex As Exception
            Throw ex
            Return False
        End Try
    End Function

    Public Function UpdateDetailsTranslation(idDetails As Integer, codeTrans As Integer, note As String, name As String, updateType As Integer, originalOrigin As String) As Boolean
        Dim arrParam(5) As SqlParameter
        arrParam(0) = New SqlParameter("@IDDetails", idDetails)
        arrParam(1) = New SqlParameter("@CodeTrans", codeTrans)
        arrParam(2) = New SqlParameter("@Note", note)
        arrParam(3) = New SqlParameter("@Name", name)
        arrParam(4) = New SqlParameter("@UpdateType", updateType)
        arrParam(5) = New SqlParameter("@OriginalOrigin", originalOrigin)
        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "[MP_UPDATEDetailsTranslation]", arrParam)
            Return True
        Catch ex As Exception
            Throw ex
            Return False
        End Try
    End Function

    Public Function GetDetailTranslation(idDetails As Integer, codeRestaurant As Integer) As DataTable
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@IDDetails", idDetails)
        arrParam(1) = New SqlParameter("@CodeRestaurant", codeRestaurant)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "[MP_GETDetailTranslation]", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
            Return Nothing
        End Try
    End Function

    Public Function LockMasterPLanItem(idDetails As Integer, lock As Boolean) As Boolean
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@IDDetails", idDetails)
        arrParam(1) = New SqlParameter("@Lock", lock)
        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "[MP_LOCKMasterPlanItem]", arrParam)
            Return True
        Catch ex As Exception
            Throw ex
            Return False
        End Try
    End Function

    Public Function DeactivateMasterPlanItem(idDetails As Integer, deactivate As Boolean) As Boolean
        Dim arrParam(1) As SqlParameter
        arrParam(0) = New SqlParameter("@IDDetails", idDetails)
        arrParam(1) = New SqlParameter("@Deactivate", deactivate)
        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "[MP_DEACTIVATEMasterPlanItem]", arrParam)
            Return True
        Catch ex As Exception
            Throw ex
            Return False
        End Try
    End Function

    Public Function CheckAutoNumber(codeSite As Integer, codeUser As Integer) As String
        Dim strNumber As String = String.Empty
        Dim arrParam(4) As SqlParameter
        arrParam(0) = New SqlParameter("@intCodeSite", codeSite)
        arrParam(1) = New SqlParameter("@intCodeUser", codeUser)
        arrParam(2) = New SqlParameter("@intItemType", 24)
        arrParam(3) = New SqlParameter("@updated", 0)
        arrParam(4) = New SqlParameter("@vchNumber", SqlDbType.NVarChar, 100)

        arrParam(4).Direction = ParameterDirection.Output
        Try
            ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "[sp_EgswGetAutoNum]", arrParam)
            strNumber = GetStr(arrParam(4).Value)
            Return strNumber
        Catch ex As Exception
            Throw ex
            Return False
        End Try
    End Function

    Public Sub RecalculateTotalCost()
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "[sp_MPRecalculateTotalCost]"
                .CommandType = CommandType.StoredProcedure
                .Connection.Open()
                .ExecuteNonQuery()

                cmd.Connection.Close()
            End With
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        Finally
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
        End Try
    End Sub
    Public Sub RecalculateTotalCostPerMenuPlan(ByVal CodeMenuplan As Integer)
        Dim cmd As New SqlCommand
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandTimeout = 5000
                .CommandText = "[sp_MPRecalculateTotalCost_PerMenuPlan]"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@CodeMenuplan", SqlDbType.Int).Value = CodeMenuplan
                .Connection.Open()
                .ExecuteNonQuery()

                cmd.Connection.Close()
            End With
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        Finally
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
        End Try
    End Sub
    Public Function UpdateDetail(ByVal IDMain As Integer, ByVal DayPlan As Integer, ByVal CodeMasterPlan As Integer, ByVal CodeRestaurant As Integer,
                                 ByVal IsLock As Boolean, ByVal PlanValue1 As Double, ByVal PlanValue2 As Double, ByVal Price As Double,
                                 ByVal CalcPrice As Double, ByVal CodeSetPrice As Integer, ByVal CodeTax As Integer, ByVal Name As String,
                                 ByVal Name_EN As String, ByVal Name_DE As String, ByVal Name_FR As String, ByVal Name_IT As String,
                                 ByVal IDDetail As Integer, ByVal DiscountedPrice As Double,
                                 Optional ByVal cboolKarma As Boolean = False,
                                 Optional ByVal cboolDisabledLogo As Boolean = False,
                                 Optional ByVal Note As String = "", Optional ByVal Note_EN As String = "", Optional ByVal Note_DE As String = "",
                                 Optional ByVal Note_FR As String = "", Optional ByVal Note_IT As String = "") As Integer
        Dim cmd As New SqlCommand
        Dim retVal As Integer

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_UPDATEDetail"
                .CommandType = CommandType.StoredProcedure

                .Parameters.Add("@intIDMain", SqlDbType.Int).Value = IDMain
                .Parameters.Add("@intDayPlan", SqlDbType.Int).Value = DayPlan
                .Parameters.Add("@intCodeMasterPlan", SqlDbType.Int).Value = CodeMasterPlan
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = CodeRestaurant
                .Parameters.Add("@bitIsLock", SqlDbType.Bit).Value = IsLock
                .Parameters.Add("@fltPlanValue1", SqlDbType.Float).Value = PlanValue1
                .Parameters.Add("@fltPlanValue2", SqlDbType.Float).Value = PlanValue2
                .Parameters.Add("@fltPrice", SqlDbType.Float).Value = Price
                .Parameters.Add("@fltCalcPrice", SqlDbType.Float).Value = CalcPrice
                .Parameters.Add("@intCodeSetPrice", SqlDbType.Int).Value = CodeSetPrice
                .Parameters.Add("@intCodeTax", SqlDbType.Int).Value = CodeTax
                .Parameters.Add("@nvcName", SqlDbType.NVarChar).Value = Name
                .Parameters.Add("@nvcName_EN", SqlDbType.NVarChar).Value = Name_EN
                .Parameters.Add("@nvcName_DE", SqlDbType.NVarChar).Value = Name_DE
                .Parameters.Add("@nvcName_FR", SqlDbType.NVarChar).Value = Name_FR
                .Parameters.Add("@nvcName_IT", SqlDbType.NVarChar).Value = Name_IT
                .Parameters.Add("@fltDiscountedPrice", SqlDbType.Float).Value = DiscountedPrice
                .Parameters.Add("@cboolKarma", SqlDbType.Bit).Value = cboolKarma
                .Parameters.Add("@cboolDisabledLogo", SqlDbType.Bit).Value = cboolDisabledLogo
                .Parameters.Add("@nvcNote", SqlDbType.NVarChar).Value = Note
                .Parameters.Add("@nvcNote_EN", SqlDbType.NVarChar).Value = Note_EN
                .Parameters.Add("@nvcNote_DE", SqlDbType.NVarChar).Value = Note_DE
                .Parameters.Add("@nvcNote_FR", SqlDbType.NVarChar).Value = Note_FR
                .Parameters.Add("@nvcNote_IT", SqlDbType.NVarChar).Value = Note_IT

                .Parameters.Add("@intID", SqlDbType.Int).Direction = ParameterDirection.InputOutput
                .Parameters("@intID").Value = IDDetail

                .Connection.Open()
                .ExecuteNonQuery()

                IDDetail = CInt(.Parameters("@intID").Value)
                Return IDDetail

            End With
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        Finally
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
        End Try
    End Function

    Public Function UpdateDetailsData(ByVal ID As Integer, ByVal IDDetails As Integer, ByVal Position As Integer, ByVal CodeListe As Integer,
                                      ByVal Quantity As Double, ByVal CodeUnit As Integer, ByVal IngrText As String, ByVal Percentage As Double) As Integer
        Dim cmd As New SqlCommand
        Dim retVal As Integer

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_UPDATEDetailsData"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.InputOutput
                .Parameters("@ID").Value = ID
                .Parameters.Add("@IDDetails", SqlDbType.Int).Value = IDDetails
                .Parameters.Add("@Position", SqlDbType.Int).Value = Position
                .Parameters.Add("@CodeListe", SqlDbType.Int).Value = CodeListe
                .Parameters.Add("@Quantity", SqlDbType.Float).Value = Quantity
                .Parameters.Add("@CodeUnit", SqlDbType.Int).Value = CodeUnit
                .Parameters.Add("@IngrText", SqlDbType.NVarChar).Value = IngrText
                .Parameters.Add("@Percentage", SqlDbType.Float).Value = Percentage

                .Connection.Open()
                .ExecuteNonQuery()

                ID = CInt(.Parameters("@ID").Value)
                Return ID

            End With
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        Finally
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
        End Try
    End Function

    Public Function DeleteDetailsData(ByVal IDDetails As Integer) As Integer
        Dim cmd As New SqlCommand
        Dim retVal As Integer

        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "MP_DELETEDetailsData"
                .CommandType = CommandType.StoredProcedure
                .Parameters.Add("@intID", SqlDbType.Int).Value = IDDetails
                .Parameters.Add("@intRetval", SqlDbType.Int).Direction = ParameterDirection.Output

                .Connection.Open()
                .ExecuteNonQuery()

                retVal = CInt(.Parameters("@intRetval").Value)
                Return retVal

            End With
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        Finally
            If cmd.Connection.State <> ConnectionState.Closed Then cmd.Connection.Close()
            cmd.Dispose()
        End Try
    End Function

    Public Function GetMenuplanList(ByVal CodeSite As Integer, ByVal CodeTrans As Integer, Optional IsHQ As Boolean? = Nothing) As DataTable
        Dim arrParam(2) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeSite", CodeSite)
        arrParam(1) = New SqlParameter("@CodeTrans", CodeTrans)
        arrParam(2) = New SqlParameter("@IsHQ", IsHQ)
        Try
            Return ExecuteDataset(L_strCnn, CommandType.StoredProcedure, "[MP_GETMenuplanList]", arrParam).Tables(0)
        Catch ex As Exception
            Throw ex
            Return Nothing
        End Try
    End Function

    Public Function CopyMenuPlanByWeekToLinkedRestaurant(ByVal CodeMenuPlan As Integer, ByVal Wk_StartDate As DateTime, ByVal CodeUser As Integer, ByVal IDMain As Integer) As Integer
        Dim arrParam(3) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeMPSource", CodeMenuPlan)
        arrParam(1) = New SqlParameter("@Wk_StartDate", Wk_StartDate)
        arrParam(2) = New SqlParameter("@CodeUser", CodeUser)
        arrParam(3) = New SqlParameter("@IDMainSource", IDMain)

        Try
            Return ExecuteScalar(L_strCnn, CommandType.StoredProcedure, "[MP_COPYMenuPlanByWeekToLinkedRestaurant]", arrParam)
        Catch ex As Exception
            Throw ex
            Return -1
        End Try
    End Function

    Public Function CopyItemsByDateRange(ByVal CodeMenuPlan As Integer, ByVal Wk_StartDate As DateTime, ByVal Wk_EndDate As DateTime, ByVal Wk_DestDate As DateTime,
                                         ByVal CodeUser As Integer, ByVal CodeMenuPlan2 As Integer, ByVal Overwrite As Boolean, ByVal MoveData As Boolean) As Integer
        Dim arrParam(7) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeMPSource", CodeMenuPlan)
        arrParam(1) = New SqlParameter("@Wk_StartDate", Wk_StartDate)
        arrParam(2) = New SqlParameter("@Wk_EndDate", Wk_EndDate)
        arrParam(3) = New SqlParameter("@Wk_DestDate", Wk_DestDate)
        arrParam(4) = New SqlParameter("@Overwrite", Overwrite)
        arrParam(5) = New SqlParameter("@CodeUser", CodeUser)
        arrParam(6) = New SqlParameter("@CodeMPNew", CodeMenuPlan2)
        arrParam(7) = New SqlParameter("@MoveData", MoveData)

        Try
            Return ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "[MP_COPYMenuPlanByDateRange]", arrParam)
        Catch ex As Exception
            Throw ex
            Return Nothing
        End Try
    End Function
    Public Function CopyItemsByDateRange2(ByVal CodeMenuPlan As Integer, ByVal Wk_StartDate As DateTime, ByVal Wk_EndDate As DateTime, ByVal Wk_DestDate As DateTime,
                                        ByVal CodeUser As Integer, ByVal CodeMenuPlan2 As Integer, ByVal Overwrite As Boolean, ByVal MoveData As Boolean) As Integer
        Dim arrParam(7) As SqlParameter
        arrParam(0) = New SqlParameter("@CodeMPSource", CodeMenuPlan)
        arrParam(1) = New SqlParameter("@Wk_SourceStartDate", Wk_StartDate)
        arrParam(2) = New SqlParameter("@Wk_SourceEndDate", Wk_EndDate)
        arrParam(3) = New SqlParameter("@Wk_DestDate", Wk_DestDate)
        arrParam(4) = New SqlParameter("@Overwrite", Overwrite)
        arrParam(5) = New SqlParameter("@CodeUser", CodeUser)
        arrParam(6) = New SqlParameter("@CodeMPNew", CodeMenuPlan2)
        arrParam(7) = New SqlParameter("@MoveData", MoveData)

        Try
            Return ExecuteNonQuery(L_strCnn, CommandType.StoredProcedure, "[MP_COPYMenuPlanByDateRange2]", arrParam)
        Catch ex As Exception
            Throw ex
            Return Nothing
        End Try
    End Function

    Public Function GetUserRole(ByVal CodeUser As Integer) As Boolean
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader
        Dim intCount As Integer

        Dim sb As New StringBuilder
        sb.Append("SELECT * FROM EgswUserRoles WHERE CodeUser = " & CodeUser & " AND Role IN (19, 20)")

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
    Public Function GetIsoWeek(ByVal IsoDate As String) As Integer
        Dim ISOWeek As Integer = 0
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = "select DATEPART(ISO_WEEK, @Date) as WeekNo"
                .CommandType = CommandType.Text
                .Parameters.Add("@Date", SqlDbType.NVarChar, 50).Value = IsoDate
                .Connection.Open()
                dr = .ExecuteReader()
                While dr.Read
                    ISOWeek = CInt(dr.Item("WeekNo"))
                End While
                .Connection.Close()
                .Connection.Dispose()
            End With
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try


        Return ISOWeek
    End Function
    Public Function GetFirstDayFromIsoWeek(ByVal IsoDate As String) As DateTime
        Dim ISOWeek As DateTime
        Dim cmd As New SqlCommand
        Dim dr As SqlDataReader
        Try
            With cmd
                .Connection = New SqlConnection(L_strCnn)
                .CommandText = <sql>
                                   SELECT (
                                    CASE DATEPART(ISO_WEEK, @date) 
                                        WHEN DATEPART(ISO_WEEK, DATEADD(DAY,-6,@date)) THEN DATEADD(DAY,-6,@date)
                                        WHEN DATEPART(ISO_WEEK, DATEADD(DAY,-5,@date)) THEN DATEADD(DAY,-5,@date)
                                        WHEN DATEPART(ISO_WEEK, DATEADD(DAY,-4,@date)) THEN DATEADD(DAY,-4,@date)
                                        WHEN DATEPART(ISO_WEEK, DATEADD(DAY,-3,@date)) THEN DATEADD(DAY,-3,@date)
                                        WHEN DATEPART(ISO_WEEK, DATEADD(DAY,-2,@date)) THEN DATEADD(DAY,-2,@date)
                                        WHEN DATEPART(ISO_WEEK, DATEADD(DAY,-1,@date)) THEN DATEADD(DAY,-1,@date)
                                        ELSE DATEADD(DAY,0,@date)
                                    END
                                ) FirstDayOfISOWeek
                               </sql>.Value
                .CommandType = CommandType.Text
                .Parameters.Add("@Date", SqlDbType.NVarChar, 50).Value = IsoDate
                .Connection.Open()
                dr = .ExecuteReader()
                While dr.Read
                    ISOWeek = CDate(dr.Item("FirstDayOfISOWeek"))
                End While
                .Connection.Close()
                .Connection.Dispose()
            End With
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try


        Return ISOWeek
    End Function

    'EatCH Functions - START
    Public Function GetMasterPlanEatCHByRestaurant(ByVal intCodeRestaurant As Integer) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter

        Try
            With cmd
                .Connection = cn
                .CommandType = CommandType.Text
                .CommandText = "SELECT * FROM EgswMPRestaurantMasterPlanEatCH WHERE CodeRestaurant = @intCodeRestaurant"
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
            End With

            cn.Open()
            da.SelectCommand = cmd
            dt.BeginLoadData()
            da.Fill(dt)
            dt.EndLoadData()

            Return dt
        Catch ex As Exception
            Return dt
        Finally
            cmd.Connection.Close()
            cmd.Connection.Dispose()
        End Try
    End Function

    Public Function GetMasterPlanRestaurantNumber(ByVal intCodeRestaurant As Integer) As DataTable
        Dim cn As New SqlConnection(L_strCnn)
        Dim cmd As New SqlCommand
        Dim dt As New DataTable
        Dim da As New SqlDataAdapter

        Try
            With cmd
                .Connection = cn
                .CommandType = CommandType.Text
                .CommandText = "SELECT * FROM EgswMPRestaurant WHERE Code = @intCodeRestaurant"
                .Parameters.Add("@intCodeRestaurant", SqlDbType.Int).Value = intCodeRestaurant
            End With

            cn.Open()
            da.SelectCommand = cmd
            dt.BeginLoadData()
            da.Fill(dt)
            dt.EndLoadData()

            Return dt
        Catch ex As Exception
            Return dt
        Finally
            cmd.Connection.Close()
            cmd.Connection.Dispose()
        End Try
    End Function
    'EatCH Functions - END
End Class

Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraPrinting.PrinterSettingsUsing
Imports DevExpress.XtraPrinting.PageHeaderArea
Imports DevExpress.XtraPrinting.Drawing
Imports DevExpress.XtraReports.UI.XRControl
Imports DevExpress.XtraReports

Imports System.Diagnostics
Imports System
Imports System.Web
Imports System.Drawing
Imports System.Drawing.Printing
Imports System.Configuration
Imports System.Security
Imports System.Security.Permissions
Imports System.Data.SqlClient
Imports System.Data
Imports System.Data.SqlClient.SqlClientPermission
Imports System.IO
Imports System.Drawing.Imaging
Imports System.Windows.Forms

Imports EgsReport_CMC.clsGlobal
Imports EgsReport_CMC
Imports EgsReport_CMC.xrReports
Imports EgsData

Imports Microsoft.Office
Imports Microsoft.Office.Tools.Excel
Imports Microsoft.Office.Interop.Owc11
Imports Microsoft.Office.Interop



Public Class clsGenericDevExpress
    Inherits DevExpress.XtraReports.UI.XtraReport


    Private Report1 As XtraReport
    Dim WithEvents Detail As New DetailBand

    Dim PageFoot As New PageFooterBand

    Private ReportHeader As New ReportHeaderBand
    Private BottomMargin As New BottomMarginBand
    Private ReportHeaderLabel As New XRLabel
    Private MyXrTable As New XRTable
    Private MyXrPageInfo As New XRPageInfo
    Private XrPanelBottomMargin As New XRPanel

    Private fntHeading As New System.Drawing.Font("Arial", 12.0!, FontStyle.Bold)
    Private fntBodyBold As New System.Drawing.Font("Arial", 8.0!, FontStyle.Bold)
    Private fntBody As New System.Drawing.Font("Arial", 8.0!, FontStyle.Regular)
    Private fntBodyItalic As New System.Drawing.Font("Arial", 8.0!, FontStyle.Italic)
    Private intGlobalMultiplier As Integer
    Private intGlobalX As Integer

    Private sf1 As StringFormat = New StringFormat(StringFormatFlags.NoClip)

    Dim strX As String
    Dim fntDetail1, fntDetail2 As System.Drawing.Font
    Dim fntDayLabel As System.Drawing.Font
    Dim intCurrentX, intCurrentY As Integer
    Dim XrTable1 As New XRTable
    Dim XrLine1 As New XRLine
    Public Function printMP(ByVal dsMain As DataSet, ByVal strFileNamePDF As String)
        Dim strMyMessage As String
        Dim strLabelText As String
        Dim DefaultPageHeight As Double
        Dim DefaultPageWidth As Double
        Dim ReportLeftMargin As Double
        Dim ReportRightMargin As Double
        Dim ReportTopMargin As Double
        Dim ReportBottomMargin As Double
        Dim TableWidth As Double
        Dim intPreviousForLocal As Integer = 0
        Dim strPreviousCountry As String = ""
        Dim strSpace As String = "   "
        Dim XrTable1 As New XRTable
        Dim dtMasterPlan As DataTable = dsMain.Tables(1)
        Dim dtRestaurants As DataTable = dsMain.Tables(0)
        Dim dtMasterPlanValues As DataTable = dsMain.Tables(2)
        Dim dtDates As DataTable = dsMain.Tables(3)
        Dim strDay As String


        fntDayLabel = New System.Drawing.Font("Arial", 10, FontStyle.Bold)
        DefaultPageHeight = 2101
        DefaultPageWidth = 2970
        ReportLeftMargin = 254
        ReportRightMargin = 254
        ReportTopMargin = 254
        ReportBottomMargin = 254

        TableWidth = DefaultPageWidth - (ReportLeftMargin + ReportRightMargin) - 1

        Dim fntPageInfo As System.Drawing.Font = New System.Drawing.Font("Arial Narrow", 6.25!, FontStyle.Regular)

        fntDayLabel = New System.Drawing.Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point)
        fntDetail1 = New System.Drawing.Font("Arial", 8, FontStyle.Bold, GraphicsUnit.Point)
        fntDetail2 = New System.Drawing.Font("Arial", 8, FontStyle.Regular, GraphicsUnit.Point)
        strLabelText = ""

        'strMyMessage = OpenConnection()

        Report1 = New XtraReport
        With Report1
            .ReportUnit = ReportUnit.TenthsOfAMillimeter
            'Papersize
            '------------------------------------------------------------------------------------
            .PaperKind = PaperKind.Custom
            .PageWidth = DefaultPageWidth
            .PageHeight = DefaultPageHeight
            '------------------------------------------------------------------------------------
            'Margins
            '------------------------------------------------------------------------------------
            .Margins.Left = ReportLeftMargin
            .Margins.Right = ReportRightMargin
            .Margins.Bottom = ReportBottomMargin
            .Margins.Top = ReportTopMargin

            '------------------------------------------------------------------------------------
            'Orientation
            '------------------------------------------------------------------------------------
            .Landscape = False

            '------------------------------------------------------------------------------------
            'ADD THE SECTION OF THE REPORT 
            '------------------------------------------------------------------------------------
            'ReportHeader
            ReportHeader.Dpi = 254.0!
            ReportHeader.Height = 0
            ReportHeader.Name = "ReportHeader"
            'BottomMargin      
            BottomMargin.Dpi = 254.0! '
            BottomMargin.Height = 0
            BottomMargin.Name = "BottomMargin"
            'Details
            Detail.Dpi = 254.0!
            Detail.Height = DefaultPageHeight - (ReportTopMargin + ReportBottomMargin) - 2 '2664
            Detail.Name = "Detail"

            Dim ctr As Integer = 0
            For ctr = 1 To 6

                Detail.Controls.AddRange(New XRControl() {fctMakeXrPanel(dtMasterPlan, dtRestaurants, dtMasterPlanValues, dtDates, ctr, Detail.Height - 2, DefaultPageWidth - (ReportLeftMargin + ReportRightMargin) - 2)})
            Next




            '--- final report -----
            .Bands.Add(ReportHeader)    'REPORT HEADER SECTION
            .Bands.Add(BottomMargin)    'REPORT BOTTOM MARGIN
            .Bands.Add(Detail)          'DETAIL SECTION
            .CreatePdfDocument(strFileNamePDF)
        End With

        strMyMessage = "" 'fctExportToPdfFormat(strFileNamePDF)
        Return strMyMessage

    End Function
    Public Function fctMakeXrPanel(ByVal dtMasterPlan As DataTable, ByVal dtRestaurants As DataTable, ByVal dtMasterPlanValues As DataTable, ByVal dtDates As DataTable, ByVal intDayPlan As Integer, ByVal intHeight As Integer, ByVal intWidth As Integer) As XRPanel
        Dim XrPanel1 As New XRPanel
        Dim intControlHeight1, intControlHeight2, intCurrentX, intCurrentY, intColWidth, intHeaderWidth, intTableStartX, intControlWidth1, intControlWidth2, intControlWidth3 As Integer
        'intCurrentX = 100 + (2981 * (intDayPlan - 1))
        'intCurrentY = 20 + (2109 * (intDayPlan - 1))
        intCurrentY = 0
        XrPanel1.Dpi = 254.0!
        XrPanel1.Size = New System.Drawing.Size(intWidth, intHeight)
        'XrPanel1.Height = intHeight
        'XrPanel1.Width = intWidth
        XrPanel1.Location = New System.Drawing.Point(0, 0 + (intHeight * (intDayPlan - 1)))
        Select Case intDayPlan
            Case 1
                strX = "Monday"
            Case 2
                strX = "Tuesday"
            Case 3
                strX = "Wednesday"
            Case 4
                strX = "Thursday"
            Case 5
                strX = "Friday"
            Case 6
                strX = "Saturday"
        End Select
        Dim XrLine1 As New XRLine
        Dim dateLabel As Date
        'day label
        dateLabel = CDate(dtDates.Rows(0).Item("startdate"))
        strX = strX & " " & dateLabel.Date.AddDays(intDayPlan - 1)
        intControlHeight1 = MeasureText(strX, fntDayLabel, 800, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
        XrPanel1.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDayLabel, Color.Black, Color.Transparent, 0, 0, 600, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})
        'day label end
        intCurrentY = intControlHeight1 + 100
        'draw thin line
        XrLine1.Dpi = 254.0!
        XrLine1.LineStyle = Drawing2D.DashStyle.Solid
        XrLine1.LineWidth = 1
        XrLine1.Location = New System.Drawing.Point(0, intCurrentY - 10)
        XrLine1.Size = New System.Drawing.Size(intWidth, 2)
        XrLine1.Visible = True
        XrPanel1.Controls.AddRange(New XRControl() {XrLine1})
        'draw thin line end
        'insert column header

        intControlHeight2 = MeasureText("N.R.", fntDetail1, intControlWidth3, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
        XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2("N.R.", fntDetail1, Color.Black, Color.Transparent, 0, intCurrentY + 10, 100, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
        intControlHeight2 = MeasureText("Restaurant", fntDetail1, intControlWidth1, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
        XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2("Restaurant", fntDetail1, Color.Black, Color.Transparent, 100 + 10, intCurrentY + 10, 600, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})

        intCurrentX = intWidth - 755

        'insert menu names

        intColWidth = intCurrentX / dtMasterPlan.Rows.Count


        intHeaderWidth = (intColWidth + 5) * dtMasterPlan.Rows.Count
        intCurrentX = intWidth - intHeaderWidth
        intTableStartX = intCurrentX
        For Each dtRow As DataRow In dtMasterPlan.Rows
            strX = dtRow.Item("name").ToString
            intControlHeight1 = MeasureText(strX, fntDetail1, intColWidth, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height()
            XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, intCurrentX, intCurrentY + 10, intColWidth, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            If intControlHeight2 < intControlHeight1 Then
                intControlHeight2 = intControlHeight1
            End If
            intCurrentX += intColWidth + 5
        Next
        'insert menu names
        'insert column header end
        intCurrentY = intCurrentY + intControlHeight2
        Dim XrLine2 As New XRLine
        'draw thick line
        XrLine2.Dpi = 254.0!
        XrLine2.LineStyle = Drawing2D.DashStyle.Solid
        XrLine2.LineWidth = 5
        XrLine2.Location = New System.Drawing.Point(0, intCurrentY + 10)
        XrLine2.Size = New System.Drawing.Size(intWidth, 6)
        XrLine2.Visible = True
        XrPanel1.Controls.AddRange(New XRControl() {XrLine2})
        'draw thick line end

        intCurrentY += 25

        'insert restaurants and values
        For Each dtRow As DataRow In dtRestaurants.Rows
            strX = dtRow.Item("codeRestaurant").ToString
            intControlHeight2 = MeasureText(strX, fntDetail1, 150, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
            XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, 0, intCurrentY + 10, 100, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            strX = dtRow.Item("name").ToString
            intControlHeight2 = MeasureText(strX, fntDetail1, 600, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
            XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, 100 + 10, intCurrentY + 10, 600, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})

            intCurrentX = intTableStartX

            'insert plan values
            For Each dtRow2 As DataRow In dtMasterPlan.Rows
                strX = ""
                For Each dtrow3 As DataRow In dtMasterPlanValues.Select("coderestaurant=" & dtRow.Item("coderestaurant").ToString & " and codeMasterplan=" & dtRow2.Item("codemasterplan") & " and dayplan=" & intDayPlan)
                    strX = dtrow3.Item("planvalue1").ToString
                Next
                intControlHeight1 = MeasureText(strX, fntDetail1, intColWidth, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height()
                If intControlHeight1 < 5 Then
                    intControlHeight1 = 40
                End If
                XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY + 10, intColWidth, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopCenter)})
                If intControlHeight2 < intControlHeight1 Then
                    intControlHeight2 = intControlHeight1
                End If
                intCurrentX += intColWidth + 5
            Next
            'insert plan values end
            intCurrentY += intControlHeight2
        Next
        'insert restaurants and values end

        'XrPanel1.Controls.AddRange(New XRControl() {fctGetMenuRow(dtMasterPlan, intWidth)})
        'For Each dtRow As DataRow In dtRestaurants.Rows
        '    XrPanel1.Controls.AddRange(New XRControl() {fctGetRestaurantRow(dtRow.Item("coderestaurant"), dtRow.Item("name"), 1)})
        'Next
        Return XrPanel1
    End Function
    'Public Function fctGetMenuRow(ByVal dtMasterPlan As DataTable, ByVal intWidth As Integer) As XRPanel
    '    Dim XrPanel1 As New XRPanel
    '    Dim intCellWidth As Integer
    '    fntDetail1 = New Font("Arial", 10, FontStyle.Bold)

    '    intCellWidth = (2700 - 700) / dtMasterPlan.Rows.Count
    '    xrPanel1.Dpi = 254.0!
    '    XrPanel1.Width = intWidth

    '    XrPanel1.Location = New System.Drawing.Point(0, 50)
    '    'For Each dtRow As DataRow In dtMasterPlan.Rows
    '    '    strX = dtRow.Item("name").ToString
    '    '    XrCell3 = fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, 0, 0, intCellWidth, 50, DevExpress.XtraPrinting.TextAlignment.TopLeft)
    '    '    XrRow1.Cells.AddRange(New XRTableCell() {XrCell3})
    '    'Next

    '    'XrRow1.CanShrink = True

    '    Return XrPanel1
    'End Function
    'Public Function fctGetRestaurantRow(ByVal strCodeRestaurant As String, ByVal strNameRestaurant As String, ByVal intDayPlan As Integer) As XRTableRow
    '    Dim XrRow1 As New XRTableRow
    '    Dim xrCell As XRTableCell
    '    xrCell.Dpi = 254.0!
    '    xrCell = fctMakeXrLabel2(strCodeRestaurant, fntDetail1, Color.Black, Color.Transparent, 0, 0, 100, 20, DevExpress.XtraPrinting.TextAlignment.TopLeft)
    '    XrRow1.Controls.AddRange((New XRControl() {xrCell}))
    '    intCurrentX += 20
    '    XrRow1.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strNameRestaurant, fntDetail1, Color.Black, Color.Transparent, 0, 0, 600, 20, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})
    '    intCurrentX = 100
    '    For Each dtRow2 As DataRow In dtMasterPlan.Rows
    '        strX = ""
    '        For Each dtRow3 As DataRow In dtMasterPlanValues.Select("coderestaurant=" & dtRow.Item("coderestaurant").ToString & " and codemasterplan=" & dtRow2.Item("codeMasterPlan").ToString & " and dayplan=" & intDayPlan)
    '            strX = dtRow3.Item("planValue1")
    '        Next
    '        XrPanel1.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY, 50, 20, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
    '        intCurrentX += 50
    '    Next
    '    intCurrentX = 20
    '    intCurrentY += 30
    'End Function
    Public Function fctMakeXrLabel2(ByVal strText As String, ByVal fntFont As System.Drawing.Font, _
                                           ByVal TextColor As System.Drawing.Color, ByVal BackColor As System.Drawing.Color, _
                                           ByVal intX As Integer, ByVal intY As Integer, _
                                           ByVal intSizeX As Integer, ByVal intSizeY As Integer, _
                                           Optional ByVal DEAllignment As DevExpress.XtraPrinting.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft, _
                                           Optional ByVal blnCanGrow As Boolean = False, Optional ByVal blnMultiline As Boolean = False, _
                                           Optional ByVal blnKeepTogether As Boolean = False) As DevExpress.XtraReports.UI.XRLabel 'VRP 04.07.2008
        Dim XRLabel1 As New DevExpress.XtraReports.UI.XRLabel
        With XRLabel1
            .Name = "XrLabel1"
            .Font = fntFont

            .ForeColor = TextColor
            .BackColor = BackColor

            .Size = New System.Drawing.Size(intSizeX, intSizeY)
            .Location = New System.Drawing.Point(intX, intY)
            .Text = strText
            .CanGrow = True
            .CanShrink = False
            .TextAlignment = DEAllignment
            .Multiline = blnMultiline
            .WordWrap = True
            .KeepTogether = blnKeepTogether

        End With

        XRLabel1.Dpi = 254.0!

        Return XRLabel1
    End Function
    Public Function fctMakeXrLabelNoDpi(ByVal strText As String, ByVal fntFont As System.Drawing.Font, _
                                           ByVal TextColor As System.Drawing.Color, ByVal BackColor As System.Drawing.Color, _
                                           ByVal intX As Integer, ByVal intY As Integer, _
                                           ByVal intSizeX As Integer, ByVal intSizeY As Integer, _
                                           Optional ByVal DEAllignment As DevExpress.XtraPrinting.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft, _
                                           Optional ByVal blnCanGrow As Boolean = False, Optional ByVal blnMultiline As Boolean = False, _
                                           Optional ByVal blnKeepTogether As Boolean = False) As DevExpress.XtraReports.UI.XRLabel 'VRP 04.07.2008
        Dim XRLabel1 As New DevExpress.XtraReports.UI.XRLabel
        With XRLabel1
            .Name = "XrLabel1"

            .Font = fntFont

            '.Borders = DevExpress.XtraPrinting.BorderSide.Bottom
            .ForeColor = TextColor
            .BackColor = BackColor

            .Size = New System.Drawing.Size(intSizeX, intSizeY)
            .Location = New System.Drawing.Point(intX, intY)
            .Text = strText
            .CanGrow = True
            .CanShrink = False
            .TextAlignment = DEAllignment
            .Multiline = blnMultiline
            .WordWrap = True
            .KeepTogether = blnKeepTogether
            .Borders = DevExpress.XtraPrinting.BorderSide.None
        End With

        'XRLabel1.Dpi = 254.0!

        Return XRLabel1
    End Function
    Private Function fctExportToPdfFormat(ByVal PdfPath As String) As String
        Dim strMessage As String = ""

        With Report1
            ' Get its PDF export options.
            'Dim pdfOptions As PdfExportOptions = Report.ExportOptions.Pdf

            'Try

            .CreatePdfDocument(PdfPath)
            'Catch ex As Exception
            'strMessage = ex.Message
            'End Try
        End With
        Return strMessage
    End Function
    Public Function printMPSinglePage(ByVal dsMain As DataSet, ByVal strFileNamePDF As String) As String
        Dim strMsg As String
        Dim strLabelText As String
        Dim DefaultPageHeight As Double
        Dim DefaultPageWidth As Double
        Dim ReportLeftMargin As Double
        Dim ReportRightMargin As Double
        Dim ReportTopMargin As Double
        Dim ReportBottomMargin As Double
        Dim TableWidth As Double
        Dim intPreviousForLocal As Integer = 0
        Dim strPreviousCountry As String = ""
        Dim strSpace As String = "   "
        Dim XrTable1 As New XRTable
        Dim dtMasterPlan As DataTable = dsMain.Tables(1)
        Dim dtRestaurants As DataTable = dsMain.Tables(0)
        Dim dtMasterPlanValues As DataTable = dsMain.Tables(2)
        Dim dtDates As DataTable = dsMain.Tables(3)
        Dim strDay As String


        fntDayLabel = New System.Drawing.Font("Arial", 10, FontStyle.Bold)
        DefaultPageHeight = 2159
        DefaultPageWidth = 3556
        ReportLeftMargin = 20
        ReportRightMargin = 20
        ReportTopMargin = 254
        ReportBottomMargin = 20

        TableWidth = DefaultPageWidth - (ReportLeftMargin + ReportRightMargin) - 1

        Dim fntPageInfo As System.Drawing.Font = New System.Drawing.Font("Arial Narrow", 6.25!, FontStyle.Regular)

        fntDayLabel = New System.Drawing.Font("Arial", 6, FontStyle.Bold, GraphicsUnit.Point)
        fntDetail1 = New System.Drawing.Font("Arial", 4, FontStyle.Bold, GraphicsUnit.Point)
        fntDetail2 = New System.Drawing.Font("Arial", 4, FontStyle.Regular, GraphicsUnit.Point)
        strLabelText = ""

        'strMyMessage = OpenConnection()

        Report1 = New XtraReport
        With Report1
            .ReportUnit = ReportUnit.TenthsOfAMillimeter
            'Papersize
            '------------------------------------------------------------------------------------
            .PaperKind = PaperKind.Custom
            .PageWidth = DefaultPageWidth
            .PageHeight = DefaultPageHeight
            '------------------------------------------------------------------------------------
            'Margins
            '------------------------------------------------------------------------------------
            .Margins.Left = ReportLeftMargin
            .Margins.Right = ReportRightMargin
            .Margins.Bottom = ReportBottomMargin
            .Margins.Top = ReportTopMargin

            '------------------------------------------------------------------------------------
            'Orientation
            '------------------------------------------------------------------------------------
            .Landscape = False

            '------------------------------------------------------------------------------------
            'ADD THE SECTION OF THE REPORT 
            '------------------------------------------------------------------------------------
            'ReportHeader
            ReportHeader.Dpi = 254.0!
            ReportHeader.Height = 0
            ReportHeader.Name = "ReportHeader"
            'BottomMargin      
            BottomMargin.Dpi = 254.0! '
            BottomMargin.Height = 0
            BottomMargin.Name = "BottomMargin"
            'Details
            Detail.Dpi = 254.0!
            Detail.Height = DefaultPageHeight - (ReportTopMargin + ReportBottomMargin) - 2 '2664
            Detail.Name = "Detail"
            Detail.Controls.AddRange(New XRControl() {fctMakeXrPanelSinglePage(dtMasterPlan, dtRestaurants, dtMasterPlanValues, dtDates, 1, Detail.Height - 2, DefaultPageWidth - (ReportLeftMargin + ReportRightMargin) - 2)})





            '--- final report -----
            .Bands.Add(ReportHeader)    'REPORT HEADER SECTION
            .Bands.Add(BottomMargin)    'REPORT BOTTOM MARGIN
            .Bands.Add(Detail)          'DETAIL SECTION
            .CreatePdfDocument(strFileNamePDF)
        End With
        Return strMsg
    End Function
    Public Function fctMakeXrPanelSinglePage(ByVal dtMasterPlan As DataTable, ByVal dtRestaurants As DataTable, ByVal dtMasterPlanValues As DataTable, ByVal dtDates As DataTable, ByVal intDayPlan As Integer, ByVal intHeight As Integer, ByVal intWidth As Integer) As XRPanel
        Dim XrPanel1 As New XRPanel
        Dim clrDayLabelBackColor As New System.Drawing.Color
        Dim intControlHeight1, intControlHeight2, intCurrentX, intCurrentY, intColWidth, intHeaderWidth, intTableStartX, intControlWidth1, intControlWidth2, intControlWidth3 As Integer
        'intCurrentX = 100 + (2981 * (intDayPlan - 1))
        'intCurrentY = 20 + (2109 * (intDayPlan - 1))
        intCurrentY = 0
        XrPanel1.Dpi = 254.0!
        XrPanel1.Size = New System.Drawing.Size(intWidth - 3, intHeight - 3)
        'XrPanel1.Height = intHeight
        'XrPanel1.Width = intWidth
        XrPanel1.Location = New System.Drawing.Point(0, 0)
        intCurrentX = 0
        intCurrentY = 0
        Dim dateLabelStart, dateLabelEnd As Date
        dateLabelStart = CDate(dtDates.Rows(0).Item("startdate"))
        dateLabelEnd = CDate(dtDates.Rows(0).Item("enddate"))
        strX = "Start Date: " & dateLabelStart.Date & vbTab & "End Date: " & dateLabelEnd.Date
        intControlHeight1 = MeasureText(strX, fntDetail1, 600, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
        XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDayLabel, Color.Black, Color.Transparent, 0, intCurrentY, 600, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
        intCurrentY = intControlHeight1 + 2
        Dim XrLine1 As New XRLine
        'draw thin line
        XrLine1.Dpi = 254.0!
        XrLine1.LineStyle = Drawing2D.DashStyle.Solid
        XrLine1.LineWidth = 1
        XrLine1.Location = New System.Drawing.Point(0, intCurrentY)
        XrLine1.Size = New System.Drawing.Size(intWidth, 2)
        XrLine1.Visible = True
        XrPanel1.Controls.AddRange(New XRControl() {XrLine1})
        'draw thin line end

        'insert column header
        intControlHeight2 = MeasureText("N.R.", fntDetail1, 50, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
        XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2("N.R.", fntDetail1, Color.Black, Color.Transparent, 0, intCurrentY + 5, 50, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
        intControlHeight2 = MeasureText("Restaurant", fntDetail1, 100, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
        XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2("Restaurant", fntDetail1, Color.Black, Color.Transparent, 52, intCurrentY + 10, 100, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})

        intCurrentX = intWidth - 155

        'insert menu names
        Dim ctr As Integer

        intColWidth = (intCurrentX / (dtMasterPlan.Rows.Count * 6)) - 2.3

        intHeaderWidth = (intColWidth + 2) * dtMasterPlan.Rows.Count * 6
        intCurrentX = 157
        intTableStartX = intCurrentX
        For ctr = 1 To 6
            If ctr Mod 2 = 1 Then
                clrDayLabelBackColor = System.Drawing.Color.Transparent
            Else
                clrDayLabelBackColor = System.Drawing.Color.Transparent
            End If
            For Each dtRow As DataRow In dtMasterPlan.Rows
                strX = dtRow.Item("name").ToString
                intControlHeight1 = MeasureText(strX, fntDetail1, intColWidth, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height()
                XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, clrDayLabelBackColor, intCurrentX, intCurrentY + 10, intColWidth, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
                If intControlHeight2 < intControlHeight1 Then
                    intControlHeight2 = intControlHeight1
                End If
                intCurrentX += intColWidth + 2
            Next
        Next
        intCurrentY += intControlHeight2
        Dim XrLine3 As New XRLine
        'draw thick line
        XrLine3.Dpi = 254.0!
        XrLine3.LineStyle = Drawing2D.DashStyle.Solid
        XrLine3.LineWidth = 1
        XrLine3.Location = New System.Drawing.Point(0, intCurrentY + 10)
        XrLine3.Size = New System.Drawing.Size(intWidth, 6)
        XrLine3.Visible = True
        XrPanel1.Controls.AddRange(New XRControl() {XrLine3})
        'draw thick line end
        'day label
        intCurrentY += 10
        intCurrentX = intTableStartX

        For ctr = 1 To 6
            Select Case ctr
                Case 1
                    strX = "Monday"
                    clrDayLabelBackColor = System.Drawing.Color.Transparent
                Case 2
                    strX = "Tuesday"
                    clrDayLabelBackColor = System.Drawing.Color.Transparent
                Case 3
                    strX = "Wednesday"
                    clrDayLabelBackColor = System.Drawing.Color.Transparent
                Case 4
                    strX = "Thursday"
                    clrDayLabelBackColor = System.Drawing.Color.Transparent
                Case 5
                    strX = "Friday"
                    clrDayLabelBackColor = System.Drawing.Color.Transparent
                Case 6
                    strX = "Saturday"
                    clrDayLabelBackColor = System.Drawing.Color.Transparent
            End Select
            intControlHeight1 = MeasureText(strX, fntDayLabel, intHeaderWidth, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
            XrPanel1.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDayLabel, Color.Black, clrDayLabelBackColor, intCurrentX, intCurrentY + 10, intHeaderWidth / 6, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopCenter, True)})
            intCurrentX += intHeaderWidth / 6
        Next
        'day label end
        'insert menu names
        'insert column header end
        intCurrentY = intCurrentY + intControlHeight1
        Dim XrLine2 As New XRLine
        'draw thick line
        XrLine2.Dpi = 254.0!
        XrLine2.LineStyle = Drawing2D.DashStyle.Solid
        XrLine2.LineWidth = 5
        XrLine2.Location = New System.Drawing.Point(0, intCurrentY + 13)
        XrLine2.Size = New System.Drawing.Size(intWidth, 6)
        XrLine2.Visible = True
        XrPanel1.Controls.AddRange(New XRControl() {XrLine2})
        'draw thick line end

        intCurrentY += 25

        'insert restaurants and values
        For Each dtRow As DataRow In dtRestaurants.Rows
            strX = dtRow.Item("codeRestaurant").ToString
            intControlHeight2 = MeasureText(strX, fntDetail1, 50, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
            XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, 0, intCurrentY + 10, 50, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            strX = dtRow.Item("name").ToString
            intControlHeight2 = MeasureText(strX, fntDetail1, 100, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
            XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, 52, intCurrentY + 10, 100, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})

            intCurrentX = intTableStartX

            'insert plan values
            For ctr = 1 To 6
                For Each dtRow2 As DataRow In dtMasterPlan.Rows
                    strX = ""
                    For Each dtrow3 As DataRow In dtMasterPlanValues.Select("coderestaurant=" & dtRow.Item("coderestaurant").ToString & " and codeMasterplan=" & dtRow2.Item("codemasterplan") & " and dayplan=" & ctr)
                        strX = dtrow3.Item("planvalue1").ToString
                    Next
                    intControlHeight1 = MeasureText(strX, fntDetail1, intColWidth, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height()
                    If intControlHeight1 < 5 Then
                        intControlHeight1 = 40
                    End If
                    XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY + 10, intColWidth, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopCenter)})
                    If intControlHeight2 < intControlHeight1 Then
                        intControlHeight2 = intControlHeight1
                    End If
                    intCurrentX += intColWidth + 2
                Next
            Next

            'insert plan values end
            intCurrentY += intControlHeight2
        Next
        'insert restaurants and values end

        'XrPanel1.Controls.AddRange(New XRControl() {fctGetMenuRow(dtMasterPlan, intWidth)})
        'For Each dtRow As DataRow In dtRestaurants.Rows
        '    XrPanel1.Controls.AddRange(New XRControl() {fctGetRestaurantRow(dtRow.Item("coderestaurant"), dtRow.Item("name"), 1)})
        'Next
        Return XrPanel1
    End Function
    Public Function fctGetTextHeight(ByVal strX As String, ByVal dblTextInitialWidth As Double, ByVal fntText As System.Drawing.Font) As Double
        Dim dblTextHeight As Double
        dblTextHeight = MeasureText(strX.ToString, fntText, dblTextInitialWidth, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height()
        Dim arrStrX() As String
        arrStrX = Split(strX, vbLf)
        dblTextHeight = dblTextHeight * (arrStrX.Length + 1)
        Return dblTextHeight
    End Function
    Public Function fctGetTextHeight2(ByVal strX As String, ByVal dblTextInitialWidth As Double, ByVal fntText As System.Drawing.Font) As Double
        Dim dblTextHeight As Double
        dblTextHeight = MeasureText(strX.ToString, fntText, dblTextInitialWidth, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height()
        Return dblTextHeight * 2
    End Function
    Public Function fctGetWrappedText(ByVal strX As String, ByVal dblTextInitialWidth As Double, ByVal fntText As System.Drawing.Font) As String
        Dim strWrappedText, strTemp As String
        Dim ctr As Integer
        'dblTextHeight = MeasureText(strX.ToString, fntText, dblTextInitialWidth, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height()
        'If dblTextHeight = dblTextInitialWidth Then
        '    Return strX
        'End If
        strWrappedText = ""
        strTemp = ""
        'dblTextHeight3 = MeasureText("X", fntText, dblTextInitialWidth, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height()
        'dblTextHeight = dblTextHeight3 * 2
        For ctr = 0 To strX.Length - 1
            If strX.Substring(ctr, 1) = " " Then
                strTemp = strTemp & vbLf & " "
            Else
                strTemp = strTemp & strX.Substring(ctr, 1)
            End If
        Next

        Return strTemp
    End Function
    Public Function fctGetMasterPlanShoppingListPDF(ByVal dtX As DataTable, ByVal strFilePath As String, ByVal intCodeLang As Integer, ByVal strShoppingListName As String) As String
        Dim strMsg As String
        Dim strLabelText As String
        Dim DefaultPageHeight As Double
        Dim DefaultPageWidth As Double
        Dim ReportLeftMargin As Double
        Dim ReportRightMargin As Double
        Dim ReportTopMargin As Double
        Dim ReportBottomMargin As Double
        Dim TableWidth As Double
        Dim intPreviousForLocal As Integer = 0
        Dim strPreviousCountry As String = ""
        Dim strSpace As String = "   "
        Dim XrTable1 As New XRTable




        fntDayLabel = New System.Drawing.Font("Arial", 10, FontStyle.Bold)
        DefaultPageHeight = 3556
        DefaultPageWidth = 2159
        ReportLeftMargin = 254
        ReportRightMargin = 254
        ReportTopMargin = 254
        ReportBottomMargin = 254

        TableWidth = DefaultPageWidth - (ReportLeftMargin + ReportRightMargin) - 1

        Dim fntPageInfo As System.Drawing.Font = New System.Drawing.Font("Arial Narrow", 6.25!, FontStyle.Regular)

        fntDayLabel = New System.Drawing.Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point)
        fntDetail1 = New System.Drawing.Font("Arial", 8, FontStyle.Bold, GraphicsUnit.Point)
        fntDetail2 = New System.Drawing.Font("Arial", 8, FontStyle.Regular, GraphicsUnit.Point)
        strLabelText = ""

        'strMyMessage = OpenConnection()

        Report1 = New XtraReport
        With Report1
            .ReportUnit = ReportUnit.TenthsOfAMillimeter
            'Papersize
            '------------------------------------------------------------------------------------
            .PaperKind = PaperKind.Custom
            .PageWidth = DefaultPageWidth
            .PageHeight = DefaultPageHeight
            '------------------------------------------------------------------------------------
            'Margins
            '------------------------------------------------------------------------------------
            .Margins.Left = ReportLeftMargin
            .Margins.Right = ReportRightMargin
            .Margins.Bottom = ReportBottomMargin
            .Margins.Top = ReportTopMargin

            '------------------------------------------------------------------------------------
            'Orientation
            '------------------------------------------------------------------------------------
            .Landscape = False

            '------------------------------------------------------------------------------------
            'ADD THE SECTION OF THE REPORT 
            '------------------------------------------------------------------------------------
            'ReportHeader
            ReportHeader.Dpi = 254.0!
            ReportHeader.Height = 0
            ReportHeader.Name = "ReportHeader"
            'BottomMargin      
            BottomMargin.Dpi = 254.0! '
            BottomMargin.Height = 0
            BottomMargin.Name = "BottomMargin"
            'Details
            Detail.Dpi = 254.0!
            Detail.Height = DefaultPageHeight - (ReportTopMargin + ReportBottomMargin) - 2 '2664
            Detail.Name = "Detail"
            Detail.Controls.AddRange(New XRControl() {fctMakeXrPanelMPShoppingListPDF(dtX, Detail.Height - 2, DefaultPageWidth - (ReportLeftMargin + ReportRightMargin) - 2, intCodeLang, strShoppingListName)})





            '--- final report -----
            .Bands.Add(ReportHeader)    'REPORT HEADER SECTION
            .Bands.Add(BottomMargin)    'REPORT BOTTOM MARGIN
            .Bands.Add(Detail)          'DETAIL SECTION
            .CreatePdfDocument(strFilePath)
        End With
        Return strMsg
    End Function
    Private Function fctMakeXrPanelMPShoppingListPDF(ByVal dtX As DataTable, ByVal intHeight As Integer, ByVal intWidth As Integer, ByVal intCodeLang As Integer, ByVal strShoppingListName As String) As XRPanel
        Dim cLang As New EgsData.clsEGSLanguage(intCodeLang)
        Dim XrPanel1 As New XRPanel
        Dim clrDayLabelBackColor As New System.Drawing.Color
        Dim str1 As String
        Dim intControlHeight1, intControlHeight2, intCurrentX, intCurrentY, intColWidth, intHeaderWidth, intTableStartX, intControlWidth1, intControlWidth2, intControlWidth3 As Integer
        Dim XrLine1, XrLine3 As New XRLine
        intCurrentY = 0
        intCurrentX = 0
        XrPanel1.Dpi = 254.0!
        XrPanel1.Size = New System.Drawing.Size(intWidth - 3, intHeight - 3)
        XrPanel1.Location = New System.Drawing.Point(0, 0)

        str1 = cLang.GetString(EgsData.clsEGSLanguage.CodeType.Shoppinglist) & " - "
        intControlWidth1 = fctGetTextLength(str1, fntDayLabel)
        intControlHeight1 = MeasureText(str1, fntDayLabel, intControlWidth1, StringFormat.GenericDefault, Me.Padding).Height()
        XrPanel1.Controls.AddRange(New XRControl() {fctMakeXrLabel2(str1, fntDayLabel, Color.Black, Color.Transparent, intCurrentX, intCurrentY, intControlWidth1, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})
        str1 = strShoppingListName
        intControlHeight2 = MeasureText(str1, fntDayLabel, intControlWidth1, StringFormat.GenericDefault, Me.Padding).Height()
        XrPanel1.Controls.AddRange(New XRControl() {fctMakeXrLabel2(str1, fntDayLabel, Color.Black, Color.Transparent, intControlWidth1, intCurrentY, intWidth - intControlWidth1, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})

        If intControlHeight1 > intControlHeight2 Then
            intCurrentY = intControlHeight1 + 15
        Else
            intCurrentY = intControlHeight2 + 15
        End If

        XrLine3.Dpi = 254.0!
        XrLine3.LineStyle = Drawing2D.DashStyle.Solid
        XrLine3.LineWidth = 2
        XrLine3.Location = New System.Drawing.Point(0, intCurrentY + 10)
        XrLine3.Size = New System.Drawing.Size(intWidth, 4)
        XrLine3.Visible = True
        XrPanel1.Controls.AddRange(New XRControl() {XrLine3})

        intCurrentY += 13
        str1 = cLang.GetString(EgsData.clsEGSLanguage.CodeType.Number).ToString
        XrPanel1.Controls.AddRange(New XRControl() {fctMakeXrLabel2(str1, fntDetail1, Color.Black, Color.Transparent, 0, intCurrentY, 250, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})
        str1 = cLang.GetString(clsEGSLanguage.CodeType.Merchandises).ToString
        intControlHeight2 = MeasureText(str1, fntDayLabel, intControlWidth1, StringFormat.GenericDefault, Me.Padding).Height()
        XrPanel1.Controls.AddRange(New XRControl() {fctMakeXrLabel2(str1, fntDetail1, Color.Black, Color.Transparent, 250, intCurrentY, 500, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})
        str1 = cLang.GetString(EgsData.clsEGSLanguage.CodeType.Price).ToString
        XrPanel1.Controls.AddRange(New XRControl() {fctMakeXrLabel2(str1, fntDetail1, Color.Black, Color.Transparent, intWidth - 750, intCurrentY, 200, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})
        str1 = cLang.GetString(EgsData.clsEGSLanguage.CodeType.Net_Qty).ToString
        XrPanel1.Controls.AddRange(New XRControl() {fctMakeXrLabel2(str1, fntDetail1, Color.Black, Color.Transparent, intWidth - 550, intCurrentY, 200, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})
        str1 = cLang.GetString(EgsData.clsEGSLanguage.CodeType.Unit).ToString
        XrPanel1.Controls.AddRange(New XRControl() {fctMakeXrLabel2(str1, fntDetail1, Color.Black, Color.Transparent, intWidth - 350, intCurrentY, 100, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})
        str1 = cLang.GetString(EgsData.clsEGSLanguage.CodeType.Amount).ToString
        XrPanel1.Controls.AddRange(New XRControl() {fctMakeXrLabel2(str1, fntDetail1, Color.Black, Color.Transparent, intWidth - 200, intCurrentY, 200, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})

        intCurrentY += intControlHeight2
        XrLine1.Dpi = 254.0!
        XrLine1.LineStyle = Drawing2D.DashStyle.Solid
        XrLine1.LineWidth = 4
        XrLine1.Location = New System.Drawing.Point(0, intCurrentY + 5)
        XrLine1.Size = New System.Drawing.Size(intWidth, 4)
        XrLine1.Visible = True
        XrPanel1.Controls.AddRange(New XRControl() {XrLine1})

        Dim arrSupplier As New ArrayList
        Dim arrTemp As New ArrayList

        For Each dtRow As DataRow In dtX.Rows
            arrTemp.Add(dtRow.Item("SupplierName"))
        Next


        For Each arrItem As String In arrTemp
            If Not arrSupplier.Contains(arrItem) Then
                arrSupplier.Add(arrItem)
            End If
        Next

        For Each strSupName As String In arrSupplier
            intCurrentY += 10
            XrPanel1.Controls.AddRange(New XRControl() {fctGetSupplierRows(strSupName, dtX, intCurrentY, intWidth)})
        Next

        Return XrPanel1
    End Function

    Private Function fctGetTextLength(ByVal strX As String, ByVal fntFont As System.Drawing.Font) As Integer
        Dim intLength As Integer
        intLength = TextRenderer.MeasureText(strX, fntFont).Width
        intLength = intLength * 1 / 72 * 170
        Return intLength
    End Function

    Private Function fctGetSupplierRows(ByVal strSupplier As String, ByVal dtX As DataTable, ByRef intCurrentY As Integer, ByVal intWidth As Integer) As XRPanel
        Dim XrPanel2 As New XRPanel
        Dim strNumber As String
        Dim intControlHeight, intControlHeightTemp, intPanelHeight, intPanelY As Integer
        Dim XrLine5 As New XRLine
        Dim str1 As String
        XrPanel2.Dpi = 254.0!
        intControlHeight = 0
        intPanelY = 0
        intPanelHeight = 4
        intPanelHeight += MeasureText(strSupplier, fntDetail1, intWidth, StringFormat.GenericDefault, Me.Padding).Height
        intCurrentY += MeasureText(strSupplier, fntDetail1, intWidth, StringFormat.GenericDefault, Me.Padding).Height
        intPanelY += MeasureText(strSupplier, fntDetail1, intWidth, StringFormat.GenericDefault, Me.Padding).Height
        XrPanel2.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strSupplier, fntDetail1, Color.Black, Color.Transparent, 0, 0, intWidth, MeasureText(strSupplier, fntDetail1, 1900, StringFormat.GenericDefault, Me.Padding).Height, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})

        XrPanel2.Location = New System.Drawing.Point(0, intCurrentY)
        XrLine5.Dpi = 254.0!
        XrLine5.LineStyle = Drawing2D.DashStyle.Solid
        XrLine5.LineWidth = 2
        XrLine5.Location = New System.Drawing.Point(0, intPanelY)
        XrLine5.Size = New System.Drawing.Size(intWidth, 4)
        XrLine5.Visible = True
        XrPanel2.Controls.AddRange(New XRControl() {XrLine5})
        intCurrentY += 4
        intPanelY += 4
        intControlHeightTemp = 0

        For Each dtRow As DataRow In dtX.Select("SupplierName='" & strSupplier.ToString & "'")
            strNumber = dtRow.Item("Number")
            intControlHeight = MeasureText(strNumber, fntDetail2, 250, StringFormat.GenericDefault, Me.Padding).Height
            XrPanel2.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strNumber, fntDetail2, Color.Black, Color.Transparent, 0, intPanelY, 200, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})

            str1 = dtRow.Item("Name")
            intControlHeightTemp = MeasureText(str1, fntDayLabel, 500, StringFormat.GenericDefault, Me.Padding).Height()
            XrPanel2.Controls.AddRange(New XRControl() {fctMakeXrLabel2(str1, fntDetail2, Color.Black, Color.Transparent, 250, intPanelY, 500, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})
            str1 = Convert.ToString(Convert.ToInt32(Val(dtRow.Item("Price").ToString) * 100) / 100)
            str1 = dtRow.Item("Symbole").ToString & " " & str1 & "/" & dtRow.Item("Unit").ToString
            XrPanel2.Controls.AddRange(New XRControl() {fctMakeXrLabel2(str1, fntDetail2, Color.Black, Color.Transparent, intWidth - 800, intPanelY, 200, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})
            str1 = dtRow.Item("NetQty").ToString
            str1 = Convert.ToString(Convert.ToInt32(Val(str1) * 100) / 100)
            XrPanel2.Controls.AddRange(New XRControl() {fctMakeXrLabel2(str1, fntDetail2, Color.Black, Color.Transparent, intWidth - 550, intPanelY, 200, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})
            str1 = dtRow.Item("Unit").ToString
            XrPanel2.Controls.AddRange(New XRControl() {fctMakeXrLabel2(str1, fntDetail2, Color.Black, Color.Transparent, intWidth - 350, intPanelY, 100, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})
            str1 = Convert.ToString(Convert.ToInt32(Val(dtRow.Item("Amount").ToString) * 100) / 100)
            str1 = dtRow.Item("Symbole").ToString & " " & str1
            XrPanel2.Controls.AddRange(New XRControl() {fctMakeXrLabel2(str1, fntDetail2, Color.Black, Color.Transparent, intWidth - 200, intPanelY, 200, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})


            If intControlHeightTemp < intControlHeight Then
                intControlHeightTemp = intControlHeight
            End If
            intPanelHeight += intControlHeightTemp
            intCurrentY += intControlHeightTemp
            intPanelY += intControlHeightTemp
        Next

        XrPanel2.Height = intPanelHeight
        XrPanel2.Width = 2159
        Return XrPanel2
    End Function

#Region "For SV"

    Public Function GenerateReportKolbs(ByVal ds As DataSet, ByVal intCodeLang As Integer, ByVal strFilepath As String, ByVal strPicPath As String, ByVal strPaperSize As String) As String
        Dim strMyMessage As String
        Dim strLabelText As String
        Dim ReportLeftMargin As Double
        Dim ReportRightMargin As Double
        Dim ReportTopMargin As Double
        Dim ReportBottomMargin As Double
        Dim TableWidth As Double
        Dim intPreviousForLocal As Integer = 0
        Dim strPreviousCountry As String = ""
        Dim strSpace As String = "   "
        Dim XrTable1 As New XRTable

        ReportTopMargin = 254
        ReportLeftMargin = 125
        ReportRightMargin = 125
        ReportBottomMargin = 125



        Dim fntPageInfo As System.Drawing.Font = New System.Drawing.Font("Arial Narrow", 6.25!, FontStyle.Regular)
        strLabelText = ""

        'strMyMessage = OpenConnection()

        Dim Report = New XtraReport
        With Report
            '.ReportUnit = ReportUnit.TenthsOfAMillimeter
            'Papersize
            '------------------------------------------------------------------------------------
            If strPaperSize.ToLower = "9" Then
                .PaperKind = Printing.PaperKind.A4
            ElseIf strPaperSize.ToLower = "11" Then
                .PaperKind = Printing.PaperKind.A5
            Else
                .PaperKind = Printing.PaperKind.A4
            End If
            .Visible = True
            '.Dpi = 254.0!

            TableWidth = .PageWidth - (ReportLeftMargin + ReportRightMargin) - 2
            '.PageWidth = DefaultPageWidth
            '.PageHeight = DefaultPageHeight
            '------------------------------------------------------------------------------------
            'Margins
            '------------------------------------------------------------------------------------
            .Margins.Left = ReportLeftMargin
            .Margins.Right = ReportRightMargin
            .Margins.Bottom = ReportBottomMargin
            .Margins.Top = ReportTopMargin

            '------------------------------------------------------------------------------------
            'Orientation
            '------------------------------------------------------------------------------------
            .Landscape = False

            '------------------------------------------------------------------------------------
            'ADD THE SECTION OF THE REPORT 
            '------------------------------------------------------------------------------------
            'BottomMargin      

            'Details
            Detail.Dpi = 254.0!
            Detail.Height = .PageHeight - (ReportTopMargin + ReportBottomMargin) - 130 '2664
            Detail.Name = "Detail"


            Dim intYPos As Integer = 0
            Dim intGlobalY As Integer = 0
            intGlobalX = .PageWidth - (ReportLeftMargin + ReportRightMargin)
            intGlobalMultiplier = .PageHeight - (ReportTopMargin + ReportBottomMargin) - 130
            Dim intRowIndex As Integer = 0
            Dim xrAllTable As New XRTable
            With xrAllTable
                .Dpi = 254.0!
                .Width = TableWidth
                '.BorderColor = Color.Black
                '.Borders = DevExpress.XtraPrinting.BorderSide.All
            End With
            Dim intPanelHeight As Integer
            For Each dtRow As DataRow In ds.Tables(3).Rows
                Dim xrRow As New XRTableRow
                xrRow.Dpi = 254.0!
                Dim xrCell As New XRTableCell
                xrCell.Dpi = 254.0!
                xrCell.Height = 2100
                Dim xrPanelRecipe As New XRPanel
                xrPanelRecipe = GetRecipePanel(TableWidth - 2, .PageHeight - (ReportTopMargin + ReportBottomMargin) - 130, ds, intCodeLang, strPicPath, intGlobalY, intRowIndex)
                xrPanelRecipe.Dpi = 254.0!
                xrCell.CanGrow = False
                xrRow.CanGrow = True
                'xrCell.BorderColor = Color.Red
                'xrCell.Borders = DevExpress.XtraPrinting.BorderSide.Bottom

                'xrCell.Height = .PageHeight - (ReportTopMargin + ReportBottomMargin) - 130
                'xrRow.Height = .PageHeight - (ReportTopMargin + ReportBottomMargin) - 130
                xrCell.Controls.AddRange(New XRPanel() {xrPanelRecipe})


                xrRow.Cells.AddRange(New XRTableCell() {xrCell})
                xrAllTable.Padding = New PaddingInfo(0, 0, 0, 0)
                xrAllTable.Rows.AddRange(New XRTableRow() {xrRow})

                'Detail.Controls.AddRange(New XRPanel() {GetRecipePanel(TableWidth - 2, ds, intCodeLang, intGlobalY, intRowIndex)})
                'Dim xrBreak As New XRPageBreak
                'Detail.Controls.AddRange(New XRPageBreak() {xrBreak})
                intRowIndex += 1
                intPanelHeight = xrPanelRecipe.Height
            Next
            xrAllTable.Height = intPanelHeight * ds.Tables(3).Rows.Count
            xrAllTable.CanGrow = False
            Detail.Controls.AddRange(New XRTable() {xrAllTable})

            '--- final report -----
            .Bands.Add(Detail)          'DETAIL SECTION
            .CreatePdfDocument(strFilepath)
        End With

        strMyMessage = "" 'fctExportToPdfFormat(strFileNamePDF)
        Return strMyMessage
    End Function

    Private Function GetKolbsHeader(ByVal intCodeLang As Integer, ByVal strRecipeName As String, ByVal intPanelWidth As Integer, ByRef intYPos As Integer, Optional ByVal strPicturePath As String = "") As XRPanel
        Dim xrHeaderPanel As New XRPanel
        Dim cLang As New EgsData.clsEGSLanguage(intCodeLang)

        Dim intControlHeight As Integer = fctGetTextHeight(strRecipeName, intPanelWidth, fntHeading)
        Dim intControlHeight2 As Integer = fctGetTextHeight(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Preparation), intPanelWidth, fntHeading)


        xrHeaderPanel.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strRecipeName, fntHeading, Color.Black, Color.Transparent, 0, 0, intPanelWidth, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter)})
        xrHeaderPanel.Width = intPanelWidth
        xrHeaderPanel.Dpi = 254.0!

        If strPicturePath <> "" Then
            Try
                Dim xrRecipePicture As New XRPictureBox
                xrRecipePicture.Dpi = 254.0!
                'xrRecipePicture.Width = 500
                'xrRecipePicture.Height = 500
                xrRecipePicture.ImageUrl = strPicturePath
                xrRecipePicture.Sizing = ImageSizeMode.AutoSize

                'xrRecipePicture.Sizing = ImageSizeMode.ZoomImage
                'xrRecipePicture.Size = fctGetPictureDimensions(xrRecipePicture.Width, xrRecipePicture.Height)

                'If Not (fctGetPictureDimensions(xrRecipePicture.Size) = xrRecipePicture.Size) Then


                'End If
                If xrRecipePicture.Height > 500 Or xrRecipePicture.Width > 1800 Then
                    xrRecipePicture.Sizing = ImageSizeMode.ZoomImage
                    xrRecipePicture.Size = fctGetPictureDimensions(xrRecipePicture.Size)
                End If
                xrRecipePicture.Location = New System.Drawing.Point((intPanelWidth / 2) - (xrRecipePicture.Width / 2), intControlHeight + 2)
                xrHeaderPanel.Controls.AddRange(New XRPictureBox() {xrRecipePicture})
                xrHeaderPanel.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Ingredients), fntHeading, Color.Black, Color.Transparent, 0, intControlHeight + 550, intPanelWidth, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter)})
                xrHeaderPanel.Height = intControlHeight + 500 + intControlHeight2 + 50
                intYPos = intControlHeight + 500 + intControlHeight2 + 50 + 5
            Catch ex As Exception
                xrHeaderPanel.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Ingredients), fntHeading, Color.Black, Color.Transparent, 0, intControlHeight + 100, intPanelWidth, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter)})
                xrHeaderPanel.Height = intControlHeight + 50 + intControlHeight2 + 50
                intYPos = intControlHeight + 50 + intControlHeight2 + 50 + 5
            End Try
        Else
            xrHeaderPanel.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Ingredients), fntHeading, Color.Black, Color.Transparent, 0, intControlHeight + 100, intPanelWidth, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter)})
            xrHeaderPanel.Height = intControlHeight + 50 + intControlHeight2 + 50
            intYPos = intControlHeight + 50 + intControlHeight2 + 50 + 5
        End If

        Return xrHeaderPanel
    End Function

    Private Function fctGetPictureDimensions(ByVal sizePictureVal As Size, Optional ByVal intMaxHeight As Integer = 500, Optional ByVal intMaxWidth As Integer = 1800) As Size
        Dim sizePicture As Size
        'sizePictureVal.Height *= 0.8
        'sizePictureVal.Width *= 0.8
        If sizePictureVal.Height <= intMaxHeight And sizePictureVal.Width <= intMaxWidth Then
            sizePicture.Height = sizePictureVal.Height
            sizePicture.Width = sizePictureVal.Width
        ElseIf sizePictureVal.Height > intMaxHeight And sizePictureVal.Width <= intMaxWidth Then
            sizePicture.Height = intMaxHeight
            sizePicture.Width = (intMaxHeight / sizePictureVal.Height) * sizePictureVal.Width
        ElseIf sizePictureVal.Height <= intMaxHeight And sizePictureVal.Width > intMaxWidth Then
            sizePicture.Height = (intMaxWidth / sizePictureVal.Width) * sizePictureVal.Height
            sizePicture.Width = intMaxWidth
        Else
            sizePicture.Height = intMaxHeight
            If sizePictureVal.Width * (intMaxHeight / sizePictureVal.Height) > intMaxWidth Then
                sizePicture.Width = intMaxWidth
                sizePicture.Height = (intMaxWidth / sizePictureVal.Width) * sizePictureVal.Height
            Else
                sizePicture.Width = sizePictureVal.Width * (intMaxHeight / sizePictureVal.Height)
            End If
        End If
        Return sizePicture
    End Function
    Private Function GetIngredientsPanel(ByVal intCodeLang As Integer, ByVal dtDetails As DataTable, ByVal dtRecipeDetails As DataTable, ByVal intTableWidth As Integer, ByRef intYPos As Integer, ByVal intRecipecode As Integer) As XRPanel
        Dim xrIngredientsPanel As New XRPanel
        Dim xrIngredientsTable As New XRTable
        Dim cLang As New EgsData.clsEGSLanguage(intCodeLang)
        Dim intHeading As Integer = 0
        Dim arrTableHeight As New ArrayList
        Dim blnShowSteps As Boolean = False
        Dim blnShowProcedure As Boolean = True
        Dim blnItalic As Boolean = False

        Dim strQty, strUnit, strIngredientName, strProcedure As String
        Dim intItemCode As Integer

        With xrIngredientsPanel
            .Width = intTableWidth - 2
            .Dpi = 254.0!
            .Location = New System.Drawing.Point(0, 0)
        End With
        Dim ctr As Integer = 0
        For Each dtStepRow As DataRow In dtDetails.Select("itemtype=75 AND codemain=" & intRecipecode)
            ctr += 1
        Next
        If ctr > 0 Then
            blnShowSteps = True
        End If

        Dim arrStepName(ctr - 1)() As String

        ctr = 0
        For Each dtStepRow As DataRow In dtDetails.Select("itemtype=75 AND codemain=" & intRecipecode)
            arrStepName(CIntDB(dtStepRow("step")) - 1) = New String() {dtStepRow("itemname"), dtStepRow("step"), 0}
        Next

        For Each dtProcRow As DataRow In dtRecipeDetails.Rows
            If dtProcRow("preparation") <> "" Then
                blnShowProcedure = True
                Exit For
            End If
        Next

        With xrIngredientsTable
            .Borders = DevExpress.XtraPrinting.BorderSide.All
            .BorderWidth = 1
            .Dpi = 254.0!
            '125 + 254 + 500 'intTableWidth - 4
            If blnShowSteps Then
                .Width = intGlobalX - 10 - 125
                .Location = New System.Drawing.Point(124, 0)
            Else
                .Width = intGlobalX - 10
                .Location = New System.Drawing.Point(0, 0)
            End If



            ctr = 1
            If blnShowSteps Then
                For Each strStep() As String In arrStepName
                    Dim intTableHeight As Integer = 0
                    For Each dtRow As DataRow In dtDetails.Select("codemain=" & intRecipecode & "AND step=" & strStep(1) & " AND itemcode<>75")
                        If intHeading = 0 Then
                            intHeading += 1
                            If blnShowProcedure Then
                                .Rows.AddRange(New XRTableRow() {GetIngredientRow(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Quantity), cLang.GetString(EgsData.clsEGSLanguage.CodeType.Unit), cLang.GetString(EgsData.clsEGSLanguage.CodeType.Ingredients), fntBodyBold, cLang.GetString(clsEGSLanguage.CodeType.Procedure), blnShowProcedure, blnShowSteps)})
                                intYPos += GetIngredientRow(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Quantity), cLang.GetString(EgsData.clsEGSLanguage.CodeType.Unit), cLang.GetString(EgsData.clsEGSLanguage.CodeType.Ingredients), fntBodyBold, cLang.GetString(clsEGSLanguage.CodeType.Procedure), blnShowProcedure, blnShowSteps).Height
                            Else
                                .Rows.AddRange(New XRTableRow() {GetIngredientRow(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Quantity), cLang.GetString(EgsData.clsEGSLanguage.CodeType.Unit), cLang.GetString(EgsData.clsEGSLanguage.CodeType.Ingredients), fntBodyBold, , blnShowProcedure, blnShowSteps)})
                                intYPos += GetIngredientRow(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Quantity), cLang.GetString(EgsData.clsEGSLanguage.CodeType.Unit), cLang.GetString(EgsData.clsEGSLanguage.CodeType.Ingredients), fntBodyBold, blnShowProcedure, , blnShowSteps).Height
                            End If

                        Else

                        End If
                        'If dtRow("itemCode").ToString <> "75" Then
                        intItemCode = dtRow("itemCode").ToString
                        strQty = ""
                        strUnit = ""
                        strIngredientName = ""
                        strProcedure = ""
                        blnItalic = False
                        If intItemCode = 0 Then
                            strQty = dtRow("tmpqty").ToString
                            strUnit = dtRow("tmpunit").ToString
                            strIngredientName = dtRow("tmpname").ToString
                            strProcedure = dtRow("tmppreparation").ToString
                        Else
                            For Each dtRow2 As DataRow In dtRecipeDetails.Select("itemcode=" & intItemCode)
                                strQty = dtRow2("quantity").ToString
                                strUnit = dtRow2("itemunit").ToString
                                strIngredientName = dtRow2("ingredient").ToString
                                strProcedure = dtRow2("preparation").ToString

                                Exit For
                            Next
                        End If
                        If dtRow("itemType").ToString = "4" Then
                            strQty = ""
                            strUnit = ""
                        Else
                            Format(Val(strQty), dtRow("itemFormat").ToString)
                        End If
                        If dtRow("itemType").ToString = "8" Then
                            blnItalic = True
                        End If
                        If strQty <> "" Or strUnit <> "" Or strIngredientName <> "" Then
                            .Rows.AddRange(New XRTableRow() {GetIngredientRow(strQty, strUnit, strIngredientName, fntBody, strProcedure, blnShowProcedure, blnShowSteps, blnItalic)})
                            intTableHeight += GetIngredientRow(strQty, strUnit, strIngredientName, fntBody, strProcedure, blnShowProcedure, blnShowSteps).Height
                            intYPos += GetIngredientRow(strQty, strUnit, strIngredientName, fntBody, strProcedure, blnShowProcedure, blnShowSteps).Height
                        End If
                        'End If

                    Next
                    strStep(2) = intTableHeight
                    If ctr < arrStepName.Length Then
                        Dim xrSpacerRow As New XRTableRow
                        Dim xrSpacerCell As New XRTableCell
                        With xrSpacerRow
                            .Dpi = 254.0!
                            .Width = xrIngredientsTable.Width
                            .Height = 30
                            .Borders = DevExpress.XtraPrinting.BorderSide.Right
                            With xrSpacerCell
                                .Dpi = 254.0!
                                .Width = xrIngredientsTable.Width
                                .Height = 30
                                Dim xrSpacerLabel As New XRLabel
                                xrSpacerLabel = fctMakeXrLabel2("", fntBody, Color.Transparent, Color.Transparent, 0, 0, 125, 30)
                                xrSpacerLabel.Borders = DevExpress.XtraPrinting.BorderSide.None
                                .Controls.AddRange(New XRControl() {xrSpacerLabel})
                            End With
                            .Cells.AddRange(New XRTableCell() {xrSpacerCell})
                        End With
                        .Rows.AddRange(New XRTableRow() {xrSpacerRow})
                    End If
                    ctr += 1
                Next
            Else
                Dim intTableHeight As Integer = 0
                For Each dtRow As DataRow In dtDetails.Select("codemain=" & intRecipecode, "position asc")
                    If intHeading = 0 Then
                        intHeading += 1
                        If blnShowProcedure Then
                            .Rows.AddRange(New XRTableRow() {GetIngredientRow(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Quantity), cLang.GetString(EgsData.clsEGSLanguage.CodeType.Unit), cLang.GetString(EgsData.clsEGSLanguage.CodeType.Ingredients), fntBodyBold, cLang.GetString(clsEGSLanguage.CodeType.Procedure), blnShowProcedure, blnShowSteps)})
                            intYPos += GetIngredientRow(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Quantity), cLang.GetString(EgsData.clsEGSLanguage.CodeType.Unit), cLang.GetString(EgsData.clsEGSLanguage.CodeType.Ingredients), fntBodyBold, cLang.GetString(clsEGSLanguage.CodeType.Procedure), blnShowProcedure, blnShowSteps).Height
                        Else
                            .Rows.AddRange(New XRTableRow() {GetIngredientRow(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Quantity), cLang.GetString(EgsData.clsEGSLanguage.CodeType.Unit), cLang.GetString(EgsData.clsEGSLanguage.CodeType.Ingredients), fntBodyBold, , blnShowProcedure, blnShowSteps)})
                            intYPos += GetIngredientRow(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Quantity), cLang.GetString(EgsData.clsEGSLanguage.CodeType.Unit), cLang.GetString(EgsData.clsEGSLanguage.CodeType.Ingredients), fntBodyBold, , blnShowProcedure, blnShowSteps).Height
                        End If

                    Else

                    End If
                    If dtRow("itemCode").ToString <> "75" Then
                        intItemCode = dtRow("itemCode").ToString
                        strQty = ""
                        strUnit = ""
                        strIngredientName = ""
                        strProcedure = ""
                        If intItemCode = 0 Then
                            strQty = dtRow("tmpqty").ToString
                            strUnit = dtRow("tmpunit").ToString
                            strIngredientName = dtRow("tmpname").ToString
                            strProcedure = dtRow("tmppreparation").ToString
                        Else
                            For Each dtRow2 As DataRow In dtRecipeDetails.Select("itemcode=" & intItemCode)
                                strQty = dtRow2("quantity").ToString
                                strUnit = dtRow2("itemunit").ToString
                                strIngredientName = dtRow2("ingredient").ToString
                                strProcedure = dtRow2("preparation").ToString

                                Exit For
                            Next
                        End If

                        If dtRow("itemType").ToString = "4" Then
                            strQty = ""
                            strUnit = ""
                        Else
                            strQty = Format(Val(strQty), dtRow("itemFormat").ToString) 'strQty.Format(dtRow("itemFormat").ToString)
                        End If
                        If dtRow("itemType").ToString = "8" Then
                            blnItalic = True
                        End If
                        If strQty <> "" Or strUnit <> "" Or strIngredientName <> "" Then
                            .Rows.AddRange(New XRTableRow() {GetIngredientRow(strQty, strUnit, strIngredientName, fntBody, , blnShowProcedure, blnShowSteps, blnItalic)})
                            intTableHeight += GetIngredientRow(strQty, strUnit, strIngredientName, fntBody).Height
                            intYPos += GetIngredientRow(strQty, strUnit, strIngredientName, fntBody).Height
                        End If
                    End If

                Next
            End If


            'arrTableHeight.Clear()
            'arrTableHeight.Add(intTableHeight)
            'xrStepLabel.Height = intTableHeight
            'xrStep.Height = intTableHeight
            'xrStepCell.Height = intTableHeight 
        End With



        Dim intX As Integer = 0
        Dim xrIngredientsStepTable As New XRTable
        If blnShowSteps Then

            With xrIngredientsStepTable
                .Dpi = 254.0!
                .Width = 125
                .Location = New System.Drawing.Point(0, 0)
                .Borders = DevExpress.XtraPrinting.BorderSide.All
                .BorderWidth = 1

                'If intHeading = 0 Then
                Dim xrStepHead As New XRTableRow
                With xrStepHead
                    Dim xrStepHeadCell As New XRTableCell
                    With xrStepHeadCell
                        .Dpi = 254.0!
                        .Width = 125
                        Dim intControlHeight As Double = MeasureText("Step", fntBodyBold, 125 / 2.7, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height
                        intControlHeight *= 2.7

                        .Controls.Add(fctMakeXrLabel2("Step", fntBodyBold, Color.Black, Color.Transparent, 0, 0, 125, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
                    End With
                    .Cells.Add(xrStepHeadCell)

                End With

                .Rows.Add(xrStepHead)
                'End If
                ctr = 1
                For Each strStep() As String In arrStepName
                    Dim xrStep As New XRTableRow
                    With xrStep
                        Dim xrStepCell As New XRTableCell
                        With xrStepCell
                            .Dpi = 254.0!
                            .Width = 125
                            .Controls.Add(fctMakeXrLabel2(strStep(0), fntBody, Color.Black, Color.Transparent, 0, 0, 125, strStep(2), DevExpress.XtraPrinting.TextAlignment.MiddleCenter, True))
                        End With
                        .Cells.Add(xrStepCell)
                    End With
                    .Rows.Add(xrStep)
                    If ctr < arrStepName.Length Then
                        Dim xrSpacerRow As New XRTableRow
                        Dim xrSpacerCell As New XRTableCell
                        With xrSpacerRow
                            .Dpi = 254.0!
                            .Width = intGlobalX - 10
                            .Height = 30
                            .Borders = DevExpress.XtraPrinting.BorderSide.Left
                            With xrSpacerCell
                                .Dpi = 254.0!
                                .Width = intGlobalX - 10
                                .Height = 30
                                .Controls.AddRange(New XRControl() {fctMakeXrLabel2("", fntBody, Color.Transparent, Color.Transparent, 0, 0, 125, 30)})
                            End With
                            .Cells.AddRange(New XRTableCell() {xrSpacerCell})
                        End With
                        .Rows.AddRange(New XRTableRow() {xrSpacerRow})
                    End If
                    ctr += 1
                Next

            End With
        End If


        'Dim xrProcedureTable As New XRTable
        'With xrProcedureTable
        '    .Dpi = 254.0!
        '    Dim intWidth As Integer = intGlobalX - (122 + 254 + 125 + 500) - 5
        '    .Width = intWidth
        '    .Location = New System.Drawing.Point((118 + 254 + 125 + 500), 0)
        '    .Borders = DevExpress.XtraPrinting.BorderSide.All
        '    .BorderWidth = 1

        '    'If intHeading = 0 Then
        '    Dim xrProcedureHead As New XRTableRow
        '    With xrProcedureHead
        '        Dim xrProcedureHeadCell As New XRTableCell
        '        With xrProcedureHeadCell
        '            .Width = intWidth
        '            Dim intControlHeight As Double = MeasureText(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Procedure), fntBodyBold, 254 / 2.7, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height
        '            intControlHeight *= 2.7
        '            .Controls.Add(fctMakeXrLabel2(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Procedure), fntBodyBold, Color.Black, Color.Transparent, 0, 0, intWidth, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
        '        End With
        '        .Cells.Add(xrProcedureHeadCell)
        '    End With

        '    .Rows.Add(xrProcedureHead)
        '    'End If

        '    Dim ctr As Integer = 0
        '    Dim xrProcedure As New XRTableRow
        '    With xrProcedure
        '        Dim xrProcedureCell As New XRTableCell
        '        With xrProcedureCell
        '            .Width = intGlobalX - (122 + 254 + 125 + 500)
        '            .Controls.Add(fctMakeXrLabel2("#", fntBody, Color.Black, Color.Transparent, 0, 0, intWidth, arrTableHeight(ctr), DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
        '        End With
        '        .Cells.Add(xrProcedureCell)
        '    End With
        '    .Rows.Add(xrProcedure)
        'End With

        'Dim intTableHeight1 As Integer = 0
        'For Each intA As Integer In arrTableHeight
        '    intTableHeight1 += intA
        'Next

        intYPos += xrIngredientsTable.Height

        xrIngredientsPanel.Controls.Add(xrIngredientsTable)
        'xrIngredientsPanel.Controls.Add(xrProcedureTable)
        If blnShowSteps Then xrIngredientsPanel.Controls.Add(xrIngredientsStepTable)
        Return xrIngredientsPanel
    End Function
    Private Function GetIngredientRow(ByVal strQuantity As String, ByVal strUnit As String, ByVal strIngredientName As String, ByVal fntFont As System.Drawing.Font, Optional ByVal strProcedure As String = "", Optional ByVal bShowProcedure As Boolean = False, Optional ByVal bShowStep As Boolean = False, Optional ByVal bItalic As Boolean = False) As XRTableRow
        Dim xrIngRow As New XRTableRow
        Dim intControlHeight As Integer
        Dim intProcedureWidth As Integer
        'bShowProcedure = False

        Dim intIngredientsCellWidth As Integer

        If bShowProcedure Then
            intIngredientsCellWidth = 500
            If bShowStep Then
                intProcedureWidth = intGlobalX - 500 - 254 - 125 - 125
            Else
                intProcedureWidth = intGlobalX - 500 - 254 - 125
            End If
        ElseIf bShowStep Then
            intIngredientsCellWidth = intGlobalX - 125 - 254 - 125
        Else
            intIngredientsCellWidth = intGlobalX - 125 - 254
        End If

        If intControlHeight = 60 Then intControlHeight += 5
        intControlHeight = MeasureText(strQuantity, fntFont, 254 / 2.7, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height
        If MeasureText(strUnit, fntFont, 125 / 2.7, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height > intControlHeight Then intControlHeight = MeasureText(strUnit, fntFont, 125 / 2.7, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height
        If MeasureText(strIngredientName, fntFont, intIngredientsCellWidth / 2.7, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height > intControlHeight Then intControlHeight = MeasureText(strIngredientName, fntFont, intIngredientsCellWidth / 2.7, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height
        If MeasureText(strProcedure, fntFont, intProcedureWidth / 2.7, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height > intControlHeight Then intControlHeight = MeasureText(strProcedure, fntFont, intProcedureWidth / 2.7, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height
        intControlHeight *= 2.7
        'intControlHeight = MeasureText(strQuantity, fntFont, 254, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height
        'If MeasureText(strUnit, fntFont, 125, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height > intControlHeight Then intControlHeight = MeasureText(strUnit, fntFont, 125, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height
        'If MeasureText(strIngredientName, fntFont, 500, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height > intControlHeight Then intControlHeight = MeasureText(strIngredientName, fntFont, 500, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height

        With xrIngRow
            .Dpi = 254.0!
            '.Borders = DevExpress.XtraPrinting.BorderSide.None
            Dim xrQuantityCell As New XRTableCell
            With xrQuantityCell
                .Width = 254
                '.Height = intControlHeight
                .Controls.Add(fctMakeXrLabel2(strQuantity, fntFont, Color.Black, Color.Transparent, 0, 0, 254, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter, False, True, True))
            End With
            .Cells.Add(xrQuantityCell)
            Dim xrUnitCell As New XRTableCell
            With xrUnitCell
                .Width = 125
                '.Height = intControlHeight
                .Controls.Add(fctMakeXrLabel2(strUnit, fntFont, Color.Black, Color.Transparent, 0, 0, 125, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter, False, True, True))
            End With
            .Cells.Add(xrUnitCell)
            Dim xrIngredientCell As New XRTableCell
            With xrIngredientCell
                .Width = intIngredientsCellWidth
                '.Height = intControlHeight
                If bItalic Then
                    .Controls.Add(fctMakeXrLabel2(strIngredientName, fntBodyItalic, Color.Black, Color.Transparent, 0, 0, intIngredientsCellWidth, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter, False, True, True))
                Else
                    .Controls.Add(fctMakeXrLabel2(strIngredientName, fntFont, Color.Black, Color.Transparent, 0, 0, intIngredientsCellWidth, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter, False, True, True))
                End If

            End With
            'If xrIngredientCell.Height > xrUnitCell.Height And xrIngredientCell.Height > xrQuantityCell.Height Then
            '    xrUnitCell.Height = xrIngredientCell.Height
            '    xrQuantityCell.Height = xrIngredientCell.Height
            'End If
            .Cells.Add(xrIngredientCell)
            If bShowProcedure Then
                Dim xrProcedureCell As New XRTableCell
                With xrProcedureCell
                    .Width = intProcedureWidth
                    '.Height = intControlHeight
                    .Controls.Add(fctMakeXrLabel2(strProcedure, fntFont, Color.Black, Color.Transparent, 0, 0, intProcedureWidth, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter, False, True, True))
                End With
                'If xrIngredientCell.Height > xrUnitCell.Height And xrIngredientCell.Height > xrQuantityCell.Height Then
                '    xrUnitCell.Height = xrIngredientCell.Height
                '    xrQuantityCell.Height = xrIngredientCell.Height
                'End If
                .Cells.Add(xrProcedureCell)
            End If
            .Height = intControlHeight
        End With

        Return xrIngRow
    End Function

    Private Function GetRecipePanel(ByVal intPanelWidth As Integer, ByVal intPanelHeight As Integer, ByVal ds As DataSet, ByVal intCodeLang As Integer, ByVal strPicPath As String, ByRef intGlobalY As Integer, ByVal intRowIndex As Integer) As XRPanel
        Dim dtMain As DataTable = ds.Tables(3)
        Dim dtIngDetails As DataTable = ds.Tables(4)
        Dim dt2 As DataTable = ds.Tables(2)
        Dim dt3 As DataTable = ds.Tables(3)
        Dim dtRecipeDetail As DataTable = ds.Tables(1)

        Dim xrRecipePanel As New XRPanel
        Dim intYPos As Integer = 0
        intYPos = 0
        Dim TableWidth As Integer = intPanelWidth - 2
        Dim strImgPath() As String
        Dim strImgFileName As String = ""
        Dim intRecipeCode As Integer = dtMain.Rows(intRowIndex)("code")
        For Each dtrow As DataRow In dtRecipeDetail.Select("code=" & intRecipeCode)
            strImgPath = dtrow("pix").ToString.Split(";")
            For Each strPicName As String In strImgPath
                Dim strPicTemp() As String = strPicName.Split("_")
                Try
                    If strPicTemp(1).Substring(0, 1) = CStrDB(dtrow("defaultpicture")) Or CStrDB(dtrow("defaultpicture")) = "" Then
                        If strPicName <> "" Then
                            strImgFileName = strPicName
                            Exit For
                        End If
                    End If
                Catch ex As Exception
                    strImgFileName = strPicName
                    Exit For
                End Try

            Next
            Exit For
        Next
        If strImgFileName <> "" Then strImgFileName = strPicPath & strImgFileName


        'intGlobalY = (intGlobalMultiplier - 500) * intRowIndex
        'If intGlobalY = (intGlobalMultiplier - 10) Then intGlobalY -= 900
        With xrRecipePanel
            .Dpi = 254.0!
            .Width = intPanelWidth
            .Height = intPanelHeight
            .Location = New System.Drawing.Point(0, 0)
            .Borders = DevExpress.XtraPrinting.BorderSide.None
            .CanGrow = True
            Dim xrTableAll As New XRTable
            Dim xrTableHeader As New XRTableRow
            Dim xrTableIngredients As New XRTableRow
            Dim xrTablePrep As New XRTableRow

            Dim xrTableHeaderCell As New XRTableCell
            Dim xrTableIngredientsCell As New XRTableCell
            Dim xrTablePrepCell As New XRTableCell

            'strImgPath = 

            xrTableHeaderCell.Controls.Add(GetKolbsHeader(intCodeLang, dtMain.Rows(intRowIndex)("name"), TableWidth, intYPos, strImgFileName))
            xrTableIngredientsCell.Controls.Add(GetIngredientsPanel(intCodeLang, dtIngDetails, dtRecipeDetail, TableWidth, intYPos, intRecipeCode))
            xrTablePrepCell.Controls.Add(GetPreparationPanel(intCodeLang, dtRecipeDetail, TableWidth, intRecipeCode, intYPos))

            xrTableHeader.Cells.Add(xrTableHeaderCell)
            xrTableIngredients.Cells.Add(xrTableIngredientsCell)
            xrTablePrep.Cells.Add(xrTablePrepCell)

            xrTableAll.Dpi = 254.0!

            Dim e As Integer = xrTableAll.Height
            Dim s As Integer = xrTableHeader.Height
            Dim w As Integer = xrTableIngredients.Height
            Dim r As Integer = xrTablePrep.Height

            xrTableAll.Width = TableWidth
            xrTableAll.Rows.Add(xrTableHeader)
            xrTableAll.Rows.Add(xrTableIngredients)
            xrTableAll.Rows.Add(xrTablePrep)
            '.Controls.AddRange(New XRPanel() {GetKolbsHeader(intCodeLang, dtMain(intRowIndex)("name"), TableWidth, intYPos, strImgPath)})
            '.Controls.AddRange(New XRPanel() {GetIngredientsPanel(intCodeLang, dtDetails, TableWidth, intYPos, intRecipeCode)})
            'Dim intYPos1 As Integer
            ''intypos1 = 
            '.Controls.AddRange(New XRPanel() {GetPreparationPanel(intCodeLang, dtDetails, TableWidth, intRecipeCode, intYPos)})

            Dim intYPosTemp As Integer = intYPos * 0.8
            For ctr As Integer = 1 To 10
                If intYPosTemp < ctr * intGlobalMultiplier Then
                    .Height = ctr * intGlobalMultiplier - intYPos - 300
                    Exit For
                End If
            Next
            .Controls.Add(xrTableAll)
        End With


        Return xrRecipePanel
    End Function

    Private Function GetPreparationPanel(ByVal intCodeLang As Integer, ByVal dtDetails As DataTable, ByVal intTableWidth As Integer, ByVal intRecipeCode As Integer, ByRef intYPos As Integer) As XRPanel
        Dim cLang As New EgsData.clsEGSLanguage(intCodeLang)
        Dim intControlHeight, intControlHeight1 As Integer
        Dim xrPrepPanel As New XRPanel
        Dim blnTemplate As Boolean = False


        With xrPrepPanel
            .Dpi = 254.0!
            .Width = intTableWidth
            .Location = New System.Drawing.Point(0, 50)
            '.Borders = DevExpress.XtraPrinting.BorderSide.All
            Dim strTitle() As String
            Dim strNotes() As String
            For Each dtRow As DataRow In dtDetails.Select("code=" & intRecipeCode)
                If Val(dtRow("templatecode").ToString) > 0 Then
                    blnTemplate = True
                    strTitle = dtRow("noteheader").ToString.Split("¤")
                    strNotes = dtRow("note").ToString.Split("¤")
                End If
                Exit For
            Next
            If blnTemplate Then
                Dim intCurrentY As Integer = 0

                Dim ctr As Integer = 0
                For ctr = 0 To strTitle.Length - 1
                    intControlHeight1 = MeasureText(strTitle(ctr), fntHeading, intTableWidth - 2, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height
                    .Controls.Add(fctMakeXrLabel2(strTitle(ctr), fntHeading, Color.Black, Color.Transparent, 0, intCurrentY, intTableWidth - 2, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopCenter))
                    intControlHeight = MeasureText(strNotes(ctr), fntBody, intTableWidth - 2, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height
                    .Controls.Add(fctMakeXrLabel2(strNotes(ctr), fntBody, Color.Black, Color.Transparent, 0, intControlHeight1 + intCurrentY, intTableWidth - 2, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter, True, True))
                    intCurrentY += intControlHeight + intControlHeight1 + 20
                Next
                intYPos += intCurrentY
            Else
                Dim strTitleHead As String = vbLf & cLang.GetString(EgsData.clsEGSLanguage.CodeType.Preparation)
                intControlHeight1 = MeasureText(strTitleHead, fntHeading, intTableWidth - 2, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height
                .Controls.Add(fctMakeXrLabel2(strTitleHead, fntHeading, Color.Black, Color.Transparent, 0, 0, intTableWidth - 2, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopCenter))
                For Each dtRow As DataRow In dtDetails.Select("code=" & intRecipeCode)
                    Dim strNote As String = dtRow("note").ToString
                    intControlHeight = MeasureText(strNote, fntBody, intTableWidth - 2, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height
                    .Controls.Add(fctMakeXrLabel2(strNote, fntBody, Color.Black, Color.Transparent, 0, intControlHeight1, intTableWidth - 2, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter, True, True))
                    Exit For
                Next
                intYPos += (intControlHeight + intControlHeight1)
            End If

        End With

        Return xrPrepPanel
    End Function

    Public Function GenerateSVReport(ByVal ds As DataSet, ByVal intCodeLang As Integer, ByVal strFilepath As String, ByVal strPicPath As String, ByVal strPaperSize As String, ByVal intCodeSite As Integer) As String
        Dim strMyMessage As String
        Dim strLabelText As String
        Dim ReportLeftMargin As Double
        Dim ReportRightMargin As Double
        Dim ReportTopMargin As Double
        Dim ReportBottomMargin As Double
        Dim TableWidth As Double
        Dim intPreviousForLocal As Integer = 0
        Dim strPreviousCountry As String = ""
        Dim strSpace As String = "   "
        Dim XrTable1 As New XRTable

        ReportTopMargin = 100
        ReportLeftMargin = 100
        ReportRightMargin = 100
        ReportBottomMargin = 100
        Dim fntPageInfo As System.Drawing.Font = New System.Drawing.Font("Arial Narrow", 6.25!, FontStyle.Regular)
        strLabelText = ""

        'strMyMessage = OpenConnection()

        Dim Report = New XtraReport
        With Report
            .ReportUnit = ReportUnit.HundredthsOfAnInch
            'Papersize
            '------------------------------------------------------------------------------------
            If strPaperSize.ToLower = "9" Then
                .PaperKind = Printing.PaperKind.A4
            ElseIf strPaperSize.ToLower = "11" Then
                .PaperKind = Printing.PaperKind.A5
            Else
                .PaperKind = Printing.PaperKind.A4
            End If
            .Visible = True
            '.Dpi = 254.0!

            'TableWidth = .PageWidth - (ReportLeftMargin + ReportRightMargin) - 2
            '.PageWidth = DefaultPageWidth
            '.PageHeight = DefaultPageHeight
            '------------------------------------------------------------------------------------
            'Margins
            '------------------------------------------------------------------------------------
            .Margins.Left = ReportLeftMargin
            .Margins.Right = ReportRightMargin
            .Margins.Bottom = ReportBottomMargin
            .Margins.Top = ReportTopMargin

            '------------------------------------------------------------------------------------
            'Orientation
            '------------------------------------------------------------------------------------
            .Landscape = False

            '------------------------------------------------------------------------------------
            'ADD THE SECTION OF THE REPORT 
            '------------------------------------------------------------------------------------
            'BottomMargin      

            'Details
            'Detail.Dpi = 254.0!
            Detail.Height = .PageHeight - (ReportTopMargin + ReportBottomMargin) ' - 130 '2664
            Detail.Name = "Detail"


            Dim intYPos As Integer = 0

            Dim intRowIndex As Integer = 0

            Dim intPanelHeight As Integer
            For Each dtRow As DataRow In ds.Tables(3).Rows
                Dim xrRow As New XRTableRow
                'xrRow.Dpi = 254.0!
                Dim xrCell As New XRTableCell
                'xrCell.Dpi = 254.0!
                xrCell.Height = 2100
                Dim xrPanelRecipe As New XRPanel
                xrPanelRecipe = GetRecipePanelSV(.pagewidth - (ReportLeftMargin + ReportRightMargin), .PageHeight - (ReportTopMargin + ReportBottomMargin), ds, intCodeLang, strPicPath, 0, intRowIndex, intCodeSite, CInt(dtRow("Code")), CInt(strPaperSize))
                Detail.Controls.AddRange(New XRPanel() {xrPanelRecipe})
                'xrPanelRecipe.Dpi = 254.0!
                'xrCell.CanGrow = False
                'xrRow.CanGrow = True
                ''xrCell.BorderColor = Color.Red
                ''xrCell.Borders = DevExpress.XtraPrinting.BorderSide.Bottom

                ''xrCell.Height = .PageHeight - (ReportTopMargin + ReportBottomMargin) - 130
                ''xrRow.Height = .PageHeight - (ReportTopMargin + ReportBottomMargin) - 130
                'xrCell.Controls.AddRange(New XRPanel() {xrPanelRecipe})


                'xrRow.Cells.AddRange(New XRTableCell() {xrCell})
                'xrAllTable.Padding = New PaddingInfo(0, 0, 0, 0)
                'xrAllTable.Rows.AddRange(New XRTableRow() {xrRow})

                ''Detail.Controls.AddRange(New XRPanel() {GetRecipePanel(TableWidth - 2, ds, intCodeLang, intGlobalY, intRowIndex)})
                ''Dim xrBreak As New XRPageBreak
                ''Detail.Controls.AddRange(New XRPageBreak() {xrBreak})
                intRowIndex += 1
                'intPanelHeight = xrPanelRecipe.Height
            Next
            'xrAllTable.Height = intPanelHeight * ds.Tables(3).Rows.Count
            'xrAllTable.CanGrow = False
            'Detail.Controls.AddRange(New XRTable() {xrAllTable})

            '--- final report -----
            .Bands.Add(Detail)          'DETAIL SECTION
            .CreatePdfDocument(strFilepath)
        End With

        strMyMessage = "" 'fctExportToPdfFormat(strFileNamePDF)
        Return strMyMessage
    End Function
    Private Function GetRecipePanelSV(ByVal intPanelWidth As Integer, ByVal intPanelHeight As Integer, ByVal ds As DataSet, ByVal intCodeLang As Integer, ByVal strPicPath As String, ByRef intGlobalY As Integer, ByVal intRowIndex As Integer, ByVal intCodeSite As Integer, ByVal intCodeListe As Integer, Optional ByVal intPaperSize As Integer = 11) As XRPanel
        Dim dtMain As DataTable = ds.Tables(0)
        Dim dtIngDetails As DataTable = ds.Tables(4)
        Dim dtAllergens As DataTable = ds.Tables(2)
        Dim dt3 As DataTable = ds.Tables(3)
        Dim dtRecipeDetail As DataTable = ds.Tables(1)

        Dim xrRecipePanel As New XRPanel
        Dim intYPos As Integer = 0
        intYPos = 0
        intCurrentY = 0
        Dim intCodeTrans As Integer = CIntDB(dtMain.Rows(0)("codeTrans").ToString())
        With xrRecipePanel
            Dim fntFoot As System.Drawing.Font = New System.Drawing.Font("Arial", 10, FontStyle.Bold)
            Dim fntHead As System.Drawing.Font = New System.Drawing.Font("Arial", 12, FontStyle.Bold Or FontStyle.Underline)
            Dim fntHead2 As System.Drawing.Font = New System.Drawing.Font("Arial", 10, FontStyle.Regular)
            Dim fntDetails As System.Drawing.Font = New System.Drawing.Font("Calibri", 10, FontStyle.Bold)

            If intPaperSize = 11 Then
                fntFoot = New System.Drawing.Font("Arial", 7, FontStyle.Bold)
                fntHead = New System.Drawing.Font("Arial", 8, FontStyle.Bold Or FontStyle.Underline)
                fntHead2 = New System.Drawing.Font("Arial", 7, FontStyle.Regular)
                fntDetails = New System.Drawing.Font("Calibri", 7, FontStyle.Bold)
            End If
            .Location = New Point(0, intRowIndex * intPanelHeight)
            '---Heading

            Dim strX As String = ""
            For Each dtRow As DataRow In dt3.Select("Code=" & intCodeListe)
                strX = dtRow("name").ToString()
            Next
            strX &= " " & Format(DateTime.Now.Day, "00") & "." & Format(DateTime.Now.Month, "00") & "." & DateTime.Now.Year
            Dim intTextLength As Integer = MeasureText(strX, fntHead, intPanelWidth, sf1, Me.Padding).Width
            Dim intTextHeight As Integer = MeasureText(strX, fntHead, intPanelWidth, sf1, Me.Padding).Height
            .CanGrow = False
            .Height = intPanelHeight
            .Width = intPanelWidth
            Dim intXHeader As Integer = (xrRecipePanel.Width / 2) - (intTextLength / 2)
            '.Borders = DevExpress.XtraPrinting.BorderSide.All

            .Controls.AddRange(New XRLabel() {fctMakeXrLabelNoDpi(strX, fntHead, Color.Black, Color.Transparent, intXHeader, 0, intTextLength, intTextHeight)})
            intCurrentY += intTextHeight + 2
            '---


            Dim intTextYPos As Integer = MeasureText("X", fntDetails, intPanelWidth, sf1, Me.Padding).Height

            '---PictureBox
            If strPicPath <> "" Then
                If System.IO.File.Exists(strPicPath) Then
                    Try
                        Dim xrRecipePicture As New XRPictureBox
                        'xrRecipePicture.Dpi = 254.0!
                        If intPaperSize = 9 Then
                            xrRecipePicture.Width = 472
                            xrRecipePicture.Height = 354
                        Else
                            xrRecipePicture.Width = 236
                            xrRecipePicture.Height = 127
                        End If

                        '.Borders = DevExpress.XtraPrinting.BorderSide.All
                        xrRecipePicture.ImageUrl = strPicPath
                        xrRecipePicture.Sizing = ImageSizeMode.StretchImage

                        'xrRecipePicture.Sizing = ImageSizeMode.ZoomImage
                        'xrRecipePicture.Size = fctGetPictureDimensions(xrRecipePicture.Width, xrRecipePicture.Height)

                        'If Not (fctGetPictureDimensions(xrRecipePicture.Size) = xrRecipePicture.Size) Then


                        'End If
                        'If xrRecipePicture.Height > 500 Or xrRecipePicture.Width > 1800 Then
                        '    xrRecipePicture.Sizing = ImageSizeMode.ZoomImage
                        '    xrRecipePicture.Size = fctGetPictureDimensions(xrRecipePicture.Size)
                        'End If
                        xrRecipePicture.Location = New System.Drawing.Point((intPanelWidth / 2) - (xrRecipePicture.Width / 2), intCurrentY + 2)
                        .Controls.AddRange(New XRPictureBox() {xrRecipePicture})

                        '.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Ingredients), fntHeading, Color.Black, Color.Transparent, 0, intControlHeight + 550, intPanelWidth, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter)})
                        '.Height = intControlHeight + 500 + intControlHeight2 + 50
                        'intYPos = intControlHeight + 500 + intControlHeight2 + 50 + 5
                    Catch ex As Exception
                        '.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Ingredients), fntHeading, Color.Black, Color.Transparent, 0, intControlHeight + 100, intPanelWidth, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter)})
                        '.Height = intControlHeight + 50 + intControlHeight2 + 50
                        'intYPos = intControlHeight + 50 + intControlHeight2 + 50 + 5
                    End Try
                Else
                    '.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Ingredients), fntHeading, Color.Black, Color.Transparent, 0, intControlHeight + 100, intPanelWidth, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter)})
                    '.Height = intControlHeight + 50 + intControlHeight2 + 50
                    'intYPos = intControlHeight + 50 + intControlHeight2 + 50 + 5
                End If

            Else
                '.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Ingredients), fntHeading, Color.Black, Color.Transparent, 0, intControlHeight + 100, intPanelWidth, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter)})
                '.Height = intControlHeight + 50 + intControlHeight2 + 50
                'intYPos = intControlHeight + 50 + intControlHeight2 + 50 + 5
            End If
            If intPaperSize = 9 Then
                intCurrentY += 354
            Else
                intCurrentY += 127
            End If
            '---

            '---Details
            Dim xrDetailsTable As New XRTable
            With xrDetailsTable
                .Borders = DevExpress.XtraPrinting.BorderSide.All
                .Width = xrRecipePanel.Width
                .Location = New Point(0, intCurrentY)
                Dim cLang As New EgsData.clsEGSLanguage(intCodeTrans)

                '---Ingerdients
                Dim xrRowIngredientsHead As New XRTableRow
                xrRowIngredientsHead.Borders = DevExpress.XtraPrinting.BorderSide.All
                Dim xrCellIngredientsHead As New XRTableCell
                With xrCellIngredientsHead
                    '.Borders = DevExpress.XtraPrinting.BorderSide.None
                    'Dim fntHead As Font = New Font("Arial", 12, FontStyle.Bold Or FontStyle.Underline)
                    .Width = intPanelWidth
                    .Controls.AddRange(New XRLabel() {fctMakeXrLabelNoDpi(cLang.GetString(clsEGSLanguage.CodeType.Ingredients) & ":", fntHead, Color.Black, Color.Transparent, 0, intTextHeight, intTextLength, intTextHeight)})
                End With
                xrRowIngredientsHead.Controls.AddRange(New XRTableCell() {xrCellIngredientsHead})
                .Controls.AddRange(New XRTableRow() {xrRowIngredientsHead})
                Dim xrRowIngredients As New XRTableRow
                xrRowIngredients.Borders = DevExpress.XtraPrinting.BorderSide.All
                Dim xrCellIngredients As New XRTableCell
                With xrCellIngredients
                    '.Borders = DevExpress.XtraPrinting.BorderSide.None
                    'Dim fntHead As Font = New Font("Arial", 12, FontStyle.Bold Or FontStyle.Underline)
                    Dim strIngredients As String = ""
                    For Each dtRow As DataRow In dtIngDetails.Select("CodeMain=" & intCodeListe)
                        strIngredients &= dtRow("itemname").ToString() & ", "
                    Next

                    If strIngredients.Length > 2 Then
                        strIngredients = strIngredients.Substring(0, strIngredients.Length - 2)
                    End If

                    intTextHeight = MeasureText(strIngredients, fntDetails, intPanelWidth, sf1, Me.Padding).Height

                    .Width = intPanelWidth
                    .Controls.AddRange(New XRLabel() {fctMakeXrLabelNoDpi(strIngredients, fntDetails, Color.Black, Color.Transparent, 0, intTextYPos, intPanelWidth, intTextHeight)})
                End With
                xrRowIngredients.Controls.AddRange(New XRTableCell() {xrCellIngredients})
                .Controls.AddRange(New XRTableRow() {xrRowIngredients})
                '---

                '---Allergens
                Dim xrRowAllergensHead As New XRTableRow
                xrRowAllergensHead.Borders = DevExpress.XtraPrinting.BorderSide.All
                Dim xrCellAllergensHead As New XRTableCell
                With xrCellAllergensHead
                    '.Borders = DevExpress.XtraPrinting.BorderSide.None
                    'Dim fntHead As Font = New Font("Arial", 12, FontStyle.Bold Or FontStyle.Underline)
                    intTextHeight = MeasureText(cLang.GetString(clsEGSLanguage.CodeType.Allergens) & ":", fntHead, intPanelWidth, sf1, Me.Padding).Height
                    .Width = intPanelWidth
                    .Controls.AddRange(New XRLabel() {fctMakeXrLabelNoDpi(cLang.GetString(clsEGSLanguage.CodeType.Allergens) & ":", fntHead, Color.Black, Color.Transparent, 0, intTextHeight, intTextLength, intTextHeight)})
                End With
                xrRowAllergensHead.Controls.AddRange(New XRTableCell() {xrCellAllergensHead})
                .Controls.AddRange(New XRTableRow() {xrRowAllergensHead})
                Dim xrRowAllergens As New XRTableRow
                xrRowAllergensHead.Borders = DevExpress.XtraPrinting.BorderSide.All
                Dim xrCellAllergens As New XRTableCell
                With xrCellAllergens
                    '.Borders = DevExpress.XtraPrinting.BorderSide.None
                    'Dim fntHead As Font = New Font("Arial", 12, FontStyle.Bold Or FontStyle.Underline)
                    Dim strAllergens As String = ""
                    For Each dtRow As DataRow In dtAllergens.Select("CodeListe=" & intCodeListe)
                        strAllergens &= dtRow("name").ToString() & ", "
                    Next
                    If strAllergens.Length > 2 Then
                        strAllergens = strAllergens.Substring(0, strAllergens.Length - 2)
                    End If

                    intTextHeight = MeasureText(strAllergens, fntDetails, intPanelWidth, sf1, Me.Padding).Height
                    .Width = intPanelWidth
                    .Controls.AddRange(New XRLabel() {fctMakeXrLabelNoDpi(strAllergens, fntDetails, Color.Black, Color.Transparent, 0, intTextYPos, intPanelWidth, intTextHeight)})
                End With
                xrRowAllergens.Controls.AddRange(New XRTableCell() {xrCellAllergens})
                .Controls.AddRange(New XRTableRow() {xrRowAllergens})
                '---

                '---Nutrients   
                Dim xrRowNutrientsHead As New XRTableRow
                xrRowNutrientsHead.Borders = DevExpress.XtraPrinting.BorderSide.All
                Dim xrCellNutrientsHead1 As New XRTableCell
                Dim xrCellNutrientsHead2 As New XRTableCell
                Dim xrCellNutrientsHead3 As New XRTableCell
                With xrCellNutrientsHead1
                    '.Borders = DevExpress.XtraPrinting.BorderSide.None
                    'Dim fntHead As Font = New Font("Arial", 12, FontStyle.Bold Or FontStyle.Underline)
                    intTextHeight = MeasureText(cLang.GetString(clsEGSLanguage.CodeType.Nutrients) & ":", fntHead, intPanelWidth, sf1, Me.Padding).Height
                    .Width = intPanelWidth * 0.3
                    .Controls.AddRange(New XRLabel() {fctMakeXrLabelNoDpi(cLang.GetString(clsEGSLanguage.CodeType.Nutrients) & ":", fntHead, Color.Black, Color.Transparent, 0, intTextHeight, intPanelWidth * 0.3, intTextHeight)})
                End With
                With xrCellNutrientsHead2
                    '.Borders = DevExpress.XtraPrinting.BorderSide.None
                    'Dim fntHead As Font = New Font("Arial", 12, FontStyle.Bold Or FontStyle.Underline)
                    'intTextHeight = MeasureText(cLang.GetString(clsEGSLanguage.CodeType.Nutrients) & ":", fntHead, intPanelWidth, sf1, Me.Padding).Height
                    .Width = intPanelWidth * 0.3
                    .Controls.AddRange(New XRLabel() {fctMakeXrLabelNoDpi(cLang.GetString(clsEGSLanguage.CodeType.PerYieldUnitAt100), fntHead2, Color.Black, Color.Transparent, 0, intTextYPos, intPanelWidth * 0.3, intTextHeight, DevExpress.XtraPrinting.TextAlignment.BottomLeft)})
                End With
                With xrCellNutrientsHead3
                    '.Borders = DevExpress.XtraPrinting.BorderSide.None
                    'Dim fntHead As Font = New Font("Arial", 12, FontStyle.Bold Or FontStyle.Underline)
                    'intTextHeight = MeasureText(cLang.GetString(clsEGSLanguage.CodeType.Nutrients) & ":", fntHead, intPanelWidth, sf1, Me.Padding).Height
                    .Width = intPanelWidth * 0.4
                    .Controls.AddRange(New XRLabel() {fctMakeXrLabelNoDpi(cLang.GetString(clsEGSLanguage.CodeType.Per100gOR100mlat100Percent), fntHead2, Color.Black, Color.Transparent, 0, intTextYPos, intPanelWidth * 0.4, intTextHeight, DevExpress.XtraPrinting.TextAlignment.BottomLeft)})
                End With


                xrRowNutrientsHead.Controls.AddRange(New XRTableCell() {xrCellNutrientsHead1})
                xrRowNutrientsHead.Controls.AddRange(New XRTableCell() {xrCellNutrientsHead2})
                xrRowNutrientsHead.Controls.AddRange(New XRTableCell() {xrCellNutrientsHead3})
                .Controls.AddRange(New XRTableRow() {xrRowNutrientsHead})
                fctGetNutrientDetails(intCodeTrans, intCodeSite)

                Dim ctr As Integer = 0
                For Each dtRow As DataRow In dtRecipeDetail.Select("Code=" & intCodeListe)
                    For ctr = 1 To 5
                        Dim xrRowNutrients As New XRTableRow
                        Dim xrCellNutrients1 As New XRTableCell
                        Dim xrCellNutrients2 As New XRTableCell
                        Dim xrCellNutrients3 As New XRTableCell
                        intTextHeight = MeasureText(G_Nutrient(ctr).Name, fntHead, intPanelWidth, sf1, Me.Padding).Height
                        xrCellNutrients1.Width = intPanelWidth * 0.3
                        xrCellNutrients1.Controls.AddRange(New XRLabel() {fctMakeXrLabelNoDpi(G_Nutrient(ctr).Name & " " & G_Nutrient(ctr).Unit, fntHead2, Color.Black, Color.Transparent, 0, 0, intPanelWidth * 0.3, intTextHeight)})
                        xrRowNutrients.Controls.AddRange(New XRTableCell() {xrCellNutrients1})

                        xrCellNutrients2.Width = intPanelWidth * 0.3
                        strX = Format(dtRow("n" & ctr), G_Nutrient(ctr).Format)
                        xrCellNutrients2.Controls.AddRange(New XRLabel() {fctMakeXrLabelNoDpi(strX, fntHead2, Color.Black, Color.Transparent, 0, 0, intPanelWidth * 0.3, intTextHeight)})
                        xrRowNutrients.Controls.AddRange(New XRTableCell() {xrCellNutrients2})

                        xrCellNutrients3.Width = intPanelWidth * 0.4
                        Dim dblNutrientFactor As Double = (CDblDB(dtRow("srWeight").ToString()) * 10)

                        If dblNutrientFactor > 0 Then
                            strX = Format(CDblDB(dtRow("n" & ctr)) / dblNutrientFactor, G_Nutrient(ctr).Format)
                        Else
                            strX = cLang.GetString(clsEGSLanguage.CodeType.NA)
                        End If
                        xrCellNutrients3.Controls.AddRange(New XRLabel() {fctMakeXrLabelNoDpi(strX, fntHead2, Color.Black, Color.Transparent, 0, 0, intPanelWidth * 0.4, intTextHeight)})

                        xrRowNutrients.Controls.AddRange(New XRTableCell() {xrCellNutrients3})

                        .Controls.AddRange(New XRTableRow() {xrRowNutrients})

                        If ctr = 1 Then
                            Dim xrRowNutrients2 As New XRTableRow
                            Dim xrCellNutrients12 As New XRTableCell
                            Dim xrCellNutrients22 As New XRTableCell
                            Dim xrCellNutrients32 As New XRTableCell
                            intTextHeight = MeasureText(G_Nutrient(ctr).Name, fntHead, intPanelWidth, sf1, Me.Padding).Height
                            xrCellNutrients12.Width = intPanelWidth * 0.3
                            xrCellNutrients12.Controls.AddRange(New XRLabel() {fctMakeXrLabelNoDpi(G_Nutrient(ctr).Name & "kcal", fntHead2, Color.Black, Color.Transparent, 0, 0, intPanelWidth * 0.3, intTextHeight)})
                            xrRowNutrients2.Controls.AddRange(New XRTableCell() {xrCellNutrients12})

                            xrCellNutrients22.Width = intPanelWidth * 0.3
                            strX = Format(CDblDB(dtRow("n" & ctr)) / ENERGYFACTOR, G_Nutrient(ctr).Format)
                            xrCellNutrients22.Controls.AddRange(New XRLabel() {fctMakeXrLabelNoDpi(strX, fntHead2, Color.Black, Color.Transparent, 0, 0, intPanelWidth * 0.3, intTextHeight)})
                            xrRowNutrients2.Controls.AddRange(New XRTableCell() {xrCellNutrients22})

                            xrCellNutrients32.Width = intPanelWidth * 0.4
                            dblNutrientFactor = (CDblDB(dtRow("srWeight").ToString()) * 10)

                            If dblNutrientFactor > 0 Then
                                strX = Format((CDblDB(dtRow("n" & ctr)) / ENERGYFACTOR) / dblNutrientFactor, G_Nutrient(ctr).Format)
                            Else
                                strX = cLang.GetString(clsEGSLanguage.CodeType.NA)
                            End If
                            xrCellNutrients32.Controls.AddRange(New XRLabel() {fctMakeXrLabelNoDpi(strX, fntHead2, Color.Black, Color.Transparent, 0, 0, intPanelWidth * 0.4, intTextHeight)})

                            xrRowNutrients2.Controls.AddRange(New XRTableCell() {xrCellNutrients32})

                            .Controls.AddRange(New XRTableRow() {xrRowNutrients2})
                        End If
                    Next
                    Exit For
                Next

                xrRowNutrientsHead.Borders = DevExpress.XtraPrinting.BorderSide.All


                '---
            End With
            '---

            '---Footer
            intCurrentY += xrDetailsTable.Height
            intTextHeight = MeasureText("Patisserie Tel: 061 3248021", fntFoot, intPanelWidth, sf1, Me.Padding).Height
            .Controls.AddRange(New XRLabel() {fctMakeXrLabelNoDpi("Patisserie Tel: 061 3248021", fntFoot, Color.Black, Color.Transparent, 0, intCurrentY, intPanelWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.BottomLeft)})
            intCurrentY += intTextHeight
            intTextHeight = MeasureText("SV (Schweiz) AG,", fntFoot, intPanelWidth, sf1, Me.Padding).Height
            .Controls.AddRange(New XRLabel() {fctMakeXrLabelNoDpi("SV (Schweiz) AG,", fntFoot, Color.Black, Color.Transparent, 0, intCurrentY, intPanelWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.BottomLeft)})
            intCurrentY += intTextHeight
            intTextHeight = MeasureText("Personalrestaurant Novartis,", fntFoot, intPanelWidth, sf1, Me.Padding).Height
            .Controls.AddRange(New XRLabel() {fctMakeXrLabelNoDpi("Personalrestaurant Novartis,", fntFoot, Color.Black, Color.Transparent, 0, intCurrentY, intPanelWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.BottomLeft)})
            intCurrentY += intTextHeight
            intTextHeight = MeasureText("St. Johann, 4002 Basel", fntFoot, intPanelWidth, sf1, Me.Padding).Height
            .Controls.AddRange(New XRLabel() {fctMakeXrLabelNoDpi("St. Johann, 4002 Basel", fntFoot, Color.Black, Color.Transparent, 0, intCurrentY, intPanelWidth, intTextHeight, DevExpress.XtraPrinting.TextAlignment.BottomLeft)})
            '---
            .Controls.AddRange(New XRTable() {xrDetailsTable})
        End With


        Return xrRecipePanel
    End Function



#End Region
#Region "For Hero"
    Public Function GenerateHeroRecipeDetail(ByVal ds As DataSet, ByVal intCodeLang As Integer, ByVal strPath As String, ByVal strPicPath As String, ByVal intUserCode As Integer, ByVal strMarginImgPath As String) As String
        Dim strMyMessage As String
        Dim strLabelText As String
        Dim ReportLeftMargin As Double
        Dim ReportRightMargin As Double
        Dim ReportTopMargin As Double
        Dim ReportBottomMargin As Double
        Dim TableWidth As Double
        Dim intPreviousForLocal As Integer = 0
        Dim strPreviousCountry As String = ""
        Dim strSpace As String = "   "
        Dim XrTable1 As New XRTable

        Dim dtPrintProfile As DataTable = ds.Tables(0)
        Dim dtRecipeDetails As DataTable = ds.Tables(1)
        Dim dtKeywords As DataTable = ds.Tables(2)
        Dim dtRecipe As DataTable = ds.Tables(3)
        Dim dtIngredients As DataTable = ds.Tables(4)

        Dim fntTitle As System.Drawing.Font = New System.Drawing.Font("Articulate", 26, FontStyle.Bold)
        Dim fntHeaderBold As System.Drawing.Font = New System.Drawing.Font("Articulate", 14, FontStyle.Bold)
        Dim fntHeader As System.Drawing.Font = New System.Drawing.Font("Helvetica Neue", 14, FontStyle.Regular)
        Dim fntBodyBold As System.Drawing.Font = New System.Drawing.Font("Articulate", 10, FontStyle.Bold)
        Dim fntBody As System.Drawing.Font = New System.Drawing.Font("Helvetica Neue", 8, FontStyle.Regular)
        Dim fntFoot As System.Drawing.Font = New System.Drawing.Font("Helvetica Neue", 8, FontStyle.Regular)

        Dim intCodeTrans As Integer = CIntDB(dtPrintProfile.Rows(0)("CodeLang"))

        Dim cLang As New EgsData.clsEGSLanguage(intCodeTrans)

        Dim fntPageInfo As System.Drawing.Font = New System.Drawing.Font("Arial Narrow", 6.25!, FontStyle.Regular)
        ReportTopMargin = 20
        ReportLeftMargin = 20
        ReportRightMargin = 20
        ReportBottomMargin = 20



        strLabelText = ""

        'strMyMessage = OpenConnection()

        Dim Report = New XtraReport
        With Report
            .ReportUnit = ReportUnit.TenthsOfAMillimeter
            'Papersize
            '------------------------------------------------------------------------------------

            .PaperKind = Printing.PaperKind.A4
            .Visible = True
            .Dpi = 254.0!

            TableWidth = .PageWidth - (ReportLeftMargin + ReportRightMargin) - 2
            '.PageWidth = DefaultPageWidth
            '.PageHeight = DefaultPageHeight
            '------------------------------------------------------------------------------------
            'Margins
            '------------------------------------------------------------------------------------
            .Margins.Left = ReportLeftMargin
            .Margins.Right = ReportRightMargin
            .Margins.Bottom = ReportBottomMargin
            .Margins.Top = ReportTopMargin

            '------------------------------------------------------------------------------------
            'Orientation
            '------------------------------------------------------------------------------------
            .Landscape = False

            '------------------------------------------------------------------------------------
            'ADD THE SECTION OF THE REPORT 
            '------------------------------------------------------------------------------------
            'BottomMargin      

            'Details
            Detail.Dpi = 254.0!
            Dim intAvailableHeight As Integer = .PageHeight - (ReportTopMargin + ReportBottomMargin)  '2664
            Dim intAvailableWidth As Integer = .pagewidth - (ReportLeftMargin + ReportRightMargin)
            Dim intCurrentY As Integer = 0
            Dim intPages As Integer = 0
            Dim intMarginHeight As Integer = intAvailableHeight - 148
            Dim intMarginWidth As Integer = intAvailableHeight * (510 / 3849)

            Dim intLineHeight As Integer
            Dim intColWidth As Integer
            Dim intBodyWidth As Integer
            Dim intTotalHeight As Integer
            Detail.Height = intMarginHeight
            PageFoot.Height = intMarginWidth * (193 / 510)
            PageFoot.Dpi = 254.0!
            For Each dtRow As DataRow In dtRecipe.Rows

                'Left margin image
                Dim xrMarginImage As New XRPictureBox
                xrMarginImage.Dpi = 254.0!
                xrMarginImage.Sizing = ImageSizeMode.StretchImage
                xrMarginImage.Size = New Point(intMarginWidth, intMarginHeight)
                xrMarginImage.Location = New Point(0, intCurrentY)
                If System.IO.File.Exists(strMarginImgPath + "statio_part1.jpg") Then
                    xrMarginImage.ImageUrl = strMarginImgPath + "statio_part1.jpg"
                    Detail.Controls.AddRange(New XRControl() {xrMarginImage})
                End If
                'Left margin image end

                'Header
                intCurrentY += 200

                Dim strPics() As String = CStrDB(dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("pix")).Split(";")

                If strPics.Length > 0 Then
                    Dim xrImage1 As New XRPictureBox
                    xrImage1.Dpi = 254.0!
                    'xrImage1.Sizing = ImageSizeMode.AutoSize
                    'xrImage1.Size = New Point(intMarginWidth, intMarginHeight)

                    If System.IO.File.Exists(strPicPath + strPics(0)) Then
                        xrImage1.ImageUrl = strPicPath + strPics(0)
                        xrImage1.Size = New Point(500, 350)
                        xrImage1.Sizing = ImageSizeMode.ZoomImage
                        xrImage1.Location = New Point(intAvailableWidth - 520, intCurrentY - 100)
                        Detail.Controls.AddRange(New XRControl() {xrImage1})
                    End If
                End If
                If strPics.Length > 1 Then
                    Dim xrImage2 As New XRPictureBox
                    xrImage2.Dpi = 254.0!
                    'xrImage1.Sizing = ImageSizeMode.AutoSize
                    'xrImage1.Size = New Point(intMarginWidth, intMarginHeight)

                    If System.IO.File.Exists(strPicPath + strPics(1)) Then
                        xrImage2.ImageUrl = strPicPath + strPics(1)
                        xrImage2.Size = New Point(400, 230)
                        xrImage2.Sizing = ImageSizeMode.ZoomImage
                        xrImage2.Location = New Point(intAvailableWidth - 420, intCurrentY + 270)
                        Detail.Controls.AddRange(New XRControl() {xrImage2})
                    End If
                End If

                intColWidth = (intAvailableWidth - (intMarginWidth + 400 + 800))
                intBodyWidth = (intAvailableWidth - (intMarginWidth + 120))
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(dtRow("name"), fntTitle, intMarginWidth + 100, intCurrentY - 100, intColWidth * 2, 50, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , , True, True)})

                intLineHeight = MeasureText("A", fntTitle, intColWidth * 2, sf1, Me.Padding).Height
                intCurrentY += ((MeasureText(dtRow("name"), fntTitle, intColWidth * 2, sf1, Me.Padding).Height / intLineHeight) * 150)

                strX = " " & cLang.GetString(clsEGSLanguage.CodeType.Category) & ":"
                intLineHeight = MeasureText("A", fntHeaderBold, intColWidth, sf1, Me.Padding).Height
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntHeaderBold, intMarginWidth + 100, intCurrentY, intColWidth, 100, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, True, , , , 3, True, True)})

                strX = dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("categoryname")
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntHeader, intMarginWidth + 100 + intColWidth + 50, intCurrentY, intColWidth, 100, DevExpress.XtraPrinting.TextAlignment.MiddleCenter, True, DevExpress.XtraPrinting.BorderSide.None, True, True, , , , 3, True, True)})

                intCurrentY += 150

                strX = " " & cLang.GetString(clsEGSLanguage.CodeType.Recipe) & " " & cLang.GetString(clsEGSLanguage.CodeType.Yield) & ":"
                intLineHeight = MeasureText("A", fntHeaderBold, intColWidth, sf1, Me.Padding).Height
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntHeaderBold, intMarginWidth + 100, intCurrentY, intColWidth, 100, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, True, , , , 3, True, True)})

                strX = dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("originalyield") & " " & dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("portionunitdef")
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntHeader, intMarginWidth + 100 + intColWidth + 50, intCurrentY, intColWidth, 100, DevExpress.XtraPrinting.TextAlignment.MiddleCenter, True, DevExpress.XtraPrinting.BorderSide.None, True, True, , , , 3, True, True)})

                intCurrentY += 150

                strX = " " & cLang.GetString(clsEGSLanguage.CodeType.Price) & " " & cLang.GetString(clsEGSLanguage.CodeType.per_serving) & ":"
                intLineHeight = MeasureText("A", fntHeaderBold, intColWidth, sf1, Me.Padding).Height
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntHeaderBold, intMarginWidth + 100, intCurrentY, intColWidth, 100, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, True, , , , 3, True, True)})

                strX = dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("symbole") & " " & Format(CDblDB(dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("calcprice")), "0.00")
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntHeader, intMarginWidth + 100 + intColWidth + 50, intCurrentY, intColWidth, 100, DevExpress.XtraPrinting.TextAlignment.MiddleCenter, True, DevExpress.XtraPrinting.BorderSide.None, True, True, , , , 3, True, True)})
                'Header end

                intCurrentY += 200

                'ingredients
                Dim xrTableIng As New XRTable
                xrTableIng.Dpi = 254.0!
                xrTableIng.Width = intBodyWidth
                strX = cLang.GetString(clsEGSLanguage.CodeType.Amount)
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBodyBold, intAvailableWidth - 300, intCurrentY, 280, 50, DevExpress.XtraPrinting.TextAlignment.MiddleCenter, True, DevExpress.XtraPrinting.BorderSide.None, True, True, , , , 3, True, True)})

                strX = cLang.GetString(clsEGSLanguage.CodeType.Units)
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBodyBold, intAvailableWidth - 450, intCurrentY, 200, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, True, , , , 3, True, True)})

                strX = cLang.GetString(clsEGSLanguage.CodeType.Price) & " / "
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBodyBold, intAvailableWidth - 600, intCurrentY, 150, 50, DevExpress.XtraPrinting.TextAlignment.MiddleRight, True, DevExpress.XtraPrinting.BorderSide.None, True, True, , , , 3, True, True)})

                strX = "" 'cLang.GetString(clsEGSLanguage.CodeType.Currency)
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBodyBold, intAvailableWidth - 650, intCurrentY, 100, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)})

                strX = cLang.GetString(clsEGSLanguage.CodeType.Ingredients)
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBodyBold, intMarginWidth + 100 + 170, intCurrentY, 810, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, True, , , , 3, True, True)})

                'strX = CStrDB(dtIngRow("itemunit"))
                'Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntbodybold, intMarginWidth + 100 + 170, intCurrentY, 150, 50, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)})

                strX = " " & cLang.GetString(clsEGSLanguage.CodeType.Quantity)
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBodyBold, intMarginWidth + 100, intCurrentY, 270, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, True, , , , 3, True, True)})
                intCurrentY += 70
                For Each dtIngRow As DataRow In dtIngredients.Select("CodeMain=" & dtRow("code"))
                    strX = Format(CDblDB(dtIngRow("itemcost")), "0.00")
                    Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, intAvailableWidth - 300, intCurrentY, 280, 50, DevExpress.XtraPrinting.TextAlignment.TopCenter, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)})

                    strX = CStrDB(dtIngRow("priceunit"))
                    Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, intAvailableWidth - 450, intCurrentY, 150, 50, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)})

                    strX = Format(CDblDB(dtIngRow("itemprice")), "0.00") & " / "
                    Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, intAvailableWidth - 600, intCurrentY, 150, 50, DevExpress.XtraPrinting.TextAlignment.TopRight, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)})

                    strX = CStrDB(dtIngRow("currency"))
                    Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, intAvailableWidth - 650, intCurrentY, 100, 50, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)})

                    strX = CStrDB(dtIngRow("itemname"))
                    Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, intMarginWidth + 100 + 170, intCurrentY, 710, 50, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)})

                    strX = CStrDB(dtIngRow("itemunit"))
                    Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, intMarginWidth + 100 + 100, intCurrentY, 100, 50, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)})

                    strX = Format(CDblDB(dtIngRow("netquantity")), "0.00")
                    Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, intMarginWidth + 100, intCurrentY, 100, 50, DevExpress.XtraPrinting.TextAlignment.TopRight, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)})


                    intCurrentY += 50
                Next

                If intCurrentY Mod intAvailableHeight > 2000 Then
                    intPages += 1
                    intCurrentY = intPages * intAvailableHeight
                    Dim xrMarginImage2 As New XRPictureBox
                    xrMarginImage2.Dpi = 254.0!
                    xrMarginImage2.Sizing = ImageSizeMode.StretchImage
                    xrMarginImage2.Size = New Point(intMarginWidth, intMarginHeight)
                    xrMarginImage2.Location = New Point(0, intCurrentY)
                    If System.IO.File.Exists(strMarginImgPath + "statio_part1.jpg") Then
                        xrMarginImage2.ImageUrl = strMarginImgPath + "statio_part1.jpg"
                        Detail.Controls.AddRange(New XRControl() {xrMarginImage2})
                    End If
                End If

                'strX = cLang.GetString(clsEGSLanguage.CodeType.Amount)
                'Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntHeader, intAvailableWidth - 300, intCurrentY, 300, 50, DevExpress.XtraPrinting.TextAlignment.TopCenter, True, DevExpress.XtraPrinting.BorderSide.None, True, True, , , , 3, True, True)})
                'ingredients end

                intCurrentY += 50
                'Dim xrPnlTemp As New XRPanel
                'With xrPnlTemp
                '    .Dpi = 254.0!
                '    .Location = New Point(intMarginWidth + 100, intCurrentY)
                '    .Width = intBodyWidth
                '    .Borders = DevExpress.XtraPrinting.BorderSide.All
                'End With

                'Procedure
                strX = " " & cLang.GetString(clsEGSLanguage.CodeType.Procedure) & ":"
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBodyBold, intMarginWidth + 100, intCurrentY, intBodyWidth, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, True, , , , 3, True, True)})
                '.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBodyBold, 0, 0, intBodyWidth, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, True, , , , 3, True, True)})
                intCurrentY += 70

                strX = dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("note")
                Dim xrLblTemp As XRLabel = fctMakeXRLabel(strX, fntBody, intMarginWidth + 100, intCurrentY, intBodyWidth, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, intMarginWidth + 100, intCurrentY, intBodyWidth, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)})
                'xrPnlTemp.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, 0, 70, intBodyWidth, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)})


                'Detail.Controls.AddRange(New XRControl() {xrPnlTemp})
                intLineHeight = MeasureText("A", fntBody, intBodyWidth, sf1, Me.Padding).Height
                Dim strTemp() As String = strX.Split(vbCrLf)
                'For Each strProc As String In strTemp
                '    Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strProc, fntBody, intMarginWidth + 100, intCurrentY, intBodyWidth, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)})
                '    intCurrentY += 50
                '    If intCurrentY Mod intAvailableHeight > 2000 Then
                '        intPages += 1
                '        intCurrentY = intPages * intAvailableHeight
                '        Dim xrMarginImage2 As New XRPictureBox
                '        xrMarginImage2.Dpi = 254.0!
                '        xrMarginImage2.Sizing = ImageSizeMode.StretchImage
                '        xrMarginImage2.Size = New Point(intMarginWidth, intMarginHeight)
                '        xrMarginImage2.Location = New Point(0, intCurrentY)
                '        If System.IO.File.Exists(strMarginImgPath + "statio_part1.jpg") Then
                '            xrMarginImage2.ImageUrl = strMarginImgPath + "statio_part1.jpg"
                '            Detail.Controls.AddRange(New XRControl() {xrMarginImage2})
                '        End If
                '    End If
                'Next
                intTotalHeight = strTemp.Length
                intCurrentY += ((intTotalHeight * 5) + 70)
                'Procedure end

                If intCurrentY Mod intAvailableHeight > 2000 Then
                    intPages += 1
                    intCurrentY = intPages * intAvailableHeight
                    Dim xrMarginImage2 As New XRPictureBox
                    xrMarginImage2.Dpi = 254.0!
                    xrMarginImage2.Sizing = ImageSizeMode.StretchImage
                    xrMarginImage2.Size = New Point(intMarginWidth, intMarginHeight)
                    xrMarginImage2.Location = New Point(0, intCurrentY)
                    If System.IO.File.Exists(strMarginImgPath + "statio_part1.jpg") Then
                        xrMarginImage2.ImageUrl = strMarginImgPath + "statio_part1.jpg"
                        Detail.Controls.AddRange(New XRControl() {xrMarginImage2})
                    End If
                End If

                'tips
                strX = " " & cLang.GetString(clsEGSLanguage.CodeType.Remark) & ":"
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBodyBold, intMarginWidth + 100, intCurrentY, intBodyWidth, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, True, , , , 3, True, True)})
                intCurrentY += 70

                strX = CStrDB(dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("remark1"))
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, intMarginWidth + 100, intCurrentY, intBodyWidth, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)})

                'tips end


                intPages += 1
                intCurrentY = intPages * intAvailableHeight

            Next
            'Dim Panel1 As New XRPanel
            'Panel1.Dpi = 254.0! 'ReportUnit.TenthsOfAMillimeter
            'Panel1.Location = New Point(0, 0)
            'Panel1.Height = intAvailableHeight - 2
            'Panel1.Width = intAvailableWidth - 2
            'Panel1.Borders = DevExpress.XtraPrinting.BorderSide.All



            'Detail.Controls.AddRange(New XRPanel() {Panel1})

            'footer
            Dim xrFooterImage As New XRPictureBox
            xrFooterImage.Dpi = 254.0!
            xrFooterImage.Sizing = ImageSizeMode.StretchImage
            xrFooterImage.Size = New Point(intMarginWidth, intMarginWidth * (193 / 510))
            xrFooterImage.Location = New Point(0, 0)
            If System.IO.File.Exists(strMarginImgPath + "statio_part2.jpg") Then
                xrFooterImage.ImageUrl = strMarginImgPath + "statio_part2.jpg"
                PageFoot.Controls.AddRange(New XRControl() {xrFooterImage})
            End If
            strX = "Hero Gastronomique | Postfach | 5600 Lenzburg 1"
            strX &= vbCrLf & "Tel. 062 885 54 50 | Fax 062 885 55 44 | gastro@hero.ch | www.gastro.hero.ch"
            PageFoot.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntFoot, intMarginWidth + 100, 0, intBodyWidth, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , "gray", , 3, True, True)})
            'footer end

            'PageFoot.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, intMarginWidth + 100, 20, intBodyWidth, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)})
            '--- final report -----
            .bands.add(PageFoot)
            .Bands.Add(Detail)          'DETAIL SECTION
            .CreatePdfDocument(strPath)
        End With

        strMyMessage = "" 'fctExportToPdfFormat(strFileNamePDF)
        Return strMyMessage
    End Function
#End Region

#Region "For Moevenpick"
    Public Function fctGetMoevenpickExportPDF(ByVal ds As DataSet, ByVal strPath As String, ByVal strPicPath As String) As String
        Dim strMyMessage As String
        Dim strLabelText As String
        Dim ReportLeftMargin As Double
        Dim ReportRightMargin As Double
        Dim ReportTopMargin As Double
        Dim ReportBottomMargin As Double
        Dim TableWidth As Double
        Dim intPreviousForLocal As Integer = 0
        Dim strPreviousCountry As String = ""
        Dim strSpace As String = "   "
        Dim XrTable1 As New XRTable

        Dim dtPrintProfile As DataTable = ds.Tables(0)
        Dim dtRecipeDetails As DataTable = ds.Tables(1)
        Dim dtKeywords As DataTable = ds.Tables(2)
        Dim dtRecipe As DataTable = ds.Tables(3)
        Dim dtIngredients As DataTable = ds.Tables(4)


        Dim fntBodyBold As System.Drawing.Font = New System.Drawing.Font("Arial", 10.0!, FontStyle.Bold)
        Dim fntBody As System.Drawing.Font = New System.Drawing.Font("Arial", 10.0!, FontStyle.Regular)


        Dim intCodeTrans As Integer = CIntDB(dtPrintProfile.Rows(0)("CodeLang"))

        Dim cLang As New EgsData.clsEGSLanguage(intCodeTrans)

        ReportTopMargin = 100
        ReportLeftMargin = 100
        ReportRightMargin = 100
        ReportBottomMargin = 100



        strLabelText = ""

        'strMyMessage = OpenConnection()

        Dim Report = New XtraReport
        With Report
            .ReportUnit = ReportUnit.TenthsOfAMillimeter
            'Papersize
            '------------------------------------------------------------------------------------

            .PaperKind = Printing.PaperKind.A4
            .Visible = True
            .Dpi = 254.0!

            TableWidth = .PageWidth - (ReportLeftMargin + ReportRightMargin) - 2
            '.PageWidth = DefaultPageWidth
            '.PageHeight = DefaultPageHeight
            '------------------------------------------------------------------------------------
            'Margins
            '------------------------------------------------------------------------------------
            .Margins.Left = ReportLeftMargin
            .Margins.Right = ReportRightMargin
            .Margins.Bottom = ReportBottomMargin
            .Margins.Top = ReportTopMargin

            '------------------------------------------------------------------------------------
            'Orientation
            '------------------------------------------------------------------------------------
            .Landscape = False

            '------------------------------------------------------------------------------------
            'ADD THE SECTION OF THE REPORT 
            '------------------------------------------------------------------------------------
            'BottomMargin      

            'Details
            Detail.Dpi = 254.0!
            Dim intAvailableHeight As Integer = .PageHeight - (ReportTopMargin + ReportBottomMargin)  '2664
            Dim intAvailableWidth As Integer = .pagewidth - (ReportLeftMargin + ReportRightMargin)
            Dim intCurrentY As Integer = 0
            Dim intPages As Integer = 0
            Detail.Height = intAvailableHeight
            Dim intLineHeight As Integer
            Dim intColWidth As Integer
            Dim intBodyWidth As Integer
            Dim intTotalHeight As Integer


            For Each dtRow As DataRow In dtRecipe.Rows

                'Header
                Dim xrPanel1 As New XRPanel

                With xrPanel1
                    .Dpi = 254.0!
                    .Width = intAvailableWidth - 2
                    .Height = intAvailableHeight - 2
                    .CanGrow = False
                    .CanShrink = True
                    .Location = New Point(0, intPages * (intAvailableHeight))
                End With
                intPages += 1
                intCurrentY = 0
                Dim strPics() As String = CStrDB(dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("pix")).Split(";")

                'If strPics.Length > 0 Then
                '    Dim xrImage1 As New XRPictureBox
                '    xrImage1.Dpi = 254.0!
                '    'xrImage1.Sizing = ImageSizeMode.AutoSize
                '    'xrImage1.Size = New Point(intMarginWidth, intMarginHeight)

                '    If System.IO.File.Exists(strPicPath + strPics(0)) Then
                '        xrImage1.ImageUrl = strPicPath + strPics(0)
                '        xrImage1.Size = New Point(500, 350)
                '        xrImage1.Sizing = ImageSizeMode.ZoomImage
                '        xrImage1.Location = New Point(intAvailableWidth - 520, intCurrentY - 100)
                '        xrpanel1.Controls.AddRange(New XRControl() {xrImage1})
                '    End If
                'End If

                strX = dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("recipenametrans")
                If strX.Trim.Length > 0 Then
                Else
                    strX = dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("recipename")
                End If
                intLineHeight = MeasureText("A", fntBodyBold, intColWidth, sf1, Me.Padding).Height
                xrPanel1.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBodyBold, 0, intCurrentY, intAvailableWidth - 10, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, False, DevExpress.XtraPrinting.BorderSide.None, False, False, , , , 0, True, True)})

                intCurrentY += 50

                strX = dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("categorynametrans")
                If strX.Trim.Length > 0 Then
                Else
                    strX = dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("categoryname")
                End If
                xrPanel1.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, 0, intCurrentY, intAvailableWidth - 10, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, False, DevExpress.XtraPrinting.BorderSide.None, False, False, , , , 0, True, True)})

                intCurrentY += 150

                strX = " " & cLang.GetString(clsEGSLanguage.CodeType.For_)
                xrPanel1.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBodyBold, 0, intCurrentY, 200, 20, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 0, True, True)})

                strX = " " & dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("Yield")
                xrPanel1.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBodyBold, 200, intCurrentY, 200, 20, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 0, True, True)})

                intCurrentY += 50

                strX = " " & cLang.GetString(clsEGSLanguage.CodeType.Ingredients)
                xrPanel1.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBodyBold, 0, intCurrentY, intAvailableWidth - 10, 20, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 0, True, True)})

                intCurrentY += 50

                For Each dtIngRow As DataRow In dtIngredients.Select("CodeMain=" & dtRow("code"))
                    If CStrDB(dtIngRow("itemname")) = "dummy" Then
                        strX = Format(CDblDB(dtIngRow("tmpqty")), "0.00")
                        xrPanel1.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, 0, intCurrentY, 200, 60, DevExpress.XtraPrinting.TextAlignment.TopRight, False, DevExpress.XtraPrinting.BorderSide.None, False, False, , , , 3, True, True)})
                        strX = CStrDB(dtIngRow("tmpunit"))
                        xrPanel1.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, 230, intCurrentY, 100, 60, DevExpress.XtraPrinting.TextAlignment.TopLeft, False, DevExpress.XtraPrinting.BorderSide.None, False, False, , , , 3, True, True)})
                        strX = CStrDB(dtIngRow("tmpname"))
                        xrPanel1.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, 360, intCurrentY, 710, 60, DevExpress.XtraPrinting.TextAlignment.TopLeft, False, DevExpress.XtraPrinting.BorderSide.None, False, False, , , , 3, True, True)})
                        intCurrentY += 60
                    Else
                        strX = Format(CDblDB(dtIngRow("grossquantity")), "0.00")
                        xrPanel1.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, 0, intCurrentY, 200, 60, DevExpress.XtraPrinting.TextAlignment.TopRight, False, DevExpress.XtraPrinting.BorderSide.None, False, False, , , , 3, True, True)})
                        strX = CStrDB(dtIngRow("itemunit"))
                        xrPanel1.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, 230, intCurrentY, 100, 60, DevExpress.XtraPrinting.TextAlignment.TopLeft, False, DevExpress.XtraPrinting.BorderSide.None, False, False, , , , 3, True, True)})
                        strX = CStrDB(dtIngRow("itemname"))
                        xrPanel1.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, 360, intCurrentY, 710, 60, DevExpress.XtraPrinting.TextAlignment.TopLeft, False, DevExpress.XtraPrinting.BorderSide.None, False, False, , , , 3, True, True)})
                        intCurrentY += 60
                    End If


                Next

                intCurrentY += 50

                strX = " " & cLang.GetString(clsEGSLanguage.CodeType.Procedure)
                xrPanel1.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBodyBold, 0, intCurrentY, intAvailableWidth - 10, 500, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, False, False, , , , 0, True, True)})

                intCurrentY += 50

                strX = dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("notetrans")
                xrPanel1.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, 0, intCurrentY, intAvailableWidth - 10, 500, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, False, False, , , , 0, True, True)})

                Dim strPix() As String = dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("pix").ToString.Split(";")
                Dim strPic As String = ""
                For Each str As String In strPix
                    If str.Trim.Length > 0 Then
                        If File.Exists(strPicPath & str) Then
                            strPic = strPicPath + str
                        End If
                    End If
                Next

                Dim xrLb As XRLabel = fctMakeXRLabel(strX, fntBody, 0, intCurrentY, intAvailableWidth - 10, 500, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, False, False, , , , 0, True, True)
                intCurrentY += xrLb.Height
                If strPic.Trim.Length > 0 Then
                    Dim xrPic As New XRPictureBox
                    With xrPic
                        .Dpi = 254.0!
                        .ImageUrl = strPic
                        .Sizing = ImageSizeMode.AutoSize
                        Dim intX As Integer = (intAvailableWidth / 2) - (.Width / 2)
                        .Location = New Point(intX, intCurrentY + 50)
                    End With
                    xrPanel1.Controls.Add(xrPic)
                End If


                Detail.Controls.Add(xrPanel1)


                'intPages += 1
                'intCurrentY = intPages * (intAvailableHeight - 200)

                ''strX = cLang.GetString(clsEGSLanguage.CodeType.Amount)
                ''Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntHeader, intAvailableWidth - 300, intCurrentY, 300, 50, DevExpress.XtraPrinting.TextAlignment.TopCenter, True, DevExpress.XtraPrinting.BorderSide.None, True, True, , , , 3, True, True)})
                ''ingredients end

                'intCurrentY += 50
                ''Dim xrPnlTemp As New XRPanel
                ''With xrPnlTemp
                ''    .Dpi = 254.0!
                ''    .Location = New Point(intMarginWidth + 100, intCurrentY)
                ''    .Width = intBodyWidth
                ''    .Borders = DevExpress.XtraPrinting.BorderSide.All
                ''End With

                ''Procedure
                'strX = " " & cLang.GetString(clsEGSLanguage.CodeType.Procedure) & ":"
                'Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBodyBold, intMarginWidth + 100, intCurrentY, intBodyWidth, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, True, , , , 3, True, True)})
                ''.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBodyBold, 0, 0, intBodyWidth, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, True, , , , 3, True, True)})
                'intCurrentY += 70

                'strX = dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("note")
                'Dim xrLblTemp As XRLabel = fctMakeXRLabel(strX, fntBody, intMarginWidth + 100, intCurrentY, intBodyWidth, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)
                'Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, intMarginWidth + 100, intCurrentY, intBodyWidth, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)})
                ''xrPnlTemp.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, 0, 70, intBodyWidth, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)})


                ''Detail.Controls.AddRange(New XRControl() {xrPnlTemp})
                'intLineHeight = MeasureText("A", fntBody, intBodyWidth, sf1, Me.Padding).Height
                'Dim strTemp() As String = strX.Split(vbCrLf)
                ''For Each strProc As String In strTemp
                ''    Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strProc, fntBody, intMarginWidth + 100, intCurrentY, intBodyWidth, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)})
                ''    intCurrentY += 50
                ''    If intCurrentY Mod intAvailableHeight > 2000 Then
                ''        intPages += 1
                ''        intCurrentY = intPages * intAvailableHeight
                ''        Dim xrMarginImage2 As New XRPictureBox
                ''        xrMarginImage2.Dpi = 254.0!
                ''        xrMarginImage2.Sizing = ImageSizeMode.StretchImage
                ''        xrMarginImage2.Size = New Point(intMarginWidth, intMarginHeight)
                ''        xrMarginImage2.Location = New Point(0, intCurrentY)
                ''        If System.IO.File.Exists(strMarginImgPath + "statio_part1.jpg") Then
                ''            xrMarginImage2.ImageUrl = strMarginImgPath + "statio_part1.jpg"
                ''            Detail.Controls.AddRange(New XRControl() {xrMarginImage2})
                ''        End If
                ''    End If
                ''Next
                'intTotalHeight = strTemp.Length
                'intCurrentY += ((intTotalHeight * 5) + 70)
                ''Procedure end



                ''tips
                'strX = " " & cLang.GetString(clsEGSLanguage.CodeType.Remark) & ":"
                'Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBodyBold, intMarginWidth + 100, intCurrentY, intBodyWidth, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, True, , , , 3, True, True)})
                'intCurrentY += 70

                'strX = CStrDB(dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("remark1"))
                'Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntBody, intMarginWidth + 100, intCurrentY, intBodyWidth, 50, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , 3, True, True)})

                ''tips end


                'intPages += 1
                'intCurrentY = intPages * intAvailableHeight

            Next
            'Dim Panel1 As New XRPanel
            'Panel1.Dpi = 254.0! 'ReportUnit.TenthsOfAMillimeter
            'Panel1.Location = New Point(0, 0)
            'Panel1.Height = intAvailableHeight - 2
            'Panel1.Width = intAvailableWidth - 2
            'Panel1.Borders = DevExpress.XtraPrinting.BorderSide.All



            'Detail.Controls.AddRange(New XRPanel() {Panel1})


            .Bands.Add(Detail)          'DETAIL SECTION
            .CreatePdfDocument(strPath)
        End With

        strMyMessage = "" 'fctExportToPdfFormat(strFileNamePDF)
        Return strMyMessage
    End Function

    Public Function fctGetMoevenpickExportXLS(ByVal ds As DataSet, ByVal strPath As String, ByVal strPicPath As String, ByVal intCodeSite As Integer) As String

        Dim oExcel As Interop.Excel.Application
        Dim oBook As Interop.Excel.Workbook
        Dim intRecipe As Integer
        Dim intCodeListe As Integer
        Dim intLatestRow As Integer
        Dim strWorksheetName As String
        Dim ctr As Integer
        Dim oSheet As New Interop.Excel.Worksheet
        Dim dblColWidth, dblRowHeight As Double
        Dim intGDARow, intRow, intNutrientRows As Integer
        Dim dtPrintProfile As DataTable = ds.Tables(0)
        Dim dtRecipeDetails As DataTable = ds.Tables(1)
        Dim dtKeywords As DataTable = ds.Tables(2)
        Dim dtRecipe As DataTable = ds.Tables(3)
        Dim dtIngredients As DataTable = ds.Tables(4)
        Dim intSheetCount As Integer = 0
        Dim hashWorkSheet As New Hashtable
        Dim intCodeTrans As Integer = CIntDB(dtPrintProfile.Rows(0)("CodeLang"))
        Dim cLang As New clsEGSLanguage(intCodeTrans)
        'oExcel = CreateObject("Excel.Application")
        oExcel = New Excel.Application
        If oExcel.Workbooks.Count = 0 And dtRecipe.Rows.Count > 0 Then
            oBook = oExcel.Workbooks.Add()
        Else
            Try
                oBook = oExcel.Workbooks(0)
            Catch ex As Exception
                oBook = oExcel.Workbooks.Add()
            End Try
        End If

        intRecipe = 1

        For Each dsRow As DataRow In dtRecipe.Rows

            If oBook.Worksheets.Count < intRecipe Then
                oSheet = Nothing
                oSheet = oBook.Worksheets.Add()
            Else
                oSheet = Nothing
                oSheet = oBook.Worksheets(intRecipe)
            End If

            'Add data to cells of the first worksheet in the new workbook.

            intRecipe += 1
            'get worksheet name

            'strWorksheetName = dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("recipenametrans")
            strWorksheetName = ""
            Dim strRecipename As String = dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("recipenametrans").ToString

            If strRecipename.Trim.Length > 0 Then
            Else
                strRecipename = dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("recipename").ToString
            End If

            For ctr = 0 To strRecipename.ToString.Length - 1
                If strRecipename.Substring(ctr, 1).ToUpper Like "[ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789]" Then
                    strWorksheetName = strWorksheetName + strRecipename.Substring(ctr, 1)
                End If
            Next
            If strWorksheetName.Length > 20 Then
                strWorksheetName = strWorksheetName.Substring(0, 20) & "..."
            End If
            intSheetCount += 1
            If hashWorkSheet.ContainsValue(strWorksheetName.ToLower) Then
                strWorksheetName &= "_" & intSheetCount
            End If
            hashWorkSheet.Add(intSheetCount, strWorksheetName.ToLower)
            'get worksheet name end

            With oSheet
                .Name = strWorksheetName
                With .Range("A2:E2")
                    .Merge()
                    .Font.Bold = True
                    .Font.Size = 12
                    .RowHeight = 50
                    .VerticalAlignment = XlVAlign.xlVAlignTop
                    .Value = strRecipename 'dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("recipenametrans").ToString
                End With
                With .Range("A3:B3")
                    .Merge()
                    .Font.Size = 12
                    .Value = cLang.GetString(clsEGSLanguage.CodeType.Category)
                End With
                With .Range("A4:B4")
                    .Merge()
                    .Font.Size = 12
                    .Value = cLang.GetString(clsEGSLanguage.CodeType.Yield)
                End With

                With .Range("C3:E3")
                    .Merge()
                    .Font.Size = 10
                    .Font.Bold = True
                    .Value = dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("categorynametrans").ToString
                End With
                With .Range("C4")
                    .Font.Size = 10
                    .Font.Bold = True
                    .HorizontalAlignment = XlHAlign.xlHAlignRight
                    .Value = dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("yield").ToString
                End With
                With .Range("D4")
                    .Font.Size = 10
                    .Font.Bold = True
                    .HorizontalAlignment = XlHAlign.xlHAlignLeft
                    .Value = dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("portionunit").ToString
                End With

                With .Range("A8:K8")
                    .Font.Size = 10
                    .Font.Bold = True
                    .VerticalAlignment = XlVAlign.xlVAlignCenter
                    .RowHeight = 25
                End With

                With .Range("A8")
                    .HorizontalAlignment = XlHAlign.xlHAlignRight
                    .Value = cLang.GetString(clsEGSLanguage.CodeType.Quantity)
                End With

                With .Range("B8")
                    .HorizontalAlignment = XlHAlign.xlHAlignLeft
                    .Value = cLang.GetString(clsEGSLanguage.CodeType.Unit)
                End With


                With .Range("C8")
                    .HorizontalAlignment = XlHAlign.xlHAlignLeft
                    .Value = cLang.GetString(clsEGSLanguage.CodeType.Ingredients)
                End With

                With .Range("G8")
                    .HorizontalAlignment = XlHAlign.xlHAlignRight
                    .Value = cLang.GetString(clsEGSLanguage.CodeType.Cost)
                End With

                With .Range("I8")
                    .HorizontalAlignment = XlHAlign.xlHAlignCenter
                    .Value = cLang.GetString(clsEGSLanguage.CodeType.Waste)
                End With

                With .Range("J8")
                    .HorizontalAlignment = XlHAlign.xlHAlignRight
                    .Value = cLang.GetString(clsEGSLanguage.CodeType.Gross_Qty)
                End With

                With .Range("K8")
                    .HorizontalAlignment = XlHAlign.xlHAlignRight
                    .Value = cLang.GetString(clsEGSLanguage.CodeType.TotalCost)
                End With

                Dim intRecipeRows As Integer = dtIngredients.Select("CodeMain=" & dsRow("code")).Length

                Dim intIngRow As Integer = 9
                For Each dtRow As DataRow In dtIngredients.Select("CodeMain=" & dsRow("code"))
                    If dtRow("itemname") = "dummy" Then
                        With .Range("A" & intIngRow)
                            .HorizontalAlignment = XlHAlign.xlHAlignRight
                            .Value = Format(CDblDB(dtRow("tmpqty")), "0.000")
                            .Font.Size = 10
                            .NumberFormat = "0.000"
                        End With
                        With .Range("B" & intIngRow)
                            .Value = CStrDB(dtRow("tmpunit"))
                            .Font.Size = 10
                            .HorizontalAlignment = XlHAlign.xlHAlignLeft
                        End With
                        With .Range("C" & intIngRow & ":E" & intIngRow)
                            .Merge()
                            .Value = CStrDB(dtRow("tmpname"))
                            .Font.Size = 10
                            .HorizontalAlignment = XlHAlign.xlHAlignLeft
                        End With
                        With .Range("G" & intIngRow)
                            .Value = Format(CDblDB(dtRow("itemprice")), "00.00")
                            .Font.Size = 10
                            .HorizontalAlignment = XlHAlign.xlHAlignRight
                            .NumberFormat = "0.00"
                        End With
                        With .Range("H" & intIngRow)
                            .Value = "/" & CStrDB(dtRow("priceunit"))
                            .Font.Size = 10
                            .HorizontalAlignment = XlHAlign.xlHAlignLeft
                        End With
                        With .Range("I" & intIngRow)
                            .Value = Format(CDblDB(dtRow("totalwastage")), "0") & "%"
                            .Font.Size = 10
                            .HorizontalAlignment = XlHAlign.xlHAlignCenter
                        End With
                        With .Range("J" & intIngRow)
                            .Value = Format(CDblDB(dtRow("tmpqty")), "0.000")
                            .Font.Size = 10
                            .HorizontalAlignment = XlHAlign.xlHAlignRight
                            .NumberFormat = "0.000"
                        End With
                        With .Range("K" & intIngRow)
                            .Formula = "=J" & intIngRow & "*G" & intIngRow
                            .Font.Size = 10
                            .HorizontalAlignment = XlHAlign.xlHAlignRight
                            .NumberFormat = "0.00"
                        End With
                    Else
                        With .Range("A" & intIngRow)
                            .HorizontalAlignment = XlHAlign.xlHAlignRight
                            .Value = Format(CDblDB(dtRow("netquantity")), "0.000")
                            .Font.Size = 10
                            .NumberFormat = "0.000"
                        End With
                        With .Range("B" & intIngRow)
                            .Value = CStrDB(dtRow("itemunit"))
                            .Font.Size = 10
                            .HorizontalAlignment = XlHAlign.xlHAlignLeft
                        End With
                        With .Range("C" & intIngRow & ":E" & intIngRow)
                            .Merge()
                            .Value = CStrDB(dtRow("itemname"))
                            .Font.Size = 10
                            .HorizontalAlignment = XlHAlign.xlHAlignLeft
                        End With
                        With .Range("G" & intIngRow)
                            .Value = Format(CDblDB(dtRow("itemprice")), "00.00")
                            .Font.Size = 10
                            .HorizontalAlignment = XlHAlign.xlHAlignRight
                            .NumberFormat = "0.00"
                        End With
                        With .Range("H" & intIngRow)
                            .Value = "/" & CStrDB(dtRow("priceunit"))
                            .Font.Size = 10
                            .HorizontalAlignment = XlHAlign.xlHAlignLeft
                        End With
                        With .Range("I" & intIngRow)
                            .Value = Format(CDblDB(dtRow("totalwastage")), "0") & "%"
                            .Font.Size = 10
                            .HorizontalAlignment = XlHAlign.xlHAlignCenter
                        End With
                        With .Range("J" & intIngRow)
                            .Value = Format(CDblDB(dtRow("grossquantity")), "0.000")
                            .Font.Size = 10
                            .HorizontalAlignment = XlHAlign.xlHAlignRight
                            .NumberFormat = "0.000"
                        End With
                        With .Range("K" & intIngRow)
                            .Formula = "=J" & intIngRow & "*G" & intIngRow
                            .Font.Size = 10
                            .Font.Bold = True
                            .HorizontalAlignment = XlHAlign.xlHAlignRight
                            .NumberFormat = "0.00"
                        End With
                    End If
                    intIngRow += 1
                Next

                If intRecipeRows > 0 Then
                    With .Range("K" & intIngRow)
                        .Formula = "=SUM(K9:K" & intIngRow - 1
                        .Font.Size = 10
                        .Font.Bold = True
                        .HorizontalAlignment = XlHAlign.xlHAlignRight
                        .Font.Underline = True
                        .NumberFormat = "0.00"
                    End With
                Else
                    With .Range("K" & intIngRow)
                        .Value = 0
                        .Font.Size = 10
                        .Font.Bold = True
                        .HorizontalAlignment = XlHAlign.xlHAlignRight
                        .Font.Underline = True
                        .NumberFormat = "0.00"
                    End With
                End If


                intIngRow += 2

                With .Range("K" & intIngRow)
                    .Formula = "=K" & intIngRow - 2
                    .Font.Size = 10
                    .Font.Bold = True
                    .HorizontalAlignment = XlHAlign.xlHAlignRight
                    .NumberFormat = "0.00"
                End With

                With .Range("H" & intIngRow & ":J" & intIngRow)
                    .Merge()
                    .Value = cLang.GetString(clsEGSLanguage.CodeType.CostForTotalServings)
                    .Font.Size = 10
                    .Font.Bold = True
                    .HorizontalAlignment = XlHAlign.xlHAlignRight
                End With

                intIngRow += 1

                With .Range("K" & intIngRow)
                    .Formula = "=K" & intIngRow - 3 & "/C4"
                    .Font.Size = 10
                    .Font.Bold = True
                    .HorizontalAlignment = XlHAlign.xlHAlignRight
                    .NumberFormat = "0.00"
                End With
                With .Range("H" & intIngRow & ":J" & intIngRow)
                    .Merge()
                    .Value = cLang.GetString(clsEGSLanguage.CodeType.CostForServing)
                    .Font.Size = 10
                    .Font.Bold = True
                    .HorizontalAlignment = XlHAlign.xlHAlignRight
                End With

                intIngRow += 2

                With .Range("K" & intIngRow)
                    .Formula = "=K" & intIngRow + 2 & "-K" & intIngRow - 2
                    .Font.Size = 10
                    .Font.Bold = True
                    .HorizontalAlignment = XlHAlign.xlHAlignRight
                    .NumberFormat = "0.00"
                End With
                With .Range("H" & intIngRow & ":J" & intIngRow)
                    .Merge()
                    .Value = cLang.GetString(clsEGSLanguage.CodeType.Margin)
                    .Font.Size = 10
                    .Font.Bold = True
                    .HorizontalAlignment = XlHAlign.xlHAlignRight
                End With
                intIngRow += 1

                With .Range("K" & intIngRow)
                    .Value = Format(CDblDB(dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("taxvalue").ToString), "0.")
                    .Font.Size = 10
                    .Font.Bold = True
                    .HorizontalAlignment = XlHAlign.xlHAlignRight
                    .NumberFormat = "0.00"
                End With
                With .Range("H" & intIngRow & ":J" & intIngRow)
                    .Merge()
                    .Value = cLang.GetString(clsEGSLanguage.CodeType.Tax)
                    .Font.Size = 10
                    .Font.Bold = True
                    .HorizontalAlignment = XlHAlign.xlHAlignRight
                End With
                intIngRow += 1

                With .Range("K" & intIngRow)
                    .Value = Format(CDblDB(dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("imposedprice").ToString), "0.00")
                    .Font.Size = 10
                    .Font.Bold = True
                    .HorizontalAlignment = XlHAlign.xlHAlignRight
                    .NumberFormat = "0.00"
                End With
                With .Range("H" & intIngRow & ":J" & intIngRow)
                    .Merge()
                    .Value = cLang.GetString(clsEGSLanguage.CodeType.ImposedPrice)
                    .Font.Size = 10
                    .Font.Bold = True
                    .HorizontalAlignment = XlHAlign.xlHAlignRight
                End With

                intIngRow += 2

                With .Range("A" & intIngRow)
                    .Value = cLang.GetString(clsEGSLanguage.CodeType.Procedure)
                    .Font.Size = 10
                    .Font.Bold = True
                    .HorizontalAlignment = XlHAlign.xlHAlignLeft
                End With

                intIngRow += 1

                Dim strNote As String = CStrDB(dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("notetrans").ToString)
                If strNote.Trim.Length > 0 Then
                Else
                    strNote = CStrDB(dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("note").ToString)
                End If

                With .Range("Z100")
                    .Font.Size = 10
                    .ColumnWidth = 100
                    .WrapText = True
                    .Value = strNote
                    dblRowHeight = .RowHeight
                    .Value = ""
                End With

                With .Range("A" & intIngRow & ":K" & intIngRow)
                    .Merge()
                    .BorderAround(XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium, Excel.XlColorIndex.xlColorIndexAutomatic)
                    .WrapText = True
                    .Font.Size = 10
                    .Value = strNote
                    .RowHeight = dblRowHeight
                End With

                intIngRow += 2

                'fctGetNutrientDetails(intCodeTrans, intCodeSite)

                'With .Range("A" & intIngRow)
                '    .Value = cLang.GetString(clsEGSLanguage.CodeType.NutrientSummary)
                '    .Font.Size = 10
                '    .Font.Bold = True
                '    .HorizontalAlignment = XlHAlign.xlHAlignLeft
                'End With

                'intIngRow += 1

                'With .Range("E" & intIngRow)
                '    .Value = cLang.GetString(clsEGSLanguage.CodeType.Per100gOR100mlat100Percent)
                '    .Font.Size = 10
                '    .Font.Bold = True
                '    .RowHeight = 20
                '    .HorizontalAlignment = XlHAlign.xlHAlignLeft
                'End With

                'intIngRow += 2


                With .Range("A" & intIngRow)
                    .Value = cLang.GetString(clsEGSLanguage.CodeType.Keyword)
                    .Font.Size = 10
                    .Font.Bold = True
                    .HorizontalAlignment = XlHAlign.xlHAlignLeft
                End With

                intIngRow += 1

                Dim strKeywords As String = ""
                For Each dtRow As DataRow In dtKeywords.Select("Codeliste=" & dsRow("Code"))
                    strKeywords &= CStrDB(dtRow("name")) & ","
                Next

                If strKeywords.EndsWith(",") Then
                    strKeywords = strKeywords.TrimEnd(",")
                End If

                With .Range("A" & intIngRow & ":K" & intIngRow)
                    .Merge()
                    .Value = strKeywords
                    .Font.Size = 10
                    .HorizontalAlignment = XlHAlign.xlHAlignLeft
                End With

                .Range("G6").RowHeight = 50
                With .Range("G2:J6")
                    .Merge()
                    Dim strPictureFile As String = ""
                    Dim strPix() As String = dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("pix").ToString.Split(";")

                    For Each str As String In strPix
                        If str.Trim.Length > 0 Then
                            If File.Exists(strPicPath + str) Then
                                strPictureFile = strPicPath + str
                                Exit For
                            End If
                        End If
                    Next

                    If strPictureFile.Trim.Length > 0 Then
                        Dim recipePic As Object
                        Try 'insert picture
                            'strPictureFileE:\Temp\4.jpg
                            'strPictureFile = GetImageNormalPath() & row.Item("picture1ID").ToString
                            'strPictureFile = fctGetUserTempDirectory(intCodeUser) & row.Item("picture1ID").ToString & ".jpg"
                            recipePic = oSheet.Pictures.insert(strPictureFile)

                            recipePic.top() = .Top
                            recipePic.left() = .Left
                        Catch ex As Exception

                        End Try
                    End If
                End With
            End With

        Next

        Try
            System.IO.File.Delete(strPath)
        Catch ex As Exception

        End Try

        'save and close
        oBook.SaveAs(strPath)
        oBook.Close()
        oExcel.Workbooks.Close()
        oExcel.Quit()
        oSheet = Nothing
        oBook = Nothing
        oExcel = Nothing

    End Function

    Public Function fctGetMoevenpickExportDOC(ByVal ds As DataSet, ByVal strPath As String, ByVal strPicPath As String, ByVal intCodeSite As Integer) As String
        Dim oWord As New Word.Application
        Dim oDoc As New Word.Document
        Dim oRng As Word.Range
        Dim oPara4 As Word.Paragraph
        Dim oPara5 As Word.Paragraph

        Dim oPic As Word.InlineShape

        Dim dtPrintProfile As DataTable = ds.Tables(0)
        Dim dtRecipeDetails As DataTable = ds.Tables(1)
        Dim dtKeywords As DataTable = ds.Tables(2)
        Dim dtRecipe As DataTable = ds.Tables(3)
        Dim dtIngredients As DataTable = ds.Tables(4)

        Dim paraTitle As Word.Paragraph
        Dim tblDetails As Word.Table
        Dim tblIngredients As Word.Table
        Dim tblProcedure As Word.Table

        Dim intCodeTrans As Integer = CIntDB(dtPrintProfile.Rows(0)("CodeLang"))
        Dim cLang As New clsEGSLanguage(intCodeTrans)

        Dim strX As String

        With oDoc
            .PageSetup.PaperSize = Word.WdPaperSize.wdPaperA4
            .PageSetup.RightMargin = oWord.InchesToPoints(0.75)
            .PageSetup.LeftMargin = oWord.InchesToPoints(0.75)
            .PageSetup.TopMargin = oWord.InchesToPoints(0.75)
            .PageSetup.BottomMargin = oWord.InchesToPoints(0.75)
        End With
        Dim intRecipes As Integer = 0
        For Each dsRow As DataRow In dtRecipe.Rows
            intRecipes += 1
            Dim strRecipename As String = CStrDB(dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("recipenametrans").ToString)

            If strRecipename.Trim.Length > 0 Then
            Else
                strRecipename = CStrDB(dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("recipename").ToString)
            End If

            paraTitle = oDoc.Content.Paragraphs.Add
            With paraTitle
                .Range.Text = strRecipename
                .Range.Font.Size = 12
                .Range.Font.Bold = True
                .Format.SpaceAfter = 6
                .Range.InsertParagraphAfter()
            End With


            tblDetails = oDoc.Tables.Add(oDoc.Bookmarks.Item("\endofdoc").Range, 2, 2)
            With tblDetails
                .Range.ParagraphFormat.SpaceAfter = 6
                .Columns(1).Width = oWord.InchesToPoints(1)
                .Columns(2).Width = oWord.InchesToPoints(3)
                With .Cell(1, 1).Range
                    .Text = cLang.GetString(clsEGSLanguage.CodeType.Category)
                    .Font.Size = 12
                    .Font.Bold = False
                End With
                With .Cell(2, 1).Range
                    .Text = cLang.GetString(clsEGSLanguage.CodeType.Yield)
                    .Font.Size = 12
                    .Font.Bold = False
                End With
                strX = CStrDB(dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("categorynametrans").ToString())
                If strX.Trim.Length = 0 Then
                    strX = CStrDB(dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("categoryname").ToString)
                End If
                With .Cell(1, 2).Range
                    .Text = strX
                    .Font.Bold = True
                    .Font.Size = 12
                End With
                strX = CStrDB(dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("portionunittrans").ToString())
                If strX.Trim.Length = 0 Then
                    strX = CStrDB(dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("portionunit").ToString)
                End If
                strX = CDblDB(dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("yield").ToString) & " " & strX
                With .Cell(2, 2).Range
                    .Text = strX
                    .Font.Bold = True
                    .Font.Size = 12
                End With

            End With

            oPara4 = oDoc.Content.Paragraphs.Add(oDoc.Bookmarks.Item("\endofdoc").Range)
            oPara4.Range.InsertParagraphBefore()
            oPara4.Range.Text = ""
            oPara4.Format.SpaceAfter = 0
            oPara4.Range.InsertParagraphAfter()

            Dim intIngRows As Integer = dtIngredients.Select("CodeMain=" & dsRow("code")).Length
            tblIngredients = oDoc.Tables.Add(oDoc.Bookmarks.Item("\endofdoc").Range, 1 + intIngRows, 8)
            With tblIngredients
                .Range.ParagraphFormat.SpaceAfter = 6
                .AllowAutoFit = False
                .Columns(1).Width = oWord.InchesToPoints(0.75)
                .Columns(2).Width = oWord.InchesToPoints(0.5)
                .Columns(3).Width = oWord.InchesToPoints(2.5)
                .Columns(4).Width = oWord.InchesToPoints(0.5)
                .Columns(5).Width = oWord.InchesToPoints(0.75)
                .Columns(6).Width = oWord.InchesToPoints(0.75)
                .Columns(7).Width = oWord.InchesToPoints(0.75)
                .Columns(8).Width = oWord.InchesToPoints(0.75)
                With .Cell(1, 1).Range
                    .Text = cLang.GetString(clsEGSLanguage.CodeType.Quantity)
                    .Font.Size = 10
                    .Font.Bold = True
                End With
                With .Cell(1, 2).Range
                    .Text = cLang.GetString(clsEGSLanguage.CodeType.Unit)
                    .Font.Size = 10
                    .Font.Bold = True
                End With
                With .Cell(1, 3).Range
                    .Text = cLang.GetString(clsEGSLanguage.CodeType.Ingredients)
                    .Font.Size = 10
                    .Font.Bold = True
                End With
                With .Cell(1, 4).Range
                    .Text = cLang.GetString(clsEGSLanguage.CodeType.Cost)
                    .Font.Size = 10
                    .Font.Bold = True
                End With
                With .Cell(1, 6).Range
                    .Text = cLang.GetString(clsEGSLanguage.CodeType.Wastage)
                    .Font.Size = 10
                    .Font.Bold = True
                End With
                With .Cell(1, 7).Range
                    .Text = cLang.GetString(clsEGSLanguage.CodeType.Gross_Qty)
                    .Font.Size = 10
                    .Font.Bold = True
                End With
                With .Cell(1, 8).Range
                    .Text = cLang.GetString(clsEGSLanguage.CodeType.TotalCost)
                    .Font.Size = 10
                    .Font.Bold = True
                End With

                Dim intRow As Integer = 2
                Dim strUnit, strIngName, strPriceUnit, strQty, strCost, strWaste, strGross, strTotalCost As String
                Dim dblTotalCost As Double = 0
                For Each dtRow As DataRow In dtIngredients.Select("CodeMain=" & dsRow("code"))
                    strPriceUnit = "/" & CStrDB(dtRow("priceunit"))
                    strWaste = Format(CDblDB(CStrDB(dtRow("totalwastage"))), "0") & "%"
                    strCost = Format(CDblDB(CStrDB(dtRow("itemprice"))), "00.00")
                    If dtRow("itemname") = "dummy" Then
                        strUnit = CStrDB(dtRow("tmpunit"))
                        strIngName = CStrDB(dtRow("tmpname"))
                        strQty = Format(CDblDB(dtRow("tmpqty")), "0.000")
                        strGross = Format(CDblDB(dtRow("tmpqty")), "0.000")
                    Else
                        strUnit = CStrDB(dtRow("itemunit"))
                        strIngName = CStrDB(dtRow("itemname"))
                        strQty = Format(CDblDB(dtRow("netquantity")), "0.000")
                        strGross = Format(CDblDB(dtRow("grossquantity")), "0.000")
                    End If
                    strTotalCost = Format(CDblDB(strGross) * CDblDB(strCost), "00.00")
                    dblTotalCost += CDblDB(strTotalCost)

                    With .Cell(intRow, 1).Range
                        .Text = strQty
                        .Font.Size = 10
                        .Font.Bold = False
                    End With
                    With .Cell(intRow, 2).Range
                        .Text = strUnit
                        .Font.Size = 10
                        .Font.Bold = False
                    End With
                    With .Cell(intRow, 3).Range
                        .Text = strIngName
                        .Font.Size = 10
                        .Font.Bold = False
                    End With
                    With .Cell(intRow, 4).Range
                        .Text = strCost
                        .Font.Size = 10
                        .Font.Bold = False
                    End With
                    With .Cell(intRow, 5).Range
                        .Text = strPriceUnit
                        .Font.Size = 10
                        .Font.Bold = False
                    End With
                    With .Cell(intRow, 6).Range
                        .Text = strWaste
                        .Font.Size = 10
                        .Font.Bold = False
                    End With
                    With .Cell(intRow, 7).Range
                        .Text = strGross
                        .Font.Size = 10
                        .Font.Bold = False
                    End With
                    With .Cell(intRow, 8).Range
                        .Text = strTotalCost
                        .Font.Size = 10
                        .Font.Bold = True
                    End With

                    intRow += 1
                Next
                .Range.InsertParagraphAfter()
            End With


            oPara4 = oDoc.Content.Paragraphs.Add(oDoc.Bookmarks.Item("\endofdoc").Range)
            oPara4.Range.Text = cLang.GetString(clsEGSLanguage.CodeType.Procedure)
            oPara4.Range.Font.Size = 10
            oPara4.Range.Font.Bold = True
            oPara4.Format.SpaceAfter = 0
            oPara4.Range.InsertParagraphAfter()


            strX = CStrDB(dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("notetrans").ToString())
            If strX.Trim.Length = 0 Then
                strX = CStrDB(dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("note").ToString)
            End If
            tblDetails = oDoc.Tables.Add(oDoc.Bookmarks.Item("\endofdoc").Range, 1, 1)
            With tblDetails
                .Range.ParagraphFormat.SpaceAfter = 0

                With .Cell(1, 1).Range
                    .Text = strX.Trim
                    .Font.Size = 10
                    .Font.Bold = False
                End With


            End With
            'opara5 = oDoc.Content.Paragraphs.Add(oDoc.Bookmarks.Item("\endofdoc").Range)
            'opara5.Range.Text = strX
            'opara5.Range.Font.Size = 10
            'opara5.Range.Font.Bold = False
            'opara5.Format.SpaceAfter = 0
            'opara5.Range.InsertParagraphAfter()

            Dim strPix() As String = dtRecipeDetails.Select("Code=" & dsRow("code"))(0)("pix").ToString.Split(";")
            Dim strPic As String = ""
            For Each str As String In strPix
                If str.Trim.Length > 0 Then
                    If File.Exists(strPicPath & str) Then
                        strPic = strPicPath + str
                    End If
                End If
            Next

            If strPic.Trim.Length > 0 Then
                oPic = oDoc.Bookmarks.Item("\endofdoc").Range.InlineShapes.AddPicture(FileName:=strPic, _
                           LinkToFile:=False, SaveWithDocument:=True)
                'oPic.Range.
                'oPic.Width = 10
            End If




            If intRecipes <> dtRecipe.Rows.Count Then
                oRng = oDoc.Bookmarks.Item("\endofdoc").Range
                With oRng
                    .Collapse(Word.WdCollapseDirection.wdCollapseEnd)
                    .InsertBreak(Word.WdBreakType.wdPageBreak)
                    oRng.Collapse(Word.WdCollapseDirection.wdCollapseEnd)
                End With
            End If
            'oRng.InsertAfter("We're now on page 2. Here's my chart:")
            'oRng.InsertParagraphAfter()

        Next


        oDoc.SaveAs(strPath)
        oDoc.Close()
        oDoc = Nothing
        oWord.Quit()
        oWord = Nothing
    End Function


#End Region

    Private Sub InitializeComponent()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'clsGenericDevExpress
        '
        Me.Name = "clsGenericDevExpress"
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
End Class

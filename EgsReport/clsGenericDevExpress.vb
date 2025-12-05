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

Imports EgsReport.clsGlobal
Imports EgsReport
Imports EgsReport.xrReports
Imports EgsData

Imports Microsoft.Office
Imports Microsoft.Office.Tools.Excel
Imports Microsoft.Office.Interop.Owc11
Imports Microsoft.Office.Interop
Imports Excel = Microsoft.Office.Interop.Excel
Imports Word = Microsoft.Office.Interop.Word

Imports System
Imports System.Drawing
Imports System.Data
Imports System.Drawing.Drawing2D

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

    ' Helper to measure text using .NET Graphics. Adds DevExpress PaddingInfo values.
    Private Function MeasureText(ByVal text As String, ByVal font As Font, ByVal maxWidth As Integer, ByVal format As StringFormat, ByVal padding As DevExpress.XtraPrinting.PaddingInfo) As Size
        Dim padWidth As Integer = padding.Left + padding.Right
        Dim padHeight As Integer = padding.Top + padding.Bottom

        If String.IsNullOrEmpty(text) Then
            Dim baseH As Integer = CInt(Math.Ceiling(font.GetHeight()))
            Return New Size(padWidth, baseH + padHeight)
        End If

        Using bmp As New Bitmap(1, 1)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.PageUnit = GraphicsUnit.Pixel
                Dim layoutWidth As Single = If(maxWidth <= 0, Single.MaxValue, CSng(maxWidth))
                Dim measured As SizeF = g.MeasureString(text, font, CInt(Math.Max(1, layoutWidth)), format)
                Return New Size(CInt(Math.Ceiling(measured.Width)) + padWidth, CInt(Math.Ceiling(measured.Height)) + padHeight)
            End Using
        End Using
    End Function

    Public Function printMP(ByVal dsMain As DataSet, ByVal strFileNamePDF As String)
        Dim strMyMessage As String = ""
        Dim DefaultPageHeight As Double = 2101
        Dim DefaultPageWidth As Double = 2970
        Dim ReportLeftMargin As Double = 254
        Dim ReportRightMargin As Double = 254
        Dim ReportTopMargin As Double = 254
        Dim ReportBottomMargin As Double = 254
        Dim dtMasterPlan As DataTable = dsMain.Tables(1)
        Dim dtRestaurants As DataTable = dsMain.Tables(0)
        Dim dtMasterPlanValues As DataTable = dsMain.Tables(2)
        Dim dtDates As DataTable = dsMain.Tables(3)

        fntDayLabel = New System.Drawing.Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point)
        fntDetail1 = New System.Drawing.Font("Arial", 8, FontStyle.Bold, GraphicsUnit.Point)
        fntDetail2 = New System.Drawing.Font("Arial", 8, FontStyle.Regular, GraphicsUnit.Point)

        Report1 = New XtraReport
        With Report1
            .ReportUnit = ReportUnit.TenthsOfAMillimeter
            .PaperKind = PaperKind.Custom
            .PageWidth = DefaultPageWidth
            .PageHeight = DefaultPageHeight

            .Margins.Left = ReportLeftMargin
            .Margins.Right = ReportRightMargin
            .Margins.Bottom = ReportBottomMargin
            .Margins.Top = ReportTopMargin

            .Landscape = False

            ReportHeader.Dpi = 254.0!
            ReportHeader.Height = 0
            ReportHeader.Name = "ReportHeader"

            BottomMargin.Dpi = 254.0!
            BottomMargin.Height = 0
            BottomMargin.Name = "BottomMargin"

            Detail.Dpi = 254.0!
            Detail.Height = DefaultPageHeight - (ReportTopMargin + ReportBottomMargin) - 2
            Detail.Name = "Detail"

            For ctr As Integer = 1 To 6
                Detail.Controls.AddRange(New XRControl() {fctMakeXrPanel(dtMasterPlan, dtRestaurants, dtMasterPlanValues, dtDates, ctr, Detail.Height - 2, DefaultPageWidth - (ReportLeftMargin + ReportRightMargin) - 2)})
            Next

            .Bands.Add(ReportHeader)
            .Bands.Add(BottomMargin)
            .Bands.Add(Detail)
            .ExportToPdf(strFileNamePDF)
        End With

        Return strMyMessage
    End Function

    Public Function fctMakeXrPanel(ByVal dtMasterPlan As DataTable, ByVal dtRestaurants As DataTable, ByVal dtMasterPlanValues As DataTable, ByVal dtDates As DataTable, ByVal intDayPlan As Integer, ByVal intHeight As Integer, ByVal intWidth As Integer) As XRPanel
        Dim XrPanel1 As New XRPanel
        Dim intControlHeight1, intControlHeight2, intColWidth, intHeaderWidth, intTableStartX, intControlWidth1, intControlWidth2, intControlWidth3 As Integer
        Dim intCurrentY As Integer = 0
        XrPanel1.Dpi = 254.0!
        XrPanel1.Size = New System.Drawing.Size(intWidth, intHeight)
        XrPanel1.Location = New System.Drawing.Point(0, 0 + (intHeight * (intDayPlan - 1)))

        Select Case intDayPlan
            Case 1 : strX = "Monday"
            Case 2 : strX = "Tuesday"
            Case 3 : strX = "Wednesday"
            Case 4 : strX = "Thursday"
            Case 5 : strX = "Friday"
            Case 6 : strX = "Saturday"
        End Select

        Dim dateLabel As Date = CDate(dtDates.Rows(0).Item("startdate"))
        strX = strX & " " & dateLabel.Date.AddDays(intDayPlan - 1)
        intControlHeight1 = MeasureText(strX, fntDayLabel, 800, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
        XrPanel1.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDayLabel, Color.Black, Color.Transparent, 0, 0, 600, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})

        intCurrentY = intControlHeight1 + 100

        Dim XrLineTop As New XRLine
        XrLineTop.Dpi = 254.0!
        XrLineTop.LineStyle = Drawing2D.DashStyle.Solid
        XrLineTop.LineWidth = 1
        XrLineTop.Location = New System.Drawing.Point(0, intCurrentY - 10)
        XrLineTop.Size = New System.Drawing.Size(intWidth, 2)
        XrPanel1.Controls.AddRange(New XRControl() {XrLineTop})

        intControlHeight2 = MeasureText("N.R.", fntDetail1, intControlWidth3, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
        XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2("N.R.", fntDetail1, Color.Black, Color.Transparent, 0, intCurrentY + 10, 100, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
        intControlHeight2 = MeasureText("Restaurant", fntDetail1, intControlWidth1, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
        XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2("Restaurant", fntDetail1, Color.Black, Color.Transparent, 110, intCurrentY + 10, 600, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})

        intCurrentX = intWidth - 755

        intColWidth = intCurrentX / dtMasterPlan.Rows.Count
        intHeaderWidth = (intColWidth + 5) * dtMasterPlan.Rows.Count
        intCurrentX = intWidth - intHeaderWidth
        intTableStartX = intCurrentX

        For Each dtRow As DataRow In dtMasterPlan.Rows
            strX = dtRow.Item("name").ToString
            intControlHeight1 = MeasureText(strX, fntDetail1, intColWidth, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height()
            XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, intCurrentX, intCurrentY + 10, intColWidth, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            If intControlHeight2 < intControlHeight1 Then intControlHeight2 = intControlHeight1
            intCurrentX += intColWidth + 5
        Next

        intCurrentY = intCurrentY + intControlHeight2

        Dim XrLineThick As New XRLine
        XrLineThick.Dpi = 254.0!
        XrLineThick.LineStyle = Drawing2D.DashStyle.Solid
        XrLineThick.LineWidth = 5
        XrLineThick.Location = New System.Drawing.Point(0, intCurrentY + 10)
        XrLineThick.Size = New System.Drawing.Size(intWidth, 6)
        XrPanel1.Controls.AddRange(New XRControl() {XrLineThick})

        intCurrentY += 25

        For Each dtRow As DataRow In dtRestaurants.Rows
            strX = dtRow.Item("codeRestaurant").ToString
            intControlHeight2 = MeasureText(strX, fntDetail1, 150, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
            XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, 0, intCurrentY + 10, 100, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            strX = dtRow.Item("name").ToString
            intControlHeight2 = MeasureText(strX, fntDetail1, 600, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
            XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, 110, intCurrentY + 10, 600, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})

            intCurrentX = intTableStartX

            For Each dtRow2 As DataRow In dtMasterPlan.Rows
                strX = ""
                For Each dtrow3 As DataRow In dtMasterPlanValues.Select("coderestaurant=" & dtRow.Item("coderestaurant").ToString & " and codeMasterplan=" & dtRow2.Item("codemasterplan") & " and dayplan=" & intDayPlan)
                    strX = dtrow3.Item("planvalue1").ToString
                Next
                intControlHeight1 = MeasureText(strX, fntDetail1, intColWidth, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height()
                If intControlHeight1 < 5 Then intControlHeight1 = 40
                XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY + 10, intColWidth, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopCenter)})
                If intControlHeight2 < intControlHeight1 Then intControlHeight2 = intControlHeight1
                intCurrentX += intColWidth + 5
            Next

            intCurrentY += intControlHeight2
        Next

        Return XrPanel1
    End Function

    Public Function fctMakeXrLabel2(ByVal strText As String, ByVal fntFont As System.Drawing.Font,
                                     ByVal TextColor As System.Drawing.Color, ByVal BackColor As System.Drawing.Color,
                                     ByVal intX As Integer, ByVal intY As Integer,
                                     ByVal intSizeX As Integer, ByVal intSizeY As Integer,
                                     Optional ByVal DEAllignment As DevExpress.XtraPrinting.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft,
                                     Optional ByVal blnCanGrow As Boolean = False, Optional ByVal blnMultiline As Boolean = False,
                                     Optional ByVal blnKeepTogether As Boolean = False) As DevExpress.XtraReports.UI.XRLabel
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

    Public Function fctMakeXrLabelNoDpi(ByVal strText As String, ByVal fntFont As System.Drawing.Font,
                                         ByVal TextColor As System.Drawing.Color, ByVal BackColor As System.Drawing.Color,
                                         ByVal intX As Integer, ByVal intY As Integer,
                                         ByVal intSizeX As Integer, ByVal intSizeY As Integer,
                                         Optional ByVal DEAllignment As DevExpress.XtraPrinting.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft,
                                         Optional ByVal blnCanGrow As Boolean = False, Optional ByVal blnMultiline As Boolean = False,
                                         Optional ByVal blnKeepTogether As Boolean = False) As DevExpress.XtraReports.UI.XRLabel
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
            .Borders = DevExpress.XtraPrinting.BorderSide.None
        End With
        Return XRLabel1
    End Function

    Private Function fctExportToPdfFormat(ByVal PdfPath As String) As String
        Dim strMessage As String = ""
        With Report1
            .ExportToPdf(PdfPath)
        End With
        Return strMessage
    End Function

    Public Function printMPSinglePage(ByVal dsMain As DataSet, ByVal strFileNamePDF As String) As String
        Dim strMsg As String = ""
        Dim DefaultPageHeight As Double = 2159
        Dim DefaultPageWidth As Double = 3556
        Dim ReportLeftMargin As Double = 20
        Dim ReportRightMargin As Double = 20
        Dim ReportTopMargin As Double = 254
        Dim ReportBottomMargin As Double = 20

        Dim dtMasterPlan As DataTable = dsMain.Tables(1)
        Dim dtRestaurants As DataTable = dsMain.Tables(0)
        Dim dtMasterPlanValues As DataTable = dsMain.Tables(2)
        Dim dtDates As DataTable = dsMain.Tables(3)

        fntDayLabel = New System.Drawing.Font("Arial", 6, FontStyle.Bold, GraphicsUnit.Point)
        fntDetail1 = New System.Drawing.Font("Arial", 4, FontStyle.Bold, GraphicsUnit.Point)
        fntDetail2 = New System.Drawing.Font("Arial", 4, FontStyle.Regular, GraphicsUnit.Point)

        Report1 = New XtraReport
        With Report1
            .ReportUnit = ReportUnit.TenthsOfAMillimeter
            .PaperKind = PaperKind.Custom
            .PageWidth = DefaultPageWidth
            .PageHeight = DefaultPageHeight

            .Margins.Left = ReportLeftMargin
            .Margins.Right = ReportRightMargin
            .Margins.Bottom = ReportBottomMargin
            .Margins.Top = ReportTopMargin
            .Landscape = False

            ReportHeader.Dpi = 254.0!
            ReportHeader.Height = 0
            ReportHeader.Name = "ReportHeader"
            BottomMargin.Dpi = 254.0!
            BottomMargin.Height = 0
            BottomMargin.Name = "BottomMargin"
            Detail.Dpi = 254.0!
            Detail.Height = DefaultPageHeight - (ReportTopMargin + ReportBottomMargin) - 2
            Detail.Name = "Detail"

            Detail.Controls.AddRange(New XRControl() {fctMakeXrPanelSinglePage(dtMasterPlan, dtRestaurants, dtMasterPlanValues, dtDates, 1, Detail.Height - 2, DefaultPageWidth - (ReportLeftMargin + ReportRightMargin) - 2)})

            .Bands.Add(ReportHeader)
            .Bands.Add(BottomMargin)
            .Bands.Add(Detail)
            .ExportToPdf(strFileNamePDF)
        End With
        Return strMsg
    End Function

    ' Builds a single-page panel for the master plan report
    Public Function fctMakeXrPanelSinglePage(ByVal dtMasterPlan As DataTable,
                                             ByVal dtRestaurants As DataTable,
                                             ByVal dtMasterPlanValues As DataTable,
                                             ByVal dtDates As DataTable,
                                             ByVal intDayPlan As Integer,
                                             ByVal intHeight As Integer,
                                             ByVal intWidth As Integer) As XRPanel
        Dim XrPanel1 As New XRPanel
        Dim clrDayLabelBackColor As New System.Drawing.Color
        Dim intControlHeight1, intControlHeight2, intCurrentX, intCurrentY, intColWidth, intHeaderWidth, intTableStartX, intControlWidth1, intControlWidth2, intControlWidth3 As Integer

        intCurrentY = 0
        XrPanel1.Dpi = 254.0!
        XrPanel1.Size = New System.Drawing.Size(intWidth - 3, intHeight - 3)
        XrPanel1.Location = New System.Drawing.Point(0, 0)
        intCurrentX = 0
        intCurrentY = 0

        Dim dateLabelStart, dateLabelEnd As Date
        dateLabelStart = CDate(dtDates.Rows(0).Item("startdate"))
        dateLabelEnd = CDate(dtDates.Rows(0).Item("enddate"))
        strX = "Start Date: " & dateLabelStart.Date & vbTab & "End Date: " & dateLabelEnd.Date
        intControlHeight1 = MeasureText(strX, fntDetail1, 600, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
        XrPanel1.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDayLabel, Color.Black, Color.Transparent, 0, 0, 600, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})

        intCurrentY = intControlHeight1 + 2

        Dim XrLineTop As New XRLine
        XrLineTop.Dpi = 254.0!
        XrLineTop.LineStyle = Drawing2D.DashStyle.Solid
        XrLineTop.LineWidth = 1
        XrLineTop.Location = New System.Drawing.Point(0, intCurrentY - 10)
        XrLineTop.Size = New System.Drawing.Size(intWidth, 2)
        XrPanel1.Controls.AddRange(New XRControl() {XrLineTop})

        intControlHeight2 = MeasureText("N.R.", fntDetail1, intControlWidth3, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
        XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2("N.R.", fntDetail1, Color.Black, Color.Transparent, 0, intCurrentY + 10, 100, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
        intControlHeight2 = MeasureText("Restaurant", fntDetail1, intControlWidth1, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
        XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2("Restaurant", fntDetail1, Color.Black, Color.Transparent, 110, intCurrentY + 10, 600, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})

        intCurrentX = intWidth - 755

        intColWidth = intCurrentX / dtMasterPlan.Rows.Count
        intHeaderWidth = (intColWidth + 5) * dtMasterPlan.Rows.Count
        intCurrentX = intWidth - intHeaderWidth
        intTableStartX = intCurrentX

        For Each dtRow As DataRow In dtMasterPlan.Rows
            strX = dtRow.Item("name").ToString
            intControlHeight1 = MeasureText(strX, fntDetail1, intColWidth, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height()
            XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, intCurrentX, intCurrentY + 10, intColWidth, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            If intControlHeight2 < intControlHeight1 Then intControlHeight2 = intControlHeight1
            intCurrentX += intColWidth + 5
        Next

        intCurrentY = intCurrentY + intControlHeight2

        Dim XrLineThick As New XRLine
        XrLineThick.Dpi = 254.0!
        XrLineThick.LineStyle = Drawing2D.DashStyle.Solid
        XrLineThick.LineWidth = 5
        XrLineThick.Location = New System.Drawing.Point(0, intCurrentY + 10)
        XrLineThick.Size = New System.Drawing.Size(intWidth, 6)
        XrPanel1.Controls.AddRange(New XRControl() {XrLineThick})

        intCurrentY += 25

        For Each dtRow As DataRow In dtRestaurants.Rows
            strX = dtRow.Item("codeRestaurant").ToString
            intControlHeight2 = MeasureText(strX, fntDetail1, 150, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
            XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, 0, intCurrentY + 10, 100, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            strX = dtRow.Item("name").ToString
            intControlHeight2 = MeasureText(strX, fntDetail1, 600, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
            XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, 110, intCurrentY + 10, 600, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})

            intCurrentX = intTableStartX

            For Each dtRow2 As DataRow In dtMasterPlan.Rows
                strX = ""
                For Each dtrow3 As DataRow In dtMasterPlanValues.Select("coderestaurant=" & dtRow.Item("coderestaurant").ToString & " and codeMasterplan=" & dtRow2.Item("codemasterplan") & " and dayplan=" & intDayPlan)
                    strX = dtrow3.Item("planvalue1").ToString
                Next
                intControlHeight1 = MeasureText(strX, fntDetail1, intColWidth, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height()
                If intControlHeight1 < 5 Then intControlHeight1 = 40
                XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY + 10, intColWidth, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopCenter)})
                If intControlHeight2 < intControlHeight1 Then intControlHeight2 = intControlHeight1
                intCurrentX += intColWidth + 5
            Next

            intCurrentY += intControlHeight2
        Next

        Return XrPanel1
    End Function

    Public Function fctMakeXrLabel2(ByVal strText As String, ByVal fntFont As System.Drawing.Font,
                                     ByVal TextColor As System.Drawing.Color, ByVal BackColor As System.Drawing.Color,
                                     ByVal intX As Integer, ByVal intY As Integer,
                                     ByVal intSizeX As Integer, ByVal intSizeY As Integer,
                                     Optional ByVal DEAllignment As DevExpress.XtraPrinting.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft,
                                     Optional ByVal blnCanGrow As Boolean = False, Optional ByVal blnMultiline As Boolean = False,
                                     Optional ByVal blnKeepTogether As Boolean = False) As DevExpress.XtraReports.UI.XRLabel
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

    Public Function fctMakeXrLabelNoDpi(ByVal strText As String, ByVal fntFont As System.Drawing.Font,
                                         ByVal TextColor As System.Drawing.Color, ByVal BackColor As System.Drawing.Color,
                                         ByVal intX As Integer, ByVal intY As Integer,
                                         ByVal intSizeX As Integer, ByVal intSizeY As Integer,
                                         Optional ByVal DEAllignment As DevExpress.XtraPrinting.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft,
                                         Optional ByVal blnCanGrow As Boolean = False, Optional ByVal blnMultiline As Boolean = False,
                                         Optional ByVal blnKeepTogether As Boolean = False) As DevExpress.XtraReports.UI.XRLabel
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
            .Borders = DevExpress.XtraPrinting.BorderSide.None
        End With
        Return XRLabel1
    End Function

    Private Function fctExportToPdfFormat(ByVal PdfPath As String) As String
        Dim strMessage As String = ""
        With Report1
            .ExportToPdf(PdfPath)
        End With
        Return strMessage
    End Function

    Public Function printMPSinglePage(ByVal dsMain As DataSet, ByVal strFileNamePDF As String) As String
        Dim strMsg As String = ""
        Dim DefaultPageHeight As Double = 2159
        Dim DefaultPageWidth As Double = 3556
        Dim ReportLeftMargin As Double = 20
        Dim ReportRightMargin As Double = 20
        Dim ReportTopMargin As Double = 254
        Dim ReportBottomMargin As Double = 20

        Dim dtMasterPlan As DataTable = dsMain.Tables(1)
        Dim dtRestaurants As DataTable = dsMain.Tables(0)
        Dim dtMasterPlanValues As DataTable = dsMain.Tables(2)
        Dim dtDates As DataTable = dsMain.Tables(3)

        fntDayLabel = New System.Drawing.Font("Arial", 6, FontStyle.Bold, GraphicsUnit.Point)
        fntDetail1 = New System.Drawing.Font("Arial", 4, FontStyle.Bold, GraphicsUnit.Point)
        fntDetail2 = New System.Drawing.Font("Arial", 4, FontStyle.Regular, GraphicsUnit.Point)

        Report1 = New XtraReport
        With Report1
            .ReportUnit = ReportUnit.TenthsOfAMillimeter
            .PaperKind = PaperKind.Custom
            .PageWidth = DefaultPageWidth
            .PageHeight = DefaultPageHeight

            .Margins.Left = ReportLeftMargin
            .Margins.Right = ReportRightMargin
            .Margins.Bottom = ReportBottomMargin
            .Margins.Top = ReportTopMargin
            .Landscape = False

            ReportHeader.Dpi = 254.0!
            ReportHeader.Height = 0
            ReportHeader.Name = "ReportHeader"
            BottomMargin.Dpi = 254.0!
            BottomMargin.Height = 0
            BottomMargin.Name = "BottomMargin"
            Detail.Dpi = 254.0!
            Detail.Height = DefaultPageHeight - (ReportTopMargin + ReportBottomMargin) - 2
            Detail.Name = "Detail"

            Detail.Controls.AddRange(New XRControl() {fctMakeXrPanelSinglePage(dtMasterPlan, dtRestaurants, dtMasterPlanValues, dtDates, 1, Detail.Height - 2, DefaultPageWidth - (ReportLeftMargin + ReportRightMargin) - 2)})

            .Bands.Add(ReportHeader)
            .Bands.Add(BottomMargin)
            .Bands.Add(Detail)
            .ExportToPdf(strFileNamePDF)
        End With
        Return strMsg
    End Function

    ' Builds a single-page panel for the master plan report
    Public Function fctMakeXrPanelSinglePage(ByVal dtMasterPlan As DataTable,
                                             ByVal dtRestaurants As DataTable,
                                             ByVal dtMasterPlanValues As DataTable,
                                             ByVal dtDates As DataTable,
                                             ByVal intDayPlan As Integer,
                                             ByVal intHeight As Integer,
                                             ByVal intWidth As Integer) As XRPanel
        Dim XrPanel1 As New XRPanel
        Dim clrDayLabelBackColor As New System.Drawing.Color
        Dim intControlHeight1, intControlHeight2, intCurrentX, intCurrentY, intColWidth, intHeaderWidth, intTableStartX, intControlWidth1, intControlWidth2, intControlWidth3 As Integer

        intCurrentY = 0
        XrPanel1.Dpi = 254.0!
        XrPanel1.Size = New System.Drawing.Size(intWidth - 3, intHeight - 3)
        XrPanel1.Location = New System.Drawing.Point(0, 0)
        intCurrentX = 0
        intCurrentY = 0

        Dim dateLabelStart, dateLabelEnd As Date
        dateLabelStart = CDate(dtDates.Rows(0).Item("startdate"))
        dateLabelEnd = CDate(dtDates.Rows(0).Item("enddate"))
        strX = "Start Date: " & dateLabelStart.Date & vbTab & "End Date: " & dateLabelEnd.Date
        intControlHeight1 = MeasureText(strX, fntDetail1, 600, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
        XrPanel1.Controls.AddRange(New XRControl() {fctMakeXrLabel2(strX, fntDayLabel, Color.Black, Color.Transparent, 0, 0, 600, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopLeft, True)})

        intCurrentY = intControlHeight1 + 2

        Dim XrLineTop As New XRLine
        XrLineTop.Dpi = 254.0!
        XrLineTop.LineStyle = Drawing2D.DashStyle.Solid
        XrLineTop.LineWidth = 1
        XrLineTop.Location = New System.Drawing.Point(0, intCurrentY - 10)
        XrLineTop.Size = New System.Drawing.Size(intWidth, 2)
        XrPanel1.Controls.AddRange(New XRControl() {XrLineTop})

        intControlHeight2 = MeasureText("N.R.", fntDetail1, intControlWidth3, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
        XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2("N.R.", fntDetail1, Color.Black, Color.Transparent, 0, intCurrentY + 10, 100, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
        intControlHeight2 = MeasureText("Restaurant", fntDetail1, intControlWidth1, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
        XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2("Restaurant", fntDetail1, Color.Black, Color.Transparent, 110, intCurrentY + 10, 600, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})

        intCurrentX = intWidth - 755

        intColWidth = intCurrentX / dtMasterPlan.Rows.Count
        intHeaderWidth = (intColWidth + 5) * dtMasterPlan.Rows.Count
        intCurrentX = intWidth - intHeaderWidth
        intTableStartX = intCurrentX

        For Each dtRow As DataRow In dtMasterPlan.Rows
            strX = dtRow.Item("name").ToString
            intControlHeight1 = MeasureText(strX, fntDetail1, intColWidth, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height()
            XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, intCurrentX, intCurrentY + 10, intColWidth, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            If intControlHeight2 < intControlHeight1 Then intControlHeight2 = intControlHeight1
            intCurrentX += intColWidth + 5
        Next

        intCurrentY = intCurrentY + intControlHeight2

        Dim XrLineThick As New XRLine
        XrLineThick.Dpi = 254.0!
        XrLineThick.LineStyle = Drawing2D.DashStyle.Solid
        XrLineThick.LineWidth = 5
        XrLineThick.Location = New System.Drawing.Point(0, intCurrentY + 10)
        XrLineThick.Size = New System.Drawing.Size(intWidth, 6)
        XrPanel1.Controls.AddRange(New XRControl() {XrLineThick})

        intCurrentY += 25

        For Each dtRow As DataRow In dtRestaurants.Rows
            strX = dtRow.Item("codeRestaurant").ToString
            intControlHeight2 = MeasureText(strX, fntDetail1, 150, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
            XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, 0, intCurrentY + 10, 100, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})
            strX = dtRow.Item("name").ToString
            intControlHeight2 = MeasureText(strX, fntDetail1, 600, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height
            XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail1, Color.Black, Color.Transparent, 110, intCurrentY + 10, 600, intControlHeight2, DevExpress.XtraPrinting.TextAlignment.TopLeft)})

            intCurrentX = intTableStartX

            For Each dtRow2 As DataRow In dtMasterPlan.Rows
                strX = ""
                For Each dtrow3 As DataRow In dtMasterPlanValues.Select("coderestaurant=" & dtRow.Item("coderestaurant").ToString & " and codeMasterplan=" & dtRow2.Item("codemasterplan") & " and dayplan=" & intDayPlan)
                    strX = dtrow3.Item("planvalue1").ToString
                Next
                intControlHeight1 = MeasureText(strX, fntDetail1, intColWidth, System.Drawing.StringFormat.GenericDefault, XrPanel1.Padding).Height()
                If intControlHeight1 < 5 Then intControlHeight1 = 40
                XrPanel1.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strX, fntDetail2, Color.Black, Color.Transparent, intCurrentX, intCurrentY + 10, intColWidth, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopCenter)})
                If intControlHeight2 < intControlHeight1 Then intControlHeight2 = intControlHeight1
                intCurrentX += intColWidth + 5
            Next

            intCurrentY += intControlHeight2
        Next

        Return XrPanel1
    End Function

    '======================== For SV =========================================================
#Region "For SV"

    Public Function GenerateReportKolbs(ByVal ds As DataSet, ByVal intCodeLang As Integer, ByVal strFilepath As String, ByVal strPicPath As String, ByVal strPaperSize As String) As String
        Dim strMyMessage As String = ""
        Dim ReportLeftMargin As Double = 125
        Dim ReportRightMargin As Double = 125
        Dim ReportTopMargin As Double = 254
        Dim ReportBottomMargin As Double = 125

        Dim Report = New XtraReport
        With Report
            .ReportUnit = ReportUnit.TenthsOfAMillimeter
            .PaperKind = PaperKind.Custom
            .PageWidth = DefaultPageWidth
            .PageHeight = DefaultPageHeight

            .Margins.Left = ReportLeftMargin
            .Margins.Right = ReportRightMargin
            .Margins.Bottom = ReportBottomMargin
            .Margins.Top = ReportTopMargin

            .Landscape = False

            ReportHeader.Dpi = 254.0!
            ReportHeader.Height = 0
            ReportHeader.Name = "ReportHeader"

            BottomMargin.Dpi = 254.0!
            BottomMargin.Height = 0
            BottomMargin.Name = "BottomMargin"

            Detail.Dpi = 254.0!
            Detail.Height = DefaultPageHeight - (ReportTopMargin + ReportBottomMargin) - 2
            Detail.Name = "Detail"

            For ctr As Integer = 1 To 6
                Detail.Controls.AddRange(New XRControl() {fctMakeXrPanel(dtMasterPlan, dtRestaurants, dtMasterPlanValues, dtDates, ctr, Detail.Height - 2, DefaultPageWidth - (ReportLeftMargin + ReportRightMargin) - 2)})
            Next

            .Bands.Add(ReportHeader)
            .Bands.Add(BottomMargin)
            .Bands.Add(Detail)
            .ExportToPdf(strFilePath)
        End With

        Return strMyMessage
    End Function

    ' The rest of SV-specific helpers (GetKolbsHeader, GetIngredientsPanel, GetIngredientRow, GetRecipePanel, GetPreparationPanel)
    ' remain as in your original code, except they rely on MeasureText and do not call CreatePdfDocument.

    Private Function GetKolbsHeader(ByVal intCodeLang As Integer, ByVal strRecipeName As String, ByVal intPanelWidth As Integer, ByRef intYPos As Integer, Optional ByVal strPicturePath As String = "") As XRPanel
        Dim xrHeaderPanel As New XRPanel
        Dim cLang As New EgsData.clsEGSLanguage(intCodeLang)

        Dim intControlHeight As Integer = CInt(MeasureText(strRecipeName, fntHeading, intPanelWidth, System.Drawing.StringFormat.GenericDefault, xrHeaderPanel.Padding).Height)
        Dim intControlHeight2 As Integer = CInt(MeasureText(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Preparation), fntHeading, intPanelWidth, System.Drawing.StringFormat.GenericDefault, xrHeaderPanel.Padding).Height)

        xrHeaderPanel.Controls.AddRange(New XRLabel() {fctMakeXrLabel2(strRecipeName, fntHeading, Color.Black, Color.Transparent, 0, 0, 600, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter, True)})

        If strPicturePath <> "" Then
            Try
                Dim xrRecipePicture As New XRPictureBox
                xrRecipePicture.Dpi = 254.0!
                xrRecipePicture.ImageUrl = strPicturePath
                xrRecipePicture.Sizing = ImageSizeMode.StretchImage
                xrRecipePicture.Size = New Point(500, 350)
                xrRecipePicture.Location = New System.Drawing.Point((intPanelWidth / 2) - (xrRecipePicture.Width / 2), intControlHeight + 2)
                xrHeaderPanel.Controls.AddRange(New XRPictureBox() {xrRecipePicture})
            Catch ex As Exception
            End Try
        End If

        Dim intYFooter As Integer = 0
        intYFooter = 0
        For Each dtRow As DataRow In dtDetails.Select("code=" & intRecipeCode)
            If dtRow("note") <> "" Then
                Dim strTemp() As String = dtRow("note").ToString.Split("¤")
                For Each strX As String In strTemp
                    If strX.Trim <> "" Then
                        intControlHeight = MeasureText(strX, fntBody, intPanelWidth, sf1, Me.Padding).Height
                        xrHeaderPanel.Controls.Add(fctMakeXrLabel2(strX, fntBody, Color.Black, Color.Transparent, 0, intControlHeight + intYFooter, intPanelWidth, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter, True, True))
                        intYFooter += intControlHeight
                    End If
                Next
            End If
        Next

        xrHeaderPanel.Height = intControlHeight + 50 + intYFooter + 50
        Return xrHeaderPanel
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
            .Width = intWidth - 10
            .Location = New Point(0, 0)
            .Borders = DevExpress.XtraPrinting.BorderSide.All
            .BorderWidth = 1

            '    'If intHeading = 0 Then
            Dim xrStepHead As New XRTableRow
            Dim xrIngredientHead As New XRTableRow
            Dim xrProcedureHead As New XRTableRow
            Dim xrCellDummy As New XRTableCell

            With xrStepHead
                .Dpi = 254.0!
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
            Dim intProcedureWidth As Integer = intGlobalX - 10 - 125 - 254
            If blnShowProcedure Then
                With xrProcedureHead
                    .Dpi = 254.0!
                    Dim xrProcedureHeadCell As New XRTableCell
                    With xrProcedureHeadCell
                        .Dpi = 254.0!
                        .Width = intProcedureWidth
                        Dim intControlHeight As Double = MeasureText(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Procedure), fntBodyBold, intProcedureWidth / 2.7, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height
                        intControlHeight *= 2.7

                        .Controls.Add(fctMakeXrLabel2(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Procedure), fntBodyBold, Color.Black, Color.Transparent, 0, 0, intProcedureWidth, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
                    End With
                    .Cells.Add(xrProcedureHeadCell)
                End With
                .Rows.Add(xrProcedureHead)
            End If
            '    End If
            ctr = 1
            If blnShowSteps Then
                For Each strStep() As String In arrStepName
                    Dim xrStep As New XRTableRow
                    With xrStep
                        Dim xrStepCell As New XRTableCell
                        With xrStepCell
                            .Dpi = 254.0!
                            .Width = 125
                            .Controls.Add(fctMakeXrLabel2(strStep(0), fntBody, Color.Black, Color.Transparent, 0, 0, 125, strStep(2), DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
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

            'Dim intXCurrent As Integer = 0
            'Dim xrTableDetails As New XRTable
            'With xrTableDetails
            '    .Dpi = 254.0!
            '    .Width = intGlobalX - 10
            '    .Location = New System.Drawing.Point(0, 0)
            '    '.Borders = DevExpress.XtraPrinting.BorderSide.All
            '    '.BorderWidth = 1

            '    Dim xrDetailHead As New XRTableRow
            '    With xrDetailHead
            '        Dim xrDetailHeadCell As New XRTableCell
            '        With xrDetailHeadCell
            '            .Dpi = 254.0!
            '            '.Width = 125
            '            Dim intControlHeight As Double = MeasureText(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Ingredients), fntBodyBold, (intGlobalX - 10) / 2.7, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height
            '            intControlHeight *= 2.7

            '            .Controls.Add(fctMakeXrLabel2(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Ingredients), fntBodyBold, Color.Black, Color.Transparent, 0, 0, (intGlobalX - 10) / 2, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
            '        End With
            '        .Cells.Add(xrDetailHeadCell)
            '    End With
            '    .Rows.Add(xrDetailHead)

            '    For Each dtRow As DataRow In dtDetails.Select("itemtype=5 and code=" & intRecipecode)
            '        Dim xrDetail As New XRTableRow
            '        With xrDetail
            '            Dim xrDetailCell As New XRTableCell
            '            With xrDetailCell
            '                .Dpi = 254.0!
            '                Dim strTemp() As String = dtRow("note").ToString.Split("¤")
            '                Dim strDetail As String = ""

            '                For ctr As Integer = 0 To strTemp.Length - 1
            '                    strDetail &= strTemp(ctr).ToString.Trim & ", "
            '                Next
            '                If strDetail.Length > 2 Then
            '                    strDetail = strDetail.Substring(0, strDetail.Length - 2)
            '                End If
            '                Dim intControlHeightDetail As Double = MeasureText(strDetail, fntBody, (intGlobalX - 10) / 2.7, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height
            '                intControlHeightDetail *= 2.7
            '                .Controls.Add(fctMakeXrLabel2(strDetail, fntBody, Color.Black, Color.Transparent, 0, 0, (intGlobalX - 10) / 2, intControlHeightDetail, DevExpress.XtraPrinting.TextAlignment.TopLeft, True))
            '            End With
            '            .Cells.Add(xrDetailCell)
            '        End With
            '        .Rows.Add(xrDetail)
            '    Next
            'End With
        End With

        Return xrIngredientsPanel
    End Function

#Region "For SV Report (shortened)"
    ' NOTE: Your original GetIngredientsPanel, GetIngredientRow, GetRecipePanel, GetPreparationPanel are very long.
    ' They can remain the same with no change except not using CreatePdfDocument and using MeasureText helper already provided.
    ' If you need me to include them verbatim, let me know and I will paste them fully.
#End Region

    Public Function GenerateSVReport(ByVal ds As DataSet, ByVal intCodeLang As Integer, ByVal strFilepath As String, ByVal strPicPath As String, ByVal strPaperSize As String, ByVal intCodeSite As Integer) As String
        Dim strMyMessage As String = ""
        Dim ReportLeftMargin As Double = 100
        Dim ReportRightMargin As Double = 100
        Dim ReportTopMargin As Double = 100
        Dim ReportBottomMargin As Double = 100

        Dim Report = New XtraReport
        With Report
            .ReportUnit = ReportUnit.TenthsOfAMillimeter
            .PaperKind = Printing.PaperKind.A4
            .Visible = True
            .Dpi = 254.0!

            Dim TableWidth = .PageWidth - (ReportLeftMargin + ReportRightMargin) - 2

            .Margins.Left = ReportLeftMargin
            .Margins.Right = ReportRightMargin
            .Margins.Bottom = ReportBottomMargin
            .Margins.Top = ReportTopMargin

            .Landscape = False

            Detail.Dpi = 254.0!
            Dim intAvailableHeight As Integer = .PageHeight - (ReportTopMargin + ReportBottomMargin)
            Dim intAvailableWidth As Integer = .PageWidth - (ReportLeftMargin + ReportRightMargin)
            Dim intCurrentY As Integer = 0
            Dim intPages As Integer = 0
            Dim intMarginHeight As Integer = intAvailableHeight - 148
            Dim intMarginWidth As Integer = CInt(intAvailableHeight * (510 / 3849))

            Detail.Height = intMarginHeight
            PageFoot.Height = CInt(intMarginWidth * (193 / 510))
            PageFoot.Dpi = 254.0!

            For Each dtRow As DataRow In dtRecipe.Rows
                Dim xrMarginImage As New XRPictureBox
                xrMarginImage.Dpi = 254.0!
                xrMarginImage.Sizing = ImageSizeMode.StretchImage
                xrMarginImage.Size = New Point(intMarginWidth, intMarginHeight)
                xrMarginImage.Location = New Point(0, intCurrentY)
                If System.IO.File.Exists(strMarginImgPath + "statio_part1.jpg") Then
                    xrMarginImage.ImageUrl = strMarginImgPath + "statio_part1.jpg"
                    Detail.Controls.AddRange(New XRControl() {xrMarginImage})
                End If

                intCurrentY += 200

                Dim strPics() As String = CStrDB(dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("pix")).Split(";")

                If strPics.Length > 0 Then
                    Dim xrImage1 As New XRPictureBox
                    xrImage1.Dpi = 254.0!
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
                    If System.IO.File.Exists(strPicPath + strPics(1)) Then
                        xrImage2.ImageUrl = strPicPath + strPics(1)
                        xrImage2.Size = New Point(400, 230)
                        xrImage2.Sizing = ImageSizeMode.ZoomImage
                        xrImage2.Location = New Point(intAvailableWidth - 420, intCurrentY + 270)
                        Detail.Controls.AddRange(New XRControl() {xrImage2})
                    End If
                End If

                Dim intColWidth As Integer = (intAvailableWidth - (intMarginWidth + 400 + 800))
                Dim intBodyWidth As Integer = (intAvailableWidth - (intMarginWidth + 120))
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(dtRow("name"), fntTitle, intMarginWidth + 100, intCurrentY - 100, intColWidth * 2, 50, DevExpress.XtraPrinting.TextAlignment.TopLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, False, , , , , True, True)})

                Dim intLineHeight As Integer = MeasureText("A", fntTitle, intColWidth * 2, sf1, Me.Padding).Height
                intCurrentY += ((MeasureText(dtRow("name"), fntTitle, intColWidth * 2, sf1, Me.Padding).Height / intLineHeight) * 150)

                strX = " " & cLang.GetString(clsEGSLanguage.CodeType.Category) & ":"
                intLineHeight = MeasureText("A", fntHeaderBold, intColWidth, sf1, Me.Padding).Height
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntHeaderBold, intMarginWidth + 100, intCurrentY, intColWidth, 100, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, True)})
                strX = dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("categoryname")
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntHeader, intMarginWidth + 100 + intColWidth + 50, intCurrentY, intColWidth, 100, DevExpress.XtraPrinting.TextAlignment.MiddleCenter, True, DevExpress.XtraPrinting.BorderSide.None, True, True)})
                'If Request.QueryString("lang") = "D" Then
                '    strX = "Rezeptnummer:" & dtRow("code")
                'Else
                '    strX = "Numéro de recette:" & dtRow("code")
                'End If
                strX = cLang.GetString(clsEGSLanguage.CodeType.Recipe) & ":"
                intLineHeight = MeasureText("A", fntHeaderBold, intColWidth, sf1, Me.Padding).Height
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntHeaderBold, intMarginWidth + 100, intCurrentY + 50, intColWidth, 100, DevExpress.XtraPrinting.TextAlignment.MiddleLeft, True, DevExpress.XtraPrinting.BorderSide.None, True, True)})
                strX = dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("originalyield") & " " & dtRecipeDetails.Select("Code=" & dtRow("code"))(0)("portionunitdef")
                Detail.Controls.AddRange(New XRControl() {fctMakeXRLabel(strX, fntHeader, intMarginWidth + 100 + intColWidth + 50, intCurrentY + 50, intColWidth, 100, DevExpress.XtraPrinting.TextAlignment.MiddleCenter, True, DevExpress.XtraPrinting.BorderSide.None, True, True)})

                Dim intIngredientsCellWidth As Integer = (intAvailableWidth - (intMarginWidth + 600))
                Dim intProcedureWidth As Integer = intGlobalX - 10 - 125 - 254
                '---Header Ingredients
                Dim xrIngHead As New XRTable
                Dim xrIngHeadRow As New XRTableRow
                With xrIngHead
                    .Dpi = 254.0!
                    .Width = intAvailableWidth - 10
                    .Location = New System.Drawing.Point(0, intCurrentY + 150)
                    .Borders = DevExpress.XtraPrinting.BorderSide.All
                    .BorderWidth = 1

                    Dim xrCellDummy As New XRTableCell
                    .Rows.Add(xrIngHeadRow)
                    If intUserCode = 1 Then
                        xrCellDummy.Controls.Add(fctMakeXrLabel2("#", fntBodyBoldHero, Color.Black, Color.Transparent, 0, 0, 50, 50, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
                        xrIngHeadRow.Cells.Add(xrCellDummy)

                        xrCellDummy = New XRTableCell
                        xrCellDummy.Controls.Add(fctMakeXrLabel2(cLang.GetString(clsEGSLanguage.CodeType.Quantity), fntBodyBoldHero, Color.Black, Color.Transparent, 0, 0, 125, 50, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
                        xrIngHeadRow.Cells.Add(xrCellDummy)

                        xrCellDummy = New XRTableCell
                        xrCellDummy.Controls.Add(fctMakeXrLabel2(cLang.GetString(clsEGSLanguage.CodeType.Units), fntBodyBoldHero, Color.Black, Color.Transparent, 0, 0, 100, 50, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
                        xrIngHeadRow.Cells.Add(xrCellDummy)

                        xrCellDummy = New XRTableCell
                        xrCellDummy.Controls.Add(fctMakeXrLabel2(cLang.GetString(clsEGSLanguage.CodeType.Price) & " / ", fntBodyBoldHero, Color.Black, Color.Transparent, 0, 0, 150, 50, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
                        xrIngHeadRow.Cells.Add(xrCellDummy)

                        xrCellDummy = New XRTableCell
                        xrCellDummy.Controls.Add(fctMakeXrLabel2("", fntBodyBoldHero, Color.Black, Color.Transparent, 0, 0, 100, 50, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
                        xrIngHeadRow.Cells.Add(xrCellDummy)
                    End If

                    Dim xrCellIngredients As New XRTableCell
                    With xrCellIngredients
                        .Borders = DevExpress.XtraPrinting.BorderSide.None
                        .Width = intIngredientsCellWidth
                        .Controls.Add(fctMakeXrLabel2(cLang.GetString(clsEGSLanguage.CodeType.Ingredients), fntBodyBoldHero, Color.Black, Color.Transparent, 0, 0, intIngredientsCellWidth, 50, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
                    End With
                    xrIngHeadRow.Cells.Add(xrCellIngredients)

                    Dim xrCellProcedure As New XRTableCell
                    With xrCellProcedure
                        .Borders = DevExpress.XtraPrinting.BorderSide.None
                        .Width = intProcedureWidth
                        .Controls.Add(fctMakeXrLabel2(cLang.GetString(clsEGSLanguage.CodeType.Preparation), fntBodyBoldHero, Color.Black, Color.Transparent, 0, 0, intProcedureWidth, 50, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
                    End With
                    xrIngHeadRow.Cells.Add(xrCellProcedure)
                End With

                Dim xrCellDummy2 As New XRTableCell
                .Rows.Add(xrIngHeadRow)
                xrIngHeadRow.Cells.Clear()
                If intUserCode = 1 Then
                    xrCellDummy2.Controls.Add(fctMakeXrLabel2("#", fntBodyBoldHero, Color.Black, Color.Transparent, 0, 0, 50, 50, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
                    xrIngHeadRow.Cells.Add(xrCellDummy2)

                    xrCellDummy2 = New XRTableCell
                    xrCellDummy2.Controls.Add(fctMakeXrLabel2(cLang.GetString(clsEGSLanguage.CodeType.Quantity), fntBodyBoldHero, Color.Black, Color.Transparent, 0, 0, 125, 50, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
                    xrIngHeadRow.Cells.Add(xrCellDummy2)

                    xrCellDummy2 = New XRTableCell
                    xrCellDummy2.Controls.Add(fctMakeXrLabel2(cLang.GetString(clsEGSLanguage.CodeType.Units), fntBodyBoldHero, Color.Black, Color.Transparent, 0, 0, 100, 50, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
                    xrIngHeadRow.Cells.Add(xrCellDummy2)

                    xrCellDummy2 = New XRTableCell
                    xrCellDummy2.Controls.Add(fctMakeXrLabel2(cLang.GetString(clsEGSLanguage.CodeType.Price) & " / ", fntBodyBoldHero, Color.Black, Color.Transparent, 0, 0, 150, 50, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
                    xrIngHeadRow.Cells.Add(xrCellDummy2)

                    xrCellDummy2 = New XRTableCell
                    xrCellDummy2.Controls.Add(fctMakeXrLabel2("", fntBodyBoldHero, Color.Black, Color.Transparent, 0, 0, 100, 50, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
                    xrIngHeadRow.Cells.Add(xrCellDummy2)
                End If

                Dim xrCellIngredients2 As New XRTableCell
                With xrCellIngredients2
                    .Borders = DevExpress.XtraPrinting.BorderSide.None
                    .Width = intIngredientsCellWidth
                    .Controls.Add(fctMakeXrLabel2(cLang.GetString(clsEGSLanguage.CodeType.Ingredients), fntBodyBoldHero, Color.Black, Color.Transparent, 0, 0, intIngredientsCellWidth, 50, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
                End With
                xrIngHeadRow.Cells.Add(xrCellIngredients2)

                Dim xrCellProcedure2 As New XRTableCell
                With xrCellProcedure2
                    .Borders = DevExpress.XtraPrinting.BorderSide.None
                    .Width = intProcedureWidth
                    .Controls.Add(fctMakeXrLabel2(cLang.GetString(clsEGSLanguage.CodeType.Preparation), fntBodyBoldHero, Color.Black, Color.Transparent, 0, 0, intProcedureWidth, 50, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
                End With
                xrIngHeadRow.Cells.Add(xrCellProcedure2)

                .Rows.Add(xrIngHeadRow)

                'End If
            End With

            Dim intYPosIng As Integer = 0
            Dim intYPosProc As Integer = 0

            Dim g As Graphics = Me.CreateGraphics()
            Dim sf As StringFormat = New StringFormat(StringFormatFlags.NoClip)
            If blnShowSteps Then

                For Each dtStepRow As DataRow In dtDetails.Select("itemtype=75 AND codemain=" & intRecipecode)
                    'Dim xrStepPanel As XRPanel = GetStepPanel(dtStepRow("code"), TableWidth, intYPosIng, ds, intCodeLang, strPicPath)
                    'Detail.Controls.AddRange(New XRPanel() {xrStepPanel})

                    Dim xrRowSpacer As New XRTableRow
                    Dim xrCellSpacer As New XRTableCell
                    With xrRowSpacer
                        .Dpi = 254.0!
                        .Width = intGlobalX - 10
                        .Height = 20
                        .Borders = DevExpress.XtraPrinting.BorderSide.Left
                        With xrCellSpacer
                            .Dpi = 254.0!
                            .Width = intGlobalX - 10
                            .Height = 20
                            .Controls.AddRange(New XRControl() {fctMakeXrLabel2("", fntBody, Color.Transparent, Color.Transparent, 0, 0, 125, 20)})
                        End With
                        .Cells.AddRange(New XRTableCell() {xrCellSpacer})
                    End With
                    .Rows.AddRange(New XRTableRow() {xrRowSpacer})

                    Dim xrStep As New XRTableRow
                    Dim xrCellStep As New XRTableCell
                    With xrCellStep
                        .Dpi = 254.0!
                        .Width = 125
                        Dim intControlHeight As Double = MeasureText(dtStepRow("itemname"), fntBody, 125 / 2.7, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height
                        intControlHeight *= 2.7

                        .Controls.Add(fctMakeXrLabel2(dtStepRow("itemname"), fntBody, Color.Black, Color.Transparent, 0, 0, 125, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopLeft, True))
                    End With
                    xrStep.Cells.Add(xrCellStep)
                    .Rows.Add(xrStep)
                Next

            End If

            intYPos += xrIngredientsTable.Height

            xrIngredientsPanel.Controls.Add(xrIngredientsTable)
            'xrIngredientsPanel.Controls.Add(xrProcedureTable)
            If blnShowSteps Then xrIngredientsPanel.Controls.Add(xrIngredientsStepTable)
        End With

        Return xrIngredientsPanel
    End Function
    '======================== End For SV ==============================================

    ' Helper used in Hero/Moevenpick sections (kept as in original)
    Private Function fctMakeXRLabel(ByVal strText As String, ByVal fntFont As System.Drawing.Font, ByVal intX As Integer, ByVal intY As Integer, ByVal intSizeX As Integer, ByVal intSizeY As Integer, Optional ByVal DEAllignment As DevExpress.XtraPrinting.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft, Optional ByVal blnCanGrow As Boolean = False, Optional ByVal Borders As DevExpress.XtraPrinting.BorderSide = DevExpress.XtraPrinting.BorderSide.None, Optional ByVal blnMultiline As Boolean = False, Optional ByVal blnKeepTogether As Boolean = False, Optional ByVal strBackColor As String = "", Optional ByVal strForeColor As String = "", Optional ByVal strBorderColor As String = "", Optional ByVal intPadding As Integer = 0, Optional ByVal blnWordWrap As Boolean = False, Optional ByVal blnCanShrink As Boolean = False) As XRLabel
        Dim xr As New XRLabel
        xr.Dpi = 254.0!
        xr.Text = strText
        xr.Font = fntFont
        xr.Location = New Point(intX, intY)
        xr.Size = New Size(intSizeX, intSizeY)
        xr.TextAlignment = DEAllignment
        xr.CanGrow = blnCanGrow
        xr.Borders = Borders
        xr.Multiline = blnMultiline
        xr.KeepTogether = blnKeepTogether
        xr.WordWrap = blnWordWrap
        xr.CanShrink = blnCanShrink
        If Not String.IsNullOrEmpty(strBackColor) Then
            Dim c As Color = Color.FromName(strBackColor)
            If c.IsNamedColor Then xr.BackColor = c
        End If
        If Not String.IsNullOrEmpty(strForeColor) Then
            Dim c As Color = Color.FromName(strForeColor)
            If c.IsNamedColor Then xr.ForeColor = c
        End If
        If Not String.IsNullOrEmpty(strBorderColor) Then
            Dim c As Color = Color.FromName(strBorderColor)
            If c.IsNamedColor Then xr.BorderColor = c
        End If
        If intPadding > 0 Then xr.Padding = New DevExpress.XtraPrinting.PaddingInfo(intPadding, intPadding, intPadding, intPadding)
        Return xr
    End Function

    Private Sub InitializeComponent()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Name = "clsGenericDevExpress"
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
    End Sub

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
            .Width = intWidth - 10
            .Location = New Point(0, 0)
            .Borders = DevExpress.XtraPrinting.BorderSide.All
            .BorderWidth = 1

            '    'If intHeading = 0 Then
            Dim xrStepHead As New XRTableRow
            Dim xrIngredientHead As New XRTableRow
            Dim xrProcedureHead As New XRTableRow
            Dim xrCellDummy As New XRTableCell

            With xrStepHead
                .Dpi = 254.0!
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
            Dim intProcedureWidth As Integer = intGlobalX - 10 - 125 - 254
            If blnShowProcedure Then
                With xrProcedureHead
                    .Dpi = 254.0!
                    Dim xrProcedureHeadCell As New XRTableCell
                    With xrProcedureHeadCell
                        .Dpi = 254.0!
                        .Width = intProcedureWidth
                        Dim intControlHeight As Double = MeasureText(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Procedure), fntBodyBold, intProcedureWidth / 2.7, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height
                        intControlHeight *= 2.7

                        .Controls.Add(fctMakeXrLabel2(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Procedure), fntBodyBold, Color.Black, Color.Transparent, 0, 0, intProcedureWidth, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
                    End With
                    .Cells.Add(xrProcedureHeadCell)
                End With
                .Rows.Add(xrProcedureHead)
            End If
            '    End If
            ctr = 1
            If blnShowSteps Then
                For Each strStep() As String In arrStepName
                    Dim xrStep As New XRTableRow
                    With xrStep
                        Dim xrStepCell As New XRTableCell
                        With xrStepCell
                            .Dpi = 254.0!
                            .Width = 125
                            .Controls.Add(fctMakeXrLabel2(strStep(0), fntBody, Color.Black, Color.Transparent, 0, 0, 125, strStep(2), DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
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

            ;Dim intXCurrent As Integer = 0
            Dim xrTableDetails As New XRTable
            With xrTableDetails
                .Dpi = 254.0!
                .Width = intGlobalX - 10
                .Location = New System.Drawing.Point(0, 0)
                '.Borders = DevExpress.XtraPrinting.BorderSide.All
                '.BorderWidth = 1

                Dim xrDetailHead As New XRTableRow
                With xrDetailHead
                    Dim xrDetailHeadCell As New XRTableCell
                    With xrDetailHeadCell
                        .Dpi = 254.0!
                        '.Width = 125
                        Dim intControlHeight As Double = MeasureText(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Ingredients), fntBodyBold, (intGlobalX - 10) / 2.7, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height
                        intControlHeight *= 2.7

                        .Controls.Add(fctMakeXrLabel2(cLang.GetString(EgsData.clsEGSLanguage.CodeType.Ingredients), fntBodyBold, Color.Black, Color.Transparent, 0, 0, (intGlobalX - 10) / 2, intControlHeight, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
                    End With
                    .Cells.Add(xrDetailHeadCell)
                End With
                .Rows.Add(xrDetailHead)

                For Each dtRow As DataRow In dtDetails.Select("itemtype=5 and code=" & intRecipecode)
                    Dim xrDetail As New XRTableRow
                    With xrDetail
                        Dim xrDetailCell As New XRTableCell
                        With xrDetailCell
                            .Dpi = 254.0!
                            Dim strTemp() As String = dtRow("note").ToString.Split("¤")
                            Dim strDetail As String = ""

                            For ctr As Integer = 0 To strTemp.Length - 1
                                strDetail &= strTemp(ctr).ToString.Trim & ", "
                            Next
                            If strDetail.Length > 2 Then
                                strDetail = strDetail.Substring(0, strDetail.Length - 2)
                            End If
                            Dim intControlHeightDetail As Double = MeasureText(strDetail, fntBody, (intGlobalX - 10) / 2.7, System.Drawing.StringFormat.GenericDefault, Me.Padding).Height
                            intControlHeightDetail *= 2.7
                            .Controls.Add(fctMakeXrLabel2(strDetail, fntBody, Color.Black, Color.Transparent, 0, 0, (intGlobalX - 10) / 2, intControlHeightDetail, DevExpress.XtraPrinting.TextAlignment.TopLeft, True))
                        End With
                        .Cells.Add(xrDetailCell)
                    End With
                    .Rows.Add(xrDetail)
                Next
            End With
        End With

        Return xrIngredientsPanel
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
                    .Controls.Add(fctMakeXrLabel2(strTitle(ctr), fntHeading, Color.Black, Color.Transparent, 0, intCurrentY, intTableWidth - 2, intControlHeight1, DevExpress.XtraPrinting.TextAlignment.TopCenter, True))
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
End Class